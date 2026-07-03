using Database.Shared.Models;
using Database.Shared.Paginacion;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class HomeProductoProximoVencerViewModel
    {
        public string ProductoCodigo { get; set; }
        public string ProductoNombre { get; set; }
        public string ProductoUbicacion { get; set; }
        public decimal ProductoStock { get; set; }
        public string FechaVencimiento { get; set; }
    }
}
