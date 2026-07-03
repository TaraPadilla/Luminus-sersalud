namespace sistema.Models
{
    public class VentaUnificadaServicioExistenteViewModel
    {
        public int ServicioId { get; set; }
        public string ServicioNombre { get; set; }
        public string ServicioCodigo { get; set; }
        public string ServicioNombreMostrar
        {
            get
            {
                return $"{ServicioCodigo} - {ServicioNombre}";
            }
        }
    }
}
