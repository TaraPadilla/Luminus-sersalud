using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class TipoEspecialidad
    {
        public TipoEspecialidad()
        {
            // Citas = new List<Cita>();
        }
        public int Id { get; set; }
        public string NombreEspecialidad {get;set;}

        // public ICollection<Cita> Citas {get;set;}
    }
}