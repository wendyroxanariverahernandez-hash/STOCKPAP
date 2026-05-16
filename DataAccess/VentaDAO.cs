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
                            // Cliente was removed, it used to be at index 2
                            Total = reader.GetDecimal(3),
                            MetodoPago = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            CantidadRecibida = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                            Cambio = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6),
                            TipoTarjeta = reader.IsDBNull(7) ? null : reader.GetString(7),
                            Banco = reader.IsDBNull(8) ? null : reader.GetString(8),
                            Ultimos4 = reader.IsDBNull(9) ? null : reader.GetString(9),
                            Referencia = reader.IsDBNull(10) ? null : reader.GetString(10),
                            Confirmacion = reader.IsDBNull(11) ? (bool?)null : reader.GetBoolean(11)
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
                using (var command = new NpgsqlCommand("INSERT INTO Ventas (Fecha, Cliente, Total, MetodoPago, CantidadRecibida, Cambio, TipoTarjeta, Banco, Ultimos4, Referencia, Confirmacion) VALUES (@fecha, @cliente, @total, @metodoPago, @cantidadRecibida, @cambio, @tipoTarjeta, @banco, @ultimos4, @referencia, @confirmacion)", connection))
                {
                    command.Parameters.AddWithValue("fecha", venta.Fecha == default(DateTime) ? DateTime.Now : venta.Fecha);
                    command.Parameters.AddWithValue("cliente", "Público General"); // Hardcoded as requested
                    command.Parameters.AddWithValue("total", venta.Total);
                    command.Parameters.AddWithValue("metodoPago", venta.MetodoPago ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("cantidadRecibida", venta.CantidadRecibida ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("cambio", venta.Cambio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("tipoTarjeta", venta.TipoTarjeta ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("banco", venta.Banco ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("ultimos4", venta.Ultimos4 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("referencia", venta.Referencia ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("confirmacion", venta.Confirmacion ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Venta venta)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("UPDATE Ventas SET Fecha=@fecha, Cliente=@cliente, Total=@total, MetodoPago=@metodoPago, CantidadRecibida=@cantidadRecibida, Cambio=@cambio, TipoTarjeta=@tipoTarjeta, Banco=@banco, Ultimos4=@ultimos4, Referencia=@referencia, Confirmacion=@confirmacion WHERE Id=@id", connection))
                {
                    command.Parameters.AddWithValue("fecha", venta.Fecha);
                    command.Parameters.AddWithValue("cliente", "Público General"); // Hardcoded as requested
                    command.Parameters.AddWithValue("total", venta.Total);
                    command.Parameters.AddWithValue("metodoPago", venta.MetodoPago ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("cantidadRecibida", venta.CantidadRecibida ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("cambio", venta.Cambio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("tipoTarjeta", venta.TipoTarjeta ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("banco", venta.Banco ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("ultimos4", venta.Ultimos4 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("referencia", venta.Referencia ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("confirmacion", venta.Confirmacion ?? (object)DBNull.Value);
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
