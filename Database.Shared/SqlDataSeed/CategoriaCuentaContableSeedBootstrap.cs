using System.Linq;
using Database.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Database.Shared.SqlDataSeed
{
    public static class CategoriaCuentaContableSeedBootstrap
    {
        private static readonly (string Nombre, string Especificacion)[] DefaultCategories =
        {
            ("Ingresos operativos", "Ingresos por actividades principales del negocio"),
            ("Gastos operativos", "Gastos directamente relacionados con la operacion"),
            ("Gastos administrativos", "Gastos de administracion y oficina"),
            ("Gastos financieros", "Intereses, comisiones bancarias y gastos financieros"),
            ("Otros", "Movimientos que no encajan en otras categorias")
        };

        public static void EnsureDefaultCategories(Context context, ILogger logger = null)
        {
            if (context.CategoriasCuentaContables.Any(x => !x.Eliminado))
                return;

            foreach (var (nombre, especificacion) in DefaultCategories)
            {
                context.CategoriasCuentaContables.Add(new CategoriasCuentaContable
                {
                    Nombre = nombre,
                    Especificacion = especificacion,
                    Eliminado = false
                });
            }

            context.SaveChanges();
            logger?.LogInformation("Created default categorias de cuenta contable.");
        }
    }
}
