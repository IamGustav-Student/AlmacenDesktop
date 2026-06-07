namespace AlmacenDesktop.Forms
{
    partial class AfipAsistenteForm
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPaso1 = new System.Windows.Forms.TabPage();
            this.lblPaso1Status = new System.Windows.Forms.Label();
            this.txtLogCSR = new System.Windows.Forms.TextBox();
            this.btnGenerarCSR = new System.Windows.Forms.Button();
            this.txtCuit = new System.Windows.Forms.TextBox();
            this.lblCuit = new System.Windows.Forms.Label();
            this.lblPaso1Desc = new System.Windows.Forms.Label();
            this.tabPaso2 = new System.Windows.Forms.TabPage();
            this.lblPaso2Instrucciones = new System.Windows.Forms.Label();
            this.tabPaso3 = new System.Windows.Forms.TabPage();
            this.lblPaso3Status = new System.Windows.Forms.Label();
            this.btnAsociar = new System.Windows.Forms.Button();
            this.txtCertPassword = new System.Windows.Forms.TextBox();
            this.lblCertPassword = new System.Windows.Forms.Label();
            this.btnBuscarCrt = new System.Windows.Forms.Button();
            this.txtCrtPath = new System.Windows.Forms.TextBox();
            this.lblCrtPath = new System.Windows.Forms.Label();
            this.lblPaso3Desc = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPaso1.SuspendLayout();
            this.tabPaso2.SuspendLayout();
            this.tabPaso3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPaso1);
            this.tabControl.Controls.Add(this.tabPaso2);
            this.tabControl.Controls.Add(this.tabPaso3);
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabControl.Location = new System.Drawing.Point(12, 60);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(480, 470);
            this.tabControl.TabIndex = 0;
            // 
            // tabPaso1
            // 
            this.tabPaso1.Controls.Add(this.lblPaso1Status);
            this.tabPaso1.Controls.Add(this.txtLogCSR);
            this.tabPaso1.Controls.Add(this.btnGenerarCSR);
            this.tabPaso1.Controls.Add(this.txtCuit);
            this.tabPaso1.Controls.Add(this.lblCuit);
            this.tabPaso1.Controls.Add(this.lblPaso1Desc);
            this.tabPaso1.Location = new System.Drawing.Point(4, 25);
            this.tabPaso1.Name = "tabPaso1";
            this.tabPaso1.Padding = new System.Windows.Forms.Padding(10);
            this.tabPaso1.Size = new System.Drawing.Size(472, 441);
            this.tabPaso1.TabIndex = 0;
            this.tabPaso1.Text = "1. Generar CSR";
            this.tabPaso1.UseVisualStyleBackColor = true;
            // 
            // lblPaso1Status
            // 
            this.lblPaso1Status.AutoSize = true;
            this.lblPaso1Status.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPaso1Status.Location = new System.Drawing.Point(15, 178);
            this.lblPaso1Status.Name = "lblPaso1Status";
            this.lblPaso1Status.Size = new System.Drawing.Size(45, 17);
            this.lblPaso1Status.TabIndex = 5;
            this.lblPaso1Status.Text = "Status";
            // 
            // txtLogCSR
            // 
            this.txtLogCSR.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtLogCSR.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLogCSR.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtLogCSR.Location = new System.Drawing.Point(15, 205);
            this.txtLogCSR.Multiline = true;
            this.txtLogCSR.Name = "txtLogCSR";
            this.txtLogCSR.ReadOnly = true;
            this.txtLogCSR.Size = new System.Drawing.Size(442, 220);
            this.txtLogCSR.TabIndex = 6;
            // 
            // btnGenerarCSR
            // 
            this.btnGenerarCSR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnGenerarCSR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerarCSR.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnGenerarCSR.ForeColor = System.Drawing.Color.White;
            this.btnGenerarCSR.Location = new System.Drawing.Point(15, 130);
            this.btnGenerarCSR.Name = "btnGenerarCSR";
            this.btnGenerarCSR.Size = new System.Drawing.Size(442, 38);
            this.btnGenerarCSR.TabIndex = 4;
            this.btnGenerarCSR.Text = "Generar Clave Privada y Pedido CSR";
            this.btnGenerarCSR.UseVisualStyleBackColor = false;
            this.btnGenerarCSR.Click += new System.EventHandler(this.btnGenerarCSR_Click);
            // 
            // txtCuit
            // 
            this.txtCuit.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtCuit.Location = new System.Drawing.Point(15, 95);
            this.txtCuit.MaxLength = 11;
            this.txtCuit.Name = "txtCuit";
            this.txtCuit.Size = new System.Drawing.Size(220, 27);
            this.txtCuit.TabIndex = 3;
            // 
            // lblCuit
            // 
            this.lblCuit.AutoSize = true;
            this.lblCuit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCuit.Location = new System.Drawing.Point(15, 73);
            this.lblCuit.Name = "lblCuit";
            this.lblCuit.Size = new System.Drawing.Size(161, 19);
            this.lblCuit.TabIndex = 2;
            this.lblCuit.Text = "CUIT Emisor (11 dígitos):";
            // 
            // lblPaso1Desc
            // 
            this.lblPaso1Desc.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPaso1Desc.Location = new System.Drawing.Point(15, 15);
            this.lblPaso1Desc.Name = "lblPaso1Desc";
            this.lblPaso1Desc.Size = new System.Drawing.Size(442, 53);
            this.lblPaso1Desc.TabIndex = 1;
            this.lblPaso1Desc.Text = "Este paso genera localmente tu clave privada y un archivo de requerimiento (.csr)" +
    " para que la AFIP firme. \r\nTodo el proceso es 100% nativo y seguro.";
            // 
            // tabPaso2
            // 
            this.tabPaso2.Controls.Add(this.lblPaso2Instrucciones);
            this.tabPaso2.Location = new System.Drawing.Point(4, 25);
            this.tabPaso2.Name = "tabPaso2";
            this.tabPaso2.Padding = new System.Windows.Forms.Padding(15);
            this.tabPaso2.Size = new System.Drawing.Size(472, 441);
            this.tabPaso2.TabIndex = 1;
            this.tabPaso2.Text = "2. Trámite AFIP";
            this.tabPaso2.UseVisualStyleBackColor = true;
            // 
            // lblPaso2Instrucciones
            // 
            this.lblPaso2Instrucciones.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPaso2Instrucciones.Location = new System.Drawing.Point(15, 15);
            this.lblPaso2Instrucciones.Name = "lblPaso2Instrucciones";
            this.lblPaso2Instrucciones.Size = new System.Drawing.Size(442, 400);
            this.lblPaso2Instrucciones.TabIndex = 0;
            this.lblPaso2Instrucciones.Text = "Para subir tu pedido (.csr) y descargar el certificado oficial de la AFIP:\r\n\r\n" +
    "1. Ingresa a la web de AFIP con Clave Fiscal.\r\n\r\n" +
    "2. Busca el servicio \"Administración de Certificados Digitales\".\r\n\r\n" +
    "3. Agrega un nuevo alias para tu computadora (ej: \"Caja1_VENDEMAX\") y sube el archivo \".csr\" que acabas de guardar en el Paso 1.\r\n\r\n" +
    "4. Presiona Generar y descarga el archivo de certificado oficial (\".crt\").\r\n\r\n" +
    "5. Una vez que tengas el archivo descargado en tu equipo, continúa en este programa con la pestaña \"3. Cargar Certificado\".";
            // 
            // tabPaso3
            // 
            this.tabPaso3.Controls.Add(this.lblPaso3Status);
            this.tabPaso3.Controls.Add(this.btnAsociar);
            this.tabPaso3.Controls.Add(this.txtCertPassword);
            this.tabPaso3.Controls.Add(this.lblCertPassword);
            this.tabPaso3.Controls.Add(this.btnBuscarCrt);
            this.tabPaso3.Controls.Add(this.txtCrtPath);
            this.tabPaso3.Controls.Add(this.lblCrtPath);
            this.tabPaso3.Controls.Add(this.lblPaso3Desc);
            this.tabPaso3.Location = new System.Drawing.Point(4, 25);
            this.tabPaso3.Name = "tabPaso3";
            this.tabPaso3.Padding = new System.Windows.Forms.Padding(10);
            this.tabPaso3.Size = new System.Drawing.Size(472, 441);
            this.tabPaso3.TabIndex = 2;
            this.tabPaso3.Text = "3. Cargar Certificado";
            this.tabPaso3.UseVisualStyleBackColor = true;
            // 
            // lblPaso3Status
            // 
            this.lblPaso3Status.AutoSize = true;
            this.lblPaso3Status.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPaso3Status.Location = new System.Drawing.Point(15, 275);
            this.lblPaso3Status.Name = "lblPaso3Status";
            this.lblPaso3Status.Size = new System.Drawing.Size(45, 17);
            this.lblPaso3Status.TabIndex = 8;
            this.lblPaso3Status.Text = "Status";
            // 
            // btnAsociar
            // 
            this.btnAsociar.BackColor = System.Drawing.Color.ForestGreen;
            this.btnAsociar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAsociar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAsociar.ForeColor = System.Drawing.Color.White;
            this.btnAsociar.Location = new System.Drawing.Point(15, 220);
            this.btnAsociar.Name = "btnAsociar";
            this.btnAsociar.Size = new System.Drawing.Size(442, 40);
            this.btnAsociar.TabIndex = 7;
            this.btnAsociar.Text = "Integrar, Encriptar y Activar Facturación";
            this.btnAsociar.UseVisualStyleBackColor = false;
            this.btnAsociar.Click += new System.EventHandler(this.btnAsociar_Click);
            // 
            // txtCertPassword
            // 
            this.txtCertPassword.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtCertPassword.Location = new System.Drawing.Point(15, 175);
            this.txtCertPassword.Name = "txtCertPassword";
            this.txtCertPassword.PasswordChar = '*';
            this.txtCertPassword.Size = new System.Drawing.Size(220, 25);
            this.txtCertPassword.TabIndex = 6;
            // 
            // lblCertPassword
            // 
            this.lblCertPassword.AutoSize = true;
            this.lblCertPassword.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCertPassword.Location = new System.Drawing.Point(15, 153);
            this.lblCertPassword.Name = "lblCertPassword";
            this.lblCertPassword.Size = new System.Drawing.Size(248, 17);
            this.lblCertPassword.TabIndex = 5;
            this.lblCertPassword.Text = "Crear contraseña para proteger el P12:";
            // 
            // btnBuscarCrt
            // 
            this.btnBuscarCrt.BackColor = System.Drawing.Color.DimGray;
            this.btnBuscarCrt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarCrt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnBuscarCrt.ForeColor = System.Drawing.Color.White;
            this.btnBuscarCrt.Location = new System.Drawing.Point(377, 105);
            this.btnBuscarCrt.Name = "btnBuscarCrt";
            this.btnBuscarCrt.Size = new System.Drawing.Size(80, 27);
            this.btnBuscarCrt.TabIndex = 4;
            this.btnBuscarCrt.Text = "Buscar...";
            this.btnBuscarCrt.UseVisualStyleBackColor = false;
            this.btnBuscarCrt.Click += new System.EventHandler(this.btnBuscarCrt_Click);
            // 
            // txtCrtPath
            // 
            this.txtCrtPath.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtCrtPath.Location = new System.Drawing.Point(15, 106);
            this.txtCrtPath.Name = "txtCrtPath";
            this.txtCrtPath.Size = new System.Drawing.Size(356, 25);
            this.txtCrtPath.TabIndex = 3;
            // 
            // lblCrtPath
            // 
            this.lblCrtPath.AutoSize = true;
            this.lblCrtPath.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCrtPath.Location = new System.Drawing.Point(15, 84);
            this.lblCrtPath.Name = "lblCrtPath";
            this.lblCrtPath.Size = new System.Drawing.Size(262, 17);
            this.lblCrtPath.TabIndex = 2;
            this.lblCrtPath.Text = "Seleccionar Certificado Oficial de AFIP (.crt):";
            // 
            // lblPaso3Desc
            // 
            this.lblPaso3Desc.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPaso3Desc.Location = new System.Drawing.Point(15, 15);
            this.lblPaso3Desc.Name = "lblPaso3Desc";
            this.lblPaso3Desc.Size = new System.Drawing.Size(442, 60);
            this.lblPaso3Desc.TabIndex = 1;
            this.lblPaso3Desc.Text = "Seleccione el archivo .crt descargado. El sistema lo combinará de forma automátic" +
    "a con su clave privada interna, generará el contenedor .p12 y encriptará los dat" +
    "os mediante Windows DPAPI.";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblTitulo.Location = new System.Drawing.Point(12, 18);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(395, 30);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Asistente Fiscal de Facturación AFIP (1-Click)";
            // 
            // AfipAsistenteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(504, 542);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AfipAsistenteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configurador Fiscal - VENDEMAX";
            this.Load += new System.EventHandler(this.AfipAsistenteForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPaso1.ResumeLayout(false);
            this.tabPaso1.PerformLayout();
            this.tabPaso2.ResumeLayout(false);
            this.tabPaso3.ResumeLayout(false);
            this.tabPaso3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPaso1;
        private System.Windows.Forms.Label lblPaso1Desc;
        private System.Windows.Forms.TextBox txtCuit;
        private System.Windows.Forms.Label lblCuit;
        private System.Windows.Forms.Button btnGenerarCSR;
        private System.Windows.Forms.TextBox txtLogCSR;
        private System.Windows.Forms.Label lblPaso1Status;
        private System.Windows.Forms.TabPage tabPaso2;
        private System.Windows.Forms.Label lblPaso2Instrucciones;
        private System.Windows.Forms.TabPage tabPaso3;
        private System.Windows.Forms.Label lblPaso3Desc;
        private System.Windows.Forms.Button btnBuscarCrt;
        private System.Windows.Forms.TextBox txtCrtPath;
        private System.Windows.Forms.Label lblCrtPath;
        private System.Windows.Forms.TextBox txtCertPassword;
        private System.Windows.Forms.Label lblCertPassword;
        private System.Windows.Forms.Button btnAsociar;
        private System.Windows.Forms.Label lblPaso3Status;
        private System.Windows.Forms.Label lblTitulo;
    }
}
