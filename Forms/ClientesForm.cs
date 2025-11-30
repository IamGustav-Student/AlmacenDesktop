using AlmacenDesktop.Data;
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
        }

        private void ClientesForm_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            using (var context = new AlmacenDbContext())
            {
                var lista = context.Clientes.ToList();
                dgvClientes.DataSource = null;
                dgvClientes.DataSource = lista;

                // Ocultar columnas internas de EF si aparecen
                if (dgvClientes.Columns["Id"] != null) dgvClientes.Columns["Id"].Visible = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text) || string.IsNullOrWhiteSpace(txtDni.Text))
            {
                MessageBox.Show("Nombre, Apellido y DNI son obligatorios.");
                return;
            }

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    if (_clienteIdSeleccionado == 0)
                    {
                        // Nuevo Cliente
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
                        // Editar Cliente
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
                    CargarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_clienteIdSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un cliente para eliminar.");
                return;
            }

            if (MessageBox.Show("¿Seguro que deseas eliminar este cliente?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var context = new AlmacenDbContext())
                {
                    var cliente = context.Clientes.Find(_clienteIdSeleccionado);
                    if (cliente != null)
                    {
                        context.Clientes.Remove(cliente);
                        context.SaveChanges();
                        CargarDatos();
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
    }
}