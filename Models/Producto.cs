namespace STOCKPAP.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string CodigoBarras { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public string ImagePath { get; set; }

        // Nuevos campos
        public string Clase { get; set; }
        public string Subclase { get; set; }
        public string Marca { get; set; }
        public int? ProveedorId { get; set; }
        public string ProveedorNombre { get; set; }
    }
}
