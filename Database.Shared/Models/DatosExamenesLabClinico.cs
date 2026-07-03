using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;

namespace Database.Shared.Models
{
    public class DatosExamenesLabClinico
    {

        public DatosExamenesLabClinico()
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
       
        public string Tipo { get; set; }
        public bool Eliminado { get; set; }

        public ExamenLabClinico ExamenLabClinico { get; set; }

        public ICollection<Resultados> Resultados { get; set; }

        


    }
}