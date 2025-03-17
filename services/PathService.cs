using System;
using System.IO;
using System.Diagnostics;

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
        private static string _baseDirectory;

        // Seçilen il
        public static string SelectedCity { get; set; }

        // Seçilen ilçe
        public static string SelectedDistrict { get; set; }

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

        // Python için komut satırı argümanı olarak kullanılabilecek path
        public static string CommandLinePathArg => $"--path=\"{FullPath}\"";

        /// <summary>
        /// Path bilgisini günceller
        /// </summary>
        /// <param name="city">İl adı</param>
        /// <param name="district">İlçe adı</param>
        public static void UpdatePath(string city, string district)
        {
            SelectedCity = city;
            SelectedDistrict = district;
            Debug.WriteLine($"Path güncellendi: {FullPath}");
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
        /// 

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

            string physicalPath = Path.Combine(BaseDirectory, FullPath);
            return Directory.Exists(physicalPath);
        }
    }
}