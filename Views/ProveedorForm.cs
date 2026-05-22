using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class ProveedorForm : Form
    {
        private TextBox txtEmpresa;
        private TextBox txtContacto;
        private TextBox txtTelefono;
        private TextBox txtEmail;
        private TextBox txtDireccion;

        public ProveedorForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Nuevo Proveedor";
            this.Size = new Size(400, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            Label lblTitle = new Label { Text = "Agregar Proveedor", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            this.Controls.Add(lblTitle);

            this.Controls.Add(new Label { Text = "Empresa:", Location = new Point(20, 70), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            txtEmpresa = new TextBox { Location = new Point(20, 95), Width = 340, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtEmpresa);

            this.Controls.Add(new Label { Text = "Contacto:", Location = new Point(20, 140), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            txtContacto = new TextBox { Location = new Point(20, 165), Width = 340, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtContacto);

            this.Controls.Add(new Label { Text = "Teléfono:", Location = new Point(20, 210), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            txtTelefono = new TextBox { Location = new Point(20, 235), Width = 340, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtTelefono);

            this.Controls.Add(new Label { Text = "Email:", Location = new Point(20, 280), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            txtEmail = new TextBox { Location = new Point(20, 305), Width = 340, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtEmail);

            this.Controls.Add(new Label { Text = "Dirección:", Location = new Point(20, 350), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            txtDireccion = new TextBox { Location = new Point(20, 375), Width = 340, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtDireccion);

            RoundedButton btnGuardar = new RoundedButton { Text = "Guardar", Location = new Point(100, 420), Size = new Size(180, 45), BackColor = Color.FromArgb(30, 96, 255), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), BorderRadius = 10 };
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmpresa.Text))
            {
                MessageBox.Show("El nombre de la empresa es requerido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Proveedor p = new Proveedor
            {
                Empresa = txtEmpresa.Text.Trim(),
                Contacto = txtContacto.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Direccion = txtDireccion.Text.Trim()
            };

            var repo = new ProveedorRepository();
            if (repo.AgregarProveedor(p))
            {
                MessageBox.Show("Proveedor registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al guardar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
