using AlmacenDesktop.Data;
using AlmacenDesktop.Helpers;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class MenuPrincipal : Form
    {
        private Usuario _usuarioActual;

        // Colores del menú. Provisorios: en la Fase 2 del roadmap estos pasan a
        // Helpers/Theme.cs junto con el resto de la app (ver docs/CONTEXTO.md).
        private static readonly Color ColorTextoSuave = Color.FromArgb(148, 163, 184);
        private static readonly Color ColorBotonMenu = Color.FromArgb(30, 45, 69);
        private static readonly Color ColorPrimario = Color.FromArgb(0, 122, 204);
        private static readonly Color ColorPeligro = Color.FromArgb(198, 40, 40);

        /// <summary>
        /// Una entrada del menú lateral. Agregar una pantalla nueva es agregar un
        /// item acá — no hay que tocar el Designer ni escribir un handler nuevo.
        /// </summary>
        private sealed class ItemMenu
        {
            public string Grupo { get; init; } = "";
            public string Texto { get; init; } = "";
            public bool SoloAdmin { get; init; }
            public Func<Form> Crear { get; init; } = null!;
            public bool Destacado { get; init; }
        }

        public MenuPrincipal(Usuario usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;
        }

        private void MenuPrincipal_Load(object sender, EventArgs e)
        {
            lblBienvenida.Text = $"Hola, {_usuarioActual.Nombre} ({_usuarioActual.Rol})";

            ConstruirMenu();
            MostrarGuiaSiCorresponde();

            // Chequeo de actualizaciones en segundo plano — no bloquea la apertura del menú.
            _ = ChequearActualizacionesAsync();
        }

        // --- CONSTRUCCIÓN DEL MENÚ ---

        private List<ItemMenu> ObtenerItems()
        {
            return new List<ItemMenu>
            {
                // Operación diaria
                new ItemMenu { Grupo = "OPERACIÓN DIARIA", Texto = "🛒  Nueva Venta", Destacado = true, Crear = () => new VentasForm(_usuarioActual) },
                new ItemMenu { Grupo = "OPERACIÓN DIARIA", Texto = "💵  Caja Diaria", Crear = () => new ControlCajaForm(_usuarioActual) },
                new ItemMenu { Grupo = "OPERACIÓN DIARIA", Texto = "🧾  Historial de Ventas", Crear = () => new HistorialVentasForm() },
                new ItemMenu { Grupo = "OPERACIÓN DIARIA", Texto = "📒  Historial de Cajas", Crear = () => new HistorialCajasForm() },

                // Clientes
                new ItemMenu { Grupo = "CLIENTES", Texto = "👥  Clientes / Fiados", Crear = () => new ClientesForm() },
                new ItemMenu { Grupo = "CLIENTES", Texto = "📋  Reporte de Fiados", Crear = () => new ReporteFiadosForm() },

                // Inventario
                new ItemMenu { Grupo = "INVENTARIO", Texto = "📦  Productos", Crear = () => new ProductosForm() },
                new ItemMenu { Grupo = "INVENTARIO", Texto = "🚚  Compras a Proveedores", Crear = () => new ComprasForm(_usuarioActual) },
                new ItemMenu { Grupo = "INVENTARIO", Texto = "🏭  Proveedores", Crear = () => new ProveedoresForm() },
                new ItemMenu { Grupo = "INVENTARIO", Texto = "🏷️  Etiquetas", Crear = () => new EtiquetasForm() },
                new ItemMenu { Grupo = "INVENTARIO", Texto = "📥  Importar Excel", SoloAdmin = true, Crear = () => new ImportarProductosForm() },

                // Reportes
                new ItemMenu { Grupo = "REPORTES", Texto = "📊  Resumen del Negocio", Crear = () => new DashboardForm() },
                new ItemMenu { Grupo = "REPORTES", Texto = "📈  Ganancias", SoloAdmin = true, Crear = () => new ReporteGananciasForm() },

                // Administración
                new ItemMenu { Grupo = "ADMINISTRACIÓN", Texto = "🧑‍💼  Usuarios", SoloAdmin = true, Crear = () => new UsuariosForm() },
                new ItemMenu { Grupo = "ADMINISTRACIÓN", Texto = "⚙️  Configuración", SoloAdmin = true, Crear = () => new ConfiguracionForm() },
            };
        }

        private void ConstruirMenu()
        {
            bool esAdmin = _usuarioActual.Rol == RolUsuario.Admin;

            // Ancho del ítem = ancho útil - márgenes horizontales (12 izq + 10 der)
            // - el ancho de la barra vertical. Ese último descuento va siempre, aunque
            // todavía no se vea la barra: si no, al aparecer (pantalla baja) reduce el
            // área útil y dispara además una barra horizontal molesta.
            int anchoItem = flowMenu.ClientSize.Width - 22 - SystemInformation.VerticalScrollBarWidth;
            if (anchoItem < 150) anchoItem = 150;

            flowMenu.SuspendLayout();
            flowMenu.Controls.Clear();

            flowMenu.Controls.Add(CrearTitulo(anchoItem));

            string grupoActual = "";
            foreach (var item in ObtenerItems())
            {
                // Los ítems de admin se ocultan, no se muestran en gris: para un
                // usuario no técnico una pared de botones muertos confunde más de
                // lo que informa. ValidarAccesoAdmin() sigue como segunda barrera.
                if (item.SoloAdmin && !esAdmin) continue;

                if (item.Grupo != grupoActual)
                {
                    grupoActual = item.Grupo;
                    flowMenu.Controls.Add(CrearEncabezadoGrupo(grupoActual, anchoItem));
                }

                flowMenu.Controls.Add(CrearBoton(item, anchoItem));
            }

            flowMenu.Controls.Add(CrearBotonSalir(anchoItem));
            flowMenu.ResumeLayout();
        }

        private Label CrearTitulo(int ancho)
        {
            return new Label
            {
                Text = "VENDEMAX",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(ancho, 55),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(12, 15, 10, 5)
            };
        }

        private Label CrearEncabezadoGrupo(string texto, int ancho)
        {
            return new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = ColorTextoSuave,
                AutoSize = false,
                Size = new Size(ancho, 22),
                TextAlign = ContentAlignment.BottomLeft,
                Margin = new Padding(14, 12, 10, 2)
            };
        }

        private Button CrearBoton(ItemMenu item, int ancho)
        {
            var btn = new Button
            {
                Text = item.Texto,
                Tag = item,
                BackColor = item.Destacado ? ColorPrimario : ColorBotonMenu,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", item.Destacado ? 11F : 10F, item.Destacado ? FontStyle.Bold : FontStyle.Regular),
                Size = new Size(ancho, item.Destacado ? 46 : 36),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Margin = new Padding(12, 2, 10, 2),
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += ItemMenu_Click;
            return btn;
        }

        private Button CrearBotonSalir(int ancho)
        {
            var btn = new Button
            {
                Text = "🚪  Salir",
                BackColor = ColorPeligro,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Size = new Size(ancho, 36),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Margin = new Padding(12, 20, 10, 5),
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => Application.Exit();
            return btn;
        }

        private void ItemMenu_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is ItemMenu item)) return;

            if (item.SoloAdmin && !ValidarAccesoAdmin()) return;

            try
            {
                AbrirFormulario(item.Crear());
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show(
                    $"No se pudo abrir «{item.Texto.Trim()}».\n\n{ExceptionHelper.ObtenerMensaje(ex)}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- GUÍA DE INICIO ---

        // Solo tiene sentido mientras el comercio no configuró usuarios todavía;
        // una vez que hay equipo cargado, ocupa lugar sin aportar nada.
        private void MostrarGuiaSiCorresponde()
        {
            if (_usuarioActual.Rol != RolUsuario.Admin) return;

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    if (context.Usuarios.Count() > 1) return;
                }
            }
            catch
            {
                return; // Si no se puede leer la base, simplemente no mostramos la guía.
            }

            var grpGuia = new GroupBox
            {
                Text = "📘 Guía de Inicio: Configurar Usuarios del Negocio",
                Location = new Point(275, 75),
                Size = new Size(520, 160),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = ColorPrimario,
                BackColor = Color.White
            };

            var lblInstrucciones = new Label
            {
                Text = "¡Bienvenido a VENDEMAX!\n\n" +
                       "Para configurar su comercio, le sugerimos seguir estos pasos:\n" +
                       "1. Crear Administradores: vaya a 'Usuarios' y registre a los socios con rol 'Admin'.\n" +
                       "2. Crear Empleados / Cajeros: registre a su personal con el rol 'Vendedor'.\n" +
                       "   (El sistema limitará su acceso protegiendo la caja, importaciones y reportes).\n" +
                       "3. Seguridad: asigne contraseñas seguras a cada cuenta registrada.",
                Location = new Point(15, 25),
                Size = new Size(490, 120),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Black
            };

            grpGuia.Controls.Add(lblInstrucciones);
            this.Controls.Add(grpGuia);
        }

        private async Task ChequearActualizacionesAsync()
        {
            try
            {
                var updateService = new UpdateService();
                var info = await updateService.BuscarActualizacionAsync();
                if (info != null && !this.IsDisposed)
                {
                    using (var dlg = new ActualizacionForm(info))
                    {
                        dlg.ShowDialog(this);
                    }
                }
            }
            catch
            {
                // Falla silenciosa — sin conexión, GitHub caído, etc. No molesta al usuario.
            }
        }

        // --- NAVEGACIÓN ---

        private void AbrirFormulario(Form formulario)
        {
            this.Hide();
            formulario.ShowDialog();
            this.Show();
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

        private void MenuPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
