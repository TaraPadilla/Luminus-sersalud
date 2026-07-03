using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using sistema.UtilidadesEmailWp.Services.IService;

namespace sistema.UtilidadesEmailWp.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly IConfiguration _configuration;

        public WhatsAppService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public async Task SendMessageAsync(string to, string body, Stream fileStream)
        //{
        //    var accountSid = _configuration["WhatsAppSettings:AccountSid"];
        //    var authToken = _configuration["WhatsAppSettings:AuthToken"];
        //    var fromNumber = _configuration["WhatsAppSettings:FromNumber"];

        //    TwilioClient.Init(accountSid, authToken);

        //    var mediaUrl = new Uri("https://yourserver.com/path/to/file"); // Cambia esto a la URL real del archivo

        //    var message = await MessageResource.CreateAsync(
        //        to: new PhoneNumber("whatsapp:" + to),
        //        from: new PhoneNumber(fromNumber),
        //        body: body,
        //        mediaUrl: new List<Uri> { mediaUrl }
        //    );
        //}
    }
}
