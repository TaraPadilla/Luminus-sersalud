using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Shared.Models;

namespace Database.Shared.IRepository
{
    public interface IDevolucionNacional
    {
        Task<List<DevolucionListaDTO>> GetAllAsync();

        Task<DevolucionNacional> CrearAsync(DevolucionNacional devolucion, string? usuario);
        Task<DevolucionNacional?> GetByIdAsync(int id);
        Task<bool> CambiarEstadoAsync(int devolucionId, int nuevoEstado, string? usuario, string? observacion);
        int? ObtenerUltimoRegistro();
        Task ActualizarFirmaAsync(DevolucionNacional devolucion);
    }
}