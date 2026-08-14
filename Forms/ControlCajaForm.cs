using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services; // Importante
using AlmacenDesktop.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ControlCajaForm : Form
    {
        private Usuario _usuarioActual;
        private Caja _cajaActual;

        // SERVICIOS
        private TicketService _ticketService;
        private BackupService _backupService;

        public ControlCajaForm(Usuario usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            // Inicializamos los servicios
            _ticketService = new TicketService();
            _backupService = new BackupService();
        }

        private void ControlCajaForm_Load(object sender, EventArgs e)
        {
            VerificarEstadoCaja();
        }

        // Método centralizado para saber si abrimos o cerramos
        private void VerificarEstadoCaja()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    // Buscamos caja abierta del usuario
                    _cajaActual = context.Cajas
                        .Where(c => c.UsuarioId == _usuarioActual.Id && c.FechaCierre == null)
                        .OrderByDescending(c => c.FechaApertura)
                        .FirstOrDefault();

                    if (_cajaActual != null)
                    {
                        // --- MODO: CAJA ABIERTA (Listo para Cerrar) ---
                        lblEstado.Text = "ESTADO: CAJA ABIERTA";
                        lblEstado.ForeColor = Color.Green;

                        // El detalle largo va en lblInfo (ancho completo); lblMonto tiene que
                        // quedar corto porque numMonto arranca a 80px de él y lo pisaría.
                        lblMonto.Text = "Contado ($):";
                        numMonto.Value = 0; // Limpiar para que el usuario ingrese lo que cuenta
                        numMonto.Enabled = true;

                        btnAccion.Text = "CERRAR CAJA Y SALIR";
                        btnAccion.BackColor = Color.Firebrick;
                        btnAccion.ForeColor = Color.White;

                        // El Designer los crea ocultos: se muestran solo con la caja abierta.
                        grpResumen.Visible = true;
                        btnRegistrarMovimiento.Visible = true;
                        btnRegistrarMovimiento.Enabled = true;

                        // Cargar resumen del turno (ventas + gastos) en la grilla
                        CargarResumen(context);
                    }
                    else
                    {
                        // --- MODO: CAJA CERRADA (Listo para Abrir) ---
                        lblEstado.Text = "ESTADO: CAJA CERRADA";
                        lblEstado.ForeColor = Color.Red;

                        lblMonto.Text = "Monto ($):";
                        numMonto.Value = 0;
                        numMonto.Enabled = true;

                        btnAccion.Text = "ABRIR CAJA";
                        btnAccion.BackColor = Color.ForestGreen;
                        btnAccion.ForeColor = Color.White;

                        grpResumen.Visible = false;
                        btnRegistrarMovimiento.Visible = false;
                        btnRegistrarMovimiento.Enabled = false;

                        dgvVentasCaja.DataSource = null;
                        lblInfo.Text = "Ingrese el saldo inicial para comenzar:";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar estado: " + ExceptionHelper.ObtenerMensaje(ex));
            }
        }

        // Muestra el movimiento real de plata del turno: las ventas y también los
        // gastos/retiros, que antes se sumaban al cerrar pero no se veían en ningún lado.
        private void CargarResumen(AlmacenDbContext context)
        {
            var ventas = context.Ventas
                .Where(v => v.CajaId == _cajaActual.Id)
                .Select(v => new { v.Fecha, v.Total, v.MetodoPago })
                .ToList();

            var movimientos = context.MovimientosCaja
                .Where(m => m.CajaId == _cajaActual.Id)
                .Select(m => new { m.Fecha, m.Monto, m.Tipo, m.Descripcion })
                .ToList();

            // Una sola lista cronológica: la salida se ve con su motivo ("Pago pan"),
            // en negativo, mezclada entre las ventas tal como ocurrió en el turno.
            var filas = ventas
                .Select(v => new
                {
                    Orden = v.Fecha,
                    Hora = v.Fecha.ToString("HH:mm"),
                    Detalle = $"Venta ({v.MetodoPago})",
                    Monto = v.Total
                })
                .Concat(movimientos.Select(m => new
                {
                    Orden = m.Fecha,
                    Hora = m.Fecha.ToString("HH:mm"),
                    Detalle = m.Descripcion,
                    Monto = m.Tipo == "EGRESO" ? -m.Monto : m.Monto
                }))
                .OrderBy(x => x.Orden)
                .Select(x => new { x.Hora, x.Detalle, x.Monto })
                .ToList();

            dgvVentasCaja.DataSource = filas;
            if (dgvVentasCaja.Columns["Monto"] != null)
            {
                dgvVentasCaja.Columns["Monto"].DefaultCellStyle.Format = "C2";
                dgvVentasCaja.Columns["Monto"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            decimal efectivo = ventas.Where(v => v.MetodoPago == "Efectivo").Sum(v => v.Total);
            decimal otrosMedios = ventas.Where(v => v.MetodoPago != "Efectivo").Sum(v => v.Total);
            decimal ingresos = movimientos.Where(m => m.Tipo == "INGRESO").Sum(m => m.Monto);
            decimal egresos = movimientos.Where(m => m.Tipo == "EGRESO").Sum(m => m.Monto);
            decimal esperado = _cajaActual.SaldoInicial + efectivo + ingresos - egresos;

            // Solo lo que realmente entra o sale del cajón. Tarjeta, transferencia y
            // cuenta corriente se informan aparte porque no son plata en el cajón.
            lblResumenDetalle.Text =
                $"{"Saldo inicial:",-19}{_cajaActual.SaldoInicial,12:C2}\n" +
                $"{"+ Ventas efectivo:",-19}{efectivo,12:C2}\n" +
                $"{"+ Otros ingresos:",-19}{ingresos,12:C2}\n" +
                $"{"- Salidas / gastos:",-19}{egresos,12:C2}\n" +
                $"{"ESPERADO EN CAJA:",-19}{esperado,12:C2}";

            lblInfo.Text = $"Abierta {_cajaActual.FechaApertura:dd/MM HH:mm} · " +
                           $"{ventas.Count} ventas · No efectivo: {otrosMedios:C2}";
        }

        private void btnAccion_Click(object sender, EventArgs e)
        {
            if (_cajaActual == null)
            {
                AbrirCaja();
            }
            else
            {
                CerrarCaja();
            }
        }

        private void AbrirCaja()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    if (context.Cajas.Any(c => c.UsuarioId == _usuarioActual.Id && c.FechaCierre == null))
                    {
                        MessageBox.Show("Ya tienes una caja abierta.");
                        return;
                    }

                    var nuevaCaja = new Caja
                    {
                        UsuarioId = _usuarioActual.Id,
                        FechaApertura = DateTime.Now,
                        SaldoInicial = numMonto.Value,
                        SaldoFinalSistema = 0,
                        SaldoFinalReal = 0,
                        Diferencia = 0
                    };

                    context.Cajas.Add(nuevaCaja);
                    context.SaveChanges();

                    AudioHelper.PlayOk();
                    MessageBox.Show("¡Caja Abierta Correctamente!");
                    VerificarEstadoCaja();
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Error al abrir: " + ExceptionHelper.ObtenerMensaje(ex));
            }
        }

        private void CerrarCaja()
        {
            if (MessageBox.Show("¿Seguro que desea cerrar la caja?\nEsta acción es irreversible.", "Confirmar Cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var cajaDb = context.Cajas.Find(_cajaActual.Id);
                    if (cajaDb == null)
                    {
                        MessageBox.Show("Error: No se encontró el registro de la caja en la base de datos.", "Error de Caja", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // El saldo esperado es la plata que tiene que haber EN EL CAJÓN, y se
                    // compara contra lo que el usuario contó a mano. Por eso solo cuentan
                    // las ventas en efectivo: transferencia, billetera virtual y cuenta
                    // corriente no ponen un peso en el cajón (la cuenta corriente ni
                    // siquiera se cobró todavía). Sumarlas marcaba un faltante inexistente.
                    decimal ventasEfectivo = context.Ventas
                        .Where(v => v.CajaId == cajaDb.Id && v.MetodoPago == "Efectivo")
                        .Sum(v => (decimal?)v.Total) ?? 0;

                    decimal ventasOtros = context.Ventas
                        .Where(v => v.CajaId == cajaDb.Id && v.MetodoPago != "Efectivo")
                        .Sum(v => (decimal?)v.Total) ?? 0;

                    decimal ingresos = context.MovimientosCaja
                        .Where(m => m.CajaId == cajaDb.Id && m.Tipo == "INGRESO")
                        .Sum(m => (decimal?)m.Monto) ?? 0;

                    decimal egresos = context.MovimientosCaja
                        .Where(m => m.CajaId == cajaDb.Id && m.Tipo == "EGRESO")
                        .Sum(m => (decimal?)m.Monto) ?? 0;

                    // Se recalculan desde las ventas reales para que el registro cerrado
                    // quede consistente aunque los acumuladores hayan quedado desfasados.
                    cajaDb.TotalVentasEfectivo = ventasEfectivo;
                    cajaDb.TotalVentasOtros = ventasOtros;

                    cajaDb.SaldoFinalSistema = cajaDb.SaldoInicial + ventasEfectivo + ingresos - egresos;
                    cajaDb.SaldoFinalReal = numMonto.Value;
                    cajaDb.Diferencia = cajaDb.SaldoFinalReal - cajaDb.SaldoFinalSistema;
                    cajaDb.FechaCierre = DateTime.Now;

                    // La caja también se marca cerrada por bandera: hay pantallas que
                    // filtran por EstaAbierta y no por FechaCierre (ej. Historial de Cajas).
                    cajaDb.EstaAbierta = false;

                    var movimientosDelTurno = context.MovimientosCaja
                        .Where(m => m.CajaId == cajaDb.Id)
                        .OrderBy(m => m.Fecha)
                        .ToList();

                    context.SaveChanges();

                    // 1. BACKUP AUTOMÁTICO
                    try
                    {
                        _backupService.RealizarBackupAutomatico();
                    }
                    catch (Exception exBackup)
                    {
                        MessageBox.Show("Caja cerrada, pero falló el backup: " + ExceptionHelper.ObtenerMensaje(exBackup));
                    }

                    // 2. IMPRESIÓN TICKET Z
                    try
                    {
                        _ticketService.ImprimirCierreCaja(cajaDb, movimientosDelTurno);
                    }
                    catch { /* Ignorar error de impresión */ }

                    AudioHelper.PlayOk();

                    string mensajeResumen = $"Caja Cerrada.\n\n" +
                                          $"Sistema: {cajaDb.SaldoFinalSistema:C2}\n" +
                                          $"Real: {cajaDb.SaldoFinalReal:C2}\n" +
                                          $"Diferencia: {cajaDb.Diferencia:C2}";

                    if (cajaDb.Diferencia != 0)
                        MessageBox.Show(mensajeResumen, "Cierre con Diferencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        MessageBox.Show(mensajeResumen, "Cierre Perfecto", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    VerificarEstadoCaja();
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Error crítico al cerrar: " + ExceptionHelper.ObtenerMensaje(ex));
            }
        }

        // --- CORRECCIÓN AQUÍ: NOMBRE DEL MÉTODO EN MAYÚSCULA ---
        // Esto coincide con lo que busca el Designer.cs
        private void BtnRegistrarMovimiento_Click(object sender, EventArgs e)
        {
            if (_cajaActual != null)
            {
                // Abrimos el formulario de movimientos pasando la caja y el usuario
                var frm = new MovimientoCajaForm(_cajaActual, _usuarioActual);
                frm.ShowDialog();
                VerificarEstadoCaja(); // Recargar montos al volver
            }
            else
            {
                MessageBox.Show("Debe abrir la caja primero para registrar movimientos.");
            }
        }
    }
}