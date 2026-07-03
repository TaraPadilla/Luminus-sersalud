using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Database.Shared.Models;
using farmamest.Models;
using sistema.Models;

namespace farmamest.Utilidades
{
    public static class RegistroAnestesiaPdfHelper
    {
        public static RegistroAnestesiaPdfVM Build(
            int hospitalizacionId,
            Hospitalizacion hospitalizacion,
            RegistroAnestesia registroGuardado,
            ConsentimientoHospiVM consentimiento,
            CuestionarioPreAnestesico cuestionario,
            AutorizacionAnestesiaPdfVM autorizacionAnestesia,
            string nombreAnestesista = null,
            string firmaAnestesista = null)
        {
            var paciente = hospitalizacion?.Paciente;
            var edad = paciente?.FechaNacimiento != null
                ? ((int)((DateTime.Now - paciente.FechaNacimiento.Value).TotalDays / 365.25)).ToString()
                : "-";

            var procedimiento = consentimiento?.ProcedimientoProgramado
                ?? consentimiento?.TratamientoMedico
                ?? cuestionario?.ProcedimientoProgramado
                ?? autorizacionAnestesia?.Procedimiento
                ?? hospitalizacion?.Consultas?.FirstOrDefault()?.Citas?.Procedimiento
                ?? "-";

            var anestesista = FirstNonEmpty(
                nombreAnestesista,
                consentimiento?.NombreAnestesista,
                autorizacionAnestesia?.NombreAnestesista,
                ExtraerAnestesistaDesdeRegistroJson(registroGuardado?.DatosJson));

            if (EsVacio(anestesista)
                || anestesista == "Nombre del Anestesista"
                || anestesista.Equals("Anestesiólogo asignado", StringComparison.OrdinalIgnoreCase))
                anestesista = "-";

            var firma = FirstNonEmpty(
                firmaAnestesista,
                consentimiento?.UrlFirmaAnestesista,
                autorizacionAnestesia?.FirmaAnestesistaBase64);

            var peso = ResolverPeso(paciente, cuestionario, registroGuardado?.DatosJson);
            var talla = ResolverTalla(paciente, cuestionario, registroGuardado?.DatosJson);
            var imc = CalcularImc(peso, talla, cuestionario);

            var fechaProcedimiento = hospitalizacion?.FechaInicio.ToString("yyyy-MM-dd")
                ?? cuestionario?.FechaProcedimiento?.ToString("yyyy-MM-dd")
                ?? consentimiento?.FechaAdmision;

            var datosJson = EnriquecerDatosJson(
                registroGuardado?.DatosJson,
                procedimiento,
                fechaProcedimiento,
                peso,
                talla,
                imc);

            return new RegistroAnestesiaPdfVM
            {
                HospitalizacionId = hospitalizacionId,
                PacienteNombre = paciente?.Nombre ?? "-",
                PacienteSexo = paciente?.Sexo?.DescripcionSexo ?? paciente?.sexoText ?? "-",
                EdadPaciente = edad,
                PacientePeso = peso,
                PacienteTalla = talla,
                Procedimiento = procedimiento,
                MedicoAnestesista = anestesista,
                UrlFirmaAnestesista = firma == "-" ? null : firma,
                FechaImpresion = HospitalTimeHelper.NowGuatemala(),
                TieneRegistroGuardado = registroGuardado != null && !string.IsNullOrWhiteSpace(registroGuardado.DatosJson),
                DatosJson = datosJson,
                FechaRegistroGuardado = registroGuardado?.FechaActualizacion ?? registroGuardado?.FechaRegistro
            };
        }

        private static string ExtraerAnestesistaDesdeRegistroJson(string datosJson)
        {
            if (string.IsNullOrWhiteSpace(datosJson))
                return null;

            try
            {
                var node = JsonNode.Parse(datosJson);
                var nota = node?["nota"]?.ToString();
                if (string.IsNullOrWhiteSpace(nota))
                    return null;

                const string marker = "Anestesiólogo:";
                var idx = nota.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                    return null;

                var nombre = nota[(idx + marker.Length)..].Trim();
                return string.IsNullOrWhiteSpace(nombre) ? null : nombre;
            }
            catch
            {
                return null;
            }
        }

        private static string ResolverPeso(Paciente paciente, CuestionarioPreAnestesico cuestionario, string datosJson)
        {
            return FirstNonEmpty(
                LeerCampoJson(datosJson, "an_peso"),
                paciente?.Peso,
                cuestionario?.Peso?.ToString("0.##", CultureInfo.InvariantCulture));
        }

        private static string ResolverTalla(Paciente paciente, CuestionarioPreAnestesico cuestionario, string datosJson)
        {
            return FirstNonEmpty(
                LeerCampoJson(datosJson, "an_talla"),
                paciente?.Talla,
                cuestionario?.Estatura?.ToString("0.##", CultureInfo.InvariantCulture));
        }

        private static string CalcularImc(string peso, string talla, CuestionarioPreAnestesico cuestionario)
        {
            if (cuestionario?.Peso.HasValue == true && cuestionario.Estatura.HasValue && cuestionario.Estatura.Value > 0)
                return (cuestionario.Peso.Value / (cuestionario.Estatura.Value * cuestionario.Estatura.Value))
                    .ToString("0.00", CultureInfo.InvariantCulture);

            if (double.TryParse(peso?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var p) &&
                double.TryParse(talla?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var t) &&
                t > 0)
                return (p / (t * t)).ToString("0.00", CultureInfo.InvariantCulture);

            return null;
        }

        private static string EnriquecerDatosJson(
            string datosJson,
            string procedimiento,
            string fechaProcedimiento,
            string peso,
            string talla,
            string imc)
        {
            if (string.IsNullOrWhiteSpace(datosJson))
                return datosJson;

            try
            {
                var node = JsonNode.Parse(datosJson)?.AsObject();
                if (node == null)
                    return datosJson;

                SetIfEmpty(node, "fecha", fechaProcedimiento);
                SetIfEmpty(node, "qx", procedimiento);

                var campos = node["campos"] as JsonObject ?? new JsonObject();
                SetIfEmpty(campos, "an_peso", peso);
                SetIfEmpty(campos, "an_talla", talla);
                SetIfEmpty(campos, "an_asa", "II");
                SetIfEmpty(campos, "an_tecnica", "General balanceada");
                SetIfEmpty(campos, "an_imc", imc);
                node["campos"] = campos;

                var radios = node["radios"] as JsonObject ?? new JsonObject();
                SetIfEmpty(radios, "an_maquina", "SI");
                SetIfEmpty(radios, "an_mon_esfigmo", "SI");
                SetIfEmpty(radios, "an_mon_ekg", "SI");
                SetIfEmpty(radios, "an_seg_ocular", "SI");
                node["radios"] = radios;

                return node.ToJsonString();
            }
            catch
            {
                return datosJson;
            }
        }

        private static void SetIfEmpty(JsonObject obj, string key, string value)
        {
            if (EsVacio(value))
                return;
            if (!obj.ContainsKey(key) || EsVacio(obj[key]?.ToString()))
                obj[key] = value;
        }

        private static string LeerCampoJson(string datosJson, string key)
        {
            if (string.IsNullOrWhiteSpace(datosJson))
                return null;

            try
            {
                var node = JsonNode.Parse(datosJson);
                if (node?["campos"] is JsonObject campos && campos.TryGetPropertyValue(key, out var el))
                {
                    var s = el?.ToString();
                    return EsVacio(s) ? null : s;
                }
            }
            catch { }

            return null;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (var v in values)
            {
                if (!EsVacio(v))
                    return v;
            }
            return "-";
        }

        private static bool EsVacio(string value)
        {
            return string.IsNullOrWhiteSpace(value) || value == "-";
        }
    }
}
