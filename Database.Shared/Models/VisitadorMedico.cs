using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class VisitadorMedico
    {
        public VisitadorMedico()
        {
            // Inicialización de listas si se necesitara alguna relación en el futuro.
        }

        public int Id { get; set; } // Identificador único del visitador médico.

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreVisitador { get; set; } // Nombre completo del visitador médico.

        public string ContactoVisitador { get; set; } // Teléfono o correo electrónico.

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreFarmaceutica { get; set; } // Nombre de la farmacéutica representada.

        public string ContactoFarmaceutica { get; set; } // Teléfono o correo electrónico de la farmacéutica.

        public string Observaciones { get; set; } // Campo de observaciones adicionales.

        public string UrlCatalogo { get; set; } // Enlace al catálogo de productos.

        public DateTime FechaRegistro { get; set; } = DateTime.Now; // Fecha y hora de registro.
        
        // Propiedad calculada para la fecha en formato legible
        public string FechaRegistroFormateada => FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
