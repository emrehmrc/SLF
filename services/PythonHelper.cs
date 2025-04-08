using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SLF.Services;

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
                string lastYear = yearService.lastYear.ToString();

                Console.WriteLine($"Deep Learning model çalıştırılıyor: {selectedCity}/{selectedDistrict}, LastYear: {lastYear}");

                // Python script yolu - artık python_kod klasöründen alınıyor
                string pythonScriptPath = PathService.GetPythonScriptPath("model_learning.py");

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

                // Log mesajı oluştur
                Console.WriteLine($"Python kod klasörü: {PathService.PythonKodDirectory}");
                Console.WriteLine($"Python script: {pythonScriptPath}");
                Console.WriteLine($"Abone verisi: {aboneVeriYolu}");
                Console.WriteLine($"Çıktı klasörü: {imarAnaliziPath}");

                // Python argümanlarını oluştur - İlçe parametresi eklendi
                string arguments = $"\"{pythonScriptPath}\" \"{aboneVeriYolu}\" \"{meskenSonucYolu}\" \"{otherSonucYolu}\" \"{selectedCity}\" \"{selectedDistrict}\" \"{lastYear}\"";

                // Python betiğini çalıştır
                ProcessStartInfo processInfo = new ProcessStartInfo("python")
                {
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                string output = "";
                string error = "";
                using (Process process = Process.Start(processInfo))
                {
                    output = process.StandardOutput.ReadToEnd();
                    error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Python betiği hata ile sonlandı. Çıktı: {output}, Hata: {error}");
                    }
                }

                // İşlem başarılı mesajı
                Console.WriteLine("Deep Learning modeli başarıyla çalıştırıldı.");
                Console.WriteLine(output);

                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deep Learning modeli çalıştırılırken hata: {ex.Message}");
                throw; // Üst seviye metodların hatayı yakalaması için yeniden fırlat
            }
        }

        /// <summary>
        /// İmar Planı analizini çalıştırır ve sonuçları döndürür
        /// </summary>
        /// <param name="kmlFilePath">KML dosyasının yolu (zorunlu)</param>
        /// <param name="csvFilePath">CSV dosyasının yolu (opsiyonel)</param>
        /// <returns>Python betiğinin çıktısı</returns>
        public static string RunImarPlanModel(string kmlFilePath, string csvFilePath = null)
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

                Console.WriteLine($"İmar Planı modeli çalıştırılıyor: {selectedCity}/{selectedDistrict}");

                // Python script yolu 
                string pythonScriptPath = PathService.GetPythonScriptPath("main.py");

                if (!File.Exists(pythonScriptPath))
                {
                    throw new Exception($"Python script bulunamadı: {pythonScriptPath}");
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
                string outputPrefix = $"imar_plan_{selectedCity}_{selectedDistrict}";
                string outputCsvPath = Path.Combine(imarAnaliziPath, $"{outputPrefix}.csv");
                string outputKmlPath = Path.Combine(imarAnaliziPath, $"{outputPrefix}.kml");
                string tempDirPath = Path.Combine(imarAnaliziPath, "temp");

                // Temp klasörü
                if (!Directory.Exists(tempDirPath))
                {
                    Directory.CreateDirectory(tempDirPath);
                }

                // Log mesajı oluştur
                Console.WriteLine($"Python kod klasörü: {PathService.PythonKodDirectory}");
                Console.WriteLine($"Python script: {pythonScriptPath}");
                Console.WriteLine($"KML dosyası: {kmlFilePath}");
                Console.WriteLine($"CSV dosyası: {csvFilePath ?? "Kullanılmıyor"}");
                Console.WriteLine($"CSV çıktı dosyası: {outputCsvPath}");
                Console.WriteLine($"KML çıktı dosyası: {outputKmlPath}");

                // Deep Learning benzeri basitleştirilmiş argüman yaklaşımı
                // Argümanlar: script.py, işlem türü, şehir, kml dosyası, csv çıktısı, kml çıktısı, ilçe, [opsiyonel csv veri dosyası]
                StringBuilder args = new StringBuilder();
                args.Append($"\"{pythonScriptPath}\" process \"{selectedCity}\" \"{kmlFilePath}\"");
                args.Append($" --district \"{selectedDistrict}\"");
                args.Append($" --output-dir \"{imarAnaliziPath}\"");  // Ana çıktı klasörü
                args.Append($" --output-prefix \"{outputPrefix}\"");
                // Eğer CSV dosyası belirtilmişse, ek argüman olarak ekle
                if (!string.IsNullOrEmpty(csvFilePath))
                {
                    args.Append($" --overpass-data \"{csvFilePath}\"");
                }

                Console.WriteLine($"Çalıştırılacak komut: python {args}");

                // Python betiğini çalıştır
                ProcessStartInfo processInfo = new ProcessStartInfo("python")
                {
                    Arguments = args.ToString(),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(pythonScriptPath)
                };

                string output = "";
                string error = "";
                using (Process process = Process.Start(processInfo))
                {
                    // Eş zamanlı çıktı yakalama
                    process.OutputDataReceived += (sender, e) => {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine($"PYTHON: {e.Data}");
                            output += e.Data + Environment.NewLine;
                        }
                    };

                    process.ErrorDataReceived += (sender, e) => {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine($"PYTHON ERROR: {e.Data}");
                            error += e.Data + Environment.NewLine;
                        }
                    };

                    // Asenkron okumaları başlat
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    // İşlem durumunu izle
                    DateTime startTime = DateTime.Now;
                    bool finished = false;

                    // Ana işlemi bekletmeden işlem durumunu kontrol et
                    while (!finished && (DateTime.Now - startTime).TotalMinutes < 10) // 10 dakika zaman aşımı
                    {
                        // İşlem bittiyse döngüden çık
                        finished = process.WaitForExit(1000); // 1 saniye bekle

                        // Her 30 saniyede bir durum raporu
                        if ((DateTime.Now - startTime).TotalSeconds % 30 < 1)
                        {
                            TimeSpan elapsed = DateTime.Now - startTime;
                            Console.WriteLine($"İşlem devam ediyor... Geçen süre: {elapsed.Minutes} dakika {elapsed.Seconds} saniye");
                        }
                    }

                    // Eğer zaman aşımına uğradıysa
                    if (!finished)
                    {
                        Console.WriteLine("İşlem 10 dakika içinde tamamlanamadı. Sonlandırılıyor...");
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"İşlem sonlandırılırken hata: {ex.Message}");
                        }
                        throw new TimeoutException("Python işlemi zaman aşımına uğradı (10 dakika).");
                    }

                    // Çıkış kodunu kontrol et
                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Python betiği hata ile sonlandı. Çıkış kodu: {process.ExitCode}");
                    }
                }

                // İşlem başarılı mesajı
                Console.WriteLine("İmar Planı modeli başarıyla çalıştırıldı.");
                Console.WriteLine($"Toplam çıktı uzunluğu: {output.Length} karakter");

                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"İmar Planı modeli çalıştırılırken hata: {ex.Message}");
                throw; // Üst seviye metodların hatayı yakalaması için yeniden fırlat
            }

        }
    }
}