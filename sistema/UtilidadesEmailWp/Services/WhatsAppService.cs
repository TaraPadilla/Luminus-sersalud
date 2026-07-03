using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using farmamest.UtilidadesEmailWp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using sistema.UtilidadesEmailWp.Services.IService;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace sistema.UtilidadesEmailWp.Services
{
    /// <summary>Servicio unificado de WhatsApp vía Meta Cloud API y/o Twilio.</summary>
    public class WhatsAppService : IWhatsAppService
    {
        private readonly WhatsAppSettings _settings;
        private readonly ILogger<WhatsAppService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public WhatsAppService(
            IOptions<WhatsAppSettings> settings,
            ILogger<WhatsAppService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _settings = settings.Value;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> SendTextMessageAsync(string toPhoneNumber, string body)
        {
            if (!IsEnabled()) return false;
            if (string.IsNullOrWhiteSpace(toPhoneNumber) || string.IsNullOrWhiteSpace(body)) return false;

            if (_settings.HasTwilio)
                return await SendTwilioTextAsync(toPhoneNumber, body);

            if (_settings.HasMetaCloudApi)
                return await SendTemplateTextAsync(
                    toPhoneNumber,
                    _settings.TemplateRecordatorioCita,
                    new[] { body },
                    "es");

            _logger.LogWarning("WhatsApp habilitado pero no hay credenciales Twilio ni Meta configuradas.");
            return false;
        }

        public async Task<bool> SendTemplateDocumentAsync(
            string toPhoneNumber,
            string templateName,
            string documentUrl,
            string filename,
            string languageCode = "es")
        {
            if (!IsEnabled()) return false;
            if (string.IsNullOrWhiteSpace(toPhoneNumber) ||
                string.IsNullOrWhiteSpace(templateName) ||
                string.IsNullOrWhiteSpace(documentUrl))
                return false;

            if (_settings.HasMetaCloudApi)
            {
                return await SendMetaTemplateDocumentAsync(
                    toPhoneNumber, templateName, documentUrl, filename, languageCode);
            }

            if (_settings.HasTwilio)
            {
                return await SendTwilioMediaAsync(
                    toPhoneNumber,
                    $"Documento: {filename}",
                    documentUrl);
            }

            _logger.LogWarning("WhatsApp: no hay credenciales para enviar documentos.");
            return false;
        }

        public async Task<bool> SendTemplateTextAsync(
            string toPhoneNumber,
            string templateName,
            string[] bodyParameters,
            string languageCode = "es")
        {
            if (!IsEnabled()) return false;
            if (!_settings.HasMetaCloudApi) return false;
            if (string.IsNullOrWhiteSpace(toPhoneNumber) || string.IsNullOrWhiteSpace(templateName))
                return false;

            var to = NormalizarNumeroMeta(toPhoneNumber);
            var payload = new
            {
                messaging_product = "whatsapp",
                to,
                type = "template",
                template = new
                {
                    name = templateName,
                    language = new { code = languageCode },
                    components = bodyParameters != null && bodyParameters.Length > 0
                        ? new[]
                        {
                            new
                            {
                                type = "body",
                                parameters = Array.ConvertAll(bodyParameters, p => new { type = "text", text = p ?? "" })
                            }
                        }
                        : null
                }
            };

            return await PostMetaMessageAsync(payload, toPhoneNumber);
        }

        private bool IsEnabled()
        {
            if (_settings.Enabled) return true;
            _logger.LogInformation("WhatsApp deshabilitado en configuración (WhatsAppSettings:Enabled=false).");
            return false;
        }

        private async Task<bool> SendTwilioTextAsync(string toPhoneNumber, string body)
        {
            try
            {
                TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
                var to = NormalizarNumeroTwilio(toPhoneNumber);
                var from = NormalizarFromTwilio(_settings.FromNumber);

                var message = await MessageResource.CreateAsync(
                    to: new PhoneNumber(to),
                    from: new PhoneNumber(from),
                    body: body);

                _logger.LogInformation("WhatsApp Twilio enviado a {To}, SID={Sid}", to, message.Sid);
                return message.ErrorCode == null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando WhatsApp Twilio a {Phone}", toPhoneNumber);
                return false;
            }
        }

        private async Task<bool> SendTwilioMediaAsync(string toPhoneNumber, string body, string mediaUrl)
        {
            try
            {
                TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
                var to = NormalizarNumeroTwilio(toPhoneNumber);
                var from = NormalizarFromTwilio(_settings.FromNumber);

                var message = await MessageResource.CreateAsync(
                    to: new PhoneNumber(to),
                    from: new PhoneNumber(from),
                    body: body,
                    mediaUrl: new List<Uri> { new Uri(mediaUrl) });

                _logger.LogInformation("WhatsApp Twilio (media) enviado a {To}, SID={Sid}", to, message.Sid);
                return message.ErrorCode == null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando media WhatsApp Twilio a {Phone}", toPhoneNumber);
                return false;
            }
        }

        private async Task<bool> SendMetaTemplateDocumentAsync(
            string toPhoneNumber,
            string templateName,
            string documentUrl,
            string filename,
            string languageCode)
        {
            var to = NormalizarNumeroMeta(toPhoneNumber);
            var payload = new
            {
                messaging_product = "whatsapp",
                to,
                type = "template",
                template = new
                {
                    name = templateName,
                    language = new { code = languageCode },
                    components = new[]
                    {
                        new
                        {
                            type = "header",
                            parameters = new[]
                            {
                                new
                                {
                                    type = "document",
                                    document = new { link = documentUrl, filename }
                                }
                            }
                        }
                    }
                }
            };

            return await PostMetaMessageAsync(payload, toPhoneNumber);
        }

        private async Task<bool> PostMetaMessageAsync(object payload, string originalPhone)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var version = string.IsNullOrWhiteSpace(_settings.MetaApiVersion)
                    ? "v20.0"
                    : _settings.MetaApiVersion.Trim();
                var url =
                    $"https://graph.facebook.com/{version}/{_settings.MetaPhoneNumberId}/messages";

                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    _settings.ResolveMetaAccessToken());
                request.Content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Error Meta WhatsApp a {Phone}. Status={Status}. Body={Body}",
                        originalPhone,
                        (int)response.StatusCode,
                        responseBody);
                    return false;
                }

                _logger.LogInformation("WhatsApp Meta enviado a {Phone}. Respuesta={Body}", originalPhone, responseBody);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando WhatsApp Meta a {Phone}", originalPhone);
                return false;
            }
        }

        private static string NormalizarFromTwilio(string fromNumber)
        {
            return fromNumber.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase)
                ? fromNumber
                : "whatsapp:" + fromNumber;
        }

        private static string NormalizarNumeroTwilio(string phone)
        {
            phone = phone.Trim().Replace(" ", "").Replace("-", "");
            if (phone.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase))
                return phone;
            if (!phone.StartsWith("+"))
                phone = "+502" + phone.TrimStart('0');
            return "whatsapp:" + phone;
        }

        private string NormalizarNumeroMeta(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;

            phone = phone.Trim().Replace(" ", "").Replace("-", "").Replace(".", "").Replace("(", "").Replace(")", "");
            if (phone.StartsWith("+"))
                return phone.TrimStart('+');

            var digits = phone.TrimStart('0');
            if (digits == "3249480108")
                return "57" + digits;

            var countryCode = string.IsNullOrWhiteSpace(_settings.DefaultCountryCode)
                ? "502"
                : _settings.DefaultCountryCode.TrimStart('+');
            return countryCode + digits;
        }
    }
}
