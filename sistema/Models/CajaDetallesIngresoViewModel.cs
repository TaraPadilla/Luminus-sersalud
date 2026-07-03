using System;

namespace sistema.Models
{
    public class CajaDetallesIngresoViewModel
    {
        public int Id { get; set; }
        public int? SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public int AmbienteId { get; set; }
        public string AmbienteNombre { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Descripcion { get; set; }
        public string NumeroComprobante { get; set; }
        public string Banco { get; set; }
        public string Cuenta { get; set; }
        public decimal Total { get; set; }
    }
}
