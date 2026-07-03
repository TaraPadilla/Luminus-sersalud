using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Database.Shared.Paginacion;
using Database.Shared.Models;
using System.Collections.Generic;

namespace sistema.Models
{
    public class PacienteArchivoVM
    {
        public int ArchivoId { get; set; }
        public string ArchivoFecha { get; set; }
        public string ArchivoNombre { get; set; }
        public string ArchivoUrl { get; set; }
    }
    public class ArchivoAutorizacionVM
    {
        public int ArchivoId { get; set; }
        public string ArchivoNombre { get; set; }
        public string ArchivoUrl { get; set; }
    }
}