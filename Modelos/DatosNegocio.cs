namespace AlmacenDesktop.Modelos
{
    public class DatosNegocio
    {
        public int Id { get; set; }
        public string NombreFantasia { get; set; } // El nombre del cartel (ej. "Kiosco Pepe")
        public string RazonSocial { get; set; } // Nombre legal (ej. "José Pérez")
        public string CUIT { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string MensajeTicket { get; set; } // Pie de página (ej. "No se aceptan devoluciones")
    }
}