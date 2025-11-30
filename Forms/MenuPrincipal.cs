using AlmacenDesktop.Modelos;
using System.Drawing; // Necesario para ImageLayout
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

            lblUsuarioInfo.Text = $"Conectado como: {_usuarioActual.NombreCompleto} ({_usuarioActual.ObtenerRol()})";

            // Seguridad: Ocultar menú admin si no corresponde
            if (_usuarioActual.NombreUsuario != "admin")
            {
                tsmiAdmin.Visible = false;
            }

            // --- CORRECCIÓN AQUÍ ---
            // Hemos eliminado la línea "tsmiHistorial.Click += tsmiHistorial_Click;"
            // porque el Designer de Visual Studio ya conecta el evento automáticamente.

            ConfigurarFondoMDI();
        }

        private void ConfigurarFondoMDI()
        {
            foreach (Control ctl in this.Controls)
            {
                if (ctl is MdiClient mdi)
                {
                    if (this.BackgroundImage != null)
                    {
                        mdi.BackgroundImage = this.BackgroundImage;
                        mdi.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    mdi.BackColor = Color.WhiteSmoke;
                    break;
                }
            }
        }

        private void tsmiSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tsmiProductos_Click(object sender, EventArgs e)
        {
            // Verificamos si ya hay una instancia abierta para no abrir múltiples
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

        private void tsmiClientes_Click(object sender, EventArgs e)
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

        private void tsmiNuevaVenta_Click(object sender, EventArgs e)
        {
            VentasForm form = new VentasForm(_usuarioActual);
            form.MdiParent = this;
            form.Show();
        }

        private void tsmiHistorial_Click(object sender, EventArgs e)
        {
            // Lógica extra de seguridad: Evitar abrir el historial mil veces
            foreach (Form f in this.MdiChildren)
            {
                if (f is HistorialVentasForm)
                {
                    f.BringToFront(); // Si ya está abierto, lo trae al frente
                    return;
                }
            }

            HistorialVentasForm form = new HistorialVentasForm();
            form.MdiParent = this;
            form.Show();
        }

        private void fiadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //// Lógica para no abrir duplicados
            //foreach (Form f in this.MdiChildren)
            //{
            //    if (f is ReporteFiadosForm)
            //    {
            //        f.BringToFront();
            //        return;
            //    }
            //}

            ReporteFiadosForm form = new ReporteFiadosForm();
            form.MdiParent = this;
            form.Show();
        }
    }
    
}
