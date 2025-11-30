using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class VentasForm : Form
    {
        private Usuario _vendedor;
        private List<DetalleVenta> _carrito = new List<DetalleVenta>();

        // CLIENTE PREDETERMINADO ---
        private const string CLIENTE_DEF_NOMBRE = "Consumidor";
        private const string CLIENTE_DEF_APELLIDO = "Final";
        private const string CLIENTE_DEF_DNI = "00000000";

        public VentasForm(Usuario vendedor)
        {
            InitializeComponent();
            _vendedor = vendedor;
        }

        private void VentasForm_Load(object sender, EventArgs e)
        {
            CargarCombos();
            CargarMetodosPago();
            this.ActiveControl = txtEscanear;
        }

        private void CargarMetodosPago()
        {
            cboMetodoPago.Items.Add("Efectivo");
            cboMetodoPago.Items.Add("Transferencia");
            cboMetodoPago.Items.Add("Fiado");
            cboMetodoPago.SelectedIndex = 0;
        }

        private void CargarCombos()
        {
            using (var context = new AlmacenDbContext())
            {
                var clientes = context.Clientes.ToList();

                // Validación de seguridad por si Program.cs no corrió o falló
                var consumidorFinal = clientes.FirstOrDefault(c => c.DniCuit == CLIENTE_DEF_DNI);

                if (consumidorFinal == null)
                {
                    // Lo creamos al vuelo si no existe
                    consumidorFinal = new Cliente
                    {
                        Nombre = CLIENTE_DEF_NOMBRE,
                        Apellido = CLIENTE_DEF_APELLIDO,
                        DniCuit = CLIENTE_DEF_DNI,
                        Email = "-",
                        Telefono = "-",
                        Direccion = "Mostrador"
                    };
                    context.Clientes.Add(consumidorFinal);
                    context.SaveChanges();
                    clientes = context.Clientes.ToList(); // Recargamos la lista
                }

                cboClientes.DataSource = clientes;
                cboClientes.DisplayMember = "NombreCompleto";
                cboClientes.ValueMember = "Id";

                // Productos
                var productos = context.Productos.Where(p => p.Stock > 0).ToList();
                cboProductos.DataSource = productos;
                cboProductos.DisplayMember = "Nombre";
                cboProductos.ValueMember = "Id";
            }
        }

        // --- LÓGICA INTELIGENTE ---
        private void cboMetodoPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            string metodo = cboMetodoPago.SelectedItem.ToString();

            if (metodo == "Fiado")
            {
                cboClientes.Enabled = true;
            }
            else
            {
                cboClientes.Enabled = false;
                SeleccionarConsumidorFinal();
            }
        }

        private void SeleccionarConsumidorFinal()
        {
            foreach (Cliente item in cboClientes.Items)
            {
                // Buscamos por DNI que es más seguro que el nombre
                if (item.DniCuit == CLIENTE_DEF_DNI)
                {
                    cboClientes.SelectedItem = item;
                    break;
                }
            }
        }

        private void txtEscanear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                string codigo = txtEscanear.Text.Trim();

                if (!string.IsNullOrEmpty(codigo))
                {
                    AgregarPorCodigo(codigo);
                }
                txtEscanear.Clear();
                txtEscanear.Focus();
            }
        }

        private void AgregarPorCodigo(string codigo)
        {
            using (var context = new AlmacenDbContext())
            {
                var producto = context.Productos.FirstOrDefault(p => p.CodigoBarras == codigo);

                if (producto != null)
                {
                    int cantidadEnCarrito = _carrito.Where(x => x.ProductoId == producto.Id).Sum(x => x.Cantidad);

                    if (producto.Stock > cantidadEnCarrito)
                    {
                        AgregarItemAlCarrito(producto, 1);
                    }
                    else
                    {
                        MessageBox.Show("No hay suficiente stock.", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Producto no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var productoSeleccionado = (Producto)cboProductos.SelectedItem;
            int cantidad = (int)numCantidad.Value;

            if (productoSeleccionado == null) return;

            int cantidadEnCarrito = _carrito.Where(x => x.ProductoId == productoSeleccionado.Id).Sum(x => x.Cantidad);

            if (productoSeleccionado.Stock >= (cantidad + cantidadEnCarrito))
            {
                AgregarItemAlCarrito(productoSeleccionado, cantidad);
            }
            else
            {
                MessageBox.Show("No hay suficiente stock disponible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AgregarItemAlCarrito(Producto producto, int cantidad)
        {
            var itemExistente = _carrito.FirstOrDefault(x => x.ProductoId == producto.Id);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                var item = new DetalleVenta
                {
                    ProductoId = producto.Id,
                    Producto = producto,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio
                };
                _carrito.Add(item);
            }

            ActualizarGrilla();
        }

        private void ActualizarGrilla()
        {
            dgvCarrito.DataSource = null;
            dgvCarrito.DataSource = _carrito.Select(x => new
            {
                Producto = x.Producto.Nombre,
                Cantidad = x.Cantidad,
                Precio = x.PrecioUnitario,
                Subtotal = x.Subtotal
            }).ToList();

            decimal total = _carrito.Sum(x => x.Subtotal);
            lblTotal.Text = $"Total a Pagar: ${total:N2}";
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (_carrito.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.");
                return;
            }

            string metodoPago = cboMetodoPago.SelectedItem.ToString();
            Cliente clienteSeleccionado = (Cliente)cboClientes.SelectedItem;

            // Validación: No permitir fiar a Consumidor Final
            if (metodoPago == "Fiado")
            {
                if (clienteSeleccionado.DniCuit == CLIENTE_DEF_DNI)
                {
                    MessageBox.Show("Para fiar debe seleccionar un cliente registrado con nombre y apellido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var nuevaVenta = new Venta
                    {
                        Fecha = DateTime.Now,
                        ClienteId = (int)cboClientes.SelectedValue,
                        UsuarioId = _vendedor.Id,
                        Total = _carrito.Sum(x => x.Subtotal),
                        MetodoPago = metodoPago
                    };

                    context.Ventas.Add(nuevaVenta);
                    context.SaveChanges();

                    foreach (var item in _carrito)
                    {
                        item.VentaId = nuevaVenta.Id;
                        item.Producto = null;
                        context.DetallesVenta.Add(item);

                        var productoEnDb = context.Productos.Find(item.ProductoId);
                        productoEnDb.ReducirStock(item.Cantidad);
                    }

                    context.SaveChanges();

                    MessageBox.Show("¡Venta registrada con éxito!");
                    Limpiar();
                    return;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar la venta: {ex.Message}");
            }
        }
        void Limpiar()
        {
            _carrito.Clear();
            ActualizarGrilla();
            SeleccionarConsumidorFinal();
            cboMetodoPago.SelectedIndex = 0;
            txtEscanear.Clear();
            txtEscanear.Focus();
        }
    }
}