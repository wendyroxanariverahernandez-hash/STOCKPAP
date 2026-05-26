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
        private Label lblImagenEstado;
        private string selectedImagePath = "";

        // Modo edición
        private Producto _productoEditar;
        public bool EsEdicion => _productoEditar != null;

        public ProductoForm() : this(null) { }

        public ProductoForm(Producto productoEditar)
        {
            _productoEditar = productoEditar;
            InitializeComponent();
            if (EsEdicion) CargarDatosEdicion();
        }

        private void InitializeComponent()
        {
            this.Text = EsEdicion ? "Editar Producto" : "Nuevo Producto";
            this.Size = new Size(520, 680);
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
                Text = EsEdicion ? "✏  Editar Producto" : "➕  Nuevo Producto",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // ── Campos ──────────────────────────────────────────────────────
            int y = 85;

            // Nombre
            AddLabel("Nombre del Producto:", 20, y);
            txtNombre = AddTextBox(20, y + 25, 460);
            y += 75;

            // Categoría
            AddLabel("Categoría:", 20, y);
            cmbCategoria = new ComboBox
            {
                Location = new Point(20, y + 25),
                Width = 460,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategoria.Items.AddRange(new string[] {
                "Cuadernos", "Escritura", "Papel", "Marcadores",
                "Organización", "Adhesivos", "Corte", "Otros"
            });
            cmbCategoria.SelectedIndex = 0;
            this.Controls.Add(cmbCategoria);
            y += 75;

            // Precios
            AddLabel("Precio Compra ($):", 20, y);
            numPrecioCompra = AddNumeric(20, y + 25, 210);
            AddLabel("Precio Venta ($):", 260, y);
            numPrecioVenta = AddNumeric(260, y + 25, 220);
            y += 75;

            // Stock
            AddLabel("Stock Inicial:", 20, y);
            numStock = AddNumeric(20, y + 25, 210);
            numStock.Maximum = 999999;
            AddLabel("Stock Mínimo:", 260, y);
            numStockMinimo = AddNumeric(260, y + 25, 220);
            numStockMinimo.Maximum = 999999;
            numStockMinimo.Value = 10;
            y += 75;

            // ── Imagen ──────────────────────────────────────────────────────
            AddLabel("Imagen del Producto:", 20, y);
            y += 25;

            picPreview = new PictureBox
            {
                Location = new Point(20, y),
                Size = new Size(160, 160),
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(245, 247, 250)
            };
            // Borde redondeado simulado
            picPreview.Paint += (s, ev) =>
            {
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(220, 220, 220), 2),
                    1, 1, picPreview.Width - 3, picPreview.Height - 3);
            };
            this.Controls.Add(picPreview);

            Panel pnlImgRight = new Panel
            {
                Location = new Point(195, y),
                Size = new Size(285, 160),
                BackColor = Color.Transparent
            };

            Label lblImgHint = new Label
            {
                Text = "Formatos: JPG, JPEG, PNG\nTamaño recomendado: 400x400 px",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            pnlImgRight.Controls.Add(lblImgHint);

            lblImagenEstado = new Label
            {
                Text = "Sin imagen seleccionada",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = false,
                Size = new Size(285, 35),
                Location = new Point(0, 50)
            };
            pnlImgRight.Controls.Add(lblImagenEstado);

            Button btnImagen = new Button
            {
                Text = "📁  Seleccionar Imagen",
                Location = new Point(0, 95),
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(245, 247, 250),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnImagen.FlatAppearance.BorderColor = Color.FromArgb(30, 96, 255);
            btnImagen.Click += BtnImagen_Click;
            pnlImgRight.Controls.Add(btnImagen);
            this.Controls.Add(pnlImgRight);

            y += 175;

            // ── Botones ─────────────────────────────────────────────────────
            Panel pnlBotones = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(520, 60),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(140, 10),
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
                Location = new Point(260, 10),
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

        // ── Helpers de UI ───────────────────────────────────────────────────
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

        private NumericUpDown AddNumeric(int x, int y, int width)
        {
            var n = new NumericUpDown
            {
                Location = new Point(x, y),
                Width = width,
                Font = new Font("Segoe UI", 11),
                DecimalPlaces = 2,
                Maximum = 999999
            };
            this.Controls.Add(n);
            return n;
        }

        // ── Carga de datos al editar ─────────────────────────────────────────
        private void CargarDatosEdicion()
        {
            txtNombre.Text = _productoEditar.Nombre;
            int idx = cmbCategoria.Items.IndexOf(_productoEditar.Categoria);
            if (idx >= 0) cmbCategoria.SelectedIndex = idx;
            numPrecioCompra.Value = _productoEditar.PrecioCompra;
            numPrecioVenta.Value  = _productoEditar.PrecioVenta;
            numStock.Value        = _productoEditar.Stock;
            numStockMinimo.Value  = _productoEditar.StockMinimo;

            if (!string.IsNullOrEmpty(_productoEditar.ImagePath))
            {
                string path = Path.Combine(Application.StartupPath, "Assets", "Images", _productoEditar.ImagePath);
                if (File.Exists(path))
                {
                    picPreview.Image = Image.FromFile(path);
                    lblImagenEstado.Text = _productoEditar.ImagePath;
                    selectedImagePath = "__keep__"; // señal de que no se reemplaza
                }
            }
        }

        // ── Eventos ─────────────────────────────────────────────────────────
        private void BtnImagen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar imagen del producto";
                ofd.Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = ofd.FileName;
                    picPreview.Image = Image.FromFile(selectedImagePath);
                    lblImagenEstado.Text = Path.GetFileName(selectedImagePath);
                    lblImagenEstado.ForeColor = Color.FromArgb(30, 96, 255);
                }
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre del producto es requerido.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            string imageName = EsEdicion ? (_productoEditar.ImagePath ?? "") : "";

            // Si se seleccionó una nueva imagen (y no es la señal __keep__)
            if (!string.IsNullOrEmpty(selectedImagePath) && selectedImagePath != "__keep__" && File.Exists(selectedImagePath))
            {
                try
                {
                    string ext = Path.GetExtension(selectedImagePath);
                    imageName = Guid.NewGuid().ToString("N") + ext;
                    string destDir = Path.Combine(Application.StartupPath, "Assets", "Images");
                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);
                    File.Copy(selectedImagePath, Path.Combine(destDir, imageName), overwrite: true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al copiar la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Producto p = new Producto
            {
                Id           = EsEdicion ? _productoEditar.Id : 0,
                Nombre       = txtNombre.Text.Trim(),
                Categoria    = cmbCategoria.SelectedItem?.ToString() ?? "Otros",
                PrecioCompra = numPrecioCompra.Value,
                PrecioVenta  = numPrecioVenta.Value,
                Stock        = (int)numStock.Value,
                StockMinimo  = (int)numStockMinimo.Value,
                ImagePath    = imageName
            };

            ProductoRepository repo = new ProductoRepository();
            bool ok = EsEdicion ? repo.ActualizarProducto(p) : repo.AgregarProducto(p);

            if (ok)
            {
                string msg = EsEdicion ? "Producto actualizado correctamente." : "Producto agregado con éxito.";
                MessageBox.Show(msg, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al guardar en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
