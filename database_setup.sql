-- Base de datos: stockpap_db

CREATE TABLE IF NOT EXISTS Productos (
    Id SERIAL PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL,
    Categoria VARCHAR(100),
    PrecioCompra NUMERIC(10,2) NOT NULL DEFAULT 0.00,
    PrecioVenta NUMERIC(10,2) NOT NULL DEFAULT 0.00,
    Stock INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS Proveedores (
    Id SERIAL PRIMARY KEY,
    Empresa VARCHAR(150) NOT NULL,
    Contacto VARCHAR(100),
    Telefono VARCHAR(20),
    Email VARCHAR(150),
    Estado VARCHAR(50)
);

CREATE TABLE IF NOT EXISTS Clientes (
    Id SERIAL PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL,
    Direccion TEXT,
    Telefono VARCHAR(20)
);

CREATE TABLE IF NOT EXISTS Ventas (
    Id SERIAL PRIMARY KEY,
    Fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Cliente VARCHAR(150),
    Total NUMERIC(10,2) NOT NULL DEFAULT 0.00,
    MetodoPago VARCHAR(50)
);

-- Insertar algunos datos de prueba
INSERT INTO Productos (Nombre, Categoria, PrecioCompra, PrecioVenta, Stock) VALUES 
('Cuaderno Profesional', 'Escolar', 15.00, 25.00, 150),
('Lápiz Mirado No. 2', 'Escolar', 2.00, 5.00, 500)
ON CONFLICT DO NOTHING;
