using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace sistema.Models
{
    public class CitaViewModel
    {

        // #region DATOS UTILIZADOS PDF
        // public string EstablecimientoImagenLogo { get; set; }
        // public string EstablecimientoImagenFirma { get; set; }
        // public string EstablecimientoDireccion { get; set; }
        // public string EstablecimientoTelefono { get; set; }
        // public string EstablecimientoCorreoElectronico { get; set; }
        // #endregion
        public SelectList HabitacionesSelectList { get; set; }



        public int? CitaId { get; set; }
        public int? PacienteId { get; set; }
        public string PacienteNombre { get; set; }
        public int? DepartamentoId { get; set; }
        public int? MunicipioId { get; set; }


        //public SelectList PacientesSelectListItem { get; set; }
        public int? PacienteEdad { get; set; }
        public int? EspecialidadId { get; set; }

        public string EspecialidadText { get; set; }
        public string EmpleadoText { get; set; }
        public string Coex { get; set; }

        public string PersonText { get; set; }
        public string EsMenorDeEdadText { get; set; }
        public SelectList EspecialidadSelectListItem { get; set; }
        public int? HabitacionId { get; set; }

        public SelectList HabitacionSelectListItem { get; set; }
        public int? CategoriaHabitacionId { get; set; }

        public SelectList CategoriaHabitacionSelectListItem { get; set; }

        public int? EmpleadoId { get; set; }
        public SelectList DoctoresSelectListItem { get; set; }
        public int? SucursalId { get; set; }
        public SelectList SucursalesSelectListItem { get; set; }
        public DateTime FechaHora { get; set; }
        public DateTime FechaHora2 { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        public string Motivo { get; set; }
        public string NombreEncargado { get; set; }
        public string DPIEncargado { get; set; }

        public string EstadoCita { get; set; }
        public string CodigoCita { get; set; }
        public string CodigoAutorizacion { get; set; }
        public int DuracionTotalHoras { get; set; }
        public int DuracionTotalMinutos { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaRegistro { get; set; }

        //Tipo de atencion
        public string CitaTipoAtencion { get; set; }

        public int? SexoId { get; set; }
        public string Telefono { get; set; }
        public string no_IGGS { get; set; }
        public string nombrePacienteSeleccionado { get; set; }
        public string dpiPacienteSeleccionado { get; set; }
        public string NumeroTurno { get; set; }
        public string EstadoTurno { get; set; }
        public string NivelPrioridadCita { get; set; }

        ////Datos de origen y Etnia de un Paciente
        public string EtniaPaciente { get; set; }
        public string OrigenPaciente { get; set; }
        public string ReligionPaciente { get; set; }

        // Datos básicos
        public string AcompananteNombre { get; set; }
        public string AcompananteTelefono { get; set; }
        public string AcompananteDPI { get; set; }

        // Dirección y contacto
        public string AcompananteDireccion { get; set; }
        public string AcompananteCorreo { get; set; }

        // Información laboral
        public string AcompananteOcupacion { get; set; }
        public string AcompananteEmpresa { get; set; }
        public string AcompananteRelacion { get; set; }

        public string AcompananteTelefonoEmpresa { get; set; }
        public string AcompananteDireccionEmpresa { get; set; }

        // Datos adicionales
        public string AcompananteTipoIdentificacion { get; set; }  // Ej. DPI, pasaporte, etc.

        // Fechas y cálculos
        public DateTime? AcompananteFechaNacimiento { get; set; }
        public string AcompananteEdad { get; set; }  // Calculada en frontend (readonly)

        public DateTime? AcompananteFechaIngreso { get; set; }
        public string AcompananteAntiguedad { get; set; }  // Calculada en frontend (readonly)

        public string ResponsableNit { get; set; }
        public string ResponsableNombre { get; set; }
        public string ResponsableDireccion { get; set; }
        public string ResponsableCorreo { get; set; }
        public string ResponsableTelefono { get; set; }
        public string ResponsableDPI { get; set; }
        public string ResponsableNacionalidad { get; set; }
        public string ResponsableOcupacion { get; set; }

        public string ResponsablePasaporte { get; set; }
        public string AcudienteNombre { get; set; }
        public string AcudienteTelefono { get; set; }
        public string RelacionAcudiente { get; set; }

        public string Correo { get; set; }

        public string Email { get; set; }

        public string Publicidad { get; set; }
        public DateTime ContadorCitaAgendada { get; set; }

        //Direccion
        public string Direccion { get; set; }


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

        public string DepartamentoNombre { get; set; }
        public string MunicipioNombre { get; set; }


        public string Anestesista { get; set; }
        public string PrimerAyudante { get; set; }
        public string SegundoAyudante { get; set; }
        public string Instrumentista { get; set; }
        public string Circulante { get; set; }

        public string Procedimiento { get; set; }

        public IList<Citas> Citas { get; set; }

        public int DepartentoId { get; set; }
        public SelectList DepartamentosSelectListItem { get; set; }

        public List<CitaServicioAgregadoViewModel> Servicios { get; set; }
        public List<CitaExamenesAgregadosViewModel> Examenes { get; set; }

        public void Init(
            ICitas citasRepository,
            IPacientes pacientesRepository,
            IEmpleado empleadoRepository,
            IServicio servicioRepository,
            ISucursal sucursalRepository
            )
        {

            EspecialidadSelectListItem = new SelectList(citasRepository.GetEspecialidadesList(), "Id", "NombreEspecialidad");
            HabitacionSelectListItem = new SelectList(citasRepository.GetHabitacionesList(6), "Id", "NombreNumeroHabitacion");
            CategoriaHabitacionSelectListItem = new SelectList(citasRepository.GetHabitacionesCategoriasList(), "Id", "NombreCategoria");

            //PacientesSelectListItem = new SelectList(pacientesRepository.GetList(), "Nombre", "PacienteWithDPI");
            //PacientesSelectListItem = new SelectList(pacientesRepository.GetList(), "Id", "PacienteWithDPI");
            //DepartamentosSelectListItem = new SelectList(pacientesRepository.GetListDepartamentos(), "Id", "NombreDepartamento");
            DoctoresSelectListItem = new SelectList(empleadoRepository.GetListEmpleadoTipoProfesional().OrderBy(e => e.Id), "Id", "NombreYApellidos");
            SucursalesSelectListItem = new SelectList(sucursalRepository.GetList(), "Id", "NombreSucursal");
        }
        public string Dia { get; set; }
        public string FechaInicio { get; set; }
        public string Hora { get; set; }
        public string CodigoSeguro { get; set; }


        public int? PrimerAyudanteId { get; set; }
        public int? SegundoAyudanteId { get; set; }
        public int? AnestesistaId { get; set; }
        public int? InstrumentistaId { get; set; }
        public int? CirculanteId { get; set; }

        public List<string> Horas = new List<string>
        {

            "08:00",
            "08:15",
            "08:30",
            "08:45",
            "09:00",
            "09:15",
            "09:30",
            "09:45",
            "10:00",
            "10:15",
            "10:30",
            "10:45",
            "11:00",
            "11:15",
            "11:30",
            "11:45",
            "12:00",
            "12:15",
            "12:30",
            "12:45",
            "13:00",
            "13:15",
            "13:30",
            "13:45",
            "14:00",
            "14:15",
            "14:30",
            "14:45",
            "15:00",
            "15:15",
            "15:30",
            "15:45",
            "16:00",
            "16:15",
            "16:30",
            "16:45",
            "17:00",
            "17:15",
            "17:30",
            "17:45"
        };
        public List<CitasServicio> servicios;
    }



    public class CitaServicioExistentePrecioViewModel
    {
        public int PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal PrecioValor { get; set; }
        public string PrecioNombreValor { get; set; }

        public string PrecioNombreMostrar
        {
            get
            {
                return $"{PrecioNombre} - {PrecioValor}";
            }
        }
    }
    public class CitaExamenesAgregadosViewModel
    {
        public int? Id { get; set; }
        public int ExamenId { get; set; }
        public string ExamenNombre { get; set; }
        //public int ServicioDuracionHoras { get; set; }
        //public int ServicioDuracionMinutos { get; set; }
        public int Cantidad { get; set; }
        public int PrecioId { get; set; }
        //public string PrecioNombre { get; set; }
        public decimal PrecioValor { get; set; }
        public decimal PrecioValorCubiertoSeguro { get; set; }
        public decimal PrecioValorCopago { get; set; }
        public bool Nuevo { get; set; }
    }

    


}
