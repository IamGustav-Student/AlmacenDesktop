namespace AlmacenDesktop.Forms
{
    partial class CuentaCorrienteForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.cboClientes = new System.Windows.Forms.ComboBox();
            this.lblSelCliente = new System.Windows.Forms.Label();
            this.pnlSaldo = new System.Windows.Forms.Panel();
            this.lblEstadoDeuda = new System.Windows.Forms.Label();
            this.lblSaldo = new System.Windows.Forms.Label();
            this.lblTituloSaldo = new System.Windows.Forms.Label();
            this.dgvHistoria = new System.Windows.Forms.DataGridView();
            this.grpPago = new System.Windows.Forms.GroupBox();
            this.btnRegistrarPago = new System.Windows.Forms.Button();
            this.numMontoPago = new System.Windows.Forms.NumericUpDown();
            this.lblMonto = new System.Windows.Forms.Label();

            this.pnlSaldo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistoria)).BeginInit();
            this.grpPago.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMontoPago)).BeginInit();
            this.SuspendLayout();

            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.DimGray;
            this.lblTitulo.Location = new System.Drawing.Point(20, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(215, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Cuentas Corrientes";

            // 
            // lblSelCliente
            // 
            this.lblSelCliente.AutoSize = true;
            this.lblSelCliente.Location = new System.Drawing.Point(25, 70);
            this.lblSelCliente.Text = "Seleccionar Cliente:";

            // 
            // cboClientes
            // 
            this.cboClientes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClientes.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboClientes.FormattingEnabled = true;
            this.cboClientes.Location = new System.Drawing.Point(25, 90);
            this.cboClientes.Name = "cboClientes";
            this.cboClientes.Size = new System.Drawing.Size(300, 28);
            this.cboClientes.SelectedIndexChanged += new System.EventHandler(this.cboClientes_SelectedIndexChanged);

            // 
            // pnlSaldo
            // 
            this.pnlSaldo.BackColor = System.Drawing.Color.White;
            this.pnlSaldo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSaldo.Controls.Add(this.lblEstadoDeuda);
            this.pnlSaldo.Controls.Add(this.lblSaldo);
            this.pnlSaldo.Controls.Add(this.lblTituloSaldo);
            this.pnlSaldo.Location = new System.Drawing.Point(350, 90);
            this.pnlSaldo.Name = "pnlSaldo";
            this.pnlSaldo.Size = new System.Drawing.Size(250, 100);

            // 
            // lblTituloSaldo
            // 
            this.lblTituloSaldo.AutoSize = true;
            this.lblTituloSaldo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTituloSaldo.ForeColor = System.Drawing.Color.Gray;
            this.lblTituloSaldo.Location = new System.Drawing.Point(10, 10);
            this.lblTituloSaldo.Text = "Saldo Actual:";

            // 
            // lblSaldo
            // 
            this.lblSaldo.AutoSize = true;
            this.lblSaldo.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblSaldo.Location = new System.Drawing.Point(10, 30);
            this.lblSaldo.Text = "$ 0.00";

            // 
            // lblEstadoDeuda
            // 
            this.lblEstadoDeuda.AutoSize = true;
            this.lblEstadoDeuda.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstadoDeuda.Location = new System.Drawing.Point(15, 75);
            this.lblEstadoDeuda.Text = "-";

            // 
            // grpPago
            // 
            this.grpPago.Controls.Add(this.btnRegistrarPago);
            this.grpPago.Controls.Add(this.numMontoPago);
            this.grpPago.Controls.Add(this.lblMonto);
            this.grpPago.Location = new System.Drawing.Point(620, 85);
            this.grpPago.Name = "grpPago";
            this.grpPago.Size = new System.Drawing.Size(250, 105);
            this.grpPago.TabIndex = 5;
            this.grpPago.TabStop = false;
            this.grpPago.Text = "Registrar Pago / Entrega";

            // 
            // lblMonto
            // 
            this.lblMonto.AutoSize = true;
            this.lblMonto.Location = new System.Drawing.Point(15, 25);
            this.lblMonto.Text = "Monto ($):";

            // 
            // numMontoPago
            // 
            this.numMontoPago.DecimalPlaces = 2;
            this.numMontoPago.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.numMontoPago.Location = new System.Drawing.Point(15, 45);
            this.numMontoPago.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            this.numMontoPago.Size = new System.Drawing.Size(120, 27);

            // 
            // btnRegistrarPago
            // 
            this.btnRegistrarPago.BackColor = System.Drawing.Color.ForestGreen;
            this.btnRegistrarPago.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistrarPago.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRegistrarPago.ForeColor = System.Drawing.Color.White;
            this.btnRegistrarPago.Location = new System.Drawing.Point(145, 42);
            this.btnRegistrarPago.Name = "btnRegistrarPago";
            this.btnRegistrarPago.Size = new System.Drawing.Size(90, 32);
            this.btnRegistrarPago.Text = "COBRAR";
            this.btnRegistrarPago.UseVisualStyleBackColor = false;
            this.btnRegistrarPago.Click += new System.EventHandler(this.btnRegistrarPago_Click);

            // 
            // dgvHistoria
            // 
            this.dgvHistoria.AllowUserToAddRows = false;
            this.dgvHistoria.AllowUserToDeleteRows = false;
            this.dgvHistoria.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvHistoria.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistoria.BackgroundColor = System.Drawing.Color.White;
            this.dgvHistoria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistoria.Location = new System.Drawing.Point(25, 210);
            this.dgvHistoria.Name = "dgvHistoria";
            this.dgvHistoria.ReadOnly = true;
            this.dgvHistoria.RowHeadersVisible = false;
            this.dgvHistoria.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistoria.Size = new System.Drawing.Size(845, 330);
            // --- CONEXIÓN DEL EVENTO ---
            this.dgvHistoria.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHistoria_CellDoubleClick);

            // 
            // CuentaCorrienteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(900, 560);
            this.Controls.Add(this.dgvHistoria);
            this.Controls.Add(this.grpPago);
            this.Controls.Add(this.pnlSaldo);
            this.Controls.Add(this.cboClientes);
            this.Controls.Add(this.lblSelCliente);
            this.Controls.Add(this.lblTitulo);
            this.Name = "CuentaCorrienteForm";
            this.Text = "Cuentas Corrientes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.CuentaCorrienteForm_Load);

            this.pnlSaldo.ResumeLayout(false);
            this.pnlSaldo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistoria)).EndInit();
            this.grpPago.ResumeLayout(false);
            this.grpPago.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMontoPago)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.ComboBox cboClientes;
        private System.Windows.Forms.Label lblSelCliente;
        private System.Windows.Forms.Panel pnlSaldo;
        private System.Windows.Forms.Label lblSaldo;
        private System.Windows.Forms.Label lblTituloSaldo;
        private System.Windows.Forms.Label lblEstadoDeuda;
        private System.Windows.Forms.DataGridView dgvHistoria;
        private System.Windows.Forms.GroupBox grpPago;
        private System.Windows.Forms.Button btnRegistrarPago;
        private System.Windows.Forms.NumericUpDown numMontoPago;
        private System.Windows.Forms.Label lblMonto;
    }
}