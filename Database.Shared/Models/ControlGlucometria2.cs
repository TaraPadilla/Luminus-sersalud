using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Models
{
    public class ControlGlucometria2
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; } = DateTime.Now;
        public string GMT { get; set; }
        public string Insulina { get; set; }
        public string Unidades { get; set; }
        public string Medicamento { get; set; }
        public decimal Dosis { get; set; }
        public string Firma { get; set; }
        public int HospitalizacionId { get; set; }
        public Hospitalizacion Hospitalizacion { get; set; }
        public ICollection<DetalleControlGlucometria2> DetalleControlGlucometria2 { get; set; }


        public bool Autorizado { get; set; }
        public string UsuarioAutoriza { get; set; }
        public DateTime? FechaAutorizacion { get; set; }
    }
}
