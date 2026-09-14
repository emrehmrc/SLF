import pandas as pd
import numpy as np
import os
import sys
import json
import random
from sklearn.cluster import DBSCAN, KMeans
from scipy.spatial import cKDTree
import sqlite3
import time
from joblib import Parallel, delayed
import sqlite3
import warnings
warnings.filterwarnings("ignore")



# ----------------- Yardımcı Fonksiyonlar -----------------

def haversine(lat1, lon1, lat2, lon2):
    R = 6371  # km
    lat1, lon1, lat2, lon2 = map(np.radians, [lat1, lon1, lat2, lon2])
    dlat = lat2 - lat1
    dlon = lon2 - lon1
    a = np.sin(dlat/2)**2 + np.cos(lat1)*np.cos(lat2)*np.sin(dlon/2)**2
    return 2 * R * np.arcsin(np.sqrt(a)) * 1000  # metre


# ------------------------------------------------------------ 1 - Saturasyon --------------------------------------------------------------------------------------#


# ----------------- Kümeleme + Altkümelere Bölme -----------------

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
        print(f"Yıl {year}: Satürasyon oranı >0 olan hiçbir kentsel hücre bulunamadı.")
        return builtup_df

    # DBSCAN ile kümeleme
    coordinates = urban_cells[['lat', 'lon']].values
    clustering = DBSCAN(eps=eps / 111320, min_samples=min_samples, metric='euclidean').fit(coordinates)
    urban_cells['cluster'] = clustering.labels_

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



# ----------------- Saturasyon Güncelleme -----------------

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
        (builtup_df['IsDevelopmentArea'].isin(['Geçici Kentsel Alan', 'İmarlı Yeni Genişleme Bölgesi', 'İmarsız Yeni Genişleme Bölgesi','Kentsel Yerleşim Alanı','Kent-Dışı Alan'])) &
        (builtup_df[f'Saturation_updated_{year}'] > 0) 
    ]
    eligible_count = len(eligible_cells)

    # Sabit 500 hücre seçimi (ya da tüm uygun hücreleri seç sayı 500'den azsa)
    if eligible_count <= 500:
        # Tüm uygun hücreleri seç
        update_indices = list(eligible_cells.index)
        update_count = eligible_count
        print(f"Bütün {eligible_count} adet uygun hücreler güncelleniyor.")
    else:
        # 500 hücre random seçim yap
        update_indices = random.sample(list(eligible_cells.index), 500)
        exact_count = len(update_indices)
        update_count = 500
                  
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



# ----------------- Tüm Yıllar İçin Loop -----------------
# Saturasyon güncellemelerini çalıştırma
def run_saturation_updates(builtup_df, start_year, end_year, initial_dynamic_eps=400):
    dynamic_eps = initial_dynamic_eps  # Dinamik EPS başlangıç değeri
    for year in range(start_year, end_year + 1):
        print(f"\nYıl {year} için satürasyon değerleri güncelleniyor.  Bakılan uzaklık: {dynamic_eps} metre...")

        if year == start_year:
            dynamic_eps = 200
        elif year < start_year+4:
            dynamic_eps = 200
            dynamic_eps += 20*(year-(start_year+1))
        elif year <= start_year+6:
            dynamic_eps = 250
            dynamic_eps += 10*(year-(start_year+4))
        else:
            dynamic_eps = 280
            dynamic_eps += 10*(year-(start_year+7))

        # Update saturation işlemini çağır
        builtup_df = update_saturation(builtup_df, year, eps=500, dynamic_eps=dynamic_eps)

        # Update sonrası, bir sonraki yılın sütununu doldur
        builtup_df[f'Saturation_updated_{year + 1}'] = builtup_df[f'Saturation_updated_{year}']

    return builtup_df





# ------------------------------------------------------------ 2 - İmar Oranı Tahminleri --------------------------------------------------------------------------------------#


# Haversine fonksiyonu (vektörize)
def haversine_np(lat1, lon1, lat2, lon2):
    R = 6371  # Dünya yarıçapı (km)
    lat1, lon1, lat2, lon2 = map(np.radians, [lat1, lon1, lat2, lon2])
    dlat = lat2 - lat1
    dlon = lon2 - lon1
    a = np.sin(dlat / 2) ** 2 + np.cos(lat1) * np.cos(lat2) * np.sin(dlon / 2) ** 2
    return 2 * R * np.arcsin(np.sqrt(a)) * 1000  # Metre cinsinden dönüş


# Filtreleme fonksiyonu
def filter_cells_for_year(builtup_df, zoning_df, year, restricted=0.85):
    saturation_col = f'Saturation_updated_{year}'
    next_saturation_col = f'Saturation_updated_{year + 1}'  # Use next year's saturation
    builtup_increase = builtup_df[next_saturation_col] - builtup_df[saturation_col]
    
    # Debugging: Check each condition
    condition1 = builtup_increase != 0
    condition2 = builtup_df[saturation_col] < 1
    condition3 = builtup_df['yasakli_alan_percentage'] < restricted
    condition4 = builtup_df['id'].isin(zoning_df[zoning_df['is_calculated'] == 0]['id'])
    
    print(f" {year} Yılı için özet değerler:")
    print(f"  Uydu verileri ile satürasyon büyümesi olacağı tespit edilen hücre sayısı (hesap mantığı: {next_saturation_col} - {saturation_col}): {condition1.sum()}")
    print(f"  Satürasyonu < 1 olan hücre sayısı: {condition2.sum()}")
    print(f"  Yasaklı alan oranı < {restricted} olan hücre sayısı: {condition3.sum()}")
    print(f"  Tüm koşulları sağlayan nihai hücre sayısı: {(condition1 & condition2 & condition3 & condition4).sum()}")
    
    return builtup_df[
        condition1 & 
        condition2 & 
        condition3 & 
        condition4
    ]


# Komşuluk hesaplama (cKDTree kullanımı)
def find_neighbors(builtup_df, center_lat, center_lon, radius):
    coords = np.radians(builtup_df[['lat', 'lon']].values)
    tree = cKDTree(coords)
    center_coords = np.radians([center_lat, center_lon])
    indices = tree.query_ball_point(center_coords, radius / 6371000)  # Radius in radians
    return indices


def calculate_weighted_average_for_cell(cell, builtup_df, zoning_df, year):
    center_lat = cell['lat']
    center_lon = cell['lon']
    
    neighbor_indices = find_neighbors(builtup_df, center_lat, center_lon, radius=500)
    neighbors = builtup_df.iloc[neighbor_indices]
    
    valid_neighbors = []
    for _, neighbor in neighbors.iterrows():
        neighbor_ratios = zoning_df.loc[zoning_df['id'] == neighbor['id'], zoning_df.columns[1:-1]]
        if neighbor_ratios.empty:
            continue
        neighbor_ratios_sum = neighbor_ratios.sum(axis=1).values[0]
        saturation = neighbor[f'Saturation_updated_{year}']
        is_valid = (
            abs(neighbor_ratios_sum - 1) < 1e-6 and
            saturation > 0 and
            neighbor['id'] != cell['id']
        )
        if is_valid:
            valid_neighbors.append(neighbor)
    
    if not valid_neighbors:
        return None
    
    weights = []
    for neighbor in valid_neighbors:
        distance = haversine_np(center_lat, center_lon, neighbor['lat'], neighbor['lon'])
        weight = neighbor[f'Saturation_updated_{year}'] / distance if distance > 0 else 0
        if weight > 0:
            weights.append((neighbor['id'], weight))
    
    if not weights:
        return None
    
    weighted_ratios = {}
    for column in zoning_df.columns[1:-1]:
        numerator = sum(
            weight * zoning_df.loc[zoning_df['id'] == neighbor_id, column].values[0]
            for neighbor_id, weight in weights
            if not zoning_df.loc[zoning_df['id'] == neighbor_id, column].empty
        )
        denominator = sum(weight for _, weight in weights)
        weighted_ratios[column] = numerator / denominator if denominator != 0 else 0
    
    return {'id': cell['id'], **weighted_ratios}


def calculate_weighted_average_parallel(filtered_cells, builtup_df, zoning_df, year, n_jobs=-1):
    
    results = Parallel(n_jobs=n_jobs)(
        delayed(calculate_weighted_average_for_cell)(cell, builtup_df, zoning_df, year)
        for _, cell in filtered_cells.iterrows()
    )
    # None olanları çıkar
    results = [res for res in results if res is not None]
    return results



def process_cell(cell, calculated_ratios_dict, zoning_df_indexed, zoning_cols):
    cell_id = cell['id']
    base_data = {
        'id': cell_id,
        'left': cell['left'],
        'top': cell['top'],
        'right': cell['right'],
        'bottom': cell['bottom']
    }
    if cell_id in calculated_ratios_dict:
        updated_ratios = calculated_ratios_dict[cell_id]
        base_data.update(updated_ratios)
    else:
        existing_ratios = zoning_df_indexed.loc[cell_id, zoning_cols].to_dict()
        base_data.update(existing_ratios)
    
    return base_data



def create_output_optimized(builtup_df, zoning_df, calculated_ratios):
    # zoning_df'yi id ile indexle
    zoning_indexed = zoning_df.set_index('id')
    zoning_cols = zoning_df.columns[1:-1]
    
    # builtup_df ile zoning_df'yi join et (left join)
    merged_df = builtup_df.merge(zoning_indexed[zoning_cols], left_on='id', right_index=True, how='left')
    
    if not calculated_ratios:
        return merged_df
    
    # calculated_ratios listesini DataFrame yapıp id'yi index yap
    calculated_df = pd.DataFrame(calculated_ratios).set_index('id')
    
    # merged_df ile calculated_df'yi join et, calculated_df sütunlarına _calc suffix'i ver
    merged_df = merged_df.merge(calculated_df, left_on='id', right_index=True, how='left', suffixes=('', '_calc'))
    
    # calculated_ratios varsa onları kullan, yoksa zoning_df değerlerini bırak
    for col in zoning_cols:
        merged_df[col] = merged_df[f'{col}_calc'].combine_first(merged_df[col])
    
    # hesaplama sütunlarının kopyalarını at
    drop_cols = [f'{col}_calc' for col in zoning_cols if f'{col}_calc' in merged_df.columns]
    merged_df.drop(columns=drop_cols, inplace=True)
    
    return merged_df

def SqlKaydet(df, table_name):
    
    conn = sqlite3.connect(os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                          'sonuclar/SLF Sonuçları/2.İmar Oranı Tahminleri/Çıktı/', 
                          f"{start_year}-{end_year}_veritabanı.db"))
    
    df.to_sql(table_name, conn, if_exists='replace', index=False)
    
    conn.close()


if __name__ == "__main__":

    config_path = sys.argv[1]
    #config_path = r"C:\Users\vural.bayrakli\Desktop\config.json"

    user_home = os.path.expanduser('~')
    with open(config_path, 'r', encoding='utf-8') as f:
        config = json.load(f)

    ana_klasor_yolu = config['Ana_Klasör_Yolu']
    il               = config['İl']
    ilce             = config['İlçe']
    proje_ismi       = config['proje_ismi']
    start_year       = config['baslangıc_yılı'] 
    end_year         = config['bitis_yılı']


    print("Ufuk yıllarına dair imar verileri kullanılarak hücre bazında satürasyon tahminleri yapılmaya başlandı.....")
    time.sleep(3)
    
    # ----------------- Girdi Oku ve Ön İşlemler -----------------
    # Construct the file path
    saturasyon_df_path = os.path.join(
        user_home, ana_klasor_yolu, il, ilce, proje_ismi,
        'imar_analizi_sonuclari/imar_planlari/saturasyon/saturasyon_sonuc/saturasyon.csv'
    )

    builtup_df = pd.read_csv(saturasyon_df_path)

    # Eksik her yıl sütununu ekle ve sayısal yap
    for y in range(start_year - 1, end_year + 2):  # Changed to end_year + 2 to include 2036
        col = f"Saturation_updated_{y}"
        if col not in builtup_df.columns:
            builtup_df[col] = 0
        builtup_df[col] = pd.to_numeric(builtup_df[col], errors='coerce').fillna(0)


    # ----------------- Saturasyon Güncellemeleri -----------------
    builtup_df = run_saturation_updates(builtup_df, start_year, end_year)


    # ----------------- Sonucu Yaz -----------------

    os.makedirs(os.path.join(
        user_home, ana_klasor_yolu, il, ilce, proje_ismi,
        'sonuclar/SLF Sonuçları/1.Saturasyon/Çıktı'),exist_ok=True)


    output_path = os.path.join(
        user_home, ana_klasor_yolu, il, ilce, proje_ismi,
        'sonuclar/SLF Sonuçları/1.Saturasyon/Çıktı/saturation_guncellenmis.xlsx'
    )

    # Define the fixed columns to keep
    fixed_columns = ['id', 'left', 'top', 'right', 'bottom', 'cell_area', 'yasakli_alan_percentage', 'IsDevelopmentArea', 'lat', 'lon']

    # Identify all Saturation_updated_{y} columns
    saturation_columns = [col for col in builtup_df.columns if col.startswith('Saturation_updated_')]

    # Combine fixed columns and saturation columns
    columns_to_export = fixed_columns + saturation_columns

    # Filter the dataframe to include only the desired columns
    filtered_df = builtup_df[columns_to_export]

    # Export the filtered dataframe to Excel
    filtered_df.to_excel(output_path, index=False)
    
    os.makedirs(os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                              'sonuclar/SLF Sonuçları/2.İmar Oranı Tahminleri/Çıktı'),exist_ok=True)

    # Girdi dosyalarını yükle
    builtup_df = pd.read_excel(output_path)
    imar_oranı_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                  'imar_analizi_sonuclari/imar_planlari/saturasyon/girdiler/imar_orani_as_is.xlsx')
    imar_oranı_df = pd.read_excel(imar_oranı_path)
    
    for year in range(start_year, end_year + 1):  
        print(f"\nİlgili Yıl: {year}..")
        first_start = time.time()
        filtered_cells = filter_cells_for_year(builtup_df, imar_oranı_df, year)
        start = time.time()
        
        if filtered_cells.empty:
            print(f"{year} yılı için herhangi bir yıl değeri bulunamadı. Halihazırda bulunan imar oranları kullanılıyor...")
            # Use existing zoning ratios without updating
            output_df2 = builtup_df.merge(
                imar_oranı_df.set_index('id')[imar_oranı_df.columns[1:-1]],
                left_on='id',
                right_index=True,
                how='left'
            )
        else:
            calculated_ratios = calculate_weighted_average_parallel(filtered_cells, builtup_df, imar_oranı_df, year)
            end = time.time()
            start2 = time.time()
            output_df2 = create_output_optimized(builtup_df, imar_oranı_df, calculated_ratios)
            end = time.time()
            # Update is_calculated for processed cells
            imar_oranı_df.loc[imar_oranı_df['id'].isin(filtered_cells['id']), 'is_calculated'] = 1

        SqlKaydet(output_df2, f"{str(year)}")
        end = time.time()

