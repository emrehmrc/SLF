import pandas as pd
from sklearn.metrics import classification_report
from sklearn.preprocessing import StandardScaler
import joblib
import xgboost as xgb

# File paths
#Home = 'C:/Users/burak.kucukaslan/OneDrive - MRC/Desktop/SLF/Data/'
#Name = 'IMARTEST'
#Adress = Home + Name + '.xlsx'

# Load data
Data = pd.read_csv("mesken_data_gdz.csv", header=0, index_col=False)

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
Data.to_excel('EMRE_IMAR_ID_FORECAST_gdz' + '.xlsx', index=False)
