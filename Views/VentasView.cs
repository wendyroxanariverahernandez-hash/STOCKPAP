using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using STOCKPAP.Models;
using STOCKPAP.Utilities;

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
        private Usuario currentUser;
        private bool puedeVender;

        private Dictionary<string, DateTime> scanCooldowns = new Dictionary<string, DateTime>();
        private const int COOLDOWN_SECONDS = 3;

        // Success banner
        private Panel pnlBannerExito;
        private Label lblBannerExito;
        private Timer timerBanner;

        public VentasView(Usuario user)
        {
            currentUser = user;
            puedeVender = EsAdmin() || EsVentas();
            repoProd = new ProductoRepository();
            repoVenta = new VentaRepository();
            ventaActual = new Venta();
            InitializeComponent();
            LoadProductos();
            UpdateCartUI();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            Panel panelIzquierdo = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 20, 0) };
            this.Controls.Add(panelIzquierdo);

            Panel pnlTopIzq = new Panel { Dock = DockStyle.Top, Height = 110 };
            panelIzquierdo.Controls.Add(pnlTopIzq);

            Label lblTitle = new Label
            {
                Text = puedeVender ? "Nueva Venta" : "Consulta de Productos",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(0, 10),
                AutoSize = true
            };
            pnlTopIzq.Controls.Add(lblTitle);

            pnlTopIzq.Controls.Add(lblTitle);

            // ── Search bar ──────────────────────────────────────────
            RoundedPanel searchPanel = new RoundedPanel
            {
                Height = 42,
                Location = new Point(0, 55),
                BackColor = Color.White,
                BorderRadius = 15,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Width = panelIzquierdo.Width - 40
            };
            
            Label lblLupa = new Label { Text = "🔍", Font = new Font("Segoe UI", 12), ForeColor = Color.Gray, Location = new Point(10, 10), AutoSize = true };
            searchPanel.Controls.Add(lblLupa);

            txtBuscar = new TextBox
            {
                Text = "Escanea o busca producto",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                Location = new Point(40, 10),
                Width = 445,
                BorderStyle = BorderStyle.None
            };
            txtBuscar.Enter += (s, e) => { if (txtBuscar.Text == "Escanea o busca producto") { txtBuscar.Text = ""; txtBuscar.ForeColor = Color.Black; } };
            txtBuscar.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtBuscar.Text)) { txtBuscar.Text = "Escanea o busca producto"; txtBuscar.ForeColor = Color.Gray; } };
            txtBuscar.TextChanged += (s, e) => { 
                if (txtBuscar.Text != "Escanea o busca producto" && !string.IsNullOrWhiteSpace(txtBuscar.Text)) 
                    LoadProductos(txtBuscar.Text); 
                else if (txtBuscar.Text == "Escanea o busca producto" || string.IsNullOrWhiteSpace(txtBuscar.Text)) 
                    LoadProductos(); 
            };
            txtBuscar.KeyDown += (s, e) => 
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    string query = txtBuscar.Text.Trim();
                    if (!string.IsNullOrEmpty(query) && query != "Escanea o busca producto")
                    {
                        var prod = repoProd.ObtenerTodos().FirstOrDefault(p => p.CodigoBarras == query);
                        if (prod != null)
                        {
                            AddToCart(prod);
                            txtBuscar.Text = "";
                            LoadProductos();
                        }
                        else
                        {
                            MessageBox.Show("No existe ningún producto registrado con ese código de barras.", "Producto no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            };
            searchPanel.Controls.Add(txtBuscar);
            pnlTopIzq.Controls.Add(searchPanel);

            // ── Success Banner (animated) ────────────────────────────
            pnlBannerExito = new Panel
            {
                Dock = DockStyle.Top,
                Height = 0,
                BackColor = Color.FromArgb(16, 185, 90)
            };
            lblBannerExito = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlBannerExito.Controls.Add(lblBannerExito);
            panelIzquierdo.Controls.Add(pnlBannerExito);

            // ── Products Grid ───────────────────────────────────────
            Panel pnlFillIzq = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 10) };
            panelIzquierdo.Controls.Add(pnlFillIzq);
            pnlFillIzq.BringToFront();

            gridProductos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            pnlFillIzq.Controls.Add(gridProductos);

            // ── Right Panel (Cart) ──────────────────────────────────
            Panel panelDerecho = new Panel { Dock = DockStyle.Right, Width = 350, BackColor = Color.White };
            this.Controls.Add(panelDerecho);

            Panel cartHeader = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(30, 96, 255) };
            Label lblCartTitle = new Label { Text = "Carrito", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, Location = new Point(20, 15), AutoSize = true };
            lblItemsCount = new Label { Text = "0 artículos", Font = new Font("Segoe UI", 10), ForeColor = Color.White, Location = new Point(23, 45), AutoSize = true };
            Button btnCancelar = new Button { Text = "Cancelar", ForeColor = Color.White, BackColor = Color.Transparent, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10), Size = new Size(100, 30), Location = new Point(230, 20), Cursor = Cursors.Hand };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => CancelarVentaActual();
            cartHeader.Controls.Add(lblCartTitle);
            cartHeader.Controls.Add(lblItemsCount);
            cartHeader.Controls.Add(btnCancelar);
            panelDerecho.Controls.Add(cartHeader);

            gridCarrito = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
            panelDerecho.Controls.Add(gridCarrito);

            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            Panel cartFooter = new Panel { Dock = DockStyle.Bottom, Height = 180, Padding = new Padding(20) };
            Label lblSubT = new Label { Text = "Subtotal:", Font = new Font("Segoe UI", 10), Location = new Point(20, 20), AutoSize = true };
            lblSubtotal = new Label { Text = $"{sym}0.00", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(250, 20), AutoSize = true, TextAlign = ContentAlignment.MiddleRight };
            Label lblIvaT = new Label { Text = "IVA (16%):", Font = new Font("Segoe UI", 10), Location = new Point(20, 50), AutoSize = true };
            lblIva = new Label { Text = $"{sym}0.00", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(250, 50), AutoSize = true, TextAlign = ContentAlignment.MiddleRight };
            Panel line = new Panel { BackColor = Color.LightGray, Height = 1, Width = 310, Location = new Point(20, 80) };
            Label lblTotT = new Label { Text = "Total:", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 95), AutoSize = true };
            lblTotal = new Label { Text = $"{sym}0.00", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(30, 96, 255), Location = new Point(230, 95), AutoSize = true, TextAlign = ContentAlignment.MiddleRight };

            RoundedButton btnProcesar = new RoundedButton
            {
                Text = "Procesar Venta",
                Size = new Size(310, 45),
                Location = new Point(20, 135),
                BackColor = puedeVender ? Color.FromArgb(100, 200, 130) : Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BorderRadius = 10,
                Enabled = puedeVender
            };
            btnProcesar.Click += BtnProcesar_Click;

            cartFooter.Controls.Add(lblSubT); cartFooter.Controls.Add(lblSubtotal);
            cartFooter.Controls.Add(lblIvaT); cartFooter.Controls.Add(lblIva);
            cartFooter.Controls.Add(line);
            cartFooter.Controls.Add(lblTotT); cartFooter.Controls.Add(lblTotal);
            cartFooter.Controls.Add(btnProcesar);
            panelDerecho.Controls.Add(cartFooter);
            gridCarrito.BringToFront();

            // ── Success banner timer ────────────────────────────────
            timerBanner = new Timer { Interval = 40 };
            int bannerStep = 0;
            timerBanner.Tick += (s, e) =>
            {
                bannerStep++;
                if (bannerStep <= 8) // expand
                {
                    pnlBannerExito.Height = Math.Min(38, bannerStep * 5);
                }
                else if (bannerStep > 60) // collapse after 2 seconds
                {
                    pnlBannerExito.Height = Math.Max(0, pnlBannerExito.Height - 5);
                    if (pnlBannerExito.Height <= 0)
                    {
                        timerBanner.Stop();
                        bannerStep = 0;
                    }
                }
            };
            this.Load += (s, e) => txtBuscar.Focus();
        }



        // ══════════════════════════════════════════════════════════════
        // Keyboard Shortcuts (Hotkeys)
        // ══════════════════════════════════════════════════════════════
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                txtBuscar.Focus();
                return true;
            }
            else if (keyData == Keys.F12)
            {
                if (puedeVender && ventaActual.Detalles.Count > 0)
                {
                    BtnProcesar_Click(null, EventArgs.Empty);
                    return true;
                }
            }
            else if (keyData == Keys.Escape)
            {
                if (ventaActual.Detalles.Count > 0)
                {
                    CancelarVentaActual();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ══════════════════════════════════════════════════════════════
        // Product loading and cart logic
        // ══════════════════════════════════════════════════════════════

        private void LoadProductos(string filtro = "")
        {
            gridProductos.Controls.Clear();
            var lista = string.IsNullOrEmpty(filtro) ? repoProd.ObtenerTodos() : repoProd.BuscarPorNombre(filtro);

            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            foreach (var p in lista)
            {
                RoundedPanel card = new RoundedPanel
                {
                    Size = new Size(160, 240),
                    BackColor = Color.White,
                    BorderRadius = 15,
                    Margin = new Padding(10),
                    Cursor = puedeVender ? Cursors.Hand : Cursors.Default
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
                string path = Path.Combine(Application.StartupPath, "Assets", "Images", p.ImagePath ?? "");
                if (File.Exists(path)) pic.Image = Image.FromFile(path);

                Label lblName = new Label { Text = p.Nombre, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(10, 120), AutoSize = false, Size = new Size(140, 35) };
                lblName.Click += (s, e) => AddToCart(p);
                Label lblPrice = new Label { Text = $"{sym}{p.PrecioVenta:0.00}", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(30, 96, 255), Location = new Point(10, 160), AutoSize = true };
                RoundedPanel stockBadge = new RoundedPanel { Size = new Size(90, 20), Location = new Point(10, 195), BackColor = Color.FromArgb(30, 30, 30), BorderRadius = 10 };
                stockBadge.Controls.Add(new Label { Text = $"{p.Stock} piezas", ForeColor = Color.White, Font = new Font("Segoe UI", 7, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });

                card.Controls.Add(pic);
                card.Controls.Add(lblName);
                card.Controls.Add(lblPrice);
                card.Controls.Add(stockBadge);
                gridProductos.Controls.Add(card);
            }
        }

        private void AddToCart(Producto p)
        {
            if (!puedeVender)
            {
                MessageBox.Show("Tu rol es de consulta. No puedes agregar productos al carrito.", "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                else
                {
                    MessageBox.Show("No hay suficiente stock.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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

            // Animated notification banner
            int cartQty = item != null ? item.Cantidad : 1;
            int remainingStock = p.Stock - cartQty;

            if (remainingStock <= p.StockMinimo)
            {
                pnlBannerExito.BackColor = Color.FromArgb(230, 126, 34); // Warning Orange
                lblBannerExito.Text = $"⚠️ Stock Bajo: {p.Nombre} (Quedan {remainingStock})";
            }
            else
            {
                pnlBannerExito.BackColor = Color.FromArgb(46, 204, 113); // Success Green
                lblBannerExito.Text = $"✅ ¡{p.Nombre} agregado correctamente!";
            }

            pnlBannerExito.Height = 0;
            timerBanner.Start();

            UpdateCartUI();
        }

        private void UpdateCartUI()
        {
            gridCarrito.Controls.Clear();
            decimal subtotal = 0;
            int count = 0;

            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            if (ventaActual.Detalles.Count == 0)
            {
                Label empty = new Label { Text = "\n\n\nCarrito vacío\nSelecciona productos para comenzar", Font = new Font("Segoe UI", 12), ForeColor = Color.Gray, AutoSize = false, Size = new Size(310, 200), TextAlign = ContentAlignment.MiddleCenter };
                gridCarrito.Controls.Add(empty);
            }
            else
            {
                foreach (var d in ventaActual.Detalles.ToList())
                {
                    RoundedPanel pnlItem = new RoundedPanel { Size = new Size(310, 80), Margin = new Padding(0, 5, 0, 5), BackColor = Color.White, BorderRadius = 15, BorderColor = Color.LightGray, BorderSize = 1 };
                    Label lblN = new Label { Text = d.ProductoNombre, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(10, 10), AutoSize = false, Size = new Size(200, 20) };
                    Button btnMinus = new Button { Text = "-", Font = new Font("Segoe UI", 12, FontStyle.Bold), Size = new Size(30, 30), Location = new Point(10, 40), FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray, BackColor = Color.WhiteSmoke, Cursor = Cursors.Hand };
                    btnMinus.FlatAppearance.BorderSize = 0;
                    btnMinus.Click += (s, e) => { if (d.Cantidad > 1) { d.Cantidad--; d.Subtotal = d.Cantidad * d.PrecioUnitario; } else { ventaActual.Detalles.Remove(d); } UpdateCartUI(); };
                    Label lblC = new Label { Text = d.Cantidad.ToString(), Font = new Font("Segoe UI", 11), Location = new Point(45, 45), AutoSize = false, Size = new Size(40, 20), TextAlign = ContentAlignment.MiddleCenter };
                    Button btnPlus = new Button { Text = "+", Font = new Font("Segoe UI", 12, FontStyle.Bold), Size = new Size(30, 30), Location = new Point(90, 40), FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray, BackColor = Color.WhiteSmoke, Cursor = Cursors.Hand };
                    btnPlus.FlatAppearance.BorderSize = 0;
                    btnPlus.Click += (s, e) =>
                    {
                        var producto = repoProd.ObtenerTodos().FirstOrDefault(p => p.Id == d.ProductoId);
                        if (producto != null && d.Cantidad >= producto.Stock)
                        {
                            MessageBox.Show("Stock insuficiente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        d.Cantidad++;
                        d.Subtotal = d.Cantidad * d.PrecioUnitario;
                        UpdateCartUI();
                    };
                    Label lblUnit = new Label { Text = $"{sym}{d.PrecioUnitario:0.00} c/u", Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, Location = new Point(220, 35), AutoSize = true };
                    Label lblSub = new Label { Text = $"{sym}{d.Subtotal:0.00}", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(30, 96, 255), Location = new Point(220, 50), AutoSize = true };
                    Button btnDel = new Button { Text = "X", ForeColor = Color.Red, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Size = new Size(30, 30), Location = new Point(270, 5), Cursor = Cursors.Hand, BackColor = Color.Transparent };
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
            lblSubtotal.Text = $"{sym}{ventaActual.Subtotal:0.00}";
            lblIva.Text = $"{sym}{ventaActual.Iva:0.00}";
            lblTotal.Text = $"{sym}{ventaActual.Total:0.00}";
            lblItemsCount.Text = count == 1 ? "1 artículo" : $"{count} artículos";
        }

        private void BtnProcesar_Click(object sender, EventArgs e)
        {
            if (!puedeVender)
            {
                MessageBox.Show("Tu rol es de consulta. No puedes procesar ventas.", "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ventaActual.Detalles.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CheckoutForm checkout = new CheckoutForm(ventaActual, AutorizarCancelacion);
            checkout.ShowDialog(this);

            if (checkout.VentaCancelada)
            {
                LimpiarCarrito();
                MessageBox.Show("Venta cancelada. El carrito se limpió automáticamente.", "Cancelación realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (checkout.VentaConfirmada)
            {
                if (repoVenta.RegistrarVenta(ventaActual))
                {
                    AppEvents.OnVentaRealizada();
                    var res = MessageBox.Show("¿Deseas generar el ticket de esta venta?", "Ticket de Venta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        GenerarTicketVenta();
                    }
                    LimpiarCarrito();
                    LoadProductos();
                }
                else
                {
                    MessageBox.Show("Error al registrar la venta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GenerarTicketVenta()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("           TICKET DE VENTA");
            sb.AppendLine("========================================");
            sb.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Usuario: {currentUser?.Username}");
            sb.AppendLine("----------------------------------------");
            sb.AppendLine("CANT | PRODUCTO             | SUBTOTAL");
            sb.AppendLine("----------------------------------------");
            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            foreach (var d in ventaActual.Detalles)
            {
                string pName = d.ProductoNombre.Length > 20 ? d.ProductoNombre.Substring(0, 20) : d.ProductoNombre.PadRight(20);
                sb.AppendLine($"{d.Cantidad.ToString().PadRight(4)} | {pName} | {sym}{d.Subtotal:0.00}");
            }
            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"Subtotal: {sym}{ventaActual.Subtotal:0.00}");
            sb.AppendLine($"IVA (16%): {sym}{ventaActual.Iva:0.00}");
            sb.AppendLine($"TOTAL:    {sym}{ventaActual.Total:0.00}");
            sb.AppendLine("========================================");
            sb.AppendLine("    ¡GRACIAS POR SU COMPRA!");
            sb.AppendLine("========================================");
            
            STOCKPAP.Utilities.Exportar.GuardarArchivo("Ticket_Venta", "Archivo de texto|*.txt", ".txt", sb.ToString());
        }

        private void CancelarVentaActual()
        {
            if (ventaActual.Detalles.Count == 0)
                return;

            if (!AutorizarCancelacion())
                return;

            LimpiarCarrito();
            MessageBox.Show("Venta cancelada. El carrito se limpió automáticamente.", "Cancelación realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LimpiarCarrito()
        {
            ventaActual = new Venta();
            UpdateCartUI();
        }

        private bool AutorizarCancelacion()
        {
            if (EsAdmin())
                return true;

            using (var form = new AdminAuthorizationForm())
            {
                form.ShowDialog(this);
                return form.Autorizado;
            }
        }

        private bool EsAdmin()
        {
            return currentUser != null && string.Equals(currentUser.Rol, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        private bool EsVentas()
        {
            return currentUser != null && string.Equals(currentUser.Rol, "Ventas", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Ensure timers are stopped when the control is disposed.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timerBanner?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
