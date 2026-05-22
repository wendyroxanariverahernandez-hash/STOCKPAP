using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class CheckoutForm : Form
    {
        private Venta ventaActual;
        private TextBox txtCliente;
        private TextBox txtEfectivo;
        private Button btnEfectivo;
        private Button btnTarjeta;
        private Button btnTransferencia;
        private string metodoPago = "Efectivo";

        public bool VentaConfirmada { get; private set; } = false;

        public CheckoutForm(Venta venta)
        {
            ventaActual = venta;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Completar Venta";
            this.Size = new Size(400, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            Label lblTitle = new Label { Text = "Completar Venta", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            this.Controls.Add(lblTitle);

            // Cliente
            Label lblC = new Label { Text = "Nombre del Cliente (Opcional)", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(20, 70) };
            this.Controls.Add(lblC);

            RoundedPanel pnlC = new RoundedPanel { Size = new Size(340, 40), Location = new Point(20, 100), BorderColor = Color.LightGray, BorderSize = 1, BackColor = Color.WhiteSmoke, BorderRadius = 10 };
            txtCliente = new TextBox { Text = "Ej: Juan Pérez", ForeColor = Color.Gray, Font = new Font("Segoe UI", 11), Width = 320, Location = new Point(10, 8), BorderStyle = BorderStyle.None, BackColor = Color.WhiteSmoke };
            txtCliente.Enter += (s, e) => { if (txtCliente.Text == "Ej: Juan Pérez") { txtCliente.Text = ""; txtCliente.ForeColor = Color.Black; } };
            pnlC.Controls.Add(txtCliente);
            this.Controls.Add(pnlC);

            // Metodo Pago
            Label lblM = new Label { Text = "Método de Pago", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(20, 160) };
            this.Controls.Add(lblM);

            btnEfectivo = CreatePaymentButton("Efectivo", 20, 190);
            btnTarjeta = CreatePaymentButton("Tarjeta", 135, 190);
            btnTransferencia = CreatePaymentButton("Transferencia", 250, 190);

            btnEfectivo.Click += (s, e) => SelectPayment("Efectivo", btnEfectivo);
            btnTarjeta.Click += (s, e) => SelectPayment("Tarjeta", btnTarjeta);
            btnTransferencia.Click += (s, e) => SelectPayment("Transferencia", btnTransferencia);

            this.Controls.Add(btnEfectivo); this.Controls.Add(btnTarjeta); this.Controls.Add(btnTransferencia);
            SelectPayment("Efectivo", btnEfectivo); // Default

            // Efectivo Recibido
            Label lblE = new Label { Text = "Efectivo Recibido", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(20, 290) };
            this.Controls.Add(lblE);

            RoundedPanel pnlE = new RoundedPanel { Size = new Size(340, 40), Location = new Point(20, 320), BackColor = Color.WhiteSmoke, BorderRadius = 10 };
            txtEfectivo = new TextBox { Text = "0.00", Font = new Font("Segoe UI", 11), Width = 320, Location = new Point(10, 8), BorderStyle = BorderStyle.None, BackColor = Color.WhiteSmoke };
            pnlE.Controls.Add(txtEfectivo);
            this.Controls.Add(pnlE);

            // Totals
            RoundedPanel pnlTot = new RoundedPanel { Size = new Size(340, 120), Location = new Point(20, 380), BackColor = Color.FromArgb(245, 247, 250), BorderRadius = 15 };
            pnlTot.Controls.Add(new Label { Text = "Subtotal:", Font = new Font("Segoe UI", 10), Location = new Point(15, 15), AutoSize = true });
            pnlTot.Controls.Add(new Label { Text = $"${ventaActual.Subtotal:0.00}", Font = new Font("Segoe UI", 10), Location = new Point(250, 15), AutoSize = true, TextAlign = ContentAlignment.MiddleRight });
            pnlTot.Controls.Add(new Label { Text = "IVA:", Font = new Font("Segoe UI", 10), Location = new Point(15, 40), AutoSize = true });
            pnlTot.Controls.Add(new Label { Text = $"${ventaActual.Iva:0.00}", Font = new Font("Segoe UI", 10), Location = new Point(250, 40), AutoSize = true, TextAlign = ContentAlignment.MiddleRight });
            pnlTot.Controls.Add(new Panel { BackColor = Color.LightGray, Height = 1, Width = 310, Location = new Point(15, 65) });
            pnlTot.Controls.Add(new Label { Text = "Total:", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(15, 80), AutoSize = true });
            pnlTot.Controls.Add(new Label { Text = $"${ventaActual.Total:0.00}", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(30, 96, 255), Location = new Point(230, 80), AutoSize = true, TextAlign = ContentAlignment.MiddleRight });
            this.Controls.Add(pnlTot);

            // Buttons
            RoundedButton btnCancel = new RoundedButton { Text = "Cancelar", Size = new Size(160, 45), Location = new Point(20, 510), BackColor = Color.White, ForeColor = Color.Black, BorderColor = Color.LightGray, BorderSize = 1, BorderRadius = 10 };
            btnCancel.Click += (s, e) => this.Close();

            RoundedButton btnConfirm = new RoundedButton { Text = "Confirmar Venta", Size = new Size(170, 45), Location = new Point(190, 510), BackColor = Color.FromArgb(10, 170, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), BorderRadius = 10 };
            btnConfirm.Click += BtnConfirm_Click;

            this.Controls.Add(btnCancel);
            this.Controls.Add(btnConfirm);
        }

        private Button CreatePaymentButton(string text, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(110, 80),
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.LightGray;
            return btn;
        }

        private void SelectPayment(string method, Button activeBtn)
        {
            metodoPago = method;
            btnEfectivo.BackColor = Color.White; btnEfectivo.ForeColor = Color.Black;
            btnTarjeta.BackColor = Color.White; btnTarjeta.ForeColor = Color.Black;
            btnTransferencia.BackColor = Color.White; btnTransferencia.ForeColor = Color.Black;

            activeBtn.BackColor = Color.FromArgb(10, 15, 30);
            activeBtn.ForeColor = Color.White;
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (metodoPago == "Efectivo")
            {
                if (decimal.TryParse(txtEfectivo.Text, out decimal recibido))
                {
                    if (recibido < ventaActual.Total)
                    {
                        MessageBox.Show("El efectivo recibido es menor al total.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    decimal cambio = recibido - ventaActual.Total;
                    MessageBox.Show($"¡Venta completada exitosamente!\n\nTotal: ${ventaActual.Total:0.00} | Cambio: ${cambio:0.00}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ingrese un monto válido en efectivo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show($"¡Venta completada exitosamente!\n\nTotal: ${ventaActual.Total:0.00}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            VentaConfirmada = true;
            this.Close();
        }
    }
}
