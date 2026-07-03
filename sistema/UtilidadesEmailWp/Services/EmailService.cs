using farmamest.Service.IService;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.IO;
using System;
using sistema.Service.IService;
using farmamest.UtilidadesEmailWp;
using sistema.UtilidadesEmailWp.Services.IService;
using System.Collections.Generic;

namespace sistema.UtilidadesEmailWp.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public void SendEmail(string subject, string body, string to, List<(string FileName, byte[] Data)> attachments)
        {
            try
            {
                if (_emailSettings == null
                    || string.IsNullOrWhiteSpace(_emailSettings.Host)
                    || string.IsNullOrWhiteSpace(_emailSettings.Username)
                    || string.IsNullOrWhiteSpace(_emailSettings.Password))
                {
                    throw new InvalidOperationException(
                        "EmailSettings no está configurado en appsettings (Host, Username, Password).");
                }

                if (string.IsNullOrWhiteSpace(to))
                {
                    throw new ArgumentException("El destinatario del correo es obligatorio.");
                }

                var fromEmail = _emailSettings.Username.Trim();
                var password = (_emailSettings.Password ?? "").Replace(" ", "");

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(to));

                // Bucle para agregar múltiples archivos adjuntos
                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var att in attachments)
                    {
                        if (att.Data != null && att.Data.Length > 0)
                        {
                            var attachment = new Attachment(new MemoryStream(att.Data), att.FileName);
                            message.Attachments.Add(attachment);
                        }
                    }
                }

                using var smtpClient = new SmtpClient(_emailSettings.Host)
                {
                    Port = _emailSettings.Port,
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true
                };

                smtpClient.Send(message);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
                throw new Exception("Error de SMTP al enviar el correo", smtpEx);
            }
            catch (FormatException formatEx)
            {
                Console.WriteLine($"Formato Error: {formatEx.Message}");
                throw new Exception("Error de formato en la dirección de correo", formatEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("No se pudo enviar el email", ex);
            }
        }
    }
}