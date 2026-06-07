using System;
using System.Security.Cryptography;
using System.Text;

namespace AlmacenDesktop.Helpers
{
    public static class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return "";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        public static bool VerificarPassword(string passwordIngresado, string hashGuardado)
        {
            return HashPassword(passwordIngresado) == hashGuardado;
        }

        // --- ENCRIPTACIÓN DPAPI ---

        public static string EncriptarSecreto(string textoPlano)
        {
            if (string.IsNullOrWhiteSpace(textoPlano)) return null;
            try
            {
                byte[] textoBytes = Encoding.UTF8.GetBytes(textoPlano);
                // Scope CurrentUser: Solo tu usuario de Windows puede desencriptarlo
                byte[] bytesCifrados = ProtectedData.Protect(textoBytes, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(bytesCifrados);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string DesencriptarSecreto(string textoCifrado)
        {
            if (string.IsNullOrWhiteSpace(textoCifrado)) return null;
            try
            {
                // Intentamos convertir Base64. Si falla (porque es texto plano viejo), va al catch.
                byte[] bytesCifrados = Convert.FromBase64String(textoCifrado);

                // Intentamos desencriptar. Si falla (porque son datos corruptos), va al catch.
                byte[] bytesPlanos = ProtectedData.Unprotect(bytesCifrados, null, DataProtectionScope.CurrentUser);

                return Encoding.UTF8.GetString(bytesPlanos);
            }
            catch
            {
                // Si llegamos acá, es porque el dato en BD no estaba encriptado o es de otra PC.
                // Retornamos NULL para indicar que el dato es inválido/viejo.
                return null;
            }
        }
    }
}