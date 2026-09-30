"""
DEK (rooftop solar) noktalarinin, bagli olduklari trafolarin YILLIK TUKETIM ve
(dolayisiyla) PIK YUK tahminine etkisini hesaplayan bagimsiz analiz scripti.

MOTIVASYON (kullanicinin akil yurutmesi):
  DEK uretimi (gunes) ogle saatlerinde gerceklesir. Bir trafonun piki de
  ogle saatlerindeyse (tipik olarak SANAYI/TICARETHANE agirlikli trafolar -
  is yerleri gunduz calisir), DEK bu pikin bir kismini karsilayip pik yuku
  DUSURUR. Ama bir trafonun piki aksam saatlerindeyse (tipik olarak MESKEN
  agirlikli trafolar - insanlar aksam eve donup elektrik kullanir), DEK'in
  bu piki dusurucu etkisi neredeyse YOKTUR (uretim zaten bitmis olur).

  Elimizde trafo-bazli SAATLIK yuk egrisi olmadigi icin (bkz. methodology.md
  ve DTR-tahmin arastirmasi: sadece yillik YIL_DEMANT/YIL_TUKETIM var), tam
  saatlik netlestirme yapamiyoruz. Bunun yerine su YAKLASIK ama mantikli
  kurali uyguluyoruz:

    1. Her trafonun ABONE_GRUBU (MESKEN/TICARETHANE/SANAYI/...) bazinda
       tuketim payini Abone Verileri'nden cikar (zaten yuklu veri, ek
       kaynak gerekmez).
    2. "ticari_sanayi_orani" = (TICARETHANE + SANAYI tuketimi) / toplam tuketim.
       Bu, o trafonun ne kadar "ogle piki" karakterinde oldugunun bir proxy'si.
    3. O trafoya bagli DEK'lerin PVGIS'ten gelen YILLIK uretimini, bu oranla
       AGIRLIKLANDIRARAK trafonun yillik tuketiminden dus (ikili/hepsi-ya da-
       hicbiri degil, oransal - bir trafo %70 ticarethane ise DEK etkisinin
       de %70'i sayilir).
    4. Mevcut uygulamanin ZATEN kullandigi ayni pik-donusum formulunu
       (Peak_kW = Yillik_kWh / 8760 * K_FACTOR, K_FACTOR=2.5 varsayilan,
       bkz. girdiModülü.cs K_FACTOR ve Kod/SLF/SLF_yük_tahmini.py:193) hem
       DEK'siz hem DEK'li (netlestirilmis) tuketime uygulayip pik farkini
       raporla.

  BU BIR YAKLASIKLIK'TIR, gercek saatlik simulasyon degildir. Ileride trafo/
  fider bazli saatlik yuk verisi (orn. Fider Verileri modulundeki FIDER_DEMANT
  saatlik kayitlari) eklenirse, bu script'in yerini gercek saatlik netlestirme
  alabilir (PVGIS'in saatlik profili zaten hazir - pvgis_uretim_tahmini.py).

  Bu script SIMDILIK Optimal DTR'nin ana (4 dosyada tekrarlanan, kirilgan)
  kapasite atama dongusune ENTEGRE EDILMEDI - bagimsiz bir "once/sonra"
  rapor scripti. Boylece riskli/karmasik koda dokunmadan hizlica kullanilabilir
  bir sonuc uretiyoruz.

Girdiler:
  - Abone Verileri (xlsx/csv): TRAFO_KODU (=BAGLANDIGI_TRAFO_KODU), ABONE_GRUBU,
    YIL_TUKETIM_<yil> kolonlari gerekli.
  - DEK noktalari (xlsx/csv): en az ID, ENLEM, BOYLAM, KURULU_GUC_KWP kolonlari.
    TRAFO_KODU kolonu varsa dogrudan kullanilir; yoksa BINA_ID varsa
    bina_trafo_eslestirme.csv'den, o da yoksa en yakin trafo (Abone
    Verileri'ndeki koordinatlardan) mesafe ile bulunur.

Cikti: dek_trafo_etki_raporu.csv
  TRAFO_KODU, ticari_sanayi_orani, toplam_tuketim_kwh, dek_sayisi,
  toplam_dek_uretim_kwh, agirlikli_dek_katkisi_kwh, net_tuketim_kwh,
  pik_oncesi_kw, pik_sonrasi_kw, pik_azalma_yuzde

Kullanim:
  python dek_trafo_etki_hesabi.py <abone_verileri_yolu> <dek_noktalari_yolu> <yil> \
      [bina_trafo_lookup_yolu] [cikti_csv_yolu] [k_factor=2.5]
"""

import sys
import os
import math
import pandas as pd

sys.path.insert(0, os.path.dirname(__file__))
from pvgis_uretim_tahmini import saatlik_uretim_tahmini, PVGISHatasi  # noqa: E402

K_FACTOR_VARSAYILAN = 2.5   # girdiModülü.cs K_FACTOR ve SLF_yük_tahmini.py ile ayni sabit
SAAT_YIL = 8760
TICARI_SEGMENTLER = {"TICARETHANE", "SANAYI"}


def _dosya_oku(path, usecols=None):
    ext = os.path.splitext(path)[1].lower()
    if ext in (".xlsx", ".xlsm", ".xls"):
        return pd.read_excel(path, usecols=usecols)
    if ext == ".csv":
        return pd.read_csv(path, usecols=usecols)
    raise ValueError(f"Desteklenmeyen dosya uzantisi: {ext}")


def trafo_segment_karisimini_hesapla(abone_df: pd.DataFrame, yil: int) -> pd.DataFrame:
    """
    TRAFO_KODU bazinda ABONE_GRUBU'na gore tuketim payini ve
    'ticari_sanayi_orani' (0-1) proxy'sini hesaplar.
    """
    tuketim_kolonu = f"YIL_TUKETIM_{yil}"
    if tuketim_kolonu not in abone_df.columns:
        raise ValueError(f"Abone verilerinde '{tuketim_kolonu}' kolonu bulunamadi.")

    df = abone_df.dropna(subset=["BAGLANDIGI_TRAFO_KODU"]).copy()
    df[tuketim_kolonu] = pd.to_numeric(df[tuketim_kolonu], errors="coerce").fillna(0)
    df.loc[df[tuketim_kolonu] < 0, tuketim_kolonu] = 0  # negatif/hatali tuketim korumasi

    toplam = df.groupby("BAGLANDIGI_TRAFO_KODU")[tuketim_kolonu].sum().rename("toplam_tuketim_kwh")

    df["ticari_mi"] = df["ABONE_GRUBU"].isin(TICARI_SEGMENTLER)
    ticari = (
        df[df["ticari_mi"]]
        .groupby("BAGLANDIGI_TRAFO_KODU")[tuketim_kolonu]
        .sum()
        .rename("ticari_sanayi_tuketim_kwh")
    )

    sonuc = pd.concat([toplam, ticari], axis=1).fillna(0).reset_index()
    sonuc = sonuc.rename(columns={"BAGLANDIGI_TRAFO_KODU": "TRAFO_KODU"})
    sonuc["ticari_sanayi_orani"] = sonuc.apply(
        lambda r: (r["ticari_sanayi_tuketim_kwh"] / r["toplam_tuketim_kwh"]) if r["toplam_tuketim_kwh"] > 0 else 0.0,
        axis=1,
    )
    return sonuc[["TRAFO_KODU", "toplam_tuketim_kwh", "ticari_sanayi_orani"]]


def _haversine_km(enlem1, boylam1, enlem2, boylam2):
    R = 6371.0
    p1, p2 = math.radians(enlem1), math.radians(enlem2)
    dphi = math.radians(enlem2 - enlem1)
    dlambda = math.radians(boylam2 - boylam1)
    a = math.sin(dphi / 2) ** 2 + math.cos(p1) * math.cos(p2) * math.sin(dlambda / 2) ** 2
    return 2 * R * math.asin(math.sqrt(a))


def en_yakin_trafoyu_bul(enlem, boylam, trafo_koordinat_df: pd.DataFrame):
    """
    trafo_koordinat_df: TRAFO_KODU, TRAFO_X_KOORDINAT (boylam), TRAFO_Y_KOORDINAT (enlem)
    Basit ama yeterli: tum trafolar uzerinde lineer tarama (DTR sayisi tipik
    olarak birkaç bin - bu olcekte KDTree'ye gerek yok).
    """
    en_yakin_kodu = None
    en_yakin_mesafe = float("inf")
    for _, row in trafo_koordinat_df.iterrows():
        mesafe = _haversine_km(enlem, boylam, row["TRAFO_Y_KOORDINAT"], row["TRAFO_X_KOORDINAT"])
        if mesafe < en_yakin_mesafe:
            en_yakin_mesafe = mesafe
            en_yakin_kodu = row["TRAFO_KODU"]
    return en_yakin_kodu, en_yakin_mesafe


def dek_noktalarina_trafo_ata(dek_df: pd.DataFrame, abone_df: pd.DataFrame,
                               bina_trafo_lookup: pd.DataFrame = None) -> pd.DataFrame:
    """
    DEK noktalarinda TRAFO_KODU eksikse sirayla dener:
      1) BINA_ID varsa ve bina_trafo_lookup verilmisse -> dogrudan eslesme
      2) yoksa -> en yakin trafo (Abone Verileri'ndeki TRAFO_X/Y_KOORDINAT'tan
         turetilen, her trafo icin ortalama koordinat)
    """
    dek_df = dek_df.copy()
    if "TRAFO_KODU" not in dek_df.columns:
        dek_df["TRAFO_KODU"] = None

    eksik_maske = dek_df["TRAFO_KODU"].isna()

    if bina_trafo_lookup is not None and "BINA_ID" in dek_df.columns:
        lookup = bina_trafo_lookup.set_index("BINA_ID")["TRAFO_KODU"]
        dek_df.loc[eksik_maske, "TRAFO_KODU"] = dek_df.loc[eksik_maske, "BINA_ID"].map(lookup)
        eksik_maske = dek_df["TRAFO_KODU"].isna()

    if eksik_maske.any():
        trafo_koordinat_df = (
            abone_df.dropna(subset=["BAGLANDIGI_TRAFO_KODU", "ABONE_X_KOORDINAT", "ABONE_Y_KOORDINAT"])
            .groupby("BAGLANDIGI_TRAFO_KODU")
            .agg(TRAFO_X_KOORDINAT=("ABONE_X_KOORDINAT", "mean"), TRAFO_Y_KOORDINAT=("ABONE_Y_KOORDINAT", "mean"))
            .reset_index()
            .rename(columns={"BAGLANDIGI_TRAFO_KODU": "TRAFO_KODU"})
        )
        for idx in dek_df[eksik_maske].index:
            enlem = dek_df.at[idx, "ENLEM"]
            boylam = dek_df.at[idx, "BOYLAM"]
            trafo_kodu, mesafe_km = en_yakin_trafoyu_bul(enlem, boylam, trafo_koordinat_df)
            dek_df.at[idx, "TRAFO_KODU"] = trafo_kodu
            dek_df.at[idx, "trafo_mesafe_km"] = round(mesafe_km, 3)

    return dek_df


def dek_uretimlerini_hesapla(dek_df: pd.DataFrame) -> pd.DataFrame:
    """Her DEK noktasi icin PVGIS'ten yillik uretimi ceker (saatlik profil hesaba dahil ama
    burada sadece yillik toplam kullanilir - saatlik netlestirme sonraki asama)."""
    dek_df = dek_df.copy()
    dek_df["yillik_uretim_kwh"] = 0.0
    for idx, row in dek_df.iterrows():
        try:
            sonuc = saatlik_uretim_tahmini(row["ENLEM"], row["BOYLAM"], row["KURULU_GUC_KWP"])
            dek_df.at[idx, "yillik_uretim_kwh"] = sonuc["yillik_uretim_kwh"]
        except PVGISHatasi as e:
            print(f"UYARI: DEK noktasi {row.get('ID', idx)} icin PVGIS hatasi: {e}", file=sys.stderr)
    return dek_df


def trafo_etkisini_hesapla(trafo_mix_df: pd.DataFrame, dek_df: pd.DataFrame,
                            k_factor: float = K_FACTOR_VARSAYILAN) -> pd.DataFrame:
    dek_ozet = (
        dek_df.groupby("TRAFO_KODU")
        .agg(dek_sayisi=("yillik_uretim_kwh", "count"), toplam_dek_uretim_kwh=("yillik_uretim_kwh", "sum"))
        .reset_index()
    )

    sonuc = trafo_mix_df.merge(dek_ozet, on="TRAFO_KODU", how="left")
    sonuc["dek_sayisi"] = sonuc["dek_sayisi"].fillna(0).astype(int)
    sonuc["toplam_dek_uretim_kwh"] = sonuc["toplam_dek_uretim_kwh"].fillna(0.0)

    # Oransal agirliklandirma: bir trafo ne kadar ticari/sanayi agirliklıysa,
    # baglı DEK'lerin ogle-piki dusurucu etkisi o kadar "sayilir".
    sonuc["agirlikli_dek_katkisi_kwh"] = sonuc["toplam_dek_uretim_kwh"] * sonuc["ticari_sanayi_orani"]
    sonuc["net_tuketim_kwh"] = (sonuc["toplam_tuketim_kwh"] - sonuc["agirlikli_dek_katkisi_kwh"]).clip(lower=0)

    sonuc["pik_oncesi_kw"] = sonuc["toplam_tuketim_kwh"] / SAAT_YIL * k_factor
    sonuc["pik_sonrasi_kw"] = sonuc["net_tuketim_kwh"] / SAAT_YIL * k_factor
    sonuc["pik_azalma_yuzde"] = sonuc.apply(
        lambda r: round(100 * (r["pik_oncesi_kw"] - r["pik_sonrasi_kw"]) / r["pik_oncesi_kw"], 2)
        if r["pik_oncesi_kw"] > 0 else 0.0,
        axis=1,
    )

    return sonuc.sort_values("agirlikli_dek_katkisi_kwh", ascending=False).reset_index(drop=True)


def main():
    if len(sys.argv) < 4:
        print("Kullanim: python dek_trafo_etki_hesabi.py <abone_verileri_yolu> <dek_noktalari_yolu> "
              "<yil> [bina_trafo_lookup_yolu] [cikti_csv_yolu] [k_factor=2.5]", file=sys.stderr)
        sys.exit(1)

    abone_yolu = sys.argv[1]
    dek_yolu = sys.argv[2]
    yil = int(sys.argv[3])
    bina_trafo_lookup_yolu = sys.argv[4] if len(sys.argv) > 4 and sys.argv[4].lower() != "none" else None
    cikti_yolu = sys.argv[5] if len(sys.argv) > 5 else os.path.join(
        os.path.dirname(__file__), "girdi", "dek_trafo_etki_raporu.csv"
    )
    k_factor = float(sys.argv[6]) if len(sys.argv) > 6 else K_FACTOR_VARSAYILAN

    print(f"Abone verileri okunuyor: {abone_yolu}")
    abone_df = _dosya_oku(abone_yolu)

    print(f"DEK noktalari okunuyor: {dek_yolu}")
    dek_df = _dosya_oku(dek_yolu)

    bina_trafo_lookup = None
    if bina_trafo_lookup_yolu:
        print(f"Bina-trafo lookup okunuyor: {bina_trafo_lookup_yolu}")
        bina_trafo_lookup = _dosya_oku(bina_trafo_lookup_yolu)

    print("Trafo segment (Mesken/Ticarethane/Sanayi) karisimi hesaplaniyor...")
    trafo_mix_df = trafo_segment_karisimini_hesapla(abone_df, yil)
    print(f"{len(trafo_mix_df)} trafo icin segment karisimi cikarildi.")

    print("DEK noktalarina trafo atanıyor (eksikse en yakin trafo ile)...")
    dek_df = dek_noktalarina_trafo_ata(dek_df, abone_df, bina_trafo_lookup)

    print(f"{len(dek_df)} DEK noktasi icin PVGIS'ten yillik uretim cekiliyor (bu biraz surebilir)...")
    dek_df = dek_uretimlerini_hesapla(dek_df)

    print("Trafo bazli DEK etkisi (pik oncesi/sonrasi) hesaplaniyor...")
    sonuc_df = trafo_etkisini_hesapla(trafo_mix_df, dek_df, k_factor)

    os.makedirs(os.path.dirname(cikti_yolu), exist_ok=True)
    sonuc_df.to_csv(cikti_yolu, index=False, encoding="utf-8-sig")
    print(f"Rapor yazildi: {cikti_yolu} ({len(sonuc_df)} trafo)")

    etkilenen = sonuc_df[sonuc_df["dek_sayisi"] > 0]
    if len(etkilenen) > 0:
        print(f"\n{len(etkilenen)} trafo DEK'ten etkilendi. Ortalama pik azalmasi: "
              f"%{etkilenen['pik_azalma_yuzde'].mean():.2f}")
        print(etkilenen[["TRAFO_KODU", "ticari_sanayi_orani", "dek_sayisi", "pik_azalma_yuzde"]].to_string(index=False))


if __name__ == "__main__":
    main()
