using System;

namespace sistema.Models
{
    public class CajaDetallesCompraViewModel
    {
        public int Id { get; set; }
        public int? SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public int AmbienteId { get; set; }
        public string AmbienteNombre { get; set; }
        public DateTime FechaCompra { get; set; }
        public string Descripcion { get; set; }
        public string Comprobante { get; set; }
        public string Proveedor { get; set; }
        public string Empleado { get; set; }
        public decimal Total { get; set; }
    }
}
