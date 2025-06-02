using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using SLF.services;

namespace SLF.Services
{
    public static class DataValidationService
    {
        /// <summary>
        /// Veritabanındaki tablo verilerini kontrol eder, config ile karşılaştırır ve CSV durumunu kontrol eder
        /// </summary>
        public static DataValidationResult ValidateAboneData(string configPath)
        {
            try
            {
                // Config'den tablo isimlerini al
                var (aboneBilgiTable, aboneTuketimTable) = ConfigService.GetDatabaseTableNames(configPath);

                if (string.IsNullOrEmpty(aboneBilgiTable) || string.IsNullOrEmpty(aboneTuketimTable))
                {
                    return new DataValidationResult
                    {
                        IsValid = false,
                        Message = "Config dosyasında tablo isimleri bulunamadı.",
                        AboneBilgiCurrentCount = 0,
                        AboneTuketimCurrentCount = 0,
                        AboneBilgiConfigCount = 0,
                        AboneTuketimConfigCount = 0,
                        HasDepoCsvFiles = false,
                        DepoPath = ""
                    };
                }

                // Debug için
                System.Diagnostics.Debug.WriteLine($"Config'den okunan tablolar:");
                System.Diagnostics.Debug.WriteLine($"Abone Bilgi: {aboneBilgiTable}");
                System.Diagnostics.Debug.WriteLine($"Abone Tüketim: {aboneTuketimTable}");

                // Depo klasörü ve CSV dosyalarının varlığını kontrol et
                var (hasDepoCsv, depoPath, csvStatus) = CheckDepoCsvFiles(configPath);

                // Veritabanından güncel satır sayılarını al
                int currentAboneBilgiCount = 0;
                int currentAboneTuketimCount = 0;
                bool databaseAccessible = true;

                try
                {
                    currentAboneBilgiCount = GetTableRowCount(aboneBilgiTable);
                    System.Diagnostics.Debug.WriteLine($"Abone Bilgi Count: {currentAboneBilgiCount}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Abone Bilgi Count Hatası: {ex.Message}");
                    databaseAccessible = false;
                }

                try
                {
                    currentAboneTuketimCount = GetTableRowCount(aboneTuketimTable);
                    System.Diagnostics.Debug.WriteLine($"Abone Tüketim Count: {currentAboneTuketimCount}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Abone Tüketim Count Hatası: {ex.Message}");
                    databaseAccessible = false;
                }

                // Config'den kayıtlı satır sayılarını al
                int configAboneBilgiCount = ConfigService.GetAboneBilgiRowCount(configPath);
                int configAboneTuketimCount = ConfigService.GetAboneTuketimRowCount(configPath);

                System.Diagnostics.Debug.WriteLine($"Config Abone Bilgi: {configAboneBilgiCount}");
                System.Diagnostics.Debug.WriteLine($"Config Abone Tüketim: {configAboneTuketimCount}");

                // Veri durumunu analiz et
                return AnalyzeDataStatus(
                    databaseAccessible,
                    currentAboneBilgiCount,
                    currentAboneTuketimCount,
                    configAboneBilgiCount,
                    configAboneTuketimCount,
                    hasDepoCsv,
                    depoPath,
                    csvStatus,
                    aboneBilgiTable,
                    aboneTuketimTable,
                    configPath
                );
            }
            catch (Exception ex)
            {
                return new DataValidationResult
                {
                    IsValid = false,
                    Message = $"Veri doğrulama sırasında hata oluştu: {ex.Message}",
                    AboneBilgiCurrentCount = 0,
                    AboneTuketimCurrentCount = 0,
                    AboneBilgiConfigCount = 0,
                    AboneTuketimConfigCount = 0,
                    HasDepoCsvFiles = false,
                    DepoPath = ""
                };
            }
        }

        /// <summary>
        /// Depo klasöründeki CSV dosyalarının varlığını kontrol eder
        /// </summary>
        private static (bool hasDepoCsv, string depoPath, string csvStatus) CheckDepoCsvFiles(string configPath)
        {
            try
            {
                // Config'den depo yolunu oluştur
                string jsonContent = File.ReadAllText(configPath);
                JObject config = JObject.Parse(jsonContent);

                string anaKlasor = config["Ana_Klasör_Yolu"]?.ToString() ?? "";
                string il = config["İl"]?.ToString() ?? "";
                string depoRelPath = config["depo"]?.ToString() ?? "depo";

                // Slash'leri temizle
                if (depoRelPath.StartsWith("/")) depoRelPath = depoRelPath.Substring(1);
                if (depoRelPath.EndsWith("/")) depoRelPath = depoRelPath.Substring(0, depoRelPath.Length - 1);

                string depoPath = Path.Combine(anaKlasor, il, depoRelPath);

                // CSV dosya yolları
                string aboneCsvPath = Path.Combine(depoPath, "DWH_MRC_SLFPROJE_ABN_BLG.csv");
                string tuketimCsvPath = Path.Combine(depoPath, "DWH_TUKETIM_DENEME.csv");

                bool aboneExists = File.Exists(aboneCsvPath);
                bool tuketimExists = File.Exists(tuketimCsvPath);

                string csvStatus = "";
                if (aboneExists && tuketimExists)
                {
                    csvStatus = "✓ Tüm CSV dosyaları mevcut";
                }
                else if (aboneExists || tuketimExists)
                {
                    csvStatus = $"⚠ Kısmi CSV dosyaları mevcut (Abone: {(aboneExists ? "✓" : "✗")}, Tüketim: {(tuketimExists ? "✓" : "✗")})";
                }
                else
                {
                    csvStatus = "✗ CSV dosyaları bulunamadı";
                }

                return (aboneExists && tuketimExists, depoPath, csvStatus);
            }
            catch (Exception ex)
            {
                return (false, "", $"CSV kontrolü hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Veri durumunu analiz eder ve uygun mesajı oluşturur
        /// </summary>
        private static DataValidationResult AnalyzeDataStatus(
            bool databaseAccessible,
            int currentAboneBilgiCount,
            int currentAboneTuketimCount,
            int configAboneBilgiCount,
            int configAboneTuketimCount,
            bool hasDepoCsv,
            string depoPath,
            string csvStatus,
            string aboneBilgiTable,
            string aboneTuketimTable,
            string configPath)
        {
            bool isDataUpToDate = false;
            string message = "";
            bool canProceed = false;

            if (!databaseAccessible)
            {
                // Veritabanına erişim yok
                if (hasDepoCsv)
                {
                    canProceed = true;
                    message = $"⚠ Veritabanına erişim yok, ancak depo CSV dosyaları kullanılabilir.\n\n" +
                             $"{csvStatus}\n" +
                             $"Depo yolu: {depoPath}\n\n" +
                             $"CSV dosyalarından abone verisi oluşturulacak.";
                }
                else
                {
                    canProceed = false;
                    message = $"❌ Veritabanına erişim yok ve depo CSV dosyaları bulunamadı.\n\n" +
                             $"{csvStatus}\n" +
                             $"Depo yolu: {depoPath}\n\n" +
                             $"Lütfen önce veritabanı bağlantısını kurun ve 'Abone Verisi Oluştur' butonuna tıklayın.";
                }
            }
            else
            {
                // Veritabanına erişim var - karşılaştırma yap
                isDataUpToDate = (currentAboneBilgiCount == configAboneBilgiCount) &&
                               (currentAboneTuketimCount == configAboneTuketimCount);

                if (isDataUpToDate)
                {
                    // Veriler güncel
                    if (hasDepoCsv)
                    {
                        canProceed = true;
                        message = $"✅ Veriler güncel! Depo CSV dosyaları kullanılacak.\n\n" +
                                 $"Abone Bilgi ({aboneBilgiTable}): {configAboneBilgiCount:N0} satır\n" +
                                 $"Abone Tüketim ({aboneTuketimTable}): {configAboneTuketimCount:N0} satır\n\n" +
                                 $"{csvStatus}\n" +
                                 $"Depo yolu: {depoPath}";
                    }
                    else
                    {
                        canProceed = false;
                        message = $"✅ Veriler güncel ancak depo CSV dosyaları bulunamadı.\n\n" +
                                 $"Abone Bilgi ({aboneBilgiTable}): {configAboneBilgiCount:N0} satır\n" +
                                 $"Abone Tüketim ({aboneTuketimTable}): {configAboneTuketimCount:N0} satır\n\n" +
                                 $"{csvStatus}\n" +
                                 $"Depo yolu: {depoPath}\n\n" +
                                 $"CSV dosyalarını oluşturmak için 'Abone Verisi Oluştur' butonuna tıklayın.";
                    }
                }
                else
                {
                    // Veriler güncel değil
                    canProceed = false;
                    message = $"⚠ Veriler güncel değil! Güncelleme gerekli.\n\n" +
                             $"Abone Bilgi ({aboneBilgiTable}):\n" +
                             $"  Güncel: {currentAboneBilgiCount:N0} satır\n" +
                             $"  Config: {configAboneBilgiCount:N0} satır\n\n" +
                             $"Abone Tüketim ({aboneTuketimTable}):\n" +
                             $"  Güncel: {currentAboneTuketimCount:N0} satır\n" +
                             $"  Config: {configAboneTuketimCount:N0} satır\n\n" +
                             $"{csvStatus}\n" +
                             $"Depo yolu: {depoPath}\n\n" +
                             $"Lütfen 'Abone Verisi Oluştur' butonuna tıklayarak verileri güncelleyin.";

                    // Config'i güncelle
                    ConfigService.UpdateAboneRowCounts(configPath, currentAboneBilgiCount, currentAboneTuketimCount);
                }
            }

            return new DataValidationResult
            {
                IsValid = canProceed,
                Message = message,
                AboneBilgiCurrentCount = currentAboneBilgiCount,
                AboneTuketimCurrentCount = currentAboneTuketimCount,
                AboneBilgiConfigCount = configAboneBilgiCount,
                AboneTuketimConfigCount = configAboneTuketimCount,
                HasDepoCsvFiles = hasDepoCsv,
                DepoPath = depoPath,
                IsDataUpToDate = isDataUpToDate,
                DatabaseAccessible = databaseAccessible
            };
        }

        /// <summary>
        /// Belirtilen tablonun satır sayısını döner
        /// </summary>
        private static int GetTableRowCount(string tableName)
        {
            try
            {
                // Config'den schema adını al
                string configPath = PathService._configKonum;
                string schema = GetSchemaFromConfig(configPath);

                // Farklı tablo adı formatlarını dene - schema ile başlayanları öncelikli yap
                List<string> tableVariations = new List<string>
                {
                    $"{schema}.{tableName}",                    // Öncelik: C##MRC_SLF_PROJE.TABLO
                    $"{schema}.\"{tableName}\"",               // Öncelik: C##MRC_SLF_PROJE."TABLO"
                    $"{schema}.\"{tableName.ToUpper()}\"",     // Öncelik: C##MRC_SLF_PROJE."TABLO"
                    $"\"{schema}\".\"{tableName}\"",           // Öncelik: "C##MRC_SLF_PROJE"."TABLO"
                    $"\"{schema}\".\"{tableName.ToUpper()}\"", // Öncelik: "C##MRC_SLF_PROJE"."TABLO"
                    tableName,                                  // Son çare: TABLO
                    tableName.ToUpper(),                       // Son çare: TABLO
                    tableName.ToLower(),                       // Son çare: tablo
                    $"\"{tableName}\"",                        // Son çare: "TABLO"
                    $"\"{tableName.ToUpper()}\""               // Son çare: "TABLO"
                };

                Exception lastException = null;
                string successfulFormat = "";

                foreach (string variation in tableVariations)
                {
                    try
                    {
                        string query = $"SELECT COUNT(*) FROM {variation}";
                        var result = DatabaseHelper.ExecuteQuery(query);

                        if (result.Rows.Count > 0)
                        {
                            int count = Convert.ToInt32(result.Rows[0][0]);
                            successfulFormat = variation;

                            // Debug için log yaz
                            System.Diagnostics.Debug.WriteLine($"Tablo: {tableName}");
                            System.Diagnostics.Debug.WriteLine($"Başarılı format: {variation}");
                            System.Diagnostics.Debug.WriteLine($"Satır sayısı: {count}");

                            // Eğer satır sayısı 0 ise ve schema prefix'li format değilse devam et
                            if (count == 0 && !variation.Contains(schema))
                            {
                                System.Diagnostics.Debug.WriteLine($"Satır sayısı 0, schema prefix'siz format atlanıyor: {variation}");
                                continue;
                            }

                            return count;
                        }
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        System.Diagnostics.Debug.WriteLine($"Format başarısız: {variation} - Hata: {ex.Message}");
                        continue; // Bir sonraki formatı dene
                    }
                }

                // Hiçbiri başarılı olmadıysa son hatayı fırlat
                throw lastException ?? new Exception($"Tablo '{tableName}' bulunamadı.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Tablo '{tableName}' satır sayısı alınırken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Config dosyasından schema adını alır
        /// </summary>
        private static string GetSchemaFromConfig(string configPath)
        {
            try
            {
                if (!File.Exists(configPath))
                    return string.Empty;

                string jsonContent = File.ReadAllText(configPath);
                JObject config = JObject.Parse(jsonContent);

                return config["Veritabanı"]?["schema"]?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Veri doğrulama sonuç sınıfı - Güncellenmiş
    /// </summary>
    public class DataValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public int AboneBilgiCurrentCount { get; set; }
        public int AboneTuketimCurrentCount { get; set; }
        public int AboneBilgiConfigCount { get; set; }
        public int AboneTuketimConfigCount { get; set; }

        // Yeni özellikler
        public bool HasDepoCsvFiles { get; set; }
        public string DepoPath { get; set; }
        public bool IsDataUpToDate { get; set; }
        public bool DatabaseAccessible { get; set; }
    }
}