import sys
import os
import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from sklearn.preprocessing import StandardScaler
import joblib
import xgboost as xgb
import traceback

def main():
    # Komut satırı argümanlarını al
    if len(sys.argv) < 5:
        print("Hatalı argüman sayısı!")
        print("Kullanım: python model_learning.py abone_verisi_path imar_train_verisi_path mesken_results_path other_results_path [sehir]")
        sys.exit(1)
    
    # Argümanları al
       
    abone_verisi_path = sys.argv[1]
    mesken_results_path = sys.argv[2]
    other_results_path = sys.argv[3]
    sehir = sys.argv[4]
    lastYear = sys.argv[5] if len(sys.argv) >= 6 else "2023"  # Varsayılan değer olarak 2023 kullanılabilir
    print(f"İşlem başlatıldı: {sehir} için deep learning modeli")
    
    # Çıktı klasörlerini kontrol et/oluştur
    output_dir = os.path.dirname(mesken_results_path)
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)
        print(f"Çıktı klasörü oluşturuldu: {output_dir}")
    
    print(f"Abone verisi: {abone_verisi_path}")
    print(f"İmar eğitim verisi: {imar_train_verisi_path}")
    print(f"Mesken sonuç yolu: {mesken_results_path}")
    print(f"Diğer sonuç yolu: {other_results_path}")
    
        # Şehre göre eğitim veri seti seçimi
    if "izmir" in sehir.lower() or "gdz" in sehir.lower():
        imar_train_verisi_path = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), 
                                            "ml_train", "ML_TRAIN_DATA.xlsx")
        print(f"GDZ/İzmir için eğitim verisi kullanılıyor: {imar_train_verisi_path}")
    else:
        imar_train_verisi_path = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), 
                                          "ml_train", "241004_WORKING_NIZAM_v6.xlsx")
    print(f"OEDAŞ/Eskişehir için eğitim verisi kullanılıyor: {imar_train_verisi_path}")
    # Şehre özgü parametreler
    
    mesken_oranı = 0.7
        
    buyuk_sanayi_baglantı_gucu = 150
    buyuk_sanayi_tuketim = 300000
    kucuk_sanayi_baglantı_gucu = 50
    kucuk_sanayi_tuketim = 20000
        
    buyuk_ticarethane_baglantı_gucu = 400
    buyuk_ticarethane_tuketim = 200000
    kucuk_ticarethane_baglantı_gucu = 15
    kucuk_ticarethane_tuketim = 10000
 
    
    try:
        # Veriyi yükle
        print(f"Abone verisi yükleniyor: {abone_verisi_path}")
        try:
            data = pd.read_csv(abone_verisi_path, encoding="cp1254")
        except:
            try:
                data = pd.read_csv(abone_verisi_path, encoding="utf-8")
            except:
                data = pd.read_csv(abone_verisi_path, encoding="latin1")
        
        print(f"Abone veri seti yüklendi: {data.shape[0]} satır, {data.shape[1]} sütun")
        # Aggregasyonları tanımla
        tuketim_column = f"YIL_TUKETIM_{lastYear}"
        
        # Veri yapısını kontrol et
        required_columns = ['BINA_ID', 'ABONE_GRUBU', 'TESISAT_NO', 'BAGLANTI_GUCU', tuketim_column]
        for col in required_columns:
            if col not in data.columns:
                print(f"UYARI: Gerekli sütun '{col}' veri setinde bulunamadı!")
                if col == tuketim_column:
                    # Alternatif sütunları kontrol et
                    if 'TUKETIM' in data.columns:
                        print(f"'TUKETIM' sütunu '{tuketim_column}' olarak kullanılacak")
                        data[tuketim_column] = data['TUKETIM']
                    elif '2023_Tuketim' in data.columns:
                        print(f"'2023_Tuketim' sütunu '{tuketim_column}' olarak kullanılacak")
                        data[tuketim_column] = data['2023_Tuketim']
        
        

        aggregations = {
            'ABONE_Y_KOORDINAT': 'first',
            'ABONE_X_KOORDINAT': 'first',
            'BAGLANDIGI_TRAFO_KODU': 'first',
            'BINA_ID':'first',
            'ABONE_ILCE_ID': 'first',
            'BAGLANTI_GUCU': 'mean',  # Average BAGLANTI_GUCU (ORT_BAG_GUCU)
        }
        
        # Abone gruplarına göre sayı ve tüketim pivot tabloları
        print("Pivot tabloları oluşturuluyor...")
        abone_counts = pd.pivot_table(data, index='BINA_ID', columns='ABONE_GRUBU', 
                                      values='TESISAT_NO', aggfunc='count', fill_value=0).add_suffix('_count')
        abone_tuketim = pd.pivot_table(data, index='BINA_ID', columns='ABONE_GRUBU', 
                             values=tuketim_column, aggfunc='sum', fill_value=0).add_suffix('_tuketim')
        
        # Ana pivot tablosu oluştur
        pivot_table = data.groupby('BINA_ID').agg(aggregations).reset_index()
        
        # BAGLANTI_GUCU'yu ORT_BAG_GUCU olarak yeniden adlandır
        pivot_table.rename(columns={'BAGLANTI_GUCU': 'ORT_BAG_GUCU'}, inplace=True)
        
        # Sayı ve tüketim değerlerini ana tabloya birleştir
        pivot_table = pivot_table.merge(abone_counts, on='BINA_ID', how='left').merge(abone_tuketim, on='BINA_ID', how='left')
        
        # ABONE SAYISI sütununu ekle
        count_columns = ['MESKEN_count', 'SANAYI_count', 'TICARETHANE_count']
        pivot_table['MESKEN_SANAYI_TICARETHANE_SAYISI'] = pivot_table[count_columns].sum(axis=1)
        
        print(f"500'den fazla mesken içeren binalar filtreleniyor...")
        # 500'den fazla mesken içeren binaları filtrele
        pivot_table = pivot_table[pivot_table['MESKEN_count'] < 500]
        
        # BINA_TIPI sütununu ekle
        print("Bina tipleri belirleniyor...")
        def determine_bina_tipi(row):
            if row['MESKEN_SANAYI_TICARETHANE_SAYISI'] == 0:
                if row.get('TARIMSALSULAMA_count', 0) >= 1:
                    return "TARIMSAL_SULAMA"
                elif row.get('AYDINLATMA_count', 0) >= 1:
                    return "AYDINLATMA"
            elif row['MESKEN_count']/row['MESKEN_SANAYI_TICARETHANE_SAYISI'] >= mesken_oranı:
                return "MESKEN"
            elif row.get('SANAYI_count', 0) >= 1:
                if row['ORT_BAG_GUCU'] >= buyuk_sanayi_baglantı_gucu and row.get('SANAYI_tuketim', 0) >= buyuk_sanayi_tuketim:
                    return "BUYUK_SANAYI"
                elif row['ORT_BAG_GUCU'] <= kucuk_sanayi_baglantı_gucu and row.get('SANAYI_tuketim', 0) <= kucuk_sanayi_tuketim:
                    return "KUCUK_SANAYI"
                else:
                    return "ORTA_SANAYI"
            elif row.get('TICARETHANE_count', 0) >= 1:  
                if row['ORT_BAG_GUCU'] >= buyuk_ticarethane_baglantı_gucu and row.get('TICARETHANE_tuketim', 0) >= buyuk_ticarethane_tuketim:
                    return "BUYUK_TICARETHANE"
                elif row['ORT_BAG_GUCU'] <= kucuk_ticarethane_baglantı_gucu and row.get('TICARETHANE_tuketim', 0) <= kucuk_ticarethane_tuketim:
                    return "KUCUK_TICARETHANE"
                else:
                    return "ORTA_TICARETHANE"
            else:
                return "DIGER"
        
        # BINA_TIPI sütununu oluştur
        pivot_table['BINA_TIPI'] = pivot_table.apply(determine_bina_tipi, axis=1)
        
        # Verileri BINA_TIPI'na göre ayır
        mesken_data = pivot_table[pivot_table['BINA_TIPI'] == 'MESKEN'].copy()
        other_data = pivot_table[pivot_table['BINA_TIPI'] != 'MESKEN'].copy()
        
        print(f"MESKEN bina sayısı: {len(mesken_data)}")
        print(f"Diğer bina sayısı: {len(other_data)}")
        
        # Tüm abone sayısı sütununu ekle
        available_columns = ['MESKEN_count', 'SANAYI_count', 'TICARETHANE_count']
        additional_columns = ['TARIMSALSULAMA_count', 'AYDINLATMA_count', 'URETICI_count']
        count_columns = available_columns + [col for col in additional_columns if col in mesken_data.columns]
        mesken_data['ABONE SAYISI'] = mesken_data[count_columns].sum(axis=1)
        
        # IMAR_ID sütununu ekle
        mesken_data['IMAR_ID'] = 1
        
        # MESKEN_tuketim'i MESKENTUKETIM olarak yeniden adlandır
        if 'MESKEN_tuketim' in mesken_data.columns:
            mesken_data.rename(columns={'MESKEN_tuketim': 'MESKENTUKETIM'}, inplace=True)
        
        # ORT_Mesken_tuketim sütununu ekle
        if 'MESKENTUKETIM' in mesken_data.columns and 'MESKEN_count' in mesken_data.columns:
            mesken_data['ORT_Mesken_tuketim'] = mesken_data['MESKENTUKETIM'] / mesken_data['MESKEN_count']
        
        # MESKEN_count'u MESKEN SAYISI olarak yeniden adlandır
        if 'MESKEN_count' in mesken_data.columns:
            mesken_data.rename(columns={'MESKEN_count': 'MESKEN SAYISI'}, inplace=True)
        
        # Ara sonuç dosyalarını kaydet
        print("Ön işlem sonuçları kaydediliyor...")
        temp_mesken_path = os.path.join(output_dir, f"mesken_data_{sehir}_temp.csv")
        temp_other_path = os.path.join(output_dir, f"other_data_{sehir}_temp.csv")
        mesken_data.to_csv(temp_mesken_path, index=False, encoding="utf-8-sig")
        other_data.to_csv(temp_other_path, index=False, encoding="utf-8-sig")
        
        # Machine Learning kısmı
        print("Makine öğrenmesi modeli başlatılıyor...")
        
        # Eğitim verisini yükle
        print(f"Eğitim verisi yükleniyor: {imar_train_verisi_path}")
        train_data = pd.read_excel(imar_train_verisi_path, header=0, index_col=False, sheet_name=0)
        print(f"Eğitim veri seti yüklendi: {len(train_data)} satır")
        
        # Eğitim verisinde gerekli sütunların varlığını kontrol et
        required_train_columns = ['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI', 'IMAR_ID']
        for col in required_train_columns:
            if col not in train_data.columns:
                print(f"UYARI: Eğitim verisinde gerekli sütun '{col}' bulunamadı!")
        
        # Hedef değişkeni düzenle (0-indexli hale getir)
        train_data['IMAR_ID'] = train_data['IMAR_ID'] - 1
        
        # Özellik ve hedef değişkenleri ayır
        X = train_data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']]
        Y = train_data['IMAR_ID']
        
        print("Veri standardize ediliyor...")
        # Verileri standardize et
        scaler = StandardScaler()
        X_scaled = scaler.fit_transform(X)
        
        # Eğitim ve test verilerini ayır
        print("Eğitim/test veri seti ayrımı yapılıyor...")
        X_train, X_test, y_train, y_test = train_test_split(X_scaled, Y, test_size=0.2, random_state=42)
        
        # XGBoost modelini başlat ve eğit
        print("XGBoost modeli eğitiliyor...")
        xgb_model = xgb.XGBClassifier()
        xgb_model.fit(X_train, y_train)
        
        # Model performansını değerlendir
        print("Model performansı değerlendiriliyor...")
        y_pred = xgb_model.predict(X_test)
        report = classification_report(y_test, y_pred)
        print('XGBoost Classifier Performance:')
        print(report)
        
        # Modeli kaydet
        print("Model kaydediliyor...")
        model_output_path = os.path.join(output_dir, f'IMAR_ID_Forecast_Model_{sehir}.pkl')
        joblib.dump(xgb_model, model_output_path)
        print(f"Model kaydedildi: {model_output_path}")
        
        # Mesken verilerine tahminleri ekle
        print("Mesken verileri için tahminler yapılıyor...")
        X_mesken = mesken_data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']]
        X_mesken_scaled = scaler.transform(X_mesken)
        mesken_data['IMAR_ID'] = mesken_data['IMAR_ID'] - 1
        y_pred_mesken = xgb_model.predict(X_mesken_scaled)
        mesken_data['IMAR_ID_FORECAST'] = y_pred_mesken
        mesken_data['IMAR_ID_FORECAST'] = mesken_data['IMAR_ID_FORECAST'] + 1
        mesken_data['IMAR_ID'] = mesken_data['IMAR_ID'] + 1
        
        # Son sonuçları kaydet
        print("Sonuçlar kaydediliyor...")
        mesken_data.to_csv(mesken_results_path, index=False, encoding="utf-8-sig")
        other_data.to_csv(other_results_path, index=False, encoding="utf-8-sig")
        print(f"Sonuçlar başarıyla kaydedildi: {mesken_results_path}, {other_results_path}")
        
        # Özet bilgi oluştur
        building_types = pivot_table['BINA_TIPI'].value_counts()
        print("\nBina Tipi Dağılımı:")
        for btype, count in building_types.items():
            print(f"{btype}: {count}")
        
        print("\nİmar ID Tahmini Dağılımı:")
        imar_forecasts = mesken_data['IMAR_ID_FORECAST'].value_counts()
        for imar_id, count in imar_forecasts.items():
            print(f"İmar ID {int(imar_id)}: {count}")
        
        print(f"\nToplam mesken bina sayısı: {len(mesken_data)}")
        print(f"Toplam diğer bina sayısı: {len(other_data)}")
        
    except Exception as e:
        print(f"İşlem sırasında hata oluştu: {e}")
        print("Hata detayları:")
        traceback.print_exc()
        sys.exit(1)
    
    print("İşlem başarıyla tamamlandı.")

if __name__ == "__main__":
    main()