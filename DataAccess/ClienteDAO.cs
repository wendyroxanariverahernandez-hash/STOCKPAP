using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class ClienteDAO
    {
        public List<Cliente> GetAll()
        {
            var list = new List<Cliente>();
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM Clientes ORDER BY Id", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Cliente
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Direccion = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Telefono = reader.IsDBNull(3) ? "" : reader.GetString(3)
                        });
                    }
                }
            }
            return list;
        }

        public void Add(Cliente cliente)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO Clientes (Nombre, Direccion, Telefono) VALUES (@nombre, @direccion, @telefono)", connection))
                {
                    command.Parameters.AddWithValue("nombre", cliente.Nombre);
                    command.Parameters.AddWithValue("direccion", cliente.Direccion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("telefono", cliente.Telefono ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Cliente cliente)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("UPDATE Clientes SET Nombre=@nombre, Direccion=@direccion, Telefono=@telefono WHERE Id=@id", connection))
                {
                    command.Parameters.AddWithValue("nombre", cliente.Nombre);
                    command.Parameters.AddWithValue("direccion", cliente.Direccion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("telefono", cliente.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("id", cliente.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM Clientes WHERE Id=@id", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
