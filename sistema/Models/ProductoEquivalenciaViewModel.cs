namespace sistema.Models
{
    public class ProductoEquivalenciaViewModel
    {
        public int? Id { get; set; }
        public int? ProductoId { get; set; }
        public int UnidadMedidaCompraId { get; set; }
        public string UnidadMedidaCompraNombre { get; set; }
        public decimal PrecioUnidadCompra { get; set; }
        public int UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public decimal CantidadEquivalente { get; set; }
        public decimal PrecioUnidadVenta { get; set; }
        public decimal PrecioUnidadVenta_1 { get; set; }
    }
}
