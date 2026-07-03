using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    [Table("HospitalizacionInsumosDirectosAplicaciones")]
    public class HospitalizacionInsumoDirectoAplicacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HospitalizacionInsumoDirectoId { get; set; }

        public int Cantidad { get; set; }

        public bool Aplicado { get; set; } = false;

        public DateTime? FechaHoraAplicacion { get; set; }

        public string UsuarioCreaId { get; set; }

        public string UsuarioAplica { get; set; }

        [MaxLength(300)]
        public string? MotivoDevolucion { get; set; }

        // ── Navegación ──
        [ForeignKey("HospitalizacionInsumoDirectoId")]
        public virtual HospitalizacionInsumoDirecto HospitalizacionInsumoDirecto { get; set; }
    }
}
