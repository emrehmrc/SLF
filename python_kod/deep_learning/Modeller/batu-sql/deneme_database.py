import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from sklearn.preprocessing import StandardScaler
import joblib
import xgboost as xgb

abone_verisi_path = '../Veriler/241004_Abone_Verileri_GDZ.csv'
imar_train_verisi_path = "../Input File/241004_WORKING_NIZAM_v6.xlsx"
combined_results_path = '../Sonuçlar/combined_results_GDZ.csv'
model_performance_path = '../Sonuçlar/model_performance.csv'

# Load the data
data = pd.read_csv(abone_verisi_path, encoding="cp1254")

# Constants
mesken_oranı = 0.7

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
    'BAGLANTI_GUCU': 'mean',
    '2023_Tuketim': 'sum'
}

# Create pivot tables for counts and consumption
abone_counts = pd.pivot_table(data, index='ADR_BINA_ID', columns='ABONE_GRUBU', 
                            values='TESISAT_NO', aggfunc='count', fill_value=0).add_suffix('_count')
abone_tuketim = pd.pivot_table(data, index='ADR_BINA_ID', columns='ABONE_GRUBU', 
                              values='2023_Tuketim', aggfunc='sum', fill_value=0).add_suffix('_tuketim')

# Create main pivot table
pivot_table = data.groupby('ADR_BINA_ID').agg(aggregations).reset_index()
pivot_table.rename(columns={'BAGLANTI_GUCU': 'ORT_BAG_GUCU'}, inplace=True)

# Merge all tables
pivot_table = pivot_table.merge(abone_counts, on='ADR_BINA_ID', how='left').merge(abone_tuketim, on='ADR_BINA_ID', how='left')

# Calculate total subscriber count
count_columns = ['MESKEN_count', 'SANAYI_count', 'TICARETHANE_count']
pivot_table['MESKEN_SANAYI_TICARETHANE_SAYISI'] = pivot_table[count_columns].sum(axis=1)

# Filter out buildings with more than 500 residential subscribers
pivot_table = pivot_table[pivot_table['MESKEN_count'] < 500]

# Define building type determination function
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

# Apply building type determination
pivot_table['BINA_TIPI'] = pivot_table.apply(determine_bina_tipi, axis=1)

# Split data into mesken and other
mesken_data = pivot_table[pivot_table['BINA_TIPI'] == 'MESKEN'].copy()
other_data = pivot_table[pivot_table['BINA_TIPI'] != 'MESKEN'].copy()

# Process mesken data
count_columns = ['MESKEN_count', 'SANAYI_count', 'TICARETHANE_count', 'TARIMSALSULAMA_count', 'AYDINLATMA_count', 'URETICI_count']
mesken_data['ABONE SAYISI'] = mesken_data[count_columns].sum(axis=1)
mesken_data['IMAR_ID'] = 1
mesken_data.rename(columns={'MESKEN_tuketim': 'MESKENTUKETIM', 'MESKEN_count': 'MESKEN SAYISI'}, inplace=True)
mesken_data['ORT_Mesken_tuketim'] = mesken_data['MESKENTUKETIM'] / mesken_data['MESKEN SAYISI']

# Machine Learning Section
print("Starting Machine Learning Process...")

# Train the model
Data = pd.read_excel(imar_train_verisi_path, header=0, index_col=False, sheet_name=0)
Data['IMAR_ID'] = Data['IMAR_ID'] - 1
X = Data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']]
Y = Data['IMAR_ID']

scaler = StandardScaler()
X_scaled = scaler.fit_transform(X)

X_train, X_test, y_train, y_test = train_test_split(X_scaled, Y, test_size=0.2, random_state=42)

xgb_model = xgb.XGBClassifier()
xgb_model.fit(X_train, y_train)

# Evaluate model performance
y_pred_test = xgb_model.predict(X_test)
test_report = classification_report(y_test, y_pred_test, output_dict=True)
test_report_df = pd.DataFrame(test_report).transpose()
print("\nModel Performance on Test Data:")
print(classification_report(y_test, y_pred_test))

# Save model performance report
test_report_df.to_csv(model_performance_path)
print(f"Model performance report saved to: {model_performance_path}")

# Make predictions on mesken_data
X_mesken = mesken_data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']]
X_mesken_scaled = scaler.transform(X_mesken)
mesken_predictions = xgb_model.predict(X_mesken_scaled)
mesken_data['IMAR_ID_FORECAST'] = mesken_predictions + 1

# Prepare other_data
other_data['IMAR_ID'] = 0
other_data['IMAR_ID_FORECAST'] = 0

# Create summary statistics
prediction_summary = pd.DataFrame(mesken_data['IMAR_ID_FORECAST'].value_counts()).reset_index()
prediction_summary.columns = ['IMAR_ID_FORECAST', 'Count']
prediction_summary = prediction_summary.sort_values('IMAR_ID_FORECAST')
print("\nPrediction Summary:")
print(prediction_summary)
prediction_summary.to_csv('../Sonuçlar/prediction_summary.csv', index=False)

# Ensure column consistency
all_columns = sorted(list(set(mesken_data.columns) | set(other_data.columns)))
for col in all_columns:
    if col not in mesken_data.columns:
        mesken_data[col] = 0
    if col not in other_data.columns:
        other_data[col] = 0

# Align columns and combine data
mesken_data = mesken_data[all_columns]
other_data = other_data[all_columns]
combined_data = pd.concat([mesken_data, other_data], axis=0, ignore_index=True)

# Save all results
combined_data.to_csv(combined_results_path, index=False)
print(f"\nCombined results saved to: {combined_results_path}")

mesken_data.to_csv('../Sonuçlar/mesken_results_deneme.csv', index=False)
other_data.to_csv('../Sonuçlar/other_results_deneme.csv', index=False)
print("Separate results saved for mesken and other data")

# Save the model
model_path = 'IMAR_ID_Forecast_Model_emre.pkl'
joblib.dump(xgb_model, model_path)
print(f"\nModel saved to: {model_path}")

# Print final statistics
print("\nFinal Statistics:")
print(f"Total number of records processed: {len(combined_data)}")
print(f"Number of residential buildings: {len(mesken_data)}")
print(f"Number of other buildings: {len(other_data)}")
print("\nProcessing complete!")