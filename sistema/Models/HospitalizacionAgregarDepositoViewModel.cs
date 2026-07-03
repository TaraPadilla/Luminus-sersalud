namespace sistema.Models
{
    public class HospitalizacionAgregarDepositoViewModel
    {
        public int CuentaId { get; set; }
        public int FormaPagoId { get; set; }
        public int MonedaId { get; set; }
        public decimal Valor { get; set; }
    }
}
