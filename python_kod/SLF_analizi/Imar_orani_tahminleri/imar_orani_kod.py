#!/usr/bin/env python
# -*- coding: utf-8 -*-

"""
İmar Oranı Tahmin Modülü Ana Scripti
"""

import os
import sys
import pandas as pd
import numpy as np
from scipy.spatial import cKDTree
import openpyxl
# Ön işleme fonksiyonu
def preprocess_zoning_data(zoning_df):
    """
    Zoning verilerini ön işlemeye tabi tutar ve oranları hesaplar
    
    Parameters:
    -----------
    zoning_df : pandas.DataFrame
        Orijinal zoning verilerini içeren DataFrame
    
    Returns:
    --------
    processed_df : pandas.DataFrame
        Ön işlenmiş zoning verilerini içeren yeni DataFrame
    """
    # Çalışma kopyasını oluştur
    df = zoning_df.copy()
    
    # Bina kategorileri
    bina_kategorileri = [
        '1-2 KATLI MESKEN', 
        '3-4 KATLI MESKEN', 
        '5-7 KATLI MESKEN', 
        '8 USTU KATLI MESKEN', 
        'VILLA MESKEN'
    ]
    
    # Diğer kategoriler
    diger_kategoriler = [
        'AYDINLATMA', 
        'BUYUK_SANAYI', 
        'BUYUK_TICARETHANE', 
        'KUCUK_SANAYI', 
        'KUCUK_TICARETHANE', 
        'ORTA_SANAYI', 
        'ORTA_TICARETHANE', 
        'TARIMSAL_SULAMA'
    ]
    
    # Tüm kategoriler
    tum_kategoriler = bina_kategorileri + diger_kategoriler
    
    # Yeni DataFrame için boş kontrol
    df['TOPLAM_BINA_SAYISI'] = df['TOPLAM_BINA_SAYISI'].fillna(0)
    
    # Her kategori için oran hesaplama
    for kategori in tum_kategoriler:
        df[f'{kategori}_ORANI'] = np.where(
            df['TOPLAM_BINA_SAYISI'] > 0, 
            df[kategori] / df['TOPLAM_BINA_SAYISI'], 
            0
        )
    
    # is_calculated sütunu ekle (başlangıçta 0)
    df['is_calculated'] = 0
    
    # Sonuç DataFrame'ini oluştur
    processed_columns = [
        'id', 
        'ORT_BAG_GUCU', 
        '2023_Tuketim', 
        'TOPLAM_BINA_SAYISI'
    ] + [f'{k}_ORANI' for k in tum_kategoriler] + [
        'is_calculated'
    ]
    
    processed_df = df[processed_columns]
    
    return processed_df

# Haversine fonksiyonu
def haversine_np(lat1, lon1, lat2, lon2):
    """
    İki coğrafi koordinat arasındaki mesafeyi hesaplar (metre cinsinden).
    """
    R = 6371  # Dünya yarıçapı (km)
    lat1, lon1, lat2, lon2 = map(np.radians, [lat1, lon1, lat2, lon2])
    dlat = lat2 - lat1
    dlon = lon2 - lon1
    a = np.sin(dlat / 2) ** 2 + np.cos(lat1) * np.cos(lat2) * np.sin(dlon / 2) ** 2
    return 2 * R * np.arcsin(np.sqrt(a)) * 1000  # Metre cinsinden dönüş

# Filtreleme fonksiyonu
def filter_cells_for_year(builtup_df, zoning_df, year, restricted=0.85):
    """
    Belirli bir yıl için filtrelenmiş hücreleri döndürür.
    """
    saturation_col = f'Saturation_updated_{year}'
    prev_year_col = f'Saturation_ratio_{year - 1}'
    
    # Önceki yıl sütunu yoksa varsayılan olarak 2024 yılını kullan
    if prev_year_col not in builtup_df.columns and year > 2024:
        prev_year_col = 'Saturation_updated_2024'
        print(f"Uyarı: {year-1} yılına ait satürasyon verisi bulunamadı, {2024} yılı verisi kullanılıyor.")
    
    # Yasaklı alan yüzdesi kontrolü
    if 'yasakli_alan_percentage' not in builtup_df.columns:
        print("Uyarı: 'yasakli_alan_percentage' sütunu bulunamadı, varsayılan olarak 0 kullanılıyor.")
        builtup_df['yasakli_alan_percentage'] = 0
    
    # Hesaplama için filtreleme
    builtup_increase = builtup_df[saturation_col] - builtup_df[prev_year_col]
    
    return builtup_df[
        (builtup_increase != 0) & 
        (builtup_df[saturation_col] < 1) & 
        (builtup_df['yasakli_alan_percentage'] < restricted) &
        builtup_df['id'].isin(zoning_df[zoning_df['is_calculated'] == 0]['id'])
    ]

# Komşuluk hesaplama
def find_neighbors(builtup_df, center_lat, center_lon, radius=500):
    """
    Verilen merkez noktası etrafındaki komşu hücreleri bulur.
    """
    coords = np.radians(builtup_df[['lat', 'lon']].values)
    tree = cKDTree(coords)
    center_coords = np.radians([center_lat, center_lon])
    indices = tree.query_ball_point(center_coords, radius / 6371000)  # Radius in radians
    return indices

# Ağırlıklı ortalama hesaplama
def calculate_weighted_average(filtered_cells, builtup_df, zoning_df, year):
    """
    Filtrelenmiş hücreler için ağırlıklı ortalama imar oranlarını hesaplar.
    """
    # Oran hesaplaması için sütunları belirle
    oran_columns = [col for col in zoning_df.columns if col.endswith('_ORANI')]
    
    results = []
    for _, cell in filtered_cells.iterrows():
        center_lat = cell['lat']
        center_lon = cell['lon']
        
        # 500 metre mesafedeki komşuları bul
        neighbor_indices = find_neighbors(builtup_df, center_lat, center_lon, radius=500)
        neighbors = builtup_df.iloc[neighbor_indices]

        # Komşuları filtrele
        valid_neighbors = []
        for _, neighbor in neighbors.iterrows():
            # Komşunun zoning verisini al
            neighbor_zoning = zoning_df[zoning_df['id'] == neighbor['id']]
            
            if neighbor_zoning.empty:
                continue
            
            # Oran toplamı kontrolü
            oran_toplam = neighbor_zoning[oran_columns].sum(axis=1).values[0]
            
            if (
                abs(oran_toplam - 1) < 1e-6 and  # Oranların toplamı yaklaşık 1
                neighbor[f'Saturation_updated_{year}'] > 0 and  # Saturasyon oranı > 0
                neighbor['id'] != cell['id']  # Hedef hücre kendisi değil
            ):
                valid_neighbors.append(neighbor)

        # Eğer geçerli komşu yoksa devam et
        if not valid_neighbors:
            continue

        # Ağırlık hesaplama (Saturation/Distance)
        weights = []
        for neighbor in valid_neighbors:
            distance = haversine_np(center_lat, center_lon, neighbor['lat'], neighbor['lon'])
            weight = neighbor[f'Saturation_updated_{year}'] / max(distance, 1)  # 0'a bölünmeyi önle
            weights.append((neighbor['id'], weight))

        if not weights:
            continue
        
        # Ağırlıklı ortalama oranlarını hesapla
        weighted_ratios = {}
        for column in oran_columns:
            numerator = sum(
                weight * zoning_df.loc[zoning_df['id'] == neighbor_id, column].values[0]
                for neighbor_id, weight in weights
                if not zoning_df.loc[zoning_df['id'] == neighbor_id, column].empty
            )
            denominator = sum(weight for _, weight in weights)
            weighted_ratios[column] = numerator / denominator if denominator != 0 else 0
        
        results.append({'id': cell['id'], **weighted_ratios})
    
    return results

# Çıktı oluşturma
def create_output(builtup_df, zoning_df, calculated_ratios, year):
    """
    Hesaplanan imar oranları için çıktı oluşturur.
    """
    # Oran sütunlarını belirle
    oran_columns = [col for col in zoning_df.columns if col.endswith('_ORANI')]
    
    output_rows = []
    for _, cell in builtup_df.iterrows():
        # Temel hücre bilgileri
        row_data = {
            'id': cell['id'],
            'left': cell.get('left'),
            'top': cell.get('top'),
            'right': cell.get('right'),
            'bottom': cell.get('bottom'),
            'ilce': cell.get('ilce')
        }
        
        # Hesaplanan veya mevcut oranları ekle
        if cell['id'] in [ratio['id'] for ratio in calculated_ratios]:
            updated_ratios = next(ratio for ratio in calculated_ratios if ratio['id'] == cell['id'])
            row_data.update(updated_ratios)
        else:
            # Mevcut zoning verilerinden oranları al
            existing_zoning = zoning_df[zoning_df['id'] == cell['id']]
            if not existing_zoning.empty:
                for col in oran_columns:
                    row_data[col] = existing_zoning[col].values[0]
            else:
                # Eğer hiç veri yoksa sıfır ata
                for col in oran_columns:
                    row_data[col] = 0
        
        output_rows.append(row_data)
    
    return pd.DataFrame(output_rows)

# Ana tahmin fonksiyonu
def tahmin_et(builtup_df, zoning_df, output_path, start_year=2024, end_year=2035):
    """
    SLF analiz sonuçlarını kullanarak imar oranı tahmini yapar.
    """
    try:
        # Çıktı dizinini oluştur
        os.makedirs(os.path.dirname(output_path), exist_ok=True)
        
        print(f"İmar oranı tahmini başlatılıyor: {start_year}-{end_year}")
        
        # Workbook ve writer oluşturma
        workbook = openpyxl.Workbook()
        
        # Varsayılan sheet'i sil
        if workbook.active:
            workbook.remove(workbook.active)
        
        # Her yıl için hesaplamaları yap
        for year in range(start_year, end_year + 1):
            print(f"\nYıl {year} işleniyor...")
            
            # Satürasyon sütununun varlığını kontrol et
            saturation_col = f'Saturation_updated_{year}'
            if saturation_col not in builtup_df.columns:
                print(f"Uyarı: {saturation_col} sütunu bulunamadı, bu yıl atlanıyor.")
                continue
            
            # Filtrelenmiş hücreleri al
            filtered_cells = filter_cells_for_year(builtup_df, zoning_df, year)
            print(f"Filtrelenen hücre sayısı: {len(filtered_cells)}")
            
            if len(filtered_cells) == 0:
                print("Filtrelenen hücre yok, bu yıl için hesaplama yapılmıyor.")
                continue
            
            # Ağırlıklı ortalama hesapla
            calculated_ratios = calculate_weighted_average(filtered_cells, builtup_df, zoning_df, year)
            print(f"Hesaplanan oran sayısı: {len(calculated_ratios)}")
            
            # Çıktı oluştur
            output_df = create_output(builtup_df, zoning_df, calculated_ratios, year)
            
            # is_calculated sütununu güncelle
            for cell_id in [ratio['id'] for ratio in calculated_ratios]:
                zoning_df.loc[zoning_df['id'] == cell_id, 'is_calculated'] = 1
            
            # Excel'e sheet olarak ekle
            sheet = workbook.create_sheet(title=str(year))
            
            # Başlıkları yaz
            for col, header in enumerate(output_df.columns, 1):
                sheet.cell(row=1, column=col, value=header)
            
            # Verileri yaz
            for r, row_data in enumerate(output_df.values, 2):
                for c, value in enumerate(row_data, 1):
                    sheet.cell(row=r, column=c, value=value)
            
            # Zoning DataFrame'ini güncelle
            oran_columns = [col for col in zoning_df.columns if col.endswith('_ORANI')]
            for col in oran_columns:
                zoning_df.loc[zoning_df['id'].isin(output_df['id']), col] = \
                    output_df.set_index('id')[col]
        
        # Dosyayı kaydet
        workbook.save(output_path)
        
        print(f"\nİmar oranı tahmini tamamlandı. Sonuçlar kaydedildi: {output_path}")
        
        return zoning_df
    
    except Exception as e:
        print(f"İmar oranı tahmininde hata: {str(e)}")
        import traceback
        traceback.print_exc()  # Detaylı hata bilgisi
        raise

# Tanılama fonksiyonu
def print_dataframe_details(df, name):
    """
    DataFrame için kapsamlı tanılama yazdırması
    """
    print(f"\n--- {name} DataFrame Details ---")
    print(f"Shape: {df.shape}")
    print(f"Columns: {list(df.columns)}")
    
    # Veri tiplerini kontrol et
    print("\nColumn Data Types:")
    print(df.dtypes)
    
    # Boş değer sayılarını kontrol et
    print("\nNull Value Counts:")
    print(df.isnull().sum())
    
    # İlk birkaç satırı yazdır
    print("\nFirst 5 Rows:")
    print(df.head())
    
    # Sayısal sütunlar için temel istatistikleri yazdır
    print("\nNumeric Column Statistics:")
    print(df.describe())

# Ana çalıştırma bloğu
if __name__ == "__main__":
    # Yeterli argüman kontrolü
    if len(sys.argv) < 4:
        print("Kullanım: python script.py builtup_dosyasi zoning_dosyasi output_dosyasi [baslangic_yili] [bitis_yili]")
        sys.exit(1)
    
    # Zorunlu argümanları al
    builtup_file = sys.argv[1]
    zoning_file = sys.argv[2]
    output_file = sys.argv[3]
    
    # İsteğe bağlı yıl argümanlarını kontrol et
    start_year = 2024
    end_year = 2035
    
    if len(sys.argv) > 4:
        try:
            start_year = int(sys.argv[4])
        except ValueError:
            print("Başlangıç yılı geçerli bir tamsayı değil. Varsayılan 2024 kullanılacak.")
    
    if len(sys.argv) > 5:
        try:
            end_year = int(sys.argv[5])
        except ValueError:
            print("Bitiş yılı geçerli bir tamsayı değil. Varsayılan 2035 kullanılacak.")
    
    try:
        # Builtup dosyasını yükle
        print(f"Builtup dosyası yükleniyor: {builtup_file}")
        builtup_file_ext = os.path.splitext(builtup_file)[1].lower()
        
        if builtup_file_ext == '.csv':
            builtup_df = pd.read_csv(builtup_file)
        elif builtup_file_ext in ['.xlsx', '.xls']:
            # Tüm sheet'leri dene
            xlsx = pd.ExcelFile(builtup_file)
            print("\nBuiltup dosyasındaki mevcut sheet'ler:", xlsx.sheet_names)
            
            sheets = xlsx.sheet_names
            for sheet in sheets:
                try:
                    builtup_df = pd.read_excel(builtup_file, sheet_name=sheet)
                    print(f"\nSheet başarıyla okundu: {sheet}")
                    break
                except Exception as sheet_error:
                    print(f"Sheet okunamadı {sheet}: {sheet_error}")
        else:
            raise ValueError(f"Desteklenmeyen dosya formatı: {builtup_file_ext}")
        
        # Zoning dosyasını yükle
        print(f"\nZoning dosyası yükleniyor: {zoning_file}")
        zoning_file_ext = os.path.splitext(zoning_file)[1].lower()
        
        if zoning_file_ext == '.csv':
            zoning_df = pd.read_csv(zoning_file)
        elif zoning_file_ext in ['.xlsx', '.xls']:
            xlsx = pd.ExcelFile(zoning_file)
            print("\nZoning dosyasındaki mevcut sheet'ler:", xlsx.sheet_names)
            
            sheets = xlsx.sheet_names
            for sheet in sheets:
                try:
                    zoning_df = pd.read_excel(zoning_file, sheet_name=sheet)
                    print(f"\nSheet başarıyla okundu: {sheet}")
                    break
                except Exception as sheet_error:
                    print(f"Sheet okunamadı {sheet}: {sheet_error}")
        else:
            raise ValueError(f"Desteklenmeyen dosya formatı: {zoning_file_ext}")
        
        # Zoning verilerini ön işleme
        zoning_df = preprocess_zoning_data(zoning_df)
        
        # Tahmin fonksiyonunu çağır
        sonuc_zoning_df = tahmin_et(
            builtup_df=builtup_df, 
            zoning_df=zoning_df, 
            output_path=output_file,
            start_year=start_year,
            end_year=end_year
        )
    
    except Exception as e:
        print(f"Hata: {str(e)}")
        sys.exit(1)