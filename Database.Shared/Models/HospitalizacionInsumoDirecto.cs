using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace Database.Shared.Models
{
    [Table("HospitalizacionInsumosDirectos")]
    public class HospitalizacionInsumoDirecto
    {
        [Key]
        public int Id { get; set; }
 
        [Required]
        public int HospitalizacionId { get; set; }
 
        [Required]
        public int ProductoId { get; set; }
 
        public int UnidadMedidaVentaId { get; set; }
 
        public int? PrecioId { get; set; }
 
        public decimal PrecioValor { get; set; }
 
        public int Cantidad { get; set; }
 
        public string Indicaciones { get; set; }
 
        public string ViaAdministracion { get; set; }
 
        public string FrecuenciaAdministracion { get; set; }
 
        public string FechaHoraAplicacionManual { get; set; }
 
        public string UsuarioCreaId { get; set; }
 
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
 
        public bool Eliminado { get; set; } = false;
 
        // ── Navegación ──
        [ForeignKey("HospitalizacionId")]
        public virtual Hospitalizacion Hospitalizacion { get; set; }
 
        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; }
 
        [ForeignKey("UnidadMedidaVentaId")]
        public virtual UnidadMedidaVenta UnidadMedidaVenta { get; set; }
 
        public virtual ICollection<HospitalizacionInsumoDirectoAplicacion> Aplicaciones { get; set; }
    }
}