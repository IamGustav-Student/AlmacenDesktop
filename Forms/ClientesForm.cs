using AlmacenDesktop.Data;
using AlmacenDesktop.Helpers;
using AlmacenDesktop.Modelos;
using System;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ClientesForm : Form
    {
        private int _clienteIdSeleccionado = 0;

        public ClientesForm()
        {
            InitializeComponent();

            // --- HABILITAR ATAJOS DE TECLADO ---
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(ClientesForm_KeyDown);
        }

        private void ClientesForm_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            using (var context = new AlmacenDbContext())
            {
                // No mostrar al consumidor final por defecto para no editarlo por error
                var lista = context.Clientes
                                   .Where(c => c.DniCuit != Constantes.CLIENTE_DEF_DNI)
                                   .ToList();
                dgvClientes.DataSource = null;
                dgvClientes.DataSource = lista;

                if (dgvClientes.Columns["Id"] != null) dgvClientes.Columns["Id"].Visible = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarCliente();
        }

        private void GuardarCliente()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text) || string.IsNullOrWhiteSpace(txtDni.Text))
            {
                MessageBox.Show("Nombre, Apellido y DNI son obligatorios.", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    // VALIDACIÓN: DNI ÚNICO
                    bool dniExiste = context.Clientes.Any(c => c.DniCuit == txtDni.Text && c.Id != _clienteIdSeleccionado);
                    if (dniExiste)
                    {
                        MessageBox.Show("Ya existe un cliente registrado con ese DNI/CUIT.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (_clienteIdSeleccionado == 0)
                    {
                        var nuevo = new Cliente
                        {
                            Nombre = txtNombre.Text,
                            Apellido = txtApellido.Text,
                            DniCuit = txtDni.Text,
                            Email = txtEmail.Text,
                            Telefono = txtTelefono.Text,
                            Direccion = txtDireccion.Text
                        };
                        context.Clientes.Add(nuevo);
                    }
                    else
                    {
                        var existente = context.Clientes.Find(_clienteIdSeleccionado);
                        if (existente != null)
                        {
                            existente.Nombre = txtNombre.Text;
                            existente.Apellido = txtApellido.Text;
                            existente.DniCuit = txtDni.Text;
                            existente.Email = txtEmail.Text;
                            existente.Telefono = txtTelefono.Text;
                            existente.Direccion = txtDireccion.Text;
                            context.Clientes.Update(existente);
                        }
                    }
                    context.SaveChanges();
                    MessageBox.Show("Cliente guardado correctamente.");
                    Limpiar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_clienteIdSeleccionado == 0) return;

            using (var context = new AlmacenDbContext())
            {
                // VALIDACIÓN DE INTEGRIDAD
                bool tieneVentas = context.Ventas.Any(v => v.ClienteId == _clienteIdSeleccionado);
                bool tienePagos = context.Pagos.Any(p => p.ClienteId == _clienteIdSeleccionado);

                if (tieneVentas || tienePagos)
                {
                    MessageBox.Show("No se puede eliminar este cliente porque tiene historial de Ventas o Pagos.\nEsto rompería los reportes de Caja y Cuenta Corriente.", "Acción Bloqueada", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (MessageBox.Show("¿Seguro que deseas eliminar este cliente?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var cliente = context.Clientes.Find(_clienteIdSeleccionado);
                    if (cliente != null)
                    {
                        context.Clientes.Remove(cliente);
                        context.SaveChanges();
                        Limpiar();
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
            txtNombre.Clear();
            txtApellido.Clear();
            txtDni.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            _clienteIdSeleccionado = 0;
            btnGuardar.Text = "GUARDAR CLIENTE";
            dgvClientes.ClearSelection();
            CargarDatos();
            // Devolvemos el foco al primer campo para seguir cargando rápido
            txtNombre.Focus();
        }

        private void dgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                var fila = dgvClientes.SelectedRows[0];
                var cliente = (Cliente)fila.DataBoundItem;

                _clienteIdSeleccionado = cliente.Id;
                txtNombre.Text = cliente.Nombre;
                txtApellido.Text = cliente.Apellido;
                txtDni.Text = cliente.DniCuit;
                txtEmail.Text = cliente.Email;
                txtTelefono.Text = cliente.Telefono;
                txtDireccion.Text = cliente.Direccion;

                btnGuardar.Text = "ACTUALIZAR CLIENTE";
            }
        }

        // --- MANEJO DE TECLAS RÁPIDAS ---
        private void ClientesForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                GuardarCliente();
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