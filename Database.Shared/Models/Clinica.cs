using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class Clinica
    {

        public Clinica()
        {
            Examens = new List<Examen>();
        }
        public int Id { get; set; }
        public string NombreClinica { get; set; }
        public bool Eliminado {get; set;} = false;
        public ICollection<Examen> Examens { get; set; }
    }
}