using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;
using System.Collections.Generic;

namespace STOCKPAP.Views
{
    public class MovimientoForm : Form
    {
        private ComboBox cmbProducto;
        private ComboBox cmbTipo;
        private NumericUpDown numCantidad;
        private TextBox txtMotivo;
        private List<Producto> productos;

        public MovimientoForm()
        {
            InitializeComponent();
            LoadProductos();
        }

        private void InitializeComponent()
        {
            this.Text = "Nuevo Movimiento";
            this.Size = new Size(400, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            Label lblTitle = new Label { Text = "Registrar Movimiento", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            this.Controls.Add(lblTitle);

            // Producto
            this.Controls.Add(new Label { Text = "Producto:", Location = new Point(20, 80), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            cmbProducto = new ComboBox { Location = new Point(20, 105), Width = 340, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cmbProducto);

            // Tipo
            this.Controls.Add(new Label { Text = "Tipo de Movimiento:", Location = new Point(20, 150), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            cmbTipo = new ComboBox { Location = new Point(20, 175), Width = 340, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipo.Items.AddRange(new string[] { "Entrada", "Salida", "Ajuste" });
            cmbTipo.SelectedIndex = 0;
            this.Controls.Add(cmbTipo);

            // Cantidad
            this.Controls.Add(new Label { Text = "Cantidad:", Location = new Point(20, 220), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            numCantidad = new NumericUpDown { Location = new Point(20, 245), Width = 340, Font = new Font("Segoe UI", 11), Maximum = 100000, Minimum = 1 };
            this.Controls.Add(numCantidad);

            // Motivo
            this.Controls.Add(new Label { Text = "Motivo:", Location = new Point(20, 290), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            txtMotivo = new TextBox { Location = new Point(20, 315), Width = 340, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtMotivo);

            // Guardar
            RoundedButton btnGuardar = new RoundedButton { Text = "Guardar", Location = new Point(100, 370), Size = new Size(180, 45), BackColor = Color.FromArgb(30, 96, 255), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), BorderRadius = 10 };
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }

        private void LoadProductos()
        {
            var repo = new ProductoRepository();
            productos = repo.ObtenerTodos();
            cmbProducto.DataSource = productos;
            cmbProducto.DisplayMember = "Nombre";
            cmbProducto.ValueMember = "Id";
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbProducto.SelectedItem == null) return;

            Producto selectedProduct = (Producto)cmbProducto.SelectedItem;
            string tipo = cmbTipo.SelectedItem.ToString();
            int cant = (int)numCantidad.Value;
            
            int newStock = selectedProduct.Stock;
            if (tipo == "Entrada" || tipo == "Ajuste") newStock += cant; // Ajuste assumed positive addition or overriding? Let's treat it as addition. Wait, "Salida" subtracts.
            if (tipo == "Salida") newStock -= cant;

            if (newStock < 0)
            {
                MessageBox.Show("El stock no puede ser negativo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Movimiento m = new Movimiento
            {
                ProductoId = selectedProduct.Id,
                Tipo = tipo,
                Cantidad = tipo == "Salida" ? -cant : cant,
                StockAnterior = selectedProduct.Stock,
                StockNuevo = newStock,
                Motivo = txtMotivo.Text.Trim()
            };

            var repoMov = new MovimientoRepository();
            var repoProd = new ProductoRepository();

            if (repoMov.AgregarMovimiento(m) && repoProd.ActualizarStock(selectedProduct.Id, newStock))
            {
                MessageBox.Show("Movimiento registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
