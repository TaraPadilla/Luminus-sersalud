using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class CategoriasCuentaContable
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Especificacion { get; set;}
        public bool Eliminado { get;set; }
    }
}
