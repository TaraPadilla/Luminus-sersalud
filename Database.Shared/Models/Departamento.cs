using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
   public class Departamento
    {
        public int Id { get; set; }
        public string NombreDepartamento { get; set; }
        public bool Eliminado { get; set; }
    }
}
