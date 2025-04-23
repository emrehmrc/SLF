# python .\imar_tipleri_analizi_v2.py "C:\Users\batuhan.yetis\OneDrive - MRC\batuhan\imar-dataları\output-shp-files\unique_gdf_izmir.csv" İzmir
# python .\imar_tipleri_analizi_v2.py "C:\Users\batuhan.yetis\OneDrive - MRC\batuhan\imar-dataları\output-shp-files\unique_gdf_eskisehir.csv" Eskişehir

import pandas as pd
import geopandas as gpd
import numpy as np
import os
import re
from shapely.geometry import Point, Polygon
from shapely import wkt
from scipy.spatial.distance import cdist
import matplotlib.pyplot as plt
import warnings

# Uyarıları bastır
warnings.filterwarnings('ignore')

#############################################
# YARDIMCI FONKSİYONLAR
#############################################

def find_coordinate_columns(df):
    """Veri çerçevesinde koordinat sütunlarını bul"""
    x_candidates = ['X_KOORDINAT', 'X', 'x', 'LON', 'lon', 'longitude', 'Longitude', 'LONGITUDE']
    y_candidates = ['Y_KOORDINAT', 'Y', 'y', 'LAT', 'lat', 'latitude', 'Latitude', 'LATITUDE']
    
    x_column = next((col for col in x_candidates if col in df.columns), None)
    y_column = next((col for col in y_candidates if col in df.columns), None)
    
    return x_column, y_column


def clean_hmax(value):
    """hmax değerlerini temizler ve sayısallaştırır"""
    try:
        if pd.isna(value):
            return None
        if isinstance(value, (int, float)):
            return float(value) if 0 < value < 200 else None
        if isinstance(value, str):
            value = value.lower().strip()
            if 'serbest' in value:
                return None
            if 'kat' in value:
                kat_match = re.findall(r'(\d+)\s*kat', value)
                if kat_match:
                    return float(kat_match[0]) * 2.5
            value = value.replace(',', '.')
            numbers = re.findall(r'\d+\.?\d*', value)
            if numbers:
                num = float(numbers[0])
                return num if 0 < num < 200 else None
        return None
    except Exception:
        return None


#############################################
# DOSYA YÜKLEME FONKSİYONLARI
#############################################

def load_point_data_from_csv(mesken_csv_path=None, ticarethane_csv_path=None):
    """Mesken ve ticarethane verilerini CSV dosyalarından okur ve GeoDataFrame olarak döndürür"""
    points_data = []
    
    # Mesken noktaları
    if mesken_csv_path and os.path.exists(mesken_csv_path):
        print(f"Mesken noktaları okunuyor: {mesken_csv_path}")
        df_mesken = pd.read_csv(mesken_csv_path, encoding='utf-8')
        x_col_m, y_col_m = find_coordinate_columns(df_mesken)
        
        if x_col_m is None or y_col_m is None:
            print(f"UYARI: Mesken dosyasında koordinat sütunları bulunamadı.")
        else:
            valid_mask_m = df_mesken[x_col_m].notna() & df_mesken[y_col_m].notna()
            df_mesken_valid = df_mesken.loc[valid_mask_m].copy()
            
            geometry_m = [Point(xy) for xy in zip(df_mesken_valid[x_col_m], df_mesken_valid[y_col_m])]
            gdf_mesken = gpd.GeoDataFrame(df_mesken_valid, geometry=geometry_m, crs="EPSG:4326")
            gdf_mesken['point_type'] = 'mesken'
            gdf_mesken['point_id'] = [f"M{i:06d}" for i in range(len(gdf_mesken))]
            points_data.append(gdf_mesken)
            print(f"Mesken noktaları yüklendi: {len(gdf_mesken)} nokta")
    
    # Ticarethane noktaları
    if ticarethane_csv_path and os.path.exists(ticarethane_csv_path):
        print(f"Ticarethane noktaları okunuyor: {ticarethane_csv_path}")
        df_ticarethane = pd.read_csv(ticarethane_csv_path, encoding='utf-8')
        x_col_t, y_col_t = find_coordinate_columns(df_ticarethane)
        
        if x_col_t is None or y_col_t is None:
            print(f"UYARI: Ticarethane dosyasında koordinat sütunları bulunamadı.")
        else:
            valid_mask_t = df_ticarethane[x_col_t].notna() & df_ticarethane[y_col_t].notna()
            df_ticarethane_valid = df_ticarethane.loc[valid_mask_t].copy()
            
            geometry_t = [Point(xy) for xy in zip(df_ticarethane_valid[x_col_t], df_ticarethane_valid[y_col_t])]
            gdf_ticarethane = gpd.GeoDataFrame(df_ticarethane_valid, geometry=geometry_t, crs="EPSG:4326")
            gdf_ticarethane['point_type'] = 'ticarethane'
            gdf_ticarethane['point_id'] = [f"T{i:06d}" for i in range(len(gdf_ticarethane))]
            points_data.append(gdf_ticarethane)
            print(f"Ticarethane noktaları yüklendi: {len(gdf_ticarethane)} nokta")
    
    # Tüm noktaları birleştir
    if points_data:
        all_points = pd.concat(points_data, ignore_index=True)
        print(f"Toplam nokta sayısı: {len(all_points)}")
        return all_points
    else:
        print("UYARI: Hiç nokta verisi yüklenemedi.")
        return None


#############################################
# BİNA EŞLEŞTİRME FONKSİYONLARI
#############################################

def create_building_polygons_dict(overpass_verileri):
    """Bina ID'lerine göre poligonları saklayan sözlük oluşturur"""
    building_polygons = {}
    print("Building polygons dictionary creating...")
    for idx, row in overpass_verileri.iterrows():
        if row.geometry is not None:
            try:
                # Geometrinin türünü kontrol et
                if isinstance(row.geometry, str):
                    # Eğer zaten string ise, doğrudan kullan
                    building_polygons[row['bina_id']] = row.geometry
                else:
                    # Değilse, wkt'ye dönüştür
                    building_polygons[row['bina_id']] = row.geometry.wkt
                
                if idx % 1000 == 0:
                    print(f"Processed {idx} buildings...")
            except Exception as e:
                print(f"Error processing building {row['bina_id']}: {e}")
    
    print(f"Total buildings in dictionary: {len(building_polygons)}")
    if building_polygons:
        first_items = list(building_polygons.items())[:3]
        print("First few items in dictionary:", first_items)
    return building_polygons
def test_grid_buffer_matching(points_data, buildings_data, buffer_values=[0.00005, 0.0001, 0.0002, 0.0005]):
    """
    Farklı buffer değerleri ile grid test yapar
    """
    print("\n=== GRID BUFFER TEST BAŞLATILIYOR ===")
    
    # Sadece ilk 100 nokta ve ilk 100 binayı kullan (hız için)
    sample_points = points_data.head(100).copy() if len(points_data) > 100 else points_data.copy()
    sample_buildings = buildings_data.head(100).copy() if len(buildings_data) > 100 else buildings_data.copy()
    
    print(f"Test için {len(sample_points)} nokta ve {len(sample_buildings)} bina kullanılıyor.")
    
    # Koordinat sınırlarını kontrol et
    points_bounds = sample_points.total_bounds
    buildings_bounds = sample_buildings.total_bounds
    print(f"Nokta sınırları: {points_bounds}")
    print(f"Bina sınırları: {buildings_bounds}")
    
    # Koordinat örneklerini göster
    print("\nİlk 3 nokta koordinatları:")
    for i, point in enumerate(sample_points.head(3).geometry):
        print(f"Nokta {i}: {point.wkt}")
    
    print("\nİlk 3 bina koordinatları:")
    for i, building in enumerate(sample_buildings.head(3).geometry):
        print(f"Bina {i}: {building.wkt}")
    
    # Her buffer değeri için test
    results = []
    for buffer_size in buffer_values:
        print(f"\nBuffer değeri: {buffer_size} (yaklaşık {buffer_size*111000:.1f} metre)")
        
        # Binaları bufferla
        buffered_buildings = sample_buildings.copy()
        buffered_buildings['geometry'] = buffered_buildings.geometry.buffer(buffer_size)
        
        # Spatial join
        joined = gpd.sjoin(
            sample_points,
            buffered_buildings,
            how='inner',
            predicate='within'
        )
        
        # Sonuçları kaydet
        match_count = len(joined)
        unique_points = joined['point_id'].nunique() if 'point_id' in joined.columns else match_count
        
        print(f"Eşleşen nokta sayısı: {match_count}")
        print(f"Benzersiz nokta sayısı: {unique_points}/{len(sample_points)}")
        
        results.append({
            'buffer_size': buffer_size,
            'buffer_meters': buffer_size*111000,
            'match_count': match_count,
            'unique_points': unique_points,
            'percent_matched': (unique_points/len(sample_points))*100 if len(sample_points) > 0 else 0
        })
        
        # İlk eşleşmeleri göster (varsa)
        if match_count > 0:
            print("İlk eşleşme örnekleri:")
            for i, row in joined.head(3).iterrows():
                point_geom = row.geometry
                point_idx = sample_points.index.get_loc(i) if i in sample_points.index else "Unknown"
                building_idx = row.index_right
                print(f"Nokta {point_idx} <-> Bina {building_idx}")
    
    # Özet
    print("\nBuffer testi özeti:")
    for result in results:
        print(f"Buffer: {result['buffer_size']} (~{result['buffer_meters']:.1f}m) -> "
              f"Eşleşme: {result['unique_points']}/{len(sample_points)} "
              f"({result['percent_matched']:.2f}%)")
    
    return results
def inspect_data_structures(points_data, buildings_data):
    """
    Veri yapılarını detaylı olarak inceler
    """
    print("\n=== VERİ YAPISI İNCELEME ===")
    
    # Nokta verisi
    print(f"Nokta verisi tipi: {type(points_data)}")
    print(f"Nokta verisi CRS: {points_data.crs}")
    print(f"Nokta verisi sütunları: {points_data.columns.tolist()}")
    print(f"Nokta verisi boyutu: {points_data.shape}")
    print(f"Nokta geometri tipleri: {points_data.geometry.type.value_counts().to_dict()}")
    
    # Bina verisi
    print(f"Bina verisi tipi: {type(buildings_data)}")
    print(f"Bina verisi CRS: {buildings_data.crs}")
    print(f"Bina verisi sütunları: {buildings_data.columns.tolist()}")
    print(f"Bina verisi boyutu: {buildings_data.shape}")
    print(f"Bina geometri tipleri: {buildings_data.geometry.type.value_counts().to_dict()}")
    
    # Geometri örnekleri
    print("\nGeometri örnekleri:")
    if len(points_data) > 0:
        print(f"Örnek nokta: {points_data.iloc[0].geometry}")
    if len(buildings_data) > 0:
        print(f"Örnek bina: {buildings_data.iloc[0].geometry}")
    
    # Nokta ve bina verisi arasındaki farkları incele
    if points_data.crs != buildings_data.crs:
        print("UYARI: CRS değerleri farklı!")
    
    # Sınırları karşılaştır
    points_bounds = points_data.total_bounds
    buildings_bounds = buildings_data.total_bounds
    
    x_overlap = (points_bounds[0] < buildings_bounds[2] and buildings_bounds[0] < points_bounds[2])
    y_overlap = (points_bounds[1] < buildings_bounds[3] and buildings_bounds[1] < points_bounds[3])
    
    if not (x_overlap and y_overlap):
        print("KRİTİK UYARI: Nokta ve bina koordinat sınırları örtüşmüyor!")
        print(f"Nokta X aralığı: {points_bounds[0]} - {points_bounds[2]}")
        print(f"Bina X aralığı: {buildings_bounds[0]} - {buildings_bounds[2]}")
        print(f"Nokta Y aralığı: {points_bounds[1]} - {points_bounds[3]}")
        print(f"Bina Y aralığı: {buildings_bounds[1]} - {buildings_bounds[3]}")
def perform_building_matching(all_points, overpass_verileri, ada_kenarlari):
    """
    Bina eşleştirme adımlarını gerçekleştirir:
      1. Direkt eşleştirme (nokta, bina poligonu içinde)
      2. Buffer eşleştirme (sabit buffer uygulanarak, her nokta için en yakın bina centroid'i bulunur)
      3. Ada bazlı eşleştirme (hâlâ eşleşmeyen noktalar için)
    """
    print("\n=== DETAYLI EŞLEŞTIRME DEBUGGER BAŞLATILIYOR ===")
    print(f"Başlangıç nokta sayısı: {len(all_points)}")
    
    # CRS kontrolü
    print(f"Nokta verisi CRS: {all_points.crs}")
    print(f"Overpass verisi CRS: {overpass_verileri.crs}")
    print(f"Ada kenarları CRS: {ada_kenarlari.crs}")
    
    if all_points.crs != overpass_verileri.crs:
        print("CRS uyumsuzluğu tespit edildi! Dönüşüm yapılıyor...")
        all_points = all_points.to_crs(overpass_verileri.crs)
        print(f"Nokta verisi CRS güncellendi: {all_points.crs}")
    
    # Bina ID kontrolü - çok önemli
    if 'bina_id' not in overpass_verileri.columns:
        print("'bina_id' sütunu bulunamadı, oluşturuluyor...")
        overpass_verileri['bina_id'] = [f"B{i:06d}" for i in range(len(overpass_verileri))]
    
    # Verilerin genel durumunu yazdır
    print("\nGenel Bilgiler:")
    print(f"Nokta sayısı: {len(all_points)}")
    print(f"Bina sayısı: {len(overpass_verileri)}")
    print(f"Ada sayısı: {len(ada_kenarlari)}")
    
    # İmar parametrelerini belirle
    imar_parameters = ['imar_kaks', 'imar_taks', 'hmax', 'emsal', 'TAKS', 'KAKS']
    
    # Mevcut imar parametrelerini kontrol et
    available_imar_params = [param for param in imar_parameters if param in overpass_verileri.columns]
    print(f"Mevcut imar parametreleri: {available_imar_params}")
    
    # Bina poligonları sözlüğünü oluştur - Burada oluşturarak performansı artır
    building_polygons = create_building_polygons_dict(overpass_verileri)
    
    # Bina geometrilerini ve imar parametrelerini sakla - Sık kullanılacak verileri önden hazırla
    buildings_for_replacement = {}
    buildings_imar_params = {}
    
    for idx, row in overpass_verileri.iterrows():
        if row.geometry is not None:
            buildings_for_replacement[row['bina_id']] = row.geometry
            
            # İmar parametrelerini sakla
            imar_data = {}
            for param in available_imar_params:
                if param in row:
                    imar_data[param] = row[param]
            buildings_imar_params[row['bina_id']] = imar_data
    
    # 1. Direkt eşleştirme
    print("\n--- 1. DIREKT EŞLEŞTIRME ---")
    
    # Point türü kontrolü ve 'point_type' sütununu kontrol et
    if 'point_type' not in all_points.columns:
        print("UYARI: 'point_type' sütunu bulunamadı, varsayılan 'mesken' oluşturuluyor...")
        all_points['point_type'] = 'mesken'  # Varsayılan değer
    
    # Point ID kontrolü - Her nokta için benzersiz ID olduğundan emin ol
    if 'point_id' not in all_points.columns:
        print("'point_id' sütunu bulunamadı, oluşturuluyor...")
        point_type_prefix = all_points['point_type'].apply(lambda x: 'M' if x == 'mesken' else 'T').values
        all_points['point_id'] = [f"{prefix}{i:06d}" for prefix, i in zip(point_type_prefix, range(len(all_points)))]
    
    # Sadece Point geometrili noktaları filtrele - hız için çok önemli!
    points_for_join = all_points[all_points.geometry.type == 'Point'].copy()
    print(f"Point geometrili kayıt sayısı: {len(points_for_join)}")
    
    # Sadece Polygon veya MultiPolygon geometrili binaları filtrele
    buildings_for_join = overpass_verileri[overpass_verileri.geometry.type.isin(['Polygon', 'MultiPolygon'])].copy()
    print(f"Polygon geometrili bina sayısı: {len(buildings_for_join)}")
    
    # Direkt eşleştirme için GeoDataFrame hazırla - Minimal sütunlar kullanarak hafızayı optimize et
    columns_to_include = ['bina_id', 'bina_alan_m2', 'bina_alan_m2_old', 'parcel_name'] + available_imar_params
    available_columns = [col for col in columns_to_include if col in buildings_for_join.columns]
    
    overpass_for_direct_join = gpd.GeoDataFrame(
        buildings_for_join[available_columns].copy(), 
        geometry=buildings_for_join.geometry,
        crs=buildings_for_join.crs
    )
    
    # Direkt kesişim kontrolü - sjoin'i optimize et
    direct_joined = gpd.sjoin(
        points_for_join, 
        overpass_for_direct_join,
        how='inner', 
        predicate='within'
    )
    print(f"Direkt eşleşen nokta sayısı: {len(direct_joined)}")
    
    # Tekrarlanan nokta ID'lerini temizle
    if len(direct_joined) > 0:
        # Aynı noktanın birden fazla bina ile eşleşmesi durumunda, tekrarlanan nokta ID'lerini belirle
        duplicate_points = direct_joined[direct_joined.duplicated('point_id', keep=False)]
        if not duplicate_points.empty:
            print(f"UYARI: {len(duplicate_points)} nokta birden fazla bina ile eşleşiyor.")
            # Bu durumda her nokta için tek bir eşleşme seç (ilk eşleşmeyi tut)
            direct_joined = direct_joined.drop_duplicates('point_id', keep='first')
            print(f"Tekrarlar temizlendi, kalan nokta sayısı: {len(direct_joined)}")
        
        direct_matched_points = direct_joined.copy()
        direct_matched_points['match_type'] = 'direct_match'
        direct_matched_points['matched_bina_id'] = direct_matched_points['bina_id']
        
        # Alan bilgisini al - birden fazla alan sütunu olasılığına karşı
        if 'bina_alan_m2_old' in direct_matched_points.columns and direct_matched_points['bina_alan_m2_old'].notna().any():
            direct_matched_points['area_m2'] = direct_matched_points['bina_alan_m2_old']
        elif 'bina_alan_m2' in direct_matched_points.columns and direct_matched_points['bina_alan_m2'].notna().any():
            direct_matched_points['area_m2'] = direct_matched_points['bina_alan_m2']
        else:
            direct_matched_points['area_m2'] = None
        
        # Building polygon bilgilerini ekle
        direct_matched_points['matched_building_polygon'] = direct_matched_points['matched_bina_id'].apply(
            lambda x: building_polygons.get(x) if x is not None else None
        )
        
        # Geometrileri değiştir - geometriyi bina geometrisiyle değiştir
        direct_data = []
        for idx, row in direct_matched_points.iterrows():
            bina_id = row['matched_bina_id']
            new_row = row.copy()
            new_row['match_type'] = 'direct_match'  # Eşleşme tipini koru
            
            # Eğer geçerli bir geometri varsa, değiştir
            if bina_id in buildings_for_replacement:
                geom = buildings_for_replacement[bina_id]
                if geom is not None and hasattr(geom, 'is_valid') and geom.is_valid:
                    new_row['geometry'] = geom
            
            # İmar parametrelerini ekle
            if bina_id in buildings_imar_params:
                imar_data = buildings_imar_params[bina_id]
                for param, value in imar_data.items():
                    new_row[param] = value
                    
            # Her durumda satırı ekle - Koşul olmadan!
            direct_data.append(new_row)
        
        # Yeni GeoDataFrame oluştur
        if direct_data:
            direct_matched_points = gpd.GeoDataFrame(
                direct_data, 
                geometry='geometry',
                crs=all_points.crs
            )
            print(f"Direct match sayısı: {len(direct_matched_points)}")
            print(f"Direct match tip kontrol: {direct_matched_points['match_type'].value_counts().to_dict()}")
        else:
            print("UYARI: direct_data listesi boş!")
            direct_matched_points = gpd.GeoDataFrame(
                columns=direct_joined.columns, 
                geometry='geometry',
                crs=all_points.crs
            )
    else:
        # Boş GeoDataFrame oluştur
        direct_matched_points = gpd.GeoDataFrame(
            columns=['geometry', 'point_type', 'point_id', 'matched_bina_id', 
                     'match_type', 'area_m2', 'matched_building_polygon'] + available_imar_params,
            geometry='geometry',
            crs=all_points.crs
        )
    
    # Eşleşen noktaların ID'lerini al
    matched_point_ids = direct_matched_points['point_id'].unique().tolist()
    
    # 2. Buffer eşleştirme
    print("\n--- 2. BUFFER EŞLEŞTIRME ---")
    unmatched_points = all_points[~all_points['point_id'].isin(matched_point_ids)].copy()
    unmatched_points = unmatched_points[unmatched_points.geometry.type == 'Point'].copy()
    print(f"Buffer ile eşleştirilecek kofre sayısı: {len(unmatched_points)}")
    if len(unmatched_points) > 0:
        print("\n--- BUFFER EŞLEŞTIRME ÖNCESİ GRID TEST ---")
        grid_test_results = test_grid_buffer_matching(unmatched_points, buildings_for_join)
        
        # Eğer hiç eşleşme bulunamadıysa durumu yazdır
        if all(result['match_count'] == 0 for result in grid_test_results):
            print("KRİTİK HATA: Hiçbir buffer değeri eşleşme sağlayamadı!")
            print("Olası nedenler:")
            print("1. Koordinat sistemleri arasında dönüşüm sorunu")
            print("2. Noktalar ve binalar arasında sistematik bir kayma")
            print("3. Yanlış veri seti (farklı bölgeler)")
    if len(unmatched_points) > 0:
        # Bina merkezlerini hesapla
        overpass_verileri['centroid'] = overpass_verileri.geometry.centroid
        
        # Binaları buffer'la (WGS84 için yaklaşık 20 metre) - önceden hesapla
        buffer_degree = 0.00018
        overpass_verileri['expanded_geometry'] = overpass_verileri.geometry.buffer(buffer_degree)
        
        # Buffer için GeoDataFrame oluştur - minimal sütunlar için
        buffer_columns = ['bina_id', 'bina_alan_m2', 'bina_alan_m2_old', 'parcel_name', 'centroid'] + available_imar_params
        available_buffer_columns = [col for col in buffer_columns if col in overpass_verileri.columns]
        
        buffer_gdf = gpd.GeoDataFrame(
            overpass_verileri[available_buffer_columns].copy(),
            geometry=overpass_verileri['expanded_geometry'],
            crs=overpass_verileri.crs
        )
        
        # Buffer ile kesişim kontrolü - optimal sjoin
        buffer_joined = gpd.sjoin(
            unmatched_points, 
            buffer_gdf,
            how='inner', 
            predicate='within'
        )
        
        print(f"Buffer ile kesişen nokta sayısı: {len(buffer_joined)}")
        
        buffer_matched_points = gpd.GeoDataFrame(
            columns=direct_matched_points.columns, 
            geometry='geometry',
            crs=all_points.crs
        )
        
        if not buffer_joined.empty:
            buffer_matched_results = []
            # Her bir benzersiz noktaya odaklan
            unique_point_ids = buffer_joined['point_id'].unique()
            
            for point_id in unique_point_ids:
                point_matches = buffer_joined[buffer_joined['point_id'] == point_id].copy()
                
                # Noktanın geometrisini al - Polygon ise centroid'i kullan
                geometry = point_matches.iloc[0].geometry
                if geometry.geom_type == 'Point':
                    point_geom = geometry
                else:
                    # Polygon veya başka bir geometri tipi için centroid al
                    point_geom = geometry.centroid
                
                # Kesişen binaların centroid'lerini al
                centroids = []
                bina_ids = []
                areas = []
                
                for _, row in point_matches.iterrows():
                    # Centroid geometrisi için x,y koordinatlarını doğru şekilde al
                    if hasattr(row['centroid'], 'x') and hasattr(row['centroid'], 'y'):
                        centroids.append((row['centroid'].x, row['centroid'].y))
                        bina_ids.append(row['bina_id'])
                    
                    # Alan bilgisini al (eski veya yeni sütundan)
                    if 'bina_alan_m2_old' in row and not pd.isna(row['bina_alan_m2_old']):
                        areas.append(row['bina_alan_m2_old'])
                    elif 'bina_alan_m2' in row and not pd.isna(row['bina_alan_m2']):
                        areas.append(row['bina_alan_m2'])
                    else:
                        areas.append(None)
                
                # Geometrinin koordinatlarını doğru şekilde al
                if hasattr(point_geom, 'x') and hasattr(point_geom, 'y'):
                    point_coords = np.array([(point_geom.x, point_geom.y)])
                    centroids_array = np.array(centroids)
                    
                    # Centroids dizisi boş olmadığından emin ol
                    if len(centroids) > 0:
                        # Mesafeleri hesapla
                        distances = cdist(point_coords, centroids_array, 'euclidean')[0]
                        
                        # En yakın binayı bul
                        closest_idx = np.argmin(distances)
                        closest_bina_id = bina_ids[closest_idx]
                        closest_area = areas[closest_idx]
                        
                        # Bu noktayı en yakın bina ile eşleştir
                        result_row = point_matches.iloc[0].copy()
                        result_row['matched_bina_id'] = closest_bina_id
                        result_row['match_type'] = 'buffer_match'
                        result_row['area_m2'] = closest_area
                        
                        # Building polygon'u ekle
                        result_row['matched_building_polygon'] = building_polygons.get(closest_bina_id)
                        
                        # Bina geometrisini kullan
                        if closest_bina_id in buildings_for_replacement:
                            geom = buildings_for_replacement[closest_bina_id]
                            if geom is not None and hasattr(geom, 'is_valid') and geom.is_valid:
                                result_row['geometry'] = geom
                        
                        # İmar parametrelerini ekle
                        if closest_bina_id in buildings_imar_params:
                            imar_data = buildings_imar_params[closest_bina_id]
                            for param, value in imar_data.items():
                                result_row[param] = value
                                
                        buffer_matched_results.append(result_row)
            
            # Buffer sonuçlarını DataFrame'e dönüştür
            if buffer_matched_results:
                buffer_matched_df = pd.DataFrame(buffer_matched_results)
                buffer_matched_points = gpd.GeoDataFrame(
                    buffer_matched_df, 
                    geometry='geometry',
                    crs=all_points.crs
                )
                
                print(f"Buffer match tip kontrol: {buffer_matched_points['match_type'].value_counts().to_dict()}")
                print(f"Buffer ile eşleştirilen nokta sayısı: {len(buffer_matched_points)}")
                matched_point_ids.extend(buffer_matched_points['point_id'].unique().tolist())
    
    # 3. ADIM 3: Hala eşleşmeyen kofreleri ada kenarı ortalama alanına göre buffer oluştur
    still_unmatched_points = all_points[~all_points['point_id'].isin(matched_point_ids)].copy()
    still_unmatched_points = still_unmatched_points[still_unmatched_points.geometry.type == 'Point'].copy()
    print(f"Hala eşleşmeyen kofre sayısı: {len(still_unmatched_points)}")
    
    # Her ada için ortalama bina alanını hesapla
    if len(still_unmatched_points) > 0 and 'parcel_name' in overpass_verileri.columns and 'parcel_name' in ada_kenarlari.columns:
        # Ada kenarları GeoDataFrame'i hazırla
        ada_for_spatial_join = gpd.GeoDataFrame(
            ada_kenarlari[['parcel_name']], 
            geometry=ada_kenarlari.geometry,
            crs=ada_kenarlari.crs
        )
        
        # Binalar ve ada kenarları arasında spatial join
        try:
            ada_bina_kesisim = gpd.sjoin(
                overpass_verileri,
                ada_for_spatial_join,
                how='inner',
                predicate='within'
            )
            
            # Her ada için ortalama bina alanını hesapla
            if len(ada_bina_kesisim) > 0:
                if 'bina_alan_m2_old' in ada_bina_kesisim.columns:
                    area_col = 'bina_alan_m2_old'
                elif 'bina_alan_m2' in ada_bina_kesisim.columns:
                    area_col = 'bina_alan_m2'
                else:
                    area_col = None
                    
                if area_col is not None:
                    parcel_total = ada_bina_kesisim.groupby('parcel_name_right')[area_col].sum().reset_index()
                    parcel_count = ada_bina_kesisim.groupby('parcel_name_right').size().reset_index(name='count')
                    parcel_avg = pd.merge(parcel_total, parcel_count, on='parcel_name_right')
                    parcel_avg['avg_area'] = parcel_avg[area_col] / parcel_avg['count']
                    
                    # Sütun adını düzelt
                    parcel_avg.rename(columns={'parcel_name_right': 'parcel_name'}, inplace=True)
                    
                    # Ada kenarlarına ortalama alanları ekle
                    ada_kenarlari_with_avg = ada_kenarlari.merge(parcel_avg[['parcel_name', 'avg_area']], on='parcel_name', how='left')
                else:
                    ada_kenarlari_with_avg = ada_kenarlari.copy()
                    ada_kenarlari_with_avg['avg_area'] = 100.0  # Varsayılan değer
            else:
                ada_kenarlari_with_avg = ada_kenarlari.copy()
                ada_kenarlari_with_avg['avg_area'] = 100.0  # Varsayılan değer
        except Exception as e:
            print(f"Ada kenarı ortalama hesaplama hatası: {e}")
            ada_kenarlari_with_avg = ada_kenarlari.copy()
            ada_kenarlari_with_avg['avg_area'] = 100.0  # Varsayılan değer
    else:
        ada_kenarlari_with_avg = ada_kenarlari.copy()
        if 'avg_area' not in ada_kenarlari_with_avg.columns:
            ada_kenarlari_with_avg['avg_area'] = 100.0  # Varsayılan değer
    
    # Eşleşmeyen noktaları ada kenarlarıyla eşleştir
    unmatched_buffer_points = gpd.GeoDataFrame(
        columns=direct_matched_points.columns, 
        geometry='geometry',
        crs=all_points.crs
    )
    
    if len(still_unmatched_points) > 0:
        try:
            # Ada kenarları GeoDataFrame'i hazırla
            ada_for_join = gpd.GeoDataFrame(
                ada_kenarlari_with_avg[['parcel_name', 'avg_area']], 
                geometry=ada_kenarlari_with_avg.geometry,
                crs=ada_kenarlari_with_avg.crs
            )
            
            # Eşleşmeyen noktaları ada kenarlarıyla eşleştir
            unmatched_with_parcel = gpd.sjoin(
                still_unmatched_points,
                ada_for_join,
                how='left',
                predicate='within'
            )
            
            # Ada kenarı bulunamayanlar için varsayılan alan değeri
            unmatched_with_parcel['avg_area'] = unmatched_with_parcel['avg_area'].fillna(100.0)
            
            # Buffer oluştur
            unmatched_results = []
            for idx, row in unmatched_with_parcel.iterrows():
                try:
                    # Sabit buffer değeri kullan (WGS84 için yaklaşık 10 metre)
                    buffer_degree = 0.00009  # yaklaşık 10 metre
                    buffer = row.geometry.buffer(buffer_degree)
                    
                    # Ada kenarı içinde sınırla (eğer ada kenarı varsa)
                    parcel_geom = None
                    if 'parcel_name' in row and not pd.isna(row['parcel_name']):
                        parcel_matches = ada_kenarlari_with_avg[ada_kenarlari_with_avg['parcel_name'] == row['parcel_name']]
                        if not parcel_matches.empty:
                            parcel_geom = parcel_matches.iloc[0].geometry
                    
                    if parcel_geom is not None:
                        buffer = buffer.intersection(parcel_geom)
                    
                    # Geçerli geometri kontrolü
                    if buffer is not None and buffer.is_valid and not buffer.is_empty:
                        result_row = row.copy()
                        result_row['geometry'] = buffer
                        result_row['area_m2'] = row['avg_area']
                        result_row['match_type'] = 'no_match_buffer'
                        result_row['matched_bina_id'] = None
                        result_row['matched_building_polygon'] = None  # Buffer için poligon yok
                        
                        # İmar parametreleri için varsayılan değerler (null)
                        for param in available_imar_params:
                            result_row[param] = None
                            
                        unmatched_results.append(result_row)
                    
                except Exception as e:
                    print(f"Buffer oluşturma hatası (nokta {idx}): {e}")
            
            # Unmatched buffer points için GeoDataFrame oluştur
            if unmatched_results:
                unmatched_buffer_df = pd.DataFrame(unmatched_results)
                unmatched_buffer_points = gpd.GeoDataFrame(
                    unmatched_buffer_df, 
                    geometry='geometry',
                    crs=all_points.crs
                )
                print(f"No-match buffer tip kontrol: {unmatched_buffer_points['match_type'].value_counts().to_dict()}")
                print(f"Buffer oluşturulan eşleşmeyen nokta sayısı: {len(unmatched_buffer_points)}")
        except Exception as e:
            print(f"Ada eşleştirme hatası: {e}")
    
    # Tüm sonuçları birleştir
    print("Combining all results...")
    result_data = []
    
    # Her parçanın eşleşme tiplerini ve uzunluklarını kontrol et
    if len(direct_matched_points) > 0:
        print(f"Direct match types: {direct_matched_points['match_type'].value_counts().to_dict()}")
        print(f"Adding {len(direct_matched_points)} direct matches")
        # Emin olmak için match_type'ları tekrar atayın
        direct_matched_points['match_type'] = 'direct_match'
        result_data.append(direct_matched_points)
    else:
        print("UYARI: direct_matched_points boş veya eksik!")
    
    if len(buffer_matched_points) > 0:
        print(f"Buffer match types: {buffer_matched_points['match_type'].value_counts().to_dict()}")
        print(f"Adding {len(buffer_matched_points)} buffer matches")
        # Emin olmak için match_type'ları tekrar atayın
        buffer_matched_points['match_type'] = 'buffer_match'
        result_data.append(buffer_matched_points)
    
    if len(unmatched_buffer_points) > 0:
        print(f"No match buffer types: {unmatched_buffer_points['match_type'].value_counts().to_dict()}")
        print(f"Adding {len(unmatched_buffer_points)} unmatched points")
        # Emin olmak için match_type'ları tekrar atayın
        unmatched_buffer_points['match_type'] = 'no_match_buffer'
        result_data.append(unmatched_buffer_points)
    
    # Sonuç yoksa boş GeoDataFrame döndür
    if not result_data:
        print("UYARI: Hiçbir eşleştirme sonucu bulunamadı!")
        empty_gdf = gpd.GeoDataFrame(
            columns=['geometry', 'point_type', 'point_id', 'area_m2', 
                    'match_type', 'matched_bina_id'] + available_imar_params,
            geometry='geometry', 
            crs="EPSG:4326"
        )
        return empty_gdf
    
    # Her veri çerçevesindeki match_type'leri birleştirmeden önce kontrol et
    for i, df in enumerate(result_data):
        match_counts = df['match_type'].value_counts()
        print(f"DataFrame {i} match_type before preparing for concat: {match_counts.to_dict()}")
    
    # Ortak sütunları belirle (imar parametrelerini ekleyerek)
    result_columns = ['geometry', 'point_type', 'point_id', 'area_m2', 
                     'match_type', 'matched_bina_id', 'matched_building_polygon'] + available_imar_params
    
    # Ek sütunları belirle
    additional_columns = []
    for df in result_data:
        for col in df.columns:
            if col not in result_columns and col not in additional_columns and col != 'geometry':
                additional_columns.append(col)
    
    # Her veri çerçevesini kontrol et ve gerekirse düzelt
    filtered_dfs = []
    for i, df in enumerate(result_data):
        # Eşleşme tipini kontrol et
        if 'match_type' in df.columns:
            match_counts = df['match_type'].value_counts()
            print(f"DataFrame {i} match_type counts before filtering: {match_counts.to_dict()}")
        else:
            print(f"WARNING: DataFrame {i} has no match_type column!")
            
        # Kopyalama işlemi
        df_copy = df.copy()
        
        # Eksik sütunları ekle
        for col in result_columns:
            if col not in df_copy.columns and col != 'geometry':
                df_copy[col] = None
        
        # Kullanılacak sütunları filtrele
        cols_to_use = [col for col in result_columns + additional_columns if col in df_copy.columns]
        filtered_df = df_copy[cols_to_use].copy()
        
        # Filtreleme sonrası eşleşme tipini tekrar kontrol et
        if 'match_type' in filtered_df.columns:
            match_counts = filtered_df['match_type'].value_counts()
            print(f"DataFrame {i} match_type counts after filtering: {match_counts.to_dict()}")
            
        filtered_dfs.append(filtered_df)
    
    # Birleştirmeden önce her DataFrame'in eşleşme tiplerini kontrol et
    for i, df in enumerate(filtered_dfs):
        print(f"DataFrame {i} match_type before concat: {df['match_type'].value_counts().to_dict()}")
    
    # Veri çerçevelerini birleştir
    result_gdf = pd.concat(filtered_dfs, ignore_index=True)
     # Concat sonrası eşleşme tiplerini kontrol et
    print(f"Match types after concat: {result_gdf['match_type'].value_counts().to_dict()}")
    
    # Herhangi bir eksik veriyi temizle
    if result_gdf['match_type'].isnull().any():
        print(f"WARNING: {result_gdf['match_type'].isnull().sum()} rows have NULL match_type!")
        # NULL değerleri düzelt
        result_gdf.loc[result_gdf['match_type'].isnull() & result_gdf['matched_bina_id'].notna(), 'match_type'] = 'direct_match'
        result_gdf.loc[result_gdf['match_type'].isnull() & result_gdf['matched_bina_id'].isna(), 'match_type'] = 'no_match_buffer'
    
    print(f"Toplam sonuç sayısı: {len(result_gdf)}")
    print(f"Final match_type counts: {result_gdf['match_type'].value_counts().to_dict()}")
    
    # GeoDataFrame olarak yeniden oluştur
    result_gdf = gpd.GeoDataFrame(result_gdf, geometry='geometry', crs=all_points.crs)
    
    # Eşleşen bina poligonlarını tekrar kontrol et
    print(f"\nBuilding polygons in result dataframe: {result_gdf['matched_building_polygon'].notna().sum()}")
    
    # Merkez noktalarını hesapla
    result_gdf['lon'] = result_gdf.geometry.centroid.x
    result_gdf['lat'] = result_gdf.geometry.centroid.y
    
    # hmax_numeric sütununu oluştur
    result_gdf = ensure_hmax_numeric(result_gdf)
    
    return result_gdf
    
    # Concat sonrası eşleşme tiplerini kontrol et
    
    #############################################
# VERİ ANALİZ FONKSİYONLARI
#############################################

def analyze_imar_data(unique_gdf, selected_region, output_dir='./denemeler'):
    """İmar verilerini analiz eder ve günceller"""
    try:
        print("\n=== İMAR VERİ ANALİZİ BAŞLATILIYOR ===")
        os.makedirs(output_dir, exist_ok=True)
        print(f"Toplam kayıt sayısı: {len(unique_gdf)}")
        print(f"CRS: {unique_gdf.crs}")
        print(f"Sütun sayısı: {len(unique_gdf.columns)}")
        geom_types = unique_gdf.geometry.type.value_counts()
        print("Geometri tipleri dağılımı:", geom_types.to_dict())
        
        # Kesisen_alan_m2 sütununu kontrol et
        if 'kesisen_alan_m2' in unique_gdf.columns:
            print(f"kesisen_alan_m2 sütunu bulundu.")
        else:
            print(f"UYARI: kesisen_alan_m2 sütunu bulunamadı!")
            
        # Overpass ve ada kenarlarını ayır
        overpass_verileri = None
        ada_kenarlari = None
        if 'bina_tipi' in unique_gdf.columns:
            bina_tipi_counts = unique_gdf['bina_tipi'].value_counts()
            print("\nBina Tipi Dağılımı:")
            print(bina_tipi_counts)
            
            # Bina tipi analizini kaydet
            bina_tipi_file = f'{output_dir}/bina_tipi_analiz-{selected_region}.csv'
            bina_tipi_counts.to_csv(bina_tipi_file, encoding='utf-8-sig')
            print(f"Bina tipi analizi kaydedildi: {bina_tipi_file}")
        
        if 'imar_id' in unique_gdf.columns:
            imar_id_counts = unique_gdf['imar_id'].value_counts()
            print("\nİmar ID Dağılımı:")
            print(imar_id_counts)
        if 'building' in unique_gdf.columns:
            building_mask = unique_gdf['building'].notna()
            overpass_verileri = unique_gdf[building_mask].copy()
            ada_kenarlari = unique_gdf[~building_mask].copy()
            print(f"Overpass verileri: {len(overpass_verileri)} kayıt")
            print(f"Ada kenarları: {len(ada_kenarlari)} kayıt")
        elif 'bina_alan_m2' in unique_gdf.columns:
            building_mask = unique_gdf['bina_alan_m2'].notna()
            overpass_verileri = unique_gdf[building_mask].copy()
            ada_kenarlari = unique_gdf[~building_mask].copy()
            print(f"Bina alanı ile filtreleme: {len(overpass_verileri)} bina, {len(ada_kenarlari)} ada")
        else:
            print("UYARI: Bina/ada ayrımı için uygun sütun bulunamadı.")
            overpass_verileri = unique_gdf.copy()
            ada_kenarlari = gpd.GeoDataFrame(columns=unique_gdf.columns, crs=unique_gdf.crs)
        
        # hmax değerlerini temizle
        if 'hmax' in unique_gdf.columns:
            unique_gdf['hmax_numeric'] = unique_gdf['hmax'].apply(clean_hmax)
        
        # Sonuç dosyasını kaydet
        result_file = f'{output_dir}/sonuc-{selected_region}-imar.csv'
        unique_gdf.to_csv(result_file, index=False, encoding='utf-8-sig')
        print(f"Sonuç dosyası kaydedildi: {result_file}")
        return unique_gdf, overpass_verileri, ada_kenarlari
    except Exception as e:
        print(f"Genel analiz hatası: {e}")
        return None, None, None


#############################################
# SONUÇ İŞLEME FONKSİYONLARI
#############################################

def visualize_results(result_gdf, output_dir='./sonuclar'):
    """Eşleştirme sonuçlarını görselleştirir"""
    try:
        os.makedirs(output_dir, exist_ok=True)
        
        # Eşleşme tipine göre görselleştirme
        fig, ax = plt.subplots(figsize=(15, 15))
        result_gdf.plot(ax=ax, column='match_type', categorical=True, 
                       legend=True, alpha=0.7, cmap='viridis')
        plt.title('Kofre-Bina Eşleştirme Sonuçları')
        output_png = os.path.join(output_dir, 'improved_visualization_match_type.png')
        plt.savefig(output_png, dpi=300, bbox_inches='tight')
        plt.close()
        
        # Nokta tipine göre görselleştirme (varsa)
        if 'point_type' in result_gdf.columns:
            fig, ax = plt.subplots(figsize=(15, 15))
            result_gdf.plot(ax=ax, column='point_type', categorical=True, 
                           legend=True, alpha=0.7, cmap='Set2')
            plt.title('Kofre-Bina Eşleştirme Sonuçları (Nokta Tipine Göre)')
            output_png2 = os.path.join(output_dir, 'improved_visualization_point_type.png')
            plt.savefig(output_png2, dpi=300, bbox_inches='tight')
            plt.close()
            print(f"Görselleştirmeler kaydedildi: {output_png} ve {output_png2}")
        else:
            print(f"Görselleştirme kaydedildi: {output_png}")
        
        # Eşleşme istatistiklerini yazdır
        match_stats = result_gdf['match_type'].value_counts()
        print("\nEşleştirme İstatistikleri:")
        print(match_stats)
        
        # Nokta tipine ve eşleşme tipine göre çapraz tablo (varsa)
        if 'point_type' in result_gdf.columns:
            match_by_type = pd.crosstab(result_gdf['point_type'], result_gdf['match_type'])
            print("\nNokta Tipine Göre Eşleşme İstatistikleri:")
            print(match_by_type)
            
        return True
    except Exception as e:
        print(f"Görselleştirme hatası: {e}")
        return False
def ensure_hmax_numeric(gdf):
    """
    GeoDataFrame'de hmax_numeric sütununu oluşturur veya günceller
    """
    print("hmax_numeric sütunu oluşturuluyor/güncelleniyor...")
    
    # clean_hmax fonksiyonunun tanımlı olduğundan emin olun
    if 'hmax' in gdf.columns:
        # İlk birkaç hmax değerini göster
        print("Örnek hmax değerleri:")
        print(gdf['hmax'].head())
        
        # hmax_numeric sütununu oluştur
        gdf['hmax_numeric'] = gdf['hmax'].apply(clean_hmax)
        
        # hmax_numeric sütununun oluşturulduğunu kontrol et
        print(f"hmax_numeric sütunu oluşturuldu mu: {'hmax_numeric' in gdf.columns}")
        if 'hmax_numeric' in gdf.columns:
            print("Örnek hmax_numeric değerleri:")
            print(gdf['hmax_numeric'].head())
    else:
        print("UYARI: hmax sütunu bulunamadı, hmax_numeric oluşturulamadı.")
    
    return gdf

def save_results(result_gdf, output_dir='./sonuclar'):
    """Eşleştirme sonuçlarını kaydeder"""
    try:
        os.makedirs(output_dir, exist_ok=True)
        
        # Önce hmax_numeric'in varlığını kontrol et ve yoksa oluştur
        if 'hmax' in result_gdf.columns and 'hmax_numeric' not in result_gdf.columns:
            print("hmax_numeric sütunu eksik! Şimdi oluşturuluyor...")
            result_gdf = ensure_hmax_numeric(result_gdf)
        
        # Tüm sütunların bir listesini yazdır - debug için
        print("Mevcut tüm sütunlar:")
        print(sorted(result_gdf.columns.tolist()))
        
        # CSV için geometri sütununu WKT formatına dönüştür
        result_for_csv = result_gdf.copy()
        result_for_csv['geometry_wkt'] = result_for_csv.geometry.apply(lambda x: x.wkt if hasattr(x, 'wkt') else str(x))
        
        # Sütun isimlerini standardize et (büyük/küçük harf tutarlılığı için)
        if 'imar_id_forecast' not in result_for_csv.columns and 'IMAR_ID_FORECAST' in result_for_csv.columns:
            result_for_csv['imar_id_forecast'] = result_for_csv['IMAR_ID_FORECAST']
        
        if 'bina_tipi' not in result_for_csv.columns and 'BINA_TIPI' in result_for_csv.columns:
            result_for_csv['bina_tipi'] = result_for_csv['BINA_TIPI']
        
        # İstenen tüm sütunlar - imar_id yerine imar_id_forecast kullanılıyor
        desired_columns = [
            'point_id', 'point_type', 'matched_bina_id', 'match_type', 
            'area_m2', 'matched_building_polygon', 'geometry_wkt',
            'imar_kaks', 'imar_taks', 'hmax', 'emsal', 'TAKS', 'KAKS',
            'imar_id_forecast', 'bina_tipi', 'hmax_numeric'
        ]
        
        # Mevcut olan sütunları belirle
        available_columns = []
        for col in desired_columns:
            if col in result_for_csv.columns:
                available_columns.append(col)
            else:
                print(f"UYARI: '{col}' sütunu veri setinde bulunmuyor!")
        
        # Önemli alanları içeren özet CSV
        imar_output_csv = os.path.join(output_dir, 'imar_analysis_results.csv')
        
        # Sonuçları kaydet
        if available_columns:
            # Önemli! column=available_columns kullanarak sıralama sağlayın
            imar_summary = result_for_csv[available_columns].copy()
            
            # İmar analizi sonuçlarını kaydet
            imar_summary.to_csv(imar_output_csv, index=False, encoding="utf-8-sig")
            print(f"İmar analizi sonuçları kaydedildi: {imar_output_csv}")
            
            # Sütun sayısını ve mevcut olan sütunları yazdır - debug için
            print(f"İmar analizi CSV'de {len(available_columns)} sütun var. Sütunlar: {available_columns}")
        else:
            print("UYARI: İmar analizi için hiçbir istenilen sütun bulunamadı!")
        
        # Geometri sütununu kaldır (CSV'de artık geometry_wkt var)
        result_for_csv = result_for_csv.drop(columns=['geometry'])
        
        # Tüm sütunlarla çıktı dosyası
        output_csv = os.path.join(output_dir, 'improved_polygon_results.csv')
        result_for_csv.to_csv(output_csv, index=False, encoding="utf-8-sig")
        
        # GeoJSON çıktısı
        output_geojson = os.path.join(output_dir, 'improved_polygon_results.geojson')
        result_gdf.to_file(output_geojson, driver='GeoJSON')
        
        print(f"İşlem tamamlandı! Toplam {len(result_gdf)} sonuç kaydedildi.")
        print(f"Sonuçlar kaydedildi: {output_geojson} ve {output_csv}")
        
        return True
    except Exception as e:
        print(f"Sonuçları kaydetme hatası: {e}")
        import traceback
        traceback.print_exc()
        return False

#############################################
# MAIN BÖLÜMÜ
#############################################

if __name__ == "__main__":
    import sys
    if len(sys.argv) < 3:
        print("Kullanım: python imar_matching.py <input_csv_path> <region_name>")
        print("Örnek: python imar_matching.py ./input/unique_gdf.csv İzmir")
        sys.exit(1)
    
    # Komut satırı parametreleri
    input_path = sys.argv[1]
    region_name = sys.argv[2]
    
    if region_name.lower() == "izmir" or region_name.lower() == "i̇zmir":  # Türkçe karakter sorunu için iki kontrol
    # İzmir için GDZ verileri
        mesken_csv = "./deep_learning_data/mesken_data_gdz.csv"
        ticarethane_csv = "./deep_learning_data/other_data_gdz.csv"
        print(f"İzmir bölgesi için GDZ verileri kullanılıyor")
        output_directory = f"./sonuclar/{region_name.lower()}"
    elif region_name.lower() == "eskisehir" or region_name.lower() == "eskişehir":
    # Eskişehir için OEDAS verileri
        mesken_csv = "./deep_learning_data/mesken_data_oedas.csv"
        ticarethane_csv = "./deep_learning_data/other_data_oedas.csv"
        print(f"Eskişehir bölgesi için OEDAS verileri kullanılıyor")
        output_directory = f"./sonuclar/{region_name.lower()}"
    else:
        # Varsayılan değerler
        mesken_csv = "./deep_learning_data/mesken_data_gdz.csv"
        ticarethane_csv = "./deep_learning_data/other_data_gdz.csv"
        print(f"UYARI: {region_name} için tanımlı veri kaynağı bulunamadı, varsayılan GDZ verileri kullanılıyor")
        output_directory = f"./sonuclar/{region_name.lower()}"
    
    print("Program başlıyor")
    print(f"Input CSV: {input_path}")
    print(f"Bölge: {region_name}")
    print(f"Mesken CSV: {mesken_csv}")
    print(f"Ticarethane CSV: {ticarethane_csv}")
    print(f"Çıktı dizini: {output_directory}")
    
    try:
        # Ana CSV dosyasını oku ve geometrileri dönüştür
        print(f"CSV dosyası okunuyor: {input_path}")
        df = pd.read_csv(input_path, encoding='utf-8')
        
        # Geometri sütununu WKT'den dönüştür
        if 'geometry' in df.columns:
            df['geometry'] = df['geometry'].apply(lambda x: wkt.loads(x) if isinstance(x, str) else None)
            gdf = gpd.GeoDataFrame(df, geometry='geometry', crs="EPSG:4326")
        else:
            print("HATA: 'geometry' sütunu bulunamadı.")
            sys.exit(1)
            
        # İmar verileri analizini yap
        analyzed_gdf, overpass_verileri, ada_kenarlari = analyze_imar_data(gdf, region_name)
        
        # Mesken ve ticarethane noktalarını yükle (sabit yollardan)
        points_data = load_point_data_from_csv(mesken_csv, ticarethane_csv)
        
        if points_data is not None and len(points_data) > 0:
            # Bina eşleştirme fonksiyonunu çalıştır
            print("\n=== BİNA EŞLEŞTİRME İŞLEMİ BAŞLATILIYOR ===")
            if overpass_verileri is not None and ada_kenarlari is not None:
                result_gdf = perform_building_matching(points_data, overpass_verileri, ada_kenarlari)
            else:
                print("UYARI: Overpass veya ada kenarları verileri eksik. Doğrudan eşleştirme yapılıyor.")
                result_gdf = perform_building_matching(points_data, gdf, gdf)
            
            # Sonuçları görselleştir ve kaydet
            visualize_results(result_gdf, output_directory)
            save_results(result_gdf, output_directory)
            
            print("Program başarıyla tamamlandı.")
        else:
            # Nokta verisi yoksa analyzed_gdf'yi kullan
            print("UYARI: Nokta verisi yüklenemedi veya boş. İmar verisi üzerinde işlem yapılacak.")
            if 'point_type' not in analyzed_gdf.columns:
                analyzed_gdf['point_type'] = 'mesken'  # Varsayılan değer
            
            print("\n=== BİNA EŞLEŞTİRME İŞLEMİ BAŞLATILIYOR ===")
            if overpass_verileri is not None and ada_kenarlari is not None:
                result_gdf = perform_building_matching(analyzed_gdf, overpass_verileri, ada_kenarlari)
            else:
                print("UYARI: Overpass veya ada kenarları verileri eksik. Doğrudan eşleştirme yapılıyor.")
                result_gdf = perform_building_matching(analyzed_gdf, analyzed_gdf, analyzed_gdf)
            
            # Sonuçları görselleştir ve kaydet
            visualize_results(result_gdf, output_directory)
            save_results(result_gdf, output_directory)
            
            print("Program başarıyla tamamlandı.")
    except Exception as e:
        print(f"Hata: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)