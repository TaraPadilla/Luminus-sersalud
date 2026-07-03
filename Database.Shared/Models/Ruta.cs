using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Shared.Models
{
    public class Ruta
    {

       public Ruta()
        {

            Envios = new List<Envio>();

        }

        public int Id { get; set; }


        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Destino { get; set; }

        public string Descripcion {get;set;}

        public bool Eliminado {get;set;}

         public ICollection<Envio> Envios { get; set; }

    }
}