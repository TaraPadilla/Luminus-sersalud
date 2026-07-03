namespace sistema.Models
{
    public class HospitalizacionProductoViewModel
    {
        public int Id { get; set; }
        public string ProductoId { get; set; }
        public string Nombre { get; set; }
        public int? TipoProductoId {get; set;}
        public decimal Cantidad { get; set; }
        public decimal CantidadAplicada { get; set; }
        public decimal Precio { get; set; }
        public string Indicaciones { get; set; }
        public string ViaAdministracion { get; set; }
        public string FrecuenciaAdministracion { get; set; }
        public decimal Subtotal { get; set; }
    }
}
