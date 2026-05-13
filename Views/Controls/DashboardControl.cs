using System;
using System.Drawing;
using System.Windows.Forms;

namespace STOCKPAP.Views.Controls
{
    public partial class DashboardControl : UserControl
    {
        public DashboardControl()
        {
            InitializeComponent();
            LoadStats();
        }

        private void LoadStats()
        {
            flowStats.Controls.Add(CreateStatCard("Total Productos", "1,284", Color.FromArgb(13, 110, 253)));
            flowStats.Controls.Add(CreateStatCard("Stock Bajo", "18", Color.FromArgb(220, 53, 69)));
            flowStats.Controls.Add(CreateStatCard("Ventas Hoy", "$4,520.00", Color.FromArgb(25, 135, 84)));
            flowStats.Controls.Add(CreateStatCard("Proveedores", "42", Color.FromArgb(108, 117, 125)));
        }

        private Panel CreateStatCard(string title, string value, Color color)
        {
            Panel card = new Panel
            {
                Size = new Size(210, 120),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 20, 0)
            };

            Label lblT = new Label { Text = title, Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, Location = new Point(15, 15), AutoSize = true };
            Label lblV = new Label { Text = value, Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = color, Location = new Point(15, 45), AutoSize = true };
            
            card.Controls.Add(lblT);
            card.Controls.Add(lblV);

            card.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, Color.FromArgb(230, 230, 230), ButtonBorderStyle.Solid);
            };

            return card;
        }
    }
}
