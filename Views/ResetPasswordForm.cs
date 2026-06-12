using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using Npgsql;

namespace STOCKPAP.Views
{
    public class ResetPasswordForm : Form
    {
        private TextBox txtUsernameToReset;
        private TextBox txtAdminUser;
        private TextBox txtAdminPassword;
        private TextBox txtNewPassword;
        private Button btnReset;
        private Button btnCancel;
        private Label lblError;

        public ResetPasswordForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Restablecer Contraseña";
            this.Size = new Size(420, 430);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            Label lblHeader = new Label
            {
                Text = "Restaurar Contraseña",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 96, 255),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblHeader);

            Label lblSub = new Label
            {
                Text = "Requiere autorización de un administrador para continuar.",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(22, 50),
                AutoSize = true
            };
            this.Controls.Add(lblSub);

            // Usuario a restablecer
            AddLabel("Usuario a restablecer:", 20, 80);
            txtUsernameToReset = AddTextBox(20, 102, 360);

            // Autorización Admin
            AddLabel("Usuario Administrador:", 20, 140);
            txtAdminUser = AddTextBox(20, 162, 360);
            txtAdminUser.Text = "administrador"; // Valor por defecto conveniente

            AddLabel("Contraseña Administrador:", 20, 200);
            txtAdminPassword = AddTextBox(20, 222, 360);
            txtAdminPassword.PasswordChar = '*';

            // Nueva Contraseña
            AddLabel("Nueva Contraseña:", 20, 260);
            txtNewPassword = AddTextBox(20, 282, 360);
            txtNewPassword.PasswordChar = '*';

            lblError = new Label
            {
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 320),
                Size = new Size(360, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblError);

            btnCancel = new Button
            {
                Text = "Cancelar",
                Location = new Point(160, 350),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            btnReset = new Button
            {
                Text = "Restablecer",
                Location = new Point(280, 350),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);

            this.AcceptButton = btnReset;
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(x, y),
                AutoSize = true
            });
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            TextBox tb = new TextBox
            {
                Location = new Point(x, y),
                Width = width,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(tb);
            return tb;
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            string targetUser = txtUsernameToReset.Text.Trim();
            string adminUser = txtAdminUser.Text.Trim();
            string adminPass = txtAdminPassword.Text.Trim();
            string newPass = txtNewPassword.Text.Trim();

            if (string.IsNullOrEmpty(targetUser) || string.IsNullOrEmpty(adminUser) || string.IsNullOrEmpty(adminPass) || string.IsNullOrEmpty(newPass))
            {
                lblError.Text = "Por favor, llene todos los campos.";
                return;
            }

            try
            {
                var repo = new UsuarioRepository();
                var admin = repo.Autenticar(adminUser, adminPass);

                if (admin == null || admin.Rol.ToLower() != "admin")
                {
                    lblError.Text = "Credenciales de administrador incorrectas.";
                    return;
                }

                // Verificar si existe el usuario de destino
                bool existeUsuario = false;
                using (var conn = Conexion.Instance.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM Usuarios WHERE Username = @u", conn))
                    {
                        cmd.Parameters.AddWithValue("u", targetUser);
                        existeUsuario = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }

                    if (!existeUsuario)
                    {
                        lblError.Text = "El usuario a restablecer no existe.";
                        return;
                    }

                    // Proceder a cambiar la contraseña
                    using (var cmdUpdate = new NpgsqlCommand("UPDATE Usuarios SET Password = @p WHERE Username = @u", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("p", newPass);
                        cmdUpdate.Parameters.AddWithValue("u", targetUser);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Contraseña restablecida con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }
    }
}
