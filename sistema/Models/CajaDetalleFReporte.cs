using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Database.Shared.Paginacion;

namespace sistema.Models
{
    public class CajaDetalleFReporte 
    {
        public Caja Caja {get;set;} = new Caja();
    }
} 