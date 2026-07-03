using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PacienteApnp
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public string Gestas { get; set; }
        public string Partos { get; set; }
        public string Abortos { get; set; }
        public string Cesareas { get; set; }
        public string Menarquia { get; set; }
        public string HijosVivos { get; set; }
        public string HijosMuertos { get; set; }
        public DateTime? FechaUltimaRegla { get; set; }
        public DateTime? FechaProbableParto { get; set; }
        public string Otros { get; set; }
    }
}
