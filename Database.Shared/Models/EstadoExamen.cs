using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Database.Shared.Models
{
    public class EstadoExamen
    {

        public EstadoExamen()
        {
            Examenes = new List<Examen>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Nombre { get; set; }

        public ICollection<Examen>Examenes { get; set; }
    }
}