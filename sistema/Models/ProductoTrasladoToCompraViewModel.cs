namespace sistema.Models
{

    public class ProductoTrasladoToCompraViewModel
    {
        public int ProductoInventarioId { get; set; }
        public int ProductoId { get; set; }
        public string CodigoReferencia { get; set; }
        public string ProductoNombre { get; set; }
        public int CantidadTrasladada { get; set; }

        // Propiedad agregada para reutilizar el modelo en la búsqueda de precios
        public string ProveedorNombre { get; set; }
    }
}