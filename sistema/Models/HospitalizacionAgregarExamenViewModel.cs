using System.Collections.Generic;

namespace sistema.Models
{
    public class HospitalizacionAgregarExamenViewModel
    {
        public int HospitalizacionId { get; set; }
        public string CodigoSeguro { get; set; }

        public int PacienteId { get; set; }
        public int CuentaId { get; set; }
        public int ExamenLabClinicoId { get; set; }
        public int ExamenLabClinicoPrecioId { get; set; }
        public List<ExamenItem> Examenes { get; set; }

    }

    public class ExamenItem
    {
        public int ExamenLabClinicoId { get; set; }
        public string Observacion { get; set; }
    }
}
