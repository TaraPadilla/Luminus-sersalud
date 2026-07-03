using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class ProductoInventario
    {
        public ProductoInventario()
        {
            ProductosInventarioPrecios = new List<ProductoInventarioPrecio>();
            DetalleTrasladoProductos = new List<DetalleTrasladoProductos>();
        }
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int? CompraId { get; set; }
        public Compra Compra { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public UnidadMedidaVenta UnidadMedidaVenta { get; set; }
        public UnidadMedidaCompra UnidadMedidaCompra { get; set; }
        public int? UnidadMedidaCompraId { get; set; }
        public int? BodegaId { get; set; }
        public Bodega Bodega { get; set; }
        public string Lote { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Stock { get; set; }
        public decimal StockMinimo { get; set; }
        public bool Eliminado { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrecioCosto { get; set; }
        public DateTime? FechaVencimientoArticuloCompra { get; set; }
        public DateTime? FechaRecepcionLote { get; set; }

        //Campos politica de devolucion CAMBIO
        public bool ManejaPoliticaDevolucion { get; set; }
        public bool ManejaPoliticaDevolucionProveedor { get; set; }
        public bool ManejaPoliticaDevolucionPersonalizada { get; set; }
        public int? PoliticaDevolucionPersonalizadaDias { get; set; }

        //Campos politica de devolucion VENCIMIENTO
        public bool ManejaPoliticaDevolucionVencimiento { get; set; }
        public bool ManejaPoliticaDevolucionVencimientoProveedor { get; set; }
        public bool ManejaPoliticaDevolucionVencimientoPersonalizada { get; set; }
        public int? PoliticaDevolucionVencimientoPersonalizadaDias { get; set; }

        //Campos CREDITO
        public bool ManejaCredito { get; set; }
        public bool ManejaCreditoProveedor { get; set; }
        public bool ManejaCreditoPersonalizado { get; set; }
        public int? CreditoPersonalizadoDias { get; set; }
        //Datos de facturacion
        public bool Facturado { get; set; }

        public ICollection<ProductoInventarioPrecio> ProductosInventarioPrecios { get; set; }
        public ICollection<DetalleTrasladoProductos> DetalleTrasladoProductos { get; set; }
    }
}
