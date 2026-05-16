using System;
using System.Windows.Forms;
using STOCKPAP.Interfaces;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views.Controls
{
    public partial class CustomersControl : UserControl, IClienteView
    {
        public CustomersControl()
        {
            InitializeComponent();
            UITheme.ApplyTheme(this);
        }

        // IClienteView properties
        public string ClienteId { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string SearchValue { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        public event EventHandler SearchEvent;
        public event EventHandler AddNewEvent;
        public event EventHandler SaveEvent;

        public void SetClienteListBindingSource(BindingSource clienteList)
        {
            dgvCustomers.DataSource = clienteList;
        }

        // Methods to invoke events can be bound to buttons later
        private void InvokeSearch() => SearchEvent?.Invoke(this, EventArgs.Empty);
        private void InvokeAddNew() => AddNewEvent?.Invoke(this, EventArgs.Empty);
        private void InvokeSave() => SaveEvent?.Invoke(this, EventArgs.Empty);
    }
}
