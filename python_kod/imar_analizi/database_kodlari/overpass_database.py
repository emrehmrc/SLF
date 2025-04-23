import geopandas as gpd
import xml.etree.ElementTree as ET
from shapely.geometry import Point, Polygon, LineString
import re
import pandas as pd
from shapely.validation import make_valid
from sqlalchemy import create_engine, text
from shapely import wkt
import sys
import os
from datetime import datetime

# Veritabanı bağlantıları
DB_CONNECTIONS = {
    "gdz": "postgresql://postgres:12345@localhost:5432/gdz",  # İzmir için
    "oedas": "postgresql://postgres:12345@localhost:5432/oedas"  # Eskişehir için
}

# Tablo adları
TABLE_NAMES = {
    "İzmir": "IZMIR_IMAR_PL",
    "Eskişehir": "ESKISEHIR_IMAR_PL"
}

def ensure_postgis_extension(engine):
    """
    Veritabanında PostGIS uzantısını kontrol eder ve gerekiyorsa etkinleştirir.
    """
    try:
        with engine.connect() as connection:
            print("PostGIS uzantısı kontrol ediliyor...")
            result = connection.execute(text("SELECT 1 FROM pg_extension WHERE extname = 'postgis';"))
            if not result.fetchone():
                print("PostGIS uzantısı etkin değil. Etkinleştiriliyor...")
                connection.execute(text("CREATE EXTENSION IF NOT EXISTS postgis;"))
                print("PostGIS uzantısı etkinleştirildi.")
    except Exception as e:
        print(f"PostGIS uzantısı kontrolü veya etkinleştirme sırasında hata oluştu: {e}")
        raise

def get_database_connection(city):
    db_choice = "gdz" if city == "İzmir" else "oedas"
    return create_engine(DB_CONNECTIONS[db_choice])

def load_hucre(city, engine):
    try:
        print("Hücre verileri yükleniyor...")
        if city == "İzmir":
            table_name = "IZMIR_HUCRE"
            ilce_filter = "('ÇİĞLİ', 'KARŞIYAKA')"
            query = f"""
            SELECT id, ilce, "left", "top", "right", "bottom"
            FROM public."{table_name}"
            WHERE ilce IN {ilce_filter}
            """
            hucre_df = pd.read_sql(query, engine)
        else:
            hucre_file = "../girdiler/eskisehir_hucreleri.xlsx"
            print(f"Hücre verisi okunuyor: {hucre_file}")
            hucre_df = pd.read_excel(hucre_file)
        
        hucre_df['geometry'] = hucre_df.apply(lambda row: Polygon([
            (float(row['left']), float(row['top'])),
            (float(row['right']), float(row['top'])),
            (float(row['right']), float(row['bottom'])),
            (float(row['left']), float(row['bottom'])),
            (float(row['left']), float(row['top']))
        ]), axis=1)
        
        return gpd.GeoDataFrame(hucre_df, geometry='geometry', crs="EPSG:4326")
    
    except Exception as e:
        print(f"Hücre verisi yüklenirken hata oluştu: {e}")
        raise

def calculate_intersection(imar_df, hucre_df):
    try:
        print("Projeksiyon dönüşümü ve kesişim hesaplama başlıyor...")
        
        if not isinstance(imar_df, gpd.GeoDataFrame):
            imar_df = gpd.GeoDataFrame(imar_df, geometry='geometry', crs="EPSG:4326")
        if not isinstance(hucre_df, gpd.GeoDataFrame):
            hucre_df = gpd.GeoDataFrame(hucre_df, geometry='geometry', crs="EPSG:4326")
        
        imar_df_proj = imar_df.to_crs("EPSG:32636")
        hucre_df_proj = hucre_df.to_crs("EPSG:32636")

        hucre_df_proj['hucre_area'] = hucre_df_proj['geometry'].area

        intersections = gpd.overlay(hucre_df_proj, imar_df_proj, how='intersection', keep_geom_type=False)
        
        intersections = intersections.merge(
            hucre_df_proj[['id', 'hucre_area']], 
            on='id', 
            how='left'
        )

        intersections['intersection_area'] = intersections['geometry'].area
        
        return intersections, hucre_df_proj
    except Exception as e:
        print(f"Kesişim hesaplanırken hata oluştu: {e}")
        raise

def calculate_metrics(intersections):
    try:
        print("Metrikler hesaplanıyor...")
        
        intersections = intersections.rename(columns={'hucre_area_x': 'hucre_area'})

        imar_type_mapping = {
            'Kentsel Dönüşüm': 'Kentsel Donusum',
            'Yasaklİ Alan': 'Yasakli Alan'
        }
        intersections['İmar Tipi (Land-Use)'] = intersections['İmar Tipi (Land-Use)'].replace(imar_type_mapping)

        intersections['intersection_area'] = intersections['geometry'].area

        total_intersection_areas = intersections.groupby('id')['intersection_area'].sum().reset_index()
        total_intersection_areas.columns = ['id', 'Grand Total']

        count_data = intersections.groupby(['id', 'İmar Tipi (Land-Use)']).size().unstack(fill_value=0).reset_index()
        count_data.columns.name = None

        count_data = count_data.merge(intersections[['id', 'hucre_area']].drop_duplicates(), on='id', how='left')
        count_data = count_data.rename(columns={'hucre_area': 'hucre_m2'})

        imar_types = ['Yasakli Alan', 'Kentsel Donusum', 'Mesken', 
                     'Tarimsal Sulama', 'Sanayi', 'Ticarethane', 'Diğer']
        
        metrics_list = []
        
        for imar_type in imar_types:
            intersection_area = (
                intersections[intersections['İmar Tipi (Land-Use)'] == imar_type]
                .groupby('id')['intersection_area']
                .sum()
                .reset_index()
            )
            intersection_area.columns = ['id', f'{imar_type.lower().replace(" ", "_")}_m2']
            metrics_list.append(intersection_area)
            
            merged_data = intersection_area.merge(count_data[['id', 'hucre_m2']], on='id', how='right')
            merged_data[f'{imar_type.lower().replace(" ", "_")}_percentage'] = (
                merged_data[f'{imar_type.lower().replace(" ", "_")}_m2'] / 
                merged_data['hucre_m2'].replace(0, 1)
            ) * 100
            metrics_list.append(
                merged_data[['id', f'{imar_type.lower().replace(" ", "_")}_percentage']]
            )

        final_metrics = count_data
        final_metrics = final_metrics.merge(total_intersection_areas, on='id', how='left')
        
        for metric_df in metrics_list:
            final_metrics = final_metrics.merge(metric_df, on='id', how='left')

        final_metrics = final_metrics.fillna(0)
        
        return final_metrics

    except Exception as e:
        print(f"Metrik hesaplama sırasında hata oluştu: {e}")
        raise

def save_to_database(dataframe, table_name, engine):
    try:
        print(f"Veri {table_name} tablosuna kaydediliyor...")
        
        safe_table_name = table_name.lower().replace('-', '_')
        
        with engine.begin() as connection:
            drop_table_query = text(f'DROP TABLE IF EXISTS {safe_table_name}')
            connection.execute(drop_table_query)
            
            dataframe.to_sql(
                safe_table_name,
                con=connection,
                index=False,
                if_exists='replace',
                schema='public'
            )
            
            check_query = f'SELECT COUNT(*) FROM {safe_table_name}'
            result = connection.execute(text(check_query))
            count = result.scalar()
            print(f"Kaydedilen satır sayısı: {count}")
        
        return True
        
    except Exception as e:
        print(f"Veritabanına kayıt sırasında hata oluştu: {e}")
        raise

def save_to_db_with_dynamic_table(geo_df, region):
    try:
        db_choice = "gdz" if "İzmir" in region else "oedas" if "Eskişehir" in region else None
        if not db_choice:
            raise ValueError("Bilinmeyen bölge: 'İzmir' veya 'Eskişehir' bekleniyor.")

        table_name = TABLE_NAMES.get(region)
        if not table_name:
            raise ValueError("Tablo adı belirlenemedi. Bölge kontrol edin.")

        engine = create_engine(DB_CONNECTIONS[db_choice])

        if geo_df.crs is None:
            geo_df.set_crs("EPSG:4326", inplace=True)

        table_exists_query = text(f"""
        SELECT EXISTS (
            SELECT FROM information_schema.tables 
            WHERE table_schema = 'public' 
            AND table_name = '{table_name.lower()}'
        );
        """)

        with engine.connect() as conn:
            result = conn.execute(table_exists_query).scalar()

        if result:
            print(f"Tablo {table_name} mevcut, güncelleniyor...")
            existing_data = gpd.read_postgis(f"SELECT * FROM {table_name}", con=engine, geom_col='geometry')
            merged_data = pd.concat([existing_data, geo_df]).drop_duplicates(subset=['name', 'styleUrl'])
            merged_data.to_postgis(table_name, engine, if_exists="replace", index=False)
            print(f"Tablo {table_name} başarıyla güncellendi.")
        else:
            print(f"Tablo {table_name} mevcut değil, oluşturuluyor...")
            geo_df.to_postgis(table_name, engine, if_exists="replace", index=False)
            print(f"Tablo {table_name} başarıyla oluşturuldu ve veri eklendi.")

    except Exception as e:
        print(f"Veritabanı işlemleri sırasında hata oluştu: {e}")
        raise

def load_and_process_data(selected_region, pl_polygons=None):
    try:
        print(f"{selected_region} için veri işleme başlatılıyor...")
        
        if selected_region == "İzmir":
            engine = get_database_connection(selected_region)
            
            table_name = "IZMIR_IMAR_PL"
            query = f"""
            SELECT name, "styleUrl", geometry
            FROM public."{table_name}"
            """
            imar_df = gpd.read_postgis(query, engine, geom_col='geometry')
            
            hucre_df = load_hucre(selected_region, engine)
            
            intersections, hucre_df = calculate_intersection(imar_df, hucre_df)
            metrics = calculate_metrics(intersections)
            
            metrics_table_name = "v4_gdz_birlestirilmis_veri_imar_ada_tipi_sayisi"
            save_to_database(metrics, metrics_table_name, engine)
            print(f"Metrikler {metrics_table_name} tablosuna kaydedildi")
            
        else:  # Eskişehir için
            if pl_polygons is None:
                csv_path = f"./pl_poligonlar/{selected_region}-data.csv"
                if os.path.exists(csv_path):
                    print(f"CSV dosyası okunuyor: {csv_path}")
                    pl_polygons = pd.read_csv(csv_path)
                    pl_polygons['geometry'] = pl_polygons['geometry'].apply(wkt.loads)
                    pl_polygons = gpd.GeoDataFrame(pl_polygons, geometry='geometry', crs="EPSG:4326")
                else:
                    raise FileNotFoundError(f"CSV dosyası bulunamadı: {csv_path}")
            
            hucre_df = load_hucre(selected_region, None)
            
            intersections, hucre_df = calculate_intersection(pl_polygons, hucre_df)
            metrics = calculate_metrics(intersections)
            
            output_filename = f"../ciktilar/eskisehir_ada_imar_tipi_sayisi_{datetime.now().strftime('%Y%m%d_%H%M%S')}.csv"
            metrics.to_csv(output_filename, index=False)
            print(f"Metrikler {output_filename} dosyasına kaydedildi")
            
            try:
                engine = create_engine(DB_CONNECTIONS["oedas"])
                ensure_postgis_extension(engine)
                metrics_table_name = "eskisehir_imar_ada_tipi_sayisi"
                save_to_database(metrics, metrics_table_name, engine)
                print(f"Metrikler {metrics_table_name} tablosuna da kaydedildi")
            except Exception as e:
                print(f"Veritabanına kayıt sırasında hata: {e}")
                print("İşleme CSV dosyası ile devam edildi")
        
        return metrics
        
    except Exception as e:
        print(f"Veri işleme sırasında hata oluştu: {e}")
        raise

def extract_kaks_taks(text):
    text = str(text).lower().strip()
    
    if re.match(r'^\d+(?:[,.]\d+)?$', text):
        value = float(text.replace(',', '.'))
        kaks = value
        taks = value
        return (kaks, taks, None)
    
    return (None, None, text)

def process_geo_dataframe_for_polygons(geo_df):
    geo_df['geometry'] = geo_df['coords'].apply(
        lambda x: Polygon(eval(x)) if len(eval(x)) >= 3 else None
    )
    
    geo_df = geo_df[geo_df['geometry'].notna()]

    if 'name_2' in geo_df.columns and 'styleUrl_2' in geo_df.columns:
        geo_df['coords_group'] = geo_df['coords'].astype(str)
        
        def process_row(row):
            name_2 = str(row['name_2'])
            kaks, taks, original = extract_kaks_taks(name_2)
            return pd.Series({
                'imar_kaks': kaks, 
                'imar_taks': taks
            })

        result = geo_df.apply(process_row, axis=1)