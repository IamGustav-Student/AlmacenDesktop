using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Services
{
    public class TicketService
    {
        private Font fuenteTitulo = new Font("Consolas", 14, FontStyle.Bold);
        private Font fuenteRegular = new Font("Consolas", 9, FontStyle.Regular);
        private Font fuenteNegrita = new Font("Consolas", 9, FontStyle.Bold);

        // Datos del Negocio (Cache)
        private string _nombreNegocio = "MI ALMACEN";
        private string _direccion = "-";

        // Variables de estado para impresión
        private Venta _ventaParaImprimir;
        private List<DetalleVenta> _detallesParaImprimir;
        private Caja _cajaParaImprimir;
        private bool _esReporteCaja = false;

        public TicketService()
        {
            CargarDatosNegocio();
        }

        private void CargarDatosNegocio()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var datos = context.DatosNegocio.FirstOrDefault();
                    if (datos != null)
                    {
                        _nombreNegocio = datos.NombreFantasia;
                        _direccion = datos.Direccion;
                    }
                }
            }
            catch { }
        }

        // --- MÉTODO 1: IMPRIMIR VENTA (TICKET FACTURA) ---
        public void Imprimir(Venta venta, List<DetalleVenta> detalles)
        {
            _ventaParaImprimir = venta;
            _detallesParaImprimir = detalles;
            _esReporteCaja = false;
            LanzarImpresion();
        }

        // --- MÉTODO 2: IMPRIMIR CIERRE (TICKET Z) ---
        public void ImprimirCierreCaja(Caja caja)
        {
            _cajaParaImprimir = caja;
            _esReporteCaja = true;
            LanzarImpresion();
        }

        private void LanzarImpresion()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(DibujarTicket);
            try { pd.Print(); }
            catch (Exception ex) { MessageBox.Show("Error de impresora: " + ex.Message); }
        }

        private void DibujarTicket(object sender, PrintPageEventArgs e)
        {
            if (_esReporteCaja) DibujarCierre(e.Graphics);
            else DibujarVenta(e.Graphics);
        }

        // Lógica de Dibujo de Venta (Resumida para este ejemplo, usa la que ya tenías o esta)
        private void DibujarVenta(Graphics g)
        {
            float y = 10;
            float ancho = 280;
            StringFormat centro = new StringFormat() { Alignment = StringAlignment.Center };
            StringFormat derecha = new StringFormat() { Alignment = StringAlignment.Far };

            g.DrawString(_nombreNegocio, fuenteTitulo, Brushes.Black, new RectangleF(0, y, ancho, 25), centro);
            y += 40;
            g.DrawString($"Venta #: {_ventaParaImprimir.Id} - {_ventaParaImprimir.Fecha:dd/MM HH:mm}", fuenteRegular, Brushes.Black, 5, y);
            y += 20;
            g.DrawString("--------------------------------", fuenteRegular, Brushes.Black, 5, y);
            y += 15;

            foreach (var item in _detallesParaImprimir)
            {
                string linea = $"{item.Cantidad} x {item.Producto.Nombre}";
                if (linea.Length > 25) linea = linea.Substring(0, 25);
                g.DrawString(linea, fuenteRegular, Brushes.Black, 5, y);
                g.DrawString($"{item.Subtotal:N2}", fuenteRegular, Brushes.Black, new RectangleF(0, y, ancho - 5, 15), derecha);
                y += 15;
            }

            y += 10;
            g.DrawString($"TOTAL: $ {_ventaParaImprimir.Total:N2}", fuenteTitulo, Brushes.Black, new RectangleF(0, y, ancho - 5, 25), derecha);
        }

        // Lógica de Dibujo de Cierre de Caja (Ticket Z)
        private void DibujarCierre(Graphics g)
        {
            float y = 10;
            float ancho = 280;
            StringFormat centro = new StringFormat() { Alignment = StringAlignment.Center };
            StringFormat derecha = new StringFormat() { Alignment = StringAlignment.Far };

            g.DrawString(_nombreNegocio, fuenteTitulo, Brushes.Black, new RectangleF(0, y, ancho, 25), centro);
            y += 25;
            g.DrawString("CIERRE DE CAJA (Z)", fuenteNegrita, Brushes.Black, new RectangleF(0, y, ancho, 20), centro);
            y += 30;

            g.DrawString($"Caja ID: {_cajaParaImprimir.Id}", fuenteRegular, Brushes.Black, 5, y);
            y += 15;
            g.DrawString($"Apertura: {_cajaParaImprimir.FechaApertura:dd/MM HH:mm}", fuenteRegular, Brushes.Black, 5, y);
            y += 15;
            g.DrawString($"Cierre:   {DateTime.Now:dd/MM HH:mm}", fuenteRegular, Brushes.Black, 5, y);
            y += 15;

            g.DrawString("--------------------------------", fuenteRegular, Brushes.Black, 5, y);
            y += 15;

            DibujarFila(g, "Saldo Inicial:", _cajaParaImprimir.SaldoInicial, ref y, ancho, derecha);
            DibujarFila(g, "Ventas Efvo:", _cajaParaImprimir.TotalVentasEfectivo, ref y, ancho, derecha);

            // Calculamos sistema vs real
            DibujarFila(g, "Debería haber:", _cajaParaImprimir.SaldoFinalSistema, ref y, ancho, derecha);

            y += 10;
            g.DrawString("REAL EN CAJA:", fuenteNegrita, Brushes.Black, 5, y);
            g.DrawString($"$ {_cajaParaImprimir.SaldoFinalReal:N2}", fuenteTitulo, Brushes.Black, new RectangleF(0, y - 2, ancho - 5, 25), derecha);
            y += 25;

            if (_cajaParaImprimir.Diferencia != 0)
            {
                string etiqueta = _cajaParaImprimir.Diferencia > 0 ? "SOBRANTE:" : "FALTANTE:";
                g.DrawString(etiqueta, fuenteNegrita, Brushes.Black, 5, y);
                g.DrawString($"$ {_cajaParaImprimir.Diferencia:N2}", fuenteNegrita, Brushes.Black, new RectangleF(0, y, ancho - 5, 15), derecha);
            }
            else
            {
                g.DrawString("** CAJA PERFECTA **", fuenteRegular, Brushes.Black, new RectangleF(0, y, ancho, 15), centro);
            }
        }

        private void DibujarFila(Graphics g, string texto, decimal valor, ref float y, float ancho, StringFormat format)
        {
            g.DrawString(texto, fuenteRegular, Brushes.Black, 5, y);
            g.DrawString($"$ {valor:N2}", fuenteRegular, Brushes.Black, new RectangleF(0, y, ancho - 5, 15), format);
            y += 15;
        }
    }
}