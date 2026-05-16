using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class VentaDAO
    {
        public List<Venta> GetAll()
        {
            var list = new List<Venta>();
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM Ventas ORDER BY Id", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Venta
                        {
                            Id = reader.GetInt32(0),
                            Fecha = reader.GetDateTime(1),
                            Cliente = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Total = reader.GetDecimal(3),
                            MetodoPago = reader.IsDBNull(4) ? "" : reader.GetString(4)
                        });
                    }
                }
            }
            return list;
        }

        public void Add(Venta venta)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO Ventas (Fecha, Cliente, Total, MetodoPago) VALUES (@fecha, @cliente, @total, @metodoPago)", connection))
                {
                    command.Parameters.AddWithValue("fecha", venta.Fecha == default(DateTime) ? DateTime.Now : venta.Fecha);
                    command.Parameters.AddWithValue("cliente", venta.Cliente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("total", venta.Total);
                    command.Parameters.AddWithValue("metodoPago", venta.MetodoPago ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Venta venta)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("UPDATE Ventas SET Fecha=@fecha, Cliente=@cliente, Total=@total, MetodoPago=@metodoPago WHERE Id=@id", connection))
                {
                    command.Parameters.AddWithValue("fecha", venta.Fecha);
                    command.Parameters.AddWithValue("cliente", venta.Cliente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("total", venta.Total);
                    command.Parameters.AddWithValue("metodoPago", venta.MetodoPago ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("id", venta.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM Ventas WHERE Id=@id", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
