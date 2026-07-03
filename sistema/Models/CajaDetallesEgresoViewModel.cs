using System;

namespace sistema.Models
{
    public class CajaDetallesEgresoViewModel
    {
        public int Id { get; set; }
        public int? SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public int AmbienteId { get; set; }
        public string AmbienteNombre { get; set; }
        public DateTime FechaEgreso { get; set; }
        public string Descripcion { get; set; }
        public string NumeroComprobante { get; set; }
        public string CuentaContable { get; set; }
        public decimal Total { get; set; }
    }
}
