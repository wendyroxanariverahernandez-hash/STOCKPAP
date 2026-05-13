using System;
using System.Collections.Generic;

namespace STOCKPAP.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
    }
}
