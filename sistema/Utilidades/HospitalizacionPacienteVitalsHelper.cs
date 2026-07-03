using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Database.Shared.Models;

namespace farmamest.Utilidades
{
    public static class HospitalizacionPacienteVitalsHelper
    {
        public static string ResolverPeso(Paciente paciente, CuestionarioPreAnestesico cuestionario, IEnumerable<ExamenFisicoHosp> examenesFisicos)
        {
            return FirstNonEmpty(
                LeerExamenFisico(examenesFisicos, "Peso"),
                paciente?.Peso,
                cuestionario?.Peso?.ToString("0.##", CultureInfo.InvariantCulture));
        }

        public static string ResolverTalla(Paciente paciente, CuestionarioPreAnestesico cuestionario, IEnumerable<ExamenFisicoHosp> examenesFisicos)
        {
            return FirstNonEmpty(
                LeerExamenFisico(examenesFisicos, "Estatura", "Talla", "Altura"),
                paciente?.Talla,
                cuestionario?.Estatura?.ToString("0.##", CultureInfo.InvariantCulture));
        }

        public static string ResolverTipoSangre(
            Paciente paciente,
            string consentimientoTipoSangre,
            IEnumerable<ExamenFisicoHosp> examenesFisicos = null)
        {
            return FirstNonEmpty(
                paciente?.TipoDeSangre,
                consentimientoTipoSangre,
                LeerExamenFisico(
                    examenesFisicos,
                    "Tipo de sangre",
                    "Tipo de Sangre",
                    "Tipo Sangre",
                    "Grupo sanguineo",
                    "Grupo Sanguineo"));
        }

        public static string ResolverNombrePaciente(Paciente paciente, params string[] fallbacks)
        {
            var values = new List<string>();
            if (!string.IsNullOrWhiteSpace(paciente?.Nombre))
                values.Add(paciente.Nombre);
            if (fallbacks != null)
            {
                foreach (var fallback in fallbacks)
                {
                    if (!string.IsNullOrWhiteSpace(fallback))
                        values.Add(fallback);
                }
            }

            return FirstNonEmpty(values.ToArray());
        }

        public static string FirstNonEmptyValue(params string[] values)
        {
            return FirstNonEmpty(values);
        }

        private static string LeerExamenFisico(IEnumerable<ExamenFisicoHosp> examenes, params string[] nombresDato)
        {
            if (examenes == null || nombresDato == null || nombresDato.Length == 0)
                return null;

            var comparer = StringComparer.OrdinalIgnoreCase;
            var nombres = new HashSet<string>(nombresDato, comparer);

            foreach (var examen in examenes.OrderByDescending(e => e.FechaHora))
            {
                if (examen?.ExamenesFisicosHospDatos == null)
                    continue;

                foreach (var dato in examen.ExamenesFisicosHospDatos)
                {
                    var nombre = dato?.DatoExamenFisicoHosp?.NombreDato;
                    if (string.IsNullOrWhiteSpace(nombre) || !nombres.Contains(nombre))
                        continue;

                    if (!string.IsNullOrWhiteSpace(dato.Valor))
                        return dato.Valor.Trim();
                }
            }

            return null;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    return value.Trim();
            }

            return null;
        }
    }
}
