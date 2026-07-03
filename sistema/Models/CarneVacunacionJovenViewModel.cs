using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using System;

namespace sistema.Models
{
    public class CarneVacunacionJovenViewModel 
    {
        public Paciente Paciente { get; set; }
        public List<VacunaPaciente> VacunasPaciente { get; set; }
    }
} 