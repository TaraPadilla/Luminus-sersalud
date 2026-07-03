using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    [Table("HospitalizacionGastosAdministrativos")]
    public class HospitalizacionGastoAdministrativo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HospitalizacionId { get; set; }

        [ForeignKey("HospitalizacionId")]
        public Hospitalizacion Hospitalizacion { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,4)")]
        public decimal PorcentajeAplicado { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,2)")]
        public decimal Monto { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;
    }
}