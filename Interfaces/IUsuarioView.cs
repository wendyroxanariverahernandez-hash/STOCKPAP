using System;
using System.Windows.Forms;

namespace STOCKPAP.Interfaces
{
    public interface IUsuarioView
    {
        string UsuarioId { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string Rol { get; set; }

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

        void SetUsuarioListBindingSource(BindingSource usuarioList);
        void Show();
    }
}
