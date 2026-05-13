using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace STOCKPAP.Interfaces
{
    public interface IVentaView
    {
        string VentaId { get; set; }
        string Cliente { get; set; }
        string Producto { get; set; }
        string Cantidad { get; set; }
        string Total { get; set; }
        string MetodoPago { get; set; }

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
