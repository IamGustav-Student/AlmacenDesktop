namespace AlmacenDesktop.Forms
{
    partial class LockForm
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnRevalidar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.lnkPagar = new System.Windows.Forms.LinkLabel();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();

            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(20)))), ((int)(((byte)(20))))); // Fondo rojo muy suave oscuro para advertencia
            this.panelHeader.Controls.Add(this.lblTitulo);
            this.panelHeader.Controls.Add(this.lblSubtitulo);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(420, 80);
            this.panelHeader.TabIndex = 0;

            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Space Grotesk", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68))))); // Rojo brillante de alerta
            this.lblTitulo.Location = new System.Drawing.Point(12, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(220, 32);
            this.lblTitulo.Text = "HEXASTRATEGY";

            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(113)))), ((int)(((byte)(113)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(14, 45);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(217, 17);
            this.lblSubtitulo.Text = "ACCESO SUSPENDIDO AL SISTEMA";

            // 
            // lblMensaje
            // 
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMensaje.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(237)))), ((int)(((byte)(245)))));
            this.lblMensaje.Location = new System.Drawing.Point(30, 110);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(360, 120);
            this.lblMensaje.TabIndex = 1;
            this.lblMensaje.Text = "Su suscripción mensual a VENDEMAX se encuentra suspendida por falta de pago o por expirar el plazo offline. Por favor, regularice su suscripción para continuar operando.";
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // btnRevalidar
            // 
            this.btnRevalidar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(245)))), ((int)(((byte)(196)))));
            this.btnRevalidar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRevalidar.FlatAppearance.BorderSize = 0;
            this.btnRevalidar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRevalidar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnRevalidar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(11)))), ((int)(((byte)(18)))));
            this.btnRevalidar.Location = new System.Drawing.Point(30, 260);
            this.btnRevalidar.Name = "btnRevalidar";
            this.btnRevalidar.Size = new System.Drawing.Size(220, 45);
            this.btnRevalidar.TabIndex = 2;
            this.btnRevalidar.Text = "RECOMPROBAR ESTADO";
            this.btnRevalidar.UseVisualStyleBackColor = false;
            this.btnRevalidar.Click += new System.EventHandler(this.btnRevalidar_Click);

            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.Transparent;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(29)))), ((int)(((byte)(46)))));
            this.btnSalir.FlatAppearance.BorderSize = 1;
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSalir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btnSalir.Location = new System.Drawing.Point(265, 260);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(125, 45);
            this.btnSalir.TabIndex = 3;
            this.btnSalir.Text = "SALIR";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);

            // 
            // lnkPagar
            // 
            this.lnkPagar.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(245)))), ((int)(((byte)(196)))));
            this.lnkPagar.AutoSize = true;
            this.lnkPagar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lnkPagar.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkPagar.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(245)))), ((int)(((byte)(196)))));
            this.lnkPagar.Location = new System.Drawing.Point(30, 325);
            this.lnkPagar.Name = "lnkPagar";
            this.lnkPagar.Size = new System.Drawing.Size(262, 17);
            this.lnkPagar.TabStop = true;
            this.lnkPagar.Text = "No pagué todavía (Ir a Regularizar Pago)";
            this.lnkPagar.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPagar_LinkClicked);

            // 
            // LockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(11)))), ((int)(((byte)(18)))));
            this.ClientSize = new System.Drawing.Size(420, 370);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnRevalidar);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lnkPagar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LockForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VENDEMAX Suspendido";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnRevalidar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.LinkLabel lnkPagar;
        private System.Windows.Forms.Panel panelHeader;
    }
}
