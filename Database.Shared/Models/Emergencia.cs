using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class Emergencia
    {
        public Emergencia()
        {
            EmergenciaDetalles = new List<EmergenciaDetalle>();
        }

        public int Id { get; set; }
        public int? PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public int? SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }
        public int? HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        public int? EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }
        public DateTime FechaEmergencia { get; set; }
        public DateTime? FechaUltimaModificacion { get; set; }
        public string Responsable { get; set; }
        public bool Ingresada { get; set; }
        public bool Eliminado { get; set; }

        public bool Pagado { get; set; }

        public string Observaciones { get; set; }

        public ICollection<EmergenciaDetalle> EmergenciaDetalles { get; set; }
    }
}