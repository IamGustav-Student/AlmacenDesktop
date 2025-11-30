namespace AlmacenDesktop.Forms
{
    partial class ReporteFiadosForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelIzq = new System.Windows.Forms.Panel();
            this.btnRegistrarPago = new System.Windows.Forms.Button();
            this.lblSaldo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboClientes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvMovimientos = new System.Windows.Forms.DataGridView();
            this.lblTituloGrilla = new System.Windows.Forms.Label();

            this.panelIzq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).BeginInit();
            this.SuspendLayout();

            // 
            // panelIzq (Panel de Control)
            // 
            this.panelIzq.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelIzq.Controls.Add(this.btnRegistrarPago);
            this.panelIzq.Controls.Add(this.lblSaldo);
            this.panelIzq.Controls.Add(this.label2);
            this.panelIzq.Controls.Add(this.cboClientes);
            this.panelIzq.Controls.Add(this.label1);
            this.panelIzq.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelIzq.Location = new System.Drawing.Point(0, 0);
            this.panelIzq.Name = "panelIzq";
            this.panelIzq.Size = new System.Drawing.Size(300, 500);
            this.panelIzq.TabIndex = 0;

            // Selector Cliente
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 30);
            this.label1.Text = "Seleccione Cliente:";

            this.cboClientes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClientes.FormattingEnabled = true;
            this.cboClientes.Location = new System.Drawing.Point(20, 50);
            this.cboClientes.Name = "cboClientes";
            this.cboClientes.Size = new System.Drawing.Size(250, 23);
            this.cboClientes.TabIndex = 0;
            this.cboClientes.SelectedIndexChanged += new System.EventHandler(this.cboClientes_SelectedIndexChanged);

            // Tarjeta de Saldo
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label2.Location = new System.Drawing.Point(20, 120);
            this.label2.Text = "Saldo Deudor Actual:";

            this.lblSaldo.AutoSize = true;
            this.lblSaldo.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblSaldo.ForeColor = System.Drawing.Color.Firebrick;
            this.lblSaldo.Location = new System.Drawing.Point(20, 150);
            this.lblSaldo.Text = "$ 0.00";

            // Botón Pagar
            this.btnRegistrarPago.BackColor = System.Drawing.Color.SeaGreen;
            this.btnRegistrarPago.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistrarPago.ForeColor = System.Drawing.Color.White;
            this.btnRegistrarPago.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRegistrarPago.Location = new System.Drawing.Point(20, 250);
            this.btnRegistrarPago.Name = "btnRegistrarPago";
            this.btnRegistrarPago.Size = new System.Drawing.Size(250, 50);
            this.btnRegistrarPago.TabIndex = 1;
            this.btnRegistrarPago.Text = "REGISTRAR PAGO";
            this.btnRegistrarPago.UseVisualStyleBackColor = false;
            this.btnRegistrarPago.Enabled = false; // Deshabilitado hasta elegir cliente
            this.btnRegistrarPago.Click += new System.EventHandler(this.btnRegistrarPago_Click);

            // 
            // dgvMovimientos (Grilla Derecha)
            // 
            this.dgvMovimientos.AllowUserToAddRows = false;
            this.dgvMovimientos.AllowUserToDeleteRows = false;
            this.dgvMovimientos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMovimientos.BackgroundColor = System.Drawing.Color.White;
            this.dgvMovimientos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMovimientos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMovimientos.Location = new System.Drawing.Point(300, 40); // Dejamos espacio para título
            this.dgvMovimientos.Name = "dgvMovimientos";
            this.dgvMovimientos.ReadOnly = true;
            this.dgvMovimientos.Size = new System.Drawing.Size(500, 460);
            this.dgvMovimientos.TabIndex = 1;

            // Título Grilla
            this.lblTituloGrilla.BackColor = System.Drawing.Color.White;
            this.lblTituloGrilla.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTituloGrilla.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTituloGrilla.Location = new System.Drawing.Point(300, 0);
            this.lblTituloGrilla.Name = "lblTituloGrilla";
            this.lblTituloGrilla.Padding = new System.Windows.Forms.Padding(10);
            this.lblTituloGrilla.Size = new System.Drawing.Size(500, 40);
            this.lblTituloGrilla.Text = "Historial de Movimientos (Fiados y Pagos)";

            // Form
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.dgvMovimientos);
            this.Controls.Add(this.lblTituloGrilla);
            this.Controls.Add(this.panelIzq);
            this.Name = "ReporteFiadosForm";
            this.Text = "Estado de Cuenta Corriente";
            this.Load += new System.EventHandler(this.ReporteFiadosForm_Load);

            this.panelIzq.ResumeLayout(false);
            this.panelIzq.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelIzq;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboClientes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSaldo;
        private System.Windows.Forms.Button btnRegistrarPago;
        private System.Windows.Forms.DataGridView dgvMovimientos;
        private System.Windows.Forms.Label lblTituloGrilla;
    }
}