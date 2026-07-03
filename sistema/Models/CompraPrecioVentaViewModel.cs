using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using System;

namespace sistema.Models
{
    public class CompraPrecioVentaViewModel
    {
        public int Item { get; set; }
        public int? DetalleOrdenCompraId { get; set; }
        public int UnidadMedidaVentaId { get; set; }
        public int PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal PrecioValor { get; set; }
    }
}