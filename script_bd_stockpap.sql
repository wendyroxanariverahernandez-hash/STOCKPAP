-- script_bd_stockpap.sql
-- Base de datos: stockpap_db

-- Eliminar tablas y vistas si existen
DROP VIEW IF EXISTS vista_productos_proveedores CASCADE;
DROP VIEW IF EXISTS vista_ventas_resumen CASCADE;

DROP TABLE IF EXISTS Detalle_Ventas CASCADE;
DROP TABLE IF EXISTS Ventas CASCADE;
DROP TABLE IF EXISTS Movimientos CASCADE;
DROP TABLE IF EXISTS Proveedor_Producto CASCADE;
DROP TABLE IF EXISTS Proveedores CASCADE;
DROP TABLE IF EXISTS Productos CASCADE;
DROP TABLE IF EXISTS Usuarios CASCADE;

-- Eliminar dominios si existen
DROP DOMAIN IF EXISTS dom_precio CASCADE;
DROP DOMAIN IF EXISTS dom_stock CASCADE;
DROP DOMAIN IF EXISTS dom_email CASCADE;

-- 1. Tipos de Dominio
CREATE DOMAIN dom_precio AS NUMERIC(10,2) CHECK (VALUE >= 0.00);
CREATE DOMAIN dom_stock AS INTEGER CHECK (VALUE >= 0);
CREATE DOMAIN dom_email AS VARCHAR(150) CHECK (VALUE LIKE '%@%');

-- A) Las instrucciones SQL para la creación de tablas (con llaves primarias y foráneas).
CREATE TABLE Usuarios (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Rol VARCHAR(50) NOT NULL
);

CREATE TABLE Productos (
    Id SERIAL PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL,
    Categoria VARCHAR(100),
    CodigoBarras VARCHAR(80),
    PrecioCompra dom_precio NOT NULL DEFAULT 0.00,
    PrecioVenta dom_precio NOT NULL DEFAULT 0.00,
    Stock dom_stock NOT NULL DEFAULT 0,
    StockMinimo dom_stock NOT NULL DEFAULT 10,
    ImagePath VARCHAR(500)
);

CREATE UNIQUE INDEX ux_productos_codigobarras
ON Productos (CodigoBarras)
WHERE CodigoBarras IS NOT NULL AND CodigoBarras <> '';

CREATE TABLE Proveedores (
    Id SERIAL PRIMARY KEY,
    Empresa VARCHAR(150) NOT NULL,
    Contacto VARCHAR(100),
    Telefono VARCHAR(20),
    Email dom_email,
    Direccion TEXT
);

CREATE TABLE Proveedor_Producto (
    ProveedorId INTEGER REFERENCES Proveedores(Id) ON DELETE CASCADE,
    ProductoId INTEGER REFERENCES Productos(Id) ON DELETE CASCADE,
    PRIMARY KEY (ProveedorId, ProductoId)
);

CREATE TABLE Movimientos (
    Id SERIAL PRIMARY KEY,
    Tipo VARCHAR(50) NOT NULL, -- 'Entrada', 'Salida', 'Ajuste'
    ProductoId INTEGER REFERENCES Productos(Id) ON DELETE CASCADE,
    Cantidad INTEGER NOT NULL,
    StockAnterior dom_stock NOT NULL,
    StockNuevo dom_stock NOT NULL,
    Motivo VARCHAR(255),
    Fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Ventas (
    Id SERIAL PRIMARY KEY,
    Fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Subtotal dom_precio NOT NULL DEFAULT 0.00,
    Iva dom_precio NOT NULL DEFAULT 0.00,
    Total dom_precio NOT NULL DEFAULT 0.00
);

CREATE TABLE Detalle_Ventas (
    Id SERIAL PRIMARY KEY,
    VentaId INTEGER REFERENCES Ventas(Id) ON DELETE CASCADE,
    ProductoId INTEGER REFERENCES Productos(Id) ON DELETE RESTRICT,
    Cantidad INTEGER NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario dom_precio NOT NULL,
    Subtotal dom_precio NOT NULL
);

-- Creación de Vistas
CREATE VIEW vista_productos_proveedores AS
SELECT p.Id AS ProductoId, p.Nombre AS Producto, p.Categoria, pr.Empresa AS Proveedor, pr.Contacto
FROM Productos p
JOIN Proveedor_Producto pp ON p.Id = pp.ProductoId
JOIN Proveedores pr ON pp.ProveedorId = pr.Id;

CREATE VIEW vista_ventas_resumen AS
SELECT v.Id AS VentaId, v.Fecha, v.Total, COUNT(dv.Id) AS Cantidad_Articulos
FROM Ventas v
LEFT JOIN Detalle_Ventas dv ON v.Id = dv.VentaId
GROUP BY v.Id, v.Fecha, v.Total;

-- B) Los comandos SQL - DML, de agregar nuevos registros en cada una de las tablas creadas.
INSERT INTO Usuarios (Username, Password, Rol) VALUES 
('administrador', 'wendy123', 'Admin'),
('cajero', 'cajero123', 'Ventas'),
('consulta', 'consulta123', 'Consulta');

INSERT INTO Productos (Nombre, Categoria, PrecioCompra, PrecioVenta, Stock, StockMinimo, ImagePath) VALUES 
('Cuaderno Profesional 100 hojas', 'Cuadernos', 15.00, 35.00, 150, 20, 'cuaderno.png'),
('Pluma BIC Cristal Azul', 'Escritura', 2.50, 45.00, 80, 10, 'pluma_bic.png'),
('Lápiz Mirado No. 2', 'Escritura', 2.00, 25.00, 120, 20, 'lapiz.png'),
('Resma Papel Bond A4', 'Papel', 80.00, 120.00, 45, 10, 'resma.png'),
('Marcadores de Colores', 'Marcadores', 40.00, 65.00, 8, 15, 'marcadores.png'),
('Carpeta Tamaño Carta', 'Organización', 25.00, 55.00, 60, 15, 'carpeta.png'),
('Pegamento en Barra', 'Adhesivos', 8.00, 18.00, 100, 30, 'pegamento.png'),
('Tijeras Escolares', 'Corte', 12.00, 28.00, 50, 10, 'tijeras.png');

INSERT INTO Proveedores (Empresa, Contacto, Telefono, Email, Direccion) VALUES
('Distribuidora Escolar del Centro', 'Juan Pérez', '555-1234-5678', 'ventas@descolar.com', 'Av. Juárez 123, CDMX'),
('Papelería y Más SA de CV', 'María García', '555-8765-4321', 'contacto@papeleriaymas.com', 'Calle Insurgentes 456, CDMX');

INSERT INTO Proveedor_Producto (ProveedorId, ProductoId) VALUES
(1, 1), (1, 3), (1, 6),
(2, 2), (2, 4), (2, 7);

INSERT INTO Movimientos (Tipo, ProductoId, Cantidad, StockAnterior, StockNuevo, Motivo) VALUES
('Entrada', 1, 50, 100, 150, 'Compra a proveedor'),
('Entrada', 2, 30, 50, 80, 'Reabastecimiento'),
('Salida', 5, -7, 15, 8, 'Venta a cliente'),
('Salida', 4, -5, 50, 45, 'Pedido mayoreo');

INSERT INTO Ventas (Fecha, Subtotal, Iva, Total) VALUES
('2026-06-01 10:00:00', 100.00, 16.00, 116.00),
('2026-06-02 11:30:00', 50.00, 8.00, 58.00);

INSERT INTO Detalle_Ventas (VentaId, ProductoId, Cantidad, PrecioUnitario, Subtotal) VALUES
(1, 1, 2, 35.00, 70.00),
(1, 2, 1, 45.00, 45.00),
(2, 3, 2, 25.00, 50.00);
