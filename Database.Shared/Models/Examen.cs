using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class Examen
    {
        public Examen()
        {
            Ventas = new List<Venta>();
            //VentasLabs = new List<VentasLab>();
            DetalleExamenes = new List<DetalleExamen>();
            HospitalizacionesExamenes = new List<HospitalizacionExamen>();
            EstadoEstudio = "Sin Crear";
        }

        public int Id { get; set; }
        public int? PacienteId { get; set; }
        public int? MedicosId { get; set; }
        public int? ClinicaId { get; set; }
        // public int ExamenLabClinicoId {get;set;}
        public int? EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }
        public string EstadoEstudio { get; set; } 
        public string UrlEstudio { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int? EstadoExamenId { get; set; }
        public DateTime FechaRealizacion { get; set; }
        public bool TipoA { get; set; }
        public bool TipoB { get; set; }
        public bool TipoC { get; set; }
        public string DoctorReferido { get; set; }
        public string ClinicaReferida { get; set; }
        public string NumeroOrden { get; set; }

        public bool? PagoCredito { get; set; }
        public bool Eliminado { get; set; }

        public string UsuarioSolicita { get; set; }
        public string UsuarioIngresa { get; set; }


        public Paciente Paciente { get; set; }
        // public ExamenLabClinico ExamenLabClinico {get;set;}
        // public Empleado Empleado {get;set;}
        public EstadoExamen EstadoExamen { get; set; }
        public Medicos Medicos { get; set; }
        public Clinica Clinicas { get; set; }
        public ICollection<Venta> Ventas { get; set; }
        //public ICollection<VentasLab> VentasLabs { get; set; }
        public ICollection<DetalleExamen> DetalleExamenes { get; set; }
		public ICollection<HospitalizacionExamen> HospitalizacionesExamenes { get; set; }
        public int? ConsultaId { get;  set; }
    }
}