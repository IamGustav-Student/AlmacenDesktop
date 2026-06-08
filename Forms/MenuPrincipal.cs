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

            // AGREGAR GUÍA DE ADMINISTRACIÓN DE USUARIOS
            if (_usuarioActual.Rol == RolUsuario.Admin)
            {
                var grpGuia = new GroupBox
                {
                    Text = "📘 Guía de Inicio: Configurar Usuarios del Negocio",
                    Location = new Point(240, 70),
                    Size = new Size(520, 160),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 122, 204),
                    BackColor = Color.White
                };

                var lblInstrucciones = new Label
                {
                    Text = "¡Bienvenido a VendeMax!\n\n" +
                           "Para configurar su comercio, le sugerimos seguir estos pasos:\n" +
                           "1. Crear Administradores: Vaya al botón '👥 Usuarios' y registre a los socios con rol 'Admin'.\n" +
                           "2. Crear Empleados / Cajeros: Registre a su personal de atención con el rol 'Vendedor'.\n" +
                           "   (El sistema limitará su acceso protegiendo la caja, importaciones y reportes sensibles).\n" +
                           "3. Seguridad: Recuerde asignar contraseñas seguras a cada cuenta registrada.",
                    Location = new Point(15, 25),
                    Size = new Size(490, 120),
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.Black
                };

                grpGuia.Controls.Add(lblInstrucciones);
                this.Controls.Add(grpGuia);
            }
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

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            // Cargar el formulario de Clientes y cuentas corrientes (Fiados)
            AbrirFormulario(new ClientesForm());
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
