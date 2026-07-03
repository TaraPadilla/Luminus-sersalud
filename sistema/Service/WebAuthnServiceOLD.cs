// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;

// using Fido2NetLib;
// using Fido2NetLib.Objects;

// using Microsoft.Extensions.Caching.Distributed;
// using Microsoft.Extensions.Logging;

// namespace Sistema.Services.WebAuthn
// {

//     public class WebAuthnService : IWebAuthnService
//     {
//         private readonly IFido2 _fido2;
//         private readonly IWebAuthnStore _store;
//         private readonly IDistributedCache _cache;
//         private readonly ILogger<WebAuthnService> _logger;

//         private static readonly TimeSpan ChallengeTtl = TimeSpan.FromMinutes(3);

//         public WebAuthnService(
//             IFido2 fido2,
//             IWebAuthnStore store,
//             IDistributedCache cache,
//             ILogger<WebAuthnService> logger)
//         {
//             _fido2 = fido2;
//             _store = store;
//             _cache = cache;
//             _logger = logger;
//         }

//         // ─────────────────────────────────────────────────────────────
//         // PASO 1: Generar el challenge
//         // ─────────────────────────────────────────────────────────────
//         public async Task<WebAuthnBeginResult> BeginVerifyAsync(string userId, string actionLabel = null)
//         {
//             try
//             {
//                 var credentials = await _store.GetCredentialsByUserIdAsync(userId);

//                 if (credentials == null || credentials.Count == 0)
//                 {
//                     _logger.LogWarning("BeginVerify: usuario {UserId} no tiene credenciales WebAuthn.", userId);
//                     return WebAuthnBeginResult.Fail("El usuario no tiene huella registrada.");
//                 }

//                 var allowedCreds = credentials
//                     .Select(c => new PublicKeyCredentialDescriptor(Convert.FromBase64String(c.DescriptorId)))
//                     .ToList();

//                 var options = _fido2.GetAssertionOptions(
//                     allowedCreds,
//                     UserVerificationRequirement.Required
//                 );

//                 var cachePayload = $"{options.ToJson()}|LABEL|{actionLabel ?? ""}";
//                 await _cache.SetStringAsync(
//                     CacheKey(userId),
//                     cachePayload,
//                     new DistributedCacheEntryOptions
//                     {
//                         AbsoluteExpirationRelativeToNow = ChallengeTtl
//                     });

//                 _logger.LogInformation(
//                     "BeginVerify: challenge generado para usuario {UserId}. Acción: '{ActionLabel}'.",
//                     userId, actionLabel ?? "(sin etiqueta)");

//                 var challengeOptions = new WebAuthnChallengeOptions
//                 {
//                     Challenge = Convert.ToBase64String(options.Challenge),
//                     Timeout = options.Timeout,
//                     RpId = options.RpId,
//                     AllowCredentials = BuildAllowCredentialsList(options.AllowCredentials),
//                     UserVerification = options.UserVerification.ToString().ToLowerInvariant(),
//                     ActionLabel = actionLabel
//                 };

//                 return WebAuthnBeginResult.Ok(challengeOptions);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "BeginVerify: error inesperado para usuario {UserId}.", userId);
//                 return WebAuthnBeginResult.Fail("No se pudo iniciar la verificación de huella.");
//             }
//         }

//         // ─────────────────────────────────────────────────────────────
//         // PASO 2: Verificar la huella y retornar resultado detallado
//         // ─────────────────────────────────────────────────────────────
//         public async Task<WebAuthnVerifyResult> CompleteVerifyAsync(
//             string userId,
//             WebAuthnAssertionPayload payload,
//             CancellationToken cancellationToken = default)
//         {
//             if (payload?.Response == null)
//             {
//                 return WebAuthnVerifyResult.Fail(
//                     "La respuesta del dispositivo no es válida.",
//                     "Payload o Response nulo.",
//                     WebAuthnErrorCode.VerificationFailed);
//             }

//             var cachedValue = await _cache.GetStringAsync(CacheKey(userId));
//             if (string.IsNullOrEmpty(cachedValue))
//             {
//                 _logger.LogWarning("CompleteVerify: challenge expirado o no encontrado para usuario {UserId}.", userId);
//                 return WebAuthnVerifyResult.Fail(
//                     "La sesión de verificación expiró. Presiona 'Guardar' nuevamente.",
//                     "Challenge no encontrado en cache.",
//                     WebAuthnErrorCode.ChallengeExpired);
//             }

//             await _cache.RemoveAsync(CacheKey(userId));

//             var (optionsJson, actionLabel) = ParseCachePayload(cachedValue);
//             var options = AssertionOptions.FromJson(optionsJson);

//             try
//             {
//                 // ── Buscar la credencial en BD ────────────────────────────
//                 var response = ConvertToFido2Format(payload);
//                 var credIdString = Convert.ToBase64String(response.RawId);
//                 var credential = await _store.FindByDescriptorIdAsync(credIdString);

//                 if (credential == null)
//                 {
//                     _logger.LogWarning(
//                         "CompleteVerify: credencial {CredId} no encontrada en BD. Usuario {UserId}.",
//                         credIdString, userId);
//                     return WebAuthnVerifyResult.Fail(
//                         "No se reconoció la huella registrada. Intenta con otro dedo o dispositivo.",
//                         $"DescriptorId '{credIdString}' no existe.",
//                         WebAuthnErrorCode.VerificationFailed);
//                 }

//                 // ── Validar que la credencial pertenece al usuario logueado ──
//                 if (credential.UserId != userId)
//                 {
//                     _logger.LogError(
//                         "CompleteVerify: ALERTA — credencial {CredId} pertenece a {OwnerUserId} pero fue usada por {UserId}.",
//                         credIdString, credential.UserId, userId);
//                     return WebAuthnVerifyResult.Fail(
//                         "La huella no corresponde a tu cuenta.",
//                         $"Credencial pertenece a usuario '{credential.UserId}', no a '{userId}'.",
//                         WebAuthnErrorCode.CredentialMismatch);
//                 }

//                 // ── Verificación criptográfica con Fido2NetLib ────────────
//                 var result = await _fido2.MakeAssertionAsync(
//                     response,
//                     options,
//                     Convert.FromBase64String(credential.PublicKey),
//                     credential.SignatureCounter,
//                     async (args, token) =>
//                     {
//                         // Verificar que el userHandle (si viene) coincide con el dueño
//                         var argCredId = Convert.ToBase64String(args.CredentialId);
//                         var cred = await _store.FindByDescriptorIdAsync(argCredId);
//                         if (cred == null) return false;

//                         if (args.UserHandle != null && args.UserHandle.Length > 0)
//                         {
//                             var userIdFromHandle = Encoding.UTF8.GetString(args.UserHandle);
//                             return userIdFromHandle == cred.UserId;
//                         }

//                         return true;
//                     },
//                     cancellationToken: cancellationToken
//                 );

//                 if (result.Status != "ok")
//                 {
//                     _logger.LogWarning(
//                         "CompleteVerify: fallo FIDO2 para usuario {UserId}: {FidoError}.",
//                         userId, result.ErrorMessage);
//                     return WebAuthnVerifyResult.Fail(
//                         "La verificación de huella no fue exitosa. Intenta de nuevo.",
//                         $"Fido2 error: {result.ErrorMessage}",
//                         WebAuthnErrorCode.VerificationFailed);
//                 }

//                 // ── Actualizar SignatureCounter (detecta autenticadores clonados) ──
//                 await _store.UpdateSignatureCounterAsync(credIdString, result.Counter);

//                 _logger.LogInformation(
//                     "CompleteVerify: guardado confirmado por huella. Usuario {UserId}. Acción: '{ActionLabel}'.",
//                     userId, actionLabel ?? "(sin etiqueta)");

//                 return WebAuthnVerifyResult.Ok(userId, actionLabel);
//             }
//             catch (Fido2VerificationException ex)
//             {
//                 _logger.LogWarning(ex, "CompleteVerify: excepción FIDO2 para usuario {UserId}.", userId);
//                 return WebAuthnVerifyResult.Fail(
//                     "La huella no pudo verificarse. Intenta de nuevo.",
//                     ex.Message,
//                     WebAuthnErrorCode.VerificationFailed);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "CompleteVerify: error interno para usuario {UserId}.", userId);
//                 return WebAuthnVerifyResult.Fail(
//                     "Ocurrió un error al verificar la huella. Contacta al soporte si persiste.",
//                     ex.Message,
//                     WebAuthnErrorCode.InternalError);
//             }
//         }

    
//         private static string CacheKey(string userId) =>
//             $"webauthn:save:{userId}";
//         private static List<AllowedCredentialDto> BuildAllowCredentialsList(
//             IEnumerable<PublicKeyCredentialDescriptor> descriptors)
//         {
//             var list = new List<AllowedCredentialDto>();
//             foreach (var c in descriptors)
//             {
//                 string[] transports = null;
//                 if (c.Transports != null && c.Transports.Any())
//                     transports = c.Transports.Select(t => t.ToString().ToLowerInvariant()).ToArray();

//                 list.Add(new AllowedCredentialDto
//                 {
//                     Id = Convert.ToBase64String(c.Id),
//                     Transports = transports
//                 });
//             }
//             return list;
//         }

//         private static AuthenticatorAssertionRawResponse ConvertToFido2Format(WebAuthnAssertionPayload p)
//         {
//             return new AuthenticatorAssertionRawResponse
//             {
//                 Id = FromBase64Url(p.Id),
//                 RawId = FromBase64Url(p.RawId),
//                 Type = PublicKeyCredentialType.PublicKey,
//                 Response = new AuthenticatorAssertionRawResponse.AssertionResponse
//                 {
//                     AuthenticatorData = FromBase64Url(p.Response.AuthenticatorData),
//                     ClientDataJson = FromBase64Url(p.Response.ClientDataJSON),
//                     Signature = FromBase64Url(p.Response.Signature),
//                     UserHandle = !string.IsNullOrEmpty(p.Response.UserHandle)
//                         ? FromBase64Url(p.Response.UserHandle)
//                         : null
//                 }
//             };
//         }

//         private static byte[] FromBase64Url(string input)
//         {
//             if (string.IsNullOrEmpty(input)) return null;
//             input = input.Replace('-', '+').Replace('_', '/');
//             switch (input.Length % 4)
//             {
//                 case 2: input += "=="; break;
//                 case 3: input += "="; break;
//             }
//             return Convert.FromBase64String(input);
//         }

//         private static (string optionsJson, string actionLabel) ParseCachePayload(string raw)
//         {
//             const string separator = "|LABEL|";
//             var idx = raw.IndexOf(separator, StringComparison.Ordinal);
//             if (idx < 0) return (raw, null);

//             var json = raw[..idx];
//             var label = raw[(idx + separator.Length)..];
//             return (json, string.IsNullOrEmpty(label) ? null : label);
//         }
//     }
// }