"""
PVGIS (Avrupa Komisyonu JRC) API kullanarak, tek bir cati/rooftop DEK
(solar) noktasi icin yillik + saatlik uretim tahmini.

API: https://re.jrc.ec.europa.eu/api/v5_2/seriescalc  (ucretsiz, key gerekmiyor)
Kapsama alani Turkiye'yi iceriyor (dogrulandi: Izmir/Karsiyaka test edildi).

!!! KOORDINAT UYARISI !!!
Bu script parametre olarak ENLEM (latitude) ve BOYLAM (longitude) bekler,
X/Y DEGIL. Kod tabanindaki modullerde X/Y kullanimi TUTARSIZ:
  - AboneVerileri / EASarjModulu / DEKModulu (VEER, toplu import): X=BOYLAM, Y=ENLEM
  - DEKCenterPopupForm.cs (harita uzerinden TEK NOKTA ekleme, DEKCenterPopupForm.cs:51-52):
        DEK_X_KOORDINAT = Enlem, DEK_Y_KOORDINAT = Boylam   <-- TERS!
Yani DEKCenterPopupForm'dan gelen DEK_X_KOORDINAT/DEK_Y_KOORDINAT degerlerini
bu scripte dogrudan X->boylam, Y->enlem seklinde vermeyin - tersini yapin
(DEK_X_KOORDINAT -> enlem, DEK_Y_KOORDINAT -> boylam). Bu script'i C#'tan
cagirirken bu iki alani karistirmamaya ozellikle dikkat edin; asagidaki
main() CLI'i de parametreleri acikca "enlem" "boylam" olarak alir.

Kullanim (CLI):
    python pvgis_uretim_tahmini.py <enlem> <boylam> <kurulu_guc_kwp> [cikti_json_yolu] [egim] [azimut]

    - egim/azimut verilmezse PVGIS "optimalangles=1" ile cati icin en iyi
      egim+yonelimi kendisi hesaplar (rooftop icin genelde makul bir varsayim).
    - Sonuc, stdout'a TEK SATIR JSON olarak yazilir (C# tarafinin process
      stdout'unu okuyup parse etmesi icin), cikti_json_yolu verilmisse ayrica
      o dosyaya da (okunabilir, indented) yazilir.

Kullanim (modul olarak):
    from pvgis_uretim_tahmini import saatlik_uretim_tahmini
    sonuc = saatlik_uretim_tahmini(enlem=38.4830, boylam=27.1298, kurulu_guc_kwp=10)
"""

import sys
import json
import time
import requests
import pandas as pd

PVGIS_BASE_URL = "https://re.jrc.ec.europa.eu/api/v5_2/seriescalc"
VARSAYILAN_KAYIP_YUZDE = 14.0       # PVGIS varsayilani (kablo, inverter, kirlenme vb. sistem kayiplari)
VARSAYILAN_TEKNOLOJI = "crystSi"     # kristal silikon (en yaygin rooftop teknolojisi)
VARSAYILAN_MONTAJ = "building"       # cati montaji (serbest-duran "free" a gore farkli isi kaybi modeli)
VARSAYILAN_YIL_ARALIGI = (2016, 2020)  # PVGIS-SARAH2/3 veritabaninin guvenilir kapsadigi ortak yillar


class PVGISHatasi(Exception):
    pass


def _pvgis_cagir(enlem, boylam, kurulu_guc_kwp, egim, azimut, kayip, teknoloji, montaj,
                  baslangic_yili, bitis_yili, deneme_sayisi=3, zaman_asimi=30):
    params = {
        "lat": enlem,
        "lon": boylam,
        "peakpower": kurulu_guc_kwp,
        "loss": kayip,
        "pvtechchoice": teknoloji,
        "mountingplace": montaj,
        "pvcalculation": 1,
        "outputformat": "json",
        "startyear": baslangic_yili,
        "endyear": bitis_yili,
    }

    if egim is None and azimut is None:
        params["optimalangles"] = 1
    elif egim is None:
        params["optimalinclination"] = 1
        params["aspect"] = azimut
    else:
        params["angle"] = egim
        params["aspect"] = azimut if azimut is not None else 0

    son_hata = None
    for deneme in range(1, deneme_sayisi + 1):
        try:
            r = requests.get(PVGIS_BASE_URL, params=params, timeout=zaman_asimi)
        except requests.exceptions.RequestException as e:
            son_hata = e
            time.sleep(1.5 * deneme)
            continue

        if r.status_code == 200:
            return r.json()

        try:
            mesaj = r.json().get("message", r.text)
        except Exception:
            mesaj = r.text
        # "Location over the sea" gibi kalici (retry ile duzelmeyecek) hatalar icin
        # hemen cik; gecici sunucu hatalarinda (5xx) tekrar dene.
        if r.status_code < 500:
            raise PVGISHatasi(f"PVGIS API hatasi ({r.status_code}): {mesaj}")
        son_hata = PVGISHatasi(f"PVGIS API hatasi ({r.status_code}): {mesaj}")
        time.sleep(1.5 * deneme)

    raise PVGISHatasi(f"PVGIS API'ye {deneme_sayisi} denemede ulasilamadi: {son_hata}")


def saatlik_uretim_tahmini(enlem, boylam, kurulu_guc_kwp,
                            egim=None, azimut=None,
                            kayip=VARSAYILAN_KAYIP_YUZDE,
                            teknoloji=VARSAYILAN_TEKNOLOJI,
                            montaj=VARSAYILAN_MONTAJ,
                            yil_araligi=VARSAYILAN_YIL_ARALIGI):
    """
    Belirtilen koordinat ve kurulu guc icin PVGIS'ten coklu-yil saatlik
    uretim verisi ceker ve saat-of-year bazinda ortalayarak TEK bir
    "tipik yil" saatlik profili (8760 satir) uretir.

    Donen dict:
        {
          "yillik_uretim_kwh": float,
          "ozgul_verim_kwh_kwp": float,           # yillik_uretim / kurulu_guc -> teknoloji-bagimsiz karsilastirma
          "kullanilan_egim": float,
          "kullanilan_azimut": float,
          "radyasyon_veritabani": str,
          "kaynak_yillari": [yil1, yil2, ...],
          "saatlik_profil_kw": [8760 float],       # sirasiyla 1 Ocak 00:00 -> 31 Aralik 23:00, kurulu_guc_kwp'e gore olceklenmis anlik guc (kW)
        }
    """
    baslangic_yili, bitis_yili = yil_araligi
    ham = _pvgis_cagir(
        enlem, boylam, kurulu_guc_kwp, egim, azimut, kayip, teknoloji, montaj,
        baslangic_yili, bitis_yili,
    )

    hourly = ham["outputs"]["hourly"]
    df = pd.DataFrame(hourly)
    # time formati: "20200615:1610" -> tarih ve saat ayikla, yili at (coklu yil ortalamasi icin)
    df["ay_gun_saat"] = df["time"].str.slice(4, 8) + df["time"].str.slice(9, 11)
    # 29 Subat gibi nadir gunleri de dahil et ama grupla ortalamaya cikacak

    ortalama = df.groupby("ay_gun_saat", sort=True)["P"].mean().reset_index()
    ortalama = ortalama.sort_values("ay_gun_saat").reset_index(drop=True)

    saatlik_kw = (ortalama["P"] / 1000.0).round(4).tolist()  # W -> kW
    yillik_kwh = float(sum(saatlik_kw))

    pv_module = ham["inputs"]["pv_module"]
    mounting_system = ham["inputs"].get("mounting_system", {})
    kullanilan_egim = None
    kullanilan_azimut = None
    for key in ("fixed", "free"):
        if key in mounting_system:
            kullanilan_egim = mounting_system[key].get("slope", {}).get("value")
            kullanilan_azimut = mounting_system[key].get("azimuth", {}).get("value")
            break

    return {
        "enlem": enlem,
        "boylam": boylam,
        "kurulu_guc_kwp": kurulu_guc_kwp,
        "yillik_uretim_kwh": round(yillik_kwh, 1),
        "ozgul_verim_kwh_kwp": round(yillik_kwh / kurulu_guc_kwp, 1) if kurulu_guc_kwp else None,
        "kullanilan_egim": kullanilan_egim,
        "kullanilan_azimut": kullanilan_azimut,
        "radyasyon_veritabani": ham["inputs"]["meteo_data"]["radiation_db"],
        "kaynak_yillari": list(range(baslangic_yili, bitis_yili + 1)),
        "teknoloji": pv_module.get("technology"),
        "sistem_kaybi_yuzde": pv_module.get("system_loss"),
        "saatlik_profil_kw": saatlik_kw,
    }


def main():
    if len(sys.argv) < 4:
        print("Kullanim: python pvgis_uretim_tahmini.py <enlem> <boylam> <kurulu_guc_kwp> "
              "[cikti_json_yolu] [egim] [azimut]", file=sys.stderr)
        sys.exit(1)

    enlem = float(sys.argv[1])
    boylam = float(sys.argv[2])
    kurulu_guc_kwp = float(sys.argv[3])
    cikti_yolu = sys.argv[4] if len(sys.argv) > 4 and sys.argv[4].lower() != "none" else None
    egim = float(sys.argv[5]) if len(sys.argv) > 5 and sys.argv[5].lower() != "none" else None
    azimut = float(sys.argv[6]) if len(sys.argv) > 6 and sys.argv[6].lower() != "none" else None

    try:
        sonuc = saatlik_uretim_tahmini(enlem, boylam, kurulu_guc_kwp, egim=egim, azimut=azimut)
    except PVGISHatasi as e:
        hata = {"hata": str(e)}
        print(json.dumps(hata, ensure_ascii=False))
        if cikti_yolu:
            with open(cikti_yolu, "w", encoding="utf-8") as f:
                json.dump(hata, f, ensure_ascii=False, indent=2)
        sys.exit(2)

    # stdout'a kompakt tek satir (C# process stdout capture icin)
    print(json.dumps(sonuc, ensure_ascii=False))

    if cikti_yolu:
        with open(cikti_yolu, "w", encoding="utf-8") as f:
            json.dump(sonuc, f, ensure_ascii=False, indent=2)


if __name__ == "__main__":
    main()
