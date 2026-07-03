using System.Collections.Generic;

namespace sistema.Models
{
    public class HospitalizacionAgregarExamenFisicoViewModel
    {
        public int HospitalizacionId { get; set; }
        public List<HospitalizacionDatoExamenFisicoExistenteViewModel> DatosExamen { get; set; }
        public string Observaciones { get; set; }
    }
}
