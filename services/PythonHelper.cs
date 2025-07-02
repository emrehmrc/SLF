using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using Newtonsoft.Json;
using SLF.Services;
using static Guna.UI2.Native.WinApi;

namespace SLF.services
{
    public static class PythonHelper
    {

        public static string RunDeepLearningModel()
        {
            try
            {

                

                // Seçilen il/ilçe bilgilerini al
                string selectedCity = PathService.SelectedCity;
                string selectedDistrict = PathService.SelectedDistrict;

                if (string.IsNullOrEmpty(selectedCity) || string.IsNullOrEmpty(selectedDistrict))
                {
                    throw new Exception("İl ve ilçe seçimi yapılmadan model çalıştırılamaz.");
                }

                // YearService'ten lastYear bilgisini al
                var yearService = YearService.GetInstance();
                string year = yearService.slfStartYear.ToString();

                Console.WriteLine($"Deep Learning model çalıştırılıyor: {selectedCity}/{selectedDistrict}, LastYear: {year}");

                // Python script yolu - artık python_kod klasöründen alınıyor
                string scriptRelativePath = Path.Combine("python_kod", "deep_learning", "kod", "model_learning.py");
                string pythonScriptPath = PathService.GetPythonScriptPath(scriptRelativePath);

                if (!File.Exists(pythonScriptPath))
                {
                    throw new Exception($"Python script bulunamadı: {pythonScriptPath}");
                }

                // Abone verisi giriş yolu (Son yüklenen dosya)
                string girdilerPath = PathService.GetGirdilerPathForDataType("Abone Verileri");
                string aboneVeriYolu = Directory.GetFiles(girdilerPath, "*.csv")
                                              .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                                              .FirstOrDefault();

                if (string.IsNullOrEmpty(aboneVeriYolu))
                {
                    throw new Exception("Abone verisi bulunamadı. Lütfen önce abone verilerini yükleyin.");
                }

                // Çıktı klasörü yolları
                string imarAnaliziPath = PathService.GetImarAnaliziPathForType("deep_learning_modeli");

                // Klasörü oluştur (yoksa)
                if (!Directory.Exists(imarAnaliziPath))
                {
                    Directory.CreateDirectory(imarAnaliziPath);
                }

                // Çıktı dosya yolları
                string meskenSonucYolu = Path.Combine(imarAnaliziPath, $"mesken_data_{selectedCity}_{selectedDistrict}.csv");
                string otherSonucYolu = Path.Combine(imarAnaliziPath, $"other_data_{selectedCity}_{selectedDistrict}.csv");

                // Python argümanlarını oluştur - İlçe parametresi eklendi
                string arguments = $"/C python \"{pythonScriptPath}\" \"{aboneVeriYolu}\" \"{meskenSonucYolu}\" \"{otherSonucYolu}\" \"{selectedCity}\" \"{selectedDistrict}\" \"{year}\"";

                // Python betiğini çalıştır
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = arguments,
                    RedirectStandardOutput = false, // Output will be displayed in the console
                    RedirectStandardError = false,  // Errors will be displayed in the console
                    UseShellExecute = false,
                    CreateNoWindow = false
                };

                Process process = new Process
                {
                    StartInfo = processInfo
                };

                process.Start();

                // İşlemin tamamlanmasını bekle
                process.WaitForExit();

                // Hata durumunda
                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Python betiği hata ile sonlandı. Hata kodunu ve komut penceresindeki mesajları kontrol edin.");
                    // Hata mesajını görmek için pencerenin açık kalmasını sağladık, ama hatayı üst katmana iletiyoruz.
                    throw new Exception($"Python betiği hata ile sonlandı. Komut penceresindeki hata mesajlarını kontrol edin.");
                }

                // Process nesnesini kapat
                process.Close();

                // İşlem başarılı mesajı
                Console.WriteLine("Deep Learning modeli başarıyla çalıştırıldı.");

                // Since output is displayed in the console, return an empty string or a success message
                return "Deep Learning modeli başarıyla çalıştırıldı.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deep Learning modeli çalıştırılırken hata: {ex.Message}");
                throw; // Üst seviye metodların hatayı yakalaması için yeniden fırlat
            }
        }

        public static string RunKatmanDeneme(string kmlFilePath, string csvFilePath)
        {
            try
            {
                // Check if the inputs are valid
                if (string.IsNullOrEmpty(kmlFilePath) || !File.Exists(kmlFilePath))
                {
                    throw new Exception("Geçerli bir KML dosyası belirtilmelidir.");
                }

                if (string.IsNullOrEmpty(csvFilePath) || !File.Exists(csvFilePath))
                {
                    throw new Exception("Geçerli bir CSV dosyası belirtilmelidir.");
                }

                Console.WriteLine($"Katman Deneme Python scripti çalıştırılıyor:");
                Console.WriteLine($"KML: {kmlFilePath}");
                Console.WriteLine($"CSV: {csvFilePath}");

                // Get the script path
                string pythonScriptPath = PathService.KatmanDenemePath;

                if (!File.Exists(pythonScriptPath))
                {
                    throw new Exception($"Katman Deneme Python script bulunamadı: {pythonScriptPath}");
                }

                // Output file path - in the same directory as KML file
                string outputDirectory = Path.GetDirectoryName(kmlFilePath);
                string outputFile = Path.Combine(outputDirectory, "katman_analiz_sonuc.csv");

                string arguments = $"/C python \"{pythonScriptPath}\" \"{csvFilePath}\" \"{kmlFilePath}\" \"{outputFile}\"";

                // Run the Python script using cmd.exe
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = arguments,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WorkingDirectory = Path.GetDirectoryName(pythonScriptPath)
                };

                Process process = new Process
                {
                    StartInfo = processInfo
                };

                process.Start();

                // İşlemin tamamlanmasını bekle
                process.WaitForExit();

                // Hata durumunda
                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Python betiği hata ile sonlandı. Hata kodunu ve komut penceresindeki mesajları kontrol edin.");
                    // Hata mesajını görmek için pencerenin açık kalmasını sağladık, ama hatayı üst katmana iletiyoruz.
                    throw new Exception($"Python betiği hata ile sonlandı. Komut penceresindeki hata mesajlarını kontrol edin.");
                }

                // Process nesnesini kapat
                process.Close();

                // Return the output file path for further processing
                return outputFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Katman Deneme scripti çalıştırılırken hata: {ex.Message}");
                throw; // Re-throw for upper level methods to catch
            }
        }

        /// <summary>
        /// Abone verisi oluşturma işlemi - Akıllı mod
        /// Veri durumuna göre CSV'den veya veritabanından çalışır
        /// </summary>
        public static void RunPythonScriptForAboneVerisi(string configPath)
        {
            try
            {
                // Önce veri doğrulaması yap
                var validationResult = DataValidationService.ValidateAboneData(configPath);

                if (validationResult.IsValid && validationResult.HasDepoCsvFiles && validationResult.IsDataUpToDate)
                {
                    // Veriler güncel ve CSV'ler mevcut - CSV'lerle işlem yap
                    Console.WriteLine("Veriler güncel ve CSV dosyaları mevcut. CSV'lerden abone verisi oluşturuluyor...");
                    ProcessAboneDataFromCsv(configPath, validationResult.DepoPath);
                }
                else
                {
                    // Veriler güncel değil veya CSV'ler yok - Veritabanından çek
                    Console.WriteLine("Veriler güncel değil veya CSV dosyaları yok. Veritabanından yeni veri çekiliyor...");
                    ProcessAboneDataFromDatabase(configPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Abone verisi işlemi sırasında hata: {ex.Message}");
                throw new Exception($"Abone verisi işlemi başarısız: {ex.Message}", ex);
            }
        }


        public static void ProcessAboneDataFromCsv(string configPath, string depoPath)
        {
            try
            {
                // CSV dosya yolları
                string aboneCsvPath = Path.Combine(depoPath, "DWH_MRC_SLFPROJE_ABN_BLG.csv");
                string tuketimCsvPath = Path.Combine(depoPath, "DWH_MRC_SLFPROJE_TUKETIM.csv");

                // Dosyaların varlığını kontrol et
                if (!File.Exists(aboneCsvPath))
                {
                    throw new FileNotFoundException($"Abone bilgi CSV dosyası bulunamadı: {aboneCsvPath}");
                }

                if (!File.Exists(tuketimCsvPath))
                {
                    throw new FileNotFoundException($"Tüketim CSV dosyası bulunamadı: {tuketimCsvPath}");
                }

                // Python script'ini CSV modunda çalıştır
                string pythonScript = PathService._configveritabanikod;

                // Python script dosyasının varlığını kontrol et
                if (string.IsNullOrEmpty(pythonScript) || !File.Exists(pythonScript))
                {
                    throw new FileNotFoundException($"Python kod dosyası bulunamadı: {pythonScript}");
                }

                // Çalışma dizininin varlığını kontrol et
                string workingDirectory = Path.GetDirectoryName(pythonScript);
                if (string.IsNullOrEmpty(workingDirectory) || !Directory.Exists(workingDirectory))
                {
                    throw new DirectoryNotFoundException($"Çalışma dizini bulunamadı: {workingDirectory}");
                }

                // configPath'in varlığını kontrol et (dosya olarak kontrol et, directory değil)
                if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
                {
                    throw new FileNotFoundException($"Config dosyası bulunamadı: {configPath}");
                }

                // Komut dizesini oluştururken tüm yolları çift tırnak içine al
                // Python komutunu şu formatta oluştur: python "path/to/code.py" "path/to/config.json" --mode CSV ...
                string arguments = $"/C python \"{pythonScript}\" \"{configPath}\" --mode CSV --abone-csv \"{aboneCsvPath}\" --tuketim-csv \"{tuketimCsvPath}\"";

                // Python sürecini cmd.exe ile çalıştır
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = arguments,
                    UseShellExecute = true,       // Kabuk kullanarak çalıştır, pencereyi göster
                    RedirectStandardOutput = false,  // Çıktıyı yönlendirme, cmd penceresinde göster
                    RedirectStandardError = false,   // Hataları yönlendirme, cmd penceresinde göster
                    CreateNoWindow = false,       // Pencere oluştur
                    WorkingDirectory = workingDirectory,
                    WindowStyle = ProcessWindowStyle.Normal  // Pencereyi normal boyutta aç
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    // İşlemin tamamlanmasını bekle
                    process.WaitForExit();

                    // Hata durumunda
                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"CSV işleme sırasında hata oluştu. Hata kodu: {process.ExitCode}. Cmd penceresindeki mesajları kontrol edin.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CSV işleme hatası: {ex.Message}");
                throw;
            }
        }


        public static void ProcessAboneDataFromDatabase(string configPath)
        {
            try
            {
                Console.WriteLine("Veritabanından veri çekme ve CSV güncelleme işlemi başlatılıyor...");


                // Python kod dosyasının yolunu PathService'ten al
                string pythonScript = PathService._configveritabanikod;
                Console.WriteLine($"Config'den alınan Python kod yolu: {pythonScript}");

                // Dosya yolunu düzgün formata getir
                if (!string.IsNullOrEmpty(pythonScript))
                {
                    pythonScript = pythonScript.Replace('\\', '/').Replace('/', '\\');
                    Console.WriteLine($"Düzenlenen Python kodu yolu: {pythonScript}");
                }

                // Dosyanın var olup olmadığını kontrol et
                if (string.IsNullOrEmpty(pythonScript) || !File.Exists(pythonScript))
                {
                    throw new FileNotFoundException($"Python kod dosyası bulunamadı! Aranan konum: {pythonScript}");
                }

                Console.WriteLine($"Python kodu çalıştırılıyor: {pythonScript}");
                Console.WriteLine($"Config dosyası: {configPath}");

                // Python sürecini başlat - "DATABASE" modunda çalışacak, cmd.exe ile çalıştır
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C python \"{pythonScript}\" \"{configPath}\" --mode DATABASE",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = false,
                    WorkingDirectory = Path.GetDirectoryName(pythonScript)
                };

                Process process = new Process
                {
                    StartInfo = startInfo
                };

                process.Start();

                // İşlemin tamamlanmasını bekle
                process.WaitForExit();

                // Hata durumunda
                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Python kodu çalıştırılırken hata oluştu. Hata kodunu ve komut penceresindeki mesajları kontrol edin.");
                    // Hata mesajını görmek için pencerenin açık kalmasını sağladık, bu yüzden burada throw yapıyoruz ama pencere kapanmayacak.
                    throw new Exception($"Python kodu çalıştırılırken hata oluştu. Komut penceresindeki hata mesajlarını kontrol edin.");
                }

                // Process nesnesini kapat
                process.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veritabanından veri çekme işlemi hatası: {ex.Message}");
                throw;
            }
        }

        private static void DeepLearningRuns()
        {
            try
            {

                // Gerekli kontroller (Abone verisi yüklü mü, il-ilçe seçilmiş mi)
                if (string.IsNullOrEmpty(PathService.SelectedCity) || string.IsNullOrEmpty(PathService.SelectedDistrict))
                {
                    MessageBox.Show("Lütfen önce il ve ilçe seçimini yapın.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!GirdiModülü.dataTablesByType.ContainsKey("Abone Verileri"))
                {
                    MessageBox.Show("Lütfen önce Abone Verileri'ni yükleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Deep Learning modelini çalıştır
                string result = RunDeepLearningModel();

                // İşlem tamamlandığında başarı mesajı göster
                //MessageBox.Show("İmar analizi başarıyla tamamlandı.\nSonuçlar 'imar_analizi_sonuclari/deep_learning_modeli' klasöründe kaydedildi.",
                  //              "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        public static void Dtr_Bağlantısallık()
        {
            string girdilerPath = PathService.GetGirdilerPathForDataType("Abone Verileri");
            string aboneVeriYolu = Directory.GetFiles(girdilerPath, "*.csv")
                                          .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                                          .FirstOrDefault();

            string dtrModuluPath = PathService.GetGirdilerPathForDataType("DTR Verileri");
            string dtrModuluFilePath = Directory.GetFiles(dtrModuluPath, "*.csv")
                                                .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                                                .FirstOrDefault();

            var yearService = YearService.GetInstance();
            string year = yearService.slfStartYear.ToString();

            string pythonScriptPath = !string.IsNullOrEmpty(PathService.dtr_bağlantısallıkPath)
                    ? PathService.dtr_bağlantısallıkPath
                    : PathService.GetPythonScriptPath("main.py");

            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{pythonScriptPath}\" \"{aboneVeriYolu}\" \"{dtrModuluFilePath}\" \"{year}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };


            using (Process process = Process.Start(processInfo))
            {

                // İşlem durumunu izleme ve ilerleme raporu
                DateTime startTime = DateTime.Now;

                // İlerleme raporlama için bir Timer başlat
                System.Timers.Timer progressTimer = new System.Timers.Timer(30000); // 30 saniyede bir rapor
                progressTimer.Elapsed += (sender, e) =>
                {
                    TimeSpan elapsed = DateTime.Now - startTime;
                    Console.WriteLine($"İşlem devam ediyor... Geçen süre: {elapsed.Minutes} dakika {elapsed.Seconds} saniye");
                };
                progressTimer.AutoReset = true;
                progressTimer.Start();

                try
                {
                    // İşlemin tamamlanmasını sonsuza kadar bekle (zaman kısıtlaması yok)
                    process.WaitForExit();

                    // İşlem tamamlandı, çıkış kodunu kontrol et
                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Python betiği hata ile sonlandı. Çıkış kodu: {process.ExitCode}");
                    }
                }

                finally
                {
                    // Her durumda Timer'ı durdur
                    progressTimer.Stop();
                    progressTimer.Dispose();
                }
            }

        }

        public static void RunImarPlanModel(string kmlFilePath, string csvFilePath = null)
        {
            try

            {
                DeepLearningRuns();

                Dtr_Bağlantısallık();

                // Hücre verisi yolunu al - SHP ya da CSV dosyasını bul
                string hucrePath = PathService.HucrePath;
                string hucreFilePath = null;
                string uyduVeriPath = PathService.UyduVerileriPath;
                string uyduVeriFilePath = null;
                var yearService = YearService.GetInstance();
                string year = yearService.slfStartYear.ToString();
                
                // Seçilen il/ilçe bilgilerini al
                string selectedCity = PathService.SelectedCity;
                string selectedDistrict = PathService.SelectedDistrict;

                // DTR Modülü ve Yeni Projelendirilmiş DTR Modülü verilerini al
                string dtrModuluPath = PathService.GetGirdilerPathForDataType("DTR Verileri");
                string dtrModuluFilePath = null;
                string yeniDtrModuluPath = PathService.GetGirdilerPathForDataType("Yeni Projelendirilmiş DTR Verileri");
                string yeniDtrModuluFilePath = null;

                // Hücre dosyasını bul
                if (Directory.Exists(hucrePath))
                {
                    string[] hucreFiles = Directory.GetFiles(hucrePath, "*.shp");
                    if (hucreFiles.Length > 0)
                    {
                        hucreFilePath = Path.Combine(hucrePath, $"{selectedDistrict}_grid.shp"); // İlk bulunan SHP dosyasını kullan
                        Console.WriteLine($"Hücre SHP dosyası bulundu: {hucreFilePath}");
                    }
                    else
                    {
                        Console.WriteLine("Hücre klasöründe SHP dosyası bulunamadı.");
                    }
                }
                else
                {
                    Console.WriteLine($"Hücre klasörü bulunamadı: {hucrePath}");
                }

                // Uydu verisi dosyasını bul
                if (Directory.Exists(uyduVeriPath))
                {
                    string[] uyduFiles = Directory.GetFiles(uyduVeriPath, "*.csv");
                    if (uyduFiles.Length > 0)
                    {
                        uyduVeriFilePath = uyduFiles[0]; // İlk bulunan CSV dosyasını kullan
                        Console.WriteLine($"Uydu verisi CSV dosyası bulundu: {uyduVeriFilePath}");
                    }
                    else
                    {
                        Console.WriteLine("Uydu verileri klasöründe CSV dosyası bulunamadı.");
                    }
                }

                // DTR Modülü dosyasını bul
                if (Directory.Exists(dtrModuluPath))
                {
                    string[] dtrFiles = Directory.GetFiles(dtrModuluPath, "*.csv");
                    if (dtrFiles.Length > 0)
                    {
                        dtrModuluFilePath = dtrFiles[0]; // İlk bulunan CSV dosyasını kullan
                        Console.WriteLine($"DTR Modülü verisi bulundu: {dtrModuluFilePath}");
                    }
                    else
                    {
                        Console.WriteLine("DTR Modülü klasöründe CSV dosyası bulunamadı.");
                    }
                }
                else
                {
                    Console.WriteLine($"DTR Modülü klasörü bulunamadı: {dtrModuluPath}");
                }

                // Yeni Projelendirilmiş DTR Modülü dosyasını bul
                if (Directory.Exists(yeniDtrModuluPath))
                {
                    string[] yeniDtrFiles = Directory.GetFiles(yeniDtrModuluPath, "*.csv");
                    if (yeniDtrFiles.Length > 0)
                    {
                        yeniDtrModuluFilePath = yeniDtrFiles[0]; // İlk bulunan CSV dosyasını kullan
                        Console.WriteLine($"Yeni Projelendirilmiş DTR Modülü verisi bulundu: {yeniDtrModuluFilePath}");
                    }
                    else
                    {
                        Console.WriteLine("Yeni Projelendirilmiş DTR Modülü klasöründe CSV dosyası bulunamadı.");
                    }
                }
                else
                {
                    Console.WriteLine($"Yeni Projelendirilmiş DTR Modülü klasörü bulunamadı: {yeniDtrModuluPath}");
                }

                if (string.IsNullOrEmpty(selectedCity) || string.IsNullOrEmpty(selectedDistrict))
                {
                    throw new Exception("İl ve ilçe seçimi yapılmadan model çalıştırılamaz.");
                }

                Console.WriteLine($"İmar Planı modeli çalıştırılıyor: {selectedCity}/{selectedDistrict}");

                // Python script yolu 
                string pythonScriptPath = !string.IsNullOrEmpty(PathService._configImarAnaliziPath)
                    ? PathService._configImarAnaliziPath
                    : PathService.GetPythonScriptPath("main.py");

                if (!File.Exists(pythonScriptPath))
                {
                    // Alternatif yolları dene
                    string altPath = Path.Combine(PathService.PythonKodDirectory, "imar_analizi", "main.py");
                    if (File.Exists(altPath))
                    {
                        pythonScriptPath = altPath;
                        Console.WriteLine($"Alternatif İmar Analizi yolu kullanılıyor: {pythonScriptPath}");
                    }
                    else
                    {
                        throw new Exception($"İmar Analizi Python script bulunamadı: {pythonScriptPath}");
                    }
                }

                // Çıktı klasörü yolları - Ana çalışma klasörü
                string imarAnaliziPath = PathService.GetImarAnaliziPathForType("imar_planlari");
                Console.WriteLine($"İmar analizi çıktı klasörü: {imarAnaliziPath}");

                // Klasörü oluştur (yoksa)
                if (!Directory.Exists(imarAnaliziPath))
                {
                    Directory.CreateDirectory(imarAnaliziPath);
                }

                // Çıktı dosya yolları - DL modelindeki yaklaşıma benzer
                string outputPrefix = $"{selectedDistrict}";
                string outputCsvPath = Path.Combine(imarAnaliziPath, $"{outputPrefix}.csv");
                string outputKmlPath = Path.Combine(imarAnaliziPath, $"{outputPrefix}.kml");
                string tempDirPath = Path.Combine(imarAnaliziPath, "temp");

                // Temp klasörü
                if (!Directory.Exists(tempDirPath))
                {
                    Directory.CreateDirectory(tempDirPath);
                }

                // YENİ: Deep Learning Klasöründen mesken ve other dosyalarını bul
                string meskenFile = null;
                string otherFile = null;

                // Deep Learning modeli klasörü
                string deepLearningPath = PathService.GetImarAnaliziPathForType("deep_learning_modeli");
                if (Directory.Exists(deepLearningPath))
                {
                    // Küçük harfe çevirip daha kesin dosya araması yapıyoruz
                    string cityLower = selectedCity.ToLower();
                    string districtLower = selectedDistrict.ToLower();

                    // Mesken dosyasını bul
                    string[] meskenFiles = Directory.GetFiles(deepLearningPath, $"mesken_data_*{cityLower}*{districtLower}*.csv");
                    if (meskenFiles.Length > 0)
                    {
                        meskenFile = meskenFiles[0];
                        Console.WriteLine($"Mesken veri dosyası bulundu: {meskenFile}");
                    }
                    else
                    {
                        // Tam eşleşme bulunamazsa daha genel bir arama yap
                        meskenFiles = Directory.GetFiles(deepLearningPath, "mesken_data_*.csv");
                        if (meskenFiles.Length > 0)
                        {
                            meskenFile = meskenFiles[0];
                            Console.WriteLine($"Mesken veri dosyası (genel arama ile) bulundu: {meskenFile}");
                        }
                    }

                    // Other dosyasını bul
                    string[] otherFiles = Directory.GetFiles(deepLearningPath, $"other_data_*{cityLower}*{districtLower}*.csv");
                    if (otherFiles.Length > 0)
                    {
                        otherFile = otherFiles[0];
                        Console.WriteLine($"Diğer bina veri dosyası bulundu: {otherFile}");
                    }
                    else
                    {
                        // Tam eşleşme bulunamazsa daha genel bir arama yap
                        otherFiles = Directory.GetFiles(deepLearningPath, "other_data_*.csv");
                        if (otherFiles.Length > 0)
                        {
                            otherFile = otherFiles[0];
                            Console.WriteLine($"Diğer bina veri dosyası (genel arama ile) bulundu: {otherFile}");
                        }
                    }
                }

                // Log mesajı oluştur
                Console.WriteLine($"Python kod klasörü: {PathService.PythonKodDirectory}");
                Console.WriteLine($"Python script: {pythonScriptPath}");
                Console.WriteLine($"KML dosyası: {kmlFilePath}");
                Console.WriteLine($"Mesken veri dosyası: {meskenFile ?? "Bulunamadı"}");
                Console.WriteLine($"Other veri dosyası: {otherFile ?? "Bulunamadı"}");
                Console.WriteLine($"DTR Modülü verisi: {dtrModuluFilePath ?? "Bulunamadı"}");
                Console.WriteLine($"Yeni Projelendirilmiş DTR Modülü verisi: {yeniDtrModuluFilePath ?? "Bulunamadı"}");
                Console.WriteLine($"CSV çıktı dosyası: {outputCsvPath}");
                Console.WriteLine($"KML çıktı dosyası: {outputKmlPath}");

                // Argümanları oluştur
                StringBuilder args = new StringBuilder();
                args.Append($"/C python \"{pythonScriptPath}\" process \"{selectedCity}\" \"{kmlFilePath}\"");
                args.Append($" --district \"{selectedDistrict}\"");
                args.Append($" --output-dir \"{imarAnaliziPath}\"");  // Ana çıktı klasörü
                args.Append($" --output-prefix \"{outputPrefix}\"");
                args.Append($" --Year \"{year}\"");

                // Hücre verisi dosyasını ekle
                if (!string.IsNullOrEmpty(hucreFilePath))
                {
                    args.Append($" --hucre-data \"{hucreFilePath}\"");
                }

                // YENİ: Mesken ve Other dosyalarını ekle
                if (!string.IsNullOrEmpty(meskenFile))
                {
                    args.Append($" --mesken-file \"{meskenFile}\"");
                }

                if (!string.IsNullOrEmpty(otherFile))
                {
                    args.Append($" --other-file \"{otherFile}\"");
                }

                // YENİ: DTR Modülü ve Yeni Projelendirilmiş DTR Modülü dosyalarını ekle
                if (!string.IsNullOrEmpty(dtrModuluFilePath))
                {
                    args.Append($" --dtr-modulu \"{dtrModuluFilePath}\"");
                }

                if (!string.IsNullOrEmpty(yeniDtrModuluFilePath))
                {
                    args.Append($" --yeni-dtr-modulu \"{yeniDtrModuluFilePath}\"");
                }

                // Uydu verisi argümanını ekle
                if (!string.IsNullOrEmpty(uyduVeriFilePath))
                {
                    args.Append($" --uydu-data \"{uyduVeriFilePath}\"");
                }

                // Overpass verisi ekle
                if (!string.IsNullOrEmpty(csvFilePath))
                {
                    args.Append($" --overpass-data \"{csvFilePath}\"");
                }

                Console.WriteLine($"Çalıştırılacak komut: python {args}");

                // Prepare the dictionary
                // Prepare the dictionary
                var argsDict = new Dictionary<string, string>
                {
                    { "region", selectedCity },
                    { "district", selectedDistrict },
                    { "kml_file", kmlFilePath },
                    { "output_dir", imarAnaliziPath },
                    { "output_prefix", outputPrefix },
                    { "year", year },
                };

                // Optional entries (added only if not null or empty)
                if (!string.IsNullOrEmpty(hucreFilePath))
                    argsDict["hucre_data"] = hucreFilePath;

                if (!string.IsNullOrEmpty(meskenFile))
                    argsDict["mesken_file"] = meskenFile;

                if (!string.IsNullOrEmpty(otherFile))
                    argsDict["other_file"] = otherFile;

                if (!string.IsNullOrEmpty(dtrModuluFilePath))
                    argsDict["dtr_modulu"] = dtrModuluFilePath;

                if (!string.IsNullOrEmpty(yeniDtrModuluFilePath))
                    argsDict["yeni_dtr_modulu"] = yeniDtrModuluFilePath;

                if (!string.IsNullOrEmpty(uyduVeriFilePath))
                    argsDict["uydu_data"] = uyduVeriFilePath;

                if (!string.IsNullOrEmpty(csvFilePath))
                    argsDict["overpass_data"] = csvFilePath;    
                                             
                // JSON formatında serileştirme
                string json = JsonConvert.SerializeObject(argsDict, Formatting.Indented);

                // JSON dosyasını yazma
                File.WriteAllText("arguments.json", json);

                // Python betiğini çalıştır
                ProcessStartInfo processInfo = new ProcessStartInfo("python")
                {
                    FileName = "cmd.exe",
                    Arguments = args.ToString(),
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WorkingDirectory = Path.GetDirectoryName(pythonScriptPath)
                };

                using (Process process = Process.Start(processInfo))
                {

                    // İşlem durumunu izleme ve ilerleme raporu
                    DateTime startTime = DateTime.Now;

                    // İlerleme raporlama için bir Timer başlat
                    System.Timers.Timer progressTimer = new System.Timers.Timer(30000); // 30 saniyede bir rapor
                    progressTimer.Elapsed += (sender, e) =>
                    {
                        TimeSpan elapsed = DateTime.Now - startTime;
                        Console.WriteLine($"İşlem devam ediyor... Geçen süre: {elapsed.Minutes} dakika {elapsed.Seconds} saniye");
                    };
                    progressTimer.AutoReset = true;
                    progressTimer.Start();

                    try
                    {
                        // İşlemin tamamlanmasını sonsuza kadar bekle (zaman kısıtlaması yok)
                        process.WaitForExit();

                        // İşlem tamamlandı, çıkış kodunu kontrol et
                        if (process.ExitCode != 0)
                        {
                            throw new Exception($"Python betiği hata ile sonlandı. Çıkış kodu: {process.ExitCode}");
                        }
                    }
                    finally
                    {
                        // Her durumda Timer'ı durdur
                        progressTimer.Stop();
                        progressTimer.Dispose();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"İmar Planı modeli çalıştırılırken hata: {ex.Message}");
                throw; // Üst seviye metodların hatayı yakalaması için yeniden fırlat
            }
        }

    }

}






