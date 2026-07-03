using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;

namespace sistema.Models
{
    public class PacienteExpedienteViewModel
    {
        public string LogoExpediente { get; set; }
        public Paciente Paciente { get; set; }
    }
}