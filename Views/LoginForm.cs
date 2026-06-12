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

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "StockPap - Iniciar Sesion";
            this.ClientSize = new Size(350, 530);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // --- Imagen Logo Forzada y Centrada ---
            PictureBox picLogo = new PictureBox
            {
                Size = new Size(250, 120),
                Location = new Point(50, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            
            string logoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images", "logo.png");
            if (System.IO.File.Exists(logoPath))
            {
                picLogo.Image = Image.FromFile(logoPath);
            }
            this.Controls.Add(picLogo);

            int startY = 170;

            // --- Etiqueta y campo de Usuario ---
            Label lblUser = new Label
            {
                Text = "Usuario",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 50, 60),
                AutoSize = true,
                Location = new Point(40, startY)
            };
            this.Controls.Add(lblUser);

            Panel pnlUser = new Panel
            {
                Size = new Size(270, 36),
                Location = new Point(40, startY + 25),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            Label iconUser = new Label { Text = "👤", Font = new Font("Segoe UI Emoji", 11), Location = new Point(5, 6), AutoSize = true, ForeColor = Color.Gray };
            pnlUser.Controls.Add(iconUser);

            txtUsername = new TextBox
            {
                Location = new Point(35, 6),
                Size = new Size(225, 30),
                Font = new Font("Segoe UI", 12),
                Text = "usuario",
                ForeColor = Color.Gray,
                BorderStyle = BorderStyle.None
            };
            txtUsername.Enter += (s, e) => { if (txtUsername.Text == "usuario") { txtUsername.Text = ""; txtUsername.ForeColor = Color.Black; } };
            txtUsername.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtUsername.Text)) { txtUsername.Text = "usuario"; txtUsername.ForeColor = Color.Gray; } };
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; BtnLogin_Click(btnLogin, EventArgs.Empty); } };
            pnlUser.Controls.Add(txtUsername);
            this.Controls.Add(pnlUser);

            // --- Etiqueta y campo de Contraseña ---
            Label lblPass = new Label
            {
                Text = "Contraseña",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 50, 60),
                AutoSize = true,
                Location = new Point(40, startY + 75)
            };
            this.Controls.Add(lblPass);

            Panel pnlPass = new Panel
            {
                Size = new Size(270, 36),
                Location = new Point(40, startY + 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label iconPass = new Label { Text = "🔒", Font = new Font("Segoe UI Emoji", 11), Location = new Point(5, 6), AutoSize = true, ForeColor = Color.Gray };
            pnlPass.Controls.Add(iconPass);

            txtPassword = new TextBox
            {
                Location = new Point(35, 6),
                Size = new Size(225, 30),
                Font = new Font("Segoe UI", 12),
                Text = "********",
                ForeColor = Color.Gray,
                BorderStyle = BorderStyle.None
            };
            txtPassword.Enter += (s, e) => { if (txtPassword.Text == "********") { txtPassword.Text = ""; txtPassword.ForeColor = Color.Black; txtPassword.PasswordChar = '●'; } };
            txtPassword.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtPassword.Text)) { txtPassword.Text = "********"; txtPassword.ForeColor = Color.Gray; txtPassword.PasswordChar = '\0'; } };
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; BtnLogin_Click(btnLogin, EventArgs.Empty); } };
            pnlPass.Controls.Add(txtPassword);
            this.Controls.Add(pnlPass);

            // --- Recordarme y Olvidaste tu contraseña ---
            CheckBox chkRecordarme = new CheckBox
            {
                Text = "Recordarme",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 70, 80),
                AutoSize = true,
                Location = new Point(40, startY + 150),
                Cursor = Cursors.Hand
            };
            this.Controls.Add(chkRecordarme);

            LinkLabel lnkRestore = new LinkLabel
            {
                Text = "¿Olvidaste tu contraseña?",
                Location = new Point(140, startY + 152),
                Size = new Size(170, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ActiveLinkColor = Color.FromArgb(10, 70, 200),
                LinkColor = Color.FromArgb(30, 96, 255), // Azul solicitado
                TextAlign = ContentAlignment.MiddleRight,
                Cursor = Cursors.Hand
            };
            lnkRestore.LinkClicked += (s, ev) =>
            {
                using (var form = new ResetPasswordForm())
                {
                    form.ShowDialog(this);
                }
            };
            this.Controls.Add(lnkRestore);

            // --- Botón Ingresar ---
            btnLogin = new RoundedButton
            {
                Text = "Ingresar",
                Size = new Size(270, 45),
                Location = new Point(40, startY + 195),
                BackColor = Color.FromArgb(30, 96, 255), // Azul solicitado
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BorderRadius = 8,
                Cursor = Cursors.Hand
            };
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            // --- Mensaje de error ---
            lblError = new Label
            {
                Location = new Point(40, startY + 250),
                Size = new Size(270, 20),
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblError);

            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || user == "usuario")
            {
                lblError.Text = "Por favor ingrese su usuario.";
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(pass) || pass == "********")
            {
                lblError.Text = "Por favor ingrese su contraseña.";
                txtPassword.Focus();
                return;
            }

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
                    lblError.Text = "Usuario o contraseña incorrectos.";
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
