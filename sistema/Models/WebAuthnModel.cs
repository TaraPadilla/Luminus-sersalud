using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sistema.Services.WebAuthn
{
    // public class WebAuthnBeginResult
    // {
    //     public bool Success { get; init; }

    //     public string ErrorMessage { get; init; }

    //     public WebAuthnChallengeOptions Options { get; init; }

    //     public static WebAuthnBeginResult Ok(WebAuthnChallengeOptions options) =>
    //         new() { Success = true, Options = options };

    //     public static WebAuthnBeginResult Fail(string error) =>
    //         new() { Success = false, ErrorMessage = error };

    //     public WebAuthnChallengeOptions ChallengeOptions { get; set; }

    // }

    public class WebAuthnBeginResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public WebAuthnChallengeOptions ChallengeOptions { get; set; } // ← Esta propiedad es crucial

        public static WebAuthnBeginResult Ok(WebAuthnChallengeOptions options)
        {
            return new WebAuthnBeginResult { Success = true, ChallengeOptions = options };
        }

        public static WebAuthnBeginResult Fail(string error)
        {
            return new WebAuthnBeginResult { Success = false, ErrorMessage = error };
        }
    }

    public class WebAuthnChallengeOptions
    {
        [JsonPropertyName("challenge")]
        public string Challenge { get; init; }

        [JsonPropertyName("timeout")]
        public uint Timeout { get; init; }

        [JsonPropertyName("rpId")]
        public string RpId { get; init; }

        [JsonPropertyName("allowCredentials")]
        public List<AllowedCredentialDto> AllowCredentials { get; init; }

        [JsonPropertyName("userVerification")]
        public string UserVerification { get; init; }

        [JsonPropertyName("actionLabel")]
        public string ActionLabel { get; init; }

        [JsonPropertyName("status")]
        public string Status { get; init; } = "ok";

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; init; } = "";
    }

    public class AllowedCredentialDto
    {
        [JsonPropertyName("type")]
        public string Type { get; init; } = "public-key";

        [JsonPropertyName("id")]
        public string Id { get; init; }

        [JsonPropertyName("transports")]
        public string[] Transports { get; init; }
    }


    public class WebAuthnVerifyResult
    {
        public bool Success { get; init; }

        public string UserMessage { get; init; }

        public string TechnicalDetail { get; init; }

        public WebAuthnErrorCode? ErrorCode { get; init; }

        public string VerifiedUserId { get; init; }

        public string ActionLabel { get; init; }

        public static WebAuthnVerifyResult Ok(string userId, string actionLabel) =>
            new()
            {
                Success = true,
                VerifiedUserId = userId,
                ActionLabel = actionLabel,
                UserMessage = string.IsNullOrEmpty(actionLabel)
                    ? "Guardado confirmado con huella digital."
                    : $"'{actionLabel}' confirmado con huella digital."
            };

        public static WebAuthnVerifyResult Fail(
            string userMessage,
            string technicalDetail = null,
            WebAuthnErrorCode errorCode = WebAuthnErrorCode.VerificationFailed) =>
            new()
            {
                Success = false,
                UserMessage = userMessage,
                TechnicalDetail = technicalDetail,
                ErrorCode = errorCode
            };
    }


    public enum WebAuthnErrorCode
    {
        NoCredentialsRegistered,

        ChallengeExpired,

        CredentialMismatch,

        VerificationFailed,

        InternalError
    }

    public class WebAuthnAssertionPayload
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("rawId")]
        public string RawId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("response")]
        public AssertionResponseData Response { get; set; }

        public class AssertionResponseData
        {
            [JsonPropertyName("authenticatorData")]
            public string AuthenticatorData { get; set; }

            [JsonPropertyName("clientDataJSON")]
            public string ClientDataJSON { get; set; }

            [JsonPropertyName("signature")]
            public string Signature { get; set; }

            [JsonPropertyName("userHandle")]
            public string UserHandle { get; set; }
        }
    }
}