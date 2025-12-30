using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace AlmacenDesktop.Forms
{
    public partial class VentasForm : Form
    {
        private Usuario _vendedor;
        private VentaService _ventaService;
        private List<DetalleVenta> _carrito = new List<DetalleVenta>();
        private int? _cajaIdActual = null;

        private decimal _subtotalBase = 0;
        private decimal _totalFinal = 0;
        private System.Windows.Forms.Timer _timerFocus;

        private AfipService _afipService;

        public VentasForm(Usuario vendedor)
        {
            InitializeComponent();
            _vendedor = vendedor;
            _ventaService = new VentaService();
            _afipService = new AfipService();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(VentasForm_KeyDown);

            ConfigurarFocusTimer();
        }

        private void ConfigurarFocusTimer()
        {
            _timerFocus = new System.Windows.Forms.Timer();
            _timerFocus.Interval = 2000;
            _timerFocus.Tick += TimerFocus_Tick;
            _timerFocus.Start();
        }

        private void TimerFocus_Tick(object sender, EventArgs e)
        {
            if (this.ActiveControl != null && this.ContainsFocus)
            {
                if (ActiveControl == txtPagaCon || ActiveControl == numCantidad ||
                    ActiveControl == cboMetodoPago || ActiveControl == cboClientes) return;

                if (ActiveControl != txtEscanear) txtEscanear.Focus();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerFocus?.Stop();
            _timerFocus?.Dispose();
            base.OnFormClosing(e);
        }

        // --- VALIDACIÓN DE CAJA EN EL LOAD ---
        private void VentasForm_Load(object sender, EventArgs e)
        {
            // Buscamos si hay caja abierta para este usuario
            _cajaIdActual = _ventaService.ObtenerCajaAbiertaId(_vendedor.Id);

            if (_cajaIdActual == null)
            {
                AudioHelper.PlayError();
                // Mensaje bloqueante
                MessageBox.Show(
                    "⛔ NO SE PUEDE VENDER\n\nNo tiene una Caja Abierta.\nDebe ir al módulo 'Caja Diaria' y abrir turno antes de realizar ventas.",
                    "Caja Cerrada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);

                // Cerramos el formulario inmediatamente.
                // Usamos BeginInvoke para asegurar que el formulario termine de cargar antes de cerrarse
                // y evitar excepciones de Handle.
                this.BeginInvoke(new Action(() => { this.Close(); }));
                return;
            }

            // Si hay caja, todo sigue normal
            this.Text = $"Punto de Venta - Caja #{_cajaIdActual} - {_vendedor.NombreUsuario}";
            CargarDatosIniciales();
            ConfigurarGrilla();
            this.ActiveControl = txtEscanear;
        }

        private void CargarDatosIniciales()
        {
            using (var context = new AlmacenDbContext())
            {
                var clientes = context.Clientes.ToList();
                var cf = clientes.FirstOrDefault(c => c.DniCuit == Constantes.CLIENTE_DEF_DNI);
                if (cf == null)
                {
                    cf = new Cliente { Nombre = Constantes.CLIENTE_DEF_NOMBRE, Apellido = Constantes.CLIENTE_DEF_APELLIDO, DniCuit = Constantes.CLIENTE_DEF_DNI, Email = "-", Telefono = "-", Direccion = "-" };
                    context.Clientes.Add(cf);
                    context.SaveChanges();
                    clientes.Add(cf);
                }

                cboClientes.DataSource = clientes;
                cboClientes.DisplayMember = "NombreCompleto";
                cboClientes.ValueMember = "Id";

                var productos = context.Productos.Where(p => p.Stock > 0).ToList();
                cboProductos.DataSource = productos;
                cboProductos.DisplayMember = "Nombre";
                cboProductos.ValueMember = "Id";
            }
            CargarMetodosPago();
        }

        private void CargarMetodosPago()
        {
            cboMetodoPago.Items.Clear();
            cboMetodoPago.Items.Add("Efectivo");
            cboMetodoPago.Items.Add("Transferencia");
            cboMetodoPago.Items.Add("Billetera Virtual");
            cboMetodoPago.Items.Add("Fiado");
            cboMetodoPago.SelectedIndex = 0;
        }

        private void VentasForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1: txtEscanear.Focus(); txtEscanear.SelectAll(); e.Handled = true; break;
                case Keys.F2: cboMetodoPago.SelectedItem = "Efectivo"; e.Handled = true; break;
                case Keys.F5: btnFinalizar.PerformClick(); e.Handled = true; break;
                case Keys.F10:
                    if (MessageBox.Show("¿Cancelar venta?", "Limpiar", MessageBoxButtons.YesNo) == DialogResult.Yes) Limpiar();
                    e.Handled = true; break;
                case Keys.Delete: EliminarItemSeleccionado(); e.Handled = true; break;
            }
        }

        private void txtEscanear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                string input = txtEscanear.Text.Trim();
                if (!string.IsNullOrEmpty(input))
                {
                    var producto = _ventaService.BuscarProducto(input);
                    if (producto != null) { AgregarAlCarrito(producto, 1); AudioHelper.PlayOk(); }
                    else { AudioHelper.PlayError(); MessageBox.Show("Producto no encontrado."); }
                }
                txtEscanear.Clear();
                txtEscanear.Focus();
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var prod = (Producto)cboProductos.SelectedItem;
            if (prod != null) AgregarAlCarrito(prod, (int)numCantidad.Value);
        }

        private void AgregarAlCarrito(Producto prod, int cantidad)
        {
            int cantidadEnCarrito = _carrito.Where(x => x.ProductoId == prod.Id).Sum(x => x.Cantidad);
            if (prod.Stock < (cantidadEnCarrito + cantidad))
            {
                AudioHelper.PlayError();
                MessageBox.Show($"Stock insuficiente. Stock real: {prod.Stock}.", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var itemExistente = _carrito.FirstOrDefault(x => x.ProductoId == prod.Id);
            if (itemExistente != null) itemExistente.Cantidad += cantidad;
            else _carrito.Add(new DetalleVenta { ProductoId = prod.Id, Producto = prod, Cantidad = cantidad, PrecioUnitario = prod.Precio });
            ActualizarUI();
        }

        private void EliminarItemSeleccionado()
        {
            if (dgvCarrito.SelectedRows.Count > 0)
            {
                _carrito.RemoveAt(dgvCarrito.SelectedRows[0].Index);
                ActualizarUI();
            }
        }

        private void ConfigurarGrilla()
        {
            dgvCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCarrito.MultiSelect = false;
            dgvCarrito.ReadOnly = true;
            dgvCarrito.RowHeadersVisible = false;
            dgvCarrito.BackgroundColor = Color.WhiteSmoke;
        }

        private void ActualizarUI()
        {
            dgvCarrito.DataSource = null;
            dgvCarrito.DataSource = _carrito.Select(x => new
            {
                Producto = x.Producto.Nombre,
                Cant = x.Cantidad,
                Precio = x.PrecioUnitario,
                Subtotal = x.SubtotalCalculado
            }).ToList();

            if (dgvCarrito.Columns["Precio"] != null) dgvCarrito.Columns["Precio"].DefaultCellStyle.Format = Constantes.MONEDA_FMT;
            if (dgvCarrito.Columns["Subtotal"] != null) dgvCarrito.Columns["Subtotal"].DefaultCellStyle.Format = Constantes.MONEDA_FMT;

            _subtotalBase = _carrito.Sum(x => x.SubtotalCalculado);

            decimal recargo = 0;
            if (cboMetodoPago.SelectedItem?.ToString() == "Billetera Virtual")
            {
                recargo = _subtotalBase * 0.06m;
                lblRecargoInfo.Visible = true;
                lblRecargoInfo.Text = $"+ Recargo (6%): {recargo.ToString(Constantes.MONEDA_FMT)}";
            }
            else lblRecargoInfo.Visible = false;

            _totalFinal = _subtotalBase + recargo;
            lblTotal.Text = $"TOTAL: {_totalFinal.ToString(Constantes.MONEDA_FMT)}";
            CalcularVuelto();
        }

        private void txtPagaCon_TextChanged(object sender, EventArgs e) => CalcularVuelto();

        private void CalcularVuelto()
        {
            if (decimal.TryParse(txtPagaCon.Text, out decimal pagaCon))
            {
                decimal vuelto = pagaCon - _totalFinal;
                lblVueltoMonto.Text = vuelto >= 0 ? vuelto.ToString(Constantes.MONEDA_FMT) : "Falta dinero";
                lblVueltoMonto.ForeColor = vuelto >= 0 ? Color.DarkGreen : Color.Red;
            }
            else lblVueltoMonto.Text = "$ 0.00";
        }

        private void cboMetodoPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMetodoPago.SelectedItem?.ToString() == "Fiado")
            {
                cboClientes.Enabled = true;
                cboClientes.Focus();
                cboClientes.DroppedDown = true;
            }
            else
            {
                cboClientes.Enabled = false;
                foreach (Cliente item in cboClientes.Items)
                {
                    if (item.DniCuit == Constantes.CLIENTE_DEF_DNI)
                    {
                        cboClientes.SelectedItem = item;
                        break;
                    }
                }
            }
            ActualizarUI();
        }

        private async void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (_carrito.Count == 0) return;

            // --- RE-VALIDACIÓN DE CAJA AL MOMENTO DE COBRAR ---
            // Por si la cerraron desde otra terminal o paso mucho tiempo
            if (_cajaIdActual == null)
            {
                MessageBox.Show("La caja se cerró o perdió sesión.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            if (cboMetodoPago.SelectedItem.ToString() == "Fiado")
            {
                var cli = (Cliente)cboClientes.SelectedItem;
                if (cli.DniCuit == Constantes.CLIENTE_DEF_DNI)
                {
                    AudioHelper.PlayError();
                    MessageBox.Show("Para vender FIADO seleccione un cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            bool facturarAfip = false;
            var resp = MessageBox.Show("¿Desea emitir FACTURA ELECTRÓNICA (AFIP)?\n\n[SÍ] = Factura Fiscal\n[NO] = Comprobante Interno (X)",
                                       "Tipo de Comprobante", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (resp == DialogResult.Cancel) return;
            facturarAfip = (resp == DialogResult.Yes);

            btnFinalizar.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // Validación de ClienteId antes de instanciar
                int clienteId = (int)cboClientes.SelectedValue;
                if (clienteId <= 0) throw new Exception("Cliente no válido seleccionado.");

                var ventaNueva = new Venta
                {
                    Fecha = DateTime.Now,
                    ClienteId = clienteId,
                    UsuarioId = _vendedor.Id,
                    Total = _totalFinal,
                    MetodoPago = cboMetodoPago.SelectedItem.ToString(),
                    CajaId = (int)_cajaIdActual,
                    TipoComprobante = facturarAfip ? "PENDIENTE" : "X",
                    CAE = "",
                    ObservacionesAFIP = ""
                };

                foreach (var item in _carrito)
                {
                    item.Subtotal = item.SubtotalCalculado;
                }

                var ventaGuardada = _ventaService.RegistrarVenta(ventaNueva, _carrito);

                if (facturarAfip)
                {
                    try
                    {
                        var resultadoAfip = await _afipService.FacturarVentaAsync(ventaGuardada);

                        if (resultadoAfip.Exito)
                        {
                            using (var db = new AlmacenDbContext())
                            {
                                var v = db.Ventas.Find(ventaGuardada.Id);
                                v.CAE = resultadoAfip.CAE;
                                v.CAEVencimiento = resultadoAfip.Vencimiento;
                                v.NumeroFactura = resultadoAfip.NumeroComprobante;
                                v.TipoComprobante = "C";
                                db.SaveChanges();

                                ventaGuardada.CAE = resultadoAfip.CAE;
                                ventaGuardada.CAEVencimiento = resultadoAfip.Vencimiento;
                                ventaGuardada.NumeroFactura = resultadoAfip.NumeroComprobante;
                                ventaGuardada.TipoComprobante = "C";
                            }
                            AudioHelper.PlayCobro();
                            MessageBox.Show($"¡Venta Autorizada!\nCAE: {resultadoAfip.CAE}", "AFIP OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            AudioHelper.PlayError();
                            MessageBox.Show($"AFIP Rechazó la factura:\n{resultadoAfip.Error}\n\nLa venta se guardó como 'Presupuesto X'.", "Rechazo AFIP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception exAfip)
                    {
                        AudioHelper.PlayError();
                        MessageBox.Show($"No se pudo facturar en AFIP:\n{exAfip.Message}\n\nLa venta se guardó como interna.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    AudioHelper.PlayCobro();
                }

                if (MessageBox.Show("Venta Finalizada. ¿Imprimir Ticket?", "Imprimir", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        Venta ventaParaImprimir;
                        using (var db = new AlmacenDbContext())
                        {
                            ventaParaImprimir = db.Ventas.Include(v => v.Cliente).FirstOrDefault(v => v.Id == ventaGuardada.Id);
                        }
                        var ticketService = new TicketService();
                        ticketService.ImprimirVenta(ventaParaImprimir, _carrito);
                    }
                    catch (Exception exPrint)
                    {
                        MessageBox.Show("Error al imprimir: " + exPrint.Message);
                    }
                }

                Limpiar();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                string detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"ERROR CRÍTICO AL GUARDAR VENTA:\n{detalle}", "Fallo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFinalizar.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void Limpiar()
        {
            _carrito.Clear();
            txtPagaCon.Text = "";
            cboMetodoPago.SelectedIndex = 0;
            ActualizarUI();
            txtEscanear.Focus();
        }

        private void txtPagaCon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnFinalizar.PerformClick();
            }
        }
    }
}