namespace sistema.Models
{
    public class HospitalizacionAgregarMedicamentoViewModel
    {
        public int HospitalizacionId { get; set; }
        public int UnidadMedidaVentaId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Indicaciones { get; set; }
        public string ViaAdministracion { get; set; }
        public string FrecuenciaAdministracion { get; set; }
        public string FechaHoraAplicacionManual { get; set; }

        public int IdProductoPrecioInventario { get; set; }
        public int PrecioId { get; set; }
    }
}
