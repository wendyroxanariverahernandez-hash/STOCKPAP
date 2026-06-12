using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class CheckoutForm : Form
    {
        private readonly Venta ventaActual;
        private readonly Func<bool> autorizarCancelacion;
        private TextBox txtCliente;
        private TextBox txtEfectivo;
        private Label lblCambio;

        public bool VentaConfirmada { get; private set; }
        public bool VentaCancelada { get; private set; }

        public CheckoutForm(Venta venta, Func<bool> autorizarCancelacion)
        {
            ventaActual = venta;
            this.autorizarCancelacion = autorizarCancelacion;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Completar Venta";
            Size = new Size(420, 680); // Increased height for new buttons
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            Controls.Add(new Label
            {
                Text = "Completar Venta",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            });

            Controls.Add(new Label { Text = "Nombre del Cliente (Opcional)", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(20, 70) });
            RoundedPanel pnlCliente = new RoundedPanel { Size = new Size(360, 40), Location = new Point(20, 100), BorderColor = Color.LightGray, BorderSize = 1, BackColor = Color.WhiteSmoke, BorderRadius = 10 };
            txtCliente = new TextBox { Text = "Ej: Juan Perez", ForeColor = Color.Gray, Font = new Font("Segoe UI", 11), Width = 340, Location = new Point(10, 8), BorderStyle = BorderStyle.None, BackColor = Color.WhiteSmoke };
            txtCliente.Enter += (s, e) => { if (txtCliente.Text == "Ej: Juan Perez") { txtCliente.Text = ""; txtCliente.ForeColor = Color.Black; } };
            pnlCliente.Controls.Add(txtCliente);
            Controls.Add(pnlCliente);

            Controls.Add(new Label { Text = "Método de Pago", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(20, 160) });
            RoundedPanel pnlMetodo = new RoundedPanel { Size = new Size(360, 50), Location = new Point(20, 188), BackColor = Color.FromArgb(10, 15, 30), BorderRadius = 10 };
            pnlMetodo.Controls.Add(new Label { Text = "Efectivo", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            Controls.Add(pnlMetodo);

            Controls.Add(new Label { Text = "Efectivo Recibido", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(20, 260) });
            RoundedPanel pnlEfectivo = new RoundedPanel { Size = new Size(360, 40), Location = new Point(20, 290), BackColor = Color.WhiteSmoke, BorderRadius = 10 };
            txtEfectivo = new TextBox { Text = "0.00", Font = new Font("Segoe UI", 11), Width = 340, Location = new Point(10, 8), BorderStyle = BorderStyle.None, BackColor = Color.WhiteSmoke };
            pnlEfectivo.Controls.Add(txtEfectivo);
            Controls.Add(pnlEfectivo);

            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            // Quick money buttons
            FlowLayoutPanel pnlBilletes = new FlowLayoutPanel { Location = new Point(20, 340), Size = new Size(360, 40) };
            int[] billetes = { 50, 100, 200, 500 };
            foreach (int b in billetes)
            {
                Button btnB = new Button { Text = $"{sym}{b}", Size = new Size(65, 35), BackColor = Color.WhiteSmoke, FlatStyle = FlatStyle.Flat, ForeColor = Color.Black, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
                btnB.FlatAppearance.BorderSize = 0;
                btnB.Click += (s, e) => txtEfectivo.Text = b.ToString();
                pnlBilletes.Controls.Add(btnB);
            }
            Button btnExacto = new Button { Text = "Exacto", Size = new Size(70, 35), BackColor = Color.FromArgb(230, 240, 255), FlatStyle = FlatStyle.Flat, ForeColor = Color.FromArgb(30, 96, 255), Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            btnExacto.FlatAppearance.BorderSize = 0;
            btnExacto.Click += (s, e) => txtEfectivo.Text = ventaActual.Total.ToString("0.00");
            pnlBilletes.Controls.Add(btnExacto);
            Controls.Add(pnlBilletes);

            RoundedPanel pnlTotales = new RoundedPanel { Size = new Size(360, 150), Location = new Point(20, 390), BackColor = Color.FromArgb(245, 247, 250), BorderRadius = 15 };
            pnlTotales.Controls.Add(new Label { Text = "Subtotal:", Font = new Font("Segoe UI", 10), Location = new Point(15, 15), AutoSize = true });
            pnlTotales.Controls.Add(new Label { Text = $"{sym}{ventaActual.Subtotal:0.00}", Font = new Font("Segoe UI", 10), Location = new Point(260, 15), AutoSize = true });
            pnlTotales.Controls.Add(new Label { Text = "IVA:", Font = new Font("Segoe UI", 10), Location = new Point(15, 40), AutoSize = true });
            pnlTotales.Controls.Add(new Label { Text = $"{sym}{ventaActual.Iva:0.00}", Font = new Font("Segoe UI", 10), Location = new Point(260, 40), AutoSize = true });
            pnlTotales.Controls.Add(new Panel { BackColor = Color.LightGray, Height = 1, Width = 330, Location = new Point(15, 65) });
            pnlTotales.Controls.Add(new Label { Text = "Total a Cobrar:", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(15, 82), AutoSize = true });
            pnlTotales.Controls.Add(new Label { Text = $"{sym}{ventaActual.Total:0.00}", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(30, 96, 255), Location = new Point(245, 80), AutoSize = true });
            
            pnlTotales.Controls.Add(new Panel { BackColor = Color.LightGray, Height = 1, Width = 330, Location = new Point(15, 115) });
            pnlTotales.Controls.Add(new Label { Text = "Cambio:", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(15, 122), AutoSize = true });
            lblCambio = new Label { Text = $"{sym}0.00", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(245, 120), AutoSize = true };
            pnlTotales.Controls.Add(lblCambio);
            Controls.Add(pnlTotales);

            txtEfectivo.TextChanged += (s, e) => 
            {
                if (decimal.TryParse(txtEfectivo.Text, out decimal recibido))
                {
                    decimal cambio = recibido - ventaActual.Total;
                    if (cambio >= 0)
                    {
                        lblCambio.Text = $"{sym}{cambio:0.00}";
                        lblCambio.ForeColor = Color.FromArgb(16, 185, 90); // Green
                    }
                    else
                    {
                        lblCambio.Text = $"Falta {sym}{Math.Abs(cambio):0.00}";
                        lblCambio.ForeColor = Color.FromArgb(220, 50, 50); // Red
                    }
                }
                else
                {
                    lblCambio.Text = $"{sym}0.00";
                    lblCambio.ForeColor = Color.Gray;
                }
            };
            
            // Trigger calculation initially
            txtEfectivo.Text = ""; 

            RoundedButton btnCerrar = new RoundedButton { Text = "Cerrar", Size = new Size(105, 40), Location = new Point(20, 565), BackColor = Color.White, ForeColor = Color.Black, BorderColor = Color.LightGray, BorderSize = 1, BorderRadius = 10 };
            btnCerrar.Click += (s, e) => Close();
            Controls.Add(btnCerrar);

            RoundedButton btnCancelar = new RoundedButton { Text = "Cancelar Pedido", Size = new Size(130, 40), Location = new Point(135, 565), BackColor = Color.FromArgb(220, 50, 50), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), BorderRadius = 10 };
            btnCancelar.Click += BtnCancel_Click;
            Controls.Add(btnCancelar);

            RoundedButton btnConfirmar = new RoundedButton { Text = "Confirmar Venta", Size = new Size(115, 40), Location = new Point(275, 565), BackColor = Color.FromArgb(10, 170, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), BorderRadius = 10 };
            btnConfirmar.Click += BtnConfirm_Click;
            Controls.Add(btnConfirmar);
            
            // Focus on Efectivo input initially
            this.Shown += (s, e) => { txtEfectivo.Focus(); };
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (autorizarCancelacion != null && !autorizarCancelacion())
                return;

            VentaCancelada = true;
            Close();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtEfectivo.Text, out decimal recibido))
            {
                MessageBox.Show("Ingrese un monto valido en efectivo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (recibido < ventaActual.Total)
            {
                MessageBox.Show("El efectivo recibido es menor al total.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            decimal cambio = recibido - ventaActual.Total;
            MessageBox.Show($"Venta completada exitosamente.\n\nTotal: {sym}{ventaActual.Total:0.00} | Cambio: {sym}{cambio:0.00}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            VentaConfirmada = true;
            Close();
        }
    }
}
