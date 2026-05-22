using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace STOCKPAP.Utilities
{
    public class RoundedPanel : Panel
    {
        public int BorderRadius { get; set; } = 15;
        public Color BorderColor { get; set; } = Color.Transparent;
        public int BorderSize { get; set; } = 0;

        public RoundedPanel()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            int radius = BorderRadius;

            if (radius > 0)
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.Width - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Height - radius, radius, radius, 90, 90);
                    path.CloseFigure();

                    this.Region = new Region(path);

                    if (BorderSize > 0)
                    {
                        using (Pen pen = new Pen(BorderColor, BorderSize))
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            e.Graphics.DrawPath(pen, path);
                        }
                    }
                }
            }
        }
    }
}
