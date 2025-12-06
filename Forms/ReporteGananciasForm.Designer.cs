namespace AlmacenDesktop.Forms
{
    partial class ReporteGananciasForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitulo = new Label();
            dtpDesde = new DateTimePicker();
            dtpHasta = new DateTimePicker();
            label1 = new Label();
            label2 = new Label();
            btnCalcular = new Button();
            pnlResultados = new Panel();
            label3 = new Label();
            lblVentaTotal = new Label();
            label4 = new Label();
            lblCostoTotal = new Label();
            label5 = new Label();
            lblGananciaNeta = new Label();
            dgvDetalle = new DataGridView();
            pnlResultados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDetalle).BeginInit();
            SuspendLayout();
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.DimGray;
            lblTitulo.Location = new Point(20, 20);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(431, 30);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Reporte de Rentabilidad y Ganancias 💰";
            // 
            // dtpDesde
            // 
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(70, 67);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(200, 23);
            dtpDesde.TabIndex = 1;
            // 
            // dtpHasta
            // 
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(286, 67);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(200, 23);
            dtpHasta.TabIndex = 3;
            // 
            // label1
            // 
            label1.Location = new Point(25, 70);
            label1.Name = "label1";
            label1.Size = new Size(100, 23);
            label1.TabIndex = 2;
            label1.Text = "Desde:";
            // 
            // label2
            // 
            label2.Location = new Point(200, 70);
            label2.Name = "label2";
            label2.Size = new Size(100, 23);
            label2.TabIndex = 4;
            label2.Text = "Hasta:";
            // 
            // btnCalcular
            // 
            btnCalcular.BackColor = Color.SteelBlue;
            btnCalcular.FlatStyle = FlatStyle.Flat;
            btnCalcular.ForeColor = Color.White;
            btnCalcular.Location = new Point(526, 65);
            btnCalcular.Name = "btnCalcular";
            btnCalcular.Size = new Size(150, 30);
            btnCalcular.TabIndex = 5;
            btnCalcular.Text = "CALCULAR GANANCIA";
            btnCalcular.UseVisualStyleBackColor = false;
            btnCalcular.Click += btnCalcular_Click;
            // 
            // pnlResultados
            // 
            pnlResultados.BackColor = Color.White;
            pnlResultados.BorderStyle = BorderStyle.FixedSingle;
            pnlResultados.Controls.Add(label3);
            pnlResultados.Controls.Add(lblVentaTotal);
            pnlResultados.Controls.Add(label4);
            pnlResultados.Controls.Add(lblCostoTotal);
            pnlResultados.Controls.Add(label5);
            pnlResultados.Controls.Add(lblGananciaNeta);
            pnlResultados.Location = new Point(25, 120);
            pnlResultados.Name = "pnlResultados";
            pnlResultados.Size = new Size(740, 100);
            pnlResultados.TabIndex = 6;
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI", 10F);
            label3.Location = new Point(20, 20);
            label3.Name = "label3";
            label3.Size = new Size(100, 23);
            label3.TabIndex = 0;
            label3.Text = "Ventas Totales";
            // 
            // lblVentaTotal
            // 
            lblVentaTotal.AutoSize = true;
            lblVentaTotal.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblVentaTotal.ForeColor = Color.Blue;
            lblVentaTotal.Location = new Point(33, 45);
            lblVentaTotal.Name = "lblVentaTotal";
            lblVentaTotal.Size = new Size(66, 25);
            lblVentaTotal.TabIndex = 1;
            lblVentaTotal.Text = "$ 0.00";
            // 
            // label4
            // 
            label4.Font = new Font("Segoe UI", 10F);
            label4.Location = new Point(229, 20);
            label4.Name = "label4";
            label4.Size = new Size(155, 25);
            label4.TabIndex = 2;
            label4.Text = "Costo Mercadería";
            // 
            // lblCostoTotal
            // 
            lblCostoTotal.AutoSize = true;
            lblCostoTotal.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblCostoTotal.ForeColor = Color.Red;
            lblCostoTotal.Location = new Point(245, 45);
            lblCostoTotal.Name = "lblCostoTotal";
            lblCostoTotal.Size = new Size(66, 25);
            lblCostoTotal.TabIndex = 3;
            lblCostoTotal.Text = "$ 0.00";
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label5.Location = new Point(480, 20);
            label5.Name = "label5";
            label5.Size = new Size(100, 23);
            label5.TabIndex = 4;
            label5.Text = "GANANCIA NETA";
            // 
            // lblGananciaNeta
            // 
            lblGananciaNeta.AutoSize = true;
            lblGananciaNeta.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblGananciaNeta.ForeColor = Color.Green;
            lblGananciaNeta.Location = new Point(480, 45);
            lblGananciaNeta.Name = "lblGananciaNeta";
            lblGananciaNeta.Size = new Size(84, 32);
            lblGananciaNeta.TabIndex = 5;
            lblGananciaNeta.Text = "$ 0.00";
            // 
            // dgvDetalle
            // 
            dgvDetalle.AllowUserToAddRows = false;
            dgvDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDetalle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDetalle.Location = new Point(25, 240);
            dgvDetalle.Name = "dgvDetalle";
            dgvDetalle.ReadOnly = true;
            dgvDetalle.Size = new Size(740, 300);
            dgvDetalle.TabIndex = 7;
            // 
            // ReporteGananciasForm
            // 
            ClientSize = new Size(784, 561);
            Controls.Add(lblTitulo);
            Controls.Add(dtpDesde);
            Controls.Add(label1);
            Controls.Add(dtpHasta);
            Controls.Add(label2);
            Controls.Add(btnCalcular);
            Controls.Add(pnlResultados);
            Controls.Add(dgvDetalle);
            Name = "ReporteGananciasForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Reporte de Ganancias";
            pnlResultados.ResumeLayout(false);
            pnlResultados.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDetalle).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCalcular;
        private System.Windows.Forms.Panel pnlResultados;
        private System.Windows.Forms.Label lblVentaTotal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCostoTotal;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblGananciaNeta;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dgvDetalle;
    }
}