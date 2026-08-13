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
            this.panelContenido = new System.Windows.Forms.Panel();
            this.panelGrafico = new System.Windows.Forms.Panel();
            this.lblBienvenida = new System.Windows.Forms.Label();
            this.lblTituloHome = new System.Windows.Forms.Label();

            this.panelMenu.SuspendLayout();
            this.panelContenido.SuspendLayout();
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
            // panelContenido — pantalla de inicio. Orden de docking: el Fill se
            // agrega PRIMERO y los bordes después (mismo criterio que ProductosForm,
            // que es el ejemplo que ya funciona en este repo).
            //
            this.panelContenido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.panelContenido.Controls.Add(this.panelGrafico);
            this.panelContenido.Controls.Add(this.lblBienvenida);
            this.panelContenido.Controls.Add(this.lblTituloHome);
            this.panelContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenido.Name = "panelContenido";
            this.panelContenido.Padding = new System.Windows.Forms.Padding(28, 22, 28, 22);
            this.panelContenido.TabIndex = 1;

            //
            // lblTituloHome
            //
            this.lblTituloHome.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTituloHome.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTituloHome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblTituloHome.Height = 38;
            this.lblTituloHome.Name = "lblTituloHome";
            this.lblTituloHome.TabIndex = 0;
            this.lblTituloHome.Text = "Evolución del negocio";

            //
            // lblBienvenida
            //
            this.lblBienvenida.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBienvenida.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblBienvenida.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblBienvenida.Height = 30;
            this.lblBienvenida.Name = "lblBienvenida";
            this.lblBienvenida.TabIndex = 1;
            this.lblBienvenida.Text = "Bienvenido";

            //
            // panelGrafico — tarjeta blanca que aloja el gráfico de 12 meses
            //
            this.panelGrafico.BackColor = System.Drawing.Color.White;
            this.panelGrafico.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrafico.Name = "panelGrafico";
            this.panelGrafico.Padding = new System.Windows.Forms.Padding(1);
            this.panelGrafico.TabIndex = 2;

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
            this.Controls.Add(this.panelContenido);
            this.Controls.Add(this.panelMenu);
            this.Name = "MenuPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menú Principal - VENDEMAX";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MenuPrincipal_FormClosing);
            this.Load += new System.EventHandler(this.MenuPrincipal_Load);
            this.panelMenu.ResumeLayout(false);
            this.panelContenido.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.FlowLayoutPanel flowMenu;
        private System.Windows.Forms.Panel panelContenido;
        private System.Windows.Forms.Panel panelGrafico;
        private System.Windows.Forms.Label lblBienvenida;
        private System.Windows.Forms.Label lblTituloHome;
    }
}
