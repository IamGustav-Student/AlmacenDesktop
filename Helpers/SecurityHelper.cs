using System;
using System.Security.Cryptography;
using System.Text;

namespace AlmacenDesktop.Helpers
{
    public static class SecurityHelper
    {
        // Método para convertir "1234" en una cadena hash ilegible e irreversible
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return "";

            using (SHA256 sha256 = SHA256.Create())
            {
                // Convertimos el string a bytes
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convertimos los bytes a string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Método para verificar si el password ingresado coincide con el hash guardado
        public static bool VerificarPassword(string passwordIngresado, string hashGuardado)
        {
            string hashIngresado = HashPassword(passwordIngresado);
            // Comparamos los hashes, no los textos planos
            return hashIngresado == hashGuardado;
        }
    }
}