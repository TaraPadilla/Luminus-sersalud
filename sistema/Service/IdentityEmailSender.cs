using Microsoft.AspNetCore.Identity.UI.Services;
using sistema.UtilidadesEmailWp.Services.IService;
using System.Threading.Tasks;

namespace farmamest.Service
{
    /// <summary>Adapter so Identity UI pages can use the app's SMTP email service.</summary>
    public class IdentityEmailSender : IEmailSender
    {
        private readonly IEmailService _emailService;

        public IdentityEmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (!string.IsNullOrWhiteSpace(email))
                _emailService.SendEmail(subject, htmlMessage, email, null);

            return Task.CompletedTask;
        }
    }
}
