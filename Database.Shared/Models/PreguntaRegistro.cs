using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PreguntaRegistro
    {
        public PreguntaRegistro()
        {
            PacientesPreguntasRegistro = new List<PacientePreguntaRegistro>();
        }
        public int Id { get; set; }
        public string Pregunta { get; set; }
        public ICollection<PacientePreguntaRegistro> PacientesPreguntasRegistro { get; set; }
    }
}
