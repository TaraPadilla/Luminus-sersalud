using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Shared.Models
{
    public class EstadosEnvio
    {

        public EstadosEnvio()
        {

            Envios = new List<Envio>();

        }

        public int Id { get; set; }


        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Estado { get; set; }

        public ICollection<Envio> Envios { get; set; }
    }
}