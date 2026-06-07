using System.ComponentModel.DataAnnotations;

namespace AlmacenDesktop.Modelos
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string NombreUsuario { get; set; }

        [Required]
        public string Password { get; set; } // Recuerda: Esto guarda el Hash, no el texto plano

        public string Email { get; set; }
        public string Telefono { get; set; }

        // --- NUEVA PROPIEDAD DE SEGURIDAD ---
        public RolUsuario Rol { get; set; } = RolUsuario.Vendedor; // Por defecto, nadie es Admin

        // Propiedad auxiliar para mostrar en grillas
        public string RolStr => Rol.ToString();
    }
}