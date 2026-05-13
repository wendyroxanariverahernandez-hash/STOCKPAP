namespace STOCKPAP.Views.Controls
{
    partial class DashboardControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.flowStats = new System.Windows.Forms.FlowLayoutPanel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblDesc = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // flowStats
            // 
            this.flowStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowStats.Location = new System.Drawing.Point(30, 80);
            this.flowStats.Name = "flowStats";
            this.flowStats.Size = new System.Drawing.Size(920, 150);
            this.flowStats.TabIndex = 0;
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblWelcome.Location = new System.Drawing.Point(30, 20);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(412, 45);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "¡Bienvenido a STOCKPAP!";
            // 
            // lblDesc
            // 
            this.lblDesc.AutoSize = true;
            this.lblDesc.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblDesc.ForeColor = System.Drawing.Color.Gray;
            this.lblDesc.Location = new System.Drawing.Point(35, 65);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(517, 21);
            this.lblDesc.TabIndex = 2;
            this.lblDesc.Text = "Gestiona tu inventario de papelería de forma eficiente y profesional.";
            // 
            // DashboardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.flowStats);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "DashboardControl";
            this.Padding = new System.Windows.Forms.Padding(30);
            this.Size = new System.Drawing.Size(980, 730);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.FlowLayoutPanel flowStats;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblDesc;
    }
}
