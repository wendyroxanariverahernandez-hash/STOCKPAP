using System;
using Npgsql;
using STOCKPAP.DataAccess;

namespace STOCKPAP.Utilities
{
    public static class ConfigHelper
    {
        public static event Action ConfiguracionesActualizadas;

        public static void NotificarCambios()
        {
            ConfiguracionesActualizadas?.Invoke();
        }

        public static string Obtener(string clave, string defecto)
        {
            try
            {
                using (var conn = Conexion.Instance.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT Valor FROM Configuracion WHERE Clave = @c", conn))
                    {
                        cmd.Parameters.AddWithValue("c", clave);
                        var val = cmd.ExecuteScalar();
                        return val != null ? val.ToString() : defecto;
                    }
                }
            }
            catch
            {
                return defecto;
            }
        }
    }
}
