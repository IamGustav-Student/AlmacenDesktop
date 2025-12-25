using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AlmacenDesktop.Helpers
{
    public static class ValidationHelper
    {
        // Valida si un string tiene texto útil
        public static bool EsTextoValido(string texto)
        {
            return !string.IsNullOrWhiteSpace(texto);
        }

        // Validación básica de formato Email
        public static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        // Valida que solo haya números (útil para eventos KeyPress)
        public static bool EsNumero(char keyChar)
        {
            return char.IsControl(keyChar) || char.IsDigit(keyChar);
        }

        // Valida números decimales (permite una coma o punto)
        public static bool EsDecimal(char keyChar, string textoActual)
        {
            if (char.IsControl(keyChar) || char.IsDigit(keyChar)) return true;

            // Permitir solo un punto o coma decimal
            if ((keyChar == '.' || keyChar == ',') &&
                (textoActual.IndexOf('.') == -1 && textoActual.IndexOf(',') == -1))
            {
                return true;
            }
            return false;
        }

        // Método UX Nivel Dios: Configura un ErrorProvider genérico
        public static bool ValidarCampo(Control control, ErrorProvider errorProvider, string mensajeError, Func<bool> reglaValidacion)
        {
            if (!reglaValidacion())
            {
                errorProvider.SetError(control, mensajeError);
                control.BackColor = System.Drawing.Color.MistyRose; // Feedback visual sutil
                return false;
            }
            else
            {
                errorProvider.SetError(control, ""); // Limpiar error
                control.BackColor = System.Drawing.Color.White;
                return true;
            }
        }
    }
}