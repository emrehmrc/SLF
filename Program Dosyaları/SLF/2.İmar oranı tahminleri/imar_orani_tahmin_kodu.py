import pandas as pd
import numpy as np
from scipy.spatial import cKDTree

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
    builtup_increase = builtup_df[saturation_col] - builtup_df[f'Saturation_updated_{year - 1}']
    
    # Sadece is_calculated = 0 olan hücreleri al
    return builtup_df[
        (builtup_increase != 0) & 
        (builtup_df[saturation_col] < 1) & 
        (builtup_df['yasakli_alan_percentage'] < restricted) &
        builtup_df['id'].isin(zoning_df[zoning_df['is_calculated'] == 0]['id'])
    ]

# Komşuluk hesaplama (cKDTree kullanımı)
def find_neighbors(builtup_df, center_lat, center_lon, radius):
    coords = np.radians(builtup_df[['lat', 'lon']].values)
    tree = cKDTree(coords)
    center_coords = np.radians([center_lat, center_lon])
    indices = tree.query_ball_point(center_coords, radius / 6371000)  # Radius in radians
    return indices

# Ağırlıklı ortalama hesaplama
def calculate_weighted_average(filtered_cells, builtup_df, zoning_df, year):
    results = []
    for _, cell in filtered_cells.iterrows():
        center_lat = cell['lat']
        center_lon = cell['lon']
        
        # 500 metre mesafedeki komşuları bul
        neighbor_indices = find_neighbors(builtup_df, center_lat, center_lon, radius=500)
        neighbors = builtup_df.iloc[neighbor_indices]

        # Komşuları filtrele: zoning_df oranlarının toplamı 1 olmalı ve saturasyon oranı > 0
        valid_neighbors = []
        for _, neighbor in neighbors.iterrows():
            neighbor_ratios_sum = zoning_df.loc[zoning_df['id'] == neighbor['id'], zoning_df.columns[2:-1]].sum(axis=1).values[0]
            
            if (
                abs(neighbor_ratios_sum - 1) < 1e-6 and  # Zoning oranlarının toplamı yaklaşık 1
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
            weight = neighbor[f'Saturation_updated_{year}'] / distance if distance > 0 else 0
            weights.append((neighbor['id'], weight))

        if not weights:
            continue
        
        # Ağırlıklı ortalama oranlarını hesapla
        weighted_ratios = {}
        for column in zoning_df.columns[2:-1]:
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
    output_rows = []
    for _, cell in builtup_df.iterrows():
        if cell['id'] in [ratio['id'] for ratio in calculated_ratios]:
            updated_ratios = next(ratio for ratio in calculated_ratios if ratio['id'] == cell['id'])
            output_rows.append({'id': cell['id'], 'left': cell['left'], 'top': cell['top'],
                                'right': cell['right'], 'bottom': cell['bottom'],'ilce': cell['ilce'], **updated_ratios})
        else:
            existing_ratios = {col: zoning_df.loc[zoning_df['id'] == cell['id'], col].values[0] for col in zoning_df.columns[2:-1]}
            output_rows.append({'id': cell['id'], 'left': cell['left'], 'top': cell['top'],
                                'right': cell['right'], 'bottom': cell['bottom'],'ilce': cell['ilce'], **existing_ratios})
    return pd.DataFrame(output_rows)

# Girdi dosyalarını yükle
builtup_df = pd.read_excel(r'C:\Users\arda.sari\Desktop\SLF PROJE\SLF_hesabı\saturation_update_results_29-versiyon.xlsx')
zoning_df = pd.read_excel(r'C:\Users\arda.sari\Desktop\SLF PROJE\SLF_hesabı\SLF_IMAR_TAHMINI_GIRDI4.3.xlsx', sheet_name='tüketim-bina_kirilimlari')

# Ana döngü
output_path = r'C:\Users\arda.sari\Desktop\SLF PROJE\SLF_hesabı\2024-2035_son_deneme_imar_tahmin_1203_4.xlsx'
with pd.ExcelWriter(output_path, engine='openpyxl') as writer:
    for year in range(2024, 2036):
        print(f"\nProcessing year {year}...")
        filtered_cells = filter_cells_for_year(builtup_df, zoning_df, year)
        calculated_ratios = calculate_weighted_average(filtered_cells, builtup_df, zoning_df, year)
        output_df = create_output(builtup_df, zoning_df, calculated_ratios, year)
        
        # is_calculated sütununu güncelle
        zoning_df.loc[zoning_df['id'].isin(filtered_cells['id']), 'is_calculated'] = 1
        
        output_df.to_excel(writer, sheet_name=str(year), index=False)
        zoning_df.iloc[:, 2:-1] = output_df.iloc[:, 6:]

