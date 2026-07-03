using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace farmamest.Dtos
{
    public class SendEmailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        // ESTO ES LO NUEVO: Ahora es una lista y se llama Attachments
        public List<IFormFile> Attachments { get; set; }
    }
}
