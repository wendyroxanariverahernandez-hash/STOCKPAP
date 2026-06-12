using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class PromptDialog : Form
    {
        private TextBox txtInput;
        public string ValorIngresado { get; private set; }

        public PromptDialog(string titulo, string mensaje, string valorPorDefecto = "")
        {
            this.Text = titulo;
            this.Size = new Size(350, 180);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            Label lblMsg = new Label
            {
                Text = mensaje,
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblMsg);

            txtInput = new TextBox
            {
                Location = new Point(20, 50),
                Width = 290,
                Font = new Font("Segoe UI", 11),
                Text = valorPorDefecto
            };
            this.Controls.Add(txtInput);

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(130, 90),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderColor = Color.LightGray;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancelar);

            Button btnAceptar = new Button
            {
                Text = "Aceptar",
                Location = new Point(220, 90),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAceptar.FlatAppearance.BorderSize = 0;
            btnAceptar.Click += BtnAceptar_Click;
            this.Controls.Add(btnAceptar);

            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                MessageBox.Show("El valor no puede estar vacío.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ValorIngresado = txtInput.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public static string Mostrar(string titulo, string mensaje, string valorPorDefecto = "")
        {
            using (var prompt = new PromptDialog(titulo, mensaje, valorPorDefecto))
            {
                if (prompt.ShowDialog() == DialogResult.OK)
                    return prompt.ValorIngresado;
                return null;
            }
        }
    }
}
