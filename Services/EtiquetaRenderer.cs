using AlmacenDesktop.Modelos;
using System;
using System.Drawing;

namespace AlmacenDesktop.Services
{
    /// <summary>
    /// Adhesiva: etiqueta chica para pegar en el producto, con el código de barras
    /// grande para que se pueda escanear en la caja.
    /// Gondola: cartel para el estante, con el precio enorme para que se lea de lejos.
    /// </summary>
    public enum EstiloEtiqueta
    {
        Adhesiva = 0,
        Gondola = 1
    }

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

        // Cartel de góndola: el precio manda (tiene que leerse desde el pasillo), el
        // nombre arriba para identificar el producto, y abajo chico el código y la fecha
        // de impresión — así se sabe de un vistazo si el cartel quedó viejo.
        private static readonly Font FuenteNombreGondola = new Font("Segoe UI", 9, FontStyle.Regular);
        private static readonly Font FuentePieGondola = new Font("Segoe UI", 6, FontStyle.Regular);

        public static void DibujarGondola(Graphics g, RectangleF rect, Producto producto, bool incluirNombre)
        {
            using (Pen pen = new Pen(Color.Gray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
            {
                g.DrawRectangle(pen, Rectangle.Round(rect));
            }

            var centro = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            float margen = 6;
            float yNombre = rect.Y + margen;
            float altoNombre = incluirNombre ? 22 : 0;

            if (incluirNombre)
            {
                // Una sola línea recortada con puntos suspensivos: en un cartel de
                // góndola el nombre partido en dos renglones le come lugar al precio.
                var rectNombre = new RectangleF(rect.X + margen, yNombre, rect.Width - (margen * 2), altoNombre);
                g.DrawString(producto.Nombre.ToUpperInvariant(), FuenteNombreGondola, Brushes.Black, rectNombre, centro);
            }

            // Pie: código en números (no las barras) + fecha de impresión
            float altoPie = 12;
            float yPie = rect.Bottom - margen - altoPie;

            var rectPrecio = new RectangleF(
                rect.X + margen,
                yNombre + altoNombre,
                rect.Width - (margen * 2),
                yPie - (yNombre + altoNombre));

            // El precio se escala al alto disponible para que ocupe el cartel entero sin
            // desbordarlo, sea la celda chica o grande.
            float tamPrecio = Math.Max(14f, rectPrecio.Height * 0.62f);
            using (var fuentePrecio = new Font("Arial Black", tamPrecio, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                string texto = $"$ {producto.Precio:N2}";

                // Si con ese cuerpo no entra a lo ancho, se baja hasta que entre.
                float ancho = g.MeasureString(texto, fuentePrecio).Width;
                if (ancho > rectPrecio.Width && ancho > 0)
                {
                    float ajustado = Math.Max(12f, tamPrecio * (rectPrecio.Width / ancho) * 0.97f);
                    using (var fuenteAjustada = new Font("Arial Black", ajustado, FontStyle.Bold, GraphicsUnit.Pixel))
                    {
                        g.DrawString(texto, fuenteAjustada, Brushes.Black, rectPrecio, centro);
                    }
                }
                else
                {
                    g.DrawString(texto, fuentePrecio, Brushes.Black, rectPrecio, centro);
                }
            }

            var izquierda = new StringFormat { Alignment = StringAlignment.Near, FormatFlags = StringFormatFlags.NoWrap };
            var derecha = new StringFormat { Alignment = StringAlignment.Far, FormatFlags = StringFormatFlags.NoWrap };
            var rectPie = new RectangleF(rect.X + margen, yPie, rect.Width - (margen * 2), altoPie);

            g.DrawString(producto.CodigoBarras ?? "", FuentePieGondola, Brushes.DimGray, rectPie, izquierda);
            g.DrawString(DateTime.Now.ToString("dd/MM/yyyy"), FuentePieGondola, Brushes.DimGray, rectPie, derecha);
        }

        public static void Dibujar(Graphics g, RectangleF rect, Producto producto, Bitmap codigoBarras, bool incluirPrecio, bool incluirNombre)
        {
            // 1. Dibujar contorno sutil (ayuda al recorte manual)
            using (Pen pen = new Pen(Color.LightGray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
            {
                g.DrawRectangle(pen, Rectangle.Round(rect));
            }

            // Los altos están ajustados para que el precio entre completo en una celda de
            // 140px: antes nombre(40) + código(50) empujaban el precio hasta y=105 y con
            // Arial Black 20 el renglón terminaba en ~143, o sea recortado por el borde.
            float y = rect.Y + 8;
            float xCentro = rect.X + (rect.Width / 2);

            StringFormat centro = new StringFormat() { Alignment = StringAlignment.Center };

            // 2. Nombre del Producto (Arriba, con ajuste de línea)
            if (incluirNombre)
            {
                // Rectángulo para el texto que permite wrapping (multilínea si es largo)
                var rectNombre = new RectangleF(rect.X + 5, y, rect.Width - 10, 36);
                g.DrawString(producto.Nombre, FuenteNombre, Brushes.Black, rectNombre, centro);
                y += 36; // Bajamos el cursor
            }

            // 3. Código de Barras (Centro)
            if (codigoBarras != null)
            {
                // Calculamos dimensiones para centrar la imagen sin deformarla
                float anchoImg = rect.Width * 0.85f; // Usar 85% del ancho disponible
                float altoImg = 46; // Altura fija para el código
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