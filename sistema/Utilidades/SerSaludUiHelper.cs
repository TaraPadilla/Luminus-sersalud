using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace farmamest.Utilidades
{
    public static class SerSaludUiHelper
    {
        public static bool EsSerSalud(string cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente))
                return false;

            return string.Equals(cliente, "SS", StringComparison.OrdinalIgnoreCase)
                || string.Equals(cliente, "SerSalud", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Normaliza Cliente para layout (_LayoutSS). Evita _LayoutSerSalud inexistente.</summary>
        public static string ResolverNombreLayout(string cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente) || EsSerSalud(cliente))
                return "_LayoutSS";

            return $"_Layout{cliente.Trim()}";
        }

        public static string ResolverLayout(IConfiguration configuration)
        {
            return ResolverNombreLayout(configuration?["Cliente"]);
        }

        public static string NormalizarCliente(string cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente))
                return "SS";
            if (EsSerSalud(cliente))
                return "SS";
            return cliente.Trim();
        }

        public static string ObtenerNombreUsuario(ClaimsPrincipal user)
        {
            return user?.Identity?.Name ?? "Usuario";
        }

        /// <summary>Primer rol en claims; evita NullReference si el usuario no tiene claim de rol en el cookie.</summary>
        public static string ObtenerRolVisible(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return "Usuario";

            var role = user.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role || string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase))
                ?.Value;

            return string.IsNullOrWhiteSpace(role) ? "Usuario" : role;
        }

        /// <summary>
        /// Calendario de citas en barra superior SerSalud (misma política que el enlace del menú lateral: visible para cualquier usuario autenticado).
        /// </summary>
        public static bool PuedeVerCalendarioCitas(ClaimsPrincipal user)
        {
            return user?.Identity?.IsAuthenticated == true;
        }
    }
}
