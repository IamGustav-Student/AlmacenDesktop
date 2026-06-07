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
                        lblEstado.Text = "ESTADO: ABIERTA";
                        lblEstado.ForeColor = Color.Green;

                        lblMonto.Text = "Saldo Real en Caja (Contar dinero):";
                        numMonto.Value = 0; // Limpiar para que el usuario ingrese lo que cuenta
                        numMonto.Enabled = true;

                        btnAccion.Text = "CERRAR CAJA Y SALIR";
                        btnAccion.BackColor = Color.Firebrick;
                        btnAccion.ForeColor = Color.White;

                        // Habilitar botón de movimientos si la caja está abierta
                        if (btnRegistrarMovimiento != null) btnRegistrarMovimiento.Enabled = true;

                        // Cargar resumen de ventas en la grilla
                        CargarVentas(context);
                    }
                    else
                    {
                        // --- MODO: CAJA CERRADA (Listo para Abrir) ---
                        lblEstado.Text = "ESTADO: CERRADA";
                        lblEstado.ForeColor = Color.Red;

                        lblMonto.Text = "Monto Inicial (Cambio):";
                        numMonto.Value = 0;
                        numMonto.Enabled = true;

                        btnAccion.Text = "ABRIR CAJA";
                        btnAccion.BackColor = Color.ForestGreen;
                        btnAccion.ForeColor = Color.White;

                        // Deshabilitar movimientos si no hay caja
                        if (btnRegistrarMovimiento != null) btnRegistrarMovimiento.Enabled = false;

                        dgvVentasCaja.DataSource = null;
                        lblInfo.Text = "Esperando apertura...";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar estado: " + ex.Message);
            }
        }

        private void CargarVentas(AlmacenDbContext context)
        {
            var ventas = context.Ventas
                .Where(v => v.CajaId == _cajaActual.Id)
                .Select(v => new {
                    Hora = v.Fecha.ToString("HH:mm"),
                    Total = v.Total,
                    Pago = v.MetodoPago
                })
                .ToList();

            dgvVentasCaja.DataSource = ventas;

            decimal totalVendido = ventas.Sum(v => v.Total);
            lblInfo.Text = $"Ventas Turno: {ventas.Count} | Total Sistema: {totalVendido:C2}";
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
                MessageBox.Show("Error al abrir: " + ex.Message);
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

                    // Calcular Totales del Sistema
                    decimal totalVentas = context.Ventas
                        .Where(v => v.CajaId == cajaDb.Id)
                        .Sum(v => (decimal?)v.Total) ?? 0;

                    decimal ingresos = context.MovimientosCaja
                        .Where(m => m.CajaId == cajaDb.Id && m.Tipo == "INGRESO")
                        .Sum(m => (decimal?)m.Monto) ?? 0;

                    decimal egresos = context.MovimientosCaja
                        .Where(m => m.CajaId == cajaDb.Id && m.Tipo == "EGRESO")
                        .Sum(m => (decimal?)m.Monto) ?? 0;

                    cajaDb.SaldoFinalSistema = cajaDb.SaldoInicial + totalVentas + ingresos - egresos;
                    cajaDb.SaldoFinalReal = numMonto.Value;
                    cajaDb.Diferencia = cajaDb.SaldoFinalReal - cajaDb.SaldoFinalSistema;
                    cajaDb.FechaCierre = DateTime.Now;

                    context.SaveChanges();

                    // 1. BACKUP AUTOMÁTICO
                    try
                    {
                        _backupService.RealizarBackupAutomatico();
                    }
                    catch (Exception exBackup)
                    {
                        MessageBox.Show("Caja cerrada, pero falló el backup: " + exBackup.Message);
                    }

                    // 2. IMPRESIÓN TICKET Z
                    try
                    {
                        _ticketService.ImprimirCierreCaja(cajaDb);
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
                MessageBox.Show("Error crítico al cerrar: " + ex.Message);
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