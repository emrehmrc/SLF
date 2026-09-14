import os
import json
import sys
import pandas as pd
import numpy as np
from itertools import product

# Configure stdout for UTF-8 to support Turkish characters in console
import sys
sys.stdout.reconfigure(encoding='utf-8')

# Load config
print("Yükleniyor: Yapılandırma dosyası...")
config_path = sys.argv[1]
user_home = os.path.expanduser('~')
with open(config_path, 'r', encoding='utf-8') as f:
    config = json.load(f)

# Extract config variables
ana_klasor_yolu = config['Ana_Klasör_Yolu']
il = config['İl']
ilce = config['İlçe']
proje_ismi = config['proje_ismi']
start_year = int(config['baslangıc_yılı'])
end_year = int(config['bitis_yılı'])
print(f"Yapılandırma yüklendi: başlangıç_yılı={start_year}, bitiş_yılı={end_year}, il={il}, ilçe={ilce}, proje_ismi={proje_ismi}")

# Construct file paths
file_path = os.path.join(
    user_home, ana_klasor_yolu, il, ilce, proje_ismi,
    'imar_analizi_sonuclari', 'imar_planlari', 'saturasyon', 'saturasyon_sonuc', 'saturasyon.csv'
)
hucre_mahalleler_path = os.path.join(
    user_home, ana_klasor_yolu, il, 'Mahalleler', 'hucre_mahalleler.csv'
)
print(f"CSV dosyası okunuyor: {file_path}")
print(f"hucre_mahalleler.csv dosyası okunuyor: {hucre_mahalleler_path}")

# Check if hucre_mahalleler.csv exists
if not os.path.exists(hucre_mahalleler_path):
    print(f"Hata: hucre_mahalleler.csv dosyası bulunamadı: {hucre_mahalleler_path}")
    sys.exit(1)

# Step 1: Import CSV as DataFrame with encoding handling and explicit 'id' dtype
def read_csv_with_encoding(file_path, encodings=['utf-8-sig', 'cp1254', 'utf-8', 'latin1']):
    for encoding in encodings:
        try:
            df = pd.read_csv(file_path, encoding=encoding, dtype={'id': str})
            print(f"CSV dosyası {encoding} kodlamasıyla başarıyla yüklendi.")
            return df
        except UnicodeDecodeError:
            print(f"CSV dosyası {encoding} kodlamasıyla yüklenemedi, başka kodlama deneniyor...")
    print(f"Hata: Hiçbir kodlama ile {file_path} dosyası okunamadı.")
    sys.exit(1)

# Read saturation.csv
df = read_csv_with_encoding(file_path)
print(f"CSV dosyası yüklendi. DataFrame boyutu: {df.shape}")

# Read hucre_mahalleler.csv
hucre_mahalleler_df = read_csv_with_encoding(hucre_mahalleler_path)
print(f"hucre_mahalleler.csv yüklendi. DataFrame boyutu: {hucre_mahalleler_df.shape}")

# Debug column names
print("hucre_mahalleler_df columns:", hucre_mahalleler_df.columns)
print("\n\n\n")
print("df columns:", df.columns)

# Step 1.5: Left join df with hucre_mahalleler_df on 'id' and rename 'Name' to 'Mahalle_Id'
# Drop duplicates in hucre_mahalleler_df based on 'id', keeping the first occurrence
hucre_mahalleler_df = hucre_mahalleler_df.drop_duplicates(subset=['id'], keep='first')
df = df.merge(hucre_mahalleler_df[['id', 'Name']], on='id', how='left', suffixes=('', '_drop'))
df = df.drop(columns=[col for col in df.columns if '_drop' in col])
df = df.rename(columns={'Name': 'Mahalle_Id'})
print(f"DataFrame 'Mahalle_Id' kolonu ile birleştirildi. Yeni boyutu: {df.shape}")

# Step 2: Create hücre_tipi column on original DataFrame
print("hücre_tipi kolonu oluşturuluyor (orijinal DataFrame üzerinde)...")
def assign_hucre_tipi(row):
    if row['TOPLAM_BINA_SAYISI'] == 0:
        return 'Mesken'
    mesken_ratio = row['MESKEN SAYISI'] / row['TOPLAM_BINA_SAYISI'] if row['TOPLAM_BINA_SAYISI'] > 0 else 0
    sanayi_sum = row['BUYUK_SANAYI'] + row['KUCUK_SANAYI'] + row['ORTA_SANAYI']
    sanayi_ratio = sanayi_sum / row['TOPLAM_BINA_SAYISI'] if row['TOPLAM_BINA_SAYISI'] > 0 else 0
    ticarethane_sum = row['BUYUK_TICARETHANE'] + row['KUCUK_TICARETHANE'] + row['ORTA_TICARETHANE']
    ticarethane_ratio = ticarethane_sum / row['TOPLAM_BINA_SAYISI'] if row['TOPLAM_BINA_SAYISI'] > 0 else 0
    
    if mesken_ratio >= 0.5:
        return 'Mesken'
    elif sanayi_ratio > 0.25:
        return 'Sanayi'
    elif ticarethane_ratio > 0.25:
        return 'Ticarethane'
    else:
        return 'Mesken'

# Ensure relevant columns are numeric
numeric_cols = ['TOPLAM_BINA_SAYISI', 'MESKEN SAYISI', 'BUYUK_SANAYI', 'KUCUK_SANAYI', 
                'ORTA_SANAYI', 'BUYUK_TICARETHANE', 'KUCUK_TICARETHANE', 'ORTA_TICARETHANE']
for col in numeric_cols:
    if col in df.columns:
        df[col] = pd.to_numeric(df[col], errors='coerce')
        if df[col].isna().any():
            print(f"Uyarı: {col} kolonunda sayısal dönüşümden sonra NaN değerler var")

df['hücre_tipi'] = df.apply(assign_hucre_tipi, axis=1)
print(f"hücre_tipi kolonu oluşturuldu. Benzersiz değerler: {df['hücre_tipi'].unique()}")

# Step 3: Divide bu_ columns by bu_{start_year} / Saturation_updated_{start_year} and cap at 1
saturation_col = f'Saturation_updated_{start_year}'
bu_start_year_col = f'bu_{start_year}'
if saturation_col not in df.columns or bu_start_year_col not in df.columns:
    raise ValueError(f"Kolonlar {saturation_col} veya {bu_start_year_col} DataFrame'de bulunamadı")
print(f"bu_ kolonları {bu_start_year_col} / {saturation_col} oranına bölünüyor")

# Ensure bu_ columns and saturation column are numeric
bu_columns = [f'bu_{year}' for year in range(2005, 2031)]
for col in bu_columns + [saturation_col, bu_start_year_col]:
    if col in df.columns:
        df[col] = pd.to_numeric(df[col], errors='coerce')
        if df[col].isna().any():
            print(f"Uyarı: {col} kolonunda sayısal dönüşümden sonra NaN değerler var")

# Handle NaN in ratio calculation
ratio = np.where(df[saturation_col].isna() | (df[saturation_col] == 0), 1, df[bu_start_year_col] / df[saturation_col])
for col in bu_columns:
    if col in df.columns:
        df[col] = df[col] / ratio
        # Cap values at 1
        capped_count = (df[col] > 1.0).sum()
        if capped_count > 0:
            print(f"{col} kolonunda {capped_count} değer 1'i aştı ve 1'e sabitlendi")
            df[col] = df[col].clip(upper=1.0)
        print(f"{col} kolonu orana bölündü")
    else:
        print(f"{col} kolonu DataFrame'de bulunamadı")

normalized_saturation_path = os.path.join(
    user_home, ana_klasor_yolu, il, ilce, proje_ismi,
    'imar_analizi_sonuclari', 'imar_planlari', 'saturasyon', 'saturasyon_sonuc', 'normalized_saturasyon.csv'
)

# Save normalized DataFrame for manual inspection
print("Normalleştirilmiş DataFrame 'normalized_saturasyon.csv' dosyasına kaydediliyor...")
df.to_csv(normalized_saturation_path, encoding='cp1254', index=False)
print("Normalleştirilmiş DataFrame kaydedildi.")

# Step 4: Filter rows with growth from below 0.1 to 0.9 or above, truncated at first >= 0.9
print("0.1'in altından 0.9'a (veya üzerine) büyüme gösteren satırları filtreleme, ilk 0.9 veya üstünde kesiliyor...")
def has_growth_from_below_01_to_09_or_above(row):
    years = list(range(2005, start_year + 1))
    bu_values = [row[f'bu_{year}'] for year in years if f'bu_{year}' in row and pd.notna(row[f'bu_{year}'])]
    if not bu_values or not all(np.isfinite(bu_values)):
        return False
    # Find the last index where value is < 0.1
    start_index = max((i for i, val in enumerate(bu_values) if val < 0.1), default=None)
    if start_index is None or start_index == len(bu_values) - 1:
        return False
    # Find the first index after start_index where value >= 0.9
    end_index = min((i for i in range(start_index + 1, len(bu_values)) if bu_values[i] >= 0.9), default=None)
    if end_index is None:
        return False
    return True

# Filter DataFrame
filtered_df = df[df.apply(has_growth_from_below_01_to_09_or_above, axis=1)].copy()
print(f"Filtreleme sonrası DataFrame boyutu: {filtered_df.shape}")

filtered_saturation_path = os.path.join(
    user_home, ana_klasor_yolu, il, ilce, proje_ismi,
    'imar_analizi_sonuclari', 'imar_planlari', 'saturasyon', 'saturasyon_sonuc', 'filtered_saturasyon.csv'
)

# Save filtered DataFrame for inspection
print("Filtrelenmiş DataFrame 'filtered_saturasyon.csv' dosyasına kaydediliyor...")
filtered_df.to_csv(filtered_saturation_path, encoding='cp1254', index=False)
print("Filtrelenmiş DataFrame kaydedildi.")

# Step 5: Identify growth years and group by Mahalle_Id and hücre_tipi
print("Büyüme yılları tespit etme ve gruplama yapılıyor...")
def find_growth_years(row):
    years = list(range(2005, start_year + 1))
    bu_values = [row[f'bu_{year}'] for year in years if f'bu_{year}' in row and pd.notna(row[f'bu_{year}'])]
    if not bu_values or not all(np.isfinite(bu_values)):
        return None
    # Find the last index where value is < 0.1
    start_index = max((i for i, val in enumerate(bu_values) if val < 0.1), default=None)
    if start_index is None or start_index == len(bu_values) - 1:
        return None
    # Find the first index after start_index where value >= 0.9
    end_index = min((i for i in range(start_index + 1, len(bu_values)) if bu_values[i] >= 0.9), default=None)
    if end_index is None:
        return None
    return years[end_index] - years[start_index]

filtered_df['growth_years'] = filtered_df.apply(find_growth_years, axis=1)
if filtered_df['growth_years'].isna().all():
    print("Uyarı: Filtrelenmiş satırlarda 0.1'in altından 0.9'a (veya üzerine) büyüme tespit edilemedi. Çıkış yapılıyor.")
    exit()

# Group by Mahalle_Id and hücre_tipi, calculate average growth years
grouped_df = filtered_df.groupby(['Mahalle_Id', 'hücre_tipi'])['growth_years'].mean().reset_index()
print(f"Gruplandırılmış DataFrame oluşturuldu. Boyut: {grouped_df.shape}")

# Ensure all Mahalle_Id and hücre_tipi combinations are represented
all_mahalle_ids = df['Mahalle_Id'].unique()
all_hucre_tipi = ["Mesken", "Sanayi", "Ticarethane"]  # Force all three types
all_combinations = pd.DataFrame(list(product(all_mahalle_ids, all_hucre_tipi)), columns=['Mahalle_Id', 'hücre_tipi'])
grouped_df = all_combinations.merge(grouped_df, on=['Mahalle_Id', 'hücre_tipi'], how='left', suffixes=('', '_drop'))
grouped_df = grouped_df.drop(columns=[col for col in grouped_df.columns if '_drop' in col])
grouped_df['growth_years'] = grouped_df['growth_years'].fillna(0)  # Use 0 for groups with no growth
print(f"Tam gruplandırılmış DataFrame boyutu: {grouped_df.shape}")

# Step 6: Fit S-curve (logistic function) and generate output
print("S-eğri (lojistik fonksiyon) uyduruluyor ve çıktı oluşturuluyor...")
def logistic_function(t, L, k, t0):
    return L / (1 + np.exp(-k * (t - t0)))

output_data = []
for idx, row in grouped_df.iterrows():
    mahalle_id = row['Mahalle_Id']
    hucre_tipi = row['hücre_tipi']
    # Count observations in the group
    group_size = filtered_df[(filtered_df['Mahalle_Id'] == mahalle_id) & (filtered_df['hücre_tipi'] == hucre_tipi)].shape[0]
    avg_years = row['growth_years'] if pd.notna(row['growth_years']) and row['growth_years'] > 0 else 10
    if group_size <= 1:  # Apply fixed years for group_size 0 or 1
        if hucre_tipi == "Sanayi":
            avg_years = np.random.randint(4, 9)  # Random integer between 4 and 8
        elif hucre_tipi == "Ticarethane":
            avg_years = np.random.randint(5, 10)  # Random integer between 5 and 9
        elif hucre_tipi == "Mesken":
            avg_years = np.random.randint(6, 12)  # Random integer between 6 and 11
    avg_years = int(np.ceil(avg_years))  # Round up to nearest integer
    print(f"Processing group: Mahalle_Id={mahalle_id}, hücre_tipi={hucre_tipi}, avg_years={avg_years}, group_size={group_size}")

    # Set logistic parameters for S-curve shape
    L = 1.0  # Maximum value (saturates at 1)
    t0 = avg_years * 0.6  # Midpoint at 60% for balanced growth
    k = 12 / avg_years  # Steepen the curve for S-shape

    # Generate dynamic number of points based on avg_years
    num_points = avg_years  # Number of years, from Year_0 to Year_{avg_years - 1}
    time_points = np.linspace(0, avg_years, num_points)
    s_curve_values = logistic_function(time_points, L, k, t0)

    # Ensure S-curve properties and 0-to-1 constraint
    s_curve_values[0] = 0.0  # Start at 0
    s_curve_values = np.clip(s_curve_values, 0.0, 1.0)  # Enforce 0 to 1 range

    # Introduce randomness with controlled perturbation
    randomized_values = [s_curve_values[0]]  # Start with 0.0
    for i in range(1, len(s_curve_values) - 1):  # Exclude last value
        base_value = s_curve_values[i]
        # Random noise: ±0.15 or ±15% of base value, whichever is smaller
        noise = np.random.uniform(-0.15, 0.15)
        max_noise = min(0.15, base_value * 0.15) if base_value > 0 else 0.15
        noise = max(min(noise, max_noise), -max_noise)  # Cap noise
        new_value = base_value + noise
        # Ensure monotonicity and cap penultimate value at 0.95
        if i == len(s_curve_values) - 2:  # Penultimate value
            new_value = max(randomized_values[-1] + 0.01, min(new_value, 0.95))
        else:
            new_value = max(randomized_values[-1] + 0.01, min(new_value, 0.95))
        randomized_values.append(new_value)
    randomized_values.append(1.0)  # Set the last value to 1.0

    # Minimal adjustments for S-curve shape, ensuring monotonicity
    prev_value = 0.0
    for i in range(1, len(randomized_values)):
        if randomized_values[i] < prev_value:
            randomized_values[i] = prev_value + 0.01  # Ensure monotonic increase
        elif i < int(avg_years * 0.3) and randomized_values[i] < 0.1:
            randomized_values[i] = max(prev_value + 0.01, randomized_values[i] * 1.5)  # Slight initial growth
        elif int(avg_years * 0.3) <= i < int(avg_years * 0.7):
            randomized_values[i] = min(prev_value + 0.1, randomized_values[i] * 1.8)  # Steepen middle growth
        # Cap all values except the last one at 0.95
        if i < len(randomized_values) - 1:
            randomized_values[i] = min(randomized_values[i], 0.95)
        prev_value = randomized_values[i]

    randomized_values = np.clip(randomized_values, 0.0, 1.0)  # Re-clip after adjustments

    # Get cell IDs for the group
    group_cells = filtered_df[(filtered_df['Mahalle_Id'] == mahalle_id) & (filtered_df['hücre_tipi'] == hucre_tipi)]
    cell_ids = ', '.join(group_cells['id'].astype(str).tolist()) if not group_cells.empty else 'None'

    # Create row with exact number of columns based on avg_years
    row_data = [mahalle_id, hucre_tipi, group_size, cell_ids] + [f"{val:.3f}" for val in randomized_values]
    output_data.append(row_data)

# Create output DataFrame with dynamic columns
max_columns = max(len(row) - 4 for row in output_data)  # Max number of Year columns, excluding Mahalle_Id, hücre_tipi, num_observations, cell_ids
columns = ['Mahalle_Id', 'hücre_tipi', 'num_observations', 'cell_ids'] + [f'Year_{i}' for i in range(max_columns)]
output_df = pd.DataFrame(output_data, columns=columns)

# Pad shorter rows with empty strings to match max columns
for i in range(len(output_df)):
    current_length = len(output_df.iloc[i]) - 4  # Exclude the first 4 columns
    if current_length < max_columns:
        output_df.iloc[i, 4 + current_length:] = [''] * (max_columns - current_length)

# Step 7: Merge output_df with original df
print("Orijinal DataFrame ile output_df birleştiriliyor...")
merged_df = df.merge(output_df, on=['Mahalle_Id', 'hücre_tipi'], how='left')
print(f"Birleştirilmiş DataFrame boyutu: {merged_df.shape}")

merged_saturation_path = os.path.join(
    user_home, ana_klasor_yolu, il, ilce, proje_ismi,
    'imar_analizi_sonuclari', 'imar_planlari', 'saturasyon', 'saturasyon_sonuc', 'merged_saturasyon.csv'
)

# Save merged DataFrame
print("Birleştirilmiş DataFrame 'merged_saturasyon.csv' dosyasına kaydediliyor...")
merged_df.to_csv(merged_saturation_path, encoding='cp1254', index=False)
print("Birleştirilmiş DataFrame kaydedildi.")

s_curve_predictions_path = os.path.join(
    user_home, ana_klasor_yolu, il, ilce, proje_ismi,
    'imar_analizi_sonuclari', 'imar_planlari', 'saturasyon', 'saturasyon_sonuc', 's_curve_tahminleri.csv'
)

# Save or output the resulting output_df
print("Sonuçlar CSV dosyasına kaydediliyor...")
output_df.to_csv(s_curve_predictions_path, encoding='cp1254', index=False)
print("İşlem tamamlandı. Sonuçlar 's_curve_tahminleri.csv' dosyasına kaydedildi")
