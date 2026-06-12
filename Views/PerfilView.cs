using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;
using Npgsql;

namespace STOCKPAP.Views
{
    public class PerfilView : UserControl
    {
        private Usuario currentUser;
        private TextBox txtNuevoNombre;
        private TextBox txtNuevoUser;
        private TextBox txtNuevaPassword;
        private ComboBox cmbNuevoRol;
        private Label lblEstadoNuevo;

        public PerfilView(Usuario user)
        {
            this.currentUser = user;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            // Título
            Label lblTitle = new Label
            {
                Text = "Perfil de Usuario",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 40),
                AutoSize = true,
                Location = new Point(30, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Administra tu información personal y agrega nuevos usuarios al sistema",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(35, 75)
            };
            this.Controls.Add(lblSubtitle);

            // ── PANEL IZQUIERDO: Tarjeta de Perfil ──
            RoundedPanel panelInfo = new RoundedPanel
            {
                Size = new Size(350, 450),
                Location = new Point(30, 120),
                BackColor = Color.White,
                BorderRadius = 15
            };
            this.Controls.Add(panelInfo);

            // Avatar circular
            Panel avatar = new Panel
            {
                Size = new Size(120, 120),
                Location = new Point(115, 40),
                BackColor = Color.FromArgb(35, 45, 60)
            };
            avatar.Paint += (s, ev) =>
            {
                ev.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                ev.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(35, 45, 60)), 0, 0, avatar.Width - 1, avatar.Height - 1);
                string initial = currentUser.Username.Length > 0 ? currentUser.Username[0].ToString().ToUpper() : "U";
                using (Font f = new Font("Segoe UI", 36, FontStyle.Bold))
                    ev.Graphics.DrawString(initial, f, Brushes.White,
                        new RectangleF(0, 0, avatar.Width, avatar.Height),
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            };
            avatar.BackColor = Color.Transparent;
            panelInfo.Controls.Add(avatar);

            Label lblName = new Label
            {
                Text = currentUser.NombreCompleto,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 50, 60),
                AutoSize = false,
                Size = new Size(350, 30),
                Location = new Point(0, 180),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelInfo.Controls.Add(lblName);

            Label lblRol = new Label
            {
                Text = currentUser.Rol == "Admin" ? "Administrador del Sistema" : "Cajero",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = false,
                Size = new Size(350, 20),
                Location = new Point(0, 215),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelInfo.Controls.Add(lblRol);

            RoundedPanel badgeVerificado = new RoundedPanel
            {
                Size = new Size(250, 40),
                Location = new Point(50, 260),
                BackColor = Color.FromArgb(235, 248, 240),
                BorderRadius = 8
            };
            Label lblVerificado = new Label
            {
                Text = "✓ Verificado",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 180, 110),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            badgeVerificado.Controls.Add(lblVerificado);
            panelInfo.Controls.Add(badgeVerificado);

            Button btnCambiarFoto = new Button
            {
                Text = "Cambiar Foto",
                Size = new Size(250, 40),
                Location = new Point(50, 320),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 70, 80),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCambiarFoto.FlatAppearance.BorderColor = Color.LightGray;
            panelInfo.Controls.Add(btnCambiarFoto);

            // ── PANEL DERECHO: Agregar Nuevo Perfil ──
            RoundedPanel panelAgregar = new RoundedPanel
            {
                Size = new Size(580, 450),
                Location = new Point(410, 120),
                BackColor = Color.White,
                BorderRadius = 15
            };
            this.Controls.Add(panelAgregar);

            Label lblAgregarTitle = new Label
            {
                Text = "Agregar Nuevo Perfil",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 50, 60),
                Location = new Point(30, 25),
                AutoSize = true
            };
            panelAgregar.Controls.Add(lblAgregarTitle);

            // Fila 1: Nombre y Usuario
            AddLabel(panelAgregar, "Nombre Completo", 30, 80);
            txtNuevoNombre = AddTextBox(panelAgregar, 30, 105, 240);

            AddLabel(panelAgregar, "Usuario (Login)", 300, 80);
            txtNuevoUser = AddTextBox(panelAgregar, 300, 105, 240);

            // Fila 2: Contraseña y Rol
            AddLabel(panelAgregar, "Contraseña", 30, 160);
            txtNuevaPassword = AddTextBox(panelAgregar, 30, 185, 240);

            AddLabel(panelAgregar, "Perfil de Acceso (Admin/Cajero)", 300, 160);
            cmbNuevoRol = new ComboBox
            {
                Location = new Point(300, 185),
                Size = new Size(240, 30),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbNuevoRol.Items.Add("Administrador");
            cmbNuevoRol.Items.Add("Cajero");
            cmbNuevoRol.SelectedIndex = 1; // Cajero por defecto
            panelAgregar.Controls.Add(cmbNuevoRol);

            lblEstadoNuevo = new Label
            {
                Location = new Point(30, 250),
                Size = new Size(510, 40),
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelAgregar.Controls.Add(lblEstadoNuevo);

            // Botones Cancelar y Guardar
            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(230, 310),
                Size = new Size(140, 45),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(60, 70, 80),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderColor = Color.LightGray;
            btnCancelar.Click += (s, e) => {
                txtNuevoNombre.Clear();
                txtNuevoUser.Clear();
                txtNuevaPassword.Clear();
                lblEstadoNuevo.Text = "";
            };
            panelAgregar.Controls.Add(btnCancelar);

            RoundedButton btnGuardarNuevo = new RoundedButton
            {
                Text = "Guardar Cambios",
                Location = new Point(390, 310),
                Size = new Size(160, 45),
                BackColor = Color.FromArgb(90, 180, 130), // Verde como en la imagen
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BorderRadius = 8,
                Cursor = Cursors.Hand
            };
            btnGuardarNuevo.Click += BtnGuardarNuevo_Click;
            panelAgregar.Controls.Add(btnGuardarNuevo);
        }

        private void AddLabel(Panel parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(x, y),
                AutoSize = true
            });
        }

        private TextBox AddTextBox(Panel parent, int x, int y, int width)
        {
            TextBox tb = new TextBox
            {
                Location = new Point(x, y),
                Width = width,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            parent.Controls.Add(tb);
            return tb;
        }

        private void BtnGuardarNuevo_Click(object sender, EventArgs e)
        {
            string nombre = txtNuevoNombre.Text.Trim();
            string user = txtNuevoUser.Text.Trim();
            string pass = txtNuevaPassword.Text.Trim();
            string rol = cmbNuevoRol.SelectedItem.ToString();
            string rolBd = rol == "Administrador" ? "Admin" : "Ventas";

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblEstadoNuevo.ForeColor = Color.Red;
                lblEstadoNuevo.Text = "Por favor, completa todos los campos.";
                return;
            }

            try
            {
                using (var conn = Conexion.Instance.GetConnection())
                {
                    conn.Open();

                    // Verificar si el usuario ya existe
                    using (var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM Usuarios WHERE Username = @u", conn))
                    {
                        checkCmd.Parameters.AddWithValue("u", user);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            lblEstadoNuevo.ForeColor = Color.Red;
                            lblEstadoNuevo.Text = "El nombre de usuario ya está en uso.";
                            return;
                        }
                    }

                    // Insertar nuevo usuario
                    string query = "INSERT INTO Usuarios (Username, Password, Rol, NombreCompleto, Email) VALUES (@u, @p, @r, @n, @e)";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("u", user);
                        cmd.Parameters.AddWithValue("p", pass);
                        cmd.Parameters.AddWithValue("r", rolBd);
                        cmd.Parameters.AddWithValue("n", nombre);
                        cmd.Parameters.AddWithValue("e", user + "@empresa.com"); // Email por defecto
                        cmd.ExecuteNonQuery();
                    }
                }

                lblEstadoNuevo.ForeColor = Color.Green;
                lblEstadoNuevo.Text = "✓ ¡Usuario agregado correctamente!";
                
                txtNuevoNombre.Clear();
                txtNuevoUser.Clear();
                txtNuevaPassword.Clear();
                
                MessageBox.Show("El nuevo usuario se ha creado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblEstadoNuevo.ForeColor = Color.Red;
                lblEstadoNuevo.Text = "Error al guardar el usuario: " + ex.Message;
            }
        }
    }
}
