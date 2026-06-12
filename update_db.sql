-- Script de actualización de base de datos para STOCKPAP

-- 1. Agregar nuevos campos a la tabla Productos
ALTER TABLE Productos ADD COLUMN IF NOT EXISTS Clase VARCHAR(120);
ALTER TABLE Productos ADD COLUMN IF NOT EXISTS Subclase VARCHAR(120);
ALTER TABLE Productos ADD COLUMN IF NOT EXISTS Marca VARCHAR(120);
ALTER TABLE Productos ADD COLUMN IF NOT EXISTS Tipo VARCHAR(120);
ALTER TABLE Productos ADD COLUMN IF NOT EXISTS CostoPieza NUMERIC(10,2) NOT NULL DEFAULT 0.00;
ALTER TABLE Productos ADD COLUMN IF NOT EXISTS CostoCaja NUMERIC(10,2) NOT NULL DEFAULT 0.00;

-- 2. Crear tabla de Alertas de Stock Bajo
CREATE TABLE IF NOT EXISTS AlertasStock (
    Id SERIAL PRIMARY KEY,
    ProductoId INTEGER REFERENCES Productos(Id) ON DELETE CASCADE,
    StockActual INTEGER NOT NULL,
    StockMinimo INTEGER NOT NULL,
    FechaGeneracion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Resuelta BOOLEAN NOT NULL DEFAULT FALSE,
    FechaResolucion TIMESTAMP
);

-- 3. Crear tabla de Configuración de la aplicación
CREATE TABLE IF NOT EXISTS Configuracion (
    Clave VARCHAR(100) PRIMARY KEY,
    Valor VARCHAR(255) NOT NULL
);

-- Sembrar configuración por defecto si está vacía
INSERT INTO Configuracion (Clave, Valor) VALUES 
('StockMinimoGlobal', '10'),
('AlertasActivas', 'true'),
('FormatoReporteDefecto', 'PDF')
ON CONFLICT (Clave) DO NOTHING;

-- 4. Agregar nuevos campos a la tabla Usuarios para información personal
ALTER TABLE Usuarios ADD COLUMN IF NOT EXISTS NombreCompleto VARCHAR(150);
ALTER TABLE Usuarios ADD COLUMN IF NOT EXISTS Email VARCHAR(150);
