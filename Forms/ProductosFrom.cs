using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ProductosForm : Form
    {
        private int? _productoIdSeleccionado = null;

        public ProductosForm()
        {
            InitializeComponent();
            CargarProveedores();
            CargarProductos();
        }

        // Para asegurar el foco al abrir
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtCodigo.Focus();
        }

        private void CargarProveedores()
        {
            using (var context = new AlmacenDbContext())
            {
                cboProveedor.DataSource = context.Proveedores.ToList();
                cboProveedor.DisplayMember = "Nombre";
                cboProveedor.ValueMember = "Id";
                cboProveedor.SelectedIndex = -1;
            }
        }

        private void CargarProductos()
        {
            using (var context = new AlmacenDbContext())
            {
                // CAMBIO: Usamos una proyección (Select) para "aplanar" los datos.
                // Esto nos permite mostrar el NOMBRE del proveedor en la columna "Proveedor"
                // en lugar de que aparezca "AlmacenDesktop.Modelos.Proveedor" o el ID.
                var lista = context.Productos
                    .Include(p => p.Proveedor)
                    .Select(p => new
                    {
                        Id = p.Id,
                        Codigo = p.CodigoBarras,
                        Nombre = p.Nombre,
                        Costo = p.Costo,
                        Margen = p.Impuesto, // Mostramos "Margen" en el encabezado
                        Precio = p.Precio,
                        Stock = p.Stock,
                        Proveedor = p.Proveedor != null ? p.Proveedor.Nombre : "Sin Proveedor"
                    })
                    .ToList();

                dgvProductos.DataSource = lista;

                // Ajustes visuales opcionales
                if (dgvProductos.Columns["Id"] != null) dgvProductos.Columns["Id"].Visible = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarProducto();
        }

        private void GuardarProducto()
        {
            // Validacion simple: al menos nombre o codigo
            if (string.IsNullOrWhiteSpace(txtNombre.Text) && string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Debe ingresar al menos el Código o el Nombre.");
                return;
            }

            using (var context = new AlmacenDbContext())
            {
                Producto producto;

                if (_productoIdSeleccionado == null)
                {
                    // Verificar si el código ya existe (solo para nuevos)
                    if (!string.IsNullOrEmpty(txtCodigo.Text))
                    {
                        bool existe = context.Productos.Any(p => p.CodigoBarras == txtCodigo.Text);
                        if (existe)
                        {
                            MessageBox.Show("Ya existe un producto con ese Código de Barras.");
                            return;
                        }
                    }

                    producto = new Producto();
                    context.Productos.Add(producto);
                }
                else
                {
                    producto = context.Productos.Find(_productoIdSeleccionado);
                }

                producto.Nombre = txtNombre.Text;
                producto.CodigoBarras = txtCodigo.Text;
                producto.Costo = numCosto.Value;
                producto.Precio = numPrecio.Value;
                producto.Stock = (int)numStock.Value;
                producto.Impuesto = numImpuesto.Value;

                if (cboProveedor.SelectedValue != null)
                    producto.ProveedorId = (int)cboProveedor.SelectedValue;

                context.SaveChanges();
            }
            CargarProductos();

            MessageBox.Show("Producto guardado correctamente");
            Limpiar(); // Limpia y recarga la grilla
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                // CAMBIO: Como ahora usamos una proyección anónima en el DataSource,
                // no podemos castear a (Producto). Obtenemos el ID de la celda y buscamos.

                var row = dgvProductos.SelectedRows[0];

                // Verificamos que la celda "Id" exista y tenga valor
                if (row.Cells["Id"].Value == null) return;

                int id = (int)row.Cells["Id"].Value;

                using (var context = new AlmacenDbContext())
                {
                    var producto = context.Productos.Find(id);
                    if (producto != null)
                    {
                        _productoIdSeleccionado = producto.Id;
                        txtNombre.Text = producto.Nombre;
                        txtCodigo.Text = producto.CodigoBarras;
                        numCosto.Value = producto.Costo;
                        numPrecio.Value = producto.Precio;
                        numStock.Value = producto.Stock;
                        numImpuesto.Value = producto.Impuesto;
                        cboProveedor.SelectedValue = producto.ProveedorId;
                    }
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            _productoIdSeleccionado = null;
            txtNombre.Clear();
            txtCodigo.Clear();
            numCosto.Value = 0;
            numPrecio.Value = 0;
            numStock.Value = 0;
            numImpuesto.Value = 0;
            cboProveedor.SelectedIndex = -1;

            

            // CAMBIO: Devolver foco al código de barras para escanear el siguiente
            txtCodigo.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_productoIdSeleccionado == null) return;

            if (MessageBox.Show("¿Está seguro de eliminar este producto?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var context = new AlmacenDbContext())
                {
                    var prod = context.Productos.Find(_productoIdSeleccionado);
                    if (prod != null)
                    {
                        context.Productos.Remove(prod);
                        context.SaveChanges();
                        Limpiar();
                    }
                }
            }
        }

        // --- Lógica de Precios ---
        private void numCosto_ValueChanged(object sender, EventArgs e)
        {
            CalcularPrecioVenta();
        }

        private void numImpuesto_ValueChanged(object sender, EventArgs e)
        {
            CalcularPrecioVenta();
        }

        private void CalcularPrecioVenta()
        {
            if (numCosto.Value > 0)
            {
                decimal costo = numCosto.Value;
                decimal porcentaje = numImpuesto.Value;
                decimal precioSugerido = costo * (1 + (porcentaje / 100m));
                numPrecio.Value = precioSugerido;
            }
        }

        // --- CAMBIO: Teclas Rápidas ---
        private void ProductosForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                GuardarProducto();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Limpiar();
                e.Handled = true;
            }
        }
    }
}