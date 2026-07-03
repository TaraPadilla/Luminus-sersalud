using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace farmamest.Utilidades
{
    public class DatabaseExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseExceptionMiddleware> _logger;

        public DatabaseExceptionMiddleware(RequestDelegate next, ILogger<DatabaseExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IWebHostEnvironment env, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (DatabaseExceptionHelper.IsConnectivityError(ex))
            {
                _logger.LogError(ex, "Error de conectividad PostgreSQL en {Path}", context.Request.Path);

                if (context.Response.HasStarted)
                    throw;

                var message = DatabaseExceptionHelper.GetFriendlyMessage(
                    ex,
                    env.IsDevelopment(),
                    ConnectionStringResolver.Resolve(configuration));

                if (DatabaseExceptionHelper.WantsJson(context.Request))
                {
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    context.Response.ContentType = "application/json; charset=utf-8";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        exitoso = false,
                        Exitoso = false,
                        mensaje = message,
                        Mensaje = message
                    }));
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                if (context.Session != null)
                {
                    await context.Session.LoadAsync();
                    context.Session.SetString("DatabaseErrorMessage", message);
                }
                context.Response.Redirect("/Home/Error?db=1");
            }
        }
    }
}
