using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers; // Para AudioHelper
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ProductosForm : Form
    {
        private ProductoService _productoService;
        private Producto _productoSeleccionado;
        private bool _modoEdicion = false;

        public ProductosForm()
        {
            InitializeComponent();
            _productoService = new ProductoService();

            // --- HABILITAR ATAJOS DE TECLADO ---
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(ProductosForm_KeyDown);
            this.KeyPress += new KeyPressEventHandler(ProductosForm_GlobalKeyPress);
        }

        // Captura inteligente global para escáner de código de barras
        private void ProductosForm_GlobalKeyPress(object sender, KeyPressEventArgs e)
        {
            // Obtener el control activo real
            Control active = this.ActiveControl;

            // Si el foco está en un control de edición manual, no interferir
            if (active == txtNombre || 
                active == txtDescripcion || 
                active == txtBusqueda ||
                active == numCosto || 
                active == numPrecio || 
                active == numStock || 
                active == numStockMinimo || 
                active == numImpuesto || 
                active == cboProveedor)
            {
                return;
            }

            // También comprobar si es un control hijo (por ejemplo, el TextBox interno de un NumericUpDown)
            if (active != null && active.Parent != null)
            {
                Control parent = active.Parent;
                if (parent == numCosto || 
                    parent == numPrecio || 
                    parent == numStock || 
                    parent == numStockMinimo || 
                    parent == numImpuesto || 
                    parent == cboProveedor)
                {
                    return;
                }
            }

            // Si es un carácter de control (como Backspace o Enter), dejar que se propague normalmente
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            // Redirigir el carácter a txtCodigo
            e.Handled = true; // Consumir el evento aquí para controlarlo totalmente

            if (!txtCodigo.Focused)
            {
                txtCodigo.Focus();
            }

            txtCodigo.AppendText(e.KeyChar.ToString());
        }

        private void ProductosForm_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            CargarProveedores();
            CargarProductos();
            LimpiarFormulario();
        }

        private void ConfigurarGrilla()
        {
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.SelectionChanged += dgvProductos_SelectionChanged;
        }

        private void CargarProveedores()
        {
            using (var context = new AlmacenDbContext())
            {
                var proveedores = context.Proveedores.OrderBy(p => p.Nombre).ToList();
                cboProveedor.DataSource = proveedores;
                cboProveedor.DisplayMember = "Nombre";
                cboProveedor.ValueMember = "Id";
            }
        }

        private void CargarProductos(string busqueda = "")
        {
            try
            {
                List<Producto> lista;
                if (string.IsNullOrWhiteSpace(busqueda))
                {
                    lista = _productoService.ObtenerTodos();
                }
                else
                {
                    lista = _productoService.Buscar(busqueda);
                }
                dgvProductos.DataSource = lista;
                lblTotalProductos.Text = $"Total: {lista.Count} productos";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando productos: " + ex.Message);
            }
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            // Búsqueda en tiempo real
            CargarProductos(txtBusqueda.Text.Trim());
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                string barcode = txtCodigo.Text.Trim();
                if (!string.IsNullOrEmpty(barcode))
                {
                    // Al presionar Enter (fin de escaneo), si el producto existe, lo cargamos para editarlo.
                    // Si no existe, dejamos el cursor listo para escribir el nombre de este producto nuevo.
                    using (var context = new AlmacenDbContext())
                    {
                        var existente = context.Productos.FirstOrDefault(p => p.CodigoBarras == barcode && p.Activo);
                        if (existente != null)
                        {
                            _productoSeleccionado = existente;
                            txtNombre.Text = existente.Nombre;
                            txtDescripcion.Text = existente.Descripcion;
                            numCosto.Value = existente.Costo;
                            numPrecio.Value = existente.Precio;
                            numStock.Value = existente.Stock;
                            numStockMinimo.Value = existente.StockMinimo;
                            numImpuesto.Value = existente.Impuesto;
                            cboProveedor.SelectedValue = existente.ProveedorId;

                            _modoEdicion = true;
                            lblModo.Text = $"Editando: {existente.Nombre}";
                            lblModo.ForeColor = Color.OrangeRed;
                            btnGuardar.Text = "Actualizar (F5)";
                            txtNombre.Focus();
                            txtNombre.SelectAll();
                            AudioHelper.PlayOk();
                        }
                        else
                        {
                            // Producto nuevo, enfocar nombre para cargarlo rápido
                            txtNombre.Focus();
                        }
                    }
                }
            }
        }

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                string busqueda = txtBusqueda.Text.Trim();
                CargarProductos(busqueda);
                
                // Si la búsqueda fue un código de barra exacto y hay coincidencia, seleccionarlo y editarlo
                if (dgvProductos.Rows.Count == 1)
                {
                    dgvProductos.Rows[0].Selected = true;
                    btnEditar_Click(sender, e);
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario()) return;

            try
            {
                var producto = _modoEdicion ? _productoSeleccionado : new Producto();

                producto.CodigoBarras = txtCodigo.Text.Trim();
                producto.Nombre = txtNombre.Text.Trim();
                producto.Descripcion = txtDescripcion.Text.Trim();
                producto.Costo = numCosto.Value;
                producto.Precio = numPrecio.Value;
                producto.Stock = (int)numStock.Value;
                producto.StockMinimo = (int)numStockMinimo.Value;
                producto.Impuesto = numImpuesto.Value; // Opcional
                producto.ProveedorId = (int)cboProveedor.SelectedValue;
                producto.Activo = true;

                _productoService.Guardar(producto);

                AudioHelper.PlayOk();
                MessageBox.Show("Producto guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarFormulario();
                CargarProductos();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show(ex.Message, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un producto para editar.");
                return;
            }

            var productoGrid = (Producto)dgvProductos.SelectedRows[0].DataBoundItem;
            // Recargamos desde la BD para tener la entidad fresca
            _productoSeleccionado = _productoService.ObtenerPorId(productoGrid.Id);

            if (_productoSeleccionado != null)
            {
                txtCodigo.Text = _productoSeleccionado.CodigoBarras;
                txtNombre.Text = _productoSeleccionado.Nombre;
                txtDescripcion.Text = _productoSeleccionado.Descripcion;
                numCosto.Value = _productoSeleccionado.Costo;
                numPrecio.Value = _productoSeleccionado.Precio;
                numStock.Value = _productoSeleccionado.Stock;
                numStockMinimo.Value = _productoSeleccionado.StockMinimo;
                numImpuesto.Value = _productoSeleccionado.Impuesto;
                cboProveedor.SelectedValue = _productoSeleccionado.ProveedorId;

                _modoEdicion = true;
                lblModo.Text = $"Editando: {_productoSeleccionado.Nombre}";
                lblModo.ForeColor = Color.OrangeRed;
                btnGuardar.Text = "Actualizar (F5)";

                // Deshabilitar código de barras en edición si es clave crítica (opcional)
                // txtCodigo.Enabled = false; 
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0) return;

            var producto = (Producto)dgvProductos.SelectedRows[0].DataBoundItem;

            if (MessageBox.Show($"¿Seguro que desea eliminar '{producto.Nombre}'?\n(Se realizará un borrado lógico)",
                "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    _productoService.Eliminar(producto.Id);
                    AudioHelper.PlayOk();
                    CargarProductos();
                    LimpiarFormulario();
                }
                catch (Exception ex)
                {
                    AudioHelper.PlayError();
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            numCosto.Value = 0;
            numPrecio.Value = 0;
            numStock.Value = 0;
            numStockMinimo.Value = 5; // Valor por defecto sensato
            numImpuesto.Value = 0;

            if (cboProveedor.Items.Count > 0) cboProveedor.SelectedIndex = 0;

            // Evitar que la selección permanezca activa al limpiar
            dgvProductos.SelectionChanged -= dgvProductos_SelectionChanged;
            dgvProductos.ClearSelection();
            dgvProductos.SelectionChanged += dgvProductos_SelectionChanged;

            _productoSeleccionado = null;
            _modoEdicion = false;

            lblModo.Text = "Nuevo Producto";
            lblModo.ForeColor = Color.Green;
            btnGuardar.Text = "Guardar (F5)";
            txtCodigo.Enabled = true;
            txtCodigo.Focus();
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Ingrese el Código de Barras.");
                txtCodigo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el Nombre del Producto.");
                txtNombre.Focus();
                return false;
            }
            if (cboProveedor.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un Proveedor.");
                cboProveedor.Focus();
                return false;
            }
            // Validación de Rentabilidad en UI (Feedback inmediato)
            if (numPrecio.Value < numCosto.Value)
            {
                var resp = MessageBox.Show("⚠️ El Precio de Venta es menor que el Costo.\n¿Está seguro de que desea guardar con pérdidas?",
                    "Advertencia de Rentabilidad", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resp == DialogResult.No) return false;
            }

            return true;
        }

        // Evento para calcular precio sugerido automáticamente (Opcional)
        private void numCosto_ValueChanged(object sender, EventArgs e)
        {
            // Si es un producto nuevo y el usuario pone costo, sugerimos precio (+30% margen por defecto)
            if (!_modoEdicion && numPrecio.Value == 0 && numCosto.Value > 0)
            {
                numPrecio.Value = numCosto.Value * 1.30m;
            }
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                var productoGrid = dgvProductos.SelectedRows[0].DataBoundItem as Producto;
                if (productoGrid != null)
                {
                    _productoSeleccionado = _productoService.ObtenerPorId(productoGrid.Id);
                    if (_productoSeleccionado != null)
                    {
                        txtCodigo.Text = _productoSeleccionado.CodigoBarras;
                        txtNombre.Text = _productoSeleccionado.Nombre;
                        txtDescripcion.Text = _productoSeleccionado.Descripcion;
                        numCosto.Value = _productoSeleccionado.Costo;
                        numPrecio.Value = _productoSeleccionado.Precio;
                        numStock.Value = _productoSeleccionado.Stock;
                        numStockMinimo.Value = _productoSeleccionado.StockMinimo;
                        numImpuesto.Value = _productoSeleccionado.Impuesto;
                        
                        if (cboProveedor.Items.Count > 0 && _productoSeleccionado.ProveedorId > 0)
                        {
                            cboProveedor.SelectedValue = _productoSeleccionado.ProveedorId;
                        }

                        _modoEdicion = true;
                        lblModo.Text = $"Editando: {_productoSeleccionado.Nombre}";
                        lblModo.ForeColor = Color.OrangeRed;
                        btnGuardar.Text = "Actualizar (F5)";
                    }
                }
            }
        }

        private void ProductosForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                btnGuardar_Click(sender, e);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                LimpiarFormulario();
                e.Handled = true;
            }
        }
    }
}