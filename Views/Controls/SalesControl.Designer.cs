namespace STOCKPAP.Views.Controls
{
    partial class SalesControl
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnNewSale = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvSales = new System.Windows.Forms.DataGridView();
            this.pnlNewSale = new System.Windows.Forms.Panel();
            
            // Base controls
            this.lblCategoria = new System.Windows.Forms.Label();
            this.cmbCategoria = new System.Windows.Forms.ComboBox();
            this.lblProducto = new System.Windows.Forms.Label();
            this.cmbProducto = new System.Windows.Forms.ComboBox();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            this.lblMetodo = new System.Windows.Forms.Label();
            this.cmbMetodo = new System.Windows.Forms.ComboBox();
            
            // Dynamic panels
            this.pnlEfectivo = new System.Windows.Forms.Panel();
            this.lblCantidadRecibida = new System.Windows.Forms.Label();
            this.txtCantidadRecibida = new System.Windows.Forms.TextBox();
            this.lblCambio = new System.Windows.Forms.Label();
            this.txtCambio = new System.Windows.Forms.TextBox();
            
            this.pnlTarjeta = new System.Windows.Forms.Panel();
            this.lblTipoTarjeta = new System.Windows.Forms.Label();
            this.cmbTipoTarjeta = new System.Windows.Forms.ComboBox();
            this.lblBanco = new System.Windows.Forms.Label();
            this.txtBanco = new System.Windows.Forms.TextBox();
            this.lblUltimos4 = new System.Windows.Forms.Label();
            this.txtUltimos4 = new System.Windows.Forms.TextBox();
            this.lblReferencia = new System.Windows.Forms.Label();
            this.txtReferencia = new System.Windows.Forms.TextBox();
            
            this.pnlTransferencia = new System.Windows.Forms.Panel();
            this.lblBancoEmisor = new System.Windows.Forms.Label();
            this.txtBancoEmisor = new System.Windows.Forms.TextBox();
            this.lblReferenciaSPEI = new System.Windows.Forms.Label();
            this.txtReferenciaSPEI = new System.Windows.Forms.TextBox();
            this.chkConfirmacion = new System.Windows.Forms.CheckBox();

            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTotalVal = new System.Windows.Forms.Label();
            this.btnConfirmSale = new System.Windows.Forms.Button();
            this.btnCancelSale = new System.Windows.Forms.Button();
            
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).BeginInit();
            this.pnlNewSale.SuspendLayout();
            
            this.pnlEfectivo.SuspendLayout();
            this.pnlTarjeta.SuspendLayout();
            this.pnlTransferencia.SuspendLayout();
            this.SuspendLayout();
            
            // pnlTop
            this.pnlTop.Controls.Add(this.btnNewSale);
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(980, 60);
            this.pnlTop.TabIndex = 0;
            
            // btnNewSale
            this.btnNewSale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewSale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(135)))), ((int)(((byte)(84)))));
            this.btnNewSale.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewSale.ForeColor = System.Drawing.Color.White;
            this.btnNewSale.Location = new System.Drawing.Point(850, 15);
            this.btnNewSale.Name = "btnNewSale";
            this.btnNewSale.Size = new System.Drawing.Size(110, 30);
            this.btnNewSale.TabIndex = 1;
            this.btnNewSale.Text = "+ Nueva Venta";
            this.btnNewSale.UseVisualStyleBackColor = false;
            
            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(151, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Registro de Ventas";
            
            // dgvSales
            this.dgvSales.AllowUserToAddRows = false;
            this.dgvSales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSales.BackgroundColor = System.Drawing.Color.White;
            this.dgvSales.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSales.Location = new System.Drawing.Point(0, 60);
            this.dgvSales.Name = "dgvSales";
            this.dgvSales.ReadOnly = true;
            this.dgvSales.RowHeadersVisible = false;
            this.dgvSales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSales.Size = new System.Drawing.Size(980, 670);
            this.dgvSales.TabIndex = 1;
            
            // pnlNewSale
            this.pnlNewSale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlNewSale.Controls.Add(this.lblCategoria);
            this.pnlNewSale.Controls.Add(this.cmbCategoria);
            this.pnlNewSale.Controls.Add(this.lblProducto);
            this.pnlNewSale.Controls.Add(this.cmbProducto);
            this.pnlNewSale.Controls.Add(this.lblCantidad);
            this.pnlNewSale.Controls.Add(this.txtCantidad);
            this.pnlNewSale.Controls.Add(this.lblMetodo);
            this.pnlNewSale.Controls.Add(this.cmbMetodo);
            
            this.pnlNewSale.Controls.Add(this.pnlEfectivo);
            this.pnlNewSale.Controls.Add(this.pnlTarjeta);
            this.pnlNewSale.Controls.Add(this.pnlTransferencia);
            
            this.pnlNewSale.Controls.Add(this.lblTotal);
            this.pnlNewSale.Controls.Add(this.lblTotalVal);
            this.pnlNewSale.Controls.Add(this.btnConfirmSale);
            this.pnlNewSale.Controls.Add(this.btnCancelSale);
            this.pnlNewSale.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlNewSale.Location = new System.Drawing.Point(680, 60);
            this.pnlNewSale.Name = "pnlNewSale";
            this.pnlNewSale.Size = new System.Drawing.Size(300, 670);
            this.pnlNewSale.TabIndex = 2;
            this.pnlNewSale.Visible = false;
            
            // Categoria
            this.lblCategoria.AutoSize = true;
            this.lblCategoria.Location = new System.Drawing.Point(20, 20);
            this.lblCategoria.Name = "lblCategoria";
            this.lblCategoria.Size = new System.Drawing.Size(61, 15);
            this.lblCategoria.Text = "Categoría:";
            
            this.cmbCategoria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategoria.FormattingEnabled = true;
            this.cmbCategoria.Items.AddRange(new object[] {
            "cuadernos y libretas", "escritura", "papel", "arte y dibujo", "oficina", "tecnologia", "impresion y copias", "escolares", "decoracion y regalos"});
            this.cmbCategoria.Location = new System.Drawing.Point(20, 40);
            this.cmbCategoria.Name = "cmbCategoria";
            this.cmbCategoria.Size = new System.Drawing.Size(260, 23);
            
            // Producto
            this.lblProducto.AutoSize = true;
            this.lblProducto.Location = new System.Drawing.Point(20, 70);
            this.lblProducto.Name = "lblProducto";
            this.lblProducto.Size = new System.Drawing.Size(59, 15);
            this.lblProducto.Text = "Producto:";
            
            this.cmbProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProducto.FormattingEnabled = true;
            this.cmbProducto.Location = new System.Drawing.Point(20, 90);
            this.cmbProducto.Name = "cmbProducto";
            this.cmbProducto.Size = new System.Drawing.Size(260, 23);
            
            // Cantidad
            this.lblCantidad.AutoSize = true;
            this.lblCantidad.Location = new System.Drawing.Point(20, 120);
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.Size = new System.Drawing.Size(58, 15);
            this.lblCantidad.Text = "Cantidad:";
            
            this.txtCantidad.Location = new System.Drawing.Point(20, 140);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.Size = new System.Drawing.Size(260, 23);
            this.txtCantidad.Text = "1";
            
            // Metodo
            this.lblMetodo.AutoSize = true;
            this.lblMetodo.Location = new System.Drawing.Point(20, 170);
            this.lblMetodo.Name = "lblMetodo";
            this.lblMetodo.Size = new System.Drawing.Size(81, 15);
            this.lblMetodo.Text = "Método Pago:";
            
            this.cmbMetodo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMetodo.FormattingEnabled = true;
            this.cmbMetodo.Items.AddRange(new object[] { "Efectivo", "Tarjeta", "Transferencia" });
            this.cmbMetodo.Location = new System.Drawing.Point(20, 190);
            this.cmbMetodo.Name = "cmbMetodo";
            this.cmbMetodo.Size = new System.Drawing.Size(260, 23);
            
            // pnlEfectivo
            this.pnlEfectivo.Location = new System.Drawing.Point(10, 220);
            this.pnlEfectivo.Size = new System.Drawing.Size(280, 220);
            this.pnlEfectivo.Controls.Add(this.lblCantidadRecibida);
            this.pnlEfectivo.Controls.Add(this.txtCantidadRecibida);
            this.pnlEfectivo.Controls.Add(this.lblCambio);
            this.pnlEfectivo.Controls.Add(this.txtCambio);
            
            this.lblCantidadRecibida.AutoSize = true;
            this.lblCantidadRecibida.Location = new System.Drawing.Point(10, 10);
            this.lblCantidadRecibida.Text = "Cantidad recibida:";
            
            this.txtCantidadRecibida.Location = new System.Drawing.Point(10, 30);
            this.txtCantidadRecibida.Size = new System.Drawing.Size(260, 23);
            
            this.lblCambio.AutoSize = true;
            this.lblCambio.Location = new System.Drawing.Point(10, 60);
            this.lblCambio.Text = "Cambio:";
            
            this.txtCambio.Location = new System.Drawing.Point(10, 80);
            this.txtCambio.Size = new System.Drawing.Size(260, 23);
            this.txtCambio.ReadOnly = true;

            // pnlTarjeta
            this.pnlTarjeta.Location = new System.Drawing.Point(10, 220);
            this.pnlTarjeta.Size = new System.Drawing.Size(280, 220);
            this.pnlTarjeta.Controls.Add(this.lblTipoTarjeta);
            this.pnlTarjeta.Controls.Add(this.cmbTipoTarjeta);
            this.pnlTarjeta.Controls.Add(this.lblBanco);
            this.pnlTarjeta.Controls.Add(this.txtBanco);
            this.pnlTarjeta.Controls.Add(this.lblUltimos4);
            this.pnlTarjeta.Controls.Add(this.txtUltimos4);
            this.pnlTarjeta.Controls.Add(this.lblReferencia);
            this.pnlTarjeta.Controls.Add(this.txtReferencia);
            
            this.lblTipoTarjeta.AutoSize = true;
            this.lblTipoTarjeta.Location = new System.Drawing.Point(10, 10);
            this.lblTipoTarjeta.Text = "Tipo tarjeta:";
            
            this.cmbTipoTarjeta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoTarjeta.Items.AddRange(new object[] { "Débito", "Crédito" });
            this.cmbTipoTarjeta.Location = new System.Drawing.Point(10, 30);
            this.cmbTipoTarjeta.Size = new System.Drawing.Size(260, 23);
            
            this.lblBanco.AutoSize = true;
            this.lblBanco.Location = new System.Drawing.Point(10, 60);
            this.lblBanco.Text = "Banco (Opcional):";
            
            this.txtBanco.Location = new System.Drawing.Point(10, 80);
            this.txtBanco.Size = new System.Drawing.Size(260, 23);
            
            this.lblUltimos4.AutoSize = true;
            this.lblUltimos4.Location = new System.Drawing.Point(10, 110);
            this.lblUltimos4.Text = "Últimos 4 dígitos:";
            
            this.txtUltimos4.Location = new System.Drawing.Point(10, 130);
            this.txtUltimos4.Size = new System.Drawing.Size(260, 23);
            this.txtUltimos4.MaxLength = 4;
            
            this.lblReferencia.AutoSize = true;
            this.lblReferencia.Location = new System.Drawing.Point(10, 160);
            this.lblReferencia.Text = "Referencia:";
            
            this.txtReferencia.Location = new System.Drawing.Point(10, 180);
            this.txtReferencia.Size = new System.Drawing.Size(260, 23);
            
            // pnlTransferencia
            this.pnlTransferencia.Location = new System.Drawing.Point(10, 220);
            this.pnlTransferencia.Size = new System.Drawing.Size(280, 220);
            this.pnlTransferencia.Controls.Add(this.lblBancoEmisor);
            this.pnlTransferencia.Controls.Add(this.txtBancoEmisor);
            this.pnlTransferencia.Controls.Add(this.lblReferenciaSPEI);
            this.pnlTransferencia.Controls.Add(this.txtReferenciaSPEI);
            this.pnlTransferencia.Controls.Add(this.chkConfirmacion);
            
            this.lblBancoEmisor.AutoSize = true;
            this.lblBancoEmisor.Location = new System.Drawing.Point(10, 10);
            this.lblBancoEmisor.Text = "Banco emisor (Opcional):";
            
            this.txtBancoEmisor.Location = new System.Drawing.Point(10, 30);
            this.txtBancoEmisor.Size = new System.Drawing.Size(260, 23);
            
            this.lblReferenciaSPEI.AutoSize = true;
            this.lblReferenciaSPEI.Location = new System.Drawing.Point(10, 60);
            this.lblReferenciaSPEI.Text = "Referencia SPEI:";
            
            this.txtReferenciaSPEI.Location = new System.Drawing.Point(10, 80);
            this.txtReferenciaSPEI.Size = new System.Drawing.Size(260, 23);
            
            this.chkConfirmacion.AutoSize = true;
            this.chkConfirmacion.Location = new System.Drawing.Point(10, 120);
            this.chkConfirmacion.Text = "Validar pago (Confirmación)";
            
            // Total
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(20, 470);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(35, 15);
            this.lblTotal.Text = "Total:";
            
            this.lblTotalVal.AutoSize = true;
            this.lblTotalVal.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotalVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(135)))), ((int)(((byte)(84)))));
            this.lblTotalVal.Location = new System.Drawing.Point(20, 490);
            this.lblTotalVal.Name = "lblTotalVal";
            this.lblTotalVal.Size = new System.Drawing.Size(78, 32);
            this.lblTotalVal.Text = "$0.00";
            
            // Buttons
            this.btnConfirmSale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(135)))), ((int)(((byte)(84)))));
            this.btnConfirmSale.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmSale.ForeColor = System.Drawing.Color.White;
            this.btnConfirmSale.Location = new System.Drawing.Point(20, 540);
            this.btnConfirmSale.Name = "btnConfirmSale";
            this.btnConfirmSale.Size = new System.Drawing.Size(260, 40);
            this.btnConfirmSale.Text = "Confirmar Venta";
            this.btnConfirmSale.UseVisualStyleBackColor = false;
            
            this.btnCancelSale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCancelSale.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelSale.ForeColor = System.Drawing.Color.White;
            this.btnCancelSale.Location = new System.Drawing.Point(20, 590);
            this.btnCancelSale.Name = "btnCancelSale";
            this.btnCancelSale.Size = new System.Drawing.Size(260, 30);
            this.btnCancelSale.Text = "Cancelar";
            this.btnCancelSale.UseVisualStyleBackColor = false;

            // SalesControl
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlNewSale);
            this.Controls.Add(this.dgvSales);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "SalesControl";
            this.Size = new System.Drawing.Size(980, 730);
            
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).EndInit();
            
            this.pnlEfectivo.ResumeLayout(false);
            this.pnlEfectivo.PerformLayout();
            this.pnlTarjeta.ResumeLayout(false);
            this.pnlTarjeta.PerformLayout();
            this.pnlTransferencia.ResumeLayout(false);
            this.pnlTransferencia.PerformLayout();
            
            this.pnlNewSale.ResumeLayout(false);
            this.pnlNewSale.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnNewSale;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridView dgvSales;
        private System.Windows.Forms.Panel pnlNewSale;
        
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.ComboBox cmbCategoria;
        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.ComboBox cmbProducto;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.TextBox txtCantidad;
        private System.Windows.Forms.Label lblMetodo;
        private System.Windows.Forms.ComboBox cmbMetodo;
        
        private System.Windows.Forms.Panel pnlEfectivo;
        private System.Windows.Forms.Label lblCantidadRecibida;
        private System.Windows.Forms.TextBox txtCantidadRecibida;
        private System.Windows.Forms.Label lblCambio;
        private System.Windows.Forms.TextBox txtCambio;
        
        private System.Windows.Forms.Panel pnlTarjeta;
        private System.Windows.Forms.Label lblTipoTarjeta;
        private System.Windows.Forms.ComboBox cmbTipoTarjeta;
        private System.Windows.Forms.Label lblBanco;
        private System.Windows.Forms.TextBox txtBanco;
        private System.Windows.Forms.Label lblUltimos4;
        private System.Windows.Forms.TextBox txtUltimos4;
        private System.Windows.Forms.Label lblReferencia;
        private System.Windows.Forms.TextBox txtReferencia;
        
        private System.Windows.Forms.Panel pnlTransferencia;
        private System.Windows.Forms.Label lblBancoEmisor;
        private System.Windows.Forms.TextBox txtBancoEmisor;
        private System.Windows.Forms.Label lblReferenciaSPEI;
        private System.Windows.Forms.TextBox txtReferenciaSPEI;
        private System.Windows.Forms.CheckBox chkConfirmacion;

        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblTotalVal;
        private System.Windows.Forms.Button btnConfirmSale;
        private System.Windows.Forms.Button btnCancelSale;
    }
}
