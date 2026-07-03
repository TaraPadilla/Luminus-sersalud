using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class ReporteCitasViewModel
    {
        public IList<Citas> Citas {get;set;}
        public string Usuario {get;set;}
    }
} 