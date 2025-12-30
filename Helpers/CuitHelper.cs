using System;
using System.Linq;

namespace AlmacenDesktop.Helpers
{
    public static class CuitHelper
    {
        public static bool Validar(string cuit)
        {
            if (string.IsNullOrEmpty(cuit)) return false;

            // Limpiar guiones
            cuit = cuit.Replace("-", "").Replace(" ", "");

            if (cuit.Length != 11) return false;
            if (!long.TryParse(cuit, out _)) return false;

            int[] multiplicadores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int total = 0;

            for (int i = 0; i < 10; i++)
            {
                total += int.Parse(cuit[i].ToString()) * multiplicadores[i];
            }

            int resto = total % 11;
            int verificador = resto == 0 ? 0 : (resto == 1 ? 9 : 11 - resto);

            return verificador == int.Parse(cuit[10].ToString());
        }
    }
}