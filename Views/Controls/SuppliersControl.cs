using System;
using System.Windows.Forms;

namespace STOCKPAP.Views.Controls
{
    public partial class SuppliersControl : UserControl
    {
        public SuppliersControl()
        {
            InitializeComponent();
            btnNew.Click += (s, e) => MessageBox.Show("Función para agregar proveedor en desarrollo.");
        }
    }
}
