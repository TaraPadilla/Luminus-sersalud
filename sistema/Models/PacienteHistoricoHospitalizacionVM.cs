using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class PacienteHistoricoHospitalizacionVM
    {
        public int? Id { get; set; }
        public string PacienteNombre { get; set; }
        public string HospitalizacionFechaInicio { get; set; }
        public string HospitalizacionFechaFin { get; set; }
        public string HabitacionNumeroNombre { get; set; }
    }
}
