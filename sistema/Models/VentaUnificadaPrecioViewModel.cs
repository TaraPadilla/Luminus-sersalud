namespace sistema.Models
{
    public class VentaUnificadaPrecioViewModel
    {
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal PrecioValor { get; set; }
        public string PrecioNombreMostrar
        {
            get
            {
                return $"{PrecioNombre} - {PrecioValor}";
            }
        }
    }
}
