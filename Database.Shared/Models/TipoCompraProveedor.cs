using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class TipoCompraProveedor
    {
        public int Id { get; set; }
        public int TipoCompraId { get; set; }
        public TipoCompra TipoCompra { get; set; }
        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; }
    }
}
