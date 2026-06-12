using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class ClasificacionRepository
    {
        public List<Clase> ObtenerClases()
        {
            var lista = new List<Clase>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT Id, Nombre FROM Clases ORDER BY Nombre", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Clase
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1)
                        });
                    }
                }
            }
            return lista;
        }

        public bool AgregarClase(Clase clase)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO Clases (Nombre) VALUES (@n) RETURNING Id", conn))
                {
                    cmd.Parameters.AddWithValue("n", clase.Nombre);
                    try
                    {
                        clase.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        public bool ActualizarClase(Clase clase)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE Clases SET Nombre = @n WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("n", clase.Nombre);
                    cmd.Parameters.AddWithValue("id", clase.Id);
                    try { return cmd.ExecuteNonQuery() > 0; }
                    catch { return false; }
                }
            }
        }

        public bool EliminarClase(int id)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM Clases WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Subclase> ObtenerSubclases(int claseId)
        {
            var lista = new List<Subclase>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT Id, Nombre, ClaseId FROM Subclases WHERE ClaseId = @c ORDER BY Nombre", conn))
                {
                    cmd.Parameters.AddWithValue("c", claseId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Subclase
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                ClaseId = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool AgregarSubclase(Subclase subclase)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO Subclases (Nombre, ClaseId) VALUES (@n, @c) RETURNING Id", conn))
                {
                    cmd.Parameters.AddWithValue("n", subclase.Nombre);
                    cmd.Parameters.AddWithValue("c", subclase.ClaseId);
                    try
                    {
                        subclase.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        public bool ActualizarSubclase(Subclase subclase)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE Subclases SET Nombre = @n WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("n", subclase.Nombre);
                    cmd.Parameters.AddWithValue("id", subclase.Id);
                    try { return cmd.ExecuteNonQuery() > 0; }
                    catch { return false; }
                }
            }
        }

        public bool EliminarSubclase(int id)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM Subclases WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Marca> ObtenerMarcas(int claseId)
        {
            var lista = new List<Marca>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT Id, Nombre, ClaseId FROM Marcas WHERE ClaseId = @c ORDER BY Nombre", conn))
                {
                    cmd.Parameters.AddWithValue("c", claseId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Marca
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                ClaseId = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool AgregarMarca(Marca marca)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO Marcas (Nombre, ClaseId) VALUES (@n, @c) RETURNING Id", conn))
                {
                    cmd.Parameters.AddWithValue("n", marca.Nombre);
                    cmd.Parameters.AddWithValue("c", marca.ClaseId);
                    try
                    {
                        marca.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        public bool ActualizarMarca(Marca marca)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE Marcas SET Nombre = @n WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("n", marca.Nombre);
                    cmd.Parameters.AddWithValue("id", marca.Id);
                    try { return cmd.ExecuteNonQuery() > 0; }
                    catch { return false; }
                }
            }
        }

        public bool EliminarMarca(int id)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM Marcas WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
