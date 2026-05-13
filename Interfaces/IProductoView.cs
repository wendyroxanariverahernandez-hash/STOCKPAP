using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace STOCKPAP.Interfaces
{
    public interface IProductoView
    {
        // Properties - Fields
        string ProdId { get; set; }
        string Nombre { get; set; }
        string Categoria { get; set; }
        string PrecioCompra { get; set; }
        string PrecioVenta { get; set; }
        string Stock { get; set; }

        // Properties - Logic
        string SearchValue { get; set; }
        bool IsEdit { get; set; }
        bool IsSuccessful { get; set; }
        string Message { get; set; }

        // Events
        event EventHandler SearchEvent;
        event EventHandler AddNewEvent;
        event EventHandler EditEvent;
        event EventHandler DeleteEvent;
        event EventHandler SaveEvent;
        event EventHandler CancelEvent;

        // Methods
        void SetProductoListBindingSource(BindingSource productoList);
        void Show();
    }
}
