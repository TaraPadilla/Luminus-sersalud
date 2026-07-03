using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class SucursalViewModel
    {
        public int? SucursalId { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Horario { get; set; }
    }
}
