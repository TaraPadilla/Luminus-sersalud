namespace sistema.Models
{
    public class VentaUnificadaProductoAgregadoViewModel
    {
        public int ProductoId { get; set; }
        public int ProductoInventarioId { get; set; }
        public string ProductoCodigo { get; set; }
        public string ProductoNombre { get; set; }
        /// <summary>
        /// Nombre del precio manejado
        /// </summary>
        public string Precio { get; set; }
        public int PrecioId { get; set; }
        public int UnidadMedidaVentaId { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal PrecioValor { get; set; }
        public decimal DescuentoValor { get; set; }
        public string UsuarioAutoriza { get; set; }
        public bool VentaPerdida { get; set; }

        public decimal Recargo { get; set; }
        public decimal DescuentoProductoPorcentaje { get; set; }

        public int TipoProductoId { get; set; }


    }
}
