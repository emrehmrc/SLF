#!/usr/bin/env python
# -*- coding: utf-8 -*-
 
"""
İmar Oranı Tahmin Modülü Ana Scripti - NaN Düzeltmesi
"""
 
import os
import sys
import pandas as pd
import numpy as np
from scipy.spatial import cKDTree
import openpyxl
 
def preprocess_zoning_data(zoning_df):
    """
    Zoning verilerini ön işlemeye tabi tutar ve oranları hesaplar
   
    Önemli: Oranlar TOPLAM_BINA_SAYISI baz alınarak hesaplanır ve toplamları 1'e eşit olmalıdır
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
   
    # TOPLAM_BINA_SAYISI sütununu kontrol et ve NaN'leri sıfırla
    if 'TOPLAM_BINA_SAYISI' not in df.columns:
        print("[ERROR] TOPLAM_BINA_SAYISI column not found!")
        raise ValueError("TOPLAM_BINA_SAYISI is required!")
   
    df['TOPLAM_BINA_SAYISI'] = df['TOPLAM_BINA_SAYISI'].fillna(0)
   
    # Kategori sütunlarının eksik olanlarını kontrol et ve sıfırla
    for kategori in tum_kategoriler:
        if kategori not in df.columns:
            print(f"[WARNING] {kategori} column not found, setting to 0")
            df[kategori] = 0
        else:
            df[kategori] = df[kategori].fillna(0)
   
    # Her satır için oran hesaplama ve normalleştirme
    for idx, row in df.iterrows():
        toplam_bina = row['TOPLAM_BINA_SAYISI']
       
        if toplam_bina > 0:
            # Kategori toplamını hesapla
            kategori_toplam = 0
            for kategori in tum_kategoriler:
                kategori_toplam += row[kategori]
           
            # Her kategori için oran hesapla
            if kategori_toplam > 0:
                # Kategoriler toplamı TOPLAM_BINA_SAYISI'na eşit olmalı
                # Eşit değilse normalize et
                for kategori in tum_kategoriler:
                    df.loc[idx, f'{kategori}_ORANI'] = row[kategori] / toplam_bina
            else:
                # Hiç kategori yok ama TOPLAM_BINA_SAYISI > 0 ise hata var
                print(f"[WARNING] Row {idx} ID {row.get('id', 'N/A')}: TOPLAM_BINA_SAYISI > 0 but no category values!")
                for kategori in tum_kategoriler:
                    df.loc[idx, f'{kategori}_ORANI'] = 0
        else:
            # TOPLAM_BINA_SAYISI = 0 ise oranlar da 0
            for kategori in tum_kategoriler:
                df.loc[idx, f'{kategori}_ORANI'] = 0
   
    # Oranları normalize et (toplamları 1'e eşitle)
    oran_columns = [f'{k}_ORANI' for k in tum_kategoriler]
   
    for idx, row in df.iterrows():
        oran_total = row[oran_columns].sum()
       
        if oran_total > 0 and abs(oran_total - 1) > 1e-6:
            # Oranları normalize et
            for col in oran_columns:
                df.loc[idx, col] = row[col] / oran_total
   
    # is_calculated sütunu ekle (başlangıçta 0)
    df['is_calculated'] = 0
   
    # Sonuç DataFrame'ini oluştur
    processed_columns = [
        'id',
        'ORT_BAG_GUCU',
        '2023_Tuketim',
        'TOPLAM_BINA_SAYISI'
    ] + oran_columns + [
        'is_calculated'
    ]
   
    # Sütunları kontrol et ve eksik olanları ekle
    missing_columns = [col for col in processed_columns if col not in df.columns]
    if missing_columns:
        print(f"[WARNING] Missing columns: {missing_columns}")
        for col in missing_columns:
            if col.endswith('_ORANI'):
                df[col] = 0
   
    processed_df = df[processed_columns]
   
    # Oran toplamlarını kontrol et
    problematic_rows = []
    for idx, row in processed_df.iterrows():
        oran_total = row[oran_columns].sum()
        if row['TOPLAM_BINA_SAYISI'] > 0 and abs(oran_total - 1) > 1e-6:
            problematic_rows.append({
                'id': row['id'],
                'oran_toplam': oran_total,
                'TOPLAM_BINA_SAYISI': row['TOPLAM_BINA_SAYISI']
            })
   
    if problematic_rows:
        print(f"\n[ERROR] {len(problematic_rows)} satırda oran toplamı 1'e eşit değil:")
        for prob in problematic_rows[:10]:
            print(f"  ID {prob['id']}: TOPLAM_BINA_SAYISI={prob['TOPLAM_BINA_SAYISI']}, Oran toplamı={prob['oran_toplam']:.6f}")
    else:
        print(f"\n[SUCCESS] Tüm satırların oran toplamları doğru.")
   
    return processed_df
 
def haversine_np(lat1, lon1, lat2, lon2):
    """İki coğrafi koordinat arasındaki mesafeyi hesaplar (metre cinsinden)."""
    R = 6371  # Dünya yarıçapı (km)
    lat1, lon1, lat2, lon2 = map(np.radians, [lat1, lon1, lat2, lon2])
    dlat = lat2 - lat1
    dlon = lon2 - lon1
    a = np.sin(dlat / 2) ** 2 + np.cos(lat1) * np.cos(lat2) * np.sin(dlon / 2) ** 2
    return 2 * R * np.arcsin(np.sqrt(a)) * 1000  # Metre cinsinden dönüş
 
def filter_cells_for_year(builtup_df, zoning_df, year, restricted=85):
    """Belirli bir yıl için filtrelenmiş hücreleri döndürür - DETAYLI DEBUG LOGLI."""
    print(f"\n[DEBUG] Yıl {year} için filtreleme başlıyor...")
    print(f"[DEBUG] Toplam hücre sayısı: {len(builtup_df)}")
   
    # Olası saturation sütun isimleri
    possible_current_cols = [
        f'Saturation_updated_{year}',  # Öncelikli tercih
        f'Saturation_{year}',
        f'Saturation{year}',
        f'saturation_ratio_{year}',
        'saturation_ratio'
    ]
    
    possible_prev_cols = [
        f'Saturation_updated_{year - 1}',  # Öncelikli tercih
        f'Saturation_{year - 1}',
        f'Saturation{year - 1}',
        f'saturation_ratio_{year - 1}',
        'saturation_ratio'
    ]
    
    # Mevcut yıl için sütun kontrolü
    saturation_col = None
    for col in possible_current_cols:
        if col in builtup_df.columns:
            saturation_col = col
            print(f"[DEBUG] Mevcut yıl için kullanılan sütun: {saturation_col}")
            break
    
    if saturation_col is None:
        print(f"[ERROR] Mevcut yıl için saturation sütunu bulunamadı! Olası sütunlar: {possible_current_cols}")
        return pd.DataFrame()
    
    # Önceki yıl için sütun kontrolü
    prev_year_col = None
    for col in possible_prev_cols:
        if col in builtup_df.columns:
            prev_year_col = col
            print(f"[DEBUG] Önceki yıl için kullanılan sütun: {prev_year_col}")
            break
    
    if prev_year_col is None:
        print(f"[ERROR] Önceki yıl için saturation sütunu bulunamadı! Olası sütunlar: {possible_prev_cols}")
        return pd.DataFrame()
    
    # Satürasyon artışını hesapla
    builtup_increase = builtup_df[saturation_col] - builtup_df[prev_year_col]
   
    # Her filtreleme adımını ayrı ayrı incele
    # 1. Satürasyon artışı > 0 olanlar
    condition1 = (builtup_increase > 0)
    cells_after_condition1 = builtup_df[condition1]
    print(f"[DEBUG] 1. Filtre - Satürasyon artışı > 0: {len(cells_after_condition1)} hücre")
   
    # Detaylı analiz: Satürasyon artışı dağılımı
    increases = builtup_increase[builtup_increase > 0]
    if len(increases) > 0:
        print(f"[DEBUG]   - Artış aralığı: {increases.min():.6f} - {increases.max():.6f}")
        print(f"[DEBUG]   - Ortalama artış: {increases.mean():.6f}")
   
    # 2. Satürasyon < 1 olanlar
    condition2 = (builtup_df[saturation_col] < 1) & condition1
    cells_after_condition2 = builtup_df[condition2]
    print(f"[DEBUG] 2. Filtre - Satürasyon < 1: {len(cells_after_condition2)} hücre")
   
    # 3. Yasaklı alan yüzdesi < restricted olanlar
    if 'yasakli_alan_percentage' in builtup_df.columns:
        # NaN değerleri 0 olarak doldur
        yasakli_alan_series = builtup_df['yasakli_alan_percentage'].fillna(0)
       
        # YASAKLI ALAN ANALİZİ (NaN değerleri 0 olarak)
        yasakli_alan_values = yasakli_alan_series
        print(f"[DEBUG] YASAKLI ALAN ANALİZİ (NaN değerleri 0 olarak):")
        print(f"[DEBUG]   - Min değer: {yasakli_alan_values.min():.2f}")
        print(f"[DEBUG]   - Max değer: {yasakli_alan_values.max():.2f}")
        print(f"[DEBUG]   - Ortalama değer: {yasakli_alan_values.mean():.2f}")
        print(f"[DEBUG]   - Medyan değer: {yasakli_alan_values.median():.2f}")
       
        # Yasaklı alan dağılımı
        print(f"[DEBUG]   - 0 olan hücre sayısı: {len(yasakli_alan_series[yasakli_alan_series == 0])}")
        print(f"[DEBUG]   - >0 ve <50 olan hücre sayısı: {len(yasakli_alan_series[(yasakli_alan_series > 0) & (yasakli_alan_series < 50)])}")
        print(f"[DEBUG]   - >=50 ve <85 olan hücre sayısı: {len(yasakli_alan_series[(yasakli_alan_series >= 50) & (yasakli_alan_series < 85)])}")
        print(f"[DEBUG]   - >=85 olan hücre sayısı: {len(yasakli_alan_series[yasakli_alan_series >= 85])}")
        print(f"[DEBUG]   - =100 olan hücre sayısı: {len(yasakli_alan_series[yasakli_alan_series == 100])}")
       
        condition3 = (yasakli_alan_series < restricted) & condition2
        cells_after_condition3 = builtup_df[condition3]
        print(f"[DEBUG] 3. Filtre - Yasaklı alan yüzdesi < {restricted}: {len(cells_after_condition3)} hücre")
       
        # Filtre uygulanan hücrelerin yasaklı alan dağılımı
        filtered_yasakli_values = yasakli_alan_series[condition2]
        print(f"[DEBUG] Filtre öncesi yasaklı alan dağılımı (condition2 sonrası):")
        print(f"[DEBUG]   - Min değer: {filtered_yasakli_values.min():.2f}")
        print(f"[DEBUG]   - Max değer: {filtered_yasakli_values.max():.2f}")
        print(f"[DEBUG]   - Ortalama değer: {filtered_yasakli_values.mean():.2f}")
       
        # Elenen hücrelerin yasaklı alan değerleri
        eliminated_cells = builtup_df[condition2 & (yasakli_alan_series >= restricted)]
        if len(eliminated_cells) > 0:
            print(f"[DEBUG] Yasaklı alan nedeniyle elenen {len(eliminated_cells)} hücrenin değerleri:")
            yasakli_degerler = yasakli_alan_series[eliminated_cells.index]
            print(f"[DEBUG]   - Min: {yasakli_degerler.min():.2f}")
            print(f"[DEBUG]   - Max: {yasakli_degerler.max():.2f}")
            print(f"[DEBUG]   - Ortalama: {yasakli_degerler.mean():.2f}")
    else:
        print("[WARNING] yasakli_alan_percentage column not found!")
        condition3 = condition2
        cells_after_condition3 = cells_after_condition2
   
    # 4. is_calculated = 0 olan hücreler
    zoning_not_calculated = zoning_df[zoning_df['is_calculated'] == 0]
    print(f"[DEBUG] 4. Filtre - is_calculated = 0 olan zoning hücreleri: {len(zoning_not_calculated)}")
   
    condition4 = condition3 & builtup_df['id'].isin(zoning_not_calculated['id'])
    cells_after_condition4 = builtup_df[condition4]
    print(f"[DEBUG] 4. Filtre - Zoning'de is_calculated = 0: {len(cells_after_condition4)} hücre")
   
    # Son filtrelenmiş sonuç
    filtered_result = cells_after_condition4
   
    # Sonuç özeti
    print(f"[DEBUG] Filtreleme sonucu: {len(filtered_result)} hücre")
    if len(filtered_result) < 10:
        # Çok az hücre varsa, neden az olduğunu anlamak için daha fazla detay
        print(f"[DEBUG] [UYARI] Çok az hücre filtrelendi!")
        print(f"[DEBUG]   Satürasyon artışı = 0 olan hücre sayısı: {len(builtup_df[builtup_increase == 0])}")
        print(f"[DEBUG]   Satürasyon = 1 olan hücre sayısı: {len(builtup_df[builtup_df[saturation_col] == 1])}")
        if 'yasakli_alan_percentage' in builtup_df.columns:
            yasakli_series_for_filter = builtup_df['yasakli_alan_percentage'].fillna(0)
            print(f"[DEBUG]   Yasaklı alan >= {restricted} olan hücre sayısı: {len(builtup_df[yasakli_series_for_filter >= restricted])}")
        print(f"[DEBUG]   is_calculated = 1 olan hücre sayısı: {len(zoning_df[zoning_df['is_calculated'] == 1])}")
   
    return filtered_result
 
def find_neighbors(builtup_df, center_lat, center_lon, radius=500):
    """Verilen merkez noktası etrafındaki komşu hücreleri bulur."""
    coords = np.radians(builtup_df[['lat', 'lon']].values)
    tree = cKDTree(coords)
    center_coords = np.radians([center_lat, center_lon])
    indices = tree.query_ball_point(center_coords, radius / 6371000)
    return indices
 
def calculate_weighted_average(filtered_cells, builtup_df, zoning_df, year):
    """
    Filtrelenmiş hücreler için ağırlıklı ortalama imar oranlarını hesaplar.
   
    Önemli: Varsayılan değer ataması YAPILMAZ! Sadece gerçek hesaplamalar yapılır.
    """
    # Olası saturation sütun isimleri
    possible_saturation_cols = [
        f'Saturation_updated_{year}',  # Öncelikli tercih
        f'Saturation_{year}',
        f'Saturation{year}',
        f'saturation_ratio_{year}',
        'saturation_ratio'
    ]
    
    # Mevcut yıl için sütun kontrolü
    saturation_col = None
    for col in possible_saturation_cols:
        if col in builtup_df.columns:
            saturation_col = col
            print(f"[DEBUG] Ağırlık hesaplaması için kullanılan saturation sütunu: {saturation_col}")
            break
    
    if saturation_col is None:
        print(f"[ERROR] Ağırlık hesaplaması için saturation sütunu bulunamadı! Olası sütunlar: {possible_saturation_cols}")
        return []
    
    oran_columns = [col for col in zoning_df.columns if col.endswith('_ORANI')]
    results = []
    no_neighbors_count = 0
   
    for cell_idx, (_, cell) in enumerate(filtered_cells.iterrows()):
        center_lat = cell['lat']
        center_lon = cell['lon']
        cell_id = cell['id']
       
        # 500 metre mesafedeki komşuları bul
        neighbor_indices = find_neighbors(builtup_df, center_lat, center_lon, radius=500)
        neighbors = builtup_df.iloc[neighbor_indices]
       
        # Komşuları filtrele
        valid_neighbors = []
        for _, neighbor in neighbors.iterrows():
            if neighbor['id'] != cell['id']:
                neighbor_saturation = neighbor[saturation_col]
                if neighbor_saturation > 0:
                    neighbor_zoning = zoning_df[zoning_df['id'] == neighbor['id']]
                    if not neighbor_zoning.empty:
                        neighbor_ratios_sum = neighbor_zoning[oran_columns].sum(axis=1).values[0]
                        if abs(neighbor_ratios_sum - 1) < 1e-6:
                            valid_neighbors.append(neighbor)
       
        # Geçerli komşu yoksa bu hücreyi atla
        if not valid_neighbors:
            no_neighbors_count += 1
            continue
       
        # Ağırlık hesaplama (Saturation/Distance)
        weights = []
        for neighbor in valid_neighbors:
            distance = haversine_np(center_lat, center_lon, neighbor['lat'], neighbor['lon'])
            saturation = neighbor[saturation_col]
            weight = saturation / max(distance, 1)
            weights.append((neighbor['id'], weight))
       
        # Ağırlıklı ortalama oranlarını hesapla
        weighted_ratios = {}
        total_weight = sum(weight for _, weight in weights)
       
        for column in oran_columns:
            weighted_sum = 0
            for neighbor_id, weight in weights:
                neighbor_zoning = zoning_df[zoning_df['id'] == neighbor_id]
                if not neighbor_zoning.empty:
                    value = neighbor_zoning[column].values[0]
                    weighted_sum += weight * value
           
            weighted_ratios[column] = weighted_sum / total_weight if total_weight > 0 else 0
       
        # Oranları normalize et (toplamı 1'e eşitle)
        ratio_sum = sum(weighted_ratios.values())
        if ratio_sum > 0:
            for column in oran_columns:
                weighted_ratios[column] = weighted_ratios[column] / ratio_sum
       
        results.append({'id': cell['id'], **weighted_ratios})
   
    print(f"[DEBUG] Geçerli komşusu olmayan hücre sayısı: {no_neighbors_count}")
    return results
 
def create_output(builtup_df, zoning_df, calculated_ratios, year):
    """Hesaplanan imar oranları için çıktı oluşturur."""
    oran_columns = [col for col in zoning_df.columns if col.endswith('_ORANI')]
   
    output_rows = []
    for _, cell in builtup_df.iterrows():
        row_data = {
            'id': cell['id'],
            'left': cell.get('left'),
            'top': cell.get('top'),
            'right': cell.get('right'),
            'bottom': cell.get('bottom'),
            'ilce': cell.get('ilce')
        }
       
        # Mevcut zoning verilerinden oranları al
        existing_zoning = zoning_df[zoning_df['id'] == cell['id']]
        if not existing_zoning.empty:
            for col in oran_columns:
                row_data[col] = existing_zoning[col].values[0]
        else:
            # Hiç veri yoksa 0 ata
            for col in oran_columns:
                row_data[col] = 0
       
        output_rows.append(row_data)
   
    return pd.DataFrame(output_rows)
 
def tahmin_et(builtup_df, zoning_df, output_path, start_year=2024, end_year=2035):
    """SLF analiz sonuçlarını kullanarak imar oranı tahmini yapar."""
    try:
        os.makedirs(os.path.dirname(output_path), exist_ok=True)
        print(f"İmar oranı tahmini başlatılıyor: {start_year}-{end_year}")
       
        # GENEL YASAKLI ALAN ANALİZİ
        if 'yasakli_alan_percentage' in builtup_df.columns:
            # NaN değerleri 0 olarak doldur
            builtup_df['yasakli_alan_percentage'] = builtup_df['yasakli_alan_percentage'].fillna(0)
            print("\n[DEBUG] GENEL YASAKLI ALAN İSTATİSTİKLERİ (NaN değerleri 0 olarak):")
            yasakli_alan_values = builtup_df['yasakli_alan_percentage']
            print(f"[DEBUG] Toplam hücre sayısı: {len(builtup_df)}")
            print(f"[DEBUG] Yasaklı alan değer aralığı: {yasakli_alan_values.min():.2f} - {yasakli_alan_values.max():.2f}")
            print(f"[DEBUG] Ortalama yasaklı alan yüzdesi: {yasakli_alan_values.mean():.2f}")
            print(f"[DEBUG] Medyan yasaklı alan yüzdesi: {yasakli_alan_values.median():.2f}")
           
            # Yasaklı alan dağılımı
            bins = [0, 25, 50, 75, 85, 90, 95, 100]
            for i in range(len(bins)-1):
                count = len(builtup_df[(builtup_df['yasakli_alan_percentage'] >= bins[i]) &
                                     (builtup_df['yasakli_alan_percentage'] < bins[i+1])])
                print(f"[DEBUG] {bins[i]}-{bins[i+1]}% arası: {count} hücre ({count/len(builtup_df)*100:.1f}%)")
           
            # 100% olanlar
            count_100 = len(builtup_df[builtup_df['yasakli_alan_percentage'] == 100])
            print(f"[DEBUG] Tam 100%: {count_100} hücre ({count_100/len(builtup_df)*100:.1f}%)")
       
        workbook = openpyxl.Workbook()
        if workbook.active:
            workbook.remove(workbook.active)
       
        # Zoning df'nin değişebilir bir kopyasını oluştur
        zoning_df_working = zoning_df.copy()
       
        # İlk durum debug bilgisi
        print(f"\n[DEBUG] BAŞLANGIÇ DURUMU:")
        print(f"[DEBUG] Toplam zoning hücresi: {len(zoning_df_working)}")
        print(f"[DEBUG] is_calculated = 0 olan hücre sayısı: {len(zoning_df_working[zoning_df_working['is_calculated'] == 0])}")
        print(f"[DEBUG] is_calculated = 1 olan hücre sayısı: {len(zoning_df_working[zoning_df_working['is_calculated'] == 1])}")
       
        # Her yıl için hesaplamaları yap
        for year in range(start_year, end_year + 1):
            print(f"\n{'='*50}")
            print(f"Yıl {year} işleniyor...")
            print(f"{'='*50}")
           
            # Filtrelenmiş hücreleri al
            filtered_cells = filter_cells_for_year(builtup_df, zoning_df_working, year)
            print(f"Filtrelenen hücre sayısı: {len(filtered_cells)}")
           
            # Eğer filtrelenen hücre sayısı çok azsa detaylı bilgi ver
            if len(filtered_cells) < 10:
                print(f"[WARNING] Çok az hücre filtrelendi! Detaylı analiz:")
                if len(filtered_cells) > 0:
                    print(f"[DEBUG] Filtrelenen hücre ID'leri: {filtered_cells['id'].tolist()}")
           
            # Ağırlıklı ortalama hesapla
            calculated_ratios = calculate_weighted_average(filtered_cells, builtup_df, zoning_df_working, year)
            print(f"Hesaplanan oran sayısı: {len(calculated_ratios)}")
           
            # is_calculated sütununu güncelle ve oranları yaz
            if len(calculated_ratios) > 0:
                calculated_ids = [ratio['id'] for ratio in calculated_ratios]
               
                # is_calculated değerini güncelle
                before_count = (zoning_df_working['is_calculated'] == 1).sum()
                zoning_df_working.loc[zoning_df_working['id'].isin(calculated_ids), 'is_calculated'] = 1
                after_count = (zoning_df_working['is_calculated'] == 1).sum()
               
                print(f"[DEBUG] is_calculated=1 count: {before_count} -> {after_count}")
                print(f"[DEBUG] Bu yıl işlenen yeni hücre sayısı: {after_count - before_count}")
               
                # Zoning DataFrame'ini güncelle
                oran_columns = [col for col in zoning_df_working.columns if col.endswith('_ORANI')]
                for ratio in calculated_ratios:
                    ratio_id = ratio['id']
                    # DataFrame'e bir satırda tüm oranları güncelle
                    for col in oran_columns:
                        if col in ratio:
                            zoning_df_working.loc[zoning_df_working['id'] == ratio_id, col] = ratio[col]
               
                # Debug için güncellenen değerleri kontrol et
                updated_rows = zoning_df_working[zoning_df_working['id'].isin(calculated_ids)]
                print(f"[DEBUG] Güncellenen ilk 5 satır örneği:")
                if not updated_rows.empty:
                    for idx, row in updated_rows.head().iterrows():
                        print(f"  ID {row['id']}: {row[oran_columns].to_dict()}")
            else:
                print("[WARNING] Bu yıl için hiç oran hesaplanamadı!")
           
            # Çıktı oluştur - güncel zoning_df_working'i kullan
            output_df = create_output(builtup_df, zoning_df_working, None, year)
           
            # Excel'e sheet olarak ekle
            sheet = workbook.create_sheet(title=str(year))
           
            # Başlıkları yaz
            for col, header in enumerate(output_df.columns, 1):
                sheet.cell(row=1, column=col, value=header)
           
            # Verileri yaz
            for r, row_data in enumerate(output_df.values, 2):
                for c, value in enumerate(row_data, 1):
                    sheet.cell(row=r, column=c, value=value)
           
            # Yıl sonu durumu
            print(f"\n[DEBUG] Yıl {year} SONU DURUMU:")
            print(f"[DEBUG] is_calculated = 0 olan hücre sayısı: {len(zoning_df_working[zoning_df_working['is_calculated'] == 0])}")
            print(f"[DEBUG] is_calculated = 1 olan hücre sayısı: {len(zoning_df_working[zoning_df_working['is_calculated'] == 1])}")
       
        workbook.save(output_path)
        print(f"\nİmar oranı tahmini tamamlandı. Sonuçlar kaydedildi: {output_path}")
       
        final_calculated_count = (zoning_df_working['is_calculated'] == 1).sum()
        total_cells = len(zoning_df_working)
        print(f"[SUMMARY] {final_calculated_count}/{total_cells} hücre işlendi.")
       
        return zoning_df_working
   
    except Exception as e:
        print(f"İmar oranı tahmininde hata: {str(e)}")
        import traceback
        traceback.print_exc()
        raise
 
# Ana çalıştırma bloğu
if __name__ == "__main__":
    if len(sys.argv) < 4:
        print("Kullanım: python script.py builtup_dosyasi zoning_dosyasi output_dosyasi [baslangic_yili] [bitis_yili]")
        sys.exit(1)
   
    builtup_file = sys.argv[1]
    zoning_file = sys.argv[2]
    output_file = sys.argv[3]
   
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
        # Dosyaları yükle
        if os.path.splitext(builtup_file)[1].lower() == '.csv':
            builtup_df = pd.read_csv(builtup_file)
        else:
            builtup_df = pd.read_excel(builtup_file)
       
        if os.path.splitext(zoning_file)[1].lower() == '.csv':
            zoning_df = pd.read_csv(zoning_file)
        else:
            zoning_df = pd.read_excel(zoning_file)
       
        # Yasaklı alan NaN değerlerini 0 olarak doldur
        if 'yasakli_alan_percentage' in builtup_df.columns:
            nan_count = builtup_df['yasakli_alan_percentage'].isna().sum()
            if nan_count > 0:
                print(f"\n[DEBUG] {nan_count} adet NaN değeri 0 olarak ayarlanıyor...")
                builtup_df['yasakli_alan_percentage'] = builtup_df['yasakli_alan_percentage'].fillna(0)
           
            # Doldurma sonrası analiz
            yasakli_degerler = builtup_df['yasakli_alan_percentage']
            print(f"[DEBUG] NaN değerleri doldurulduktan sonra yasaklı alan dağılımı:")
            print(f"[DEBUG]   - Min: {yasakli_degerler.min():.2f}")
            print(f"[DEBUG]   - Max: {yasakli_degerler.max():.2f}")
            print(f"[DEBUG]   - Ortalama: {yasakli_degerler.mean():.2f}")
            print(f"[DEBUG]   - Medyan: {yasakli_degerler.median():.2f}")
           
            # Farklı eşik değerleri için analiz
            thresholds = [50, 75, 85, 90, 95]
            for threshold in thresholds:
                count = len(builtup_df[builtup_df['yasakli_alan_percentage'] < threshold])
                print(f"[DEBUG]   - < {threshold}% olan hücre sayısı: {count} ({count/len(builtup_df)*100:.1f}%)")
       
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