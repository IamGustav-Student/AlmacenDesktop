using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers;
using Microsoft.EntityFrameworkCore;
using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers; // Importar Helpers
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms; // Necesario para los controles

namespace AlmacenDesktop.Forms
{
    public partial class VentasForm : Form
    {
        private Usuario _vendedor;
        private List<DetalleVenta> _carrito = new List<DetalleVenta>();
        private int? _cajaIdActual = null;

        private decimal _subtotalBase = 0;
        private decimal _totalFinal = 0;

        // CORRECCIÓN: Especificamos explícitamente que es un Timer de Windows Forms
        // para evitar el conflicto con System.Threading.Timer
        private System.Windows.Forms.Timer _timerFocus;

        public VentasForm(Usuario vendedor)
        {
            InitializeComponent();
            _vendedor = vendedor;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(VentasForm_KeyDown);

            // --- CONFIGURACIÓN FOCUS AGRESIVO ---
            _timerFocus = new System.Windows.Forms.Timer(); // Especificamos el tipo completo
            _timerFocus.Interval = 1000; // Chequear cada 1 segundo
            _timerFocus.Tick += TimerFocus_Tick; // Ahora el método existe abajo
            _timerFocus.Start();
        }

        // CORRECCIÓN: Aquí está el método que faltaba y daba error CS0103
        private void TimerFocus_Tick(object sender, EventArgs e)
        {
            // Solo forzamos el foco si la ventana está activa
            if (this.ActiveControl != null && this.ContainsFocus)
            {
                // Lista de controles donde permitimos que el usuario esté "tranquilo"
                if (ActiveControl == txtPagaCon || ActiveControl == numCantidad || ActiveControl == cboMetodoPago || ActiveControl == cboClientes)
                {
                    return; // Si está escribiendo el pago o cantidad, no molestamos
                }

                // Si el foco se perdió o está en un botón, lo devolvemos al escáner
                if (ActiveControl != txtEscanear)
                {
                    txtEscanear.Focus();
                }
            }
        }

        // Importante: Limpiar el timer al salir
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_timerFocus != null)
            {
                _timerFocus.Stop();
                _timerFocus.Dispose();
            }
            base.OnFormClosing(e);
        }

        private void VentasForm_Load(object sender, EventArgs e)
        {
            if (!VerificarCajaAbierta())
            {
                this.BeginInvoke(new Action(() => { this.Close(); }));
                return;
            }

            CargarCombos();
            CargarMetodosPago();
            ConfigurarGrilla();
            this.ActiveControl = txtEscanear;
        }

        private void VentasForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    txtEscanear.Focus();
                    txtEscanear.SelectAll();
                    e.Handled = true;
                    break;

                case Keys.F2:
                    cboMetodoPago.SelectedItem = "Efectivo";
                    e.Handled = true;
                    break;

                case Keys.F3:
                    int indexBill = cboMetodoPago.FindString("Billetera Virtual");
                    if (indexBill != -1) cboMetodoPago.SelectedIndex = indexBill;
                    e.Handled = true;
                    break;

                case Keys.F4:
                    cboMetodoPago.SelectedItem = "Fiado";
                    e.Handled = true;
                    break;

                case Keys.F5:
                    btnFinalizar.PerformClick();
                    e.Handled = true;
                    break;

                case Keys.F10:
                    if (MessageBox.Show("¿Cancelar venta?", "Cancelar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Limpiar();
                    }
                    e.Handled = true;
                    break;

                case Keys.Delete:
                    EliminarItemSeleccionado();
                    e.Handled = true;
                    break;
            }
        }

        private void txtPagaCon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnFinalizar.PerformClick();
            }
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

        private bool VerificarCajaAbierta()
        {
            using (var context = new AlmacenDbContext())
            {
                var cajaAbierta = context.Cajas.FirstOrDefault(c => c.UsuarioId == _vendedor.Id && c.EstaAbierta);
                if (cajaAbierta == null)
                {
                    AudioHelper.PlayError();
                    MessageBox.Show("Caja Cerrada. Abra caja primero.");
                    return false;
                }
                _cajaIdActual = cajaAbierta.Id;
                this.Text = $"Nueva Venta - Caja #{_cajaIdActual}";
                return true;
            }
        }

        private void CargarCombos()
        {
            using (var context = new AlmacenDbContext())
            {
                var clientes = context.Clientes.ToList();
                var consumidorFinal = clientes.FirstOrDefault(c => c.DniCuit == Constantes.CLIENTE_DEF_DNI);
                if (consumidorFinal == null)
                {
                    consumidorFinal = new Cliente { Nombre = Constantes.CLIENTE_DEF_NOMBRE, Apellido = Constantes.CLIENTE_DEF_APELLIDO, DniCuit = Constantes.CLIENTE_DEF_DNI, Email = "-", Telefono = "-", Direccion = "-" };
                    context.Clientes.Add(consumidorFinal);
                    context.SaveChanges();
                    clientes = context.Clientes.ToList();
                }
                cboClientes.DataSource = clientes;
                cboClientes.DisplayMember = "NombreCompleto";
                cboClientes.ValueMember = "Id";

                var productos = context.Productos.Where(p => p.Stock > 0).ToList();
                cboProductos.DataSource = productos;
                cboProductos.DisplayMember = "Nombre";
                cboProductos.ValueMember = "Id";
            }
        }

        private void txtEscanear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                string entrada = txtEscanear.Text.Trim();
                if (!string.IsNullOrEmpty(entrada)) ProcesarEntradaInteligente(entrada);
                txtEscanear.Clear();
                txtEscanear.Focus();
            }
        }

        private void ProcesarEntradaInteligente(string entrada)
        {
            using (var context = new AlmacenDbContext())
            {
                var producto = context.Productos.FirstOrDefault(p => p.CodigoBarras == entrada);
                if (producto != null)
                {
                    AgregarProductoLogica(producto);
                    return;
                }
                var lista = context.Productos.Where(p => p.Nombre.ToLower().Contains(entrada.ToLower())).ToList();
                if (lista.Count == 1)
                {
                    AgregarProductoLogica(lista[0]);
                }
                else if (lista.Count > 1)
                {
                    AudioHelper.PlayError(); // SONIDO
                    MessageBox.Show("Múltiples resultados. Sea más específico.");
                }
                else
                {
                    AudioHelper.PlayError(); // SONIDO
                    MessageBox.Show("No encontrado.");
                }
            }
        }

        private void AgregarProductoLogica(Producto producto)
        {
            int enCarrito = _carrito.Where(x => x.ProductoId == producto.Id).Sum(x => x.Cantidad);
            if (producto.Stock > enCarrito)
            {
                AudioHelper.PlayOk(); // SONIDO DE ÉXITO (BIP)
                AgregarItemAlCarrito(producto, 1);
            }
            else
            {
                AudioHelper.PlayError(); // SONIDO ERROR
                MessageBox.Show("Sin Stock suficiente.");
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var prod = (Producto)cboProductos.SelectedItem;
            if (prod != null) AgregarItemAlCarrito(prod, (int)numCantidad.Value);
        }

        private void ConfigurarGrilla()
        {
            dgvCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCarrito.MultiSelect = false;
            dgvCarrito.ReadOnly = true;
            dgvCarrito.RowHeadersVisible = false;
            dgvCarrito.BackgroundColor = Color.White;
        }

        private void EliminarItemSeleccionado()
        {
            if (dgvCarrito.SelectedRows.Count > 0)
            {
                int index = dgvCarrito.SelectedRows[0].Index;
                if (index >= 0 && index < _carrito.Count)
                {
                    _carrito.RemoveAt(index);
                    CalcularTotales();
                }
            }
        }

        private void AgregarItemAlCarrito(Producto producto, int cantidad)
        {
            var itemExistente = _carrito.FirstOrDefault(x => x.ProductoId == producto.Id);
            if (itemExistente != null) itemExistente.Cantidad += cantidad;
            else _carrito.Add(new DetalleVenta { ProductoId = producto.Id, Producto = producto, Cantidad = cantidad, PrecioUnitario = producto.Precio });

            CalcularTotales();
        }

        private void CalcularTotales()
        {
            dgvCarrito.DataSource = null;
            dgvCarrito.DataSource = _carrito.Select(x => new
            {
                Producto = x.Producto.Nombre,
                Cant = x.Cantidad,
                Precio = x.PrecioUnitario,
                Subtotal = x.Subtotal
            }).ToList();

            if (dgvCarrito.Columns["Precio"] != null) dgvCarrito.Columns["Precio"].DefaultCellStyle.Format = Constantes.MONEDA_FMT;
            if (dgvCarrito.Columns["Subtotal"] != null) dgvCarrito.Columns["Subtotal"].DefaultCellStyle.Format = Constantes.MONEDA_FMT;

            _subtotalBase = _carrito.Sum(x => x.Subtotal);

            string metodo = cboMetodoPago.SelectedItem?.ToString();
            decimal recargo = 0;

            if (metodo == "Billetera Virtual")
            {
                recargo = _subtotalBase * 0.06m;
                lblRecargoInfo.Visible = true;
            }
            else
            {
                lblRecargoInfo.Visible = false;
            }

            _totalFinal = _subtotalBase + recargo;
            lblTotal.Text = $"Total: {_totalFinal.ToString(Constantes.MONEDA_FMT)}";

            CalcularVuelto();
        }

        private void cboMetodoPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            string metodo = cboMetodoPago.SelectedItem?.ToString();

            if (metodo == "Fiado")
            {
                cboClientes.Enabled = true;
                cboClientes.Focus();
                cboClientes.DroppedDown = true;
            }
            else
            {
                cboClientes.Enabled = false;
                SeleccionarConsumidorFinal();
            }

            CalcularTotales();
        }

        private void SeleccionarConsumidorFinal()
        {
            foreach (Cliente item in cboClientes.Items)
            {
                if (item.DniCuit == Constantes.CLIENTE_DEF_DNI)
                {
                    cboClientes.SelectedItem = item;
                    break;
                }
            }
        }

        private void txtPagaCon_TextChanged(object sender, EventArgs e)
        {
            CalcularVuelto();
        }

        private void CalcularVuelto()
        {
            if (decimal.TryParse(txtPagaCon.Text, out decimal pagaCon))
            {
                decimal vuelto = pagaCon - _totalFinal;
                if (vuelto < 0)
                {
                    lblVueltoMonto.ForeColor = Color.Red;
                    lblVueltoMonto.Text = "Falta dinero";
                }
                else
                {
                    lblVueltoMonto.ForeColor = Color.Green;
                    lblVueltoMonto.Text = vuelto.ToString(Constantes.MONEDA_FMT);
                }
            }
            else
            {
                lblVueltoMonto.Text = "$ 0.00";
            }
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (_carrito.Count == 0) return;
            if (!_cajaIdActual.HasValue) return;

            if (cboMetodoPago.SelectedItem.ToString() == "Fiado")
            {
                var cliente = (Cliente)cboClientes.SelectedItem;
                if (cliente.DniCuit == Constantes.CLIENTE_DEF_DNI)
                {
                    AudioHelper.PlayError(); // SONIDO ERROR
                    MessageBox.Show("Debe seleccionar un cliente para fiar.");
                    return;
                }
            }

            // Variable para guardar la venta y usarla FUERA del using
            Venta ventaGuardada = null;

            try
            {
                // 1. GUARDADO DE DATOS (Bloque de Base de Datos)
                using (var context = new AlmacenDbContext())
                {
                    ventaGuardada = new Venta
                    {
                        Fecha = DateTime.Now,
                        ClienteId = (int)cboClientes.SelectedValue,
                        UsuarioId = _vendedor.Id,
                        Total = _totalFinal,
                        MetodoPago = cboMetodoPago.SelectedItem.ToString(),
                        CajaId = _cajaIdActual
                    };

                    context.Ventas.Add(ventaGuardada);
                    context.SaveChanges();

                    foreach (var item in _carrito)
                    {
                        var detalle = new DetalleVenta
                        {
                            VentaId = ventaGuardada.Id,
                            ProductoId = item.ProductoId,
                            Cantidad = item.Cantidad,
                            PrecioUnitario = item.PrecioUnitario
                        };
                        context.DetallesVenta.Add(detalle);

                        var prodDb = context.Productos.Find(item.ProductoId);
                        if (prodDb != null) prodDb.ReducirStock(item.Cantidad);
                    }
                    context.SaveChanges();
                } // <--- AQUÍ SE CIERRA LA CONEXIÓN A LA BD

                // SONIDO DE CAJA REGISTRADORA
                AudioHelper.PlayCobro();

                // 2. IMPRESIÓN (Fuera del bloqueo de BD)
                if (ventaGuardada != null)
                {
                    if (MessageBox.Show("Venta Exitosa. ¿Ticket?", "Imprimir", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        // Ahora TicketService puede abrir su propia conexión sin chocar
                        new TicketService().Imprimir(ventaGuardada, _carrito);
                    }
                }

                // 3. LIMPIEZA
                Limpiar();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError(); // SONIDO ERROR
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        void Limpiar()
        {
            _carrito.Clear();
            CalcularTotales();
            SeleccionarConsumidorFinal();
            cboMetodoPago.SelectedIndex = 0;
            txtPagaCon.Text = "";
            txtEscanear.Clear();
            txtEscanear.Focus();
        }
    }
}