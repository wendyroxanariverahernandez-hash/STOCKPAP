using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using STOCKPAP.DataAccess;
using STOCKPAP.Models;
using STOCKPAP.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = System.Drawing.Font;

namespace STOCKPAP.Views
{
    public class ReportesView : UserControl
    {
        private readonly ProductoRepository repoProducto;
        private readonly ProveedorRepository repoProveedor;
        private readonly MovimientoRepository repoMovimiento;
        private readonly Usuario currentUser;
        private readonly bool puedeEditar;

        private DataGridView dgvProductos;
        private DataGridView dgvMovimientos;
        private Label lblSubtitle;
        private Label lblTotalProd;
        private Label lblTotalUnidades;
        private Label lblValorInv;
        private Label lblStockBajo;

        // Filtros de Fecha
        private ComboBox cmbRangoFecha;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private Panel pnlCustomDates;

        private Label lblReporte;
        private ComboBox cmbTipoReporte;
        private RoundedButton btnExportar;

        public ReportesView(Usuario user)
        {
            this.currentUser = user;
            this.puedeEditar = user != null && string.Equals(user.Rol, "Admin", StringComparison.OrdinalIgnoreCase);
            repoProducto = new ProductoRepository();
            repoProveedor = new ProveedorRepository();
            repoMovimiento = new MovimientoRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);
            this.AutoScroll = true;

            // ── Título ──────────────────────────────────────────────────────
            Label lblTitle = new Label
            {
                Text = "Movimientos y Reportes",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 40),
                AutoSize = true,
                Location = new Point(30, 25)
            };
            Controls.Add(lblTitle);

            lblSubtitle = new Label
            {
                Text = "Consulta inventario, movimientos y exporta reportes detallados",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(35, 68)
            };
            Controls.Add(lblSubtitle);

            // ── Panel Superior (Filtros y Exportación) ───────────────────
            RoundedPanel panelSuperior = new RoundedPanel
            {
                Location = new Point(30, 110),
                Size = new Size(960, 80),
                BackColor = Color.White,
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(panelSuperior);

            // Filtro Rango de Tiempo
            Label lblFiltro = new Label
            {
                Text = "Rango de Tiempo:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 80),
                Location = new Point(15, 30),
                AutoSize = true
            };
            panelSuperior.Controls.Add(lblFiltro);

            cmbRangoFecha = new ComboBox
            {
                Location = new Point(135, 27),
                Width = 120,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRangoFecha.Items.AddRange(new[] { "Todo el historial", "Hoy (Día)", "Esta semana", "Este mes", "Personalizado" });
            cmbRangoFecha.SelectedIndex = 0;
            cmbRangoFecha.SelectedIndexChanged += CmbRangoFecha_SelectedIndexChanged;
            panelSuperior.Controls.Add(cmbRangoFecha);

            pnlCustomDates = new Panel
            {
                Location = new Point(260, 20),
                Size = new Size(295, 40),
                BackColor = Color.Transparent,
                Visible = false
            };
            panelSuperior.Controls.Add(pnlCustomDates);

            Label lblDesde = new Label { Text = "Desde:", Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, Location = new Point(0, 12), AutoSize = true };
            dtpDesde = new DateTimePicker { Location = new Point(45, 8), Width = 100, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 9.5f) };
            Label lblHasta = new Label { Text = "Hasta:", Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, Location = new Point(150, 12), AutoSize = true };
            dtpHasta = new DateTimePicker { Location = new Point(190, 8), Width = 100, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 9.5f) };
            
            dtpDesde.ValueChanged += (s, e) => LoadData();
            dtpHasta.ValueChanged += (s, e) => LoadData();

            pnlCustomDates.Controls.Add(lblDesde);
            pnlCustomDates.Controls.Add(dtpDesde);
            pnlCustomDates.Controls.Add(lblHasta);
            pnlCustomDates.Controls.Add(dtpHasta);

            // Botón Nuevo Movimiento (Mover a la derecha del todo)
            RoundedButton btnNuevoMovimiento = CrearBoton("✚ Movimiento", Color.FromArgb(30, 96, 255));
            btnNuevoMovimiento.Size = new Size(105, 32);
            btnNuevoMovimiento.Location = new Point(850, 25);
            btnNuevoMovimiento.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNuevoMovimiento.Visible = puedeEditar;
            btnNuevoMovimiento.Click += BtnNuevoMovimiento_Click;
            panelSuperior.Controls.Add(btnNuevoMovimiento);

            // Sección de Exportación de Reportes
            lblReporte = new Label
            {
                Text = "Reporte:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 80),
                Location = new Point(270, 30),
                AutoSize = true
            };
            panelSuperior.Controls.Add(lblReporte);

            cmbTipoReporte = new ComboBox
            {
                Location = new Point(335, 27),
                Width = 125,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTipoReporte.Items.AddRange(new[] { "Inventario PDF", "Inventario Excel", "Movimientos PDF", "Movimientos Excel", "Consolidado PDF" });
            
            string defFormat = ConfigHelper.Obtener("FormatoReporteDefecto", "PDF");
            cmbTipoReporte.SelectedIndex = defFormat == "Excel (.xls)" ? 1 : 0;
            
            panelSuperior.Controls.Add(cmbTipoReporte);

            btnExportar = CrearBoton("Exportar", Color.FromArgb(22, 163, 74));
            btnExportar.Size = new Size(80, 32);
            btnExportar.Location = new Point(470, 25);
            btnExportar.Click += (s, e) => {
                string sel = cmbTipoReporte.SelectedItem.ToString();
                if (sel == "Inventario PDF") ExportarInventarioPdf();
                else if (sel == "Inventario Excel") ExportarInventarioExcel();
                else if (sel == "Movimientos PDF") ExportarMovimientosPdf();
                else if (sel == "Movimientos Excel") ExportarMovimientosExcel();
                else if (sel == "Consolidado PDF") ExportarTodoJuntoPdf();
            };
            panelSuperior.Controls.Add(btnExportar);

            // Ajuste visual si fechas personalizadas están visibles
            cmbRangoFecha.SelectedIndexChanged += (s, e) => {
                bool isCustom = cmbRangoFecha.SelectedIndex == 4;
                // Si es custom, el reporte consolidado/movimientos tomara las fechas de dtp
            };


            // ── Estadísticas ────────────────────────────────────────────────
            int statsY = 210;
            lblTotalProd = CrearTarjetaStat("Total Productos", "...", "productos registrados", Color.FromArgb(30, 96, 255), 30, statsY);
            lblTotalUnidades = CrearTarjetaStat("Unidades Totales", "...", "en inventario", Color.FromArgb(22, 163, 74), 225, statsY);
            lblValorInv = CrearTarjetaStat("Valor Inventario", "...", "valor de venta", Color.FromArgb(234, 88, 12), 420, statsY);
            lblStockBajo = CrearTarjetaStat("Stock Bajo", "...", "necesitan reabastecer", Color.FromArgb(190, 18, 60), 615, statsY);

            // ── Tabla Detalle Inventario ────────────────────────────────────
            RoundedPanel panelInventario = CrearPanelTabla("Detalle del Inventario", 30, 360, 960, 320);
            dgvProductos = CrearGrid();
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "Producto", FillWeight = 26 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colClase", HeaderText = "Clase", FillWeight = 14 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMarca", HeaderText = "Marca", FillWeight = 12 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVenta", HeaderText = "Precio Venta", FillWeight = 12 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStock", HeaderText = "Stock", FillWeight = 9 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMinimo", HeaderText = "Mínimo", FillWeight = 9 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colValor", HeaderText = "Valor Total", FillWeight = 12 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "Estado", FillWeight = 12 });
            dgvProductos.CellFormatting += DgvProductos_CellFormatting;
            panelInventario.Controls.Add(dgvProductos);
            Controls.Add(panelInventario);

            // ── Tabla Historial Movimientos ─────────────────────────────────
            RoundedPanel panelMovimientos = CrearPanelTabla("Historial de Movimientos", 30, 710, 960, 320);
            dgvMovimientos = CrearGrid();
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Fecha", FillWeight = 18 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTipo", HeaderText = "Tipo", FillWeight = 12 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colProducto", HeaderText = "Producto", FillWeight = 28 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCantidad", HeaderText = "Cant.", FillWeight = 10 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStock", HeaderText = "Stock", FillWeight = 18 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetalle", HeaderText = "Detalle", FillWeight = 34 });
            panelMovimientos.Controls.Add(dgvMovimientos);
            Controls.Add(panelMovimientos);

            // Spacer de scroll
            Panel spacer = new Panel { Location = new Point(0, 1025), Size = new Size(10, 30), BackColor = Color.Transparent };
            Controls.Add(spacer);
        }

        private RoundedButton CrearBoton(string texto, Color color)
        {
            return new RoundedButton
            {
                Text = texto,
                Size = new Size(145, 36),
                Margin = new Padding(0, 0, 8, 8),
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 8,
                Cursor = Cursors.Hand
            };
        }

        private Label CrearTarjetaStat(string titulo, string valor, string subtitulo, Color accentColor, int x, int y)
        {
            RoundedPanel card = new RoundedPanel { Size = new Size(180, 130), Location = new Point(x, y), BackColor = Color.White, BorderRadius = 14 };
            card.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 5, BackColor = accentColor });
            card.Controls.Add(new Label { Text = titulo, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(14, 18) });
            Label lblValor = new Label { Text = valor, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = accentColor, AutoSize = true, Location = new Point(14, 45) };
            card.Controls.Add(lblValor);
            card.Controls.Add(new Label { Text = subtitulo, Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, AutoSize = false, Size = new Size(152, 30), Location = new Point(14, 95) });
            Controls.Add(card);
            return lblValor;
        }

        private RoundedPanel CrearPanelTabla(string titulo, int x, int y, int w, int h)
        {
            RoundedPanel panel = new RoundedPanel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = Color.White,
                BorderRadius = 14,
                Padding = new Padding(10, 50, 10, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            panel.Controls.Add(new Label { Text = titulo, Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, Location = new Point(15, 15), ForeColor = Color.FromArgb(20, 20, 40) });
            return panel;
        }

        private DataGridView CrearGrid()
        {
            DataGridView grid = new DataGridView
            {
                Location = new Point(10, 50),
                Size = new Size(0, 0),
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                Font = new Font("Segoe UI", 9),
                GridColor = Color.FromArgb(235, 238, 245)
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 80);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 247, 250);
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(60, 60, 80);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            grid.ColumnHeadersHeight = 34;
            grid.RowTemplate.Height = 32;
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 235, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(20, 20, 40);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 255);
            return grid;
        }

        private void CmbRangoFecha_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isCustom = cmbRangoFecha.SelectedIndex == 4;
            pnlCustomDates.Visible = isCustom;

            if (isCustom)
            {
                lblReporte.Location = new Point(560, 30);
                cmbTipoReporte.Location = new Point(630, 27);
                btnExportar.Location = new Point(765, 25);
            }
            else
            {
                lblReporte.Location = new Point(270, 30);
                cmbTipoReporte.Location = new Point(340, 27);
                btnExportar.Location = new Point(475, 25);
            }

            LoadData();
        }

        private DateTime GetFechaInicioFiltro()
        {
            int idx = cmbRangoFecha.SelectedIndex;
            if (idx == 1) // Hoy
                return DateTime.Today;
            if (idx == 2) // Esta semana
                return DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek); // Principio de semana
            if (idx == 3) // Este mes
                return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (idx == 4) // Personalizado
                return dtpDesde.Value.Date;
            
            return DateTime.MinValue; // Todo el historial
        }

        private DateTime GetFechaFinFiltro()
        {
            int idx = cmbRangoFecha.SelectedIndex;
            if (idx == 1) // Hoy
                return DateTime.Today.AddDays(1).AddTicks(-1);
            if (idx == 2) // Esta semana
                return DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek).AddTicks(-1);
            if (idx == 3) // Este mes
                return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)).AddTicks(-1);
            if (idx == 4) // Personalizado
                return dtpHasta.Value.Date.AddDays(1).AddTicks(-1);

            return DateTime.MaxValue; // Todo
        }

        private void LoadData()
        {
            var productos = repoProducto.ObtenerTodos();
            var movimientos = repoMovimiento.ObtenerTodos();

            // Filtrar movimientos por fecha
            DateTime inicio = GetFechaInicioFiltro();
            DateTime fin = GetFechaFinFiltro();
            movimientos = movimientos.Where(m => m.Fecha >= inicio && m.Fecha <= fin).ToList();

            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            // Actualizar tarjetas
            lblTotalProd.Text = productos.Count.ToString();
            lblTotalUnidades.Text = productos.Sum(p => p.Stock).ToString();
            lblValorInv.Text = $"{sym}{productos.Sum(p => p.Stock * p.PrecioVenta):0.00}";
            lblStockBajo.Text = productos.Count(p => p.Stock <= p.StockMinimo).ToString();

            lblSubtitle.Text = $"Rango: {cmbRangoFecha.Text}  |  Actualizado: {DateTime.Now:dd/MM/yyyy HH:mm}  |  {movimientos.Count} movimientos";

            // Detalle de Inventario
            dgvProductos.Rows.Clear();
            foreach (var p in productos)
            {
                bool bajo = p.Stock <= p.StockMinimo;
                string clase = string.IsNullOrEmpty(p.Clase) ? "Sin clase" : p.Clase;
                string marca = string.IsNullOrEmpty(p.Marca) ? "Sin marca" : p.Marca;
                dgvProductos.Rows.Add(p.Nombre, clase, marca, $"{sym}{p.PrecioVenta:0.00}", p.Stock, p.StockMinimo, $"{sym}{p.Stock * p.PrecioVenta:0.00}", bajo ? "Stock Bajo" : "OK");
            }

            // Historial de Movimientos
            dgvMovimientos.Rows.Clear();
            foreach (var m in movimientos)
            {
                dgvMovimientos.Rows.Add(m.Fecha.ToString("dd/MM/yyyy HH:mm"), m.Tipo, m.ProductoNombre, m.Cantidad, $"{m.StockAnterior} -> {m.StockNuevo}", CrearDetalleMovimiento(m));
            }
        }

        private void BtnNuevoMovimiento_Click(object sender, EventArgs e)
        {
            using (var form = new MovimientoForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
        }

        private string CrearDetalleMovimiento(Movimiento movimiento)
        {
            if (!string.IsNullOrWhiteSpace(movimiento.Motivo))
                return movimiento.Motivo;

            return $"{movimiento.Tipo} de {Math.Abs(movimiento.Cantidad)} unidad(es).";
        }

        private void DgvProductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvProductos.Columns["colEstado"].Index && e.Value != null)
            {
                string val = e.Value.ToString();
                e.CellStyle.ForeColor = val.Contains("Bajo") ? Color.FromArgb(190, 18, 60) : Color.FromArgb(22, 163, 74);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }

        // ── Generación de Reportes PDF con Metadatos y Totales ────────────────
        private void ExportarInventarioPdf()
        {
            var productos = repoProducto.ObtenerTodos();
            string empresa = ConfigHelper.Obtener("NombreEmpresa", "StockPap");
            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            var sb = new StringBuilder();
            sb.AppendLine("==========================================================================");
            sb.AppendLine($"                        REPORTE DE INVENTARIO - {empresa.ToUpper()}");
            sb.AppendLine("==========================================================================");
            sb.AppendLine($"Generado por: {currentUser?.Username ?? "Usuario"}");
            sb.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Filtros aplicados: Categorías: Todas");
            sb.AppendLine("--------------------------------------------------------------------------");
            sb.AppendLine($"Total Productos Diferentes: {productos.Count}");
            sb.AppendLine($"Total de Piezas en Stock: {productos.Sum(p => p.Stock)}");
            sb.AppendLine($"Valor del Inventario (Venta): {sym}{productos.Sum(p => p.Stock * p.PrecioVenta):0.00}");
            sb.AppendLine($"Productos con Stock Bajo: {productos.Count(p => p.Stock <= p.StockMinimo)}");
            sb.AppendLine("==========================================================================");
            sb.AppendLine();
            sb.AppendLine("PRODUCTO | CLASE | MARCA | PRECIO VENTA | STOCK | STOCK MÍN | ESTADO");
            sb.AppendLine("--------------------------------------------------------------------------");
            foreach (var p in productos.OrderBy(x => x.Nombre))
            {
                string estado = p.Stock <= p.StockMinimo ? "Bajo" : "OK";
                string clase = string.IsNullOrEmpty(p.Clase) ? "N/A" : p.Clase;
                string marca = string.IsNullOrEmpty(p.Marca) ? "N/A" : p.Marca;
                sb.AppendLine($"{p.Nombre} | {clase} | {marca} | {sym}{p.PrecioVenta:0.00} | {p.Stock} | {p.StockMinimo} | {estado}");
            }

            Exportar.GuardarArchivoPdf("Reporte_Inventario", sb.ToString());
        }

        private void ExportarMovimientosPdf()
        {
            var movimientos = repoMovimiento.ObtenerTodos();
            DateTime inicio = GetFechaInicioFiltro();
            DateTime fin = GetFechaFinFiltro();
            movimientos = movimientos.Where(m => m.Fecha >= inicio && m.Fecha <= fin).ToList();

            string empresa = ConfigHelper.Obtener("NombreEmpresa", "StockPap");

            var sb = new StringBuilder();
            sb.AppendLine("==========================================================================");
            sb.AppendLine($"                       REPORTE DE MOVIMIENTOS - {empresa.ToUpper()}");
            sb.AppendLine("==========================================================================");
            sb.AppendLine($"Generado por: {currentUser?.Username ?? "Usuario"}");
            sb.AppendLine($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Filtro Temporal: {cmbRangoFecha.Text}");
            sb.AppendLine($"Rango de fechas: {inicio:dd/MM/yyyy} - {fin:dd/MM/yyyy}");
            sb.AppendLine("--------------------------------------------------------------------------");
            sb.AppendLine($"Total Movimientos en Rango: {movimientos.Count}");
            sb.AppendLine($"Total Entradas: {movimientos.Where(m => m.Tipo == "Entrada").Sum(m => Math.Abs(m.Cantidad))} unidades");
            sb.AppendLine($"Total Salidas: {movimientos.Where(m => m.Tipo == "Salida").Sum(m => Math.Abs(m.Cantidad))} unidades");
            sb.AppendLine("==========================================================================");
            sb.AppendLine();
            sb.AppendLine("FECHA Y HORA | TIPO | PRODUCTO | CANTIDAD | STOCK ANT | STOCK NUEVO | MOTIVO");
            sb.AppendLine("--------------------------------------------------------------------------");
            foreach (var m in movimientos)
            {
                string signo = m.Tipo == "Entrada" ? "+" : "-";
                sb.AppendLine($"{m.Fecha:dd/MM/yyyy HH:mm} | {m.Tipo} | {m.ProductoNombre} | {signo}{Math.Abs(m.Cantidad)} | {m.StockAnterior} | {m.StockNuevo} | {CrearDetalleMovimiento(m)}");
            }

            Exportar.GuardarArchivoPdf("Reporte_Movimientos", sb.ToString());
        }

        // ── Generación de Reportes Excel (.xls HTML estructurado) ────────────
        private void ExportarInventarioExcel()
        {
            var productos = repoProducto.ObtenerTodos();
            
            var html = new StringBuilder();
            html.AppendLine("<html xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns=\"http://www.w3.org/TR/REC-html40\">");
            html.AppendLine("<head>");
            html.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            html.AppendLine("<style>");
            html.AppendLine("table { border-collapse: collapse; font-family: Segoe UI, sans-serif; font-size: 10pt; }");
            html.AppendLine("th { background-color: #1e60ff; color: #ffffff; font-weight: bold; border: 1px solid #dcdcdc; padding: 6px; }");
            html.AppendLine("td { border: 1px solid #eef2f9; padding: 6px; }");
            html.AppendLine(".title { font-size: 16pt; font-weight: bold; color: #1e60ff; }");
            html.AppendLine(".meta { color: #808080; font-style: italic; }");
            html.AppendLine(".total { font-weight: bold; background-color: #f5f7fa; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            string empresa = ConfigHelper.Obtener("NombreEmpresa", "StockPap");
            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            html.AppendLine($"<div class=\"title\">REPORTE DE INVENTARIO - {empresa.ToUpper()}</div>");
            html.AppendLine($"<div class=\"meta\">Generado por: {currentUser?.Username ?? "Usuario"} | Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Filtros: Todos</div>");
            html.AppendLine("<br/>");
            
            html.AppendLine("<table>");
            html.AppendLine("<tr>");
            html.AppendLine("<th>ID</th><th>Nombre</th><th>Código Barras</th><th>Clase</th><th>Subclase</th><th>Marca</th><th>Precio Venta</th><th>Stock</th><th>Stock Mín.</th><th>Proveedor</th><th>Estado</th>");
            html.AppendLine("</tr>");
            
            foreach (var p in productos)
            {
                string estado = p.Stock <= p.StockMinimo ? "STOCK BAJO" : "OK";
                string prov = string.IsNullOrEmpty(p.ProveedorNombre) ? "Sin proveedor" : p.ProveedorNombre;
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{p.Id}</td>");
                html.AppendLine($"<td>{p.Nombre}</td>");
                html.AppendLine($"<td>{p.CodigoBarras}</td>");
                html.AppendLine($"<td>{p.Clase}</td>");
                html.AppendLine($"<td>{p.Subclase}</td>");
                html.AppendLine($"<td>{p.Marca}</td>");
                html.AppendLine($"<td>{sym}{p.PrecioVenta:0.00}</td>");
                html.AppendLine($"<td>{p.Stock}</td>");
                html.AppendLine($"<td>{p.StockMinimo}</td>");
                html.AppendLine($"<td>{prov}</td>");
                html.AppendLine($"<td style=\"color:{(estado.Contains("BAJO") ? "#be123c" : "#16a34a")}; font-weight:bold;\">{estado}</td>");
                html.AppendLine("</tr>");
            }
            
            // Fila de totales
            html.AppendLine("<tr class=\"total\">");
            html.AppendLine("<td colspan=\"7\">TOTALES</td>");
            html.AppendLine($"<td>-</td>");
            html.AppendLine($"<td>-</td>");
            html.AppendLine($"<td>{sym}{productos.Sum(p => p.PrecioVenta):0.00} (Prom)</td>");
            html.AppendLine($"<td>{productos.Sum(p => p.Stock)} (Suma)</td>");
            html.AppendLine($"<td>-</td>");
            html.AppendLine($"<td colspan=\"2\">Valor Inventario: {sym}{productos.Sum(p => p.Stock * p.PrecioVenta):0.00}</td>");
            html.AppendLine("</tr>");
            html.AppendLine("</table>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            Exportar.GuardarArchivo("Reporte_Inventario", "Archivo Excel|*.xls", ".xls", html.ToString());
        }

        private void ExportarMovimientosExcel()
        {
            var movimientos = repoMovimiento.ObtenerTodos();
            DateTime inicio = GetFechaInicioFiltro();
            DateTime fin = GetFechaFinFiltro();
            movimientos = movimientos.Where(m => m.Fecha >= inicio && m.Fecha <= fin).ToList();

            var html = new StringBuilder();
            html.AppendLine("<html xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns=\"http://www.w3.org/TR/REC-html40\">");
            html.AppendLine("<head>");
            html.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            html.AppendLine("<style>");
            html.AppendLine("table { border-collapse: collapse; font-family: Segoe UI, sans-serif; font-size: 10pt; }");
            html.AppendLine("th { background-color: #7c3aed; color: #ffffff; font-weight: bold; border: 1px solid #dcdcdc; padding: 6px; }");
            html.AppendLine("td { border: 1px solid #eef2f9; padding: 6px; }");
            html.AppendLine(".title { font-size: 16pt; font-weight: bold; color: #7c3aed; }");
            html.AppendLine(".meta { color: #808080; font-style: italic; }");
            html.AppendLine(".total { font-weight: bold; background-color: #f5f7fa; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            string empresa = ConfigHelper.Obtener("NombreEmpresa", "StockPap");
            html.AppendLine($"<div class=\"title\">REPORTE DE MOVIMIENTOS - {empresa.ToUpper()}</div>");
            html.AppendLine($"<div class=\"meta\">Generado por: {currentUser?.Username ?? "Usuario"} | Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Rango: {cmbRangoFecha.Text} ({inicio:dd/MM/yyyy} - {fin:dd/MM/yyyy})</div>");
            html.AppendLine("<br/>");

            html.AppendLine("<table>");
            html.AppendLine("<tr>");
            html.AppendLine("<th>ID</th><th>Fecha y Hora</th><th>Tipo</th><th>ID Producto</th><th>Producto</th><th>Cantidad</th><th>Stock Anterior</th><th>Stock Nuevo</th><th>Motivo</th>");
            html.AppendLine("</tr>");

            foreach (var m in movimientos)
            {
                string color = m.Tipo == "Entrada" ? "#27ae60" : (m.Tipo == "Salida" ? "#c0392b" : "#7f8c8d");
                string prefijo = m.Tipo == "Entrada" ? "+" : (m.Tipo == "Salida" ? "-" : "");
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{m.Id}</td>");
                html.AppendLine($"<td>{m.Fecha:dd/MM/yyyy HH:mm}</td>");
                html.AppendLine($"<td style=\"color:{color}; font-weight:bold;\">{m.Tipo}</td>");
                html.AppendLine($"<td>{m.ProductoId}</td>");
                html.AppendLine($"<td>{m.ProductoNombre}</td>");
                html.AppendLine($"<td style=\"color:{color}; font-weight:bold;\">{prefijo}{Math.Abs(m.Cantidad)}</td>");
                html.AppendLine($"<td>{m.StockAnterior}</td>");
                html.AppendLine($"<td>{m.StockNuevo}</td>");
                html.AppendLine($"<td>{m.Motivo}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("<tr class=\"total\">");
            html.AppendLine("<td colspan=\"5\">TOTALES EN EL RANGO</td>");
            html.AppendLine($"<td>Entradas: +{movimientos.Where(m => m.Tipo == "Entrada").Sum(m => Math.Abs(m.Cantidad))} | Salidas: -{movimientos.Where(m => m.Tipo == "Salida").Sum(m => Math.Abs(m.Cantidad))}</td>");
            html.AppendLine("<td colspan=\"3\"></td>");
            html.AppendLine("</tr>");
            html.AppendLine("</table>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            Exportar.GuardarArchivo("Reporte_Movimientos", "Archivo Excel|*.xls", ".xls", html.ToString());
        }

        private void ExportarTodoJuntoPdf()
        {
            var productos = repoProducto.ObtenerTodos();
            var movimientos = repoMovimiento.ObtenerTodos();
            DateTime inicio = GetFechaInicioFiltro();
            DateTime fin = GetFechaFinFiltro();
            movimientos = movimientos.Where(m => m.Fecha >= inicio && m.Fecha <= fin).ToList();

            string empresa = ConfigHelper.Obtener("NombreEmpresa", "StockPap");
            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            var sb = new StringBuilder();
            sb.AppendLine("==========================================================================");
            sb.AppendLine($"                 REPORTE CONSOLIDADO DE INVENTARIO Y MOVIMIENTOS - {empresa.ToUpper()}");
            sb.AppendLine("==========================================================================");
            sb.AppendLine($"Generado por: {currentUser?.Username ?? "Usuario"}");
            sb.AppendLine($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Filtro Temporal de Movimientos: {cmbRangoFecha.Text}");
            sb.AppendLine($"Rango de fechas: {inicio:dd/MM/yyyy} - {fin:dd/MM/yyyy}");
            sb.AppendLine("--------------------------------------------------------------------------");
            sb.AppendLine($"Total Productos Diferentes: {productos.Count}");
            sb.AppendLine($"Total de Hojas/Piezas en Stock: {productos.Sum(p => p.Stock)}");
            sb.AppendLine($"Valor del Inventario (Venta): {sym}{productos.Sum(p => p.Stock * p.PrecioVenta):0.00}");
            sb.AppendLine($"Total Movimientos en Rango: {movimientos.Count}");
            sb.AppendLine("==========================================================================");
            sb.AppendLine();
            sb.AppendLine("1. DETALLE DE INVENTARIO");
            sb.AppendLine("--------------------------------------------------------------------------");
            sb.AppendLine("PRODUCTO | CLASE | MARCA | PRECIO VENTA | STOCK | STOCK MÍN | ESTADO");
            sb.AppendLine("--------------------------------------------------------------------------");
            foreach (var p in productos.OrderBy(x => x.Nombre))
            {
                string estado = p.Stock <= p.StockMinimo ? "Bajo" : "OK";
                string clase = string.IsNullOrEmpty(p.Clase) ? "N/A" : p.Clase;
                string marca = string.IsNullOrEmpty(p.Marca) ? "N/A" : p.Marca;
                sb.AppendLine($"{p.Nombre} | {clase} | {marca} | {sym}{p.PrecioVenta:0.00} | {p.Stock} | {p.StockMinimo} | {estado}");
            }
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("2. DETALLE DE MOVIMIENTOS");
            sb.AppendLine("--------------------------------------------------------------------------");
            sb.AppendLine("FECHA Y HORA | TIPO | PRODUCTO | CANTIDAD | STOCK ANT | STOCK NUEVO | MOTIVO");
            sb.AppendLine("--------------------------------------------------------------------------");
            foreach (var m in movimientos)
            {
                string signo = m.Tipo == "Entrada" ? "+" : "-";
                sb.AppendLine($"{m.Fecha:dd/MM/yyyy HH:mm} | {m.Tipo} | {m.ProductoNombre} | {signo}{Math.Abs(m.Cantidad)} | {m.StockAnterior} | {m.StockNuevo} | {CrearDetalleMovimiento(m)}");
            }

            Exportar.GuardarArchivoPdf("Reporte_Consolidado", sb.ToString());
        }
    }
}
