import pandas as pd
import geopandas as gpd
import os
from pathlib import Path
import xml.etree.ElementTree as ET
from shapely.wkt import loads

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
        common_columns = ['id', 'left', 'top', 'right', 'bottom', 'geometry']
        
        # Define the columns that will be prefixed with sheet names
        variable_columns = ['Mesken', 'Sanayi', 'Ticarethane', 'Tarımsal Sulama', 'Aydınlatma', 'TOPLAM_YÜK', 'Hücre İçi Yerleşim Alanı', 'Yük_Yoğunluğu']
        
        # Define data types and rounding precision
        coordinate_columns = ['left', 'top', 'right', 'bottom']  # Round to 6 decimal places
        other_float_columns = ['id','Mesken', 'Sanayi', 'Ticarethane', 'Tarımsal Sulama', 'Aydınlatma', 
                               'TOPLAM_YÜK', 'Hücre İçi Yerleşim Alanı', 'Yük_Yoğunluğu']  # Round to 3 decimal places
        
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
                # Ensure the column is treated as float64 to preserve decimals
                combined_data[new_col_name] = df[col].astype('float64')
        
        # Create a DataFrame from the combined data
        final_df = pd.DataFrame(combined_data)
        
        # Ensure correct data types and apply rounding
        # Round coordinate columns to 6 decimal places
        for col in coordinate_columns:
            if col in final_df.columns:
                final_df[col] = final_df[col].astype('float64')
                final_df[col] = final_df[col].round(6)
        
        # Round other float columns to 3 decimal places
        for col in variable_columns:
            for sheet_name in sheet_names:
                col_name = f"{col}_{sheet_name}"
                if col_name in final_df.columns:
                    final_df[col_name] = final_df[col_name].astype('float64')
                    final_df[col_name] = final_df[col_name].round(3)
        
        
        # Check if 'geometry' column exists and convert to GeoDataFrame
        if 'geometry' not in final_df.columns:
            print("No 'geometry' column found in the data, cannot create KML.")
            return
        
        # Convert to GeoDataFrame using WKT geometry
        gdf = gpd.GeoDataFrame(
            final_df,
            geometry=gpd.GeoSeries.from_wkt(final_df['geometry'])
        )
        
        # Set CRS to WGS84 (EPSG:4326), using the older method for compatibility
        gdf.crs = "EPSG:4326"
        
        # Convert string columns to UTF-8 for KML compatibility
        for col in gdf.columns:
            if gdf[col].dtype == 'object' and col != 'geometry':
                gdf[col] = gdf[col].apply(
                    lambda x: x.encode('cp1254', errors='replace').decode('utf-8', errors='replace') 
                    if pd.notna(x) else x
                )
        
        # Sort columns for consistent output
        sorted_columns = common_columns.copy()
        for sheet_name in sheet_names:
            for var_col in variable_columns:
                sorted_columns.append(f"{var_col}_{sheet_name}")
        # Reorder the GeoDataFrame columns (excluding 'geometry' which is handled separately)
        sorted_columns = [col for col in sorted_columns if col != 'geometry'] + ['geometry']
        gdf = gdf[sorted_columns]
        
        # Define the output KML file path
        excel_filename = Path(excel_file_path).stem
        kml_filename = f"{excel_filename}_Load_Density.kml"
        kml_output_path = os.path.join(kml_output_dir, kml_filename)
        
        # Manually create the KML file
        # Define the KML namespace
        kml_ns = "http://www.opengis.net/kml/2.2"
        # Register the namespace with an empty prefix (default namespace)
        ET.register_namespace('', kml_ns)
        
        # Create the root KML element with the correct namespace
        kml_root = ET.Element('kml', xmlns=kml_ns)
        
        # Create the Document element
        document = ET.SubElement(kml_root, 'Document')
        document_id = ET.SubElement(document, 'id')
        document_id.text = 'root_doc'
        
        # Create the Schema element
        schema = ET.SubElement(document, 'Schema', name=f"{excel_filename}_combined", id=f"{excel_filename}_combined")
        # Add SimpleField for each column (except geometry)
        for col in gdf.columns:
            if col != 'geometry':
                ET.SubElement(schema, 'SimpleField', name=col, type='float')
        
        # Create the Folder element
        folder = ET.SubElement(document, 'Folder')
        folder_name = ET.SubElement(folder, 'name')
        folder_name.text = f"{excel_filename}_combined"
        
        # Add a Placemark for each row in the GeoDataFrame
        for idx, row in gdf.iterrows():
            placemark = ET.SubElement(folder, 'Placemark')
            
            # Set the <name> field (using the id value)
            name = ET.SubElement(placemark, 'name')
            name.text = str(row['id'])
            
            # Set the <description> field (using id - left)
            description = ET.SubElement(placemark, 'description')
            # 'left' is already rounded to 6 decimal places in the DataFrame
            description.text = f"{row['id']} - {row['left']}"
            
            # Add the Style element (same as in the original KML)
            style = ET.SubElement(placemark, 'Style')
            line_style = ET.SubElement(style, 'LineStyle')
            color = ET.SubElement(line_style, 'color')
            color.text = 'ff0000ff'
            poly_style = ET.SubElement(style, 'PolyStyle')
            fill = ET.SubElement(poly_style, 'fill')
            fill.text = '0'
            
            # Add the ExtendedData/SchemaData section
            extended_data = ET.SubElement(placemark, 'ExtendedData')
            schema_data = ET.SubElement(extended_data, 'SchemaData', schemaUrl=f"#{excel_filename}_combined")
            # Add SimpleData for each column (except geometry)
            for col in gdf.columns:
                if col != 'geometry':
                    simple_data = ET.SubElement(schema_data, 'SimpleData', name=col)
                    simple_data.text = str(row[col])
            
            # Add the Polygon geometry
            polygon = ET.SubElement(placemark, 'Polygon')
            outer_boundary = ET.SubElement(polygon, 'outerBoundaryIs')
            linear_ring = ET.SubElement(outer_boundary, 'LinearRing')
            coordinates = ET.SubElement(linear_ring, 'coordinates')
            # Convert the geometry to coordinates string, rounding coordinates to 6 decimal places
            geom = row['geometry']
            coords = []
            for x, y in geom.exterior.coords:  # Include all points, including the closing point
                x_rounded = round(x, 6)
                y_rounded = round(y, 6)
                coords.append(f"{x_rounded},{y_rounded}")
            coordinates.text = ' '.join(coords)
        
        # Write the KML file
        tree = ET.ElementTree(kml_root)
        tree.write(kml_output_path, encoding='utf-8', xml_declaration=True)
        
        print(f"Created KML file: {kml_output_path}")
        print(f"Columns in KML: {list(gdf.columns)}")
        # Print data types to verify
        print("Column data types:")
        for col in gdf.columns:
            print(f"{col}: {gdf[col].dtype}")
        
    except Exception as e:
        print(f"An error occurred: {str(e)}")

if __name__ == "__main__":
    excel_file = "../../Excel Files/SLF Sonuçları/SONUCLAR.xlsx"
    excel_to_single_kml(excel_file)