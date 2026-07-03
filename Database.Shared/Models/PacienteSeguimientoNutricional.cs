using System.Net;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class PacienteSeguimientoNutricional
    {
        public int Id { get; set; }
        public int? PacienteId {get;set;}
        public Paciente Paciente {get; set;}
        public DateTime? Fecha { get; set; }
        public decimal? Peso { get; set; }
        public decimal? IMC { get; set; }
        public decimal? PGC { get; set; }
        public decimal? Cuello { get; set; }
        public decimal? Busto { get; set; }
        public decimal? CinturaAbdomen { get; set; }
        public decimal? Cadera { get; set; }
        public decimal? Muslo { get; set; }
        public decimal? Brazo { get; set; }
        public decimal? Muñeca { get; set; }

        public string FechaText
        {
            get { return Fecha == null ? "-" : String.Format("{0:dd-MM-yyyy}",Fecha); }
        } 
    }
}