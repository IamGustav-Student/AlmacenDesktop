namespace AlmacenDesktop.Helpers
{
    public static class Constantes
    {
        // Datos del Consumidor Final
        public const string CLIENTE_DEF_NOMBRE = "Consumidor";
        public const string CLIENTE_DEF_APELLIDO = "Final";
        public const string CLIENTE_DEF_DNI = "00000000";

        // Configuración de UI
        public const string MONEDA_FMT = "C2"; // Formato moneda ($ 1,200.00)
        public const string FECHA_HORA_FMT = "dd/MM/yyyy HH:mm";

        // Reglas de Negocio
        public const int ALERTA_STOCK_MINIMO = 5; // Umbral para el dashboard

        // Servidor de Licencias — ahora integrado al ops-dashboard del ecosistema
        // ProgramadorGS (reemplaza el servicio Node standalone de server/).
        public const string API_LICENCIAS_URL = "https://www.programadorgs.com.ar/ops";

        // Actualizador automático — consulta el último release público de GitHub.
        public const string GITHUB_RELEASES_API = "https://api.github.com/repos/IamGustav-Student/AlmacenDesktop/releases/latest";
    }
}