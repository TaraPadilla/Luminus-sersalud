using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class Grabacion
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string PalabraClave { get; set; }
        public bool Eliminada { get; set; }
    }
}