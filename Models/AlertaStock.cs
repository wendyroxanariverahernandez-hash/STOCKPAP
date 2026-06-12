using System;

namespace STOCKPAP.Models
{
    public class AlertaStock
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public bool Resuelta { get; set; }
        public DateTime? FechaResolucion { get; set; }
    }
}
