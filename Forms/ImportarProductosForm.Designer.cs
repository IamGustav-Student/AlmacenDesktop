namespace AlmacenDesktop.Forms
{
    partial class ImportarProductosForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnSeleccionar = new System.Windows.Forms.Button();
            this.txtArchivo = new System.Windows.Forms.TextBox();
            this.btnDescargarPlantilla = new System.Windows.Forms.Button();
            this.btnImportar = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.panelResultados = new System.Windows.Forms.Panel();
            this.lblResumen = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.panelResultados.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitulo.Location = new System.Drawing.Point(20, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(320, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Importación Masiva Inteligente";
            // 
            // btnSeleccionar
            // 
            this.btnSeleccionar.Location = new System.Drawing.Point(450, 70);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(100, 30);
            this.btnSeleccionar.TabIndex = 1;
            this.btnSeleccionar.Text = "Seleccionar...";
            this.btnSeleccionar.UseVisualStyleBackColor = true;
            this.btnSeleccionar.Click += new System.EventHandler(this.btnSeleccionar_Click);
            // 
            // txtArchivo
            // 
            this.txtArchivo.Location = new System.Drawing.Point(25, 74);
            this.txtArchivo.Name = "txtArchivo";
            this.txtArchivo.ReadOnly = true;
            this.txtArchivo.Size = new System.Drawing.Size(419, 23);
            this.txtArchivo.TabIndex = 2;
            // 
            // btnDescargarPlantilla
            // 
            this.btnDescargarPlantilla.Location = new System.Drawing.Point(25, 120);
            this.btnDescargarPlantilla.Name = "btnDescargarPlantilla";
            this.btnDescargarPlantilla.Size = new System.Drawing.Size(180, 40);
            this.btnDescargarPlantilla.TabIndex = 3;
            this.btnDescargarPlantilla.Text = "⬇ Descargar Plantilla";
            this.btnDescargarPlantilla.UseVisualStyleBackColor = true;
            this.btnDescargarPlantilla.Click += new System.EventHandler(this.btnDescargarPlantilla_Click);
            // 
            // btnImportar
            // 
            this.btnImportar.BackColor = System.Drawing.Color.SeaGreen;
            this.btnImportar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnImportar.ForeColor = System.Drawing.Color.White;
            this.btnImportar.Location = new System.Drawing.Point(370, 120);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(180, 40);
            this.btnImportar.TabIndex = 4;
            this.btnImportar.Text = "Iniciar Importación";
            this.btnImportar.UseVisualStyleBackColor = false;
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.Location = new System.Drawing.Point(25, 180);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(525, 60);
            this.lblInfo.TabIndex = 5;
            this.lblInfo.Text = "El sistema detectará automáticamente las columnas: 'Código', 'Nombre', 'Precio', 'Stock', etc.\r\nSi el producto ya existe (por Código de Barras), se actualizarán sus datos.\r\nSi no existe, se creará uno nuevo.";
            // 
            // panelResultados
            // 
            this.panelResultados.Controls.Add(this.txtLog);
            this.panelResultados.Controls.Add(this.lblResumen);
            this.panelResultados.Location = new System.Drawing.Point(25, 250);
            this.panelResultados.Name = "panelResultados";
            this.panelResultados.Size = new System.Drawing.Size(525, 200);
            this.panelResultados.TabIndex = 6;
            this.panelResultados.Visible = false;
            // 
            // lblResumen
            // 
            this.lblResumen.AutoSize = true;
            this.lblResumen.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblResumen.Location = new System.Drawing.Point(10, 10);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(180, 19);
            this.lblResumen.TabIndex = 0;
            this.lblResumen.Text = "Resultados del Proceso:";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(10, 40);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(500, 150);
            this.txtLog.TabIndex = 1;
            // 
            // ImportarProductosForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.panelResultados);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnImportar);
            this.Controls.Add(this.btnDescargarPlantilla);
            this.Controls.Add(this.txtArchivo);
            this.Controls.Add(this.btnSeleccionar);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ImportarProductosForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importar Excel";
            this.panelResultados.ResumeLayout(false);
            this.panelResultados.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnSeleccionar;
        private System.Windows.Forms.TextBox txtArchivo;
        private System.Windows.Forms.Button btnDescargarPlantilla;
        private System.Windows.Forms.Button btnImportar;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Panel panelResultados;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.TextBox txtLog;
    }
}