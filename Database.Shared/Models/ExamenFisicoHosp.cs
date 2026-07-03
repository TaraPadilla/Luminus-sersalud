using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class ExamenFisicoHosp
    {
        public ExamenFisicoHosp()
        {
            ExamenesFisicosHospDatos = new List<ExamenFisicoHospDato>();
        }
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string UsuarioToma { get; set; }
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        public string Observaciones { get; set; }
        public ICollection<ExamenFisicoHospDato> ExamenesFisicosHospDatos { get; set; }


        public bool Autorizado { get; set; } = false;
        public string? UsuarioAutoriza { get; set; }
        public DateTime? FechaAutorizacion { get; set; }
    }
}