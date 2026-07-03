using System.Linq;
using Database.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Database.Shared.SqlDataSeed
{
    /// <summary>
    /// Seeded sucursales are created without bodegas; new sucursales get one bodega per ambiente in
    /// <c>SucursalesController.Nueva</c>. This bootstrap backfills missing bodegas for existing data.
    /// </summary>
    public static class BodegaSeedBootstrap
    {
        public static void EnsureDefaultBodegas(Context context, ILogger logger = null)
        {
            var sucursales = context.Sucursales
                .Where(s => !s.Eliminado)
                .ToList();
            var ambientes = context.Ambientes.ToList();

            if (sucursales.Count == 0 || ambientes.Count == 0)
                return;

            var added = 0;
            foreach (var sucursal in sucursales)
            {
                foreach (var ambiente in ambientes)
                {
                    var exists = context.Bodegas.Any(b =>
                        !b.Eliminada
                        && b.SucursalId == sucursal.Id
                        && b.AmbienteId == ambiente.Id);

                    if (exists)
                        continue;

                    context.Bodegas.Add(new Bodega
                    {
                        SucursalId = sucursal.Id,
                        AmbienteId = ambiente.Id,
                        NombreBodega = ambiente.NombreAmbiente + " Sucursal " + sucursal.NombreSucursal,
                        Eliminada = false
                    });
                    added++;
                }
            }

            if (added == 0)
                return;

            context.SaveChanges();
            logger?.LogInformation("Created {Count} default bodega(s) for existing sucursales.", added);
        }
    }
}
