
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using farmamest.Utilidades;

namespace farmamest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
                var env = services.GetRequiredService<IWebHostEnvironment>();
                var configuration = services.GetRequiredService<IConfiguration>();

                ServerConfigurationValidator.ValidateOrFail(env, configuration, logger);
            }

            DatabaseStartupBootstrap.RunIfAllowed(host);

            using (var scope = host.Services.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseConnectivity");
                DatabaseConnectivityProbe.ProbeAsync(configuration, logger).GetAwaiter().GetResult();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var contentRoot = context.HostingEnvironment.ContentRootPath;
                    var isPublishedDeploy = !File.Exists(Path.Combine(contentRoot, "farmamest.csproj"));

                    if (context.HostingEnvironment.IsProduction() && isPublishedDeploy)
                    {
                        // Published production servers: ONLY appsettings.json (+ env vars).
                        config.Sources.Clear();
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        config.AddEnvironmentVariables();
                        if (args != null && args.Length > 0)
                            config.AddCommandLine(args);
                    }
                    else
                    {
                        config.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
                        if (!context.HostingEnvironment.IsProduction())
                        {
                            config.AddJsonFile("appsettings.Development.local.json", optional: true, reloadOnChange: true);
                        }
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
