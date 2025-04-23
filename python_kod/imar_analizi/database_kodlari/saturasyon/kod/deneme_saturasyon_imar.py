import pandas as pd
import chardet
import numpy as np
import os

# Dizinlerin tanımlanması
INPUT_DIR = '../girdiler'
OUTPUT_DIR = '../ciktilar'
STOKASTIK_DIR = '../../stokastik/girdiler'

# Helper fonksiyonlar
def fix_2023_builtup(row):
    value = row['2023_builtup_m2']
    if value < 0.05:
        return 0
    elif 0.05 <= value <= 0.40:
        return value
    else:
        return 1

def calculate_weighted_taks(row, mesken_df, ticaret_df, imar_id_mapping):
    total_buildings = 0
    weighted_taks_sum = 0

    if row['MESKEN SAYISI'] > 0:
        for imar_id, kolon in imar_id_mapping.items():
            if row[kolon] > 0:
                bina_sayisi = row[kolon]
                matching_rows = mesken_df.loc[mesken_df['IMAR_ID'] == imar_id]
                if not matching_rows.empty:
                    taks = matching_rows['Ortalama TAKS'].iloc[0]
                    weighted_taks_sum += bina_sayisi * taks
                    total_buildings += bina_sayisi

    for bina_tip in ['BUYUK_SANAYI', 'BUYUK_TICARETHANE', 'KUCUK_SANAYI', 
                     'KUCUK_TICARETHANE', 'ORTA_SANAYI', 'ORTA_TICARETHANE']:
        if row[bina_tip] > 0:
            bina_sayisi = row[bina_tip]
            matching_rows = ticaret_df[ticaret_df['Bina Tipi'] == bina_tip]
            if not matching_rows.empty:
                taks = matching_rows['Ortalama TAKS'].iloc[0]
                weighted_taks_sum += bina_sayisi * taks
                total_buildings += bina_sayisi
    
    return weighted_taks_sum / total_buildings if total_buildings > 0 else 0

def calculate_total_building_area(row, mesken_df, ticaret_df, imar_id_mapping):
    total_area = 0

    if row['MESKEN SAYISI'] > 0:
        for imar_id, kolon in imar_id_mapping.items():
            if row[kolon] > 0:
                bina_sayisi = row[kolon]
                matching_rows = mesken_df.loc[mesken_df['IMAR_ID'] == imar_id]
                if not matching_rows.empty:
                    taks = matching_rows['Ortalama TAKS'].iloc[0]
                    ortalama_alan = matching_rows['Ortalama Bina Alanı'].iloc[0]
                    total_area += bina_sayisi * (1/taks * ortalama_alan)

    for bina_tip in ['BUYUK_SANAYI', 'BUYUK_TICARETHANE', 'KUCUK_SANAYI', 
                     'KUCUK_TICARETHANE', 'ORTA_SANAYI', 'ORTA_TICARETHANE']:
        if row[bina_tip] > 0:
            bina_sayisi = row[bina_tip]
            matching_rows = ticaret_df[ticaret_df['Bina Tipi'] == bina_tip]
            if not matching_rows.empty:
                taks = matching_rows['Ortalama TAKS'].iloc[0]
                ortalama_alan = matching_rows['Ortalama Bina Alanı'].iloc[0]
                total_area += bina_sayisi * (1/taks * ortalama_alan)
    
    return total_area

def determine_development_area(row):
    imar_kolonlari = ['Diğer', 'Kentsel Donusum', 'Mesken', 'Sanayi', 
                    'Tarimsal Sulama', 'Ticarethane', 'Yasakli Alan']
    
    imar_toplam = sum(row[col] if pd.notna(row[col]) else 0 for col in imar_kolonlari)
    builtup = row['bu_2023_fixed']
    bag_gucu = 0 if pd.isna(row['ORT_BAG_GUCU']) else row['ORT_BAG_GUCU']
    
    if builtup < 0.05 and imar_toplam >= 1:
        return 'İmarlı Yeni Genişleme Alanı'
    elif 0.05 <= builtup <= 1 and imar_toplam >= 1:
        return 'Kentsel Yerleşim Alanı'
    elif (bag_gucu > 0 and imar_toplam == 0) or (0.05 <= builtup):
        return 'Kent-Dışı Alan'
    elif builtup < 0.05 and bag_gucu == 0 and imar_toplam == 0:
        return 'İmarsız yeni genisleme alanı'

def calculate_net_buildable_area_ratio(row):
    if row['IsDevelopmentArea'] == 'İmarsız yeni genisleme alanı':
        return "Seçilecek"
    elif row['IsDevelopmentArea'] == 'Kent-Dışı Alan':
        return "Kent-Dışı Alan"
    
    # Yol ve kaldırım oranını hesaba kat (yasaksız imar alanının %22'si)
    yol_kaldirim_orani = 0.22
    kullanilabilir_oran = 1 - yol_kaldirim_orani
    
    # Önce yasaksız imar alanını yol/kaldırım için azalt, sonra TAKS'ı uygula
    return row['yasaksiz_imar_alani_m2'] * kullanilabilir_oran * row['Weighted_average_TAKS_ratio']

def calculate_real_usage_ratio(df):
    df['Ratio Bina Brüt'] = df.apply(
        lambda row: min(row['Toplam_Bina_Alani'] / row['yasaksiz_imar_alani_m2'], 1) 
        if row['yasaksiz_imar_alani_m2'] > 0 else 0, 
        axis=1
    )
    return df

def calculate_adjusted_taks_ratio(df):
    def calculate_saturation(row):
        try:
            imar_kolonlari = ['Diğer', 'Kentsel Donusum', 'Mesken', 'Sanayi', 
                            'Tarimsal Sulama', 'Ticarethane', 'Yasakli Alan']
            
            if row['Yasakli Alan'] > 0 and sum(row[col] if not pd.isna(row[col]) else 0 
                for col in imar_kolonlari if col != 'Yasakli Alan') == 0:
                return 1

            if row['bu_2023_fixed'] == 1:
                return 1

            if row['IsDevelopmentArea'] == 'İmarsız yeni genisleme alanı':
                return "Seçilecek"
            elif row['IsDevelopmentArea'] == 'Kent-Dışı Alan':
                return "Kent-Dışı Alan"
                
            net_build_up_area = row['2023_builtup_m2'] * row['cell_area']
            yapilasilabilecek_net_alan = row['Hucre_Yapilasilabilecek_Net_Alan_Orani']
            
            if yapilasilabilecek_net_alan == 0:
                return 0

            if (net_build_up_area > yapilasilabilecek_net_alan or 
                row['Toplam_Bina_Alani'] > yapilasilabilecek_net_alan):
                return 1

            saturated_net = min(net_build_up_area / yapilasilabilecek_net_alan, 1)
            ratio_brut = row['Ratio Bina Brüt']

            if abs(ratio_brut - saturated_net) > 0.20:
                return ratio_brut
            
            return saturated_net
                
        except Exception as e:
            print(f"Hata: {e}")
            return "Seçilecek"
    
    df['Saturation_ratio_2023'] = df.apply(calculate_saturation, axis=1)
    return df

def process_region_data(selected_region):
    try:
        print(f"Processing data for {selected_region}...")

        # CSV'leri okuma
        print(f"Processing data for {selected_region}...")

        # CSV'leri okuma - FIXED PATHS
        uydu = pd.read_csv(os.path.join(INPUT_DIR, 'uyduverisi-izmir.csv'))
        bina = pd.read_csv(os.path.join(INPUT_DIR, 'izmir_ada_imar_tipi_sayisi.csv'))
        imar = pd.read_csv(os.path.join(INPUT_DIR, 'v4_gdz_birlestirilmis_veri_bina_sanayi_kirilimlari.csv')) #ters yazılmıs

        # Excel'leri okuma
        mesken_df = pd.read_excel(os.path.join(INPUT_DIR, f'imar_id_sonuclari_{selected_region}.xlsx'))
        ticaret_df = pd.read_excel(os.path.join(INPUT_DIR, f'ticarethane_sanayi_analizleri-{selected_region}.xlsx'))
        mesken_df = mesken_df.dropna(subset=['IMAR_ID'])
        mesken_df['IMAR_ID'] = mesken_df['IMAR_ID'].astype(int)

        # Birleştirme
        birlesik = uydu.merge(bina, how='left', on='id')
        birlesik = birlesik.merge(imar, how='left', on='id')

        # Mapping
        imar_id_mapping = {
            1: '1-2 KATLI MESKEN',
            2: '3-4 KATLI MESKEN',
            3: '5-7 KATLI MESKEN',
            4: '8 USTU KATLI MESKEN',
            5: 'VILLA MESKEN'
        }

        # TAKS hesaplama
        birlesik['MESKEN SAYISI'] = birlesik['MESKEN SAYISI'].fillna(0)
        birlesik['Weighted_average_TAKS_ratio'] = birlesik.apply(
            lambda row: calculate_weighted_taks(row, mesken_df, ticaret_df, imar_id_mapping),
            axis=1
        )

        # Diğer hesaplamalar
        birlesik['bu_2023_fixed'] = birlesik.apply(fix_2023_builtup, axis=1)
        birlesik['IsDevelopmentArea'] = birlesik.apply(determine_development_area, axis=1)
        birlesik['Toplam_Bina_Alani'] = birlesik.apply(
            lambda row: calculate_total_building_area(row, mesken_df, ticaret_df, imar_id_mapping),
            axis=1
        )
        
        # Alan hesaplamaları
        imar_kolonlari = ['Diğer', 'Kentsel Donusum', 'Mesken', 'Sanayi', 
                         'Tarimsal Sulama', 'Ticarethane', 'Yasakli Alan']
        
        birlesik['yasaksiz_imar_alani_m2'] = birlesik.apply(
            lambda row: row['Grand Total'] - row['yasakli_alan_m2'] 
            if any(row[col] >= 1 for col in imar_kolonlari if col in birlesik.columns) else 0, 
            axis=1
        )
        
        birlesik['Hucre_Yapilasilabilecek_Net_Alan_Orani'] = birlesik.apply(
            lambda row: calculate_net_buildable_area_ratio(row),
            axis=1
        )
        
        # Final hesaplamalar
        birlesik = calculate_real_usage_ratio(birlesik)
        birlesik = calculate_adjusted_taks_ratio(birlesik)

        return birlesik

    except Exception as e:
        print(f"Error processing {selected_region}: {str(e)}")
        import traceback
        traceback.print_exc()
        return None

def save_outputs(birlesik, selected_region):
    try:
        # Ana CSV
        output_filename = os.path.join(OUTPUT_DIR, f'saturasyon_v8_{selected_region.lower()}.csv')
        birlesik.to_csv(output_filename, index=False, encoding="utf-8-sig")
        print(f"\nAna CSV kaydedildi: {output_filename}")

        # Stokastik CSV
        selected_columns = [
            'id', 'left', 'top', 'right', 'bottom', 'cell_area',
            '2018_builtup_m2', '2019_builtup_m2', '2020_builtup_m2',
            '2021_builtup_m2', '2022_builtup_m2', '2023_builtup_m2',
            '2024_builtup_m2', '2025_builtup_m2', '2026_builtup_m2',
            '2027_builtup_m2', '2028_builtup_m2', '2029_builtup_m2',
            '2030_builtup_m2', 'IsDevelopmentArea','Saturation_ratio_2023'
        ]
        
        new_df = birlesik[selected_columns]
        stokastik_output = os.path.join(STOKASTIK_DIR, f'saturation_update_{selected_region.lower()}.csv')
        new_df.to_csv(stokastik_output, index=False, encoding='utf-8-sig')
        print(f"Stokastik CSV kaydedildi: {stokastik_output}")
        
        return True
    except Exception as e:
        print(f"Error saving outputs: {str(e)}")
        return False

if __name__ == "__main__":
    selected_region = 'İzmir'
    result = process_region_data(selected_region)
    if result is not None:
        save_outputs(result, selected_region)
        print(f"{selected_region} processing completed successfully!")