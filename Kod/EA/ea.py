import pandas as pd
import numpy as np
import sqlite3
import sys
import json
import os
import logging
import time
import numbers  # Added for potential future use, though not required for string-only validation



# Ensure UTF-8 encoding for console output on Windows
if sys.platform == "win32":
    os.system("chcp 65001 > nul")  # Set console to UTF-8

# Logging settings with explicit UTF-8 encoding
logging.basicConfig(
    filename='ea_distribution.log',
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s',
    encoding='utf-8'  # Explicitly set UTF-8 for log file
)

# Get the root directory (default directory when opening cmd)
root_directory = os.path.expanduser("~")

# Read command-line arguments
config_file_path = sys.argv[1]
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

# Access config objects
ana_klasor_yolu = config['Ana_Klasör_Yolu']
il = config['İl']
ilce = config['İlçe']
proje_ismi = config['proje_ismi']
ea_config = config['EA']
start_year = config['baslangıc_yılı']
end_year = config['bitis_yılı']
# Map config ilce to integer (defined outside the loop)
ilce_mapping = {
    'Çiğli': 1,
    'Karşıyaka': 2,
    'Tepebaşı': 3
}
reverse_ilce_mapping = {v: k for k, v in ilce_mapping.items()}

# Validate ilce from config
if not isinstance(ilce, str):
    logging.error(f"Invalid ilce type in config: {type(ilce)}. Must be a string")
    raise ValueError(f"Invalid ilce type: {type(ilce)}. Expected string")
if ilce not in ilce_mapping:
    valid_ilces = ', '.join(ilce_mapping.keys())
    logging.error(f"Invalid ilce value in config: {ilce}. Must be one of: {valid_ilces}")
    raise ValueError(f"Invalid ilce: {ilce}. Expected one of: {valid_ilces}")
ilce_value = ilce_mapping[ilce]  # Convert string to integer (e.g., "Çiğli" → 1)
logging.info(f"Config ilce: {ilce} mapped to integer: {ilce_value}")

#start_year = 2024
#end_year = 2035

# Construct paths using os.path.join for platform-independent paths
min_max_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['min_max_path'])
ilce_results_df_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['ilce_path'])
cikti_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['cikti_dosyasi'])
cikti_path_xlsx = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['cikti_dosyasi_xlsx'])
ilk_yıl_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['ilk_yıl'])
utilization_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['utilization_path'])

sqlite_db_path = os.path.join(
    root_directory, ana_klasor_yolu, il, ilce, proje_ismi,
    'sonuclar/SLF Sonuçları/3.Bina Sayısı/Çıktı/',
    f"bina_sayisi_hesaplama_{start_year}_{end_year}.db"
)

print("Dosyalar yükleniyor...")
# Load static files
min_max_df = pd.read_excel(min_max_path)
utilization_df = pd.read_excel(utilization_path)

# Load utilization factors with error handling
try:
    utilization_df = pd.read_excel(utilization_path)
    print("Columns in UtilizasyonFaktoru.xlsx:", utilization_df.columns.tolist())
except FileNotFoundError:
    logging.error(f"Utilization factor file not found: {utilization_path}")
    sys.exit(1)
except Exception as e:
    logging.error(f"Error loading UtilizasyonFaktoru.xlsx: {str(e)}")
    sys.exit(1)

# Verify required columns in utilization_df
required_columns = ['min_soket', 'max_soket', 'kullanilacak_yuk_faktoru_rdf']
missing_columns = [col for col in required_columns if col not in utilization_df.columns]
if missing_columns:
    logging.error(f"Missing required columns in UtilizasyonFaktoru.xlsx: {missing_columns}")
    sys.exit(1)

print("Dosyalar başarıyla yüklendi!")

# Charging station types and corresponding names
evcs_types = {
    'HomeACDağıtılanEVCS': 'AC (Home)',
    'WorkDağıtılanEVCS': 'AC (Work)',
    'PublicDağıtılanEVCS': 'AC (Public)',
    'FastDCDağıtılanEVCS': 'Fast DC'
}

# Charging station power ratings (kW)
evcs_power = {
    'AC (Home)': 11,
    'AC (Work)': 11,
    'AC (Public)': 22,
    'Fast DC': 150
}

def monte_carlo_simulation_with_min_max(imar_analyses_dummy_df, min_max_df, evcs_types, karşıyaka_results_df, num_simulations=100):
    print(f"  Monte Carlo başlıyor: {num_simulations} simülasyon...")
    results = []
    
    # Show progress every 25 simulations
    for sim_num in range(num_simulations):
        if (sim_num + 1) % 25 == 0:
            print(f"    Simülasyon {sim_num + 1}/{num_simulations} tamamlandı...")
            
        # Create probability table from min-max values
        categories = min_max_df['Category'].unique()
        probability_cols = [col.replace(' - min', '') for col in min_max_df.columns if ' - min' in col]
        
        randomized_probabilities = pd.DataFrame({'Category': categories})
        for category in categories:
            category_mask = min_max_df['Category'] == category
            for col in probability_cols:
                if col in imar_analyses_dummy_df.columns:
                    min_col = f"{col} - min"
                    max_col = f"{col} - max"
                    min_val = min_max_df.loc[category_mask, min_col].values[0]
                    max_val = min_max_df.loc[category_mask, max_col].values[0]
                    random_value = np.random.uniform(min_val, max_val)
                    randomized_probabilities.loc[randomized_probabilities['Category'] == category, col] = random_value
        
        # Normalize probabilities
        for category in categories:
            category_mask = randomized_probabilities['Category'] == category
            category_cols = [col for col in probability_cols if col in imar_analyses_dummy_df.columns]
            category_values = randomized_probabilities.loc[category_mask, category_cols].values[0]
            normalization_factor = sum(category_values)
            if normalization_factor > 0:
                randomized_probabilities.loc[category_mask, category_cols] = [val / normalization_factor for val in category_values]
        
        simulation_result = apply_existing_methodology(imar_analyses_dummy_df, randomized_probabilities, evcs_types, karşıyaka_results_df)
        results.append(simulation_result)
    
    print("  Monte Carlo sonuçları birleştiriliyor...")
    final_results = pd.concat(results).groupby(['id', 'left', 'top', 'right', 'bottom', 'ilce']).mean().reset_index()
    print("  Monte Carlo tamamlandı!")
    return final_results

def apply_existing_methodology(imar_analyses_dummy_df, randomized_probabilities, evcs_types, karşıyaka_results_df):
    if 'ilce' not in imar_analyses_dummy_df.columns:
        raise KeyError("'ilce' sütunu imar_analyses_dummy_df veri çerçevesinde bulunamadı. Lütfen veri çerçevesinde bu sütunun mevcut olduğundan emin olun.")
    
    imar_analyses_dummy_df = imar_analyses_dummy_df.fillna(0)
    results_df = imar_analyses_dummy_df[['id', 'left', 'top', 'right', 'bottom', 'ilce']].copy()
    
    # Ensure ilce is Python int
    results_df['ilce'] = results_df['ilce'].astype(int)
    
    for evcs_type, evcs_name in evcs_types.items():
        for ilce in results_df['ilce'].unique():
            ilce = int(ilce)  # Convert to Python int
            ilce_mask = results_df['ilce'] == ilce
            ilce_imar_df = imar_analyses_dummy_df.loc[ilce_mask].copy()
            ilce_randomized_probabilities = randomized_probabilities.copy()
            
            for col in ilce_randomized_probabilities.columns[1:]:
                if col in ilce_imar_df.columns:
                    ilce_imar_df[col + '_weighted'] = np.log(ilce_imar_df[col] + 1) * ilce_randomized_probabilities.loc[ilce_randomized_probabilities['Category'] == evcs_name, col].values[0]
            
            weighted_columns = [col + '_weighted' for col in ilce_randomized_probabilities.columns[1:] if col in ilce_imar_df.columns]
            ilce_imar_df[evcs_name + '_sum'] = ilce_imar_df[weighted_columns].sum(axis=1)
            ilce_imar_df[evcs_name + '_normalized'] = ilce_imar_df[evcs_name + '_sum'] / ilce_imar_df[evcs_name + '_sum'].sum()
            total_evcs = karşıyaka_results_df.loc[karşıyaka_results_df['ilce'] == ilce, evcs_type].values[0]
            results_df.loc[ilce_mask, evcs_name + '_count'] = ilce_imar_df[evcs_name + '_normalized'] * total_evcs
    
    return results_df

def redistribute_low_values_with_smoothing(output_df, threshold=0.5):
    print("  Redistribution işlemi başlıyor...")
    evcs_types_counts = ['AC (Home)_count', 'AC (Work)_count', 'AC (Public)_count', 'Fast DC_count']
    
    # Ensure ilce is Python int
    output_df['ilce'] = output_df['ilce'].astype(int)
    
    for ilce in output_df['ilce'].unique():
        ilce = int(ilce)  # Convert to Python int
        ilce_mask = output_df['ilce'] == ilce
        ilce_df = output_df.loc[ilce_mask].copy()
        
        for evcs_type in evcs_types_counts:
            # Identify low and high value cells
            low_mask = ilce_df[evcs_type] < threshold
            high_mask = ilce_df[evcs_type] >= threshold
            
            if not low_mask.any():
                continue
            
            if not high_mask.any():
                continue
            
            # Calculate total amount to redistribute from low-value cells
            total_to_distribute = ilce_df.loc[low_mask, evcs_type].sum()
            
            # Existing values in high-value cells
            high_values = ilce_df.loc[high_mask, evcs_type]
            
            # Smoothed distribution ratios with Min-Max mixture
            if high_values.sum() > 0:
                # Inverse proportional ratios
                inverse_ratios = (1 / (high_values + 0.01)) / (1 / (high_values + 0.01)).sum()
                # Equal distribution ratios
                equal_ratios = pd.Series([1/len(high_values)] * len(high_values), index=high_values.index)
                # 60% inverse + 40% equal mixture
                smoothed_ratios = 0.6 * inverse_ratios + 0.4 * equal_ratios
                
                # Perform redistribution
                ilce_df.loc[high_mask, evcs_type] += smoothed_ratios * total_to_distribute
                
                # Zero out low-value cells
                ilce_df.loc[low_mask, evcs_type] = 0
        
        # Update main DataFrame with redistributed values
        output_df.loc[ilce_mask, evcs_types_counts] = ilce_df[evcs_types_counts]
    
    print("  Redistribution tamamlandı!")
    return output_df

# Multi-year calculation loop
years = range(start_year, end_year + 1)

print(f"\n{'='*60}")
print(f"EVCS MONTE CARLO SİMÜLASYONU BAŞLIYOR")
print(f"{'='*60}")
print(f"Yıllar: {years.start} - {years.stop-1}")
print(f"Toplam {len(years)} yıl işlenecek")
print(f"{'='*60}")

# Initial state for the first year
print("\nBaşlangıç verileri yükleniyor...")
previous_results = pd.read_excel(ilk_yıl_path)

# Calculate X and Y coordinates
previous_results['x_koordinat'] = (previous_results['left'] + previous_results['right']) / 2
previous_results['y_koordinat'] = (previous_results['top'] + previous_results['bottom']) / 2

print(f"Başlangıç verileri yüklendi: {len(previous_results)} hücre")

# SQLite output file
output_sqlite_path = cikti_path

# List to collect all results
all_results = []

# Start time
start_time = time.time()

for year_index, year in enumerate(years, 1):
    year_start_time = time.time()
    
    print(f"\n{'='*50}")
    print(f"YIL: {year} ({year_index}/{len(years)})")
    print(f"{'='*50}")
    
    # SQLite veritabanından veri okuma
    print("  Veritabanından veri okunuyor...")
    conn = sqlite3.connect(sqlite_db_path)
    imar_analyses_dummy_df = pd.read_sql_query(f"SELECT * FROM `new_buildings_{year}`", conn)
    conn.close()
    print(f"  {len(imar_analyses_dummy_df)} hücre verisi yüklendi")
    
    # Debug: Print columns to verify
    print(f"Columns in new_buildings_{year} table:", imar_analyses_dummy_df.columns.tolist())
    
    # Add 'ilce' column if missing
    if 'ilce' not in imar_analyses_dummy_df.columns:
        logging.warning(f"'ilce' column not found in new_buildings_{year}. Adding from config: {ilce} (mapped to {ilce_value})")
        imar_analyses_dummy_df['ilce'] = int(ilce_value)  # Explicitly cast to Python int
    else:
        # Ensure existing ilce column is Python int
        imar_analyses_dummy_df['ilce'] = imar_analyses_dummy_df['ilce'].astype(int)

    # Excel verisi yükleme
    print("  Excel verisi yükleniyor...")
    karşıyaka_results_df = pd.read_excel(ilce_results_df_path, sheet_name=str(year))
    print(f"  {len(karşıyaka_results_df)} ilçe hedef verisi yüklendi")

    # Yeni gelen şarj istasyonlarını dağıt
    final_results = monte_carlo_simulation_with_min_max(imar_analyses_dummy_df, min_max_df, evcs_types, karşıyaka_results_df)

    # X ve Y koordinatlarını hesapla
    print("  Koordinatlar hesaplanıyor...")
    final_results['x_koordinat'] = (final_results['left'] + final_results['right']) / 2
    final_results['y_koordinat'] = (final_results['top'] + final_results['bottom']) / 2

    # Yeni redistribution fonksiyonunu kullan
    output_df = redistribute_low_values_with_smoothing(final_results.copy(), threshold=0.5)

    # Tam sayıya yuvarlama ve toplamları koruma
    print("  Tam sayıya yuvarlama işlemi...")
    evcs_types_counts = ['AC (Home)_count', 'AC (Work)_count', 'AC (Public)_count', 'Fast DC_count']
    
    for ilce in output_df['ilce'].unique():
        ilce = int(ilce)  # Convert to Python int
        ilce_mask = output_df['ilce'] == ilce
        ilce_df = output_df.loc[ilce_mask, evcs_types_counts].copy()
        
        for evcs_type in evcs_types_counts:
            original_total = ilce_df[evcs_type].sum()
            
            # Eğer original total 1'den küçükse, hepsini 0 yap
            if original_total < 1:
                output_df.loc[ilce_mask, evcs_type] = 0
                continue
                
            rounded_values = ilce_df[evcs_type].round().astype(int)
            current_sum = rounded_values.sum()
            difference = original_total - current_sum
            
            # Eğer fark varsa düzeltme yap
            if difference != 0:
                if difference > 0:
                    # Eksik var - en büyük değerli hücrelere 1 ekle                    
                    # Sadece 0'a yuvarlanmış olanları bul (orijinali > 0 ama rounded = 0)
                    zero_but_had_value = (rounded_values == 0) & (ilce_df[evcs_type] > 0)
                    zero_values = rounded_values[zero_but_had_value]
                    if len(zero_values) >= int(difference):
                        # 0 olan hücreler arasından original değeri en büyük olanları seç
                        zero_indices = zero_values.index
                        original_zero_values = ilce_df.loc[zero_indices, evcs_type]
                        # En büyük original değere sahip olanları seç
                        selected_indices = original_zero_values.nlargest(int(difference)).index
                        rounded_values.loc[selected_indices] += 1
                    else:
                        # Yeterli 0 değeri yok - TÜM hücreler arasından original değeri en büyük olanları seç
                        all_original_values = ilce_df[evcs_type]
                        selected_indices = all_original_values.nlargest(int(difference)).index
                        rounded_values.loc[selected_indices] += 1
                else:
                    # Fazla var - en küçük değerli hücrelerin değerini azalt
                    adjustment_indices = rounded_values.nlargest(int(abs(difference))).index
                    rounded_values.loc[adjustment_indices] += np.sign(difference)
            
            output_df.loc[ilce_mask, evcs_type] = rounded_values

    # Yeni sonuçları kümülatif olarak ekle
    print("  Kümülatif ekleme işlemi...")
    # Önce her iki DataFrame'i aynı sıraya getir
    previous_results = previous_results.sort_values('id').reset_index(drop=True)
    output_df = output_df.sort_values('id').reset_index(drop=True)

    for evcs_type in evcs_types.values():
        count_col = evcs_type + '_count'
        previous_results[count_col] += output_df[count_col]

    # Her hücre için toplam kapasite hesapla
    previous_results['toplam_kapasite'] = 0
    for evcs_type, power in evcs_power.items():
        count_col = evcs_type + '_count'
        previous_results['toplam_kapasite'] += previous_results[count_col] * power

    # Her hücre için ayrı ayrı EV tipleri için yük hesapla
    print("  Yük hesaplamaları...")
    for evcs_type, power in evcs_power.items():
        count_col = evcs_type + '_count'
        yuk_col = evcs_type + '_yuk'
        # Initialize the yuk column as float
        previous_results[yuk_col] = 0.0
        # Calculate yük for each row based on the count and utilization factor
        for idx in previous_results.index:
            count = previous_results.loc[idx, count_col]
            # Find the appropriate utilization factor based on the count
            factor = 1.0  # Default factor for counts 0-1
            for _, row in utilization_df.iterrows():
                if row['min_soket'] <= count <= row['max_soket']:
                    factor = row['kullanilacak_yuk_faktoru_rdf']
                    break
            # Calculate yük as count * power * factor and ensure it's float
            previous_results.loc[idx, yuk_col] = float(count * power * factor)

    # Her hücre için toplam yük hesapla
    previous_results['toplam_yuk'] = 0
    for evcs_type in evcs_power.keys():
        yuk_col = evcs_type + '_yuk'
        previous_results['toplam_yuk'] += previous_results[yuk_col]

    # Year sütunu ekle
    previous_results_with_year = previous_results.copy()
    previous_results_with_year['year'] = year
    
    # Tüm sonuçları liste append et
    all_results.append(previous_results_with_year)
    
    # Yıl süresini hesapla
    year_duration = time.time() - year_start_time
    total_elapsed = time.time() - start_time
    estimated_total = (total_elapsed / year_index) * len(years)
    remaining_time = estimated_total - total_elapsed
    
    print(f"   {year} yılı tamamlandı!")
    print(f"   Bu yıl süresi: {year_duration:.1f} dakika")
    print(f"   Toplam geçen süre: {total_elapsed/60:.1f} dakika")
    print(f"   Tahmini kalan süre: {remaining_time/60:.1f} dakika")
    print(f"   İlerleme: {(year_index/len(years)*100):.1f}%")

# Save to SQLite database
print(f"\n{'='*50}")
print("SQLite VERİTABANINA KAYDETME")
print(f"{'='*50}")
table_name = config['İlçe']
# Combine all years' data
combined_data = pd.concat(all_results, ignore_index=True)

conn = sqlite3.connect(output_sqlite_path)

# Debug: Verify ilce value and type
logging.info(f"Saving to table with ilce: {table_name} (type: {type(table_name)})")

# Save as a single table with ilce name (string from config)
combined_data.to_sql(table_name, conn, if_exists='replace', index=False)
print(f" Tüm sonuçlar için {len(combined_data)} satır '{table_name}' tablosuna kaydedildi.")

conn.close()
# Final summary
total_duration = time.time() - start_time
print(f"\n{'='*60}")
print(f"TÜM İŞLEMLER TAMAMLANDI!")
print(f"{'='*60}")
print(f" Çıktı dosyası: {output_sqlite_path}")
print(f" Toplam süre: {total_duration/60:.1f} dakika")
print(f" İşlenen yıl sayısı: {len(years)}")
print(f" Oluşturulan tablo sayısı: 1")
print(f"{'='*60}")