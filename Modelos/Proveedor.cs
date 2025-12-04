namespace AlmacenDesktop.Modelos
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } // Ej: "Coca Cola Dist."
        public string Contacto { get; set; } // Ej: "Juan Perez"
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Cuit { get; set; }
    }
}