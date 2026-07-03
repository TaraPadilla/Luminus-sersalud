using farmamest.Models;
using System.Collections.Generic;

namespace sistema.Models
{
    public class CuentaPorCobrarReciboViewModel
    {
        //Info documento
        public string FechaEmision { get; set; }
        public bool PagoParcial { get; set; }
        public bool PagoTotal { get; set; }

        //Cuenta
        public decimal CuentaValorPendiente { get; set; }

        //Paciente
        public string PacienteNombre { get; set; }
        public string PacienteEdad { get; set; }

        //Pagos
        public List<CuentaPorCobrarReciboRegistroPagoViewModel> Pagos { get; set; }
        public decimal TotalPagos { get; set; }


        public string TipoPagoText
        {
            get
            {
                return PagoParcial ? "Pago parcial" : "Pago total";
            }
        }
    }
}
