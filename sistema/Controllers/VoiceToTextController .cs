using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class VoiceToTextController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public VoiceToTextController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] IFormFile audioFile)
    {
        // ✅ URL ahora viene de appsettings:  "VoiceToText": { "ApiUrl": "https://..." }
        var apiUrl = _configuration["VoiceToText:ApiUrl"];
        var token = _configuration["MultiAPI_Token"];

        if (string.IsNullOrWhiteSpace(apiUrl))
            return StatusCode(StatusCodes.Status500InternalServerError, "No está configurado VoiceToText:ApiUrl en appsettings.");

        if (string.IsNullOrWhiteSpace(token))
            return StatusCode(StatusCodes.Status500InternalServerError, "No está configurado MultiAPI_Token en appsettings.");

        if (audioFile == null || audioFile.Length == 0)
            return BadRequest("No se ha enviado ningún archivo.");

        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(180); // ajusta 20–60 según tu caso

        using var formData = new MultipartFormDataContent();
        using var fileStream = audioFile.OpenReadStream();
        using var fileContent = new StreamContent(fileStream);

        // (Opcional pero recomendable) forward del content-type si viene
        if (!string.IsNullOrWhiteSpace(audioFile.ContentType))
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(audioFile.ContentType);

        formData.Add(fileContent, "audioFile", audioFile.FileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        request.Content = formData;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response;
        try
        {
            response = await httpClient.SendAsync(request);
        }
        catch (TaskCanceledException)
        {
            // timeout o cancelación
            return StatusCode(StatusCodes.Status504GatewayTimeout, "Timeout llamando al servicio VoiceToText.");
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, $"Error conectando al servicio VoiceToText: {ex.Message}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return Ok(responseContent);

        // Propaga status y cuerpo del upstream
        return StatusCode((int)response.StatusCode, responseContent);
    }
}