using System;
using Npgsql;
using STOCKPAP.DataAccess;

namespace STOCKPAP.DataAccess
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            try
            {
                using (var conn = Conexion.Instance.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(@"
                        CREATE TABLE IF NOT EXISTS Clases (
                            Id SERIAL PRIMARY KEY,
                            Nombre VARCHAR(100) NOT NULL UNIQUE
                        );

                        CREATE TABLE IF NOT EXISTS Subclases (
                            Id SERIAL PRIMARY KEY,
                            Nombre VARCHAR(100) NOT NULL,
                            ClaseId INTEGER REFERENCES Clases(Id) ON DELETE CASCADE,
                            UNIQUE(Nombre, ClaseId)
                        );

                        CREATE TABLE IF NOT EXISTS Marcas (
                            Id SERIAL PRIMARY KEY,
                            Nombre VARCHAR(100) NOT NULL,
                            ClaseId INTEGER REFERENCES Clases(Id) ON DELETE CASCADE,
                            UNIQUE(Nombre, ClaseId)
                        );
                    ", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
            }
        }
    }
}
