using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class CitaServicioAgregadoViewModel
    {
        public int? Id { get; set; }
        public int ServicioId { get; set; }
        public string ServicioNombre { get; set; }
        public int ServicioDuracionHoras { get; set; }
        public int ServicioDuracionMinutos { get; set; }
        public int Cantidad { get; set; }
        public int PrecioId { get; set; }
        public string PrecioNombre { get; set; }
        public decimal PrecioValor { get; set; }
        public decimal PrecioValorCubiertoSeguro { get; set; }
        public decimal PrecioValorCopago { get; set; }
        public bool Nuevo { get; set; }
    }
}
