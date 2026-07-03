using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class DetalleCompraUbicacionPrecio
    {
        public int Id { get; set; }
        public int? DetalleCompraUbicacionId { get; set; }
        public DetalleCompraUbicacion DetalleCompraUbicacion { get; set; }
        public int? PrecioId { get; set; }
        public Precio Precio { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioValor { get; set; }

    }
}