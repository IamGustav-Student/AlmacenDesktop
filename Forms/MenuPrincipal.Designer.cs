namespace AlmacenDesktop.Forms
{
    partial class MenuPrincipal
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuPrincipal));
            menuStrip1 = new MenuStrip();
            tsmiArchivo = new ToolStripMenuItem();
            tsmiSalir = new ToolStripMenuItem();
            tsmiGestion = new ToolStripMenuItem();
            tsmiProductos = new ToolStripMenuItem();
            tsmiClientes = new ToolStripMenuItem();
            tsmiProveedores = new ToolStripMenuItem();
            tsmiVentas = new ToolStripMenuItem();
            tsmiNuevaVenta = new ToolStripMenuItem();
            tsmiHistorial = new ToolStripMenuItem();
            tsmiAdmin = new ToolStripMenuItem();
            tsmiUsuarios = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            lblUsuarioInfo = new ToolStripStatusLabel();
            fiadoToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { tsmiArchivo, tsmiGestion, tsmiVentas, tsmiAdmin });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // tsmiArchivo
            // 
            tsmiArchivo.DropDownItems.AddRange(new ToolStripItem[] { tsmiSalir });
            tsmiArchivo.Name = "tsmiArchivo";
            tsmiArchivo.Size = new Size(60, 20);
            tsmiArchivo.Text = "Archivo";
            // 
            // tsmiSalir
            // 
            tsmiSalir.Name = "tsmiSalir";
            tsmiSalir.Size = new Size(159, 22);
            tsmiSalir.Text = "Salir del Sistema";
            tsmiSalir.Click += tsmiSalir_Click;
            // 
            // tsmiGestion
            // 
            tsmiGestion.DropDownItems.AddRange(new ToolStripItem[] { tsmiProductos, tsmiClientes, tsmiProveedores });
            tsmiGestion.Name = "tsmiGestion";
            tsmiGestion.Size = new Size(59, 20);
            tsmiGestion.Text = "Gestión";
            // 
            // tsmiProductos
            // 
            tsmiProductos.Name = "tsmiProductos";
            tsmiProductos.Size = new Size(180, 22);
            tsmiProductos.Text = "Productos";
            tsmiProductos.Click += tsmiProductos_Click;
            // 
            // tsmiClientes
            // 
            tsmiClientes.DropDownItems.AddRange(new ToolStripItem[] { fiadoToolStripMenuItem });
            tsmiClientes.Name = "tsmiClientes";
            tsmiClientes.Size = new Size(180, 22);
            tsmiClientes.Text = "Clientes";
            tsmiClientes.Click += tsmiClientes_Click;
            // 
            // tsmiProveedores
            // 
            tsmiProveedores.Name = "tsmiProveedores";
            tsmiProveedores.Size = new Size(180, 22);
            tsmiProveedores.Text = "Proveedores";
            // 
            // tsmiVentas
            // 
            tsmiVentas.DropDownItems.AddRange(new ToolStripItem[] { tsmiNuevaVenta, tsmiHistorial });
            tsmiVentas.Name = "tsmiVentas";
            tsmiVentas.Size = new Size(53, 20);
            tsmiVentas.Text = "Ventas";
            // 
            // tsmiNuevaVenta
            // 
            tsmiNuevaVenta.Name = "tsmiNuevaVenta";
            tsmiNuevaVenta.Size = new Size(140, 22);
            tsmiNuevaVenta.Text = "Nueva Venta";
            tsmiNuevaVenta.Click += tsmiNuevaVenta_Click;
            // 
            // tsmiHistorial
            // 
            tsmiHistorial.Name = "tsmiHistorial";
            tsmiHistorial.Size = new Size(140, 22);
            tsmiHistorial.Text = "Historial";
            tsmiHistorial.Click += tsmiHistorial_Click;
            // 
            // tsmiAdmin
            // 
            tsmiAdmin.DropDownItems.AddRange(new ToolStripItem[] { tsmiUsuarios });
            tsmiAdmin.Name = "tsmiAdmin";
            tsmiAdmin.Size = new Size(100, 20);
            tsmiAdmin.Text = "Administración";
            // 
            // tsmiUsuarios
            // 
            tsmiUsuarios.Name = "tsmiUsuarios";
            tsmiUsuarios.Size = new Size(178, 22);
            tsmiUsuarios.Text = "Gestión de Usuarios";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblUsuarioInfo });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 1;
            // 
            // lblUsuarioInfo
            // 
            lblUsuarioInfo.Name = "lblUsuarioInfo";
            lblUsuarioInfo.Size = new Size(122, 17);
            lblUsuarioInfo.Text = "Usuario: Desconocido";
            // 
            // fiadoToolStripMenuItem
            // 
            fiadoToolStripMenuItem.Name = "fiadoToolStripMenuItem";
            fiadoToolStripMenuItem.Size = new Size(180, 22);
            fiadoToolStripMenuItem.Text = "Fiado";
            fiadoToolStripMenuItem.Click += fiadoToolStripMenuItem_Click;
            // 
            // MenuPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(800, 450);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Name = "MenuPrincipal";
            Text = "Sistema de Gestión - AlmacenDesktop";
            WindowState = FormWindowState.Maximized;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiArchivo;
        private System.Windows.Forms.ToolStripMenuItem tsmiSalir;
        private System.Windows.Forms.ToolStripMenuItem tsmiGestion;
        private System.Windows.Forms.ToolStripMenuItem tsmiProductos;
        private System.Windows.Forms.ToolStripMenuItem tsmiClientes;
        private System.Windows.Forms.ToolStripMenuItem tsmiProveedores;
        private System.Windows.Forms.ToolStripMenuItem tsmiVentas;
        private System.Windows.Forms.ToolStripMenuItem tsmiNuevaVenta;
        private System.Windows.Forms.ToolStripMenuItem tsmiHistorial;
        private System.Windows.Forms.ToolStripMenuItem tsmiAdmin;
        private System.Windows.Forms.ToolStripMenuItem tsmiUsuarios;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblUsuarioInfo;
        private ToolStripMenuItem fiadoToolStripMenuItem;
    }
}

