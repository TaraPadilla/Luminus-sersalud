using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{
    public class ExamenFisicoViewModel
    {
        public ExamenFisico ExamenFisico {get;set;}
        public int ConsultaId {get;set;}
        public int Id
        {
            get { return ExamenFisico.Id; }
            set { ExamenFisico.Id = value; }
        }
    }
} 