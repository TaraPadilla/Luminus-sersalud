namespace sistema.Models
{
    public class ProductoCompraViewModel
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; }

        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public string ProveedorPrincipal { get; set; }

        public decimal PrecioCompra { get; set; }
    }
}