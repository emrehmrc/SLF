import xml.etree.ElementTree as ET
import re
import pandas as pd
import requests
import json
from shapely.geometry import Point, Polygon
import geopandas as gpd
from pyproj import Transformer
from sqlalchemy import create_engine
import os
# KML dosyasının yolunu belirtin


# overpass datası gelecek 

# area_name = input("Lütfen sorgulamak istediğiniz alanın adını girin: ")
# print(f"Sorgulanan alan: {area_name}")

# Overpass Turbo API'sini kullanarak veri çekme
# Sorgulanan alan adı
DB_CONNECTIONS = {
    "gdz": "postgresql://postgres:12345@localhost:5432/gdz"
}

def fetch_and_process_data(area_name):
    global file_name
    file_name = area_name        
    overpass_url = "http://overpass-api.de/api/interpreter"
    # Fonksiyon içinde dinamik alan adını yakalayacak şekilde sorgularınızı tanımlayın
    queries_grouped = {
     "buildings": f"""
        [out:json][timeout:180];
        area["name"="{area_name}"]->.searchArea;
        (
            way["building"](area.searchArea);
            relation["building"](area.searchArea);
        );
        (._;>;);
        out body;
        """,

    "group_1": f"""
    [out:json][timeout:180];
    area["name"="{area_name}"]->.searchArea;

    (
      node["shop"="supermarket"](area.searchArea);
      way["shop"="supermarket"](area.searchArea);
      relation["shop"="supermarket"](area.searchArea);
    
      node["shop"="convenience"](area.searchArea);
      way["shop"="convenience"](area.searchArea);
      relation["shop"="convenience"](area.searchArea);
    
      node["shop"="boutique"](area.searchArea);
      way["shop"="boutique"](area.searchArea);
      relation["shop"="boutique"](area.searchArea);

      
    );
    (._;>;);
    out body;
    """,
    
    "group_2": f"""
    [out:json][timeout:180];
    area["name"="{area_name}"]->.searchArea;

    (
      node["shop"="stationery"](area.searchArea);
      way["shop"="stationery"](area.searchArea);
      relation["shop"="stationery"](area.searchArea);
   
      node["shop"="furniture"](area.searchArea);
      way["shop"="furniture"](area.searchArea);
      relation["shop"="furniture"](area.searchArea);
    
      node["shop"="electronics"](area.searchArea);
      way["shop"="electronics"](area.searchArea);
      relation["shop"="electronics"](area.searchArea);
    );
    (._;>;);
    out body;
    """,
    
    
    "group_3": f"""
    [out:json][timeout:180];
    area["name"="{area_name}"]->.searchArea;

    (
      node["shop"="jewelry"](area.searchArea);
      way["shop"="jewelry"](area.searchArea);
      relation["shop"="jewelry"](area.searchArea);
   
      node["shop"="mobile_phone"](area.searchArea);
      way["shop"="mobile_phone"](area.searchArea);
      relation["shop"="mobile_phone"](area.searchArea);
    
      node["shop"="books"](area.searchArea);
      way["shop"="books"](area.searchArea);
      relation["shop"="books"](area.searchArea);
    );
    (._;>;);
    out body;
    """,

    "group_4": f"""
    [out:json][timeout:180];
    area["name"="{area_name}"]->.searchArea;

    (
      node["shop"="florist"](area.searchArea);
      way["shop"="florist"](area.searchArea);
      relation["shop"="florist"](area.searchArea);
    
      node["shop"="hairdresser"](area.searchArea);
      way["shop"="hairdresser"](area.searchArea);
      relation["shop"="hairdresser"](area.searchArea);
   
      node["shop"="mall"](area.searchArea);
      way["shop"="mall"](area.searchArea);
      relation["shop"="mall"](area.searchArea);
    );
    (._;>;);
    out body;
    """,

    "group_5": f"""
    [out:json][timeout:180];
    area["name"="{area_name}"]->.searchArea;

    (
      node["shop"="car_repair"](area.searchArea);
      way["shop"="car_repair"](area.searchArea);
      relation["shop"="car_repair"](area.searchArea);
   
      node["shop"="car"](area.searchArea);
      way["shop"="car"](area.searchArea);
      relation["shop"="car"](area.searchArea);
   
      node["shop"="bakery"](area.searchArea);
      way["shop"="bakery"](area.searchArea);
      relation["shop"="bakery"](area.searchArea);
    );
    (._;>;);
    out body;
    """,
    
    "group_7": f"""
    [out:json][timeout:180];
    area["name"="{area_name}"]->.searchArea;

    (
      node["amenity"="school"](area.searchArea);
      way["amenity"="school"](area.searchArea);
      relation["amenity"="school"](area.searchArea);
   
      node["amenity"="university"](area.searchArea);
      way["amenity"="university"](area.searchArea);
      relation["amenity"="university"](area.searchArea);
    
      node["amenity"="dentist"](area.searchArea);
      way["amenity"="dentist"](area.searchArea);
      relation["amenity"="dentist"](area.searchArea);
    );
    (._;>;);
    out body;
    """,

    "group_8": f"""
    [out:json][timeout:180];
    area["name"="{area_name}"]->.searchArea;

    (
      node["healthcare"="dentist"](area.searchArea);
      way["healthcare"="dentist"](area.searchArea);
      relation["healthcare"="dentist"](area.searchArea);
    
      node["amenity"="kindergarten"](area.searchArea);
      way["amenity"="kindergarten"](area.searchArea);
      relation["amenity"="kindergarten"](area.searchArea);
    
      node["amenity"="public_building"](area.searchArea);
      way["amenity"="public_building"](area.searchArea);
      relation["amenity"="public_building"](area.searchArea);
    );
    (._;>;);
    out body;
    """,

    "group_9": f"""
    [out:json][timeout:180];
    area["name"="{area_name}"]->.searchArea;

    (
      node["amenity"="townhall"](area.searchArea);
      way["amenity"="townhall"](area.searchArea);
      relation["amenity"="townhall"](area.searchArea);
    
      node["amenity"="courthouse"](area.searchArea);
      way["amenity"="courthouse"](area.searchArea);
      relation["amenity"="courthouse"](area.searchArea);
   
      node["office"](area.searchArea);
      way["office"](area.searchArea);
      relation["office"](area.searchArea);
    );
    (._;>;);
    out body;
    """,

    "group_10": f"""
    [out:json][timeout:180];
    area["name"="{area_name}"]->.searchArea;

    (
      node["building"="commercial"](area.searchArea);
      way["building"="commercial"](area.searchArea);
      relation["building"="commercial"](area.searchArea);
   
      node["amenity"="university"](area.searchArea);
      way["amenity"="university"](area.searchArea);
      relation["amenity"="university"](area.searchArea);
   
      node["amenity"="library"](area.searchArea);
      way["amenity"="library"](area.searchArea);
      relation["amenity"="library"](area.searchArea);
    );
    (._;>;);
    out body;
    """,

    "group_11": f"""
    [out:json][timeout:180];
    area["name"="{area_name}"]->.searchArea;

    (
      node["healthcare"="hospital"](area.searchArea);
      way["healthcare"="hospital"](area.searchArea);
      relation["healthcare"="hospital"](area.searchArea);
   
      node["amenity"="clinic"](area.searchArea);
      way["amenity"="clinic"](area.searchArea);
      relation["amenity"="clinic"](area.searchArea);
    
      node["amenity"="pharmacy"](area.searchArea);
      way["amenity"="pharmacy"](area.searchArea);
      relation["amenity"="pharmacy"](area.searchArea);
    );
    (._;>;);
    out body;
    """,
"group_12": f"""
[out:json][timeout:180];
area["name"="{area_name}"]->.searchArea;

(
  node["healthcare"="pharmacy"](area.searchArea);
  way["healthcare"="pharmacy"](area.searchArea);
  relation["healthcare"="pharmacy"](area.searchArea);

  node["leisure"="sports_centre"](area.searchArea);
  way["leisure"="sports_centre"](area.searchArea);
  relation["leisure"="sports_centre"](area.searchArea);

  node["leisure"="swimming_pool"](area.searchArea);
  way["leisure"="swimming_pool"](area.searchArea);
  relation["leisure"="swimming_pool"](area.searchArea);
);
(._;>;);
out body;
""",

"group_13": f"""
[out:json][timeout:180];
area["name"="{area_name}"]->.searchArea;

(
  node["leisure"="stadium"](area.searchArea);
  way["leisure"="stadium"](area.searchArea);
  relation["leisure"="stadium"](area.searchArea);

  node["amenity"="community_centre"](area.searchArea);
  way["amenity"="community_centre"](area.searchArea);
  relation["amenity"="community_centre"](area.searchArea);

  node["building"="civic"](area.searchArea);
  way["building"="civic"](area.searchArea);
  relation["building"="civic"](area.searchArea);
);
(._;>;);
out body;
""",

"group_14": f"""
[out:json][timeout:180];
area["name"="{area_name}"]->.searchArea;

(
  node["tourism"="museum"](area.searchArea);
  way["tourism"="museum"](area.searchArea);
  relation["tourism"="museum"](area.searchArea);

  node["railway"="station"](area.searchArea);
  way["railway"="station"](area.searchArea);
  relation["railway"="station"](area.searchArea);

  node["amenity"="fuel"](area.searchArea);
  way["amenity"="fuel"](area.searchArea);
  relation["amenity"="fuel"](area.searchArea);
);
(._;>;);
out body;
""",

"group_15": f"""
[out:json][timeout:180];
area["name"="{area_name}"]->.searchArea;

(
  node["building"="warehouse"](area.searchArea);
  way["building"="warehouse"](area.searchArea);
  relation["building"="warehouse"](area.searchArea);

  node["landuse"="industrial"](area.searchArea);
  way["landuse"="industrial"](area.searchArea);
  relation["landuse"="industrial"](area.searchArea);

  node["landuse"="bus_station"](area.searchArea);
  way["amenity"="bus_station"](area.searchArea);
  relation["landuse"="bus_station"](area.searchArea);
);
(._;>;);
out body;
""",

      
    }

    

    # Tüm sorguları kullanarak veri çekin
    combined_results = []
    for key, query in queries_grouped.items():
        response = requests.post(overpass_url, data={'data': query})
        if response.status_code == 200:
            try:
                data = json.loads(response.text)
                processed_data = replace_node_ids_with_objects(data)
                combined_results.extend(processed_data['elements']) # Doğru listeye eleman eklemek
            except json.JSONDecodeError:
                print(f"Invalid JSON response from query: {key}")
        else:
            print(f"Failed to fetch data for {key}: Status code {response.status_code}")

    final_data = {
        'elements': combined_results
    }
    processed_final_data = process_data(final_data) # İşlenmiş verileri almak
    return processed_final_data
 

def replace_node_ids_with_objects(data):
    node_dict = {}
    
    # Tüm node'ları node_dict sözlüğüne ekle
    for element in data["elements"]:
        
        if element["type"] == "node": 
            node_dict[element["id"]] = element
        else:
            print(f"Node eklenemedi: {element}")
    
    # 'Way' türündeki elementler için node ID'lerini node nesneleriyle değiştir
    for element in data["elements"]:
        if element["type"] == "way":
            node_list = element["nodes"]
            updated_node_list = []
            for node_id in node_list:
                if node_id in node_dict:
                    updated_node_list.append(node_dict[node_id])
                else:
                    print(f"Warning: Node ID {node_id} not found in node_dict.")
                    updated_node_list.append({"id": node_id, "lat": None, "lon": None})  # Eksik node'lar için varsayılan değerler
            element["nodes"] = updated_node_list
    return data
# Node ID'lerini node nesneleriyle değiştir
def replace_node_ids_with_objects(data):
    node_dict = {element['id']: element for element in data['elements'] if element['type'] == 'node'}
    for element in data['elements']:
        if element['type'] == 'way':
            element['nodes'] = [node_dict.get(node_id, {'id': node_id, 'lat': None, 'lon': None}) for node_id in element.get('nodes', [])]
    return data 
# Function to calculate the area of a polygon
def calculate_area_m2(coords):
    if len(coords) < 3:
        print("Error: A polygon requires at least 3 distinct coordinates.")
        return None

    if coords[0] != coords[-1]:
        coords.append(coords[0])

    try:
        # Create the polygon and transform it to UTM to calculate area
        polygon = Polygon(coords)
        transformer = Transformer.from_crs("epsg:4326", "epsg:32635", always_xy=True)
        utm_coords = [transformer.transform(lon, lat) for lon, lat in coords]
        utm_polygon = Polygon(utm_coords)
        return utm_polygon.area
    except Exception as e:
        print(f"An error occurred while calculating area: {e}")
        return None
    

# Function to get the coordinates from a 'way' element
def get_polygon_coords(element):
    if element['type'] == 'way':
        coords = [(node['lon'], node['lat']) for node in element['nodes'] if node['lon'] is not None and node['lat'] is not None]
        return coords
    return []

def createGeo(element):
    if element['type'] == 'node':
        # Create a Point geometry for nodes
        if 'lat' in element and 'lon' in element:
            return Point(element['lon'], element['lat'])
        else:
            return None  # Return None if coordinates are missing
    
    elif element['type'] == 'way':
        # Create a Polygon geometry for ways
        coords = get_polygon_coords(element)  # Use the function to get the coordinates
        if len(coords) >= 3:  # A polygon needs at least 3 points
            return Polygon(coords)
        else:
            return None  # Return None if there are not enough coordinates for a polygon

    return None  # Default return if the type is not 'node' or 'way'
   
    
# Process the data
def process_data(data):
    data_rows = []
    for element in data['elements']:
        
        coords = get_polygon_coords(element) if element['type'] == 'way' else ""

        row = {
            'name': element.get('tags', {}).get('name', ''),
            'geometry': createGeo(element),
            'amenity': element.get('tags', {}).get('amenity', ''),
            'landuse': element.get('tags', {}).get('landuse', ''),
            'shop': element.get('tags', {}).get('shop', ''),
            'office': element.get('tags', {}).get('office', ''),
            'building': element.get('tags', {}).get('building', ''),
            'tourism': element.get('tags', {}).get('tourism', ''),
            'healthcare': element.get('tags', {}).get('healthcare', ''),
            'leisure': element.get('tags', {}).get('leisure', ''),
            'levels': element.get('tags', {}).get('building:levels', ''),  # Kat sayısı bilgisi
            'coords': coords,
            'area_m2': calculate_area_m2(coords) if coords else ""
        }

        # Ortalama koordinat hesaplama (way türünde elementler için)
        if element['type'] == 'way' and coords:
            latitudes = [lat for _, lat in coords]
            longitudes = [lon for lon, _ in coords]
            row['lat'] = sum(latitudes) / len(latitudes) if latitudes else ''
            row['lon'] = sum(longitudes) / len(longitudes) if longitudes else ''

        # Eğer ilgilenilen alanlardan biri boş değilse veriyi ekle
        if any(row[key] for key in ['amenity', 'landuse', 'shop', 'office', 'building', 'tourism', 'healthcare', 'leisure', 'levels']):
            data_rows.append(row)
    
    # GeoDataFrame oluşturma
    df = gpd.GeoDataFrame(data_rows)
    df = df.drop_duplicates(subset=['name', 'geometry'], keep='first')
    # Veriyi kaydetme
    save_to_database(df, file_name)
    file_path = os.path.join("database_kodlari", "overpass_tablolari", "girdiler", f"overpass-data-{file_name}.csv")
    # df.to_csv(file_path, index=False) 
    return df


def save_to_database(gdf, area_name, table_name="overpass_data"):
    try:
        # `area_name`'e göre şehir seçimi
        # area_name_lower = area_name.lower()
        if "İzmir" in area_name:
            db_choice = "gdz"
        elif "eskişehir" in area_name or "Eskişehir" in area_name:
            return 
        else:
            raise ValueError(f"Geçersiz şehir bilgisi! Beklenen: 'İzmir' veya 'Eskişehir'. Gelen: '{area_name}'")

        # Veritabanı bağlantısı oluştur
        engine = create_engine(DB_CONNECTIONS[db_choice])
        print(f"Veritabanı bağlantısı kuruldu: {db_choice}")
        
        # Veriyi veritabanına kaydet
        gdf.to_postgis(table_name, engine, if_exists="replace", index=False)
        print(f"Veri başarıyla '{table_name}' tablosuna kaydedildi ({db_choice}).")
    except Exception as e:
        print(f"Veritabanına kaydetme sırasında hata oluştu: {e}")
        raise

def xml_safe_text(text):
    return text.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace("\"", "&quot;").replace("'", "&apos;")


