namespace sistema.Models
{
    public class HospitalizacionRegistroHospitalizacionViewModel
    {
        public int Id { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public int NumeroNoches { get; set; }
        public int HabitacionId { get; set; }
        public string HabitacionNumeroNombre { get; set; }
        public string HabitacionCategoria { get; set; }
        public string HabitacionTarifa { get; set; }
        public string HabitacionValorNoche { get; set; }
        public string HabitacionNumeroCamas { get; set; }
        public string Observaciones { get; set; }
        public string Precio { get; set; }
    }
}
