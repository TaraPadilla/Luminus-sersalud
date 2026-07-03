using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class Municipio
    {
        public int Id { get; set; }
        public string NombreMunicipio { get; set; }

        public int DepartamentoId { get; set; }
        public  Departamento Departamento { get; set; }
        public bool Eliminado { get; set; }
    }
}
