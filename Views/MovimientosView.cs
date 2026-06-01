using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;
using System.Linq;

namespace STOCKPAP.Views
{
    public class MovimientosView : UserControl
    {
        private FlowLayoutPanel listMovimientos;
        private MovimientoRepository repo;
        private bool puedeEditar;

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

            // Título
            Label lblTitle = new Label { Text = "Movimientos de Inventario", Font = new Font("Segoe UI", 24, FontStyle.Bold), AutoSize = true, Location = new Point(30, 30) };
            Label lblSubtitle = new Label { Text = "Registra y consulta entradas, salidas y ajustes", Font = new Font("Segoe UI", 11), ForeColor = Color.Gray, AutoSize = true, Location = new Point(35, 75) };
            this.Controls.Add(lblTitle); this.Controls.Add(lblSubtitle);

            RoundedButton btnNuevo = new RoundedButton
            {
                Text = "+  Nuevo Movimiento",
                Size = new Size(180, 45),
                Location = new Point(this.Width - 210, 30),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnNuevo.Visible = puedeEditar;
            btnNuevo.Click += BtnNuevo_Click;
            this.Controls.Add(btnNuevo);

            // Stats Panels
            CreateStatCard("Total Entradas", "80", "unidades agregadas", Color.Green, 30, 120);
            CreateStatCard("Total Salidas", "12", "unidades retiradas", Color.Red, 280, 120);
            CreateStatCard("Movimientos Totales", "5", "operaciones registradas", Color.Blue, 530, 120);

            // Tabla/Lista
            RoundedPanel panelLista = new RoundedPanel
            {
                Size = new Size(750, 450),
                Location = new Point(30, 260),
                BackColor = Color.White,
                BorderRadius = 15,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelLista);

            Label lblListTitle = new Label { Text = "Historial de Movimientos", Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            panelLista.Controls.Add(lblListTitle);

            // Encabezados
            int yHeaders = 90;
            string[] headers = { "Tipo", "Producto", "Cantidad", "Stock Anterior", "Stock Nuevo", "Motivo" };
            int[] xPos = { 20, 120, 350, 450, 560, 660 };
            for (int i = 0; i < headers.Length; i++)
            {
                panelLista.Controls.Add(new Label { Text = headers[i], Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(xPos[i], yHeaders) });
            }
            Panel lineH = new Panel { BackColor = Color.LightGray, Height = 1, Width = 710, Location = new Point(20, yHeaders + 25) };
            panelLista.Controls.Add(lineH);

            listMovimientos = new FlowLayoutPanel
            {
                Location = new Point(10, 120),
                Size = new Size(730, 320),
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White
            };
            panelLista.Controls.Add(listMovimientos);
        }

        private void CreateStatCard(string title, string mainVal, string subVal, Color c, int x, int y)
        {
            RoundedPanel card = new RoundedPanel { Size = new Size(230, 120), Location = new Point(x, y), BackColor = Color.White, BorderRadius = 15 };
            card.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, AutoSize = true, Location = new Point(20, 20) });
            card.Controls.Add(new Label { Text = mainVal, Font = new Font("Segoe UI", 20, FontStyle.Bold), AutoSize = true, Location = new Point(20, 50) });
            card.Controls.Add(new Label { Text = subVal, Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, AutoSize = true, Location = new Point(20, 90) });
            this.Controls.Add(card);
        }

        private void LoadData()
        {
            listMovimientos.Controls.Clear();
            var movs = repo.ObtenerTodos();
            foreach (var m in movs)
            {
                Panel row = new Panel { Size = new Size(700, 40), Margin = new Padding(0) };

                // Tipo Badge
                RoundedPanel badge = new RoundedPanel { Size = new Size(80, 25), Location = new Point(10, 7), BackColor = m.Tipo == "Entrada" ? Color.Black : Color.FromArgb(200, 50, 50), BorderRadius = 10 };
                badge.Controls.Add(new Label { Text = m.Tipo, ForeColor = Color.White, Font = new Font("Segoe UI", 8, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
                row.Controls.Add(badge);

                // Info
                row.Controls.Add(new Label { Text = m.ProductoNombre, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(110, 10), AutoSize = false, Size = new Size(220, 20) });
                
                Label lblCant = new Label { Text = (m.Tipo == "Entrada" ? "+" : "") + m.Cantidad, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(340, 10), AutoSize = true };
                lblCant.ForeColor = m.Tipo == "Entrada" ? Color.Green : Color.Red;
                row.Controls.Add(lblCant);

                row.Controls.Add(new Label { Text = m.StockAnterior.ToString(), Font = new Font("Segoe UI", 10), Location = new Point(460, 10), AutoSize = true });
                row.Controls.Add(new Label { Text = m.StockNuevo.ToString(), Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(560, 10), AutoSize = true });
                row.Controls.Add(new Label { Text = m.Motivo, Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, Location = new Point(650, 10), AutoSize = false, Size = new Size(150, 20) });

                listMovimientos.Controls.Add(row);
                listMovimientos.Controls.Add(new Panel { BackColor = Color.WhiteSmoke, Height = 1, Width = 700 });
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
