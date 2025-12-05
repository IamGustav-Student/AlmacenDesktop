using AlmacenDesktop.Modelos;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class MenuPrincipal : Form
    {
        private Usuario _usuarioActual;

        public MenuPrincipal(Usuario usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            // Conectar evento Load
            this.Load += new EventHandler(MenuPrincipal_Load);

            lblUsuarioInfo.Text = $"Conectado como: {_usuarioActual.NombreCompleto} ({_usuarioActual.ObtenerRol()})";

            if (_usuarioActual.NombreUsuario != "admin")
            {
                tsmiAdmin.Visible = false;
            }

            ConfigurarFondoMDI();
        }

        private void MenuPrincipal_Load(object sender, EventArgs e)
        {
            MostrarDashboard();
        }

        public void MostrarDashboard() // Hacemos público por si queremos llamarlo desde fuera
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is DashboardForm)
                {
                    f.BringToFront();
                    return;
                }
            }

            DashboardForm form = new DashboardForm();
            form.MdiParent = this;
            form.Dock = DockStyle.Fill;
            form.Show();
        }

        private void ConfigurarFondoMDI()
        {
            foreach (Control ctl in this.Controls)
            {
                if (ctl is MdiClient mdi)
                {
                    mdi.BackColor = Color.FromArgb(240, 240, 240);
                    break;
                }
            }
        }

        private void tsmiSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // --- CAMBIO CLAVE: DE PRIVATE A PUBLIC ---
        public void tsmiProductos_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is ProductosForm)
                {
                    f.BringToFront();
                    return;
                }
            }

            ProductosForm form = new ProductosForm();
            form.MdiParent = this;
            form.Show();
        }

        public void tsmiClientes_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is ClientesForm)
                {
                    f.BringToFront();
                    return;
                }
            }

            ClientesForm form = new ClientesForm();
            form.MdiParent = this;
            form.Show();
        }

        // --- CAMBIO CLAVE: DE PRIVATE A PUBLIC ---
        public void tsmiNuevaVenta_Click(object sender, EventArgs e)
        {
            VentasForm form = new VentasForm(_usuarioActual);
            form.MdiParent = this;
            form.Show();
        }

        private void tsmiHistorial_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is HistorialVentasForm)
                {
                    f.BringToFront();
                    return;
                }
            }

            HistorialVentasForm form = new HistorialVentasForm();
            form.MdiParent = this;
            form.Show();
        }



        public void inicioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MostrarDashboard();
        }

        // Lo dejamos public para poder llamarlo si hiciera falta
        public void controlDeCajaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlCajaForm form = new ControlCajaForm(_usuarioActual);
            form.ShowDialog();
            MostrarDashboard();
        }

        private void configuracionPersonalizadaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfiguracionForm form = new ConfiguracionForm();
            form.MdiParent = this;
            form.Show();
        }

        private void cuentasCorrientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reemplazamos el reporte simple por el gestor completo
            CuentaCorrienteForm form = new CuentaCorrienteForm();
            form.MdiParent = this; // O form.ShowDialog() si prefieres modal
            form.Show();
        }

        private void tsmiProveedores_Click(object sender, EventArgs e)
        {
            ProveedoresForm form = new ProveedoresForm();

            form.MdiParent = this;
            form.Show();
        }

        private void comprasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComprasForm form = new ComprasForm();
            form.MdiParent = this;
            form.Show();
        }

        private void etiquetasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f is EtiquetasForm)
                {
                    f.BringToFront();
                    return;
                }
            }

            EtiquetasForm form = new EtiquetasForm();
            form.MdiParent = this;
            form.Show();
        }
    }
}