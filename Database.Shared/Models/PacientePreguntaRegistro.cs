using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PacientePreguntaRegistro
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public int PreguntaRegistroId { get; set; }
        public PreguntaRegistro PreguntaRegistro { get; set; }
        public string Respuesta { get; set; }
    }
}
