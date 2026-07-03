namespace sistema.Models
{
    public class HistoriaClinicaViewModel
    {
        public int? ConsultaId { get; set; }
        public int? HistoriaId { get; set; }
        public int? PacienteId { get; set; }

        public string HistoriaEnfermedadActual { get; set; }
        public string ConsultaMotivo { get; set; }

        public string EstaEmbarazada { get; set; }
        public int? NumeroSemanasEmbarazo { get; set; }
        public string EstaAmamantando { get; set; }

        public string PacienteMedicos { get; set; }
        public string PacienteQuirurgicos { get; set; }
        public string PacienteAlergias { get; set; }
        public string PacienteTraumaticos { get; set; }
        public string PacienteVicios { get; set; }
        public string PacienteMedicamentos { get; set; }
        public string? ExamenFisicoEstadoGeneral { get; set; }
        public string ExamenFisicoPeso { get; set; }
        public string ExamenFisicoTalla { get; set; }
        public string ExamenFisicoFrecuenciaCardiaca { get; set; }
        public string ExamenFisicoFrecuenciaRespiratoria { get; set; }
        public string ExamenFisicoPresionArterial { get; set; }
        public string ExamenFisicoTemperatura { get; set; }
        public string ExamenFisicoSaturacionOxigeno { get; set; }
        public string ExamenFisicoGlasgow { get; set; }
        public string RevisionSistemasAparienciaGeneral { get; set; }
        public string RevisionSistemasCabeza { get; set; }
        public string RevisionSistemasOidosBoca { get; set; }
        public string RevisionSistemasCuello { get; set; }
        public string RevisionSistemasTorax { get; set; }
        public string RevisionSistemasAbdomen { get; set; }
        public string RevisionSistemasDorsoYExtremidades { get; set; }
        public string RevisionSistemasGenitales { get; set; }
        public string HistoriaClinicaComentario { get; set; }
        public string HistoriaClinicaImpresionClinica { get; set; }

    }
}
