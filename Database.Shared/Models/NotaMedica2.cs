using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class NotaMedica2
    {
        public int Id { get; set; }
        public string HistoriaProblema { get; set; }
        public string Sintomas { get; set; }
        public string Diagnostico { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string ProfesionalId { get; set; }
        public User Profesional { get; set; }
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }


        public bool Autorizado { get; set; }
        public string UsuarioAutoriza { get; set; }
        public DateTime? FechaAutorizacion { get; set; }

        /// <summary>Ingreso, Traslado, Recepcion, Egreso — para expediente completo.</summary>
        public string TipoNota { get; set; }
    }
}
