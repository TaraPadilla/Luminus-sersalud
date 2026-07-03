using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class UnidadMedidaVenta
    {
        public UnidadMedidaVenta()
        {
            ProductosEquivalencias = new List<ProductoEquivalencia>();
            DetalleComprasUnidadesVentaPrecio = new List<DetalleCompraUnidadVentaPrecio>();
        }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public ICollection<ProductoEquivalencia> ProductosEquivalencias { get; set; }
        public ICollection<DetalleCompraUnidadVentaPrecio> DetalleComprasUnidadesVentaPrecio { get; set; }
    }
}
