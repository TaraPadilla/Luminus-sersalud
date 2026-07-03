using farmamest.Models;
using sistema.Models;
using System.Collections.Generic;

namespace sistema.Service.IService
{
    public interface ICuentasPorCobrarService
    {
        public CuentasPorCobrarViewModel GetDetalleCuentaPorCobrar(int cuentaId);
        List<HospitalizacionProductoViewModel> GetMedicamentosNoPagadosHospitalizaciones(int cuentaId);
        List<HospitalizacionPaqueteViewModel> GetPaquetesNoPagadosHospitalizacion(int cuentaId);
    }
}
