using Database.Shared.Enumeraciones;
using Database.Shared.IRepository;
using Database.Shared.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using farmamest.Service.IService;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using sistema.Models;
using sistema.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sistema.Service
{
    public class PacientesService : IPacientesService
    {
        private readonly IPacientes _pacientesRespository;

        public PacientesService(IPacientes pacientesRespository)
        {
            _pacientesRespository = pacientesRespository;
        }
        public List<Paciente> Buscar(string terminoBusquedaNombre, string terminoBusquedaDpi)
        {
            return _pacientesRespository.Buscar(terminoBusquedaNombre, terminoBusquedaDpi);
        }
        public List<PacienteArchivoVM> GetArchivos(int pacienteId)
        {
            var listaArchivos = new List<PacienteArchivoVM>();
            var archivosBd = _pacientesRespository.GetArchivosConsultas(pacienteId);
            if (archivosBd != null)
            {
                foreach (var archivo in archivosBd)
                {
                    listaArchivos.Add(new PacienteArchivoVM
                    {
                        ArchivoId = archivo.Id,
                        ArchivoNombre = archivo.NombreArchivo,
                        ArchivoFecha = archivo.FechaCarga.ToString(),
                        ArchivoUrl = archivo.UrlArchivo
                    });
                }
            }
            return listaArchivos;
        }
        public List<ConsultasViewModel> GetHistorialConsultas(int pacienteId)
        {
            var historialConsultas = new List<ConsultasViewModel>();
            var consultasBd = _pacientesRespository.GetConsultas(pacienteId);
            if (consultasBd != null)
            {
                foreach (var consulta in consultasBd)
                {
                    var cita = consulta.Citas ?? new Citas();
                    var historiaClinica = consulta.Historia ?? new Historia();
                    var historiaPediatria = consulta.HistoriaPediatria ?? new HistoriaPediatria();
                    var examenFisico = consulta.ExamenFisico ?? new ExamenFisico();
                    var examenFisicoPediatria = consulta.ExamenFisicoPediatria ?? new ExamenFisicoPediatria();
                    var revisionSistemas = consulta.ConsultaRevisionSistemas ?? new ConsultaRevisionSistemas();
                    var revisionSistemasPediatria = consulta.ConsultaRevisionSistemasPediatria ?? new ConsultaRevisionSistemasPediatria();
                    var gineAntPatologicos = consulta.ConsultaAntPatologicosGinecologia ?? new ConsultaAntPatologicosGinecologia();
                    var gineAntNoPatologicos = consulta.ConsultaAntNoPatologicosGinecologia ?? new ConsultaAntNoPatologicosGinecologia();
                    var obsteAntNoPatologicos = consulta.ConsultaAntNoPatologicosObstetricia ?? new ConsultaAntNoPatologicosObstetricia();
                    var obsteAntNoPatologicosFechaUltimaRegla = obsteAntNoPatologicos.FechaUltimaRegla != null
                        ? ((DateTime)obsteAntNoPatologicos.FechaUltimaRegla).ToString("yyyy-MM-dd")
                        : "-";
                    var obsteAntNoPatologicosFechaProbableParto = obsteAntNoPatologicos.FechaProbableParto != null
                        ? ((DateTime)obsteAntNoPatologicos.FechaProbableParto).ToString("yyyy-MM-dd")
                        : "-";
                    var gineExamenFisico = consulta.ConsultaExamenFisicoGinecologia ?? new ConsultaExamenFisicoGinecologia();
                    var servicios = consulta.ConsultasServicios;
                    var historicoConsulta = new ConsultasViewModel
                    {
                        ConsultaId = consulta.Id,
                        CitaMotivo = cita.Motivo ?? "-",
                        Fecha = consulta.FechaYHoraInicioConsulta.ToString(),
                        ServiciosConsulta = new List<ConsultasServicioAgregadoViewModel>(),

                        ConsultaMotivo = consulta.ConsultaMotivo,
                        HistoriaPediatriaConsultaMotivo = consulta.ConsultaMotivoPediatria,

                        #region HISTORIA CLINICA

                        Sintomas = historiaClinica.Sintomas ?? "-",
                        Diagnostico = historiaClinica.Diagnostico ?? "-",
                        HistoriaEnfermedadActual = historiaClinica.HistoriaEnfermedadActual ?? "-",
                        HistoriaProblema = historiaClinica.HistoriaProblema ?? "-",
                        HistoriaClinicaComentario = historiaClinica.Comentario ?? "-",
                        HistoriaClinicaImpresionClinica = historiaClinica.ImpresionClinica ?? "-",

                        #endregion

                        #region HISTORIA CLINICA - PEDIATRIA

                        HistoriaPediatriaSintomas = historiaPediatria.Sintomas ?? "-",
                        HistoriaPediatriaDiagnostico = historiaPediatria.Diagnostico ?? "-",
                        HistoriaPediatriaHistoriaEnfermedadActual = historiaPediatria.HistoriaEnfermedadActual ?? "-",
                        HistoriaPediatriaHistoriaProblema = historiaPediatria.HistoriaProblema ?? "-",
                        HistoriaPediatriaHistoriaClinicaComentario = historiaPediatria.Comentario ?? "-",
                        HistoriaPediatriaHistoriaClinicaImpresionClinica = historiaPediatria.ImpresionClinica ?? "-",

                        #endregion

                        #region EXAMEN FISICO

                        ExamenFisicoEstadoGeneral = examenFisico.EstadoGeneral,
                        ExamenFisicoPeso = examenFisico.Peso,
                        ExamenFisicoTalla = examenFisico.Talla,
                        ExamenFisicoFrecuenciaCardiaca = examenFisico.FrecuenciaCardiaca,
                        ExamenFisicoFrecuenciaRespiratoria = examenFisico.FrecuenciaRespiratoria,
                        ExamenFisicoPresionArterial = examenFisico.PresionArterial,
                        ExamenFisicoTemperatura = examenFisico.Temperatura,
                        ExamenFisicoSaturacionOxigeno = examenFisico.SaturacionDeOxigeno,
                        ExamenFisicoGlasgow = examenFisico.Glasgow,

                        #endregion

                        #region EXAMEN FISICO - PEDIATRIA

                        ExamenFisicoPediatriaEstadoGeneral = examenFisicoPediatria.EstadoGeneral,
                        ExamenFisicoPediatriaPeso = examenFisicoPediatria.Peso,
                        ExamenFisicoPediatriaTalla = examenFisicoPediatria.Talla,
                        ExamenFisicoPediatriaFrecuenciaCardiaca = examenFisicoPediatria.FrecuenciaCardiaca,
                        ExamenFisicoPediatriaFrecuenciaRespiratoria = examenFisicoPediatria.FrecuenciaRespiratoria,
                        ExamenFisicoPediatriaPresionArterial = examenFisicoPediatria.PresionArterial,
                        ExamenFisicoPediatriaTemperatura = examenFisicoPediatria.Temperatura,
                        ExamenFisicoPediatriaSaturacionOxigeno = examenFisicoPediatria.SaturacionDeOxigeno,
                        ExamenFisicoPediatriaGlasgow = examenFisicoPediatria.Glasgow,
                        ExamenFisicoPediatricoPesoTalla = examenFisicoPediatria.PediatricoPesoTalla,
                        ExamenFisicoPediatricoPesoEdad = examenFisicoPediatria.PediatricoPesoEdad,
                        ExamenFisicoPediatricoTallaEdad = examenFisicoPediatria.PediatricoTallaEdad,

                        #endregion

                        #region REVISION POR SISTEMAS

                        RevisionSistemasAparienciaGeneral = revisionSistemas.AparienciaGeneral ?? "-",
                        RevisionSistemasCabeza = revisionSistemas.Cabeza ?? "-",
                        RevisionSistemasOidosBoca = revisionSistemas.OidosBoca ?? "-",
                        RevisionSistemasCuello = revisionSistemas.Cuello ?? "-",
                        RevisionSistemasTorax = revisionSistemas.Torax ?? "-",
                        RevisionSistemasAbdomen = revisionSistemas.Abdomen ?? "-",

                        #endregion

                        #region REVISION POR SISTEMAS - PEDIATRIA

                        RevisionSistemasPediatriaAparienciaGeneral = revisionSistemasPediatria.AparienciaGeneral ?? "-",
                        RevisionSistemasPediatriaCabeza = revisionSistemasPediatria.Cabeza ?? "-",
                        RevisionSistemasPediatriaOidosBoca = revisionSistemasPediatria.OidosBoca ?? "-",
                        RevisionSistemasPediatriaCuello = revisionSistemasPediatria.Cuello ?? "-",
                        RevisionSistemasPediatriaTorax = revisionSistemasPediatria.Torax ?? "-",
                        RevisionSistemasPediatriaAbdomen = revisionSistemasPediatria.Abdomen ?? "-",

                        #endregion

                        #region GINECOLOGIA - ANT PATOLOGICOS

                        GinecologiaAntPatologicosInfecciones = gineAntPatologicos.Infecciones ?? "-",
                        GinecologiaAntPatologicosEts = gineAntPatologicos.Ets ?? "-",
                        GinecologiaAntPatologicosPapanicolau = gineAntPatologicos.Papanicolau ?? "-",
                        GinecologiaAntPatologicosOtros = gineAntPatologicos.Otros ?? "-",

                        #endregion

                        #region GINECOLOGIA - ANT NO PATOLOGICOS

                        GinecologiaAntNoPatologicosMenarquia = gineAntNoPatologicos.Menarquia ?? "-",
                        GinecologiaAntNoPatologicosFechaUltimaRegla = gineAntNoPatologicos.FechaUltimaRegla ?? "-",
                        GinecologiaAntNoPatologicosCicloMenstrual = gineAntNoPatologicos.CicloMenstrual ?? "-",
                        GinecologiaAntNoPatologicosMetodoAnticonceptivo = gineAntNoPatologicos.MetodoAnticonceptivo ?? "-",
                        GinecologiaAntNoPatologicosLactanciaMaterna = gineAntNoPatologicos.LactanciaMaterna ?? "-",
                        GinecologiaAntNoPatologicosGestas = gineAntNoPatologicos.Gestas ?? "-",
                        GinecologiaAntNoPatologicosPartos = gineAntNoPatologicos.Partos ?? "-",
                        GinecologiaAntNoPatologicosAbortos = gineAntNoPatologicos.Abortos ?? "-",
                        GinecologiaAntNoPatologicosCesareas = gineAntNoPatologicos.Cesareas ?? "-",
                        GinecologiaAntNoPatologicosHijosVivos = gineAntNoPatologicos.HijosVivos ?? "-",
                        GinecologiaAntNoPatologicosHijosMuertos = gineAntNoPatologicos.HijosMuertos ?? "-",
                        GinecologiaAntNoPatologicosOtros = gineAntNoPatologicos.Otros ?? "-",

                        #endregion

                        #region OBSTETRICIA - ANT NO PATOLOGICOS

                        ObstetriciaAntNoPatologicosGestas = obsteAntNoPatologicos.Gestas ?? "-",
                        ObstetriciaAntNoPatologicosPartos = obsteAntNoPatologicos.Partos ?? "-",
                        ObstetriciaAntNoPatologicosAbortos = obsteAntNoPatologicos.Abortos ?? "-",
                        ObstetriciaAntNoPatologicosCesareas = obsteAntNoPatologicos.Cesareas ?? "-",
                        ObstetriciaAntNoPatologicosHijosVivos = obsteAntNoPatologicos.HijosVivos ?? "-",
                        ObstetriciaAntNoPatologicosHijosMuertos = obsteAntNoPatologicos.HijosMuertos ?? "-",
                        ObstetriciaAntNoPatologicosFechaProbableParto = obsteAntNoPatologicosFechaProbableParto,
                        ObstetriciaAntNoPatologicosFechaUltimaRegla = obsteAntNoPatologicosFechaUltimaRegla,

                        #endregion

                        #region GINECOLOGIA - EXAMEN FISICO

                        GinecologiaExamenFisicoMamas = gineExamenFisico.Mamas ?? "-",
                        GinecologiaExamenFisicoTactoRectal = gineExamenFisico.TactoRectal ?? "-",
                        GinecologiaExamenFisicoTactoVaginal = gineExamenFisico.TactoVaginal ?? "-",
                        GinecologiaExamenFisicoEspeculoscopia = gineExamenFisico.Especuloscopia ?? "-",
                        GinecologiaExamenFisicoVulvaVagina = gineExamenFisico.VulvaVagina ?? "-",

                        #endregion

                        #region ECOGRAFIA OBSTETRICA

                        EcografiaObstetricaEstado = consulta.EcografiaObstetricaEstado ?? "-",
                        EcografiaObstetricaDorso = consulta.EcografiaObstetricaDorso ?? "-",
                        EcografiaObstetricaFeto = consulta.EcografiaObstetricaFeto ?? "-",
                        EcografiaObstetricaPosicion = consulta.EcografiaObstetricaPosicion ?? "-",
                        EcografiaObstetricaPresentacion = consulta.EcografiaObstetricaPresentacion ?? "-",
                        EcografiaObstetricaSituacion = consulta.EcografiaObstetricaSituacion ?? "-",

                        #endregion

                        #region ECOGRAFIA ENDOCAVITARIA

                        EcografiaEndocavitariaLongitudinal = consulta.EcografiaEndocavitariaLongitudinal ?? "-",
                        EcografiaEndocavitariaComentario = consulta.EcografiaEndocavitariaComentario ?? "-",
                        EcografiaEndocavitariaEndometrio = consulta.EcografiaEndocavitariaEndometrio ?? "-",
                        EcografiaEndocavitariaFondoSaco = consulta.EcografiaEndocavitariaFondoSaco ?? "-",
                        EcografiaEndocavitariaImpresionClinica = consulta.EcografiaEndocavitariaImpresionClinica ?? "-",
                        EcografiaEndocavitariaOvarioDerecho = consulta.EcografiaEndocavitariaOvarioDerecho ?? "-",
                        EcografiaEndocavitariaOvarioIzquierdo = consulta.EcografiaEndocavitariaOvarioIzquierdo ?? "-",
                        EcografiaEndocavitariaTransverso = consulta.EcografiaEndocavitariaTransverso ?? "-",
                        EcografiaEndocavitariaUtero = consulta.EcografiaEndocavitariaUtero ?? "-",

                        #endregion

                        #region OBSTETRICIA - BIOMETRIA

                        ObsteBiometriaAc = consulta.ObsteBiometriaAc ?? "-",
                        ObsteBiometriaComentario = consulta.ObsteBiometriaComentario ?? "-",
                        ObsteBiometriaDbp = consulta.ObsteBiometriaDbp ?? "-",
                        ObsteBiometriaEg = consulta.ObsteBiometriaEg ?? "-",
                        ObsteBiometriaFcf = consulta.ObsteBiometriaFcf ?? "-",
                        ObsteBiometriaFechaParto = consulta.ObsteBiometriaFechaParto ?? "-",
                        ObsteBiometriaGrado = consulta.ObsteBiometriaGrado ?? "-",
                        ObsteBiometriaHc = consulta.ObsteBiometriaHc ?? "-",
                        ObsteBiometriaIla = consulta.ObsteBiometriaIla ?? "-",
                        ObsteBiometriaLf = consulta.ObsteBiometriaLf ?? "-",
                        ObsteBiometriaMalformaciones = consulta.ObsteBiometriaMalformaciones ?? "-",
                        ObsteBiometriaPeso = consulta.ObsteBiometriaPeso ?? "-",
                        ObsteBiometriaPlacenta = consulta.ObsteBiometriaPlacenta ?? "-",
                        ObsteBiometriaRlc = consulta.ObsteBiometriaRlc ?? "-",
                        ObsteBiometriaSexo = consulta.ObsteBiometriaSexo ?? "-",
                        ObsteBiometriaSg = consulta.ObsteBiometriaSg ?? "-",
                        ObsteBiometriaW = consulta.ObsteBiometriaW ?? "-",

                        #endregion

                        #region OBSTETRICIA - EVALUACION OBSTETRICA

                        EvaluacionObstetricaAbdomenObstetrico = consulta.EvaluacionObstetricaAbdomenObstetrico ?? "-",
                        EvaluacionObstetricaActividadUterina = consulta.EvaluacionObstetricaActividadUterina ?? "-",
                        EvaluacionObstetricaAltitud = consulta.EvaluacionObstetricaAltitud ?? "-",
                        EvaluacionObstetricaAu = consulta.EvaluacionObstetricaAu ?? "-",
                        EvaluacionObstetricaBPorciento = consulta.EvaluacionObstetricaBPorciento ?? "-",
                        EvaluacionObstetricaCms = consulta.EvaluacionObstetricaCms ?? "-",
                        EvaluacionObstetricaConsistencia = consulta.EvaluacionObstetricaConsistencia ?? "-",
                        EvaluacionObstetricaD = consulta.EvaluacionObstetricaD ?? "-",
                        EvaluacionObstetricaFcf = consulta.EvaluacionObstetricaFcf ?? "-",
                        EvaluacionObstetricaLiquidoAmniotico = consulta.EvaluacionObstetricaLiquidoAmniotico ?? "-",
                        EvaluacionObstetricaLiquidoAmnioticoEspecifique = consulta.EvaluacionObstetricaLiquidoAmnioticoEspecifique ?? "-",
                        EvaluacionObstetricaMembranasOvulares = consulta.EvaluacionObstetricaMembranasOvulares ?? "-",
                        EvaluacionObstetricaMovimientoFetalEspecifique = consulta.EvaluacionObstetricaMovimientoFetalEspecifique ?? "-",
                        EvaluacionObstetricaMovimientoFetalPercetible = consulta.EvaluacionObstetricaMovimientoFetalPercetible ?? "-",
                        EvaluacionObstetricaOtras = consulta.EvaluacionObstetricaOtras ?? "-",
                        EvaluacionObstetricaPelvis = consulta.EvaluacionObstetricaPelvis ?? "-",
                        EvaluacionObstetricaPosicionCervix = consulta.EvaluacionObstetricaPosicionCervix ?? "-",
                        EvaluacionObstetricaPresentacionLeopold = consulta.EvaluacionObstetricaPresentacionLeopold ?? "-",
                        EvaluacionObstetricaTactoVaginal = consulta.EvaluacionObstetricaTactoVaginal ?? "-",
                        EvaluacionObstetricaUteroGravio = consulta.EvaluacionObstetricaUteroGravio ?? "-",

                        #endregion

                        #region CIE-10

                        Cie10Codigo = consulta.Cie10Codigo ?? "-",

                        #endregion
                    };
                    if (servicios != null)
                    {
                        foreach (var servicioConsulta in servicios)
                        {
                            var servicio = servicioConsulta.Servicio ?? new Servicio();
                            historicoConsulta.ServiciosConsulta.Add(new ConsultasServicioAgregadoViewModel
                            {
                                NombreServicio = servicio.NombreServicio
                            });
                        }
                    }
                    historialConsultas.Add(historicoConsulta);
                }
            }
            return historialConsultas;
        }
        public List<PacienteHistoricoHospitalizacionVM> GetHistorialHospitalizaciones(int pacienteId)
        {
            var historial = new List<PacienteHistoricoHospitalizacionVM>();

            var hospitalizaciones = _pacientesRespository.GetHospitalizaciones(pacienteId);
            if (hospitalizaciones != null)
            {
                foreach (var hospitalizacion in hospitalizaciones)
                {
                    var paciente = hospitalizacion.Paciente ?? new Paciente();
                    var habitacion = hospitalizacion.Habitacion ?? new Habitacion();
                    historial.Add(new PacienteHistoricoHospitalizacionVM
                    {
                        Id = hospitalizacion.Id,
                        PacienteNombre = paciente.Nombre,
                        HospitalizacionFechaInicio = hospitalizacion.FechaInicio.ToString(),
                        HospitalizacionFechaFin = hospitalizacion.FechaFin.ToString(),
                        HabitacionNumeroNombre = habitacion.NombreNumeroHabitacion
                    });
                }
            }


            return historial;
        }
        public List<PacientesBaseViewModel> GetPacientesExistentes()
        {
            var listaPacientes = new List<PacientesBaseViewModel>();
            var pacientesBd = _pacientesRespository.GetList();
            if (pacientesBd.Any())
            {
                foreach (var paciente in pacientesBd)
                {
                    listaPacientes.Add(new PacientesBaseViewModel
                    {
                        PacienteId = paciente.Id,
                        Nombre = paciente.Nombre,
                        Telefono = paciente.Telefono,
                        Nit = paciente.Nit,
                        Direccion = paciente.Direccion
                    });
                }
            }
            return listaPacientes;
        }
        public PacientesBaseViewModel ValidarExistenciaPaciente(PacientesBaseViewModel model)
        {
            if (model.PacienteId == 0)
            {
                //Se registra un nuevo paciente
                var paciente = _pacientesRespository.Add(new Paciente
                {
                    Nombre = model.Nombre,
                    Direccion = model.Direccion,
                    Nit = model.Nit,
                    EstadoPacienteId = (int)EstadoPacienteEnum.Activo,
                    TipoPacienteId = (int)TipoPacienteEnum.Nuevo
                });
                model.PacienteId = paciente.Id;
            }
            else
            {
                var paciente = _pacientesRespository.Get(model.PacienteId, false);
                if (paciente != null)
                {
                    //Modificacion del paciente
                    paciente.Nombre = model.Nombre ?? paciente.Nombre;
                    paciente.Direccion = model.Direccion ?? paciente.Direccion;
                    paciente.Nit = model.Nit ?? paciente.Nit;
                    _pacientesRespository.Update(paciente);

                    //Envio del ID de paciente al model
                    model.PacienteId = paciente.Id;
                }
            }
            return model;
        }
    }
}
