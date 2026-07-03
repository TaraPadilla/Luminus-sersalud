using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PacienteHistorial
    {
        public int Id { get; set; }
        public int? PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public int? AccionPacienteId { get; set; }
        public AccionPaciente AccionPaciente { get; set; }
        public DateTime? Fecha { get; set; }
        public string MotivoRetiro { get; set; }
        public bool? VolverAContactar { get; set; }
        public DateTime? FechaContacto { get; set; }
    }
}
