using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SLF.Services
{
    public static class ConfigService
    {
        /// <summary>
        /// Config dosyasından abone verilerinin satır sayısını okur
        /// </summary>
        public static int GetAboneBilgiRowCount(string configPath)
        {
            try
            {
                if (!File.Exists(configPath))
                    return 0;

                string jsonContent = File.ReadAllText(configPath);
                JObject config = JObject.Parse(jsonContent);

                return config["Abone_verileri"]?["veritabanı_abone_bilgi_row"]?.ToObject<int>() ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Config dosyası okunurken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Config dosyasından abone tüketim verilerinin satır sayısını okur
        /// </summary>
        public static int GetAboneTuketimRowCount(string configPath)
        {
            try
            {
                if (!File.Exists(configPath))
                    return 0;

                string jsonContent = File.ReadAllText(configPath);
                JObject config = JObject.Parse(jsonContent);

                return config["Abone_verileri"]?["veritabanı_abone_tuketim_row"]?.ToObject<int>() ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Config dosyası okunurken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Config dosyasındaki abone verilerinin satır sayılarını günceller
        /// </summary>
        public static void UpdateAboneRowCounts(string configPath, int aboneBilgiCount, int aboneTuketimCount)
        {
            try
            {
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException("Config dosyası bulunamadı.");
                }

                string jsonContent = File.ReadAllText(configPath);
                JObject config = JObject.Parse(jsonContent);

                // Abone_verileri bölümünü güncelle
                if (config["Abone_verileri"] == null)
                {
                    config["Abone_verileri"] = new JObject();
                }

                config["Abone_verileri"]["veritabanı_abone_bilgi_row"] = aboneBilgiCount;
                config["Abone_verileri"]["veritabanı_abone_tuketim_row"] = aboneTuketimCount;

                // Dosyaya geri yaz
                File.WriteAllText(configPath, config.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"Config dosyası güncellenirken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Config dosyasından veritabanı tablo isimlerini okur
        /// </summary>
        public static (string aboneBilgiTable, string aboneTuketimTable) GetDatabaseTableNames(string configPath)
        {
            try
            {
                if (!File.Exists(configPath))
                    return (string.Empty, string.Empty);

                string jsonContent = File.ReadAllText(configPath);
                JObject config = JObject.Parse(jsonContent);

                string aboneBilgiTable = config["Veritabanı"]?["Dwh_Abone_Bilgi"]?.ToString() ?? string.Empty;
                string aboneTuketimTable = config["Veritabanı"]?["Dwh_Abone_Tuketim_Tablo"]?.ToString() ?? string.Empty;

                return (aboneBilgiTable, aboneTuketimTable);
            }
            catch (Exception ex)
            {
                throw new Exception($"Config dosyasından tablo isimleri okunurken hata: {ex.Message}");
            }
        }
    }
}