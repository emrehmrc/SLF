# -*- coding: utf-8 -*-
"""
Created on Wed Feb 19 12:11:49 2025

@author: vural.bayrakli
"""

import pydeck as pdk
import pandas as pd
import numpy as np
import sys
import logging
import os
import config3
from Functions3 import ReadParquet

def PydeckKismi(Trafolar, output_path):
        
    odtr = config3.get()
    
    dfsuperhucre = ReadParquet(odtr["dfsuperhucre"])
    
    dfsuperhucre["coordinates"] = dfsuperhucre.apply(lambda row: [
        [row["left"], row["bottom"]],
        [row["left"], row["top"]],
        [row["right"], row["top"]],
        [row["right"], row["bottom"]],
        [row["left"], row["bottom"]]  # Poligonu kapatmak için tekrar başlangıç noktasına dön
    ], axis=1)
    
    def get_trafo_color(action):
        if action == "yeni trafo tesis":
            return [0, 255, 0, 255]  # Yeşil
        elif action == "mevcut":
            return [128, 128, 128, 255]  # Gri
        elif action == "trafo yükseltme-yükten":
            return [255, 165, 0, 255]  # Turuncu
        elif action == "trafo yükseltme-kapasiteden":
            return [255, 255, 0, 255]  # Sarı
        elif action == "trafo yenileme-yaştan":
            return [0, 128, 255, 255]  # Açık Mavi
        elif action == "gerilim dönüşümü":
            return [138, 43, 226, 255]  # Mor (BlueViolet)
        elif action == "deplase":
            return [255, 105, 180, 255]  # Pembe (HotPink)
        elif action == "güç artırımı":
            return [255, 69, 0, 255]  # Kırmızı-turuncu (OrangeRed)
        elif action == "projelendirilmiş yeni trafo":
            return [0, 206, 209, 255]  # Turkuaz (DarkTurquoise)
        else:
            return [100, 100, 100, 255]  # Bilinmeyen için koyu gri



    # Trafo verilerini tekrar oluşturma
    trafo_data = []
    for _, row in Trafolar.iterrows():
            
        try:
            trafo_data.append({
                "Trafo ID": row['trafo_id'],
                "SHID": row['shid'],
                "x": float(row["Koord_x"].replace(",", ".")),
                "y": float(row["Koord_y"].replace(",", ".")),
                "year": row.get('year', None),
                "Trafo Aksiyon": row.get("Trafo Aksiyon", "Belirsiz"),  # Eğer yoksa "Hayır" varsayılan                               
                "Gelen Toplam Yük": row.get("GelenYukToplam", "Belirsiz"),
                "Kapasite": row.get("kapasite", "Belirsiz"),
                "İşlem Tarihi": row.get("İşlem Tarihi", "")
            })
        
        except:
            trafo_data.append({
                "Trafo ID": row['trafo_id'],
                "SHID": row['shid'],
                "İşlem Tarihi": row.get("İşlem Tarihi", ""),
                "x": float(row["Koord_x"]),
                "y": float(row["Koord_y"]),
                "year": row.get('year', None),
                "Trafo Aksiyon": row.get("Trafo Aksiyon", "Belirsiz"),  # Eğer yoksa "Hayır" varsayılan                               
                "Gelen Toplam Yük": row.get("GelenYukToplam", "Belirsiz"),
                "Kapasite": row.get("kapasite", "Belirsiz"),
            
            })
    
    
    # Trafo verilerini DataFrame'e çevirme
    df_trafo_pydeck = pd.DataFrame(trafo_data)
    
    df_trafo_pydeck["color"] = df_trafo_pydeck["Trafo Aksiyon"].apply(get_trafo_color)
    
    # **Yan yana kesişimli iki çember oluşturma**
    circle_offset = 0.0005  # Çemberlerin ayrılma mesafesi
    
    df_trafo_pydeck["x_left"] = df_trafo_pydeck["x"] - circle_offset
    df_trafo_pydeck["x_right"] = df_trafo_pydeck["x"] + circle_offset
    
    circle_offset = 0.00009  # Çemberlerin ayrılma mesafesi (daha küçük yaptım)
    circle_radius = 0.0001  # Çember yarıçapı küçültüldü
    
    df_trafo_pydeck["x_left"] = df_trafo_pydeck["x"] - circle_offset
    df_trafo_pydeck["x_right"] = df_trafo_pydeck["x"] + circle_offset

    
    def create_circle_path(center_x, center_y, radius, num_points=30):
        # Belirtilen merkez ve yarıçap ile bir çemberin koordinatlarını oluşturur
        angles = np.linspace(0, 2 * np.pi, num_points)
        path = [[center_x + radius * np.cos(a), center_y + radius * np.sin(a)] for a in angles]
        path.append(path[0])  # Çemberi kapatmak için ilk noktayı tekrar ekliyoruz
        return path
    
    # Sol çemberin yol verisini oluşturma
    df_trafo_pydeck["left_circle_path"] = df_trafo_pydeck.apply(
        lambda row: create_circle_path(row["x_left"], row["y"], circle_radius), axis=1
    )
    
    # Sağ çemberin yol verisini oluşturma
    df_trafo_pydeck["right_circle_path"] = df_trafo_pydeck.apply(
        lambda row: create_circle_path(row["x_right"], row["y"], circle_radius), axis=1
    )
    
    # Sol çember (kapasite artırma durumu için)
    left_circle_layer = pdk.Layer(
        "PathLayer",
        df_trafo_pydeck,
        get_path="left_circle_path",
        get_color="color",
        width_min_pixels=2,  # Çizgi kalınlığı
        pickable=True,  # Seçilebilir yap
        tooltip= True
    )
    
    # Sağ çember (2024 sonrası durum için)
    right_circle_layer = pdk.Layer(
        "PathLayer",
        df_trafo_pydeck,
        get_path="right_circle_path",
        get_color="color",
        width_min_pixels=2,  # Çizgi kalınlığı
        pickable=True,  # Seçilebilir yap
        tooltip=True
    )
    
    dfsuperhucre_layer = pdk.Layer(
        "PolygonLayer",
        dfsuperhucre,
        get_polygon="coordinates",
        get_fill_color=[255, 0, 0, 20],
        width_min_pixels=4,  # Çizgi kalınlığı
        pickable=True,  # Seçilebilir yap
        
    )
    
    # Harita görünüm ayarları
    view_state = pdk.ViewState(
        latitude=df_trafo_pydeck["y"].mean(),
        longitude=df_trafo_pydeck["x"].mean(),
        zoom=14,  # Daha yakından göstermek için artırıldı
        pitch=0,
    )
    
    # **Tooltip Tanımlama**
    
    # Haritayı oluştur
    deck = pdk.Deck(
        layers=[dfsuperhucre_layer, left_circle_layer, right_circle_layer],  # Yalnızca kenarlıkları olan çemberler
        initial_view_state=view_state,
        map_style="road",
       
    )
    
    
    
    # **Haritayı HTML olarak kaydetme**
    pydeck_html_path = os.path.join(output_path, odtr["pydeckRunOutput"])
    
    deck.to_html(pydeck_html_path)

    add_legend_to_html(pydeck_html_path)
    
    

def add_legend_to_html(file_path):
    legend_html = """
       <div style="
            position: absolute;
            top: 20px;
            left: 20px;
            background-color: white;
            padding: 10px;
            font-family: sans-serif;
            font-size: 12px;
            border: 1px solid #ccc;
            border-radius: 6px;
            box-shadow: 2px 2px 5px rgba(0,0,0,0.2);
            z-index: 999;
        ">
          <b>Trafo Aksiyonları</b><br>
          <div>
            <div><span style="display:inline-block;width:14px;height:14px;background:#00FF00;border:1px solid #000;margin-right:5px;"></span> Yeni Trafo Tesis</div>
            <div><span style="display:inline-block;width:14px;height:14px;background:#808080;border:1px solid #000;margin-right:5px;"></span> Mevcut</div>
            <div><span style="display:inline-block;width:14px;height:14px;background:#FFA500;border:1px solid #000;margin-right:5px;"></span> Trafo Yükseltme - Yükten</div>
            <div><span style="display:inline-block;width:14px;height:14px;background:#FFFF00;border:1px solid #000;margin-right:5px;"></span> Trafo Yükseltme - Kapasiteden</div>
            <div><span style="display:inline-block;width:14px;height:14px;background:#0080FF;border:1px solid #000;margin-right:5px;"></span> Trafo Yenileme - Yaştan</div>
            <div><span style="display:inline-block;width:14px;height:14px;background:#8A2BE2;border:1px solid #000;margin-right:5px;"></span> Gerilim Dönüşümü</div>
            <div><span style="display:inline-block;width:14px;height:14px;background:#FF69B4;border:1px solid #000;margin-right:5px;"></span> Deplase</div>
            <div><span style="display:inline-block;width:14px;height:14px;background:#FF4500;border:1px solid #000;margin-right:5px;"></span> Güç Artırımı</div>
            <div><span style="display:inline-block;width:14px;height:14px;background:#00CED1;border:1px solid #000;margin-right:5px;"></span> Projelendirilmiş Yeni Trafo</div>
          </div>
    </div>

    """

    with open(file_path, "r", encoding="utf-8") as f:
        content = f.read()

    content = content.replace("</body>", legend_html + "\n</body>")

    with open(file_path, "w", encoding="utf-8") as f:
        f.write(content)


if __name__ == "__main__":
    
    # Script dosyasının bulunduğu dizin
    script_dir = os.path.dirname(os.path.abspath(__file__))
    
    odtr_file_path = os.path.join(script_dir, "ODTR.json")
    
    odtr = config3.ODTR(odtr_file_path)
    
    input_file =  sys.argv[1]
    
    output_path = sys.argv[2]

    # input_file = r"C:\Users\vural.bayrakli\OneDrive - MRC\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı\il_ilce_kırılımları\İzmir\Karşıyaka\proje_a20\sonuclar\Optimal DTR Sonuçları\Arşiv\Sonuç.xlsx"

    logging.info(f"ISLEM BASLADI...") 
    
    trafolar = pd.read_excel(input_file, engine="openpyxl")
                
    PydeckKismi(trafolar, output_path)
    
 

    
    