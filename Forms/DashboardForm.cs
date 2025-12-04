using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class DashboardForm : Form
    {
        private Dictionary<string, decimal> _ventasSemana = new Dictionary<string, decimal>();

        public DashboardForm()
        {
            InitializeComponent();

            // 🔥 PARTE 1: ACTIVAR LA ESCUCHA DE TECLAS 🔥
            // ------------------------------------------------------------------
            this.KeyPreview = true; // <--- OBLIGATORIO: Permite que el Form capture la tecla antes que el botón
            this.KeyDown += new KeyEventHandler(DashboardForm_KeyDown); // <--- Conectamos el evento
            // ------------------------------------------------------------------
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            CargarDatos();
            this.Focus(); // Aseguramos que el dashboard tenga el foco al abrir
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            CargarDatos();
        }

        // 🔥 PARTE 2: EL CEREBRO QUE DECIDE QUÉ HACER 🔥
        // ------------------------------------------------------------------
        private void DashboardForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1: // <--- Si presionan F1...
                    btnRapidoVenta.PerformClick(); // ...simulamos clic en "Nueva Venta"
                    e.Handled = true; // Evita el sonido "ding" de Windows
                    break;

                case Keys.F2: // <--- Si presionan F2...
                    btnRapidoProductos.PerformClick(); // ...simulamos clic en "Productos"
                    e.Handled = true;
                    break;

                case Keys.F3: // <--- Si presionan F3...
                    btnRapidoCaja.PerformClick(); // ...simulamos clic en "Control Caja"
                    e.Handled = true;
                    break;

                case Keys.F5: // <--- Si presionan F5...
                    btnRefrescar.PerformClick(); // ...actualizamos datos
                    e.Handled = true;
                    break;
            }
        }
        // ------------------------------------------------------------------

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void btnRapidoVenta_Click(object sender, EventArgs e)
        {
            // Llama a la función pública del Menú Principal
            if (this.MdiParent is MenuPrincipal menu) menu.tsmiNuevaVenta_Click(sender, e);
        }

        private void btnRapidoProductos_Click(object sender, EventArgs e)
        {
            if (this.MdiParent is MenuPrincipal menu) menu.tsmiProductos_Click(sender, e);
        }

        private void btnRapidoCaja_Click(object sender, EventArgs e)
        {
            ControlCajaForm form = new ControlCajaForm(Program.UsuarioActualGlobal);
            form.ShowDialog();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    DateTime inicioDia = DateTime.Today;
                    DateTime finDia = DateTime.Today.AddDays(1).AddTicks(-1);

                    // 1. VENTAS DEL DÍA
                    var ventasHoy = context.Ventas
                        .AsNoTracking()
                        .Where(v => v.Fecha >= inicioDia && v.Fecha <= finDia)
                        .ToList();

                    decimal totalVentas = ventasHoy.Sum(v => v.Total);
                    int cantidadVentas = ventasHoy.Count;
                    decimal ticketPromedio = cantidadVentas > 0 ? totalVentas / cantidadVentas : 0;

                    lblVentasHoyMonto.Text = $"$ {totalVentas:N2}";
                    lblCantVentasNumero.Text = cantidadVentas.ToString();
                    lblTicketPromedioMonto.Text = $"$ {ticketPromedio:N2}";

                    // 2. ALERTAS DE STOCK
                    var productosBajos = context.Productos
                        .AsNoTracking()
                        .Where(p => p.Stock <= 10)
                        .OrderBy(p => p.Stock)
                        .Select(p => new { Id = p.Id, Producto = p.Nombre, Stock = p.Stock })
                        .ToList();

                    dgvAlertasStock.DataSource = productosBajos;
                    if (dgvAlertasStock.Columns["Id"] != null) dgvAlertasStock.Columns["Id"].Visible = false;

                    // 3. TOP 5 PRODUCTOS
                    var fechaSemanaAtras = DateTime.Today.AddDays(-7);

                    var ranking = context.DetallesVenta
                        .AsNoTracking()
                        .Include(d => d.Producto)
                        .Where(d => d.Venta.Fecha >= fechaSemanaAtras)
                        .AsEnumerable()
                        .GroupBy(d => d.Producto.Nombre)
                        .Select(g => new { Nombre = g.Key, TotalVendido = g.Sum(x => x.Cantidad) })
                        .OrderByDescending(x => x.TotalVendido)
                        .Take(5)
                        .ToList();

                    lstTopProductos.Items.Clear();
                    foreach (var item in ranking)
                    {
                        lstTopProductos.Items.Add($"{item.TotalVendido} x {item.Nombre}");
                    }

                    // 4. DATOS PARA GRÁFICO
                    _ventasSemana.Clear();
                    for (int i = 6; i >= 0; i--)
                    {
                        DateTime fecha = DateTime.Today.AddDays(-i);
                        DateTime desde = fecha.Date;
                        DateTime hasta = fecha.Date.AddDays(1).AddTicks(-1);

                        decimal totalDia = context.Ventas
                             .Where(v => v.Fecha >= desde && v.Fecha <= hasta)
                             .Sum(v => (decimal?)v.Total) ?? 0;

                        _ventasSemana.Add(fecha.ToString("dd/MM"), totalDia);
                    }

                    pnlGrafico.Invalidate();
                }
            }
            catch (Exception ex)
            {
                // Ignorar errores en dashboard
            }
        }

        private void pnlGrafico_Paint(object sender, PaintEventArgs e)
        {
            if (_ventasSemana.Count == 0) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int margen = 30;
            int anchoUtil = pnlGrafico.Width - (margen * 2);
            int altoUtil = pnlGrafico.Height - (margen * 2);

            decimal maxVenta = _ventasSemana.Values.Count > 0 ? _ventasSemana.Values.Max() : 1;
            if (maxVenta == 0) maxVenta = 1;

            int anchoBarra = (anchoUtil / _ventasSemana.Count) - 10;
            int x = margen;

            using (Pen penEjes = new Pen(Color.LightGray, 1))
            {
                g.DrawLine(penEjes, margen, pnlGrafico.Height - margen, pnlGrafico.Width - margen, pnlGrafico.Height - margen);
            }

            foreach (var dia in _ventasSemana)
            {
                int alturaBarra = (int)((dia.Value / maxVenta) * (altoUtil - 20));

                Rectangle rectBarra = new Rectangle(
                    x,
                    (pnlGrafico.Height - margen) - alturaBarra,
                    anchoBarra,
                    alturaBarra
                );

                if (alturaBarra > 0)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(rectBarra, Color.CornflowerBlue, Color.RoyalBlue, 90F))
                    {
                        g.FillRectangle(brush, rectBarra);
                    }
                }

                g.DrawString(dia.Key, new Font("Segoe UI", 8), Brushes.DimGray, x + (anchoBarra / 2) - 10, pnlGrafico.Height - margen + 5);

                if (alturaBarra > 0)
                {
                    g.DrawString($"${dia.Value:N0}", new Font("Segoe UI", 7), Brushes.Black, x, rectBarra.Y - 15);
                }

                x += anchoBarra + 10;
            }
        }
    }
}