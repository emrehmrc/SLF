#!/usr/bin/env python3
"""
Overpass API Modülü

Bu modül, Overpass API'den veri çekme, işleme ve GeoDataFrame'lere dönüştürme işlemlerini gerçekleştirir.
"""

import json
import logging
import os
from typing import Dict, List, Any, Optional

import pandas as pd
import requests
import geopandas as gpd
from shapely.geometry import Point, Polygon
from pyproj import Transformer

# Günlük kaydı yapılandırması
logger = logging.getLogger(__name__)

def _build_query_groups(area_name: str) -> Dict[str, str]:
    """
    Farklı veri kategorileri için Overpass API sorgu grupları oluşturur.
    
    Args:
        area_name: Sorgulanacak alan adı.
        
    Returns:
        Kategori adları anahtarları ve sorgu metinleri değerleri olan sözlük.
    """
    queries = {
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
    
    # Diğer sorgu grupları aynı kalabilir
    # ...
    }
    
    return queries

def replace_node_ids_with_objects(data: Dict[str, Any]) -> Dict[str, Any]:
    """
    Overpass API yanıtındaki node ID'lerini gerçek node nesneleriyle değiştirir.
    
    Args:
        data: Overpass API yanıt verisi.
        
    Returns:
        Node ID'leri node nesneleriyle değiştirilmiş işlenmiş veri.
    """
    node_dict = {}
    
    # Tüm node'ları node_dict sözlüğüne ekle
    for element in data["elements"]:
        if element["type"] == "node": 
            node_dict[element["id"]] = element
    
    # 'Way' türündeki elementler için node ID'lerini node nesneleriyle değiştir
    for element in data["elements"]:
        if element["type"] == "way":
            node_list = element.get("nodes", [])
            updated_node_list = []
            for node_id in node_list:
                if node_id in node_dict:
                    updated_node_list.append(node_dict[node_id])
                else:
                    updated_node_list.append({"id": node_id, "lat": None, "lon": None})  # Eksik node'lar için varsayılan değerler
            element["nodes"] = updated_node_list
    return data

def calculate_area_m2(coords: List[tuple]) -> Optional[float]:
    """
    Bir poligonun metrekare cinsinden alanını hesaplar.
    
    Args:
        coords: (lon, lat) koordinat çiftleri listesi.
        
    Returns:
        Metrekare cinsinden alan veya hesaplama başarısız olursa None.
    """
    if len(coords) < 3:
        logger.warning("Error: A polygon requires at least 3 distinct coordinates.")
        return None

    if coords[0] != coords[-1]:
        coords.append(coords[0])

    try:
        # Poligonu oluştur ve alanı hesaplamak için UTM'e dönüştür
        polygon = Polygon(coords)
        transformer = Transformer.from_crs("epsg:4326", "epsg:32635", always_xy=True)
        utm_coords = [transformer.transform(lon, lat) for lon, lat in coords]
        utm_polygon = Polygon(utm_coords)
        return utm_polygon.area
    except Exception as e:
        logger.warning(f"An error occurred while calculating area: {e}")
        return None
    

def get_polygon_coords(element: Dict[str, Any]) -> List[tuple]:
    """
    Bir 'way' elementinden koordinat çiftlerini çıkarır.
    
    Args:
        element: Overpass API element sözlüğü.
        
    Returns:
        (lon, lat) koordinat çiftleri listesi.
    """
    if element['type'] == 'way':
        coords = [(node.get('lon'), node.get('lat')) for node in element.get('nodes', []) 
                 if node.get('lon') is not None and node.get('lat') is not None]
        return coords
    return []

def createGeo(element: Dict[str, Any]) -> Any:
    """
    Bir Overpass API elementinden Shapely geometri nesnesi oluşturur.
    
    Args:
        element: Overpass API element sözlüğü.
        
    Returns:
        Shapely Point veya Polygon nesnesi, veya geometri oluşturulamazsa None.
    """
    if element['type'] == 'node':
        # Node'lar için Point geometrisi oluştur
        if 'lat' in element and 'lon' in element:
            return Point(element['lon'], element['lat'])
        else:
            return None  # Koordinatlar eksikse None döndür
    
    elif element['type'] == 'way':
        # Way'ler için Polygon geometrisi oluştur
        coords = get_polygon_coords(element)  # Koordinatları almak için fonksiyonu kullan
        if len(coords) >= 3:  # Bir poligon en az 3 nokta gerektirir
            return Polygon(coords)
        else:
            return None  # Yeterli koordinat yoksa None döndür

    return None  # Tip 'node' veya 'way' değilse varsayılan olarak None döndür

def process_data(data: Dict[str, Any], area_name: str, output_file: Optional[str] = None) -> gpd.GeoDataFrame:
    """
    Overpass API verilerini bir GeoDataFrame'e işler.
    
    Args:
        data: Overpass API yanıt verisi.
        area_name: İşlem yapılan alan adı.
        output_file: Sonuçların kaydedileceği dosya yolu (opsiyonel).
        
    Returns:
        İşlenmiş veriyi içeren GeoDataFrame.
    """
    data_rows = []
    for element in data['elements']:
        coords = get_polygon_coords(element) if element['type'] == 'way' else []

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
            'area_m2': calculate_area_m2(coords) if coords else None,
            'tags': json.dumps(element.get('tags', {}))  # Tüm tags'leri JSON olarak sakla
        }

        # Way türündeki elementler için ortalama koordinat hesapla
        if element['type'] == 'way' and coords:
            latitudes = [lat for _, lat in coords]
            longitudes = [lon for lon, _ in coords]
            row['lat'] = sum(latitudes) / len(latitudes) if latitudes else None
            row['lon'] = sum(longitudes) / len(longitudes) if longitudes else None

        # İlgilenilen alanlardan biri boş değilse veriyi ekle ve geometri varsa
        if row['geometry'] is not None and any(row[key] for key in ['amenity', 'landuse', 'shop', 'office', 'building', 'tourism', 'healthcare', 'leisure', 'levels']):
            data_rows.append(row)
    
    # Veri yoksa boş GeoDataFrame döndür
    if not data_rows:
        return gpd.GeoDataFrame(geometry=[], crs="EPSG:4326")
    
    # GeoDataFrame oluşturma
    df = gpd.GeoDataFrame(data_rows, geometry='geometry', crs="EPSG:4326")
    df = df.drop_duplicates(subset=['name', 'geometry'], keep='first')
    
    # Veriyi dosyaya kaydet (eğer belirtilmişse)
    if output_file:
        os.makedirs(os.path.dirname(output_file), exist_ok=True)
        df.to_csv(output_file, index=False) 
        logger.info(f"Overpass verisi dosyaya kaydedildi: {output_file}")
    
    return df

def fetch_and_process_data(area_name: str, output_file: Optional[str] = None) -> gpd.GeoDataFrame:
    """
    Overpass API'den veri çeker ve bir GeoDataFrame'e işler.
    
    Args:
        area_name: Overpass API'de sorgulanacak alan adı.
        output_file: Sonuçların kaydedileceği dosya yolu (opsiyonel).
        
    Returns:
        İşlenmiş OSM verilerini içeren GeoDataFrame.
    """
    overpass_url = "http://overpass-api.de/api/interpreter"
    # Farklı kategorilerdeki veriler için sorguları tanımla
    queries_grouped = _build_query_groups(area_name)
    
    # Tüm sorgulardan veri çek ve birleştir
    combined_results = []
    for key, query in queries_grouped.items():
        logger.info(f"Overpass sorgusu yürütülüyor: {key}")
        response = requests.post(overpass_url, data={'data': query})
        if response.status_code == 200:
            try:
                data = json.loads(response.text)
                processed_data = replace_node_ids_with_objects(data)
                combined_results.extend(processed_data['elements'])
                logger.info(f"{key} sorgusu {len(processed_data['elements'])} öğe döndürdü")
            except json.JSONDecodeError:
                logger.error(f"Sorguda geçersiz JSON yanıtı: {key}")
        else:
            logger.error(f"{key} için veri çekilemedi: Durum kodu {response.status_code}")

    # Tüm toplanan verileri işle
    final_data = {
        'elements': combined_results
    }
    processed_final_data = process_data(final_data, area_name, output_file)
    
    return processed_final_data

def xml_safe_text(text: str) -> str:
    """
    Metni XML için güvenli hale getirir, özel karakterleri kaçırır.
    
    Args:
        text: İşlenecek metin.
        
    Returns:
        XML için güvenli hale getirilmiş metin.
    """
    return text.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace("\"", "&quot;").replace("'", "&apos;")

# Bu modül doğrudan çalıştırılırsa
if __name__ == "__main__":
    # Konsol günlük kaydını ayarla
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s - %(name)s - %(levelname)s - %(message)s"
    )
    
    # Komut satırı argümanlarını kontrol et
    import sys
    if len(sys.argv) < 2:
        print("Kullanım: python imar_overpass.py <alan_adı> [çıktı_dosyası]")
        sys.exit(1)
    
    area_name = sys.argv[1]
    output_file = sys.argv[2] if len(sys.argv) > 2 else f"overpass-data-{area_name}.csv"
    
    print(f"Alan için veri çekiliyor: {area_name}")
    
    result = fetch_and_process_data(area_name, output_file)
    print(f"{len(result)} kayıt işlendi")
    print(f"Sonuç dosya yolu: {output_file}")