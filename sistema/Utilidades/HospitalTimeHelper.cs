using System;

namespace farmamest.Utilidades
{
    public static class HospitalTimeHelper
    {
        private static readonly string HospitalTimeZoneId = "America/Guatemala";
        private static readonly TimeZoneInfo HospitalTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(HospitalTimeZoneId);

        public static DateTime NowGuatemala()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, HospitalTimeZone);
        }

        /// <summary>Marca de tiempo UTC para persistir aplicaciones (se muestra con ToGuatemalaDisplay).</summary>
        public static DateTime UtcNowForStorage() => DateTime.UtcNow;

        /// <summary>
        /// Convierte a hora de pared Guatemala para mostrar en PDF/UI.
        /// UTC se convierte; Unspecified se asume ya en hora Guatemala (legacy).
        /// </summary>
        public static DateTime ToGuatemalaDisplay(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return TimeZoneInfo.ConvertTimeFromUtc(dateTime, HospitalTimeZone);

            if (dateTime.Kind == DateTimeKind.Local)
                return TimeZoneInfo.ConvertTime(dateTime, HospitalTimeZone);

            return dateTime;
        }

        public static DateTime ToGuatemala(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return TimeZoneInfo.ConvertTimeFromUtc(dateTime, HospitalTimeZone);

            if (dateTime.Kind == DateTimeKind.Local)
                return TimeZoneInfo.ConvertTime(dateTime, HospitalTimeZone);

            return dateTime;
        }

        /// <summary>
        /// Normaliza instante de aplicación guardado en BD (UTC nuevo, Unspecified legacy) a hora Guatemala.
        /// Si hay fecha programada y el valor legacy difiere 3–12 h, se usa la programada (servidor UTC guardó mal).
        /// </summary>
        public static DateTime ToGuatemalaApplicationDisplay(DateTime stored, DateTime? scheduledLocal = null)
        {
            if (stored.Year < 2000)
                return stored;

            DateTime asUtc = stored.Kind == DateTimeKind.Utc
                ? ToGuatemalaDisplay(stored)
                : ToGuatemalaDisplay(DateTime.SpecifyKind(stored, DateTimeKind.Utc));

            DateTime asWall = stored.Kind == DateTimeKind.Unspecified
                ? stored
                : ToGuatemalaDisplay(stored);

            if (scheduledLocal.HasValue && scheduledLocal.Value.Year >= 2000)
            {
                var sched = scheduledLocal.Value;
                var diffUtc = Math.Abs((asUtc - sched).TotalHours);
                var diffWall = Math.Abs((asWall - sched).TotalHours);

                if (diffUtc >= 3 && diffUtc <= 12 && diffUtc > diffWall)
                    return asWall;

                if (diffUtc >= 3 && diffUtc <= 12 && diffWall >= 3)
                    return sched;

                if (diffUtc < diffWall)
                    return asUtc;
                return asWall;
            }

            if (stored.Kind == DateTimeKind.Utc)
                return asUtc;

            if (stored.Kind == DateTimeKind.Unspecified)
                return asWall;

            return ToGuatemalaDisplay(stored);
        }
    }
}
