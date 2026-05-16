using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class ProductoDAO
    {
        public List<Producto> GetAll()
        {
            var list = new List<Producto>();
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM Productos ORDER BY Id", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Producto
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Categoria = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            PrecioCompra = reader.GetDecimal(3),
                            PrecioVenta = reader.GetDecimal(4),
                            Stock = reader.GetInt32(5)
                        });
                    }
                }
            }
            return list;
        }

        public void Add(Producto producto)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO Productos (Nombre, Categoria, PrecioCompra, PrecioVenta, Stock) VALUES (@nombre, @categoria, @precioCompra, @precioVenta, @stock)", connection))
                {
                    command.Parameters.AddWithValue("nombre", producto.Nombre);
                    command.Parameters.AddWithValue("categoria", producto.Categoria ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("precioCompra", producto.PrecioCompra);
                    command.Parameters.AddWithValue("precioVenta", producto.PrecioVenta);
                    command.Parameters.AddWithValue("stock", producto.Stock);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Producto producto)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("UPDATE Productos SET Nombre=@nombre, Categoria=@categoria, PrecioCompra=@precioCompra, PrecioVenta=@precioVenta, Stock=@stock WHERE Id=@id", connection))
                {
                    command.Parameters.AddWithValue("nombre", producto.Nombre);
                    command.Parameters.AddWithValue("categoria", producto.Categoria ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("precioCompra", producto.PrecioCompra);
                    command.Parameters.AddWithValue("precioVenta", producto.PrecioVenta);
                    command.Parameters.AddWithValue("stock", producto.Stock);
                    command.Parameters.AddWithValue("id", producto.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = Conexion.Instance.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM Productos WHERE Id=@id", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
