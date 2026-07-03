using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class HospitalizacionPaqueteHospitalizacion
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        public int PaqueteHospitalizacionId { get; set; }
        public PaqueteHospitalizacion PaqueteHospitalizacion { get; set; }
        public bool Eliminado { get; set; }
        public ICollection<HospitalizacionDetallePaqueteHospitalizacion> HospitalizacionDetallePaqueteHospitalizacion { get; set; }
    }
}
