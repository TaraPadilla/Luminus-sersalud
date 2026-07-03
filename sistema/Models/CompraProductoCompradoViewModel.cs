using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class CompraProductoCompradoViewModel
    {
        public int? Id { get; set; }
        public int Item { get; set; }
        public int ProductoId { get; set; }
        public string ProductoCodigo { get; set; }
        public string NombreProducto { get; set; }

        //POLITICAS DE DEVOLUCION CAMBIO
        public string PoliticaDevolucionProducto { get; set; }
        public int? PoliticaDevolucionPersonalizadaDias { get; set; }

        //POLITICAS DE DEVOLUCION VENCIMIENTO
        public string PoliticaDevolucionVencimientoProducto { get; set; }
        public int? PoliticaDevolucionVencimientoPersonalizadaDias { get; set; }

        //CREDITO
        public string CreditoProducto { get; set; }
        public int? CreditoPersonalizadoDias { get; set; }

        public int UnidadMedidaCompraId { get; set; }
        public UnidadMedidaCompraViewModel UnidadMedidaCompra { get; set; }
        public List<UnidadMedidaVentaCompraViewModel> UnidadesMedidaVenta { get; set; }
        public string Lote { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal Total { get; set; }
        public string FechaVencimiento { get; set; }
        public bool Nuevo { get; set; }
        public bool Eliminado { get; set; }
    }
    public class UnidadMedidaVentaCompraViewModel
    {
        public int Id { get; set; }
        public string NombreUnidad { get; set; }
        public string Equivalencia { get; set; }
        public decimal CantidadEquivalenteDestino { get; set; }
        public List<PrecioVentaUnidadVentaCompraViewModel> Precios { get; set; }
    }

    public class UnidadMedidaCompraViewModel
    {
        public int? Id { get; set; }
        public string NombreUnidad { get; set; }
        public string Abreviatura { get; set; }
    }
    public class PrecioVentaUnidadVentaCompraViewModel
    {
        public int PrecioId { get; set; }
        public decimal Valor { get; set; }
    }
}
