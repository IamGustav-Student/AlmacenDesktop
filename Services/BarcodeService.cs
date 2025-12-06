using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ZXing;
using ZXing.Common;

namespace AlmacenDesktop.Services
{
    public class BarcodeService
    {
        // Generamos códigos con suficiente resolución para que no se pixelen al imprimir
        public Bitmap GenerarCodigoBarras(string contenido, int ancho = 300, int alto = 100)
        {
            if (string.IsNullOrWhiteSpace(contenido)) return null;

            var escritor = new BarcodeWriterPixelData
            {
                // CODE_128 es el más versátil (lee letras y números, longitud variable)
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = alto,
                    Width = ancho,
                    PureBarcode = false, // false = Muestra los números humanamente legibles abajo
                    Margin = 2
                }
            };

            // Si detectamos que es un código estándar de comercio (13 números), usamos EAN_13
            if (contenido.Length == 13 && EsSoloNumeros(contenido))
            {
                escritor.Format = BarcodeFormat.EAN_13;
            }

            try
            {
                var pixelData = escritor.Write(contenido);

                // Convertimos los datos crudos de ZXing a un Bitmap de Windows GDI+
                var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

                try
                {
                    Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                return bitmap;
            }
            catch
            {
                // Si el contenido no es válido para el formato, devolvemos null
                return null;
            }
        }

        private bool EsSoloNumeros(string str)
        {
            foreach (char c in str) if (!char.IsDigit(c)) return false;
            return true;
        }
    }
}