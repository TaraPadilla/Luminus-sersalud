using System.Net;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class PacienteRangoSaludable
    {
        public int Id { get; set; }
        public int? PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public DateTime? Fecha { get; set; }
        public string IMC { get; set; }
        public string Peso { get; set; }
        public string PorcentajeGrasaCorporal { get; set; }

        public string FechaText
        {
            get { return Fecha == null ? "-" : String.Format("{0:dd-MM-yyyy}", Fecha); }
        }
    }
}