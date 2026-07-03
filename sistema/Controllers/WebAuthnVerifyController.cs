using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Database.Shared.Models;
using Sistema.Services.WebAuthn;
using System.Linq;
using Database.Shared.IRepository;

namespace Sistema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WebAuthnVerifyController : ControllerBase
    {
        private readonly IWebAuthnService _webAuthn;
        private readonly UserManager<User> _userManager;
        private readonly IWebAuthnStore _webAuthnStore;
        private readonly IEmpleado _empleadoRepository; // Si necesitas nombres de empleados

        public WebAuthnVerifyController(
            IWebAuthnService webAuthn,
            UserManager<User> userManager,
                        IWebAuthnStore webAuthnStore,
                        IEmpleado empleadoRepository)
        {
            _webAuthn = webAuthn;
            _userManager = userManager;
            _webAuthnStore = webAuthnStore;
            _empleadoRepository = empleadoRepository;


        }

        /// <summary>
        /// Paso 1: genera el challenge SIN restringir allowCredentials.
        /// El dispositivo presentará la credencial que tenga — el backend
        /// identifica al autorizador por su credentialId en CompleteVerify.
        /// </summary>
        // ─────────────────────────────────────────────────────────
        // 1. Endpoint existente (lo modificamos para aceptar targetUserId)
        // ─────────────────────────────────────────────────────────
        [HttpPost("Begin")]
        public async Task<IActionResult> Begin(string targetUserId = null, string actionLabel = null)
        {
            string userId;
            var currentUserId = _userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(targetUserId))
            {
                var currentHasCred = await _webAuthnStore.GetCredentialsByUserIdAsync(currentUserId);
                if (currentHasCred != null && currentHasCred.Any())
                {
                    return BadRequest(new { message = "El usuario actual ya tiene huella registrada. Use su propia huella." });
                }
                userId = targetUserId;
            }
            else
            {
                userId = currentUserId;
            }

            var result = await _webAuthn.BeginVerifyAsync(userId, actionLabel);

            // Validación robusta
            if (result == null)
                return BadRequest(new { message = "No se pudo iniciar la verificación (resultado nulo)." });

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage ?? "Error desconocido al iniciar verificación." });

            if (result.ChallengeOptions == null)
                return BadRequest(new { message = "No se generaron opciones de verificación. ¿El usuario tiene huellas registradas?" });

            return Ok(result.ChallengeOptions);
        }

        // ─────────────────────────────────────────────────────────
        // 2. Nuevo endpoint: verifica si el usuario actual tiene huella
        // ─────────────────────────────────────────────────────────
        [HttpGet("HasCredential")]
        public async Task<IActionResult> HasCredential()
        {
            var userId = _userManager.GetUserId(User);
            var credentials = await _webAuthnStore.GetCredentialsByUserIdAsync(userId);
            var hasCredential = credentials != null && credentials.Any();
            return Ok(new { hasCredential = hasCredential });
        }

        // ─────────────────────────────────────────────────────────
        // 3. Nuevo endpoint: lista usuarios (con huella) que pueden autorizar
        // ─────────────────────────────────────────────────────────
        [HttpGet("ListUsersWithCredentials")]
        public async Task<IActionResult> ListUsersWithCredentials()
        {
            // Roles autorizados para firmar (puedes ajustar según tu negocio)
            var allowedRoles = new[] { "Administrador", "Medico General", "Medico Interno", "Medico Externo", "Enfermeria" };

            var usersInRoles = new System.Collections.Generic.List<User>();
            foreach (var role in allowedRoles)
            {
                var users = await _userManager.GetUsersInRoleAsync(role);
                usersInRoles.AddRange(users);
            }

            usersInRoles = usersInRoles.DistinctBy(u => u.Id).ToList();

            var result = new System.Collections.Generic.List<object>();
            foreach (var user in usersInRoles)
            {
                var creds = await _webAuthnStore.GetCredentialsByUserIdAsync(user.Id);
                if (creds != null && creds.Any())
                {
                    string fullName = user.UserName;
                    if (user.EmpleadoId.HasValue)
                    {
                        var empleado = _empleadoRepository.Get(user.EmpleadoId.Value);
                        if (empleado != null)
                            fullName = empleado.NombreYApellidos;
                    }
                    result.Add(new
                    {
                        userId = user.Id,
                        fullName = fullName,
                        empleadoId = user.EmpleadoId
                    });
                }
            }

            return Ok(result);
        }

        /// <summary>
        /// Paso 2: verifica la firma. El autorizador se identifica por su credentialId.
        /// Solo se usa desde módulos que manejan su propia validación de permisos
        /// </summary>
        [HttpPost("Complete")]
        public async Task<IActionResult> Complete(
            [FromBody] WebAuthnAssertionPayload payload,
            CancellationToken cancellationToken)
        {
            if (payload == null)
                return BadRequest(new { status = "failed", message = "Payload requerido." });

            var userId = _userManager.GetUserId(User);
            var result = await _webAuthn.CompleteVerifyAsync(userId, payload, cancellationToken);

            if (!result.Success)
                return BadRequest(new
                {
                    status = "failed",
                    message = result.UserMessage,
                    errorCode = result.ErrorCode?.ToString()
                });

            return Ok(new
            {
                status = "ok",
                message = result.UserMessage,
                actionLabel = result.ActionLabel,
                verifiedUserId = result.VerifiedUserId
            });
        }
    }
}