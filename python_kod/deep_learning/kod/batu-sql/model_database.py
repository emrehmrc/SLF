import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from sklearn.preprocessing import StandardScaler
import joblib
import xgboost as xgb
from sqlalchemy import create_engine

# Database connection parameters
DB_CONNECTIONS = {
    "gdz": "postgresql://postgres:12345@localhost:5432/gdz",
    "oedas": "postgresql://postgres:12345@localhost:5432/oedas"
}

# Function to save DataFrame to PostgreSQL database
def save_to_database(df, table_name, db_choice):
    """DataFrame'i belirtilen tablo adıyla PostgreSQL veritabanına kaydeder."""
    try:
        engine = create_engine(DB_CONNECTIONS[db_choice])
        df.to_sql(table_name, con=engine, if_exists="replace", index=False)
        print(f"Veri '{table_name}' tablosuna başarıyla kaydedildi.")
    except Exception as e:
        print(f"Veritabanına yükleme sırasında hata oluştu: {e}")

# Load the data (replace with your actual file path)
abone_verisi_path = '../../Veriler/241004_Abone_Verileri_GDZ.csv'
data = pd.read_csv(abone_verisi_path, encoding="cp1254")

# MESKEN SAYISI ORANI DEGISKENI
mesken_oranı = 0.7

# Define connection parameters
buyuk_sanayi_baglantı_gucu = 150
buyuk_sanayi_tuketim = 300000
kucuk_sanayi_baglantı_gucu = 50
kucuk_sanayi_tuketim = 20000
buyuk_ticarethane_baglantı_gucu = 400
buyuk_ticarethane_tuketim = 200000
kucuk_ticarethane_baglantı_gucu = 15
kucuk_ticarethane_tuketim = 10000

# Step 1: Group the data by ADR_BINA_ID and calculate relevant aggregations
aggregations = {
    'Y_KOORDINAT': 'first',
    'X_KOORDINAT': 'first',
    'ENERJI_TABLO_KAYIT_KODU': 'first',
    'IL_KODU': 'first',
    'ILCE_ADI': 'first',
    'MAHALLE': 'first',
    'BAGLANTI_GUCU': 'mean',  # Average BAGLANTI_GUCU (ORT_BAG_GUCU)
    '2023_Tuketim': 'sum'
}

# Adding ABONE_GRUBU specific counts and sums using pivot and merging back to the main dataset
abone_counts = pd.pivot_table(data, index='ADR_BINA_ID', columns='ABONE_GRUBU', 
                              values='TESISAT_NO', aggfunc='count', fill_value=0).add_suffix('_count')
abone_tuketim = pd.pivot_table(data, index='ADR_BINA_ID', columns='ABONE_GRUBU', 
                               values='2023_Tuketim', aggfunc='sum', fill_value=0).add_suffix('_tuketim')

# Step 2: Create the main pivot table with basic aggregations
pivot_table = data.groupby('ADR_BINA_ID').agg(aggregations).reset_index()

# Rename 'BAGLANTI_GUCU' to 'ORT_BAG_GUCU'
pivot_table.rename(columns={'BAGLANTI_GUCU': 'ORT_BAG_GUCU'}, inplace=True)

# Merge the counts and consumption values back into the pivot table
pivot_table = pivot_table.merge(abone_counts, on='ADR_BINA_ID', how='left').merge(abone_tuketim, on='ADR_BINA_ID', how='left')

# Step 5: Add the "ABONE SAYISI" column as the sum of all count columns
count_columns = ['MESKEN_count', 'SANAYI_count', 'TICARETHANE_count']
pivot_table['MESKEN_SANAYI_TICARETHANE_SAYISI'] = pivot_table[count_columns].sum(axis=1)

pivot_table = pivot_table[pivot_table['MESKEN_count'] < 500]

# Step 3: Add the "BINA_TIPI" column based on the specified conditions
def determine_bina_tipi(row):
    if row['MESKEN_SANAYI_TICARETHANE_SAYISI'] == 0:
        if row['TARIMSALSULAMA_count'] >= 1:
            return "TARIMSAL_SULAMA"
        elif row['AYDINLATMA_count'] >= 1:
            return "AYDINLATMA"
    elif row['MESKEN_count'] / row['MESKEN_SANAYI_TICARETHANE_SAYISI'] >= mesken_oranı:
        return "MESKEN"
    elif row['SANAYI_count'] >= 1:
        if row['ORT_BAG_GUCU'] >= buyuk_sanayi_baglantı_gucu and row['SANAYI_tuketim'] >= buyuk_sanayi_tuketim:
            return "BUYUK_SANAYI"
        elif row['ORT_BAG_GUCU'] <= kucuk_sanayi_baglantı_gucu and row['SANAYI_tuketim'] <= kucuk_sanayi_tuketim:
            return "KUCUK_SANAYI"
        else:
            return "ORTA_SANAYI"
    elif row['TICARETHANE_count'] >= 1:  
        if row['ORT_BAG_GUCU'] >= buyuk_ticarethane_baglantı_gucu and row['TICARETHANE_tuketim'] >= buyuk_ticarethane_tuketim:
            return "BUYUK_TICARETHANE"
        elif row['ORT_BAG_GUCU'] <= kucuk_ticarethane_baglantı_gucu and row['TICARETHANE_tuketim'] <= kucuk_ticarethane_tuketim:
            return "KUCUK_TICARETHANE"
        else:
            return "ORTA_TICARETHANE"
    else:
        return "DIGER"

# Apply the function to create the BINA_TIPI column
pivot_table['BINA_TIPI'] = pivot_table.apply(determine_bina_tipi, axis=1)

# Step 4: Divide the data into two datasets based on BINA_TIPI
mesken_data = pivot_table[pivot_table['BINA_TIPI'] == 'MESKEN'].copy()
other_data = pivot_table[pivot_table['BINA_TIPI'] != 'MESKEN'].copy()

# Step 5: Add the "ABONE SAYISI" column as the sum of all count columns
count_columns = ['MESKEN_count', 'SANAYI_count', 'TICARETHANE_count', 'TARIMSALSULAMA_count', 'AYDINLATMA_count', 'URETICI_count']
mesken_data['ABONE SAYISI'] = mesken_data[count_columns].sum(axis=1)

# Step 6: Add a new column "IMAR_ID" and set all values to 1
mesken_data['IMAR_ID'] = 1

# Step 7: Rename "MESKEN_tuketim" to "MESKENTUKETIM"
mesken_data.rename(columns={'MESKEN_tuketim': 'MESKENTUKETIM'}, inplace=True)

# Step 8: Add a new column "ORT_Mesken_tuketim" which is MESKENTUKETIM divided by MESKEN_count
mesken_data['ORT_Mesken_tuketim'] = mesken_data['MESKENTUKETIM'] / mesken_data['MESKEN_count']

mesken_data.rename(columns={'MESKEN_count': 'MESKEN SAYISI'}, inplace=True)

# User input for city selection
city = input("Şehir seçin (İzmir/Eskisehir): ").strip().lower()

# Set database choice based on city
if city == "izmir":
    db_choice = "gdz"
    mesken_table = "deep_learning_mesken"
    other_table = "deep_learning_other"
elif city == "eskisehir":
    db_choice = "oedas"
    mesken_table = "deep_learning_mesken"
    other_table = "deep_learning_other"
else:
    raise ValueError("Geçersiz şehir. Lütfen 'İzmir' veya 'Eskisehir' girin.")

# Save mesken data to database
save_to_database(mesken_data, mesken_table, db_choice)

# Save other data to database
save_to_database(other_data, other_table, db_choice)

# Load training data for machine learning
imar_train_verisi_path = "../../Input File/ML_TRAIN_DATA.xlsx"
Data = pd.read_excel(imar_train_verisi_path, header=0, index_col=False, sheet_name=0)
print('Data')
print(len(Data))

# Adjust the target labels to be zero-indexed
Data['IMAR_ID'] = Data['IMAR_ID'] - 1

X = Data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']]
Y = Data['IMAR_ID']

# Standardize the data
scaler = StandardScaler()
X_scaled = scaler.fit_transform(X)

# Split data
X_train, X_test, y_train, y_test = train_test_split(X_scaled, Y, test_size=0.2, random_state=42)

# Initialize and train XGBoost model
xgb_model = xgb.XGBClassifier()
xgb_model.fit(X_train, y_train)

# Predict and evaluate
y_pred = xgb_model.predict(X_test)
print('XGBoost Classifier')
print(classification_report(y_test, y_pred))

# Save the model
joblib.dump(xgb_model, 'IMAR_ID_Forecast_Model_emre.pkl')

# Load data for predictions
Data = mesken_data.copy()

# Adjust the target labels to be zero-indexed
Data['IMAR_ID'] = Data['IMAR_ID'] - 1

X = Data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']]
Y = Data['IMAR_ID']

# Standardize the data using the same scaler used for training
X_scaled = scaler.transform(X)

# Load the model
XGBOOST_loaded = joblib.load('IMAR_ID_Forecast_Model_emre.pkl')

# Predict with the loaded model
y_pred = XGBOOST_loaded.predict(X_scaled)

# Add the predictions to the original DataFrame
# After making predictions
Data['IMAR_ID_FORECAST'] = y_pred

# Adjust for zero-indexing
Data['IMAR_ID_FORECAST'] = Data['IMAR_ID_FORECAST'] + 1

# Ensure the IMAR_ID_FORECAST column is included before saving to the database
mesken_data = Data[['ADR_BINA_ID', 'Y_KOORDINAT', 'X_KOORDINAT', 'ENERJI_TABLO_KAYIT_KODU', 
                    'IL_KODU', 'ILCE_ADI', 'MAHALLE', 'ORT_BAG_GUCU', '2023_Tuketim', 
                    'AYDINLATMA_count', 'MESKEN SAYISI', 'SANAYI_count', 
                    'TARIMSALSULAMA_count', 'TICARETHANE_count', 'URETICI_count', 
                    'AYDINLATMA_tuketim', 'MESKENTUKETIM', 'SANAYI_tuketim', 
                    'TARIMSALSULAMA_tuketim', 'TICARETHANE_tuketim', 
                    'URETICI_tuketim', 'MESKEN_SANAYI_TICARETHANE_SAYISI', 
                    'BINA_TIPI', 'ABONE SAYISI', 'IMAR_ID', 'ORT_Mesken_tuketim', 
                    'IMAR_ID_FORECAST']]

# Save mesken data to database
save_to_database(mesken_data, mesken_table, db_choice)

# Save the DataFrame with predictions to a CSV file
# mesken_results_path = '../Sonuçlar/mesken_data_GDZ.csv'
# Data.to_csv(mesken_results_path, index=False)