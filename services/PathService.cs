using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;

namespace SLF.Services
{
    /// <summary>
    /// Uygulama genelinde path yönetimi sağlayan servis sınıfı
    /// </summary>
    public static class PathService
    {
        // Proje klasörüne göre relatif il-ilçe kırılımı klasörü yolu
        private const string RELATIVE_DATA_PATH = @"il_ilce_kırılımları";

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

        /// <summary>
        /// Uygulama tarafından kullanılacak temel veri dizini
        /// </summary>
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

        /// <summary>
        /// Python kod klasörü yolu - SLF kök dizini altında
        /// </summary>
        public static string PythonKodDirectory
        {
            get
            {
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
        /// </summary>
        public static string DeepLearningTrainDirectory => Path.Combine(DeepLearningDirectory, "ml_train");

        /// <summary>
        /// İmar planları kod klasör yolu
        /// </summary>
        //public static string ImarPlansCodeDirectory => Path.Combine(ImarPlansDirectory, "kod");

        /// <summary>
        /// İmar planları veri seti klasör yolu
        /// </summary>
        /// 
        
        public static string ImarPlansDataDirectory => Path.Combine(ImarPlansDirectory, "data");
        // Hücre ve uydu verileri klasörlerinin yolları için özellikler
        // Bunlar il/ilçe klasöründe doğrudan bulunuyor (temp içinde değil)
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

                // Deep Learning alt klasörlerini oluştur
                if (!Directory.Exists(deepLearningPath))
                    Directory.CreateDirectory(deepLearningPath);

                if (!Directory.Exists(deepLearningCodePath))
                    Directory.CreateDirectory(deepLearningCodePath);

                if (!Directory.Exists(deepLearningTrainPath))
                    Directory.CreateDirectory(deepLearningTrainPath);

                // İmar planları alt klasörlerini oluştur
                if (!Directory.Exists(imarPlansPath))
                    Directory.CreateDirectory(imarPlansPath);

                if (!Directory.Exists(imarPlansCodePath))
                    Directory.CreateDirectory(imarPlansCodePath);

                if (!Directory.Exists(imarPlansDataPath))
                    Directory.CreateDirectory(imarPlansDataPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Python kod alt klasörleri oluşturulurken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Python script dosyasının tam yolunu döndürür
        /// </summary>
        public static string GetPythonScriptPath(string scriptName)
        {
            // Eğer main.py ise İmar Planları klasöründe ara
            if (scriptName.ToLower() == "main.py")
            {
                return Path.Combine(ImarPlansDirectory, scriptName);
            }
            // Diğer durumlarda DeepLearning klasöründe ara
            return Path.Combine(DeepLearningCodeDirectory, scriptName);
        }

        /// <summary>
        /// Eğitim veri setinin tam yolunu döndürür
        /// </summary>
        public static string GetTrainingDataPath(string datasetName)
        {
            return Path.Combine(DeepLearningTrainDirectory, datasetName);
        }

        /// <summary>
        /// İmar planları için ham veri dosyasının yolunu döndürür
        /// </summary>
        /// <param name="fileName">Dosya adı</param>
        /// <returns>Tam dosya yolu</returns>
        public static string GetImarPlansDataPath(string fileName)
        {
            return Path.Combine(ImarPlansDataDirectory, fileName);
        }

        /// <summary>
        /// KML dosyasını İmar planları veri klasörüne kopyalar
        /// </summary>
        /// <param name="sourcePath">Kaynak dosya yolu</param>
        /// <returns>Kopyalanan dosyanın hedef yolu</returns>
        public static string CopyKmlToImarPlansData(string sourcePath)
        {
            try
            {
                // Klasör yoksa oluştur
                if (!Directory.Exists(ImarPlansDataDirectory))
                {
                    Directory.CreateDirectory(ImarPlansDataDirectory);
                }

                string fileName = Path.GetFileName(sourcePath);
                string destinationPath = Path.Combine(ImarPlansDataDirectory, fileName);

                // Aynı isimde dosya varsa, benzersiz isim oluştur
                if (File.Exists(destinationPath))
                {
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string extension = Path.GetExtension(fileName);
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    destinationPath = Path.Combine(ImarPlansDataDirectory, $"{fileNameWithoutExt}_{timestamp}{extension}");
                }

                File.Copy(sourcePath, destinationPath, true);
                Debug.WriteLine($"KML dosyası kopyalandı: {destinationPath}");

                return destinationPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KML dosyası kopyalanırken hata: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// CSV dosyasını İmar planları veri klasörüne kopyalar
        /// </summary>
        /// <param name="sourcePath">Kaynak dosya yolu</param>
        /// <returns>Kopyalanan dosyanın hedef yolu</returns>
        public static string CopyCsvToImarPlansData(string sourcePath)
        {
            try
            {
                // Klasör yoksa oluştur
                if (!Directory.Exists(ImarPlansDataDirectory))
                {
                    Directory.CreateDirectory(ImarPlansDataDirectory);
                }

                string fileName = Path.GetFileName(sourcePath);
                string destinationPath = Path.Combine(ImarPlansDataDirectory, fileName);

                // Aynı isimde dosya varsa, benzersiz isim oluştur
                if (File.Exists(destinationPath))
                {
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string extension = Path.GetExtension(fileName);
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    destinationPath = Path.Combine(ImarPlansDataDirectory, $"{fileNameWithoutExt}_{timestamp}{extension}");
                }

                File.Copy(sourcePath, destinationPath, true);
                Debug.WriteLine($"CSV dosyası kopyalandı: {destinationPath}");

                return destinationPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CSV dosyası kopyalanırken hata: {ex.Message}");
                throw;
            }
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

        /// <summary>
        /// Mevcut çalışma klasörünü temizler (önceki klasörü siler)
        /// </summary>
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
            CurrentWorkingFolder = $"temp_{timestamp}";
            CurrentMode = WorkingMode.Temporary;

            // Klasör yapısını oluştur
            CreateFolderStructure();

            Debug.WriteLine($"Yeni geçici klasör oluşturuldu: {CurrentWorkingFolder}");
            return true;
        }

        /// <summary>
        /// Aktif geçici klasörü temizler
        /// </summary>
        /// <returns>İşlem başarılı olduysa true, değilse false</returns>
        /// <summary>
        /// SLF analizi için kullanılacak saturasyon dosyasının tam yolunu döndürür
        /// </summary>
        /// <returns>Saturasyon dosyasının tam yolu</returns>
        /// <summary>
        /// SLF analizi için kullanılacak saturasyon dosyasının tam yolunu döndürür
        /// </summary>
        /// <returns>Saturasyon dosyasının tam yolu</returns>
        public static string GetSaturasyonFilePath()
        {
            try
            {
                // İmar planları klasörü
                string imarPlanlariPath = GetImarAnaliziPathForType("imar_planlari");

                // Spesifik olarak belirtilen yol: imar_planlari/saturasyon/saturasyon_sonuc/saturasyon.csv
                string specificPath = Path.Combine(imarPlanlariPath, "saturasyon", "saturasyon_sonuc", "saturasyon.csv");

                // Öncelikle spesifik yolu kontrol et
                if (File.Exists(specificPath))
                {
                    Console.WriteLine($"Saturasyon dosyası bulundu (spesifik yol): {specificPath}");
                    return specificPath;
                }

                // Spesifik dosya bulunamadıysa, saturasyon alt klasörünü kontrol et
                string saturasyonFolderPath = Path.Combine(imarPlanlariPath, "saturasyon");
                if (Directory.Exists(saturasyonFolderPath))
                {
                    // saturasyon.csv dosyasını ara
                    string saturasyonFile = Path.Combine(saturasyonFolderPath, "saturasyon.csv");
                    if (File.Exists(saturasyonFile))
                    {
                        Console.WriteLine($"Saturasyon dosyası bulundu (alt klasör): {saturasyonFile}");
                        return saturasyonFile;
                    }

                    // Klasördeki tüm csv dosyalarını ara
                    var csvFiles = Directory.GetFiles(saturasyonFolderPath, "*.csv", SearchOption.AllDirectories);
                    if (csvFiles.Length > 0)
                    {
                        // En son oluşturulan csv dosyasını al
                        string latestFile = csvFiles.OrderByDescending(f => new FileInfo(f).LastWriteTime).First();
                        Console.WriteLine($"Saturasyon dosyası bulundu (en son csv): {latestFile}");
                        return latestFile;
                    }
                }

                // Hiçbir dosya bulunamadıysa
                Console.WriteLine("Saturasyon dosyası bulunamadı!");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Saturasyon dosyası aranırken hata: {ex.Message}");
                return null;
            }
        }
        public static bool CleanupCurrentTempFolder()
        {
            try
            {
                // Geçici moddaysak ve geçerli bir klasör varsa
                if (CurrentMode == WorkingMode.Temporary && !string.IsNullOrEmpty(CurrentWorkingFolder))
                {
                    string tempPath = Path.Combine(BaseDirectory, FullPath, CurrentWorkingFolder);

                    // Klasör varsa sil
                    if (Directory.Exists(tempPath))
                    {
                        // Önce klasörde içerik olup olmadığını kontrol et
                        bool hasContent = DirectoryHasContent(tempPath);

                        if (!hasContent)
                        {
                            // İçerik yoksa direkt sil
                            Directory.Delete(tempPath, true);
                            Debug.WriteLine($"Boş geçici klasör silindi: {tempPath}");
                            return true;
                        }
                        else
                        {
                            // İçerik varsa - opsiyonel olarak burada bir uyarı gösterilebilir
                            // veya direkt olarak silinebilir
                            Directory.Delete(tempPath, true);
                            Debug.WriteLine($"İçerik bulunan geçici klasör silindi: {tempPath}");
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Geçici klasör temizlenirken hata: {ex.Message}");
                return false;
            }
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
        /// Belirtilen isme sahip bir proje klasörü oluşturur ve geçici klasörden verileri kopyalar
        /// </summary>
        /// <param name="projectName">Proje adı</param>
        /// <returns>Yeni oluşturulan proje klasörünün tam yolu</returns>
        public static string SaveAsProject(string projectName)
        {
            if (CurrentMode == WorkingMode.Project)
            {
                throw new InvalidOperationException("Zaten bir proje klasöründe çalışıyorsunuz.");
            }

            string oldFolder = CurrentWorkingFolder;
            string projectFolderName = $"proje_{projectName}";

            string oldPath = Path.Combine(BaseDirectory, FullPath, oldFolder);
            string newPath = Path.Combine(BaseDirectory, FullPath, projectFolderName);

            // Dizin zaten varsa hata ver
            if (Directory.Exists(newPath))
            {
                throw new IOException($"'{projectName}' adında bir proje zaten var.");
            }

            // Klasör yapısını oluştur
            Directory.CreateDirectory(newPath);

            // Geçici klasörden proje klasörüne kopyala
            CopyDirectory(oldPath, newPath);

            // Aktif klasörü güncelle
            CurrentWorkingFolder = projectFolderName;
            CurrentMode = WorkingMode.Project;

            // Proje durum dosyasını oluştur
            SaveProjectState(newPath);

            Debug.WriteLine($"Proje kaydedildi: {projectFolderName}");

            return newPath;
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
        /// Proje alt klasörlerini oluşturur
        /// </summary>
        private static void CreateProjectSubfolders(string projectPath)
        {
            // Create standard subfolders for projects
            Directory.CreateDirectory(Path.Combine(projectPath, "Girdiler"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Sonuçlar"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Raporlar"));

            // Create module-specific folders in the Girdiler folder
            string girdilerPath = Path.Combine(projectPath, "Girdiler");
            Directory.CreateDirectory(Path.Combine(girdilerPath, "DTR_Verileri"));
            Directory.CreateDirectory(Path.Combine(girdilerPath, "Abone_Verileri"));
            Directory.CreateDirectory(Path.Combine(girdilerPath, "DEK_Verileri"));
            Directory.CreateDirectory(Path.Combine(girdilerPath, "EA_Sarj_Verileri"));
            // Add other module folders as needed
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
        /// İlçe için mevcut proje listesini döndürür
        /// </summary>
        /// <returns>Proje isimleri listesi</returns>
        public static List<string> GetProjectList()
        {
            List<string> projects = new List<string>();

            if (string.IsNullOrEmpty(SelectedCity) || string.IsNullOrEmpty(SelectedDistrict))
                return projects;

            string districtPath = Path.Combine(BaseDirectory, FullPath);

            if (!Directory.Exists(districtPath))
                return projects;

            // "proje_" ile başlayan tüm klasörleri bul
            string[] projectFolders = Directory.GetDirectories(districtPath, "proje_*");

            // Klasör isimlerinden "proje_" önekini çıkar
            foreach (string folder in projectFolders)
            {
                string folderName = Path.GetFileName(folder);
                if (folderName.StartsWith("proje_"))
                {
                    projects.Add(folderName.Substring(6)); // "proje_" önekini çıkar
                }
            }

            return projects;
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
                if (Directory.Exists(Path.Combine(currentDir, RELATIVE_DATA_PATH)))
                {
                    _baseDirectory = Path.Combine(currentDir, RELATIVE_DATA_PATH);
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
                        if (Directory.Exists(Path.Combine(currentDir, RELATIVE_DATA_PATH)))
                        {
                            _baseDirectory = Path.Combine(currentDir, RELATIVE_DATA_PATH);
                            foundDataFolder = true;
                            break;
                        }
                    }
                }

                // Hala bulunamadıysa, son çare olarak tam path'i dene
                if (!foundDataFolder)
                {
                    string gitRepoPath = @"C:\Users\batuhan.yetis\MRC\MRC - 1.1.3_T&SI\MRC2023-X_Jeo-Uzamsal Talep Tahmini Yazılımı";

                    if (Directory.Exists(Path.Combine(gitRepoPath, RELATIVE_DATA_PATH)))
                    {
                        _baseDirectory = Path.Combine(gitRepoPath, RELATIVE_DATA_PATH);
                        foundDataFolder = true;
                    }
                }

                // Veri klasörü bulunamadıysa, exe dizini altında yeni bir klasör oluştur
                if (!foundDataFolder)
                {
                    _baseDirectory = Path.Combine(exeDirectory, RELATIVE_DATA_PATH);
                    Directory.CreateDirectory(_baseDirectory);
                    Debug.WriteLine($"Veri klasörü bulunamadı, yeni klasör oluşturuldu: {_baseDirectory}");
                }

                Debug.WriteLine($"Veri klasörü yolu: {_baseDirectory}");
            }
            catch (Exception ex)
            {
                // Herhangi bir hata durumunda, exe dizini altında bir klasör kullan
                string fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RELATIVE_DATA_PATH);
                _baseDirectory = fallbackPath;

                if (!Directory.Exists(fallbackPath))
                {
                    Directory.CreateDirectory(fallbackPath);
                }

                Debug.WriteLine($"Veri klasörü belirlenirken hata oluştu: {ex.Message}");
                Debug.WriteLine($"Varsayılan klasör kullanılıyor: {fallbackPath}");
            }
        }

        /// <summary>
        /// Python betiği çalıştırır ve seçili path'i argüman olarak geçer
        /// </summary>
        /// <param name="scriptPath">Python betik dosyasının yolu</param>
        /// <param name="additionalArgs">Ek komut satırı argümanları</param>
        /// <returns>Python betiğinin çıktısı</returns>
        public static string RunPythonScript(string scriptPath, string additionalArgs = "")
        {
            string arguments = $"\"{scriptPath}\" {CommandLinePathArg} {additionalArgs}";
            string output = string.Empty;

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "python";  // veya "python3" Linux/macOS sistemlerinde
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Debug.WriteLine($"Python betiği çalıştırıldı: {scriptPath}");
                Debug.WriteLine($"Sonuç: {output}");

                return output;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Python betiği çalıştırılırken hata: {ex.Message}");
                throw new Exception($"Python betiği çalıştırılamadı: {ex.Message}", ex);
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
        /// Belirtilen path'in fiziksel olarak var olup olmadığını kontrol eder
        /// </summary>
        /// <returns>Fiziksel klasör varsa true, yoksa false</returns>
        public static bool DirectoryExists()
        {
            if (!IsValidPath())
                return false;

            string physicalPath = Path.Combine(BaseDirectory, FullWorkingPath);
            return Directory.Exists(physicalPath);
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

        /// <summary>
        /// Çalışma ortamını sıfırlar ve tüm değişkenleri temizler.PythonKodDirectory
        /// Yeni bir il/ilçe seçildiğinde kullanılır.
        /// </summary>
        /// 
        public static void DiagnosticTest()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"ExeDirectory: {exeDirectory}");

            // Bir üst dizine çık (bin/Debug'den bin'e)
            DirectoryInfo parentDir = Directory.GetParent(exeDirectory);
            Console.WriteLine($"ParentDir: {parentDir?.FullName}");

            // İki üst dizine çık (bin'den SLF'ye)
            DirectoryInfo projectDir = parentDir?.Parent;
            Console.WriteLine($"ProjectDir: {projectDir?.FullName}");

            // python_kod klasörünü ara
            string pythonKodPath = Path.Combine(projectDir?.FullName ?? "", "python_kod");
            Console.WriteLine($"Python kod yolu: {pythonKodPath}");
            Console.WriteLine($"Python kod klasörü var mı: {Directory.Exists(pythonKodPath)}");
        }
        /// <summary>
        /// SLF (Saturation Load Flow) için path ayarlaması yapar
        /// </summary>
        /// <param name="saturasyonFilePath">Saturasyon dosyasının yolu</param>
        /// <returns>İşlem başarılı olduysa true, değilse false</returns>
        public static bool SetSLFPath(string saturasyonFilePath)
        {
            try
            {
                // Parametre kontrolü
                if (string.IsNullOrEmpty(saturasyonFilePath) || !File.Exists(saturasyonFilePath))
                {
                    Debug.WriteLine("SLF path ayarlanamadı: Geçersiz dosya yolu.");
                    return false;
                }

                // Saturasyon dosyasından il ve ilçe bilgisini çıkar
                string fileName = Path.GetFileName(saturasyonFilePath);

                // Dosya adından il ve ilçe bilgisini çıkarmak için varsayılan bir format kullan
                // Örnek: "saturasyon_izmir_cigli.csv" veya benzer bir formatta olduğunu varsay
                string[] parts = fileName.ToLower().Replace("saturasyon_", "").Replace(".csv", "").Split('_');

                if (parts.Length < 2)
                {
                    Debug.WriteLine("SLF path ayarlanamadı: Dosya adından il/ilçe çıkarılamadı.");
                    return false;
                }

                // İlk eleman il, ikinci eleman ilçe olarak kabul edilir
                string city = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[0]);
                string district = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[1]);

                // Path'i güncelle
                bool pathUpdateResult = UpdatePath(city, district);

                if (!pathUpdateResult)
                {
                    Debug.WriteLine("SLF path ayarlanamadı: Path güncellenemedi.");
                    return false;
                }

                // Saturasyon dosyasını ilgili klasöre kopyala
                string saturasyonOutputPath = GetImarAnaliziPathForType("slf_sonuclari");
                string destinationPath = Path.Combine(saturasyonOutputPath, fileName);

                // Dosyayı kopyala (varsa üzerine yaz)
                File.Copy(saturasyonFilePath, destinationPath, true);

                Debug.WriteLine($"SLF path ayarlandı: {city}/{district}");
                Debug.WriteLine($"Saturasyon dosyası kopyalandı: {destinationPath}");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SLF path ayarlanırken hata: {ex.Message}");
                return false;
            }
        }
        public static string GetImarOraniFilePath()
        {
            try
            {
                // İmar planları klasörü
                string imarPlanlariPath = GetImarAnaliziPathForType("imar_planlari");

                // Saturasyon girdiler klasörü - öncelikli olarak burada arayacağız
                string saturasyonGirdilerPath = Path.Combine(imarPlanlariPath, "saturasyon", "girdiler");

                // Bina kırılımları dosyasını arayacağımız klasörler (öncelik sırasına göre)
                string[] searchFolders = new string[]
                {
            saturasyonGirdilerPath,
            Path.Combine(imarPlanlariPath, "saturasyon"),
            imarPlanlariPath,
            Path.Combine(imarPlanlariPath, "bina_kirilimlari")
                };

                // Dosya isimleri için arama desenleri
                string[] searchPatterns = new string[]
                {
            $"*bina_kirilimlari*.csv",
            $"*bina_kirilimlari*.xlsx",
            $"*{RemoveDiacritics(SelectedCity)}*{RemoveDiacritics(SelectedDistrict)}*bina*.csv",
            $"*{RemoveDiacritics(SelectedCity)}*{RemoveDiacritics(SelectedDistrict)}*bina*.xlsx"
                };

                Console.WriteLine($"İmar oranı dosyası aranıyor...");

                // Tüm klasörlerde, tüm desenleri ara
                foreach (var folder in searchFolders)
                {
                    if (!Directory.Exists(folder))
                    {
                        Console.WriteLine($"Klasör bulunamadı: {folder}");
                        continue;
                    }

                    Console.WriteLine($"Klasörde arama yapılıyor: {folder}");

                    foreach (var pattern in searchPatterns)
                    {
                        var files = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories);

                        if (files.Length > 0)
                        {
                            // En son oluşturulan dosyayı al
                            string latestFile = files.OrderByDescending(f => new FileInfo(f).LastWriteTime).First();
                            Console.WriteLine($"İmar oranı dosyası bulundu: {latestFile}");
                            return latestFile; // Return the most recent file found
                        }
                    }
                }

                // Hiçbir dosya bulunamadıysa
                Console.WriteLine("İmar oranı dosyası bulunamadı!");
                return null; // Return null if no file is found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"İmar oranı dosyası aranırken hata: {ex.Message}");
                return null; // Return null if an error occurs
            }
        }
        /// <summary>
        /// Türkçe karakterleri ASCII karşılıklarına dönüştürür
        /// </summary>
        /// <param name="text">Dönüştürülecek metin</param>
        /// <returns>ASCII karakterli metin</returns>
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Özel Türkçe karakterler için ek dönüşümler
            return stringBuilder.ToString()
                .Normalize(NormalizationForm.FormC)
                .Replace('ı', 'i')
                .Replace('İ', 'I')
                .Replace('ğ', 'g')
                .Replace('Ğ', 'G')
                .Replace('ü', 'u')
                .Replace('Ü', 'U')
                .Replace('ş', 's')
                .Replace('Ş', 'S')
                .Replace('ç', 'c')
                .Replace('Ç', 'C')
                .Replace('ö', 'o')
                .Replace('Ö', 'O');
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