using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Common;

namespace STOCKPAP.Views
{
    public class BarcodeScannerForm : Form
    {
        private ComboBox cmbCamaras;
        private PictureBox picCamara;
        private Label lblEstado;
        private Label lblCodigoResult;
        private Button btnDetener;
        private Button btnCerrar;
        private Panel pnlFlashOverlay;
        private Panel pnlHeader;
        private Panel pnlFooter;
        private FilterInfoCollection dispositivos;
        private VideoCaptureDevice camara;
        private BarcodeReader lector;
        private bool codigoEncontrado;

        // Laser animation
        private Timer timerLaser;
        private int laserPos = 0;
        private int laserDir = 1;

        // Green flash animation
        private Timer timerFlash;
        private int flashAlpha = 0;
        private int flashPhase = 0; // 0=flash in, 1=hold, 2=flash out

        public string CodigoDetectado { get; private set; }

        public BarcodeScannerForm()
        {
            InitializeComponent();
            CargarCamaras();
        }

        private void InitializeComponent()
        {
            Text = "ESCÁNER DE CÓDIGO DE BARRAS";
            Size = new Size(800, 620);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(18, 18, 24);

            // ── Header ──────────────────────────────────────────────
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(25, 25, 35),
                Padding = new Padding(15, 0, 15, 0)
            };

            Label lblTitle = new Label
            {
                Text = "📷  ESCÁNER POS",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 18)
            };
            pnlHeader.Controls.Add(lblTitle);

            Label lblCamLabel = new Label
            {
                Text = "Cámara:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(150, 160, 180),
                AutoSize = true,
                Location = new Point(320, 22)
            };
            pnlHeader.Controls.Add(lblCamLabel);

            cmbCamaras = new ComboBox
            {
                Location = new Point(385, 18),
                Size = new Size(250, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(40, 42, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cmbCamaras.SelectedIndexChanged += (s, e) =>
            {
                DetenerCamara();
                IniciarCamara();
            };
            pnlHeader.Controls.Add(cmbCamaras);

            btnDetener = new Button
            {
                Text = "⏹ Detener",
                Location = new Point(650, 15),
                Size = new Size(110, 32),
                BackColor = Color.FromArgb(60, 60, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDetener.FlatAppearance.BorderSize = 0;
            btnDetener.Click += (s, e) => DetenerCamara();
            pnlHeader.Controls.Add(btnDetener);

            this.Controls.Add(pnlHeader);

            // ── Camera View (main area) ─────────────────────────────
            Panel pnlCameraContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 10, 20, 10),
                BackColor = Color.FromArgb(18, 18, 24)
            };

            picCamara = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(10, 10, 15),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None
            };
            picCamara.Paint += PicCamara_Paint;
            pnlCameraContainer.Controls.Add(picCamara);

            // Green flash overlay (on top of camera)
            pnlFlashOverlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Visible = false
            };
            pnlFlashOverlay.Paint += PnlFlash_Paint;
            pnlCameraContainer.Controls.Add(pnlFlashOverlay);
            pnlFlashOverlay.BringToFront();

            this.Controls.Add(pnlCameraContainer);

            // ── Footer ──────────────────────────────────────────────
            pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                BackColor = Color.FromArgb(25, 25, 35),
                Padding = new Padding(20, 10, 20, 10)
            };

            // Status indicator dot
            Panel pnlDot = new Panel
            {
                Size = new Size(12, 12),
                Location = new Point(25, 18),
                BackColor = Color.FromArgb(0, 200, 100)
            };
            pnlDot.Paint += (s, ev) =>
            {
                ev.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                ev.Graphics.Clear(pnlFooter.BackColor);
                using (SolidBrush b = new SolidBrush(codigoEncontrado ? Color.FromArgb(0, 220, 80) : Color.FromArgb(0, 150, 255)))
                    ev.Graphics.FillEllipse(b, 0, 0, 11, 11);
            };
            pnlFooter.Controls.Add(pnlDot);

            lblEstado = new Label
            {
                Text = "Iniciando cámara...",
                Location = new Point(45, 14),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 150, 255)
            };
            pnlFooter.Controls.Add(lblEstado);

            lblCodigoResult = new Label
            {
                Text = "",
                Location = new Point(45, 40),
                AutoSize = true,
                Font = new Font("Consolas", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 220, 80),
                Visible = false
            };
            pnlFooter.Controls.Add(lblCodigoResult);

            btnCerrar = new Button
            {
                Text = "✕ Cerrar",
                Size = new Size(120, 38),
                Location = new Point(640, 20),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            pnlFooter.Controls.Add(btnCerrar);

            this.Controls.Add(pnlFooter);

            // ── ZXing reader ────────────────────────────────────────
            lector = new BarcodeReader
            {
                AutoRotate = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    TryInverted = true,
                    PossibleFormats = new[]
                    {
                        BarcodeFormat.EAN_13,
                        BarcodeFormat.EAN_8,
                        BarcodeFormat.UPC_A,
                        BarcodeFormat.UPC_E,
                        BarcodeFormat.CODE_128,
                        BarcodeFormat.CODE_39,
                        BarcodeFormat.CODE_93,
                        BarcodeFormat.ITF,
                        BarcodeFormat.CODABAR,
                        BarcodeFormat.QR_CODE,
                        BarcodeFormat.DATA_MATRIX
                    }.ToList()
                }
            };

            // ── Laser animation timer ───────────────────────────────
            timerLaser = new Timer { Interval = 15 };
            timerLaser.Tick += (s, e) =>
            {
                if (picCamara.Image == null) return;
                int maxH = picCamara.Height / 2 + 100;
                int minH = picCamara.Height / 2 - 100;

                if (laserPos == 0) laserPos = minH;

                laserPos += (4 * laserDir);
                if (laserPos > maxH) { laserPos = maxH; laserDir = -1; }
                if (laserPos < minH) { laserPos = minH; laserDir = 1; }

                picCamara.Invalidate();
            };

            // ── Green flash timer ───────────────────────────────────
            timerFlash = new Timer { Interval = 20 };
            timerFlash.Tick += TimerFlash_Tick;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (cmbCamaras.Items.Count > 0)
                IniciarCamara();
        }

        // ── Paint: POS-style overlay on camera ─────────────────────
        private void PicCamara_Paint(object sender, PaintEventArgs e)
        {
            if (picCamara.Image == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = picCamara.Width / 2;
            int cy = picCamara.Height / 2;
            int w = 340;
            int h = 200;

            Rectangle rect = new Rectangle(cx - w / 2, cy - h / 2, w, h);

            if (codigoEncontrado)
            {
                // GREEN SUCCESS FRAME
                using (Pen pen = new Pen(Color.FromArgb(0, 220, 80), 5))
                {
                    g.DrawRectangle(pen, rect);
                }
                // Checkmark icon
                using (Font f = new Font("Segoe UI", 28, FontStyle.Bold))
                using (SolidBrush br = new SolidBrush(Color.FromArgb(200, 0, 220, 80)))
                {
                    g.DrawString("✓", f, br, cx - 22, rect.Y - 50);
                }
                return;
            }

            // ── Dark overlay outside scan zone ──────────────────────
            using (SolidBrush shade = new SolidBrush(Color.FromArgb(140, 0, 0, 0)))
            {
                Region rgn = new Region(new Rectangle(0, 0, picCamara.Width, picCamara.Height));
                rgn.Exclude(rect);
                g.FillRegion(shade, rgn);
            }

            // ── Corner brackets (POS-style) ─────────────────────────
            Color bracketColor = Color.FromArgb(0, 180, 255);
            using (Pen pen = new Pen(bracketColor, 4))
            {
                int len = 35;
                // Top left
                g.DrawLine(pen, rect.X, rect.Y, rect.X + len, rect.Y);
                g.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Y + len);
                // Top right
                g.DrawLine(pen, rect.Right, rect.Y, rect.Right - len, rect.Y);
                g.DrawLine(pen, rect.Right, rect.Y, rect.Right, rect.Y + len);
                // Bottom left
                g.DrawLine(pen, rect.X, rect.Bottom, rect.X + len, rect.Bottom);
                g.DrawLine(pen, rect.X, rect.Bottom, rect.X, rect.Bottom - len);
                // Bottom right
                g.DrawLine(pen, rect.Right, rect.Bottom, rect.Right - len, rect.Bottom);
                g.DrawLine(pen, rect.Right, rect.Bottom, rect.Right, rect.Bottom - len);
            }

            // ── Instruction text ────────────────────────────────────
            using (Font f = new Font("Segoe UI", 9, FontStyle.Bold))
            using (SolidBrush br = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                string txt = "Coloca el código de barras en el centro";
                SizeF sz = g.MeasureString(txt, f);
                g.DrawString(txt, f, br, cx - sz.Width / 2, rect.Bottom + 15);
            }

            // ── Animated laser line ─────────────────────────────────
            if (laserPos > 0 && timerLaser.Enabled)
            {
                int clampedPos = Math.Max(rect.Y + 5, Math.Min(rect.Bottom - 5, laserPos));

                // Main red laser line
                using (Pen laserPen = new Pen(Color.FromArgb(230, 255, 50, 50), 2))
                {
                    g.DrawLine(laserPen, rect.X + 8, clampedPos, rect.Right - 8, clampedPos);
                }

                // Glow around laser
                using (LinearGradientBrush glow = new LinearGradientBrush(
                    new Rectangle(rect.X + 8, clampedPos - 12, rect.Width - 16, 24),
                    Color.FromArgb(60, 255, 30, 30),
                    Color.FromArgb(0, 255, 0, 0),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(glow, rect.X + 8, clampedPos - 12, rect.Width - 16, 24);
                }
            }
        }

        // ── Green flash overlay paint ───────────────────────────────
        private void PnlFlash_Paint(object sender, PaintEventArgs e)
        {
            if (flashAlpha > 0)
            {
                using (SolidBrush br = new SolidBrush(Color.FromArgb(flashAlpha, 0, 220, 80)))
                {
                    e.Graphics.FillRectangle(br, 0, 0, pnlFlashOverlay.Width, pnlFlashOverlay.Height);
                }
            }
        }

        private void TimerFlash_Tick(object sender, EventArgs e)
        {
            switch (flashPhase)
            {
                case 0: // flash in
                    flashAlpha += 15;
                    if (flashAlpha >= 100)
                    {
                        flashAlpha = 100;
                        flashPhase = 1;
                    }
                    break;
                case 1: // hold
                    flashPhase = 2;
                    break;
                case 2: // flash out
                    flashAlpha -= 8;
                    if (flashAlpha <= 0)
                    {
                        flashAlpha = 0;
                        timerFlash.Stop();
                        // Close the form after flash
                        Timer tClose = new Timer { Interval = 400 };
                        tClose.Tick += (s2, e2) =>
                        {
                            tClose.Stop();
                            tClose.Dispose();
                            DialogResult = DialogResult.OK;
                            Close();
                        };
                        tClose.Start();
                    }
                    break;
            }
            pnlFlashOverlay.Invalidate();
        }

        // ── Camera management ───────────────────────────────────────
        private void CargarCamaras()
        {
            dispositivos = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            cmbCamaras.Items.Clear();

            foreach (FilterInfo dispositivo in dispositivos)
                cmbCamaras.Items.Add(dispositivo.Name);

            if (cmbCamaras.Items.Count > 0)
                cmbCamaras.SelectedIndex = 0;
            else
            {
                lblEstado.Text = "⚠ No se encontró una cámara conectada.";
                lblEstado.ForeColor = Color.FromArgb(255, 100, 100);
            }
        }

        private void IniciarCamara()
        {
            if (cmbCamaras.SelectedIndex < 0 || camara != null)
                return;

            codigoEncontrado = false;
            camara = new VideoCaptureDevice(dispositivos[cmbCamaras.SelectedIndex].MonikerString);
            camara.NewFrame += Camara_NewFrame;
            camara.Start();
            lblEstado.Text = "🔍 Buscando código de barras...";
            lblEstado.ForeColor = Color.FromArgb(0, 150, 255);
            timerLaser.Start();
        }

        private void Camara_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (codigoEncontrado)
                return;

            Bitmap frame = null;
            Bitmap decodeFrame = null;
            try
            {
                frame = (Bitmap)eventArgs.Frame.Clone();
                decodeFrame = (Bitmap)frame.Clone();
                var resultado = lector.Decode(decodeFrame);

                BeginInvoke(new Action(() =>
                {
                    var anterior = picCamara.Image;
                    picCamara.Image = frame;
                    anterior?.Dispose();
                }));
                frame = null; // now owned by picCamara

                if (resultado != null && !codigoEncontrado)
                {
                    codigoEncontrado = true;
                    CodigoDetectado = resultado.Text;

                    // Beep sound
                    System.Media.SystemSounds.Beep.Play();

                    BeginInvoke(new Action(() =>
                    {
                        timerLaser.Stop();

                        // Update status to green success
                        lblEstado.Text = "✅ ¡CÓDIGO DETECTADO!";
                        lblEstado.ForeColor = Color.FromArgb(0, 220, 80);

                        // Show the detected code
                        lblCodigoResult.Text = CodigoDetectado;
                        lblCodigoResult.Visible = true;

                        // Repaint camera with green frame
                        picCamara.Invalidate();

                        // Start green flash animation
                        flashAlpha = 0;
                        flashPhase = 0;
                        pnlFlashOverlay.Visible = true;
                        timerFlash.Start();

                        // Update the status dot
                        foreach (Control c in pnlFooter.Controls)
                        {
                            if (c is Panel p && p.Size.Width == 12)
                                p.Invalidate();
                        }
                    }));
                }
            }
            catch
            {
                frame?.Dispose();
            }
            finally
            {
                decodeFrame?.Dispose();
            }
        }

        private void DetenerCamara()
        {
            timerLaser.Stop();
            if (camara != null)
            {
                camara.NewFrame -= Camara_NewFrame;
                if (camara.IsRunning)
                {
                    camara.SignalToStop();
                    camara.WaitForStop();
                }
                camara = null;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timerFlash.Stop();
            DetenerCamara();
            base.OnFormClosing(e);
        }
    }
}
