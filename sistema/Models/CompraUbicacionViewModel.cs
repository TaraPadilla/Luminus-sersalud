using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using System;

namespace sistema.Models
{
    public class CompraUbicacionViewModel
    {
        public int Item { get; set; }
        public int? DetalleOrdenCompraId { get; set; }
        public int UnidadMedidaVentaId { get; set; }
        public string NombreUnidad { get; set; }
        public string UnidadEquivalencia { get; set; }
        public decimal CantidadEquivalenteDestino { get; set; }
        public int BodegaId { get; set; }
        public string NombreUbicacion { get; set; }
        public decimal Cantidad { get; set; }
    }
}