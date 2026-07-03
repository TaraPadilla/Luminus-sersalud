using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class DetalleCompraUbicacion
    {
        public DetalleCompraUbicacion()
        {
            DetalleCompraUbicacionPrecios = new List<DetalleCompraUbicacionPrecio>();
        }
        public int Id { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public UnidadMedidaVenta UnidadMedidaVenta { get; set; }
        public int? DetalleCompraId { get; set; }
        public DetalleCompra DetalleCompra { get; set; }
        public int? BodegaId { get; set; }
        public Bodega Bodega { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }
        public ICollection<DetalleCompraUbicacionPrecio> DetalleCompraUbicacionPrecios { get; set; }
    }
}