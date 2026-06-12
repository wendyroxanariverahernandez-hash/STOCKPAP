using System;
using System.Drawing;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.DataAccess;
using STOCKPAP.Utilities;

namespace STOCKPAP.Views
{
    public class ClasificacionesView : UserControl
    {
        private ListBox lstClases;
        private ListBox lstSubclases;
        private ListBox lstMarcas;

        private Button btnAddClase;
        private Button btnEditClase;
        private Button btnDelClase;
        private Button btnAddSubclase;
        private Button btnEditSubclase;
        private Button btnDelSubclase;
        private Button btnAddMarca;
        private Button btnEditMarca;
        private Button btnDelMarca;

        private ClasificacionRepository repo;

        public ClasificacionesView()
        {
            repo = new ClasificacionRepository();
            InitializeComponent();
            this.Load += (s, e) => CargarClases();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Padding = new Padding(30);

            // Título Principal
            Label lblTitle = new Label
            {
                Text = "Gestión de Clasificaciones",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 96, 255),
                AutoSize = true,
                Dock = DockStyle.Top
            };
            this.Controls.Add(lblTitle);

            // Contenedor Principal (Espaciado del título)
            Panel spacer = new Panel { Dock = DockStyle.Top, Height = 25 };
            this.Controls.Add(spacer);

            TableLayoutPanel table = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 1,
                Dock = DockStyle.Fill
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            this.Controls.Add(table);
            table.BringToFront();

            // Panel Clases
            Panel pnlClases = CrearPanelLista("1. Clases", "Ej: Escritura, Papel, Oficina", out lstClases, out btnAddClase, out btnEditClase, out btnDelClase);
            table.Controls.Add(pnlClases, 0, 0);
            
            // Panel Subclases
            Panel pnlSubclases = CrearPanelLista("2. Subclases", "Ej: Bolígrafos, Lápices (depende de Clase)", out lstSubclases, out btnAddSubclase, out btnEditSubclase, out btnDelSubclase);
            table.Controls.Add(pnlSubclases, 1, 0);

            // Panel Marcas
            Panel pnlMarcas = CrearPanelLista("3. Marcas", "Ej: BIC, Pelikan (depende de Clase)", out lstMarcas, out btnAddMarca, out btnEditMarca, out btnDelMarca);
            table.Controls.Add(pnlMarcas, 2, 0);

            // Eventos
            lstClases.SelectedIndexChanged += LstClases_SelectedIndexChanged;
            btnAddClase.Click += BtnAddClase_Click;
            btnEditClase.Click += BtnEditClase_Click;
            btnDelClase.Click += BtnDelClase_Click;
            btnAddSubclase.Click += BtnAddSubclase_Click;
            btnEditSubclase.Click += BtnEditSubclase_Click;
            btnDelSubclase.Click += BtnDelSubclase_Click;
            btnAddMarca.Click += BtnAddMarca_Click;
            btnEditMarca.Click += BtnEditMarca_Click;
            btnDelMarca.Click += BtnDelMarca_Click;
        }

        private Panel CrearPanelLista(string titulo, string subtitulo, out ListBox lst, out Button btnAdd, out Button btnEdit, out Button btnDel)
        {
            // Panel tarjeta blanco
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                BackColor = Color.White,
                Padding = new Padding(15)
            };
            // Efecto sutil de borde para simular tarjeta
            card.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, Color.FromArgb(220, 225, 230), ButtonBorderStyle.Solid);
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Título
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Subtítulo
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // ListBox
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Botones
            card.Controls.Add(layout);

            // 1. Título
            Label lbl = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5)
            };
            layout.Controls.Add(lbl, 0, 0);

            // 2. Subtítulo
            Label lblSub = new Label
            {
                Text = subtitulo,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15)
            };
            layout.Controls.Add(lblSub, 0, 1);

            // 3. Lista
            Panel pnlListaContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(5)
            };
            lst = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(250, 250, 250),
                IntegralHeight = false,
                ItemHeight = 25
            };
            pnlListaContainer.Controls.Add(lst);
            layout.Controls.Add(pnlListaContainer, 0, 2);

            // 4. Botones
            Panel pnlBotones = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0) };
            
            TableLayoutPanel tblBotones = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            tblBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tblBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tblBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            
            btnAdd = new RoundedButton
            {
                Text = "+",
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                BackColor = Color.FromArgb(30, 96, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 5,
                Cursor = Cursors.Hand
            };
            
            btnEdit = new RoundedButton
            {
                Text = "✏",
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                BackColor = Color.FromArgb(240, 173, 78),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 5,
                Cursor = Cursors.Hand
            };
            
            btnDel = new RoundedButton
            {
                Text = "🗑",
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                BackColor = Color.FromArgb(255, 240, 240),
                ForeColor = Color.FromArgb(200, 50, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderRadius = 5,
                Cursor = Cursors.Hand
            };

            tblBotones.Controls.Add(btnAdd, 0, 0);
            tblBotones.Controls.Add(btnEdit, 1, 0);
            tblBotones.Controls.Add(btnDel, 2, 0);
            pnlBotones.Controls.Add(tblBotones);
            layout.Controls.Add(pnlBotones, 0, 3);

            // Contenedor exterior para margen en TableLayout
            Panel wrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            wrapper.Controls.Add(card);

            return wrapper;
        }

        // --- Resto de lógica sin cambios ---

        private void CargarClases()
        {
            lstClases.Items.Clear();
            var clases = repo.ObtenerClases();
            foreach (var c in clases)
            {
                lstClases.Items.Add(c);
            }
            lstSubclases.Items.Clear();
            lstMarcas.Items.Clear();
        }

        private void LstClases_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstClases.SelectedItem is Clase clase)
            {
                CargarSubclases(clase.Id);
                CargarMarcas(clase.Id);
            }
            else
            {
                lstSubclases.Items.Clear();
                lstMarcas.Items.Clear();
            }
        }

        private void CargarSubclases(int claseId)
        {
            lstSubclases.Items.Clear();
            var subclases = repo.ObtenerSubclases(claseId);
            foreach (var s in subclases)
            {
                lstSubclases.Items.Add(s);
            }
        }

        private void CargarMarcas(int claseId)
        {
            lstMarcas.Items.Clear();
            var marcas = repo.ObtenerMarcas(claseId);
            foreach (var m in marcas)
            {
                lstMarcas.Items.Add(m);
            }
        }

        private void BtnAddClase_Click(object sender, EventArgs e)
        {
            string nombre = PromptDialog.Mostrar("Nueva Clase", "Ingrese el nombre de la clase (Ej: Escritura, Cuadernos):");
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                var clase = new Clase { Nombre = nombre.Trim() };
                if (repo.AgregarClase(clase))
                {
                    CargarClases();
                    for (int i = 0; i < lstClases.Items.Count; i++)
                    {
                        if (((Clase)lstClases.Items[i]).Id == clase.Id)
                        {
                            lstClases.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se pudo agregar la clase. Puede que ya exista.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDelClase_Click(object sender, EventArgs e)
        {
            if (lstClases.SelectedItem is Clase clase)
            {
                if (MessageBox.Show($"¿Eliminar la clase '{clase.Nombre}' y todas sus subclases y marcas?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (repo.EliminarClase(clase.Id)) CargarClases();
                }
            }
        }

        private void BtnEditClase_Click(object sender, EventArgs e)
        {
            if (lstClases.SelectedItem is Clase clase)
            {
                string nuevoNombre = PromptDialog.Mostrar("Editar Clase", "Ingrese el nuevo nombre para la clase:", clase.Nombre);
                if (!string.IsNullOrWhiteSpace(nuevoNombre) && nuevoNombre.Trim() != clase.Nombre)
                {
                    clase.Nombre = nuevoNombre.Trim();
                    if (repo.ActualizarClase(clase))
                    {
                        CargarClases();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar la clase. Puede que ya exista.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Primero seleccione una Clase de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnAddSubclase_Click(object sender, EventArgs e)
        {
            if (lstClases.SelectedItem is Clase clase)
            {
                string nombre = PromptDialog.Mostrar("Nueva Subclase", $"Ingrese el nombre de la subclase para '{clase.Nombre}':");
                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (repo.AgregarSubclase(new Subclase { Nombre = nombre.Trim(), ClaseId = clase.Id }))
                        CargarSubclases(clase.Id);
                    else
                        MessageBox.Show("No se pudo agregar la subclase.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Primero seleccione una Clase de la lista izquierda.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelSubclase_Click(object sender, EventArgs e)
        {
            if (lstClases.SelectedItem is Clase clase && lstSubclases.SelectedItem is Subclase subclase)
            {
                if (MessageBox.Show($"¿Eliminar la subclase '{subclase.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (repo.EliminarSubclase(subclase.Id)) CargarSubclases(clase.Id);
                }
            }
        }

        private void BtnEditSubclase_Click(object sender, EventArgs e)
        {
            if (lstClases.SelectedItem is Clase clase && lstSubclases.SelectedItem is Subclase subclase)
            {
                string nuevoNombre = PromptDialog.Mostrar("Editar Subclase", $"Ingrese el nuevo nombre para '{subclase.Nombre}':", subclase.Nombre);
                if (!string.IsNullOrWhiteSpace(nuevoNombre) && nuevoNombre.Trim() != subclase.Nombre)
                {
                    subclase.Nombre = nuevoNombre.Trim();
                    if (repo.ActualizarSubclase(subclase))
                    {
                        CargarSubclases(clase.Id);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar la subclase.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Primero seleccione una Subclase de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnAddMarca_Click(object sender, EventArgs e)
        {
            if (lstClases.SelectedItem is Clase clase)
            {
                string nombre = PromptDialog.Mostrar("Nueva Marca", $"Ingrese el nombre de la marca para '{clase.Nombre}':");
                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    if (repo.AgregarMarca(new Marca { Nombre = nombre.Trim(), ClaseId = clase.Id }))
                        CargarMarcas(clase.Id);
                    else
                        MessageBox.Show("No se pudo agregar la marca.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Primero seleccione una Clase de la lista izquierda.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelMarca_Click(object sender, EventArgs e)
        {
            if (lstClases.SelectedItem is Clase clase && lstMarcas.SelectedItem is Marca marca)
            {
                if (MessageBox.Show($"¿Eliminar la marca '{marca.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (repo.EliminarMarca(marca.Id)) CargarMarcas(clase.Id);
                }
            }
        }

        private void BtnEditMarca_Click(object sender, EventArgs e)
        {
            if (lstClases.SelectedItem is Clase clase && lstMarcas.SelectedItem is Marca marca)
            {
                string nuevoNombre = PromptDialog.Mostrar("Editar Marca", $"Ingrese el nuevo nombre para '{marca.Nombre}':", marca.Nombre);
                if (!string.IsNullOrWhiteSpace(nuevoNombre) && nuevoNombre.Trim() != marca.Nombre)
                {
                    marca.Nombre = nuevoNombre.Trim();
                    if (repo.ActualizarMarca(marca))
                    {
                        CargarMarcas(clase.Id);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar la marca.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Primero seleccione una Marca de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
