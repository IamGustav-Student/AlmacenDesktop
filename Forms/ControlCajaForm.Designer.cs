namespace AlmacenDesktop.Forms
{
    partial class ControlCajaForm
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
            this.lblEstado = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblMonto = new System.Windows.Forms.Label();
            this.numMonto = new System.Windows.Forms.NumericUpDown();
            this.btnAccion = new System.Windows.Forms.Button();
            this.grpResumen = new System.Windows.Forms.GroupBox();
            this.dgvVentasCaja = new System.Windows.Forms.DataGridView();
            this.lblResumenDetalle = new System.Windows.Forms.Label();
            this.btnRegistrarMovimiento = new System.Windows.Forms.Button(); // INCLUIDO NATIVAMENTE
            ((System.ComponentModel.ISupportInitialize)(this.numMonto)).BeginInit();
            this.grpResumen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentasCaja)).BeginInit();
            this.SuspendLayout();

            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEstado.Location = new System.Drawing.Point(20, 20);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(250, 25);
            this.lblEstado.TabIndex = 0;
            this.lblEstado.Text = "ESTADO: CAJA CERRADA";

            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(20, 60);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(200, 15);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "Ingrese el saldo inicial para comenzar:";

            // 
            // lblMonto
            // 
            this.lblMonto.AutoSize = true;
            this.lblMonto.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblMonto.Location = new System.Drawing.Point(20, 95);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(75, 19);
            this.lblMonto.TabIndex = 2;
            this.lblMonto.Text = "Monto ($):";

            // 
            // numMonto
            // 
            this.numMonto.DecimalPlaces = 2;
            this.numMonto.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.numMonto.Location = new System.Drawing.Point(100, 90);
            this.numMonto.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            this.numMonto.Name = "numMonto";
            this.numMonto.Size = new System.Drawing.Size(180, 29);
            this.numMonto.TabIndex = 3;
            this.numMonto.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // 
            // btnAccion
            // 
            this.btnAccion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccion.BackColor = System.Drawing.Color.ForestGreen;
            this.btnAccion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAccion.ForeColor = System.Drawing.Color.White;
            this.btnAccion.Location = new System.Drawing.Point(20, 140);
            this.btnAccion.Name = "btnAccion";
            this.btnAccion.Size = new System.Drawing.Size(460, 50);
            this.btnAccion.TabIndex = 4;
            this.btnAccion.Text = "ABRIR CAJA";
            this.btnAccion.UseVisualStyleBackColor = false;
            this.btnAccion.Click += new System.EventHandler(this.btnAccion_Click);

            // 
            // grpResumen
            // 
            // ANCLAJE TOTAL: Se estira hacia abajo si agrandan la ventana, pero deja espacio al botón rojo
            this.grpResumen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpResumen.Controls.Add(this.dgvVentasCaja);
            this.grpResumen.Controls.Add(this.lblResumenDetalle);
            this.grpResumen.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.grpResumen.Location = new System.Drawing.Point(20, 210);
            this.grpResumen.Name = "grpResumen";
            this.grpResumen.Size = new System.Drawing.Size(460, 310);
            this.grpResumen.TabIndex = 5;
            this.grpResumen.TabStop = false;
            this.grpResumen.Text = "Resumen del Turno (Sistema)";
            this.grpResumen.Visible = false;

            // 
            // lblResumenDetalle
            // 
            this.lblResumenDetalle.AutoSize = true;
            this.lblResumenDetalle.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblResumenDetalle.Location = new System.Drawing.Point(15, 25);
            this.lblResumenDetalle.Name = "lblResumenDetalle";
            this.lblResumenDetalle.Size = new System.Drawing.Size(16, 18);
            this.lblResumenDetalle.TabIndex = 0;
            this.lblResumenDetalle.Text = "-";

            // 
            // dgvVentasCaja
            // 
            this.dgvVentasCaja.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvVentasCaja.AllowUserToAddRows = false;
            this.dgvVentasCaja.AllowUserToDeleteRows = false;
            this.dgvVentasCaja.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVentasCaja.BackgroundColor = System.Drawing.Color.White;
            this.dgvVentasCaja.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVentasCaja.Location = new System.Drawing.Point(15, 130);
            this.dgvVentasCaja.Name = "dgvVentasCaja";
            this.dgvVentasCaja.ReadOnly = true;
            this.dgvVentasCaja.RowHeadersVisible = false;
            this.dgvVentasCaja.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVentasCaja.Size = new System.Drawing.Size(430, 165);
            this.dgvVentasCaja.TabIndex = 1;

            // 
            // btnRegistrarMovimiento
            // 
            // ANCLAJE INFERIOR: Siempre pegado abajo, no importa el tamaño de pantalla
            this.btnRegistrarMovimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRegistrarMovimiento.BackColor = System.Drawing.Color.Firebrick;
            this.btnRegistrarMovimiento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistrarMovimiento.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnRegistrarMovimiento.ForeColor = System.Drawing.Color.White;
            this.btnRegistrarMovimiento.Location = new System.Drawing.Point(20, 535); // Posición Y calculada para no chocar
            this.btnRegistrarMovimiento.Name = "btnRegistrarMovimiento";
            this.btnRegistrarMovimiento.Size = new System.Drawing.Size(460, 45);
            this.btnRegistrarMovimiento.TabIndex = 6;
            this.btnRegistrarMovimiento.Text = "➖ REGISTRAR GASTO / RETIRO";
            this.btnRegistrarMovimiento.UseVisualStyleBackColor = false;
            this.btnRegistrarMovimiento.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegistrarMovimiento.Visible = false; // Se oculta si la caja está cerrada
            this.btnRegistrarMovimiento.Click += new System.EventHandler(this.BtnRegistrarMovimiento_Click);

            // 
            // ControlCajaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(500, 600); // Altura aumentada para que todo respire
            this.Controls.Add(this.btnRegistrarMovimiento);
            this.Controls.Add(this.grpResumen);
            this.Controls.Add(this.btnAccion);
            this.Controls.Add(this.numMonto);
            this.Controls.Add(this.lblMonto);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblEstado);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            // this.MaximizeBox = false; // Permitimos maximizar para probar el responsive
            this.Name = "ControlCajaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Control de Caja";
            ((System.ComponentModel.ISupportInitialize)(this.numMonto)).EndInit();
            this.grpResumen.ResumeLayout(false);
            this.grpResumen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVentasCaja)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.NumericUpDown numMonto;
        private System.Windows.Forms.Button btnAccion;
        private System.Windows.Forms.GroupBox grpResumen;
        private System.Windows.Forms.Label lblResumenDetalle;
        private System.Windows.Forms.DataGridView dgvVentasCaja;
        // Ahora declarado aquí formalmente
        private System.Windows.Forms.Button btnRegistrarMovimiento;
    }
}