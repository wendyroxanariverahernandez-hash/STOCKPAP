using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;
using STOCKPAP.Models;

namespace STOCKPAP.Views
{
    public class InventarioView : UserControl
    {
        private FlowLayoutPanel gridProductos;
        private ProductoRepository repo;
        private TextBox txtBuscar;

        public InventarioView()
        {
            repo = new ProductoRepository();
            InitializeComponent();
            LoadProductos();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            // Título
            Label lblTitle = new Label
            {
                Text = "Inventario",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 30)
            };
            this.Controls.Add(lblTitle);

            // Subtítulo
            Label lblSubtitle = new Label
            {
                Text = "12 productos • 1 con stock bajo",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(35, 75)
            };
            this.Controls.Add(lblSubtitle);

            // Botón Agregar
            RoundedButton btnAgregar = new RoundedButton
            {
                Text = "+  Agregar Producto",
                Size = new Size(180, 45),
                Location = new Point(this.Width - 210, 30),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.Controls.Add(btnAgregar);

            // Barra de búsqueda y filtros
            RoundedPanel panelFiltros = new RoundedPanel
            {
                Size = new Size(800, 110),
                Location = new Point(30, 120),
                BackColor = Color.White,
                BorderRadius = 15,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelFiltros);

            txtBuscar = new TextBox
            {
                Text = "Buscar productos...",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                Location = new Point(20, 20),
                Width = 760,
                BorderStyle = BorderStyle.None
            };
            txtBuscar.Enter += (s, e) => { if (txtBuscar.Text == "Buscar productos...") { txtBuscar.Text = ""; txtBuscar.ForeColor = Color.Black; } };
            txtBuscar.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtBuscar.Text)) { txtBuscar.Text = "Buscar productos..."; txtBuscar.ForeColor = Color.Gray; } };
            txtBuscar.TextChanged += (s, e) => { if(txtBuscar.Text != "Buscar productos...") LoadProductos(txtBuscar.Text); };
            
            Panel lineSearch = new Panel { BackColor = Color.LightGray, Height = 1, Width = 760, Location = new Point(20, 45) };
            panelFiltros.Controls.Add(txtBuscar);
            panelFiltros.Controls.Add(lineSearch);

            // Filtros
            string[] cats = { "Todas", "Cuadernos", "Escritura", "Papel", "Marcadores", "Organización", "Adhesivos", "Corte" };
            int fx = 20;
            foreach(var cat in cats)
            {
                RoundedButton btnCat = new RoundedButton
                {
                    Text = cat,
                    Font = new Font("Segoe UI", 9),
                    Size = new Size(cat == "Todas" ? 70 : TextRenderer.MeasureText(cat, new Font("Segoe UI", 9)).Width + 30, 35),
                    Location = new Point(fx, 60),
                    BackColor = cat == "Todas" ? Color.FromArgb(30, 96, 255) : Color.White,
                    ForeColor = cat == "Todas" ? Color.White : Color.Black,
                    BorderColor = cat == "Todas" ? Color.Transparent : Color.LightGray,
                    BorderSize = cat == "Todas" ? 0 : 1,
                    BorderRadius = 10
                };
                panelFiltros.Controls.Add(btnCat);
                fx += btnCat.Width + 10;
            }

            // Grid de productos
            gridProductos = new FlowLayoutPanel
            {
                Location = new Point(30, 250),
                Size = new Size(800, 500),
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };
            this.Controls.Add(gridProductos);
        }

        private void LoadProductos(string filtro = "")
        {
            gridProductos.Controls.Clear();
            var lista = string.IsNullOrEmpty(filtro) ? repo.ObtenerTodos() : repo.BuscarPorNombre(filtro);

            foreach (var p in lista)
            {
                RoundedPanel card = new RoundedPanel
                {
                    Size = new Size(200, 280),
                    BackColor = Color.White,
                    BorderRadius = 15,
                    Margin = new Padding(10)
                };

                PictureBox pic = new PictureBox
                {
                    Size = new Size(200, 150),
                    Location = new Point(0, 0),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.LightGray
                };
                string path = Path.Combine(Application.StartupPath, "Assets", "Images", p.ImagePath);
                if (File.Exists(path)) pic.Image = Image.FromFile(path);
                
                Label lblName = new Label
                {
                    Text = p.Nombre,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(15, 160),
                    AutoSize = false,
                    Size = new Size(170, 40)
                };

                Label lblPrice = new Label
                {
                    Text = $"${p.PrecioVenta:0.00}",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 96, 255),
                    Location = new Point(15, 205),
                    AutoSize = true
                };

                RoundedPanel stockBadge = new RoundedPanel
                {
                    Size = new Size(100, 25),
                    Location = new Point(15, 240),
                    BackColor = p.Stock <= p.StockMinimo ? Color.FromArgb(255, 235, 238) : Color.FromArgb(30, 30, 30),
                    BorderRadius = 10
                };
                Label lblStock = new Label
                {
                    Text = p.Stock <= p.StockMinimo ? "Stock Bajo" : $"{p.Stock} unidades",
                    ForeColor = p.Stock <= p.StockMinimo ? Color.Red : Color.White,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                stockBadge.Controls.Add(lblStock);

                card.Controls.Add(pic);
                card.Controls.Add(lblName);
                card.Controls.Add(lblPrice);
                card.Controls.Add(stockBadge);

                gridProductos.Controls.Add(card);
            }
        }
    }
}
