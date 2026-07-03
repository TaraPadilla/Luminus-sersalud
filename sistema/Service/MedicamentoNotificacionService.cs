using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Database.Shared;
using Database.Shared.Models;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using farmamest.Service.IService;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace farmamest.Service
{
    public class MedicamentoNotificacionService : IMedicamentoNotificacionService
    {
        private readonly Context _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MedicamentoNotificacionService> _logger;
        private static readonly string HospitalTimeZoneId = "America/Guatemala";
        private static readonly TimeZoneInfo HospitalTimeZone = TimeZoneInfo.FindSystemTimeZoneById(HospitalTimeZoneId);
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public MedicamentoNotificacionService(
            Context db,
            IHttpClientFactory httpClientFactory,
            ILogger<MedicamentoNotificacionService> logger,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _env = env;

        }

        // ============================================================
        // MÉTODO PRINCIPAL (público)
        // ============================================================
        public async Task ProgramarNotificacionesAsync(
            int hospitalizacionId,
            string nombreProducto,
            decimal cantidad,
            string indicaciones,
            string viaAdministracion,
            string frecuenciaAdministracion,
            string fechaHoraAplicacionManual,
            string baseUrl,
            string usuarioSolicitanteId = null)
        {
            try
            {
                // 1. Obtener emails de los médicos asociados a la hospitalización
                var emails = await ObtenerEmailsMedicosDesdeCitaAsync(hospitalizacionId);
                if (emails == null || emails.Count == 0)
                {
                    _logger.LogInformation("No hay médicos asociados a la hospitalización {HospId}. No se enviarán correos.", hospitalizacionId);
                    return;
                }

                // 2. Obtener nombre del paciente
                var pacienteNombre = await _db.Hospitalizaciones
                    .Where(h => h.Id == hospitalizacionId)
                    .Select(h => h.Paciente.Nombre)
                    .FirstOrDefaultAsync() ?? "Paciente";

                // 3. Parsear la fecha/hora programada (UTC)
                DateTime? utcScheduledTime = ParseFechaHoraAplicacion(fechaHoraAplicacionManual);
                TimeSpan initialDelay = TimeSpan.Zero;
                DateTime nowUtc = DateTime.UtcNow;

                if (utcScheduledTime.HasValue)
                {
                    if (utcScheduledTime.Value > nowUtc)
                    {
                        initialDelay = utcScheduledTime.Value - nowUtc;
                        _logger.LogInformation("Primera dosis programada para UTC {Scheduled}, retraso de {Delay} minutos.", utcScheduledTime, initialDelay.TotalMinutes);
                    }
                    else
                    {
                        if (frecuenciaAdministracion.StartsWith("Cada", StringComparison.OrdinalIgnoreCase))
                        {
                            var localOriginal = TimeZoneInfo.ConvertTimeFromUtc(utcScheduledTime.Value, HospitalTimeZone);
                            var nextLocal = localOriginal.AddDays(1);
                            utcScheduledTime = TimeZoneInfo.ConvertTimeToUtc(nextLocal, HospitalTimeZone);
                            initialDelay = utcScheduledTime.Value - nowUtc;
                            _logger.LogInformation("Hora pasada y recurrente → reprogramado para mañana. Nuevo retraso {Delay} minutos.", initialDelay.TotalMinutes);
                        }
                        else
                        {
                            _logger.LogInformation("Hora pasada y no recurrente → se enviará inmediatamente.");
                            utcScheduledTime = null;
                        }
                    }
                }

                // 4. Construir objeto simulado (solo para reutilizar el cuerpo del correo)
                var solicitudMock = new SolicitudMedicamento
                {
                    NombreProducto = nombreProducto,
                    Cantidad = (int)cantidad,
                    ViaAdministracion = viaAdministracion,
                    FrecuenciaAdministracion = frecuenciaAdministracion,
                    Indicaciones = indicaciones,
                    HospitalizacionId = hospitalizacionId
                };

                // 5. Lógica según frecuencia
                if (frecuenciaAdministracion == "STAT")
                {
                    var body = await CargarYReemplazarPlantillaAsync(
                        hospitalizacionId,
                        pacienteNombre,
                        nombreProducto,
                        (int)cantidad,
                        indicaciones,
                        viaAdministracion,
                        frecuenciaAdministracion,
                        null,
                        null,
                        fechaHoraAplicacionManual,
                        baseUrl);                    // var body = ObtenerCuerpoCorreo(solicitudMock, pacienteNombre, null, null);
                    if (initialDelay > TimeSpan.Zero)
                    {
                        BackgroundJob.Schedule(() =>
            EnviarCorreoBaseAsync(emails,
                $"Recordatorio de medicación (1/1) - {nombreProducto} para {pacienteNombre}",
                body, pacienteNombre, nombreProducto, baseUrl),
            initialDelay);
                        _logger.LogInformation("STAT programado con retraso de {Delay} min.", initialDelay.TotalMinutes);
                    }
                    else
                    {
                        await EnviarCorreoBaseAsync(emails,
                                   $"Recordatorio de medicación (1/1) - {nombreProducto} para {pacienteNombre}",
                                   body, pacienteNombre, nombreProducto, baseUrl);
                        _logger.LogInformation("STAT enviado inmediatamente.");
                    }
                }
                else if (frecuenciaAdministracion.Contains("Cada"))
                {
                    var match = Regex.Match(frecuenciaAdministracion, @"\d+");
                    if (match.Success)
                    {
                        int valor = int.Parse(match.Value);
                        string frecuenciaLower = frecuenciaAdministracion.ToLower();
                        int intervaloMinutos = 0;
                        if (frecuenciaLower.Contains("minuto"))
                            intervaloMinutos = valor;
                        else if (frecuenciaLower.Contains("hora"))
                            intervaloMinutos = valor * 60;
                        else
                            intervaloMinutos = valor * 60;

                        int cantidadInt = (int)cantidad;
                        if (intervaloMinutos > 0 && cantidadInt > 0)
                        {
                            // Primera dosis
                            var bodyFirst = await CargarYReemplazarPlantillaAsync(
    hospitalizacionId,
    pacienteNombre,
    nombreProducto,
    (int)cantidad,
    indicaciones,
    viaAdministracion,
    frecuenciaAdministracion,
    1,           // dosisActual
    cantidadInt, // dosisTotal
    fechaHoraAplicacionManual,
    baseUrl);
                            // var bodyFirst = ObtenerCuerpoCorreo(solicitudMock, pacienteNombre, 1, cantidadInt);
                            if (initialDelay > TimeSpan.Zero)
                            {
                                BackgroundJob.Schedule(() =>
    EnviarCorreoBaseAsync(emails,
        $"Recordatorio de medicación (1/{cantidadInt}) - {nombreProducto} para {pacienteNombre}",
        bodyFirst, pacienteNombre, nombreProducto, baseUrl),
    initialDelay);
                                _logger.LogInformation("Dosis 1/{Total} programada con retraso {Delay} min.", cantidadInt, initialDelay.TotalMinutes);
                            }
                            else
                            {
                                await EnviarCorreoBaseAsync(emails,
                                      $"Recordatorio de medicación (1/{cantidadInt}) - {nombreProducto} para {pacienteNombre}",
                                      bodyFirst, pacienteNombre, nombreProducto, baseUrl);
                                _logger.LogInformation("Dosis 1/{Total} enviada inmediatamente.", cantidadInt);
                            }

                            // Programar siguientes dosis
                            if (cantidadInt > 1)
                            {
                                TimeSpan delayForSecond = initialDelay + TimeSpan.FromMinutes(intervaloMinutos);
                                BackgroundJob.Schedule(() =>
     EnviarRecordatorioMedicamento(
         hospitalizacionId, pacienteNombre, nombreProducto, cantidad,
         indicaciones, viaAdministracion, 2, cantidadInt, intervaloMinutos,
         emails, nombreProducto, baseUrl, frecuenciaAdministracion), 
     delayForSecond);
                                _logger.LogInformation("Siguientes dosis programadas cada {Intervalo} minutos, primera repetición en {Delay} min.", intervaloMinutos, delayForSecond.TotalMinutes);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Intervalo o cantidad inválidos para frecuencia {Frecuencia}", frecuenciaAdministracion);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No se pudo extraer número de la frecuencia: {Frecuencia}", frecuenciaAdministracion);
                    }
                }
                else
                {
                    _logger.LogWarning("Frecuencia no reconocida: {Frecuencia}", frecuenciaAdministracion);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al programar notificaciones para hospitalización {HospId}", hospitalizacionId);
            }
        }

        // ============================================================
        // MÉTODO PARA ENVIAR RECORDATORIO (público, llamado por Hangfire)
        // ============================================================
        public async Task EnviarRecordatorioMedicamento(
            int hospitalizacionId,
            string pacienteNombre,
            string nombreProducto,
            decimal cantidad,
            string indicaciones,
            string viaAdministracion,
            int dosisActual,
            int cantidadTotal,
            int intervaloMinutos,
            List<string> emails,
            string medicamentoNombre,
            string baseUrl,
            string frecuenciaAdministracion = null) // ← PARÁMETRO NUEVO
        {
            try
            {
                // Usar frecuencia original si viene, sino construirla desde el intervalo
                string frecuencia = !string.IsNullOrEmpty(frecuenciaAdministracion)
                    ? frecuenciaAdministracion
                    : $"Cada {intervaloMinutos} minutos";

                var body = await CargarYReemplazarPlantillaAsync(
                    hospitalizacionId,
                    pacienteNombre,
                    nombreProducto,
                    (int)cantidad,
                    indicaciones,
                    viaAdministracion,
                    frecuencia,       // ← CORREGIDO (antes era intervaloMinutos.ToString())
                    dosisActual,
                    cantidadTotal,
                    "",
                    baseUrl);

                var subject = $"Recordatorio de medicación ({dosisActual}/{cantidadTotal}) - {medicamentoNombre} para {pacienteNombre}";
                await EnviarCorreoBaseAsync(emails, subject, body, pacienteNombre, medicamentoNombre, baseUrl);

                if (dosisActual < cantidadTotal)
                {
                    BackgroundJob.Schedule(() =>
                        EnviarRecordatorioMedicamento(
                            hospitalizacionId, pacienteNombre, nombreProducto, cantidad,
                            indicaciones, viaAdministracion, dosisActual + 1,
                            cantidadTotal,             // ← usa cantidadTotal (no cantidadInt)
                            intervaloMinutos, emails, medicamentoNombre, baseUrl, frecuencia),
                        TimeSpan.FromMinutes(intervaloMinutos)); // ← delay directo (no delayForSecond)
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en recordatorio de medicamento");
            }
        }

        // ============================================================
        // MÉTODO BASE PARA ENVIAR CORREO (público, llamado por Hangfire)
        // ============================================================
        public async Task EnviarCorreoBaseAsync(
            List<string> emailsDestino,
            string subject,
            string body,
            string pacienteNombre,
            string medicamento,
            string baseUrl)
        {
            if (emailsDestino == null || emailsDestino.Count == 0)
            {
                Console.WriteLine("[Correo] Lista de emails vacía, no se envía nada.");
                return;
            }

            Console.WriteLine($"[Correo] Iniciando envío. Destinatarios: {emailsDestino.Count}, Subject: {subject}, Body: {System.Text.Encoding.UTF8.GetByteCount(body)} bytes");

            using var client = _httpClientFactory.CreateClient();
            foreach (var email in emailsDestino)
            {
                Console.WriteLine($"[Correo] Enviando a {email}...");
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(subject), "Subject");
                form.Add(new StringContent(body), "Body");
                form.Add(new StringContent(email), "To");

                try
                {
                    var url = $"{baseUrl}/api/SendEmail";
                    Console.WriteLine($"[Correo] POST a {url}");

                    var response = await client.PostAsync(url, form);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"[Correo] ERROR {(int)response.StatusCode} enviando a {email}: {responseBody}");
                    }
                    else
                    {
                        Console.WriteLine($"[Correo] OK enviado a {email}. Respuesta: {responseBody}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Correo] EXCEPCIÓN enviando a {email}: {ex.Message}");
                }
            }

            // Guardar notificación en BD
            if (!string.IsNullOrEmpty(pacienteNombre) && !string.IsNullOrEmpty(medicamento))
            {
                try
                {
                    var horaGuatemala = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, HospitalTimeZone);
                    _db.Notificaciones.Add(new Notificacion
                    {
                        Titulo = "Recordatorio de medicación",
                        Mensaje = $"Administrar {medicamento} al paciente {pacienteNombre}",
                        FechaCreacion = horaGuatemala,
                        Leida = false
                    });
                    await _db.SaveChangesAsync();
                    Console.WriteLine($"[Correo] Notificación guardada en BD para {pacienteNombre}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Correo] ERROR guardando en BD: {ex.Message}");
                }
            }
        }

        // ============================================================
        // MÉTODOS PRIVADOS AUXILIARES
        // ============================================================
        private async Task<List<string>> ObtenerEmailsMedicosDesdeCitaAsync(int hospitalizacionId)
        {
            var cita = await ObtenerCitaAsociadaAsync(hospitalizacionId);
            if (cita == null) return new List<string>();

            var emails = new List<string>();

            if (cita.EmpleadoId.HasValue)
            {
                var emp = await _db.Empleados.FindAsync(cita.EmpleadoId.Value);
                if (emp?.Email != null) emails.Add(emp.Email);
            }
            if (cita.PrimerAyudanteId.HasValue)
            {
                var emp = await _db.Empleados.FindAsync(cita.PrimerAyudanteId.Value);
                if (emp?.Email != null) emails.Add(emp.Email);
            }
            if (cita.SegundoAyudanteId.HasValue)
            {
                var emp = await _db.Empleados.FindAsync(cita.SegundoAyudanteId.Value);
                if (emp?.Email != null) emails.Add(emp.Email);
            }
            if (cita.AnestesistaId.HasValue)
            {
                var emp = await _db.Empleados.FindAsync(cita.AnestesistaId.Value);
                if (emp?.Email != null) emails.Add(emp.Email);
            }
            if (cita.InstrumentistaId.HasValue)
            {
                var emp = await _db.Empleados.FindAsync(cita.InstrumentistaId.Value);
                if (emp?.Email != null) emails.Add(emp.Email);
            }
            if (cita.CirculanteId.HasValue)
            {
                var emp = await _db.Empleados.FindAsync(cita.CirculanteId.Value);
                if (emp?.Email != null) emails.Add(emp.Email);
            }
            return emails;
        }

        private async Task<Citas> ObtenerCitaAsociadaAsync(int hospitalizacionId)
        {
            var hospitalizacion = await _db.Hospitalizaciones
                .Include(h => h.Paciente)
                .FirstOrDefaultAsync(h => h.Id == hospitalizacionId);
            if (hospitalizacion == null) return null;

            var citasDelPaciente = await _db.Citass
                .Where(c => c.PacienteId == hospitalizacion.PacienteId && !c.Eliminado)
                .ToListAsync();

            if (!citasDelPaciente.Any()) return null;

            return citasDelPaciente
                .Select(c => new { Cita = c, Diferencia = c.FechaInicio.HasValue ? Math.Abs((c.FechaInicio.Value - hospitalizacion.FechaInicio).TotalMinutes) : double.MaxValue })
                .OrderBy(x => x.Diferencia)
                .FirstOrDefault()?.Cita;
        }

        private DateTime? ParseFechaHoraAplicacion(string rawDate)
        {
            if (string.IsNullOrWhiteSpace(rawDate)) return null;

            rawDate = rawDate.Trim()
                .Replace("p. m.", "PM", StringComparison.OrdinalIgnoreCase)
                .Replace("a. m.", "AM", StringComparison.OrdinalIgnoreCase)
                .Replace("p.m.", "PM", StringComparison.OrdinalIgnoreCase)
                .Replace("a.m.", "AM", StringComparison.OrdinalIgnoreCase);

            if (rawDate.Contains("T"))
            {
                string[] isoFormats = { "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm" };
                if (DateTime.TryParseExact(rawDate, isoFormats, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out var utcDirect))
                {
                    return utcDirect;
                }
            }

            string[] localFormats = { "dd/MM/yyyy HH:mm", "dd/MM/yyyy H:mm", "dd/MM/yyyy hh:mm tt", "dd/MM/yyyy h:mm tt" };
            var cultures = new[] { System.Globalization.CultureInfo.InvariantCulture, new System.Globalization.CultureInfo("es-GT") };
            foreach (var culture in cultures)
            {
                if (DateTime.TryParseExact(rawDate, localFormats, culture, System.Globalization.DateTimeStyles.None, out var parsedLocalFull))
                {
                    return TimeZoneInfo.ConvertTimeToUtc(parsedLocalFull, HospitalTimeZone);
                }
            }

            string[] timeOnlyFormats = { "HH:mm", "H:mm", "hh:mm tt", "h:mm tt", "HH:mm:ss" };
            foreach (var culture in cultures)
            {
                if (DateTime.TryParseExact(rawDate, timeOnlyFormats, culture, System.Globalization.DateTimeStyles.None, out var parsedTime))
                {
                    var todayGuatemala = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, HospitalTimeZone).Date;
                    var localDateTime = todayGuatemala.Add(parsedTime.TimeOfDay);
                    return TimeZoneInfo.ConvertTimeToUtc(localDateTime, HospitalTimeZone);
                }
            }
            return null;
        }

        private string ObtenerCuerpoCorreo(SolicitudMedicamento solicitud, string pacienteNombre, int? dosisActual, int? cantidadTotal)
        {
            string dosisInfo = "";
            if (dosisActual.HasValue && cantidadTotal.HasValue)
                dosisInfo = $"<p><strong>Dosis:</strong> {dosisActual} de {cantidadTotal}</p>";

            return $@"
<div style='font-family: Arial, sans-serif;'>
    <h3 style='color: #2c3e50;'>Orden de medicamento aprobada</h3>
    <p><strong>Paciente:</strong> {pacienteNombre}</p>
    {dosisInfo}
    <p><strong>Medicamento:</strong> {solicitud.NombreProducto}</p>
    <p><strong>Cantidad:</strong> {solicitud.Cantidad}</p>
    <p><strong>Vía administración:</strong> {solicitud.ViaAdministracion}</p>
    <p><strong>Frecuencia:</strong> {solicitud.FrecuenciaAdministracion}</p>
    <p><strong>Indicaciones:</strong> {solicitud.Indicaciones}</p>
    <hr />
    <small>Este mensaje es automático, por favor no responder.</small>
</div>";
        }



        private async Task<string> CargarYReemplazarPlantillaAsync(
            int hospitalizacionId,
            string pacienteNombre,
            string nombreProducto,
            int cantidad,
            string indicaciones,
            string viaAdministracion,
            string frecuenciaAdministracion,
            int? dosisActual,
            int? dosisTotal,
            string fechaHoraAplicacionManual,
            string baseUrl)
        {
            var templatePath = Path.Combine(_env.WebRootPath, "js-utilidades-wp-email", "email-plantilla-alerta-medicamento.html");
            if (!File.Exists(templatePath))
            {
                _logger.LogError("Plantilla no encontrada en {Path}", templatePath);
                return "<p>Error: plantilla no encontrada.</p>";
            }

            var htmlContent = await File.ReadAllTextAsync(templatePath);

            // 1. Construir la fila de dosis (si aplica)
            string dosisInfoHtml = "";
            if (dosisActual.HasValue && dosisTotal.HasValue && dosisTotal.Value > 0)
            {
                dosisInfoHtml = @"
        <tr style='background:#f0f4ff;'>
          <td style='padding:10px; border:1px solid #ddd;'><strong>Dosis programada</strong></td>
          <td style='padding:10px; border:1px solid #ddd;'>" + dosisActual.Value + " de " + dosisTotal.Value + @"</td>
         </tr>";
            }

            // 2. Formatear la fecha/hora de aplicación (convertir a hora local de Guatemala)
            string fechaAplicacionFormateada = "Inmediata (STAT)";
            if (!string.IsNullOrEmpty(fechaHoraAplicacionManual))
            {
                // Intentar parsear la fecha (puede venir en UTC o en formato local)
                if (DateTime.TryParse(fechaHoraAplicacionManual, out var fechaUtc))
                {
                    // Convertir UTC a hora local de Guatemala
                    var guatemalaZone = TimeZoneInfo.FindSystemTimeZoneById("America/Guatemala");
                    var fechaLocal = TimeZoneInfo.ConvertTimeFromUtc(fechaUtc, guatemalaZone);
                    fechaAplicacionFormateada = fechaLocal.ToString("dd/MM/yyyy hh:mm tt");
                }
                else
                {
                    fechaAplicacionFormateada = fechaHoraAplicacionManual;
                }
            }

            string nombreEmpresa = _configuration["EstablecimientoNombreEmpresa"] ?? "Hospital";
            string direccion = _configuration["EstablecimientoDireccion"] ?? "";
            string telefono = _configuration["EstablecimientoTelefono"] ?? "";
            string correo = _configuration["EstablecimientoCorreoElectronico"] ?? "";


            string logoUrl = _configuration["ImagenLogoBase64"] ?? "";
            Console.WriteLine($"[Logo] Valor config: '{logoUrl}'");

            if (!string.IsNullOrEmpty(logoUrl) && !logoUrl.StartsWith("http") && !logoUrl.StartsWith("data:"))
            {
                try
                {
                    var imagePath = Path.Combine(_env.WebRootPath, logoUrl.TrimStart('/'));
                    Console.WriteLine($"[Logo] Buscando archivo en: '{imagePath}'");
                    Console.WriteLine($"[Logo] Archivo existe: {File.Exists(imagePath)}");

                    if (File.Exists(imagePath))
                    {
                        var imageBytes = await File.ReadAllBytesAsync(imagePath);
                        Console.WriteLine($"[Logo] Imagen leída: {imageBytes.Length} bytes");
                        var ext = Path.GetExtension(imagePath).TrimStart('.').ToLower();
                        var mimeType = ext switch
                        {
                            "png" => "image/png",
                            "jpg" => "image/jpeg",
                            "jpeg" => "image/jpeg",
                            _ => "image/png"
                        };
                        logoUrl = $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
                        Console.WriteLine($"[Logo] Base64 generado OK, longitud: {logoUrl.Length}");
                    }
                    else
                    {
                        Console.WriteLine("[Logo] ARCHIVO NO ENCONTRADO, logoUrl quedará vacío");
                        logoUrl = "";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Logo] EXCEPCIÓN: {ex.Message}");
                    logoUrl = "";
                }
            }

            string logoHtml = !string.IsNullOrEmpty(logoUrl)
                ? $"<img src=\"{logoUrl}\" alt=\"{nombreEmpresa}\" style=\"height:60px;\">"
                : "";

            Console.WriteLine($"[Logo] logoHtml generado: {(string.IsNullOrEmpty(logoHtml) ? "VACÍO" : $"{logoHtml.Length} chars")}");

            // 4. Reemplazar placeholders
            var replacements = new Dictionary<string, string>
    {
        { "{{LogoHtml}}", logoHtml },
        { "{{NombreEmpresa}}", nombreEmpresa },
        { "{{Direccion}}", direccion },
        { "{{Telefono}}", telefono },
        { "{{Correo}}", correo },
        { "{{Year}}", DateTime.Now.Year.ToString() },
        { "{{PacienteNombre}}", pacienteNombre },
        { "{{NombreProducto}}", nombreProducto },
        { "{{Cantidad}}", cantidad.ToString() },
        { "{{ViaAdministracion}}", viaAdministracion ?? "No especificada" },
        { "{{FrecuenciaAdministracion}}", frecuenciaAdministracion ?? "STAT" },
        { "{{Indicaciones}}", indicaciones ?? "Ninguna" },
        { "{{DosisInfo}}", dosisInfoHtml },
        { "{{FechaAplicacion}}", fechaAplicacionFormateada }
    };

            foreach (var kvp in replacements)
            {
                htmlContent = htmlContent.Replace(kvp.Key, kvp.Value);
            }

            return htmlContent;
        }

    }
}