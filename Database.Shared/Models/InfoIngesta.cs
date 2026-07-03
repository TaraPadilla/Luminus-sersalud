using System;

namespace Database.Shared.Models
{
    public class InfoIngesta
    {
        public int Id { get; set; } // Identificador único para cada entrada de información de ingesta

        // Relación con IngestaExcreta2
        public int IngestaExcreta2Id { get; set; } // Id de IngestaExcreta2 al cual se refiere esta información
        public IngestaExcreta2 IngestaExcreta2 { get; set; } // Objeto de navegación hacia IngestaExcreta2

        // Propiedades opcionales de tipo texto para almacenar detalles de la ingesta
        public string InfoIngestaIV1 { get; set; }
        public string InfoIngestaIV2 { get; set; }
        public string InfoIngestaIV3 { get; set; }
        public string InfoIngestaIV4 { get; set; }
        public string InfoIngestaIV5 { get; set; }
        public string InfoIngestaIV6 { get; set; }
        public string InfoIngestaPO { get; set; }

        // Fecha de registro de la información de ingesta
        public DateTime FechaRegistro { get; set; } // Fecha en la que se registró la información

        // UserId y relación con User (opcional, si se requiere la trazabilidad de quién agregó la información)
        public string UserId { get; set; } // ID del usuario que creó la entrada
        public User User { get; set; } // Objeto de navegación para el usuario
    }
}