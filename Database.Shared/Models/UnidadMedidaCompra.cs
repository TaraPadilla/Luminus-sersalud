using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.Models
{
    public class UnidadMedidaCompra
    {
        public UnidadMedidaCompra()
        {
            ProductosEquivalencias = new List<ProductoEquivalencia>();
        }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public ICollection<ProductoEquivalencia> ProductosEquivalencias { get; set; }
    }
}
