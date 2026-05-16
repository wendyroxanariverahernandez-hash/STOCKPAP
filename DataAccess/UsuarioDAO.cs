using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class UsuarioDAO
    {
        public List<Usuario> GetAll()
        {
            var list = new List<Usuario>();
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM usuarios ORDER BY id", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Usuario
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Password = reader.GetString(2),
                            Rol = reader.GetString(3)
                        });
                    }
                }
            }
            return list;
        }

        public void Add(Usuario usuario)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO usuarios (usuario, password, rol) VALUES (@username, @password, @rol)", connection))
                {
                    command.Parameters.AddWithValue("username", usuario.Username);
                    command.Parameters.AddWithValue("password", usuario.Password);
                    command.Parameters.AddWithValue("rol", usuario.Rol);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Usuario usuario)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("UPDATE usuarios SET usuario=@username, password=@password, rol=@rol WHERE id=@id", connection))
                {
                    command.Parameters.AddWithValue("username", usuario.Username);
                    command.Parameters.AddWithValue("password", usuario.Password);
                    command.Parameters.AddWithValue("rol", usuario.Rol);
                    command.Parameters.AddWithValue("id", usuario.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM usuarios WHERE id=@id", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Usuario Authenticate(string username, string password)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM usuarios WHERE usuario=@username AND password=@password", connection))
                {
                    command.Parameters.AddWithValue("username", username);
                    command.Parameters.AddWithValue("password", password);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Rol = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
