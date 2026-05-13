using System;

namespace STOCKPAP.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public string Estado => Stock < 20 ? "Bajo Stock" : "Óptimo";
    }
}
