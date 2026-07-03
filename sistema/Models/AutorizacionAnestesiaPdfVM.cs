namespace farmamest.Models
{
    public class AutorizacionAnestesiaPdfVM
    {
        public string NombreMedicoTratante { get; set; }
        public string ColegiadoMedico { get; set; }
        public string FirmaMedicoBase64 { get; set; }
        public string NombreAnestesista { get; set; }
        public string ColegiadoAnestesista { get; set; }
        public string FirmaAnestesistaBase64 { get; set; }
        public string NombrePaciente { get; set; }
        public string FechaAdmision { get; set; }
        public string Procedimiento { get; set; }
    }
}
