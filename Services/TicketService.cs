using AlmacenDesktop.Modelos;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing; // Necesario para Modo Gráfico
using System.Drawing.Printing; // Necesario para Modo Gráfico
using System.Runtime.InteropServices;
using System.Linq;

namespace AlmacenDesktop.Services
{
    // MOTOR DE IMPRESIÓN HÍBRIDO E INTELIGENTE
    // Detecta automáticamente si la impresora es Térmica (ESC/POS) o Tradicional (Tinta/Láser)
    public class TicketService
    {
        private readonly EPSON _e;

        // CONFIGURACIÓN: Nombre de tu impresora actual
        private string _nombreImpresora = "HP Smart Tank 580-590 series";

        // Palabras clave para detectar Ticketeras automáticamente
        private readonly string[] _keywordsTermicas = {
            "POS", "TICKET", "EPSON TM", "THERMAL", "RECEIPT", "GENERIC / TEXT ONLY", "XPRINTER", "SAM4S"
        };

        // Estado para impresión gráfica (paso de parámetros entre métodos)
        private Venta _ventaPendiente;
        private List<DetalleVenta> _detallesPendientes;
        private Caja _cajaPendiente;
        private MovimientoCaja _movimientoPendiente;
        private string _tipoDocumentoGrafico; // "VENTA", "CIERRE", "MOVIMIENTO"

        public TicketService()
        {
            _e = new EPSON();
        }

        // --- PUNTO DE ENTRADA INTELIGENTE ---

        public void ImprimirVenta(Venta venta, List<DetalleVenta> detalles)
        {
            if (EsImpresoraTermica(_nombreImpresora))
            {
                // MODO 1: TICKETERA (Velocidad Extrema, Corte, Cajón)
                try
                {
                    var bytes = ConstruirTicketVentaBytes(venta, detalles);
                    EnviarAImpresoraRaw(bytes, _nombreImpresora);
                }
                catch (Exception ex) { throw new Exception("Error Modo RAW: " + ex.Message); }
            }
            else
            {
                // MODO 2: TRADICIONAL (Inyección/Láser - Dibuja bonito y legible)
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

        // --- LÓGICA DE DETECCIÓN DE HARDWARE ---
        private bool EsImpresoraTermica(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return false;
            string nombreNormalizado = nombre.ToUpper();

            // Busca coincidencias en la lista de palabras clave
            foreach (var key in _keywordsTermicas)
            {
                if (nombreNormalizado.Contains(key)) return true;
            }
            return false; // Por defecto asumimos Tradicional para evitar imprimir símbolos raros
        }

        // ==========================================
        //        MOTOR 1: IMPRESIÓN GRÁFICA (GDI+)
        //        (Para tu HP Smart Tank)
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
            pd.PrintPage += new PrintPageEventHandler(DibujarPagina);

            try
            {
                pd.Print();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en impresión gráfica (Verifique que la impresora '{_nombreImpresora}' esté lista): {ex.Message}");
            }
        }

        private void DibujarPagina(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            // Fuentes estándar de Windows (se verán bien en cualquier impresora)
            Font fontTitulo = new Font("Arial", 14, FontStyle.Bold);
            Font fontSubtitulo = new Font("Arial", 12, FontStyle.Bold);
            Font fontRegular = new Font("Consolas", 10, FontStyle.Regular); // Consolas para alinear números
            Font fontBold = new Font("Consolas", 10, FontStyle.Bold);

            float y = 40;
            float leftMargin = 40;
            float anchoTicket = 300; // Simulamos un ancho de ticket visualmente en la hoja A4

            StringFormat centro = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat derecha = new StringFormat { Alignment = StringAlignment.Far };

            // --- DIBUJO COMÚN: CABECERA ---
            g.DrawString("MI ALMACEN", fontTitulo, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 25), centro);
            y += 25;
            g.DrawString("Av. Siempre Viva 123", fontRegular, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), centro);
            y += 30;
            g.DrawLine(Pens.Black, leftMargin, y, leftMargin + anchoTicket, y);
            y += 10;

            // --- CUERPO SEGÚN TIPO ---
            if (_tipoDocumentoGrafico == "VENTA")
            {
                g.DrawString($"Ticket #: {_ventaPendiente.Id}", fontBold, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawString($"Fecha: {_ventaPendiente.Fecha:dd/MM/yyyy HH:mm}", fontRegular, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawString($"Cliente: {_ventaPendiente.Cliente?.NombreCompleto ?? "-"}", fontRegular, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawLine(Pens.Black, leftMargin, y, leftMargin + anchoTicket, y);
                y += 10;

                // Detalles
                g.DrawString("CANT   PRODUCTO            TOTAL", fontBold, Brushes.Black, leftMargin, y);
                y += 20;

                foreach (var item in _detallesPendientes)
                {
                    string nombre = item.Producto.Nombre;
                    if (nombre.Length > 20) nombre = nombre.Substring(0, 20) + "..";

                    g.DrawString($"{item.Cantidad}", fontRegular, Brushes.Black, leftMargin, y);
                    g.DrawString($"{nombre}", fontRegular, Brushes.Black, leftMargin + 40, y);
                    g.DrawString($"$ {item.Subtotal:N2}", fontRegular, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), derecha);
                    y += 20;
                }

                y += 10;
                g.DrawLine(Pens.Black, leftMargin, y, leftMargin + anchoTicket, y);
                y += 10;
                g.DrawString($"TOTAL: $ {_ventaPendiente.Total:N2}", fontTitulo, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 30), derecha);
                y += 40;
                g.DrawString("¡Gracias por su compra!", fontRegular, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), centro);
            }
            else if (_tipoDocumentoGrafico == "CIERRE")
            {
                g.DrawString("CIERRE DE CAJA (Z)", fontTitulo, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 30), centro);
                y += 40;
                g.DrawString($"Caja ID: {_cajaPendiente.Id}", fontRegular, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawString($"Apertura: {_cajaPendiente.FechaApertura:dd/MM HH:mm}", fontRegular, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawLine(Pens.Black, leftMargin, y, leftMargin + anchoTicket, y);
                y += 10;

                g.DrawString($"Saldo Inicial: $ {_cajaPendiente.SaldoInicial:N2}", fontRegular, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawString($"Ventas Efvo:   $ {_cajaPendiente.TotalVentasEfectivo:N2}", fontRegular, Brushes.Black, leftMargin, y);
                y += 30;

                g.DrawString($"SISTEMA:    $ {_cajaPendiente.SaldoFinalSistema:N2}", fontBold, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawString($"REAL (Arqueo): $ {_cajaPendiente.SaldoFinalReal:N2}", fontBold, Brushes.Black, leftMargin, y);
                y += 20;

                string estado = _cajaPendiente.Diferencia == 0 ? "PERFECTO" : (_cajaPendiente.Diferencia > 0 ? "SOBRANTE" : "FALTANTE");
                g.DrawString($"DIFERENCIA: $ {_cajaPendiente.Diferencia:N2} ({estado})", fontTitulo, Brushes.Black, leftMargin, y);
            }
            else if (_tipoDocumentoGrafico == "MOVIMIENTO")
            {
                g.DrawString($"COMPROBANTE {_movimientoPendiente.Tipo}", fontTitulo, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 30), centro);
                y += 40;
                g.DrawString($"Fecha: {_movimientoPendiente.Fecha:dd/MM HH:mm}", fontRegular, Brushes.Black, leftMargin, y);
                y += 20;
                g.DrawString($"Monto: $ {_movimientoPendiente.Monto:N2}", fontTitulo, Brushes.Black, leftMargin, y);
                y += 30;
                g.DrawString($"Motivo: {_movimientoPendiente.Descripcion}", fontRegular, Brushes.Black, leftMargin, y);
            }
        }

        // ==========================================
        //        MOTOR 2: ESC/POS (RAW)
        //        (Para Ticketeras Reales)
        // ==========================================

        private void EnviarAImpresoraRaw(byte[] bytes, string printerName)
        {
            if (!RawPrinterHelper.SendBytesToPrinter(printerName, bytes))
            {
                throw new Exception($"No se pudo enviar a la ticketera '{printerName}'.");
            }
        }

        private byte[] ConstruirTicketVentaBytes(Venta venta, List<DetalleVenta> detalles)
        {
            var b = ByteSplicer.Combine(
                Comandos.Center,
                _e.SetStyles(PrintStyle.Bold | PrintStyle.DoubleHeight),
                _e.PrintLine("MI ALMACEN"),
                _e.SetStyles(PrintStyle.None),
                _e.PrintLine("Av. Siempre Viva 123"),
                _e.PrintLine("--------------------------------"),
                Comandos.Left,
                _e.PrintLine($"Fecha: {venta.Fecha:dd/MM/yyyy HH:mm}"),
                _e.PrintLine($"Ticket #: {venta.Id}"),
                _e.PrintLine($"Cliente: {venta.Cliente?.NombreCompleto ?? "-"}"),
                _e.PrintLine("--------------------------------"),
                _e.SetStyles(PrintStyle.Bold),
                _e.PrintLine("CANT  DESCRIPCION      TOTAL"),
                _e.SetStyles(PrintStyle.None)
            );

            foreach (var item in detalles)
            {
                string cant = item.Cantidad.ToString().PadRight(4);
                string nombreProd = item.Producto?.Nombre ?? "Art";
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
                _e.FeedLines(3),
                _e.FullCut()
            );
            return b;
        }

        private byte[] ConstruirTicketCierreBytes(Caja caja)
        {
            var b = ByteSplicer.Combine(
                Comandos.Center,
                _e.SetStyles(PrintStyle.Bold),
                _e.PrintLine("CIERRE DE CAJA (Z)"),
                _e.SetStyles(PrintStyle.None),
                _e.PrintLine("--------------------------------"),
                Comandos.Left,
                _e.PrintLine($"Caja ID: {caja.Id}"),
                _e.PrintLine($"Saldo Ini: $ {caja.SaldoInicial:N2}"),
                _e.PrintLine($"Ventas:    $ {caja.TotalVentasEfectivo:N2}"),
                _e.SetStyles(PrintStyle.Bold),
                _e.PrintLine($"REAL:      $ {caja.SaldoFinalReal:N2}"),
                _e.SetStyles(PrintStyle.None),
                _e.PrintLine($"Dif:       $ {caja.Diferencia:N2}"),
                _e.FeedLines(3),
                _e.FullCut()
            );
            return b;
        }

        private byte[] ConstruirTicketMovimientoBytes(MovimientoCaja mov)
        {
            var b = ByteSplicer.Combine(
                Comandos.Center,
                _e.PrintLine("MOVIMIENTO DE CAJA"),
                Comandos.Left,
                _e.PrintLine($"Tipo: {mov.Tipo}"),
                _e.PrintLine($"Monto: $ {mov.Monto:N2}"),
                _e.PrintLine($"Motivo: {mov.Descripcion}"),
                _e.FeedLines(3),
                _e.FullCut()
            );
            return b;
        }
    }

    // --- COMANDOS ESC/POS MANUALES ---
    public static class Comandos
    {
        public static byte[] Center => new byte[] { 0x1B, 0x61, 0x01 };
        public static byte[] Left => new byte[] { 0x1B, 0x61, 0x00 };
        public static byte[] Right => new byte[] { 0x1B, 0x61, 0x02 };
    }

    // --- RAW PRINTER HELPER (Win32 API) ---
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
            DOCINFOA di = new DOCINFOA { pDocName = "Ticket Sistema", pDataType = "RAW" };
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