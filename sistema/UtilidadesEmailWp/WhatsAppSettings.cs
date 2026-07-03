using System;

namespace farmamest.UtilidadesEmailWp
{
    /// <summary>Configuración de WhatsApp (Twilio y/o Meta Cloud API).</summary>
    public class WhatsAppSettings
    {
        public bool Enabled { get; set; }

        /// <summary>Compatibilidad con endpoints legacy. Usar MetaAccessToken.</summary>
        public string Token { get; set; }

        public string DefaultCountryCode { get; set; } = "502";

        public string PublicBaseUrlDev { get; set; } = "http://localhost:5000/";
        public string PublicBaseUrlProd { get; set; }

        #region Twilio
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        /// <summary>Número remitente en formato whatsapp:+14155238886</summary>
        public string FromNumber { get; set; }
        #endregion

        #region Meta Cloud API
        public string MetaAccessToken { get; set; }
        public string MetaPhoneNumberId { get; set; }
        public string MetaApiVersion { get; set; } = "v20.0";
        public string TemplateLabResults { get; set; } = "manantiales_resultadoslab";
        public string TemplateCompras { get; set; } = "manantiales_compras";
        public string TemplateRecordatorioCita { get; set; } = "recordatorio_cita";
        #endregion

        public string ResolveMetaAccessToken()
        {
            return !string.IsNullOrWhiteSpace(MetaAccessToken) ? MetaAccessToken : Token;
        }

        public bool HasMetaCloudApi =>
            IsConfigured(ResolveMetaAccessToken()) &&
            IsConfigured(MetaPhoneNumberId);

        public bool HasTwilio =>
            IsConfigured(AccountSid) &&
            IsConfigured(AuthToken) &&
            IsConfigured(FromNumber);

        private static bool IsConfigured(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (value.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }
    }
}
