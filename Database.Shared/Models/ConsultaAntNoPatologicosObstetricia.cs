using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Database.Shared.Models;
using System.Reflection.Emit;

namespace Database.Shared.Models
{
    public class ConsultaAntNoPatologicosObstetricia
    {
        public int Id { get; set; }
        public string Gestas { get; set; }
        public string Partos { get; set; }
        public string Abortos { get; set; }
        public string Cesareas { get; set; }
        public string HijosVivos { get; set; }
        public string HijosMuertos { get; set; }
        public string Cotarquia { get; set; }
        public string Ultrasonido { get; set; }
        public string NumeroParejas { get; set; }

        public DateTime? FechaUltimaRegla { get; set; }
        public DateTime? FechaProbableParto { get; set; }
    }
}