using System.Xml.Schema;
using System.Net.Cache;
using System.Net.WebSockets;
using System.Linq.Expressions;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;
using System.Reflection.Emit;
using System.Reflection;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Database.Shared;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using sistema.Models;
using Database.Shared.IRepository;
using Database.Shared.Data;
using Database.Shared.Models;
using System.Web;
using System.Net.Http;
using sistema.Json;
using Microsoft.AspNetCore.Authorization;
using static sistema.Models.PacientesBaseViewModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Database.Shared.Enumeraciones;
using farmamest.Models;
using static farmamest.Models.ConsentimientoHospiVM;

namespace sistema.Controllers
{
    [Authorize]
    public class PacientesController : Controller
    {
        private readonly IPacientes _pacientesRepository = null;
        private readonly ICuentasPorCobrar _cuentasPorCobrarRepository = null;
        private readonly IAlergiaRaraPacientes _alergiaRaraPacientesRepository = null;
        private readonly IConsultas _consultasRepository = null;
        private readonly IHabitacion _habitacionRepository = null;
        private readonly ICitas _citasRepository = null;
        private readonly Context _context;


        public PacientesController(
            IPacientes pacientesRepository,
            ICuentasPorCobrar cuentasPorCobrarRepository,
            IAlergiaRaraPacientes alergiasRarasRepository,
            IConsultas consultasRepository,
            IHabitacion habitacionRepository,
            ICitas citasRepository,
            Context context
        )
        {
            _pacientesRepository = pacientesRepository;
            _cuentasPorCobrarRepository = cuentasPorCobrarRepository;
            _alergiaRaraPacientesRepository = alergiasRarasRepository;
            _consultasRepository = consultasRepository;
            _habitacionRepository = habitacionRepository;
            _citasRepository = citasRepository;
            _context = context;
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult Lista()
        {
            var lista = _pacientesRepository.GetList();

            return View(lista);
        }
        public IActionResult Nuevo()
        {
            var model = new PacientesBaseViewModel();
            model.FechaRegistro = DateTime.Today;
            model.RegistrarCobroInicial = true;
            model.Init(_pacientesRepository);
            return View(model);
        }
        [HttpPost]
        public JsonResult Nuevo(PacientesBaseViewModel model)
        {
            try
            {
                //Verificar existencia
                var existente = _pacientesRepository.GetPacientePorNombre(model.Nombre);
                if (existente != null)
                {
                    return Json(new
                    {
                        Exitoso = false,
                        Mensaje = "Ya existe un paciente con este nombre"
                    });
                }

                //Crear objeto paciente
                var paciente = new Paciente();

                //Datos personales
                paciente.Nombre = model.Nombre;
                paciente.Telefono = model.Telefono;
                paciente.Celular = model.Celular;
                paciente.Nit = model.Nit;
                paciente.Direccion = model.Direccion;
                paciente.Alias = model.Alias;
                paciente.no_IGGS = model.no_IGGS;
                paciente.esta_Afiliado = model.esta_Afiliado;
                paciente.FechaNacimiento = model.FechaNacimiento;
                paciente.Edad = model.Edad;
                paciente.Eliminado = false;
                paciente.SexoId = model.SexoId;
                paciente.SeguroEpssId = model.SeguroEpssId;
                paciente.CodigoEPS = model.CodigoEPS;
                paciente.Religion = model.Religion;
                paciente.Ocupacion = model.Ocupacion;
                paciente.EstadoCivil = model.EstadoCivil;
                paciente.ContactoEmergencia = model.ContactoEmergencia;
                paciente.NumeroContactoEmergencia = model.NumeroContactoEmergencia;
                paciente.NombreContactoEmergencia = model.NombreContactoEmergencia;
                paciente.Nacionalidad = model.Nacionalidad;
                paciente.PaisResidencia = model.PaisResidencia;
                paciente.DepartamentoResidencia = model.DepartamentoResidencia;
                paciente.MunicipioResidencia = model.MunicipioResidencia;
                paciente.TipoDeSangre = model.TipoDeSangre;
                paciente.Email = model.Email;
                paciente.Contrasennia = model.Contrasennia;
                paciente.Dpi = model.Dpi;
                paciente.TipoPacienteId = model.TipoPacienteId ?? (int)TipoPacienteEnum.Nuevo;
                paciente.EstadoPacienteId = (int)EstadoPacienteEnum.Activo;
                paciente.FechaRetomaServicio = model.TipoPacienteId == (int)TipoPacienteEnum.Retomante ? model.FechaRegistro : null;
                paciente.ModalidadAtencion = model.ModalidadAtencion;
                paciente.Peso = model.Peso;
                paciente.NombreEncargado = model.NombreEncargado;
                paciente.DPIEncargado = model.DPIEncargado;





                //Información de nacimiento
                paciente.PesoAlNacer = model.PesoAlNacer;
                paciente.Talla = model.Talla;
                paciente.CircunferenciaCefalica = model.CircunferenciaCefalica;
                paciente.TipoParto = model.TipoParto;

                paciente.TieneMembresia = false;
                paciente.FechaRegistro = model.FechaRegistro;

                //Antecedentes personales y patologicos observaciones
                paciente.AntecedentesPersonalesObservaciones = model.AntecedentesPersonalesObservaciones;
                paciente.AntecedentesPersonalesOtros = model.AntecedentesPersonalesOtros;

                //Alergias
                paciente.AlergiaAnestesiaLocal = model.AlergiaAnestesiaLocal;
                paciente.AlergiaAspirina = model.AlergiaAspirina;
                paciente.AlergiaPenicilina = model.AlergiaPenicilina;
                paciente.AlergiaBarbiturios = model.AlergiaBarbiturios;
                paciente.AlergiaSulfas = model.AlergiaSulfas;
                paciente.AlergiaCodeina = model.AlergiaCodeina;
                paciente.AlergiaMetales = model.AlergiaMetales;
                paciente.AlergiaLatex = model.AlergiaLatex;
                paciente.AlergiaYodo = model.AlergiaYodo;
                paciente.AlergiaPolen = model.AlergiaPolen;
                paciente.AlergiaAnimales = model.AlergiaAnimales;
                paciente.AlergiaAlimentos = model.AlergiaAlimentos;
                paciente.AlergiaOtros = model.AlergiaOtros;
                paciente.AlergiaOtrosDescripcion = model.AlergiaOtrosDescripcion;

                //Informacion medica
                paciente.MedicaUsaLentesContacto = model.MedicaUsaLentesContacto;
                paciente.MedicaUsaLentesContactoDescripcion = model.MedicaUsaLentesContactoDescripcion;
                paciente.MedicaArticulacionesArtificiales = model.MedicaArticulacionesArtificiales;
                paciente.MedicaArticulacionesArtificialesFecha = model.MedicaArticulacionesArtificialesFecha;
                paciente.MedicaArticulacionesArtificialesComplicaciones = model.MedicaArticulacionesArtificialesComplicaciones;
                paciente.MedicaTomaAlendronato = model.MedicaTomaAlendronato;
                paciente.MedicaTomaAlendronatoFecha = model.MedicaTomaAlendronatoFecha;
                paciente.MedicaTratamientoDolorHuesos = model.MedicaTratamientoDolorHuesos;
                paciente.MedicaTratamientoDolorHuesosFechaInicio = model.MedicaTratamientoDolorHuesosFechaInicio;
                paciente.MedicaTratamientoDolorHuesosDescripcionCaso = model.MedicaTratamientoDolorHuesosDescripcionCaso;
                paciente.MedicaSustanciasReguladorasDrogas = model.MedicaSustanciasReguladorasDrogas;
                paciente.MedicaSustanciasReguladorasDrogasFecha = model.MedicaSustanciasReguladorasDrogasFecha;
                paciente.MedicaUsaTabaco = model.MedicaUsaTabaco;
                paciente.MedicaBebidasAlcoholicas = model.MedicaBebidasAlcoholicas;
                paciente.MedicaBebidasAlcoholicasDescripcion = model.MedicaBebidasAlcoholicasDescripcion;

                //Información dental
                paciente.DentalSangradoCepillar = model.DentalSangradoCepillar;
                paciente.DentalDolorFrio = model.DentalDolorFrio;
                paciente.DentalDolorPresionar = model.DentalDolorPresionar;
                paciente.DentalObjetosAtorados = model.DentalObjetosAtorados;
                paciente.DentalBocaSeca = model.DentalBocaSeca;
                paciente.DentalTratamientoPeriondal = model.DentalTratamientoPeriondal;
                paciente.DentalTratamientoOrtodoncia = model.DentalTratamientoOrtodoncia;
                paciente.DentalProblemasTratamientoDental = model.DentalProblemasTratamientoDental;
                paciente.DentalProblemasTratamientoDentalDescripcion = model.DentalProblemasTratamientoDentalDescripcion;
                paciente.DentalFluoradaAguaDomicilio = model.DentalFluoradaAguaDomicilio;
                paciente.DentalBebeAguaFiltrada = model.DentalBebeAguaFiltrada;
                paciente.DentalDolorOidos = model.DentalDolorOidos;
                paciente.DentalMolestiaRuidoAlto = model.DentalMolestiaRuidoAlto;
                paciente.DentalMolestiaRuidoAltoDescripcion = model.DentalMolestiaRuidoAltoDescripcion;
                paciente.DentalBrumismo = model.DentalBrumismo;
                paciente.DentalLesiones = model.DentalLesiones;
                paciente.DentalLesionesDescripcion = model.DentalLesionesDescripcion;
                paciente.DentalDentaduraPlacas = model.DentalDentaduraPlacas;
                paciente.DentalDentaduraPlacasDescripcion = model.DentalDentaduraPlacasDescripcion;
                paciente.DentalActividadesRecreacion = model.DentalActividadesRecreacion;
                paciente.DentalActividadesRecreacionDescripcion = model.DentalActividadesRecreacionDescripcion;
                paciente.DentalLesionesCabeza = model.DentalLesionesCabeza;
                paciente.DentalLesionesCabezaDescripcion = model.DentalLesionesCabezaDescripcion;

                //Análisis facial
                paciente.FacialPatron = model.FacialPatron;
                paciente.FacialPatronObservaciones = model.FacialPatronObservaciones;
                paciente.FacialPerfil = model.FacialPerfil;
                paciente.FacialPerfilObservaciones = model.FacialPerfilObservaciones;
                paciente.FacialAsimetria = model.FacialAsimetria;
                paciente.FacialAsimetriaObservaciones = model.FacialAsimetriaObservaciones;
                paciente.FacialAlturaFacialEquilibrada = model.FacialAlturaFacialEquilibrada;
                paciente.FacialAlturaFacialEquilibradaObservaciones = model.FacialAlturaFacialEquilibradaObservaciones;
                paciente.FacialAnchoFacialEquilibrada = model.FacialAnchoFacialEquilibrada;
                paciente.FacialAnchoFacialEquilibradaObservaciones = model.FacialAnchoFacialEquilibradaObservaciones;
                paciente.FacialPerfilMaxilar = model.FacialPerfilMaxilar;
                paciente.FacialPerfilMaxilarObservaciones = model.FacialPerfilMaxilarObservaciones;
                paciente.FacialPerfilMandibular = model.FacialPerfilMandibular;
                paciente.FacialPerfilMandibularObservaciones = model.FacialPerfilMandibularObservaciones;
                paciente.FacialSurcoLabialMenton = model.FacialSurcoLabialMenton;
                paciente.FacialSurcoLabialMentonObservaciones = model.FacialSurcoLabialMentonObservaciones;
                paciente.FacialLabiosReposo = model.FacialLabiosReposo;

                //Análisis funcional
                paciente.FuncionalActividadComisurial = model.FuncionalActividadComisurial;
                paciente.FuncionalActividadLingual = model.FuncionalActividadLingual;
                paciente.FuncionalLabioSuperior = model.FuncionalLabioSuperior;
                paciente.FuncionalLabioInferior = model.FuncionalLabioInferior;
                paciente.FuncionalMasetero = model.FuncionalMasetero;
                paciente.FuncionalMentoniano = model.FuncionalMentoniano;
                paciente.FuncionalRespiracion = model.FuncionalRespiracion;
                paciente.FuncionalDeglucion = model.FuncionalDeglucion;

                ////Patrón facial
                paciente.PatronFacial = model.PatronFacial;
                paciente.CaracteristicaPatronFacial = model.CaracteristicaPatronFacial;

                //MEDICOS
                //Antecedentes
                paciente.AntecedentesMedicos = model.AntecedentesMedicos;
                paciente.AntecedentesQuirurgicos = model.AntecedentesQuirurgicos;
                paciente.AntecedentesTraumaticos = model.AntecedentesTraumaticos;
                paciente.AntecedentesAlergias = model.AntecedentesAlergias;
                paciente.AntecedentesVicios = model.AntecedentesVicios;
                paciente.AntecedentesMedicamentos = model.AntecedentesMedicamentos;


                //Pediatricos
                paciente.NombrePadre = model.NombrePadre;
                paciente.NombreMadre = model.NombreMadre;


                //Información de la madre
                paciente.MadreFechaNacimiento = model.MadreFechaNacimiento;
                paciente.MadreEmbarazos = model.MadreEmbarazos;
                paciente.MadrePartosNormales = model.MadrePartosNormales;
                paciente.MadreCesareas = model.MadreCesareas;
                paciente.MadreAbortos = model.MadreAbortos;
                paciente.MadreHijosMuertos = model.MadreHijosMuertos;
                paciente.MadreComplicaciones = model.MadreComplicaciones;

                //Historia medica
                paciente.HistoriaMedicoPersonal = model.HistoriaMedicoPersonal;
                paciente.HistoriaTelefonoMedico = model.HistoriaTelefonoMedico;
                paciente.HistoriaEspecialidadMedico = model.HistoriaEspecialidadMedico;
                paciente.HistoriaTratamientoMedico = model.HistoriaTratamientoMedico;
                paciente.HistoriaSangraMuchoCortarse = model.HistoriaSangraMuchoCortarse;
                paciente.HistoriaHospitalizado = model.HistoriaHospitalizado;
                paciente.HistoriaOperado = model.HistoriaOperado;
                paciente.HistoriaAlergiaMedicina = model.HistoriaAlergiaMedicina;
                paciente.HistoriaAlergiaComida = model.HistoriaAlergiaComida;
                paciente.HistoriaAlergiaOtros = model.HistoriaAlergiaOtros;
                paciente.HistoriaProblemaEmocional = model.HistoriaProblemaEmocional;
                paciente.HistoriaObservaciones = model.HistoriaObservaciones;

                paciente.Retirado = false;

                //Datos de IGSS
                paciente.IgssTipoAfiliacion = model.IgssTipoAfiliacion;
                paciente.IgssNumeroAfiliacion = model.IgssNumeroAfiliacion;
                paciente.IgssCantidadDependientes = model.IgssCantidadDependientes;
                paciente.IgssParentescoDependientes = model.IgssParentescoDependientes;

                //Politicas de pago
                paciente.PoliticasPagoAceptaTerminos = model.PoliticasPagoAceptaTerminos;

                //Datos de pago
                paciente.NumeroTarjetaCredito = model.NumeroTarjetaCredito;
                //Datos GINECOLOGICOS ESTUARDO PRUEBA
                paciente.CicloMenstGine = model.CicloMenstGine;
                paciente.ETSGine = model.ETSGine;
                paciente.VIHGine = model.VIHGine;
                paciente.GrupoFactorGine = model.GrupoFactorGine;
                paciente.TorchGine = model.TorchGine;
                paciente.InicioVidaSexualGine = model.InicioVidaSexualGine;
                paciente.ParejasSexGine = model.ParejasSexGine;
                paciente.ObesidadGine = model.ObesidadGine;
                paciente.DesnutricionGine = model.DesnutricionGine;
                paciente.QGine = model.QGine;
                paciente.PGine = model.PGine;
                paciente.ABGine = model.ABGine;
                paciente.CGine = model.CGine;
                paciente.FURGine = model.FURGine;
                paciente.MuerteNeoGine = model.MuerteNeoGine;
                paciente.FPPGine = model.FPPGine;
                paciente.HVGine = model.HVGine;
                paciente.MuerteGine = model.MuerteGine;
                paciente.ControlPrenatalGine = model.ControlPrenatalGine;
                paciente.ComadronaGine = model.ComadronaGine;
                paciente.NoControlesGine = model.NoControlesGine;

                //Datos Mama Ginecologico
                paciente.AbdomenObstetricoGine = model.AbdomenObstetricoGine;
                paciente.UteroGravioGine = model.UteroGravioGine;
                paciente.FCBGine = model.FCBGine;
                paciente.AUGine = model.AUGine;
                paciente.PresentacionLeopoldGine = model.PresentacionLeopoldGine;
                paciente.OtrasGine = model.OtrasGine;
                paciente.ActividadUterinaGine = model.ActividadUterinaGine;
                paciente.MovimientoFetalPercetibleGine = model.MovimientoFetalPercetibleGine;
                paciente.EspecifiqueGine = model.EspecifiqueGine;
                paciente.TactoVaginalGine = model.TactoVaginalGine;
                paciente.DGine = model.DGine;
                paciente.CMSGine = model.CMSGine;
                paciente.BPorcientoGine = model.BPorcientoGine;
                paciente.AltiutudGine = model.AltiutudGine;
                paciente.VariedadPosicionGine = model.VariedadPosicionGine;
                paciente.MembranasOvularesGine = model.MembranasOvularesGine;
                paciente.LiquidoAmnioticoGine = model.LiquidoAmnioticoGine;
                paciente.Especifique2Gine = model.Especifique2Gine;
                paciente.PelvisGine = model.PelvisGine;


                //Cuentas por cobrar
                if (model.RegistrarCobroInicial)
                {
                    var fechaRegistro = DateTime.Today;
                    model.FechaRegistro = fechaRegistro;
                    var cuentaPorCobrar = new CuentaPorCobrar();
                    cuentaPorCobrar.FechaLimitePago = fechaRegistro.AddMonths(1);
                    cuentaPorCobrar.Valor = 1650;
                    cuentaPorCobrar.Pagada = false;
                    cuentaPorCobrar.Eliminada = false;
                    paciente.CuentasPorCobrar.Add(cuentaPorCobrar);
                }


                //Antecedentes y patologias familiares
                paciente.PatologiasPaciente = new List<PatologiaPaciente>();
                if (model.PatologiasPacienteViewModel != null && model.PatologiasPacienteViewModel.Count > 0)
                {
                    foreach (var patologiaModel in model.PatologiasPacienteViewModel)
                    {
                        var patologia = new PatologiaPaciente
                        {
                            PacienteId = paciente.Id,
                            TipoPatologiaId = patologiaModel.TipoPatologiaId,
                            Madre = patologiaModel.Madre,
                            AbuelaMaterna = patologiaModel.AbuelaMaterna,
                            AbueloMaterno = patologiaModel.AbueloMaterno,
                            OtrosMaterno = patologiaModel.OtrosMaterno,
                            Padre = patologiaModel.Padre,
                            AbuelaPaterna = patologiaModel.AbuelaPaterna,
                            AbueloPaterno = patologiaModel.AbueloPaterno,
                            Hermanos = patologiaModel.Hermanos,
                            OtrosPaterno = patologiaModel.OtrosPaterno,
                            DescripcionOtraPatologia = patologiaModel.DescripcionOtraPatologia
                        };
                        paciente.PatologiasPaciente.Add(patologia);
                    }
                }


                //Preguntas de registro
                paciente.PacientesPreguntasRegistro = new List<PacientePreguntaRegistro>();
                if (model.PreguntasRegistroPacienteViewModel != null && model.PreguntasRegistroPacienteViewModel.Count > 0)
                {
                    foreach (var preguntaRegistro in model.PreguntasRegistroPacienteViewModel)
                    {
                        var pregunta = new PacientePreguntaRegistro
                        {
                            PreguntaRegistroId = preguntaRegistro.PreguntaId,
                            Respuesta = preguntaRegistro.Respuesta
                        };
                        paciente.PacientesPreguntasRegistro.Add(pregunta);
                    }
                }


                //Vacunas paciente
                paciente.VacunasPaciente = new List<VacunaPaciente>();
                if (model.VacunasPacienteViewModel != null && model.VacunasPacienteViewModel.Count > 0)
                {
                    foreach (var vacunaModel in model.VacunasPacienteViewModel)
                    {
                        var vacuna = new VacunaPaciente
                        {
                            VacunaId = vacunaModel.VacunaId,
                            Primera = vacunaModel.Primera,
                            Segunda = vacunaModel.Segunda,
                            Tercera = vacunaModel.Tercera,
                            PrimerRefuerzo = vacunaModel.PrimerRefuerzo,
                            SegundoRefuerzo = vacunaModel.SegundoRefuerzo,
                            FechaPrimera = vacunaModel.FechaPrimera,
                            FechaSegunda = vacunaModel.FechaSegunda,
                            FechaTercera = vacunaModel.FechaTercera,
                            FechaPrimerRefuerzo = vacunaModel.FechaPrimerRefuerzo,
                            FechaSegundoRefuerzo = vacunaModel.FechaSegundoRefuerzo
                        };
                        paciente.VacunasPaciente.Add(vacuna);
                    }
                }


                //Antecedentes personales
                if (model.AntecedentesPersonalesViewModel != null && model.AntecedentesPersonalesViewModel.Count > 0)
                {
                    foreach (var antecedenteViewModel in model.AntecedentesPersonalesViewModel)
                    {
                        var antecedente = new PacienteAntecedentePersonal
                        {
                            AntecedentePersonalId = antecedenteViewModel.AntecedenteId,
                            PresentoAntecedente = antecedenteViewModel.PresentoAntecedente,
                            FechaAntecedente = antecedenteViewModel.FechaAntecedente
                        };
                        paciente.PacientesAntecedentesPersonales.Add(antecedente);
                    }
                }

                //Beneficiarios EPSS
                if (model.BeneficiariosEpssPacientesViewModel != null && model.BeneficiariosEpssPacientesViewModel.Count > 0)
                {
                    foreach (var beneficiarioViewModel in model.BeneficiariosEpssPacientesViewModel)
                    {
                        var beneficiario = new PersonaSeguro
                        {
                            Name = beneficiarioViewModel.Name,
                            Nit = beneficiarioViewModel.Nit,
                            Tipo = beneficiarioViewModel.Tipo
                        };
                        paciente.PersonasSeguro.Add(beneficiario);
                    }
                }

                //Archivos
                if (model.ArchivosSubidos != null)
                {
                    foreach (var archivo in model.ArchivosSubidos)
                    {
                        paciente.PacienteArchivos.Add(new PacienteArchivo
                        {
                            NombreArchivo = archivo.NombreArchivo,
                            UrlArchivo = archivo.UrlArchivo
                        });
                    }
                }

                _pacientesRepository.Add(paciente);

                TempData["Message"] = "¡El paciente se ha guardado con éxito.!";
                return Json(new { Exitoso = true, PacienteId = paciente.Id });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al registrar paciente. " + ex.Message });
            }
        }

        [HttpPost]
        public string ConsultarAntecedentesPersonales(int? pacienteId)
        {
            var listaAntecedentes = new List<AntecedentePersonalPacienteViewModel>();

            if (pacienteId == null)
            {
                var antecedentes = _pacientesRepository.GetAntecedentesPersonales();
                foreach (var antecedente in antecedentes)
                {
                    var antcedenteVM = new AntecedentePersonalPacienteViewModel
                    {
                        AntecedenteId = antecedente.Id,
                        NombreAntecedente = antecedente.NombreAntecedente
                    };
                    listaAntecedentes.Add(antcedenteVM);
                }
            }
            else
            {
                var antecedentesPaciente = _pacientesRepository.GetAntecedentesPersonalesPaciente((int)pacienteId);
                if (antecedentesPaciente == null || antecedentesPaciente.Count == 0)
                {
                    var antecedentes = _pacientesRepository.GetAntecedentesPersonales();
                    foreach (var antecedente in antecedentes)
                    {
                        var antcedenteVM = new AntecedentePersonalPacienteViewModel
                        {
                            AntecedenteId = antecedente.Id,
                            NombreAntecedente = antecedente.NombreAntecedente
                        };
                        listaAntecedentes.Add(antcedenteVM);
                    }
                }
                else
                {
                    foreach (var antecedente in antecedentesPaciente)
                    {
                        var antecedenteVM = new AntecedentePersonalPacienteViewModel
                        {
                            Id = antecedente.Id,
                            NombreAntecedente = antecedente.AntecedentePersonal.NombreAntecedente,
                            AntecedenteId = antecedente.AntecedentePersonalId,
                            PresentoAntecedente = antecedente.PresentoAntecedente,
                            FechaAntecedente = antecedente.FechaAntecedente
                        };
                        listaAntecedentes.Add(antecedenteVM);
                    }
                }
            }

            return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaAntecedentes });
        }
        [HttpPost]
        public string ConsultarPreguntasRegistro(int? pacienteId)
        {
            var listaPreguntas = new List<PreguntaRegistroPacienteViewModel>();

            if (pacienteId == null)
            {
                var preguntas = _pacientesRepository.GetPreguntasRegistro();
                foreach (var pregunta in preguntas)
                {
                    var preguntaVM = new PreguntaRegistroPacienteViewModel
                    {
                        PreguntaId = pregunta.Id,
                        Pregunta = pregunta.Pregunta
                    };
                    listaPreguntas.Add(preguntaVM);
                }
            }
            else
            {
                var preguntasPaciente = _pacientesRepository.GetPreguntasRegistroPaciente((int)pacienteId);
                if (preguntasPaciente == null || preguntasPaciente.Count == 0)
                {
                    var preguntas = _pacientesRepository.GetPreguntasRegistro();
                    foreach (var pregunta in preguntas)
                    {
                        var preguntaVM = new PreguntaRegistroPacienteViewModel
                        {
                            PreguntaId = pregunta.Id,
                            Pregunta = pregunta.Pregunta
                        };
                        listaPreguntas.Add(preguntaVM);
                    }
                }
                else
                {
                    foreach (var pregunta in preguntasPaciente)
                    {
                        var preguntaVM = new PreguntaRegistroPacienteViewModel
                        {
                            Id = pregunta.Id,
                            PreguntaId = pregunta.PreguntaRegistroId,
                            Pregunta = pregunta.PreguntaRegistro.Pregunta,
                            Respuesta = pregunta.Respuesta
                        };
                        listaPreguntas.Add(preguntaVM);
                    }
                }
            }

            return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaPreguntas });
        }
        [HttpPost]
        public string ConsultarVacunas(int? pacienteId)
        {

            try
            {
                var listaVacunas = new List<VacunaPacienteViewModel>();

                if (pacienteId == null)
                {
                    var vacunas = _pacientesRepository.GetVacunas();
                    foreach (var vacuna in vacunas)
                    {
                        var vacunaPaciente = new VacunaPacienteViewModel
                        {
                            VacunaId = vacuna.Id,
                            NombreVacuna = vacuna.Nombre
                        };
                        listaVacunas.Add(vacunaPaciente);
                    }
                }
                else
                {
                    var vacunasPaciente = _pacientesRepository.GetVacunasPaciente((int)pacienteId);
                    if (vacunasPaciente == null || vacunasPaciente.Count == 0)
                    {
                        var vacunas = _pacientesRepository.GetVacunas();
                        foreach (var vacuna in vacunas)
                        {
                            var vacunaPaciente = new VacunaPacienteViewModel
                            {
                                VacunaId = vacuna.Id
                            };
                            listaVacunas.Add(vacunaPaciente);
                        }
                    }
                    else
                    {
                        foreach (var vacuna in vacunasPaciente)
                        {
                            var vacunaPaciente = new VacunaPacienteViewModel
                            {
                                Id = vacuna.Id,
                                VacunaId = vacuna.VacunaId,
                                NombreVacuna = vacuna.Vacuna.Nombre,
                                Primera = vacuna.Primera,
                                FechaPrimera = vacuna.FechaPrimera,
                                Segunda = vacuna.Segunda,
                                FechaSegunda = vacuna.FechaSegunda,
                                Tercera = vacuna.Tercera,
                                FechaTercera = vacuna.FechaTercera,
                                PrimerRefuerzo = vacuna.PrimerRefuerzo,
                                FechaPrimerRefuerzo = vacuna.FechaPrimerRefuerzo,
                                SegundoRefuerzo = vacuna.SegundoRefuerzo,
                                FechaSegundoRefuerzo = vacuna.FechaSegundoRefuerzo
                            };
                            listaVacunas.Add(vacunaPaciente);
                        }
                    }
                }

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaVacunas });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al consultar vacunas. " + ex.Message });
            }
        }
        //COnsultar las alergias no usuales
        [HttpPost]
        public string ConsultarAlergiasRaras(int? pacienteId)
        {

            try
            {
                var listaAlergias = new List<AlergiaRaraPacienteViewModel>();

                if (pacienteId == null)
                {
                    //var vacunas = _pacientesRepository.GetVacunas();
                    //foreach (var vacuna in vacunas)
                    //{
                    //    var vacunaPaciente = new AlergiaRaraPacienteViewModel
                    //    {
                    //        VacunaId = vacuna.Id,
                    //        NombreVacuna = vacuna.Nombre
                    //    };
                    //    listaVacunas.Add(vacunaPaciente);
                    //}
                }
                else
                {
                    var alergiasPacientes = _pacientesRepository.GetAlergiasPaciente((int)pacienteId);
                    if (alergiasPacientes == null || alergiasPacientes.Count == 0)
                    {
                        //var vacunas = _pacientesRepository.GetVacunas();
                        //foreach (var vacuna in vacunas)
                        //{
                        //    var vacunaPaciente = new AlergiaRaraPacienteViewModel
                        //    {
                        //        VacunaId = vacuna.Id
                        //    };
                        //    listaVacunas.Add(vacunaPaciente);
                        //}
                    }
                    else
                    {
                        foreach (var alergiaPaciente in alergiasPacientes)
                        {
                            var alergiasPaciente = new AlergiaRaraPacienteViewModel
                            {
                                Id = alergiaPaciente.Id,
                                AlergiaRaraId = alergiaPaciente.AlergiaRara.Id,
                                NombreAlergia = alergiaPaciente.AlergiaRara.Nombre,
                                Estado = alergiaPaciente.Estado
                            };

                            listaAlergias.Add(alergiasPaciente);
                        }
                    }
                }

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaAlergias });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al consultar alergias. " + ex.Message });
            }
        }

        public string AgregarAlergiaRara(AgregarAlergiaViewModel alergiaNueva)
        {
            try
            {
                //Crea un objeto en alergiaRara
                var alergiaRara = new AlergiaRara
                {
                    Nombre = alergiaNueva.NombreAlergia,
                    Eliminado = false

                };
                _alergiaRaraPacientesRepository.AddAlergiaRara(alergiaRara);
                var AlergiaAgregada = alergiaRara.Id;

                //Crea un objeto en alergiaRaraPaciente
                var alergiaRaraPaciente = new AlergiaRaraPaciente
                {
                    Estado = alergiaNueva.Estado,
                    PacienteId = alergiaNueva.PacienteId,
                    AlergiaRaraId = AlergiaAgregada

                };
                _alergiaRaraPacientesRepository.AddAlergiasPacientes(alergiaRaraPaciente);
                TempData["Message"] = "la alergia ha sido registrado";


                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al crear examen. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarAntecedentesFamiliares(int? pacienteId)
        {
            try
            {
                var listaAntecedentes = new List<PatologiaPacienteViewModel>();

                if (pacienteId == null)
                {
                    var antecedentes = _pacientesRepository.GetTipoPatologias();
                    foreach (var antecedente in antecedentes)
                    {
                        var antecedentePaciente = new PatologiaPacienteViewModel
                        {
                            TipoPatologiaId = antecedente.Id,
                            TipoPatologia = antecedente.Tipo,
                            TipoPatologiaVerInputDescripcion = antecedente.VerInputDescripcion
                        };
                        listaAntecedentes.Add(antecedentePaciente);
                    }
                }
                else
                {
                    var antecedentesPaciente = _pacientesRepository.GetAntecedentesFamiliaresPaciente((int)pacienteId);
                    if (antecedentesPaciente == null || antecedentesPaciente.Count == 0)
                    {
                        var antecedentes = _pacientesRepository.GetTipoPatologias();
                        foreach (var antecedente in antecedentes)
                        {
                            var antecedentePaciente = new PatologiaPacienteViewModel
                            {
                                TipoPatologiaId = antecedente.Id,
                                TipoPatologia = antecedente.Tipo,
                                TipoPatologiaVerInputDescripcion = antecedente.VerInputDescripcion
                            };
                            listaAntecedentes.Add(antecedentePaciente);
                        }
                    }
                    else
                    {
                        foreach (var antecedente in antecedentesPaciente)
                        {
                            var antecedentePaciente = new PatologiaPacienteViewModel
                            {
                                Id = antecedente.Id,
                                TipoPatologiaId = antecedente.TipoPatologiaId,
                                TipoPatologia = antecedente.TipoPatologia.Tipo,
                                Madre = antecedente.Madre,
                                AbuelaMaterna = antecedente.AbuelaMaterna,
                                AbueloMaterno = antecedente.AbueloMaterno,
                                OtrosMaterno = antecedente.OtrosMaterno,
                                Padre = antecedente.Padre,
                                AbuelaPaterna = antecedente.AbuelaPaterna,
                                AbueloPaterno = antecedente.AbueloPaterno,
                                Hermanos = antecedente.Hermanos,
                                OtrosPaterno = antecedente.OtrosPaterno,
                                DescripcionOtraPatologia = antecedente.DescripcionOtraPatologia,
                                TipoPatologiaVerInputDescripcion = antecedente.TipoPatologia.VerInputDescripcion
                            };
                            listaAntecedentes.Add(antecedentePaciente);
                        }
                    }
                }

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = listaAntecedentes });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al consultar vacunas. " + ex.Message });
            }
        }
        [HttpPost]
        public string ConsultarPersonasSeguro(int pacienteId)
        {
            try
            {
                var personasSeguro = _pacientesRepository.GetPersonasSeguro(pacienteId);
                if (personasSeguro != null || personasSeguro.Count > 0)
                {
                    foreach (var persona in personasSeguro)
                    {
                        var personaSeguroPaciente = new PersonasSeguroPacienteViewModel
                        {
                            Id = persona.Id,
                            PacienteId = persona.PacienteId,
                            Name = persona.Name,
                            Nit = persona.Nit,
                            Tipo = persona.Tipo
                        };
                    }
                }


                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = personasSeguro });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al consultar beneficiarios. " + ex.Message });
            }
        }
        public IActionResult Modificar(int? id)
        {
            if (id == null)
            {

                return BadRequest("request is incorrect");
            }

            var paciente = _pacientesRepository.Get((int)id);

            if (paciente == null)
            {
                return StatusCode(404);
            }

            var model = new PacientesBaseViewModel();
            model.PacienteId = paciente.Id;
            model.Peso = paciente.Peso;
            model.NombreEncargado = paciente.NombreEncargado;
            model.DPIEncargado = paciente.DPIEncargado;
            model.FechaRegistro = paciente.FechaRegistro;
            model.Nombre = paciente.Nombre;
            model.Alias = paciente.Alias;
            model.SexoId = paciente.SexoId;
            model.SeguroEpssId = paciente.SeguroEpssId;
            model.FechaNacimiento = paciente.FechaNacimiento;
            model.Edad = paciente.Edad;
            model.TipoDeSangre = paciente.TipoDeSangre;
            model.Telefono = paciente.Telefono;
            model.Celular = paciente.Celular;
            model.Email = paciente.Email;
            model.Contrasennia = paciente.Contrasennia;
            model.Nit = paciente.Nit;
            model.Dpi = paciente.Dpi;
            model.Direccion = paciente.Direccion;
            model.no_IGGS = paciente.no_IGGS;
            model.esta_Afiliado = paciente.esta_Afiliado;
            model.CodigoEPS = paciente.CodigoEPS;
            model.Religion = paciente.Religion;
            model.Ocupacion = paciente.Ocupacion;
            model.EstadoCivil = paciente.EstadoCivil;
            model.ContactoEmergencia = paciente.ContactoEmergencia;
            model.NumeroContactoEmergencia = paciente.NumeroContactoEmergencia;
            model.NombreContactoEmergencia = paciente.NombreContactoEmergencia;
            model.Nacionalidad = paciente.Nacionalidad;
            model.PaisResidencia = paciente.PaisResidencia;
            model.DepartamentoResidencia = paciente.DepartamentoResidencia;
            model.MunicipioResidencia = paciente.MunicipioResidencia;
            model.PesoAlNacer = paciente.PesoAlNacer;
            model.Talla = paciente.Talla;
            model.CircunferenciaCefalica = paciente.CircunferenciaCefalica;
            model.TipoParto = paciente.TipoParto;
            model.TipoPacienteId = paciente.TipoPacienteId;
            model.ModalidadAtencion = paciente.ModalidadAtencion ?? "presencial";
            model.UrlFirmaRegistro = paciente.UrlFirmaRegistro;
            model.UrlFirmaPoliticas = paciente.UrlFirmaPoliticas;
            model.UrlFirmaConsentimiento = paciente.UrlFirmaConsentimiento;
            model.UrlFirmaTarjetaCredito = paciente.UrlFirmaTarjetaCredito;

            //Antecedentes personales otros
            model.AntecedentesPersonalesObservaciones = paciente.AntecedentesPersonalesObservaciones;
            model.AntecedentesPersonalesOtros = paciente.AntecedentesPersonalesOtros;

            //Alergias
            model.AlergiaAnestesiaLocal = paciente.AlergiaAnestesiaLocal;
            model.AlergiaAspirina = paciente.AlergiaAspirina;
            model.AlergiaPenicilina = paciente.AlergiaPenicilina;
            model.AlergiaBarbiturios = paciente.AlergiaBarbiturios;
            model.AlergiaSulfas = paciente.AlergiaSulfas;
            model.AlergiaCodeina = paciente.AlergiaCodeina;
            model.AlergiaMetales = paciente.AlergiaMetales;
            model.AlergiaLatex = paciente.AlergiaLatex;
            model.AlergiaYodo = paciente.AlergiaYodo;
            model.AlergiaPolen = paciente.AlergiaPolen;
            model.AlergiaAnimales = paciente.AlergiaAnimales;
            model.AlergiaAlimentos = paciente.AlergiaAlimentos;
            model.AlergiaOtros = paciente.AlergiaOtros;
            model.AlergiaOtrosDescripcion = paciente.AlergiaOtrosDescripcion;



            //Información médica
            model.MedicaUsaLentesContacto = paciente.MedicaUsaLentesContacto;
            model.MedicaUsaLentesContactoDescripcion = paciente.MedicaUsaLentesContactoDescripcion;
            model.MedicaArticulacionesArtificiales = paciente.MedicaArticulacionesArtificiales;
            model.MedicaArticulacionesArtificialesFecha = paciente.MedicaArticulacionesArtificialesFecha;
            model.MedicaArticulacionesArtificialesComplicaciones = paciente.MedicaArticulacionesArtificialesComplicaciones;
            model.MedicaTomaAlendronato = paciente.MedicaTomaAlendronato;
            model.MedicaTomaAlendronatoFecha = paciente.MedicaTomaAlendronatoFecha;
            model.MedicaTratamientoDolorHuesos = paciente.MedicaTratamientoDolorHuesos;
            model.MedicaTratamientoDolorHuesosFechaInicio = paciente.MedicaTratamientoDolorHuesosFechaInicio;
            model.MedicaTratamientoDolorHuesosDescripcionCaso = paciente.MedicaTratamientoDolorHuesosDescripcionCaso;
            model.MedicaSustanciasReguladorasDrogas = paciente.MedicaSustanciasReguladorasDrogas;
            model.MedicaSustanciasReguladorasDrogasFecha = paciente.MedicaSustanciasReguladorasDrogasFecha;
            model.MedicaUsaTabaco = paciente.MedicaUsaTabaco;
            model.MedicaBebidasAlcoholicas = paciente.MedicaBebidasAlcoholicas;
            model.MedicaBebidasAlcoholicasDescripcion = paciente.MedicaBebidasAlcoholicasDescripcion;


            //Información dental
            model.DentalSangradoCepillar = paciente.DentalSangradoCepillar;
            model.DentalDolorFrio = paciente.DentalDolorFrio;
            model.DentalDolorPresionar = paciente.DentalDolorPresionar;
            model.DentalObjetosAtorados = paciente.DentalObjetosAtorados;
            model.DentalTratamientoPeriondal = paciente.DentalTratamientoPeriondal;
            model.DentalTratamientoOrtodoncia = paciente.DentalTratamientoOrtodoncia;
            model.DentalProblemasTratamientoDental = paciente.DentalProblemasTratamientoDental;
            model.DentalProblemasTratamientoDentalDescripcion = paciente.DentalProblemasTratamientoDentalDescripcion;
            model.DentalFluoradaAguaDomicilio = paciente.DentalFluoradaAguaDomicilio;
            model.DentalBebeAguaFiltrada = paciente.DentalBebeAguaFiltrada;
            model.DentalDolorOidos = paciente.DentalDolorOidos;
            model.DentalMolestiaRuidoAlto = paciente.DentalMolestiaRuidoAlto;
            model.DentalMolestiaRuidoAltoDescripcion = paciente.DentalMolestiaRuidoAltoDescripcion;
            model.DentalBrumismo = paciente.DentalBrumismo;
            model.DentalLesiones = paciente.DentalLesiones;
            model.DentalLesionesDescripcion = paciente.DentalLesionesDescripcion;
            model.DentalDentaduraPlacas = paciente.DentalDentaduraPlacas;
            model.DentalDentaduraPlacasDescripcion = paciente.DentalDentaduraPlacasDescripcion;
            model.DentalActividadesRecreacion = paciente.DentalActividadesRecreacion;
            model.DentalActividadesRecreacionDescripcion = paciente.DentalActividadesRecreacionDescripcion;
            model.DentalLesionesCabeza = paciente.DentalLesionesCabeza;
            model.DentalLesionesCabezaDescripcion = paciente.MedicaUsaTabaco;


            #region Analisis facial

            model.FacialPatron = paciente.FacialPatron;
            model.FacialPatronObservaciones = paciente.FacialPatronObservaciones;
            model.FacialPerfil = paciente.FacialPerfil;
            model.FacialPerfilObservaciones = paciente.FacialPerfilObservaciones;
            model.FacialAsimetria = paciente.FacialAsimetria;
            model.FacialAsimetriaObservaciones = paciente.FacialAsimetriaObservaciones;
            model.FacialAlturaFacialEquilibrada = paciente.FacialAlturaFacialEquilibrada;
            model.FacialAlturaFacialEquilibradaObservaciones = paciente.FacialAlturaFacialEquilibradaObservaciones;
            model.FacialAnchoFacialEquilibrada = paciente.FacialAnchoFacialEquilibrada;
            model.FacialAnchoFacialEquilibradaObservaciones = paciente.FacialAnchoFacialEquilibradaObservaciones;
            model.FacialPerfilMaxilar = paciente.FacialPerfilMaxilar;
            model.FacialPerfilMaxilarObservaciones = paciente.FacialPerfilMaxilarObservaciones;
            model.FacialPerfilMandibular = paciente.FacialPerfilMandibular;
            model.FacialPerfilMandibularObservaciones = paciente.FacialPerfilMandibularObservaciones;
            model.FacialSurcoLabialMenton = paciente.FacialSurcoLabialMenton;
            model.FacialSurcoLabialMentonObservaciones = paciente.FacialSurcoLabialMentonObservaciones;
            model.FacialLabiosReposo = paciente.FacialLabiosReposo;

            #endregion

            //Ánálisis funcional
            model.FuncionalActividadComisurial = paciente.FuncionalActividadComisurial;
            model.FuncionalActividadLingual = paciente.FuncionalActividadLingual;
            model.FuncionalLabioSuperior = paciente.FuncionalLabioSuperior;
            model.FuncionalLabioInferior = paciente.FuncionalLabioInferior;
            model.FuncionalMasetero = paciente.FuncionalMasetero;
            model.FuncionalMentoniano = paciente.FuncionalMentoniano;
            model.FuncionalRespiracion = paciente.FuncionalRespiracion;
            model.FuncionalDeglucion = paciente.FuncionalDeglucion;

            //Patrón facial
            model.PatronFacial = paciente.PatronFacial;
            model.CaracteristicaPatronFacial = paciente.CaracteristicaPatronFacial;

            //Medico
            model.AntecedentesMedicos = paciente.AntecedentesMedicos;
            model.AntecedentesQuirurgicos = paciente.AntecedentesQuirurgicos;
            model.AntecedentesTraumaticos = paciente.AntecedentesTraumaticos;
            model.AntecedentesAlergias = paciente.AntecedentesAlergias;
            model.AntecedentesVicios = paciente.AntecedentesVicios;
            model.AntecedentesMedicamentos = paciente.AntecedentesMedicamentos;

            //Pediátricos
            model.NombrePadre = paciente.NombrePadre;
            model.NombreMadre = paciente.NombreMadre;
            model.MadreFechaNacimiento = paciente.MadreFechaNacimiento;
            model.MadreEmbarazos = paciente.MadreEmbarazos;
            model.MadrePartosNormales = paciente.MadrePartosNormales;
            model.MadreCesareas = paciente.MadreCesareas;
            model.MadreAbortos = paciente.MadreAbortos;
            model.MadreHijosVivos = paciente.MadreHijosVivos;
            model.MadreHijosMuertos = paciente.MadreHijosMuertos;
            model.MadreComplicaciones = paciente.MadreComplicaciones;

            //Historia
            model.HistoriaMedicoPersonal = paciente.HistoriaMedicoPersonal;
            model.HistoriaObservaciones = paciente.HistoriaObservaciones;
            model.HistoriaProblemaEmocional = paciente.HistoriaProblemaEmocional == null ? false :
                (bool)paciente.HistoriaProblemaEmocional;
            model.HistoriaAlergiaOtros = paciente.HistoriaAlergiaOtros;
            model.HistoriaAlergiaComida = paciente.HistoriaAlergiaComida ?? false;
            model.HistoriaAlergiaMedicina = paciente.HistoriaAlergiaMedicina ?? false;
            model.HistoriaOperado = paciente.HistoriaOperado ?? false;
            model.HistoriaHospitalizado = paciente.HistoriaHospitalizado ?? false;
            model.HistoriaSangraMuchoCortarse = paciente.HistoriaSangraMuchoCortarse ?? false;
            model.HistoriaTratamientoMedico = paciente.HistoriaTratamientoMedico ?? false;
            model.HistoriaEspecialidadMedico = paciente.HistoriaEspecialidadMedico;
            model.HistoriaTelefonoMedico = paciente.HistoriaTelefonoMedico;

            //Datos GINECOLOGICOS ESTUARDO PRUEBA
            model.CicloMenstGine = paciente.CicloMenstGine;
            model.ETSGine = paciente.ETSGine;
            model.VIHGine = paciente.VIHGine;
            model.GrupoFactorGine = paciente.GrupoFactorGine;
            model.TorchGine = paciente.TorchGine;
            model.InicioVidaSexualGine = paciente.InicioVidaSexualGine;
            model.ParejasSexGine = paciente.ParejasSexGine;
            model.ObesidadGine = paciente.ObesidadGine;
            model.DesnutricionGine = paciente.DesnutricionGine;
            model.QGine = paciente.QGine;
            model.PGine = paciente.PGine;
            model.ABGine = paciente.ABGine;
            model.CGine = paciente.CGine;
            model.FURGine = paciente.FURGine;
            model.MuerteNeoGine = paciente.MuerteNeoGine;
            model.FPPGine = paciente.FPPGine;
            model.HVGine = paciente.HVGine;
            model.MuerteGine = paciente.MuerteGine;
            model.ControlPrenatalGine = paciente.ControlPrenatalGine;
            model.ComadronaGine = paciente.ComadronaGine;
            model.NoControlesGine = paciente.NoControlesGine;

            //Datos de la Mamá

            model.AbdomenObstetricoGine = paciente.AbdomenObstetricoGine;
            model.UteroGravioGine = paciente.UteroGravioGine;
            model.FCBGine = paciente.FCBGine;
            model.AUGine = paciente.AUGine;
            model.PresentacionLeopoldGine = paciente.PresentacionLeopoldGine;
            model.OtrasGine = paciente.OtrasGine;
            model.ActividadUterinaGine = paciente.ActividadUterinaGine;
            model.MovimientoFetalPercetibleGine = paciente.MovimientoFetalPercetibleGine;
            model.EspecifiqueGine = paciente.EspecifiqueGine;
            model.TactoVaginalGine = paciente.TactoVaginalGine;
            model.DGine = paciente.DGine;
            model.CMSGine = paciente.CMSGine;
            model.BPorcientoGine = paciente.BPorcientoGine;
            model.AltiutudGine = paciente.AltiutudGine;
            model.VariedadPosicionGine = paciente.VariedadPosicionGine;
            model.MembranasOvularesGine = paciente.MembranasOvularesGine;
            model.LiquidoAmnioticoGine = paciente.LiquidoAmnioticoGine;
            model.Especifique2Gine = paciente.Especifique2Gine;
            model.PelvisGine = paciente.PelvisGine;


            model.Init(_pacientesRepository);

            ViewBag.PacienteId = paciente.Id;
            return View(model);
        }
        [HttpPost]
        public JsonResult Modificar(PacientesBaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var paciente = _pacientesRepository.Get(model.PacienteId);

                    //Datos personales
                    paciente.Nombre = model.Nombre;
                    paciente.Peso = model.Peso;
                    paciente.NombreEncargado = model.NombreEncargado;
                    paciente.DPIEncargado = model.DPIEncargado;
                    paciente.Telefono = model.Telefono;
                    paciente.Celular = model.Celular;
                    paciente.Nit = model.Nit;
                    paciente.Direccion = model.Direccion;
                    paciente.Alias = model.Alias;
                    paciente.no_IGGS = model.no_IGGS;
                    paciente.esta_Afiliado = model.esta_Afiliado;
                    paciente.FechaNacimiento = model.FechaNacimiento;
                    paciente.Edad = model.Edad;
                    paciente.Eliminado = false;
                    paciente.SexoId = model.SexoId;
                    paciente.SeguroEpssId = model.SeguroEpssId;
                    paciente.CodigoEPS = model.CodigoEPS;
                    paciente.Religion = model.Religion;
                    paciente.Ocupacion = model.Ocupacion;
                    paciente.EstadoCivil = model.EstadoCivil;
                    paciente.ContactoEmergencia = model.ContactoEmergencia;
                    paciente.NumeroContactoEmergencia = model.NumeroContactoEmergencia;
                    paciente.NombreContactoEmergencia = model.NombreContactoEmergencia;
                    paciente.Nacionalidad = model.Nacionalidad;
                    paciente.PaisResidencia = model.PaisResidencia;
                    paciente.DepartamentoResidencia = model.DepartamentoResidencia;
                    paciente.MunicipioResidencia = model.MunicipioResidencia;
                    paciente.TipoDeSangre = model.TipoDeSangre;
                    paciente.Email = model.Email;
                    paciente.Contrasennia = model.Contrasennia;
                    paciente.Dpi = model.Dpi;
                    paciente.TipoPacienteId = (int)model.TipoPacienteId;
                    paciente.FechaRetomaServicio = model.TipoPacienteId == (int)TipoPacienteEnum.Retomante ? model.FechaRegistro : null;
                    paciente.ModalidadAtencion = model.ModalidadAtencion;

                    //Información de nacimiento
                    paciente.PesoAlNacer = model.PesoAlNacer;
                    paciente.Talla = model.Talla;
                    paciente.CircunferenciaCefalica = model.CircunferenciaCefalica;
                    paciente.TipoParto = model.TipoParto;

                    paciente.TieneMembresia = false;
                    paciente.FechaRegistro = model.FechaRegistro;

                    //Antecedentes personales y patologicos observaciones
                    paciente.AntecedentesPersonalesObservaciones = model.AntecedentesPersonalesObservaciones;
                    paciente.AntecedentesPersonalesOtros = model.AntecedentesPersonalesOtros;

                    //Alergias
                    paciente.AlergiaAnestesiaLocal = model.AlergiaAnestesiaLocal;
                    paciente.AlergiaAspirina = model.AlergiaAspirina;
                    paciente.AlergiaPenicilina = model.AlergiaPenicilina;
                    paciente.AlergiaBarbiturios = model.AlergiaBarbiturios;
                    paciente.AlergiaSulfas = model.AlergiaSulfas;
                    paciente.AlergiaCodeina = model.AlergiaCodeina;
                    paciente.AlergiaMetales = model.AlergiaMetales;
                    paciente.AlergiaLatex = model.AlergiaLatex;
                    paciente.AlergiaYodo = model.AlergiaYodo;
                    paciente.AlergiaPolen = model.AlergiaPolen;
                    paciente.AlergiaAnimales = model.AlergiaAnimales;
                    paciente.AlergiaAlimentos = model.AlergiaAlimentos;
                    paciente.AlergiaOtros = model.AlergiaOtros;
                    paciente.AlergiaOtrosDescripcion = model.AlergiaOtrosDescripcion;

                    //Informacion medica
                    paciente.MedicaUsaLentesContacto = model.MedicaUsaLentesContacto;
                    paciente.MedicaUsaLentesContactoDescripcion = model.MedicaUsaLentesContactoDescripcion;
                    paciente.MedicaArticulacionesArtificiales = model.MedicaArticulacionesArtificiales;
                    paciente.MedicaArticulacionesArtificialesFecha = model.MedicaArticulacionesArtificialesFecha;
                    paciente.MedicaArticulacionesArtificialesComplicaciones = model.MedicaArticulacionesArtificialesComplicaciones;
                    paciente.MedicaTomaAlendronato = model.MedicaTomaAlendronato;
                    paciente.MedicaTomaAlendronatoFecha = model.MedicaTomaAlendronatoFecha;
                    paciente.MedicaTratamientoDolorHuesos = model.MedicaTratamientoDolorHuesos;
                    paciente.MedicaTratamientoDolorHuesosFechaInicio = model.MedicaTratamientoDolorHuesosFechaInicio;
                    paciente.MedicaTratamientoDolorHuesosDescripcionCaso = model.MedicaTratamientoDolorHuesosDescripcionCaso;
                    paciente.MedicaSustanciasReguladorasDrogas = model.MedicaSustanciasReguladorasDrogas;
                    paciente.MedicaSustanciasReguladorasDrogasFecha = model.MedicaSustanciasReguladorasDrogasFecha;
                    paciente.MedicaUsaTabaco = model.MedicaUsaTabaco;
                    paciente.MedicaBebidasAlcoholicas = model.MedicaBebidasAlcoholicas;
                    paciente.MedicaBebidasAlcoholicasDescripcion = model.MedicaBebidasAlcoholicasDescripcion;

                    ////Información dental
                    paciente.DentalSangradoCepillar = model.DentalSangradoCepillar;
                    paciente.DentalDolorFrio = model.DentalDolorFrio;
                    paciente.DentalDolorPresionar = model.DentalDolorPresionar;
                    paciente.DentalObjetosAtorados = model.DentalObjetosAtorados;
                    paciente.DentalBocaSeca = model.DentalBocaSeca;
                    paciente.DentalTratamientoPeriondal = model.DentalTratamientoPeriondal;
                    paciente.DentalTratamientoOrtodoncia = model.DentalTratamientoOrtodoncia;
                    paciente.DentalProblemasTratamientoDental = model.DentalProblemasTratamientoDental;
                    paciente.DentalProblemasTratamientoDentalDescripcion = model.DentalProblemasTratamientoDentalDescripcion;
                    paciente.DentalFluoradaAguaDomicilio = model.DentalFluoradaAguaDomicilio;
                    paciente.DentalBebeAguaFiltrada = model.DentalBebeAguaFiltrada;
                    paciente.DentalDolorOidos = model.DentalDolorOidos;
                    paciente.DentalMolestiaRuidoAlto = model.DentalMolestiaRuidoAlto;
                    paciente.DentalMolestiaRuidoAltoDescripcion = model.DentalMolestiaRuidoAltoDescripcion;
                    paciente.DentalBrumismo = model.DentalBrumismo;
                    paciente.DentalLesiones = model.DentalLesiones;
                    paciente.DentalLesionesDescripcion = model.DentalLesionesDescripcion;
                    paciente.DentalDentaduraPlacas = model.DentalDentaduraPlacas;
                    paciente.DentalDentaduraPlacasDescripcion = model.DentalDentaduraPlacasDescripcion;
                    paciente.DentalActividadesRecreacion = model.DentalActividadesRecreacion;
                    paciente.DentalActividadesRecreacionDescripcion = model.DentalActividadesRecreacionDescripcion;
                    paciente.DentalLesionesCabeza = model.DentalLesionesCabeza;
                    paciente.DentalLesionesCabezaDescripcion = model.DentalLesionesCabezaDescripcion;

                    //Análisis facial
                    paciente.FacialPatron = model.FacialPatron;
                    paciente.FacialPatronObservaciones = model.FacialPatronObservaciones;
                    paciente.FacialPerfil = model.FacialPerfil;
                    paciente.FacialPerfilObservaciones = model.FacialPerfilObservaciones;
                    paciente.FacialAsimetria = model.FacialAsimetria;
                    paciente.FacialAsimetriaObservaciones = model.FacialAsimetriaObservaciones;
                    paciente.FacialAlturaFacialEquilibrada = model.FacialAlturaFacialEquilibrada;
                    paciente.FacialAlturaFacialEquilibradaObservaciones = model.FacialAlturaFacialEquilibradaObservaciones;
                    paciente.FacialAnchoFacialEquilibrada = model.FacialAnchoFacialEquilibrada;
                    paciente.FacialAnchoFacialEquilibradaObservaciones = model.FacialAnchoFacialEquilibradaObservaciones;
                    paciente.FacialPerfilMaxilar = model.FacialPerfilMaxilar;
                    paciente.FacialPerfilMaxilarObservaciones = model.FacialPerfilMaxilarObservaciones;
                    paciente.FacialPerfilMandibular = model.FacialPerfilMandibular;
                    paciente.FacialPerfilMandibularObservaciones = model.FacialPerfilMandibularObservaciones;
                    paciente.FacialSurcoLabialMenton = model.FacialSurcoLabialMenton;
                    paciente.FacialSurcoLabialMentonObservaciones = model.FacialSurcoLabialMentonObservaciones;
                    paciente.FacialLabiosReposo = model.FacialLabiosReposo;

                    //Análisis funcional
                    paciente.FuncionalActividadComisurial = model.FuncionalActividadComisurial;
                    paciente.FuncionalActividadLingual = model.FuncionalActividadLingual;
                    paciente.FuncionalLabioSuperior = model.FuncionalLabioSuperior;
                    paciente.FuncionalLabioInferior = model.FuncionalLabioInferior;
                    paciente.FuncionalMasetero = model.FuncionalMasetero;
                    paciente.FuncionalMentoniano = model.FuncionalMentoniano;
                    paciente.FuncionalRespiracion = model.FuncionalRespiracion;
                    paciente.FuncionalDeglucion = model.FuncionalDeglucion;

                    ////Patrón facial
                    paciente.PatronFacial = model.PatronFacial;
                    paciente.CaracteristicaPatronFacial = model.CaracteristicaPatronFacial;

                    //MEDICOS
                    //Antecedentes
                    paciente.AntecedentesMedicos = model.AntecedentesMedicos;
                    paciente.AntecedentesQuirurgicos = model.AntecedentesQuirurgicos;
                    paciente.AntecedentesTraumaticos = model.AntecedentesTraumaticos;
                    paciente.AntecedentesAlergias = model.AntecedentesAlergias;
                    paciente.AntecedentesVicios = model.AntecedentesVicios;
                    paciente.AntecedentesMedicamentos = model.AntecedentesMedicamentos;


                    //Pediatricos
                    paciente.NombrePadre = model.NombrePadre;
                    paciente.NombreMadre = model.NombreMadre;


                    //Información de la madre
                    paciente.MadreFechaNacimiento = model.MadreFechaNacimiento;
                    paciente.MadreEmbarazos = model.MadreEmbarazos;
                    paciente.MadrePartosNormales = model.MadrePartosNormales;
                    paciente.MadreCesareas = model.MadreCesareas;
                    paciente.MadreAbortos = model.MadreAbortos;
                    paciente.MadreHijosMuertos = model.MadreHijosMuertos;
                    paciente.MadreComplicaciones = model.MadreComplicaciones;

                    //Historia medica
                    paciente.HistoriaMedicoPersonal = model.HistoriaMedicoPersonal;
                    paciente.HistoriaTelefonoMedico = model.HistoriaTelefonoMedico;
                    paciente.HistoriaEspecialidadMedico = model.HistoriaEspecialidadMedico;
                    paciente.HistoriaTratamientoMedico = model.HistoriaTratamientoMedico;
                    paciente.HistoriaSangraMuchoCortarse = model.HistoriaSangraMuchoCortarse;
                    paciente.HistoriaHospitalizado = model.HistoriaHospitalizado;
                    paciente.HistoriaOperado = model.HistoriaOperado;
                    paciente.HistoriaAlergiaMedicina = model.HistoriaAlergiaMedicina;
                    paciente.HistoriaAlergiaComida = model.HistoriaAlergiaComida;
                    paciente.HistoriaAlergiaOtros = model.HistoriaAlergiaOtros;
                    paciente.HistoriaProblemaEmocional = model.HistoriaProblemaEmocional;
                    paciente.HistoriaObservaciones = model.HistoriaObservaciones;

                    //Datos GINECOLOGICOS ESTUARDO PRUEBA
                    paciente.CicloMenstGine = model.CicloMenstGine;
                    paciente.ETSGine = model.ETSGine;
                    paciente.VIHGine = model.VIHGine;
                    paciente.GrupoFactorGine = model.GrupoFactorGine;
                    paciente.TorchGine = model.TorchGine;
                    paciente.InicioVidaSexualGine = model.InicioVidaSexualGine;
                    paciente.ParejasSexGine = model.ParejasSexGine;
                    paciente.ObesidadGine = model.ObesidadGine;
                    paciente.DesnutricionGine = model.DesnutricionGine;
                    paciente.QGine = model.QGine;
                    paciente.PGine = model.PGine;
                    paciente.ABGine = model.ABGine;
                    paciente.CGine = model.CGine;
                    paciente.FURGine = model.FURGine;
                    paciente.MuerteNeoGine = model.MuerteNeoGine;
                    paciente.FPPGine = model.FPPGine;
                    paciente.HVGine = model.HVGine;
                    paciente.MuerteGine = model.MuerteGine;
                    paciente.ControlPrenatalGine = model.ControlPrenatalGine;
                    paciente.ComadronaGine = model.ComadronaGine;
                    paciente.NoControlesGine = model.NoControlesGine;

                    //Datos de la Mamá
                    paciente.UteroGravioGine = model.UteroGravioGine;
                    paciente.AbdomenObstetricoGine = model.AbdomenObstetricoGine;
                    paciente.FCBGine = model.FCBGine;
                    paciente.AUGine = model.AUGine;
                    paciente.PresentacionLeopoldGine = model.PresentacionLeopoldGine;
                    paciente.OtrasGine = model.OtrasGine;
                    paciente.ActividadUterinaGine = model.ActividadUterinaGine;
                    paciente.MovimientoFetalPercetibleGine = model.MovimientoFetalPercetibleGine;
                    paciente.EspecifiqueGine = model.EspecifiqueGine;
                    paciente.TactoVaginalGine = model.TactoVaginalGine;
                    paciente.DGine = model.DGine;
                    paciente.CMSGine = model.CMSGine;
                    paciente.BPorcientoGine = model.BPorcientoGine;
                    paciente.AltiutudGine = model.AltiutudGine;
                    paciente.VariedadPosicionGine = model.VariedadPosicionGine;
                    paciente.MembranasOvularesGine = model.MembranasOvularesGine;
                    paciente.LiquidoAmnioticoGine = model.LiquidoAmnioticoGine;
                    paciente.Especifique2Gine = model.Especifique2Gine;
                    paciente.PelvisGine = model.PelvisGine;

                    //Beneficiarios EPSS
                    if (model.BeneficiariosEpssPacientesViewModel != null && model.BeneficiariosEpssPacientesViewModel.Count > 0)
                    {
                        foreach (var beneficiarioViewModel in model.BeneficiariosEpssPacientesViewModel)
                        {
                            var beneficiario = new PersonaSeguro
                            {
                                Name = beneficiarioViewModel.Name,
                                Nit = beneficiarioViewModel.Nit,
                                Tipo = beneficiarioViewModel.Tipo
                            };
                            paciente.PersonasSeguro.Add(beneficiario);
                        }
                    }
                    //Antecedentes y patologias familiares
                    //if (model.PatologiasPacienteViewModel != null && model.PatologiasPacienteViewModel.Count > 0)
                    //{
                    //    foreach (var patologiaModel in model.PatologiasPacienteViewModel)
                    //    {
                    //        var patologia = new PatologiaPaciente
                    //        {
                    //            PacienteId = paciente.Id,
                    //            TipoPatologiaId = patologiaModel.TipoPatologiaId,
                    //            Madre = patologiaModel.Madre,
                    //            AbuelaMaterna = patologiaModel.AbuelaMaterna,
                    //            AbueloMaterno = patologiaModel.AbueloMaterno,
                    //            OtrosMaterno = patologiaModel.OtrosMaterno,
                    //            Padre = patologiaModel.Padre,
                    //            AbuelaPaterna = patologiaModel.AbuelaPaterna,
                    //            AbueloPaterno = patologiaModel.AbueloPaterno,
                    //            Hermanos = patologiaModel.Hermanos,
                    //            OtrosPaterno = patologiaModel.OtrosPaterno,
                    //            DescripcionOtraPatologia = patologiaModel.DescripcionOtraPatologia
                    //        };
                    //        paciente.PatologiasPaciente = new List<PatologiaPaciente>();
                    //        paciente.PatologiasPaciente.Add(patologia);
                    //    }
                    //}


                    //Preguntas de registro
                    //if (model.PreguntasRegistroPacienteViewModel != null && model.PreguntasRegistroPacienteViewModel.Count > 0)
                    //{
                    //    foreach (var preguntaRegistro in model.PreguntasRegistroPacienteViewModel)
                    //    {
                    //        var pregunta = new PacientePreguntaRegistro
                    //        {
                    //            PreguntaRegistroId = preguntaRegistro.PreguntaId,
                    //            Respuesta = preguntaRegistro.Respuesta
                    //        };
                    //        paciente.PacientesPreguntasRegistro = new List<PacientePreguntaRegistro>();
                    //        paciente.PacientesPreguntasRegistro.Add(pregunta);
                    //    }
                    //}


                    //Vacunas paciente
                    //if (model.VacunasPacienteViewModel != null && model.VacunasPacienteViewModel.Count > 0)
                    //{
                    //    foreach (var vacunaModel in model.VacunasPacienteViewModel)
                    //    {
                    //        var vacuna = new VacunaPaciente
                    //        {
                    //            VacunaId = vacunaModel.VacunaId,
                    //            Primera = vacunaModel.Primera,
                    //            Segunda = vacunaModel.Segunda,
                    //            Tercera = vacunaModel.Tercera,
                    //            PrimerRefuerzo = vacunaModel.PrimerRefuerzo,
                    //            SegundoRefuerzo = vacunaModel.SegundoRefuerzo,
                    //            FechaPrimera = vacunaModel.FechaPrimera,
                    //            FechaSegunda = vacunaModel.FechaSegunda,
                    //            FechaTercera = vacunaModel.FechaTercera,
                    //            FechaPrimerRefuerzo = vacunaModel.FechaPrimerRefuerzo,
                    //            FechaSegundoRefuerzo = vacunaModel.FechaSegundoRefuerzo
                    //        };
                    //        paciente.VacunasPaciente = new List<VacunaPaciente>();
                    //        paciente.VacunasPaciente.Add(vacuna);
                    //    }
                    //}


                    //Antecedentes personales
                    //if (model.AntecedentesPersonalesViewModel != null && model.AntecedentesPersonalesViewModel.Count > 0)
                    //{
                    //    foreach (var antecedenteViewModel in model.AntecedentesPersonalesViewModel)
                    //    {
                    //        var antecedente = new PacienteAntecedentePersonal
                    //        {
                    //            AntecedentePersonalId = antecedenteViewModel.AntecedenteId,
                    //            PresentoAntecedente = antecedenteViewModel.PresentoAntecedente,
                    //            FechaAntecedente = antecedenteViewModel.FechaAntecedente
                    //        };
                    //        paciente.PacientesAntecedentesPersonales.Add(antecedente);
                    //    }
                    //}


                    _pacientesRepository.Update(paciente);
                    TempData["Message"] = "¡El paciente se ha modificado con exito.!";
                    return Json(new { Exitoso = true });
                }
                catch (Exception ex)
                {
                    return Json(new { Exitoso = false, Mensaje = "Error al modificar paciente. " + ex.Message });
                }
            }

            return Json(new { Exitoso = false, Mensaje = "Asegúrese de diligenciar todos los campos obligatorios" });
        }

        public IActionResult CambiarFase(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var paciente = _pacientesRepository.Get((int)id);

            if (paciente == null)
            {
                return StatusCode(404);
            }

            var model = new PacientesCambiarFaseViewModel();
            model.PacienteId = paciente.Id;
            model.PacienteFechaRegistro = paciente.FechaRegistro == null ? "-"
                : Convert.ToDateTime(paciente.FechaRegistro).ToString("yyyy/MM/dd");
            model.PacienteNombre = paciente.Nombre;

            if (paciente.PacientesFasesTratamiento.Count == 0)
            {
                model.PacienteFaseTratamientoActual = "Sin asignar";
            }
            else
            {
                var maximaFecha = paciente.PacientesFasesTratamiento.Max(p => p.FechaInicioFase);
                var ultimaFase = paciente.PacientesFasesTratamiento
                    .Where(p => p.FechaInicioFase == maximaFecha && !p.FaseFinalizada).FirstOrDefault();
                model.PacienteFaseTratamientoActual = ultimaFase.FaseTratamiento.NombreFase;
            }

            return View(model);
        }
        [HttpPost]
        public JsonResult CambiarFase(PacientesCambiarFaseViewModel model)
        {
            if (model.FaseTratamientoNueva == null || model.FaseTratamientoNueva == "")
            {
                return Json(new { Exitoso = false, Mensaje = "Selecciona una fase" });
            }
            switch (model.FaseTratamientoNueva)
            {
                case "adelgazamiento":
                    if (model.PacienteFaseTratamientoActual == "Adelgazamiento")
                    {
                        return Json(new { Exitoso = false, Mensaje = "La fase nueva debe ser distinta a la actual" });
                    }
                    break;
                case "mantenimiento1":
                    if (model.PacienteFaseTratamientoActual == "Mantenimiento 1")
                    {
                        return Json(new { Exitoso = false, Mensaje = "La fase nueva debe ser distinta a la actual" });
                    }
                    break;
                case "mantenimiento2":
                    if (model.PacienteFaseTratamientoActual == "Mantenimiento 2")
                    {
                        return Json(new { Exitoso = false, Mensaje = "La fase nueva debe ser distinta a la actual" });
                    }
                    break;
                case "mantenimiento3":
                    if (model.PacienteFaseTratamientoActual == "Mantenimiento 3")
                    {
                        return Json(new { Exitoso = false, Mensaje = "La fase nueva debe ser distinta a la actual" });
                    }
                    break;
            }
            if (ModelState.IsValid)
            {
                var paciente = _pacientesRepository.Get(model.PacienteId);
                if (paciente.PacientesFasesTratamiento.Count > 0)
                {

                    foreach (var fase in paciente.PacientesFasesTratamiento)
                    {
                        if (!fase.FaseFinalizada)
                        {
                            fase.FaseFinalizada = true;
                            fase.FechaFinalizacionFase = DateTime.Today;
                        }
                    }
                }
                var faseId = 0;
                switch (model.FaseTratamientoNueva)
                {
                    case "adelgazamiento":
                        faseId = (int)FaseTratamientoEnum.Adelgazamiento;
                        break;
                    case "mantenimiento1":
                        faseId = (int)FaseTratamientoEnum.Mantenimiento1;
                        break;
                    case "mantenimiento2":
                        faseId = (int)FaseTratamientoEnum.Mantenimiento2;
                        break;
                    case "mantenimiento3":
                        faseId = (int)FaseTratamientoEnum.Mantenimiento3;
                        break;
                    default:
                        faseId = 1;
                        break;
                }
                var faseAgregada = new PacienteFaseTratamiento
                {
                    FaseTratamientoId = faseId,
                    FechaInicioFase = model.FechaCambioFase,
                    FaseFinalizada = false,
                    PesoAlIniciar = model.PesoAlIniciar
                };

                paciente.PacientesFasesTratamiento.Add(faseAgregada);

                _pacientesRepository.Update(paciente);
                TempData["Message"] = "¡El cambio de fase del paciente ha sido exitoso!";
                return Json(new { Exitoso = true, PacienteId = model.PacienteId });
            }

            return Json(new { Exitoso = false, Mensaje = "Asegúrese de diligenciar los campos obligatorios" });
        }
        public IActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _pacientesRepository.Get((int)id);


            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _pacientesRepository.Update(model);
            TempData["Message"] = "¡El paciente se ha eliminado con exito.!";
            return RedirectToAction("Lista");
        }
        public JsonResult RetornarCliente(string nombre)
        {
            var clientebuscado = _pacientesRepository.GetPacientePorNombre(nombre);
            return Json(clientebuscado);
        }
        public JsonResult RetornarPacienteById(int id)
        {
            var pacientebuscado = _pacientesRepository.GetPacientePorId(id);
            return Json(pacientebuscado);
        }
        public JsonResult GetPacienteByDPI(string Dpi)
        {
            var pacientebuscado = _pacientesRepository.GetPacientePorDPI(Dpi);

            if (pacientebuscado != null)
            {
                // Paciente DPI encontrado
                return Json(new { existe = true, paciente = pacientebuscado });
            }
            else
            {
                // Paciente DPI no encontrado
                return Json(new { existe = false });
            }
            //return Json(pacientebuscado);
        }

        public JsonResult GetPacienteById(int pacienteId)
        {
            var pacientebuscado = _pacientesRepository.GetPacientePorId(pacienteId);

            if (pacientebuscado != null)
            {
                // Paciente DPI encontrado
                return Json(new { existe = true, paciente = pacientebuscado });
            }
            else
            {
                // Paciente DPI no encontrado
                return Json(new { existe = false });
            }
            //return Json(pacientebuscado);
        }
        public IActionResult AplicarMembresia(int? pacienteId)
        {
            if (pacienteId == null)
                return RedirectToAction("Index", "Home");
            var paciente = _pacientesRepository.Get((int)pacienteId);
            if (paciente == null)
                return RedirectToAction("Index", "Home");
            var model = new PacientesAplicarMembresiaViewModel
            {
                PacienteId = paciente.Id,
                PacienteNombre = paciente.Nombre
            };
            return View(model);
        }
        [HttpPost]
        public JsonResult AplicarMembresia(PacientesAplicarMembresiaViewModel model)
        {
            try
            {
                var paciente = _pacientesRepository.Get(model.PacienteId);
                paciente.TieneMembresia = true;
                paciente.FechaInicioMembresia = DateTime.Today;
                _pacientesRepository.Update(paciente);
                TempData["Message"] = "Se ha registrado la membresía del paciente";
                return Json(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = "Error al registrar membresía. " + ex.Message });
            }
        }
        public IActionResult RegistrarRetiro(int? pacienteId)
        {
            if (pacienteId == null)
                return RedirectToAction("Index", "Home");
            var paciente = _pacientesRepository.Get((int)pacienteId);
            if (paciente == null)
                return RedirectToAction("Index", "Home");
            if (paciente.EstadoPacienteId == (int)EstadoPacienteEnum.Inactivo)
            {
                TempData["Message"] = "El paciente ya se encuentra retirado";
                return RedirectToAction("Index", "Home");
            }
            var model = new PacientesRegistrarRetiroViewModel
            {
                PacienteId = paciente.Id,
                PacienteNombre = paciente.Nombre
            };
            return View(model);
        }
        [HttpPost]
        public JsonResult RegistrarRetiro(PacientesRegistrarRetiroViewModel model)
        {
            try
            {
                //Actualizar estado de paciente
                var paciente = _pacientesRepository.Get(model.PacienteId);
                paciente.EstadoPacienteId = (int)EstadoPacienteEnum.Inactivo;
                paciente.NumeroTarjetaCredito = null;
                _pacientesRepository.Update(paciente);


                #region Generar cuenta por cobrar

                decimal valorCuenta = 0;
                var tieneMembresia = (bool)paciente.TieneMembresia;
                if (tieneMembresia)
                {
                    valorCuenta = (int)ValorMembresiaEnum.Membresia;
                }
                else
                {
                    var faseActual = _pacientesRepository.GetFaseTratamientoActual(model.PacienteId);
                    if (faseActual != null && faseActual.FaseTratamiento != null)
                    {
                        valorCuenta = faseActual.FaseTratamiento.Valor;
                    }
                }
                if (valorCuenta != 0)
                {
                    var cuentaPorCobrar = new CuentaPorCobrar
                    {
                        PacienteId = paciente.Id,
                        Valor = valorCuenta,
                        FechaLimitePago = DateTime.Today.AddMonths(1),
                        Pagada = false,
                        Eliminada = false
                    };
                    _cuentasPorCobrarRepository.Add(cuentaPorCobrar);
                }

                #endregion

                //Agregar registro historial
                var registroHistorico = new PacienteHistorial
                {
                    PacienteId = model.PacienteId,
                    AccionPacienteId = (int)AccionPacienteEnum.Retiro,
                    MotivoRetiro = model.MotivoRetiro,
                    VolverAContactar = model.VolverAContactar,
                    FechaContacto = model.FechaContacto,
                    Fecha = DateTime.Now
                };
                _pacientesRepository.AddHistorial(registroHistorico);

                TempData["Message"] = "Se ha registrado el retiro del paciente";
                return Json(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = "Error al registrar retiro. " + ex.Message });
            }
        }
        public IActionResult RetomarServicio(int? pacienteId)
        {
            var paciente = _pacientesRepository.Get((int)pacienteId);
            paciente.TipoPacienteId = (int)TipoPacienteEnum.Retomante;
            _pacientesRepository.Update(paciente);
            TempData["Message"] = "Se ha registrado el paciente como retomante";
            return RedirectToAction("Lista");
        }
        public IActionResult Informacion(int? pacienteId)
        {
            if (pacienteId == null || pacienteId <= 0)
                return RedirectToAction("Lista");

            var paciente = _pacientesRepository.Get((int)pacienteId);
            if (paciente == null)
                return RedirectToAction("Lista");

            var informacionExtra = _context.PacientesInformacionExtra
                .Where(x => x.PacienteId == pacienteId)
                .ToList();

            ViewBag.AntPatologicos = informacionExtra
                .Where(x => x.Posicion >= 13 && x.Posicion <= 26)
                .ToList();

            ViewBag.AntPersonalesNoPatologicos = informacionExtra
                .Where(x => (x.Posicion >= 1 && x.Posicion <= 12)
                    || (x.Posicion >= 27 && x.Posicion <= 30))
                .ToList();

            return View(paciente);
        }
        [HttpPost]
        public JsonResult ActualizarFotografiaPaciente(int pacienteId, string rutaFotografia)
        {
            try
            {
                var paciente = _pacientesRepository.Get(pacienteId, false);
                paciente.UrlFotografiaPaciente = rutaFotografia;
                _pacientesRepository.Update(paciente);
                TempData["Message"] = "Se ha actualizado la fotografía del paciente";
                return Json(new
                {
                    exitoso = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exitoso = false,
                    mensaje = "Error al actualizar fotografía. " + ex.Message
                });
            }
        }
        [HttpPost]
        public JsonResult ActualizarFirmaRegistro(int pacienteId, string rutaFirma)
        {
            try
            {
                if (pacienteId <= 0)
                {
                    return Json(new
                    {
                        exitoso = false,
                        mensaje = "ID de paciente inválido."
                    });
                }

                if (string.IsNullOrWhiteSpace(rutaFirma))
                {
                    return Json(new { exitoso = true });
                }

                var paciente = _pacientesRepository.Get(pacienteId, false);
                if (paciente == null)
                {
                    return Json(new
                    {
                        exitoso = false,
                        mensaje = $"No se encontró el paciente con ID {pacienteId}."
                    });
                }

                paciente.UrlFirmaRegistro = rutaFirma;
                _pacientesRepository.Update(paciente);
                TempData["Message"] = "Se ha actualizado la firma de registro del paciente";
                return Json(new
                {
                    exitoso = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exitoso = false,
                    mensaje = "Error al actualizar firma. " + ex.Message
                });
            }
        }
        [HttpPost]
        public JsonResult ActualizarFirmaPoliticasPago(int pacienteId, string rutaFirma)
        {
            try
            {
                if (pacienteId <= 0 || string.IsNullOrWhiteSpace(rutaFirma))
                {
                    return Json(new { exitoso = pacienteId > 0 });
                }

                var paciente = _pacientesRepository.Get(pacienteId, false);
                if (paciente == null)
                {
                    return Json(new { exitoso = false, mensaje = "Paciente no encontrado." });
                }

                paciente.UrlFirmaPoliticas = rutaFirma;
                _pacientesRepository.Update(paciente);
                TempData["Message"] = "Se ha actualizado la firma de políticas de pago del paciente";
                return Json(new
                {
                    exitoso = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exitoso = false,
                    mensaje = "Error al actualizar firma. " + ex.Message
                });
            }
        }
        [HttpPost]
        public JsonResult ActualizarFirmaDeclaracionConsentimiento(int pacienteId, string rutaFirma)
        {
            try
            {
                var paciente = _pacientesRepository.Get(pacienteId, false);
                paciente.UrlFirmaConsentimiento = rutaFirma;
                _pacientesRepository.Update(paciente);
                TempData["Message"] = "Se ha actualizado la firma de consentimiento del paciente";
                return Json(new
                {
                    exitoso = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exitoso = false,
                    mensaje = "Error al actualizar firma. " + ex.Message
                });
            }
        }
        [HttpPost]
        public JsonResult ActualizarFirmaTarjetaCredito(int pacienteId, string rutaFirma)
        {
            try
            {
                var paciente = _pacientesRepository.Get(pacienteId, false);
                paciente.UrlFirmaTarjetaCredito = rutaFirma;
                _pacientesRepository.Update(paciente);
                TempData["Message"] = "Se ha actualizado la firma de tarjeta de crédito del paciente";
                return Json(new
                {
                    exitoso = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exitoso = false,
                    mensaje = "Error al actualizar firma. " + ex.Message
                });
            }
        }
        [HttpPost]
        public JsonResult RegistrarArchivo(int pacienteId, string nombreArchivo, string rutaArchivo)
        {
            try
            {
                var paciente = _pacientesRepository.Get(pacienteId);
                paciente.PacienteArchivos.Add(new PacienteArchivo
                {
                    NombreArchivo = nombreArchivo,
                    UrlArchivo = rutaArchivo
                });
                _pacientesRepository.Update(paciente);
                TempData["Message"] = "Se ha subido el archivo " + nombreArchivo;
                return Json(new
                {
                    exitoso = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    exitoso = false,
                    mensaje = "Error al subir archivo. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string RegistrarPacienteRapido(string Nombre, string Nit, string Direccion,
            string Telefono, string FechaNacimiento, string Genero)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Nombre))
                {
                    return System.Text.Json.JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "El nombre del paciente es requerido."
                    });
                }

                var existente = _pacientesRepository.GetPacientePorNombre(Nombre.Trim());
                if (existente != null)
                {
                    return System.Text.Json.JsonSerializer.Serialize(new
                    {
                        Exitoso = true,
                        YaExistia = true,
                        Resultado = new
                        {
                            PacienteId = existente.Id,
                            Nombre = existente.Nombre,
                            Nit = existente.Nit,
                            Direccion = existente.Direccion
                        }
                    });
                }

                var paciente = new Paciente();
                paciente.Nombre = Nombre.Trim();
                paciente.Nit = Nit ?? "CF";
                paciente.Direccion = Direccion ?? "";
                paciente.Telefono = Telefono ?? "";
                paciente.Celular = Telefono ?? "";
                paciente.Eliminado = false;
                paciente.TipoPacienteId = (int)TipoPacienteEnum.Nuevo;
                paciente.EstadoPacienteId = (int)EstadoPacienteEnum.Activo;
                paciente.FechaRegistro = DateTime.Today;
                paciente.TieneMembresia = false;
                paciente.Retirado = false;
                paciente.esta_Afiliado = false;

                if (!string.IsNullOrWhiteSpace(FechaNacimiento)
                    && DateTime.TryParse(FechaNacimiento, out DateTime fechaNac))
                {
                    paciente.FechaNacimiento = fechaNac;
                }

                if (!string.IsNullOrWhiteSpace(Genero))
                {
                    var sexos = _pacientesRepository.GetSexosList();
                    var sexoEncontrado = sexos.FirstOrDefault(s =>
                        s.DescripcionSexo != null &&
                        s.DescripcionSexo.StartsWith(Genero, StringComparison.OrdinalIgnoreCase));
                    if (sexoEncontrado != null)
                        paciente.SexoId = sexoEncontrado.Id;
                }

                _pacientesRepository.Add(paciente);

                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = new
                    {
                        PacienteId = paciente.Id,
                        Nombre = paciente.Nombre,
                        Nit = paciente.Nit,
                        Direccion = paciente.Direccion
                    }
                });
            }
            catch (Exception ex)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al registrar paciente. " + ex.Message
                });
            }
        }

        public IActionResult CrearFormConsentimientoHospi(int HabitacionId, int PacienteId, int? ConsultaId = null, int? CitaId = null)
        {
            if (PacienteId <= 0 || HabitacionId <= 0)
            {
                return Content(
                    "<p>Paciente o habitación no válidos. Cierre esta ventana y abra el consentimiento desde el asistente de hospitalización (paso 5).</p>",
                    "text/html; charset=utf-8");
            }

            // Obtener datos
            var paciente = _pacientesRepository.Get(PacienteId);
            if (paciente == null)
            {
                return Content(
                    $"<p>No se encontró el paciente con ID {PacienteId}. Verifique que el paciente existe en el sistema.</p>",
                    "text/html; charset=utf-8");
            }

            var habitacion = _habitacionRepository.Get(HabitacionId);
            if (habitacion == null)
            {
                return Content(
                    $"<p>No se encontró la habitación con ID {HabitacionId}.</p>",
                    "text/html; charset=utf-8");
            }

            var ultimaConsulta = _consultasRepository.GetUltimaConsultaPaciente(PacienteId);
            var cita = (CitaId.HasValue && CitaId.Value > 0)
                ? _citasRepository.GetCita(CitaId.Value)
                : null;

            if (cita == null && ConsultaId.HasValue && ConsultaId.Value > 0)
            {
                var consultaCtx = _consultasRepository.GetConsulta(ConsultaId.Value);
                if (consultaCtx?.CitasId != null && consultaCtx.CitasId.Value > 0)
                    cita = _citasRepository.GetCita(consultaCtx.CitasId.Value);
                else if (consultaCtx?.Citas != null)
                    cita = consultaCtx.Citas;
            }

            if (cita == null)
            {
                cita = _context.Citass
                    .Where(c => c.PacienteId == PacienteId
                        && c.HabitacionId == HabitacionId
                        && !c.Eliminado)
                    .OrderByDescending(c => c.FechaInicio)
                    .ThenByDescending(c => c.Id)
                    .FirstOrDefault();
            }

            var nombreMedicoTratante = "";
            var colegiadoMedico = "";
            var urlFirmaMedico = "";
            var especialidadMedico = "";
            if (cita?.Empleado != null)
            {
                nombreMedicoTratante = cita.Empleado.NombreYApellidos ?? "";
                colegiadoMedico = cita.Empleado.Colegiado ?? "";
                urlFirmaMedico = cita.Empleado.FirmaEmpleado ?? "";
            }
            if (cita != null && cita.EspecialidadText != "N/A")
                especialidadMedico = cita.EspecialidadText;

            var contactosEmergencia = new List<ContactoEmergenciaVM>();

            if (cita != null && !string.IsNullOrWhiteSpace(cita.AcompananteNombre))
            {
                contactosEmergencia.Add(new ContactoEmergenciaVM
                {
                    Nombre = cita.AcompananteNombre,
                    Telefono = cita.AcompananteTelefono ?? "",
                    Parentesco = cita.AcompananteRelacion ?? ""
                });
            }
            else
            {
                contactosEmergencia.Add(new ContactoEmergenciaVM
                {
                    Nombre = "",
                    Telefono = "",
                    Parentesco = ""
                });
            }

            var model = new ConsentimientoHospiVM()
            {
                // Relación con Paciente
                PacienteId = paciente?.Id ?? 0,
                NombrePaciente = paciente?.Nombre ?? "",

                // Relación con Habitacion
                HabitacionId = habitacion?.Id ?? 0,
                NumeroHabitacion = habitacion?.NombreNumeroHabitacion ?? "",

                // Datos de la Hospitalización
                HospitalizacionId = "",

                // Datos del Paciente
                HoraIngreso = "",
                NumeroPaciente = paciente?.Id.ToString() ?? "",
                NombreCompleto = paciente?.Nombre ?? "",
                EstadoCivil = paciente?.EstadoCivil ?? "",
                DPI = paciente?.Dpi ?? "",
                FechaNacimiento = paciente?.FechaNacimiento?.ToString("yyyy-MM-dd") ?? "",
                Edad = "",
                Nacionalidad = paciente?.OrigenPaciente ?? "",
                Direccion = paciente?.Direccion ?? "",
                Celular = string.IsNullOrEmpty(paciente?.Celular) ? paciente?.Telefono ?? "" : paciente?.Celular,
                Email = paciente?.Email ?? "",
                TipoSangre = paciente?.TipoDeSangre ?? "",
                Genero = paciente?.sexoText ?? "",
                Religion = paciente?.ReligionPaciente ?? "",
                Ocupacion = paciente?.Ocupacion ?? "",

                // Información del seguro médico
                PoseeSeguroMedico = "",
                Aseguradora = "",
                TipoPoliza = "",
                NombreEmpresa = "",
                FormularioPreAutorizacion = "",
                TratamientoMedico = "",

                // Datos del Responsable de la Cuenta
                NombreResponsable = cita?.ResponsableNombre ?? "",
                DPIResponsable = cita?.ResponsableDPI ?? "",
                EdadResponsable = "",
                DireccionResponsable = cita?.ResponsableDireccion ?? "",
                CelularResponsable = cita?.ResponsableTelefono ?? "",
                EmailResponsable = "",
                NITResponsable = cita?.ResponsableNit ?? "",
                NombreFacturacion = cita?.ResponsableNombre ?? "",

                // Contactos de Emergencia (múltiples)
                ContactosEmergencia = contactosEmergencia,

                // Información Adicional
                HospitalProporcionoMedico = "",
                MedicoAfiliado = "",
                NombreMedicoTratante = nombreMedicoTratante,
                RecetaMedica = "",
                CitaId = cita?.Id,
                ConsultaId = ConsultaId,
                EspecialidadMedico = especialidadMedico,
                ColegiadoMedico = colegiadoMedico,
                UrlFirmaMedico = urlFirmaMedico,

                // Firmas
                URLFirmaPaciente = paciente?.UrlFirmaRegistro ?? "",
                URLFirmaResponsable = "",
                NombreNotaria = "",
                NombreRepresentanteNarajo = "",
                URLFirmaNotaria = "",
                URLFirmaRepresentanteNaranjo = ""
            };

            return View(model);
        }


        [HttpPost]
        public JsonResult ActualizarCampo(int pacienteId, string campo, string valor)
        {
            try
            {
                if (pacienteId <= 0)
                    return Json(new { success = false, message = "Paciente inválido." });

                valor = (valor ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(valor))
                    return Json(new { success = false, message = "Valor vacío." });

                var paciente = _pacientesRepository.Get(pacienteId);
                if (paciente == null)
                    return Json(new { success = false, message = "Paciente no encontrado" });

                switch (campo)
                {
                    case "TipoSangre":
                        paciente.TipoDeSangre = valor;
                        break;
                    case "Peso":
                        paciente.Peso = valor;
                        break;
                    case "Estatura":
                        paciente.Talla = valor;
                        break;
                    default:
                        return Json(new { success = false, message = "Campo no válido" });
                }

                _pacientesRepository.Update(paciente);
                
                return Json(new { success = true, nuevoValor = valor });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}