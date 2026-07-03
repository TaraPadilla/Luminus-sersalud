namespace sistema.Models
{
    public class HabitacionViewModel
    {
        public int? HabitacionId { get; set; }
        public string NumeroNombre { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; }
        public int CapacidadPersonas { get; set; }
        public int NumeroCamas { get; set; }
    }
    public class HabitacionCategoriaExistenteViewModel
    {
        public int Id { get; set; }
        public string NombreCategoria { get; set; }
    }
    public class HabitacionEstadoExistenteViewModel
    {
        public int Id { get; set; }
        public string NombreEstado { get; set; }
    }
}
