using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using sistema.Utilidades;
namespace sistema.Models
{
    public class HospitalizacionDetallesViewModel
    {
        private readonly IConfiguration _configuration;

        public HospitalizacionDetallesViewModel(IConfiguration configuration = null)
        {
            _configuration = configuration;
        }

        public Consulta Consulta { get; set; }
        public string NombreProfesional { get; set; }  // El nombre del profesional que hizo la acción
        public DateTime FechaHora { get; set; }  // Fecha y hora de la acción
        public int CuentaId { get; set; }
        public int HospitalizacionId { get; set; }
        public bool HospitalizacionFinalizada { get; set; }
        public bool Pagada { get; set; }
        public SelectList SegurosSelectListItem { get; set; }
        public SelectList DoctoresSelectListItem { get; set; }
        public List<MedicoSecundarioDtoHospi> MedicosSecundarios { get; set; } = new();

        public string FechaNacimientoPaciente { get; set; }
        public int? EdadPaciente { get; set; }

        public string EspecialidadNombre { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaInicial { get; set; }
        public DateTime? FechaFinal { get; set; }
        public int HabitacionId { get; set; }
        public string CategoriaHabitacionNombre { get; set; }
        public string CodigoSeguro { get; set; }
        public string PacienteTalla { get; set; }
        public string PacientePeso { get; set; }

        public string MedicoAsignado { get; set; }
        public int? MedicoAsignadoEmpleadoId { get; set; }
        public int BodegaId { get; set; }
        public string NumeroNombreHabitacion { get; set; }
        public int? NumeroCamas { get; set; }
        public int PacienteId { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteDpi { get; set; }
        public string TipoPaciente { get; set; }
        public int PacienteEstadoId { get; set; }
        public string PacienteEstado { get; set; }
        public string PacienteSexo { get; set; }
        public string PacienteTelefono { get; set; }
        public string PacienteEmail { get; set; }
        public string PacienteCelular { get; set; }
        public string PacienteTipoSangre { get; set; }
        public string MedicamentosControladosInicialJson { get; set; }
        public string FechaProcedimientoControlados { get; set; }

        //Depositos
        public int FormaPagoId { get; set; }
        public SelectList FormaPagoSelectListItems { get; set; }
        public int MonedaId { get; set; }
        public SelectList MonedaSelectListItems { get; set; }

        //Permisos usuarios TABS
        public bool AutorizacionTabEnfermeria { get; set; }
        public bool AutorizacionTabActualizarEstadia { get; set; }
        public bool AutorizacionTabNotaEnfermeria { get; set; }
        public bool AutorizacionTabNotaEvolucion { get; set; }
        public bool AutorizacionTabNotaMedica { get; set; }
        public bool AutorizacionTabNotaOperatoria { get; set; }
        public bool AutorizacionTabDietas { get; set; }
        public bool AutorizacionTabPagos { get; set; }
        public bool AutorizacionTabControlGlucometria { get; set; }
        public bool AutorizacionTabSignosVitales { get; set; }
        public bool AutorizacionTabIncretaExcreta { get; set; }

        public int PacienteIdPDF { get; set; }
        public int HabitacionIdPDF { get; set; }
        public int CitaIdPDF { get; set; }

        public string CitaAnestesista { get; set; }
        public string CitaPrimerAyudante { get; set; }
        public string CitaSegundoAyudante { get; set; }
        public string CitaInstrumentista { get; set; }
        public string CitaCirculante { get; set; }
        public string CitaProcedimiento { get; set; }

        public int? CitaCirujanoId { get; set; }

        public decimal TotalHonorarios { get; set; }

        // Estructura para listar los honorarios (solo consulta)
        public List<HonorarioDetalleDto> Honorarios { get; set; } = new();

        public string NumeroNombreHabitacionText
        {
            get
            {
                return NumeroNombreHabitacion ?? "-";
            }
        }
        public string FechaInicialText
        {
            get
            {
                return FechaInicial == null ? "-" : Convert.ToDateTime(FechaInicial).ToString("yyyy/MM/dd hh:mm t");
            }
        }
        public string FechaFinalText
        {
            get
            {
                return FechaFinal == null ? "-" : Convert.ToDateTime(FechaFinal).ToString("yyyy/MM/dd hh:mm t");
            }
        }
        public string ObservacionesText
        {
            get
            {
                return Observaciones ?? "Sin observaciones";
            }
        }
        public string NumeroCamasText
        {
            get
            {
                return NumeroCamas == null ? "-" : NumeroCamas.ToString();
            }
        }

        public string UrlArchivoConsentimiento { get; internal set; }

        public void Init(ICuentasPorCobrar cuentasPorCobrarRepository, ISeguro seguroRepository = null, IEmpleado empleadoRepository = null)
        {
            FormaPagoSelectListItems = new SelectList(cuentasPorCobrarRepository.GetFormasPago(), "Id", "NombreFormaPago");
            MonedaSelectListItems = new SelectList(cuentasPorCobrarRepository.GetMonedas(), "Id", "NombreMoneda");
            SegurosSelectListItem = SelectListHelper.Create(seguroRepository.GetList(), "Id", "Nombre");
            DoctoresSelectListItem = EmpleadoSelectListHelper.Crear(empleadoRepository.GetListEmpleadoTipoProfesional());
        }

        // Método para asignar BodegaId basado en CategoriaHabitacionNombre
        public void AsignarBodegaId()
        {
            string clienteSetting = _configuration?["Cliente"] ?? string.Empty;
            BodegaId = (clienteSetting == "SS") ? 8 : 14;

            switch (CategoriaHabitacionNombre)
            {
                case "Hospitalización Estandar":
                case "Neonatos":
                case "Emergencia":
                case "Sala de Operaciones":
                case "Sala de Recuperacion":
                case "Intensivo":
                default:
                    BodegaId = (clienteSetting == "SS") ? 8 : 14;
                    break;
            }
        }
    }
    public class HonorarioDetalleDto
    {
        public int Id { get; set; }
        public string NombreMedico { get; set; }
        public decimal Monto { get; set; }
    }
}