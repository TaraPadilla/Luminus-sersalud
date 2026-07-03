using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class Medicos
    {

        public Medicos()
        {
            Examens = new List<Examen>();
        }
        public int Id { get; set; }


        public string Titulo {get;set;}
        public string Nombres { get; set; }
        public bool Eliminado {get; set;} = false;
        public ICollection<Examen> Examens { get; set; }
    }
}