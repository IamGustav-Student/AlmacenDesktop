namespace AlmacenDesktop.Forms
{
    partial class DashboardForm
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
            lblBienvenida = new Label();
            btnRefrescar = new Button();
            pnlVentasHoy = new Panel();
            lblVentasHoyMonto = new Label();
            lblTituloVentas = new Label();
            pnlCantVentas = new Panel();
            lblCantVentasNumero = new Label();
            lblTituloCant = new Label();
            pnlTicketPromedio = new Panel();
            lblTicketPromedioMonto = new Label();
            lblTituloTicket = new Label();
            pnlGrafico = new Panel();
            lblTituloGrafico = new Label();
            grpAccesos = new GroupBox();
            btnRapidoCaja = new Button();
            btnRapidoProductos = new Button();
            btnRapidoVenta = new Button();
            grpAtajos = new GroupBox();
            lblKeyF5 = new Label();
            lblHelpF5 = new Label();
            lblKeyF3 = new Label();
            lblHelpF3 = new Label();
            lblKeyF2 = new Label();
            lblHelpF2 = new Label();
            lblKeyF1 = new Label();
            lblHelpF1 = new Label();
            grpAlertas = new GroupBox();
            dgvAlertasStock = new DataGridView();
            grpTopProductos = new GroupBox();
            lstTopProductos = new ListBox();
            pnlVentasHoy.SuspendLayout();
            pnlCantVentas.SuspendLayout();
            pnlTicketPromedio.SuspendLayout();
            pnlGrafico.SuspendLayout();
            grpAccesos.SuspendLayout();
            grpAtajos.SuspendLayout();
            grpAlertas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAlertasStock).BeginInit();
            grpTopProductos.SuspendLayout();
            SuspendLayout();
            // 
            // lblBienvenida
            // 
            lblBienvenida.AutoSize = true;
            lblBienvenida.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblBienvenida.ForeColor = Color.DimGray;
            lblBienvenida.Location = new Point(20, 15);
            lblBienvenida.Name = "lblBienvenida";
            lblBienvenida.Size = new Size(268, 32);
            lblBienvenida.TabIndex = 0;
            lblBienvenida.Text = "Tablero de Control 📊";
            // 
            // btnRefrescar
            // 
            btnRefrescar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefrescar.FlatStyle = FlatStyle.Flat;
            btnRefrescar.ForeColor = Color.DimGray;
            btnRefrescar.Location = new Point(850, 15);
            btnRefrescar.Name = "btnRefrescar";
            btnRefrescar.Size = new Size(120, 35);
            btnRefrescar.TabIndex = 99;
            btnRefrescar.Text = "🔄 Actualizar (F5)";
            btnRefrescar.UseVisualStyleBackColor = true;
            btnRefrescar.Click += btnRefrescar_Click;
            // 
            // pnlVentasHoy
            // 
            pnlVentasHoy.BackColor = Color.FromArgb(46, 204, 113);
            pnlVentasHoy.Controls.Add(lblVentasHoyMonto);
            pnlVentasHoy.Controls.Add(lblTituloVentas);
            pnlVentasHoy.Location = new Point(25, 70);
            pnlVentasHoy.Name = "pnlVentasHoy";
            pnlVentasHoy.Size = new Size(250, 100);
            pnlVentasHoy.TabIndex = 1;
            // 
            // lblVentasHoyMonto
            // 
            lblVentasHoyMonto.AutoSize = true;
            lblVentasHoyMonto.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblVentasHoyMonto.ForeColor = Color.White;
            lblVentasHoyMonto.Location = new Point(10, 40);
            lblVentasHoyMonto.Name = "lblVentasHoyMonto";
            lblVentasHoyMonto.Size = new Size(95, 37);
            lblVentasHoyMonto.TabIndex = 0;
            lblVentasHoyMonto.Text = "$ 0.00";
            // 
            // lblTituloVentas
            // 
            lblTituloVentas.AutoSize = true;
            lblTituloVentas.Font = new Font("Segoe UI", 10F);
            lblTituloVentas.ForeColor = Color.White;
            lblTituloVentas.Location = new Point(10, 10);
            lblTituloVentas.Name = "lblTituloVentas";
            lblTituloVentas.Size = new Size(98, 19);
            lblTituloVentas.TabIndex = 1;
            lblTituloVentas.Text = "Ventas de Hoy";
            // 
            // pnlCantVentas
            // 
            pnlCantVentas.BackColor = Color.FromArgb(52, 152, 219);
            pnlCantVentas.Controls.Add(lblCantVentasNumero);
            pnlCantVentas.Controls.Add(lblTituloCant);
            pnlCantVentas.Location = new Point(290, 70);
            pnlCantVentas.Name = "pnlCantVentas";
            pnlCantVentas.Size = new Size(200, 100);
            pnlCantVentas.TabIndex = 2;
            // 
            // lblCantVentasNumero
            // 
            lblCantVentasNumero.AutoSize = true;
            lblCantVentasNumero.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblCantVentasNumero.ForeColor = Color.White;
            lblCantVentasNumero.Location = new Point(10, 40);
            lblCantVentasNumero.Name = "lblCantVentasNumero";
            lblCantVentasNumero.Size = new Size(33, 37);
            lblCantVentasNumero.TabIndex = 0;
            lblCantVentasNumero.Text = "0";
            // 
            // lblTituloCant
            // 
            lblTituloCant.AutoSize = true;
            lblTituloCant.Font = new Font("Segoe UI", 10F);
            lblTituloCant.ForeColor = Color.White;
            lblTituloCant.Location = new Point(10, 10);
            lblTituloCant.Name = "lblTituloCant";
            lblTituloCant.Size = new Size(86, 19);
            lblTituloCant.TabIndex = 1;
            lblTituloCant.Text = "Cant. Ventas";
            // 
            // pnlTicketPromedio
            // 
            pnlTicketPromedio.BackColor = Color.FromArgb(155, 89, 182);
            pnlTicketPromedio.Controls.Add(lblTicketPromedioMonto);
            pnlTicketPromedio.Controls.Add(lblTituloTicket);
            pnlTicketPromedio.Location = new Point(505, 70);
            pnlTicketPromedio.Name = "pnlTicketPromedio";
            pnlTicketPromedio.Size = new Size(200, 100);
            pnlTicketPromedio.TabIndex = 3;
            // 
            // lblTicketPromedioMonto
            // 
            lblTicketPromedioMonto.AutoSize = true;
            lblTicketPromedioMonto.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTicketPromedioMonto.ForeColor = Color.White;
            lblTicketPromedioMonto.Location = new Point(10, 40);
            lblTicketPromedioMonto.Name = "lblTicketPromedioMonto";
            lblTicketPromedioMonto.Size = new Size(95, 37);
            lblTicketPromedioMonto.TabIndex = 0;
            lblTicketPromedioMonto.Text = "$ 0.00";
            // 
            // lblTituloTicket
            // 
            lblTituloTicket.AutoSize = true;
            lblTituloTicket.Font = new Font("Segoe UI", 10F);
            lblTituloTicket.ForeColor = Color.White;
            lblTituloTicket.Location = new Point(10, 10);
            lblTituloTicket.Name = "lblTituloTicket";
            lblTituloTicket.Size = new Size(107, 19);
            lblTituloTicket.TabIndex = 1;
            lblTituloTicket.Text = "Ticket Promedio";
            // 
            // pnlGrafico
            // 
            pnlGrafico.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlGrafico.BackColor = Color.White;
            pnlGrafico.BorderStyle = BorderStyle.FixedSingle;
            pnlGrafico.Controls.Add(lblTituloGrafico);
            pnlGrafico.Location = new Point(25, 190);
            pnlGrafico.Name = "pnlGrafico";
            pnlGrafico.Size = new Size(680, 180);
            pnlGrafico.TabIndex = 5;
            pnlGrafico.Paint += pnlGrafico_Paint;
            // 
            // lblTituloGrafico
            // 
            lblTituloGrafico.AutoSize = true;
            lblTituloGrafico.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTituloGrafico.ForeColor = Color.Gray;
            lblTituloGrafico.Location = new Point(5, 5);
            lblTituloGrafico.Name = "lblTituloGrafico";
            lblTituloGrafico.Size = new Size(126, 15);
            lblTituloGrafico.TabIndex = 0;
            lblTituloGrafico.Text = "Ventas Últimos 7 Días";
            // 
            // grpAccesos
            // 
            grpAccesos.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpAccesos.Controls.Add(btnRapidoCaja);
            grpAccesos.Controls.Add(btnRapidoProductos);
            grpAccesos.Controls.Add(btnRapidoVenta);
            grpAccesos.Font = new Font("Segoe UI", 10F);
            grpAccesos.Location = new Point(720, 70);
            grpAccesos.Name = "grpAccesos";
            grpAccesos.Size = new Size(250, 200);
            grpAccesos.TabIndex = 4;
            grpAccesos.TabStop = false;
            grpAccesos.Text = "Accesos Rápidos";
            // 
            // btnRapidoCaja
            // 
            btnRapidoCaja.BackColor = Color.DarkOrange;
            btnRapidoCaja.FlatStyle = FlatStyle.Flat;
            btnRapidoCaja.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnRapidoCaja.ForeColor = Color.White;
            btnRapidoCaja.Location = new Point(15, 135);
            btnRapidoCaja.Name = "btnRapidoCaja";
            btnRapidoCaja.Size = new Size(220, 40);
            btnRapidoCaja.TabIndex = 2;
            btnRapidoCaja.Text = "💰 Control Caja";
            btnRapidoCaja.UseVisualStyleBackColor = false;
            btnRapidoCaja.Click += btnRapidoCaja_Click;
            // 
            // btnRapidoProductos
            // 
            btnRapidoProductos.BackColor = Color.SteelBlue;
            btnRapidoProductos.FlatStyle = FlatStyle.Flat;
            btnRapidoProductos.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnRapidoProductos.ForeColor = Color.White;
            btnRapidoProductos.Location = new Point(15, 85);
            btnRapidoProductos.Name = "btnRapidoProductos";
            btnRapidoProductos.Size = new Size(220, 40);
            btnRapidoProductos.TabIndex = 1;
            btnRapidoProductos.Text = "📦 Productos";
            btnRapidoProductos.UseVisualStyleBackColor = false;
            btnRapidoProductos.Click += btnRapidoProductos_Click;
            // 
            // btnRapidoVenta
            // 
            btnRapidoVenta.BackColor = Color.Teal;
            btnRapidoVenta.FlatStyle = FlatStyle.Flat;
            btnRapidoVenta.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRapidoVenta.ForeColor = Color.White;
            btnRapidoVenta.Location = new Point(15, 30);
            btnRapidoVenta.Name = "btnRapidoVenta";
            btnRapidoVenta.Size = new Size(220, 45);
            btnRapidoVenta.TabIndex = 0;
            btnRapidoVenta.Text = "\U0001f6d2 NUEVA VENTA";
            btnRapidoVenta.UseVisualStyleBackColor = false;
            btnRapidoVenta.Click += btnRapidoVenta_Click;
            // 
            // grpAtajos
            // 
            grpAtajos.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            grpAtajos.Controls.Add(lblKeyF5);
            grpAtajos.Controls.Add(lblHelpF5);
            grpAtajos.Controls.Add(lblKeyF3);
            grpAtajos.Controls.Add(lblHelpF3);
            grpAtajos.Controls.Add(lblKeyF2);
            grpAtajos.Controls.Add(lblHelpF2);
            grpAtajos.Controls.Add(lblKeyF1);
            grpAtajos.Controls.Add(lblHelpF1);
            grpAtajos.Font = new Font("Segoe UI", 10F);
            grpAtajos.ForeColor = Color.DimGray;
            grpAtajos.Location = new Point(720, 280);
            grpAtajos.Name = "grpAtajos";
            grpAtajos.Size = new Size(250, 150);
            grpAtajos.TabIndex = 8;
            grpAtajos.TabStop = false;
            grpAtajos.Text = "⌨️ Teclas Rápidas";
            // 
            // lblKeyF5
            // 
            lblKeyF5.AutoSize = true;
            lblKeyF5.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblKeyF5.ForeColor = Color.Gray;
            lblKeyF5.Location = new Point(20, 105);
            lblKeyF5.Name = "lblKeyF5";
            lblKeyF5.Size = new Size(24, 19);
            lblKeyF5.TabIndex = 0;
            lblKeyF5.Text = "F5";
            // 
            // lblHelpF5
            // 
            lblHelpF5.AutoSize = true;
            lblHelpF5.Location = new Point(55, 105);
            lblHelpF5.Name = "lblHelpF5";
            lblHelpF5.Size = new Size(108, 19);
            lblHelpF5.TabIndex = 1;
            lblHelpF5.Text = "Actualizar Datos";
            // 
            // lblKeyF3
            // 
            lblKeyF3.AutoSize = true;
            lblKeyF3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblKeyF3.ForeColor = Color.DarkOrange;
            lblKeyF3.Location = new Point(20, 80);
            lblKeyF3.Name = "lblKeyF3";
            lblKeyF3.Size = new Size(24, 19);
            lblKeyF3.TabIndex = 2;
            lblKeyF3.Text = "F3";
            // 
            // lblHelpF3
            // 
            lblHelpF3.AutoSize = true;
            lblHelpF3.Location = new Point(55, 80);
            lblHelpF3.Name = "lblHelpF3";
            lblHelpF3.Size = new Size(104, 19);
            lblHelpF3.TabIndex = 3;
            lblHelpF3.Text = "Control de Caja";
            // 
            // lblKeyF2
            // 
            lblKeyF2.AutoSize = true;
            lblKeyF2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblKeyF2.ForeColor = Color.SteelBlue;
            lblKeyF2.Location = new Point(20, 55);
            lblKeyF2.Name = "lblKeyF2";
            lblKeyF2.Size = new Size(24, 19);
            lblKeyF2.TabIndex = 4;
            lblKeyF2.Text = "F2";
            // 
            // lblHelpF2
            // 
            lblHelpF2.AutoSize = true;
            lblHelpF2.Location = new Point(55, 55);
            lblHelpF2.Name = "lblHelpF2";
            lblHelpF2.Size = new Size(95, 19);
            lblHelpF2.TabIndex = 5;
            lblHelpF2.Text = "Ver Productos";
            // 
            // lblKeyF1
            // 
            lblKeyF1.AutoSize = true;
            lblKeyF1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblKeyF1.ForeColor = Color.Teal;
            lblKeyF1.Location = new Point(20, 30);
            lblKeyF1.Name = "lblKeyF1";
            lblKeyF1.Size = new Size(24, 19);
            lblKeyF1.TabIndex = 6;
            lblKeyF1.Text = "F1";
            // 
            // lblHelpF1
            // 
            lblHelpF1.AutoSize = true;
            lblHelpF1.Location = new Point(55, 30);
            lblHelpF1.Name = "lblHelpF1";
            lblHelpF1.Size = new Size(87, 19);
            lblHelpF1.TabIndex = 7;
            lblHelpF1.Text = "Nueva Venta";
            // 
            // grpAlertas
            // 
            grpAlertas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpAlertas.Controls.Add(dgvAlertasStock);
            grpAlertas.Font = new Font("Segoe UI", 10F);
            grpAlertas.ForeColor = Color.Firebrick;
            grpAlertas.Location = new Point(25, 380);
            grpAlertas.Name = "grpAlertas";
            grpAlertas.Size = new Size(680, 300);
            grpAlertas.TabIndex = 6;
            grpAlertas.TabStop = false;
            grpAlertas.Text = "⚠️ Alertas de Stock Bajo (Menos de 10 unidades)";
            // 
            // dgvAlertasStock
            // 
            dgvAlertasStock.AllowUserToAddRows = false;
            dgvAlertasStock.AllowUserToDeleteRows = false;
            dgvAlertasStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAlertasStock.BackgroundColor = Color.White;
            dgvAlertasStock.BorderStyle = BorderStyle.None;
            dgvAlertasStock.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAlertasStock.Dock = DockStyle.Fill;
            dgvAlertasStock.Location = new Point(3, 21);
            dgvAlertasStock.Name = "dgvAlertasStock";
            dgvAlertasStock.ReadOnly = true;
            dgvAlertasStock.RowHeadersVisible = false;
            dgvAlertasStock.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAlertasStock.Size = new Size(674, 276);
            dgvAlertasStock.TabIndex = 0;
            // 
            // grpTopProductos
            // 
            grpTopProductos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            grpTopProductos.Controls.Add(lstTopProductos);
            grpTopProductos.Font = new Font("Segoe UI", 9F);
            grpTopProductos.Location = new Point(720, 440);
            grpTopProductos.Name = "grpTopProductos";
            grpTopProductos.Size = new Size(250, 240);
            grpTopProductos.TabIndex = 7;
            grpTopProductos.TabStop = false;
            grpTopProductos.Text = "⭐ Top 5 Más Vendidos";
            // 
            // lstTopProductos
            // 
            lstTopProductos.BackColor = Color.WhiteSmoke;
            lstTopProductos.BorderStyle = BorderStyle.None;
            lstTopProductos.Dock = DockStyle.Fill;
            lstTopProductos.FormattingEnabled = true;
            lstTopProductos.ItemHeight = 15;
            lstTopProductos.Location = new Point(3, 19);
            lstTopProductos.Name = "lstTopProductos";
            lstTopProductos.Size = new Size(244, 218);
            lstTopProductos.TabIndex = 0;
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1000, 700);
            Controls.Add(grpAtajos);
            Controls.Add(grpTopProductos);
            Controls.Add(grpAlertas);
            Controls.Add(pnlGrafico);
            Controls.Add(grpAccesos);
            Controls.Add(pnlTicketPromedio);
            Controls.Add(pnlCantVentas);
            Controls.Add(pnlVentasHoy);
            Controls.Add(btnRefrescar);
            Controls.Add(lblBienvenida);
            FormBorderStyle = FormBorderStyle.None;
            MinimizeBox = false;
            MinimumSize = new Size(1000, 700);
            Name = "DashboardForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dashboard";
            pnlVentasHoy.ResumeLayout(false);
            pnlVentasHoy.PerformLayout();
            pnlCantVentas.ResumeLayout(false);
            pnlCantVentas.PerformLayout();
            pnlTicketPromedio.ResumeLayout(false);
            pnlTicketPromedio.PerformLayout();
            pnlGrafico.ResumeLayout(false);
            pnlGrafico.PerformLayout();
            grpAccesos.ResumeLayout(false);
            grpAtajos.ResumeLayout(false);
            grpAtajos.PerformLayout();
            grpAlertas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvAlertasStock).EndInit();
            grpTopProductos.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblBienvenida;
        private System.Windows.Forms.Button btnRefrescar;

        private System.Windows.Forms.Panel pnlVentasHoy;
        private System.Windows.Forms.Label lblVentasHoyMonto;
        private System.Windows.Forms.Label lblTituloVentas;

        private System.Windows.Forms.Panel pnlCantVentas;
        private System.Windows.Forms.Label lblCantVentasNumero;
        private System.Windows.Forms.Label lblTituloCant;

        private System.Windows.Forms.Panel pnlTicketPromedio;
        private System.Windows.Forms.Label lblTicketPromedioMonto;
        private System.Windows.Forms.Label lblTituloTicket;

        private System.Windows.Forms.Panel pnlGrafico;
        private System.Windows.Forms.Label lblTituloGrafico;

        private System.Windows.Forms.GroupBox grpAccesos;
        private System.Windows.Forms.Button btnRapidoVenta;
        private System.Windows.Forms.Button btnRapidoProductos;
        private System.Windows.Forms.Button btnRapidoCaja;

        private System.Windows.Forms.GroupBox grpAtajos;
        private System.Windows.Forms.Label lblKeyF1;
        private System.Windows.Forms.Label lblHelpF1;
        private System.Windows.Forms.Label lblKeyF2;
        private System.Windows.Forms.Label lblHelpF2;
        private System.Windows.Forms.Label lblKeyF3;
        private System.Windows.Forms.Label lblHelpF3;
        private System.Windows.Forms.Label lblKeyF5;
        private System.Windows.Forms.Label lblHelpF5;

        private System.Windows.Forms.GroupBox grpAlertas;
        private System.Windows.Forms.DataGridView dgvAlertasStock;

        private System.Windows.Forms.GroupBox grpTopProductos;
        private System.Windows.Forms.ListBox lstTopProductos;
    }
}