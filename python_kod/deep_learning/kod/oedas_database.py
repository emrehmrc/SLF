import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from sklearn.preprocessing import StandardScaler
import joblib
import xgboost as xgb
from sqlalchemy import create_engine
DB_CONNECTION = "postgresql://postgres:12345@localhost:5432/oedas"
engine = create_engine(DB_CONNECTION)
abone_verisi_path = '../Veriler/241004_Abone_Verileri_OEDAS.csv'
imar_train_verisi_path = "../Input File/241004_WORKING_NIZAM_v6.xlsx"


# Load the data (replace with your actual file path)
data = pd.read_csv(abone_verisi_path, encoding= "cp1254")

# MODEL PARAMETRELERİ
mesken_oranı = 0.7

buyuk_sanayi_baglantı_gucu = 150
buyuk_sanayi_tuketim = 300000
kucuk_sanayi_baglantı_gucu = 50
kucuk_sanayi_tuketim = 20000

buyuk_ticarethane_baglantı_gucu = 400
buyuk_ticarethane_tuketim = 200000
kucuk_ticarethane_baglantı_gucu = 15
kucuk_ticarethane_tuketim = 10000


################# DONT FORGET TO DELETE ROWS THAT HAVE CONS NUMBERS LESS THAN 5 kwh. ALSO DELETE NULLS.  DELETE ALL ROWS WITH MESKEN VALUE MORE THAN 500.

# Step 1: Rename the required columns to match the previous dataset
data.rename(columns={
    'Bağlı Olduğu Kofre/Bina ID': 'ADR_BINA_ID',
    'Bagli Oldugu Kofre Koordinat - Y': 'Y_KOORDINAT',
    'Bagli Oldugu Kofre Koordinat - X': 'X_KOORDINAT',
    'Trafo AssetNumber': 'ENERJI_TABLO_KAYIT_KODU',
    'Bağlantı Gücü': 'BAGLANTI_GUCU',
    'Abone Grubu Eşleme': 'ABONE_GRUBU',
    'Unique Code': 'TESISAT_NO',
    '2023 Yili Toplam Fatura (kWh)': '2023_Tuketim'
}, inplace=True)

# Step 1: Group the data by ADR_BINA_ID and calculate relevant aggregations including ORT_BAG_GUCU
aggregations = {
    'Y_KOORDINAT': 'first',
    'X_KOORDINAT': 'first',
    'ENERJI_TABLO_KAYIT_KODU': 'first',
    #'IL_KODU': 'first',
    #'ILCE_ADI': 'first',
    #'MAHALLE': 'first',
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

    elif row['MESKEN_count']/row['MESKEN_SANAYI_TICARETHANE_SAYISI'] >= mesken_oranı:
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

# Step 9: Export the new pivot table
#pivot_table.to_csv('final_pivot_table_with_additions_oedas.csv', index=False)

# Step 4: Divide the data into two datasets based on BINA_TIPI
mesken_data = pivot_table[pivot_table['BINA_TIPI'] == 'MESKEN'].copy()
other_data = pivot_table[pivot_table['BINA_TIPI'] != 'MESKEN'].copy()

# Step 5: Add the "ABONE SAYISI" column as the sum of all count columns
count_columns = ['MESKEN_count', 'SANAYI_count', 'TICARETHANE_count', 'TARIMSALSULAMA_count', 'AYDINLATMA_count']
mesken_data['ABONE SAYISI'] = mesken_data[count_columns].sum(axis=1)

# Step 6: Add a new column "IMAR_ID" and set all values to 1
mesken_data['IMAR_ID'] = 1

# Step 7: Rename "MESKEN_tuketim" to "MESKENTUKETIM"
mesken_data.rename(columns={'MESKEN_tuketim': 'MESKENTUKETIM'}, inplace=True)

# Step 8: Add a new column "ORT_Mesken_tuketim" which is MESKENTUKETIM divided by MESKEN_count
mesken_data['ORT_Mesken_tuketim'] = mesken_data['MESKENTUKETIM'] / mesken_data['MESKEN_count']

mesken_data.rename(columns={'MESKEN_count': 'MESKEN SAYISI'}, inplace=True)


# Step 10: Export the two new datasets
#mesken_data.to_csv('../Sonuçlar/mesken_data_oedas.csv', index=False)
# other_data.to_csv(other_results_path, index=False)



######### ----------------------------------------- MACHINE LEARNING --------------------------------------------- #######

# Load data
Data = pd.read_excel(imar_train_verisi_path, header=0, index_col=False, sheet_name = 0)
print('Data')
print(len(Data))

# Select relevant columns
#Data = Data[['ORT_Mesken_tuketim',  'ABONE SAYISI', 'MESKENTUKETIM', 'IMAR_ID', 'ORT_BAG_GUCU']]

# Adjust the target labels to be zero-indexed
Data['IMAR_ID'] = Data['IMAR_ID'] - 1

X = Data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']]

Y = Data['IMAR_ID']

# Standardize the data
scaler = StandardScaler()
X_scaled = scaler.fit_transform(X)

# Split data
X_train, X_test, y_train, y_test = train_test_split(X_scaled, Y, test_size=0.2, random_state=42)

# Initialize and train XGBoost model logloss mlogloss
xgb_model = xgb.XGBClassifier()
# xgb_model = xgb.XGBClassifier(use_label_encoder=False, eval_metric='logloss', max_depth=10, n_estimators=200)
xgb_model.fit(X_train, y_train)

# Predict and evaluate
y_pred = xgb_model.predict(X_test)
print('XGBoost Classifier')
print(classification_report(y_test, y_pred))

# Save the model
joblib.dump(xgb_model, 'IMAR_ID_Forecast_Model_emre.pkl')


# Load data
Data = mesken_data

# Select relevant columns
#Data = Data[['ORT_Mesken_tuketim', 'ABONE SAYISI', 'MESKENTUKETIM', 'IMAR_ID', 'ORT_BAG_GUCU']]

# Adjust the target labels to be zero-indexed
Data['IMAR_ID'] = Data['IMAR_ID'] - 1

#X = Data[['ORT_BAG_GUCU', 'ABONE SAYISI', 'YUKFAKTORU', 'X', 'Y', 'XY', 'MESKENTUKETIM', 'SANAYITUKETIM', 'TICARETHANETUKETIM', 'TARIMSALSULAMATUKETIM']]
X = Data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU',  'MESKEN SAYISI']]

Y = Data['IMAR_ID']

# Standardize the data using the same scaler used for training
scaler = StandardScaler()
X_scaled = scaler.fit_transform(X)

# Load the model XGBoost_Classifier NIZAM_XGBoost_Model
XGBOOST_loaded = joblib.load('IMAR_ID_Forecast_Model_emre.pkl')

# Predict with the loaded model
y_pred = XGBOOST_loaded.predict(X_scaled)
#print('XGBOOST FORECAST PERFORMANCE')
#print(classification_report(Y, y_pred, zero_division=0))

# Convert the classification report to a DataFrame
#report = classification_report(Y, y_pred, zero_division=0, output_dict=True)
#report_df = pd.DataFrame(report).transpose()

# Save the report to an Excel file
#report_path = 'EMRE_classification_report.xlsx'
#report_df.to_excel(report_path, sheet_name='XGBOOST_MODEL', index=True)

# Add the predictions to the original DataFrame
Data['IMAR_ID_FORECAST'] = y_pred

Data['IMAR_ID_FORECAST'] = Data['IMAR_ID_FORECAST'] + 1

# Save the DataFrame with predictions to an Excel file
#Data.to_excel('EMRE_IMAR_ID_FORECAST_oedas' + '.xlsx', index=False)

# Data.to_csv(mesken_results_path, index=False)
Data.to_sql('deep_learning_mesken', engine, if_exists='replace', index=False)
other_data.to_sql('deep_learning_other', engine, if_exists='replace', index=False)