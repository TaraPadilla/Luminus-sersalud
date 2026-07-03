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
    public class PacientesCambiarFaseViewModel
    {
        public int PacienteId { get; set; }
        public string PacienteFechaRegistro { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteFaseTratamientoActual { get; set; }
        public string FaseTratamientoNueva { get; set; }
        public DateTime FechaCambioFase { get; set; }
        public decimal PesoAlIniciar { get; set; }
    }
}