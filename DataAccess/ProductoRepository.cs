using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class ProductoRepository
    {
        private const string SelectProductos = "SELECT Id, Nombre, Categoria, Clasificacion, Detalle, CodigoBarras, PrecioCompra, PrecioVenta, Stock, StockMinimo, ImagePath FROM Productos";

        public ProductoRepository()
        {
            AsegurarCamposProducto();
            SembrarProductosProfesionales();
        }

        private void AsegurarCamposProducto()
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "ALTER TABLE Productos ADD COLUMN IF NOT EXISTS CodigoBarras VARCHAR(80);" +
                    "ALTER TABLE Productos ADD COLUMN IF NOT EXISTS Clasificacion VARCHAR(120);" +
                    "ALTER TABLE Productos ADD COLUMN IF NOT EXISTS Detalle VARCHAR(120);" +
                    "CREATE UNIQUE INDEX IF NOT EXISTS ux_productos_codigobarras ON Productos (CodigoBarras) WHERE CodigoBarras IS NOT NULL AND CodigoBarras <> '';",
                    conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SembrarProductosProfesionales()
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
INSERT INTO Productos (Nombre, Categoria, Clasificacion, Detalle, CodigoBarras, PrecioCompra, PrecioVenta, Stock, StockMinimo, ImagePath) VALUES
('Lapiz Mirado No. 2 HB', 'Escritura', 'Lapices', 'Numero 2', '7502000000018', 3.00, 6.00, 240, 30, 'lapiz.png'),
('Lapiz grafito 2B profesional', 'Escritura', 'Lapices', '2B', '7502000000025', 5.00, 12.00, 120, 20, 'lapiz.png'),
('Lapicero gel azul punta fina 0.5 mm', 'Escritura', 'Lapiceros', 'Punta fina', '7502000000032', 6.50, 15.00, 180, 25, 'pluma_bic.png'),
('Lapicero tinta negra punta media 0.7 mm', 'Escritura', 'Lapiceros', 'Punta media', '7502000000049', 5.50, 13.00, 180, 25, 'pluma_bic.png'),
('Lapicero rojo punta gruesa 1.0 mm', 'Escritura', 'Lapiceros', 'Punta gruesa', '7502000000056', 5.50, 13.00, 100, 20, 'pluma_bic.png'),
('Crayones jumbo lavables 12 colores', 'Escritura', 'Crayones', 'Jumbo', '7502000000063', 28.00, 55.00, 70, 12, ''),
('Colores de madera 24 piezas', 'Escritura', 'Colores', 'Madera 24 piezas', '7502000000070', 48.00, 89.00, 80, 12, ''),
('Marcador permanente negro punta media', 'Escritura', 'Marcadores', 'Permanentes', '7502000000087', 12.00, 24.00, 90, 15, 'marcadores.png'),
('Marcadores para pizarron set 4 colores', 'Escritura', 'Marcadores', 'Para pizarron', '7502000000094', 45.00, 89.00, 45, 10, 'marcadores.png'),
('Resaltador pastel set 6 piezas', 'Escritura', 'Resaltadores', 'Pastel', '7502000000100', 42.00, 85.00, 60, 10, ''),
('Corrector en cinta 5 mm x 8 m', 'Escritura', 'Correctores', 'Cinta', '7502000000117', 14.00, 29.00, 85, 15, ''),
('Resma papel bond carta 500 hojas', 'Papel', 'Hojas', 'Carta', '7502000000124', 82.00, 129.00, 55, 10, 'resma.png'),
('Resma papel bond oficio 500 hojas', 'Papel', 'Hojas', 'Oficio', '7502000000131', 96.00, 149.00, 45, 10, 'resma.png'),
('Cuaderno profesional cuadro chico 100 hojas', 'Papel', 'Cuadernos', 'Cuadro chico', '7502000000148', 18.00, 39.00, 160, 25, 'cuaderno.png'),
('Cuaderno profesional raya 100 hojas', 'Papel', 'Cuadernos', 'Raya', '7502000000155', 18.00, 39.00, 160, 25, 'cuaderno.png'),
('Libreta taquigrafia 80 hojas', 'Papel', 'Libretas', 'Taquigrafia', '7502000000162', 12.00, 26.00, 110, 20, ''),
('Cartulina blanca escolar', 'Papel', 'Cartulinas', 'Blanca', '7502000000179', 3.50, 8.00, 200, 40, ''),
('Papel crepe color surtido', 'Papel', 'Papeles especiales', 'Crepe', '7502000000186', 5.00, 12.00, 150, 25, ''),
('Block marquilla 20 hojas', 'Papel', 'Blocks', 'Marquilla', '7502000000193', 28.00, 55.00, 50, 10, ''),
('Pegamento en barra 22 g', 'Adhesivos', 'Pegamentos', 'Barra', '7502000000209', 8.00, 18.00, 120, 25, 'pegamento.png'),
('Silicon frio escolar 100 ml', 'Adhesivos', 'Pegamentos', 'Silicon frio', '7502000000216', 14.00, 32.00, 65, 12, ''),
('Cinta transparente 18 mm', 'Adhesivos', 'Cintas', 'Transparente', '7502000000223', 7.00, 16.00, 100, 20, ''),
('Cinta masking tape 24 mm', 'Adhesivos', 'Cintas', 'Masking tape', '7502000000230', 16.00, 34.00, 60, 12, ''),
('Etiquetas blancas carta 100 hojas', 'Adhesivos', 'Etiquetas', 'Adhesivas carta', '7502000000247', 95.00, 169.00, 20, 5, ''),
('Folder manila tamano carta paquete 100', 'Organizacion', 'Folders', 'Manila carta', '7502000000254', 95.00, 155.00, 35, 8, ''),
('Carpeta argolla blanca 1 pulgada', 'Organizacion', 'Carpetas', 'Argolla 1 pulgada', '7502000000261', 28.00, 59.00, 55, 10, 'carpeta.png'),
('Archivador acordeon 13 divisiones', 'Organizacion', 'Archivadores', 'Acordeon', '7502000000278', 85.00, 149.00, 30, 6, ''),
('Agenda ejecutiva semanal', 'Organizacion', 'Agendas', 'Ejecutiva', '7502000000285', 78.00, 139.00, 25, 5, ''),
('Grapadora metalica escritorio', 'Oficina', 'Grapado', 'Grapadora escritorio', '7502000000292', 65.00, 119.00, 30, 6, ''),
('Caja grapas estandar 5000 piezas', 'Oficina', 'Grapado', 'Grapas estandar', '7502000000308', 18.00, 39.00, 80, 15, ''),
('Binder clip mediano caja 12 piezas', 'Oficina', 'Clips y broches', 'Binder clip mediano', '7502000000315', 22.00, 45.00, 70, 12, ''),
('Perforadora dos orificios', 'Oficina', 'Accesorios', 'Perforadora', '7502000000322', 52.00, 99.00, 35, 7, ''),
('Pintura acrilica escolar 250 ml', 'Arte y dibujo', 'Pinturas', 'Acrilica', '7502000000339', 25.00, 52.00, 65, 12, ''),
('Pinceles set escolar 6 piezas', 'Arte y dibujo', 'Pinceles', 'Set escolar', '7502000000346', 24.00, 49.00, 70, 12, ''),
('Juego geometrico flexible 4 piezas', 'Corte y medicion', 'Geometria', 'Juego geometrico', '7502000000353', 18.00, 38.00, 90, 18, ''),
('Tijeras escolares punta roma', 'Corte y medicion', 'Tijeras', 'Escolar punta roma', '7502000000360', 12.00, 28.00, 75, 12, 'tijeras.png'),
('Cutter grande con seguro', 'Corte y medicion', 'Cutters', 'Grande', '7502000000377', 18.00, 39.00, 45, 8, ''),
('Calculadora cientifica escolar', 'Tecnologia e impresion', 'Accesorios', 'Calculadora cientifica', '7502000000384', 145.00, 249.00, 18, 4, ''),
('Memoria USB 32 GB', 'Tecnologia e impresion', 'Accesorios', 'USB', '7502000000391', 72.00, 129.00, 28, 6, ''),
('Tinta Epson negra botella', 'Tecnologia e impresion', 'Tintas', 'Epson', '7502000000407', 135.00, 219.00, 22, 5, ''),
('Sobre manila carta paquete 50', 'Empaque y regalos', 'Sobres', 'Manila', '7502000000414', 48.00, 89.00, 40, 8, ''),
('Bolsa de regalo mediana', 'Empaque y regalos', 'Bolsas', 'Regalo mediana', '7502000000421', 7.00, 18.00, 120, 20, ''),
('Papel regalo infantil', 'Empaque y regalos', 'Envolturas', 'Papel regalo', '7502000000438', 5.50, 15.00, 140, 25, ''),
('Mochila escolar primaria reforzada', 'Escolar y mochilas', 'Mochilas', 'Primaria', '7502000000445', 190.00, 329.00, 16, 4, ''),
('Lapicera escolar doble cierre', 'Escolar y mochilas', 'Mochilas', 'Lapicera', '7502000000452', 35.00, 75.00, 45, 8, ''),
('Plastilina 12 colores', 'Escolar y mochilas', 'Didactico', 'Plastilina', '7502000000469', 22.00, 45.00, 80, 15, ''),
('Gel antibacterial 250 ml', 'Limpieza y varios', 'Limpieza', 'Gel antibacterial', '7502000000476', 24.00, 49.00, 50, 10, ''),
('Pilas alcalinas AA paquete 4', 'Limpieza y varios', 'Varios', 'Pilas AA', '7502000000483', 34.00, 69.00, 55, 10, '')
ON CONFLICT (CodigoBarras) WHERE CodigoBarras IS NOT NULL AND CodigoBarras <> '' DO NOTHING;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Producto> ObtenerTodos()
        {
            var lista = new List<Producto>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(SelectProductos + " ORDER BY Nombre", conn))
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
                using (var cmd = new NpgsqlCommand(SelectProductos + " WHERE Nombre ILIKE @t OR CodigoBarras ILIKE @t OR Categoria ILIKE @t OR Clasificacion ILIKE @t OR Detalle ILIKE @t ORDER BY Nombre", conn))
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
                using (var cmd = new NpgsqlCommand(SelectProductos + " WHERE CodigoBarras = @codigo LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("codigo", codigo.Trim());
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.Read() ? MapearProducto(reader) : null;
                    }
                }
            }
        }

        public List<string> ObtenerCategorias()
        {
            var categorias = new List<string>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT DISTINCT Categoria FROM Productos WHERE Categoria IS NOT NULL AND Categoria <> '' ORDER BY Categoria", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        categorias.Add(reader.GetString(0));
                }
            }
            return categorias;
        }

        public List<string> ObtenerClasificaciones(string categoria)
        {
            var clasificaciones = new List<string>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT DISTINCT Clasificacion FROM Productos WHERE (@categoria = '' OR Categoria = @categoria) AND Clasificacion IS NOT NULL AND Clasificacion <> '' ORDER BY Clasificacion", conn))
                {
                    cmd.Parameters.AddWithValue("categoria", categoria == "Todas" ? "" : (categoria ?? ""));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            clasificaciones.Add(reader.GetString(0));
                    }
                }
            }
            return clasificaciones;
        }

        public List<string> ObtenerDetalles(string categoria, string clasificacion)
        {
            var detalles = new List<string>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT DISTINCT Detalle FROM Productos WHERE (@categoria = '' OR Categoria = @categoria) AND (@clasificacion = '' OR Clasificacion = @clasificacion) AND Detalle IS NOT NULL AND Detalle <> '' ORDER BY Detalle", conn))
                {
                    cmd.Parameters.AddWithValue("categoria", categoria == "Todas" ? "" : (categoria ?? ""));
                    cmd.Parameters.AddWithValue("clasificacion", clasificacion == "Todas" ? "" : (clasificacion ?? ""));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            detalles.Add(reader.GetString(0));
                    }
                }
            }
            return detalles;
        }

        public List<Producto> BuscarPorCategoria(string categoria)
        {
            return BuscarPorFiltros(categoria, "Todas", "Todos");
        }

        public List<Producto> BuscarPorFiltros(string categoria, string clasificacion, string detalle)
        {
            if ((string.IsNullOrEmpty(categoria) || categoria == "Todas") &&
                (string.IsNullOrEmpty(clasificacion) || clasificacion == "Todas") &&
                (string.IsNullOrEmpty(detalle) || detalle == "Todos"))
                return ObtenerTodos();

            var lista = new List<Producto>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(SelectProductos + " WHERE (@c = '' OR Categoria = @c) AND (@cl = '' OR Clasificacion = @cl) AND (@d = '' OR Detalle = @d) ORDER BY Nombre", conn))
                {
                    cmd.Parameters.AddWithValue("c", categoria == "Todas" ? "" : (categoria ?? ""));
                    cmd.Parameters.AddWithValue("cl", clasificacion == "Todas" ? "" : (clasificacion ?? ""));
                    cmd.Parameters.AddWithValue("d", detalle == "Todos" ? "" : (detalle ?? ""));
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
                Clasificacion = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Detalle = reader.IsDBNull(4) ? "" : reader.GetString(4),
                CodigoBarras = reader.IsDBNull(5) ? "" : reader.GetString(5),
                PrecioCompra = reader.GetDecimal(6),
                PrecioVenta = reader.GetDecimal(7),
                Stock = reader.GetInt32(8),
                StockMinimo = reader.GetInt32(9),
                ImagePath = reader.IsDBNull(10) ? "" : reader.GetString(10)
            };
        }

        public bool AgregarProducto(Producto p)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO Productos (Nombre, Categoria, Clasificacion, Detalle, CodigoBarras, PrecioCompra, PrecioVenta, Stock, StockMinimo, ImagePath) VALUES (@n, @c, @cl, @d, @cb, @pc, @pv, @s, @sm, @i)", conn))
                {
                    cmd.Parameters.AddWithValue("n", p.Nombre);
                    cmd.Parameters.AddWithValue("c", p.Categoria ?? "");
                    cmd.Parameters.AddWithValue("cl", p.Clasificacion ?? "");
                    cmd.Parameters.AddWithValue("d", p.Detalle ?? "");
                    cmd.Parameters.AddWithValue("cb", p.CodigoBarras ?? "");
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
                    "UPDATE Productos SET Nombre=@n, Categoria=@c, Clasificacion=@cl, Detalle=@d, CodigoBarras=@cb, PrecioCompra=@pc, PrecioVenta=@pv, Stock=@s, StockMinimo=@sm, ImagePath=@i WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("n", p.Nombre);
                    cmd.Parameters.AddWithValue("c", p.Categoria ?? "");
                    cmd.Parameters.AddWithValue("cl", p.Clasificacion ?? "");
                    cmd.Parameters.AddWithValue("d", p.Detalle ?? "");
                    cmd.Parameters.AddWithValue("cb", p.CodigoBarras ?? "");
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
