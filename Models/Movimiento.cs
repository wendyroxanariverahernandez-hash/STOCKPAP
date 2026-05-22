using System;

namespace STOCKPAP.Models
{
    public class Movimiento
    {
        public int Id { get; set; }
        public string Tipo { get; set; } // Entrada, Salida, Ajuste
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } // For display
        public int Cantidad { get; set; }
        public int StockAnterior { get; set; }
        public int StockNuevo { get; set; }
        public string Motivo { get; set; }
        public DateTime Fecha { get; set; }
    }
}
