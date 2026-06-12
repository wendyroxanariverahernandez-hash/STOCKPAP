using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using STOCKPAP.DataAccess;
using STOCKPAP.Models;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class InventarioView : UserControl
    {
        private FlowLayoutPanel gridProductos;
        private DataGridView dgvProductos;
        private Panel pnlAlertsContainer;
        private FlowLayoutPanel flowAlerts;
        private RoundedPanel panelFiltros;
        private Panel pnlTop;
        private TextBox txtBuscar;
        private Label lblSubtitle;
        private string _claseActiva = "Todas";
        private string _subclaseActiva = "Todas";
        private string _marcaActiva = "Todas";
        private int currentPage = 1;
        private int totalPages = 1;
        private Dictionary<int, int> pedidosRealizados = new Dictionary<int, int>();
        private bool puedeEditar;
        private bool esVistaTabla = false;
        private ProductoRepository repo;

        public InventarioView(bool puedeEditar = true)
        {
            this.puedeEditar = puedeEditar;
            repo = new ProductoRepository();
            InitializeComponent();
            LoadAlerts();
            LoadProductos();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            // ── Contenedor Superior (Título, Filtros, Alertas) ───────────────
            pnlTop = new Panel 
            { 
                Dock = DockStyle.Top, 
                Height = 230, // Se ajustará dinámicamente en LoadAlerts()
                Padding = new Padding(30, 30, 30, 0),
                BackColor = Color.Transparent
            };
            this.Controls.Add(pnlTop);

            // ── Título ──────────────────────────────────────────────────────
            Label lblTitle = new Label
            {
                Text = "Inventario",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 40),
                AutoSize = true,
                Location = new Point(30, 20)
            };
            pnlTop.Controls.Add(lblTitle);

            lblSubtitle = new Label
            {
                Text = "Cargando...",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(35, 62)
            };
            pnlTop.Controls.Add(lblSubtitle);

            // ── Botón de Cambio de Vista (Tarjetas vs Tabla) ────────────────
            RoundedButton btnViewCards = new RoundedButton
            {
                Text = "⊞ Tarjetas",
                Size = new Size(100, 36),
                Location = new Point(pnlTop.Width - 440, 25),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 8,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };

            RoundedButton btnViewTable = new RoundedButton
            {
                Text = "☰ Tabla",
                Size = new Size(100, 36),
                Location = new Point(pnlTop.Width - 335, 25),
                BackColor = Color.FromArgb(220, 225, 235),
                ForeColor = Color.FromArgb(60, 60, 80),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 8,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };

            pnlTop.Controls.Add(btnViewCards);
            pnlTop.Controls.Add(btnViewTable);

            // ── Botón Agregar Producto ──────────────────────────────────────
            RoundedButton btnAgregar = new RoundedButton
            {
                Text = "➕  Agregar Producto",
                Size = new Size(180, 36),
                Location = new Point(pnlTop.Width - 220, 25),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 8,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnAgregar.Click += BtnAgregar_Click;
            btnAgregar.Visible = puedeEditar;
            pnlTop.Controls.Add(btnAgregar);

            // ── Panel de Alertas de Stock Bajo ──────────────────────────────
            pnlAlertsContainer = new Panel
            {
                Location = new Point(30, 100),
                Height = 110,
                Width = pnlTop.Width - 60,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Visible = false
            };
            pnlTop.Controls.Add(pnlAlertsContainer);

            RoundedPanel pnlAlertsInner = new RoundedPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(255, 235, 235),
                BorderColor = Color.FromArgb(255, 180, 180),
                BorderSize = 1,
                BorderRadius = 12,
                Padding = new Padding(15, 10, 15, 10)
            };
            pnlAlertsContainer.Controls.Add(pnlAlertsInner);

            Label lblAlertsTitle = new Label
            {
                Text = "⚠  ALERTAS DE STOCK BAJO:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(190, 18, 60),
                Location = new Point(15, 8),
                AutoSize = true
            };
            pnlAlertsInner.Controls.Add(lblAlertsTitle);

            flowAlerts = new FlowLayoutPanel
            {
                Location = new Point(15, 30),
                Size = new Size(pnlAlertsInner.Width - 30, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            pnlAlertsInner.Controls.Add(flowAlerts);

            // ── Panel Búsqueda + Filtros (Clase, Subclase, Marca, Tipo) ─────
            panelFiltros = new RoundedPanel
            {
                Height = 110,
                Location = new Point(30, 105), // Se moverá dinámicamente si hay alertas
                BackColor = Color.White,
                BorderRadius = 12,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Width = pnlTop.Width - 60
            };
            pnlTop.Controls.Add(panelFiltros);

            // Icono búsqueda
            Label lblLupa = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point(15, 16)
            };
            panelFiltros.Controls.Add(lblLupa);

            txtBuscar = new TextBox
            {
                Text = "Buscar productos...",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Location = new Point(45, 15),
                Width = 720,
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            txtBuscar.Enter += (s, e) =>
            {
                if (txtBuscar.Text == "Buscar productos...")
                { txtBuscar.Text = ""; txtBuscar.ForeColor = Color.Black; }
            };
            txtBuscar.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                { txtBuscar.Text = "Buscar productos..."; txtBuscar.ForeColor = Color.Gray; }
            };
            txtBuscar.TextChanged += (s, e) =>
            {
                if (txtBuscar.Text != "Buscar productos...")
                    LoadProductos(txtBuscar.Text);
            };
            Panel lineSearch = new Panel
            {
                BackColor = Color.FromArgb(220, 220, 220),
                Height = 1, Width = 740, Location = new Point(15, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(lineSearch);

            // ComboBoxes de Filtro
            Label lblClase = new Label { Text = "Clase:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 80), AutoSize = true, Location = new Point(15, 66) };
            ComboBox cmbClase = new ComboBox { Location = new Point(60, 62), Size = new Size(130, 28), Font = new Font("Segoe UI", 9.5f), DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblSubclase = new Label { Text = "Subclase:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 80), AutoSize = true, Location = new Point(200, 66) };
            ComboBox cmbSubclase = new ComboBox { Location = new Point(265, 62), Size = new Size(130, 28), Font = new Font("Segoe UI", 9.5f), DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblMarca = new Label { Text = "Marca:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(60, 60, 80), AutoSize = true, Location = new Point(410, 66) };
            ComboBox cmbMarca = new ComboBox { Location = new Point(465, 62), Size = new Size(130, 28), Font = new Font("Segoe UI", 9.5f), DropDownStyle = ComboBoxStyle.DropDownList };

            panelFiltros.Controls.Add(lblClase); panelFiltros.Controls.Add(cmbClase);
            panelFiltros.Controls.Add(lblSubclase); panelFiltros.Controls.Add(cmbSubclase);
            panelFiltros.Controls.Add(lblMarca); panelFiltros.Controls.Add(cmbMarca);

            Action cargarMarcas = () =>
            {
                cmbMarca.Items.Clear();
                cmbMarca.Items.Add("Todas");
                cmbMarca.Items.AddRange(repo.ObtenerMarcas(_claseActiva, _subclaseActiva).ToArray());
                cmbMarca.SelectedItem = "Todas";
                _marcaActiva = "Todas";
            };

            Action cargarSubclases = () =>
            {
                cmbSubclase.Items.Clear();
                cmbSubclase.Items.Add("Todas");
                cmbSubclase.Items.AddRange(repo.ObtenerSubclases(_claseActiva).ToArray());
                cmbSubclase.SelectedItem = "Todas";
                _subclaseActiva = "Todas";
                _marcaActiva = "Todas";
                cargarMarcas();
            };

            Action cargarClases = () =>
            {
                cmbClase.Items.Clear();
                cmbClase.Items.Add("Todas");
                cmbClase.Items.AddRange(repo.ObtenerClases().ToArray());
                cmbClase.SelectedItem = "Todas";
                _claseActiva = "Todas";
                cargarSubclases();
            };

            cmbClase.SelectedIndexChanged += (s, e) =>
            {
                _claseActiva = cmbClase.SelectedItem?.ToString() ?? "Todas";
                cargarSubclases();
                LoadProductos(txtBuscar.Text == "Buscar productos..." ? "" : txtBuscar.Text);
            };

            cmbSubclase.SelectedIndexChanged += (s, e) =>
            {
                _subclaseActiva = cmbSubclase.SelectedItem?.ToString() ?? "Todas";
                cargarMarcas();
                LoadProductos(txtBuscar.Text == "Buscar productos..." ? "" : txtBuscar.Text);
            };

            cmbMarca.SelectedIndexChanged += (s, e) =>
            {
                _marcaActiva = cmbMarca.SelectedItem?.ToString() ?? "Todas";
                LoadProductos(txtBuscar.Text == "Buscar productos..." ? "" : txtBuscar.Text);
            };

            cargarClases();

            // ── Grid & Table Containment ─────────────────────────────────────
            Panel pnlFill = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30, 0, 30, 30) };
            this.Controls.Add(pnlFill);
            pnlFill.BringToFront();

            // Vista Tarjetas (Grid)
            gridProductos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 5, 0, 5),
                Visible = true
            };
            pnlFill.Controls.Add(gridProductos);

            // Vista Tabla (DataGridView)
            dgvProductos = CrearGrid();
            dgvProductos.Visible = false;
            pnlFill.Controls.Add(dgvProductos);

            // Definir Columnas de DataGridView
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID", Width = 50 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "Nombre", Width = 210 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "Cód. Barras", Width = 110 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colClase", HeaderText = "Clase", Width = 110 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSubclase", HeaderText = "Subclase", Width = 110 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMarca", HeaderText = "Marca", Width = 100 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrecioCompra", HeaderText = "Precio Compra", Width = 100 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrecio", HeaderText = "Precio Venta", Width = 95 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStock", HeaderText = "Stock", Width = 70 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colProveedor", HeaderText = "Proveedor", Width = 150 });

            // Doble click para editar en la tabla
            dgvProductos.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    int id = Convert.ToInt32(dgvProductos.Rows[e.RowIndex].Cells["colId"].Value);
                    var prod = repo.ObtenerTodos().FirstOrDefault(p => p.Id == id);
                    if (prod != null)
                    {
                        EditarProducto(prod);
                    }
                }
            };

            // Acciones de toggle
            Action refrescarVista = () =>
            {
                gridProductos.Visible = !esVistaTabla;
                dgvProductos.Visible = esVistaTabla;

                btnViewCards.BackColor = !esVistaTabla ? Color.FromArgb(30, 96, 255) : Color.FromArgb(220, 225, 235);
                btnViewCards.ForeColor = !esVistaTabla ? Color.White : Color.FromArgb(60, 60, 80);

                btnViewTable.BackColor = esVistaTabla ? Color.FromArgb(30, 96, 255) : Color.FromArgb(220, 225, 235);
                btnViewTable.ForeColor = esVistaTabla ? Color.White : Color.FromArgb(60, 60, 80);
            };

            btnViewCards.Click += (s, e) => { esVistaTabla = false; refrescarVista(); };
            btnViewTable.Click += (s, e) => { esVistaTabla = true; refrescarVista(); };
        }

        private DataGridView CrearGrid()
        {
            DataGridView grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Segoe UI", 9.5f),
                GridColor = Color.FromArgb(235, 238, 245)
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 80);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersHeight = 36;
            grid.RowTemplate.Height = 34;
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 235, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(20, 20, 40);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 255);
            return grid;
        }

        private void LoadAlerts()
        {
            flowAlerts.Controls.Clear();

            if (ConfigHelper.Obtener("AlertasActivas", "true").ToLower() != "true")
            {
                pnlAlertsContainer.Visible = false;
                pnlAlertsContainer.Height = 0;
                panelFiltros.Location = new Point(30, 105);
                pnlTop.Height = 230;
                return;
            }

            // Sincronizar stock
            var todos = repo.ObtenerTodos();
            foreach (var p in todos)
            {
                if (p.Stock <= p.StockMinimo)
                    repo.RegistrarAlerta(p.Id, p.Stock, p.StockMinimo);
                else
                    repo.AutoResolverAlertas(p.Id, p.Stock, p.StockMinimo);
            }

            var list = repo.ObtenerAlertasActivas();

            if (list.Count > 0)
            {
                pnlAlertsContainer.Visible = true;
                pnlAlertsContainer.Height = 110;
                panelFiltros.Location = new Point(30, 220);
                pnlTop.Height = 340;

                foreach (var alert in list)
                {
                    flowAlerts.Controls.Add(CrearCardAlerta(alert));
                }
            }
            else
            {
                pnlAlertsContainer.Visible = false;
                pnlAlertsContainer.Height = 0;
                panelFiltros.Location = new Point(30, 105);
                pnlTop.Height = 230;
            }
        }

        private Panel CrearCardAlerta(AlertaStock a)
        {
            RoundedPanel card = new RoundedPanel
            {
                Size = new Size(295, 52),
                BackColor = Color.White,
                BorderColor = Color.FromArgb(255, 200, 200),
                BorderSize = 1,
                BorderRadius = 8,
                Margin = new Padding(0, 0, 10, 0)
            };

            Label lblName = new Label
            {
                Text = a.ProductoNombre,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(8, 4),
                Size = new Size(180, 18),
                AutoEllipsis = true
            };
            card.Controls.Add(lblName);

            Label lblInfo = new Label
            {
                Text = $"Stock: {a.StockActual} (Mín: {a.StockMinimo})  |  {a.FechaGeneracion:dd/MM/yyyy}",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.Gray,
                Location = new Point(8, 24),
                Size = new Size(180, 16)
            };
            card.Controls.Add(lblInfo);

            Button btnAtender = new Button
            {
                Text = "Atiende",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Size = new Size(58, 22),
                Location = new Point(190, 14),
                BackColor = Color.FromArgb(255, 240, 240),
                ForeColor = Color.FromArgb(190, 18, 60),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAtender.FlatAppearance.BorderColor = Color.FromArgb(255, 180, 180);
            btnAtender.Click += (sender, e) => PedirMasProductos(a.ProductoId);
            card.Controls.Add(btnAtender);

            Button btnPedir = new Button
            {
                Text = "🛒",
                Font = new Font("Segoe UI", 9),
                Size = new Size(25, 22),
                Location = new Point(252, 14),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPedir.FlatAppearance.BorderSize = 0;
            btnPedir.Click += (sender, e) =>
            {
                if (!pedidosRealizados.ContainsKey(a.Id))
                {
                    MessageBox.Show("Primero debe realizar el pedido dando clic en 'Atiende'.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int cantidadSugerida = pedidosRealizados[a.Id];
                int stockAnterior = a.StockActual;
                int stockNuevo = stockAnterior + cantidadSugerida;

                using (var conn = Conexion.Instance.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("UPDATE Productos SET Stock = Stock + @qty WHERE Id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("qty", cantidadSugerida);
                        cmd.Parameters.AddWithValue("id", a.ProductoId);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmdMov = new NpgsqlCommand("INSERT INTO Movimientos (Tipo, ProductoId, Cantidad, StockAnterior, StockNuevo, Motivo) VALUES ('Entrada', @p, @c, @sa, @sn, 'Reabastecimiento')", conn))
                    {
                        cmdMov.Parameters.AddWithValue("p", a.ProductoId);
                        cmdMov.Parameters.AddWithValue("c", cantidadSugerida);
                        cmdMov.Parameters.AddWithValue("sa", stockAnterior);
                        cmdMov.Parameters.AddWithValue("sn", stockNuevo);
                        cmdMov.ExecuteNonQuery();
                    }
                }
                
                pedidosRealizados.Remove(a.Id);
                MessageBox.Show($"Se han agregado {cantidadSugerida} unidades al inventario automáticamente.", "Recepción de Stock", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAlerts();
                LoadProductos(txtBuscar.Text == "Buscar productos..." ? "" : txtBuscar.Text);
            };
            card.Controls.Add(btnPedir);

            return card;
        }

        private void PedirMasProductos(int productoId)
        {
            var p = repo.ObtenerTodos().FirstOrDefault(prod => prod.Id == productoId);
            if (p == null) return;

            using (var form = new ReabastecimientoForm(p))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    var alert = repo.ObtenerAlertasActivas().FirstOrDefault(a => a.ProductoId == productoId);
                    if (alert != null)
                    {
                        pedidosRealizados[alert.Id] = form.CantidadPedida;
                    }
                }
            }
        }

        private void LoadProductos(string busqueda = "")
        {
            if (gridProductos == null || dgvProductos == null) return;

            gridProductos.Controls.Clear();
            dgvProductos.Rows.Clear();

            List<Producto> lista;
            if (!string.IsNullOrEmpty(busqueda))
                lista = repo.BuscarPorNombre(busqueda);
            else
                lista = repo.BuscarPorNuevosFiltros(_claseActiva, _subclaseActiva, _marcaActiva, "Todas");

            int stockBajo = 0;
            foreach (var p in lista)
                if (p.Stock <= p.StockMinimo) stockBajo++;

            lblSubtitle.Text = $"{lista.Count} producto(s)  •  {stockBajo} con stock bajo";

            // Llenar Grid (Cards)
            if (lista.Count == 0)
            {
                Label lblVacio = new Label
                {
                    Text = "No se encontraron productos.",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(20)
                };
                gridProductos.Controls.Add(lblVacio);
            }
            else
            {
                foreach (var p in lista)
                    gridProductos.Controls.Add(CrearCardProducto(p));
            }

            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            bool isDark = ConfigHelper.Obtener("TemaVisual", "Modo Claro") == "Modo Oscuro";
            Color bgRed = isDark ? Color.FromArgb(80, 40, 40) : Color.FromArgb(255, 235, 235);
            Color bgYellow = isDark ? Color.FromArgb(80, 75, 30) : Color.FromArgb(255, 252, 230);

            // Llenar Tabla
            foreach (var p in lista)
            {
                int rowIndex = dgvProductos.Rows.Add(
                    p.Id,
                    p.Nombre,
                    p.CodigoBarras,
                    p.Clase,
                    p.Subclase,
                    p.Marca,
                    $"{sym}{p.PrecioCompra:0.00}",
                    $"{sym}{p.PrecioVenta:0.00}",
                    p.Stock,
                    string.IsNullOrEmpty(p.ProveedorNombre) ? "Sin proveedor" : p.ProveedorNombre
                );

                if (p.Stock <= 0)
                {
                    dgvProductos.Rows[rowIndex].DefaultCellStyle.BackColor = bgRed;
                }
                else if (p.Stock <= p.StockMinimo)
                {
                    dgvProductos.Rows[rowIndex].DefaultCellStyle.BackColor = bgYellow;
                }
            }
        }

        private Panel CrearCardProducto(Producto p)
        {
            RoundedPanel card = new RoundedPanel
            {
                Size = new Size(210, 320),
                BackColor = Color.White,
                BorderRadius = 14,
                Margin = new Padding(4)
            };

            // Imagen
            PictureBox pic = new PictureBox
            {
                Size = new Size(210, 130),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(245, 247, 250)
            };
            if (!string.IsNullOrEmpty(p.ImagePath))
            {
                string path = Path.Combine(Application.StartupPath, "Assets", "Images", p.ImagePath);
                if (File.Exists(path))
                {
                    try { pic.Image = Image.FromFile(path); }
                    catch { }
                }
            }
            if (pic.Image == null)
            {
                pic.Paint += (s, ev) =>
                {
                    ev.Graphics.Clear(Color.FromArgb(230, 235, 250));
                    string ini = p.Nombre.Length > 0 ? p.Nombre[0].ToString().ToUpper() : "?";
                    using (Font f = new Font("Segoe UI", 32, FontStyle.Bold))
                        ev.Graphics.DrawString(ini, f, new SolidBrush(Color.FromArgb(30, 96, 255)),
                            new RectangleF(0, 0, 210, 130),
                            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                };
            }
            card.Controls.Add(pic);

            // Nombre
            Label lblName = new Label
            {
                Text = p.Nombre,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 40),
                Location = new Point(12, 138),
                AutoSize = false,
                Size = new Size(186, 38)
            };
            card.Controls.Add(lblName);

            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            // Precio
            Label lblPrice = new Label
            {
                Text = $"{sym}{p.PrecioVenta:0.00}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 96, 255),
                Location = new Point(12, 178),
                AutoSize = true
            };
            card.Controls.Add(lblPrice);

            // Badge stock
            bool bajoStock = p.Stock <= p.StockMinimo;
            RoundedPanel badge = new RoundedPanel
            {
                Size = new Size(125, 22),
                Location = new Point(12, 204),
                BackColor = bajoStock ? Color.FromArgb(255, 235, 235) : Color.FromArgb(230, 250, 235),
                BorderRadius = 8
            };
            Label lblStock = new Label
            {
                Text = bajoStock ? $"⚠ Stock bajo ({p.Stock})" : $"✓ {p.Stock} unidades",
                ForeColor = bajoStock ? Color.FromArgb(180, 30, 30) : Color.FromArgb(30, 140, 60),
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            badge.Controls.Add(lblStock);
            card.Controls.Add(badge);

            // Botón Pedir Más
            Button btnPedirMas = new Button
            {
                Text = "Pedir más productos",
                Location = new Point(12, 232),
                Size = new Size(186, 26),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(245, 247, 250),
                ForeColor = Color.FromArgb(30, 96, 255),
                Cursor = Cursors.Hand
            };
            btnPedirMas.FlatAppearance.BorderColor = Color.FromArgb(200, 215, 255);
            btnPedirMas.Click += (s, e) => PedirMasProductos(p.Id);
            card.Controls.Add(btnPedirMas);

            // Botones Editar / Eliminar
            Button btnEditar = new Button
            {
                Text = "✏ Editar",
                Location = new Point(12, 276),
                Size = new Size(84, 30),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 244, 255),
                ForeColor = Color.FromArgb(30, 96, 255),
                Cursor = Cursors.Hand
            };
            btnEditar.FlatAppearance.BorderColor = Color.FromArgb(180, 200, 255);
            btnEditar.Click += (s, e) => EditarProducto(p);
            if (puedeEditar) card.Controls.Add(btnEditar);

            Button btnEliminar = new Button
            {
                Text = "🗑 Eliminar",
                Location = new Point(104, 276),
                Size = new Size(94, 30),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 240, 240),
                ForeColor = Color.FromArgb(180, 30, 30),
                Cursor = Cursors.Hand
            };
            btnEliminar.FlatAppearance.BorderColor = Color.FromArgb(255, 180, 180);
            btnEliminar.Click += (s, e) => EliminarProducto(p);
            if (puedeEditar) card.Controls.Add(btnEliminar);

            return card;
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            using (var form = new ProductoForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadAlerts();
                    LoadProductos();
                }
            }
        }

        private void EditarProducto(Producto p)
        {
            using (var form = new ProductoForm(p))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadAlerts();
                    LoadProductos();
                }
            }
        }

        private void EliminarProducto(Producto p)
        {
            var res = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar el producto:\n\n\"{p.Nombre}\"?\n\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (res == DialogResult.Yes)
            {
                if (repo.EliminarProducto(p.Id))
                {
                    MessageBox.Show("Producto eliminado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAlerts();
                    LoadProductos();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el producto.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
