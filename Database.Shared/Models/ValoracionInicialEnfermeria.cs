using System;

namespace Database.Shared.Models
{
    public class ConsultasValoracionInicialEnfermeria
    {
        public long Id { get; set; }
        public int ConsultaId { get; set; }
        public int PacienteId { get; set; }

        // 1) Datos de valoración inicial
        public string Motivo { get; set; }                  // TEXT
        public string DiagnosticoMedico { get; set; }       // TEXT
        public string Labs { get; set; }                    // TEXT

        // 2) ¿Cómo se enteró del servicio?
        public string[] Medio { get; set; } = Array.Empty<string>();        // TEXT[]

        // 3) Oxigenación y Circulación
        public string[] Resp { get; set; } = Array.Empty<string>();         // TEXT[]
        public string[] Circ { get; set; } = Array.Empty<string>();         // TEXT[]

        // 4) Necesidad de Nutrición
        public string[] Nutricion { get; set; } = Array.Empty<string>();    // TEXT[]
        public string NutricionObs { get; set; }                            // TEXT

        // 5) Necesidad de Eliminación
        public string[] Urinario { get; set; } = Array.Empty<string>();     // TEXT[]
        public string[] Intestinal { get; set; } = Array.Empty<string>();   // TEXT[]

        // 6) Movilización y Estado de Conciencia
        public string[] Mov { get; set; } = Array.Empty<string>();          // TEXT[]
        public string[] Conciencia { get; set; } = Array.Empty<string>();   // TEXT[]

        // 7) Autocuidado y Reposo
        public string[] Sueno { get; set; } = Array.Empty<string>();        // TEXT[]
        public string Vestirse { get; set; }                                // TEXT (radio)
        public string Higiene { get; set; }                                 // TEXT (radio)
        public string[] Piel { get; set; } = Array.Empty<string>();         // TEXT[]
        public string PielUbicacion { get; set; }                           // TEXT

        // 8) Necesidad de Comunicación
        public string[] Lenguaje { get; set; } = Array.Empty<string>();     // TEXT[]
        public string[] Vision { get; set; } = Array.Empty<string>();       // TEXT[]
        public string[] Oido { get; set; } = Array.Empty<string>();         // TEXT[]

        // 9) Seguridad y Factores Psicosociales
        public string[] Seg { get; set; } = Array.Empty<string>();          // TEXT[]
        public string Religiosos { get; set; }                              // TEXT (radio "si"/"no")
        public string CreenciasObservaciones { get; set; }                  // TEXT
        public string ConoceMotivo { get; set; }                            // TEXT (radio "si"/"no")
        public string NecesitaInfo { get; set; }                            // TEXT (radio "si"/"no")

        // 10) Medicación actual y plan terapéutico
        public string MedicacionActual { get; set; }                        // TEXT
        public string PlanTerapeutico { get; set; }                         // TEXT

        public DateTime Fecha { get; set; }
    }
}
