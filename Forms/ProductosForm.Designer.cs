namespace AlmacenDesktop.Forms
{
    partial class ProductosForm
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
            System.Windows.Forms.DataGridViewCellStyle cellStyleRow = new System.Windows.Forms.DataGridViewCellStyle();

            this.panelEditor = new System.Windows.Forms.Panel();
            this.lblModo = new System.Windows.Forms.Label();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();

            // Campos
            this.lblCodigo = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblProveedor = new System.Windows.Forms.Label();
            this.cboProveedor = new System.Windows.Forms.ComboBox();

            // Numéricos
            this.lblCosto = new System.Windows.Forms.Label();
            this.numCosto = new System.Windows.Forms.NumericUpDown();
            this.lblPrecio = new System.Windows.Forms.Label();
            this.numPrecio = new System.Windows.Forms.NumericUpDown();
            this.lblStock = new System.Windows.Forms.Label();
            this.numStock = new System.Windows.Forms.NumericUpDown();
            this.lblStockMinimo = new System.Windows.Forms.Label();
            this.numStockMinimo = new System.Windows.Forms.NumericUpDown();
            this.lblImpuesto = new System.Windows.Forms.Label();
            this.numImpuesto = new System.Windows.Forms.NumericUpDown();

            // Panel Principal
            this.panelMain = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.txtBusqueda = new System.Windows.Forms.TextBox();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.lblTotalProductos = new System.Windows.Forms.Label();

            // Botones de Acción Grid
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();

            // Grid
            this.dgvProductos = new System.Windows.Forms.DataGridView();

            // Columnas Grid
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrecio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStock = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProveedor = new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.panelEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCosto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStockMinimo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numImpuesto)).BeginInit();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            this.SuspendLayout();

            // ---------------------------------------------------------
            // PANEL EDITOR (IZQUIERDA)
            // ---------------------------------------------------------
            this.panelEditor.BackColor = System.Drawing.Color.White;
            this.panelEditor.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelEditor.Width = 300;
            this.panelEditor.Controls.Add(this.lblModo);
            this.panelEditor.Controls.Add(this.txtCodigo); this.panelEditor.Controls.Add(this.lblCodigo);
            this.panelEditor.Controls.Add(this.txtNombre); this.panelEditor.Controls.Add(this.lblNombre);
            this.panelEditor.Controls.Add(this.txtDescripcion); this.panelEditor.Controls.Add(this.lblDescripcion);
            this.panelEditor.Controls.Add(this.cboProveedor); this.panelEditor.Controls.Add(this.lblProveedor);

            this.panelEditor.Controls.Add(this.numCosto); this.panelEditor.Controls.Add(this.lblCosto);
            this.panelEditor.Controls.Add(this.numPrecio); this.panelEditor.Controls.Add(this.lblPrecio);
            this.panelEditor.Controls.Add(this.numStock); this.panelEditor.Controls.Add(this.lblStock);
            this.panelEditor.Controls.Add(this.numStockMinimo); this.panelEditor.Controls.Add(this.lblStockMinimo);
            this.panelEditor.Controls.Add(this.numImpuesto); this.panelEditor.Controls.Add(this.lblImpuesto);

            this.panelEditor.Controls.Add(this.btnGuardar);
            this.panelEditor.Controls.Add(this.btnLimpiar);

            // Título Modo
            this.lblModo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblModo.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblModo.Location = new System.Drawing.Point(15, 15);
            this.lblModo.Size = new System.Drawing.Size(270, 30);
            this.lblModo.Text = "Nuevo Producto";
            this.lblModo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Campos Texto
            int y = 60;
            this.lblCodigo.Text = "Código de Barras:"; this.lblCodigo.Location = new System.Drawing.Point(15, y);
            this.txtCodigo.Location = new System.Drawing.Point(15, y + 20); this.txtCodigo.Width = 260;

            y += 50;
            this.lblNombre.Text = "Nombre Producto:"; this.lblNombre.Location = new System.Drawing.Point(15, y);
            this.txtNombre.Location = new System.Drawing.Point(15, y + 20); this.txtNombre.Width = 260;

            y += 50;
            this.lblDescripcion.Text = "Descripción (Opcional):"; this.lblDescripcion.Location = new System.Drawing.Point(15, y);
            this.txtDescripcion.Location = new System.Drawing.Point(15, y + 20); this.txtDescripcion.Width = 260;

            y += 50;
            this.lblProveedor.Text = "Proveedor:"; this.lblProveedor.Location = new System.Drawing.Point(15, y);
            this.cboProveedor.Location = new System.Drawing.Point(15, y + 20); this.cboProveedor.Width = 260;
            this.cboProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // Campos Numéricos (2 columnas)
            y += 60;
            this.lblCosto.Text = "Costo ($):"; this.lblCosto.Location = new System.Drawing.Point(15, y);
            this.numCosto.Location = new System.Drawing.Point(15, y + 20); this.numCosto.Width = 120;
            this.numCosto.DecimalPlaces = 2; this.numCosto.Maximum = 99999999;
            this.numCosto.ValueChanged += new System.EventHandler(this.numCosto_ValueChanged);

            this.lblPrecio.Text = "Precio Venta ($):"; this.lblPrecio.Location = new System.Drawing.Point(155, y);
            this.numPrecio.Location = new System.Drawing.Point(155, y + 20); this.numPrecio.Width = 120;
            this.numPrecio.DecimalPlaces = 2; this.numPrecio.Maximum = 99999999;

            y += 50;
            this.lblStock.Text = "Stock Actual:"; this.lblStock.Location = new System.Drawing.Point(15, y);
            this.numStock.Location = new System.Drawing.Point(15, y + 20); this.numStock.Width = 120;
            this.numStock.Maximum = 10000;

            this.lblStockMinimo.Text = "Stock Mínimo:"; this.lblStockMinimo.Location = new System.Drawing.Point(155, y);
            this.numStockMinimo.Location = new System.Drawing.Point(155, y + 20); this.numStockMinimo.Width = 120;
            this.numStockMinimo.Maximum = 1000;

            y += 50;
            this.lblImpuesto.Text = "Impuesto (%):"; this.lblImpuesto.Location = new System.Drawing.Point(15, y);
            this.numImpuesto.Location = new System.Drawing.Point(15, y + 20); this.numImpuesto.Width = 120;

            // Botones Editor
            this.btnGuardar.Text = "Guardar (F5)";
            this.btnGuardar.BackColor = System.Drawing.Color.ForestGreen;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Location = new System.Drawing.Point(15, 450);
            this.btnGuardar.Size = new System.Drawing.Size(260, 40);
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            this.btnLimpiar.Text = "Cancelar / Nuevo (Esc)";
            this.btnLimpiar.BackColor = System.Drawing.Color.Gray;
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.Location = new System.Drawing.Point(15, 500);
            this.btnLimpiar.Size = new System.Drawing.Size(260, 30);
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);


            // ---------------------------------------------------------
            // PANEL PRINCIPAL (DERECHA)
            // ---------------------------------------------------------
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelMain.Controls.Add(this.lblTitulo);
            this.panelMain.Controls.Add(this.lblBuscar);
            this.panelMain.Controls.Add(this.txtBusqueda);
            this.panelMain.Controls.Add(this.dgvProductos);
            this.panelMain.Controls.Add(this.btnEditar);
            this.panelMain.Controls.Add(this.btnEliminar);
            this.panelMain.Controls.Add(this.lblTotalProductos);

            // Título
            this.lblTitulo.Text = "Gestión de Productos";
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.AutoSize = true;

            // Buscador
            this.lblBuscar.Text = "Buscar (Nombre o Código):";
            this.lblBuscar.Location = new System.Drawing.Point(20, 60);
            this.lblBuscar.AutoSize = true;

            this.txtBusqueda.Location = new System.Drawing.Point(20, 80);
            this.txtBusqueda.Size = new System.Drawing.Size(400, 23);
            this.txtBusqueda.TextChanged += new System.EventHandler(this.txtBusqueda_TextChanged);

            // Botones Acción Grid
            this.btnEditar.Text = "✏️ Editar";
            this.btnEditar.BackColor = System.Drawing.Color.SteelBlue;
            this.btnEditar.ForeColor = System.Drawing.Color.White;
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.Location = new System.Drawing.Point(440, 70);
            this.btnEditar.Size = new System.Drawing.Size(100, 35);
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);

            this.btnEliminar.Text = "🗑️ Eliminar";
            this.btnEliminar.BackColor = System.Drawing.Color.IndianRed;
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.Location = new System.Drawing.Point(550, 70);
            this.btnEliminar.Size = new System.Drawing.Size(100, 35);
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

            // Label Total
            this.lblTotalProductos.Text = "Total: 0";
            this.lblTotalProductos.Location = new System.Drawing.Point(670, 80);
            this.lblTotalProductos.AutoSize = true;
            this.lblTotalProductos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // DataGridView
            this.dgvProductos.Location = new System.Drawing.Point(20, 120);
            this.dgvProductos.Size = new System.Drawing.Size(740, 420);
            this.dgvProductos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProductos.BackgroundColor = System.Drawing.Color.White;
            this.dgvProductos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProductos.RowHeadersVisible = false;
            this.dgvProductos.AllowUserToAddRows = false;
            this.dgvProductos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductos.MultiSelect = false;
            this.dgvProductos.ReadOnly = true;
            this.dgvProductos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            // Columnas
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "Id";
            this.colId.Visible = false; // Oculto

            this.colCodigo.DataPropertyName = "CodigoBarras";
            this.colCodigo.HeaderText = "Código";
            this.colCodigo.FillWeight = 80;

            this.colNombre.DataPropertyName = "Nombre";
            this.colNombre.HeaderText = "Producto";
            this.colNombre.FillWeight = 200;

            this.colPrecio.DataPropertyName = "Precio";
            this.colPrecio.HeaderText = "Precio ($)";
            this.colPrecio.DefaultCellStyle.Format = "N2";
            this.colPrecio.FillWeight = 80;

            this.colStock.DataPropertyName = "Stock";
            this.colStock.HeaderText = "Stock";
            this.colStock.FillWeight = 60;

            // Nota: Para mostrar "Proveedor.Nombre", se requiere un pequeño truco en el CellFormatting
            // o Binding avanzado. Por ahora usamos DataPropertyName simple, si falla se verá vacío.
            this.colProveedor.DataPropertyName = "ProveedorId";
            this.colProveedor.HeaderText = "Prov ID";
            this.colProveedor.Visible = false; // Ocultamos por simplicidad, o lo mostramos si es útil

            this.dgvProductos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colId, this.colCodigo, this.colNombre, this.colPrecio, this.colStock, this.colProveedor
            });

            // ---------------------------------------------------------
            // FORM
            // ---------------------------------------------------------
            this.ClientSize = new System.Drawing.Size(1084, 561);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelEditor);
            this.Name = "ProductosForm";
            this.Text = "Gestión de Productos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.ProductosForm_Load);

            this.panelEditor.ResumeLayout(false);
            this.panelEditor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCosto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStockMinimo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numImpuesto)).EndInit();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            this.ResumeLayout(false);
        }

        // Variables
        private System.Windows.Forms.Panel panelEditor;
        private System.Windows.Forms.Label lblModo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnLimpiar;

        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.ComboBox cboProveedor;

        private System.Windows.Forms.NumericUpDown numCosto;
        private System.Windows.Forms.NumericUpDown numPrecio;
        private System.Windows.Forms.NumericUpDown numStock;
        private System.Windows.Forms.NumericUpDown numStockMinimo;
        private System.Windows.Forms.NumericUpDown numImpuesto;

        // Labels
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.Label lblCosto;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.Label lblStock;
        private System.Windows.Forms.Label lblStockMinimo;
        private System.Windows.Forms.Label lblImpuesto;

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.TextBox txtBusqueda;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.Label lblTotalProductos;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.DataGridView dgvProductos;

        // Columnas
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrecio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStock;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProveedor;
    }
}