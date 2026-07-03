using System.Runtime.CompilerServices;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using System;
using Database.Shared.Paginacion;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Cryptography;
using System.Reflection.Emit;

namespace sistema.Models
{
    public class ConsultasViewModel
    {
        #region DATOS UTILIZADOS PDF

        public string EstablecimientoImagenLogo { get; set; }
        public string EstablecimientoImagenFirma { get; set; }

        public string EstablecimientoImagenFirmaMedico { get; set; }

        public string EstablecimientoDireccion { get; set; }
        public string EstablecimientoTelefono { get; set; }
        public string EstablecimientoCorreoElectronico { get; set; }

        #endregion

        public bool HabilitarListaEmpleados { get; set; }

        public InfoConsultaViewModel InfoConsultaViewModel { get; set; }
        public bool HabilitarEdicion { get; set; } = true;
        public bool HayPrescripcion { get; set; } = false;

        public int? CitaId { get; set; }
        public int? ConsultaId { get; set; }
        public bool PrimeraConsulta { get; set; }
        public int? PacienteId { get; set; }
        public string Fecha { get; set; }
        public string Nombres { get; set; }
        public string Servicio { get; set; }
        public string CitaMotivo { get; set; }
        public string ConsultaMotivo { get; set; }
        public int? EstadoPagoId { get; set; } = 2;
        public int? FaseTratamientoId { get; set; }
        public string MedicoAsignado { get; set; }
        public string FirmaEmpleado { get; set; }  // Esta es la propiedad que necesitas

        public DateTime? FechaYHoraInicio { get; set; }
        public DateTime? FechaProximaConsulta { get; set; }
        public bool ProximaCitaAgendada { get; set; }


        #region DATOS DEL PADRE
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
        #endregion

        #region DATOS DE LA MADRE
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
        #endregion

        #region DATOS DEL ACOMPAÑANTE
        public string AcompananteNombre { get; set; }
        public string AcompananteDPI { get; set; }
        public string AcompananteTelefono { get; set; }
        #endregion



        #region PACIENTE - INFO

        public string PacienteNombre { get; set; }
        public string PacienteNit { get; set; }
        public string PacienteDireccion { get; set; }
        public string PacienteSeguroEPSS { get; set; }
        public string NombreEncargado { get; set; }
        public string DPIEncargado { get; set; }

        public string PacienteAlias { get; set; }
        public string PacienteSexo { get; set; }
        public string PacienteFechaNacimiento { get; set; }
        public string PacienteNivelEducativoCompletado { get; set; }

        public int? PacienteDepartamentoId { get; set; }
        public string PacienteDepartamentoNombre { get; set; }

        public int? PacienteMunicipioId { get; set; }
        public string PacienteMunicipioNombre { get; set; }

        public string PacienteEdad { get; set; }
        public string PacienteTelefono { get; set; }
        public string PacienteCelular { get; set; }


        #endregion

        #region ANTECEDENTES - ADULTOS
        public string PacienteMedicos { get; set; }
        public string PacienteQuirurgicos { get; set; }
        public string PacienteTraumaticos { get; set; }
        public string PacienteAlergias { get; set; }
        public string PacienteVicios { get; set; }
        public string PacienteMedicamentos { get; set; }

        #endregion

        #region ANTECEDENTES - PEDIATRICO
        public string PediatricoAntMedicos { get; set; }
        public string PediatricoAntQuirurgicos { get; set; }
        public string PediatricoAntTraumaticos { get; set; }
        public string PediatricoAntAlergias { get; set; }
        public string PediatricoAntVicios { get; set; }
        public string PediatricoAntMedicamentos { get; set; }

        #endregion

        public List<MedicamentoOtro> MedicamentosOtros { get; set; }

        public string ObservacionesAdicionales { get; set; }
        public decimal? CostoConsulta { get; set; }
        //public decimal? CuentasPorCobrar { get; set; }
        public string UrlFiles { get; set; }
        public string CitaTipoAtencion { get; set; } //IGSS, PRIVADO, SEGURO
        public string TipoConsulta { get; set; } // puede ser Primera consulta, Reconsulta, Observacion
        public string TipoReferencia { get; set; } // hospital regional, medico particular
        public string MedicoReferido { get; set; } // el medico al que se refire

        #region GINECOLOGIA - ANT PATOLOGICOS


        public string GinecologiaAntPatologicosInfecciones { get; set; }
        public string GinecologiaAntPatologicosEts { get; set; }
        public string GinecologiaAntPatologicosPapanicolau { get; set; }
        public string GinecologiaAntPatologicosOtros { get; set; }

        #endregion

        #region GINECOLOGIA - ANT NO PATOLOGICOS

        public string GinecologiaConsultaMotivo { get; set; }

        public int? GinecologiaAntNoPatologicosId { get; set; }
        public string GinecologiaAntNoPatologicosMenarquia { get; set; }
        public string GinecologiaAntNoPatologicosFechaUltimaRegla { get; set; }
        public string GinecologiaAntNoPatologicosCicloMenstrual { get; set; }
        public string GinecologiaAntNoPatologicosMetodoAnticonceptivo { get; set; }
        public string GinecologiaAntNoPatologicosLactanciaMaterna { get; set; }
        public string GinecologiaAntNoPatologicosGestas { get; set; }
        public string GinecologiaAntNoPatologicosPartos { get; set; }
        public string GinecologiaAntNoPatologicosAbortos { get; set; }
        public string GinecologiaAntNoPatologicosCesareas { get; set; }
        public string GinecologiaAntNoPatologicosHijosVivos { get; set; }
        public string GinecologiaAntNoPatologicosHijosMuertos { get; set; }
        public string GinecologiaAntNoPatologicosOtros { get; set; }

        #endregion

        #region OBSTETRICIA - ANT NO PATOLOGICOS

        public int? ObstetriciaAntNoPatologicosId { get; set; }
        public string ObstetriciaAntNoPatologicosGestas { get; set; }
        public string ObstetriciaAntNoPatologicosPartos { get; set; }
        public string ObstetriciaAntNoPatologicosAbortos { get; set; }
        public string ObstetriciaAntNoPatologicosCesareas { get; set; }
        public string ObstetriciaAntNoPatologicosHijosVivos { get; set; }
        public string ObstetriciaAntNoPatologicosHijosMuertos { get; set; }
        public string ObstetriciaAntNoPatologicosFechaUltimaRegla { get; set; }
        public string ObstetriciaAntNoPatologicosFechaProbableParto { get; set; }
        public string ObstetriciaAntNoPatologicosCotarquia { get; set; }
        public string ObstetriciaAntNoPatologicosUltrasonido { get; set; }
        public string ObstetriciaAntNoPatologicosNumeroParejas { get; set; }

        #endregion

        #region GINECOLOGIA - EXAMEN FISICO

        public string GinecologiaExamenFisicoMamas { get; set; }
        public string GinecologiaExamenFisicoEspeculoscopia { get; set; }
        public string GinecologiaExamenFisicoTactoVaginal { get; set; }
        public string GinecologiaExamenFisicoTactoRectal { get; set; }
        public string GinecologiaExamenFisicoVulvaVagina { get; set; }

        #endregion

        #region HISTORIA CLINICA

        public int? HistoriaId { get; set; }
        public string HistoriaProblema { get; set; }
        public string Sintomas { get; set; }
        public string HistoriaEnfermedadActual { get; set; }
        public string Diagnostico { get; set; }
        public string HistoriaClinicaComentario { get; set; }
        public string HistoriaClinicaImpresionClinica { get; set; }

        #endregion

        #region HISTORIA CLINICA - PEDIATRIA


        public string HistoriaPediatriaConsultaMotivo { get; set; }
        public string HistoriaPediatriaHistoriaProblema { get; set; }
        public string HistoriaPediatriaSintomas { get; set; }
        public string HistoriaPediatriaHistoriaEnfermedadActual { get; set; }
        public string HistoriaPediatriaDiagnostico { get; set; }
        public string HistoriaPediatriaHistoriaClinicaComentario { get; set; }
        public string HistoriaPediatriaHistoriaClinicaImpresionClinica { get; set; }

        #endregion

        #region PACIENTE - APNP

        public string PacienteApnpGestas { get; set; }
        public string PacienteApnpPartos { get; set; }
        public string PacienteApnpAbortos { get; set; }
        public string PacienteApnpCesareas { get; set; }
        public string PacienteApnpMenarquia { get; set; }
        public string PacienteApnpHijosVivos { get; set; }
        public string PacienteApnpHijosMuertos { get; set; }
        public string PacienteApnpFechaUltimaRegla { get; set; }
        public string PacienteApnpFechaProbableParto { get; set; }
        public string PacienteApnpOtros { get; set; }

        #endregion

        #region PACIENTE - APNP - PEDIATRICO

        public string PediatricoApnpParto { get; set; }
        public string PediatricoApnpAtendidoPor { get; set; }
        public string PediatricoApnpPesoAlNacer { get; set; }
        public string PediatricoApnpInmunizaciones { get; set; }
        public string PediatricoApnpGesta { get; set; }

        #endregion


        //Neurologico
        public string ExamenNeurologico { get; set; }


        #region REVISION POR SISTEMAS

        public string SistemaCardiopulmonar { get; set; }
        public string SistemaOsteoarticular { get; set; }
        public string SistemaHematologico { get; set; }
        public string RevisionSistemasAparienciaGeneral { get; set; }
        public string RevisionSistemasCabeza { get; set; }
        public string RevisionSistemasOidosBoca { get; set; }
        public string RevisionSistemasCuello { get; set; }
        public string RevisionSistemasTorax { get; set; }
        public string RevisionSistemasAbdomen { get; set; }
        public string RevisionSistemasDorsoYExtremidades { get; set; }
        public string RevisionSistemasGenitales { get; set; }


        #endregion

        #region REVISION POR SISTEMAS - PEDIATRIA
        public string RevisionSistemasPediatriaAparienciaGeneral { get; set; }
        public string RevisionSistemasPediatriaCabeza { get; set; }
        public string RevisionSistemasPediatriaOidosBoca { get; set; }
        public string RevisionSistemasPediatriaCuello { get; set; }
        public string RevisionSistemasPediatriaTorax { get; set; }
        public string RevisionSistemasPediatriaAbdomen { get; set; }
        public string RevisionSistemasPediatriaDorsoYExtremidades { get; set; }
        public string RevisionSistemasPediatriaGenitales { get; set; }

        #endregion

        //Sección solo para mujeres
        public string EstaEmbarazada { get; set; }
        public string NumeroSemanasEmbarazo { get; set; }
        public string TomaPildorasAnticonceptivas { get; set; }
        public string EstaAmamantando { get; set; }

        //Bebidas Alcoholicas
        public string BebeBebidasAlcoholicas { get; set; }
        public string AlcoholUltimas24Horas { get; set; }
        public string AlcoholSemanal { get; set; }

        //Dental
        public DateTime? FechaUltimaRadiografiaDental { get; set; }

        //Estetico
        public string Metabolismo { get; set; }
        public string Grasa { get; set; }
        public string IMC { get; set; }
        public string Agua { get; set; }
        public string Obesidad { get; set; }
        public string ContornoBrazos { get; set; }
        public string ContornoBusto { get; set; }
        public string ContornoAbdomen { get; set; }
        public string ContornoCadera { get; set; }
        public string ContornoPiernas { get; set; }
        public string Estatura { get; set; }

        //Ginecologico
        public string ExamenGinecologico { get; set; }
        public string PacienteCicloMenstGine { get; set; }
        public string PacienteETSGine { get; set; }
        public int? PacienteVIHGine { get; set; }
        public string PacienteGrupoFactorGine { get; set; }
        public string PacienteTorchGine { get; set; }
        public string PacienteInicioVidaSexualGine { get; set; }
        public int? PacienteParejasSexGine { get; set; }
        public string PacienteObesidadGine { get; set; }
        public string PacienteDesnutricionGine { get; set; }
        public string PacienteQGine { get; set; }
        public string PacientePGine { get; set; }
        public string PacienteABGine { get; set; }
        public string PacienteCGine { get; set; }
        public string PacienteFURGine { get; set; }
        public int? PacienteMuerteNeoGine { get; set; }
        public string PacienteFPPGine { get; set; }
        public string PacienteHVGine { get; set; }
        public string PacienteMuerteGine { get; set; }
        public bool? PacienteControlPrenatalGine { get; set; }
        public string PacienteComadronaGine { get; set; }
        public int? PacienteNoControlesGine { get; set; }


        #region OBSTETRICIA - EVALUACION OBSTETRICA

        public string EvaluacionObstetricaUteroGravio { get; set; }
        public string EvaluacionObstetricaAbdomenObstetrico { get; set; }
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
        public string EvaluacionObstetricaBPorciento { get; set; }
        public string EvaluacionObstetricaAltitud { get; set; }
        public string EvaluacionObstetricaPosicionCervix { get; set; }
        public string EvaluacionObstetricaMembranasOvulares { get; set; }
        public string EvaluacionObstetricaLiquidoAmniotico { get; set; }
        public string EvaluacionObstetricaLiquidoAmnioticoEspecifique { get; set; }
        public string EvaluacionObstetricaPelvis { get; set; }
        public string EvaluacionObstetricaConsistencia { get; set; }
        public string EvaluacionObstetricaCms { get; set; }

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

        public string? Color { get; set; }

        // Primer conjunto de propiedades
        public string ObsteBiometriaRlc { get; set; }
        public string ObsteBiometriaSg { get; set; }
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
        public string ObsteBiometriaW { get; set; }
        public string ObsteBiometriaPresentacion { get; set; }


        // Segundo conjunto de propiedades
        public string? ObsteBiometriaRlc2 { get; set; }
        public string? ObsteBiometriaSg2 { get; set; }
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
        public string? ObsteBiometriaW2 { get; set; }
        public string? ObsteBiometriaPresentacion2 { get; set; }

        // Tercer conjunto de propiedades
        public string? ObsteBiometriaRlc3 { get; set; }
        public string? ObsteBiometriaSg3 { get; set; }
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
        public string? ObsteBiometriaW3 { get; set; }
        public string? ObsteBiometriaPresentacion3 { get; set; }


        // Cuarto conjunto de propiedades
        public string? ObsteBiometriaRlc4 { get; set; }
        public string? ObsteBiometriaSg4 { get; set; }
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
        public string? ObsteBiometriaW4 { get; set; }
        public string? ObsteBiometriaPresentacion4 { get; set; }


        #endregion

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




        public SelectList EstadosPagosLista { get; set; }
        public SelectList ServiciosSelectListItems { get; set; }
        public SelectList FasesTratamientoSelectListItems { get; set; }




        public void Init(ICitas citaRepository, IServicio _serviciosRepository, IPacientes _pacientesRepository, IEmpleado empleadoRepository = null)
        {
            ServiciosSelectListItems = new SelectList(_serviciosRepository.GetListaServicios(), "Id", "NombreServicio");
            EstadosPagosLista = new SelectList(citaRepository.EstadoPagosConsultasLista(), "Id", "Estado", 2);
            FasesTratamientoSelectListItems = new SelectList(_pacientesRepository.GetFasesTratamiento(), "Id", "NombreFase");
            //SucursalesSelectList = new SelectList(sucursalRepository.GetList(), "Id", "NombreSucursal");

            EspecialidadSelectListItem = new SelectList(citaRepository.GetEspecialidadesList(), "Id", "NombreEspecialidad");
            if (empleadoRepository != null)
            {
                // Lógica para el caso en que se proporciona un IEmpleado
                EmpleadosSelectList = new SelectList(empleadoRepository.GetListEmpleadoTipoProfesional(), "Id", "NombreYApellidos");
            }
            else
            {
                // Lógica para el caso en que NO se proporciona un IEmpleado
            }
        }

        public DateTime? FechaUltimaConsulta { get; set; }
        public string MotivoUltimaConsulta { get; set; }

        #region EXAMEN FISICO

        public int? ExamenFisicoId { get; set; }
        public string ExamenFisicoEstadoGeneral { get; set; }
        public string ExamenFisicoTemperatura { get; set; }
        public string ExamenFisicoFrecuenciaRespiratoria { get; set; }
        public string ExamenFisicoFrecuenciaCardiaca { get; set; }
        public string ExamenFisicoSaturacionOxigeno { get; set; }
        public string PresionArterialBrazoDerecho { get; set; }
        public string ExamenFisicoPresionArterialBrazoIzquierdo { get; set; }
        public string ExamenFisicoPresionArterial { get; set; }
        public string ExamenFisicoPeso { get; set; }
        public string ExamenFisicoTalla { get; set; }
        public string ExamenFisicoIMC { get; set; }
        public string ExamenFisicoObservaciones { get; set; }
        public string ExamenFisicoGlucosa { get; set; }
        public string ExamenFisicoGlasgow { get; set; }
        public string ExamenFisicoTensionArterial { get; set; }
        public string ExamenFisicoDerecho { get; set; }
        public string ExamenFisicoIzquierdo { get; set; }
        public string ExamenFisicoPresionArterialMedia { get; set; }

        #endregion



        #region REVISION POR SISTEMAS - NUEVA VERSION

        public string NewRevisionSistemasNeurologico { get; set; }
        public string NewRevisionSistemasCardiovascular { get; set; }
        public string NewRevisionSistemasRespiratorio { get; set; }
        public string NewRevisionSistemasGastrointestinal { get; set; }
        public string NewRevisionSistemasMusculoesqueletico { get; set; }
        public string NewRevisionSistemasPielFanera { get; set; }
        public string NewRevisionSistemasGenitourinario { get; set; }

        #endregion

        #region EXAMEN FISICO - PEDIATRIA

        public string ExamenFisicoPediatriaEstadoGeneral { get; set; }
        public string ExamenFisicoPediatriaTemperatura { get; set; }
        public string ExamenFisicoPediatriaFrecuenciaRespiratoria { get; set; }
        public string ExamenFisicoPediatriaFrecuenciaCardiaca { get; set; }
        public string ExamenFisicoPediatriaSaturacionOxigeno { get; set; }
        public string ExamenFisicoPediatriaPresionArterialBrazoDerecho { get; set; }
        public string ExamenFisicoPediatriaPresionArterialBrazoIzquierdo { get; set; }
        public string ExamenFisicoPediatriaPresionArterial { get; set; }
        public string ExamenFisicoPediatriaPeso { get; set; }
        public string ExamenFisicoPediatriaTalla { get; set; }
        public string ExamenFisicoPediatriaIMC { get; set; }
        public string ExamenFisicoPediatriaObservaciones { get; set; }
        public string ExamenFisicoPediatriaGlucosa { get; set; }
        public string ExamenFisicoPediatriaGlasgow { get; set; }
        public string ExamenFisicoPediatriaTensionArterial { get; set; }
        public string ExamenFisicoPediatriaDerecho { get; set; }
        public string ExamenFisicoPediatriaIzquierdo { get; set; }
        public string ExamenFisicoPediatricoPesoTalla { get; set; }
        public string ExamenFisicoPediatricoPresionArterial { get; set; }
        public string ExamenFisicoPediatricoPesoEdad { get; set; }
        public string ExamenFisicoPediatricoTallaEdad { get; set; }

        #endregion




        //Area terapeutica
        public string TerapeuticoDatosGenerales { get; set; }
        public string TerapeuticoActividadesDiarias { get; set; }
        public string TerapeuticoConQuienVive { get; set; }
        public string TerapeuticoHabitosAlimenticios { get; set; }
        public string TerapeuticoEjercicio { get; set; }
        public string TerapeuticoFinesSemana { get; set; }
        public string TerapeuticoHistoriaMedica { get; set; }
        public string TerapeuticoHistoriaPeso { get; set; }
        public List<HistoricoObservacionesTerapeuticoConsultaViewModel> TerapeuticoHistoricoObservaciones { get; set; }
        public string TerapeuticoObservaciones { get; set; }

        #region HOSPITALIZACION

        public bool Hospitalizada { get; set; }
        public int? HospitalizacionId { get; set; }
        public string HospitalizacionNumeroHabitacion { get; set; }

        #endregion

        #region OFTALMOLOGIA

        // Identificación y control
        public long? Oft_Id { get; set; }                    // Mapea a "Id" (BIGSERIAL)
        public int? Oft_ConsultaId { get; set; }

        public int? Oft_PacienteId { get; set; }

        // Motivo / Antecedentes
        public string Oft_HistoriaEnfermedadActual { get; set; }
        public string Oft_PacienteMedicos { get; set; }
        public string Oft_PacienteQuirurgicos { get; set; }
        public string Oft_PacienteTraumaticos { get; set; }
        public string Oft_PacienteAlergias { get; set; }
        public string Oft_PacienteFamiliares { get; set; }

        // AV sin corrección
        public string Oft_AgudezaSC_Test { get; set; }       // ENUM en BD -> string en VM
        public string Oft_AgudezaSC_OD { get; set; }         // ENUM en BD -> string en VM
        public string Oft_AgudezaSC_OS { get; set; }         // ENUM en BD -> string en VM

        // Sensibilidad de contraste
        public string Oft_Contraste_OD { get; set; }
        public string Oft_Contraste_OS { get; set; }

        // AV cerca sin corrección
        public string Oft_AVCerca_OD { get; set; }           // ENUM en BD -> string en VM
        public string Oft_AVCerca_OS { get; set; }           // ENUM en BD -> string en VM

        // Tests especiales
        public string Oft_TestIshihara_OD { get; set; }
        public string Oft_TestIshihara_OS { get; set; }
        public string Oft_TestEstereopsis_OD { get; set; }
        public string Oft_TestEstereopsis_OS { get; set; }

        // Óptica - Lensometría (Histórico)
        public decimal? Oft_Lensometria_OD_Esfera { get; set; }
        public decimal? Oft_Lensometria_OD_Cilindro { get; set; }
        public int? Oft_Lensometria_OD_Eje { get; set; }         // 0-180
        public string Oft_Lensometria_OD_Agudeza { get; set; }
        public decimal? Oft_Lensometria_OS_Esfera { get; set; }
        public decimal? Oft_Lensometria_OS_Cilindro { get; set; }
        public int? Oft_Lensometria_OS_Eje { get; set; }         // 0-180
        public string Oft_Lensometria_OS_Agudeza { get; set; }

        // Óptica - Final (RX final)
        public decimal? Oft_Final_OD_Esfera { get; set; }
        public decimal? Oft_Final_OD_Cilindro { get; set; }
        public int? Oft_Final_OD_Eje { get; set; }               // 0-180
        public string Oft_Final_OD_Agudeza { get; set; }
        public decimal? Oft_Final_OS_Esfera { get; set; }
        public decimal? Oft_Final_OS_Cilindro { get; set; }
        public int? Oft_Final_OS_Eje { get; set; }               // 0-180
        public string Oft_Final_OS_Agudeza { get; set; }
        public decimal? Oft_Final_Adicion { get; set; }
        public decimal? Oft_Final_DIP_mm { get; set; }

        // Óptica - Retinoscopía
        public decimal? Oft_Retino_OD_Esfera { get; set; }
        public decimal? Oft_Retino_OD_Cilindro { get; set; }
        public int? Oft_Retino_OD_Eje { get; set; }              // 0-180
        public decimal? Oft_Retino_OS_Esfera { get; set; }
        public decimal? Oft_Retino_OS_Cilindro { get; set; }
        public int? Oft_Retino_OS_Eje { get; set; }              // 0-180

        // Tipo de lente / Material
        public string Oft_TipoLente { get; set; }                    // ENUM en BD -> string en VM
        public string Oft_LenteMaterialTratamiento { get; set; }

        // Inspección / LH / Oftalmoscopía (Segmento anterior y pruebas)
        public string Oft_Inspeccion_MovExtraoculares_OD { get; set; }  // ENUM en BD -> string
        public string Oft_Inspeccion_MovExtraoculares_OS { get; set; }
        public string Oft_Inspeccion_Cejas_OD { get; set; }
        public string Oft_Inspeccion_Cejas_OS { get; set; }
        public string Oft_Inspeccion_ParpadosPestanas_OD { get; set; }
        public string Oft_Inspeccion_ParpadosPestanas_OS { get; set; }
        public string Oft_Inspeccion_ViaLagrimal_OD { get; set; }
        public string Oft_Inspeccion_ViaLagrimal_OS { get; set; }

        // Segmento anterior
        public string Oft_Inspeccion_Conjuntiva_OD { get; set; }
        public string Oft_Inspeccion_Conjuntiva_OS { get; set; }
        public string Oft_Inspeccion_CorneaEsclera_OD { get; set; }
        public string Oft_Inspeccion_CorneaEsclera_OS { get; set; }
        public string Oft_Inspeccion_CamaraAnteriorAngulo_OD { get; set; }
        public string Oft_Inspeccion_CamaraAnteriorAngulo_OS { get; set; }
        public string Oft_Inspeccion_IrisPupila_OD { get; set; }
        public string Oft_Inspeccion_IrisPupila_OS { get; set; }
        public string Oft_Inspeccion_Cristalino_OD { get; set; }
        public string Oft_Inspeccion_Cristalino_OS { get; set; }
        public string Oft_Inspeccion_BUT_OD { get; set; }
        public string Oft_Inspeccion_BUT_OS { get; set; }
        public decimal? Oft_Inspeccion_PresionIntraocular_OD { get; set; }   // NUMERIC(4,1)
        public decimal? Oft_Inspeccion_PresionIntraocular_OS { get; set; }   // NUMERIC(4,1)

        // Segmento posterior
        public string Oft_Inspeccion_Vitreo_OD { get; set; }
        public string Oft_Inspeccion_Vitreo_OS { get; set; }
        public string Oft_Inspeccion_NervioOptico_OD { get; set; }
        public string Oft_Inspeccion_NervioOptico_OS { get; set; }
        public string Oft_Inspeccion_Macula_OD { get; set; }
        public string Oft_Inspeccion_Macula_OS { get; set; }
        public string Oft_Inspeccion_Retina_OD { get; set; }
        public string Oft_Inspeccion_Retina_OS { get; set; }

        // Impresión clínica / Tratamiento
        public string Oft_HistoriaClinicaImpresionClinica { get; set; }
        public string Oft_HistoriaClinicaComentario { get; set; }

        #endregion

        #region PODOLOGIA

        // Identificación y control
        public long? Pod_Id { get; set; }               // Mapea a "Id" (BIGSERIAL)
        public int? Pod_ConsultaId { get; set; }       // FK a Consultas(Id)
        public int? Pod_PacienteId { get; set; }       // FK a Pacientes(Id)

        // 1) Antecedentes Médicos
        public string[] Pod_Enfermedades { get; set; } = Array.Empty<string>(); // TEXT[] en BD
        public string Pod_Enfermedades_Otros { get; set; }                     // TEXT
        public string Pod_Medicamentos { get; set; }                           // TEXT
        public string Pod_PresionArterial { get; set; }                        // TEXT ("120/80 mmHg")

        // 2) Examen del Pie
        // Pulsos
        public string Pod_Pulso_Pedio { get; set; }             // "Palpable" | "Débil" | "Ausente"
        public string Pod_Pulso_TibialPosterior { get; set; }
        public string Pod_Pulso_Popliteo { get; set; }

        // Estado general
        public string Pod_TemperaturaPie { get; set; }          // "Fría" | "Normal" | "Caliente"
        public bool? Pod_ProblemasCirculatorios { get; set; }  // BOOLEAN
        public string Pod_EstadoPiel { get; set; }              // "Seca" | "Normal" | "Húmeda"
        public string Pod_ObservacionesExamen { get; set; }     // TEXT

        // 3) Tratamiento Realizado
        public string[] Pod_Procedimientos { get; set; } = Array.Empty<string>(); // TEXT[] en BD
        public string Pod_OtrosProcedimientos { get; set; }     // TEXT
        public string Pod_ObservacionesTratamiento { get; set; }// TEXT

        // 4) Indicaciones y Datos Finales
        public string Pod_Indicaciones { get; set; }            // TEXT
        public decimal? Pod_PesoKg { get; set; }                  // NUMERIC(5,1)
        public decimal? Pod_EstaturaM { get; set; }               // NUMERIC(4,2)
        public DateTime? Pod_FechaAtencion { get; set; }          // DATE
        public string Pod_Profesional { get; set; }             // TEXT

        #endregion

        #region HISTORIA CLÍNICA DE ENFERMERÍA

        // Identificación y control (mapeo a PK/FK)
        public long? Hce_Id { get; set; }              // mapea a Id (BIGSERIAL)
        public int? Hce_ConsultaId { get; set; }       // FK a Consultas(Id)
        public int? Hce_PacienteId { get; set; }       // FK a Pacientes(Id)

        // 1) Tipo de consulta
        public string Hce_TipoConsulta { get; set; }

        // 2) Motivo de consulta
        public string Hce_MotivoConsulta { get; set; }

        // 3) Antecedentes personales y familiares
        public string Hce_AntecedentesPatologicos { get; set; }
        public string Hce_AntecedentesQuirurgicos { get; set; }
        public string Hce_AntecedentesTraumaticos { get; set; }
        public string Hce_Hospitalizaciones { get; set; }
        public string Hce_Alergias { get; set; }
        public string Hce_AntecedentesFamiliares { get; set; }

        // 4) Hábitos personales
        public string Hce_HabitoAlimentacion { get; set; }
        public string Hce_ActividadFisica { get; set; }
        public string Hce_HabitoAlcoholTexto { get; set; }
        public string Hce_HabitoTabacoTexto { get; set; }
        public string Hce_OtrosHabitos { get; set; }

        // 5) Signos vitales y antropometría
        public string Hce_PresionArterialTxt { get; set; }
        public int? Hce_FC { get; set; }
        public int? Hce_FR { get; set; }
        public decimal? Hce_TemperaturaC { get; set; }
        public int? Hce_SPO2 { get; set; }
        public decimal? Hce_PesoKg { get; set; }
        public decimal? Hce_TallaM { get; set; }
        public decimal? Hce_IMC { get; set; }

        // 6) Exploración física por aparatos y sistemas
        public string Hce_CabezaCuello { get; set; }
        public string Hce_ToraxPulmones { get; set; }
        public string Hce_Corazon { get; set; }
        public string Hce_Abdomen { get; set; }
        public string Hce_Extremidades { get; set; }
        public string Hce_SistemaNeurologico { get; set; }
        public string Hce_PielAnexos { get; set; }

        // 7) Valoración de enfermería
        public string Hce_ValConcienciaOrientacion { get; set; }
        public string Hce_ValEstadoNutricional { get; set; }
        public string Hce_ValEliminacion { get; set; }
        public string Hce_ValSuenoDescanso { get; set; }
        public string Hce_ValActividadMovilidad { get; set; }
        public string Hce_ValAutonomia { get; set; }

        // 8) Laboratorios
        public string Hce_Laboratorios { get; set; }

        // 9) Diagnóstico de enfermería
        public string Hce_DiagnosticoEnfermeria { get; set; }

        // 10) Plan de cuidados / Intervenciones
        public string Hce_AccionesRealizadas { get; set; }
        public string Hce_MedicamentosAdministrados { get; set; }
        public string Hce_Tratamiento { get; set; }

        // 11) Seguimiento / Evolución / Cita
        public string Hce_Seguimiento { get; set; }

        #endregion

        #region VALORACIÓN INICIAL DE ENFERMERÍA

        // Identificación y control (mapeo a PK/FK)
        public long? Ve_Id { get; set; }              // mapea a Id (BIGSERIAL)
        public int? Ve_ConsultaId { get; set; }       // FK a Consultas(Id)
        public int? Ve_PacienteId { get; set; }       // FK a Pacientes(Id)

        // 1) Datos de valoración inicial
        public string Ve_Motivo { get; set; }
        public string Ve_DiagnosticoMedico { get; set; }
        public string Ve_Labs { get; set; }

        // 2) ¿Cómo se enteró del servicio? (checkbox múltiple)
        public string[] Ve_Medio { get; set; } = Array.Empty<string>();

        // 3) Oxigenación y Circulación
        public string[] Ve_Resp { get; set; } = Array.Empty<string>();
        public string[] Ve_Circ { get; set; } = Array.Empty<string>();

        // 4) Necesidad de Nutrición
        public string[] Ve_Nutricion { get; set; } = Array.Empty<string>();
        public string Ve_NutricionObs { get; set; }

        // 5) Necesidad de Eliminación
        public string[] Ve_Urinario { get; set; } = Array.Empty<string>();
        public string[] Ve_Intestinal { get; set; } = Array.Empty<string>();

        // 6) Movilización y Estado de Conciencia
        public string[] Ve_Mov { get; set; } = Array.Empty<string>();
        public string[] Ve_Conciencia { get; set; } = Array.Empty<string>();

        // 7) Autocuidado y Reposo
        public string[] Ve_Sueno { get; set; } = Array.Empty<string>();
        public string Ve_Vestirse { get; set; }           // radio
        public string Ve_Higiene { get; set; }            // radio
        public string[] Ve_Piel { get; set; } = Array.Empty<string>();
        public string Ve_PielUbicacion { get; set; }

        // 8) Necesidad de Comunicación
        public string[] Ve_Lenguaje { get; set; } = Array.Empty<string>();
        public string[] Ve_Vision { get; set; } = Array.Empty<string>();
        public string[] Ve_Oido { get; set; } = Array.Empty<string>();

        // 9) Seguridad y Factores Psicosociales
        public string[] Ve_Seg { get; set; } = Array.Empty<string>();
        public string Ve_Religiosos { get; set; }          // "si"/"no"
        public string Ve_CreenciasObservaciones { get; set; }
        public string Ve_ConoceMotivo { get; set; }        // "si"/"no"
        public string Ve_NecesitaInfo { get; set; }        // "si"/"no"

        // 10) Medicación actual y plan terapéutico
        public string Ve_MedicacionActual { get; set; }
        public string Ve_PlanTerapeutico { get; set; }

        #endregion

        #region SUEROTERAPIA

        // Identificación y control (mapeo a PK/FK de ConsultasSueroterapia)
        public long? Suero_Id { get; set; }             // mapea a Id (BIGSERIAL)
        public int? Suero_ConsultaId { get; set; }     // FK a Consultas(Id)
        public int? Suero_PacienteId { get; set; }     // FK a Pacientes(Id)

        // 1) Datos de valoración inicial
        public string Suero_Motivo { get; set; }                 // = Motivo
        public string Suero_DiagnosticoMedico { get; set; }      // = DiagnosticoMedico
        public string Suero_Labs { get; set; }                   // = Labs

        // 2) ¿Cómo se enteró del servicio? (checkbox múltiple)
        public string[] Suero_Medio { get; set; } = Array.Empty<string>();   // = Medio (text[])

        // 3) Oxigenación y Circulación (checkbox múltiple)
        public string[] Suero_Resp { get; set; } = Array.Empty<string>();    // = Resp (text[])
        public string[] Suero_Circ { get; set; } = Array.Empty<string>();    // = Circ (text[])

        // 4) Necesidad de Nutrición
        public string[] Suero_Nutricion { get; set; } = Array.Empty<string>(); // = Nutricion (text[])
        public string Suero_NutricionObs { get; set; }                        // = NutricionObs

        // 5) Plan terapéutico
        public string Suero_PlanTerapeutico { get; set; }          // = PlanTerapeutico

        #endregion

        //Información médica
        public string MedicaUsaLentesContacto { get; set; }
        public string MedicaUsaLentesContactoDescripcion { get; set; }
        public string MedicaArticulacionesArtificiales { get; set; }
        public DateTime? MedicaArticulacionesArtificialesFecha { get; set; }
        public string MedicaArticulacionesArtificialesComplicaciones { get; set; }
        public string MedicaTomaAlendronato { get; set; }
        public DateTime? MedicaTomaAlendronatoFecha { get; set; }
        public string MedicaTratamientoDolorHuesos { get; set; }
        public DateTime? MedicaTratamientoDolorHuesosFechaInicio { get; set; }
        public string MedicaTratamientoDolorHuesosDescripcionCaso { get; set; }
        public string MedicaSustanciasReguladorasDrogas { get; set; }
        public DateTime? MedicaSustanciasReguladorasDrogasFecha { get; set; }
        public string MedicaUsaTabaco { get; set; }
        public string MedicaBebidasAlcoholicas { get; set; }
        public string MedicaBebidasAlcoholicasDescripcion { get; set; }


        //Información dental
        public string DentalSangradoCepillar { get; set; }
        public string DentalDolorFrio { get; set; }
        public string DentalDolorPresionar { get; set; }
        public string DentalObjetosAtorados { get; set; }
        public string DentalBocaSeca { get; set; }
        public string DentalTratamientoPeriondal { get; set; }
        public string DentalTratamientoOrtodoncia { get; set; }
        public string DentalProblemasTratamientoDental { get; set; }
        public string DentalProblemasTratamientoDentalDescripcion { get; set; }
        public string DentalFluoradaAguaDomicilio { get; set; }
        public string DentalBebeAguaFiltrada { get; set; }
        public string DentalDolorOidos { get; set; }
        public string DentalMolestiaRuidoAlto { get; set; }
        public string DentalMolestiaRuidoAltoDescripcion { get; set; }
        public string DentalBrumismo { get; set; }
        public string DentalLesiones { get; set; }
        public string DentalLesionesDescripcion { get; set; }
        public string DentalDentaduraPlacas { get; set; }
        public string DentalDentaduraPlacasDescripcion { get; set; }
        public string DentalActividadesRecreacion { get; set; }
        public string DentalActividadesRecreacionDescripcion { get; set; }
        public string DentalLesionesCabeza { get; set; }
        public string DentalLesionesCabezaDescripcion { get; set; }
        public string notaOperatoria { get; set; }          //nota nuevo notaoperatoria pestaña notaOperatoria RECONSULTA

        public string EstadoTurno { get; set; } // informacion del estado del turno
        public string NivelPrioridadCita { get; set; }
        public string NumeroTurno { get; set; }
        public int? EmpleadoId { get; set; }
        public int? PrescripcionId { get; set; }

        //Archivos Examenes Consulltas
        public List<ConsultaExamenArchivo> ConsultaExamenArchivos { get; set; }
        public SelectList EmpleadosSelectList { get; set; }
        public int? EspecialidadId { get; set; }
        public SelectList EspecialidadSelectListItem { get; set; }
        public IList<Consulta> Consultas { get; set; }

        public string Cie10Codigo { get; set; }
        public string Cie10Descripcion { get; set; }



        public List<ConsultaExamenAgregadoViewModel> ExamenesAgregados { get; set; }
        public List<ConsultasServicioAgregadoViewModel> ServiciosAgregados { get; set; }

        /// <summary>
        ///Muy pronto se deberia simplificar esta para solo utilizar el campo 'ServiciosAgregados'.
        ///Esta es una repeticion provisional mientras se revisa la hospitalizacion y su historial
        /// </summary>
        public List<ConsultasServicioAgregadoViewModel> ServiciosConsulta { get; set; }
        public List<CaracteristicasDiente> CaracteristicasDientes { get; set; }
        public List<SeguimientoNutricionalConsulta> SeguimientosNutricionales { get; set; }
        public List<VacunaPacienteConsulta> VacunasPaciente { get; set; }
        public List<AntecedenteFamiliarPacienteConsultaViewModel> AntecedentesFamiliaresPaciente { get; set; }
        public List<RangoSaludableConsultaViewModel> RangosSaludables { get; set; }
        public List<RangoSaludableConsultaViewModel> RangosSaludablesHistorico { get; set; }
        //Examenes de laboratorio
        /// <summary>
        /// Pendiente por revision para poder ser normalizado y evitar bucles
        /// e ilegibjilidades de codigo
        /// ANOTACION HECHA POR: JuanP (17-Agosto-2024)
        /// </summary>
        public List<ResultadoExamenLaboratorioConsultaViewModel> ExamenesLaboratorio { get; set; } // Pedniente por revision
        //Prescripciones
        public List<ConsultaPrescripcionViewModel> ElementosPrescripcion { get; set; }
        public List<CitaExamenesViewModel> CitaExamenesViewModel { get; set; }
        public string MedicoNombre { get; set; }
        public string MedicoCelular { get; set; }
        public string ResponsableNit { get; set; }
        public string ResponsableNombre { get; set; }
        public string ResponsableDireccion { get; set; }
        public string ResponsableCorreo { get; set; }
        public string CodigoSeguro { get; set; }

        /// <summary>
        /// Todas las consultas previas con su CIE‑10.
        /// </summary>
        public List<HistorialCie10ViewModel> HistorialCie10 { get; set; } = new();

    }

    public class CitaExamenesViewModel
    {
        public int Id { get; set; }
        public Citas Citas { get; set; }
        public string ExamenCodigo { get; set; }
        public string NombreExamen { get; set; }
        public List<ExamenLabClinicoPregunta> Preguntas { get; set; }
        public ExamenLabClinico ExamenLabClinico { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal? PrecioValor { get; set; }
        public decimal ValorCubiertoSeguro { get; set; }
        public decimal ValorCopago { get; set; }
        public bool Eliminado { get; set; }
    }
    public class ConsultaExamenAgregadoViewModel
    {
        public int? ExamenId { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string ExamenNombre { get; set; }
        public string ExamenCodigo { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorSubtotal { get; set; }
        public decimal ValorCubiertoSeguro { get; set; }
        public decimal ValorCopago { get; set; }
        public int Cantidad { get; set; }
        public bool Pagado { get; set; }
    }
    public class ConsultaServicioExistenteViewModel
    {
        public int Id { get; set; }
        public string NombreServicio { get; set; }
        public string CodigoInterno { get; set; }
        public string ServicioNombreCodigo { get; set; }
    }
    public class CaracteristicasDiente
    {
        public int NumeroDiente { get; set; }
        public bool Percusiones_VerticalMas { get; set; }
        public bool Percusiones_HorizontalMas { get; set; }
        public bool Percusiones_VerticalMenos { get; set; }
        public bool Percusiones_HorizontalMenos { get; set; }
        public bool Dolor_Localizado { get; set; }
        public bool Dolor_Fugaz { get; set; }
        public bool Dolor_Persistente { get; set; }
        public bool Dolor_Referido { get; set; }
        public bool Dolor_Espontaneo { get; set; }
        public bool Estimulo_Frio { get; set; }
        public bool Estimulo_Calor { get; set; }
        public bool Estimulo_DulceAcido { get; set; }
        public bool Estimulo_Masticacion { get; set; }
        public bool Estimulo_Otro { get; set; }
        public bool TermicaFrio_Positiva { get; set; }
        public bool TermicaFrio_Negativa { get; set; }
        public bool TermicaFrio_Localizada { get; set; }
        public bool TermicaFrio_Fugaz { get; set; }
        public bool TermicaFrio_Incrementa { get; set; }
        public bool TermicaFrio_Referida { get; set; }
        public bool TermicaFrio_Irradiado { get; set; }
        public bool TermicaFrio_Persistente { get; set; }
        public bool TermicaFrio_Decrece { get; set; }
        public bool TermicaCalor_Positiva { get; set; }
        public bool TermicaCalor_Negativa { get; set; }
        public bool TermicaCalor_Localizada { get; set; }
        public bool TermicaCalor_Fugaz { get; set; }
        public bool TermicaCalor_Incrementa { get; set; }
        public bool TermicaCalor_Referida { get; set; }
        public bool TermicaCalor_Irradiado { get; set; }
        public bool TermicaCalor_Persistente { get; set; }
        public bool TermicaCalor_Decrece { get; set; }
        public bool Diagnostico_ManchaBlanca { get; set; }
        public bool Diagnostico_Caries { get; set; }
        public bool Diagnostico_Traumatismo { get; set; }
        public bool Diagnostico_Abfraccion { get; set; }
        public bool Diagnostico_Atricion { get; set; }
        public bool Diagnostico_Erosion { get; set; }
        public bool Diagnostico_Restauracion { get; set; }
        public bool Diagnostico_Ajustada { get; set; }
        public bool Diagnostico_Desajustada { get; set; }
        public bool Diagnostico_PulpaSana { get; set; }
        public bool Diagnostico_PulpitisReversible { get; set; }
        public bool Diagnostico_PulpitisIrreversible { get; set; }
        public bool Diagnostico_NecrosisPulpar { get; set; }
    }
    public class SeguimientoNutricionalConsulta
    {
        public int? Id { get; set; }
        public DateTime? Fecha { get; set; }
        public bool? Nuevo { get; set; }
        public decimal? Peso { get; set; }
        public decimal? IMC { get; set; }
        public decimal? PGC { get; set; }
        public decimal? Cuello { get; set; }
        public decimal? Busto { get; set; }
        public decimal? CinturaAbdomen { get; set; }
        public decimal? Cadera { get; set; }
        public decimal? Muslo { get; set; }
        public decimal? Brazo { get; set; }
        public decimal? Munneca { get; set; }
    }
    public class VacunaPacienteConsulta
    {
        public int? VacunaPacienteId { get; set; }
        public int? VacunaId { get; set; }
        public string NombreVacuna { get; set; }
        public bool? Primera { get; set; }
        public DateTime? FechaPrimera { get; set; }
        public bool? Segunda { get; set; }
        public DateTime? FechaSegunda { get; set; }
        public bool? Tercera { get; set; }
        public DateTime? FechaTercera { get; set; }
        public bool? PrimerRefuerzo { get; set; }
        public DateTime? FechaPrimerRefuerzo { get; set; }
        public bool? SegundoRefuerzo { get; set; }
        public DateTime? FechaSegundoRefuerzo { get; set; }
    }
    public class AntecedenteFamiliarPacienteConsultaViewModel
    {
        public int? Id { get; set; }
        public int TipoPatologiaId { get; set; }
        public string TipoPatologia { get; set; }
        public bool TipoPatologiaVerInputDescripcion { get; set; }
        public bool Madre { get; set; }
        public bool AbuelaMaterna { get; set; }
        public bool AbueloMaterno { get; set; }
        public bool OtrosMaterno { get; set; }
        public bool Padre { get; set; }
        public bool AbuelaPaterna { get; set; }
        public bool AbueloPaterno { get; set; }
        public bool Hermanos { get; set; }
        public bool OtrosPaterno { get; set; }
        public string DescripcionOtraPatologia { get; set; }
    }
    public class HistoricoObservacionesTerapeuticoConsultaViewModel
    {
        public DateTime? Fecha { get; set; }
        public string Observaciones { get; set; }
    }
    public class ResultadoExamenLaboratorioConsultaViewModel
    {
        public int? Id { get; set; }
        public DateTime? Fecha { get; set; }
        public string GlucosaPre { get; set; }
        public string GlucosaPos { get; set; }
        public string HemoglobinaGlicosilada { get; set; }
        public string CurvaGlucosa { get; set; }
        public string ColesterolTotal { get; set; }
        public string Trigliceridos { get; set; }
        public string PerfilLipidico { get; set; }
        public string Creatinina { get; set; }
        public string AcidoUrico { get; set; }
        public string Hemoglobina { get; set; }
        public string T3 { get; set; }
        public string T4 { get; set; }
        public string ExamenHeces { get; set; }
        public string ExamenOrina { get; set; }
        public string Otros { get; set; }
        public string UrlResultados { get; set; }
    }
    public class RangoSaludableConsultaViewModel
    {
        public int? Id { get; set; }
        public int? PacienteId { get; set; }
        public DateTime? Fecha { get; set; }
        public string IMC { get; set; }
        public string Peso { get; set; }
        public string PorcentajeGrasaCorporal { get; set; }
    }
    public class ConsultasPrecioServicioViewModel
    {
        public int PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal PrecioValor { get; set; }
        public string PrecioNombreValor { get; set; }
    }
    public class MedicamentoOtro
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Indicaciones { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaPrescripcion { get; set; }
    }

    public class HistorialCie10ViewModel
    {
        public DateTime Fecha { get; set; }
        public string Cie10Codigo { get; set; }
        public string Cie10Descripcion { get; set; }
    }
}