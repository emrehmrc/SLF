# SLF

Bu dosyada SLF projesinde yazýlým geliþtirirken yararlý olacak bilgiler yer almaktadýr.

## Git/Github Kullanýmý

### Visual Studio üzerinden Git reposu klonlama
Bu dosyayý görüyorsanýz, Github hesabýnýz var ve bu repoya eriþiminiz var demektir.
Projeyi ilk defa kullanacaksanýz, projeyi klonlamanýz gerekir. Bunun Visual Studio'yu açýn ve ilk ekranda saðda yer alan "Clone a repository" seçeneðine týklayýn. Açýlan pencerede en altta "Browse a repository" seçeneðine týklayýn. Burda Github hesabýnýza ekli repolar çýkacaktýr. Hesabýnýz Visual Studio'da ekli deðilse, "Add an account" seçeneðine týklayarak Github hesabýnýzý iliþkilendirebilirsiniz. Hesabýnýzý ekledikten sonra "Search or enter a URL" kýsmýna [https://github.com/emrehmrc/SLF.git](https://github.com/emrehmrc/SLF.git) girin ve altta bu repoyu bilgisayarýnýzda klonlamak istediðiniz yeri seçin. Klonlama iþlemi tamamlandýktan sonra projeyi açabilirsiniz.

Herhangi bir sorun olduðunda [Clone a repo in Visual Studio](https://learn.microsoft.com/en-us/visualstudio/version-control/git-clone-repository?view=vs-2022) linkinden de yardým alabilirsiniz. Genel olarak Visual Studio'nun Git dökümantasyonu güzel epey.

### Visual Studio içerisinde Git Ayarlarý
Projeyi klonladýktan sonra, projeyi kullanmaya baþlamadan önce Visual Studio içerisinde bazý ayarlarý yapmanýz gerekmektedir. Bunlar:
[þu adreste](https://learn.microsoft.com/en-us/visualstudio/version-control/git-settings?view=vs-2022) yer almaktadýr. Genel itibariyle burda anlatýlanlar yeterlidir. Hatta klonladýðýnýz için bazý özellikler zaten otomatik olarak ayarlanmýþ olabilir.

### Klasör üzerinden Git ayarlarý (ÖNEMLÝ)
Proje içerisinde hatalý commitler, Linux/Mac/Windows satýr sonu, dosya izinleri gibi sorunlar oluþmamasý için klasör bazýnda bazý ayarlar yapmanýz gerekmektedir. Bu ayarlarý yapmak için projenin kök dizininde yer alan .git klasörüne gidin (Gizli klasör olarak görünecektir muhtemelen) ve config dosyasýný Not Defteri'nde açýn. Ordaki sadece \[core\] kýsmýný deðiþtirerek aþaðýdaki gibi yapýn. Eðer bu ayarlar yapýlmazsa, projede commit yaparken hatalarla karþýlaþabilirsiniz. Bu ayarlar, projenin Linux/Mac/Windows uyumluluðunu saðlar. Ayrýca dosya izinleri ve satýr sonu karakterleri gibi sorunlarý da çözer.

```
[core]
	repositoryformatversion = 0
	bare = false
	logallrefupdates = true
	ignorecase = false
	autocrlf = true
	filemode = false
```


Bu ayarlar, projenin kök dizininde yer alan `.gitattributes` ve `.gitignore` dosyalarýdýr. Bu dosyalar, projenin Git üzerinde nasýl davranacaðýný belirler. `.gitattributes` dosyasý, dosya izinleri, satýr sonu karakterleri gibi dosya bazlý ayarlarý belirler. `.gitignore` dosyasý ise Git'in takip etmemesi gereken dosya ve klasörleri belirler. Bu dosyalarý projenin kök dizininde bulabilirsiniz. Bu dosyalarý açarak içeriklerini inceleyebilirsiniz. Eðer bu dosyalarý deðiþtirmeniz gerekiyorsa, deðiþiklikleri yaparak commit etmeyi unutmayýn.

### Branchler
Projede 2 tane ana branch bulunmaktadýr. Bunlar `main` ve `development` branchleridir. `main` branchi, projenin stabil ve çalýþýr durumda olduðu branchdir. `development` branchi ise projenin geliþtirme aþamasýnda olduðu branch'tir. Branchleri deðiþtirmek için Visual Studio'da sað alt köþede yer alan branch ismine sað týklayýp checkout diyerek deðiþtirebilirsiniz. Branchleri deðiþtirirken dikkatli olunmalýdýr. Halihazýrda kaydedilmemiþ ya da commit edilmemiþ deðiþiklikler varsa, branch deðiþtirme iþlemi yapýlmamalýdýr. Zaten aþaðýda da bahsedeceðim üzere deðiþikliklerinizi kendi branchlerinizde yapacaksýnýz.

Muhtemelen repo kurulumunda sadece main gelecek. Diðer branchlere ulaþmak için branch menüsünde remotes kýsmýnda en azýndan development'a da sað týklayýp checkout diyebilirsiniz.

Geliþtirme aþamasýnda her kiþinin kendi branchi olacak. Bu branchlerin isimlendirmesi ise `isim/feature-name` þeklinde olmalýdýr. `isim/feature-name` gibi bir isimlendirme kiþilerin branchlerini `isim` klasöründe toplar. Ayný anda birden fazla branchte çalýþan kiþi için derli toplu olunmada yardýmcý olacaktýr bu. Bir kiþi bir özellik eklemek istediðinde, `isim/login-page` þeklinde bir branch oluþturabilir. Branch oluþturmak için sað alttaki branch menüsünde new branch deyin. Based on için `development`'ý seçin çünkü kendi branchimizde çalýþýrken development'ýn en güncel sürümünden branch açmamýz lazým. En güncel sürüm ise pull yaparak elde edilir. Git Changes penceresinde pull butonuna týklayýnca Github'dan mevcut branchin en güncel sürümü indirilir. Branch açarken ya da pull yaparken kaydedilmemiþ ya da commitlenmemiþ deðiþiklikler olmadýðýna dikkat edin. Ama diyelim ki development'ýn güncel sürümündesiniz ve kod yazmaya baþladýnýz, commitlemeden kaydettiniz. Yeni branch'i açýp commitlenmemiþ deðiþiklikleri yeni branche geçir seçeneðiyle deðiþikliklerinizi yeni branche taþýyabilirsiniz.

### Commitler
Commitler, reponun kaydetme butonu gibi bir þeydir. Diyelim ki 1 haftalýk bir iþiniz var ve birkaç farklý fonksiyon yazmanýz gerekiyor. Kodda yapýlabilecek en küçük modüler deðiþiklik commit'e tekabül ediyor diyebiliriz. Mesela "Butona basýp dosya adý seçince o dosyayý yüklemek" fonksiyonu, yapýlmakta olan iþte bir önceki commite göre kaydadeðer bir ilerlemedir. Bu ilerlemeyi commitleyerek hem kodunuzu kaydetmiþ olursunuz hem de ilerlemelerinizi takip edebilirsiniz. Kodda bir satýr deðiþtirip, bir iki deðiþken ekleyip commitlemek genellikle iyi bir fikir deðildir. Bu kodda ilerlemeyi anlamakta zorluk çýkarabilir. Burda ana amaç þu olmalý: "Bu committe branchin önceki haline kýyasla ne deðiþti?" sorusuna cevap verebilmek. Mesela, ekstrem bir durum olarak 1 satýr deðiþtirerek veritabaný hýzýný %90 arttýrýp optimize etmiþliðim olmuþtu. Bunu commit etmek, iyi bir fikirdir mesela. Çünkü, bu commitin ne yaptýðýný açýkça anlayabiliriz.

Commitlerinizi yaparken commit mesajlarýnýzýn açýklayýcý olmasýna dikkat edin. Commit mesajlarý, commitin ne yaptýðýný açýklamalýdýr. Bu, takýmdaki diðer kiþilerin ya da sizin, daha sonra projeye dönüp baktýðýnýzda ne yaptýðýnýzý hatýrlamanýzý saðlar.

Örnek commit mesajlarý:
    - "Login sayfasýna Facebook ile baðlanma mantýðý eklendi"    
  	- "Dosya yükleme esnasýnda oluþan geçersiz dosya hatasý giderildi"

Kötü commit mesajlarý:
    - "Deðiþiklikler"
	- "updateeee"
	- "idk"
	- "Ekonometri modülü" (Bu, commitin ne yaptýðýný açýklamýyor. Ekonometri modülünde ne deðiþiklikler yapýldýðýný açýklayýn.))

Genelde, yapýlan deðiþikliklerde Commit All yeterli olacaktýr ama bazen sadece belirli dosyalarý commit etmek isteyebilirsiniz. Bu durumda Changes sekmesindeki dosyalara sað týklayarak stage diyebilirsiniz. Stage alanýný, commit edilecek dosyalarý commit edilmeyecek dosyalardan ayýran bir alan olarak düþünebilirsiniz. Eðer stage alanýnda commit etmek istemediðiniz dosyalar varsa, bu dosyalarý stage alanýndan çýkarabilirsiniz. Stage alanýndaki dosyalarý çýkarmak için dosyalara sað týklayarak unstage diyebilirsiniz. Unstage, stage alanýndaki dosyalarý çýkartýr. Unstage alanýndaki dosyalar, commit edilmeyecek dosyalardýr. Unstage alanýndaki dosyalarý stage alanýna almak için de ayný þekilde dosyalara sað týklayarak stage diyebilirsiniz. Stage alanýndaki dosyalarý commit etmek için ise Commit Staged butonuna basmanýz yeterlidir.

Changes ya da staged alanýndaki dosyalara çift týklayarak onlarda son committen beri yapýlan deðiþiklikleri görebilirsiniz. Commit ederken nolur her bir dosyada neleri deðiþtirdiðinizi iyi inceleyin. Bazen yanlýþlýkla alakasýz bir modüldeki deðiþkenleri siliyor olabiliyorsunuz. Bu durumda bazen çakýþmalar oluyor, daha da kötüsü o modülün iþleyiþini bozabiliyorsunuz.

### Push ve Pull Request
Commitlerinizi yaptýktan sonra bu commitleri Github'a göndermeniz gerekmektedir. Bunun için `Push` butonuna basmanýz yeterlidir. Push yaptýktan sonra Github'da repoya gittiðinizde commitlerinizi görebilirsiniz. Ancak dikkat edin. Push yaparken commit edilmemiþ deðiþiklikler olabilir. Bu durumda muhtemelen boþ push yapmýþ olursunuz. Push'lar commitler yapýldýktan sonra Github'a yüklenmek istendiðinde yapýlmalýdýr.

Eðer bir branchte çalýþýyorsanýz ve bu branchteki iþiniz bittiðinde, bu branchi `development` branchine birleþtirmek isteyebilirsiniz. Bunun için **Github** üzerinden `Pull Request` yapmanýz gerekmektedir. Pull Request, branchler arasýndaki farklarý gösteren ve bu farklarý birleþtirmenizi saðlayan bir araçtýr. ~~Pull Request yaparken, yaptýðýnýz iþi açýklayan bir baþlýk ve açýklama yazmalýsýnýz.~~ (Bu gerekli deðil) Pull Request yaptýktan sonra Github yetkilisi bu Pull Request'i inceleyip onaylayabilir ya da deðiþiklikler isteyebilir. Pull Request'iniz onaylandýktan sonra branchler birleþtirilir ve `development` branchi güncellenir. Olasý çakýþma ya da yetersiz kodlama durumlarýnda Github yetkilisi deðiþiklikler isteyebilir. Bu durumda deðiþiklikleri yaparak tekrar commit edip yeniden Pull Request yapabilirsiniz.

Github üzerinden Pull Request yapmaya dair dökümantasyon için [Creating a pull request](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/creating-a-pull-request) linkine bakabilirsiniz.
