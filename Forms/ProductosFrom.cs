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
        private int _productoIdSeleccionado = 0;

        public ProductosForm()
        {
            InitializeComponent();
        }

        private void ProductosForm_Load(object sender, EventArgs e)
        {
            CargarDatos();
            // Poner el foco en el código al abrir para empezar a escanear de una
            this.ActiveControl = txtCodigo;
        }

        private void CargarDatos()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var lista = context.Productos.ToList();
                    dgvProductos.DataSource = null;
                    dgvProductos.DataSource = lista;

                    if (dgvProductos.Columns["Id"] != null)
                        dgvProductos.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}");
            }
        }

        // --- NUEVA LÓGICA: ESCÁNER ---
        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Detectar el ENTER del lector de códigos
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Evitar el sonido beep
                string codigo = txtCodigo.Text.Trim();

                if (!string.IsNullOrEmpty(codigo))
                {
                    BuscarPorCodigo(codigo);
                }
            }
        }

        private void BuscarPorCodigo(string codigo)
        {
            using (var context = new AlmacenDbContext())
            {
                var producto = context.Productos.FirstOrDefault(p => p.CodigoBarras == codigo);

                if (producto != null)
                {
                    // CASO 1: El producto YA EXISTE.
                    // Lo cargamos en pantalla para editarlo (ej. agregar más stock)
                    _productoIdSeleccionado = producto.Id;
                    txtNombre.Text = producto.Nombre;
                    numPrecio.Value = producto.Precio;
                    numStock.Value = producto.Stock;

                    // Seleccionamos visualmente en la grilla para que el usuario sepa cuál es
                    foreach (DataGridViewRow row in dgvProductos.Rows)
                    {
                        var prodGrilla = (Producto)row.DataBoundItem;
                        if (prodGrilla.Id == producto.Id)
                        {
                            row.Selected = true;
                            dgvProductos.FirstDisplayedScrollingRowIndex = row.Index;
                            break;
                        }
                    }

                    btnGuardar.Text = "ACTUALIZAR (Producto Existente)";
                    MessageBox.Show("Producto encontrado. Puede editar el stock o el precio.", "Producto Existente", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Ponemos foco en Stock para actualización rápida
                    numStock.Focus();
                    numStock.Select(0, numStock.ToString().Length);
                }
                else
                {
                    // CASO 2: El producto NO EXISTE.
                    // Preparamos para crear uno nuevo
                    // Limpiamos todo MENOS el código que acabamos de escanear
                    LimpiarExceptoCodigo();

                    btnGuardar.Text = "GUARDAR (Nuevo Producto)";
                    // Foco en nombre para escribirlo rápido
                    txtNombre.Focus();
                }
            }
        }

        private void LimpiarExceptoCodigo()
        {
            // Guarda el código actual
            string codigoActual = txtCodigo.Text;

            // Limpia todo usando el método base
            LimpiarFormulario();

            // Restaura el código y el estado de "Nuevo"
            txtCodigo.Text = codigoActual;
            _productoIdSeleccionado = 0;
        }
        // -----------------------------

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // 1. Validaciones básicas
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || numPrecio.Value <= 0)
            {
                MessageBox.Show("El nombre y el precio son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    // 2. VALIDACIONES DE DUPLICADOS

                    // A) Validar Nombre Duplicado
                    bool nombreExiste = context.Productos.Any(p => p.Nombre.ToLower() == txtNombre.Text.ToLower() && p.Id != _productoIdSeleccionado);
                    if (nombreExiste)
                    {
                        MessageBox.Show("Ya existe un producto con ese Nombre.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // B) Validar Código de Barras Duplicado
                    if (!string.IsNullOrWhiteSpace(txtCodigo.Text))
                    {
                        bool codigoExiste = context.Productos.Any(p => p.CodigoBarras == txtCodigo.Text && p.Id != _productoIdSeleccionado);
                        if (codigoExiste)
                        {
                            MessageBox.Show("Ya existe un producto con ese Código de Barras.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (_productoIdSeleccionado == 0)
                    {
                        var nuevoProducto = new Producto
                        {
                            Nombre = txtNombre.Text,
                            CodigoBarras = txtCodigo.Text,
                            Precio = numPrecio.Value,
                            Stock = (int)numStock.Value
                        };
                        context.Productos.Add(nuevoProducto);
                    }
                    else
                    {
                        var productoExistente = context.Productos.Find(_productoIdSeleccionado);
                        if (productoExistente != null)
                        {
                            productoExistente.Nombre = txtNombre.Text;
                            productoExistente.CodigoBarras = txtCodigo.Text;
                            productoExistente.Precio = numPrecio.Value;
                            productoExistente.Stock = (int)numStock.Value;
                            context.Productos.Update(productoExistente);
                        }
                    }

                    context.SaveChanges();

                    MessageBox.Show("Datos guardados correctamente.");
                    LimpiarFormulario();
                    CargarDatos();

                    // Volver foco al código para seguir escaneando
                    txtCodigo.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_productoIdSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto de la lista para eliminar.");
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este producto?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (var context = new AlmacenDbContext())
                    {
                        var producto = context.Productos.Find(_productoIdSeleccionado);
                        if (producto != null)
                        {
                            context.Productos.Remove(producto);
                            context.SaveChanges();
                            MessageBox.Show("Producto eliminado.");
                            LimpiarFormulario();
                            CargarDatos();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar: {ex.Message}");
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            txtCodigo.Focus(); // Volver a poner foco para escanear
        }

        private void LimpiarFormulario()
        {
            txtNombre.Clear();
            txtCodigo.Clear();
            numPrecio.Value = 0;
            numStock.Value = 0;
            _productoIdSeleccionado = 0;
            btnGuardar.Text = "GUARDAR / CREAR";
            dgvProductos.ClearSelection();
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                var fila = dgvProductos.SelectedRows[0];
                var producto = (Producto)fila.DataBoundItem;

                _productoIdSeleccionado = producto.Id;
                txtNombre.Text = producto.Nombre;
                txtCodigo.Text = producto.CodigoBarras;
                numPrecio.Value = producto.Precio;
                numStock.Value = producto.Stock;

                btnGuardar.Text = "ACTUALIZAR";
            }
        }
    }
}