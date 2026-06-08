using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ComprasForm : Form
    {
        private List<DetalleCompra> _carritoCompra = new List<DetalleCompra>();
        private List<Producto> _todosLosProductos = new List<Producto>();
        private Usuario _usuarioActual;

        public ComprasForm(Usuario usuario)
        {
            InitializeComponent();
            _usuarioActual = usuario;

            // --- ESCÁNER Y TECLAS RÁPIDAS ---
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(ComprasForm_KeyDown);
            this.KeyPress += new KeyPressEventHandler(ComprasForm_GlobalKeyPress);

            cboProveedores.SelectedIndexChanged += cboProveedores_SelectedIndexChanged;
            cboProductos.SelectedIndexChanged += cboProductos_SelectedIndexChanged;
            txtEscanear.KeyPress += new KeyPressEventHandler(txtEscanear_KeyPress);
        }

        // ── CAPTURA INTELIGENTE GLOBAL (cualquier tecla → escáner) ─────────────
        private void ComprasForm_GlobalKeyPress(object sender, KeyPressEventArgs e)
        {
            // No interferir si el usuario está escribiendo en campos manuales
            if (this.ActiveControl == numCantidad ||
                this.ActiveControl == numCosto ||
                cboProveedores.DroppedDown ||
                cboProductos.DroppedDown)
            {
                return;
            }

            if (char.IsControl(e.KeyChar)) return;

            // Si el foco ya está en el campo del escáner, el SO escribe el carácter solo
            if (txtEscanear.Focused) return;

            // Redirigir al campo de escaneo
            e.Handled = true;
            txtEscanear.Focus();
            txtEscanear.Text += e.KeyChar;
            txtEscanear.SelectionStart = txtEscanear.Text.Length;
        }

        // ── LÓGICA AL TERMINAR EL ESCANEO (Enter del pistola) ─────────────────
        private void txtEscanear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            e.Handled = true;

            string codigo = txtEscanear.Text.Trim();
            if (string.IsNullOrEmpty(codigo)) return;

            // Buscar en la lista completa de productos (no solo los filtrados)
            var producto = _todosLosProductos.FirstOrDefault(
                p => p.CodigoBarras != null &&
                     p.CodigoBarras.Equals(codigo, StringComparison.OrdinalIgnoreCase));

            if (producto == null)
            {
                System.Media.SystemSounds.Exclamation.Play();
                MessageBox.Show($"Producto con código '{codigo}' no encontrado.\nVerifique el código o cárguelo primero en el módulo de Productos.",
                    "Código no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEscanear.Clear();
                return;
            }

            // Si el producto tiene proveedor asignado, seleccionarlo automáticamente
            if (producto.ProveedorId > 0)
            {
                cboProveedores.SelectedValue = producto.ProveedorId;
            }

            // Seleccionar el producto en el combo
            var itemEnCombo = (cboProductos.DataSource as List<Producto>)
                              ?.FirstOrDefault(p => p.Id == producto.Id);

            if (itemEnCombo != null)
                cboProductos.SelectedItem = itemEnCombo;
            else
                cboProductos.SelectedValue = producto.Id;

            // Pre-cargar costo actual del producto
            if (producto.Costo > 0)
                numCosto.Value = producto.Costo;

            System.Media.SystemSounds.Beep.Play();
            txtEscanear.Clear();
            numCantidad.Focus();
            numCantidad.Select();
        }

        private void ComprasForm_Load(object sender, EventArgs e)
        {
            CargarCombos();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            object provSel = cboProveedores.SelectedValue;
            CargarCombos();
            if (provSel != null && provSel is int)
            {
                cboProveedores.SelectedValue = provSel;
            }
        }

        private void CargarCombos()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var proveedores = context.Proveedores.AsNoTracking().ToList();
                    _todosLosProductos = context.Productos.AsNoTracking().ToList();

                    cboProveedores.DisplayMember = "Nombre";
                    cboProveedores.ValueMember = "Id";
                    cboProveedores.DataSource = proveedores;

                    FiltrarProductos();
                }
            }
            catch { }
        }

        private void cboProveedores_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltrarProductos();
        }

        private void FiltrarProductos()
        {
            if (cboProveedores.SelectedItem == null) return;
            if (!(cboProveedores.SelectedValue is int idProveedor)) return;

            var productosFiltrados = _todosLosProductos
                                    .Where(p => p.ProveedorId == idProveedor)
                                    .ToList();

            if (productosFiltrados.Count == 0 && _todosLosProductos.Count > 0)
                cboProductos.DataSource = null;
            else
                cboProductos.DataSource = productosFiltrados;

            cboProductos.DisplayMember = "Nombre";
            cboProductos.ValueMember = "Id";
        }

        private void cboProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Opcional: Mostrar info extra
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AgregarProducto();
        }

        private void AgregarProducto()
        {
            var producto = (Producto)cboProductos.SelectedItem;
            if (producto == null) return;

            int cantidad = (int)numCantidad.Value;
            decimal costo = numCosto.Value;

            if (costo <= 0) { MessageBox.Show("Ingrese el costo unitario."); return; }

            _carritoCompra.Add(new DetalleCompra
            {
                ProductoId = producto.Id,
                Producto = producto,
                Cantidad = cantidad,
                CostoUnitario = costo
            });

            ActualizarGrilla();
            // Foco al producto para seguir cargando rápido
            cboProductos.Focus();
        }

        private void ActualizarGrilla()
        {
            dgvDetalle.DataSource = null;
            dgvDetalle.DataSource = _carritoCompra.Select(x => new
            {
                Producto = x.Producto.Nombre,
                Cantidad = x.Cantidad,
                CostoUnitario = x.CostoUnitario,
                Subtotal = x.Subtotal
            }).ToList();

            decimal total = _carritoCompra.Sum(x => x.Subtotal);
            lblTotal.Text = $"Total Compra: {total:C2}";

            if (dgvDetalle.Columns["CostoUnitario"] != null)
                dgvDetalle.Columns["CostoUnitario"].DefaultCellStyle.Format = "C2";
            if (dgvDetalle.Columns["Subtotal"] != null)
                dgvDetalle.Columns["Subtotal"].DefaultCellStyle.Format = "C2";
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            FinalizarCompra();
        }

        private void FinalizarCompra()
        {
            if (_carritoCompra.Count == 0) return;

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var nuevaCompra = new Compra
                    {
                        Fecha = DateTime.Now,
                        ProveedorId = (int)cboProveedores.SelectedValue,
                        UsuarioId = Program.UsuarioActualGlobal.Id,
                        Total = _carritoCompra.Sum(x => x.Subtotal)
                    };
                    context.Compras.Add(nuevaCompra);
                    context.SaveChanges();

                    foreach (var item in _carritoCompra)
                    {
                        var detalle = new DetalleCompra
                        {
                            CompraId = nuevaCompra.Id,
                            ProductoId = item.ProductoId,
                            Cantidad = item.Cantidad,
                            CostoUnitario = item.CostoUnitario
                        };
                        context.DetallesCompra.Add(detalle);

                        var prodDb = context.Productos.Find(item.ProductoId);
                        if (prodDb != null)
                        {
                            prodDb.AumentarStock(item.Cantidad);
                            prodDb.Costo = item.CostoUnitario;

                            if (prodDb.Impuesto > 0)
                            {
                                prodDb.Precio = prodDb.Costo * (1 + (prodDb.Impuesto / 100m));
                            }
                        }
                    }

                    var cajaAbierta = context.Cajas.FirstOrDefault(c => c.UsuarioId == Program.UsuarioActualGlobal.Id && c.EstaAbierta);
                    if (cajaAbierta != null)
                    {
                        var egreso = new MovimientoCaja
                        {
                            Fecha = DateTime.Now,
                            CajaId = cajaAbierta.Id,
                            UsuarioId = Program.UsuarioActualGlobal.Id,
                            Tipo = "EGRESO",
                            Monto = nuevaCompra.Total,
                            Descripcion = $"Compra Mercadería - Prov: {cboProveedores.Text}"
                        };
                        context.MovimientosCaja.Add(egreso);
                        MessageBox.Show("Compra registrada y descontada de caja.", "Éxito");
                    }
                    else
                    {
                        MessageBox.Show("Compra registrada (Caja cerrada, no se descontó dinero).", "Éxito");
                    }

                    context.SaveChanges();

                    _carritoCompra.Clear();
                    ActualizarGrilla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // --- MOTOR DE TECLAS RÁPIDAS ---
        private void ComprasForm_KeyDown(object sender, KeyEventArgs e)
        {
            // F5 = Finalizar
            if (e.KeyCode == Keys.F5)
            {
                FinalizarCompra();
                e.Handled = true;
            }
            // Enter en controles de texto para Agregar rápido
            else if (e.KeyCode == Keys.Enter && !btnFinalizar.Focused)
            {
                // Si no estoy en el botón finalizar, Enter intenta agregar producto
                AgregarProducto();
                e.Handled = true;
                e.SuppressKeyPress = true; // Evita el "ding"
            }
            // Escape = Salir o Cancelar
            else if (e.KeyCode == Keys.Escape)
            {
                if (_carritoCompra.Count > 0)
                {
                    if (MessageBox.Show("¿Cancelar la compra en curso?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _carritoCompra.Clear();
                        ActualizarGrilla();
                    }
                }
                else
                {
                    this.Close();
                }
                e.Handled = true;
            }
        }
    }

    // DISEÑO VISUAL
    partial class ComprasForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblEscanear, lblProv, lblProd, lblCant, lblCosto, lblTotal;
        private TextBox txtEscanear;
        private ComboBox cboProveedores, cboProductos;
        private NumericUpDown numCantidad, numCosto;
        private Button btnAgregar, btnFinalizar;
        private DataGridView dgvDetalle;

        private void InitializeComponent()
        {
            this.Size = new Size(840, 660);
            this.Text = "Ingreso de Mercadería (Compras)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += ComprasForm_Load;

            // ── ZONA DE ESCÁNER ──────────────────────────────────────────────────
            lblEscanear = new Label
            {
                Text = "📷  Escanear Código de Barras:",
                Location = new Point(20, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            txtEscanear = new TextBox
            {
                Location = new Point(20, 35),
                Width = 320,
                Height = 26,
                PlaceholderText = "Apunte el escáner aquí y escanee...",
                Font = new Font("Segoe UI", 10)
            };

            // ── PROVEEDOR ────────────────────────────────────────────────────────
            lblProv = new Label { Text = "Proveedor:", Location = new Point(20, 75), AutoSize = true };
            cboProveedores = new ComboBox { Location = new Point(20, 95), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };

            // ── PRODUCTO ─────────────────────────────────────────────────────────
            lblProd = new Label { Text = "Producto:", Location = new Point(20, 135), AutoSize = true };
            cboProductos = new ComboBox { Location = new Point(20, 155), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };

            // ── CANTIDAD / COSTO / BOTÓN ─────────────────────────────────────────
            lblCant = new Label { Text = "Cantidad:", Location = new Point(340, 135), AutoSize = true };
            numCantidad = new NumericUpDown { Location = new Point(340, 155), Width = 80, Minimum = 1, Maximum = 10000 };

            lblCosto = new Label { Text = "Costo Unitario ($):", Location = new Point(440, 135), AutoSize = true };
            numCosto = new NumericUpDown { Location = new Point(440, 155), Width = 120, DecimalPlaces = 2, Maximum = 1000000 };

            btnAgregar = new Button
            {
                Text = "Agregar (+)",
                Location = new Point(580, 150),
                Width = 110,
                Height = 30,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            btnAgregar.Click += btnAgregar_Click;

            // ── GRILLA ───────────────────────────────────────────────────────────
            dgvDetalle = new DataGridView
            {
                Location = new Point(20, 200),
                Size = new Size(790, 320),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };

            // ── TOTALES Y FINALIZAR ──────────────────────────────────────────────
            lblTotal = new Label
            {
                Text = "Total Compra: $ 0.00",
                Location = new Point(450, 535),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true
            };

            btnFinalizar = new Button
            {
                Text = "CONFIRMAR INGRESO (F5)",
                Location = new Point(20, 530),
                Width = 300,
                Height = 50,
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnFinalizar.Click += btnFinalizar_Click;

            this.Controls.AddRange(new Control[]
            {
                lblEscanear, txtEscanear,
                lblProv, cboProveedores,
                lblProd, cboProductos,
                lblCant, numCantidad,
                lblCosto, numCosto,
                btnAgregar, dgvDetalle,
                lblTotal, btnFinalizar
            });
        }
    }
}