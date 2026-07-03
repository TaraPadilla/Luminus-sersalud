using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Database.Shared.Models
{
    public class DetalleCompra
    {
        public DetalleCompra()
        {
            DetalleComprasUnidadesVentaPrecio = new List<DetalleCompraUnidadVentaPrecio>();
            DetalleComprasUbicaciones = new List<DetalleCompraUbicacion>();
        }
        public int Id { get; set; }
        public int CompraId { get; set; }
        public int ProductoId { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int? UnidadMedidaCompraId { get; set; }
        public UnidadMedidaCompra UnidadMedidaCompra { get; set; }
        public int Cantidad { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal Precio { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal BaseImponible { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal PrecioTotal { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal Impuesto { get; set; }
        public string Lote { get; set; }
        public Producto Producto { get; set; }
        public Compra Compra { get; set; }
        public bool Eliminado { get; set; }

        //Campos politicas de devolucion CAMBIO
        public bool ManejaPoliticaDevolucion { get; set; }
        public bool ManejaPoliticaDevolucionProveedor { get; set; }
        public bool ManejaPoliticaDevolucionPersonalizada { get; set; }
        public int? PoliticaDevolucionPersonalizadaDias { get; set; }

        //Campos politica de devolucion VENCIMIENTO
        public bool ManejaPoliticaDevolucionVencimiento { get; set; }
        public bool ManejaPoliticaDevolucionVencimientoProveedor { get; set; }
        public bool ManejaPoliticaDevolucionVencimientoPersonalizada { get; set; }
        public int? PoliticaDevolucionVencimientoPersonalizadaDias { get; set; }

        //Campos credito
        public bool ManejaCredito { get; set; }
        public bool ManejaCreditoProveedor { get; set; }
        public bool ManejaCreditoPersonalizado { get; set; }
        public int? CreditoPersonalizadoDias { get; set; }

        //Relacion Colecciones
        public ICollection<DetalleCompraUnidadVentaPrecio> DetalleComprasUnidadesVentaPrecio { get; set; }
        public ICollection<DetalleCompraUbicacion> DetalleComprasUbicaciones { get; set; }

    }
}