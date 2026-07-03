using sistema.Models;

namespace sistema.Service.IService
{
    public interface ICalculadorCuentaService
    {
        EstadoCuentaFinancieroViewModel Calcular(CuentasPorCobrarPagarViewModel model);
    }
}