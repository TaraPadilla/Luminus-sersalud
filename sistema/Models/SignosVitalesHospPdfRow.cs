using System;
using System.Collections.Generic;

namespace farmamest.Models
{
    public class SignosVitalesHospPdfRow
    {
        public DateTime FechaHora { get; set; }
        public string Profesional { get; set; }
        public string Observaciones { get; set; }
        public bool Autorizado { get; set; }
        public string AutorizadoPor { get; set; }
        public string FirmaBase64 { get; set; }
        public Dictionary<string, string> Valores { get; set; } = new();
    }
}
