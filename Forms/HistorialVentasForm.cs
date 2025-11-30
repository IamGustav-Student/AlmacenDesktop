using AlmacenDesktop.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class HistorialVentasForm : Form
    {
        public HistorialVentasForm()
        {
            InitializeComponent();
        }

        private void HistorialVentasForm_Load(object sender, EventArgs e)
        {
            // Por defecto, mostramos las ventas del mes actual
            DateTime hoy = DateTime.Now;
            dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
            dtpHasta.Value = hoy;

            BuscarVentas();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarVentas();
        }

        private void BuscarVentas()
        {
            using (var context = new AlmacenDbContext())
            {
                var desde = dtpDesde.Value.Date; // 00:00:00
                var hasta = dtpHasta.Value.Date.AddDays(1).AddTicks(-1); // 23:59:59 del mismo día

                // [POO - Relaciones]
                // Usamos Include para "unir" tablas y traer el objeto Cliente y Usuario asociado
                var ventas = context.Ventas
                    .Include(v => v.Cliente)
                    .Include(v => v.Usuario)
                    .Where(v => v.Fecha >= desde && v.Fecha <= hasta)
                    .OrderByDescending(v => v.Fecha) // Las más recientes primero
                    .ToList();

                // Proyectamos a una clase anónima para que la grilla muestre datos lindos
                dgvVentas.DataSource = ventas.Select(v => new
                {
                    Id = v.Id,
                    Fecha = v.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    Cliente = v.Cliente.NombreCompleto, // Polimorfismo en acción
                    Vendedor = v.Usuario.NombreUsuario,
                    Pago = v.MetodoPago,
                    Total = $"${v.Total:N2}"
                }).ToList();
            }
        }

        // Evento Maestro-Detalle
        private void dgvVentas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVentas.SelectedRows.Count > 0)
            {
                // Obtenemos el ID de la venta seleccionada
                // (La celda 0 es "Id" porque así lo definimos en el Select de arriba)
                int ventaId = (int)dgvVentas.SelectedRows[0].Cells["Id"].Value;

                CargarDetalles(ventaId);
            }
            else
            {
                dgvDetalles.DataSource = null;
            }
        }

        private void CargarDetalles(int ventaId)
        {
            using (var context = new AlmacenDbContext())
            {
                var detalles = context.DetallesVenta
                    .Include(d => d.Producto)
                    .Where(d => d.VentaId == ventaId)
                    .ToList();

                dgvDetalles.DataSource = detalles.Select(d => new
                {
                    Producto = d.Producto.Nombre,
                    PrecioUnitario = $"${d.PrecioUnitario:N2}",
                    Cantidad = d.Cantidad,
                    Subtotal = $"${d.Subtotal:N2}"
                }).ToList();
            }
        }
    }
}