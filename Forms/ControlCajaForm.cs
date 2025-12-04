using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
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
            // Ya no llamamos a ConfigurarBotonMovimiento() porque está en el Designer
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            VerificarEstadoCaja();
        }

        private void BtnRegistrarMovimiento_Click(object sender, EventArgs e)
        {
            if (_cajaActual == null || !_cajaActual.EstaAbierta)
            {
                MessageBox.Show("Debe abrir la caja primero.");
                return;
            }

            MovimientoCajaForm form = new MovimientoCajaForm(_cajaActual, _usuario);
            form.ShowDialog();

            // Al volver, recalculamos los totales
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
                    lblInfo.Text = "Ingrese el saldo inicial (cambio) para comenzar:";
                    btnAccion.Text = "ABRIR CAJA";
                    btnAccion.BackColor = Color.ForestGreen;
                    grpResumen.Visible = false;
                    btnRegistrarMovimiento.Visible = false;

                    if (numMonto.Value != 0 && !numMonto.Focused) numMonto.Value = 0;
                }
                else
                {
                    lblEstado.Text = "ESTADO: CAJA ABIERTA (Turno Activo)";
                    lblEstado.ForeColor = Color.Green;
                    lblInfo.Text = "Ingrese el efectivo REAL que cuenta en caja:";
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
            // 1. VENTAS
            var ventasCaja = context.Ventas
                .AsNoTracking()
                .Where(v => v.CajaId == _cajaActual.Id)
                .OrderByDescending(v => v.Fecha)
                .ToList();

            // 2. MOVIMIENTOS (GASTOS Y ENTRADAS)
            var movimientos = context.MovimientosCaja
                .AsNoTracking()
                .Where(m => m.CajaId == _cajaActual.Id)
                .ToList();

            decimal totalEfectivoVentas = ventasCaja.Where(v => v.MetodoPago == "Efectivo").Sum(v => v.Total);

            // Calcular Entradas y Salidas manuales
            decimal totalIngresosManuales = movimientos.Where(m => m.Tipo == "INGRESO").Sum(m => m.Monto);
            decimal totalEgresosManuales = movimientos.Where(m => m.Tipo == "EGRESO").Sum(m => m.Monto);

            // --- FÓRMULA MAESTRA DE CAJA ---
            decimal saldoSistema = _cajaActual.SaldoInicial + totalEfectivoVentas + totalIngresosManuales - totalEgresosManuales;

            lblResumenDetalle.Text =
                $"Saldo Inicial:     $ {_cajaActual.SaldoInicial:N2}\n" +
                $"Ventas Efectivo:   + $ {totalEfectivoVentas:N2}\n" +
                $"Ingresos Extras:   + $ {totalIngresosManuales:N2}\n" +
                $"Gastos / Retiros:  - $ {totalEgresosManuales:N2}\n" +
                $"-----------------------------\n" +
                $"EN CAJA DEBE HABER: $ {saldoSistema:N2}";

            // Llenar grilla combinada
            var listaVentas = ventasCaja.Select(v => new {
                Hora = v.Fecha.ToString("HH:mm"),
                Concepto = "Venta #" + v.Id,
                Ingreso = v.MetodoPago == "Efectivo" ? v.Total : 0,
                Egreso = 0m
            });

            var listaMovs = movimientos.Select(m => new {
                Hora = m.Fecha.ToString("HH:mm"),
                Concepto = m.Tipo + ": " + m.Descripcion,
                Ingreso = m.Tipo == "INGRESO" ? m.Monto : 0,
                Egreso = m.Tipo == "EGRESO" ? m.Monto : 0
            });

            var listaUnificada = listaVentas.Concat(listaMovs)
                                            .OrderByDescending(x => x.Hora)
                                            .ToList();

            dgvVentasCaja.DataSource = null;
            dgvVentasCaja.DataSource = listaUnificada;

            if (dgvVentasCaja.Columns["Ingreso"] != null) dgvVentasCaja.Columns["Ingreso"].DefaultCellStyle.Format = "C2";
            if (dgvVentasCaja.Columns["Egreso"] != null) dgvVentasCaja.Columns["Egreso"].DefaultCellStyle.Format = "C2";

            // Estilos de grilla para diferenciar visualmente gastos de ingresos
            dgvVentasCaja.Columns["Ingreso"].DefaultCellStyle.ForeColor = Color.Green;
            dgvVentasCaja.Columns["Egreso"].DefaultCellStyle.ForeColor = Color.Red;

            _cajaActual.SaldoFinalSistema = saldoSistema;
        }

        private void btnAccion_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    if (_cajaActual == null)
                    {
                        if (numMonto.Value < 0) { MessageBox.Show("Saldo inválido"); return; }

                        var nuevaCaja = new Caja
                        {
                            UsuarioId = _usuario.Id,
                            FechaApertura = DateTime.Now,
                            SaldoInicial = numMonto.Value,
                            EstaAbierta = true
                        };
                        context.Cajas.Add(nuevaCaja);
                        context.SaveChanges();

                        MessageBox.Show("¡Caja Abierta!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        VerificarEstadoCaja();
                    }
                    else
                    {
                        var cajaDb = context.Cajas.Find(_cajaActual.Id);
                        CalcularTotalesCierre(context);

                        decimal saldoReal = numMonto.Value;
                        decimal diferencia = saldoReal - _cajaActual.SaldoFinalSistema;

                        cajaDb.FechaCierre = DateTime.Now;
                        cajaDb.EstaAbierta = false;
                        cajaDb.SaldoFinalSistema = _cajaActual.SaldoFinalSistema;
                        cajaDb.SaldoFinalReal = saldoReal;
                        cajaDb.Diferencia = diferencia;

                        context.SaveChanges();

                        string msj = diferencia == 0 ? "Exacto." : diferencia > 0 ? $"Sobra ${diferencia:N2}" : $"Faltan ${diferencia:N2}";
                        MessageBox.Show($"Caja Cerrada.\n{msj}", "Fin Turno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}