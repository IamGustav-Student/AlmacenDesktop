using AlmacenDesktop.Data;
using AlmacenDesktop.Forms;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop
{
    internal static class Program
    {
        public static Usuario UsuarioActualGlobal;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            InicializarBaseDeDatos();

            LoginForm login = new LoginForm();
            if (login.ShowDialog() == DialogResult.OK)
            {
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
                            Password = SecurityHelper.HashPassword("123"),
                            Telefono = "000-0000"
                        };
                        context.Usuarios.Add(admin);
                        context.SaveChanges();
                    }

                    if (!context.Clientes.Any(c => c.DniCuit == Constantes.CLIENTE_DEF_DNI))
                    {
                        context.Clientes.Add(new Cliente { Nombre = Constantes.CLIENTE_DEF_NOMBRE, Apellido = Constantes.CLIENTE_DEF_APELLIDO, DniCuit = Constantes.CLIENTE_DEF_DNI, Email = "-", Telefono = "-", Direccion = "Mostrador" });
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