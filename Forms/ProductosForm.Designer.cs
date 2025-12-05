namespace AlmacenDesktop.Forms
{
    partial class ProductosForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cboProveedor = new System.Windows.Forms.ComboBox();
            this.dgvProductos = new System.Windows.Forms.DataGridView();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.numCosto = new System.Windows.Forms.NumericUpDown();
            this.numPrecio = new System.Windows.Forms.NumericUpDown();
            this.numStock = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numImpuesto = new System.Windows.Forms.NumericUpDown();
            this.grpDatos = new System.Windows.Forms.GroupBox();
            // NUEVOS BOTONES
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnImportar = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCosto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numImpuesto)).BeginInit();
            this.grpDatos.SuspendLayout();
            this.SuspendLayout();

            // ... (Controles existentes iguales) ...
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre:";

            this.txtNombre.Location = new System.Drawing.Point(100, 62);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(226, 23);
            this.txtNombre.TabIndex = 1;

            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(22, 33);
            this.label2.Name = "label2";
            this.label2.Text = "Código:";

            this.txtCodigo.Location = new System.Drawing.Point(100, 30);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(226, 23);
            this.txtCodigo.TabIndex = 0;

            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 97);
            this.label3.Name = "label3";
            this.label3.Text = "Costo:";

            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(22, 163);
            this.label4.Name = "label4";
            this.label4.Text = "Precio:";

            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(359, 33);
            this.label5.Name = "label5";
            this.label5.Text = "Stock:";

            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(359, 65);
            this.label6.Name = "label6";
            this.label6.Text = "Proveedor:";

            this.cboProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProveedor.FormattingEnabled = true;
            this.cboProveedor.Location = new System.Drawing.Point(437, 62);
            this.cboProveedor.Name = "cboProveedor";
            this.cboProveedor.Size = new System.Drawing.Size(202, 23);
            this.cboProveedor.TabIndex = 6;

            // DGV
            this.dgvProductos.AllowUserToAddRows = false;
            this.dgvProductos.AllowUserToDeleteRows = false;
            this.dgvProductos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProductos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductos.Location = new System.Drawing.Point(12, 233);
            this.dgvProductos.MultiSelect = false;
            this.dgvProductos.Name = "dgvProductos";
            this.dgvProductos.ReadOnly = true;
            this.dgvProductos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductos.Size = new System.Drawing.Size(760, 266);
            this.dgvProductos.TabIndex = 10;
            this.dgvProductos.SelectionChanged += new System.EventHandler(this.dgvProductos_SelectionChanged);

            // BOTONES
            this.btnGuardar.BackColor = System.Drawing.Color.ForestGreen;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(668, 30);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(95, 35);
            this.btnGuardar.TabIndex = 7;
            this.btnGuardar.Text = "Guardar (F5)";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            this.btnLimpiar.BackColor = System.Drawing.Color.SteelBlue;
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.Location = new System.Drawing.Point(668, 71);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(95, 35);
            this.btnLimpiar.TabIndex = 8;
            this.btnLimpiar.Text = "Limpiar (Esc)";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

            this.btnEliminar.BackColor = System.Drawing.Color.Firebrick;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.Location = new System.Drawing.Point(668, 112);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(95, 35);
            this.btnEliminar.TabIndex = 9;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

            // --- NUEVOS BOTONES EXCEL ---

            this.btnExportar.BackColor = System.Drawing.Color.SeaGreen;
            this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportar.ForeColor = System.Drawing.Color.White;
            this.btnExportar.Location = new System.Drawing.Point(527, 163); // Posición estratégica
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(110, 30);
            this.btnExportar.TabIndex = 20;
            this.btnExportar.Text = "📤 Exportar Excel";
            this.btnExportar.UseVisualStyleBackColor = false;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);

            this.btnImportar.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.btnImportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImportar.ForeColor = System.Drawing.Color.White;
            this.btnImportar.Location = new System.Drawing.Point(653, 163);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(110, 30);
            this.btnImportar.TabIndex = 21;
            this.btnImportar.Text = "📥 Importar Excel";
            this.btnImportar.UseVisualStyleBackColor = false;
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);

            // NUMERIC UP DOWNS
            this.numCosto.DecimalPlaces = 2;
            this.numCosto.Location = new System.Drawing.Point(100, 95);
            this.numCosto.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numCosto.Name = "numCosto";
            this.numCosto.Size = new System.Drawing.Size(120, 23);
            this.numCosto.TabIndex = 2;
            this.numCosto.ValueChanged += new System.EventHandler(this.numCosto_ValueChanged);

            this.numPrecio.DecimalPlaces = 2;
            this.numPrecio.Location = new System.Drawing.Point(100, 161);
            this.numPrecio.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numPrecio.Name = "numPrecio";
            this.numPrecio.Size = new System.Drawing.Size(120, 23);
            this.numPrecio.TabIndex = 4;

            this.numStock.Location = new System.Drawing.Point(437, 30);
            this.numStock.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numStock.Name = "numStock";
            this.numStock.Size = new System.Drawing.Size(120, 23);
            this.numStock.TabIndex = 5;

            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 131);
            this.label7.Name = "label7";
            this.label7.Text = "Margen (%):";

            this.numImpuesto.DecimalPlaces = 2;
            this.numImpuesto.Location = new System.Drawing.Point(100, 129);
            this.numImpuesto.Name = "numImpuesto";
            this.numImpuesto.Size = new System.Drawing.Size(120, 23);
            this.numImpuesto.TabIndex = 3;
            this.numImpuesto.ValueChanged += new System.EventHandler(this.numImpuesto_ValueChanged);

            // GROUP BOX
            this.grpDatos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDatos.Controls.Add(this.label1);
            this.grpDatos.Controls.Add(this.numImpuesto);
            this.grpDatos.Controls.Add(this.txtNombre);
            this.grpDatos.Controls.Add(this.label7);
            this.grpDatos.Controls.Add(this.label2);
            this.grpDatos.Controls.Add(this.numStock);
            this.grpDatos.Controls.Add(this.txtCodigo);
            this.grpDatos.Controls.Add(this.numPrecio);
            this.grpDatos.Controls.Add(this.label3);
            this.grpDatos.Controls.Add(this.numCosto);
            this.grpDatos.Controls.Add(this.label4);
            this.grpDatos.Controls.Add(this.btnEliminar);
            this.grpDatos.Controls.Add(this.label5);
            this.grpDatos.Controls.Add(this.btnLimpiar);
            this.grpDatos.Controls.Add(this.label6);
            this.grpDatos.Controls.Add(this.btnGuardar);
            this.grpDatos.Controls.Add(this.cboProveedor);
            // Agregamos los botones nuevos al groupbox
            this.grpDatos.Controls.Add(this.btnExportar);
            this.grpDatos.Controls.Add(this.btnImportar);

            this.grpDatos.Location = new System.Drawing.Point(12, 12);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(776, 215);
            this.grpDatos.TabIndex = 19;
            this.grpDatos.TabStop = false;
            this.grpDatos.Text = "Datos del Producto";

            // FORM
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 511);
            this.Controls.Add(this.grpDatos);
            this.Controls.Add(this.dgvProductos);
            this.KeyPreview = true;
            this.Name = "ProductosForm";
            this.Text = "Gestión de Productos";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ProductosForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCosto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numImpuesto)).EndInit();
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        // ... Declaraciones existentes ...
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboProveedor;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.NumericUpDown numCosto;
        private System.Windows.Forms.NumericUpDown numPrecio;
        private System.Windows.Forms.NumericUpDown numStock;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numImpuesto;
        private System.Windows.Forms.GroupBox grpDatos;

        // NUEVOS
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnImportar;
    }
}