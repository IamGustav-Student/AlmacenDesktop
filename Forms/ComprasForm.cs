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

        public ComprasForm()
        {
            InitializeComponent();
            cboProveedores.SelectedIndexChanged += cboProveedores_SelectedIndexChanged;
            cboProductos.SelectedIndexChanged += cboProductos_SelectedIndexChanged;
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
            // Aquí podrías mostrar el impuesto actual si tuvieras un label
            // var producto = (Producto)cboProductos.SelectedItem;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
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

                            // 1. Actualizamos el Costo
                            prodDb.Costo = item.CostoUnitario;

                            // 2. ACTUALIZACIÓN AUTOMÁTICA DE PRECIOS USANDO EL IMPUESTO
                            if (prodDb.Impuesto > 0)
                            {
                                prodDb.Precio = prodDb.Costo * (1 + (prodDb.Impuesto / 100m));
                            }
                        }
                    }

                    // ... Lógica de caja ...
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
                    }

                    context.SaveChanges();
                    MessageBox.Show("Compra registrada correctamente.", "Éxito");

                    _carritoCompra.Clear();
                    ActualizarGrilla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }

    // DISEÑO VISUAL
    partial class ComprasForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblProv, lblProd, lblCant, lblCosto, lblTotal;
        private ComboBox cboProveedores, cboProductos;
        private NumericUpDown numCantidad, numCosto;
        private Button btnAgregar, btnFinalizar;
        private DataGridView dgvDetalle;

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
            this.Text = "Ingreso de Mercadería (Compras)";
            this.StartPosition = FormStartPosition.CenterScreen;

            lblProv = new Label { Text = "Proveedor:", Location = new Point(20, 20), AutoSize = true };
            cboProveedores = new ComboBox { Location = new Point(20, 40), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };

            lblProd = new Label { Text = "Producto:", Location = new Point(20, 80), AutoSize = true };
            cboProductos = new ComboBox { Location = new Point(20, 100), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };

            lblCant = new Label { Text = "Cantidad:", Location = new Point(340, 80), AutoSize = true };
            numCantidad = new NumericUpDown { Location = new Point(340, 100), Width = 80, Minimum = 1, Maximum = 10000 };

            lblCosto = new Label { Text = "Costo Unitario ($):", Location = new Point(440, 80), AutoSize = true };
            numCosto = new NumericUpDown { Location = new Point(440, 100), Width = 120, DecimalPlaces = 2, Maximum = 1000000 };

            btnAgregar = new Button { Text = "Agregar (+)", Location = new Point(580, 95), Width = 100, Height = 30, BackColor = Color.SteelBlue, ForeColor = Color.White };
            btnAgregar.Click += btnAgregar_Click;

            dgvDetalle = new DataGridView { Location = new Point(20, 150), Size = new Size(740, 300), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true };

            lblTotal = new Label { Text = "Total Compra: $ 0.00", Location = new Point(450, 470), Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true };

            btnFinalizar = new Button { Text = "CONFIRMAR INGRESO", Location = new Point(20, 470), Width = 300, Height = 50, BackColor = Color.ForestGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            btnFinalizar.Click += btnFinalizar_Click;

            this.Controls.AddRange(new Control[] { lblProv, cboProveedores, lblProd, cboProductos, lblCant, numCantidad, lblCosto, numCosto, btnAgregar, dgvDetalle, lblTotal, btnFinalizar });
        }
    }
}