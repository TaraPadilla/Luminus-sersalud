using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class LaboratorioProducto
    {
        public LaboratorioProducto()
        {
            Productos = new List<Producto>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreLaboratorioProducto { get; set; }

        public bool Eliminado {get; set;}

        public ICollection<Producto> Productos { get; set; }
    }
}