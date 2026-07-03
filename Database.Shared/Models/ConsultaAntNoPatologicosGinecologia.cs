using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Database.Shared.Models;
using System.Reflection.Emit;

namespace Database.Shared.Models
{
    public class ConsultaAntNoPatologicosGinecologia
    {
        public int Id { get; set; }
        public string Menarquia { get; set; }
        public string FechaUltimaRegla { get; set; }
        public string CicloMenstrual { get; set; }
        public string MetodoAnticonceptivo { get; set; }
        public string LactanciaMaterna { get; set; }
        public string Gestas { get; set; }
        public string Partos { get; set; }
        public string Abortos { get; set; }
        public string Cesareas { get; set; }
        public string HijosVivos { get; set; }
        public string HijosMuertos { get; set; }
        public string Otros { get; set; }
    }
}