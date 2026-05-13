using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace STOCKPAP.Interfaces
{
    public interface IProveedorView
    {
        string ProvId { get; set; }
        string Empresa { get; set; }
        string Contacto { get; set; }
        string Telefono { get; set; }
        string Email { get; set; }
        string Estado { get; set; }

        string SearchValue { get; set; }
        bool IsEdit { get; set; }
        bool IsSuccessful { get; set; }
        string Message { get; set; }

        event EventHandler SearchEvent;
        event EventHandler AddNewEvent;
        event EventHandler EditEvent;
        event EventHandler DeleteEvent;
        event EventHandler SaveEvent;
        event EventHandler CancelEvent;

        void SetProveedorListBindingSource(BindingSource proveedorList);
    }
}
