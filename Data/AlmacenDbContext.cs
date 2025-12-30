using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace AlmacenDesktop.Data
{
    public class AlmacenDbContext : DbContext
    {
        // --- MÓDULO VENTAS Y PRODUCTOS ---
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        // --- MÓDULO COMPRAS Y PROVEEDORES ---
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetallesCompra { get; set; }

        // --- MÓDULO CAJA Y FINANZAS ---
        public DbSet<Caja> Cajas { get; set; }
        public DbSet<MovimientoCaja> MovimientosCaja { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        // --- CONFIGURACIÓN GENERAL ---
        public DbSet<DatosNegocio> DatosNegocio { get; set; }
        public DbSet<ConfiguracionAfip> ConfiguracionesAfip { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "almacen.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CONFIGURACIÓN DE PRECISIÓN DECIMAL
            // Es vital que las propiedades existan y tengan 'set' en los modelos

            modelBuilder.Entity<Producto>().Property(p => p.Costo).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Producto>().Property(p => p.Precio).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Venta>().Property(v => v.Total).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DetalleVenta>().Property(d => d.PrecioUnitario).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DetalleVenta>().Property(d => d.Subtotal).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Compra>().Property(c => c.Total).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DetalleCompra>().Property(d => d.CostoUnitario).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DetalleCompra>().Property(d => d.Subtotal).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Caja>().Property(c => c.SaldoInicial).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Caja>().Property(c => c.TotalVentasEfectivo).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Caja>().Property(c => c.TotalVentasOtros).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Caja>().Property(c => c.SaldoFinalSistema).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Caja>().Property(c => c.SaldoFinalReal).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Caja>().Property(c => c.Diferencia).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MovimientoCaja>().Property(m => m.Monto).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Pago>().Property(p => p.Monto).HasColumnType("decimal(18,2)");

            // ÍNDICES
            modelBuilder.Entity<Producto>().HasIndex(p => p.CodigoBarras).IsUnique();
            modelBuilder.Entity<Cliente>().HasIndex(c => c.DniCuit);
        }
    }
}