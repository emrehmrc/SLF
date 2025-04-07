import geopandas as gpd
import xml.etree.ElementTree as ET
from shapely.geometry import Point, Polygon, LineString
import re
import pandas as pd
from shapely.validation import make_valid
from sqlalchemy import create_engine
from sqlalchemy.sql import text
import sys
import os
current_dir = os.getcwd()
parent_dir = os.path.dirname(current_dir)
# Şunu kullanın:
from database_kodlari.imar_kirilimlari.kod.kod_imar_kesisim_imar_tipi_v2 import process_city_data

DB_CONNECTIONS = {
    "gdz": "postgresql://postgres:12345@localhost:5432/gdz",  # İzmir için
    "oedas": "postgresql://postgres:12345@localhost:5432/oedas"  # Eskişehir için
}

# Tablo adları
TABLE_NAMES = {
    "İzmir": "IZMIR_IMAR_PL",
    "Eskişehir": "ESKISEHIR_IMAR_PL"
}
def save_to_db_with_dynamic_table(geo_df, region):
    """
    GeoDataFrame'i belirtilen şehir için uygun tabloya kaydeder veya günceller. 
    Eğer tablo mevcut değilse, oluşturur.
    """
    try:
        # Hangi veritabanına kaydedileceğini belirle
        db_choice = "gdz" if "İzmir" in region else "oedas" if "Eskişehir" in region else None
        if not db_choice:
            raise ValueError("Bilinmeyen bölge: 'İzmir' veya 'Eskişehir' bekleniyor.")

        # Tablo adını belirle
        table_name = TABLE_NAMES.get(region)
        if not table_name:
            raise ValueError("Tablo adı belirlenemedi. Bölge kontrol edin.")

        # Veritabanı bağlantısını oluştur
        engine = create_engine(DB_CONNECTIONS[db_choice])

        # CRS doğrulama ve dönüştürme
        if geo_df.crs is None:
            geo_df.set_crs("EPSG:4326", inplace=True)

        # Tablo kontrolü
        table_exists_query = text(f"""
        SELECT EXISTS (
            SELECT FROM information_schema.tables 
            WHERE table_schema = 'public' 
            AND table_name = '{table_name.lower()}'
        );
        """)

        with engine.connect() as conn:
            result = conn.execute(table_exists_query).scalar()

        if result:  # Tablo mevcut
            print(f"Tablo {table_name} mevcut, güncelleniyor...")
            # Mevcut verileri çek
            existing_data = gpd.read_postgis(f"SELECT * FROM {table_name}", con=engine, geom_col='geometry')

            # Yeni verileri mevcut verilerle birleştir
            merged_data = pd.concat([existing_data, geo_df]).drop_duplicates(subset=['name', 'styleUrl'])

            # Tabloyu güncelle
            merged_data.to_postgis(table_name, engine, if_exists="replace", index=False)
            print(f"Tablo {table_name} başarıyla güncellendi.")

        else:  # Tablo mevcut değil
            print(f"Tablo {table_name} mevcut değil, oluşturuluyor...")
            geo_df.to_postgis(table_name, engine, if_exists="replace", index=False)
            print(f"Tablo {table_name} başarıyla oluşturuldu ve veri eklendi.")

    except Exception as e:
        print(f"Veritabanı işlemleri sırasında hata oluştu: {e}")
        raise

def kml_to_geodataframe_check(kml_file_path,selected_region):
    
    tree = ET.parse(kml_file_path)
    root = tree.getroot()
    selected_region =selected_region
    if selected_region is None:
        raise ValueError("selected_region global değişkeni tanımlanmamış!")
    print(f"selected_region tipi: {type(selected_region)}, değeri: {selected_region}")
    
    geometries = []
    geometry_types = []
    names = []
    style_urls = []
    coords_list = []  # Koordinatları ayrı bir sütun olarak tutmak için

    # Parse each Placemark
    for placemark in root.findall('.//{http://www.opengis.net/kml/2.2}Placemark'):
        name = placemark.find('.//{http://www.opengis.net/kml/2.2}name')
        style_url = placemark.find('.//{http://www.opengis.net/kml/2.2}styleUrl')

        # Eğer styleUrl varsa, name kısmına styleUrl'yi atayacağız
        if style_url is not None:
            style_url_text = style_url.text
        else:
            style_url_text = "No styleUrl"

        # Eğer name yoksa "Unnamed" olarak ayarla
        if name is None:
            name_text = style_url_text  # name yerine styleUrl kullanıyoruz
        else:
            name_text = name.text

        coordinates_element = placemark.find('.//{http://www.opengis.net/kml/2.2}coordinates')
        if coordinates_element is None or coordinates_element.text.strip() == "":
            continue  # Koordinatlar eksik, bu placemark'ı atla

        # Koordinatları dize olarak al ve listeye dönüştür
        coordinates_str = coordinates_element.text
        coords = [tuple(map(float, coord.split(',')[:2])) for coord in re.sub(r'\s+', ' ', coordinates_str.strip()).split()]

        # Koordinatları ham olarak sakla
        coords_list.append(coords)

        # Geometri oluştur
        if len(coords) == 1:  # Tek nokta
            geometries.append(Point(coords[0]))
            geometry_types.append('Point')
        elif len(coords) == 2:  # İki nokta varsa LineString olarak tanımla
            geometries.append(LineString(coords))
            geometry_types.append('LineString')
        elif len(coords) >= 3:  # Üç veya daha fazla nokta varsa Polygon olarak tanımla
            geometries.append(Polygon(coords))
            geometry_types.append('Polygon')

        names.append(name_text)
        style_urls.append(style_url_text)

    # Koordinatları string formatına dönüştür
    coords_str_list = [str(c) for c in coords_list]  # Burada koordinat listelerini string'e çeviriyoruz

    # GeoDataFrame oluştur ve koordinatları ayrı bir sütun olarak ekle
    gdf = gpd.GeoDataFrame({
        'name': names,
        'styleUrl': style_urls,
        'geometry': geometries,
        'geometry_type': geometry_types,
        'coords': coords_str_list  # Koordinatların string formatı
    })

    return separate_polygons(gdf,selected_region)




def extract_kaks_taks(text):
    # Remove any whitespace and convert to lowercase
    text = str(text).lower().strip()
    
    # Check if the text contains only numerical values (including decimal points)
    if re.match(r'^\d+(?:[,.]\d+)?$', text):
        value = float(text.replace(',', '.'))
        # Assume the value is both kaks and taks
        kaks = value
        taks = value
        return (kaks, taks, None)  # Return None as the third value
    
    # If it's not purely numerical, return the original text
    return (None, None, text)

def process_geo_dataframe_for_polygons(geo_df):
    # Koordinatları Polygon geometrilerine dönüştür
    geo_df['geometry'] = geo_df['coords'].apply(
        lambda x: Polygon(eval(x)) if len(eval(x)) >= 3 else None
    )
    
    # Geçersiz geometrileri filtrele
    geo_df = geo_df[geo_df['geometry'].notna()]

    if 'name_2' in geo_df.columns and 'styleUrl_2' in geo_df.columns:
        # Koordinatlara göre gruplama için yeni sütun ekle
        geo_df['coords_group'] = geo_df['coords'].astype(str)
        
        def process_row(row):
            name_2 = str(row['name_2'])
            kaks, taks, original = extract_kaks_taks(name_2)
            if kaks is not None and taks is not None:
                return pd.Series({
                    'imar_kaks': kaks, 
                    'imar_taks': taks
                })
            else:
                return pd.Series({
                    'imar_kaks': None, 
                    'imar_taks': None
                })

        # Her satır için KAKS ve TAKS değerlerini işle
        result = geo_df.apply(process_row, axis=1)
        geo_df = pd.concat([geo_df, result], axis=1)

        # Koordinat gruplarına göre KAKS ve TAKS değerlerini güncelle
        grouped = geo_df.groupby('coords_group')
        for coords, group in grouped:
            valid_group = group[group['imar_kaks'].notna() & group['imar_taks'].notna()]
            if not valid_group.empty:
                max_kaks = valid_group['imar_kaks'].max()
                min_taks = valid_group['imar_taks'].min()
                geo_df.loc[group.index, 'imar_kaks'] = max_kaks
                geo_df.loc[group.index, 'imar_taks'] = min_taks
    else:
        # Gerekli sütunlar yoksa varsayılan değerleri ata
        geo_df['imar_kaks'] = None
        geo_df['imar_taks'] = None

    # Sadece gerekli sütunları tut
    columns_to_keep = ['name', 'styleUrl', 'geometry', 'imar_kaks', 'imar_taks', 'hmax', 'emsal']
    intersections_filtered = geo_df[columns_to_keep]

    # GeoDataFrame oluştur
    geo_filtered = gpd.GeoDataFrame(
        intersections_filtered, 
        geometry='geometry', 
        crs="EPSG:4326"
    )
    
    return geo_filtered

def separate_polygons(gdf,selected_region):
    print("separate_polygons")
    if selected_region is None:
        raise ValueError("selected_region global değişkeni tanımlanmamış veya atanmış değil!")
    polygons = gdf[gdf['geometry_type'] == 'Polygon']
    points = gdf[gdf['geometry_type'] == 'Point']

    # styleUrl ile filtreleme
    sm_polygons = polygons[polygons['styleUrl'].str.startswith('#SM')]
    pl_polygons = polygons[polygons['styleUrl'].str.startswith('#PL')]
    save_to_db_with_dynamic_table(pl_polygons, selected_region)
    pl_polygons.to_csv(f"./database_kodlari/imar_kirilimlari/girdiler/{selected_region}-data.csv")

    # İkinci script için process_city_data çağrısı
    print(f"\n{selected_region} için imar analizi başlatılıyor...")
    process_city_data(selected_region, pl_polygons)
    
    # SM_Hm noktalarını ayrı filtrele
    sm_hm_points = points[
        points['styleUrl'].str.startswith('#SM_HM') | 
        points['styleUrl'].str.startswith('#SM_Hm')
    ]
    
    # SM_Em noktalarını filtrele
    sm_em_points = points[
        points['styleUrl'].str.startswith('#SM_EM') | 
        points['styleUrl'].str.startswith('#SM_Em')
    ]
    
    # Diğer SM noktaları (KAKS/TAKS)
    other_sm_points = points[
        points['styleUrl'].str.startswith('#SM_KAK') | 
        points['styleUrl'].str.startswith('#SM_TAK')
    ]
    
    # SM_Hm noktalarının PL poligonlarıyla kesişimini bul
    hm_intersections = gpd.overlay(pl_polygons, sm_hm_points, how='intersection', keep_geom_type=False)
    
    # SM_Em noktalarının PL poligonlarıyla kesişimini bul
    em_intersections = gpd.overlay(pl_polygons, sm_em_points, how='intersection', keep_geom_type=False)
    
    # Diğer kesişimleri bul
    intersections1 = gpd.overlay(sm_polygons, other_sm_points, how='intersection', keep_geom_type=False)
    intersections4 = gpd.overlay(pl_polygons, intersections1, how='intersection', keep_geom_type=False)
    intersections5 = gpd.overlay(pl_polygons, intersections1, how='difference', keep_geom_type=False)
    
    # Her PL poligonu için Hmax değerlerini bir dictionary'de topla
    hmax_values = {}
    for idx, row in hm_intersections.iterrows():
        coords = str(row['coords_1'])
        hmax_value = row['name_2'] if pd.notna(row['name_2']) else ''
        if coords in hmax_values:
            if hmax_value and hmax_value not in hmax_values[coords].split(', '):
                hmax_values[coords] = f"{hmax_values[coords]}, {hmax_value}"
        else:
            hmax_values[coords] = hmax_value

    # Her PL poligonu için Emsal değerlerini bir dictionary'de topla
    emsal_values = {}
    for idx, row in em_intersections.iterrows():
        coords = str(row['coords_1'])
        emsal_value = row['name_2'] if pd.notna(row['name_2']) else ''
        if coords in emsal_values:
            if emsal_value and emsal_value not in emsal_values[coords].split(', '):
                emsal_values[coords] = f"{emsal_values[coords]}, {emsal_value}"
        else:
            emsal_values[coords] = emsal_value

    # Sonuç DataFrame'lerine hmax ve emsal sütunlarını ekle
    def add_columns(row):
        coords = str(row['coords'])
        return pd.Series({
            'hmax': hmax_values.get(coords, ''),
            'emsal': emsal_values.get(coords, '')
        })

    columns = intersections4.apply(add_columns, axis=1)
    intersections4[['hmax', 'emsal']] = columns
    
    columns = intersections5.apply(add_columns, axis=1)
    intersections5[['hmax', 'emsal']] = columns
    
    # Sonuçları birleştir
    result = pd.concat([intersections4, intersections5], ignore_index=True)
    
    # result.to_csv("imar-datası-izmir.csv") 
    return process_geo_dataframe_for_polygons(result)

    

# def processData(intersections4):
#     # Aynı coords_1'e sahip satırları birleştiriyoruz
#     print(intersections4)
#     grouped = intersections4.groupby('coords_1').agg({
#         'name_1': 'first',  # İlk satırın name_1 değerini al
#         'styleUrl_1': 'first',  # İlk satırın styleUrl_1 değerini al
#         'geometry_type_1': 'first',  # İlk satırın geometry_type_1 değerini al
#         'coords_1': 'first',  # İlk satırın coords değerini al
#         'name_2': lambda x: ', '.join(x.dropna().astype(str)),  # name_2'yi virgülle birleştir
#         'styleUrl_2': 'first',  # İlk satırın styleUrl_2 değerini al
#         'geometry_type_2': 'first',  # İlk satırın geometry_type_2'sini al
#         'geometry': 'first'  # İlk satırın geometry'sini al
#     }).reset_index(drop=True)  # Gruptan sonra index'i sıfırla

#     # coords sütunundaki string verileri Point geometrilerine çeviriyoruz
#     grouped['geometry'] = grouped['coords_1'].apply(lambda x: Point(eval(x)[0]))  # İlk koordinatı alıyoruz

#     # grouped DataFrame'ini GeoDataFrame'e çevir ve CRS'yi ayarla
#     geo_grouped = gpd.GeoDataFrame(grouped, geometry='geometry', crs="EPSG:4326")
#     geo_grouped.to_csv("izmir-imar-verileri.csv",index=False)
    
#     # Sonuçları CSV dosyasına kaydet
   

#     return geo_grouped



