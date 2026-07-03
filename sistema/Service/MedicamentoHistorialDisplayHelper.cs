using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Database.Shared;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace farmamest.Service
{
    public static class MedicamentoHistorialDisplayHelper
    {
        public const string TextoPendiente = "Pendiente";
        public const string TextoNoRegistrado = "No registrado";

        public static bool EsFechaHistorialValida(DateTime? fecha)
        {
            return fecha.HasValue && fecha.Value.Year >= 2000;
        }

        public static string NormalizarTexto(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor) || valor.Trim() == "-")
                return TextoNoRegistrado;
            return valor.Trim();
        }

        public static string FormatearFechaProgramadaMedicamento(
            HospitalizacionProducto med,
            HospitalizacionProductoAplicacion app,
            int idx,
            List<DateTime> horarios)
        {
            if (EsFechaHistorialValida(app.FechaHoraAplicacionManual))
                return MedicamentoScheduleHelper.FormatearHorarioLocal(app.FechaHoraAplicacionManual.Value);

            if (idx < horarios.Count && EsFechaHistorialValida(horarios[idx]))
                return MedicamentoScheduleHelper.FormatearHorarioLocal(horarios[idx]);

            return MedicamentoAplicacionHelper.FormatearFechaProgramada(
                idx,
                (int)med.Cantidad,
                med.FrecuenciaAdministracion ?? "",
                med.FechaHoraAplicacionManual);
        }

        public static string FormatearFechaProgramadaInsumo(
            HospitalizacionInsumoDirecto insumo,
            HospitalizacionInsumoDirectoAplicacion app,
            int idx,
            bool aplicado)
        {
            var inicioManual = ResolverInicioManualInsumo(insumo);
            DateTime? fechaAlmacenada = !aplicado && EsFechaHistorialValida(app.FechaHoraAplicacion)
                ? HospitalTimeHelper.ToGuatemalaDisplay(app.FechaHoraAplicacion.Value)
                : null;

            return MedicamentoAplicacionHelper.FormatearFechaProgramada(
                idx,
                (int)insumo.Cantidad,
                insumo.FrecuenciaAdministracion ?? "",
                inicioManual,
                fechaAlmacenada);
        }

        public static string ResolverInicioManualInsumo(HospitalizacionInsumoDirecto insumo)
        {
            if (!string.IsNullOrWhiteSpace(insumo.FechaHoraAplicacionManual))
                return insumo.FechaHoraAplicacionManual;

            if (insumo.FechaCreacion.Year >= 2000)
                return MedicamentoScheduleHelper.FormatearHorarioLocal(
                    HospitalTimeHelper.ToGuatemalaDisplay(insumo.FechaCreacion));

            return MedicamentoScheduleHelper.FormatearHorarioLocal(HospitalTimeHelper.NowGuatemala());
        }

        public static DateTime? ParsearFechaProgramada(string fechaProgramadaTexto)
        {
            if (string.IsNullOrWhiteSpace(fechaProgramadaTexto) || fechaProgramadaTexto == "-")
                return null;

            var culture = new CultureInfo("es-GT");
            string[] formats =
            {
                "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "dd/MM/yyyy H:mm"
            };
            if (DateTime.TryParseExact(fechaProgramadaTexto.Trim(), formats, culture,
                    DateTimeStyles.None, out var parsed) && parsed.Year >= 2000)
                return parsed;

            if (DateTime.TryParse(fechaProgramadaTexto, culture, DateTimeStyles.None, out parsed)
                && parsed.Year >= 2000)
                return parsed;

            return null;
        }

        public static string FormatearInstanteAplicacion(DateTime? fechaHora, DateTime? scheduledLocal = null)
        {
            if (!EsFechaHistorialValida(fechaHora))
                return TextoNoRegistrado;

            var culture = new CultureInfo("es-GT");
            var local = HospitalTimeHelper.ToGuatemalaApplicationDisplay(
                fechaHora.Value, scheduledLocal);
            return local.ToString("dd/MM/yyyy HH:mm:ss", culture);
        }

        public static (string fecha, string hora) FormatearFechaHoraAplicacion(
            bool aplicado,
            DateTime? fechaHora,
            string fechaProgramadaTexto = null)
        {
            if (!aplicado)
                return (TextoPendiente, TextoPendiente);

            if (!EsFechaHistorialValida(fechaHora))
                return (TextoNoRegistrado, TextoNoRegistrado);

            var scheduled = ParsearFechaProgramada(fechaProgramadaTexto);
            var culture = new CultureInfo("es-GT");
            var local = HospitalTimeHelper.ToGuatemalaApplicationDisplay(
                fechaHora.Value, scheduled);
            return (
                local.ToString("dd/MM/yyyy", culture),
                local.ToString("HH:mm", culture));
        }

        public static string ResolverAplicadoPor(bool aplicado, Dictionary<string, string> nombresUsuario, string userId)
        {
            if (!aplicado)
                return TextoPendiente;

            if (string.IsNullOrWhiteSpace(userId))
                return TextoNoRegistrado;

            return nombresUsuario.TryGetValue(userId, out var nombre) && !string.IsNullOrWhiteSpace(nombre)
                ? nombre
                : TextoNoRegistrado;
        }

        public static Dictionary<string, string> ResolverNombresUsuarios(Context context, List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
                return new Dictionary<string, string>();

            var usuarios = context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.EmpleadoId, u.UserName })
                .ToList();

            var empleadoIds = usuarios
                .Where(u => u.EmpleadoId.HasValue)
                .Select(u => u.EmpleadoId.Value)
                .Distinct()
                .ToList();

            var empleados = context.Empleados
                .Where(e => empleadoIds.Contains(e.Id))
                .AsEnumerable()
                .ToDictionary(e => e.Id, e => FormatearNombreEmpleado(e.Nombre, e.Apellido));

            var map = new Dictionary<string, string>();
            foreach (var u in usuarios)
            {
                if (u.EmpleadoId.HasValue && empleados.TryGetValue(u.EmpleadoId.Value, out var nombre))
                    map[u.Id] = nombre;
                else if (!string.IsNullOrWhiteSpace(u.UserName))
                    map[u.Id] = u.UserName;
            }

            return map;
        }

        private static string FormatearNombreEmpleado(string nombre, string apellido)
        {
            var n = (nombre ?? "").Trim();
            var a = (apellido ?? "").Trim();
            if (string.IsNullOrEmpty(n)) return a;
            if (string.IsNullOrEmpty(a)) return n;
            return $"{n} {a}";
        }
    }
}
