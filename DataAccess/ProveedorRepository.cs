using System;
using System.Collections.Generic;
using Npgsql;
using STOCKPAP.Models;

namespace STOCKPAP.DataAccess
{
    public class ProveedorRepository
    {
        public List<Proveedor> ObtenerTodos()
        {
            var lista = new List<Proveedor>();
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT Id, Empresa, Contacto, Telefono, Email, Direccion FROM Proveedores ORDER BY Empresa", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lista.Add(MapearProveedor(reader));
                }
            }
            return lista;
        }

        private Proveedor MapearProveedor(NpgsqlDataReader reader)
        {
            return new Proveedor
            {
                Id       = reader.GetInt32(0),
                Empresa  = reader.GetString(1),
                Contacto = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Telefono = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Email    = reader.IsDBNull(4) ? "" : reader.GetString(4),
                Direccion= reader.IsDBNull(5) ? "" : reader.GetString(5)
            };
        }

        public bool AgregarProveedor(Proveedor p)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO Proveedores (Empresa, Contacto, Telefono, Email, Direccion) VALUES (@e, @c, @t, @em, @d)", conn))
                {
                    cmd.Parameters.AddWithValue("e",  p.Empresa);
                    cmd.Parameters.AddWithValue("c",  p.Contacto  ?? "");
                    cmd.Parameters.AddWithValue("t",  p.Telefono  ?? "");
                    cmd.Parameters.AddWithValue("em", p.Email     ?? "");
                    cmd.Parameters.AddWithValue("d",  p.Direccion ?? "");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ActualizarProveedor(Proveedor p)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "UPDATE Proveedores SET Empresa=@e, Contacto=@c, Telefono=@t, Email=@em, Direccion=@d WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("e",  p.Empresa);
                    cmd.Parameters.AddWithValue("c",  p.Contacto  ?? "");
                    cmd.Parameters.AddWithValue("t",  p.Telefono  ?? "");
                    cmd.Parameters.AddWithValue("em", p.Email     ?? "");
                    cmd.Parameters.AddWithValue("d",  p.Direccion ?? "");
                    cmd.Parameters.AddWithValue("id", p.Id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool EliminarProveedor(int id)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("DELETE FROM Proveedores WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
