using AlmacenDesktop.Data; // Solo para modelos simples si hace falta
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class VentasForm : Form
    {
        // Dependencias y Estado
        private Usuario _vendedor;
        private VentaService _ventaService; // Nuestro nuevo cerebro
        private List<DetalleVenta> _carrito = new List<DetalleVenta>();
        private int? _cajaIdActual = null;

        private decimal _subtotalBase = 0;
        private decimal _totalFinal = 0;
        private System.Windows.Forms.Timer _timerFocus;

        public VentasForm(Usuario vendedor)
        {
            InitializeComponent();
            _vendedor = vendedor;
            _ventaService = new VentaService(); // Inyección manual (simplificada)

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(VentasForm_KeyDown);

            ConfigurarFocusTimer();
        }

        private void ConfigurarFocusTimer()
        {
            _timerFocus = new System.Windows.Forms.Timer();
            _timerFocus.Interval = 2000; // Chequear cada 2 segundos (más ágil)
            _timerFocus.Tick += TimerFocus_Tick;
            _timerFocus.Start();
        }

        private void TimerFocus_Tick(object sender, EventArgs e)
        {
            if (this.ActiveControl != null && this.ContainsFocus)
            {
                // Lista blanca de controles donde NO robamos el foco
                if (ActiveControl == txtPagaCon || ActiveControl == numCantidad ||
                    ActiveControl == cboMetodoPago || ActiveControl == cboClientes)
                {
                    return;
                }

                // Si el foco está perdido, vuelve al escáner
                if (ActiveControl != txtEscanear)
                {
                    txtEscanear.Focus();
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerFocus?.Stop();
            _timerFocus?.Dispose();
            base.OnFormClosing(e);
        }

        private void VentasForm_Load(object sender, EventArgs e)
        {
            // 1. Validar Caja Abierta usando el Servicio
            _cajaIdActual = _ventaService.ObtenerCajaAbiertaId(_vendedor.Id);

            if (_cajaIdActual == null)
            {
                AudioHelper.PlayError();
                MessageBox.Show("¡Atención! La caja está cerrada.\nDebe abrir caja antes de vender.", "Caja Cerrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            // Nota: Idealmente moveríamos esto a ClienteService y ProductoService
            // pero para esta fase, mantenemos el acceso directo solo para lectura de combos
            using (var context = new AlmacenDbContext())
            {
                var clientes = context.Clientes.ToList();
                // Asegurar Consumidor Final
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
            cboMetodoPago.Items.Add("Billetera Virtual"); // QR
            cboMetodoPago.Items.Add("Fiado"); // Cta Cte
            cboMetodoPago.SelectedIndex = 0;
        }

        // --- EVENTOS DE TECLADO (ACCESOS RÁPIDOS) ---
        private void VentasForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1: txtEscanear.Focus(); txtEscanear.SelectAll(); e.Handled = true; break;
                case Keys.F2: cboMetodoPago.SelectedItem = "Efectivo"; e.Handled = true; break;
                case Keys.F3:
                    int idx = cboMetodoPago.FindString("Billetera");
                    if (idx != -1) cboMetodoPago.SelectedIndex = idx;
                    e.Handled = true; break;
                case Keys.F5: btnFinalizar.PerformClick(); e.Handled = true; break; // COBRAR
                case Keys.F10:
                    if (MessageBox.Show("¿Cancelar venta actual?", "Limpiar", MessageBoxButtons.YesNo) == DialogResult.Yes) Limpiar();
                    e.Handled = true; break;
                case Keys.Delete: EliminarItemSeleccionado(); e.Handled = true; break;
            }
        }

        // --- LÓGICA DE ESCANEO ---
        private void txtEscanear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                string input = txtEscanear.Text.Trim();
                if (!string.IsNullOrEmpty(input))
                {
                    // Delegamos la búsqueda al servicio
                    var producto = _ventaService.BuscarProducto(input);
                    if (producto != null)
                    {
                        AgregarAlCarrito(producto, 1);
                        AudioHelper.PlayOk();
                    }
                    else
                    {
                        AudioHelper.PlayError();
                        MessageBox.Show("Producto no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
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
            // Validar stock localmente en el carrito actual antes de agregar
            int cantidadEnCarrito = _carrito.Where(x => x.ProductoId == prod.Id).Sum(x => x.Cantidad);

            if (prod.Stock < (cantidadEnCarrito + cantidad))
            {
                AudioHelper.PlayError();
                MessageBox.Show($"Stock insuficiente. Stock real: {prod.Stock}. En carrito: {cantidadEnCarrito}.", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var itemExistente = _carrito.FirstOrDefault(x => x.ProductoId == prod.Id);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                _carrito.Add(new DetalleVenta
                {
                    ProductoId = prod.Id,
                    Producto = prod,
                    Cantidad = cantidad,
                    PrecioUnitario = prod.Precio
                });
            }
            ActualizarUI();
        }

        private void EliminarItemSeleccionado()
        {
            if (dgvCarrito.SelectedRows.Count > 0)
            {
                int idx = dgvCarrito.SelectedRows[0].Index;
                _carrito.RemoveAt(idx);
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
            // Refrescar Grilla
            dgvCarrito.DataSource = null;
            dgvCarrito.DataSource = _carrito.Select(x => new
            {
                Producto = x.Producto.Nombre,
                Cant = x.Cantidad,
                Precio = x.PrecioUnitario,
                Subtotal = x.Subtotal
            }).ToList();

            // Formatos
            if (dgvCarrito.Columns["Precio"] != null) dgvCarrito.Columns["Precio"].DefaultCellStyle.Format = Constantes.MONEDA_FMT;
            if (dgvCarrito.Columns["Subtotal"] != null) dgvCarrito.Columns["Subtotal"].DefaultCellStyle.Format = Constantes.MONEDA_FMT;

            // Cálculos Totales
            _subtotalBase = _carrito.Sum(x => x.Subtotal);

            // Lógica de Recargos (Ej: Billetera Virtual +6%)
            decimal recargo = 0;
            if (cboMetodoPago.SelectedItem?.ToString() == "Billetera Virtual")
            {
                recargo = _subtotalBase * 0.06m; // Ejemplo parametrizable a futuro
                lblRecargoInfo.Visible = true;
                lblRecargoInfo.Text = $"+ Recargo (6%): {recargo.ToString(Constantes.MONEDA_FMT)}";
            }
            else
            {
                lblRecargoInfo.Visible = false;
            }

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
            else
            {
                lblVueltoMonto.Text = "$ 0.00";
            }
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

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (_carrito.Count == 0) return;

            if (cboMetodoPago.SelectedItem.ToString() == "Fiado")
            {
                var cli = (Cliente)cboClientes.SelectedItem;
                if (cli.DniCuit == Constantes.CLIENTE_DEF_DNI)
                {
                    AudioHelper.PlayError();
                    MessageBox.Show("Para vender FIADO debe seleccionar un cliente registrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                // REGLA CRÍTICA DE VALIDACIÓN ANTES DE CONVERTIR
                if (_cajaIdActual == null)
                {
                    MessageBox.Show("La caja se cerró o perdió sesión. Por favor reabra la caja.");
                    this.Close();
                    return;
                }

                // Construir Objeto Venta
                var ventaNueva = new Venta
                {
                    Fecha = DateTime.Now,
                    ClienteId = (int)cboClientes.SelectedValue,
                    UsuarioId = _vendedor.Id,
                    Total = _totalFinal,
                    MetodoPago = cboMetodoPago.SelectedItem.ToString(),
                    // CORRECCIÓN DEL ERROR: Casteo explícito seguro porque validamos null arriba
                    CajaId = (int)_cajaIdActual
                };

                var ventaProcesada = _ventaService.RegistrarVenta(ventaNueva, _carrito);

                AudioHelper.PlayCobro();

                if (MessageBox.Show("Venta registrada. ¿Imprimir Ticket?", "Ticket", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        var ticketService = new TicketService();
                        ticketService.ImprimirVenta(ventaProcesada, _carrito);
                    }
                    catch (Exception exPrint)
                    {
                        MessageBox.Show("Error al imprimir (La venta sí se guardó): " + exPrint.Message);
                    }
                }

                Limpiar();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show($"ERROR CRÍTICO:\n{ex.Message}", "Fallo en Venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
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