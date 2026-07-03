using System.Collections.Generic;
namespace sistema.Models
{
    public class CotizacionViewModel
    {
        public List<ProductoTrasladoViewModel> Productos { get; set; } = new List<ProductoTrasladoViewModel>();
        public List<string> Proveedores { get; set; } = new List<string>();
        public string ProveedorPrincipal { get; set; }
        public Dictionary<string, string> ProveedorPrincipalPorItem { get; set; } = new Dictionary<string, string>();

        // NUEVO: tipo de compra que mandamos desde el JS ("CompraDividida" | "ProveedorUnico")
        public string TipoCompra { get; set; }

    }


    public class ProductoTrasladoViewModel
    {
        public string ProductoId { get; set; }

        public string CodigoReferencia { get; set; }
        public string ProductoNombre { get; set; }
        public int CantidadTrasladada { get; set; }
        public Dictionary<string, int> CantidadesProveedores { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> PreciosProveedores { get; set; } = new Dictionary<string, decimal>();
    }


}