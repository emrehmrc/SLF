using Npgsql;
using System;
using System.Data;

namespace SLF.Services
{
    public class DatabaseManager
    {
        private static DatabaseManager _instance;
        private static readonly object _lock = new object();
        private NpgsqlConnection _connection;
        private string _connectionString;

        private DatabaseManager()
        {
        }

        public static DatabaseManager GetInstance(string connString = null)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DatabaseManager();
                    }
                }
            }

            if (!string.IsNullOrEmpty(connString))
            {
                _instance._connectionString = connString;
            }

            return _instance;
        }

        public NpgsqlConnection GetConnection()
        {
            try
            {
                if (_connection == null || _connection.State != ConnectionState.Open)
                {
                    if (string.IsNullOrEmpty(_connectionString))
                    {
                        throw new Exception("Veritabanı bağlantı bilgileri bulunamadı. Lütfen tekrar giriş yapın.");
                    }

                    if (_connection != null)
                    {
                        _connection.Dispose();
                    }

                    _connection = new NpgsqlConnection(_connectionString);
                    _connection.Open();
                }

                return _connection;
            }
            catch (Exception ex)
            {
                throw new Exception($"Veritabanı bağlantısı kurulamadı: {ex.Message}");
            }
        }

        public void CloseConnection()
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Dispose();
                _connection = null;
            }
        }

        public bool IsConnected()
        {
            return _connection != null && _connection.State == ConnectionState.Open;
        }

        public bool HasConnectionString()
        {
            return !string.IsNullOrEmpty(_connectionString);
        }
    }
}