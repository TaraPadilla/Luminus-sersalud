using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Shared.Models;

namespace Database.Shared.IRepository
{
    public interface IRequision
    {
        // Crear requisición completa (encabezado + detalles + historial inicial)
        Task<Requision> CrearAsync(Requision requision, IEnumerable<RequisionDetalle> detalles, string? usuario);

        // Consultas principales
        Task<Requision?> GetByIdAsync(int id);
        Task<Requision?> GetByNumeroRequisicionAsync(int numeroRequisicion);
        Task<Requision?> GetByNumeroOrdenAsync(int numeroOrden);

        // Cambiar estado y dejar trazabilidad
        Task<bool> CambiarEstadoAsync(int requisionId, int nuevoEstado, string? usuario, string? observacion);

        // Listado simple (para vista Lista)
        Task<List<Requision>> ListarAsync(int top = 200) => Task.FromResult(new List<Requision>());

        public record PagedResult<T>(int RecordsTotal, int RecordsFiltered, IReadOnlyList<T> Items);

        Task<PagedResult<RequisionListaItemDto>> ListarDataTableAsync(
            int start,
            int length,
            string? search,
            string? orderColumn,
            string? orderDir)
            => Task.FromResult(new PagedResult<RequisionListaItemDto>(
                RecordsTotal: 0,
                RecordsFiltered: 0,
                Items: Array.Empty<RequisionListaItemDto>()
            ));

        (int? NumeroRequisicion, int? NumeroOrden) ObtenerUltimoRegistro();

        void Update(Requision requision);

        Task<bool> ActualizarCantidadesDespachoPorProductoAsync(List<RequisionDetalle> items);

        // NUEVO: Entrega a Kardex (Estado 6)
        Task<(bool Exitoso, string Mensaje)> ProcesarEntregaAKardexAsync(
            int requisionId,
            string usuarioId,
            string rutaFirmaAlmacen,
            string? nombreAlmacen,
            string usuarioId2);
    }
}