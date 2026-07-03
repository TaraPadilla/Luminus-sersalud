using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System;

namespace sistema.Models
{
    public class RealizarExamenLabClinicoViewModel
    {

        public RealizarExamenLabClinicoViewModel()
        {
        }

        public Examen Examen {get;set;} = new Examen();
        public SelectList ListaExamenesDisponibles {get;set;}
        public SelectList ListaMedicosDisponibles {get;set;}
        public SelectList ListaClinicasDisponibles {get;set;}
        public SelectList ListaPaciente {get;set;}
        public SelectList ListaEstados {get;set;}
        public SelectList ListaFormaPagos {get;set;}

        // Nuevas propiedades
        public int? ExamenId { get; set; }
        public int? HospitalizacionId { get; set; }
        // Datos del Paciente
        public string PacienteNombre { get; set; }
        public string PacienteDPI { get; set; }
        public DateTime? PacienteFechaNacimiento { get; set; }
        public int? PacienteEdad { get; set; }

        // Datos de la Habitación
        public string NombreHabitacion { get; set; }

        // Datos de la Cita (Seguro)
        public string NombreSeguro { get; set; }

        // Datos del Empleado (Médico)
        public string NombreMedico { get; set; }

        public void Init(ILaboratorioClinico laboratorioClinico, IEmpleado empleadoRepository, IPacientes pacienteRepository, IEnvio envioRepository)
        {
            ListaExamenesDisponibles = new SelectList(laboratorioClinico.GetListExamenesLaboratorio(), "Id", "NombreExamen");
            
            ListaMedicosDisponibles = new SelectList(empleadoRepository.GetListMedicos(), "Id", "Nombres");
            ListaClinicasDisponibles = new SelectList(empleadoRepository.GetListClinicas(), "Id", "NombreClinica");
            
            ListaPaciente = new SelectList(pacienteRepository.GetList(), "Id", "PacienteConIGSS");
            ListaEstados = new SelectList(pacienteRepository.GetListEstadosExamen(), "Id", "Nombre"); 
            ListaFormaPagos = new SelectList(envioRepository.GetListPagos(), "Id", "NombreFormaPago");

        }
        public int Id
        {
            get { return Examen.Id; }
            set { Examen.Id = value; }
        }
    }
} 