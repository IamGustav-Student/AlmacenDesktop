namespace AlmacenDesktop.Forms
{
    partial class ConfiguracionAfipForm
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
            this.btnBuscarCertificado = new System.Windows.Forms.Button();
            this.txtCertificadoPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkProduccion = new System.Windows.Forms.CheckBox();
            this.numPuntoVenta = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCuit = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnProbar = new System.Windows.Forms.Button();
            this.lblEstado = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPuntoVenta)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.Location = new System.Drawing.Point(20, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(211, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Configuración AFIP";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBuscarCertificado);
            this.groupBox1.Controls.Add(this.txtCertificadoPath);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.chkProduccion);
            this.groupBox1.Controls.Add(this.numPuntoVenta);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtCuit);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.groupBox1.Location = new System.Drawing.Point(25, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(530, 280);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Credenciales de Facturación";
            // 
            // btnBuscarCertificado
            // 
            this.btnBuscarCertificado.Location = new System.Drawing.Point(440, 158);
            this.btnBuscarCertificado.Name = "btnBuscarCertificado";
            this.btnBuscarCertificado.Size = new System.Drawing.Size(75, 29);
            this.btnBuscarCertificado.TabIndex = 9;
            this.btnBuscarCertificado.Text = "...";
            this.btnBuscarCertificado.UseVisualStyleBackColor = true;
            this.btnBuscarCertificado.Click += new System.EventHandler(this.btnBuscarCertificado_Click);
            // 
            // txtCertificadoPath
            // 
            this.txtCertificadoPath.Location = new System.Drawing.Point(20, 160);
            this.txtCertificadoPath.Name = "txtCertificadoPath";
            this.txtCertificadoPath.ReadOnly = true;
            this.txtCertificadoPath.Size = new System.Drawing.Size(414, 25);
            this.txtCertificadoPath.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "Certificado Digital (.p12)";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(20, 225);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(200, 25);
            this.txtPassword.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 203);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 19);
            this.label3.TabIndex = 5;
            this.label3.Text = "Contraseña del Certificado";
            // 
            // chkProduccion
            // 
            this.chkProduccion.AutoSize = true;
            this.chkProduccion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.chkProduccion.ForeColor = System.Drawing.Color.Red;
            this.chkProduccion.Location = new System.Drawing.Point(300, 227);
            this.chkProduccion.Name = "chkProduccion";
            this.chkProduccion.Size = new System.Drawing.Size(193, 23);
            this.chkProduccion.TabIndex = 4;
            this.chkProduccion.Text = "MODO PRODUCCIÓN (Real)";
            this.chkProduccion.UseVisualStyleBackColor = true;
            // 
            // numPuntoVenta
            // 
            this.numPuntoVenta.Location = new System.Drawing.Point(300, 60);
            this.numPuntoVenta.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numPuntoVenta.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPuntoVenta.Name = "numPuntoVenta";
            this.numPuntoVenta.Size = new System.Drawing.Size(120, 25);
            this.numPuntoVenta.TabIndex = 3;
            this.numPuntoVenta.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(300, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "Punto de Venta";
            // 
            // txtCuit
            // 
            this.txtCuit.Location = new System.Drawing.Point(20, 60);
            this.txtCuit.Name = "txtCuit";
            this.txtCuit.Size = new System.Drawing.Size(200, 25);
            this.txtCuit.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "CUIT del Emisor";
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.SeaGreen;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(375, 370);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(180, 45);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.Text = "Guardar Configuración";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnProbar
            // 
            this.btnProbar.BackColor = System.Drawing.Color.SteelBlue;
            this.btnProbar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProbar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnProbar.ForeColor = System.Drawing.Color.White;
            this.btnProbar.Location = new System.Drawing.Point(25, 370);
            this.btnProbar.Name = "btnProbar";
            this.btnProbar.Size = new System.Drawing.Size(150, 45);
            this.btnProbar.TabIndex = 3;
            this.btnProbar.Text = "Probar Conexión";
            this.btnProbar.UseVisualStyleBackColor = false;
            this.btnProbar.Click += new System.EventHandler(this.btnProbar_Click);
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblEstado.Location = new System.Drawing.Point(190, 385);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(0, 15);
            this.lblEstado.TabIndex = 4;
            // 
            // ConfiguracionAfipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 441);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.btnProbar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ConfiguracionAfipForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración AFIP";
            this.Load += new System.EventHandler(this.ConfiguracionAfipForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPuntoVenta)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCuit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numPuntoVenta;
        private System.Windows.Forms.CheckBox chkProduccion;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBuscarCertificado;
        private System.Windows.Forms.TextBox txtCertificadoPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnProbar;
        private System.Windows.Forms.Label lblEstado;
    }
}