using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                string lastYear = yearService.LastYear.ToString();

                Console.WriteLine($"Deep Learning model çalıştırılıyor: {selectedCity}/{selectedDistrict}, LastYear: {lastYear}");

                // Python script yolu - artık python_kod klasöründen alınıyor
                string scriptRelativePath = Path.Combine("python_kod", "deep_learning", "kod", "model_learning.py");
                string pythonScriptPath = PathService.GetPythonScriptPath(scriptRelativePath);
                //Console.WriteLine("deeplearningpath"+pythonScriptPath.ToString());
                

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


        public static string RunImarPlanModel(string kmlFilePath, string csvFilePath = null)
        {
            try
            {
                // Hücre verisi yolunu al - SHP ya da CSV dosyasını bul
                string hucrePath = PathService.HucrePath;
                string hucreFilePath = null;
                string uyduVeriPath = PathService.UyduVerileriPath;
                string uyduVeriFilePath = null;

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
                        hucreFilePath = hucreFiles[0]; // İlk bulunan SHP dosyasını kullan
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
                string outputPrefix = $"imar_plan_{selectedCity}_{selectedDistrict}";
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
                args.Append($"\"{pythonScriptPath}\" process \"{selectedCity}\" \"{kmlFilePath}\"");
                args.Append($" --district \"{selectedDistrict}\"");
                args.Append($" --output-dir \"{imarAnaliziPath}\"");  // Ana çıktı klasörü
                args.Append($" --output-prefix \"{outputPrefix}\"");

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
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine($"PYTHON: {e.Data}");
                            output += e.Data + Environment.NewLine;
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine($"PYTHON: {e.Data}");
                            error += e.Data + Environment.NewLine;
                        }
                    };

                    // Asenkron okumaları başlat
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

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




