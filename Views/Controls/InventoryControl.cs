using System;
using System.Windows.Forms;
using STOCKPAP.Interfaces;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views.Controls
{
    public partial class InventoryControl : UserControl, IProductoView
    {
        private string message;
        private bool isSuccessful;
        private bool isEdit;

        public InventoryControl()
        {
            InitializeComponent();
            UITheme.ApplyTheme(this);
            AssociateAndRaiseViewEvents();
            tabControl.TabPages.Remove(tabDetail);
        }

        private void AssociateAndRaiseViewEvents()
        {
            // Search
            btnAddNew.Click += delegate { 
                AddNewEvent?.Invoke(this, EventArgs.Empty);
                tabControl.TabPages.Remove(tabList);
                tabControl.TabPages.Add(tabDetail);
                tabDetail.Text = "Agregar Nuevo Producto";
            };
            
            btnEdit.Click += delegate { 
                EditEvent?.Invoke(this, EventArgs.Empty);
                tabControl.TabPages.Remove(tabList);
                tabControl.TabPages.Add(tabDetail);
                tabDetail.Text = "Editar Producto";
            };

            btnDelete.Click += delegate { 
                var result = MessageBox.Show("¿Estás seguro de eliminar este producto?", "Advertencia",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    DeleteEvent?.Invoke(this, EventArgs.Empty);
                    MessageBox.Show(Message);
                }
            };

            btnSave.Click += delegate { 
                SaveEvent?.Invoke(this, EventArgs.Empty);
                if (isSuccessful)
                {
                    tabControl.TabPages.Remove(tabDetail);
                    tabControl.TabPages.Add(tabList);
                }
                MessageBox.Show(Message);
            };

            btnCancel.Click += delegate { 
                CancelEvent?.Invoke(this, EventArgs.Empty);
                tabControl.TabPages.Remove(tabDetail);
                tabControl.TabPages.Add(tabList);
            };

            txtSearch.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) SearchEvent?.Invoke(this, EventArgs.Empty);
            };
        }

        // Properties
        public string ProdId { get => txtId.Text; set => txtId.Text = value; }
        public string Nombre { get => txtNombre.Text; set => txtNombre.Text = value; }
        public string Categoria { get => cmbCategoria.Text; set => cmbCategoria.Text = value; }
        public string PrecioCompra { get => txtPrecioCompra.Text; set => txtPrecioCompra.Text = value; }
        public string PrecioVenta { get => txtPrecioVenta.Text; set => txtPrecioVenta.Text = value; }
        public string Stock { get => txtStock.Text; set => txtStock.Text = value; }
        public string SearchValue { get => txtSearch.Text; set => txtSearch.Text = value; }
        public bool IsEdit { get => isEdit; set => isEdit = value; }
        public bool IsSuccessful { get => isSuccessful; set => isSuccessful = value; }
        public string Message { get => message; set => message = value; }

        // Events
        public event EventHandler SearchEvent;
        public event EventHandler AddNewEvent;
        public event EventHandler EditEvent;
        public event EventHandler DeleteEvent;
        public event EventHandler SaveEvent;
        public event EventHandler CancelEvent;

        // Methods
        public void SetProductoListBindingSource(BindingSource productoList)
        {
            dgvProductos.DataSource = productoList;
        }
    }
}
