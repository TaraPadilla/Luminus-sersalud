using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PacienteResultadoExamenLaboratorio
    {
        public int Id { get; set; }
        public DateTime? Fecha { get; set; }
        public int? PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public string GlucosaPre { get; set; }
        public string GlucosaPos { get; set; }
        public string HemoglobinaGlicosilada { get; set; }
        public string CurvaGlucosa { get; set; }
        public string ColesterolTotal { get; set; }
        public string Trigliceridos { get; set; }
        public string PerfilLipidico { get; set; }
        public string Creatinina { get; set; }
        public string AcidoUrico { get; set; }
        public string Hemoglobina { get; set; }
        public string T3 { get; set; }
        public string T4 { get; set; }
        public string ExamenHeces { get; set; }
        public string ExamenOrina { get; set; }
        public string Otros { get; set; }
        public string UrlResultados { get; set; }
    }
}
