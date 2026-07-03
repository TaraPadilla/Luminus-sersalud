using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Database.Shared.Models;
using System.Reflection.Emit;

namespace Database.Shared.Models
{
    public class ConsultaAntPatologicosGinecologia
    {
        public int Id { get; set; }
        public string Infecciones { get; set; }
        public string Ets { get; set; }
        public string Papanicolau { get; set; }
        public string Otros { get; set; }
    }
}