using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Database.Shared.Paginacion;

namespace sistema.Models
{
    public class ServicioCategoriaViewModel
    {
        public int? CategoriaId { get; set; }
        public string NombreCategoria { get; set; }
    }
}