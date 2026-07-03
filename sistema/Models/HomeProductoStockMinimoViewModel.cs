using Database.Shared.Models;
using Database.Shared.Paginacion;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class HomeProductoStockMinimoViewModel
    {
        public string ProductoCodigo { get; set; }
        public string AmbienteNombre { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoUbicacion { get; set; }
        public decimal ProductoStock { get; set; }
    }
}
