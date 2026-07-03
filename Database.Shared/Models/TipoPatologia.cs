using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class TipoPatologia
    {
        public int Id { get; set; }
        public string Tipo {get;set;}
        public bool VerInputDescripcion { get; set; }
    }
}