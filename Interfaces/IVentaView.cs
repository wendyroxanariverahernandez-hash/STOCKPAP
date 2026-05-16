using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace STOCKPAP.Interfaces
{
    public interface IVentaView
    {
        string VentaId { get; set; }
        string Producto { get; set; }
        string Cantidad { get; set; }
        string Total { get; set; }
        string MetodoPago { get; set; }
        
        string CantidadRecibida { get; set; }
        string Cambio { get; set; }
        string TipoTarjeta { get; set; }
        string Banco { get; set; }
        string Ultimos4 { get; set; }
        string Referencia { get; set; }
        bool Confirmacion { get; set; }

        string SearchValue { get; set; }
        bool IsSuccessful { get; set; }
        string Message { get; set; }

        event EventHandler SearchEvent;
        event EventHandler AddNewEvent;
        event EventHandler SaveEvent;
        event EventHandler CancelEvent;

        void SetVentaListBindingSource(BindingSource ventaList);
    }
}
