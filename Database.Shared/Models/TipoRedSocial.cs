using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class TipoRedSocial
    {
        public TipoRedSocial()
        {
            Personas = new List<Persona>();
        }
        public int Id { get; set; }
        public string NombreRedSocial { get; set; }
        public bool Eliminado { get; set; }
        public ICollection<Persona> Personas { get; set; }
    }
}