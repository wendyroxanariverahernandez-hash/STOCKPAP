using System;
using System.Collections.Generic;
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
        private ProductoRepository repoProducto;
        private ProveedorRepository repoProveedor;
        private DataGridView dgvProductos;
        private Label lblSubtitle;

        // Tarjetas de stats
        private Label lblTotalProd;
        private Label lblTotalUnidades;
        private Label lblValorInv;
        private Label lblStockBajo;

        public ReportesView()
        {
            repoProducto  = new ProductoRepository();
            repoProveedor = new ProveedorRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            // ── Encabezado ──────────────────────────────────────────────────
            Label lblTitle = new Label
            {
                Text = "Reportes",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 40),
                AutoSize = true,
                Location = new Point(30, 30)
            };
            this.Controls.Add(lblTitle);

            lblSubtitle = new Label
            {
                Text = "Análisis y métricas del inventario",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(35, 72)
            };
            this.Controls.Add(lblSubtitle);

            // ── Botones de exportación ───────────────────────────────────────
            RoundedButton btnCSV = new RoundedButton
            {
                Text = "⬇  Descargar CSV",
                Size = new Size(165, 40),
                Location = new Point(this.Width - 390, 38),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnCSV.Click += (s, e) => ExportarCSV();
            this.Controls.Add(btnCSV);

            RoundedButton btnTXT = new RoundedButton
            {
                Text = "📄  Descargar TXT",
                Size = new Size(165, 40),
                Location = new Point(this.Width - 215, 38),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnTXT.Click += (s, e) => ExportarTXT();
            this.Controls.Add(btnTXT);

            // ── Tarjetas de estadísticas ─────────────────────────────────────
            lblTotalProd     = CrearTarjetaStat("📦 Total Productos",   "...", "productos",   Color.FromArgb(239, 246, 255), Color.FromArgb(30, 96, 255),  30,  110);
            lblTotalUnidades = CrearTarjetaStat("📊 Unidades Totales",  "...", "en inventario",Color.FromArgb(240, 253, 244), Color.FromArgb(22, 163, 74),  225, 110);
            lblValorInv      = CrearTarjetaStat("💰 Valor Inventario",  "...", "valor estimado",Color.FromArgb(255, 247, 237), Color.FromArgb(234, 88, 12),  420, 110);
            lblStockBajo     = CrearTarjetaStat("⚠  Stock Bajo",        "...", "necesitan atención",Color.FromArgb(255, 241, 242), Color.FromArgb(190, 18, 60), 615, 110);

            // ── Tabla de inventario ──────────────────────────────────────────
            RoundedPanel panelTabla = new RoundedPanel
            {
                Location = new Point(30, 270),
                Size = new Size(840, 450),
                BackColor = Color.White,
                BorderRadius = 14,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelTabla);

            Label lblTablaTitle = new Label
            {
                Text = "📋  Detalle del Inventario",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 18),
                ForeColor = Color.FromArgb(20, 20, 40)
            };
            panelTabla.Controls.Add(lblTablaTitle);

            dgvProductos = new DataGridView
            {
                Location = new Point(10, 52),
                Size = new Size(820, 385),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
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
                Font = new Font("Segoe UI", 10),
                GridColor = Color.FromArgb(235, 238, 245)
            };
            dgvProductos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 80);
            dgvProductos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvProductos.ColumnHeadersHeight = 40;
            dgvProductos.RowTemplate.Height = 38;
            dgvProductos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 235, 255);
            dgvProductos.DefaultCellStyle.SelectionForeColor = Color.FromArgb(20, 20, 40);
            dgvProductos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 255);

            // Columnas
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre",    HeaderText = "Producto",       FillWeight = 25 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCategoria", HeaderText = "Categoría",      FillWeight = 15 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCompra",    HeaderText = "Precio Compra",  FillWeight = 15 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVenta",     HeaderText = "Precio Venta",   FillWeight = 15 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStock",     HeaderText = "Stock",          FillWeight = 10 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMinimo",    HeaderText = "Stock Mínimo",   FillWeight = 12 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colValor",     HeaderText = "Valor Total",    FillWeight = 18 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado",    HeaderText = "Estado",         FillWeight = 12 });

            dgvProductos.CellFormatting += DgvProductos_CellFormatting;
            panelTabla.Controls.Add(dgvProductos);
        }

        // ── Tarjeta estadística ──────────────────────────────────────────────
        private Label CrearTarjetaStat(string titulo, string valor, string subtitulo,
                                       Color bgColor, Color accentColor, int x, int y)
        {
            RoundedPanel card = new RoundedPanel
            {
                Size = new Size(180, 140),
                Location = new Point(x, y),
                BackColor = Color.White,
                BorderRadius = 14,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            // Barra de color superior
            Panel barraColor = new Panel
            {
                Dock = DockStyle.Top,
                Height = 5,
                BackColor = accentColor
            };
            card.Controls.Add(barraColor);

            Label lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(14, 18)
            };
            card.Controls.Add(lblTitulo);

            Label lblValor = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = accentColor,
                AutoSize = true,
                Location = new Point(14, 45)
            };
            card.Controls.Add(lblValor);

            Label lblSub = new Label
            {
                Text = subtitulo,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                AutoSize = false,
                Size = new Size(152, 30),
                Location = new Point(14, 100)
            };
            card.Controls.Add(lblSub);

            this.Controls.Add(card);
            return lblValor; // retornamos la label del valor para actualizarla en LoadData
        }

        // ── Carga de datos ───────────────────────────────────────────────────
        private void LoadData()
        {
            var productos = repoProducto.ObtenerTodos();

            int totalProd    = productos.Count;
            int totalUnits   = productos.Sum(p => p.Stock);
            decimal valor    = productos.Sum(p => p.Stock * p.PrecioVenta);
            int stockBajo    = productos.Count(p => p.Stock <= p.StockMinimo);

            // Actualizar tarjetas
            lblTotalProd.Text     = totalProd.ToString();
            lblTotalUnidades.Text = totalUnits.ToString();
            lblValorInv.Text      = $"${valor:0.00}";
            lblStockBajo.Text     = stockBajo.ToString();
            lblSubtitle.Text      = $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm}  •  {totalProd} productos";

            // Cargar tabla
            dgvProductos.Rows.Clear();
            foreach (var p in productos)
            {
                bool bajo = p.Stock <= p.StockMinimo;
                dgvProductos.Rows.Add(
                    p.Nombre,
                    p.Categoria,
                    $"${p.PrecioCompra:0.00}",
                    $"${p.PrecioVenta:0.00}",
                    p.Stock,
                    p.StockMinimo,
                    $"${p.Stock * p.PrecioVenta:0.00}",
                    bajo ? "⚠ Stock Bajo" : "✓ OK"
                );
            }
        }

        // Coloreado de la columna Estado
        private void DgvProductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvProductos.Columns["colEstado"].Index && e.Value != null)
            {
                string val = e.Value.ToString();
                if (val.Contains("Bajo"))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(190, 18, 60);
                    e.CellStyle.BackColor = Color.FromArgb(255, 241, 242);
                    e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(22, 163, 74);
                    e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }
            }
        }

        // ── Exportar CSV ─────────────────────────────────────────────────────
        private void ExportarCSV()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Guardar reporte CSV";
                sfd.Filter = "Archivo CSV|*.csv";
                sfd.FileName = $"Inventario_STOCKPAP_{DateTime.Now:yyyyMMdd_HHmm}.csv";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var productos = repoProducto.ObtenerTodos();
                    var sb = new StringBuilder();

                    // Encabezado
                    sb.AppendLine("\"Producto\",\"Categoría\",\"Precio Compra\",\"Precio Venta\",\"Stock\",\"Stock Mínimo\",\"Valor Total\",\"Estado\"");

                    foreach (var p in productos)
                    {
                        bool bajo = p.Stock <= p.StockMinimo;
                        sb.AppendLine(
                            $"\"{EscapeCsv(p.Nombre)}\"," +
                            $"\"{EscapeCsv(p.Categoria)}\"," +
                            $"{p.PrecioCompra:0.00}," +
                            $"{p.PrecioVenta:0.00}," +
                            $"{p.Stock}," +
                            $"{p.StockMinimo}," +
                            $"{p.Stock * p.PrecioVenta:0.00}," +
                            $"\"{(bajo ? "Stock Bajo" : "OK")}\""
                        );
                    }

                    // Totales al final
                    sb.AppendLine();
                    sb.AppendLine($"\"TOTAL\",\"\",\"\",\"\"," +
                                  $"{productos.Sum(p => p.Stock)},\"\"," +
                                  $"{productos.Sum(p => p.Stock * p.PrecioVenta):0.00},\"\"");

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);

                    var res = MessageBox.Show(
                        $"Reporte exportado exitosamente.\n\nUbicación:\n{sfd.FileName}\n\n¿Deseas abrir el archivo?",
                        "Exportación Exitosa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (res == DialogResult.Yes)
                        System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ── Exportar TXT (Reporte Detallado) ─────────────────────────────────
        private void ExportarTXT()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Guardar reporte de texto";
                sfd.Filter = "Archivo de texto|*.txt";
                sfd.FileName = $"Reporte_STOCKPAP_{DateTime.Now:yyyyMMdd_HHmm}.txt";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var productos  = repoProducto.ObtenerTodos();
                    var proveedores = repoProveedor.ObtenerTodos();
                    var sb = new StringBuilder();
                    string linea = new string('═', 60);
                    string lineaFina = new string('─', 60);

                    sb.AppendLine(linea);
                    sb.AppendLine("         REPORTE DE INVENTARIO - STOCKPAP");
                    sb.AppendLine($"         Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                    sb.AppendLine(linea);
                    sb.AppendLine();

                    // Resumen
                    sb.AppendLine("  RESUMEN GENERAL");
                    sb.AppendLine(lineaFina);
                    sb.AppendLine($"  Total de productos   : {productos.Count}");
                    sb.AppendLine($"  Unidades en stock    : {productos.Sum(p => p.Stock)}");
                    sb.AppendLine($"  Valor del inventario : ${productos.Sum(p => p.Stock * p.PrecioVenta):0.00}");
                    sb.AppendLine($"  Productos stock bajo : {productos.Count(p => p.Stock <= p.StockMinimo)}");
                    sb.AppendLine($"  Proveedores          : {proveedores.Count}");
                    sb.AppendLine();

                    // Detalle de productos
                    sb.AppendLine("  DETALLE DE PRODUCTOS");
                    sb.AppendLine(lineaFina);
                    sb.AppendLine($"  {"Producto",-28} {"Cat",-14} {"P.Venta",9} {"Stock",7} {"Valor",12} {"Estado",-12}");
                    sb.AppendLine(lineaFina);

                    foreach (var p in productos.OrderBy(x => x.Nombre))
                    {
                        bool bajo = p.Stock <= p.StockMinimo;
                        string nombre = p.Nombre.Length > 27 ? p.Nombre.Substring(0, 24) + "..." : p.Nombre;
                        string cat    = (p.Categoria ?? "").Length > 13 ? (p.Categoria ?? "").Substring(0, 10) + "..." : (p.Categoria ?? "");
                        sb.AppendLine(
                            $"  {nombre,-28} {cat,-14} ${p.PrecioVenta,8:0.00} {p.Stock,7} ${p.Stock * p.PrecioVenta,11:0.00} {(bajo ? "!! BAJO" : "OK"),-12}");
                    }
                    sb.AppendLine(lineaFina);
                    sb.AppendLine($"  {"TOTAL",-28} {"",14} {"",10} {productos.Sum(p=>p.Stock),7} ${productos.Sum(p=>p.Stock*p.PrecioVenta),11:0.00}");
                    sb.AppendLine();

                    // Alertas
                    var alertas = productos.Where(p => p.Stock <= p.StockMinimo).ToList();
                    if (alertas.Count > 0)
                    {
                        sb.AppendLine("  !! ALERTAS DE STOCK BAJO");
                        sb.AppendLine(lineaFina);
                        foreach (var a in alertas)
                            sb.AppendLine($"  - {a.Nombre}: {a.Stock} uds (mínimo: {a.StockMinimo})");
                        sb.AppendLine();
                    }

                    // Proveedores
                    sb.AppendLine("  PROVEEDORES REGISTRADOS");
                    sb.AppendLine(lineaFina);
                    foreach (var pv in proveedores)
                    {
                        sb.AppendLine($"  • {pv.Empresa}");
                        if (!string.IsNullOrEmpty(pv.Contacto))  sb.AppendLine($"      Contacto : {pv.Contacto}");
                        if (!string.IsNullOrEmpty(pv.Telefono))  sb.AppendLine($"      Teléfono : {pv.Telefono}");
                        if (!string.IsNullOrEmpty(pv.Email))     sb.AppendLine($"      Email    : {pv.Email}");
                    }
                    sb.AppendLine();
                    sb.AppendLine(linea);
                    sb.AppendLine("         Fin del reporte - STOCKPAP");
                    sb.AppendLine(linea);

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);

                    var res = MessageBox.Show(
                        $"Reporte TXT generado exitosamente.\n\nUbicación:\n{sfd.FileName}\n\n¿Deseas abrir el archivo?",
                        "Exportación Exitosa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (res == DialogResult.Yes)
                        System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string EscapeCsv(string s)
            => (s ?? "").Replace("\"", "\"\"");
    }
}
