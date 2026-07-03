// Domain/Entities/ConsultasSueroterapia.cs
using System;

namespace Database.Shared.Models
{
    public class ConsultasSueroterapia
    {
        public long Id { get; set; }
        public int PacienteId { get; set; }
        public int ConsultaId { get; set; }

        // 1) Valoración inicial
        public string? Motivo { get; set; }
        public string? DiagnosticoMedico { get; set; }
        public string? Labs { get; set; }

        // 2) Medio (checkbox múltiple)
        public string[]? Medio { get; set; }

        // 3) Oxigenación y circulación (checkbox múltiple)
        public string[]? Resp { get; set; }
        public string[]? Circ { get; set; }

        // 4) Nutrición
        public string[]? Nutricion { get; set; }
        public string? NutricionObs { get; set; }

        // 5) Plan
        public string? PlanTerapeutico { get; set; }

        // Navegación (opcional; ajusta los tipos/nombres a tus entidades reales)
        public Paciente? Paciente { get; set; }
        public Consulta? Consulta { get; set; }

        public DateTime Fecha { get; set; }
    }
}
