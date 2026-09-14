"""
İmar Analiz - Ana Uygulama

KML dosyalarını işleyen, Overpass API'den veri çeken ve
kentsel metrikler (TAKS, KAKS) hesaplayan uygulama.

Kullanım:
    python main.py process [bölge] [kml_dosyası] [--district DISTRICT] [--output-dir OUTPUT_DIR]
    python main.py fetch-data [bölge] [--district DISTRICT] [--output-dir OUTPUT_DIR]

Örnek:
    python main.py process Eskişehir path/to/file.kml --district Tepebaşı --output-dir results
    python main.py process Eskişehir path/to/file.kml --overpass-data path/to/overpass_data.csv
    python main.py fetch-data İzmir --district Konak --output-dir data
"""

import sys
import os
import sys
import argparse
import logging
from datetime import datetime
import pandas as pd
import geopandas as gpd
from shapely import wkt
import io
from shapely.wkt import loads
from shapely.geometry import Point, Polygon,MultiPolygon
# Modüllerimizi içe aktarıyoruz
import imar_overpass
import taks_kaks
import imar_check
import imar_datalar
from kod.hucre_imar_kirilimlari import test_imar
from kod.hucre_bina_kirilimlari.bina_kirilimlari import process_city_data
from kod.kofre_analiz import imar_tipleri_analizi_v2 as ita
from kod.saturasyon import saturasyon_analiz
from kod.hucre_trafo_analiz import trafo_merkez_hucre_v2
from kod.hucre_trafo_rezerve_alanlar import trafo_rezerv_alanlar_analiz
from kod.hucre_Abone_kirilim import hucre_abone_as_is


# import kod.hucre_imar_kirilimlari. 
# UTF-8 kodlaması için
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')
sys.breakpointhook = lambda *args, **kwargs: None
# Logger oluşturma
logger = logging.getLogger(__name__)

def load_from_csv(file_path):
    """CSV dosyasını GeoDataFrame'e yükler"""
    try:
        df = pd.read_csv(file_path)
        
        # Geometri sütunu varsa Shapely nesnesine dönüştür
        if 'geometry' in df.columns:
            df['geometry'] = df['geometry'].apply(lambda x: wkt.loads(x) if isinstance(x, str) else None)
            return gpd.GeoDataFrame(df, geometry='geometry', crs="EPSG:4326")
        else:
            return pd.DataFrame(df)
    except Exception as e:
        logger.error(f"CSV yükleme hatası: {str(e)}")
        return None

def setup_logging(config):
    """Loglama yapılandırması oluşturur"""
    log_dir = config.get("log_dir", "logs")
    os.makedirs(log_dir, exist_ok=True)
    
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    region = config['region']
    district = config.get('district', 'Merkez')  # district parametresini kullan
    log_file = os.path.join(log_dir, f"imar_analysis_{region}_{district}_{timestamp}.log")
    
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s - %(name)s - %(levelname)s - %(message)s",
        handlers=[
            logging.FileHandler(log_file, encoding='utf-8'),
            logging.StreamHandler()
        ]
    )


def process_command(config):
    """KML dosyasını belirli bir bölge için işleme"""
    region = config["region"]
    district = config.get("district", "Merkez")
    output_dir = config["output_dir"]
    temp_dir = config["temp_dir"]
    log_dir = config["log_dir"]
    year = config.get("Year")
    year = year-1
    print("year:"+str(year))
    import time
    start_time = time.time()
    
    # Çalışma dizinini ve mutlak yolları logla
    logger.info(f"Çalışma dizini: {os.getcwd()}")
    logger.info(f"Output dizini (mutlak): {output_dir}")
    
    # output_prefix bilgisini doğrudan kullan
    output_prefix = config.get("output_prefix")
    if not output_prefix:
        output_prefix = f"I_{region}_{district}"
    
    # Format klasörleri
    formats = config.get("output_formats", "csv,kml,shp").split(",")
    
    # Format klasörlerini oluştur
    format_dirs = {}
    for fmt in formats:
        fmt = fmt.strip().lower()
        format_dirs[fmt] = os.path.join(output_dir, fmt)
        os.makedirs(format_dirs[fmt], exist_ok=True)
    
    logger.info(f"İşlem başlıyor - Bölge: {region}, İlçe: {district}")
    logger.info(f"Ana klasör: {output_dir}")
    logger.info(f"Geçici klasör: {temp_dir}")
    logger.info(f"Log klasörü: {log_dir}")
    logger.info(f"Çıktı dosya ismi öneki: {output_prefix}")
    
    # Matching table'ı yükle
    matching_table = None
    if "matching_table" in config and config["matching_table"]:
        try:
            matching_table = pd.read_csv(config["matching_table"], encoding='utf-8-sig')
            logger.info(f"Eşleştirme tablosu yüklendi: {len(matching_table)} kayıt")
        except Exception as e:
            logger.warning(f"Eşleştirme tablosu yüklenemedi: {str(e)}")
            matching_table = None
            
    # 1. Overpass verilerini hazırla
    if "overpass_data" in config and config["overpass_data"]:
        logger.info(f"Overpass verisi dosyadan yükleniyor: {config['overpass_data']}")
        osm_data = load_from_csv(config["overpass_data"])
        if osm_data is None:
            logger.error("Overpass verisi yüklenemedi!")
            return 1
    else:
        logger.info(f"Overpass verisi API'den çekiliyor: {region}, İlçe: {district}")
        osm_data = imar_overpass.fetch_and_process_data(
        district,  # Now district is first
        region,    # Now region is second
        output_file=os.path.join(output_dir, f"{output_prefix}.csv")
    )
        logger.info(f"Overpass verisi çekildi: {len(osm_data)} kayıt")
    
    # 2. KML dosyasını işle - şimdi iki döndürülen değer alıyoruz
    logger.info(f"KML dosyası işleniyor: {config['kml_file']}")
    kml_data, pl_polygons = taks_kaks.kml_to_geodataframe_check(
        config['kml_file'],
        selected_region=config['region']
    )
    
    # PL poligonları CSV olarak kaydet
    if pl_polygons is not None and not pl_polygons.empty:
        pl_file = os.path.join(format_dirs["csv"], f"{output_prefix}_pl_polygons.csv")
        pl_polygons.to_csv(pl_file, index=False, encoding="utf-8-sig")
        logger.info(f"PL Poligonları CSV olarak kaydedildi: {pl_file}")
    
    # 3. Kesişim analizi yap
    if kml_data is not None and not kml_data.empty:
        logger.info("Kesişim analizi yapılıyor")
        kml_data = imar_check.process_to_geodataframe(
            kml_data, 
            osm_data, 
            region
        )
        if config.get("save_intermediate", False) and kml_data is not None and not kml_data.empty:
            output_file = os.path.join(temp_dir, f"intersection_{region}_{district}.csv")
            kml_data.to_csv(output_file, index=False, encoding="utf-8-sig")
            logger.info(f"Kesişim analizi sonucu kaydedildi: {output_file}")
    
        # 4. Katman ayırma ve analiz
        if kml_data is not None and not kml_data.empty:
            # Katman eşleştirme tablosunu yükle (varsa)
            matching_table_for_analysis = matching_table
            
            # Renk şemasını yükle (varsa)
            color_scheme = None
            if "color_scheme" in config and config["color_scheme"]:
                try:
                    import json
                    with open(config["color_scheme"], 'r', encoding='utf-8') as f:
                        color_scheme = json.load(f)
                    logger.info(f"Renk şeması yüklendi: {len(color_scheme)} renk")
                except Exception as e:
                    logger.warning(f"Renk şeması yüklenemedi: {str(e)}")
            
            logger.info("Katman ayırma ve analiz yapılıyor")
            final_result = imar_datalar.katmanAyirma(
                kml_data, 
                region,
                matching_table=matching_table_for_analysis,
                color_scheme=color_scheme
            )
            
           # İmar Tipleri Analizi
            try:
                logger.info("İmar tipleri analizi başlatılıyor...")
                
                
                # Analiz için output klasörü oluştur
                kofre_dir = os.path.join(output_dir, "kofre_analiz")
                os.makedirs(kofre_dir, exist_ok=True)
                logger.info(f"Kofre analiz klasörü oluşturuldu: {kofre_dir}")
                
                # Doğrudan final_result'ı kullan
                logger.info(f"İmar verilerini analiz ediliyor... (toplam {len(final_result)} kayıt)")
                
                # Geometri sütununu kontrol et
                if not all(isinstance(geom, (Polygon, MultiPolygon)) for geom in final_result.geometry if geom is not None):
                    logger.warning("Bazı geometriler Polygon veya MultiPolygon değil, filtreleniyor...")
                    valid_mask = final_result.geometry.apply(lambda g: isinstance(g, (Polygon, MultiPolygon)) if g is not None else False)
                    final_result = final_result[valid_mask].copy()
                    logger.info(f"Filtreleme sonrası kayıt sayısı: {len(final_result)}")
                
                # İmar verilerini analiz et
                _,overpass_verileri, ada_kenarlari = ita.analyze_imar_data(
                    final_result, 
                    region, 
                    output_dir=kofre_dir,
                    
                )
                
                # 4. Nokta verilerini yükle
                mesken_file = config.get("mesken_file")
                other_file = config.get("other_file")
                
                # Dosya yolları belirtilmemişse otomatik olarak bul
                if not mesken_file or not other_file:
                    logger.info("Mesken ve diğer bina dosyaları için arama yapılıyor...")
                    import glob
                    
                    # Potansiyel dizinler listesi
                    potential_dirs = [
                        os.path.join(output_dir, "deep_learning_modeli"),
                        os.path.join(os.path.dirname(output_dir), "deep_learning_modeli"),
                        os.path.join(os.path.dirname(output_dir), "imar_analizi_sonuclari", "deep_learning_modeli"),
                        "deep_learning_modeli"
                    ]
                    
                    # Her potansiyel dizini kontrol et
                    for dir_path in potential_dirs:
                        if os.path.exists(dir_path):
                            logger.info(f"Deep learning dizini bulundu: {dir_path}")
                            
                            # Alan adı ve ilçe adına göre arama desenleri
                            region_lower = region.lower()
                            district_lower = district.lower()
                            
                            # Mesken dosyası için arama
                            if not mesken_file:
                                pattern1 = os.path.join(dir_path, f"mesken_data_{region_lower}_{district_lower}*.csv")
                                pattern2 = os.path.join(dir_path, f"mesken_data_{region_lower}*.csv")
                                
                                mesken_files = glob.glob(pattern1)
                                if not mesken_files:
                                    mesken_files = glob.glob(pattern2)
                                    
                                if mesken_files:
                                    mesken_file = mesken_files[0]
                                    logger.info(f"Mesken dosyası bulundu: {mesken_file}")
                            
                            # Diğer bina dosyası için arama
                            if not other_file:
                                pattern1 = os.path.join(dir_path, f"other_data_{region_lower}_{district_lower}*.csv")
                                pattern2 = os.path.join(dir_path, f"other_data_{region_lower}*.csv")
                                
                                other_files = glob.glob(pattern1)
                                if not other_files:
                                    other_files = glob.glob(pattern2)
                                    
                                if other_files:
                                    other_file = other_files[0]
                                    logger.info(f"Diğer bina dosyası bulundu: {other_file}")
                            
                            # İki dosya da bulunduysa döngüden çık
                            if mesken_file and other_file:
                                break
                    
                    # Hala bulunamadıysa genel bir arama yap
                    if not mesken_file or not other_file:
                        all_csv_files = []
                        for dir_path in potential_dirs:
                            if os.path.exists(dir_path):
                                all_csv_files.extend(glob.glob(os.path.join(dir_path, "*.csv")))
                        
                        for csv_file in all_csv_files:
                            file_name = os.path.basename(csv_file).lower()
                            if "mesken" in file_name and not mesken_file:
                                mesken_file = csv_file
                                logger.info(f"Mesken dosyası bulundu (genel arama): {mesken_file}")
                            elif "other" in file_name and not other_file:
                                other_file = csv_file
                                logger.info(f"Diğer bina dosyası bulundu (genel arama): {other_file}")
                
                logger.info(f"Mesken dosyası: {mesken_file}")
                logger.info(f"Diğer bina dosyası: {other_file}")
                
                # 5. Nokta verilerini yükle
                all_points = ita.load_point_data_from_csv(mesken_file, other_file)
                
                # 6. Eşleştirme işlemini gerçekleştir
                if all_points is not None and overpass_verileri is not None:
                    logger.info("Kofre-bina eşleştirme işlemi başlatılıyor...")
                    
                    # Verilerin CRS'ini kontrol et ve gerekirse dönüştür
                    if overpass_verileri.crs is None:
                        logger.warning("Overpass verileri CRS'i None, EPSG:4326 olarak ayarlanıyor")
                        overpass_verileri = overpass_verileri.set_crs("EPSG:4326")
                    
                    if ada_kenarlari is None or len(ada_kenarlari) == 0:
                        logger.warning("Ada kenarları verisi bulunamadı, boş GeoDataFrame oluşturuluyor...")
                        ada_kenarlari = gpd.GeoDataFrame(columns=['parcel_name', 'geometry'], geometry='geometry', crs=overpass_verileri.crs)
                    
                    if all_points.crs != overpass_verileri.crs:
                        logger.info(f"Nokta verisi CRS'i dönüştürülüyor: {all_points.crs} -> {overpass_verileri.crs}")
                        all_points = all_points.to_crs(overpass_verileri.crs)
                    
                    # Eşleştirme işlemini gerçekleştir
                    result_gdf = ita.perform_building_matching(all_points, overpass_verileri, ada_kenarlari)
                    
                    # Sonuçları kaydet ve görselleştir
                    if result_gdf is not None and len(result_gdf) > 0:
                        logger.info(f"Eşleştirme işlemi tamamlandı: {len(result_gdf)} kayıt")
                        
                        # Sonuçları kaydet
                        ita.save_results(result_gdf, output_dir=kofre_dir)
                        
                        # Sonuçları görselleştir
                        ita.visualize_results(result_gdf, output_dir=kofre_dir)
                        # Nokta tipine göre analiz
                        try:
                            logger.info("Mesken ve ticarethane istatistikleri analizi başlatılıyor...")
                            mesken_analyzed, ticarethane_analyzed = ita.analyze_and_export_by_point_type(
                              result_gdf, 
                              output_dir=kofre_dir
                            )
                            logger.info(f"Mesken analizi tamamlandı: {len(mesken_analyzed)} kayıt")
                            logger.info(f"Ticarethane analizi tamamlandı: {len(ticarethane_analyzed)} kayıt")
                            logger.info("Mesken ve ticarethane analizi tamamlandı")
                        except Exception as e:
                            logger.error(f"Nokta tipine göre analiz sırasında hata: {str(e)}")
                            import traceback
                            logger.error(traceback.format_exc())
                            logger.info(f"İmar tipleri analizi başarıyla tamamlandı. Sonuçlar: {kofre_dir}")
                    else:
                        logger.error("Eşleştirme işlemi başarısız oldu veya sonuç boş.")
                else:
                    if all_points is None:
                        logger.error("Nokta verisi yüklenemedi.")
                    if overpass_verileri is None:
                        logger.error("Overpass verisi bulunamadı.")
            except Exception as e:
                logger.error(f"İmar Tipleri Analizi sırasında hata: {str(e)}")
                import traceback
                logger.error(traceback.format_exc())
            
            # Sonuçları istenen formatlarda kaydet
            output_formats = config.get("output_formats", "csv,kml").split(",")
            for fmt in output_formats:
                fmt = fmt.strip().lower()
                if fmt == "csv":
                    # Doğrudan format klasörüne kaydet
                    output_file = os.path.join(format_dirs["csv"], "İMAR_SONUÇLAR.csv")
                    final_result.to_csv(output_file, index=False, encoding="utf-8-sig")
                    logger.info(f"CSV sonucu kaydedildi: {output_file}")
                
                elif fmt == "kml":
                    # Doğrudan format klasörüne kaydet
                    output_file = os.path.join(format_dirs["kml"],"İMAR_SONUÇLAR.kml")
                    imar_datalar.createKml(final_result, output_file, region)
                    logger.info(f"KML sonucu kaydedildi: {output_file}")
                
                elif fmt == "shp":
                    # Doğrudan format klasörüne kaydet
                    output_file = os.path.join(format_dirs["shp"], "İMAR_SONUÇLAR.shp")
                    try:
                        final_result.to_file(output_file, driver="ESRI Shapefile", encoding="utf-8")
                        logger.info(f"Shapefile sonucu kaydedildi: {output_file}")
                    except Exception as e:
                        logger.error(f"Shapefile kaydetme hatası: {str(e)}")
            
            # Hücre verisi analizi
            if "hucre_data" in config and config["hucre_data"]:
                logger.info(f"Hücre verisi ile PL poligonları kesişim analizi başlatılıyor")

                # Saturasyon klasörü ve girdiler alt klasörü oluştur
                saturasyon_dir = os.path.join(output_dir, "saturasyon")
                girdiler_dir = os.path.join(saturasyon_dir, "girdiler")
                os.makedirs(girdiler_dir, exist_ok=True)
                logger.info(f"Saturasyon girdiler klasörü oluşturuldu: {girdiler_dir}")

                # PL poligonlarını imar girişi olarak kullan
                pl_polygons_csv = os.path.join(format_dirs["csv"], f"{output_prefix}_pl_polygons.csv")
                hucre_output_file = os.path.join(girdiler_dir, f"{output_prefix}_hucre_analiz.csv")

                # Matching table değişkenini hazırla
                matching_table_path = config.get("matching_table", None)

                logger.info(f"test_imar modülü çalıştırılıyor: city={region}, imar_input={pl_polygons_csv}, hucre_input={config['hucre_data']}")

                try:
                    # Hücre analizi
                    test_imar.process_city_data(
                        city=region,
                        imar_input=pl_polygons_csv,
                        hucre_input=config["hucre_data"],
                        output_file=hucre_output_file,
                        matching_table=matching_table_path
                    )

                    # Bina kırılım dosyaları için path belirle
                    mesken_file = config.get("mesken_file")
                    other_file = config.get("other_file")

                    # Otomatik dosya arama
                    if not mesken_file or not other_file:
                        logger.info("Mesken ve diğer bina dosyaları için arama yapılıyor...")
                        imar_sonuc_dir = os.path.join(os.path.dirname(output_dir), "imar_analizi_sonuclari")
                        if not os.path.exists(imar_sonuc_dir):
                            imar_sonuc_dir = "imar_analizi_sonuclari"

                        logger.info(f"İmar sonuçları dizini: {imar_sonuc_dir}")
                        deep_learning_dir = os.path.join(imar_sonuc_dir, "deep_learning_modeli")

                        if os.path.exists(deep_learning_dir):
                            import glob
                            mesken_pattern = os.path.join(deep_learning_dir, f"mesken_data_*{region.lower()}*{district.lower()}*.csv")
                            other_pattern = os.path.join(deep_learning_dir, f"other_data_*{region.lower()}*{district.lower()}*.csv")

                            mesken_files = glob.glob(mesken_pattern)
                            other_files = glob.glob(other_pattern)

                            if mesken_files:
                                mesken_file = mesken_files[0]
                            if other_files:
                                other_file = other_files[0]

                            # Yine de bulunamadıysa tüm CSV'leri tara
                            if not mesken_file or not other_file:
                                all_csv_files = glob.glob(os.path.join(deep_learning_dir, "*.csv"))
                                for csv_file in all_csv_files:
                                    file_name = os.path.basename(csv_file).lower()
                                    if "mesken" in file_name and not mesken_file:
                                        mesken_file = csv_file
                                    elif "other" in file_name and not other_file:
                                        other_file = csv_file

                    logger.info(f"Mesken dosyası: {mesken_file}")
                    logger.info(f"Diğer bina dosyası: {other_file}")

                    # Bina kırılımı analiz
                    if mesken_file and other_file:
                        bina_output_file = os.path.join(girdiler_dir, f"{output_prefix}_bina_kirilimlari.csv")
                        logger.info(f"Bina kırılımları analizi başlatılıyor. Çıktı: {bina_output_file}")

                        result_df = process_city_data(
                            region,
                            mesken_file=mesken_file,
                            other_file=other_file,
                            hucre_input=config["hucre_data"],
                            output_file=bina_output_file
                        )

                        if result_df is not None and not result_df.empty:
                            logger.info(f"Bina kırılımları analizi tamamlandı. Toplam {len(result_df)} hücre işlendi.")
                            logger.info(f"Sonuçlar {bina_output_file} dosyasına kaydedildi.")
                            
                            # SATURASYON ANALİZİ
                            
                            if "uydu_data" in config and config["uydu_data"]:
                                logger.info("Saturasyon analizi başlatılıyor...")
                                
                                # Saturasyon klasörü
                                saturasyon_dir = os.path.join(output_dir, "saturasyon")
                                stokastik_dir = os.path.join(saturasyon_dir, "stokastik")
                                os.makedirs(saturasyon_dir, exist_ok=True)
                                os.makedirs(stokastik_dir, exist_ok=True)
                                
                                # Uydu verisi
                                uydu_data = config["uydu_data"]
                                
                                # Kofre analiz çıktıları
                                mesken_stats = os.path.join(kofre_dir, "mesken_istatistikleri.csv")
                                ticaret_stats = os.path.join(kofre_dir, "ticaret_istatistikleri.csv")
                                
                                if os.path.exists(mesken_stats) and os.path.exists(ticaret_stats):
                                    try:
                                        # Doğrudan saturasyon analizi yapan bir modülü import et
                                        # import saturasyon_analiz
                                        
                                        # İşlemi gerçekleştir
                                        result = saturasyon_analiz.process_region_data(
                                            uydu_data,            # Uydu verisi
                                            bina_output_file,     # Bina kırılımları
                                            hucre_output_file,    # İmar kırılımları
                                            mesken_stats,         # Mesken istatistikleri
                                            ticaret_stats,        # Ticaret istatistikleri
                                            region,                # Bölge
                                            year 
                                        )
                                        
                                        if result is not None:
                                            # Sonuçları kaydet
                                            saturasyon_analiz.save_outputs(
                                                result, 
                                                region, 
                                                saturasyon_dir, 
                                                stokastik_dir
                                            )
                                            
                                            logger.info(f"Saturasyon analizi başarıyla tamamlandı. Sonuçlar: {saturasyon_dir}")
                                            
                                            # HÜCRE ABONE KIRILIMLAri ANALİZİ - Saturasyon sonrası eklenen yeni bölüm
                                            logger.info("Hücre abone kırılımları analizi başlatılıyor...")
                                            
                                            # Saturasyon sonuç dosyası
                                            saturasyon_sonuc_dir = os.path.join(saturasyon_dir, "saturasyon_sonuc")
                                            saturasyon_sonuc_file = os.path.join(saturasyon_sonuc_dir, "saturasyon.csv")
                                            
                                            # Hücre abone analiz parametrelerini yazdır
                                            print(f"HÜCRE ABONE ANALİZİ GİRDİ PARAMETRELERİ:")
                                            print(f"Saturasyon dosyası: {saturasyon_sonuc_file}")
                                            print(f"Mesken dosyası: {mesken_file}")
                                            print(f"Diğer binalar dosyası: {other_file}")
                                            
                                            # Saturasyon dosyasının varlığını kontrol et
                                            if os.path.exists(saturasyon_sonuc_file):
                                                try:
                                                    # Hücre abone analizi klasörü oluştur
                                                    hucre_abone_dir = os.path.join(output_dir, "hucre_abone_analizi")
                                                    os.makedirs(hucre_abone_dir, exist_ok=True)
                                                    
                                                    # Hücre abone analizi çalıştır
                                                    hucre_abone_result = hucre_abone_as_is.process_analysis(
                                                        saturasyon_file=saturasyon_sonuc_file,
                                                        mesken_file=mesken_file,
                                                        other_file=other_file,
                                                        output_dir=hucre_abone_dir,
                                                        region=region,
                                                        district=district,
                                                        hucre_file=config['hucre_data']
                                                    )
                                                    
                                                    if hucre_abone_result:
                                                        logger.info(f"Hücre abone kırılımları analizi başarıyla tamamlandı. Sonuçlar: {hucre_abone_dir}")
                                                    else:
                                                        logger.error("Hücre abone kırılımları analizi başarısız oldu.")
                                                        
                                                except Exception as e:
                                                    logger.error(f"Hücre abone kırılımları analizi sırasında hata: {str(e)}")
                                                    import traceback
                                                    logger.error(traceback.format_exc())
                                            else:
                                                logger.warning(f"Saturasyon dosyası bulunamadı: {saturasyon_sonuc_file}")
                                                logger.warning("Hücre abone kırılımları analizi atlanıyor.")
                                        
                                        else:
                                            logger.error("Saturasyon analizi başarısız oldu.")
                                # DTR Modülü ve Yeni DTR Modülü Analizi
                            
                                        # DTR Modülü ve Yeni DTR Modülü Analizi
                                        if "dtr_modulu" in config and config["dtr_modulu"]:
                                            logger.info("DTR Modülü analizi başlatılıyor...")
                                            
                                            # DTR Modülü klasörü
                                            dtr_analiz_dir = os.path.join(output_dir, "hucresel_dtr_verileri")
                                            os.makedirs(dtr_analiz_dir, exist_ok=True)
                                            logger.info(f"DTR analiz klasörü oluşturuldu: {dtr_analiz_dir}")
                                            
                                            # DTR dosyasını yükle
                                            dtr_file = config["dtr_modulu"]
                                            
                                            # Yeni Projelendirilmiş DTR dosyasını yükle (varsa)
                                            yeni_dtr_file = config.get("yeni_dtr_modulu")
                                            
                                            # Hücre dosyasını bul (zaten hucre-data olarak gelmişti)
                                            hucre_file = config.get("hucre_data")
                                            
                                            # Mesken ve diğer bina verileri (varsa)
                                            mesken_file = config.get("mesken_file")
                                            other_file = config.get("other_file")
                                            
                                            if not hucre_file:
                                                logger.error("DTR Modülü analizi için hücre verisi bulunamadı.")
                                            else:
                                                try:
                                                    logger.info(f"DTR dosyası: {dtr_file}")
                                                    logger.info(f"Yeni DTR dosyası: {yeni_dtr_file or 'Belirtilmedi'}")
                                                    logger.info(f"Hücre dosyası: {hucre_file}")
                                                    logger.info(f"Mesken dosyası: {mesken_file or 'Belirtilmedi'}")
                                                    logger.info(f"Diğer bina dosyası: {other_file or 'Belirtilmedi'}")
                                                    
                                                    # DTR verileri analiz modülünü import et
                                                    
                                                    
                                                    # Ana dosya adını oluştur
                                                    output_prefix = f"dtr_hucresel_{region}_{district}"
                                                    output_file = os.path.join(dtr_analiz_dir, f"{output_prefix}.csv")
                                                    
                                                    # Analizi gerçekleştir
                                                    result = trafo_merkez_hucre_v2.main(
                                                        hucre_file=hucre_file,
                                                        dtr_file=dtr_file,
                                                        yeni_dtr_file=yeni_dtr_file,
                                                        mesken_file=mesken_file,
                                                        other_file=other_file,
                                                        output_file=output_file,
                                                        city=f"{region}_{district}",
                                                        year =year
                                                    )
                                                    
                                                    if result:
                                                        logger.info(f"DTR Modülü analizi başarıyla tamamlandı. Sonuç: {output_file}")
                                                        
                                                        # Özet raporu oluşturuldu
                                                        summary_file = os.path.join(dtr_analiz_dir, f"trafo_ozet_raporu_{region}_{district}.csv")
                                                        if os.path.exists(summary_file):
                                                            logger.info(f"DTR özet raporu oluşturuldu: {summary_file}")
                                                    else:
                                                        logger.error("DTR Modülü analizi başarısız oldu.")
                                                        
                                                except Exception as e:
                                                    logger.error(f"DTR Modülü analizi sırasında hata: {str(e)}")
                                                    import traceback
                                                    logger.error(traceback.format_exc())
                                        else:
                                            logger.info("DTR Modülü verisi belirtilmediği için DTR analizi atlanıyor.")
                                        
                                    except Exception as e:
                                        logger.error(f"Saturasyon analizi hatası: {str(e)}")
                                        import traceback
                                        logger.error(traceback.format_exc())
                                else:
                                    logger.error("Saturasyon analizi için gerekli kofre çıktıları bulunamadı.")
                                    if not os.path.exists(mesken_stats):
                                        logger.error(f"Mesken istatistikleri bulunamadı: {mesken_stats}")
                                    if not os.path.exists(ticaret_stats):
                                        logger.error(f"Ticaret istatistikleri bulunamadı: {ticaret_stats}")
                            else:
                                logger.info("Uydu verisi belirtilmediği için saturasyon analizi atlanıyor.")
                
                    else:
                        logger.error("Mesken ve/veya diğer bina verisi bulunamadı.")
                        logger.error(f"Aranan bölge: {region}, İlçe: {district}")
                            # DTR Modülü analizi sonrası eklenen kod
                            # Trafo Analiz Modülü
                        # Trafo Analiz Modülü
                    if "trafo_analiz" in config and config["trafo_analiz"]:
                        logger.info("Trafo Analiz Modülü başlatılıyor...")
                        
                        # Trafo analiz klasörü
                        trafo_analiz_dir = os.path.join(output_dir, "trafo_analiz")
                        os.makedirs(trafo_analiz_dir, exist_ok=True)
                        
                        # Çıktı dosyaları
                        output_file = os.path.join(trafo_analiz_dir, f"trafo_analiz_{region}_{district}_results.csv")
                        output_analysis_file = os.path.join(trafo_analiz_dir, f"trafo_analiz_{region}_{district}_analysis.csv")
                        
                        try:
                            logger.info("Trafo analizi fonksiyonu çağrılıyor...")
                            
                            # PL poligonları CSV dosyasını bul
                            pl_file = os.path.join(format_dirs["csv"], f"{output_prefix}_pl_polygons.csv")
                            
                            # Saturasyon dosyası yolunu oluştur
                            saturasyon_path = os.path.join(output_dir, "saturasyon", "saturasyon_sonuc", "saturasyon.csv")
                            if not os.path.exists(saturasyon_path):
                                logger.warning(f"Saturasyon dosyası bulunamadı: {saturasyon_path}")
                                saturasyon_path = None
                                
                            # Doğrudan fonksiyonu çağır - trafo_file ve ypdtr_file için doğrudan dtr_modulu ve yeni_dtr_modulu kullan
                            result = trafo_rezerv_alanlar_analiz.process_city_data(
                                city=region,
                                imar_file=pl_file,  # PL poligonları içeren CSV dosyası
                                hucre_file=config.get("hucre_data"),
                                overpass_file=config.get("overpass_data"),
                                trafo_file=config.get("dtr_modulu"),  # trafo_file yerine dtr_modulu kullan
                                ypdtr_file=config.get("yeni_dtr_modulu"),  # ypdtr_file yerine yeni_dtr_modulu kullan
                                saturation_file=saturasyon_path if saturasyon_path else "",
                                output_file=output_file,
                                output_analysis_file=output_analysis_file
                            )
                            
                            if result is not None:
                                logger.info(f"Trafo Analiz Modülü başarıyla tamamlandı.")
                                logger.info(f"Sonuçlar: {output_file}")
                                logger.info(f"Trafo analizi: {output_analysis_file}")
                            else:
                                logger.error("Trafo analizi başarısız oldu.")
                            
                        except Exception as e:
                            logger.error(f"Trafo Analiz Modülü çalıştırılırken hata oluştu: {str(e)}")
                            logger.error(traceback.format_exc())
                except Exception as e:
                    logger.error(f"İşlem sırasında hata oluştu: {str(e)}")
                    import traceback
                    logger.error(traceback.format_exc())
            else:
                logger.info("Hücre verisi belirtilmediği için bina kırılımları analizi atlanıyor.")
                
            elapsed_time = time.time() - start_time
            logger.info(f"İşlem tamamlandı. Geçen süre: {elapsed_time:.2f} saniye")
            return 0
   
def fetch_data_command(config):
    """Bir bölge için Overpass verisi çekme"""
    region = config["region"]
    district = config.get("district", "Merkez")  # İlçe parametresi
    output_dir = config["output_dir"]
    
    logger.info(f"Overpass verisi çekiliyor, bölge: {region}, ilçe: {district}")
    
    try:
        output_file = os.path.join(output_dir, f"overpass_data_{region}_{district}.csv")
        # İlçe bilgisini geçir
        osm_data = imar_overpass.fetch_and_process_data(district, region=region, output_file=output_file)
        logger.info(f"Veri başarıyla çekildi: {len(osm_data)} kayıt")
        logger.info(f"Veri kaydedildi: {output_file}")
        
        return 0
    except Exception as e:
        logger.error(f"Veri çekme hatası: {str(e)}")
        import traceback
        logger.error(traceback.format_exc())
        return 1

def main():
    """Uygulama giriş noktası"""
    parser = argparse.ArgumentParser(description="İmar Analiz Aracı")
    subparsers = parser.add_subparsers(title="komutlar", dest="command")
    
    # Process komutu
    process_parser = subparsers.add_parser("process", help="KML dosyasını belirli bir bölge için işle")
    process_parser.add_argument("region", help="Bölge adı (örn. Eskişehir, İzmir)")
    process_parser.add_argument("kml_file", help="KML dosyasının yolu")
    process_parser.add_argument("--district", default="Merkez", help="İlçe adı (varsayılan: Merkez)")
    
    # C# tarafından gelen yol parametreleri
    process_parser.add_argument("--output-dir", default="outputs", help="Çıktı verilerinin bulunduğu dizin")
    process_parser.add_argument("--output-prefix", help="Çıktı dosya ismi öneki")
    process_parser.add_argument("--Year", type=int, help="SLF başlangıç yılı")
    
    # Diğer parametreler
    process_parser.add_argument("--overpass-data", help="Kullanılacak hazır Overpass veri dosyası")
    process_parser.add_argument("--output-formats", default="csv,kml", help="Çıktı formatları virgülle ayrılmış (csv,kml,shp)")
    process_parser.add_argument("--save-intermediate", action="store_true", help="Ara adım sonuçlarını kaydet")
    process_parser.add_argument("--matching-table", help="İmar tipi eşleştirme tablosu")
    process_parser.add_argument("--color-scheme", help="Renk şeması JSON dosyası")
    
    # Yeni parametreler - Saturasyon analizi için gerekli parametreler
    process_parser.add_argument("--hucre-data", help="Hücre verisi dosyası (SHP veya CSV)")
    process_parser.add_argument("--uydu-data", help="Uydu verisi dosyası (CSV)")
    process_parser.add_argument("--mesken-file", help="Mesken verisi dosyası yolu")
    process_parser.add_argument("--other-file", help="Diğer binalar verisi dosyası yolu")
    
    process_parser.set_defaults(func=process_command)
    
    # Fetch-data komutu
    fetch_parser = subparsers.add_parser("fetch-data", help="Bir bölge için Overpass verisi çek")
    fetch_parser.add_argument("region", help="Bölge adı (örn. Eskişehir, İzmir)")
    fetch_parser.add_argument("--district", default="Merkez", help="İlçe adı (varsayılan: Merkez)")
    fetch_parser.add_argument("--output-dir", default="outputs", help="Çıktı verilerinin bulunduğu dizin")
    fetch_parser.set_defaults(func=fetch_data_command)
    # moduller
    process_parser.add_argument("--dtr-modulu", help="DTR Modülü verilerinin dosya yolu")
    process_parser.add_argument("--yeni-dtr-modulu", help="Yeni Projelendirilmiş DTR Modülü verilerinin dosya yolu")
    args = parser.parse_args()
    
    if args.command is None:
        parser.print_help()
        return 1
    
    # Yapılandırma oluştur
    config = vars(args)
    
    # Dizinleri belirle
    output_dir = os.path.abspath(config["output_dir"])  # Mutlak yol kullan
    
    # Alt dizinleri ana klasör içinde oluştur
    temp_dir = os.path.join(output_dir, "temp")
    log_dir = os.path.join(output_dir, "logs")
    
    # Şu anki çalışma dizinini ve mutlak yolları yazdır
    print(f"Çalışma dizini: {os.getcwd()}")
    print(f"Output dizini (mutlak): {output_dir}")
    
    # Yapılandırmayı güncelle
    config["output_dir"] = output_dir
    config["temp_dir"] = temp_dir
    config["log_dir"] = log_dir
    year = config.get("Year")  # Doğru kullanım
    # Dizinleri oluştur - sadece burada oluştur
    os.makedirs(output_dir, exist_ok=True)
    os.makedirs(temp_dir, exist_ok=True)
    os.makedirs(log_dir, exist_ok=True)
    
    print(f"Ana klasör: {output_dir}")
    print(f"Geçici klasör: {temp_dir}")
    print(f"Log klasörü: {log_dir}")
    
    # Log yapılandırması
    setup_logging(config)
    
    # Komutu çalıştır
    return args.func(config)

if __name__ == "__main__":
    sys.exit(main())