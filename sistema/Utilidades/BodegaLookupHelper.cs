using System.Linq;
using Database.Shared;
using Database.Shared.Enumeraciones;

namespace sistema.Utilidades
{
    public static class BodegaLookupHelper
    {
        /// <summary>
        /// Resolves a warehouse primary key by ambiente (not AmbienteEnum value used as BodegaId).
        /// </summary>
        public static int? ObtenerBodegaIdPorAmbiente(Context context, AmbienteEnum ambiente, int? sucursalId = null)
            => ObtenerBodegaIdPorAmbiente(context, (int)ambiente, sucursalId);

        public static int? ObtenerBodegaIdPorAmbiente(Context context, int ambienteId, int? sucursalId = null)
        {
            var query = context.Bodegas.Where(b => !b.Eliminada && b.AmbienteId == ambienteId);
            if (sucursalId.HasValue)
                query = query.Where(b => b.SucursalId == sucursalId);
            return query.OrderBy(b => b.Id).Select(b => (int?)b.Id).FirstOrDefault();
        }
    }
}
