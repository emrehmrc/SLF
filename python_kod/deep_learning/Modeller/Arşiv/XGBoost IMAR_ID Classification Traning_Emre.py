import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from sklearn.preprocessing import StandardScaler
import joblib
import xgboost as xgb

# Load data
Data = pd.read_excel("241004_WORKING_NIZAM_v6.xlsx", header=0, index_col=False, sheet_name = 0)
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
