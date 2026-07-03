using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;

namespace sistema.Models
{
    public class PresupuestoPdfViewModel
    {
        public string LogoPresupuesto { get; set; }
        public PresupuestoDental Presupuesto { get; set; }
    }
}