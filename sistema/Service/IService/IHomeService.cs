using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using sistema.Models;
using System.Collections.Generic;

namespace sistema.Service.IService
{
    public interface IHomeService
    {
        List<HomeProductoStockMinimoViewModel> GetComprasStockMinimo();
        List<HomeProductoStockMinimoViewModel> GetProductosStockMinimo(int tipoBodegaId);
        List<HomeProductoProximoVencerViewModel> GetProductosProximosVencer(int tipoBodegaId);
        List<HomeProductoVencidoViewModel> GetProductosVencidos(int tipoBodegaId);
        List<HomeExamenSolicitadoViewModel> GetExamenesSolicitados();
        List<HomeExamenFinalizadoViewModel> GetExamenesFinalizados();
        List<HomeCuentaPagarViewModel> GetCuentasPorPagar();
    }
}
