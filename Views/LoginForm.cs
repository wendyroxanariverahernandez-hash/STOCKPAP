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

            // Make the panel rounded visually using Paint event
            pnlCard.Paint += (s, e) =>
            {
                var rect = pnlCard.ClientRectangle;
                rect.Width -= 1; rect.Height -= 1;
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };
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
