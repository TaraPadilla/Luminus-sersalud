using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System;
using DocumentFormat.OpenXml.Drawing;

namespace sistema.Models
{
    public class ExamenLabClinicoPreguntasViewModel
    {
        public int? Id { get; set; }
        public string Pregunta { get; set; }
        public string Detalles { get; set; }
        public bool? Respuesta { get; set; }
        public bool Eliminada { get; set; }
    }
}
