using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace STOCKPAP.Interfaces
{
    public interface IClienteView
    {
        string ClienteId { get; set; }
        string Nombre { get; set; }
        string Direccion { get; set; }
        string Telefono { get; set; }

        string SearchValue { get; set; }
        bool IsSuccessful { get; set; }
        string Message { get; set; }

        event EventHandler SearchEvent;
        event EventHandler AddNewEvent;
        event EventHandler SaveEvent;

        void SetClienteListBindingSource(BindingSource clienteList);
    }
}
