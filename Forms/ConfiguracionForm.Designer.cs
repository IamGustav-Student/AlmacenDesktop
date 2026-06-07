namespace AlmacenDesktop.Forms
{
    partial class ConfiguracionForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtNombreNegocio = new System.Windows.Forms.TextBox();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCuit = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMensajeTicket = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnConfigurarAfip = new System.Windows.Forms.Button();

            // CAMBIO: Ahora es un ComboBox
            this.cboImpresoras = new System.Windows.Forms.ComboBox();

            this.SuspendLayout();

            // label1 (Nombre)
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 15);
            this.label1.Text = "Nombre del Negocio";

            // txtNombreNegocio
            this.txtNombreNegocio.Location = new System.Drawing.Point(30, 48);
            this.txtNombreNegocio.Name = "txtNombreNegocio";
            this.txtNombreNegocio.Size = new System.Drawing.Size(250, 23);

            // label2 (Dirección)
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 15);
            this.label2.Text = "Dirección";

            // txtDireccion
            this.txtDireccion.Location = new System.Drawing.Point(30, 103);
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(250, 23);

            // label3 (Teléfono)
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.Text = "Teléfono";

            // txtTelefono
            this.txtTelefono.Location = new System.Drawing.Point(30, 158);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(250, 23);

            // label4 (CUIT)
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(310, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 15);
            this.label4.Text = "CUIT";

            // txtCuit
            this.txtCuit.Location = new System.Drawing.Point(310, 48);
            this.txtCuit.Name = "txtCuit";
            this.txtCuit.Size = new System.Drawing.Size(250, 23);

            // label5 (Email)
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(310, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 15);
            this.label5.Text = "Email";

            // txtEmail
            this.txtEmail.Location = new System.Drawing.Point(310, 103);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(250, 23);

            // label6 (Impresora)
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(310, 140);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 15);
            this.label6.Text = "Impresora de Tickets";

            // cboImpresoras (Nuevo)
            this.cboImpresoras.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboImpresoras.FormattingEnabled = true;
            this.cboImpresoras.Location = new System.Drawing.Point(310, 158);
            this.cboImpresoras.Name = "cboImpresoras";
            this.cboImpresoras.Size = new System.Drawing.Size(250, 23);

            // label7 (Mensaje Ticket)
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(30, 195);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(149, 15);
            this.label7.Text = "Mensaje al pie del Ticket";

            // txtMensajeTicket
            this.txtMensajeTicket.Location = new System.Drawing.Point(30, 213);
            this.txtMensajeTicket.Name = "txtMensajeTicket";
            this.txtMensajeTicket.Size = new System.Drawing.Size(530, 23);

            // btnGuardar
            this.btnGuardar.BackColor = System.Drawing.Color.ForestGreen;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(410, 260);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(150, 40);
            this.btnGuardar.Text = "GUARDAR DATOS";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            // btnBackup
            this.btnBackup.BackColor = System.Drawing.Color.SteelBlue;
            this.btnBackup.ForeColor = System.Drawing.Color.White;
            this.btnBackup.Location = new System.Drawing.Point(30, 260);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(150, 40);
            this.btnBackup.Text = "GENERAR BACKUP";
            this.btnBackup.UseVisualStyleBackColor = false;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);

            // btnConfigurarAfip
            this.btnConfigurarAfip.BackColor = System.Drawing.Color.Orange;
            this.btnConfigurarAfip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfigurarAfip.ForeColor = System.Drawing.Color.Black;
            this.btnConfigurarAfip.Location = new System.Drawing.Point(220, 260);
            this.btnConfigurarAfip.Name = "btnConfigurarAfip";
            this.btnConfigurarAfip.Size = new System.Drawing.Size(150, 40);
            this.btnConfigurarAfip.Text = "CONFIGURAR AFIP";
            this.btnConfigurarAfip.UseVisualStyleBackColor = false;
            this.btnConfigurarAfip.Click += new System.EventHandler(this.btnConfigurarAfip_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 330);
            this.Controls.Add(this.btnConfigurarAfip);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.txtMensajeTicket);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cboImpresoras); // Agregado
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtCuit);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDireccion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNombreNegocio);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ConfiguracionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración del Sistema";
            this.Load += new System.EventHandler(this.ConfiguracionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNombreNegocio;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCuit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMensajeTicket;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnConfigurarAfip;
        private System.Windows.Forms.ComboBox cboImpresoras; // Variable nueva
    }
}