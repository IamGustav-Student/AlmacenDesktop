using AlmacenDesktop.Data;
using AlmacenDesktop.Helpers;
using AlmacenDesktop.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ClientesForm : Form
    {
        private int _clienteIdSeleccionado = 0;
        private List<Cliente> _clientesCache = new List<Cliente>();

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
                _clientesCache = context.Clientes
                                   .Where(c => c.DniCuit != Constantes.CLIENTE_DEF_DNI)
                                   .OrderBy(c => c.Apellido).ThenBy(c => c.Nombre)
                                   .ToList();
            }
            AplicarFiltro(txtBuscar.Text);
        }

        private void AplicarFiltro(string termino)
        {
            termino = (termino ?? "").Trim();

            IEnumerable<Cliente> lista = _clientesCache;
            if (!string.IsNullOrEmpty(termino))
            {
                lista = _clientesCache.Where(c =>
                    (c.Nombre?.Contains(termino, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Apellido?.Contains(termino, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.DniCuit?.Contains(termino, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            dgvClientes.DataSource = null;
            dgvClientes.DataSource = lista.ToList();

            if (dgvClientes.Columns["Id"] != null) dgvClientes.Columns["Id"].Visible = false;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            // Búsqueda en tiempo real, letra por letra
            AplicarFiltro(txtBuscar.Text);
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
                MessageBox.Show($"Error al guardar: {ExceptionHelper.ObtenerMensaje(ex)}");
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
            // CargarDatos() reasigna el DataSource del grid, y WinForms autoselecciona
            // la primera fila al rebindear — eso disparaba SelectionChanged de nuevo y
            // repoblaba los campos con ese cliente, así que "limpiar" nunca dejaba la
            // pantalla realmente en blanco (ej. al apretar Escape para cargar uno nuevo).
            // Desconectamos el evento mientras se recarga/limpia la selección, y los
            // campos se limpian al final para que ese sea siempre el estado visible.
            dgvClientes.SelectionChanged -= dgvClientes_SelectionChanged;
            CargarDatos();
            dgvClientes.ClearSelection();
            dgvClientes.SelectionChanged += dgvClientes_SelectionChanged;

            txtNombre.Clear();
            txtApellido.Clear();
            txtDni.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            _clienteIdSeleccionado = 0;
            btnGuardar.Text = "GUARDAR CLIENTE (F5)";
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

                btnGuardar.Text = "ACTUALIZAR CLIENTE (F5)";
            }
        }

        private void btnVerCtaCte_Click(object sender, EventArgs e)
        {
            var formCtaCte = new CuentaCorrienteForm();
            this.Hide();
            formCtaCte.ShowDialog();
            this.Show();
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