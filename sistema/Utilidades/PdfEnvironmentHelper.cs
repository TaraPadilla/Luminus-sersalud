using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace farmamest.Utilidades
{
    public static class PdfEnvironmentHelper
    {
        private const long MinValidBinaryBytes = 100_000;

        /// <summary>
        /// Path used by Wkhtmltopdf.NetCore on Windows (AppContext.BaseDirectory/Rotativa/Windows).
        /// </summary>
        public static string GetEngineWindowsPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Rotativa", "Windows", "wkhtmltopdf.exe");
        }

        public static string GetWebRootWindowsPath(string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(webRootPath))
                return null;

            return Path.Combine(webRootPath, "Rotativa", "Windows", "wkhtmltopdf.exe");
        }

        /// <summary>
        /// Path used by Wkhtmltopdf.NetCore on Linux (AppContext.BaseDirectory/Rotativa/Linux).
        /// </summary>
        public static string GetEngineLinuxPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Rotativa", "Linux", "wkhtmltopdf");
        }

        public static void VerificarWkhtmltopdf(ILogger logger, string contentRootPath = null, string webRootPath = null)
        {
            try
            {
                EnsureWindowsRotativaBinary(contentRootPath, webRootPath, logger);
                EnsureLinuxRotativaBinary(webRootPath, logger);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var enginePath = GetEngineWindowsPath();
                    if (IsValidWkhtmlBinary(enginePath))
                    {
                        logger.LogInformation("PDF: wkhtmltopdf listo en {Path}", enginePath);
                        return;
                    }

                    var wwwPath = GetWebRootWindowsPath(webRootPath);
                    if (IsValidWkhtmlBinary(wwwPath))
                    {
                        logger.LogWarning(
                            "PDF: wkhtmltopdf existe en wwwroot ({WwwPath}) pero no en la ruta del motor ({EnginePath}). " +
                            "Se intentara sincronizar al generar PDFs.",
                            wwwPath,
                            enginePath);
                        return;
                    }

                    var path = BuscarEnPath("wkhtmltopdf.exe");
                    if (path == null)
                    {
                        logger.LogWarning(
                            "PDF: wkhtmltopdf no encontrado. Coloque el ejecutable en wwwroot/Rotativa/Windows/wkhtmltopdf.exe " +
                            "o en {EnginePath}.",
                            enginePath);
                    }
                    else
                    {
                        logger.LogInformation("PDF: wkhtmltopdf detectado en PATH: {Path}", path);
                    }

                    return;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                    RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    var enginePath = GetEngineLinuxPath();
                    if (IsUsableWkhtmlPath(enginePath))
                    {
                        logger.LogInformation("PDF: wkhtmltopdf listo en {Path}", enginePath);
                        return;
                    }

                    var path = BuscarEnPath("wkhtmltopdf");
                    if (path == null)
                    {
                        logger.LogWarning(
                            "PDF: wkhtmltopdf NO instalado en Linux/macOS. " +
                            "Ejecute: sudo apt-get install -y wkhtmltopdf (Debian/Ubuntu).");
                    }
                    else
                    {
                        logger.LogWarning(
                            "PDF: wkhtmltopdf en PATH ({SystemPath}) pero falta en {EnginePath}. " +
                            "Se intentara crear el wrapper al generar PDFs.",
                            path,
                            enginePath);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "No se pudo verificar wkhtmltopdf.");
            }
        }

        /// <summary>
        /// Wkhtmltopdf.NetCore reads bin/Rotativa/Windows; wwwroot holds the deploy copy. Sync on startup/build.
        /// </summary>
        public static void EnsureWindowsRotativaBinary(string contentRootPath, string webRootPath, ILogger logger = null)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            var enginePath = GetEngineWindowsPath();
            if (IsValidWkhtmlBinary(enginePath))
                return;

            var source = ResolveWindowsSourceBinary(contentRootPath, webRootPath);
            if (source == null)
                return;

            try
            {
                var destDir = Path.GetDirectoryName(enginePath);
                if (!string.IsNullOrEmpty(destDir))
                    Directory.CreateDirectory(destDir);

                File.Copy(source, enginePath, overwrite: true);
                logger?.LogInformation("PDF: wkhtmltopdf copiado {Source} -> {Dest}", source, enginePath);
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "PDF: no se pudo copiar wkhtmltopdf a {Path}", enginePath);
            }
        }

        /// <summary>
        /// Wkhtmltopdf.NetCore on Linux reads AppContext.BaseDirectory/Rotativa/Linux/wkhtmltopdf.
        /// Rotativa.AspNetCore uses wwwroot/Rotativa/Linux/wkhtmltopdf. Create wrappers for both.
        /// </summary>
        public static void EnsureLinuxRotativaBinary(string webRootPath, ILogger logger = null)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return;

            var systemPath = BuscarEnPath("wkhtmltopdf");
            if (systemPath == null)
            {
                logger?.LogWarning(
                    "PDF: wkhtmltopdf no esta en PATH. Instale con: apt-get install -y wkhtmltopdf");
                return;
            }

            EnsureLinuxWrapperAt(GetEngineLinuxPath(), systemPath, logger);

            if (!string.IsNullOrWhiteSpace(webRootPath))
            {
                var wwwWrapper = Path.Combine(webRootPath, "Rotativa", "Linux", "wkhtmltopdf");
                EnsureLinuxWrapperAt(wwwWrapper, systemPath, logger);
            }
        }

        /// <summary>Backward-compatible alias.</summary>
        public static void EnsureLinuxRotativaWrapper(string webRootPath, ILogger logger = null)
            => EnsureLinuxRotativaBinary(webRootPath, logger);

        public static bool IsWkhtmltopdfAvailable(string webRootPath, string contentRootPath = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                EnsureWindowsRotativaBinary(contentRootPath, webRootPath);
                if (IsValidWkhtmlBinary(GetEngineWindowsPath()))
                    return true;

                if (IsValidWkhtmlBinary(GetWebRootWindowsPath(webRootPath)))
                    return true;

                return BuscarEnPath("wkhtmltopdf.exe") != null;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                EnsureLinuxRotativaBinary(webRootPath);
                if (IsUsableWkhtmlPath(GetEngineLinuxPath()))
                    return true;

                if (!string.IsNullOrWhiteSpace(webRootPath))
                {
                    var wwwWrapper = Path.Combine(webRootPath, "Rotativa", "Linux", "wkhtmltopdf");
                    if (IsUsableWkhtmlPath(wwwWrapper))
                        return true;
                }

                return false;
            }

            return false;
        }

        public static string GetUnavailableMessage()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "No se puede generar el PDF: falta wkhtmltopdf en "
                     + GetEngineLinuxPath()
                     + ". En Docker instale wkhtmltopdf en la imagen y reinicie la aplicacion.";
            }

            var enginePath = GetEngineWindowsPath();
            return "No se puede generar el PDF: falta wkhtmltopdf. " +
                   "En Windows coloque wkhtmltopdf.exe en wwwroot/Rotativa/Windows/ " +
                   $"o en {enginePath}.";
        }

        private static string ResolveWindowsSourceBinary(string contentRootPath, string webRootPath)
        {
            var www = GetWebRootWindowsPath(webRootPath);
            if (IsValidWkhtmlBinary(www))
                return www;

            var projectRoot = contentRootPath;
            if (!string.IsNullOrWhiteSpace(projectRoot))
            {
                var fromProjectWww = Path.Combine(projectRoot, "wwwroot", "Rotativa", "Windows", "wkhtmltopdf.exe");
                if (IsValidWkhtmlBinary(fromProjectWww))
                    return fromProjectWww;

                var fromProjectRotativa = Path.Combine(projectRoot, "Rotativa", "Windows", "wkhtmltopdf.exe");
                if (IsValidWkhtmlBinary(fromProjectRotativa))
                    return fromProjectRotativa;
            }

            return BuscarEnPath("wkhtmltopdf.exe");
        }

        private static void EnsureLinuxWrapperAt(string wrapperPath, string systemPath, ILogger logger)
        {
            if (IsUsableWkhtmlPath(wrapperPath))
                return;

            try
            {
                var dir = Path.GetDirectoryName(wrapperPath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                var script = "#!/bin/bash\nexec \"" + systemPath.Replace("\"", "\\\"") + "\" \"$@\"\n";
                File.WriteAllText(wrapperPath, script);
                MakeExecutable(wrapperPath);
                logger?.LogInformation("PDF: wrapper {Wrapper} -> {SystemPath}", wrapperPath, systemPath);
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "PDF: no se pudo crear wrapper wkhtmltopdf en {Path}", wrapperPath);
            }
        }

        private static void MakeExecutable(string path)
        {
            if (!OperatingSystem.IsLinux())
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = "+x \"" + path + "\"",
                UseShellExecute = false,
                CreateNoWindow = true
            })?.WaitForExit(3000);
        }

        private static bool IsUsableWkhtmlPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return false;

            if (IsValidWkhtmlBinary(path))
                return true;

            try
            {
                return new FileInfo(path).Length > 10;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidWkhtmlBinary(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return false;

            try
            {
                return new FileInfo(path).Length >= MinValidBinaryBytes;
            }
            catch
            {
                return false;
            }
        }

        private static string BuscarEnPath(string executable)
        {
            var pathVar = Environment.GetEnvironmentVariable("PATH") ?? "";
            foreach (var dir in pathVar.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    var full = Path.Combine(dir.Trim(), executable);
                    if (IsValidWkhtmlBinary(full))
                        return full;
                }
                catch
                {
                    // ignore
                }
            }

            try
            {
                using var proc = Process.Start(new ProcessStartInfo
                {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which",
                    Arguments = executable.Replace(".exe", ""),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
                if (proc == null)
                    return null;
                var output = proc.StandardOutput.ReadToEnd().Trim();
                proc.WaitForExit(3000);
                if (proc.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
                    return null;

                var candidate = output.Split('\n', '\r')[0].Trim();
                return IsValidWkhtmlBinary(candidate) ? candidate : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
