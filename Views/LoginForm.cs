using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Utilities;
using STOCKPAP.DataAccess;

namespace STOCKPAP.Views
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private RoundedButton btnLogin;
        private Label lblError;
        private RoundedPanel panelLogin;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "StockPap - Iniciar Sesion";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            try
            {
                string bgPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images", "login_bg.png");
                if (System.IO.File.Exists(bgPath))
                {
                    this.BackgroundImage = Image.FromFile(bgPath);
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
            catch { }

            panelLogin = new RoundedPanel
            {
                Size = new Size(320, 380),
                Location = new Point(32, 40),
                BackColor = Color.FromArgb(245, 255, 255, 255),
                BorderRadius = 20
            };
            this.Controls.Add(panelLogin);

            Label lblTitle = new Label
            {
                Text = "StockPap",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 96, 255),
                AutoSize = true,
                Location = new Point(80, 40)
            };
            panelLogin.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Sistema de Inventario",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(90, 85)
            };
            panelLogin.Controls.Add(lblSubtitle);

            txtUsername = new TextBox
            {
                Location = new Point(40, 150),
                Size = new Size(240, 30),
                Font = new Font("Segoe UI", 12),
                Text = "Usuario",
                ForeColor = Color.Gray
            };
            txtUsername.Enter += (s, e) => { if (txtUsername.Text == "Usuario") { txtUsername.Text = ""; txtUsername.ForeColor = Color.Black; } };
            txtUsername.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtUsername.Text)) { txtUsername.Text = "Usuario"; txtUsername.ForeColor = Color.Gray; } };
            panelLogin.Controls.Add(txtUsername);

            txtPassword = new TextBox
            {
                Location = new Point(40, 200),
                Size = new Size(240, 30),
                Font = new Font("Segoe UI", 12),
                Text = "Contrasena",
                ForeColor = Color.Gray
            };
            txtPassword.Enter += (s, e) => { if (txtPassword.Text == "Contrasena") { txtPassword.Text = ""; txtPassword.ForeColor = Color.Black; txtPassword.PasswordChar = '*'; } };
            txtPassword.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtPassword.Text)) { txtPassword.Text = "Contrasena"; txtPassword.ForeColor = Color.Gray; txtPassword.PasswordChar = '\0'; } };
            panelLogin.Controls.Add(txtPassword);

            lblError = new Label
            {
                Location = new Point(40, 240),
                Size = new Size(240, 20),
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelLogin.Controls.Add(lblError);

            btnLogin = new RoundedButton
            {
                Text = "Ingresar",
                Size = new Size(240, 45),
                Location = new Point(40, 280),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BorderRadius = 10
            };
            btnLogin.Click += BtnLogin_Click;
            panelLogin.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            try
            {
                var repo = new UsuarioRepository();
                var usuario = repo.Autenticar(user, pass);

                if (usuario != null)
                {
                    MainForm mainForm = new MainForm(usuario);
                    mainForm.Show();
                    this.Hide();
                    mainForm.FormClosed += (s, args) => this.Close();
                }
                else
                {
                    lblError.Text = "Usuario o contrasena incorrectos.";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "No se pudo conectar a la base de datos.";
                MessageBox.Show(
                    "Revisa que PostgreSQL este iniciado, que exista la base stockpap_db y que la cadena de conexion sea correcta.\n\nDetalle: " + ex.Message,
                    "Error de conexion",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
