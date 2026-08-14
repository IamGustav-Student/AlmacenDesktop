using AlmacenDesktop.Data;
using AlmacenDesktop.Helpers;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
            // Crear abre la pantalla en una ventana modal (el caso normal).
            // Accion es para las que viven embebidas en esta misma ventana.
            public Func<Form> Crear { get; init; }
            public Action Accion { get; init; }
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

            // En una instalación recién estrenada no hay historial que graficar, así
            // que ese espacio lo aprovecha la guía de inicio. Son excluyentes a
            // propósito: evita además que la carga async del gráfico le pise el panel.
            if (!MostrarGuiaSiCorresponde())
            {
                _ = CargarEvolucionAsync();
            }

            _ = ChequearActualizacionesAsync();
        }

        // --- PANTALLA DE INICIO: EVOLUCIÓN MES A MES ---

        private const int MesesHistorial = 12;

        private sealed class VentaMes
        {
            public DateTime Mes { get; set; }
            public decimal Total { get; set; }
            public decimal Ganancia { get; set; }
        }

        private async Task CargarEvolucionAsync()
        {
            MostrarMensajeGrafico("Cargando evolución del negocio…");
            try
            {
                var datos = await Task.Run(() => ObtenerVentasPorMes(MesesHistorial));
                if (this.IsDisposed || panelGrafico.IsDisposed) return;

                if (datos.All(d => d.Total == 0))
                {
                    MostrarMensajeGrafico("Todavía no hay ventas registradas.\nCuando empieces a vender vas a ver acá la evolución mes a mes.");
                    return;
                }

                DibujarEvolucion(datos);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error cargando evolución: " + ExceptionHelper.ObtenerMensaje(ex));
                MostrarMensajeGrafico("No se pudo cargar la evolución del negocio.");
            }
        }

        /// <summary>
        /// Ventas y ganancia estimada agrupadas por mes, incluyendo los meses sin
        /// ventas (si se omitieran, el gráfico saltearía huecos y mentiría sobre
        /// la evolución real).
        /// </summary>
        private List<VentaMes> ObtenerVentasPorMes(int meses)
        {
            var primerMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-(meses - 1));

            var resultado = new List<VentaMes>();
            for (int i = 0; i < meses; i++)
                resultado.Add(new VentaMes { Mes = primerMes.AddMonths(i) });

            using (var context = new AlmacenDbContext())
            {
                var ventas = context.Ventas
                    .Where(v => v.Fecha >= primerMes)
                    .Select(v => new { v.Fecha, v.Total })
                    .ToList();

                foreach (var v in ventas)
                {
                    var bucket = resultado.FirstOrDefault(x => x.Mes.Year == v.Fecha.Year && x.Mes.Month == v.Fecha.Month);
                    if (bucket != null) bucket.Total += v.Total;
                }

                // Ganancia ESTIMADA: usa el costo actual del producto, no el costo al
                // momento de la venta — el esquema no guarda un snapshot del costo en
                // DetalleVenta. Mismo criterio que ya usa el Resumen del Negocio, así
                // que ambas pantallas dan el mismo número.
                var detalles = context.DetallesVenta
                    .Include(d => d.Producto)
                    .Include(d => d.Venta)
                    .Where(d => d.Venta.Fecha >= primerMes)
                    .Select(d => new { d.Venta.Fecha, d.PrecioUnitario, d.Cantidad, d.Producto.Costo })
                    .ToList();

                foreach (var d in detalles)
                {
                    var bucket = resultado.FirstOrDefault(x => x.Mes.Year == d.Fecha.Year && x.Mes.Month == d.Fecha.Month);
                    if (bucket != null) bucket.Ganancia += (d.PrecioUnitario - d.Costo) * d.Cantidad;
                }
            }

            return resultado;
        }

        private void DibujarEvolucion(List<VentaMes> datos)
        {
            panelGrafico.Controls.Clear();

            var chart = new Chart { Dock = DockStyle.Fill, BackColor = Color.White };

            var area = new ChartArea("Evolucion");
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.LineColor = Color.FromArgb(203, 213, 225);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8.5F);
            area.AxisX.Interval = 1;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 238, 242);
            area.AxisY.LineColor = Color.FromArgb(203, 213, 225);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8.5F);
            area.AxisY.LabelStyle.Format = "C0";
            chart.ChartAreas.Add(area);

            var serieVentas = new Series("Ventas")
            {
                ChartType = SeriesChartType.Column,
                Color = ColorPrimario,
                BorderWidth = 0
            };
            var serieGanancia = new Series("Ganancia estimada")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(46, 125, 50),
                BorderWidth = 3,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7
            };

            var cultura = new CultureInfo("es-AR");
            foreach (var d in datos)
            {
                string etiqueta = d.Mes.ToString("MMM yy", cultura);
                serieVentas.Points.AddXY(etiqueta, (double)d.Total);
                serieGanancia.Points.AddXY(etiqueta, (double)d.Ganancia);
            }

            chart.Series.Add(serieVentas);
            chart.Series.Add(serieGanancia);
            chart.Legends.Add(new Legend("Leyenda")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Near,
                Font = new Font("Segoe UI", 9F),
                BorderColor = Color.Transparent
            });

            chart.Titles.Add(new Title(
                $"Ventas y ganancia — últimos {MesesHistorial} meses",
                Docking.Top,
                new Font("Segoe UI", 11F, FontStyle.Bold),
                Color.FromArgb(30, 41, 59))
            { Alignment = ContentAlignment.TopLeft });

            panelGrafico.Controls.Add(chart);
        }

        private void MostrarMensajeGrafico(string mensaje)
        {
            if (panelGrafico.IsDisposed) return;
            panelGrafico.Controls.Clear();
            panelGrafico.Controls.Add(new Label
            {
                Text = mensaje,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11F),
                ForeColor = ColorTextoSuave
            });
        }

        // --- CONSTRUCCIÓN DEL MENÚ ---

        private List<ItemMenu> ObtenerItems()
        {
            return new List<ItemMenu>
            {
                // Operación diaria
                // La venta va embebida en esta misma ventana (no en una aparte), así el
                // cajero no salta entre ventanas durante el turno.
                new ItemMenu { Grupo = "OPERACIÓN DIARIA", Texto = "🛒  Nueva Venta", Destacado = true, Accion = MostrarVenta },
                new ItemMenu { Grupo = "OPERACIÓN DIARIA", Texto = "🏠  Inicio / Resumen", Accion = MostrarInicio },
                new ItemMenu { Grupo = "OPERACIÓN DIARIA", Texto = "💵  Caja Diaria", Crear = () => new ControlCajaForm(_usuarioActual) },
                new ItemMenu { Grupo = "OPERACIÓN DIARIA", Texto = "🧾  Historial de Ventas", Crear = () => new HistorialVentasForm() },
                new ItemMenu { Grupo = "OPERACIÓN DIARIA", Texto = "📒  Historial de Cajas", Crear = () => new HistorialCajasForm() },

                // Clientes
                new ItemMenu { Grupo = "CLIENTES", Texto = "👥  Clientes / Cta. Cte.", Crear = () => new ClientesForm() },
                new ItemMenu { Grupo = "CLIENTES", Texto = "📋  Reporte de Cuenta Corriente", Crear = () => new ReporteFiadosForm() },

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
                if (item.Accion != null) item.Accion();
                else AbrirFormulario(item.Crear());
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show(
                    $"No se pudo abrir «{item.Texto.Trim()}».\n\n{ExceptionHelper.ObtenerMensaje(ex)}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- PANTALLA DE VENTA EMBEBIDA ---

        private VentasForm _ventaEmbebida;
        private int? _cajaDeLaVentaEmbebida;
        private bool _arranqueHecho;

        // Al arrancar el software se entra derecho a vender. Si todavía no hay turno
        // abierto se abre primero la caja, que es el orden natural de la mañana en un
        // comercio; recién con la caja abierta tiene sentido mostrar la venta.
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // AbrirFormulario hace Hide()/Show(), lo que puede volver a disparar Shown.
            if (_arranqueHecho) return;
            _arranqueHecho = true;

            try
            {
                MostrarVenta(explicarFaltaDeCaja: false);
            }
            catch (Exception ex)
            {
                // Que falle el arranque directo en ventas no puede dejar la app inusable:
                // en el peor caso queda el resumen y el menú, como antes.
                AudioHelper.PlayError();
                MessageBox.Show(
                    "No se pudo abrir la pantalla de venta al iniciar.\n\n" + ExceptionHelper.ObtenerMensaje(ex),
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MostrarInicio();
            }
        }

        private void MostrarVenta() => MostrarVenta(explicarFaltaDeCaja: true);

        private void MostrarVenta(bool explicarFaltaDeCaja)
        {
            var ventaService = new VentaService();
            int? cajaId = ventaService.ObtenerCajaAbiertaId(_usuarioActual.Id);

            // Se valida acá y no se delega a VentasForm: embebida, si se autocerrara por
            // falta de caja dejaría el panel en blanco sin explicación.
            if (cajaId == null)
            {
                if (explicarFaltaDeCaja)
                {
                    MessageBox.Show(
                        "Para vender primero hay que abrir la caja del turno.\n\n" +
                        "Se va a abrir «Caja Diaria» para cargar el saldo inicial.",
                        "Caja Cerrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                AbrirFormulario(new ControlCajaForm(_usuarioActual));

                cajaId = ventaService.ObtenerCajaAbiertaId(_usuarioActual.Id);
                if (cajaId == null)
                {
                    // Salió sin abrir el turno: queda en el resumen.
                    MostrarInicio();
                    return;
                }
            }

            // Si cambió el turno (se cerró la caja y se abrió otra) hay que rearmar la
            // pantalla: VentasForm cachea el id de caja al cargar, y seguir con la
            // instancia vieja registraría las ventas en un turno ya cerrado.
            if (_ventaEmbebida != null && (_ventaEmbebida.IsDisposed || _cajaDeLaVentaEmbebida != cajaId))
            {
                if (!_ventaEmbebida.IsDisposed) _ventaEmbebida.Dispose();
                _ventaEmbebida = null;
            }

            if (_ventaEmbebida == null)
            {
                var venta = new VentasForm(_usuarioActual);

                // TopLevel=false convierte el formulario en un control más: se puede
                // meter dentro de un panel sin reescribir su layout ni su lógica.
                venta.TopLevel = false;
                venta.FormBorderStyle = FormBorderStyle.None;
                venta.Dock = DockStyle.Fill;

                panelVenta.Controls.Clear();
                panelVenta.Controls.Add(venta);
                venta.Show();

                _ventaEmbebida = venta;
                _cajaDeLaVentaEmbebida = cajaId;
            }

            panelContenido.Visible = false;
            panelVenta.Visible = true;
            _ventaEmbebida.Focus();
        }

        private void MostrarInicio()
        {
            panelVenta.Visible = false;
            panelContenido.Visible = true;
        }

        // --- GUÍA DE INICIO ---

        // Solo tiene sentido mientras el comercio no configuró usuarios todavía;
        // una vez que hay equipo cargado, ocupa lugar sin aportar nada.
        // Devuelve true si efectivamente mostró la guía.
        private bool MostrarGuiaSiCorresponde()
        {
            if (_usuarioActual.Rol != RolUsuario.Admin) return false;

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    if (context.Usuarios.Count() > 1) return false;
                }
            }
            catch
            {
                return false; // Si no se puede leer la base, simplemente no mostramos la guía.
            }

            lblTituloHome.Text = "Primeros pasos";

            var lblInstrucciones = new Label
            {
                Text = "¡Bienvenido a VENDEMAX!\n\n" +
                       "Para configurar su comercio, le sugerimos seguir estos pasos:\n\n" +
                       "1.  Crear Administradores: vaya a «Usuarios» y registre a los socios con rol Admin.\n\n" +
                       "2.  Crear Empleados / Cajeros: registre a su personal con el rol Vendedor.\n" +
                       "     El sistema limitará su acceso protegiendo la caja, importaciones y reportes.\n\n" +
                       "3.  Seguridad: asigne contraseñas seguras a cada cuenta registrada.\n\n" +
                       "Cuando empiece a vender, acá va a ver la evolución mes a mes de su negocio.",
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 25, 30, 25),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(30, 41, 59)
            };

            panelGrafico.Controls.Clear();
            panelGrafico.Controls.Add(lblInstrucciones);
            return true;
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
