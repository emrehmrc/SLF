# -*- coding: utf-8 -*-
"""
Created on Mon Apr 21 18:52:21 2025

@author: vural.bayrakli
"""

import pandas as pd
from PointLoadSinif2 import PointLoadKismi2, OzelTrafoIslem2, Paralelİslem_Batch
import config3
from datetime import datetime
from SuperHucreAlgoritma12 import *
from KirsalAlanAlgoritmasi import KirsalAlan
from TrafoKoordinatEkle import KoordinatHesapla
import argparse
import sys
from veriOtomasyon import VeriOtomasyon
import shutil
from EA import EA
from PydeckRun import PydeckKismi
import time
from Functions3 import e

odtr = None

def KurumTrafoKismi(odtr, p):
    
    sonuc_yolu = os.path.join(odtr['excel_kayit_path'])
    
    #os.makedirs(sonuc_yolu, exist_ok=True)  # `exist_ok=True`, dizin zaten varsa hata vermez
    
    test = Run(p, None, sonuc_yolu)
    
    test.Process()
    
    return test

def KirsalAlanKismi(trafo, trafoYıllık, p, odtr):
    
    #trafo, df_binary, aksiyon_df = FonkTrafo()
    
    # df = ReadParquet(odtr["yuk_path_parquet"])
    
    db_path = odtr.get("yuk_db_yolu", "")
    df = SqliteOkuYuk(db_path, odtr["yuk_db_adi"])
    
    
    trafo_alanlari = SqliteOku(f"trafo_alanlari_{odtr['ilce']}")
    #trafo_alanlari = ReadParquet(odtr["trafo_alanlari_parquet"])
    
    mask = trafo_alanlari[trafo_alanlari["kent_disi_alan"]==1]["hucre_id"].unique()
    
    df = df[df["id"].isin(mask)]
    
    try:
        
        dfsuperhucre = ReadParquet(odtr["dfsuperhucre"])
        
        dfhucre_super = ReadParquet(odtr["dfhucre_super"])
    
    except Exception as e:
        
        print(e)
        
        sys.exit()
    
    df["shid"] = df["id"].map(dfhucre_super.set_index("hucreid")["shid"]) 
        
    ka = KirsalAlan(df, trafo, trafoYıllık, dfsuperhucre, dfhucre_super, p, odtr)
    
    return ka

def EAKismi(odtr, p, trafo, trafoYıllık):
    
    df = None
    
    ea = None
    
    try:
        
        df = SqliteOkuYuk(odtr["EAVeritabani"], odtr["EAVeri_db_adi"])
        
    except e:
        
        print(e)
    
    if df is not None:
        
        hepsiSıfır = df.groupby("id")["toplam_yuk"].apply(lambda x: (x==0).all())
        
        power_to_drop = hepsiSıfır[hepsiSıfır].index.tolist()
        
        df = df[~df['id'].isin(power_to_drop)]
    
        dfhucre_super = ReadParquet(odtr["dfhucre_super"])  
        
        # tea = df.merge(dfhucre_super, left_on="id", right_on="hucreid", how="left")
        
        ea = EA(df, trafo, trafoYıllık, dfhucre_super, p)
    
    return ea
    
def Cikti1(dfTrafolar, YıllıkTrafolar, odtr, dftrafoOzet, p, dfToplam):
    
    dftrafoOzet["Toplam Eklenen Kapasite (KVA)"] = 0
    
    def EAYukleri(dfTrafolar):
        
        years = pd.DataFrame({'İşlem Tarihi': list(range(odtr["ilk_yil"]-1, odtr["son_yil"]+1))})

        EATrafolar = dfTrafolar[dfTrafolar["Trafo Aksiyon"]=="yeni trafo tesis EA"].copy()
        
        if EATrafolar.empty:
            
            year = list(range(odtr["ilk_yil"]-1, odtr["son_yil"]+1))
            
            EATrafolar = pd.DataFrame({'İşlem Tarihi': year,
                                       'toplam_kapasite_ea':len(year)*[0],
                                       'trafo_sayisi_ea':len(year)*[0]})
        
          
        else:
            
            trafo_yillik_EA = EATrafolar.groupby("İşlem Tarihi").agg(
                toplam_kapasite_ea=("kapasite", "sum"),
                trafo_sayisi_ea=("kapasite", "count")
            ).reset_index()

        
        trafo_yillik_EA = years.merge(trafo_yillik_EA, on="İşlem Tarihi", how="left").fillna(0)

        trafo_yillik_EA = trafo_yillik_EA.rename(columns={"İşlem Tarihi":"year"})
        
        return trafo_yillik_EA
        
    def PointloadYukleri(dfToplam):

        def DeltaAlma2(data, pl):
            
            df_merged = data.copy() 
            
            all_years = list(range(odtr["ilk_yil"]-1, odtr["son_yil"]+1))
            
            if pl == "sanayi":
                df_merged["TOTAL_POWER"] = df_merged["SANAYI_POINT_LOAD"].copy()
                
            if pl == "ticarethane":
                df_merged["TOTAL_POWER"] = df_merged["TICARETHANE_POINT_LOAD"].copy()
       
            if pl == "tarimsal":
                
                try:
                    df_merged = df_merged.groupby(["shid", "year"])["TARIMSAL_SULAMA"].sum().reset_index()
                    
                    df_merged["TOTAL_POWER"] = df_merged["TARIMSAL_SULAMA"].copy()
                    
                    df_merged["TOTAL_POWER"] = (df_merged["TOTAL_POWER"] / p["toplam_saat"]) * p["toplam_saat_katsayi"] * 1000 / 0.9
                    
                    df_pivot = df_merged.pivot(index="shid", columns="year", values="TOTAL_POWER").reset_index()
                      
                    df_pivot = df_pivot.set_index("shid")
                    
                    df_diff = df_pivot.diff(axis=1)
                    
                    df_diff.iloc[:, 0] = df_pivot.iloc[:, 0]  # 2024 yılı verisini bozmamak için
                    
                    df_diff = df_diff.reset_index()
                    
                    df_result = df_diff.melt(id_vars="shid", var_name="year", value_name="power_distribution")
                    
                    return df_result
                
                except:
                    df_result = pd.DataFrame({
                        "year": all_years,
                        "power_distribution": len(all_years)*[0]
                        }) 
                    
                    return df_result
                    
                    
            try:
                
                df_merged = df_merged.rename(columns={"id":"ID"})
                
                df_merged["TOTAL_POWER"] = (df_merged["TOTAL_POWER"] / p["toplam_saat"]) * p["toplam_saat_katsayi"] * 1000 / 0.9
                
                df_pivot = df_merged.pivot(index="ID", columns="year", values="TOTAL_POWER").reset_index()
                  
                df_pivot = df_pivot.set_index("ID")
                
                df_diff = df_pivot.diff(axis=1)
                
                df_diff.iloc[:, 0] = df_pivot.iloc[:, 0]  # 2024 yılı verisini bozmamak için
                
                df_diff = df_diff.reset_index()
                
                df_result = df_diff.melt(id_vars="ID", var_name="year", value_name="power_distribution")
                
                return df_result
            
            except:
                df_result = pd.DataFrame({
                    "year": all_years,
                    "power_distribution": len(all_years)*[0]
                    }) 
                
                return df_result
        
        try:
            
            df_BT = SqliteOku(f"df_BT_{odtr['ilce']}")
            
            df_BT = DeltaAlma2(df_BT, "ticarethane")
            
            yillik_toplamdf_BT = df_BT.groupby('year')['power_distribution'].sum().reset_index()
        
        except:
            
            years = list(range(odtr["ilk_yil"]-1, odtr["son_yil"]+1))
            
            power_distribution = [0] * len(years)
            
            yillik_toplamdf_BT = pd.DataFrame({'year': years, 'power_distribution':power_distribution})

            
        try:
            
            df_BS = SqliteOku(f"df_BS_{odtr['ilce']}")
            
            df_BS = DeltaAlma2(df_BS, "sanayi")
            
            yillik_toplamdf_BS = df_BS.groupby('year')['power_distribution'].sum().reset_index()
        
        except:
            
            years = list(range(odtr["ilk_yil"]-1, odtr["son_yil"]+1))
            
            power_distribution = [0] * len(years)
            
            yillik_toplamdf_BS = pd.DataFrame({'year': years, 'power_distribution':power_distribution})


        try:
            
            df_TS = SqliteOku(f"df_TS_{odtr['ilce']}")
            
            df_TS = DeltaAlma2(df_TS, "tarimsal")
            
            yillik_toplamdf_TS = df_TS.groupby('year')['power_distribution'].sum().reset_index()
            
        except:
            
            years = list(range(odtr["ilk_yil"]-1, odtr["son_yil"]+1))
            
            power_distribution = [0] * len(years)
            
            yillik_toplamdf_TS = pd.DataFrame({'year': years, 'power_distribution':power_distribution})
        
        years = pd.DataFrame({'İşlem Tarihi': range(odtr["ilk_yil"]-1, odtr["son_yil"]+1)})

        # Merge işlemi ile üç DataFrame'i birleştir
        yillik_toplamdf = yillik_toplamdf_BT.merge(
            yillik_toplamdf_BS, on='year', how='outer', suffixes=('_BT', '_BS')
        ).merge(
            yillik_toplamdf_TS, on='year', how='outer', suffixes=('', '_TS')
        )
        
        # 'power_distribution' sütunlarını topla
        yillik_toplamdf['toplam_power_distribution'] = (
            yillik_toplamdf['power_distribution_BT'] + 
            yillik_toplamdf['power_distribution_BS'] + 
            yillik_toplamdf['power_distribution']
        )
        
        yillik_toplamdf['kümülatif_power_distribution'] = yillik_toplamdf['toplam_power_distribution'].cumsum()
        
        dfTrafolarYaştanYenilemeÖzel = dfToplam[(dfToplam["Trafo Aksiyon"] == "trafo yenileme-yaştan")                                            
                                              ].copy()
        
        # Güç artırımı verisini yıllara göre gruplayıp, toplam kapasite ve trafo sayısını hesaplıyoruz
        trafo_yillik_yaştan_yenileme = dfTrafolarYaştanYenilemeÖzel.groupby("İşlem Tarihi").agg(
            toplam_kapasite_yaştan_yenileme=("kapasite", "sum"),
            trafo_sayisi_yaştan_yenileme=("kapasite", "count")
        ).reset_index()
        
        # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
        trafo_yillik_yaştan_yenileme = years.merge(trafo_yillik_yaştan_yenileme, on="İşlem Tarihi", how="left").fillna({
            'toplam_kapasite_yenileme': 0,
            'trafo_sayisi_yaştan_yenileme': 0
        })
        
        dfTrafolarKapasiteYenilemeÖzel = dfToplam[(dfToplam["Trafo Aksiyon"] == "trafo yükseltme-kapasiteden")                                           
                                              ].copy()
        
        trafo_yillik_kapasite_yenileme = dfTrafolarKapasiteYenilemeÖzel.groupby("İşlem Tarihi").agg(
        toplam_kapasite_yeni=("kapasite", "sum"),
        toplam_kapasite_eski=("eski kapasite", "sum"),
        trafo_sayisi_kapasite_yenileme=("kapasite", "count")
        ).reset_index()
        
        
        # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
        trafo_yillik_kapasite_yenileme = years.merge(trafo_yillik_kapasite_yenileme, on="İşlem Tarihi", how="left").fillna({
            'toplam_kapasite_yeni': 0,
            'toplam_kapasite_eski': 0,
            'trafo_sayisi_kapasite_yenileme': 0
        })
        
        # Kapasite artırımını hesaplamak için 'toplam_kapasite_yeni' ve 'toplam_kapasite_eski' farkını yeni bir sütun olarak ekliyoruz
        trafo_yillik_kapasite_yenileme["toplam_kapasite_yenileme"] = trafo_yillik_kapasite_yenileme["toplam_kapasite_yeni"] - trafo_yillik_kapasite_yenileme["toplam_kapasite_eski"]
        
        
        dfTrafolarYeni = dfToplam[(dfToplam["Trafo Aksiyon"] == "yeni trafo tesis")                                          
                                              ].copy()
        
        # Güç artırımı verisini yıllara göre gruplayıp, toplam kapasite ve trafo sayısını hesaplıyoruz
        trafo_yillik_yeni = dfTrafolarYeni.groupby("İşlem Tarihi").agg(
            toplam_yeni=("kapasite", "sum"),
            trafo_sayisi_yeni=("kapasite", "count")
        ).reset_index()
        
        # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
        trafo_yillik_yeni = years.merge(trafo_yillik_yeni, on="İşlem Tarihi", how="left").fillna({
            'toplam_yeni': 0,
            'trafo_sayisi_yeni': 0
        })
        
        return yillik_toplamdf, trafo_yillik_yaştan_yenileme, trafo_yillik_kapasite_yenileme, trafo_yillik_yeni
        
        
    veri_path = os.path.join(odtr["excel_kayit_path"], "SuperHucreI1.parquet")
    veri = pd.read_parquet(veri_path)
    dfYuk = veri.copy()
    
    # 1. Yıllık toplamı hesapla
    yillik_toplam = dfYuk.groupby('year')['delta_TOTAL_POWER_KVA'].sum().reset_index()

    # 2. Kümülatif toplamı oluştur
    yillik_toplam['kümülatif_TOTAL_POWER_KVA'] = yillik_toplam['delta_TOTAL_POWER_KVA'].cumsum()
    
    # 2024-2035 yıllarını içeren bir DataFrame oluşturuyoruz
    years = pd.DataFrame({'İşlem Tarihi': range(odtr["ilk_yil"], odtr["son_yil"]+1)})
    
    dfTrafolarYaştanYenilemeKurum = dfTrafolar[(dfTrafolar["Trafo Aksiyon"] == "trafo yenileme-yaştan") &
                                          (dfTrafolar["Trafo Mülkiyeti"] == "Kurum")
                                          ].copy()
    
    # Güç artırımı verisini yıllara göre gruplayıp, toplam kapasite ve trafo sayısını hesaplıyoruz
    trafo_yillik_yaştan_yenileme = dfTrafolarYaştanYenilemeKurum.groupby("İşlem Tarihi").agg(
        toplam_kapasite_yaştan_yenileme=("kapasite", "sum"),
        trafo_sayisi_yaştan_yenileme=("kapasite", "count")
    ).reset_index()
    
    # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
    trafo_yillik_yaştan_yenileme = years.merge(trafo_yillik_yaştan_yenileme, on="İşlem Tarihi", how="left").fillna({
        'toplam_kapasite_yaştan_yenileme': 0,
        'trafo_sayisi_yaştan_yenileme': 0
    })
    
    trafo_yillik_yaştan_yenileme = trafo_yillik_yaştan_yenileme.rename(columns={"İşlem Tarihi": "year"})
    
    dfTrafolarKapasiteYenilemeYuktenKurum = dfTrafolar[(dfTrafolar["Trafo Aksiyon"] == "trafo yükseltme-yükten") &
                                          (dfTrafolar["Trafo Mülkiyeti"] == "Kurum")
                                          ].copy()
    
    # Güç artırımı verisini yıllara göre gruplayıp, toplam kapasite ve trafo sayısını hesaplıyoruz
    trafo_yillik_kapasite_yenileme_yükten = dfTrafolarKapasiteYenilemeYuktenKurum.groupby("İşlem Tarihi").agg(
        toplam_kapasite_yeni=("kapasite", "sum"),
        toplam_kapasite_eski=("eski kapasite", "sum"),
        trafo_sayisi_kapasite_yenileme=("kapasite", "count")
    ).reset_index()
    
    # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
    trafo_yillik_kapasite_yenileme_yükten = years.merge(trafo_yillik_kapasite_yenileme_yükten, on="İşlem Tarihi", how="left").fillna(0)
    
    trafo_yillik_kapasite_yenileme_yükten["toplam_kapasite_yenileme"] = trafo_yillik_kapasite_yenileme_yükten["toplam_kapasite_yeni"] - trafo_yillik_kapasite_yenileme_yükten["toplam_kapasite_eski"]
    
    trafo_yillik_kapasite_yenileme_yükten = trafo_yillik_kapasite_yenileme_yükten.rename(columns={"İşlem Tarihi": "year"})

    dfTrafolarKapasiteYenilemeKurum = dfTrafolar[(dfTrafolar["Trafo Aksiyon"] == "trafo yükseltme-kapasiteden") &
                                          (dfTrafolar["Trafo Mülkiyeti"] == "Kurum")
                                          ].copy()
    
    # Güç artırımı verisini yıllara göre gruplayıp, toplam kapasite ve trafo sayısını hesaplıyoruz
    trafo_yillik_kapasite_yenileme = dfTrafolarKapasiteYenilemeKurum.groupby("İşlem Tarihi").agg(
        toplam_kapasite_yenileme_yeni=("kapasite", "sum"),
        toplam_kapasite_yenileme_eski=("eski kapasite", "sum"),
        trafo_sayisi_kapasite_yenileme=("kapasite", "count")
    ).reset_index()
    
    # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
    trafo_yillik_kapasite_yenileme = years.merge(trafo_yillik_kapasite_yenileme, on="İşlem Tarihi", how="left").fillna(0)
    
    trafo_yillik_kapasite_yenileme["toplam_kapasite_yenileme"] = trafo_yillik_kapasite_yenileme["toplam_kapasite_yenileme_yeni"] - trafo_yillik_kapasite_yenileme["toplam_kapasite_yenileme_eski"]
    
    trafo_yillik_kapasite_yenileme = trafo_yillik_kapasite_yenileme.rename(columns={"İşlem Tarihi": "year"})

    dfTrafolarGucArtirimi = dfTrafolar[dfTrafolar["Trafo Aksiyon"] == "güç artırımı"].copy()
    # Güç artırımı verisini yıllara göre gruplayıp, toplam kapasite ve trafo sayısını hesaplıyoruz
    trafo_yillik_guc_artirimi = dfTrafolarGucArtirimi.groupby("İşlem Tarihi").agg(
    toplam_kapasite_yeni=("kapasite", "sum"),
    toplam_kapasite_eski=("eski kapasite", "sum"),
    trafo_sayisi_guc_artirimi=("kapasite", "count")
    ).reset_index()
    
    # Kapasite artırımını hesaplamak için 'toplam_kapasite_yeni' ve 'toplam_kapasite_eski' farkını yeni bir sütun olarak ekliyoruz
    trafo_yillik_guc_artirimi["toplam_kapasite_guc_artirimi"] = trafo_yillik_guc_artirimi["toplam_kapasite_yeni"] - trafo_yillik_guc_artirimi["toplam_kapasite_eski"]

    
    # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
    trafo_yillik_guc_artirimi = years.merge(trafo_yillik_guc_artirimi, on="İşlem Tarihi", how="left").fillna(0)
    
    trafo_yillik_guc_artirimi = trafo_yillik_guc_artirimi.rename(columns={"İşlem Tarihi": "year"})

    def gerilim_grubu(g):
        if g in [31.5, 34.5, 33.0]:
            return "YÜKSEK"
        elif g in [6.3, 10.5, 10.0, 6.0]:
            return "ORTA"
        elif g in [15.0, 20.0]:
            return "DİĞER"
        else:
            return "BİLİNMEYEN"
        
    # 2. Gerilim Dönüşümü için
    dfTrafolarDonusum = dfTrafolar[dfTrafolar["Trafo Aksiyon"] == "gerilim dönüşümü"].copy()
    
    trafo_yillik_donusum = dfTrafolarDonusum.groupby("İşlem Tarihi").agg(
        toplam_kapasite_donusum=("kapasite", "sum"),
        trafo_sayisi_donusum=("kapasite", "count")
    ).reset_index()
    
    dfTrafolarDonusum["Bilgi"] = (
    dfTrafolarDonusum["Eski Gerilim"].astype(str)
    + " --> "
    + dfTrafolarDonusum["Gerilim"].astype(str)
    + " Dönüşümü"
    )

    
    # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
    trafo_yillik_donusum = years.merge(trafo_yillik_donusum, on="İşlem Tarihi", how="left").fillna(0)
    
    trafo_yillik_donusum = trafo_yillik_donusum.rename(columns={"İşlem Tarihi": "year"})
    
    ## YENİ TRAFOLAR DÖNÜŞÜM
    df = dfTrafolar[(dfTrafolar["Trafo Aksiyon"] == "yeni trafo tesis")].copy()
    
    #df = KoordinatHesapla(df)
    columns = ["trafo_id", "year", "Gerilim", "kapasite", "Koord_x", "Koord_y", "SH Gerilim"]
    
    df = df[columns]
    
    df["EskiGrup"] = df["SH Gerilim"].apply(gerilim_grubu)
    df["YeniGrup"] = df["Gerilim"].apply(gerilim_grubu)
    df["DÖNÜŞÜM VAR MI"] = (df["EskiGrup"] != df["YeniGrup"]) & df["SH Gerilim"].notna()
    
    # df["DÖNÜŞÜM VAR MI"] = (
    #     df["Gerilim"] != df["SH Gerilim"]
    # ) & df["SH Gerilim"].notna()  # sadece karşılaştırma yap anlamlıysa

    df["DÖNÜŞÜM VAR MI"] = df["DÖNÜŞÜM VAR MI"].map({True: "VAR", False: "YOK"})
    
    df_donusum = df[df["DÖNÜŞÜM VAR MI"] == "VAR"]
    
    donusum_trafo_yeniler_ids = df_donusum["trafo_id"].values

    istatistik = df_donusum.groupby("year").agg(
        Donusum_Adedi = ("trafo_id", "count"),
        Toplam_Kapasite = ("kapasite", "sum")
    ).reset_index()
    
    df_donusum["Bilgi"] = (
    df_donusum["SH Gerilim"].astype(str)
    + " --> "
    + df_donusum["Gerilim"].astype(str)
    + " Dönüşümü"
    )
    

    # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
    istatistik = years.merge(istatistik, right_on = "year", left_on="İşlem Tarihi", how="left").fillna(0)
    
    dfTrafolarYeni = dfTrafolar[(dfTrafolar["Trafo Aksiyon"] == "yeni trafo tesis") & (dfTrafolar["Trafo Mülkiyeti"] == "Kurum")].copy()
    
    dfTrafolarYeni = dfTrafolarYeni[~dfTrafolarYeni["trafo_id"].isin(donusum_trafo_yeniler_ids)]
    
    # Güç artırımı verisini yıllara göre gruplayıp, toplam kapasite ve trafo sayısını hesaplıyoruz
    trafo_yillik_yeni_kapasite = dfTrafolarYeni.groupby("İşlem Tarihi").agg(
        toplam_kapasite_yeni=("kapasite", "sum"),
        trafo_sayisi_yeni=("kapasite", "count")
    ).reset_index()
    
    # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
    trafo_yillik_yeni_kapasite = years.merge(trafo_yillik_yeni_kapasite, on="İşlem Tarihi", how="left").fillna(0)
    
    ## KAPASİTESİ ARTAN TRAFOLAR DONÜŞÜM
    
    df = dfTrafolar[(dfTrafolar["Trafo Aksiyon"] == "trafo yükseltme-kapasiteden") | (dfTrafolar["Trafo Aksiyon"] == "trafo yükseltme-yükten")].copy()

    columns = ["trafo_id", "İşlem Tarihi", "Eski Gerilim", "Gerilim", "kapasite", "eski kapasite", "Koord_x", "Koord_y"]
    
    df = df[columns]
    
    # df["DÖNÜŞÜM VAR MI"] = df["Eski Gerilim"] != df["Gerilim"]
    df["EskiGrup"] = df["Eski Gerilim"].apply(gerilim_grubu)
    df["YeniGrup"] = df["Gerilim"].apply(gerilim_grubu)
    df["DÖNÜŞÜM VAR MI"] = df["EskiGrup"] != df["YeniGrup"]

    df["DÖNÜŞÜM VAR MI"] = df["DÖNÜŞÜM VAR MI"].map({True: "VAR", False: "YOK"})
    
    df_donusum_kapasite = df[df["DÖNÜŞÜM VAR MI"] == "VAR"]
    
    donusum_trafo_kapasitesi_artanlar_ids = df_donusum_kapasite["trafo_id"].values
    
    istatistik_kapasite_artanlar = df_donusum_kapasite.groupby("İşlem Tarihi").agg(
        Donusum_Adedi = ("trafo_id", "count"),
        Toplam_Kapasite = ("kapasite", "sum")
    ).reset_index()
    
    df_donusum_kapasite["Bilgi"] = (
    df_donusum_kapasite["Eski Gerilim"].astype(str)
    + " --> "
    + df_donusum_kapasite["Gerilim"].astype(str)
    + " Dönüşümü"
    )
    
    # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
    istatistik_kapasite_artanlar = years.merge(istatistik_kapasite_artanlar, on="İşlem Tarihi", how="left").fillna(0)

    dfTrafolarDeplase = dfTrafolar[dfTrafolar["Trafo Aksiyon"] == "deplase"].copy()
    
    trafo_yillik_deplase = dfTrafolarDeplase.groupby("İşlem Tarihi").agg(
        toplam_kapasite_deplase=("kapasite", "sum"),
        trafo_sayisi_deplase=("kapasite", "count")
    ).reset_index()
    
    # Yıllara göre birleştiriyoruz, eksik yıllar için 0 değerler olacak
    trafo_yillik_deplase = years.merge(trafo_yillik_deplase, on="İşlem Tarihi", how="left").fillna({
        'toplam_kapasite_deplase': 0,
        'trafo_sayisi_deplase': 0
    })
    
    grup_sayilari_donusum_kapasite = (
        df_donusum_kapasite
        .groupby(["İşlem Tarihi", "Bilgi"])
        .size()
        .reset_index(name="Adet")
    )
    
    grup_sayilari_donusum_yeni = (
        df_donusum
        .groupby(["year", "Bilgi"])
        .size()
        .reset_index(name="Adet")
    )
    
    grup_sayilari_donusum = (
        dfTrafolarDonusum
        .groupby(["year", "Bilgi"])
        .size()
        .reset_index(name="Adet")
    )
    
    trafo_yillik_deplase = trafo_yillik_deplase.rename(columns={"İşlem Tarihi": "year"})
    
    
    
    
    dfmerged = yillik_toplam.merge(trafo_yillik_yeni_kapasite, left_on="year", right_on="İşlem Tarihi", how="left")
    #dfmerged = dfmerged.merge(yillik_ortalama, on="year", how="left")
    dfmerged = dfmerged.merge(dftrafoOzet, on="year", how="left")
    dfmerged = dfmerged.merge(trafo_yillik_guc_artirimi, on="year", how="left")
    dfmerged = dfmerged.merge(trafo_yillik_donusum, on="year", how="left")
    dfmerged = dfmerged.merge(trafo_yillik_deplase, on="year", how="left")
    dfmerged = dfmerged.merge(trafo_yillik_yaştan_yenileme, on="year", how="left")
    dfmerged = dfmerged.merge(trafo_yillik_kapasite_yenileme, on="year", how="left")

    tum_veriler = []
    
    DfÖzel1, DfÖzel2, DfÖzel3, DfÖzel4 =  PointloadYukleri(dfToplam)
    
    DfEa = EAYukleri(dfTrafolar)
    
    for year in dfmerged["year"].values[1:]:
         
        talep_artisi = dfmerged.loc[dfmerged["year"]==year, "delta_TOTAL_POWER_KVA"].values
        talep_artisi = talep_artisi[0] if len(talep_artisi) > 0 else 0
        
        mevcut_talep_artisi = dfmerged.loc[dfmerged["year"]==year, "kümülatif_TOTAL_POWER_KVA"].values
        mevcut_talep_artisi = mevcut_talep_artisi[0] if len(mevcut_talep_artisi) > 0 else 0
        
        trafo_sayisi_donusum = dfmerged.loc[dfmerged["year"] == year, "trafo_sayisi_donusum"].values
        trafo_sayisi_donusum = trafo_sayisi_donusum[0] if len(trafo_sayisi_donusum) > 0 else 0
        
        toplam_kapasite_donusum = dfmerged.loc[dfmerged["year"] == year, "toplam_kapasite_donusum"].values
        toplam_kapasite_donusum = toplam_kapasite_donusum[0] if len(toplam_kapasite_donusum) > 0 else 0
            
        yeni_trafo_sayisi_donusum = istatistik.loc[istatistik["İşlem Tarihi"] == year, "Donusum_Adedi"].values
        yeni_trafo_sayisi_donusum = yeni_trafo_sayisi_donusum[0] if len(yeni_trafo_sayisi_donusum) > 0 else 0
        
        yeni_toplam_kapasite_donusum = istatistik.loc[dfmerged["İşlem Tarihi"] == year, "Toplam_Kapasite"].values
        yeni_toplam_kapasite_donusum = yeni_toplam_kapasite_donusum[0] if len(yeni_toplam_kapasite_donusum) > 0 else 0
            
        kapasite_artan_trafo_sayisi_donusum = istatistik_kapasite_artanlar.loc[istatistik["İşlem Tarihi"] == year, "Donusum_Adedi"].values
        kapasite_artan_trafo_sayisi_donusum = kapasite_artan_trafo_sayisi_donusum[0] if len(kapasite_artan_trafo_sayisi_donusum) > 0 else 0
        
        kapasite_artan_toplam_kapasite_donusum = istatistik_kapasite_artanlar.loc[dfmerged["İşlem Tarihi"] == year, "Toplam_Kapasite"].values
        kapasite_artan_toplam_kapasite_donusum = kapasite_artan_toplam_kapasite_donusum[0] if len(kapasite_artan_toplam_kapasite_donusum) > 0 else 0
            
        toplam_trafo_sayisi_donusum = trafo_sayisi_donusum + yeni_trafo_sayisi_donusum + kapasite_artan_trafo_sayisi_donusum
        toplam_kapasite_donusum = toplam_kapasite_donusum + yeni_toplam_kapasite_donusum + kapasite_artan_toplam_kapasite_donusum
            
        yeni_trafo_tesis_sayisi = trafo_yillik_yeni_kapasite.loc[trafo_yillik_yeni_kapasite["İşlem Tarihi"]==year, "trafo_sayisi_yeni"].fillna(0).values[0]
        
        yeni_trafo_kurulu_gücü = trafo_yillik_yeni_kapasite.loc[trafo_yillik_yeni_kapasite["İşlem Tarihi"]==year, "toplam_kapasite_yeni"].values
        yeni_trafo_kurulu_gücü = yeni_trafo_kurulu_gücü[0] if len(yeni_trafo_kurulu_gücü) > 0 else 0
            
        yaştan_yenileme_trafo_sayisi = dfmerged.loc[dfmerged["year"]==year, "trafo_sayisi_yaştan_yenileme"].values
        yaştan_yenileme_trafo_sayisi = yaştan_yenileme_trafo_sayisi[0] if len(yaştan_yenileme_trafo_sayisi) > 0 else 0

        yaştan_yenileme_toplam_kapasite = dfmerged.loc[dfmerged["year"]==year, "toplam_kapasite_yaştan_yenileme"].values
        yaştan_yenileme_toplam_kapasite = yaştan_yenileme_toplam_kapasite[0] if len(yaştan_yenileme_toplam_kapasite) > 0 else 0

        deplase_trafo_sayisi = dfmerged.loc[dfmerged["year"]==year, "trafo_sayisi_deplase"].values
        deplase_trafo_sayisi = deplase_trafo_sayisi[0] if len(deplase_trafo_sayisi) > 0 else 0

        deplase_toplam_kapasite = dfmerged.loc[dfmerged["year"]==year, "toplam_kapasite_deplase"].values
        deplase_toplam_kapasite = deplase_toplam_kapasite[0] if len(deplase_toplam_kapasite) > 0 else 0  
        
        guc_artirimi_trafo_sayisi = dfmerged.loc[dfmerged["year"]==year, "trafo_sayisi_guc_artirimi"].values
        guc_artirimi_toplam_kapasite = dfmerged.loc[dfmerged["year"]==year, "toplam_kapasite_guc_artirimi"].values
        
        guc_artirimi_trafo_sayisi = 0 if pd.isna(guc_artirimi_trafo_sayisi) else guc_artirimi_trafo_sayisi[0]
        guc_artirimi_toplam_kapasite = 0 if pd.isna(guc_artirimi_toplam_kapasite) else guc_artirimi_toplam_kapasite[0]

        kapasite_yenileme_trafo_sayisi = dfmerged.loc[dfmerged["year"]==year, "trafo_sayisi_kapasite_yenileme"].values
        kapasite_yenileme_trafo_sayisi = kapasite_yenileme_trafo_sayisi[0] if len(kapasite_yenileme_trafo_sayisi) > 0 else 0

        kapasite_yenileme_toplam_kapasite = dfmerged.loc[dfmerged["year"]==year, "toplam_kapasite_yenileme"].values
        kapasite_yenileme_toplam_kapasite = kapasite_yenileme_toplam_kapasite[0] if len(kapasite_yenileme_toplam_kapasite) > 0 else 0

        güç_Yükseltme_toplam_trafo = guc_artirimi_trafo_sayisi + kapasite_yenileme_trafo_sayisi
        
        güç_Yükseltme_toplam_kapasite = guc_artirimi_toplam_kapasite + kapasite_yenileme_toplam_kapasite
        
        # Değeri alırken NaN kontrolü yapıp 0 atama
        deplase_trafo_sayisi = dfmerged.loc[dfmerged["year"] == year, "trafo_sayisi_deplase"].values
        deplase_trafo_sayisi = 0 if pd.isna(deplase_trafo_sayisi) else deplase_trafo_sayisi[0]
        
        
        deplase_toplam_kapasite = dfmerged.loc[dfmerged["year"] == year, "toplam_kapasite_deplase"].values
        deplase_toplam_kapasite = 0 if pd.isna(deplase_toplam_kapasite) else deplase_toplam_kapasite[0]
        
        
        
        kapasite_yenileme_yükten_kapasite = trafo_yillik_kapasite_yenileme_yükten.loc[trafo_yillik_kapasite_yenileme_yükten["year"]==year, "toplam_kapasite_yeni"].values
        kapasite_yenileme_yükten_kapasite = kapasite_yenileme_yükten_kapasite[0] if len(kapasite_yenileme_yükten_kapasite) > 0 else 0
        
        kapasite_yenileme_kapasite = trafo_yillik_kapasite_yenileme.loc[trafo_yillik_kapasite_yenileme["year"]==year, "toplam_kapasite_yenileme_yeni"].values
        kapasite_yenileme_kapasite = kapasite_yenileme_kapasite[0] if len(kapasite_yenileme_kapasite) > 0 else 0
        
        kapasite_guc_artirimi_kapasite = trafo_yillik_guc_artirimi.loc[trafo_yillik_guc_artirimi["year"]==year, "toplam_kapasite_yeni"].values
        kapasite_guc_artirimi_kapasite = kapasite_guc_artirimi_kapasite[0] if len(kapasite_guc_artirimi_kapasite) > 0 else 0

        # kapasite_yaştan_yenileme = trafo_yillik_yaştan_yenileme.loc[trafo_yillik_yaştan_yenileme["year"]==year, "toplam_kapasite_yaştan_yenileme"].fillna(0).values[0]

        kurulu_guc_artisi = yeni_trafo_kurulu_gücü + \
                            deplase_toplam_kapasite + \
                            güç_Yükseltme_toplam_kapasite + \
                            toplam_kapasite_donusum +\
                            yaştan_yenileme_toplam_kapasite
                            
        
        # Başlangıç ortalama yıl-1'den al, yoksa yıl'dan al
        baslangic_ortalama = YıllıkTrafolar.loc[YıllıkTrafolar["year"] == year-1, "Doluluk Oranı"].mean()
        
        son_ortalama = YıllıkTrafolar.loc[YıllıkTrafolar["year"]==year, "Doluluk Oranı"].mean()

        # Özel Trafo --------------------------------------
        
        ÜçüncüŞahıs_yeni_trafo_sayi = DfÖzel4.loc[DfÖzel4["İşlem Tarihi"]==year, "trafo_sayisi_yeni"].values
        ÜçüncüŞahıs_yeni_trafo_sayi = 0 if pd.isna(ÜçüncüŞahıs_yeni_trafo_sayi) else ÜçüncüŞahıs_yeni_trafo_sayi[0]
        
        ÜçüncüŞahıs_yeni_trafo_kapasite = DfÖzel4.loc[DfÖzel4["İşlem Tarihi"]==year, "toplam_yeni"].values
        ÜçüncüŞahıs_yeni_trafo_kapasite = 0 if pd.isna(ÜçüncüŞahıs_yeni_trafo_kapasite) else ÜçüncüŞahıs_yeni_trafo_kapasite[0]

        
        ÜçüncüŞahıs_kapasite_yenileme_sayi = DfÖzel3.loc[DfÖzel3["İşlem Tarihi"]==year, "trafo_sayisi_kapasite_yenileme"].values
        ÜçüncüŞahıs_kapasite_yenileme_sayi = 0 if pd.isna(ÜçüncüŞahıs_kapasite_yenileme_sayi) else ÜçüncüŞahıs_kapasite_yenileme_sayi[0]

        ÜçüncüŞahıs_kapasite_yenileme = DfÖzel3.loc[DfÖzel3["İşlem Tarihi"]==year, "toplam_kapasite_yenileme"].values
        ÜçüncüŞahıs_kapasite_yenileme = 0 if pd.isna(ÜçüncüŞahıs_kapasite_yenileme) else ÜçüncüŞahıs_kapasite_yenileme[0]


        ÜçüncüŞahıs_yaştan_yenileme_sayi = DfÖzel2.loc[DfÖzel2["İşlem Tarihi"]==year, "trafo_sayisi_yaştan_yenileme"].values
        ÜçüncüŞahıs_yaştan_yenileme_sayi = 0 if pd.isna(ÜçüncüŞahıs_yaştan_yenileme_sayi) else ÜçüncüŞahıs_yaştan_yenileme_sayi

        ÜçüncüŞahıs_yaştan_yenileme = DfÖzel2.loc[DfÖzel2["İşlem Tarihi"]==year, "toplam_kapasite_yaştan_yenileme"].values
        ÜçüncüŞahıs_yaştan_yenileme = 0 if pd.isna(ÜçüncüŞahıs_yaştan_yenileme) else ÜçüncüŞahıs_yaştan_yenileme[0]
        
        ÜçüncüŞahıs_kurulu_guc_artisi = ÜçüncüŞahıs_yeni_trafo_kapasite + \
                                        ÜçüncüŞahıs_kapasite_yenileme + \
                                        ÜçüncüŞahıs_yaştan_yenileme
        
        
        # "year" sütunu ile eşleşen satırlarda "trafo_sayisi_ea" sütununun NaN değerlerini 0 ile doldurma
        EA_yeni_trafo_sayisi = DfEa.loc[DfEa["year"] == year, "trafo_sayisi_ea"].fillna(0).values[0]

        EA_yeni_trafo_kapasite = DfEa.loc[DfEa["year"] == year, "toplam_kapasite_ea"].fillna(0).values[0]
        
        
        dftrafoOzet.loc[dftrafoOzet["year"] == year, 
                        "Toplam Eklenen Kapasite (KVA)"] = yeni_trafo_kurulu_gücü + \
                                                     kapasite_yenileme_yükten_kapasite + \
                                                     kapasite_yenileme_kapasite + \
                                                     yeni_toplam_kapasite_donusum + \
                                                     kapasite_guc_artirimi_kapasite + \
                                                     EA_yeni_trafo_kapasite + \
                                                     ÜçüncüŞahıs_yeni_trafo_kapasite + \
                                                     yaştan_yenileme_toplam_kapasite
                                                     
        
        Atanamayan_Talep = dftrafoOzet.loc[dftrafoOzet["year"] == year, "Atanamayan Talep (KVA)"].values
        
        Atanamayan_Talep = Atanamayan_Talep[0] if len(Atanamayan_Talep) > 0 else 0
        
        Atanamayan_DTR = dftrafoOzet.loc[dftrafoOzet["year"] == year, "%1 Altındaki Trafo Sayısı (Adet)"].values
        Atanamayan_DTR = Atanamayan_DTR[0] if len(Atanamayan_DTR) > 0 else 0
        
        #------------------------------------------------------------------------------------------------------------------------
        # MEVCUT TRAFO KURULU GÜCÜ VE TRAFO SAYISI
        
        dfTrafolarMevcut = YıllıkTrafolar[((YıllıkTrafolar["Trafo Aksiyon"] != "yeni trafo tesis") & (YıllıkTrafolar["Trafo Aksiyon"] != "yeni trafo tesis EA")) &
                                      (YıllıkTrafolar["Trafo Mülkiyeti"] == "Kurum") &
                                      (YıllıkTrafolar["year"] == year)
                                      ].copy()
        
        toplam_kapasite_mevcut = dfTrafolarMevcut["kapasite"].fillna(0).sum()
        
        toplam_trafo_mevcut = int(dfTrafolarMevcut["kapasite"].count())
        
        dfTrafolarMevcutÖzel = YıllıkTrafolar[((YıllıkTrafolar["Trafo Aksiyon"] != "yeni trafo tesis") | (YıllıkTrafolar["Trafo Aksiyon"] != "yeni trafo tesis EA")) &
                                      (YıllıkTrafolar["Trafo Mülkiyeti"] == "Özel") &
                                      (YıllıkTrafolar["year"] == year)
                                      ].copy()
        
        
        toplam_kapasite_mevcut_ozel = dfTrafolarMevcutÖzel["kapasite"].fillna(0).sum()
        
        toplam_trafo_mevcut_ozel = int(dfTrafolarMevcutÖzel["kapasite"].count())
        
        
        #------------------------------------------------------------------------------------------------------------------------
        # DONUSUM KIRILIMI

        
        kapasite_df = grup_sayilari_donusum_kapasite[grup_sayilari_donusum_kapasite["İşlem Tarihi"] == year][["Bilgi", "Adet"]].rename(columns={"Adet": "Adet_kapasite"})
        yeni_df     = grup_sayilari_donusum_yeni[grup_sayilari_donusum_yeni["year"] == year][["Bilgi", "Adet"]].rename(columns={"Adet": "Adet_yeni"})
        projelendirilmiş_df = grup_sayilari_donusum[grup_sayilari_donusum["year"] == year][["Bilgi", "Adet"]].rename(columns={"Adet": "Adet_projelendirilmiş"})
        
        # Merge işlemleri
        df_donusum_kirilimi = kapasite_df.merge(yeni_df, on="Bilgi", how="outer")
        df_donusum_kirilimi = df_donusum_kirilimi.merge(projelendirilmiş_df, on="Bilgi", how="outer")
        
        # NaN'leri 0 yap ve tiplere çevir
        df_donusum_kirilimi.fillna(0, inplace=True)
        df_donusum_kirilimi[["Adet_kapasite", "Adet_yeni", "Adet_projelendirilmiş"]] = df_donusum_kirilimi[["Adet_kapasite", "Adet_yeni", "Adet_projelendirilmiş"]].astype(int)
        
        # Toplam sütunu
        df_donusum_kirilimi["Adet_toplam"] = (
            df_donusum_kirilimi["Adet_kapasite"] +
            df_donusum_kirilimi["Adet_yeni"] +
            df_donusum_kirilimi["Adet_projelendirilmiş"]
        )


        #------------------------------------------------------------------------------------------------------------------------
        
        veri_kirilimi = [f"{a} Adet {b}\n" for a, b in df_donusum_kirilimi[["Adet_toplam", "Bilgi"]].values]
        
        veri = [
            [int(year), "", ""],  # 2013 başlığı
            ["Talep Artışı ( Sanayi ve 3. Şahıslar Hariç) (kVA)", talep_artisi, ""],
            ["Mevcut Talep ( Sanayi ve 3. Şahıslar Hariç) (kVA)", mevcut_talep_artisi, ""],
            [f"{toplam_trafo_mevcut} Adet Mevcut Trafo Kurulu Gücü  ( Sanayi ve 3. Şahıslar Hariç) (kVA)", toplam_kapasite_mevcut, ""],
            ["Kurulu Güç Artışı ( Sanayi ve 3. Şahıslar Hariç) (kVA)", kurulu_guc_artisi, ""],
            [10*" " +f"{int(yeni_trafo_tesis_sayisi)} Adet Yeni Trafonun Kurulu Gücü (kVA)", yeni_trafo_kurulu_gücü, ""],
            [10*" " +f"{int(yaştan_yenileme_trafo_sayisi)} Adet Yaştan Dolayı Trafo Yenileme Kurulu Gücü (kVA)", yaştan_yenileme_toplam_kapasite, ""],
            [10*" " +f"{int(güç_Yükseltme_toplam_trafo)} Adet Kurulu Güç Yükseltilmesinin Ek Kapasitesi (kVA)", güç_Yükseltme_toplam_kapasite, ""],
            [10*" " +f"{int(toplam_trafo_sayisi_donusum)} Adet Trafonun OG Seviyesi Dönüşümü", toplam_kapasite_donusum, ""]
            ]
        
        veri.extend([[20*" "+ satir, "", ""] for satir in veri_kirilimi])
            
        veri.extend([
            [10*" " +f"{int(deplase_trafo_sayisi)} Adet Trafo Deplase", deplase_toplam_kapasite, ""],
            [""],
            [f"{toplam_trafo_mevcut_ozel} Adet Mevcut Trafo Kurulu Gücü  ( Sanayi ve 3. Şahıslar) (kVA)", toplam_kapasite_mevcut_ozel, ""],
            ["Kurulu Güç Artışı (Sanayi ve 3. Şahıslar) (kVA)", ÜçüncüŞahıs_kurulu_guc_artisi, ""],
            [10*" " +f"{int(ÜçüncüŞahıs_yeni_trafo_sayi)} Yeni Trafonun Kurulu Gücü (kVA)", ÜçüncüŞahıs_yeni_trafo_kapasite, ""],
            [10*" " +f"{int(ÜçüncüŞahıs_yaştan_yenileme_sayi)} Adet Yaştan Dolayı Trafo Yenileme Kurulu Gücü (kVA)", ÜçüncüŞahıs_yaştan_yenileme, ""],
            [10*" " +f"{int(ÜçüncüŞahıs_kapasite_yenileme_sayi)} Kurulu Güç Yükseltilmesinin Ek Kapasitesi (kVA)", ÜçüncüŞahıs_kapasite_yenileme, ""],
            
            [""],
            [f"{int(EA_yeni_trafo_sayisi)} Adet EA için Yeni Trafoların Kurulu Gücü (kVA)", EA_yeni_trafo_kapasite, ""],
            [""],
            [f"Atanamayan Talep/%1 Altındaki Yeni DTR'ler", f"{Atanamayan_Talep} / {Atanamayan_DTR}"""],
            [""],
            ["Başlangıç Ortalama Kurum DTR Yüklenmesi ( Sanayi ve 3. Şahıslar Hariç) (%)", baslangic_ortalama*100, ""],
            ["Son Ortalama Kurum DTR Yüklenmesi ( Sanayi ve 3. Şahıslar Hariç) (%)", son_ortalama*100, ""],
            ["", "", ""],  # boşluk bırakıyoruz 
            ["", "", ""],  # boşluk bırakıyoruz
        ])
        
        # veri = [
        #     [int(year), "", ""],  # 2013 başlığı
        #     ["Talep Artışı ( Sanayi ve 3. Şahıslar Hariç) (kVA)", talep_artisi, ""],
        #     ["Mevcut Talep ( Sanayi ve 3. Şahıslar Hariç) (kVA)", mevcut_talep_artisi, ""],
        #     [f"{toplam_trafo_mevcut} Adet Mevcut Trafo Kurulu Gücü  ( Sanayi ve 3. Şahıslar Hariç) (kVA)", toplam_kapasite_mevcut, ""],
        #     ["Kurulu Güç Artışı ( Sanayi ve 3. Şahıslar Hariç) (kVA)", kurulu_guc_artisi, ""],
        #     [10*" " +f"{int(yeni_trafo_tesis_sayisi)}  Yeni Trafonun Kurulu Gücü (kVA)", yeni_trafo_kurulu_gücü, ""],
        #     [10*" " +f"{int(yaştan_yenileme_trafo_sayisi)} Adet Yaştan Dolayı Trafo Yenileme Kurulu Gücü (kVA)", yaştan_yenileme_toplam_kapasite, ""],
        #     [10*" " +f"{int(güç_Yükseltme_toplam_trafo)} Kurulu Güç Yükseltilmesinin Ek Kapasitesi (kVA)", güç_Yükseltme_toplam_kapasite, ""],
        #     [10*" " +f"{int(toplam_trafo_sayisi_donusum)} Adet Trafonun OG Seviyesi Dönüşümü", toplam_kapasite_donusum, ""],
            
        #     [10*" " +f"{int(deplase_trafo_sayisi)} Adet Trafo Deplase", deplase_toplam_kapasite, ""],
        #     [""],
        #     [f"{toplam_trafo_mevcut_ozel} Adet Mevcut Trafo Kurulu Gücü  ( Sanayi ve 3. Şahıslar) (kVA)", toplam_kapasite_mevcut_ozel, ""],
        #     ["Kurulu Güç Artışı (Sanayi ve 3. Şahıslar) (kVA)", ÜçüncüŞahıs_kurulu_guc_artisi, ""],
        #     [10*" " +f"{int(ÜçüncüŞahıs_yeni_trafo_sayi)} Yeni Trafonun Kurulu Gücü (kVA)", ÜçüncüŞahıs_yeni_trafo_kapasite, ""],
        #     [10*" " +f"{int(ÜçüncüŞahıs_yaştan_yenileme_sayi)} Adet Yaştan Dolayı Trafo Yenileme Kurulu Gücü (kVA)", ÜçüncüŞahıs_yaştan_yenileme, ""],
        #     [10*" " +f"{int(ÜçüncüŞahıs_kapasite_yenileme_sayi)} Kurulu Güç Yükseltilmesinin Ek Kapasitesi (kVA)", ÜçüncüŞahıs_kapasite_yenileme, ""],
            
        #     [""],
        #     [f"{int(EA_yeni_trafo_sayisi)} Adet EA için Yeni Trafoların Kurulu Gücü (kVA)", EA_yeni_trafo_kapasite, ""],
        #     [""],
        #     [f"Atanamayan Talep/%1 Altındaki Yeni DTR'ler", f"{Atanamayan_Talep} / {Atanamayan_DTR}"""],
        #     [""],
        #     ["Başlangıç Ortalama Kurum DTR Yüklenmesi ( Sanayi ve 3. Şahıslar Hariç) (%)", baslangic_ortalama*100, ""],
        #     ["Son Ortalama Kurum DTR Yüklenmesi ( Sanayi ve 3. Şahıslar Hariç) (%)", son_ortalama*100, ""],
        #     ["", "", ""],  # boşluk bırakıyoruz 
        #     ["", "", ""],  # boşluk bırakıyoruz
        # ]
        
        tum_veriler.extend(veri)

        
    df = pd.DataFrame(tum_veriler, columns=["Kalem", "Değer", "Boş"])
    
    dosya_adi = "SLF Sonuc Ozetleri"
    
    Arsivleme(dosya_adi)
    
    excelKaydet(df, f"{odtr['ilce']} SLF Sonuc Ozetleri")

def Cikti3(df, odtr):
    
    df3 = df.copy()
    
    df3 = df3.rename(columns={"trafo_id":"KOD", "year": "YIL", "Gerilim": "GERİLİM", "kapasite": "KAPASİTE", "GelenYukToplam":"YÜK (kVA)"})

    columns=["KOD", "GERİLİM", "YIL", "KAPASİTE", "YÜK (kVA)"]
    
    df3 = df3[columns]

    # Pivot işleminde sıralamayı değiştireceğiz
    df_pivot = df3.pivot(index=["KOD"], columns="YIL")[["KAPASİTE", "YÜK (kVA)"]]
    
    df3.groupby(["KOD", "YIL"]).size().reset_index(name="adet").query("adet > 1")

    
    # Sütun sırasını ters çevir: (YIL üstte, ALTINDA KAPASİTE - YÜK)
    df_pivot.columns = pd.MultiIndex.from_tuples([
        (f"YIL={yıl}", alan.upper()) for alan, yıl in df_pivot.columns
    ])

    # Kolonları yıl birinci, alan ikinci olacak şekilde grupla
    df_pivot = df_pivot.sort_index(axis=1, level=0)

    # Reset index
    df_pivot = df_pivot.reset_index()
    
    dosya_adi = "Tum DTR Yuklenmeleri"
    
    Arsivleme(dosya_adi)
    
    excelKaydet(df_pivot, f"{odtr['ilce']} Tum DTR Yuklenmeleri")

def Cikti4(df):

    df = df[(df["Trafo Aksiyon"] == "yeni trafo tesis") | (df["Trafo Aksiyon"] == "yeni trafo tesis EA")] 
    
    #df = KoordinatHesapla(df)
    columns = ["trafo_id", "year", "Gerilim", "kapasite", "Koord_x", "Koord_y", "SH Gerilim"]
    
    df = df[columns]
    
    df["DÖNÜŞÜM VAR MI"] = (
        df["Gerilim"] != df["SH Gerilim"]
    ) & df["SH Gerilim"].notna()  # sadece karşılaştırma yap anlamlıysa

    df["DÖNÜŞÜM VAR MI"] = df["DÖNÜŞÜM VAR MI"].map({True: "VAR", False: "YOK"})
    
    df = df.rename(columns={"trafo_id":"KOD", "gerilim":"GERİLİM", "kapasite": "KAPASİTE", "year":"YIL"})
    
    df = df.drop(["SH Gerilim"], axis=1)
    
    dosya_adi = "Yeni Eklenen Trafolar"
    
    Arsivleme(dosya_adi)
    
    excelKaydet(df, f"{odtr['ilce']} Yeni Eklenen Trafolar")

def Cikti5(df):
    
    df = df[(df["Trafo Aksiyon"] == "trafo yükseltme-kapasiteden") | (df["Trafo Aksiyon"] == "trafo yükseltme-yükten")] 

    columns = ["trafo_id", "İşlem Tarihi", "Eski Gerilim", "Gerilim", "kapasite", "eski kapasite", "Koord_x", "Koord_y"]
    
    df = df[columns]
    
    df["DÖNÜŞÜM VAR MI"] = df["Eski Gerilim"] != df["Gerilim"]
    df["DÖNÜŞÜM VAR MI"] = df["DÖNÜŞÜM VAR MI"].map({True: "VAR", False: "YOK"})
    
    df = df.rename(columns={"trafo_id":"KOD", "Eski Gerilim":"ESKİ GERİLİM", "Gerilim":"GERİLİM", "eski kapasite":"ÖNCEKİ KAPASİTE", "kapasite": "SONRAKİ KAPASİTE", "İşlem Tarihi":"YIL"})
    
    dosya = "Kapasitesi Artan Trafolar"
    
    Arsivleme(dosya)
    
    excelKaydet(df, f"{odtr['ilce']} Kapasitesi Artan Trafolar")


def Arsivleme(dosya):
    
    odtr = config3.get()
    # Kaynak klasör ve hedef klasör yolları
    source_folder = (odtr["sonuc_yolu"])  # Buraya kaynak klasör yolunu girin
    destination_folder = (odtr["arsiv"])  # Buraya hedef klasör yolunu girin
    
    try:
        
        # Hedef klasör yoksa oluşturuyoruz
        if not os.path.exists(destination_folder):
            os.makedirs(destination_folder)
        
        # Kaynak klasördeki dosyaları listele
        for filename in os.listdir(source_folder):
            # Eğer dosya ismi 'bir' kelimesini içeriyorsa
            if dosya in filename:
                # Dosyanın tam yolunu oluştur                                
                
                # 2. Yeni dosya adını üret
                # yeni_dosya_adi = filename.replace(silinecek_kisim, "", 1)  # sadece ilk geçen ifadeyi sil
                
                source_path = os.path.join(source_folder, filename)
                # source_destination_path = os.path.join(source_folder, yeni_dosya_adi)
                
                # # Dosyayı yeniden adlandır (aynı klasörde)
                # os.rename(source_path, source_destination_path)
                
                destination_path = os.path.join(destination_folder, filename)                           
   
                # Dosyayı taşımak
                shutil.move(source_path, destination_path)                              
                
                logging.info(F"{filename} TASINDI...")
                
    except Exception as e:
        # Hata mesajını logluyoruz
        logging.error(e)

def BosOzet(ad):
    
    years = range(odtr["ilk_yil"]-1, odtr["son_yil"] + 1)
    
    dfOzet = pd.DataFrame({
        "year": years,
        f"{ad} (Adet)": len(years)*[0]
        })
    
    return dfOzet

def PointLoadKismii(dftrafo, dftrafoYıllık, p):
    
    dftrafoOzel, dftrafoYıllıkOzel = OzelTrafoIslem2()
    
    dftrafo = pd.concat([dftrafo, dftrafoOzel], ignore_index=True).reset_index(drop=True)
    
    dftrafoYıllık = pd.concat([dftrafoYıllık, dftrafoYıllıkOzel], ignore_index=True).reset_index(drop=True)
    
    dfsuperhucre = ReadParquet(odtr["dfsuperhucre"])
    
    
    path = os.path.join(odtr["python_dosya_yolu"], f"TSYuk{odtr['ilce']}.parquet")
    
    dfSanayi, dfTicarethane, dfTarimsal = None, None, None
    
    dfsanYıllık, dftsYıllık, dfticYıllık = None, None, None
    
    if os.path.exists(path):
        data = ReadParquet(path)
    
    if not data.empty:
        
        # dftrafo = ka.trafo.copy()
        # dftrafoYıllık = ka.trafoYıllık.copy()
        
        test = PointLoadKismi2(data,"tarimsal")
        
        dftrafoYıllık, dftrafo, dfatama_sonuc = Paralelİslem_Batch(test.data2, dftrafoYıllık, dftrafo, dfsuperhucre , p)
         
        # e(dfatama_sonuc, "TarımsalSulamaAtamaSonuç1_")
        
        dfatama_sonuc = dfatama_sonuc.rename(columns={"kumulatif_yuk": "power_distribution"})
        dfatama_sonuc["shid"] = dfatama_sonuc["shid"].astype(int)
        # test.YukAtamaTarimsal(test.data2, dftrafo, dftrafoYıllık, "Tarımsal")
        
        atanamayanlar = dfatama_sonuc[dfatama_sonuc["durum"]=="Atanmadı"]
        
        dfTarimsal = pd.DataFrame()
        
        if not atanamayanlar.empty:
            dfTarimsal, dftsYıllık = test.TrafoEkleTarimsalSulama(atanamayanlar)
        
        
        if not dfTarimsal.empty:
            dftsYıllık, dfTarimsal, dfatama_sonuc2 = Paralelİslem_Batch(atanamayanlar, dftsYıllık, dfTarimsal, dfsuperhucre , p, False)
            # e(dfatama_sonuc2, "TarımsalSulamaAtamaSonuç2_")

        # test.YukAtamaTarimsal(test.data2, dfTarimsal, dftsYıllık, "Tarımsal", False)
        
            dfOzetTS = test.TrafoOzet(dfTarimsal)
        
            atanamayanlar[~atanamayanlar["shid"].isin(dfatama_sonuc2["shid"])]
        
        else:
            dfOzetTS = BosOzet("Tarımsal Sulama")
        
    else:
        
        dfOzetTS = BosOzet("Tarımsal Sulama")
    
    path = os.path.join(odtr["python_dosya_yolu"], f"BTYuk{odtr['ilce']}.parquet")
    
    if os.path.exists(path):
        data = ReadParquet(path)
        
    else:
        data = None
    
    
    if data is not None:
        test = PointLoadKismi2(data, "ticarethane")
        
        dfTicarethane, dfticYıllık = test.TrafoEkle(TD="Ticarethane")
        
        test.YukAtama(test.data2, dfTicarethane, dfticYıllık, "Ticarethane")
        
        dfOzetT = test.TrafoOzet(dfTicarethane)
        
    else:
        
        dfOzetT = BosOzet("Ticarethane")
    
    path = os.path.join(odtr["python_dosya_yolu"], f"BSYuk{odtr['ilce']}.parquet")
    
    if os.path.exists(path):
        data = ReadParquet(path)
    
    else:
        data = None
    
    if data is not None:
        
        test = PointLoadKismi2(data, "sanayi")
        
        dfSanayi, dfsanYıllık = test.TrafoEkle(TD="Sanayi")
        
        test.YukAtama(test.data2, dfSanayi, dfsanYıllık, "Sanayi")
        
        dfOzetS = test.TrafoOzet(dfSanayi)
    
    else:
       
        dfOzetS = BosOzet("Sanayi")
        
        
    dfOzet = pd.merge(dfOzetS, dfOzetT, on="year", how="outer")
    
    dfOzet = pd.merge(dfOzet, dfOzetTS, on="year", how="outer")
    
    dfOzelToplam = pd.concat([dfSanayi, dfTicarethane, dfTarimsal], ignore_index=True)

    # dftrafoOzel, dftrafoYıllıkOzel = OzelTrafoIslem2()
    
    # dfToplam = pd.concat([dftrafo, dfOzelToplam], ignore_index=True)
    
    # dftrafoYıllık = pd.concat([dftrafoYıllık, dfsanYıllık, dftsYıllık, dfticYıllık], ignore_index=True)
    
    BütünTrafolar = pd.concat([dftrafo, dfOzelToplam], ignore_index=True).reset_index(drop=True)
    
    BütünYıllıkTrafolar = pd.concat([dftrafoYıllık, dfsanYıllık, dftsYıllık, dfticYıllık], ignore_index=True).reset_index(drop=True)

    return BütünTrafolar, BütünYıllıkTrafolar, dfOzet
    
                
def İslemDoluluk(odtr, dfYıllık, df, atanamayan_talep, atanamayan_talep_kırsal): 
    
    atanamayan_talep = atanamayan_talep.copy()
    
    mask = (
    (dfYıllık["year"] == odtr["son_yil"]) &
    (dfYıllık["Doluluk Oranı"] < 0.01) &
    (dfYıllık["trafo_id"].str.contains("yenitrafo", case=False, na=False))
    )

    # Az dolu trafoları al
    az_dolu_df = dfYıllık[mask][["trafo_id", "GelenYukToplam"]]
        
    ilk_yillar = dfYıllık[dfYıllık["trafo_id"].isin(az_dolu_df["trafo_id"])] \
    .groupby("trafo_id")["year"].min().reset_index()

    az_dolu_with_yil = az_dolu_df.merge(ilk_yillar, on="trafo_id")  # year burada ilk yıl
    
    # Grupla: yıl bazında toplam yük ve trafo adedi
    yil_bazli_ozet = az_dolu_with_yil.groupby("year").agg(
        Atanamayan_Yuk=("GelenYukToplam", "sum"),
        Trafo_Sayisi=("trafo_id", "nunique")
    ).reset_index()
    
    yil_bazli_ozet = yil_bazli_ozet.rename(columns={"Atanamayan_Yuk": "Atanamayan Talep",
                                            "Trafo_Sayisi": "%1 Altındaki Trafo Sayısı"})
    
    # Tüm yıl aralığını oluştur
    y = range(odtr["ilk_yil"], odtr["son_yil"] + 1)
    years = pd.DataFrame({"year": y})
    
    # Merge et
    years = years.merge(yil_bazli_ozet, on="year", how="left")
    years[["Atanamayan Talep", "%1 Altındaki Trafo Sayısı"]] = years[["Atanamayan Talep", "%1 Altındaki Trafo Sayısı"]].fillna(0)
    # years["Atanamayan Talep"] = years["Atanamayan Talep"] * (-1)
    
    atanamayan_talep["Atanamayan Talep"] = atanamayan_talep["Atanamayan Talep"].apply(lambda x: abs(x) if x < 0 else x)
    atanamayan_talep.rename(columns={"Atanamayan Talep": "Atanamayan Talep Ek"}, inplace=True)
    
    atanamayan_talep_kırsal["Atanamayan Talep"] = atanamayan_talep_kırsal["Atanamayan Talep"].apply(lambda x: abs(x) if x < 0 else x)
    atanamayan_talep_kırsal.rename(columns={"Atanamayan Talep": "Atanamayan Talep Ek Kırsal"}, inplace=True)

    years["Atanamayan Talep"] = years["Atanamayan Talep"].apply(lambda x: abs(x) if x < 0 else x)
    
    years = years.merge(atanamayan_talep, on="year", how="left")
    
    years = years.merge(atanamayan_talep_kırsal, on="year", how="left")

    years["Atanamayan Talep"] = (years["Atanamayan Talep"].fillna(0) + \
                                 years["Atanamayan Talep Ek"].fillna(0) +\
                                 years["Atanamayan Talep Ek Kırsal"].fillna(0)                                 
                                 ) 
                                 

    years = years.rename(columns={"Atanamayan Talep": "Atanamayan Talep (KVA)",
                                  "%1 Altındaki Trafo Sayısı": "%1 Altındaki Trafo Sayısı (Adet)"
                                  })
    
    years = years.drop(["Atanamayan Talep Ek"], axis=1)
    
    years = years.drop(["Atanamayan Talep Ek Kırsal"], axis=1)
    
    dfYıllık = dfYıllık[~dfYıllık["trafo_id"].isin(az_dolu_df["trafo_id"])]
    
    df = df[~df["trafo_id"].isin(az_dolu_df["trafo_id"])]
    
    return years, dfYıllık, df

def ÖzetDüzenleme(df):
    
    if df.empty:
        return df

    odtr = config3.get()
    
    df.loc[odtr["ilk_yil"]-1] = [0] * len(df.columns)
    
    # 2024 yılındaki satırı 0. indekse taşıyalım
    df = df.reset_index()  # İlk olarak mevcut indeksleri sıfırlayalım
    
    # 2024 yılına ait satırı alıp indeksini 0 yapalım
    df = df.set_index('İşlem Tarihi')
    
    # 2024 yılını 0. indekse taşıyalım
    df = df.sort_index()  # Yıllara göre sıralama yapalım
    
    return df


def BütünÖzetGüncelle(BütünTrafolar, BütünÖzet):
    def grupla_ve_düzenle(df, grup_kolon, say_kolon, sütun_etiketi=" (Adet)"):
        """
        Verilen kolonlara göre gruplayıp sayar ve ÖzetDüzenleme uygular.
        """
        gruplanmış = df.groupby(grup_kolon)[say_kolon].value_counts().unstack(fill_value=0)
        gruplanmış.columns = gruplanmış.columns + sütun_etiketi
        return ÖzetDüzenleme(gruplanmış)

    # 1. Kurum mülkiyetine sahip trafolar
    trafolar_Kurum = grupla_ve_düzenle(
        BütünTrafolar[BütünTrafolar["Trafo Mülkiyeti"] == "Kurum"],
        "İşlem Tarihi", "Trafo Aksiyon"
    )

    # 2. Yeni trafo tesis EA trafoları
    trafolar_EA = grupla_ve_düzenle(
        BütünTrafolar[BütünTrafolar["Trafo Aksiyon"] == "yeni trafo tesis EA"],
        "İşlem Tarihi", "Trafo Aksiyon"
    )

    # 3. Trafo müşterisi tanımlı trafolar (Sanayi, Ticarethane, Tarımsal)
    musteri_mask = BütünTrafolar["Trafo Müşterisi"].notna() & (BütünTrafolar["Trafo Müşterisi"] != "")
    trafolar_PL = BütünTrafolar[musteri_mask].groupby("İşlem Tarihi")["Trafo Müşterisi"].value_counts().unstack(fill_value=0)
    trafolar_PL.columns = trafolar_PL.columns + " (Adet)"
    trafolar_PL = ÖzetDüzenleme(trafolar_PL)

    # 3.a Eksik müşteri türleri için boş özetler ekle
    Trafo_musterileri = ["Sanayi", "Ticarethane", "Tarımsal"]
    eksik_olanlar = [m for m in Trafo_musterileri if f"{m} (Adet)" not in trafolar_PL.columns]
    if eksik_olanlar:
        bos_df_list = [BosOzet(musteri) for musteri in eksik_olanlar]
        for i in range(1, len(bos_df_list)):
            bos_df_list[i] = bos_df_list[i].drop(columns=["year"], errors="ignore")
        eksik_df = pd.concat(bos_df_list, axis=1).set_index("year")
    else:
        eksik_df = pd.DataFrame()

    # 4. Diğer özet veriler
    diger_ozetler = [
        "Atanamayan Talep (KVA)",
        "%1 Altındaki Trafo Sayısı (Adet)",
        "Toplam Eklenen Kapasite (KVA)"
    ]
    diger_df_list = [BütünÖzet[["year", kol]].set_index("year") for kol in diger_ozetler]

    # 5. Hepsini birleştir
    birleşik_df = pd.concat(
        [trafolar_Kurum, trafolar_EA, trafolar_PL] + diger_df_list + ([eksik_df] if not eksik_df.empty else []),
        axis=1
    )

    return birleşik_df.fillna(0)


if __name__ == "__main__":
    
        start = time.time() 
        
        # input_file = r"C:\Users\vural.bayrakli\OneDrive - MRC\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\il_ilce_kırılımları\Program Dosyaları\Optimal DTR\ODTR.json"
        input_file = sys.argv[1]
        
        logging.info(f"OPTIMAL TRAFO ALGORITMASI BASLIYOR...")   
        
        odtr = config3.ODTR(input_file)
        
        katsayilar_path = os.path.join(odtr["dosyalar"] , "katsayılar.xlsx")
        df = pd.read_excel(katsayilar_path, engine="openpyxl")  # Daha hızlı olması için openpyxl kullanılır
        df = df.iloc[:,[0,3]]
        p = config3.Start(df)
        
        # VeriOtomasyon(odtr["yuk_path"], True)
        
        start = time.time()
            
        logging.info(f"KURUMSAL TRAFO KISMI(1/4) YAPILIYOR...")
    
        test = KurumTrafoKismi(odtr, p)
        end = time.time()
        
        logging.info(f"TOPLAM SURE: {end - start:.2f} saniye")
    
        logging.info(f"KURUMSAL TRAFO KISMI BITTI...")
    
        logging.info(f"KIRSAL ALAN KISMI(2/4) YAPILIYOR...")
    
        ka = KirsalAlanKismi(test.trafo.copy(), test.trafoYıllık.copy(), p, odtr)
         
        logging.info(f"KIRSAL ALAN KISMI BITTI...")
        
        logging.info(f"POINT LOAD KISMI YAPILIYOR...")
    
        BütünTrafolar, BütünYıllıkTrafolar, dfOzet = PointLoadKismii(ka.trafo.copy(), ka.trafoYıllık.copy(), p)
            
        logging.info(f"POINT LOAD KISMI(3/4) BITTI...")
        BütünÖzet = test.TrafoOzet.merge(dfOzet, on="year", how="left")
        
        # BütünTrafolar = pd.concat([ka.trafo, dfToplam], ignore_index=True)
        
        # BütünYıllıkTrafolar = pd.concat([ka.trafoYıllık, dfYıllık], ignore_index=True)
        logging.info(f"EA KISMI(4/4) YAPILIYOR...")
    
        ea = EAKismi(odtr, p, BütünTrafolar, BütünYıllıkTrafolar)
        
        logging.info(f"EA KISMI BITTI...")
        
        BütünÖzet = BütünÖzet.merge(ea.TrafoOzet, on="year", how="left")
        
        BütünTrafolar = ea.trafo.copy()
        
        BütünYıllıkTrafolar = ea.trafoYıllık.copy()
        
        # BütünTrafolar = pd.concat([ea.trafo, ea.dfyeni], ignore_index=True)
        
        # BütünYıllıkTrafolar = pd.concat([ea.trafoYıllık, ea.dfYıllık],  ignore_index=True)
        
        kalanYüklerÖzet, BütünYıllıkTrafolar,BütünTrafolar = İslemDoluluk(odtr, BütünYıllıkTrafolar, BütünTrafolar, test.atanamayan_talepler.copy(), ka.atanamayan_talepler.copy())
        
        BütünÖzet = BütünÖzet.merge(kalanYüklerÖzet, on="year", how="left").fillna(0)
    
        BütünTrafolar = KoordinatHesapla(BütünTrafolar)
            
        # trafo_id'ye göre eşleştirerek yeni sütun oluştur
        BütünYıllıkTrafolar["Koord_x"] = BütünYıllıkTrafolar["trafo_id"].map(
            BütünTrafolar.set_index("trafo_id")["Koord_x"]
        )
        
        # trafo_id'ye göre eşleştirerek yeni sütun oluştur
        BütünYıllıkTrafolar["Koord_y"] = BütünYıllıkTrafolar["trafo_id"].map(
            BütünTrafolar.set_index("trafo_id")["Koord_y"]
        )
        
        dfToplam = BütünTrafolar[(BütünTrafolar["Trafo Mülkiyeti"] == "Özel")].copy()
        
        Cikti1(BütünTrafolar, BütünYıllıkTrafolar, odtr, BütünÖzet, p, dfToplam)
        
        Cikti3(BütünYıllıkTrafolar, odtr)
        
        Cikti4(BütünTrafolar)
        
        Cikti5(BütünTrafolar)
        
        dosya_adi = "Optimal Trafo Sonuçlar"
        
        Arsivleme(dosya_adi) 
        
        BütünTrafolar.set_index("trafo_id", inplace=True)
        BütünTrafolar.drop("index", axis=1, inplace=True)
        BütünTrafolar.rename(columns={"year":"yıl"}, inplace=True)
        excelKaydet(BütünTrafolar, "Optimal Trafo Sonuçlar")
        
        
        dosya_adi_yillik = "Optimal Trafo Yıllık Sonuçlar"
        
        Arsivleme(dosya_adi_yillik) 
        
        BütünYıllıkTrafolar.set_index("trafo_id", inplace=True)
        BütünYıllıkTrafolar.drop("index", axis=1, inplace=True)
        BütünYıllıkTrafolar.rename(columns={"year":"yıl"}, inplace=True)

        excelKaydet(BütünYıllıkTrafolar, dosya_adi_yillik)
        
        dosya_adi = "Trafo Özet Sonuçlar"
        
        Arsivleme(dosya_adi) 
        
        BütünÖzet = BütünÖzetGüncelle(BütünTrafolar, BütünÖzet)
        
        excelKaydet(BütünÖzet, "Trafo Özet Sonuçlar")
        
        logging.info("RAPOR DOSYALARI OLUSTURULDU...")
        logging.info("BUTUN ISLEMLER BITTI...")
        
        # BütünÖzelTrafolar = BütünTrafolar[(BütünTrafolar["Trafo Mülkiyeti"] == "Özel") & (BütünTrafolar["Trafo Aksiyon"] != "yeni trafo tesis EA")].copy()
        # PydeckKismi(BütünÖzelTrafolar, odtr["sonuc_yolu"])
        
        end = time.time() 
        
        print(f"{end - start:.2f} saniye")
        
    