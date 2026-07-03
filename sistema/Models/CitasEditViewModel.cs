using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.Models;
using Database.Shared.IRepository;

namespace sistema.Models
{
    public class CitasEditViewModel : CitaBaseViewModel
    {
        
        public override void Init(ICitas citasRepository, IPacientes pacientesRepository, IEmpleado empleadoRepository, IServicio servicioRepository)
        {
            // EspecialidadSelectListItem = new SelectList(citasRepository.GetEspecialidadesList(), "Id", "NombreEspecialidad");
            // ClientesSelectListItem = new SelectList(clienteRepository.GetList(), "Id", "Nombre");
            // DoctoresSelectListItem = new SelectList(empleadoRepository.GetList(), "Id", "NombreYApellidos");
            // ServiciosSelectListItem = new SelectList(servicioRepository.GetList(), "Id", "NombreYApellidos");
            base.Init(citasRepository, pacientesRepository, empleadoRepository, servicioRepository);

        }
        public int Id
        {
            get { return Cita.Id; }
            set { Cita.Id = value; }
        }
    }
}