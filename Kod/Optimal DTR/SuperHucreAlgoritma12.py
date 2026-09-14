# -*- coding: utf-8 -*-
"""
Created on Tue Mar 18 22:38:31 2025

@author: vural.bayrakli
"""

import pandas as pd
import numpy as np
import concurrent.futures
from Functions3 import *
import config3
import os
import sys
import logging
import sqlite3
from datetime import datetime
import time
from joblib import Parallel, delayed
import pickle

test = None
dftrafoYıllık = None
dftrafocopy = None

logging.basicConfig(level=logging.DEBUG, format='%(asctime)s - %(levelname)s - %(message)s', stream=sys.stdout)

def init_globals(dfyıllık, df):
    global dftrafoYıllık, dftrafocopy
    dftrafoYıllık = dfyıllık
    dftrafocopy = df

def set_globals(dfy, df):
    global dftrafoYıllık, dftrafocopy
    dftrafoYıllık = dfy
    dftrafocopy = df

def get_globals():
    
    return dftrafoYıllık, dftrafocopy

# def process_shid(shid, group):
    
#     return YukAta4_3(group, dftrafo_filtered, dftrafoYıllık_filtered, dfhucre_super)

def process_shid(shid, group, dftrafoY_shid, dftrafocopy):
    
    return YukAta4_3(group, dftrafocopy, dftrafoY_shid)


def Paralelİslem(df_result1):
    
    dftrafoYıllık, dftrafocopy = get_globals()

    # Grup bazında işle
    groups = [(shid, dftrafoYıllık[dftrafoYıllık["shid"] == shid].copy(), dftrafocopy[dftrafocopy["shid"] == shid].copy(), group) 
              for shid, group in df_result1.groupby("shid")]

    # joblib ile paralel dağıt
    results = Parallel(n_jobs=6)(
        delayed(process_shid)(shid, group, dfy, df) for shid, dfy, df, group in groups
    )
    
    updated_partsYıllık = []
    updated_parts = []
    atama_parts = []
    
    for r in results:
        updated_dfYıllık = r[0]
        updated_df = r[1]
        atama_df = r[2]
    
        updated_partsYıllık.append(updated_dfYıllık)
        atama_parts.append(atama_df) 
        updated_parts.append(updated_df)

    
    # Şimdi concat edebilirsin
    dftrafoYillik_updated_all = pd.concat(updated_partsYıllık)

    dftrafoYıllık.update(dftrafoYillik_updated_all)
    
    dftrafo_updated_all = pd.concat(updated_parts)
    dftrafocopy.update(dftrafo_updated_all)
    
    set_globals(dftrafoYıllık, dftrafocopy)
    
    atama_sonucu_final = pd.concat(atama_parts)
    
    return atama_sonucu_final

def pickle_kaydet(veri, dosya_yolu):
    """
    Verilen veriyi .pkl formatında belirtilen yola kaydeder.

    Args:
        veri: Kaydedilecek Python nesnesi (ör. DataFrame, liste, dict vs.)
        dosya_yolu (str): Kaydedilecek dosyanın tam yolu (*.pkl uzantılı)
    """
    with open(dosya_yolu, 'wb') as dosya:
        pickle.dump(veri, dosya)
  
def pickle_oku(dosya_yolu):
    """
    Belirtilen .pkl dosyasını okuyarak Python nesnesi olarak döner.

    Args:
        dosya_yolu (str): Okunacak dosyanın tam yolu (*.pkl uzantılı)

    Returns:
        Python nesnesi (veri yapısı)
    """
    with open(dosya_yolu, 'rb') as dosya:
        veri = pickle.load(dosya)
    return veri

def pickle_kaydet_nesneler(nesne_dict, klasor="veri_kayitlari"):
    """
    Birden fazla nesneyi ayrı .pkl dosyalarına kaydeder.

    Args:
        nesne_dict (dict): {"dosya_adi": nesne} şeklinde sözlük.
        klasor (str): Kaydedilecek klasör adı.
    """
    import os
    os.makedirs(klasor, exist_ok=True)
    
    for isim, nesne in nesne_dict.items():
        yol = os.path.join(klasor, f"{isim}.pkl")
        with open(yol, 'wb') as dosya:
            pickle.dump(nesne, dosya)


def pickle_yukle_nesneler(isim_listesi, klasor="veri_kayitlari"):
    """
    Belirtilen klasörden birden fazla .pkl dosyasını yükler.

    Args:
        isim_listesi (list): Yüklenecek dosya adları (uzantısız).
        klasor (str): Dosyaların bulunduğu klasör adı.

    Returns:
        dict: {"dosya_adi": nesne} şeklinde döner.
    """
    veriler = {}
    for isim in isim_listesi:
        yol = os.path.join(klasor, f"{isim}.pkl")
        with open(yol, 'rb') as dosya:
            veriler[isim] = pickle.load(dosya)
    return veriler

    
class Run:
    
    def __init__(self, parametreler, dfinput, dosya):
        
        self.p = parametreler
        self.dfinputFile = dfinput
        self.d = dosya
        self.sayac = 0
        
    def Process(self):
        
        odtr = config3.get()
        
        self.sayac += 1
        
        dftrafo, dftrafoYıllık, df_binary, aksiyon_df, trafo, trafoYıllık = FonkTrafo()

        init_globals(dftrafoYıllık, dftrafo)
        
        testdf, _ = get_globals()
        
        dftrafocopy = dftrafo.copy()
    
        aksiyon_df = aksiyon_df.drop_duplicates(subset='trafo_id', keep='first')

        for idx, row in aksiyon_df.iterrows():
            
            trafo_id = row["trafo_id"]
            
            yıl = row["İşlem Tarihi"]
            
            aksiyon = row["aksiyon"]
            
            kapasite = row["yeni_kapasite"]
                        
            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"]==trafo_id) & (dftrafoYıllık["year"]==yıl)), "Trafo Aksiyon"] = aksiyon
            
            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"]==trafo_id)), "İşlem Tarihi"] = yıl
            
            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"]==trafo_id) & (dftrafoYıllık["year"]>=yıl)), "kapasite"] = kapasite
        
            dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo_id, "kapasite"] = kapasite
                        
            dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo_id,"Trafo Aksiyon"] = aksiyon
            
            dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo_id,"İşlem Tarihi"] = yıl
        
        if not trafoYıllık.empty:
            
            dftrafoYıllık = pd.concat([dftrafoYıllık, trafoYıllık], ignore_index=True)
            
        dftrafoYıllık = dftrafoYıllık.reset_index()
                             
        if not trafo.empty:
            
            dftrafocopy = pd.concat([dftrafocopy, trafo], ignore_index=True)     
                
        dftrafocopy = dftrafocopy.reset_index() 
        
        set_globals(dftrafoYıllık, dftrafocopy)
        
        testdf, _ = get_globals()
        
        logging.info(f"ITERASYON1 CALISIYOR(1/3)...")
        
        df_result1, dfdouble_superhucre = Start7(self.p, None, dftrafocopy, df_binary)
   
        # self.ExcelKaydet(df_result1, f"SuperHucreI{self.sayac}")
        
        path = os.path.join(self.d, f"SuperHucreI{self.sayac}.parquet")
        ToParquet(df_result1, path)
        #df_result1 = ReadParquet(odtr['yuk_parquet'])
        
        # self.ExcelKaydet(df_result1, f"SuperHucreI{self.sayac}") 
        
        dfhucre_super = ReadParquet(odtr["dfhucre_super"])
        
        df_result1 = df_result1[df_result1["year"] != odtr["ilk_yil"]-1].copy()

        dftrafocopy["shid"] = dftrafocopy["merkez_hucre"].map(dfhucre_super.set_index("hucreid")["shid"])
        dftrafoYıllık["shid"] = dftrafoYıllık["merkez_hucre"].map(dfhucre_super.set_index("hucreid")["shid"])
        
        dfsuperhucre = ReadParquet(odtr["dfsuperhucre"])
                
        gerilimler = dftrafocopy.groupby("shid")["primer_gerilim"].apply(list)
        
        max_gerilim = gerilimler.map(max)
        
        dftrafocopy["SH Gerilim"] = dftrafocopy["shid"].map(max_gerilim)
        dftrafoYıllık["SH Gerilim"] = dftrafoYıllık["shid"].map(max_gerilim)
        
        set_globals(dftrafoYıllık, dftrafocopy)
        
        testdf, _ = get_globals()
        
        dfsuperhucre["SH Gerilim"] = dfsuperhucre["shid"].map(max_gerilim)
        
        ToParquet(dfsuperhucre, odtr["dfsuperhucre"])

        logging.info(f"ITERASYON1 YUK ATAMA YAPILIYOR...")

        start = time.time()  # Başlangıç zamanı
        
        #dftrafocopy, dfatama_sonuc, trafoRapor = YukAta4(df_result1, dftrafocopy, dftrafoYıllık, dfhucre_super)
        dfatama_sonuc = Paralelİslem(df_result1)
        
        # self.ExcelKaydet(dfatama_sonuc, f"dfatama_sonucI{self.sayac}")
        
        logging.info(f"ITERASYON1 YUK ATAMA BITTI...")
        
        years = pd.DataFrame({
            "year":range(odtr["ilk_yil"], odtr["son_yil"]+1)
            })
                
        atanamayan_talep = dfatama_sonuc.groupby("year")["Atanamayan Talep"].sum().reset_index()
        
        atanamayan_talep = years.merge(atanamayan_talep, on="year", how="left")
        
        dftrafoYıllık, dftrafocopy = get_globals()

        end = time.time()  # Bitiş zamanı
        
        print(f"PARALEL ISLEM SURESI: {end - start:.2f} saniye")
        
        path3 = os.path.join(self.d, f"dfatama_sonucI{self.sayac}.parquet")
        
        ToParquet(dfatama_sonuc, path3)
        
        atanamayanlar = dfatama_sonuc[dfatama_sonuc["durum"] == "Atanmadı"]
        
        logging.info(f"ITERASYON1 KAPASITE ARTIRMA ISLEMI YAPILIYOR...")

        dftrafocopy = TrafoKapasiteArtirGuncel2(self.p, atanamayanlar, dftrafocopy, dftrafoYıllık, dfhucre_super) 
        
        set_globals(dftrafoYıllık, dftrafocopy)
        
        dftrafocopy, dftrafoYıllık = TrafoKapasiteArtirGuncel3(self.p, dftrafocopy, dftrafoYıllık, dfhucre_super)
              
        set_globals(dftrafoYıllık, dftrafocopy)
        
        dftrafocopy = TrafoYenile(dftrafocopy, dftrafoYıllık)   
        
        set_globals(dftrafoYıllık, dftrafocopy)
        
        self.dftrafocopy3 = dftrafocopy.copy()          
        
        dfatama_sonuc2 = dfatama_sonuc.copy() 

        #dfatama_sonuc2.loc[dfatama_sonuc2["durum"] == "Atandı", "kumulatif_yuk"] = 0.0
        
        dfatama_sonuc2 = dfatama_sonuc2[dfatama_sonuc2["durum"]=="Atanmadı"]

        dfatama_sonuc2.rename(columns={"kumulatif_yuk": "delta_TOTAL_POWER_KVA"}, inplace=True)
        
        dfatama_sonuc2 = dfatama_sonuc2.merge(df_result1[["shid", "year", "TOTAL_POWER", "delta_TOTAL_POWER"]], on=["shid", "year"], how="left")
        
        logging.info(f"ITERASYON1 BITTI...")

        self.sayac += 1
        
        logging.info(f"ITERASYON2(2/3) BASLIYOR...")


        #dfsupertest2 = SkorHesapla4(p, dfhucre_super, dftrafocopy3, df_binary, dfatama_sonuc2, dfdouble_superhucre, False)
        dfsupertest2 = SkorHesapla4(self.p, dfhucre_super, dftrafocopy, df_binary, dfatama_sonuc2, dfdouble_superhucre, False)
 
        logging.info(f"ITERASYON2 YUK ATAMA YAPILIYOR...")
        
        dfatama_sonuc3 = Paralelİslem(dfsupertest2)
        
        logging.info(f"ITERASYON2 YUK ATAMA BITTI...")

        atanamayan_talep2 = dfatama_sonuc3.groupby("year")["Atanamayan Talep"].sum().reset_index()
        
        atanamayan_talep2 = years.merge(atanamayan_talep2, on="year", how="left")
        
        dftrafoYıllık, dftrafocopy = get_globals()
        
        path = os.path.join(self.d, f"SuperHucreI{self.sayac}.parquet")
        path2 = os.path.join(self.d, f"dfatama_sonucI{self.sayac}.parquet")

        ToParquet(dfsupertest2, path)
        ToParquet(dfatama_sonuc3, path2)
        

        # self.ExcelKaydet(dfsupertest2, f"SuperHucreI{self.sayac}")
        # self.ExcelKaydet(dfatama_sonuc3, f"dfatama_sonucI{self.sayac}")
        
        atanamayanlar2 = dfatama_sonuc3[dfatama_sonuc3["durum"] == "Atanmadı"]
        
        logging.info(f"ITERASYON2 TRAFO EKLEME ISLEMI YAPILIYOR...")

        dfyeni_trafo, dfyenitrafoyıllık = TrafoEkle(self.p, dftrafocopy, df_binary, atanamayanlar2, dfhucre_super)
        
        # Her iki DataFrame'in indekslerini sıfırlayalım
        dftrafocopy = dftrafocopy.reset_index(drop=True)
        dfyeni_trafo = dfyeni_trafo.reset_index(drop=True)
        dfyenitrafoyıllık = dfyenitrafoyıllık.reset_index(drop=True)
        
        dftrafoYıllık = pd.concat([dftrafoYıllık, dfyenitrafoyıllık], ignore_index=True)
        
        # concat işlemiyle iki DataFrame'i birleştir
        dftrafocopy = pd.concat([dftrafocopy, dfyeni_trafo], ignore_index=True)
        
        set_globals(dftrafoYıllık, dftrafocopy)

        self.dfyeni_trafo = dfyeni_trafo.copy()
        self.atanamayanlar2 = atanamayanlar2.copy()
        
        dfatama_sonuc3 = dfatama_sonuc3[dfatama_sonuc3["durum"]=="Atanmadı"]

        dfatama_sonuc3.rename(columns={"kumulatif_yuk": "delta_TOTAL_POWER_KVA"}, inplace=True)
        
        dfatama_sonuc3 = dfatama_sonuc3.merge(dfsupertest2[["shid", "year", "TOTAL_POWER", "delta_TOTAL_POWER"]], on=["shid", "year"], how="left")

        self.sayac += 1
        
        logging.info(f"ITERASYON2 BITTI...")
   
        logging.info(f"ITERASYON3(3/3) BASLIYOR...")

        dfsupertest3 = SkorHesapla4(self.p, dfhucre_super, dftrafocopy, df_binary, dfatama_sonuc3, dfdouble_superhucre, False)
        # dfsupertest3 = SkorHesapla5(p, dfhucre_super, dftrafoYıllık3, df_binary, dfatama_sonuc3, dfdouble_superhucre, False)

        logging.info(f"ITERASYON3 YUK ATAMA YAPILIYOR...")
        
        dfatama_sonuc4 = Paralelİslem(dfsupertest3)
        
        logging.info(f"ITERASYON3 YUK ATAMA BITTI...")
        
        atanamayan_talep3 = dfatama_sonuc4.groupby("year")["Atanamayan Talep"].sum().reset_index()
        
        atanamayan_talep3 = years.merge(atanamayan_talep3, on="year", how="left")
        
        dftrafoYıllık, dftrafocopy = get_globals()
        
        path = os.path.join(self.d, f"SuperHucreI{self.sayac}.parquet")
        path2 = os.path.join(self.d, f"dfatama_sonucI{self.sayac}.parquet")

        ToParquet(dfsupertest3, path)
        ToParquet(dfatama_sonuc4, path2)
        
        # self.ExcelKaydet(dfsupertest3, f"SuperHucreI{self.sayac}")
        # self.ExcelKaydet(dfatama_sonuc4, f"dfatama_sonucI{self.sayac}")

        atanamayanlar3 = dfatama_sonuc4[dfatama_sonuc4["durum"] == "Atanmadı"]
        
        logging.info(f"ITERASYON3 TRAFO EKLEME ISLEMI YAPILIYOR...")

        dfyeni_trafo2, dfyenitrafoyıllık2 = TrafoEkle(self.p, dftrafocopy, df_binary, atanamayanlar3, dfhucre_super, False)
        
        dfyeni_trafo2 = dfyeni_trafo2.reset_index(drop=True)
        dfyenitrafoyıllık2 = dfyenitrafoyıllık2.reset_index(drop=True)
        
        dftrafoYıllık = pd.concat([dftrafoYıllık, dfyenitrafoyıllık2], ignore_index=True)
        dftrafocopy = pd.concat([dftrafocopy, dfyeni_trafo2], ignore_index=True) 
        
        set_globals(dftrafoYıllık, dftrafocopy)
        
        atanamayanlar4 = atanamayanlar3.rename(columns={"kumulatif_yuk":"power_distribution"})
        
        dfatama_sonuc5 = Paralelİslem(atanamayanlar4)
        
        atanamayan_talep4 = dfatama_sonuc5.groupby("year")["Atanamayan Talep"].sum().reset_index()
        
        atanamayan_talep4 = years.merge(atanamayan_talep4, on="year", how="left")
        
        atanamayan_talepler = pd.concat([atanamayan_talep, atanamayan_talep2, atanamayan_talep3, atanamayan_talep4], ignore_index=True)

        # Yıl bazında tekrar grupla ve 'Atanamayan Talep' toplamını al
        atanamayan_talepler = atanamayan_talepler.groupby("year", as_index=False).agg({"Atanamayan Talep": "sum"})
        
        dftrafoYıllık, dftrafocopy = get_globals()

        atanamayanlar4 = dfatama_sonuc5[dfatama_sonuc5["durum"] == "Atanmadı"]
        
        # Grupla: yıl bazında toplam yük ve trafo adedi
        yil_bazli_Atanamayanlar = (
            atanamayanlar4.groupby("year")["kumulatif_yuk"]
            .sum()
            .reset_index(name="Atanamayan Talep")
        )

        yil_bazli_Atanamayanlar = years.merge(yil_bazli_Atanamayanlar, on="year", how="left")
        
        atanamayan_talepler = pd.concat([atanamayan_talepler, yil_bazli_Atanamayanlar], ignore_index=True)
        
        self.atanamayan_talepler = atanamayan_talepler.groupby("year", as_index=False).agg({"Atanamayan Talep": "sum"})

        self.sayac += 1

        path2 = os.path.join(self.d, f"dfatama_sonucI{self.sayac}.parquet")
        
        ToParquet(dfatama_sonuc5, path2)

        # self.ExcelKaydet(dfatama_sonuc5, f"dfatama_sonucI{self.sayac}")

        logging.info(f"ITERASYON3 BITTI...")        

        self.trafo = dftrafocopy.copy()
        
        self.trafoYıllık = dftrafoYıllık.copy()
        
        self.TrafoOzet2()
    
    
    def ExcelKaydet(self, df, name):
        
        path = os.path.join(self.d, f"{name}{self.sayac}.xlsx")
        
        df.to_excel(path, engine="openpyxl")
        
    def TrafoOzet2(self):
        
        odtr = config3.get()
        
        all_years = pd.Series(list(range(odtr["ilk_yil"]-1, odtr["son_yil"] +1)), name="year")
        
        # df = self.dftrafocopy7.copy()
        df = self.trafo.copy()

        
        df = df[df["Trafo Aksiyon"] == "yeni trafo tesis"].copy()

        df_year_counts = df.groupby("year").size().reset_index(name="yeni eklenen").astype(int)
        
        # all_years ile birleştirme
        df_full_year_Eklenen = all_years.to_frame().merge(df_year_counts, on="year", how="left").fillna(0)
        
        df_full_year_Eklenen = df_full_year_Eklenen.rename(columns={"yeni eklenen": "yeni trafo tesis (Adet)"})
        
        
        # yenilenen
        # df = self.dftrafocopy7.copy()
        df = self.trafo.copy()

        df = df[df["Trafo Aksiyon"] == "trafo yenileme-yaştan"].copy()
        
        df_year_Yenilenen = df.groupby("yenileme_yili").size().reset_index(name="yenilenen").astype(int)
        
        df_year_Yenilenen["year"] = df_year_Yenilenen["yenileme_yili"].astype(int)
        
        # all_years ile birleştirme
        df_full_year_Yenilenen = all_years.to_frame().merge(df_year_Yenilenen, on="year", how="left").fillna(0)
        
        df_full_year_Yenilenen.drop(["yenileme_yili"], axis=1, inplace=True)
        
        df_full_year_Yenilenen = df_full_year_Yenilenen.rename(columns={"yenilenen": "trafo yenileme-yaştan dolayı (Adet)"})

        
        # df_year_kYenilenen = self.dftrafocopy7[self.dftrafocopy7["Trafo Aksiyon"] == "trafo yükseltme-kapasiteden"].copy()
        df_year_kYenilenen = self.trafo[self.trafo["Trafo Aksiyon"] == "trafo yükseltme-kapasiteden"].copy()

        df_year_kYenilenen = df_year_kYenilenen.groupby("Kapasite_yenilenme_yili").size().reset_index(name="yenilenen").astype(int)

        df_year_kYenilenen["year"] = df_year_kYenilenen["Kapasite_yenilenme_yili"].astype(int)
        
        # all_years ile birleştirme
        df_full_year_kYenilenen = all_years.to_frame().merge(df_year_kYenilenen, on="year", how="left").fillna(0)
        
        df_full_year_kYenilenen.drop(["Kapasite_yenilenme_yili"], axis=1, inplace=True)
        
        df_full_year_kYenilenen = df_full_year_kYenilenen.rename(columns={"yenilenen": "trafo yenileme-kapasiteden (Adet)"})

        
        # kapasite artan
        # df_year_Artan = self.dftrafocopy3.groupby("Kapasite_artirma_yili").size().reset_index(name="kapasitesi artan").astype(int)
        df_year_Artan = self.trafo.groupby("Kapasite_artirma_yili").size().reset_index(name="kapasitesi artan").astype(int)

        df_year_Artan["year"] = df_year_Artan["Kapasite_artirma_yili"].astype(int)

        df_full_year_Artan = all_years.to_frame().merge(df_year_Artan, on="year", how="left").fillna(0)

        df_full_year_Artan.drop(["Kapasite_artirma_yili"], axis=1, inplace=True)
        
        df_full_year_Artan = df_full_year_Artan.rename(columns={"kapasitesi artan": "trafo yükseltme-yükten (Adet)"})       
        
        df_merged = df_full_year_Eklenen.merge(df_full_year_Yenilenen, on="year") 
        df_merged = df_merged.merge(df_full_year_Artan, on="year")
        df_merged = df_merged.merge(df_full_year_kYenilenen, on="year")
        
        df_merged["year"] = df_merged["year"].astype(int)
        
        self.TrafoOzet = df_merged
        
        self.ExcelKaydet(df_merged, "trafoOzet")