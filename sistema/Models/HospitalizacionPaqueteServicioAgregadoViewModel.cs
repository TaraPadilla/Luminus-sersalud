namespace sistema.Models
{
    public class HospitalizacionPaqueteServicioAgregadoViewModel
    {
        public int? DetallePaqueteId { get; set; }
        public int ServicioId { get; set; }
        public string ServicioNombre { get; set; }
        public string ServicioCodigo { get; set; }
        public string Precio { get; set; }
        public int Cantidad { get; set; }
        public int? PrecioId { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal PrecioValor { get; set; }
        public decimal PrecioCompra { get; set; }
        public bool Nuevo { get; set; }
        public bool Eliminado { get; set; }
    }
}
