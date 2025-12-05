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
        public Bitmap GenerarCodigoBarras(string contenido)
        {
            if (string.IsNullOrWhiteSpace(contenido)) return null;

            // Configuración del escritor usando PixelData (Independiente de la plataforma)
            var escritor = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.EAN_13,
                Options = new EncodingOptions
                {
                    Height = 80,
                    Width = 200,
                    PureBarcode = false, // Muestra los números abajo
                    Margin = 0
                }
            };

            // Validación simple: Si no tiene 13 dígitos, usamos Code128 que acepta cualquier cosa
            if (contenido.Length != 13 || !esSoloNumeros(contenido))
            {
                escritor.Format = BarcodeFormat.CODE_128;
            }

            // 1. Generamos los datos crudos (píxeles)
            var pixelData = escritor.Write(contenido);

            // 2. Convertimos a Bitmap de Windows
            // Esta conversión manual es la clave para evitar errores de compatibilidad
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

        private bool esSoloNumeros(string str)
        {
            foreach (char c in str)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }
    }
}