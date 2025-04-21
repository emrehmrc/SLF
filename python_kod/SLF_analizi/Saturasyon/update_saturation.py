#!/usr/bin/env python
# -*- coding: utf-8 -*-

"""
SLF (Saturation Load Flow) Analizi - Saturasyon Güncelleme Modülü
Bu modül, kentsel alanların satürasyon değerlerini kümeleme ve gelişim alanı tiplerine
göre güncellemek için gerekli fonksiyonları içerir.
"""
# python .\update_saturation.py   "C:\Users\batuhan.yetis\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\il_ilce_kırılımları\İzmir\Çiğli\proje_cigli_deneme\imar_analizi_sonuclari\imar_planlari\saturasyon\saturasyon_deneme.csv"  "izmir" "çiğli" "C:\Users\batuhan.yetis\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\il_ilce_kırılımları\İzmir\Çiğli\proje_cigli_deneme\imar_analizi_sonuclari\slf_sonuclari"

import pandas as pd
from sklearn.cluster import DBSCAN
from sklearn.cluster import KMeans
import numpy as np
import random
import argparse
import sys
import os

def haversine(lat1, lon1, lat2, lon2):
    """Haversine formülü ile iki nokta arasındaki mesafeyi metre cinsinden hesaplar."""
    R = 6371  # Dünya yarıçapı (km)
    lat1, lon1, lat2, lon2 = map(np.radians, [lat1, lon1, lat2, lon2])
    dlat = lat2 - lat1
    dlon = lon2 - lon1
    a = np.sin(dlat / 2) ** 2 + np.cos(lat1) * np.cos(lat2) * np.sin(dlon / 2) ** 2
    return 2 * R * np.arcsin(np.sqrt(a)) * 1000  # Metre cinsinden dönüş

def cluster_with_dbscan_and_split(builtup_df, year, eps=500, min_samples=1, max_cluster_size=1000, subcluster_size=50):
    """
    DBSCAN ile kümeleme ve büyük kümeleri alt kümelere bölme işlemi yapar.
    
    Parameters:
    -----------
    builtup_df : pandas.DataFrame
        Satürasyon verilerini içeren DataFrame
    year : int
        Satürasyon hesaplaması yapılacak yıl
    eps : float
        DBSCAN mesafe parametresi (metre)
    min_samples : int
        DBSCAN minimum örnek sayısı
    max_cluster_size : int
        Bir kümenin alt kümelere bölünmesi için maksimum boyutu
    subcluster_size : int
        Alt kümelerin yaklaşık boyutu
        
    Returns:
    --------
    pandas.DataFrame
        Kümelenmiş ve alt kümelenmiş veri çerçevesi
    """
    # Latitude ve longitude sütunları yoksa ekle
    if 'lat' not in builtup_df.columns or 'lon' not in builtup_df.columns:
        builtup_df['lat'] = (builtup_df['top'] + builtup_df['bottom']) / 2
        builtup_df['lon'] = (builtup_df['left'] + builtup_df['right']) / 2

    # Satürasyon sütununu sayısal değere çevir
    saturation_col = f'Saturation_updated_{year}'
    if saturation_col in builtup_df.columns:
        builtup_df[saturation_col] = pd.to_numeric(builtup_df[saturation_col], errors='coerce')
        builtup_df[saturation_col].fillna(0, inplace=True)
    
    # IsDevelopmentArea sütununu kontrol et
    if 'IsDevelopmentArea' not in builtup_df.columns:
        development_cols = [col for col in builtup_df.columns if 'development' in col.lower() or 'area' in col.lower()]
        if development_cols:
            # İlk uygun sütunu kullan
            builtup_df['IsDevelopmentArea'] = builtup_df[development_cols[0]]
            print(f"Warning: 'IsDevelopmentArea' column not found. Using '{development_cols[0]}' instead.")
        else:
            print("Warning: No development area column found. Analysis may not be accurate.")
            # Varsayılan değer oluştur
            builtup_df['IsDevelopmentArea'] = 'Unknown'

    # Kentsel yerleşim alanı olan hücreleri al
    urban_cells = builtup_df[(builtup_df[f'Saturation_updated_{year}'] > 0) &
                             ((builtup_df['IsDevelopmentArea'].str.contains('Kentsel', case=False, na=False)) |
                              (builtup_df['IsDevelopmentArea'].str.contains('İmarl', case=False, na=False)) |
                              (builtup_df['IsDevelopmentArea'].str.contains('Imarl', case=False, na=False)) |
                              (builtup_df['IsDevelopmentArea'].str.contains('İmars', case=False, na=False)) |
                              (builtup_df['IsDevelopmentArea'].str.contains('Imars', case=False, na=False)) |
                              (builtup_df['IsDevelopmentArea'].str.contains('Geniş', case=False, na=False)) |
                              (builtup_df['IsDevelopmentArea'].str.contains('Genis', case=False, na=False)) |
                              (builtup_df['IsDevelopmentArea'].str.contains('Gecici', case=False, na=False)) |
                              (builtup_df['IsDevelopmentArea'].str.contains('Geçici', case=False, na=False)))]

    if urban_cells.empty:
        print(f"Year {year}: No urban cells found with saturation > 0.")
        # Orijinal DataFrame'e 'subcluster' sütunu ekleyerek boş bir urban_cells seti döndür
        builtup_df['subcluster'] = None
        return builtup_df

    # DBSCAN ile kümeleme
    coordinates = urban_cells[['lat', 'lon']].values
    clustering = DBSCAN(eps=eps / 111320, min_samples=min_samples, metric='euclidean').fit(coordinates)
    urban_cells['cluster'] = clustering.labels_

    # Kümeleri yazdır
    cluster_sizes = urban_cells['cluster'].value_counts()
    print("Cluster sizes:")
    for cluster_id, size in cluster_sizes.items():
        if cluster_id == -1:
            print(f"Cluster: Noise (-1), Size: {size}")
        else:
            print(f"Cluster: {cluster_id}, Size: {size}")

    # Büyük kümeleri alt kümelere bölme
    all_subclusters = []
    large_clusters = urban_cells['cluster'].value_counts()
    large_clusters = large_clusters[large_clusters > max_cluster_size].index

    for cluster_id in large_clusters:
        # Büyük küme için hücreleri al
        cluster_points = urban_cells[urban_cells['cluster'] == cluster_id]
        coordinates = cluster_points[['lat', 'lon']].values

        # Alt küme sayısını belirle
        n_clusters = len(cluster_points) // subcluster_size
        if n_clusters < 1:
            n_clusters = 1

        # KMeans ile alt kümeler oluştur
        kmeans = KMeans(n_clusters=n_clusters, random_state=42).fit(coordinates)
        cluster_points['subcluster'] = [f'{cluster_id}_{label}' for label in kmeans.labels_]
        all_subclusters.append(cluster_points)

    # Küçük kümeler olduğu gibi eklenir
    small_clusters = urban_cells[~urban_cells['cluster'].isin(large_clusters)]
    if not small_clusters.empty:
        small_clusters['subcluster'] = small_clusters['cluster'].astype(str)
        all_subclusters.append(small_clusters)

    # Tüm alt kümeleri birleştir
    if all_subclusters:
        return pd.concat(all_subclusters, ignore_index=True)
    else:
        return builtup_df

def update_saturation(builtup_df, year, eps=500, dynamic_eps=200, min_samples=1):
    """
    Belirli bir yıl için satürasyon değerlerini günceller.
    
    Parameters:
    -----------
    builtup_df : pandas.DataFrame
        Satürasyon verilerini içeren DataFrame
    year : int
        Satürasyon hesaplaması yapılacak yıl
    eps : float
        DBSCAN mesafe parametresi (metre)
    dynamic_eps : float
        Küme merkezinden hücreleri bulmak için dinamik mesafe parametresi
    min_samples : int
        DBSCAN minimum örnek sayısı
        
    Returns:
    --------
    pandas.DataFrame
        Güncellenmiş satürasyon değerlerine sahip DataFrame
    """
    # Latitude ve longitude sütunları yoksa ekle
    if 'lat' not in builtup_df.columns or 'lon' not in builtup_df.columns:
        builtup_df['lat'] = (builtup_df['top'] + builtup_df['bottom']) / 2
        builtup_df['lon'] = (builtup_df['left'] + builtup_df['right']) / 2

    # Satürasyon sütununu sayısal değere çevir
    saturation_col = f'Saturation_updated_{year}'
    if saturation_col in builtup_df.columns:
        builtup_df[saturation_col] = pd.to_numeric(builtup_df[saturation_col], errors='coerce')
        builtup_df[saturation_col] = builtup_df[saturation_col].fillna(0)

    # Bir önceki yılın satürasyon değerlerini değişkene ata
    # İlk yıl için özel durum
    if year == 2024:
        # Farklı olası sütun isimlerini kontrol et
        possible_columns = [
            'Saturation_ratio_2023',
            f'Saturation_ratio_{year-1}',
            f'Saturation_{year-1}',
            'Saturation'
        ]
        
        prev_saturation_col = None
        for col in possible_columns:
            if col in builtup_df.columns:
                prev_saturation_col = col
                print(f"İlk yıl için satürasyon sütunu bulundu: {prev_saturation_col}")
                break
                
        if prev_saturation_col is None:
            print("UYARI: İlk yıl için satürasyon sütunu bulunamadı. Sıfır kullanılacak.")
            prev_saturation_col = 'temp_saturation'
            builtup_df[prev_saturation_col] = 0
    else:
        prev_saturation_col = f'Saturation_updated_{year - 1}'
    
    # Önceki yıl sütununu kontrol et ve sayısal değere çevir
    if prev_saturation_col in builtup_df.columns:
        print(f"{prev_saturation_col} sütunu kullanılıyor.")
        builtup_df[prev_saturation_col] = pd.to_numeric(builtup_df[prev_saturation_col], errors='coerce')
        builtup_df[prev_saturation_col] = builtup_df[prev_saturation_col].fillna(0)
    else:
        print(f"UYARI: {prev_saturation_col} sütunu bulunamadı!")
        builtup_df[prev_saturation_col] = 0
        print(f"{prev_saturation_col} sütunu oluşturuldu ve 0 değeri atandı.")

    # DBSCAN ve alt kümeleme
    clustered_cells = cluster_with_dbscan_and_split(builtup_df, year, eps=eps, min_samples=min_samples)

    # Eğer alt kümeler bulunamadıysa (No urban cells with saturation > 0)
    if 'subcluster' not in clustered_cells.columns:
        print(f"No subclusters found for year {year}. Skipping subcluster processing.")
        return builtup_df

    # Her alt küme için işlem yap
    for subcluster_id in clustered_cells['subcluster'].unique():
        cluster_cells = clustered_cells[clustered_cells['subcluster'] == subcluster_id]

        # Küme merkezi hesapla
        cluster_center = cluster_cells[['lat', 'lon']].mean().values

        # Dinamik EPS ile küme merkezinden belirli mesafedeki hücreleri bul
        distances = haversine(cluster_center[0], cluster_center[1], builtup_df['lat'], builtup_df['lon'])
        nearby_cells = builtup_df[(distances <= dynamic_eps)]

        new_count = 0
        for _, cell in nearby_cells.iterrows():
            # Saturasyonu olmayan yeni genişleme alanları
            if cell['IsDevelopmentArea'] in ['İmarlı Yeni Genişleme Alanı', 'İmarsız Yeni Genişleme Bölgesi', 'İmarsız yeni genisleme alanı'] and \
                    cell[f'Saturation_updated_{year}'] == 0:
                # Ortalama saturasyon hesapla
                cell_center = (cell['lat'], cell['lon'])
                distances_from_cell = haversine(cell_center[0], cell_center[1], builtup_df['lat'], builtup_df['lon'])
                valid_neighbors = builtup_df[
                    (distances_from_cell <= eps) &
                    (builtup_df['IsDevelopmentArea'].isin(['Kentsel Yerleşim Alanı', 'Geçici Kentsel Alan']))
                ]
                if not valid_neighbors.empty:
                    nearby_saturation = valid_neighbors[f'Saturation_updated_{year}'].mean()
                    if 'İmarsız' in cell['IsDevelopmentArea']:
                        nearby_saturation /= 2  # İmarsız bölgelerde saturasyon yarıya düşer
                    builtup_df.loc[cell.name, f'Saturation_updated_{year}'] = nearby_saturation
                    new_count += 1
        
        if new_count > 0:
            print(f"Küme {subcluster_id}: {new_count} hücre için satürasyon hesaplandı.")
        
    # Geçici Kentsel Alan, İmarlı ve İmarsız Genişleme Bölgelerinden saturasyonu olan hücreleri seç
    eligible_cells = builtup_df[
        ((builtup_df['IsDevelopmentArea'].str.contains('Geçici', case=False, na=False)) |
         (builtup_df['IsDevelopmentArea'].str.contains('Gecici', case=False, na=False)) |
         (builtup_df['IsDevelopmentArea'].str.contains('İmarl', case=False, na=False)) |
         (builtup_df['IsDevelopmentArea'].str.contains('Imarl', case=False, na=False)) |
         (builtup_df['IsDevelopmentArea'].str.contains('İmars', case=False, na=False)) |
         (builtup_df['IsDevelopmentArea'].str.contains('Imars', case=False, na=False)) |
         (builtup_df['IsDevelopmentArea'].str.contains('Geniş', case=False, na=False)) |
         (builtup_df['IsDevelopmentArea'].str.contains('Genis', case=False, na=False))) &
        (builtup_df[f'Saturation_updated_{year}'] > 0) & (builtup_df[f'Saturation_updated_{year}'] <= 0.85)
    ]
    eligible_count = len(eligible_cells)
    
    # Geliştirme alanı değerlerini kontrol et ve yazdır (ilk 10 farklı değer)
    unique_dev_areas = builtup_df['IsDevelopmentArea'].unique()
    print(f"Veri setindeki benzersiz geliştirme alanı türleri: {unique_dev_areas[:10]}")
    
    # Saturasyon değerlerinin dağılımını kontrol et
    sat_counts = builtup_df[f'Saturation_updated_{year}'].value_counts().sort_index()
    print(f"Saturasyon değerlerinin dağılımı (ilk 5): {sat_counts.head()}")

    # Sabit 300 hücre seçimi (ya da tüm uygun hücreleri seç sayı 300'den azsa)
    if eligible_count <= 300:
        # Tüm uygun hücreleri seç
        update_indices = list(eligible_cells.index)
        update_count = eligible_count
        print(f"Updating all {eligible_count} eligible cells (below 300 limit)")
    else:
        # 300 hücre random seçim yap
        update_indices = random.sample(list(eligible_cells.index), 300)
        exact_count = len(update_indices)
        print(f"Randomly exact selected {exact_count}")
        update_count = 300
        print(f"Randomly selected {update_count} cells out of {eligible_count} eligible cells")
                  
    # Seçilen hücrelerin saturasyon değerlerini güncelle
    for idx in update_indices:
        cell = builtup_df.loc[idx]
        current_saturation = cell[f'Saturation_updated_{year}']
        increase = random.uniform(0.05, 0.15)
        new_saturation = min(1, current_saturation + increase)
        builtup_df.loc[idx, f'Saturation_updated_{year}'] = new_saturation
    
    # Geçici kentsel alan etiketi işlemi
    print(f"\nGeçici kentsel alan etiketleme başlatılıyor... (Yıl: {year})")
    
    # Geliştirme alanı filtresi oluştur
    gelistirme_alani_filtresi = (
        builtup_df['IsDevelopmentArea'].str.contains('İmarl', case=False, na=False) | 
        builtup_df['IsDevelopmentArea'].str.contains('Imarl', case=False, na=False) |
        builtup_df['IsDevelopmentArea'].str.contains('İmars', case=False, na=False) |
        builtup_df['IsDevelopmentArea'].str.contains('Imars', case=False, na=False) |
        builtup_df['IsDevelopmentArea'].str.contains('Geniş', case=False, na=False) |
        builtup_df['IsDevelopmentArea'].str.contains('Genis', case=False, na=False)
    )
    
    # Satürasyon filtresi oluştur
    saturation_filtresi = (
        (builtup_df[prev_saturation_col] == 0) & 
        (builtup_df[f'Saturation_updated_{year}'] > 0)
    )
    
    # Filtreleri birleştir
    gecici_kentsel_alan_filtresi = gelistirme_alani_filtresi & saturation_filtresi
    
    # Filtreleme bilgisi
    print(f"Geliştirme alanı filtresine uyan hücre sayısı: {sum(gelistirme_alani_filtresi)}")
    print(f"Satürasyon filtresine uyan hücre sayısı: {sum(saturation_filtresi)}")
    print(f"Geçici kentsel alan olarak işaretlenecek hücre sayısı: {sum(gecici_kentsel_alan_filtresi)}")
    
    # Filtreyi uygula
    builtup_df.loc[gecici_kentsel_alan_filtresi, 'IsDevelopmentArea'] = 'Geçici Kentsel Alan'
    
    # Güncellenen hücre sayısı
    guncellenen_hucre_sayisi = sum(gecici_kentsel_alan_filtresi)
    print(f"Geçici Kentsel Alan olarak güncellenen hücre sayısı: {guncellenen_hucre_sayisi}")
    
    # İşlem sonrası IsDevelopmentArea sütunundaki benzersiz değerleri kontrol et
    print(f"İşlem sonrası IsDevelopmentArea sütunundaki benzersiz değerler:")
    for alan_tipi, sayisi in builtup_df['IsDevelopmentArea'].value_counts().items():
        print(f"  {alan_tipi}: {sayisi}")

    return builtup_df

def run_saturation_updates(builtup_df, start_year=2024, end_year=2035, initial_dynamic_eps=400, eps_increment=0):
    """
    Birden çok yıl için satürasyon güncelleme sürecini çalıştırır.
    
    Parameters:
    -----------
    builtup_df : pandas.DataFrame
        Satürasyon verilerini içeren DataFrame
    start_year : int
        Satürasyon hesaplaması başlangıç yılı
    end_year : int
        Satürasyon hesaplaması bitiş yılı
    initial_dynamic_eps : int
        DBSCAN kümelemesi için başlangıç eps parametresi
    eps_increment : int
        Her yıl için eps artış değeri
        
    Returns:
    --------
    pandas.DataFrame
        Tüm yıllar için güncellenmiş satürasyon değerlerine sahip DataFrame
    """
    # İlk yıl için satürasyon sütunu yoksa mevcut Saturation sütunundan kopyala
    if f'Saturation_updated_{start_year}' not in builtup_df.columns:
        # 2023 satürasyon oranını sayısal değere çevir
        if 'Saturation_ratio_2023' in builtup_df.columns:
            # Eğer Saturation_ratio_2023 sütunu varsa, onu kullan
            builtup_df['Saturation_ratio_2023'] = pd.to_numeric(builtup_df['Saturation_ratio_2023'], errors='coerce')
            builtup_df['Saturation_ratio_2023'].fillna(0, inplace=True)
            builtup_df[f'Saturation_updated_{start_year}'] = builtup_df['Saturation_ratio_2023']
        elif 'Saturation' in builtup_df.columns:
            # Eğer Saturation sütunu varsa, onu kullan
            builtup_df['Saturation'] = pd.to_numeric(builtup_df['Saturation'], errors='coerce')
            builtup_df['Saturation'].fillna(0, inplace=True)
            builtup_df[f'Saturation_updated_{start_year}'] = builtup_df['Saturation']
        else:
            # İlk değer olarak 0 atama
            print(f"Warning: Neither 'Saturation_ratio_2023' nor 'Saturation' columns found. Setting initial saturation to 0.")
            builtup_df[f'Saturation_updated_{start_year}'] = 0
    
    dynamic_eps = initial_dynamic_eps  # Dinamik EPS başlangıç değeri
    for year in range(start_year, end_year + 1):
        print(f"\nProcessing saturation updates for year {year} with dynamic_eps={dynamic_eps} meters...")

        # Yıla göre dinamik eps değerini ayarla
        if year == 2024:
            dynamic_eps = 350
        elif year < 2028:
            dynamic_eps = 350
            dynamic_eps += 20*(year-2025)
        elif year <= 2030:
            dynamic_eps = 380
            dynamic_eps += 10*(year-2028)
        else:
            dynamic_eps = 400
            dynamic_eps += 5*(year-2031)

        # Update saturation işlemini çağır
        builtup_df = update_saturation(builtup_df, year, eps=500, dynamic_eps=dynamic_eps)

        # Update sonrası, bir sonraki yılın sütununu doldur (son yıl değilse)
        if year < end_year:
            builtup_df[f'Saturation_updated_{year + 1}'] = builtup_df[f'Saturation_updated_{year}']

    return builtup_df

def parse_arguments():
    """Komut satırı argümanlarını ayrıştırır."""
    parser = argparse.ArgumentParser(description='Satürasyon Güncelleme İşlemi')
    
    # Gerekli argümanlar
    parser.add_argument('input_file', type=str, help='Satürasyon verilerini içeren dosyanın yolu')
    parser.add_argument('city', type=str, help='Şehir adı')
    parser.add_argument('district', type=str, help='İlçe adı')
    parser.add_argument('output_dir', type=str, help='Sonuçların kaydedileceği dizin yolu')
    
    # İsteğe bağlı argümanlar
    parser.add_argument('--start-year', type=int, default=2024, help='Analiz başlangıç yılı')
    parser.add_argument('--end-year', type=int, default=2035, help='Analiz bitiş yılı')
    parser.add_argument('--initial-dynamic-eps', type=int, default=400, help='Başlangıç dynamic EPS değeri')
    parser.add_argument('--eps-increment', type=int, default=0, help='Yıllık EPS artış değeri')
    
    return parser.parse_args()

def main():
    """Ana fonksiyon."""
    print("Starting SLF Analysis...")
    
    # Komut satırı argümanlarını ayrıştır
    args = parse_arguments()
    
    # Argümanları logla
    print(f"Arguments received:")
    print(f"  Saturation File: {args.input_file}")
    print(f"  City: {args.city}")
    print(f"  District: {args.district}")
    print(f"  Output Directory: {args.output_dir}")
    print(f"  Analysis Period: {args.start_year} - {args.end_year}")
    
    # Dosya yollarını absolute path'e çevir
    input_file_abs = os.path.abspath(args.input_file)
    output_dir_abs = os.path.abspath(args.output_dir)
    
    print(f"Absolute paths:")
    print(f"  Saturation File: {input_file_abs}")
    print(f"  Output Directory: {output_dir_abs}")
    
    # Girdi dosyasının varlığını kontrol et
    if not os.path.exists(input_file_abs):
        raise FileNotFoundError(f"Saturation file not found: {input_file_abs}")
    
    # Çıktı dizininin varlığını kontrol et, yoksa oluştur
    if not os.path.exists(output_dir_abs):
        os.makedirs(output_dir_abs)
        print(f"Output directory created: {output_dir_abs}")
    
    # Satürasyon verilerini yükle
    print(f"Using saturation file: {input_file_abs}")
    print(f"Loading saturation data from file: {input_file_abs}")
    try:
        # Dosya uzantısına göre yükleme metodunu belirle
        file_ext = os.path.splitext(input_file_abs)[1].lower()
        if file_ext == '.csv':
            builtup_df = pd.read_csv(input_file_abs)
        elif file_ext in ['.xlsx', '.xls']:
            builtup_df = pd.read_excel(input_file_abs, sheet_name='builtup')
        else:
            raise ValueError(f"Unsupported file format: {file_ext}")
        
        print(f"Loaded data with {len(builtup_df)} rows and {len(builtup_df.columns)} columns")
        
        # IsDevelopmentArea sütunundaki değerleri kontrol et
        if 'IsDevelopmentArea' in builtup_df.columns:
            unique_areas = builtup_df['IsDevelopmentArea'].unique()
            print(f"Unique development area types found: {unique_areas}")
        else:
            print("Warning: 'IsDevelopmentArea' column not found in the data!")
            potential_columns = [col for col in builtup_df.columns if 'area' in col.lower() or 'development' in col.lower()]
            if potential_columns:
                print(f"Possible similar columns: {potential_columns}")
        
        # Satürasyon güncellemelerini çalıştır
        print(f"Running saturation updates for years {args.start_year} to {args.end_year}...")
        builtup_df = run_saturation_updates(
            builtup_df, 
            start_year=args.start_year, 
            end_year=args.end_year, 
            initial_dynamic_eps=args.initial_dynamic_eps,
            eps_increment=args.eps_increment
        )
        
        # Sonuçları kaydet
        output_file = os.path.join(output_dir_abs, f"saturation_updated_{args.city}_{args.district}_{args.start_year}-{args.end_year}.xlsx")
        print(f"Saving results to: {output_file}")
        builtup_df.to_excel(output_file, index=False)
        
        print("SLF Analysis completed successfully!")
        print(f"Results saved to: {output_file}")
        
        return 0
    
    except Exception as e:
        print(f"Error in SLF analysis: {str(e)}")
        import traceback
        traceback.print_exc()
        # İstisnayı C# kodunun yakalayabilmesi için yeniden fırlat
        raise

if __name__ == "__main__":
    sys.exit(main())