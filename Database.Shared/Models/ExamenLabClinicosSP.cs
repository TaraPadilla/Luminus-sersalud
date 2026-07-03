using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Models
{
    public class ExamenLabClinicosSP
    {
        [Key]
        public int ExamenId { get; set; }
        public string ExamenNombre { get; set; }
        public string ExamenCodigo { get; set; }
        public string ExamenNombreMostrar { get; set; }
    }
}
