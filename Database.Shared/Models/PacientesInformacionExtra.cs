using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class PacientesInformacionExtra
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }
        public int? AntecedentePersonalId { get; set; }
        public AntecedentePersonal AntecedentePersonal { get; set; }
        public bool? Presento { get; set; }
        public DateTime? Fecha { get; set; }
        public string Descripcion { get; set; }
        public int? Posicion { get; set; }
        public int? Seccion { get; set; }
        public string Titulo { get; set; }
        public string Nombre { get; set; }
        public int? Numero { get; set; }
        public string Variable1 { get; set; }
        public string Variable2 { get; set; }
        public string Variable3 { get; set; }
        public string Variable4 { get; set; }
        public string Base64 { get; set; }
    }
}