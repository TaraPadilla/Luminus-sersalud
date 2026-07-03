using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class GuardarPDFWhatsappController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    public GuardarPDFWhatsappController(IWebHostEnvironment env, IConfiguration configuration)
    {
        _env = env;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> GuardarPDFWhatsapp(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { success = false, message = "Archivo no válido." });
        }

        // Nombre de archivo seguro
        var fileName = Path.GetFileName(file.FileName);

        // Ruta física: wwwroot/pdf
        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var folderPath = Path.Combine(webRoot, "pdf");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Elegir la base URL según ambiente
        // appsettings.json (ejemplo):
        //
        // "WhatsApp": {
        //   "PublicBaseUrlDev": "http://localhost:5000/",
        //   "PublicBaseUrlProd": "https://tu-dominio.com/"
        // }
        string publicBaseUrl;

        if (_env.IsDevelopment())
        {
            publicBaseUrl = _configuration["WhatsAppSettings:PublicBaseUrlDev"];
        }
        else
        {
            publicBaseUrl = _configuration["WhatsAppSettings:PublicBaseUrlProd"];
        }

        // Fallback: si no está configurado, usar el host de la petición
        if (string.IsNullOrWhiteSpace(publicBaseUrl))
        {
            publicBaseUrl = $"{Request.Scheme}://{Request.Host}/";
        }

        if (!publicBaseUrl.EndsWith("/"))
        {
            publicBaseUrl += "/";
        }

        var fileUrl = $"{publicBaseUrl}pdf/{fileName}";

        return Ok(new { success = true, url = fileUrl });
    }
}
