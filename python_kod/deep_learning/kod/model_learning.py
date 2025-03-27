#!/usr/bin/env python
# -*- coding: utf-8 -*-

"""
Deep Learning Model for SLF Project
This script processes subscriber data, predicts zoning patterns, and outputs analysis results.
Usage: python model_learning.py abone_file_path mesken_output_path other_output_path city_name district_name last_year
"""

import os
import sys
import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from sklearn.preprocessing import StandardScaler
import joblib
import xgboost as xgb
import traceback
import io
import re
from datetime import datetime

# Character encoding fix
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8', errors='replace')

def normalize_str(s):
    """Fixes Turkish characters and Unicode issues"""
    # Special normalization: İ -> i conversion
    s = s.replace('İ', 'i').replace('ı', 'i')
    # Other Turkish character conversions
    s = s.replace('ğ', 'g').replace('Ğ', 'g')
    s = s.replace('ü', 'u').replace('Ü', 'u')
    s = s.replace('ş', 's').replace('Ş', 's')
    s = s.replace('ö', 'o').replace('Ö', 'o')
    s = s.replace('ç', 'c').replace('Ç', 'c')
    return s.lower()

def find_training_file(base_dir=None, file_names=None):
    """Tries to find the training file in various directories"""
    if base_dir is None:
        # Use parent directory of the current script
        base_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    
    if file_names is None:
        file_names = ["ML_TRAIN_DATA.xlsx", "241004_WORKING_NIZAM_v6.xlsx"]
    
    # Check all possible directories
    possible_dirs = [
        os.path.join(base_dir, "ml_train"),
        os.path.join(base_dir, "deep_learning", "ml_train"),
        os.path.join(base_dir, "kod", "ml_train"),
        os.path.join(base_dir, "data"),
        base_dir
    ]
    
    # Search for each file in each directory
    for dir_path in possible_dirs:
        for file_name in file_names:
            full_path = os.path.join(dir_path, file_name)
            if os.path.exists(full_path):
                print(f"Training file found: {full_path}")
                return full_path
    
    # If file not found
    print("WARNING: Training file not found! Returning a default path.")
    # Find path that could be the project root
    possible_root = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
    return os.path.join(possible_root, "ml_train", file_names[0])  # Return a default path

def main():
    # Get command line arguments
    if len(sys.argv) < 6:
        print("Incorrect number of arguments!")
        print("Usage: python model_learning.py subscriber_data_path residential_results_path other_results_path city district [lastYear]")
        sys.exit(1)
    
    # Get arguments
    abone_verisi_path = sys.argv[1]
    mesken_results_path = sys.argv[2]
    other_results_path = sys.argv[3]
    sehir = sys.argv[4]
    ilce = sys.argv[5]
    lastYear = sys.argv[6] if len(sys.argv) >= 7 else "2023"  # Default value 2023
    print(f"Process started: deep learning model for {sehir}/{ilce}")
    
    # Check/create output directories
    output_dir = os.path.dirname(mesken_results_path)
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)
        print(f"Output directory created: {output_dir}")
    
    print(f"Subscriber data: {abone_verisi_path}")
    
    # City and district value check using normalized string
    print(f"City parameter: '{sehir}'")
    print(f"District parameter: '{ilce}'")
    normalized_sehir = normalize_str(sehir)
    normalized_ilce = normalize_str(ilce)
    print(f"Normalized city: '{normalized_sehir}'")
    print(f"Normalized district: '{normalized_ilce}'")
    
    # Select training dataset based on city
    if "izmir" in normalized_sehir or "gdz" in normalized_sehir or "aliaga" in normalized_sehir:
        default_file = "ML_TRAIN_DATA.xlsx"
        print(f"Searching for training data for GDZ/İzmir...")
    else:
        default_file = "241004_WORKING_NIZAM_v6.xlsx"
        print(f"Searching for training data for OEDAŞ/Eskişehir...")
    
    # Search for and find training file
    script_dir = os.path.dirname(os.path.abspath(__file__))
    ml_train_dir = os.path.join(os.path.dirname(script_dir), "ml_train")
    imar_train_verisi_path = find_training_file(
        base_dir=os.path.dirname(script_dir),
        file_names=[default_file, "ML_TRAIN_DATA.xlsx", "241004_WORKING_NIZAM_v6.xlsx"]
    )
    
    # Define aggregations
    aggregations = {
        'ABONE_Y_KOORDINAT': 'first',
        'ABONE_X_KOORDINAT': 'first',
        'BAGLANDIGI_TRAFO_KODU': 'first',
        'ABONE_ILCE_ID': 'first',
        'BAGLANTI_GUCU': 'mean',  # Average BAGLANTI_GUCU (ORT_BAG_GUCU)
    }
    
    print(f"Zoning training data: {imar_train_verisi_path}")
    
    # Check if training file exists
    if not os.path.exists(imar_train_verisi_path):
        print(f"ERROR: Training file not found! {imar_train_verisi_path}")
        # Alternative file search methods...
        possible_files = []
        for root, dirs, files in os.walk(os.path.dirname(script_dir)):
            for file in files:
                if file.endswith(".xlsx"):
                    possible_files.append(os.path.join(root, file))
        
        if possible_files:
            print("Alternative training files found:")
            for i, file_path in enumerate(possible_files[:5]):  # Show first 5 files
                print(f"{i+1}. {file_path}")
            
            # Use the first file
            imar_train_verisi_path = possible_files[0]
            print(f"Using first alternative file: {imar_train_verisi_path}")
        else:
            print("No Excel files found.")
            sys.exit(1)
    
    print(f"Residential result path: {mesken_results_path}")
    print(f"Other result path: {other_results_path}")
    
    # City-specific parameters
    mesken_oranı = 0.7
        
    buyuk_sanayi_baglantı_gucu = 150
    buyuk_sanayi_tuketim = 300000
    kucuk_sanayi_baglantı_gucu = 50
    kucuk_sanayi_tuketim = 20000
        
    buyuk_ticarethane_baglantı_gucu = 400
    buyuk_ticarethane_tuketim = 200000
    kucuk_ticarethane_baglantı_gucu = 15
    kucuk_ticarethane_tuketim = 10000
 
    try:
        # Load data
        print(f"Loading subscriber data: {abone_verisi_path}")
        try:
            data = pd.read_csv(abone_verisi_path, encoding="cp1254", errors='replace')
            print("File read with CP1254.")
        except:
            try:
                data = pd.read_csv(abone_verisi_path, encoding="utf-8")
                print("File read with UTF-8.")
            except:
                data = pd.read_csv(abone_verisi_path, encoding="latin1")
                print("File read with Latin1.")
        
        print(f"Subscriber dataset loaded: {data.shape[0]} rows, {data.shape[1]} columns")
        
        # Define consumption column
        tuketim_column = f"YIL_TUKETIM_{lastYear}"
        
        # Clean column names from BOM characters
        data.columns = data.columns.str.replace('ï»¿', '')
        
        # Print column names
        print("Column names:", data.columns.tolist())
        
        # Create pivot tables based on subscriber groups
        print("Creating pivot tables...")
        abone_counts = pd.pivot_table(data, index='BINA_ID', columns='ABONE_GRUBU', 
                                      values='TESISAT_NO', aggfunc='count', fill_value=0).add_suffix('_count')
        abone_tuketim = pd.pivot_table(data, index='BINA_ID', columns='ABONE_GRUBU', 
                                     values=tuketim_column, aggfunc='sum', fill_value=0).add_suffix('_tuketim')
        
        # Fix: Create main pivot table without including BINA_ID in the aggregations
        grouped_data = data.groupby('BINA_ID').agg(aggregations)
        
        # Now reset_index to add BINA_ID as a column (this avoids duplicate BINA_ID)
        pivot_table = grouped_data.reset_index()
        
        # Rename 'BAGLANTI_GUCU' to 'ORT_BAG_GUCU'
        pivot_table = pivot_table.rename(columns={'BAGLANTI_GUCU': 'ORT_BAG_GUCU'})
        
        # Merge counts and consumption values to main table
        pivot_table = pivot_table.merge(abone_counts, on='BINA_ID', how='left')
        pivot_table = pivot_table.merge(abone_tuketim, on='BINA_ID', how='left')
        
        # Add MESKEN_SANAYI_TICARETHANE_SAYISI column
        count_columns = [col for col in pivot_table.columns if col.endswith('_count')]
        count_columns_filtered = ['MESKEN_count', 'SANAYI_count', 'TICARETHANE_count']
        count_columns_filtered = [col for col in count_columns_filtered if col in pivot_table.columns]
        
        if not count_columns_filtered:
            print("WARNING: MESKEN_count, SANAYI_count, TICARETHANE_count columns not found.")
            print("Existing _count columns:", [col for col in pivot_table.columns if col.endswith('_count')])
            # Use existing count columns
            count_columns_filtered = count_columns[:3] if len(count_columns) >= 3 else count_columns
        
        pivot_table['MESKEN_SANAYI_TICARETHANE_SAYISI'] = pivot_table[count_columns_filtered].sum(axis=1)
        
        print(f"Filtering buildings with more than 500 residential units...")
        # Filter buildings with more than 500 residential units
        if 'MESKEN_count' in pivot_table.columns:
            pivot_table = pivot_table[pivot_table['MESKEN_count'] < 500]
        
        # Add BINA_TIPI column
        print("Determining building types...")
        def determine_bina_tipi(row):
            if 'MESKEN_SANAYI_TICARETHANE_SAYISI' not in row or row['MESKEN_SANAYI_TICARETHANE_SAYISI'] == 0:
                if row.get('TARIMSALSULAMA_count', 0) >= 1:
                    return "TARIMSAL_SULAMA"
                elif row.get('AYDINLATMA_count', 0) >= 1:
                    return "AYDINLATMA"
                else:
                    return "DIGER"
            elif 'MESKEN_count' in row and row['MESKEN_count']/row['MESKEN_SANAYI_TICARETHANE_SAYISI'] >= mesken_oranı:
                return "MESKEN"
            elif row.get('SANAYI_count', 0) >= 1:
                if row['ORT_BAG_GUCU'] >= buyuk_sanayi_baglantı_gucu and row.get('SANAYI_tuketim', 0) >= buyuk_sanayi_tuketim:
                    return "BUYUK_SANAYI"
                elif row['ORT_BAG_GUCU'] <= kucuk_sanayi_baglantı_gucu and row.get('SANAYI_tuketim', 0) <= kucuk_sanayi_tuketim:
                    return "KUCUK_SANAYI"
                else:
                    return "ORTA_SANAYI"
            elif row.get('TICARETHANE_count', 0) >= 1:  
                if row['ORT_BAG_GUCU'] >= buyuk_ticarethane_baglantı_gucu and row.get('TICARETHANE_tuketim', 0) >= buyuk_ticarethane_tuketim:
                    return "BUYUK_TICARETHANE"
                elif row['ORT_BAG_GUCU'] <= kucuk_ticarethane_baglantı_gucu and row.get('TICARETHANE_tuketim', 0) <= kucuk_ticarethane_tuketim:
                    return "KUCUK_TICARETHANE"
                else:
                    return "ORTA_TICARETHANE"
            else:
                return "DIGER"
        
        # Create BINA_TIPI column
        pivot_table['BINA_TIPI'] = pivot_table.apply(determine_bina_tipi, axis=1)
        
        # Separate data by BINA_TIPI
        mesken_data = pivot_table[pivot_table['BINA_TIPI'] == 'MESKEN'].copy()
        other_data = pivot_table[pivot_table['BINA_TIPI'] != 'MESKEN'].copy()
        
        print(f"MESKEN building count: {len(mesken_data)}")
        print(f"Other building count: {len(other_data)}")
        
        # Add subscriber count column
        available_columns = [col for col in ['MESKEN_count', 'SANAYI_count', 'TICARETHANE_count'] if col in mesken_data.columns]
        additional_columns = [col for col in ['TARIMSALSULAMA_count', 'AYDINLATMA_count', 'URETICI_count'] if col in mesken_data.columns]
        count_columns = available_columns + additional_columns
        if count_columns:
            mesken_data['ABONE SAYISI'] = mesken_data[count_columns].sum(axis=1)
        else:
            print("WARNING: Subscriber count could not be calculated, count columns not found.")
            mesken_data['ABONE SAYISI'] = 0
        
        # Add IMAR_ID column
        mesken_data['IMAR_ID'] = 1
        
        # Rename MESKEN_tuketim to MESKENTUKETIM
        if 'MESKEN_tuketim' in mesken_data.columns:
            mesken_data.rename(columns={'MESKEN_tuketim': 'MESKENTUKETIM'}, inplace=True)
        
        # Add ORT_Mesken_tuketim column
        if 'MESKENTUKETIM' in mesken_data.columns and 'MESKEN_count' in mesken_data.columns:
            mesken_data['ORT_Mesken_tuketim'] = mesken_data['MESKENTUKETIM'] / mesken_data['MESKEN_count']
        elif 'MESKENTUKETIM' not in mesken_data.columns:
            print("WARNING: MESKENTUKETIM column not found, ORT_Mesken_tuketim could not be calculated")
            mesken_data['ORT_Mesken_tuketim'] = 0
        
        # Rename MESKEN_count to MESKEN SAYISI
        if 'MESKEN_count' in mesken_data.columns:
            mesken_data.rename(columns={'MESKEN_count': 'MESKEN SAYISI'}, inplace=True)
        
        # Machine Learning part
        print("Starting machine learning model...")
        
        # Load training data
        print(f"Loading training data: {imar_train_verisi_path}")
        
        # Check file existence again
        if not os.path.exists(imar_train_verisi_path):
            print(f"ERROR: Training file not found: {imar_train_verisi_path}")
            print("Continuing without training dataset, using default predictions...")
            
            # Create default predictions
            mesken_data['IMAR_ID_FORECAST'] = 3  # Default zoning ID value
            
            # Add city and district information to the datasets
            mesken_data['SEHIR'] = sehir
            mesken_data['ILCE'] = ilce
            other_data['SEHIR'] = sehir
            other_data['ILCE'] = ilce
            
            # Save final results (ONLY TO SPECIFIED PATHS - no temporary files)
            print("Saving results...")
            mesken_data.to_csv(mesken_results_path, index=False, encoding="utf-8-sig")
            other_data.to_csv(other_results_path, index=False, encoding="utf-8-sig")
            print(f"Results successfully saved (without training): {mesken_results_path}, {other_results_path}")
            
            print(f"\nTotal residential building count: {len(mesken_data)}")
            print(f"Total other building count: {len(other_data)}")
            
            print("Process complete (untrained mode).")
            return  # Exit function
        
        # If file exists, continue and read
        try:
            train_data = pd.read_excel(imar_train_verisi_path, header=0, index_col=False, sheet_name=0)
            print(f"Training dataset loaded: {len(train_data)} rows")
        except Exception as e:
            print(f"Excel file reading error: {e}")
            print("Continuing without training dataset, using default predictions...")
            
            # Create default predictions
            mesken_data['IMAR_ID_FORECAST'] = 3  # Default value
            
            # Add city and district information to the datasets
            mesken_data['SEHIR'] = sehir
            mesken_data['ILCE'] = ilce
            other_data['SEHIR'] = sehir
            other_data['ILCE'] = ilce
            
            # Save final results (ONLY TO SPECIFIED PATHS)
            print("Saving results...")
            mesken_data.to_csv(mesken_results_path, index=False, encoding="utf-8-sig")
            other_data.to_csv(other_results_path, index=False, encoding="utf-8-sig")
            print(f"Results successfully saved (without training): {mesken_results_path}, {other_results_path}")
            
            print(f"\nTotal residential building count: {len(mesken_data)}")
            print(f"Total other building count: {len(other_data)}")
            
            print("Process complete (untrained mode).")
            return  # Exit function
        
        # Check required columns in training data
        required_train_columns = ['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI', 'IMAR_ID']
        missing_columns = []
        for col in required_train_columns:
            if col not in train_data.columns:
                missing_columns.append(col)
                print(f"WARNING: Required column '{col}' not found in training data!")
        
        if missing_columns:
            print(f"Training not possible due to missing columns: {missing_columns}")
            # Use default predictions
            mesken_data['IMAR_ID_FORECAST'] = 3  # Default value
            
            # Add city and district information to the datasets
            mesken_data['SEHIR'] = sehir
            mesken_data['ILCE'] = ilce
            other_data['SEHIR'] = sehir
            other_data['ILCE'] = ilce
            
            # Save final results (ONLY TO SPECIFIED PATHS)
            print("Saving results (untrained)...")
            mesken_data.to_csv(mesken_results_path, index=False, encoding="utf-8-sig")
            other_data.to_csv(other_results_path, index=False, encoding="utf-8-sig")
            print(f"Results successfully saved: {mesken_results_path}, {other_results_path}")
            
            print(f"\nTotal residential building count: {len(mesken_data)}")
            print(f"Total other building count: {len(other_data)}")
            
            print("Process complete (untrained mode).")
            return  # Exit function
        
        # Adjust target variable (make 0-indexed)
        train_data['IMAR_ID'] = train_data['IMAR_ID'] - 1
        
        # Split features and target variables
        X = train_data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']]
        Y = train_data['IMAR_ID']
        
        print("Standardizing data...")
        # Standardize data
        scaler = StandardScaler()
        X_scaled = scaler.fit_transform(X)
        
        # Split training and test data
        print("Splitting training/test datasets...")
        X_train, X_test, y_train, y_test = train_test_split(X_scaled, Y, test_size=0.2, random_state=42)
        
        # Initialize and train XGBoost model
        print("Training XGBoost model...")
        xgb_model = xgb.XGBClassifier()
        xgb_model.fit(X_train, y_train)
        
        # Evaluate model performance
        print("Evaluating model performance...")
        y_pred = xgb_model.predict(X_test)
        report = classification_report(y_test, y_pred)
        print('XGBoost Classifier Performance:')
        print(report)
        
        # Save model (in the specified output directory)
        print("Saving model...")
        model_output_path = os.path.join(output_dir, f'IMAR_ID_Forecast_Model_{normalized_sehir}_{normalized_ilce}.pkl')
        joblib.dump(xgb_model, model_output_path)
        print(f"Model saved: {model_output_path}")
        
        # Add predictions to residential data
        print("Making predictions for residential data...")
        # Check required columns
        required_pred_columns = ['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']
        missing_pred_columns = [col for col in required_pred_columns if col not in mesken_data.columns]
        
        if missing_pred_columns:
            print(f"WARNING: Columns required for prediction are missing: {missing_pred_columns}")
            # Fill missing columns
            for col in missing_pred_columns:
                mesken_data[col] = 0
                print(f"Column '{col}' filled with 0 values")
        
        X_mesken = mesken_data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']]
        X_mesken_scaled = scaler.transform(X_mesken)
        mesken_data['IMAR_ID'] = mesken_data['IMAR_ID'] - 1
        y_pred_mesken = xgb_model.predict(X_mesken_scaled)
        mesken_data['IMAR_ID_FORECAST'] = y_pred_mesken
        mesken_data['IMAR_ID_FORECAST'] = mesken_data['IMAR_ID_FORECAST'] + 1
        mesken_data['IMAR_ID'] = mesken_data['IMAR_ID'] + 1
        
        # Add city and district information to the datasets
        mesken_data['SEHIR'] = sehir
        mesken_data['ILCE'] = ilce
        other_data['SEHIR'] = sehir
        other_data['ILCE'] = ilce
        
        # Save final results (ONLY TO SPECIFIED PATHS)
        print("Saving results...")
        mesken_data.to_csv(mesken_results_path, index=False, encoding="utf-8-sig")
        other_data.to_csv(other_results_path, index=False, encoding="utf-8-sig")
        print(f"Results successfully saved: {mesken_results_path}, {other_results_path}")
        
        # Create summary information
        building_types = pivot_table['BINA_TIPI'].value_counts()
        print("\nBuilding Type Distribution:")
        for btype, count in building_types.items():
            print(f"{btype}: {count}")
        
        print("\nZoning ID Prediction Distribution:")
        imar_forecasts = mesken_data['IMAR_ID_FORECAST'].value_counts()
        for imar_id, count in imar_forecasts.items():
            print(f"Zoning ID {int(imar_id)}: {count}")
        
        print(f"\nTotal residential building count: {len(mesken_data)}")
        print(f"Total other building count: {len(other_data)}")
        
    except Exception as e:
        print(f"Error occurred during processing: {e}")
        print("Error details:")
        traceback.print_exc()
        sys.exit(1)
    
    print("Process successfully completed.")

if __name__ == "__main__":
    main()