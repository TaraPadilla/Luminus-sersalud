using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class RegistroAnestesia
    {
        [Key]
        public int Id { get; set; }

        public int HospitalizacionId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string UserId { get; set; }

        /// <summary>JSON con el formulario completo (6 secciones, gráfica, balances, medicamentos).</summary>
        public string DatosJson { get; set; }

        [ForeignKey("HospitalizacionId")]
        public virtual Hospitalizacion Hospitalizacion { get; set; }
    }
}
