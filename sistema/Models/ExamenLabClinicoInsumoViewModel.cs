using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System;
using DocumentFormat.OpenXml.Drawing;

namespace sistema.Models
{
    public class ExamenLabClinicoInsumoViewModel
    {
        public int? Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int? UnidadMedidaVentaId { get; set; }
        public string UnidadMedidaVentaNombre { get; set; }
        public decimal CantidadUtilizada { get; set; }
        public decimal TotalInsumo { get; set; }
        public decimal PrecioCostoInsumo { get; set; }
        public bool Nuevo { get; set; }
    }
}