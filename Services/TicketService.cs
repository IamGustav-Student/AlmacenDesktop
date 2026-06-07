using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;

namespace AlmacenDesktop.Services
{
    public class TicketService
    {
        private readonly EPSON _e;
        private string _nombreImpresora;

        private readonly string[] _keywordsTermicas = {
            "POS", "TICKET", "EPSON TM", "THERMAL", "RECEIPT", "GENERIC", "TEXT ONLY", "XPRINTER", "SAM4S", "BIXOLON"
        };

        private Venta _ventaPendiente;
        private List<DetalleVenta> _detallesPendientes;
        private Caja _cajaPendiente;
        private MovimientoCaja _movimientoPendiente;
        private string _tipoDocumentoGrafico;

        public TicketService()
        {
            _e = new EPSON();
            CargarImpresoraConfigurada();
        }

        private void CargarImpresoraConfigurada()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var config = context.DatosNegocio.FirstOrDefault();
                    if (config != null && !string.IsNullOrEmpty(config.NombreImpresora))
                    {
                        // VALIDACIÓN: Verificar si la impresora guardada realmente existe en Windows
                        if (PrinterSettings.InstalledPrinters.Cast<string>().Any(p => p == config.NombreImpresora))
                        {
                            _nombreImpresora = config.NombreImpresora;
                        }
                        else
                        {
                            // Si no existe, fallback
                            _nombreImpresora = "Microsoft Print to PDF";
                        }
                    }
                    else
                    {
                        _nombreImpresora = "Microsoft Print to PDF";
                    }
                }
            }
            catch
            {
                _nombreImpresora = "Microsoft Print to PDF";
            }
        }

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
                    // Si falla el modo RAW, intentamos gráfico como respaldo último
                    try { ImprimirGrafico(venta, detalles, null, null, "VENTA"); }
                    catch { throw new Exception("Fallo impresión RAW y Gráfica: " + ex.Message); }
                }
            }
            else
            {
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

        private bool EsImpresoraTermica(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return false;
            string nombreNormalizado = nombre.ToUpper();
            foreach (var key in _keywordsTermicas)
            {
                if (nombreNormalizado.Contains(key)) return true;
            }
            return false;
        }

        // ==========================================
        //        MOTOR 1: IMPRESIÓN GRÁFICA (GDI+)
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

            if (!pd.PrinterSettings.IsValid)
            {
                throw new Exception($"La impresora '{_nombreImpresora}' no es válida o está desconectada.");
            }

            pd.PrintPage += new PrintPageEventHandler(DibujarPagina);
            pd.Print();
        }

        private void DibujarPagina(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            // Usamos fuentes estándar de Windows para evitar errores si "Consolas" no existe
            Font fontTitulo = new Font("Arial", 12, FontStyle.Bold);
            Font fontRegular = new Font("Arial", 9, FontStyle.Regular);
            Font fontBold = new Font("Arial", 9, FontStyle.Bold);

            float y = 20;
            float leftMargin = 10;
            // Ajustamos ancho a 280 (aprox 72-80mm)
            float anchoTicket = 270;

            StringFormat centro = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat derecha = new StringFormat { Alignment = StringAlignment.Far };

            g.DrawString("VENDEMAX", fontTitulo, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 25), centro);
            y += 20;
            g.DrawLine(Pens.Black, leftMargin, y, leftMargin + anchoTicket, y);
            y += 10;

            if (_tipoDocumentoGrafico == "VENTA")
            {
                string tipoCbt = _ventaPendiente.TipoComprobante == "X" ? "NO FISCAL" : $"FACTURA {_ventaPendiente.TipoComprobante}";
                g.DrawString(tipoCbt, fontBold, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), centro);
                y += 20;

                g.DrawString($"Fecha: {_ventaPendiente.Fecha:dd/MM HH:mm}", fontRegular, Brushes.Black, leftMargin, y);
                y += 15;

                g.DrawString("Cant  Producto             Total", fontBold, Brushes.Black, leftMargin, y);
                y += 15;

                foreach (var item in _detallesPendientes)
                {
                    string nombre = item.Producto.Nombre;
                    if (nombre.Length > 15) nombre = nombre.Substring(0, 15);

                    g.DrawString($"{item.Cantidad}", fontRegular, Brushes.Black, leftMargin, y);
                    g.DrawString($"{nombre}", fontRegular, Brushes.Black, leftMargin + 25, y);
                    g.DrawString($"$ {item.Subtotal:N2}", fontRegular, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 20), derecha);
                    y += 15;
                }

                y += 10;
                g.DrawLine(Pens.Black, leftMargin, y, leftMargin + anchoTicket, y);
                y += 5;

                g.DrawString($"TOTAL: $ {_ventaPendiente.Total:N2}", fontTitulo, Brushes.Black, new RectangleF(leftMargin, y, anchoTicket, 30), derecha);
                y += 30;

                if (!string.IsNullOrEmpty(_ventaPendiente.CAE))
                {
                    g.DrawString($"CAE: {_ventaPendiente.CAE}", fontRegular, Brushes.Black, leftMargin, y);
                    y += 15;
                    g.DrawString($"Vto: {_ventaPendiente.CAEVencimiento:dd/MM/yyyy}", fontRegular, Brushes.Black, leftMargin, y);
                }
            }
            // ... (Resto de lógica gráfica similar)
        }

        // ==========================================
        //        MOTOR 2: ESC/POS (RAW)
        // ==========================================

        private void EnviarAImpresoraRaw(byte[] bytes, string printerName)
        {
            if (!RawPrinterHelper.SendBytesToPrinter(printerName, bytes))
            {
                throw new Exception($"Error enviando RAW a '{printerName}'. Verifique driver.");
            }
        }

        private byte[] ConstruirTicketVentaBytes(Venta venta, List<DetalleVenta> detalles)
        {
            // Construcción simplificada y robusta
            var b = ByteSplicer.Combine(
                Comandos.Center,
                _e.PrintLine("VENDEMAX"),
                _e.PrintLine("--------------------------------"),
                Comandos.Left,
                _e.PrintLine($"Fecha: {venta.Fecha:dd/MM/yy HH:mm}"),
                _e.PrintLine($"Doc: {venta.TipoComprobante} - {venta.NumeroFactura}"),
                _e.PrintLine("--------------------------------"),
                _e.PrintLine("CANT DESCRIPCION      TOTAL")
            );

            foreach (var item in detalles)
            {
                string cant = item.Cantidad.ToString().PadRight(3);
                string nombreProd = item.Producto?.Nombre ?? "Art";
                string desc = nombreProd.Length > 15 ? nombreProd.Substring(0, 15) : nombreProd.PadRight(15);
                string total = item.Subtotal.ToString("N2").PadLeft(10);

                b = ByteSplicer.Combine(b, _e.PrintLine($"{cant} {desc} {total}"));
            }

            b = ByteSplicer.Combine(b,
                _e.PrintLine("--------------------------------"),
                Comandos.Right,
                _e.SetStyles(PrintStyle.Bold),
                _e.PrintLine($"TOTAL: $ {venta.Total:N2}"),
                _e.SetStyles(PrintStyle.None),
                Comandos.Center,
                _e.FeedLines(3),
                _e.FullCut()
            );

            return b;
        }

        private byte[] ConstruirTicketCierreBytes(Caja caja)
        {
            return ByteSplicer.Combine(
                Comandos.Center,
                _e.PrintLine("CIERRE DE CAJA"),
                _e.PrintLine("----------------"),
                Comandos.Left,
                _e.PrintLine($"Real: $ {caja.SaldoFinalReal:N2}"),
                _e.FeedLines(3),
                _e.FullCut()
            );
        }

        private byte[] ConstruirTicketMovimientoBytes(MovimientoCaja mov)
        {
            return ByteSplicer.Combine(
                Comandos.Center,
                _e.PrintLine("MOVIMIENTO DE CAJA"),
                Comandos.Left,
                _e.PrintLine($"Monto: $ {mov.Monto:N2}"),
                _e.PrintLine($"Motivo: {mov.Descripcion}"),
                _e.FeedLines(3),
                _e.FullCut()
            );
        }
    }

    public static class Comandos
    {
        public static byte[] Center => new byte[] { 0x1B, 0x61, 0x01 };
        public static byte[] Left => new byte[] { 0x1B, 0x61, 0x00 };
        public static byte[] Right => new byte[] { 0x1B, 0x61, 0x02 };
    }

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
            if (OpenPrinter(szPrinterName, out hPrinter, IntPtr.Zero))
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