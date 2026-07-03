using Database.Shared.Models;

namespace farmamest.Models
{
    public class CuentasPorCobrarViewModel
    {
        public int CuentaId { get; set; }
        public CuentaPorCobrar CuentasPorCobrar{ get; set; }
        public decimal? TotalHospitalizacion { get; set; }
        public decimal? TotalMedicamentos { get; set; }
        public decimal? TotalTotal { get; set; }
    }
}
