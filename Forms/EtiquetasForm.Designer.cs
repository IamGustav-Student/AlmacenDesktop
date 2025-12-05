namespace AlmacenDesktop.Forms
{
    partial class EtiquetasForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpFiltros = new System.Windows.Forms.GroupBox();
            this.lblProd = new System.Windows.Forms.Label();
            this.cboProductos = new System.Windows.Forms.ComboBox();
            this.numCopias = new System.Windows.Forms.NumericUpDown();
            this.lblCopias = new System.Windows.Forms.Label();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.chkPrecio = new System.Windows.Forms.CheckBox();
            this.chkNombre = new System.Windows.Forms.CheckBox();

            this.pnlPreview = new System.Windows.Forms.FlowLayoutPanel();

            this.btnImprimir = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();

            this.grpFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCopias)).BeginInit();
            this.SuspendLayout();

            // 
            // grpFiltros
            // 
            this.grpFiltros.Controls.Add(this.chkNombre);
            this.grpFiltros.Controls.Add(this.chkPrecio);
            this.grpFiltros.Controls.Add(this.btnAgregar);
            this.grpFiltros.Controls.Add(this.lblCopias);
            this.grpFiltros.Controls.Add(this.numCopias);
            this.grpFiltros.Controls.Add(this.cboProductos);
            this.grpFiltros.Controls.Add(this.lblProd);
            this.grpFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpFiltros.Location = new System.Drawing.Point(0, 0);
            this.grpFiltros.Name = "grpFiltros";
            this.grpFiltros.Size = new System.Drawing.Size(900, 100);
            this.grpFiltros.TabIndex = 0;
            this.grpFiltros.TabStop = false;
            this.grpFiltros.Text = "Configuración de Etiquetas";

            // lblProd
            this.lblProd.AutoSize = true;
            this.lblProd.Location = new System.Drawing.Point(20, 30);
            this.lblProd.Name = "lblProd";
            this.lblProd.Text = "Seleccionar Producto:";

            // cboProductos
            this.cboProductos.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboProductos.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboProductos.FormattingEnabled = true;
            this.cboProductos.Location = new System.Drawing.Point(20, 50);
            this.cboProductos.Name = "cboProductos";
            this.cboProductos.Size = new System.Drawing.Size(300, 23);
            this.cboProductos.TabIndex = 0; // Foco inicial

            // lblCopias
            this.lblCopias.AutoSize = true;
            this.lblCopias.Location = new System.Drawing.Point(340, 30);
            this.lblCopias.Name = "lblCopias";
            this.lblCopias.Text = "Cantidad:";

            // numCopias
            this.numCopias.Location = new System.Drawing.Point(340, 50);
            this.numCopias.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numCopias.Name = "numCopias";
            this.numCopias.Size = new System.Drawing.Size(80, 23);
            this.numCopias.TabIndex = 1;
            this.numCopias.Value = new decimal(new int[] { 1, 0, 0, 0 });

            // Checks
            this.chkNombre.AutoSize = true;
            this.chkNombre.Checked = true;
            this.chkNombre.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNombre.Location = new System.Drawing.Point(440, 30);
            this.chkNombre.Name = "chkNombre";
            this.chkNombre.Text = "Incluir Nombre";
            this.chkNombre.UseVisualStyleBackColor = true;

            this.chkPrecio.AutoSize = true;
            this.chkPrecio.Checked = true;
            this.chkPrecio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPrecio.Location = new System.Drawing.Point(440, 55);
            this.chkPrecio.Name = "chkPrecio";
            this.chkPrecio.Text = "Incluir Precio";
            this.chkPrecio.UseVisualStyleBackColor = true;

            // btnAgregar
            this.btnAgregar.BackColor = System.Drawing.Color.SteelBlue;
            this.btnAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregar.ForeColor = System.Drawing.Color.White;
            this.btnAgregar.Location = new System.Drawing.Point(560, 40);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(120, 35);
            this.btnAgregar.TabIndex = 2;
            this.btnAgregar.Text = "Agregar a la Cola";
            this.btnAgregar.UseVisualStyleBackColor = false;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);

            // 
            // pnlPreview
            // 
            this.pnlPreview.AutoScroll = true;
            this.pnlPreview.BackColor = System.Drawing.Color.White;
            this.pnlPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPreview.Location = new System.Drawing.Point(0, 100);
            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Padding = new System.Windows.Forms.Padding(10);
            this.pnlPreview.Size = new System.Drawing.Size(900, 400);
            this.pnlPreview.TabIndex = 1;

            // 
            // btnImprimir
            // 
            this.btnImprimir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImprimir.BackColor = System.Drawing.Color.ForestGreen;
            this.btnImprimir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImprimir.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnImprimir.ForeColor = System.Drawing.Color.White;
            this.btnImprimir.Location = new System.Drawing.Point(720, 510);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(160, 45);
            this.btnImprimir.TabIndex = 3;
            this.btnImprimir.Text = "IMPRIMIR (F5)";
            this.btnImprimir.UseVisualStyleBackColor = false;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);

            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLimpiar.Location = new System.Drawing.Point(20, 510);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(120, 45);
            this.btnLimpiar.TabIndex = 4;
            this.btnLimpiar.Text = "Limpiar Cola (Esc)";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

            // 
            // EtiquetasForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 570);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.pnlPreview);
            this.Controls.Add(this.grpFiltros);
            this.Name = "EtiquetasForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generador de Etiquetas";
            this.Load += new System.EventHandler(this.EtiquetasForm_Load);
            this.grpFiltros.ResumeLayout(false);
            this.grpFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCopias)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpFiltros;
        private System.Windows.Forms.Label lblProd;
        private System.Windows.Forms.ComboBox cboProductos;
        private System.Windows.Forms.Label lblCopias;
        private System.Windows.Forms.NumericUpDown numCopias;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.CheckBox chkPrecio;
        private System.Windows.Forms.CheckBox chkNombre;
        private System.Windows.Forms.FlowLayoutPanel pnlPreview;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Button btnLimpiar;
    }
}