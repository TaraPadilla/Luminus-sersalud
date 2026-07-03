using System;
using System.Collections.Generic;

namespace Database.Shared.Models
{
    public class TurnoEnfermeria
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; } // Fecha y hora en que se registró el turno
        public int NumeroTurno { get; set; } // Número del turno
        public string NombreTurno { get; set; } // Nombre descriptivo del turno (e.g., "Mañana", "Tarde")
        public int HospitalizacionId { get; set; } // Relación con la hospitalización
        public Hospitalizacion Hospitalizacion { get; set; } // Objeto de navegación para Hospitalización
        public bool Firmado { get; set; } // Indica si el turno ha sido firmado
        public string UserId { get; set; } // ID del usuario (profesional) que creó el turno
        public User User { get; set; } // Objeto de navegación para el usuario
        public ICollection<NotaEnfermeria2> NotasEnfermeria { get; set; } // Relación con las notas de enfermería

        // Constructor para inicializar colecciones
        public TurnoEnfermeria()
        {
            NotasEnfermeria = new List<NotaEnfermeria2>();
        }


        public string? FirmaRuta { get; set; }
        public DateTime? FechaFirma { get; set; }
        public string? UsuarioFirmaId { get; set; }
    }
}
