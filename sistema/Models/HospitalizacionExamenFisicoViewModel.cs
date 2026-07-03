namespace sistema.Models
{
    public class HospitalizacionExamenFisicoViewModel
    {
        public int Id { get; set; }
        public string FechaHora { get; set; }
        public int HospitalizacionId { get; set; }
        public string Persona { get; set; }
        public string Datos { get; set; }
        public string Observaciones { get; set; }

        public bool Autorizado { get; set; }

    }
}
