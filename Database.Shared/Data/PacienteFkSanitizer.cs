using System.Linq;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Shared.Data
{
    internal static class PacienteFkSanitizer
    {
        public static void SanitizeOptionalForeignKeys(Context context, Paciente paciente)
        {
            if (paciente == null)
                return;

            SanitizeUbicacionResidencia(context, paciente);
            SanitizeOptionalReference(context, paciente);
        }

        private static void SanitizeUbicacionResidencia(Context context, Paciente paciente)
        {
            int? departamentoId = null;
            int? municipioId = null;

            if (paciente.DepartamentoId is > 0 &&
                context.Departamentos.AsNoTracking().Any(d => d.Id == paciente.DepartamentoId && !d.Eliminado))
            {
                departamentoId = paciente.DepartamentoId;
            }

            if (paciente.MunicipioId is > 0)
            {
                var municipio = context.Municipios
                    .AsNoTracking()
                    .FirstOrDefault(m => m.Id == paciente.MunicipioId && !m.Eliminado);

                if (municipio != null && (departamentoId == null || municipio.DepartamentoId == departamentoId))
                {
                    municipioId = municipio.Id;
                    departamentoId ??= municipio.DepartamentoId;
                }
            }

            paciente.DepartamentoId = departamentoId;
            paciente.MunicipioId = municipioId;
        }

        private static void SanitizeOptionalReference(Context context, Paciente paciente)
        {
            if (paciente.SeguroEpssId is <= 0 ||
                !context.SegurosEpss.AsNoTracking().Any(s => s.Id == paciente.SeguroEpssId))
            {
                paciente.SeguroEpssId = null;
            }

            if (paciente.PacientePediatricoApnpId is <= 0 ||
                !context.PacientePediatricoApnp.AsNoTracking().Any(p => p.Id == paciente.PacientePediatricoApnpId))
            {
                paciente.PacientePediatricoApnpId = null;
            }
        }
    }
}
