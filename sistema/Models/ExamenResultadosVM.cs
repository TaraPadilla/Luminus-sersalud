using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Database.Shared.Paginacion;
using System.ComponentModel.DataAnnotations;
using System;

namespace sistema.Models
{
    public class ExamenResultadosVM : Examen
    {
        public string UsuarioSolicita { get; set; }
        public string UsuarioIngresa { get; set; }
    }
}