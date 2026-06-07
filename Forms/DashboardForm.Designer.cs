namespace AlmacenDesktop.Forms
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle cellStyleHeader = new System.Windows.Forms.DataGridViewCellStyle();

            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelVentasHoy = new System.Windows.Forms.Panel();
            this.lblVentasHoyMonto = new System.Windows.Forms.Label();
            this.lblVentasHoyTitulo = new System.Windows.Forms.Label();
            this.panelMes = new System.Windows.Forms.Panel();
            this.lblVentasMesMonto = new System.Windows.Forms.Label();
            this.lblVentasMesTitulo = new System.Windows.Forms.Label();
            this.panelGanancia = new System.Windows.Forms.Panel();
            this.lblGananciaMonto = new System.Windows.Forms.Label();
            this.lblGananciaTitulo = new System.Windows.Forms.Label();
            this.panelAlerta = new System.Windows.Forms.Panel();
            this.lblAlertaCantidad = new System.Windows.Forms.Label();
            this.lblAlertaTitulo = new System.Windows.Forms.Label();

            this.dgvTopProductos = new System.Windows.Forms.DataGridView();
            this.lblTopProductos = new System.Windows.Forms.Label();
            this.dgvBajoStock = new System.Windows.Forms.DataGridView();
            this.lblBajoStock = new System.Windows.Forms.Label();
            this.btnActualizar = new System.Windows.Forms.Button();

            this.panelVentasHoy.SuspendLayout();
            this.panelMes.SuspendLayout();
            this.panelGanancia.SuspendLayout();
            this.panelAlerta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopProductos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBajoStock)).BeginInit();
            this.SuspendLayout();

            // Estilos Comunes
            this.BackColor = System.Drawing.Color.WhiteSmoke;

            // Título Principal
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTitulo.Location = new System.Drawing.Point(20, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(350, 37);
            this.lblTitulo.Text = "Tablero de Control (KPIs)";

            // --- TARJETA 1: VENTAS HOY ---
            this.panelVentasHoy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.panelVentasHoy.Controls.Add(this.lblVentasHoyMonto);
            this.panelVentasHoy.Controls.Add(this.lblVentasHoyTitulo);
            this.panelVentasHoy.Location = new System.Drawing.Point(20, 80);
            this.panelVentasHoy.Name = "panelVentasHoy";
            this.panelVentasHoy.Size = new System.Drawing.Size(220, 120);
            this.panelVentasHoy.TabIndex = 1;

            this.lblVentasHoyTitulo.AutoSize = true;
            this.lblVentasHoyTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblVentasHoyTitulo.ForeColor = System.Drawing.Color.White;
            this.lblVentasHoyTitulo.Location = new System.Drawing.Point(15, 15);
            this.lblVentasHoyTitulo.Name = "lblVentasHoyTitulo";
            this.lblVentasHoyTitulo.Text = "Ventas de Hoy";

            this.lblVentasHoyMonto.AutoSize = true;
            this.lblVentasHoyMonto.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblVentasHoyMonto.ForeColor = System.Drawing.Color.White;
            this.lblVentasHoyMonto.Location = new System.Drawing.Point(10, 50);
            this.lblVentasHoyMonto.Name = "lblVentasHoyMonto";
            this.lblVentasHoyMonto.Text = "$ 0.00";

            // --- TARJETA 2: VENTAS MES ---
            this.panelMes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.panelMes.Controls.Add(this.lblVentasMesMonto);
            this.panelMes.Controls.Add(this.lblVentasMesTitulo);
            this.panelMes.Location = new System.Drawing.Point(260, 80);
            this.panelMes.Name = "panelMes";
            this.panelMes.Size = new System.Drawing.Size(220, 120);
            this.panelMes.TabIndex = 2;

            this.lblVentasMesTitulo.AutoSize = true;
            this.lblVentasMesTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblVentasMesTitulo.ForeColor = System.Drawing.Color.White;
            this.lblVentasMesTitulo.Location = new System.Drawing.Point(15, 15);
            this.lblVentasMesTitulo.Name = "lblVentasMesTitulo";
            this.lblVentasMesTitulo.Text = "Acumulado Mes";

            this.lblVentasMesMonto.AutoSize = true;
            this.lblVentasMesMonto.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblVentasMesMonto.ForeColor = System.Drawing.Color.White;
            this.lblVentasMesMonto.Location = new System.Drawing.Point(10, 50);
            this.lblVentasMesMonto.Name = "lblVentasMesMonto";
            this.lblVentasMesMonto.Text = "$ 0.00";

            // --- TARJETA 3: GANANCIA ESTIMADA ---
            this.panelGanancia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(125)))), ((int)(((byte)(50)))));
            this.panelGanancia.Controls.Add(this.lblGananciaMonto);
            this.panelGanancia.Controls.Add(this.lblGananciaTitulo);
            this.panelGanancia.Location = new System.Drawing.Point(500, 80);
            this.panelGanancia.Name = "panelGanancia";
            this.panelGanancia.Size = new System.Drawing.Size(220, 120);
            this.panelGanancia.TabIndex = 3;

            this.lblGananciaTitulo.AutoSize = true;
            this.lblGananciaTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblGananciaTitulo.ForeColor = System.Drawing.Color.White;
            this.lblGananciaTitulo.Location = new System.Drawing.Point(15, 15);
            this.lblGananciaTitulo.Name = "lblGananciaTitulo";
            this.lblGananciaTitulo.Text = "Ganancia (Mes)";

            this.lblGananciaMonto.AutoSize = true;
            this.lblGananciaMonto.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblGananciaMonto.ForeColor = System.Drawing.Color.White;
            this.lblGananciaMonto.Location = new System.Drawing.Point(10, 50);
            this.lblGananciaMonto.Name = "lblGananciaMonto";
            this.lblGananciaMonto.Text = "$ 0.00";

            // --- TARJETA 4: ALERTAS STOCK ---
            this.panelAlerta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(198)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.panelAlerta.Controls.Add(this.lblAlertaCantidad);
            this.panelAlerta.Controls.Add(this.lblAlertaTitulo);
            this.panelAlerta.Location = new System.Drawing.Point(740, 80);
            this.panelAlerta.Name = "panelAlerta";
            this.panelAlerta.Size = new System.Drawing.Size(220, 120);
            this.panelAlerta.TabIndex = 4;

            this.lblAlertaTitulo.AutoSize = true;
            this.lblAlertaTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblAlertaTitulo.ForeColor = System.Drawing.Color.White;
            this.lblAlertaTitulo.Location = new System.Drawing.Point(15, 15);
            this.lblAlertaTitulo.Name = "lblAlertaTitulo";
            this.lblAlertaTitulo.Text = "Alertas de Stock";

            this.lblAlertaCantidad.AutoSize = true;
            this.lblAlertaCantidad.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAlertaCantidad.ForeColor = System.Drawing.Color.White;
            this.lblAlertaCantidad.Location = new System.Drawing.Point(10, 50);
            this.lblAlertaCantidad.Name = "lblAlertaCantidad";
            this.lblAlertaCantidad.Text = "0";

            // --- GRID: TOP PRODUCTOS ---
            this.lblTopProductos.AutoSize = true;
            this.lblTopProductos.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTopProductos.Location = new System.Drawing.Point(20, 230);
            this.lblTopProductos.Name = "lblTopProductos";
            this.lblTopProductos.Text = "🔥 Top 5 Productos Más Vendidos";

            this.dgvTopProductos.BackgroundColor = System.Drawing.Color.White;
            this.dgvTopProductos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTopProductos.ColumnHeadersHeight = 30;
            this.dgvTopProductos.Location = new System.Drawing.Point(20, 260);
            this.dgvTopProductos.Name = "dgvTopProductos";
            this.dgvTopProductos.ReadOnly = true;
            this.dgvTopProductos.RowHeadersVisible = false;
            this.dgvTopProductos.Size = new System.Drawing.Size(460, 250);
            this.dgvTopProductos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            // --- GRID: BAJO STOCK ---
            this.lblBajoStock.AutoSize = true;
            this.lblBajoStock.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBajoStock.ForeColor = System.Drawing.Color.Firebrick;
            this.lblBajoStock.Location = new System.Drawing.Point(500, 230);
            this.lblBajoStock.Name = "lblBajoStock";
            this.lblBajoStock.Text = "⚠️ Productos con Stock Crítico";

            this.dgvBajoStock.BackgroundColor = System.Drawing.Color.White;
            this.dgvBajoStock.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvBajoStock.ColumnHeadersHeight = 30;
            this.dgvBajoStock.Location = new System.Drawing.Point(500, 260);
            this.dgvBajoStock.Name = "dgvBajoStock";
            this.dgvBajoStock.ReadOnly = true;
            this.dgvBajoStock.RowHeadersVisible = false;
            this.dgvBajoStock.Size = new System.Drawing.Size(460, 250);
            this.dgvBajoStock.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            // BOTÓN ACTUALIZAR
            this.btnActualizar.BackColor = System.Drawing.Color.DimGray;
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(830, 25);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(130, 35);
            this.btnActualizar.Text = "↻ Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);

            // Form Config
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.dgvBajoStock);
            this.Controls.Add(this.lblBajoStock);
            this.Controls.Add(this.dgvTopProductos);
            this.Controls.Add(this.lblTopProductos);
            this.Controls.Add(this.panelAlerta);
            this.Controls.Add(this.panelGanancia);
            this.Controls.Add(this.panelMes);
            this.Controls.Add(this.panelVentasHoy);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Name = "DashboardForm";
            this.Text = "Dashboard VENDEMAX";
            this.Load += new System.EventHandler(this.DashboardForm_Load);

            this.panelVentasHoy.ResumeLayout(false);
            this.panelVentasHoy.PerformLayout();
            this.panelMes.ResumeLayout(false);
            this.panelMes.PerformLayout();
            this.panelGanancia.ResumeLayout(false);
            this.panelGanancia.PerformLayout();
            this.panelAlerta.ResumeLayout(false);
            this.panelAlerta.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopProductos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBajoStock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panelVentasHoy;
        private System.Windows.Forms.Label lblVentasHoyMonto;
        private System.Windows.Forms.Label lblVentasHoyTitulo;
        private System.Windows.Forms.Panel panelMes;
        private System.Windows.Forms.Label lblVentasMesMonto;
        private System.Windows.Forms.Label lblVentasMesTitulo;
        private System.Windows.Forms.Panel panelGanancia;
        private System.Windows.Forms.Label lblGananciaMonto;
        private System.Windows.Forms.Label lblGananciaTitulo;
        private System.Windows.Forms.Panel panelAlerta;
        private System.Windows.Forms.Label lblAlertaCantidad;
        private System.Windows.Forms.Label lblAlertaTitulo;
        private System.Windows.Forms.DataGridView dgvTopProductos;
        private System.Windows.Forms.Label lblTopProductos;
        private System.Windows.Forms.DataGridView dgvBajoStock;
        private System.Windows.Forms.Label lblBajoStock;
        private System.Windows.Forms.Button btnActualizar;
    }
}