#!/usr/bin/env python3
"""
İmar Check Modülü

Bu modül, KML dosyalarının işlenmesi, OSM verileriyle kesişim analizi
ve TAKS ve KAKS gibi kentsel metriklerin hesaplanmasını sağlar.
"""

import logging
import traceback
import os
from typing import Dict, Optional, Any, Union, List, Tuple
import re
import geopandas as gpd
import pandas as pd
from shapely.geometry import Point, Polygon, LineString

# Günlük kaydı yapılandırması
logger = logging.getLogger(__name__)

def process_to_geodataframe(kml_file_path: str, global_data: gpd.GeoDataFrame, selected_region: str, output_file: Optional[str] = None) -> gpd.GeoDataFrame:
    """
    KML dosyasını işler, GeoDataFrame'e dönüştürür ve kesişim analizini gerçekleştirir.
    
    Args:
        kml_file_path: KML dosyasının yolu veya yüklenmiş GeoDataFrame.
        global_data: OSM'den gelen global veriyi içeren GeoDataFrame.
        selected_region: Seçili bölge adı.
        output_file: Kesişim sonuçlarının kaydedileceği dosya yolu (opsiyonel).
        
    Returns:
        Kesişim analizi sonuçlarını içeren işlenmiş GeoDataFrame.
    """
    try:
        logger.info(f"KML dosyası işleniyor: {kml_file_path if isinstance(kml_file_path, str) else 'Yüklenmiş GeoDataFrame'}")
        
        # Veri temizleme ve hazırlık
        global_dataa = global_data.drop_duplicates(subset=['name', 'geometry'], keep='first')
        logger.info(f"Veri sayısı - Önce: {len(global_data)}, Sonra: {len(global_dataa)}")
        
        # Dairesel import'u önlemek için sadece kullanıldığında import et
        from taks_kaks import kml_to_geodataframe_check
        
        # KML dosyasını GeoDataFrame'e dönüştür veya doğrudan GeoDataFrame'i kullan
        if isinstance(kml_file_path, str):
            gdf = kml_to_geodataframe_check(kml_file_path, selected_region)
        else:
            gdf = kml_file_path
        
        # Kesişim analizini yap
        result = intersection_and_clean(gdf, global_dataa)
        
        # Sonuçları kaydet (eğer dosya yolu belirtilmişse)
        if output_file is not None and result is not None and not result.empty:
            output_dir = os.path.dirname(output_file)
            if output_dir:
                os.makedirs(output_dir, exist_ok=True)
            result.to_csv(output_file, index=False, encoding='utf-8-sig')
            logger.info(f"Kesişim analizi sonucu kaydedildi: {output_file}")
            
        return result
        
    except Exception as e:
        logger.error(f"Hata oluştu: {str(e)}")
        logger.error(f"Hata detayı:\n{traceback.format_exc()}")
        return gpd.GeoDataFrame()

def get_building_levels(tags: Any) -> Union[float, str]:
    """
    Tags'den bina kat yüksekliği bilgisini çıkarır
    """
    try:
        if isinstance(tags, str):
            tags_dict = eval(tags)
            if 'building:levels' in tags_dict:
                return float(tags_dict['building:levels'])
            elif 'levels' in tags_dict:
                return float(tags_dict['levels'])
    except:
        pass
    return "Bilinmiyor"

def calculate_area_in_utm(geometry: Any, source_crs: str = "EPSG:4326") -> float:
    """
    Converts geometry to UTM and calculates area in square meters.
    
    Args:
        geometry: Shapely geometry object
        source_crs: Source coordinate reference system
        
    Returns:
        float: Area in square meters, rounded to 2 decimal places
    """
    # Convert to GeoSeries for CRS transformation
    geom_series = gpd.GeoSeries([geometry], crs=source_crs)
    
    # Transform to UTM zone 36N (Turkey)in
    utm_geom = geom_series.to_crs("EPSG:32636")
    
    # Calculate area in square meters
    area = utm_geom.area.iloc[0]
    
    return round(area, 2)

def calculate_taks_kaks(overlap_gdf: gpd.GeoDataFrame) -> pd.DataFrame:
    """
    TAKS ve KAKS değerlerini hesaplar
    """
    taks_kaks = overlap_gdf.groupby(['parcel_name', 'styleUrl', 'parsel_alan_m2']).agg({
        'bina_alan_m2': 'sum',
        'kat_yuksekligi': 'first'
    }).reset_index()
    
    # TAKS hesapla
    taks_kaks['bina_alan_m2'] = taks_kaks['bina_alan_m2'].clip(upper=taks_kaks['parsel_alan_m2'])
    taks_kaks['TAKS'] = (taks_kaks['bina_alan_m2'] / taks_kaks['parsel_alan_m2']).round(4)
    
    # KAKS hesapla
    taks_kaks['toplam_insaat_alani'] = taks_kaks.apply(
        lambda row: row['bina_alan_m2'] * float(row['kat_yuksekligi']) 
        if row['kat_yuksekligi'] != "Bilinmiyor" 
        else row['bina_alan_m2'], axis=1
    )
    taks_kaks['KAKS'] = (taks_kaks['toplam_insaat_alani'] / taks_kaks['parsel_alan_m2']).round(4)
    
    return taks_kaks

def merge_results(overlap_gdf: gpd.GeoDataFrame, taks_kaks_toplam: pd.DataFrame) -> gpd.GeoDataFrame:
    """
    Sonuçları birleştirir ve alan bilgisi ekler
    """
    # TAKS ve KAKS değerlerini ana DataFrame'e ekle
    result = overlap_gdf.merge(
        taks_kaks_toplam[['parcel_name', 'styleUrl', 'parsel_alan_m2', 'TAKS', 'KAKS', 
                         'bina_alan_m2', 'toplam_insaat_alani']],
        on=['parcel_name', 'styleUrl', 'parsel_alan_m2'],
        how='left',
        suffixes=('_old', '')
    )
    
    # Alan bilgisi string'ini ekle
    result['alan_bilgisi'] = result.apply(
        lambda row: (
            f"Parsel Alanı: {row['parsel_alan_m2']:,.2f} m² | "
            f"Toplam Bina Alanı: {row['bina_alan_m2']:,.2f} m² | "
            f"Toplam İnşaat Alanı: {row['toplam_insaat_alani']:,.2f} m² | "
            f"TAKS: {row['TAKS']:.4f} | "
            f"KAKS: {row['KAKS']:.4f}"
        ), axis=1
    )
    
    return result

def intersection_and_clean(gdf: gpd.GeoDataFrame, global_dataa: gpd.GeoDataFrame) -> gpd.GeoDataFrame:
    """
    Kesişim analizi yapar ve sonuçları temizler
    """
    logger.info("İşlem başlıyor...")
    
    # Veri kontrolü
    if gdf.empty or global_dataa.empty:
        logger.warning("Verilerden biri boş!")
        return gpd.GeoDataFrame()
    
    
     # GeoDataFrame kontrolü ve dönüşümü
    if not isinstance(gdf, gpd.GeoDataFrame):
        gdf = gpd.GeoDataFrame(gdf, crs="EPSG:4326")
    if not isinstance(global_dataa, gpd.GeoDataFrame):
        global_dataa = gpd.GeoDataFrame(global_dataa, crs="EPSG:4326")
    # imar_kaks ve imar_taks değerlerini kaydedelim
    imar_values = {}
    if 'imar_kaks' in gdf.columns or 'imar_taks' in gdf.columns:
        logger.info("taks_kaks.py'den gelen imar değerleri bulundu, kaydediliyor...")
        for _, row in gdf.iterrows():
            if pd.notna(row.get('imar_kaks')) or pd.notna(row.get('imar_taks')):
                parcel = str(row.get('name', ''))
                imar_values[parcel] = {
                    'imar_kaks': row.get('imar_kaks'),
                    'imar_taks': row.get('imar_taks')
                }
        logger.info(f"Kaydedilen imar değeri sayısı: {len(imar_values)}")
    # Geometri kontrolü
    if 'geometry' not in gdf.columns or 'geometry' not in global_dataa.columns:
        logger.warning("Geometri sütunu eksik!")
        return gpd.GeoDataFrame()
    
    logger.info(f"gdf satır sayısı: {len(gdf)}")
    logger.info(f"global_dataa satır sayısı: {len(global_dataa)}")
    
    try:
        logger.info("CRS kontrolü yapılıyor...")
        logger.info(f"gdf CRS: {gdf.crs}")
        logger.info(f"global_dataa CRS: {global_dataa.crs}")
        
        # CRS ayarlamaları
        if gdf.crs is None:
            gdf = gdf.set_crs("EPSG:4326")
        if global_dataa.crs is None:
            global_dataa = global_dataa.set_crs("EPSG:4326")
        
        # UTM dönüşümü için CRS belirleme 
        utm_crs = "EPSG:32636"  # Türkiye için UTM zone 36N
        
        # UTM dönüşümü
        gdf_utm = gdf.to_crs(utm_crs)
        global_dataa_utm = global_dataa.to_crs(utm_crs)
        
        # Poligon filtreleme
        gdf_polygons = gdf_utm[gdf_utm.geometry.type == 'Polygon'].copy()
        global_dataa_polygons = global_dataa_utm[global_dataa_utm.geometry.type == 'Polygon'].copy()
        
        logger.info(f"Filtrelenen poligon sayısı - gdf: {len(gdf_polygons)}, global_data: {len(global_dataa_polygons)}")
        
        # İsim kolonu
        name_column = 'name_1' if 'name_1' in gdf_polygons.columns else 'name'
        gdf_polygons = gdf_polygons.rename(columns={name_column: 'parcel_name'})
        
        # Ada kenarı filtreleme
        name_column = 'parcel_name'
        if name_column in gdf_polygons.columns:
            logger.info("Ada kenarı filtreleme yapılıyor...")
            contains_pattern = '|'.join(['adakenari', 'ada kenari', 'ada kenarı', 'adakenarı', 'ada kenar'])
            filtered = gdf_polygons[
                gdf_polygons[name_column].str.contains(
                    contains_pattern, 
                    case=False, 
                    na=False, 
                    regex=True
                )
            ].copy()
            
            logger.info(f"Ada kenarı içeren poligon sayısı: {len(filtered)}")
            
            if filtered.empty or len(filtered) < 10:
                logger.info("Yeterli ada kenarı bulunamadı, tüm poligonlar kullanılacak")
                filtered = gdf_polygons.copy()
        else:
            logger.info(f"{name_column} sütunu bulunamadı, tüm poligonlar kullanılacak")
            filtered = gdf_polygons.copy()
            filtered[name_column] = 'unnamed'
        
        # Alan hesaplamaları
        logger.info("Alan hesaplamaları yapılıyor...")
        filtered['parsel_alan_m2'] = filtered.geometry.area.round(2)
        global_dataa_polygons['bina_alan_m2'] = global_dataa_polygons.geometry.area.round(2)
        
        logger.info(f"Ortalama parsel alanı: {filtered['parsel_alan_m2'].mean():.2f} m²")
        logger.info(f"Ortalama bina alanı: {global_dataa_polygons['bina_alan_m2'].mean():.2f} m²")
        
        # Kesişim analizi
        logger.info("Kesişim analizi yapılıyor...")
        if not filtered.empty and not global_dataa_polygons.empty:
            # Önce intersection (Bina ve parsel kesişimi)
            try:
                overlap_gdf = gpd.overlay(filtered, global_dataa_polygons, how='intersection')
                logger.info(f"Kesişim sonucu - satır sayısı: {len(overlap_gdf)}")
                
                # Sonra difference (Binasız parseller)
                diff_gdf = gpd.overlay(filtered, global_dataa_polygons, how='difference')
                logger.info(f"Fark sonucu - satır sayısı: {len(diff_gdf)}")
                
                if not overlap_gdf.empty:
                    # Kesişen alanları hesapla
                    overlap_gdf['kesisen_alan_m2'] = overlap_gdf.geometry.area.round(2)
                    
                    # Kat yüksekliği bilgisini getir
                    if 'levels' in overlap_gdf.columns:
                        overlap_gdf['kat_yuksekligi'] = overlap_gdf['levels'].apply(
                            lambda x: float(x) if pd.notna(x) and re.match(r'^\d+(?:[,.]\d+)?$', str(x)) else "Bilinmiyor"
                        )
                    elif 'tags' in overlap_gdf.columns:
                        overlap_gdf['kat_yuksekligi'] = overlap_gdf['tags'].apply(get_building_levels)
                    else:
                        overlap_gdf['kat_yuksekligi'] = "Bilinmiyor"
                    
                    # ÖNEMLİ: imar_kaks ve imar_taks sütunlarının varlığını kontrol et
                    # Eğer yoksa bunları oluştur
                     
                    # overlap_gdf oluşturulduktan sonra imar değerlerini ekle
                    if overlap_gdf is not None and not overlap_gdf.empty and imar_values:
                        logger.info("Kaydedilen imar değerlerini overlap_gdf'e aktarıyorum...")
                        for idx, row in overlap_gdf.iterrows():
                            parcel = str(row.get('parcel_name', ''))
                            if parcel in imar_values:
                                overlap_gdf.at[idx, 'imar_kaks'] = imar_values[parcel]['imar_kaks']
                                overlap_gdf.at[idx, 'imar_taks'] = imar_values[parcel]['imar_taks']
                        logger.info(f"imar değerleri aktarıldı, şimdi imar_kaks null olmayan değer sayısı: {overlap_gdf['imar_kaks'].notna().sum()}")
                    
                    # Gruplandırma ve toplam hesaplama 
                    logger.info("TAKS/KAKS hesaplama için gruplandırma yapılıyor...")
                    # Mevcut sütunları logla
                    logger.info(f"Gruplandırma öncesi mevcut sütunlar: {overlap_gdf.columns.tolist()}")
                    
                    # Gruplandırma işlemi
                    taks_kaks = overlap_gdf.groupby(['parcel_name', 'styleUrl', 'parsel_alan_m2']).agg({
                        'bina_alan_m2': 'sum',
                        'kat_yuksekligi': 'first',
                        'imar_kaks': 'first',  # Bu satırın var olduğundan emin ol
                        'imar_taks': 'first'   # Bu satırın var olduğundan emin ol
                    }).reset_index()
                    
                    # TAKS hesaplama
                    logger.info("TAKS değerleri hesaplanıyor...")
                    taks_kaks['bina_alan_m2'] = taks_kaks['bina_alan_m2'].clip(upper=taks_kaks['parsel_alan_m2'])
                    taks_kaks['TAKS'] = (taks_kaks['bina_alan_m2'] / taks_kaks['parsel_alan_m2']).round(4)
                    
                    # KAKS hesaplama
                    logger.info("KAKS değerleri hesaplanıyor...")
                    taks_kaks['toplam_insaat_alani'] = taks_kaks.apply(
                        lambda row: row['bina_alan_m2'] * float(row['kat_yuksekligi']) 
                        if row['kat_yuksekligi'] != "Bilinmiyor" 
                        and not isinstance(row['kat_yuksekligi'], str)
                        else row['bina_alan_m2'], axis=1
                    )
                    taks_kaks['KAKS'] = (taks_kaks['toplam_insaat_alani'] / taks_kaks['parsel_alan_m2']).round(4)
                    
                    # Tekrar ana dataframe'e birleştir
                    logger.info("TAKS/KAKS değerlerini ana veriye birleştirme...")
                    # Taks_kaks içindeki sütunları kontrol et
                    logger.info(f"taks_kaks sütunları: {taks_kaks.columns.tolist()}")
                    
                    # Birleştirme için kullanılacak sütunlar
                    result_columns = ['parcel_name', 'styleUrl', 'parsel_alan_m2', 'TAKS', 'KAKS', 
                                    'bina_alan_m2', 'toplam_insaat_alani', 'imar_kaks', 'imar_taks']
                    
                    # Sütunların var olduğunu kontrol et
                    available_columns = [col for col in result_columns if col in taks_kaks.columns]
                    missing_columns = [col for col in result_columns if col not in taks_kaks.columns]
                    
                    if missing_columns:
                        logger.warning(f"Eksik sütunlar: {missing_columns}")
                        # Eksik sütunları ekle
                        for col in missing_columns:
                            taks_kaks[col] = None
                                    # Birleştirme öncesi
                    
                    logger.info(f"Birleştirme öncesi taks_kaks içindeki imar_kaks değerleri: {taks_kaks['imar_kaks'].dropna().tolist()[:5]}")
                    logger.info(f"Birleştirme öncesi taks_kaks içindeki imar_taks değerleri: {taks_kaks['imar_taks'].dropna().tolist()[:5]}")    
                    # Birleştirme işlemi
                    overlap_gdf = overlap_gdf.merge(
                        taks_kaks[result_columns],
                        on=['parcel_name', 'styleUrl'],
                        how='left',
                        suffixes=('_old', '')
                    )
                    # overlap_gdf.to_csv("debug_overlap_gdf.csv", index=False)                    
                    # Birleştirme sonrası
                    logger.info(f"Birleştirme sonrası overlap_gdf içindeki imar_kaks değerleri: {overlap_gdf['imar_kaks'].dropna().tolist()[:5]}")
                    logger.info(f"Birleştirme sonrası overlap_gdf içindeki imar_taks değerleri: {overlap_gdf['imar_taks'].dropna().tolist()[:5]}")
                    # Birleştirme sonrası sütunları kontrol et
                    logger.info(f"Birleştirme sonrası sütunlar: {overlap_gdf.columns.tolist()}")
                    
                    # Önemli: imar_kaks ve imar_taks'ın NaN değil null olduğunu kontrol et
                    if 'imar_kaks' in overlap_gdf.columns and 'imar_taks' in overlap_gdf.columns:
                        logger.info(f"imar_kaks null olmayan değer sayısı: {overlap_gdf['imar_kaks'].notna().sum()}")
                        logger.info(f"imar_taks null olmayan değer sayısı: {overlap_gdf['imar_taks'].notna().sum()}")
                    
                    # Alan bilgisi ekle (String olarak parsel, bina, inşaat alanı ve TAKS/KAKS)
                    logger.info("Alan bilgisi stringi oluşturuluyor...")
                    overlap_gdf['alan_bilgisi'] = overlap_gdf.apply(
                        lambda row: (
                            f"Parsel Alanı: {row['parsel_alan_m2']:,.2f} m² | "
                            f"Toplam Bina Alanı: {row['bina_alan_m2']:,.2f} m² | "
                            f"Toplam İnşaat Alanı: {row['toplam_insaat_alani']:,.2f} m² | "
                            f"TAKS: {row['TAKS']:.4f} | "
                            f"KAKS: {row['KAKS']:.4f}"
                            + (f" | İmar TAKS: {row['imar_taks']:.4f}" if pd.notna(row['imar_taks']) else "")
                            + (f" | İmar KAKS: {row['imar_kaks']:.4f}" if pd.notna(row['imar_kaks']) else "")
                        ), axis=1
                    )
                    
                    # Fark dataframe'e de gerekli sütunları ekle
                    for col in ['TAKS', 'KAKS', 'bina_alan_m2', 'toplam_insaat_alani', 'imar_kaks', 'imar_taks']:
                        if col not in diff_gdf.columns:
                            diff_gdf[col] = None
                    
                    # Farka da alan bilgisi ekle
                    diff_gdf['alan_bilgisi'] = diff_gdf.apply(
                        lambda row: (
                            f"Parsel Alanı: {row['parsel_alan_m2']:,.2f} m²"
                            + (f" | İmar TAKS: {row['imar_taks']:.4f}" if pd.notna(row['imar_taks']) else "")
                            + (f" | İmar KAKS: {row['imar_kaks']:.4f}" if pd.notna(row['imar_kaks']) else "")
                        ), axis=1
                    )
                    
                    # WGS84'e geri dönüştür
                    logger.info("WGS84'e dönüştürülüyor...")
                    overlap_gdf = overlap_gdf.to_crs("EPSG:4326")
                    diff_gdf = diff_gdf.to_crs("EPSG:4326")
                    
                    # Sonuçları birleştir
                    logger.info("Kesişim ve fark verileri birleştiriliyor...")
                    final_gdf = pd.concat([overlap_gdf, diff_gdf], ignore_index=True)
                    final_gdf.to_csv("debug_final_gdf.csv", index=False)
                    # Son kontrol: imar_kaks ve imar_taks sütunları var mı?
                    logger.info(f"Son çıktı sütunları: {final_gdf.columns.tolist()}")
                    if 'imar_kaks' in final_gdf.columns and 'imar_taks' in final_gdf.columns:
                        logger.info(f"Son çıktıda imar_kaks null olmayan değer sayısı: {final_gdf['imar_kaks'].notna().sum()}")
                        logger.info(f"Son çıktıda imar_taks null olmayan değer sayısı: {final_gdf['imar_taks'].notna().sum()}")
                    
                    # Sonucu kaydet
                    logger.info("Sonuçlar kaydediliyor...")
                    os.makedirs("merge-data", exist_ok=True)
                    # Sonuç verisini CSV olarak kaydet (debugging için)
                    # final_gdf.to_csv("merge-data/merge-datas-debug.csv", index=False, encoding='utf-8-sig')
                    logger.info("İşlem tamamlandı!")
                    logger.info(f"Dönen sonuç sütunları: {final_gdf.columns.tolist()}")
                    taks_kaks.to_csv("debug_taks_kaks.csv", index=False)
                    overlap_gdf.to_csv("debug_overlap_gdf.csv", index=False)
                    final_gdf.to_csv("debug_final_gdf.csv", index=False)
                    return final_gdf
                else:
                    logger.warning("Kesişim sonucu boş!")
                    return filtered.to_crs("EPSG:4326")  # Sonuç yoksa filtrelenmiş parselleri döndür
            
            except Exception as e:
                logger.error(f"Kesişim analizi hatası: {str(e)}")
                logger.error(traceback.format_exc())
        else:
            logger.warning("Kesişim için uygun veri bulunamadı!")
        
        return gpd.GeoDataFrame()
    
    except Exception as e:
        logger.error(f"Genel hata: {str(e)}")
        logger.error(traceback.format_exc())
        return gpd.GeoDataFrame()
def prepare_data(gdf: gpd.GeoDataFrame, global_dataa: gpd.GeoDataFrame) -> bool:
    """
    Veriyi analiz için hazırlar ve kontrolleri yapar
    """
    if gdf.empty or global_dataa.empty:
        logger.warning("Verilerden biri boş!")
        return False
        
    # GeoDataFrame dönüşümleri
    if not isinstance(gdf, gpd.GeoDataFrame):
        gdf = gpd.GeoDataFrame(gdf, crs="EPSG:4326")
    if not isinstance(global_dataa, gpd.GeoDataFrame):
        global_dataa = gpd.GeoDataFrame(global_dataa, crs="EPSG:4326")
        
    # CRS kontrolleri
    if gdf.crs is None:
        gdf = gdf.set_crs("EPSG:4326")
    if global_dataa.crs is None:
        global_dataa = global_dataa.set_crs("EPSG:4326")
        
    return True

def convert_and_filter_data(gdf: gpd.GeoDataFrame, global_dataa: gpd.GeoDataFrame) -> Tuple[gpd.GeoDataFrame, gpd.GeoDataFrame]:
    """
    Veriyi UTM'e dönüştürür ve poligonları filtreler
    """
    # UTM dönüşümü
    utm_crs = "EPSG:32636"
    gdf_utm = gdf.to_crs(utm_crs)
    global_dataa_utm = global_dataa.to_crs(utm_crs)
    
    # Poligon filtreleme
    gdf_polygons = gdf_utm[gdf_utm.geometry.type == 'Polygon'].copy()
    global_dataa_polygons = global_dataa_utm[global_dataa_utm.geometry.type == 'Polygon'].copy()
    
    # İsim kolonu düzenleme
    name_column = 'name_1' if 'name_1' in gdf_polygons.columns else 'name'
    gdf_polygons = gdf_polygons.rename(columns={name_column: 'parcel_name'})
    
    
    
    # Alan hesaplamaları
    if not gdf_polygons.empty:
        gdf_polygons['parsel_alan_m2'] = gdf_polygons.geometry.area.round(2)
    if not global_dataa_polygons.empty:
        global_dataa_polygons['bina_alan_m2'] = global_dataa_polygons.geometry.area.round(2)
    
    return gdf_polygons, global_dataa_polygons



# Bu modül doğrudan çalıştırılırsa
if __name__ == "__main__":
    # Konsol günlük kaydını ayarla
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s - %(name)s - %(levelname)s - %(message)s"
    )
    
    # Komut satırı argümanlarını kontrol et
    import sys
    if len(sys.argv) < 3:
        print("Kullanım: python imar_check.py <kml_dosya_yolu> <bölge>")
        sys.exit(1)
    
    import imar_overpass
    
    kml_file_path = sys.argv[1]
    selected_region = sys.argv[2]
    
    print(f"KML dosyası işleniyor: {kml_file_path}, Bölge: {selected_region}")
    
    # Overpass verilerini çek
    global_data = imar_overpass.fetch_and_process_data(selected_region)
    
    # KML dosyasını işle
    result = process_to_geodataframe(kml_file_path, global_data, selected_region)
    
    if not result.empty:
        print(f"İşlem başarılı: {len(result)} kayıt oluşturuldu.")
        result.to_csv(f"sonuclar_{selected_region}.csv", index=False)
    else:
        print("İşlem sırasında hata oluştu, sonuç boş.")