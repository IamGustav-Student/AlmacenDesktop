namespace AlmacenDesktop.Forms
{
    partial class VentasForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblProducto = new System.Windows.Forms.Label();
            this.txtEscanear = new System.Windows.Forms.TextBox();
            this.cboProductos = new System.Windows.Forms.ComboBox();
            this.numCantidad = new System.Windows.Forms.NumericUpDown();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.dgvCarrito = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnFinalizar = new System.Windows.Forms.Button();
            this.cboClientes = new System.Windows.Forms.ComboBox();
            this.cboMetodoPago = new System.Windows.Forms.ComboBox();
            this.lblCliente = new System.Windows.Forms.Label();
            this.lblMetodo = new System.Windows.Forms.Label();

            // NUEVOS CONTROLES
            this.grpPago = new System.Windows.Forms.GroupBox();
            this.lblPagoCon = new System.Windows.Forms.Label();
            this.txtPagaCon = new System.Windows.Forms.TextBox();
            this.lblVuelto = new System.Windows.Forms.Label();
            this.lblVueltoMonto = new System.Windows.Forms.Label();
            this.lblRecargoInfo = new System.Windows.Forms.Label();

            this.grpAtajos = new System.Windows.Forms.GroupBox();
            this.lblAtajo1 = new System.Windows.Forms.Label();
            this.lblAtajo2 = new System.Windows.Forms.Label();
            this.lblAtajo3 = new System.Windows.Forms.Label();
            this.lblAtajo4 = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito)).BeginInit();
            this.grpPago.SuspendLayout();
            this.grpAtajos.SuspendLayout();
            this.SuspendLayout();

            // 
            // lblProducto
            // 
            this.lblProducto.AutoSize = true;
            this.lblProducto.Location = new System.Drawing.Point(20, 20);
            this.lblProducto.Name = "lblProducto";
            this.lblProducto.Size = new System.Drawing.Size(165, 15);
            this.lblProducto.TabIndex = 0;
            this.lblProducto.Text = "Escanear Código o Buscar (F1):";

            // 
            // txtEscanear
            // 
            this.txtEscanear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtEscanear.Location = new System.Drawing.Point(20, 40);
            this.txtEscanear.Name = "txtEscanear";
            this.txtEscanear.Size = new System.Drawing.Size(300, 29);
            this.txtEscanear.TabIndex = 0;
            this.txtEscanear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEscanear_KeyPress);

            // 
            // cboProductos (Oculto visualmente o secundario, se mantiene por compatibilidad)
            // 
            this.cboProductos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProductos.FormattingEnabled = true;
            this.cboProductos.Location = new System.Drawing.Point(340, 44);
            this.cboProductos.Name = "cboProductos";
            this.cboProductos.Size = new System.Drawing.Size(200, 23);
            this.cboProductos.TabIndex = 10;

            // 
            // numCantidad
            // 
            this.numCantidad.Location = new System.Drawing.Point(550, 44);
            this.numCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numCantidad.Name = "numCantidad";
            this.numCantidad.Size = new System.Drawing.Size(50, 23);
            this.numCantidad.TabIndex = 11;
            this.numCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });

            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(610, 40);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(80, 30);
            this.btnAgregar.TabIndex = 12;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);

            // 
            // dgvCarrito
            // 
            this.dgvCarrito.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCarrito.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCarrito.BackgroundColor = System.Drawing.Color.White;
            this.dgvCarrito.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCarrito.Location = new System.Drawing.Point(20, 90);
            this.dgvCarrito.Name = "dgvCarrito";
            this.dgvCarrito.RowTemplate.Height = 25;
            this.dgvCarrito.Size = new System.Drawing.Size(670, 300); // Ajustado para dar espacio a la derecha
            this.dgvCarrito.TabIndex = 15;
            this.dgvCarrito.TabStop = false; // No focus con tab

            // --- PANEL DERECHO DE COBRO ---

            // Lbl Cliente
            this.lblCliente.AutoSize = true;
            this.lblCliente.Location = new System.Drawing.Point(710, 20);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Text = "Cliente (F4 para Fiado):";

            // Cbo Cliente
            this.cboClientes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboClientes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClientes.FormattingEnabled = true;
            this.cboClientes.Location = new System.Drawing.Point(710, 40);
            this.cboClientes.Name = "cboClientes";
            this.cboClientes.Size = new System.Drawing.Size(250, 23);
            this.cboClientes.TabIndex = 1; // Tab index después del scanner

            // Lbl Metodo
            this.lblMetodo.AutoSize = true;
            this.lblMetodo.Location = new System.Drawing.Point(710, 80);
            this.lblMetodo.Name = "lblMetodo";
            this.lblMetodo.Text = "Método de Pago (F2/F3):";

            // Cbo Metodo
            this.cboMetodoPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboMetodoPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMetodoPago.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cboMetodoPago.FormattingEnabled = true;
            this.cboMetodoPago.Location = new System.Drawing.Point(710, 100);
            this.cboMetodoPago.Name = "cboMetodoPago";
            this.cboMetodoPago.Size = new System.Drawing.Size(250, 28);
            this.cboMetodoPago.TabIndex = 2;
            this.cboMetodoPago.SelectedIndexChanged += new System.EventHandler(this.cboMetodoPago_SelectedIndexChanged);

            // 
            // grpPago (CALCULADORA DE VUELTO)
            // 
            this.grpPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPago.Controls.Add(this.lblPagoCon);
            this.grpPago.Controls.Add(this.txtPagaCon);
            this.grpPago.Controls.Add(this.lblVuelto);
            this.grpPago.Controls.Add(this.lblVueltoMonto);
            this.grpPago.Location = new System.Drawing.Point(710, 150);
            this.grpPago.Name = "grpPago";
            this.grpPago.Size = new System.Drawing.Size(250, 130);
            this.grpPago.TabIndex = 3;
            this.grpPago.TabStop = false;
            this.grpPago.Text = "Cálculo de Vuelto";

            this.lblPagoCon.AutoSize = true;
            this.lblPagoCon.Location = new System.Drawing.Point(15, 30);
            this.lblPagoCon.Text = "Paga con ($):";

            this.txtPagaCon.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtPagaCon.Location = new System.Drawing.Point(15, 50);
            this.txtPagaCon.Name = "txtPagaCon";
            this.txtPagaCon.Size = new System.Drawing.Size(220, 32);
            this.txtPagaCon.TabIndex = 0; // Foco aquí para Enter final
            this.txtPagaCon.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPagaCon.TextChanged += new System.EventHandler(this.txtPagaCon_TextChanged);
            this.txtPagaCon.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPagaCon_KeyPress);

            this.lblVuelto.AutoSize = true;
            this.lblVuelto.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblVuelto.Location = new System.Drawing.Point(15, 95);
            this.lblVuelto.Text = "SU VUELTO:";

            this.lblVueltoMonto.AutoSize = true;
            this.lblVueltoMonto.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblVueltoMonto.ForeColor = System.Drawing.Color.Green;
            this.lblVueltoMonto.Location = new System.Drawing.Point(110, 93);
            this.lblVueltoMonto.Text = "$ 0.00";

            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTotal.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTotal.Location = new System.Drawing.Point(710, 320);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(250, 45);
            this.lblTotal.TabIndex = 5;
            this.lblTotal.Text = "$ 0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // Lbl Recargo Info
            this.lblRecargoInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRecargoInfo.AutoSize = true;
            this.lblRecargoInfo.ForeColor = System.Drawing.Color.Red;
            this.lblRecargoInfo.Location = new System.Drawing.Point(780, 365);
            this.lblRecargoInfo.Name = "lblRecargoInfo";
            this.lblRecargoInfo.Text = "+6% Interés Aplicado";
            this.lblRecargoInfo.Visible = false;

            // 
            // btnFinalizar
            // 
            this.btnFinalizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFinalizar.BackColor = System.Drawing.Color.ForestGreen;
            this.btnFinalizar.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnFinalizar.ForeColor = System.Drawing.Color.White;
            this.btnFinalizar.Location = new System.Drawing.Point(710, 390);
            this.btnFinalizar.Name = "btnFinalizar";
            this.btnFinalizar.Size = new System.Drawing.Size(250, 60);
            this.btnFinalizar.TabIndex = 4;
            this.btnFinalizar.Text = "COBRAR (F5)";
            this.btnFinalizar.UseVisualStyleBackColor = false;
            this.btnFinalizar.Click += new System.EventHandler(this.btnFinalizar_Click);

            // 
            // grpAtajos (VISUAL PARA EL USUARIO)
            // 
            this.grpAtajos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpAtajos.Controls.Add(this.lblAtajo1);
            this.grpAtajos.Controls.Add(this.lblAtajo2);
            this.grpAtajos.Controls.Add(this.lblAtajo3);
            this.grpAtajos.Controls.Add(this.lblAtajo4);
            this.grpAtajos.ForeColor = System.Drawing.Color.DimGray;
            this.grpAtajos.Location = new System.Drawing.Point(20, 400);
            this.grpAtajos.Name = "grpAtajos";
            this.grpAtajos.Size = new System.Drawing.Size(670, 50);
            this.grpAtajos.TabIndex = 20;
            this.grpAtajos.TabStop = false;
            this.grpAtajos.Text = "Atajos de Teclado";

            this.lblAtajo1.AutoSize = true;
            this.lblAtajo1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAtajo1.Location = new System.Drawing.Point(20, 22);
            this.lblAtajo1.Text = "F1: Escanear";

            this.lblAtajo2.AutoSize = true;
            this.lblAtajo2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAtajo2.Location = new System.Drawing.Point(120, 22);
            this.lblAtajo2.Text = "F2: Efectivo";

            this.lblAtajo3.AutoSize = true;
            this.lblAtajo3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAtajo3.Location = new System.Drawing.Point(220, 22);
            this.lblAtajo3.Text = "F3: Billetera Virtual (6%)";

            this.lblAtajo4.AutoSize = true;
            this.lblAtajo4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAtajo4.Location = new System.Drawing.Point(380, 22);
            this.lblAtajo4.Text = "ENTER: Finalizar Venta";

            // 
            // VentasForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 461);
            this.Controls.Add(this.grpAtajos);
            this.Controls.Add(this.lblRecargoInfo);
            this.Controls.Add(this.grpPago);
            this.Controls.Add(this.lblMetodo);
            this.Controls.Add(this.cboMetodoPago);
            this.Controls.Add(this.lblCliente);
            this.Controls.Add(this.cboClientes);
            this.Controls.Add(this.btnFinalizar);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvCarrito);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.numCantidad);
            this.Controls.Add(this.cboProductos);
            this.Controls.Add(this.txtEscanear);
            this.Controls.Add(this.lblProducto);
            this.Name = "VentasForm";
            this.Text = "Punto de Venta";
            this.Load += new System.EventHandler(this.VentasForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito)).EndInit();
            this.grpPago.ResumeLayout(false);
            this.grpPago.PerformLayout();
            this.grpAtajos.ResumeLayout(false);
            this.grpAtajos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.TextBox txtEscanear;
        private System.Windows.Forms.ComboBox cboProductos;
        private System.Windows.Forms.NumericUpDown numCantidad;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.DataGridView dgvCarrito;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnFinalizar;
        private System.Windows.Forms.ComboBox cboClientes;
        private System.Windows.Forms.ComboBox cboMetodoPago;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.Label lblMetodo;

        // Controles Nuevos
        private System.Windows.Forms.GroupBox grpPago;
        private System.Windows.Forms.Label lblPagoCon;
        private System.Windows.Forms.TextBox txtPagaCon;
        private System.Windows.Forms.Label lblVuelto;
        private System.Windows.Forms.Label lblVueltoMonto;
        private System.Windows.Forms.Label lblRecargoInfo;

        private System.Windows.Forms.GroupBox grpAtajos;
        private System.Windows.Forms.Label lblAtajo1;
        private System.Windows.Forms.Label lblAtajo2;
        private System.Windows.Forms.Label lblAtajo3;
        private System.Windows.Forms.Label lblAtajo4;
    }
}