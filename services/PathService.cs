using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

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
        public static void SetMode(WorkingMode mode)
        {
            // This method can access the private setter
            CurrentMode = mode;
        }

        // Then in your ProjeEkleButton_Click method, change:
        // PathService.CurrentMode = PathService.WorkingMode.Project;
        // to:
        //PathService.SetMode(PathService.WorkingMode.Project);
        /// <summary>
        /// Yeni bir geçici çalışma klasörü oluşturur
        /// </summary>
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
        /// Belirtilen isme sahip bir proje klasörü oluşturur ve geçici klasörden verileri kopyalar
        /// </summary>
        /// <param name="projectName">Proje adı</param>
        /// <returns>Yeni oluşturulan proje klasörünün tam yolu</returns>
        /// /// <summary>
        /// Aktif geçici klasörü temizler
        /// </summary>
        /// <returns>İşlem başarılı olduysa true, değilse false</returns>
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
        /// 
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
        /// Çalışma ortamını sıfırlar ve tüm değişkenleri temizler.
        /// Yeni bir il/ilçe seçildiğinde kullanılır.
        /// </summary>
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
        /// <summary>
        /// Sonuçlar için tam klasör yolunu döndürür
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
    }
}