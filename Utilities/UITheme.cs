using System.Drawing;
using System.Windows.Forms;

namespace STOCKPAP.Utilities
{
    public static class UITheme
    {
        // Light Mode Premium Palette
        public static Color BackgroundColor = Color.FromArgb(245, 246, 250); // Light clean gray
        public static Color SurfaceColor = Color.White; // Pure white for cards/panels
        public static Color PrimaryColor = Color.FromArgb(41, 128, 185); // Nice corporate blue
        public static Color SecondaryColor = Color.FromArgb(52, 73, 94); // Dark slate blue/gray for headers
        public static Color TextColor = Color.FromArgb(44, 62, 80); // Dark gray for text
        public static Color SubtextColor = Color.FromArgb(127, 140, 141); // Soft gray for subtext
        public static Color SidebarColor = Color.FromArgb(255, 255, 255); // White sidebar

        public static void ApplyTheme(Control control)
        {
            if (control is Form form)
            {
                form.BackColor = BackgroundColor;
                form.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                form.ForeColor = TextColor;
            }
            else if (control is UserControl uc)
            {
                uc.BackColor = BackgroundColor;
                uc.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                uc.ForeColor = TextColor;
            }

            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child);
            }
        }

        private static void ApplyThemeToControl(Control control)
        {
            if (control is Panel panel)
            {
                if (panel.Name.Contains("Sidebar"))
                {
                    panel.BackColor = SidebarColor;
                }
                else if (panel.Name.Contains("Top"))
                {
                    panel.BackColor = PrimaryColor;
                    panel.ForeColor = Color.White;
                }
                else if (panel.Name.Contains("Header") || panel.Name.Contains("Card"))
                {
                    panel.BackColor = SurfaceColor;
                }
                else
                {
                    panel.BackColor = BackgroundColor;
                }
            }
            else if (control is Button button)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                
                // Sidebar buttons vs Action buttons
                if (button.Name.StartsWith("btn") && button.Parent?.Name == "pnlSidebar")
                {
                    button.BackColor = SidebarColor;
                    button.ForeColor = TextColor;
                    button.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
                }
                else
                {
                    button.BackColor = PrimaryColor;
                    button.ForeColor = Color.White;
                    button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                }
                button.Cursor = Cursors.Hand;
            }
            else if (control is DataGridView grid)
            {
                grid.BorderStyle = BorderStyle.None;
                grid.BackgroundColor = SurfaceColor;
                grid.GridColor = Color.FromArgb(236, 240, 241); // Very light gray lines
                
                grid.DefaultCellStyle.BackColor = SurfaceColor;
                grid.DefaultCellStyle.ForeColor = TextColor;
                grid.DefaultCellStyle.SelectionBackColor = PrimaryColor;
                grid.DefaultCellStyle.SelectionForeColor = Color.White;
                
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                
                grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                grid.EnableHeadersVisualStyles = false;
                grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                grid.ColumnHeadersDefaultCellStyle.BackColor = SecondaryColor;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                grid.ColumnHeadersHeight = 40;
                grid.RowTemplate.Height = 35;
            }
            else if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.BackColor = Color.White;
                textBox.ForeColor = TextColor;
                textBox.Font = new Font("Segoe UI", 11F);
            }
            else if (control is Label label)
            {
                if (label.Font.Size >= 14 || label.Name.Contains("Title")) // Titles
                {
                    label.ForeColor = PrimaryColor;
                    label.Font = new Font("Segoe UI", label.Font.Size, FontStyle.Bold);
                }
                else
                {
                    label.ForeColor = TextColor;
                }
            }

            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child);
            }
        }
    }
}
