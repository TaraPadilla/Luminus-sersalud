namespace sistema.Models
{
    public class VentaUnificadaServicioAgregadoViewModel
    {
        public int? Id { get; set; } //Este es el ID de registro en Base de datos tabla "DetalleVenta"
        public int ServicioId { get; set; }
        public string ServicioCodigo { get; set; }
        public string ServicioNombre { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorSubtotal { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorCubiertoSeguro { get; set; }
        public decimal ValorCopago { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal DescuentoValor { get; set; }
        public string UsuarioAutoriza { get; set; }

        public decimal Recargo { get; set; }

        public decimal DescuentoServicioPorcentaje { get; set; }

    }
}
