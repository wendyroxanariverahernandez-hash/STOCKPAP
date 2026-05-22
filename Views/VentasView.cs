using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;
using STOCKPAP.Models;
using System.Linq;

namespace STOCKPAP.Views
{
    public class VentasView : UserControl
    {
        private FlowLayoutPanel gridProductos;
        private FlowLayoutPanel gridCarrito;
        private ProductoRepository repoProd;
        private VentaRepository repoVenta;
        private TextBox txtBuscar;
        private Label lblSubtotal;
        private Label lblIva;
        private Label lblTotal;
        private Label lblItemsCount;
        private Venta ventaActual;

        public VentasView()
        {
            repoProd = new ProductoRepository();
            repoVenta = new VentaRepository();
            ventaActual = new Venta();
            InitializeComponent();
            LoadProductos();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            // Left Side: Products
            Panel panelIzquierdo = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 20, 0)
            };
            this.Controls.Add(panelIzquierdo);

            Label lblTitle = new Label
            {
                Text = "Nueva Venta",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(0, 30),
                AutoSize = true
            };
            panelIzquierdo.Controls.Add(lblTitle);

            RoundedPanel searchPanel = new RoundedPanel
            {
                Size = new Size(500, 45),
                Location = new Point(0, 90),
                BackColor = Color.White,
                BorderRadius = 15
            };
            txtBuscar = new TextBox
            {
                Text = "Buscar producto por nombre...",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                Location = new Point(15, 12),
                Width = 470,
                BorderStyle = BorderStyle.None
            };
            txtBuscar.Enter += (s, e) => { if (txtBuscar.Text == "Buscar producto por nombre...") { txtBuscar.Text = ""; txtBuscar.ForeColor = Color.Black; } };
            txtBuscar.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtBuscar.Text)) { txtBuscar.Text = "Buscar producto por nombre..."; txtBuscar.ForeColor = Color.Gray; } };
            txtBuscar.TextChanged += (s, e) => { if(txtBuscar.Text != "Buscar producto por nombre...") LoadProductos(txtBuscar.Text); };
            searchPanel.Controls.Add(txtBuscar);
            panelIzquierdo.Controls.Add(searchPanel);

            gridProductos = new FlowLayoutPanel
            {
                Location = new Point(0, 160),
                Size = new Size(600, 500),
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            panelIzquierdo.Controls.Add(gridProductos);

            // Right Side: Cart
            Panel panelDerecho = new Panel
            {
                Dock = DockStyle.Right,
                Width = 350,
                BackColor = Color.White
            };
            this.Controls.Add(panelDerecho);

            Panel cartHeader = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(30, 96, 255) };
            Label lblCartTitle = new Label { Text = "Carrito", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 15), AutoSize = true };
            lblItemsCount = new Label { Text = "0 artículos", Font = new Font("Segoe UI", 10), ForeColor = Color.White, Location = new Point(23, 45), AutoSize = true };
            
            Button btnLimpiar = new Button { Text = "✕  Limpiar", ForeColor = Color.White, BackColor = Color.Transparent, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10), Size = new Size(100, 30), Location = new Point(230, 20), Cursor = Cursors.Hand };
            btnLimpiar.FlatAppearance.BorderSize = 0;
            btnLimpiar.Click += (s, e) => { ventaActual.Detalles.Clear(); UpdateCartUI(); };

            cartHeader.Controls.Add(lblCartTitle);
            cartHeader.Controls.Add(lblItemsCount);
            cartHeader.Controls.Add(btnLimpiar);
            panelDerecho.Controls.Add(cartHeader);

            gridCarrito = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };
            panelDerecho.Controls.Add(gridCarrito);

            Panel cartFooter = new Panel { Dock = DockStyle.Bottom, Height = 180, Padding = new Padding(20) };
            
            Label lblSubT = new Label { Text = "Subtotal:", Font = new Font("Segoe UI", 10), Location = new Point(20, 20), AutoSize = true };
            lblSubtotal = new Label { Text = "$0.00", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(250, 20), AutoSize = true, TextAlign = ContentAlignment.MiddleRight };
            
            Label lblIvaT = new Label { Text = "IVA (16%):", Font = new Font("Segoe UI", 10), Location = new Point(20, 50), AutoSize = true };
            lblIva = new Label { Text = "$0.00", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(250, 50), AutoSize = true, TextAlign = ContentAlignment.MiddleRight };
            
            Panel line = new Panel { BackColor = Color.LightGray, Height = 1, Width = 310, Location = new Point(20, 80) };

            Label lblTotT = new Label { Text = "Total:", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 95), AutoSize = true };
            lblTotal = new Label { Text = "$0.00", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(30, 96, 255), Location = new Point(230, 95), AutoSize = true, TextAlign = ContentAlignment.MiddleRight };

            RoundedButton btnProcesar = new RoundedButton
            {
                Text = "Procesar Venta",
                Size = new Size(310, 45),
                Location = new Point(20, 135),
                BackColor = Color.FromArgb(100, 200, 130),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BorderRadius = 10
            };
            btnProcesar.Click += BtnProcesar_Click;

            cartFooter.Controls.Add(lblSubT); cartFooter.Controls.Add(lblSubtotal);
            cartFooter.Controls.Add(lblIvaT); cartFooter.Controls.Add(lblIva);
            cartFooter.Controls.Add(line);
            cartFooter.Controls.Add(lblTotT); cartFooter.Controls.Add(lblTotal);
            cartFooter.Controls.Add(btnProcesar);
            
            panelDerecho.Controls.Add(cartFooter);
            gridCarrito.BringToFront(); // Ensure grid is between header and footer
        }

        private void LoadProductos(string filtro = "")
        {
            gridProductos.Controls.Clear();
            var lista = string.IsNullOrEmpty(filtro) ? repoProd.ObtenerTodos() : repoProd.BuscarPorNombre(filtro);

            foreach (var p in lista)
            {
                RoundedPanel card = new RoundedPanel
                {
                    Size = new Size(160, 240),
                    BackColor = Color.White,
                    BorderRadius = 15,
                    Margin = new Padding(10),
                    Cursor = Cursors.Hand
                };
                card.Click += (s, e) => AddToCart(p);

                PictureBox pic = new PictureBox
                {
                    Size = new Size(160, 110),
                    Location = new Point(0, 0),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.LightGray
                };
                pic.Click += (s, e) => AddToCart(p);
                string path = Path.Combine(Application.StartupPath, "Assets", "Images", p.ImagePath);
                if (File.Exists(path)) pic.Image = Image.FromFile(path);
                
                Label lblName = new Label
                {
                    Text = p.Nombre,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Location = new Point(10, 120),
                    AutoSize = false,
                    Size = new Size(140, 35)
                };
                lblName.Click += (s, e) => AddToCart(p);

                Label lblPrice = new Label
                {
                    Text = $"${p.PrecioVenta:0.00}",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 96, 255),
                    Location = new Point(10, 160),
                    AutoSize = true
                };

                RoundedPanel stockBadge = new RoundedPanel
                {
                    Size = new Size(80, 20),
                    Location = new Point(10, 195),
                    BackColor = Color.FromArgb(30, 30, 30),
                    BorderRadius = 10
                };
                Label lblStock = new Label
                {
                    Text = $"{p.Stock} pieza",
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 7, FontStyle.Bold),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                stockBadge.Controls.Add(lblStock);

                card.Controls.Add(pic);
                card.Controls.Add(lblName);
                card.Controls.Add(lblPrice);
                card.Controls.Add(stockBadge);

                gridProductos.Controls.Add(card);
            }
        }

        private void AddToCart(Producto p)
        {
            if (p.Stock <= 0)
            {
                MessageBox.Show("Producto sin stock.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = ventaActual.Detalles.FirstOrDefault(d => d.ProductoId == p.Id);
            if (item != null)
            {
                if (item.Cantidad < p.Stock)
                {
                    item.Cantidad++;
                    item.Subtotal = item.Cantidad * item.PrecioUnitario;
                }
                else MessageBox.Show("No hay suficiente stock.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                ventaActual.Detalles.Add(new DetalleVenta
                {
                    ProductoId = p.Id,
                    ProductoNombre = p.Nombre,
                    Cantidad = 1,
                    PrecioUnitario = p.PrecioVenta,
                    Subtotal = p.PrecioVenta
                });
            }
            UpdateCartUI();
        }

        private void UpdateCartUI()
        {
            gridCarrito.Controls.Clear();
            decimal subtotal = 0;
            int count = 0;

            if (ventaActual.Detalles.Count == 0)
            {
                Label empty = new Label { Text = "\n\n\n🛒\n\nCarrito vacío\nSelecciona productos para comenzar", Font = new Font("Segoe UI", 12), ForeColor = Color.Gray, AutoSize = false, Size = new Size(310, 200), TextAlign = ContentAlignment.MiddleCenter };
                gridCarrito.Controls.Add(empty);
            }
            else
            {
                foreach (var d in ventaActual.Detalles)
                {
                    RoundedPanel pnlItem = new RoundedPanel { Size = new Size(310, 80), Margin = new Padding(0, 5, 0, 5), BackColor = Color.White, BorderRadius = 15, BorderColor = Color.LightGray, BorderSize = 1 };
                    
                    Label lblN = new Label { Text = d.ProductoNombre, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(10, 10), AutoSize = false, Size = new Size(200, 20) };
                    
                    Button btnMinus = new Button { Text = "−", Font = new Font("Segoe UI", 12, FontStyle.Bold), Size = new Size(30, 30), Location = new Point(10, 40), FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray, BackColor = Color.WhiteSmoke, Cursor = Cursors.Hand };
                    btnMinus.FlatAppearance.BorderSize = 0;
                    btnMinus.Click += (s, e) => { if (d.Cantidad > 1) { d.Cantidad--; d.Subtotal = d.Cantidad * d.PrecioUnitario; UpdateCartUI(); } else { ventaActual.Detalles.Remove(d); UpdateCartUI(); } };

                    Label lblC = new Label { Text = d.Cantidad.ToString(), Font = new Font("Segoe UI", 11), Location = new Point(45, 45), AutoSize = false, Size = new Size(40, 20), TextAlign = ContentAlignment.MiddleCenter };

                    Button btnPlus = new Button { Text = "+", Font = new Font("Segoe UI", 12, FontStyle.Bold), Size = new Size(30, 30), Location = new Point(90, 40), FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray, BackColor = Color.WhiteSmoke, Cursor = Cursors.Hand };
                    btnPlus.FlatAppearance.BorderSize = 0;
                    btnPlus.Click += (s, e) => { 
                        var p = repoProd.BuscarPorNombre(d.ProductoNombre).FirstOrDefault();
                        if (p != null && d.Cantidad < p.Stock) { d.Cantidad++; d.Subtotal = d.Cantidad * d.PrecioUnitario; UpdateCartUI(); } 
                        else { MessageBox.Show("Stock insuficiente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                    };

                    Label lblUnit = new Label { Text = $"${d.PrecioUnitario:0.00} c/u", Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, Location = new Point(220, 35), AutoSize = true, TextAlign = ContentAlignment.MiddleRight };
                    Label lblSub = new Label { Text = $"${d.Subtotal:0.00}", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(30, 96, 255), Location = new Point(220, 50), AutoSize = true, TextAlign = ContentAlignment.MiddleRight };
                    
                    Button btnDel = new Button { Text = "🗑", ForeColor = Color.Red, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10), Size = new Size(30, 30), Location = new Point(270, 5), Cursor = Cursors.Hand, BackColor = Color.Transparent };
                    btnDel.FlatAppearance.BorderSize = 0;
                    btnDel.Click += (s, e) => { ventaActual.Detalles.Remove(d); UpdateCartUI(); };

                    pnlItem.Controls.Add(lblN);
                    pnlItem.Controls.Add(btnMinus);
                    pnlItem.Controls.Add(lblC);
                    pnlItem.Controls.Add(btnPlus);
                    pnlItem.Controls.Add(lblUnit);
                    pnlItem.Controls.Add(lblSub);
                    pnlItem.Controls.Add(btnDel);

                    gridCarrito.Controls.Add(pnlItem);

                    subtotal += d.Subtotal;
                    count += d.Cantidad;
                }
            }

            ventaActual.Subtotal = subtotal;
            ventaActual.Iva = subtotal * 0.16m;
            ventaActual.Total = subtotal + ventaActual.Iva;

            lblSubtotal.Text = $"${ventaActual.Subtotal:0.00}";
            lblIva.Text = $"${ventaActual.Iva:0.00}";
            lblTotal.Text = $"${ventaActual.Total:0.00}";
            lblItemsCount.Text = count == 1 ? "1 artículo" : $"{count} artículos";
        }

        private void BtnProcesar_Click(object sender, EventArgs e)
        {
            if (ventaActual.Detalles.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CheckoutForm checkout = new CheckoutForm(ventaActual);
            checkout.ShowDialog();

            if (checkout.VentaConfirmada)
            {
                if (repoVenta.RegistrarVenta(ventaActual))
                {
                    ventaActual = new Venta();
                    UpdateCartUI();
                    LoadProductos(); // Refresh stock
                }
                else
                {
                    MessageBox.Show("Error al registrar la venta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
