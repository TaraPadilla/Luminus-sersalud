using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Models
{
    public class AuditoriaNuevoSP
    {
        [Key]
        public int? IdProductoInventario { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoReferencia { get; set; }
        public string Lote { get; set; }
        public DateTime? FechaRecepcionLote { get; set; }
        public decimal? Stock { get; set; }
        public decimal? PrecioCosto { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string NombreUnidadCompra { get; set; }
        public string NombreUnidadVenta { get; set; }
        public int? PresentacionProductoId { get; set; }
        public int? PresentacionProductoId2 { get; set; }
        public int? PresentacionProductoId3 { get; set; }

        public int? PresentacionProductoId4 { get; set; }

        public int? PresentacionProductoId5 { get; set; }
    }
}
