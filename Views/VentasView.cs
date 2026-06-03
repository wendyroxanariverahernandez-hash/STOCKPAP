using System;
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
        private TextBox txtCodigoBarras;
        private Label lblSubtotal;
        private Label lblIva;
        private Label lblTotal;
        private Label lblItemsCount;
        private Label lblScanStatus;
        private Panel pnlScanIndicator;
        private Venta ventaActual;
        private Usuario currentUser;
        private bool puedeVender;

        // Flash feedback timer for inline scan
        private Timer timerScanFeedback;
        private int scanFeedbackCount;

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

            Panel pnlTopIzq = new Panel { Dock = DockStyle.Top, Height = 220 };
            panelIzquierdo.Controls.Add(pnlTopIzq);

            Label lblTitle = new Label
            {
                Text = puedeVender ? "Nueva Venta" : "Consulta de Productos",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(0, 10),
                AutoSize = true
            };
            pnlTopIzq.Controls.Add(lblTitle);

            // Botón Cerrar Sesión
            Button btnLogoutVentas = new Button
            {
                Text = "Cerrar Sesión",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 30),
                Location = new Point(pnlTopIzq.Width - 140, 22),
                BackColor = Color.FromArgb(220, 50, 50),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnLogoutVentas.FlatAppearance.BorderSize = 0;
            btnLogoutVentas.Click += (s, e) => {
                if (MessageBox.Show("¿Seguro que deseas cerrar sesión?", "Cerrar Sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    System.Windows.Forms.Application.Restart();
            };
            btnLogoutVentas.Visible = false;
            pnlTopIzq.Controls.Add(btnLogoutVentas);

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
            txtBuscar = new TextBox
            {
                Text = "Buscar producto por nombre...",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                Location = new Point(15, 10),
                Width = 470,
                BorderStyle = BorderStyle.None
            };
            txtBuscar.Enter += (s, e) => { if (txtBuscar.Text == "Buscar producto por nombre...") { txtBuscar.Text = ""; txtBuscar.ForeColor = Color.Black; } };
            txtBuscar.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtBuscar.Text)) { txtBuscar.Text = "Buscar producto por nombre..."; txtBuscar.ForeColor = Color.Gray; } };
            txtBuscar.TextChanged += (s, e) => { if (txtBuscar.Text != "Buscar producto por nombre...") LoadProductos(txtBuscar.Text); };
            searchPanel.Controls.Add(txtBuscar);
            pnlTopIzq.Controls.Add(searchPanel);

            // ── Barcode Scanner Section (POS-style) ─────────────────
            RoundedPanel barcodePanel = new RoundedPanel
            {
                Height = 95,
                Location = new Point(0, 108),
                BackColor = Color.FromArgb(20, 25, 35),
                BorderRadius = 12,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Width = panelIzquierdo.Width - 40
            };

            // Scan status indicator dot
            pnlScanIndicator = new Panel
            {
                Size = new Size(10, 10),
                Location = new Point(15, 12),
                BackColor = Color.Transparent
            };
            pnlScanIndicator.Paint += (s, ev) =>
            {
                ev.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                ev.Graphics.Clear(barcodePanel.BackColor);
                using (SolidBrush br = new SolidBrush(Color.FromArgb(0, 150, 255)))
                    ev.Graphics.FillEllipse(br, 0, 0, 9, 9);
            };
            barcodePanel.Controls.Add(pnlScanIndicator);

            Label lblBarcodeTitle = new Label
            {
                Text = "ESCÁNER DE CÓDIGO DE BARRAS",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 170, 255),
                Location = new Point(30, 9),
                AutoSize = true
            };
            barcodePanel.Controls.Add(lblBarcodeTitle);

            txtCodigoBarras = new TextBox
            {
                Text = "Escribe o escanea el código...",
                Font = new Font("Consolas", 12),
                ForeColor = Color.Gray,
                BackColor = Color.FromArgb(35, 40, 55),
                Location = new Point(15, 32),
                Width = 240,
                Height = 32,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtCodigoBarras.Enter += (s, e) => { if (txtCodigoBarras.Text == "Escribe o escanea el código...") { txtCodigoBarras.Text = ""; txtCodigoBarras.ForeColor = Color.FromArgb(0, 220, 120); } };
            txtCodigoBarras.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtCodigoBarras.Text)) { txtCodigoBarras.Text = "Escribe o escanea el código..."; txtCodigoBarras.ForeColor = Color.Gray; } };
            txtCodigoBarras.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    AgregarProductoPorCodigo(txtCodigoBarras.Text);
                    e.SuppressKeyPress = true;
                }
            };
            barcodePanel.Controls.Add(txtCodigoBarras);

            Button btnAgregarCodigo = new Button
            {
                Text = "✚ Agregar",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(95, 32),
                Location = new Point(265, 32),
                BackColor = Color.FromArgb(0, 180, 100),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnAgregarCodigo.FlatAppearance.BorderSize = 0;
            btnAgregarCodigo.Click += (s, e) => AgregarProductoPorCodigo(txtCodigoBarras.Text);
            barcodePanel.Controls.Add(btnAgregarCodigo);

            Button btnEscanear = new Button
            {
                Text = "📷 ESCANEAR",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 32),
                Location = new Point(370, 32),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnEscanear.FlatAppearance.BorderSize = 0;
            btnEscanear.Click += BtnEscanear_Click;
            barcodePanel.Controls.Add(btnEscanear);

            lblScanStatus = new Label
            {
                Text = "Listo para escanear",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(120, 130, 150),
                Location = new Point(15, 72),
                AutoSize = true
            };
            barcodePanel.Controls.Add(lblScanStatus);

            pnlTopIzq.Controls.Add(barcodePanel);

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
            lblItemsCount = new Label { Text = "0 articulos", Font = new Font("Segoe UI", 10), ForeColor = Color.White, Location = new Point(23, 45), AutoSize = true };
            Button btnCancelar = new Button { Text = "Cancelar", ForeColor = Color.White, BackColor = Color.Transparent, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10), Size = new Size(100, 30), Location = new Point(230, 20), Cursor = Cursors.Hand };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => CancelarVentaActual();
            cartHeader.Controls.Add(lblCartTitle);
            cartHeader.Controls.Add(lblItemsCount);
            cartHeader.Controls.Add(btnCancelar);
            panelDerecho.Controls.Add(cartHeader);

            gridCarrito = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };
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

            // ── Scan feedback timer ─────────────────────────────────
            timerScanFeedback = new Timer { Interval = 150 };
            timerScanFeedback.Tick += (s, e) =>
            {
                scanFeedbackCount++;
                if (scanFeedbackCount > 6)
                {
                    timerScanFeedback.Stop();
                    lblScanStatus.ForeColor = Color.FromArgb(120, 130, 150);
                    lblScanStatus.Text = "Listo para escanear";
                    pnlScanIndicator.Invalidate();
                }
            };

            this.Load += (s, e) => txtCodigoBarras.Focus();
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
                Label lblPrice = new Label { Text = $"${p.PrecioVenta:0.00}", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(30, 96, 255), Location = new Point(10, 160), AutoSize = true };
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

            // Show green feedback
            ShowScanSuccess(p.Nombre);
            UpdateCartUI();
        }

        private void ShowScanSuccess(string productoNombre)
        {
            lblScanStatus.Text = $"✅ {productoNombre} agregado al carrito";
            lblScanStatus.ForeColor = Color.FromArgb(0, 220, 80);
            scanFeedbackCount = 0;
            timerScanFeedback.Start();
        }

        private void BtnEscanear_Click(object sender, EventArgs e)
        {
            if (!puedeVender)
            {
                MessageBox.Show("Tu rol es de consulta. No puedes agregar productos al carrito.", "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var scanner = new BarcodeScannerForm())
            {
                if (scanner.ShowDialog(this) != DialogResult.OK)
                    return;

                txtCodigoBarras.Text = scanner.CodigoDetectado;
                txtCodigoBarras.ForeColor = Color.FromArgb(0, 220, 120);
                AgregarProductoPorCodigo(scanner.CodigoDetectado);
            }
        }

        private void AgregarProductoPorCodigo(string codigo)
        {
            if (!puedeVender)
            {
                MessageBox.Show("Tu rol es de consulta. No puedes agregar productos al carrito.", "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(codigo) || codigo == "Escribe o escanea el código...")
            {
                MessageBox.Show("Escribe o escanea un codigo de barras.", "Codigo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigoBarras.Focus();
                return;
            }

            var producto = repoProd.BuscarPorCodigoBarras(codigo.Trim());
            if (producto == null)
            {
                lblScanStatus.Text = $"❌ No se encontró producto con código: {codigo.Trim()}";
                lblScanStatus.ForeColor = Color.FromArgb(255, 80, 80);
                MessageBox.Show("No hay un producto registrado con el codigo: " + codigo.Trim(), "Producto no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCodigoBarras.SelectAll();
                txtCodigoBarras.Focus();
                return;
            }

            AddToCart(producto);
            txtBuscar.Text = producto.Nombre;
            txtBuscar.ForeColor = Color.Black;
            txtCodigoBarras.Clear();
            txtCodigoBarras.Focus();
        }

        private void UpdateCartUI()
        {
            gridCarrito.Controls.Clear();
            decimal subtotal = 0;
            int count = 0;

            if (ventaActual.Detalles.Count == 0)
            {
                Label empty = new Label { Text = "\n\n\nCarrito vacio\nSelecciona productos para comenzar", Font = new Font("Segoe UI", 12), ForeColor = Color.Gray, AutoSize = false, Size = new Size(310, 200), TextAlign = ContentAlignment.MiddleCenter };
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
                    Label lblUnit = new Label { Text = $"${d.PrecioUnitario:0.00} c/u", Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, Location = new Point(220, 35), AutoSize = true };
                    Label lblSub = new Label { Text = $"${d.Subtotal:0.00}", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(30, 96, 255), Location = new Point(220, 50), AutoSize = true };
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
            lblSubtotal.Text = $"${ventaActual.Subtotal:0.00}";
            lblIva.Text = $"${ventaActual.Iva:0.00}";
            lblTotal.Text = $"${ventaActual.Total:0.00}";
            lblItemsCount.Text = count == 1 ? "1 articulo" : $"{count} articulos";
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
                MessageBox.Show("El carrito esta vacio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CheckoutForm checkout = new CheckoutForm(ventaActual, AutorizarCancelacion);
            checkout.ShowDialog(this);

            if (checkout.VentaCancelada)
            {
                LimpiarCarrito();
                MessageBox.Show("Venta cancelada. El carrito se limpio automaticamente.", "Cancelacion realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            foreach (var d in ventaActual.Detalles)
            {
                string pName = d.ProductoNombre.Length > 20 ? d.ProductoNombre.Substring(0, 20) : d.ProductoNombre.PadRight(20);
                sb.AppendLine($"{d.Cantidad.ToString().PadRight(4)} | {pName} | ${d.Subtotal:0.00}");
            }
            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"Subtotal: ${ventaActual.Subtotal:0.00}");
            sb.AppendLine($"IVA (16%): ${ventaActual.Iva:0.00}");
            sb.AppendLine($"TOTAL:    ${ventaActual.Total:0.00}");
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
            MessageBox.Show("Venta cancelada. El carrito se limpio automaticamente.", "Cancelacion realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}
