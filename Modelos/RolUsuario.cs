namespace AlmacenDesktop.Modelos
{
    public enum RolUsuario
    {
        Admin = 0,      // Acceso Total
        Vendedor = 1,   // Solo Ventas, Clientes y Stock (Lectura)
        Gerente = 2     // Reportes y Stock (Escritura), pero no Configuración Técnica
    }
}