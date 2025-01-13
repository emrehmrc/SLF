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
                string query = $"SELECT * FROM \"{tableName}\"";
                using (var cmd = new NpgsqlCommand(query, connection))
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                    
                    // Kolon isimlerini büyük harfe çevir
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        col.ColumnName = col.ColumnName.ToUpperInvariant();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Tablo yüklenirken hata: {ex.Message}");
            }

            return dataTable;
        }
    }
}
