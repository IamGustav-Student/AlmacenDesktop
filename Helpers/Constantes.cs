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
        // OJO: esto es la API interna, NO una página para el cliente. Si se abre
        // en el navegador, el cliente cae en el login del panel de administración.
        public const string API_LICENCIAS_URL = "https://www.programadorgs.com.ar/ops";

        // Página pública de compra/renovación. Es la que hay que abrir cuando el
        // cliente quiere pagar — antes se usaba API_LICENCIAS_URL por error y
        // terminaba en el panel de admin, justo en el momento de cobrar.
        public const string URL_CHECKOUT = "https://www.programadorgs.com.ar/vendemax-desktop";

        // Actualizador automático — consulta el último release público de GitHub.
        public const string GITHUB_RELEASES_API = "https://api.github.com/repos/IamGustav-Student/AlmacenDesktop/releases/latest";

        // Catálogo compartido de productos entre instalaciones (solo nombre + código
        // de barras — nunca costo/precio/stock/proveedor). Debe coincidir con
        // CATALOG_UPLOAD_SECRET en ops-dashboard.
        public const string CATALOG_UPLOAD_SECRET = "69b3033d81be49e820703626f557d7c77a0af41e2e909417";
    }
}