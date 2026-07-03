using System.Runtime.CompilerServices;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using System;
using Database.Shared.Paginacion;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema.Models
{
    public class GrabacionViewModel
    {
        public int? GrabacionId { get; set; }
        public string Numero { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string PalabraClave { get; set; }
    }
}