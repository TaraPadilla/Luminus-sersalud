using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using Database.Shared.DataBindings;

namespace sistema.Models
{
    public class CuentasPorCobrarModificarViewModel
    {
        public int CuentaId { get; set; }
        public decimal Valor { get; set; }
        public string Observaciones { get; set; }
    }
}
