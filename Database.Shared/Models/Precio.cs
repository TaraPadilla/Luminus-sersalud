using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Database.Shared.Models
{
    public class Precio
    {
        public Precio()
        {
            ProductosInventarioPrecios = new List<ProductoInventarioPrecio>();
            DetalleComprasUnidadesVentaPrecio = new List<DetalleCompraUnidadVentaPrecio>();
             ServiciosPrecios = new List<ServicioPrecio>();
        }
        public int Id { get; set; }
        public string NombrePrecio { get; set; }
        public bool Eliminado { get; set; }
        public ICollection<ProductoInventarioPrecio> ProductosInventarioPrecios { get; set; }
        public ICollection<DetalleCompraUnidadVentaPrecio> DetalleComprasUnidadesVentaPrecio { get; set; }
        public ICollection<ServicioPrecio> ServiciosPrecios { get; set; }
    }
}