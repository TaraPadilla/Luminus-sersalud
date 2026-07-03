using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class PersonaSeguro
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nit { get; set; }
        public string Tipo { get; set; }
        public int PacienteId {get;set;}
        public Paciente Paciente { get; set; }
    }
}