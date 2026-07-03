//EN PROCESO DE SER ELIMINADA ESTA CLASE PUES YA SE SIMPLIFICO
//UTILIZANDO LA MISMA CONSULTAS VIEWMODEL
//using Database.Shared.Models;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Database.Shared.IRepository;
//using Microsoft.AspNetCore.Http;
//using Database.Shared.Data;

//namespace sistema.Models
//{
//    public class PacienteConsultaHistoricoVM
//    {
//        public int ConsultaId { get; set; }
//        public string Fecha { get; set; }
//        public string CitaMotivo { get; set; }
//        public List<ConsultasServicioAgregadoViewModel> ServiciosConsulta { get; set; }

//        public string ConsultaMotivo { get; set; }

//        #region HISTORIA CLINICA
//        public string HistoriaClinicaHistoriaProblema { get; set; }
//        public string HistoriaClinicaSintomas { get; set; }
//        public string HistoriaClinicaHistoriaEnfermedadActual { get; set; }
//        public string HistoriaClinicaDiagnostico { get; set; }
//        public string HistoriaClinicaComentario { get; set; }
//        public string HistoriaClinicaImpresionClinica { get; set; }

//        #endregion

//        #region EXAMEN FISICO

//        public string ExamenFisicoEstadoGeneral { get; internal set; }
//        public string ExamenFisicoPeso { get; internal set; }
//        public string ExamenFisicoTalla { get; internal set; }
//        public string ExamenFisicoFrecuenciaCardiaca { get; internal set; }
//        public string ExamenFisicoFrecuenciaRespiratoria { get; internal set; }
//        public string ExamenFisicoPresionArterial { get; internal set; }
//        public string ExamenFisicoTemperatura { get; internal set; }
//        public string ExamenFisicoSaturacionDeOxigeno { get; internal set; }
//        public string ExamenFisicoGlasgow { get; internal set; }
//        public string ExamenFisicoPediatricoPesoTalla { get; internal set; }
//        public string ExamenFisicoPediatricoPesoEdad { get; internal set; }
//        public string ExamenFisicoPediatricoTallaEdad { get; internal set; }

//        #endregion

//        #region REVISION POR SISTEMAS

//        public string RevisionSistemasAparienciaGeneral { get; set; }
//        public string RevisionSistemasCabeza { get; set; }
//        public string RevisionSistemasOidosBoca { get; set; }
//        public string RevisionSistemasCuello { get; set; }
//        public string RevisionSistemasTorax { get; set; }
//        public string RevisionSistemasAbdomen { get; set; }

//        #endregion

//        #region GINECOLOGIA - ANT PATOLOGICOS

//        public string GinecologiaAntPatologicosInfecciones { get; set; }
//        public string GinecologiaAntPatologicosEts { get; set; }
//        public string GinecologiaAntPatologicosPapanicolau { get; set; }
//        public string GinecologiaAntPatologicosOtros { get; set; }

//        #endregion

//        #region GINECOLOGIA - ANT NO PATOLOGICOS

//        public string GinecologiaAntNoPatologicosMenarquia { get; set; }
//        public string GinecologiaAntNoPatologicosFechaUltimaRegla { get; set; }
//        public string GinecologiaAntNoPatologicosCicloMenstrual { get; set; }
//        public string GinecologiaAntNoPatologicosMetodoAnticonceptivo { get; set; }
//        public string GinecologiaAntNoPatologicosLactanciaMaterna { get; set; }
//        public string GinecologiaAntNoPatologicosGestas { get; set; }
//        public string GinecologiaAntNoPatologicosPartos { get; set; }
//        public string GinecologiaAntNoPatologicosAbortos { get; set; }
//        public string GinecologiaAntNoPatologicosCesareas { get; set; }
//        public string GinecologiaAntNoPatologicosHijosVivos { get; set; }
//        public string GinecologiaAntNoPatologicosHijosMuertos { get; set; }
//        public string GinecologiaAntNoPatologicosOtros { get; set; }

//        #endregion

//        #region GINECOLOGIA - EXAMEN FISICO

//        public string GinecologiaExamenFisicoMamas { get; set; }
//        public string GinecologiaExamenFisicoEspeculoscopia { get; set; }
//        public string GinecologiaExamenFisicoTactoVaginal { get; set; }
//        public string GinecologiaExamenFisicoTactoRectal { get; set; }
//        public string GinecologiaExamenFisicoVulvaVagina { get; set; }

//        #endregion
//    }
//}