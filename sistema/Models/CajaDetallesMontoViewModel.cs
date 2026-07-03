using System;

namespace sistema.Models
{
    public class CajaDetallesMontoViewModel
    {
        public int? SucursalId { get; set; }
        public string SucursalNombre { get; set; }
        public int AmbienteId { get; set; }
        public string AmbienteNombre { get; set; }
        public decimal Efectivo { get; set; }
        public decimal Visa { get; set; }
        public decimal MasterCard { get; set; }
        public decimal Cheques { get; set; }
        public decimal Transferencia { get; set; }
        public decimal Visalink { get; set; }
        public decimal Visanet { get; set; }
        public decimal Seguro { get; set; }
        public decimal Total { get; set; }


        public decimal EfectivoEmergencia { get; set; }
        public decimal VisaEmergencia { get; set; }
        public decimal MasterCardEmergencia { get; set; }
        public decimal ChequesEmergencia { get; set; }
        public decimal TransferenciaEmergencia { get; set; }
        public decimal VisalinkEmergencia { get; set; }
        public decimal VisanetEmergencia { get; set; }
        public decimal SeguroEmergencia { get; set; }
        public decimal TotalEmergencia { get; set; }
    }
}
