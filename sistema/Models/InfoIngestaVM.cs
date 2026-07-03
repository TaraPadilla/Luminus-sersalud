using Database.Shared.Models;

namespace farmamest.Models
{
    public class InfoIngestaVM
    {
        public int Id { get; set; } // ID del registro de InfoIngesta
        public string FechaRegistro { get; set; } // Fecha formateada en la que se registró la ingesta
        public string InfoIngestaIV1 { get; set; } // Información asociada a la ingesta IV1
        public string InfoIngestaIV2 { get; set; } // Información asociada a la ingesta IV2
        public string InfoIngestaIV3 { get; set; } // Información asociada a la ingesta IV3
        public string InfoIngestaIV4 { get; set; } // Información asociada a la ingesta IV4
        public string InfoIngestaIV5 { get; set; } // Información asociada a la ingesta IV5
        public string InfoIngestaIV6 { get; set; } // Información asociada a la ingesta IV6
        public string InfoIngestaPO { get; set; } // Información asociada a la ingesta PO (oral)
        public string UserId { get; set; } // ID del profesional (quien registró la ingesta)
        public string Profesional { get; set; } // Nombre del profesional (persona que registró la ingesta)
    }
}
