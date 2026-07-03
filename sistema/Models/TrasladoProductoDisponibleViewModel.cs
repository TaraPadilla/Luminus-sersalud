using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;

namespace sistema.Models
{

    public class TrasladoProductoDisponibleViewModel
    {
        public int ProductoId { get; set; }
        public int ProductoInventarioId { get; set; }
        public string ProductoCodigo { get; set; }
        public string ProductoNombre { get; set; }
        public int UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public decimal CantidadExistente { get; set; }
        public decimal CantidadTrasladada { get; set; }
    }

}