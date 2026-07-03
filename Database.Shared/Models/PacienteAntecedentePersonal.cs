using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PacienteAntecedentePersonal
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public int AntecedentePersonalId { get; set; }
        public AntecedentePersonal AntecedentePersonal { get; set; }
        public bool PresentoAntecedente { get; set; }
        public DateTime? FechaAntecedente { get; set; }
    }
}
