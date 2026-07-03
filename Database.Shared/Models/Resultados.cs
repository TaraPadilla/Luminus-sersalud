using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class Resultados
    {

        public Resultados()
        {
        }

        public int Id { get; set; }
        public int? DatosExamenesLabClinicoId {get;set;}
        public int? DetalleExamenId {get;set;}
        public string ValorResultado {get;set;}

        //public bool Larvas { get; set; }
        //public bool Quistes { get; set; }
        //public bool Huevos { get; set; }
        //public bool Trofozoitos { get; set; }

        public string Larvas { get; set; }
        public string Quistes { get; set; }
        public string Huevos { get; set; }
        public string Trofozoitos { get; set; }

        public DatosExamenesLabClinico  DatosExamenesLabClinico {get;set;}
        public DetalleExamen DetalleExamen {get;set;}


    }
}