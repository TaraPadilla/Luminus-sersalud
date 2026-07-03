using Database.Shared.Models;
using farmamest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace farmamest.Service.IService
{
    public interface IKitIngresoService
    {
        void Add(KitIngreso kitIngreso);
        KitIngreso GetById(int id);
        IEnumerable<KitIngreso> GetGlobalKits();
        IEnumerable<KitIngreso> GetByHospitalizacionId(int hospitalizacionId);
        void UpdateKit(KitIngreso kit);
        void AgregarProducto(KitIngresoDetalleInputVM detalle);
        void EliminarDetalle(int detalleId);
        KitIngresoDetalle GetDetalleById(int detalleId);
        void ActualizarDetalle(KitIngresoDetalleInputVM model);
        KitIngreso ClonarKit(int kitOrigenId, int nuevaHospitalizacionId, string userId);

        // Nuevos métodos para consumos
        Task<decimal> ObtenerUtilizadoAsync(int detalleId, int hospitalizacionId);
        Task GuardarUtilizadoAsync(int detalleId, int hospitalizacionId, decimal utilizado);
        Task<IEnumerable<HospitalizacionKitConsumo>> ObtenerConsumosPorHospitalizacionAsync(int hospitalizacionId);

        IEnumerable<KitIngreso> GetGlobalesYPorHospitalizacion(int hospitalizacionId);

        KitIngreso ClonarKitConDatos(
            int kitOrigenId,
            int nuevaHospitalizacionId,
            string userId,
            string nombrePaciente,
            string medico,
            string procedimiento,
            string responsable);
    }
}