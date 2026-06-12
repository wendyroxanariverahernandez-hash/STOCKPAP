using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using STOCKPAP.Models;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class ProductoForm : Form
    {
        private Label lblTitle;
        private TextBox txtNombre;
        private TextBox txtCodigoBarras;
        
        // Removed unused Categoria, Clasificacion, Detalle variables

        // Nuevos campos
        private ComboBox cmbClase;
        private ComboBox cmbSubclase;
        private ComboBox cmbMarca;
        private ComboBox cmbProveedor;

        // Precios y Stock
        private NumericUpDown numPrecioCompra;
        private NumericUpDown numPrecioVenta;
        private NumericUpDown numStock;
        private NumericUpDown numCantidadAgregar;
        private PictureBox picPreview;
        private Label lblImagenEstado;
        private string selectedImagePath = "";

        // Botones
        private Button btnNuevo;
        private Button btnEditar;
        private Button btnEliminar;
        private Button btnGuardar;
        private Button btnCancelar;

        private ProductoRepository repo;
        private ProveedorRepository repoProveedor;
        private ClasificacionRepository repoClasificacion;
        private Producto _productoEditar;
        public bool EsEdicion => _productoEditar != null;

        public ProductoForm() : this(null) { }

        public ProductoForm(Producto productoEditar)
        {
            _productoEditar = productoEditar;
            repo = new ProductoRepository();
            repoProveedor = new ProveedorRepository();
            repoClasificacion = new ClasificacionRepository();
            InitializeComponent();
            CargarProveedores();
            CargarListasAuxiliares();
            if (EsEdicion)
            {
                CargarDatosEdicion();
            }
            else
            {
                LimpiarFormulario();
            }
        }

        private void InitializeComponent()
        {
            this.Text = EsEdicion ? "Editar Producto" : "Nuevo Producto";
            this.Size = new Size(540, 780);
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
            lblTitle = new Label
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
                Height = 70, // Increased height for larger buttons
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(15, 10, 15, 10)
            };

            btnNuevo = new Button
            {
                Text = "Nuevo",
                Size = new Size(80, 40),
                Location = new Point(15, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 96, 255),
                Cursor = Cursors.Hand
            };
            btnNuevo.FlatAppearance.BorderColor = Color.FromArgb(30, 96, 255);
            btnNuevo.Click += BtnNuevo_Click;

            btnEditar = new Button
            {
                Text = "Editar",
                Size = new Size(80, 40),
                Location = new Point(105, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 96, 255),
                Cursor = Cursors.Hand
            };
            btnEditar.FlatAppearance.BorderColor = Color.FromArgb(30, 96, 255);
            btnEditar.Click += BtnEditar_Click;

            btnEliminar = new Button
            {
                Text = "Eliminar",
                Size = new Size(100, 45), // Larger size
                Location = new Point(15, 12),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 240, 240),
                ForeColor = Color.FromArgb(180, 30, 30),
                Cursor = Cursors.Hand
            };
            btnEliminar.FlatAppearance.BorderColor = Color.FromArgb(255, 180, 180);
            btnEliminar.Click += BtnEliminar_Click;
            
            btnNuevo.Visible = false;
            btnEditar.Visible = false;
            btnEliminar.Visible = false;

            btnGuardar = new RoundedButton
            {
                Text = EsEdicion ? "Actualizar" : "Guardar",
                Size = new Size(150, 45), // Larger size
                Location = new Point(135, 12), // Centered slightly right to avoid Eliminar
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold), // Larger font
                BorderRadius = 8,
                Cursor = Cursors.Hand
            };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Size = new Size(150, 45), // Larger size
                Location = new Point(300, 12), // Next to Guardar
                Font = new Font("Segoe UI", 11), // Larger font
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCancelar.Click += (s, ev) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            pnlBotones.Controls.Add(btnNuevo);
            pnlBotones.Controls.Add(btnEditar);
            pnlBotones.Controls.Add(btnEliminar);
            pnlBotones.Controls.Add(btnGuardar);
            pnlBotones.Controls.Add(btnCancelar);
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

            int contentWidth = 480;
            int y = 15;

            // Nombre
            AddLabel(pnlScroll, "Nombre del Producto:", 20, y);
            txtNombre = AddTextBox(pnlScroll, 20, y + 22, contentWidth - 20);
            y += 75;

            // Código de barras
            AddLabel(pnlScroll, "Código de Barras (escribe o escanea):", 20, y);
            txtCodigoBarras = AddTextBox(pnlScroll, 20, y + 22, 310);
            txtCodigoBarras.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) {
                    e.SuppressKeyPress = true;
                    CheckBarcode();
                }
            };
            txtCodigoBarras.Leave += (s, e) => CheckBarcode();
            y += 75;

            // ── Nuevos Filtros organizacionales (Clase, Subclase, Marca, Tipo) ──
            AddLabel(pnlScroll, "Clase de Producto:", 20, y);
            cmbClase = AddComboBox(pnlScroll, 20, y + 22, contentWidth - 20);
            cmbClase.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbClase.SelectedIndexChanged += CmbClase_SelectedIndexChanged;
            y += 75;

            AddLabel(pnlScroll, "Subclase:", 20, y);
            cmbSubclase = AddComboBox(pnlScroll, 20, y + 22, contentWidth - 20);
            cmbSubclase.DropDownStyle = ComboBoxStyle.DropDownList;
            y += 75;

            AddLabel(pnlScroll, "Marca:", 20, y);
            cmbMarca = AddComboBox(pnlScroll, 20, y + 22, contentWidth - 20);
            cmbMarca.DropDownStyle = ComboBoxStyle.DropDownList;
            y += 75;

            // Precios y Costos
            string moneda = ConfigHelper.Obtener("Moneda", "MXN");
            string sym = moneda.Contains("USD") ? "USD$" : moneda.Contains("EUR") ? "€" : "$";

            AddLabel(pnlScroll, $"Precio de Compra ({sym}):", 20, y);
            numPrecioCompra = AddNumeric(pnlScroll, 20, y + 22, 210);
            AddLabel(pnlScroll, $"Precio Venta ({sym}):", 255, y);
            numPrecioVenta = AddNumeric(pnlScroll, 255, y + 22, 210);
            y += 75;

            // Stock
            AddLabel(pnlScroll, "Stock Actual:", 20, y);
            AddLabel(pnlScroll, "Cantidad a Agregar:", 255, y);
            numStock = AddNumeric(pnlScroll, 20, y + 22, 210);
            numStock.Enabled = false; // Mostrar inventario actual
            numCantidadAgregar = AddNumeric(pnlScroll, 255, y + 22, 210);
            numCantidadAgregar.Value = 0;
            y += 75;

            // Proveedor
            AddLabel(pnlScroll, "Proveedor Asociado:", 20, y);
            cmbProveedor = AddComboBox(pnlScroll, 20, y + 22, contentWidth - 20);
            y += 75;

            // Removed hidden compatibility fields

            // Imagen
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

            // Spacer
            Panel spacer = new Panel { Location = new Point(0, y), Size = new Size(10, 10), BackColor = Color.Transparent };
            pnlScroll.Controls.Add(spacer);
        }

        private void AddLabel(Panel container, string text, int x, int y)
        {
            container.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(x, y),
                AutoSize = true
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

        private ComboBox AddComboBox(Panel container, int x, int y, int width)
        {
            var cb = new ComboBox
            {
                Location = new Point(x, y),
                Width = width,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDown
            };
            container.Controls.Add(cb);
            return cb;
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

        private Button CreatePlusButton(Panel container, int x, int y)
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
            container.Controls.Add(btn);
            return btn;
        }

        private void CmbClase_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbSubclase.Items.Clear();
            cmbMarca.Items.Clear();
            if (cmbClase.SelectedItem is Clase clase)
            {
                var subclases = repoClasificacion.ObtenerSubclases(clase.Id);
                foreach (var s in subclases) cmbSubclase.Items.Add(s);

                var marcas = repoClasificacion.ObtenerMarcas(clase.Id);
                foreach (var m in marcas) cmbMarca.Items.Add(m);
            }
        }

        private bool isScanningExisting = false;
        private void CheckBarcode()
        {
            string code = txtCodigoBarras.Text.Trim();
            if (string.IsNullOrEmpty(code)) return;
            var prod = repo.BuscarPorCodigoBarras(code);
            if (prod != null && (_productoEditar == null || _productoEditar.Id != prod.Id))
            {
                _productoEditar = prod;
                isScanningExisting = true;
                CargarDatosEdicion();
            }
        }

        private void CargarProveedores()
        {
            cmbProveedor.Items.Clear();
            var provs = repoProveedor.ObtenerTodos();
            foreach (var prov in provs)
            {
                cmbProveedor.Items.Add(new ProveedorItem { Id = prov.Id, Empresa = prov.Empresa });
            }
        }

        private void CargarListasAuxiliares()
        {
            cmbClase.Items.Clear();
            var clases = repoClasificacion.ObtenerClases();
            foreach (var c in clases) cmbClase.Items.Add(c);
        }

        private void CargarDatosEdicion()
        {
            btnEliminar.Enabled = true;
            btnEliminar.Visible = true;
            btnGuardar.Text = "Actualizar";
            lblTitle.Text = "✏  Editar Producto";
            this.Text = "Editar Producto";

            txtNombre.Text = _productoEditar.Nombre;
            txtCodigoBarras.Text = _productoEditar.CodigoBarras;

            for (int i = 0; i < cmbClase.Items.Count; i++)
            {
                if (cmbClase.Items[i].ToString() == _productoEditar.Clase)
                {
                    cmbClase.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < cmbSubclase.Items.Count; i++)
            {
                if (cmbSubclase.Items[i].ToString() == _productoEditar.Subclase)
                {
                    cmbSubclase.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < cmbMarca.Items.Count; i++)
            {
                if (cmbMarca.Items[i].ToString() == _productoEditar.Marca)
                {
                    cmbMarca.SelectedIndex = i;
                    break;
                }
            }
            
            numPrecioCompra.Value = _productoEditar.PrecioCompra;
            numPrecioVenta.Value = _productoEditar.PrecioVenta;
            numStock.Value = _productoEditar.Stock;
            if (isScanningExisting) {
                numCantidadAgregar.Value = 1;
            } else {
                numCantidadAgregar.Value = 0;
            }

            // Seleccionar Proveedor
            if (_productoEditar.ProveedorId.HasValue)
            {
                foreach (object item in cmbProveedor.Items)
                {
                    if (item is ProveedorItem p && p.Id == _productoEditar.ProveedorId.Value)
                    {
                        cmbProveedor.SelectedItem = p;
                        break;
                    }
                }
            }
            else
            {
                cmbProveedor.SelectedIndex = -1;
            }

            if (!string.IsNullOrEmpty(_productoEditar.ImagePath))
            {
                string path = Path.Combine(Application.StartupPath, "Assets", "Images", _productoEditar.ImagePath);
                if (File.Exists(path))
                {
                    picPreview.Image = Image.FromFile(path);
                    lblImagenEstado.Text = _productoEditar.ImagePath;
                    selectedImagePath = "__keep__";
                }
            }
        }

        private void LimpiarFormulario()
        {
            btnEliminar.Enabled = false;
            btnGuardar.Text = "Guardar";
            lblTitle.Text = "➕  Nuevo Producto";
            this.Text = "Nuevo Producto";

            txtNombre.Text = "";
            txtCodigoBarras.Text = "";

            cmbClase.Text = "General";
            cmbSubclase.Text = "General";
            cmbMarca.Text = "General";

            numPrecioCompra.Value = 0;
            numPrecioVenta.Value = 0;
            numStock.Value = 0;
            if (numCantidadAgregar != null) numCantidadAgregar.Value = 0;
            cmbProveedor.SelectedIndex = -1;

            picPreview.Image = null;
            lblImagenEstado.Text = "Sin imagen seleccionada";
            lblImagenEstado.ForeColor = Color.Gray;
            selectedImagePath = "";
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            _productoEditar = null;
            LimpiarFormulario();
            txtNombre.Focus();
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            // La edición siempre está habilitada, pero por seguridad, podemos re-confirmar el estado o enfocar el Nombre.
            txtNombre.Focus();
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (_productoEditar == null) return;

            var res = MessageBox.Show(
                $"¿Seguro que deseas eliminar el producto:\n\n\"{_productoEditar.Nombre}\"?\n\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (res == DialogResult.Yes)
            {
                if (repo.EliminarProducto(_productoEditar.Id))
                {
                    MessageBox.Show("Producto eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



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

        private void BtnSaveLogic()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre del producto es requerido.", "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            string imageName = EsEdicion ? (_productoEditar.ImagePath ?? "") : "";

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

            int? provId = null;
            if (cmbProveedor.SelectedItem is ProveedorItem pItem)
            {
                provId = pItem.Id;
            }

            string clase = string.IsNullOrWhiteSpace(cmbClase.Text) ? "General" : cmbClase.Text.Trim();
            string subclase = string.IsNullOrWhiteSpace(cmbSubclase.Text) ? "General" : cmbSubclase.Text.Trim();
            int newStock = (int)(numStock.Value + numCantidadAgregar.Value);

            Producto p = new Producto
            {
                Id = EsEdicion ? _productoEditar.Id : 0,
                Nombre = txtNombre.Text.Trim(),

                CodigoBarras = txtCodigoBarras.Text.Trim(),
                PrecioCompra = numPrecioCompra.Value,
                PrecioVenta = numPrecioVenta.Value,
                Stock = newStock,
                ImagePath = imageName,

                Clase = clase,
                Subclase = subclase,
                Marca = string.IsNullOrWhiteSpace(cmbMarca.Text) ? "General" : cmbMarca.Text.Trim(),
                ProveedorId = provId,
                StockMinimo = int.Parse(ConfigHelper.Obtener("StockMinimoGlobal", "10"))
            };

            bool ok = EsEdicion ? repo.ActualizarProducto(p) : repo.AgregarProducto(p);

            if (ok)
            {
                string msg = EsEdicion ? "Producto actualizado correctamente." : "Producto guardado con éxito.";
                MessageBox.Show(msg, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al guardar en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            BtnSaveLogic();
        }

        // Estructura auxiliar para guardar proveedores en ComboBox
        private class ProveedorItem
        {
            public int Id { get; set; }
            public string Empresa { get; set; }

            public override string ToString()
            {
                return Empresa;
            }
        }
    }
}
