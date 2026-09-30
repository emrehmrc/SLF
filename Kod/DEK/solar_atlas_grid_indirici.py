"""
Global Solar Atlas (World Bank / Solargis, api.globalsolaratlas.info) nokta-sorgu
API'sini kullanarak, verilen bir bounding box (il/ilce sinirlari) icin bir
IZGARA (grid) halinde gunes radyasyonu/PV potansiyeli verisi ceker ve tek bir
CSV'ye yazar. Bu CSV, mevcut CBS.cs'teki heatmap altyapisiyla (CreateHeatmap/
GetHeatmapColor, sayisal bir kolona gore renklendirme) DOGRUDAN uyumlu -
harita uzerinde ac/kapa (checkbox) bir "Solar Irradiance" katmani olarak
gosterilebilir.

Neden Global Solar Atlas (PVGIS degil)?
  - Global Solar Atlas'in resmi toplu (ulke bazli GeoTIFF) indirme mekanizmasi
    script'lenebilir bir API degil (JS/form tabanli site, bkz. arastirma notu
    asagida) - o yuzden BIZ kendimiz nokta nokta orneklem aliyoruz.
  - PVGIS de ayni sekilde nokta-sorgu API'si (bkz. pvgis_uretim_tahmini.py),
    o script tek-nokta (DEK ekleme aninda) hassas saatlik uretim icin;
    bu script ise coklu-nokta (bolgesel katman) icin. Ikisi de kullanilabilir,
    Global Solar Atlas burada tercih edildi cunku annual PVOUT_csi (kWh/kWp)
    degeri dogrudan "bu bolgede 1 kWp bir sistem yilda ne kadar uretir" seklinde
    yorumlanabilir ve tek cagrida (aylik kirilim da dahil) donuyor.

API: https://api.globalsolaratlas.info/data/lta?loc=<lat>,<lon>  (ucretsiz, key yok)
Donen alanlar (annual): PVOUT_csi (kWh/kWp/yil), GHI, DNI, DIF, GTI_opta (kWh/m2/yil),
                         OPTA (optimal egim, derece), TEMP (°C), ELE (rakim, m)

Kullanim:
    python solar_atlas_grid_indirici.py                     # argumansiz -> Karsiyaka/Izmir varsayilani, 250m
    python solar_atlas_grid_indirici.py <min_enlem> <min_boylam> <max_enlem> <max_boylam> \
        [cozunurluk_metre] [cikti_csv_yolu]                  # baska bir bolge icin acik sinir

Not: Cozunurluk kucultukce (orn. 100m) nokta sayisi ve dolayisiyla sure/istek
sayisi karesel artar. Gunes radyasyonu, bina golgelemesi haricinde, sehir
olcegindeki kisa mesafelerde (100-500m) cok fazla degismez (arazi/rakim
etkisi haric) - o yuzden ilk katman icin 250-500m onerilir. Tek tek cati
golgelemesi bu katmanda YOK; onun icin DEK noktasi eklenirken
pvgis_uretim_tahmini.py ile tekil, o noktaya ozel sorgu yapilmali.
"""

import sys
import os
import csv
import time
import concurrent.futures
import requests

API_URL = "https://api.globalsolaratlas.info/data/lta"
ES_YARICAPI_KM = 6371.0
ESZAMANLI_ISTEK_SAYISI = 6          # API'ye kibarca davranmak icin sinirli concurrency
DENEME_SAYISI = 3
ZAMAN_ASIMI = 20

# Karsiyaka/Izmir icin, "hucre/Karsiyaka_grid.csv" (8023 hucre, ~73x93m) dosyasindan
# okunan gercek sinirlar (min/max left/right/top/bottom). Şimdilik tek ilcede
# calisildigi icin varsayilan olarak buraya sabitlendi - baska bir ilce/il icin
# CLI argumanlariyla (asagida main()) override edilebilir.
KARSIYAKA_MIN_ENLEM = 38.4479
KARSIYAKA_MIN_BOYLAM = 27.0621
KARSIYAKA_MAX_ENLEM = 38.5588
KARSIYAKA_MAX_BOYLAM = 27.1871
KARSIYAKA_COZUNURLUK_METRE = 250.0


def metre_to_derece(metre, referans_enlem):
    """Kaba (yerel) metre->derece donusumu - kucuk bolgeler icin yeterince dogru."""
    enlem_derece = metre / 111_320.0
    boylam_derece = metre / (111_320.0 * max(0.1, abs(__import__("math").cos(__import__("math").radians(referans_enlem)))))
    return enlem_derece, boylam_derece


def grid_olustur(min_enlem, min_boylam, max_enlem, max_boylam, cozunurluk_metre):
    ref_enlem = (min_enlem + max_enlem) / 2
    d_enlem, d_boylam = metre_to_derece(cozunurluk_metre, ref_enlem)

    noktalar = []
    enlem = min_enlem
    while enlem <= max_enlem:
        boylam = min_boylam
        while boylam <= max_boylam:
            noktalar.append((round(enlem, 6), round(boylam, 6)))
            boylam += d_boylam
        enlem += d_enlem
    return noktalar


def nokta_sorgula(enlem_boylam):
    enlem, boylam = enlem_boylam
    for deneme in range(1, DENEME_SAYISI + 1):
        try:
            r = requests.get(API_URL, params={"loc": f"{enlem},{boylam}"}, timeout=ZAMAN_ASIMI)
        except requests.exceptions.RequestException:
            time.sleep(1.0 * deneme)
            continue

        if r.status_code == 200:
            try:
                d = r.json()
                yillik = d["annual"]["data"] if "data" in d.get("annual", {}) else d["annual"]
                return {
                    "ENLEM": enlem,
                    "BOYLAM": boylam,
                    "PVOUT_kWh_kWp_yil": yillik.get("PVOUT_csi"),
                    "GHI_kWh_m2_yil": yillik.get("GHI"),
                    "DNI_kWh_m2_yil": yillik.get("DNI"),
                    "GTI_opta_kWh_m2_yil": yillik.get("GTI_opta"),
                    "Optimal_Egim_derece": yillik.get("OPTA"),
                    "Ortalama_Sicaklik_C": yillik.get("TEMP"),
                    "Rakim_m": yillik.get("ELE"),
                }
            except (KeyError, ValueError):
                return None
        time.sleep(1.0 * deneme)
    return None


def grid_indir(min_enlem, min_boylam, max_enlem, max_boylam, cozunurluk_metre, ilerleme_yaz=True):
    noktalar = grid_olustur(min_enlem, min_boylam, max_enlem, max_boylam, cozunurluk_metre)
    toplam = len(noktalar)
    if ilerleme_yaz:
        print(f"{toplam} grid noktasi olusturuldu ({cozunurluk_metre}m cozunurluk). Sorgulaniyor...")

    sonuclar = []
    basarisiz = 0
    with concurrent.futures.ThreadPoolExecutor(max_workers=ESZAMANLI_ISTEK_SAYISI) as havuz:
        for i, sonuc in enumerate(havuz.map(nokta_sorgula, noktalar), start=1):
            if sonuc is not None:
                sonuclar.append(sonuc)
            else:
                basarisiz += 1
            if ilerleme_yaz and i % 50 == 0:
                print(f"  {i}/{toplam} tamamlandi ({basarisiz} basarisiz/deniz-uzeri)")

    if ilerleme_yaz:
        print(f"Bitti: {len(sonuclar)} basarili, {basarisiz} basarisiz (muhtemelen deniz ustu/API hatasi).")
    return sonuclar


def main():
    if len(sys.argv) >= 5:
        # Baska bir bolge icin acikca sinir verildiyse onu kullan.
        min_enlem, min_boylam, max_enlem, max_boylam = (float(x) for x in sys.argv[1:5])
        cozunurluk_metre = float(sys.argv[5]) if len(sys.argv) > 5 else 250.0
        cikti_yolu = sys.argv[6] if len(sys.argv) > 6 else os.path.join(
            os.path.dirname(__file__), "girdi", "solar_irradiance_grid.csv"
        )
    else:
        # Varsayilan: Karsiyaka/Izmir (hucre/Karsiyaka_grid.csv sinirlari, 250m).
        print("Argumansiz calistirildi - Karsiyaka/Izmir varsayilan sinirlari kullaniliyor.")
        min_enlem, min_boylam = KARSIYAKA_MIN_ENLEM, KARSIYAKA_MIN_BOYLAM
        max_enlem, max_boylam = KARSIYAKA_MAX_ENLEM, KARSIYAKA_MAX_BOYLAM
        cozunurluk_metre = KARSIYAKA_COZUNURLUK_METRE
        cikti_yolu = os.path.join(os.path.dirname(__file__), "girdi", "solar_irradiance_karsiyaka.csv")

    os.makedirs(os.path.dirname(cikti_yolu), exist_ok=True)

    sonuclar = grid_indir(min_enlem, min_boylam, max_enlem, max_boylam, cozunurluk_metre)

    if not sonuclar:
        print("HATA: hicbir nokta basariyla cekilemedi.", file=sys.stderr)
        sys.exit(2)

    alanlar = list(sonuclar[0].keys())
    with open(cikti_yolu, "w", newline="", encoding="utf-8-sig") as f:
        writer = csv.DictWriter(f, fieldnames=alanlar)
        writer.writeheader()
        writer.writerows(sonuclar)

    print(f"Cikti yazildi: {cikti_yolu} ({len(sonuclar)} satir)")


if __name__ == "__main__":
    main()
