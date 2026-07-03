using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class FaseTratamiento
    {
        public int Id { get; set; }
        public string NombreFase { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }
    }
}
