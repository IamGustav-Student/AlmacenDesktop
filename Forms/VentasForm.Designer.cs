namespace AlmacenDesktop.Forms
{
    partial class VentasForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboMetodoPago = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboClientes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtEscanear = new System.Windows.Forms.TextBox();
            this.labelEscanear = new System.Windows.Forms.Label();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.numCantidad = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.cboProductos = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvCarrito = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnFinalizar = new System.Windows.Forms.Button();

            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito)).BeginInit();
            this.SuspendLayout();

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Text = "Nueva Venta";

            // 
            // groupBox1 (Datos de la Venta)
            // 
            this.groupBox1.Controls.Add(this.cboMetodoPago);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cboClientes);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(18, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(760, 70);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Datos de Pago";

            // label4 (Método Pago)
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(20, 30);
            this.label4.Text = "Método Pago:";

            // cboMetodoPago (NUEVO)
            this.cboMetodoPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMetodoPago.FormattingEnabled = true;
            this.cboMetodoPago.Location = new System.Drawing.Point(110, 27);
            this.cboMetodoPago.Name = "cboMetodoPago";
            this.cboMetodoPago.Size = new System.Drawing.Size(150, 23);
            this.cboMetodoPago.TabIndex = 0;
            this.cboMetodoPago.SelectedIndexChanged += new System.EventHandler(this.cboMetodoPago_SelectedIndexChanged);

            // label1 (Cliente)
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(280, 30);
            this.label1.Text = "Cliente:";

            // cboClientes
            this.cboClientes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClientes.FormattingEnabled = true;
            this.cboClientes.Location = new System.Drawing.Point(330, 27);
            this.cboClientes.Name = "cboClientes";
            this.cboClientes.Size = new System.Drawing.Size(400, 23);
            this.cboClientes.TabIndex = 1;

            // 
            // groupBox2 (Agregar Producto)
            // 
            this.groupBox2.Controls.Add(this.txtEscanear);
            this.groupBox2.Controls.Add(this.labelEscanear);
            this.groupBox2.Controls.Add(this.btnAgregar);
            this.groupBox2.Controls.Add(this.numCantidad);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cboProductos);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(18, 130);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(760, 100);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Agregar Producto";

            // labelEscanear
            this.labelEscanear.AutoSize = true;
            this.labelEscanear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelEscanear.ForeColor = System.Drawing.Color.DarkBlue;
            this.labelEscanear.Location = new System.Drawing.Point(15, 25);
            this.labelEscanear.Text = "ESCANEAR CÓDIGO:";

            // txtEscanear
            this.txtEscanear.BackColor = System.Drawing.Color.LightCyan;
            this.txtEscanear.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.txtEscanear.Location = new System.Drawing.Point(140, 22);
            this.txtEscanear.Name = "txtEscanear";
            this.txtEscanear.Size = new System.Drawing.Size(200, 27);
            this.txtEscanear.TabIndex = 0;
            this.txtEscanear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEscanear_KeyPress);

            // cboProductos
            this.cboProductos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProductos.FormattingEnabled = true;
            this.cboProductos.Location = new System.Drawing.Point(80, 60);
            this.cboProductos.Name = "cboProductos";
            this.cboProductos.Size = new System.Drawing.Size(300, 23);
            this.cboProductos.TabIndex = 2;

            // label2
            this.label2.AutoSize = true; this.label2.Location = new System.Drawing.Point(15, 63); this.label2.Text = "O Manual:";

            // numCantidad
            this.numCantidad.Location = new System.Drawing.Point(460, 60);
            this.numCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numCantidad.Name = "numCantidad";
            this.numCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numCantidad.Size = new System.Drawing.Size(60, 23);
            this.numCantidad.TabIndex = 3;

            // label3
            this.label3.AutoSize = true; this.label3.Location = new System.Drawing.Point(395, 63); this.label3.Text = "Cantidad:";

            // btnAgregar
            this.btnAgregar.BackColor = System.Drawing.Color.SteelBlue;
            this.btnAgregar.ForeColor = System.Drawing.Color.White;
            this.btnAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregar.Location = new System.Drawing.Point(550, 55);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(120, 30);
            this.btnAgregar.TabIndex = 4;
            this.btnAgregar.Text = "Agregar Manual";
            this.btnAgregar.UseVisualStyleBackColor = false;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);

            // dgvCarrito
            this.dgvCarrito.AllowUserToAddRows = false;
            this.dgvCarrito.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCarrito.BackgroundColor = System.Drawing.Color.White;
            this.dgvCarrito.Location = new System.Drawing.Point(18, 250);
            this.dgvCarrito.Name = "dgvCarrito";
            this.dgvCarrito.ReadOnly = true;
            this.dgvCarrito.Size = new System.Drawing.Size(760, 200);
            this.dgvCarrito.TabIndex = 3;

            // lblTotal
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(500, 460);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Text = "Total a Pagar: $0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // btnFinalizar
            this.btnFinalizar.BackColor = System.Drawing.Color.DarkGreen;
            this.btnFinalizar.ForeColor = System.Drawing.Color.White;
            this.btnFinalizar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnFinalizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFinalizar.Location = new System.Drawing.Point(18, 460);
            this.btnFinalizar.Name = "btnFinalizar";
            this.btnFinalizar.Size = new System.Drawing.Size(200, 40);
            this.btnFinalizar.TabIndex = 5;
            this.btnFinalizar.Text = "FINALIZAR VENTA";
            this.btnFinalizar.UseVisualStyleBackColor = false;
            this.btnFinalizar.Click += new System.EventHandler(this.btnFinalizar_Click);

            // VentasForm
            this.ClientSize = new System.Drawing.Size(800, 520);
            this.Controls.Add(this.btnFinalizar);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvCarrito);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblTitulo);
            this.Name = "VentasForm";
            this.Text = "Punto de Venta";
            this.Load += new System.EventHandler(this.VentasForm_Load);
            this.groupBox1.ResumeLayout(false); this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false); this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCarrito)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboClientes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.NumericUpDown numCantidad;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboProductos;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvCarrito;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnFinalizar;
        private System.Windows.Forms.TextBox txtEscanear;
        private System.Windows.Forms.Label labelEscanear;

        // Controles Nuevos
        private System.Windows.Forms.ComboBox cboMetodoPago;
        private System.Windows.Forms.Label label4;
    }
}