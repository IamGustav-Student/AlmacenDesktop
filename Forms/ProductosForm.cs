using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ProductosForm : Form
    {
        private Producto _productoSeleccionado;
        private readonly ExcelService _excelService;
        private ErrorProvider _errorProvider;

        public ProductosForm()
        {
            InitializeComponent();
            _excelService = new ExcelService();

            _errorProvider = new ErrorProvider();
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            // EVENTO DE VALIDACIÓN EN TIEMPO REAL (KEYPRESS)
            txtCodigo.KeyPress += new KeyPressEventHandler(txtCodigo_KeyPress);

            ConfigurarGrilla();
        }

        private void ProductosForm_Load(object sender, EventArgs e)
        {
            try
            {
                AsegurarProveedorGeneral();
                CargarProveedores();
                CargarProductos();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show($"Error crítico iniciando Productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void ConfigurarGrilla()
        {
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.BorderStyle = BorderStyle.None;
        }

        private void AsegurarProveedorGeneral()
        {
            using (var context = new AlmacenDbContext())
            {
                if (!context.Proveedores.Any())
                {
                    var general = new Proveedor
                    {
                        Nombre = "PROVEEDOR GENERAL",
                        Contacto = "SISTEMA",
                        Direccion = "-",
                        Telefono = "-",
                        Cuit = "-"
                    };
                    context.Proveedores.Add(general);
                    context.SaveChanges();
                }
            }
        }

        private void CargarProveedores()
        {
            using (var context = new AlmacenDbContext())
            {
                var proveedores = context.Proveedores.OrderBy(p => p.Nombre).ToList();
                cboProveedor.DataSource = proveedores;
                cboProveedor.DisplayMember = "Nombre";
                cboProveedor.ValueMember = "Id";
                cboProveedor.SelectedIndex = -1;
            }
        }

        private void CargarProductos(string filtro = "")
        {
            using (var context = new AlmacenDbContext())
            {
                var query = context.Productos.Include(p => p.Proveedor).AsQueryable();

                if (!string.IsNullOrEmpty(filtro))
                {
                    query = query.Where(p => p.Nombre.Contains(filtro) || p.CodigoBarras.Contains(filtro));
                }

                var lista = query.Select(p => new
                {
                    p.Id,
                    Codigo = p.CodigoBarras,
                    p.Nombre,
                    p.Precio,
                    Stock = p.Stock,
                    Proveedor = p.Proveedor != null ? p.Proveedor.Nombre : "SIN ASIGNAR"
                }).ToList();

                dgvProductos.DataSource = lista;
                if (dgvProductos.Columns["Id"] != null) dgvProductos.Columns["Id"].Visible = false;
            }
        }

        // --- VALIDACIÓN DE TECLADO: SOLO NÚMEROS ---
        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo dígitos y teclas de control (BackSpace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Bloquea la tecla

                // Feedback visual opcional (parpadeo o sonido sutil)
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormularioConUX())
            {
                AudioHelper.PlayError();
                return;
            }

            if (numPrecio.Value < numCosto.Value)
            {
                var result = MessageBox.Show(
                    $"¡CUIDADO! El precio de venta (${numPrecio.Value}) es MENOR al costo (${numCosto.Value}).\n¿Desea guardar de todas formas y perder dinero?",
                    "Alerta de Rentabilidad",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No) return;
            }

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    int proveedorIdSeleccionado = Convert.ToInt32(cboProveedor.SelectedValue);

                    if (_productoSeleccionado == null)
                    {
                        if (context.Productos.Any(p => p.CodigoBarras == txtCodigo.Text.Trim()))
                        {
                            _errorProvider.SetError(txtCodigo, "Este código ya existe.");
                            AudioHelper.PlayError();
                            MessageBox.Show("El Código de Barras ya existe.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var nuevo = new Producto
                        {
                            CodigoBarras = txtCodigo.Text.Trim(),
                            Nombre = txtNombre.Text.Trim(),
                            Descripcion = txtDescripcion.Text.Trim(),
                            Costo = numCosto.Value,
                            Precio = numPrecio.Value,
                            Stock = Convert.ToInt32(numStock.Value),
                            StockMinimo = Convert.ToInt32(numStockMinimo.Value),
                            ProveedorId = proveedorIdSeleccionado
                        };
                        context.Productos.Add(nuevo);
                        MessageBox.Show("Producto creado exitosamente.");
                    }
                    else
                    {
                        var productoDb = context.Productos.Find(_productoSeleccionado.Id);
                        if (productoDb != null)
                        {
                            if (context.Productos.Any(p => p.CodigoBarras == txtCodigo.Text.Trim() && p.Id != _productoSeleccionado.Id))
                            {
                                MessageBox.Show("El código de barras ya pertenece a otro producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            productoDb.CodigoBarras = txtCodigo.Text.Trim();
                            productoDb.Nombre = txtNombre.Text.Trim();
                            productoDb.Descripcion = txtDescripcion.Text.Trim();
                            productoDb.Costo = numCosto.Value;
                            productoDb.Precio = numPrecio.Value;
                            productoDb.Stock = Convert.ToInt32(numStock.Value);
                            productoDb.StockMinimo = Convert.ToInt32(numStockMinimo.Value);
                            productoDb.ProveedorId = proveedorIdSeleccionado;
                        }
                        MessageBox.Show("Producto actualizado.");
                    }
                    context.SaveChanges();
                    AudioHelper.PlayOk();
                }

                LimpiarFormulario();
                CargarProductos();
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show($"Error al guardar:\n{ex.Message}", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarFormularioConUX()
        {
            bool esValido = true;
            _errorProvider.Clear();

            // 1. CÓDIGO: No vacío Y Numérico
            if (!ValidationHelper.ValidarCampo(txtCodigo, _errorProvider, "El código es obligatorio y debe ser numérico.",
                () => {
                    string texto = txtCodigo.Text.Trim();
                    // Valida que no esté vacío Y que sea un número válido (long para soportar EAN-13)
                    return !string.IsNullOrWhiteSpace(texto) && long.TryParse(texto, out _);
                })) esValido = false;

            if (!ValidationHelper.ValidarCampo(txtNombre, _errorProvider, "El nombre es obligatorio.",
                () => !string.IsNullOrWhiteSpace(txtNombre.Text))) esValido = false;

            if (!ValidationHelper.ValidarCampo(cboProveedor, _errorProvider, "Seleccione un proveedor.",
                () => cboProveedor.SelectedValue != null)) esValido = false;

            return esValido;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null) return;

            if (MessageBox.Show($"¿Eliminar '{_productoSeleccionado.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (var context = new AlmacenDbContext())
                    {
                        var prod = context.Productos.Find(_productoSeleccionado.Id);
                        if (prod != null)
                        {
                            context.Productos.Remove(prod);
                            context.SaveChanges();
                            AudioHelper.PlayOk();
                        }
                    }
                    LimpiarFormulario();
                    CargarProductos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se puede eliminar el producto. Puede que tenga ventas asociadas o historial.\n\nSugerencia: Cambie el nombre a 'INACTIVO' o el stock a 0.", "Restricción de Datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e) => LimpiarFormulario();

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                var celdaId = dgvProductos.SelectedRows[0].Cells["Id"].Value;
                if (celdaId == null) return;
                int id = Convert.ToInt32(celdaId);

                using (var context = new AlmacenDbContext())
                {
                    _productoSeleccionado = context.Productos.Find(id);
                }
                if (_productoSeleccionado != null) LlenarFormulario(_productoSeleccionado);
            }
        }

        private void LlenarFormulario(Producto p)
        {
            txtCodigo.Text = p.CodigoBarras;
            txtNombre.Text = p.Nombre;
            txtDescripcion.Text = p.Descripcion;
            numCosto.Value = p.Costo;
            numPrecio.Value = p.Precio;
            numStock.Value = p.Stock;
            numStockMinimo.Value = p.StockMinimo;
            cboProveedor.SelectedValue = p.ProveedorId;

            _errorProvider.Clear();
            txtCodigo.BackColor = Color.White;
            txtNombre.BackColor = Color.White;

            btnEliminar.Enabled = true;
            btnGuardar.Text = "MODIFICAR PRODUCTO";
        }

        private void LimpiarFormulario()
        {
            _productoSeleccionado = null;
            txtCodigo.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            numCosto.Value = 0;
            numPrecio.Value = 0;
            numStock.Value = 0;
            numStockMinimo.Value = 5;
            cboProveedor.SelectedIndex = -1;

            _errorProvider.Clear();
            txtCodigo.BackColor = Color.White;
            txtNombre.BackColor = Color.White;

            dgvProductos.ClearSelection();
            txtCodigo.Focus();

            btnGuardar.Text = "GUARDAR PRODUCTO";
            btnEliminar.Enabled = false;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e) => CargarProductos(txtBuscar.Text);

        private void btnImportar_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog() { Filter = "Excel Files|*.xlsx" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        var resultado = _excelService.ImportarProductosInteligente(ofd.FileName);
                        AudioHelper.PlayOk();
                        MessageBox.Show($"Importación Finalizada:\n\nNuevos: {resultado.Nuevos}\nActualizados: {resultado.Actualizados}\nErrores: {resultado.Errores}", "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarProductos();
                    }
                    catch (Exception ex)
                    {
                        AudioHelper.PlayError();
                        MessageBox.Show("Error importando: " + ex.Message);
                    }
                    finally { this.Cursor = Cursors.Default; }
                }
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog() { Filter = "Excel Files|*.xlsx", FileName = $"Inventario_{DateTime.Now:yyyyMMdd}.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        List<Producto> listaParaExportar;
                        using (var context = new AlmacenDbContext())
                        {
                            listaParaExportar = context.Productos.Include(p => p.Proveedor).ToList();
                        }
                        _excelService.ExportarProductos(sfd.FileName, listaParaExportar);
                        AudioHelper.PlayOk();
                        MessageBox.Show("Exportación exitosa.");
                    }
                    catch (Exception ex)
                    {
                        AudioHelper.PlayError();
                        MessageBox.Show("Error exportando: " + ex.Message);
                    }
                    finally { this.Cursor = Cursors.Default; }
                }
            }
        }
    }
}