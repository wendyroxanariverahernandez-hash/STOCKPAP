using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using STOCKPAP.Models;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class ProveedoresView : UserControl
    {
        private FlowLayoutPanel gridProveedores;
        private ProveedorRepository repo;
        private Label lblSubtitle;
        private TextBox txtBuscar;
        private bool puedeEditar;

        public ProveedoresView(bool puedeEditar = true)
        {
            this.puedeEditar = puedeEditar;
            repo = new ProveedorRepository();
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
                Text = "Proveedores",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 40),
                AutoSize = true,
                Location = new Point(30, 30)
            };
            this.Controls.Add(lblTitle);

            lblSubtitle = new Label
            {
                Text = "Cargando...",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(35, 72)
            };
            this.Controls.Add(lblSubtitle);

            // ── Botón Agregar ────────────────────────────────────────────────
            RoundedButton btnNuevo = new RoundedButton
            {
                Text = "➕  Agregar Proveedor",
                Size = new Size(195, 42),
                Location = new Point(this.Width - 225, 35),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnNuevo.Click += BtnNuevo_Click;
            btnNuevo.Visible = puedeEditar;
            this.Controls.Add(btnNuevo);

            // ── Barra de búsqueda ────────────────────────────────────────────
            RoundedPanel panelBuscar = new RoundedPanel
            {
                Size = new Size(800, 52),
                Location = new Point(30, 110),
                BackColor = Color.White,
                BorderRadius = 12,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelBuscar);

            Label lblLupa = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point(12, 13)
            };
            panelBuscar.Controls.Add(lblLupa);

            txtBuscar = new TextBox
            {
                Text = "Buscar proveedor...",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Location = new Point(42, 12),
                Width = 720,
                BorderStyle = BorderStyle.None
            };
            txtBuscar.Enter += (s, e) =>
            {
                if (txtBuscar.Text == "Buscar proveedor...")
                { txtBuscar.Text = ""; txtBuscar.ForeColor = Color.Black; }
            };
            txtBuscar.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                { txtBuscar.Text = "Buscar proveedor..."; txtBuscar.ForeColor = Color.Gray; }
            };
            txtBuscar.TextChanged += (s, e) =>
            {
                if (txtBuscar.Text != "Buscar proveedor...")
                    LoadData(txtBuscar.Text);
            };
            panelBuscar.Controls.Add(txtBuscar);

            // ── Grid ─────────────────────────────────────────────────────────
            gridProveedores = new FlowLayoutPanel
            {
                Location = new Point(30, 178),
                Size = new Size(830, 550),
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 5, 0, 5)
            };
            this.Controls.Add(gridProveedores);
        }

        // ── Carga de datos ───────────────────────────────────────────────────
        private void LoadData(string filtro = "")
        {
            gridProveedores.Controls.Clear();
            var provs = repo.ObtenerTodos();

            // Filtrar localmente si hay texto de búsqueda
            if (!string.IsNullOrEmpty(filtro))
            {
                string f = filtro.ToLower();
                provs = provs.FindAll(p =>
                    (p.Empresa?.ToLower().Contains(f) == true) ||
                    (p.Contacto?.ToLower().Contains(f) == true) ||
                    (p.Email?.ToLower().Contains(f) == true));
            }

            lblSubtitle.Text = $"{provs.Count} proveedor(es) registrado(s)";

            if (provs.Count == 0)
            {
                Label lblVacio = new Label
                {
                    Text = "No se encontraron proveedores.",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(20)
                };
                gridProveedores.Controls.Add(lblVacio);
                return;
            }

            foreach (var p in provs)
                gridProveedores.Controls.Add(CrearCardProveedor(p));
        }

        // ── Card de proveedor con botones Editar / Eliminar ──────────────────
        private Panel CrearCardProveedor(Proveedor p)
        {
            RoundedPanel card = new RoundedPanel
            {
                Size = new Size(370, 230),
                BackColor = Color.White,
                BorderRadius = 14,
                Margin = new Padding(8)
            };

            // Icono avatar
            Panel avatar = new Panel
            {
                Size = new Size(50, 50),
                Location = new Point(16, 16),
                BackColor = Color.FromArgb(30, 96, 255)
            };
            // Avatar circular con inicial
            avatar.Paint += (s, ev) =>
            {
                ev.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                ev.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(30, 96, 255)),
                    0, 0, avatar.Width - 1, avatar.Height - 1);
                string ini = p.Empresa.Length > 0 ? p.Empresa[0].ToString().ToUpper() : "P";
                using (Font f2 = new Font("Segoe UI", 20, FontStyle.Bold))
                    ev.Graphics.DrawString(ini, f2, Brushes.White,
                        new RectangleF(0, 0, avatar.Width, avatar.Height),
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            };
            avatar.BackColor = Color.Transparent;
            card.Controls.Add(avatar);

            // Nombre empresa
            Label lblNombre = new Label
            {
                Text = p.Empresa,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 40),
                AutoSize = false,
                Size = new Size(270, 28),
                Location = new Point(76, 16)
            };
            card.Controls.Add(lblNombre);

            // Línea divisora
            Panel linea = new Panel
            {
                BackColor = Color.FromArgb(235, 238, 245),
                Height = 1,
                Width = 338,
                Location = new Point(16, 76)
            };
            card.Controls.Add(linea);

            // Datos de contacto
            AddInfoRow(card, "👤", string.IsNullOrEmpty(p.Contacto) ? "Sin contacto" : p.Contacto, 16, 88, 250);
            AddInfoRow(card, "✉", string.IsNullOrEmpty(p.Email) ? "Sin correo" : p.Email, 16, 118, 250);
            AddInfoRow(card, "📞", string.IsNullOrEmpty(p.Telefono) ? "Sin teléfono" : p.Telefono, 16, 148, 250);

            // ── Botones ──────────────────────────────────────────────────────
            Button btnEditar = new Button
            {
                Text = "✏ Editar",
                Location = new Point(16, 188),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 244, 255),
                ForeColor = Color.FromArgb(30, 96, 255),
                Cursor = Cursors.Hand
            };
            btnEditar.FlatAppearance.BorderColor = Color.FromArgb(180, 200, 255);
            btnEditar.Click += (s, e) => EditarProveedor(p);
            if (puedeEditar) card.Controls.Add(btnEditar);

            Button btnEliminar = new Button
            {
                Text = "🗑 Eliminar",
                Location = new Point(126, 188),
                Size = new Size(110, 30),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 240, 240),
                ForeColor = Color.FromArgb(180, 30, 30),
                Cursor = Cursors.Hand
            };
            btnEliminar.FlatAppearance.BorderColor = Color.FromArgb(255, 180, 180);
            btnEliminar.Click += (s, e) => EliminarProveedor(p);
            if (puedeEditar) card.Controls.Add(btnEliminar);

            return card;
        }

        private void AddInfoRow(Control parent, string icon, string text, int x, int y, int maxW)
        {
            Label lbl = new Label
            {
                Text = $"{icon}  {text}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 100),
                AutoSize = false,
                Size = new Size(maxW, 22),
                Location = new Point(x, y)
            };
            parent.Controls.Add(lbl);
        }

        // ── Acciones CRUD ────────────────────────────────────────────────────
        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            using (var form = new ProveedorForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
        }

        private void EditarProveedor(Proveedor p)
        {
            using (var form = new ProveedorForm(p))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
        }

        private void EliminarProveedor(Proveedor p)
        {
            var res = MessageBox.Show(
                $"¿Deseas eliminar al proveedor:\n\n\"{p.Empresa}\"?\n\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (res == DialogResult.Yes)
            {
                if (repo.EliminarProveedor(p.Id))
                {
                    MessageBox.Show("Proveedor eliminado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el proveedor.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
