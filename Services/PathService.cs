using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SLF.Services
{
    /// <summary>
    /// Uygulama genelinde path yönetimi sağlayan servis sınıfı
    /// </summary>
    /// 

    public class PathService
    {
        /// <summary>
        /// Uygulama genelinde path yönetimi sağlayan servis sınıfı
        /// </summary>
        public static string _configPythonKodPath;
        public static string _configImarAnaliziPath;
        public static string _configKatmanEslestirmePath;
        public static string _configKatmanDenemePath;
        public static string _configDeepLearningModelPath;
        public static string _configCbs_abone;
        public static string _configveritabanikod;
        public static string _configKonum;
        // Proje klasörüne göre relatif il-ilçe kırılımı klasörü yolu
        private static string _relativeDataPath = "il_ilce_kırılımları"; // Varsayılan değer
        public static string _configSLFMainPath;
        public static string _configDepo; // Yeni eklenen depo path'i
                                          // Temel dizin - ilk çalıştırmada hesaplanır
        public static string _baseDirectory;

        // Seçilen il
        public static string SelectedCity { get; private set; }

        // Seçilen ilçe
        public static string SelectedDistrict { get; private set; }

        // Aktif çalışma klasörü (temp veya proje)
        public static string CurrentWorkingFolder { get; private set; }


        // Çalışma modu
        public static WorkingMode CurrentMode { get; private set; } = WorkingMode.Temporary;

        // Çalışma modları
        public enum WorkingMode
        {
            Temporary, // Geçici çalışma klasörü
            Project    // Kaydedilmiş proje klasörü
        }

        public static string KatmanEslestirmePath
        {
            get
            {
                // If set from config, use that path
                if (!string.IsNullOrEmpty(_configKatmanEslestirmePath) && File.Exists(_configKatmanEslestirmePath))
                {
                    return _configKatmanEslestirmePath;
                }

                // Otherwise, use a default path in the imar_analizi folder
                return Path.Combine(ImarPlansDirectory, "katman_eslesme.csv");
            }
        }
        public static string KatmanDenemePath
        {
            get
            {
                // If set from config, use that path
                if (!string.IsNullOrEmpty(_configKatmanDenemePath) && File.Exists(_configKatmanDenemePath))
                {
                    return _configKatmanDenemePath;
                }

                // Otherwise, use a default path in the imar_analizi folder
                return Path.Combine(ImarPlansDirectory, "katman_deneme.py");
            }
        }
        public static string SLFMainPath
        {
            get
            {
                // If set from config, use that path
                if (!string.IsNullOrEmpty(_configSLFMainPath) && File.Exists(_configSLFMainPath))
                {
                    return _configSLFMainPath;
                }

                // Otherwise, use a default path based on PythonKodDirectory
                return Path.Combine(PythonKodDirectory, "SLF_analizi", "slf_main.py");
            }
        }

        public static string BaseDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_baseDirectory))
                {
                    InitializeBaseDirectory();
                }
                return _baseDirectory;
            }
        }

        public static void SetConfigPath(string configPath)
        {
            try
            {
                // Dosyanın gerçekten var olup olmadığını kontrol et

                // Config dosyasını oku
                if (File.Exists(configPath))
                {
                    string jsonFile = File.ReadAllText(configPath);
                    _configKonum = configPath;
                    dynamic config = JsonConvert.DeserializeObject(jsonFile);

                    // Ana_Klasör_Yolu değerini al
                    if (config != null && config.Ana_Klasör_Yolu != null)
                    {
                        string userRootPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        string anaKlasorYolu = config.Ana_Klasör_Yolu.ToString();

                        // Tam yolu oluştur
                        string fullPath = Path.Combine(userRootPath, anaKlasorYolu);

                        // Eğer bu dizin varsa, _baseDirectory olarak ayarla
                        if (Directory.Exists(fullPath))
                        {
                            _baseDirectory = fullPath;
                            Debug.WriteLine($"Config'den alınan veri klasörü yolu: {_baseDirectory}");

                            // CBS yolunu ayarla (yeni eklenen kısım)
                            if (config.CBS_abone != null)
                            {
                                string cbsPath = config.CBS_abone.ToString();

                                // Eğer yol "/" ile başlıyorsa, başındaki "/" karakterini kaldır
                                if (cbsPath.StartsWith("/"))
                                {
                                    cbsPath = cbsPath.Substring(1);
                                }

                                // İl ve ilçe değerlerini al
                                string il = config.İl?.ToString();
                                string ilce = config.İlçe?.ToString();

                                if (!string.IsNullOrEmpty(il) && !string.IsNullOrEmpty(ilce))
                                {
                                    // CBS yolunu oluştur: Ana_Klasör_Yolu/İl/İlçe/CBS
                                    string cbsFullPath = Path.Combine(fullPath, il, ilce, cbsPath);

                                    // Dosya yolunu düzgün şekilde ayıklama
                                    string cbsDirectory = Path.GetDirectoryName(cbsFullPath);
                                    string cbsFileName = Path.GetFileName(cbsFullPath);

                                    // CBS klasörünün varlığını kontrol et
                                    if (Directory.Exists(cbsDirectory))
                                    {
                                        Console.WriteLine($"CBS klasörü bulundu: {cbsDirectory}");

                                        // Eğer klasörde belirtilen dosya varsa
                                        if (File.Exists(cbsFullPath))
                                        {
                                            _configCbs_abone = cbsFullPath;
                                            Console.WriteLine($"CBS ABONE.TAB dosyası bulundu: {_configCbs_abone}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"CBS ABONE.TAB dosyası bulunamadı: {cbsFullPath}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"CBS klasörü bulunamadı: {cbsDirectory}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("İl veya ilçe bilgisi eksik, CBS yolu ayarlanamadı.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Config dosyasında CBS yolu belirtilmemiş.");
                            }

                            // Program dosyaları klasörü (temel yapı için gerekli)
                            string programDosyalariPath = config.program_dosyaları_path?.ToString() ?? "Program Dosyaları";
                            string programDosyalariFullPath = Path.Combine(fullPath, programDosyalariPath);

                            // "Python Kodları" bölümünü oku
                            if (config["Python Kodları"] != null)
                            {
                                // IMAR_ANALİZİ yolunu oku
                                if (config["Python Kodları"].IMAR_ANALİZİ != null)
                                {
                                    string marAnaliziRelativePath = config["Python Kodları"].IMAR_ANALİZİ.ToString();

                                    // Eğer yol "/" ile başlıyorsa, başındaki "/" karakterini kaldır
                                    if (marAnaliziRelativePath.StartsWith("/"))
                                    {
                                        marAnaliziRelativePath = marAnaliziRelativePath.Substring(1);
                                    }

                                    // ÖNEMLİ DEĞİŞİKLİK: İl değerini path'e dahil etme, doğrudan program dosyaları ile birleştir
                                    string marAnaliziFullPath = Path.Combine(programDosyalariFullPath, marAnaliziRelativePath);

                                    // Dizin kısmını al (dosya adını çıkar)
                                    string marAnaliziDirPath = Path.GetDirectoryName(marAnaliziFullPath);

                                    // İmar analizi yolunu ayarla (dosya yolu)
                                    _configImarAnaliziPath = marAnaliziFullPath;
                                    Debug.WriteLine($"Config'den alınan İmar Analizi kod yolu: {_configImarAnaliziPath}");

                                    // Python kodları ana dizinini de ayarla - marAnaliziDirPath yerine python_kod klasörünü doğrudan bul
                                    // Bu şekilde "İmar/python_kod" içindeki imar_analizi klasörü yerine doğrudan "python_kod" dizinine ulaşacağız
                                    string pythonKodDir = Path.Combine(programDosyalariFullPath, "python_kod");
                                    if (!Directory.Exists(pythonKodDir))
                                    {
                                        // Python kodları dizini bulunamadıysa, dizin yapısından çıkarmaya çalış
                                        pythonKodDir = Path.GetDirectoryName(Path.GetDirectoryName(marAnaliziDirPath));
                                    }
                                    _configPythonKodPath = pythonKodDir;
                                    Debug.WriteLine($"Config'den alınan Python kod yolu: {_configPythonKodPath}");
                                }

                                // SLF_Main yolunu oku
                                if (config["Python Kodları"].SLF_Main != null)
                                {
                                    string slfMainRelativePath = config["Python Kodları"].SLF_Main.ToString();

                                    // Eğer yol "/" ile başlıyorsa, başındaki "/" karakterini kaldır
                                    if (slfMainRelativePath.StartsWith("/"))
                                    {
                                        slfMainRelativePath = slfMainRelativePath.Substring(1);
                                    }

                                    // ÖNEMLİ DEĞİŞİKLİK: İl değerini path'e dahil etme, doğrudan program dosyaları ile birleştir
                                    string slfMainFullPath = Path.Combine(programDosyalariFullPath, slfMainRelativePath);

                                    // Dizin kısmını al (dosya adını çıkar)
                                    string slfMainDirPath = Path.GetDirectoryName(slfMainFullPath);


                                    // SLF Main yolunu ayarla
                                    _configSLFMainPath = slfMainFullPath;
                                    Debug.WriteLine($"Config'den alınan SLF Main kod yolu: {_configSLFMainPath}");
                                }
                            }
                            if (config.katman_eslestirme != null)
                            {
                                string katmanEslestirmeRelativePath = config.katman_eslestirme.ToString();

                                // Remove leading slash if present
                                if (katmanEslestirmeRelativePath.StartsWith("/"))
                                {
                                    katmanEslestirmeRelativePath = katmanEslestirmeRelativePath.Substring(1);
                                }

                                // Combine with program files path
                                string katmanEslestirmeFullPath = Path.Combine(programDosyalariFullPath, katmanEslestirmeRelativePath);

                                // Get directory
                                string katmanEslestirmeDirPath = Path.GetDirectoryName(katmanEslestirmeFullPath);

                                // Set the layer mapping path
                                _configKatmanEslestirmePath = katmanEslestirmeFullPath;
                                Debug.WriteLine($"Config'den alınan Katman Eşleştirme dosya yolu: {_configKatmanEslestirmePath}");
                            }
                            if (config["Python Kodları"] != null && config["Python Kodları"].katman_eslestirme_kod != null)
                            {
                                string katmanKodRelativePath = config["Python Kodları"].katman_eslestirme_kod.ToString();

                                if (katmanKodRelativePath.StartsWith("/"))
                                {
                                    katmanKodRelativePath = katmanKodRelativePath.Substring(1);
                                }

                                string katmanKodFullPath = Path.Combine(programDosyalariFullPath, katmanKodRelativePath);

                                // katman_deneme.py dosyasının yolunu oluştur (katman_kod.py ile aynı dizinde)
                                string katmanDir = Path.GetDirectoryName(katmanKodFullPath);
                                _configKatmanDenemePath = Path.Combine(katmanDir, "katman_deneme.py");

                                Debug.WriteLine($"Python Kodları.katman_eslestirme_kod'dan türetilen Katman Deneme yolu: {_configKatmanDenemePath}");
                            }
                            if (config["Python Kodları"] != null && config["Python Kodları"].deep_learning != null)
                            {
                                string deepLearningRelativePath = config["Python Kodları"].deep_learning.ToString();

                                // Eğer yol "/" ile başlıyorsa, başındaki "/" karakterini kaldır
                                if (deepLearningRelativePath.StartsWith("/"))
                                {
                                    deepLearningRelativePath = deepLearningRelativePath.Substring(1);
                                }

                                // ÖNEMLİ DEĞİŞİKLİK: İl değerini path'e dahil etme, doğrudan program dosyaları ile birleştir
                                string deepLearningFullPath = Path.Combine(programDosyalariFullPath, deepLearningRelativePath);

                                // Dizin kısmını al (dosya adını çıkar)
                                string deepLearningDirPath = Path.GetDirectoryName(deepLearningFullPath);

                                // Deep Learning model yolunu ayarla
                                _configDeepLearningModelPath = deepLearningFullPath;
                                Debug.WriteLine($"Config'den alınan Deep Learning model yolu: {_configDeepLearningModelPath}");
                            }
                            if (config["Python Kodları"] != null && config["Python Kodları"].veritabani_kod != null)
                            {
                                string veritabaniKodRelativePath = config["Python Kodları"].veritabani_kod.ToString();

                                // "/" başındaki karakteri kaldır
                                if (veritabaniKodRelativePath.StartsWith("/"))
                                {
                                    veritabaniKodRelativePath = veritabaniKodRelativePath.Substring(1);
                                }

                                // Program dosyaları ile birleştir
                                string veritabaniKodFullPath = Path.Combine(programDosyalariFullPath, veritabaniKodRelativePath);


                                _configveritabanikod = veritabaniKodFullPath;
                            }
                            if (config.konum != null)
                            {
                                // Konum değerini direkt olarak al
                                string konumValue = config.konum.ToString();

                                // Konum değeri bir dosya yolu mu yoksa sadece dosya adı mı kontrol et
                                if (Path.IsPathRooted(konumValue))
                                {
                                    // Tam yol verilmiş
                                    _configKonum = konumValue;
                                }
                                else
                                {
                                    // Göreceli yol verilmiş, ana klasör ile birleştir
                                    _configKonum = Path.Combine(fullPath, konumValue.TrimStart('/'));
                                }

                                Console.WriteLine($"Konum değeri ayarlandı: {_configKonum}");
                            }

                            else
                            {
                                // Python Kodları bölümü yoksa varsayılan yapıya devam et
                                string pythonKodlariPath = Path.Combine(programDosyalariFullPath, "imar");
                                if (!Directory.Exists(pythonKodlariPath))
                                {
                                    //Directory.CreateDirectory(pythonKodlariPath);
                                }

                                // Python kodu yolunu ayarla
                                _configPythonKodPath = pythonKodlariPath;
                                Debug.WriteLine($"Config'den alınan Python kod yolu: {_configPythonKodPath}");

                            }
                        }
                    }

                    return;
                }

                // Config kullanılamazsa mevcut yöntemi kullan
                InitializeBaseDirectory();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Config ayarlanırken hata: {ex.Message}");
                // Hata durumunda varsayılan yöntemle devam et
                InitializeBaseDirectory();
            }
        }

        /// <summary>
        /// Temel veri dizinini başlatır, relative path'i bulur
        /// </summary>
        private static void InitializeBaseDirectory()
        {
            try
            {
                // Uygulama dizini (exe'nin bulunduğu yer)
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Ana veri klasörünü bulabilmek için birkaç seviye yukarı çıkarak arama
                string currentDir = exeDirectory;
                bool foundDataFolder = false;

                // Önce mevcut dizinde ara
                if (Directory.Exists(Path.Combine(currentDir, _relativeDataPath)))
                {
                    _baseDirectory = Path.Combine(currentDir, _relativeDataPath);
                    foundDataFolder = true;
                }

                // Bulunamadıysa 5 seviye yukarı kadar arama yap
                if (!foundDataFolder)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        // Bir üst dizine çık
                        DirectoryInfo parentDir = Directory.GetParent(currentDir);

                        // Eğer üst dizin yoksa veya kök dizine ulaşıldıysa döngüden çık
                        if (parentDir == null)
                            break;

                        currentDir = parentDir.FullName;

                        // Veri klasörünü kontrol et
                        if (Directory.Exists(Path.Combine(currentDir, _relativeDataPath)))
                        {
                            _baseDirectory = Path.Combine(currentDir, _relativeDataPath);
                            foundDataFolder = true;
                            break;
                        }
                    }
                }

                // Hala bulunamadıysa, son çare olarak tam path'i dene


                // Veri klasörü bulunamadıysa, exe dizini altında yeni bir klasör oluştur
                if (!foundDataFolder)
                {
                    _baseDirectory = Path.Combine(exeDirectory, _relativeDataPath);
                    Directory.CreateDirectory(_baseDirectory);
                    Debug.WriteLine($"Veri klasörü bulunamadı, yeni klasör oluşturuldu: {_baseDirectory}");
                }
            }
            catch (Exception ex)
            {
                // Herhangi bir hata durumunda, exe dizini altında bir klasör kullan
                string fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _relativeDataPath);
                _baseDirectory = fallbackPath;

                if (!Directory.Exists(fallbackPath))
                {
                    Directory.CreateDirectory(fallbackPath);
                }

            }
        }

        /// <summary>
        /// Python kod klasörü yolu - SLF kök dizini altında
        /// </summary>
        public static string PythonKodDirectory
        {
            get
            {    // Eğer config'den ayarlanmışsa, o yolu kullan
                if (!string.IsNullOrEmpty(_configPythonKodPath) && Directory.Exists(_configPythonKodPath))
                {
                    return _configPythonKodPath;
                }
                try
                {
                    // SLF ana dizininde python_kod klasörü
                    string slftRootDir = GetSLFRootDirectory();
                    Console.WriteLine($"SLF kök dizini: {slftRootDir}");

                    // Eğer GetSLFRootDirectory bin dizinini döndürüyorsa, bir seviye daha yukarı çık
                    if (slftRootDir.EndsWith("\\bin"))
                    {
                        slftRootDir = Directory.GetParent(slftRootDir).FullName;
                        Console.WriteLine($"Düzeltilmiş SLF kök dizini: {slftRootDir}");
                    }

                    string pythonKodPath = Path.Combine(slftRootDir, "python_kod");
                    Console.WriteLine($"Python kod yolu: {pythonKodPath}");

                    // Klasör yoksa oluştur
                    if (!Directory.Exists(pythonKodPath))
                    {
                        Directory.CreateDirectory(pythonKodPath);
                        Console.WriteLine($"Python kod klasörü oluşturuldu: {pythonKodPath}");
                    }

                    return pythonKodPath;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Python kod dizini belirlenirken hata: {ex.Message}");
                    // Hata durumunda sabit yolu döndür
                    return @"C:\Users\batuhan.yetis\source\repos\SLF\python_kod";
                }
            }
        }

        /// <summary>
        /// Deep Learning klasör yolu
        /// </summary>
        public static string DeepLearningDirectory => Path.Combine(PythonKodDirectory, "deep_learning");

        /// <summary>
        /// İmar planları klasör yolu
        /// </summary>
        public static string ImarPlansDirectory => Path.Combine(PythonKodDirectory, "imar_analizi");

        /// <summary>
        /// Deep Learning kod klasör yolu
        /// </summary>
        public static string DeepLearningCodeDirectory => Path.Combine(DeepLearningDirectory, "kod");

        /// <summary>
        /// Deep Learning eğitim veri seti klasör yolu

        public static string HucrePath => Path.Combine(BaseDirectory, FullPath, "hücre");

        public static string UyduVerileriPath => Path.Combine(BaseDirectory, FullPath, "uydu_verileri");
        /// <summary>
        /// SLF kök dizinini döndürür
        /// </summary>
        private static string GetSLFRootDirectory()
        {
            try
            {
                // Uygulama dizini (exe'nin bulunduğu yer)
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Bir üst dizine çık (genellikle bin/Debug veya bin/Release içindedir)
                DirectoryInfo parentDir = Directory.GetParent(exeDirectory);
                if (parentDir == null) return exeDirectory;

                // İki üst dizine çık (bin klasörünün üstüne)
                DirectoryInfo projectDir = parentDir.Parent;
                if (projectDir == null) return parentDir.FullName;

                // SLF ana dizinine ulaşana kadar yukarı çık
                DirectoryInfo currentDir = projectDir;
                for (int i = 0; i < 5; i++) // En fazla 5 seviye yukarı çık
                {
                    // Eğer python_kod klasörü bu seviyede varsa, bu SLF kök dizinidir
                    if (Directory.Exists(Path.Combine(currentDir.FullName, "python_kod")))
                    {
                        return currentDir.FullName;
                    }

                    // Bir üst dizine çık
                    if (currentDir.Parent == null) break;
                    currentDir = currentDir.Parent;
                }

                // Bulunamadıysa, proje dizinini döndür
                return projectDir.FullName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SLF kök dizini belirlenirken hata: {ex.Message}");
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// Python kod klasörü yapısını oluşturur
        /// </summary>
        private static void EnsurePythonKodStructure()
        {
            try
            {
                // Deep Learning klasör yapısı
                string deepLearningPath = Path.Combine(PythonKodDirectory, "deep_learning");
                string deepLearningCodePath = Path.Combine(deepLearningPath, "kod");
                string deepLearningTrainPath = Path.Combine(deepLearningPath, "ml_train");

                // İmar planları klasör yapısı
                string imarPlansPath = Path.Combine(PythonKodDirectory, "imar_analizi");
                string imarPlansCodePath = Path.Combine(imarPlansPath, "kod");
                string imarPlansDataPath = Path.Combine(imarPlansPath, "data");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Python kod alt klasörleri oluşturulurken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Python script dosyasının tam yolunu döndürür
        /// </summary>
        public static string GetPythonScriptPath(string relativePath)
        {
            // 1. İlk olarak orijinal Config yolunu dene
            if (!string.IsNullOrEmpty(_configPythonKodPath))
            {
                string configPath = Path.Combine(_configPythonKodPath, relativePath);
                if (File.Exists(configPath))
                {
                    return configPath;
                }
            }

            // 2. Temel dizin üzerinden oluşturulan yolu dene
            string basePath = Path.Combine(PythonKodDirectory, relativePath);
            if (File.Exists(basePath))
            {
                return basePath;
            }

            // 3. Hiçbiri çalışmazsa, en azından var olan bir dizin döndürmeye çalış
            if (!string.IsNullOrEmpty(_configPythonKodPath))
            {
                return Path.Combine(_configPythonKodPath, relativePath);
            }

            return basePath;
        }


        // İl/İlçe formatında tam yol
        public static string FullPath => !string.IsNullOrEmpty(SelectedCity) && !string.IsNullOrEmpty(SelectedDistrict)
            ? Path.Combine(SelectedCity, SelectedDistrict)
            : string.Empty;

        // Çalışma klasörü dahil tam yol
        public static string FullWorkingPath => !string.IsNullOrEmpty(CurrentWorkingFolder)
            ? Path.Combine(FullPath, CurrentWorkingFolder)
            : FullPath;

        // Python için komut satırı argümanı olarak kullanılabilecek path
        public static string CommandLinePathArg => $"--path=\"{FullWorkingPath}\"";

        // Alt klasör yolları
        public static string GirdilerPath => Path.Combine(BaseDirectory, FullWorkingPath, "girdiler");
        public static string ImarAnaliziPath => Path.Combine(BaseDirectory, FullWorkingPath, "imar_analizi_sonuclari");
        public static string SonuclarPath => Path.Combine(BaseDirectory, FullWorkingPath, "sonuclar");


        /// <summary>
        /// Path bilgisini günceller ve yeni bir geçici çalışma klasörü oluşturur
        /// </summary>
        /// <param name="city">İl adı</param>
        /// <param name="district">İlçe adı</param>
        /// <returns>İşlem başarılı olduysa true, değilse false</returns>
        public static bool UpdatePath(string city, string district)
        {
            try
            {
                // Parametre kontrolü
                if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(district))
                {
                    Debug.WriteLine("Path güncellenemedi: Geçersiz il veya ilçe.");
                    return false;
                }

                // Mevcut verileri temizle
                if (CurrentMode == WorkingMode.Temporary && !string.IsNullOrEmpty(CurrentWorkingFolder))
                {
                    CleanupCurrentFolder();
                }

                // Değişkenleri güncelle
                SelectedCity = city;
                SelectedDistrict = district;
                CurrentMode = WorkingMode.Temporary;
                CurrentWorkingFolder = null;

                // Yeni geçici çalışma klasörü oluştur
                bool result = CreateNewTempFolder();

                // Python kod yapısını oluştur
                EnsurePythonKodStructure();

                Debug.WriteLine($"Path güncellendi: {FullWorkingPath}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Path güncellenirken hata: {ex.Message}");
                return false;
            }
        }


        private static void CleanupCurrentFolder()
        {
            try
            {
                string oldFolderPath = Path.Combine(BaseDirectory, FullPath, CurrentWorkingFolder);

                if (Directory.Exists(oldFolderPath))
                {
                    // Klasörü silebilmek için dosya tanıtıcılarını temizle
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    try
                    {
                        Directory.Delete(oldFolderPath, true);
                        Debug.WriteLine($"Eski klasör temizlendi: {oldFolderPath}");
                    }
                    catch (IOException)
                    {
                        // Dosya işlemleri nedeniyle silemiyorsak, zorla silmeyi dene
                        ForceDeleteFolder(oldFolderPath);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // İzin sorunu varsa, zorla silmeyi dene
                        ForceDeleteFolder(oldFolderPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Mevcut klasör temizlenirken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Komut satırı kullanarak klasörü zorla siler
        /// </summary>
        /// <param name="folderPath">Silinecek klasör yolu</param>
        private static void ForceDeleteFolder(string folderPath)
        {
            try
            {
                using (var process = new System.Diagnostics.Process())
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C rd /s /q \"{folderPath}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        Debug.WriteLine($"Klasör başarıyla zorla silindi: {folderPath}");
                    }
                    else
                    {
                        Debug.WriteLine($"Klasör zorla silinemedi, çıkış kodu: {process.ExitCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Klasör zorla silinirken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Çalışma modunu güvenli şekilde değiştirir
        /// </summary>
        public static void SetMode(WorkingMode mode)
        {
            CurrentMode = mode;
        }

        /// <summary>
        /// Yeni bir geçici çalışma klasörü oluşturur
        /// </summary>
        public static bool CreateNewTempFolder()
        {
            // İl ve ilçe seçilip seçilmediğini kontrol et
            if (string.IsNullOrEmpty(SelectedCity) || string.IsNullOrEmpty(SelectedDistrict))
            {
                Debug.WriteLine("Geçici klasör oluşturulamadı: İl ve ilçe seçilmemiş.");
                return false;
            }

            // Eğer zaten geçici bir klasör varsa ve aynı il/ilçe için çalışıyorsak
            if (CurrentMode == WorkingMode.Temporary && !string.IsNullOrEmpty(CurrentWorkingFolder))
            {
                // Şu anki geçici klasör yolu
                string currentTempPath = Path.Combine(BaseDirectory, FullPath, CurrentWorkingFolder);

                // Eğer klasör hala varsa, yeni oluşturmaya gerek yok
                if (Directory.Exists(currentTempPath))
                {
                    Debug.WriteLine($"Zaten aktif bir geçici klasör var: {CurrentWorkingFolder}");
                    return true;
                }
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            CurrentWorkingFolder = $"proje_temp_{timestamp}";
            CurrentMode = WorkingMode.Temporary;

            // Klasör yapısını oluştur
            CreateFolderStructure();

            Debug.WriteLine($"Yeni geçici klasör oluşturuldu: {CurrentWorkingFolder}");
            return true;
        }


        /// <summary>
        /// Bir dizinin içerik barındırıp barındırmadığını kontrol eder
        /// </summary>
        /// <param name="path">Kontrol edilecek dizin yolu</param>
        /// <returns>İçerik varsa true, yoksa false</returns>
        private static bool DirectoryHasContent(string path)
        {
            if (!Directory.Exists(path))
                return false;

            // Alt klasörlerdeki dosya sayısı (girdiler, imar_analizi_sonuclari, sonuclar)
            int fileCount = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Length;

            // Sadece alt klasörler varsa içerik sayılmasın
            if (fileCount == 0)
            {
                // Alt klasörler dışında başka dosya yoksa
                return false;
            }

            return true;
        }


        /// <summary>
        /// Proje durumunu kaydeder
        /// </summary>
        private static void SaveProjectState(string projectPath)
        {
            try
            {
                // Tamamlanan modülleri GirdiModülü.dataTablesByType'dan al
                var completedModules = GirdiModülü.dataTablesByType.Keys.ToList();

                var projectState = new Dictionary<string, object>
                {
                    ["CompletedModules"] = completedModules,
                    ["LastSaved"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ["CreatedBy"] = Environment.UserName
                };

                string json = System.Text.Json.JsonSerializer.Serialize(projectState,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                string statePath = Path.Combine(projectPath, "project_state.json");
                File.WriteAllText(statePath, json);

                Debug.WriteLine($"Proje durumu kaydedildi: {statePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Proje durumu kaydedilirken hata: {ex.Message}");
            }
        }


        /// <summary>
        /// Mevcut bir projeyi açar
        /// </summary>
        /// <param name="projectName">Açılacak proje adı</param>
        public static string OpenProject(string projectName)
        {
            try
            {
                // Proje klasör adını oluştur
                string projectFolderName = $"proje_{projectName}";
                string projectPath = Path.Combine(BaseDirectory, FullPath, projectFolderName);

                // Proje klasörü yoksa oluştur
                if (!Directory.Exists(projectPath))
                {
                    Directory.CreateDirectory(projectPath);

                    // Directly create folders here instead of calling CreateProjectSubfolders
                    Directory.CreateDirectory(Path.Combine(projectPath, "Girdiler"));
                    Directory.CreateDirectory(Path.Combine(projectPath, "Sonuçlar"));
                    // Add other folders as needed
                }

                // Use our new method to set the mode
                SetMode(WorkingMode.Project);
                CurrentWorkingFolder = projectFolderName;

                Console.WriteLine($"Proje açıldı: {projectName}");
                Console.WriteLine($"Çalışma modu: {CurrentMode}");
                Console.WriteLine($"Çalışma klasörü: {CurrentWorkingFolder}");

                return projectPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Proje açılırken hata: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Klasör yapısını oluşturur
        /// </summary>
        private static void CreateFolderStructure()
        {
            // Ana klasörü oluştur
            string workingPath = Path.Combine(BaseDirectory, FullPath, CurrentWorkingFolder);
            Directory.CreateDirectory(workingPath);

            // Alt klasörleri oluştur
            Directory.CreateDirectory(GirdilerPath);
            Directory.CreateDirectory(ImarAnaliziPath);
            Directory.CreateDirectory(SonuclarPath);

            Debug.WriteLine($"Klasör yapısı oluşturuldu: {workingPath}");
        }

        /// <summary>
        /// Bir klasörü tüm içeriğiyle birlikte başka bir klasöre kopyalar
        /// </summary>
        private static void CopyDirectory(string sourceDir, string targetDir)
        {
            // Hedef dizini oluştur
            Directory.CreateDirectory(targetDir);

            // Tüm dosyaları kopyala
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destFile, true);
            }

            // Tüm alt dizinleri kopyala
            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(directory);
                CopyDirectory(directory, Path.Combine(targetDir, dirName));
            }
        }


        /// <summary>
        /// Belirtilen path'in geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <returns>Path geçerli ise true, değilse false</returns>
        public static bool IsValidPath()
        {
            return !string.IsNullOrEmpty(SelectedCity) && !string.IsNullOrEmpty(SelectedDistrict);
        }


        /// <summary>
        /// Veri tipi için tam girdiler klasör yolunu döndürür
        /// </summary>
        /// <param name="dataType">Veri tipi (örn. "DTR_Verileri")</param>
        /// <returns>Tam klasör yolu</returns>
        public static string GetGirdilerPathForDataType(string dataType)
        {
            string dataTypeFolder = dataType.Replace(" ", "_");
            string dataTypePath = Path.Combine(GirdilerPath, dataTypeFolder);

            // Klasör yoksa oluştur
            if (!Directory.Exists(dataTypePath))
            {
                Directory.CreateDirectory(dataTypePath);
            }

            return dataTypePath;
        }

        /// <summary>
        /// İmar analizi sonuçları için tam klasör yolunu döndürür
        /// </summary>
        /// <param name="analysisType">Analiz tipi (opsiyonel)</param>
        /// <returns>Tam klasör yolu</returns>
        public static string GetImarAnaliziPathForType(string analysisType = null)
        {
            if (string.IsNullOrEmpty(analysisType))
                return ImarAnaliziPath;

            string typePath = Path.Combine(ImarAnaliziPath, analysisType.Replace(" ", "_"));

            // Klasör yoksa oluştur
            if (!Directory.Exists(typePath))
            {
                Directory.CreateDirectory(typePath);
            }

            return typePath;
        }

        /// <summary>
        /// Sonuç tipi için sonuçlar klasör yolunu döndürür
        /// </summary>
        /// <param name="resultType">Sonuç tipi (opsiyonel)</param>
        /// <returns>Tam klasör yolu</returns>
        public static string GetSonuclarPathForType(string resultType = null)
        {
            if (string.IsNullOrEmpty(resultType))
                return SonuclarPath;

            string typePath = Path.Combine(SonuclarPath, resultType.Replace(" ", "_"));

            // Klasör yoksa oluştur
            if (!Directory.Exists(typePath))
            {
                Directory.CreateDirectory(typePath);
            }

            return typePath;
        }


        public static void ResetWorkingEnvironment()
        {
            try
            {
                // Geçici bir klasörde çalışılıyorsa ve klasör varsa
                if (CurrentMode == WorkingMode.Temporary && !string.IsNullOrEmpty(CurrentWorkingFolder))
                {
                    string tempPath = Path.Combine(BaseDirectory, FullPath, CurrentWorkingFolder);

                    try
                    {
                        // Klasör var mı kontrol et ve içeriğini temizle
                        if (Directory.Exists(tempPath))
                        {
                            Directory.Delete(tempPath, true);
                            Console.WriteLine($"Eski çalışma ortamı temizlendi: {tempPath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Eski çalışma klasörü temizlenirken hata: {ex.Message}");
                        // Hatayı yut ve devam et
                    }
                }

                // Değişkenleri temizle
                SelectedCity = null;
                SelectedDistrict = null;
                CurrentWorkingFolder = null;
                CurrentMode = WorkingMode.Temporary;

                Console.WriteLine("Çalışma ortamı sıfırlandı");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Çalışma ortamı sıfırlanırken hata: {ex.Message}");
            }
        }
    }
}