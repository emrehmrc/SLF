import geopandas as gpd
from shapely.geometry import Point, Polygon, LineString
import xml.etree.ElementTree as ET
import re
import pandas as pd
from task_kask_check import kml_to_geodataframe_check 
from database_kodlari.overpass_tablolari.kod.overpass_tablolar import process_all_data

def process_to_geodataframe(kml_file_path, global_data, selected_region):
    """
    KML dosyasını işleyerek GeoDataFrame'e dönüştürür ve kesişim analizini yapar
    
    Args:
        kml_file_path (str): KML dosyasının yolu
        global_data (GeoDataFrame): Global veri seti
        selected_region (str): Seçili bölge adı
    
    Returns:
        GeoDataFrame: İşlenmiş ve kesişim analizi yapılmış veri
    """
    try:
        # Veri temizleme ve hazırlık
        global_dataa = global_data.drop_duplicates(subset=['name', 'geometry'], keep='first')
        print(f"Veri sayısı - Önce: {len(global_data)}, Sonra: {len(global_dataa)}")
        
        # Seçili bölge için veriyi işle
        process_all_data(selected_region.lower())
        
        # KML dosyasını GeoDataFrame'e dönüştür
        gdf = kml_to_geodataframe_check(kml_file_path, selected_region)
        
        # Kesişim analizini yap
        return intersection_and_clean(gdf, global_dataa)
        
    except Exception as e:
        print(f"Hata oluştu: {str(e)}")
        import traceback
        print(f"Hata detayı:\n{traceback.format_exc()}")
        return gpd.GeoDataFrame()
def get_building_levels(tags):
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

def process_intersection_results(overlap_gdf, gdf_tum_ada):
    """
    Kesişim sonuçlarını işler ve metrikler hesaplar
    """
    # Kesişen alanları hesapla
    overlap_gdf['kesisen_alan_m2'] = overlap_gdf.geometry.area.round(2)
    
    # Kat yüksekliği bilgisini ekle
    overlap_gdf['kat_yuksekligi'] = overlap_gdf['tags'].apply(get_building_levels) if 'tags' in overlap_gdf.columns else "nan"
    
    # Tekrarlı kayıtları temizle
    overlap_gdf = overlap_gdf.drop_duplicates(subset=['geometry'])
    
    # TAKS ve KAKS hesapla
    taks_kaks_toplam = calculate_taks_kaks(overlap_gdf)
    
    # Sonuçları birleştir
    return merge_results(overlap_gdf, taks_kaks_toplam)
def calculate_area_in_utm(geometry, source_crs="EPSG:4326"):
    """
    Geometriyi UTM'e dönüştürüp alanı metre kare cinsinden hesaplar
    """
    # Geometri GeoSeries'e dönüştürülüyor
    geom_series = gpd.GeoSeries([geometry], crs=source_crs)
    
    # UTM zone 36N'e dönüştür
    utm_geom = geom_series.to_crs("EPSG:32636")
    
    # Alanı hesapla (m²)
    area = utm_geom.area.iloc[0]
    
    return round(area, 2)
def calculate_taks_kaks(overlap_gdf):
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

def merge_results(overlap_gdf, taks_kaks_toplam):
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

def intersection_and_clean(gdf, global_dataa):
    """
    Kesişim analizi yapar ve sonuçları temizler
    """
    print("İşlem başlıyor...")
    
    # Veri kontrolü
    if gdf.empty or global_dataa.empty:
        print("Verilerden biri boş!")
        return gpd.GeoDataFrame()
    
    # GeoDataFrame kontrolü ve dönüşümü
    if not isinstance(gdf, gpd.GeoDataFrame):
        gdf = gpd.GeoDataFrame(gdf, crs="EPSG:4326")
    if not isinstance(global_dataa, gpd.GeoDataFrame):
        global_dataa = gpd.GeoDataFrame(global_dataa, crs="EPSG:4326")
    
    # Geometri kontrolü
    if 'geometry' not in gdf.columns or 'geometry' not in global_dataa.columns:
        print("Geometri sütunu eksik!")
        return gpd.GeoDataFrame()
    
    print(f"gdf satır sayısı: {len(gdf)}")
    print(f"global_dataa satır sayısı: {len(global_dataa)}")
    
    try:
        print("\nCRS kontrolü yapılıyor...")
        print(f"gdf CRS: {gdf.crs}")
        print(f"global_dataa CRS: {global_dataa.crs}")
        
        # CRS ayarları
        if gdf.crs is None:
            print("gdf CRS ayarlanıyor...")
            gdf = gdf.set_crs("EPSG:4326")
        if global_dataa.crs is None:
            print("global_dataa CRS ayarlanıyor...")
            global_dataa = global_dataa.set_crs("EPSG:4326")

        utm_crs = "EPSG:32636"  # UTM zone 36N için
        
        print("UTM dönüşümü başlıyor...")
        # UTM projeksiyon dönüşümü
        gdf_tum_utm = gdf.to_crs(utm_crs)
        gdf_dek_utm = global_dataa.to_crs(utm_crs)
        print("UTM dönüşümü tamamlandı")
        
        # Sadece poligonları filtrele
        print("Poligon filtreleme başlıyor...")
        gdf_tum_polygons = gdf_tum_utm[gdf_tum_utm.geometry.type == 'Polygon'].copy()
        gdf_dek_polygons = gdf_dek_utm[gdf_dek_utm.geometry.type == 'Polygon'].copy()
        print(f"Filtrelenen poligon sayısı - gdf: {len(gdf_tum_polygons)}, global_data: {len(gdf_dek_polygons)}")

        # name veya name_1 kolonunu kontrol et
        name_column = 'name_1' if 'name_1' in gdf_tum_polygons.columns else 'name'
        print(f"\nKullanılan isim kolonu: {name_column}")
        
        # Kolonları yeniden adlandır
        gdf_tum_polygons = gdf_tum_polygons.rename(columns={name_column: 'parcel_name'})
        name_column = 'parcel_name'  # Yeni kolon adını güncelle
        
        # Adakenari filtrelemesi
        print("\nAdakenari filtrelemesi yapılıyor...")
        if name_column in gdf_tum_polygons.columns:
            contains_pattern = '|'.join(['adakenari', 'ada kenari', 'ada kenarı', 'adakenarı'])
            gdf_tum_ada = gdf_tum_polygons[
                gdf_tum_polygons[name_column].str.contains(
                    contains_pattern, 
                    case=False, 
                    na=False, 
                    regex=True
                )
            ].copy()
            print(f"Adakenari içeren poligon sayısı: {len(gdf_tum_ada)}")
            
            if gdf_tum_ada.empty:
                print("Adakenari bulunamadı, tüm poligonlar kullanılacak")
                gdf_tum_ada = gdf_tum_polygons.copy()
        else:
            print("İsim kolonu bulunamadı, tüm poligonlar kullanılıyor")
            gdf_tum_ada = gdf_tum_polygons.copy()
            gdf_tum_ada[name_column] = 'unnamed'

        # Alan hesaplamaları (m²)
        print("\nAlan hesaplamaları yapılıyor...")
        if not gdf_tum_ada.empty:
            # Her geometri için UTM'de alan hesapla
            gdf_tum_ada['parsel_alan_m2'] = gdf_tum_ada.geometry.apply(
                lambda geom: calculate_area_in_utm(geom, gdf_tum_ada.crs)
            )
            print("Parsel alanları hesaplandı")
            print("Örnek parsel alanları:")
            print(gdf_tum_ada['parsel_alan_m2'].head())

        # DEBUG BAŞLANGICI: Sonraki adımları takip etmek için
        print("\nDEBUG: Bina alanları hesaplama öncesi kontroller")
        print(f"gdf_dek_polygons boş mu: {gdf_dek_polygons.empty}")
        print(f"gdf_dek_polygons satır sayısı: {len(gdf_dek_polygons)}")
        print(f"gdf_dek_polygons sütunları: {gdf_dek_polygons.columns.tolist()}")
        print(f"gdf_dek_polygons geometri tipleri: {gdf_dek_polygons.geometry.type.unique().tolist()}")
        print(f"gdf_dek_polygons CRS: {gdf_dek_polygons.crs}")
        
        # İlk birkaç geometrinin geçerli olup olmadığını kontrol et
        if not gdf_dek_polygons.empty:
            print("\nDEBUG: İlk 5 geometrinin geçerliliği:")
            for i, geom in enumerate(gdf_dek_polygons.geometry.head()):
                print(f"Geometri {i}: Geçerli mi = {geom.is_valid}, Boş mu = {geom.is_empty}")
        # DEBUG SONU

        if not gdf_dek_polygons.empty:
            try:
                # Her geometri için UTM'de alan hesapla
                print("\nDEBUG: Bina alanları hesaplama başlıyor...")
                # Önce küçük bir örnek üzerinde dene
                sample_size = min(5, len(gdf_dek_polygons))
                sample_geoms = gdf_dek_polygons.head(sample_size)
                
                print(f"DEBUG: {sample_size} örnek geometri üzerinde alan hesaplama deneniyor")
                for i, geom in enumerate(sample_geoms.geometry):
                    try:
                        area = calculate_area_in_utm(geom, gdf_dek_polygons.crs)
                        print(f"DEBUG: Geometri {i} için alan hesaplandı: {area} m²")
                    except Exception as e:
                        print(f"DEBUG: Geometri {i} için alan hesaplamada hata: {str(e)}")
                
                print("DEBUG: Tüm geometriler için alan hesaplamaya başlanıyor...")
                gdf_dek_polygons['bina_alan_m2'] = gdf_dek_polygons.geometry.apply(
                    lambda geom: calculate_area_in_utm(geom, gdf_dek_polygons.crs)
                )
                print("Bina alanları hesaplandı")
                print("Örnek bina alanları:")
                print(gdf_dek_polygons['bina_alan_m2'].head())
            except Exception as e:
                print(f"DEBUG: Bina alanları hesaplanırken hata oluştu: {str(e)}")
                import traceback
                print(f"DEBUG: Hata detayı:\n{traceback.format_exc()}")

        # Kesişim analizi
        print("\nKesişim analizi başlıyor...")
        if not gdf_tum_ada.empty and not gdf_dek_polygons.empty:
            try:
                print("DEBUG: Kesişim (intersection) işlemi başlıyor...")
                overlap_gdf = gpd.overlay(gdf_tum_ada, gdf_dek_polygons, how='intersection')
                print(f"DEBUG: Kesişim sonucu - satır sayısı: {len(overlap_gdf)}")
                
                print("DEBUG: Fark (difference) işlemi başlıyor...")
                diff_overlay_gdf = gpd.overlay(gdf_tum_ada, gdf_dek_polygons, how='difference')
                print(f"DEBUG: Fark sonucu - satır sayısı: {len(diff_overlay_gdf)}")
                
                if not overlap_gdf.empty:
                    # Kesişim alanını UTM'de hesapla
                    print("DEBUG: Kesişim alanları hesaplanıyor...")
                    overlap_gdf['kesisen_alan_m2'] = overlap_gdf.geometry.apply(
                        lambda geom: calculate_area_in_utm(geom, overlap_gdf.crs)
                    )
                    
                    # Tags'den building:levels bilgisini çıkar
                    print("DEBUG: Kat yüksekliği bilgisi çıkarılıyor...")
                    if 'tags' in overlap_gdf.columns:
                        overlap_gdf['kat_yuksekligi'] = overlap_gdf['tags'].apply(get_building_levels)
                    else:
                        print("DEBUG: 'tags' sütunu bulunamadı!")
                        overlap_gdf['kat_yuksekligi'] = "nan"

                    print("DEBUG: Tekrarlı kayıtlar temizleniyor...")
                    overlap_gdf = overlap_gdf.drop_duplicates(subset=['geometry'])

                    # TAKS ve KAKS hesaplamaları için gruplandırma
                    print("DEBUG: TAKS ve KAKS hesaplamaları için gruplandırma yapılıyor...")
                    print(f"DEBUG: overlap_gdf sütunları: {overlap_gdf.columns.tolist()}")
                    taks_kaks_toplam = overlap_gdf.groupby(['parcel_name', 'styleUrl', 'parsel_alan_m2']).agg({
                        'bina_alan_m2': 'sum',
                        'kat_yuksekligi': 'first'
                    }).reset_index()

                    # TAKS hesapla
                    print("DEBUG: TAKS hesaplanıyor...")
                    taks_kaks_toplam['bina_alan_m2'] = taks_kaks_toplam['bina_alan_m2'].clip(upper=taks_kaks_toplam['parsel_alan_m2'])
                    taks_kaks_toplam['TAKS'] = (taks_kaks_toplam['bina_alan_m2'] / taks_kaks_toplam['parsel_alan_m2']).round(4)
                    
                    # KAKS hesapla
                    print("DEBUG: KAKS hesaplanıyor...")
                    taks_kaks_toplam['toplam_insaat_alani'] = taks_kaks_toplam.apply(
                        lambda row: row['bina_alan_m2'] * float(row['kat_yuksekligi']) 
                        if row['kat_yuksekligi'] != "Bilinmiyor" 
                        else row['bina_alan_m2'], axis=1
                    )
                    taks_kaks_toplam['KAKS'] = (taks_kaks_toplam['toplam_insaat_alani'] / taks_kaks_toplam['parsel_alan_m2']).round(4)

                    # Ana DataFrame'e TAKS ve KAKS değerlerini ekle
                    print("DEBUG: Ana DataFrame'e TAKS ve KAKS değerleri ekleniyor...")
                    overlap_gdf = overlap_gdf.merge(
                        taks_kaks_toplam[['parcel_name', 'styleUrl', 'parsel_alan_m2', 'TAKS', 'KAKS', 'bina_alan_m2', 'toplam_insaat_alani']],
                        on=['parcel_name', 'styleUrl', 'parsel_alan_m2'],
                        how='left',
                        suffixes=('_old', '')
                    )

                    # Alan bilgisi string'ini güncelle
                    print("DEBUG: Alan bilgisi string'i güncelleniyor...")
                    overlap_gdf['alan_bilgisi'] = overlap_gdf.apply(
                        lambda row: (
                            f"Parsel Alanı: {row['parsel_alan_m2']:,.2f} m² | "
                            f"Toplam Bina Alanı: {row['bina_alan_m2']:,.2f} m² | "
                            f"Toplam İnşaat Alanı: {row['toplam_insaat_alani']:,.2f} m² | "
                            f"TAKS: {row['TAKS']:.4f} | "
                            f"KAKS: {row['KAKS']:.4f}"
                        ), axis=1
                    )
                    
                    # WGS84'e geri dönüştür
                    print("DEBUG: WGS84'e dönüştürülüyor...")
                    overlap_gdf = overlap_gdf.to_crs("EPSG:4326")
                    diff_overlay_gdf = diff_overlay_gdf.to_crs("EPSG:4326")
                    
                    # Orijinal sütunları koru
                    print("DEBUG: Orijinal sütunlar korunuyor...")
                    for col in global_dataa.columns:
                        if col not in overlap_gdf.columns and col != 'geometry':
                            overlap_gdf[col] = global_dataa[col].iloc[0]
                    
                    # Sonuçları kaydet
                    print("DEBUG: Sonuçlar kaydediliyor...")
                    overlap_gdf.to_csv("merge-data/merge-datas.csv", index=False)
                    print("İşlem başarıyla tamamlandı!")
                    
                    print("DEBUG: Son birleştirme yapılıyor...")
                    overlap_gdf = pd.concat([overlap_gdf, diff_overlay_gdf], ignore_index=True)
                    return overlap_gdf
                else:
                    print("Kesişim sonucu boş!")
            except Exception as e:
                print(f"DEBUG: Kesişim işleminde hata: {str(e)}")
                import traceback
                print(f"DEBUG: Kesişim hatası detayı:\n{traceback.format_exc()}")
        else:
            print("Kesişim için uygun veri bulunamadı!")
        
        return gpd.GeoDataFrame()

    except Exception as e:
        print(f"Kesişim analizinde hata: {str(e)}")
        import traceback
        print(f"Hata detayı:\n{traceback.format_exc()}")
        return gpd.GeoDataFrame()


def prepare_data(gdf, global_dataa):
    """
    Veriyi analiz için hazırlar ve kontrolleri yapar
    """
    if gdf.empty or global_dataa.empty:
        print("Verilerden biri boş!")
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

def convert_and_filter_data(gdf, global_dataa):
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
    
    # Adakenari filtreleme
    gdf_ada = filter_adakenari(gdf_polygons)
    
    # Alan hesaplamaları
    if not gdf_ada.empty:
        gdf_ada['parsel_alan_m2'] = gdf_ada.geometry.area.round(2)
    if not global_dataa_polygons.empty:
        global_dataa_polygons['bina_alan_m2'] = global_dataa_polygons.geometry.area.round(2)
    
    return gdf_ada, global_dataa_polygons

def filter_adakenari(gdf_polygons):
    """
    Adakenari filtrelemesi yapar
    """
    name_column = 'parcel_name'
    if name_column in gdf_polygons.columns:
        contains_pattern = '|'.join(['adakenari', 'ada kenari', 'ada kenarı', 'adakenarı'])
        filtered = gdf_polygons[
            gdf_polygons[name_column].str.contains(
                contains_pattern, 
                case=False, 
                na=False, 
                regex=True
            )
        ].copy()
        
        if filtered.empty:
            filtered = gdf_polygons.copy()
    else:
        filtered = gdf_polygons.copy()
        filtered[name_column] = 'unnamed'
        
    return filtered