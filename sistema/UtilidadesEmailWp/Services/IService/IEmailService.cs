using System.Collections.Generic;

namespace sistema.UtilidadesEmailWp.Services.IService
{
    public interface IEmailService
    {
        // Cambiamos los parámetros individuales por una lista de Tuplas que contenga nombre y datos
        void SendEmail(string subject, string body, string to, List<(string FileName, byte[] Data)> attachments);
    }
}