using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PacientePediatricoApnp
    {
        public int Id { get; set; }
        public string Parto { get; set; }
        public string AtendidoPor { get; set; }
        public string PesoAlNacer { get; set; }
        public string Inmunizaciones { get; set; }
        public string Gesta { get; set; }
    }
}
