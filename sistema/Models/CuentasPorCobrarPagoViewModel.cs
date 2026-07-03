using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using System.Collections.Generic;

namespace sistema.Models
{
    public class CuentasPorCobrarPagoViewModel
    {
        public string Fecha { get; set; }
        public int? PagoId { get; set; }
        public int FormaPagoId { get; set; }
        public string FormaPago { get; set; }
        public int MonedaId { get; set; }
        public string Moneda { get; set; }
        public decimal Monto { get; set; }
        public bool Nuevo { get; set; }
    }
}
