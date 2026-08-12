namespace AlmacenDesktop.Modelos
{
    // Unidad = venta por pieza entera (cantidad siempre entera).
    // El resto son fraccionables: admiten cantidades con decimales (ej. 0.750 kg de leña).
    public enum UnidadMedida
    {
        Unidad = 0,
        Kilogramo = 1,
        Gramo = 2,
        Litro = 3,
        Metro = 4,
        Docena = 5
    }
}
