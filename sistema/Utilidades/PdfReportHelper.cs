using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Database.Shared.IRepository;
using Database.Shared.Models;
using System.IO;
using System.Linq;
using Docnet.Core;
using Docnet.Core.Models;
using farmamest.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace farmamest.Utilidades
{
    public static class PdfReportHelper
    {
        public static bool EsClienteSerSalud(string cliente)
        {
            return string.Equals(cliente, "SS", StringComparison.OrdinalIgnoreCase)
                || string.Equals(cliente, "SerSalud", StringComparison.OrdinalIgnoreCase);
        }

        public static string ObtenerFirmaBase64Local(string rutaFirma, string contentRootPath)
        {
            if (string.IsNullOrWhiteSpace(rutaFirma) || string.IsNullOrWhiteSpace(contentRootPath))
                return "";

            var rutaFinal = ResolverRutaArchivoLocal(rutaFirma, contentRootPath);
            if (rutaFinal == null)
                return "";

            var bytes = File.ReadAllBytes(rutaFinal);
            var mime = ObtenerMimeImagenPorExtension(rutaFinal);
            return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
        }

        public static string ResolverRutaArchivoLocal(string rutaArchivo, string contentRootPath)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo) || string.IsNullOrWhiteSpace(contentRootPath))
                return null;

            if (rutaArchivo.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return null;

            if (Path.IsPathRooted(rutaArchivo) && File.Exists(rutaArchivo))
                return rutaArchivo;

            var rutaNormalizada = rutaArchivo.Replace("\\", "/").Trim();
            if (rutaNormalizada.StartsWith("~/", StringComparison.Ordinal))
                rutaNormalizada = rutaNormalizada[2..];

            while (rutaNormalizada.StartsWith("../", StringComparison.Ordinal)
                || rutaNormalizada.StartsWith("./", StringComparison.Ordinal)
                || rutaNormalizada.StartsWith("/..", StringComparison.Ordinal)
                || rutaNormalizada.StartsWith("/.", StringComparison.Ordinal))
            {
                rutaNormalizada = rutaNormalizada.TrimStart('.').TrimStart('/');
            }

            rutaNormalizada = rutaNormalizada.TrimStart('/');
            var wwwroot = Path.Combine(contentRootPath, "wwwroot");
            var candidatos = new List<string>
            {
                Path.Combine(wwwroot, rutaNormalizada),
                Path.Combine(contentRootPath, rutaNormalizada),
                Path.Combine(wwwroot, rutaNormalizada.Replace('/', Path.DirectorySeparatorChar))
            };

            if (rutaNormalizada.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
                candidatos.Add(Path.Combine(wwwroot, rutaNormalizada));

            foreach (var rutaFinal in candidatos.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (File.Exists(rutaFinal))
                    return rutaFinal;
            }

            return null;
        }

        public static List<string> RenderizarPdfComoPaginasBase64(string rutaPdf, int maxPaginas = 15)
        {
            var paginas = new List<string>();
            if (string.IsNullOrWhiteSpace(rutaPdf) || !File.Exists(rutaPdf))
                return paginas;

            try
            {
                using var docReader = DocLib.Instance.GetDocReader(
                    File.ReadAllBytes(rutaPdf),
                    new PageDimensions(1080, 1920));
                var total = Math.Min(docReader.GetPageCount(), Math.Max(1, maxPaginas));
                for (var i = 0; i < total; i++)
                {
                    using var pageReader = docReader.GetPageReader(i);
                    var rawBytes = pageReader.GetImage();
                    var width = pageReader.GetPageWidth();
                    var height = pageReader.GetPageHeight();
                    using var image = Image.LoadPixelData<Bgra32>(rawBytes, width, height);
                    using var ms = new MemoryStream();
                    image.Save(ms, new PngEncoder());
                    paginas.Add($"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}");
                }
            }
            catch
            {
                // Si el PDF no se puede renderizar, el llamador mostrará la URL original.
            }

            return paginas;
        }

        public static DocumentoEmbebidoVm ResolverDocumentoEmbebido(
            string url,
            string nombre,
            string contentRootPath,
            int maxPaginasPdf = 15)
        {
            var vm = new DocumentoEmbebidoVm
            {
                Nombre = nombre ?? "-",
                UrlOriginal = url ?? ""
            };

            if (string.IsNullOrWhiteSpace(url))
                return vm;

            if (url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                vm.ArchivoExiste = true;
                vm.PaginasBase64.Add(url);
                return vm;
            }

            var ruta = ResolverRutaArchivoLocal(url, contentRootPath);
            if (ruta == null)
                return vm;

            vm.ArchivoExiste = true;
            var ext = Path.GetExtension(ruta)?.ToLowerInvariant();
            if (ext == ".pdf")
                vm.PaginasBase64 = RenderizarPdfComoPaginasBase64(ruta, maxPaginasPdf);
            else if (ext is ".png" or ".jpg" or ".jpeg" or ".gif" or ".webp")
            {
                var img = ObtenerFirmaBase64Local(url, contentRootPath);
                if (!string.IsNullOrEmpty(img))
                    vm.PaginasBase64.Add(img);
            }

            return vm;
        }

        public static List<DocumentoEmbebidoVm> ResolverDocumentosEmbebidos(
            IEnumerable<(string Nombre, string Url)> archivos,
            string contentRootPath,
            int maxPaginasPdf = 15)
        {
            return archivos?
                .Where(a => !string.IsNullOrWhiteSpace(a.Url))
                .Select(a => ResolverDocumentoEmbebido(a.Url, a.Nombre, contentRootPath, maxPaginasPdf))
                .ToList() ?? new List<DocumentoEmbebidoVm>();
        }

        public static string ObtenerArchivoEmbebibleBase64(string rutaArchivo, string contentRootPath)
        {
            return ResolverDocumentoEmbebido(rutaArchivo, null, contentRootPath, 1)
                .PaginasBase64
                .FirstOrDefault() ?? "";
        }

        public static string ResolverFirmaDocumentoClinico(
            bool autorizado,
            string usuarioAutorizaId,
            User profesional,
            IReadOnlyDictionary<string, User> usuariosPorId,
            IEmpleado empleadoRepository,
            string firmaMedicoTratanteFallback,
            string contentRootPath)
        {
            var firmante = ResolverFirmanteClinico(
                autorizado,
                usuarioAutorizaId,
                profesional,
                usuariosPorId,
                empleadoRepository,
                firmaMedicoTratanteFallback,
                contentRootPath);
            return firmante.FirmaBase64 ?? "";
        }

        public static void AsignarFirmaFarmaciaMedicamentos(
            ConsentimientoHospiVM consentimiento,
            string imagenFirmaEstablecimiento,
            string contentRootPath)
        {
            if (consentimiento == null || string.IsNullOrWhiteSpace(imagenFirmaEstablecimiento))
                return;

            consentimiento.UrlFirmaFarmacia = ResolverFirmaEstablecimiento(imagenFirmaEstablecimiento, contentRootPath);
        }

        public static string ResolverFirmaEstablecimiento(string imagenFirmaEstablecimiento, string contentRootPath)
        {
            if (string.IsNullOrWhiteSpace(imagenFirmaEstablecimiento))
                return "";

            if (imagenFirmaEstablecimiento.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return imagenFirmaEstablecimiento;

            return ObtenerFirmaBase64Local(imagenFirmaEstablecimiento, contentRootPath);
        }

        public static void AplicarFirmaEstablecimientoComoFallbackMedico(
            ConsentimientoHospiVM consentimiento,
            string imagenFirmaEstablecimiento,
            string contentRootPath)
        {
            if (consentimiento == null || EsRutaImagenLogo(imagenFirmaEstablecimiento))
                return;

            var firma = ResolverFirmaEstablecimiento(imagenFirmaEstablecimiento, contentRootPath);
            if (string.IsNullOrEmpty(firma) || EsRutaImagenLogo(firma))
                return;

            if (string.IsNullOrEmpty(consentimiento.UrlFirmaMedico))
                consentimiento.UrlFirmaMedico = firma;
            if (string.IsNullOrEmpty(consentimiento.FirmaMedicoBase64))
                consentimiento.FirmaMedicoBase64 = firma;
        }

        public static bool EsRutaImagenLogo(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return false;

            return valor.Contains("logo", StringComparison.OrdinalIgnoreCase)
                || valor.Contains("LogoSS", StringComparison.OrdinalIgnoreCase)
                || valor.Contains("logoSS", StringComparison.OrdinalIgnoreCase);
        }

        public static string GenerarFirmaSvgBase64(string nombre)
        {
            nombre = string.IsNullOrWhiteSpace(nombre) ? "Firma" : nombre.Trim();
            var seguro = WebUtility.HtmlEncode(nombre);
            var svg =
                $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""220"" height=""80"" viewBox=""0 0 220 80"">
<rect width=""220"" height=""80"" fill=""white""/>
<path d=""M8 42 C28 18, 48 58, 68 38 S108 52, 128 36 S168 54, 188 40 S208 48, 214 44"" stroke=""#1e3a8a"" fill=""none"" stroke-width=""2""/>
<path d=""M12 54 C32 62, 52 48, 72 58 S112 50, 132 60"" stroke=""#1e3a8a"" fill=""none"" stroke-width=""1.5"" opacity=""0.8""/>
<text x=""14"" y=""72"" font-family=""'Segoe Script', cursive, serif"" font-size=""15"" fill=""#111827"">{seguro}</text>
</svg>";
            return "data:image/svg+xml;base64," + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svg));
        }

        public static void CompletarFirmasConsentimientoSerSalud(
            ConsentimientoHospiVM consentimiento,
            string medicoFirmaBase64,
            string contentRootPath,
            string establecimientoImagenFirma = null,
            string establecimientoFirmaRepresentante = null,
            bool permitirFirmaSvgPlaceholder = false)
        {
            if (consentimiento == null)
                return;

            EnriquecerConsentimientoFirmas(consentimiento, contentRootPath);

            var firmaMedico = FirstNonEmptyFirma(
                medicoFirmaBase64,
                consentimiento.FirmaMedicoBase64,
                consentimiento.UrlFirmaMedico);

            if (EsRutaImagenLogo(firmaMedico))
                firmaMedico = null;

            if (string.IsNullOrEmpty(firmaMedico) && permitirFirmaSvgPlaceholder)
            {
                firmaMedico = GenerarFirmaSvgBase64(
                    consentimiento.NombreMedicoTratante ?? "Médico tratante");
            }

            consentimiento.FirmaMedicoBase64 = firmaMedico;
            consentimiento.UrlFirmaMedico = firmaMedico;

            var firmaRepresentante = FirstNonEmptyFirma(
                consentimiento.FirmaRepresentanteBase64,
                ResolverFirmaDesdeCampo(consentimiento.URLFirmaRepresentanteNaranjo, contentRootPath));

            if (string.IsNullOrEmpty(firmaRepresentante)
                && !string.IsNullOrWhiteSpace(establecimientoFirmaRepresentante)
                && !EsRutaImagenLogo(establecimientoFirmaRepresentante))
            {
                firmaRepresentante = ResolverFirmaEstablecimiento(establecimientoFirmaRepresentante, contentRootPath);
            }

            if (string.IsNullOrEmpty(firmaRepresentante)
                && !string.IsNullOrWhiteSpace(establecimientoImagenFirma)
                && !EsRutaImagenLogo(establecimientoImagenFirma))
            {
                firmaRepresentante = ResolverFirmaEstablecimiento(establecimientoImagenFirma, contentRootPath);
            }

            if ((string.IsNullOrEmpty(firmaRepresentante) || EsRutaImagenLogo(firmaRepresentante))
                && permitirFirmaSvgPlaceholder)
            {
                firmaRepresentante = GenerarFirmaSvgBase64("Erwin A. Aragón M.");
            }

            consentimiento.FirmaRepresentanteBase64 = firmaRepresentante;
            consentimiento.URLFirmaRepresentanteNaranjo = firmaRepresentante;
        }

        private static string FirstNonEmptyFirma(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value) && value != "-")
                    return value;
            }

            return null;
        }

        private static string ObtenerMimeImagenPorExtension(string ruta)
        {
            return Path.GetExtension(ruta)?.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/png"
            };
        }

        /// <summary>
        /// Decodifica entidades HTML y elimina etiquetas para texto plano en tablas.
        /// </summary>
        public static string TextoPlanoDesdeHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return "-";

            var decoded = WebUtility.HtmlDecode(html);
            var sinTags = Regex.Replace(decoded, "<[^>]+>", " ");
            return Regex.Replace(sinTags, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Normaliza contenido HTML de editor enriquecido para impresión PDF.
        /// </summary>
        public static string NormalizarHtmlParaPdf(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return "-";

            return WebUtility.HtmlDecode(html);
        }

        public static bool EsEmpleadoMedicoClinico(Empleado empleado)
        {
            if (empleado == null)
                return false;

            if (empleado.EspecialidadId is > 0)
                return true;

            if (!string.IsNullOrWhiteSpace(empleado.Colegiado))
                return true;

            var tipo = empleado.TipoEmpleado ?? "";
            return tipo.Contains("medico", StringComparison.OrdinalIgnoreCase)
                || tipo.Contains("médico", StringComparison.OrdinalIgnoreCase)
                || tipo.Contains("doctor", StringComparison.OrdinalIgnoreCase);
        }

        public static (string Nombre, string Especialidad, string Colegiado, string FirmaBase64) ResolverMedicoTratanteHospitalizacion(
            Hospitalizacion hospitalizacion,
            IEmpleado empleadoRepository,
            string contentRootPath,
            ICitas citasRepository = null,
            int? citaId = null,
            string cirujanoNotaOperatoria = null,
            string nombreMedicoTratantePreferido = null)
        {
            if (hospitalizacion == null)
                return ("-", "-", "-", "");

            if (!string.IsNullOrWhiteSpace(nombreMedicoTratantePreferido)
                && nombreMedicoTratantePreferido != "-")
            {
                var empPref = BuscarEmpleadoPorNombre(nombreMedicoTratantePreferido, empleadoRepository);
                if (empPref != null && EsEmpleadoMedicoClinico(empPref))
                {
                    return (
                        nombreMedicoTratantePreferido.Trim(),
                        empPref.Especialidad?.NombreEspecialidad ?? "-",
                        empPref.Colegiado ?? "-",
                        ObtenerFirmaBase64Local(empPref.FirmaEmpleado, contentRootPath));
                }

                return (nombreMedicoTratantePreferido.Trim(), "-", "-", "");
            }

            var cita = hospitalizacion.Consultas?.FirstOrDefault()?.Citas;
            if (cita == null && citasRepository != null)
            {
                var citaIdResuelto = citaId ?? hospitalizacion.Consultas?.FirstOrDefault()?.CitasId ?? 0;
                if (citaIdResuelto > 0)
                    cita = citasRepository.GetCita(citaIdResuelto);
            }

            string nombre = cita?.EmpleadoText ?? "";
            string especialidad = cita?.Empleado?.Especialidad?.NombreEspecialidad ?? "-";
            string colegiado = cita?.Empleado?.Colegiado ?? "-";
            string firma = "";

            if (cita?.EmpleadoId != null)
            {
                var emp = empleadoRepository.Get(cita.EmpleadoId.Value);
                if (emp != null && EsEmpleadoMedicoClinico(emp))
                {
                    if (string.IsNullOrWhiteSpace(nombre))
                        nombre = emp.NombreYApellidos ?? "";
                    if (string.IsNullOrWhiteSpace(colegiado) || colegiado == "-")
                        colegiado = emp.Colegiado ?? "-";
                    if (string.IsNullOrWhiteSpace(especialidad) || especialidad == "-")
                        especialidad = emp.Especialidad?.NombreEspecialidad ?? "-";
                    firma = ObtenerFirmaBase64Local(emp.FirmaEmpleado, contentRootPath);
                }
                else if (emp != null)
                {
                    nombre = "";
                    especialidad = "-";
                    colegiado = "-";
                    firma = "";
                }
            }

            if (string.IsNullOrWhiteSpace(nombre) || nombre == "-")
            {
                if (!string.IsNullOrWhiteSpace(cirujanoNotaOperatoria))
                {
                    nombre = cirujanoNotaOperatoria;
                    var empCir = BuscarEmpleadoPorNombre(cirujanoNotaOperatoria, empleadoRepository);
                    if (empCir != null)
                    {
                        if (string.IsNullOrWhiteSpace(colegiado) || colegiado == "-")
                            colegiado = empCir.Colegiado ?? "-";
                        if (string.IsNullOrEmpty(firma))
                            firma = ObtenerFirmaBase64Local(empCir.FirmaEmpleado, contentRootPath);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(nombre))
                nombre = "-";

            return (nombre, especialidad, colegiado, firma);
        }

        public static (string Nombre, string Especialidad, string Colegiado, string FirmaBase64) ComplementarMedicoTratante(
            (string Nombre, string Especialidad, string Colegiado, string FirmaBase64) medico,
            ConsentimientoHospiVM consentimiento = null,
            CuestionarioPreAnestesico cuestionario = null,
            AutorizacionAnestesiaPdfVM autorizacionAnestesia = null,
            IEmpleado empleadoRepository = null,
            string contentRootPath = null)
        {
            var nombreConsentimiento = consentimiento?.NombreMedicoTratante;
            if (!string.IsNullOrWhiteSpace(nombreConsentimiento) && nombreConsentimiento != "-")
            {
                var colegiadoConsentimiento = consentimiento?.ColegiadoMedico;
                if (string.IsNullOrWhiteSpace(colegiadoConsentimiento) || colegiadoConsentimiento == "-")
                    colegiadoConsentimiento = medico.Colegiado;

                var firma = medico.FirmaBase64;
                if (empleadoRepository != null && string.IsNullOrEmpty(firma))
                {
                    var emp = BuscarEmpleadoPorNombre(nombreConsentimiento, empleadoRepository);
                    if (emp != null)
                        firma = ObtenerFirmaBase64Local(emp.FirmaEmpleado, contentRootPath);
                }

                return (nombreConsentimiento.Trim(), medico.Especialidad, colegiadoConsentimiento ?? "-", firma ?? "");
            }

            if (!string.IsNullOrWhiteSpace(medico.Nombre) && medico.Nombre != "-")
                return medico;

            var nombre = consentimiento?.NombreMedicoTratante;
            if (string.IsNullOrWhiteSpace(nombre))
                nombre = cuestionario?.Cirujano;
            if (string.IsNullOrWhiteSpace(nombre))
                nombre = autorizacionAnestesia?.NombreMedicoTratante;
            if (string.IsNullOrWhiteSpace(nombre))
                nombre = "-";

            var colegiado = medico.Colegiado;
            if (string.IsNullOrWhiteSpace(colegiado) || colegiado == "-")
                colegiado = consentimiento?.ColegiadoMedico
                    ?? autorizacionAnestesia?.ColegiadoMedico
                    ?? "-";

            return (nombre, medico.Especialidad, colegiado, medico.FirmaBase64);
        }

        public static string ObtenerFirmaEmpleadoPorUser(User user, IEmpleado empleadoRepository, string contentRootPath)
        {
            if (user?.EmpleadoId == null)
                return "";

            var emp = empleadoRepository.Get(user.EmpleadoId.Value);
            return emp == null ? "" : ObtenerFirmaBase64Local(emp.FirmaEmpleado, contentRootPath);
        }

        public static bool EsGuid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && Guid.TryParse(value.Trim(), out _);
        }

        public static string ObtenerNombreEmpleadoPorUser(User user, IEmpleado empleadoRepository = null, IUser userRepository = null)
        {
            if (user == null)
                return "-";

            if (user.Persona != null && !string.IsNullOrWhiteSpace(user.Persona.NombreYApellidos))
                return user.Persona.NombreYApellidos;

            if (empleadoRepository != null && user.EmpleadoId != null)
            {
                var emp = empleadoRepository.Get(user.EmpleadoId.Value);
                if (!string.IsNullOrWhiteSpace(emp?.NombreYApellidos))
                    return emp.NombreYApellidos;
            }

            if (userRepository != null)
            {
                var display = userRepository.GetDisplayName(user.Id);
                if (!string.IsNullOrWhiteSpace(display) && !EsGuid(display))
                    return display;
            }

            if (EsGuid(user.UserName))
                return "-";

            return user.UserName ?? "-";
        }

        public static User ResolverUsuarioProfesionalTexto(string texto, IUser userRepository)
        {
            if (string.IsNullOrWhiteSpace(texto) || userRepository == null)
                return null;

            texto = texto.Trim();
            if (Guid.TryParse(texto, out _))
                return userRepository.GetbyId(texto);

            if (texto.Contains('@'))
                return userRepository.Get(texto);

            return null;
        }

        public static string ResolverNombreProfesionalTexto(
            string texto,
            IUser userRepository,
            IEmpleado empleadoRepository)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "-";

            texto = texto.Trim();

            if (Guid.TryParse(texto, out _) && userRepository != null)
            {
                var display = userRepository.GetDisplayName(texto);
                if (!string.IsNullOrWhiteSpace(display) && display != texto)
                    return display;

                var user = userRepository.GetbyId(texto);
                if (user != null)
                    return ObtenerNombreEmpleadoPorUser(user, empleadoRepository);
            }

            if (texto.Contains('@') && userRepository != null)
            {
                var user = userRepository.Get(texto);
                if (user != null)
                    return ObtenerNombreEmpleadoPorUser(user, empleadoRepository);
            }

            var emp = BuscarEmpleadoPorNombre(texto, empleadoRepository);
            if (emp != null)
                return emp.NombreYApellidos;

            return EsGuid(texto) ? "-" : texto;
        }

        public static void ResolverMedicoSecundarioCita(
            string datoCita,
            IEmpleado empleadoRepository,
            out string nombre,
            out string colegiado,
            out string firmaBase64,
            string contentRootPath)
        {
            nombre = "";
            colegiado = "";
            firmaBase64 = "";

            if (string.IsNullOrWhiteSpace(datoCita) || EsNombreAnestesistaVacio(datoCita))
                return;

            if (int.TryParse(datoCita, out int idEmp))
            {
                var emp = empleadoRepository.Get(idEmp, false);
                if (emp != null)
                {
                    nombre = emp.NombreYApellidos;
                    colegiado = emp.Colegiado ?? "";
                    firmaBase64 = ObtenerFirmaBase64Local(emp.FirmaEmpleado, contentRootPath);
                }
                return;
            }

            var empPorNombre = BuscarEmpleadoPorNombre(datoCita, empleadoRepository);
            if (empPorNombre != null)
            {
                nombre = empPorNombre.NombreYApellidos;
                colegiado = empPorNombre.Colegiado ?? "";
                firmaBase64 = ObtenerFirmaBase64Local(empPorNombre.FirmaEmpleado, contentRootPath);
            }
            else if (!EsNombreAnestesistaVacio(datoCita))
            {
                nombre = datoCita.Trim();
            }
        }

        public static void AplicarMedicoTratanteEnNotas(
            IList<NotaMedica2ViewModel> notas,
            string nombreMedico,
            string colegiadoMedico,
            string firmaMedico = null)
        {
            if (notas == null || string.IsNullOrWhiteSpace(nombreMedico) || nombreMedico == "-")
                return;

            foreach (var nota in notas)
            {
                if (nota == null)
                    continue;

                if (string.IsNullOrWhiteSpace(nota.EmpleadoText) || nota.EmpleadoText == "-" || nota.EmpleadoText == "Sin asignar")
                    nota.EmpleadoText = nombreMedico;

                if (string.IsNullOrWhiteSpace(nota.ColegioEmpleado) || nota.ColegioEmpleado == "-" || nota.ColegioEmpleado == "No disponible")
                    nota.ColegioEmpleado = colegiadoMedico;

                nota.Profesional = nombreMedico;

                var tieneCapturador = !string.IsNullOrWhiteSpace(nota.RegistradoPor)
                    || !string.IsNullOrWhiteSpace(nota.NombreFirmante);
                if (tieneCapturador || nota.Autorizado)
                    continue;

                if (string.IsNullOrEmpty(nota.FirmaBase64) && !string.IsNullOrEmpty(firmaMedico))
                    nota.FirmaBase64 = firmaMedico;

                if (string.IsNullOrWhiteSpace(nota.NombreFirmante) || nota.NombreFirmante == "-")
                    nota.NombreFirmante = nombreMedico;
            }
        }

        public static void AplicarMedicoTratanteEnOrdenes(
            IList<OrdenMedicaViewModel> ordenes,
            string nombreMedico,
            string colegiadoMedico)
        {
            if (ordenes == null || string.IsNullOrWhiteSpace(nombreMedico) || nombreMedico == "-")
                return;

            foreach (var orden in ordenes)
            {
                if (orden == null)
                    continue;

                if (string.IsNullOrWhiteSpace(orden.EmpleadoText) || orden.EmpleadoText == "-" || orden.EmpleadoText == "Sin asignar")
                    orden.EmpleadoText = nombreMedico;

                if (string.IsNullOrWhiteSpace(orden.ColegioEmpleado) || orden.ColegioEmpleado == "-" || orden.ColegioEmpleado == "No disponible")
                    orden.ColegioEmpleado = colegiadoMedico;
            }
        }

        public static string ResolverNombreMedicoPdf(string citaEmpleado, string profesionalOrden, string medicoTratante)
        {
            if (!string.IsNullOrWhiteSpace(citaEmpleado) && citaEmpleado != "Sin asignar" && citaEmpleado != "-")
                return citaEmpleado;
            if (!string.IsNullOrWhiteSpace(profesionalOrden) && profesionalOrden != "Sin asignar")
                return profesionalOrden;
            if (!string.IsNullOrWhiteSpace(medicoTratante) && medicoTratante != "-")
                return medicoTratante;
            return "Sin asignar";
        }

        public static Empleado BuscarEmpleadoPorNombre(string nombre, IEmpleado empleadoRepository)
        {
            if (string.IsNullOrWhiteSpace(nombre) || empleadoRepository == null)
                return null;

            return empleadoRepository.GetList()
                .FirstOrDefault(e =>
                    string.Equals(e.NombreYApellidos, nombre, StringComparison.OrdinalIgnoreCase)
                    || (!string.IsNullOrWhiteSpace(e.NombreYApellidos)
                        && e.NombreYApellidos.Contains(nombre, StringComparison.OrdinalIgnoreCase)));
        }

        public static string ObtenerFirmaEmpleadoPorNombre(string nombre, IEmpleado empleadoRepository, string contentRootPath)
        {
            var emp = BuscarEmpleadoPorNombre(nombre, empleadoRepository);
            return emp == null ? "" : ObtenerFirmaBase64Local(emp.FirmaEmpleado, contentRootPath);
        }

        public static string ObtenerColegiadoPorNombre(string nombre, IEmpleado empleadoRepository, string fallback = "-")
        {
            var emp = BuscarEmpleadoPorNombre(nombre, empleadoRepository);
            return !string.IsNullOrWhiteSpace(emp?.Colegiado) ? emp.Colegiado : fallback;
        }

        public static string ObtenerNombreRegistroNota(User profesional, IEmpleado empleadoRepository)
        {
            var nombre = profesional?.Persona?.NombreYApellidos
                ?? profesional?.Persona?.Nombre
                ?? ObtenerNombreEmpleadoPorUser(profesional, empleadoRepository);
            if (string.IsNullOrWhiteSpace(nombre))
                nombre = profesional?.UserName;
            return string.IsNullOrWhiteSpace(nombre) ? null : nombre.Trim();
        }

        public static string NormalizarNombreMedicoTratante(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre) || nombre == "-" || nombre == "Sin asignar")
                return "Sin asignar";
            return nombre.Trim();
        }

        public static (string FirmaBase64, string NombreFirmante, string AutorizadoPor) ResolverFirmanteNotaMedicaPdf(
            bool autorizado,
            string usuarioAutorizaId,
            User profesionalRegistro,
            IReadOnlyDictionary<string, User> usuariosPorId,
            IEmpleado empleadoRepository,
            (string Nombre, string Especialidad, string Colegiado, string FirmaBase64) medicoTratante,
            string contentRootPath)
        {
            var medicoNombre = NormalizarNombreMedicoTratante(medicoTratante.Nombre);

            if (autorizado
                && !string.IsNullOrWhiteSpace(usuarioAutorizaId)
                && usuariosPorId != null
                && usuariosPorId.TryGetValue(usuarioAutorizaId, out var autorizador))
            {
                var firmanteAutorizador = ResolverFirmaYNombrePorUser(
                    autorizador, empleadoRepository, contentRootPath);
                if (!string.IsNullOrWhiteSpace(firmanteAutorizador.Nombre))
                    return (firmanteAutorizador.FirmaBase64, firmanteAutorizador.Nombre, firmanteAutorizador.Nombre);
            }

            var nombreRegistrante = ObtenerNombreRegistroNota(profesionalRegistro, empleadoRepository);
            var firmaRegistrante = ObtenerFirmaEmpleadoPorUser(profesionalRegistro, empleadoRepository, contentRootPath);
            if (!string.IsNullOrEmpty(firmaRegistrante) && !string.IsNullOrWhiteSpace(nombreRegistrante))
                return (firmaRegistrante, nombreRegistrante, null);

            if (!string.IsNullOrWhiteSpace(nombreRegistrante))
            {
                var firmaPorRegistrante = ObtenerFirmaEmpleadoPorNombre(nombreRegistrante, empleadoRepository, contentRootPath);
                if (!string.IsNullOrEmpty(firmaPorRegistrante))
                    return (firmaPorRegistrante, nombreRegistrante, null);
                return ("", nombreRegistrante, null);
            }

            if (!string.IsNullOrEmpty(medicoTratante.FirmaBase64))
                return (medicoTratante.FirmaBase64, medicoNombre, null);

            var firmaMedico = ObtenerFirmaEmpleadoPorNombre(medicoNombre, empleadoRepository, contentRootPath);
            if (!string.IsNullOrEmpty(firmaMedico))
                return (firmaMedico, medicoNombre, null);

            return ("", medicoNombre, null);
        }

        private static (string FirmaBase64, string Nombre) ResolverFirmaYNombrePorUser(
            User user,
            IEmpleado empleadoRepository,
            string contentRootPath)
        {
            var nombre = ObtenerNombreEmpleadoPorUser(user, empleadoRepository);
            if (string.IsNullOrWhiteSpace(nombre))
                nombre = ObtenerNombreRegistroNota(user, empleadoRepository);

            var firma = ObtenerFirmaEmpleadoPorUser(user, empleadoRepository, contentRootPath);
            if (string.IsNullOrEmpty(firma) && !string.IsNullOrWhiteSpace(nombre))
                firma = ObtenerFirmaEmpleadoPorNombre(nombre, empleadoRepository, contentRootPath);

            return (firma ?? "", nombre);
        }

        public static void CompletarProfesionalNotaMedicaPdf(
            NotaMedica2ViewModel vm,
            User profesionalRegistro,
            (string Nombre, string Especialidad, string Colegiado, string FirmaBase64) medicoTratante,
            bool autorizado,
            string usuarioAutorizaId,
            IReadOnlyDictionary<string, User> usuariosPorId,
            IEmpleado empleadoRepository,
            string contentRootPath)
        {
            if (vm == null)
                return;

            var medicoNombre = NormalizarNombreMedicoTratante(medicoTratante.Nombre);
            var colegiado = !string.IsNullOrWhiteSpace(medicoTratante.Colegiado) && medicoTratante.Colegiado != "-"
                ? medicoTratante.Colegiado
                : ObtenerColegiadoPorNombre(medicoNombre, empleadoRepository, "No disponible");

            var firmante = ResolverFirmanteNotaMedicaPdf(
                autorizado,
                usuarioAutorizaId,
                profesionalRegistro,
                usuariosPorId,
                empleadoRepository,
                medicoTratante,
                contentRootPath);

            vm.RegistradoPor = ObtenerNombreRegistroNota(profesionalRegistro, empleadoRepository);
            vm.Profesional = medicoNombre;
            vm.EmpleadoText = medicoNombre;
            vm.ColegioEmpleado = colegiado;
            vm.FirmaBase64 = firmante.FirmaBase64;
            vm.NombreFirmante = firmante.NombreFirmante;
            vm.AutorizadoPor = firmante.AutorizadoPor;
        }

        public static (string FirmaBase64, string NombreFirmante, string AutorizadoPor) ResolverFirmanteClinico(
            bool autorizado,
            string usuarioAutorizaId,
            User profesional,
            IReadOnlyDictionary<string, User> usuariosPorId,
            IEmpleado empleadoRepository,
            string firmaMedicoTratanteFallback,
            string contentRootPath,
            string registradoPor = null)
        {
            if (autorizado
                && !string.IsNullOrWhiteSpace(usuarioAutorizaId)
                && usuariosPorId != null
                && usuariosPorId.TryGetValue(usuarioAutorizaId, out var autorizador))
            {
                var firmanteAutorizador = ResolverFirmaYNombrePorUser(
                    autorizador, empleadoRepository, contentRootPath);
                if (!string.IsNullOrWhiteSpace(firmanteAutorizador.Nombre))
                    return (firmanteAutorizador.FirmaBase64, firmanteAutorizador.Nombre, firmanteAutorizador.Nombre);
            }

            var nombreProfesional = !string.IsNullOrWhiteSpace(registradoPor)
                ? registradoPor
                : ObtenerNombreEmpleadoPorUser(profesional, empleadoRepository);

            var firmaProfesional = ObtenerFirmaEmpleadoPorUser(profesional, empleadoRepository, contentRootPath);
            if (!string.IsNullOrEmpty(firmaProfesional) && !string.IsNullOrWhiteSpace(nombreProfesional))
                return (firmaProfesional, nombreProfesional, null);

            if (!string.IsNullOrWhiteSpace(nombreProfesional))
            {
                var firmaPorNombre = ObtenerFirmaEmpleadoPorNombre(nombreProfesional, empleadoRepository, contentRootPath);
                if (!string.IsNullOrEmpty(firmaPorNombre))
                    return (firmaPorNombre, nombreProfesional, null);
                return ("", nombreProfesional, null);
            }

            if (!string.IsNullOrEmpty(firmaMedicoTratanteFallback))
                return (firmaMedicoTratanteFallback, nombreProfesional ?? "Sin asignar", null);

            return ("", nombreProfesional ?? "Sin asignar", null);
        }

        public static AutorizacionAnestesiaPdfVM BuildAutorizacionAnestesia(
            Hospitalizacion hospitalizacion,
            ConsentimientoHospiVM consentimiento,
            ICitas citasRepository,
            IEmpleado empleadoRepository,
            string contentRootPath,
            int? citaId = null,
            string anestesistaNotaOperatoria = null)
        {
            var vm = new AutorizacionAnestesiaPdfVM
            {
                NombrePaciente = consentimiento?.NombreCompleto
                    ?? hospitalizacion?.Paciente?.Nombre
                    ?? "-",
                FechaAdmision = consentimiento?.HoraIngreso
                    ?? DateTime.Now.ToString("dd/MM/yyyy"),
                Procedimiento = consentimiento?.TratamientoMedico ?? "",
                NombreMedicoTratante = consentimiento?.NombreMedicoTratante ?? "",
                ColegiadoMedico = consentimiento?.ColegiadoMedico ?? ""
            };

            int citaIdResuelto = citaId ?? 0;
            if (citaIdResuelto == 0 && consentimiento?.CitaId is > 0)
                citaIdResuelto = consentimiento.CitaId.Value;
            if (citaIdResuelto == 0)
            {
                var consulta = hospitalizacion?.Consultas?.FirstOrDefault();
                if (consulta?.CitasId != null)
                    citaIdResuelto = consulta.CitasId.Value;
            }

            Citas cita = null;
            if (citaIdResuelto > 0)
            {
                cita = citasRepository.GetCita(citaIdResuelto);
                if (cita != null)
                {
                    vm.FechaAdmision = cita.FechaInicio?.ToString("dd/MM/yyyy") ?? vm.FechaAdmision;
                    if (!string.IsNullOrWhiteSpace(cita.Procedimiento))
                        vm.Procedimiento = cita.Procedimiento;

                    if (cita.Empleado != null)
                    {
                        vm.NombreMedicoTratante = cita.Empleado.NombreYApellidos;
                        vm.ColegiadoMedico = cita.Empleado.Colegiado ?? "";
                        vm.FirmaMedicoBase64 = ObtenerFirmaBase64Local(cita.Empleado.FirmaEmpleado, contentRootPath);
                    }

                    AplicarAnestesistaDesdeCita(vm, cita, empleadoRepository, contentRootPath);
                }
            }

            AplicarAnestesistaDesdeNotaOperatoria(vm, anestesistaNotaOperatoria, empleadoRepository, contentRootPath);
            return vm;
        }

        private static bool EsNombreAnestesistaVacio(string nombre)
        {
            return string.IsNullOrWhiteSpace(nombre)
                || nombre == "-"
                || nombre.Equals("Nombre del Anestesista", StringComparison.OrdinalIgnoreCase)
                || nombre.Equals("Anestesiólogo asignado", StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsColegiadoAnestesistaVacio(string colegiado)
        {
            return string.IsNullOrWhiteSpace(colegiado)
                || colegiado == "-"
                || colegiado.Equals("Número de Colegiado", StringComparison.OrdinalIgnoreCase);
        }

        public static void CompletarDatosAnestesista(
            ConsentimientoHospiVM consentimiento,
            AutorizacionAnestesiaPdfVM autorizacion,
            IEmpleado empleadoRepository,
            string contentRootPath,
            string anestesistaNotaOperatoria = null,
            string medicoTratanteNombre = null,
            ICitas citasRepository = null,
            int? citaId = null,
            Hospitalizacion hospitalizacion = null)
        {
            if (consentimiento == null)
                return;

            if (EsNombreAnestesistaVacio(consentimiento.NombreAnestesista))
                consentimiento.NombreAnestesista = null;
            if (EsColegiadoAnestesistaVacio(consentimiento.ColegiadoAnestesista))
                consentimiento.ColegiadoAnestesista = null;

            if (autorizacion != null)
            {
                if (EsNombreAnestesistaVacio(autorizacion.NombreAnestesista))
                    autorizacion.NombreAnestesista = null;
                if (EsColegiadoAnestesistaVacio(autorizacion.ColegiadoAnestesista))
                    autorizacion.ColegiadoAnestesista = null;
            }

            void Aplicar(string nombre, string colegiado, string firma)
            {
                if (!EsNombreAnestesistaVacio(consentimiento.NombreAnestesista))
                    return;
                if (EsNombreAnestesistaVacio(nombre))
                    return;

                consentimiento.NombreAnestesista = nombre.Trim();
                if (!EsColegiadoAnestesistaVacio(colegiado))
                    consentimiento.ColegiadoAnestesista = colegiado.Trim();
                if (!string.IsNullOrEmpty(firma))
                    consentimiento.UrlFirmaAnestesista = firma;
            }

            Aplicar(
                autorizacion?.NombreAnestesista,
                autorizacion?.ColegiadoAnestesista,
                autorizacion?.FirmaAnestesistaBase64);

            if (!EsNombreAnestesistaVacio(consentimiento.NombreAnestesista))
                return;

            if (citasRepository != null)
            {
                var citaResuelta = ResolverCitaParaAnestesista(
                    citasRepository,
                    hospitalizacion,
                    consentimiento,
                    citaId);
                if (citaResuelta != null)
                {
                    var desdeCita = new AutorizacionAnestesiaPdfVM();
                    AplicarAnestesistaDesdeCita(desdeCita, citaResuelta, empleadoRepository, contentRootPath);
                    Aplicar(desdeCita.NombreAnestesista, desdeCita.ColegiadoAnestesista, desdeCita.FirmaAnestesistaBase64);
                }
            }

            if (!EsNombreAnestesistaVacio(consentimiento.NombreAnestesista))
                return;

            if (!string.IsNullOrWhiteSpace(anestesistaNotaOperatoria))
            {
                ResolverMedicoSecundarioCita(
                    anestesistaNotaOperatoria,
                    empleadoRepository,
                    out var nombreNota,
                    out var colNota,
                    out var firmaNota,
                    contentRootPath);
                Aplicar(nombreNota, colNota, firmaNota);
            }

            if (!EsNombreAnestesistaVacio(consentimiento.NombreAnestesista) || empleadoRepository == null)
                return;

            medicoTratanteNombre ??= consentimiento.NombreMedicoTratante;
            var anestEmp = BuscarEmpleadoAnestesista(empleadoRepository, medicoTratanteNombre);

            if (anestEmp != null)
            {
                consentimiento.NombreAnestesista = anestEmp.NombreYApellidos;
                consentimiento.ColegiadoAnestesista = anestEmp.Colegiado ?? "";
                consentimiento.UrlFirmaAnestesista = ObtenerFirmaBase64Local(anestEmp.FirmaEmpleado, contentRootPath);
            }
            else if (!string.IsNullOrWhiteSpace(anestesistaNotaOperatoria)
                && !EsNombreAnestesistaVacio(anestesistaNotaOperatoria))
            {
                Aplicar(anestesistaNotaOperatoria.Trim(), null, null);
            }

            ComplementarFirmaYColegiadoAnestesista(consentimiento, empleadoRepository, contentRootPath);
        }

        private static void ComplementarFirmaYColegiadoAnestesista(
            ConsentimientoHospiVM consentimiento,
            IEmpleado empleadoRepository,
            string contentRootPath)
        {
            if (consentimiento == null
                || EsNombreAnestesistaVacio(consentimiento.NombreAnestesista)
                || empleadoRepository == null)
                return;

            var emp = BuscarEmpleadoPorNombre(consentimiento.NombreAnestesista, empleadoRepository);
            if (emp == null)
                return;

            if (EsColegiadoAnestesistaVacio(consentimiento.ColegiadoAnestesista))
                consentimiento.ColegiadoAnestesista = emp.Colegiado ?? "";
            if (string.IsNullOrEmpty(consentimiento.UrlFirmaAnestesista))
                consentimiento.UrlFirmaAnestesista = ObtenerFirmaBase64Local(emp.FirmaEmpleado, contentRootPath);
        }

        public static (string Nombre, string Colegiado, string FirmaBase64) ResolverAnestesistaParaPdf(
            Hospitalizacion hospitalizacion,
            ConsentimientoHospiVM consentimiento,
            AutorizacionAnestesiaPdfVM autorizacion,
            IEmpleado empleadoRepository,
            ICitas citasRepository,
            string contentRootPath,
            string anestesistaNotaOperatoria = null,
            string medicoTratanteNombre = null,
            int? citaId = null)
        {
            consentimiento ??= new ConsentimientoHospiVM
            {
                NombrePaciente = hospitalizacion?.Paciente?.Nombre ?? "-",
                NombreCompleto = hospitalizacion?.Paciente?.Nombre ?? "-"
            };

            var citaIdResuelto = citaId
                ?? hospitalizacion?.Consultas?.FirstOrDefault()?.Citas?.Id
                ?? hospitalizacion?.Consultas?.FirstOrDefault()?.CitasId;

            autorizacion ??= BuildAutorizacionAnestesia(
                hospitalizacion,
                consentimiento,
                citasRepository,
                empleadoRepository,
                contentRootPath,
                citaIdResuelto,
                anestesistaNotaOperatoria);

            if (string.IsNullOrWhiteSpace(medicoTratanteNombre))
            {
                medicoTratanteNombre = consentimiento.NombreMedicoTratante
                    ?? autorizacion.NombreMedicoTratante
                    ?? ResolverMedicoTratanteHospitalizacion(
                        hospitalizacion, empleadoRepository, contentRootPath, citasRepository, citaIdResuelto).Nombre;
            }

            CompletarDatosAnestesista(
                consentimiento,
                autorizacion,
                empleadoRepository,
                contentRootPath,
                anestesistaNotaOperatoria,
                medicoTratanteNombre,
                citasRepository,
                citaIdResuelto,
                hospitalizacion);

            SincronizarAutorizacionAnestesiaDesdeConsentimiento(autorizacion, consentimiento);

            var nombre = !EsNombreAnestesistaVacio(consentimiento.NombreAnestesista)
                ? consentimiento.NombreAnestesista
                : autorizacion.NombreAnestesista;
            var colegiado = !EsColegiadoAnestesistaVacio(consentimiento.ColegiadoAnestesista)
                ? consentimiento.ColegiadoAnestesista
                : autorizacion.ColegiadoAnestesista;
            var firma = FirstNonEmpty(
                consentimiento.UrlFirmaAnestesista,
                autorizacion.FirmaAnestesistaBase64);

            return (nombre, colegiado, firma);
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value) && value != "-")
                    return value;
            }
            return null;
        }

        private static Citas ResolverCitaParaAnestesista(
            ICitas citasRepository,
            Hospitalizacion hospitalizacion,
            ConsentimientoHospiVM consentimiento,
            int? citaId)
        {
            int citaIdResuelto = citaId ?? 0;
            if (citaIdResuelto == 0 && consentimiento?.CitaId is > 0)
                citaIdResuelto = consentimiento.CitaId.Value;
            if (citaIdResuelto == 0)
            {
                var consulta = hospitalizacion?.Consultas?.FirstOrDefault();
                if (consulta?.CitasId != null)
                    citaIdResuelto = consulta.CitasId.Value;
            }

            return citaIdResuelto > 0 ? citasRepository.GetCita(citaIdResuelto) : null;
        }

        private static Empleado BuscarEmpleadoAnestesista(IEmpleado empleadoRepository, string medicoTratanteNombre)
        {
            var empleados = empleadoRepository.GetListEmpleadoTipoProfesional();
            if (empleados == null || empleados.Count == 0)
                empleados = empleadoRepository.GetList();

            empleados = empleados?
                .Where(e => e != null && !e.Eliminado)
                .ToList() ?? new List<Empleado>();

            var anestEmp = empleados.FirstOrDefault(e =>
                (!string.IsNullOrWhiteSpace(e.Nombre) && e.Nombre.Contains("Anest", StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrWhiteSpace(e.Apellido) && e.Apellido.Contains("Anest", StringComparison.OrdinalIgnoreCase)));

            if (anestEmp != null)
                return anestEmp;

            return empleados.FirstOrDefault(e =>
            {
                var nom = $"{e.Nombre} {e.Apellido}".Trim();
                return !string.IsNullOrWhiteSpace(nom)
                    && !string.Equals(nom, medicoTratanteNombre, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(e.Nombre, medicoTratanteNombre, StringComparison.OrdinalIgnoreCase);
            });
        }

        public static void SincronizarAutorizacionAnestesiaDesdeConsentimiento(
            AutorizacionAnestesiaPdfVM autorizacion,
            ConsentimientoHospiVM consentimiento)
        {
            if (autorizacion == null || consentimiento == null)
                return;

            if (!EsNombreAnestesistaVacio(consentimiento.NombreAnestesista))
            {
                autorizacion.NombreAnestesista = consentimiento.NombreAnestesista;
                autorizacion.ColegiadoAnestesista = consentimiento.ColegiadoAnestesista;
                if (!string.IsNullOrEmpty(consentimiento.UrlFirmaAnestesista))
                    autorizacion.FirmaAnestesistaBase64 = consentimiento.UrlFirmaAnestesista;
            }
            else if (EsNombreAnestesistaVacio(autorizacion.NombreAnestesista))
            {
                autorizacion.NombreAnestesista = null;
                autorizacion.ColegiadoAnestesista = null;
            }

            if (string.IsNullOrWhiteSpace(autorizacion.NombreMedicoTratante))
                autorizacion.NombreMedicoTratante = consentimiento.NombreMedicoTratante;
            if (string.IsNullOrWhiteSpace(autorizacion.ColegiadoMedico))
                autorizacion.ColegiadoMedico = consentimiento.ColegiadoMedico;
            if (string.IsNullOrEmpty(autorizacion.FirmaMedicoBase64))
                autorizacion.FirmaMedicoBase64 = consentimiento.UrlFirmaMedico ?? consentimiento.FirmaMedicoBase64;
            if (string.IsNullOrWhiteSpace(autorizacion.FechaAdmision))
                autorizacion.FechaAdmision = consentimiento.FechaAdmision;
            if (string.IsNullOrWhiteSpace(autorizacion.Procedimiento))
                autorizacion.Procedimiento = consentimiento.ProcedimientoProgramado;
        }

        private static void AplicarAnestesistaDesdeCita(
            AutorizacionAnestesiaPdfVM vm,
            Citas cita,
            IEmpleado empleadoRepository,
            string contentRootPath)
        {
            if (cita == null || !EsNombreAnestesistaVacio(vm.NombreAnestesista))
                return;

            if (cita.AnestesistaId is > 0)
            {
                var anestesista = empleadoRepository.Get(cita.AnestesistaId.Value, false);
                if (anestesista != null)
                {
                    vm.NombreAnestesista = anestesista.NombreYApellidos;
                    vm.ColegiadoAnestesista = anestesista.Colegiado ?? "";
                    vm.FirmaAnestesistaBase64 = ObtenerFirmaBase64Local(anestesista.FirmaEmpleado, contentRootPath);
                    return;
                }
            }

            if (int.TryParse(cita.Anestesista, out int idAnestesista))
            {
                var anestesista = empleadoRepository.Get(idAnestesista, false);
                if (anestesista != null)
                {
                    vm.NombreAnestesista = anestesista.NombreYApellidos;
                    vm.ColegiadoAnestesista = anestesista.Colegiado ?? "";
                    vm.FirmaAnestesistaBase64 = ObtenerFirmaBase64Local(anestesista.FirmaEmpleado, contentRootPath);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(cita.Anestesista) && !EsNombreAnestesistaVacio(cita.Anestesista))
            {
                ResolverMedicoSecundarioCita(
                    cita.Anestesista,
                    empleadoRepository,
                    out var nombreAnest,
                    out var colAnest,
                    out var firmaAnest,
                    contentRootPath);
                if (!EsNombreAnestesistaVacio(nombreAnest))
                {
                    vm.NombreAnestesista = nombreAnest;
                    vm.ColegiadoAnestesista = colAnest;
                    vm.FirmaAnestesistaBase64 = firmaAnest;
                }
            }
        }

        private static void AplicarAnestesistaDesdeNotaOperatoria(
            AutorizacionAnestesiaPdfVM vm,
            string anestesistaNotaOperatoria,
            IEmpleado empleadoRepository,
            string contentRootPath)
        {
            if (!EsNombreAnestesistaVacio(vm.NombreAnestesista))
                return;

            if (string.IsNullOrWhiteSpace(anestesistaNotaOperatoria)
                || anestesistaNotaOperatoria.Equals("Anestesiólogo asignado", StringComparison.OrdinalIgnoreCase))
                return;

            ResolverMedicoSecundarioCita(
                anestesistaNotaOperatoria,
                empleadoRepository,
                out var nombreAnest,
                out var colAnest,
                out var firmaAnest,
                contentRootPath);

            if (!EsNombreAnestesistaVacio(nombreAnest))
            {
                vm.NombreAnestesista = nombreAnest;
                vm.ColegiadoAnestesista = colAnest;
                vm.FirmaAnestesistaBase64 = firmaAnest;
            }
        }

        public static string ResolverNombreUsuarioToma(
            string usuarioTomaId,
            IUser userRepository,
            IEmpleado empleadoRepository)
        {
            if (string.IsNullOrWhiteSpace(usuarioTomaId))
                return "-";

            return ResolverNombreProfesionalTexto(usuarioTomaId, userRepository, empleadoRepository);
        }

        public static List<SignosVitalesHospPdfRow> MapSignosVitalesHosp(
            IEnumerable<ExamenFisicoHosp> examenes,
            IUser userRepository = null,
            IEmpleado empleadoRepository = null,
            string contentRootPath = null,
            string firmaMedicoTratanteFallback = null,
            IReadOnlyDictionary<string, User> autorizadores = null)
        {
            return examenes?
                .OrderBy(e => e.FechaHora)
                .Select(e =>
                {
                    var valores = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var d in e.ExamenesFisicosHospDatos ?? Enumerable.Empty<ExamenFisicoHospDato>())
                    {
                        var nombre = d.DatoExamenFisicoHosp?.NombreDato ?? "Dato";
                        valores[nombre] = d.Valor ?? "-";
                    }

                    var profesional = ResolverNombreUsuarioToma(e.UsuarioToma, userRepository, empleadoRepository);
                    string autorizadoPor = null;
                    string firmaBase64 = null;

                    if (e.Autorizado && !string.IsNullOrWhiteSpace(e.UsuarioAutoriza))
                    {
                        User autorizador = null;
                        autorizadores?.TryGetValue(e.UsuarioAutoriza, out autorizador);
                        User profesionalUser = null;
                        if (!string.IsNullOrWhiteSpace(e.UsuarioToma))
                            profesionalUser = userRepository?.GetbyId(e.UsuarioToma);

                        var firmante = ResolverFirmanteClinico(
                            e.Autorizado,
                            e.UsuarioAutoriza,
                            profesionalUser,
                            autorizador == null
                                ? null
                                : new Dictionary<string, User> { { autorizador.Id, autorizador } },
                            empleadoRepository,
                            firmaMedicoTratanteFallback,
                            contentRootPath,
                            profesional);
                        autorizadoPor = firmante.AutorizadoPor;
                        firmaBase64 = firmante.FirmaBase64;
                    }

                    return new SignosVitalesHospPdfRow
                    {
                        FechaHora = e.FechaHora,
                        Profesional = profesional,
                        Observaciones = e.Observaciones ?? "",
                        Autorizado = e.Autorizado,
                        AutorizadoPor = autorizadoPor,
                        FirmaBase64 = firmaBase64,
                        Valores = valores
                    };
                })
                .ToList() ?? new List<SignosVitalesHospPdfRow>();
        }

        public static NotaMedica2ViewModel ResolverNotaEvolucionPorTipo(
            IList<NotaMedica2ViewModel> notas,
            string tipo,
            int fallbackIndex)
        {
            if (notas == null || notas.Count == 0)
                return null;

            var porTipo = notas.FirstOrDefault(n =>
                string.Equals(n.TipoNota, tipo, StringComparison.OrdinalIgnoreCase));
            if (porTipo != null)
                return porTipo;

            return fallbackIndex >= 0 && notas.Count > fallbackIndex
                ? notas[fallbackIndex]
                : null;
        }

        public static NotaEnfermeriaPdfViewModel ResolverNotaEnfermeriaPorTipo(
            IList<NotaEnfermeriaPdfViewModel> notas,
            string tipo,
            int fallbackIndex)
        {
            if (notas == null || notas.Count == 0)
                return null;

            var porTipo = notas.FirstOrDefault(n =>
                string.Equals(n.TipoNota, tipo, StringComparison.OrdinalIgnoreCase));
            if (porTipo != null)
                return porTipo;

            return fallbackIndex >= 0 && notas.Count > fallbackIndex
                ? notas[fallbackIndex]
                : null;
        }

        public static List<NotaMedica2ViewModel> ResolverNotasEgresoEvolucion(
            IList<NotaMedica2ViewModel> notas)
        {
            if (notas == null || notas.Count == 0)
                return new List<NotaMedica2ViewModel>();

            var porTipo = notas
                .Where(n => string.Equals(n.TipoNota, TipoNotaClinica.Egreso, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (porTipo.Any())
                return porTipo;

            return notas.Count > 3 ? notas.Skip(3).ToList() : new List<NotaMedica2ViewModel>();
        }

        public static List<NotaEnfermeriaPdfViewModel> ResolverNotasEgresoEnfermeria(
            IList<NotaEnfermeriaPdfViewModel> notas)
        {
            if (notas == null || notas.Count == 0)
                return new List<NotaEnfermeriaPdfViewModel>();

            var porTipo = notas
                .Where(n => string.Equals(n.TipoNota, TipoNotaClinica.Egreso, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (porTipo.Any())
                return porTipo;

            return notas.Count > 3 ? notas.Skip(3).ToList() : new List<NotaEnfermeriaPdfViewModel>();
        }

        public static void EnriquecerConsentimientoFirmas(ConsentimientoHospiVM consentimiento, string contentRootPath)
        {
            if (consentimiento == null)
                return;

            consentimiento.FirmaPacienteBase64 = ResolverFirmaDesdeCampo(consentimiento.URLFirmaPaciente, contentRootPath);
            consentimiento.FirmaResponsableBase64 = ResolverFirmaDesdeCampo(consentimiento.URLFirmaResponsable, contentRootPath);
            consentimiento.FirmaNotariaBase64 = ResolverFirmaDesdeCampo(consentimiento.URLFirmaNotaria, contentRootPath);
            consentimiento.FirmaRepresentanteBase64 = ResolverFirmaDesdeCampo(consentimiento.URLFirmaRepresentanteNaranjo, contentRootPath);
            if (string.IsNullOrEmpty(consentimiento.FirmaMedicoBase64))
                consentimiento.FirmaMedicoBase64 = ResolverFirmaDesdeCampo(consentimiento.UrlFirmaMedico, contentRootPath);

            if (!string.IsNullOrEmpty(consentimiento.FirmaPacienteBase64))
                consentimiento.URLFirmaPaciente = consentimiento.FirmaPacienteBase64;
            if (!string.IsNullOrEmpty(consentimiento.FirmaResponsableBase64))
                consentimiento.URLFirmaResponsable = consentimiento.FirmaResponsableBase64;
            if (!string.IsNullOrEmpty(consentimiento.FirmaMedicoBase64))
            {
                consentimiento.UrlFirmaMedico = consentimiento.FirmaMedicoBase64;
            }
        }

        private static string ResolverFirmaDesdeCampo(string valor, string contentRootPath)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return "";

            if (valor.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return valor;

            return ObtenerFirmaBase64Local(valor, contentRootPath);
        }

        public static (NotaMedica2ViewModel Ingreso, NotaMedica2ViewModel Traslado, NotaMedica2ViewModel Recepcion, List<NotaMedica2ViewModel> Egreso)
            AsignarNotasEvolucionExpediente(IList<NotaMedica2ViewModel> notas)
        {
            var lista = notas?.Where(n => n != null).ToList() ?? new List<NotaMedica2ViewModel>();
            var usados = new HashSet<int>();

            NotaMedica2ViewModel Tomar(string tipo, int fallbackIndex)
            {
                var porTipo = lista.FirstOrDefault(n =>
                    n.Id > 0 && !usados.Contains(n.Id) &&
                    string.Equals(n.TipoNota, tipo, StringComparison.OrdinalIgnoreCase));
                if (porTipo != null)
                {
                    usados.Add(porTipo.Id);
                    return porTipo;
                }

                var libre = lista.Where(n => n.Id <= 0 || !usados.Contains(n.Id)).ToList();
                if (fallbackIndex >= 0 && libre.Count > fallbackIndex)
                {
                    var elegida = libre[fallbackIndex];
                    if (elegida.Id > 0)
                        usados.Add(elegida.Id);
                    return elegida;
                }

                return null;
            }

            var egresoPorTipo = lista
                .Where(n => n.Id > 0 && !usados.Contains(n.Id) &&
                            string.Equals(n.TipoNota, TipoNotaClinica.Egreso, StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var n in egresoPorTipo)
                usados.Add(n.Id);

            var egreso = egresoPorTipo.Any()
                ? egresoPorTipo
                : lista.Where(n => n.Id > 0 && !usados.Contains(n.Id)).Skip(Math.Max(0, lista.Count - 1)).ToList();

            return (
                Tomar(TipoNotaClinica.Ingreso, 0),
                Tomar(TipoNotaClinica.Traslado, 1),
                Tomar(TipoNotaClinica.Recepcion, 2),
                egreso);
        }

        public static (NotaEnfermeriaPdfViewModel Ingreso, NotaEnfermeriaPdfViewModel Traslado, NotaEnfermeriaPdfViewModel Recepcion, List<NotaEnfermeriaPdfViewModel> Egreso)
            AsignarNotasEnfermeriaExpediente(IList<NotaEnfermeriaPdfViewModel> notas)
        {
            var lista = notas?.Where(n => n != null).ToList() ?? new List<NotaEnfermeriaPdfViewModel>();
            var usados = new HashSet<int>();

            NotaEnfermeriaPdfViewModel Tomar(string tipo, int fallbackIndex)
            {
                var porTipo = lista.FirstOrDefault(n =>
                    n.Id > 0 && !usados.Contains(n.Id) &&
                    string.Equals(n.TipoNota, tipo, StringComparison.OrdinalIgnoreCase));
                if (porTipo != null)
                {
                    usados.Add(porTipo.Id);
                    return porTipo;
                }

                var libre = lista.Where(n => n.Id <= 0 || !usados.Contains(n.Id)).ToList();
                if (fallbackIndex >= 0 && libre.Count > fallbackIndex)
                {
                    var elegida = libre[fallbackIndex];
                    if (elegida.Id > 0)
                        usados.Add(elegida.Id);
                    return elegida;
                }

                return null;
            }

            var egresoPorTipo = lista
                .Where(n => n.Id > 0 && !usados.Contains(n.Id) &&
                            string.Equals(n.TipoNota, TipoNotaClinica.Egreso, StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var n in egresoPorTipo)
                usados.Add(n.Id);

            var egreso = egresoPorTipo.Any()
                ? egresoPorTipo
                : lista.Where(n => n.Id > 0 && !usados.Contains(n.Id)).Skip(Math.Max(0, lista.Count - 1)).ToList();

            return (
                Tomar(TipoNotaClinica.Ingreso, 0),
                Tomar(TipoNotaClinica.Traslado, 1),
                Tomar(TipoNotaClinica.Recepcion, 2),
                egreso);
        }

        public static void CompletarConsentimientoPdfVm(
            ConsentimientoHospiVM consentimiento,
            Hospitalizacion hospitalizacion,
            ICitas citasRepository,
            IEmpleado empleadoRepository,
            string contentRootPath,
            int? citaId,
            IEnumerable<MedicamentoNoControladoPdfVM> medicamentos,
            string anestesistaNotaOperatoria = null)
        {
            if (consentimiento == null)
                return;

            if (string.IsNullOrWhiteSpace(consentimiento.NombrePaciente))
                consentimiento.NombrePaciente = consentimiento.NombreCompleto
                    ?? hospitalizacion?.Paciente?.Nombre;

            EnriquecerConsentimientoFirmas(consentimiento, contentRootPath);

            void ResolverMedicoSecundario(string datoCita, out string nombre, out string colegiado)
            {
                ResolverMedicoSecundarioCita(datoCita, empleadoRepository, out nombre, out colegiado, out _, contentRootPath);
            }

            int citaIdResuelto = citaId ?? 0;
            if (citaIdResuelto == 0 && consentimiento.CitaId is > 0)
                citaIdResuelto = consentimiento.CitaId.Value;
            if (citaIdResuelto == 0)
            {
                var consulta = hospitalizacion?.Consultas?.FirstOrDefault();
                if (consulta?.CitasId != null)
                    citaIdResuelto = consulta.CitasId.Value;
            }

            if (citaIdResuelto > 0)
            {
                var cita = citasRepository.GetCita(citaIdResuelto);
                if (cita != null)
                {
                    consentimiento.FechaAdmision = cita.FechaInicio?.ToString("dd/MM/yyyy")
                        ?? consentimiento.FechaAdmision
                        ?? consentimiento.HoraIngreso;
                    if (!string.IsNullOrWhiteSpace(cita.Procedimiento))
                        consentimiento.ProcedimientoProgramado = cita.Procedimiento;

                    if (cita.Empleado != null)
                    {
                        consentimiento.NombreMedicoTratante = cita.Empleado.NombreYApellidos;
                        consentimiento.ColegiadoMedico = cita.Empleado.Colegiado ?? "";
                        consentimiento.EspecialidadMedico = cita.EspecialidadText != "N/A"
                            ? cita.EspecialidadText
                            : consentimiento.EspecialidadMedico;
                        var firmaMed = ObtenerFirmaBase64Local(cita.Empleado.FirmaEmpleado, contentRootPath);
                        consentimiento.UrlFirmaMedico = firmaMed;
                        consentimiento.FirmaMedicoBase64 = firmaMed;
                    }

                    ResolverMedicoSecundario(cita.PrimerAyudante, out var n1, out var c1);
                    consentimiento.NombrePrimerAyudante = n1;
                    consentimiento.ColegiadoPrimerAyudante = c1;

                    ResolverMedicoSecundario(cita.SegundoAyudante, out var n2, out var c2);
                    consentimiento.NombreSegundoAyudante = n2;
                    consentimiento.ColegiadoSegundoAyudante = c2;

                    ResolverMedicoSecundario(cita.Anestesista, out var nAnest, out var cAnest);
                    if (!EsNombreAnestesistaVacio(nAnest))
                    {
                        consentimiento.NombreAnestesista = nAnest;
                        consentimiento.ColegiadoAnestesista = cAnest;
                    }

                    if (cita.AnestesistaId is > 0)
                    {
                        var anestesista = empleadoRepository.Get(cita.AnestesistaId.Value, false);
                        if (anestesista != null)
                        {
                            consentimiento.NombreAnestesista = anestesista.NombreYApellidos;
                            consentimiento.ColegiadoAnestesista = anestesista.Colegiado ?? "";
                            consentimiento.UrlFirmaAnestesista = ObtenerFirmaBase64Local(anestesista.FirmaEmpleado, contentRootPath);
                        }
                    }
                    else if (int.TryParse(cita.Anestesista, out int idAnestesista))
                    {
                        var anestesista = empleadoRepository.Get(idAnestesista, false);
                        if (anestesista != null)
                        {
                            consentimiento.NombreAnestesista = anestesista.NombreYApellidos;
                            consentimiento.ColegiadoAnestesista = anestesista.Colegiado ?? "";
                            consentimiento.UrlFirmaAnestesista = ObtenerFirmaBase64Local(anestesista.FirmaEmpleado, contentRootPath);
                        }
                    }
                }
            }

            var anest = BuildAutorizacionAnestesia(
                hospitalizacion,
                consentimiento,
                citasRepository,
                empleadoRepository,
                contentRootPath,
                citaId,
                anestesistaNotaOperatoria);

            if (string.IsNullOrWhiteSpace(consentimiento.NombreMedicoTratante))
                consentimiento.NombreMedicoTratante = anest.NombreMedicoTratante;
            if (string.IsNullOrWhiteSpace(consentimiento.ColegiadoMedico))
                consentimiento.ColegiadoMedico = anest.ColegiadoMedico;
            if (string.IsNullOrEmpty(consentimiento.UrlFirmaMedico))
                consentimiento.UrlFirmaMedico = anest.FirmaMedicoBase64;
            if (string.IsNullOrEmpty(consentimiento.FirmaMedicoBase64))
                consentimiento.FirmaMedicoBase64 = consentimiento.UrlFirmaMedico;

            if (EsNombreAnestesistaVacio(consentimiento.NombreAnestesista) && !EsNombreAnestesistaVacio(anest.NombreAnestesista))
                consentimiento.NombreAnestesista = anest.NombreAnestesista;
            if (EsColegiadoAnestesistaVacio(consentimiento.ColegiadoAnestesista) && !EsColegiadoAnestesistaVacio(anest.ColegiadoAnestesista))
                consentimiento.ColegiadoAnestesista = anest.ColegiadoAnestesista;
            if (string.IsNullOrEmpty(consentimiento.UrlFirmaAnestesista))
                consentimiento.UrlFirmaAnestesista = anest.FirmaAnestesistaBase64;

            CompletarDatosAnestesista(
                consentimiento,
                anest,
                empleadoRepository,
                contentRootPath,
                anestesistaNotaOperatoria,
                consentimiento.NombreMedicoTratante,
                citasRepository,
                citaId,
                hospitalizacion);

            CompletarFirmasConsentimientoSerSalud(
                consentimiento,
                consentimiento.FirmaMedicoBase64 ?? consentimiento.UrlFirmaMedico,
                contentRootPath);

            if (string.IsNullOrWhiteSpace(consentimiento.FechaAdmision))
                consentimiento.FechaAdmision = anest.FechaAdmision;
            if (string.IsNullOrWhiteSpace(consentimiento.ProcedimientoProgramado))
                consentimiento.ProcedimientoProgramado = anest.Procedimiento;

            consentimiento.MedicamentosNoControlados = medicamentos?.ToList() ?? new List<MedicamentoNoControladoPdfVM>();
            SincronizarUrlFirmasConsentimiento(consentimiento, contentRootPath);
            ExpedienteClinicalPdfEnricher.EnriquecerConsentimientoRadiologia(consentimiento, hospitalizacion);
        }

        public static string ResolverVistaConsentimientoPdf(string cliente)
        {
            if (EsClienteSerSalud(cliente))
                return "CrearPDF/ConsentimientoHospiPDF_SS";
            if (string.Equals(cliente, "HSC", StringComparison.OrdinalIgnoreCase))
                return "CrearPDF/ConsentimientoHospiPDF_HSC";
            return "CrearPDF/ConsentimientoHospiPDF_G";
        }

        public static void SincronizarUrlFirmasConsentimiento(ConsentimientoHospiVM consentimiento, string contentRootPath)
        {
            if (consentimiento == null)
                return;

            string Resolver(string url, string b64)
            {
                if (!string.IsNullOrEmpty(b64))
                    return b64;
                if (string.IsNullOrEmpty(url))
                    return "";
                if (url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                    return url;
                return ObtenerFirmaBase64Local(url, contentRootPath);
            }

            consentimiento.URLFirmaPaciente = Resolver(consentimiento.URLFirmaPaciente, consentimiento.FirmaPacienteBase64);
            consentimiento.URLFirmaResponsable = Resolver(consentimiento.URLFirmaResponsable, consentimiento.FirmaResponsableBase64);
            consentimiento.URLFirmaNotaria = Resolver(consentimiento.URLFirmaNotaria, consentimiento.FirmaNotariaBase64);
            consentimiento.URLFirmaRepresentanteNaranjo = Resolver(consentimiento.URLFirmaRepresentanteNaranjo, consentimiento.FirmaRepresentanteBase64);

            if (string.IsNullOrEmpty(consentimiento.UrlFirmaMedico))
                consentimiento.UrlFirmaMedico = consentimiento.FirmaMedicoBase64 ?? "";
            if (string.IsNullOrEmpty(consentimiento.FirmaMedicoBase64))
                consentimiento.FirmaMedicoBase64 = consentimiento.UrlFirmaMedico;
        }
    }

}
