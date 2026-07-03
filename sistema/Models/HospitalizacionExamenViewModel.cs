namespace sistema.Models
{
    public class HospitalizacionExamenViewModel
    {
        public int Id { get; set; }
        public int ExamenId { get; set; }
        public int DetalleExamenId { get; set; }
        public string FechaHora { get; set; } // Mantienes la fecha de la hora
        public string Nombre { get; set; } // Nombre del examen
        public decimal Precio { get; set; } // Precio del examen
        public string Profesional { get; set; } // Nombre del profesional encargado del examen
        public string FechaRegistro { get; set; } // Nueva propiedad para la fecha de registro
    }
}
