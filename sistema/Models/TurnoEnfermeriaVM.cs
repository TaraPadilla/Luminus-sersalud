using Database.Shared.Models;

namespace farmamest.Models
{
    public class TurnoEnfermeriaVM
    {
        public int Id { get; set; }
        public string FechaRegistro { get; set; } // Fecha formateada
        public int NumeroTurno { get; set; } // Número del turno
        public string NombreTurno { get; set; } // Nombre del turno (por ejemplo, "Mañana", "Tarde")
        public int HospitalizacionId { get; set; } // ID de la hospitalización asociada
        public string UserId { get; set; } // ID del profesional
        public string Profesional { get; set; } // Nombre del profesional (persona que creó el turno)
        public bool Firmado { get; set; } // Indica si el turno ha sido firmado o no


        public string FirmaRuta { get; set; } // Nombre del profesional (persona que creó el turno)

    }
}
