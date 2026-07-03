using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using Database.Shared;
using Database.Shared.Models;

using Fido2NetLib;
using Fido2NetLib.Objects;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace sistema.Controllers
{
    public class CustomAuthenticatorAssertionRawResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("rawId")]
        public string RawId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("response")]
        public CustomAssertionResponseData Response { get; set; }

        public class CustomAssertionResponseData
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

        public AuthenticatorAssertionRawResponse ToFido2Format()
        {
            if (Response == null)
                throw new ArgumentException("response es requerido.");

            return new AuthenticatorAssertionRawResponse
            {
                Id = SafeFromBase64String(Id),
                RawId = SafeFromBase64String(RawId),
                Type = PublicKeyCredentialType.PublicKey,
                Response = new AuthenticatorAssertionRawResponse.AssertionResponse
                {
                    AuthenticatorData = SafeFromBase64String(Response.AuthenticatorData),
                    ClientDataJson = SafeFromBase64String(Response.ClientDataJSON),
                    Signature = SafeFromBase64String(Response.Signature),
                    UserHandle = !string.IsNullOrEmpty(Response.UserHandle) ? SafeFromBase64String(Response.UserHandle) : null
                }
            };
        }

        private static byte[] SafeFromBase64String(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            input = input.Replace('-', '+').Replace('_', '/');

            switch (input.Length % 4)
            {
                case 2: input += "=="; break;
                case 3: input += "="; break;
            }

            return Convert.FromBase64String(input);
        }
    }

    public class CustomAuthenticatorAttestationRawResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("rawId")]
        public string RawId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("response")]
        public CustomResponseData Response { get; set; }

        [JsonPropertyName("extensions")]
        public object Extensions { get; set; }

        public class CustomResponseData
        {
            [JsonPropertyName("clientDataJSON")]
            public string ClientDataJSON { get; set; }

            [JsonPropertyName("attestationObject")]
            public string AttestationObject { get; set; }
        }

        public AuthenticatorAttestationRawResponse ToFido2Format()
        {
            if (Response == null)
                throw new ArgumentException("response es requerido.");

            // Mantengo tu comportamiento original (Convert.FromBase64String),
            // porque tu front está enviando base64 estándar con btoa().
            return new AuthenticatorAttestationRawResponse
            {
                Id = Convert.FromBase64String(Id),
                RawId = Convert.FromBase64String(RawId),
                Type = PublicKeyCredentialType.PublicKey,
                Response = new AuthenticatorAttestationRawResponse.ResponseData
                {
                    AttestationObject = Convert.FromBase64String(Response.AttestationObject),
                    ClientDataJson = Convert.FromBase64String(Response.ClientDataJSON)
                },
                Extensions = null
            };
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class WebAuthnController : ControllerBase
    {
        private readonly IFido2 _fido2;
        private readonly Context dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public WebAuthnController(
            IFido2 fido2,
            Context context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _fido2 = fido2;
            dbContext = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // [HttpPost("BeginRegister")]
        // public async Task<IActionResult> BeginRegister()
        // {
        //     var user = await _userManager.GetUserAsync(User);
        //     if (user == null) return Unauthorized();

        //     var existingCredentials = dbContext.WebAuthnCredentials
        //         .Where(c => c.UserId == user.Id)
        //         .ToList();

        //     // ✅ Ajuste: NO limitar transports a Internal.
        //     // Esto permite autenticadores híbridos/roaming (teléfono/llave).
        //     var existingCreds = existingCredentials.Select(c =>
        //     {
        //         var descriptor = new PublicKeyCredentialDescriptor(Convert.FromBase64String(c.DescriptorId));
        //         // descriptor.Transports = new[] { AuthenticatorTransport.Internal }; // ❌ quitado
        //         return descriptor;
        //     }).ToList();

        //     var fidoUser = new Fido2User
        //     {
        //         DisplayName = user.UserName,
        //         Name = user.Email,
        //         Id = Encoding.UTF8.GetBytes(user.Id)
        //     };

        //     var options = _fido2.RequestNewCredential(fidoUser, existingCreds);

        //     // ✅ Ajuste: NO forzar Platform. Dejar que Chrome use teléfono/otros.
        //     // Requerimos verificación de usuario para que sea “huella/PIN”.
        //     options.AuthenticatorSelection = new AuthenticatorSelection
        //     {
        //         // AuthenticatorAttachment = AuthenticatorAttachment.Platform, // ❌ no setear
        //         RequireResidentKey = false,
        //         UserVerification = UserVerificationRequirement.Required,
        //     };

        //     // ✅ Mejor compatibilidad (no obligar attestation direct)
        //     options.Attestation = AttestationConveyancePreference.None;

        //     HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

        //     // ✅ FIX: No enviar transports: null (Android/Chrome falla). Omitir propiedad.
        //     var excludeCredentialsList = new List<object>();
        //     if (options.ExcludeCredentials != null && options.ExcludeCredentials.Any())
        //     {
        //         foreach (var c in options.ExcludeCredentials)
        //         {
        //             string[] transportsArray = null;
        //             if (c.Transports != null && c.Transports.Any())
        //             {
        //                 transportsArray = c.Transports.Select(t => t.ToString().ToLowerInvariant()).ToArray();
        //             }

        //             if (transportsArray != null)
        //             {
        //                 excludeCredentialsList.Add(new
        //                 {
        //                     id = Convert.ToBase64String(c.Id),
        //                     type = "public-key",
        //                     transports = transportsArray
        //                 });
        //             }
        //             else
        //             {
        //                 excludeCredentialsList.Add(new
        //                 {
        //                     id = Convert.ToBase64String(c.Id),
        //                     type = "public-key"
        //                 });
        //             }
        //         }
        //     }

        //     // ✅ Ajuste: AuthenticatorAttachment puede ser null; no llamar ToString() directo.
        //     var authenticatorAttachment = options.AuthenticatorSelection?.AuthenticatorAttachment?.ToString()?.ToLowerInvariant();

        //     var webAuthnOptions = new
        //     {
        //         rp = options.Rp,
        //         user = new
        //         {
        //             id = Convert.ToBase64String(options.User.Id),
        //             name = options.User.Name,
        //             displayName = options.User.DisplayName
        //         },
        //         challenge = Convert.ToBase64String(options.Challenge),
        //         pubKeyCredParams = options.PubKeyCredParams.Select(p => new
        //         {
        //             type = "public-key",
        //             alg = p.Alg
        //         }).ToList(),
        //         timeout = options.Timeout,
        //         excludeCredentials = excludeCredentialsList,
        //         authenticatorSelection = new
        //         {
        //             authenticatorAttachment = authenticatorAttachment, // puede ser null (ok)
        //             requireResidentKey = options.AuthenticatorSelection.RequireResidentKey,
        //             userVerification = options.AuthenticatorSelection.UserVerification.ToString().ToLowerInvariant()
        //         },
        //         attestation = options.Attestation.ToString().ToLowerInvariant(),
        //         extensions = options.Extensions,
        //         status = "ok",
        //         errorMessage = ""
        //     };

        //     return Ok(webAuthnOptions);
        // }

        [HttpPost("BeginRegister")]
        public async Task<IActionResult> BeginRegister([FromQuery] string userId)
        {
            User targetUser;

            // 1. Verificamos si se envió un ID específico (desde la vista de modificar)
            if (!string.IsNullOrEmpty(userId))
            {
                targetUser = await _userManager.FindByIdAsync(userId);
                if (targetUser == null) return BadRequest("Usuario seleccionado no encontrado.");
            }
            else
            {
                // Respaldo: si no hay ID, usamos al usuario con la sesión activa
                targetUser = await _userManager.GetUserAsync(User);
                if (targetUser == null) return Unauthorized();
            }

            // 2. GUARDAMOS EL ID DEL USUARIO EN SESIÓN PARA EL SIGUIENTE PASO
            HttpContext.Session.SetString("fido2.registerUserId", targetUser.Id);

            // 3. Buscamos credenciales del usuario objetivo
            var existingCredentials = dbContext.WebAuthnCredentials
                .Where(c => c.UserId == targetUser.Id)
                .ToList();

            var existingCreds = existingCredentials.Select(c =>
            {
                var descriptor = new PublicKeyCredentialDescriptor(Convert.FromBase64String(c.DescriptorId));
                return descriptor;
            }).ToList();

            var fidoUser = new Fido2User
            {
                DisplayName = targetUser.UserName,
                Name = targetUser.Email,
                Id = Encoding.UTF8.GetBytes(targetUser.Id) // Usamos el ID del objetivo
            };

            var options = _fido2.RequestNewCredential(fidoUser, existingCreds);

            options.AuthenticatorSelection = new AuthenticatorSelection
            {
                RequireResidentKey = false,
                UserVerification = UserVerificationRequirement.Required,
            };

            options.Attestation = AttestationConveyancePreference.None;

            HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

            var excludeCredentialsList = new List<object>();
            if (options.ExcludeCredentials != null && options.ExcludeCredentials.Any())
            {
                foreach (var c in options.ExcludeCredentials)
                {
                    string[] transportsArray = null;
                    if (c.Transports != null && c.Transports.Any())
                    {
                        transportsArray = c.Transports.Select(t => t.ToString().ToLowerInvariant()).ToArray();
                    }

                    if (transportsArray != null)
                    {
                        excludeCredentialsList.Add(new
                        {
                            id = Convert.ToBase64String(c.Id),
                            type = "public-key",
                            transports = transportsArray
                        });
                    }
                    else
                    {
                        excludeCredentialsList.Add(new
                        {
                            id = Convert.ToBase64String(c.Id),
                            type = "public-key"
                        });
                    }
                }
            }

            var authenticatorAttachment = options.AuthenticatorSelection?.AuthenticatorAttachment?.ToString()?.ToLowerInvariant();

            var webAuthnOptions = new
            {
                rp = options.Rp,
                user = new
                {
                    id = Convert.ToBase64String(options.User.Id),
                    name = options.User.Name,
                    displayName = options.User.DisplayName
                },
                challenge = Convert.ToBase64String(options.Challenge),
                pubKeyCredParams = options.PubKeyCredParams.Select(p => new
                {
                    type = "public-key",
                    alg = p.Alg
                }).ToList(),
                timeout = options.Timeout,
                excludeCredentials = excludeCredentialsList,
                authenticatorSelection = new
                {
                    authenticatorAttachment = authenticatorAttachment,
                    requireResidentKey = options.AuthenticatorSelection.RequireResidentKey,
                    userVerification = options.AuthenticatorSelection.UserVerification.ToString().ToLowerInvariant()
                },
                attestation = options.Attestation.ToString().ToLowerInvariant(),
                extensions = options.Extensions,
                status = "ok",
                errorMessage = ""
            };

            return Ok(webAuthnOptions);
        }

        // [HttpPost("CompleteRegister")]
        // public async Task<IActionResult> CompleteRegister([FromBody] CustomAuthenticatorAttestationRawResponse customResponse)
        // {
        //     try
        //     {
        //         if (customResponse == null)
        //             return BadRequest("Body requerido.");

        //         if (customResponse.Response == null)
        //             return BadRequest("La respuesta del autenticador no es válida.");

        //         var currentUser = await _userManager.GetUserAsync(User);
        //         if (currentUser == null)
        //             return Unauthorized();

        //         var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");
        //         if (string.IsNullOrEmpty(jsonOptions))
        //             return BadRequest("No se encontró la sesión de registro.");

        //         var options = CredentialCreateOptions.FromJson(jsonOptions);

        //         var response = customResponse.ToFido2Format();

        //         if (response.Id == null || response.RawId == null ||
        //             response.Response == null || response.Response.ClientDataJson == null ||
        //             response.Response.AttestationObject == null)
        //         {
        //             return BadRequest("La respuesta del autenticador no es válida.");
        //         }

        //         var result = await _fido2.MakeNewCredentialAsync(
        //             response,
        //             options,
        //             (args, token) =>
        //             {
        //                 var credIdString = Convert.ToBase64String(args.CredentialId);
        //                 var exists = dbContext.WebAuthnCredentials.Any(c => c.DescriptorId == credIdString);
        //                 return Task.FromResult(!exists);
        //             });

        //         var newCredIdString = Convert.ToBase64String(result.Result.CredentialId);
        //         bool credExists = dbContext.WebAuthnCredentials.Any(c => c.DescriptorId == newCredIdString);
        //         if (credExists)
        //             return Conflict("Ya existe una credencial con este ID.");

        //         var cred = new WebAuthnCredential
        //         {
        //             UserId = currentUser.Id,
        //             DescriptorId = newCredIdString,
        //             PublicKey = Convert.ToBase64String(result.Result.PublicKey),
        //             SignatureCounter = result.Result.Counter
        //         };

        //         dbContext.WebAuthnCredentials.Add(cred);
        //         await dbContext.SaveChangesAsync();

        //         return Ok(new { status = "ok", message = "Registro exitoso" });
        //     }
        //     catch (Fido2VerificationException ex)
        //     {
        //         return BadRequest(new { status = "failed", message = ex.Message });
        //     }
        //     catch (Exception)
        //     {
        //         return BadRequest(new { status = "failed", message = "Error al completar el registro." });
        //     }
        // }

        [HttpPost("CompleteRegister")]
        public async Task<IActionResult> CompleteRegister([FromBody] CustomAuthenticatorAttestationRawResponse customResponse)
        {
            try
            {
                if (customResponse == null)
                    return BadRequest("Body requerido.");

                if (customResponse.Response == null)
                    return BadRequest("La respuesta del autenticador no es válida.");

                // 1. RECUPERAMOS EL ID DEL USUARIO OBJETIVO DE LA SESIÓN
                var targetUserId = HttpContext.Session.GetString("fido2.registerUserId");
                if (string.IsNullOrEmpty(targetUserId))
                    return BadRequest("No se encontró la sesión de registro del usuario.");

                // 2. Buscamos a ese usuario específico
                var targetUser = await _userManager.FindByIdAsync(targetUserId);
                if (targetUser == null)
                    return BadRequest("El usuario objetivo no existe o fue eliminado.");

                var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");
                if (string.IsNullOrEmpty(jsonOptions))
                    return BadRequest("No se encontró la sesión de opciones fido2.");

                var options = CredentialCreateOptions.FromJson(jsonOptions);

                var response = customResponse.ToFido2Format();

                if (response.Id == null || response.RawId == null ||
                    response.Response == null || response.Response.ClientDataJson == null ||
                    response.Response.AttestationObject == null)
                {
                    return BadRequest("La respuesta del autenticador no es válida.");
                }

                var result = await _fido2.MakeNewCredentialAsync(
                    response,
                    options,
                    (args, token) =>
                    {
                        var credIdString = Convert.ToBase64String(args.CredentialId);
                        var exists = dbContext.WebAuthnCredentials.Any(c => c.DescriptorId == credIdString);
                        return Task.FromResult(!exists);
                    });

                var newCredIdString = Convert.ToBase64String(result.Result.CredentialId);
                bool credExists = dbContext.WebAuthnCredentials.Any(c => c.DescriptorId == newCredIdString);
                if (credExists)
                    return Conflict("Ya existe una credencial con este ID.");

                // 3. ASIGNAMOS LA HUELLA AL USUARIO OBJETIVO
                var cred = new WebAuthnCredential
                {
                    UserId = targetUser.Id, // <-- Aquí guardamos correctamente para el usuario seleccionado
                    DescriptorId = newCredIdString,
                    PublicKey = Convert.ToBase64String(result.Result.PublicKey),
                    SignatureCounter = result.Result.Counter
                };

                dbContext.WebAuthnCredentials.Add(cred);
                await dbContext.SaveChangesAsync();

                // 4. Limpiamos la sesión por seguridad
                HttpContext.Session.Remove("fido2.registerUserId");
                HttpContext.Session.Remove("fido2.attestationOptions");

                return Ok(new { status = "ok", message = "Registro exitoso" });
            }
            catch (Fido2VerificationException ex)
            {
                return BadRequest(new { status = "failed", message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { status = "failed", message = "Error al completar el registro." });
            }
        }

        // ✅ BeginLogin sin body. Usa querystring.
        // Ejemplo: POST /api/WebAuthn/BeginLogin?email=correo@dominio.com
        [HttpPost("BeginLogin")]
        public async Task<IActionResult> BeginLogin([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new
                    {
                        status = "failed",
                        errorMessage = "Debe enviar el email del usuario para iniciar sesión con huella."
                    });
                }

                email = email.Trim();

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest(new { status = "failed", errorMessage = "Usuario no encontrado." });
                }

                var userCredentials = dbContext.WebAuthnCredentials
                    .Where(c => c.UserId == user.Id)
                    .ToList();

                if (userCredentials.Count == 0)
                {
                    return BadRequest(new
                    {
                        status = "failed",
                        errorMessage = "El usuario no tiene credenciales WebAuthn registradas."
                    });
                }

                // ✅ Ajuste: NO limitar transports a Internal.
                var allowedCreds = userCredentials.Select(c =>
                {
                    var descriptor = new PublicKeyCredentialDescriptor(Convert.FromBase64String(c.DescriptorId));
                    // descriptor.Transports = new[] { AuthenticatorTransport.Internal }; // ❌ quitado
                    return descriptor;
                }).ToList();

                // ✅ Recomendado: Required para asegurar “huella/PIN”
                var options = _fido2.GetAssertionOptions(
                    allowedCreds,
                    UserVerificationRequirement.Required
                );

                HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());

                // ✅ FIX: No enviar transports: null (Android/Chrome falla). Omitir propiedad.
                var allowCredentialsList = new List<object>();
                foreach (var c in options.AllowCredentials)
                {
                    string[] transportsArray = null;
                    if (c.Transports != null && c.Transports.Any())
                    {
                        transportsArray = c.Transports.Select(t => t.ToString().ToLowerInvariant()).ToArray();
                    }

                    if (transportsArray != null)
                    {
                        allowCredentialsList.Add(new
                        {
                            type = "public-key",
                            id = Convert.ToBase64String(c.Id),
                            transports = transportsArray
                        });
                    }
                    else
                    {
                        allowCredentialsList.Add(new
                        {
                            type = "public-key",
                            id = Convert.ToBase64String(c.Id)
                        });
                    }
                }

                var loginOptions = new
                {
                    challenge = Convert.ToBase64String(options.Challenge),
                    timeout = options.Timeout,
                    rpId = options.RpId,
                    allowCredentials = allowCredentialsList,
                    userVerification = options.UserVerification.ToString().ToLowerInvariant(),
                    status = "ok",
                    errorMessage = ""
                };

                return Ok(loginOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", errorMessage = ex.Message });
            }
        }

        [HttpPost("CompleteLogin")]
        public async Task<IActionResult> CompleteLogin(
            [FromBody] CustomAuthenticatorAssertionRawResponse customResponse,
            CancellationToken cancellationToken)
        {
            try
            {
                if (customResponse == null)
                    return BadRequest(new { status = "failed", message = "Body requerido." });

                var jsonOptions = HttpContext.Session.GetString("fido2.assertionOptions");
                if (string.IsNullOrEmpty(jsonOptions))
                    return BadRequest(new { status = "failed", message = "No se encontró la sesión de autenticación." });

                var options = AssertionOptions.FromJson(jsonOptions);

                var response = customResponse.ToFido2Format();

                // Buscar credencial por RawId
                var credIdString = Convert.ToBase64String(response.RawId);
                var credential = dbContext.WebAuthnCredentials.FirstOrDefault(c => c.DescriptorId == credIdString);
                if (credential == null)
                    return BadRequest(new { status = "failed", message = "Credencial no encontrada." });

                var user = await _userManager.FindByIdAsync(credential.UserId);
                if (user == null)
                    return BadRequest(new { status = "failed", message = "Usuario no encontrado." });

                var result = await _fido2.MakeAssertionAsync(
                    response,
                    options,
                    Convert.FromBase64String(credential.PublicKey),
                    credential.SignatureCounter,
                    async (args, token) =>
                    {
                        var argsCredIdString = Convert.ToBase64String(args.CredentialId);
                        var cred = dbContext.WebAuthnCredentials.FirstOrDefault(c => c.DescriptorId == argsCredIdString);

                        if (cred == null) return false;

                        if (args.UserHandle != null && args.UserHandle.Length > 0)
                        {
                            var userIdFromHandle = Encoding.UTF8.GetString(args.UserHandle);
                            return userIdFromHandle == cred.UserId;
                        }

                        return true;
                    },
                    cancellationToken: cancellationToken
                );

                if (result.Status != "ok")
                {
                    return BadRequest(new { status = "failed", message = $"Error de verificación: {result.ErrorMessage}" });
                }

                credential.SignatureCounter = result.Counter;
                dbContext.WebAuthnCredentials.Update(credential);
                await dbContext.SaveChangesAsync(cancellationToken);

                await _userManager.UpdateSecurityStampAsync(user);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return Ok(new { status = "ok", message = "Inicio de sesión exitoso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", message = $"Error al completar la autenticación: {ex.Message}" });
            }
        }

        [HttpPost("GenerateQrToken")]
        public async Task<IActionResult> GenerateQrToken([FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest(new { status = "failed", message = "El ID del usuario es requerido." });

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return BadRequest(new { status = "failed", message = "Usuario no encontrado." });

                // 1. Generamos un código único, seguro y limpio
                var token = Guid.NewGuid().ToString("N");

                // 2. Creamos el registro en la base de datos con expiración de 15 minutos
                var regToken = new WebAuthnRegistrationToken
                {
                    UserId = user.Id,
                    Token = token,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    IsUsed = false
                };

                dbContext.WebAuthnRegistrationTokens.Add(regToken);
                await dbContext.SaveChangesAsync();

                // 3. Armamos la URL que el empleado deberá visitar al escanear el QR
                // Esto toma el dominio actual de tu aplicación dinámicamente
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var registrationUrl = $"{baseUrl}/WebAuthnVista/RegistroQR?token={token}";

                return Ok(new { status = "ok", url = registrationUrl, token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "failed", message = $"Error al generar el token: {ex.Message}" });
            }
        }

        [AllowAnonymous] // Importante: Permite al teléfono acceder sin estar logueado
        [HttpPost("BeginRegisterQR")]
        public async Task<IActionResult> BeginRegisterQR([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token)) return BadRequest("Token requerido.");

            // 1. Validar que el token exista, no esté usado y no haya expirado
            var regToken = dbContext.WebAuthnRegistrationTokens
                .FirstOrDefault(t => t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);

            if (regToken == null) return BadRequest("Token inválido o expirado.");

            // 2. Buscar al usuario dueño de este token
            var user = await _userManager.FindByIdAsync(regToken.UserId);
            if (user == null) return BadRequest("Usuario no encontrado.");

            // 3. Guardar en sesión los datos para cuando el teléfono envíe la huella
            HttpContext.Session.SetString("fido2.registerUserId", user.Id);
            HttpContext.Session.SetString("fido2.qrToken", token); // Guardamos el token para "quemarlo" después

            // 4. Lógica estándar de FIDO2
            var existingCredentials = dbContext.WebAuthnCredentials
                .Where(c => c.UserId == user.Id)
                .Select(c => new PublicKeyCredentialDescriptor(Convert.FromBase64String(c.DescriptorId)))
                .ToList();

            var fidoUser = new Fido2User
            {
                DisplayName = user.UserName,
                Name = user.Email,
                Id = Encoding.UTF8.GetBytes(user.Id)
            };

            var options = _fido2.RequestNewCredential(fidoUser, existingCredentials);

            options.AuthenticatorSelection = new AuthenticatorSelection
            {
                RequireResidentKey = false,
                UserVerification = UserVerificationRequirement.Required,
            };
            options.Attestation = AttestationConveyancePreference.None;

            HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

            // 5. Formatear para el frontend
            var excludeCredentialsList = new List<object>();
            if (options.ExcludeCredentials != null)
            {
                foreach (var c in options.ExcludeCredentials)
                {
                    excludeCredentialsList.Add(new
                    {
                        id = Convert.ToBase64String(c.Id),
                        type = "public-key",
                        transports = c.Transports?.Select(t => t.ToString().ToLowerInvariant()).ToArray()
                    });
                }
            }

            var webAuthnOptions = new
            {
                rp = options.Rp,
                user = new
                {
                    id = Convert.ToBase64String(options.User.Id),
                    name = options.User.Name,
                    displayName = options.User.DisplayName
                },
                challenge = Convert.ToBase64String(options.Challenge),
                pubKeyCredParams = options.PubKeyCredParams.Select(p => new { type = "public-key", alg = p.Alg }).ToList(),
                timeout = options.Timeout,
                excludeCredentials = excludeCredentialsList,
                authenticatorSelection = new
                {
                    authenticatorAttachment = options.AuthenticatorSelection?.AuthenticatorAttachment?.ToString()?.ToLowerInvariant(),
                    requireResidentKey = options.AuthenticatorSelection.RequireResidentKey,
                    userVerification = options.AuthenticatorSelection.UserVerification.ToString().ToLowerInvariant()
                },
                attestation = options.Attestation.ToString().ToLowerInvariant()
            };

            return Ok(webAuthnOptions);
        }

        [AllowAnonymous]
        [HttpPost("CompleteRegisterQR")]
        public async Task<IActionResult> CompleteRegisterQR([FromBody] CustomAuthenticatorAttestationRawResponse customResponse)
        {
            try
            {
                // 1. Recuperar los datos de la sesión del teléfono
                var targetUserId = HttpContext.Session.GetString("fido2.registerUserId");
                var qrToken = HttpContext.Session.GetString("fido2.qrToken");
                var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");

                if (string.IsNullOrEmpty(targetUserId) || string.IsNullOrEmpty(jsonOptions) || string.IsNullOrEmpty(qrToken))
                    return BadRequest("La sesión expiró. Por favor, escanea el QR nuevamente.");

                var options = CredentialCreateOptions.FromJson(jsonOptions);
                var response = customResponse.ToFido2Format();

                // 2. Crear y validar la credencial
                var result = await _fido2.MakeNewCredentialAsync(
                    response,
                    options,
                    (args, t) =>
                    {
                        var credIdString = Convert.ToBase64String(args.CredentialId);
                        var exists = dbContext.WebAuthnCredentials.Any(c => c.DescriptorId == credIdString);
                        return Task.FromResult(!exists);
                    });

                var newCredIdString = Convert.ToBase64String(result.Result.CredentialId);

                // 3. Guardar la huella en la Base de Datos para el usuario
                var cred = new WebAuthnCredential
                {
                    UserId = targetUserId,
                    DescriptorId = newCredIdString,
                    PublicKey = Convert.ToBase64String(result.Result.PublicKey),
                    SignatureCounter = result.Result.Counter
                };
                dbContext.WebAuthnCredentials.Add(cred);

                // 4. IMPORTANTE: Invalidar el token para que no se pueda usar 2 veces
                var regToken = dbContext.WebAuthnRegistrationTokens.FirstOrDefault(t => t.Token == qrToken);
                if (regToken != null)
                {
                    regToken.IsUsed = true;
                    dbContext.WebAuthnRegistrationTokens.Update(regToken);
                }

                await dbContext.SaveChangesAsync();

                // 5. Limpiar la sesión por seguridad
                HttpContext.Session.Remove("fido2.registerUserId");
                HttpContext.Session.Remove("fido2.qrToken");
                HttpContext.Session.Remove("fido2.attestationOptions");

                return Ok(new { status = "ok", message = "Registro exitoso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al completar el registro: {ex.Message}");
            }
        }


        
    }

    public class WebAuthnVistaController : Controller
    {
        public IActionResult AgregarHuella()
        {
            return View();
        }

        public IActionResult IniciarSesionHuella()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous] // Permite que el empleado entre sin iniciar sesión
        public IActionResult RegistroQR([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Content("Token no válido o no proporcionado.");
            }

            // Pasamos el token a la vista
            ViewBag.Token = token;
            return View();
        }
    }
}