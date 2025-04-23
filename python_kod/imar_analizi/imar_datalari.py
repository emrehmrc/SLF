import xml.etree.ElementTree as ET
import re
import pandas as pd
import geopandas as gpd
from shapely.geometry import Point, Polygon
from shapely import wkt
from imar_overpass import fetch_and_process_data
import tkinter as tk
from tkinter import ttk, filedialog, messagebox
import os
import numpy as np
from sqlalchemy import create_engine
from imar_check import process_to_geodataframe
from shapely.ops import unary_union
global_data = None
selected_region = None
def createKml(unique_gdf):
    kml = ET.Element("kml")
    document = ET.SubElement(kml, "Document")

    for i, row in unique_gdf.iterrows():
        placemark = ET.SubElement(document, "Placemark")

        # styleUrl bilgisi normal olarak ekleniyor
        style_url = ET.SubElement(placemark, "styleUrl")
        style_url.text = row.get("styleUrl", "")

        # name bilgisi ekleniyor
        name = ET.SubElement(placemark, "name")
        name.text = row.get("name_1", "")

        # Geometry tipi ve koordinatlar ekleniyor
        geometry = row["geometry"]
        if isinstance(geometry, Point):
            point = ET.SubElement(placemark, "Point")
            coordinates = ET.SubElement(point, "coordinates")
            coordinates.text = f"{geometry.x},{geometry.y},0"
        elif isinstance(geometry, Polygon):
            polygon = ET.SubElement(placemark, "Polygon")
            outer_boundary = ET.SubElement(polygon, "outerBoundaryIs")
            linear_ring = ET.SubElement(outer_boundary, "LinearRing")
            coordinates = ET.SubElement(linear_ring, "coordinates")
            coordinates.text = " ".join([f"{coord[0]},{coord[1]},0" for coord in geometry.exterior.coords])

        # ExtendedData bilgileri ekleniyor (None olmayanlar)
        extended_data = ET.SubElement(placemark, "ExtendedData")
        for col in unique_gdf.columns:
            if col not in ["geometry", "name_1", "name", "Fill"] and pd.notna(row[col]):
                data = ET.SubElement(extended_data, "Data", name=col)
                value = ET.SubElement(data, "value")
                value.text = str(row[col])

        # styleUrl bilgisini ExtendedData'ya ekleme
        if pd.notna(row.get("styleUrl")):
            data = ET.SubElement(extended_data, "Data", name="styleUrl")
            value = ET.SubElement(data, "value")
            value.text = row.get("styleUrl", "")

        # Fill rengini ekleme (bgcolor olarak)
        if "Fill" in row:
            style = ET.SubElement(placemark, "Style")
            poly_style = ET.SubElement(style, "PolyStyle")
            color = ET.SubElement(poly_style, "color")
            color.text = row["Fill"].replace("#", "ff")  # KML renk formatına uygun hale getiriliyor (aabbggrr)

    # KML dosyasını yazdır
    tree = ET.ElementTree(kml)
    tree.write(f"output-kml-files/imar-sonuç-tam{selected_region}-all.kml", encoding="utf-8", xml_declaration=True)

# Eşleşme tablosu - Anahtar kelimeler ve ilgili alanlar
def katmanAyirma(kml_file):
 
 matching_table = [
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Belediye Hizmet", "Anahtar Kelimeler": r"BEL|BLD|BHZ|BHA|HIZMET|IHA", "Katman Adi": "PL_BEL_HIZ"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Belediye Hizmet", "Anahtar Kelimeler": r"BEL|BLD|BHZ|BHA|HIZMET|IHA", "Katman Adi": "PL_BHA_IZSU_3"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Metropoliten", "Anahtar Kelimeler": r"METROPOL", "Katman Adi": "PL_1_2_3_METROPOLITEN"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Tiyatro", "Anahtar Kelimeler": r"TIYATRO|SANAT", "Katman Adi": "PL_ACIKHAVA_TIYATRO"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Akaryakit İstasyonu", "Anahtar Kelimeler": r"AKAR|POMPA", "Katman Adi": "PL_AKARYAKIT_POMPA"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Dini Tesis", "Anahtar Kelimeler": r"DINI|IBADET|CAMİİ|CAMI|KILISE|CEMEVI|CAMİ", "Katman Adi": "PL_DINI_SOS_TES"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Emniyet", "Anahtar Kelimeler": r"EMN|KARAKOL", "Katman Adi": "PL_EMNIYET"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Spor Tesisi", "Anahtar Kelimeler": r"SPOR|OYUN|STADYUM", "Katman Adi": "PL_KAPALI_SPOR_TES"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Sosyal Tesisler", "Anahtar Kelimeler": r"SOS|HUZUREVI|BAKIMEVI", "Katman Adi": "PL_IDARI_SOSYAL_TES"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Aritma Tesisi, Soğuk Hava", "Anahtar Kelimeler": r"ARITMA|SOGUKHAVA", "Katman Adi": "PL_ARITMA_DOGAL"}, 
    {"İmar Tipi (Land-Use)": "Kentsel Dönüşüm", "Katman Adlandirma": "Kentsel Dönüşüm", "Anahtar Kelimeler": r"KENTSEL|GELISME|KONUTG", "Katman Adi": "PL_KENTSEL_CALISMA"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Otopark", "Anahtar Kelimeler": r"OTOPARK|BOP|OP", "Katman Adi": "PL_OTOPARK"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Kütüphane", "Anahtar Kelimeler": r"KUTUP", "Katman Adi": "PL_KUTUPHANE"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Eğitim Kurumu", "Anahtar Kelimeler": r"LISE|OKUL|GITIM|RETIM|VERSITE|KRES", "Katman Adi": "PL_LISE_ALANI"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Sağlik Tesisi", "Anahtar Kelimeler": r"SAGL|DISPANSER|HASTANE", "Katman Adi": "PL_SAGLIK_ALANLARI"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Pazar Alani", "Anahtar Kelimeler": r"PAZAR", "Katman Adi": "PL_PAZARLAMA_ALANI"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Gar", "Anahtar Kelimeler": r"RAY|TCDD|ISTASYON", "Katman Adi": "PL_RAY_ISTASYON"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Resmi Alan", "Anahtar Kelimeler": r"RESMI", "Katman Adi": "PL_RESMI_KURUM"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Ticarethane", "Anahtar Kelimeler": r"TICARET|TT|Ti", "Katman Adi": "PL_TICARET_BOLGESI"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Trafo Alani", "Anahtar Kelimeler": r"TRAFO|DT", "Katman Adi": "PL_TRAFO_ALANI_3"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Askeri Alan", "Anahtar Kelimeler": r"ASK|JANDA", "Katman Adi": "PL_ASKERI_ALAN"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Park Alani", "Anahtar Kelimeler": r"PARK|COC", "Katman Adi": "PL_COCUK_BAH"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Haberleşme Tesisleri", "Anahtar Kelimeler": r"TELEKOM|HABERLESME", "Katman Adi": "PL_TELEKOM_ALANI"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Turistik Alan", "Anahtar Kelimeler": r"TURIST", "Katman Adi": "PL_TURISTIK_TESIS_A"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Konut Alani", "Anahtar Kelimeler": r"KONUT|MESKEN|KT|M_|_M", "Katman Adi": "PL_KONUT"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Yeşil Alan", "Anahtar Kelimeler": r"YESIL|REKREASYON", "Katman Adi": "PL_PASIF_YESIL"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Kültürel Tesis", "Anahtar Kelimeler": r"KULTUREL|KULT|KÜLT", "Katman Adi": "PL_KULTUREL_TESIS"},
    {"İmar Tipi (Land-Use)": "Ticarethane-Konut", "Katman Adlandirma": "Ticarethane-Konut Alanlari", "Anahtar Kelimeler": r"TICK", "Katman Adi": "PL_TICK_1"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Etaplama Alani", "Anahtar Kelimeler": r"ETAPLAMA", "Katman Adi": "PL_2_ETAPLAMA_ALAN"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Ağaçlandirilacak Alan", "Anahtar Kelimeler": r"AGAC", "Katman Adi": "PL_AGACLANDIRILACAK"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Regületör", "Anahtar Kelimeler": r"REGULATOR|DOGALGAZ|KANAL|GOLET", "Katman Adi": "PL_BOLGE_REGULATOR"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Doğal Alan", "Anahtar Kelimeler": r"ORMAN|DKKA|DOGAL_YASAM|DERE", "Katman Adi": "PL_DOGAL_YASAM_ADA"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Durak", "Anahtar Kelimeler": r"DURAK", "Katman Adi": "PL_DURAK"},
    {"İmar Tipi (Land-Use)": "Sanayi", "Katman Adlandirma": "Depolama", "Anahtar Kelimeler": r"KATI_ATIK|DEPOLAMA", "Katman Adi": "PL_KATI_ATIK_DEPONI"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Meydan", "Anahtar Kelimeler": r"MEYDAN", "Katman Adi": "PL_MEYDAN"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Mezar", "Anahtar Kelimeler": r"MEZAR", "Katman Adi": "PL_MEZARLIK"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Refüj", "Anahtar Kelimeler": r"REFUJ|REFÜJ", "Katman Adi": "PL_REFUJ"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Terminal", "Anahtar Kelimeler": r"TERMINAL", "Katman Adi": "PL_TERMINAL_2"},
    {"İmar Tipi (Land-Use)": "Sanayi", "Katman Adlandirma": "Üretim Tesisi", "Anahtar Kelimeler": r"ENER|URET", "Katman Adi": "PL_BHA_ENERJI_URETIM"},
    {"İmar Tipi (Land-Use)": "Sanayi", "Katman Adlandirma": "Atik Su Tesisi", "Anahtar Kelimeler": r"ATIK", "Katman Adi": "PL_ATIKSU_TESISI"},
    {"İmar Tipi (Land-Use)": "Sanayi", "Katman Adlandirma": "Sanayi", "Anahtar Kelimeler": r"SAN|IMALAT", "Katman Adi": "PL_KUCUK_SAN_TIC"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "OSB", "Anahtar Kelimeler": r"OSB|Orga", "Katman Adi": "PL_OSB_2"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Dolgu Alani", "Anahtar Kelimeler": r"DOLGU", "Katman Adi": "PL_DOLGU_ALANI"},
    {"İmar Tipi (Land-Use)": "Tarimsal Sulama", "Katman Adlandirma": "Tarim ve Hayvancilik Tesis Alani", "Anahtar Kelimeler": r"TARIM|TAR", "Katman Adi": ""},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Köysel Yerleşim", "Anahtar Kelimeler": r"KOYYER", "Katman Adi": "PL_KOYYERLESIM"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Yurt", "Anahtar Kelimeler": r"YURT", "Katman Adi": "PL_YURT"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Fuar Alani", "Anahtar Kelimeler": r"FUAR", "Katman Adi": "PL_FUAR"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "İş Merkezi", "Anahtar Kelimeler": r"ISMERKEZ|RKEZI_IS", "Katman Adi": "PL_ISMERKEZI"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Jeolojik Sakincali", "Anahtar Kelimeler": r"JEOLOJIK", "Katman Adi": "PL_JEOLOJIKSAKINCA"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Katli Otopark", "Anahtar Kelimeler": r"KATLIOTO", "Katman Adi": "PL_KATLIOTOPARK"},
    {"İmar Tipi (Land-Use)": "Tarimsal Sulama", "Katman Adlandirma": "Özel Proje Alani", "Anahtar Kelimeler": r"OPA|OZEL_PRO|ÖZEL_PRO", "Katman Adi": "PL_OPA"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Uygun Alanlar (UA)", "Anahtar Kelimeler": r"UA", "Katman Adi": "PL_UA"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Konut Dişi Kentsel Çalişma Alani", "Anahtar Kelimeler": r"KDK", "Katman Adi": "PL_KDKCA"},
    {"İmar Tipi (Land-Use)": "Yasaklİ Alan", "Katman Adlandirma": "Altyapİ Tesisi", "Anahtar Kelimeler": r"ALTY|KALT", "Katman Adİ": "PL_TEKNIK_ALTYAPI"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "İş Merkezi", "Anahtar Kelimeler": r"TAL|TALİ|TALI", "Katman Adi": "PL_2_3_TALI_IS_MER"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "İletim Tesisi ve İşletme Alani", "Anahtar Kelimeler": r"K_HA", "Katman Adi": "PL_TEKNIK_HA_3"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Ticarethane-Konut Alanlari", "Anahtar Kelimeler": r"IS_AL|İS_AL|TURIZM|TATIL|OTEL", "Katman Adi": "PL_MERKEZI_IS_ALANI"},
    # {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Adakenari", "Anahtar Kelimeler": r"ADAK","Katman Adi": "PL_ADAKENARI"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Dinlenme Tesisi", "Anahtar Kelimeler": r"RLIK_TE|GUNU","Katman Adi": "PL_GUNUBIRLIK_TESIS"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Konut Alani", "Anahtar Kelimeler": r"ALLE_M","Katman Adi": "PL_MAHALLE_MER"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Su Kanali", "Anahtar Kelimeler": r"SU_Y","Katman Adi": "PL_SU_YUZEYI"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Milli Park", "Anahtar Kelimeler": r"zla","Katman Adi": "PL_TUZLA"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Ticarethane-Konut Alanlari", "Anahtar Kelimeler": r"_TM","Katman Adi": "PL_TEK_CEPHE_TM "},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Konut Alani", "Anahtar Kelimeler": r"SAÐ","Katman Adi": "PL_SAÐLIK10"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Konut Alani", "Anahtar Kelimeler": r"T_M","Katman Adi": "PL_T_M"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Konut Alani", "Anahtar Kelimeler": r"SU_DEP","Katman Adi": "PL_SU_DEPOSU"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Demir yolu", "Anahtar Kelimeler": r"RYOLU|IRYOLU","Katman Adi": "#PL_DEMIRYOLU22"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Agaclik Alan", "Anahtar Kelimeler": r"OA|O_A","Katman Adi": "##PL_OA"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Arazi tanimsiz", "Anahtar Kelimeler": r"LEJA","Katman Adi": "##PL_LEJANT"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "PORSUK CAYI", "Anahtar Kelimeler": r"PORS","Katman Adi": "##PL_PORSUK"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Kaldirim", "Anahtar Kelimeler": r"KALD","Katman Adi": "##PL_KALDIRIM"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "DERE", "Anahtar Kelimeler": r"DERIV","Katman Adi": "#DERIVASYON"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "SULAMA KANALİ", "Anahtar Kelimeler": r"SULAK","Katman Adi": "#SULAK"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Boru Hatti", "Anahtar Kelimeler": r"BASIN","Katman Adi": "#HAT_BASINC"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "YOL", "Anahtar Kelimeler": r"YOL","Katman Adi": "#HAT_YOL"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Ormanlik Alan", "Anahtar Kelimeler": r"KA_","Katman Adi": "#KA_"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Deniz Koruma alani", "Anahtar Kelimeler": r"ONLEM|ÖNLEM","Katman Adi": "#KST_ONLEMLİ_ALAN"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Konut Alani", "Anahtar Kelimeler": r"STAB_SOR","Katman Adi": "#STAB_SORUN"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Bisiklet Yolu", "Anahtar Kelimeler": r"HAT_BIS","Katman Adi": "#STAB_SORUN"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "İskele", "Anahtar Kelimeler": r"ISK|ISKE","Katman Adi": "#PL_ISKELE"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Yaya Gecidi", "Anahtar Kelimeler": r"YAY","Katman Adi": "#PL_YAYA_GEC"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Rehabilitasyon merkezi", "Anahtar Kelimeler": r"REHAB","Katman Adi": "#PL_O_REHABILITASYON"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Çicek Bahçesi", "Anahtar Kelimeler": r"CICE|CİCE","Katman Adi": "#TAS_CICEK_BAHCESI"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Büfe", "Anahtar Kelimeler": r"BUFE","Katman Adi": "#TAS_BUFE"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Ant Alan", "Anahtar Kelimeler": r"ANIT","Katman Adi": "#TAS_ANIT_TOREN_ALAN"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Park alani", "Anahtar Kelimeler": r"IK_AL","Katman Adi": "#TAS_ACIK_ALAN"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Regülatör", "Anahtar Kelimeler": r"REGUL","Katman Adi": "#PL_BOLGESEL_REGULATR"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Mesken", "Anahtar Kelimeler": r"BINA|BİNA","Katman Adi": "#TESCILLI_BINA_3"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Park", "Anahtar Kelimeler": r"TAS_","Katman Adi": "#TAS_"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Cizim", "Anahtar Kelimeler": r"HAT_DET","Katman Adi": "#HAT_DETAY"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Mesken", "Anahtar Kelimeler": r"KONTROL_ADA","Katman Adi": "#KONTROL_ADA_AYRIM"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Mesken", "Anahtar Kelimeler": r"TESC","Katman Adi": "#TESCILLI_PARSEL"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Marina-Barinak", "Anahtar Kelimeler": r"TESC","Katman Adi": "#TESCILLI_PARSEL"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Bataklik", "Anahtar Kelimeler": r"BATAK","Katman Adi": "#PL_BATAKLIK"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Marina Tekne Otoparki", "Anahtar Kelimeler": r"CEKEK|BARIN","Katman Adi": "#PL_CEKEK"}, 
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Havuzlar", "Anahtar Kelimeler": r"swimming_pool", "Katman Adi": "leisure"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Stadyum", "Anahtar Kelimeler": r"stadium", "Katman Adi": "leisure"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Sosyal Hizmet Yapıları", "Anahtar Kelimeler": r"community_centre", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Kamu Hizmeti", "Katman Adlandirma": "Belediye Hizmeti", "Anahtar Kelimeler": r"civic", "Katman Adi": "building"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Müze", "Anahtar Kelimeler": r"museum", "Katman Adi": "tourism"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Trenyolu", "Anahtar Kelimeler": r"station", "Katman Adi": "railway"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Benzin İstasyonu", "Anahtar Kelimeler": r"fuel", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Depo Alanı", "Anahtar Kelimeler": r"warehouse", "Katman Adi": "building"},
    {"İmar Tipi (Land-Use)": "Sanayi", "Katman Adlandirma": "Endüstriyel Üretim Alanı", "Anahtar Kelimeler": r"industrial", "Katman Adi": "landuse"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan","Katman Adlandirma": "Otobus Durağı", "Anahtar Kelimeler": r"bus_station", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Apartman", "Anahtar Kelimeler": r"apartments", "Katman Adi": "building"},
    {"İmar Tipi (Land-Use)": "Mesken", "Katman Adlandirma": "Ev", "Anahtar Kelimeler": r"house", "Katman Adi": "building"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Kafe", "Anahtar Kelimeler": r"cafe", "Katman Adi": "building"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Restorant", "Anahtar Kelimeler": r"restaurant", "Katman Adi": "building"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Camii", "Anahtar Kelimeler": r"mosque", "Katman Adi": "building"},
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Tren İstasyonu", "Anahtar Kelimeler": r"train_station", "Katman Adi": "building"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Supermarket", "Anahtar Kelimeler": r"supermarket", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Bakkal", "Anahtar Kelimeler": r"convenience", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Küçük Magaza", "Anahtar Kelimeler": r"boutique", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Kirtasiye", "Anahtar Kelimeler": r"stationery", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Mobilya Satiş Noktası", "Anahtar Kelimeler": r"furniture", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Elektronik Satiş Noktaları", "Anahtar Kelimeler": r"electronics", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Kuyumcu", "Anahtar Kelimeler": r"jewelry", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Telefon Satiş Noktasi", "Anahtar Kelimeler": r"mobile_phone", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Kitapevi", "Anahtar Kelimeler": r"books", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Çicek Satis", "Anahtar Kelimeler": r"florist", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Kuafor", "Anahtar Kelimeler": r"hairdresser", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Avm-Büyük Magazalar", "Anahtar Kelimeler": r"mall", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Oto Bakım", "Anahtar Kelimeler": r"car_repair", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Araç Satış-Bakım Yerleri", "Anahtar Kelimeler": r"car", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Pastane", "Anahtar Kelimeler": r"bakery", "Katman Adi": "shop"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Okul", "Anahtar Kelimeler": r"school", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Diş Klinigi", "Anahtar Kelimeler": r"dentist", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Eczane", "Anahtar Kelimeler": r"pharmacy", "Katman Adi": "healthcare"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Anaokulu", "Anahtar Kelimeler": r"kindergarten", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Devlet Binalari", "Anahtar Kelimeler": r"public_building", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Belediye Hizmetleri", "Anahtar Kelimeler": r"townhall", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Adliye", "Anahtar Kelimeler": r"courthouse", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Ofis", "Anahtar Kelimeler": r"office", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Ticari Faaliyet", "Anahtar Kelimeler": r"commercial", "Katman Adi": "building"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "üniversite", "Anahtar Kelimeler": r"university", "Katman Adi": "amenity"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Kütüphane", "Anahtar Kelimeler": r"library", "Katman Adi": "amenity"},  
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Hastane", "Anahtar Kelimeler": r"hospital", "Katman Adi": "healthcare"},
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Klinik", "Anahtar Kelimeler": r"clinic", "Katman Adi": "amenity"}, 
    {"İmar Tipi (Land-Use)": "Ticarethane", "Katman Adlandirma": "Spormerkezi", "Anahtar Kelimeler": r"sports_centre", "Katman Adi": "leisure"}
        ]
 color_dict = {
    "Ticarethane": "#FF6347",  # Tomato
    "Yasakli Alan": "#4682B4",  # SteelBlue
    "Mesken": "#32CD32",        # LimeGreen
    "Sanayi": "#FFD700",        # Gold
    "Kentsel Dönüşüm": "#8A2BE2",  # BlueViolet
    "Tarimsal Sulama": "#DAA520",  # GoldenRod
    "Kamu Hizmeti": "#FF4500",  # OrangeRed
    "Kamu Tesisi": "#7FFFD4",   # Aquamarine
    # Daha fazla kategoriye renk ekleyebilirsiniz
 }
    # 'Katman Adi' ve 'İmar Tipi' sütunlarını ekleyelim
        # 'Katman Adlandırma' ve 'İmar Tipi (Land-Use)' sütunlarını ekleyelim
 kml_file['Katman Adlandırma'] = ''
 kml_file['İmar Tipi (Land-Use)'] = ''
 kml_file['Fill']= ''
    # Her bir satırı matching_table ile eşleştir
 for i, row in kml_file.iterrows():
    matched = False
    building_value = row.get('building', '')
    
    # Building=yes durumu için özel kontrol
    if isinstance(building_value, str) and building_value.lower() == 'yes':
        # Önce diğer sütunlarda değer var mı kontrol et
        for col in ['amenity', 'landuse', 'shop', 'office', 'tourism', 'healthcare', 'leisure']:
            col_value = row.get(col)
            if pd.notna(col_value) and str(col_value).strip() != '':
                # Matching table'dan eşleştirme yap
                for entry in matching_table:
                    if re.search(entry["Anahtar Kelimeler"], str(col_value), re.IGNORECASE):
                        # Building değerini değiştirmiyoruz, sadece İmar Tipi ve diğer özellikleri güncelliyoruz
                        kml_file.at[i, 'İmar Tipi (Land-Use)'] = entry['İmar Tipi (Land-Use)']
                        kml_file.at[i, 'Katman Adlandırma'] = entry['Katman Adlandirma']
                        kml_file.at[i, 'Fill'] = color_dict.get(entry['İmar Tipi (Land-Use)'], '#FFFFFF')
                        matched = True
                        break
                if matched:
                    break
        
        # Diğer sütunlarda eşleşme yoksa styleUrl'e bak
        if not matched:
            style_url_text = str(row.get('styleUrl', '')).strip()
            for entry in matching_table:
                if re.search(entry["Anahtar Kelimeler"], style_url_text, re.IGNORECASE):
                    kml_file.at[i, 'İmar Tipi (Land-Use)'] = entry['İmar Tipi (Land-Use)']
                    kml_file.at[i, 'Katman Adlandırma'] = entry['Katman Adlandirma']
                    kml_file.at[i, 'Fill'] = color_dict.get(entry['İmar Tipi (Land-Use)'], '#FFFFFF')
                    matched = True
                    break
    
    # Building=yes değilse normal işlem
    else:
        # Önce diğer sütunlarda değer var mı kontrol et
        for col in ['amenity', 'landuse', 'shop', 'office', 'tourism', 'healthcare', 'leisure']:
            col_value = row.get(col)
            if pd.notna(col_value) and str(col_value).strip() != '':
                # Matching table'dan eşleştirme yap
                for entry in matching_table:
                    if re.search(entry["Anahtar Kelimeler"], str(col_value), re.IGNORECASE):
                        kml_file.at[i, 'İmar Tipi (Land-Use)'] = entry['İmar Tipi (Land-Use)']
                        kml_file.at[i, 'Katman Adlandırma'] = entry['Katman Adlandirma']
                        kml_file.at[i, 'Fill'] = color_dict.get(entry['İmar Tipi (Land-Use)'], '#FFFFFF')
                        matched = True
                        break
                if matched:
                    break
        
        # Diğer sütunlarda eşleşme yoksa styleUrl'e bak
        if not matched:
            style_url_text = str(row.get('styleUrl', '')).strip()
            for entry in matching_table:
                if re.search(entry["Anahtar Kelimeler"], style_url_text, re.IGNORECASE):
                    kml_file.at[i, 'İmar Tipi (Land-Use)'] = entry['İmar Tipi (Land-Use)']
                    kml_file.at[i, 'Katman Adlandırma'] = entry['Katman Adlandirma']
                    kml_file.at[i, 'Fill'] = color_dict.get(entry['İmar Tipi (Land-Use)'], '#FFFFFF')
                    matched = True
                    break

    # Hiç eşleşme bulunamadıysa
    if not matched:
        kml_file.at[i, 'İmar Tipi (Land-Use)'] = 'unclassified'
        kml_file.at[i, 'Katman Adlandırma'] = 'unclassified'
        kml_file.at[i, 'Fill'] = '#FFFFFF'


# Sütunların sırasını değiştirelim: 'Katman Adlandırma' ve 'İmar Tipi (Land-Use)' ilk iki sütun olsun
 columns_order = ['Katman Adlandırma', 'İmar Tipi (Land-Use)'] + [col for col in kml_file.columns if col not in ['Katman Adlandırma', 'İmar Tipi (Land-Use)']]
 kml_file = kml_file[columns_order]

# name ve name_1 sütunlarını düzenle
 if 'name_1' in kml_file.columns and 'name' in kml_file.columns:
    kml_file['name_1'] = kml_file['name_1'].fillna(kml_file['name'])
    
 if 'coords' in kml_file.columns:
        del kml_file['coords']
    
 if 'name' in kml_file.columns and ('name_1' in kml_file.columns or 'name_2' in kml_file.columns):
        del kml_file['name']

 unique_gdf = kml_file.drop_duplicates(subset='geometry') 

 # Sonra yazdır  
 print("\nUnique GeoDataFrame'in ilk 5 satırı:")
 pd.set_option('display.max_columns', None)  # Tüm kolonları göster
 pd.set_option('display.width', None)        # Satır genişliğini sınırlama
 pd.set_option('display.max_colwidth', None) # Kolon içeriğini tam göster
      
# 1. IMAR ID verilerini puanlardan ekleyin
 unique_gdf.to_csv(f"./output-shp-files/unique_gdf_{selected_region}.csv", encoding="utf-8-sig", index=False)
 add_imar_ids_from_points_mesken(unique_gdf,selected_region)
 add_imar_ids_from_points_ticarethane(unique_gdf, selected_region)
 analyze_all_ticarethane(unique_gdf,selected_region)
# 2. hmax değerlerini temizleyin ve güncelleyin
 clean_and_test_hmax(unique_gdf)

# 3. IMAR verilerini analiz edin ve IMAR ID'lerini güncelleyin
 analyze_and_update_imar(unique_gdf)
#  process_region_data(selected_region)
# 4. hmax ve diğer güncellenmiş değerlerin CSV dosyalarını kaydedin
 hmax_values = unique_gdf['hmax'].drop_duplicates()
 hmax_values.to_csv('output-files/buffer-deneme/hmax_original_values.csv', header=True, index=False, encoding='utf-8-sig')
 
    # Sonra styleUrl temizliği yap
 unique_gdf = unique_gdf[unique_gdf['styleUrl'].str.startswith('#', na=False)].copy()
    
    # Temizlenmiş veriyi kaydet
 unique_gdf.to_csv(f'output-files/sonuc-{selected_region}-imar.csv', index=False, encoding='utf-8-sig')


# 5. IMAR ID 5 analizi gerçekleştirin
 calc_imar_results(unique_gdf,selected_region)

# 6. Sonuçları kaydetmek ve KML dosyası oluşturmak
 create_single_shp_with_buffer_preserving_data(unique_gdf)
 return createKml(unique_gdf)
#  


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


def clean_and_test_hmax(df):
    """
    hmax kolonunu temizler ve test raporu oluşturur
    """
    def clean_value(value):
        original_value = value
        result = None
        conversion_note = ""
        
        try:
            if pd.isna(value):
                conversion_note = "Boş değer"
                return None, original_value, conversion_note
                
            if isinstance(value, (int, float)):
                result = float(value)
                conversion_note = "Direkt sayısal dönüşüm"
                
            elif isinstance(value, str):
                value = value.lower().strip()
                
                if 'serbest' in value:
                    conversion_note = "Serbest değer"
                    result = None
                    
                elif 'kat' in value:
                    kat_match = re.findall(r'(\d+)\s*kat', value)
                    if kat_match:
                        result = float(kat_match[0]) * 2.5
                        conversion_note = f"{kat_match[0]} kat * 2.5m"
                
                else:
                    value = value.replace(',', '.')
                    numbers = re.findall(r'\d+\.?\d*', value)
                    if numbers:
                        num = float(numbers[0])
                        if 0 < num < 200:
                            result = num
                            conversion_note = "Sayısal değer çıkarıldı"
                        else:
                            conversion_note = "Değer aralık dışında"
                    else:
                        conversion_note = "Sayısal değer bulunamadı"
                    
        except Exception as e:
            conversion_note = f"Hata: {str(e)}"
            
        return result, original_value, conversion_note

    print("Orijinal benzersiz değerler:")
    print(df['hmax'].unique())
    
    results = df['hmax'].apply(clean_value)
    
    df['hmax_clean'] = [r[0] for r in results]
    df['hmax_original'] = [r[1] for r in results]
    df['hmax_note'] = [r[2] for r in results]
    
    print("\nDönüşüm örnekleri (ilk 20 kayıt):")
    print(df[['hmax_original', 'hmax_clean', 'hmax_note']].head(20))
    
    unique_transformations = df[['hmax_original', 'hmax_clean', 'hmax_note']].drop_duplicates().sort_values('hmax_original')
    print("\nBenzersiz dönüşümler:")
    print(unique_transformations)
    
    print("\nTemizlenmiş değerlerin istatistikleri:")
    print(df['hmax_clean'].describe())
    
    null_count = df['hmax_clean'].isnull().sum()
    total_count = len(df)
    print(f"\nBoş değer sayısı: {null_count} ({(null_count/total_count*100):.2f}%)")
    
    valid_values = df['hmax_clean'].dropna()
    print("\nDeğer aralığı:")
    print(f"Minimum: {valid_values.min():.2f}")
    print(f"Maksimum: {valid_values.max():.2f}")
    
    df['hmax'] = df['hmax_clean']
    
    return df


def analyze_and_update_imar(gdf):
    """
    İmar verilerini analiz eder ve günceller
    """
    print("\n=== Analiz Başlangıç ===")
    
    required_columns = ['IMAR_ID', 'parcel_name', 'styleUrl', 'parsel_alan_m2', 'hmax']
    missing_columns = [col for col in required_columns if col not in gdf.columns]
    
    if missing_columns:
        print(f"HATA: Eksik kolonlar: {missing_columns}")
        return gdf
    
    gdf['hmax_numeric'] = gdf['hmax'].apply(clean_hmax)
    
    print(f"Toplam kayıt sayısı: {len(gdf)}")
    print(f"Unique parcel_name sayısı: {len(gdf['parcel_name'].unique())}")
    
    try:
        before_count = gdf['IMAR_ID'].value_counts()
        print("\nBaşlangıç IMAR_ID dağılımı:")
        print(before_count)
        
        for parcel_name in gdf['parcel_name'].unique():
            parcel_group = gdf[gdf['parcel_name'] == parcel_name]
            target_imars = parcel_group[parcel_group['IMAR_ID'].isin(['1', '5'])]
            
            if len(target_imars) > 0:
                imar_counts = target_imars['IMAR_ID'].value_counts()
                total_count = len(target_imars)
                
                pct_5 = (imar_counts.get('5', 0) / total_count * 100)
                pct_1 = (imar_counts.get('1', 0) / total_count * 100)
                
                if pct_5 > 47:
                    mask = (gdf['parcel_name'] == parcel_name) & (gdf['IMAR_ID'] == '1')
                    gdf.loc[mask, 'IMAR_ID'] = '5'
                
                elif pct_1 > 38:
                    mask = (gdf['parcel_name'] == parcel_name) & (gdf['IMAR_ID'] == '5')
                    gdf.loc[mask, 'IMAR_ID'] = '1'
                
                else:
                    if pct_5 > pct_1:
                        mask = (gdf['parcel_name'] == parcel_name) & (gdf['IMAR_ID'] == '1')
                        gdf.loc[mask, 'IMAR_ID'] = '5'
                    elif pct_1 > pct_5:
                        mask = (gdf['parcel_name'] == parcel_name) & (gdf['IMAR_ID'] == '5')
                        gdf.loc[mask, 'IMAR_ID'] = '1'
        
        after_count = gdf['IMAR_ID'].value_counts()
        changes = after_count - before_count
        
        print("\nFinal IMAR_ID değişiklikleri:")
        print("Öncesi:", before_count)
        print("Sonrası:", after_count)
        print("\nNet değişimler:")
        print(changes)
        
    except Exception as e:
        print(f"\nHATA: {str(e)}")
        return gdf
    
    return gdf
def analyze_all_ticarethane(gdf, selected_region):
    # Sonuçları tutacak liste
    all_results = []
    
    # Building types to process
    building_types = [
        ('KUCUK_TICARETHANE', 'TICARETHANE'),
        ('ORTA_TICARETHANE', 'TICARETHANE'),
        ('BUYUK_TICARETHANE', 'TICARETHANE'),
        ('KUCUK_SANAYI', 'SANAYI'),
        ('ORTA_SANAYI', 'SANAYI'),
        ('BUYUK_SANAYI', 'SANAYI')
    ]
    
    for building_type, consumption_type in building_types:
        mask = (
            (gdf['BINA_TIPI'] == building_type) &
            (gdf['TAKS'] <= 1) &
            (gdf[f'{consumption_type}_COUNT'] > 0) &  # Abone sayısı kontrolü
            (gdf[f'{consumption_type}_TUKETIM'] > 0)  # Tüketim kontrolü
        )
        filtered = gdf[mask].copy()
        
        # Calculate consumption metrics
        filtered['tuketim_per_m2'] = filtered[f'{consumption_type}_TUKETIM'] / filtered['bina_alan_m2_old']
        filtered['tuketim_per_abone'] = filtered[f'{consumption_type}_TUKETIM'] / filtered[f'{consumption_type}_COUNT']
        
        # Aggregate results
        results = {
            'Bina Tipi': building_type,
            'Toplam Sample': len(filtered),
            'Ortalama TAKS': filtered['TAKS'].mean(),
            'Ortalama Bina Alanı': filtered['bina_alan_m2_old'].mean(),
            'Min Bina Alanı': filtered['bina_alan_m2_old'].min(),
            'Max Bina Alanı': filtered['bina_alan_m2_old'].max(),
            'Toplam Abone Sayısı': filtered[f'{consumption_type}_COUNT'].sum(),
            'Ortalama Tüketim (kWh/m²)': filtered['tuketim_per_m2'].mean(),
            'Min Tüketim (kWh/m²)': filtered['tuketim_per_m2'].min(),
            'Max Tüketim (kWh/m²)': filtered['tuketim_per_m2'].max(),
            'Ortalama Tüketim (kWh/Abone)': filtered['tuketim_per_abone'].mean(),
            'Min Tüketim (kWh/Abone)': filtered['tuketim_per_abone'].min(),
            'Max Tüketim (kWh/Abone)': filtered['tuketim_per_abone'].max()
        }
        all_results.append(results)
    
    # Sonuçları DataFrame'e çevir
    results_df = pd.DataFrame(all_results)
    
    # Excel'e kaydet
    results_df.to_excel(f'ticarethane_sanayi_analizleri-{selected_region}.xlsx', index=False)
    
    # Sonuçları yazdır
    for result in all_results:
        print(f"\n{result['Bina Tipi']} Sonuçları:")
        print(f"Toplam Sample: {result['Toplam Sample']}")
        print(f"Toplam Abone Sayısı: {result['Toplam Abone Sayısı']}")
        print(f"Ortalama Bina Alanı: {result['Ortalama Bina Alanı']:.2f}")
        print(f"Min Bina Alanı: {result['Min Bina Alanı']:.2f}")
        print(f"Max Bina Alanı: {result['Max Bina Alanı']:.2f}")
        print(f"Ortalama Tüketim (kWh/m²): {result['Ortalama Tüketim (kWh/m²)']:.2f}")
        print(f"Min Tüketim (kWh/m²): {result['Min Tüketim (kWh/m²)']:.2f}")
        print(f"Max Tüketim (kWh/m²): {result['Max Tüketim (kWh/m²)']:.2f}")
        print(f"Ortalama Tüketim (kWh/Abone): {result['Ortalama Tüketim (kWh/Abone)']:.2f}")
        print(f"Min Tüketim (kWh/Abone): {result['Min Tüketim (kWh/Abone)']:.2f}")
        print(f"Max Tüketim (kWh/Abone): {result['Max Tüketim (kWh/Abone)']:.2f}")
    
    return results_df
def calc_imar_results(gdf, selected_region):
    """
    İmar ID 1, 2, 3, 4 ve 5 analizi ve tüketim hesaplamaları
    """
    def process_imar_id(data, imar_id, hmax_values=None, m2_limit=None):
        # Base filters
        filters = [
            (data['IMAR_ID'] == str(imar_id)),
            (data['bina_alan_m2_old'].notna()),
            (data['TAKS'].between(0.05, 0.9)),
            (data['ABONE_SAYISI'] > 0),
            (data['ORT_MESKEN_TUKETIM'] > 0)
        ]
        
        # Alan sınırı varsa ekle
        if m2_limit:
            filters.append(data['bina_alan_m2_old'] <= m2_limit)
            
        base = data[np.all(filters, axis=0)].copy()
        
        # Sayısal dönüşümler
        numeric_columns = ['hmax', 'TAKS', 'imar_taks', 'imar_kaks', 'bina_alan_m2', 
                         'bina_alan_m2_old', 'levels', 'ABONE_SAYISI', 'ORT_MESKEN_TUKETIM']
        for col in numeric_columns:
            if col in base.columns:
                base[col] = pd.to_numeric(base[col], errors='coerce')
        
        # Tüketim hesaplamaları
        base['total_tuketim'] = base['ORT_MESKEN_TUKETIM'] * base['ABONE_SAYISI']  # Binanın toplam tüketimi
        base['tuketim_kwh_m2'] = base['total_tuketim'] / base['bina_alan_m2_old']  # kWh/m²
        base['tuketim_kwh_abone'] = base['total_tuketim'] / base['ABONE_SAYISI']   # kWh/Abone
        
        # İmar ID'ye özel filtreler
        if imar_id == '2':
            hmax_values = [2.8, 4.5, 5.8, 6.8, 9.8, 12.0, 12.8, 15.8, 16.8]
            valid_mask = (
                (base['hmax'].isin(hmax_values)) |
                (
                    (base['imar_kaks'].notna() & base['imar_taks'].notna()) &
                    ((base['imar_kaks'] / base['imar_taks']).between(1, 4))
                ) |
                ((base['imar_kaks'].isna() | base['imar_taks'].isna()) & 
                 (base['hmax'].isna()))
            )
            valid_data = base[valid_mask]
        elif imar_id == '3':
            hmax_values = [15.0, 16.8, 17.8, 18.8, 20.0, 20.8, 21.8, 24.8, 27.8, 26.8]
            valid_mask = (
                (base['hmax'].isin(hmax_values)) |
                (
                    (base['imar_kaks'].notna() & base['imar_taks'].notna()) &
                    ((base['imar_kaks'] / base['imar_taks']).between(4, 7))
                ) |
                ((base['bina_alan_m2_old'] > 90) & 
                 (base['imar_kaks'].isna() | base['imar_taks'].isna()) & 
                 (base['hmax'].isna()))
            )
            valid_data = base[valid_mask]
        elif imar_id == '4':
            hmax_values = [24.8, 27.0, 27.8, 28.5, 30.0, 34.5, 69.8]
            valid_mask = (
                (base['hmax'].isin(hmax_values)) |
                (
                    (base['imar_kaks'].notna() & base['imar_taks'].notna()) &
                    ((base['imar_kaks'] / base['imar_taks']) > 7)
                ) |
                (base['levels'] > 8) |
                (
                    (base['imar_taks'].notna()) &
                    (base['imar_taks'].between(0.05, 0.95))
                )
            )
            valid_data = base[valid_mask]
        else:
            valid_mask = (
                (base['imar_kaks'].notna() & base['imar_taks'].notna()) |
                (base['hmax'].isin([6.8, 9.8, 12.8])) |
                ((base['bina_alan_m2_old'] > 90) & 
                (base['imar_kaks'].isna() | base['imar_taks'].isna()) & 
                (base['hmax'].isna()))
            )
            valid_data = base[valid_mask]

        # Sonuç hesapla
        result = {
            'Toplam Sample': len(valid_data),
            'Ortalama TAKS': valid_data['TAKS'].mean(),
            'Ortalama Bina Alanı': valid_data['bina_alan_m2_old'].mean(),
            'Min Tüketim (kWh/m²)': valid_data['tuketim_kwh_m2'].min(),
            'Max Tüketim (kWh/m²)': valid_data['tuketim_kwh_m2'].max(),
            'Ort Tüketim (kWh/m²)': valid_data['tuketim_kwh_m2'].mean(),
            'Min Tüketim/Abone (kWh)': valid_data['tuketim_kwh_abone'].min(),
            'Max Tüketim/Abone (kWh)': valid_data['tuketim_kwh_abone'].max(),
            'Ort Tüketim/Abone (kWh)': valid_data['tuketim_kwh_abone'].mean(),
            'Toplam Abone Sayısı': valid_data['ABONE_SAYISI'].sum(),
            'Toplam Tüketim (kWh)': valid_data['ORT_MESKEN_TUKETIM'].sum()
        }

        return pd.DataFrame([result]), valid_data
    
    # Her İmar ID için hesapla
    result_df_5, valid_data_5 = process_imar_id(gdf, '5', m2_limit=350)
    result_df_1, valid_data_1 = process_imar_id(gdf, '1', m2_limit=350)
    result_df_2, valid_data_2 = process_imar_id(gdf, '2', m2_limit=450)
    result_df_3, valid_data_3 = process_imar_id(gdf, '3')
    result_df_4, valid_data_4 = process_imar_id(gdf, '4')
    
    # Tüm sonuçları tek bir DataFrame'de birleştir
    combined_results = pd.concat([
        result_df_5.assign(IMAR_ID='5'),
        result_df_1.assign(IMAR_ID='1'),
        result_df_2.assign(IMAR_ID='2'),
        result_df_3.assign(IMAR_ID='3'),
        result_df_4.assign(IMAR_ID='4')
    ], ignore_index=True)
    
    # IMAR_ID sütununu en başa al
    cols = combined_results.columns.tolist()
    cols = ['IMAR_ID'] + [col for col in cols if col != 'IMAR_ID']
    combined_results = combined_results[cols]
    
    # Excel'e kaydet
    excel_filename = f'imar_id_sonuclari_{selected_region}.xlsx'
    
    with pd.ExcelWriter(excel_filename, engine='openpyxl') as writer:
        # Özet tablo
        combined_results.to_excel(writer, sheet_name='Özet', index=False)
        
        # Detaylı veriler
        valid_data_5.to_excel(writer, sheet_name='İmar ID 5 Detay', index=False)
        valid_data_1.to_excel(writer, sheet_name='İmar ID 1 Detay', index=False)
        valid_data_2.to_excel(writer, sheet_name='İmar ID 2 Detay', index=False)
        valid_data_3.to_excel(writer, sheet_name='İmar ID 3 Detay', index=False)
        valid_data_4.to_excel(writer, sheet_name='İmar ID 4 Detay', index=False)
    
    # Sonuçları yazdır
    for imar_id in ['5', '1', '2', '3', '4']:
        result_row = combined_results[combined_results['IMAR_ID'] == imar_id].iloc[0]
        print(f"\nİMAR ID {imar_id} SONUÇLARI:")
        print(f"Toplam Sample: {result_row['Toplam Sample']}")
        print(f"Ortalama TAKS: {result_row['Ortalama TAKS']:.3f}")
        print(f"Ortalama Bina Alanı: {result_row['Ortalama Bina Alanı']:.2f} m²")
        print(f"Tüketim (kWh/m²): Min={result_row['Min Tüketim (kWh/m²)']:.2f}, "
              f"Max={result_row['Max Tüketim (kWh/m²)']:.2f}, "
              f"Ort={result_row['Ort Tüketim (kWh/m²)']:.2f}")
        print(f"Tüketim/Abone (kWh): Min={result_row['Min Tüketim/Abone (kWh)']:.2f}, "
              f"Max={result_row['Max Tüketim/Abone (kWh)']:.2f}, "
              f"Ort={result_row['Ort Tüketim/Abone (kWh)']:.2f}")
        print(f"Toplam Abone Sayısı: {result_row['Toplam Abone Sayısı']:.0f}")
        print(f"Toplam Tüketim: {result_row['Toplam Tüketim (kWh)']:.2f} kWh")
    
    return (result_df_5, valid_data_5), (result_df_1, valid_data_1), (result_df_2, valid_data_2), \
           (result_df_3, valid_data_3), (result_df_4, valid_data_4)

def add_imar_ids_from_points_mesken(unique_gdf, selected_region):
    """
    Veritabanından IMAR_ID, ABONE_SAYISI ve ORT_Mesken_tuketim verilerini alıp GeoDataFrame'e ekler.
    """
    try:
        print("\n--- DEBUG: Fonksiyon Başladı ---")
        print(f"--- DEBUG: Seçilen Bölge: {selected_region} ---")

        # Bölge ismine göre veritabanı bağlantısı oluştur
        if "İzmir" in selected_region or "İZMİR" in selected_region:
            print("İzmir bağlantısı seçildi")
            engine = create_engine('postgresql://postgres:12345@localhost:5432/gdz')
        elif "Eskişehir" in selected_region or "ESKİŞEHİR" in selected_region:
            print("Eskişehir bağlantısı seçildi")
            engine = create_engine('postgresql://postgres:12345@localhost:5432/oedas')
        else:
            raise ValueError(f"Geçersiz şehir adı: {selected_region}. İzmir veya Eskişehir seçin.")

        # SQL sorgusu: Veritabanından verileri al
        query = """
            SELECT 
                "X_KOORDINAT" AS X, 
                "Y_KOORDINAT" AS Y, 
                "IMAR_ID_FORECAST", 
                "ABONE SAYISI",
                "ORT_Mesken_tuketim"
            FROM deep_learning_mesken
        """
        print(f"--- DEBUG: SQL Sorgusu Çalıştırılıyor ---\n{query}")
        points_df = pd.read_sql(query, engine)
        
        # Sütun isimlerini normalize et
        points_df.columns = [col.upper().strip() for col in points_df.columns]
        print(f"--- DEBUG: Sütun İsimleri Normalize Edildi: {points_df.columns.tolist()} ---")
        
        print(f"--- DEBUG: SQL Sorgusu Tamamlandı, {len(points_df)} satır veri çekildi ---")
        print(f"--- DEBUG: İlk 5 Satır: ---\n{points_df.head()}")

        # Geometri oluştur ve GeoDataFrame'e dönüştür
        print("--- DEBUG: Geometri oluşturuluyor ---")
        geometry = [Point(xy) for xy in zip(points_df['X'], points_df['Y'])]
        points_gdf = gpd.GeoDataFrame(points_df, geometry=geometry, crs=unique_gdf.crs)
        print(f"--- DEBUG: Points GeoDataFrame oluşturuldu, toplam {len(points_gdf)} satır ---")

        # CRS kontrolü
        if points_gdf.crs != unique_gdf.crs:
            print("--- DEBUG: CRS farkı algılandı. CRS güncelleniyor... ---")
            points_gdf = points_gdf.to_crs(unique_gdf.crs)
        print(f"--- DEBUG: CRS Kontrolü Tamamlandı: {points_gdf.crs} ---")

        # Spatial Join işlemi (buffer ile hassasiyet artırma)
        print("--- DEBUG: Spatial Join işlemi başlıyor ---")
        # unique_gdf['geometry'] = unique_gdf.geometry.buffer(0.00001)
        joined = gpd.sjoin(unique_gdf, points_gdf, how='left', predicate='intersects')
        print(f"--- DEBUG: Spatial Join Tamamlandı, {len(joined)} satır eşleşti ---")
        print(f"--- DEBUG: Joined GeoDataFrame İlk 5 Satır: ---\n{joined.head()}")

        # IMAR_ID işlemi
        def combine_imar_ids(x):
            values = x[x.notna()].astype(int).astype(str).unique()
            return ','.join(values) if len(values) > 0 else '0'

        # ABONE_SAYISI işlemi
        def sum_abone_sayisi(x):
            return x[x.notna()].sum() if len(x) > 0 else 0

        # ORT_Mesken_tuketim işlemi
        def mean_mesken_tuketim(x):
            return x[x.notna()].mean() if len(x) > 0 else 0

        # Grup bazında işlemler
        print("--- DEBUG: IMAR_ID, ABONE_SAYISI ve ORT_MESKEN_TUKETIM hesaplamaları başlıyor ---")
        result_imar = joined.groupby(joined.index)['IMAR_ID_FORECAST'].apply(combine_imar_ids).reset_index()
        result_abone = joined.groupby(joined.index)['ABONE SAYISI'].apply(sum_abone_sayisi).reset_index()
        result_mesken_tuketim = joined.groupby(joined.index)['ORT_MESKEN_TUKETIM'].apply(mean_mesken_tuketim).reset_index()
        print(f"--- DEBUG: IMAR_ID FORECAST Sonuçları ---\n{result_imar.head()}")
        print(f"--- DEBUG: ABONE_SAYISI Sonuçları ---\n{result_abone.head()}")
        print(f"--- DEBUG: ORT_MESKEN_TUKETIM Sonuçları ---\n{result_mesken_tuketim.head()}")

        # Sonuçları orijinal GeoDataFrame'e ekle
        unique_gdf['IMAR_ID'] = result_imar['IMAR_ID_FORECAST']
        unique_gdf['ABONE_SAYISI'] = result_abone['ABONE SAYISI']
        unique_gdf['ORT_MESKEN_TUKETIM'] = result_mesken_tuketim['ORT_MESKEN_TUKETIM']
        print("--- DEBUG: IMAR_ID, ABONE_SAYISI ve ORT_MESKEN_TUKETIM orijinal GeoDataFrame'e eklendi ---")

        # Buffer'ı kaldır
        unique_gdf['geometry'] = unique_gdf.geometry.buffer(0)
        print("--- DEBUG: Buffer kaldırıldı ---")

        # Kontrol çıktıları
        print("\n--- DEBUG: Nihai Sonuçlar ---")
        print("IMAR_ID Sütunu:")
        print(unique_gdf['IMAR_ID'].value_counts().head())
        print("ABONE_SAYISI İstatistikleri:")
        print(unique_gdf['ABONE_SAYISI'].describe())
        print("ORT_MESKEN_TUKETIM İstatistikleri:")
        print(unique_gdf['ORT_MESKEN_TUKETIM'].describe())

        return unique_gdf

    except Exception as e:
        print(f"\nHata: {str(e)}")
        import traceback
        traceback.print_exc()
        return None
def calculate_cell_coverage_union(buildings_gdf, cells_gdf):
    """
    Her hücredeki bina poligonlarını birleştirip (union), 
    hücreyle tek seferde kesişim alanı hesaplar.
    
    Dönüş: DataFrame [hucre_id, bina_sayisi, toplam_kesisim_alani_m2, doluluk_orani, ...]
    """

    # Hücre ID'lerini tutan kolonun adı "id" varsayıyoruz
    # Eğer shapefile'de hücre kimliği farklı bir kolon ise onu kullanın
    cell_id_col = 'id'

    # Hücre alanlarını hesaplayalım (CRS mutlaka metre cinsinden olmalı)
    cells_gdf['hucre_alani_m2'] = cells_gdf.geometry.area

    # 1) Spatial join: hangi bina hangi hücreyle kesişiyor?
    #   op='intersects' ya da 'within' seçilebilir. Burada intersects yeterli.
    joined = gpd.sjoin(buildings_gdf, cells_gdf, how='inner', op='intersects')

    # joined içerisinde:
    #  - index_left (binanın indeksi)
    #  - index_right (hücrenin indeksi)
    #  - bina geometrisi
    #  - hücre geometrisi (cells_gdf'deki kolonlar)
    #  - vs. bulunur

    # 2) Her hücre için bina geometrilerini toplayalım
    #    groupby ile hücreye ait bina geometlerini unary_union yapıyoruz
    union_per_cell = joined.groupby(joined['index_right'])['geometry'].apply(unary_union)
    # Bu bize "her hücrenin bina union geometrisi"ni verir.

    # 3) union_per_cell'i hücre DataFrame'ine geri merge edelim
    #    index_right = hücre indeksi demek
    union_gdf = gpd.GeoDataFrame(union_per_cell, geometry='geometry', crs=cells_gdf.crs)
    union_gdf.rename(columns={'geometry': 'union_bina_geom'}, inplace=True)

    # 4) Hücre GDF ile birleştirelim
    #    Hücrelerin orijinal geometry'si (cell geometry) de var
    union_gdf = cells_gdf.merge(union_gdf, left_index=True, right_on='index_right', how='left')

    # 5) Her hücre için kesişim alanı
    #    union_bina_geom olabilir, NaN ya da boş geometri olabilir
    coverage_areas = []
    for idx, row in union_gdf.iterrows():
        cell_geom = row['geometry']
        union_bina_geom = row['union_bina_geom']
        if union_bina_geom is None or union_bina_geom.is_empty:
            coverage_areas.append(0.0)
        else:
            inter_geom = cell_geom.intersection(union_bina_geom)
            coverage_areas.append(inter_geom.area)

    union_gdf['toplam_kesisim_alani_m2'] = coverage_areas

    # 6) Hücre bazında bina sayısı
    #    Tek tek bina sayısı için yine joined tablosundan groupby yapabilirsiniz
    bina_sayisi = joined.groupby('index_right')['index_left'].nunique()

    # Bunu da union_gdf'e ekleyelim
    union_gdf['bina_sayisi'] = union_gdf.index.map(bina_sayisi).fillna(0).astype(int)

    # 7) Doluluk oranı
    union_gdf['doluluk_orani'] = (union_gdf['toplam_kesisim_alani_m2'] / union_gdf['hucre_alani_m2'] * 100).round(2)

    # 8) Sonuç DataFrame
    #    Hücre ID, bina_sayisi, toplam_kesisim_alani_m2, hucre_alani_m2, doluluk_orani
    result_cols = [
        cell_id_col, 'bina_sayisi', 'toplam_kesisim_alani_m2',
        'hucre_alani_m2', 'doluluk_orani'
    ]
    result_df = union_gdf[result_cols].copy()

    return result_df
def add_imar_ids_from_points_ticarethane(unique_gdf, selected_region):
    """
    Veritabanından TICARETHANE ve SANAYI verilerini alıp GeoDataFrame'e ekler.
    """
    try:
        print("\n--- DEBUG: Fonksiyon Başladı ---")
        print(f"--- DEBUG: Seçilen Bölge: {selected_region} ---")

        # Bölge ismine göre veritabanı bağlantısı oluştur
        if "İzmir" in selected_region or "İZMİR" in selected_region:
            print("İzmir bağlantısı seçildi")
            engine = create_engine('postgresql://postgres:12345@localhost:5432/gdz')
        elif "Eskişehir" in selected_region or "ESKİŞEHİR" in selected_region:
            print("Eskişehir bağlantısı seçildi")
            engine = create_engine('postgresql://postgres:12345@localhost:5432/oedas')
        else:
            raise ValueError(f"Geçersiz şehir adı: {selected_region}. İzmir veya Eskişehir seçin.")

        # SQL sorgusu - SANAYI_count eklendi
        query = """
            SELECT 
                "X_KOORDINAT" AS X, 
                "Y_KOORDINAT" AS Y, 
                "BINA_TIPI", 
                "TICARETHANE_tuketim",
                "SANAYI_tuketim",
                "TICARETHANE_count",
                "SANAYI_count"
            FROM deep_learning_other
        """
        print("--- DEBUG: SQL Sorgusu Çalıştırılıyor ---")
        points_df = pd.read_sql(query, engine)
        
        # Sütun isimlerini normalize et
        points_df.columns = [col.upper().strip() for col in points_df.columns]
        print(f"--- DEBUG: Sütun İsimleri: {points_df.columns.tolist()}")

        # Geometri oluştur ve GeoDataFrame'e dönüştür
        print("--- DEBUG: Geometri oluşturuluyor ---")
        geometry = [Point(xy) for xy in zip(points_df['X'], points_df['Y'])]
        points_gdf = gpd.GeoDataFrame(points_df, geometry=geometry, crs=unique_gdf.crs)
        print(f"--- DEBUG: Points GeoDataFrame oluşturuldu, toplam {len(points_gdf)} satır ---")

        # CRS kontrolü
        if points_gdf.crs != unique_gdf.crs:
            print("--- DEBUG: CRS farkı algılandı. CRS güncelleniyor... ---")
            points_gdf = points_gdf.to_crs(unique_gdf.crs)
        print(f"--- DEBUG: CRS: {points_gdf.crs} ---")

        # Spatial Join işlemi
        print("--- DEBUG: Spatial Join işlemi başlıyor ---")
        joined = gpd.sjoin(unique_gdf, points_gdf, how='left', predicate='intersects')
        print(f"--- DEBUG: Spatial Join Tamamlandı, {len(joined)} satır eşleşti ---")

        # Bina tipi işlemi
        def combine_bina_tipleri(x):
            values = x[x.notna()].astype(str).unique()
            return ','.join(values) if len(values) > 0 else ''

        # Tüketim ve count işlemi
        def sum_tuketim(x):
            return x[x.notna()].sum() if len(x) > 0 else 0

        # Grup bazında işlemler
        print("--- DEBUG: BINA_TIPI, TICARETHANE_TUKETIM, SANAYI_TUKETIM, TICARETHANE_COUNT ve SANAYI_COUNT hesaplamaları başlıyor ---")
        result_bina = joined.groupby(joined.index)['BINA_TIPI'].apply(combine_bina_tipleri).reset_index()
        result_ticarethane = joined.groupby(joined.index)['TICARETHANE_TUKETIM'].apply(sum_tuketim).reset_index()
        result_sanayi = joined.groupby(joined.index)['SANAYI_TUKETIM'].apply(sum_tuketim).reset_index()
        result_ticarethane_count = joined.groupby(joined.index)['TICARETHANE_COUNT'].apply(sum_tuketim).reset_index()
        result_sanayi_count = joined.groupby(joined.index)['SANAYI_COUNT'].apply(sum_tuketim).reset_index()

        # Sonuçları orijinal GeoDataFrame'e ekle
        unique_gdf['BINA_TIPI'] = result_bina['BINA_TIPI']
        unique_gdf['TICARETHANE_TUKETIM'] = result_ticarethane['TICARETHANE_TUKETIM']
        unique_gdf['SANAYI_TUKETIM'] = result_sanayi['SANAYI_TUKETIM']
        unique_gdf['TICARETHANE_COUNT'] = result_ticarethane_count['TICARETHANE_COUNT']
        unique_gdf['SANAYI_COUNT'] = result_sanayi_count['SANAYI_COUNT']
        print("--- DEBUG: BINA_TIPI, TICARETHANE_TUKETIM, SANAYI_TUKETIM, TICARETHANE_COUNT ve SANAYI_COUNT orijinal GeoDataFrame'e eklendi ---")

        # Buffer'ı kaldır
        unique_gdf['geometry'] = unique_gdf.geometry.buffer(0)
        print("--- DEBUG: Buffer kaldırıldı ---")

        # Kontrol çıktıları
        print("\n--- DEBUG: Nihai Sonuçlar ---")
        print("BINA_TIPI değerleri:")
        print(unique_gdf['BINA_TIPI'].value_counts().head())
        print("\nTICARETHANE_TUKETIM istatistikleri:")
        print(unique_gdf['TICARETHANE_TUKETIM'].describe())
        print("\nSANAYI_TUKETIM istatistikleri:")
        print(unique_gdf['SANAYI_TUKETIM'].describe())
        print("\nTICARETHANE_COUNT istatistikleri:")
        print(unique_gdf['TICARETHANE_COUNT'].describe())
        print("\nSANAYI_COUNT istatistikleri:")
        print(unique_gdf['SANAYI_COUNT'].describe())

        return unique_gdf

    except Exception as e:
        print(f"\nHata: {str(e)}")
        import traceback
        traceback.print_exc()
        return None

def create_single_shp_with_buffer_preserving_data(unique_gdf, point_radius=5):
    output_dir = 'output-shp-files'
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
    
    # Tek bir SHP dosyasına kaydet
    output_path = os.path.join(output_dir, 'combined_output_eskisehir.shp')
    try:
        combined_gdf.to_file(output_path, driver='ESRI Shapefile', encoding='utf-8')
        print(f"Tüm geometriler tek bir SHP dosyasına kaydedildi: {output_path}")
    except Exception as e:
        print(f"SHP kaydetme hatası: {str(e)}")

 


def handle_selection():
    global global_data
    global selected_region
    selected_region = region_combo.get()  # Seçilen bölgeyi alıyoruz
    
    print(f"Seçilen bölge: {selected_region}")  # Test amaçlı ekleyin
    
    if selected_region in ["Eskişehir", "İzmir"]:  # Eğer Eskişehir veya İzmir seçilmişse
        root.destroy()  # GUI'yi kapat
        print(f"Bölge seçildi: {selected_region}")
        global_data = fetch_and_process_data(selected_region)
    
    elif selected_region == "Dosya Seç":  # Eğer Dosya Seç seçeneği seçilmişse
     root.destroy()  # GUI'yi kapat
     print("Dosya seçiliyor...")
     file_path = filedialog.askopenfilename(title="Bir CSV dosyası seçin", filetypes=[("CSV files", "*.csv")])

    if file_path:  # Eğer dosya seçilmişse
        # Dosya yolunda İzmir veya Eskişehir kontrolü yap
        if "İzmir" in file_path or "izmir" in file_path:
            selected_region = "İzmir"
        elif "Eskişehir" in file_path or "eskisehir" in file_path:
            selected_region = "Eskişehir"
        else:
            # Eğer dosya İzmir veya Eskişehir ile eşleşmiyorsa
            selected_region = "Bilinmeyen Bölge"

        print(f"Dosya seçildi: {file_path}, Bölge: {selected_region}")
        
        # Dosya içeriğini yükleme
        df = pd.read_csv(file_path)
        df['geometry'] = df['geometry'].apply(lambda x: wkt.loads(x) if isinstance(x, str) else np.nan)
        dfd = gpd.GeoDataFrame(df, geometry='geometry')
        global_data = dfd

    else:
        print("Dosya seçimi iptal edildi. Bölge belirlenemedi.")
        selected_region = None  # Dosya seçilmezse bölge None olarak ayarlanır
# 
def clean_geodataframe(dfd):
    # Eğer CSV'de lat/lon sütunları varsa geometrileri oluştur
    if 'lat' in dfd.columns and 'lon' in dfd.columns:
        # 'lat' ve 'lon' sütunlarından Point geometrileri oluşturun
        dfd['geometry'] = [Point(xy) for xy in zip(dfd['lon'], dfd['lat'])]
        
        # GeoDataFrame oluşturun ve 'geometry' sütununu aktif hale getirin
        gdf_cleaned = gpd.GeoDataFrame(dfd, geometry='geometry')
        
        return gdf_cleaned
    else:
        print("CSV dosyasında 'lat' ve 'lon' sütunları bulunamadı.")
        return None
#

root = tk.Tk()
root.title("Bölge Seçimi")
root.geometry("300x150")

# Kullanıcıya seçenekleri sunmak için bir label ekleyelim
label = ttk.Label(root, text="Lütfen çalışmak istediğiniz bölgeyi seçin:")
label.pack(pady=10)

# 3 seçenekli combo box oluştur
region_combo = ttk.Combobox(root, values=["Eskişehir","İzmir","Dosya Seç"])
region_combo.pack(pady=10)

# Seçimi onaylamak için bir buton ekle
button = ttk.Button(root, text="Seçimi Onayla", command=handle_selection)
button.pack(pady=10)

# Pencereyi başlat
root.mainloop()

def open_file_dialog():
   file_path = filedialog.askopenfilename(title="KML Dosyası Seçin", filetypes=[("KML Files", "*.kml")])
    
   if file_path:
        # Global değişkene dosya yolunu kaydet
        global kml_file
        global selected_file_path
        
        kml_file = process_to_geodataframe(file_path,global_data,selected_region)
            
        root.destroy()
        return katmanAyirma(kml_file) 
        
   else:
        print("Dosya seçilmedi.")
   
# Ana pencereyi oluştur
root = tk.Tk()
root.title("Dosya Seçimi")
root.geometry("400x200")

# Kullanıcıya bilgi vermek için bir label ekleyelim
label = tk.Label(root, text="KML Dosyası Yüklemek İçin Butona Tıklayın")
label.pack(pady=20)

# Dosya seçimi için bir buton ekleyelim
button = tk.Button(root, text="KML Dosyası Seç", command=open_file_dialog)
button.pack(pady=10)

# Pencereyi başlat
root.mainloop()