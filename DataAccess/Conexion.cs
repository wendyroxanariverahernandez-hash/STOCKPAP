using System.Configuration;
using Npgsql;

namespace STOCKPAP.DataAccess
{
    public sealed class Conexion
    {
        private readonly string connectionString;
        private static Conexion _instance;
        private static readonly object _lock = new object();

        private Conexion()
        {
            var configuredConnection = ConfigurationManager.ConnectionStrings["StockPapDb"];
            connectionString = configuredConnection != null
                ? configuredConnection.ConnectionString
                : "Server=localhost;Port=5432;Database=stockpap_db;User Id=postgres;Password=postgres;";
        }

        public static Conexion Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Conexion();
                        }
                    }
                }
                return _instance;
            }
        }

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
