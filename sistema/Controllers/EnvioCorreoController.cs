using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Shared.IRepository;
using sistema.Models;
using Database.Shared.Paginacion;
using System.Linq;
using Database.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text.Json.Serialization;


using System.Net.Mail;
using System.Configuration;
// using System.Web.Configuration;
// using System.Net.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace sisrest.Controllers
{
    [Authorize]
    public class EnvioCorreoController : Controller
    {
        //private readonly ICajaClinica _cajaClinicaRepository = null;

        //public CajaClinicaController(ICajaClinica cajaClinicaRepository)
        //{

        //    _cajaClinicaRepository = cajaClinicaRepository;

        //}

        [HttpPost]
        public string EnviarCorreo()
        {

            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("xxxxxx@gmail.com", "Kyocode", System.Text.Encoding.UTF8);//Correo de salida
            correo.To.Add("xxxxxx@kyocode.com"); //Correo destino?
            correo.Subject = "Correo de prueba"; //Asunto
            correo.Body = "Este es un correo de prueba desde c#"; //Mensaje del correo
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient();
            smtp.UseDefaultCredentials = false;
            smtp.Host = "smtp.gmail.com"; //Host del servidor de correo
            smtp.Port = 25; //Puerto de salida
            smtp.Credentials = new System.Net.NetworkCredential("xxxxxx@gmail.com", "*******");//Cuenta de correo
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            smtp.EnableSsl = true;//True si el servidor de correo permite ssl
            smtp.Send(correo);

            return JsonSerializer.Serialize(new {Exitoso=true,Mensaje="Mensaje enviado"});
        }
    }
}