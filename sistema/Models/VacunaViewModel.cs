using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System;

namespace sistema.Models
{
    public class VacunaViewModel
    {
        public Vacuna Vacuna { get; set; } = new Vacuna();

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Preparacion { get; set; }
       
    }
}