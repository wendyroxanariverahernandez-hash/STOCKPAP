using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class VentaRepository
    {
        public List<Venta> ObtenerTodas()
        {
            var lista = new List<Venta>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    @"SELECT v.Id, v.Fecha, v.Subtotal, v.Iva, v.Total,
                             d.ProductoId, p.Nombre, d.Cantidad, d.PrecioUnitario, d.Subtotal AS DetSub
                      FROM Ventas v
                      LEFT JOIN Detalle_Ventas d ON d.VentaId = v.Id
                      LEFT JOIN Productos p ON p.Id = d.ProductoId
                      ORDER BY v.Fecha DESC, v.Id", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    Venta ventaActual = null;
                    while (reader.Read())
                    {
                        int ventaId = reader.GetInt32(0);
                        if (ventaActual == null || ventaActual.Id != ventaId)
                        {
                            ventaActual = new Venta
                            {
                                Id       = ventaId,
                                Fecha    = reader.GetDateTime(1),
                                Subtotal = reader.GetDecimal(2),
                                Iva      = reader.GetDecimal(3),
                                Total    = reader.GetDecimal(4)
                            };
                            lista.Add(ventaActual);
                        }
                        // Detalle
                        if (!reader.IsDBNull(5))
                        {
                            ventaActual.Detalles.Add(new DetalleVenta
                            {
                                ProductoId      = reader.GetInt32(5),
                                ProductoNombre  = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                Cantidad        = reader.GetInt32(7),
                                PrecioUnitario  = reader.GetDecimal(8),
                                Subtotal        = reader.GetDecimal(9)
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool RegistrarVenta(Venta venta)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar Venta
                        int ventaId = 0;
                        using (var cmd = new NpgsqlCommand("INSERT INTO Ventas (Subtotal, Iva, Total) VALUES (@s, @i, @t) RETURNING Id", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("s", venta.Subtotal);
                            cmd.Parameters.AddWithValue("i", venta.Iva);
                            cmd.Parameters.AddWithValue("t", venta.Total);
                            ventaId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 2. Insertar Detalles y Actualizar Stock/Movimientos
                        foreach (var detalle in venta.Detalles)
                        {
                            // Insertar Detalle
                            using (var cmd = new NpgsqlCommand("INSERT INTO Detalle_Ventas (VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal) VALUES (@v, @p, @c, @pu, @sub)", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("v",   ventaId);
                                cmd.Parameters.AddWithValue("p",   detalle.ProductoId);
                                cmd.Parameters.AddWithValue("c",   detalle.Cantidad);
                                cmd.Parameters.AddWithValue("pu",  detalle.PrecioUnitario);
                                cmd.Parameters.AddWithValue("sub", detalle.Subtotal);
                                cmd.ExecuteNonQuery();
                            }

                            // Obtener stock actual y minimo
                            int stockAnterior = 0;
                            int stockMinimo = 10;
                            using (var cmdStock = new NpgsqlCommand("SELECT Stock, StockMinimo FROM Productos WHERE Id = @p", conn, transaction))
                            {
                                cmdStock.Parameters.AddWithValue("p", detalle.ProductoId);
                                using (var reader = cmdStock.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        stockAnterior = reader.GetInt32(0);
                                        stockMinimo = reader.GetInt32(1);
                                    }
                                }
                            }

                            int stockNuevo = stockAnterior - detalle.Cantidad;

                            // Actualizar Stock
                            using (var cmdUpd = new NpgsqlCommand("UPDATE Productos SET Stock = @s WHERE Id = @p", conn, transaction))
                            {
                                cmdUpd.Parameters.AddWithValue("s", stockNuevo);
                                cmdUpd.Parameters.AddWithValue("p", detalle.ProductoId);
                                cmdUpd.ExecuteNonQuery();
                            }

                            // Registrar Movimiento
                            using (var cmdMov = new NpgsqlCommand(
                                "INSERT INTO Movimientos (Tipo, ProductoId, Cantidad, StockAnterior, StockNuevo, Motivo) VALUES ('Salida', @p, @c, @sa, @sn, @m)",
                                conn, transaction))
                            {
                                cmdMov.Parameters.AddWithValue("p",  detalle.ProductoId);
                                cmdMov.Parameters.AddWithValue("c",  -detalle.Cantidad);
                                cmdMov.Parameters.AddWithValue("sa", stockAnterior);
                                cmdMov.Parameters.AddWithValue("sn", stockNuevo);
                                cmdMov.Parameters.AddWithValue("m", $"Venta #{ventaId}: {detalle.ProductoNombre} x{detalle.Cantidad}");
                                cmdMov.ExecuteNonQuery();
                            }

                            // Registrar Alerta de Stock Bajo si es necesario
                            if (stockNuevo <= stockMinimo)
                            {
                                bool alertaExiste = false;
                                using (var cmdCheck = new NpgsqlCommand("SELECT COUNT(*) FROM AlertasStock WHERE ProductoId = @p AND Resuelta = FALSE", conn, transaction))
                                {
                                    cmdCheck.Parameters.AddWithValue("p", detalle.ProductoId);
                                    alertaExiste = Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0;
                                }

                                if (!alertaExiste)
                                {
                                    using (var cmdAlert = new NpgsqlCommand("INSERT INTO AlertasStock (ProductoId, StockActual, StockMinimo) VALUES (@p, @sa, @sm)", conn, transaction))
                                    {
                                        cmdAlert.Parameters.AddWithValue("p", detalle.ProductoId);
                                        cmdAlert.Parameters.AddWithValue("sa", stockNuevo);
                                        cmdAlert.Parameters.AddWithValue("sm", stockMinimo);
                                        cmdAlert.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}

