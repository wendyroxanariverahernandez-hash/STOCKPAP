using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class ProductoRepository
    {
        private const string SelectProductos = @"SELECT p.Id, p.Nombre, p.CodigoBarras, 
            p.PrecioCompra, p.PrecioVenta, p.Stock, p.StockMinimo, p.ImagePath, 
            p.Clase, p.Subclase, p.Marca,
            pp.ProveedorId, prov.Contacto AS ProveedorNombre
            FROM Productos p
            LEFT JOIN Proveedor_Producto pp ON pp.ProductoId = p.Id
            LEFT JOIN Proveedores prov ON prov.Id = pp.ProveedorId";

        public ProductoRepository()
        {
            AsegurarCamposProducto();
            // SembrarProductosProfesionales();
        }

        private void AsegurarCamposProducto()
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "ALTER TABLE Productos ADD COLUMN IF NOT EXISTS CodigoBarras VARCHAR(80);" +
                    "ALTER TABLE Productos ADD COLUMN IF NOT EXISTS Clase VARCHAR(120);" +
                    "ALTER TABLE Productos ADD COLUMN IF NOT EXISTS Subclase VARCHAR(120);" +
                    "ALTER TABLE Productos ADD COLUMN IF NOT EXISTS Marca VARCHAR(120);" +
                    "ALTER TABLE Productos DROP COLUMN IF EXISTS Categoria CASCADE;" +
                    "ALTER TABLE Productos DROP COLUMN IF EXISTS Clasificacion CASCADE;" +
                    "ALTER TABLE Productos DROP COLUMN IF EXISTS Detalle CASCADE;" +
                    "ALTER TABLE Productos DROP COLUMN IF EXISTS Tipo CASCADE;" +
                    "CREATE UNIQUE INDEX IF NOT EXISTS ux_productos_codigobarras ON Productos (CodigoBarras) WHERE CodigoBarras IS NOT NULL AND CodigoBarras <> '';" +
                    "CREATE TABLE IF NOT EXISTS AlertasStock (" +
                    "    Id SERIAL PRIMARY KEY," +
                    "    ProductoId INTEGER REFERENCES Productos(Id) ON DELETE CASCADE," +
                    "    StockActual INTEGER NOT NULL," +
                    "    StockMinimo INTEGER NOT NULL," +
                    "    FechaGeneracion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP," +
                    "    Resuelta BOOLEAN NOT NULL DEFAULT FALSE," +
                    "    FechaResolucion TIMESTAMP" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS Configuracion (" +
                    "    Clave VARCHAR(100) PRIMARY KEY," +
                    "    Valor VARCHAR(255) NOT NULL" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS Clases (" +
                    "    Id SERIAL PRIMARY KEY," +
                    "    Nombre VARCHAR(120) UNIQUE NOT NULL" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS Subclases (" +
                    "    Id SERIAL PRIMARY KEY," +
                    "    Nombre VARCHAR(120) NOT NULL," +
                    "    ClaseId INTEGER REFERENCES Clases(Id) ON DELETE CASCADE," +
                    "    UNIQUE (Nombre, ClaseId)" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS Marcas (" +
                    "    Id SERIAL PRIMARY KEY," +
                    "    Nombre VARCHAR(120) NOT NULL," +
                    "    ClaseId INTEGER REFERENCES Clases(Id) ON DELETE CASCADE," +
                    "    UNIQUE (Nombre, ClaseId)" +
                    ");" +
                    "INSERT INTO Configuracion (Clave, Valor) VALUES " +
                    "('StockMinimoGlobal', '10'), " +
                    "('AlertasActivas', 'true'), " +
                    "('FormatoReporteDefecto', 'PDF') " +
                    "ON CONFLICT (Clave) DO NOTHING;",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SembrarProductosProfesionales()
        {
            // Omitted since it contains old columns. We don't need it.
        }

        public List<Producto> ObtenerTodos()
        {
            var lista = new List<Producto>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(SelectProductos + " ORDER BY p.Nombre", conn))
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
                using (var cmd = new NpgsqlCommand(SelectProductos + " WHERE p.Nombre ILIKE @t OR p.CodigoBarras ILIKE @t OR p.Clase ILIKE @t OR p.Subclase ILIKE @t OR p.Marca ILIKE @t ORDER BY p.Nombre", conn))
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

        public Producto BuscarPorCodigoBarras(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(SelectProductos + " WHERE p.CodigoBarras = @codigo LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("codigo", codigo.Trim());
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.Read() ? MapearProducto(reader) : null;
                    }
                }
            }
        }

        public List<string> ObtenerClases()
        {
            var lista = new List<string>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT Nombre FROM Clases ORDER BY Nombre", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lista.Add(reader.GetString(0));
                }
            }
            return lista;
        }

        public List<string> ObtenerSubclases(string clase)
        {
            var lista = new List<string>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT s.Nombre FROM Subclases s JOIN Clases c ON s.ClaseId = c.Id WHERE (@clase = '' OR c.Nombre = @clase) ORDER BY s.Nombre", conn))
                {
                    cmd.Parameters.AddWithValue("clase", clase == "Todas" ? "" : (clase ?? ""));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(reader.GetString(0));
                    }
                }
            }
            return lista;
        }

        public List<string> ObtenerMarcas(string clase, string subclase)
        {
            var lista = new List<string>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT m.Nombre FROM Marcas m JOIN Clases c ON m.ClaseId = c.Id WHERE (@clase = '' OR c.Nombre = @clase) ORDER BY m.Nombre", conn))
                {
                    cmd.Parameters.AddWithValue("clase", clase == "Todas" ? "" : (clase ?? ""));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            lista.Add(reader.GetString(0));
                    }
                }
            }
            return lista;
        }

        public List<Producto> BuscarPorNuevosFiltros(string clase, string subclase, string marca, string tipo)
        {
            var lista = new List<Producto>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(SelectProductos + " WHERE (@c = '' OR p.Clase = @c) AND (@s = '' OR p.Subclase = @s) AND (@m = '' OR p.Marca = @m) ORDER BY p.Nombre", conn))
                {
                    cmd.Parameters.AddWithValue("c", clase == "Todas" ? "" : (clase ?? ""));
                    cmd.Parameters.AddWithValue("s", subclase == "Todas" ? "" : (subclase ?? ""));
                    cmd.Parameters.AddWithValue("m", marca == "Todas" ? "" : (marca ?? ""));
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
                CodigoBarras = reader.IsDBNull(2) ? "" : reader.GetString(2),
                PrecioCompra = reader.GetDecimal(3),
                PrecioVenta = reader.GetDecimal(4),
                Stock = reader.GetInt32(5),
                StockMinimo = reader.GetInt32(6),
                ImagePath = reader.IsDBNull(7) ? "" : reader.GetString(7),
                Clase = reader.IsDBNull(8) ? "" : reader.GetString(8),
                Subclase = reader.IsDBNull(9) ? "" : reader.GetString(9),
                Marca = reader.IsDBNull(10) ? "" : reader.GetString(10),
                ProveedorId = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                ProveedorNombre = reader.IsDBNull(12) ? "" : reader.GetString(12)
            };
        }

        public bool AgregarProducto(Producto p)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        int id = 0;
                        using (var cmd = new NpgsqlCommand(
                            @"INSERT INTO Productos (Nombre, CodigoBarras, PrecioCompra, PrecioVenta, Stock, StockMinimo, ImagePath, Clase, Subclase, Marca) 
                              VALUES (@n, @cb, @pc, @pv, @s, @sm, @i, @cls, @sub, @mrc) RETURNING Id", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("n", p.Nombre);
                            cmd.Parameters.AddWithValue("cb", p.CodigoBarras ?? "");
                            cmd.Parameters.AddWithValue("pc", p.PrecioCompra);
                            cmd.Parameters.AddWithValue("pv", p.PrecioVenta);
                            cmd.Parameters.AddWithValue("s", p.Stock);
                            cmd.Parameters.AddWithValue("sm", p.StockMinimo);
                            cmd.Parameters.AddWithValue("i", p.ImagePath ?? "");
                            cmd.Parameters.AddWithValue("cls", p.Clase ?? "");
                            cmd.Parameters.AddWithValue("sub", p.Subclase ?? "");
                            cmd.Parameters.AddWithValue("mrc", p.Marca ?? "");
                            id = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        if (p.ProveedorId.HasValue)
                        {
                            using (var cmdProv = new NpgsqlCommand("INSERT INTO Proveedor_Producto (ProveedorId, ProductoId) VALUES (@prov, @prod)", conn, trans))
                            {
                                cmdProv.Parameters.AddWithValue("prov", p.ProveedorId.Value);
                                cmdProv.Parameters.AddWithValue("prod", id);
                                cmdProv.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        p.Id = id;
                        
                        if (p.Stock <= p.StockMinimo)
                        {
                            RegistrarAlerta(p.Id, p.Stock, p.StockMinimo);
                        }

                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool ActualizarProducto(Producto p)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new NpgsqlCommand(
                            @"UPDATE Productos 
                              SET Nombre=@n, CodigoBarras=@cb, 
                                  PrecioCompra=@pc, PrecioVenta=@pv, Stock=@s, StockMinimo=@sm, ImagePath=@i,
                                  Clase=@cls, Subclase=@sub, Marca=@mrc
                              WHERE Id=@id", conn, trans))
                        {
                            cmd.Parameters.AddWithValue("n", p.Nombre);
                            cmd.Parameters.AddWithValue("cb", p.CodigoBarras ?? "");
                            cmd.Parameters.AddWithValue("pc", p.PrecioCompra);
                            cmd.Parameters.AddWithValue("pv", p.PrecioVenta);
                            cmd.Parameters.AddWithValue("s", p.Stock);
                            cmd.Parameters.AddWithValue("sm", p.StockMinimo);
                            cmd.Parameters.AddWithValue("i", p.ImagePath ?? "");
                            cmd.Parameters.AddWithValue("cls", (object)p.Clase ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("sub", (object)p.Subclase ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("mrc", (object)p.Marca ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("id", p.Id);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmdDel = new NpgsqlCommand("DELETE FROM Proveedor_Producto WHERE ProductoId = @prod", conn, trans))
                        {
                            cmdDel.Parameters.AddWithValue("prod", p.Id);
                            cmdDel.ExecuteNonQuery();
                        }

                        if (p.ProveedorId.HasValue)
                        {
                            using (var cmdProv = new NpgsqlCommand("INSERT INTO Proveedor_Producto (ProveedorId, ProductoId) VALUES (@prov, @prod)", conn, trans))
                            {
                                cmdProv.Parameters.AddWithValue("prov", p.ProveedorId.Value);
                                cmdProv.Parameters.AddWithValue("prod", p.Id);
                                cmdProv.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();

                        if (p.Stock <= p.StockMinimo)
                        {
                            RegistrarAlerta(p.Id, p.Stock, p.StockMinimo);
                        }
                        else
                        {
                            AutoResolverAlertas(p.Id, p.Stock, p.StockMinimo);
                        }

                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        return false;
                    }
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
                int stockMinimo = 10;
                using (var cmdMin = new NpgsqlCommand("SELECT StockMinimo FROM Productos WHERE Id = @id", conn))
                {
                    cmdMin.Parameters.AddWithValue("id", id);
                    var val = cmdMin.ExecuteScalar();
                    if (val != null && val != DBNull.Value)
                        stockMinimo = Convert.ToInt32(val);
                }

                using (var cmd = new NpgsqlCommand("UPDATE Productos SET Stock = @s WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("s", nuevoStock);
                    cmd.Parameters.AddWithValue("id", id);
                    bool ok = cmd.ExecuteNonQuery() > 0;
                    if (ok)
                    {
                        if (nuevoStock <= stockMinimo)
                            RegistrarAlerta(id, nuevoStock, stockMinimo);
                        else
                            AutoResolverAlertas(id, nuevoStock, stockMinimo);
                    }
                    return ok;
                }
            }
        }

        public List<AlertaStock> ObtenerAlertasActivas()
        {
            var lista = new List<AlertaStock>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT a.Id, a.ProductoId, p.Nombre, a.StockActual, a.StockMinimo, a.FechaGeneracion, a.Resuelta
                    FROM AlertasStock a
                    JOIN Productos p ON a.ProductoId = p.Id
                    WHERE a.Resuelta = FALSE
                    ORDER BY a.FechaGeneracion DESC";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new AlertaStock
                        {
                            Id = reader.GetInt32(0),
                            ProductoId = reader.GetInt32(1),
                            ProductoNombre = reader.GetString(2),
                            StockActual = reader.GetInt32(3),
                            StockMinimo = reader.GetInt32(4),
                            FechaGeneracion = reader.GetDateTime(5),
                            Resuelta = reader.GetBoolean(6)
                        });
                    }
                }
            }
            return lista;
        }

        public void RegistrarAlerta(int productoId, int stockActual, int stockMinimo)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmdCheck = new NpgsqlCommand("SELECT COUNT(*) FROM AlertasStock WHERE ProductoId = @p AND Resuelta = FALSE", conn))
                {
                    cmdCheck.Parameters.AddWithValue("p", productoId);
                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                    if (count > 0) return;
                }

                using (var cmd = new NpgsqlCommand("INSERT INTO AlertasStock (ProductoId, StockActual, StockMinimo) VALUES (@p, @sa, @sm)", conn))
                {
                    cmd.Parameters.AddWithValue("p", productoId);
                    cmd.Parameters.AddWithValue("sa", stockActual);
                    cmd.Parameters.AddWithValue("sm", stockMinimo);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool ResolverAlerta(int alertaId)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE AlertasStock SET Resuelta = TRUE, FechaResolucion = CURRENT_TIMESTAMP WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", alertaId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public void AutoResolverAlertas(int productoId, int stockNuevo, int stockMinimo)
        {
            if (stockNuevo > stockMinimo)
            {
                using (var conn = Conexion.Instance.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("UPDATE AlertasStock SET Resuelta = TRUE, FechaResolucion = CURRENT_TIMESTAMP WHERE ProductoId = @p AND Resuelta = FALSE", conn))
                    {
                        cmd.Parameters.AddWithValue("p", productoId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
