import pandas as pd
from sklearn.cluster import KMeans
import numpy as np
import sys
import json
import os


# Get the config path from the first command-line argument
config_path = sys.argv[1]

# Load the config.json file
with open(config_path, 'r', encoding='utf-8') as f:
    config = json.load(f)

# Extract the necessary paths from config.json
ana_klasor_yolu = config['Ana_Klasör_Yolu']
il = config['İl']
ilce = config['İlçe']
ea_klasor = config['EA']['Klasör']
ea_sonuclar_klasor = config['EA']['SONUÇLAR_klasör']
ea_hucre_ilk_versiyon = config['EA']['GİRDİLER_ilk']
ea_hucresel_bina_sayıları = config['EA']['BİNA_SAYILARI']
ea_hucresel_bina_sayıları = config['EA']['BİNA_SAYILARI']
ea_delta = config['EA']['DELTA_SONUCLAR']

# Construct the full path to the EA_SONUCLAR.xlsx file output dynamically
output_dir = os.path.join(ana_klasor_yolu, il, ilce, ea_klasor, ea_sonuclar_klasor)
output_file = os.path.join(output_dir, 'EA_SONUÇLAR.xlsx')

# EA ların hucrelere imar analizi sonucu ilk kez dagıtıldıgı dosyanın pathi
hucre_ilk_versiyon_dosyası = os.path.join(ana_klasor_yolu, il, ilce, ea_klasor, ea_hucre_ilk_versiyon)
bina_sayıları_dosyası = os.path.join(ana_klasor_yolu, il, ilce, ea_klasor, ea_hucresel_bina_sayıları)
delta_sonuclar_path = os.path.join(ana_klasor_yolu, il, ilce, ea_klasor, ea_delta)

print(pd.read_excel(delta_sonuclar_path))