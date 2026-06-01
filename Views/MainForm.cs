using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class MainForm : Form
    {
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Usuario currentUser;
        private bool isSidebarExpanded = true;
        private Label lblLogo;
        private Label lblSubLogo;
        private Label lblUser;
        private Label lblDate;
        private Button btnToggle;
        private Button btnLogout;

        public MainForm(Usuario user)
        {
            currentUser = user;
            InitializeComponent();
            LoadView(new VentasView(currentUser)); // Default view
        }

        private void InitializeComponent()
        {
            this.Text = "StockPap";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Sidebar
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.White
            };
            
            // Content Panel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            // It's important to add contentPanel first so it takes the remaining space after sidebarPanel
            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebarPanel);

            // Toggle Button
            btnToggle = new Button
            {
                Text = "☰",
                Font = new Font("Segoe UI", 16),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 40),
                Location = new Point(10, 20),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(30, 96, 255),
                Cursor = Cursors.Hand
            };
            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.Click += BtnToggle_Click;
            sidebarPanel.Controls.Add(btnToggle);

            // Sidebar Title
            lblLogo = new Label
            {
                Text = "StockPap",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 96, 255),
                AutoSize = true,
                Location = new Point(50, 20)
            };
            lblSubLogo = new Label
            {
                Text = "Sistema de Inventario",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(55, 55)
            };
            sidebarPanel.Controls.Add(lblLogo);
            sidebarPanel.Controls.Add(lblSubLogo);

            // Menu Buttons
            int startY = 120;
            string[] menus = { "Ventas", "Inventario", "Proveedores", "Reportes" };
            foreach (var menu in menus)
            {
                Button btn = new Button
                {
                    Name = "btn" + menu,
                    Text = "   " + menu, // Keep text but we will hide it when collapsed
                    Font = new Font("Segoe UI", 12),
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Size = new Size(230, 50),
                    Location = new Point(10, startY),
                    BackColor = Color.Transparent,
                    ForeColor = Color.Black,
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => MenuClick(menu, btn);
                sidebarPanel.Controls.Add(btn);
                startY += 60;
            }

            // User Info
            lblUser = new Label
            {
                Text = $"Usuario: {currentUser.Username} ({currentUser.Rol})",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, this.Height - 100)
            };
            lblDate = new Label
            {
                Text = $"Fecha: {DateTime.Now.ToString("dd/MM/yyyy")}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, this.Height - 80)
            };
            sidebarPanel.Controls.Add(lblUser);
            sidebarPanel.Controls.Add(lblDate);
            
            btnLogout = new Button
            {
                Text = "Cerrar Sesion",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(210, 35),
                Location = new Point(20, this.Height - 50),
                BackColor = Color.FromArgb(220, 50, 50),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) => {
                if (MessageBox.Show("¿Seguro que deseas cerrar sesión?", "Cerrar Sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Restart();
                }
            };
            sidebarPanel.Controls.Add(btnLogout);
            
            // Set initial active state
            MenuClick("Ventas", (Button)sidebarPanel.Controls["btnVentas"]);
        }

        private void BtnToggle_Click(object sender, EventArgs e)
        {
            isSidebarExpanded = !isSidebarExpanded;
            sidebarPanel.Width = isSidebarExpanded ? 250 : 60;
            
            lblLogo.Visible = isSidebarExpanded;
            lblSubLogo.Visible = isSidebarExpanded;
            lblUser.Visible = isSidebarExpanded;
            lblDate.Visible = isSidebarExpanded;
            btnLogout.Visible = isSidebarExpanded;

            foreach (Control c in sidebarPanel.Controls)
            {
                if (c is Button b && b.Name.StartsWith("btn"))
                {
                    if (isSidebarExpanded)
                    {
                        b.Text = "   " + b.Name.Substring(3); // Restore name
                        b.Width = 230;
                    }
                    else
                    {
                        b.Text = b.Name.Substring(3, 1); // Just first letter as icon
                        b.Width = 40;
                    }
                }
            }
        }

        private void MenuClick(string menuName, Button clickedBtn)
        {
            // Reset styles
            foreach (Control c in sidebarPanel.Controls)
            {
                if (c is Button b && b.Name.StartsWith("btn"))
                {
                    b.BackColor = Color.Transparent;
                    b.ForeColor = Color.Black;
                }
            }

            // Active style
            clickedBtn.BackColor = Color.FromArgb(230, 240, 255);
            clickedBtn.ForeColor = Color.FromArgb(30, 96, 255);

            // Load View
            UserControl view = null;
            switch (menuName)
            {
                case "Ventas": view = new VentasView(currentUser); break;
                case "Inventario": view = new InventarioView(EsAdmin()); break;
                case "Proveedores": view = new ProveedoresView(EsAdmin()); break;
                case "Reportes": view = new ReportesView(EsAdmin()); break;
            }

            if (view != null) LoadView(view);
        }

        private void LoadView(UserControl view)
        {
            contentPanel.Controls.Clear();
            view.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(view);
        }

        private bool EsAdmin()
        {
            return currentUser != null && string.Equals(currentUser.Rol, "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
