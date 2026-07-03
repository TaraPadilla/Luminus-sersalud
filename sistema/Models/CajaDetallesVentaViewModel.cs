using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class CajaDetallesVentaViewModel
    {
        public int VentaId { get; set; }
        public int? SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public int AmbienteId { get; set; }
        public string AmbienteNombre { get; set; }
        public DateTime FechaVenta { get; set; }
        public string Comprobante { get; set; }
        public string Descripcion { get; set; }
        public string Cliente { get; set; }
        public string Vendedor { get; set; }
        public List<CajaDetallesVentaFormaPagoViewModel> FormasPago { get; set; }
        public decimal Total { get; set; }

        public string Origen { get; set; }

        public bool EsEmergencia { get; set; }
        public string OrigenVenta { get; set; }

    }

    public class CajaDetallesVentaFormaPagoViewModel
    {
        public int FormaPagoId { get; set; }
        public string NombreFormaPago { get; set; }
        public decimal Monto { get; set; }
    }
}
