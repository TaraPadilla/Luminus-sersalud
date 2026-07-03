using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class DetalleCuentaPorCobrar
    {
        public int Id { get; set; }
        public int CuentaPorCobrarId { get; set; }
        public CuentaPorCobrar CuentaPorCobrar { get; set; }
        public string Descripcion { get; set; }
        public int? HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        public int? EmergenciaId { get; set; }
        public Emergencia Emergencia { get; set; }
    }
}
