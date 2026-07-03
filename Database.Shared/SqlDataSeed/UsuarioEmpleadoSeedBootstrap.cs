using System;
using System.Linq;
using Database.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Database.Shared.SqlDataSeed
{
    /// <summary>
    /// Seed users (dev/admin) are created without EmpleadoId. This bootstrap links them to an
    /// existing empleado or creates a minimal one so firma digital and requisiciones work in dev.
    /// </summary>
    public static class UsuarioEmpleadoSeedBootstrap
    {
        private static readonly string[] DefaultUserNames = { "dev@redowl.com", "admin@redowl.com" };

        public static void EnsureDefaultEmpleadoLinks(Context context, ILogger logger = null)
        {
            var usersWithoutEmpleado = context.Users
                .Where(u => u.EmpleadoId == null && DefaultUserNames.Contains(u.UserName))
                .ToList();

            if (usersWithoutEmpleado.Count == 0)
                return;

            var empleado = context.Empleados
                .Where(e => !e.Eliminado)
                .OrderBy(e => e.Id)
                .FirstOrDefault();

            if (empleado == null)
            {
                empleado = new Empleado
                {
                    Nombre = "Desarrollador",
                    Apellido = "Sistema",
                    Telefono = "00000000",
                    Dpi = "0000000000000",
                    Nit = "CF",
                    Email = "dev@redowl.com",
                    FechaInicioLabores = DateTime.UtcNow,
                    VacacionesProgramadas = DateTime.UtcNow,
                    VacacionesProgramadasFinal = DateTime.UtcNow,
                    Eliminado = false,
                    TipoEmpleado = "Administrativo"
                };
                context.Empleados.Add(empleado);
                context.SaveChanges();
                logger?.LogInformation("Created default empleado for system users.");
            }

            foreach (var user in usersWithoutEmpleado)
                user.EmpleadoId = empleado.Id;

            context.SaveChanges();
            logger?.LogInformation(
                "Linked {Count} default user(s) to empleado {EmpleadoId}.",
                usersWithoutEmpleado.Count,
                empleado.Id);
        }
    }
}
