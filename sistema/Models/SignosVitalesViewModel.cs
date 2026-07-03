using System.Collections.Generic;

namespace sistema.Models
{
    public class SignosVitalesViewModel
    {
        public int? ConsultaId { get; set; }
        public string? ExamenFisicoEstadoGeneral { get; set; }
        public string ExamenFisicoPeso { get; set; }
        public string ExamenFisicoTalla { get; set; }
        public string ExamenFisicoFrecuenciaCardiaca { get; set; }
        public string ExamenFisicoFrecuenciaRespiratoria { get; set; }
        public string ExamenFisicoPresionArterial { get; set; }
        public string ExamenFisicoTemperatura { get; set; }
        public string ExamenFisicoSaturacionOxigeno { get; set; }
        public string ExamenFisicoGlasgow { get; set; }
        public List<MedicamentoOtro> MedicamentosOtros { get; set; }

    }
}
