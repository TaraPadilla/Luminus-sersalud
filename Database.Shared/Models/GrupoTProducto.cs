using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class GrupoTProducto
    {

        public GrupoTProducto()
        {
            Productos = new List<Producto>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreGrupoT { get; set; }

        public bool Eliminado {get; set;}

        public ICollection<Producto> Productos { get; set; }
    }
}