namespace AlmacenDesktop.Forms
{
    partial class PagoQRForm
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.picQr = new System.Windows.Forms.PictureBox();
            this.lblMonto = new System.Windows.Forms.Label();
            this.lblEstado = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            this.btnSimularAprobado = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.picSuccess = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picQr)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitulo.Location = new System.Drawing.Point(45, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(262, 25);
            this.lblTitulo.Text = "Cobro Integrado Mercado Pago";
            // 
            // picQr
            // 
            this.picQr.BackColor = System.Drawing.Color.White;
            this.picQr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picQr.Location = new System.Drawing.Point(55, 85);
            this.picQr.Name = "picQr";
            this.picQr.Size = new System.Drawing.Size(240, 240);
            this.picQr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picQr.TabIndex = 1;
            this.picQr.TabStop = false;
            // 
            // lblMonto
            // 
            this.lblMonto.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMonto.ForeColor = System.Drawing.Color.Black;
            this.lblMonto.Location = new System.Drawing.Point(12, 45);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(326, 30);
            this.lblMonto.TabIndex = 2;
            this.lblMonto.Text = "Importe: $ 0.00";
            this.lblMonto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEstado
            // 
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblEstado.ForeColor = System.Drawing.Color.DimGray;
            this.lblEstado.Location = new System.Drawing.Point(12, 332);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(326, 25);
            this.lblEstado.TabIndex = 3;
            this.lblEstado.Text = "Verificando pago asíncrono...";
            this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTimer
            // 
            this.lblTimer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTimer.ForeColor = System.Drawing.Color.DarkGray;
            this.lblTimer.Location = new System.Drawing.Point(12, 357);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(326, 20);
            this.lblTimer.TabIndex = 4;
            this.lblTimer.Text = "El código expira en: 60 seg.";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSimularAprobado
            // 
            this.btnSimularAprobado.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnSimularAprobado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSimularAprobado.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSimularAprobado.ForeColor = System.Drawing.Color.Black;
            this.btnSimularAprobado.Location = new System.Drawing.Point(12, 395);
            this.btnSimularAprobado.Name = "btnSimularAprobado";
            this.btnSimularAprobado.Size = new System.Drawing.Size(155, 33);
            this.btnSimularAprobado.TabIndex = 5;
            this.btnSimularAprobado.Text = "⚡ Simular Aprobado";
            this.btnSimularAprobado.UseVisualStyleBackColor = false;
            this.btnSimularAprobado.Click += new System.EventHandler(this.btnSimularAprobado_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.DarkGray;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(183, 395);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(155, 33);
            this.btnCancelar.TabIndex = 6;
            this.btnCancelar.Text = "❌ Cancelar Cobro";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // picSuccess
            // 
            this.picSuccess.BackColor = System.Drawing.Color.White;
            this.picSuccess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSuccess.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.picSuccess.ForeColor = System.Drawing.Color.ForestGreen;
            this.picSuccess.Location = new System.Drawing.Point(55, 85);
            this.picSuccess.Name = "picSuccess";
            this.picSuccess.Size = new System.Drawing.Size(240, 240);
            this.picSuccess.TabIndex = 7;
            this.picSuccess.Text = "✓\r\nCOBRADO";
            this.picSuccess.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.picSuccess.Visible = false;
            // 
            // PagoQRForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(350, 440);
            this.Controls.Add(this.picSuccess);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnSimularAprobado);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.lblMonto);
            this.Controls.Add(this.picQr);
            this.Controls.Add(this.lblTitulo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PagoQRForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Verificador QR - VENDEMAX";
            this.Load += new System.EventHandler(this.PagoQRForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picQr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.PictureBox picQr;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Button btnSimularAprobado;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label picSuccess;
    }
}
