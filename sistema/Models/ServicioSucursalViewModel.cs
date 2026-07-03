using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class ServicioSucursalViewModel
    {
        public int SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public bool Activar { get; set; }
    }
}