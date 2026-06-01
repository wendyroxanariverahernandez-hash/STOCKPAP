using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class AdminAuthorizationForm : Form
    {
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private Label lblError;

        public bool Autorizado { get; private set; }

        public AdminAuthorizationForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Autorizacion de administrador";
            this.Size = new Size(360, 260);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            this.Controls.Add(new Label
            {
                Text = "Se requiere usuario administrador",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            });

            this.Controls.Add(new Label { Text = "Usuario", Location = new Point(20, 65), AutoSize = true });
            txtUsuario = new TextBox { Location = new Point(20, 88), Width = 300, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txtUsuario);

            this.Controls.Add(new Label { Text = "Contrasena", Location = new Point(20, 120), AutoSize = true });
            txtPassword = new TextBox { Location = new Point(20, 143), Width = 300, Font = new Font("Segoe UI", 10), PasswordChar = '*' };
            this.Controls.Add(txtPassword);

            lblError = new Label
            {
                Location = new Point(20, 172),
                Size = new Size(300, 20),
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblError);

            RoundedButton btnCancelar = new RoundedButton
            {
                Text = "Cerrar",
                Size = new Size(140, 35),
                Location = new Point(20, 195),
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderColor = Color.LightGray,
                BorderSize = 1,
                BorderRadius = 8
            };
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);

            RoundedButton btnAutorizar = new RoundedButton
            {
                Text = "Autorizar",
                Size = new Size(150, 35),
                Location = new Point(170, 195),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                BorderRadius = 8
            };
            btnAutorizar.Click += BtnAutorizar_Click;
            this.Controls.Add(btnAutorizar);
        }

        private void BtnAutorizar_Click(object sender, EventArgs e)
        {
            try
            {
                var repo = new UsuarioRepository();
                if (repo.EsAdministrador(txtUsuario.Text.Trim(), txtPassword.Text.Trim()))
                {
                    Autorizado = true;
                    this.Close();
                    return;
                }

                lblError.Text = "El usuario no es administrador.";
            }
            catch (Exception ex)
            {
                lblError.Text = "No se pudo validar el usuario.";
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
