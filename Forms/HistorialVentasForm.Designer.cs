namespace AlmacenDesktop.Forms
{
    partial class HistorialVentasForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelFiltros = new Panel();
            btnBuscar = new Button();
            dtpHasta = new DateTimePicker();
            label2 = new Label();
            dtpDesde = new DateTimePicker();
            label1 = new Label();
            splitContainer1 = new SplitContainer();
            dgvVentas = new DataGridView();
            lblVentas = new Label();
            dgvDetalles = new DataGridView();
            lblDetalles = new Label();
            panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvDetalles).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.BackColor = Color.WhiteSmoke;
            panelFiltros.Controls.Add(btnBuscar);
            panelFiltros.Controls.Add(dtpHasta);
            panelFiltros.Controls.Add(label2);
            panelFiltros.Controls.Add(dtpDesde);
            panelFiltros.Controls.Add(label1);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Location = new Point(0, 0);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(900, 70);
            panelFiltros.TabIndex = 0;
            // 
            // btnBuscar
            // 
            btnBuscar.BackColor = Color.SteelBlue;
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.ForeColor = Color.White;
            btnBuscar.Location = new Point(400, 18);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(150, 30);
            btnBuscar.TabIndex = 0;
            btnBuscar.Text = "BUSCAR VENTAS";
            btnBuscar.UseVisualStyleBackColor = false;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // dtpHasta
            // 
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(260, 22);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(120, 23);
            dtpHasta.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(210, 25);
            label2.Name = "label2";
            label2.Size = new Size(40, 15);
            label2.TabIndex = 2;
            label2.Text = "Hasta:";
            // 
            // dtpDesde
            // 
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(70, 22);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(120, 23);
            dtpDesde.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 25);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 4;
            label1.Text = "Desde:";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 70);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(dgvVentas);
            splitContainer1.Panel1.Controls.Add(lblVentas);
            splitContainer1.Panel1.Padding = new Padding(10);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dgvDetalles);
            splitContainer1.Panel2.Controls.Add(lblDetalles);
            splitContainer1.Panel2.Padding = new Padding(10);
            splitContainer1.Size = new Size(900, 530);
            splitContainer1.SplitterDistance = 265;
            splitContainer1.TabIndex = 1;
            // 
            // dgvVentas
            // 
            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.BackgroundColor = Color.White;
            dgvVentas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvVentas.Dock = DockStyle.Fill;
            dgvVentas.Location = new Point(10, 39);
            dgvVentas.MultiSelect = false;
            dgvVentas.Name = "dgvVentas";
            dgvVentas.ReadOnly = true;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.Size = new Size(880, 216);
            dgvVentas.TabIndex = 1;
            dgvVentas.SelectionChanged += dgvVentas_SelectionChanged;
            // 
            // lblVentas
            // 
            lblVentas.AutoSize = true;
            lblVentas.Dock = DockStyle.Top;
            lblVentas.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblVentas.Location = new Point(10, 10);
            lblVentas.Name = "lblVentas";
            lblVentas.Padding = new Padding(0, 0, 0, 10);
            lblVentas.Size = new Size(194, 29);
            lblVentas.TabIndex = 2;
            lblVentas.Text = "Listado de Ventas (Facturas)";
            // 
            // dgvDetalles
            // 
            dgvDetalles.AllowUserToAddRows = false;
            dgvDetalles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDetalles.BackgroundColor = Color.WhiteSmoke;
            dgvDetalles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDetalles.Dock = DockStyle.Fill;
            dgvDetalles.Location = new Point(10, 39);
            dgvDetalles.Name = "dgvDetalles";
            dgvDetalles.ReadOnly = true;
            dgvDetalles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDetalles.Size = new Size(880, 212);
            dgvDetalles.TabIndex = 1;
            // 
            // lblDetalles
            // 
            lblDetalles.AutoSize = true;
            lblDetalles.Dock = DockStyle.Top;
            lblDetalles.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDetalles.Location = new Point(10, 10);
            lblDetalles.Name = "lblDetalles";
            lblDetalles.Padding = new Padding(0, 0, 0, 10);
            lblDetalles.Size = new Size(319, 29);
            lblDetalles.TabIndex = 2;
            lblDetalles.Text = "Detalle de Productos de la Venta Seleccionada";
            // 
            // HistorialVentasForm
            // 
            ClientSize = new Size(900, 600);
            Controls.Add(splitContainer1);
            Controls.Add(panelFiltros);
            Name = "HistorialVentasForm";
            Text = "Historial de Ventas";
            Load += HistorialVentasForm_Load;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvVentas).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvDetalles).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelFiltros;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvVentas;
        private System.Windows.Forms.Label lblVentas;
        private System.Windows.Forms.DataGridView dgvDetalles;
        private System.Windows.Forms.Label lblDetalles;
    }
}