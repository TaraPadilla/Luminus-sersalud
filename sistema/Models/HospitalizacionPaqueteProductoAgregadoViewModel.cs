namespace sistema.Models
{
    public class HospitalizacionPaqueteProductoAgregadoViewModel
    {
        public int? DetallePaqueteId { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoCodigo { get; set; }
        public int Cantidad { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public int? ProductoPrecioId { get; set; }
        public string Precio { get; set; }
        public decimal PrecioValor { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal ValorTotal { get; set; }
        public bool Nuevo { get; set; }
        public bool Eliminado { get; set; }
    }
}
