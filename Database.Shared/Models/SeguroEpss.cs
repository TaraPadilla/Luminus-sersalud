using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class SeguroEpss
    {

        public SeguroEpss()
        {
            Pacientes = new List<Paciente>();
        }
        public int Id { get; set; }
        public string Nombre {get;set;}
        public ICollection<Paciente> Pacientes {get;set;}


    }
}