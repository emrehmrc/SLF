# VEER Modülü dokümantasyonu

## [Modüller.cs] dosyası

[Modüller.cs] dosyası bizim ana modül dosyamız. Orada `button1_Click` fonksiyonu bizim dosya seçimi gerçekleştiğinde gerçekleştirdiğimiz işlemleri içeriyor. 

- `seçilenVeriTipi` değişkeni combobox'ta seçilen veri tipidir ve `GirdiModülü` sınıfı ve alt sınıflarında kullanılabilir. Bu değişkeni dosya yükleme dışında kullanmıyoruz.
- `girdiModülü` değişkeni ise `girdiModülleri` sözlüğü içerisinde seçilen veri tipine karşılık gelen girdiModülü sınıf ailesinden birini tutar. Burda sınıf ailesinden kastım sınıf ve kendisinin alt sınıflarıdır. Bu değişken [Modüller.cs](..\Modüller.cs) dosyasının başında sözlük olarak tanımlıdır. Şu ana kadar `AboneVerileri` ve `EASarjModulu` sınıfları oluşturulduğu için o veri tipine karşılık gelen sınıflar oluşturulmuştur. Geri kalanlardaki `new GirdiModülü ()` ifadeleri de henüz başlanmamış veri tipleri için atanan öylesine değişkenler aslında. Yeni veri tiplerini yazmaya başladıkça onu değiştirin.
- `girdiModülü.ProcessFileSelection(seçilenVeriTipi)` dosyayı yüklemekle alakalı bir fonksiyon. Bu fonksiyon [girdiModülü.cs]'de tanımlıdır. Bu fonksiyon, seçilen veri tipine göre Excel mi, CSV mi yoksa tabular veri mi yükleyeceğini anlar. Bunu da fonksiyon içerisindeki `veri_listesi_requires_xlsx`,  `veri_listesi_requires_csv` ve `veri_listesi_requires_tabular` değişkenlerine göre yapar. Bu değişkenler GirdiModülü sınıfının başında tanımlıdır. Bu 3 değişken, bir liste olarak veri tipinin xlsx mi, csv mi yoksa tabular veri mi yükleyeceğini tutar. Yeni bir veri tipine başladığınıdzda bu listelerden birine veri tipinin adını ekleyin.

- `button1_Click` fonksiyonu üzerinde gidiyordum şimdiye kadar ancak gerisine girdi modülü içi fonksiyonlardan devam edebilirim. Burdaki Validate, Remove, Impute gibi fonksiyonları girdi modülü üzerinden anlatacağım. Ama basitçe burdaki mantık şu: Dosya seçimi diyaloğunda dosyayı seçtikten sonra dosyayı C#'taki datatable denen bir yapıya aktarıyoruz, bu datatable'ı görsel olarak DataGridView üzerinde gösterebildiğimiz için kullanışlı bir yapı. Veri düzgün yüklenmişse VEER modülü işlevlerini yerine getiriyoruz, değilse hata mesajlarından uygun olanlarını verdiriyoruz.


## [girdiModülü.cs] dosyası
GirdiModülü sınıfı, kullanıcı tarafından seçilen dosyaları işler ve çeşitli veri türleri için doğrulama yapar. Ayrıca, hataları, uyarıları ve bilgi mesajlarını saklamak için farklı DataTable nesneleri kullanır.

- `NoFileSelectedException`: Bir dosya seçilmediğinde atılan özel bir istisna sınıfıdır. Başka yerde kullanmayacağız.

Değişkenler:
- **onizleme1**: Verileri önizlemek için kullanılan Önizleme nesnesidir. Alt sınıflarda kullanmaya gerek yok.
- **veri_listesi_requires_xlsx, veri_listesi_requires_csv, veri_listesi_requires_tabular**: Belirli dosya türleri için gereken veri listelerini tutan sabit listelerdir. Bunu yukarda açıklamıştım, alt sınıflarda kullanmamıza gerek yok.
- **nullLikeStrings**: Null benzeri değerleri tanımlamak için kullanılan string listesi. Bu değerlere sahip stringler null muamelesi görüyor. Normalde null benzeri değerler için IsNullLike fonksiyonunu kullanıyoruz, o fonksiyon bu değişkeni içerisinde kullanıyor zaten. Bunu ellemeyiz ya da alt sınıflara kopyalamayız ama belki atıyorum bir dosyada "NULL" diye bir değer vardır bu bizim listemizde ekli değildir, o zaman bu listeye yeni bir null like string ekleyebiliriz.
- **seçilenVeriTipi**: Seçilen veri türünü tutan string değişken. Bunu yukarıda açıklamıştım.
- **HoursInYear, lastYear, penultimateYear**: Bunları tüketim verilerinde kullanmıştık, diğer veri türlerinde de lazım olabilir.
- **combinedExcelFilter, combinedCsvFilter, combinedTabularFilter**: Farklı dosya türleri için birleştirilmiş filtre stringleri. Bunu değiştirmeye ya da alt sınıflarda kullanmaya gerek yok.
- **currentDataTable, errorDataTable, warningDataTable, infoDataTable**: ÖNEMLİ! Bunlar dosya yükledikten sonra karşımıza çıkan önizlemede 4 sekmede gösterilen datatable değişkenleri. Bunları çok sık kullanacağız. Genellikle `Validate`, `Remove` ya da `Impute` ederken bu değişkenleri kullanarak datatable'lara satır vesaire ekleyeceğiz. Örnekleri [AboneVerileri.cs] dosyasında görebilirsiniz.
- **columnNullRowsMap, imputableRowsMap, binaIdToMostFrequentCoordinates, aboneGrubuMostFrequent**: Veri işleme sırasında kullanılan çeşitli sözlükler. Bunlar [girdiModülü.cs] dosyasında deklare edilse de her veri tipinde farklı sütunlar null vesaire olacak, o yüzden sözlüğü asıl alt sınıflarda tanımlıyoruz. Bunların tipi zaten yazıyor, string key oluyor, değerleri de bazen integer listesi, bazen ikili double (koordinat için mesela)
- **COORDINATE_ROUNDING_PRECISION**: Abone verilerinde aynı bina için koordinatları 3 ondalığa kadar aynı olanları aynı saydık mesela, 3 ondalıkta farklı ise impüte ettik, onun için. Belki koordinatlı diğer veri tiplerinde lazım olur.
- **MAX_THRESHOLD, MIN_THRESHOLD, WARNING_ONLY, INFO_ONLY, ERROR_ONLY**: ÖNEMLİ! MAX_THRESHOLD ve MIN_THRESHOLD kullanmayacağız, sadece diğer değişkenleri tanımlamak için lazım. Basitçe Info Warning Error'un yüzdelik eşiklerini kolayca tanımlayabilmek için böyle bir şey yaptım. [AboneVerileri.cs]'nde `nullFieldsCheckWithLevel` değişkenini görmüşsündür, orda InfoErrorBoundary, WarningErrorBoundary, InfoWarningBoundary gibi fonksiyonlar var. Onlara bir float değeri verildiğinde, mesela `0.2f` verdiğimizde eşik %20'dir. Fonksiyon InfoError ise bir validasyonda atıyorum null değer %20'nin altındaysa infoDataTable'a satır yazarsın, üstündeyse errorDataTable'a yazarsın. Ama onun dışında her halükarda aynı bilgi seviyesinde datatable'a yazacaksan, `WARNING_ONLY, INFO_ONLY, ERROR_ONLY` değişkenlerini bu `nullFieldsCheckWithLevel` veya diğer gerekli değişkenlerde sözlük değeri olarak atayabilirsin.

- **CurrentDataTable, ErrorDataTable, WarningDataTable, InfoDataTable, Onizleme1**: Public olarak erişilebilen özelliklerdir. Bunlar bizi çok ilgilendirmiyor, o biraz da programlamadaki enkapsülasyon denen şeyle alakalı olduğu için var. Bunlar değişkenlere dışardaki sınıflardan erişmek için, ama biz alt sınıflarda böyle public değişkenlerle uğraşmıyoruz. Burda önemli şey şu: Bu 5 değişken büyük harfle başlıyorsa bizi ilgilendirmiyor, küçük harfli versiyonlarını kullanacağız. Büyük harfle başladıysak onu değiştirip küçük harfle yazacağız.

### Fonksiyonlar:
- **IsNullLike(object value)**: ÖNEMLİ! Bir değerin null benzeri olup olmadığını kontrol eder. true ya da false döndürür.
- **WarningErrorBoundary(float boundary), InfoErrorBoundary(float boundary), InfoWarningBoundary(float boundary)**: ÖNEMLİ! Eşikler arasında uyarı, bilgi ve hata sınırlarını belirleyen statik metodlardır.
- **GirdiModülü()**: Constructor dediğimiz şey, sınıfı initialize ederken lazım ama biz bunu alt sınıflarda kullanmayacağız. Burda datatable'ları frontend'deki datatable'lara eşliyoruz mesela. Onun dışıında birtakım ayarlar da yapılıyor, datatable'lara sütun adları ekleniyor.
- **IsError(), IsWarning(), IsInfo()**: Hata, uyarı ve bilgi tablolarında satır olup olmadığını kontrol eden metodlar. true ya da false döndürür. Ben bunu Modüller.cs'te kullandım ama alt sınıflarda da kullanılabilir, sakıncası yok.
- **AddColumnsToDataTable(DataTable table)**: Bir DataTable'a sütun ekler. Bu lazım değil, Error, Warning ve Info datatable'lara sütun eklemek için varlar.
- **ProcessFileSelection(string seçilenVeriTipi)**: Kullanıcının seçtiği dosya türüne göre uygun dosya işleme metodunu çağırır. Kullanmayacağız.
- **ProcessExcelFile(string fileName, string seçilenVeriTipi)**: Excel dosyasını işleyip DataTable olarak döner. Kullanmayacağız.
- **ProcessCsvFile(string fileName), ProcessTabularFile(string fileName)**: CSV ve Tabular dosyalarını işleyip DataTable olarak döner. (Fonksiyonlar implement edilmedi henüz, belki CSV verisi kullanacaksak implement ederim.)
- **Validate(), Remove(), Impute()**: Verileri doğrulayan, çıkaran ve tamamlayan metodlar. ÖNEMLİ! Bunları alt sınıflarda `private override void` diye override edip kullanın, örneği [AboneVerileri.cs]'te var. Bu fonksiyonları kendi alt sınıflarınızda kullanacaksınız, içini dilediğiniz impütasyon metoduyla doldurun.
- **ClearRows()**: Hata, uyarı ve bilgi tablolarını temizler. Onu ben validasyonları birbirinin ardına yaparken tabloları temizlemek için kullanıyorum, bize lazım değil.
- **GetDataTableBasedOnThreshold(float currentPercentage, float warningThreshold, float errorThreshold)**: Verilen eşik değerlere göre uygun DataTable'ı döner. ÖNEMLİ! Validasyon metodlarında null vesaire gibi şeyler için yüzdelik eşik değerine göre hangi datatable'a seçeceğimizi bilmek için lazım. Mesela %20'lik bir eşik değerinde altındaysa Warning, üstündese Error datatable'ını döndür bana gibi. Bunu anlamak için uğraşmamak lazım, [AboneVerileri.cs]'ten kopyala geç :D


## [AboneVerileri.cs] dosyası

Bu kod, AboneVerileri adında bir sınıfı tanımlayan ve çeşitli veri doğrulama ve işleme yöntemleri içeren bir C# sınıfıdır. Kodun genel yapısını ve işlevlerini gözden geçirelim: (Bu kısımda bence açıklamaların hangisini ChatGPT yazdı, hangisini OnurGPT yazdı anlarsın :D)

### 1. Sınıf ve Alanlar
Sınıf, GirdiModülü adında bir üst sınıftan türetilmiş ve çeşitli alanlar, sabitler ve sözlükler tanımlanmış:

- **minMaxCheckMap**: X ve Y koordinatları için minimum ve maksimum değerleri saklar. Bunları test amaçlı değiştirebilirsin, bu muhtemelen ilerde değişecek imar verileri geldiğinde ama değerlerle oynayarak yüzde kaç hatalı çıkıyor görebilirsin.
- **TUKETIM_ERROR_THRESHOLD ve COORDINATE_ERROR_THRESHOLD**: Uyarı ve hata eşiklerini saklar. Bu tüketimde sıfırdan düşük değerlerde %20'den fazla ise error verdiriyorduk, tipik sadece null-like kontrolü yapmıyorduk, o yüzden ayrı bir sabit değişken atadım lazım oldu diye. `COORDINATE_ERROR_THRESHOLD` da aynı şekilde.
- **ABONE_KAPASITE_LIMIT**: Abone kapasite sınırını belirler.
- **DATE_FORMAT**: Tarih formatı için kullanılan sabit. Kullanmadım bile.
- **nullFieldsCheckWithLevel, dateFormatCheckWithLevel, duplicateFieldsGivingError**: Bunları direkt kopyalayıp ihtiyacına göre uyarlayabilirsin.

### 2. Metodlar
- **Validate()**: Verileri doğrulamak için bir dizi yöntem çağırır. Burda sırasına göre gerekli validasyonları yapmak için fonksiyonları çağırıyorum. Validasyon demek sadece error warning veya info datatable'larına veri koymak demek. Burda silme ya da impütasyon olmaz, önce kullanıcıya validasyonları göstereceğiz ki ona göre aksiyon alsın.
- **Remove()**: Datatable'da bir 2. satırı siliyorsak 3. satır 2'ye kayıyor, Excel mantığı. Python Pandas'taki gibi 2 nolu indeks boş kalıyor 1'den 3'e geçiyoruz gibi bir şey yok. Öyle olsa işimiz kolaylaşırdı. O yüzden silinecek satırların numarasını bir araya toplayıp öyle silmek gerek. Yoksa yanlış satırları silebiliriz. Orda yardım ederim epey.
- **RemoveCombinedRows(List\<int\> rowsToRemoveList)**: Bu fonksiyon aslında silinecek satırlar listesinin distinctini alıyor ve sonra o satırları siliyor. Aslında bunu üst sınıfa koymam gerekiyordu ama unutmuşum, direkt kopyalayabilirsin.
- **Impute()**: Eksik verileri doldurmak için yöntemler çağırır.
- **ImputeCoordinates(), AboneGrubuImpute(), BaglantiGucuImpute()**: Eksik verileri doldurmak için özel yöntemlerdir.


### 3. Raporlama Metodları
- Aslında buna validasyon metodu da diyebiliriz. Amaçları Datatable'a satır yazmak. Ama bir yandan da sözlüklere silinecek ya da impüte edilecek satırların numarasını ekliyor. Validasyonda sadece tablolara satır yazıp bir işlem yapmıyoruz demiştik. Ama tabii, örneğin null satırların yüzdesini warning olarak verirken bir yandan da satır numaralarını sözlüklerden birine koyalım ki sonrasında silme kısmına geçerken o sözlüklerden veri çekelim.
- **ReportNullCounts(), ReportSanalCounts(), ReportDateFormatErrors(), ReportDuplicateRowCounts(), ReportDuplicateCounts(), ReportCoordinatesOutOfLimits(), ReportErrorLessThanZero()**: Verileri doğrulamak ve belirli koşullar altında hataları raporlamak için kullanılan yöntemlerdir.
Bu yöntemler, belirli koşulları kontrol eder ve sonuçları belirli bir formatta raporlar.
- **ReportNullCounts**: Bu önemli. Bunu muhtemelen çoğu veri tipinde kullanacağız. Direkt kopyalayalım. Belki ufak iki üç satır farklılaşabilir. Kısaca null satırların toplam satırlara oranını yazdırıyor, bir yandan da null satırların indeksini columnNullRowsMap'e koyuyor Bu değişkeni sık kullanıyoruz sen de kullan bence.

### 4. Eksik Veri Doldurma
- **ImputeCoordinates()**: Eksik koordinat verilerini doldurur.
- **AboneGrubuImpute(), BaglantiGucuImpute()**: Diğer eksik verileri doldurur.

Kodun tamamında çeşitli kontroller ve raporlama mekanizmaları bulunmakta. Bu yapı, veri doğrulama ve işleme için oldukça kapsamlı bir yaklaşımdır. Kodu geliştirmek ve hata ayıklamak için belirli test senaryoları oluşturabilir ve her bir yöntemi ayrı ayrı test edebilirsiniz. Herhangi bir sorunuz veya belirli bir kısımda yardıma ihtiyacınız olursa, sormaktan çekinmeyin!

[Modüller.cs]: ..\Modüller.cs
[girdiModülü.cs]: ..\girdiModülü.cs
[AboneVerileri.cs]: ..\AboneVerileri.cs

