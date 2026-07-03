using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class ProductoEquivalencia
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int? UnidadMedidaCompraId { get; set; }
        public UnidadMedidaCompra UnidadMedidaCompra { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public UnidadMedidaVenta UnidadMedidaVenta { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadEquivalenteDestino { get; set; }
        public bool Eliminada { get; set; }
    }
}
