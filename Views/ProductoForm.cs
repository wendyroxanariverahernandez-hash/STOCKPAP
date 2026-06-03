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
        private TextBox txtCodigoBarras;
        private ComboBox cmbCategoria;
        private ComboBox cmbClasificacion;
        private ComboBox cmbDetalle;
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
            this.Size = new Size(540, 750);
            this.MinimumSize = new Size(540, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.BackColor = Color.White;

            // ── Encabezado ──────────────────────────────────────────────────
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.FromArgb(30, 96, 255)
            };
            Label lblTitle = new Label
            {
                Text = EsEdicion ? "✏  Editar Producto" : "➕  Nuevo Producto",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 14)
            };
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // ── Botones (FOOTER fijo abajo) ─────────────────────────────────
            Panel pnlBotones = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(20, 10, 20, 10)
            };

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Size = new Size(100, 40),
                Location = new Point(200, 10),
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnCancelar.Click += (s, ev) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            RoundedButton btnGuardar = new RoundedButton
            {
                Text = EsEdicion ? "💾  Actualizar" : "💾  Guardar",
                Size = new Size(160, 40),
                Location = new Point(320, 10),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 8,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnGuardar.Click += BtnGuardar_Click;

            pnlBotones.Controls.Add(btnCancelar);
            pnlBotones.Controls.Add(btnGuardar);
            this.Controls.Add(pnlBotones);

            // ── Scrollable content panel ────────────────────────────────────
            Panel pnlScroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(0, 10, 0, 10),
                BackColor = Color.White
            };
            this.Controls.Add(pnlScroll);
            pnlScroll.BringToFront();

            // All fields go into this inner panel
            int contentWidth = 480;
            int y = 15;

            // Nombre
            AddLabel(pnlScroll, "Nombre del Producto:", 20, y);
            txtNombre = AddTextBox(pnlScroll, 20, y + 22, contentWidth - 20);
            y += 65;

            // Código de barras
            AddLabel(pnlScroll, "Código de Barras (escribe o escanea):", 20, y);
            txtCodigoBarras = AddTextBox(pnlScroll, 20, y + 22, 310);
            Button btnEscanearCodigo = new Button
            {
                Text = "📷 Escanear",
                Location = new Point(345, y + 20),
                Size = new Size(135, 32),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEscanearCodigo.FlatAppearance.BorderSize = 0;
            btnEscanearCodigo.Click += BtnEscanearCodigo_Click;
            pnlScroll.Controls.Add(btnEscanearCodigo);
            y += 65;

            // Categoría
            AddLabel(pnlScroll, "Categoría:", 20, y);
            cmbCategoria = new ComboBox
            {
                Location = new Point(20, y + 22),
                Width = contentWidth - 65,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDown
            };
            cmbCategoria.Items.AddRange(ProductCategories.Merge(new ProductoRepository().ObtenerCategorias()));
            cmbCategoria.SelectedIndex = 0;
            cmbCategoria.SelectedIndexChanged += (s, e) => ActualizarClasificaciones();
            cmbCategoria.TextChanged += (s, e) => ActualizarClasificaciones();
            pnlScroll.Controls.Add(cmbCategoria);

            Button btnAddCategoria = CreatePlusButton(contentWidth - 35, y + 21);
            btnAddCategoria.Click += (s, e) =>
            {
                string nueva = PromptDialog.Mostrar("Nueva Categoría", "Ingresa el nombre de la nueva categoría:");
                if (!string.IsNullOrWhiteSpace(nueva) && !cmbCategoria.Items.Contains(nueva))
                {
                    cmbCategoria.Items.Add(nueva);
                    cmbCategoria.SelectedItem = nueva;
                }
            };
            pnlScroll.Controls.Add(btnAddCategoria);
            y += 65;

            // Clasificación
            AddLabel(pnlScroll, "Clasificación:", 20, y);
            cmbClasificacion = new ComboBox
            {
                Location = new Point(20, y + 22),
                Width = contentWidth - 65,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDown
            };
            cmbClasificacion.SelectedIndexChanged += (s, e) => ActualizarDetalles();
            cmbClasificacion.TextChanged += (s, e) => ActualizarDetalles();
            pnlScroll.Controls.Add(cmbClasificacion);

            Button btnAddClasificacion = CreatePlusButton(contentWidth - 35, y + 21);
            btnAddClasificacion.Click += (s, e) =>
            {
                string nueva = PromptDialog.Mostrar("Nueva Clasificación", "Ingresa el nombre de la nueva clasificación:");
                if (!string.IsNullOrWhiteSpace(nueva) && !cmbClasificacion.Items.Contains(nueva))
                {
                    cmbClasificacion.Items.Add(nueva);
                    cmbClasificacion.SelectedItem = nueva;
                }
            };
            pnlScroll.Controls.Add(btnAddClasificacion);
            y += 65;

            // Detalle / Tipo
            AddLabel(pnlScroll, "Detalle / Tipo:", 20, y);
            cmbDetalle = new ComboBox
            {
                Location = new Point(20, y + 22),
                Width = contentWidth - 65,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDown
            };
            pnlScroll.Controls.Add(cmbDetalle);

            Button btnAddDetalle = CreatePlusButton(contentWidth - 35, y + 21);
            btnAddDetalle.Click += (s, e) =>
            {
                string nueva = PromptDialog.Mostrar("Nuevo Tipo", "Ingresa el nombre del nuevo tipo o detalle:");
                if (!string.IsNullOrWhiteSpace(nueva) && !cmbDetalle.Items.Contains(nueva))
                {
                    cmbDetalle.Items.Add(nueva);
                    cmbDetalle.SelectedItem = nueva;
                }
            };
            pnlScroll.Controls.Add(btnAddDetalle);
            y += 65;

            // Precios
            AddLabel(pnlScroll, "Precio Compra ($):", 20, y);
            AddLabel(pnlScroll, "Precio Venta ($):", 255, y);
            numPrecioCompra = AddNumeric(pnlScroll, 20, y + 22, 210);
            numPrecioVenta = AddNumeric(pnlScroll, 255, y + 22, 210);
            y += 65;

            // Stock
            AddLabel(pnlScroll, "Stock Inicial:", 20, y);
            AddLabel(pnlScroll, "Stock Mínimo:", 255, y);
            numStock = AddNumeric(pnlScroll, 20, y + 22, 210);
            numStock.Maximum = 999999;
            numStockMinimo = AddNumeric(pnlScroll, 255, y + 22, 210);
            numStockMinimo.Maximum = 999999;
            numStockMinimo.Value = 10;
            y += 65;

            // ── Imagen ──────────────────────────────────────────────────────
            AddLabel(pnlScroll, "Imagen del Producto:", 20, y);
            y += 22;

            picPreview = new PictureBox
            {
                Location = new Point(20, y),
                Size = new Size(130, 130),
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(245, 247, 250)
            };
            picPreview.Paint += (s, ev) =>
            {
                ev.Graphics.DrawRectangle(new Pen(Color.FromArgb(220, 220, 220), 2),
                    1, 1, picPreview.Width - 3, picPreview.Height - 3);
            };
            pnlScroll.Controls.Add(picPreview);

            Panel pnlImgRight = new Panel
            {
                Location = new Point(165, y),
                Size = new Size(300, 130),
                BackColor = Color.Transparent
            };

            Label lblImgHint = new Label
            {
                Text = "Formatos: JPG, JPEG, PNG\nTamaño recomendado: 400x400 px",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            pnlImgRight.Controls.Add(lblImgHint);

            lblImagenEstado = new Label
            {
                Text = "Sin imagen seleccionada",
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = false,
                Size = new Size(285, 30),
                Location = new Point(0, 40)
            };
            pnlImgRight.Controls.Add(lblImagenEstado);

            Button btnImagen = new Button
            {
                Text = "📁 Seleccionar Imagen",
                Location = new Point(0, 80),
                Size = new Size(170, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(245, 247, 250),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnImagen.FlatAppearance.BorderColor = Color.FromArgb(30, 96, 255);
            btnImagen.Click += BtnImagen_Click;
            pnlImgRight.Controls.Add(btnImagen);
            pnlScroll.Controls.Add(pnlImgRight);

            y += 145;

            // Add a spacer at the bottom to ensure scrolling works properly
            Panel spacer = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(10, 10),
                BackColor = Color.Transparent
            };
            pnlScroll.Controls.Add(spacer);

            ActualizarClasificaciones();
        }

        // ── Helper: Create "+" button ───────────────────────────────────────
        private Button CreatePlusButton(int x, int y)
        {
            Button btn = new Button
            {
                Text = "+",
                Location = new Point(x, y),
                Size = new Size(35, 30),
                BackColor = Color.FromArgb(245, 247, 250),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            return btn;
        }

        // ── Helpers de UI ───────────────────────────────────────────────────
        private void AddLabel(Panel container, string text, int x, int y)
        {
            container.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Location = new Point(x, y)
            });
        }

        private TextBox AddTextBox(Panel container, int x, int y, int width)
        {
            var tb = new TextBox
            {
                Location = new Point(x, y),
                Width = width,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            container.Controls.Add(tb);
            return tb;
        }

        private NumericUpDown AddNumeric(Panel container, int x, int y, int width)
        {
            var n = new NumericUpDown
            {
                Location = new Point(x, y),
                Width = width,
                Font = new Font("Segoe UI", 11),
                DecimalPlaces = 2,
                Maximum = 999999
            };
            container.Controls.Add(n);
            return n;
        }

        private void ActualizarClasificaciones()
        {
            if (cmbClasificacion == null)
                return;

            string valorActual = cmbClasificacion.Text;
            cmbClasificacion.Items.Clear();
            cmbClasificacion.Items.AddRange(ProductCategories.MergeClassifications(cmbCategoria.Text, new ProductoRepository().ObtenerClasificaciones(cmbCategoria.Text)));

            int idx = cmbClasificacion.Items.IndexOf(valorActual);
            if (idx >= 0)
                cmbClasificacion.SelectedIndex = idx;
            else if (cmbClasificacion.Items.Count > 0 && string.IsNullOrWhiteSpace(valorActual))
                cmbClasificacion.SelectedIndex = 0;
            else
                cmbClasificacion.Text = valorActual;

            ActualizarDetalles();
        }

        private void ActualizarDetalles()
        {
            if (cmbDetalle == null)
                return;

            string valorActual = cmbDetalle.Text;
            cmbDetalle.Items.Clear();
            cmbDetalle.Items.AddRange(ProductCategories.MergeDetails(cmbCategoria.Text, cmbClasificacion.Text, new ProductoRepository().ObtenerDetalles(cmbCategoria.Text, cmbClasificacion.Text)));

            int idx = cmbDetalle.Items.IndexOf(valorActual);
            if (idx >= 0)
                cmbDetalle.SelectedIndex = idx;
            else if (cmbDetalle.Items.Count > 0 && string.IsNullOrWhiteSpace(valorActual))
                cmbDetalle.SelectedIndex = 0;
            else
                cmbDetalle.Text = valorActual;
        }

        // ── Carga de datos al editar ─────────────────────────────────────────
        private void CargarDatosEdicion()
        {
            txtNombre.Text = _productoEditar.Nombre;
            txtCodigoBarras.Text = _productoEditar.CodigoBarras;
            int idx = cmbCategoria.Items.IndexOf(_productoEditar.Categoria);
            if (idx >= 0) cmbCategoria.SelectedIndex = idx;
            else cmbCategoria.Text = _productoEditar.Categoria;
            ActualizarClasificaciones();
            int idxClasificacion = cmbClasificacion.Items.IndexOf(_productoEditar.Clasificacion);
            if (idxClasificacion >= 0) cmbClasificacion.SelectedIndex = idxClasificacion;
            else cmbClasificacion.Text = _productoEditar.Clasificacion;
            ActualizarDetalles();
            int idxDetalle = cmbDetalle.Items.IndexOf(_productoEditar.Detalle);
            if (idxDetalle >= 0) cmbDetalle.SelectedIndex = idxDetalle;
            else cmbDetalle.Text = _productoEditar.Detalle;
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

        private void BtnEscanearCodigo_Click(object sender, EventArgs e)
        {
            using (var scanner = new BarcodeScannerForm())
            {
                if (scanner.ShowDialog(this) == DialogResult.OK)
                    txtCodigoBarras.Text = scanner.CodigoDetectado;
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
                Categoria    = string.IsNullOrWhiteSpace(cmbCategoria.Text) ? "Otros" : cmbCategoria.Text.Trim(),
                Clasificacion = string.IsNullOrWhiteSpace(cmbClasificacion.Text) ? "General" : cmbClasificacion.Text.Trim(),
                Detalle      = string.IsNullOrWhiteSpace(cmbDetalle.Text) ? "General" : cmbDetalle.Text.Trim(),
                CodigoBarras = txtCodigoBarras.Text.Trim(),
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
