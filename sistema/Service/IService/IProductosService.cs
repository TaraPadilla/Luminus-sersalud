using Database.Shared.Dto;
using Database.Shared.Models;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace farmamest.Service.IService
{
    public interface IProductosService
    {
        //En revision y proceso de ser eliminado pues el nuevo SP consume menos tiempo,
        //aun no se elimina ya que hay lugares donde se utiliza todavia
        List<Producto> GetInventarioBySp(int? tipoProductoId, int? grupoTerapeuticoId, int? bodegaId, int? sucursalId, int? ambienteId);
        // List<MovimientoProductoViewModel> GetHistoricoProductos(DateTime fechaInicio, DateTime fechaFin, List<int> productosIds);
        List<MovimientoProductoViewModel> GetHistoricoProductos(DateTime? fechaInicio, DateTime? fechaFin, List<int> productosIds);

        List<MovimientoProductoNacionalViewModel> GetHistoricoProductosNacional(DateTime? fechaInicio, DateTime? fechaFin, List<int> productosIds);

        //Este consume el nuevo SP, que es mas optimizado
        List<DtoSpInventarioProductos> GetInventario(int? ambienteId, int? bodegaId);

        //List<DtoSpInventarioProductos> GetInventarioEmergencias(int? tipoProductoId,int? ambienteId, int? bodegaId);

        public void RealizarDescuentoInventario(int productoId, int? unidadMedidaVentaId, decimal cantidad);
    }
}
