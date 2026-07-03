using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class ViaAdministracions
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string NombreViaAdministracion { get; set; }

        public bool Eliminado {get; set;}

        // public ICollection<Producto> Productos { get; set; }
    }
}