using System.Runtime.ExceptionServices;
using System.ComponentModel;
using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using System;

namespace sistema.Models
{
    public class PacientesRegistrarRetiroViewModel
    {
        public int PacienteId { get; set; }
        public string PacienteNombre { get; set; }
        public string MotivoRetiro { get; set; }
        public bool VolverAContactar { get; set; }
        public DateTime? FechaContacto { get; set; }
    }
}