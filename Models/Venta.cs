using System;
using System.Collections.Generic;

namespace STOCKPAP.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public decimal? CantidadRecibida { get; set; }
        public decimal? Cambio { get; set; }
        public string TipoTarjeta { get; set; }
        public string Banco { get; set; }
        public string Ultimos4 { get; set; }
        public string Referencia { get; set; }
        public bool? Confirmacion { get; set; }
    }
}
