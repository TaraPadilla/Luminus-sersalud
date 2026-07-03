using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class Sexo
    {

        public Sexo()
        {
            Clientes = new List<Paciente>();
        }
        public int Id { get; set; }
        public string DescripcionSexo {get;set;}
        public ICollection<Paciente> Clientes {get;set;}


    }
}