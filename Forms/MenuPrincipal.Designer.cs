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
            menuStrip1 = new MenuStrip();
            tsmiArchivo = new ToolStripMenuItem();
            tsmiSalir = new ToolStripMenuItem();
            tsmiGestion = new ToolStripMenuItem();
            tsmiProductos = new ToolStripMenuItem();
            tsmiClientes = new ToolStripMenuItem();
            tsmiProveedores = new ToolStripMenuItem();
            comprasToolStripMenuItem = new ToolStripMenuItem();
            etiquetasToolStripMenuItem = new ToolStripMenuItem();
            tsmiReporteGanancias = new ToolStripMenuItem();
            tsmiVentas = new ToolStripMenuItem();
            tsmiNuevaVenta = new ToolStripMenuItem();
            tsmiHistorial = new ToolStripMenuItem();
            controlDeCajaToolStripMenuItem = new ToolStripMenuItem();
            configuracionPersonalizadaToolStripMenuItem = new ToolStripMenuItem();
            cuentasCorrientesToolStripMenuItem = new ToolStripMenuItem();
            tsmiAdmin = new ToolStripMenuItem();
            tsmiUsuarios = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            lblUsuarioInfo = new ToolStripStatusLabel();
            tsmiHistorialCajas = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { tsmiArchivo, tsmiGestion, tsmiVentas, controlDeCajaToolStripMenuItem, configuracionPersonalizadaToolStripMenuItem, cuentasCorrientesToolStripMenuItem, tsmiAdmin });
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
            tsmiSalir.Size = new Size(180, 22);
            tsmiSalir.Text = "Salir del Sistema";
            tsmiSalir.Click += tsmiSalir_Click;
            // 
            // tsmiGestion
            // 
            tsmiGestion.DropDownItems.AddRange(new ToolStripItem[] { tsmiProductos, tsmiClientes, tsmiProveedores, comprasToolStripMenuItem, etiquetasToolStripMenuItem, tsmiReporteGanancias, tsmiHistorialCajas });
            tsmiGestion.Name = "tsmiGestion";
            tsmiGestion.Size = new Size(59, 20);
            tsmiGestion.Text = "Gestión";
            // 
            // tsmiProductos
            // 
            tsmiProductos.Name = "tsmiProductos";
            tsmiProductos.Size = new Size(182, 22);
            tsmiProductos.Text = "Productos";
            tsmiProductos.Click += tsmiProductos_Click;
            // 
            // tsmiClientes
            // 
            tsmiClientes.Name = "tsmiClientes";
            tsmiClientes.Size = new Size(182, 22);
            tsmiClientes.Text = "Clientes";
            tsmiClientes.Click += tsmiClientes_Click;
            // 
            // tsmiProveedores
            // 
            tsmiProveedores.Name = "tsmiProveedores";
            tsmiProveedores.Size = new Size(182, 22);
            tsmiProveedores.Text = "Proveedores";
            tsmiProveedores.Click += tsmiProveedores_Click;
            // 
            // comprasToolStripMenuItem
            // 
            comprasToolStripMenuItem.Name = "comprasToolStripMenuItem";
            comprasToolStripMenuItem.Size = new Size(182, 22);
            comprasToolStripMenuItem.Text = "Compras";
            comprasToolStripMenuItem.Click += comprasToolStripMenuItem_Click;
            // 
            // etiquetasToolStripMenuItem
            // 
            etiquetasToolStripMenuItem.Name = "etiquetasToolStripMenuItem";
            etiquetasToolStripMenuItem.Size = new Size(182, 22);
            etiquetasToolStripMenuItem.Text = "Etiquetas";
            etiquetasToolStripMenuItem.Click += etiquetasToolStripMenuItem_Click;
            // 
            // tsmiReporteGanancias
            // 
            tsmiReporteGanancias.Name = "tsmiReporteGanancias";
            tsmiReporteGanancias.Size = new Size(182, 22);
            tsmiReporteGanancias.Text = "Report de Ganancias";
            tsmiReporteGanancias.Click += tsmiReporteGanancias_Click;
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
            // controlDeCajaToolStripMenuItem
            // 
            controlDeCajaToolStripMenuItem.Name = "controlDeCajaToolStripMenuItem";
            controlDeCajaToolStripMenuItem.Size = new Size(102, 20);
            controlDeCajaToolStripMenuItem.Text = "Control De Caja";
            controlDeCajaToolStripMenuItem.Click += controlDeCajaToolStripMenuItem_Click;
            // 
            // configuracionPersonalizadaToolStripMenuItem
            // 
            configuracionPersonalizadaToolStripMenuItem.Name = "configuracionPersonalizadaToolStripMenuItem";
            configuracionPersonalizadaToolStripMenuItem.Size = new Size(170, 20);
            configuracionPersonalizadaToolStripMenuItem.Text = "Configuracion Personalizada";
            configuracionPersonalizadaToolStripMenuItem.Click += configuracionPersonalizadaToolStripMenuItem_Click;
            // 
            // cuentasCorrientesToolStripMenuItem
            // 
            cuentasCorrientesToolStripMenuItem.Name = "cuentasCorrientesToolStripMenuItem";
            cuentasCorrientesToolStripMenuItem.Size = new Size(119, 20);
            cuentasCorrientesToolStripMenuItem.Text = "Cuentas Corrientes";
            cuentasCorrientesToolStripMenuItem.Click += cuentasCorrientesToolStripMenuItem_Click;
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
            // tsmiHistorialCajas
            // 
            tsmiHistorialCajas.Name = "tsmiHistorialCajas";
            tsmiHistorialCajas.Size = new Size(182, 22);
            tsmiHistorialCajas.Text = "Historial de Caja";
            tsmiHistorialCajas.Click += tsmiHistorialCajas_Click;
            // 
            // MenuPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
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
        private ToolStripMenuItem controlDeCajaToolStripMenuItem;
        private ToolStripMenuItem configuracionPersonalizadaToolStripMenuItem;
        private ToolStripMenuItem cuentasCorrientesToolStripMenuItem;
        private ToolStripMenuItem comprasToolStripMenuItem;
        private ToolStripMenuItem etiquetasToolStripMenuItem;
        private ToolStripMenuItem tsmiReporteGanancias;
        private ToolStripMenuItem tsmiHistorialCajas;
    }
}

