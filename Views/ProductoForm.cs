using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class ProductoForm : Form
    {
        private TextBox txtNombre;
        private ComboBox cmbCategoria;
        private NumericUpDown numPrecioCompra;
        private NumericUpDown numPrecioVenta;
        private NumericUpDown numStock;
        private NumericUpDown numStockMinimo;
        private PictureBox picPreview;
        private string selectedImagePath = "";

        public ProductoForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Nuevo Producto";
            this.Size = new Size(500, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            Label lblTitle = new Label { Text = "Agregar Producto", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            this.Controls.Add(lblTitle);

            // Nombre
            this.Controls.Add(new Label { Text = "Nombre:", Location = new Point(20, 80), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            txtNombre = new TextBox { Location = new Point(20, 105), Width = 440, Font = new Font("Segoe UI", 11) };
            this.Controls.Add(txtNombre);

            // Categoria
            this.Controls.Add(new Label { Text = "Categoría:", Location = new Point(20, 150), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            cmbCategoria = new ComboBox { Location = new Point(20, 175), Width = 440, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategoria.Items.AddRange(new string[] { "Cuadernos", "Escritura", "Papel", "Marcadores", "Organización", "Adhesivos", "Corte", "Otros" });
            cmbCategoria.SelectedIndex = 0;
            this.Controls.Add(cmbCategoria);

            // Precios
            this.Controls.Add(new Label { Text = "Precio Compra ($):", Location = new Point(20, 220), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            numPrecioCompra = new NumericUpDown { Location = new Point(20, 245), Width = 200, Font = new Font("Segoe UI", 11), DecimalPlaces = 2, Maximum = 100000 };
            this.Controls.Add(numPrecioCompra);

            this.Controls.Add(new Label { Text = "Precio Venta ($):", Location = new Point(260, 220), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            numPrecioVenta = new NumericUpDown { Location = new Point(260, 245), Width = 200, Font = new Font("Segoe UI", 11), DecimalPlaces = 2, Maximum = 100000 };
            this.Controls.Add(numPrecioVenta);

            // Stock
            this.Controls.Add(new Label { Text = "Stock Inicial:", Location = new Point(20, 290), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            numStock = new NumericUpDown { Location = new Point(20, 315), Width = 200, Font = new Font("Segoe UI", 11), Maximum = 100000 };
            this.Controls.Add(numStock);

            this.Controls.Add(new Label { Text = "Stock Mínimo:", Location = new Point(260, 290), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            numStockMinimo = new NumericUpDown { Location = new Point(260, 315), Width = 200, Font = new Font("Segoe UI", 11), Maximum = 100000, Value = 10 };
            this.Controls.Add(numStockMinimo);

            // Imagen
            this.Controls.Add(new Label { Text = "Imagen del Producto:", Location = new Point(20, 360), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            picPreview = new PictureBox { Location = new Point(20, 385), Size = new Size(150, 150), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.WhiteSmoke };
            this.Controls.Add(picPreview);

            Button btnImagen = new Button { Text = "Seleccionar Imagen...", Location = new Point(190, 385), Size = new Size(150, 40), Font = new Font("Segoe UI", 10) };
            btnImagen.Click += BtnImagen_Click;
            this.Controls.Add(btnImagen);

            // Guardar
            RoundedButton btnGuardar = new RoundedButton { Text = "Guardar", Location = new Point(150, 550), Size = new Size(200, 45), BackColor = Color.FromArgb(30, 96, 255), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), BorderRadius = 10 };
            btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(btnGuardar);
        }

        private void BtnImagen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = ofd.FileName;
                    picPreview.ImageLocation = selectedImagePath;
                }
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es requerido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string imageName = "";
            if (!string.IsNullOrEmpty(selectedImagePath) && File.Exists(selectedImagePath))
            {
                try
                {
                    string ext = Path.GetExtension(selectedImagePath);
                    imageName = Guid.NewGuid().ToString() + ext;
                    string destPath = Path.Combine(Application.StartupPath, "Assets", "Images", imageName);
                    
                    if (!Directory.Exists(Path.Combine(Application.StartupPath, "Assets", "Images")))
                        Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Assets", "Images"));
                        
                    File.Copy(selectedImagePath, destPath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al copiar imagen: " + ex.Message);
                }
            }

            Producto p = new Producto
            {
                Nombre = txtNombre.Text.Trim(),
                Categoria = cmbCategoria.SelectedItem.ToString(),
                PrecioCompra = numPrecioCompra.Value,
                PrecioVenta = numPrecioVenta.Value,
                Stock = (int)numStock.Value,
                StockMinimo = (int)numStockMinimo.Value,
                ImagePath = imageName
            };

            ProductoRepository repo = new ProductoRepository();
            if (repo.AgregarProducto(p))
            {
                MessageBox.Show("Producto agregado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al guardar producto en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
