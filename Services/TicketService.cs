using AlmacenDesktop.Modelos;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing; // Para GDI+ (Impresoras tradicionales)
using System.Drawing.Printing; // Para GDI+
using System.Runtime.InteropServices;
using System.Linq;

namespace AlmacenDesktop.Services
{
    // MOTOR DE IMPRESIÓN HÍBRIDO E INTELIGENTE
    public class TicketService
    {
        private readonly EPSON _e;

        // CONFIGURACIÓN: Nombre de la impresora (Idealmente leer de BD)
        // Puedes cambiar esto manualmente o hacer que lo lea de ConfiguracionAfip o un archivo settings
        private string _nombreImpresora = "Microsoft Print to PDF"; // Default seguro para pruebas

        // Palabras clave para detectar Ticketeras automáticamente
        private readonly string[] _keywordsTermicas = {
            "POS", "TICKET", "EPSON TM", "THERMAL", "RECEIPT", "GENERIC", "TEXT ONLY", "XPRINTER", "SAM4S", "BIXOLON"
        };

        // Estado interno para pasar datos al evento PrintPage de GDI+
        private Venta _ventaPendiente;
        private List<DetalleVenta> _detallesPendientes;
        private Caja _cajaPendiente;
        private MovimientoCaja _movimientoPendiente;
        private string _tipoDocumentoGrafico;

        public TicketService()
        {
            _e = new EPSON();
            // Intentar leer nombre de impresora de algún lado si es posible, sino usa el default
        }

        // --- PUNTO DE ENTRADA: IMPRIMIR VENTA ---
        public void ImprimirVenta(Venta venta, List<DetalleVenta> detalles)
        {
            if (EsImpresoraTermica(_nombreImpresora))
            {
                try
                {
                    var bytes = ConstruirTicketVentaBytes(venta, detalles);
                    EnviarAImpresoraRaw(bytes, _nombreImpresora);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error Modo RAW (Térmica): " + ex.Message);
                }
            }
            else
            {
                // Modo Gráfico (Impresora Hogar/Oficina)
                ImprimirGrafico(venta, detalles, null, null, "VENTA");
            }
        }

        public void ImprimirCierreCaja(Caja caja)
        {
            if (EsImpresoraTermica(_nombreImpresora))
            {
                var bytes = ConstruirTicketCierreBytes(caja);
                EnviarAImpresoraRaw(bytes, _nombreImpresora);
            }
            else
            {
                ImprimirGrafico(null, null, caja, null, "CIERRE");
            }
        }

        public void ImprimirMovimiento(MovimientoCaja mov)
        {
            if (EsImpresoraTermica(_nombreImpresora))
            {
                var bytes = ConstruirTicketMovimientoBytes(mov);
                EnviarAImpresoraRaw(bytes, _nombreImpresora);
            }
            else
            {
                ImprimirGrafico(null, null, null, mov, "MOVIMIENTO");
            }
        }

        // --- LÓGICA DE DETECCIÓN INTELIGENTE ---
        private bool EsImpresoraTermica(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return false;
            string nombreNormalizado = nombre.ToUpper();

            // 1. Buscar palabras clave
            foreach (var key in _keywordsTermicas)
            {
                if (nombreNormalizado.Contains(key)) return true;
            }
            return false; // Por defecto asumimos Tradicional para evitar imprimir basura en una HP/Canon
        }

        // ==========================================
        //        MOTOR 1: IMPRESIÓN GRÁFICA (GDI+)
        //        (Para HP, Canon, PDF, Láser)
        // ==========================================

        private void ImprimirGrafico(Venta v, List<DetalleVenta> d, Caja c, MovimientoCaja m, string tipo)
        {
            _ventaPendiente = v;
            _detallesPendientes = d;
            _cajaPendiente = c;
            _movimientoPendiente = m;
            _tipoDocumentoGrafico = tipo;

            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = _nombreImpresora;

            // Si la impresora no es válida, intentar con la por defecto de Windows
            if (!pd.PrinterSettings.IsValid)
            {
                // Fallback a impresora default
            }

            pd.PrintPage += new PrintPageEventHandler(DibujarPagina);

            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en impresión gráfica. Verifique que '{_nombreImpresora}' esté conectada. Detalle: {ex.Message}");
            }
        }

        private void DibujarPagina(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            // Configuración de Fuentes
            Font fontTitulo = new Font("Arial", 14, FontStyle.Bold);
            Font fontRegular = new Font("Consolas", 10, FontStyle.Regular); // Monospace para alinear
            Font fontBold = new Font("Consolas", 10, FontStyle.Bold);
            Font fontChica = new Font("Consolas", 8, FontStyle.Regular);

            float y = 40;
            float leftMargin = 40;
            // Simulamos el ancho de un ticket (80mm aprox) incluso en hoja A4 para mantener el formato
            float anchoTicket = 280;

            StringFormat centro = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat derecha = new StringFormat { Alignment = StringAlignment.Far };
            StringFormat izquierda = new StringFormat { Alignment = StringAlignment.Near };

            // CABECERA COMÚN
            g.DrawString("VENDEMAX POS", fontTitulo, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 25), centro);
            y += 25;
            g.DrawString("Tu Negocio S.A.", fontRegular, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), centro);
            y += 20;
            g.DrawString("Av. Siempre Viva 123", fontChica, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 15), centro);
            y += 20;
            g.DrawLine(Pens.Black, leftMargin, y, leftMargin + anchoTicket, y);
            y += 10;

            if (_tipoDocumentoGrafico == "VENTA")
            {
                // TIPO DE COMPROBANTE
                string tipoCbt = _ventaPendiente.TipoComprobante == "X" ? "NO FISCAL" : $"FACTURA {_ventaPendiente.TipoComprobante}";
                g.DrawString(tipoCbt, fontTitulo, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 25), centro);
                y += 30;

                // DATOS VENTA
                g.DrawString($"Nro: {_ventaPendiente.NumeroFactura}", fontRegular, Brushes.Black, leftMargin, y);
                y += 15;
                g.DrawString($"Fecha: {_ventaPendiente.Fecha:dd/MM/yyyy HH:mm}", fontRegular, Brushes.Black, leftMargin, y);
                y += 15;
                g.DrawString($"Cliente: {_ventaPendiente.Cliente?.NombreCompleto ?? "Consumidor Final"}", fontRegular, Brushes.Black, leftMargin, y);
                y += 15;
                if (_ventaPendiente.Cliente != null && _ventaPendiente.Cliente.DniCuit.Length > 5)
                {
                    g.DrawString($"DNI/CUIT: {_ventaPendiente.Cliente.DniCuit}", fontRegular, Brushes.Black, leftMargin, y);
                    y += 15;
                }

                g.DrawLine(Pens.Black, leftMargin, y, leftMargin + anchoTicket, y);
                y += 5;

                // DETALLES (GRILLA SIMULADA)
                g.DrawString("CANT  DESCRIPCION           TOTAL", fontBold, Brushes.Black, leftMargin, y);
                y += 20;

                foreach (var item in _detallesPendientes)
                {
                    string nombre = item.Producto.Nombre;
                    if (nombre.Length > 18) nombre = nombre.Substring(0, 18) + "."; // Truncar nombre

                    // Columna Cantidad
                    g.DrawString($"{item.Cantidad}", fontRegular, Brushes.Black, leftMargin, y);
                    // Columna Descripción
                    g.DrawString($"{nombre}", fontRegular, Brushes.Black, leftMargin + 35, y);
                    // Columna Total (Alineada derecha)
                    g.DrawString($"$ {item.Subtotal:N2}", fontRegular, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), derecha);
                    y += 18;
                }

                y += 10;
                g.DrawLine(Pens.Black, leftMargin, y, leftMargin + anchoTicket, y);
                y += 10;

                // TOTALES
                g.DrawString($"TOTAL: $ {_ventaPendiente.Total:N2}", fontTitulo, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 30), derecha);
                y += 35;
                g.DrawString($"Pago: {_ventaPendiente.MetodoPago}", fontRegular, Brushes.Black, leftMargin, y);
                y += 20;

                // DATOS FISCALES (SI EXISTEN)
                if (!string.IsNullOrEmpty(_ventaPendiente.CAE))
                {
                    y += 10;
                    g.DrawString($"CAE: {_ventaPendiente.CAE}", fontBold, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), centro);
                    y += 20;
                    g.DrawString($"Vto CAE: {_ventaPendiente.CAEVencimiento:dd/MM/yyyy}", fontRegular, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), centro);
                    y += 20;
                }

                y += 20;
                g.DrawString("¡Gracias por su compra!", fontRegular, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), centro);
            }
            else if (_tipoDocumentoGrafico == "CIERRE")
            {
                // ... (Lógica de cierre gráfica, simplificada)
                g.DrawString("CIERRE DE CAJA", fontBold, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), centro);
                y += 40;
                g.DrawString($"Saldo Real: $ {_cajaPendiente.SaldoFinalReal:N2}", fontRegular, Brushes.Black, leftMargin, y);
                // ...
            }
        }

        // ==========================================
        //        MOTOR 2: ESC/POS (RAW)
        //        (Para Ticketeras Reales - Velocidad)
        // ==========================================

        private void EnviarAImpresoraRaw(byte[] bytes, string printerName)
        {
            if (!RawPrinterHelper.SendBytesToPrinter(printerName, bytes))
            {
                throw new Exception($"No se pudo enviar a la ticketera '{printerName}'. Verifique conexión.");
            }
        }

        private byte[] ConstruirTicketVentaBytes(Venta venta, List<DetalleVenta> detalles)
        {
            var b = ByteSplicer.Combine(
                Comandos.Center,
                _e.SetStyles(PrintStyle.Bold | PrintStyle.DoubleHeight),
                _e.PrintLine("VENDEMAX POS"),
                _e.SetStyles(PrintStyle.None),
                _e.PrintLine("Av. Siempre Viva 123"),
                _e.PrintLine("--------------------------------"),
                Comandos.Left,
                _e.PrintLine($"Fecha: {venta.Fecha:dd/MM/yyyy HH:mm}"),
                _e.PrintLine($"Cbte: {venta.TipoComprobante} - Nro: {venta.NumeroFactura}"),
                _e.PrintLine($"Cliente: {venta.Cliente?.NombreCompleto ?? "Consumidor Final"}"),
                _e.PrintLine("--------------------------------"),
                _e.SetStyles(PrintStyle.Bold),
                _e.PrintLine("CANT  DESCRIPCION      TOTAL"),
                _e.SetStyles(PrintStyle.None)
            );

            foreach (var item in detalles)
            {
                string cant = item.Cantidad.ToString().PadRight(4);
                string nombreProd = item.Producto?.Nombre ?? "Art";
                // Cortar nombre para que entre en 80mm
                string desc = nombreProd.Length > 16 ? nombreProd.Substring(0, 16) : nombreProd.PadRight(16);
                string total = item.Subtotal.ToString("N2").PadLeft(10);

                b = ByteSplicer.Combine(b, _e.PrintLine($"{cant} {desc} {total}"));
            }

            b = ByteSplicer.Combine(b,
                _e.PrintLine("--------------------------------"),
                Comandos.Right,
                _e.SetStyles(PrintStyle.Bold | PrintStyle.DoubleHeight),
                _e.PrintLine($"TOTAL: $ {venta.Total:N2}"),
                _e.SetStyles(PrintStyle.None),
                _e.FeedLines(1),
                Comandos.Left,
                _e.PrintLine($"Pago: {venta.MetodoPago}")
            );

            // DATOS FISCALES
            if (!string.IsNullOrEmpty(venta.CAE))
            {
                b = ByteSplicer.Combine(b,
                    _e.FeedLines(1),
                    Comandos.Center,
                    _e.SetStyles(PrintStyle.Bold),
                    _e.PrintLine($"CAE: {venta.CAE}"),
                    _e.PrintLine($"Vto: {venta.CAEVencimiento:dd/MM/yyyy}"),
                    _e.SetStyles(PrintStyle.None)
                );
            }

            b = ByteSplicer.Combine(b,
                Comandos.Center,
                _e.PrintLine("¡Gracias por su compra!"),
                _e.FeedLines(3),
                _e.FullCut()
            );

            return b;
        }

        private byte[] ConstruirTicketCierreBytes(Caja caja)
        {
            // Construcción RAW del cierre de caja...
            var b = ByteSplicer.Combine(
                Comandos.Center,
                _e.PrintLine("CIERRE DE CAJA"),
                _e.PrintLine("--------------------------------"),
                Comandos.Left,
                _e.PrintLine($"Sistema: $ {caja.SaldoFinalSistema:N2}"),
                _e.PrintLine($"Real:    $ {caja.SaldoFinalReal:N2}"),
                _e.PrintLine($"Dif:     $ {caja.Diferencia:N2}"),
                _e.FeedLines(3),
                _e.FullCut()
            );
            return b;
        }

        private byte[] ConstruirTicketMovimientoBytes(MovimientoCaja mov)
        {
            var b = ByteSplicer.Combine(
                Comandos.Center,
                _e.PrintLine("COMPROBANTE CAJA"),
                Comandos.Left,
                _e.PrintLine($"{mov.Tipo}: $ {mov.Monto:N2}"),
                _e.PrintLine(mov.Descripcion),
                _e.FeedLines(3),
                _e.FullCut()
            );
            return b;
        }
    }

    // Comandos manuales para evitar dependencia de versión
    public static class Comandos
    {
        public static byte[] Center => new byte[] { 0x1B, 0x61, 0x01 };
        public static byte[] Left => new byte[] { 0x1B, 0x61, 0x00 };
        public static byte[] Right => new byte[] { 0x1B, 0x61, 0x02 };
    }

    // Helper Win32 API para RAW Printing
    public static class RawPrinterHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        public static bool SendBytesToPrinter(string szPrinterName, byte[] pBytes)
        {
            IntPtr hPrinter;
            DOCINFOA di = new DOCINFOA { pDocName = "Ticket VENDEMAX", pDataType = "RAW" };
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(pBytes.Length);
                        Marshal.Copy(pBytes, 0, pUnmanagedBytes, pBytes.Length);
                        WritePrinter(hPrinter, pUnmanagedBytes, pBytes.Length, out int _);
                        Marshal.FreeCoTaskMem(pUnmanagedBytes);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
                return true;
            }
            return false;
        }
    }
}