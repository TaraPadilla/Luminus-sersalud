using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class CategoriaServicio
    {
        public CategoriaServicio()
        {
            Servicios = new List<Servicio>();
        }
        public int Id { get; set; }
        public string NombreCategoria { get; set; }
        public bool Eliminada { get; set; }
        public ICollection<Servicio> Servicios { get; set; }
    }
}