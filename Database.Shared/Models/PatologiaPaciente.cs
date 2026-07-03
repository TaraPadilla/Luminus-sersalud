using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class PatologiaPaciente
    {
        public int? Id { get; set; }
        public int PacienteId {get;set;}
        public Paciente Paciente {get; set;}
        public int TipoPatologiaId { get; set; }
        public TipoPatologia TipoPatologia { get; set; }
        public bool Madre { get; set; }
        public bool AbuelaMaterna { get; set; }
        public bool AbueloMaterno { get; set; }
        public bool OtrosMaterno { get; set; }
        public bool Padre { get; set; }
        public bool AbuelaPaterna { get; set; }
        public bool AbueloPaterno { get; set; }
        public bool Hermanos { get; set; }
        public bool OtrosPaterno { get; set; }
        public string DescripcionOtraPatologia { get; set; }
    }
}