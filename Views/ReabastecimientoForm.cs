using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.DataAccess;
using System.Linq;
using System.Text.RegularExpressions;

namespace STOCKPAP.Views
{
    public class ReabastecimientoForm : Form
    {
        private Producto producto;
        private Proveedor proveedor;
        public int CantidadPedida { get; private set; }
        private TextBox txtDetallePedido;
        private Label lblProveedorNombre;
        private Label lblProveedorContacto;
        private Label lblProveedorTelefono;
        private Label lblProveedorEmail;
        private Label lblProveedorDireccion;
        private Button btnEnviarEmail;
        private Button btnCerrar;

        public ReabastecimientoForm(Producto p)
        {
            this.producto = p;
            CargarProveedor();
            InitializeComponent();
            GenerarPedido();
        }

        private void CargarProveedor()
        {
            if (producto.ProveedorId.HasValue)
            {
                var repoProv = new ProveedorRepository();
                this.proveedor = repoProv.ObtenerTodos().FirstOrDefault(pr => pr.Id == producto.ProveedorId.Value);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Solicitud de Reabastecimiento - " + producto.Nombre;
            this.Size = new Size(500, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            Label lblTitle = new Label
            {
                Text = "Reabastecimiento de Producto",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 96, 255),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Ficha del Proveedor
            GroupBox grpProveedor = new GroupBox
            {
                Text = " Información del Proveedor Asociado ",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(20, 60),
                Size = new Size(440, 190),
                ForeColor = Color.FromArgb(60, 60, 80)
            };
            this.Controls.Add(grpProveedor);

            int y = 30;
            if (proveedor != null)
            {
                lblProveedorNombre = AddFilaProveedor(grpProveedor, "Empresa:", proveedor.Empresa, ref y);
                lblProveedorContacto = AddFilaProveedor(grpProveedor, "Contacto:", proveedor.Contacto, ref y);
                lblProveedorTelefono = AddFilaProveedor(grpProveedor, "Teléfono:", proveedor.Telefono, ref y);
                lblProveedorEmail = AddFilaProveedor(grpProveedor, "Email:", proveedor.Email, ref y);
                lblProveedorDireccion = AddFilaProveedor(grpProveedor, "Dirección:", proveedor.Direccion, ref y);
            }
            else
            {
                Label lblNoProv = new Label
                {
                    Text = "No hay un proveedor asociado a este producto.\nPara asociar un proveedor, edite el producto en el catálogo.",
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                    ForeColor = Color.FromArgb(190, 18, 60),
                    Location = new Point(20, 50),
                    Size = new Size(400, 60),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                grpProveedor.Controls.Add(lblNoProv);
            }

            // Cuerpo del Pedido
            Label lblPedido = new Label
            {
                Text = "Detalle del Pedido Sugerido:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(20, 265),
                AutoSize = true
            };
            this.Controls.Add(lblPedido);

            txtDetallePedido = new TextBox
            {
                Location = new Point(20, 290),
                Size = new Size(440, 160),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtDetallePedido);

            // Botones (Centrados)
            btnCerrar = new Button
            {
                Text = "Cerrar",
                Location = new Point(100, 465),
                Size = new Size(120, 36),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCerrar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCerrar);

            btnEnviarEmail = new Button
            {
                Text = "Realizar Pedido",
                Location = new Point(250, 465),
                Size = new Size(150, 36),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnEnviarEmail.Click += BtnEnviarEmail_Click;
            this.Controls.Add(btnEnviarEmail);

            if (proveedor == null)
            {
                btnEnviarEmail.Enabled = false;
                btnEnviarEmail.BackColor = Color.Gray;
            }
        }

        private Label AddFilaProveedor(GroupBox box, string label, string value, ref int y)
        {
            Label lblKey = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 100),
                Location = new Point(15, y),
                AutoSize = true
            };
            box.Controls.Add(lblKey);

            Label lblVal = new Label
            {
                Text = string.IsNullOrEmpty(value) ? "Sin registrar" : value,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Black,
                Location = new Point(95, y),
                Width = 320,
                AutoEllipsis = true,
                AutoSize = false,
                Height = 18
            };
            box.Controls.Add(lblVal);

            y += 28;
            return lblVal;
        }

        private void GenerarPedido()
        {
            int cantidadSugerida = Math.Max(20, (producto.StockMinimo * 2) - producto.Stock);
            string lineas = string.Format(
                "Solicitud de Reabastecimiento de Mercancía\r\n" +
                "----------------------------------------\r\n" +
                "Fecha: {0:dd/MM/yyyy}\r\n" +
                "Proveedor: {1}\r\n" +
                "Atn: {2}\r\n\r\n" +
                "Estimado proveedor,\r\n" +
                "Por medio del presente solicitamos cotización y reabastecimiento para el siguiente artículo:\r\n\r\n" +
                " - Producto: {3}\r\n" +
                " - Código de Barras: {4}\r\n" +
                " - Cantidad Solicitada: {5} unidades\r\n\r\n" +
                "Quedamos a la espera de sus comentarios sobre costo, disponibilidad y tiempo de entrega.\r\n\r\n" +
                "Atentamente,\r\n" +
                "STOCKPAP - Control de Inventario",
                DateTime.Now,
                proveedor != null ? proveedor.Empresa : "Sin Proveedor",
                proveedor != null ? proveedor.Contacto : "Contacto",
                producto.Nombre,
                string.IsNullOrEmpty(producto.CodigoBarras) ? "N/A" : producto.CodigoBarras,
                cantidadSugerida
            );
            txtDetallePedido.Text = lineas;
        }



        private void BtnEnviarEmail_Click(object sender, EventArgs e)
        {
            var match = Regex.Match(txtDetallePedido.Text, @"Cantidad Solicitada:\s*(\d+)\s*unidades", RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int qty))
            {
                CantidadPedida = qty;
            }
            else
            {
                CantidadPedida = Math.Max(20, (producto.StockMinimo * 2) - producto.Stock);
            }

            MessageBox.Show("Ha realizado correctamente su pedido", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
