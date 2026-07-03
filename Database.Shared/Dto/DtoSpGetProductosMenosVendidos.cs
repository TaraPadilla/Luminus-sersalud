using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Shared.Dto
{
    public class DtoSpGetProductosMenosVendidos
    {
        [Key]
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal ProductoCantidad { get; set; }
    }
}
