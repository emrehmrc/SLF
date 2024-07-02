# SLF

Bu dosyada SLF projesinde yazılım geliştirirken yararlı olacak bilgiler yer almaktadır.

## Git/Github Kullanımı

### Visual Studio üzerinden Git reposu klonlama
Bu dosyayı görüyorsanız, Github hesabınız var ve bu repoya erişiminiz var demektir.
Projeyi ilk defa kullanacaksanız, projeyi klonlamanız gerekir. Bunun Visual Studio'yu açın ve ilk ekranda sağda yer alan "Clone a repository" seçeneğine tıklayın. Açılan pencerede en altta "Browse a repository" seçeneğine tıklayın. Burda Github hesabınıza ekli repolar çıkacaktır. Hesabınız Visual Studio'da ekli değilse, "Add an account" seçeneğine tıklayarak Github hesabınızı ilişkilendirebilirsiniz. Hesabınızı ekledikten sonra "Search or enter a URL" kısmına [https://github.com/emrehmrc/SLF.git](https://github.com/emrehmrc/SLF.git) girin ve altta bu repoyu bilgisayarınızda klonlamak istediğiniz yeri seçin. Klonlama işlemi tamamlandıktan sonra projeyi açabilirsiniz.

Herhangi bir sorun olduğunda [Clone a repo in Visual Studio](https://learn.microsoft.com/en-us/visualstudio/version-control/git-clone-repository?view=vs-2022) linkinden de yardım alabilirsiniz. Genel olarak Visual Studio'nun Git dökümantasyonu güzel epey.

### Visual Studio içerisinde Git Ayarları
Projeyi klonladıktan sonra, projeyi kullanmaya başlamadan önce Visual Studio içerisinde bazı ayarları yapmanız gerekmektedir. Bunlar:
[şu adreste](https://learn.microsoft.com/en-us/visualstudio/version-control/git-settings?view=vs-2022) yer almaktadır. Genel itibariyle burda anlatılanlar yeterlidir. Hatta klonladığınız için bazı özellikler zaten otomatik olarak ayarlanmış olabilir.

### Klasör üzerinden Git ayarları (ÖNEMLİ)
Proje içerisinde hatalı commitler, Linux/Mac/Windows satır sonu, dosya izinleri gibi sorunlar oluşmaması için klasör bazında bazı ayarlar yapmanız gerekmektedir. Bu ayarları yapmak için projenin kök dizininde yer alan .git klasörüne gidin (Gizli klasör olarak görünecektir muhtemelen) ve config dosyasını Not Defteri'nde açın. Ordaki sadece \[core\] kısmını değiştirerek aşağıdaki gibi yapın. Eğer bu ayarlar yapılmazsa, projede commit yaparken hatalarla karşılaşabilirsiniz. Bu ayarlar, projenin Linux/Mac/Windows uyumluluğunu sağlar. Ayrıca dosya izinleri ve satır sonu karakterleri gibi sorunları da çözer.

```
[core]
	repositoryformatversion = 0
	bare = false
	logallrefupdates = true
	ignorecase = false
	autocrlf = true
	filemode = false
```


Bu ayarlar, projenin kök dizininde yer alan `.gitattributes` ve `.gitignore` dosyalarıdır. Bu dosyalar, projenin Git üzerinde nasıl davranacağını belirler. `.gitattributes` dosyası, dosya izinleri, satır sonu karakterleri gibi dosya bazlı ayarları belirler. `.gitignore` dosyası ise Git'in takip etmemesi gereken dosya ve klasörleri belirler. Bu dosyaları projenin kök dizininde bulabilirsiniz. Bu dosyaları açarak içeriklerini inceleyebilirsiniz. Eğer bu dosyaları değiştirmeniz gerekiyorsa, değişiklikleri yaparak commit etmeyi unutmayın.

### Branchler
Projede 2 tane ana branch bulunmaktadır. Bunlar `main` ve `development` branchleridir. `main` branchi, projenin stabil ve çalışır durumda olduğu branchdir. `development` branchi ise projenin geliştirme aşamasında olduğu branch'tir. Branchleri değiştirmek için Visual Studio'da sağ alt köşede yer alan branch ismine sağ tıklayıp checkout diyerek değiştirebilirsiniz. Branchleri değiştirirken dikkatli olunmalıdır. Halihazırda kaydedilmemiş ya da commit edilmemiş değişiklikler varsa, branch değiştirme işlemi yapılmamalıdır. Zaten aşağıda da bahsedeceğim üzere değişikliklerinizi kendi branchlerinizde yapacaksınız.

Muhtemelen repo kurulumunda sadece main gelecek. Diğer branchlere ulaşmak için branch menüsünde remotes kısmında en azından development'a da sağ tıklayıp checkout diyebilirsiniz.

Geliştirme aşamasında her kişinin kendi branchi olacak. Bu branchlerin isimlendirmesi ise `isim/feature-name` şeklinde olmalıdır. `isim/feature-name` gibi bir isimlendirme kişilerin branchlerini `isim` klasöründe toplar. Aynı anda birden fazla branchte çalışan kişi için derli toplu olunmada yardımcı olacaktır bu. Bir kişi bir özellik eklemek istediğinde, `isim/login-page` şeklinde bir branch oluşturabilir. Branch oluşturmak için sağ alttaki branch menüsünde new branch deyin. Based on için `development`'ı seçin çünkü kendi branchimizde çalışırken development'ın en güncel sürümünden branch açmamız lazım. En güncel sürüm ise pull yaparak elde edilir. Git Changes penceresinde pull butonuna tıklayınca Github'dan mevcut branchin en güncel sürümü indirilir. Branch açarken ya da pull yaparken kaydedilmemiş ya da commitlenmemiş değişiklikler olmadığına dikkat edin. Ama diyelim ki development'ın güncel sürümündesiniz ve kod yazmaya başladınız, commitlemeden kaydettiniz. Yeni branch'i açıp commitlenmemiş değişiklikleri yeni branche geçir seçeneğiyle değişikliklerinizi yeni branche taşıyabilirsiniz.

### Commitler
Commitler, reponun kaydetme butonu gibi bir şeydir. Diyelim ki 1 haftalık bir işiniz var ve birkaç farklı fonksiyon yazmanız gerekiyor. Kodda yapılabilecek en küçük modüler değişiklik commit'e tekabül ediyor diyebiliriz. Mesela "Butona basıp dosya adı seçince o dosyayı yüklemek" fonksiyonu, yapılmakta olan işte bir önceki commite göre kaydadeğer bir ilerlemedir. Bu ilerlemeyi commitleyerek hem kodunuzu kaydetmiş olursunuz hem de ilerlemelerinizi takip edebilirsiniz. Kodda bir satır değiştirip, bir iki değişken ekleyip commitlemek genellikle iyi bir fikir değildir. Bu kodda ilerlemeyi anlamakta zorluk çıkarabilir. Burda ana amaç şu olmalı: "Bu committe branchin önceki haline kıyasla ne değişti?" sorusuna cevap verebilmek. Mesela, ekstrem bir durum olarak 1 satır değiştirerek veritabanı hızını %90 arttırıp optimize etmişliğim olmuştu. Bunu commit etmek, iyi bir fikirdir mesela. Çünkü, bu commitin ne yaptığını açıkça anlayabiliriz.

Commitlerinizi yaparken commit mesajlarınızın açıklayıcı olmasına dikkat edin. Commit mesajları, commitin ne yaptığını açıklamalıdır. Bu, takımdaki diğer kişilerin ya da sizin, daha sonra projeye dönüp baktığınızda ne yaptığınızı hatırlamanızı sağlar.

Örnek commit mesajları:
    - "Login sayfasına Facebook ile bağlanma mantığı eklendi"    
  	- "Dosya yükleme esnasında oluşan geçersiz dosya hatası giderildi"

Kötü commit mesajları:
    - "Değişiklikler"
	- "updateeee"
	- "idk"
	- "Ekonometri modülü" (Bu, commitin ne yaptığını açıklamıyor. Ekonometri modülünde ne değişiklikler yapıldığını açıklayın.))

Genelde, yapılan değişikliklerde Commit All yeterli olacaktır ama bazen sadece belirli dosyaları commit etmek isteyebilirsiniz. Bu durumda Changes sekmesindeki dosyalara sağ tıklayarak stage diyebilirsiniz. Stage alanını, commit edilecek dosyaları commit edilmeyecek dosyalardan ayıran bir alan olarak düşünebilirsiniz. Eğer stage alanında commit etmek istemediğiniz dosyalar varsa, bu dosyaları stage alanından çıkarabilirsiniz. Stage alanındaki dosyaları çıkarmak için dosyalara sağ tıklayarak unstage diyebilirsiniz. Unstage, stage alanındaki dosyaları çıkartır. Unstage alanındaki dosyalar, commit edilmeyecek dosyalardır. Unstage alanındaki dosyaları stage alanına almak için de aynı şekilde dosyalara sağ tıklayarak stage diyebilirsiniz. Stage alanındaki dosyaları commit etmek için ise Commit Staged butonuna basmanız yeterlidir.

Changes ya da staged alanındaki dosyalara çift tıklayarak onlarda son committen beri yapılan değişiklikleri görebilirsiniz. Commit ederken nolur her bir dosyada neleri değiştirdiğinizi iyi inceleyin. Bazen yanlışlıkla alakasız bir modüldeki değişkenleri siliyor olabiliyorsunuz. Bu durumda bazen çakışmalar oluyor, daha da kötüsü o modülün işleyişini bozabiliyorsunuz.

### Push ve Pull Request
Commitlerinizi yaptıktan sonra bu commitleri Github'a göndermeniz gerekmektedir. Bunun için `Push` butonuna basmanız yeterlidir. Push yaptıktan sonra Github'da repoya gittiğinizde commitlerinizi görebilirsiniz. Ancak dikkat edin. Push yaparken commit edilmemiş değişiklikler olabilir. Bu durumda muhtemelen boş push yapmış olursunuz. Push'lar commitler yapıldıktan sonra Github'a yüklenmek istendiğinde yapılmalıdır.

Eğer bir branchte çalışıyorsanız ve bu branchteki işiniz bittiğinde, bu branchi `development` branchine birleştirmek isteyebilirsiniz. Bunun için **Github** üzerinden `Pull Request` yapmanız gerekmektedir. Pull Request, branchler arasındaki farkları gösteren ve bu farkları birleştirmenizi sağlayan bir araçtır. ~~Pull Request yaparken, yaptığınız işi açıklayan bir başlık ve açıklama yazmalısınız.~~ (Bu gerekli değil) Pull Request yaptıktan sonra Github yetkilisi bu Pull Request'i inceleyip onaylayabilir ya da değişiklikler isteyebilir. Pull Request'iniz onaylandıktan sonra branchler birleştirilir ve `development` branchi güncellenir. Olası çakışma ya da yetersiz kodlama durumlarında Github yetkilisi değişiklikler isteyebilir. Bu durumda değişiklikleri yaparak tekrar commit edip yeniden Pull Request yapabilirsiniz.

Github üzerinden Pull Request yapmaya dair dökümantasyon için [Creating a pull request](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/creating-a-pull-request) linkine bakabilirsiniz.
