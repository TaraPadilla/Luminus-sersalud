namespace sistema.Models
{
    public class HospitalizacionHabitacionViewModel
    {
        public int HabitacionId { get; set; }
        public int? HospitalizacionId { get; set; }
        public string HabitacionNombre { get; set; }
        public string HabitacionCategoria { get; set; }
        public int HabitacionEstadoId { get; set; }
        public string HabitacionEstado { get; set; }
        public string HabitacionOcupante { get; set; }
        public int HabitacionNumeroCamas { get; set; }
        public int HabitacionCapacidadPersonas { get; set; }

        public int? CitaId { get; set; }

    }
}
