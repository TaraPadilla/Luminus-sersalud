using Database.Shared.Models;
using sistema.Models;
using System.Collections.Generic;

namespace farmamest.Models
{
    public class UltimaCompraProductoViewModel
    {
        public int ProductoId { get; set; }
        public string CodigoReferencia { get; set; }
        public string NombreProducto { get; set; }
        public string UnidadCompra { get; set; }
        public string UnidadVenta { get; set; }
        public decimal? Precio { get; set; }
        public int? ProveedorId { get; set; }
        public int? TipoCompra { get; set; }
        public List<ProductoEquivalenciaViewModel> ProductoEquivalencias { get; set; }
    }
}
