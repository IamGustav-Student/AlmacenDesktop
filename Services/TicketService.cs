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
        private Font fuentePeque = new Font("Consolas", 8, FontStyle.Regular); // Usaremos esta fuente pequeña

        private Venta _ventaActual;
        private List<DetalleVenta> _detallesActuales;

        private string _nombreNegocio = "MI ALMACEN";
        private string _direccion = "Dirección no configurada";
        private string _cuit = "";
        private string _telefono = "";
        private string _mensajePie = "¡Gracias por su compra!";

        public void Imprimir(Venta venta, List<DetalleVenta> detalles)
        {
            _ventaActual = venta;
            _detallesActuales = detalles;

            CargarDatosNegocio();

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(DibujarTicket);

            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al intentar imprimir: {ex.Message}", "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                        if (!string.IsNullOrEmpty(datos.NombreFantasia)) _nombreNegocio = datos.NombreFantasia;
                        if (!string.IsNullOrEmpty(datos.Direccion)) _direccion = datos.Direccion;
                        if (!string.IsNullOrEmpty(datos.CUIT)) _cuit = $"CUIT: {datos.CUIT}";
                        if (!string.IsNullOrEmpty(datos.Telefono)) _telefono = $"Tel: {datos.Telefono}";
                        if (!string.IsNullOrEmpty(datos.MensajeTicket)) _mensajePie = datos.MensajeTicket;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Advertencia: No se pudo leer la configuración del negocio.\n" + ex.Message, "Configuración", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DibujarTicket(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            float y = 10;
            float margenIzquierdo = 5;
            float ancho = 280;

            StringFormat centro = new StringFormat() { Alignment = StringAlignment.Center };
            StringFormat derecha = new StringFormat() { Alignment = StringAlignment.Far };

            // 1. ENCABEZADO
            g.DrawString(_nombreNegocio, fuenteTitulo, Brushes.Black, new RectangleF(0, y, ancho, 25), centro);
            y += 25;

            g.DrawString(_direccion, fuenteRegular, Brushes.Black, new RectangleF(0, y, ancho, 15), centro);
            y += 15;

            if (!string.IsNullOrEmpty(_telefono))
            {
                g.DrawString(_telefono, fuenteRegular, Brushes.Black, new RectangleF(0, y, ancho, 15), centro);
                y += 15;
            }

            if (!string.IsNullOrEmpty(_cuit))
            {
                g.DrawString(_cuit, fuenteRegular, Brushes.Black, new RectangleF(0, y, ancho, 15), centro);
                y += 15;
            }

            g.DrawString("--------------------------------", fuenteRegular, Brushes.Black, new RectangleF(0, y, ancho, 15), centro);
            y += 15;

            // 2. DATOS DE VENTA
            g.DrawString($"Fecha: {_ventaActual.Fecha:dd/MM/yyyy HH:mm}", fuentePeque, Brushes.Black, margenIzquierdo, y);
            y += 12;
            g.DrawString($"Venta #: {_ventaActual.Id}", fuentePeque, Brushes.Black, margenIzquierdo, y);
            y += 12;

            if (_ventaActual.ClienteId > 0)
                g.DrawString($"Cliente ID: {_ventaActual.ClienteId}", fuentePeque, Brushes.Black, margenIzquierdo, y);
            y += 15;

            g.DrawString("CANT  PRODUCTO           TOTAL", fuenteNegrita, Brushes.Black, margenIzquierdo, y);
            y += 15;
            g.DrawString("--------------------------------", fuenteRegular, Brushes.Black, new RectangleF(0, y, ancho, 15), centro);
            y += 15;

            // 3. DETALLES
            foreach (var item in _detallesActuales)
            {
                string nombreProd = item.Producto.Nombre.Length > 18
                    ? item.Producto.Nombre.Substring(0, 18) + ".."
                    : item.Producto.Nombre;

                string linea = $"{item.Cantidad} x {nombreProd}";

                g.DrawString(linea, fuenteRegular, Brushes.Black, margenIzquierdo, y);
                g.DrawString($"{item.Subtotal:N2}", fuenteRegular, Brushes.Black, new RectangleF(0, y, ancho - 5, 15), derecha);

                y += 15;
            }

            g.DrawString("--------------------------------", fuenteRegular, Brushes.Black, new RectangleF(0, y, ancho, 15), centro);
            y += 15;

            // 4. TOTALES
            g.DrawString("TOTAL A PAGAR:", fuenteNegrita, Brushes.Black, margenIzquierdo, y);
            // Dibujamos el total grande
            g.DrawString($"$ {_ventaActual.Total:N2}", fuenteTitulo, Brushes.Black, new RectangleF(0, y - 2, ancho - 5, 25), derecha);

            y += 25; // Bajamos el cursor después del número grande

            // --- NUEVO: DETALLE DE RECARGO ---
            if (_ventaActual.MetodoPago == "Billetera Virtual")
            {
                // Dibujamos la nota pequeña alineada a la derecha, justo debajo del total
                g.DrawString("(Incl. 6% Recargo)", fuentePeque, Brushes.Black, new RectangleF(0, y, ancho - 5, 15), derecha);
                y += 15; // Espacio extra para que no se pegue al método de pago
            }
            // ---------------------------------

            y += 5; // Un pequeño respiro antes de la siguiente sección

            g.DrawString($"Pago: {_ventaActual.MetodoPago}", fuenteRegular, Brushes.Black, margenIzquierdo, y);
            y += 20;

            // 5. PIE
            g.DrawString(_mensajePie, fuenteNegrita, Brushes.Black, new RectangleF(0, y, ancho, 20), centro);
            y += 20;
            g.DrawString(".", fuentePeque, Brushes.Black, new RectangleF(0, y, ancho, 20), centro);
        }
    }
}