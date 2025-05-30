using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client; // PostgreSQL yerine Oracle kütüphanesi
using System.Data;
namespace SLF.Services
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
                using (var cmd = new OracleCommand(query, connection)) // NpgsqlCommand -> OracleCommand
                {
                    // Parametreleri ekle - Oracle'da parametre formatı farklıdır
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            // Oracle'da parametre işareti @ yerine : kullanılır
                            string paramName = param.Key;
                            if (paramName.StartsWith("@"))
                                paramName = ":" + paramName.Substring(1);

                            cmd.Parameters.Add(new OracleParameter(paramName, param.Value ?? DBNull.Value));
                        }
                    }

                    using (var adapter = new OracleDataAdapter(cmd)) // NpgsqlDataAdapter -> OracleDataAdapter
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
                using (var cmd = new OracleCommand(query, connection)) // NpgsqlCommand -> OracleCommand
                {
                    // Parametreleri ekle
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            // Oracle'da parametre işareti @ yerine : kullanılır
                            string paramName = param.Key;
                            if (paramName.StartsWith("@"))
                                paramName = ":" + paramName.Substring(1);

                            cmd.Parameters.Add(new OracleParameter(paramName, param.Value ?? DBNull.Value));
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
                // Oracle için tablo var mı kontrolü sorgusu
                string query = @"
                    SELECT COUNT(*) 
                    FROM ALL_TABLES 
                    WHERE OWNER = USER 
                    AND TABLE_NAME = :tableName";

                var parameters = new Dictionary<string, object>
                {
                    { ":tableName", tableName.ToUpper() } // Oracle genellikle büyük harf kullanır
                };

                var result = ExecuteQuery(query, parameters);
                return Convert.ToInt32(result.Rows[0][0]) > 0;
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
                Console.WriteLine($"Bağlantı durumu: {connection.State}");

                // Oracle'da tablo ve kolon isimleri genellikle büyük harflidir
                // ve çift tırnak yerine tek tırnak kullanılır
                string query = $"SELECT * FROM {tableName}";
                Console.WriteLine($"Çalıştırılacak sorgu: {query}");

                using (var cmd = new OracleCommand(query, connection)) // NpgsqlCommand -> OracleCommand
                {
                    Console.WriteLine("OracleCommand oluşturuldu");

                    using (var adapter = new OracleDataAdapter(cmd)) // NpgsqlDataAdapter -> OracleDataAdapter
                    {
                        Console.WriteLine("DataAdapter oluşturuldu");

                        Console.WriteLine("Fill işlemi başlıyor...");
                        adapter.Fill(dataTable);
                        Console.WriteLine($"Fill işlemi tamamlandı. Satır sayısı: {dataTable.Rows.Count}");

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