using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class Consulta
    {
        public Consulta()
        {
            ConsultasCaracteristicasDentales = new List<ConsultaCaracteristicaDental>();
            ConsultasServicios = new List<ConsultaServicio>();
            Prescripciones = new List<Prescripcion>();
            ConsultaExamenArchivos = new List<ConsultaExamenArchivo>();
            ConsultaArchivos = new List<ConsultaArchivo>();
        }
        public int Id { get; set; }
        public int? CitasId { get; set; }
        public Citas Citas { get; set; }
        public int? HistoriaId { get; set; }
        public Historia Historia { get; set; }
        public int? HistoriaPediatriaId { get; set; }
        public HistoriaPediatria HistoriaPediatria { get; set; }
        public int? ExamenFisicoId { get; set; }
        public ExamenFisico ExamenFisico { get; set; }
        public int? ExamenFisicoPediatriaId { get; set; }
        public ExamenFisicoPediatria ExamenFisicoPediatria { get; set; }
        public int? ConsultaExamenFisicoGinecologiaId { get; set; }
        public ConsultaExamenFisicoGinecologia ConsultaExamenFisicoGinecologia { get; set; }
        public int? ConsultaAntPatologicosGinecologiaId { get; set; }
        public ConsultaAntPatologicosGinecologia ConsultaAntPatologicosGinecologia { get; set; }
        public int? ConsultaAntNoPatologicosGinecologiaId { get; set; }
        public ConsultaAntNoPatologicosGinecologia ConsultaAntNoPatologicosGinecologia { get; set; }
        public int? ConsultaAntNoPatologicosObstetriciaId { get; set; }
        public ConsultaAntNoPatologicosObstetricia ConsultaAntNoPatologicosObstetricia { get; set; }
        public string ObservacionesAdicionales { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoConsulta { get; set; }
        public string ConsultaMotivo { get; set; }
        public string ConsultaMotivoPediatria { get; set; }
        public DateTime FechaYHoraInicioConsulta { get; set; }
        public DateTime? FechaProximaConsulta { get; set; }
        public bool ProximaCitaAgendada { get; set; }
        public bool Archivado { get; set; }
        public int EstadoPagoConsultaId { get; set; }
        public EstadoPagoConsulta EstadoPagoConsulta { get; set; }
        public int? FaseTratamientoId { get; set; }
        public FaseTratamiento FaseTratamiento { get; set; }
        public string UrlFiles { get; set; }
        public string ExamenGinecologico { get; set; }
        public string ExamenNeurologico { get; set; }
        public string TipoConsulta { get; set; } // puede ser Primera consulta, Reconsulta, Observacion
        public string TipoReferencia { get; set; } // hospital regional, medico particular
        public string MedicoReferido { get; set; } // el medico al que se refire
        //Dental
        public DateTime? FechaUltimaRadiografiaDental { get; set; }
        public ICollection<MedicamentoOtroConsulta> MedicamentosOtros { get; set; }



        // Nuevos campos para datos del padre
        public string NombrePadre { get; set; }
        public DateTime? FechaNacimientoPadre { get; set; }
        public int? EdadPadre { get; set; }
        public string DPIPadre { get; set; }
        public string DireccionPadre { get; set; }
        public string TelefonoPadre { get; set; }
        public string CorreoPadre { get; set; }
        public string OcupacionPadre { get; set; }
        public string EmpresaPadre { get; set; }
        public string TelefonoEmpresaPadre { get; set; }
        public string DireccionEmpresaPadre { get; set; }

        // Nuevos campos para datos de la madre
        public string NombreMadre { get; set; }
        public DateTime? FechaNacimientoMadre { get; set; }
        public int? EdadMadre { get; set; }
        public string DPIMadre { get; set; }
        public string DireccionMadre { get; set; }
        public string TelefonoMadre { get; set; }
        public string CorreoMadre { get; set; }
        public string OcupacionMadre { get; set; }
        public string EmpresaMadre { get; set; }
        public string TelefonoEmpresaMadre { get; set; }
        public string DireccionEmpresaMadre { get; set; }

        // Nuevos campos para datos del acompañante
        public string AcompananteNombre { get; set; }
        public string AcompananteDPI { get; set; }
        public string AcompananteTelefono { get; set; }


        #region ECOGRAFIA ENDOCAVITARIA

        public string EcografiaEndocavitariaUtero { get; set; }
        public string EcografiaEndocavitariaLongitudinal { get; set; }
        public string EcografiaEndocavitariaTransverso { get; set; }
        public string EcografiaEndocavitariaEndometrio { get; set; }
        public string EcografiaEndocavitariaOvarioDerecho { get; set; }
        public string EcografiaEndocavitariaOvarioIzquierdo { get; set; }
        public string EcografiaEndocavitariaFondoSaco { get; set; }
        public string EcografiaEndocavitariaImpresionClinica { get; set; }
        public string EcografiaEndocavitariaComentario { get; set; }

        #endregion

        //Campo pendiente por revision
        public string notaOperatoria { get; set; }          //nota nuevo notaoperatoria pestaña notaOperatoria RECONSULTA

        #region REVISION POR SISTEMAS

        public string SistemaCardiopulmonar { get; set; }
        public string SistemaOsteoarticular { get; set; }
        public string SistemaHematologico { get; set; }
        public int? ConsultaRevisionSistemasId { get; set; }
        public ConsultaRevisionSistemas ConsultaRevisionSistemas { get; set; }
        public int? ConsultaRevisionSistemasPediatriaId { get; set; }
        public ConsultaRevisionSistemasPediatria ConsultaRevisionSistemasPediatria { get; set; }

        #endregion
        [Column("cie10_codigo")]
        public string Cie10Codigo { get; set; }
      

        public Cie10 Cie10 { get; set; }
        #region SOLO PARA MUJERES
        public string EstaEmbarazada { get; set; }
        public int? NumeroSemanasEmbarazo { get; set; }
        public string TomaPildorasAnticonceptivas { get; set; }
        public string EstaAmamantando { get; set; }

        #endregion

        #region Bebidas Alcoholicas
        public string BebeBebidasAlcoholicas { get; set; }
        public string AlcoholUltimas24Horas { get; set; }
        public string AlcoholSemanal { get; set; }

        #endregion

        #region ESTETICO

        public string Estetico_Metabolismo { get; set; }
        public string Estetico_Grasa { get; set; }
        public string Estetico_IMC { get; set; }
        public string Estetico_Agua { get; set; }
        public string Estetico_Obesidad { get; set; }
        public string Estetico_ContornoBrazos { get; set; }
        public string Estetico_ContornoBusto { get; set; }
        public string Estetico_ContornoAbdomen { get; set; }
        public string Estetico_ContornoCadera { get; set; }
        public string Estetico_ContornoPiernas { get; set; }
        public string Estetico_Estatura { get; set; }

        #endregion

        #region AREA TERAPEUTICA
        public string TerapeuticoDatosGenerales { get; set; }
        public string TerapeuticoActividadesDiarias { get; set; }
        public string TerapeuticoConQuienVive { get; set; }
        public string TerapeuticoHabitosAlimenticios { get; set; }
        public string TerapeuticoEjercicio { get; set; }
        public string TerapeuticoFinesSemana { get; set; }
        public string TerapeuticoHistoriaMedica { get; set; }
        public string TerapeuticoHistoriaPeso { get; set; }
        public string TerapeuticoObservaciones { get; set; }

        #endregion

        #region OBSTETRICIA - EVALUACION OBSTETRICA
        public string EvaluacionObstetricaAbdomenObstetrico { get; set; }
        public string EvaluacionObstetricaUteroGravio { get; set; }
        public string EvaluacionObstetricaFcf { get; set; }
        public string EvaluacionObstetricaAu { get; set; }
        public string EvaluacionObstetricaBishop { get; set; }

        public string EvaluacionObstetricaPresentacionLeopold { get; set; }
        public string EvaluacionObstetricaOtras { get; set; }
        public string EvaluacionObstetricaActividadUterina { get; set; }
        public string EvaluacionObstetricaMovimientoFetalPercetible { get; set; }
        public string EvaluacionObstetricaMovimientoFetalEspecifique { get; set; }
        public string EvaluacionObstetricaTactoVaginal { get; set; }
        public string EvaluacionObstetricaD { get; set; }
        public string EvaluacionObstetricaCms { get; set; }
        public string EvaluacionObstetricaBPorciento { get; set; }
        public string EvaluacionObstetricaAltitud { get; set; }
        public string EvaluacionObstetricaConsistencia { get; set; }
        public string EvaluacionObstetricaPosicionCervix { get; set; }
        public string EvaluacionObstetricaMembranasOvulares { get; set; }
        public string EvaluacionObstetricaLiquidoAmniotico { get; set; }
        public string EvaluacionObstetricaLiquidoAmnioticoEspecifique { get; set; }
        public string EvaluacionObstetricaPelvis { get; set; }

        #endregion

        #region ECOGRAFIA OBSTETRICA

        public string EcografiaObstetricaFeto { get; set; }
        public string EcografiaObstetricaEstado { get; set; }
        public string EcografiaObstetricaSituacion { get; set; }
        public string EcografiaObstetricaPresentacion { get; set; }
        public string EcografiaObstetricaPosicion { get; set; }
        public string EcografiaObstetricaDorso { get; set; }

        #endregion

        #region OBSTETRICIA - BIOMETRIA
        public int? NumeroBebes { get; set; }


        // Primer conjunto de propiedades
        public string ObsteBiometriaRlc { get; set; }
        public string ObsteBiometriaSg { get; set; }
        public string ObsteBiometriaW { get; set; }
        public string ObsteBiometriaDbp { get; set; }
        public string ObsteBiometriaHc { get; set; }
        public string ObsteBiometriaAc { get; set; }
        public string ObsteBiometriaLf { get; set; }
        public string ObsteBiometriaEg { get; set; }
        public string ObsteBiometriaFcf { get; set; }
        public string ObsteBiometriaPlacenta { get; set; }
        public string ObsteBiometriaGrado { get; set; }
        public string ObsteBiometriaIla { get; set; }
        public string ObsteBiometriaMalformaciones { get; set; }
        public string ObsteBiometriaPeso { get; set; }
        public string ObsteBiometriaSexo { get; set; }
        public string ObsteBiometriaFechaParto { get; set; }
        public string ObsteBiometriaComentario { get; set; }
        public string ObsteBiometriaPresentacion { get; set; }

        // Segundo conjunto de propiedades
        public string? ObsteBiometriaRlc2 { get; set; }
        public string? ObsteBiometriaSg2 { get; set; }
        public string? ObsteBiometriaW2 { get; set; }
        public string? ObsteBiometriaDbp2 { get; set; }
        public string? ObsteBiometriaHc2 { get; set; }
        public string? ObsteBiometriaAc2 { get; set; }
        public string? ObsteBiometriaLf2 { get; set; }
        public string? ObsteBiometriaEg2 { get; set; }
        public string? ObsteBiometriaFcf2 { get; set; }
        public string? ObsteBiometriaPlacenta2 { get; set; }
        public string? ObsteBiometriaGrado2 { get; set; }
        public string? ObsteBiometriaIla2 { get; set; }
        public string? ObsteBiometriaMalformaciones2 { get; set; }
        public string? ObsteBiometriaPeso2 { get; set; }
        public string? ObsteBiometriaSexo2 { get; set; }
        public string? ObsteBiometriaFechaParto2 { get; set; }
        public string? ObsteBiometriaComentario2 { get; set; }
        public string? ObsteBiometriaPresentacion2 { get; set; }

        // Tercer conjunto de propiedades
        public string? ObsteBiometriaRlc3 { get; set; }
        public string? ObsteBiometriaSg3 { get; set; }
        public string? ObsteBiometriaW3 { get; set; }
        public string? ObsteBiometriaDbp3 { get; set; }
        public string? ObsteBiometriaHc3 { get; set; }
        public string? ObsteBiometriaAc3 { get; set; }
        public string? ObsteBiometriaLf3 { get; set; }
        public string? ObsteBiometriaEg3 { get; set; }
        public string? ObsteBiometriaFcf3 { get; set; }
        public string? ObsteBiometriaPlacenta3 { get; set; }
        public string? ObsteBiometriaGrado3 { get; set; }
        public string? ObsteBiometriaIla3 { get; set; }
        public string? ObsteBiometriaMalformaciones3 { get; set; }
        public string? ObsteBiometriaPeso3 { get; set; }
        public string? ObsteBiometriaSexo3 { get; set; }
        public string? ObsteBiometriaFechaParto3 { get; set; }
        public string? ObsteBiometriaComentario3 { get; set; }
        public string? ObsteBiometriaPresentacion3 { get; set; }

        // Cuarto conjunto de propiedades
        public string? ObsteBiometriaRlc4 { get; set; }
        public string? ObsteBiometriaSg4 { get; set; }
        public string? ObsteBiometriaW4 { get; set; }
        public string? ObsteBiometriaDbp4 { get; set; }
        public string? ObsteBiometriaHc4 { get; set; }
        public string? ObsteBiometriaAc4 { get; set; }
        public string? ObsteBiometriaLf4 { get; set; }
        public string? ObsteBiometriaEg4 { get; set; }
        public string? ObsteBiometriaFcf4 { get; set; }
        public string? ObsteBiometriaPlacenta4 { get; set; }
        public string? ObsteBiometriaGrado4 { get; set; }
        public string? ObsteBiometriaIla4 { get; set; }
        public string? ObsteBiometriaMalformaciones4 { get; set; }
        public string? ObsteBiometriaPeso4 { get; set; }
        public string? ObsteBiometriaSexo4 { get; set; }
        public string? ObsteBiometriaFechaParto4 { get; set; }
        public string? ObsteBiometriaComentario4 { get; set; }
        public string? ObsteBiometriaPresentacion4 { get; set; }


        #endregion

        #region HOSPITALIZACION

        public bool Hospitalizado { get; set; }
        public int? HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }

        #endregion

        public ICollection<ConsultaCaracteristicaDental> ConsultasCaracteristicasDentales { get; set; }
        public ICollection<ConsultaExamenArchivo> ConsultaExamenArchivos { get; set; }
        public ICollection<ConsultaServicio> ConsultasServicios { get; set; }
        public ICollection<Prescripcion> Prescripciones { get; set; }
        public ICollection<ConsultaArchivo> ConsultaArchivos { get; set; }
        public ICollection<ConsultaExamenLabClinico> ConsultaExamenesAgregados { get; set; }
        public string GinecologiaConsultaMotivo { get; set; }
        public string ResponsableNit { get; set; }
        public string ResponsableNombre { get; set; }
        public string ResponsableDireccion { get; set; }
        public string ResponsableCorreo { get; set; }
         public string ResponsableNacionalidad { get; set; }
        public string ResponsableOcupacion { get; set; }

        public bool Eliminado { get; set; }
    }
}