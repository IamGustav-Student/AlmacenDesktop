namespace AlmacenDesktop.Modelos
{
    public class DetalleCompra
    {
        public int Id { get; set; }

        public int CompraId { get; set; }
        public Compra Compra { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; } // Costo al momento de esta compra
        public decimal Subtotal => Cantidad * CostoUnitario;
    }
}