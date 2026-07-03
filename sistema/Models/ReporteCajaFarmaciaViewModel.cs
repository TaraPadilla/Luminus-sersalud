using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class ReporteCajaFarmaciaViewModel
    {
        public IList<Caja> Cajas { get; set; }
        public string Desde {get;set;}
        public string Hasta {get;set;}
        public string NombreAmbiente {get;set;}
    }
}
