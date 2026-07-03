namespace sistema.Models
{
    public class ConsultasServicioAgregadoViewModel
    {
        public int? Id { get; set; }
        public int? ConsultaId { get; set; }
        public int? ServicioId { get; set; }
        public string ServicioCodigo { get; set; }
        public string NombreServicio { get; set; }
        public int? NumeroDiente { get; set; }
        public int ServicioCantidad { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal? PrecioValor { get; set; }
        public decimal? ServicioValorTotal { get; set; }
        public decimal ServicioValorCubiertoSeguro { get; set; }
        public decimal ServicioValorCopago { get; set; }
        public bool Aplicar { get; set; }
        public bool Pagado { get; set; }
    }
}
