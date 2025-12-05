using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services; // Importamos el Servicio
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

            // GARANTÍA DE TECLAS RÁPIDAS
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(ProductosForm_KeyDown);
            txtCodigo.KeyDown += new KeyEventHandler(txtCodigo_KeyDown);

            CargarProveedores();
            CargarProductos();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtCodigo.Focus();
        }

        // ... (Lógica de Escaneo y Carga de Combos se mantiene igual) ...

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                BuscarProductoPorCodigo(txtCodigo.Text.Trim());
            }
        }

        private void BuscarProductoPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return;

            using (var context = new AlmacenDbContext())
            {
                var producto = context.Productos.FirstOrDefault(p => p.CodigoBarras == codigo);

                if (producto != null)
                {
                    _productoIdSeleccionado = producto.Id;
                    txtNombre.Text = producto.Nombre;
                    numCosto.Value = producto.Costo;
                    numImpuesto.Value = producto.Impuesto;
                    numPrecio.Value = producto.Precio;
                    numStock.Value = producto.Stock;
                    cboProveedor.SelectedValue = producto.ProveedorId;

                    btnGuardar.Text = "Modificar (F5)";
                    txtNombre.Focus();
                    txtNombre.SelectAll();
                }
                else
                {
                    LimpiarCamposMenosCodigo();
                    btnGuardar.Text = "Guardar (F5)";
                    txtNombre.Focus();
                }
            }
        }

        private void LimpiarCamposMenosCodigo()
        {
            _productoIdSeleccionado = null;
            txtNombre.Clear();
            numCosto.Value = 0;
            numPrecio.Value = 0;
            numStock.Value = 0;
            numImpuesto.Value = 0;
            cboProveedor.SelectedIndex = -1;
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
                var lista = context.Productos
                    .Include(p => p.Proveedor)
                    .Select(p => new
                    {
                        Id = p.Id,
                        Codigo = p.CodigoBarras,
                        Nombre = p.Nombre,
                        Costo = p.Costo,
                        Margen = p.Impuesto,
                        Precio = p.Precio,
                        Stock = p.Stock,
                        Proveedor = p.Proveedor != null ? p.Proveedor.Nombre : "Sin Proveedor"
                    })
                    .ToList();

                dgvProductos.DataSource = lista;
                if (dgvProductos.Columns["Id"] != null) dgvProductos.Columns["Id"].Visible = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarProducto();
        }

        private void GuardarProducto()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El Nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("El Código es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numPrecio.Value < numCosto.Value)
            {
                if (MessageBox.Show("⚠️ El Precio es MENOR al Costo.\n¿Guardar igual?", "Alerta", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
            }

            using (var context = new AlmacenDbContext())
            {
                bool codigoExiste = context.Productos.Any(p => p.CodigoBarras == txtCodigo.Text && p.Id != (_productoIdSeleccionado ?? 0));

                if (codigoExiste)
                {
                    MessageBox.Show($"El código '{txtCodigo.Text}' ya existe.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCodigo.SelectAll();
                    txtCodigo.Focus();
                    return;
                }

                Producto producto;

                if (_productoIdSeleccionado == null)
                {
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

            MessageBox.Show("Guardado.");
            Limpiar();
        }

        // --- EXPORTAR EXCEL ---
        private void btnExportar_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.xlsx)|*.xlsx";
            sfd.FileName = "Productos_" + DateTime.Now.ToString("yyyyMMdd");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var context = new AlmacenDbContext())
                    {
                        var lista = context.Productos.ToList();
                        var servicio = new ExcelService();
                        servicio.ExportarProductos(sfd.FileName, lista);
                        MessageBox.Show("Exportación exitosa.", "Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message);
                }
            }
        }

        // --- IMPORTAR EXCEL O CSV ---
        private void btnImportar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de Datos (*.xlsx;*.csv)|*.xlsx;*.csv";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var servicio = new ExcelService();
                    var productosImportados = servicio.LeerProductos(ofd.FileName);

                    if (productosImportados.Count == 0)
                    {
                        MessageBox.Show("El archivo está vacío o no tiene el formato correcto.");
                        return;
                    }

                    int nuevos = 0;
                    int actualizados = 0;

                    using (var context = new AlmacenDbContext())
                    {
                        foreach (var dto in productosImportados)
                        {
                            // VALIDACIÓN DE SEGURIDAD:
                            // Buscamos si ya existe por código de barras
                            var productoExistente = context.Productos.FirstOrDefault(p => p.CodigoBarras == dto.CodigoBarras);

                            if (productoExistente != null)
                            {
                                // ACTUALIZAMOS (Respetando ID existente)
                                productoExistente.Nombre = dto.Nombre;
                                productoExistente.Costo = dto.Costo;
                                productoExistente.Impuesto = dto.Impuesto;
                                productoExistente.Stock = dto.Stock; // Opcional: Podríamos sumar stock en vez de reemplazar
                                productoExistente.ProveedorId = dto.ProveedorId;

                                // Recalculamos precio automáticamente
                                productoExistente.Precio = dto.Costo * (1 + (dto.Impuesto / 100m));

                                actualizados++;
                            }
                            else
                            {
                                // CREAMOS NUEVO
                                var nuevoProd = new Producto
                                {
                                    CodigoBarras = dto.CodigoBarras,
                                    Nombre = dto.Nombre,
                                    Costo = dto.Costo,
                                    Impuesto = dto.Impuesto,
                                    Stock = dto.Stock,
                                    ProveedorId = dto.ProveedorId,
                                    // Calculamos precio
                                    Precio = dto.Costo * (1 + (dto.Impuesto / 100m))
                                };
                                context.Productos.Add(nuevoProd);
                                nuevos++;
                            }
                        }
                        context.SaveChanges();
                    }

                    MessageBox.Show($"Proceso terminado.\nNuevos: {nuevos}\nActualizados: {actualizados}", "Importación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Limpiar(); // Recarga la grilla
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error crítico al importar: " + ex.Message);
                }
            }
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                var row = dgvProductos.SelectedRows[0];
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

                        btnGuardar.Text = "Modificar (F5)";
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

            btnGuardar.Text = "Guardar (F5)";
            CargarProductos();
            txtCodigo.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_productoIdSeleccionado == null) return;

            using (var context = new AlmacenDbContext())
            {
                bool tieneVentas = context.DetallesVenta.Any(d => d.ProductoId == _productoIdSeleccionado);
                if (tieneVentas)
                {
                    MessageBox.Show("No se puede eliminar: Tiene historial de ventas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MessageBox.Show("¿Eliminar producto?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

        // --- MOTOR DE TECLAS RÁPIDAS (SAGRADO) ---
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
            // Podríamos agregar F9 para Importar y F10 para Exportar si quieres
        }
    }
}