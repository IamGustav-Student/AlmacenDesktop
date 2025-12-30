using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; // Necesario para gráficos

namespace AlmacenDesktop.Forms
{
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var hoy = DateTime.Today;

                    // --- TARJETAS KPI ---
                    decimal ventasHoy = context.Ventas
                        .Where(v => v.Fecha >= hoy)
                        .Sum(v => v.Total);
                    lblVentasHoy.Text = $"$ {ventasHoy:N0}"; // N0 para quitar centavos en dashboard

                    var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
                    decimal ventasMes = context.Ventas
                        .Where(v => v.Fecha >= inicioMes)
                        .Sum(v => v.Total);
                    lblVentasMes.Text = $"$ {ventasMes:N0}";

                    int bajoStock = context.Productos.Count(p => p.Stock <= p.StockMinimo);
                    lblBajoStock.Text = bajoStock.ToString();

                    int clientes = context.Clientes.Count();
                    lblClientes.Text = clientes.ToString();

                    // --- GRILLA (Últimas 5) ---
                    var ultimasVentas = context.Ventas
                        .Include(v => v.Cliente)
                        .OrderByDescending(v => v.Fecha)
                        .Take(5)
                        .Select(v => new
                        {
                            v.Id,
                            Hora = v.Fecha.ToString("HH:mm"),
                            Cliente = v.Cliente.Nombre + " " + v.Cliente.Apellido,
                            Total = v.Total,
                            Pago = v.MetodoPago
                        })
                        .ToList();
                    dgvUltimasVentas.DataSource = ultimasVentas;

                    // --- GRÁFICO 1: BARRAS (Ventas últimos 7 días) ---
                    var fechaLimite = hoy.AddDays(-6);
                    var datosSemana = context.Ventas
                        .Where(v => v.Fecha >= fechaLimite)
                        .ToList() // Traemos a memoria para agrupar por fecha corta
                        .GroupBy(v => v.Fecha.Date)
                        .Select(g => new { Fecha = g.Key, Total = g.Sum(x => x.Total) })
                        .OrderBy(x => x.Fecha)
                        .ToList();

                    chartVentas.Series["Ventas"].Points.Clear();

                    // Rellenar días vacíos si no hubo ventas
                    for (int i = 0; i < 7; i++)
                    {
                        var dia = fechaLimite.AddDays(i);
                        var ventaDia = datosSemana.FirstOrDefault(d => d.Fecha == dia);
                        decimal totalDia = ventaDia != null ? ventaDia.Total : 0;

                        chartVentas.Series["Ventas"].Points.AddXY(dia.ToString("dd/MM"), totalDia);
                    }

                    // --- GRÁFICO 2: TORTA (Métodos de Pago - Mes Actual) ---
                    var datosPagos = context.Ventas
                        .Where(v => v.Fecha >= inicioMes)
                        .GroupBy(v => v.MetodoPago)
                        .Select(g => new { Metodo = g.Key, Cantidad = g.Count() })
                        .ToList();

                    chartPagos.Series[0].Points.Clear();
                    foreach (var pago in datosPagos)
                    {
                        chartPagos.Series[0].Points.AddXY(pago.Metodo, pago.Cantidad);
                    }
                    chartPagos.Series[0].IsValueShownAsLabel = true;
                }
            }
            catch (Exception ex)
            {
                // Log silencioso o visual sutil
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarDatos();
        }

        // Métodos de compatibilidad por si quedaron referencias viejas
        private void tsmiProductos_Click(object sender, EventArgs e) { }
        private void tsmiNuevaVenta_Click(object sender, EventArgs e) { }
    }
}