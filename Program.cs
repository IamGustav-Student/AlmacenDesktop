using AlmacenDesktop.Data;
using AlmacenDesktop.Forms;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop
{
    internal static class Program
    {
        // --- NUEVO: VARIABLE GLOBAL ACCESIBLE DESDE TODO EL PROYECTO ---
        public static Usuario UsuarioActualGlobal;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            InicializarBaseDeDatos();

            LoginForm login = new LoginForm();

            if (login.ShowDialog() == DialogResult.OK)
            {
                // Guardamos el usuario que entró para que cualquiera pueda consultarlo
                UsuarioActualGlobal = login.UsuarioLogueado;

                MenuPrincipal menu = new MenuPrincipal(login.UsuarioLogueado);
                Application.Run(menu);
            }
            else
            {
                Application.Exit();
            }
        }

        private static void InicializarBaseDeDatos()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    context.Database.Migrate();

                    if (!context.Usuarios.Any())
                    {
                        var admin = new Usuario
                        {
                            Nombre = "Administrador",
                            Apellido = "Principal",
                            Email = "admin@almacen.com",
                            NombreUsuario = "admin",
                            Password = "123",
                            Telefono = "000-0000"
                        };
                        context.Usuarios.Add(admin);
                        context.SaveChanges();
                    }

                    if (!context.Clientes.Any(c => c.DniCuit == "00000000"))
                    {
                        var consumidorFinal = new Cliente
                        {
                            Nombre = "Consumidor",
                            Apellido = "Final",
                            DniCuit = "00000000",
                            Email = "-",
                            Telefono = "-",
                            Direccion = "Mostrador"
                        };
                        context.Clientes.Add(consumidorFinal);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fatal BD: {ex.Message}");
            }
        }
    }
}