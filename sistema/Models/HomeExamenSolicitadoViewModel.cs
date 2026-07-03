using Database.Shared.Models;
using Database.Shared.Paginacion;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class HomeExamenSolicitadoViewModel
    {
        public int ExamenId { get; set; }
        public string FechaSolicitud { get; set; }
        public string PacienteNombre { get; set; }
        public string MedicoNombre { get; set; }
        public string ClinicaNombre { get; set; }
    }
}
