using AlmacenDesktop.Data;
using AlmacenDesktop.Helpers;
using AlmacenDesktop.Modelos; // Importante: Ahora usa el modelo compartido
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class CuentaCorrienteForm : Form
    {
        private Cliente _clienteSeleccionado;

        public CuentaCorrienteForm()
        {
            InitializeComponent();
        }

        private void CuentaCorrienteForm_Load(object sender, EventArgs e)
        {
            CargarClientes();
        }

        private void CargarClientes()
        {
            using (var context = new AlmacenDbContext())
            {
                var clientes = context.Clientes
                    .Where(c => c.DniCuit != Constantes.CLIENTE_DEF_DNI)
                    .OrderBy(c => c.Apellido)
                    .ToList();

                cboClientes.DataSource = clientes;
                cboClientes.DisplayMember = "NombreCompleto";
                cboClientes.ValueMember = "Id";
                cboClientes.SelectedIndex = -1;
            }
        }

        private void cboClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboClientes.SelectedItem != null)
            {
                _clienteSeleccionado = (Cliente)cboClientes.SelectedItem;
                CargarMovimientos();
            }
            else
            {
                LimpiarPantalla();
            }
        }

        private void CargarMovimientos()
        {
            if (_clienteSeleccionado == null) return;

            using (var context = new AlmacenDbContext())
            {
                // 1. VENTAS (Agregamos VentaId oculto)
                var ventasFiado = context.Ventas
                    .AsNoTracking()
                    .Where(v => v.ClienteId == _clienteSeleccionado.Id && v.MetodoPago == "Fiado")
                    .Select(v => new MovimientoCtaCte
                    {
                        Fecha = v.Fecha,
                        Tipo = "COMPRA",
                        Descripcion = $"Compra (Venta #{v.Id})",
                        Debe = v.Total,
                        Haber = 0,
                        VentaId = v.Id
                    })
                    .ToList();

                // 2. PAGOS
                var pagos = context.Pagos
                    .AsNoTracking()
                    .Where(p => p.ClienteId == _clienteSeleccionado.Id)
                    .Select(p => new MovimientoCtaCte
                    {
                        Fecha = p.Fecha,
                        Tipo = "PAGO",
                        Descripcion = "Pago a Cuenta",
                        Debe = 0,
                        Haber = p.Monto,
                        VentaId = null
                    })
                    .ToList();

                var historia = ventasFiado.Concat(pagos).OrderBy(x => x.Fecha).ToList();

                decimal saldo = 0;
                foreach (var mov in historia)
                {
                    saldo += (mov.Debe - mov.Haber);
                    // CAMBIO: Antes era SaldoParcial, ahora es Saldo (unificado)
                    mov.Saldo = saldo;
                }

                dgvHistoria.DataSource = null;
                dgvHistoria.DataSource = historia;
                FormatearGrilla();

                lblSaldo.Text = saldo.ToString(Constantes.MONEDA_FMT);
                ActualizarEstadoSaldo(saldo);
            }
        }

        private void ActualizarEstadoSaldo(decimal saldo)
        {
            if (saldo > 0)
            {
                lblSaldo.ForeColor = Color.Red;
                lblEstadoDeuda.Text = "DEUDA PENDIENTE";
                lblEstadoDeuda.ForeColor = Color.Red;
            }
            else if (saldo < 0)
            {
                lblSaldo.ForeColor = Color.Green;
                lblEstadoDeuda.Text = "SALDO A FAVOR";
                lblEstadoDeuda.ForeColor = Color.Green;
            }
            else
            {
                lblSaldo.ForeColor = Color.Black;
                lblEstadoDeuda.Text = "AL DÍA";
                lblEstadoDeuda.ForeColor = Color.Gray;
            }
        }

        private void dgvHistoria_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var movimiento = (MovimientoCtaCte)dgvHistoria.Rows[e.RowIndex].DataBoundItem;

            if (movimiento.VentaId.HasValue)
            {
                MostrarDetalleVenta(movimiento.VentaId.Value);
            }
        }

        private void MostrarDetalleVenta(int ventaId)
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var detalles = context.DetallesVenta
                        .AsNoTracking()
                        .Include(d => d.Producto)
                        .Where(d => d.VentaId == ventaId)
                        .ToList();

                    if (detalles.Count > 0)
                    {
                        string mensaje = $"Detalle de Venta #{ventaId}\n\n";
                        mensaje += "CANT  PRODUCTO           SUBTOTAL\n";
                        mensaje += "-----------------------------------\n";

                        foreach (var d in detalles)
                        {
                            mensaje += $"{d.Cantidad} x {d.Producto.Nombre}  ({d.Subtotal.ToString("C2")})\n";
                        }

                        mensaje += "\n-----------------------------------\n";
                        decimal total = detalles.Sum(d => d.Subtotal);
                        mensaje += $"TOTAL: {total.ToString("C2")}";

                        MessageBox.Show(mensaje, "Detalle de Compra", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar el detalle: " + ex.Message);
            }
        }

        private void btnRegistrarPago_Click(object sender, EventArgs e)
        {
            if (_clienteSeleccionado == null)
            {
                MessageBox.Show("Seleccione un cliente.");
                return;
            }

            if (numMontoPago.Value <= 0)
            {
                MessageBox.Show("Ingrese un monto mayor a 0.");
                return;
            }

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var nuevoPago = new Pago
                    {
                        Fecha = DateTime.Now,
                        ClienteId = _clienteSeleccionado.Id,
                        Monto = numMontoPago.Value,
                        UsuarioId = Program.UsuarioActualGlobal.Id
                    };
                    context.Pagos.Add(nuevoPago);

                    var cajaAbierta = context.Cajas.FirstOrDefault(c => c.UsuarioId == Program.UsuarioActualGlobal.Id && c.EstaAbierta);
                    if (cajaAbierta != null)
                    {
                        var movimientoCaja = new MovimientoCaja
                        {
                            Fecha = DateTime.Now,
                            CajaId = cajaAbierta.Id,
                            UsuarioId = Program.UsuarioActualGlobal.Id,
                            Tipo = "INGRESO",
                            Monto = numMontoPago.Value,
                            Descripcion = $"Cobro Fiado - {_clienteSeleccionado.Apellido}"
                        };
                        context.MovimientosCaja.Add(movimientoCaja);
                    }

                    context.SaveChanges();

                    MessageBox.Show("Pago registrado correctamente.");
                    numMontoPago.Value = 0;
                    CargarMovimientos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void LimpiarPantalla()
        {
            _clienteSeleccionado = null;
            dgvHistoria.DataSource = null;
            lblSaldo.Text = "$ 0.00";
            lblEstadoDeuda.Text = "-";
            lblEstadoDeuda.ForeColor = Color.Black;
        }

        private void FormatearGrilla()
        {
            // Validar que las columnas existen antes de formatear para evitar crashes
            if (dgvHistoria.Columns["Fecha"] != null) dgvHistoria.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

            if (dgvHistoria.Columns["Debe"] != null)
            {
                dgvHistoria.Columns["Debe"].DefaultCellStyle.Format = "C2";
                dgvHistoria.Columns["Debe"].DefaultCellStyle.ForeColor = Color.Red;
            }

            if (dgvHistoria.Columns["Haber"] != null)
            {
                dgvHistoria.Columns["Haber"].DefaultCellStyle.Format = "C2";
                dgvHistoria.Columns["Haber"].DefaultCellStyle.ForeColor = Color.Green;
            }

            // CAMBIO: Ahora usamos "Saldo", no "SaldoParcial"
            if (dgvHistoria.Columns["Saldo"] != null)
            {
                dgvHistoria.Columns["Saldo"].DefaultCellStyle.Format = "C2";
                dgvHistoria.Columns["Saldo"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }

            if (dgvHistoria.Columns["VentaId"] != null) dgvHistoria.Columns["VentaId"].Visible = false;
        }
    }
}