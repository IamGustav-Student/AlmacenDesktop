using AlmacenDesktop.Data;
using AlmacenDesktop.Forms;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Helpers;
using AlmacenDesktop.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace AlmacenDesktop
{
    internal static class Program
    {
        public static Usuario UsuarioActualGlobal;
        public static string ConnectionStringGlobal;
        public static IServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            // 1. MANEJO GLOBAL DE ERRORES (Anti-Crash)
            Application.ThreadException += new ThreadExceptionEventHandler(GlobalThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalUnhandledException);

            ApplicationConfiguration.Initialize();

            try
            {
                ConfigurarServicios();
                InicializarBaseDeDatos();

                // 1. CHEQUEO DE LICENCIA Y SUSCRIPCIÃ“N (HEXASTRATEGY)
                var licencia = LicenseHelper.LeerLicenciaLocal();
                if (licencia == null)
                {
                    using (var activationForm = new ActivationForm())
                    {
                        if (activationForm.ShowDialog() != DialogResult.OK)
                        {
                            return; // CancelÃ³ activaciÃ³n, salir de la app
                        }
                    }
                }
                else
                {
                    var (validoLocal, mensajeLocal) = LicenseHelper.ValidarLicenciaLocal();
                    if (!validoLocal)
                    {
                        using (var lockForm = new LockForm(mensajeLocal))
                        {
                            if (lockForm.ShowDialog() != DialogResult.OK)
                            {
                                return; // FallÃ³ revalidaciÃ³n, salir de la app
                            }
                        }
                    }
                    else
                    {
                        // SincronizaciÃ³n silenciosa de fondo
                        System.Threading.Tasks.Task.Run(async () =>
                        {
                            try
                            {
                                var licenseService = new LicenseService();
                                await licenseService.ValidarOnlineAsync(licencia.Email, licencia.Clave);
                            }
                            catch
                            {
                                // Falla silenciosa de red, sigue usando perÃ­odo offline
                            }
                        });
                    }
                }

                Usuario usuarioAuto = null;
                using (var context = new AlmacenDbContext())
                {
                    usuarioAuto = context.Usuarios.FirstOrDefault(u => u.NombreUsuario == "admin");
                }

                if (usuarioAuto != null)
                {
                    UsuarioActualGlobal = usuarioAuto;
                    MenuPrincipal menu = new MenuPrincipal(usuarioAuto);
                    Application.Run(menu);
                }
                else
                {
                    var login = ServiceProvider.GetRequiredService<LoginForm>();
                    if (login.ShowDialog() == DialogResult.OK)
                    {
                        UsuarioActualGlobal = login.UsuarioLogueado;
                        MenuPrincipal menu = new MenuPrincipal(login.UsuarioLogueado);
                        Application.Run(menu);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_error.txt"), ex.ToString());
                }
                catch {}
                MessageBox.Show($"Error Fatal en el inicio: {ex.Message}", "Error Crtico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 2. BACKUP AL CERRAR (Safety First)
                if (UsuarioActualGlobal != null) // Solo si hubo login exitoso
                {
                    var backupService = new BackupService();
                    backupService.RealizarBackupAutomatico();
                }
            }
        }

        // Manejador de excepciones UI (WinForms)
        private static void GlobalThreadException(object sender, ThreadExceptionEventArgs e)
        {
            AudioHelper.PlayError();
            MessageBox.Show($"Ocurri� un error inesperado:\n{e.Exception.Message}\n\nConsulte al soporte t�cnico.", "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // Aqu� se podr�a agregar un Logger a archivo de texto
        }

        // Manejador de excepciones no UI (Threads profundos)
        private static void GlobalUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            MessageBox.Show($"Error Cr�tico de Aplicaci�n:\n{ex.Message}\nEl sistema se cerrar�.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void ConfigurarServicios()
        {
            var services = new ServiceCollection();
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "almacen.db");
            ConnectionStringGlobal = $"Data Source={dbPath}";

            services.AddDbContext<AlmacenDbContext>(options => options.UseSqlite(ConnectionStringGlobal));

            // Servicios
            services.AddTransient<VentaService>();
            services.AddTransient<TicketService>();
            services.AddTransient<AfipService>();
            services.AddTransient<AfipAuthService>();
            services.AddTransient<BackupService>(); // Registramos el nuevo servicio

            // Forms
            services.AddTransient<LoginForm>();

            ServiceProvider = services.BuildServiceProvider();
        }

        private static void InicializarBaseDeDatos()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AlmacenDbContext>();
                context.Database.Migrate();

                if (!context.Usuarios.Any())
                {
                    var admin = new Usuario
                    {
                        Nombre = "Administrador",
                        Apellido = "Principal",
                        Email = "admin@almacen.com",
                        NombreUsuario = "admin",
                        Password = SecurityHelper.HashPassword("123"),
                        Telefono = "000-0000",
                        Rol = RolUsuario.Admin // Aseg�rate de tener este Enum o propiedad
                    };
                    context.Usuarios.Add(admin);
                    context.SaveChanges();
                }

                // Seed Cliente Default
                if (!context.Clientes.Any(c => c.DniCuit == Constantes.CLIENTE_DEF_DNI))
                {
                    context.Clientes.Add(new Cliente
                    {
                        Nombre = Constantes.CLIENTE_DEF_NOMBRE,
                        Apellido = Constantes.CLIENTE_DEF_APELLIDO,
                        DniCuit = Constantes.CLIENTE_DEF_DNI,
                        Email = "-",
                        Telefono = "-",
                        Direccion = "Mostrador"
                    });
                    context.SaveChanges();
                }

                // Carga previa automática de productos de almacén (+1,500 productos) si está vacío
                if (!context.Productos.Any())
                {
                    AlmacenSeedData.SembrarProductos(context);
                }
            }
        }
    }
}