import pandas as pd
import numpy as np
import os
import sys
import json
import geopandas as gpd
from pathlib import Path
import xml.etree.ElementTree as ET
import time
import sqlite3  # Add sqlite3 impo
import mimetypes


# Get the config path from the first command-line argument
config_path = sys.argv[1]
point_load_konsolidasyonu = sys.argv[2]

user_home = os.path.expanduser('~')


# Load the config.json file
with open(config_path, 'r', encoding='utf-8') as f:
    config = json.load(f)

# Extract the necessary paths from config.json
ana_klasor_yolu = config['Ana_Klasör_Yolu']
il = config['İl']
ilce = config['İlçe']
proje_ismi = config['proje_ismi']
start_year = config['baslangıc_yılı']
end_year = config['bitis_yılı']



# ANA SLF KODU
def yuk_hesaplama():

    """Ana hesaplama fonksiyonu"""
    print("Yük hesaplama başlatılıyor...")
    time.sleep(3)

    # Define file paths
    YUKLER_DOSYASI = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/5.Yük Tahmini/girdi/horizontal_vertical_yuk.xlsx')
    ABONELER_DOSYASI = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/4.Abone Sayısı/çıktı/ABONE_SAYISI_CIKTI.xlsx')

    os.makedirs(os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/5.Yük Tahmini/çıktı'), exist_ok=True)
    CIKTI_DOSYASI = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/5.Yük Tahmini/çıktı/SONUCLAR.xlsx')
    DB_DOSYASI = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/5.Yük Tahmini/çıktı/SONUCLAR.db')

    # Abone tipleri
    ABONE_TIPLERI = ["MESKEN", "SANAYI", "TICARETHANE", "TARIMSAL_SULAMA", "AYDINLATMA"]

    # Yıllar
    YILLAR = list(range(start_year, end_year+1))

    # Check YUKLER_DOSYASI
    try:
        print(f"Reading {YUKLER_DOSYASI}...")
        yukler_df = pd.read_excel(YUKLER_DOSYASI, engine='openpyxl')
    except FileNotFoundError:
        print(f"Yatay ve dikey büyüme oranları bulunamadı!\n {YUKLER_DOSYASI} ELF sonuçları çıkartılmamış. \n\n Lütfen öncelikle ELF modülünü kullanarak ekonometrik talep tahminlerini oluşturunuz.!")
        sys.exit(1)
    except Exception as e:
        print(f"Yük verileri okunurken hata oluştu: {e}")
        sys.exit(1)

    # Check ABONELER_DOSYASI
    if not os.path.isfile(ABONELER_DOSYASI):
        print(f"Abone sayısı çıktıları bulunamadı!\n {ABONELER_DOSYASI} \n\n Lütfen öncelikle 'Abone Sayısı Tahmini Yap' butonunu kullanarak hücrelere gelecek abone sayılarını oluşturunuz.!")
        sys.exit(1)

    # Verify file type
    mime_type, _ = mimetypes.guess_type(ABONELER_DOSYASI)
    print(f"Detected MIME type for {ABONELER_DOSYASI}: {mime_type}")
    engine = 'openpyxl' if mime_type == 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' else 'xlrd' if mime_type == 'application/vnd.ms-excel' else None

    # Check available sheets
    try:
        xl = pd.ExcelFile(ABONELER_DOSYASI, engine=engine)
        available_sheets = xl.sheet_names
        print(f"Available sheets in {ABONELER_DOSYASI}: {available_sheets}")
    except Exception as e:
        print(f"Excel dosyası okunurken hata oluştu: {e}")
        sys.exit(1)

    # Excel yazıcısı oluştur
    writer = pd.ExcelWriter(CIKTI_DOSYASI, engine='xlsxwriter')
    
    # SQLite bağlantısı oluştur
    conn = sqlite3.connect(DB_DOSYASI)
    cursor = conn.cursor()
    
    # Tabloyu oluştur (eğer yoksa)
    create_table = False
    
    BütünYıllarYük = pd.DataFrame()
    # Her yıl için hesaplama yap
    for yil in YILLAR:
        print(f"{yil} yılı için hesaplama yapılıyor...")
        
        # Önceki yıl - hesaplamalar için
        onceki_yil = yil - 1
        
        # Aboneleri oku
        try:
            # Mevcut yıl aboneleri
            current_year_sheet = str(yil)
            if current_year_sheet not in available_sheets:
                print(f"UYARI: {current_year_sheet} sayfası {ABONELER_DOSYASI} dosyasında bulunamadı!")
                continue
            current_year_aboneler = pd.read_excel(ABONELER_DOSYASI, sheet_name=current_year_sheet, engine=engine)
          
            # Önceki yıl aboneleri
            previous_year_sheet = str(onceki_yil)
            if onceki_yil == start_year - 1:
                previous_year_sheet = "As-is"
            if previous_year_sheet not in available_sheets:
                print(f"UYARI: {previous_year_sheet} sayfası {ABONELER_DOSYASI} dosyasında bulunamadı!")
                continue
            previous_year_aboneler = pd.read_excel(ABONELER_DOSYASI, sheet_name=previous_year_sheet, engine=engine)
            
            # Sonuç dataframe'ini oluştur - aynı format ile
            sonuc_df = current_year_aboneler.copy()
            
            # Abone sütunlarını sıfırla - sadece yapısını kullanacağız
            for abone_tipi in ABONE_TIPLERI:
                sonuc_df[abone_tipi] = 0.0
            
            # Her abone tipi için hesaplama yap
            for abone_tipi in ABONE_TIPLERI:
                # Yük verilerinden ilgili değerleri al
                yuk_satiri = yukler_df[yukler_df["YIL"] == yil]
                onceki_yuk_satiri = yukler_df[yukler_df["YIL"] == onceki_yil]
                
                if yuk_satiri.empty or onceki_yuk_satiri.empty:
                    print(f"UYARI: {yil} veya {onceki_yil} yılı için yük verisi bulunamadı.")
                    continue
                
                # Toplam faturalanan yük ve vertical yük değerlerini al
                faturalanan_yuk_kolonu = f"{abone_tipi}_FATURALANAN"
                vertical_yuk_kolonu = f"{abone_tipi}_VERTICAL"
                
                toplam_faturalanan = yuk_satiri[faturalanan_yuk_kolonu].values[0]
                toplam_vertical = yuk_satiri[vertical_yuk_kolonu].values[0]
                
                # Reel yükü hesapla (toplam - vertical)
                reel_yuk = toplam_faturalanan - toplam_vertical
                
                # İlgili yıl için toplam abone sayısını hesapla
                toplam_abone_sayisi = current_year_aboneler[abone_tipi].sum()
                onceki_yil_toplam_abone = previous_year_aboneler[abone_tipi].sum()
                
                if toplam_abone_sayisi == 0:
                    print(f"UYARI: {yil} yılı için {abone_tipi} abone sayısı 0, hesaplama yapılamadı.")
                    continue
                
                if onceki_yil_toplam_abone == 0:
                    print(f"UYARI: {onceki_yil} yılı için {abone_tipi} abone sayısı 0, vertical yük hesaplanamadı.")
                    abone_basina_vertical_yuk = 0
                else:
                    # Abone başına vertical yük
                    abone_basina_vertical_yuk = toplam_vertical / onceki_yil_toplam_abone
                
                # Abone başına reel yük
                abone_basina_reel_yuk = reel_yuk / toplam_abone_sayisi
                
                # Her hücre için yük hesaplama
                for hucre_idx, hucre in current_year_aboneler.iterrows():
                    id = hucre["id"]
                    current_abone_sayisi = hucre[abone_tipi]
                    
                    # Önceki yıldaki aynı hücreyi bul
                    previous_hucre = previous_year_aboneler[previous_year_aboneler["id"] == id]
                    
                    if not previous_hucre.empty:
                        previous_abone_sayisi = previous_hucre[abone_tipi].values[0]
                    else:
                        previous_abone_sayisi = 0
                    
                    # Hücre reel yük hesapla
                    hucre_reel_yuk = abone_basina_reel_yuk * current_abone_sayisi
                    
                    # Hücre vertical yük hesapla
                    hucre_vertical_yuk = abone_basina_vertical_yuk * previous_abone_sayisi
                    
                    # Toplam hücre yükü
                    toplam_hucre_yuku = hucre_reel_yuk + hucre_vertical_yuk
                    
                    # Sonuçları kaydet
                    sonuc_df.loc[hucre_idx, abone_tipi] = toplam_hucre_yuku
            
            # Add new columns: TOPLAM_YÜK and Yük_Yoğunluğu
            sonuc_df['TOPLAM_YÜK'] = (sonuc_df[ABONE_TIPLERI].sum(axis=1) / 8760) * 2.5
            
            sonuc_df['Yük_Yoğunluğu'] = np.where(
                sonuc_df['Hücre İçi Yerleşim Alanı'] != 0,
                (sonuc_df['TOPLAM_YÜK'] / sonuc_df['Hücre İçi Yerleşim Alanı'])*1000,
                0
            )
            
            # Sonuç dataframe'ini Excel'e yaz
            sonuc_df.to_excel(writer, sheet_name=str(yil), index=False)
            
            # Veritabanına yazmak için 'year' sütunu ekle
            sonuc_df['year'] = yil
            
            # İlk döngüde tabloyu oluştur
            if not create_table:
                # Tabloyu oluştur
                columns = sonuc_df.columns
                # SQLite için sütun tiplerini belirle
                sql_columns = []
                for col in columns:
                    if col == 'year':
                        sql_columns.append(f'"{col}" INTEGER')
                    elif sonuc_df[col].dtype in [np.float64, np.int64]:
                        sql_columns.append(f'"{col}" REAL')
                    else:
                        sql_columns.append(f'"{col}" TEXT')
                
                create_table_sql = f"""
                CREATE TABLE IF NOT EXISTS yuk_hesaplama (
                    {', '.join(sql_columns)}
                )
                """
                cursor.execute(create_table_sql)
                conn.commit()
                create_table = True
            
            # Veriyi veritabanına ekle
            #sonuc_df.to_sql('yuk_hesaplama', conn, if_exists='append', index=False)
            BütünYıllarYük = pd.concat([BütünYıllarYük, sonuc_df], axis=1)
            
        except Exception as e:
            print(f"{yil} yılı için hesaplama sırasında hata oluştu: {e}")
            continue
    
    
    SqliteKaydet(BütünYıllarYük, ilce, DB_DOSYASI)
    # Excel dosyasını kaydet
    writer.close()
    
    # Veritabanı bağlantısını kapat
    conn.close()
    
    print(f"\n.\nHesaplamalar tamamlandı!. Sonuçlar {CIKTI_DOSYASI} dosyasına ve {DB_DOSYASI} veritabanına kaydedildi.!!!\n\n")

def SqliteKaydet(df, table_name, db_name):
    
    with sqlite3.connect(db_name) as conn:
        cursor = conn.cursor()
        cursor.execute(f"DROP TABLE IF EXISTS {table_name}")
        conn.commit()  # commit et
        df.to_sql(table_name, conn, if_exists="replace", index=False)
        cursor.close()




#--------------------------------------------- POINT LOAD TAHMİNİ --------------------------------------------------------#


YUKLER_DOSYASI = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                            f"sonuclar/SLF Sonuçları/5.Yük Tahmini/girdi/horizontal_vertical_yuk.xlsx")

ABONELER_DOSYASI = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                'sonuclar/SLF Sonuçları/4.Abone Sayısı/çıktı/ABONE_SAYISI_CIKTI.xlsx')

os.makedirs(os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/5.Yük Tahmini/çıktı'), exist_ok=True)

POINT_LOAD_DOSYASI = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                'sonuclar/SLF Sonuçları/Point Load/Girdi/point_load_poligonlar.xlsx')

# Abone tipleri
ABONE_TIPLERI = ["MESKEN", "SANAYI", "TICARETHANE", "TARIMSAL_SULAMA", "AYDINLATMA"]

# Yıllar
YILLAR = list(range(start_year, end_year+1)) 

# MW'den MWh'e dönüştürme faktörü
MW_TO_MWH_FACTOR = 8760 / 2.5  # (MW / 2.5) * 8760


def point_load_basit_hesapla():

    """Point load hesaplama - basit yüzde mantığı ile"""
    try:
        try:
            point_load_df = pd.read_excel(POINT_LOAD_DOSYASI)
        except FileNotFoundError:
            print(f"UYARI: {POINT_LOAD_DOSYASI} dosyası bulunamadı. Point load hesaplamaları yapılmayacak.")
            return
        except PermissionError:
            print(f"{POINT_LOAD_DOSYASI} dosyası Windows üzerinde açıktır. Lütfen dosyayı kapattıktan sonra kodu bir daha çalıştırınız.\nŞu an için Point Load hesaplama kodu pas geçiliyor...\n")
            return
        
        # Point load bilgileri
        # Yapı: {hücre_id: {abone_tipi: {'baslangic_yili': yıl, 'toplam_kapasite': değer}}}
        point_load_bilgileri = {}
        
        # Her tesisi kaydet
        for _, row in point_load_df.iterrows():
            hucre_id = row['id']
            abone_tipi = row['Tüketim Sınıfı'].upper()
            baslangic_yili = row['ENERJILENDIRME_YILI']
            demand_mw = row['Pik Demant (kW)']
            
            # MW'yi MWh'e dönüştür
            demand_mwh = demand_mw * MW_TO_MWH_FACTOR
            
            # Hücre kayıtlarını başlat
            if hucre_id not in point_load_bilgileri:
                point_load_bilgileri[hucre_id] = {}
            
            point_load_bilgileri[hucre_id][abone_tipi] = {
                'baslangic_yili': baslangic_yili,
                'toplam_kapasite': demand_mwh
            }
        
        return point_load_bilgileri
    
    except Exception as e:
        print(f"Point load verileri yüklenirken hata oluştu: {e}")
        return {}


def point_load_yil_hesapla(baslangic_yili, toplam_kapasite, hedef_yil):

    """Belirli bir yıl için point load yükünü hesapla"""
    if hedef_yil < baslangic_yili:
        return 0  # Tesis henüz yok
    elif hedef_yil == baslangic_yili:
        return toplam_kapasite * 0.5  # İlk yıl %50
    elif hedef_yil == baslangic_yili + 1:
        return toplam_kapasite * 0.75  # İkinci yıl %75
    else:
        return toplam_kapasite * 1.0  # Üçüncü yıl ve sonrası %100



def point_load_tahmini():
    """Ana hesaplama fonksiyonu - basit yüzde mantığı ile"""
    print("Yük hesaplama başlatılıyor...", flush=True)
    time.sleep(3)

    # Define database path
    output_dir = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/5.Yük Tahmini/çıktı')
    DB_DOSYASI = os.path.join(output_dir, 'SONUCLAR.db')

    # Create output directory
    try:
        os.makedirs(output_dir, exist_ok=True)
    except PermissionError:
        print(f"HATA: Çıktı dizinine yazma izni yok: {output_dir}. Program sonlandırılıyor.", flush=True)
        return
    except Exception as e:
        print(f"HATA: Çıktı dizini oluşturulurken hata oluştu: {e}. Program sonlandırılıyor.", flush=True)
        return

    # Delete existing database to avoid duplicates
    if os.path.exists(DB_DOSYASI):
        try:
            os.remove(DB_DOSYASI)
            print(f"Mevcut veritabanı silindi: {DB_DOSYASI}", flush=True)
        except PermissionError:
            print(f"HATA: Veritabanı dosyası silinirken izin hatası: {DB_DOSYASI}. Program sonlandırılıyor.", flush=True)
            return
        except Exception as e:
            print(f"HATA: Veritabanı dosyası silinirken hata oluştu: {e}. Program sonlandırılıyor.", flush=True)
            return

    # Point load bilgilerini al
    point_load_bilgileri = point_load_basit_hesapla()
    
    if point_load_bilgileri is None:
        print("UYARI: Point load bilgileri alınamadı (dosya erişim hatası veya dosya bulunamadı). \n...\nPoint load hesaplamaları yapılmayacak.\n...", flush=True)
        return
    elif not point_load_bilgileri:
        print("UYARI: Point load bilgileri boş. Point load hesaplamaları yapılmayacak.\n..\n..", flush=True)
        return

    print("Veriler okunuyor...", flush=True)
    time.sleep(1)

    # Yük verilerini oku
    try:
        yukler_df = pd.read_excel(YUKLER_DOSYASI)
    except FileNotFoundError:
        print(f"HATA: {YUKLER_DOSYASI} dosyası bulunamadı. Program sonlandırılıyor.", flush=True)
        return
    except PermissionError:
        print(f"HATA: {YUKLER_DOSYASI} dosyasına erişim izni yok. Program sonlandırılıyor.", flush=True)
        return
    except Exception as e:
        print(f"HATA: Yük dosyası okunurken hata oluştu: {e}. Program sonlandırılıyor.", flush=True)
        return

    # SQLite bağlantısı oluştur
    try:
        conn = sqlite3.connect(DB_DOSYASI)
        cursor = conn.cursor()
    except sqlite3.OperationalError as e:
        print(f"HATA: Veritabanı dosyasına erişim hatası: {e}. Program sonlandırılıyor.", flush=True)
        return
    except Exception as e:
        print(f"HATA: Veritabanı bağlantısı oluşturulurken hata oluştu: {e}. Program sonlandırılıyor.", flush=True)
        return

    # Tabloyu oluştur (eğer yoksa)
    create_table = False

    # Her yıl için hesaplama yap
    for yil in YILLAR:
        print(f"{yil} yılı için noktasal yükün üzerinde bulunacağı hücrelere dair abone sayıları hesaplaması güncelleniyor...", flush=True)
        
        # Önceki yıl - hesaplamalar için
        onceki_yil = yil - 1
        
        # Aboneleri oku
        try:
            # Mevcut yıl aboneleri
            current_year_sheet = str(yil)
            current_year_aboneler = pd.read_excel(ABONELER_DOSYASI, sheet_name=current_year_sheet)
            
            # Önceki yıl aboneleri
            previous_year_sheet = str(onceki_yil)
            if onceki_yil == (start_year-1):
                previous_year_sheet = "As-is" 
            
            previous_year_aboneler = pd.read_excel(ABONELER_DOSYASI, sheet_name=previous_year_sheet)
            
            # Sonuç dataframe'ini oluştur - aynı format ile
            sonuc_df = current_year_aboneler.copy()
            
            # Abone sütunlarını sıfırla ve yeni sütunlar ekle
            for abone_tipi in ABONE_TIPLERI:
                # Toplam yük sütunu (eski haliyle aynı)
                sonuc_df[abone_tipi] = 0.0
                
                # Horizontal ve Vertical yük sütunlarını ekle
                sonuc_df[f"{abone_tipi}_HORIZONTAL"] = 0.0
                sonuc_df[f"{abone_tipi}_VERTICAL"] = 0.0
                
                # Point load sütununu ekle (takip için)
                sonuc_df[f"{abone_tipi}_POINT_LOAD"] = 0.0
            
            # Yük verilerinden ilgili değerleri al
            yuk_satiri = yukler_df[yukler_df["YIL"] == yil]
            onceki_yuk_satiri = yukler_df[yukler_df["YIL"] == onceki_yil]
            
            if yuk_satiri.empty or onceki_yuk_satiri.empty:
                print(f"UYARI: {yil} veya {onceki_yil} yılı için yük verisi bulunamadı. {yil} yılı atlanıyor.", flush=True)
                continue
            
            # SANAYI ve TICARETHANE hesaplamaları için özel mantık
            for abone_tipi in ["SANAYI", "TICARETHANE"]:
                # Toplam faturalanan yük ve vertical yük değerlerini al
                faturalanan_yuk_kolonu = f"{abone_tipi}_FATURALANAN"
                vertical_yuk_kolonu = f"{abone_tipi}_VERTICAL"
                
                toplam_faturalanan = yuk_satiri[faturalanan_yuk_kolonu].values[0]
                toplam_vertical = yuk_satiri[vertical_yuk_kolonu].values[0]
                
                # Reel yükü hesapla (toplam - vertical)
                reel_yuk = toplam_faturalanan - toplam_vertical
                
                # Bu yıl için point load bilgilerini topla
                point_load_hucreleri = {}
                toplam_point_load_horizontal = 0
                
                for hucre_idx, hucre in current_year_aboneler.iterrows():
                    hucre_id = hucre["id"]
                    
                    # Point load kontrol et
                    if (hucre_id in point_load_bilgileri and 
                        abone_tipi in point_load_bilgileri[hucre_id]):
                        
                        bilgi = point_load_bilgileri[hucre_id][abone_tipi]
                        hucre_horizontal = point_load_yil_hesapla(
                            bilgi['baslangic_yili'], 
                            bilgi['toplam_kapasite'], 
                            yil
                        )
                        
                        if hucre_horizontal > 0:
                            point_load_hucreleri[hucre_id] = {
                                'horizontal': hucre_horizontal,
                                'baslangic_yili': bilgi['baslangic_yili'],
                                'toplam_kapasite': bilgi['toplam_kapasite']
                            }
                            toplam_point_load_horizontal += hucre_horizontal
                
                # Bu yıl için point load var mı kontrol et
                point_load_var = toplam_point_load_horizontal > 0
                
                # Point load için horizontal dağıtım oranını belirle
                vejetatif_oran = 0.05 if abone_tipi == "SANAYI" and point_load_var else 0.5 if abone_tipi == "TICARETHANE" and point_load_var else 1.0
                
                # Vertical için point load oranını belirle
                vertical_point_load_orani = 0.95 if abone_tipi == "SANAYI" and point_load_var else 0.5 if abone_tipi == "TICARETHANE" and point_load_var else 0.0
                
                # Vertical yük dağılımı
                point_load_vertical_yuku = toplam_vertical * vertical_point_load_orani
                normal_hucre_vertical_yuku = toplam_vertical * (1 - vertical_point_load_orani)
                
                # Vejetatif büyüme için kullanılacak reel yük
                vejetatif_reel_yuk = reel_yuk * vejetatif_oran
                
                # İlgili yıl için toplam abone sayısını hesapla (point load hariç)
                toplam_abone_sayisi = 0
                for hucre_idx, hucre in current_year_aboneler.iterrows():
                    hucre_id = hucre["id"]
                    if hucre_id not in point_load_hucreleri:  # Point load olmayan hücreler
                        toplam_abone_sayisi += hucre[abone_tipi]
                
                onceki_yil_toplam_abone = previous_year_aboneler[abone_tipi].sum()
                
                if toplam_abone_sayisi == 0 and vejetatif_reel_yuk > 0:
                    print(f"UYARI: {yil} yılı için {abone_tipi} abone sayısı 0, vejetatif hesaplama yapılamadı.", flush=True)
                    continue
                
                if onceki_yil_toplam_abone == 0:
                    print(f"UYARI: {onceki_yil} yılı için {abone_tipi} abone sayısı 0, vertical yük hesaplanamadı.", flush=True)
                    abone_basina_normal_vertical_yuk = 0
                else:
                    # Normal hücreler için abone başına vertical yük
                    abone_basina_normal_vertical_yuk = normal_hucre_vertical_yuku / onceki_yil_toplam_abone
                
                # Abone başına vejetatif reel yük (point load hariç)
                abone_basina_reel_yuk = vejetatif_reel_yuk / toplam_abone_sayisi if toplam_abone_sayisi > 0 else 0
                
                # Her hücre için yük hesaplama
                for hucre_idx, hucre in current_year_aboneler.iterrows():
                    hucre_id = hucre["id"]
                    current_abone_sayisi = hucre[abone_tipi]
                    
                    # Önceki yıldaki aynı hücreyi bul
                    previous_hucre = previous_year_aboneler[previous_year_aboneler["id"] == hucre_id]
                    
                    if not previous_hucre.empty:
                        previous_abone_sayisi = previous_hucre[abone_tipi].values[0]
                    else:
                        previous_abone_sayisi = 0
                    
                    # Point load hücresi mi kontrol et
                    if hucre_id in point_load_hucreleri:
                        # Point load hücresi
                        hucre_bilgileri = point_load_hucreleri[hucre_id]
                        hucre_horizontal = hucre_bilgileri['horizontal']
                        baslangic_yili = hucre_bilgileri['baslangic_yili']
                        
                        # Vertical hesaplama - 3 yıl sonra başlar
                        if yil >= baslangic_yili + 3:
                            # 3 yıl geçti - vertical hesaplama başlar
                            if toplam_point_load_horizontal > 0:
                                hucre_vertical = (hucre_horizontal / toplam_point_load_horizontal) * point_load_vertical_yuku
                            else:
                                hucre_vertical = 0
                        else:
                            # Henüz 3 yıl geçmedi - vertical yok
                            hucre_vertical = 0
                        
                        hucre_toplam = hucre_horizontal + hucre_vertical
                        
                        # Sonuçları kaydet
                        sonuc_df.loc[hucre_idx, abone_tipi] = hucre_toplam
                        sonuc_df.loc[hucre_idx, f"{abone_tipi}_HORIZONTAL"] = hucre_horizontal
                        sonuc_df.loc[hucre_idx, f"{abone_tipi}_VERTICAL"] = hucre_vertical
                        sonuc_df.loc[hucre_idx, f"{abone_tipi}_POINT_LOAD"] = hucre_toplam  # Ana sütunla aynı
                        
                    else:
                        # Normal vejetatif hücre
                        # Hücre reel yük hesapla (Horizontal)
                        hucre_reel_yuk = abone_basina_reel_yuk * current_abone_sayisi
                        
                        # Hücre vertical yük hesapla (normal hesaplama)
                        hucre_vertical_yuk = abone_basina_normal_vertical_yuk * previous_abone_sayisi
                        
                        # Toplam hücre yükü
                        toplam_hucre_yuku = hucre_reel_yuk + hucre_vertical_yuk
                        
                        # Sonuçları kaydet
                        sonuc_df.loc[hucre_idx, abone_tipi] = toplam_hucre_yuku  # Toplam yük
                        sonuc_df.loc[hucre_idx, f"{abone_tipi}_HORIZONTAL"] = hucre_reel_yuk  # Horizontal yük
                        sonuc_df.loc[hucre_idx, f"{abone_tipi}_VERTICAL"] = hucre_vertical_yuk  # Vertical yük
                        sonuc_df.loc[hucre_idx, f"{abone_tipi}_POINT_LOAD"] = 0  # Point load yok
            
            # Diğer abone tipleri (MESKEN, TARIMSAL_SULAMA, AYDINLATMA) için normal hesaplama
            for abone_tipi in ["MESKEN", "TARIMSAL_SULAMA", "AYDINLATMA"]:
                # Toplam faturalanan yük ve vertical yük değerlerini al
                faturalanan_yuk_kolonu = f"{abone_tipi}_FATURALANAN"
                vertical_yuk_kolonu = f"{abone_tipi}_VERTICAL"
                
                toplam_faturalanan = yuk_satiri[faturalanan_yuk_kolonu].values[0]
                toplam_vertical = yuk_satiri[vertical_yuk_kolonu].values[0]
                
                # Reel yükü hesapla (toplam - vertical)
                reel_yuk = toplam_faturalanan - toplam_vertical
                
                # İlgili yıl için toplam abone sayısını hesapla
                toplam_abone_sayisi = current_year_aboneler[abone_tipi].sum()
                onceki_yil_toplam_abone = previous_year_aboneler[abone_tipi].sum()
                
                if toplam_abone_sayisi == 0:
                    print(f"UYARI: {yil} yılı için {abone_tipi} abone sayısı 0, hesaplama yapılamadı.", flush=True)
                    continue  # Skip this abone_tipi, not the year
                
                if onceki_yil_toplam_abone == 0:
                    print(f"UYARI: {onceki_yil} yılı için {abone_tipi} abone sayısı 0, vertical yük hesaplanamadı.", flush=True)
                    abone_basina_vertical_yuk = 0
                else:
                    # Abone başına vertical yük
                    abone_basina_vertical_yuk = toplam_vertical / onceki_yil_toplam_abone
                
                # Abone başına reel yük
                abone_basina_reel_yuk = reel_yuk / toplam_abone_sayisi
                
                # Her hücre için yük hesaplama
                for hucre_idx, hucre in current_year_aboneler.iterrows():
                    hucre_id = hucre["id"]
                    current_abone_sayisi = hucre[abone_tipi]
                    
                    # Önceki yıldaki aynı hücreyi bul
                    previous_hucre = previous_year_aboneler[previous_year_aboneler["id"] == hucre_id]
                    
                    if not previous_hucre.empty:
                        previous_abone_sayisi = previous_hucre[abone_tipi].values[0]
                    else:
                        previous_abone_sayisi = 0
                    
                    # Hücre reel yük hesapla (Horizontal)
                    hucre_reel_yuk = abone_basina_reel_yuk * current_abone_sayisi
                    
                    # Hücre vertical yük hesapla
                    hucre_vertical_yuk = abone_basina_vertical_yuk * previous_abone_sayisi
                    
                    # Toplam hücre yükü
                    toplam_hucre_yuku = hucre_reel_yuk + hucre_vertical_yuk
                    
                    # Sonuçları kaydet
                    sonuc_df.loc[hucre_idx, abone_tipi] = toplam_hucre_yuku  # Toplam yük
                    sonuc_df.loc[hucre_idx, f"{abone_tipi}_HORIZONTAL"] = hucre_reel_yuk  # Horizontal yük
                    sonuc_df.loc[hucre_idx, f"{abone_tipi}_VERTICAL"] = hucre_vertical_yuk  # Vertical yük
                    sonuc_df.loc[hucre_idx, f"{abone_tipi}_POINT_LOAD"] = 0  # Point load yok
            
            # Add year column
            sonuc_df['year'] = yil
            
            # Create table if not exists
            if not create_table:
                columns = sonuc_df.columns
                sql_columns = []
                for col in columns:
                    if col == 'year':
                        sql_columns.append(f'"{col}" INTEGER')
                    elif sonuc_df[col].dtype in [np.float64, np.int64]:
                        sql_columns.append(f'"{col}" REAL')
                    else:
                        sql_columns.append(f'"{col}" TEXT')
                
                create_table_sql = f"""
                CREATE TABLE IF NOT EXISTS SONUCLAR (
                    {', '.join(sql_columns)}
                )
                """
                try:
                    cursor.execute(create_table_sql)
                    conn.commit()
                    create_table = True
                except sqlite3.OperationalError as e:
                    print(f"HATA: Veritabanı tablosu oluşturulurken hata oluştu: {e}. Program sonlandırılıyor.", flush=True)
                    conn.close()
                    return
                except Exception as e:
                    print(f"HATA: Veritabanı tablosu oluşturulurken hata oluştu: {e}. Program sonlandırılıyor.", flush=True)
                    conn.close()
                    return
            
            # Write to database
            try:
                sonuc_df.to_sql('SONUCLAR', conn, if_exists='append', index=False)
                print(f"{yil} yılı veritabanına başarıyla kaydedildi.", flush=True)
            except sqlite3.OperationalError as e:
                print(f"HATA: {yil} yılı için veritabanına veri yazılırken hata oluştu: {e}. {yil} yılı atlanıyor.", flush=True)
                continue
            except Exception as e:
                print(f"HATA: {yil} yılı için veritabanına veri yazılırken hata oluştu: {e}. {yil} yılı atlanıyor.", flush=True)
                continue
        
        except Exception as e:
            print(f"Hata: {yil} yılı için hesaplama sırasında hata oluştu: {e}. {yil} yılı atlanıyor.", flush=True)
            continue
    
    # Veritabanı bağlantısını kapat
    try:
        conn.close()
        print(f"Hesaplamalar tamamlandı!!.\n Sonuçlar {DB_DOSYASI} veritabanına kaydedildi.\n...", flush=True)
    except Exception as e:
        print(f"HATA: Veritabanı bağlantısı kapatılırken hata oluştu: {e}.", flush=True)




# ------------------------------------------------------ YÜK HARİTASI SONUÇLARI HESAPLA------------------------------------------------------------------------------------------#





def excel_to_single_kml(excel_file_path):
    
    # Define the output directory for the KML file
    kml_output_dir = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/5.Yük Tahmini/çıktı')

    if not os.path.exists(kml_output_dir):
        os.makedirs(kml_output_dir)

    print("Yük tahmini sonuçlarından poligonlar oluşturuluyor...")

    try:
        # Read the Excel file
        excel_file = pd.ExcelFile(excel_file_path)
        
        # Get all sheet names
        sheet_names = excel_file.sheet_names
        
        if not sheet_names:
            print("No sheets found in the Excel file.")
            return
        
        # Define the columns that are identical across sheets
        common_columns = ['id', 'left', 'top', 'right', 'bottom', 'geometry']
        
        # Define the columns that will be prefixed with sheet names
        variable_columns = ['MESKEN', 'SANAYI', 'TICARETHANE', 'TARIMSAL_SULAMA', 'AYDINLATMA', 'TOPLAM_YÜK', 'Hücre İçi Yerleşim Alanı', 'Yük_Yoğunluğu']
        
        # Define data types and rounding precision
        coordinate_columns = ['left', 'top', 'right', 'bottom']  # Round to 6 decimal places
        
        # Initialize a dictionary to store data
        combined_data = {}
        
        # Process each sheet
        for sheet_name in sheet_names:

            # Read the sheet into a DataFrame
            df = pd.read_excel(excel_file, sheet_name=sheet_name)

            # Create the 'geometry' column using the Excel formula equivalent
            df['geometry'] = df.apply(
                lambda row: f"POLYGON(({row['left']} {row['top']}, {row['right']} {row['top']}, {row['right']} {row['bottom']}, {row['left']} {row['bottom']}, {row['left']} {row['top']}))",
                axis=1
            )
            
            # On the first sheet, store the common columns
            if not combined_data:
                for col in common_columns:
                    combined_data[col] = df[col]
            
            # Add variable columns with sheet name prefix
            for col in variable_columns:
                new_col_name = f"{col}_{sheet_name}"
                # Ensure the column is treated as float64 to preserve decimals
                combined_data[new_col_name] = df[col].astype('float64')
        
        # Create a DataFrame from the combined data
        final_df = pd.DataFrame(combined_data)
        
        # Ensure correct data types and apply rounding
        for col in coordinate_columns:
            if col in final_df.columns:
                final_df[col] = final_df[col].astype('float64')
                final_df[col] = final_df[col].round(6)
        
        # Round other float columns to 3 decimal places
        for col in variable_columns:
            for sheet_name in sheet_names:
                col_name = f"{col}_{sheet_name}"
                if col_name in final_df.columns:
                    final_df[col_name] = final_df[col_name].astype('float64')
                    final_df[col_name] = final_df[col_name].round(3)
        
        
        # Convert to GeoDataFrame using WKT geometry
        gdf = gpd.GeoDataFrame(
            final_df,
            geometry=gpd.GeoSeries.from_wkt(final_df['geometry'])
        )
        
        # Set CRS to WGS84 (EPSG:4326), using the older method for compatibility
        gdf.crs = "EPSG:4326"
        
        # Convert string columns to UTF-8 for KML compatibility
        for col in gdf.columns:
            if gdf[col].dtype == 'object' and col != 'geometry':
                gdf[col] = gdf[col].apply(
                    lambda x: x.encode('cp1254', errors='replace').decode('utf-8', errors='replace') 
                    if pd.notna(x) else x
                )

        print("Sonuçlar .kml dosyasına yazılıyor...")
        
        # Sort columns for consistent output
        sorted_columns = common_columns.copy()
        for sheet_name in sheet_names:
            for var_col in variable_columns:
                sorted_columns.append(f"{var_col}_{sheet_name}")
        # Reorder the GeoDataFrame columns (excluding 'geometry' which is handled separately)
        sorted_columns = [col for col in sorted_columns if col != 'geometry'] + ['geometry']
        gdf = gdf[sorted_columns]
        
        # Define the output KML file path
        excel_filename = Path(excel_file_path).stem
        kml_filename = f"{excel_filename}_Yük_Yoğunluğu.kml"
        kml_output_path = os.path.join(kml_output_dir, kml_filename)
        
        # Manually create the KML file
        # Define the KML namespace
        kml_ns = "http://www.opengis.net/kml/2.2"
        # Register the namespace with an empty prefix (default namespace)
        ET.register_namespace('', kml_ns)
        
        # Create the root KML element with the correct namespace
        kml_root = ET.Element('kml', xmlns=kml_ns)
        
        # Create the Document element
        document = ET.SubElement(kml_root, 'Document')
        document_id = ET.SubElement(document, 'id')
        document_id.text = 'root_doc'
        
        # Create the Schema element
        schema = ET.SubElement(document, 'Schema', name=f"{excel_filename}_combined", id=f"{excel_filename}_combined")

        # Add SimpleField for each column (except geometry)
        for col in gdf.columns:
            if col != 'geometry':
                ET.SubElement(schema, 'SimpleField', name=col, type='float')
        
        # Create the Folder element
        folder = ET.SubElement(document, 'Folder')
        folder_name = ET.SubElement(folder, 'name')
        folder_name.text = f"{excel_filename}_combined"
        
        # Add a Placemark for each row in the GeoDataFrame
        for idx, row in gdf.iterrows():
            placemark = ET.SubElement(folder, 'Placemark')
            
            # Set the <name> field (using the id value)
            name = ET.SubElement(placemark, 'name')
            name.text = str(row['id'])
            
            # Set the <description> field (using id - left)
            description = ET.SubElement(placemark, 'description')
            # 'left' is already rounded to 6 decimal places in the DataFrame
            description.text = f"{row['id']} - {row['left']}"
            
            # Add the Style element (same as in the original KML)
            style = ET.SubElement(placemark, 'Style')
            line_style = ET.SubElement(style, 'LineStyle')
            color = ET.SubElement(line_style, 'color')
            color.text = 'ff0000ff'
            poly_style = ET.SubElement(style, 'PolyStyle')
            fill = ET.SubElement(poly_style, 'fill')
            fill.text = '0'
            
            # Add the ExtendedData/SchemaData section
            extended_data = ET.SubElement(placemark, 'ExtendedData')
            schema_data = ET.SubElement(extended_data, 'SchemaData', schemaUrl=f"#{excel_filename}_combined")
            # Add SimpleData for each column (except geometry)
            for col in gdf.columns:
                if col != 'geometry':
                    simple_data = ET.SubElement(schema_data, 'SimpleData', name=col)
                    simple_data.text = str(row[col])
            
            # Add the Polygon geometry
            polygon = ET.SubElement(placemark, 'Polygon')
            outer_boundary = ET.SubElement(polygon, 'outerBoundaryIs')
            linear_ring = ET.SubElement(outer_boundary, 'LinearRing')
            coordinates = ET.SubElement(linear_ring, 'coordinates')
            # Convert the geometry to coordinates string, rounding coordinates to 6 decimal places
            geom = row['geometry']
            coords = []
            for x, y in geom.exterior.coords:  # Include all points, including the closing point
                x_rounded = round(x, 6)
                y_rounded = round(y, 6)
                coords.append(f"{x_rounded},{y_rounded}")
            coordinates.text = ' '.join(coords)
        
        # Write the KML file
        tree = ET.ElementTree(kml_root)
        tree.write(kml_output_path, encoding='utf-8', xml_declaration=True)

        print("Sonuçlar başarıyla oluşturuldu ve 'SONUÇLAR_Yük_Yoğunluğu.kml' dosyasında yazdırıldı.!!")
        time.sleep(3)


        
    except Exception as e:
        print(f"An error occurred: {str(e)}")


if __name__ == "__main__":
    
    if point_load_konsolidasyonu == str(True):
        point_load_tahmini()

    yuk_hesaplama()

    if point_load_konsolidasyonu == str(False):
        point_load_tahmini()

    excel_file = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/5.Yük Tahmini/çıktı/SONUCLAR.xlsx')
    excel_to_single_kml(excel_file)

