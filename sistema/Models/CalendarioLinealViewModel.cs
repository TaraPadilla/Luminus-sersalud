using System.Collections.Generic;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace sistema.Models
{
    public class CalendarioLinealViewModel
    {
        public int? SucursalId { get; set; }
        public SelectList SucursalesSelectList { get; set; }
        public int? EmpleadoId { get; set; }
        public SelectList EmpleadosSelectList { get; set; }
        public int? EspecialidadId { get; set; }
        public SelectList EspecialidadSelectListItem { get; set; }
        public string Fecha { get; set; }
        public bool FechaBloqueada { get; set; }
        public string Motivo { get; set; }
        public int? ServicioId { get; set; }
        public int CitasNoPagadas { get; internal set; }
        public int CitasEnEspera { get; internal set; }
        public int CitasFinalizadas { get; internal set; }
        public string FechaInicio { get; internal set; }
        public string FechaFin { get; internal set; }

        public SelectList ServiciosSelectList { get; set; } // SelectList para los servicios
        public List<string> Horas { get; set; } = new List<string>();

        public IList<Citas> Citas { get; set; }

        public string FechaYHoras => $"{Fecha}";

        public CalendarioLinealViewModel()
        {
            for (int h = 0; h < 24; h++)
            {
                Horas.Add($"{h:D2}:00");
                Horas.Add($"{h:D2}:30");
            }
        }

        public void Init(ISucursal sucursalRepository, IEmpleado empleadoRepository, ICitas citasRepository)
        {
            SucursalesSelectList = new SelectList(sucursalRepository.GetList(), "Id", "NombreSucursal");
            EmpleadosSelectList = new SelectList(empleadoRepository.GetListEmpleadoTipoProfesional(), "Id", "NombreYApellidos");
            EspecialidadSelectListItem = new SelectList(citasRepository.GetEspecialidadesList(), "Id", "NombreEspecialidad");
            ServiciosSelectList = new SelectList(citasRepository.GetServiciosList(), "Id", "NombreServicio"); // Carga la lista de servicios
        }


        public int? HabitacionId { get; set; }
        public List<SelectListItem> HabitacionesSelectList { get; set; }
    }
}