using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Models
{
    public class DatosExamenesLabClinicoInusuales
    {
        public DatosExamenesLabClinicoInusuales()
        {
            Resultados = new List<Resultados>();
        }

        public int Id { get; set; }
        public int ExamenLabClinicoId { get; set; }
        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Campos { get; set; }
        public string Resultado { get; set; }
        public string ValorReferencia { get; set; }
        public bool Activo { get; set; } = true;
        public string Indicaciones { get; set; }
        public string Unidad { get; set; }
        public bool Eliminado { get; set; }

        public ExamenLabClinico ExamenLabClinico { get; set; }

        public ICollection<Resultados> Resultados { get; set; }
    }
}
