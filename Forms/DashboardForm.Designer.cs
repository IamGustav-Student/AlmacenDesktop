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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblVentasHoy = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblVentasMes = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblBajoStock = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblClientes = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dgvUltimasVentas = new System.Windows.Forms.DataGridView();
            this.label9 = new System.Windows.Forms.Label();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.chartVentas = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartPagos = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUltimasVentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPagos)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.ForeColor = System.Drawing.Color.DimGray;
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(203, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Panel de Resumen";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SeaGreen;
            this.panel1.Controls.Add(this.lblVentasHoy);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(25, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(220, 90);
            this.panel1.TabIndex = 1;
            // 
            // lblVentasHoy
            // 
            this.lblVentasHoy.AutoSize = true;
            this.lblVentasHoy.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblVentasHoy.ForeColor = System.Drawing.Color.White;
            this.lblVentasHoy.Location = new System.Drawing.Point(15, 40);
            this.lblVentasHoy.Name = "lblVentasHoy";
            this.lblVentasHoy.Size = new System.Drawing.Size(84, 32);
            this.lblVentasHoy.TabIndex = 1;
            this.lblVentasHoy.Text = "$ 0.00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(15, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ventas de Hoy";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.SteelBlue;
            this.panel2.Controls.Add(this.lblVentasMes);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(265, 60);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(220, 90);
            this.panel2.TabIndex = 2;
            // 
            // lblVentasMes
            // 
            this.lblVentasMes.AutoSize = true;
            this.lblVentasMes.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblVentasMes.ForeColor = System.Drawing.Color.White;
            this.lblVentasMes.Location = new System.Drawing.Point(15, 40);
            this.lblVentasMes.Name = "lblVentasMes";
            this.lblVentasMes.Size = new System.Drawing.Size(84, 32);
            this.lblVentasMes.TabIndex = 1;
            this.lblVentasMes.Text = "$ 0.00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label3.Location = new System.Drawing.Point(15, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 19);
            this.label3.TabIndex = 0;
            this.label3.Text = "Ventas del Mes";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.IndianRed;
            this.panel3.Controls.Add(this.lblBajoStock);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Location = new System.Drawing.Point(505, 60);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(220, 90);
            this.panel3.TabIndex = 3;
            // 
            // lblBajoStock
            // 
            this.lblBajoStock.AutoSize = true;
            this.lblBajoStock.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBajoStock.ForeColor = System.Drawing.Color.White;
            this.lblBajoStock.Location = new System.Drawing.Point(15, 40);
            this.lblBajoStock.Name = "lblBajoStock";
            this.lblBajoStock.Size = new System.Drawing.Size(28, 32);
            this.lblBajoStock.TabIndex = 1;
            this.lblBajoStock.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label5.Location = new System.Drawing.Point(15, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 19);
            this.label5.TabIndex = 0;
            this.label5.Text = "Alerta: Stock Crítico";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.DarkOrange;
            this.panel4.Controls.Add(this.lblClientes);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Location = new System.Drawing.Point(745, 60);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(220, 90);
            this.panel4.TabIndex = 4;
            // 
            // lblClientes
            // 
            this.lblClientes.AutoSize = true;
            this.lblClientes.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblClientes.ForeColor = System.Drawing.Color.White;
            this.lblClientes.Location = new System.Drawing.Point(15, 40);
            this.lblClientes.Name = "lblClientes";
            this.lblClientes.Size = new System.Drawing.Size(28, 32);
            this.lblClientes.TabIndex = 1;
            this.lblClientes.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label7.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label7.Location = new System.Drawing.Point(15, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 19);
            this.label7.TabIndex = 0;
            this.label7.Text = "Clientes Activos";
            // 
            // chartVentas
            // 
            chartArea1.Name = "ChartArea1";
            this.chartVentas.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartVentas.Legends.Add(legend1);
            this.chartVentas.Location = new System.Drawing.Point(25, 170);
            this.chartVentas.Name = "chartVentas";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Ventas";
            this.chartVentas.Series.Add(series1);
            this.chartVentas.Size = new System.Drawing.Size(580, 200);
            this.chartVentas.TabIndex = 8;
            this.chartVentas.Text = "Ventas Semanales";
            title1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            title1.Name = "Title1";
            title1.Text = "Ventas Últimos 7 Días";
            this.chartVentas.Titles.Add(title1);
            // 
            // chartPagos
            // 
            chartArea2.Name = "ChartArea1";
            this.chartPagos.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartPagos.Legends.Add(legend2);
            this.chartPagos.Location = new System.Drawing.Point(620, 170);
            this.chartPagos.Name = "chartPagos";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartPagos.Series.Add(series2);
            this.chartPagos.Size = new System.Drawing.Size(345, 200);
            this.chartPagos.TabIndex = 9;
            this.chartPagos.Text = "Métodos Pago";
            title2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            title2.Name = "Title1";
            title2.Text = "Métodos de Pago";
            this.chartPagos.Titles.Add(title2);
            // 
            // dgvUltimasVentas
            // 
            this.dgvUltimasVentas.AllowUserToAddRows = false;
            this.dgvUltimasVentas.AllowUserToDeleteRows = false;
            this.dgvUltimasVentas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUltimasVentas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUltimasVentas.BackgroundColor = System.Drawing.Color.White;
            this.dgvUltimasVentas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvUltimasVentas.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvUltimasVentas.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvUltimasVentas.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvUltimasVentas.ColumnHeadersHeight = 30;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.AliceBlue;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvUltimasVentas.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvUltimasVentas.EnableHeadersVisualStyles = false;
            this.dgvUltimasVentas.GridColor = System.Drawing.Color.WhiteSmoke;
            this.dgvUltimasVentas.Location = new System.Drawing.Point(25, 410);
            this.dgvUltimasVentas.Name = "dgvUltimasVentas";
            this.dgvUltimasVentas.ReadOnly = true;
            this.dgvUltimasVentas.RowHeadersVisible = false;
            this.dgvUltimasVentas.RowTemplate.Height = 35;
            this.dgvUltimasVentas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUltimasVentas.Size = new System.Drawing.Size(940, 180);
            this.dgvUltimasVentas.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label9.ForeColor = System.Drawing.Color.DimGray;
            this.label9.Location = new System.Drawing.Point(25, 385);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(243, 21);
            this.label9.TabIndex = 6;
            this.label9.Text = "Últimas Operaciones (En vivo)";
            // 
            // btnActualizar
            // 
            this.btnActualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActualizar.BackColor = System.Drawing.Color.White;
            this.btnActualizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnActualizar.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.ForeColor = System.Drawing.Color.DimGray;
            this.btnActualizar.Location = new System.Drawing.Point(865, 15);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 30);
            this.btnActualizar.TabIndex = 7;
            this.btnActualizar.Text = "↻ Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            // 
            // DashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.chartPagos);
            this.Controls.Add(this.chartVentas);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.dgvUltimasVentas);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DashboardForm";
            this.Text = "Dashboard";
            this.Load += new System.EventHandler(this.DashboardForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUltimasVentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPagos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblVentasHoy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblVentasMes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblBajoStock;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblClientes;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dgvUltimasVentas;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartVentas;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPagos;
    }
}