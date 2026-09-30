"""
BINA_ID -> TRAFO_KODU statik eslestirme tablosu olusturucu.

Amac: Haritada bir bina poligonuna tiklandiginda (rooftop DEK/solar yerlesimi),
o binanin hangi dagitim trafosuna (DTR) bagli oldugunu aninda soyleyebilmek.
Abone Verileri tablosunda zaten her abonenin BINA_ID + BAGLANDIGI_TRAFO_KODU
bilgisi var; bu script o tabloyu bina bazinda tekillestirip (bir bina = bir
trafo, coklu abone -> tek satir) ayri, hafif bir lookup tablosu haline getirir.

Ornek veri uzerinde (ABONE_KARSIYAKA_DENEME.xlsx, 197.233 satir / 23.756 bina)
DOGRULANDI: hicbir binada birden fazla FARKLI trafo kodu yok (conflict = 0).
Yine de ileride gercek/canli veride conflict cikabilir ihtimaline karsi, bu
script conflict'leri sessizce yutmuyor, ayri bir rapor dosyasina yaziyor.

Kullanim:
    python bina_trafo_eslestirme_olustur.py <abone_dosyasi.xlsx|csv> [cikti_klasoru]

Cikti (varsayilan: Kod/DEK/girdi/):
    bina_trafo_eslestirme.csv      -> BINA_ID, TRAFO_KODU, X, Y, ILCE_ID, ABONE_SAYISI
    bina_trafo_conflict_raporu.csv -> sadece >1 farkli trafoya sahip binalar (varsa)

Not: Bu script Oracle DB'ye baglanmiyor; girdi olarak zaten disari aktarilmis
(Excel/CSV) bir Abone Verileri dosyasi bekliyor. Canli DB akisina baglamak
istenirse, `DatabaseHelper.LoadTable("abone_final_tablosu")` C# tarafindan
CSV'ye export edilip bu script'e verilebilir, ya da bu script'in mantigi
ayni sorguyla (GROUP BY BINA_ID) dogrudan SQL'e tasinabilir.
"""

import sys
import os
import pandas as pd

REQUIRED_COLUMNS = [
    "BINA_ID",
    "BAGLANDIGI_TRAFO_KODU",
    "ABONE_X_KOORDINAT",
    "ABONE_Y_KOORDINAT",
    "ABONE_ILCE_ID",
]


def read_abone_verileri(path: str) -> pd.DataFrame:
    ext = os.path.splitext(path)[1].lower()
    if ext in (".xlsx", ".xlsm", ".xls"):
        df = pd.read_excel(path, usecols=lambda c: c in REQUIRED_COLUMNS)
    elif ext == ".csv":
        df = pd.read_csv(path, usecols=lambda c: c in REQUIRED_COLUMNS)
    else:
        raise ValueError(f"Desteklenmeyen dosya uzantisi: {ext}")

    missing = [c for c in REQUIRED_COLUMNS if c not in df.columns]
    if missing:
        raise ValueError(f"Girdi dosyasinda beklenen kolonlar eksik: {missing}")
    return df


def build_lookup(df: pd.DataFrame):
    """
    BINA_ID bazinda tekillestirir.
    Donen: (lookup_df, conflict_df)
        lookup_df: BINA_ID, TRAFO_KODU, X, Y, ILCE_ID, ABONE_SAYISI
        conflict_df: >1 farkli (null olmayan) trafo koduna sahip binalar
                      (varsa; normalde bos olmasi beklenir)
    """
    df = df.dropna(subset=["BINA_ID"]).copy()

    trafo_per_bina = df.groupby("BINA_ID")["BAGLANDIGI_TRAFO_KODU"].nunique(dropna=True)
    conflict_bina_ids = trafo_per_bina[trafo_per_bina > 1].index

    conflict_df = (
        df[df["BINA_ID"].isin(conflict_bina_ids)]
        .groupby(["BINA_ID", "BAGLANDIGI_TRAFO_KODU"])
        .size()
        .reset_index(name="ABONE_SAYISI")
        .sort_values(["BINA_ID", "ABONE_SAYISI"], ascending=[True, False])
    )

    clean_df = df[~df["BINA_ID"].isin(conflict_bina_ids)]

    agg = clean_df.groupby("BINA_ID").agg(
        TRAFO_KODU=("BAGLANDIGI_TRAFO_KODU", "first"),
        X=("ABONE_X_KOORDINAT", "mean"),
        Y=("ABONE_Y_KOORDINAT", "mean"),
        ILCE_ID=("ABONE_ILCE_ID", "first"),
        ABONE_SAYISI=("BAGLANDIGI_TRAFO_KODU", "size"),
    ).reset_index()

    # Coklu-trafolu (conflict) binalar icin de en az bir satir uretelim:
    # en cok abonenin bagli oldugu trafoyu "en olasi" trafo olarak isaretleyip
    # lookup tablosuna dahil ediyoruz, ama conflict_df'de de rapor ediliyor
    # ki kullanici/gelistirici bu belirsizligin farkinda olsun.
    if len(conflict_bina_ids) > 0:
        fallback_rows = []
        for bina_id, group in df[df["BINA_ID"].isin(conflict_bina_ids)].groupby("BINA_ID"):
            most_common_trafo = group["BAGLANDIGI_TRAFO_KODU"].value_counts().idxmax()
            fallback_rows.append({
                "BINA_ID": bina_id,
                "TRAFO_KODU": most_common_trafo,
                "X": group["ABONE_X_KOORDINAT"].mean(),
                "Y": group["ABONE_Y_KOORDINAT"].mean(),
                "ILCE_ID": group["ABONE_ILCE_ID"].iloc[0],
                "ABONE_SAYISI": len(group),
            })
        agg = pd.concat([agg, pd.DataFrame(fallback_rows)], ignore_index=True)

    agg = agg.sort_values("BINA_ID").reset_index(drop=True)
    return agg, conflict_df


def main():
    if len(sys.argv) < 2:
        print("Kullanim: python bina_trafo_eslestirme_olustur.py <abone_dosyasi> [cikti_klasoru]")
        sys.exit(1)

    abone_path = sys.argv[1]
    out_dir = sys.argv[2] if len(sys.argv) > 2 else os.path.join(os.path.dirname(__file__), "girdi")
    os.makedirs(out_dir, exist_ok=True)

    print(f"Abone verileri okunuyor: {abone_path}")
    df = read_abone_verileri(abone_path)
    print(f"{len(df)} satir okundu, {df['BINA_ID'].nunique()} benzersiz BINA_ID.")

    lookup_df, conflict_df = build_lookup(df)

    lookup_path = os.path.join(out_dir, "bina_trafo_eslestirme.csv")
    lookup_df.to_csv(lookup_path, index=False, encoding="utf-8-sig")
    print(f"Lookup tablosu yazildi: {lookup_path} ({len(lookup_df)} bina)")

    if len(conflict_df) > 0:
        conflict_path = os.path.join(out_dir, "bina_trafo_conflict_raporu.csv")
        conflict_df.to_csv(conflict_path, index=False, encoding="utf-8-sig")
        print(f"UYARI: {conflict_df['BINA_ID'].nunique()} binada birden fazla farkli trafo kodu bulundu.")
        print(f"Detaylar: {conflict_path}")
        print("(Bu binalar icin lookup tablosunda en cok abonenin bagli oldugu trafo 'tahmini' olarak kullanildi.)")
    else:
        print("Cakisma yok: her bina tek bir trafoya bagli.")


if __name__ == "__main__":
    main()
