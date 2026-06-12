using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;
using Npgsql;

namespace STOCKPAP.Views
{
    public class ConfiguracionView : UserControl
    {
        private bool esAdmin;
        private NumericUpDown numStockGlobal;
        private CheckBox chkAlertas;
        private ComboBox cmbReportFormat;
        private TextBox txtEmpresa;
        private ComboBox cmbMoneda;
        private ComboBox cmbTema;
        private TextBox txtBackupPath;
        private Label lblEstado;

        public ConfiguracionView(bool esAdmin = true)
        {
            this.esAdmin = esAdmin;
            InitializeComponent();
            CargarConfiguraciones();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            // Título
            Label lblTitle = new Label
            {
                Text = "Configuración del Sistema",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 40),
                AutoSize = true,
                Location = new Point(30, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Configura parámetros generales de stock, alertas, reportes e integridad del sistema",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(35, 75)
            };
            this.Controls.Add(lblSubtitle);

            // Contenedor
            RoundedPanel panelConfig = new RoundedPanel
            {
                Size = new Size(540, 480),
                Location = new Point(30, 120),
                BackColor = Color.White,
                BorderRadius = 15
            };
            this.Controls.Add(panelConfig);

            int y = 30;

            // 1. Límites de Stock
            AddLabel(panelConfig, "Límite de Stock Mínimo Global (Predeterminado):", 30, y);
            numStockGlobal = new NumericUpDown
            {
                Location = new Point(30, y + 22),
                Width = 480,
                Font = new Font("Segoe UI", 11),
                Minimum = 1,
                Maximum = 1000,
                Value = 10
            };
            panelConfig.Controls.Add(numStockGlobal);
            y += 75;

            // 2. Alertas
            chkAlertas = new CheckBox
            {
                Text = "Activar notificaciones permanentes de Stock Bajo",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 80),
                Location = new Point(30, y),
                Size = new Size(480, 24),
                Cursor = Cursors.Hand,
                Checked = true
            };
            panelConfig.Controls.Add(chkAlertas);
            y += 50;

            // 3. Reportes
            AddLabel(panelConfig, "Formato predeterminado para exportación de reportes:", 30, y);
            cmbReportFormat = new ComboBox
            {
                Location = new Point(30, y + 22),
                Width = 480,
                Font = new Font("Segoe UI", 10.5f),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbReportFormat.Items.AddRange(new[] { "PDF", "Excel (.xls)", "CSV" });
            cmbReportFormat.SelectedIndex = 0;
            panelConfig.Controls.Add(cmbReportFormat);
            y += 75;

            // 4. Parámetros de la aplicación / Ruta de exportación
            AddLabel(panelConfig, "Carpeta de descarga predeterminada para reportes:", 30, y);
            txtBackupPath = new TextBox
            {
                Location = new Point(30, y + 22),
                Width = 360,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            panelConfig.Controls.Add(txtBackupPath);

            Button btnSelectFolder = new Button
            {
                Text = "Examinar...",
                Location = new Point(400, y + 21),
                Size = new Size(110, 25),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSelectFolder.Click += BtnSelectFolder_Click;
            panelConfig.Controls.Add(btnSelectFolder);
            y += 85;

            // 5. Nombre de la Empresa
            AddLabel(panelConfig, "Nombre de la Empresa (Para Reportes):", 30, y);
            txtEmpresa = new TextBox
            {
                Location = new Point(30, y + 22),
                Width = 480,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            panelConfig.Controls.Add(txtEmpresa);
            y += 75;

            // 6. Moneda
            AddLabel(panelConfig, "Moneda Predeterminada:", 30, y);
            cmbMoneda = new ComboBox
            {
                Location = new Point(30, y + 22),
                Width = 480,
                Font = new Font("Segoe UI", 10.5f),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbMoneda.Items.AddRange(new[] { "MXN (Pesos)", "USD (Dólares)", "EUR (Euros)", "Otro" });
            cmbMoneda.SelectedIndex = 0;
            panelConfig.Controls.Add(cmbMoneda);
            y += 75;

            // 7. Tema Visual
            AddLabel(panelConfig, "Tema Visual:", 30, y);
            cmbTema = new ComboBox
            {
                Location = new Point(30, y + 22),
                Width = 480,
                Font = new Font("Segoe UI", 10.5f),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTema.Items.AddRange(new[] { "Modo Claro", "Modo Oscuro" });
            cmbTema.SelectedIndex = 0;
            panelConfig.Controls.Add(cmbTema);
            y += 90;

            panelConfig.Size = new Size(540, y + 70); // Adjust panel size for new items

            // Barra de estado y Guardar
            lblEstado = new Label
            {
                ForeColor = Color.Green,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                Location = new Point(30, y + 10),
                Size = new Size(290, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelConfig.Controls.Add(lblEstado);

            Button btnGuardar = new Button
            {
                Text = "Guardar Configuración",
                Location = new Point(330, y),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGuardar.Click += BtnGuardar_Click;
            panelConfig.Controls.Add(btnGuardar);

            if (!esAdmin)
            {
                numStockGlobal.Enabled = false;
                chkAlertas.Enabled = false;
                cmbReportFormat.Enabled = false;
                txtBackupPath.Enabled = false;
                btnSelectFolder.Enabled = false;
                txtEmpresa.Enabled = false;
                cmbMoneda.Enabled = false;
                cmbTema.Enabled = false;
                btnGuardar.Enabled = false;
                btnGuardar.BackColor = Color.Gray;
                lblEstado.Text = "Solo lectura: Requiere rol de Administrador.";
                lblEstado.ForeColor = Color.FromArgb(190, 18, 60);
            }
        }

        private void AddLabel(Panel parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(x, y),
                AutoSize = true
            });
        }

        private void CargarConfiguraciones()
        {
            numStockGlobal.Value = Convert.ToDecimal(ObtenerConfiguracion("StockMinimoGlobal", "10"));
            chkAlertas.Checked = ObtenerConfiguracion("AlertasActivas", "true").ToLower() == "true";
            
            string fmt = ObtenerConfiguracion("FormatoReporteDefecto", "PDF");
            int idx = cmbReportFormat.Items.IndexOf(fmt);
            if (idx >= 0) cmbReportFormat.SelectedIndex = idx;

            txtBackupPath.Text = ObtenerConfiguracion("RutaDescargas", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            txtEmpresa.Text = ObtenerConfiguracion("NombreEmpresa", "StockPap");

            string mon = ObtenerConfiguracion("Moneda", "MXN (Pesos)");
            int iMon = cmbMoneda.Items.IndexOf(mon);
            if (iMon >= 0) cmbMoneda.SelectedIndex = iMon;

            string tem = ObtenerConfiguracion("TemaVisual", "Modo Claro");
            int iTem = cmbTema.Items.IndexOf(tem);
            if (iTem >= 0) cmbTema.SelectedIndex = iTem;
        }

        private string ObtenerConfiguracion(string clave, string defecto)
        {
            try
            {
                using (var conn = Conexion.Instance.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT Valor FROM Configuracion WHERE Clave = @c", conn))
                    {
                        cmd.Parameters.AddWithValue("c", clave);
                        var val = cmd.ExecuteScalar();
                        return val != null ? val.ToString() : defecto;
                    }
                }
            }
            catch
            {
                return defecto;
            }
        }

        private void GuardarConfiguracion(string clave, string valor)
        {
            using (var conn = Conexion.Instance.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    @"INSERT INTO Configuracion (Clave, Valor) VALUES (@c, @v)
                      ON CONFLICT (Clave) DO UPDATE SET Valor = EXCLUDED.Valor", conn))
                {
                    cmd.Parameters.AddWithValue("c", clave);
                    cmd.Parameters.AddWithValue("v", valor);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void BtnSelectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Seleccione la carpeta predeterminada para descargas";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtBackupPath.Text = fbd.SelectedPath;
                }
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!esAdmin) return;

            try
            {
                GuardarConfiguracion("StockMinimoGlobal", numStockGlobal.Value.ToString());
                GuardarConfiguracion("AlertasActivas", chkAlertas.Checked.ToString().ToLower());
                GuardarConfiguracion("FormatoReporteDefecto", cmbReportFormat.SelectedItem.ToString());
                GuardarConfiguracion("RutaDescargas", txtBackupPath.Text.Trim());
                GuardarConfiguracion("NombreEmpresa", txtEmpresa.Text.Trim());
                GuardarConfiguracion("Moneda", cmbMoneda.SelectedItem.ToString());
                GuardarConfiguracion("TemaVisual", cmbTema.SelectedItem.ToString());

                // Actualizar stock mínimo de TODOS los productos existentes
                using (var conn = Conexion.Instance.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("UPDATE Productos SET StockMinimo = @sm", conn))
                    {
                        cmd.Parameters.AddWithValue("sm", (int)numStockGlobal.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Trigger global update
                ConfigHelper.NotificarCambios();

                lblEstado.ForeColor = Color.Green;
                lblEstado.Text = "✓ Configuraciones guardadas con éxito.";

                // Disparar temporizador para limpiar mensaje
                Timer t = new Timer { Interval = 3000 };
                t.Tick += (s2, e2) => { lblEstado.Text = ""; t.Stop(); t.Dispose(); };
                t.Start();

                MessageBox.Show("Configuración guardada correctamente. Los cambios (como Tema y Moneda) se han aplicado en todo el sistema.", "Configuración", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblEstado.ForeColor = Color.Red;
                lblEstado.Text = "Error: " + ex.Message;
            }
        }
    }
}
