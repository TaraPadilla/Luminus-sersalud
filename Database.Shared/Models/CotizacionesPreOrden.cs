using System;

namespace Database.Shared.Models
{
    public class CotizacionesPreOrden
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Items { get; set; }
        public string ProveedorPrincipal { get; set; }


    }
}