using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Dto
{
    public class DtoSpInventarioProductos
    {
        [Key]
        public int Item { get; set; }
        public int ProductoId { get; set; }
        public int? ProductoInventarioId { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoCodigo { get; set; }
        public string ProductoDescripcion { get; set; }
        public string ProductoActivoConcentracion { get; set; }
        public string ProductoImagen { get; set; }
        public decimal? Stock { get; set; }
        public int? BodegaId { get; set; }
        public string BodegaNombre { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public int? PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal? PrecioValor { get; set; }
        public decimal? PrecioCompra { get; set; }
    }
}


