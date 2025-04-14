import pandas as pd
from sklearn.cluster import KMeans
import numpy as np
import sys
import json
import os


# Get the config path from the first command-line argument
config_path = sys.argv[1]

ilk_yıl = sys.argv[2]
son_yıl = sys.argv[3]

# Load the config.json file
with open(config_path, 'r', encoding='utf-8') as f:
    config = json.load(f)

# Extract the necessary paths from config.json
ana_klasor_yolu = config['Ana_Klasör_Yolu']
il = config['İl']
ilce = config['İlçe']
ea_klasor = config['EA']['Klasör']
ea_sonuclar_klasor = config['EA']['SONUÇLAR_klasör']
ea_hucre_ilk_versiyon = config['EA']['GİRDİLER_ilk']
ea_hucresel_bina_sayıları = config['EA']['BİNA_SAYILARI']
ea_hucresel_bina_sayıları = config['EA']['BİNA_SAYILARI']
ea_delta = config['EA']['DELTA_SONUCLAR']

# Construct the full path to the EA_SONUCLAR.xlsx file output dynamically
output_dir = os.path.join(ana_klasor_yolu, il, ilce, ea_klasor, ea_sonuclar_klasor)
output_file = os.path.join(output_dir, 'EA_SONUÇLAR.xlsx')

# EA ların hucrelere imar analizi sonucu ilk kez dagıtıldıgı dosyanın pathi
hucre_ilk_versiyon_dosyası = os.path.join(ana_klasor_yolu, il, ilce, ea_klasor, ea_hucre_ilk_versiyon)
bina_sayıları_dosyası = os.path.join(ana_klasor_yolu, il, ilce, ea_klasor, ea_hucresel_bina_sayıları)
delta_sonuclar_path = os.path.join(ana_klasor_yolu, il, ilce, ea_klasor, ea_delta)


# Static dictionary to replace the Excel file
min_max_dict = {
    'Category': ['AC(Home)', 'AC(Public)', 'AC(Work)', 'Fast DC'],
    '1-2 KATLI MESKEN - min': [0, 0, 0, 0],
    '1-2 KATLI MESKEN - max': [0, 0, 0, 0.3],
    '3-4 KATLI MESKEN - min': [0, 0, 0, 0],
    '3-4 KATLI MESKEN - max': [0, 0, 0, 0.3],
    '5-7 KATLI MESKEN - min': [0, 0, 0, 0],
    '5-7 KATLI MESKEN - max': [0.2, 0, 0, 0.1],
    '8 USTU KATLI MESKEN - min': [0, 0, 0, 0],
    '8 USTU KATLI MESKEN - max': [0.3, 0, 0, 0.4],
    'VILLA MESKEN - min': [0, 0, 0, 0],
    'VILLA MESKEN - max': [0.3, 0, 0, 0],
    'ORTA_TICARETHANE - min': [0, 0, 0, 0],
    'ORTA_TICARETHANE - max': [0, 0.3, 0, 0],
    'BUYUK_TICARETHANE - min': [0, 0, 0, 0],
    'BUYUK_TICARETHANE - max': [0, 0.3, 0, 0],
    'KUCUK_SANAYI - min': [0, 0, 0, 0],
    'KUCUK_SANAYI - max': [0, 0, 0, 0.3],
    'ORTA_SANAYI - min': [0, 0, 0, 0],
    'ORTA_SANAYI - max': [0, 0, 0, 0.3],
    'BUYUK_SANAYI - min': [0, 0, 0, 0],
    'BUYUK_SANAYI - max': [0, 0, 0, 0.4],
    'TARIMSAL_SULAMA - min': [0, 0, 0, 0],
    'TARIMSAL_SULAMA - max': [0, 0, 0, 0],
    'AYDINLATMA - min': [0, 0, 0, 0],
    'AYDINLATMA - max': [0, 0, 0, 0]
}

# Convert the dictionary to a DataFrame
min_max_df = pd.DataFrame(min_max_dict)

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
    
    final_results = pd.concat(results).groupby(['id', 'left', 'top', 'right', 'bottom', 'ilce']).mean().reset_index()
    return final_results

def apply_existing_methodology(imar_analyses_dummy_df, randomized_probabilities, evcs_types, karşıyaka_results_df):

    if 'ilce' not in imar_analyses_dummy_df.columns:
        raise KeyError("'ilce' sütunu imar_analyses_dummy_df veri çerçevesinde bulunamadı. Lütfen veri çerçevesinde bu sütunun mevcut olduğundan emin olun.")
    
    imar_analyses_dummy_df = imar_analyses_dummy_df.fillna(0)
    results_df = imar_analyses_dummy_df[['id', 'left', 'top', 'right', 'bottom', 'ilce']].copy()
    
    for evcs_type, evcs_name in evcs_types.items():
        for ilce in results_df['ilce'].unique():
            ilce_mask = results_df['ilce'] == ilce
            ilce_imar_df = imar_analyses_dummy_df.loc[ilce_mask].copy()
            ilce_randomized_probabilities = randomized_probabilities.copy()
            
            for col in ilce_randomized_probabilities.columns[1:]:
                if col in ilce_imar_df.columns:
                    ilce_imar_df[col + '_weighted'] = ilce_imar_df[col] * ilce_randomized_probabilities.loc[ilce_randomized_probabilities['Category'] == evcs_name, col].values[0]
            
            weighted_columns = [col + '_weighted' for col in ilce_randomized_probabilities.columns[1:] if col in ilce_imar_df.columns]
            ilce_imar_df[evcs_name + '_sum'] = ilce_imar_df[weighted_columns].sum(axis=1)
            ilce_imar_df[evcs_name + '_normalized'] = ilce_imar_df[evcs_name + '_sum'] / ilce_imar_df[evcs_name + '_sum'].sum()
            total_evcs = karşıyaka_results_df.loc[karşıyaka_results_df['ilce'] == ilce, evcs_type].values[0]
            results_df.loc[ilce_mask, evcs_name + '_count'] = ilce_imar_df[evcs_name + '_normalized'] * total_evcs
    
    return results_df



# Use the constructed path in pd.ExcelWriter
output_writer = pd.ExcelWriter(output_file, engine='xlsxwriter')
years = range(ilk_yıl, son_yıl)


# İlk yıl için başlangıç durumu
previous_results = pd.read_excel(hucre_ilk_versiyon_dosyası)


# X ve Y koordinatlarını hesapla
previous_results['x_koordinat'] = (previous_results['left'] + previous_results['right']) / 2
previous_results['y_koordinat'] = (previous_results['top'] + previous_results['bottom']) / 2

for year in years:

    imar_analyses_dummy_df = pd.read_excel(bina_sayıları_dosyası, sheet_name=str(year))
    delta_sonuclar = pd.read_excel(delta_sonuclar_path, sheet_name=str(year))

    # Yeni gelen şarj istasyonlarını dağıt
    final_results = monte_carlo_simulation_with_min_max(imar_analyses_dummy_df, min_max_df, evcs_types, delta_sonuclar)

    # X ve Y koordinatlarını hesapla
    final_results['x_koordinat'] = (final_results['left'] + final_results['right']) / 2
    final_results['y_koordinat'] = (final_results['top'] + final_results['bottom']) / 2

    # K-Means ve sonuçların güncellenmesi
    output_df = final_results.copy()
    evcs_types_counts = ['AC (Home)_count', 'AC (Work)_count', 'AC (Public)_count', 'Fast DC_count']
    
    for ilce in output_df['ilce'].unique():
        ilce_mask = output_df['ilce'] == ilce
        ilce_df = output_df.loc[ilce_mask].copy()
        
        for evcs_type in evcs_types_counts:
            values = ilce_df[[evcs_type]].values
            kmeans = KMeans(n_clusters=4, random_state=42)
            ilce_df['cluster'] = kmeans.fit_predict(values)
            cluster_averages = ilce_df.groupby('cluster')[evcs_type].mean()
            max_cluster = cluster_averages.idxmax()
            
            for cluster in cluster_averages.index:
                if cluster != max_cluster:
                    cluster_mask = ilce_df['cluster'] == cluster
                    total_to_distribute = ilce_df.loc[cluster_mask, evcs_type].sum()
                    max_cluster_mask = ilce_df['cluster'] == max_cluster
                    max_cluster_values = ilce_df.loc[max_cluster_mask, evcs_type]
                    distribution_ratios = max_cluster_values / max_cluster_values.sum()
                    ilce_df.loc[max_cluster_mask, evcs_type] += distribution_ratios * total_to_distribute
                    ilce_df.loc[cluster_mask, evcs_type] = 0
            
            ilce_df.drop(columns=['cluster'], inplace=True)
        
        output_df.loc[ilce_mask, evcs_types_counts] = ilce_df[evcs_types_counts]

    # Tam sayıya yuvarlama ve toplamları koruma
    for ilce in output_df['ilce'].unique():
        ilce_mask = output_df['ilce'] == ilce
        ilce_df = output_df.loc[ilce_mask, evcs_types_counts].copy()
        
        for evcs_type in evcs_types_counts:
            original_total = ilce_df[evcs_type].sum()
            rounded_values = ilce_df[evcs_type].round().astype(int)
            difference = original_total - rounded_values.sum()
            
            if difference != 0:
                adjustment_indices = rounded_values.nlargest(int(abs(difference))).index if difference < 0 else rounded_values.nsmallest(int(abs(difference))).index
                rounded_values.loc[adjustment_indices] += np.sign(difference)
            
            output_df.loc[ilce_mask, evcs_type] = rounded_values

    # Yeni sonuçları kümülatif olarak ekle
    for evcs_type in evcs_types.values():
        count_col = evcs_type + '_count'
        previous_results[count_col] += output_df[count_col]

    # Her hücre için toplam yük hesapla
    previous_results['toplam_yuk'] = 0
    for evcs_type, power in evcs_power.items():
        count_col = evcs_type + '_count'
        previous_results['toplam_yuk'] += previous_results[count_col] * power

    # Güncellenmiş kümülatif sonuçları yazdır
    previous_results.to_excel(output_writer, sheet_name=str(year), index=False)

# Dosyayı kaydet ve kapat
output_writer.close()

print("Kümülatif sonuçlar başarıyla kaydedildi.")