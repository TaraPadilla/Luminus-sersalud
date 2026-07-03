using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class Especialidad
    {
        public Especialidad()
        {
            Citas = new List<Citas>();
        }
        public int Id { get; set; }
        
        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreEspecialidad {get;set;}
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool Eliminado { get; set; }
        public ICollection<Citas> Citas {get;set;}
    }
}