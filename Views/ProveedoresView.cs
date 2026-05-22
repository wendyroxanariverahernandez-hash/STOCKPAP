using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class ProveedoresView : UserControl
    {
        private FlowLayoutPanel gridProveedores;
        private ProveedorRepository repo;

        public ProveedoresView()
        {
            repo = new ProveedorRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            Label lblTitle = new Label { Text = "Proveedores", Font = new Font("Segoe UI", 24, FontStyle.Bold), AutoSize = true, Location = new Point(30, 30) };
            Label lblSubtitle = new Label { Text = "Administra tus proveedores y contactos", Font = new Font("Segoe UI", 11), ForeColor = Color.Gray, AutoSize = true, Location = new Point(35, 75) };
            this.Controls.Add(lblTitle); this.Controls.Add(lblSubtitle);

            RoundedButton btnNuevo = new RoundedButton
            {
                Text = "+  Agregar Proveedor",
                Size = new Size(180, 45),
                Location = new Point(this.Width - 210, 30),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.Controls.Add(btnNuevo);

            gridProveedores = new FlowLayoutPanel
            {
                Location = new Point(30, 130),
                Size = new Size(800, 600),
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };
            this.Controls.Add(gridProveedores);
        }

        private void LoadData()
        {
            gridProveedores.Controls.Clear();
            var provs = repo.ObtenerTodos();

            foreach (var p in provs)
            {
                RoundedPanel card = new RoundedPanel
                {
                    Size = new Size(350, 250),
                    BackColor = Color.White,
                    BorderRadius = 15,
                    Margin = new Padding(10)
                };

                Label lblNombre = new Label { Text = p.Empresa, Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20), MaximumSize = new Size(250, 0) };
                Label lblContacto = new Label { Text = "Contacto: " + p.Contacto, Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, AutoSize = true, Location = new Point(20, 65) };
                Label lblEmail = new Label { Text = "✉  " + p.Email, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(30, 96, 255), AutoSize = true, Location = new Point(20, 100) };
                Label lblTel = new Label { Text = "📞  " + p.Telefono, Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, AutoSize = true, Location = new Point(20, 125) };
                Label lblDir = new Label { Text = "📍  " + p.Direccion, Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, AutoSize = true, Location = new Point(20, 150), MaximumSize = new Size(300, 0) };

                card.Controls.Add(lblNombre);
                card.Controls.Add(lblContacto);
                card.Controls.Add(lblEmail);
                card.Controls.Add(lblTel);
                card.Controls.Add(lblDir);

                gridProveedores.Controls.Add(card);
            }
        }
    }
}
