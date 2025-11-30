using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace AlmacenDesktop.Data
{
    public class AlmacenDbContext : DbContext
    {
        // Definimos las tablas (DbSet)
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        // Configuración de SQLite
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Esto crea el archivo "almacen.db" en la misma carpeta donde corre el .exe
            string dbPath = "almacen.db";
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        // Configuración adicional de modelos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuramos que Usuario y Cliente se guarden en tablas separadas
            // aunque hereden de Persona (Estrategia TPT o TPC, EF Core lo maneja inteligente por defecto)
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");

            base.OnModelCreating(modelBuilder);
        }
    }
}
