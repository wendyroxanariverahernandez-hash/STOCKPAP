using System;
using System.Windows.Forms;
using STOCKPAP.Interfaces;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views.Controls
{
    public partial class SuppliersControl : UserControl, IProveedorView
    {
        public SuppliersControl()
        {
            InitializeComponent();
            UITheme.ApplyTheme(this);

            btnNew.Click += delegate { AddNewEvent?.Invoke(this, EventArgs.Empty); };
        }

        // IProveedorView properties
        public string ProvId { get; set; }
        public string Empresa { get; set; }
        public string Contacto { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; }
        public string SearchValue { get; set; }
        public bool IsEdit { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        public event EventHandler SearchEvent;
        public event EventHandler AddNewEvent;
        public event EventHandler EditEvent;
        public event EventHandler DeleteEvent;
        public event EventHandler SaveEvent;
        public event EventHandler CancelEvent;

        public void SetProveedorListBindingSource(BindingSource proveedorList)
        {
            dgvSuppliers.DataSource = proveedorList;
        }

        // Helper methods for events
        private void InvokeSearch() => SearchEvent?.Invoke(this, EventArgs.Empty);
        private void InvokeEdit() => EditEvent?.Invoke(this, EventArgs.Empty);
        private void InvokeDelete() => DeleteEvent?.Invoke(this, EventArgs.Empty);
        private void InvokeSave() => SaveEvent?.Invoke(this, EventArgs.Empty);
        private void InvokeCancel() => CancelEvent?.Invoke(this, EventArgs.Empty);
    }
}
