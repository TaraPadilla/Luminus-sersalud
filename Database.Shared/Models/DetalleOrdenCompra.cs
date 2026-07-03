using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Database.Shared.Models
{
    public class DetalleOrdenCompra
    {
        public DetalleOrdenCompra()
        {
            DetalleOrdenCompraUbicaciones = new List<DetalleOrdenCompraUbicacion>();
        }
        public int Id { get; set; }
        public int OrdenCompraId { get; set; }
        public OrdenCompra OrdenCompra { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int? UnidadMedidaCompraId { get; set; }
        public UnidadMedidaCompra UnidadMedidaCompra { get; set; }
        public int Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseImponible { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioTotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Impuesto { get; set; }
        public bool Eliminado { get; set; }
        public ICollection<DetalleOrdenCompraUbicacion> DetalleOrdenCompraUbicaciones { get; set; }
    }
}