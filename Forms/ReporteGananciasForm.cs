using AlmacenDesktop.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ReporteGananciasForm : Form
    {
        public ReporteGananciasForm()
        {
            InitializeComponent();

            // Configurar fechas por defecto (Mes actual)
            DateTime hoy = DateTime.Today;
            dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
            dtpHasta.Value = hoy;
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            CalcularGanancias();
        }

        private void CalcularGanancias()
        {
            using (var context = new AlmacenDbContext())
            {
                var desde = dtpDesde.Value.Date;
                var hasta = dtpHasta.Value.Date.AddDays(1).AddTicks(-1);

                // Consulta Inteligente: Agrupamos por producto para ver qué rindió más
                var reporte = context.DetallesVenta
                    .Include(d => d.Producto)
                    .Where(d => d.Venta.Fecha >= desde && d.Venta.Fecha <= hasta)
                    .AsEnumerable() // Traemos a memoria para cálculos complejos
                    .GroupBy(d => d.Producto.Nombre)
                    .Select(g => new
                    {
                        Producto = g.Key,
                        Cantidad = g.Sum(x => x.Cantidad),
                        TotalVenta = g.Sum(x => x.Subtotal),
                        // OJO: Usamos el costo actual del producto como referencia aproximada
                        // (Un sistema perfecto guardaría el costo histórico en DetalleVenta, 
                        // pero para competir con FacilVirtual esto es suficiente por ahora)
                        CostoEstimado = g.Sum(x => x.Cantidad * x.Producto.Costo),
                        Ganancia = g.Sum(x => x.Subtotal - (x.Cantidad * x.Producto.Costo))
                    })
                    .OrderByDescending(x => x.Ganancia) // Lo que más plata dejó arriba
                    .ToList();

                // Totales Generales
                decimal ventaTotal = reporte.Sum(x => x.TotalVenta);
                decimal costoTotal = reporte.Sum(x => x.CostoEstimado);
                decimal gananciaTotal = ventaTotal - costoTotal;

                // Mostrar en UI
                lblVentaTotal.Text = $"$ {ventaTotal:N2}";
                lblCostoTotal.Text = $"$ {costoTotal:N2}";
                lblGananciaNeta.Text = $"$ {gananciaTotal:N2}";

                if (gananciaTotal > 0) lblGananciaNeta.ForeColor = Color.Green;
                else lblGananciaNeta.ForeColor = Color.Red;

                dgvDetalle.DataSource = reporte;

                // Formato Grilla
                if (dgvDetalle.Columns["TotalVenta"] != null) dgvDetalle.Columns["TotalVenta"].DefaultCellStyle.Format = "N2";
                if (dgvDetalle.Columns["CostoEstimado"] != null) dgvDetalle.Columns["CostoEstimado"].DefaultCellStyle.Format = "N2";
                if (dgvDetalle.Columns["Ganancia"] != null) dgvDetalle.Columns["Ganancia"].DefaultCellStyle.Format = "N2";
            }
        }
    }
}