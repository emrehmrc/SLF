import pandas as pd
import numpy as np
import sqlite3
import sys
import json
import os
import logging

# Logging ayarları
logging.basicConfig(filename='ea_distribution.log', level=logging.INFO,
                    format='%(asctime)s - %(levelname)s - %(message)s')

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
ea_config = config['EA']
start_year = config['baslangıc_yılı']
end_year = config['bitis_yılı']
proje_ismi = config['proje_ismi']



#start_year = 2024
#end_year = 2035



# Construct paths using os.path.join for platform-independent paths
min_max_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['min_max_path'])
ilce_results_df_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['ilce_path'])
cikti_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['cikti_dosyasi'])
ilk_yıl_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['ilk_yıl'])
utilization_path = os.path.join(root_directory, ana_klasor_yolu, il, ilce, ea_config['ea_klasörü'], ea_config['utilization_path'])

sqlite_db_path = os.path.join(
    root_directory, ana_klasor_yolu, il, ilce, proje_ismi,
    'sonuclar/SLF Sonuçları/3.Bina Sayısı/Çıktı/',
    f"bina_sayisi_hesaplama_{start_year}_{end_year}.db"
)

# Sabit dosyaları yükle
min_max_df = pd.read_excel(min_max_path)

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

# Şarj istasyonu tipleri ve karşılık gelen sayılar
evcs_types = {
    'HomeACDağıtılanEVCS': 'AC (Home)',
    'WorkDağıtılanEVCS': 'AC (Work)',
    'PublicDağıtılanEVCS': 'AC (Public)',
    'FastDCDağıtılanEVCS': 'Fast DC'
}

# Şarj istasyonu kurulu güçleri (kW)
evcs_power = {
    'AC (Home)': 11,
    'AC (Work)': 11,
    'AC (Public)': 22,
    'Fast DC': 150
}

def monte_carlo_simulation_with_min_max(imar_analyses_dummy_df, min_max_df, evcs_types, karşıyaka_results_df, num_simulations=300):
    results = []
    for _ in range(num_simulations):
        # Min-max değerlerinden olasılık tablosu oluştur
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
        
        # Olasılıkları normalize et
        for category in categories:
            category_mask = randomized_probabilities['Category'] == category
            category_cols = [col for col in probability_cols if col in imar_analyses_dummy_df.columns]
            category_values = randomized_probabilities.loc[category_mask, category_cols].values[0]
            normalization_factor = sum(category_values)
            if normalization_factor > 0:
                randomized_probabilities.loc[category_mask, category_cols] = [val / normalization_factor for val in category_values]
        
        simulation_result = apply_existing_methodology(imar_analyses_dummy_df, randomized_probabilities, evcs_types, karşıyaka_results_df)
        results.append(simulation_result)
    
    # Remove 'ilce' from grouping
    final_results = pd.concat(results).groupby(['id', 'left', 'top', 'right', 'bottom']).mean().reset_index()
    return final_results

def apply_existing_methodology(imar_analyses_dummy_df, randomized_probabilities, evcs_types, karşıyaka_results_df):
    # Remove ilce dependency
    imar_analyses_dummy_df = imar_analyses_dummy_df.fillna(0)
    results_df = imar_analyses_dummy_df[['id', 'left', 'top', 'right', 'bottom']].copy()
    
    for evcs_type, evcs_name in evcs_types.items():
        imar_df = imar_analyses_dummy_df.copy()
        randomized_probs = randomized_probabilities.copy()
        
        for col in randomized_probs.columns[1:]:
            if col in imar_df.columns:
                imar_df[col + '_weighted'] = np.log(imar_df[col] + 1) * randomized_probs.loc[randomized_probs['Category'] == evcs_name, col].values[0]
        
        weighted_columns = [col + '_weighted' for col in randomized_probs.columns[1:] if col in imar_df.columns]
        imar_df[evcs_name + '_sum'] = imar_df[weighted_columns].sum(axis=1)
        imar_df[evcs_name + '_normalized'] = imar_df[evcs_name + '_sum'] / imar_df[evcs_name + '_sum'].sum()
        # Assume karşıyaka_results_df has a single row or use the first row
        total_evcs = karşıyaka_results_df[evcs_type].iloc[0]
        results_df[evcs_name + '_count'] = imar_df[evcs_name + '_normalized'] * total_evcs
    
    return results_df

def redistribute_low_values_with_smoothing(output_df, threshold=0.5):
    """
    Monte Carlo sonucunda belirtilen eşik değerinden düşük olan hücrelerdeki
    şarj istasyonlarını eşik değerinden yüksek olan hücrelere Min-Max karışımı ile dağıtır.
    """
    evcs_types_counts = ['AC (Home)_count', 'AC (Work)_count', 'AC (Public)_count', 'Fast DC_count']
    
    # Process the entire DataFrame without ilce
    df = output_df.copy()
    
    for evcs_type in evcs_types_counts:
        # Düşük ve yüksek değerli hücreleri belirle
        low_mask = df[evcs_type] < threshold
        high_mask = df[evcs_type] >= threshold
        
        # Eğer düşük değerli hücre yoksa bu EVCS tipi için işlem yapma
        if not low_mask.any():
            continue
        
        # Eğer yüksek değerli hücre yoksa bu EVCS tipi için işlem yapma
        if not high_mask.any():
            continue
        
        # Düşük değerli hücrelerden toplam miktarı hesapla
        total_to_distribute = df.loc[low_mask, evcs_type].sum()
        
        # Yüksek değerli hücrelerdeki mevcut değerler
        high_values = df.loc[high_mask, evcs_type]
        
        # Min-Max Karışımı ile yumuşatılmış dağıtım oranları
        if high_values.sum() > 0:
            # Ters orantılı dağıtım oranları
            inverse_ratios = (1 / (high_values + 0.01)) / (1 / (high_values + 0.01)).sum()
            # Eşit dağıtım oranları
            equal_ratios = pd.Series([1/len(high_values)] * len(high_values), index=high_values.index)
            # %70 ters orantılı + %30 eşit dağıtım karışımı
            smoothed_ratios = 0.6 * inverse_ratios + 0.4 * equal_ratios
            
            # Dağıtımı gerçekleştir
            df.loc[high_mask, evcs_type] += smoothed_ratios * total_to_distribute
            
            # Düşük değerli hücreleri sıfırla
            df.loc[low_mask, evcs_type] = 0
    
    return df

# Çok yıllık hesaplama için döngü
output_writer = pd.ExcelWriter(cikti_path, engine='xlsxwriter')
years = range(start_year, end_year + 1)

# İlk yıl için başlangıç durumu
previous_results = pd.read_excel(ilk_yıl_path)

# X ve Y koordinatlarını hesapla
previous_results['x_koordinat'] = (previous_results['left'] + previous_results['right']) / 2
previous_results['y_koordinat'] = (previous_results['top'] + previous_results['bottom']) / 2

for year in years:
    # SQLite veritabanından veri okuma
    conn = sqlite3.connect(sqlite_db_path)
    imar_analyses_dummy_df = pd.read_sql_query(f"SELECT * FROM `new_buildings_{year}`", conn)
    conn.close()
    
    # Debug: Print columns to verify
    print(f"Columns in new_buildings_{year} table:", imar_analyses_dummy_df.columns.tolist())
    
    karşıyaka_results_df = pd.read_excel(ilce_results_df_path, sheet_name=str(year))
    
    # Continue with Monte Carlo simulation
    final_results = monte_carlo_simulation_with_min_max(imar_analyses_dummy_df, min_max_df, evcs_types, karşıyaka_results_df)
    
    # X ve Y koordinatlarını hesapla
    final_results['x_koordinat'] = (final_results['left'] + final_results['right']) / 2
    final_results['y_koordinat'] = (final_results['top'] + final_results['bottom']) / 2
    
    # Yeni redistribution fonksiyonunu kullan - Min-Max karışımı ile yumuşatılmış dağıtım
    output_df = redistribute_low_values_with_smoothing(final_results.copy(), threshold=0.5)
    
    # Tam sayıya yuvarlama ve toplamları koruma
    evcs_types_counts = ['AC (Home)_count', 'AC (Work)_count', 'AC (Public)_count', 'Fast DC_count']
    df = output_df[evcs_types_counts].copy()
    
    for evcs_type in evcs_types_counts:
        original_total = df[evcs_type].sum()
        
        # Eğer original total 1'den küçükse, hepsini 0 yap
        if original_total < 1:
            output_df[evcs_type] = 0
            continue
        
        rounded_values = df[evcs_type].round().astype(int)
        current_sum = rounded_values.sum()
        difference = original_total - current_sum
        
        # Eğer fark varsa düzeltme yap
        if difference != 0:
            if difference > 0:
                # Eksik var - en büyük değerli hücrelere 1 ekle
                zero_but_had_value = (rounded_values == 0) & (df[evcs_type] > 0)
                zero_values = rounded_values[zero_but_had_value]
                if len(zero_values) >= int(difference):
                    zero_indices = zero_values.index
                    original_zero_values = df.loc[zero_indices, evcs_type]
                    selected_indices = original_zero_values.nlargest(int(difference)).index
                    rounded_values.loc[selected_indices] += 1
                else:
                    all_original_values = df[evcs_type]
                    selected_indices = all_original_values.nlargest(int(difference)).index
                    rounded_values.loc[selected_indices] += 1
            else:
                # Fazla var - en küçük değerli hücrelerin değerini azalt
                adjustment_indices = rounded_values.nlargest(int(abs(difference))).index
                rounded_values.loc[adjustment_indices] += np.sign(difference)
        
        output_df[evcs_type] = rounded_values
    
    # Yeni sonuçları kümülatif olarak ekle - Basit ekleme
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
    for evcs_type, power in evcs_power.items():
        count_col = evcs_type + '_count'
        yuk_col = evcs_type + '_yuk'
        previous_results[yuk_col] = 0
        for idx in previous_results.index:
            count = previous_results.loc[idx, count_col]
            factor = 1.0  # Default factor for counts 0-1
            for _, row in utilization_df.iterrows():
                if row['min_soket'] <= count <= row['max_soket']:
                    factor = row['kullanilacak_yuk_faktoru_rdf']
                    break
            previous_results.loc[idx, yuk_col] = count * power * factor
    
    # Her hücre için toplam yük hesapla
    previous_results['toplam_yuk'] = 0
    for evcs_type in evcs_power.keys():
        yuk_col = evcs_type + '_yuk'
        previous_results['toplam_yuk'] += previous_results[yuk_col]
    
    # Güncellenmiş kümülatif sonuçları yazdır
    previous_results.to_excel(output_writer, sheet_name=str(year), index=False)

# Dosyayı kaydet ve kapat
output_writer.close()
print("Kümülatif sonuçlar başarıyla kaydedildi.")