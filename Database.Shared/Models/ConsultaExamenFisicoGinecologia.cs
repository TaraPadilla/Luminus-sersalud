using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Database.Shared.Models;
using System.Reflection.Emit;

namespace Database.Shared.Models
{
    public class ConsultaExamenFisicoGinecologia
    {
        public int Id { get; set; }
        public string Mamas { get; set; }
        public string Especuloscopia { get; set; }
        public string TactoVaginal { get; set; }
        public string TactoRectal { get; set; }
        public string VulvaVagina { get; set; }
    }
}