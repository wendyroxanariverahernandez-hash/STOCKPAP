using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Utilities;
using STOCKPAP.DataAccess;
using System.Linq;

namespace STOCKPAP.Views
{
    public class ReportesView : UserControl
    {
        private ProductoRepository repo;

        public ReportesView()
        {
            repo = new ProductoRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            Label lblTitle = new Label { Text = "Reportes de Inventario", Font = new Font("Segoe UI", 24, FontStyle.Bold), AutoSize = true, Location = new Point(30, 30) };
            Label lblSubtitle = new Label { Text = "Análisis y métricas del inventario", Font = new Font("Segoe UI", 11), ForeColor = Color.Gray, AutoSize = true, Location = new Point(35, 75) };
            this.Controls.Add(lblTitle); this.Controls.Add(lblSubtitle);

            RoundedButton btnExport = new RoundedButton
            {
                Text = "Descargar CSV",
                Size = new Size(150, 40),
                Location = new Point(this.Width - 180, 30),
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderColor = Color.LightGray,
                BorderSize = 1,
                Font = new Font("Segoe UI", 10),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.Controls.Add(btnExport);

            // Create placeholders for charts/data since we can't use external libraries
            RoundedPanel panelBars = new RoundedPanel { Size = new Size(350, 300), Location = new Point(30, 260), BackColor = Color.White, BorderRadius = 15 };
            Label lblBarTitle = new Label { Text = "Movimientos de Inventario", Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            panelBars.Controls.Add(lblBarTitle);
            
            // Fake Bar Chart
            Panel b1 = new Panel { Size = new Size(60, 200), Location = new Point(50, 70), BackColor = Color.FromArgb(30, 96, 255) };
            Panel b2 = new Panel { Size = new Size(60, 40), Location = new Point(140, 230), BackColor = Color.FromArgb(30, 96, 255) };
            Panel b3 = new Panel { Size = new Size(60, 30), Location = new Point(230, 240), BackColor = Color.FromArgb(30, 96, 255) };
            panelBars.Controls.Add(b1); panelBars.Controls.Add(b2); panelBars.Controls.Add(b3);
            panelBars.Controls.Add(new Label { Text = "Entradas", Location = new Point(50, 275), AutoSize = true });
            panelBars.Controls.Add(new Label { Text = "Salidas", Location = new Point(145, 275), AutoSize = true });
            panelBars.Controls.Add(new Label { Text = "Ajustes", Location = new Point(235, 275), AutoSize = true });
            
            this.Controls.Add(panelBars);
        }

        private void LoadData()
        {
            var productos = repo.ObtenerTodos();
            
            int totalProd = productos.Count;
            int totalUnits = productos.Sum(p => p.Stock);
            decimal valor = productos.Sum(p => p.Stock * p.PrecioVenta);
            int stockBajo = productos.Count(p => p.Stock <= p.StockMinimo);

            CreateStatCard("Total Productos", totalProd.ToString(), "productos registrados", Color.Blue, 30, 120);
            CreateStatCard("Unidades Totales", totalUnits.ToString(), "unidades en inventario", Color.Green, 230, 120);
            CreateStatCard("Valor del Inventario", $"${valor:0.00}", "valor total estimado", Color.Purple, 430, 120);
            CreateStatCard("Stock Bajo", stockBajo.ToString(), "requieren reabastecimiento", Color.Red, 630, 120);
        }

        private void CreateStatCard(string title, string mainVal, string subVal, Color c, int x, int y)
        {
            RoundedPanel card = new RoundedPanel { Size = new Size(180, 120), Location = new Point(x, y), BackColor = Color.White, BorderRadius = 15 };
            card.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(15, 15) });
            card.Controls.Add(new Label { Text = mainVal, Font = new Font("Segoe UI", 18, FontStyle.Bold), AutoSize = true, Location = new Point(15, 45) });
            card.Controls.Add(new Label { Text = subVal, Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, AutoSize = true, Location = new Point(15, 85) });
            this.Controls.Add(card);
        }
    }
}
