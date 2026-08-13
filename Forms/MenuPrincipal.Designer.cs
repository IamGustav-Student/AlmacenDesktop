namespace AlmacenDesktop.Forms
{
    partial class MenuPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        // A diferencia de la versión anterior, acá ya no se declara un botón por
        // pantalla: el menú se arma en tiempo de ejecución desde la lista de
        // ItemMenu de MenuPrincipal.cs. Así agregar una pantalla nueva es una línea
        // en esa lista y no tocar el Designer + los handlers + ConfigurarSeguridad.
        private void InitializeComponent()
        {
            this.panelMenu = new System.Windows.Forms.Panel();
            this.flowMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.lblBienvenida = new System.Windows.Forms.Label();

            this.panelMenu.SuspendLayout();
            this.SuspendLayout();

            //
            // panelMenu (barra lateral)
            //
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(29)))), ((int)(((byte)(46)))));
            this.panelMenu.Controls.Add(this.flowMenu);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(250, 500);
            this.panelMenu.TabIndex = 0;

            //
            // flowMenu — único hijo del panel lateral, así no hay ambigüedad de
            // orden de docking (Fill vs Top/Bottom entre hermanos). AutoScroll deja
            // llegar a todos los ítems aunque la pantalla sea chica.
            //
            this.flowMenu.AutoScroll = true;
            this.flowMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowMenu.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowMenu.Name = "flowMenu";
            this.flowMenu.Padding = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.flowMenu.TabIndex = 0;
            this.flowMenu.WrapContents = false;

            //
            // lblBienvenida
            //
            this.lblBienvenida.AutoSize = true;
            this.lblBienvenida.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblBienvenida.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblBienvenida.Location = new System.Drawing.Point(275, 25);
            this.lblBienvenida.Name = "lblBienvenida";
            this.lblBienvenida.Size = new System.Drawing.Size(109, 25);
            this.lblBienvenida.TabIndex = 1;
            this.lblBienvenida.Text = "Bienvenido";

            //
            // MenuPrincipal
            //
            // AutoScaleDimensions + AutoScaleMode.Font es la combinación canónica de
            // WinForms y acá está bien configurada — no migrar a None (ver docs/CONTEXTO.md).
            // El menú usa Dock/FlowLayoutPanel, que es agnóstico al modo de escalado.
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.lblBienvenida);
            this.Controls.Add(this.panelMenu);
            this.Name = "MenuPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menú Principal - VENDEMAX";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MenuPrincipal_FormClosing);
            this.Load += new System.EventHandler(this.MenuPrincipal_Load);
            this.panelMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.FlowLayoutPanel flowMenu;
        private System.Windows.Forms.Label lblBienvenida;
    }
}
