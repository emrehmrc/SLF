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
        /// Veritabanındaki tablo verilerini kontrol eder ve config ile karşılaştırır
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
                        AboneTuketimConfigCount = 0
                    };
                }

                // Debug için
                System.Diagnostics.Debug.WriteLine($"Config'den okunan tablolar:");
                System.Diagnostics.Debug.WriteLine($"Abone Bilgi: {aboneBilgiTable}");
                System.Diagnostics.Debug.WriteLine($"Abone Tüketim: {aboneTuketimTable}");

                // Veritabanından güncel satır sayılarını al
                int currentAboneBilgiCount = 0;
                int currentAboneTuketimCount = 0;

                try
                {
                    currentAboneBilgiCount = GetTableRowCount(aboneBilgiTable);
                    System.Diagnostics.Debug.WriteLine($"Abone Bilgi Count: {currentAboneBilgiCount}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Abone Bilgi Count Hatası: {ex.Message}");
                    throw new Exception($"Abone bilgi tablosu erişim hatası: {ex.Message}");
                }

                try
                {
                    currentAboneTuketimCount = GetTableRowCount(aboneTuketimTable);
                    System.Diagnostics.Debug.WriteLine($"Abone Tüketim Count: {currentAboneTuketimCount}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Abone Tüketim Count Hatası: {ex.Message}");
                    throw new Exception($"Abone tüketim tablosu erişim hatası: {ex.Message}");
                }

                // Config'den kayıtlı satır sayılarını al
                int configAboneBilgiCount = ConfigService.GetAboneBilgiRowCount(configPath);
                int configAboneTuketimCount = ConfigService.GetAboneTuketimRowCount(configPath);

                System.Diagnostics.Debug.WriteLine($"Config Abone Bilgi: {configAboneBilgiCount}");
                System.Diagnostics.Debug.WriteLine($"Config Abone Tüketim: {configAboneTuketimCount}");

                // Karşılaştırma yap
                bool isUpToDate = (currentAboneBilgiCount == configAboneBilgiCount) &&
                                 (currentAboneTuketimCount == configAboneTuketimCount);

                string message;
                if (isUpToDate)
                {
                    message = "Verileriniz güncel.";
                }
                else
                {
                    message = $"Verileriniz güncel değil.\n\n" +
                             $"Abone Bilgi Tablosu ({aboneBilgiTable}):\n" +
                             $"  Güncel: {currentAboneBilgiCount:N0} satır\n" +
                             $"  Config: {configAboneBilgiCount:N0} satır\n\n" +
                             $"Abone Tüketim Tablosu ({aboneTuketimTable}):\n" +
                             $"  Güncel: {currentAboneTuketimCount:N0} satır\n" +
                             $"  Config: {configAboneTuketimCount:N0} satır";
                }

                // Config'i güncelle
                if (!isUpToDate)
                {
                    ConfigService.UpdateAboneRowCounts(configPath, currentAboneBilgiCount, currentAboneTuketimCount);
                }

                return new DataValidationResult
                {
                    IsValid = isUpToDate,
                    Message = message,
                    AboneBilgiCurrentCount = currentAboneBilgiCount,
                    AboneTuketimCurrentCount = currentAboneTuketimCount,
                    AboneBilgiConfigCount = configAboneBilgiCount,
                    AboneTuketimConfigCount = configAboneTuketimCount
                };
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
                    AboneTuketimConfigCount = 0
                };
            }
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
    /// Veri doğrulama sonuç sınıfı
    /// </summary>
    public class DataValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public int AboneBilgiCurrentCount { get; set; }
        public int AboneTuketimCurrentCount { get; set; }
        public int AboneBilgiConfigCount { get; set; }
        public int AboneTuketimConfigCount { get; set; }
    }
}