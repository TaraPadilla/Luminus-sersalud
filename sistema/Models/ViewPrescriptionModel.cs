using System;
using sistema.Models;
using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace sistema.Models
{
    public class ViewPrescriptionModel
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Direccion { get; set; } 
        public DateTime NextDate { get; set; }
        public int ConsultaId { get; set; }
        public List<DetallePrescripcion> Prescriptions { get; set; }

        public InfoConsultaViewModel InfoConsultaViewModel { get; set; }

    }
}
