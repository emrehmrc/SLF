#!/usr/bin/env python3
"""
İmar Veri İşleme Modülü

Bu modül, katmanları ayırma, verileri analiz etme ve KML'e dönüştürme işlemlerini gerçekleştirir.
"""

import os
import re
import logging
import xml.etree.ElementTree as ET
from typing import Dict, List, Any, Optional, Tuple, Union

import pandas as pd
import numpy as np
import geopandas as gpd
from shapely.geometry import Point, Polygon,MultiPolygon
from shapely import wkt
from shapely.ops import unary_union

# Günlük kaydı yapılandırması
logger = logging.getLogger(__name__)
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
def createKml(gdf: gpd.GeoDataFrame, output_file: Optional[str] = None, selected_region: Optional[str] = None) -> str:
    """
    GeoDataFrame'den bir KML dosyası oluşturur.
    
    Args:
        gdf: KML'e dönüştürülecek GeoDataFrame.
        output_file: Çıktı dosyasının yolu (opsiyonel).
        selected_region: Bölge adı (opsiyonel, dosya adı için).
        
    Returns:
        Oluşturulan KML dosyasının yolu.
    """
    logger.info("KML dosyası oluşturuluyor...")
    
    kml = ET.Element("kml")
    kml.set("xmlns", "http://www.opengis.net/kml/2.2")
    document = ET.SubElement(kml, "Document")
    
    # Doküman adı ekle
    if selected_region:
        name_element = ET.SubElement(document, "name")
        name_element.text = f"İmar Analiz - {selected_region}"

    # Stiller için tanımlamalar
    style_colors = {
        "#Ticarethane": "ff4763ff",   # FF6347 (Tomato) - aabbggrr formatında
        "#Yasakli Alan": "ffb48246",  # 4682B4 (SteelBlue)
        "#Mesken": "ff32cd32",        # 32CD32 (LimeGreen)
        "#Sanayi": "ff00d7ff",        # FFD700 (Gold)
        "#Kentsel Dönüşüm": "ffe22b8a", # 8A2BE2 (BlueViolet)
        "#Tarimsal Sulama": "ff20a5da", # DAA520 (GoldenRod)
        "#Kamu Hizmeti": "ff0045ff",  # FF4500 (OrangeRed)
        "#Kamu Tesisi": "ffd4ff7f",   # 7FFFD4 (Aquamarine)
        "#PL_KONUT": "ff32cd32",      # LimeGreen - Eski formatlar
        "#PL_GELISME_KONUT": "ff32cd32" # LimeGreen - Eski formatlar
    }
    
    # Stil tanımlamaları ekle
    for style_id, color in style_colors.items():
        style = ET.SubElement(document, "Style")
        style.set("id", style_id.replace("#", ""))
        
        poly_style = ET.SubElement(style, "PolyStyle")
        color_element = ET.SubElement(poly_style, "color")
        color_element.text = color
        
        outline = ET.SubElement(poly_style, "outline")
        outline.text = "1"
        
        fill = ET.SubElement(poly_style, "fill")
        fill.text = "1"

    for i, row in gdf.iterrows():
        placemark = ET.SubElement(document, "Placemark")

        # styleUrl bilgisi ekleniyor
        style_url = ET.SubElement(placemark, "styleUrl")
        style_url.text = row.get("styleUrl", "")

        # name bilgisi ekleniyor - eksik veya NaN ise boş bırak
        name = ET.SubElement(placemark, "name")
        name_value = ""
        if "name_1" in row and pd.notna(row["name_1"]):
            name_value = str(row["name_1"])
        elif "name" in row and pd.notna(row["name"]):
            name_value = str(row["name"])
        name.text = name_value

        # description etiketini kaldırıyoruz, verilerin tamamı ExtendedData içinde tutulacak
        # Bu satırı kaldırıldı: 
        # if "alan_bilgisi" in row and pd.notna(row["alan_bilgisi"]):
        #     description = ET.SubElement(placemark, "description")
        #     description.text = str(row["alan_bilgisi"])

        # Geometry tipi ve koordinatlar ekleniyor
        geometry = row["geometry"]
        if isinstance(geometry, Point):
            point = ET.SubElement(placemark, "Point")
            coordinates = ET.SubElement(point, "coordinates")
            coordinates.text = f"{geometry.x},{geometry.y}"  # Z değeri olmadan
        elif isinstance(geometry, Polygon):
            polygon = ET.SubElement(placemark, "Polygon")
            outer_boundary = ET.SubElement(polygon, "outerBoundaryIs")
            linear_ring = ET.SubElement(outer_boundary, "LinearRing")
            coordinates = ET.SubElement(linear_ring, "coordinates")
            
            # Dış sınır koordinatlarını ekle (Z değeri eklenir)
            exterior_coords = []
            for x, y in geometry.exterior.coords:
                exterior_coords.append(f"{x},{y}")  # Z değeri olmadan
            coordinates.text = " ".join(exterior_coords)
            
            # İç sınırlar (delikler) varsa onları da ekle
            for interior in geometry.interiors:
                inner_boundary = ET.SubElement(polygon, "innerBoundaryIs")
                inner_linear_ring = ET.SubElement(inner_boundary, "LinearRing")
                inner_coordinates = ET.SubElement(inner_linear_ring, "coordinates")
                
                interior_coords = []
                for x, y in interior.coords:
                    interior_coords.append(f"{x},{y}")  # Z değeri olmadan
                inner_coordinates.text = " ".join(interior_coords)
                
        # MultiPolygon desteği ekle
        elif isinstance(geometry, MultiPolygon):
            multi_geometry = ET.SubElement(placemark, "MultiGeometry")
            
            for poly in geometry.geoms:
                polygon = ET.SubElement(multi_geometry, "Polygon")
                outer_boundary = ET.SubElement(polygon, "outerBoundaryIs")
                linear_ring = ET.SubElement(outer_boundary, "LinearRing")
                coordinates = ET.SubElement(linear_ring, "coordinates")
                
                # Dış sınır koordinatlarını ekle (Z değeri eklenir)
                exterior_coords = []
                for x, y in poly.exterior.coords:
                    exterior_coords.append(f"{x},{y}")  # Z değeri olmadan
                coordinates.text = " ".join(exterior_coords)
                
                # İç sınırlar (delikler) varsa onları da ekle
                for interior in poly.interiors:
                    inner_boundary = ET.SubElement(polygon, "innerBoundaryIs")
                    inner_linear_ring = ET.SubElement(inner_boundary, "LinearRing")
                    inner_coordinates = ET.SubElement(inner_linear_ring, "coordinates")
                    
                    interior_coords = []
                    for x, y in interior.coords:
                        interior_coords.append(f"{x},{y},0")  # Z değerini ekledik (0)
                    inner_coordinates.text = " ".join(interior_coords)
        
        # ExtendedData bilgileri ekleniyor - Tüm sütunlar attribute olarak ekleniyor
        extended_data = ET.SubElement(placemark, "ExtendedData")
        for col in gdf.columns:
            # geometry hariç tüm sütunları ekle
            if col != "geometry":
                # NaN veya None değerlerini boş string olarak kaydet
                value_text = ""
                if pd.notna(row[col]):
                    value_text = str(row[col])
                
                data = ET.SubElement(extended_data, "Data")
                data.set("name", col)
                value = ET.SubElement(data, "value")
                value.text = value_text
        
        # Eski formatta her zaman bir Style elemanı ekliyoruz
        inline_style = ET.SubElement(placemark, "Style")
        poly_style = ET.SubElement(inline_style, "PolyStyle")
        color = ET.SubElement(poly_style, "color")
        
        # Renk formatını KML için uygun hale getir
        fill_color = ""
        if "Fill" in row and pd.notna(row["Fill"]):
            fill_color = row["Fill"].replace("#", "")
        else:
            # Varsayılan yeşil renk (konut alanları için)
            fill_color = "32CD32"
        
        if len(fill_color) == 6:  # Standart hex renk kodu
            r, g, b = fill_color[0:2], fill_color[2:4], fill_color[4:6]
            kml_color = f"ff{b}{g}{r}"  # KML format: aabbggrr
            color.text = kml_color

    # KML dosyasını yazdır
    if output_file is None:
        output_dir = "output-kml-files"
        os.makedirs(output_dir, exist_ok=True)
        region_part = f"_{selected_region}" if selected_region else ""
        output_file = os.path.join(output_dir, f"imar-sonuc{region_part}.kml")
    else:
        os.makedirs(os.path.dirname(output_file), exist_ok=True)
        
    tree = ET.ElementTree(kml)
    tree.write(output_file, encoding="utf-8", xml_declaration=True)
    logger.info(f"KML dosyası oluşturuldu: {output_file}")
    
    return output_file
def katmanAyirma(kml_file: gpd.GeoDataFrame, selected_region: str, 
                matching_table: Optional[pd.DataFrame] = None, 
                color_scheme: Optional[Dict[str, str]] = None, 
                output_dir: Optional[str] = None) -> gpd.GeoDataFrame:
    """
    GeoDataFrame'deki özellikleri anahtar kelimelere göre kategorilere ayırır.
    
    Args:
        kml_file: İşlenecek GeoDataFrame.
        selected_region: Bölge adı.
        matching_table: Eşleştirme tablosu DataFrame'i (artık opsiyonel, sabit yoldan yüklenecek).
        color_scheme: Renk şeması (opsiyonel).
        output_dir: Çıktı dizini (opsiyonel).
        
    Returns:
        İşlenmiş GeoDataFrame.
    """
    logger.info("Katman ayırma işlemi başlıyor...")
    
    # İlk olarak duplicate geometrileri temizleyelim
    logger.info(f"Orijinal veri boyutu: {len(kml_file)} satır")
    
    # Geometrileri WKT formatına çevirip kontrol edelim
    kml_file['geom_wkt'] = kml_file['geometry'].apply(lambda geom: geom.wkt if geom else None)
    
    # Duplicate geometrileri tespit et
    duplicates = kml_file.duplicated(subset=['geom_wkt'], keep='first')
    duplicate_count = duplicates.sum()
    
    # Duplicate olmayan geometrileri al
    unique_gdf = kml_file[~duplicates].copy()
    
    # Sütun temizliği - hata ayıklama için kontrol ekleyelim
    columns_to_drop = ['imar_kaks_old', 'imar_taks_old']
    for col in columns_to_drop:
        if col in unique_gdf.columns:
            unique_gdf.drop(columns=[col], inplace=True)
            logger.info(f"'{col}' sütunu kaldırıldı")
    
    logger.info(f"Duplicate geometriler silindi: {duplicate_count} adet duplicate, kalan veri boyutu: {len(unique_gdf)} satır")
    
    # Geçici WKT sütununu kaldır
    if 'geom_wkt' in unique_gdf.columns:
        unique_gdf.drop(columns=['geom_wkt'], inplace=True)
    
    # İmar tiplerine göre renk şeması tanımla
    if color_scheme is None:
        color_scheme = {
            "Ticarethane": "#FF6347",  # Tomato
            "Yasakli Alan": "#4682B4",  # SteelBlue
            "Mesken": "#32CD32",        # LimeGreen
            "Sanayi": "#FFD700",        # Gold
            "Kentsel Dönüşüm": "#8A2BE2",  # BlueViolet
            "Tarimsal Sulama": "#DAA520",  # GoldenRod
            "Kamu Hizmeti": "#FF4500",  # OrangeRed
            "Kamu Tesisi": "#7FFFD4",   # Aquamarine
        }
    
    # Eşleştirme tablosunu yükle (harici parametreden veya sabit dosya yolundan)
    if matching_table is None:
        # Sabit dosya yolu kullan
        default_matching_table_path = os.path.join(SCRIPT_DIR, "./deneme_girdiler/katman_eslesme.csv")
        
        try:
            logger.info(f"Eşleştirme tablosu sabit yoldan yükleniyor: {default_matching_table_path}")
            matching_table = pd.read_csv(default_matching_table_path, encoding='utf-8-sig')
        except UnicodeDecodeError:
            try:
                matching_table = pd.read_csv(default_matching_table_path, encoding='latin-1')
            except UnicodeDecodeError:
                matching_table = pd.read_csv(default_matching_table_path, encoding='cp1254')
            # Gerekli sütunları kontrol et
            required_columns = ['Imar Tipi', 'Katman Adlandirma', 'Katman Adi', 'Anahtar Kelimeler']
            missing_columns = [col for col in required_columns if col not in matching_table.columns]
            
            if missing_columns:
                logger.warning(f"Eşleştirme tablosunda bazı sütunlar eksik: {missing_columns}")
        except Exception as e:
            logger.warning(f"Sabit yoldan eşleştirme tablosu yüklenemedi: {str(e)}")
            logger.error(f"Katman ayırma hatası: {str(e)}")
            import traceback
            logger.error(traceback.format_exc())
            return gpd.GeoDataFrame(crs=kml_file.crs)
    
    logger.info(f"Eşleştirme tablosu kullanılıyor: {len(matching_table)} kayıt")
    logger.info(f"Eşleştirme tablosu sütunları: {matching_table.columns.tolist()}")
    
    # 'Katman Adlandirma' ve 'Imar Tipi' sütunlarını ekleyelim
    unique_gdf['Katman Adlandirma'] = ''
    unique_gdf['Imar Tipi'] = ''
    unique_gdf['Fill'] = ''
    
    # StyleUrl temizliği yap
    styled_gdf = unique_gdf[unique_gdf['styleUrl'].str.startswith('#', na=False)].copy()
    count = 0
    total = len(styled_gdf)
    report_interval = max(1, min(1000, total // 10))
    # Her bir satırı işle
    for i, row in styled_gdf.iterrows():
        count += 1
        if count % report_interval == 0:
            logger.info(f"İşlenen satır: {count}/{total} ({count/total*100:.1f}%)")
        # Önce varsayılan değerleri ata (eşleşme bulunamazsa)
        styled_gdf.at[i, 'Imar Tipi'] = 'unclassified'
        styled_gdf.at[i, 'Katman Adlandirma'] = 'unclassified'
        styled_gdf.at[i, 'Fill'] = '#FFFFFF'  # Beyaz
        
        # Eşleştirme tablosunu kullan
        # styleUrl değerini al ve eşleştirme yap
        style_url = str(row.get('styleUrl', '')).strip()
        
        # Tüm eşleştirme tablosunu kontrol et
        for _, match_row in matching_table.iterrows():
            
            # Katman Adi sütunu varsa ve styleUrl içinde bu değeri içeriyorsa
            if 'Katman Adi' in match_row and style_url.upper().find(str(match_row['Katman Adi']).upper()) >= 0:
                # Eşleştirme bulundu - değerleri ata
                styled_gdf.at[i, 'Imar Tipi'] = match_row['Imar Tipi']
                styled_gdf.at[i, 'Katman Adlandirma'] = match_row['Katman Adlandirma']
                styled_gdf.at[i, 'Fill'] = color_scheme.get(match_row['Imar Tipi'], '#FFFFFF')
                break
                
            # Ayrıca name/description gibi alanlarda anahtar kelimeleri ara
            elif 'Anahtar Kelimeler' in match_row:
                keywords = str(match_row['Anahtar Kelimeler']).split('|')
                
                # Arama yapılacak alanlar
                search_fields = ['name', 'description', 'styleUrl']
                search_text = ''
                
                # Tüm arama alanlarını birleştir
                for field in search_fields:
                    if field in row and pd.notna(row[field]):
                        search_text += str(row[field]).upper() + ' '
                
                # Anahtar kelimeleri ara
                for keyword in keywords:
                    if keyword and search_text.find(keyword.upper()) >= 0:
                        # Eşleştirme bulundu - değerleri ata
                        styled_gdf.at[i, 'Imar Tipi'] = match_row['Imar Tipi']
                        styled_gdf.at[i, 'Katman Adlandirma'] = match_row['Katman Adlandirma']
                        styled_gdf.at[i, 'Fill'] = color_scheme.get(match_row['Imar Tipi'], '#FFFFFF')
                        break
        
        # Eşleştirme bulunamazsa temel sınıflandırma yap
        if styled_gdf.at[i, 'Imar Tipi'] == 'unclassified':
            style_url = str(row.get('styleUrl', '')).strip().lower()
            
            if 'sm' in style_url:
                styled_gdf.at[i, 'Imar Tipi'] = 'Ticarethane'
                styled_gdf.at[i, 'Katman Adlandirma'] = 'Ticari Bina'
                styled_gdf.at[i, 'Fill'] = color_scheme.get('Ticarethane')
            elif 'pl' in style_url:
                styled_gdf.at[i, 'Imar Tipi'] = 'Mesken'
                styled_gdf.at[i, 'Katman Adlandirma'] = 'Konut'
                styled_gdf.at[i, 'Fill'] = color_scheme.get('Mesken')
    
    # Style olmayan verileri de ekle
    unstyled_gdf = unique_gdf[~unique_gdf['styleUrl'].str.startswith('#', na=True)].copy()
    # Birleştir
    result_gdf = pd.concat([styled_gdf, unstyled_gdf], ignore_index=True)
    
    # name ve name_1 sütunlarını düzenle
    if 'name_1' in result_gdf.columns and 'name' in result_gdf.columns:
        result_gdf['name_1'] = result_gdf['name_1'].fillna(result_gdf['name'])
        
    if 'coords' in result_gdf.columns:
        del result_gdf['coords']

    # Sonuçları kaydet (eğer output_dir belirtilmişse)
    if output_dir:
        os.makedirs(output_dir, exist_ok=True)
        
        # CSV kaydet
        output_file = os.path.join(output_dir, f"sonuc-{selected_region}-imar.csv")
        result_gdf.to_csv(output_file, index=False, encoding='utf-8-sig')
        logger.info(f"CSV sonuç dosyası kaydedildi: {output_file}")
        
        # KML dosyası oluştur
        kml_output = os.path.join(output_dir, f"sonuc-{selected_region}-imar.kml")
        createKml(result_gdf, kml_output, selected_region)
    
    return result_gdf

def clean_hmax(value):
    """
    hmax değerlerini temizler ve sayısallaştırır
    """
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

def create_single_shp_with_buffer_preserving_data(unique_gdf: gpd.GeoDataFrame, output_file: str, point_radius: int = 5) -> str:
    """
    GeoDataFrame'i shapefile olarak kaydeder
    
    Args:
        unique_gdf: Kaydedilecek GeoDataFrame.
        output_file: Çıktı dosyasının yolu.
        point_radius: Nokta geometriler için tampon yarıçapı (m).
        
    Returns:
        Oluşturulan shapefile'ın yolu.
    """
    output_dir = os.path.dirname(output_file)
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)

    # Sütun isimlerini kısaltma (10 karakter sınırı)
    gdf = unique_gdf.copy()
    max_length = 10
    renamed_columns = {col: col[:max_length] for col in gdf.columns if len(col) > max_length}
    gdf = gdf.rename(columns=renamed_columns)

    # CRS kontrolü (Varsayılan WGS84)
    if gdf.crs is None:
        gdf.set_crs(epsg=4326, inplace=True)
    
    # Geometriyi metrik sisteme dönüştür (UTM zone 35N - Türkiye için) ve lat/lon hesapla
    gdf_utm = gdf.to_crs(epsg=32635)
    gdf['lat'] = gdf_utm.geometry.centroid.y
    gdf['lon'] = gdf_utm.geometry.centroid.x
    
    # UTM koordinat sisteminde alan hesaplama (metrekare cinsinden)
    gdf_utm['area_m2'] = gdf_utm.area
    
    # Point ve Polygon geometrilerini ayır
    points = gdf_utm[gdf_utm.geometry.type == 'Point']
    polygons = gdf_utm[gdf_utm.geometry.type == 'Polygon']
    
    # Point geometrilerine buffer uygulayarak küçük dairelere dönüştür
    if not points.empty:
        points.loc[:, 'geometry'] = points['geometry'].buffer(point_radius)
    
    # Polygon ve daire haline getirilen Point geometrilerini birleştir
    combined_gdf = pd.concat([points, polygons], ignore_index=True)

    # Son çıktıyı tekrar WGS84'e çevir
    combined_gdf = combined_gdf.to_crs(epsg=4326)
    
    # Shapefile'a kaydet
    try:
        combined_gdf.to_file(output_file, driver='ESRI Shapefile', encoding='utf-8')
        logger.info(f"Shapefile dosyası kaydedildi: {output_file}")
        return output_file
    except Exception as e:
        logger.error(f"SHP kaydetme hatası: {str(e)}")
        return ""
def create_single_shp_with_buffer_preserving_data(unique_gdf: gpd.GeoDataFrame, output_file: str, point_radius: int = 5) -> str:
    """
    GeoDataFrame'i shapefile olarak kaydeder
    
    Args:
        unique_gdf: Kaydedilecek GeoDataFrame.
        output_file: Çıktı dosyasının yolu.
        point_radius: Nokta geometriler için tampon yarıçapı (m).
        
    Returns:
        Oluşturulan shapefile'ın yolu.
    """
    output_dir = os.path.dirname(output_file)
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)

    # Sütun isimlerini kısaltma (10 karakter sınırı)
    gdf = unique_gdf.copy()
    max_length = 10
    renamed_columns = {col: col[:max_length] for col in gdf.columns if len(col) > max_length}
    gdf = gdf.rename(columns=renamed_columns)

    # CRS kontrolü (Varsayılan WGS84)
    if gdf.crs is None:
        gdf.set_crs(epsg=4326, inplace=True)
    
    # Geometriyi metrik sisteme dönüştür (UTM zone 35N - Türkiye için) ve lat/lon hesapla
    gdf_utm = gdf.to_crs(epsg=32635)
    gdf['lat'] = gdf_utm.geometry.centroid.y
    gdf['lon'] = gdf_utm.geometry.centroid.x
    
    # UTM koordinat sisteminde alan hesaplama (metrekare cinsinden)
    gdf_utm['area_m2'] = gdf_utm.area
    
    # Point ve Polygon geometrilerini ayır
    points = gdf_utm[gdf_utm.geometry.type == 'Point']
    polygons = gdf_utm[gdf_utm.geometry.type == 'Polygon']
    
    # Point geometrilerine buffer uygulayarak küçük dairelere dönüştür
    if not points.empty:
        points.loc[:, 'geometry'] = points['geometry'].buffer(point_radius)
    
    # Polygon ve daire haline getirilen Point geometrilerini birleştir
    combined_gdf = pd.concat([points, polygons], ignore_index=True)

    # Son çıktıyı tekrar WGS84'e çevir
    combined_gdf = combined_gdf.to_crs(epsg=4326)
    
    # Shapefile'a kaydet
    try:
        combined_gdf.to_file(output_file, driver='ESRI Shapefile', encoding='utf-8')
        logger.info(f"Shapefile dosyası kaydedildi: {output_file}")
        return output_file
    except Exception as e:
        logger.error(f"SHP kaydetme hatası: {str(e)}")
        return ""

if __name__ == "__main__":
    # Konsol günlük kaydını ayarla
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s - %(name)s - %(levelname)s - %(message)s"
    )
    
    # Komut satırı argümanlarını kontrol et
    import sys
    if len(sys.argv) < 3:
        print("Kullanım: python imar_datalar.py <kesisim_csv> <bölge> [çıktı_dizini]")
        sys.exit(1)
    
    kesisim_csv = sys.argv[1]
    selected_region = sys.argv[2]
    output_dir = sys.argv[3] if len(sys.argv) > 3 else "outputs"
    
    print(f"İşlem başlıyor - CSV: {kesisim_csv}, Bölge: {selected_region}, Çıktı: {output_dir}")
    
    try:
        # CSV dosyasını oku
        import pandas as pd
        from shapely import wkt
        import geopandas as gpd
        
        df = pd.read_csv(kesisim_csv)
        
        # Geometri sütununu düzelt (WKT'den Shapely'ye dönüştür)
        if 'geometry' in df.columns:
            df['geometry'] = df['geometry'].apply(lambda x: wkt.loads(x) if isinstance(x, str) else None)
            intersection_gdf = gpd.GeoDataFrame(df, geometry='geometry', crs="EPSG:4326")
            
            # Katman ayırma işlemi
            result = katmanAyirma(intersection_gdf, selected_region, output_dir=output_dir)
            
            print(f"İşlem başarılı: {len(result)} kayıt işlendi")
            print(f"Sonuç dosyaları {output_dir} dizinine kaydedildi")
        else:
            print("CSV dosyasında 'geometry' sütunu bulunamadı!")
            sys.exit(1)
            
    except Exception as e:
        print(f"Hata oluştu: {str(e)}")
        import traceback
        print(traceback.format_exc())
        sys.exit(1)
