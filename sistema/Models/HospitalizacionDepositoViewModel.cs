using System;

namespace sistema.Models
{
    public class HospitalizacionDepositoViewModel
    {
        public int? Id { get; set; }
        public string FechaHora { get; set; }
        public string FormaPago { get; set; }
        public string Moneda { get; set; }
        public string Monto { get; set; }
    }
}
