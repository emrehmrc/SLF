import pandas as pd
from sklearn.cluster import KMeans
import numpy as np
import logging
import sqlite3
from tqdm import tqdm
import sys
import json
import os
from sklearn.metrics import silhouette_score
import time

# Logging settings (unchanged)
logging.basicConfig(filename='dek_distribution.log', level=logging.INFO,
                    format='%(asctime)s - %(levelname)s - %(message)s')

# Get the root directory (unchanged)
root_directory = os.path.expanduser("~")
config_file_path = sys.argv[1]
# Read config file (unchanged)
#config_file_path = r"C:\Users\begum.orhan\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\il_ilce_kırılımları\Program Dosyaları\config.json"

# Read and parse config.json
try:
    with open(config_file_path, 'r', encoding='utf-8') as config_file:
        config = json.load(config_file)
except FileNotFoundError:
    logging.error(f"Config file not found: {config_file_path}")
    sys.exit(1)
except json.JSONDecodeError:
    logging.error(f"Invalid JSON format in config file: {config_file_path}")
    sys.exit(1)

# Access config objects (remove ilce)
ana_klasor_yolu = config['Ana_Klasör_Yolu']
il = config['İl']
ilce = config['İlçe']
dek_config = config['DEK']
start_year = config['baslangıc_yılı']
end_year = config['bitis_yılı']
proje_ismi = config['proje_ismi']

print("DEK projeksiyonları metodolojisi çalıştırılıyor...\n")
time.sleep(2)
print("İlgili veriler yükleniyor...\n")
time.sleep(2)

# Construct paths without ilce
min_max_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, dek_config['dek_klasörü'], dek_config['min_max_path'])
imar_tipi_bilgileri_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, dek_config['dek_klasörü'], dek_config['imar_alanları'])
ilce_results_df_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, dek_config['dek_klasörü'], dek_config['ilce_path'])
cikti_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, dek_config['dek_klasörü'], dek_config['cikti_dosyasi'])
ilk_yıl_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, dek_config['dek_klasörü'], dek_config['ilk_yıl'])
sqlite_db_path = os.path.join(
    root_directory, ana_klasor_yolu, il, ilce, proje_ismi,
    'sonuclar/SLF Sonuçları/3.Bina Sayısı/Çıktı/',
    f"bina_sayisi_hesaplama_{start_year}_{end_year}.db"
)

# Load min-max and imar files (unchanged)
min_max_df = pd.read_excel(min_max_path)
imar_tipi_bilgileri_df = pd.read_excel(imar_tipi_bilgileri_path)

# Clean column names (unchanged)
min_max_df.columns = min_max_df.columns.str.strip()
min_max_df.columns = min_max_df.columns.str.replace('–', '-')
min_max_df.columns = min_max_df.columns.str.replace('—', '-')

# DEK probability type (unchanged)
dek_probability_type = 'DEK_Probability'

# Parameter columns from min_max_df (unchanged)
parameter_columns = [col.replace(' - min', '').replace(' - max', '')
                    for col in min_max_df.columns
                    if ' - min' in col]
parameter_columns = list(set(parameter_columns))

def monte_carlo_simulation_with_building_growth(imar_analyses_df, min_max_df, imar_tipi_bilgileri_df,
                                               karşıyaka_results_df, num_simulations=300):
    results = []
    for _ in tqdm(range(num_simulations), desc="Monte Carlo simulasyonları çalıştırılıyor: "):
        # Calculate roof areas
        roof_areas_df = calculate_roof_areas(imar_analyses_df, imar_tipi_bilgileri_df)
        # Create random weights
        randomized_weights = create_random_weights(min_max_df, parameter_columns)
        # Apply methodology with roof areas
        simulation_result = apply_methodology_with_roof_areas(roof_areas_df, randomized_weights, karşıyaka_results_df)
        results.append(simulation_result)
    # Group by spatial coordinates only
    final_results = pd.concat(results).groupby(['id', 'left', 'top', 'right', 'bottom']).mean().reset_index()
    return final_results

def calculate_roof_areas(imar_analyses_df, imar_tipi_bilgileri_df):
    """Calculate total roof area for each cell"""
    # Initialize results without ilce
    roof_areas_df = imar_analyses_df[['id', 'left', 'top', 'right', 'bottom']].copy()
    
    # Calculate roof area for each imar type
    for _, row in imar_tipi_bilgileri_df.iterrows():
        imar_tipi = row['imar_tipi']
        bina_basi_brut_alan = row['bina_başı_brüt_alan']
        taks = row['taks'] if 'taks' in row and not pd.isna(row['taks']) else 0.5
        
        if imar_tipi in imar_analyses_df.columns:
            roof_areas_df[f'{imar_tipi}_roof_area'] = imar_analyses_df[imar_tipi] * bina_basi_brut_alan * taks
        else:
            roof_areas_df[f'{imar_tipi}_roof_area'] = 0
    
    # Sum total roof area
    roof_area_columns = [col for col in roof_areas_df.columns if '_roof_area' in col]
    roof_areas_df['total_roof_area'] = roof_areas_df[roof_area_columns].sum(axis=1)
    return roof_areas_df

def create_random_weights(min_max_df, parameter_columns):
    # Unchanged
    randomized_weights_df = pd.DataFrame(columns=['Category'] + parameter_columns)
    randomized_weights_df.loc[0, 'Category'] = dek_probability_type
    
    random_values = []
    for col in parameter_columns:
        min_col = f"{col} - min"
        max_col = f"{col} - max"
        min_val = min_max_df.loc[min_max_df['Category'] == dek_probability_type, min_col].values
        max_val = min_max_df.loc[min_max_df['Category'] == dek_probability_type, max_col].values
        
        if len(min_val) > 0 and len(max_val) > 0:
            random_value = np.random.uniform(min_val[0], max_val[0])
            random_values.append(random_value)
            randomized_weights_df.loc[0, col] = random_value
        else:
            logging.warning(f"Min or max values missing for category '{dek_probability_type}' and columns '{min_col}' or '{max_col}'")
            randomized_weights_df.loc[0, col] = 0
    
    normalization_factor = sum(random_values)
    if normalization_factor > 0:
        for i, col in enumerate(parameter_columns):
            randomized_weights_df.loc[0, col] = random_values[i] / normalization_factor
    
    return randomized_weights_df

def apply_methodology_with_roof_areas(roof_areas_df, randomized_weights, karşıyaka_results_df):
    """Apply DEK distribution methodology using roof areas"""
    # Initialize results without ilce
    results_df = roof_areas_df[['id', 'left', 'top', 'right', 'bottom', 'total_roof_area']].copy()
    
    # Calculate total roof area for entire dataset
    total_roof_area = roof_areas_df['total_roof_area'].sum()
    
    if total_roof_area > 0:
        results_df['DEK_normalized'] = roof_areas_df['total_roof_area'] / total_roof_area
    else:
        results_df['DEK_normalized'] = 0
    
    # Distribute total DEK capacity
    total_dek = karşıyaka_results_df['Installed Capacity'].sum()  # Sum across all rows
    results_df['DEK_distributed'] = results_df['DEK_normalized'] * total_dek
    
    return results_df

def run_multiyear_simulation():
    try:
        output_writer = pd.ExcelWriter(cikti_path, engine='xlsxwriter')
        years = range(start_year, end_year + 1)
        
        # Load initial year data
        previous_year_df = pd.read_excel(ilk_yıl_path)
        previous_year_df['x_koordinat'] = (previous_year_df['left'] + previous_year_df['right']) / 2
        previous_year_df['y_koordinat'] = (previous_year_df['top'] + previous_year_df['bottom']) / 2
        
        for year in years:
            logging.info(f"Simulasyon yılı: {year}")
            
            # Load data from SQLite
            conn = sqlite3.connect(sqlite_db_path)
            imar_analyses_df = pd.read_sql_query(f"SELECT * FROM `new_buildings_{year}`", conn)
            conn.close()
            ilce_results_df = pd.read_excel(ilce_results_df_path, sheet_name=str(year))
            
            # Run Monte Carlo simulation
            final_results = monte_carlo_simulation_with_building_growth(
                imar_analyses_df, min_max_df, imar_tipi_bilgileri_df, ilce_results_df)
            
            # Calculate coordinates
            final_results['x_koordinat'] = (final_results['left'] + final_results['right']) / 2
            final_results['y_koordinat'] = (final_results['top'] + final_results['bottom']) / 2
            
            # Optimize with K-Means
            output_df = optimize_with_kmeans(final_results)
            
            # Update cumulative results
            dek_types_counts = ['DEK_distributed']
            previous_year_df[dek_types_counts] += output_df[dek_types_counts]
            
            # Write results
            previous_year_df.to_excel(output_writer, sheet_name=str(year), index=False)
            logging.info(f"{year} yılı için sonuçlar oluşturuldu.")
        
        output_writer.close()
        print("Kod başarıyla çalıştırıldı.! Sonuçlar 'dek_distribution_cumulative.xlsx' dosyasına eklendi.")
        time.sleep(3)
        
    except Exception as e:
        logging.error(f"Error in run_multiyear_simulation: {e}")
        raise

def optimize_with_kmeans(final_results):
    """Optimize DEK distribution using K-Means clustering"""
    output_df = final_results.copy()
    dek_types_counts = ['DEK_distributed']
    
    # Apply clustering to entire dataset
    values = output_df[dek_types_counts].values
    
    if len(values) < 5:
        return output_df
    
    best_score = -1
    best_n_clusters = 4
    max_clusters = min(6, len(values) // 2)
    
    for n_clusters in range(2, max_clusters + 1):
        try:
            kmeans = KMeans(n_clusters=n_clusters, random_state=42)
            cluster_labels = kmeans.fit_predict(values)
            if len(set(cluster_labels)) > 1:
                score = silhouette_score(values, cluster_labels)
                if score > best_score:
                    best_score = score
                    best_n_clusters = n_clusters
        except Exception as e:
            logging.warning(f"Error in silhouette calculation with {n_clusters} clusters: {e}")
    
    kmeans = KMeans(n_clusters=best_n_clusters, random_state=42)
    output_df['cluster'] = kmeans.fit_predict(values)
    
    # Calculate cluster averages
    cluster_averages = output_df.groupby('cluster')['DEK_distributed'].mean()
    max_cluster = cluster_averages.idxmax()
    
    # Redistribute values from other clusters to max cluster
    for cluster in cluster_averages.index:
        if cluster != max_cluster:
            cluster_mask = output_df['cluster'] == cluster
            total_to_distribute = output_df.loc[cluster_mask, 'DEK_distributed'].sum()
            max_cluster_mask = output_df['cluster'] == max_cluster
            max_cluster_values = output_df.loc[max_cluster_mask, 'DEK_distributed']
            if not max_cluster_values.empty:
                distribution_ratios = max_cluster_values / max_cluster_values.sum()
                output_df.loc[max_cluster_mask, 'DEK_distributed'] += distribution_ratios * total_to_distribute
                output_df.loc[cluster_mask, 'DEK_distributed'] = 0
    
    output_df.drop(columns=['cluster'], inplace=True)
    return output_df

if __name__ == "__main__":
    print("\nSimulasyonlar çalıştırılıyor!\n")
    run_multiyear_simulation()