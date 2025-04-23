import os
import pandas as pd
import geopandas as gpd
from sqlalchemy import create_engine
import re
from shapely.geometry import Point, Polygon
from shapely import wkt
from shapely.wkt import loads
from sqlalchemy.sql import text
from shapely import wkb
import os
# PostgreSQL bağlantısı
DB_CONNECTIONS = {
    "gdz": "postgresql://postgres:12345@localhost:5432/gdz",
    "oedas": "postgresql://postgres:12345@localhost:5432/oedas"
}


# matching_table tanımlama
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
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Depolama", "Anahtar Kelimeler": r"KATI_ATIK|DEPOLAMA", "Katman Adi": "PL_KATI_ATIK_DEPONI"},
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
    {"İmar Tipi (Land-Use)": "Yasakli Alan", "Katman Adlandirma": "Agaclik Alan", "Anahtar Kelimeler": r"OA|O_A","Katman Adi": "##PL_OA"},
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


engine = create_engine('postgresql://postgres:12345@localhost:5432/gdz')

def get_database_connection(city):
    db_choice = "gdz" if city == "İzmir" else "oedas"
    return create_engine(DB_CONNECTIONS[db_choice])

def load_imar_plan(city, engine):
    try:
        print("İmar planı verileri yükleniyor...")
        table_name = "IZMIR_IMAR_PL" if city == "İzmir" else "ESKISEHIR_IMAR_PL"
        query = f"""
        SELECT name, "styleUrl", ST_AsText(geometry) AS geometry
        FROM public."{table_name}"
        """
        print(f"SQL Sorgusu: {query}")
        imar_df = pd.read_sql(query, engine)
        print(f"İmar verisi başarıyla yüklendi. İlk 5 satır: \n{imar_df.head()}")

        imar_df['geometry'] = imar_df['geometry'].apply(loads)
        imar_df = gpd.GeoDataFrame(imar_df, geometry='geometry', crs="EPSG:4326")
        
        imar_df = label_IZMIR_IMAR_PL(imar_df, matching_table)
        
        return imar_df
    except Exception as e:
        print(f"İmar verisi yüklenirken hata oluştu: {e}")
        raise

def label_IZMIR_IMAR_PL(imar_df, matching_table):
    try:
        print("İmar planına etiketler ekleniyor...")

        if 'styleUrl' not in imar_df.columns:
            raise ValueError("'styleUrl' sütunu imar planı verisinde bulunamadı.")
        
        # Etiketleme için eşleştirme fonksiyonu
        def match_styleurl(styleurl):
            for match in matching_table:
                if re.search(match["Anahtar Kelimeler"], str(styleurl), re.IGNORECASE):
                    return match["İmar Tipi (Land-Use)"]
            return "Diğer"
        
        # Yeni sütun ekleniyor
        imar_df['İmar Tipi (Land-Use)'] = imar_df['styleUrl'].apply(match_styleurl)

        # Etiketleme kontrolü
        if 'İmar Tipi (Land-Use)' not in imar_df.columns:
            raise ValueError("'İmar Tipi (Land-Use)' sütunu oluşturulamadı.")
        
        print(f"Etiketleme sonrası unique değerler:\n{imar_df['İmar Tipi (Land-Use)'].unique()}")
        return imar_df
    except Exception as e:
        print(f"Etiketleme sırasında hata oluştu: {e}")
        raise

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
            # Eskişehir için Excel okuma
            current_dir = os.path.dirname(os.path.abspath(__file__))
            hucre_file = os.path.join(current_dir, "..", "girdiler", "eskisehir_hucreleri.xlsx")
            hucre_df = pd.read_excel(hucre_file)
            
        # Geometri oluşturma
        hucre_df['geometry'] = hucre_df.apply(lambda row: Polygon([
            (float(row['left']), float(row['top'])),
            (float(row['right']), float(row['top'])),
            (float(row['right']), float(row['bottom'])),
            (float(row['left']), float(row['bottom'])),
            (float(row['left']), float(row['top']))
        ]), axis=1)
        
        # GeoDataFrame oluştur
        hucre_df = gpd.GeoDataFrame(hucre_df, geometry='geometry', crs="EPSG:4326")
        
        # Koordinat sistemini kontrol et ve yazdır
        print(f"Hücre verisi CRS: {hucre_df.crs}")
        bounds = hucre_df.total_bounds
        print(f"Hücre verisi sınırları:")
        print(f"Boylam: {bounds[0]} - {bounds[2]}")
        print(f"Enlem: {bounds[1]} - {bounds[3]}")
        
        return hucre_df
    
    except Exception as e:
        print(f"Hücre verisi yüklenirken hata oluştu: {e}")
        raise



def calculate_metrics(intersections):
    try:
        print("Metrikler hesaplanıyor...")
        
        print("Kesişim DataFrame'inin sütunları:")
        print(intersections.columns)

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

        print("\nHesaplanan metrikler:")
        print(f"Toplam hücre sayısı: {len(final_metrics)}")
        print("\nÖrnek metrikler (ilk 5 satır):")
        print(final_metrics.head())
        
        return final_metrics

    except Exception as e:
        print(f"Metrik hesaplama sırasında hata oluştu: {e}")
        raise

def save_to_database(dataframe, table_name, engine):
    try:
        print(f"Veri {table_name} tablosuna kaydediliyor...")
        print(f"Veri boyutu: {dataframe.shape}")
        
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
        
        print(f"Veri başarıyla {safe_table_name} tablosuna kaydedildi.")
        return True
        
    except Exception as e:
        print(f"Veritabanına kayıt sırasında hata oluştu: {e}")
        print("Hata detayları:")
        print(f"Tablo adı: {table_name}")
        print(f"Veri tipleri:\n{dataframe.dtypes}")
        raise

def get_input_city():
    while True:
        city = input("Şehir seçiniz (İzmir/Eskişehir): ").strip()
        if city in ["İzmir", "Eskişehir"]:
            return city
        print("Geçersiz şehir. Lütfen 'İzmir' veya 'Eskişehir' giriniz.")

def load_imar_plan(city, engine):
    try:
        print("İmar planı verileri yükleniyor...")
        if city == "İzmir":
            table_name = "IZMIR_IMAR_PL"
            query = f"""
            SELECT name, "styleUrl", ST_AsText(geometry) AS geometry
            FROM public."{table_name}"
            """
            print(f"SQL Sorgusu: {query}")
            imar_df = pd.read_sql(query, engine)
            imar_df['geometry'] = imar_df['geometry'].apply(loads)
            # GeoDataFrame oluştur
            imar_df = gpd.GeoDataFrame(imar_df, geometry='geometry', crs="EPSG:4326")
            
        else:
            # Eskişehir için CSV okuma
            imar_file = "../girdiler/eskisehir_imar_pl.csv"
            print(f"İmar verisi okunuyor: {imar_file}")
            imar_df = pd.read_csv(imar_file)
            
            def convert_geometry(geom_str):
                try:
                    # WKT formatından geometri oluştur
                    geom = wkt.loads(geom_str)
                    return geom
                except Exception as e:
                    print(f"Geometri dönüşümünde hata: {geom_str[:50]}...")
                    print(f"Hata detayı: {str(e)}")
                    return None

            # Geometri dönüşümü
            print("Geometri dönüşümü yapılıyor...")
            imar_df['geometry'] = imar_df['geometry'].apply(convert_geometry)
            
            # Hatalı geometrileri temizle
            imar_df = imar_df.dropna(subset=['geometry'])
            
            # GeoDataFrame oluştur
            imar_df = gpd.GeoDataFrame(imar_df, geometry='geometry', crs="EPSG:4326")

        # Etiketleme işlemi
        imar_df = label_IZMIR_IMAR_PL(imar_df, matching_table)
        
        # Kontrol yazdırmaları
        print(f"\nİmar verisi CRS: {imar_df.crs}")
        bounds = imar_df.total_bounds
        print(f"İmar verisi sınırları:")
        print(f"Boylam: {bounds[0]} - {bounds[2]}")
        print(f"Enlem: {bounds[1]} - {bounds[3]}")
        print(f"Toplam geometri sayısı: {len(imar_df)}")
        print(f"\nİmar verisi başarıyla yüklendi. İlk 5 satır: \n{imar_df.head()}")
        
        return imar_df
    except Exception as e:
        print(f"İmar verisi yüklenirken hata oluştu: {e}")
        raise

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
            # Eskişehir için Excel okuma
            current_dir = os.path.dirname(os.path.abspath(__file__))
            hucre_file = os.path.join(current_dir,  "..", "girdiler", "eskisehir_hucreleri.xlsx")
            hucre_df = pd.read_excel(hucre_file)
            
        # Geometri oluşturma
        hucre_df['geometry'] = hucre_df.apply(lambda row: Polygon([
            (float(row['left']), float(row['top'])),
            (float(row['right']), float(row['top'])),
            (float(row['right']), float(row['bottom'])),
            (float(row['left']), float(row['bottom'])),
            (float(row['left']), float(row['top']))
        ]), axis=1)
        
        # GeoDataFrame oluştur
        hucre_df = gpd.GeoDataFrame(hucre_df, geometry='geometry', crs="EPSG:4326")
        
        # Koordinat sistemini kontrol et ve yazdır
        print(f"Hücre verisi CRS: {hucre_df.crs}")
        bounds = hucre_df.total_bounds
        print(f"Hücre verisi sınırları:")
        print(f"Boylam: {bounds[0]} - {bounds[2]}")
        print(f"Enlem: {bounds[1]} - {bounds[3]}")
        
        return hucre_df
    
    except Exception as e:
        print(f"Hücre verisi yüklenirken hata oluştu: {e}")
        raise

def calculate_intersection(imar_df, hucre_df):
    try:
        print("Projeksiyon dönüşümü ve kesişim hesaplama başlıyor...")
        
        # İmar ve hücre verilerinin GeoDataFrame olduğundan emin olun
        if not isinstance(imar_df, gpd.GeoDataFrame):
            imar_df = gpd.GeoDataFrame(imar_df, geometry='geometry', crs="EPSG:4326")
        if not isinstance(hucre_df, gpd.GeoDataFrame):
            hucre_df = gpd.GeoDataFrame(hucre_df, geometry='geometry', crs="EPSG:4326")
        
        # Koordinat sistemlerini kontrol et
        print(f"İmar verisi CRS: {imar_df.crs}")
        print(f"Hücre verisi CRS: {hucre_df.crs}")
        
        # Her iki veriyi de UTM Zone 36N'ye dönüştür
        imar_df_proj = imar_df.to_crs("EPSG:32636")
        hucre_df_proj = hucre_df.to_crs("EPSG:32636")

        # Hücre alanlarını hesapla
        hucre_df_proj['hucre_area'] = hucre_df_proj['geometry'].area
        print("\nHücre alanları başarıyla hesaplandı. İlk 5 satır:")
        print(hucre_df_proj[['id', 'hucre_area']].head())

        print("\nKesişim hesaplanıyor...")
        # Kesişim hesapla
        intersections = gpd.overlay(hucre_df_proj, imar_df_proj, how='intersection', keep_geom_type=False)
        print("Kesişim hesaplandı. İlk 5 satır:")
        print(intersections.head())

        # Hücre alanlarını ekle
        intersections = intersections.merge(
            hucre_df_proj[['id', 'hucre_area']], 
            on='id', 
            how='left'
        )

        # Kesişim alanlarını hesapla
        intersections['intersection_area'] = intersections['geometry'].area
        print("\nKesişim alanları hesaplandı. İlk 5 satır:")
        print(intersections[['id', 'intersection_area']].head())
        
        # Kontrol yazdırmaları
        print(f"\nToplam kesişim sayısı: {len(intersections)}")
        print(f"Benzersiz hücre sayısı: {intersections['id'].nunique()}")
        if 'İmar Tipi (Land-Use)' in intersections.columns:
            print(f"Benzersiz imar tipi sayısı: {intersections['İmar Tipi (Land-Use)'].nunique()}")
        else:
            print("İmar Tipi (Land-Use) sütunu bulunamadı.")
        
        return intersections, hucre_df_proj
    except Exception as e:
        print(f"Kesişim hesaplanırken hata oluştu: {e}")
        raise
    except Exception as e:
        print(f"Kesişim hesaplanırken hata oluştu: {e}")
        raise

def process_city_data(city, pl_polygons=None):
    """
    Şehir verilerini işler ve imar analizini gerçekleştirir.
    """
    try:
        print(f"{city} için işlem başlıyor...")

        engine = get_database_connection(city) if city == "İzmir" else None

        # İmar ve hücre verilerini yükle
        imar_df = pl_polygons if pl_polygons is not None else load_imar_plan(city, engine)
        hucre_df = load_hucre(city, engine)

        # İmar verilerine etiketleme işlemi uygula
        if 'İmar Tipi (Land-Use)' not in imar_df.columns:
            imar_df = label_IZMIR_IMAR_PL(imar_df, matching_table)

        # Kesişim hesapla
        intersections, hucre_df = calculate_intersection(imar_df, hucre_df)

        # Kesişim sonrası `İmar Tipi (Land-Use)` sütununu kontrol et
        if 'İmar Tipi (Land-Use)' not in intersections.columns:
            raise ValueError("'İmar Tipi (Land-Use)' sütunu kesişim sonrasında eksik.")

        # Metrikleri hesapla
        metrics = calculate_metrics(intersections)

        if city == "İzmir":
            # Veritabanına kaydet
            table_name = "v4_gdz_birlestirilmis_veri_imar_ada_tipi_sayisi"
            save_to_database(metrics, table_name, engine)

            with engine.connect() as connection:
                check_query = f'SELECT * FROM {table_name} LIMIT 5'
                result = pd.read_sql(text(check_query), connection)
                print("\nVeritabanından ilk 5 satır:")
                print(result)
        else:
                current_dir = os.path.dirname(os.path.abspath(__file__))
                print(f"Mevcut konum: {current_dir}")
                
                # Çıktı dizini oluştur
                output_dir = os.path.join(current_dir, "..", "ciktilar")
                output_file = os.path.join(output_dir, "eskisehir_ada_imar_tipi_sayisi.csv")
                
                # Klasörü oluştur
                os.makedirs(output_dir, exist_ok=True)
                
                # CSV'ye kaydet
                metrics.to_csv(output_file)
                print(f"Veriler kaydedildi: {output_file}")

    except Exception as e:
        print(f"İşlem sırasında hata oluştu: {e}")
        raise
if __name__ == "__main__":
    city = get_input_city()
    print(f"\n{city} için işlemler başlatılıyor...")
    process_city_data(city)