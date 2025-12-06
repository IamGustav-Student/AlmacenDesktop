using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Helpers; // Importante
using System;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class LoginForm : Form
    {
        public Usuario UsuarioLogueado { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsuario.Text;
            string pass = txtPass.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                AudioHelper.PlayError(); // Feedback auditivo
                MessageBox.Show("Por favor, complete todos los campos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    // 1. Buscamos solo por nombre de usuario
                    var usuarioEncontrado = context.Usuarios
                                            .FirstOrDefault(u => u.NombreUsuario == user);

                    if (usuarioEncontrado != null)
                    {
                        // 2. Verificamos el password usando el Helper de Seguridad
                        // NOTA: Si el sistema es viejo y tiene claves sin encriptar, esto fallará la primera vez.
                        // Para este upgrade, asumimos que reseteamos o migramos claves.

                        // Truco para compatibilidad: Si la clave en BD es corta (ej "123"), asumimos que es vieja y la comparamos directo.
                        // Si es larga (Hash SHA256 son 64 chars), usamos el verificador.
                        bool esValido = false;

                        if (usuarioEncontrado.Password.Length < 50)
                        {
                            // Legacy: Comparación directa (y la actualizamos automáticamente para la próxima)
                            if (usuarioEncontrado.Password == pass)
                            {
                                esValido = true;
                                usuarioEncontrado.Password = SecurityHelper.HashPassword(pass);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            // Moderno: Verificación Hash
                            esValido = SecurityHelper.VerificarPassword(pass, usuarioEncontrado.Password);
                        }

                        if (esValido)
                        {
                            AudioHelper.PlayOk();
                            UsuarioLogueado = usuarioEncontrado;
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            AudioHelper.PlayError();
                            MessageBox.Show("Contraseña incorrecta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        AudioHelper.PlayError();
                        MessageBox.Show("Usuario no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
