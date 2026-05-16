using System;
using System.Windows.Forms;
using STOCKPAP.Interfaces;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views.Controls
{
    public partial class SalesControl : UserControl, IVentaView
    {
        private string message;
        private bool isSuccessful;

        public SalesControl()
        {
            InitializeComponent();
            UITheme.ApplyTheme(this);
            AssociateAndRaiseViewEvents();
        }

        private void AssociateAndRaiseViewEvents()
        {
            btnNewSale.Click += delegate { 
                pnlNewSale.Visible = true;
                AddNewEvent?.Invoke(this, EventArgs.Empty);
            };

            btnConfirmSale.Click += delegate { 
                SaveEvent?.Invoke(this, EventArgs.Empty);
                if (isSuccessful) pnlNewSale.Visible = false;
                MessageBox.Show(Message);
            };

            btnCancelSale.Click += delegate { 
                pnlNewSale.Visible = false;
                CancelEvent?.Invoke(this, EventArgs.Empty);
            };
        }

        // Properties
        public string VentaId { get; set; }
        public string Cliente { get => txtCliente.Text; set => txtCliente.Text = value; }
        public string Producto { get => cmbProducto.Text; set => cmbProducto.Text = value; }
        public string Cantidad { get => txtCantidad.Text; set => txtCantidad.Text = value; }
        public string Total { get => lblTotalVal.Text; set => lblTotalVal.Text = value; }
        public string MetodoPago { get => cmbMetodo.Text; set => cmbMetodo.Text = value; }
        public string SearchValue { get; set; }
        public bool IsSuccessful { get => isSuccessful; set => isSuccessful = value; }
        public string Message { get => message; set => message = value; }

        // Events
        public event EventHandler SearchEvent;
        public event EventHandler AddNewEvent;
        public event EventHandler SaveEvent;
        public event EventHandler CancelEvent;

        // Methods
        public void SetVentaListBindingSource(BindingSource ventaList)
        {
            dgvSales.DataSource = ventaList;
        }
    }
}
