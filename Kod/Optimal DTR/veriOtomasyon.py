# -*- coding: utf-8 -*-
"""
Created on Fri Apr 11 18:55:11 2025

@author: vural.bayrakli
"""

# Birinci veri seti


import pandas as pd
import numpy as np
import config3
from Functions3 import *
from datetime import datetime
import pickle
import os
    
    
def VeriOtomasyon(path, orijinalVeri=False):
    
    odtr = config3.get()
    
    path = odtr["yuk_path"]
    
    if orijinalVeri:
        
        dflist = []
        
        dfListEA = []
        
        years = range(odtr["ilk_yil"]-1, odtr["son_yil"]+1)
        
        for y in years:
            
            # df2 = pd.read_excel(path, sheet_name=f"{y}") 
            # df2["year"] = y
            # dflist.append(df2)
            
            try:
                df3 = pd.read_excel(odtr["EAVeriYolu"], sheet_name=f"{y}") 
                
                df3["year"] = y
                
                dfListEA.append(df3)
                
            except Exception as e:
                print(e)
        
        # Pickle dosyasına yaz
        with open("dflist.pkl", "wb") as f:
            pickle.dump(dflist, f)
            
        # Pickle dosyasına yaz
        with open("dflistEA.pkl", "wb") as f:
            pickle.dump(dfListEA, f)
    
    dflistpath = os.path.join(odtr["dosyalar"],f"dflist_{odtr['ilce']}.pkl")
    
    if (not orijinalVeri) and os.path.exists(dflistpath):
        
        with open(dflistpath, "rb") as f:
            dflist = pickle.load(f) 
    
    df = pd.concat([d for d in dflist], ignore_index=True)
    SqliteKaydet(df, odtr["yuk_db_adi"])
    ToParquet(df, odtr['yuk_path_parquet'])
    
    try:
        
        dfEApath = os.path.join(odtr["dosyalar"],"dflistEA_{odtr['ilce']}.pkl")
        
        if (not orijinalVeri) and os.path.exists(dfEApath):
            
            with open(dfEApath, "rb") as f:
                dflistEAa = pickle.load(f) 
                
        df2 = pd.concat([d for d in dfListEA], ignore_index=True)
        SqliteKaydet(df2, odtr["EAVeri_db_adi"])
        ToParquet(df, odtr['EA_yuk_path_parquet'])

        
    except Exception as e:
        
        print(e)
        
               
    SqliteKaydet2(odtr["yuk_db_yolu"], dfK, "karşıyaka")
    SqliteKaydet2(odtr["yuk_db_yolu"], dfC, "çiğli")
    SqliteKaydet2(odtr["yuk_db_yolu"], df, "tepebaşı")
    
    SqliteKaydet2(dfC, "çiğli")

    dfC = df[df["ilce"]==1]
    dfK = df[df["ilce"]==2]
    
    SqliteKaydet2(odtr["EAVeritabani"], dfCEA, "çiğli")
    SqliteKaydet2(odtr["EAVeritabani"], df2, "tepebaşı")
    dfCEA = df2[df2["ilce"]==1]
    dfKEA = df2[df2["ilce"]==2]
    
    
    
    










