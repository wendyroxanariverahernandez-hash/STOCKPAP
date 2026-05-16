using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using STOCKPAP.Interfaces;
using STOCKPAP.Utilities;
using STOCKPAP.DataAccess;
using STOCKPAP.Models;

namespace STOCKPAP.Views.Controls
{
    public partial class SalesControl : UserControl, IVentaView
    {
        private string message;
        private bool isSuccessful;
        private ProductoDAO productoDAO;
        private List<Producto> allProducts;
        private Producto selectedProduct;

        public SalesControl()
        {
            InitializeComponent();
            UITheme.ApplyTheme(this);
            productoDAO = new ProductoDAO();
            AssociateAndRaiseViewEvents();
            SetupDynamicEvents();
        }

        private void AssociateAndRaiseViewEvents()
        {
            btnNewSale.Click += delegate { 
                pnlNewSale.Visible = true;
                LoadCategoriesAndProducts();
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

        private void SetupDynamicEvents()
        {
            cmbCategoria.SelectedIndexChanged += (s, e) => {
                FilterProductsByCategory();
            };

            cmbProducto.SelectedIndexChanged += (s, e) => {
                UpdateTotal();
            };

            txtCantidad.TextChanged += (s, e) => {
                UpdateTotal();
            };

            cmbMetodo.SelectedIndexChanged += (s, e) => {
                pnlEfectivo.Visible = false;
                pnlTarjeta.Visible = false;
                pnlTransferencia.Visible = false;

                if (cmbMetodo.SelectedItem == null) return;
                
                string method = cmbMetodo.SelectedItem.ToString();
                if (method == "Efectivo") pnlEfectivo.Visible = true;
                else if (method == "Tarjeta") pnlTarjeta.Visible = true;
                else if (method == "Transferencia") pnlTransferencia.Visible = true;
            };

            txtCantidadRecibida.TextChanged += (s, e) => {
                CalculateChange();
            };
        }

        private void LoadCategoriesAndProducts()
        {
            allProducts = productoDAO.GetAll();
            if (cmbCategoria.Items.Count > 0)
                cmbCategoria.SelectedIndex = 0;
            FilterProductsByCategory();
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public Producto Value { get; set; }
        }

        private void FilterProductsByCategory()
        {
            if (cmbCategoria.SelectedItem == null || allProducts == null) return;
            string cat = cmbCategoria.SelectedItem.ToString();
            
            var filtered = allProducts.Where(p => p.Categoria != null && p.Categoria.ToLower() == cat.ToLower()).ToList();
            
            cmbProducto.Items.Clear();
            foreach(var p in filtered)
            {
                cmbProducto.Items.Add(new ComboBoxItem { Text = p.Nombre, Value = p });
            }
            cmbProducto.DisplayMember = "Text";
            cmbProducto.ValueMember = "Value";

            if (cmbProducto.Items.Count > 0)
                cmbProducto.SelectedIndex = 0;
            else {
                lblTotalVal.Text = "$0.00";
                selectedProduct = null;
            }
        }

        private void UpdateTotal()
        {
            if (cmbProducto.SelectedItem == null) {
                selectedProduct = null;
                lblTotalVal.Text = "$0.00";
                return;
            }

            var item = (ComboBoxItem)cmbProducto.SelectedItem;
            selectedProduct = item.Value;

            if (int.TryParse(txtCantidad.Text, out int qty))
            {
                decimal total = qty * selectedProduct.PrecioVenta;
                lblTotalVal.Text = total.ToString();
            }
            else
            {
                lblTotalVal.Text = "$0.00";
            }
            CalculateChange();
        }

        private void CalculateChange()
        {
            if (decimal.TryParse(txtCantidadRecibida.Text, out decimal recibida) && decimal.TryParse(lblTotalVal.Text, out decimal total))
            {
                decimal cambio = recibida - total;
                txtCambio.Text = cambio > 0 ? cambio.ToString() : "0.00";
            }
            else
            {
                txtCambio.Text = "0.00";
            }
        }

        // Properties
        public string VentaId { get; set; }
        public string Producto { get => cmbProducto.Text; set => cmbProducto.Text = value; }
        public string Cantidad { get => txtCantidad.Text; set => txtCantidad.Text = value; }
        public string Total { get => lblTotalVal.Text; set => lblTotalVal.Text = value; }
        public string MetodoPago { get => cmbMetodo.Text; set => cmbMetodo.Text = value; }
        
        public string CantidadRecibida { get => txtCantidadRecibida.Text; set => txtCantidadRecibida.Text = value; }
        public string Cambio { get => txtCambio.Text; set => txtCambio.Text = value; }
        public string TipoTarjeta { get => cmbTipoTarjeta.Text; set => cmbTipoTarjeta.Text = value; }
        public string Banco { get => txtBanco.Text; set => txtBanco.Text = value; }
        public string Ultimos4 { get => txtUltimos4.Text; set => txtUltimos4.Text = value; }
        public string Referencia { get => cmbMetodo.Text == "Tarjeta" ? txtReferencia.Text : txtReferenciaSPEI.Text; set { if(cmbMetodo.Text == "Tarjeta") txtReferencia.Text = value; else txtReferenciaSPEI.Text = value; } }
        public bool Confirmacion { get => chkConfirmacion.Checked; set => chkConfirmacion.Checked = value; }

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
