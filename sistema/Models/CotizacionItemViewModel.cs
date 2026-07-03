using System.Collections.Generic;

namespace sistema.Models
{
    public class CotizacionItemViewModel
    {
        public int ProductoInventarioId { get; set; }
        public int ProductoId { get; set; }
        public string CodigoReferencia { get; set; }
        public string ProductoNombre { get; set; }
        public int CantidadTrasladada { get; set; }

        // Precios por proveedor
        public Dictionary<string, decimal> Proveedores { get; set; }

        // NUEVO: Cantidades por proveedor (para compra dividida)
        public Dictionary<string, int> Cantidades { get; set; }

        // Proveedor “principal” (mayor cantidad, o ganador único)
        public string ProveedorPrincipal { get; set; }
    }
}
