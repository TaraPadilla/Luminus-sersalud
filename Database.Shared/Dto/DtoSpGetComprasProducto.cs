using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Dto
{
    public class DtoSpGetComprasProducto
    {
        [Key]
        public int Item { get; set; }
        public int DetalleCompraId { get; set; }
        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; }
        public int UnidadMedidaCompraId { get; set; }
        public string UnidadMedidaCompraNombre { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal PrecioCompra { get; set; }
    }
}
