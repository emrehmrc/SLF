public class DatabaseManager
{
    private static DatabaseManager instance;
    private NpgsqlConnection connection;
    private bool isConnected = false;

    public static DatabaseManager GetInstance(string connectionString = "")
    {
        if (instance == null)
        {
            instance = new DatabaseManager(connectionString);
        }
        return instance;
    }

    public NpgsqlConnection GetConnection()
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            OpenConnection();
        }
        return connection;
    }

    public bool IsConnected()
    {
        return isConnected && connection != null && connection.State == ConnectionState.Open;
    }

    private void OpenConnection()
    {
        try
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                connection.Open();
                isConnected = true;
            }
        }
        catch (Exception ex)
        {
            isConnected = false;
            throw new Exception("Veritabanı bağlantısı açılamadı: " + ex.Message);
        }
    }
} 