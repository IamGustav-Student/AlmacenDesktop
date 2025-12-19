using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services; // Importante para usar el BackupService
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ControlCajaForm : Form
    {
        private Usuario _usuario;
        private Caja _cajaActual;

        public ControlCajaForm(Usuario usuario)
        {
            _usuario = usuario;
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            VerificarEstadoCaja();
        }

        private void BtnRegistrarMovimiento_Click(object sender, EventArgs e)
        {
            if (_cajaActual == null || !_cajaActual.EstaAbierta) return;
            MovimientoCajaForm form = new MovimientoCajaForm(_cajaActual, _usuario);
            form.ShowDialog();
            VerificarEstadoCaja();
        }

        private void VerificarEstadoCaja()
        {
            using (var context = new AlmacenDbContext())
            {
                _cajaActual = context.Cajas
                    .AsNoTracking()
                    .FirstOrDefault(c => c.UsuarioId == _usuario.Id && c.EstaAbierta);

                if (_cajaActual == null)
                {
                    lblEstado.Text = "ESTADO: CAJA CERRADA";
                    lblEstado.ForeColor = Color.Red;
                    lblInfo.Text = "Ingrese el saldo inicial:";
                    btnAccion.Text = "ABRIR CAJA";
                    btnAccion.BackColor = Color.ForestGreen;
                    grpResumen.Visible = false;
                    btnRegistrarMovimiento.Visible = false;
                }
                else
                {
                    lblEstado.Text = "ESTADO: CAJA ABIERTA";
                    lblEstado.ForeColor = Color.Green;
                    lblInfo.Text = "Ingrese el efectivo REAL:";
                    btnAccion.Text = "CERRAR CAJA (Arqueo)";
                    btnAccion.BackColor = Color.OrangeRed;
                    grpResumen.Visible = true;
                    btnRegistrarMovimiento.Visible = true;
                    CalcularTotalesCierre(context);
                }
            }
        }

        private void CalcularTotalesCierre(AlmacenDbContext context)
        {
            var ventasCaja = context.Ventas.AsNoTracking().Where(v => v.CajaId == _cajaActual.Id).ToList();
            var movimientos = context.MovimientosCaja.AsNoTracking().Where(m => m.CajaId == _cajaActual.Id).ToList();

            decimal totalEfectivoVentas = ventasCaja.Where(v => v.MetodoPago == "Efectivo").Sum(v => v.Total);
            decimal totalIngresosManuales = movimientos.Where(m => m.Tipo == "INGRESO").Sum(m => m.Monto);
            decimal totalEgresosManuales = movimientos.Where(m => m.Tipo == "EGRESO").Sum(m => m.Monto);

            _cajaActual.TotalVentasEfectivo = totalEfectivoVentas;
            _cajaActual.SaldoFinalSistema = _cajaActual.SaldoInicial + totalEfectivoVentas + totalIngresosManuales - totalEgresosManuales;

            lblResumenDetalle.Text = $"Sistema dice: $ {_cajaActual.SaldoFinalSistema:N2}";
        }

        private void btnAccion_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    if (_cajaActual == null) // ABRIR
                    {
                        var nuevaCaja = new Caja
                        {
                            UsuarioId = _usuario.Id,
                            FechaApertura = DateTime.Now,
                            SaldoInicial = numMonto.Value,
                            EstaAbierta = true
                        };
                        context.Cajas.Add(nuevaCaja);
                        context.SaveChanges();
                        MessageBox.Show("Caja Abierta.");
                        VerificarEstadoCaja();
                    }
                    else // CERRAR
                    {
                        // 1. Lógica de Cierre
                        CalcularTotalesCierre(context);

                        var cajaDb = context.Cajas.Find(_cajaActual.Id);
                        cajaDb.FechaCierre = DateTime.Now;
                        cajaDb.EstaAbierta = false;
                        cajaDb.SaldoFinalSistema = _cajaActual.SaldoFinalSistema;
                        cajaDb.TotalVentasEfectivo = _cajaActual.TotalVentasEfectivo;
                        cajaDb.SaldoFinalReal = numMonto.Value;
                        cajaDb.Diferencia = numMonto.Value - _cajaActual.SaldoFinalSistema;

                        context.SaveChanges(); // Guardamos el cierre en BD

                        // 2. BACKUP AUTOMÁTICO (Aquí está la magia)
                        // Lo hacemos dentro de un try-catch pequeño para que si falla el backup,
                        // NO impida que la caja se cierre (la caja ya se cerró en la línea anterior).
                        try
                        {
                            BackupService.RealizarBackupAutomatico();
                            // Opcional: MessageBox.Show("Backup realizado correctamente."); 
                            // (Generalmente no se avisa si sale bien para no molestar, solo si sale mal)
                        }
                        catch (Exception exBackup)
                        {
                            MessageBox.Show("⚠️ La caja se cerró, pero HUBO UN ERROR EN EL BACKUP:\n" + exBackup.Message, "Alerta de Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        // 3. Impresión de Ticket Z
                        if (MessageBox.Show("Caja Cerrada Exitosamente. ¿Imprimir Ticket Z?", "Imprimir", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            new TicketService().ImprimirCierreCaja(cajaDb);
                        }

                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error crítico: " + ex.Message);
            }
        }
    }
}