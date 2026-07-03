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
    }
}
