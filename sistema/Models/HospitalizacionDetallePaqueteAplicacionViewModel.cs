namespace farmamest.Models
{
    public class HospitalizacionDetallePaqueteAplicacionViewModel
    {
        public int Id { get; set; }
        public int? ExamenId { get; set; }
        public int? DetalleExamenId { get; set; }
        public string Tipo { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Aplicado { get; set; }
        public string FechaHoraAplicacion { get; set; }
        public string Persona { get; set; }
    }
}
