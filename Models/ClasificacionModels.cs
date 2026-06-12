using System;

namespace STOCKPAP.Models
{
    public class Clase
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        
        public override string ToString()
        {
            return Nombre;
        }
    }

    public class Subclase
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int ClaseId { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }

    public class Marca
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int ClaseId { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
