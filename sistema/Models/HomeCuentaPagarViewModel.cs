using Database.Shared.Models;
using Database.Shared.Paginacion;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class HomeCuentaPagarViewModel
    {
        public int CompraId { get; set; }
        public string AmbienteNombre { get; set; }
        public string FechaCompra { get; set; }
        public string Proveedor { get; set; }
        public decimal Valor { get; set; }
    }
}
