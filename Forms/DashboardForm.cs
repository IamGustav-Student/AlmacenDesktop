using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace AlmacenDesktop.Forms
{
    public partial class DashboardForm : Form
    {
        private Chart chartVentas;
        private Chart chartTop;

        public DashboardForm()
        {
            InitializeComponent();
            InicializarGraficos();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            CargarMetricas();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarMetricas();
            AudioHelper.PlayOk();
        }

        private void InicializarGraficos()
        {
            // Redimensionar Form para alojar gráficos abajo sin desfasar el diseño
            this.Size = new Size(1000, 780);
            this.MinimumSize = new Size(1000, 780);
            this.MaximumSize = new Size(1000, 780);

            // 1. CHART VENTAS (Línea de Tendencia)
            chartVentas = new Chart();
            chartVentas.Size = new Size(540, 200);
            chartVentas.Location = new Point(20, 520);
            chartVentas.BorderlineColor = Color.LightGray;
            chartVentas.BorderlineDashStyle = ChartDashStyle.Solid;

            var areaVentas = new ChartArea("AreaVentas");
            areaVentas.AxisX.MajorGrid.LineColor = Color.FromArgb(235, 235, 235);
            areaVentas.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 235, 235);
            chartVentas.ChartAreas.Add(areaVentas);

            var seriesVentas = new Series("Ventas")
            {
                ChartType = SeriesChartType.SplineArea,
                Color = Color.FromArgb(100, 0, 122, 204), // Azul transparente
                BorderColor = Color.FromArgb(0, 122, 204),
                BorderWidth = 3
            };
            chartVentas.Series.Add(seriesVentas);
            this.Controls.Add(chartVentas);

            // 2. CHART TOP (Doughnut 3D)
            chartTop = new Chart();
            chartTop.Size = new Size(380, 200);
            chartTop.Location = new Point(580, 520);
            chartTop.BorderlineColor = Color.LightGray;
            chartTop.BorderlineDashStyle = ChartDashStyle.Solid;

            var areaTop = new ChartArea("AreaTop");
            areaTop.Area3DStyle.Enable3D = true; // Efecto 3D
            chartTop.ChartAreas.Add(areaTop);

            var seriesTop = new Series("Top")
            {
                ChartType = SeriesChartType.Doughnut
            };
            seriesTop["DoughnutRadius"] = "50";
            chartTop.Series.Add(seriesTop);
            
            var legend = new Legend("Legend1")
            {
                Docking = Docking.Right,
                Alignment = StringAlignment.Center
            };
            chartTop.Legends.Add(legend);

            this.Controls.Add(chartTop);
        }

        private void CargarMetricas()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                using (var db = new AlmacenDbContext())
                {
                    // 1. KPI: Ventas de Hoy
                    DateTime hoy = DateTime.Today;
                    var ventasHoy = db.Ventas
                        .Where(v => v.Fecha >= hoy)
                        .Sum(v => (decimal?)v.Total) ?? 0;

                    lblVentasHoyMonto.Text = ventasHoy.ToString("C2");

                    // 2. KPI: Acumulado Mes Actual
                    DateTime inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var ventasMes = db.Ventas
                        .Where(v => v.Fecha >= inicioMes)
                        .Sum(v => (decimal?)v.Total) ?? 0;

                    lblVentasMesMonto.Text = ventasMes.ToString("C2");

                    // 3. KPI: Ganancia Estimada (Mes)
                    var gananciaMes = db.DetallesVenta
                        .Include(d => d.Producto)
                        .Include(d => d.Venta)
                        .Where(d => d.Venta.Fecha >= inicioMes)
                        .AsEnumerable()
                        .Sum(d => (d.PrecioUnitario - d.Producto.Costo) * d.Cantidad);

                    lblGananciaMonto.Text = gananciaMes.ToString("C2");

                    // 4. KPI: Alertas de Stock
                    var productosCriticos = db.Productos
                        .Where(p => p.Stock <= p.StockMinimo && p.Activo)
                        .OrderBy(p => p.Stock)
                        .Select(p => new {
                            Id = p.Id,
                            Producto = p.Nombre,
                            Stock = p.Stock,
                            Minimo = p.StockMinimo
                        })
                        .ToList();

                    lblAlertaCantidad.Text = productosCriticos.Count.ToString();
                    if (productosCriticos.Count > 0)
                    {
                        panelAlerta.BackColor = Color.Firebrick;
                    }
                    else
                    {
                        panelAlerta.BackColor = Color.ForestGreen;
                    }

                    dgvBajoStock.DataSource = productosCriticos;

                    // 5. GRID: Top 5 Productos (Ranking)
                    var ranking = db.DetallesVenta
                        .Where(d => d.Venta.Fecha >= inicioMes)
                        .GroupBy(d => d.Producto.Nombre)
                        .Select(g => new {
                            Producto = g.Key,
                            Unidades = g.Sum(x => x.Cantidad),
                            TotalGenerado = g.Sum(x => x.Subtotal)
                        })
                        .OrderByDescending(x => x.Unidades)
                        .Take(5)
                        .ToList();

                    dgvTopProductos.DataSource = ranking;

                    if (dgvTopProductos.Columns["TotalGenerado"] != null)
                        dgvTopProductos.Columns["TotalGenerado"].DefaultCellStyle.Format = "C2";

                    // 6. GRAFICOS: Tendencia de Ventas (últimos 15 días)
                    DateTime limite = DateTime.Today.AddDays(-15);
                    var ventasUltimosDias = db.Ventas
                        .Where(v => v.Fecha >= limite)
                        .AsEnumerable() // Traer a memoria para agrupar por fecha formateada
                        .GroupBy(v => v.Fecha.Date)
                        .Select(g => new {
                            Fecha = g.Key,
                            Total = g.Sum(x => x.Total)
                        })
                        .OrderBy(x => x.Fecha)
                        .ToList();

                    chartVentas.Series["Ventas"].Points.Clear();
                    foreach (var item in ventasUltimosDias)
                    {
                        chartVentas.Series["Ventas"].Points.AddXY(item.Fecha.ToString("dd/MM"), (double)item.Total);
                    }

                    // 7. GRAFICOS: Top 5 Productos en Doughnut Chart
                    chartTop.Series["Top"].Points.Clear();
                    foreach (var item in ranking)
                    {
                        var punto = chartTop.Series["Top"].Points.Add((double)item.TotalGenerado);
                        punto.LegendText = item.Producto;
                        punto.Label = $"{item.Unidades} u.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando métricas: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}