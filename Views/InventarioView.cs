using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using STOCKPAP.Models;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class InventarioView : UserControl
    {
        private FlowLayoutPanel gridProductos;
        private ProductoRepository repo;
        private TextBox txtBuscar;
        private Label lblSubtitle;
        private string _categoriaActiva = "Todas";
        private bool puedeEditar;

        public InventarioView(bool puedeEditar = true)
        {
            this.puedeEditar = puedeEditar;
            repo = new ProductoRepository();
            InitializeComponent();
            LoadProductos();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            // ── Contenedor Superior (Título, Filtros) ───────────────────────
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 230, Padding = new Padding(30, 30, 30, 0) };
            this.Controls.Add(pnlTop);

            // ── Título ──────────────────────────────────────────────────────
            Label lblTitle = new Label
            {
                Text = "Inventario",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 40),
                AutoSize = true,
                Location = new Point(30, 20)
            };
            pnlTop.Controls.Add(lblTitle);

            lblSubtitle = new Label
            {
                Text = "Cargando...",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(35, 62)
            };
            pnlTop.Controls.Add(lblSubtitle);

            // ── Botón Agregar ────────────────────────────────────────────────
            RoundedButton btnAgregar = new RoundedButton
            {
                Text = "➕  Agregar Producto",
                Size = new Size(190, 42),
                Location = new Point(pnlTop.Width - 220, 25),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnAgregar.Click += BtnAgregar_Click;
            btnAgregar.Visible = puedeEditar;
            pnlTop.Controls.Add(btnAgregar);

            // ── Panel búsqueda + filtros ─────────────────────────────────────
            RoundedPanel panelFiltros = new RoundedPanel
            {
                Height = 110,
                Location = new Point(30, 105),
                BackColor = Color.White,
                BorderRadius = 12,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Width = pnlTop.Width - 60
            };
            pnlTop.Controls.Add(panelFiltros);

            // Icono búsqueda
            Label lblLupa = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point(15, 16)
            };
            panelFiltros.Controls.Add(lblLupa);

            txtBuscar = new TextBox
            {
                Text = "Buscar productos...",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Location = new Point(45, 15),
                Width = 720,
                BorderStyle = BorderStyle.None
            };
            txtBuscar.Enter += (s, e) =>
            {
                if (txtBuscar.Text == "Buscar productos...")
                { txtBuscar.Text = ""; txtBuscar.ForeColor = Color.Black; }
            };
            txtBuscar.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                { txtBuscar.Text = "Buscar productos..."; txtBuscar.ForeColor = Color.Gray; }
            };
            txtBuscar.TextChanged += (s, e) =>
            {
                if (txtBuscar.Text != "Buscar productos...")
                    LoadProductos(txtBuscar.Text);
            };
            Panel lineSearch = new Panel
            {
                BackColor = Color.FromArgb(220, 220, 220),
                Height = 1, Width = 740, Location = new Point(15, 42)
            };
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(lineSearch);

            // Botones de categoría
            string[] cats = { "Todas", "Cuadernos", "Escritura", "Papel", "Marcadores", "Organización", "Adhesivos", "Corte" };
            int fx = 15;
            foreach (var cat in cats)
            {
                string catLocal = cat;
                int btnW = TextRenderer.MeasureText(cat, new Font("Segoe UI", 9)).Width + 28;
                RoundedButton btnCat = new RoundedButton
                {
                    Text = cat,
                    Font = new Font("Segoe UI", 9),
                    Size = new Size(btnW, 32),
                    Location = new Point(fx, 62),
                    BackColor = cat == "Todas" ? Color.FromArgb(30, 96, 255) : Color.FromArgb(245, 247, 250),
                    ForeColor = cat == "Todas" ? Color.White : Color.FromArgb(60, 60, 60),
                    BorderColor = cat == "Todas" ? Color.Transparent : Color.FromArgb(210, 215, 225),
                    BorderSize = cat == "Todas" ? 0 : 1,
                    BorderRadius = 8,
                    Cursor = Cursors.Hand,
                    Tag = catLocal
                };
                btnCat.Click += (s, e) =>
                {
                    _categoriaActiva = catLocal;
                    // Refrescar estilos de todos los botones de cat
                    foreach (Control ctrl in panelFiltros.Controls)
                    {
                        if (ctrl is RoundedButton rb && rb.Tag is string tag &&
                            cats != null && System.Array.IndexOf(cats, tag) >= 0)
                        {
                            rb.BackColor = tag == _categoriaActiva ? Color.FromArgb(30, 96, 255) : Color.FromArgb(245, 247, 250);
                            rb.ForeColor = tag == _categoriaActiva ? Color.White : Color.FromArgb(60, 60, 60);
                            rb.BorderColor = tag == _categoriaActiva ? Color.Transparent : Color.FromArgb(210, 215, 225);
                            rb.Invalidate();
                        }
                    }
                    LoadProductos(busqueda: txtBuscar.Text == "Buscar productos..." ? "" : txtBuscar.Text);
                };
                panelFiltros.Controls.Add(btnCat);
                fx += btnW + 8;
            }

            // ── Grid de productos ────────────────────────────────────────────
            Panel pnlFill = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30, 0, 30, 30) };
            this.Controls.Add(pnlFill);
            pnlFill.BringToFront();

            gridProductos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 5, 0, 5)
            };
            pnlFill.Controls.Add(gridProductos);
        }

        // ── Carga de datos ───────────────────────────────────────────────────
        private void LoadProductos(string busqueda = "")
        {
            gridProductos.Controls.Clear();

            System.Collections.Generic.List<Producto> lista;
            if (!string.IsNullOrEmpty(busqueda))
                lista = repo.BuscarPorNombre(busqueda);
            else
                lista = repo.BuscarPorCategoria(_categoriaActiva);

            int stockBajo = 0;
            foreach (var p in lista)
                if (p.Stock <= p.StockMinimo) stockBajo++;

            lblSubtitle.Text = $"{lista.Count} producto(s)  •  {stockBajo} con stock bajo";

            if (lista.Count == 0)
            {
                Label lblVacio = new Label
                {
                    Text = "No se encontraron productos.",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(20)
                };
                gridProductos.Controls.Add(lblVacio);
                return;
            }

            foreach (var p in lista)
                gridProductos.Controls.Add(CrearCardProducto(p));
        }

        // ── Card de producto con botones Editar / Eliminar ───────────────────
        private Panel CrearCardProducto(Producto p)
        {
            RoundedPanel card = new RoundedPanel
            {
                Size = new Size(210, 310),
                BackColor = Color.White,
                BorderRadius = 14,
                Margin = new Padding(8)
            };

            // Imagen
            PictureBox pic = new PictureBox
            {
                Size = new Size(210, 140),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(245, 247, 250)
            };
            if (!string.IsNullOrEmpty(p.ImagePath))
            {
                string path = Path.Combine(Application.StartupPath, "Assets", "Images", p.ImagePath);
                if (File.Exists(path))
                {
                    try { pic.Image = Image.FromFile(path); }
                    catch { }
                }
            }
            if (pic.Image == null)
            {
                // Placeholder con inicial
                pic.Paint += (s, ev) =>
                {
                    ev.Graphics.Clear(Color.FromArgb(230, 235, 250));
                    string ini = p.Nombre.Length > 0 ? p.Nombre[0].ToString().ToUpper() : "?";
                    using (Font f = new Font("Segoe UI", 32, FontStyle.Bold))
                        ev.Graphics.DrawString(ini, f, new SolidBrush(Color.FromArgb(30, 96, 255)),
                            new RectangleF(0, 0, 210, 140),
                            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                };
            }
            card.Controls.Add(pic);

            // Nombre
            Label lblName = new Label
            {
                Text = p.Nombre,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 40),
                Location = new Point(12, 148),
                AutoSize = false,
                Size = new Size(186, 38)
            };
            card.Controls.Add(lblName);

            // Precio
            Label lblPrice = new Label
            {
                Text = $"${p.PrecioVenta:0.00}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 96, 255),
                Location = new Point(12, 188),
                AutoSize = true
            };
            card.Controls.Add(lblPrice);

            // Badge stock
            bool bajoStock = p.Stock <= p.StockMinimo;
            RoundedPanel badge = new RoundedPanel
            {
                Size = new Size(115, 22),
                Location = new Point(12, 216),
                BackColor = bajoStock ? Color.FromArgb(255, 235, 235) : Color.FromArgb(230, 250, 235),
                BorderRadius = 8
            };
            Label lblStock = new Label
            {
                Text = bajoStock ? $"⚠ Stock bajo ({p.Stock})" : $"✓ {p.Stock} unidades",
                ForeColor = bajoStock ? Color.FromArgb(180, 30, 30) : Color.FromArgb(30, 140, 60),
                Font = new Font("Segoe UI", 7, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            badge.Controls.Add(lblStock);
            card.Controls.Add(badge);

            // ── Botones Editar / Eliminar ────────────────────────────────────
            Button btnEditar = new Button
            {
                Text = "✏ Editar",
                Location = new Point(12, 246),
                Size = new Size(84, 30),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 244, 255),
                ForeColor = Color.FromArgb(30, 96, 255),
                Cursor = Cursors.Hand
            };
            btnEditar.FlatAppearance.BorderColor = Color.FromArgb(180, 200, 255);
            btnEditar.Click += (s, e) => EditarProducto(p);
            if (puedeEditar) card.Controls.Add(btnEditar);

            Button btnEliminar = new Button
            {
                Text = "🗑 Eliminar",
                Location = new Point(104, 246),
                Size = new Size(94, 30),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 240, 240),
                ForeColor = Color.FromArgb(180, 30, 30),
                Cursor = Cursors.Hand
            };
            btnEliminar.FlatAppearance.BorderColor = Color.FromArgb(255, 180, 180);
            btnEliminar.Click += (s, e) => EliminarProducto(p);
            if (puedeEditar) card.Controls.Add(btnEliminar);

            return card;
        }

        // ── Acciones CRUD ────────────────────────────────────────────────────
        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            using (var form = new ProductoForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadProductos();
            }
        }

        private void EditarProducto(Producto p)
        {
            using (var form = new ProductoForm(p))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadProductos();
            }
        }

        private void EliminarProducto(Producto p)
        {
            var res = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar el producto:\n\n\"{p.Nombre}\"?\n\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (res == DialogResult.Yes)
            {
                if (repo.EliminarProducto(p.Id))
                {
                    MessageBox.Show("Producto eliminado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProductos();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el producto.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
