# -*- coding: utf-8 -*-
"""
Created on Fri Apr 11 11:12:52 2025

@author: vural.bayrakli
"""

from Functions3 import SqliteKaydet, YukAta4_3, SqliteOku, yeni_trafo_df_olustur, TrafoOlustur, TrafoKapasiteArtirGuncel3, ReadParquet, TrafoYenile, e
import numpy as np
import pandas as pd
import config3
import os
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

# Batch oluşturma için bağımsız SHID gruplarını ayıran fonksiyon
def gruplari_ayir_disjoint(groups, dftrafocopy, dfsuper, hat_uzunluk):
    # 1. Her SHID'in kullandığı trafoları topla
    shid_kullanim = defaultdict(set)

    for shid, group in groups:
        
        trafolar = dftrafocopy[(dftrafocopy["shid"] == shid) & (dftrafocopy["Trafo Mülkiyeti"] == "Özel")]
        
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
            trafolar = dftrafocopy[(dftrafocopy["shid"].isin(komsu_shids)) & (dftrafocopy["Trafo Mülkiyeti"] == "Özel")].copy()

        for tid in trafolar["trafo_id"].unique():
            shid_kullanim[shid].add(tid)

    # 2. Bağımsız grupları oluştur
    disjoint_batches = []
    kalan = set(shid for shid, _ in groups)
    
    non_empty = [s for s, t in shid_kullanim.items() if t]
    
    if not non_empty:
        disjoint_batches = [list(kalan)]  # tüm SHID'ler tek batch
        print("Hiç trafo bulunamadı. Tüm SHID'ler tek batch'e alındı.")
        return disjoint_batches
    
    while kalan:
        batch = []
        kullanilan_trafolar = set()
        kalan_list = list(kalan)
    
        for shid in kalan_list:
            trafolar = shid_kullanim.get(shid, set())
            if not trafolar:
                kalan.remove(shid)
                continue
            if trafolar.isdisjoint(kullanilan_trafolar):
                batch.append(shid)
                kullanilan_trafolar.update(trafolar)
                kalan.remove(shid)
    
        if not batch:
            # Alternatif fallback
            batch = list(kalan)
            kalan.clear()
            print("Uyarı: Tüm kalan SHID'ler çakışıyor. Hepsi tek batch'e alındı.")
    
        disjoint_batches.append(batch)

    # disjoint_batches = create_conflict_free_batches(shid_kullanim)

    return disjoint_batches

def create_conflict_free_batches(shid_to_trafos):
    
    conflict_map = defaultdict(set)
    shid_list = list(shid_to_trafos.keys())

    # 1. Conflict Graph oluştur
    for i in range(len(shid_list)):
        for j in range(i + 1, len(shid_list)):
            s1, s2 = shid_list[i], shid_list[j]
            if set(shid_to_trafos[s1]) & set(shid_to_trafos[s2]):
                conflict_map[s1].add(s2)
                conflict_map[s2].add(s1)

    remaining = set(shid_list)
    batches = []

    # 2. Çakışmasız batch'ler oluştur
    while remaining:
        batch = []
        for shid in list(remaining):
            if all(other not in conflict_map[shid] for other in batch):
                batch.append(shid)
                remaining.remove(shid)
        batches.append(batch)

    return batches

def process_shid_group(shid, group, dftrafoYıllık, dftrafocopy, dfsuper, hat_uzunluk, ilk_atama=True):
    
    trafolar = pd.DataFrame(columns=dftrafocopy.columns)
    trafolarYillik = pd.DataFrame(columns=dftrafoYıllık.columns)
    
    if ilk_atama:
        trafolarYillik = dftrafoYıllık[(dftrafoYıllık["shid"] == shid) & (dftrafoYıllık["Trafo Mülkiyeti"] == "Özel")].copy()
        trafolar = dftrafocopy[(dftrafocopy["shid"] == shid) & (dftrafocopy["Trafo Mülkiyeti"] == "Özel")].copy()
        
        if trafolar.empty:
            sh = dfsuper[dfsuper["shid"] == shid]
            if sh.empty:
                return (shid, pd.DataFrame(), pd.DataFrame(), group)

            i, j = sh.iloc[0]["i"], sh.iloc[0]["j"]
            komsular = komsulari_bul(i, j, mesafe=hat_uzunluk, capraz_dahil=True)
            komsular_set = set(komsular)

            komsu_satirlar = dfsuper[dfsuper[["i", "j"]].apply(tuple, axis=1).isin(komsular_set)]
            komsu_shids = komsu_satirlar["shid"].tolist()
            trafolar_komsu = dftrafocopy[(dftrafocopy["shid"].isin(komsu_shids)) & (dftrafocopy["Trafo Mülkiyeti"] == "Özel")].copy()
            
            # trafolar = dftrafocopy[(dftrafocopy["trafo_id"].isin(trafolar_komsu["trafo_id"].values))].copy()
            trafolar = trafolar_komsu.copy()
            trafolarYillik = dftrafoYıllık[dftrafoYıllık["trafo_id"].isin(trafolar["trafo_id"].values)].copy()
        
    else:
        # dftrafoYıllık = ka.trafoYıllık.copy()
        # dftrafocopy = ka.trafo.copy()
        trafolar = dftrafocopy[(dftrafocopy["shid"] == shid) & (dftrafocopy["Trafo Mülkiyeti"] == "Özel") & (dftrafocopy["Trafo Aksiyon"] == "yeni trafo tesis")].copy()
        trafolarYillik = dftrafoYıllık[dftrafoYıllık["trafo_id"].isin(trafolar["trafo_id"].values)].copy()
        
    
    return (shid, trafolarYillik, trafolar, group)

def process_shid(shid, group, dftrafoY_shid, dftrafocopy):
    
    return YukAta4_3(group, dftrafocopy, dftrafoY_shid)

def test_batch_conflicts(disjoint_batches, dftrafocopy):
    batch_trafolar_list = []

    for batch_index, batch in enumerate(disjoint_batches):
        # 1. Bu batch'teki tüm SHID'lerin trafolarını topla
        batch_trafolar = set()
        for shid in batch:
            trafolar = dftrafocopy[(dftrafocopy["shid"] == shid) & (dftrafocopy["Trafo Mülkiyeti"] == "Özel")]
            batch_trafolar.update(trafolar["trafo_id"].unique())

        # 2. Önceki batch'lerle çakışma kontrolü
        for prev_index, prev_trafolar in enumerate(batch_trafolar_list):
            if batch_trafolar & prev_trafolar:
                print(f"❌ Çakışma bulundu: Batch {prev_index+1} ile Batch {batch_index+1}")
                return False

        batch_trafolar_list.append(batch_trafolar)

    print("✅ Tüm batch'ler trafo açısından çakışmasız.")
    return True

def safe_update(df_original, df_updates, key_cols):
    # Anahtar sütunlar üzerinden güncellenebilir hale getir 
    df_combined = pd.merge(
        df_original,
        df_updates,
        on=key_cols,
        how="left",
        suffixes=("", "_updated")
    ).copy()  # 👈 burası kritik!
    
    for col in df_updates.columns:
        if col in key_cols:
            continue
        if f"{col}_updated" in df_combined.columns:
            df_combined[col] = df_combined[f"{col}_updated"].combine_first(df_combined[col])
            df_combined.drop(columns=[f"{col}_updated"], inplace=True)
    
    return df_combined

# Yeni versiyon: Paralelİslem fonksiyonu içinde bağımsız batch mantığıyla paralel çalışma
def Paralelİslem_Batch(df_result1, dftrafoYıllık, dftrafocopy, dfsuper, p, ilk=True):
    
    hat_uzunluk = p["tarımsal_sulama_hat_uzunluk"]

    # SHID bazlı gruplar
    groups = [(shid, group) for shid, group in df_result1.groupby("shid")]
    
    # SHID → group eşlemesi
    group_map = {shid: group for shid, group in groups}
    # dftrafocopy= dftrafo.copy()
    # Bağımsız batch'leri oluştur
    disjoint_batches = gruplari_ayir_disjoint(groups, dftrafocopy, dfsuper, hat_uzunluk)
    # test_batch_conflicts(disjoint_batches, dftrafocopy)
    
    # disjoint_batches = create_conflict_free_batches(group_map)

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



class PointLoadKismi2:
    
    def __init__(self, data, pl):
        
        odtr = config3.get()
        
        katsayilar_path = os.path.join(odtr["dosyalar"], "katsayılar.xlsx")

        df = pd.read_excel(katsayilar_path, engine="openpyxl")  # Daha hızlı olması için openpyxl kullanılır
        
        df = df.iloc[:,[0,3]]
        
        self.p = config3.Start(df)
        
        try:
            self.dfsuperhucre = ReadParquet(odtr["dfsuperhucre"])
            
            self.dfhucre_super = ReadParquet(odtr["dfhucre_super"])
        
        except Exception as e:
            print(e)
            
            return
        
        self.odtr = odtr
        
        self.pl = pl
        
        self.data = data
        
        if not data.empty:
            
            self.data2 = self.DeltaAlma2(data)
        
    def DeltaAlma2(self, data):
        
        df_merged = data.copy() 
        
        if self.pl == "sanayi":
            df_merged["TOTAL_POWER"] = df_merged["SANAYI_POINT_LOAD"].copy()
            
        if self.pl == "ticarethane":
            df_merged["TOTAL_POWER"] = df_merged["TICARETHANE_POINT_LOAD"].copy()
   
        if self.pl == "tarimsal":

            df_merged = df_merged.groupby(["shid", "year"])["TARIMSAL_SULAMA"].sum().reset_index()
            
            df_merged["TOTAL_POWER"] = df_merged["TARIMSAL_SULAMA"].copy()
            
            df_merged["TOTAL_POWER"] = (df_merged["TOTAL_POWER"] / self.p["toplam_saat"]) * self.p["toplam_saat_katsayi"] / self.p["KVA_cevirme"]
            
            df_pivot = df_merged.pivot(index="shid", columns="year", values="TOTAL_POWER").reset_index()
              
            df_pivot = df_pivot.set_index("shid")
            
            df_diff = df_pivot.diff(axis=1)
            
            df_diff.iloc[:, 0] = df_pivot.iloc[:, 0]  # 2024 yılı verisini bozmamak için
            
            df_diff = df_diff.reset_index()
            
            df_result = df_diff.melt(id_vars="shid", var_name="year", value_name="power_distribution")
            
            df_result = df_result[df_result["year"] != self.odtr["ilk_yil"]-1]
            
            return df_result
            
        df_merged = df_merged.rename(columns={"id":"ID"})
        
        df_merged["TOTAL_POWER"] = (df_merged["TOTAL_POWER"] / self.p["toplam_saat"]) * self.p["toplam_saat_katsayi"] / self.p["KVA_cevirme"]
        
        df_pivot = df_merged.pivot(index="ID", columns="year", values="TOTAL_POWER").reset_index()
          
        df_pivot = df_pivot.set_index("ID")
        
        df_diff = df_pivot.diff(axis=1)
        
        df_diff.iloc[:, 0] = df_pivot.iloc[:, 0]  # 2024 yılı verisini bozmamak için
        
        df_diff = df_diff.reset_index()
        
        df_result = df_diff.melt(id_vars="ID", var_name="year", value_name="power_distribution")
        
        return df_result
    
    def TrafoOzet(self, df):
        
        # all_years = pd.Series(self.p["years"], name="year")
        
        odtr = config3.get()

        all_years = pd.Series(list(range(odtr["ilk_yil"]-1, odtr["son_yil"] +1)), name="year")


        df_year_counts = df.groupby("year").size().reset_index(name="yeni eklenen").astype(int)
        
        # all_years ile birleştirme
        df_full_year_Eklenen = all_years.to_frame().merge(df_year_counts, on="year", how="left").fillna(0)
        
        df_full_year_Eklenen = df_full_year_Eklenen.rename(columns={"yeni eklenen": f"{df['Trafo Müşterisi'].iloc[0]} (Adet)"})
        
        return df_full_year_Eklenen
        
    def TrafoDurum(self):
        
        df = self.dftrafocopy4.copy()
        
        df.loc[df["Kapasite_artirma_yili"].notna(), "Durum"] = "Kapasite Artan"
        df.loc[df["Kapasite_artirma_yili"].notna(), "Trafo Durum"] = "Kapasite Artan"

        
        df.loc[df["trafo_id"].str.contains("New"), "Durum"] = "Eklenen"
        
        df2 = TrafoYenile(self.dftrafocopy3)  
        
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
                
    def SqlKaydet(self):
        SqliteKaydet(self.data, "PointloadTest")
        
    def ExcelKaydet(self, df, name):
        df.to_excel(f'{name}.xlsx', index=False, engine='openpyxl')
        
    def YukAtama(self, df_result, dftrafocopy2, dftrafoYıllık, musteri):
        
        dftrafocopy = dftrafoYıllık.copy()
        
        atama_sonucu_yeni = []
        
        dftrafocopy["Nominal_Bos_yuzde"] = 0.0
        dftrafocopy["Efektif_Bos_yuzde"] = 0.0
        dftrafocopy["atanan_miktar_ek"] = 0.0

        # Dosyayı aç (append mode - "a" ile her çalıştırmada ekleme yap)
        
        
        trafoshids = dftrafocopy["shid"].unique()
        
        for idhucre, df in df_result.groupby("ID"):
            
                
            if not df["power_distribution"].eq(0).all():
            
                Atanmadı_Cikti = False
                
                df = df.copy()
                
                #yukToplami = df["power_distribution"].sum()
                
                superhucredeki_trafolar = pd.DataFrame()
                
                for index, row in df.iterrows():
                    
                    yil = row["year"]
                    
                    yuk_miktari = row["power_distribution"]

                    superhucredeki_trafolar = dftrafoYıllık[
                        (dftrafoYıllık["merkez_hucre"] == idhucre) &
                        (dftrafoYıllık["year"] == yil) &
                        (dftrafoYıllık["Trafo Müşterisi"] == musteri)
                    ].copy()
                                        
                    if (superhucredeki_trafolar["efektif_bos_kapasite"] > 0).any():
                        superhucredeki_trafolar = superhucredeki_trafolar[superhucredeki_trafolar["efektif_bos_kapasite"] > 0]
                               
                    toplam_boskapasite = superhucredeki_trafolar["bos_kapasite"].sum()  
                    toplam_kapasite = superhucredeki_trafolar["kapasite"].sum()  
                    toplam_boskapasite_ek = superhucredeki_trafolar["efektif_bos_kapasite"].sum() 
                    toplam_kapasite_ek = superhucredeki_trafolar["efektif_kapasite"].sum() 
                    
                    
                    superhucredeki_trafolar["atama_miktari"] = 0.0
                    superhucredeki_trafolar["atama_miktari_ek"] = 0.0
                    superhucredeki_trafolar["kumulatif_atama_miktari"] = 0.0
                    superhucredeki_trafolar["kumulatif_bos_kapasite"] = 0.0
                    superhucredeki_trafolar["atama_orani"] = 0
                    superhucredeki_trafolar["atama_orani_ek"] = 0
                    superhucredeki_trafolar["atama_miktari"] = 0
                    superhucredeki_trafolar["atama_miktari_ek"] = 0
    
                    
                    if not Atanmadı_Cikti:
                        
                        if ((toplam_boskapasite_ek > yuk_miktari and toplam_boskapasite_ek > 0) or yuk_miktari <= 0):
                            
                            # Payda sıfırsa atama işlemi yapmadan önce kontrol ekleyelim
                            superhucredeki_trafolar["atama_orani"] = np.where(
                                toplam_boskapasite != 0, 
                                superhucredeki_trafolar["bos_kapasite"] / toplam_boskapasite, 
                                0  # Payda sıfırsa 0 değeri atansın
                            )
                            
                            superhucredeki_trafolar["atama_orani_ek"] = np.where(
                                toplam_boskapasite_ek != 0, 
                                superhucredeki_trafolar["efektif_bos_kapasite"] / toplam_boskapasite_ek, 
                                0  # Payda sıfırsa 0 değeri atansın
                            )
                                           
                            superhucredeki_trafolar["atama_miktari"] = yuk_miktari * superhucredeki_trafolar["atama_orani"]
                            superhucredeki_trafolar["atama_miktari_ek"] = yuk_miktari * superhucredeki_trafolar["atama_orani_ek"]
                            
                            superhucredeki_trafolar["GelenYukToplam"] += superhucredeki_trafolar["atama_miktari_ek"]
                            
                            superhucredeki_trafolar["kumulatif_bos_kapasite"] = superhucredeki_trafolar["efektif_kapasite"] - superhucredeki_trafolar["GelenYukToplam"]
                            
                            superhucredeki_trafolar["bos_kapasite"] -= superhucredeki_trafolar["atama_miktari"]
                            superhucredeki_trafolar["bos_kapasite"] = superhucredeki_trafolar[
                                "bos_kapasite"
                            ]
                            
                            superhucredeki_trafolar["efektif_bos_kapasite"] -= superhucredeki_trafolar["atama_miktari_ek"]
                            superhucredeki_trafolar["efektif_bos_kapasite"] = superhucredeki_trafolar[
                                "efektif_bos_kapasite"
                            ]
                            
                            superhucredeki_trafolar.loc[(superhucredeki_trafolar["kapasite"] != 0), "Nominal_Bos_yuzde"] = (
                                superhucredeki_trafolar["bos_kapasite"] / superhucredeki_trafolar["kapasite"]
                            )
                            
                            superhucredeki_trafolar.loc[(superhucredeki_trafolar["efektif_kapasite"] != 0), "Efektif_Bos_yuzde"] = (
                                superhucredeki_trafolar["kumulatif_bos_kapasite"] / superhucredeki_trafolar["efektif_kapasite"]
                            )
                            
                            superhucredeki_trafolar.loc[(superhucredeki_trafolar["kapasite"] != 0), "Doluluk Oranı"] = (
                                superhucredeki_trafolar["GelenYukToplam"] / superhucredeki_trafolar["kapasite"])

                            superhucredeki_trafolar["Rapor"] = superhucredeki_trafolar.apply(
                                lambda row: (row["Rapor"] if pd.notna(row["Rapor"]) else "") + f"Süper Hücre {idhucre} den {row['atama_miktari_ek']} kVA yük geldi\n", axis=1)
                            
                            for index, row in superhucredeki_trafolar.iterrows():
                                
                                dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"] = row.GelenYukToplam
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
                                                                                                                                  
                                                

    def YukAtamaTarimsal(self, df_result, dftrafocopy2, dftrafoYıllık, musteri, ilk=True):
                
        atama_sonucu_yeni = []
                
        for shid, df in df_result.groupby("shid"):
                
                superhucredeki_trafolar = pd.DataFrame()
                
                if not df["power_distribution"].eq(0).all():
                
                    Atanmadı_Cikti = False
                    
                    df = df.copy()                                      
                                       
                    for index, row in df.iterrows():
                        
                        yil = row["year"]
                        
                        yuk_miktari = row["power_distribution"]
                        
                        if ilk:
                            
                            superhucredeki_trafolar = dftrafoYıllık[
                                (dftrafoYıllık["merkez_hucre"].isin(self.dfhucre_super[self.dfhucre_super["shid"] == shid]["hucreid"])) &
                                (dftrafoYıllık["year"] == yil)                               
                            ].copy()
                            
                            if superhucredeki_trafolar.empty:
                                
                                sh = self.dfsuperhucre[self.dfsuperhucre["shid"] == shid]
                                if sh.empty:
                                    continue
                                i, j = sh.iloc[0]["i"], sh.iloc[0]["j"]
                                komsular = komsulari_bul(i, j, mesafe=1, capraz_dahil=True)
                                komsular_set = set(komsular)
                                komsu_satirlar = self.dfsuperhucre[self.dfsuperhucre[["i", "j"]].apply(tuple, axis=1).isin(komsular_set)]
                                komsu_shids = komsu_satirlar["shid"].tolist()
                                
                                superhucredeki_trafolar = dftrafoYıllık[
                                    (dftrafoYıllık["shid"].isin(komsu_shids))&
                                    (dftrafoYıllık["year"] == yil)                           
                                ].copy()
                                                 
                        else: 
                            
                            superhucredeki_trafolar = dftrafoYıllık[
                                (dftrafoYıllık["merkez_hucre"].isin(self.dfhucre_super[self.dfhucre_super["shid"] == shid]["hucreid"])) &
                                (dftrafoYıllık["year"] == yil) &
                                (dftrafoYıllık["Trafo Müşterisi"] == musteri)
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
                        superhucredeki_trafolar["atama_miktari"] = 0
                        superhucredeki_trafolar["atama_miktari_ek"] = 0
                        superhucredeki_trafolar["NegatifFark"] = 0.0
        
                        
                        if not Atanmadı_Cikti:
                            
                            if ((toplam_boskapasite_ek > yuk_miktari and toplam_boskapasite_ek > 0) or yuk_miktari <= 0):
                                
                                # Payda sıfırsa atama işlemi yapmadan önce kontrol ekleyelim
                                superhucredeki_trafolar["atama_orani"] = np.where(
                                    toplam_boskapasite != 0, 
                                    superhucredeki_trafolar["bos_kapasite"] / toplam_boskapasite, 
                                    0  # Payda sıfırsa 0 değeri atansın
                                )
                                
                                superhucredeki_trafolar["atama_orani_ek"] = np.where(
                                    toplam_boskapasite_ek != 0, 
                                    superhucredeki_trafolar["efektif_bos_kapasite"] / toplam_boskapasite_ek, 
                                    0  # Payda sıfırsa 0 değeri atansın
                                )
                                               
                                superhucredeki_trafolar["atama_miktari"] = yuk_miktari * superhucredeki_trafolar["atama_orani"]
                                superhucredeki_trafolar["atama_miktari_ek"] = yuk_miktari * superhucredeki_trafolar["atama_orani_ek"]
                                
                                superhucredeki_trafolar["GelenYukToplamGecici"] = superhucredeki_trafolar["GelenYukToplam"].copy()

                                superhucredeki_trafolar["GelenYukToplam"] += superhucredeki_trafolar["atama_miktari_ek"]
                                
                                # Negatif farkı hesapla (0'dan küçük olanlar, diğerleri 0)
                                superhucredeki_trafolar["NegatifFark"] = superhucredeki_trafolar["GelenYukToplam"].where(
                                    superhucredeki_trafolar["GelenYukToplam"] < 0, 0.0
                                )
                                
                                superhucredeki_trafolar["atama_miktari"] = np.where(
                                    superhucredeki_trafolar["GelenYukToplam"] < 0,
                                    -1 * superhucredeki_trafolar["GelenYukToplamGecici"],
                                    superhucredeki_trafolar["atama_miktari"]
                                )
                                
                                superhucredeki_trafolar["atama_miktari_ek"] = np.where(
                                    superhucredeki_trafolar["GelenYukToplam"] < 0,
                                    -1 * superhucredeki_trafolar["GelenYukToplamGecici"],
                                    superhucredeki_trafolar["atama_miktari"]
                                )
                                
                                superhucredeki_trafolar["bos_kapasite"] -= superhucredeki_trafolar["atama_miktari"]
                                superhucredeki_trafolar["bos_kapasite"] = superhucredeki_trafolar[
                                    "bos_kapasite"
                                ]
                                
                                superhucredeki_trafolar["efektif_bos_kapasite"] -= superhucredeki_trafolar["atama_miktari_ek"]
                                superhucredeki_trafolar["efektif_bos_kapasite"] = superhucredeki_trafolar[
                                    "efektif_bos_kapasite"
                                ]
                                
                                # GelenYukToplam'ı 0'dan küçükse 0 yap
                                superhucredeki_trafolar.loc[
                                    superhucredeki_trafolar["GelenYukToplam"] < 0, "GelenYukToplam"
                                ] = 0
                                
                                
                                superhucredeki_trafolar.loc[(superhucredeki_trafolar["kapasite"] != 0), "Nominal_Bos_yuzde"] = (
                                    superhucredeki_trafolar["bos_kapasite"] / superhucredeki_trafolar["kapasite"]
                                )                                                               
                                
                                superhucredeki_trafolar.loc[(superhucredeki_trafolar["kapasite"] != 0), "Doluluk Oranı"] = (
                                    superhucredeki_trafolar["GelenYukToplam"] / superhucredeki_trafolar["kapasite"])
                                    
                                for row in superhucredeki_trafolar.itertuples(index=False):
                                    
                                    dftrafoYıllık.loc[((dftrafoYıllık["trafo_id"] == row.trafo_id) & (dftrafoYıllık["year"] >= yil)), "GelenYukToplam"] = row.GelenYukToplam
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
                                                                                                                                                                                              
                
                if superhucredeki_trafolar.empty:
                    
                    durum = "Atanmadı"

                    if not df["power_distribution"].eq(0).all():                                                
                        
                        for row in df.itertuples(index=False):
                            atama_sonucu_yeni.append([row.year, row.shid, row.power_distribution, 0, 0, 0, durum])
    
        self.atama_sonucu = pd.DataFrame(atama_sonucu_yeni, columns=["year", "shid", "power_distribution", "toplam_boskapasite", "GelenYukToplam", "Atanamayan Talep", "durum"])
    
    def cumsum_from_first_positive(self,series):
        first_pos_idx = series[series > 0].index.min()
        result = pd.Series(0, index=series.index)
        if pd.notna(first_pos_idx):
            result.loc[first_pos_idx:] = series.loc[first_pos_idx:].cumsum()
        return result
    
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


    def max_cumsum_from_any_positive(self, series):
        arr = series.to_numpy()
        max_sum = 0
        for i in range(len(arr)):
            if arr[i] > 0:
                cum_sum = np.cumsum(arr[i:])[-1]
                if cum_sum > max_sum:
                    max_sum = cum_sum
        return max_sum
    
    def cumsum_from_largest_positive(self, series):
        """
        Serideki en büyük pozitif değerin index'inden başlayarak
        kümülatif toplam yapar ve sonucu döndürür.
        """
        # Negatif veya boş seri varsa hemen sıfır döndür
        if (series <= 0).all():
            return 0
    
        # En büyük pozitif değerin index'ini bul
        max_val = series[series > 0].max()
        max_idx = series[series == max_val].index[0]  # ilk eşleşeni al
    
        # O index'ten başlayarak cumsum al
        cumsum_result = series.loc[max_idx:].cumsum().iloc[-1]
    
        return cumsum_result
    
    def cumsum_from_all_positive_starts(self, series):
        """
        Tüm pozitif değerlerden itibaren yapılan cumsum'ların en büyüğünü bulur.
        Ancak sadece cumsum değil, tek başına pozitif sayı da max olabilir (örneğin sonrası hep negatifse).
        """
        series = series.copy()
        max_sum = 0
    
        positive_indices = series[series > 0].index
    
        for idx in positive_indices:
            sub_series = series.loc[idx:]
            cum_values = sub_series.cumsum()
            max_cum_sum = cum_values.iloc[-1]
            max_sum = max(max_sum, sub_series.iloc[0], max_cum_sum)  # hem ilk sayı, hem cumsum
    
        return max_sum

    def from_first_positive(self, group):
        # İlk pozitif değerin index'ini bul
        first_pos_idx = group[group["power_distribution"] > 0].first_valid_index()
        
        if first_pos_idx is not None:
            return group.loc[first_pos_idx:]
        else:
            return pd.DataFrame(columns=group.columns)  # pozitif değer yoksa boş döndür

        

    def TrafoEkleTarimsalSulama(self, df, TD="Tarımsal", ST=True):    
        
        odtr = config3.get()
        
        try:
            index = self.p["ozel_trafo_liste"].index(400)
            
        except ValueError:
            index = len(self.p["ozel_trafo_liste"])
            
        ozel_trafo_liste = self.p["ozel_trafo_liste"][:index+1].copy()
        
        dfyeni_trafo = pd.DataFrame()
        dfyeni_trafoyıllık = pd.DataFrame()
        
        #dfsuperhucre = pd.concat([results[y][0] for y in results], ignore_index=True)
        dfsuperhucre = ReadParquet(odtr["dfsuperhucre"])
        
        #dfhucre_super = results[2024][1]
        dfhucre_super = ReadParquet(odtr["dfhucre_super"])
        
        yeni_trafo_list = []
        
        df = df[df["year"] != odtr["ilk_yil"]-1]
        # Her shid için toplam kümülatif yükü hesapla
        
        # df = df[df["power_distribution"]>0].copy()
        # df = atanamayanlar.copy()
        
        valid_shids = df.groupby("shid")["power_distribution"].sum()
        
        valid_shids = valid_shids[valid_shids > 0].index
        
        # 2. Bu shid’lere sahip satırları filtrele
        df = df[df["shid"].isin(valid_shids)].copy()

        # shid_total_yuk = df.groupby("shid")["power_distribution"].sum().reset_index()
        
        df.sort_values(by=["shid", "year"], inplace=True)
        
        # 3. Her shid için kümülatif toplam (zaman içinde birikimli yük)
        # df["kumulatif_yuk_cumsum"] = df.groupby("shid")["power_distribution"].cumsum()

        df["kumulatif_yuk_cumsum"] = (
            df.groupby("shid")["power_distribution"]
              .transform(self.cumsum_from_first_positive_and_reset_on_negative)
        )
          
        # df["kumulatif_yuk_cumsum"] = (
        #     df.groupby("shid")["power_distribution"]
        #       .apply(self.max_cumsum_from_any_positive)
        #       .reset_index(name="max_kumulatif_yuk")
        # )
        
        # shid_max_kumulatif = (
        #     df.groupby("shid")["power_distribution"]
        #       .apply(self.cumsum_from_all_positive_starts)
        #       .reset_index(name="kumulatif_yuk_cumsum")
        # )
      
        # 4. Her shid için bu cumsum’un maksimumunu al
        shid_max_kumulatif = df.groupby("shid")["kumulatif_yuk_cumsum"].max().reset_index()
        
        # Her shid için ilk görüldüğü yılı bul
        # Her shid için ilk görüldüğü yılı bul
        df2 = df.groupby("shid", group_keys=False).apply(self.from_first_positive)
        
        # df2 = df[df["power_distribution"]>0].copy()

        shid_min_year = df2.groupby("shid")["year"].min().reset_index()
        
        # Bu iki bilgiyi birleştirerek toplam yükü ilk yıla aktarma
        df_final = shid_min_year.merge(shid_max_kumulatif, on="shid")
        
        df_final2 = df_final.merge(df[["shid", "year"]], on = ["shid", "year"], how="left")
        
        df_final2 = df_final2.sort_values(by="year", ascending=True).reset_index(drop=True)
    
        for index, row in df_final2.iterrows():
            
            shid = row["shid"]
            
            gerekli_kapasite = row["kumulatif_yuk_cumsum"]
            print(f"{shid} -- {gerekli_kapasite}")
            yil = row["year"]
        
            trafo_list = pd.DataFrame(columns=["sayac", "hucre_id", "kapasite", "efektif_kapasite"])
            sayac = 0
            break_outer_loop = False
        
            shid_hucreler = self.data[self.data["shid"] == shid]["id"].unique()
        
            for i, hucre in enumerate(shid_hucreler):
                while True:
                    for kapasite in ozel_trafo_liste:
                        # efektif_miktar = kapasite * self.p["yeni_trafo_kapasite_kullanim_ust_limiti"]
                        efektif_miktar = kapasite * 0.7

                        yeni_efektif = trafo_list["efektif_kapasite"].sum() + efektif_miktar
                    
                        # Trafo her durumda geçici olarak tanımlanır
                        yeni_trafo = pd.DataFrame({
                            "sayac": [sayac],
                            "hucre_id": [hucre],
                            "kapasite": [kapasite],
                            "efektif_kapasite": [efektif_miktar],
                            "Koord_x": None,
                            "Koord_y": None
                        })
                    
                        # Trafoyu kaydet (artık silme yok)
                        trafo_list = pd.concat([trafo_list, yeni_trafo], ignore_index=True)
                    
                        if yeni_efektif >= gerekli_kapasite:
                            break_outer_loop = True
                            break  # Tüm işlemler biter
                    
                        elif kapasite == ozel_trafo_liste[-1]:
                            # Liste sonuna geldik, ama kapasite yetmedi
                            # Trafo zaten eklendi, sonraki hücreye geçmek için dış döngüye çık
                            break
                        
                        else:
                            trafo_list = trafo_list.iloc[:-1]
                        
                        
        
                    if break_outer_loop:
                        break  # while'dan çık
                    
                    sayac += 1
                    
                    if i < len(shid_hucreler) - 1:
                        break  # bir sonraki hücreye geç
                    else:
                        continue  # son hücredeyiz → aynı hücreye eklemeye devam

        
            yukler = df2[df2["shid"] == shid].copy()
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
                dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(
                    trafo, self.dfhucre_super, ilkyil, int(tesis_yili),
                    None, self.p, bolge="", mulkiyet=0, musteri=TD
                )
                dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])
            
                if break_occurred:
                    yukler = kalan_yukler
            
                    # Eğer son trafodaysan ve hâlâ yük varsa → aynı trafoyu tekrar ekle
                    if idtrafo == len(trafo_list) - 1:
                        # ekstra trafo daha
                        dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(
                            trafo, self.dfhucre_super, ilkyil, int(tesis_yili),
                            None, self.p, bolge="", mulkiyet=0, musteri=TD
                        )
                        dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                        dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])
                else:
                    break  # tüm yük karşılandıysa çık

                                                           
        return dfyeni_trafo.reset_index(drop=True), dfyeni_trafoyıllık.reset_index(drop=True)           

    def TrafoEkle(self, TD, ST=True):    
        
        odtr = config3.get()
        
        yeni_trafo_list = []
        dfyeni_trafo = pd.DataFrame()
        dfyeni_trafoyıllık = pd.DataFrame()
        
        df = self.data2.copy()
        
        df = df[df["year"] != odtr["ilk_yil"]-1]
        
        df = df[df["power_distribution"]>0].copy()
        
        shid_total_yuk = df.groupby("ID")["power_distribution"].sum().reset_index()
        
        # Her shid için ilk görüldüğü yılı bul
        shid_min_year = df.groupby("ID")["year"].min().reset_index()
        
        # Bu iki bilgiyi birleştirerek toplam yükü ilk yıla aktarma
        df_final = shid_min_year.merge(shid_total_yuk, on="ID")
        
        df_final2 = df_final.merge(df[["ID", "year"]], on = ["ID", "year"], how="left")
        
        df_final2 = df_final2.sort_values(by="year", ascending=True).reset_index(drop=True)
    
        for index, row in df_final2.iterrows():
              
            ID = row["ID"]
            
            toplam_atanamayan_yuk = row["power_distribution"]
            
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
                        "Koord_x": None,
                        "Koord_y": None
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
            
            yukler = df[df["ID"]==ID].copy()
            
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
    
                        dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfhucre_super, ilkyil, int(tesis_yili), None, self.p, mulkiyet=0, musteri=TD)
                        
                        dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                        dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])   
                        
                        mask = yukler["year"] >= yil
                        
                        yukler = yukler[mask]
                        
                        break
                    
                    if idtrafo == len(trafo_list) - 1:
                        
                        dftrafo, dftrafoyıllık = yeni_trafo_df_olustur(trafo, self.dfhucre_super, ilkyil, int(tesis_yili), None, self.p, mulkiyet=0, musteri=TD)
                        
                        dfyeni_trafo = pd.concat([dfyeni_trafo, dftrafo])
                        dfyeni_trafoyıllık = pd.concat([dfyeni_trafoyıllık, dftrafoyıllık])
                        
                        break
                                                           
        return dfyeni_trafo.reset_index(drop=True), dfyeni_trafoyıllık.reset_index(drop=True)

    
def OzelTrafoIslem2():
    
    odtr = config3.get()
    
    parametre_path = os.path.join(odtr["dosyalar"], "katsayılar.xlsx")
    
    df = pd.read_excel(parametre_path, engine="openpyxl")  # Daha hızlı olması için openpyxl kullanılır
    df = df.iloc[:,[0,3]]
    p = config3.Start(df)

    #dftrafo = SqliteOku("dftrafo")
    #dftrafo = ReadParquet(odtr['trafo_path_parquet'])
    dftrafo, dftrafoYıllık = TrafoOlustur(odtr, p, mulkiyet=0)
    
    dfhucre_super = ReadParquet(odtr["dfhucre_super"])
    
    dftrafo["shid"] = dftrafo["merkez_hucre"].map(dfhucre_super.set_index("hucreid")["shid"])
    dftrafoYıllık["shid"] = dftrafoYıllık["merkez_hucre"].map(dfhucre_super.set_index("hucreid")["shid"])
    
    dftrafo, dff = TrafoKapasiteArtirGuncel3(p, dftrafo, dftrafoYıllık, dfhucre_super)
    
    dftrafo = TrafoYenile(dftrafo, dftrafoYıllık)
    
    return dftrafo, dftrafoYıllık
