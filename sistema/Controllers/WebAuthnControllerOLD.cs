// using System;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using System.Collections.Generic;
// using Database.Shared;
// using Database.Shared.Models;
// using Fido2NetLib;
// using Fido2NetLib.Objects;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using System.Threading;
// using Microsoft.AspNetCore.Identity.UI;
// namespace sistema.Controllers
// {
//     public class CustomAuthenticatorAssertionRawResponse
//     {
//         [JsonPropertyName("id")]
//         public string Id { get; set; }

//         [JsonPropertyName("rawId")]
//         public string RawId { get; set; }

//         [JsonPropertyName("type")]
//         public string Type { get; set; }

//         [JsonPropertyName("response")]
//         public CustomAssertionResponseData Response { get; set; }

//         public class CustomAssertionResponseData
//         {
//             [JsonPropertyName("authenticatorData")]
//             public string AuthenticatorData { get; set; }

//             [JsonPropertyName("clientDataJSON")]
//             public string ClientDataJSON { get; set; }

//             [JsonPropertyName("signature")]
//             public string Signature { get; set; }

//             [JsonPropertyName("userHandle")]
//             public string UserHandle { get; set; }
//         }

//         // Convierte esta clase personalizada a la clase que espera Fido2
//         public AuthenticatorAssertionRawResponse ToFido2Format()
//         {
//             return new AuthenticatorAssertionRawResponse
//             {
//                 Id = SafeFromBase64String(Id),
//                 RawId = SafeFromBase64String(RawId),
//                 Type = PublicKeyCredentialType.PublicKey,
//                 Response = new AuthenticatorAssertionRawResponse.AssertionResponse
//                 {
//                     AuthenticatorData = SafeFromBase64String(Response.AuthenticatorData),
//                     ClientDataJson = SafeFromBase64String(Response.ClientDataJSON),
//                     Signature = SafeFromBase64String(Response.Signature),
//                     UserHandle = !string.IsNullOrEmpty(Response.UserHandle) ? SafeFromBase64String(Response.UserHandle) : null
//                 }
//             };
//         }

//         // Método auxiliar para manejar correctamente cadenas Base64
//         private byte[] SafeFromBase64String(string input)
//         {
//             if (string.IsNullOrEmpty(input))
//                 return null;

//             // Aseguramos que la entrada esté correctamente formateada para Base64
//             input = input.Replace('-', '+').Replace('_', '/');

//             // Añadimos padding si es necesario
//             switch (input.Length % 4)
//             {
//                 case 2: input += "=="; break;
//                 case 3: input += "="; break;
//             }

//             try
//             {
//                 return Convert.FromBase64String(input);
//             }
//             catch
//             {
//                 // Log del error
//                 Console.WriteLine($"Error al decodificar Base64: {input}");
//                 throw;
//             }
//         }
//     }

//     // Clase para deserializar correctamente la respuesta del cliente
//     public class CustomAuthenticatorAttestationRawResponse
//     {
//         [JsonPropertyName("id")]
//         public string Id { get; set; }

//         [JsonPropertyName("rawId")]
//         public string RawId { get; set; }

//         [JsonPropertyName("type")]
//         public string Type { get; set; }

//         [JsonPropertyName("response")]
//         public CustomResponseData Response { get; set; }

//         [JsonPropertyName("extensions")]
//         public object Extensions { get; set; }

//         public class CustomResponseData
//         {
//             [JsonPropertyName("clientDataJSON")]
//             public string ClientDataJSON { get; set; }

//             [JsonPropertyName("attestationObject")]
//             public string AttestationObject { get; set; }
//         }

//         // Convierte esta clase personalizada a la clase que espera Fido2
//         public AuthenticatorAttestationRawResponse ToFido2Format()
//         {
//             return new AuthenticatorAttestationRawResponse
//             {
//                 Id = Convert.FromBase64String(Id),
//                 RawId = Convert.FromBase64String(RawId),
//                 Type = PublicKeyCredentialType.PublicKey,
//                 Response = new AuthenticatorAttestationRawResponse.ResponseData
//                 {
//                     AttestationObject = Convert.FromBase64String(Response.AttestationObject),
//                     ClientDataJson = Convert.FromBase64String(Response.ClientDataJSON)
//                 },
//                 Extensions = null
//             };
//         }
//     }

//     [ApiController]
//     [Route("api/[controller]")]
//     public class WebAuthnController : ControllerBase
//     {
//         private readonly IFido2 _fido2;
//         private readonly Context dbContext;
//         private readonly UserManager<User> _userManager;
//         private readonly RoleManager<IdentityRole> _roleManager;
//         private readonly SignInManager<User> _signInManager;

//         public WebAuthnController(IFido2 fido2, Context context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
//         {
//             _fido2 = fido2;
//             dbContext = context;
//             _userManager = userManager;
//             _signInManager = signInManager;
//             _roleManager = roleManager;
//         }

//         [HttpPost("BeginRegister")]
//         public async Task<IActionResult> BeginRegister()
//         {
//             var user = await _userManager.GetUserAsync(User);
//             if (user == null) return Unauthorized();

//             // Primero obtén las credenciales existentes del usuario
//             var existingCredentials = dbContext.WebAuthnCredentials
//                 .Where(c => c.UserId == user.Id)
//                 .ToList();

//             // Luego transformarlas a descriptores fuera de la consulta LINQ
//             var existingCreds = existingCredentials.Select(c =>
//             {
//                 var descriptor = new PublicKeyCredentialDescriptor(Convert.FromBase64String(c.DescriptorId));
//                 descriptor.Transports = new[] { AuthenticatorTransport.Internal };
//                 return descriptor;
//             }).ToList();

//             var fidoUser = new Fido2User
//             {
//                 DisplayName = user.UserName,
//                 Name = user.Email,
//                 Id = Encoding.UTF8.GetBytes(user.Id)
//             };

//             // Solicitar las credenciales
//             var options = _fido2.RequestNewCredential(fidoUser, existingCreds);

//             // Configurar las opciones de autenticador
//             options.AuthenticatorSelection = new AuthenticatorSelection
//             {
//                 AuthenticatorAttachment = AuthenticatorAttachment.Platform,
//                 RequireResidentKey = false,
//                 UserVerification = UserVerificationRequirement.Preferred,
//             };

//             // Solicitar atestación directa
//             options.Attestation = AttestationConveyancePreference.Direct;

//             // Guardar las opciones de registro en la sesión
//             HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

//             // Procesar excludeCredentials para el formato correcto hacia el cliente
//             var excludeCredentialsList = new List<object>();
//             if (options.ExcludeCredentials != null && options.ExcludeCredentials.Any())
//             {
//                 foreach (var c in options.ExcludeCredentials)
//                 {
//                     // Convertir transports a un array de strings
//                     string[] transportsArray = null;
//                     if (c.Transports != null && c.Transports.Any())
//                     {
//                         transportsArray = c.Transports.Select(t => t.ToString().ToLowerInvariant()).ToArray();
//                     }

//                     excludeCredentialsList.Add(new
//                     {
//                         id = Convert.ToBase64String(c.Id),
//                         type = "public-key",
//                         transports = transportsArray // Enviar como array de strings o null si no hay transportes
//                     });
//                 }
//             }

//             var webAuthnOptions = new
//             {
//                 rp = options.Rp,
//                 user = new
//                 {
//                     id = Convert.ToBase64String(options.User.Id),
//                     name = options.User.Name,
//                     displayName = options.User.DisplayName
//                 },
//                 challenge = Convert.ToBase64String(options.Challenge),
//                 pubKeyCredParams = options.PubKeyCredParams.Select(p => new
//                 {
//                     type = "public-key",
//                     alg = p.Alg
//                 }).ToList(),
//                 timeout = options.Timeout,
//                 excludeCredentials = excludeCredentialsList,
//                 authenticatorSelection = new
//                 {
//                     authenticatorAttachment = options.AuthenticatorSelection.AuthenticatorAttachment.ToString().ToLowerInvariant(),
//                     requireResidentKey = options.AuthenticatorSelection.RequireResidentKey,
//                     userVerification = options.AuthenticatorSelection.UserVerification.ToString().ToLowerInvariant()
//                 },
//                 attestation = options.Attestation.ToString().ToLowerInvariant(),
//                 extensions = options.Extensions,
//                 status = "ok",
//                 errorMessage = ""
//             };

//             return Ok(webAuthnOptions);
//         }



//         [HttpPost("CompleteRegister")]
//         public async Task<IActionResult> CompleteRegister([FromBody] CustomAuthenticatorAttestationRawResponse customResponse)
//         {
//             try
//             {
//                 Console.WriteLine("Received response: " + System.Text.Json.JsonSerializer.Serialize(customResponse));

//                 // Convertir la respuesta personalizada al formato esperado por Fido2
//                 var response = customResponse.ToFido2Format();

//                 var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");
//                 if (string.IsNullOrEmpty(jsonOptions))
//                     return BadRequest("No se encontró la sesión de registro.");

//                 var options = CredentialCreateOptions.FromJson(jsonOptions);

//                 // Validación básica de datos
//                 if (response.Id == null || response.RawId == null ||
//                     response.Response == null || response.Response.ClientDataJson == null ||
//                     response.Response.AttestationObject == null)
//                 {
//                     return BadRequest("La respuesta del autenticador no es válida");
//                 }

//                 // Pasar la lambda directamente para reducir cualquier problema de conversión de tipos
//                 var result = await _fido2.MakeNewCredentialAsync(
//                     response,
//                     options,
//                     (credId, user) => Task.FromResult(true));

//                 var user = await _userManager.GetUserAsync(User);
//                 if (user == null) return Unauthorized();

//                 bool credExists = dbContext.WebAuthnCredentials
//                     .Any(c => c.DescriptorId == Convert.ToBase64String(result.Result.CredentialId));

//                 if (credExists)
//                     return Conflict("Ya existe una credencial con este ID.");

//                 // Opcional: Podrías guardar también información sobre los transportes disponibles
//                 var cred = new WebAuthnCredential
//                 {
//                     UserId = user.Id,
//                     DescriptorId = Convert.ToBase64String(result.Result.CredentialId),
//                     PublicKey = Convert.ToBase64String(result.Result.PublicKey),
//                     SignatureCounter = result.Result.Counter,
//                     // Si quieres guardar los transportes, necesitarías añadir una columna a tu modelo WebAuthnCredential
//                     // Transports = string.Join(",", transportes detectados)
//                 };

//                 dbContext.WebAuthnCredentials.Add(cred);
//                 await dbContext.SaveChangesAsync();

//                 return Ok(new { status = "ok", message = "Registro exitoso" });
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest($"Error al completar el registro: {ex.Message}");
//             }
//         }


//         [HttpPost("BeginLogin")]
//         public async Task<IActionResult> BeginLogin()
//         {
//             try
//             {
//                 // En lugar de solicitar credenciales para un usuario específico,
//                 // obtenemos todas las credenciales registradas en el sistema
//                 var allCredentials = dbContext.WebAuthnCredentials.ToList();

//                 if (allCredentials.Count == 0)
//                 {
//                     return BadRequest(new { status = "failed", errorMessage = "No hay credenciales registradas en el sistema." });
//                 }

//                 var allowedCreds = allCredentials.Select(c =>
//                 {
//                     var descriptor = new PublicKeyCredentialDescriptor(Convert.FromBase64String(c.DescriptorId));
//                     descriptor.Transports = new[] { AuthenticatorTransport.Internal };
//                     return descriptor;
//                 }).ToList();

//                 var options = _fido2.GetAssertionOptions(
//                     allowedCreds,
//                     UserVerificationRequirement.Preferred
//                 );

//                 // Guardar en sesión
//                 HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());

//                 var loginOptions = new
//                 {
//                     challenge = Convert.ToBase64String(options.Challenge),
//                     timeout = options.Timeout,
//                     rpId = options.RpId,
//                     allowCredentials = options.AllowCredentials.Select(c => new
//                     {
//                         type = "public-key",
//                         id = Convert.ToBase64String(c.Id),
//                         transports = c.Transports?.Select(t => t.ToString().ToLowerInvariant()).ToArray()
//                     }),
//                     userVerification = options.UserVerification.ToString().ToLowerInvariant(),
//                     status = "ok",
//                     errorMessage = ""
//                 };

//                 return Ok(loginOptions);
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(new { status = "failed", errorMessage = ex.Message });
//             }
//         }

//         public class BeginLoginRequest
//         {
//             public string Email { get; set; }
//         }


//         [HttpPost("CompleteLogin")]
//         public async Task<IActionResult> CompleteLogin([FromBody] CustomAuthenticatorAssertionRawResponse customResponse, CancellationToken cancellationToken)
//         {
//             try
//             {
//                 // Recuperar las opciones de la sesión
//                 var jsonOptions = HttpContext.Session.GetString("fido2.assertionOptions");
//                 if (string.IsNullOrEmpty(jsonOptions))
//                     return BadRequest(new { status = "failed", message = "No se encontró la sesión de autenticación." });

//                 var options = AssertionOptions.FromJson(jsonOptions);

//                 // Convertir la respuesta personalizada al formato esperado por Fido2
//                 var response = customResponse.ToFido2Format();

//                 // Función para buscar las credenciales por ID
//                 async Task<WebAuthnCredential> FindCredentialById(byte[] credentialId)
//                 {
//                     var credIdString = Convert.ToBase64String(credentialId);
//                     var cred = dbContext.WebAuthnCredentials.FirstOrDefault(c => c.DescriptorId == credIdString);
//                     return await Task.FromResult(cred);
//                 }

//                 // Verificar la aserción
//                 var credential = await FindCredentialById(response.RawId);
//                 if (credential == null)
//                     return BadRequest(new { status = "failed", message = "Credencial no encontrada." });

//                 var user = await _userManager.FindByIdAsync(credential.UserId);
//                 if (user == null)
//                     return BadRequest(new { status = "failed", message = "Usuario no encontrado." });

//                 // Verificar la firma utilizando las funciones de verificación correctas
//                 var result = await _fido2.MakeAssertionAsync(
//                     response,
//                     options,
//                     Convert.FromBase64String(credential.PublicKey),
//                     credential.SignatureCounter,

//                     // Verificador de propiedad de credencial
//                     async (args, token) =>
//                     {
//                         var credIdString = Convert.ToBase64String(args.CredentialId);
//                         var cred = dbContext.WebAuthnCredentials.FirstOrDefault(c => c.DescriptorId == credIdString);

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

//                 // Verificar el resultado antes de iniciar sesión
//                 if (result.Status != "ok")
//                 {
//                     return BadRequest(new { status = "failed", message = $"Error de verificación: {result.ErrorMessage}" });
//                 }

//                 // ACTUALIZA el contador después de la verificación
//                 credential.SignatureCounter = result.Counter;
//                 dbContext.WebAuthnCredentials.Update(credential);
//                 await dbContext.SaveChangesAsync(cancellationToken);

//                 // Actualiza el sello de seguridad
//                 await _userManager.UpdateSecurityStampAsync(user);

//                 // Inicia sesión usando el esquema de Identity.Application en lugar de cookies directamente
//                 await _signInManager.SignInAsync(user, isPersistent: false);

//                 // Retorna el mensaje de éxito
//                 return Ok(new { status = "ok", message = "Inicio de sesión exitoso" });
//             }
//             catch (Exception ex)
//             {
//                 // Retorna error en caso de excepción
//                 return BadRequest(new { status = "failed", message = $"Error al completar la autenticación: {ex.Message}" });
//             }
//         }


//     }



//     public class WebAuthnVistaController : Controller
//     {
//         public IActionResult AgregarHuella()
//         {
//             return View();
//         }
//         public IActionResult IniciarSesionHuella()
//         {
//             return View();
//         }
//         public async Task<IActionResult> Logout()
//         {
//             await HttpContext.SignOutAsync(); // Cierra sesión
//             return RedirectToAction("Login", "Account"); // Redirige al login
//         }
//     }
// }