using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ProveedoresForm : Form
    {
        private int _idSeleccionado = 0;

        public ProveedoresForm()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void ProveedoresForm_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            using (var context = new AlmacenDbContext())
            {
                dgvDatos.DataSource = null;
                dgvDatos.DataSource = context.Proveedores.ToList();
                if (dgvDatos.Columns["Id"] != null) dgvDatos.Columns["Id"].Visible = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("El nombre es obligatorio."); return; }

            using (var context = new AlmacenDbContext())
            {
                if (_idSeleccionado == 0)
                {
                    context.Proveedores.Add(new Proveedor
                    {
                        Nombre = txtNombre.Text,
                        Telefono = txtTelefono.Text,
                        Direccion = txtDireccion.Text,
                        Cuit = txtCuit.Text,
                        Contacto = txtContacto.Text
                    });
                }
                else
                {
                    var prov = context.Proveedores.Find(_idSeleccionado);
                    if (prov != null)
                    {
                        prov.Nombre = txtNombre.Text; prov.Telefono = txtTelefono.Text;
                        prov.Direccion = txtDireccion.Text; prov.Cuit = txtCuit.Text; prov.Contacto = txtContacto.Text;
                    }
                }
                context.SaveChanges();
                Limpiar();
                CargarDatos();
                MessageBox.Show("Guardado.");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0) return;
            if (MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var context = new AlmacenDbContext())
                {
                    var p = context.Proveedores.Find(_idSeleccionado);
                    if (p != null) { context.Proveedores.Remove(p); context.SaveChanges(); }
                }
                Limpiar();
                CargarDatos();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            txtNombre.Clear(); txtTelefono.Clear(); txtDireccion.Clear(); txtCuit.Clear(); txtContacto.Clear();
            _idSeleccionado = 0;
            btnGuardar.Text = "Guardar";
        }

        private void dgvDatos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDatos.SelectedRows.Count > 0)
            {
                var prov = (Proveedor)dgvDatos.SelectedRows[0].DataBoundItem;
                _idSeleccionado = prov.Id;
                txtNombre.Text = prov.Nombre; txtTelefono.Text = prov.Telefono;
                txtDireccion.Text = prov.Direccion; txtCuit.Text = prov.Cuit; txtContacto.Text = prov.Contacto;
                btnGuardar.Text = "Actualizar";
            }
        }
    }

    // DISEÑO VISUAL SIMPLIFICADO
    partial class ProveedoresForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblNombre, lblTel, lblDir, lblCuit, lblContacto;
        private TextBox txtNombre, txtTelefono, txtDireccion, txtCuit, txtContacto;
        private Button btnGuardar, btnEliminar, btnLimpiar;
        private DataGridView dgvDatos;
        private GroupBox grpEdicion;

        private void InitializeComponent()
        {
            this.Size = new Size(800, 500);
            this.Text = "Gestión de Proveedores";
            this.StartPosition = FormStartPosition.CenterScreen;

            grpEdicion = new GroupBox { Text = "Datos del Proveedor", Location = new Point(10, 10), Size = new Size(300, 430) };

            lblNombre = new Label { Text = "Empresa / Nombre:", Location = new Point(10, 30), AutoSize = true };
            txtNombre = new TextBox { Location = new Point(10, 50), Width = 280 };

            lblContacto = new Label { Text = "Contacto (Vendedor):", Location = new Point(10, 80), AutoSize = true };
            txtContacto = new TextBox { Location = new Point(10, 100), Width = 280 };

            lblTel = new Label { Text = "Teléfono:", Location = new Point(10, 130), AutoSize = true };
            txtTelefono = new TextBox { Location = new Point(10, 150), Width = 280 };

            lblDir = new Label { Text = "Dirección:", Location = new Point(10, 180), AutoSize = true };
            txtDireccion = new TextBox { Location = new Point(10, 200), Width = 280 };

            lblCuit = new Label { Text = "CUIT:", Location = new Point(10, 230), AutoSize = true };
            txtCuit = new TextBox { Location = new Point(10, 250), Width = 280 };

            btnGuardar = new Button { Text = "Guardar", Location = new Point(10, 300), Width = 280, Height = 40, BackColor = Color.SteelBlue, ForeColor = Color.White };
            btnGuardar.Click += btnGuardar_Click;

            btnLimpiar = new Button { Text = "Limpiar / Nuevo", Location = new Point(10, 350), Width = 135, Height = 30 };
            btnLimpiar.Click += btnLimpiar_Click;

            btnEliminar = new Button { Text = "Eliminar", Location = new Point(155, 350), Width = 135, Height = 30, BackColor = Color.Firebrick, ForeColor = Color.White };
            btnEliminar.Click += btnEliminar_Click;

            grpEdicion.Controls.AddRange(new Control[] { lblNombre, txtNombre, lblContacto, txtContacto, lblTel, txtTelefono, lblDir, txtDireccion, lblCuit, txtCuit, btnGuardar, btnLimpiar, btnEliminar });

            dgvDatos = new DataGridView { Location = new Point(320, 20), Size = new Size(450, 420), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false };
            dgvDatos.SelectionChanged += dgvDatos_SelectionChanged;

            this.Controls.Add(grpEdicion);
            this.Controls.Add(dgvDatos);
        }
    }
}