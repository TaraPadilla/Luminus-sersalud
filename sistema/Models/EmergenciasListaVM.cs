using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace sistema.Models
{
    public class EmergenciasListaVM
    {
        public bool Ingresadas { get; set; }

        public bool Pagado { get; set; }

    }
}
