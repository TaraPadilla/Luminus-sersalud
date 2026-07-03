using Database.Shared.Models;
using Database.Shared.Paginacion;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class HomeCitasViewModel
    {
        public int? CitaId { get; set; }
        public string FechaInicio { get; set; }
        public string Hora { get; set; }

        public string Sucursal { get; set; }
        public string Empleado { get; set; }
        public string Paciente { get; set; }
        public string PacienteNombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        public string Asistencia { get; set;}

        public string NumeroTurno { get; set; }

        public List<Servicio> Servicios { get; set; }

        public IList<Citas> citas { get; set; }
        public string Especialidad { get; internal set; }
    }
}
