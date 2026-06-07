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

        // Timer eliminado por mala UX. Reemplazado por KeyPreview Global.

        private AfipService _afipService;

        public VentasForm(Usuario vendedor)
        {
            InitializeComponent();
            _vendedor = vendedor;
            _ventaService = new VentaService();
            _afipService = new AfipService(); // Nota: Este servicio deberá leer la pass encriptada de BD

            // --- UX MEJORADA: CAPTURA INTELIGENTE DE ESCÁNER ---
            this.KeyPreview = true;
            this.KeyDown += VentasForm_KeyDown;
            this.KeyPress += VentasForm_GlobalKeyPress;
        }

        // Detecta input de teclado (scanner) sin importar dónde esté el foco
        private void VentasForm_GlobalKeyPress(object sender, KeyPressEventArgs e)
        {
            // Si el foco NO está en un campo donde el usuario escribe manualmente (Pago o Buscador de Cliente)
            if (this.ActiveControl != txtPagaCon &&
                this.ActiveControl != cboClientes &&
                !cboClientes.DroppedDown)
            {
                // Si el caracter no es de control, redirigirlo al scanner
                if (!char.IsControl(e.KeyChar))
                {
                    if (!txtEscanear.Focused)
                    {
                        txtEscanear.Focus();
                        txtEscanear.Text += e.KeyChar;
                        txtEscanear.SelectionStart = txtEscanear.Text.Length;
                        e.Handled = true; // Evita que se escriba doble o active menús
                    }
                }
            }
        }

        private void VentasForm_Load(object sender, EventArgs e)
        {
            _cajaIdActual = _ventaService.ObtenerCajaAbiertaId(_vendedor.Id);

            if (_cajaIdActual == null)
            {
                AudioHelper.PlayError();
                MessageBox.Show(
                    "⛔ NO SE PUEDE VENDER\n\nNo tiene una Caja Abierta.\nAbrir turno en 'Caja Diaria'.",
                    "Caja Cerrada", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                this.BeginInvoke(new Action(() => { this.Close(); }));
                return;
            }

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
                case Keys.F3: cboMetodoPago.SelectedItem = "Billetera Virtual"; e.Handled = true; break;
                case Keys.F4: cboMetodoPago.SelectedItem = "Fiado"; e.Handled = true; break;
                case Keys.F5: btnFinalizar.PerformClick(); e.Handled = true; break;
                case Keys.F10:
                    if (MessageBox.Show("¿Limpiar venta actual?", "Limpiar", MessageBoxButtons.YesNo) == DialogResult.Yes) Limpiar();
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
                // No forzamos Focus() aquí, dejamos que el flujo natural siga o el usuario decida
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
                MessageBox.Show($"Stock insuficiente. Disponible: {prod.Stock}.", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                // Volver a consumidor final
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

            if (_cajaIdActual == null)
            {
                MessageBox.Show("Caja cerrada o sesión perdida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            if (cboMetodoPago.SelectedItem?.ToString() == "Fiado")
            {
                var cli = cboClientes.SelectedItem as Cliente;
                if (cli == null || cli.DniCuit == Constantes.CLIENTE_DEF_DNI)
                {
                    AudioHelper.PlayError();
                    MessageBox.Show("Debe seleccionar un cliente para Fiado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            bool facturarAfip = false;
            var resp = MessageBox.Show("¿Emitir FACTURA ELECTRÓNICA AFIP?\n\n[SÍ] = Factura Oficial\n[NO] = Ticket Interno (X)",
                                       "Tipo Comprobante", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (resp == DialogResult.Cancel) return;
            facturarAfip = (resp == DialogResult.Yes);

            btnFinalizar.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            // Integración de Billetera Virtual (Mercado Pago QR Dinámico)
            if (cboMetodoPago.SelectedItem?.ToString() == "Billetera Virtual")
            {
                using (var qrForm = new PagoQRForm(_totalFinal))
                {
                    this.Cursor = Cursors.Default;
                    if (qrForm.ShowDialog() != DialogResult.OK)
                    {
                        btnFinalizar.Enabled = true;
                        return;
                    }
                    this.Cursor = Cursors.WaitCursor;
                }
            }

            try
            {
                int clienteId = cboClientes.SelectedValue != null ? (int)cboClientes.SelectedValue : 0;
                var ventaNueva = new Venta
                {
                    Fecha = DateTime.Now,
                    ClienteId = clienteId,
                    UsuarioId = _vendedor.Id,
                    Total = _totalFinal,
                    MetodoPago = cboMetodoPago.SelectedItem?.ToString() ?? "Efectivo",
                    CajaId = (int)_cajaIdActual,
                    TipoComprobante = facturarAfip ? "PENDIENTE" : "X",
                    CAE = "",
                    ObservacionesAFIP = ""
                };

                foreach (var item in _carrito) item.Subtotal = item.SubtotalCalculado;

                var ventaGuardada = _ventaService.RegistrarVenta(ventaNueva, _carrito);

                if (facturarAfip)
                {
                    try
                    {
                        var resultadoAfip = await _afipService.FacturarVentaAsync(ventaGuardada);
                        if (resultadoAfip.Exito)
                        {
                            ActualizarVentaConAfip(ventaGuardada, resultadoAfip);
                            AudioHelper.PlayCobro();
                            MessageBox.Show($"Factura Autorizada.\nCAE: {resultadoAfip.CAE}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            AudioHelper.PlayError();
                            MessageBox.Show($"Rechazo AFIP:\n{resultadoAfip.Error}", "Error AFIP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception exAfip)
                    {
                        AudioHelper.PlayError();
                        MessageBox.Show($"Error de conexión AFIP. Se guardó como X.\n{exAfip.Message}", "Offline", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    AudioHelper.PlayCobro();
                }

                if (MessageBox.Show("¿Imprimir Ticket?", "Imprimir", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ImprimirTicket(ventaGuardada);
                }

                Limpiar();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show($"Error al finalizar venta: {ex.Message}", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFinalizar.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void ActualizarVentaConAfip(Venta v, dynamic resultado)
        {
            using (var db = new AlmacenDbContext())
            {
                var ventaDb = db.Ventas.Find(v.Id);
                if (ventaDb != null)
                {
                    ventaDb.CAE = resultado.CAE;
                    ventaDb.CAEVencimiento = resultado.Vencimiento;
                    ventaDb.NumeroFactura = resultado.NumeroComprobante;
                    ventaDb.TipoComprobante = "C"; // O "B" segn corresponda
                    db.SaveChanges();
                }
            }
        }

        private void ImprimirTicket(Venta v)
        {
            try
            {
                Venta? ventaFull;
                using (var db = new AlmacenDbContext())
                {
                    ventaFull = db.Ventas.Include(x => x.Cliente).FirstOrDefault(x => x.Id == v.Id);
                }
                
                if (ventaFull == null)
                {
                    MessageBox.Show("Error: No se pudo cargar los detalles de la venta para imprimir.", "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var ticketService = new TicketService();
                ticketService.ImprimirVenta(ventaFull, _carrito);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error impresión: " + ex.Message);
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