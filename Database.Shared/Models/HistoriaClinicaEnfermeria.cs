using System;

namespace Database.Shared.Models
{
    public class ConsultasHistoriaClinicaEnfermeria
    {
        public long Id { get; set; }
        public int ConsultaId { get; set; }
        public int PacienteId { get; set; }

        // 1) Tipo de consulta
        public string TipoConsulta { get; set; }

        // 2) Motivo de consulta
        public string MotivoConsulta { get; set; }

        // 3) Antecedentes personales y familiares
        public string AntecedentesPatologicos { get; set; }
        public string AntecedentesQuirurgicos { get; set; }
        public string AntecedentesTraumaticos { get; set; }
        public string Hospitalizaciones { get; set; }
        public string Alergias { get; set; }
        public string AntecedentesFamiliares { get; set; }

        // 4) Hábitos personales
        public string HabitoAlimentacion { get; set; }
        public string ActividadFisica { get; set; }
        public string HabitoAlcoholTexto { get; set; }
        public string HabitoTabacoTexto { get; set; }
        public string OtrosHabitos { get; set; }

        // 5) Signos vitales y antropometría
        public string PresionArterialTxt { get; set; }
        public int? FC { get; set; }
        public int? FR { get; set; }
        public decimal? TemperaturaC { get; set; }
        public int? SPO2 { get; set; }
        public decimal? PesoKg { get; set; }
        public decimal? TallaM { get; set; }
        public decimal? IMC { get; set; }

        // 6) Exploración física por aparatos y sistemas
        public string CabezaCuello { get; set; }
        public string ToraxPulmones { get; set; }
        public string Corazon { get; set; }
        public string Abdomen { get; set; }
        public string Extremidades { get; set; }
        public string SistemaNeurologico { get; set; }
        public string PielAnexos { get; set; }

        // 7) Valoración de enfermería
        public string ValConcienciaOrientacion { get; set; }
        public string ValEstadoNutricional { get; set; }
        public string ValEliminacion { get; set; }
        public string ValSuenoDescanso { get; set; }
        public string ValActividadMovilidad { get; set; }
        public string ValAutonomia { get; set; }

        // 8) Laboratorios
        public string Laboratorios { get; set; }

        // 9) Diagnóstico de enfermería
        public string DiagnosticoEnfermeria { get; set; }

        // 10) Plan de cuidados / Intervenciones
        public string AccionesRealizadas { get; set; }
        public string MedicamentosAdministrados { get; set; }
        public string Tratamiento { get; set; }

        // 11) Seguimiento / Evolución / Cita
        public string Seguimiento { get; set; }

        public DateTime Fecha { get; set; }
    }
}
