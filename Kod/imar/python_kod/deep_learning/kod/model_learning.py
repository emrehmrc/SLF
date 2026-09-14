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
import time
from min_max_hesap.min_max_kirilimlari import min_max_kirilim

print("Abone verilerinden bina tipi oluşturan makine öğrenmesi modeli başlatılıyor.....")


# Abone grubu standardizasyonu için sözlük
ABONE_GRUBU_MAPPING = {

    # Mesken kategorileri
    "MESKEN": "MESKEN",
    "KENT MESKEN M.FAZE TEK TERIM": "MESKEN",
    "RESMÝ DAÝRELER MESKEN": "MESKEN",
    "RESMİ DAİRELER MESKEN": "MESKEN",
    "SEHIT AILELERI VE MUHARIP/MALUL GAZILER": "MESKEN",
    "BELEDİYE MESKEN": "MESKEN",
    "BELEDÝYE MESKEN": "MESKEN",

    # Ticarethane kategorileri
    "TIC.BÜRO": "TICARETHANE",
    "BELEDÝYE-TÝCARETHANE": "TICARETHANE",
    "BELEDİYE-TİCARETHANE": "TICARETHANE",
    "GEÇÝCÝ ABONELER": "TICARETHANE",
    "GEÇİCİ ABONELER": "TICARETHANE",
    "RESMÝ DAÝRE": "TICARETHANE",
    "RESMİ DAİRE": "TICARETHANE",
    "ÞANTÝYE VE GEÇÝCÝ ABONELER": "TICARETHANE",
    "ŞANTİYE VE GEÇİCİ ABONELER": "TICARETHANE",
    "HAYIR KURUMU BEDELLÝ": "TICARETHANE",
    "HAYIR KURUMU BEDELLİ": "TICARETHANE",
    "BELEDÝYELER ÝÇME VE KULLAN.SUY": "TICARETHANE",
    "BELEDİYELER İÇME VE KULLAN.SUY": "TICARETHANE",
    "RESMÝ DAÝRELER HAYIR KURUMLARI": "TICARETHANE",
    "RESMİ DAİRELER HAYIR KURUMLARI": "TICARETHANE",
    "BELEDÝYELER": "TICARETHANE",
    "BELEDİYELER": "TICARETHANE",
    "ÝÇ.VE KUL.SUY.M.FAZE T.TER": "TICARETHANE",
    "İÇ.VE KUL.SUY.M.FAZE T.TER": "TICARETHANE",
    "BELEDÝYE ÞANTÝYE VE GEÇÝCÝ ABN": "TICARETHANE",
    "BELEDİYE ŞANTİYE VE GEÇİCİ ABN": "TICARETHANE",
    
    # Sanayi kategorileri
    "SANAYÝ": "SANAYI",
    "SANAYİ": "SANAYI",
    
    # Tarımsal sulama
    "TARIMSAL SULAMA": "TARIMSAL_SULAMA",
    
    # Aydınlatma
    "GENEL AYDINLATMA": "AYDINLATMA",
    "SOKAK AYDINLATMA": "AYDINLATMA",
    "RESMI AYDINLATMA": "AYDINLATMA"
}

def standardize_abone_grubu(abone_grubu_value):

    """
    Abone grubu değerlerini standart formata dönüştürür.
    Sadece belirli bilinen kategorileri döndürür, eşleşme yoksa None değeri döndürür.
    """
    if pd.isna(abone_grubu_value):
        return None
        
    # Değeri string'e çevir, büyük harfe dönüştür ve boşlukları temizle
    normalized_value = str(abone_grubu_value).upper().strip()
    
    # Bilinen standart kategoriler
    standart_kategoriler = ["MESKEN", "TICARETHANE", "SANAYI", "TARIMSAL_SULAMA", "AYDINLATMA", "URETICI"]
    
    # Değer zaten standart bir kategori ise doğrudan döndür
    if normalized_value in standart_kategoriler:
        return normalized_value
    
    # Sözlükte arama yap
    if normalized_value in ABONE_GRUBU_MAPPING:
        return ABONE_GRUBU_MAPPING[normalized_value]
    
    # Tam eşleşme yoksa, kısmi eşleşme kontrolü
    for key, value in ABONE_GRUBU_MAPPING.items():
        if key in normalized_value or normalized_value in key:
            return value
    
    # Eşleşme bulunamazsa None döndür (bu değer pivot tabloda kullanılmayacak)
    return None

# Character encoding fix
sys.stdout = io.TextIOWrapper(
    sys.stdout.buffer,
    encoding='utf-8',
    errors='replace',
    line_buffering=True
)
sys.stderr = io.TextIOWrapper(
    sys.stderr.buffer,
    encoding='utf-8',
    errors='replace',
    line_buffering=True
)


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
        file_names = ["ML_TRAIN_DATA.xlsx"]
    
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
                return full_path


def standardize_output_columns(df):

    """Çıktı verilerindeki kolon adlarını standardize eder"""
    
    # MESKEN_count ve MESKEN SAYISI arasında uyumsuzluk varsa düzelt
    if 'MESKEN SAYISI' in df.columns and 'MESKEN_count' not in df.columns:
        df = df.rename(columns={'MESKEN SAYISI': 'MESKEN_count'})
    
    # MESKENTUKETIM kolonunu standardize et
    if 'MESKENTUKETIM' in df.columns and 'MESKEN_tuketim' not in df.columns:
        df = df.rename(columns={'MESKENTUKETIM': 'MESKEN_tuketim'})
    
    # ABONE SAYISI kolonunu standardize et
    if 'ABONE SAYISI' in df.columns:
        df = df.rename(columns={'ABONE SAYISI': 'ABONE_SAYISI'})
    
    # Tarımsal sulama kolonu standardizasyonu
    if 'TARIMSAL_SULAMA_count' in df.columns:
        df = df.rename(columns={'TARIMSAL_SULAMA_count': 'TARIMSAL_SULAMA_count'})
    
    if 'TARIMSAL_SULAMA_tuketim' in df.columns:
        df = df.rename(columns={'TARIMSAL_SULAMA_tuketim': 'TARIMSAL_SULAMA_tuketim'})
    
    return df


def main():
    
    # Get command line arguments
    if len(sys.argv) < 6:
        sys.exit(1)
    
    # Get arguments
    abone_verisi_path = sys.argv[1]
    mesken_results_path = sys.argv[2]
    other_results_path = sys.argv[3]
    sehir = sys.argv[4]
    ilce = sys.argv[5]
    year = sys.argv[6] if len(sys.argv) >= 7 else 2023
    year = int(year)-1

    time.sleep(3)
    print("Abone verileri içeri aktarılıyor....")
    
    # Check/create output directories
    output_dir = os.path.dirname(mesken_results_path)
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)
    

    # City and district value check using normalized string
    normalized_sehir = normalize_str(sehir)
    normalized_ilce = normalize_str(ilce)
    

    # Search for and find training file
    imar_train_verisi_path = find_training_file()
    
    # Define aggregations
    aggregations = {
        'ABONE_Y_KOORDINAT': 'first',
        'ABONE_X_KOORDINAT': 'first',
        'BAGLANDIGI_TRAFO_KODU': 'first',
        'ABONE_ILCE_ID': 'first',
        'BAGLANTI_GUCU': 'mean',  # Average BAGLANTI_GUCU (ORT_BAG_GUCU)
    }
    
    
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
        try:
            data = pd.read_csv(abone_verisi_path, encoding="cp1254", errors='replace')
        except:
            try:
                data = pd.read_csv(abone_verisi_path, encoding="utf-8")
            except:
                data = pd.read_csv(abone_verisi_path, encoding="latin1")
        
        # Sütun isimlerini temizle
        data.columns = data.columns.str.replace('ï»¿', '').str.strip()
        
        # ABONE_GRUBU sütununu kontrol et ve standartlaştır
        if 'ABONE_GRUBU' in data.columns:
            
            # Standartlaştırmayı uygula
            data['ABONE_GRUBU'] = data['ABONE_GRUBU'].apply(standardize_abone_grubu)
            data = data.dropna(subset=['ABONE_GRUBU'])
        else:
            sys.exit(1)
        
        # Tüketim sütunu tanımla
        tuketim_column = f"YIL_TUKETIM_{year}"
        
        data[tuketim_column] = pd.to_numeric(data[tuketim_column], errors="coerce")
        data['BAGLANTI_GUCU'] = pd.to_numeric(data['BAGLANTI_GUCU'], errors='coerce')


        # Pivot tabloları oluştur
        abone_counts = pd.pivot_table(data, index='BINA_ID', columns='ABONE_GRUBU', 
                                    values='TESISAT_NO', aggfunc='count', fill_value=0)
        
        # Kolon adlarını standardize et
        abone_counts.columns = [f"{col}_count" for col in abone_counts.columns]
        
        abone_tuketim = pd.pivot_table(data, index='BINA_ID', columns='ABONE_GRUBU', 
                                    values=tuketim_column, aggfunc='sum', fill_value=0)
        
        # Kolon adlarını standardize et
        abone_tuketim.columns = [f"{col}_tuketim" for col in abone_tuketim.columns]
        
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
        # Sadece var olan sütunları kullan
        count_columns = []
        if 'MESKEN_count' in pivot_table.columns:
            count_columns.append('MESKEN_count')
        if 'SANAYI_count' in pivot_table.columns:
            count_columns.append('SANAYI_count')
        if 'TICARETHANE_count' in pivot_table.columns:
            count_columns.append('TICARETHANE_count')
        
        if count_columns:
            pivot_table['MESKEN_SANAYI_TICARETHANE_SAYISI'] = pivot_table[count_columns].sum(axis=1)
        else:
            # Use first 3 available count columns as fallback
            fallback_columns = [col for col in pivot_table.columns if col.endswith('_count')][:3]
            if fallback_columns:
                pivot_table['MESKEN_SANAYI_TICARETHANE_SAYISI'] = pivot_table[fallback_columns].sum(axis=1)
            else:
                pivot_table['MESKEN_SANAYI_TICARETHANE_SAYISI'] = 0

        # Filter buildings with more than 500 residential units
        if 'MESKEN_count' in pivot_table.columns:
            pivot_table = pivot_table[pivot_table['MESKEN_count'] < 500]
        

        def determine_bina_tipi(row):

            aydinlatma_sutunlari = [col for col in row.index if ('AYDINLATMA' in col) and col.endswith('_count')]
            aydinlatma_var = any(row.get(col, 0) >= 1 for col in aydinlatma_sutunlari)
            
            # MESKEN_SANAYI_TICARETHANE_SAYISI kontrolü
            if row['MESKEN_SANAYI_TICARETHANE_SAYISI'] == 0:

                tarimsal_kolonlar = [col for col in row.index if 'TARIMSAL_SULAMA' in col and col.endswith('_count')]
                tarimsal_var = any(row.get(col, 0) >= 1 for col in tarimsal_kolonlar)
                
                if tarimsal_var:
                    return "TARIMSAL_SULAMA"
                elif aydinlatma_var:
                    return "AYDINLATMA"
                
            elif 'MESKEN_count' in row.index and row['MESKEN_count']/row['MESKEN_SANAYI_TICARETHANE_SAYISI'] >= mesken_oranı:
                    return "MESKEN"
            
            elif 'SANAYI_count' in row.index and row['SANAYI_count'] >= 1:

                sanayi_tuketim = row.get('SANAYI_tuketim', 0)
                if row['ORT_BAG_GUCU'] >= buyuk_sanayi_baglantı_gucu and sanayi_tuketim >= buyuk_sanayi_tuketim:
                    return "BUYUK_SANAYI"
                elif row['ORT_BAG_GUCU'] <= kucuk_sanayi_baglantı_gucu and sanayi_tuketim <= kucuk_sanayi_tuketim:
                    return "KUCUK_SANAYI"
                else:
                    return "ORTA_SANAYI"
                
            elif 'TICARETHANE_count' in row.index and row['TICARETHANE_count'] >= 1:  

                ticarethane_tuketim = row.get('TICARETHANE_tuketim', 0)
                if row['ORT_BAG_GUCU'] >= buyuk_ticarethane_baglantı_gucu and ticarethane_tuketim >= buyuk_ticarethane_tuketim:
                    return "BUYUK_TICARETHANE"
                elif row['ORT_BAG_GUCU'] <= kucuk_ticarethane_baglantı_gucu and ticarethane_tuketim <= kucuk_ticarethane_tuketim:
                    return "KUCUK_TICARETHANE"
                else:
                    return "ORTA_TICARETHANE"
                
            elif 'URETICI_count' in row.index and row['URETICI_count'] >= 1:
                    return "URETICI"        

            else:
                if row['AYDINLATMA_count'] >=1:
                    return "AYDINLATMA"
                elif row['TARIMSAL_SULAMA_count'] >=1:
                    return "TARIMSAL_SULAMA"
        
        # Create BINA_TIPI column
        pivot_table['BINA_TIPI'] = pivot_table.apply(determine_bina_tipi, axis=1)
        
        # Separate data by BINA_TIPI
        mesken_data = pivot_table[pivot_table['BINA_TIPI'] == 'MESKEN'].copy()
        other_data = pivot_table[pivot_table['BINA_TIPI'] != 'MESKEN'].copy()
        
        
        # Mevcut sütunları kontrol et
        available_columns = []
        if 'MESKEN_count' in mesken_data.columns:
            available_columns.append('MESKEN_count')
        if 'SANAYI_count' in mesken_data.columns:
            available_columns.append('SANAYI_count')
        if 'TICARETHANE_count' in mesken_data.columns:
            available_columns.append('TICARETHANE_count')
            
        additional_columns = []
        if 'TARIMSAL_SULAMA_count' in mesken_data.columns:
            additional_columns.append('TARIMSAL_SULAMA_count')
        if 'AYDINLATMA_count' in mesken_data.columns:
            additional_columns.append('AYDINLATMA_count')
            
        count_columns = available_columns + additional_columns
        
        if count_columns:
            mesken_data['ABONE SAYISI'] = mesken_data[count_columns].sum(axis=1)
        else:
            all_count_columns = [col for col in mesken_data.columns if col.endswith('_count')]
            if all_count_columns:
                mesken_data['ABONE SAYISI'] = mesken_data[all_count_columns].sum(axis=1)
            else:
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
            mesken_data['ORT_Mesken_tuketim'] = 0
        
        # Rename MESKEN_count to MESKEN SAYISI
        if 'MESKEN_count' in mesken_data.columns:
            mesken_data.rename(columns={'MESKEN_count': 'MESKEN SAYISI'}, inplace=True)
        
        
        # Check file existence again
        if not os.path.exists(imar_train_verisi_path):
            
            # Create default predictions
            mesken_data['IMAR_ID_FORECAST'] = 3  # Default zoning ID value
            
            # Add city and district information to the datasets
            mesken_data['SEHIR'] = sehir
            mesken_data['ILCE'] = ilce
            other_data['SEHIR'] = sehir
            other_data['ILCE'] = ilce
            
            # Standardize output columns
            mesken_data = standardize_output_columns(mesken_data)
            other_data = standardize_output_columns(other_data)
            
            mesken_data.to_csv(mesken_results_path, index=False, encoding="utf-8-sig")
            other_data.to_csv(other_results_path, index=False, encoding="utf-8-sig")
            
            # Buraya ekleyin:
            output_min_max_path = os.path.join(output_dir, "min_max_table.xlsx")

            try:
                print(f"min_max tablosu oluşturuluyor: {output_min_max_path}")
                min_max_kirilim(mesken_results_path, other_results_path, output_min_max_path)
               
                print("min_max tablosu oluşturma tamamlandı")

            except Exception as e:

                print(f"min_max tablosu oluşturma hatası: {e}")
                traceback.print_exc()

            return  # Exit function
        
        # If file exists, continue and read
        try:
            train_data = pd.read_excel(imar_train_verisi_path, header=0, index_col=False, sheet_name=0)
        except Exception as e:
            print(f"Dosya okuma hatası: {e}")
            
            # Create default predictions
            mesken_data['IMAR_ID_FORECAST'] = 3  # Default value
            
            # Add city and district information to the datasets
            mesken_data['SEHIR'] = sehir
            mesken_data['ILCE'] = ilce
            other_data['SEHIR'] = sehir
            other_data['ILCE'] = ilce
            
            # Standardize output columns
            mesken_data = standardize_output_columns(mesken_data)
            other_data = standardize_output_columns(other_data)
            
            mesken_data.to_csv(mesken_results_path, index=False, encoding="utf-8-sig")
            other_data.to_csv(other_results_path, index=False, encoding="utf-8-sig")
            
            return  # Exit function
        
        # Check required columns in training data
        required_train_columns = ['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI', 'IMAR_ID']
        missing_columns = []
        for col in required_train_columns:
            if col not in train_data.columns:
                missing_columns.append(col)
        
        if missing_columns:
            # Use default predictions
            mesken_data['IMAR_ID_FORECAST'] = 3  # Default value
            
            # Add city and district information to the datasets
            mesken_data['SEHIR'] = sehir
            mesken_data['ILCE'] = ilce
            other_data['SEHIR'] = sehir
            other_data['ILCE'] = ilce
            
            # Standardize output columns
            mesken_data = standardize_output_columns(mesken_data)
            other_data = standardize_output_columns(other_data)
            
            output_min_max_path = os.path.join(output_dir, "min_max_table.xlsx")
            mesken_data.to_csv(mesken_results_path, index=False, encoding="utf-8-sig")
            other_data.to_csv(other_results_path, index=False, encoding="utf-8-sig")
            min_max_kirilim(mesken_results_path,other_results_path,output_min_max_path)
            
            return  # Exit function
        
        # Adjust target variable (make 0-indexed)
        train_data['IMAR_ID'] = train_data['IMAR_ID'] - 1


        print("Makine öğrenmesi modeli oluşturuluyor.....")

        time.sleep(4)
        
        # Split features and target variables
        X = train_data[['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']]
        Y = train_data['IMAR_ID']
        
        print("Veri standartlaştırılıyor...")
        time.sleep(1)
        # Standardize data
        scaler = StandardScaler()
        X_scaled = scaler.fit_transform(X)
        
        # Split training and test data
        print("Train ve test datasetleri oluşturuluyor...")
        time.sleep(1)
        X_train, X_test, y_train, y_test = train_test_split(X_scaled, Y, test_size=0.2, random_state=42)
        
        # Initialize and train XGBoost model
        print("XGBoost modeli eğitiliyor...")
        time.sleep(5)
        xgb_model = xgb.XGBClassifier()
        xgb_model.fit(X_train, y_train)
        
        y_pred = xgb_model.predict(X_test)
        report = classification_report(y_test, y_pred)
        # Save model (in the specified output directory)

        print("En iyi model kaydediliyor...")
        model_output_path = os.path.join(output_dir, f'IMAR_ID_Forecast_Model_{normalized_sehir}_{normalized_ilce}.pkl')
        joblib.dump(xgb_model, model_output_path)
        
        # Check required columns
        required_pred_columns = ['ORT_Mesken_tuketim', 'ORT_BAG_GUCU', 'MESKEN SAYISI']
        missing_pred_columns = [col for col in required_pred_columns if col not in mesken_data.columns]
        
        if missing_pred_columns:
            # Fill missing columns
            for col in missing_pred_columns:
                mesken_data[col] = 0
        
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

        
        print("\nMakine öğrenmesi başarıyla tamamlandı..!\n\n Mesken binaları '1-2 Katlı', '3-4 Katlı', '5-7 Katlı', '8 Üstü' ve 'Villa' olmak üzere" \
        ", Sanayi ve Ticarethane binaları ise 'küçük', 'orta' ve 'büyük' olmak üzere parçalara ayrıldı.\n Sonuç Excel dosyaları oluşturuluyor...")
        time.sleep(3)
        
        # Standardize output columns
        mesken_data = standardize_output_columns(mesken_data)
        other_data = standardize_output_columns(other_data)
        
        output_min_max_path = os.path.join(output_dir, "min_max_table.xlsx")
        
        mesken_data.to_csv(mesken_results_path, index=False, encoding="utf-8-sig")
        other_data.to_csv(other_results_path, index=False, encoding="utf-8-sig")
        min_max_kirilim(mesken_results_path,other_results_path,output_min_max_path)
        

        print(f"Sonuç dosyaları başarıyla oluşturuldu!!: {mesken_results_path}, {other_results_path}")
        time.sleep(2)
        
        
    except Exception as e:
        print(f"Bir hata oluştu:\n {e}")
        print("\nHata detayları:\n")
        traceback.print_exc()
        sys.exit(1)

if __name__ == "__main__":
    main()