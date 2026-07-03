using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PacienteArchivo
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public string NombreArchivo { get; set; }
        public string UrlArchivo { get; set; }
    }
}
