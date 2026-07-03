using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace farmamest.Helpers
{
    public static class HttpRequestJsonHelper
    {
        public static async Task<T> LeerCuerpoJsonAsync<T>(
            HttpRequest request,
            ControllerBase controller = null,
            System.Text.Json.JsonSerializerOptions jsonOptions = null) where T : class, new()
        {
            jsonOptions ??= new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (request.HasFormContentType && request.Form.Count > 0)
            {
                if (controller != null)
                {
                    var desdeFormulario = new T();
                    if (await controller.TryUpdateModelAsync(desdeFormulario))
                        return desdeFormulario;
                }
            }

            var body = await LeerCuerpoRawAsync(request);
            if (string.IsNullOrWhiteSpace(body))
                return null;

            var trimmed = body.TrimStart();
            if (!trimmed.StartsWith("{", StringComparison.Ordinal)
                && !trimmed.StartsWith("[", StringComparison.Ordinal))
            {
                return null;
            }

            try
            {
                var deserialized = JsonSerializer.Deserialize<T>(body, jsonOptions);
                if (deserialized != null)
                    return deserialized;
            }
            catch (System.Text.Json.JsonException)
            {
                // Intentar con Newtonsoft, más tolerante con este proyecto MVC.
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                return null;
            }
        }

        public static async Task<string> LeerCuerpoRawAsync(HttpRequest request)
        {
            if (request?.Body == null || !request.Body.CanRead)
                return null;

            request.EnableBuffering();
            request.Body.Position = 0;

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
    }
}
