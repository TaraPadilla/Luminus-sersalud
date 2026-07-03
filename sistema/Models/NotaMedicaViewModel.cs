using Database.Shared.Models;

namespace farmamest.Models
{
    public class NotaMedica2ViewModel
    {
        public int Id { get; set; }
        public string HistoriaProblema { get; set; }
        public string Sintomas { get; set; }
        public string Diagnostico { get; set; }
        public string FechaRegistro { get; set; }
        public string Profesional { get; set; }
        /// <summary>Usuario que capturó la nota en sistema (p. ej. recepción). No sustituye al médico tratante.</summary>
        public string RegistradoPor { get; set; }
        public int HospitalizacionId { get; set; }

        public string PacienteNombre { get; set; }
        public string PacienteSexoText { get; set; }
        public string PacienteEdad { get; set; }
        public string EmpleadoText { get; set; }
        public string ColegioEmpleado { get; set; }

        public string Evolucion { get; set; }

        public bool Autorizado { get; set; }
        public string UsuarioAutoriza { get; set; }
    public string FechaAutorizacion { get; set; }

    public string FirmaBase64 { get; set; }
    public string TipoNota { get; set; }
    public string NombreFirmante { get; set; }
    public string AutorizadoPor { get; set; }
}
}
