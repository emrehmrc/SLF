import pandas as pd
from sklearn.cluster import KMeans
import numpy as np
import logging
from tqdm import tqdm
import sys


# Logging ayarları
logging.basicConfig(filename='dek_distribution.log', level=logging.INFO, 
                    format='%(asctime)s - %(levelname)s - %(message)s')

# Sabit dosya yolları
min_max_path = r'C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek\v2\girdi\Probability of DEK Placement_monteCarlo2.xlsx'
imar_tipi_bilgileri_path = r'C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek\v2\girdi\construction_areas2.xlsx'  # Bu dosya Image 2'deki bilgileri içermeli
#input_file_path = sys.argv[1]  # The input file path passed as an argument
# Min-max dosyasını yükleyelim
min_max_df = pd.read_excel(min_max_path)

# İmar tipi bilgilerini içeren dosyayı yükleyelim
imar_tipi_bilgileri_df = pd.read_excel(imar_tipi_bilgileri_path)

# Sütun isimlerini temizleyelim
min_max_df.columns = min_max_df.columns.str.strip()
min_max_df.columns = min_max_df.columns.str.replace('–', '-')
min_max_df.columns = min_max_df.columns.str.replace('—', '-')

# DEK_Probability kategorisini alalım
dek_probability_type = 'DEK_Probability'

# Parametre sütun isimlerini min_max_df'den alalım
# Min-max sütun isimlerinden parametre isimlerini çıkaralım
parameter_columns = [col.replace(' - min', '').replace(' - max', '') 
                    for col in min_max_df.columns 
                    if ' - min' in col]
parameter_columns = list(set(parameter_columns))  # Tekrarları temizle

# DEK işlemleri için Monte Carlo fonksiyonu - bina sayısı artışına göre güncellenmiş
def monte_carlo_simulation_with_building_growth(imar_analyses_df, min_max_df, imar_tipi_bilgileri_df, 
                                               karşıyaka_results_df, num_simulations=300):
    results = []
    
    # Tüm simülasyonları tqdm ile ilerleme çubuğu göstererek çalıştır
    for _ in tqdm(range(num_simulations), desc="Running Monte Carlo simulations"):
        # Her imar tipi için çatı alanı hesapla
        roof_areas_df = calculate_roof_areas(imar_analyses_df, imar_tipi_bilgileri_df)
        
        # Ağırlıklandırmalar için rasgele parametreler oluştur
        randomized_weights = create_random_weights(min_max_df, parameter_columns)
        
        # Mevcut metodolojiyi uygula, ancak imar tipleri yerine çatı alanlarını kullanarak
        simulation_result = apply_methodology_with_roof_areas(roof_areas_df, randomized_weights, karşıyaka_results_df)
        
        results.append(simulation_result)
    
    # Tüm sonuçları birleştir ve ortalamasını al
    final_results = pd.concat(results).groupby(['id', 'left', 'top', 'right', 'bottom']).mean().reset_index()
    return final_results

# Çatı alanlarını hesapla
def calculate_roof_areas(imar_analyses_df, imar_tipi_bilgileri_df):
    """Her hücre için imar tiplerine göre toplam çatı alanını hesaplar"""
    # Sonuç için imar_analyses_df'in bir kopyasını oluştur
    roof_areas_df = imar_analyses_df[['id', 'left', 'top', 'right', 'bottom', 'ilce']].copy()
    
    # Her imar tipi için çatı alanı hesapla
    for _, row in imar_tipi_bilgileri_df.iterrows():
        imar_tipi = row['imar_tipi']
        bina_basi_brut_alan = row['bina_başı_brüt_alan']
        taks = row['taks'] if 'taks' in row and not pd.isna(row['taks']) else 0.5  # Varsayılan TAKS
        
        # Eğer imar tipi, imar_analyses_df'de varsa hesapla
        if imar_tipi in imar_analyses_df.columns:
            # Her hücre için bu imar tipindeki binaların toplam çatı alanını hesapla
            # Bina sayısı * brüt alan * TAKS (taban alanı katsayısı)
            roof_areas_df[f'{imar_tipi}_roof_area'] = imar_analyses_df[imar_tipi] * bina_basi_brut_alan * taks
        else:
            # İmar tipi yoksa, 0 olarak ayarla
            roof_areas_df[f'{imar_tipi}_roof_area'] = 0
    
    # Toplam çatı alanını hesapla
    roof_area_columns = [col for col in roof_areas_df.columns if '_roof_area' in col]
    roof_areas_df['total_roof_area'] = roof_areas_df[roof_area_columns].sum(axis=1)
    
    return roof_areas_df

# Rasgele ağırlıklar oluştur
def create_random_weights(min_max_df, parameter_columns):
    """Monte Carlo simülasyonu için min-max aralığında rasgele ağırlıklar oluşturur"""
    # Sonuç için DataFrame oluştur
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
    
    # Ağırlıkları normalize et
    normalization_factor = sum(random_values)
    if normalization_factor > 0:
        for i, col in enumerate(parameter_columns):
            randomized_weights_df.loc[0, col] = random_values[i] / normalization_factor
    
    return randomized_weights_df

# Çatı alanlarına göre metodolojiyi uygula
def apply_methodology_with_roof_areas(roof_areas_df, randomized_weights, karşıyaka_results_df):
    """Çatı alanlarını kullanarak DEK dağıtım metodolojisini uygular"""
    if 'ilce' not in roof_areas_df.columns:
        raise KeyError("'ilce' sütunu roof_areas_df veri çerçevesinde bulunamadı.")
    
    results_df = roof_areas_df[['id', 'left', 'top', 'right', 'bottom', 'ilce', 'total_roof_area']].copy()
    
    # İlçe bazında hesapla
    for ilce in results_df['ilce'].unique():
        ilce_mask = results_df['ilce'] == ilce
        
        # İlçedeki toplam çatı alanı
        ilce_roof_areas = roof_areas_df.loc[ilce_mask].copy()
        
        # Toplam çatı alanına göre normalize et (DEK potansiyeli)
        total_roof_area_ilce = ilce_roof_areas['total_roof_area'].sum()
        
        if total_roof_area_ilce > 0:
            results_df.loc[ilce_mask, 'DEK_normalized'] = ilce_roof_areas['total_roof_area'] / total_roof_area_ilce
        else:
            results_df.loc[ilce_mask, 'DEK_normalized'] = 0
        
        # İlçedeki toplam DEK kapasitesini dağıt
        total_dek = karşıyaka_results_df.loc[karşıyaka_results_df['ilce'] == ilce, 'Installed Capacity'].values[0]
        results_df.loc[ilce_mask, 'DEK_distributed'] = results_df.loc[ilce_mask, 'DEK_normalized'] * total_dek
    
    return results_df

# Çok yıllık hesaplama için ana döngü
def run_multiyear_simulation():
    try:
        output_writer = pd.ExcelWriter('C:/Users/begum.orhan/OneDrive - MRC/Masaüstü/SLF/arda/EA-DEK/dek/v2/cıktı/dek_distribution_cumulative_0704.xlsx', engine='xlsxwriter')
        years = range(2024, 2036)  # 2035 yılını da dahil etmek için 2036'ya kadar

        # İlk yıl için başlangıç durumu
        previous_year_df = pd.read_excel(r'C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek\v2\girdi\initial_state_dek_izmir.xlsx')
        
        # X ve Y koordinatları ekle
        previous_year_df['x_koordinat'] = (previous_year_df['left'] + previous_year_df['right']) / 2
        previous_year_df['y_koordinat'] = (previous_year_df['top'] + previous_year_df['bottom']) / 2

        for year in years:
            logging.info(f"Processing year {year}")
            
            # O yıl için imar analizleri ve karşıyaka sonuçlarını oku
            imar_analyses_df = pd.read_excel(r'C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek\v2\girdi\new_buildings_2024_2035_1703.xlsx', sheet_name=str(year))
            #imar_analyses_df = pd.read_excel(input_file_path, sheet_name=str(year))
            karşıyaka_results_df = pd.read_excel(r'C:\Users\begum.orhan\OneDrive - MRC\Masaüstü\SLF\arda\EA-DEK\dek\v2\girdi\yeni_results3_DEK2_delta.xlsx', sheet_name=str(year))

            # Monte Carlo simülasyonu çalıştır
            final_results = monte_carlo_simulation_with_building_growth(
                imar_analyses_df, min_max_df, imar_tipi_bilgileri_df, karşıyaka_results_df)
            
            # X ve Y koordinatları hesapla
            final_results['x_koordinat'] = (final_results['left'] + final_results['right']) / 2
            final_results['y_koordinat'] = (final_results['top'] + final_results['bottom']) / 2

            # K-Means ve sonuçların güncellenmesi
            output_df = optimize_with_kmeans(final_results)

            # Yeni sonuçları kümülatif olarak önceki yıla ekle
            dek_types_counts = ['DEK_distributed']
            previous_year_df[dek_types_counts] += output_df[dek_types_counts]

            # Yıllık sonuçları çıktı dosyasına yaz
            previous_year_df.to_excel(output_writer, sheet_name=str(year), index=False)
            logging.info(f"Year {year} processed successfully")

        # Dosyayı kaydet ve kapat
        output_writer.close()
        print("All results saved to 'dek_distribution_cumulative.xlsx'")
        
    except Exception as e:
        logging.error(f"Error in run_multiyear_simulation: {e}")
        raise

# K-Means ile optimizasyon
def optimize_with_kmeans(final_results):
    """K-Means kümeleme kullanarak DEK dağıtımını optimize eder"""
    output_df = final_results.copy()
    dek_types_counts = ['DEK_distributed']
    
    for ilce in output_df['ilce'].unique():
        ilce_mask = output_df['ilce'] == ilce
        ilce_df = output_df.loc[ilce_mask].copy()
        
        for dek_type in dek_types_counts:
            values = ilce_df[[dek_type]].values
            
            # K-Means için minimum örnek sayısı kontrolü
            if len(values) < 5:  # Eğer çok az hücre varsa kümeleme yapma
                continue
                
            # Silhouette score ile optimum küme sayısını bul
            from sklearn.metrics import silhouette_score
            
            best_score = -1
            best_n_clusters = 4  # Varsayılan küme sayısı
            
            # En az 2, en çok min(6, hücre sayısı/2) küme dene
            max_clusters = min(6, len(values) // 2)
            for n_clusters in range(2, max_clusters + 1):
                try:
                    kmeans = KMeans(n_clusters=n_clusters, random_state=42)
                    cluster_labels = kmeans.fit_predict(values)
                    
                    # Silhouette skoru hesapla
                    if len(set(cluster_labels)) > 1:  # Birden fazla küme varsa
                        score = silhouette_score(values, cluster_labels)
                        if score > best_score:
                            best_score = score
                            best_n_clusters = n_clusters
                except Exception as e:
                    logging.warning(f"Error in silhouette calculation with {n_clusters} clusters: {e}")
            
            # En iyi küme sayısı ile K-Means uygula
            kmeans = KMeans(n_clusters=best_n_clusters, random_state=42)
            ilce_df['cluster'] = kmeans.fit_predict(values)
            
            # Küme ortalamalarını hesapla
            cluster_averages = ilce_df.groupby('cluster')[dek_type].mean()
            
            # En yüksek ortalamaya sahip kümeyi bul
            max_cluster = cluster_averages.idxmax()
            
            # Diğer kümelerdeki değerleri en yüksek ortalamaya sahip kümeye dağıt
            for cluster in cluster_averages.index:
                if cluster != max_cluster:
                    cluster_mask = ilce_df['cluster'] == cluster
                    total_to_distribute = ilce_df.loc[cluster_mask, dek_type].sum()
                    
                    max_cluster_mask = ilce_df['cluster'] == max_cluster
                    max_cluster_values = ilce_df.loc[max_cluster_mask, dek_type]
                    
                    # Eğer max küme boş değilse
                    if not max_cluster_values.empty:
                        distribution_ratios = max_cluster_values / max_cluster_values.sum()
                        ilce_df.loc[max_cluster_mask, dek_type] += distribution_ratios * total_to_distribute
                        ilce_df.loc[cluster_mask, dek_type] = 0
            
            ilce_df.drop(columns=['cluster'], inplace=True)
        
        output_df.loc[ilce_mask, dek_types_counts] = ilce_df[dek_types_counts]
    
    return output_df

# Programı çalıştır
if __name__ == "__main__":
    run_multiyear_simulation()