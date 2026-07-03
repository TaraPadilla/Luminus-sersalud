using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using System;

namespace sistema.Models
{
    public class ProductoLoteExistenteVM
    {
        public int Item { get; set; }
        public int ProductoId { get; set; }
        public decimal Stock { get; set; }
        public decimal PrecioCompra { get; set; }
        public string FechaVencimiento { get; set; }
        public string ProveedorNombre { get; set; }
        public string Lote { get; set; }
        public string FechaRecepcionLote { get; set; }
        public string BodegaNombre { get; set; }
    }
}