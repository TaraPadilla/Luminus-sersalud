namespace sistema.Models
{
    public class EmergenciaServicioAgregadoViewModel
    {
        public int? ServicioId { get; set; }
        public string ServicioCodigo { get; set; }
        public string ServicioNombre { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorSubtotal { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal DescuentoValor { get; set; }
        public bool Eliminado { get; set; }

        public int Id { get; set; }

    }
}
