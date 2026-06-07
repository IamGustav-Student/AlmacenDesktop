using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Helpers;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class UsuariosForm : Form
    {
        private int _idSeleccionado = 0;

        public UsuariosForm()
        {
            InitializeComponent();
            
            // Garantía de teclas rápidas
            this.KeyPreview = true;
            this.KeyDown += UsuariosForm_KeyDown;
        }

        private void UsuariosForm_Load(object sender, EventArgs e)
        {
            CargarRoles();
            CargarDatos();
            Limpiar();
        }

        private void CargarRoles()
        {
            cboRol.Items.Clear();
            cboRol.Items.Add(RolUsuario.Vendedor);
            cboRol.Items.Add(RolUsuario.Admin);
            cboRol.Items.Add(RolUsuario.Gerente);
            cboRol.SelectedIndex = 0;
        }

        private void CargarDatos()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    dgvDatos.DataSource = null;
                    // Mapeamos a un tipo anónimo para no mostrar la contraseña (hash) en pantalla por seguridad
                    var lista = context.Usuarios.Select(u => new
                    {
                        u.Id,
                        u.NombreUsuario,
                        u.Nombre,
                        u.Apellido,
                        Rol = u.Rol.ToString(),
                        u.Email,
                        u.Telefono
                    }).ToList();

                    dgvDatos.DataSource = lista;
                    
                    if (dgvDatos.Columns["Id"] != null) dgvDatos.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarUsuario();
        }

        private void GuardarUsuario()
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                MessageBox.Show("El nombre de usuario es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El nombre y apellido son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new AlmacenDbContext())
            {
                // VALIDACIÓN: Nombre de usuario único
                bool existe = context.Usuarios.Any(u => u.NombreUsuario.ToLower() == txtNombreUsuario.Text.Trim().ToLower() && u.Id != _idSeleccionado);
                if (existe)
                {
                    MessageBox.Show("Ya existe un usuario registrado con ese nombre de usuario.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtNombreUsuario.Focus();
                    return;
                }

                if (_idSeleccionado == 0)
                {
                    // Registro nuevo - Contraseña obligatoria
                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show("La contraseña es obligatoria para nuevos usuarios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPassword.Focus();
                        return;
                    }

                    var nuevoUsuario = new Usuario
                    {
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        NombreUsuario = txtNombreUsuario.Text.Trim(),
                        Password = SecurityHelper.HashPassword(txtPassword.Text),
                        Rol = (RolUsuario)cboRol.SelectedItem,
                        Email = txtEmail.Text.Trim(),
                        Telefono = txtTelefono.Text.Trim()
                    };

                    context.Usuarios.Add(nuevoUsuario);
                }
                else
                {
                    // Modificación
                    var usr = context.Usuarios.Find(_idSeleccionado);
                    if (usr == null)
                    {
                        MessageBox.Show("El usuario ya no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Impedir que un admin se quite el rol de admin a sí mismo si es el único
                    if (usr.Id == Program.UsuarioActualGlobal.Id && usr.Rol == RolUsuario.Admin && (RolUsuario)cboRol.SelectedItem != RolUsuario.Admin)
                    {
                        // Verificar si hay otros administradores
                        bool hayOtrosAdmins = context.Usuarios.Any(u => u.Rol == RolUsuario.Admin && u.Id != usr.Id);
                        if (!hayOtrosAdmins)
                        {
                            MessageBox.Show("No puedes quitarte el rol de Administrador ya que eres el único administrador en el sistema.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }

                    usr.Nombre = txtNombre.Text.Trim();
                    usr.Apellido = txtApellido.Text.Trim();
                    usr.NombreUsuario = txtNombreUsuario.Text.Trim();
                    usr.Rol = (RolUsuario)cboRol.SelectedItem;
                    usr.Email = txtEmail.Text.Trim();
                    usr.Telefono = txtTelefono.Text.Trim();

                    // Si escribió algo en el campo de contraseña, se actualiza, sino se mantiene la anterior
                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        usr.Password = SecurityHelper.HashPassword(txtPassword.Text);
                    }
                }

                context.SaveChanges();
                AudioHelper.PlayOk();
                MessageBox.Show("Usuario guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Limpiar();
                CargarDatos();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0) return;

            // PREVENCIÓN: No eliminarse a sí mismo
            if (_idSeleccionado == Program.UsuarioActualGlobal.Id)
            {
                AudioHelper.PlayError();
                MessageBox.Show("No puedes eliminar a tu propio usuario activo del sistema.", "Acción Denegada", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (MessageBox.Show("¿Seguro que desea eliminar este usuario?\nEsta acción no se puede deshacer.", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var usr = context.Usuarios.Find(_idSeleccionado);
                    if (usr != null)
                    {
                        // PREVENCIÓN: No eliminar al último administrador
                        if (usr.Rol == RolUsuario.Admin)
                        {
                            bool hayOtrosAdmins = context.Usuarios.Any(u => u.Rol == RolUsuario.Admin && u.Id != usr.Id);
                            if (!hayOtrosAdmins)
                            {
                                AudioHelper.PlayError();
                                MessageBox.Show("No se puede eliminar este usuario porque es el único Administrador activo del sistema.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                        }

                        context.Usuarios.Remove(usr);
                        context.SaveChanges();
                        
                        AudioHelper.PlayOk();
                        MessageBox.Show("Usuario eliminado del sistema.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Limpiar();
                        CargarDatos();
                    }
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            _idSeleccionado = 0;
            txtNombre.Clear();
            txtApellido.Clear();
            txtNombreUsuario.Clear();
            txtPassword.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            cboRol.SelectedIndex = 0;
            
            btnEliminar.Enabled = false;
            btnGuardar.Text = "Guardar Nuevo (F5)";
            lblClaveInfo.Text = "Clave (Obligatoria para nuevos):";

            txtNombreUsuario.BackColor = Color.White;
            txtNombre.BackColor = Color.White;
            txtApellido.BackColor = Color.White;
        }

        private void dgvDatos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDatos.CurrentRow != null && dgvDatos.CurrentRow.Index >= 0 && dgvDatos.Columns.Contains("Id"))
            {
                try
                {
                    var row = dgvDatos.CurrentRow;
                    _idSeleccionado = Convert.ToInt32(row.Cells["Id"].Value);
                    txtNombreUsuario.Text = row.Cells["NombreUsuario"].Value?.ToString() ?? "";
                    txtNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
                    txtApellido.Text = row.Cells["Apellido"].Value?.ToString() ?? "";
                    txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                    txtTelefono.Text = row.Cells["Telefono"].Value?.ToString() ?? "";

                    string rolStr = row.Cells["Rol"].Value?.ToString() ?? "Vendedor";
                    foreach (RolUsuario item in cboRol.Items)
                    {
                        if (item.ToString() == rolStr)
                        {
                            cboRol.SelectedItem = item;
                            break;
                        }
                    }

                    txtPassword.Clear(); // Clave vacía indica que no se modifica
                    lblClaveInfo.Text = "Nueva Clave (Dejar vacío para mantener):";
                    btnGuardar.Text = "Actualizar Registro (F5)";
                    btnEliminar.Enabled = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error de Selección: " + ex.Message);
                }
            }
        }

        private void UsuariosForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                GuardarUsuario();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (_idSeleccionado > 0)
                {
                    Limpiar();
                }
                else
                {
                    this.Close();
                }
                e.Handled = true;
            }
        }
    }
}
