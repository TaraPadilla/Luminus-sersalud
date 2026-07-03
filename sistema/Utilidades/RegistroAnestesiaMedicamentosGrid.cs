using System;
using System.Collections.Generic;
using System.Linq;

namespace sistema.Utilidades
{
    /// <summary>
    /// Cuadrícula oficial SerSalud — Tercera sección del registro anestésico.
    /// </summary>
    public static class RegistroAnestesiaMedicamentosGrid
    {
        public const int Columnas = 9;

        public sealed class FilaDef
        {
            public string Item { get; init; }
            public bool Resaltada { get; init; }
        }

        public static IReadOnlyList<FilaDef> Filas { get; } = new List<FilaDef>
        {
            new() { Item = "Oxígeno (l/min)" },
            new() { Item = "N2O (l/min)" },
            new() { Item = "Sevoflurano (%)" },
            new() { Item = "Desflurano (%)" },
            new() { Item = "Propofol (mg)" },
            new() { Item = "Midazolam (mg)" },
            new() { Item = "Fentanyl (mcg)" },
            new() { Item = "Atracurio (mg)" },
            new() { Item = "Vecuronio (mg)" },
            new() { Item = "Succinilcolina (mg)" },
            new() { Item = "Efedrina (mg)", Resaltada = true },
            new() { Item = "Morfina (mg)", Resaltada = true },
            new() { Item = "Petidina (mg)", Resaltada = true },
        };

        private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Oxigeno (l/min)"] = "Oxígeno (l/min)",
            ["Oxígeno"] = "Oxígeno (l/min)",
            ["Oxigeno"] = "Oxígeno (l/min)",
            ["N2O"] = "N2O (l/min)",
            ["Sevoflurano"] = "Sevoflurano (%)",
            ["Desflurano"] = "Desflurano (%)",
            ["Propofol"] = "Propofol (mg)",
            ["Midazolam"] = "Midazolam (mg)",
            ["Fentanyl"] = "Fentanyl (mcg)",
            ["Atracurio"] = "Atracurio (mg)",
            ["Vecuronio"] = "Vecuronio (mg)",
            ["Succinilcolina"] = "Succinilcolina (mg)",
            ["Efedrina"] = "Efedrina (mg)",
            ["Morfina"] = "Morfina (mg)",
            ["Petidina"] = "Petidina (mg)",
        };

        public static string ResolverItemCanonico(string itemGuardado)
        {
            if (string.IsNullOrWhiteSpace(itemGuardado))
                return itemGuardado;

            var trimmed = itemGuardado.Trim();
            if (Filas.Any(f => string.Equals(f.Item, trimmed, StringComparison.OrdinalIgnoreCase)))
                return Filas.First(f => string.Equals(f.Item, trimmed, StringComparison.OrdinalIgnoreCase)).Item;

            if (Aliases.TryGetValue(trimmed, out var canonico))
                return canonico;

            return trimmed;
        }

        public static bool EsFilaEstandar(string item) =>
            Filas.Any(f => string.Equals(f.Item, ResolverItemCanonico(item), StringComparison.OrdinalIgnoreCase));
    }
}
