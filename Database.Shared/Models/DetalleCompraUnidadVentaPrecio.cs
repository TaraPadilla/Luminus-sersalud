using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class DetalleCompraUnidadVentaPrecio
    {
        public int Id { get; set; }
        public int DetalleCompraId { get; set; }
        public DetalleCompra DetalleCompra { get; set; }
        public int UnidadMedidaVentaId { get; set; }
        public UnidadMedidaVenta UnidadMedidaVenta { get; set; }
        public int PrecioId { get; set; }
        public Precio Precio { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorPrecio { get; set; }
    }
}