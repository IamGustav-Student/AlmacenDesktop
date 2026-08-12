using AlmacenDesktop.Modelos;

namespace AlmacenDesktop.Helpers
{
    public static class UnidadMedidaHelper
    {
        public static bool EsFraccionable(UnidadMedida unidad) => unidad != UnidadMedida.Unidad;

        public static string Abreviatura(UnidadMedida unidad)
        {
            switch (unidad)
            {
                case UnidadMedida.Kilogramo: return "kg";
                case UnidadMedida.Gramo: return "g";
                case UnidadMedida.Litro: return "L";
                case UnidadMedida.Metro: return "m";
                case UnidadMedida.Docena: return "doc";
                default: return "un";
            }
        }

        // Enteras se muestran sin decimales ("3"); fraccionables con hasta 3 sin ceros de más ("0.75").
        public static string FormatearCantidad(decimal cantidad, UnidadMedida unidad)
        {
            return EsFraccionable(unidad) ? cantidad.ToString("0.###") : ((int)cantidad).ToString();
        }

        public static string FormatearCantidadConUnidad(decimal cantidad, UnidadMedida unidad)
        {
            return $"{FormatearCantidad(cantidad, unidad)} {Abreviatura(unidad)}";
        }
    }
}
