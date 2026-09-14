#!/usr/bin/env python
# -*- coding: utf-8 -*-

"""
min_max_kirilimlari.py - İmar ID Forecast ve Bina Tipi bazında abonelik dağılım tablosu

Bu script, mesken ve diğer veri setlerini analiz ederek IMAR_ID_FORECAST ve BINA_TIPI bazında
abonelik türlerinin minimum ve maksimum değerlerini hesaplar. Boş hücreleri 0 ile doldurur.

Kullanım: python min_max_kirilimlari.py mesken_file_path other_file_path [output_path]
"""

import sys
import os
import pandas as pd
import numpy as np
from tabulate import tabulate
import warnings
warnings.filterwarnings('ignore')

def dosyayi_oku(dosya_yolu):

    """
    Belirtilen dosyayı uygun kodlama ile okumaya çalışır.
    
    Args:
        dosya_yolu (str): Okunacak CSV dosyasının yolu
        
    Returns:
        pandas.DataFrame: Okunan veri
    """
    kodlamalar = ["utf-8-sig", "cp1254", "latin1", "utf-8"]
    
    for kodlama in kodlamalar:
        try:
            veri = pd.read_csv(dosya_yolu, encoding=kodlama)
            return veri
        except Exception as e:
            continue
    
    raise Exception(f"Dosya okunamadı: {dosya_yolu}. Hiçbir kodlama çalışmadı.")


def mantiksal_kombinasyon_mi(bina_tipi, abone_tipi):

    """
    Bina tipi ve abone türü kombinasyonunun mantıksal olup olmadığını kontrol eder.
    
    Args:
        bina_tipi (str): Bina tipi
        abone_tipi (str): Abone türü
        
    Returns:
        bool: Kombinasyonun mantıksal olup olmadığı
    """

    # Tip kontrolü yap
    if not isinstance(bina_tipi, str):
        return False
    
    # NaN kontrolü
    if 'nan' in bina_tipi.lower():
        return False
    
    # MESKEN_SAYISI yerine kullanılan özel durum
    if abone_tipi == "MESKEN" and "MESKEN" in bina_tipi:
        return True  # Mesken kategorisi için MESKEN tipi binalar mantıklı
    
    # Diğer mantıksal kombinasyonları kontrol et
    if abone_tipi == "SANAYI":
        # Sanayi abone tipi genellikle sanayi binalarında olur
        return "SANAYI" in bina_tipi
    
    elif abone_tipi == "TICARETHANE":
        # Ticarethane abone tipi genellikle ticarethane binalarında olabilir
        return "TICARETHANE" in bina_tipi
    
    elif abone_tipi == "TARIMSAL_SULAMA":
        # DÜZELTME: Hem "TARIMSAL" hem de "TARIMSAL_SULAMA" kontrolü
        return "TARIMSAL_SULAMA" in bina_tipi
    
    elif abone_tipi == "AYDINLATMA":
        # Aydınlatma abone tipi genellikle aydınlatma binalarında olur
        return "AYDINLATMA" in bina_tipi
    
    # Bilinmeyen abone tipi için varsayılan olarak False dön
    return False



def pivot_tablo_olustur(mesken_df, other_df):

    """
    Veri setlerinden imar ID ve bina tipi bazında min-max pivot tablosu oluşturur.
    DÜZELTME: TARIMSAL SULAMA sütun eşleştirmesi sorunu çözüldü.
    
    Args:
        mesken_df (pandas.DataFrame): Mesken binaları veri seti
        other_df (pandas.DataFrame): Diğer binalar veri seti
        
    Returns:
        pandas.DataFrame: Oluşturulan pivot tablosu
    """
    # IMAR_ID_FORECAST değerlerini bina tiplerine eşleştir
    imar_id_map = {
        1: "1-2 KATLI MESKEN",
        2: "3-4 KATLI MESKEN",
        3: "5-7 KATLI MESKEN",
        4: "8 USTU KATLI MESKEN",
        5: "VILLA MESKEN"
    }
    
    # Abone grupları
    abone_tipleri = ["MESKEN", "SANAYI", "TICARETHANE", "TARIMSAL_SULAMA", "AYDINLATMA"]
    
    # DÜZELTME: Abone tipi - sütun adı eşleştirme tablosu
    abone_sutun_map = {
        "MESKEN": "MESKEN_count",
        "SANAYI": "SANAYI_count", 
        "TICARETHANE": "TICARETHANE_count",
        "TARIMSAL_SULAMA": "TARIMSAL_SULAMA_count",  # ← Ana düzeltme burada
        "AYDINLATMA": "AYDINLATMA_count"
    }
    
    # Mesken verilerinde IMAR_ID_FORECAST değerini bina tipine dönüştür
    if 'IMAR_ID_FORECAST' in mesken_df.columns:
        mesken_df['BINA_TIPI_DETAY'] = mesken_df['IMAR_ID_FORECAST'].map(imar_id_map)
    
    # Sonuç tablosu için kategorileri hazırla
    mesken_kategoriler = list(imar_id_map.values())
    
    # Diğer veri setinden bina tiplerini al
    other_kategoriler = []
    if 'BINA_TIPI' in other_df.columns:
        other_kategoriler = other_df['BINA_TIPI'].unique().tolist()
        # NaN değerlerini filtrele
        other_kategoriler = [x for x in other_kategoriler if isinstance(x, str)]
    
    # Birleştirilmiş kategori listesi
    tum_kategoriler = mesken_kategoriler + other_kategoriler
    
    # Sonuç tablosu için boş DataFrame
    sonuc_df = pd.DataFrame(index=abone_tipleri)
    
    # Mesken verileri için hesaplama
    for imar_id, bina_tipi in imar_id_map.items():
        imar_grubu = mesken_df[mesken_df['IMAR_ID_FORECAST'] == imar_id]
        
        # Sütun adlarını hazırla
        min_col = f"{bina_tipi} - min"
        max_col = f"{bina_tipi} - max"
        
        # Her abone tipi için değerleri hesapla
        for abone_tipi in abone_tipleri:
            # Kombinasyon mantıksal mı kontrol et
            if not mantiksal_kombinasyon_mi(bina_tipi, abone_tipi):
                sonuc_df.loc[abone_tipi, min_col] = 0
                sonuc_df.loc[abone_tipi, max_col] = 0
                continue
            
            # Doğru sütun adını al
            count_col = abone_sutun_map.get(abone_tipi)
            if not count_col:
                sonuc_df.loc[abone_tipi, min_col] = 0
                sonuc_df.loc[abone_tipi, max_col] = 0
                continue
                
            # Sütun var mı kontrol et
            if count_col in imar_grubu.columns and len(imar_grubu) > 0:
                min_val = int(imar_grubu[count_col].min())
                max_val = int(imar_grubu[count_col].max())
                
                sonuc_df.loc[abone_tipi, min_col] = min_val
                sonuc_df.loc[abone_tipi, max_col] = max_val
            else:
                sonuc_df.loc[abone_tipi, min_col] = 0
                sonuc_df.loc[abone_tipi, max_col] = 0
    
    # Diğer veriler için hesaplama
    for bina_tipi in other_kategoriler:
        # NaN kontrolü
        if not isinstance(bina_tipi, str) or 'nan' in str(bina_tipi).lower():
            continue
            
        bina_grubu = other_df[other_df['BINA_TIPI'] == bina_tipi]
        
        # Sütun adlarını hazırla
        min_col = f"{bina_tipi} - min"
        max_col = f"{bina_tipi} - max"
        
        # Her abone tipi için değerleri hesapla
        for abone_tipi in abone_tipleri:
            # Kombinasyon mantıksal mı kontrol et
            if not mantiksal_kombinasyon_mi(bina_tipi, abone_tipi):
                sonuc_df.loc[abone_tipi, min_col] = 0
                sonuc_df.loc[abone_tipi, max_col] = 0
                continue
                
            # Doğru sütun adını al
            count_col = abone_sutun_map.get(abone_tipi)
            if not count_col:
                sonuc_df.loc[abone_tipi, min_col] = 0
                sonuc_df.loc[abone_tipi, max_col] = 0
                continue
            
            
            # Sütun var mı kontrol et ve değerleri hesapla
            if count_col in bina_grubu.columns and len(bina_grubu) > 0:

                min_val = int(bina_grubu[count_col].min())
                max_val = int(bina_grubu[count_col].max())
                
                sonuc_df.loc[abone_tipi, min_col] = min_val
                sonuc_df.loc[abone_tipi, max_col] = max_val

            else:
                
                sonuc_df.loc[abone_tipi, min_col] = 0
                sonuc_df.loc[abone_tipi, max_col] = 0
    
    # NaN değerleri 0 ile doldur
    sonuc_df = sonuc_df.fillna(0)
    
    # İndeksi "Category" adlı bir sütuna dönüştür
    sonuc_df = sonuc_df.reset_index().rename(columns={"index": "Category"})
    
    # 'nan' içeren sütunları kaldır
    nan_columns = [col for col in sonuc_df.columns if 'nan' in col.lower()]
    sonuc_df = sonuc_df.drop(columns=nan_columns)
    
    return sonuc_df

def min_max_kirilim(mesken_file_path, other_file_path, output_path="min_max_table.xlsx"):

    """Ana fonksiyon - dosya yollarını işler ve veri setlerini analiz eder."""
    print("buradayım")
    try:
        # Dosyaların var olup olmadığını kontrol et
        if not os.path.exists(mesken_file_path):
            return False
        
        if not os.path.exists(other_file_path):
            return False
        
        # Dosyaları oku
        try:
            mesken_data = dosyayi_oku(mesken_file_path)
            other_data = dosyayi_oku(other_file_path)
        except Exception as e:
            return False
        
        # IMAR_ID_FORECAST sütunu kontrolü
        if 'IMAR_ID_FORECAST' not in mesken_data.columns:
            
            # Olası benzer sütun isimlerini kontrol et
            for col in mesken_data.columns:
                if 'IMAR' in col or 'FORECAST' in col:

                    mesken_data['IMAR_ID_FORECAST'] = mesken_data[col]
                    break
        
        # Pivot tabloyu oluştur
        pivot_table = pivot_tablo_olustur(mesken_data, other_data)
        
        # Sonuçları Excel'e kaydet
        try:
            # Çıktı dizininin var olduğundan emin ol
            output_dir = os.path.dirname(output_path)
            if output_dir and not os.path.exists(output_dir):
                os.makedirs(output_dir, exist_ok=True)
            
            # Çıktı yolunun Excel uzantısına sahip olduğundan emin ol
            if not output_path.lower().endswith(('.xlsx', '.xls')):
                output_path = output_path + ".xlsx"
                
            # Excel dosyasını kaydet
            pivot_table.to_excel(output_path, index=False, engine='openpyxl')
        except Exception as e:
            print(f"Excel kaydetme hatası: {e}")
            # Alternatif olarak CSV formatında kaydet
            try:
                csv_path = output_path.rsplit('.', 1)[0] + '.csv'
                pivot_table.to_csv(csv_path, index=False, encoding='utf-8-sig')
            except Exception as csv_err:
                return False
        
        print("\nİşlem başarıyla tamamlandı.")
        return True
        
    
        
    except Exception as e:
        print(f"Hata oluştu: {str(e)}")
        import traceback
        traceback.print_exc()
        sys.exit(1)


if __name__ == "__main__":
    min_max_kirilim()