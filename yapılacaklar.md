# SLF — Yapılacaklar / Geliştirme Yol Haritası

> Bu dosya, backend (Python/R) algoritma tarafında yapılan, yapılmakta olan ve planlanan
> geliştirmelerin takip listesidir. `methodology.md` mevcut sistemin nasıl çalıştığını
> anlatır; bu dosya ise **neyi değiştirdik / değiştireceğiz**i takip eder.

---

## ✅ Tamamlanan (2026-09 oturumu)

- **DEK/EA koordinat sırası düzeltmesi** — `DEKCenterPopupForm.cs` ve `EAStationPopupForm.cs`,
  haritadan tıklayarak eklenen tekil noktalarda enlem/boylamı diğer modüllerin tersi sırayla
  kaydediyordu (X=enlem/Y=boylam yerine olması gereken X=boylam/Y=enlem). Her iki formda
  düzeltildi.
- **Bina → trafo statik eşleştirme tablosu** (`Kod/DEK/bina_trafo_eslestirme_olustur.py`) —
  Abone Verileri'nden (BINA_ID + BAGLANDIGI_TRAFO_KODU) her binanın bağlı olduğu trafoyu
  gösteren bir lookup tablosu üretiyor. Test verisinde 23.756 bina, sıfır çelişki.
- **PVGIS entegrasyonu** (`Kod/DEK/pvgis_uretim_tahmini.py`) — tek bir DEK/rooftop noktası
  için PVGIS API'sinden 5 yıllık ortalama, "tipik yıl" yıllık + saatlik (8760 satır) üretim
  tahmini çekiyor. Eski versiyonda (dek.py) hiç fiziksel/coğrafi üretim hesabı yoktu, sadece
  senaryo-bazlı kapasite dağıtımı vardı.
- **Solar irradiance harita katmanı** (`Kod/DEK/solar_atlas_grid_indirici.py`) — Global Solar
  Atlas nokta-sorgu API'sinden Karşıyaka için 250m çözünürlükte 2200 noktalık ışınım grid'i
  indirildi (`Kod/DEK/girdi/solar_irradiance_karsiyaka.csv`), CBS.cs'in mevcut heatmap
  altyapısına bağlanmaya hazır.
- **Trafo bazlı DEK-pik etki analizi** (`Kod/DEK/dek_trafo_etki_hesabi.py`) — trafo başına
  ticari/sanayi tüketim oranını hesaplayıp, bağlı DEK'lerin üretimini bu oranla
  ağırlıklandırarak trafonun pikini düşürüyor. **Bu, aşağıdaki "ana metodoloji" maddesinin
  ilk/basit versiyonu.**

## 🔧 Bilinen ama henüz düzeltilmemiş hata

- **`dek.py`'de Monte Carlo etkisiz** — `apply_methodology_with_roof_areas` fonksiyonu
  `randomized_weights` parametresini alıyor ama hiç kullanmıyor; `calculate_roof_areas` da
  zaten deterministik. Yani 300 "simülasyon" birebir aynı sonucu üretiyor, ortalama almanın
  hiçbir etkisi yok — pratikte ölü kod. Ayrıca dağıtım, imar tipine göre (mesken/sanayi/
  ticarethane) hiç ayrım yapmadan sadece çatı alanı büyüklüğüne göre yapılıyor.
  **Öncelik: düşük** (şu an nokta-bazlı yaklaşıma odaklanıyoruz, dek.py'nin senaryo-seviyesi
  top-down modeli ayrı bir konu).

---

## 📋 Planlanan Geliştirmeler

### Ana go-to metodoloji (şu anki ve gelecekteki temel yaklaşım)
- Bir DEK noktası eklendiğinde/değerlendirildiğinde, bağlı olduğu trafo, o trafodaki abone
  karışımına göre (Sanayi/Ticarethane payı yüksek mi, Mesken ağırlıklı mı) etiketlenir.
- Trafo sanayi/ticarethane ağırlıklıysa, bağlı DEK'lerin (ağırlıklandırılmış) yıllık/saatlik
  üretimi o trafonun pik yükünden düşülür — çünkü güneş üretimi öğle saatlerinde olur ve bu
  ancak gün-ortası piki olan ticari/sanayi trafolarında gerçek bir düşürücü etki yaratır.
  Mesken ağırlıklı trafolarda (pik akşam saatlerinde olduğu için) bu etki uygulanmaz/azaltılır.
- Bu üretim düşümü sadece trafo seviyesinde değil, **DEK'in bağlı olduğu bina üzerinden
  ilgili hücrelerden (cell) de düşülür** — Uzamsal Yük Tahmini (SLF) modülü yükü zaten hücre
  bazında hesaplayıp trafolara oradan dağıtıyor; DEK üretimi de aynı hücre seviyesinde
  netleştirilerek bu akışa doğal şekilde dahil edilir.
- Bu şekilde düşürülmüş hücre/trafo yükü, **Optimal DTR'nin trafo konumlandırma
  (kapasite artırma / yeni trafo tesisi) kararlarında da doğrudan kullanılacak** — Optimal DTR
  zaten hücre→trafo atamasında bina-trafo bağlantısallığını kullanıyor; aynı bağlantısallık,
  DEK üretiminin hangi trafonun yükünü hangi oranda düşürdüğünü belirlemek için de
  kullanılacak. Yani DEK'in pik-düşürücü etkisi, trafo yatırım zamanlamasını/gerekliliğini
  doğrudan etkileyebilecek şekilde tasarlanıyor.
- **Bu mantık zaten `dek_trafo_etki_hesabi.py`'de var** (oransal ağırlıklandırmayla, şu an
  sadece trafo seviyesinde); aşağıdaki OSOS/saatlik ve hücre-seviyesi/Optimal DTR entegrasyonu
  geliştirmeleri bunun üzerine inşa edilecek, temel mantığı değiştirmeyecek.

### OSOS verisiyle gerçek saatlik netleştirme
- Mevcut yaklaşım (yıllık üretim/8760×2,5 sabit katsayı) bir **yaklaşıklık**. Gerçek OSOS
  (saatlik tüketim/sayaç) verisi varsa, her trafo için kendi gerçek saatlik yük eğrisi
  kullanılabilir.
- Bu durumda: DEK'in PVGIS'ten gelen saatlik üretim profili, trafonun OSOS'tan gelen saatlik
  yük eğrisinden **saat saat** düşülür (yıllık toplam yerine gerçek zaman-eşleşmeli netleştirme).
- Bu, "sanayi/ticarethane trafosu → düşürücü etki var" varsayımını veriye dayalı, gerçek bir
  hesaba çevirir; artık oransal ağırlıklandırmaya (tahmine) gerek kalmaz.
- **Durum**: OSOS verisinin bu proje için erişilebilirliği/formatı netleşince uygulanacak,
  şu an tasarım aşamasında.

### Optimal DTR — mevcut alanlarda gerçek AG bağlantısallığının kullanılması
- **Doğrulandı (kod incelemesiyle)**: Optimal DTR şu an mevcut/dolu alanlar ile yeni gelişen
  alanlar arasında hiç ayrım yapmıyor — ikisine de aynı, tamamen geometrik "süper hücre"
  kümeleme mantığını uyguluyor. Bir trafo, sadece fiziksel konumunun hangi hücreye düştüğüne
  bakılarak bir süper hücreye "ait" sayılıyor; gerçek abone-trafo bağlantısallığı
  (`BAGLANDIGI_TRAFO_KODU`) bu atamada hiç kullanılmıyor (sadece bir trafonun eksik tüketim
  verisini tamamlamak için yedek kaynak olarak kullanılıyor, başka hiçbir yerde değil).
- **Önerilen geliştirme**: Mevcut/dolu alanlar için — hangi abonenin hangi trafoya bağlı
  olduğu zaten kesin olarak bilindiğinden, o trafoya gelecek yatay büyümeyi ve buna bağlı pik
  yüklenme etkisini **doğrudan bu gerçek bağlantı üzerinden** hesapla; süper hücre
  yaklaşıklığına ihtiyaç yok. Yeni/henüz gelişmemiş hücreler için ise gerçek bağlantı verisi
  olmadığından süper hücre mantığı (bir trafonun kapsayabileceği yaklaşık alan, örn.
  ~210x280m) kullanılmaya devam eder — bu kısım zaten mevcut yaklaşıma yakın.
- **Durum**: Henüz uygulanmadı, yeni geliştirme önerisi olarak not edildi (2026-09-30).

### Mevcut (zaten inşa edilmiş) binalarda kademeli abone dolumu — şu an hiç yok
- **Doğrulandı (kod incelemesiyle)**: `abone_sayısı_tahmini.py` ve `hucre_abone_as_is.py`'de,
  zaten var olan bir binanın "imara göre olası kapasitesi" ile "şu an gerçekten bağlı abone
  sayısı" hiç karşılaştırılmıyor. `hucre_abone_as_is.py` sadece mevcut abone noktalarının ham
  coğrafi sayımını yapıyor (kapasite/doluluk/boşluk kavramı yok). Bir bina, satürasyon
  modelinde "yapıldı" sayıldığı anda, o bina tipi için sabit bir ortalama abone sayısıyla
  tek seferde "dolu" kabul ediliyor — sonrasında o binaya özel bir "hâlâ abone alıyor mu"
  takibi yapılmıyor.
- **Önerilen geliştirme**: Zaten inşa edilmiş bir binanın tüketim eğiliminden (örn. bina
  içindeki bağlantı gücü kullanım oranı, zaman içindeki tüketim artış paterni), o binanın
  hâlâ "tüketim anlamında" satüre olup olmadığına dair bir sinyal çıkarılabilir; buna göre
  bu binaya kademeli olarak ek abone atanabilir. Not: mevsimlik/boş konutlar ve ticari
  tüketim dalgalanmaları bu sinyali gürültülü yapabilir — tek başına güvenilir bir kaynak
  olmayabilir, ama en azından ek bir gösterge olarak değerlendirilebilir.
- **Durum**: Henüz uygulanmadı, yeni geliştirme önerisi olarak not edildi (2026-09-30).

### DTR bazında saatlik kısa/orta/uzun vadeli talep tahmini (opsiyonel)
- İstenirse, her trafo için ayrı ayrı saatlik (kısa vadeli), aylık/mevsimsel (orta vadeli) ve
  çok-yıllı (uzun vadeli) talep tahmin modelleri kurulabilir.
- Bu saatlik/çok-ufuklu tahminler, nihai trafo yatırım kararlarına (Optimal DTR modülünün
  kapasite artırma/yeni trafo tesis kararlarına) doğrudan girdi olarak kullanılabilir —
  şu anki yıllık/sabit-katsayı bazlı karardan daha hassas bir planlama sağlar.
- **Durum**: talep gelirse ayrı bir iş paketi olarak ele alınacak, şu an kapsamda değil.
