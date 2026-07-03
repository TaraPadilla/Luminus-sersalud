namespace farmamest.Models
{
    public class HospitalizacionPaqueteLaboratorioAgregadoViewModel
    {
        public int? DetallePaqueteId { get; set; }
        public int Id { get; set; }
        public string NombreExamen { get; set; }
        public int Cantidad { get; set; }
        public int? PrecioId { get; set; }
        public string Precio { get; set; }
        public decimal PrecioValor { get; set; }
        /// <summary>
        /// Este es el Precio de Costo
        /// </summary>
        public decimal PrecioCompra { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public bool Nuevo { get; set; }
        public bool Eliminado { get; set; }
    }
}
