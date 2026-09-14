# -*- coding: utf-8 -*-
"""
Created on Sun Apr 20 18:54:56 2025

@author: vural.bayrakli
"""


from shapely.geometry import Point, Polygon
import pandas as pd
import numpy as np
from Functions3 import ReadParquet
import config3
    
def create_polygon_from_coordinates(left, top, right, bottom):
    # Koordinatları 8 basamağa yuvarla
    l = round(left, 8)
    t = round(top, 8)
    r = round(right, 8)
    b = round(bottom, 8)
    
    return Polygon([
        (l, b),
        (l, t),
        (r, t),
        (r, b),
        (l, b)
    ])

def KoordinatHesapla(df):
    
    odtr = config3.get()
    
    dfhucre = ReadParquet(odtr["df_hucre_parquet"])
    dfhucre = dfhucre.drop_duplicates(subset="id", keep="first")
    
    # dfhucre.drop(["year"], axis=1, inplace=True)
     
    # dftrafo_hucre = df.copy()
    dftrafo_hucre = df.merge(dfhucre[['id', 'left', 'top', 'right', 'bottom']], left_on="merkez_hucre", right_on="id", how="left")
    
    dftrafo_hucre = dftrafo_hucre.dropna(subset=['left', 'top', 'right', 'bottom'])
    
    # Merkez hücreler ve koordinatları ile poligonlar oluştur
    polygons = {}
    
    for idx, row in dftrafo_hucre.iterrows():
        
        # l, r = sorted([row.left, row.right])
        # b, t = sorted([row.bottom, row.top])
        # # print(row.id, l,r,b,t)
        # polygon = create_polygon_from_coordinates(l, t, r, b)
        
        polygon = create_polygon_from_coordinates(row['left'], row['top'], row['right'], row['bottom'])
        
        # Poligonları merkez_hucre'ye göre kaydet
        polygons[row['merkez_hucre']] = polygon
        
    # Koordinatları benzersiz yapma
    unique_coords = []
    
    # Yeni koordinatları DataFrame'e ekleme
    for i, row in dftrafo_hucre.iterrows():
        # Merkezi hücrenin koordinatlarını al
        
        koord_x = row["Koord_x"]
        koord_y = row["Koord_y"]
        
        # Eğer boşsa veya NaN ise, NaN olarak kabul et
        if pd.notna(koord_x) and koord_x != '':
            continue
        
        x_center = (row['left'] + row['right']) / 2
        y_center = (row['top'] + row['bottom']) / 2
        
        # Poligon oluştur
        polygon = polygons[row['merkez_hucre']]
        
        # Koordinatın poligon içinde olup olmadığını kontrol et
        point = Point(x_center, y_center)
        
        if polygon.contains(point):
            # Koordinatların benzersiz olduğundan emin olmak için ekle
            if (x_center, y_center) not in unique_coords:
                
                unique_coords.append((x_center, y_center))
                
                dftrafo_hucre.at[i, 'Koord_x'] = x_center  # Koordinatları DataFrame'e ekle
                dftrafo_hucre.at[i, 'Koord_y'] = y_center  # Koordinatları DataFrame'e ekle
            else:
                # Eğer koordinat daha önce eklenmişse, benzersiz hale getirmek için bir değişiklik yap (epsilon ekleyerek)
                epsilon = 0.00005  # Küçük bir fark
                new_x = x_center + epsilon * np.random.rand()
                new_y = y_center + epsilon * np.random.rand()
                unique_coords.append((new_x, new_y))
                dftrafo_hucre.at[i, 'Koord_x'] = new_x  # Benzersiz koordinatları DataFrame'e ekle
                dftrafo_hucre.at[i, 'Koord_y'] = new_y  # Benzersiz koordinatları DataFrame'e ekle
        
    return dftrafo_hucre.iloc[:,:-5]
