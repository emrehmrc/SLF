import os
import sys
import pandas as pd
import geopandas as gpd
import numpy as np
import tkinter as tk
from tkinter import ttk, filedialog, messagebox
from shapely.geometry import Point, Polygon
from shapely import wkt
from sqlalchemy import create_engine

# Modül importları
from imar_overpass import fetch_and_process_data
from imar_check import process_to_geodataframe
import xml.etree.ElementTree as ET
import re

# Global değişkenler
global_data = None
selected_region = None
kml_file = None

# Veritabanı bağlantı ayarları
DB_CONNECTIONS = {
    "gdz": "postgresql://postgres:12345@localhost:5432/gdz",  # İzmir için
    "oedas": "postgresql://postgres:12345@localhost:5432/oedas"  # Eskişehir için
}

# Tablo adları
TABLE_NAMES = {
    "İzmir": "IZMIR_IMAR_PL",
    "Eskişehir": "ESKISEHIR_IMAR_PL"
}

# İmar kategorileri için renk sözlüğü
COLOR_DICT = {
    "Ticarethane": "#FF6347",  # Tomato
    "Yasakli Alan": "#4682B4",  # SteelBlue
    "Mesken": "#32CD32",        # LimeGreen
    "Sanayi": "#FFD700",        # Gold
    "Kentsel Dönüşüm": "#8A2BE2",  # BlueViolet
    "Tarimsal Sulama": "#DAA520",  # GoldenRod
    "Kamu Hizmeti": "#FF4500",  # OrangeRed
    "Kamu Tesisi": "#7FFFD4",   # Aquamarine
}

# Anahtar kelime eşleştirme tablosu (katmanAyirma fonksiyonundan)
MATCHING_TABLE = [
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Belediye Hizmet", "Anahtar Kelimeler": r"BEL|BLD|BHZ|BHA|HIZMET|IHA", "Katman Adi": "PL_BEL_HIZ"},
    # Diğer eşleştirmeler için yer tutucu - tam tabloyu ekleyin
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Spormerkezi", "Anahtar Kelimeler": r"sports_centre", "Katman Adi": "leisure"}
]

def bölge_seçim_arayüzü():
    """Bölge seçimi için GUI oluşturur"""
    root = tk.Tk()
    root.title("Bölge Seçimi")
    root.geometry("300x150")
    
    label = ttk.Label(root, text="Lütfen çalışmak istediğiniz bölgeyi seçin:")
    label.pack(pady=10)
    
    region_combo = ttk.Combobox(root, values=["Eskişehir", "İzmir", "Dosya Seç"])
    region_combo.pack(pady=10)
    
    button = ttk.Button(root, text="Seçimi Onayla", command=lambda: handle_selection(root, region_combo))
    button.pack(pady=10)
    
    root.mainloop()
    return selected_region, global_data

def handle_selection(root, region_combo):
    """Bölge seçimini işler"""
    global global_data
    global selected_region
    selected_region = region_combo.get()
    
    print(f"Seçilen bölge: {selected_region}")
    
    if selected_region in ["Eskişehir", "İzmir"]:
        root.destroy()
        print(f"Bölge seçildi: {selected_region}")
        global_data = fetch_and_process_data(selected_region)
    
    elif selected_region == "Dosya Seç":
        root.destroy()
        print("Dosya seçiliyor...")
        file_path = filedialog.askopenfilename(title="Bir CSV dosyası seçin", filetypes=[("CSV files", "*.csv")])
        
        if file_path:
            if "İzmir" in file_path or "izmir" in file_path:
                selected_region = "İzmir"
            elif "Eskişehir" in file_path or "eskisehir" in file_path:
                selected_region = "Eskişehir"
            else:
                selected_region = "Bilinmeyen Bölge"
                
            print(f"Dosya seçildi: {file_path}, Bölge: {selected_region}")
            
            df = pd.read_csv(file_path)
            df['geometry'] = df['geometry'].apply(lambda x: wkt.loads(x) if isinstance(x, str) else np.nan)
            dfd = gpd.GeoDataFrame(df, geometry='geometry')
            global_data = dfd
        else:
            print("Dosya seçimi iptal edildi. Bölge belirlenemedi.")
            selected_region = None

def kml_seçim_arayüzü():
    """KML dosyası seçimi için GUI oluşturur"""
    root = tk.Tk()
    root.title("Dosya Seçimi")
    root.geometry("400x200")
    
    label = tk.Label(root, text="KML Dosyası Yüklemek İçin Butona Tıklayın")
    label.pack(pady=20)
    
    button = tk.Button(root, text="KML Dosyası Seç", command=lambda: open_file_dialog(root))
    button.pack(pady=10)
    
    root.mainloop()
    return kml_file

def open_file_dialog(root):
    """KML dosyası seçim penceresini açar"""
    global kml_file
    global selected_region
    global global_data
    
    file_path = filedialog.askopenfilename(title="KML Dosyası Seçin", filetypes=[("KML Files", "*.kml")])
    
    if file_path:
        print(f"Seçilen KML dosyası: {file_path}")
        kml_file = process_to_geodataframe(file_path, global_data, selected_region)
        root.destroy()
        process_kml_data(kml_file)
    else:
        print("Dosya seçilmedi.")

def process_kml_data(kml_file):
    """KML verisini işler ve katmanları ayırır"""
    if kml_file is not None:
        print("KML verisi işleniyor...")
        katmanAyirma(kml_file)
    else:
        print("KML verisi bulunamadı veya geçersiz.")

def katmanAyirma(kml_file):
    """Anahtar kelimelere göre katmanları ayırır ve veriyi işler"""
    print("Katmanlar ayrılıyor...")
    
    # 'Katman Adlandırma' ve 'İmar Tipi (Land-Use)' sütunlarını ekle
    kml_file['Katman Adlandırma'] = ''
    kml_file['İmar Tipi (Land-Use)'] = ''
    kml_file['Fill'] = ''
    
    # Her bir satırı MATCHING_TABLE ile eşleştir
    for i, row in kml_file.iterrows():
        matched = False
        building_value = row.get('building', '')
        
        # Building=yes durumu için özel kontrol
        if isinstance(building_value, str) and building_value.lower() == 'yes':
            # Diğer sütunlarda değer var mı kontrol et
            for col in ['amenity', 'landuse', 'shop', 'office', 'tourism', 'healthcare', 'leisure']:
                col_value = row.get(col)
                if pd.notna(col_value) and str(col_value).strip() != '':
                    # Matching table'dan eşleştirme yap
                    for entry in MATCHING_TABLE:
                        if re.search(entry["Anahtar Kelimeler"], str(col_value), re.IGNORECASE):
                            kml_file.at[i, 'İmar Tipi (Land-Use)'] = entry['İmar Tipi (Land-Use)']
                            kml_file.at[i, 'Katman Adlandirma'] = entry['Katman Adlandirma']
                            kml_file.at[i, 'Fill'] = COLOR_DICT.get(entry['İmar Tipi (Land-Use)'], '#FFFFFF')
                            matched = True
                            break
                    if matched:
                        break
            
            # Diğer sütunlarda eşleşme yoksa styleUrl'e bak
            if not matched:
                style_url_text = str(row.get('styleUrl', '')).strip()
                for entry in MATCHING_TABLE:
                    if re.search(entry["Anahtar Kelimeler"], style_url_text, re.IGNORECASE):
                        kml_file.at[i, 'İmar Tipi (Land-Use)'] = entry['İmar Tipi (Land-Use)']
                        kml_file.at[i, 'Katman Adlandirma'] = entry['Katman Adlandirma']
                        kml_file.at[i, 'Fill'] = COLOR_DICT.get(entry['İmar Tipi (Land-Use)'], '#FFFFFF')
                        matched = True
                        break
        
        # Building=yes değilse normal işlem
        else:
            # Diğer sütunlarda değer var mı kontrol et
            for col in ['amenity', 'landuse', 'shop', 'office', 'tourism', 'healthcare', 'leisure']:
                col_value = row.get(col)
                if pd.notna(col_value) and str(col_value).strip() != '':
                    # Matching table'dan eşleştirme yap
                    for entry in MATCHING_TABLE:
                        if re.search(entry["Anahtar Kelimeler"], str(col_value), re.IGNORECASE):
                            kml_file.at[i, 'İmar Tipi (Land-Use)'] = entry['İmar Tipi (Land-Use)']
                            kml_file.at[i, 'Katman Adlandirma'] = entry['Katman Adlandirma']
                            kml_file.at[i, 'Fill'] = COLOR_DICT.get(entry['İmar Tipi (Land-Use)'], '#FFFFFF')
                            matched = True
                            break
                    if matched:
                        break
            
            # Diğer sütunlarda eşleşme yoksa styleUrl'e bak
            if not matched:
                style_url_text = str(row.get('styleUrl', '')).strip()
                for entry in MATCHING_TABLE:
                    if re.search(entry["Anahtar Kelimeler"], style_url_text, re.IGNORECASE):
                        kml_file.at[i, 'İmar Tipi (Land-Use)'] = entry['İmar Tipi (Land-Use)']
                        kml_file.at[i, 'Katman Adlandirma'] = entry['Katman Adlandirma']
                        kml_file.at[i, 'Fill'] = COLOR_DICT.get(entry['İmar Tipi (Land-Use)'], '#FFFFFF')
                        matched = True
                        break

        # Hiç eşleşme bulunamadıysa
        if not matched:
            kml_file.at[i, 'İmar Tipi (Land-Use)'] = 'unclassified'
            kml_file.at[i, 'Katman Adlandirma'] = 'unclassified'
            kml_file.at[i, 'Fill'] = '#FFFFFF'
    
    # Sütunların sırasını değiştirelim
    columns_order = ['Katman Adlandirma', 'İmar Tipi (Land-Use)'] + [col for col in kml_file.columns if col not in ['Katman Adlandirma', 'İmar Tipi (Land-Use)']]
    kml_file = kml_file[columns_order]
    
    # name ve name_1 sütunlarını düzenle
    if 'name_1' in kml_file.columns and 'name' in kml_file.columns:
        kml_file['name_1'] = kml_file['name_1'].fillna(kml_file['name'])
        
    if 'coords' in kml_file.columns:
        del kml_file['coords']
        
    if 'name' in kml_file.columns and ('name_1' in kml_file.columns or 'name_2' in kml_file.columns):
        del kml_file['name']
    
    unique_gdf = kml_file.drop_duplicates(subset='geometry')
    
    # Sonuçları kaydet ve diğer analizleri yap
    unique_gdf.to_csv(f"./output-shp-files/unique_gdf_{selected_region}.csv", encoding="utf-8-sig", index=False)
    
    # Veri analiz fonksiyonlarını çağır
    process_complete_data(unique_gdf)
    
    return unique_gdf

def process_complete_data(unique_gdf):
    """Tüm veri işleme adımlarını sırasıyla çalıştırır"""
    print("Veri işleme süreci başlatılıyor...")
    
    # 1. İmar ID verilerini ekle
    add_imar_ids_from_points_mesken(unique_gdf, selected_region)
    add_imar_ids_from_points_ticarethane(unique_gdf, selected_region)
    analyze_all_ticarethane(unique_gdf, selected_region)
    
    # 2. hmax değerlerini temizle ve güncelle
    clean_and_test_hmax(unique_gdf)
    
    # 3. İmar verilerini analiz et ve İmar ID'lerini güncelle
    analyze_and_update_imar(unique_gdf)
    
    # 4. hmax ve diğer güncellenmiş değerlerin CSV dosyalarını kaydet
    hmax_values = unique_gdf['hmax'].drop_duplicates()
    os.makedirs('output-files/buffer-deneme', exist_ok=True)
    hmax_values.to_csv('output-files/buffer-deneme/hmax_original_values.csv', header=True, index=False, encoding='utf-8-sig')
    
    # Temizlenmiş veriyi kaydet
    unique_gdf = unique_gdf[unique_gdf['styleUrl'].str.startswith('#', na=False)].copy()
    os.makedirs('output-files', exist_ok=True)
    unique_gdf.to_csv(f'output-files/sonuc-{selected_region}-imar.csv', index=False, encoding='utf-8-sig')
    
    # 5. İmar ID 5 analizi gerçekleştir
    calc_imar_results(unique_gdf, selected_region)
    
    # 6. Sonuçları kaydet ve KML dosyası oluştur
    create_single_shp_with_buffer_preserving_data(unique_gdf)
    createKml(unique_gdf)
    
    print("Veri işleme süreci tamamlandı.")

def add_imar_ids_from_points_mesken(unique_gdf, selected_region):
    """Veritabanından IMAR_ID, ABONE_SAYISI ve ORT_MESKEN_TUKETIM verilerini alıp GeoDataFrame'e ekler."""
    try:
        print("\n--- ADD IMAR IDS FROM POINTS MESKEN: Fonksiyon Başladı ---")
        print(f"--- Seçilen Bölge: {selected_region} ---")

        # Bölge ismine göre veritabanı bağlantısı oluştur
        if "İzmir" in selected_region or "İZMİR" in selected_region:
            print("İzmir bağlantısı seçildi")
            engine = create_engine(DB_CONNECTIONS["gdz"])
        elif "Eskişehir" in selected_region or "ESKİŞEHİR" in selected_region:
            print("Eskişehir bağlantısı seçildi")
            engine = create_engine(DB_CONNECTIONS["oedas"])
        else:
            raise ValueError(f"Geçersiz şehir adı: {selected_region}. İzmir veya Eskişehir seçin.")

        # SQL sorgusu
        query = """
            SELECT 
                "X_KOORDINAT" AS X, 
                "Y_KOORDINAT" AS Y, 
                "IMAR_ID_FORECAST", 
                "ABONE SAYISI",
                "ORT_Mesken_tuketim"
            FROM deep_learning_mesken
        """
        print(f"--- SQL Sorgusu Çalıştırılıyor ---\n{query}")
        points_df = pd.read_sql(query, engine)
        
        # Sütun isimlerini normalize et
        points_df.columns = [col.upper().strip() for col in points_df.columns]
        print(f"--- Sütun İsimleri Normalize Edildi: {points_df.columns.tolist()} ---")
        
        # Geometri oluştur ve GeoDataFrame'e dönüştür
        print("--- Geometri oluşturuluyor ---")
        geometry = [Point(xy) for xy in zip(points_df['X'], points_df['Y'])]
        points_gdf = gpd.GeoDataFrame(points_df, geometry=geometry, crs=unique_gdf.crs)
        
        # CRS kontrolü
        if points_gdf.crs != unique_gdf.crs:
            print("--- CRS farkı algılandı. CRS güncelleniyor... ---")
            points_gdf = points_gdf.to_crs(unique_gdf.crs)
        
        # Spatial Join işlemi
        print("--- Spatial Join işlemi başlıyor ---")
        joined = gpd.sjoin(unique_gdf, points_gdf, how='left', predicate='intersects')
        
        # IMAR_ID işlemi
        def combine_imar_ids(x):
            values = x[x.notna()].astype(int).astype(str).unique()
            return ','.join(values) if len(values) > 0 else '0'

        # ABONE_SAYISI işlemi
        def sum_abone_sayisi(x):
            return x[x.notna()].sum() if len(x) > 0 else 0

        # ORT_Mesken_tuketim işlemi
        def mean_mesken_tuketim(x):
            return x[x.notna()].mean() if len(x) > 0 else 0

        # Grup bazında işlemler
        print("--- IMAR_ID, ABONE_SAYISI ve ORT_MESKEN_TUKETIM hesaplamaları başlıyor ---")
        result_imar = joined.groupby(joined.index)['IMAR_ID_FORECAST'].apply(combine_imar_ids).reset_index()
        result_abone = joined.groupby(joined.index)['ABONE SAYISI'].apply(sum_abone_sayisi).reset_index()
        result_mesken_tuketim = joined.groupby(joined.index)['ORT_MESKEN_TUKETIM'].apply(mean_mesken_tuketim).reset_index()

        # Sonuçları orijinal GeoDataFrame'e ekle
        unique_gdf['IMAR_ID'] = result_imar['IMAR_ID_FORECAST']
        unique_gdf['ABONE_SAYISI'] = result_abone['ABONE SAYISI']
        unique_gdf['ORT_MESKEN_TUKETIM'] = result_mesken_tuketim['ORT_MESKEN_TUKETIM']
        
        print("--- IMAR_ID ve tüketim bilgileri orijinal GeoDataFrame'e eklendi ---")
        return unique_gdf

    except Exception as e:
        print(f"\nHata: {str(e)}")
        import traceback
        traceback.print_exc()
        return unique_gdf

def add_imar_ids_from_points_ticarethane(unique_gdf, selected_region):
    """
    Ticarethane ve sanayi verilerini al ve GeoDataFrame'e ekle
    """
    # Benzer şekilde diğer fonksiyonların implementasyonu...
    # Bu fonksiyonun tam kodunu ekleyebilirsiniz
    return unique_gdf

def clean_and_test_hmax(df):
    """hmax değerlerini temizler ve test eder"""
    # Fonksiyonun tam kodunu ekleyebilirsiniz
    return df

def analyze_and_update_imar(gdf):
    """İmar verilerini analiz eder ve günceller"""
    # Fonksiyonun tam kodunu ekleyebilirsiniz
    return gdf

def analyze_all_ticarethane(gdf, selected_region):
    """Ticarethane bina tiplerini analiz eder"""
    # Fonksiyonun tam kodunu ekleyebilirsiniz
    return gdf

def calc_imar_results(gdf, selected_region):
    """İmar ID analizi ve tüketim hesaplamalarını yapar"""
    # Fonksiyonun tam kodunu ekleyebilirsiniz
    return gdf

def create_single_shp_with_buffer_preserving_data(unique_gdf, point_radius=5):
    """Tüm geometrileri tek bir SHP dosyasına kaydeder"""
    # Fonksiyonun tam kodunu ekleyebilirsiniz
    return unique_gdf

def createKml(unique_gdf):
    """İşlenmiş veriyi KML formatında dışa aktarır"""
    # Fonksiyonun tam kodunu ekleyebilirsiniz
    return unique_gdf

def main():
    """
    Ana fonksiyon - programın akışını kontrol eder
    """
    print("İmar Veri İşleme Programı Başlatılıyor...")
    
    # 1. Bölge seçimi
    global selected_region
    global global_data
    selected_region, global_data = bölge_seçim_arayüzü()
    
    if selected_region is None:
        print("Bölge seçimi yapılmadı. Program sonlandırılıyor.")
        return
    
    print(f"Seçilen bölge: {selected_region}")
    print("Bölge verisi yüklendi. KML dosyası seçimi bekleniyor...")
    
    # 2. KML dosyası seçimi
    global kml_file
    kml_file = kml_seçim_arayüzü()
    
    if kml_file is None:
        print("KML dosyası seçilmedi. Program sonlandırılıyor.")
        return
    
    print("Program başarıyla tamamlandı.")

if __name__ == "__main__":
    main()