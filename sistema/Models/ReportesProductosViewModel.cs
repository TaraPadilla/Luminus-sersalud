using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class ReportesProductosViewModel
    {
        public IList<Producto> Productos { get; set; }
        public string Usuario { get; set; }

 
        public IList<SelectListItem> Medicamentos { get; set; }


        public string MedicamentoSeleccionado { get; set; }
    }
}