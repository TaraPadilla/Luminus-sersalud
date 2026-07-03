namespace sistema.Models
{
    public class HospitalizacionProductoAplicacionViewModel
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public string Indicaciones { get; set; }
        public string ViaAdministracion { get; set; }
        public string FrecuenciaAdministracion { get; set; }
        public bool Aplicado { get; set; }
        public string FechaHoraAplicacion { get; set; }
        public string FechaHoraAplicacionManual { get; set; }

        public string PersonaAplica { get; set; }
        public string PersonaCrea { get; set; }
    }
}
