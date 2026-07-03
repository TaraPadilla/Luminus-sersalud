using Database.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database.Shared.IRepository
{
    public interface IKitIngreso
    {
        void Add(KitIngreso kitIngreso);
        KitIngreso GetById(int id);
        IEnumerable<KitIngreso> GetByHospitalizacionId(int hospitalizacionId);
        void AddDetalle(KitIngresoDetalle detalle);
        void EliminarDetalle(int detalleId);

        KitIngresoDetalle GetDetalleById(int detalleId);
        void UpdateDetalle(KitIngresoDetalle detalle);
        IEnumerable<KitIngresoDetalle> GetDetallesByKitId(int kitIngresoId);

        IEnumerable<KitIngreso> GetGlobalesYPorHospitalizacion(int hospitalizacionId);

        IEnumerable<KitIngreso> GetGlobalKits();               // solo kits con HospitalizacionId == null
        IEnumerable<KitIngreso> GetLocalKits(int hospitalizacionId); // solo kits de esa hospitalización
        void UpdateKit(KitIngreso kit);
        Task<decimal> ObtenerUtilizadoPorDetalleYHospitalizacionAsync(int detalleId, int hospitalizacionId);
        Task GuardarConsumoAsync(int detalleId, int hospitalizacionId, decimal utilizado);
        Task<IEnumerable<HospitalizacionKitConsumo>> ObtenerConsumosPorHospitalizacionAsync(int hospitalizacionId);
        
    }
}