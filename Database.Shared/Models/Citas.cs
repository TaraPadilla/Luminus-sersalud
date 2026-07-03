using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Database.Shared.Models
{
    public class Citas
    {
        public Citas()
        {
            Consultas = new List<Consulta>();
            CitasServicios = new List<CitasServicio>();
            Prescripciones = new List<Prescripcion>();
        }

        public int Id { get; set; }

        // public int? TipoEspecialidadId {get;set;}
        public int? EspecialidadId { get; set; }
        public int? HabitacionId { get; set; }
        public int? CategoriaHabitacionId { get; set; }
        public string Coex { get; set; }

        public int? PacienteId { get; set; }
        public int? ServicioId { get; set; }
        public int? SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }
        public int? EmpleadoId { get; set; }
        public string UserId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }
        public string Motivo { get; set; }

        public int? Edad { get; set; }
        public string NombreEncargado { get; set; }
        public bool Eliminado { get; set; }
        public bool Finalizada { get; set; }
        public bool EsMenorDeEdad { get; set; }
        public string EstadoCita { get; set; }
        public string NombreMedicoTemporal { get; set; }
        public string NivelPrioridadCita { get; set; }

        public string CodigoDeCita { get; set; }
        public string CodigoAutorizacion { get; set; }
        public bool? Bloqueada { get; set; }
        public DateTime? FechaHoraInicioTurno { get; set; }
        public string EstadoTurno { get; set; }
        public string NumeroTurno { get; set; }
        public string DescripcionTurno { get; set; }
        //Tipo de atencion
        public string CitaTipoAtencion { get; set; } //SEGURO - PRIVADO - IGSS

        // public TipoEspecialidad TipoEspecialidad {get;set;}
        public Especialidad Especialidad { get; set; }
        public Habitacion Habitacion { get; set; }

        public Paciente Paciente { get; set; }
        public Servicio Servicio { get; set; } // duda
        public Empleado Empleado { get; set; }
        public User User { get; set; }

        //se Agrega la relacion con Examenes Fisico ID para poder agregar ExamenesFisicos antes de inicar a la consulta
        public int? ExamenFisicoId { get; set; }

        public ICollection<Consulta> Consultas { get; set; }
        public ICollection<CitasServicio> CitasServicios { get; set; }
        public ICollection<CitasExamenes> CitasExamenes { get; set; }
        public ICollection<Prescripcion> Prescripciones { get; set; }



        public string EspecialidadText
        {
            get { return Especialidad == null ? "N/A" : Especialidad.NombreEspecialidad.ToString(); }
        }
        public string ClienteText
        {
            get { return Paciente == null ? "N/A" : Paciente.Nombre.ToString(); }
        }

        public string PersonText
        {
            get { return User == null || User.Persona == null ? "Root" : User.Persona.NombreYApellidos; }
        }

        public string EmpleadoText
        {
            get { return Empleado == null ? "N/A" : Empleado.NombreYApellidos; }
        }

        public string ServicioText
        {
            get { return Servicio == null ? "N/A" : Servicio.NombreServicio; }
        }


        public string EsMenorDeEdadText
        {
            get { return EsMenorDeEdad ? "Si" : "No"; }
        }

        public string DPIEncargado { get; set; }
        public string ResponsableNit { get; set; }
        public string ResponsableNombre { get; set; }
        public string ResponsableDireccion { get; set; }
        public string ResponsableCorreo { get; set; }
        public string ResponsableTelefono { get; set; }
        public string ResponsableDPI { get; set; }
        public string ResponsablePasaporte { get; set; }
        public string ResponsableNacionalidad { get; set; }
        public string ResponsableOcupacion { get; set; }


        // Nuevos campos - Datos del Padre
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

        public bool Reconsulta { get; set; }

        // Nuevos campos - Datos de la Madre
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

        // Nuevos campos - Datos del Acompanante
        // Datos básicos
        public string AcompananteNombre { get; set; }
        public string AcompananteRelacion { get; set; }

        public string AcompananteTelefono { get; set; }
        public string AcompananteDPI { get; set; }

        // Dirección y contacto
        public string AcompananteDireccion { get; set; }
        public string AcompananteCorreo { get; set; }

        // Información laboral
        public string AcompananteOcupacion { get; set; }
        public string AcompananteEmpresa { get; set; }
        public string AcompananteTelefonoEmpresa { get; set; }
        public string AcompananteDireccionEmpresa { get; set; }

        // Datos adicionales
        public string AcompananteTipoIdentificacion { get; set; }  // Ej. DPI, pasaporte, etc.

        // Fechas y cálculos
        public DateTime? AcompananteFechaNacimiento { get; set; }
        public string AcompananteEdad { get; set; }  // Calculada en frontend (readonly)

        public DateTime? AcompananteFechaIngreso { get; set; }
        public string AcompananteAntiguedad { get; set; }  // Calculada en frontend (readonly)
        public int? ConsultaId { get; set; }
        public List<int> MedicosSecundarios { get; set; } = new List<int>();
        public DateTime? ContadorCitaAgendada { get; set; }
        public DateTime? ContadorCitaIniciada { get; set; }
        public DateTime? ContadorCitaFinalizada { get; set; }


        public string Anestesista { get; set; }
        public string PrimerAyudante { get; set; }
        public string SegundoAyudante { get; set; }
        public string Instrumentista { get; set; }
        public string Circulante { get; set; }

        public string Procedimiento { get; set; }

        public int? PrimerAyudanteId { get; set; }
        public int? SegundoAyudanteId { get; set; }
        public int? AnestesistaId { get; set; }
        public int? InstrumentistaId { get; set; }
        public int? CirculanteId { get; set; }

    }
}