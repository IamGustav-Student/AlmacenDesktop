using AlmacenDesktop.Modelos;
using System.Drawing;

namespace AlmacenDesktop.Services
{
    /// <summary>
    /// Clase estática encargada EXCLUSIVAMENTE de dibujar.
    /// No sabe de formularios, ni de impresoras, solo sabe pintar en un Graphics.
    /// Esto permite usar la misma lógica para pantalla (Preview) y papel (Print).
    /// </summary>
    public static class EtiquetaRenderer
    {
        // Definimos fuentes estáticas para optimizar memoria
        private static readonly Font FuentePrecio = new Font("Arial Black", 20, FontStyle.Bold);
        private static readonly Font FuenteNombre = new Font("Segoe UI", 10, FontStyle.Bold);
        private static readonly Font FuenteSimbolo = new Font("Segoe UI", 12, FontStyle.Bold);

        public static void Dibujar(Graphics g, RectangleF rect, Producto producto, Bitmap codigoBarras, bool incluirPrecio, bool incluirNombre)
        {
            // 1. Dibujar contorno sutil (ayuda al recorte manual)
            using (Pen pen = new Pen(Color.LightGray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
            {
                g.DrawRectangle(pen, Rectangle.Round(rect));
            }

            float y = rect.Y + 10;
            float xCentro = rect.X + (rect.Width / 2);

            StringFormat centro = new StringFormat() { Alignment = StringAlignment.Center };

            // 2. Nombre del Producto (Arriba, con ajuste de línea)
            if (incluirNombre)
            {
                // Rectángulo para el texto que permite wrapping (multilínea si es largo)
                var rectNombre = new RectangleF(rect.X + 5, y, rect.Width - 10, 40);
                g.DrawString(producto.Nombre, FuenteNombre, Brushes.Black, rectNombre, centro);
                y += 40; // Bajamos el cursor
            }

            // 3. Código de Barras (Centro)
            if (codigoBarras != null)
            {
                // Calculamos dimensiones para centrar la imagen sin deformarla
                float anchoImg = rect.Width * 0.85f; // Usar 85% del ancho disponible
                float altoImg = 50; // Altura fija para el código
                float xImg = rect.X + (rect.Width - anchoImg) / 2;

                // Interpolación alta para que el código se vea nítido al redimensionar
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(codigoBarras, xImg, y, anchoImg, altoImg);
                y += altoImg + 5;
            }

            // 4. Precio (Abajo, Grande)
            if (incluirPrecio)
            {
                string precioTexto = $"$ {producto.Precio:N2}";

                // Fondo amarillo opcional para resaltar el precio (estilo supermercado)
                // g.FillRectangle(Brushes.Yellow, rect.X + 20, y, rect.Width - 40, 35);

                g.DrawString(precioTexto, FuentePrecio, Brushes.Black, xCentro, y, centro);
            }
        }
    }
}