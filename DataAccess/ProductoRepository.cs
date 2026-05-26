using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class ProductoRepository
    {
        public List<Producto> ObtenerTodos()
        {
            var lista = new List<Producto>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT Id, Nombre, Categoria, PrecioCompra, PrecioVenta, Stock, StockMinimo, ImagePath FROM Productos ORDER BY Nombre", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lista.Add(MapearProducto(reader));
                }
            }
            return lista;
        }

        public List<Producto> BuscarPorNombre(string texto)
        {
            var lista = new List<Producto>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT Id, Nombre, Categoria, PrecioCompra, PrecioVenta, Stock, StockMinimo, ImagePath FROM Productos WHERE Nombre ILIKE @t ORDER BY Nombre", conn))
                {
                    cmd.Parameters.AddWithValue("t", $"%{texto}%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(MapearProducto(reader));
                    }
                }
            }
            return lista;
        }

        public List<Producto> BuscarPorCategoria(string categoria)
        {
            if (string.IsNullOrEmpty(categoria) || categoria == "Todas")
                return ObtenerTodos();

            var lista = new List<Producto>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT Id, Nombre, Categoria, PrecioCompra, PrecioVenta, Stock, StockMinimo, ImagePath FROM Productos WHERE Categoria = @c ORDER BY Nombre", conn))
                {
                    cmd.Parameters.AddWithValue("c", categoria);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(MapearProducto(reader));
                    }
                }
            }
            return lista;
        }

        private Producto MapearProducto(NpgsqlDataReader reader)
        {
            return new Producto
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Categoria = reader.IsDBNull(2) ? "" : reader.GetString(2),
                PrecioCompra = reader.GetDecimal(3),
                PrecioVenta = reader.GetDecimal(4),
                Stock = reader.GetInt32(5),
                StockMinimo = reader.GetInt32(6),
                ImagePath = reader.IsDBNull(7) ? "" : reader.GetString(7)
            };
        }

        public bool AgregarProducto(Producto p)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO Productos (Nombre, Categoria, PrecioCompra, PrecioVenta, Stock, StockMinimo, ImagePath) VALUES (@n, @c, @pc, @pv, @s, @sm, @i)", conn))
                {
                    cmd.Parameters.AddWithValue("n", p.Nombre);
                    cmd.Parameters.AddWithValue("c", p.Categoria ?? "");
                    cmd.Parameters.AddWithValue("pc", p.PrecioCompra);
                    cmd.Parameters.AddWithValue("pv", p.PrecioVenta);
                    cmd.Parameters.AddWithValue("s", p.Stock);
                    cmd.Parameters.AddWithValue("sm", p.StockMinimo);
                    cmd.Parameters.AddWithValue("i", p.ImagePath ?? "");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ActualizarProducto(Producto p)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "UPDATE Productos SET Nombre=@n, Categoria=@c, PrecioCompra=@pc, PrecioVenta=@pv, Stock=@s, StockMinimo=@sm, ImagePath=@i WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("n", p.Nombre);
                    cmd.Parameters.AddWithValue("c", p.Categoria ?? "");
                    cmd.Parameters.AddWithValue("pc", p.PrecioCompra);
                    cmd.Parameters.AddWithValue("pv", p.PrecioVenta);
                    cmd.Parameters.AddWithValue("s", p.Stock);
                    cmd.Parameters.AddWithValue("sm", p.StockMinimo);
                    cmd.Parameters.AddWithValue("i", p.ImagePath ?? "");
                    cmd.Parameters.AddWithValue("id", p.Id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool EliminarProducto(int id)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM Productos WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ActualizarStock(int id, int nuevoStock)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE Productos SET Stock = @s WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("s", nuevoStock);
                    cmd.Parameters.AddWithValue("id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
