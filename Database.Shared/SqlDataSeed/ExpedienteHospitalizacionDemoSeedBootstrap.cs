using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Shared.SqlDataSeed
{
    /// <summary>
    /// Demo clinical data for expediente completo PDF samples (SerSalud).
    /// Idempotent per hospitalization via marker EXP-DEMO-{id}.
    /// </summary>
    public static class ExpedienteHospitalizacionDemoSeedBootstrap
    {
        public const int DemoHospitalizacionId = 655;

        private static readonly int[] DefaultHospitalizacionIds = { 655, 698, 704, 705 };

        public static void EnsureDemoExpedienteData(Context context, ILogger logger = null, string webRootPath = null)
        {
            if (!string.IsNullOrWhiteSpace(webRootPath))
                EnsureDemoExpedientePdfAssets(webRootPath, logger);

            foreach (var hospId in DefaultHospitalizacionIds)
                EnsureDemoExpedienteDataForHospitalizacion(context, hospId, logger);
        }

        public static void EnsureDemoExpedientePdfAssets(string webRootPath, ILogger logger = null)
        {
            if (string.IsNullOrWhiteSpace(webRootPath))
                return;

            var dir = Path.Combine(webRootPath, "uploads", "expediente");
            Directory.CreateDirectory(dir);

            var templateLab = Path.Combine(dir, "laboratorios-hosp-704.pdf");
            var templateRx = Path.Combine(dir, "rx-rodilla-hosp-704.pdf");

            EnsureDemoDocumentoPdfTemplate(templateLab, "laboratorios");
            EnsureDemoDocumentoPdfTemplate(templateRx, "radiografia");

            foreach (var id in DefaultHospitalizacionIds)
            {
                CopyPdfIfLegacyOrMissing(templateLab, Path.Combine(dir, $"laboratorios-hosp-{id}.pdf"), logger);
                CopyPdfIfLegacyOrMissing(templateRx, Path.Combine(dir, $"rx-rodilla-hosp-{id}.pdf"), logger);
            }
        }

        public static void EnsureDemoExpedienteDataForHospitalizacion(Context context, int hospitalizacionId, ILogger logger = null)
        {
            var marker = $"EXP-DEMO-{hospitalizacionId}";

            var hosp = context.Hospitalizaciones
                .Include(h => h.Paciente)
                .FirstOrDefault(h => h.Id == hospitalizacionId && !h.Eliminada);
            if (hosp == null)
            {
                logger?.LogInformation("Expediente demo seed: hospitalización {Id} no encontrada.", hospitalizacionId);
                return;
            }

            var userId = ResolverUserId(context, hospitalizacionId);
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger?.LogWarning("Expediente demo seed: no hay usuarios para hospitalización {Id}.", hospitalizacionId);
                return;
            }

            var baseDate = hosp.FechaInicio == default ? DateTime.Now.AddDays(-2) : hosp.FechaInicio;
            var medicoNombre = ObtenerMedicoNombre(context, hospitalizacionId) ?? "Dr. Demo SerSalud";

            AplicarSeedExpedienteCompleto(context, hosp, hospitalizacionId, marker, userId, baseDate, medicoNombre, logger);
        }

        private static void AplicarSeedExpedienteCompleto(
            Context context,
            Hospitalizacion hosp,
            int hospitalizacionId,
            string marker,
            string userId,
            DateTime baseDate,
            string medicoNombre,
            ILogger logger)
        {
            EtiquetarNotasExistentes(context, hospitalizacionId);
            AsegurarNotasEvolucionCompletas(context, hospitalizacionId, userId, baseDate, marker);
            AsegurarNotasEnfermeriaCompletas(context, hospitalizacionId, userId, baseDate, marker);
            AsegurarOrdenesMedicasCompletas(context, hospitalizacionId, userId, baseDate, marker, medicoNombre);
            AsegurarNotaOperatoria(context, hospitalizacionId, userId, baseDate, marker, medicoNombre);
            AsegurarAnestesistaEnCita(context, hosp, medicoNombre);
            AsegurarCuestionarioPreAnestesico(context, hosp, hospitalizacionId, userId, baseDate, marker, medicoNombre);
            AsegurarListaChequeo(context, hosp, hospitalizacionId, userId, baseDate, marker, medicoNombre);
            AsegurarRegistroAnestesia(context, hosp, hospitalizacionId, baseDate, marker, medicoNombre, logger);
            AsegurarSignosVitales(context, hospitalizacionId, userId, baseDate, marker);
            AsegurarDocumentosYMedicamentos(context, hosp, hospitalizacionId, marker, logger);
            AsegurarConsentimientoDemo(context, hosp, marker, medicoNombre);
            AsegurarVinculoConsultaHospitalizacion(context, hosp);
            VincularConsentimientoHospitalizacion(context, hosp);
            context.SaveChanges();
            logger?.LogInformation("Expediente demo seed aplicado a hospitalización {Id}.", hospitalizacionId);
        }

        private static string ResolverUserId(Context context, int hospitalizacionId)
        {
            var userId = context.Users
                .OrderBy(u => u.Id)
                .Select(u => u.Id)
                .FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = context.NotaMedica2
                    .Where(n => n.HospitalizacionId == hospitalizacionId && n.ProfesionalId != null)
                    .Select(n => n.ProfesionalId)
                    .FirstOrDefault();
            }
            return userId;
        }

        private static void AsegurarNotasEvolucionCompletas(
            Context context,
            int hospitalizacionId,
            string userId,
            DateTime baseDate,
            string marker)
        {
            if (!TieneNotaEvolucion(context, hospitalizacionId, TipoNotaClinica.Ingreso))
            {
                context.NotaMedica2.Add(CrearNotaEvolucion(hospitalizacionId, userId, TipoNotaClinica.Ingreso, baseDate.AddHours(1),
                    $"{marker} Paciente ingresa para procedimiento programado. Evaluación preoperatoria favorable."));
            }

            if (!TieneNotaEvolucion(context, hospitalizacionId, TipoNotaClinica.Traslado))
            {
                context.NotaMedica2.Add(CrearNotaEvolucion(hospitalizacionId, userId, TipoNotaClinica.Traslado, baseDate.AddHours(4),
                    $"{marker} Traslado a sala de recuperación post artroplastia de rodilla. Paciente estable, dolor controlado."));
            }

            if (!TieneNotaEvolucion(context, hospitalizacionId, TipoNotaClinica.Recepcion))
            {
                context.NotaMedica2.Add(CrearNotaEvolucion(hospitalizacionId, userId, TipoNotaClinica.Recepcion, baseDate.AddHours(8),
                    $"{marker} Recepción en sala. Herida operatoria limpia. Sonda Foley y drenaje Jackson-Pratt funcionales."));
            }

            if (!TieneNotaEvolucion(context, hospitalizacionId, TipoNotaClinica.Egreso))
            {
                context.NotaMedica2.Add(CrearNotaEvolucion(hospitalizacionId, userId, TipoNotaClinica.Egreso, baseDate.AddDays(1),
                    $"{marker} Egreso hospitalario en condiciones estables. Indicaciones de control ambulatorio."));
            }
        }

        private static void AsegurarNotasEnfermeriaCompletas(
            Context context,
            int hospitalizacionId,
            string userId,
            DateTime baseDate,
            string marker)
        {
            var turnoId = context.NotaEnfermeria2
                .Where(n => n.HospitalizacionId == hospitalizacionId)
                .Select(n => n.TurnoEnfermeriaId)
                .FirstOrDefault();

            if (turnoId == 0)
            {
                var turno = new TurnoEnfermeria
                {
                    FechaRegistro = baseDate,
                    NumeroTurno = 1,
                    NombreTurno = "Mañana",
                    HospitalizacionId = hospitalizacionId,
                    UserId = userId,
                    Firmado = false
                };
                context.TurnoEnfermeria.Add(turno);
                context.SaveChanges();
                turnoId = turno.Id;
            }

            EtiquetarNotasEnfermeriaExistentes(context, hospitalizacionId);

            if (!TieneNotaEnfermeria(context, hospitalizacionId, TipoNotaClinica.Ingreso))
            {
                context.NotaEnfermeria2.Add(CrearNotaEnfermeria(hospitalizacionId, userId, turnoId, TipoNotaClinica.Ingreso, baseDate.AddHours(1),
                    $"{marker} Ingreso a enfermería. Identificación verificada, pulsera colocada, signos vitales basales registrados."));
            }

            if (!TieneNotaEnfermeria(context, hospitalizacionId, TipoNotaClinica.Traslado))
            {
                context.NotaEnfermeria2.Add(CrearNotaEnfermeria(hospitalizacionId, userId, turnoId, TipoNotaClinica.Traslado, baseDate.AddHours(4),
                    $"{marker} Traslado a sala de recuperación. Monitorización continua durante traslado."));
            }

            if (!TieneNotaEnfermeria(context, hospitalizacionId, TipoNotaClinica.Recepcion))
            {
                context.NotaEnfermeria2.Add(CrearNotaEnfermeria(hospitalizacionId, userId, turnoId, TipoNotaClinica.Recepcion, baseDate.AddHours(8),
                    $"{marker} Recepción en sala. Herida operatoria evaluada, drenajes patentes, analgesia administrada."));
            }

            if (!TieneNotaEnfermeria(context, hospitalizacionId, TipoNotaClinica.Egreso))
            {
                context.NotaEnfermeria2.Add(CrearNotaEnfermeria(hospitalizacionId, userId, turnoId, TipoNotaClinica.Egreso, baseDate.AddDays(1),
                    $"{marker} Educación al paciente. Egreso acompañado con indicaciones de cuidado de herida y movilización."));
            }
        }

        private static void AsegurarOrdenesMedicasCompletas(
            Context context,
            int hospitalizacionId,
            string userId,
            DateTime baseDate,
            string marker,
            string medicoNombre)
        {
            var ordenes = context.OrdenesMedicas
                .Where(o => o.HospitalizacionId == hospitalizacionId)
                .OrderBy(o => o.FechaHora)
                .ToList();

            if (!ordenes.Any(o => o.Autorizado))
            {
                var primeraOrden = ordenes.FirstOrDefault();
                if (primeraOrden != null)
                {
                    primeraOrden.Autorizado = true;
                    primeraOrden.FechaAutorizacion = baseDate;
                    if (string.IsNullOrWhiteSpace(primeraOrden.UsuarioAutoriza))
                        primeraOrden.UsuarioAutoriza = userId;
                }
                else
                {
                    context.OrdenesMedicas.Add(new OrdenesMedicas
                    {
                        HospitalizacionId = hospitalizacionId,
                        FechaHora = baseDate.AddHours(-1),
                        Profesional = medicoNombre,
                        Descripcion = $"{marker} Pre-operatorio: ayuno 8h, profilaxis antibiótica, laboratorios de control.",
                        Realizada = true,
                        FechaRealizacion = baseDate,
                        ProfesionalRealiza = medicoNombre,
                        Autorizado = true,
                        FechaAutorizacion = baseDate.AddHours(-1),
                        UsuarioAutoriza = userId
                    });
                    ordenes = context.OrdenesMedicas
                        .Where(o => o.HospitalizacionId == hospitalizacionId)
                        .OrderBy(o => o.FechaHora)
                        .ToList();
                }
            }

            var preOpIdx = ordenes.FindIndex(o => o.Autorizado);
            var tienePostOp = preOpIdx >= 0 && ordenes.Count > preOpIdx + 1;
            if (!tienePostOp)
            {
                context.OrdenesMedicas.Add(new OrdenesMedicas
                {
                    HospitalizacionId = hospitalizacionId,
                    FechaHora = baseDate.AddHours(10),
                    Profesional = medicoNombre,
                    Descripcion = $"{marker} Post-operatorio: analgesia IV, profilaxis anticoagulante, fisioterapia temprana, dieta progresiva.",
                    Realizada = true,
                    FechaRealizacion = baseDate.AddHours(10),
                    ProfesionalRealiza = medicoNombre,
                    Autorizado = false
                });
            }
        }

        private static void AsegurarNotaOperatoria(
            Context context,
            int hospitalizacionId,
            string userId,
            DateTime baseDate,
            string marker,
            string medicoNombre)
        {
            var anestNombre = ObtenerAnestesistaNombre(context, medicoNombre);
            var existente = context.NotaOperatoria
                .FirstOrDefault(n => n.HospitalizacionId == hospitalizacionId);

            if (existente != null)
            {
                if (string.IsNullOrWhiteSpace(existente.Anestesista)
                    || existente.Anestesista.Equals("Anestesiólogo asignado", StringComparison.OrdinalIgnoreCase))
                    existente.Anestesista = anestNombre;
                return;
            }

            context.NotaOperatoria.Add(new NotaOperatoria
            {
                HospitalizacionId = hospitalizacionId,
                UserId = userId,
                FechaRegistro = baseDate.AddHours(3),
                FechaOperacion = baseDate.AddHours(2),
                HoraComenzo = baseDate.AddHours(2).ToString("HH:mm"),
                HoraTermino = baseDate.AddHours(3).ToString("HH:mm"),
                Cirujano = medicoNombre,
                Anestesista = anestNombre,
                Diagnostico = $"{marker} Post artroplastia de rodilla izquierda",
                DiagnosticoPreOperatorio = "Gonartrosis severa rodilla izquierda",
                DiagnosticoPostOperatorio = "Post artroplastia total de rodilla izquierda",
                OperacionEfectuada = "Artroplastia de rodilla izquierda",
                Evolucion = "Procedimiento sin complicaciones. Traslado estable a recuperación.",
                HallazgosTransOperatorios = "Sin hallazgos adversos transoperatorios."
            });
        }

        private static void AsegurarSignosVitales(
            Context context,
            int hospitalizacionId,
            string userId,
            DateTime baseDate,
            string marker)
        {
            if (context.ExamenesFisicosHosp.Any(e => e.HospitalizacionId == hospitalizacionId))
                return;

            var catalogo = context.DatosExamenFisicoHosp.OrderBy(d => d.Id).Take(6).ToList();
            if (!catalogo.Any())
                return;

            var valoresDemo = new Dictionary<int, string>
            {
                { 1, "118/76" },
                { 2, "120/78" },
                { 3, "36.6 °C" },
                { 4, "78 lpm" },
                { 5, "16 rpm" },
                { 6, "98 %" }
            };

            var datos = catalogo.Select(d => new ExamenFisicoHospDato
            {
                DatoExamenFisicoHospId = d.Id,
                Valor = valoresDemo.TryGetValue(d.Id, out var v) ? v : "Normal"
            }).ToList();

            context.ExamenesFisicosHosp.Add(new ExamenFisicoHosp
            {
                HospitalizacionId = hospitalizacionId,
                FechaHora = baseDate.AddHours(12),
                UsuarioToma = userId,
                Observaciones = $"{marker} Signos vitales postoperatorios estables.",
                Autorizado = true,
                FechaAutorizacion = baseDate.AddHours(12),
                UsuarioAutoriza = userId,
                ExamenesFisicosHospDatos = datos
            });
        }

        private static void EtiquetarNotasExistentes(Context context, int hospitalizacionId)
        {
            var notas = context.NotaMedica2
                .Where(n => n.HospitalizacionId == hospitalizacionId)
                .OrderBy(n => n.FechaRegistro)
                .ToList();
            if (!notas.Any())
                return;

            var tipos = new[] { TipoNotaClinica.Ingreso, TipoNotaClinica.Traslado, TipoNotaClinica.Recepcion, TipoNotaClinica.Egreso };
            if (notas.Count == 2)
            {
                if (string.IsNullOrWhiteSpace(notas[0].TipoNota))
                    notas[0].TipoNota = TipoNotaClinica.Ingreso;
                if (string.IsNullOrWhiteSpace(notas[1].TipoNota))
                    notas[1].TipoNota = TipoNotaClinica.Egreso;
                return;
            }

            for (var i = 0; i < notas.Count && i < tipos.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(notas[i].TipoNota))
                    notas[i].TipoNota = tipos[i];
            }
        }

        private static void EtiquetarNotasEnfermeriaExistentes(Context context, int hospitalizacionId)
        {
            var notas = context.NotaEnfermeria2
                .Where(n => n.HospitalizacionId == hospitalizacionId)
                .OrderBy(n => n.FechaRegistro)
                .ToList();
            if (!notas.Any())
                return;

            var tipos = new[] { TipoNotaClinica.Ingreso, TipoNotaClinica.Traslado, TipoNotaClinica.Recepcion, TipoNotaClinica.Egreso };
            for (var i = 0; i < notas.Count && i < tipos.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(notas[i].TipoNota))
                    notas[i].TipoNota = tipos[i];
            }
        }

        private static bool TieneNotaEvolucion(Context context, int hospitalizacionId, string tipo) =>
            context.NotaMedica2.Any(n =>
                n.HospitalizacionId == hospitalizacionId &&
                n.TipoNota == tipo);

        private static bool TieneNotaEnfermeria(Context context, int hospitalizacionId, string tipo) =>
            context.NotaEnfermeria2.Any(n =>
                n.HospitalizacionId == hospitalizacionId &&
                n.TipoNota == tipo);

        private static string ObtenerMedicoNombre(Context context, int hospitalizacionId)
        {
            var fromNota = context.NotaOperatoria
                .Where(n => n.HospitalizacionId == hospitalizacionId && n.Cirujano != null)
                .Select(n => n.Cirujano)
                .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(fromNota))
                return fromNota;

            return context.CuestionariosPreAnestesicos
                .Where(c => c.HospitalizacionId == hospitalizacionId && c.Cirujano != null)
                .OrderByDescending(c => c.FechaRegistro)
                .Select(c => c.Cirujano)
                .FirstOrDefault();
        }

        private static string FormatearNombreEmpleado(string nombre, string apellido)
        {
            var n = (nombre ?? "").Trim();
            var a = (apellido ?? "").Trim();
            if (string.IsNullOrEmpty(n)) return a;
            if (string.IsNullOrEmpty(a)) return n;
            return $"{n} {a}";
        }

        private static bool EsAnestesistaEmpleado(string nombre, string apellido)
        {
            return (!string.IsNullOrWhiteSpace(nombre) && nombre.Contains("Anest", StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(apellido) && apellido.Contains("Anest", StringComparison.OrdinalIgnoreCase));
        }

        private static string ObtenerAnestesistaNombre(Context context, string medicoNombreFallback)
        {
            var empleados = context.Empleados
                .Where(e => !e.Eliminado)
                .OrderBy(e => e.Id)
                .Select(e => new { e.Nombre, e.Apellido })
                .AsEnumerable();

            var anest = empleados.FirstOrDefault(e => EsAnestesistaEmpleado(e.Nombre, e.Apellido));
            if (anest != null)
                return FormatearNombreEmpleado(anest.Nombre, anest.Apellido);

            var otro = empleados
                .Select(e => FormatearNombreEmpleado(e.Nombre, e.Apellido))
                .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n) && n != medicoNombreFallback);

            return !string.IsNullOrWhiteSpace(otro) ? otro : "Dr. Anestesiólogo SerSalud";
        }

        private static void AsegurarAnestesistaEnCita(Context context, Hospitalizacion hosp, string medicoNombre)
        {
            var consulta = context.Consultas
                .Where(c => c.HospitalizacionId == hosp.Id)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            int? citaId = consulta?.CitasId;
            if (!citaId.HasValue || citaId.Value <= 0)
            {
                citaId = context.Citass
                    .Where(c => c.PacienteId == hosp.PacienteId && !c.Eliminado)
                    .OrderByDescending(c => c.Id)
                    .Select(c => c.Id)
                    .FirstOrDefault();
            }

            if (!citaId.HasValue || citaId.Value <= 0)
                return;

            var cita = context.Citass.Find(citaId.Value);
            if (cita == null)
                return;

            if (cita.AnestesistaId is > 0)
                return;

            if (!string.IsNullOrWhiteSpace(cita.Anestesista)
                && !cita.Anestesista.Equals("Nombre del Anestesista", StringComparison.OrdinalIgnoreCase)
                && !cita.Anestesista.Equals("Anestesiólogo asignado", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(cita.Anestesista, out _))
                    return;
                if (BuscarEmpleadoPorNombreEnLista(context, cita.Anestesista) != null)
                    return;
            }

            var anest = context.Empleados
                .Where(e => !e.Eliminado)
                .OrderBy(e => e.Id)
                .Select(e => new { e.Id, e.Nombre, e.Apellido })
                .AsEnumerable()
                .FirstOrDefault(e => EsAnestesistaEmpleado(e.Nombre, e.Apellido));

            if (anest == null)
            {
                var nombre = ObtenerAnestesistaNombre(context, medicoNombre);
                cita.Anestesista = nombre;
                return;
            }

            cita.AnestesistaId = anest.Id;
            cita.Anestesista = anest.Id.ToString();
        }

        private static Empleado BuscarEmpleadoPorNombreEnLista(Context context, string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return null;

            return context.Empleados
                .Where(e => !e.Eliminado)
                .AsEnumerable()
                .FirstOrDefault(e =>
                {
                    var full = FormatearNombreEmpleado(e.Nombre, e.Apellido);
                    return string.Equals(full, nombre.Trim(), StringComparison.OrdinalIgnoreCase)
                        || full.Contains(nombre.Trim(), StringComparison.OrdinalIgnoreCase);
                });
        }

        private static void AsegurarRegistroAnestesia(
            Context context,
            Hospitalizacion hosp,
            int hospitalizacionId,
            DateTime baseDate,
            string marker,
            string medicoNombre,
            ILogger logger)
        {
            var procedimiento = context.CuestionariosPreAnestesicos
                .Where(c => c.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(c => c.FechaRegistro)
                .Select(c => c.ProcedimientoProgramado)
                .FirstOrDefault()
                ?? "Artroplastia de rodilla izquierda";

            var fecha = baseDate.ToString("yyyy-MM-dd");
            var json = $@"{{
  ""fecha"": ""{fecha}"",
  ""qx"": ""{procedimiento}"",
  ""radios"": {{
    ""an_prep_identificacion"": ""SI"",
    ""an_prep_expediente"": ""SI"",
    ""an_prep_consentimiento"": ""SI"",
    ""an_prep_miembro"": ""SI"",
    ""an_seg_equipo"": ""SI"",
    ""an_seg_ocular"": ""SI"",
    ""an_mon_esfigmo"": ""SI"",
    ""an_mon_ekg"": ""SI"",
    ""an_mon_oximetro"": ""SI"",
    ""an_maquina"": ""SI""
  }},
  ""campos"": {{
    ""an_peso"": ""72"",
    ""an_talla"": ""1.68"",
    ""an_asa"": ""II"",
    ""an_tecnica"": ""General balanceada"",
    ""an_imc"": ""25.51"",
    ""an_ayuno"": ""8 horas"",
    ""an_posicion"": ""Decúbito supino"",
    ""an_induccion_tipo"": ""IV"",
    ""an_induccion_detalle"": ""Propofol + Fentanilo"",
    ""an_bal_cristaloides"": ""1000 ml Hartmann"",
    ""an_bal_orina"": ""400 ml"",
    ""an_hora_inicio_anestesia"": ""{baseDate.AddHours(2):HH:mm}"",
    ""an_hora_inicio_cirugia"": ""{baseDate.AddHours(2).AddMinutes(30):HH:mm}"",
    ""an_hora_fin_cirugia"": ""{baseDate.AddHours(5):HH:mm}"",
    ""an_hora_fin_anestesia"": ""{baseDate.AddHours(5).AddMinutes(15):HH:mm}""
  }},
  ""medicamentosGrid"": {{
    ""filas"": [
      {{ ""item"": ""Propofol"", ""valores"": [""200""], ""nota"": ""Inducción"" }},
      {{ ""item"": ""Fentanilo"", ""valores"": [""100""], ""nota"": ""Analgesia intraoperatoria"" }},
      {{ ""item"": ""Rocuronio"", ""valores"": [""50""], ""nota"": ""Relajación muscular"" }}
    ]
  }},
  ""nota"": ""{marker} Registro anestésico transoperatorio. Anestesiólogo: {ObtenerAnestesistaNombre(context, medicoNombre)}""
}}";

            var existente = context.RegistrosAnestesia.FirstOrDefault(r => r.HospitalizacionId == hospitalizacionId);
            var necesitaActualizar = existente == null
                || string.IsNullOrWhiteSpace(existente.DatosJson)
                || !existente.DatosJson.Contains("\"fecha\"")
                || !existente.DatosJson.Contains("\"qx\"");

            if (!necesitaActualizar)
                return;

            var userId = context.Users.Select(u => u.Id).FirstOrDefault()
                ?? context.NotaMedica2.Where(n => n.HospitalizacionId == hospitalizacionId).Select(n => n.ProfesionalId).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
                return;

            if (existente == null)
            {
                context.RegistrosAnestesia.Add(new RegistroAnestesia
                {
                    HospitalizacionId = hospitalizacionId,
                    UserId = userId,
                    FechaRegistro = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                    DatosJson = json
                });
            }
            else
            {
                existente.DatosJson = json;
                existente.FechaActualizacion = DateTime.Now;
            }
        }

        private static void AsegurarDocumentosYMedicamentos(Context context, Hospitalizacion hosp, int hospitalizacionId, string marker, ILogger logger)
        {
            var urlLab = $"/uploads/expediente/laboratorios-hosp-{hospitalizacionId}.pdf";
            var urlRx = $"/uploads/expediente/rx-rodilla-hosp-{hospitalizacionId}.pdf";

            if (!context.PacienteArchivos.Any(a => a.PacienteId == hosp.PacienteId && a.UrlArchivo == urlLab))
            {
                context.PacienteArchivos.Add(new PacienteArchivo
                {
                    PacienteId = hosp.PacienteId,
                    NombreArchivo = "Laboratorios preoperatorios.pdf",
                    UrlArchivo = urlLab
                });
            }

            if (!context.PacienteArchivos.Any(a => a.PacienteId == hosp.PacienteId && a.UrlArchivo == urlRx))
            {
                context.PacienteArchivos.Add(new PacienteArchivo
                {
                    PacienteId = hosp.PacienteId,
                    NombreArchivo = "Radiografía rodilla.pdf",
                    UrlArchivo = urlRx
                });
            }

            if (context.MedicamentosNoControlado.Any(m => m.HospitalizacionId == hospitalizacionId && !m.Eliminado))
                return;

            var producto = context.Productos
                .Where(p => !p.Eliminado && p.NombreProducto != null && p.NombreProducto.Contains("Fentanilo"))
                .OrderBy(p => p.Id)
                .FirstOrDefault()
                ?? context.Productos.Where(p => !p.Eliminado).OrderBy(p => p.Id).FirstOrDefault();

            if (producto == null)
            {
                logger?.LogWarning("Expediente demo seed: no hay productos para medicamentos controlados (hosp {Id}).", hospitalizacionId);
                return;
            }

            var userId = context.Users.Select(u => u.Id).FirstOrDefault();
            context.MedicamentosNoControlado.Add(new MedicamentoNoControlado
            {
                HospitalizacionId = hospitalizacionId,
                ProductoId = producto.Id,
                ProductoNombre = producto.NombreProducto,
                FechaProcedimiento = hosp.FechaInicio,
                FechaRegistro = DateTime.Now,
                UnidadesIniciales = 2,
                UnidadesExtra = 0,
                Utilizado = 1,
                Descartado = 0,
                Retornadas = 1,
                UsuarioRegistroId = userId,
                Eliminado = false
            });
        }

        private static void AsegurarCuestionarioPreAnestesico(
            Context context,
            Hospitalizacion hosp,
            int hospitalizacionId,
            string userId,
            DateTime baseDate,
            string marker,
            string medicoNombre)
        {
            var paciente = hosp.Paciente ?? context.Pacientes.Find(hosp.PacienteId);
            var existente = context.CuestionariosPreAnestesicos
                .Where(c => c.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(c => c.FechaRegistro)
                .FirstOrDefault();

            var necesitaDatos = existente == null
                || !existente.Peso.HasValue
                || string.IsNullOrWhiteSpace(existente.PA_Alergia);

            if (!necesitaDatos)
                return;

            if (existente == null)
            {
                context.CuestionariosPreAnestesicos.Add(new CuestionarioPreAnestesico
                {
                    HospitalizacionId = hospitalizacionId,
                    FechaRegistro = baseDate.AddHours(-2),
                    UserId = userId,
                    NombreCompleto = paciente?.Nombre ?? "Paciente demo",
                    RegistroMedico = paciente?.Dpi,
                    Edad = paciente?.FechaNacimiento != null
                        ? ((int)((DateTime.Now - paciente.FechaNacimiento.Value).TotalDays / 365.25)).ToString()
                        : "45",
                    FechaCuestionario = baseDate.AddHours(-2),
                    Peso = 72,
                    Estatura = 1.68,
                    FechaProcedimiento = baseDate,
                    ProcedimientoProgramado = "Artroplastia de rodilla izquierda",
                    Cirujano = medicoNombre,
                    PA_Alergia = "NO",
                    PA_Fuma = "NO",
                    PA_Drogas = "NO",
                    PA_Alcohol = "NO",
                    PA_Embarazo = "NO",
                    PA_Transfusion = "NO",
                    PA_Asma = "NO",
                    PA_Pulmones = "NO",
                    PA_Corazon = "NO",
                    PA_AtaqueCardiaco = "NO",
                    PA_Angina = "NO",
                    PA_Soplo = "NO",
                    PA_Presion = "NO",
                    PA_Higado = "NO",
                    PA_Rinones = "NO",
                    PA_Diabetes = "NO",
                    PA_Epilepsia = "NO",
                    PA_Derrame = "NO",
                    PA_Tiroides = "NO",
                    PA_Anestesico = "NO",
                    PA_AceptaTransfusion = "SI",
                    AI_Medicamentos = "Losartan 50 mg c/24h",
                    AI_Comentarios = $"{marker} Evaluación preanestésica favorable para cirugía programada."
                });
                return;
            }

            if (!existente.Peso.HasValue) existente.Peso = 72;
            if (!existente.Estatura.HasValue) existente.Estatura = 1.68;
            if (!existente.FechaProcedimiento.HasValue) existente.FechaProcedimiento = baseDate;
            if (string.IsNullOrWhiteSpace(existente.ProcedimientoProgramado))
                existente.ProcedimientoProgramado = "Artroplastia de rodilla izquierda";
            if (string.IsNullOrWhiteSpace(existente.Cirujano))
                existente.Cirujano = medicoNombre;
            if (string.IsNullOrWhiteSpace(existente.RegistroMedico))
                existente.RegistroMedico = paciente?.Dpi;

            if (string.IsNullOrWhiteSpace(existente.PA_Alergia)) existente.PA_Alergia = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Fuma)) existente.PA_Fuma = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Drogas)) existente.PA_Drogas = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Alcohol)) existente.PA_Alcohol = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Embarazo)) existente.PA_Embarazo = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Transfusion)) existente.PA_Transfusion = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Asma)) existente.PA_Asma = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Pulmones)) existente.PA_Pulmones = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Corazon)) existente.PA_Corazon = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_AtaqueCardiaco)) existente.PA_AtaqueCardiaco = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Angina)) existente.PA_Angina = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Soplo)) existente.PA_Soplo = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Presion)) existente.PA_Presion = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Higado)) existente.PA_Higado = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Rinones)) existente.PA_Rinones = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Diabetes)) existente.PA_Diabetes = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Epilepsia)) existente.PA_Epilepsia = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Derrame)) existente.PA_Derrame = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Tiroides)) existente.PA_Tiroides = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_Anestesico)) existente.PA_Anestesico = "NO";
            if (string.IsNullOrWhiteSpace(existente.PA_AceptaTransfusion))
                existente.PA_AceptaTransfusion = "SI";
            if (string.IsNullOrWhiteSpace(existente.AI_Medicamentos))
                existente.AI_Medicamentos = "Losartan 50 mg c/24h";
        }

        private static void AsegurarListaChequeo(
            Context context,
            Hospitalizacion hosp,
            int hospitalizacionId,
            string userId,
            DateTime baseDate,
            string marker,
            string medicoNombre)
        {
            var paciente = hosp.Paciente ?? context.Pacientes.Find(hosp.PacienteId);
            var nombreParts = (paciente?.Nombre ?? "Paciente Demo").Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var primerNombre = nombreParts.Length > 0 ? nombreParts[0] : "Paciente";
            var restoNombre = nombreParts.Length > 1 ? nombreParts[1] : "Demo";

            var existente = context.ListasChequeo
                .Where(l => l.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(l => l.FechaRegistro)
                .FirstOrDefault();

            var necesitaDatos = existente == null
                || string.IsNullOrWhiteSpace(existente.MedicoTratante)
                || string.IsNullOrWhiteSpace(existente.CI_EvalPreanestesica);

            if (!necesitaDatos)
                return;

            if (existente == null)
            {
                context.ListasChequeo.Add(new ListaChequeo
                {
                    HospitalizacionId = hospitalizacionId,
                    FechaRegistro = baseDate.AddHours(2),
                    UserId = userId,
                    NombrePaciente = primerNombre,
                    ApellidoPaciente = restoNombre,
                    FechaNacimiento = paciente?.FechaNacimiento,
                    FechaChequeo = baseDate.AddHours(2),
                    HoraChequeo = baseDate.AddHours(2).ToString("HH:mm"),
                    MedicoTratante = medicoNombre,
                    CI_NombreConfirma = primerNombre,
                    CI_ApellidoConfirma = restoNombre,
                    CI_FechaNacConfirma = paciente?.FechaNacimiento,
                    CI_Consentimiento = "SI",
                    CI_Operacion = "Artroplastia de rodilla izquierda",
                    CI_LadoOperar = "Izquierda",
                    CI_SitioMarcado = "SI",
                    CI_Alergia = "NO",
                    CI_EvalPreanestesica = "SI",
                    CI_AccesoIV = "SI",
                    CI_EquipoAnestesia = "SI",
                    CI_Medicamentos = "SI",
                    CI_Oximetro = "SI",
                    CI_EquipoAspiracion = "SI",
                    CI_ViaAerea = "SI",
                    CP_Presentacion = "SI",
                    CP_NombrePacienteCirujano = primerNombre,
                    CP_ApellidoPacienteCirujano = restoNombre,
                    CP_FechaNacCirujano = paciente?.FechaNacimiento,
                    CP_NombreCirugia = "Artroplastia de rodilla izquierda",
                    CP_EventosCriticos = "NO",
                    CP_Esterilidad = "SI",
                    CP_EventosCriticosAnest = "NO",
                    CP_ProfilaxisAntibiotica = "SI",
                    CP_Tromboprofilaxis = "SI",
                    CP_ManejoDolor = "SI",
                    CS_NombreOperacion = "SI",
                    CS_RecuentoCompleto = "SI",
                    CS_RepasoPostOp = "SI",
                    CS_Traslado = "SI",
                    CS_Complicaciones = "NO",
                    CS_ServicioNumCama = $"{marker} Check list quirúrgico completo."
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(existente.MedicoTratante))
                existente.MedicoTratante = medicoNombre;
            if (!existente.CI_FechaNacConfirma.HasValue)
                existente.CI_FechaNacConfirma = paciente?.FechaNacimiento ?? existente.FechaNacimiento;
            if (!existente.CP_FechaNacCirujano.HasValue)
                existente.CP_FechaNacCirujano = paciente?.FechaNacimiento ?? existente.FechaNacimiento;

            if (string.IsNullOrWhiteSpace(existente.CI_EvalPreanestesica)) existente.CI_EvalPreanestesica = "SI";
            if (string.IsNullOrWhiteSpace(existente.CI_AccesoIV)) existente.CI_AccesoIV = "SI";
            if (string.IsNullOrWhiteSpace(existente.CI_EquipoAnestesia)) existente.CI_EquipoAnestesia = "SI";
            if (string.IsNullOrWhiteSpace(existente.CI_Medicamentos)) existente.CI_Medicamentos = "SI";
            if (string.IsNullOrWhiteSpace(existente.CI_Oximetro)) existente.CI_Oximetro = "SI";
            if (string.IsNullOrWhiteSpace(existente.CI_EquipoAspiracion)) existente.CI_EquipoAspiracion = "SI";
            if (string.IsNullOrWhiteSpace(existente.CI_ViaAerea)) existente.CI_ViaAerea = "SI";
            if (string.IsNullOrWhiteSpace(existente.CP_ProfilaxisAntibiotica)) existente.CP_ProfilaxisAntibiotica = "SI";
            if (string.IsNullOrWhiteSpace(existente.CP_Tromboprofilaxis)) existente.CP_Tromboprofilaxis = "SI";
            if (string.IsNullOrWhiteSpace(existente.CP_ManejoDolor)) existente.CP_ManejoDolor = "SI";
            if (string.IsNullOrWhiteSpace(existente.CP_EventosCriticosAnest)) existente.CP_EventosCriticosAnest = "NO";
        }

        private static void AsegurarConsentimientoDemo(
            Context context,
            Hospitalizacion hosp,
            string marker,
            string medicoNombre)
        {
            var hospIdStr = hosp.Id.ToString();
            var existente = context.ConsentimientoHospi.FirstOrDefault(c =>
                c.PacienteId == hosp.PacienteId &&
                (c.HospitalizacionId == hospIdStr || c.HabitacionId == hosp.HabitacionId));

            var paciente = hosp.Paciente ?? context.Pacientes.Find(hosp.PacienteId);

            if (existente != null)
            {
                if (string.IsNullOrWhiteSpace(existente.NombreMedicoTratante))
                    existente.NombreMedicoTratante = medicoNombre;
                if (!string.IsNullOrWhiteSpace(existente.DPIResponsable)
                    && string.Equals(existente.DPIResponsable, existente.DPI, StringComparison.OrdinalIgnoreCase))
                {
                    existente.DPIResponsable = null;
                }
                return;
            }

            context.ConsentimientoHospi.Add(new ConsentimientoHospi
            {
                PacienteId = hosp.PacienteId,
                HabitacionId = hosp.HabitacionId,
                HospitalizacionId = hospIdStr,
                HoraIngreso = hosp.FechaInicio.ToString("dd/MM/yyyy HH:mm"),
                NumeroHabitacion = hosp.Habitacion?.NombreNumeroHabitacion ?? "-",
                NombreCompleto = paciente?.Nombre ?? "Paciente demo",
                DPI = paciente?.Dpi ?? "0000000000000",
                Edad = paciente?.FechaNacimiento != null
                    ? ((int)((DateTime.Now - paciente.FechaNacimiento.Value).TotalDays / 365.25)).ToString()
                    : "45",
                TipoSangre = paciente?.TipoDeSangre ?? "O+",
                Genero = paciente?.sexoText ?? "No especificado",
                PoseeSeguroMedico = "SI",
                Aseguradora = "Seguro demo",
                TratamientoMedico = $"{marker} Artroplastia de rodilla izquierda",
                NombreMedicoTratante = medicoNombre,
                HospitalProporcionoMedico = "SI",
                MedicoAfiliado = "SI"
            });
        }

        private static void EnsureDemoDocumentoPdfTemplate(string filePath, string tipo)
        {
            if (File.Exists(filePath) && !EsPdfDemoLegado(filePath))
                return;

            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

            var firmaCursiva = tipo == "radiografia" ? "M. E. Radiologa" : "C. Pathologist";
            var lineas = tipo == "radiografia"
                ? new[]
                {
                    ("SER SALUD - INFORME RADIOLOGICO", 14),
                    ("Estudio: Radiografia de rodilla izquierda AP y lateral", 11),
                    ("Paciente: Paciente demo expediente", 11),
                    ("Fecha del estudio: " + DateTime.Now.ToString("dd/MM/yyyy"), 11),
                    ("", 11),
                    ("Hallazgos: Disminucion del espacio articular femorotibial.", 11),
                    ("Osteofitos marginales compatibles con gonartrosis.", 11),
                    ("No se observan fracturas ni luxaciones.", 11),
                    ("", 11),
                    ("Impresion diagnostica: Gonartrosis severa rodilla izquierda.", 11),
                    ("", 11),
                    ("", 11),
                    ("Firma del radiologo responsable:", 10),
                    ("Dr. Maria Elena Radiologa SerSalud", 11),
                    ("Colegiado No. 55678", 10),
                }
                : new[]
                {
                    ("SER SALUD - RESULTADOS DE LABORATORIO", 14),
                    ("Paciente: Paciente demo expediente", 11),
                    ("Examen: Hemograma completo / Quimica sanguinea preoperatoria", 11),
                    ("Fecha de toma: " + DateTime.Now.ToString("dd/MM/yyyy"), 11),
                    ("", 11),
                    ("Hemoglobina: 14.2 g/dL     Hematocrito: 42%", 11),
                    ("Leucocitos: 7,200 /uL      Plaquetas: 245,000 /uL", 11),
                    ("Glucosa: 98 mg/dL          Creatinina: 0.9 mg/dL", 11),
                    ("Tiempo de protrombina: 12.5 seg (INR 1.0)", 11),
                    ("", 11),
                    ("Conclusion: Valores dentro de limites aceptables para cirugia programada.", 11),
                    ("", 11),
                    ("", 11),
                    ("Firma del medico responsable:", 10),
                    ("Dr. Carlos Pathologist SerSalud", 11),
                    ("Colegiado No. 78901", 10),
                };

            var content = new System.Text.StringBuilder();
            var y = 750;
            foreach (var (texto, size) in lineas)
            {
                if (string.IsNullOrEmpty(texto))
                {
                    y -= 12;
                    continue;
                }

                content.Append("BT /F1 ").Append(size).Append(" Tf 72 ").Append(y)
                    .Append(" Td (").Append(EscapePdfText(texto)).Append(") Tj ET\n");
                y -= size >= 14 ? 28 : 20;
            }

            AppendBloqueFirmaDemo(content, firmaCursiva);
            WriteSimplePdf(filePath, content.ToString());
        }

        private static void AppendBloqueFirmaDemo(System.Text.StringBuilder content, string firmaCursiva)
        {
            content.Append("0.8 w 72 108 m 320 108 l S\n");
            content.Append("BT /F1 9 Tf 72 92 Td (Firma:) Tj ET\n");
            content.Append("1 w 72 86 200 42 re S\n");
            content.Append("q 0.45 w 0 0 0.55 RG\n");
            content.Append("82 112 m 98 118 112 104 128 111 c 144 118 158 103 174 110 c 190 117 206 102 222 109 c 238 116 252 105 266 111 c S\n");
            content.Append("82 104 m 96 110 110 98 126 105 c 142 112 158 97 174 103 c 190 109 206 96 220 102 c S\n");
            content.Append("Q\n");
            content.Append("BT /F2 15 Tf 1 0 0 1 88 98 Tm (")
                .Append(EscapePdfText(firmaCursiva))
                .Append(") Tj ET\n");
            content.Append("% SIG_DEMO_V2\n");
        }

        private static bool EsPdfDemoLegado(string filePath)
        {
            try
            {
                var text = File.ReadAllText(filePath);
                if (!text.Contains("SIG_DEMO_V2", StringComparison.Ordinal))
                    return true;
                if (text.Contains("- demo)", StringComparison.Ordinal) || text.Contains("Radiografia rodilla - demo", StringComparison.Ordinal))
                    return true;
                return text.Length < 1800 && !text.Contains("Colegiado", StringComparison.Ordinal);
            }
            catch
            {
                return true;
            }
        }

        private static string EscapePdfText(string text)
        {
            return (text ?? "")
                .Replace("\\", "\\\\", StringComparison.Ordinal)
                .Replace("(", "\\(", StringComparison.Ordinal)
                .Replace(")", "\\)", StringComparison.Ordinal);
        }

        private static void WriteSimplePdf(string filePath, string contentStream)
        {
            using var ms = new MemoryStream();
            var offsets = new List<long> { 0 };

            void WriteAscii(string s)
            {
                offsets.Add(ms.Position);
                var bytes = System.Text.Encoding.ASCII.GetBytes(s);
                ms.Write(bytes, 0, bytes.Length);
            }

            ms.Write(System.Text.Encoding.ASCII.GetBytes("%PDF-1.4\n"));
            WriteAscii("1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj\n");
            WriteAscii("2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj\n");
            WriteAscii("3 0 obj<</Type/Page/Parent 2 0 R/MediaBox[0 0 612 792]/Contents 4 0 R/Resources<</Font<</F1 5 0 R/F2 6 0 R>>>>>>endobj\n");

            var streamObj = $"4 0 obj<</Length {System.Text.Encoding.ASCII.GetByteCount(contentStream)}>>stream\n{contentStream}\nendstream\nendobj\n";
            WriteAscii(streamObj);
            WriteAscii("5 0 obj<</Type/Font/Subtype/Type1/BaseFont/Helvetica>>endobj\n");
            WriteAscii("6 0 obj<</Type/Font/Subtype/Type1/BaseFont/Helvetica-Oblique>>endobj\n");

            var xrefPos = ms.Position;
            var objCount = offsets.Count - 1;
            ms.Write(System.Text.Encoding.ASCII.GetBytes($"xref\n0 {objCount + 1}\n"));
            ms.Write(System.Text.Encoding.ASCII.GetBytes("0000000000 65535 f \n"));
            for (var i = 1; i <= objCount; i++)
                ms.Write(System.Text.Encoding.ASCII.GetBytes($"{offsets[i]:D10} 00000 n \n"));

            ms.Write(System.Text.Encoding.ASCII.GetBytes($"trailer<</Size {objCount + 1}/Root 1 0 R>>\n"));
            ms.Write(System.Text.Encoding.ASCII.GetBytes($"startxref\n{xrefPos}\n%%EOF"));

            File.WriteAllBytes(filePath, ms.ToArray());
        }

        private static void AsegurarVinculoConsultaHospitalizacion(Context context, Hospitalizacion hosp)
        {
            if (context.Consultas.Any(c => c.HospitalizacionId == hosp.Id))
                return;

            var citaId = context.Citass
                .Where(c => c.PacienteId == hosp.PacienteId && !c.Eliminado)
                .OrderByDescending(c => c.Id)
                .Select(c => c.Id)
                .FirstOrDefault();

            Consulta consulta = null;
            if (citaId > 0)
            {
                consulta = context.Consultas
                    .Where(c => c.CitasId == citaId)
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();
            }

            if (consulta == null)
                return;

            consulta.HospitalizacionId = hosp.Id;
            consulta.Hospitalizado = true;
        }

        private static void VincularConsentimientoHospitalizacion(Context context, Hospitalizacion hosp)
        {
            var hospIdStr = hosp.Id.ToString();
            var consent = context.ConsentimientoHospi
                .Where(c => c.PacienteId == hosp.PacienteId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault(c =>
                    c.HospitalizacionId == hospIdStr
                    || c.HabitacionId == hosp.HabitacionId
                    || string.IsNullOrWhiteSpace(c.HospitalizacionId));

            if (consent == null)
                return;

            if (string.IsNullOrWhiteSpace(consent.HospitalizacionId) || consent.HospitalizacionId != hospIdStr)
                consent.HospitalizacionId = hospIdStr;

            if (consent.HabitacionId != hosp.HabitacionId)
                consent.HabitacionId = hosp.HabitacionId;
        }

        private static void CopyPdfIfLegacyOrMissing(string sourcePath, string targetPath, ILogger logger)
        {
            if (File.Exists(targetPath) && !EsPdfDemoLegado(targetPath))
                return;

            if (!File.Exists(sourcePath))
            {
                logger?.LogWarning("Expediente demo seed: plantilla PDF no encontrada: {Path}", sourcePath);
                return;
            }

            File.Copy(sourcePath, targetPath, overwrite: true);
            logger?.LogInformation("Expediente demo seed: actualizado {Target}", targetPath);
        }

        private static void CopyPdfIfMissing(string sourcePath, string targetPath, ILogger logger)
        {
            if (File.Exists(targetPath))
                return;

            if (!File.Exists(sourcePath))
            {
                logger?.LogWarning("Expediente demo seed: plantilla PDF no encontrada: {Path}", sourcePath);
                return;
            }

            File.Copy(sourcePath, targetPath);
            logger?.LogInformation("Expediente demo seed: copiado {Target}", targetPath);
        }

        private static NotaMedica2 CrearNotaEvolucion(int hospitalizacionId, string userId, string tipo, DateTime fecha, string texto) =>
            new NotaMedica2
            {
                HospitalizacionId = hospitalizacionId,
                ProfesionalId = userId,
                TipoNota = tipo,
                FechaRegistro = fecha,
                Diagnostico = texto,
                Autorizado = true,
                FechaAutorizacion = fecha
            };

        private static NotaEnfermeria2 CrearNotaEnfermeria(int hospitalizacionId, string userId, int turnoId, string tipo, DateTime fecha, string texto) =>
            new NotaEnfermeria2
            {
                HospitalizacionId = hospitalizacionId,
                UserId = userId,
                TurnoEnfermeriaId = turnoId,
                TipoNota = tipo,
                FechaRegistro = fecha,
                Diagnostico = texto,
                Firmado = false
            };
    }
}
