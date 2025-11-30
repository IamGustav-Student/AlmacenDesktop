using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
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

        // [POO - Encapsulamiento]
        // Toda la lógica de validación está encapsulada en este evento.
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsuario.Text;
            string pass = txtPass.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    // Buscamos al usuario en la BD (LINQ)
                    // NOTA: Para este prototipo comparamos texto plano. En producción usaríamos Hash.
                    var usuarioEncontrado = context.Usuarios
                                            .FirstOrDefault(u => u.NombreUsuario == user && u.Password == pass);

                    if (usuarioEncontrado != null)
                    {
                        // ¡Login Exitoso!
                        UsuarioLogueado = usuarioEncontrado;
                        this.DialogResult = DialogResult.OK; // Esto le avisa a Program.cs que todo salió bien
                        this.Close(); // Cerramos la ventana de Login
                        
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
