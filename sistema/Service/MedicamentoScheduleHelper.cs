using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace farmamest.Service
{
    /// <summary>
    /// Calcula horarios de aplicación de medicamentos según cantidad, frecuencia e inicio.
    /// Zona horaria: America/Guatemala (misma que MedicamentoNotificacionService).
    /// </summary>
    public static class MedicamentoScheduleHelper
    {
        private static readonly TimeZoneInfo HospitalTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("America/Guatemala");

        /// <summary>
        /// Genera N horarios equidistantes a partir de la fecha/hora de inicio.
        /// Ejemplo: cantidad=3, frecuencia="Cada 1 hora", inicio=9:00 → 9:00, 10:00, 11:00.
        /// </summary>
        public static List<DateTime> CalcularHorariosAplicacion(
            int cantidadDosis,
            string frecuenciaAdministracion,
            string fechaHoraInicioManual)
        {
            var horarios = new List<DateTime>();
            if (cantidadDosis <= 0) return horarios;

            var inicio = ParseFechaHoraLocal(fechaHoraInicioManual)
                         ?? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, HospitalTimeZone);

            int intervaloMinutos = ObtenerIntervaloMinutos(frecuenciaAdministracion);

            for (int i = 0; i < cantidadDosis; i++)
                horarios.Add(inicio.AddMinutes(intervaloMinutos * i));

            return horarios;
        }

        public static int ObtenerIntervaloMinutos(string frecuenciaAdministracion)
        {
            if (string.IsNullOrWhiteSpace(frecuenciaAdministracion))
                return 0;

            if (!frecuenciaAdministracion.Contains("Cada", StringComparison.OrdinalIgnoreCase))
                return 0;

            var match = Regex.Match(frecuenciaAdministracion, @"\d+");
            if (!match.Success) return 0;

            int valor = int.Parse(match.Value);
            string lower = frecuenciaAdministracion.ToLowerInvariant();

            if (lower.Contains("minuto")) return valor;
            if (lower.Contains("hora")) return valor * 60;
            return valor * 60;
        }

        public static DateTime? ParseFechaHoraLocal(string rawDate)
        {
            if (string.IsNullOrWhiteSpace(rawDate)) return null;

            rawDate = rawDate.Trim()
                .Replace("p. m.", "PM", StringComparison.OrdinalIgnoreCase)
                .Replace("a. m.", "AM", StringComparison.OrdinalIgnoreCase)
                .Replace("p.m.", "PM", StringComparison.OrdinalIgnoreCase)
                .Replace("a.m.", "AM", StringComparison.OrdinalIgnoreCase);

            if (rawDate.Contains('T'))
            {
                string[] isoFormats = { "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm" };
                if (DateTime.TryParseExact(rawDate, isoFormats, CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal, out var isoLocal))
                    return isoLocal;
            }

            string[] localFormats =
            {
                "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "dd/MM/yyyy H:mm",
                "dd/MM/yyyy hh:mm tt", "dd/MM/yyyy h:mm tt",
                "MM/dd/yyyy HH:mm", "yyyy-MM-dd HH:mm"
            };
            var cultures = new[] { CultureInfo.InvariantCulture, new CultureInfo("es-GT") };
            foreach (var culture in cultures)
            {
                if (DateTime.TryParseExact(rawDate, localFormats, culture, DateTimeStyles.None, out var parsed))
                    return parsed;
            }

            if (DateTime.TryParse(rawDate, new CultureInfo("es-GT"), DateTimeStyles.None, out var fallback))
                return fallback;

            return null;
        }

        public static string FormatearHorarioLocal(DateTime localDateTime)
        {
            if (localDateTime.Year < 2000)
                return "-";

            return localDateTime.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("es-GT"));
        }
    }
}
