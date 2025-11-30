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
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 1. Inicializar DB (Crear tablas y datos por defecto)
            InicializarBaseDeDatos();

            // 2. Mostrar Login
            LoginForm login = new LoginForm();

            if (login.ShowDialog() == DialogResult.OK)
            {
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

                    // --- SEMILLA DE USUARIOS ---
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

                    // --- NUEVO: SEMILLA DE CLIENTE "CONSUMIDOR FINAL" ---
                    // Verificamos si ya existe por su DNI ficticio (ej. 00000000)
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