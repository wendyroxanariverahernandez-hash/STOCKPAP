using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            btnLogin.Click += BtnLogin_Click;
            txtPass.PasswordChar = '\0'; // Show text initially for placeholder
        }

        private void txtUser_Enter(object sender, EventArgs e)
        {
            if (txtUser.Text == "Usuario")
            {
                txtUser.Text = "";
                txtUser.ForeColor = Color.White;
            }
        }

        private void txtUser_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text))
            {
                txtUser.Text = "Usuario";
                txtUser.ForeColor = Color.LightGray;
            }
        }

        private void txtPass_Enter(object sender, EventArgs e)
        {
            if (txtPass.Text == "Contraseña")
            {
                txtPass.Text = "";
                txtPass.ForeColor = Color.White;
                txtPass.PasswordChar = '*';
            }
        }

        private void txtPass_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPass.Text))
            {
                txtPass.Text = "Contraseña";
                txtPass.ForeColor = Color.LightGray;
                txtPass.PasswordChar = '\0';
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text.Trim();
            string pass = txtPass.Text.Trim();

            if (user == "administrador" && pass == "wendy123")
            {
                OpenMainForm("administrador");
            }
            else if (user == "cajero" && pass == "cajero123")
            {
                OpenMainForm("cajero");
            }
            else
            {
                lblError.Text = "Usuario o contraseña incorrectos.";
            }
        }

        private void OpenMainForm(string role)
        {
            this.Hide();
            Form1 mainForm = new Form1(role);
            mainForm.FormClosed += (s, args) => this.Close();
            mainForm.Show();
        }
    }
}
