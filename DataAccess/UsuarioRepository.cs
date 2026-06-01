using System.Data;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class UsuarioRepository
    {
        public Usuario Autenticar(string username, string password)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT Id, Username, Rol FROM Usuarios WHERE Username = @u AND Password = @p", conn))
                {
                    cmd.Parameters.AddWithValue("u", username);
                    cmd.Parameters.AddWithValue("p", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Rol = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool EsAdministrador(string username, string password)
        {
            var usuario = Autenticar(username, password);
            return usuario != null && usuario.Rol.ToLower() == "admin";
        }
    }
}
