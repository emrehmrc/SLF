#!/usr/bin/env python3
"""
KML Parsing ve Analiz Modülü (DB'siz Versiyon)

Bu versiyon, KML dosyasından gelen verileri GeoDataFrame'e çevirir,
TAKS/KAKS/hmax/emsal gibi bilgileri parse eder ama database bağlantısı kullanmaz.
"""

import os
import logging
import re
from typing import Any, Optional, Tuple

import geopandas as gpd
import pandas as pd
import xml.etree.ElementTree as ET
from shapely.geometry import Point, Polygon, LineString

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# def extract_kaks_taks(text: Any) -> Tuple[Optional[float], Optional[float], Optional[str]]:
#     """
#     Metinden KAKS ve TAKS değerlerini ayıklar.
#     Format örnekleri:
#     - "0.4" -> Sadece bir değer varsa, bu durumda KAKS için kullanılır, TAKS None olur
#     - "KAKS:0.4, TAKS:0.2" -> Her iki değer de belirtilmiş
#     - "K:0.4, T:0.2" -> Kısaltmalar kullanılmış
#     - "0.4/0.2" -> KAKS/TAKS formatı
#     """
#     text = str(text).lower().strip()
    
#     # Sadece tek bir sayı ise (0.4 gibi)
#     if re.match(r'^\d+(?:[,.]\d+)?$', text):
#         value = float(text.replace(',', '.'))
#         return (value, None, None)  # Sadece KAKS değeri var, TAKS None
    
#     # KAKS:0.4, TAKS:0.2 veya K:0.4, T:0.2 formatı
#     kaks_match = re.search(r'(?:kaks|k)[:\s]*(\d+(?:[,.]\d+)?)', text)
#     taks_match = re.search(r'(?:taks|t)[:\s]*(\d+(?:[,.]\d+)?)', text)
    
#     kaks = float(kaks_match.group(1).replace(',', '.')) if kaks_match else None
#     taks = float(taks_match.group(1).replace(',', '.')) if taks_match else None
    
#     # 0.4/0.2 formatı (KAKS/TAKS)
#     if kaks is None and taks is None:
#         slash_match = re.search(r'(\d+(?:[,.]\d+)?)/(\d+(?:[,.]\d+)?)', text)
#         if slash_match:
#             kaks = float(slash_match.group(1).replace(',', '.'))
#             taks = float(slash_match.group(2).replace(',', '.'))
    
#     # Herhangi bir değer bulunamadıysa
#     if kaks is None and taks is None:
#         return (None, None, text)
    
#     return (kaks, taks, None)

def kml_to_geodataframe_check(kml_file_path: str, selected_region: str, output_file: Optional[str] = None) -> Tuple[gpd.GeoDataFrame, gpd.GeoDataFrame]:
    """
    KML dosyasını okur ve GeoDataFrame'e dönüştürür.
    """
    print(f"KML dosyası işleniyor: {kml_file_path}")
    print(f"Seçili bölge: {selected_region}")
    
    tree = ET.parse(kml_file_path)
    root = tree.getroot()

    geometries, geometry_types, names, style_urls, coords_list = [], [], [], [], []

    # KML öğelerini işle
    for placemark in root.findall('.//{http://www.opengis.net/kml/2.2}Placemark'):
        name = placemark.find('.//{http://www.opengis.net/kml/2.2}name')
        style_url = placemark.find('.//{http://www.opengis.net/kml/2.2}styleUrl')
        coordinates_element = placemark.find('.//{http://www.opengis.net/kml/2.2}coordinates')
        
        if coordinates_element is None or coordinates_element.text.strip() == "":
            continue
        
        coords = [tuple(map(float, coord.split(',')[:2])) for coord in re.sub(r'\s+', ' ', coordinates_element.text.strip()).split()]
        coords_list.append(coords)

        if len(coords) == 1:
            geometries.append(Point(coords[0]))
            geometry_types.append('Point')
        elif len(coords) == 2:
            geometries.append(LineString(coords))
            geometry_types.append('LineString')
        elif len(coords) >= 3:
            geometries.append(Polygon(coords))
            geometry_types.append('Polygon')
        else:
            continue

        style_urls.append(style_url.text if style_url is not None else "No styleUrl")
        names.append(name.text if name is not None else (style_url.text if style_url is not None else "No name"))

    # GeoDataFrame oluştur
    gdf = gpd.GeoDataFrame({
        'name': names,
        'styleUrl': style_urls,
        'geometry': geometries,
        'geometry_type': geometry_types,
        'coords': [str(c) for c in coords_list]
    }, crs="EPSG:4326")
    
    print(f"GeoDataFrame oluşturuldu, {len(gdf)} öğe içeriyor")
    
    return separate_polygons(gdf)
def normalize_coords(coords) -> str:
    """
    Koordinatları normalize ederek string olarak döndürür
    """
    try:
        if isinstance(coords, str):
            coords = eval(coords)
    except Exception:
        return str(coords).strip().replace(" ", "")
    
    normalized = sorted([(round(x, 5), round(y, 5)) for x, y in coords])
    return str(normalized)
def debug_sm_points(points_df, style_prefix, label):
    """
    SM noktalarını debug amaçlı görüntülemek için yardımcı fonksiyon
    """
    filtered = points_df[points_df['styleUrl'].str.lower().str.startswith(style_prefix.lower())]
    print(f"\n{label} noktaları ({len(filtered)}):")
    
    if not filtered.empty:
        print(f"İlk 5 {label} noktası:")
        for i, (_, row) in enumerate(filtered.head(5).iterrows()):
            print(f"  {i+1}. name: {row['name']}, styleUrl: {row['styleUrl']}")
    else:
        print(f"   (Hiç {label} noktası bulunamadı)")
    
    return filtered

def extract_kaks_taks_values(sm_polygons, kak_points, tak_points, debug=True):
    """
    SM poligonlarından KAKS ve TAKS değerlerini ayrı ayrı çıkarır.
    """
    if debug:
        print("\n--- KAKS ve TAKS değerleri ayıklanıyor ---")
    
    # Kesişimleri hesapla
    kak_inter = gpd.overlay(sm_polygons, kak_points, how='intersection', keep_geom_type=False)
    tak_inter = gpd.overlay(sm_polygons, tak_points, how='intersection', keep_geom_type=False)
    
    if debug:
        print(f"SM-KAK kesişim sayısı: {len(kak_inter)}")
        print(f"SM-TAK kesişim sayısı: {len(tak_inter)}")
    
    # KAKS değerlerini topla
    kaks_dict = {}
    for idx, row in kak_inter.iterrows():
        if pd.notna(row.get('name_2')):
            name_val = str(row['name_2'])
            # Sayısal değer kontrolü
            if re.match(r'^\d+(?:[,.]\d+)?$', name_val):
                key = normalize_coords(row['coords_1'])
                value = float(name_val.replace(',', '.'))
                kaks_dict[key] = value
                if debug:
                    print(f"KAKS değeri: {key[:20]}... = {value}")
    
    # TAKS değerlerini topla
    taks_dict = {}
    for idx, row in tak_inter.iterrows():
        if pd.notna(row.get('name_2')):
            name_val = str(row['name_2'])
            # Sayısal değer kontrolü
            if re.match(r'^\d+(?:[,.]\d+)?$', name_val):
                key = normalize_coords(row['coords_1'])
                value = float(name_val.replace(',', '.'))
                taks_dict[key] = value
                if debug:
                    print(f"TAKS değeri: {key[:20]}... = {value}")
    
    if debug:
        print(f"Toplam KAKS değeri: {len(kaks_dict)}")
        print(f"Toplam TAKS değeri: {len(taks_dict)}")
    
    return kaks_dict, taks_dict

def separate_polygons(gdf: gpd.GeoDataFrame) -> Tuple[gpd.GeoDataFrame, gpd.GeoDataFrame]:
    """
    GeoDataFrame'i farklı katmanlara ayırır ve hem sonuç hem pl_polygons döndürür.
    KAKS ve TAKS değerlerinin doğru işlenmesini sağlar.
    """
    print("\n--- separate_polygons başlatılıyor ---")
    
    # Geometri türlerine göre ayırma
    polygons = gdf[gdf['geometry_type'] == 'Polygon']
    points = gdf[gdf['geometry_type'] == 'Point']
    
    print(f"Toplam poligon sayısı: {len(polygons)}")
    print(f"Toplam nokta sayısı: {len(points)}")

    # styleUrl ile filtreleme
    sm_polygons = polygons[polygons['styleUrl'].str.lower().str.startswith('#sm')]
    pl_polygons = polygons[polygons['styleUrl'].str.lower().str.startswith('#pl')]
    
    print(f"SM Poligon sayısı: {len(sm_polygons)}")
    print(f"PL Poligon sayısı: {len(pl_polygons)}")
    
    # Debug: SM poligonları ve nokta verilerini incele
    if not sm_polygons.empty:
        print("\nSM Poligonları (ilk 3):")
        for i, (_, row) in enumerate(sm_polygons.head(3).iterrows()):
            print(f"  {i+1}. name: {row['name']}, styleUrl: {row['styleUrl']}")
    
    # Nokta gruplarını filtrele ve debug bilgilerini göster
    sm_hm = debug_sm_points(points, '#sm_hm', 'SM_HM')
    sm_em = debug_sm_points(points, '#sm_em', 'SM_EM')
    sm_kak = debug_sm_points(points, '#sm_kak', 'SM_KAK')
    sm_tak = debug_sm_points(points, '#sm_tak', 'SM_TAK')
    
    # ÖNEMLİ: KAKS ve TAKS değerlerini ayrı ayrı çıkar
    # Bu her bir SM poligonu için farklı KAKS ve TAKS değerleri almasını sağlar
    kaks_dict, taks_dict = extract_kaks_taks_values(sm_polygons, sm_kak, sm_tak)
    
    # Yükseklik ve emsal kesişimleri
    print("\nYükseklik ve emsal kesişimleri hesaplanıyor...")
    hm_intersections = gpd.overlay(pl_polygons, sm_hm, how='intersection', keep_geom_type=False)
    em_intersections = gpd.overlay(pl_polygons, sm_em, how='intersection', keep_geom_type=False)
    
    # KAKS/TAKS noktalarını birleştir
    sm_kk = pd.concat([sm_kak, sm_tak], ignore_index=True)
    inter_1 = gpd.overlay(sm_polygons, sm_kk, how='intersection', keep_geom_type=False)
    
    # PL poligonlarla kesişimler
    print("\nPL poligonlarla kesişimler hesaplanıyor...")
    inter_4 = gpd.overlay(pl_polygons, inter_1, how='intersection', keep_geom_type=False)
    inter_5 = gpd.overlay(pl_polygons, inter_1, how='difference', keep_geom_type=False)
    
    print(f"PL-inter_1 kesişim sayısı: {len(inter_4)}")
    print(f"PL-inter_1 fark sayısı: {len(inter_5)}")
    
    # Yükseklik ve emsal değerlerini sözlüklere dönüştür
    hmax_values = {normalize_coords(row['coords_1']): row['name_2'] 
                   for _, row in hm_intersections.iterrows() 
                   if pd.notna(row.get('name_2'))}
    
    emsal_values = {normalize_coords(row['coords_1']): row['name_2'] 
                    for _, row in em_intersections.iterrows() 
                    if pd.notna(row.get('name_2'))}
    
    print(f"Toplam Hmax değeri sayısı: {len(hmax_values)}")
    print(f"Toplam Emsal değeri sayısı: {len(emsal_values)}")
    
    # Helper function to add hmax and emsal values
    def add_columns(row):
        coords = ""
        if 'coords_1' in row and pd.notna(row['coords_1']):
            coords = normalize_coords(row['coords_1'])
        elif 'coords' in row and pd.notna(row['coords']):
            coords = normalize_coords(row['coords'])
        
        hmax = hmax_values.get(coords, '')
        emsal = emsal_values.get(coords, '')
        
        # Name_2 bilgisini kontrol et ve kaydet (SM noktasının name'i)
        imar_name = row.get('name_2', '')
        if pd.isna(imar_name):
            imar_name = ''
        
        return pd.Series({
            'hmax': hmax,
            'emsal': emsal,
            'imar_name': imar_name
        })
    
    # ÖNEMLİ FIX: İmar değerleri atama fonksiyonu
    def assign_imar_values(row):
        # Eğer imar_name varsa, bu değeri işle
        if pd.notna(row.get('name_2')):
            imar_name = str(row['name_2'])
            
            # "0 60" formatını kontrol et
            parts = imar_name.split()
            if len(parts) == 2:
                try:
                    # İlk sayı KAKS, ikinci sayı TAKS
                    kaks = float(parts[0]) if parts[0].replace('.', '', 1).isdigit() else None
                    taks = float(parts[1]) if parts[1].replace('.', '', 1).isdigit() else None
                    
                    # Sonuçları döndür
                    if kaks is not None or taks is not None:
                        return pd.Series({'imar_kaks': kaks, 'imar_taks': taks})
                except (ValueError, IndexError):
                    pass
        
        # Buraya geldiyse, normal sözlüklerden değerleri bul
        kaks = kaks_dict.get(normalize_coords(row.get('coords_1', '')))
        taks = taks_dict.get(normalize_coords(row.get('coords_1', '')))
        
        return pd.Series({'imar_kaks': kaks, 'imar_taks': taks})
    
    # Sütunları ekle
    print("\nHmax, emsal ve imar_name ekleniyor...")
    inter_4[['hmax', 'emsal', 'imar_name']] = inter_4.apply(add_columns, axis=1)
    inter_5[['hmax', 'emsal', 'imar_name']] = inter_5.apply(add_columns, axis=1)
    
    # KAKS ve TAKS değerlerini atayalım
    print("\nKAKS ve TAKS değerleri atanıyor...")
    inter_4[['imar_kaks', 'imar_taks']] = inter_4.apply(assign_imar_values, axis=1)
    inter_5[['imar_kaks', 'imar_taks']] = inter_5.apply(assign_imar_values, axis=1)
    
    # Sonuçları birleştir
    result = pd.concat([inter_4, inter_5], ignore_index=True)
    print(f"Birleştirilmiş sonuç: {len(result)} satır")
    
    # Geometri oluştur
    print("\nGeometri oluşturuluyor...")
    result['geometry'] = result.apply(
        lambda row: row['geometry'] if 'geometry' in row and pd.notna(row['geometry']) else (
            Polygon(eval(row['coords'])) if 'coords' in row and pd.notna(row['coords']) else None
        ),
        axis=1
    )
    
    # Geçersiz geometrileri filtrele
    result = result[result['geometry'].notna()]
    print(f"Geçerli geometriye sahip satır sayısı: {len(result)}")
    
    # Gerekli sütunları seç
    final_columns = [
        'name', 'styleUrl', 'geometry', 
        'imar_kaks', 'imar_taks', 
        'hmax', 'emsal', 
        'imar_name'
    ]
    
    # Eksik sütunları ekle
    for col in final_columns:
        if col not in result.columns:
            result[col] = None
    
    # Son DataFrame'i oluştur
    processed_result = result[final_columns]
    
    # İstatistikler
    non_null_kaks = processed_result['imar_kaks'].notna().sum()
    non_null_taks = processed_result['imar_taks'].notna().sum()
    
    print(f"\nSonuç istatistikleri:")
    print(f"Toplam satır sayısı: {len(processed_result)}")
    print(f"KAKS değeri olan satır sayısı: {non_null_kaks}")
    print(f"TAKS değeri olan satır sayısı: {non_null_taks}")
    
    # Sonuç örnekleri
    if not processed_result.empty:
        print("\nİşlenmiş sonuçtan örnekler (ilk 5):")
        for i, (_, row) in enumerate(processed_result.head(5).iterrows()):
            print(f"  {i+1}. name: {row['name']}")
            print(f"      styleUrl: {row['styleUrl']}")
            print(f"      imar_kaks: {row['imar_kaks']}")
            print(f"      imar_taks: {row['imar_taks']}")
            print(f"      hmax: {row['hmax']}")
            print(f"      emsal: {row['emsal']}")
            print(f"      imar_name: {row['imar_name']}")
    
    # CSV olarak kaydet
    processed_result.to_csv("taks_kaks_deneme.csv", index=False)
    print("CSV kaydedildi: taks_kaks_deneme.csv")
    
    # pl_polygons da döndür (gerekli sütunları seçerek)
    pl_cols = ['name', 'styleUrl', 'geometry', 'coords'] 
    pl_polygons_result = pl_polygons[pl_cols].copy() if all(col in pl_polygons.columns for col in pl_cols) else pl_polygons.copy()
    
    return processed_result, pl_polygons_result
    
def process_geo_dataframe_for_polygons(geo_df: gpd.GeoDataFrame) -> gpd.GeoDataFrame:
    """
    GeoDataFrame içinde poligonlar için imar verilerini işler.
    """
    # Geometry zaten varsa tekrar oluşturma
    if geo_df.geometry.isna().any() and 'coords' in geo_df.columns:
        geo_df['geometry'] = geo_df['coords'].apply(
            lambda x: Polygon(eval(x)) if len(eval(x)) >= 3 else None
        )

    geo_df = geo_df[geo_df.geometry.notna()].copy()

    # Koordinat grubunu tanımla
    geo_df['coords_group'] = geo_df['coords'].astype(str)

    # Her satırdan imar verisini çıkar
    def parse_imar(row):
        name2 = str(row.get('name_2', ''))
        style2 = str(row.get('styleUrl_2', ''))
        combined = f"{name2} {style2}"
        
        # extract_kaks_taks yerine ayrı bir mantık kullanalım
        # Önce KAKS ve TAKS belirteçlerini arayalım
        kaks_match = re.search(r'kaks[^\d]*(\d+(?:[,.]\d+)?)', combined, re.IGNORECASE)
        taks_match = re.search(r'taks[^\d]*(\d+(?:[,.]\d+)?)', combined, re.IGNORECASE)
        
        kaks = float(kaks_match.group(1).replace(',', '.')) if kaks_match else None
        taks = float(taks_match.group(1).replace(',', '.')) if taks_match else None
        
        # Sadece sayısal bir değer var mı?
        if kaks is None and taks is None:
            match = re.search(r'(\d+(?:[,.]\d+)?)', combined)
            if match:
                # Sayısal değeri al, ama bir sonraki adımda KAKS olarak kullanılacak
                value = float(match.group(1).replace(',', '.'))
                kaks = value
                # TAKS değerini boş bırak, gruplamadaki mantık kullanılacak
                taks = None
        return pd.Series({'imar_kaks': kaks, 'imar_taks': taks})

    # Satırları işle
    imar_df = geo_df.apply(parse_imar, axis=1)
    geo_df = pd.concat([geo_df, imar_df], axis=1)

    # Aynı koordinatlar için maksimum KAKS, minimum TAKS seç
    grouped = geo_df.groupby('coords_group')
    for coords, group in grouped:
        valid_kaks = group.dropna(subset=['imar_kaks'])
        valid_taks = group.dropna(subset=['imar_taks'])
        
        if not valid_kaks.empty:
            geo_df.loc[group.index, 'imar_kaks'] = valid_kaks['imar_kaks'].max()
        
        if not valid_taks.empty:
            geo_df.loc[group.index, 'imar_taks'] = valid_taks['imar_taks'].min()

    # Gerekli kolonları filtrele
    final_cols = ['name', 'styleUrl', 'geometry', 'imar_kaks', 'imar_taks']
    if 'hmax' in geo_df.columns:
        final_cols.append('hmax')
    if 'emsal' in geo_df.columns:
        final_cols.append('emsal')

    return geo_df[final_cols].copy()