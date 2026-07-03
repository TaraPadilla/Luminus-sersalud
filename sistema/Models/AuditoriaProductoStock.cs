using System;

namespace farmamest.Models
{
    public class AuditoriaProductoStock
    {
        public int IdProducto { get; set; }
        public int IdProductoInventario { get; set; }
        public int? StockIngresado { get; set; }
        public string CodigoReferencia { get; set; }
        public string NombreProducto { get; set; }
        public string LoteProducto { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaRecepcionLote { get; set; }
        public decimal PrecioCosto { get; set; }
    }
}
