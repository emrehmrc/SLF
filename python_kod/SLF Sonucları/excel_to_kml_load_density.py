import pandas as pd
import geopandas as gpd
import os
from pathlib import Path

def excel_to_single_kml(excel_file_path):
    # Define the output directory for the KML file
    kml_output_dir = "../../Excel Files/SLF Sonuçları/KML File"
    if not os.path.exists(kml_output_dir):
        os.makedirs(kml_output_dir)

    try:
        # Read the Excel file
        excel_file = pd.ExcelFile(excel_file_path)
        
        # Get all sheet names
        sheet_names = excel_file.sheet_names
        
        if not sheet_names:
            print("No sheets found in the Excel file.")
            return
        
        # Define the columns that are identical across sheets
        common_columns = ['id', 'left', 'top', 'right', 'bottom', 'geometry', 'Hücre İçi Yerleşim Alanı']
        
        # Define the columns that will be prefixed with sheet names
        variable_columns = ['Mesken', 'Sanayi', 'Ticarethane', 'Tarımsal Sulama', 'Aydınlatma', 'TOPLAM_YÜK', 'Yük_Yoğunluğu']
        
        # Define data types
        float_columns = ['left', 'top', 'right', 'bottom', 'Hücre İçi Yerleşim Alanı', 
                         'Mesken', 'Sanayi', 'Ticarethane', 'Tarımsal Sulama', 'Aydınlatma', 
                         'TOPLAM_YÜK', 'Yük_Yoğunluğu']
        int_columns = ['id']
        
        # Initialize a dictionary to store data
        combined_data = {}
        
        # Process each sheet
        for sheet_name in sheet_names:
            # Read the sheet into a DataFrame
            df = pd.read_excel(excel_file, sheet_name=sheet_name)
            
            # Verify that required columns exist
            if not all(col in df.columns for col in common_columns + variable_columns):
                print(f"Skipping sheet {sheet_name}: Missing required columns")
                continue
            
            # On the first sheet, store the common columns
            if not combined_data:
                for col in common_columns:
                    combined_data[col] = df[col]
            
            # Add variable columns with sheet name prefix
            for col in variable_columns:
                new_col_name = f"{col}_{sheet_name}"
                combined_data[new_col_name] = df[col].astype('float64')
        
        # Create a DataFrame from the combined data
        final_df = pd.DataFrame(combined_data)
        
        # Ensure correct data types
        for col in float_columns:
            if col in final_df.columns:
                final_df[col] = final_df[col].astype('float64')
        
        for col in int_columns:
            if col in final_df.columns:
                final_df[col] = final_df[col].astype('int64')
        
        # Check if 'geometry' column exists and convert to GeoDataFrame
        if 'geometry' not in final_df.columns:
            print("No 'geometry' column found in the data, cannot create KML.")
            return
        
        # Convert to GeoDataFrame using WKT geometry
        gdf = gpd.GeoDataFrame(
            final_df,
            geometry=gpd.GeoSeries.from_wkt(final_df['geometry'])
        )
        
        # Drop the 'geometry' column from the attributes (but keep the geometry itself)
        gdf = gdf.drop(columns=['geometry'])
        
        # Set CRS to WGS84 (EPSG:4326), using the older method for compatibility
        gdf.crs = "EPSG:4326"
        
        # Convert string columns to UTF-8 for KML compatibility
        for col in gdf.columns:
            if gdf[col].dtype == 'object':
                gdf[col] = gdf[col].apply(
                    lambda x: x.encode('cp1254', errors='replace').decode('utf-8', errors='replace') 
                    if pd.notna(x) else x
                )
        
        # Sort columns for consistent output
        # Common columns (excluding geometry) first, then variable columns sorted by name
        common_columns_without_geometry = [col for col in common_columns if col != 'geometry']
        sorted_columns = common_columns_without_geometry + sorted(
            [col for col in gdf.columns if col not in common_columns_without_geometry]
        )
        gdf = gdf[sorted_columns]
        
        # Define the output KML file path
        excel_filename = Path(excel_file_path).stem
        kml_filename = f"{excel_filename}_combined.kml"
        kml_output_path = os.path.join(kml_output_dir, kml_filename)
        
        # Save as a single KML file
        gdf.to_file(
            kml_output_path,
            driver='KML',
            encoding='utf-8'
        )
        
        print(f"Created single KML file: {kml_output_path}")
        print(f"Columns in KML: {list(gdf.columns)}")
        # Print data types to verify
        print("Column data types:")
        for col in gdf.columns:
            print(f"{col}: {gdf[col].dtype}")
        
        # Print first few rows to verify data
        print("\nFirst few rows of the GeoDataFrame:")
        print(gdf.head())
        
    except Exception as e:
        print(f"An error occurred: {str(e)}")

if __name__ == "__main__":
    excel_file = "../../Excel Files/SLF Sonuçları/SONUCLAR.xlsx"
    excel_to_single_kml(excel_file)