using System;
using System.Globalization;

namespace Database.Shared.Models
{
    public class HospitalizacionExamenPdf
    {
        public int Id { get; set; }  // Identificador único del registro
        public string PdfUrl { get; set; } // URL donde se almacena el archivo PDF
        public bool Eliminado { get; set; } = false; // Indica si el registro ha sido eliminado o no
        public DateTime FechaCreacion { get; set; } = DateTime.Now; // Fecha en que se crea el registro
        public string FechaCreacionFormatted => FechaCreacion.ToString("dddd, dd MMMM yyyy - hh:mm tt", new CultureInfo("es-ES"));
        public string UserId { get; set; } // ID del usuario que creó el registro (sin FK)
        public bool IsPrincipalPdf { get; set; } = false; // Indica si el archivo PDF es el principal o no
        public string NombreExamen { get; set; } // Nombre del examen

        // Relaciones
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        
        public int ExamenId { get; set; }
        public Examen Examen { get; set; }
    }
}
