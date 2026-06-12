using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace STOCKPAP.Utilities
{
    public static class ThemeManager
    {
        public static void AplicarTema(Control parent, string tema)
        {
            bool isDark = tema == "Modo Oscuro";
            
            Color bg = isDark ? Color.FromArgb(30, 30, 31) : Color.FromArgb(245, 247, 250);
            Color fg = isDark ? Color.White : Color.FromArgb(20, 20, 40);
            Color panelBg = isDark ? Color.FromArgb(45, 45, 48) : Color.White;
            Color textGray = isDark ? Color.LightGray : Color.Gray;

            parent.BackColor = bg;
            if (!(parent is Form))
                parent.ForeColor = fg;

            AplicarRecursivo(parent, isDark, bg, fg, panelBg, textGray);
        }

        private static Color GetEffectiveBackColor(Control c)
        {
            while (c != null && (c.BackColor == Color.Transparent || c.BackColor.A == 0))
            {
                c = c.Parent;
            }
            return c?.BackColor ?? Color.White;
        }

        private static void AplicarRecursivo(Control parent, bool isDark, Color bg, Color fg, Color panelBg, Color textGray)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is DataGridView dgv)
                {
                    dgv.BackgroundColor = isDark ? panelBg : Color.White;
                    dgv.DefaultCellStyle.BackColor = isDark ? panelBg : Color.White;
                    dgv.DefaultCellStyle.ForeColor = fg;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = isDark ? Color.FromArgb(60, 60, 60) : Color.FromArgb(245, 247, 250);
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = fg;
                    dgv.AlternatingRowsDefaultCellStyle.BackColor = isDark ? Color.FromArgb(50, 50, 55) : Color.FromArgb(250, 251, 255);
                    dgv.GridColor = isDark ? Color.FromArgb(80, 80, 80) : Color.FromArgb(235, 238, 245);
                }
                else if (c is Button btn)
                {
                    if (btn.BackColor == Color.White || btn.BackColor == Color.FromArgb(245, 247, 250) || btn.BackColor == Color.FromArgb(60, 60, 65) || btn.BackColor == Color.WhiteSmoke)
                    {
                        btn.BackColor = isDark ? Color.FromArgb(60, 60, 65) : Color.WhiteSmoke;
                        btn.ForeColor = fg;
                    }
                    else if (btn.BackColor == Color.FromArgb(230, 240, 255) || btn.BackColor == Color.FromArgb(40, 50, 70))
                    {
                        btn.BackColor = isDark ? Color.FromArgb(40, 50, 70) : Color.FromArgb(230, 240, 255);
                    }
                }
                else if (c is Panel pnl)
                {
                    if (pnl.BackColor == Color.White || pnl.BackColor == Color.FromArgb(45, 45, 48) || pnl.BackColor == Color.WhiteSmoke) 
                        pnl.BackColor = panelBg;
                    else if (pnl.BackColor == Color.FromArgb(245, 247, 250) || pnl.BackColor == Color.FromArgb(30, 30, 31)) 
                        pnl.BackColor = bg;
                    else if (pnl.BackColor == Color.LightGray || pnl.BackColor == Color.FromArgb(60, 60, 65))
                        pnl.BackColor = isDark ? Color.FromArgb(60, 60, 65) : Color.LightGray;

                    AplicarRecursivo(pnl, isDark, bg, fg, panelBg, textGray);
                }
                else if (c is Label lbl)
                {
                    bool isAccent = (lbl.ForeColor == Color.FromArgb(30, 96, 255) ||
                                     lbl.ForeColor == Color.FromArgb(16, 185, 90) ||
                                     lbl.ForeColor == Color.FromArgb(220, 50, 50) ||
                                     lbl.ForeColor == Color.FromArgb(39, 174, 96) ||
                                     lbl.ForeColor == Color.FromArgb(192, 57, 43));
                    if (isAccent)
                    {
                        // Keep accent color
                    }
                    else if (lbl.ForeColor == Color.Gray || lbl.ForeColor == Color.LightGray)
                    {
                        lbl.ForeColor = textGray;
                    }
                    else
                    {
                        Color effectiveBg = GetEffectiveBackColor(lbl.Parent);
                        bool isParentDark = effectiveBg.GetBrightness() < 0.5f;
                        lbl.ForeColor = isParentDark ? Color.White : fg;
                    }
                }
                else if (c is TextBox txt)
                {
                    txt.BackColor = isDark ? Color.FromArgb(60, 60, 60) : Color.White;
                    txt.ForeColor = fg;
                }
                else if (c is ComboBox cmb)
                {
                    cmb.BackColor = isDark ? Color.FromArgb(60, 60, 60) : Color.White;
                    cmb.ForeColor = fg;
                }
                else if (c is NumericUpDown num)
                {
                    num.BackColor = isDark ? Color.FromArgb(60, 60, 60) : Color.White;
                    num.ForeColor = fg;
                }
                else
                {
                    AplicarRecursivo(c, isDark, bg, fg, panelBg, textGray);
                }
            }
        }
    }
}
