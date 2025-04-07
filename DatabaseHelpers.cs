using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using System.Diagnostics;
namespace SLF.services
{
    public static class DatabaseHelper
    {
        /// <summary>
        /// Veritabanında sorgu çalıştırır ve sonucu DataTable olarak döner.
        /// </summary>
        public static DataTable ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                var connection = DatabaseManager.GetInstance().GetConnection();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    // Parametreleri ekle
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable result = new DataTable();
                        adapter.Fill(result);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Sorgu çalıştırılamadı: {ex.Message}");
            }
        }

        /// <summary>
        /// Veritabanında sorgu çalıştırır (INSERT, UPDATE, DELETE gibi) ve etkilenen satır sayısını döner.
        /// </summary>
        public static int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                var connection = DatabaseManager.GetInstance().GetConnection();
                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    // Parametreleri ekle
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Sorgu çalıştırılamadı: {ex.Message}");
            }
        }

        /// <summary>
        /// Veritabanında bir tablonun var olup olmadığını kontrol eder.
        /// </summary>
        public static bool TableExists(string tableName)
        {
            try
            {
                string query = @"
                    SELECT EXISTS (
                        SELECT FROM information_schema.tables 
                        WHERE table_schema = 'public' 
                        AND table_name = @tableName
                    )";

                var parameters = new Dictionary<string, object>
                {
                    { "@tableName", tableName.ToLower() }
                };

                var result = ExecuteQuery(query, parameters);
                return Convert.ToBoolean(result.Rows[0][0]);
            }
            catch (Exception ex)
            {
                throw new Exception($"Tablo varlık kontrolü sırasında hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// Veritabanında belirli bir tabloyu alır.
        /// </summary>
        public static DataTable LoadTable(string tableName)
        {
            DataTable dataTable = new DataTable();
            var connection = DatabaseManager.GetInstance().GetConnection();

            try
            {
                Console.WriteLine($"LoadTable başladı - Tablo adı: {tableName}");

                // Bağlantı durumunu kontrol et
                Console.WriteLine($"Bağlantı durumu: {connection.State}");

                string query = $"SELECT * FROM \"{tableName}\"";
                Console.WriteLine($"Çalıştırılacak sorgu: {query}");

                using (var cmd = new NpgsqlCommand(query, connection))
                {
                    Console.WriteLine("NpgsqlCommand oluşturuldu");

                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        Console.WriteLine("DataAdapter oluşturuldu");

                        Console.WriteLine("Fill işlemi başlıyor...");
                        adapter.Fill(dataTable);
                        Console.WriteLine($"Fill işlemi tamamlandı. Satır sayısı: {dataTable.Rows.Count}");

                        Console.WriteLine("Kolon isimleri büyük harfe çevriliyor...");
                        // Kolon isimlerini büyük harfe çevir
                        foreach (DataColumn col in dataTable.Columns)
                        {
                            string oldName = col.ColumnName;
                            col.ColumnName = col.ColumnName.ToUpperInvariant();
                            Console.WriteLine($"Kolon adı değiştirildi: {oldName} -> {col.ColumnName}");
                        }
                    }
                }

                Console.WriteLine($"LoadTable başarıyla tamamlandı. Toplam satır: {dataTable.Rows.Count}");
                return dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadTable HATA: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new Exception($"Tablo yüklenirken hata: {ex.Message}");
            }
        }
    }
}

