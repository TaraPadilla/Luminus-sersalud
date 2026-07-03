using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class EstadoPagoConsulta
    {
        public EstadoPagoConsulta()
        {
            Consultas = new List<Consulta>();
        }
        public int Id { get; set; }
        public string Estado {get;set;}

        public ICollection<Consulta> Consultas {get;set;}
    }
}