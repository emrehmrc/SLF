# -*- coding: utf-8 -*-
"""
Created on Mon Feb 24 09:38:46 2025

@author: vural.bayrakli
"""

import pandas as pd
import numpy as np
import logging
import sys
import datetime
import pickle
from datetime import datetime
#from config2 import *
import config3
import os
import ast
from concurrent.futures import ThreadPoolExecutor
import sqlite3
import warnings
warnings.filterwarnings("ignore")
import re
import random
import ast

logging.basicConfig(level=logging.DEBUG, format='%(asctime)s - %(levelname)s - %(message)s', stream=sys.stdout)


def YukAta4_3(df, dftrafocopy2, dftrafoYıllık):
    
    atama_sonucu_yeni = []
          
    if not df["power_distribution"].eq(0).all():
    
        Atanmadı_Cikti = False
        
        if not dftrafoYıllık.empty:
            
            for row in df.itertuples(index=False):
                
                yil = row.year
                shid = row.shid
                
                yuk_miktari = row.power_distribution
            
                superhucredeki_trafolar = dftrafoYıllık[                  
                    (dftrafoYıllık["year"] == yil)
                ].copy()
                                    
                if (superhucredeki_trafolar["efektif_bos_kapasite"] > 0).any():
                    superhucredeki_trafolar = superhucredeki_trafolar[superhucredeki_trafolar["efektif_bos_kapasite"] > 0]
                           
                toplam_boskapasite = superhucredeki_trafolar["bos_kapasite"].sum()  
                toplam_kapasite = superhucredeki_trafolar["kapasite"].sum()  
                toplam_boskapasite_ek = superhucredeki_trafolar["efektif_bos_kapasite"].sum() 
                toplam_kapasite_ek = superhucredeki_trafolar["efektif_kapasite"].sum() 
                
                
                superhucredeki_trafolar["atama_miktari"] = 0.0
                superhucredeki_trafolar["atama_miktari_ek"] = 0.0
                superhucredeki_trafolar["atama_orani"] = 0
                superhucredeki_trafolar["atama_orani_ek"] = 0
                superhucredeki_trafolar["NegatifFark"] = 0.0
    
                if not Atanmadı_Cikti:
                    
                    if ((toplam_boskapasite_ek > yuk_miktari and toplam_boskapasite_ek > 0) or yuk_miktari <= 0):
                        
                        if yuk_miktari >= 0:
                            # Pozitif yük → boş kapasiteye göre dağıt
                            superhucredeki_trafolar["atama_orani"] = np.where(
                                toplam_boskapasite != 0,
                                superhucredeki_trafolar["bos_kapasite"] / toplam_boskapasite,
                                0
                            )
                            
                            superhucredeki_trafolar["atama_orani_ek"] = np.where(
                                toplam_boskapasite_ek != 0,
                                superhucredeki_trafolar["efektif_bos_kapasite"] / toplam_boskapasite_ek,
                                0
                            )
                        else:
                            # Negatif yük → GelenYukToplam'a göre azalt
                            toplam_gelenyuk = superhucredeki_trafolar["GelenYukToplam"].sum()
                        
                            superhucredeki_trafolar["atama_orani"] = np.where(
                                toplam_gelenyuk != 0,
                                superhucredeki_trafolar["GelenYukToplam"] / toplam_gelenyuk,
                                0
                            )
                        
                            superhucredeki_trafolar["atama_orani_ek"] = superhucredeki_trafolar["atama_orani"]  # Aynı oranlar kullanılabilir

                                       
                        superhucredeki_trafolar["atama_miktari"] = yuk_miktari * superhucredeki_trafolar["atama_orani"]
                        superhucredeki_trafolar["atama_miktari_ek"] = yuk_miktari * superhucredeki_trafolar["atama_orani_ek"]
                        
                        # 1. Mevcut GelenYukToplam yedeğe alınır
                        superhucredeki_trafolar["GelenYukToplamGecici"] = superhucredeki_trafolar["GelenYukToplam"]
                        
                        # 2. atama_miktari_ek toplam yüke eklenir
                        superhucredeki_trafolar["GelenYukToplam"] += superhucredeki_trafolar["atama_miktari_ek"]                                                
                                                                                
                        # 3. Negatif gelen yük durumu için maske oluştur
                        negatif_mask = superhucredeki_trafolar["GelenYukToplam"] < 0
                        
                        superhucredeki_trafolar["NegatifFark"] = 0.0
                        superhucredeki_trafolar.loc[negatif_mask, "NegatifFark"] = (
                            superhucredeki_trafolar.loc[negatif_mask, "GelenYukToplamGecici"]
                            - superhucredeki_trafolar.loc[negatif_mask, "atama_miktari_ek"]
                        )
                        
                        # 4. atama_miktari ve atama_miktari_ek düzelt: negatif durumda önceki yükü geri al
                        superhucredeki_trafolar.loc[negatif_mask, "atama_miktari"] = (
                            -1 * superhucredeki_trafolar.loc[negatif_mask, "GelenYukToplamGecici"]
                        )
                        
                        superhucredeki_trafolar.loc[negatif_mask, "atama_miktari_ek"] = (
                            -1 * superhucredeki_trafolar.loc[negatif_mask, "GelenYukToplamGecici"]
                        )
                        
                        # 5. GelenYukToplam negatifse 0 yap
                        superhucredeki_trafolar.loc[negatif_mask, "GelenYukToplam"] = 0
                        
                        # 6. (İsteğe bağlı) NegatifFark kolonunu temizle veya açık şekilde belirt
                        superhucredeki_trafolar["NegatifFark"] = 0.0
                        abs(
                            yuk_miktari
                            + superhucredeki_trafolar.loc[negatif_mask, "GelenYukToplamGecici"]
                        )
                        
                        # GelenYukToplam'ı 0'dan küçükse 0 yap
                        superhucredeki_trafolar.loc[
                            superhucredeki_trafolar["GelenYukToplam"] < 0, "GelenYukToplam"
                        ] = 0
                                                    
                        superhucredeki_trafolar["bos_kapasite"] -= superhucredeki_trafolar["atama_miktari"]                   
                        
                        superhucredeki_trafolar["efektif_bos_kapasite"] -= superhucredeki_trafolar["atama_miktari_ek"]                                           
                        
                        superhucredeki_trafolar.loc[(superhucredeki_trafolar["kapasite"] != 0), "Doluluk Oranı"] = (
                            superhucredeki_trafolar["GelenYukToplam"] / superhucredeki_trafolar["kapasite"])
                        
                        superhucredeki_trafolar["Rapor"] = superhucredeki_trafolar.apply(
                            lambda row: (row["Rapor"] if pd.notna(row["Rapor"]) else "") + f"Süper Hücre {shid} den {row['atama_miktari_ek']} kVA yük geldi. \n", axis=1)                       

                        for row in superhucredeki_trafolar.itertuples(index=False):
                            
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"] += row.atama_miktari_ek
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] == yil+1)), "İlkGelenYukToplam"] = row.GelenYukToplam
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "bos_kapasite"] = row.bos_kapasite
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "efektif_bos_kapasite"] = row.efektif_bos_kapasite
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "kullanilan_kapasite_efektif"] = row.kullanilan_kapasite_efektif
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "Doluluk Oranı"] = row.GelenYukToplam / row.kapasite
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] == yil)), "Rapor"] = row.Rapor

                            
                            dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row.trafo_id, "bos_kapasite"] = row.bos_kapasite
                            dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row.trafo_id, "efektif_bos_kapasite"] = row.efektif_bos_kapasite
                            dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row.trafo_id, "kullanilan_kapasite_efektif"] = row.kullanilan_kapasite_efektif
                            dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row.trafo_id, "GelenYukToplam"] = row.GelenYukToplam
                            dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row.trafo_id, "Doluluk Oranı"] = row.GelenYukToplam / row.kapasite
                                      
    
                        durum = "Atandı"    
                                                                                                       
                    else:
                        
                        durum = "Atanmadı"
                        
                        Atanmadı_Cikti = True
                    
                else:
                    
                    durum = "Atanmadı"
                    
                GelenYukToplam = superhucredeki_trafolar["GelenYukToplam"].sum()
            
                toplam_boskapasite_ek = superhucredeki_trafolar["efektif_bos_kapasite"].sum()
                
                atanamayan_talep = superhucredeki_trafolar["NegatifFark"].sum()
            
                atama_sonucu_yeni.append([yil, shid, yuk_miktari, toplam_boskapasite_ek, GelenYukToplam, atanamayan_talep, durum])

        else:
            
            durum = "Atanmadı"

            if not df["power_distribution"].eq(0).all():
                
                # hepsi_negatif_mi = (df["power_distribution"] < 0).all() 
                
                # if hepsi_negatif_mi:
                #     durum = "Atandı"
                
                for row in df.itertuples(index=False):
                    atama_sonucu_yeni.append([row.year, row.shid, row.power_distribution, 0, 0, 0, durum])

    return dftrafoYıllık, dftrafocopy2, pd.DataFrame(atama_sonucu_yeni, columns=["year", "shid", "kumulatif_yuk", "toplam_boskapasite", "GelenYukToplam", "Atanamayan Talep", "durum"])

def YukAtaEA(df, dftrafocopy2, dftrafoYıllık):
    
    atama_sonucu_yeni = []
          
    if not df["power_distribution"].eq(0).all():
    
        Atanmadı_Cikti = False
        
        if not dftrafoYıllık.empty:
            
            for row in df.itertuples(index=False):
                
                yil = row.year
                ID = row.ID
                
                yuk_miktari = row.power_distribution
            
                superhucredeki_trafolar = dftrafoYıllık[                  
                    (dftrafoYıllık["year"] == yil)
                ].copy()
                                    
                if (superhucredeki_trafolar["efektif_bos_kapasite"] > 0).any():
                    superhucredeki_trafolar = superhucredeki_trafolar[superhucredeki_trafolar["efektif_bos_kapasite"] > 0]
                           
                toplam_boskapasite = superhucredeki_trafolar["bos_kapasite"].sum()  
                toplam_kapasite = superhucredeki_trafolar["kapasite"].sum()  
                toplam_boskapasite_ek = superhucredeki_trafolar["efektif_bos_kapasite"].sum() 
                toplam_kapasite_ek = superhucredeki_trafolar["efektif_kapasite"].sum() 
                
                
                superhucredeki_trafolar["atama_miktari"] = 0.0
                superhucredeki_trafolar["atama_miktari_ek"] = 0.0
                superhucredeki_trafolar["atama_orani"] = 0
                superhucredeki_trafolar["atama_orani_ek"] = 0
                superhucredeki_trafolar["NegatifFark"] = 0.0
    
                if not Atanmadı_Cikti:
                    
                    if ((toplam_boskapasite_ek > yuk_miktari and toplam_boskapasite_ek > 0) or yuk_miktari <= 0):
                        
                        if yuk_miktari >= 0:
                            # Pozitif yük → boş kapasiteye göre dağıt
                            superhucredeki_trafolar["atama_orani"] = np.where(
                                toplam_boskapasite != 0,
                                superhucredeki_trafolar["bos_kapasite"] / toplam_boskapasite,
                                0
                            )
                            
                            superhucredeki_trafolar["atama_orani_ek"] = np.where(
                                toplam_boskapasite_ek != 0,
                                superhucredeki_trafolar["efektif_bos_kapasite"] / toplam_boskapasite_ek,
                                0
                            )
                        else:
                            # Negatif yük → GelenYukToplam'a göre azalt
                            toplam_gelenyuk = superhucredeki_trafolar["GelenYukToplam"].sum()
                        
                            superhucredeki_trafolar["atama_orani"] = np.where(
                                toplam_gelenyuk != 0,
                                superhucredeki_trafolar["GelenYukToplam"] / toplam_gelenyuk,
                                0
                            )
                        
                            superhucredeki_trafolar["atama_orani_ek"] = superhucredeki_trafolar["atama_orani"]  # Aynı oranlar kullanılabilir

                                       
                        superhucredeki_trafolar["atama_miktari"] = yuk_miktari * superhucredeki_trafolar["atama_orani"]
                        superhucredeki_trafolar["atama_miktari_ek"] = yuk_miktari * superhucredeki_trafolar["atama_orani_ek"]
                        
                        # 1. Mevcut GelenYukToplam yedeğe alınır
                        superhucredeki_trafolar["GelenYukToplamGecici"] = superhucredeki_trafolar["GelenYukToplam"]
                        
                        # 2. atama_miktari_ek toplam yüke eklenir
                        superhucredeki_trafolar["GelenYukToplam"] += superhucredeki_trafolar["atama_miktari_ek"]
                        
                        # 3. Negatif gelen yük durumu için maske oluştur
                        negatif_mask = superhucredeki_trafolar["GelenYukToplam"] < 0
                        
                        # 4. atama_miktari ve atama_miktari_ek düzelt: negatif durumda önceki yükü geri al
                        superhucredeki_trafolar.loc[negatif_mask, "atama_miktari"] = (
                            -1 * superhucredeki_trafolar.loc[negatif_mask, "GelenYukToplamGecici"]
                        )
                        
                        superhucredeki_trafolar.loc[negatif_mask, "atama_miktari_ek"] = (
                            -1 * superhucredeki_trafolar.loc[negatif_mask, "GelenYukToplamGecici"]
                        )
                        
                        # 5. GelenYukToplam negatifse 0 yap
                        superhucredeki_trafolar.loc[negatif_mask, "GelenYukToplam"] = 0
                        
                        # 6. (İsteğe bağlı) NegatifFark kolonunu temizle veya açık şekilde belirt
                        superhucredeki_trafolar["NegatifFark"] = 0.0
                        superhucredeki_trafolar.loc[negatif_mask, "NegatifFark"] = (
                            superhucredeki_trafolar.loc[negatif_mask, "GelenYukToplamGecici"]
                        )
                        
                        # GelenYukToplam'ı 0'dan küçükse 0 yap
                        superhucredeki_trafolar.loc[
                            superhucredeki_trafolar["GelenYukToplam"] < 0, "GelenYukToplam"
                        ] = 0
                                                    
                        superhucredeki_trafolar["bos_kapasite"] -= superhucredeki_trafolar["atama_miktari"]                   
                        
                        superhucredeki_trafolar["efektif_bos_kapasite"] -= superhucredeki_trafolar["atama_miktari_ek"]                                           
                        
                        superhucredeki_trafolar.loc[(superhucredeki_trafolar["kapasite"] != 0), "Doluluk Oranı"] = (
                            superhucredeki_trafolar["GelenYukToplam"] / superhucredeki_trafolar["kapasite"])
                        
                        superhucredeki_trafolar["Rapor"] = superhucredeki_trafolar.apply(
                            lambda row: (row["Rapor"] if pd.notna(row["Rapor"]) else "") + f"Süper Hücre {ID} den {row['atama_miktari_ek']} kVA yük geldi\n", axis=1)
                        
                        for row in superhucredeki_trafolar.itertuples(index=False):
                            
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"] += row.atama_miktari_ek
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] == yil+1)), "İlkGelenYukToplam"] = row.GelenYukToplam
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "bos_kapasite"] = row.bos_kapasite
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "efektif_bos_kapasite"] = row.efektif_bos_kapasite
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "kullanilan_kapasite_efektif"] = row.kullanilan_kapasite_efektif
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "Doluluk Oranı"] = row.GelenYukToplam / row.kapasite
                            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] == yil)), "Rapor"] = row.Rapor

                            
                            dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row.trafo_id, "bos_kapasite"] = row.bos_kapasite
                            dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row.trafo_id, "efektif_bos_kapasite"] = row.efektif_bos_kapasite
                            dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row.trafo_id, "kullanilan_kapasite_efektif"] = row.kullanilan_kapasite_efektif
                            dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row.trafo_id, "GelenYukToplam"] = row.GelenYukToplam
                            dftrafocopy2.loc[dftrafocopy2["trafo_id"] == row.trafo_id, "Doluluk Oranı"] = row.GelenYukToplam / row.kapasite
                                      
    
                        durum = "Atandı"    
                                                                                                       
                    else:
                        
                        durum = "Atanmadı"
                        
                        Atanmadı_Cikti = True
                    
                else:
                    
                    durum = "Atanmadı"
                    
                GelenYukToplam = superhucredeki_trafolar["GelenYukToplam"].sum()
            
                toplam_boskapasite_ek = superhucredeki_trafolar["efektif_bos_kapasite"].sum()
                
                atanamayan_talep = superhucredeki_trafolar["NegatifFark"].sum()
            
                atama_sonucu_yeni.append([yil, ID, yuk_miktari, toplam_boskapasite_ek, GelenYukToplam, atanamayan_talep, durum])

        else:
            
            durum = "Atanmadı"

            if not df["power_distribution"].eq(0).all():
                
                # hepsi_negatif_mi = (df["power_distribution"] < 0).all() 
                
                # if hepsi_negatif_mi:
                #     durum = "Atandı"
                
                for row in df.itertuples(index=False):
                    atama_sonucu_yeni.append([row.year, row.ID, row.power_distribution, 0, 0, 0, durum])

    return dftrafoYıllık, dftrafocopy2, pd.DataFrame(atama_sonucu_yeni, columns=["year", "ID", "kumulatif_yuk", "toplam_boskapasite", "GelenYukToplam", "Atanamayan Talep", "durum"])

def TrafoKapasiteArtirGuncel2(p, atanamayanlar, dftrafocopy, dftrafoYıllık, dfhucre_super):
    # İlk atanamayan yılı bulan index'leri al
    odtr = config3.get()
    dftrafoYıllık["Kapasite_artirma_yili"] = np.nan
    dftrafocopy["Kapasite_artirma_yili"] = np.nan

    dftrafo = dftrafoYıllık.copy()
    
    # İlgili Süper Hücredeki uygun trafoları seç
    mask = dftrafocopy[(dftrafocopy["Trafo Aksiyon"].isin(["deplase", "gerilim dönüşümü", "güç artırımı"]))]["trafo_id"].values
    
    dftrafo = dftrafo[~dftrafo["trafo_id"].isin(mask)]
    
    df = atanamayanlar.copy()
    
    for shid, df_shid in df.groupby("shid"):
        
        ilk_yil = odtr["ilk_yil"]
        
        islem_gerceklesti = False    
        
        df_shid = df_shid.copy()
                       
        df_shid = df_shid.sort_values(by="year", ascending=True).reset_index(drop=True)
        
        df_shid_ = df_shid[df_shid["kumulatif_yuk"]>0].copy()
        
        atanamayan_yuk = df_shid_["kumulatif_yuk"].sum()
        
        kapasite_yeterlimi = False
        
        for index, row in df_shid.iterrows():
                
            if islem_gerceklesti:
                break
            
            yil = row["year"] 
            
            trafolar = dftrafo[
                (dftrafo["merkez_hucre"].isin(dfhucre_super[dfhucre_super["shid"] == shid]["hucreid"])) &
                (dftrafo["year"] == yil)
            ].copy()
            
            
            uygun_trafolar = trafolar[
                (trafolar["trafo_yasi"] > 15) & 
                (trafolar["kapasite"] < max(p["kurum_trafo_kentsel_alan_liste"]))
            ].copy()
            

            if not uygun_trafolar.empty:
                                                                                                                 
                uygun_trafolar["doluluk"] = uygun_trafolar["GelenYukToplam"] / uygun_trafolar["kapasite"]
                
                uygun_trafolar = uygun_trafolar.sort_values(by="doluluk", ascending=False)  # Trafoları küçükten büyüğe sırala

                toplam_mevcut_kapasite = uygun_trafolar["efektif_bos_kapasite"].sum()

                mevcut_efektif_kapasite = toplam_mevcut_kapasite.copy()
                
                if mevcut_efektif_kapasite < 0:
                    
                    mevcut_efektif_kapasite = 0
                           
                secilen_trafolar = []  # Kapasite artırımı yapılacak trafolar
                
                uygun_trafolar["efektif_bos_kapasite_gecici"] = uygun_trafolar["efektif_bos_kapasite"].copy()
                uygun_trafolar["efektif_bos_kapasite_gecici"] = uygun_trafolar["efektif_bos_kapasite_gecici"].fillna(0)
                
                #uygun_trafolar = uygun_trafolar.sort_values(by="efektif_bos_kapasite", ascending=True)
                
                for idx, trafo in uygun_trafolar.iterrows():
                    
                    # 630, 1000, 1250 kapasiteleri arasından en uygununu seç
                    for kapasite_artis in p["kurum_trafo_kentsel_alan_liste"]:
                        
                        if trafo["kapasite"] < kapasite_artis:
                            
                            efektif_miktar = kapasite_artis * p["mevcut_trafo_kapasite_kullanim_ust_limiti"]
                            
                            efektif_bos_miktar = efektif_miktar - uygun_trafolar.loc[idx, "GelenYukToplam"]
                            
                            uygun_trafolar.loc[idx, "efektif_bos_kapasite_gecici"] = efektif_bos_miktar
                            
                            toplam_efektif_kapasite = uygun_trafolar["efektif_bos_kapasite_gecici"].sum()                                                      
                            
                            if toplam_efektif_kapasite >= atanamayan_yuk:
                                                               
                                # dftrafoYıllık.loc[idx, "kapasite"] = kapasite_artis
                                # dftrafoYıllık.loc[idx, "efektif_kapasite"] = kapasite_artis * 0.7
                                # dftrafoYıllık.loc[idx, "kapasite_arttirildi_mi"] = "Evet"
                                # dftrafoYıllık.loc[idx, "Kapasite_artirma_yili"] = yil
                                # dftrafoYıllık.loc[idx, "İşlem Tarihi"] = yil
                                # dftrafoYıllık.loc[idx, "Trafo Aksiyon"] = "trafo yükseltme-yükten"
                                # dftrafoYıllık.loc[idx, "efektif_bos_kapasite"] = dftrafoYıllık.loc[idx, "efektif_kapasite"] - dftrafoYıllık.loc[idx, "GelenYukToplam"]
                                # dftrafoYıllık.loc[idx, "bos_kapasite"] = dftrafoYıllık.loc[idx, "kapasite"] - dftrafoYıllık.loc[idx, "GelenYukToplam"]
                                
                                
                                
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] == yil)), "Trafo Aksiyon"] = "trafo yükseltme-yükten"
                                # Doğru koşul yazımı
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "kapasite"] = kapasite_artis
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "efektif_kapasite"] = kapasite_artis * 0.7
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] == yil)), "kapasite_arttirildi_mi"] = "Evet"
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] == yil)), "Kapasite_artirma_yili"] = yil
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] == yil)), "İşlem Tarihi"] = yil
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "efektif_bos_kapasite"] = dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "efektif_kapasite"] - dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"]
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "bos_kapasite"] =  dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "kapasite"] - dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"]
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "Doluluk oranı"] =  dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"] / dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "kapasite"]
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "Gerilim"] = 34.5



                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "kapasite"] = kapasite_artis
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "efektif_kapasite"] = kapasite_artis * 0.7
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "kapasite_arttirildi_mi"] = "Evet"
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "Kapasite_artirma_yili"] = yil
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "İşlem Tarihi"] = yil
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "Gerilim"] = 34.5
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "Trafo Aksiyon"] = "trafo yükseltme-yükten"
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "efektif_bos_kapasite"] = dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "efektif_kapasite"] - dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "GelenYukToplam"]
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "bos_kapasite"] = dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "kapasite"] - dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "GelenYukToplam"]
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "Doluluk Oranı"] = dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "GelenYukToplam"] / dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "kapasite"]


                                kapasite_yeterlimi = True
                                
                                islem_gerceklesti = True    
                                
                                break  # Yeterli kapasiteye ulaşıldığında dur
                                
                            if (toplam_efektif_kapasite < atanamayan_yuk) and (kapasite_artis == p["kurum_trafo_kentsel_alan_liste"][-1]):
                                
                                # dftrafoYıllık.loc[idx, "kapasite"] = kapasite_artis
                                # dftrafoYıllık.loc[idx, "efektif_kapasite"] = kapasite_artis * 0.7
                                # dftrafoYıllık.loc[idx, "kapasite_arttirildi_mi"] = "Evet"
                                # dftrafoYıllık.loc[idx, "Kapasite_artirma_yili"] = yil
                                # dftrafoYıllık.loc[idx, "İşlem Tarihi"] = yil
                                # dftrafoYıllık.loc[idx, "Trafo Aksiyon"] = "trafo yükseltme-yükten"
                                # dftrafoYıllık.loc[idx, "efektif_bos_kapasite"] = dftrafoYıllık.loc[idx, "efektif_kapasite"] - dftrafoYıllık.loc[idx, "GelenYukToplam"]
                                # dftrafoYıllık.loc[idx, "bos_kapasite"] = dftrafoYıllık.loc[idx, "kapasite"] - dftrafoYıllık.loc[idx, "GelenYukToplam"]
                                
                                
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] == yil)), "Trafo Aksiyon"] = "trafo yükseltme-yükten"
                                # Doğru koşul yazımı
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "kapasite"] = kapasite_artis
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "efektif_kapasite"] = kapasite_artis * 0.7
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] == yil)), "kapasite_arttirildi_mi"] = "Evet"
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] == yil)), "Kapasite_artirma_yili"] = yil
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] == yil)), "İşlem Tarihi"] = yil
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "efektif_bos_kapasite"] = dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "efektif_kapasite"] - dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"]
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "bos_kapasite"] =  dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "kapasite"] - dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"]                                
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "Doluluk oranı"] =  dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"] / dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "kapasite"]
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "Gerilim"] = 34.5
                            
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "kapasite"] = kapasite_artis
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "efektif_kapasite"] = kapasite_artis * 0.7
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "kapasite_arttirildi_mi"] = "Evet"
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "Kapasite_artirma_yili"] = yil
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "İşlem Tarihi"] = yil
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "Trafo Aksiyon"] = "trafo yükseltme-yükten"                              
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "efektif_bos_kapasite"] = dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "efektif_kapasite"] - dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "GelenYukToplam"]
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "bos_kapasite"] = dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "kapasite"] - dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "GelenYukToplam"]
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "Doluluk Oranı"] = dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "GelenYukToplam"] / dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "kapasite"]
                                dftrafocopy.loc[dftrafocopy["trafo_id"] == trafo["trafo_id"], "Gerilim"] = 34.5

                                                                                                                                                    
                                atanamayan_yuk -= toplam_efektif_kapasite
                                    
                            else: 
                                
                                uygun_trafolar.loc[idx, "efektif_bos_kapasite_gecici"] = uygun_trafolar.loc[idx, "efektif_bos_kapasite"]
                                
                            
                    if kapasite_yeterlimi:
                        break  # Atanamayan yükü karşılamak için yeterli trafo seçildi                                                 
                            
    dftrafocopy["trafo_yasi"] = dftrafocopy["trafo_yas"].copy()
    
    return dftrafocopy

def TrafoKapasiteArtirGuncel3(p, dftrafocopy, dftrafoYıllık, dfhucre_super):
    
    odtr = config3.get()
    # İlk atanamayan yılı bulan index'leri al
    
    dftrafocopy["trafo_yasi"] = dftrafocopy["trafo_yas"].copy()
    dftrafocopy["Kapasite_Yenilendimi"] = ""
    dftrafocopy["Kapasite_yenilenme_yili"] = np.nan
    dftrafocopy["efektif_kapasite_gecici"] = np.nan
    dftrafocopy["kapasite_gecici"] = np.nan

    
    dftrafoYıllık["Kapasite_Yenilendimi"] = ""
    dftrafoYıllık["Kapasite_yenilenme_yili"] = np.nan
    
    mask = ~(dftrafocopy["Trafo Aksiyon"].isin(["deplase", "gerilim dönüşümü", "güç artırımı"]))
    
    df = dftrafocopy[mask].copy()
    
    df = df[(df["kapasite_arttirildi_mi"] == "Hayır") & (df["efektif_bos_kapasite"] < 0)]
        
    #df = df[(df["kapasite_arttirildi_mi"] == "Hayır")]
    
    for idx, trafo in df.iterrows():
                
        kapasite_yeterlimi = False
        
        for i in range(odtr["ilk_yil"], odtr["son_yil"] + 1):
            
            yil = i
            
            if kapasite_yeterlimi:
                
                break
            
            yas = trafo["trafo_yasi"]
            
            if yas > 15:
                # 630, 1000, 1250 kapasiteleri arasından en uygununu seç
                for kapasite_artis in p["kurum_trafo_kentsel_alan_liste"]:
                    
                    if trafo["kapasite"] < kapasite_artis:
                        
                        efektif_miktar = kapasite_artis * p["mevcut_trafo_kapasite_kullanim_ust_limiti"]
                        
                        efektif_bos_miktar = efektif_miktar - trafo["GelenYukToplam"]  
                        doluluk = trafo["GelenYukToplam"] / kapasite_artis
                        
                        # if (efektif_bos_miktar >= 0) or ((efektif_bos_miktar < 0) and (kapasite_artis == p["kurum_trafo_kentsel_alan_liste"][-1])):
                        if (doluluk < 0.7) or ((kapasite_artis == p["kurum_trafo_kentsel_alan_liste"][-1])):

                            dftrafocopy.loc[idx, "kapasite_gecici"] = kapasite_artis
                            dftrafocopy.loc[idx, "efektif_kapasite_gecici"] = kapasite_artis * 0.7
                            dftrafocopy.loc[idx, "Kapasite_Yenilendimi"] = "Evet"
                            #dftrafocopy.loc[idx, "Kapasite_yenilenme_yili"] = yil 
                            #dftrafocopy.loc[idx, "İşlem Tarihi"] = yil                
                                                                                                          
                            #dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] == yil)), "Trafo Aksiyon"] = "trafo yükseltme-kapasiteden"
                            #dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "kapasite"] = kapasite_artis
                            #dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "efektif_kapasite"] = kapasite_artis * 0.7
                            #dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "Kapasite_Yenilendimi"] = "Evet"
                            #dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "efektif_bos_kapasite"] = dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "efektif_kapasite"] - dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"]
                            #dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "bos_kapasite"] =  dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "kapasite"] - dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == trafo["trafo_id"]) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"]                                
                                                                                                                                          
                            kapasite_yeterlimi = True
                            
                            break  # Yeterli kapasiteye ulaşıldığında dur
                           
            trafo["trafo_yasi"] += 1                
            
    dftrafocopy["trafo_yasi"] = dftrafocopy["trafo_yas"].copy()
        
    # 1. Kapasite artırımı gereken trafoları filtreleyin
    df_kapasite_artis = dftrafocopy[(dftrafocopy["Kapasite_Yenilendimi"] == "Evet")].copy()
    
    df_kapasite_artis["doluluk_orani"] = df_kapasite_artis["GelenYukToplam"]/df_kapasite_artis["eski kapasite"]
    
    # KRİTER = 0.2 * (yaş - 15)  / 15    +  0.8 * (kapasite kullanımı -70 ) / 20
    
    for i in range(odtr["ilk_yil"],odtr["son_yil"] + 1):
        
        df_kapasite_artis[f"kriter{i}"] = 0.2 * (df_kapasite_artis["trafo_yasi"] - 15) / 15 + 0.8 * (df_kapasite_artis["doluluk_orani"] - 0.7) / 0.2
        
        #df_kapasite_artis[f"{i}_Yaş"] = df_kapasite_artis["trafo_yasi"]
        
        mask = df_kapasite_artis["trafo_yasi"] < 15
        
        mask2 = df_kapasite_artis["trafo_yasi"] > 30
        
        df_kapasite_artis.loc[mask, f"kriter{i}"] = -9
        
        df_kapasite_artis.loc[mask2, f"kriter{i}"] = 9
        
        df_kapasite_artis["trafo_yasi"] += 1
    
    df_kapasite_artisCopy = df_kapasite_artis.copy()
    # 3. Trafoları yıllara eşit şekilde dağıtın
    baslangic_yili = odtr["ilk_yil"]
    bitis_yili = odtr["son_yil"]
    toplam_yil = bitis_yili - baslangic_yili + 1
    
    uzunluk = len(df_kapasite_artis) // toplam_yil + 1
    
    value = uzunluk * (bitis_yili - baslangic_yili)
    
    for i in range(odtr["ilk_yil"],odtr["son_yil"] + 1):
        
        #mask = df_kapasite_artis[f"kriter{i}"].sort_values(ascending=False)[:uzunluk]
        mask = df_kapasite_artis[df_kapasite_artis[f"kriter{i}"] > 0] \
            .sort_values(by=f"kriter{i}", ascending=False) \
            .head(uzunluk)
        
        for idx, row in mask.iterrows():
            dftrafoYıllık.loc[
                (dftrafoYıllık["trafo_id"] == row['trafo_id']) & 
                (dftrafoYıllık["year"] == i),
                "Kapasite_yenilenme_yili"
            ] = i
            
            dftrafoYıllık.loc[
                (dftrafoYıllık["trafo_id"] == row['trafo_id']) & 
                (dftrafoYıllık["year"] == i),
                "İşlem Tarihi"
            ] = i
            
            dftrafoYıllık.loc[
                (dftrafoYıllık["trafo_id"] == row['trafo_id']) & 
                (dftrafoYıllık["year"] == i),
                "Trafo Aksiyon"
            ] = "trafo yükseltme-kapasiteden"                        
            
            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row["trafo_id"]) & (dftrafoYıllık["year"] >= i)), "Gerilim"] = 34.5
            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row['trafo_id']) & (dftrafoYıllık["year"] >= i)), "kapasite"] = row["kapasite_gecici"]
            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row['trafo_id']) & (dftrafoYıllık["year"] >= i)), "efektif_kapasite"] = row["kapasite_gecici"] * 0.7
            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row['trafo_id']) & (dftrafoYıllık["year"] == i)), "Kapasite_Yenilendimi"] = "Evet"
            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row['trafo_id']) & (dftrafoYıllık["year"] >= i)), "efektif_bos_kapasite"] = dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row['trafo_id']) & (dftrafoYıllık["year"] >= i)), "efektif_kapasite"] - dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row['trafo_id']) & (dftrafoYıllık["year"] >= i)), "GelenYukToplam"]
            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row['trafo_id']) & (dftrafoYıllık["year"] >= i)), "bos_kapasite"] =  dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row['trafo_id']) & (dftrafoYıllık["year"] >= i)), "kapasite"] - dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row['trafo_id']) & (dftrafoYıllık["year"] >= i)), "GelenYukToplam"]                                
            dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row['trafo_id']) & (dftrafoYıllık["year"] >= i)), "Doluluk Oranı"] =  dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row["trafo_id"]) & (dftrafoYıllık["year"] >= i)), "GelenYukToplam"] / dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row["trafo_id"]) & (dftrafoYıllık["year"] >= i)), "kapasite"]

            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "Kapasite_yenilenme_yili"] = i
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "kapasite"] = row["kapasite_gecici"]
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "efektif_kapasite"] = row["kapasite_gecici"] * 0.7
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "İşlem Tarihi"] = i
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "Gerilim"] = 34.5
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "efektif_bos_kapasite"] = dftrafocopy.loc[mask.index, "efektif_kapasite_gecici"] - dftrafocopy.loc[mask.index, "GelenYukToplam"]
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "bos_kapasite"] = dftrafocopy.loc[mask.index, "kapasite_gecici"] - dftrafocopy.loc[mask.index, "GelenYukToplam"]
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "Trafo Aksiyon"] = "trafo yükseltme-kapasiteden"
            dftrafocopy.loc[dftrafocopy["trafo_id"] == row['trafo_id'], "Doluluk Oranı"] = dftrafocopy.loc[dftrafocopy["trafo_id"] == row['trafo_id'], "GelenYukToplam"] / dftrafocopy.loc[dftrafocopy["trafo_id"] == row['trafo_id'], "kapasite"]

        df_kapasite_artis = df_kapasite_artis.loc[~df_kapasite_artis.index.isin(mask.index)]
            
    return dftrafocopy, dftrafoYıllık

def cumsum_from_first_positive_and_reset_on_negative(series):
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

def from_first_positive(group):
    # İlk pozitif değerin index'ini bul
    first_pos_idx = group[group["kumulatif_yuk"] > 0].first_valid_index()
    
    if first_pos_idx is not None:
        return group.loc[first_pos_idx:]
    else:
        return pd.DataFrame(columns=group.columns)  # pozitif değer yoksa boş döndür


def TrafoEkle(p, dftrafo, df_binary, atanamayanlar, dfhucre_super, ilk = True):    
    yeni_trafo_list = []
    yeni_trafo_listyıllık = []
    
    dfyeni_trafo = pd.DataFrame()
    dfyeni_trafoyıllık = pd.DataFrame()
    
    df = atanamayanlar.copy()
    # Her shid için toplam kümülatif yükü hesapla
    
    # df = df[df["kumulatif_yuk"]>0].copy()
    
    # shid_total_yuk = df.groupby("shid")["kumulatif_yuk"].sum().reset_index()
    valid_shids = df.groupby("shid")["kumulatif_yuk"].sum()
    
    valid_shids = valid_shids[valid_shids > 0].index
    
    # 2. Bu shid’lere sahip satırları filtrele
    df = df[df["shid"].isin(valid_shids)].copy()

    # 2. Yıla göre sırala
    df.sort_values(by=["shid", "year"], inplace=True)
    
    # 3. Her shid için kümülatif toplam (zaman içinde birikimli yük)
    # df["kumulatif_yuk_cumsum"] = df.groupby("shid")["kumulatif_yuk"].cumsum()
    df["kumulatif_yuk_cumsum"] = (
        df.groupby("shid")["kumulatif_yuk"]
          .transform(cumsum_from_first_positive_and_reset_on_negative)
    )
    
    
    # 4. Her shid için bu cumsum’un maksimumunu al
    shid_max_kumulatif = df.groupby("shid")["kumulatif_yuk_cumsum"].max().reset_index()
    
    # Her shid için ilk görüldüğü yılı bul
    df2 = df.groupby("shid", group_keys=False).apply(from_first_positive)
    # df2 = df[df["kumulatif_yuk"]>0].copy()
    
    shid_min_year = df2.groupby("shid")["year"].min().reset_index()
    
    # Bu iki bilgiyi birleştirerek toplam yükü ilk yıla aktarma
    # df_final = shid_min_year.merge(shid_total_yuk, on="shid")
    
    df_final = shid_min_year.merge(shid_max_kumulatif, on="shid")
    
    df_final2 = df_final.merge(df[["shid", "year", "toplam_boskapasite"]], on = ["shid", "year"], how="left")
    
    df_final2 = df_final2.sort_values(by="year", ascending=True).reset_index(drop=True)

    for index, row in df_final2.iterrows():
        
        shid = row["shid"]
        
        toplam_atanamayan_yuk = row["kumulatif_yuk_cumsum"]
        
        # gerekli_kapasite = toplam_atanamayan_yuk.copy()
        gerekli_kapasite = toplam_atanamayan_yuk
        
        yil = row["year"]  # **İlk atanamayan yıl**
        
        columns_to_check = ['teknik_altyapi','rezerve_trafo', 'park', 'belediye_hizmet', 'otopark',
                        'cami', 'pazar_alani', 'cocuk_parki'
                        ]

        df_filtered = df_binary[df_binary[columns_to_check].ne(0).any(axis=1)]
        
        if ilk:
            # **5. Süper Hücre İçindeki Hücreleri Seç**
            uygun_hucreler = df_filtered[
                (df_filtered["hucre_id"].isin(dfhucre_super[dfhucre_super["shid"] == shid]["hucreid"]))
                
            ]
            
            
            if not uygun_hucreler.empty:
                
                # NaN değerlerini 0 ile dolduruyoruz (NaN'ları sıfır yapıyoruz)
                df_filtered[columns_to_check] = df_filtered[columns_to_check].fillna(0)
    
                columns_to_check = ['teknik_altyapi','rezerve_trafo', 'park',
                                'cami', 'pazar_alani', 'otopark', 
                                'belediye_hizmet', 'cocuk_parki']
                
                        # Öncelik sütunlarını oluşturuyoruz: '1' olanları öncelikli yapmak için
                for column in columns_to_check:
                    if column in df_filtered.columns:
                        uygun_hucreler[f'{column}_priority'] = (df_filtered[column] == 1).astype(int)
                    else:
                        uygun_hucreler[f'{column}_priority'] = 0
            
                # Bu öncelik sütunlarına göre sıralama yapıyoruz
                sort_columns = [f'{column}_priority' for column in columns_to_check]
                
                uygun_hucreler_sorted = uygun_hucreler.sort_values(by=sort_columns, ascending=False)
            
                # Geçici öncelik sütunlarını kaldırıyoruz
                uygun_hucreler = uygun_hucreler_sorted.drop(columns=[f'{column}_priority' for column in columns_to_check])
                
                try:
                    
                    alanKoordinat = SqliteOku("alanlar_df")
                    
                    uygun_hucreler = uygun_hucreler.merge(alanKoordinat, on="hucre_id", how="left")
                    
                except Exception as e:
                    pass
                                   
                durum = "yeni trafo tesis"
            
        else:
            
            uygun_hucreler = df_binary[
                (df_binary["hucre_id"].isin(dfhucre_super[dfhucre_super["shid"] == shid]["hucreid"]))   
            ]
            
            uygun_hucreler = uygun_hucreler[~uygun_hucreler["hucre_id"].isin(dftrafo["merkez_hucre"])]
            
            durum = "yeni trafo tesis-hücresi belirsiz"


        trafo_list = pd.DataFrame(columns=["hucre_id", "kapasite", "efektif_kapasite", "Koord_x", "Koord_y"])
        
        break_outer_loop = False  # Dış döngüden çıkmayı kontrol eden değişken
        
        
        if not uygun_hucreler.empty:
            
            for idx, hucre in uygun_hucreler.iterrows():
                    
                    
                for kapasite in p["kurum_trafo_kentsel_alan_liste"]:
                    
                    efektif_miktar = kapasite * p["yeni_trafo_kapasite_kullanim_ust_limiti"]
                    
                    # 1. Stringi listeye çevir
                    
                    x, y = None, None
                    
                    alan_tipi = None 
                    
                    try:
                        if "centroid" in uygun_hucreler.columns:
                            if hucre["centroid"] != "":  # Boş değerleri kontrol ediyoruz
                                # literal_eval sadece geçerli bir formatta olan veriyi işleyebilir
                                koordinat = ast.literal_eval(hucre["centroid"])
                                x, y = koordinat[1], koordinat[0]
                            else:
                                # Eğer centroid boşsa, bir şey yapmıyoruz
                                x, y = None, None
                                
     
                    except (ValueError, SyntaxError):
                        # Eğer literal_eval hatası alırsak, veriyi geçiyoruz
                    
                        x, y = None, None
                        
                        
                    try:                   
                                
                        if "alan_tipi" in uygun_hucreler.columns:
                            if hucre["alan_tipi"] != "":  # Boş değerleri kontrol ediyoruz
                                
                                alan_tipi = hucre["alan_tipi"]
                                
     
                    except (ValueError, SyntaxError):
                        
                        alan_tipi = None
                        
                        
                                    
                    data = pd.DataFrame({
                        "hucre_id": [hucre['hucre_id']],
                        "kapasite": [kapasite],
                        "efektif_kapasite": [efektif_miktar],
                        "alan_tipi": [alan_tipi],
                        "Koord_x": [x],
                        "Koord_y": [y],
                    })
            
                    trafo_list = pd.concat([trafo_list, data], ignore_index=True)
                    
                    toplam_efektif = trafo_list["efektif_kapasite"].sum()
            
                    if toplam_efektif >= gerekli_kapasite:                                             
                        break_outer_loop = True  # Dış döngüden çıkılmasını tetikle
                        break  # İç döngüden çık
                        
                    if toplam_efektif < gerekli_kapasite and kapasite==p["kurum_trafo_kentsel_alan_liste"][-1]: 
                        continue                    
                    else:
                        
                        trafo_list = trafo_list.drop(trafo_list.index[-1])                   
            
                if break_outer_loop:
                    break  # **Dış döngüden de çık**
                
        yukler = df2[df2["shid"]==shid].copy()
        
        ilkyil = yukler["year"].min()
        
        kapasite = 0
        
        for idtrafo, trafo in trafo_list.iterrows():
                                 
            
            kapasite += trafo["efektif_kapasite"]
            
            tesis_yili = yukler["year"].min()
            
            for idx, row in yukler.iterrows():
                
                yuk = row["kumulatif_yuk"]
                yil = row["year"]                              
                
                if yuk <= kapasite:
                    kapasite -= yuk
                    
                else:
                    
                    dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, dfhucre_super, ilkyil, int(tesis_yili), durum, p)
                    
                    dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                    dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])                                    
                    
                    mask = yukler["year"] >= yil
                    
                    yukler = yukler[mask]
                    
                    break
                
                if idtrafo == len(trafo_list) - 1:
                    
                    dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, dfhucre_super, ilkyil, int(tesis_yili), durum, p)
                    
                    dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                    dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])
                  
                    break
                
            # yukler = df2[df2["shid"] == shid].copy()
            # ilkyil = yukler["year"].min()
            # kapasite = 0
            
            # for idtrafo, trafo in trafo_list.iterrows():
            #     kapasite += trafo["efektif_kapasite"]
            #     tesis_yili = yukler["year"].min()
            
            #     kalan_yukler = []
            #     break_occurred = False
            
            #     for idx, row in yukler.iterrows():
            #         yuk = row["kumulatif_yuk"]
            #         yil = row["year"]
            
            #         if yuk <= kapasite:
            #             kapasite -= yuk
            #         else:
            #             kalan_yukler = yukler[yukler["year"] >= yil].copy()
            #             break_occurred = True
            #             break  # bu yıldan sonrası yeniden değerlendirilecek
            
            #     # ✅ HER TRAFO KAYDEDİLİR (yuk karsılasa da karsılamasa da)
            #     dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, dfhucre_super, ilkyil, int(tesis_yili), durum, p)
                
            #     dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
            #     dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])                                    
                
            #     if break_occurred:
            #         yukler = kalan_yukler
            
            #         # Eğer son trafodaysan ve hâlâ yük varsa → aynı trafoyu tekrar ekle
            #         if idtrafo == len(trafo_list) - 1:
            #             # ekstra trafo daha
            #             dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, dfhucre_super, ilkyil, int(tesis_yili), durum, p)
                        
            #             dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
            #             dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])
            #     else:
            #         break  # tüm yük karşılandıysa çık

      
    return dfyeni_trafo, dfyeni_trafoyıllık

def yeni_trafo_df_olustur(trafo, dfhucre_super, ilkyil, tesis_yili, durum, p, mulkiyet=1, bolge="Kentsel", aksiyon="yeni trafo tesis", musteri=""):
    
    def imar_grubu(g):
        if g in ["teknik_altyapi"]:
            return "imarlı"

        else:
            return "imarsız"
    
    try:
        trafo["İmar Bilgisi"] = imar_grubu(trafo["alan_tipi"])
    
    except:
        trafo["İmar Bilgisi"] = "" 
    
    odtr = config3.get()
    
    try:
        shid = dfhucre_super[dfhucre_super["hucreid"]==trafo["hucre_id"]]["shid"].iloc[0]
    
    except:
        
        shid=0
    
    try:
       
        dfsuperhucre = ReadParquet(odtr["dfsuperhucre"])
        shgerilim = dfsuperhucre.loc[dfsuperhucre["shid"]==shid, "SH Gerilim"].iloc[0]
    
    except:
        shgerilim = 34.5
    
    if mulkiyet == 1:
        sahip = "Kurum"
        
    else:
        sahip = "Özel"
        
    if trafo["kapasite"] > 400:
        trafo_tipi = "Mono Blok"
        
    else:
        trafo_tipi = "Direk Üstü"
        
    
    
    # if "sayac" in trafo.columns:
        
    #     sayac = trafo["sayac"]
        
    #     trafo_id = f"YeniTrafo_{trafo['hucre_id']}_{tesis_yili}_{sahip}_{sayac}"
    
    # else:
        
    sayi = random.randint(10000, 99999)
    
    trafo_id = f"YeniTrafo_{trafo['hucre_id']}_{tesis_yili}_{sahip}_{sayi}"

    veri = {
        "trafo_id": trafo_id,
        "year": tesis_yili,
        "shid": shid,
        "kullanilan_kapasite": 0,
        "trafo_yasi": 0,
        "bos_kapasite":         trafo["kapasite"],
        "merkez_hucre":         trafo["hucre_id"],
        "kapasite":             trafo["kapasite"],
        "eski kapasite":        trafo["kapasite"],
        "efektif_kapasite":     trafo["efektif_kapasite"],
        "eski_efektif_kapasite":trafo["efektif_kapasite"],
        "efektif_bos_kapasite": trafo["efektif_kapasite"],
        "kullanilan_kapasite_efektif": 0,
        "Trafo Mülkiyeti":      sahip,
        "Trafo Bölgesi":        bolge,
        "Trafo Müşterisi":      musteri,
        "Eski Trafo Tipi":      trafo_tipi,
        "Trafo Tipi":           trafo_tipi,
        "İlkGelenYukToplam":    0,
        "GelenYukToplam":       0,
        "İşlem Tarihi":         tesis_yili,
        "Eski Gerilim":         34.5,
        "Gerilim":              34.5,
        "Trafo Aksiyon":        aksiyon,
        "Koord_x":              trafo["Koord_x"],
        "Koord_y":              trafo["Koord_y"],
        "Eski Doluluk Oranı":   0.0,
        "Doluluk Oranı":        0.0,
        "SH Gerilim":           shgerilim,
        "İmar Bilgisi":         trafo["İmar Bilgisi"],
        "Rapor":                ""
    }
    
    dftrafo = pd.DataFrame([veri])
    # 2025'ten 2035'e kadar olan yılları ekle
    years_to_add = list(range(tesis_yili+1, odtr["son_yil"]+1))
    
    # Yeni DataFrame oluştur
    new_rows = []
    
    for year in years_to_add:
        df_year = dftrafo.copy()
        df_year['year'] = year
        df_year['trafo_yasi'] = year - tesis_yili
        df_year['Trafo Aksiyon'] = "mevcut"
        new_rows.append(df_year)
    
    # Yılları ekle ve yeni DataFrame oluştur
    dftrafoyıllık = pd.concat([dftrafo] + new_rows, ignore_index=True)

    return dftrafo, dftrafoyıllık

def TrafoYenile(dftrafocopy, dftrafoYıllık):
    
    dftrafocopy["yenileme_yili"] = np.nan
    dftrafoYıllık["yenileme_yili"] = np.nan

    df = dftrafocopy.copy()
    
    mask = ~(df["Trafo Aksiyon"].isin(["deplase", "gerilim dönüşümü", "güç artırımı"]))
    
    df = df[mask].copy()
    
    # Genişletilmiş DataFrame için boş liste
    expanded_data = []
    
    dftrafo = df[(df["kapasite_arttirildi_mi"] == "Hayır") & (df["Kapasite_yenilenme_yili"].isna())].copy()

    odtr = config3.get()

    # 2025'ten 2030'a kadar olan yılları ekleyerek DataFrame'i genişlet
    for year in range(odtr["ilk_yil"],odtr["son_yil"] + 1):  # 2030 dahil
        temp_df = dftrafo.copy()
        temp_df['year'] = year
        temp_df['trafo_yas'] += (year - odtr["ilk_yil"])  # Yıllara göre yaş güncelle
        expanded_data.append(temp_df)   

    df_expanded = pd.concat(expanded_data, ignore_index=True)
    
    # Her trafo için 30'u geçen ilk yılı bulmak
    df_30dan_buyuk_ilk = df_expanded[df_expanded["trafo_yas"] >= 30] \
        .groupby("trafo_id")["year"].min() \
        .reset_index()
    
    df_30dan_buyuk_ilk["year"] = df_30dan_buyuk_ilk["year"].astype(int)
    df_30dan_buyuk_ilk.rename(columns={"year": "yenileme_yili"}, inplace=True)
    
    df_30dan_buyuk_ilk = df_30dan_buyuk_ilk.merge(df[["trafo_id", "trafo_yasi"]], on="trafo_id", how="left")
    
    df_30dan_buyuk_ilkcopy = df_30dan_buyuk_ilk.copy()
    
    # 3. Trafoları yıllara eşit şekilde dağıtın
    baslangic_yili = odtr["ilk_yil"]
    bitis_yili = odtr["son_yil"]+1
    toplam_yil = bitis_yili - baslangic_yili + 1
    
    uzunluk = len(df_30dan_buyuk_ilk) // toplam_yil + 1
    
    for year in range(odtr["ilk_yil"],odtr["son_yil"] + 1):
        
        year = int(year)
        
        # mask = df_30dan_buyuk_ilkcopy["trafo_yasi"].sort_values(ascending=False)[:uzunluk]
        mask = df_30dan_buyuk_ilkcopy.sort_values(by="trafo_yasi", ascending=False).head(uzunluk)

        for idx, row in mask.iterrows():
            dftrafoYıllık.loc[
                (dftrafoYıllık["trafo_id"] == row['trafo_id']) & 
                (dftrafoYıllık["year"] == year),
                "yenileme_yili"
            ] = year
            
            dftrafoYıllık.loc[
                (dftrafoYıllık["trafo_id"] == row['trafo_id']) & 
                (dftrafoYıllık["year"] == year),
                "İşlem Tarihi"
            ] = year
            
            dftrafoYıllık.loc[
                (dftrafoYıllık["trafo_id"] == row['trafo_id']) & 
                (dftrafoYıllık["year"] == year),
                "Trafo Aksiyon"
            ] = "trafo yenileme-yaştan"
            
            dftrafoYıllık.loc[
                (dftrafoYıllık["trafo_id"] == row['trafo_id']) & 
                (dftrafoYıllık["year"] >= year),
                "Gerilim"
            ] = 34.5
            
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "yenileme_yili"] = year
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "İşlem Tarihi"] = year
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "Trafo Aksiyon"] = "trafo yenileme-yaştan"
            dftrafocopy.loc[(dftrafocopy["trafo_id"] == row['trafo_id']), "Gerilim"] = 34.5
                 
        df_30dan_buyuk_ilkcopy = df_30dan_buyuk_ilkcopy.loc[~df_30dan_buyuk_ilkcopy.index.isin(mask.index)]   
 
    dftrafocopy["trafo_yasi"] = df["trafo_yas"].copy()
        
    return dftrafocopy
    
def ToParquet(df, path):
    
    df.to_parquet(path, engine='pyarrow', index=False)

def ReadParquet(path):
    
    df = pd.read_parquet(path, engine='pyarrow')  # veya engine='fastparquet'
    
    return df

def SaturasyonlariBirlestir(trafoAlanları):
    
    try:
        
        dfsaturasyon = pd.read_excel(r"C:\Users\vural.bayrakli\OneDrive - MRC\İletişim sitesi - MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\il_ilce_kırılımları\İzmir\Karşıyaka\proje_karsıyaka_deneme\imar_analizi_sonuclari\imar_planlari\hucresel_dtr_verileri\karşıyaka_saturasyon.xlsx")
    
        trafoAlanları = trafoAlanları.merge(dfsaturasyon, on="hucre_id", how="left")
    
    except Exception as e:
        print(e)
    
    return trafoAlanları
    
def process_row(row, odtr):
    # Eğer 'rezerve_trafo_adlari' boşsa, işlemi atla
    if pd.isna(row['rezerve_trafo_adlari']) or str(row['rezerve_trafo_adlari']).strip() == '':
        return []

    # Trafo adlarını ve yaşlarını ayır
    trafo_adlari = [ad.strip() for ad in str(row['rezerve_trafo_adlari']).split(',')]
    trafo_yaslari = [yas.strip() for yas in str(row['trafo_yaslari']).split(',')]

    # Eğer varsa, trafo kodlarını ve merkez centroidlerini al
    trafo_kodlari = [yas.strip() for yas in str(row['rezerve_trafo_kodlari']).split(',')] if "rezerve_trafo_kodlari" in row else []
    rezerv_trafo_centroids = [yas.strip() for yas in str(row['rezerv_trafo_centroids']).split(',')] if "rezerv_trafo_centroids" in row else []

    # Proje trafo id'leri ve yatırım yılları
    proje_trafo_idler = str(row['proje_trafo_idler']) if not pd.isna(row['proje_trafo_idler']) else ''
    yatirim_yillari = row['yatirim_yillari'] if not pd.isna(row['yatirim_yillari']) else pd.NA

    satirlar = []
    # Trafo adı, yaşı, kodu ve merkez centroidi üzerinden yeni satırları oluştur
    for ad, yas, kod, merkez_centroid in zip(trafo_adlari, trafo_yaslari, trafo_kodlari, rezerv_trafo_centroids):
        yeni_satir = row.copy()
        yeni_satir['rezerve_trafo_adlari'] = ad
        yeni_satir['rezerve_trafo_adedi'] = 1
        yeni_satir['trafo_yaslari'] = float(yas)

        if "rezerve_trafo_kodlari" in row:
            yeni_satir['rezerve_trafo_kodlari'] = kod
            yeni_satir['rezerv_trafo_centroids'] = merkez_centroid

        # Proje trafo ID'yi ayarla
        numara = re.findall(r'\d+', ad)
        numara = numara[0] if numara else None

        if numara and numara in proje_trafo_idler:
            yeni_satir['proje_trafo_idler'] = proje_trafo_idler
            yeni_satir['yatirim_yillari'] = yatirim_yillari
        else:
            yeni_satir['proje_trafo_idler'] = pd.NA
            yeni_satir['yatirim_yillari'] = odtr["ilk_yil"]

        satirlar.append(yeni_satir)

    return satirlar

def TrafoAlanlariIslem(odtr, trafo_alanlari,p):
    
    veri = trafo_alanlari.copy()
    
    # veri = SaturasyonlariBirlestir(veri)
    
    trafo_kod_satirlar = []
    
    centroid_satirlar = []
    
    trafo_kod_silinen_indexler = []
    
    centroid_silinen_indexler = []

    for idx, row in veri.iterrows():
        
        trafo_kodlari = pd.DataFrame()
        rezerv_trafo_centroids = pd.DataFrame()
        
        if pd.isna(row['rezerve_trafo_kodlari']) or str(row['rezerve_trafo_kodlari']).strip() == '':
            continue

        trafo_adlari = [ad.strip() for ad in str(row['rezerve_trafo_adlari']).split(',')]        
            
        trafo_kodlari = [kod.strip() for kod in str(row['rezerve_trafo_kodlari']).split(',')]

        # Diyelim row['rezerv_trafo_centroids'] = '[[38.47298, 27.06617], [38.47349, 27.06585]]'
        text = row['rezerv_trafo_centroids']
        
        try:
            centroids = ast.literal_eval(text)  # Güvenli şekilde str → Python objesi
            if isinstance(centroids[0], list):
                # Zaten liste listesi: birden çok centroid
                rezerv_trafo_centroids = centroids
            else:
                # Tek bir centroid liste içinde: listele
                rezerv_trafo_centroids = [centroids]
        except:
            rezerv_trafo_centroids = []

        # rezerv_trafo_centroids = [centroid.strip() for centroid in str(row['rezerv_trafo_centroids']).split(',')]

        proje_trafo_idler = str(row['proje_trafo_idler']) if not pd.isna(row['proje_trafo_idler']) else ''
        yatirim_yillari = row['yatirim_yillari'] if not pd.isna(row['yatirim_yillari']) else pd.NA

        if len(trafo_kodlari) > 1:  # sadece çoklu satırları işaretle
            trafo_kod_silinen_indexler.append(idx)
            
        if len(rezerv_trafo_centroids) > 1:  # sadece çoklu satırları işaretle
            centroid_silinen_indexler.append(idx)
        
        if len(trafo_kodlari) > 1:
            
            for kod in trafo_kodlari:
                yeni_satir = row.copy()
                
                yeni_satir['rezerve_trafo_adedi'] = 1
                                          
                yeni_satir['proje_trafo_idler'] = kod.strip()
                
                if pd.notna(yatirim_yillari):
                    yeni_satir['yatirim_yillari'] = yatirim_yillari
                else:
                    yeni_satir['yatirim_yillari'] = pd.NA  
                    
                trafo_kod_satirlar.append(yeni_satir)
                
    duzgun_df = pd.DataFrame(trafo_kod_satirlar)
    
    duzgun_df_cent = pd.DataFrame(centroid_satirlar)

    # Eski çoklu satırları veri setinden çıkar
    trafo_kod_veri_temiz = veri.drop(index=trafo_kod_silinen_indexler)
    
    # Temiz veri ile yeni satırları birleştir
    veri_yeni = pd.concat([trafo_kod_veri_temiz, duzgun_df], ignore_index=True).reset_index(drop=True)
    
    veri_yeni["yatırım_rezerve"] = veri_yeni["rezerve_trafo_kodlari"].str.contains("YATIRIM REZERVE", case=False)
    
    if veri_yeni["yatırım_rezerve"].any():
        mask = (veri_yeni["rezerve"] == 1) & (veri_yeni["proje_trafo_idler"].isna()) & (veri_yeni["yatirim_yillari"].isna())
        veri_yeni["rezerve_trafo"] = np.where(mask, 1,0) 
    
    else:
        veri_yeni["rezerve_trafo"] = 0
    
    ## Aksiyonlara göre trafo id leri çekme. Trafo listeside Trafo Aksiyon kısmında bu değerler olacak.
    aksiyonlar = []
    projelendirilmis = []
    projelendirilmis_trafo = pd.DataFrame()
    projelendirilmis_trafoyıllık = pd.DataFrame()

    for idx, row in veri_yeni.iterrows():
        
        aktif_sütun = ""
        
        if "rezerve_trafo_kodlari" in veri_yeni.columns:
            
            trafo_id = row.get('proje_trafo_idler', "proje-Trafo")
            aktif_sütun = "rezerve_trafo_kodlari"

        else:
            
            trafo_id = row.get('proje_trafo_idler', "proje-Trafo")
            aktif_sütun = "proje_trafo_idler"
                          
        hucre_id = row.get('hucre_id')

        if pd.isna(trafo_id):
            continue  # Proje Trafo ID boşsa geç

        if row.get('guc_artirimi_sayisi', 0) == 1:
            
            aksiyonlar.append({
                'trafo_id': trafo_id,
                'aksiyon': 'güç artırımı',
                'hucre_id': hucre_id,
                'mevcut_kapasite': row.get('mevcut_kapasite', 0),
                'yeni_kapasite': row.get('yeni_kapasite', 0),
                'İşlem Tarihi': row.get('yatirim_yillari', odtr['ilk_yil']),
                'year': odtr['ilk_yil'],
            })

        if row.get('gerilim_donusumu_sayisi', 0) == 1:
            aksiyonlar.append({
                'trafo_id': trafo_id,
                'aksiyon': 'gerilim dönüşümü',
                'hucre_id': hucre_id,
                'mevcut_kapasite': row.get('mevcut_kapasite', 0),
                'yeni_kapasite': row.get('yeni_kapasite', 0),
                'İşlem Tarihi': row.get('yatirim_yillari', odtr['ilk_yil']),
                'year': odtr['ilk_yil'],

            })

        if row.get('deplase_sayisi', 0) == 1:
            aksiyonlar.append({
                'trafo_id': trafo_id,
                'aksiyon': 'deplase',
                'hucre_id': hucre_id,
                'mevcut_kapasite': row.get('mevcut_kapasite', 0),
                'yeni_kapasite': row.get('yeni_kapasite', 0),
                'İşlem Tarihi': row.get('yatirim_yillari', odtr['ilk_yil']),               
                'year': row.get('yatirim_yillari', odtr['ilk_yil']),

            })
        
        
        if (row.get(aktif_sütun, "") != "") and (row.get('yeni_trafo_sayisi', 0) == 1) and (row.get('rezerve', 0) == 1) and pd.isna(row.get('trafo_yaslari', "")) and row.get('yatirim_yillari', 0) >= odtr["ilk_yil"] :
            
            trafo = pd.DataFrame({
                
                 "trafo_id":[trafo_id],
                 "hucre_id":[hucre_id],
                 "kapasite":[row.get('yeni_kapasite', 0)],
                 "efektif_kapasite":[row.get('yeni_kapasite', 0)],
                 "Koord_x":[None],
                 "Koord_y":[None]
                 
                 }, columns=["trafo_id", "hucre_id", "kapasite", "efektif_kapasite", "Koord_x", "Koord_y"])
            
            trafo = trafo.iloc[0]

            tesis_yili = int(row.get('yatirim_yillari', odtr['ilk_yil']))
            
            dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, None, None, tesis_yili, None, p, aksiyon='projelendirilmiş yeni trafo')
            
            projelendirilmis_trafoyıllık = pd.concat([projelendirilmis_trafoyıllık, dftrafoyıllık], ignore_index=True)
            projelendirilmis_trafo = pd.concat([projelendirilmis_trafo, dftrafo], ignore_index=True)
                            
    aksiyon_df = pd.DataFrame(aksiyonlar)
    
    try:
        
        SqliteKaydet(aksiyon_df, "aksiyon_df")
    
    except Exception as e:
        
        logging.info("Aksiyon Verisi Boş!")
        
    ## Yeni Trafo eklerken rezerve, park koordinatlarına göre ekleme yapılacak. 
    ## Bu koordinatları tutan dataframe
    # Alan kolonları ile centroid kolonları eşleşmesi
    alan_centroid_eslesme = {
        
        'teknik_altyapi': 'teknik_altyapi_centroids',
        'rezerve_trafo': 'rezerv_trafo_centroids',
        'park': 'park_centroids',
        'cocuk_parki': 'cocuk_parki_centroids',
        'cami': 'cami_centroids',
        'pazar_alani': 'pazar_alani_centroids',
        'otopark': 'otopark_centroids',
        'belediye_hizmet': 'belediye_hizmet_centroids'
        
    }

    # Yeni veri listesi
    yeni_satirlar = []
    
    try:
        
        for idx, row in veri.iterrows():
            hucre_id = row['hucre_id']  # Hücre ID'yi alalım
            for alan in alan_centroid_eslesme:  # Sıralı dict iterasyonu (öncelik önemli)
                centroid_kolon = alan_centroid_eslesme[alan]
                if not pd.isna(row.get(alan)) and row.get(alan) != 0:
                    centroidler = eval(row.get(centroid_kolon))  # str olanları listeye çevir
                    for i, centroid in enumerate(centroidler, 1):
                        yeni_satirlar.append({
                            'hucre_id': hucre_id,
                            'alan_tipi': alan,
                            'centroid': str(centroid),
                            'alan_id': i
                        })
                    
    except Exception as e:
        logging.error(e)
        
    # for idx, row in veri_yeni.iterrows():
    #     hucre_id = row['hucre_id']  # Hücre ID'yi alalım

    #     for alan, centroid_kolon in alan_centroid_eslesme.items():
    #         if not pd.isna(row.get(alan)) and row.get(alan) != 0:
    #             centroidler = eval(row.get(centroid_kolon))  # String gibi görünüyorsa listeye çevir
    #             for i, centroid in enumerate(centroidler, 1):
    #                 yeni_satirlar.append({
    #                     'hucre_id': hucre_id,
    #                     'alan_tipi': alan,
    #                     'centroid': str(centroid),
    #                     'alan_id': i
    #                 })

    # Yeni df
    
    try: 
        alanlar_df = pd.DataFrame(yeni_satirlar)
        
        SqliteKaydet(alanlar_df, "alanlar_df")
        
    except Exception as e:
        logging.error(e)
   
    return veri_yeni, aksiyon_df, projelendirilmis_trafo, projelendirilmis_trafoyıllık
    
def TrafoOlusturYıllık(odtr, p, mulkiyet):
    
    if odtr["ilk"]:
        
        dftrafo = pd.read_csv(odtr['trafo_path'], encoding='utf-8-sig', sep=',')
        # dftrafo = pd.read_excel(odtr["trafo_path"])
        SqliteKaydet(dftrafo, odtr["trafo_db_adi"])
        
    else:
        
        dftrafo = SqliteOku(odtr["trafo_db_adi"])
    
    dftrafo["year"] = odtr['ilk_yil']-1
    dftrafo["kullanilan_kapasite"] = dftrafo["trafo_demand_kw"] / 0.9
    dftrafo["bos_kapasite"] = dftrafo["trafo_kapasite_kva"] - dftrafo["kullanilan_kapasite"]
    dftrafo["kapasite_arttirildi_mi"] = "Hayır"
    dftrafo["Kapasite_artirma_yili"] = np.nan
    dftrafo["trafo_yasi"] = dftrafo["trafo_yas"].copy()  # Mevcut trafo yaşlarını koru
    
    if mulkiyet == 1:
        sahip = "Kurum"
        
    else:
        sahip = "Özel"
        
    kurum_trafolari = dftrafo[dftrafo["kurum_trafosu"]==mulkiyet]["trafo_id"].values
    dftrafo.loc[dftrafo["trafo_id"].isin(kurum_trafolari), "Trafo Mülkiyeti"] = sahip
    dftrafo = dftrafo[dftrafo["trafo_id"].isin(kurum_trafolari)]
    dftrafo["Eski Gerilim"] = dftrafo["primer_gerilim"]
    dftrafo["İlkGelenYukToplam"] = dftrafo["kullanilan_kapasite"]
    dftrafo["eski kapasite"] = dftrafo["trafo_kapasite_kva"].copy()
    dftrafo["Eski Doluluk Oranı"] = dftrafo["İlkGelenYukToplam"]/dftrafo["eski kapasite"]
    dftrafo["eski_merkez_hucre"] = dftrafo["merkez_hucre"].copy()
    dftrafo["Eski Trafo Tipi"] = ""
    dftrafo["eski_efektif_kapasite"] = dftrafo["eski kapasite"] * p["mevcut_trafo_kapasite_kullanim_ust_limiti"]
    
    dftrafo["kapasite"] = dftrafo["trafo_kapasite_kva"].copy()
    dftrafo["primer_gerilim"] = dftrafo["primer_gerilim"].where(
    dftrafo["primer_gerilim"] <= 1000,
    dftrafo["primer_gerilim"] / 1000
    )
    
    dftrafo["kullanilan_kapasite_efektif"] = dftrafo["kullanilan_kapasite"] * p["mevcut_trafo_kapasite_kullanim_ust_limiti"]
    dftrafo["efektif_kapasite"] = dftrafo["kapasite"] * p["mevcut_trafo_kapasite_kullanim_ust_limiti"]
    dftrafo["efektif_bos_kapasite"] = dftrafo["efektif_kapasite"] - dftrafo["kullanilan_kapasite"]
    dftrafo["GelenYukToplam"] = dftrafo["kullanilan_kapasite"]
    dftrafo["Trafo Durum"] = ""
    dftrafo["yenileme_yili"] = ""
    dftrafo["Trafo Aksiyon"] = "mevcut"
    dftrafo = dftrafo.rename(columns={'TRAFO_X_KOORDINAT':'Koord_x', 'TRAFO_Y_KOORDINAT':'Koord_y'})
    # dftrafo["Koord_x"] = ""
    # dftrafo["Koord_y"] = ""
    dftrafo["Trafo Bölgesi"] = "Kentsel"
    dftrafo["Trafo Mülkiyeti"] = sahip
    dftrafo["Gerilim"] = dftrafo["primer_gerilim"]
    dftrafo["Hücre Gerilimi"] = ""
    dftrafo["Doluluk Oranı"] = dftrafo["İlkGelenYukToplam"]/dftrafo["eski kapasite"]
    dftrafo["İmar Bilgisi"] = ""
    dftrafo["Rapor"] = ""
    
    # 2025'ten 2035'e kadar olan yılları ekle
    years_to_add = list(range(odtr["ilk_yil"], odtr["son_yil"]+1))
    
    # Yeni DataFrame oluştur
    new_rows = []
    
    for year in years_to_add:
        df_year = dftrafo[dftrafo["year"] == odtr["ilk_yil"]-1].copy()
        df_year['year'] = year
        df_year['trafo_yasi'] = year - (odtr["ilk_yil"] - 1) + df_year['trafo_yas']
        new_rows.append(df_year)
    
    # Yılları ekle ve yeni DataFrame oluştur
    dftrafo = pd.concat([dftrafo[dftrafo["year"] == odtr["ilk_yil"]-1]] + new_rows, ignore_index=True)
    
    return dftrafo

def TrafoOlustur(odtr, p, mulkiyet=1):
    
    if odtr["ilk"]:
        
        dftrafo = pd.read_csv(odtr['trafo_path'], encoding='utf-8-sig', sep=',')
        # dftrafo = pd.read_excel(odtr["trafo_path"])
        SqliteKaydet(dftrafo, odtr["trafo_db_adi"])
        
    else:
        
        dftrafo = SqliteOku(odtr["trafo_db_adi"])
    
    dftrafo["year"] = odtr['ilk_yil']-1
    dftrafo["kullanilan_kapasite"] = dftrafo["trafo_demand_kw"] / 0.9
    dftrafo["bos_kapasite"] = dftrafo["trafo_kapasite_kva"] - dftrafo["kullanilan_kapasite"]
    dftrafo["kapasite_arttirildi_mi"] = "Hayır"
    dftrafo["Kapasite_artirma_yili"] = np.nan
    
    dftrafo["primer_gerilim"] = dftrafo["primer_gerilim"].where(
    dftrafo["primer_gerilim"] <= 1000,
    dftrafo["primer_gerilim"] / 1000
    )

    dftrafo["trafo_yasi"] = dftrafo["trafo_yas"].copy()  # Mevcut trafo yaşlarını koru
    
    if mulkiyet == 1:
        sahip = "Kurum"
        
    else:
        sahip = "Özel"
        
    kurum_trafolari = dftrafo[dftrafo["kurum_trafosu"]==mulkiyet]["trafo_id"].values
    dftrafo.loc[dftrafo["trafo_id"].isin(kurum_trafolari), "Trafo Mülkiyeti"] = sahip
    dftrafo = dftrafo[dftrafo["trafo_id"].isin(kurum_trafolari)]
    
    dftrafo["eski kapasite"] = dftrafo["trafo_kapasite_kva"].copy()
    dftrafo["efektif_kapasite"] = dftrafo["eski kapasite"] * p["mevcut_trafo_kapasite_kullanim_ust_limiti"]
    dftrafo["eski_efektif_kapasite"] = dftrafo["eski kapasite"] * p["mevcut_trafo_kapasite_kullanim_ust_limiti"]
    dftrafo["eski_merkez_hucre"] = dftrafo["merkez_hucre"].copy()
    dftrafo["Eski Trafo Tipi"] = ""
    dftrafo["Eski Gerilim"] = dftrafo["primer_gerilim"]
    dftrafo["İlkGelenYukToplam"] = dftrafo["kullanilan_kapasite"]
    dftrafo["Eski Doluluk Oranı"] = dftrafo["İlkGelenYukToplam"]/dftrafo["eski kapasite"]

    dftrafo["kapasite"] = dftrafo["trafo_kapasite_kva"].copy()
    dftrafo["kullanilan_kapasite_efektif"] = dftrafo["kullanilan_kapasite"] * p["mevcut_trafo_kapasite_kullanim_ust_limiti"]
    dftrafo["Trafo Tipi"] = ""   
    dftrafo["efektif_bos_kapasite"] = dftrafo["efektif_kapasite"] - dftrafo["kullanilan_kapasite"]
    dftrafo["GelenYukToplam"] = dftrafo["kullanilan_kapasite"]
    dftrafo["Trafo Durum"] = ""
    dftrafo["yenileme_yili"] = ""
    dftrafo["İşlem Tarihi"] = np.nan
    dftrafo["Trafo Aksiyon"] = "mevcut"
    dftrafo = dftrafo.rename(columns={'TRAFO_X_KOORDINAT':'Koord_x', 'TRAFO_Y_KOORDINAT':'Koord_y'})
    # dftrafo["Koord_x"] = ""
    # dftrafo["Koord_y"] = ""
    dftrafo["Trafo Bölgesi"] = "Kentsel"
    dftrafo["Trafo Mülkiyeti"] = sahip
    dftrafo["Gerilim"] = dftrafo["primer_gerilim"]
    dftrafo["Hücre Gerilimi"] = ""
    dftrafo["Doluluk Oranı"] = dftrafo["İlkGelenYukToplam"]/dftrafo["eski kapasite"]
    dftrafo["İmar Bilgisi"] = ""
    dftrafo["Rapor"] = ""
    
    dftrafoYıllık = TrafoOlusturYıllık(odtr, p, mulkiyet)
    
    return dftrafo, dftrafoYıllık

def FonkTrafo():
                
    odtr = config3.get()
    
    katsayilar_path = os.path.join(odtr["dosyalar"], "katsayılar.xlsx")
    df = pd.read_excel(katsayilar_path, engine="openpyxl")  # Daha hızlı olması için openpyxl kullanılır
    df = df.iloc[:,[0,3]]
    p = config3.Start(df)
    
    try:
    # Veri çekilmeye çalışılıyor
        
        if odtr["ilk"]: 
            
            trafo_alanlari = pd.read_csv(odtr['trafo_alanlari_path'], encoding='utf-8-sig', sep=',')
            # trafo_alanlari = pd.read_csv(odtr['trafo_alanlari_path'], engine="openpyxl")
            
            # trafo_alanlari = trafo_alanlari.astype(str)

            ToParquet(trafo_alanlari, odtr['trafo_alanlari_orijinal_parquet'])
            
        else:
            
            # trafo_alanlari = SqliteOku(f"trafo_alanlari_{odtr['ilce']}")
            trafo_alanlari = ReadParquet(odtr['trafo_alanlari_orijinal_parquet'])
            
            # Sayısal sütunları tespit et ve sayısal hale getir
            for column in trafo_alanlari.columns:
                # Eğer sütun tamamen sayısal ise (sayısal olmayan değerleri içeriyorsa bu işlemi geç)
                if trafo_alanlari[column].str.isnumeric().all():
                    trafo_alanlari[column] = pd.to_numeric(trafo_alanlari[column])
            
    except Exception as e:
        print(f"veri okunamadı: {e}")
        sys.exit()
             
    # İşle
    trafo_alanlari, aksiyon_df, trafo, trafoYıllık = TrafoAlanlariIslem(odtr, trafo_alanlari, p)
    
    for col in trafo_alanlari.columns:
        if trafo_alanlari[col].dtype == 'object':
            try:
                trafo_alanlari[col] = pd.to_numeric(trafo_alanlari[col].str.replace(",", "").str.strip(), errors='raise').astype('int64')
            except:
                pass  # Sayıya çevrilemeyenleri atla

    # SQLite'a kaydet
    SqliteKaydet(trafo_alanlari, f"trafo_alanlari_{odtr['ilce']}")
    
    binary_columns = [col for col in trafo_alanlari.columns if trafo_alanlari[col].dropna().isin([0, 1]).all()] 
    
    columns = ["hucre_id", "teknik_altyapi", "rezerve_trafo", "park", "cocuk_parki", "cami", "pazar_alani", "otopark", "belediye_hizmet"]
    #df_binary = trafo_alanlari[binary_columns].iloc[:,:10]  
    #df_binary = trafo_alanlari[binary_columns].iloc[:,:13]  
    df_binary = trafo_alanlari.loc[:,columns]  

    # df_binary["hucre_id"] = trafo_alanlari["hucre_id"] 
    
    df_binary = df_binary.drop_duplicates(subset='hucre_id', keep='first')
    
    df_binary = df_binary.astype({col: 'int64' for col in df_binary.select_dtypes(include='number').columns})

    dftrafo = pd.read_csv(odtr['trafo_path'], encoding='utf-8-sig', sep=',')  # Daha hızlı olması için openpyxl kullanılır
    # dftrafo = pd.read_excel(odtr['trafo_path'], engine="openpyxl")  # Daha hızlı olması için openpyxl kullanılır
    #dftrafo.to_parquet(trafo_path_parquet, engine='pyarrow', index=False)
    #ToParquet(dftrafo, odtr['trafo_path_parquet'])
    
    SqliteKaydet(dftrafo, f"dftrafo_{odtr['ilce']}")
    
    dftrafo, dftrafoYıllık = TrafoOlustur(odtr, p)

    return dftrafo, dftrafoYıllık, df_binary, aksiyon_df, trafo, trafoYıllık

def PLYukAtama(dfYuk, odtr):
    
    point_load = pd.read_excel(odtr["pointload_path"]) 
    
    liste = np.arange(0.5, 3.5 + 0.5, 0.5)
    
    liste_yuk = [(l * 8760 / 2.5) for l in liste]
    
    for index, row in point_load.iterrows():
        
        cell_id = row['cell_id']
        yil = row['year']
        bt = row['building_type']
        yuk = random.choice(liste_yuk)
        
        if (bt == "BUYUK_TICARETHANE"):
            kolon2 = "TICARETHANE_HORIZONTAL" 
            kolon1 = "TICARETHANE"
        
        else:
            kolon2 = "SANAYI_HORIZONTAL"
            kolon1 = "SANAYI"

        # Yükü 2/4, 1/4, 1/4 şeklinde ayırma
        y1 = yuk * 2 / 4  # Başlangıç yılına ekle
        y2 = yuk * 1 / 4  # Bir sonraki yıl
        y3 = yuk * 1 / 4  # Sonraki yıl
        
        # dfYuk'te cell_id'ye göre uygun yıllarda kümülatif yük eklemek
        # Başlangıç yılına yükün 2/4'ü
        
        dfYuk.loc[(dfYuk['id'] == cell_id) & (dfYuk['year'] >= yil), kolon1] = 0
        dfYuk.loc[(dfYuk['id'] == cell_id) & (dfYuk['year'] < yil), kolon2] = 0
        
        dfYuk.loc[(dfYuk['id'] == cell_id) & (dfYuk['year'] == yil), kolon2] = y1
        
        # Bir sonraki yıl, yükün 1/4'ü eklenir
        dfYuk.loc[(dfYuk['id'] == cell_id) & (dfYuk['year'] == yil + 1), kolon2] += (y1 + y2)
        
        # İki yıl sonraki yıl, yükün kalan 1/4'ü eklenir
        dfYuk.loc[(dfYuk['id'] == cell_id) & (dfYuk['year'] == yil + 2), kolon2] += (y1 + y2 + y3)
        
        # Kalan yıllar için, kümülatif yük devam eder
        for year in range(yil + 3, odtr["son_yil"] + 1):
            dfYuk.loc[(dfYuk['id'] == cell_id) & (dfYuk['year'] == year), kolon2] += (y1 + y2 + y3)
            
        
    return dfYuk


def KurumYukGuncel(df,dfEA, dfhucre_super):
    
    odtr = config3.get()
    
    point_load = pd.read_excel(odtr["pointload_path"]) 
    
    bsid = point_load[(point_load["Tüketim Sınıfı"]=="SANAYI") & (point_load["Pik Demant (kW)"]>0)]
    
    df = df.merge(bsid, on="id", how="left", suffixes=("", "_cutoff"))
        
    # 1. year < cutoff → 0
    df.loc[df["year"] < df["ENERJILENDIRME_YILI"], "SANAYI_SULAMA_POINT_LOAD"] = 0

    # 2. year > cutoff → SANAYI değerini al
    df.loc[df["year"] >= df["ENERJILENDIRME_YILI"], "SANAYI_SULAMA_POINT_LOAD"] = df.loc[df["year"] >= df["ENERJILENDIRME_YILI"], "Pik Demant (kW)"]
    
    mask = df["ENERJILENDIRME_YILI"].notna() & (df["year"] >= df["ENERJILENDIRME_YILI"])
    
    df.loc[mask, "SANAYI"] -= df.loc[mask, "Pik Demant (kW)"] 
            
    df_BS = df[df["id"].isin(bsid["id"].values)].copy()
    
    SqliteKaydet(df_BS, f"df_BS_{odtr['ilce']}")
    bs_parquet = os.path.join(odtr["python_dosya_yolu"], f"BSYUK{odtr['ilce']}.parquet")
    ToParquet(df_BS, bs_parquet)
          
    bstid = point_load[(point_load["Tüketim Sınıfı"]=="TICARETHANE") & (point_load["Pik Demant (kW)"]>0)]
    
    df = df.merge(bstid, on="id", how="left", suffixes=("", "_cutoff"))

    # 1. year < cutoff → 0
    df.loc[df["year"] < df["ENERJILENDIRME_YILI"], "TICARETHANE_POINT_LOAD"] = 0

    # 2. year > cutoff → SANAYI değerini al
    df.loc[df["year"] >= df["ENERJILENDIRME_YILI"], "TICARETHANE_POINT_LOAD"] = df.loc[df["year"] >= df["ENERJILENDIRME_YILI"], "Pik Demant (kW)"]
    
    df.loc[df["year"] >= df["ENERJILENDIRME_YILI"], "TICARETHANE"] -= df.loc[df["year"] >= df["ENERJILENDIRME_YILI"], "Pik Demant (kW)"]       
    
    df_BT = df[df["id"].isin(bstid["id"].values)].copy()
    
    #df_BT.to_parquet(f"df_BT.parquet", engine='pyarrow', index=False)
    SqliteKaydet(df_BT, f"df_BT_{odtr['ilce']}")
    bt_parquet = os.path.join(odtr["python_dosya_yolu"], f"BTYUK{odtr['ilce']}.parquet")
    ToParquet(df_BT, bt_parquet)
    
    tsid = point_load[(point_load["Tüketim Sınıfı"]=="TARIMSAL SULAMA") & (point_load["Pik Demant (kW)"]>0)]
    
    df = df.merge(tsid, on="id", how="left", suffixes=("", "_cutoff"))

    # 1. year < cutoff → 0
    df.loc[df["year"] < df["ENERJILENDIRME_YILI"], "TARIMSAL_SULAMA_POINT_LOAD"] = 0

    # 2. year > cutoff → SANAYI değerini al
    df.loc[df["year"] >= df["ENERJILENDIRME_YILI"], "TARIMSAL_SULAMA_POINT_LOAD"] = df.loc[df["year"] >= df["ENERJILENDIRME_YILI"], "Pik Demant (kW)"]
    
    df.loc[df["year"] >= df["ENERJILENDIRME_YILI"], "TARIMSAL_SULAMA"] -= df.loc[df["year"] >= df["ENERJILENDIRME_YILI"], "Pik Demant (kW)"]                     
             
    df_TS = df[df["id"].isin(tsid["id"].values)].copy()
    
    # Hücre - Süper hücre eşleşmesi
    df_TS = df_TS.merge(dfhucre_super, left_on="id", right_on="hucreid", how="left").drop(["hucreid"], axis=1)
         
    SqliteKaydet(df_TS, f"df_TS_{odtr['ilce']}") 
    ts_parquet = os.path.join(odtr["python_dosya_yolu"], f"TSYUK{odtr['ilce']}.parquet")
    ToParquet(df_TS, ts_parquet)
          
    return df

def KurumYuk(df,dfEA, dfhucre_super):
    
    PLeklendi = True
    
    odtr = config3.get()
    
    point_load = pd.read_excel(odtr["pointload_path"]) 
    
    bsid = point_load[point_load["building_type"]=="BUYUK_SANAYI"]
    
    if PLeklendi:
        
        df = PLYukAtama(df.copy(), odtr)
    
        df["SANAYI_HORIZONTAL_temp"] = df["SANAYI_HORIZONTAL"]
        
    if not PLeklendi:
        
        df = df.merge(bsid, left_on="id", right_on="cell_id", how="left", suffixes=("", "_cutoff"))
            
        # 1. year < cutoff → 0
        df.loc[df["year"] < df["year_cutoff"], "SANAYI_HORIZONTAL_temp"] = 0
    
        # 2. year > cutoff → SANAYI değerini al
        df.loc[df["year"] >= df["year_cutoff"], "SANAYI_HORIZONTAL_temp"] = df.loc[df["year"] >= df["year_cutoff"], "SANAYI"]
        
        mask = df["year_cutoff"].notna() & (df["year"] >= df["year_cutoff"])
        
        df.loc[mask, "SANAYI"] = 0
            
    df_BS = df[df["id"].isin(bsid["cell_id"].values)].copy()
    
    SqliteKaydet(df_BS, f"df_BS_{odtr['ilce']}")
    bs_parquet = os.path.join(odtr["python_dosya_yolu"], f"BSYUK{odtr['ilce']}.parquet")
    ToParquet(df_BS, bs_parquet)
          
    bstid = point_load[point_load["building_type"]=="BUYUK_TICARETHANE"]
    
    if PLeklendi:
        
        df["TICARETHANE_HORIZONTAL_temp"] = df["TICARETHANE_HORIZONTAL"]
        
    if not PLeklendi:
        
        df = df.drop(["year_cutoff", "cell_id", "building_type"], axis=1)
        
        df = df.merge(bstid, left_on="id", right_on="cell_id", how="left", suffixes=("", "_cutoff"))
    
        # 1. year < cutoff → 0
        df.loc[df["year"] < df["year_cutoff"], "TICARETHANE_HORIZONTAL_temp"] = 0
    
        # 2. year > cutoff → SANAYI değerini al
        df.loc[df["year"] >= df["year_cutoff"], "TICARETHANE_HORIZONTAL_temp"] = df.loc[df["year"] >= df["year_cutoff"], "TICARETHANE"]
        
        df.loc[df["year"] >= df["year_cutoff"], "TICARETHANE"] = 0                         
            
        df_BT = df[df["id"].isin(bstid["cell_id"].values)].copy()
    
        df = df.drop(["year_cutoff", "cell_id", "building_type"], axis=1)
    
    df_BT = df[df["id"].isin(bstid["cell_id"].values)].copy()
    
    #df_BT.to_parquet(f"df_BT.parquet", engine='pyarrow', index=False)
    SqliteKaydet(df_BT, f"df_BT_{odtr['ilce']}")
    bt_parquet = os.path.join(odtr["python_dosya_yolu"], f"BTYUK{odtr['ilce']}.parquet")
    ToParquet(df_BT, bt_parquet)
     
    # mask = df.groupby("id")["TARIMSAL_SULAMA_HORIZONTAL"].transform(lambda x: (x != 0).any())    
    
    # mask = df["id"].map(
    #     df.groupby("id")["TARIMSAL_SULAMA_HORIZONTAL"].apply(lambda x: (x != 0).any())
    # )

    # # 2. Bu maske ile tüm yılları filtrele
    # df_TS = df[mask].copy()
    
    # 1. Her id için TARIMSAL_SULAMA_HORIZONTAL değerlerinin en az biri 0'dan farklı mı?
    nonzero_ids = df.groupby("id")["TARIMSAL_SULAMA_HORIZONTAL"].any()
    
    # 2. Bu bilgiyi orijinal df'e map et
    mask = df["id"].map(nonzero_ids)
    
    # 3. Maskeyi kullanarak sadece en az bir yıl yük almış id'leri al
    df_TS = df[mask].copy()

    # Hücre - Süper hücre eşleşmesi
    df_TS = df_TS.merge(dfhucre_super, left_on="id", right_on="hucreid", how="left").drop(["hucreid"], axis=1)
    
    dfresult = pd.DataFrame()
    
    sulama_point_load = pd.DataFrame()
    
    for year in range(odtr["ilk_yil"], odtr["son_yil"] + 1):
    
        # Geçmiş yıllarda hiç yük almamış hücreleri bulma (ilk_yil-1 ile year-1 arasında) kısmını kaldırıyoruz.
        # Artık sadece odtr["ilk_yil"]'de yük alan hücreleri alıyoruz.
        if year == odtr["ilk_yil"]:
            # İlk yıl (ilk_yil) için, o yıl yük almış hücreleri al
            mask_sicrama = (df_TS['year'] == year) & (df_TS['TARIMSAL_SULAMA_HORIZONTAL'] != 0)
            ids_sicrayan = df_TS[mask_sicrama]['id'].unique()           
            df.loc[((df["year"] >= year-1) & df["id"].isin(ids_sicrayan)), "TARIMSAL_SULAMA"] = 0  
            
        else:
            # Geçmiş yıllarda hiç yük almamış hücreleri bul
            mask_gecmis = df_TS['year'].between(odtr["ilk_yil"] - 1, year - 1)
            vertical_sum_gecmis = df_TS[mask_gecmis].groupby('id')['TARIMSAL_SULAMA_HORIZONTAL'].sum()
            ids_always_zero = vertical_sum_gecmis[vertical_sum_gecmis == 0].index
    
            # Şu yıl (year) için sıçrama yapanları bul (ilk kez yük alıyor)
            mask_sicrama = (df_TS['year'] == year) & df_TS['id'].isin(ids_always_zero)
            ids_sicrayan = df_TS[mask_sicrama & (df_TS['TARIMSAL_SULAMA_HORIZONTAL'] != 0)]['id'].unique()
        
        # Bu id’lerin year'dan son_yil’a kadar olan tüm verilerini al
        mask_future = (df_TS['id'].isin(ids_sicrayan)) & (df_TS['year'] >= odtr["ilk_yil"] - 1)
        df_future = df_TS[mask_future]
        
        cell_ids = df_future["id"].values
        years = len(cell_ids) * year 
        
        # Point-load formatında çıktı
        tspl = pd.DataFrame({
            "cell_id": ids_sicrayan,
            "year": [year] * len(ids_sicrayan),
            "building_type": ["TARIMSAL_SULAMA"] * len(ids_sicrayan)
        })
    
        sulama_point_load = pd.concat([sulama_point_load, tspl], ignore_index=True)
        dfresult = pd.concat([dfresult, df_future], ignore_index=True)
     
    dfresult = dfresult.reset_index(drop=True)
    
    tsid = sulama_point_load["cell_id"].values
    
    df = df.merge(sulama_point_load, left_on="id", right_on="cell_id", how="left", suffixes=("", "_cutoff"))

    # 1. year < cutoff → 0
    df.loc[df["year"] < df["year_cutoff"], "TARIMSAL_SULAMA_HORIZONTAL"] = 0

    # 2. year > cutoff → SANAYI değerini al
    df.loc[df["year"] >= df["year_cutoff"], "TARIMSAL_SULAMA_HORIZONTAL"] = df.loc[df["year"] >= df["year_cutoff"], "TARIMSAL_SULAMA"]
    
    df.loc[df["year"] >= df["year_cutoff"], "TARIMSAL_SULAMA"] = 0                         
        
    df_TS = df[df["id"].isin(sulama_point_load["cell_id"].values)].copy()
    
    df_TS = df_TS.merge(dfhucre_super, left_on="id", right_on="hucreid", how="left").drop(["hucreid"], axis=1)

    SqliteKaydet(df_TS, f"df_TS_{odtr['ilce']}") 
    ts_parquet = os.path.join(odtr["python_dosya_yolu"], f"TSYUK{odtr['ilce']}.parquet")
    ToParquet(df_TS, ts_parquet)
          
    return df

def KurumYukGuncel2(df,dfEA, dfhucre_super):
    
    PLeklendi = True
    
    odtr = config3.get()
    
    point_load = pd.read_excel(odtr["pointload_path"]) 
    
    bsid = point_load[point_load["building_type"]=="BUYUK_SANAYI"]
    
    if PLeklendi:
        
        df = PLYukAtama(df.copy(), odtr)
    
        df["SANAYI_HORIZONTAL_temp"] = df["SANAYI_HORIZONTAL"]
        
    if not PLeklendi:
        
        df = df.merge(bsid, left_on="id", right_on="cell_id", how="left", suffixes=("", "_cutoff"))
            
        # 1. year < cutoff → 0
        df.loc[df["year"] < df["year_cutoff"], "SANAYI_HORIZONTAL_temp"] = 0
    
        # 2. year > cutoff → SANAYI değerini al
        df.loc[df["year"] >= df["year_cutoff"], "SANAYI_HORIZONTAL_temp"] = df.loc[df["year"] >= df["year_cutoff"], "SANAYI"]
        
        mask = df["year_cutoff"].notna() & (df["year"] >= df["year_cutoff"])
        
        df.loc[mask, "SANAYI"] = 0
            
    df_BS = df[df["id"].isin(bsid["cell_id"].values)].copy()
    
    SqliteKaydet(df_BS, f"df_BS_{odtr['ilce']}")
    bs_parquet = os.path.join(odtr["python_dosya_yolu"], f"BSYUK{odtr['ilce']}.parquet")
    ToParquet(df_BS, bs_parquet)
          
    bstid = point_load[point_load["building_type"]=="BUYUK_TICARETHANE"]
    
    if PLeklendi:
        
        df["TICARETHANE_HORIZONTAL_temp"] = df["TICARETHANE_HORIZONTAL"]
        
    if not PLeklendi:
        
        df = df.drop(["year_cutoff", "cell_id", "building_type"], axis=1)
        
        df = df.merge(bstid, left_on="id", right_on="cell_id", how="left", suffixes=("", "_cutoff"))
    
        # 1. year < cutoff → 0
        df.loc[df["year"] < df["year_cutoff"], "TICARETHANE_HORIZONTAL_temp"] = 0
    
        # 2. year > cutoff → SANAYI değerini al
        df.loc[df["year"] >= df["year_cutoff"], "TICARETHANE_HORIZONTAL_temp"] = df.loc[df["year"] >= df["year_cutoff"], "TICARETHANE"]
        
        df.loc[df["year"] >= df["year_cutoff"], "TICARETHANE"] = 0                         
            
        df_BT = df[df["id"].isin(bstid["cell_id"].values)].copy()
    
        df = df.drop(["year_cutoff", "cell_id", "building_type"], axis=1)
    
    df_BT = df[df["id"].isin(bstid["cell_id"].values)].copy()
    
    #df_BT.to_parquet(f"df_BT.parquet", engine='pyarrow', index=False)
    SqliteKaydet(df_BT, f"df_BT_{odtr['ilce']}")
    bt_parquet = os.path.join(odtr["python_dosya_yolu"], f"BTYUK{odtr['ilce']}.parquet")
    ToParquet(df_BT, bt_parquet)
     
    # mask = df.groupby("id")["TARIMSAL_SULAMA_HORIZONTAL"].transform(lambda x: (x != 0).any())    
    
    # mask = df["id"].map(
    #     df.groupby("id")["TARIMSAL_SULAMA_HORIZONTAL"].apply(lambda x: (x != 0).any())
    # )

    # # 2. Bu maske ile tüm yılları filtrele
    # df_TS = df[mask].copy()
    
    # 1. Her id için TARIMSAL_SULAMA_HORIZONTAL değerlerinin en az biri 0'dan farklı mı?
    nonzero_ids = df.groupby("id")["TARIMSAL_SULAMA_HORIZONTAL"].any()
    
    # 2. Bu bilgiyi orijinal df'e map et
    mask = df["id"].map(nonzero_ids)
    
    # 3. Maskeyi kullanarak sadece en az bir yıl yük almış id'leri al
    df_TS = df[mask].copy()

    # Hücre - Süper hücre eşleşmesi
    df_TS = df_TS.merge(dfhucre_super, left_on="id", right_on="hucreid", how="left").drop(["hucreid"], axis=1)
    
    dfresult = pd.DataFrame()
    
    sulama_point_load = pd.DataFrame()
    
    for year in range(odtr["ilk_yil"] - 1, odtr["son_yil"] + 1):
        # (1) Geçmiş yıllardaki yükler (ilgili yıldan önce)
        mask_past = df_TS['year'] < year
        yük_past = df_TS[mask_past].groupby('id')['TARIMSAL_SULAMA_HORIZONTAL'].sum()
    
        # (2) Şu yılda yük almış hücreleri bul
        mask_current = df_TS['year'] == year
        df_current = df_TS[mask_current]
        yük_current = df_current[df_current['TARIMSAL_SULAMA_HORIZONTAL'] != 0]
    
        # (3) İlk yıl mı? (ilk_yil - 1 yılı özel olarak al)
        if year == odtr["ilk_yil"] - 1:
            ids_sicrayan = yük_current['id'].unique()
        else:
            # Yalnızca geçmişte hiç yük almamış olanları al
            ids_zero_in_past = yük_past[yük_past == 0].index
            ids_sicrayan = yük_current[yük_current['id'].isin(ids_zero_in_past)]['id'].unique()
    
        # (4) Bu hücrelerin ilgili yıldan itibaren tüm yıllardaki verileri
        mask_future = (df_TS['id'].isin(ids_sicrayan)) & (df_TS['year'] >= year)
        df_future = df_TS[mask_future]
    
        # (5) Point-load formatı
        tspl = pd.DataFrame({
            "cell_id": ids_sicrayan,
            "year": [year] * len(ids_sicrayan),
            "building_type": ["TARIMSAL_SULAMA"] * len(ids_sicrayan)
        })
    
        sulama_point_load = pd.concat([sulama_point_load, tspl], ignore_index=True)
        dfresult = pd.concat([dfresult, df_future], ignore_index=True)
        
   # 1. TSNY sütununu oluştur
    df_TS["TSNY"] = df_TS["TARIMSAL_SULAMA_HORIZONTAL"]
    
    # 2. Sadece sulama yapılan cell_id'leri al
    sulama_ids = sulama_point_load["cell_id"].unique()
    
    # 3. Bu hücrelere ait verileri filtrele
    mask = df_TS["id"].isin(sulama_ids)
    df_filtered = df_TS[mask].copy()
    
    # 4. Delta hesapla
    df_filtered.sort_values(by=["id", "year"], inplace=True)
    df_filtered["TSNY"] = df_filtered.groupby("id")["TARIMSAL_SULAMA_HORIZONTAL"].diff().fillna(0)
    
    # 5. Negatif TSNY değerlerini sıfırla
    df_filtered.loc[df_filtered["TSNY"] < 0, "TSNY"] = 0
    
    # 6. ilk_yil - 1 için TSNY = 0 yap
    ilk_yil_eksi_bir = odtr["ilk_yil"] - 1
    df_filtered.loc[df_filtered["year"] == ilk_yil_eksi_bir, "TSNY"] = 0
        
    # 6. Güncellenmiş TSNY'leri ana tabloya aktar
    df_TS.loc[df_filtered.index, "TSNY"] = df_filtered["TSNY"]    
        
    dfresult = dfresult.reset_index(drop=True)
    
    tsid = sulama_point_load["cell_id"].values
    
    df = df.merge(sulama_point_load, left_on="id", right_on="cell_id", how="left", suffixes=("", "_cutoff"))

    # 1. year < cutoff → 0
    df.loc[df["year"] < df["year_cutoff"], "TARIMSAL_SULAMA_HORIZONTAL"] = 0

    # 2. year > cutoff → SANAYI değerini al
    df.loc[df["year"] >= df["year_cutoff"], "TARIMSAL_SULAMA_HORIZONTAL"] = df.loc[df["year"] >= df["year_cutoff"], "TARIMSAL_SULAMA"]
    
    df.loc[df["year"] >= df["year_cutoff"], "TARIMSAL_SULAMA"] = 0                         
        
    df_TS = df[df["id"].isin(sulama_point_load["cell_id"].values)].copy()
    
    df_TS = df_TS.merge(dfhucre_super, left_on="id", right_on="hucreid", how="left").drop(["hucreid"], axis=1)

    SqliteKaydet(df_TS, f"df_TS_{odtr['ilce']}") 
    ts_parquet = os.path.join(odtr["python_dosya_yolu"], f"TSYUK{odtr['ilce']}.parquet")
    ToParquet(df_TS, ts_parquet)
          
    return df

def KurumYukGuncel3(df,dfEA, dfhucre_super):
    
    PLeklendi = True
    
    odtr = config3.get()
    
    point_load = pd.read_excel(odtr["pointload_path"]) 
    
    bsid = point_load[point_load["building_type"]=="BUYUK_SANAYI"]
    
    if PLeklendi:
        
        df = PLYukAtama(df.copy(), odtr)
    
        df["SANAYI_HORIZONTAL_temp"] = df["SANAYI_HORIZONTAL"]
        
    if not PLeklendi:
        
        df = df.merge(bsid, left_on="id", right_on="cell_id", how="left", suffixes=("", "_cutoff"))
            
        # 1. year < cutoff → 0
        df.loc[df["year"] < df["year_cutoff"], "SANAYI_HORIZONTAL_temp"] = 0
    
        # 2. year > cutoff → SANAYI değerini al
        df.loc[df["year"] >= df["year_cutoff"], "SANAYI_HORIZONTAL_temp"] = df.loc[df["year"] >= df["year_cutoff"], "SANAYI"]
        
        mask = df["year_cutoff"].notna() & (df["year"] >= df["year_cutoff"])
        
        df.loc[mask, "SANAYI"] = 0
            
    df_BS = df[df["id"].isin(bsid["cell_id"].values)].copy()
    
    SqliteKaydet(df_BS, f"df_BS_{odtr['ilce']}")
    bs_parquet = os.path.join(odtr["python_dosya_yolu"], f"BSYUK{odtr['ilce']}.parquet")
    ToParquet(df_BS, bs_parquet)
          
    bstid = point_load[point_load["building_type"]=="BUYUK_TICARETHANE"]
    
    if PLeklendi:
        
        df["TICARETHANE_HORIZONTAL_temp"] = df["TICARETHANE_HORIZONTAL"]
        
    if not PLeklendi:
        
        df = df.drop(["year_cutoff", "cell_id", "building_type"], axis=1)
        
        df = df.merge(bstid, left_on="id", right_on="cell_id", how="left", suffixes=("", "_cutoff"))
    
        # 1. year < cutoff → 0
        df.loc[df["year"] < df["year_cutoff"], "TICARETHANE_HORIZONTAL_temp"] = 0
    
        # 2. year > cutoff → SANAYI değerini al
        df.loc[df["year"] >= df["year_cutoff"], "TICARETHANE_HORIZONTAL_temp"] = df.loc[df["year"] >= df["year_cutoff"], "TICARETHANE"]
        
        df.loc[df["year"] >= df["year_cutoff"], "TICARETHANE"] = 0                         
            
        df_BT = df[df["id"].isin(bstid["cell_id"].values)].copy()
    
        df = df.drop(["year_cutoff", "cell_id", "building_type"], axis=1)
    
    df_BT = df[df["id"].isin(bstid["cell_id"].values)].copy()
    
    #df_BT.to_parquet(f"df_BT.parquet", engine='pyarrow', index=False)
    SqliteKaydet(df_BT, f"df_BT_{odtr['ilce']}")
    bt_parquet = os.path.join(odtr["python_dosya_yolu"], f"BTYUK{odtr['ilce']}.parquet")
    ToParquet(df_BT, bt_parquet)
    
    # 1. Her id için TARIMSAL_SULAMA_HORIZONTAL değerlerinin en az biri 0'dan farklı mı?
    nonzero_ids = df.groupby("id")["TARIMSAL_SULAMA"].any()
    
    # 2. Bu bilgiyi orijinal df'e map et
    mask = df["id"].map(nonzero_ids)
    
    # 3. Maskeyi kullanarak sadece en az bir yıl yük almış id'leri al
    df_TS = df[mask].copy()

    # Hücre - Süper hücre eşleşmesi
    df_TS = df_TS.merge(dfhucre_super, left_on="id", right_on="hucreid", how="left").drop(["hucreid"], axis=1)
    
    SqliteKaydet(df_TS, f"df_TS_{odtr['ilce']}") 
    ts_parquet = os.path.join(odtr["python_dosya_yolu"], f"TSYUK{odtr['ilce']}.parquet")
    ToParquet(df_TS, ts_parquet)
          
    return df

#Sütunda PL verisi var mı
def pointLoadVeriDoğrula(columns, PL):
    for column in columns:
        
        if PL in column:
            return True
    
    return False
    
def pointloadHücreFiltrele(df):
    
    mask_bs = (df["SANAYI_POINT_LOAD"] != 0)
    df_filtered = df[mask_bs]
    

    
def YukleriAyir(df, dfhucre_super):
    
    odtr = config3.get()
    df_BS = pd.DataFrame()
    df_BT = pd.DataFrame()

    
    if pointLoadVeriDoğrula(df.columns, "SANAYI_POINT_LOAD"):
        
        df["SANAYI"] -= df["SANAYI_POINT_LOAD"]
        
        mask_bs = (df["SANAYI_POINT_LOAD"] != 0)
        
        df_BS = df[mask_bs]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
        
        SqliteKaydet(df_BS, f"df_BS_{odtr['ilce']}")
        bs_parquet = os.path.join(odtr["python_dosya_yolu"], f"BSYUK{odtr['ilce']}.parquet")
        ToParquet(df_BS, bs_parquet)
        
    if pointLoadVeriDoğrula(df.columns, "TICARETHANE_POINT_LOAD"):
        
        df["TICARETHANE"] -= df["TICARETHANE_POINT_LOAD"]
        
        
        mask_ts = df["TICARETHANE_POINT_LOAD"] != 0
        df_BT = df[mask_ts]
                
        #df_BT.to_parquet(f"df_BT.parquet", engine='pyarrow', index=False)
        SqliteKaydet(df_BT, f"df_BT_{odtr['ilce']}")
        bt_parquet = os.path.join(odtr["python_dosya_yolu"], f"BTYUK{odtr['ilce']}.parquet")
        ToParquet(df_BT, bt_parquet)
    
    # 1. Her id için TARIMSAL_SULAMA_HORIZONTAL değerlerinin en az biri 0'dan farklı mı?
    nonzero_ids = df.groupby("id")["TARIMSAL_SULAMA"].any()
    
    # 2. Bu bilgiyi orijinal df'e map et
    mask = df["id"].map(nonzero_ids)
    
    # 3. Maskeyi kullanarak sadece en az bir yıl yük almış id'leri al
    df_TS = df[mask].copy()

    # Hücre - Süper hücre eşleşmesi
    df_TS = df_TS.merge(dfhucre_super, left_on="id", right_on="hucreid", how="left").drop(["hucreid"], axis=1)
    
    SqliteKaydet(df_TS, f"df_TS_{odtr['ilce']}") 
    ts_parquet = os.path.join(odtr["python_dosya_yolu"], f"TSYUK{odtr['ilce']}.parquet")
    ToParquet(df_TS, ts_parquet)
          
    return df

def EAIslem(df):
    
    odtr = config3.get()
    
    #df = dfEA.copy()
    
    df["toplam_yuk"] = df["toplam_yuk"] / 0.9
    
    df_pivot = df.pivot(index="id", columns="year", values="toplam_yuk").reset_index()
      
    df_pivot = df_pivot.set_index("id")
    
    df_diff = df_pivot.diff(axis=1)
    
    df_diff.iloc[:, 0] = df_pivot.iloc[:, 0]  # 2024 yılı verisini bozmamak için
    
    df_diff = df_diff.reset_index()
    
    df_result = df_diff.melt(id_vars="id", var_name="year", value_name="power_distribution")
    
    df_result = df_result.rename(columns={"id":"ID"})
    
    return df_result
    
def Start7(p, dfinputFile , dftrafo, df_binary):
    
    odtr = config3.get()  
        
    try:
        
        # df = SqliteOku(odtr["yuk_db_adi"])
        logging.info(f"YUK VERISI OKUNUYOR...")
        
        db_path = odtr.get("yuk_db_yolu", "")
        df = SqliteOkuYuk(db_path, odtr["yuk_db_adi"])
        
        # df = ReadParquet(odtr["yuk_path_parquet"])
        dfEA = SqliteOkuYuk(odtr["EAVeritabani"], odtr["EAVeri_db_adi"])
        
        logging.info(f"YUK VERISI OKUNDU...")
    except Exception as e:
        logging.error(e)
        sys.exit(1)
            

    # dfhucre = df[df["year"] == odtr["ilk_yil"]-1] # bütün yıllara ait hücreler aynı zaten    
    
    # dfhucre= dfhucre.iloc[:,:6].copy()
    
    dfhucre = pd.read_csv(os.path.join(odtr["hucre_path"], f"{odtr['ilce']}_grid.csv"), encoding="windows-1254")
        
    ToParquet(dfhucre, odtr["df_hucre_parquet"])
        
    dfhucre["year"] = df["year"].copy()
    
    logging.info(f"SUPER HUCRE BULUNUYOR...")
    
    results = SuperHucreBul(dfhucre)        
    
    logging.info(f"SUPER HUCRE BULUNDU...")
    # 2025'ten 2035'e kadar olan yılları ekle
    years_to_add = list(range(odtr["ilk_yil"], odtr["son_yil"]+1))
    
    # Yeni DataFrame oluştur
    new_rows = []
    
    for year in years_to_add:
        df_year = results[odtr["ilk_yil"]-1][0].copy()
        df_year['year'] = year
        new_rows.append(df_year)
    
    # Yılları ekle ve yeni DataFrame oluştur
    dfsuperhucre = pd.concat([results[odtr["ilk_yil"]-1][0]] + new_rows, ignore_index=True)
    
    #dfsuperhucre.to_parquet(f"dfsuperhucre_{odtr['ilce']}.parquet", engine='pyarrow', index=False)
    ToParquet(dfsuperhucre, odtr["dfsuperhucre"])
    
    dfhucre_super = results[odtr["ilk_yil"]-1][1]
    
    #dfhucre_super.to_parquet(f"dfhucre_super_{odtr['ilce']}.parquet", engine='pyarrow', index=False)
    ToParquet(dfhucre_super, odtr["dfhucre_super"])
    
    #dfsuperhucre = pd.read_parquet(f"dfsuperhucre_{odtr['ilce']}.parquet", engine='pyarrow')  # veya engine='fastparquet'
    dfsuperhucre = ReadParquet(odtr["dfsuperhucre"])
    
    #dfhucre_super = pd.read_parquet(f"dfhucre_super_{odtr['ilce']}.parquet", engine='pyarrow')  # veya engine='fastparquet'
    dfhucre_super = ReadParquet(odtr["dfhucre_super"])

    cols = ['MESKEN', 'SANAYI', 'TICARETHANE', 'AYDINLATMA', 'TARIMSAL_SULAMA']
    
    # power_zero_all_years = df.groupby(['id'])[cols].apply(lambda x: (x == 0).all().all())
    
    power_zero_all_years = (df[cols] == 0).groupby(df['id']).all().all(axis=1)
    
    dft = pd.read_csv(odtr['trafo_path'], encoding='utf-8-sig', sep=',')  # Daha hızlı olması için openpyxl kullanılır
    # dft = pd.read_excel(odtr['trafo_path'], engine="openpyxl")

    # trafo_merkez_id değerlerini set olarak alalım (hızlı arama için)
    trafo_set = set(dft["merkez_hucre"].values)
    
    hepsiSıfır = dfEA.groupby("id")["toplam_yuk"].apply(lambda x: (x==0).all())
    
    power_to_drop = hepsiSıfır[hepsiSıfır].index.tolist()
    
    dfEAids = dfEA[~dfEA['id'].isin(power_to_drop)]["id"]
    
    power_to_drop = power_zero_all_years[power_zero_all_years].index.tolist()

    # power_to_drop listesinden, trafo_set içinde olanları çıkar
    dfEAids = set(dfEAids)
    
    # power_to_drop listesi, diğer setlerle fark alınıyor
    power_to_drop = [pid for pid in power_to_drop if pid not in trafo_set and pid not in dfEAids]

    df = df[~df['id'].isin(power_to_drop)]
    
    # dfhucre = df[df["year"] == odtr["ilk_yil"]-1] # bütün yıllara ait hücreler aynı zaten    
    dfhucre = dfhucre[~dfhucre["id"].isin(power_to_drop)]
        
    ToParquet(dfhucre, odtr["df_hucre_parquet"])

    dfhucre_super = dfhucre_super[~dfhucre_super["hucreid"].isin(power_to_drop)]
    
    dfsuperhucre = dfsuperhucre[dfsuperhucre["shid"].isin(dfhucre_super["shid"])]
    
    #dfsuperhucre.to_parquet(f"dfsuperhucre_{odtr['ilce']}.parquet", engine='pyarrow', index=False)
    ToParquet(dfsuperhucre, odtr["dfsuperhucre"])
    
    #dfhucre_super.to_parquet(f"dfhucre_super_{odtr['ilce']}.parquet", engine='pyarrow', index=False)
    ToParquet(dfhucre_super, odtr["dfhucre_super"])
    
    df = YukleriAyir(df, dfhucre_super)
    # df = KurumYukGuncel3(df, dfEA, dfhucre_super)
    
    # df = df.merge(dfhucre_super, left_on="id", right_on="hucreid", how="left")
    
    trafo_alanlari = SqliteOku(f"trafo_alanlari_{odtr['ilce']}")
    #trafo_alanlari = ReadParquet(odtr["trafo_alanlari_parquet"])
    
    mask = trafo_alanlari[trafo_alanlari["kent_disi_alan"]==0]["hucre_id"].values
    
    df = df[df["id"].isin(mask)]
    
    dfdouble_superhucre = DoubleSuperHucre(dfsuperhucre[dfsuperhucre["year"]==odtr["ilk_yil"]-1])
    
    # **1. Eşleşmeyi Doğru Yap**
    df_merged = df.merge(dfhucre_super, left_on="id", right_on="hucreid", how="left").drop(columns=["hucreid"])

    # **2. Sayısal olmayan sütunları düzelt ve NaN'leri 0 yap**
    columns_to_sum = ['MESKEN', 'SANAYI', 'TICARETHANE', 'AYDINLATMA']
    df_merged[columns_to_sum] = df_merged[columns_to_sum].apply(pd.to_numeric, errors='coerce').fillna(0)

    # **3. Her Süper Hücre için ilgili sütunları topla**
    superhucre_aggregated = df_merged.groupby(["shid", "year"])[columns_to_sum].sum().reset_index()

    # **4. Tüm tüketim sütunlarının toplamını hesapla (Yeni sütun: `TOTAL_POWER`)**
    superhucre_aggregated["TOTAL_POWER"] = superhucre_aggregated[columns_to_sum].sum(axis=1)

    dfsuperhucre = dfsuperhucre.merge(superhucre_aggregated, on=["shid", "year"], how="left")
    
    # Eğer her değer 1 saatlik yükü temsil ediyorsa (t = 1 saat)
    dfsuperhucre["TOTAL_POWER"] = (dfsuperhucre["TOTAL_POWER"] / p["toplam_saat"]) * p["toplam_saat_katsayi"]
    
    # Pivot tablo oluştur (shid bazında yılları kolon yap)
    df_pivot = dfsuperhucre.pivot(index="shid", columns="year", values="TOTAL_POWER").reset_index()
    
    # Yıllık değişimi hesapla (fark alma işlemi)
    df_delta = df_pivot.copy()
    df_delta.iloc[:, 1:] = df_pivot.iloc[:, 1:].diff(axis=1)  # İlk sütun hariç fark al
    
    # İlk yıl olan 2024'ü orijinal değeriyle koruyalım
    df_delta.iloc[:, 1] = df_pivot.iloc[:, 1]  # 2024 yılı verisini bozmamak için
    
    df_result = df_delta.melt(id_vars="shid", var_name="year", value_name="delta_TOTAL_POWER")

    dfsuperhucre = dfsuperhucre.merge(df_result, on=["shid", "year"], how="left")
    
    dfsuperhucre["delta_TOTAL_POWER_KVA"] = dfsuperhucre["delta_TOTAL_POWER"] / p["KVA_cevirme"]
    
    dfsuperhucre = dfsuperhucre.dropna(axis=0)
    
    SonucDF = SkorHesapla4(p, dfhucre_super, dftrafo, df_binary, dfsuperhucre, dfdouble_superhucre)

    
    return SonucDF, dfdouble_superhucre

def SkorHesapla4(p, dfhucre_super, dftrafocopy, df_binary, dfsuperhucre, dfdouble_superhucre, ilk=True):
    
    odtr = config3.get()
    logging.info(f"SUPER HUCRE SKOR HESAPLAMA BASLIYOR...")

    years = list(range(odtr["ilk_yil"]-1, odtr["son_yil"]+1))
    
    dfhucre_supertest = dfhucre_super.copy()
    
    dftrafo = dftrafocopy.copy()
    
    negatif_efektif_bos_kapasiteler = dftrafo[dftrafo["efektif_bos_kapasite"] < 0]["trafo_id"].values
    
    dftrafo.loc[dftrafo["trafo_id"].isin(negatif_efektif_bos_kapasiteler), "efektif_bos_kapasite"] = 0
    
    # Süper hücrelerde toplam boş kapasiteyi hesapla
    if not "shid" in dftrafo.columns:
        dftrafo_superhucre = dfhucre_supertest.merge(dftrafo, left_on="hucreid", right_on="merkez_hucre", how="inner")
        
        df_sonuclar_kapasite = dftrafo_superhucre.groupby(["shid"])[["efektif_bos_kapasite"]].sum().reset_index()
    
    else:
        df_sonuclar_kapasite = dftrafo.groupby(["shid"])[["efektif_bos_kapasite"]].sum().reset_index()  
        
    trafo_bulunan_shler = df_sonuclar_kapasite["shid"].values

    #df_sonuclar_kapasite = df_sonuclar_kapasite[df_sonuclar_kapasite["sahip"]=="Kurum"]
    
    # df_binary = df_binary.astype({col: 'int64' for col in df_binary.select_dtypes(include='number').columns})
    
    df_binary["hucre_id"] = df_binary["hucre_id"].astype("int64")

    df_birlesik = dfhucre_supertest.merge(df_binary, left_on="hucreid", right_on="hucre_id", how="left")  # Hücre ID üzerinden birleştir
    
    # for col in df_birlesik.columns:
    #     if col not in ["hucreid", "hucre_id", "shid"]:
    #         df_birlesik[col] = pd.to_numeric(df_birlesik[col], errors='coerce')
    
    df_toplamlar = df_birlesik.loc[:, ~df_birlesik.columns.isin(["hucreid", "hucre_id"])].groupby("shid").sum().reset_index()
    
    df_toplamlar2 = df_toplamlar.merge(df_sonuclar_kapasite, on="shid", how="left")
    
    df_toplamlar2.loc[df_toplamlar2["efektif_bos_kapasite"].isna(), "efektif_bos_kapasite"] = 0

    #df_toplamlar2.loc[df_toplamlar2["sahip"].isna(), "sahip"] = "KurumDegil"

    df_toplamlar2 = df_toplamlar2.merge(dfsuperhucre[["shid", "year", "TOTAL_POWER", "delta_TOTAL_POWER", "delta_TOTAL_POWER_KVA"]], on="shid", how="left")
    
    df_toplamlar2["belediye_hizmet"] = df_toplamlar2["belediye_hizmet"] + df_toplamlar2["otopark"] + df_toplamlar2["cami"] + df_toplamlar2["pazar_alani"]
    
    
    # **2. Formülde kullanılan değerleri hesapla**
    df_toplamlar2["calc_value"] = (
        df_toplamlar2["efektif_bos_kapasite"] * p["boskapasite_katsayisi"] +
        df_toplamlar2["rezerve_trafo"] * p["rezerv_katsayisi"] +
        df_toplamlar2["park"] * p["park_katsayisi"] +
        df_toplamlar2["belediye_hizmet"] * p["belediye_katsayisi"]

    )
    
    dfsuperProcess = df_toplamlar2.copy()
    # **1. dfdouble_superhucre ile dfsupertest'i eşleştir**

    dfsuperProcess = dfsuperProcess.merge(dfdouble_superhucre, left_on=["shid"], right_on="dshid", how="left")

    # **3. Sonuçları depolamak için yeni sütun ekleyelim (Kümülatif toplama için)**
    dfsuperProcess["power_distribution"] = 0.0
    
    columns_to_add = ["superhucrenorthid", "superhucresouthid", "superhucreeastid", "superhucrewestid"]
        
    results = {}

    for year, df_year in dfsuperProcess.groupby("year"):                   
        
        df_year = df_year.copy()
    
        # **1. Komşuluk değerlerini hesapla**
        neighbor_cols = ["superhucrenorthid", "superhucresouthid", "superhucreeastid", "superhucrewestid"]
        neighbors = ["north", "south", "east", "west"]
        trafo_set = set(df_sonuclar_kapasite["shid"])
        
        for direction in ["merkez", "north", "south", "east", "west"]:
            if direction == "merkez":
                colname = f"shid"
            
            else:
                colname = f"superhucre{direction}id"
            
            trafo_col = f"trafo_{direction}"
            
            # Komşu hücrelerin shid'lerine göre trafoVarMi bilgisi al
            df_year[trafo_col] = df_year[colname].apply(lambda x: x in trafo_set)
            
        neighbor_values = df_year[neighbor_cols].apply(lambda x: df_year.set_index("shid").reindex(x).calc_value.values, axis=0)
        neighbor_values = (neighbor_values.fillna(0) * p["komsuluk_katsayisi_diger"])
        
        for direction in ["north", "south", "east", "west"]:
            
            neighbor_col = f"superhucre{direction}id"
            
            df_year[f"{direction}_skor"] = neighbor_values[neighbor_col]
 
        
        df_year["merkez_skor"] = (p["komsuluk_katsayisi"] * df_year["calc_value"]) + p["skor_sabiti"]
        
        aktif_mask = df_year["delta_TOTAL_POWER_KVA"] > p["degisim_limiti"] 
        
        # Bu maskeye sahip olan hücrelerin komşuluk katkısını sıfırla
        for direction in ["north", "south", "east", "west"]:
            neighbor_col = f"superhucre{direction}id"
            trafo_col = f"trafo_{direction}"                      
            
            df_year[f"{direction}_skor"] = np.where(
                aktif_mask & (~df_year[trafo_col]),  # delta büyük VE komşuda trafo yok
                0,
                df_year[f"{direction}_skor"]
            )
        
        # KURAL 1: Değişim limiti altında kalan ve trafo bulunan hücreler
        mask1 = (df_year["delta_TOTAL_POWER_KVA"] <= 0)
        
        mask1_ = (df_year["delta_TOTAL_POWER_KVA"] > 0) & (df_year["delta_TOTAL_POWER_KVA"] <= p["degisim_limiti"]) & df_year["trafo_merkez"]
        
        for direction in ["north", "south", "east", "west"]:
            skor_col = f"{direction}_skor"
            df_year[skor_col] = np.where(mask1, 0, df_year[skor_col])
            df_year[skor_col] = np.where(mask1_, 0, df_year[skor_col])
        
        # KURAL 2: Değişim limiti altında kalan, trafo olmayan ama trafolu komşusu olan hücreler
        mask2 = (
            (df_year["delta_TOTAL_POWER_KVA"] > 0) &
            (df_year["delta_TOTAL_POWER_KVA"] <= p["degisim_limiti"]) &
            (~df_year["trafo_merkez"]) &
            df_year[["trafo_{}".format(col) for col in ["north", "south", "east", "west"]]].any(axis=1)
        )
        
        mask2_ = (
            (df_year["delta_TOTAL_POWER_KVA"] > 0) &
            (df_year["delta_TOTAL_POWER_KVA"] <= p["degisim_limiti"]) &
            (~df_year["trafo_merkez"]) &
            (df_year[["trafo_north", "trafo_south", "trafo_east", "trafo_west"]].eq(False).all(axis=1))
        )
        
        # 
        '''for idx in df_year[mask2_].index:
            
            df_year.at[idx, "merkez_skor"] = 0

            for direction in ["north", "south", "east", "west"]:
                
                skor_col = f"{direction}_skor"
                
                if not df_year.at[idx, skor_col]:
                    df_year.at[idx, skor_col] = 0
                    
                else:
                    df_year.at[idx, skor_col] = p["skor_sabiti"]'''
                
                
        # Sadece mask2 sağlayan hücrelerde işlem yapılacak
        for idx in df_year[mask2].index:
            trafo_scores = {}
            
            for direction in ["north", "south", "east", "west"]:
                trafo_col = f"trafo_{direction}"
                skor_col = f"{direction}_skor"
                kapasite_col = f"efektif_bos_kapasite"
        
                if df_year.at[idx, trafo_col]:  # Eğer o yönde trafo varsa
                    trafo_scores[direction] = df_year.at[idx, kapasite_col]
            
            # Eğer trafolu komşu varsa en yüksek kapasiteli olanı bul
            if trafo_scores:
                keep_direction = max(trafo_scores, key=trafo_scores.get)
                
                df_year.at[idx, "merkez_skor"] = 0
                
                for direction in ["north", "south", "east", "west"]:
                    skor_col = f"{direction}_skor"
                    if direction != keep_direction:
                        df_year.at[idx, skor_col] = 0
                        
                    else:
                        df_year.at[idx, skor_col] = p["skor_sabiti"]
                  
    
        df_year["toplam_skor"] = (
        df_year["merkez_skor"] +
        df_year[["north_skor", "south_skor", "east_skor", "west_skor"]].sum(axis=1)
        )
        
        ###KURAL3
        df_year["threshold"] = np.where(
            df_year["delta_TOTAL_POWER_KVA"] != 0,
            (df_year["toplam_skor"] / df_year["delta_TOTAL_POWER_KVA"]) * p["skor_bolu_yuk_katsayi"],
            0
        )
        
        for idx in df_year[mask2].index:
            
            df_year.at[idx, "threshold"] = df_year.loc[idx, ["north_skor", "south_skor", "east_skor", "west_skor"]].max()
                    
        #excelKaydet(df_year, f"isim{datetime.now().strftime('%Y%m%d_%H%M%S')}")
      
        # **4. Threshold üzerinde olanları filtrele**
        
        df_year["yeni_merkez_skor"] = np.where(df_year["merkez_skor"] >= df_year["threshold"], df_year["merkez_skor"], 0)
        
        for direction in ["north", "south", "east", "west"]:
            df_year[f"yeni_{direction}_skor"] = np.where(df_year[f"{direction}_skor"] >= df_year["threshold"], df_year[f"{direction}_skor"], 0)            
        
        # Eğer süper hücrenin bütün 'filtered' değerleri 0 ise, power_distribution'a TOTAL_POWER ekle
        zero_mask = (df_year[["yeni_merkez_skor", 
                                   "yeni_north_skor", 
                                   "yeni_south_skor", 
                                   "yeni_east_skor", 
                                   "yeni_west_skor"]] == 0).all(axis=1)
        
        df_year.loc[zero_mask, "yeni_merkez_skor"] = df_year.loc[zero_mask, "merkez_skor"]
        
        df_year["yeni_toplam_skor"] = (
        df_year["yeni_merkez_skor"] +
        df_year[["yeni_north_skor", "yeni_south_skor", "yeni_east_skor", "yeni_west_skor"]].sum(axis=1)
        )
        
        df_year["merkez_yuzde"] = np.where(df_year["yeni_toplam_skor"] > 0, df_year["yeni_merkez_skor"] / df_year["yeni_toplam_skor"], 0)
        
        for direction in ["north", "south", "east", "west"]:
            df_year[f"{direction}_yuzde"] = np.where(df_year["yeni_toplam_skor"] > 0, df_year[f"yeni_{direction}_skor"] / df_year["yeni_toplam_skor"], 0)
              
        # **7. Power Transfer Hesaplamaları**
        df_year["power_distribution"] = df_year["delta_TOTAL_POWER_KVA"] * df_year["merkez_yuzde"]
        for direction in ["north", "south", "east", "west"]:
            df_year[f"{direction}_transfer"] = df_year["delta_TOTAL_POWER_KVA"] * df_year[f"{direction}_yuzde"]
            
            
        # Eğer süper hücrenin bütün 'filtered' değerleri 0 ise, power_distribution'a TOTAL_POWER ekle
        zero_mask = (df_year[["yeni_merkez_skor", 
                                   "yeni_north_skor", 
                                   "yeni_south_skor", 
                                   "yeni_east_skor", 
                                   "yeni_west_skor"]] == 0).all(axis=1)
        
        #df_year.loc[zero_mask, "yeni_merkez_skor"] = 1
        #df_year.loc[zero_mask, "power_distribution"] = df_year["delta_TOTAL_POWER_KVA"]
        
        # **8. Komşulara aktarılan gücü vektörel olarak ekle**
        transfer_df = df_year.melt(id_vars=["shid"], value_vars=[f"{col}_transfer" for col in neighbors], 
                                   var_name="direction", value_name="transferred_power")
        
        # **Komşu sütun ismini alarak neighbor_shid belirle**
        transfer_df["neighbor_type"] = transfer_df["direction"].str.replace("_transfer", "")

        # **Komşu `shid` bilgilerini ekleyelim**
        neighbor_mapping = df_year.set_index("shid")[neighbor_cols].stack().reset_index()
        neighbor_mapping.columns = ["shid", "neighbor_type", "neighbor_shid"]
        
        neighbor_mapping["neighbor_type"] = neighbor_mapping["neighbor_type"].str.extract(r"superhucre(.*)id")

        # **Eksik komşuları kontrol edelim**
        if neighbor_mapping["neighbor_shid"].isna().sum() > 0:
            print("Warning: Some neighbor_shid values are NaN. These will be ignored.")

        # **Komşuları `merge()` ile bağlayalım**
        transfer_df = transfer_df.merge(neighbor_mapping, on=["shid", "neighbor_type"], how="left")

        # **Geçersiz komşuları çıkar (NaN olanları)**
        transfer_df = transfer_df.dropna(subset=["neighbor_shid"])

        logging.info(f"{year}...")

        # **Transfer edilen yükleri gruplama**
        power_transfer_summary = transfer_df.groupby(["neighbor_shid", "neighbor_type"])["transferred_power"].sum().reset_index()
        
        # **Yönlere göre pivot tablosu oluştur**
        '''power_transfer_pivot = power_transfer_summary.pivot(index="neighbor_shid", columns="neighbor_type", values="transferred_power").fillna(0)
        
        # **Sütun isimlerini uygun hale getirme**
        power_transfer_pivot.columns = [f"transfer_from_{col}" for col in power_transfer_pivot.columns]'''
        
        # **Yönleri tersine çeviren bir fonksiyon oluşturma**
        
        def reverse_direction(direction):
            """Verilen yönün tersini döndüren fonksiyon"""
            reverse_map = {
                "north": "south",
                "south": "north",
                "east": "west",
                "west": "east"
            }
            return reverse_map.get(direction, direction)  # Eğer yön tanımlanmamışsa, olduğu gibi döndür
        
        # **Yönlere göre pivot tablosu oluştur**
        power_transfer_pivot = power_transfer_summary.pivot(index="neighbor_shid", columns="neighbor_type", values="transferred_power").fillna(0)
        
        # **Sütun isimlerini ters yönlere göre uygun hale getirme**
        power_transfer_pivot.columns = [f"transfer_from_{reverse_direction(col)}" for col in power_transfer_pivot.columns]

        # **Komşu hücrelerden gelen yükleri df_year ile birleştir**
        #df_year = df_year.merge(power_transfer_pivot, left_on="shid", right_index=True, how="left").fillna(0)
        df_year = df_year.merge(power_transfer_pivot, left_on="shid", right_index=True, how="left").infer_objects(copy=False)
        
        # **Toplam transfer edilen yükü hesaplama**
        df_year["transfer_sum"] = df_year[[col for col in df_year.columns if col.startswith("transfer_from_")]].sum(axis=1)
        # **Gelen yükleri power_distribution'a ekleme**
        df_year["power_distribution"] += df_year["transfer_sum"]
              
        results[year] = df_year        

    if not ilk:
        
        dfRes = pd.concat([results[y] for y in years[1:]])
        
    else:
        dfRes = pd.concat([results[y] for y in years])

    #dfRes.to_excel("Resrt6.xlsx", engine="openpyxl")
    
    logging.info(f"SUPER HUCRE SKOR HESAPLAMA BITTI...")

    #path_parquet = os.path.join(odtr['python_dosya_yolu'], f"dfYuk_{ilce}.parquet")
    
    #ToParquet(dfRes, odtr['yuk_parquet'])

    return dfRes

def ExcelToParquet(path, save_path=""):
    
    df = pd.read_excel(path)
    
    ToParquet(df, save_path)

def excelKaydet(df, isim):
    
    odtr = config3.get()
    
    path = os.path.join(odtr["sonuc_yolu"], f"{isim}{datetime.now().strftime('%Y%m%d_%H%M%S')}.xlsx")
    
    df.to_excel(path, engine="openpyxl")
    
def e(df, name="test"):
    
    odtr = config3.get()
    
    path = os.path.join(f"{name}{datetime.now().strftime('%Y%m%d_%H%M%S')}.xlsx")
    
    df.to_excel(path, engine="openpyxl")

def SqliteOku(tablo_adi):
    """
    SQLite veritabanından belirtilen tabloyu güvenli şekilde okur.
    """
    try:
        odtr = config3.get()
        db_path = os.path.join(odtr.get("dosyalar", ""), "veriler.db")

        if not os.path.isfile(db_path):
            raise FileNotFoundError(f"Veritabanı bulunamadı: {db_path}")
        
        # Bağlantıyı context manager ile aç
        with sqlite3.connect(db_path) as conn:
            # Tablo adı SQL injection'a karşı güvenli olmadığından kontrol et
            if not tablo_adi.isidentifier():
                raise ValueError(f"Geçersiz tablo adı: {tablo_adi}")
            
            query = f"SELECT * FROM {tablo_adi}"
            df = pd.read_sql(query, conn)

        return df
    
    except Exception as e:
        print(f"[HATA] '{tablo_adi}' tablosu okunamadı: {e}")
        return pd.DataFrame()  # Hatalıysa boş DataFrame döndür

def SqliteOkuYuk(db_path, tablo_adi):
    """
    SQLite veritabanından belirtilen tabloyu güvenli şekilde okur.
    """
    try:
        odtr = config3.get()

        if not os.path.isfile(db_path):
            raise FileNotFoundError(f"Veritabanı bulunamadı: {db_path}")
        
        # Bağlantıyı context manager ile aç
        with sqlite3.connect(db_path) as conn:
            # Tablo adı SQL injection'a karşı güvenli olmadığından kontrol et
            if not tablo_adi.isidentifier():
                raise ValueError(f"Geçersiz tablo adı: {tablo_adi}")
            
            query = f"SELECT * FROM {tablo_adi}"
            df = pd.read_sql(query, conn)

        return df
    
    except Exception as e:
        print(f"[HATA] '{tablo_adi}' tablosu okunamadı: {e}")
        return pd.DataFrame()  # Hatalıysa boş DataFrame döndür


def SqliteKaydet(df, table_name):
    odtr = config3.get()
    db_name = os.path.join(odtr["dosyalar"], "veriler.db")
    
    with sqlite3.connect(db_name) as conn:
        cursor = conn.cursor()
        cursor.execute(f"DROP TABLE IF EXISTS {table_name}")
        conn.commit()  # commit et
        df.to_sql(table_name, conn, if_exists="replace", index=False)
        cursor.close()
    
def SqliteKaydet2(db_name, df, table_name):
    odtr = config3.get()
    
    with sqlite3.connect(db_name) as conn:
        cursor = conn.cursor()
        cursor.execute(f"DROP TABLE IF EXISTS {table_name}")
        conn.commit()  # commit et
        df.to_sql(table_name, conn, if_exists="replace", index=False)
        cursor.close()   

def SuperHucreBul(dfhucreAll):
    results = {}

    for year, dfhucre in dfhucreAll.groupby("year"):
        dfhucre = dfhucre.sort_values(by=["bottom", "left"], ascending=[True, True]).reset_index(drop=True)

        columns_per_super = 4
        rows_per_super = 3
        epsilon = 1e-6

        cell_width = (dfhucre["right"] - dfhucre["left"]).mode()[0]
        cell_height = (dfhucre["top"] - dfhucre["bottom"]).mode()[0]

        super_width = columns_per_super * cell_width
        super_height = rows_per_super * cell_height

        dfhucre["center_x"] = (dfhucre["left"] + dfhucre["right"]) / 2
        dfhucre["center_y"] = (dfhucre["bottom"] + dfhucre["top"]) / 2

        unique_lefts = sorted(dfhucre["left"].unique())
        unique_bottoms = sorted(dfhucre["bottom"].unique())

        num_i = len(unique_lefts) // columns_per_super
        num_j = len(unique_bottoms) // rows_per_super

        superhucre_list = []
        superhucre_hucre_mapping = []
        shid = 1

        for i, super_left in enumerate(unique_lefts[::columns_per_super]):
            for j, super_bottom in enumerate(unique_bottoms[::rows_per_super]):
                j_reversed = num_j - j - 1  # Alt satır 0 olacak şekilde tersine çeviriyoruz

                super_right = super_left + super_width
                super_top = super_bottom + super_height

                hucre_grubu = dfhucre[
                    (dfhucre["center_x"] >= super_left - epsilon) & (dfhucre["center_x"] <= super_right + epsilon) &
                    (dfhucre["center_y"] >= super_bottom - epsilon) & (dfhucre["center_y"] <= super_top + epsilon)
                ]

                if hucre_grubu.empty:
                    continue

                # Artık i ve j değerleriyle birlikte kayıt yapılıyor
                superhucre_list.append([
                    shid, year, super_left, super_top, super_right, super_bottom, i, j_reversed
                ])

                for h_id in hucre_grubu["id"]:
                    superhucre_hucre_mapping.append([shid, h_id])

                shid += 1

        dfsuperhucre = pd.DataFrame(
            superhucre_list,
            columns=["shid", "year", "left", "top", "right", "bottom", "i", "j"]
        )
        dfhucre_super = pd.DataFrame(superhucre_hucre_mapping, columns=["shid", "hucreid"])

        results[year] = dfsuperhucre, dfhucre_super

    return results

def DoubleSuperHucre(dfsuperhucre):
    # **DoubleSuperhucre listesi oluştur**
    double_superhucre_list = []

    # **Süper Hücreleri (shid, i, j) tablosuna çevirelim**
    superhucre_dict = {(row["i"], row["j"]): row["shid"] for _, row in dfsuperhucre.iterrows()}

    # **Her süper hücre için komşularını belirleyelim**
    for _, row in dfsuperhucre.iterrows():
        center_id = row["shid"]
        i, j = row["i"], row["j"]

        # Komşuları belirle
        north_id = superhucre_dict.get((i, j - 1), None)  # Yukarıdaki komşu
        south_id = superhucre_dict.get((i, j + 1), None)  # Aşağıdaki komşu
        west_id = superhucre_dict.get((i - 1, j), None)   # Solundaki komşu
        east_id = superhucre_dict.get((i + 1, j), None)   # Sağındaki komşu

        # Double Super Hücreyi ekle
        double_superhucre_list.append([center_id, north_id, south_id, west_id, east_id])

    # **Double Super Hücre DataFrame'i oluştur**
    dfdouble_superhucre = pd.DataFrame(double_superhucre_list, columns=[
        "dshid", "superhucrenorthid", "superhucresouthid", "superhucrewestid", "superhucreeastid"
    ])
    
    return dfdouble_superhucre