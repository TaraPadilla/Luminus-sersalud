using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class Categoria
    {

        public Categoria()
        {

            Productos = new List<Producto>();

        }
        public int Id { get; set; }


        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreCategoria { get; set; }

        public bool Eliminado {get; set;}

        public ICollection<Producto> Productos { get; set; }
    }
}