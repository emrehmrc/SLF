#!/usr/bin/env python
# -*- coding: utf-8 -*-

"""
SLF (Saturation Load Flow) Analysis Main Script
This script serves as the entry point for the SLF analysis pipeline.
It receives arguments from the C# PythonHelper class and orchestrates the analysis process.
"""

import os
import sys
import pandas as pd
import numpy as np
import argparse
from datetime import datetime
import sys
import io
import pdb  # pdb modülünü import ediyoruz
# Fix console encoding for Turkish characters
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

# Import the SLF analysis modules
try:
    
    from Saturasyon.update_saturation import run_saturation_updates, cluster_with_dbscan_and_split, update_saturation
    from Imar_orani_tahminleri.imar_orani_kod import tahmin_et, preprocess_zoning_data
    from bina_tip_analiz.bina_kodu import run_building_calculation
except ImportError:
    # If importing fails, add the current directory to the path
    sys.path.append(os.path.dirname(os.path.abspath(__file__)))
    try:
        from Saturasyon.update_saturation import run_saturation_updates, cluster_with_dbscan_and_split, update_saturation
        from Imar_orani_tahminleri.imar_orani_kod import tahmin_et, preprocess_zoning_data
        from bina_tip_analiz.bina_kodu import run_building_calculation  # Import bina modülü
    except ImportError:
        print("Modül içe aktarma hatasi: İlgili modüller bulunamadı.")
        print("İmar oranı tahmini modülü yüklenmedi. Sadece saturasyon analizi yapılacak.")
        HAS_IMAR_MODULE = False
    else:
        HAS_IMAR_MODULE = True
else:
    HAS_IMAR_MODULE = True

def parse_arguments():
    """Parse command line arguments passed from C# PythonHelper."""
    parser = argparse.ArgumentParser(description='SLF Analysis Pipeline')
    
    # Required arguments as defined in PythonHelper.RunSLFModel
    parser.add_argument('saturasyon_file', type=str, help='Path to the saturation file')
    parser.add_argument('city', type=str, help='Selected city name')
    parser.add_argument('district', type=str, help='Selected district name')
    parser.add_argument('output_dir', type=str, help='Output directory path')
    
    # Optional arguments
    parser.add_argument('--start-year', type=int, default=2024, help='Start year for analysis')
    parser.add_argument('--end-year', type=int, default=2035, help='End year for analysis')
    parser.add_argument('--dynamic-eps', type=int, default=350, help='Initial dynamic EPS value')
    
    # İmar oranı ve bina hesaplama için dosyalar
    parser.add_argument('--imar-orani-file', type=str, required=True, help='Path to the imar orani file')
    parser.add_argument('--imar-stats-file', type=str, required=True, help='Path to the construction areas (imar stats) file')
    
    parser.add_argument('--skip-imar-analizi', action='store_true', help='Skip imar analysis step')
    parser.add_argument('--skip-bina-hesap', action='store_true', help='Skip building calculation step')
    
    return parser.parse_args()

def main():
    """Main function to run the SLF analysis pipeline."""
    print("Starting SLF Analysis...")
    
    # Parse command line arguments
    args = parse_arguments()
    
    # Log the arguments
    print(f"Arguments received:")
    print(f"  Saturation File: {args.saturasyon_file}")
    print(f"  City: {args.city}")
    print(f"  District: {args.district}")
    print(f"  Output Directory: {args.output_dir}")
    print(f"  Analysis Period: {args.start_year} - {args.end_year}")
    if args.imar_orani_file:
        print(f"  İmar Oranı File: {args.imar_orani_file}")
    if args.imar_stats_file:
        print(f"  İmar Stats File: {args.imar_stats_file}")
    
    # Normalize file paths to absolute paths
    saturasyon_file_abs = os.path.abspath(args.saturasyon_file)
    output_dir_abs = os.path.abspath(args.output_dir)
    
    print(f"Absolute paths:")
    print(f"  Saturation File: {saturasyon_file_abs}")
    print(f"  Output Directory: {output_dir_abs}")
    
    # Check if the saturation file exists in the specified path
    if not os.path.exists(saturasyon_file_abs):
        # Try looking in the output directory
        alt_path = os.path.join(output_dir_abs, os.path.basename(saturasyon_file_abs))
        print(f"Saturation file not found at primary path. Trying alternative path: {alt_path}")
        
        if os.path.exists(alt_path):
            print(f"Saturation file found at alternative path!")
            saturasyon_file_abs = alt_path
        else:
            # Try looking in the current directory
            current_dir_path = os.path.join(os.getcwd(), os.path.basename(saturasyon_file_abs))
            print(f"Trying current directory path: {current_dir_path}")
            
            if os.path.exists(current_dir_path):
                print(f"Saturation file found in current directory!")
                saturasyon_file_abs = current_dir_path
            else:
                raise FileNotFoundError(f"Saturation file not found in any location. Tried:\n"
                                        f"1. {args.saturasyon_file}\n"
                                        f"2. {alt_path}\n"
                                        f"3. {current_dir_path}")
    
    print(f"Using saturation file: {saturasyon_file_abs}")
    
    # Check if the output directory exists, create if not
    if not os.path.exists(output_dir_abs):
        os.makedirs(output_dir_abs)
        print(f"Created output directory: {output_dir_abs}")
    
    # Load the saturation data
    print(f"Loading saturation data from file: {saturasyon_file_abs}")
    try:
        # Determine file type based on extension
        file_ext = os.path.splitext(saturasyon_file_abs)[1].lower()
        if file_ext == '.csv':
            builtup_df = pd.read_csv(saturasyon_file_abs)
        elif file_ext in ['.xlsx', '.xls']:
            builtup_df = pd.read_excel(saturasyon_file_abs, sheet_name='builtup')
        else:
            raise ValueError(f"Unsupported file format: {file_ext}")
        
        print(f"Loaded data with {len(builtup_df)} rows and {len(builtup_df.columns)} columns")
        
        # Run the saturation updates
        print(f"Running saturation updates for years {args.start_year} to {args.end_year}...")
        builtup_df = run_saturation_updates(
            builtup_df, 
            start_year=args.start_year, 
            end_year=args.end_year, 
            initial_dynamic_eps=args.dynamic_eps
        )
        
        # Generate output file name for saturation results
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        saturation_output_filename = f"slf_results_{args.city}_{args.district}_{timestamp}.xlsx"
        saturation_output_path = os.path.join(args.output_dir, saturation_output_filename)
        
        # Save the saturation results
        print(f"Saving saturation results to: {saturation_output_path}")
        builtup_df.to_excel(saturation_output_path, index=False)
        
        # Create a summary CSV file if needed
        summary_df = create_summary(builtup_df, args.start_year, args.end_year)
        summary_filename = f"slf_summary_{args.city}_{args.district}_{timestamp}.csv"
        summary_path = os.path.join(args.output_dir, summary_filename)
        summary_df.to_csv(summary_path, index=False)
        
        print("Saturation Analysis completed successfully!")
        print(f"Results saved to:")
        print(f"  - Full data: {saturation_output_path}")
        print(f"  - Summary: {summary_path}")
        
        # Run İmar Oranı Analysis if module exists and not skipped
        if HAS_IMAR_MODULE and not args.skip_imar_analizi:
            if args.imar_orani_file:
                imar_orani_file_abs = os.path.abspath(args.imar_orani_file)
                
                if not os.path.exists(imar_orani_file_abs):
                    print(f"İmar oranı dosyası bulunamadı: {imar_orani_file_abs}")
                    print("İmar oranı analizi atlanıyor.")
                else:
                    print("\n--- Starting İmar Oranı Analysis ---")
                    print(f"Using İmar Oranı file: {imar_orani_file_abs}")
                    
                    try:
                        # Load the zoning (imar orani) file into a DataFrame
                        zoning_file_ext = os.path.splitext(imar_orani_file_abs)[1].lower()
                        if zoning_file_ext == '.csv':
                            zoning_df = pd.read_csv(imar_orani_file_abs)
                        elif zoning_file_ext in ['.xlsx', '.xls']:
                            zoning_df = pd.read_excel(imar_orani_file_abs)
                        else:
                            raise ValueError(f"Unsupported zoning file format: {zoning_file_ext}")
                        
                        print(f"Loaded zoning data with {len(zoning_df)} rows and {len(zoning_df.columns)} columns")
                        
                        # Preprocess zoning data
                        processed_zoning_df = preprocess_zoning_data(zoning_df)
                        
                        # Generate output file name for imar orani results
                        imar_output_filename = f"imar_orani_tahmin_{args.city}_{args.district}_{timestamp}.xlsx"
                        imar_output_dir = os.path.join(args.output_dir, "imar_orani")
                        
                        # Create imar output directory if not exists
                        if not os.path.exists(imar_output_dir):
                            os.makedirs(imar_output_dir)
                        
                        imar_output_path = os.path.join(imar_output_dir, imar_output_filename)
                        
                        print(f"Running İmar Oranı analysis with saturation data: {saturation_output_path}")
                        # Call the tahmin_et function with the proper parameters
                        tahmin_et(
                            builtup_df=builtup_df,      # Pass the DataFrame
                            zoning_df=processed_zoning_df,  # Pass the processed zoning DataFrame
                            output_path=imar_output_path,
                            start_year=args.start_year,
                            end_year=args.end_year
                        )
                        
                        print("İmar Oranı Analysis completed successfully!")
                        print(f"Results saved to: {imar_output_path}")
                        
                        # Run Bina Sayısı (Building Count) Analysis if not skipped
                        if not args.skip_bina_hesap and args.imar_stats_file:
                            print("\n--- Starting Bina Sayısı (Building Count) Analysis ---")
                            
                            # Load construction areas (imar stats)
                            imar_stats_file_abs = os.path.abspath(args.imar_stats_file)
                            print(f"Checking imar stats file existence: {imar_stats_file_abs}")
                            print(f"File exists: {os.path.exists(imar_stats_file_abs)}")
                            
                            # Check for alternative paths if the file doesn't exist
                            if not os.path.exists(imar_stats_file_abs):
                                print(f"Looking for imar stats file in alternative locations...")
                                # Try alternative file name patterns or locations
                                alt_imar_stats_paths = [
                                    os.path.join(os.path.dirname(imar_stats_file_abs), 'imar_tipi_ozet.xlsx'),
                                    os.path.join(os.path.dirname(imar_stats_file_abs), 'imar_stats.xlsx'),
                                    os.path.join(args.output_dir, 'imar_tipi_ozet_tablo.xlsx')
                                ]
                                
                                for alt_path in alt_imar_stats_paths:
                                    print(f"Trying alternative path: {alt_path}")
                                    if os.path.exists(alt_path):
                                        print(f"Found imar stats file at alternative path: {alt_path}")
                                        imar_stats_file_abs = alt_path
                                        break
                            
                            if os.path.exists(imar_stats_file_abs):
                                print(f"Loading construction areas from: {imar_stats_file_abs}")
                                
                                # Debug point 1: Dosya okuma öncesi
                                print("[DEBUG] İmar stats dosyasını okumadan önce set_trace() - sheets kontrol edilecek")
                                pdb.set_trace()  # İLK DEBUG NOKTASI
                                
                                try:
                                    # Önce Excel dosyasının tüm sheet'lerini kontrol et
                                    xls = pd.ExcelFile(imar_stats_file_abs)
                                    print(f"Excel dosyasındaki sheets: {xls.sheet_names}")
                                    
                                    if 'areas' in xls.sheet_names:
                                        construction_areas_df = pd.read_excel(imar_stats_file_abs, sheet_name='areas')
                                    else:
                                        # Alternatif sheet isimlerini dene
                                        alternative_sheet_names = ['alan', 'Areas', 'imar_alanlari', 'alanlar']
                                        found_sheet = False
                                        
                                        for sheet_name in alternative_sheet_names:
                                            if sheet_name in xls.sheet_names:
                                                print(f"Alternatif sheet kullanılıyor: {sheet_name}")
                                                construction_areas_df = pd.read_excel(imar_stats_file_abs, sheet_name=sheet_name)
                                                found_sheet = True
                                                break
                                        
                                        if not found_sheet:
                                            # İlk sheet'i kullan
                                            first_sheet = xls.sheet_names[0]
                                            print(f"Hiçbir bilinen sheet adı bulunamadı. İlk sheet kullanılıyor: {first_sheet}")
                                            construction_areas_df = pd.read_excel(imar_stats_file_abs, sheet_name=first_sheet)
                                    
                                    print(f"Construction areas DataFrame:")
                                    print(construction_areas_df.head())
                                    print(f"DataFrame columns: {construction_areas_df.columns.tolist()}")
                                    
                                    # Debug point 2: DataFrame yapısını kontrol et
                                    print("[DEBUG] DataFrame yapısını incelemek için set_trace()")
                                    pdb.set_trace()  # İKİNCİ DEBUG NOKTASI
                                    
                                    # Gerekli sütunları kontrol et ve dönüştür
                                    required_columns = ['imar_tipi', 'bina_başı_brüt_alan']
                                    alternative_columns = {
                                    'imar_tipi': ['imar_type', 'tipi', 'zone_type', 'bina_tipi'],
                                    'bina_başı_brüt_alan': ['brut_alan', 'gross_area', 'bina_alani', 'building_area', 'bina_basi_brut_alan']
                                }

                                    
                                    # Sütun isimlerini normalleştir
                                    renamed_columns = {}
                                    
                                    for req_col in required_columns:
                                        if req_col in construction_areas_df.columns:
                                            continue
                                        else:
                                            # Alternatif sütun isimlerini kontrol et
                                            for alt_col in alternative_columns[req_col]:
                                                if alt_col in construction_areas_df.columns:
                                                    renamed_columns[alt_col] = req_col
                                                    break
                                    
                                    if renamed_columns:
                                        print(f"Sütun isimleri yeniden adlandırılıyor: {renamed_columns}")
                                        construction_areas_df = construction_areas_df.rename(columns=renamed_columns)
                                    
                                    # Gerekli sütunların varlığını son kez kontrol et
                                    missing_columns = [col for col in required_columns if col not in construction_areas_df.columns]
                                    if missing_columns:
                                        raise ValueError(f"Gerekli sütunlar eksik: {missing_columns}")
                                    
                                    construction_areas_dict = construction_areas_df.set_index('imar_tipi')['bina_başı_brüt_alan'].to_dict()
                                    
                                    # Debug point 3: Oluşturulan sözlüğü kontrol et
                                    print("[DEBUG] Oluşturulan construction_areas_dict:")
                                    print(construction_areas_dict)
                                    print("[DEBUG] run_building_calculation çağrısı öncesi set_trace()")
                                    pdb.set_trace()  # ÜÇÜNCÜ DEBUG NOKTASI
                                    
                                    # Generate output file names for building calculation
                                    building_output_filename = f"bina_sayisi_hesaplama_{args.city}_{args.district}_{timestamp}.xlsx"
                                    new_buildings_output_filename = f"new_buildings_{args.city}_{args.district}_{timestamp}.xlsx"
                                    
                                    building_output_dir = os.path.join(args.output_dir, "bina_sayisi")
                                    
                                    # Create bina output directory if not exists
                                    if not os.path.exists(building_output_dir):
                                        os.makedirs(building_output_dir)
                                    
                                    building_output_path = os.path.join(building_output_dir, building_output_filename)
                                    new_buildings_output_path = os.path.join(building_output_dir, new_buildings_output_filename)

                                    # Run building calculation
                                    print("[DEBUG] Run building calculation çağrısı yapılıyor...")
                                    run_building_calculation(
                                        builtup_df=builtup_df,           # Saturasyon verisi
                                        zoning_df=zoning_df,             # İmar oranı verisi (preprocess edilmemiş)
                                        imar_ratios_path=imar_output_path,  # İmar oranı tahmin sonuçları
                                        construction_areas=construction_areas_dict,
                                        output_path=building_output_path,
                                        new_buildings_output_path=new_buildings_output_path,
                                        start_year=args.start_year,
                                        end_year=args.end_year
                                    )
                                    
                                    print("Bina Sayısı Analysis completed successfully!")
                                    print(f"Results saved to:")
                                    print(f"  - Total buildings: {building_output_path}")
                                    print(f"  - New buildings: {new_buildings_output_path}")
                                    
                                except Exception as e:
                                    print(f"İmar stats dosyası okuma veya işleme hatası: {str(e)}")
                                    print("[DEBUG] Hata durumunda set_trace() - hata inceleme")
                                    pdb.set_trace()  # HATA DURUMUNDA DEBUG NOKTASI
                                    import traceback
                                    traceback.print_exc()
                                    print("Bina sayısı analizi atlanıyor.")
                            else:
                                print(f"İmar stats dosyası bulunamadı: {args.imar_stats_file}")
                                print("Bina sayısı analizi atlanıyor.")
                        
                    except Exception as e:
                        print(f"Error in İmar Oranı analysis: {str(e)}")
                        import traceback
                        traceback.print_exc()
                        print("Continuing with other steps...")
            else:
                print("\nİmar oranı dosyası belirtilmediği için İmar Oranı analizi atlanıyor.")
        
        print("\nSLF Analysis pipeline completed successfully!")
        return 0
    
    except Exception as e:
        print(f"Error in SLF analysis: {str(e)}")
        # Raise the exception to be caught by the C# code
        raise

def create_summary(df, start_year, end_year):
    """Create a summary dataframe with key statistics for each year."""
    summary_data = []
    
    for year in range(start_year, end_year + 1):
        saturation_col = f'Saturation_updated_{year}'
        
        # Check if the column exists
        if saturation_col in df.columns:
            year_stats = {
                'Year': year,
                'Average_Saturation': df[saturation_col].mean(),
                'Max_Saturation': df[saturation_col].max(),
                'Cells_Over_50_Percent': len(df[df[saturation_col] > 0.5]),
                'Cells_Over_80_Percent': len(df[df[saturation_col] > 0.8])
            }
            
            # Add development area statistics if the column exists
            if 'IsDevelopmentArea' in df.columns:
                year_stats.update({
                    'Kentsel_Yerlesim_Count': len(df[df['IsDevelopmentArea'] == 'Kentsel Yerleşim Alanı']),
                    'Gecici_Kentsel_Count': len(df[df['IsDevelopmentArea'] == 'Geçici Kentsel Alan']),
                    'Imarli_Genisleme_Count': len(df[df['IsDevelopmentArea'] == 'İmarlı Yeni Genişleme Bölgesi']),
                    'Imarsiz_Genisleme_Count': len(df[df['IsDevelopmentArea'] == 'İmarsız Yeni Genişleme Bölgesi'])
                })
            
            summary_data.append(year_stats)
    
    return pd.DataFrame(summary_data)

if __name__ == "__main__":
    sys.exit(main())