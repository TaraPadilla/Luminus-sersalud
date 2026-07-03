using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class DetalleTrasladoProductos
    {
        public DetalleTrasladoProductos()
        {

        }
        public int Id { get; set; }
        public int? ProductoId { get; set; }
        public int? ProductoInventarioId { get; set; }

        public ProductoInventario ProductoInventario { get; set; }
        public int TrasladosProductosId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }
        public TrasladosProductos TrasladosProductos { get; set; }
        public Producto Producto { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaTraslado { get; set; }
    }
}