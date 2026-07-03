using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class HospitalizacionRecetaDetalle
    {
        public int Id { get; set; }
        public int HospitalizacionRecetaId { get; set; }
        public HospitalizacionReceta HospitalizacionReceta { get; set; }
        public bool Aplicado { get; set; }
        /// <summary>
        /// Usuario que aplica la receta
        /// </summary>
        public string UsuarioId { get; set; }
        public User Usuario { get; set; }
        public DateTime? FechaHoraAplicada { get; set; }
        public bool Eliminado { get; set; }
    }
}
