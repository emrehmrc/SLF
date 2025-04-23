import geopandas as gpd
import pandas as pd
from sqlalchemy import create_engine
from shapely.geometry import Point, Polygon, box
from shapely.wkt import loads
from pyproj import Geod
import os

# Veritabanı bağlantısı (sadece İzmir için)
DB_CONNECTION_IZMIR = "postgresql://postgres:12345@localhost:5432/gdz"

# Sınıf isimleri
categories = [
    "school", "hospital", "university", "clinic", "college", "cafe", "library",
    "shop", "courthouse", "restaurant", "fuel", "bus_station", "industrial",
    "supermarket", "mall", "car", "educational_institution", "government",
    "company", "political_party", "financial", "energy_supplier", "apartments",
    "house", "train_station", "commercial", "office", "stadium", "garages",
    "apartment", "hotel", "museum"
]

# Öncelik sırasına göre sütunlar
columns_priority = ["leisure", "healthcare", "tourism", "building", "office", "shop", "landuse", "amenity"]

def calculate_geodetic_area(geometry):
    geod = Geod(ellps="WGS84")
    return abs(geod.geometry_area_perimeter(geometry)[0])

def get_grid_data_izmir(engine):
    grid_query = """
        SELECT 
            id, "left", "bottom", "right", "top", "ilce"
        FROM "IZMIR_HUCRE"
        WHERE ilce IN ('ÇİĞLİ', 'KARŞIYAKA')
    """
    grid_data = pd.read_sql(grid_query, con=engine)
    grid_data['geometry'] = grid_data.apply(
        lambda row: box(row['left'], row['bottom'], row['right'], row['top']), axis=1
    )
    return gpd.GeoDataFrame(grid_data, geometry='geometry', crs="EPSG:4326")

def get_grid_data_eskisehir():
    # Excel dosyasının tam yolunu oluştur
    current_dir = os.path.dirname(os.path.abspath(__file__))
    file_path = os.path.join(current_dir, "../girdiler/eskisehir_hucreleri.xlsx")
    
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"Excel dosyası bulunamadı: {file_path}")
    
    # Excel dosyasını oku
    grid_data = pd.read_excel(file_path)
    grid_data['geometry'] = grid_data.apply(
        lambda row: box(row['left'], row['bottom'], row['right'], row['top']), axis=1
    )
    return gpd.GeoDataFrame(grid_data, geometry='geometry', crs="EPSG:4326")
def get_overpass_data_izmir(engine):
    overpass_query = "SELECT *, ST_AsText(geometry) AS geometry_wkt FROM overpass_data"
    overpass_data = pd.read_sql(overpass_query, con=engine)
    overpass_data['geometry'] = overpass_data['geometry_wkt'].apply(
        lambda x: loads(x) if isinstance(x, str) else None
    )
    return overpass_data[overpass_data['geometry'].notna()]

def get_overpass_data_eskisehir():
    # CSV dosyasından overpass verilerini oku
    current_dir = os.path.dirname(os.path.abspath(__file__))
    file_path = os.path.join(current_dir, "../girdiler/overpass-data-Eskişehir.csv")
    overpass_data = pd.read_csv(file_path)
    # Geometry sütununu WKT formatından dönüştür
    overpass_data['geometry'] = overpass_data['geometry'].apply(
        lambda x: loads(x) if isinstance(x, str) else None
    )
    return overpass_data[overpass_data['geometry'].notna()]

def process_count_data(overpass_gdf, grid_gdf, city):
    # Noktasal veriye dönüştür
    overpass_gdf['geometry'] = overpass_gdf['geometry'].apply(
        lambda geom: geom.centroid if geom.geom_type in ['Polygon', 'MultiPolygon'] else geom
    )
    
    # Spatial join
    joined = gpd.sjoin(overpass_gdf, grid_gdf, how="inner", predicate="within")
    
    # Kategorileri belirle
    joined['category'] = None
    
    if city == 'izmir':
        for category in categories:
            for col in columns_priority:
                joined.loc[joined['category'].isna() & (joined[col] == category), 'category'] = category
    else:  # eskisehir
        # Eskişehir için building type'a göre kategori belirleme
        building_type_mapping = {
            'apartments': 'apartments',
            'house': 'house',
            'commercial': 'commercial',
            'industrial': 'industrial',
            'school': 'school',
            'office': 'office',
            'hospital': 'hospital',
            'university': 'university',
            'residential': 'house',
            'yes': 'building',
            'retail': 'commercial',
            'warehouse': 'industrial',
            'public': 'government',
            'hotel': 'hotel',
            'supermarket': 'supermarket'
        }
        
        for building_type, category in building_type_mapping.items():
            joined.loc[joined['category'].isna() & (joined['building'] == building_type), 'category'] = category
    
    # Sınıf sayısını gruplama
    summary = joined.groupby(['index_right', 'category']).size().unstack(fill_value=0).reset_index()
    
    # Eksik sınıfları sıfır ile doldur
    for category in categories:
        if category not in summary.columns:
            summary[category] = 0
            
    return grid_gdf.merge(summary, left_index=True, right_on='index_right', how='left').fillna(0)

def process_area_data(overpass_gdf, grid_gdf, city):
    # Sadece Polygon geometrileri seç
    overpass_gdf = overpass_gdf[overpass_gdf['geometry'].apply(lambda geom: geom.geom_type == 'Polygon')]
    
    # Grid hücre alanını hesapla
    grid_gdf['cell_area'] = grid_gdf['geometry'].apply(calculate_geodetic_area).round(2)
    
    # Kesişim hesapla
    intersected = gpd.overlay(overpass_gdf, grid_gdf, how='intersection')
    intersected['intersection_area_m2'] = intersected['geometry'].apply(calculate_geodetic_area).round(2)
    
    # Kategorileri belirle
    intersected['category'] = None
    
    if city == 'izmir':
        for category in categories:
            for col in columns_priority:
                intersected.loc[intersected['category'].isna() & (intersected[col] == category), 'category'] = category
    else:  # eskisehir
        # Eskişehir için building type'a göre kategori belirleme
        building_type_mapping = {
            'apartments': 'apartments',
            'house': 'house',
            'commercial': 'commercial',
            'industrial': 'industrial',
            'school': 'school',
            'office': 'office',
            'hospital': 'hospital',
            'university': 'university',
            'residential': 'house',
            'yes': 'building',
            'retail': 'commercial',
            'warehouse': 'industrial',
            'public': 'government',
            'hotel': 'hotel',
            'supermarket': 'supermarket'
        }
        
        for building_type, category in building_type_mapping.items():
            intersected.loc[intersected['category'].isna() & (intersected['building'] == building_type), 'category'] = category
    
    # Hücre başına kategori alanı hesaplama
    summary = intersected.groupby(['id', 'category'])['intersection_area_m2'].sum().unstack(fill_value=0).reset_index()
    
    # Eksik kategoriler için sıfır doldurma
    for category in categories:
        if category not in summary.columns:
            summary[category] = 0
            
    return grid_gdf[['id', 'cell_area']].merge(summary, on='id', how='left').fillna(0)

def process_all_data(city):
    city = city.strip().lower()  # Boşlukları kaldır ve küçük harfe çevir
    print(f"Şehir: {city}")
    if city not in ["izmir", "eskişehir"]:
        print("Geçersiz şehir. İzmir veya Eskişehir seçin.")
        return

    try:
        if city == "izmir":
            engine = create_engine(DB_CONNECTION_IZMIR)
            grid_gdf = get_grid_data_izmir(engine)
            overpass_data = get_overpass_data_izmir(engine)
        else:  # eskisehir
            grid_gdf = get_grid_data_eskisehir()
            overpass_data = get_overpass_data_eskisehir()
        
        overpass_gdf = gpd.GeoDataFrame(overpass_data, geometry='geometry', crs="EPSG:4326")
        
        # Adet hesaplama
        count_results = process_count_data(overpass_gdf.copy(), grid_gdf.copy(), city)
        count_results = count_results.drop(columns=['left', 'bottom', 'right', 'top', 'ilce', 'geometry', 'index_right'], errors='ignore')
        
        # Alan hesaplama
        area_results = process_area_data(overpass_gdf.copy(), grid_gdf.copy(), city)
        
        # Sonuçları kaydet
        if city == "izmir":
            with engine.begin() as connection:
                count_results.to_sql('g4_overpass_dagilim_tablosu', con=connection, if_exists="replace", index=False)
                area_results.to_sql(f'g4_{city}_overpass_m2', con=connection, if_exists="replace", index=False)
        
        # CSV dosyalarını kaydet
        current_dir = os.path.dirname(os.path.abspath(__file__))
        file_path = os.path.join(current_dir, f"../ciktilar/v4_{city}_overpass_dagilim.csv")
        file_path1 = os.path.join(current_dir,f"../ciktilar/v4_{city}_overpass_m2.csv")
        count_results.to_csv(file_path)
        area_results.to_csv(file_path1)
        
        print(f"{city.capitalize()} için işlemler başarıyla tamamlandı.")
        print(f"Dosyalar kaydedildi: v4_{city}_overpass_dagilim.csv ve g4_{city}_overpass_m2.csv")
        
    except Exception as e:
        print(f"Hata oluştu: {e}")
        # Hata ayıklama için daha detaylı bilgi
        import traceback
        print(traceback.format_exc())

# if __name__ == "__main__":
#     city = input("Hangi şehri seçmek istersiniz? (İzmir / Eskişehir): ").strip()
#     process_all_data(city)