# -*- coding: utf-8 -*-
"""
Created on Sun Apr 20 12:53:11 2025

@author: vural.bayrakli
"""


#-----------------------
# Değişkenler. Excel den çekilecek
import pandas as pd
import pickle
import os
import json

odtr = None

p = None

def to_float(value):
    try:
        # Virgülü noktaya çevir ve yüzde işaretini sil
        value = str(value).replace(',', '.').replace('%', '').strip()
        return float(value)
    except (ValueError, TypeError):
        return None  # Hatalıysa None dön


def Start(df):
    
    global p
    #df = pd.read_excel(r"Katsayılar.xlsx", sheet_name="Sheet2", engine="openpyxl")  # Daha hızlı olması için openpyxl kullanılır
    df = df.set_index("Parametre")
    
    komsuluk_katsayisi = to_float(df.loc['komşuluk katsayısı-Merkez süper hücre', 'Değer'])/100
    komsuluk_katsayisi_diger = to_float(df.loc['komşuluk katsayısı-komşu süper hücreler', 'Değer'])/100
    PF = 0.9 # df.loc['PF', 'Değer']
    toplam_saat = 8760.0 # df.loc['PF', 'Değer']
    boskapasite_katsayisi =  to_float(df.loc['boş kapasite katsayısı', 'Değer'])
    toplam_saat_katsayi = to_float(df.loc['toplam_saat_katsayi', 'Değer'])
    rezerv_katsayisi =  to_float(df.loc['Rezerv Hücre katsayısı', 'Değer'])
    park_katsayisi = to_float(df.loc['Park Alanı katsayısı', 'Değer'])
    belediye_katsayisi = to_float(df.loc['Belediye Hizmet Alanları katsayısı', 'Değer'])
    skor_bolu_yuk_katsayi = to_float(df.loc['dağıtım limiti', 'Değer'])
    KVA_cevirme =  to_float(df.loc['KVA_cevirme', 'Değer'])
    yeni_trafo_kapasite_kullanim_ust_limiti =to_float (df.loc['yeni_trafo_kapasite_kullanim_ust_limiti', 'Değer'])
    mevcut_trafo_kapasite_kullanim_ust_limiti = to_float(df.loc['mevcut_trafo_kapasite_kullanim_ust_limiti', 'Değer'])
    Mevcut_trafo_yükseltmede_nominal_kapasite_üst_sınırı_kentsel = to_float(df.loc['Mevcut trafo yükseltmede nominal kapasite üst sınırı-kentsel', 'Değer'])
    Yeni_trafo_tesisde_nominal_kapasite_ust_siniri_kentsel = to_float(df.loc['Yeni_trafo_tesisde_nominal_kapasite_ust_siniri_kentsel', 'Değer'])
    Yeni_trafo_tesisde_nominal_kapasite_alt_siniri_kentsel =to_float(df.loc['Yeni trafo tesisde nominal kapasite alt sınırı-kentsel', 'Değer'])
    Yeni_trafo_tesisde_nominal_kapasite_ust_siniri_kırsal = to_float(df.loc['Yeni_trafo_tesisde_nominal_kapasite_alt_siniri_kırsal', 'Değer'])
    Yeni_trafo_tesisde_nominal_kapasite_alt_siniri_kırsal = to_float(df.loc['Yeni trafo tesisde nominal kapasite alt sınırı-kırsal', 'Değer'])
    skor_sabiti = to_float(df.loc['skor sabiti', 'Değer'])
    degisim_limiti = to_float(df.loc['değişim_limiti', 'Değer'])
    tarımsal_sulama_hat_uzunluk = int(df.loc['tarımsal sulamada dikey hat uzunluğu', 'Değer'])
    hat_uzunluk =  int(df.loc['kırsalda dikey hat uzunluğu', 'Değer'])
    kurum_trafo_kentsel_alan_liste = eval(df.loc['kurum_trafo_kentsel_alan_liste', 'Değer'])
    kurum_trafo_kirsal_alan_liste = eval(df.loc['kurum_trafo_kirsal_alan_liste', 'Değer'])
    kurum_trafo_kentdisi_alan_liste = eval(df.loc['kurum_trafo_kentdisi_alan_liste', 'Değer'])
    ozel_trafo_liste = eval(df.loc['ozel_trafo_liste', 'Değer'])
    
    
    # Dictionary ile değişkenleri döndürme
    result = {
        "degisim_limiti": degisim_limiti,
        "skor_sabiti": skor_sabiti,
        
        "komsuluk_katsayisi": komsuluk_katsayisi,
        "komsuluk_katsayisi_diger": komsuluk_katsayisi_diger,
        "PF": PF,
        "KVA_cevirme": KVA_cevirme,
        "toplam_saat": toplam_saat,
        "boskapasite_katsayisi": boskapasite_katsayisi,
        "toplam_saat_katsayi": toplam_saat_katsayi,
        "rezerv_katsayisi": rezerv_katsayisi,
        "park_katsayisi": park_katsayisi,
        "belediye_katsayisi": belediye_katsayisi,
        "skor_bolu_yuk_katsayi": skor_bolu_yuk_katsayi,
        "yeni_trafo_kapasite_kullanim_ust_limiti": yeni_trafo_kapasite_kullanim_ust_limiti,
        "mevcut_trafo_kapasite_kullanim_ust_limiti": mevcut_trafo_kapasite_kullanim_ust_limiti,
        "Yeni_trafo_tesisde_nominal_kapasite_ust_siniri_kentsel": Yeni_trafo_tesisde_nominal_kapasite_ust_siniri_kentsel,
        "Yeni_trafo_tesisde_nominal_kapasite_alt_siniri_kentsel": Yeni_trafo_tesisde_nominal_kapasite_alt_siniri_kentsel,
        "Yeni_trafo_tesisde_nominal_kapasite_ust_siniri_kırsal": Yeni_trafo_tesisde_nominal_kapasite_ust_siniri_kırsal,
        "Yeni_trafo_tesisde_nominal_kapasite_alt_siniri_kırsal": Yeni_trafo_tesisde_nominal_kapasite_alt_siniri_kırsal,
        "kurum_trafo_kentsel_alan_liste": kurum_trafo_kentsel_alan_liste,
        "kurum_trafo_kirsal_alan_liste": kurum_trafo_kirsal_alan_liste,
        "kurum_trafo_kentdisi_alan_liste": kurum_trafo_kentdisi_alan_liste,
        "ozel_trafo_liste": ozel_trafo_liste,
        "hat_uzunluk": hat_uzunluk,
        "tarımsal_sulama_hat_uzunluk": tarımsal_sulama_hat_uzunluk
    }
    
    p = result.copy()
    
    return p

def DosyaOlustur(path):
    
    if not os.path.exists(path):
        os.makedirs(path)
    
def ODTR(input_file):
    
    global odtr
    
    #input_file = r"C:\Users\vural.bayrakli\source\repos\SLF\bin\Debug\il_ilce_kırılımları\İzmir\Karşıyaka\Yeni Projelenmiş DTR Verileri\TESTjson.json"
    with open(input_file, "r", encoding="utf-8") as file:
        data = json.load(file)
        
    
        
    ilk = True
    
    ilk_yil = data["Degiskenler"]["İlkYıl"]

    son_yil =  data["Degiskenler"]["SonYıl"]
    
    il = data["Degiskenler"]["İl"]
    
    ilce = data["Degiskenler"]["İlçe"]
    
    sonuc_yolu = data["FilePaths"]["SonucYolu"]
    
    ara_dosya_yolu = os.path.join(data["FilePaths"]["SonucYolu"], "ara_dosyalar")
    
    DosyaOlustur(ara_dosya_yolu)
    
    sonuc_yolu_ilce = os.path.join(ara_dosya_yolu, ilce)
    
    # DosyaOlustur(sonuc_yolu_ilce)
    
    python_dosya_yolu = os.path.join(ara_dosya_yolu)
    
    dosyalar = os.path.join(data["FilePaths"]["PythonFilePath"], "dosyalar")
    
    DosyaOlustur(dosyalar)

    excel_kayit_path = os.path.join(python_dosya_yolu, "sonuçlar")
    
    hucre_path = os.path.join(data["FilePaths"]["İlİlceYol"], "hücre")
    
    #db_yol = os.path.join(python_dosya_yolu, "sonuçlar")
    db_yol = data["FilePaths"]["VeriTabanıYolu"]

    db_ad = data["Degiskenler"]["veriTabaniAdi"]
    
    pydeckRunOutput = data["Degiskenler"]["HTML"]
    
    DosyaOlustur(excel_kayit_path)

    yuk_path = os.path.join(data["FilePaths"]["YükVeriYolu"],"5.Yük Tahmini\çıktı", data["Degiskenler"]["TuketimDosyaAdi"])
    
    yuk_db_yolu = data["FilePaths"]["YukVeriTabanıYolu"]
    
    #pointload_path = os.path.join(data["FilePaths"]["YükVeriYolu"], f"5.Yük Tahmini/çıktı/pointload_{ilce}.xlsx")
    pointload_path = data["FilePaths"]["PointLoadYolu"]
    pointload_path = os.path.join(data["FilePaths"]["YükVeriYolu"], f"Point load/çıktı/pointload_{ilce}.xlsx")

    #yuk_db_adi = data["Degiskenler"]["YukTabloAdi"]
    yuk_db_adi = ilce

    yuk_path_parquet = os.path.join(python_dosya_yolu, f"dfKurumYuk_{ilce}.parquet")
    EA_yuk_path_parquet = os.path.join(python_dosya_yolu, f"EAYuk_{ilce}.parquet")
    
    trafo_path = os.path.join(data["FilePaths"]["İmarVeriYolu"], data["Degiskenler"]["TrafoDosyaAdi"])
    trafo_path_parquet = os.path.join(python_dosya_yolu, f"trafo_{ilce}.parquet")
    trafo_db_adi = f"trafo_{ilce}"

    trafo_sheet_name = "trafo_analiz"

    trafo_alanlari_path = os.path.join(data["FilePaths"]["İmarVeriYolu"], data["Degiskenler"]["TrafoAlanDosyaAdi"])
    
    trafo_alanlari_orijinal_parquet= os.path.join(python_dosya_yolu, f"trafo_alanlari_orijinal_{ilce}.parquet")
    trafo_alanlari_parquet = os.path.join(python_dosya_yolu, f"trafo_alanlari_{ilce}.parquet")
    trafo_alanlari_db_adi = f"trafo_alanlari_{ilce}"
    
    trafo_alanlari_sheet_name = "in"
    
    dfsuperhucre = os.path.join(python_dosya_yolu, f'dfsuperhucre_{ilce}.parquet')  # veya engine='fastparquet'
    dfsuperhucre_db_adi = f"dfsuperhucre_{ilce}"

    dfhucre_super = os.path.join(python_dosya_yolu, f'dfhucre_super_{ilce}.parquet')
    dfhucre_super_db_adi = f"dfhucre_super_{ilce}"
    
    df_hucre_parquet = os.path.join(python_dosya_yolu, f'dfhucre_{ilce}.parquet')
    df_hucre_db_adi = f"df_hucre_{ilce}"
    
    arsiv = os.path.join(sonuc_yolu, "Arşiv")
    DosyaOlustur(arsiv)
    
    debug = False
    
    # EAVeri = os.path.join(data["FilePaths"]["EAVeriYolu"], "ea_kumulatif_dagilim_sonuclariFinal.xlsx")
    # EAVeri = os.path.join(data["FilePaths"]["EAVeriYolu"], "ea_tepebaşı.xlsx")

    EAVeritabani = data["FilePaths"]["EAYukVeriTabanıYolu"]
    # EAVeri_db_adi = data["Degiskenler"]["AlansalYukTabloAdi"]
    EAVeri_db_adi = ilce
    
    odtr = {
        
        "ilk": ilk,
        
        "sonuc_yolu": sonuc_yolu,
        
        "python_dosya_yolu": python_dosya_yolu,
        
        "excel_kayit_path": excel_kayit_path,
        
        "yuk_path": yuk_path,
        
        "hucre_path": hucre_path,
        
        "yuk_db_yolu": yuk_db_yolu,
        
        "pointload_path": pointload_path,

        "yuk_path_parquet" : yuk_path_parquet,
        
        "il": il,

        "ilk_yil" : int(ilk_yil) + 1,

        "son_yil" : int(son_yil),

        "ilce" : ilce,
        
        "trafo_sheet_name" : trafo_sheet_name,

        "trafo_alanlari_path" : trafo_alanlari_path,

        "trafo_alanlari_sheet_name" : trafo_alanlari_sheet_name,
        
        "dfsuperhucre": dfsuperhucre,
        
        "dfhucre_super": dfhucre_super,
        
        "df_hucre_parquet": df_hucre_parquet,
        
        "dosyalar": dosyalar,
        
        "yuk_db_adi": yuk_db_adi,
        
        "trafo_db_adi": trafo_db_adi,
        
        "trafo_alanlari_db_adi": trafo_alanlari_db_adi,
        
        "dfsuperhucre_db_adi": dfsuperhucre_db_adi,
        
        "dfhucre_super_db_adi": dfhucre_super_db_adi,
        
        "df_hucre_db_adi": df_hucre_db_adi,
        
        "arsiv": arsiv,
        
        "debug": debug,
        
        "EAVeriYolu": None,
        
        "EAVeri_db_adi": EAVeri_db_adi,
        
        "EAVeritabani": EAVeritabani,
        
        "db_yol": db_yol,
        
        "db_ad": db_ad,
        
        "yuk_db_adi": yuk_db_adi,
        
        "EAVeri_db_adi": EAVeri_db_adi,
        
        "pydeckRunOutput": pydeckRunOutput
        
               
   }

    odtr["trafo_path"] = trafo_path

    odtr["trafo_path_parquet"] = trafo_path_parquet

    odtr["trafo_alanlari_parquet"] = trafo_alanlari_parquet
    
    odtr["trafo_alanlari_orijinal_parquet"] = trafo_alanlari_orijinal_parquet

    return odtr

def get():
    """Daha önce load() ile yüklenmiş config'i döndür."""
    if odtr is None:
        
        raise RuntimeError("Config henüz yüklenmedi. Önce config.load(path) çağırın.")
    
    return odtr

def getp():
    """Daha önce load() ile yüklenmiş config'i döndür."""
    if p is None:
        
        raise RuntimeError("Config henüz yüklenmedi. Önce config3.ODTR(path) çağırın.")
    
    return p
