using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;

namespace sistema.Models
{
    public class CitasSchedulerViewModel
    {
        public Cita Cita {get;set;}
        public IList<Cita> CitasList {get;set;}
    }
}