namespace sistema.Models
{
    public class HomeHabitacionesOcupadasViewModel
    {
        public int HospitalizacionId { get; set; }
        public int HabitacionId { get; set; }
        public string HabitacionNumeroNombre { get; set; }
        public string HabitacionCategoria { get; set; }
        public string Paciente { get; set; }
        public int PacienteEdad { get; set; }
        public string CodigoCita { get; set; }
        public string MedicoAsignado { get; set; }


    }
}
