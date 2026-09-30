# SLF (Spatial Load Forecasting) — Mimari ve Metodoloji Dokümanı

> Bu doküman, SLF masaüstü uygulamasının (C# WinForms) kod tabanının tamamının incelenmesiyle
> oluşturulmuştur — form akışları, `.Designer.cs` buton/event bağlantıları, veri
> doğrulama pipeline'ı (VEER), tahmin algoritmaları (R/Python), GIS katmanı, veritabanı ve
> servis katmanı dahil. Amaç: hem insan geliştiricilerin hem de ileride bu projede çalışacak
> diğer AI ajanlarının uygulamanın "neyin nerede olduğunu" ve "hangi buton hangi backend'e
> gidiyor" sorusunu hızlıca cevaplayabilmesi. Mevcut kısmi doküman `docs/VEER.md` bu dosyada
> doğrulanmış ve genişletilmiştir; çelişki olduğunda bu dosya güncel kod okumasına dayanır.
>
> Not: Kod tabanının büyük kısmı Türkçe isimlendirilmiş (form/değişken/fonksiyon adları).
> Bu doküman İngilizce terimleri parantez içinde veriyor ama kod referansları orijinal
> Türkçe isimlerle veriliyor (arama/grep kolaylığı için).

---

## 1. Genel Bakış

**SLF**, elektrik dağıtım şirketleri (OEDAŞ, GDZ EDAŞ) için **mekânsal (coğrafi) yük tahmini**
yapan bir masaüstü uygulamasıdır. Uygulama iki ana metodolojiyi destekler (bkz. `MethodForm`):

1. **"SLF (Jeo-Uzamsal)"** — hücre/grid bazlı mekânsal yük tahmini: imar planları,
   nüfus/inşaat verileri, mevcut trafo/abone verileri kullanılarak gelecekteki yük
   dağılımının **coğrafi hücreler** (grid cells) bazında tahmini ve buna göre trafo yatırım
   planlaması ("Optimal DTR").
2. **"ELF (Ekonometrik)"** — klasik ekonometrik/zaman-serisi yük tahmini: makroekonomik
   göstergeler (GSYH, nüfus, HDD/CDD vb.) kullanılarak ilçe/bölge bazında toplam tüketim ve
   abone sayısı tahmini (R dilinde yazılmış istatistiksel modeller).

Uygulama; **C# WinForms (.NET Framework 4.7.2)** üzerine kurulu, **Oracle** veritabanına
bağlanan, **GMap.NET** ile harita gösteren, ağır analitik işleri **harici R ve Python
scriptlerine `cmd.exe`/`Process` üzerinden devrederek** çalıştıran bir orkestrasyon
katmanıdır. C# tarafı esasen **UI + veri girişi/doğrulama + dosya/DB I/O + script tetikleme**
işini yapar; gerçek istatistiksel/optimizasyon modelleme R ve Python'da yaşar.

### 1.1 Teknoloji yığını (özet)

| Katman | Teknoloji |
|---|---|
| UI | WinForms, Guna.UI2, MaterialSkin.2, `.Designer.cs` ile klasik WinForms designer akışı |
| Harita/GIS | **GMap.NET.WinForms** (ana harita kontrolü), **MapWinGIS** (COM, shapefile I/O), **NetTopologySuite**, **SharpKml.Core** (KML), GDAL/OGR bindings |
| Excel/CSV | **EPPlus** (asıl kullanılan — `Program.cs`'de lisans set ediliyor), ClosedXML (bazı formlarda), ExcelDataReader (bazı formlarda), CsvHelper referanslı ama **kullanılmıyor** (elle yazılmış parser var) |
| Veritabanı | **Oracle** (ODP.NET Managed, `Oracle.ManagedDataAccess`) — ana; **SQLite** — bazı modüllerin (EA, Optimal DTR) yerel sonuç depoları |
| Config | `Kod/config.json` (Newtonsoft.Json ile okunuyor, çoğu yerde) — path'ler, DB bilgileri, script yolları, yıl aralığı |
| İstatistik/Tahmin | **R** (`forecast`, `MASS::stepAIC`, `caret`, `glmnet`) — ekonometrik model; **Python** (pandas/geopandas benzeri) — imar analizi ve Optimal DTR optimizasyonu |
| Diğer | Roslyn scripting, SkiaSharp, Avalonia (muhtemelen kullanılmayan transitive bağımlılık), Microsoft.Office.Interop.Excel (COM) |

---

## 2. Uygulama Akışı (Navigasyon Haritası)

### 2.1 Önemli: Dosya adı ≠ Sınıf adı uyumsuzlukları

Kod tabanında dosya adları ile içindeki sınıf adları **örtüşmüyor**. Bu, grep/arama
yaparken kafa karıştırabilir:

| Dosya | Gerçek sınıf adı | Rol |
|---|---|---|
| `Ana_Menü.cs` / `.Designer.cs` | **`HomePageForm`** | Uygulamanın ilk açılan ekranı |
| `Modüller.cs` / `.Designer.cs` | **`ModülFormu`** | Ana çalışma ekranı (~9300 satır, harita+veri+tüm modüller) |
| `Attribute_Table.cs` | **`Tablo_Formu`** | Harita katmanı öznitelik tablosu |
| `Bekle_Formu.cs` | **`BekleForm`** | "Lütfen bekleyin" modal'ı |
| `Nokta_Yük_Bilgi_Formu.cs` | `Nokta_Yuk_Bilgi_Formu` | Nokta yük tipi referans editörü |
| `Poligon Özellik Tanımlama.cs` | `Poligon_Özellik_Tanımlama` | Poligon öznitelik girişi |
| `Fonksiyon Oluştur.cs` | `Fonksiyon_Oluştur` | **Formül editörü DEĞİL** — iki katman arası mekânsal join aracı |

### 2.2 Akış diyagramı (metinsel)

```
Program.cs (Main) — en-US culture, EPPlus lisansı, global exception handler
 └─ HomePageForm (Ana_Menü.cs)                          [İLK EKRAN — login yok]
     ├─ StartButton → MethodForm (modal)
     │    İl/İlçe seçimi (hardcoded liste: İzmir/Eskişehir/Manisa + ilçeler)
     │    Metot seçimi: "SLF (Jeo-Uzamsal)" | "ELF (Ekonometrik)"
     │    └─ ForwardButton → PathService.UpdatePath(il,ilçe) → config kaydet
     │         → ModülFormu(seçilenMetot) (modal; Home+Method gizlenir)
     │
     │         ═══ ModülFormu = ANA ÇALIŞMA EKRANI ═══
     │         ├─ SelectFolderButton → [İmar ise imarFileSelectionPopup önce] →
     │         │    girdiModülleri[tip].VEERProcess() → Önizleme (import önizleme, modal)
     │         ├─ buton_database_giris → LoginForm (modal) — Oracle auth
     │         │    (NOT: login sonrası DatabaseListForm açma kodu YORUM SATIRI, devre dışı)
     │         ├─ button_tablo_olustur → DatabaseListForm (modal)
     │         │    → Abone/DTR verisi Oracle'dan Python scriptleriyle yeniden üretme
     │         ├─ ProjeEkleButton → ProjectFolderPicker (modal, proje seç/oluştur)
     │         ├─ İmar_Grid_Oluştur → Grid_Seçenekler (modal, hücre boyutu: 100/250/400/1000)
     │         ├─ YGA_Ekle / Point_Load_Ekle / Kentsel_Donusum_Ekle → poligon çizim modu
     │         │    → Poligon_Kaydet → Poligon_Özellik_Tanımlama (modal)
     │         │         → [gerekirse] Nokta_Yuk_Bilgi_Formu (nokta yük tipi lookup)
     │         ├─ EAStationAddButton → EAStationPopupForm (modal, EV şarj istasyonu ekle)
     │         ├─ DEKCenterAddButton → DEKCenterPopupForm (modal, DEK merkezi ekle)
     │         ├─ katman_birleştir → Fonksiyon_Oluştur (mekânsal join dialog)
     │         ├─ tabloyuGör → Tablo_Formu (katman öznitelik tablosu)
     │         ├─ CreateReportButton(2) → ReportTableForm("DEK"/"EA") (modal)
     │         ├─ raporGoruntuleButonu / ExcelDownloadButton → Raporlama (export)
     │         ├─ Google_Earth_Click(_Desktop) → Google_Earth (WebView2, Google Earth Web)
     │         ├─ buton_imar_tahmini / buton_abone_sayısı_tahmini / ELFTahminButonu
     │         │    → HARİCİ Python/R SCRIPT ÇALIŞTIRMA (bkz. §4)
     │         └─ HomePageButton_Click → HomePageForm'a dön
     ├─ buton_hakkında / roundButton1 → Hakkında (About — statik metin, "PostgreSQL" hatası var, bkz §7)
     └─ roundButton2 → Yardım (Help — neredeyse boş stub)

[UI'dan erişilemeyen / yetim kod] Optimal DTR/DTR_Arayuz.cs
    Kendi CBS/GMap örneğine sahip, bağımsız/paralel bir araç. Modüller.cs veya
    Ana_Menü.cs içinde `new DTR_Arayuz()` çağrısı YOK. ModülFormu sadece kapanışta
    FormManager.Form2Instance/RaporInstance'ı dispose ediyor. Muhtemelen eski/prototip
    bir araç ya da entegrasyonu unutulmuş — kontrol edilmeli.
```

### 2.3 Form referans listesi (özet, dosya:satır ile)

| Form/Sınıf | Amaç |
|---|---|
| `LoginForm` | Gerçek Oracle kimlik doğrulama; `config.json`'daki Host/Port/Service_Name'den TNS-tarzı connection string kurar (`LoginForm.cs:154`), "Beni Hatırla" işaretliyse şifreyi **düz metin olarak config.json'a geri yazar** |
| `DatabaseListForm` | İsmi yanıltıcı — "veritabanı listesi" değil, **veri yenileme paneli**. Abone/DTR verisini Oracle'dan Python scriptleriyle (`hibrit_kod.py`, `dtr_kodu.py`) yeniden üretir |
| `ProjectFolderPicker` | Var olan projeleri (`proje_*` klasörleri) listeleyip seçtirir |
| `imarFileSelectionPopup` | İmar Verileri için CSV+KML dosya çifti seçtirir; S-eğrisi Python scriptini tetikleyebilir |
| `Grid_Seçenekler` | Sabit hücre boyutu seçimi (100/250/400/1000 m) |
| `Poligon Özellik Tanımlama.cs` | Çizilen poligona öznitelik atama (YUK/YGA/Kentsel Dönüşüm moduna göre farklı kolonlar); ELF ufuk değerleriyle çapraz kontrol yapabiliyor (`point_load_konsolidasyonu`) |
| `Nokta_Yük_Bilgi_Formu.cs` | Nokta yük tipi referans tablosu editörü (Anaokulu/AVM/Hastane vb. ~23 tip, hardcoded default) |
| `DEKCenterPopupForm` / `EAStationPopupForm` | Harita üzerine DEK/EA noktası eklerken açılan form; DEK → Excel'e, EA → **SQLite**'a yazıyor |
| `Raporlama` / `ReportTableForm` / `RaporlamaDosyası/Rapor_Arayuz.cs` | 3 ayrı, birbirinden bağımsız raporlama alt sistemi (bkz §6) |
| `Önizleme` | VEER pipeline'ının genel önizleme diyaloğu — 4 sekme (Hatalar/Düzeltilecekler/Silinecekler/Bilgiler) + çalışma tablosu |
| `Attribute_Table.cs` (`Tablo_Formu`) | Harita katmanı öznitelik tablosu; "S" butonu ile satürasyon S-eğrisi grafiği gösteriyor (`imar_analizi` Python çıktısına bağlı) |

---

## 3. Veri Girişi ve Doğrulama Pipeline'ı ("VEER")

Bu, uygulamanın en olgun ve tutarlı tasarlanmış katmanı. `docs/VEER.md` bu konuda kısmi
bir doküman içeriyor — burada doğrulanmış ve genişletilmiştir.

### 3.1 Temel sınıf: `GirdiModülü` (`girdiModülü.cs`)

Tüm veri-tipi modülleri bu sınıftan türer. Kalıtım ağacı:

```
GirdiModülü
 ├─ AboneVerileri            (Abone Verileri — subscriber data)
 ├─ DTRModulu                (DTR Verileri — dağıtım trafoları)
 ├─ DEKModulu                (DEK Verileri — dağıtık enerji kaynakları)
 ├─ EASarjModulu              (EA Şarj Verileri — EV şarj istasyonları)
 ├─ EkonometrikYukTahminiModulu (Ekonometrik Yük Tahmini Verileri — R modeline giden ham veri temizliği)
 ├─ EnerjiMusaadeleri          (Enerji Müsaadeleri — bağlantı izinleri)
 ├─ FiderVerileri              ("Fider Verileri.cs" — besleyici zaman serisi)
 ├─ YeniProjelendirilmisDTR    (planlı yeni/yükseltilmiş trafo yatırımları)
 └─ (İmar Verileri → HENÜZ ALT SINIF YOK, doğrudan `new GirdiModülü()` kullanılıyor — bkz §3.5)
```

**Dispatch sözlüğü** (`Modüller.cs:794-801`, `ModülFormu` sınıfı içinde):

```csharp
girdiModülleri = {
    "Abone Verileri"                     -> new AboneVerileri(),
    "DEK Verileri"                       -> new DEKModulu(),
    "DTR Verileri"                       -> new DTRModulu(),
    "EA Şarj Verileri"                   -> new EASarjModulu(),
    "Ekonometrik Yük Tahmini Verileri"   -> new EkonometrikYukTahminiModulu(),
    "İmar Verileri"                      -> new GirdiModülü()   // placeholder!
}
```

### 3.2 Pipeline akışı — `VEERProcess` (`girdiModülü.cs:247`)

Bu, tüm veri-tipi importlarının **tek giriş noktası**dır (`Modüller.cs`'deki
`SelectFolderButton_Click` sadece bunu çağırır, kendisi hiçbir validasyon/DB/export
mantığı içermez):

```
CheckPrerequisites(tip)          // örn. EA Şarj/DEK, "DTR Verileri" yüklenmiş olmasını ister
 → ProcessFileSelection(tip)     // Excel/CSV/DB seçimine göre uygun import metodunu çağırır
 → Preprocess()                  // (virtual) — trafo kodu eşleştirme gibi ön-temizlik
 → while (true):
     ClearRows()
     Validate()                  // (virtual) — SADECE errorDataTable/warningDataTable/
                                  //   infoDataTable'a satır yazar, hiçbir veri SİLİNMEZ/DEĞİŞTİRİLMEZ
     Önizleme dialogunu göster (4 sekme: Hatalar/Düzeltilecekler/Silinecekler/Bilgiler)
     ├─ Cancel  → return false (iptal)
     ├─ OK      → ImportProcessedData(); break   (hata/uyarı yoksa "Yükle" aktif olur)
     └─ Retry   → Remove(); ClearRows(); Validate(); Impute()   (döngü devam eder)
 → Postprocess()                 // (virtual) — bazı modüllerde override edilmemiş
 → return true
```

**Önemli tasarım kararı:** Validasyon adımı **salt-okunur**dur — hata/uyarı/bilgi
tablolarına satır yazmaktan başka bir şey yapmaz. Silme (`Remove()`) ve doldurma
(`Impute()`) sadece kullanıcı "İlerle" dediğinde, ayrı bir adımda çalışır. Bu, kullanıcının
otomatik veri manipülasyonundan önce sorunları görmesini sağlıyor.

### 3.3 Ortak alt yapı (`GirdiModülü` içinde, alt sınıflarda tekrar kullanılan)

- **4 DataTable deseni**: `currentDataTable` (çalışma verisi), `errorDataTable`,
  `warningDataTable`, `infoDataTable` — `Önizleme` formunun 4 grid'ine bağlanır.
  `columnNullRowsMap` / `imputableRowsMap` sözlükleri hangi satırların
  silineceğini/doldurulacağını takip eder.
- **Eşik (threshold) deseni**: `MAX_THRESHOLD`/`MIN_THRESHOLD` + `WarningErrorBoundary`,
  `InfoErrorBoundary`, `InfoWarningBoundary` — bir null/hata oranının yüzdesine göre hangi
  tabloya (Info/Warning/Error) yazılacağını belirler. **DTRModulu** ve **AboneVerileri**'nde
  ayrıca "%20 altı otomatik impute, üstü manuel düzeltme gerektir" deseni var
  (`imputableRowsMap` ile).
- **`IsNullLike(value)`**: `nullLikeStrings` listesine göre null-benzeri string kontrolü.
- **`RemoveCombinedRows(List<int>)`**: silinecek satır indekslerini distinct alıp,
  büyükten küçüğe sıralayarak siler (Excel/DataTable index kayması sorununu önler).

### 3.4 Modül-bazlı olgunluk durumu (önemli — backend geliştirme önceliklendirmesi için)

| Modül | Satır | Preprocess | Validate | Remove | Impute | Postprocess | Notlar |
|---|---|---|---|---|---|---|---|
| `AboneVerileri` | — | ✓ | ✓ | ✓ | ✓ | — | En olgun, referans implementasyon |
| `DTRModulu` | 1233 | — | ✓ (8 kontrol) | ✓ | ✓ (5 adım) | — | En kapsamlı ama **buglu**: `ReportNullCounts`'ta ölü kod (`isInvalid` hiç `true` olamıyor, `girdiModülü.cs` değil `DTRModulu.cs:427-429`), koordinat sınır kontrolü **devre dışı** (placeholder `float.MinValue/MaxValue`, `TODO` yorumu var, `:346-347`), `ReportTrafoLoad`'da raporlama kodu yorum satırı — sessizce impute ediyor |
| `EASarjModulu` | 690 | ✓ | ✓ | ✓ (minimal, 2 kolon) | ✓ (4 imputer) | — | En savunmacı kodlanmış (try/catch, safe parsing); ölü/tekrar kod parçaları var |
| `DEKModulu` | 390 | ✓ | ✓ | **YOK** (base class davranışı) | ✓ (4 imputer) | — | En az olgun; mod-bazlı (en sık değer) impute kullanan tek modül |
| `EnerjiMusaadeleri` | — | — | ✓ | ✓ | ✓ | — | Koordinat-bazlı en yakın trafo eşleştirme mantığı var |
| `FiderVerileri` | — | ✓ | ✓ | ✓ (sadece tarih) | **YOK** | — | DTR toplam talebiyle besleyici pikini çapraz kontrol ediyor (%50-80 aralığı) |
| `YeniProjelendirilmisDTR` | — | — | ✓ | ✓ | — | — | Bu aslında bir tahmin modeli DEĞİL — kullanıcının elle girdiği planlı yatırımları doğruluyor; `"OPTIMIZE"` sentinel değeriyle yıl kararını Optimal DTR Python optimizerına devrediyor |
| `EkonometrikYukTahminiModulu` | 633 | ✓ | ✓ | — | ✓ | — | Bu da bir tahmin modeli DEĞİL — R scriptine giden Excel'i temizliyor (bkz §4.1) |
| **İmar Verileri** | — | — | — | — | — | — | **Alt sınıfı yok**, `new GirdiModülü()` placeholder kullanılıyor; standart VEER akışının dışında, özel popup (`imarFileSelectionPopup`) ile yönetiliyor |

### 3.5 Dosya I/O katmanı

- **Excel** (`ExcelImporter.cs`, `ExcelExporter.cs`) — **EPPlus** tabanlı, gerçekten
  kullanılan tek Excel kütüphanesi. `ExcelImporter`'ın **iki farklı** header-map'i var
  (`ImportExcelFile` vs `ImportExcelFileAsync`) ve bunlar **CsvHandler'ın header-map'inden
  de sapmış** — örn. Ekonometrik kolon isimleri (`GDP_BUYUME_ORANI`/`ILCE_NUFUS` vs
  `GDP_GROWTH`/`ULKE_NUFUS`) farklı. **Bu, yeni bir veri tipi eklerken veya format
  değişikliğinde dikkat edilmesi gereken bir teknik borç.**
- **CSV** (`CsvHandler.cs`) — elle yazılmış parser (CsvHelper paketi referanslı ama
  kullanılmıyor). **`girdiModülü.cs:807-811`'deki `ProcessCsvFile` bir TODO stub'ı —
  boş `DataTable` döndürüyor.** Yani CSV importu, `GirdiModülü`'nün standart
  `VEERProcess` akışına **bağlı değil**; `CsvHandler` sadece `Modüller.cs`'de proje
  yeniden açılışında son-yüklenen veriyi geri yüklemek için (`LoadModuleData`)
  kullanılıyor, validasyondan geçmeden.
- **Oracle** (`ProcesssqlFile` → `DatabaseHelper.LoadTable`) — `SELECT * FROM {tableName}`,
  kolon adları upper-case'e çevriliyor. Hangi mantıksal veri tiplerinin DB'den
  yüklenebileceği `veri_listesi_requires_database` sözlüğünde sabit (girdiModülü.cs:56-64).

### 3.6 Önizleme diyaloğu (`Önizleme.cs`)

Pasif bir form — 5 sekme (`Onizleme_Onizleme`/çalışma verisi, `Hata`, `Warning`,
`Information`, `Statistics`), her biri bir `DataGridView`. Kullanıcı satır-bazlı manuel
seçim/silme **yapamaz** — silme/doldurma tamamen otomatik, alt sınıfın `Report*`
metodlarının doldurduğu `columnNullRowsMap`/`imputableRowsMap` üzerinden yürür. 3 buton:
`Buton_YUKLE` (OK, commit), `Buton_İLERLE` (Retry, tekrar validate/impute döngüsü),
`Buton_ÇIK` (Cancel, onaylı çıkış).

---

## 4. Tahmin/Analiz Algoritma Katmanı

**Kritik bulgu:** İsimlerine rağmen `EkonometrikYukTahminiModulu.cs`, `YeniProjelendirilmisDTR.cs`
ve `Fonksiyon Oluştur.cs` gibi C# sınıfları **gerçek tahmin algoritmaları içermez** —
bunlar veri hazırlama/doğrulama/UI katmanıdır. Asıl istatistiksel/optimizasyon mantığı
**R** (`Kod/ELF/`) ve **Python** (`Kod/Optimal DTR/`, `Kod/imar/python_kod/`) dosyalarında
yaşıyor ve C# tarafından `Process`/`cmd.exe` ile harici olarak tetikleniyor.

### 4.1 ELF — Ekonometrik Yük Tahmini (`Kod/ELF/senaryolar.R`, `Kod/ELF/model.R`)

Tetikleme: `girdiModülü.cs:664-705` (`RunRScriptSenaryolar`) ve `Modüller.cs:8022-8039`
(`RunModelRScript`) → `Rscript.exe --vanilla "<script>" "<config_path>"`.

**`senaryolar.R`** (546 satır) — 5 makroekonomik **senaryo** üretir (minimum/düşük/baz/
yüksek/maximum):
- GSYH büyüme oranı: son bilinen değer + sabit ofset (min: −0.02, düşük: −0.01, baz: 0,
  yüksek: +0.01, maximum: +0.02) — istatistiksel dağılım değil, **sabit ek/çıkarma bandı**.
- KKO (kayıp-kaçak oranı): 7 yıllık hareketli ortalama ile impute ediliyor, tüm
  senaryolarda **aynı** (senaryoya göre değişmiyor).
- Sektörel GSYH/GRP payları, CDD/HDD, ilçe nüfusu: `forecast::ets(damped=TRUE)`
  (sönümlü üstel düzeltme) ile tahmin ediliyor; **5 senaryo, bu ETS modelinin
  %-güven aralığı alt/üst sınırlarından türetiliyor** (bağımsız ekonometrik senaryolar
  değil).
- Çıktı: aynı Excel dosyasının (`VERİLER.xlsx`) 2-6. sayfalarına yazılıyor
  (`senaryolar_minimum...maximum`), `model.R` bunları girdi olarak okuyor.

**`model.R`** (1957 satır, ~1190'a kadar detaylı okundu) — asıl ekonometrik tüketim/abone
tahmin modeli, her segment için (**Mesken, Sanayi, Ticarethane, Sulama, Aydınlatma** ×
{tüketim, abone}):
1. İki eğitim seti kurulur: laglı ve lagsız öngörücüler.
2. `best_regression_model`: **BIC-cezalı stepwise lineer regresyon** (`MASS::stepAIC`,
   `k=log(n)`), segment-özel öngörücü kümeleri, **VIF>5 ile çoklu-doğrusallık ayıklama**,
   50×tekrarlı 3-katlı CV ile 2013 vs 2014 veri seti karşılaştırması (yüksek adj-R² kazanır).
   `n<10` ise **Elastic Net** (`glmnet`) yedek modele düşülüyor.
3. `best_time_series_model`: **ETS, ARIMA (auto.arima), Bagged-ETS** — üçü de ileri-doğru
   (expanding-window) tek-adım-ilerisi RMSE ile karşılaştırılıyor, kazanan seçiliyor.
4. `best_model_among_all`: regresyon vs zaman-serisi modelini **in-sample MAPE** ile
   karşılaştırıp final modeli seçiyor (Sulama segmenti her zaman regresyona sabitlenmiş).
5. `step_by_step_forecast`: **otoregresif rollout** — her yılın tahmini bir sonraki yılın
   lag öngörücüsü olarak geri besleniyor.
6. **Takip önerisi**: `model.R`'ın 1190-1957 satırları (final Excel yazma aşaması) ve
   `hor_ver.R` (ufuk/dikey hesap scripti, `Modüller.cs:~9153`'te çağrılıyor) henüz detaylı
   okunmadı — bu proje için backend geliştirme öncesi tam okunmalı.

**Metodoloji özeti**: Segment-bazlı **{BIC-stepwise regresyon, Elastic Net, ETS, ARIMA,
Bagged-ETS} arasından en iyisini seçen** bir ansambl, 5 makro senaryo (ETS güven
aralıklarından türetilmiş) altında koşuluyor.

### 4.2 Optimal DTR — Trafo Yatırım Optimizasyonu (`Optimal DTR/` [C#] + `Kod/Optimal DTR/` [Python])

C# tarafı (`Optimal DTR/DTR_Arayuz.cs`, 1569 satır) **UI'dan erişilemiyor** (bkz §2.2) ama
kod olarak mevcut ve tam işlevli görünüyor:
- `algoritmaÇalıştır.py`'yi `python "<script>" "<ODTRJson>"` ile çalıştırıyor.
- `ReadDB.cs` sonuç SQLite DB'sini (`veriler.db`) okuyor (`Trafo`, `Hucre` listeleri).
- `Ogeler.cs` — **boş stub** (sadece `InitializeComponent()`), kullanılmıyor.

Python tarafı (`Kod/Optimal DTR/`, ~10.4k satır toplam):
- `AlgoritmaÇalıştır.py` (1378 satır) — orkestratör; 3 alt-algoritma çağırıyor:
  - `KurumTrafoKismi` → nokta yük (kurumsal) trafo ataması
  - `KirsalAlanKismi` → kırsal alan trafo yerleşimi (`kent_disi_alan==1` hücrelerle sınırlı)
  - `EAKismi` → **"EA" = Elektrikli Araç (EV) şarj yükü**, "Evrimsel Algoritma" DEĞİL.
    `EA.py` sınıfı: komşu hücre bulma, kümülatif yük toplamı eşik aşınca yeni trafo ekleme
    (`cumsum_from_first_positive_and_reset_on_negative`).
- `SuperHucreAlgoritma12.py` (537 satır) — hücre kümeleme/gruplama ("super hücre").
- `Functions3.py` (3028 satır, **en büyük dosya, detaylı okunmadı**) — paylaşılan
  yardımcı kütüphane (SQLite/Parquet okuma, muhtemelen geometri/kapasite hesapları).
- Çıktı: `sonuclar/Optimal DTR Sonuçları` altına Excel/HTML/DB.

**Net etki**: Yıllık hücre-bazlı yük tahminleri (SLF/ELF çıktısı), nokta yükler, kırsal
alan kısıtları ve EV şarj yükü büyümesi verildiğinde, **nerede ve ne zaman** yeni/yükseltilmiş
trafo yatırımı yapılması gerektiğine karar veren bir mekânsal kapasite planlama optimizeri.

### 4.3 İmar Analizi (`Kod/imar/python_kod/imar_analizi/`)

`main.py` (`process_command`) tarafından orkestre edilen zincir:
1. Overpass API'den imar planı verisi çekme/doğrulama (`imar_overpass`/`imar_datalar`/`imar_check`).
2. `imar_tipleri_analizi_v2.py` — bina noktalarını (mesken/ticarethane) imar planı
   geometrilerine eşleştirme (buffer-tabanlı grid matching).
3. `test_imar.py` / `bina_kirilimlari.py` — hücre bazlı imar/bina kırılımları.
4. **`saturasyon_analiz.py`** — **satürasyon modeli**: TAKS/KAKS katsayıları, net
   yapılaşabilir alan oranı, "gerçek kullanım oranı" ve düzeltilmiş TAKS oranı hesaplayarak
   bir bölgenin yasal yapı hakkının ne kadarının zaten inşa edildiğini (→ gelecekteki yeni
   inşaat/abone büyümesi tahmini için) modelliyor. Çıktı `config.json → SLF.Saturasyon_Dosyası`.
5. `trafo_merkez_hucre_v2.py` / `trafo_rezerv_alanlar_analiz.py` — hücre↔trafo eşleştirme
   (Optimal DTR'ye besleniyor).
6. `hucre_abone_as_is.py` — mevcut durum ("as-is") abone kırılımı.

**Tetikleme**: `Services/PythonHelper.cs` — `RunDeepLearningModel`, `RunKatmanDeneme`,
`RunImarPlanModel` (ana giriş noktası), hepsi `cmd.exe /K python ...` ile.

**⚠️ Sistemik güvenilirlik sorunu**: `PythonHelper.cs`'deki **her** Python çağrısında
`process.WaitForExit()` / exit-code kontrolü **yorum satırı yapılmış** (devre dışı). C#
tarafı, script'i başlatıp **hemen geri dönüyor**, başarılı olup olmadığını hiç
doğrulamıyor. Bu, backend geliştirme sırasında ilk düzeltilmesi gereken şeylerden biri
olmalı — sessiz hatalar kullanıcıya hiç yansımıyor.

Not: `Kod/imar/python_kod/imar_analizi/` klasörü kendi içinde **ayrı bir git deposu**
(`.git/` var) — muhtemelen kazara nested repo, kontrol edilmeli.

### 4.4 Dış process çağrı envanteri

| Dosya:Satır | Çalıştırdığı | Mekanizma |
|---|---|---|
| `girdiModülü.cs:664-705` | `ELF/senaryolar.R` | `Rscript --vanilla`, çıktı yakalanıyor |
| `Modüller.cs:8022-8039` | `ELF/model.R` | `Rscript --vanilla`, çıktı yakalanıyor |
| `Modüller.cs:~9153` | muhtemelen `ELF/hor_ver.R` | `Rscript` (tam okunmadı) |
| `Optimal DTR/DTR_Arayuz.cs:518-524,849-860` | `algoritmaÇalıştır.py` | `python`, çıktı yakalanıyor |
| `Services/PythonHelper.cs` (çoğu metod) | çeşitli imar/DB Python scriptleri | `cmd.exe /K python`, **wait/exit-check devre dışı** |

### 4.5 `Kod/config.json` — script/path yapılandırması

Ana bloklar: `ELF` (R script yolları + `ufuk_yılı`=11), `Veritabanı` (Oracle bağlantısı —
**düz metin şifre içeriyor**, bkz §7), `Python Kodları` (script yolları), `DEK`/`EA`
(senaryo dosyaları), `İmar Analizi`, `SLF` (`secilen_senaryo: "minimum"` — **hardcoded**,
dikkat), `ODTR` (Optimal DTR path'leri), `Abone_verileri` (DB satır sayısı cache'i).
`Kod/Kod_YENİ.rar` (52 MB) deposunda mevcut ama içeriği incelenmedi — ne olduğu
belirsiz, muhtemelen eski/yedek kod arşivi; kontrol edilmeli.

---

## 5. GIS / Harita Katmanı

**Çekirdek sınıf: `CBS`** (`CBS.cs`, 2791 satır, plain class — Form değil; harita UI'ı
`ModülFormu` içinde). "CBS" = Coğrafi Bilgi Sistemi.

- **Harita motoru**: **GMap.NET** (`GMapControl`, `GMapOverlay`, `GMapPolygon`,
  `GMarkerGoogle`). **MapWinGIS** (COM) sadece shapefile import/export için. **NetTopologySuite**
  geometri işlemleri (kesişim, grid üretimi, WKT). **SharpKml.Core** KML export/import.
- **`Google_Earth.cs`** gerçek bir entegrasyon değil — sabit kamera pozisyonlu
  `earth.google.com/web` adresine yönlendirilmiş bir **WebView2** browser'ı.
- **Katman modeli**: 50 slot'luk paralel dizi çifti (`tüm_katmanlar_array_imar`/`_yuk`),
  her tab için ayrı `GMapControl`. `CopyOverlayContents` içinde **explicit OutOfMemoryException
  yakalama/retry/GC.Collect döngüsü var** — büyük shapefile/KML importlarında gerçek
  bellek sorunları yaşandığının güçlü işareti.
- **Yetenekler**: shapefile/KML import, cetvel (mesafe ölçümü), poligon seçim/highlight,
  sabit boyutlu grid oluşturma, poligon çizimi, iki katman arası mekânsal join +
  toplulaştırma (`JoinAttributesByLocation`/`_summary` — count/sum/min/max), sabit
  eşiklerle ısı haritası (heatmap), `İMAR_SONUÇLAR.kml` için özel lejant render'ı
  (hardcoded dosya adı kontrolü).
- **`CBS` sadece 2 yerde instantiate ediliyor**: `Modüller.cs:254` (ana harita) ve
  `Poligon Özellik Tanımlama.cs:52` (**aynı `ModülFormu` referansını sarmalıyor** —
  yorum satırında yeni bir `ModülFormu` oluşturmamak gerektiği açıkça uyarılmış, COM
  bileşenlerinin istemsiz yeniden başlatılmasını önlemek için).

**`Poligon Özellik Tanımlama.cs`** poligon tipine göre farklı kolon setleri sunuyor: YUK
(nokta yük — Tipi/Alan/Tüketim Sınıfı/Kurulu Güç/Pik Yüklenme/Pik Demant, `point_load_musaade.xlsx`'ten
dropdown), YGA/Kentsel Dönüşüm (bina tipi yüzdeleri, satürasyon hızı, TAKS). Poligon
kaydedilirken, `point_load_konsolidasyonu` açıksa, birikimli düşüşleri statik bir
sözlükte (`AdjustedHorizontalValues`) takip ederek **ELF ufuk tahminini aşmayacak şekilde**
çapraz kontrol yapıyor — GIS katmanı ile ekonometrik tahmin katmanı arasındaki tek doğrudan
bağlantı noktalarından biri.

---

## 6. Raporlama Katmanı

**Üç bağımsız raporlama alt sistemi var** (birleştirilmemiş, tutarsız):

1. **`Raporlama.cs`** — genel VEER validasyon raporu; 4 grid'i (Hata/Warning/Info/Stat)
   tek bir çok-sayfalı `.xlsx`'e export eder (`ExcelExporter.ExportExcelFileWithMultipleSheets`).
2. **`ReportTableForm.cs`** — EA/DEK sonuç dosyalarının yıl-bazlı görüntüleyicisi/exporter'ı
   (`config.EA.cikti_dosyasi_xlsx` / `config.DEK.cikti_dosyasi`).
3. **`RaporlamaDosyası/Rapor_Arayuz.cs`** — en büyük (1000+ satır), **Optimal DTR**
   sonuçlarına özel dashboard (DTR/EA/Yük/DEK verilerini haritada gösteriyor, SQLite sonuç
   DB'si okuyor). **Kendi config yükleme mekanizmasını kullanıyor**
   (`Microsoft.Extensions.Configuration.ConfigurationBuilder`) — diğer her yerdeki
   `Newtonsoft.Json.Linq.JObject` yaklaşımından farklı. İçinde başka bir geliştiricinin
   (`vural.bayrakli`) OneDrive yoluna hardcoded (yorum satırı) referans var — config
   path kırılganlığının kanıtı.

Tüm raporlama çıktıları **sadece Excel (.xlsx)** — PDF/print pipeline yok.

---

## 7. Veritabanı ve Servis Katmanı

### 7.1 Veritabanı

**Oracle** (ODP.NET Managed Driver, `Oracle.ManagedDataAccess`) — ana/prodüksiyon
veritabanı. `Services/DatabaseManager.cs` (`OracleConnection` sarmalayan thread-safe
singleton), `Services/DatabaseHelpers.cs` (`ExecuteQuery`/`ExecuteNonQuery`/`TableExists`/
`LoadTable` — `LoadTable`, `girdiModülü.cs`'in Oracle-tabanlı import yolunun kullandığı
metod).

Bilinen tablolar: `DWH_MRC_SLFPROJE_TUKETIM` (abone tüketim), `DWH_MRC_SLFPROJE_ABN_BLG`
(abone bilgi), sorgular `ALL_TABLES` sistem görünümünü de kullanıyor.

**SQLite** — ikincil, bazı modüllerin yerel sonuç depoları için (`EAStationPopupForm` →
`sonuclar2.db`, Optimal DTR → `veriler.db`, `Rapor_Arayuz` → sonuç DB'leri). `DatabaseManager`/
`DatabaseHelper` soyutlamasının **dışında**, doğrudan `System.Data.SQLite` ile.

**⚠️ KRİTİK GÜVENLİK SORUNU**: `Kod/config.json` içinde **düz metin Oracle şifresi ve
gerçek görünen internal hostname/IP** commit edilmiş durumda:
```
"Server_Address": "admdc-scan.aydem.corp", "DSN": "10.27.130.189:1521/XE",
"Username": "ehan0", "Password": "12345"
```
`git status`'ta bu dosya "modified" (tracked) görünüyor. Ayrıca `LoginForm.cs`
("Beni Hatırla" seçiliyse) kullanıcının girdiği şifreyi **aynı dosyaya geri yazıyor**.
**Backend çalışmasına başlamadan önce**: (a) bu dosyayı `.gitignore`'a alıp git
geçmişinden temizlemek, (b) bağlantı bilgilerini bir secrets store/ortam değişkenine
taşımak, (c) `LoginForm`'un şifreyi düz metin kaydetmesini durdurmak önerilir.

### 7.2 Servisler (`Services/`, `Utilities/`)

| Servis | Amaç |
|---|---|
| `DatabaseManager` | Oracle connection singleton |
| `DatabaseHelpers` | Query execution wrapper (parametreli SELECT/UPDATE, `LoadTable`) |
| `ConfigService` | `config.json`'dan sadece Abone satır sayıları + DB tablo isimleri okuyor/yazıyor — config'in geri kalanı ad-hoc okunuyor (merkezi tip-güvenli erişim yok) |
| `PathService` | En büyük/en çok kullanılan servis — tüm dosya yollarının (Python script'leri, CBS dosyaları, proje klasörleri) merkezi kaynağı; `config.json`'u `SetConfigPath`'te parse ediyor |
| `YearServices` (`YearService`) | Tahmin ufku (`slfStartYear`/`slfEndYear`, `LastYear`/`PenultimateYear`/`HorizonYear`) singleton'ı, `OnYearChanged` event'i |
| `PythonHelper` | Python script tetikleme (bkz §4.3-4.4) — **wait/exit-check'siz, fire-and-forget** |
| `DataValidationService` | Cache'lenmiş CSV satır sayılarını canlı Oracle sayılarıyla karşılaştırıp veri bayatlığını (staleness) tespit ediyor |
| `ControlExtensions` | `Control.InvokeAsync` — UI thread'e async iş gönderme yardımcı fonksiyonu |
| `Utilities/DialogHelpers` | `OpenFileDialog`'un "boş/beyaz diyalog" sorunu için 3-deneme retry wrapper'ı |

### 7.3 Bağımlılık yığını (özet, `packages.config`'ten)

GIS/Geometri: GMap.NET.* 2.1.7, NetTopologySuite 2.5.0, GDAL/OGR 3.8.5, SharpKml.Core 6.1.0,
SharpMap 1.2.0. Excel/Office: EPPlus 7.1.2 (asıl kullanılan), ClosedXML 0.104.2,
ExcelDataReader 3.7.0, Microsoft.Office.Interop.Excel (COM). DB: Oracle.ManagedDataAccess
23.8.0, System.Data.SQLite 1.0.119.0, **Npgsql 8.0.3 (referanslı ama kullanılmıyor —
muhtemelen eski PostgreSQL planının kalıntısı)**, EntityFramework 6.4.4 (kullanılmıyor
gibi görünüyor). UI: Guna.UI2.WinForms, MaterialSkin.2, WebView2. **Avalonia 11.0.10**
(tam yığın) — bu WinForms uygulamasında kullanılmıyor gibi görünüyor, muhtemelen
transitive bağımlılık; denetim yüzeyini azaltmak için kaldırılabilir mi kontrol edilmeli.
LINQPad 5.46.0 — geliştirme aracı, paket referansı olarak garip.

---

## 8. Bilinen Sorunlar / Riskler / TODO Listesi

Backend algoritma çalışmasına başlamadan önce bilinmesi gereken, kodda tespit edilmiş
somut sorunlar:

1. **🔴 Güvenlik — düz metin Oracle şifresi + gerçek host/IP** `Kod/config.json`'da
   commit edilmiş (bkz §7.1). `LoginForm` "Beni Hatırla" ile şifreyi geri yazıyor.
2. **🔴 `PythonHelper.cs`'deki TÜM Python çağrılarında `WaitForExit()`/exit-code kontrolü
   devre dışı** — script hataları sessizce yutuluyor, kullanıcıya hiç yansımıyor (§4.3).
3. **🟠 `DTRModulu.cs` — koordinat sınır kontrolü devre dışı** (`minMaxCheckMap` hâlâ
   `float.MinValue/MaxValue` placeholder, `TODO` yorumu var, satır ~346-347) — bu kontrol
   şu an **hiçbir zaman** hata üretmiyor.
4. **🟠 `DTRModulu.ReportNullCounts` içinde ölü/kırık kod** — `isInvalid` değişkeni asla
   `true` olamıyor (satır ~427-429), bu döngü etkisiz.
5. **🟠 `DTRModulu.ReportTrafoLoad`'da raporlama bloğu yorum satırı** — aşırı yük tespiti
   sessizce impute ediyor, kullanıcıya uyarı gösterilmiyor.
6. **🟠 CSV import pipeline'ı fiilen stub** — `girdiModülü.cs`'deki `ProcessCsvFile` boş
   `DataTable` döndürüyor; CSV import'u `VEERProcess`'e bağlı değil.
7. **🟠 Excel import header map'leri (ExcelImporter vs CsvHandler) birbirinden sapmış** —
   aynı veri tipi için farklı beklenen kolon isimleri (özellikle Ekonometrik Yük Tahmini
   verisi). Yeni veri tipi eklerken veya kolon isimlerini değiştirirken bu iki dosyayı
   birlikte güncellemek gerekiyor.
8. **🟠 `Hakkında.cs` (About) "Veritabanı: PostgreSQL" diyor** — gerçek implementasyon
   %100 Oracle. Dokümantasyon/UI tutarsızlığı, düzeltilmeli.
9. **🟡 "İmar Verileri" hâlâ `GirdiModülü` placeholder** — kendi alt sınıfı yok,
   standart VEER akışının dışında özel bir popup ile yönetiliyor.
10. **🟡 `DEKModulu`'nda `Remove()` override'ı yok** — DEK satırları sadece base class'ın
    varsayılan (muhtemelen no-op) davranışıyla "siliniyor".
11. **🟡 `Optimal DTR/DTR_Arayuz.cs` ve tüm "Optimal DTR" C# aracı, güncel UI akışından
    erişilemiyor** — kod var ve işlevli görünüyor ama hiçbir buton onu açmıyor. Ya bilinçli
    olarak devre dışı bırakılmış ya da entegrasyonu tamamlanmamış; ekiple netleştirilmeli.
12. **🟡 `CBS.cs`'te büyük shapefile/KML importlarında `OutOfMemoryException`
    yakala/retry/GC.Collect deseni** — büyük dosyalarda gerçek bellek sorunu yaşandığının
    işareti; kalıcı bir streaming/virtualization çözümü düşünülmeli.
13. **🟡 İki farklı config-yükleme mekanizması aynı `config.json` için**
    (`Newtonsoft.Json.Linq.JObject` çoğu yerde, `Microsoft.Extensions.Configuration` sadece
    `Rapor_Arayuz.cs`'de).
14. **🟡 Makineye özel absolute path'ler** — `config.json`'daki `program_dosyaları_path`,
    `CBS.cs:259`'daki varsayılan dialog dizini, `Rapor_Arayuz.cs`'deki yorum satırı yapılmış
    başka bir geliştiricinin OneDrive yolu — başka bir makinede/kullanıcıda değişiklik
    yapılmadan çalışmaz.
15. **🟡 `Kod/imar/python_kod/imar_analizi/` kendi içinde ayrı bir git deposu** — kazara
    nested repo olabilir, kontrol edilmeli.
16. **🟡 `GirdiModülü()` constructor'ı her instantiate edildiğinde yeni bir `ModülFormu`
    VE yeni bir `HomePageForm` oluşturuyor** (`girdiModülü.cs:400-401`) — her veri-tipi
    modülü nesnesi (6+ tane, `Modüller.cs`'deki dispatch sözlüğünde) gereksiz yere tam
    birer Form ağacı yaratıyor; bellek/performans açısından incelenmeye değer.
17. **⚪ `Kod/Kod_YENİ.rar`** (52 MB) — içeriği incelenmedi, ne olduğu belirsiz.
18. `Kod/ELF/model.R`'ın son ~770 satırı (final Excel yazma + `hor_ver.R` etkileşimi) ve
    `Kod/Optimal DTR/Functions3.py` (3028 satır, en büyük dosya) detaylı okunmadı —
    backend çalışmasına başlamadan önce bu ikisi tam okunmalı.

---

## 9. Terim Sözlüğü (Türkçe → İngilizce/Açıklama)

| Terim | Açıklama |
|---|---|
| VEER | Validate-Error-Warning-Remove(?) — veri doğrulama/temizleme pipeline'ının kod içi adı |
| CBS | Coğrafi Bilgi Sistemi = GIS |
| DTR | Dağıtım Trafosu = Distribution Transformer |
| DEK | Dağıtık Enerji Kaynağı = Distributed Energy Resource |
| EA | Elektrikli Araç (şarj) = Electric Vehicle (charging) — **"Evrimsel Algoritma" DEĞİL** |
| ELF | Ekonometrik Yük Tahmini (R modeli) |
| SLF | Spatial Load Forecasting / Jeo-Uzamsal Yük Tahmini (uygulamanın kendisi + metodolojilerden biri) |
| Fider | Feeder (besleyici hat) |
| İmar | Zoning/urban development plan |
| Satürasyon | Bir bölgenin yasal yapı hakkının ne kadarının inşa edildiği oranı |
| TAKS/KAKS | Taban Alanı Katsayısı / Kat Alanı Katsayısı — imar yoğunluk katsayıları |
| YGA | (bağlamdan) Yüksek Gerilim Alanı veya benzeri bina-tipi karışım poligonu |
| Kentsel Dönüşüm | Urban transformation/redevelopment |
| Abone | Subscriber |
| Trafo | Transformer |
| KKO / KKM | Kayıp-Kaçak Oranı / Kayıp-Kaçak Miktarı — network loss ratio/amount |
| ODTR | Optimal DTR (trafo yatırım optimizasyon modülü) |

---

## 10. Backend Algoritma Geliştirmesi İçin Öneriler

Kullanıcı ile birlikte backend tarafında algoritma geliştirmeye başlarken:

1. **Önce §8'deki kırmızı/turuncu maddeleri triage edin** — özellikle DTR koordinat
   kontrolü ve Python exit-code kontrolünün devre dışı olması, yanlış negatif
   sonuçlara yol açabilir.
2. **`model.R`'ın kalan kısmını ve `hor_ver.R`'ı tam okuyun** — ekonometrik modelin
   nihai çıktı biçimini (hangi Excel sayfaları, hangi format) anlamadan bu katmana
   dokunmak riskli.
3. **`Functions3.py`'ı (Optimal DTR'nin en büyük dosyası) tam okuyun** — muhtemelen
   temel geometri/kapasite hesap mantığı burada.
4. **"Optimal DTR" C# aracının neden UI'dan erişilemez olduğunu ekiple netleştirin** —
   yeniden entegre mi edilecek, yoksa kaldırılacak mı?
5. Yeni bir istatistiksel/optimizasyon yaklaşımı eklerken, mevcut desenle tutarlı olun:
   **C# = orkestrasyon/UI/veri hazırlama, R/Python = model** — bu ayrımı bozmayın,
   aksi halde WinForms UI thread'i ağır hesaplamalarla kilitlenir.
