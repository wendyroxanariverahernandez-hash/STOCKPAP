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

        private Proveedor _proveedorEditar;
        public bool EsEdicion => _proveedorEditar != null;

        public ProveedorForm() : this(null) { }

        public ProveedorForm(Proveedor proveedorEditar)
        {
            _proveedorEditar = proveedorEditar;
            InitializeComponent();
            if (EsEdicion) CargarDatosEdicion();
        }

        private void InitializeComponent()
        {
            this.Text = EsEdicion ? "Editar Proveedor" : "Nuevo Proveedor";
            this.Size = new Size(460, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // ── Encabezado ──────────────────────────────────────────────────
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(30, 96, 255)
            };
            Label lblTitle = new Label
            {
                Text = EsEdicion ? "✏  Editar Proveedor" : "➕  Nuevo Proveedor",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // ── Campos ──────────────────────────────────────────────────────
            int y = 85;
            int fieldWidth = 400;

            AddLabel("Empresa / Razón Social: *", 20, y);
            txtEmpresa = AddTextBox(20, y + 25, fieldWidth);
            y += 72;

            AddLabel("Nombre del Contacto:", 20, y);
            txtContacto = AddTextBox(20, y + 25, fieldWidth);
            y += 72;

            AddLabel("Teléfono:", 20, y);
            txtTelefono = AddTextBox(20, y + 25, fieldWidth);
            y += 72;

            AddLabel("Correo Electrónico:", 20, y);
            txtEmail = AddTextBox(20, y + 25, fieldWidth);
            y += 72;

            AddLabel("Dirección:", 20, y);
            txtDireccion = AddTextBox(20, y + 25, fieldWidth);
            y += 72;

            // ── Botones ─────────────────────────────────────────────────────
            Panel pnlBotones = new Panel
            {
                Location = new Point(0, y + 5),
                Size = new Size(460, 60),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(100, 10),
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCancelar.Click += (s, ev) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            RoundedButton btnGuardar = new RoundedButton
            {
                Text = EsEdicion ? "💾  Actualizar" : "💾  Guardar",
                Location = new Point(220, 10),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 8,
                Cursor = Cursors.Hand
            };
            btnGuardar.Click += BtnGuardar_Click;

            pnlBotones.Controls.Add(btnCancelar);
            pnlBotones.Controls.Add(btnGuardar);
            this.Controls.Add(pnlBotones);
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Location = new Point(x, y)
            });
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            var tb = new TextBox
            {
                Location = new Point(x, y),
                Width = width,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(tb);
            return tb;
        }

        private void CargarDatosEdicion()
        {
            txtEmpresa.Text   = _proveedorEditar.Empresa;
            txtContacto.Text  = _proveedorEditar.Contacto;
            txtTelefono.Text  = _proveedorEditar.Telefono;
            txtEmail.Text     = _proveedorEditar.Email;
            txtDireccion.Text = _proveedorEditar.Direccion;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmpresa.Text))
            {
                MessageBox.Show("El nombre de la empresa es requerido.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmpresa.Focus();
                return;
            }

            Proveedor p = new Proveedor
            {
                Id        = EsEdicion ? _proveedorEditar.Id : 0,
                Empresa   = txtEmpresa.Text.Trim(),
                Contacto  = txtContacto.Text.Trim(),
                Telefono  = txtTelefono.Text.Trim(),
                Email     = txtEmail.Text.Trim(),
                Direccion = txtDireccion.Text.Trim()
            };

            var repo = new ProveedorRepository();
            bool ok = EsEdicion ? repo.ActualizarProveedor(p) : repo.AgregarProveedor(p);

            if (ok)
            {
                string msg = EsEdicion ? "Proveedor actualizado correctamente." : "Proveedor registrado con éxito.";
                MessageBox.Show(msg, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al guardar en la base de datos.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
