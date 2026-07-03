using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Database.Shared.Models
{
    public class Banco
    {

        public Banco()
        {

            Proveedores = new List<Proveedor>();

        }

        public int Id { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public bool  Eliminado { get; set; }

        public ICollection<Proveedor> Proveedores { get; set; }
    }
}