import pandas as pd
from sklearn.cluster import DBSCAN
from sklearn.cluster import KMeans
import numpy as np
import random

def haversine(lat1, lon1, lat2, lon2):
    R = 6371  # Dünya yarıçapı (km)
    lat1, lon1, lat2, lon2 = map(np.radians, [lat1, lon1, lat2, lon2])
    dlat = lat2 - lat1
    dlon = lon2 - lon1
    a = np.sin(dlat / 2) ** 2 + np.cos(lat1) * np.cos(lat2) * np.sin(dlon / 2) ** 2
    return 2 * R * np.arcsin(np.sqrt(a)) * 1000  # Metre cinsinden dönüş

# DBSCAN ile kümeleme ve alt kümelere bölme

def cluster_with_dbscan_and_split(builtup_df, year, eps=500, min_samples=1, max_cluster_size=1000, subcluster_size=50):
    # Latitude ve longitude sütunları yoksa ekle
    if 'lat' not in builtup_df.columns or 'lon' not in builtup_df.columns:
        builtup_df['lat'] = (builtup_df['top'] + builtup_df['bottom']) / 2
        builtup_df['lon'] = (builtup_df['left'] + builtup_df['right']) / 2

    # Kentsel yerleşim alanı olan hücreleri al
    urban_cells = builtup_df[(builtup_df[f'Saturation_updated_{year}'] > 0) &
                             ((builtup_df['IsDevelopmentArea'] == 'Kentsel Yerleşim Alanı') |
                              (builtup_df['IsDevelopmentArea'] == 'İmarlı Yeni Genişleme Bölgesi') |
                              (builtup_df['IsDevelopmentArea'] == 'İmarsız Yeni Genişleme Bölgesi') |
                              (builtup_df['IsDevelopmentArea'] == 'Geçici Kentsel Alan'))]

    if urban_cells.empty:
        print(f"Year {year}: No urban cells found with saturation > 0.")
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

# Saturasyon güncelleme fonksiyonu

def update_saturation(builtup_df, year, eps=500, dynamic_eps=200, min_samples=1):
    # Latitude ve longitude sütunları yoksa ekle
    if 'lat' not in builtup_df.columns or 'lon' not in builtup_df.columns:
        builtup_df['lat'] = (builtup_df['top'] + builtup_df['bottom']) / 2
        builtup_df['lon'] = (builtup_df['left'] + builtup_df['right']) / 2

    # DBSCAN ve alt kümeleme
    clustered_cells = cluster_with_dbscan_and_split(builtup_df, year, eps=eps, min_samples=min_samples)

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
            if cell['IsDevelopmentArea'] in ['İmarlı Yeni Genişleme Bölgesi', 'İmarsız Yeni Genişleme Bölgesi'] and \
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
                    if cell['IsDevelopmentArea'] == 'İmarsız Yeni Genişleme Bölgesi':
                        nearby_saturation /= 2  # İmarsız bölgelerde saturasyon yarıya düşer
                    builtup_df.loc[cell.name, f'Saturation_updated_{year}'] = nearby_saturation
                    new_count += 1
        
    # Geçici Kentsel Alan, İmarlı ve İmarsız Genişleme Bölgelerinden saturasyonu olan hücreleri seç
    eligible_cells = builtup_df[
        (builtup_df['IsDevelopmentArea'].isin(['Geçici Kentsel Alan', 'İmarlı Yeni Genişleme Bölgesi', 'İmarsız Yeni Genişleme Bölgesi'])) &
        (builtup_df[f'Saturation_updated_{year}'] > 0) & (builtup_df[f'Saturation_updated_{year}'] <= 0.85)
    ]
    eligible_count = len(eligible_cells)

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
    
    # Geçici kentsel alan etiketi işlemi: Saturasyonu hesaplanmış alanlar için
    builtup_df.loc[
        (builtup_df['IsDevelopmentArea'].isin(['İmarlı Yeni Genişleme Bölgesi', 'İmarsız Yeni Genişleme Bölgesi'])) &
        (builtup_df[f'Saturation_updated_{year - 1}'] == 0) &
        (builtup_df[f'Saturation_updated_{year}'] > 0),
        'IsDevelopmentArea'
    ] = 'Geçici Kentsel Alan'

    return builtup_df

# Saturasyon güncellemelerini çalıştırma
def run_saturation_updates(builtup_df, start_year=2024, end_year=2035, initial_dynamic_eps=400, eps_increment=0):
    dynamic_eps = initial_dynamic_eps  # Dinamik EPS başlangıç değeri
    for year in range(start_year, end_year + 1):
        print(f"\nProcessing saturation updates for year {year} with dynamic_eps={dynamic_eps} meters...")

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

        # Update sonrası, bir sonraki yılın sütununu doldur
        builtup_df[f'Saturation_updated_{year + 1}'] = builtup_df[f'Saturation_updated_{year}']

        # Dinamik EPS'yi artır
        #dynamic_eps += eps_increment
        #eps_increment -= 0

    return builtup_df

# Girdi dosyasını yükle
builtup_df = pd.read_excel('C:\\Users\\arda.sari\\Desktop\\SLF PROJE\\SLF_hesabı\\update_saturation_deneme3.xlsx', sheet_name='builtup')

# Saturasyon güncellemelerini çalıştır
builtup_df = run_saturation_updates(builtup_df, start_year=2024, end_year=2035)

# Çıktıyı kaydet
output_path = 'C:\\Users\\arda.sari\\Desktop\\SLF PROJE\\SLF_hesabı\\saturation_update_results_29-versiyon.xlsx'
builtup_df.to_excel(output_path, index=False)
print(f"Results saved to {output_path}")
