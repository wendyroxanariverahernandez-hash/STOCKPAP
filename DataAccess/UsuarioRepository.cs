using System;
using System.Data;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class UsuarioRepository
    {
        public UsuarioRepository()
        {
            AsegurarCamposUsuario();
        }

        private void AsegurarCamposUsuario()
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "ALTER TABLE Usuarios ADD COLUMN IF NOT EXISTS NombreCompleto VARCHAR(150);" +
                    "ALTER TABLE Usuarios ADD COLUMN IF NOT EXISTS Email VARCHAR(150);", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Usuario Autenticar(string username, string password)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT Id, Username, Rol, NombreCompleto, Email FROM Usuarios WHERE Username = @u AND Password = @p", conn))
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
                                Rol = reader.GetString(2),
                                NombreCompleto = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Email = reader.IsDBNull(4) ? "" : reader.GetString(4)
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

        public bool ActualizarInformacionPersonal(int id, string username, string nombreCompleto, string email)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmdCheck = new NpgsqlCommand("SELECT COUNT(*) FROM Usuarios WHERE Username = @u AND Id <> @id", conn))
                {
                    cmdCheck.Parameters.AddWithValue("u", username);
                    cmdCheck.Parameters.AddWithValue("id", id);
                    if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                    {
                        return false; // Username already taken
                    }
                }

                using (var cmd = new NpgsqlCommand("UPDATE Usuarios SET Username = @u, NombreCompleto = @n, Email = @e WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("u", username);
                    cmd.Parameters.AddWithValue("n", nombreCompleto ?? "");
                    cmd.Parameters.AddWithValue("e", email ?? "");
                    cmd.Parameters.AddWithValue("id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
