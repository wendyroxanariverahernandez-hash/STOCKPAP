using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class MovimientoRepository
    {
        public List<Movimiento> ObtenerTodos()
        {
            var lista = new List<Movimiento>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT m.Id, m.Tipo, m.ProductoId, p.Nombre, m.Cantidad, m.StockAnterior, m.StockNuevo, m.Motivo, m.Fecha 
                    FROM Movimientos m
                    JOIN Productos p ON m.ProductoId = p.Id
                    ORDER BY m.Fecha DESC";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Movimiento
                        {
                            Id = reader.GetInt32(0),
                            Tipo = reader.GetString(1),
                            ProductoId = reader.GetInt32(2),
                            ProductoNombre = reader.GetString(3),
                            Cantidad = reader.GetInt32(4),
                            StockAnterior = reader.GetInt32(5),
                            StockNuevo = reader.GetInt32(6),
                            Motivo = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            Fecha = reader.GetDateTime(8)
                        });
                    }
                }
            }
            return lista;
        }
        public bool AgregarMovimiento(Movimiento m)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO Movimientos (Tipo, ProductoId, Cantidad, StockAnterior, StockNuevo, Motivo) VALUES (@t, @p, @c, @sa, @sn, @m)", conn))
                {
                    cmd.Parameters.AddWithValue("t", m.Tipo);
                    cmd.Parameters.AddWithValue("p", m.ProductoId);
                    cmd.Parameters.AddWithValue("c", m.Cantidad);
                    cmd.Parameters.AddWithValue("sa", m.StockAnterior);
                    cmd.Parameters.AddWithValue("sn", m.StockNuevo);
                    cmd.Parameters.AddWithValue("m", m.Motivo ?? "");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
