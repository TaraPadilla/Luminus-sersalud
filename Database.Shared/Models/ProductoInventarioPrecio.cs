using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Database.Shared.Models
{
    public class ProductoInventarioPrecio
    {
        public int Id { get; set; }
        public int? PrecioId { get; set; }
        public Precio Precio { get; set; }
        public int ProductoInventarioId { get; set; }
        public ProductoInventario ProductoInventario { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }
        public bool Eliminado { get; set; }
    }
}
