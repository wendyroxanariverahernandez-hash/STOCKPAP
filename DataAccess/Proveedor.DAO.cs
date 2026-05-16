using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class ProveedorDAO
    {
        public List<Proveedor> GetAll()
        {
            var list = new List<Proveedor>();
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM Proveedores ORDER BY Id", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Proveedor
                        {
                            Id = reader.GetInt32(0),
                            Empresa = reader.GetString(1),
                            Contacto = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Telefono = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            Estado = reader.IsDBNull(5) ? "" : reader.GetString(5)
                        });
                    }
                }
            }
            return list;
        }

        public void Add(Proveedor proveedor)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO Proveedores (Empresa, Contacto, Telefono, Email, Estado) VALUES (@empresa, @contacto, @telefono, @email, @estado)", connection))
                {
                    command.Parameters.AddWithValue("empresa", proveedor.Empresa);
                    command.Parameters.AddWithValue("contacto", proveedor.Contacto ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("telefono", proveedor.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("email", proveedor.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("estado", proveedor.Estado ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Proveedor proveedor)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("UPDATE Proveedores SET Empresa=@empresa, Contacto=@contacto, Telefono=@telefono, Email=@email, Estado=@estado WHERE Id=@id", connection))
                {
                    command.Parameters.AddWithValue("empresa", proveedor.Empresa);
                    command.Parameters.AddWithValue("contacto", proveedor.Contacto ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("telefono", proveedor.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("email", proveedor.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("estado", proveedor.Estado ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("id", proveedor.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM Proveedores WHERE Id=@id", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
