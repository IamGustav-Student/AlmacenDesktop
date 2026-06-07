using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services; // Para BackupService si lo llamas manualmente
using System;
using System.Windows.Forms;
using System.Drawing;

namespace AlmacenDesktop.Forms
{
    public partial class MenuPrincipal : Form
    {
        private Usuario _usuarioActual;

        public MenuPrincipal(Usuario usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;
        }

        private void MenuPrincipal_Load(object sender, EventArgs e)
        {
            ConfigurarSeguridad();
            lblBienvenida.Text = $"Hola, {_usuarioActual.Nombre} ({_usuarioActual.Rol})";
        }

        private void ConfigurarSeguridad()
        {
            // L�gica de Permisos "Nivel Dios"
            // Si es ADMIN, no tocamos nada (tiene acceso a todo).
            // Si NO es Admin, empezamos a restringir.

            if (_usuarioActual.Rol != RolUsuario.Admin)
            {
                // 1. Bloquear Configuraci�n (CR�TICO)
                BloquearBoton(btnConfiguracion);
                BloquearBoton(btnUsuarios); // Gesti�n de usuarios solo para admins
                BloquearBoton(btnProveedores); // Opcional, seg�n tu negocio

                // 2. Bloquear Reportes Sensibles (Ganancias)
                BloquearBoton(btnReportes);

                // 3. Bloquear Gesti�n Avanzada de Stock (Importaci�n masiva, etc)
                // btnProductos.Enabled = true; // Vendedores suelen necesitar ver productos
                BloquearBoton(btnImportar); // Importar Excel es peligroso para un vendedor
            }

            // Si tienes un rol intermedio "Gerente", puedes agregar un 'else if' aqu�.
        }

        // M�todo auxiliar para desactivar visualmente un bot�n
        private void BloquearBoton(Button btn)
        {
            if (btn != null)
            {
                btn.Enabled = false;
                btn.BackColor = Color.LightGray;
                btn.ForeColor = Color.DarkGray;
                // Opcional: Ocultarlo del todo
                // btn.Visible = false; 
            }
        }

        // --- EVENTOS DE NAVEGACI�N (Ejemplos) ---

        private void btnVentas_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new VentasForm(_usuarioActual));
        }

        private void btnCaja_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new ControlCajaForm(_usuarioActual));
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            // Todos pueden ver productos, pero quiz�s quieras restringir editar/borrar dentro del form
            AbrirFormulario(new ProductosForm());
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            // Doble chequeo de seguridad
            if (!ValidarAccesoAdmin()) return;
            AbrirFormulario(new ConfiguracionForm());
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            if (!ValidarAccesoAdmin()) return;
            AbrirFormulario(new ReporteGananciasForm());
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            if (!ValidarAccesoAdmin()) return;
            AbrirFormulario(new UsuariosForm());
        }

        // --- HELPER DE NAVEGACIN ---
        private void AbrirFormulario(Form formulario)
        {
            this.Hide();
            formulario.ShowDialog();
            this.Show(); // Al cerrar el hijo, vuelve el men
        }

        private bool ValidarAccesoAdmin()
        {
            if (_usuarioActual.Rol != RolUsuario.Admin)
            {
                MessageBox.Show("⛔ Acceso Denegado.\nSe requieren permisos de Administrador.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            if (!ValidarAccesoAdmin()) return;
            AbrirFormulario(new ImportarProductosForm());
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MenuPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
