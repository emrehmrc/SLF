# -*- coding: utf-8 -*-
"""
Created on Mon May  5 18:08:35 2025

@author: vural.bayrakli
"""

import pandas as pd
import numpy as np
import pickle
#from config2 import Start
import config3
from Functions3 import EAIslem, ReadParquet, SqliteOku, yeni_trafo_df_olustur, YukAtaEA
import logging
import time
from joblib import Parallel, delayed
from collections import defaultdict

# Batch oluşturma için bağımsız SHID gruplarını ayıran fonksiyon
def gruplari_ayir_disjoint(groups, dftrafocopy, dfsuperhucre):
    # 1. Her SHID'in kullandığı trafoları topla
    shid_kullanim = defaultdict(set)

    for ID, group in groups:
        
        trafolar = dftrafocopy[
            (dftrafocopy["merkez_hucre"] == ID) &               
            (dftrafocopy["Trafo Mülkiyeti"] == "Özel")
        ].copy()
        
        if trafolar.empty:
            
            trafolar = dftrafocopy[
                (dftrafocopy["merkez_hucre"] == ID) &               
                (dftrafocopy["Trafo Mülkiyeti"] == "Kurum")
            ].copy()
        
        
            if trafolar.empty:
                # Komşulara bak
                
                shid = dfsuperhucre[dfsuperhucre["hucreid"]==ID]["shid"].values
                
                try:
                    if len(shid) > 0:
                        shid=shid[0]
                    
                    trafolar = dftrafocopy[
                        (dftrafocopy["shid"] == shid) &                       
                        (dftrafocopy["Trafo Mülkiyeti"] == "Kurum")
                    ].copy()  
                    
                except Exception as e:
                    logging.error()
    
            for tid in trafolar["trafo_id"].unique():
                shid_kullanim[ID].add(tid)

    # 2. Bağımsız grupları oluştur
    disjoint_batches = []
    kalan = set(ID for ID, _ in groups)

    while kalan:
        batch = []
        kullanilan_trafolar = set()

        for ID in list(kalan):
            trafolar = shid_kullanim.get(ID, set())
            if trafolar.isdisjoint(kullanilan_trafolar):
                batch.append(ID)
                kullanilan_trafolar.update(trafolar)
                kalan.remove(ID)

        disjoint_batches.append(batch)

    return disjoint_batches

def process_shid_group(ID, group, dftrafoYıllık, dftrafocopy, dfsuperhucre, ilk_atama=True):
    
    trafolar = pd.DataFrame()
    
    if ilk_atama:
        
        trafolar = dftrafoYıllık[
            (dftrafoYıllık["merkez_hucre"] == ID) &
            (dftrafoYıllık["Trafo Mülkiyeti"] == "Özel")
        ].copy() 
        
        if trafolar.empty:
            
            trafolar = dftrafoYıllık[
                (dftrafoYıllık["merkez_hucre"] == ID) &               
                (dftrafoYıllık["Trafo Mülkiyeti"] == "Kurum")
            ].copy()
            
            if trafolar.empty:
                
                shid = dfsuperhucre[dfsuperhucre["hucreid"]==ID]["shid"].values
                
                try:
                    if len(shid) > 0:
                        
                        shid=shid[0]
                    
                    trafolar = dftrafoYıllık[
                        (dftrafoYıllık["shid"] == shid) &                       
                        (dftrafoYıllık["Trafo Mülkiyeti"] == "Kurum")
                    ].copy()  
                    
                except Exception as e:
                    logging.error()
                  
        trafolarYillik = trafolar.copy()
        
        trafolar = dftrafocopy[dftrafocopy["trafo_id"].isin(trafolar["trafo_id"].values)].copy()
        
    else:
        
        # dftrafoYıllık = ka.trafoYıllık.copy()
        # dftrafocopy = ka.trafo.copy()
        trafolar = dftrafocopy[(dftrafocopy["merkez_hucre"] == ID) & (dftrafocopy["Trafo Aksiyon"] == "yeni trafo tesis EA")].copy()
        trafolarYillik = dftrafoYıllık[dftrafoYıllık["trafo_id"].isin(trafolar["trafo_id"].values)].copy()
        
    if trafolar.empty:
        
            trafolar = pd.DataFrame(columns=dftrafocopy.columns)
            trafolarYillik = pd.DataFrame(columns=dftrafoYıllık.columns)

    return (ID, trafolarYillik, trafolar, group)

def process_shid(ID, group, dftrafoY_shid, dftrafocopy):
    
    return YukAtaEA(group, dftrafocopy, dftrafoY_shid)


# Yeni versiyon: Paralelİslem fonksiyonu içinde bağımsız batch mantığıyla paralel çalışma
def Paralelİslem_Batch(df_result1, dftrafoYıllık, dftrafocopy, dfhucresuper, p, ilk=True):

    # SHID bazlı gruplar
    groups = [(ID, group) for ID, group in df_result1.groupby("ID")]

    # SHID → group eşlemesi
    group_map = {ID: group for ID, group in groups}

    # Bağımsız batch'leri oluştur
    disjoint_batches = gruplari_ayir_disjoint(groups, dftrafocopy, dfhucresuper)

    updated_partsYıllık = []
    updated_parts = []
    atama_parts = []

    for batch in disjoint_batches:
        batch_groups = [(ID, group_map[ID]) for ID in batch]

        groups2 = Parallel(n_jobs=4)(
            delayed(process_shid_group)(ID, group, dftrafoYıllık, dftrafocopy, dfhucresuper, ilk)
            for ID, group in batch_groups
        )

        results = Parallel(n_jobs=4)(
            delayed(process_shid)(ID, group, dfy, df) for ID, dfy, df, group in groups2
        )

        for r in results:
            updated_dfYıllık = r[0]
            updated_df = r[1]
            atama_df = r[2]

            updated_partsYıllık.append(updated_dfYıllık)
            atama_parts.append(atama_df)
            updated_parts.append(updated_df)

        # Güncellemeyi bu batch sonunda uygula (çakışma riski sıfır)
        if updated_partsYıllık:
            dftrafoYillik_updated_all = pd.concat(updated_partsYıllık)
            dftrafoYıllık.update(dftrafoYillik_updated_all)
            updated_partsYıllık.clear()

        if updated_parts:
            dftrafo_updated_all = pd.concat(updated_parts)
            dftrafocopy.update(dftrafo_updated_all)
            updated_parts.clear()

    atama_sonucu_final = pd.concat(atama_parts) if atama_parts else pd.DataFrame()

    return dftrafoYıllık, dftrafocopy, atama_sonucu_final

class EA:
    
    def __init__(self, df, trafo, trafoYıllık, dfsuperhucre, p):
        
        try:
            odtr = config3.get()
            
        except Exception as e:
            logging.error(e)
        
        self.p = p
        
        try:            
            
            self.dfhucre_super = ReadParquet(odtr["dfhucre_super"])
        
        except Exception as e:
            print(e)
            
            return
        
        self.trafo = trafo
        
        self.trafoYıllık = trafoYıllık
        
        self.df = df
        
        self.df = EAIslem(df)
        
        self.df = self.df[self.df["year"] != odtr["ilk_yil"]-1]
        
        self.dfsuperhucre = dfsuperhucre
        
        start = time.time()
        
        # self.dfatama_sonuc, self.trafoRapor = self.YukAtama(self.df, self.trafo, self.trafoYıllık)
        self.trafoYıllık, self.trafo, self.dfatama_sonuc = Paralelİslem_Batch(self.df, self.trafoYıllık, self.trafo, self.dfhucre_super , self.p)
                
        self.dfatama_sonuc = self.dfatama_sonuc.rename(columns={"kumulatif_yuk": "power_distribution"})

        self.atanamayanlar = self.dfatama_sonuc[self.dfatama_sonuc["durum"] == "Atanmadı"]
        
        self.dfyeni, self.dfYıllık = self.TrafoEkle(self.atanamayanlar)

        end = time.time()
                
        # self.dfatama_sonuc, self.trafoRapor = self.YukAtama(self.df, self.dfyeni, self.dfYıllık)
        self.dfYıllık, self.dfyeni, self.dfatama_sonuc = Paralelİslem_Batch(self.atanamayanlar, self.dfYıllık, self.dfyeni, self.dfhucre_super , self.p, False)
                
        self.trafoYıllık = pd.concat([self.trafoYıllık, self.dfYıllık], ignore_index=True).reset_index(drop=True)
        
        self.trafo = pd.concat([self.trafo, self.dfyeni], ignore_index=True).reset_index(drop=True)
        
        self.TrafoOzet()
        
    
    def komsulari_bul(self, i, j, mesafe=1, capraz_dahil=True):
        komsu_listesi = []
        
        for di in range(-mesafe, mesafe + 1):
            for dj in range(-mesafe, mesafe + 1):
                if di == 0 and dj == 0:
                    continue  # Kendisi hariç
                if not capraz_dahil and abs(di) + abs(dj) != 1:
                    continue  # Çapraz hariçse sadece 4 yön
                komsu_listesi.append((i + di, j + dj))
        
        return komsu_listesi
        
    def YukAtama(self, df, dftrafocopy2, dftrafoYıllık):

        atama_sonucu_yeni = []

        # Yeni DataFrame oluştur (trafoların yıl bazında atama miktarlarını takip etmek için)
        df_yil_bazli_atama = pd.DataFrame(columns=["year", 
                                                   "trafo_id",
                                                   "merkez_hucre",
                                                   "kalan_kapasite", 
                                                   "kalan_kapasite_ek",
                                                   "atanan_miktar", 
                                                   "atanan_miktar_ek",
                                                   "yaş",
                                                   "eski kapasite",
                                                   "yeni kapasite", 
                                                   "kapasite artirma yili",
                                                   "GelenYukToplam"])

        # Dosyayı aç (append mode - "a" ile her çalıştırmada ekleme yap)
     
        dfMain = df.copy()

        for hucreid, dfsect in dfMain.groupby("id"):
            
            Atanmadı_Cikti = False
            
            dff = dfsect.copy() 
            
            if not dff["power_distribution"].eq(0).all():
                
                for index, row in dff.iterrows():
                    
                    yil = row["year"]                                      
                    
                    yuk_miktari = row["power_distribution"]
                                       
                    trafolar = dftrafoYıllık[
                        (dftrafoYıllık["merkez_hucre"] == hucreid) &
                        (dftrafoYıllık["year"] == yil) &
                        (dftrafoYıllık["Trafo Mülkiyeti"] == "Özel")
                    
                    ].copy() 
                    
                    if trafolar.empty:
                        
                        trafolar = dftrafoYıllık[
                            (dftrafoYıllık["merkez_hucre"] == hucreid) &
                            (dftrafoYıllık["year"] == yil) &
                            (dftrafoYıllık["Trafo Mülkiyeti"] == "Kurum")
                        
                        ].copy()
                        
                        if trafolar.empty:
                                                              
                            shid = self.dfsuperhucre[self.dfsuperhucre["hucreid"]==hucreid]["shid"].values
                            
                            try:
                                shid=shid[0]
                                
                                trafolar = dftrafoYıllık[
                                    (dftrafoYıllık["shid"] == shid) &
                                    (dftrafoYıllık["year"] == yil) &
                                    (dftrafoYıllık["Trafo Mülkiyeti"] == "Kurum")
                                
                                ].copy()  
                                
                            except Exception as e:
                                logging.error(e)                              
                                
                        if not trafolar.empty:
                            trafolar = trafolar.loc[[trafolar["efektif_bos_kapasite"].idxmax()]]
                                                    
                
                    toplam_boskapasite = trafolar["bos_kapasite"].sum()  
                    toplam_kapasite = trafolar["kapasite"].sum()  
                    toplam_boskapasite_ek = trafolar["efektif_bos_kapasite"].sum() 
                    toplam_kapasite_ek = trafolar["efektif_kapasite"].sum() 
                    
                    
                    trafolar["atama_miktari"] = 0.0
                    trafolar["atama_miktari_ek"] = 0.0
                    trafolar["kumulatif_atama_miktari"] = 0.0
                    trafolar["kumulatif_bos_kapasite"] = 0.0
                    
                    if not Atanmadı_Cikti:
                        
                        if ((toplam_boskapasite_ek > yuk_miktari and toplam_boskapasite_ek > 0) or yuk_miktari <= 0):
                            
                            # Payda sıfırsa atama işlemi yapmadan önce kontrol ekleyelim
                            trafolar["atama_orani"] = np.where(
                                toplam_boskapasite != 0, 
                                trafolar["bos_kapasite"] / toplam_boskapasite, 
                                0  # Payda sıfırsa 0 değeri atansın
                            )
                            
                            trafolar["atama_orani_ek"] = np.where(
                                toplam_boskapasite_ek != 0, 
                                trafolar["efektif_bos_kapasite"] / toplam_boskapasite_ek, 
                                0  # Payda sıfırsa 0 değeri atansın
                            )
                                                                            
                            trafolar["atama_miktari"] = yuk_miktari * trafolar["atama_orani"]
                            trafolar["atama_miktari_ek"] = yuk_miktari * trafolar["atama_orani_ek"]
                                                            
                            trafolar["GelenYukToplam"] += trafolar["atama_miktari_ek"]
                            
                            trafolar["kumulatif_bos_kapasite"] = trafolar["efektif_kapasite"] - trafolar["GelenYukToplam"]
                            
                            trafolar["bos_kapasite"] -= trafolar["atama_miktari"]
                            
                            
                            trafolar["efektif_bos_kapasite"] -= trafolar["atama_miktari_ek"]                       
                            
                            trafolar.loc[(trafolar["kapasite"] != 0), "Nominal_Bos_yuzde"] = (
                                trafolar["bos_kapasite"] / trafolar["kapasite"]
                            )
                            
                            trafolar.loc[(trafolar["efektif_kapasite"] != 0), "Efektif_Bos_yuzde"] = (
                                trafolar["kumulatif_bos_kapasite"] / trafolar["efektif_kapasite"]
                            )
                            
                            trafolar.loc[(trafolar["kapasite"] != 0), "Doluluk Oranı"] = (
                                trafolar["GelenYukToplam"] / trafolar["kapasite"])

                            
                                            
                            dftrafoYıllık.update(trafolar)
                            
                            for index, row in trafolar.iterrows():
                                
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row["trafo_id"]) & (dftrafoYıllık["year"] >= yil+1)), "GelenYukToplam"] = row["GelenYukToplam"]
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row["trafo_id"]) & (dftrafoYıllık["year"] >= yil+1)), "İlkGelenYukToplam"] = row["GelenYukToplam"]
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row["trafo_id"]) & (dftrafoYıllık["year"] >= yil+1)), "bos_kapasite"] = row["bos_kapasite"]
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row["trafo_id"]) & (dftrafoYıllık["year"] >= yil+1)), "efektif_bos_kapasite"] = row["efektif_bos_kapasite"]
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row["trafo_id"]) & (dftrafoYıllık["year"] >= yil+1)), "kullanilan_kapasite_efektif"] = row["kullanilan_kapasite_efektif"]
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row["trafo_id"]) & (dftrafoYıllık["year"] >= yil+1)), "Doluluk Oranı"] = row["GelenYukToplam"] / row["kapasite"]
                                
                                
                                dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row["trafo_id"], "bos_kapasite"] = row["bos_kapasite"]
                                dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row["trafo_id"], "efektif_bos_kapasite"] = row["efektif_bos_kapasite"]
                                dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row["trafo_id"], "kullanilan_kapasite_efektif"] = row["kullanilan_kapasite_efektif"]
                                dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row["trafo_id"], "trafo_yasi"] = row["trafo_yasi"]
                                dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row["trafo_id"], "GelenYukToplam"] = row["GelenYukToplam"]
                                dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row["trafo_id"], "Doluluk Oranı"] = row["GelenYukToplam"] / row["kapasite"]
                                            
                            durum = "Atandı"               
                            
                        else:
                            
                            durum = "Atanmadı"
                            
                            Atanmadı_Cikti = True
                            
                        GelenYukToplam = trafolar["GelenYukToplam"].sum()
                    
                        toplam_boskapasite_ek = trafolar["efektif_bos_kapasite"].sum() 
                    
                        atama_sonucu_yeni.append([yil, hucreid, yuk_miktari, toplam_boskapasite_ek, GelenYukToplam, durum])

                            
                    else:
                        
                        durum = "Atanmadı"
                        
                        GelenYukToplam = trafolar["GelenYukToplam"].sum()
                    
                        toplam_boskapasite_ek = trafolar["efektif_bos_kapasite"].sum()
                        
                        atama_sonucu_yeni.append([yil, hucreid, yuk_miktari, toplam_boskapasite_ek, GelenYukToplam, durum])
                    

        return pd.DataFrame(atama_sonucu_yeni, columns=["year", "ID", "power_distribution", "toplam_boskapasite", "GelenYukToplam", "durum"]), df_yil_bazli_atama   
    
    def YukAtama2(self, df, dftrafocopy):
    
        
        atama_sonucu_yeni = []
        
        dftrafocopy["Nominal_Bos_yuzde"] = 0.0
        dftrafocopy["Efektif_Bos_yuzde"] = 0.0
        dftrafocopy["atanan_miktar_ek"] = 0.0
    
        # Yeni DataFrame oluştur (trafoların yıl bazında atama miktarlarını takip etmek için)
        df_yil_bazli_atama = pd.DataFrame(columns=["year", 
                                                   "trafo_id",
                                                   "merkez_hucre",
                                                   "kalan_kapasite", 
                                                   "kalan_kapasite_ek",
                                                   "atanan_miktar", 
                                                   "atanan_miktar_ek",
                                                   "yaş",
                                                   "eski kapasite",
                                                   "yeni kapasite", 
                                                   "kapasite artirma yili",
                                                   "GelenYukToplam"])
    
        # Dosyayı aç (append mode - "a" ile her çalıştırmada ekleme yap)
     
        dfMain = df.copy()
    
        for hucreid, dfsect in dfMain.groupby("id"):
            
            Atanmadı_Cikti = False
            
            dff = dfsect.copy() 
            
            trafolar = pd.DataFrame(columns=dftrafocopy.columns)     
            
            if not dff["power_distribution"].eq(0).all():
                
                for index, row in dff.iterrows():
                    
                    yil = row["year"]                                      
                    
                    yuk_miktari = row["power_distribution"]
                    
                    if trafolar.empty:
                        
                        trafolar = dftrafocopy[
                            (dftrafocopy["merkez_hucre"] == hucreid) &
                            (dftrafocopy["year"] <= yil) &
                            (dftrafocopy["Trafo Mülkiyeti"] == "Özel")
                        
                        ].copy() 
                        
                        if trafolar.empty:
                            
                            trafolar = dftrafocopy[
                                (dftrafocopy["merkez_hucre"] == hucreid) &
                                (dftrafocopy["year"] <= yil) &
                                (dftrafocopy["Trafo Mülkiyeti"] == "Kurum")
                            
                            ].copy()
                            
                            if trafolar.empty:
                                                                  
                                shid = self.dfsuperhucre[self.dfsuperhucre["hucreid"]==hucreid]["shid"].values
                                
                                try:
                                    shid=shid[0]
                                    
                                    trafolar = dftrafocopy[
                                        (dftrafocopy["shid"] == shid) &
                                        (dftrafocopy["year"] <= yil) &
                                        (dftrafocopy["Trafo Mülkiyeti"] == "Kurum")
                                    
                                    ].copy()  
                                    
                                except Exception as e:
                                    logging.error(e)                              
                                    
                            if not trafolar.empty:
                                trafolar = trafolar.loc[[trafolar["efektif_bos_kapasite"].idxmax()]]
                                                    
                
                    toplam_boskapasite = trafolar["bos_kapasite"].sum()  
                    toplam_kapasite = trafolar["kapasite"].sum()  
                    toplam_boskapasite_ek = trafolar["efektif_bos_kapasite"].sum() 
                    toplam_kapasite_ek = trafolar["efektif_kapasite"].sum() 
                    
                    
                    trafolar["atama_miktari"] = 0.0
                    trafolar["atama_miktari_ek"] = 0.0
                    trafolar["kumulatif_atama_miktari"] = 0.0
                    trafolar["kumulatif_bos_kapasite"] = 0.0
                    
                    if not Atanmadı_Cikti:
                        
                        if ((toplam_boskapasite_ek > yuk_miktari and toplam_boskapasite_ek > 0) or yuk_miktari <= 0):
                            
                            # Payda sıfırsa atama işlemi yapmadan önce kontrol ekleyelim
                            trafolar["atama_orani"] = np.where(
                                toplam_boskapasite != 0, 
                                trafolar["bos_kapasite"] / toplam_boskapasite, 
                                0  # Payda sıfırsa 0 değeri atansın
                            )
                            
                            trafolar["atama_orani_ek"] = np.where(
                                toplam_boskapasite_ek != 0, 
                                trafolar["efektif_bos_kapasite"] / toplam_boskapasite_ek, 
                                0  # Payda sıfırsa 0 değeri atansın
                            )
                                                                            
                            trafolar["atama_miktari"] = yuk_miktari * trafolar["atama_orani"]
                            trafolar["atama_miktari_ek"] = yuk_miktari * trafolar["atama_orani_ek"]
                                                            
                            trafolar["GelenYukToplam"] += trafolar["atama_miktari_ek"]
                            
                            trafolar["kumulatif_bos_kapasite"] = trafolar["efektif_kapasite"] - trafolar["GelenYukToplam"]
                            
                            trafolar["bos_kapasite"] -= trafolar["atama_miktari"]
                            
                            
                            trafolar["efektif_bos_kapasite"] -= trafolar["atama_miktari_ek"]                       
                            
                            trafolar.loc[(trafolar["kapasite"] != 0), "Nominal_Bos_yuzde"] = (
                                trafolar["bos_kapasite"] / trafolar["kapasite"]
                            )
                            
                            trafolar.loc[(trafolar["efektif_kapasite"] != 0), "Efektif_Bos_yuzde"] = (
                                trafolar["kumulatif_bos_kapasite"] / trafolar["efektif_kapasite"]
                            )
                            
                                            
                            # Yeni veriyi tek seferde DataFrame olarak oluştur
                            new_data = pd.DataFrame({
                                "year": yil,
                                "trafo_id": trafolar["trafo_id"],
                                "merkez_hucre": trafolar["merkez_hucre"],
                                "kalan_kapasite": trafolar["bos_kapasite"],
                                "kalan_kapasite_ek": trafolar["efektif_bos_kapasite"],
                                "atanan_miktar": trafolar["atama_miktari"],
                                "atanan_miktar_ek": trafolar["atama_miktari_ek"],
                                "yaş": trafolar["trafo_yasi"],
                                "eski kapasite": trafolar["eski kapasite"],
                                "yeni kapasite": trafolar["kapasite"],
                                "kapasite artirma yili": trafolar["Kapasite_artirma_yili"],
                                "Nominal_Bos_yuzde": trafolar["Nominal_Bos_yuzde"],
                                "Efektif_Bos_yuzde": trafolar["Efektif_Bos_yuzde"],
                                "GelenYukToplam": trafolar["GelenYukToplam"]
                            })
                            
                            trafolar["trafo_yasi"] += 1
                            
                            # Vektörel olarak birleştir (for döngüsüne gerek kalmadan)
                            df_yil_bazli_atama = pd.concat([df_yil_bazli_atama, new_data], ignore_index=True)
                            
                            for index, row in trafolar.iterrows():
                                dftrafocopy.loc[dftrafocopy.index == index, "bos_kapasite"] = row["bos_kapasite"]
                                dftrafocopy.loc[dftrafocopy.index == index, "efektif_bos_kapasite"] = row["efektif_bos_kapasite"]
                                dftrafocopy.loc[dftrafocopy.index == index, "trafo_yasi"] = row["trafo_yasi"]
                                dftrafocopy.loc[dftrafocopy.index == index, "GelenYukToplam"] = row["GelenYukToplam"]
                        
                            durum = "Atandı"               
                            
                        else:
                            
                            durum = "Atanmadı"
                            
                            Atanmadı_Cikti = True
                            
                        GelenYukToplam = trafolar["GelenYukToplam"].sum()
                    
                        toplam_boskapasite_ek = trafolar["efektif_bos_kapasite"].sum() 
                    
                        atama_sonucu_yeni.append([yil, hucreid, yuk_miktari, toplam_boskapasite_ek, GelenYukToplam, durum])
    
                            
                    else:
                        
                        durum = "Atanmadı"
                        
                        GelenYukToplam = trafolar["GelenYukToplam"].sum()
                    
                        toplam_boskapasite_ek = trafolar["efektif_bos_kapasite"].sum()
                        
                        atama_sonucu_yeni.append([yil, hucreid, yuk_miktari, toplam_boskapasite_ek, GelenYukToplam, durum])
    
            
        
        dftrafocopy["trafo_yasi"] = dftrafocopy["trafo_yas"].copy()
        
        self.trafo = dftrafocopy.copy()
    
        return dftrafocopy, pd.DataFrame(atama_sonucu_yeni, columns=["year", "ID", "power_distribution", "toplam_boskapasite", "GelenYukToplam", "durum"]), df_yil_bazli_atama   
        
    def TrafoOzet(self):
        
        odtr = config3.get()
        
        all_years = pd.Series(list(range(odtr["ilk_yil"]-1, odtr["son_yil"] +1)), name="year")
        
        # yeni eklenen        
        df = self.dfyeni.copy()

        df_year_counts = df.groupby("year").size().reset_index(name="yeni trafo tesis EA").astype(int)
        
        # all_years ile birleştirme
        df_full_year_Eklenen = all_years.to_frame().merge(df_year_counts, on="year", how="left").fillna(0)
        
           

        
        df_full_year_Eklenen["year"] = df_full_year_Eklenen["year"].astype(int)
        
        self.TrafoOzet = df_full_year_Eklenen.copy()
    
    def TrafoDurum(self):
        
        df = self.dftrafocopy4.copy()
        
        df.loc[df["Kapasite_artirma_yili"].notna(), "Durum"] = "Kapasite Artan"
        df.loc[df["Kapasite_artirma_yili"].notna(), "Trafo Durum"] = "Kapasite Artan"

        
        df.loc[df["trafo_id"].str.contains("New"), "Durum"] = "Eklenen"
        
        df2 = TrafoYenile(self.dftrafocopy3)  
        
        #trafoid = df2[(df2["yenileme_yili"].notna()) & (df2["sahip"]=="Kurum")]["trafo_id"]
        trafoid = df2[(df2["yenileme_yili"].notna()) & (df2["sahip"]=="Kurum")]["trafo_id"]
        trafoid2 = df[df["Kapasite_artirma_yili"].notna()]["trafo_id"]
        
        fark = set(trafoid) - set(trafoid2)
        
        df.loc[df["trafo_id"].isin(fark), "Durum"] = "Yenilenen"
        
        df.loc[df["trafo_id"].isin(fark), "Trafo Durum"] = "Yenilenen"
        
        df3 = df2[df2["trafo_id"].isin(fark)]
        
        df4 = df[df["trafo_id"].isin(trafoid2)]
        
        df.loc[df["trafo_id"].isin(fark),"year"] = df["trafo_id"].map(df3.set_index("trafo_id")["yenileme_yili"])
        
        df.loc[df["trafo_id"].isin(trafoid2), "year"] = df["trafo_id"].map(df4.set_index("trafo_id")["Kapasite_artirma_yili"])
        
        df.loc[df["Durum"].isna(), "Durum"] = "Mevcut"
        
        df.loc[df["Durum"].isna(), "Trafo Durum"] = "Mevcut"
        
        self.dftrafocopy4 = df.copy()
        
        self.dftrafocopy4["year"] = self.dftrafocopy4["year"].fillna(2025).astype(int)
        
    def cumsum_from_first_positive_and_reset_on_negative(self, series):
        result = pd.Series(0, index=series.index)
        first_pos_idx = series[series > 0].index.min()
    
        if pd.notna(first_pos_idx):
            cumsum = 0
            for idx in series.loc[first_pos_idx:].index:
                cumsum += series.loc[idx]
                if cumsum < 0:
                    cumsum = 0
                result.loc[idx] = cumsum
    
        return result
    
    def from_first_positive(self, group):
        # İlk pozitif değerin index'ini bul
        first_pos_idx = group[group["power_distribution"] > 0].first_valid_index()
        
        if first_pos_idx is not None:
            return group.loc[first_pos_idx:]
        else:
            return pd.DataFrame(columns=group.columns)  # pozitif değer yoksa boş döndür

    def TrafoEkle(self, atanamayanlar):    
        
        dfyeni_trafo = pd.DataFrame()
        dfyeni_trafoyıllık = pd.DataFrame()
        
        df = atanamayanlar.copy()
        # Her shid için toplam kümülatif yükü hesapla
        
        valid_shids = df.groupby("ID")["power_distribution"].sum()
        
        valid_shids = valid_shids[valid_shids > 0].index
        
        # 2. Bu shid’lere sahip satırları filtrele
        df = df[df["ID"].isin(valid_shids)].copy()
        
        df.sort_values(by=["ID", "year"], inplace=True)
        
        # 3. Her shid için kümülatif toplam (zaman içinde birikimli yük)
        df["kumulatif_yuk_cumsum"] = (
            df.groupby("ID")["power_distribution"]
              .transform(self.cumsum_from_first_positive_and_reset_on_negative)
        )
        # df["kumulatif_yuk_cumsum"] = df.groupby("ID")["power_distribution"].cumsum()
        
        # 4. Her shid için bu cumsum’un maksimumunu al
        shid_max_kumulatif = df.groupby("ID")["kumulatif_yuk_cumsum"].max().reset_index()
        
        df2 = df.groupby("ID", group_keys=False).apply(self.from_first_positive)
        # df2 = df[df["power_distribution"]>0].copy()
        
        # shid_total_yuk = df.groupby("ID")["power_distribution"].sum().reset_index()
        
        # Her shid için ilk görüldüğü yılı bul
        shid_min_year = df2.groupby("ID")["year"].min().reset_index()
        
        # Bu iki bilgiyi birleştirerek toplam yükü ilk yıla aktarma
        df_final = shid_min_year.merge(shid_max_kumulatif, on="ID")
        
        df_final2 = df_final.merge(df[["ID", "year", "toplam_boskapasite"]], on = ["ID", "year"], how="left")
        
        df_final2 = df_final2.sort_values(by="year", ascending=True).reset_index(drop=True)
    
        for index, row in df_final2.iterrows():
              
            ID = row["ID"]
            
            try:
                # Attempt to extract the value of 'shid'
                shid = ea.dfsuperhucre[ea.dfsuperhucre["hucreid"] == ID]["shid"].values
                
                # Check if 'shid' is not empty before accessing the first element
                if len(shid) > 0:
                    shid = shid[0]
                else:
                    shid = 0  # Or handle the case where no result is found
                
            except Exception as e:
                shid = 0  # In case of any error, set 'shid' to 0

            
            toplam_atanamayan_yuk = row["kumulatif_yuk_cumsum"]
            
            gerekli_kapasite = toplam_atanamayan_yuk
            
            yil = row["year"]  # **İlk atanamayan yıl**
    
            trafo_list = pd.DataFrame(columns=["sayac","hucre_id","kapasite", "efektif_kapasite"])
            
            break_outer_loop = False  # Dış döngüden çıkmayı kontrol eden değişken
            
            sayac = 0
            
            while not break_outer_loop:
                
                for kapasite in self.p["ozel_trafo_liste"]:
                    
                    efektif_miktar = kapasite * self.p["yeni_trafo_kapasite_kullanim_ust_limiti"]
                    
                    data = pd.DataFrame({
                        "sayac":[sayac],
                        "hucre_id": [ID],
                        "kapasite": [kapasite],
                        "efektif_kapasite": [efektif_miktar],
                        "Koord_x":None,
                        "Koord_y":None
                    })
            
                    # Filtrele: boş olmayanları al
                    valid_frames = [df for df in [trafo_list, data] if not df.empty and not df.isna().all().all()]
                    
                    # Sonra birleştir
                    trafo_list = pd.concat(valid_frames, ignore_index=True)
                                        
                    toplam_efektif = trafo_list["efektif_kapasite"].sum()
            
                    if toplam_efektif >= gerekli_kapasite:
                        
                        break_outer_loop = True  # Dış döngüden çıkılmasını tetikle
                        break  # İç döngüden çık
                        
                    if toplam_efektif < gerekli_kapasite and kapasite == self.p["ozel_trafo_liste"][-1]: 
                        
                        continue
                        
                    else:
                        
                        trafo_list = trafo_list.drop(trafo_list.index[-1])                   
                    
                sayac += 1
                
                if break_outer_loop:
                    break  # **Dış döngüden de çık**
            
            # yukler = df2[df2["ID"]==ID].copy()            
            
            # kapasite = 0
            
            # for idtrafo, trafo in trafo_list.iterrows():
                            
            #     ilkyil = yukler["year"].min()
                
            #     kapasite += trafo["efektif_kapasite"]
                
            #     tesis_yili = yukler["year"].min()
                
            #     for idx, row in yukler.iterrows():
                    
            #         yuk = row["power_distribution"]
                    
            #         yil = row["year"]                              
                    
            #         if yuk <= kapasite:
            #             kapasite -= yuk
                        
            #         else:
                        
            #             dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfhucre_super, ilkyil, int(tesis_yili), None, self.p, bolge="Kentsel", mulkiyet=0, aksiyon="yeni trafo tesis EA")
                        
            #             dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
            #             dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])  
                        
            #             mask = yukler["year"] >= yil
                        
            #             yukler = yukler[mask]
                        
            #             break
                    
            #         if idtrafo == len(trafo_list) - 1:
                        
            #             dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfhucre_super, ilkyil, int(tesis_yili), None, self.p, bolge="Kentsel", mulkiyet=0, aksiyon="yeni trafo tesis EA")
                        
            #             dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
            #             dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])  
                        
            #             break
                    
            yukler = df2[df2["ID"]==ID].copy()            
            ilkyil = yukler["year"].min()
            kapasite = 0
            
            for idtrafo, trafo in trafo_list.iterrows():
                kapasite += trafo["efektif_kapasite"]
                tesis_yili = yukler["year"].min()
            
                kalan_yukler = []
                break_occurred = False
            
                for idx, row in yukler.iterrows():
                    yuk = row["power_distribution"]
                    yil = row["year"]
            
                    if yuk <= kapasite:
                        kapasite -= yuk
                    else:
                        kalan_yukler = yukler[yukler["year"] >= yil].copy()
                        break_occurred = True
                        break  # bu yıldan sonrası yeniden değerlendirilecek
            
                # ✅ HER TRAFO KAYDEDİLİR (yuk karsılasa da karsılamasa da)
                dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfhucre_super, ilkyil, int(tesis_yili), None, self.p, bolge="Kentsel", mulkiyet=0, aksiyon="yeni trafo tesis EA")
                
                dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])  
            
                if break_occurred:
                    yukler = kalan_yukler
            
                    # Eğer son trafodaysan ve hâlâ yük varsa → aynı trafoyu tekrar ekle
                    if idtrafo == len(trafo_list) - 1:
                        # ekstra trafo daha
                        dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfhucre_super, ilkyil, int(tesis_yili), None, self.p, bolge="Kentsel", mulkiyet=0, aksiyon="yeni trafo tesis EA")
                        
                        dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                        dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])  
                else:
                    break  # tüm yük karşılandıysa çık

                                                           
        
        return dfyeni_trafo.reset_index(drop=True), dfyeni_trafoyıllık.reset_index(drop=True)
    
    
if __name__ == "__main__":

    input_file = r"C:\Users\vural.bayrakli\OneDrive - MRC\İletişim sitesi - MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\il_ilce_kırılımları\Program Dosyaları\Optimal DTR\ODTR.json"
    
    logging.info(f"{input_file} icin islem yapılıyor...")   
    
    odtr = config3.ODTR(input_file)
    
    odtr = config3.get()

    df = pd.read_excel(r"Katsayılar.xlsx", sheet_name="Sheet2", engine="openpyxl")  # Daha hızlı olması için openpyxl kullanılır
    p = config3.Start(df)
    
    df = SqliteOku(odtr["EAVeri_db_adi"])
    df = df[df["ilce"]==2].copy()

    dfhucre_super = ReadParquet(odtr["dfhucre_super"])
    
    #trafo = pd.read_excel(r"C:\Users\vural.bayrakli\Desktop\OneDrive_1_03.02.2025\Sonuçlar\DTR\Trafo0204.xlsx", engine="openpyxl")
    
    trafoPath = r"C:\Users\vural.bayrakli\OneDrive - MRC\İletişim sitesi - MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\il_ilce_kırılımları\İzmir\Karşıyaka\proje_karsıyaka_deneme\sonuclar\Optimal DTR Sonuçları\Optimal Trafo Sonuçlar20250505_110801.xlsx"
    trafo = pd.read_excel(trafoPath, engine="openpyxl")
    
    
    ea = EA(df, trafo, dfhucre_super, p)
    
    
    
    if False:
        
        dftrafocopy2, dfatama_sonuc, trafoRapor = ea.YukAtama(ea.df, ea.trafo)
        
        atanamayanlar = dfatama_sonuc[dfatama_sonuc["durum"] == "Atanmadı"]
        
        dfyeni = ea.TrafoEkle(atanamayanlar)
        
        trafoYeni = pd.concat([trafo, dfyeni], ignore_index=True)
        
        
        
        # Her iki DataFrame'in indekslerini sıfırlayalım
        dftrafocopy3 = dftrafocopy3.reset_index(drop=True)
        dfyeni = dfyeni.reset_index(drop=True)
        
        # concat işlemiyle iki DataFrame'i birleştir
        dftrafocopy4 = pd.concat([dftrafocopy3, dfyeni], ignore_index=True)
        
        dftrafocopy4, dfatama_sonuc3, trafoRapor3 = ka.YukAtama(atanamayanlar2, dftrafocopy4)
        
        atanamayanlar3 = dfatama_sonuc3[dfatama_sonuc3["durum"] == "Atanmadı"]
        
        self.AtanmadiIslem(self.atanamayanlar3)
        
        self.ExcelKaydet(self.atanamayanlar , "self.atanamayanlar") 
        
        self.ExcelKaydet(self.atanamayanlar2 , "self.atanamayanlar2") 
        
        self.ExcelKaydet(self.atanamayanlar3 , "self.atanamayanlar3") 
        
        self.ExcelKaydet(self.dfyeni, "test.dfyeni")
        
        self.TrafoDurum()
        
        self.TrafoOzet()
    
    
    
    
    
    
    
    
    
    
    
    
    