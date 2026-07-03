namespace sistema.Models
{
    public class VentaUnificadaExamenExistenteViewModel
    {
        public int ExamenId { get; set; }
        public string ExamenNombre { get; set; }
        public string ExamenCodigo { get; set; }
        public string ExamenNombreMostrar
        {
            get
            {
                return $"{ExamenCodigo} - {ExamenNombre}";
            }
        }
    }
}
