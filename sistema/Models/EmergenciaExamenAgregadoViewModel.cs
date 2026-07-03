namespace sistema.Models
{
    public class EmergenciaExamenAgregadoViewModel
    {
        public int? ExamenId { get; set; }
        public string ExamenCodigo { get; set; }
        public string ExamenNombre { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorSubtotal { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal DescuentoValor { get; set; }
        public bool Eliminado { get; set; }

        public int Id { get; set; }

    }
}
