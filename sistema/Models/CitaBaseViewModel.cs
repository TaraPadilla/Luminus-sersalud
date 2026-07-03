using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.Models;
using Database.Shared.IRepository;
using System.ComponentModel.DataAnnotations;

namespace sistema.Models
{
    public class CitaBaseViewModel
    {
        public CitaBaseViewModel()
        {
            Cita.FechaInicio = DateTime.Now;
        }

        public SelectList EspecialidadSelectListItem { get; set; }
        public SelectList PacientesSelectListItem { get; set; }
        public SelectList DoctoresSelectListItem { get; set; }
        public SelectList ServiciosSelectListItem { get; set; }
        public Citas Cita { get; set; } = new Citas();
        public DateTime HoraYFecha { get; set; }
        public decimal CargoPenalizacion { get; set; }

        [Required]
        public int ClienteId { get; set; }
        public virtual void Init(ICitas citasRepository, IPacientes pacientesRepository, IEmpleado empleadoRepository, IServicio servicioRepository)
        {
            EspecialidadSelectListItem = new SelectList(citasRepository.GetEspecialidadesList(), "Id", "NombreEspecialidad");
            PacientesSelectListItem = new SelectList(pacientesRepository.GetList(), "Id", "Nombre");
            DoctoresSelectListItem = new SelectList(empleadoRepository.GetList(), "Id", "NombreYApellidos");
            ServiciosSelectListItem = new SelectList(servicioRepository.GetList(), "Id", "NombreServicio");
        }

        // public int Id
        // {
        //     get { return Cita.Id; }
        //     set { Cita.Id = value; }
        // }
    }
}