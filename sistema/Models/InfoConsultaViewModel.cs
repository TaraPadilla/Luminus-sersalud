using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using System;
using Database.Shared.Paginacion;

namespace sistema.Models
{
    public class InfoConsultaViewModel : ConsultasViewModel
    {
        public Consulta Consulta { get; set; }
        public Paciente Paciente { get; set; }
        public Prescripcion Prescripcion { get; set; }
        public string EmpleadoText { get; internal set; }

        //public Examen Examen { get; set; }
        //public ConsultaPrescripcionViewModel ConsultaPrescripcionViewModel { get; set; }


    }
}