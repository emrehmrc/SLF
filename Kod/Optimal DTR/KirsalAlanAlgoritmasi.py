# -*- coding: utf-8 -*-
"""
Created on Tue Apr  8 12:49:31 2025

@author: vural.bayrakli
"""
import pandas as pd
import numpy as np
import pickle
import config3
from Functions3 import YukAta4_3, SqliteKaydet, SqliteOku, yeni_trafo_df_olustur
import logging
import time
from joblib import Parallel, delayed
from collections import defaultdict

def komsulari_bul(i, j, mesafe=1, capraz_dahil=True):
    komsu_listesi = []
    
    for di in range(-mesafe, mesafe + 1):
        for dj in range(-mesafe, mesafe + 1):
            if di == 0 and dj == 0:
                continue  # Kendisi hariç
            if not capraz_dahil and abs(di) + abs(dj) != 1:
                continue  # Çapraz hariçse sadece 4 yön
            komsu_listesi.append((i + di, j + dj))
    
    return komsu_listesi

def process_shid_group(shid, group, dftrafoYıllık, dftrafocopy, dfsuper, hat_uzunluk, ilk_atama=True):
    
    trafolar = pd.DataFrame()
    
    if ilk_atama:
        trafolarYillik = dftrafoYıllık[dftrafoYıllık["shid"] == shid].copy()
        trafolar = dftrafocopy[dftrafocopy["shid"] == shid].copy()
        
    else:
        # dftrafoYıllık = ka.trafoYıllık.copy()
        # dftrafocopy = ka.trafo.copy()
        trafolar = dftrafocopy[(dftrafocopy["shid"] == shid) & (dftrafocopy["Trafo Bölgesi"] == "Kırsal") & (dftrafocopy["Trafo Aksiyon"] == "yeni trafo tesis")].copy()
        trafolarYillik = dftrafoYıllık[dftrafoYıllık["trafo_id"].isin(trafolar["trafo_id"].values)].copy()
        
    if trafolar.empty:
        
        sh = dfsuper[dfsuper["shid"] == shid]
        if sh.empty:
            return (shid, pd.DataFrame(), pd.DataFrame(), group)

        i, j = sh.iloc[0]["i"], sh.iloc[0]["j"]
        komsular = komsulari_bul(i, j, mesafe=hat_uzunluk, capraz_dahil=True)
        komsular_set = set(komsular)

        komsu_satirlar = dfsuper[dfsuper[["i", "j"]].apply(tuple, axis=1).isin(komsular_set)]
        komsu_shids = komsu_satirlar["shid"].tolist()
        trafolar_komsu = dftrafocopy[dftrafocopy["shid"].isin(komsu_shids)].copy()

        if not trafolar_komsu.empty:
            try:
                
                if ilk_atama:
                    
                    secilen_idx = trafolar_komsu["efektif_bos_kapasite"].idxmax()
                    secilen_shid = trafolar_komsu.loc[secilen_idx, "shid"]
                    trafolar = dftrafocopy[dftrafocopy["shid"] == secilen_shid].copy()
                    trafolarYillik = dftrafoYıllık[dftrafoYıllık["shid"] == secilen_shid].copy()
                
                else:
                    
                    trafolar_komsu = trafolar_komsu[(trafolar_komsu["Trafo Aksiyon"] == "yeni trafo tesis") & (trafolar_komsu["Trafo Bölgesi"] == "Kırsal")]
                    trafolar = dftrafocopy[(dftrafocopy["trafo_id"].isin(trafolar_komsu["trafo_id"].values))].copy()
                    trafolarYillik = dftrafoYıllık[dftrafoYıllık["trafo_id"].isin(trafolar["trafo_id"].values)].copy()

            except Exception:
                trafolar = pd.DataFrame(columns=dftrafocopy.columns)
                trafolarYillik = pd.DataFrame(columns=dftrafoYıllık.columns)
        else:
            trafolar = pd.DataFrame(columns=dftrafocopy.columns)
            trafolarYillik = pd.DataFrame(columns=dftrafoYıllık.columns)

    return (shid, trafolarYillik, trafolar, group)


def process_shid(shid, group, dftrafoY_shid, dftrafocopy):
    
    return YukAta4_3(group, dftrafocopy, dftrafoY_shid)


def Paralelİslem(df_result1, dftrafoYıllık, dftrafocopy, dfsuper, p):
    
    hat_uzunluk = p["hat_uzunluk"]
    
    groups = [
    (shid, group)
    for (shid, group) in df_result1.groupby(["shid"])
    ]
    
    # Paralel çalıştır
    groups2 = Parallel(n_jobs=6)(
        delayed(process_shid_group)(shid, group, dftrafoYıllık, dftrafocopy, dfsuper, hat_uzunluk)
        for shid, group in groups
    )
    
    # groups2 = []
    
    # for shid, group in groups:
        
    #     trafolarYıllık = dftrafoYıllık[
    #         (dftrafoYıllık["shid"] == shid)
    #     ].copy() 

    #     trafolar = dftrafocopy[
    #         (dftrafocopy["shid"] == shid)
    #     ].copy()                                         
                           
    #     if trafolar.empty:
                
    #             sh = dfsuper[sdfsuper["shid"]==shid]
                
    #             i, j = sh["i"].values[0], sh["j"].values[0]
    #             komsular = komsulari_bul(i, j, mesafe=hat_uzunluk, capraz_dahil=True)
                
    #             # r DataFrame'inden bu koordinatlara sahip satırları çek
    #             komsu_satirlar = dfsuper[dfsuper.apply(lambda x: (x["i"], x["j"]) in komsular, axis=1)]
                
    #             trafolar = dftrafoYıllık[
                    
    #                 (dftrafoYıllık["shid"].isin(komsu_satirlar["shid"]))                
                
    #             ].copy() 
                
                
    #             if not trafolar.empty:
                    
    #                 trafolar = trafolar.loc[[trafolar["efektif_bos_kapasite"].idxmax()]]
    #                 secilen_shid = trafolar["shid"].values[0]
                    
    #                 trafolarYıllık = dftrafoYıllık[
    #                     (dftrafoYıllık["shid"] == secilen_shid)
    #                 ].copy() 
                    
    #                 trafolar = dftrafocopy[
    #                     (dftrafocopy["shid"] == secilen_shid)
    #                 ].copy() 
                    
                                      
    #             else:
                    
    #                 trafolarYıllık = pd.DataFrame(columns=trafolar.columns)  # Boş ama aynı sütunlara sahip DF
    #                 trafolar = pd.DataFrame(columns=trafolar.columns)  # Boş ama aynı sütunlara sahip DF

                
    #     groups2.extend((shid, group, trafolarYıllık, trafolar))


    # joblib ile paralel dağıt
    results = Parallel(n_jobs=6)(
        delayed(process_shid)(shid, group, dfy, df) for shid, dfy, df, group in groups2
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
    
    return dftrafoYıllık, dftrafocopy, atama_sonucu_final

# Batch oluşturma için bağımsız SHID gruplarını ayıran fonksiyon
def gruplari_ayir_disjoint(groups, dftrafocopy, dfsuper, hat_uzunluk):
    # 1. Her SHID'in kullandığı trafoları topla
    shid_kullanim = defaultdict(set)

    for shid, group in groups:
        trafolar = dftrafocopy[dftrafocopy["shid"] == shid]
        
        if trafolar.empty:
            # Komşulara bak
            sh = dfsuper[dfsuper["shid"] == shid]
            if sh.empty:
                continue
            i, j = sh.iloc[0]["i"], sh.iloc[0]["j"]
            komsular = komsulari_bul(i, j, mesafe=hat_uzunluk, capraz_dahil=True)
            komsular_set = set(komsular)
            komsu_satirlar = dfsuper[dfsuper[["i", "j"]].apply(tuple, axis=1).isin(komsular_set)]
            komsu_shids = komsu_satirlar["shid"].tolist()
            trafolar = dftrafocopy[dftrafocopy["shid"].isin(komsu_shids)]

        for tid in trafolar["trafo_id"].unique():
            shid_kullanim[shid].add(tid)

    # 2. Bağımsız grupları oluştur
    disjoint_batches = []
    kalan = set(shid for shid, _ in groups)

    while kalan:
        batch = []
        kullanilan_trafolar = set()

        for shid in list(kalan):
            trafolar = shid_kullanim.get(shid, set())
            if trafolar.isdisjoint(kullanilan_trafolar):
                batch.append(shid)
                kullanilan_trafolar.update(trafolar)
                kalan.remove(shid)

        disjoint_batches.append(batch)

    return disjoint_batches

# Yeni versiyon: Paralelİslem fonksiyonu içinde bağımsız batch mantığıyla paralel çalışma
def Paralelİslem_Batch(df_result1, dftrafoYıllık, dftrafocopy, dfsuper, p, ilk=True):
    
    hat_uzunluk = p["hat_uzunluk"]

    # SHID bazlı gruplar
    groups = [(shid, group) for shid, group in df_result1.groupby("shid")]

    # SHID → group eşlemesi
    group_map = {shid: group for shid, group in groups}

    # Bağımsız batch'leri oluştur
    disjoint_batches = gruplari_ayir_disjoint(groups, dftrafocopy, dfsuper, hat_uzunluk)

    updated_partsYıllık = []
    updated_parts = []
    atama_parts = []

    for batch in disjoint_batches:
        batch_groups = [(shid, group_map[shid]) for shid in batch]

        groups2 = Parallel(n_jobs=4)(
            delayed(process_shid_group)(shid, group, dftrafoYıllık, dftrafocopy, dfsuper, hat_uzunluk, ilk)
            for shid, group in batch_groups
        )

        results = Parallel(n_jobs=4)(
            delayed(process_shid)(shid, group, dfy, df) for shid, dfy, df, group in groups2
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


def tum_kombinasyonlar(i, j, n, negatif_izinli=False):
    sonuc = []

    for di in range(-n, n + 1):
        for dj in range(-n, n + 1):
            if di == 0 and dj == 0:
                continue  # Başlangıç noktası hariç

            yeni_i = i + di
            yeni_j = j + dj

            # Negatif koordinatları istemiyorsan filtrele
            if not negatif_izinli and (yeni_i < 0 or yeni_j < 0):
                continue

            sonuc.append((yeni_i, yeni_j))

    return sonuc

class KirsalAlan:
    
    def __init__(self, df, trafo, trafoYıllık, dfsuper, dfsuperhucre, p, odtr):
        
        self.odtr = odtr
        
        self.p = p
        
        year_list = list(range(odtr["ilk_yil"], odtr["son_yil"]+1))
        
        self.atanamayan_talepler = pd.DataFrame({
            "year":range(odtr["ilk_yil"], odtr["son_yil"]+1),
            "Atanamayan Talep": [0] * len(year_list)
            })

        self.trafo = trafo
        
        self.trafoYıllık = trafoYıllık
        
        self.df = df
        
        self.dfsuperhucre = dfsuperhucre
        
        self.dfsuper = dfsuper
        
        if not self.df.empty:
            
            self.df = self.DeltaAlma4()
        
            start = time.time()  # Bitiş zamanı
            
            self.trafoYıllık, self.trafo, self.dfatama_sonuc = Paralelİslem_Batch(self.df, self.trafoYıllık, self.trafo, self.dfsuper , self.p)
            
            self.dfatama_sonuc = self.dfatama_sonuc.rename(columns={"kumulatif_yuk": "power_distribution"})
            
            atanamayan_talep = self.dfatama_sonuc.groupby("year")["Atanamayan Talep"].sum().reset_index()
            
            self.dfatama_sonuc["shid"] = self.dfatama_sonuc["shid"].astype(int)
            # self.trafoYıllık, self.trafo, self.dfatama_sonuc = Paralelİslem(self.df, self.trafoYıllık, self.trafo, self.dfsuper , self.p)
            # self.dfatama_sonuc = self.YukAtama(self.df, self.trafo, self.trafoYıllık)
            
            end = time.time()  # Bitiş zamanı
            
            logging.info(f"KIRSAL ALAN YUK ATAMA ISLEM SURESI: {end - start:.2f} saniye")       
            
            self.atanamayanlar = self.dfatama_sonuc[self.dfatama_sonuc["durum"] == "Atanmadı"]
            
            dfyeni_trafo, dfyenitrafoyıllık = self.TrafoEkle2(self.atanamayanlar)
            
            dfyeni_trafo = dfyeni_trafo.reset_index(drop=True)
            dfyenitrafoyıllık = dfyenitrafoyıllık.reset_index(drop=True)
            
            self.trafoYıllık = pd.concat([self.trafoYıllık, dfyenitrafoyıllık], ignore_index=True)
            
            # concat işlemiyle iki DataFrame'i birleştir
            self.trafo = pd.concat([self.trafo, dfyeni_trafo], ignore_index=True)
            
            # self.trafoYıllık, self.trafo, self.dfatama_sonuc2 = Paralelİslem_Batch(self.atanamayanlar, self.trafoYıllık, self.trafo, self.dfsuper , self.p, False)
            
            self.trafoYıllık, self.trafo, self.dfatama_sonuc2 = Paralelİslem_Batch(self.df_atanamayanlar_kume, self.trafoYıllık, self.trafo, self.dfsuper , self.p, False)
            
            years = pd.DataFrame({
                "year":range(odtr["ilk_yil"], odtr["son_yil"]+1)
            })
            
            atanamayan_talep2 = self.dfatama_sonuc2.groupby("year")["Atanamayan Talep"].sum().reset_index()
            
            atanamayanlar = self.dfatama_sonuc2[self.dfatama_sonuc2["durum"] == "Atanmadı"]
            
            # Grupla: yıl bazında toplam yük ve trafo adedi
            yil_bazli_Atanamayanlar = (
                atanamayanlar.groupby("year")["kumulatif_yuk"]
                .sum()
                .reset_index(name="Atanamayan Talep")
            )
            
            yil_bazli_Atanamayanlar = years.merge(yil_bazli_Atanamayanlar, on="year", how="left")
            
            atanamayan_talepler = pd.concat([atanamayan_talep, atanamayan_talep2, yil_bazli_Atanamayanlar], ignore_index=True)
            
            self.atanamayan_talepler = atanamayan_talepler.groupby("year", as_index=False).agg({"Atanamayan Talep": "sum"})
    
            # dfatama_sonuc3 = self.YukAtama(self.atanamayanlar, self.trafo, self.trafoYıllık)
    
    def DeltaAlma4(self):
        
        df_merged = self.df.copy() 
                
        columns_to_sum = ['MESKEN', 'SANAYI', 'TICARETHANE', 'AYDINLATMA']
        
        df_merged[columns_to_sum] = df_merged[columns_to_sum].apply(pd.to_numeric, errors='coerce').fillna(0)
        
        superhucre_aggregated = df_merged.groupby(["shid","year"])[columns_to_sum].sum().reset_index()
        
        superhucre_aggregated["TOTAL_POWER"] = superhucre_aggregated[columns_to_sum].sum(axis=1)
        
        #df_merged = df_merged.merge(superhucre_aggregated[["shid", "year", "TOTAL_POWER"]], on=["shid", "year"], how="inner")
        
        df_merged = superhucre_aggregated.copy()
        
        df_merged["TOTAL_POWER"] = (df_merged["TOTAL_POWER"] / self.p["toplam_saat"]) * self.p["toplam_saat_katsayi"] * 1000
        
        df_pivot = df_merged.pivot(index="shid", columns="year", values="TOTAL_POWER").reset_index()
          
        df_pivot = df_pivot.set_index("shid")
        
        df_diff = df_pivot.diff(axis=1)
        
        df_diff.iloc[:, 0] = df_pivot.iloc[:, 0]  # 2024 yılı verisini bozmamak için
        
        df_diff = df_diff.reset_index()
        
        df_result = df_diff.melt(id_vars="shid", var_name="year", value_name="power_distribution")
        
        df_result = df_result[df_result["year"] != self.odtr["ilk_yil"]-1]
        
        return df_result
    
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
    
    
    def TrafoBul(self, ID):
        
        shid = self.dfsuperhucre[self.dfsuperhucre["hucreid"]==ID]["shid"].values

        shid=shid[0]
        
        trafolar = self.trafo[
            (self.trafo["shid"] == shid)
        ].copy()                                         
                            
        if trafolar.empty:
                
                sh = self.dfsuper[self.dfsuper["shid"]==shid]
                
                i, j = sh["i"].values[0], sh["j"].values[0]
                komsular = self.komsulari_bul(i, j, mesafe=1, capraz_dahil=True)
                
                # r DataFrame'inden bu koordinatlara sahip satırları çek
                komsu_satirlar = self.dfsuper[self.dfsuper.apply(lambda x: (x["i"], x["j"]) in komsular, axis=1)]
                
                trafolar = self.trafo[
                    (self.trafo["shid"].isin(komsu_satirlar["shid"]))
                ].copy() 
                
                if not trafolar.empty:
                    trafolar = trafolar.loc[[trafolar["efektif_bos_kapasite"].idxmax()]]
                else:
                    trafolar = pd.DataFrame(columns=trafolar.columns)  # Boş ama aynı sütunlara sahip DF
        
        return trafolar
        
    def YukAtama(self, df, dftrafocopy2, dftrafoYıllık):
        
        hat_uzunluk = self.p["hat_uzunluk"]
        
        atama_sonucu_yeni = []
        dftrafocopy = dftrafoYıllık.copy()
        dftrafocopy["Nominal_Bos_yuzde"] = 0.0
        dftrafocopy["Efektif_Bos_yuzde"] = 0.0
        dftrafocopy["atanan_miktar_ek"] = 0.0

        # Dosyayı aç (append mode - "a" ile her çalıştırmada ekleme yap)
     
        dfMain = df.copy()

        for shid, dfsect in dfMain.groupby("shid"):
            
            Atanmadı_Cikti = False
            
            dff = dfsect.copy() 

            if (dff["power_distribution"]!=0).any():
                
                for index, row in dff.iterrows():
                    
                    yil = row["year"]
                    
                    yuk_miktari = row["power_distribution"]
                    
                    if not dff["power_distribution"].eq(0).all():
                        
                        
                        #shid = self.dfsuperhucre[self.dfsuperhucre["hucreid"]==row["ID"]]["shid"].values
        
                        #shid=shid[0]
                        mask = ~(dftrafocopy["Trafo Aksiyon"].isin(["deplase", "gerilim dönüşümü", "güç artırımı"]))
                        
                        trafolar = dftrafoYıllık[
                            (dftrafoYıllık["shid"] == shid) &
                            (dftrafoYıllık["year"] == yil) &
                            mask
                        ].copy()                                         
                                            
                        if trafolar.empty:
                                
                                sh = self.dfsuper[self.dfsuper["shid"]==shid]
                                
                                i, j = sh["i"].values[0], sh["j"].values[0]
                                komsular = self.komsulari_bul(i, j, mesafe=hat_uzunluk, capraz_dahil=True)
                                
                                # r DataFrame'inden bu koordinatlara sahip satırları çek
                                komsu_satirlar = self.dfsuper[self.dfsuper.apply(lambda x: (x["i"], x["j"]) in komsular, axis=1)]
                                
                                trafolar = dftrafocopy[
                                    (dftrafocopy["shid"].isin(komsu_satirlar["shid"])) &
                                    (dftrafocopy["year"] == yil) &
                                    mask
                                ].copy() 
                                
                                
                                if not trafolar.empty:
                                    trafolar = trafolar.loc[[trafolar["efektif_bos_kapasite"].idxmax()]]
                                    print(trafolar.iloc[0])
                                else:
                                    trafolar = pd.DataFrame(columns=trafolar.columns)  # Boş ama aynı sütunlara sahip DF
                        
                
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
                                    
                                    mask = (dftrafoYıllık["trafo_id"] == row["trafo_id"]) & (dftrafoYıllık["year"] >= yil+1)

                                    dftrafoYıllık.loc[mask, [
                                        "GelenYukToplam",
                                        "İlkGelenYukToplam",
                                        "bos_kapasite",
                                        "efektif_bos_kapasite",
                                        "kullanilan_kapasite_efektif",
                                        "Doluluk Oranı"
                                    ]] = [
                                        row["GelenYukToplam"],
                                        row["GelenYukToplam"],  # İlkGelenYukToplam aynısı
                                        row["bos_kapasite"],
                                        row["efektif_bos_kapasite"],
                                        row["kullanilan_kapasite_efektif"],
                                        row["GelenYukToplam"] / row["kapasite"]
                                    ]
                                    
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
                                
                        else:
                            
                            durum = "Atanmadı"
                            
                            GelenYukToplam = trafolar["GelenYukToplam"].sum()
                        
                            toplam_boskapasite_ek = trafolar["efektif_bos_kapasite"].sum()
                            
                            atama_sonucu_yeni.append([yil, shid, yuk_miktari, toplam_boskapasite_ek, GelenYukToplam, durum])

            
        
        dftrafocopy2["trafo_yasi"] = dftrafocopy2["trafo_yas"].copy()
        
        self.trafo = dftrafocopy2.copy()
        
        self.trafoYıllık = dftrafoYıllık.copy()

        return pd.DataFrame(atama_sonucu_yeni, columns=["year", "shid", "power_distribution", "toplam_boskapasite", "GelenYukToplam", "durum"])
    
    def TrafoOzet(self):
        
        all_years = pd.Series(self.p["years"], name="year")
        
        # yeni eklenen        
        df = self.dfyeni.copy()
        
        df = self.dftrafocopy4[self.dftrafocopy4["Durum"] == "Eklenen"].copy()

        df_year_counts = df.groupby("year").size().reset_index(name="yeni eklenen").astype(int)
        
        # all_years ile birleştirme
        df_full_year_Eklenen = all_years.to_frame().merge(df_year_counts, on="year", how="left").fillna(0)
        
        df_full_year_Eklenen = df_full_year_Eklenen.rename(columns={"yeni eklenen": "yeni trafo tesis"})
        
        # yenilenen
        df = TrafoYenile(self.dftrafocopy3)   
        
        df_year_Yenilenen = df.groupby("yenileme_yili").size().reset_index(name="yenilenen").astype(int)
        
        df_year_Yenilenen["year"] = df_year_Yenilenen["yenileme_yili"].astype(int)
        
        # all_years ile birleştirme
        df_full_year_Yenilenen = all_years.to_frame().merge(df_year_Yenilenen, on="year", how="left").fillna(0)
        
        df_full_year_Yenilenen.drop(["yenileme_yili"], axis=1, inplace=True)
        
        df_full_year_Yenilenen = df_full_year_Yenilenen.rename(columns={"yenilenen": "trafo yenileme-yaştan dolayı"})
   
        
        # Kapasite yenilenen
        df = self.dftrafocopy3.copy()
        
        df_year_kYenilenen = df.groupby("Kapasite_yenilenme_yili").size().reset_index(name="kapasite yenilenen").astype(int)
        
        df_year_kYenilenen["year"] = df_year_kYenilenen["Kapasite_yenilenme_yili"].astype(int)
        
        # all_years ile birleştirme
        df_full_year_kYenilenen = all_years.to_frame().merge(df_year_kYenilenen, on="year", how="left").fillna(0)
        
        df_full_year_kYenilenen.drop(["Kapasite_yenilenme_yili"], axis=1, inplace=True)
        
        df_full_year_kYenilenen = df_full_year_kYenilenen.rename(columns={"kapasite yenilenen": "trafo yenileme-kapasite aşımından dolayı"})


        
        # kapasite artan
        df_year_Artan = self.dftrafocopy3.groupby("Kapasite_artirma_yili").size().reset_index(name="kapasitesi artan").astype(int)
        
        df_year_Artan["year"] = df_year_Artan["Kapasite_artirma_yili"].astype(int)

        df_full_year_Artan = all_years.to_frame().merge(df_year_Artan, on="year", how="left").fillna(0)

        df_full_year_Artan.drop(["Kapasite_artirma_yili"], axis=1, inplace=True)
        
        df_full_year_Artan = df_full_year_Artan.rename(columns={"kapasitesi artan": "trafo yükseltme"})

        
        df_merged = df_full_year_Eklenen.merge(df_full_year_Yenilenen, on="year") 
        df_merged = df_merged.merge(df_full_year_Artan, on="year")
        df_merged = df_merged.merge(df_full_year_kYenilenen, on="year")
        
        df_merged["year"] = df_merged["year"].astype(int)
        
        self.TrafoSonuc = df_merged.copy()
    
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
    
    def Kumele(self, dfYuk):
        
        odtr = config3.get()
        
        dfsuper = self.dfsuper[self.dfsuper["year"] == odtr["ilk_yil"]]
        
        mask = dfYuk.groupby("shid")["durum"].apply(lambda x: (x == "Atanmadı").any())

        dfYuk2 = dfYuk[dfYuk["shid"].isin(mask[mask].index)]
        
        dfYuk2 = dfYuk2[dfYuk2["year"] == odtr["ilk_yil"]]
        
        df = dfYuk2.merge(dfsuper[["shid", "i", "j"]], on="shid", how="left")
        
        # Her hücre için komşularını bulma (i ve j farkı 5'ten küçük olanlar)
        komsular = {}
        for index, row in df.iterrows():
            shid = row["shid"]
            i, j = row["i"], row["j"]
            komsu_shidler = df[
                ((df["i"] - i).abs() <= 5) & ((df["j"] - j).abs() <= 5) & (df["shid"] != shid)
            ]["shid"].tolist()
            komsular[shid] = komsu_shidler

        # Komşu sayısına göre sırala
        sorted_shidler = sorted(komsular.items(), key=lambda x: len(x[1]), reverse=True)

        # Kümeleme
        kume_id = 0
        atandilar = set()
        kume_sonuclari = []

        for shid, komsulari in sorted_shidler:
            if shid in atandilar:
                continue

            kume = [shid] + [k for k in komsulari if k not in atandilar]
            atandilar.update(kume)

            # Merkez SHID’i belirle
            merkez_shid = max(kume, key=lambda x: len(komsular.get(x, [])))

            for s in kume:
                kume_sonuclari.append({
                    "shid": s,
                    "kume_id": kume_id,
                    "merkez_shid": merkez_shid
                })
            
            kume_id += 1

        df_kume = pd.DataFrame(kume_sonuclari)
        
        # tt = ka.atanamayanlar.merge(df_kume, on="shid", how="left")
        # tt["shid"] = tt["merkez_shid"].astype(int)
        # dfsuper = dfsuper[dfsuper["shid"].isin(df_kume.shid.values)]
        
        # dfsuper = dfsuper.merge(df_kume, on="shid", how="left")
        
        dfYuk = dfYuk.merge(df_kume, on="shid", how="left")
        
        # den3 = den.merge(df_kume, on="shid", how="left")
        
        df_kume_toplam_Yuk = dfYuk.groupby(["kume_id", "year"])["power_distribution"].sum().reset_index()
        df_kume_toplam_Kapasite = dfYuk.groupby(["kume_id", "year"])["toplam_boskapasite"].sum().reset_index()
        
        df_kume_toplam = pd.merge(
            df_kume_toplam_Yuk,
            df_kume_toplam_Kapasite,
            on=["kume_id", "year"],
            how="outer"  # veya "inner", ihtiyaç durumuna göre
        )
        
        df_kume_toplam["kume_id"] = df_kume_toplam["kume_id"].astype(int)
        
        self.dfkumeleme = df_kume_toplam
        
        return df_kume_toplam, df_kume
        
    def TrafoEkle(self, atanamayanlar):    
        
        yeni_trafo_list = []
        
        dfyeni_trafo = pd.DataFrame()
        dfyeni_trafoyıllık = pd.DataFrame()
        
        df = atanamayanlar.copy()
        
        self.dfs, self.dfkume = self.Kumele(df)
        # Her shid için toplam kümülatif yükü hesapla
        
        df = df[df["power_distribution"]>0].copy()
        
        shid_total_yuk = df.groupby("shid")["power_distribution"].sum().reset_index()
        
        # Her shid için ilk görüldüğü yılı bul
        shid_min_year = df.groupby("shid")["year"].min().reset_index()
        
        # Bu iki bilgiyi birleştirerek toplam yükü ilk yıla aktarma
        df_final = shid_min_year.merge(shid_total_yuk, on="shid")
        
        df_final2 = df_final.merge(df[["shid", "year", "toplam_boskapasite"]], on = ["shid", "year"], how="left")
        
        df_final2 = df_final2.sort_values(by="year", ascending=True).reset_index(drop=True)
    
        for index, row in df_final2.iterrows():
              
            shid = row["shid"]
            
            #shid = self.dfsuperhucre[self.dfsuperhucre["hucreid"]==row["ID"]]["shid"].values

            #shid=shid[0]
            
            toplam_atanamayan_yuk = row["power_distribution"]
            
            gerekli_kapasite = toplam_atanamayan_yuk
            
            yil = row["year"]  # **İlk atanamayan yıl**
    
            trafo_list = pd.DataFrame(columns=["sayac", "hucre_id", "shid","kapasite", "efektif_kapasite", "Koord_x", "Koord_y"])
            
            break_outer_loop = False  # Dış döngüden çıkmayı kontrol eden değişken
            
            sayac = 0
            
            uygun_hucreler = (self.dfsuperhucre[self.dfsuperhucre["shid"] == shid]["hucreid"])
            
            while not break_outer_loop:
                
                for kapasite in self.p["kurum_trafo_kirsal_alan_liste"]:
                    
                    efektif_miktar = kapasite * self.p["yeni_trafo_kapasite_kullanim_ust_limiti"]
                                       
                    data = pd.DataFrame({
                        "sayac":[sayac],
                        "hucre_id": uygun_hucreler.iloc[0],
                        "shid": [shid],
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
                        
                    if toplam_efektif < gerekli_kapasite:
                        if kapasite != self.p["kurum_trafo_kirsal_alan_liste"][-1]:
                            trafo_list = trafo_list.drop(trafo_list.index[-1])                                         
                    
                sayac += 1
                
                uygun_hucreler = uygun_hucreler.iloc[1:]
                
                if break_outer_loop:
                    break  # **Dış döngüden de çık**
            
            yukler = df[df["shid"]==shid].copy()
            
            ilkyil = yukler["year"].min()
            
            kapasite = 0
            
            for idtrafo, trafo in trafo_list.iterrows():
                            
                kapasite += trafo["efektif_kapasite"]
                
                tesis_yili = yukler["year"].min()
                
                for idx, row in yukler.iterrows():
                    
                    yuk = row["power_distribution"]
                    
                    yil = row["year"]                              
                    
                    if yuk <= kapasite:
                        kapasite -= yuk
                        
                    else:
    
                        dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfsuperhucre, ilkyil, int(tesis_yili), None, self.p, bolge="Kırsal")
                        
                        dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                        dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])   
                        
                        mask = yukler["year"] >= yil
                        
                        yukler = yukler[mask]
                        
                        break
                    
                    if idtrafo == len(trafo_list) - 1:
                        
                        dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfsuperhucre, ilkyil, int(tesis_yili), None, self.p, bolge="Kırsal")
                        
                        dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                        dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])
                        
                        break                                                           
        
        return dfyeni_trafo, dfyeni_trafoyıllık
        
    
    def from_first_positive(self, group):
        # İlk pozitif değerin index'ini bul
        first_pos_idx = group[group["power_distribution"] > 0].first_valid_index()
        
        if first_pos_idx is not None:
            return group.loc[first_pos_idx:]
        else:
            return pd.DataFrame(columns=group.columns)  # pozitif değer yoksa boş döndür

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
    
    def TrafoEkle2(self, atanamayanlar):    
        
        yeni_trafo_list = []
        
        dfyeni_trafo = pd.DataFrame()
        dfyeni_trafoyıllık = pd.DataFrame()
        
        df_kume_toplam, self.df_kume = self.Kumele(self.dfatama_sonuc)
        
        df_kume = self.df_kume.copy()
        
        self.df_kume_toplam = df_kume_toplam.copy()
        
        # df = df_kume_toplam[df_kume_toplam["power_distribution"]>0].copy()
        
        df = df_kume_toplam.copy()
        
        valid_shids = df.groupby("kume_id")["power_distribution"].sum()
        
        valid_shids = valid_shids[valid_shids > 0].index
        
        # 2. Bu shid’lere sahip satırları filtrele
        df = df[df["kume_id"].isin(valid_shids)].copy()
        
        
        
        df.sort_values(by=["kume_id", "year"], inplace=True)
        
        # 3. Her shid için kümülatif toplam (zaman içinde birikimli yük)
        df["power_distribution_cumsum"] = (
            df.groupby("kume_id")["power_distribution"]
              .transform(self.cumsum_from_first_positive_and_reset_on_negative)
        )
        
        # df["power_distribution_cumsum"] = df.groupby("kume_id")["power_distribution"].cumsum()
        
        # 4. Her shid için bu cumsum’un maksimumunu al
        shid_max_kumulatif = df.groupby("kume_id")["power_distribution_cumsum"].max().reset_index()
        # shid_total_yuk = df.groupby("kume_id")["power_distribution"].sum().reset_index()
        
        # Her shid için ilk görüldüğü yılı bul
        # Her shid için ilk görüldüğü yılı bul
        # df2 = df[df["power_distribution"]>0].copy()
        df2 = df.groupby("kume_id", group_keys=False).apply(self.from_first_positive)

        shid_min_year = df2.groupby("kume_id")["year"].min().reset_index()
        
        # Bu iki bilgiyi birleştirerek toplam yükü ilk yıla aktarma
        df_final = shid_min_year.merge(shid_max_kumulatif, on="kume_id")
        
        df_final2 = df_final.merge(df[["kume_id", "year"]], on = ["kume_id", "year"], how="left")
        
        df_final2 = df_final2.sort_values(by="year", ascending=True).reset_index(drop=True)
        
        df_final2["kume_id"] = df_final2["kume_id"].astype(int)
        
        df_kume_merkez_hucre = self.df_kume.groupby("kume_id")["merkez_shid"].first().reset_index()
        
        df_atanamayanlar_kume = df_kume_toplam.merge(df_kume_merkez_hucre, on="kume_id", how="left")

        # df_atanamayanlar_kume = df_final2.merge(df_kume_merkez_hucre, on="kume_id", how="left")
        
        df_atanamayanlar_kume["shid"] = df_atanamayanlar_kume["merkez_shid"]
        
        df_atanamayanlar_kume = df_atanamayanlar_kume.groupby(["shid", "year"])[["power_distribution", "toplam_boskapasite"]].sum().reset_index()
        
        self.df_atanamayanlar_kume = df_atanamayanlar_kume
    
        for index, row in df_final2.iterrows():
              
            kume = row["kume_id"]
            
            #shid = self.dfsuperhucre[self.dfsuperhucre["hucreid"]==row["ID"]]["shid"].values

            #shid=shid[0]
            
            toplam_atanamayan_yuk = row["power_distribution_cumsum"]
            
            gerekli_kapasite = toplam_atanamayan_yuk
            
            yil = row["year"]  # **İlk atanamayan yıl**
    
            trafo_list = pd.DataFrame(columns=["sayac", "hucre_id", "shid","kapasite", "efektif_kapasite", "Koord_x", "Koord_y"])
            
            break_outer_loop = False  # Dış döngüden çıkmayı kontrol eden değişken
            
            sayac = 0
            
            merkez_shid = df_kume[df_kume["kume_id"]==kume]["merkez_shid"].iloc[0]
            
            uygun_hucreler = (self.dfsuperhucre[self.dfsuperhucre["shid"] == merkez_shid]["hucreid"])
            
            while not break_outer_loop:
                
                for kapasite in self.p["kurum_trafo_kirsal_alan_liste"]:
                    
                    efektif_miktar = kapasite * self.p["yeni_trafo_kapasite_kullanim_ust_limiti"]
                                       
                    data = pd.DataFrame({
                        "sayac":[sayac],
                        "hucre_id": uygun_hucreler.iloc[0],
                        "shid": [merkez_shid],
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
                        
                    if toplam_efektif < gerekli_kapasite:
                        if kapasite != self.p["kurum_trafo_kirsal_alan_liste"][-1]:
                            trafo_list = trafo_list.drop(trafo_list.index[-1])                                         
                    
                sayac += 1
                
                if not uygun_hucreler.iloc[1:].empty:
                    uygun_hucreler = uygun_hucreler.iloc[1:]                
                
                if break_outer_loop:
                    break  # **Dış döngüden de çık**
                
            
            # yukler = df2[df2["kume_id"]==kume].copy()
            
            # ilkyil = yukler["year"].min()
            
            # kapasite = 0
            
            # for idtrafo, trafo in trafo_list.iterrows():
                            
            #     kapasite += trafo["efektif_kapasite"]
                
            #     tesis_yili = yukler["year"].min()
                
            #     for idx, row in yukler.iterrows():
                    
            #         yuk = row["power_distribution"]
                    
            #         yil = row["year"]                              
                    
            #         if yuk <= kapasite:
            #             kapasite -= yuk
                        
            #         else:
    
            #             dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfsuperhucre, ilkyil, int(tesis_yili), None, self.p, bolge="Kırsal")
                        
            #             dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
            #             dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])   
                        
            #             mask = yukler["year"] >= yil
                        
            #             yukler = yukler[mask]
                        
            #             break
                    
            #         if idtrafo == len(trafo_list) - 1:
                        
            #             dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfsuperhucre, ilkyil, int(tesis_yili), None, self.p, bolge="Kırsal")
                        
            #             dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
            #             dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])
                        
            #             break 
            
            
            yukler = df2[df2["kume_id"]==kume].copy()
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
                dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfsuperhucre, ilkyil, int(tesis_yili), None, self.p, bolge="Kırsal")
                
                dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])   
               
                if break_occurred:
                    yukler = kalan_yukler
            
                    # Eğer son trafodaysan ve hâlâ yük varsa → aynı trafoyu tekrar ekle
                    if idtrafo == len(trafo_list) - 1:
                        # ekstra trafo daha
                        dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfsuperhucre, ilkyil, int(tesis_yili), None, self.p, bolge="Kırsal")
                        
                        dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                        dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])
                        
                else:
                    break  # tüm yük karşılandıysa çık
                                                                      
                    
        return dfyeni_trafo, dfyeni_trafoyıllık
    
    
    
    
    
    
    
    
    
    
    
    
    