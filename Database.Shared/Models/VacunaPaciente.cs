using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class VacunaPaciente
    {
        public int? Id { get; set; }
        public int PacienteId {get;set;}
        public Paciente Paciente {get; set;}
        public int VacunaId { get; set; }
        public Vacuna Vacuna {get; set;}
        public bool Primera { get; set; }
        public bool Segunda { get; set; }
        public bool Tercera { get; set; }
        public bool PrimerRefuerzo { get; set; }
        public bool SegundoRefuerzo { get; set; }
        public DateTime? FechaPrimera { get; set; }
        public DateTime? FechaSegunda{ get; set; }
        public DateTime? FechaTercera { get; set; }
        public DateTime? FechaPrimerRefuerzo { get; set; }
        public DateTime? FechaSegundoRefuerzo { get; set; } 
    }
}