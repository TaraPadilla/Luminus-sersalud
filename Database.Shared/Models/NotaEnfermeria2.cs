using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class NotaEnfermeria2
    {
        public int Id { get; set; }
        public string Evolucion { get; set; }
        public string Sintomas { get; set; }
        public string Diagnostico { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        /// <summary>
        /// El user es el mismo profesional, persona que crea la nota de enfermeria
        /// </summary>
        public string UserId { get; set; }
        public User User { get; set; }

        // Relación con TurnoEnfermeria
        public int TurnoEnfermeriaId { get; set; } // ID del turno asociado
        public TurnoEnfermeria TurnoEnfermeria { get; set; } // Propiedad de navegación al turno de enfermería

        public string? FirmaRuta { get; set; }
        public bool Firmado { get; set; }
        public DateTime? FechaFirma { get; set; }
        public string? UsuarioFirmaId { get; set; }

        /// <summary>Ingreso, Traslado, Recepcion, Egreso — para expediente completo.</summary>
        public string TipoNota { get; set; }
    }
}
