import pandas as pd
import numpy as np
import os
import sys
import json
from scipy.spatial import KDTree
import sqlite3
import random
import shutil
import math
from collections import defaultdict
import warnings
import time
warnings.filterwarnings("ignore")
import io
from contextlib import redirect_stdout, redirect_stderr


# Custom stream to capture output
class DualOutput:
    def __init__(self):
        self.output = io.StringIO()
        self.terminal = sys.stdout

    def write(self, message):
        self.output.write(message)
        self.terminal.write(message)

    def flush(self):
        self.output.flush()
        self.terminal.flush()

if __name__ == "__main__":
    # Initialize status dictionary
    status = {
        "ExitCode": 1,  # Default to failure
        "Output": "",
        "Error": ""
    }
    
    # Temporary JSON file path from command-line argument
    if len(sys.argv) < 3:
        print("Error: Temporary status file path not provided.")
        sys.exit(1)
    
    temp_status_file = sys.argv[2]
    
    # Redirect stdout and stderr
    stdout_capture = DualOutput()
    stderr_capture = io.StringIO()
    
    try:
        with redirect_stdout(stdout_capture), redirect_stderr(stderr_capture):
    
            # Get the config path from the first command-line argument
            config_path = sys.argv[1]
            user_home = os.path.expanduser('~')
    
            # Load the config.json file
            with open(config_path, 'r', encoding='utf-8') as f:
                config = json.load(f)
    
            # Extract the necessary paths from config.json
            ana_klasor_yolu = config['Ana_Klasör_Yolu']
            il = config['İl']
            ilce = config['İlçe']
            proje_ismi = config['proje_ismi']
            start_year = config['baslangıc_yılı']
            end_year = config['bitis_yılı']
            program_dosyaları_path = config['program_dosyaları_path']
            ELF_sonuc_klasor = config['ELF']['SONUÇLAR_klasör']
            ELF_sonuc_name = config['ELF']['SONUÇLAR_name']
            ELF_sonuc_senaryo = config['SLF']['secilen_senaryo']
    
    
            # Define the function to get nearest cells
            def get_nearest_cells(builtup_df, target_cell, k=25):
    
                """
                En yakın hücreleri bulmak için KDTree kullanılır.
                """
                coords = builtup_df[['lat', 'lon']].values  # Hücre merkezlerinin koordinatları
                tree = KDTree(coords)  # KDTree oluştur
                distances, indices = tree.query(target_cell, k=k)  # En yakın k hücreyi bul
                return indices
    
    
            def calculate_new_buildings(builtup_df, zoning_ratios, construction_areas, year, as_is_buildings):
                results = []
                # Define valid zoning types
                valid_zoning_types = [
                    '1-2 KATLI MESKEN', '3-4 KATLI MESKEN', '5-7 KATLI MESKEN', '8 USTU KATLI MESKEN',
                    'AYDINLATMA', 'BUYUK_SANAYI', 'BUYUK_TICARETHANE', 'KUCUK_SANAYI', 'KUCUK_TICARETHANE',
                    'ORTA_SANAYI', 'ORTA_TICARETHANE', 'TARIMSAL_SULAMA', 'VILLA MESKEN'
                ]
                
                for _, cell in builtup_df.iterrows():
                    cell_id = cell['id']
                    cell_area = cell['cell_area']
                    saturation = cell[f'Saturation_updated_{year}']
                    target_cell = (cell['lat'], cell['lon'])
            
                    new_buildings = {'id': cell_id, 'left': cell['left'], 'top': cell['top'],
                                    'right': cell['right'], 'bottom': cell['bottom']}
                    
                    # Sadece saturasyon artışı olan hücrelerde işlem yap
                    if saturation > cell[f'Saturation_updated_{year-1}']:
                        # En yüksek imar oranını bulma
                        max_zoning_type = None
                        max_ratio = 0
                
                        # Only iterate over valid zoning types
                        for zoning_type in valid_zoning_types:
                            if zoning_type in zoning_ratios.columns:
                                ratio = float(zoning_ratios.loc[zoning_ratios['id'] == cell_id, zoning_type].iloc[0])
                                if ratio > max_ratio:
                                    max_ratio = ratio
                                    max_zoning_type = zoning_type
                
                        if max_zoning_type and max_ratio > 0.9:
                            required_area = construction_areas.get(max_zoning_type, None)
                            as_is_bina_sayisi = as_is_buildings.loc[as_is_buildings['id'] == cell_id, max_zoning_type].values[0]
                            average_area = construction_areas.get(max_zoning_type, 1)
                            as_is_area = as_is_bina_sayisi * average_area
                
                            available_area = max(0, cell_area * saturation * max_ratio - as_is_area)
                
                            if required_area and available_area < required_area:
                                nearest_indices = get_nearest_cells(builtup_df, target_cell)
                                total_neighbor_area = sum(
                                    max(0, builtup_df.iloc[idx]['cell_area'] * builtup_df.iloc[idx][f'Saturation_updated_{year}'] * max_ratio -
                                        as_is_buildings.iloc[idx][max_zoning_type] * construction_areas.get(max_zoning_type, 1))
                                    for idx in nearest_indices
                                )
                                if total_neighbor_area >= required_area:
                                    new_buildings[max_zoning_type] = 1
                                else:
                                    new_buildings[max_zoning_type] = 0
                            else:
                                if required_area and required_area > 0:
                                    new_buildings[max_zoning_type] = int(np.floor(available_area / required_area))
                                else:
                                    new_buildings[max_zoning_type] = 0
                        else:
                            # Standart hesaplama for valid zoning types
                            for zoning_type in valid_zoning_types:
                                if zoning_type in zoning_ratios.columns:
                                    ratio = float(zoning_ratios.loc[zoning_ratios['id'] == cell_id, zoning_type].iloc[0])
                                    as_is_bina_sayisi = as_is_buildings.loc[as_is_buildings['id'] == cell_id, zoning_type].values[0]
                                    average_area = construction_areas.get(zoning_type, 1)
                                    as_is_area = as_is_bina_sayisi * average_area
                                    available_area = max(0, cell_area * saturation * ratio - as_is_area)
                
                                    if average_area and average_area > 0:
                                        new_buildings[zoning_type] = int(np.floor(available_area / average_area))
                                    else:
                                        new_buildings[zoning_type] = 0
                    else:
                        # Set 0 for all valid zoning types if no saturation increase
                        for zoning_type in valid_zoning_types:
                            new_buildings[zoning_type] = 0
            
                    results.append(new_buildings)
            
                return pd.DataFrame(results)
    
    
            # Input file paths
            builtup_df_path = os.path.join(
                user_home, ana_klasor_yolu, il, ilce, proje_ismi,
                'sonuclar/SLF Sonuçları/1.Saturasyon/Çıktı/saturation_guncellenmis.xlsx'
            )
    
            construction_areas_path = os.path.join(
                user_home, ana_klasor_yolu, il, ilce, proje_ismi,
                'imar_analizi_sonuclari/imar_planlari/kofre_analiz/imar_tipi_ozet_tablo.xlsx'
            )
    
            zoning_path = os.path.join(
                user_home, ana_klasor_yolu, il, ilce, proje_ismi,
                f"imar_analizi_sonuclari/imar_planlari/saturasyon/girdiler/imar_plan_{il}_{ilce}_bina_kirilimlari.csv"
            )
    
            # SQLite database path
            db_path = os.path.join(
                user_home, ana_klasor_yolu, il, ilce, proje_ismi,
                'sonuclar/SLF Sonuçları/2.İmar Oranı Tahminleri/Çıktı/',
                f"{start_year}-{end_year}_veritabanı.db"
            )
    
            # Error handling for input files
            try:
                # Check if files exist
                for file_path in [builtup_df_path, construction_areas_path, zoning_path, db_path]:
                    if not os.path.exists(file_path):
                        raise FileNotFoundError(f"Dosya bulunamadı!!\n: {file_path}\n\nLütfen daha önceki aşamaları başarıyla tamamladığınıza emin olunuz.")
    
                # Load input files
                try:
                    builtup_df = pd.read_excel(builtup_df_path)
                except Exception as e:
                    raise Exception(f"Dosya bulunamadı!!\n: {builtup_df_path}: {str(e)}\n\nLütfen daha önceki aşamaları başarıyla tamamladığınıza emin olunuz.")
    
                try:
                    construction_areas = pd.read_excel(construction_areas_path, sheet_name='Sheet1')
                except Exception as e:
                    raise Exception(f"Dosya bulunamadı!!\n: {construction_areas_path}: {str(e)}\n\nLütfen daha önceki aşamaları başarıyla tamamladığınıza emin olunuz.")
    
                try:
                    as_is_buildings = pd.read_csv(zoning_path, encoding="cp1254")
                except Exception as e:
                    raise Exception(f"Dosya bulunamadı!!\n: {zoning_path}: {str(e)}\n\nLütfen daha önceki aşamaları başarıyla tamamladığınıza emin olunuz.")
    
                # Convert construction_areas to dictionary
                try:
                    construction_areas_dict = construction_areas.set_index('imar_tipi')['bina_basi_brut_alan'].to_dict()
                except Exception as e:
                    raise Exception(f"Error creating dictionary from construction_areas: {str(e)}")
    
            except FileNotFoundError as e:
                print(f"Error: {str(e)}")
                sys.exit(1)
            except Exception as e:
                print(f"Error: {str(e)}")
                sys.exit(1)
    
            os.makedirs(os.path.join(
                user_home, ana_klasor_yolu, il, ilce, proje_ismi,
                'sonuclar/SLF Sonuçları/3.Bina Sayısı/Çıktı'), exist_ok=True)
    
    
            # SQLite database path for output
            output_db_path = os.path.join(
                user_home, ana_klasor_yolu, il, ilce, proje_ismi,
                'sonuclar/SLF Sonuçları/3.Bina Sayısı/Çıktı/',
                f"bina_sayisi_hesaplama_{start_year}_{end_year}.db")
    
            # Function to save DataFrame to SQLite
            def SqlKaydet(df, table_name, db_path):
                conn = sqlite3.connect(db_path)
                df.to_sql(table_name, conn, if_exists='replace', index=False)
                conn.close()
    
    
            # Process each year
            for year in range(start_year, end_year+1):
    
                print(f'{year} yılına ait yeni bina sayıları oluşturuluyor...')
    
                # Load zoning ratios from SQLite database
                try:
                    conn = sqlite3.connect(db_path)
                    zoning_ratios = pd.read_sql(f"SELECT * FROM '{year}'", conn)
                    
                    # Rename zoning_ratios columns to remove '_ORAN' suffix
                    zoning_ratios.columns = [col.replace('_ORAN', '') if '_ORAN' in col else col for col in zoning_ratios.columns]
                    
                    conn.close()
                except sqlite3.OperationalError as e:
                    print(f"Hata: {year} yılına ait tablo {db_path} veritabanında bulunamadı!")
                    raise e
    
                # New building calculations
                new_buildings_df = calculate_new_buildings(builtup_df, zoning_ratios, construction_areas_dict, year, as_is_buildings)
                SqlKaydet(new_buildings_df, f"new_buildings_{year}", output_db_path)  # Write new buildings to SQLite
    
                # Calculate TOPLAM_BINA_SAYISI in as_is_buildings as the sum of specified columns
                building_columns = [
                    '1-2 KATLI MESKEN', '3-4 KATLI MESKEN', '5-7 KATLI MESKEN', '8 USTU KATLI MESKEN',
                    'AYDINLATMA', 'BUYUK_SANAYI', 'BUYUK_TICARETHANE', 'KUCUK_SANAYI', 'KUCUK_TICARETHANE',
                    'ORTA_SANAYI', 'ORTA_TICARETHANE', 'TARIMSAL_SULAMA', 'VILLA MESKEN'
                ]
                as_is_buildings['TOPLAM_BINA_SAYISI'] = as_is_buildings[building_columns].sum(axis=1)
    
                # Combine with as-is buildings
                as_is_buildings.set_index('id', inplace=True)
                new_buildings_df.set_index('id', inplace=True)
                combined_df = as_is_buildings.add(new_buildings_df, fill_value=0).reset_index()
    
                # Reorder columns
                column_order = ['id', 'left', 'top', 'right', 'bottom'] + [col for col in combined_df.columns if col not in ['id', 'left', 'top', 'right', 'bottom']]
                combined_df = combined_df[column_order]
    
                # Write combined DataFrame to SQLite
                SqlKaydet(combined_df, f"combined_{year}", output_db_path)
    
                # Update as-is buildings for the next year
                as_is_buildings = combined_df.copy()
    
            print("Hücre bazlı bina sayıları tahminleri başarıyla kaydedildi.!!\n...\n...\n")
    
    
            time.sleep(2)
    
            # --------------------------------------------YENİ GENİSLEME ALANLARI ---------------------------------------------------#
    
            print("Yeni genişleme alanları ile alakalı metodoloji başlıyor...\n....\n....")
    
    
            # Get the config path from the first command-line argument
            config_path = sys.argv[1]
            user_home = os.path.expanduser('~')
    
    
            # Load the config.json file
            with open(config_path, 'r', encoding='utf-8') as f:
                config = json.load(f)
    
            # Extract the necessary paths from config.json
            ana_klasor_yolu = config['Ana_Klasör_Yolu']
            il = config['İl']
            ilce = config['İlçe']
            proje_ismi = config['proje_ismi']
            start_year = config['baslangıc_yılı']
            end_year = config['bitis_yılı']
            program_dosyaları_path = config['program_dosyaları_path']
    
    
            # Construct the paths using string concatenation
            bina_abone_sayilari_path_mesken = os.path.join(
                user_home, 
                ana_klasor_yolu, 
                il, 
                ilce, 
                proje_ismi, 
                'imar_analizi_sonuclari/deep_learning_modeli', 
                f'mesken_data_{il}_{ilce}.csv'
            )
    
    
            bina_abone_sayilari_df_mesken = pd.read_csv(bina_abone_sayilari_path_mesken)
    
    
            bina_abone_sayilari_path_other = os.path.join(
                user_home, 
                ana_klasor_yolu, 
                il, 
                ilce, 
                proje_ismi, 
                'imar_analizi_sonuclari/deep_learning_modeli', 
                f'other_data_{il}_{ilce}.csv'
            )
    
            bina_abone_sayilari_df_other = pd.read_csv(bina_abone_sayilari_path_other)
    
    
            # Saturasyon büyüme parametreleri
            yayilma_parametreleri = {
                0: (0.05, 0.1),
                1: (0.1, 0.125),
                2: (0.125, 0.167),
                3: (0.167, 0.25),
                4: (0.25, 0.5),
                5: (0.5, 1)
            }
    
    
            # Yarıçap artışı (metre)
            mesafe_artis = {
                0: 50,
                1: 100,
                2: 150,
                3: 200,
                4: 250,
                5: 300
            }
    
    
            # Bina başına ortalama abone sayıları
            bina_abone_sayilari = {
                "1-2_KATLI_MESKEN": bina_abone_sayilari_df_mesken[bina_abone_sayilari_df_mesken['IMAR_ID_FORECAST'] == 1]['ABONE_SAYISI'].mean(),
                "3-4_KATLI_MESKEN": bina_abone_sayilari_df_mesken[bina_abone_sayilari_df_mesken['IMAR_ID_FORECAST'] == 2]['ABONE_SAYISI'].mean(),
                "5-7_KATLI_MESKEN": bina_abone_sayilari_df_mesken[bina_abone_sayilari_df_mesken['IMAR_ID_FORECAST'] == 3]['ABONE_SAYISI'].mean(),
                "8_USTU_KATLI_MESKEN": bina_abone_sayilari_df_mesken[bina_abone_sayilari_df_mesken['IMAR_ID_FORECAST'] == 4]['ABONE_SAYISI'].mean(),
                "VILLA_MESKEN": bina_abone_sayilari_df_mesken[bina_abone_sayilari_df_mesken['IMAR_ID_FORECAST'] == 5]['ABONE_SAYISI'].mean(),
                "ORTA_TICARETHANE": bina_abone_sayilari_df_other[bina_abone_sayilari_df_other['BINA_TIPI'] == "ORTA_TICARETHANE"][['AYDINLATMA_count', 'MESKEN_count', 'SANAYI_count', 'TARIMSAL_SULAMA_count', 'TICARETHANE_count']].sum(axis=1).mean(),
                "KUCUK_TICARETHANE": bina_abone_sayilari_df_other[bina_abone_sayilari_df_other['BINA_TIPI'] == "KUCUK_TICARETHANE"][['AYDINLATMA_count', 'MESKEN_count', 'SANAYI_count', 'TARIMSAL_SULAMA_count', 'TICARETHANE_count']].sum(axis=1).mean(),
                "KUCUK_SANAYI": bina_abone_sayilari_df_other[bina_abone_sayilari_df_other['BINA_TIPI'] == "KUCUK_SANAYI"][['AYDINLATMA_count', 'MESKEN_count', 'SANAYI_count', 'TARIMSAL_SULAMA_count', 'TICARETHANE_count']].sum(axis=1).mean(),
                "ORTA_SANAYI": bina_abone_sayilari_df_other[bina_abone_sayilari_df_other['BINA_TIPI'] == "ORTA_SANAYI"][['AYDINLATMA_count', 'MESKEN_count', 'SANAYI_count', 'TARIMSAL_SULAMA_count', 'TICARETHANE_count']].sum(axis=1).mean(),
                "TARIMSAL_SULAMA": bina_abone_sayilari_df_other[bina_abone_sayilari_df_other['BINA_TIPI'] == "TARIMSAL_SULAMA"][['AYDINLATMA_count', 'MESKEN_count', 'SANAYI_count', 'TARIMSAL_SULAMA_count', 'TICARETHANE_count']].sum(axis=1).mean(),
                "AYDINLATMA": bina_abone_sayilari_df_other[bina_abone_sayilari_df_other['BINA_TIPI'] == "AYDINLATMA"][['AYDINLATMA_count', 'MESKEN_count', 'SANAYI_count', 'TARIMSAL_SULAMA_count', 'TICARETHANE_count']].sum(axis=1).mean()
            }
    
    
            # Abone tipine göre ortalama tüketimler
            abone_tuketimleri = {
                "MESKEN": bina_abone_sayilari_df_mesken['ORT_Mesken_tuketim'].mean(),
                "SANAYI": bina_abone_sayilari_df_other[
                            (bina_abone_sayilari_df_other['BINA_TIPI'] == 'ORTA_SANAYI') | 
                            
                            (bina_abone_sayilari_df_other['BINA_TIPI'] == 'KUCUK_SANAYI') |
                            (bina_abone_sayilari_df_other['BINA_TIPI'] == 'BUYUK_SANAYI')
                        ]['SANAYI_tuketim'].mean(),
                "TICARETHANE": bina_abone_sayilari_df_other[
                            (bina_abone_sayilari_df_other['BINA_TIPI'] == 'ORTA_TICARETHANE') | 
                            (bina_abone_sayilari_df_other['BINA_TIPI'] == 'KUCUK_TICARETHANE') |
                            (bina_abone_sayilari_df_other['BINA_TIPI'] == 'BUYUK_TICARETHANE')
                        ]['TICARETHANE_tuketim'].mean(),
                "TARIMSAL_SULAMA": (
                    bina_abone_sayilari_df_other[
                        bina_abone_sayilari_df_other['BINA_TIPI'] == 'TARIMSAL_SULAMA'
                    ]['TARIMSAL_SULAMA_tuketim'].mean()
                    if 'TARIMSAL_SULAMA_tuketim' in bina_abone_sayilari_df_other.columns and
                    len(bina_abone_sayilari_df_other[bina_abone_sayilari_df_other['BINA_TIPI'] == 'TARIMSAL_SULAMA']) > 0
                    else 1.83405133
                ),
                "AYDINLATMA": bina_abone_sayilari_df_other[
                            bina_abone_sayilari_df_other['BINA_TIPI'] == 'AYDINLATMA'
                        ]['AYDINLATMA_tuketim'].mean(),
            }
    
    
    
            # Haversine mesafe hesaplama fonksiyonu
            def haversine(lat1, lon1, lat2, lon2):
                R = 6371000  # radius of Earth in meters
                phi1 = math.radians(lat1)
                phi2 = math.radians(lat2)
                dphi = math.radians(lat2 - lat1)
                dlambda = math.radians(lon2 - lon1)
                a = math.sin(dphi / 2) ** 2 + math.cos(phi1) * math.cos(phi2) * math.sin(dlambda / 2) ** 2
                return R * 2 * math.atan2(math.sqrt(a), math.sqrt(1 - a))
    
    
            # Normalize etme fonksiyonu (pozitif değerler için)
            def normalize_pozitif_carpan(carpan, orta_nokta=1000, duyarlilik=500):
                # Negatif değer kontrolü
                carpan = max(0, carpan)
                # Normalize et
                return 1 / (1 + math.exp(-(carpan - orta_nokta) / duyarlilik))
    
            # Function to save DataFrame to SQLite
            def SqlKaydet(df, table_name, db_path):
                try:
                    conn = sqlite3.connect(db_path)
                    df.to_sql(table_name, conn, if_exists='replace', index=False)
                    conn.close()
                except Exception as e:
                    print(f" {db_path} veritabanına {table_name} tablosu kaydedilirken hata oluştu: {e}")
                    raise e
    
    
            # ana YGA kodu
            def YGA_metodu(start_year, end_year):
    
                # Dosya yolları
                kullanici_girdisi_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                    f'sonuclar/SLF Sonuçları/Yga/girdi/yga_poligonlar.xlsx')
                
                # Check if the file exists
                if not os.path.exists(kullanici_girdisi_path):
                    print(f"Herhangi bir YGA poligonu bulunamadı.\n.\n Path: {kullanici_girdisi_path}. \n.\n Abone sayılarını hesaplarken Yeni Genişleme Alanları dikkate alınmayacaktır.")
                    return  # Exit the function if the file does not exist
                
    
                bolge_verisi_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                    f'imar_analizi_sonuclari/imar_planlari/saturasyon/hucre/hucre_alanlar.csv')
    
                bina_alan_verisi_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                    f'imar_analizi_sonuclari/imar_planlari/kofre_analiz/imar_tipi_ozet_tablo.xlsx')
    
                vejetatif_saturasyon_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                    f'sonuclar/SLF Sonuçları/1.Saturasyon/Çıktı/saturation_guncellenmis.xlsx')
    
                # SQLite database path for ana_algoritma_sonuclari
                ana_algoritma_path = os.path.join(
                    user_home, ana_klasor_yolu, il, ilce, proje_ismi,
                    'sonuclar/SLF Sonuçları/3.Bina Sayısı/Çıktı/',
                    f"bina_sayisi_hesaplama_{start_year}_{end_year}.db"
                )
    
    
                # Kullanıcı girdisi, bölge verisi, bina alanı ve vejetatif saturasyon verilerini oku
                kullanici_girdisi = pd.read_excel(kullanici_girdisi_path)
                bolge_verisi = pd.read_csv(bolge_verisi_path)
                bina_alan_verisi = pd.read_excel(bina_alan_verisi_path)
                vejetatif_saturasyon = pd.read_excel(vejetatif_saturasyon_path)
                
                # Bina alanı verisini sözlüğe çevir
                bina_alanlari = {}
                for _, row in bina_alan_verisi.iterrows():
                    bina_alanlari[row['imar_tipi']] = row['bina_basi_brut_alan']
                
                # Başlangıç yılı ve bitiş yılı
                min_year = start_year  # Minimum yıl
                end_year = end_year  # Bitiş yılı
                
                # Alt kırılım kolonlarını tespit et (1-2_KATLI_MESKEN gibi imar tiplerini içeren kolonlar)
                imar_kolonlari =  [
                        '1-2 KATLI MESKEN', '3-4 KATLI MESKEN', '5-7 KATLI MESKEN', '8 USTU KATLI MESKEN',
                        'AYDINLATMA', 'KUCUK_SANAYI', 'KUCUK_TICARETHANE','ORTA_SANAYI', 'ORTA_TICARETHANE', 
                        'TARIMSAL_SULAMA', 'VILLA MESKEN'
                    ]
                
    
                os.makedirs(os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                    'sonuclar/SLF Sonuçları/YGA/çıktı'),exist_ok=True)
                
                # Çıktı dosya yolları
                yga_output_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                    f'sonuclar/SLF Sonuçları/Yga/çıktı/yga_sonuc.xlsx')
    
                yillik_degisim_output_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                    f'sonuclar/SLF Sonuçları/Yga/çıktı/yga_yıllık_degisim.xlsx')
    
                birlestirilmis_output_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                    f'sonuclar/SLF Sonuçları/Yga/çıktı/yga_birlestirilmis.xlsx')
                
                # Ana algoritma sonuçlarını oku
                ana_algoritma_sonuclari = {}
                try:
                    conn = sqlite3.connect(ana_algoritma_path)
                    cursor = conn.cursor()
                    cursor.execute("SELECT name FROM sqlite_master WHERE type='table';")
                    tables = cursor.fetchall()
                    
                    for table_name in tables:
                        table_name = table_name[0]
                        if table_name.startswith('new_buildings_'):
                            try:
                                year = int(table_name.replace('new_buildings_', ''))
                                ana_algoritma_sonuclari[year] = pd.read_sql_query(f"SELECT * FROM {table_name}", conn)
                            except ValueError:
                                continue
                        else:
                            continue
                    conn.close()
                except Exception as e:
                    print(f"Ana algoritma sonuçları okunamadı: {str(e)}")
                    ana_algoritma_sonuclari = {}
                
                # Vejetatif saturasyon verilerini yıllara göre sözlüğe dönüştür
                vejetatif_saturasyon_by_year = {}
                for year in range(min_year, end_year + 1):
                    col_name = f"Saturation_updated_{year}"
                    if col_name in vejetatif_saturasyon.columns:
                        vejetatif_saturasyon_by_year[year] = dict(zip(vejetatif_saturasyon['id'], vejetatif_saturasyon[col_name]))
                    else:
                        print(f"UYARI: Vejetatif saturasyon verisi {year} yılı için bulunamadı")
                
                # Kullanıcının tanımladığı YGA poligonlarını belirle
                yga_poligonlari = kullanici_girdisi['Polygon ID'].unique()
    
                print(f"Bulunan poligon ID'leri: {yga_poligonlari}")
                
                # Her poligon için başlangıç yılını, hız faktörünü ve TAKS oranını belirle
                poligon_start_years = {}
                poligon_speed_factors = {}
                poligon_taks_oranlari = {}  # YENİ: TAKS oranları
                
                for poligon_id in yga_poligonlari:
                    start_years = kullanici_girdisi.loc[kullanici_girdisi['Polygon ID'] == poligon_id, 'Başlangıç Yılı'].unique()
                    poligon_start_years[poligon_id] = start_years[0]
                
                    speed_factors = kullanici_girdisi.loc[kullanici_girdisi['Polygon ID'] == poligon_id, 'Satürasyon Hızı'].unique()
                    poligon_speed_factors[poligon_id] = speed_factors[0]
                
                    # YENİ: TAKS oranını oku
                    taks_oranlari = kullanici_girdisi.loc[kullanici_girdisi['Polygon ID'] == poligon_id, 'TAKS'].unique()
                    poligon_taks_oranlari[poligon_id] = taks_oranlari[0]
                    print(f"Poligon {poligon_id}: TAKS oranı = {taks_oranlari[0]}")
                
                # ÖNEMLİ OPTİMİZASYON: En erken başlangıç yılını bulalım
                # Bu yıldan önceki yıllar için hiçbir hesaplama yapılmayacak
                min_start_year = min(poligon_start_years.values())
    
                print(f"En erken başlangıç yılı: {min_start_year}")
                
                # Bölge verisine x, y koordinatlarını ekle
                if 'lat' in vejetatif_saturasyon.columns and 'lon' in vejetatif_saturasyon.columns:
                    coord_dict = dict(zip(vejetatif_saturasyon['id'], zip(vejetatif_saturasyon['lat'], vejetatif_saturasyon['lon'])))
                    bolge_verisi['x'] = bolge_verisi['id'].map(lambda id: coord_dict.get(id, (0, 0))[0])
                    bolge_verisi['y'] = bolge_verisi['id'].map(lambda id: coord_dict.get(id, (0, 0))[1])
                elif 'left' in bolge_verisi.columns and 'right' in bolge_verisi.columns:
                    bolge_verisi['x'] = (bolge_verisi['left'] + bolge_verisi['right']) / 2
                    bolge_verisi['y'] = (bolge_verisi['top'] + bolge_verisi['bottom']) / 2
                else:
                    print("UYARI: Koordinat verileri bulunamadı. Rastgele koordinatlar kullanılacak.")
                    bolge_verisi['x'] = np.random.rand(len(bolge_verisi))
                    bolge_verisi['y'] = np.random.rand(len(bolge_verisi))
                
                # DEĞİŞİKLİK 1: Komşuluk ilişkilerini belirle - mesafe parametresi güncellendi
                def find_neighbors(bolge_verisi, max_distance=0.001):  # Yaklaşık 100 metre
                    neighbors = defaultdict(list)
                    hucre_list = bolge_verisi.to_dict('records')
                
                    for i, hucre1 in enumerate(hucre_list):
                        for j, hucre2 in enumerate(hucre_list):
                            if i == j:
                                continue
                        
                            distance = math.sqrt((hucre1['x'] - hucre2['x'])**2 + (hucre1['y'] - hucre2['y'])**2)
                            if distance <= max_distance:
                                neighbors[hucre1['id']].append(hucre2['id'])
                
                    return neighbors
                
                # Komşuluk ilişkilerini bul
                neighbors = find_neighbors(bolge_verisi)
                
                # YGA hücrelerini takip etmek için veri yapısı oluştur
                yga_takip = {}
    
                for _, row in kullanici_girdisi.iterrows():
                    id_ = row['id']
                    poligon_id = row['Polygon ID']
                
                    if id_ not in yga_takip:
                        yga_takip[id_] = {
                            'poligon_id': poligon_id,
                            'start_year': row['Başlangıç Yılı'],
                            'speed_factor': row['Satürasyon Hızı'],
                            'taks': row['TAKS'],  # YENİ: TAKS oranını ekle
                            'imar_oranlari': {kolon: row[kolon] for kolon in imar_kolonlari},
                            'saturasyon': {},  # Her yıl için saturasyon değerleri
                            'yeni_binalar': {kolon: {} for kolon in imar_kolonlari},  # Her yıl ve imar tipi için yeni bina sayıları
                            'dalga_seviyesi': 0,  # Dalga seviyesi - başlangıçta tüm hücreler 0
                            'onceden_bina_var': False  # YENİ: Bu hücrede önceden bina var mı?
                        }
                
                # YENİ: Önceden bina olan hücreleri tespit et
                print("Önceden bina olan hücreler tespit ediliyor...")
    
                for id_, info in yga_takip.items():
                    start_year = info['start_year']
                    vej_saturasyon = vejetatif_saturasyon_by_year.get(start_year, {}).get(id_, 0)
                
                    # Eğer başlangıç yılında saturasyon 0'dan büyükse, önceden bina var demektir
                    if vej_saturasyon > 0:
                        yga_takip[id_]['onceden_bina_var'] = True
                        print(f"Hücre {id_}: Önceden bina var (saturasyon: {vej_saturasyon:.3f})")
                
                # Hesaplama sonuçlarını saklamak için yapı
                sonuclar = {}
                
                # DEĞİŞİKLİK 2: Her poligon için başlangıç hücrelerini belirle (ilk dalga) - mantık güncellendi
                poligon_baslangic_hucreleri = {}
                for poligon_id in yga_poligonlari:
                    # Poligona ait hücreleri bul
                    poligon_hucreleri = [id_ for id_, info in yga_takip.items() if info['poligon_id'] == poligon_id]
                    hucre_saturasyonlari = []
                
                    for id_ in poligon_hucreleri:
                        start_year = yga_takip[id_]['start_year']
                        vej_saturasyon = vejetatif_saturasyon_by_year.get(start_year, {}).get(id_, 0)
                        hucre_saturasyonlari.append((id_, vej_saturasyon))
                
                    # Saturasyona göre sırala
                    hucre_saturasyonlari.sort(key=lambda x: x[1], reverse=True)
                
                    # Poligon büyüklüğüne göre hedef başlangıç hücre sayısını belirle - %10 oranı
                    n_baslangic = max(1, int(len(poligon_hucreleri) * 0.1))
                
                    # 1. DURUM: Eğer saturasyonu 0'dan büyük olan hücreler varsa
                    dolu_hucreler = [(id_, sat) for id_, sat in hucre_saturasyonlari if sat > 0]
                
                    if dolu_hucreler:
                        # Dolu hücrelerin sayısı poligonun %10'undan az mı kontrol et
                        if len(dolu_hucreler) <= n_baslangic:
                            # Eğer dolu hücre sayısı az ise, tümünü seç
                            baslangic_hucreleri = [id_ for id_, _ in dolu_hucreler]
                            print(f"Poligon {poligon_id} için {len(baslangic_hucreleri)} dolu hücre 1. dalga olarak seçildi (toplam {len(poligon_hucreleri)} hücreden)")
                        else:
                            # Çok sayıda dolu hücre var, saturasyonu en yüksek olan %10'unu seç
                            # Dolu hücreleri saturasyona göre zaten sıralamıştık
                            baslangic_hucreleri = [id_ for id_, _ in dolu_hucreler[:n_baslangic]]
                            print(f"Poligon {poligon_id} için saturasyonu en yüksek {len(baslangic_hucreleri)} dolu hücre 1. dalga olarak seçildi (%10 sınırı) (toplam {len(dolu_hucreler)} dolu hücre, {len(poligon_hucreleri)} toplam hücre)")
                    else:
                        # 2. DURUM: Eğer tamamen boşsa, coğrafi merkeze yakın olanları seç
                        print(f"Poligon {poligon_id} tamamen boş. Coğrafi merkeze göre başlangıç hücreleri seçiliyor.")
                    
                        # Coğrafi merkezi hesapla
                        if 'x' in bolge_verisi.columns and 'y' in bolge_verisi.columns:
                            merkez_x = bolge_verisi.loc[bolge_verisi['id'].isin(poligon_hucreleri), 'x'].mean()
                            merkez_y = bolge_verisi.loc[bolge_verisi['id'].isin(poligon_hucreleri), 'y'].mean()
                        
                            # Merkeze olan uzaklıkları hesapla
                            merkez_uzakliklari = []
                            for id_ in poligon_hucreleri:
                                x = bolge_verisi.loc[bolge_verisi['id'] == id_, 'x'].values[0]
                                y = bolge_verisi.loc[bolge_verisi['id'] == id_, 'y'].values[0]
                                uzaklik = math.sqrt((x - merkez_x)**2 + (y - merkez_y)**2)
                                merkez_uzakliklari.append((id_, uzaklik))
                        
                            # Merkeze en yakın %20 hücreyi seç
                            n_merkez = max(1, int(len(poligon_hucreleri) * 0.2))
                            merkez_uzakliklari.sort(key=lambda x: x[1])  # Uzaklığa göre sırala
                            baslangic_hucreleri = [id_ for id_, _ in merkez_uzakliklari[:n_merkez]]
                            print(f"Poligon {poligon_id} için coğrafi merkeze en yakın {len(baslangic_hucreleri)} hücre 1. dalga olarak seçildi.")
                        else:
                            # Eğer koordinat bilgisi yoksa rastgele seç
                            n_merkez = max(1, int(len(poligon_hucreleri) * 0.2))
                            baslangic_hucreleri = random.sample(poligon_hucreleri, min(n_merkez, len(poligon_hucreleri)))
                            print(f"Poligon {poligon_id} için koordinat bilgisi bulunamadı. Rastgele {len(baslangic_hucreleri)} hücre 1. dalga olarak seçildi.")
                
                    # Dalga seviyelerini güncelle
                    for id_ in baslangic_hucreleri:
                        yga_takip[id_]['dalga_seviyesi'] = 1
                
                    poligon_baslangic_hucreleri[poligon_id] = baslangic_hucreleri
                    print(f"Poligon {poligon_id} için toplam {len(baslangic_hucreleri)} başlangıç hücresi seçildi ({len(poligon_hucreleri)} toplam hücreden)")
                
                # BÜYÜK OPTİMİZASYON: Başlangıç yılından önceki yıllar için Excel dosyalarını hızlı bir şekilde oluşturalım
                # Bu yıllar için hiçbir hesaplama yapmadan sadece ana algoritma sonuçlarını kullanacağız
                hucre_ids = list(yga_takip.keys())
    
                for year in range(min_year, min_start_year):
                    print(f"{year} yılı işleniyor (sadece Ana Algoritma sonuçları kullanılıyor)...")
                
                    if year in ana_algoritma_sonuclari:
                        # Ana algoritma sonuçlarını direkt olarak birleştirilmiş dosyaya yazacağız
                        sonuclar[year] = {
                            'guncel_durum': pd.DataFrame({'id': hucre_ids, **{kolon: 0 for kolon in imar_kolonlari}, 'dalga_seviyesi': 0, 'saturasyon': 0}),
                            'yillik_degisim': pd.DataFrame({'id': hucre_ids, **{kolon: 0 for kolon in imar_kolonlari}, 'dalga_seviyesi': 0}),
                            'birlestirilmis': ana_algoritma_sonuclari[year].copy()
                        }
                    else:
                        print(f"UYARI: {year} yılı için ana algoritma sonuçları bulunamadı")
                
                # YGA işleme döngüsü - Sadece en erken başlangıç yılından itibaren
                for year in range(min_start_year, end_year + 1):
    
                    print(f"{year} yılı hesaplanıyor...")
                
                    # Bu yılda hesaplama yapılacak aktif poligonları belirle
                    aktif_poligonlar = [p_id for p_id, start_year in poligon_start_years.items() if year >= start_year]
    
                    if not aktif_poligonlar:
    
                        print(f"{year} yılında aktif poligon bulunmamaktadır. Ana algoritma sonuçları kullanılacak.")
                        if year in ana_algoritma_sonuclari:
                            sonuclar[year] = {
                                'guncel_durum': pd.DataFrame({'id': hucre_ids, **{kolon: 0 for kolon in imar_kolonlari}, 'dalga_seviyesi': 0, 'saturasyon': 0}).astype(float),
                                'yillik_degisim': pd.DataFrame({'id': hucre_ids, **{kolon: 0 for kolon in imar_kolonlari}, 'dalga_seviyesi': 0}).astype(float),
                                'birlestirilmis': ana_algoritma_sonuclari[year].copy()
                            }
                        continue
                
                    # Her poligon için o yıldaki aktif dalgaları işle
                    for poligon_id in aktif_poligonlar:
                        start_year = poligon_start_years[poligon_id]
                    
                        # Poligona ait hücreleri bul
                        poligon_hucreleri = [id_ for id_, info in yga_takip.items() if info['poligon_id'] == poligon_id]
                    
                        # Bu yılın dalga seviyelerini belirle
                        max_dalga_seviyesi = max(yga_takip[id_]['dalga_seviyesi'] for id_ in poligon_hucreleri)
                    
                        # Hız faktörünü kullanarak yılda kaç dalga ilerleyeceğini hesapla
                        speed_factor = poligon_speed_factors[poligon_id]
                        yillik_dalga_artisi = 1  # Baz dalga artışı
    
                        # Daha hassas dalga artışı
                        if speed_factor == 0:
                            yillik_dalga_artisi = 1  # Çok yavaş (2 yılda 1 dalga)
                        elif speed_factor == 1:
                            yillik_dalga_artisi = 1  # Yavaş (4 yılda 3 dalga)
                        elif speed_factor == 2:
                            yillik_dalga_artisi = 2  # Normal (yılda 1 dalga)
                        elif speed_factor == 3:
                            yillik_dalga_artisi = 3  # Hızlı (2 yılda 3 dalga)
                        elif speed_factor == 4:
                            yillik_dalga_artisi = 4  # Çok hızlı (yılda 2 dalga)
                        elif speed_factor == 5:
                            yillik_dalga_artisi = 5  # Son derece hızlı (yılda 3 dalga)
                    
                        # Bu yıl için hedef dalga seviyesi
                        y_since_start = year - start_year
                        hedef_dalga_seviyesi = min(1 + (y_since_start * yillik_dalga_artisi), 100)  # Maksimum 100 dalga seviyesi
                    
                        # Her dalga için yeni aktifleşen hücreleri takip et
                        yeni_aktif_hucreler = []
                    
                        # Hücreleri dalga seviyelerine göre işle (düşük seviyeden yükseğe)
                        for dalga in range(1, hedef_dalga_seviyesi + 1):
                            # Bu dalga seviyesindeki hücreleri bul
                            dalga_hucreleri = [id_ for id_ in poligon_hucreleri if yga_takip[id_]['dalga_seviyesi'] == dalga]
                        
                            # Bu dalga seviyesindeki hücreleri geliştir
                            for id_ in dalga_hucreleri:
                                # Saturasyon hesapla
                                if year > start_year and year-1 in yga_takip[id_]['saturasyon']:
                                    mevcut_saturasyon = yga_takip[id_]['saturasyon'][year-1]
                                else:
                                    mevcut_saturasyon = vejetatif_saturasyon_by_year.get(year, {}).get(id_, 0)
                            
                                # Eğer saturasyon zaten 1 ise, daha fazla büyüme olmaz
                                if mevcut_saturasyon >= 1.0:
                                    yga_takip[id_]['saturasyon'][year] = 1.0
                                    # Büyüme duruyor ama komşularını etkilemeye devam edebilir
                                else:
                                    # Boş alan oranı
                                    bos_alan_orani = 1 - mevcut_saturasyon
                                
                                    # Dalga seviyesine göre azalan büyüme hızı (erken dalgalar daha hızlı büyür)
                                    dalga_faktoru = max(0.2, 1.0 - (dalga - 1) * 0.1)  # 1. dalga: 1.0, 2. dalga: 0.9, ...
                                
                                    # Poligon hızını ayarla
                                    speed_factor = poligon_speed_factors[poligon_id]
                                    hiz_araligi = yayilma_parametreleri[speed_factor]
                                
                                    # Hücre hızını hesapla
                                    hucre_hizi = hiz_araligi[0] + (hiz_araligi[1] - hiz_araligi[0]) * dalga_faktoru
                                
                                    # Saturasyon artışını hesapla (maks. %30'u doldurabilir)
                                    max_artis = bos_alan_orani * 0.3  # Bir yılda boş alanın en fazla %30'u doldurulabilir
                                    saturation_growth = min(hucre_hizi, max_artis)
                                
                                    # Yeni saturasyonu hesapla
                                    yeni_saturasyon = min(mevcut_saturasyon + saturation_growth, 1.0)
                                    saturasyon_artisi = yeni_saturasyon - mevcut_saturasyon
                                
                                    # Saturasyonu kaydet
                                    yga_takip[id_]['saturasyon'][year] = yeni_saturasyon
                                
                                    # YENİ: Yeni binalar hesapla - TAKS kontrolü ile
                                    hucre_alani = bolge_verisi.loc[bolge_verisi['id'] == id_, 'cell_area'].values[0]
                                
                                    # Önceden bina var mı kontrolü
                                    onceden_bina_var = yga_takip[id_]['onceden_bina_var']
                                    poligon_taks = poligon_taks_oranlari[poligon_id]
                                
                                    for imar_tipi in imar_kolonlari:
                                        imar_orani = yga_takip[id_]['imar_oranlari'][imar_tipi]
                                    
                                        if onceden_bina_var:
                                            # ESKİ FORMÜL: Önceden bina olan hücreler
                                            yeni_bina_sayisi = hucre_alani * saturasyon_artisi * imar_orani / bina_alanlari[imar_tipi]
                                        else:
                                            # YENİ FORMÜL: Yeni gelişen hücrelerde TAKS oranı dahil
                                            yeni_bina_sayisi = (hucre_alani * saturasyon_artisi * imar_orani * poligon_taks) / bina_alanlari[imar_tipi]
                                    
                                        yeni_bina_sayisi = round(yeni_bina_sayisi, 2)
                                        yga_takip[id_]['yeni_binalar'][imar_tipi][year] = yeni_bina_sayisi
                                    
                                        # Debug için log (sadece ilk birkaç hücre için)
                                        if id_ in list(yga_takip.keys())[:3]:  # İlk 3 hücre için log
                                            print(f"Hücre {id_} ({imar_tipi}): önceden_bina={onceden_bina_var}, TAKS={poligon_taks}, bina={yeni_bina_sayisi:.3f}")
                            
                                # Önemli değişiklik: Saturasyon 1 olsa bile komşularını aktive et
                                # Komşu hücrelerin dalga seviyelerini güncelle (bir sonraki dalga)
                                if dalga < hedef_dalga_seviyesi:
                                    komsu_ids = neighbors.get(id_, [])
                                    for komsu_id in komsu_ids:
                                        # Komşu aynı poligonda ve henüz aktifleşmemişse
                                        if komsu_id in poligon_hucreleri and yga_takip[komsu_id]['dalga_seviyesi'] == 0:
                                            yga_takip[komsu_id]['dalga_seviyesi'] = dalga + 1
                                            yeni_aktif_hucreler.append(komsu_id)
                    
                        # Yeni aktifleşen hücrelerin ilk aktifleşme yılını kaydet
                        for id_ in yeni_aktif_hucreler:
                            if 'ilk_aktif_yil' not in yga_takip[id_]:
                                yga_takip[id_]['ilk_aktif_yil'] = year
                
                    if year in ana_algoritma_sonuclari:
    
                        # Ana algoritma sonuçlarını al
                        ana_sonuc = ana_algoritma_sonuclari[year].copy()
                    
                        # ID indeks eşleştirmesi oluştur
                        id_index_map = {id_: idx for idx, id_ in enumerate(hucre_ids)}
                    
                        # Güncel durumu ve yıllık değişimi tutacak DataFrame'ler - daha verimli oluştur
                        guncel_durum = pd.DataFrame(0, index=range(len(hucre_ids)),
                                                columns=['id'] + imar_kolonlari + ['dalga_seviyesi', 'saturasyon'])
                        guncel_durum['id'] = hucre_ids
                    
                        yillik_degisim = pd.DataFrame(0, index=range(len(hucre_ids)),
                                                    columns=['id'] + imar_kolonlari + ['dalga_seviyesi'])
                        yillik_degisim['id'] = hucre_ids
                    
                        # Yıllık değişimi ve güncel durumu hesapla - daha verimli indeksleme kullanarak
                        for id_, info in yga_takip.items():
                            idx = id_index_map[id_]
                        
                            # Dalga seviyesi ve saturasyonu güncelle
                            guncel_durum.iloc[idx, guncel_durum.columns.get_loc('dalga_seviyesi')] = info['dalga_seviyesi']
                            guncel_durum.iloc[idx, guncel_durum.columns.get_loc('saturasyon')] = info['saturasyon'].get(year, 0)
                            yillik_degisim.iloc[idx, yillik_degisim.columns.get_loc('dalga_seviyesi')] = info['dalga_seviyesi']
                        
                            # İmar tipleri için hesaplamalar
                            for kolon in imar_kolonlari:
                                # Yıllık değişim
                                if year in info.get('yeni_binalar', {}).get(kolon, {}):
                                    yillik_degisim.iloc[idx, yillik_degisim.columns.get_loc(kolon)] = info['yeni_binalar'][kolon][year]
                            
                                # Kümülatif durum
                                toplam_bina = 0
                                for y in range(info['start_year'], year + 1):
                                    if y in info.get('yeni_binalar', {}).get(kolon, {}):
                                        toplam_bina += info['yeni_binalar'][kolon][y]
                            
                                guncel_durum.iloc[idx, guncel_durum.columns.get_loc(kolon)] = toplam_bina
                    
                        # Ana sonuçları güncelle - daha verimli indeksleme kullanarak
                        if 'id' in ana_sonuc.columns:
                            ana_id_index_map = {id_: idx for idx, id_ in enumerate(ana_sonuc['id'])}
                        
                            for id_, info in yga_takip.items():
                                if year >= info['start_year'] and info['dalga_seviyesi'] > 0 and id_ in ana_id_index_map:
                                    idx = ana_id_index_map[id_]
                                    for kolon in imar_kolonlari:
                                        toplam_bina = 0
                                        for y in range(info['start_year'], year + 1):
                                            if y in info.get('yeni_binalar', {}).get(kolon, {}):
                                                toplam_bina += info['yeni_binalar'][kolon][y]
                                    
                                        if kolon in ana_sonuc.columns:
                                            ana_sonuc.iloc[idx, ana_sonuc.columns.get_loc(kolon)] = toplam_bina
                    
                        # Geçiş bölgesi için hibrit hesaplama
                        sinir_hucreleri = []
                        for id_, info in yga_takip.items():
                            if info['dalga_seviyesi'] > 0:  # Aktif hücreleri kullan
                                komsu_ids = neighbors.get(id_, [])
                                for komsu_id in komsu_ids:
                                    # Komşu başka poligonda veya YGA dışında
                                    if komsu_id not in yga_takip or yga_takip[komsu_id]['dalga_seviyesi'] == 0:
                                        if 'id' in ana_sonuc.columns and komsu_id in ana_id_index_map:
                                            # Bu komşu bir geçiş hücresi
                                            sinir_hucreleri.append((komsu_id, id_))
                    
                        # Geçiş hücrelerini işle - daha verimli indeksleme kullanarak
                        for dis_id, ic_id in sinir_hucreleri:
                            dis_idx = ana_id_index_map[dis_id]
                        
                            # Geçiş faktörü - mesafeye göre hesaplanabilir, burada sabit kullanıyoruz
                            gecis_faktoru = 0.3  # %30 YGA etkisi
                        
                            for kolon in imar_kolonlari:
                                if kolon in ana_sonuc.columns:
                                    # Vejetatif sonuç
                                    vej_bina = ana_sonuc.iloc[dis_idx, ana_sonuc.columns.get_loc(kolon)]
                                    vej_bina = 0 if pd.isna(vej_bina) else vej_bina
                                
                                    # YGA sonucu
                                    yga_bina = 0
                                    for y in range(yga_takip[ic_id]['start_year'], year + 1):
                                        if y in yga_takip[ic_id]['yeni_binalar'].get(kolon, {}):
                                            bina_ekle = yga_takip[ic_id]['yeni_binalar'][kolon][y]
                                            bina_ekle = 0 if pd.isna(bina_ekle) else bina_ekle
                                            yga_bina += bina_ekle
                                
                                    # Hibrit sonuç
                                    hibrit_bina = int((1 - gecis_faktoru) * vej_bina + gecis_faktoru * yga_bina)
                                
                                    # Ana sonuçları güncelle
                                    ana_sonuc.iloc[dis_idx, ana_sonuc.columns.get_loc(kolon)] = hibrit_bina
                    
                        # Bu yılın sonuçlarını sakla
                        sonuclar[year] = {
                            'guncel_durum': guncel_durum,
                            'yillik_degisim': yillik_degisim,
                            'birlestirilmis': ana_sonuc
                        }
                    else:
                        print(f"UYARI: {year} yılı için ana algoritma sonuçları bulunamadı")
                
                # Tüm hesaplamalar tamamlandıktan sonra Excel dosyalarını oluştur
                print("Hesaplamalar tamamlandı. Excel dosyaları oluşturuluyor...")
                
                # Excel seçeneklerini tamamen kaldır ve düz bir şekilde yazdır
                with pd.ExcelWriter(yga_output_path, engine="xlsxwriter") as yga_writer:
                    for year in range(min_year, end_year + 1):
                        if year in sonuclar and 'guncel_durum' in sonuclar[year] and sonuclar[year]['guncel_durum'] is not None:
                            # Excel'e yaz
                            sonuclar[year]['guncel_durum'].to_excel(yga_writer, sheet_name=str(year), index=False)
                
                with pd.ExcelWriter(yillik_degisim_output_path, engine="xlsxwriter") as yillik_degisim_writer:
                    for year in range(min_year, end_year + 1):
                        if year in sonuclar and 'yillik_degisim' in sonuclar[year] and sonuclar[year]['yillik_degisim'] is not None:
                            # Excel'e yaz
                            sonuclar[year]['yillik_degisim'].to_excel(yillik_degisim_writer, sheet_name=str(year), index=False)
                
                with pd.ExcelWriter(birlestirilmis_output_path, engine="xlsxwriter") as birlestirilmis_writer:
                    for year in range(min_year, end_year + 1):
                        if year in sonuclar and 'birlestirilmis' in sonuclar[year] and sonuclar[year]['birlestirilmis'] is not None:
                            # Excel'e yaz
                            sonuclar[year]['birlestirilmis'].to_excel(birlestirilmis_writer, sheet_name=str(year), index=False)
                
                print("YGA hesaplamaları tamamlandı ve sonuçlar kaydedildi.!!\n...\n...\n")
    
    
                time.sleep(3)
    
    
            YGA_metodu(start_year=start_year, end_year=end_year)
    
    
    
            # -------------------------------------------- KENTSEL DONUSUM ALANLARI ---------------------------------------------------#
    
    
            print("Kentsel dönüşüm alanlarına ait metodoloji uygulanıyor...\n....\n....")
    
    
            # Get the config path from the first command-line argument
            config_path = sys.argv[1]
            user_home = os.path.expanduser('~')
    
            # Load the config.json file
            with open(config_path, 'r', encoding='utf-8') as f:
                config = json.load(f)
    
            # Extract the necessary paths from config.json
            ana_klasor_yolu = config['Ana_Klasör_Yolu']
            il = config['İl']
            ilce = config['İlçe']
            proje_ismi = config['proje_ismi']
            start_year = config['baslangıc_yılı']
            end_year = config['bitis_yılı']
            program_dosyaları_path = config['program_dosyaları_path']
    
    
            # Saturasyon büyüme parametreleri
            yayilma_parametreleri = {
                0: (0.05, 0.1),
                1: (0.1, 0.125),
                2: (0.125, 0.167),
                3: (0.167, 0.25),
                4: (0.25, 0.5),
                5: (0.5, 1)
            }
    
    
            # Dosya yolları
            kullanici_girdisi_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                f'sonuclar/SLF Sonuçları/Kentsel Dönüşüm/girdi/kentsel_donusum_poligonlar.xlsx')
    
            bolge_verisi_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                f'imar_analizi_sonuclari/imar_planlari/saturasyon/hucre/hucre_alanlar.csv')
    
            bina_alan_verisi_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                f'imar_analizi_sonuclari/imar_planlari/kofre_analiz/imar_tipi_ozet_tablo.xlsx')
    
            # SQLite database path for ana_algoritma_sonuclari and kumulative_bina_sayilari
            ana_algoritma_path = os.path.join(
                user_home, ana_klasor_yolu, il, ilce, proje_ismi,
                'sonuclar/SLF Sonuçları/3.Bina Sayısı/Çıktı/',
                f"bina_sayisi_hesaplama_{start_year}_{end_year}.db"
            )
    
            kumulative_bina_path = ana_algoritma_path  # Same database as ana_algoritma_path
    
            # Function to normalize column names (replace spaces/hyphens with underscores, uppercase)
            def normalize_column_name(col_name):
                if isinstance(col_name, str):
                    return col_name.replace(' ', '_').replace('-', '_').upper()
                return col_name
    
            def kentsel_donusum(end_year):
    
                try:
    
                    # Kullanıcı girdisi, bölge verisi, bina alanı verilerini oku
                    try:
                        kullanici_girdisi = pd.read_excel(kullanici_girdisi_path)
                    except FileNotFoundError:
                        print(f"Kentsel donusum poligonları bulunamadı: {kullanici_girdisi_path}. İşlem pas geçiliyor...")
                        return # Exit the script with a non-zero status code to indicate an error
    
                    bolge_verisi = pd.read_csv(bolge_verisi_path)
                    bina_alan_verisi = pd.read_excel(bina_alan_verisi_path)
    
                    # Normalize column names across all data sources
                    # Normalize kullanici_girdisi columns
                    kullanici_girdisi.columns = [normalize_column_name(col) for col in kullanici_girdisi.columns]
    
                    # Normalize bina_alan_verisi columns
                    bina_alan_verisi.columns = [normalize_column_name(col) for col in bina_alan_verisi.columns]
    
                    # Bina alanı verisini sözlüğe çevir with normalized keys
                    bina_alanlari = {}
                    for _, row in bina_alan_verisi.iterrows():
                        normalized_key = normalize_column_name(row['IMAR_TIPI'])
                        bina_alanlari[normalized_key] = row['BINA_BASI_BRUT_ALAN']
    
                    # Validate input columns in kullanici_girdisi
                    required_columns = ['ID', 'BAŞLANGIÇ_YILI', 'SATÜRASYON_HIZI']  # Normalized names
                    if not all(col in kullanici_girdisi.columns for col in required_columns):
                        raise ValueError(f"Missing required columns in {kullanici_girdisi_path}: {required_columns}")
    
                    # Kentsel dönüşüm hızını kullanıcı girdisinden al
                    donusum_hizi_raw = kullanici_girdisi["SATÜRASYON_HIZI"].iloc[0]
                    if pd.isna(donusum_hizi_raw):
                        raise ValueError(f"'SATÜRASYON_HIZI' is NaN in {kullanici_girdisi_path}")
                    try:
                        donusum_hizi = int(float(donusum_hizi_raw))  # Handle numpy.int64, float, or int
                        if donusum_hizi not in yayilma_parametreleri:
                            raise ValueError(f"Invalid 'SATÜRASYON_HIZI' in {kullanici_girdisi_path}: {donusum_hizi_raw}. Must be an integer in {list(yayilma_parametreleri.keys())}")
                    except (ValueError, TypeError):
                        raise ValueError(f"Invalid 'SATÜRASYON_HIZI' in {kullanici_girdisi_path}: {donusum_hizi_raw}. Must be a number convertible to an integer in {list(yayilma_parametreleri.keys())}")
    
                    # Başlangıç yılı ve bitiş yılı
                    start_year_raw = kullanici_girdisi["BAŞLANGIÇ_YILI"].iloc[0]
                    if pd.isna(start_year_raw):
                        raise ValueError(f"'BAŞLANGIÇ_YILI' is NaN in {kullanici_girdisi_path}")
                    try:
                        start_year = int(float(start_year_raw))  # Handle numpy.int64, float, or int
                        if start_year <= 0:
                            raise ValueError(f"Invalid 'BAŞLANGIÇ_YILI' in {kullanici_girdisi_path}: {start_year_raw}. Must be a positive integer")
                    except (ValueError, TypeError):
                        raise ValueError(f"Invalid 'BAŞLANGIÇ_YILI' in {kullanici_girdisi_path}: {start_year_raw}. Must be a positive integer")
                    min_year = start_year
                    end_year = end_year
    
                    # Başlangıç yılı için kümülatif bina sayılarını oku - combined_{start_year} tablosundan
                    try:
                        conn = sqlite3.connect(kumulative_bina_path)
                        kumulative_bina_sayilari = pd.read_sql(f"SELECT * FROM 'combined_{start_year}'", conn)
                        # Normalize database column names
                        kumulative_bina_sayilari.columns = [normalize_column_name(col) for col in kumulative_bina_sayilari.columns]
                        conn.close()
                        print(f"Başlangıç bina sayıları {kumulative_bina_path} dosyasının combined_{start_year} tablosundan alındı.")
                    except Exception as e:
                        print(f"UYARI: Kümülatif bina sayıları combined_{start_year} tablosundan okunamadı: {str(e)}")
                        kumulative_bina_sayilari = pd.DataFrame()  # Empty DataFrame if table is missing
    
                    # Define imar_kolonlari in normalized format
                    imar_kolonlari = [
                        '1_2_KATLI_MESKEN', '3_4_KATLI_MESKEN', '5_7_KATLI_MESKEN', '8_USTU_KATLI_MESKEN',
                        'VILLA_MESKEN', 'AYDINLATMA', 'KUCUK_SANAYI', 'KUCUK_TICARETHANE', 'ORTA_SANAYI', 
                        'ORTA_TICARETHANE', 'TARIMSAL_SULAMA'
                    ]
    
                    # Validate imar_kolonlari in kullanici_girdisi
                    missing_cols = [col for col in imar_kolonlari if col not in kullanici_girdisi.columns]
                    if missing_cols:
                        raise ValueError(f"Missing zoning type columns in {kullanici_girdisi_path}: {missing_cols}")
    
                    # Validate bina_alanlari
                    missing_bina_alanlari = [col for col in imar_kolonlari if col not in bina_alanlari]
                    if missing_bina_alanlari:
                        raise ValueError(f"Missing zoning types in {bina_alan_verisi_path} for: {missing_bina_alanlari}")
    
    
                    os.makedirs(os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                    'sonuclar/SLF Sonuçları/Kentsel Dönüşüm/çıktı'),exist_ok=True)
    
                    # Çıktı dosya yolları
                    donusum_output_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                    'sonuclar/SLF Sonuçları/Kentsel Dönüşüm/çıktı/kentsel_donusum_sonuc.xlsx')
    
                    yillik_degisim_output_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                            'sonuclar/SLF Sonuçları/Kentsel Dönüşüm/çıktı/kentsel_donusum_yillik_degisim.xlsx')
    
                    birlestirilmis_output_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                            'sonuclar/SLF Sonuçları/Kentsel Dönüşüm/çıktı/kentsel_donusum_birlestirilmis.xlsx')
    
                    # Excel writer'lar
                    donusum_writer = pd.ExcelWriter(donusum_output_path, engine="xlsxwriter")
                    yillik_degisim_writer = pd.ExcelWriter(yillik_degisim_output_path, engine="xlsxwriter")
                    birlestirilmis_writer = pd.ExcelWriter(birlestirilmis_output_path, engine="xlsxwriter")
    
                    # Ana algoritma sonuçlarını oku
                    ana_algoritma_sonuclari = {}
                    try:
                        conn = sqlite3.connect(ana_algoritma_path)
                        cursor = conn.cursor()
                        cursor.execute("SELECT name FROM sqlite_master WHERE type='table' AND name LIKE 'new_buildings_%';")
                        tables = cursor.fetchall()
                        for table in tables:
                            table_name = table[0]
                            try:
                                year = int(table_name.replace('new_buildings_', ''))
                                df = pd.read_sql(f"SELECT * FROM '{table_name}'", conn)
                                df.columns = [normalize_column_name(col) for col in df.columns]
                                ana_algoritma_sonuclari[year] = df
                            except ValueError:
                                continue
                        conn.close()
                    except Exception as e:
                        print(f"Ana algoritma sonuçları okunamadı: {str(e)}")
                        ana_algoritma_sonuclari = {}
    
                    # Start year'dan önceki yılları kopyala
                    for year in range(min_year, start_year):
                        if year in ana_algoritma_sonuclari:
                            ana_algoritma_sonuclari[year].to_excel(birlestirilmis_writer, sheet_name=str(year), index=False)
                            print(f"{year} yılı verileri doğrudan kopyalandı.")
    
                    # Kentsel dönüşüm hücrelerini belirle
                    donusum_hucreleri = set(kullanici_girdisi["ID"].tolist())
    
                    # Başlangıç için saturasyon değerlerini belirle
                    tum_hucreler = bolge_verisi.copy()
                    tum_hucreler.columns = [normalize_column_name(col) for col in tum_hucreler.columns]
                    tum_hucreler["SATURASYON"] = 0.0  # Initialize as float
    
                    # Dönüşüm hücrelerinde saturasyon 1
                    for id_ in donusum_hucreleri:
                        tum_hucreler.loc[tum_hucreler['ID'] == id_, "SATURASYON"] = 1.0
    
                    # İmar tiplerini başlat
                    for kolon in imar_kolonlari:
                        tum_hucreler[kolon] = 0.0  # Initialize as float
                        for _, row in kullanici_girdisi.iterrows():
                            id_ = row['ID']
                            try:
                                value = float(row[kolon]) if not pd.isna(row[kolon]) else 0.0
                                tum_hucreler.loc[tum_hucreler['ID'] == id_, kolon] = value
                            except (ValueError, TypeError):
                                tum_hucreler.loc[tum_hucreler['ID'] == id_, kolon] = 0.0
    
                    # Validate tum_hucreler columns
                    if not pd.api.types.is_float_dtype(tum_hucreler['CELL_AREA']):
                        tum_hucreler['CELL_AREA'] = tum_hucreler['CELL_AREA'].astype(float)
    
                    # Bina takip matrisini hazırla
                    bina_takip = {}
                    for id_ in donusum_hucreleri:
                        bina_takip[id_] = {}
                        for kolon in imar_kolonlari:
                            bina_takip[id_][kolon] = {
                                "MEVCUT_BINA": 0.0,
                                "YENI_BINA": 0.0,
                                "DONUSUM_TAMAMLANDI": False,
                            }
    
                    # Kümülatif bina sayılarını aktar
                    for _, row in kumulative_bina_sayilari.iterrows():
                        id_ = row['ID']
                        if id_ in donusum_hucreleri:
                            for kolon in imar_kolonlari:
                                if kolon in row:
                                    bina_takip[id_][kolon]["MEVCUT_BINA"] = float(row[kolon])
    
                    # Yıllık değişim DataFrame'i
                    yillik_degisim_base = pd.DataFrame({'ID': list(donusum_hucreleri)})
                    yillik_degisim_base = yillik_degisim_base.set_index('ID')
    
                    # İlk yıl durumu
                    ilk_yil_durumu = pd.DataFrame({'ID': list(donusum_hucreleri)})
                    for kolon in imar_kolonlari:
                        ilk_yil_durumu[kolon] = ilk_yil_durumu['ID'].apply(lambda id_: bina_takip[id_][kolon]["MEVCUT_BINA"])
    
                    ilk_yil_durumu.to_excel(donusum_writer, sheet_name=str(start_year), index=False)
    
                    # Yıllık işlem
                    for year in range(start_year, end_year + 1):
    
                        print(f"{year} yılı hesaplanıyor...")
                        
                        # Yıllık değişim DataFrame'i
                        yillik_degisim = yillik_degisim_base.copy()
                        for kolon in imar_kolonlari:
                            yillik_degisim[kolon] = 0.0  # Initialize as float
                        
                        # Güncel durum DataFrame'i
                        guncel_durum = pd.DataFrame({'ID': list(donusum_hucreleri)})
                        
                        # Kentsel dönüşüm hesaplaması
                        for id_ in donusum_hucreleri:
                            hucre_toplam_bina = sum(bina_takip[id_][kolon]["MEVCUT_BINA"] for kolon in imar_kolonlari)
                            
                            if hucre_toplam_bina <= 0:
                                if tum_hucreler.loc[tum_hucreler['ID'] == id_, "SATURASYON"].iloc[0] >= 1:
                                    tum_hucreler.loc[tum_hucreler['ID'] == id_, "SATURASYON"] = 0.0
                                
                                # Saturasyon artışı
                                saturation_range = yayilma_parametreleri[donusum_hizi]
                                saturation_growth = float(random.uniform(*saturation_range))
                                
                                # Saturasyonu artır
                                current_saturasyon = float(tum_hucreler.loc[tum_hucreler['ID'] == id_, "SATURASYON"].iloc[0])
                                tum_hucreler.loc[tum_hucreler['ID'] == id_, "SATURASYON"] = min(current_saturasyon + saturation_growth, 1.0)
                                
                                # Yeni binalar
                                for kolon in imar_kolonlari:
                                    hucre_alani = float(tum_hucreler.loc[tum_hucreler['ID'] == id_, "CELL_AREA"].iloc[0])
                                    saturasyon = float(tum_hucreler.loc[tum_hucreler['ID'] == id_, "SATURASYON"].iloc[0])
                                    imar_orani = float(tum_hucreler.loc[tum_hucreler['ID'] == id_, kolon].iloc[0])
                                    
                                    insa_edilebilir_alan = hucre_alani * saturasyon * imar_orani
                                    yeni_bina_sayisi = insa_edilebilir_alan / bina_alanlari[kolon]
                                    yeni_bina_sayisi = round(yeni_bina_sayisi, 2)
                                    
                                    onceki_yeni_bina = bina_takip[id_][kolon]["YENI_BINA"]
                                    eklenen_bina = yeni_bina_sayisi - onceki_yeni_bina
                                    eklenen_bina = max(0, eklenen_bina)
                                    
                                    bina_takip[id_][kolon]["YENI_BINA"] = yeni_bina_sayisi
                                    yillik_degisim.loc[id_, kolon] = eklenen_bina
                            else:
                                yikma_orani = float(random.uniform(*yayilma_parametreleri[donusum_hizi]))
                                
                                for kolon in imar_kolonlari:
                                    if bina_takip[id_][kolon]["MEVCUT_BINA"] > 0:
                                        if "BASLANGIC_BINA" not in bina_takip[id_][kolon]:
                                            bina_takip[id_][kolon]["BASLANGIC_BINA"] = bina_takip[id_][kolon]["MEVCUT_BINA"]
                                        
                                        baslangic_bina = bina_takip[id_][kolon]["BASLANGIC_BINA"]
                                        azalacak_miktar = baslangic_bina * yikma_orani
                                        azalacak_miktar = min(azalacak_miktar, bina_takip[id_][kolon]["MEVCUT_BINA"])
                                        
                                        bina_takip[id_][kolon]["MEVCUT_BINA"] -= azalacak_miktar
                                        yillik_degisim.loc[id_, kolon] = -azalacak_miktar
                        
                        # Güncel durum
                        for kolon in imar_kolonlari:
                            guncel_durum[kolon] = guncel_durum['ID'].apply(
                                lambda id_: bina_takip[id_][kolon]["MEVCUT_BINA"] + bina_takip[id_][kolon]["YENI_BINA"]
                            )
                        
                        guncel_durum.to_excel(donusum_writer, sheet_name=str(year), index=False)
                        yillik_degisim.reset_index().to_excel(yillik_degisim_writer, sheet_name=str(year), index=False)
                        
                        # Ana algoritma sonuçlarını güncelle
                        if year in ana_algoritma_sonuclari:
                            ana_sonuc = ana_algoritma_sonuclari[year].copy()
                            birlestirilmis_sonuc = pd.merge(
                                ana_sonuc,
                                yillik_degisim.reset_index(),
                                on='ID',
                                how='left',
                                suffixes=('', '_YENI')
                            )
                            
                            for kolon in imar_kolonlari:
                                if kolon in birlestirilmis_sonuc.columns and f'{kolon}_YENI' in birlestirilmis_sonuc.columns:
                                    birlestirilmis_sonuc[f'{kolon}_YENI'] = birlestirilmis_sonuc[f'{kolon}_YENI'].fillna(0)
                                    birlestirilmis_sonuc[kolon] = birlestirilmis_sonuc[kolon] + birlestirilmis_sonuc[f'{kolon}_YENI']
                                    birlestirilmis_sonuc = birlestirilmis_sonuc.drop(columns=[f'{kolon}_YENI'])
                            
                            birlestirilmis_sonuc.to_excel(birlestirilmis_writer, sheet_name=str(year), index=False)
                        else:
                            yillik_degisim.reset_index().to_excel(birlestirilmis_writer, sheet_name=str(year), index=False)
    
                    # Dosyaları kaydet
                    donusum_writer.close()
                    yillik_degisim_writer.close()
                    birlestirilmis_writer.close()
    
                    print("Kentsel dönüşüm hesaplamaları tamamlandı ve sonuçlar kaydedildi.!!\n...\n...\n")
    
                except FileNotFoundError as e:
                    if str(e).find(kullanici_girdisi_path) != -1:
                        print(f"Kullanıcı girdisi dosyası bulunamadı: {kullanici_girdisi_path}. İşlem atlanıyor.")
                    else:
                        raise e
                except Exception as e:
                    print(f"Bir hata oluştu: {str(e)}")
                    raise e
    
    
            kentsel_donusum(end_year=end_year)
    
    
    
            time.sleep(3)
    
            # ----------------------------------------------- ABONE SAYISI TAHMİNİ --------------------------------------------------------#
    
    
            def abone_sayısı_tahmini():
    
                print("Hücre bazlı abone sayısı tahmini oluşturma algoritması başlıyor....\n....\n....")
    
                # Define file paths
                elf_sonuclar_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, ELF_sonuc_klasor, ELF_sonuc_name)
    
                os.makedirs(os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                        'sonuclar/SLF Sonuçları/4.Abone Sayısı/girdi'), exist_ok=True)
                os.makedirs(os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                        'sonuclar/SLF Sonuçları/4.Abone Sayısı/çıktı'), exist_ok=True)
                
                abone_artis_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 
                                                'sonuclar/SLF Sonuçları/4.Abone Sayısı/girdi/dagitilacak_abone_delta.xlsx')
    
                # Map ELF_sonuc_senaryo to sheet index
                senaryo_to_sheet = {
                    "minimum": 0,
                    "düşük": 1,
                    "baz": 2,
                    "yüksek": 3,
                    "maksimum": 4
                }
    
                # Validate ELF_sonuc_senaryo
                if ELF_sonuc_senaryo not in senaryo_to_sheet:
                    raise ValueError(f"Invalid ELF_sonuc_senaryo value: {ELF_sonuc_senaryo}. Must be one of {list(senaryo_to_sheet.keys())}")
    
                # Step 1: Open the Excel file and read the specified sheet
                try:
                    sheet_index = senaryo_to_sheet[ELF_sonuc_senaryo]
                    excel_file = pd.ExcelFile(elf_sonuclar_path)
                    if len(excel_file.sheet_names) != 5:
                        raise ValueError(f"Expected exactly 5 sheets in {elf_sonuclar_path}, but found {len(excel_file.sheet_names)}")
                    if sheet_index >= len(excel_file.sheet_names):
                        raise ValueError(f"Sheet index {sheet_index} (for senaryo '{ELF_sonuc_senaryo}') does not exist in {elf_sonuclar_path}")
                    sheet_name = excel_file.sheet_names[sheet_index]
                    df = pd.read_excel(elf_sonuclar_path, sheet_name=sheet_name)
                except FileNotFoundError:
                    raise Exception(
                        f"ELF Modülü Çalıştırılmamış.\n"
                        f"Ekonometrik Talep Tahmini Modülü Kısmından Modülü Çalıştırın."
                    )

                    # raise FileNotFoundError(f"Excel file not found at {elf_sonuclar_path}")
                except Exception as e:
                    raise Exception(f"Error reading Excel file {elf_sonuclar_path}: {str(e)}")
    
                # Step 2: Validate required columns
                required_columns = ['YIL', 'MESKEN_ABONE_SAYISI', 'SANAYI_ABONE_SAYISI', 
                                    'TICARETHANE_ABONE_SAYISI', 'TARIMSAL_SULAMA_ABONE_SAYISI', 
                                    'AYDINLATMA_ABONE_SAYISI']
                missing_cols = [col for col in required_columns if col not in df.columns]
                if missing_cols:
                    raise ValueError(f"Missing required columns in {elf_sonuclar_path} (sheet: {sheet_name}): {missing_cols}")
    
                # Step 3: Filter years between start_year and end_year + 1 (to calculate difference for end_year)
                df = df.sort_values('YIL')
                df['YIL'] = df['YIL'].astype(int)  # Ensure years are integers
                required_years = list(range(start_year, end_year + 2))  # +2 because range is exclusive of the end, and we need end_year + 1
                available_years = df['YIL'].tolist()
                missing_years = [yr for yr in required_years if yr not in available_years]
                if missing_years:
                    raise ValueError(f"Missing required years in {elf_sonuclar_path} (sheet: {sheet_name}): {missing_years}")
    
                # Filter the DataFrame to only include years in the required range
                df = df[df['YIL'].isin(required_years)]
                years = df['YIL'].tolist()
    
                # Define output columns
                output_columns = ['MESKEN', 'SANAYI', 'TICARETHANE', 'TARIMSAL_SULAMA', 'AYDINLATMA']
                input_columns = ['MESKEN_ABONE_SAYISI', 'SANAYI_ABONE_SAYISI', 'TICARETHANE_ABONE_SAYISI',
                                'TARIMSAL_SULAMA_ABONE_SAYISI', 'AYDINLATMA_ABONE_SAYISI']
    
                # Create an Excel writer for the output
                writer = pd.ExcelWriter(abone_artis_path, engine='xlsxwriter')
    
                # Step 4: Calculate yearly differences for years from start_year to end_year
                # Special case for start_year (2024): Write raw data (or zero differences)
                if start_year in years:
                    start_data = df[df['YIL'] == start_year][input_columns].iloc[0]
                    start_data.index = output_columns  # Rename columns for output
                    start_df = pd.DataFrame([start_data * 0], columns=output_columns)  # Set differences to 0 for 2024
                    start_df.to_excel(writer, sheet_name=str(start_year), index=False)
    
                # Calculate differences for subsequent years
                for i in range(len(years)-1):
                    current_year = years[i]
                    next_year = years[i + 1]
    
                    # Only process years from start_year to end_year
                    if current_year < start_year or current_year > end_year:
                        continue
    
                    # Validate consecutive years
                    if next_year != current_year + 1:
                        raise ValueError(f"Years are not consecutive: {current_year} to {next_year}")
    
                    # Get data for current and next year
                    current_data = df[df['YIL'] == current_year][input_columns].iloc[0]
                    next_data = df[df['YIL'] == next_year][input_columns].iloc[0]
    
                    # Calculate differences
                    differences = next_data - current_data
                    differences.index = output_columns  # Rename columns for output
    
                    # Create a DataFrame with one row of differences
                    diff_df = pd.DataFrame([differences], columns=output_columns)
    
                    # Write to a sheet named after the NEXT year (the year the difference applies to)
                    diff_df.to_excel(writer, sheet_name=str(next_year), index=False)
    
                # Step 5: Save the output file
                writer.close()
                print(f"{start_year} yılından {end_year} yılına kadar ardışık abone sayıları artışları kaydedildi: {abone_artis_path}")
    
                shutil.copy2(os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'imar_analizi_sonuclari/imar_planlari/hucre_abone_analizi/as_is_abone_sayılari.xlsx'), 
                            os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/4.Abone Sayısı/girdi/as_is_abone_sayılari.xlsx'))
    
                # Construct the paths
                as_is_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/4.Abone Sayısı/girdi/as_is_abone_sayılari.xlsx')
                output_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/4.Abone Sayısı/çıktı/ABONE_SAYISI_CIKTI.xlsx')
                abone_artis_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'sonuclar/SLF Sonuçları/4.Abone Sayısı/girdi/dagitilacak_abone_delta.xlsx')
                min_max_path = os.path.join(user_home, ana_klasor_yolu, il, ilce, proje_ismi, 'imar_analizi_sonuclari/deep_learning_modeli/min_max_table.xlsx')
    
                # SQLite database path for bina_artis_df
                output_db_path = os.path.join(
                    user_home, ana_klasor_yolu, il, ilce, proje_ismi,
                    'sonuclar/SLF Sonuçları/3.Bina Sayısı/Çıktı/',
                    f"bina_sayisi_hesaplama_{start_year}_{end_year}.db"
                )
    
                # Define tuketim_tipleri
                tuketim_tipleri = ['MESKEN', 'SANAYI', 'TICARETHANE', 'TARIMSAL_SULAMA', 'AYDINLATMA']
    
                # Monte Carlo ile ağırlık belirleme fonksiyonu
                def calculate_random_weights(min_max_df, imar_types, abone_tipi):
                    """
                    Belirli bir abone tipi için imar tiplerine göre rastgele ağırlıklar hesaplar
                    """
                    weights = {}
                    
                    for imar_type in imar_types:
                        # Min ve Max sütun adlarını oluştur
                        min_col = f"{imar_type} - min"
                        max_col = f"{imar_type} - max"
                        
                        # İlgili abone tipi satırını bul
                        abone_row = min_max_df[min_max_df['Category'] == abone_tipi]
                        
                        if not abone_row.empty:
                            min_val = abone_row[min_col].values[0]
                            max_val = abone_row[max_col].values[0]
                            
                            # Rastgele ağırlık hesapla
                            random_weight = np.random.uniform(min_val, max_val)
                            weights[imar_type] = random_weight
                        else:
                            # Abone tipi bulunamazsa 0 ağırlık ver
                            weights[imar_type] = 0
                            print(f"Uyarı: '{abone_tipi}' için herhangi bir değer bulunamadı. Lütfen ana abone verisinde bu abone tipine dair verilerin olup olmadığına bakabilirsiniz."
                                f"Şimdilik '{imar_type}' için ilgili ağırlıklar 0 alınıyor.")
                    
                    return weights
    
                def distribute_electric_load(min_max_df, bina_sayilari_df, abone_artis_df, num_simulations=100):
    
                    """
                    Abone artışlarını dağıtan fonksiyon - Orjinal dağıtım fonksiyonu ile aynı mantık, ilce kaldırıldı
                    """
                    # Yıllık abone artışlarını al (ilce olmadan)
                    tuketim_yillik = abone_artis_df.loc[:, tuketim_tipleri]
                    
                    # Sonuçları saklamak için boş bir DataFrame (ilce sütunu kaldırıldı)
                    results_df = bina_sayilari_df[['id', 'left', 'top', 'right', 'bottom']].copy()
                    for tuketim_tipi in tuketim_tipleri:
                        results_df[tuketim_tipi] = 0
                    
                    imar_types = bina_sayilari_df.columns[5:]  # imar tipleri 5. sütundan başlar
                    cols = min_max_df.columns.tolist()
                    cols = [col for col in cols if col != "Category"]
                    clean_cols = [col.replace(" - min", "").replace(" - max", "") for col in cols]
                    imar_types = list(set(clean_cols))

                    # Monte Carlo Simülasyonu: num_simulations kez çalıştır
                    simulation_results = []
                    
                    for sim in range(num_simulations):
                        temp_results = results_df.copy()
                        
                        # HER ABONE TİPİ İÇİN AYRI DÖNGÜ
                        for tuketim_tipi in tuketim_tipleri:
    
                            # Bu abone tipi için ağırlıkları hesapla
                            weights = calculate_random_weights(min_max_df, imar_types, tuketim_tipi)
                            
                            # Tüm hücreler için hesaplama
                            temp_df = bina_sayilari_df.copy()
                                        
                            # Ensure numeric data types for imar_type columns
                            for imar_type in imar_types:
                                temp_df[imar_type] = pd.to_numeric(temp_df[imar_type], errors='coerce').fillna(0)
                            
                            # HER HÜCRENİN BU ABONE TİPİ İÇİN AĞIRLIĞINI HESAPLA
                            temp_df[f'weight_{tuketim_tipi}'] = 0
                            for imar_type in imar_types:
                                contribution = temp_df[imar_type] * weights[imar_type]
                                temp_df[f'weight_{tuketim_tipi}'] += contribution
                            
                            # BU ABONE TİPİ İÇİN NORMALİZASYON
                            total_weight = temp_df[f'weight_{tuketim_tipi}'].sum()
                            
                            # Artış miktarını al (- değer de olabilir)
                            artis_miktari = tuketim_yillik[tuketim_tipi].values[0]
                            
                            # Sadece ARTIŞLARI dağıt, azalışları dağıtma
                            if artis_miktari > 0:
                                if total_weight > 0:
                                    # Normalleştirme ve dağıtım işlemi
                                    temp_df[tuketim_tipi] = (temp_df[f'weight_{tuketim_tipi}'] / total_weight) * artis_miktari
                                else:
                                    # Recalculate weights based on buildings present
                                    fallback_weight = 0
                                    temp_df[f'fallback_weight_{tuketim_tipi}'] = 0
                                    for imar_type in imar_types:
                                        if temp_df[imar_type].sum() > 0 and weights[imar_type] > 0:
                                            temp_df[f'fallback_weight_{tuketim_tipi}'] += temp_df[imar_type] * weights[imar_type]
                                            fallback_weight += weights[imar_type]
                                    
                                    fallback_total_weight = temp_df[f'fallback_weight_{tuketim_tipi}'].sum()
                                    
                                    if fallback_total_weight > 0:
                                        temp_df[tuketim_tipi] = (temp_df[f'fallback_weight_{tuketim_tipi}'] / fallback_total_weight) * artis_miktari
                                    else:
                                        temp_df[tuketim_tipi] = 0
                                    
                                    # Clean up fallback column
                                    temp_df = temp_df.drop(columns=[f'fallback_weight_{tuketim_tipi}'], errors='ignore')
                                
                                # Sonuçları geçici tabloya ekle
                                temp_results[tuketim_tipi] = temp_df[tuketim_tipi]
                            else:
                                # Azalış durumunda değer atama - azalışlar ayrı işlenecek
                                temp_results[tuketim_tipi] = 0
                        
                        # Simülasyon sonucunu kaydet
                        simulation_results.append(temp_results)
                    
                    # Simülasyonların ortalamasını al
                    final_results = pd.concat(simulation_results).groupby(['id', 'left', 'top', 'right', 'bottom']).mean().reset_index()
                    return final_results
    
                def apply_decrease(current_df, abone_artis_df, bina_artis_df):
    
                    """
                    Abone azalışlarını mevcut durumdan çıkaran fonksiyon (ilce kaldırıldı)
                    """
                    result_df = current_df.copy()
                    tuketim_yillik = abone_artis_df.loc[:, tuketim_tipleri]
                    imar_types = bina_artis_df.columns[5:]  # 5. sütundan başlar
                    
                    # Hızlı ID erişimi için sözlük oluştur
                    id_to_index = {id_val: i for i, id_val in enumerate(result_df['id'])}
                    
                    # Her abone tipi için işlem yap
                    for tuketim_tipi in tuketim_tipleri:
    
                        artis_miktari = tuketim_yillik[tuketim_tipi].values[0]
                        
                        # Sadece azalış durumunda işlem yap
                        if artis_miktari < 0:
    
                            azalis_miktari = abs(artis_miktari)
                            print(f"ELF ve SLF yi konsolide etmek için {tuketim_tipi}'ne dair {azalis_miktari} adet abone sayısı azaltılıyor...") 
                            
                            # Tüm hücreler
                            all_cells = result_df
                            
                            # Bina artışı olmayan hücreleri bul
                            no_increase_cells = []
                            for cell_id in all_cells['id'].values:
                                cell_artis = bina_artis_df[bina_artis_df['id'] == cell_id]
                                
                                if not cell_artis.empty:
                                    has_increase = False
                                    for imar_type in imar_types:
                                        if cell_artis[imar_type].sum() > 0:
                                            has_increase = True
                                            break
                                    
                                    if not has_increase and cell_id in id_to_index:
                                        no_increase_cells.append(cell_id)
                            
                            # Rastgele karıştır
                            np.random.shuffle(no_increase_cells)
                            
                            # Kalan azaltılacak miktar
                            remaining_decrease = azalis_miktari
                            
                            # Önce bina artışı olmayan hücrelerden azalt
                            for cell_id in no_increase_cells:
                                if remaining_decrease <= 0:
                                    break
                                    
                                idx = id_to_index[cell_id]
                                current_value = result_df.iat[idx, result_df.columns.get_loc(tuketim_tipi)]
                                
                                if current_value > 0:
                                    decrease_amount = min(current_value, remaining_decrease)
                                    result_df.iat[idx, result_df.columns.get_loc(tuketim_tipi)] -= decrease_amount
                                    remaining_decrease -= decrease_amount
                                    print(f"{decrease_amount} adet abone sayısı, {cell_id} id'ye sahip hücreden {tuketim_tipi} bazında azaltıldı. Hücrede kalan ilgili tipte abone sayısı: {remaining_decrease}") 
                            
                            # Eğer hala azaltılacak miktar kaldıysa, diğer hücrelerden de azalt
                            if remaining_decrease > 0:
                                other_cells = [cell_id for cell_id in all_cells['id'].values if cell_id not in no_increase_cells and cell_id in id_to_index]
                                np.random.shuffle(other_cells)
                                
                                for cell_id in other_cells:
                                    if remaining_decrease <= 0:
                                        break
                                        
                                    idx = id_to_index[cell_id]
                                    current_value = result_df.iat[idx, result_df.columns.get_loc(tuketim_tipi)]
                                    
                                    if current_value > 0:
                                        decrease_amount = min(current_value, remaining_decrease)
                                        result_df.iat[idx, result_df.columns.get_loc(tuketim_tipi)] -= decrease_amount
                                        remaining_decrease -= decrease_amount
                                        print(f"{decrease_amount} adet abone sayısı {cell_id} hücresi için {tuketim_tipi} bazında azaltıldı. Remaining: {remaining_decrease}")  # Debug
                    
                    print(f"Azalış sonrası toplamlar: {result_df[tuketim_tipleri].sum().to_dict()}")  # Debug
                    return result_df
    
                # Ana fonksiyon: Kümülatif hesaplama
                def calculate_cumulative_distribution():
    
                    print("Elektrik yükü dağıtımı kümülatif hesaplama başlıyor...")
                    
                    # As-is durumu yükle
                    try:
                        as_is_df = pd.read_excel(as_is_path)
                        print(f"As-is durumu yüklendi. Toplam {len(as_is_df)} hücre var.")
                        print(f"As-is toplamlar: {as_is_df[tuketim_tipleri].sum().to_dict()}")  # Debug
                    except Exception as e:
                        print(f"As-is dosyası yüklenirken hata oluştu: {e}")
                        return
                    
                    # Ağırlık bilgilerini yükle
                    try:
                        min_max_df = pd.read_excel(min_max_path)
                        print("Ağırlık bilgileri yüklendi.")
                        print(f"min_max_df kategorileri: {min_max_df['Category'].unique()}")  # Debug
                    except Exception as e:
                        print(f"Ağırlık bilgileri yüklenirken hata oluştu: {e}")
                        return
                    
                    # Sonuçları yazmak için Excel writer oluştur
                    output_writer = pd.ExcelWriter(output_path, engine='xlsxwriter')
                    
                    # Yıl aralığı
                    num_simulations = 25 
                    
                    # Mevcut durumu sakla (kümülatif hesaplama için)
                    current_distribution = as_is_df.copy()
                    
                    # İlk olarak as-is durumu kaydet
                    current_distribution.to_excel(output_writer, sheet_name="As-is", index=False)
                    print("As-is durumu Excel'e kaydedildi.")
                    
                    # Her yıl için hesapla
                    for year in range(start_year, end_year + 1):
                        
                        print(f"İşleniyor: {year} yılı")
                        
                        try:
                            # O yıla ait verileri SQLite veritabanından yükle
                            conn = sqlite3.connect(output_db_path)
                            bina_artis_df = pd.read_sql(f"SELECT * FROM 'new_buildings_{year}'", conn)
                            conn.close()
                            
                            # Debug: Check for non-zero cells in bina_artis_df
                            for imar_type in bina_artis_df.columns[5:]:
                                non_zero_count = (bina_artis_df[imar_type] > 0).sum()
                                print(f"{imar_type} tipinde 0 dan fazla olan bina artış sayıları, {year} yılı için: {non_zero_count}")
                            
                            # Load abone_artis_df for the current year
                            abone_artis_df = pd.read_excel(abone_artis_path, sheet_name=str(year))
                            
                            # 1. ADIM: Artışları Monte Carlo ile dağıt
                            artis_dagilimi = distribute_electric_load(
                                min_max_df, bina_artis_df, abone_artis_df, num_simulations
                            )
    
                            # 2. ADIM: Artışları mevcut dağılıma ekle
                            # Merge in the deltas under *_delta names
                            delta_cols = {t: f"{t}_delta" for t in tuketim_tipleri}
                            artis_delta = artis_dagilimi.rename(columns=delta_cols)[
                                ['id'] + list(delta_cols.values())
                            ]
    
                            # Merge on id only
                            merged = current_distribution.merge(
                                artis_delta,
                                on='id',
                                how='left'
                            )
    
                            # Fill missing deltas with zero
                            for dc in delta_cols.values():
                                merged[dc] = merged[dc].fillna(0)
    
                            # Apply the deltas
                            for t in tuketim_tipleri:
                                merged[t] += merged[f"{t}_delta"]
    
                            # Drop the temporary delta columns
                            merged.drop(columns=list(delta_cols.values()), inplace=True)
    
                            current_distribution = merged
    
                            # 3. ADIM: Azalışları uygula
                            current_distribution = apply_decrease(
                                current_distribution, abone_artis_df, bina_artis_df
                            )
    
                            # 4. ADIM: Sonuçları yaz
                            current_distribution.to_excel(output_writer, sheet_name=str(year), index=False)
                            print(f"  → {year} kaydedildi.\n")
                            
                        except Exception as e:
                            print(f"{year} yılı işlenirken hata oluştu: {e}")
                            continue
                    
                    # Excel dosyasını kapat
                    output_writer.close()
                    print("Elektrik yükü kümülatif dağıtımı tamamlandı.")
    
                # Programı çalıştır ve kümülatif abone sayılarını hesapla
                calculate_cumulative_distribution()
    
            abone_sayısı_tahmini()
    
            # Set success status
            status["ExitCode"] = 0
            status["Output"] = stdout_capture.output.getvalue()
            status["Error"] = stderr_capture.getvalue()
    
    except Exception as e:
    
        status["Error"] = f"{str(e)}\n{stderr_capture.getvalue()}"
        status["Output"] = stdout_capture.output.getvalue()
    
    finally:
        
        # Write status to JSON file
        try:
            with open(temp_status_file, 'w', encoding='utf-8') as f:
                json.dump(status, f, ensure_ascii=False, indent=2)
        except Exception as e:
            print(f"Error writing status file: {str(e)}")
        # Close StringIO objects
        stdout_capture.output.close()
        stderr_capture.close()

