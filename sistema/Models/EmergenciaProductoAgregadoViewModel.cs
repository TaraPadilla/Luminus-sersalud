namespace sistema.Models
{
    public class EmergenciaProductoAgregadoViewModel
    {
        public int? ProductoId { get; set; }
        public string ProductoCodigo { get; set; }
        public string ProductoNombre { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal PrecioValor { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public float DescuentoPorcentaje { get; set; }
        public decimal DescuentoValor { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorSubtotal { get; set; }
        public decimal ValorTotal { get; set; }
        public bool Eliminado { get; set; }

        public int TipoProductoId { get; set; }

        public int Id { get; set; }

    }
}
