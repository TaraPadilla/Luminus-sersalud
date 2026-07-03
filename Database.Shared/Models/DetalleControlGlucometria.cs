using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class DetalleControlGlucometria
    {
        public int Id { get; set; }
        public int ControlGlucometriaId { get; set; }
        public ControlGlucometria ControlGlucometria { get; set; }
        public bool Aplicado { get; set; }
        public DateTime? FechaAplicacion { get; set; }
        /// <summary>
        /// Persona que aplica el control
        /// </summary>
        public string UserId { get; set; }
        public User User { get; set; }
        public bool Eliminado { get; set; }
        public string ProfesionalId { get; set; }
        public User Profesional { get; set; }
    }
}

