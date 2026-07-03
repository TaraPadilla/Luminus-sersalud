using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Database.Shared.Models
{
    public class Seguro
    {

    

        public int Id { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Nombre { get; set; }
        public string Nit { get; set; }

        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Web { get; set; }
        public bool  Eliminado { get; set; }

        public ICollection<Proveedor> Proveedores { get; set; }
    }
}