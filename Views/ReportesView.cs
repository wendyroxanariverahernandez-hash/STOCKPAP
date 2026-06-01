using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using STOCKPAP.Models;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class ReportesView : UserControl
    {
        private readonly ProductoRepository repoProducto;
        private readonly ProveedorRepository repoProveedor;
        private readonly MovimientoRepository repoMovimiento;
        private readonly bool puedeEditar;

        private DataGridView dgvProductos;
        private DataGridView dgvMovimientos;
        private Label lblSubtitle;
        private Label lblTotalProd;
        private Label lblTotalUnidades;
        private Label lblValorInv;
        private Label lblStockBajo;

        public ReportesView(bool puedeEditar = true)
        {
            this.puedeEditar = puedeEditar;
            repoProducto = new ProductoRepository();
            repoProveedor = new ProveedorRepository();
            repoMovimiento = new MovimientoRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            Padding = new Padding(30);

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
                Text = "Consulta inventario, movimientos y descargas por separado",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(35, 68)
            };
            Controls.Add(lblSubtitle);

            FlowLayoutPanel accionesPanel = new FlowLayoutPanel
            {
                Size = new Size(790, 90),
                Location = new Point(250, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };
            Controls.Add(accionesPanel);

            RoundedButton btnNuevoMovimiento = CrearBoton("Nuevo Movimiento", Color.FromArgb(30, 96, 255));
            btnNuevoMovimiento.Visible = puedeEditar;
            btnNuevoMovimiento.Click += BtnNuevoMovimiento_Click;
            accionesPanel.Controls.Add(btnNuevoMovimiento);

            RoundedButton btnInventarioPdf = CrearBoton("Inventario PDF", Color.FromArgb(100, 116, 139));
            btnInventarioPdf.Click += (s, e) => ExportarInventarioPdf();
            accionesPanel.Controls.Add(btnInventarioPdf);

            RoundedButton btnMovimientosPdf = CrearBoton("Movimientos PDF", Color.FromArgb(124, 58, 237));
            btnMovimientosPdf.Click += (s, e) => ExportarMovimientosPdf();
            accionesPanel.Controls.Add(btnMovimientosPdf);

            RoundedButton btnTicketGlobal = CrearBoton("Ticket Global PDF", Color.FromArgb(234, 88, 12));
            btnTicketGlobal.Click += (s, e) => ExportarTicketGlobalPdf();
            accionesPanel.Controls.Add(btnTicketGlobal);

            lblTotalProd = CrearTarjetaStat("Total Productos", "...", "productos", Color.FromArgb(30, 96, 255), 30, 110);
            lblTotalUnidades = CrearTarjetaStat("Unidades Totales", "...", "en inventario", Color.FromArgb(22, 163, 74), 225, 110);
            lblValorInv = CrearTarjetaStat("Valor Inventario", "...", "valor estimado", Color.FromArgb(234, 88, 12), 420, 110);
            lblStockBajo = CrearTarjetaStat("Stock Bajo", "...", "necesitan atencion", Color.FromArgb(190, 18, 60), 615, 110);

            RoundedPanel panelInventario = CrearPanelTabla("Detalle del Inventario", 30, 270, 500, 430);
            dgvProductos = CrearGrid();
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "Producto", FillWeight = 28 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCategoria", HeaderText = "Categoria", FillWeight = 16 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVenta", HeaderText = "Precio Venta", FillWeight = 14 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStock", HeaderText = "Stock", FillWeight = 10 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMinimo", HeaderText = "Minimo", FillWeight = 10 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colValor", HeaderText = "Valor", FillWeight = 14 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "Estado", FillWeight = 14 });
            dgvProductos.CellFormatting += DgvProductos_CellFormatting;
            panelInventario.Controls.Add(dgvProductos);
            Controls.Add(panelInventario);

            RoundedPanel panelMovimientos = CrearPanelTabla("Historial de Movimientos", 550, 270, 500, 430);
            dgvMovimientos = CrearGrid();
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Fecha", FillWeight = 18 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTipo", HeaderText = "Tipo", FillWeight = 12 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colProducto", HeaderText = "Producto", FillWeight = 28 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCantidad", HeaderText = "Cant.", FillWeight = 10 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStock", HeaderText = "Stock", FillWeight = 18 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetalle", HeaderText = "Detalle", FillWeight = 34 });
            panelMovimientos.Controls.Add(dgvMovimientos);
            Controls.Add(panelMovimientos);
        }

        private RoundedButton CrearBoton(string texto, Color color)
        {
            return new RoundedButton
            {
                Text = texto,
                Size = new Size(155, 40),
                Margin = new Padding(0, 0, 10, 8),
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 10,
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
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
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
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            grid.ColumnHeadersHeight = 34;
            grid.RowTemplate.Height = 32;
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 235, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(20, 20, 40);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 255);
            return grid;
        }

        private void LoadData()
        {
            var productos = repoProducto.ObtenerTodos();
            var movimientos = repoMovimiento.ObtenerTodos();

            lblTotalProd.Text = productos.Count.ToString();
            lblTotalUnidades.Text = productos.Sum(p => p.Stock).ToString();
            lblValorInv.Text = $"${productos.Sum(p => p.Stock * p.PrecioVenta):0.00}";
            lblStockBajo.Text = productos.Count(p => p.Stock <= p.StockMinimo).ToString();
            lblSubtitle.Text = $"Ultima actualizacion: {DateTime.Now:dd/MM/yyyy HH:mm}  |  {movimientos.Count} movimientos";

            dgvProductos.Rows.Clear();
            foreach (var p in productos)
            {
                bool bajo = p.Stock <= p.StockMinimo;
                dgvProductos.Rows.Add(p.Nombre, p.Categoria, $"${p.PrecioVenta:0.00}", p.Stock, p.StockMinimo, $"${p.Stock * p.PrecioVenta:0.00}", bajo ? "Stock Bajo" : "OK");
            }

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

            return $"{movimiento.Tipo} de {Math.Abs(movimiento.Cantidad)} unidad(es). Stock {movimiento.StockAnterior} a {movimiento.StockNuevo}.";
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

        private void ExportarInventarioCsv()
        {
            Exportar.GuardarArchivo("Inventario_STOCKPAP", "Archivo CSV|*.csv", ".csv", CrearInventarioCsv());
        }

        private void ExportarInventarioPdf()
        {
            Exportar.GuardarArchivoPdf("Inventario_STOCKPAP", CrearInventarioTxt());
        }

        private void ExportarMovimientosPdf()
        {
            Exportar.GuardarArchivoPdf("Movimientos_STOCKPAP", CrearMovimientosTxt());
        }

        private void ExportarTicketGlobalPdf()
        {
            Exportar.GuardarArchivoPdf("Ticket_Global_STOCKPAP", CrearTicketGlobalTxt());
        }

        private string CrearInventarioCsv()
        {
            var productos = repoProducto.ObtenerTodos();
            var sb = new StringBuilder();
            sb.AppendLine("\"Producto\",\"Categoria\",\"Precio Compra\",\"Precio Venta\",\"Stock\",\"Stock Minimo\",\"Valor Total\",\"Estado\"");
            foreach (var p in productos)
            {
                bool bajo = p.Stock <= p.StockMinimo;
                sb.AppendLine($"\"{EscapeCsv(p.Nombre)}\",\"{EscapeCsv(p.Categoria)}\",{p.PrecioCompra:0.00},{p.PrecioVenta:0.00},{p.Stock},{p.StockMinimo},{p.Stock * p.PrecioVenta:0.00},\"{(bajo ? "Stock Bajo" : "OK")}\"");
            }
            return sb.ToString();
        }

        private string CrearInventarioTxt()
        {
            var productos = repoProducto.ObtenerTodos();
            var proveedores = repoProveedor.ObtenerTodos();
            var sb = new StringBuilder();
            sb.AppendLine("REPORTE DE INVENTARIO - STOCKPAP");
            sb.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine(new string('=', 60));
            sb.AppendLine($"Total productos: {productos.Count}");
            sb.AppendLine($"Unidades en stock: {productos.Sum(p => p.Stock)}");
            sb.AppendLine($"Valor inventario: ${productos.Sum(p => p.Stock * p.PrecioVenta):0.00}");
            sb.AppendLine($"Stock bajo: {productos.Count(p => p.Stock <= p.StockMinimo)}");
            sb.AppendLine($"Proveedores: {proveedores.Count}");
            sb.AppendLine();
            foreach (var p in productos.OrderBy(p => p.Nombre))
            {
                string estado = p.Stock <= p.StockMinimo ? "Stock Bajo" : "OK";
                sb.AppendLine($"{p.Nombre} | {p.Categoria} | Venta ${p.PrecioVenta:0.00} | Stock {p.Stock} | {estado}");
            }
            return sb.ToString();
        }

        private string CrearMovimientosCsv()
        {
            var movimientos = repoMovimiento.ObtenerTodos();
            var sb = new StringBuilder();
            sb.AppendLine("\"Fecha\",\"Tipo\",\"Producto\",\"Cantidad\",\"Stock Anterior\",\"Stock Nuevo\",\"Motivo\"");
            foreach (var m in movimientos)
                sb.AppendLine($"\"{m.Fecha:dd/MM/yyyy HH:mm}\",\"{EscapeCsv(m.Tipo)}\",\"{EscapeCsv(m.ProductoNombre)}\",{m.Cantidad},{m.StockAnterior},{m.StockNuevo},\"{EscapeCsv(CrearDetalleMovimiento(m))}\"");
            return sb.ToString();
        }

        private string CrearMovimientosTxt()
        {
            var movimientos = repoMovimiento.ObtenerTodos();
            var sb = new StringBuilder();
            sb.AppendLine("REPORTE DE MOVIMIENTOS - STOCKPAP");
            sb.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine(new string('=', 60));
            foreach (var m in movimientos)
                sb.AppendLine($"{m.Fecha:dd/MM/yyyy HH:mm} | {m.Tipo} | {m.ProductoNombre} | Cantidad {m.Cantidad} | Stock {m.StockAnterior}->{m.StockNuevo} | {CrearDetalleMovimiento(m)}");
            return sb.ToString();
        }

        private string CrearTicketGlobalTxt()
        {
            var sb = new StringBuilder();
            sb.AppendLine("============================================================");
            sb.AppendLine("                TICKET GLOBAL - STOCKPAP");
            sb.AppendLine("============================================================");
            sb.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine();
            sb.AppendLine("--- RESUMEN DE INVENTARIO ---");
            sb.Append(CrearInventarioTxt().Replace("REPORTE DE INVENTARIO - STOCKPAP\r\n", "").Replace($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\r\n", "").Replace(new string('=', 60) + "\r\n", ""));
            sb.AppendLine();
            sb.AppendLine("--- HISTORIAL DE MOVIMIENTOS ---");
            sb.Append(CrearMovimientosTxt().Replace("REPORTE DE MOVIMIENTOS - STOCKPAP\r\n", "").Replace($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\r\n", "").Replace(new string('=', 60) + "\r\n", ""));
            sb.AppendLine("============================================================");
            return sb.ToString();
        }

        private static string EscapeCsv(string s)
        {
            return (s ?? "").Replace("\"", "\"\"");
        }
    }
}
