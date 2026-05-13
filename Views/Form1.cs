using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Views.Controls;
using STOCKPAP.Presenters;

namespace STOCKPAP
{
    public partial class Form1 : Form
    {
        private Button currentBtn;
        private UserControl currentView;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Padding = new Padding(0);
            SetupNavigation();
            ShowDashboard();
        }

        private void SetupNavigation()
        {
            btnDashboard.Click += (s, e) => ShowDashboard();
            btnInventory.Click += (s, e) => ShowInventory();
            btnSales.Click += (s, e) => ShowSales();
            btnSuppliers.Click += (s, e) => ShowSuppliers();
            btnCustomers.Click += (s, e) => ShowCustomers();
            
            // Placeholders for other buttons
            btnStaff.Click += (s, e) => ShowPlaceholder("Personal", btnStaff);
            btnSettings.Click += (s, e) => ShowPlaceholder("Configuración", btnSettings);
        }

        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentBtn != (Button)btnSender)
                {
                    DisableButton();
                    Color color = Color.FromArgb(52, 152, 219);
                    currentBtn = (Button)btnSender;
                    currentBtn.BackColor = color;
                    currentBtn.ForeColor = Color.White;
                    currentBtn.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
                }
            }
        }

        private void DisableButton()
        {
            foreach (Control previousBtn in pnlSidebar.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(33, 37, 41);
                    previousBtn.ForeColor = Color.LightGray;
                    previousBtn.Font = new Font("Segoe UI", 11F);
                }
            }
        }

        private void SetView(UserControl view, string title, Button btn)
        {
            if (currentView != null) currentView.Dispose();
            
            currentView = view;
            currentView.Dock = DockStyle.Fill;
            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(currentView);
            lblTitle.Text = title;
            ActivateButton(btn);
        }

        private void ShowDashboard()
        {
            var view = new DashboardControl();
            SetView(view, "Dashboard", btnDashboard);
        }

        private void ShowInventory()
        {
            var view = new InventoryControl();
            new ProductoPresenter(view);
            SetView(view, "Gestión de Inventario", btnInventory);
        }

        private void ShowSales()
        {
            var view = new SalesControl();
            new VentaPresenter(view);
            SetView(view, "Registro de Ventas", btnSales);
        }

        private void ShowSuppliers()
        {
            var view = new SuppliersControl();
            SetView(view, "Proveedores", btnSuppliers);
        }

        private void ShowCustomers()
        {
            var view = new CustomersControl();
            SetView(view, "Clientes", btnCustomers);
        }

        private void ShowPlaceholder(string title, Button btn)
        {
            var pnl = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            var lbl = new Label { 
                Text = $"La vista de {title} está siendo optimizada.", 
                Font = new Font("Segoe UI", 16), 
                AutoSize = true, 
                ForeColor = Color.Gray,
                Location = new Point(50, 50)
            };
            pnl.Controls.Add(lbl);
            
            if (currentView != null) currentView.Dispose();
            currentView = null;
            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(pnl);
            lblTitle.Text = title;
            ActivateButton(btn);
        }
    }
}
