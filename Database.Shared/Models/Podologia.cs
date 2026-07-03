using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class ConsultasPodologia
    {
        public long Id { get; set; }
        public int ConsultaId { get; set; }
        public int PacienteId { get; set; }

        // 1) Antecedentes Médicos
        public string[] Enfermedades { get; set; } = Array.Empty<string>();       // TEXT[] en BD
        public string Enfermedades_Otros { get; set; }      // TEXT
        public string Medicamentos { get; set; }            // TEXT
        public string PresionArterial { get; set; }         // TEXT (p.ej. "120/80 mmHg")

        // 2) Examen del Pie
        // Pulsos (texto libre, provenientes de radios)
        public string Pulso_Pedio { get; set; }             // TEXT: "Palpable"|"Débil"|"Ausente"
        public string Pulso_TibialPosterior { get; set; }   // TEXT
        public string Pulso_Popliteo { get; set; }          // TEXT

        // Estado general
        public string TemperaturaPie { get; set; }          // TEXT: "Fría"|"Normal"|"Caliente"
        public bool? ProblemasCirculatorios { get; set; }   // BOOLEAN
        public string EstadoPiel { get; set; }              // TEXT: "Seca"|"Normal"|"Húmeda"
        public string ObservacionesExamen { get; set; }     // TEXT

        // 3) Tratamiento Realizado
        public string[] Procedimientos { get; set; } = Array.Empty<string>();        // TEXT[] en BD
        public string OtrosProcedimientos { get; set; }     // TEXT
        public string ObservacionesTratamiento { get; set; }// TEXT

        // 4) Indicaciones y Datos Finales
        public string Indicaciones { get; set; }            // TEXT
        public decimal? PesoKg { get; set; }                // NUMERIC(5,1)
        public decimal? EstaturaM { get; set; }             // NUMERIC(4,2)
        public DateTime? FechaAtencion { get; set; }        // DATE (solo fecha)
        public string Profesional { get; set; }             // TEXT
        public DateTime Fecha { get; set; }
    }
}
