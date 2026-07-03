using System.Threading;
using System.Threading.Tasks;

namespace Sistema.Services.WebAuthn
{
    public interface IWebAuthnService
    {
        /// <summary>
        /// Genera el challenge para el usuario logueado.
        /// Llamar antes de mostrar el diálogo de huella en el front.
        /// </summary>
        /// <param name="userId">Id del usuario autenticado (de UserManager.GetUserId).</param>
        /// <param name="actionLabel">
        ///   Descripción legible de lo que se va a guardar.
        /// </param>
        Task<WebAuthnBeginResult> BeginVerifyAsync(string userId, string actionLabel = null);

        /// <summary>
        /// Verifica la respuesta de huella del dispositivo.
        /// Retorna un resultado detallado con éxito, mensaje para el usuario y datos del contexto.
        /// </summary>
        /// <param name="userId">Id del usuario autenticado. Se valida contra la credencial.</param>
        /// <param name="payload">Respuesta del autenticador enviada desde el front.</param>
        Task<WebAuthnVerifyResult> CompleteVerifyAsync(
            string userId,
            WebAuthnAssertionPayload payload,
            CancellationToken cancellationToken = default);
    }
}