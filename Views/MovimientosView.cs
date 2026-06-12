using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using STOCKPAP.DataAccess;
using STOCKPAP.Models;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class MovimientosView : UserControl
    {
        private DataGridView dgvMovimientos;
        private Label lblEntradas;
        private Label lblSalidas;
        private Label lblTotalOps;
        private MovimientoRepository repo;
        private List<Movimiento> currentMovimientos;
        private bool puedeEditar;

        // Sorting configuration
        private string sortColumn = "colFecha";
        private bool sortAscending = false; // Descending by default for latest date

        public MovimientosView(bool puedeEditar = true)
        {
            this.puedeEditar = puedeEditar;
            repo = new MovimientoRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            // ── Título ──────────────────────────────────────────────────────
            Label lblTitle = new Label 
            { 
                Text = "Movimientos de Inventario", 
                Font = new Font("Segoe UI", 24, FontStyle.Bold), 
                AutoSize = true, 
                Location = new Point(30, 30) 
            };
            Label lblSubtitle = new Label 
            { 
                Text = "Registra y consulta entradas, salidas y ajustes del catálogo", 
                Font = new Font("Segoe UI", 11), 
                ForeColor = Color.Gray, 
                AutoSize = true, 
                Location = new Point(35, 75) 
            };
            this.Controls.Add(lblTitle); 
            this.Controls.Add(lblSubtitle);

            // Botón nuevo movimiento
            RoundedButton btnNuevo = new RoundedButton
            {
                Text = "+  Nuevo Movimiento",
                Size = new Size(180, 45),
                Location = new Point(this.Width - 210, 30),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnNuevo.Visible = puedeEditar;
            btnNuevo.Click += BtnNuevo_Click;
            this.Controls.Add(btnNuevo);

            // ── Tarjetas Estadísticas ───────────────────────────────────────
            lblEntradas = CreateStatCard("Total Entradas", "0", "unidades agregadas", Color.FromArgb(39, 174, 96), 30, 120);
            lblSalidas = CreateStatCard("Total Salidas", "0", "unidades retiradas", Color.FromArgb(192, 57, 43), 280, 120);
            lblTotalOps = CreateStatCard("Movimientos Totales", "0", "operaciones registradas", Color.FromArgb(41, 128, 185), 530, 120);

            // ── Contenedor de Tabla/Grid ─────────────────────────────────────
            RoundedPanel panelLista = new RoundedPanel
            {
                Location = new Point(30, 260),
                Size = new Size(this.Width - 60, this.Height - 290),
                BackColor = Color.White,
                BorderRadius = 15,
                Padding = new Padding(15, 60, 15, 15),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelLista);

            Label lblListTitle = new Label 
            { 
                Text = "Historial de Movimientos (Haz clic en las cabeceras para ordenar)", 
                Font = new Font("Segoe UI", 12, FontStyle.Bold), 
                ForeColor = Color.FromArgb(20, 20, 40),
                AutoSize = true, 
                Location = new Point(20, 20) 
            };
            panelLista.Controls.Add(lblListTitle);

            // DataGridView con scrollbar horizontal
            dgvMovimientos = new DataGridView
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
            dgvMovimientos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvMovimientos.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 80);
            dgvMovimientos.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 247, 250);
            dgvMovimientos.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(60, 60, 80);
            dgvMovimientos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvMovimientos.ColumnHeadersHeight = 38;
            dgvMovimientos.RowTemplate.Height = 34;
            dgvMovimientos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 235, 255);
            dgvMovimientos.DefaultCellStyle.SelectionForeColor = Color.FromArgb(20, 20, 40);
            dgvMovimientos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 255);

            // Columnas manuales con tamaño fijo
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Fecha y Hora", Width = 145 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTipo", HeaderText = "Tipo", Width = 95 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colProducto", HeaderText = "Producto", Width = 230 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCantidad", HeaderText = "Cantidad", Width = 85 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAnterior", HeaderText = "Stock Ant.", Width = 95 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNuevo", HeaderText = "Stock Nuevo", Width = 95 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMotivo", HeaderText = "Motivo / Comentario", Width = 250 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID Mov.", Width = 80 });
            dgvMovimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colProductoId", HeaderText = "ID Prod.", Width = 80 });

            // Formatear celdas (colores para Entrada/Salida)
            dgvMovimientos.CellFormatting += DgvMovimientos_CellFormatting;

            // Manejar ordenación manual
            dgvMovimientos.ColumnHeaderMouseClick += DgvMovimientos_ColumnHeaderMouseClick;

            panelLista.Controls.Add(dgvMovimientos);
        }

        private Label CreateStatCard(string title, string mainVal, string subVal, Color accent, int x, int y)
        {
            RoundedPanel card = new RoundedPanel 
            { 
                Size = new Size(230, 120), 
                Location = new Point(x, y), 
                BackColor = Color.White, 
                BorderRadius = 15 
            };
            card.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 5, BackColor = accent });
            card.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(20, 18) });
            
            Label lblVal = new Label { Text = mainVal, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.Black, AutoSize = true, Location = new Point(20, 45) };
            card.Controls.Add(lblVal);

            card.Controls.Add(new Label { Text = subVal, Font = new Font("Segoe UI", 8.5f), ForeColor = Color.Gray, AutoSize = true, Location = new Point(20, 90) });
            this.Controls.Add(card);
            return lblVal;
        }

        private void LoadData()
        {
            currentMovimientos = repo.ObtenerTodos();

            // Calcular Estadísticas
            int entradas = currentMovimientos.Where(m => m.Tipo == "Entrada").Sum(m => Math.Abs(m.Cantidad));
            int salidas = currentMovimientos.Where(m => m.Tipo == "Salida").Sum(m => Math.Abs(m.Cantidad));
            
            lblEntradas.Text = $"+{entradas}";
            lblSalidas.Text = $"-{salidas}";
            lblTotalOps.Text = currentMovimientos.Count.ToString();

            SortData();
            RenderGrid();
        }

        private void SortData()
        {
            if (currentMovimientos == null) return;

            switch (sortColumn)
            {
                case "colFecha":
                    currentMovimientos = sortAscending 
                        ? currentMovimientos.OrderBy(m => m.Fecha).ToList() 
                        : currentMovimientos.OrderByDescending(m => m.Fecha).ToList();
                    break;
                case "colTipo":
                    currentMovimientos = sortAscending 
                        ? currentMovimientos.OrderBy(m => m.Tipo).ToList() 
                        : currentMovimientos.OrderByDescending(m => m.Tipo).ToList();
                    break;
                case "colProducto":
                    currentMovimientos = sortAscending 
                        ? currentMovimientos.OrderBy(m => m.ProductoNombre).ToList() 
                        : currentMovimientos.OrderByDescending(m => m.ProductoNombre).ToList();
                    break;
                case "colCantidad":
                    currentMovimientos = sortAscending 
                        ? currentMovimientos.OrderBy(m => Math.Abs(m.Cantidad)).ToList() 
                        : currentMovimientos.OrderByDescending(m => Math.Abs(m.Cantidad)).ToList();
                    break;
                case "colAnterior":
                    currentMovimientos = sortAscending 
                        ? currentMovimientos.OrderBy(m => m.StockAnterior).ToList() 
                        : currentMovimientos.OrderByDescending(m => m.StockAnterior).ToList();
                    break;
                case "colNuevo":
                    currentMovimientos = sortAscending 
                        ? currentMovimientos.OrderBy(m => m.StockNuevo).ToList() 
                        : currentMovimientos.OrderByDescending(m => m.StockNuevo).ToList();
                    break;
                case "colMotivo":
                    currentMovimientos = sortAscending 
                        ? currentMovimientos.OrderBy(m => m.Motivo).ToList() 
                        : currentMovimientos.OrderByDescending(m => m.Motivo).ToList();
                    break;
                case "colId":
                    currentMovimientos = sortAscending 
                        ? currentMovimientos.OrderBy(m => m.Id).ToList() 
                        : currentMovimientos.OrderByDescending(m => m.Id).ToList();
                    break;
                case "colProductoId":
                    currentMovimientos = sortAscending 
                        ? currentMovimientos.OrderBy(m => m.ProductoId).ToList() 
                        : currentMovimientos.OrderByDescending(m => m.ProductoId).ToList();
                    break;
            }
        }

        private void RenderGrid()
        {
            dgvMovimientos.Rows.Clear();
            if (currentMovimientos == null) return;

            foreach (var m in currentMovimientos)
            {
                string prefijo = m.Tipo == "Entrada" ? "+" : (m.Tipo == "Salida" ? "-" : "");
                dgvMovimientos.Rows.Add(
                    m.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    m.Tipo,
                    m.ProductoNombre,
                    prefijo + Math.Abs(m.Cantidad),
                    m.StockAnterior,
                    m.StockNuevo,
                    m.Motivo,
                    m.Id,
                    m.ProductoId
                );
            }
        }

        private void DgvMovimientos_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string colName = dgvMovimientos.Columns[e.ColumnIndex].Name;
            if (sortColumn == colName)
            {
                sortAscending = !sortAscending;
            }
            else
            {
                sortColumn = colName;
                sortAscending = true;
            }

            SortData();
            RenderGrid();
        }

        private void DgvMovimientos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvMovimientos.Columns["colTipo"].Index && e.Value != null)
            {
                string val = e.Value.ToString();
                if (val == "Entrada")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                else if (val == "Salida")
                {
                    e.CellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(127, 140, 141);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
                }
            }
            else if (e.ColumnIndex == dgvMovimientos.Columns["colCantidad"].Index && e.Value != null)
            {
                string val = e.Value.ToString();
                if (val.StartsWith("+"))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                else if (val.StartsWith("-"))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
            }
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            using (var form = new MovimientoForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
        }
    }
}
