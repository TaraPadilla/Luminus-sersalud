using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Service.IService;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace farmamest.Controllers
{
    public class HospitalizacionExamenPdfController : Controller
    {
        private readonly IHospitalizacionExamenPdfService _hospitalizacionExamenPdfService;
        private readonly UserManager<User> _userManager;
        private readonly IUser _userRepository;
        private readonly IEmpleado _empleadoRepository;
        private readonly IWebHostEnvironment _environment;

        public HospitalizacionExamenPdfController(
            IHospitalizacionExamenPdfService hospitalizacionExamenPdfService,
            UserManager<User> userManager,
            IUser userRepository,
            IEmpleado empleadoRepository,
            IWebHostEnvironment environment)
        {
            _hospitalizacionExamenPdfService = hospitalizacionExamenPdfService;
            _userManager = userManager;
            _userRepository = userRepository;
            _empleadoRepository = empleadoRepository;
            _environment = environment;
        }

        // ✅ Obtener el ID del usuario autenticado
        [HttpGet]
        public ActionResult<string> GetCurrentUserId()
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                if (string.IsNullOrEmpty(userId))
                    return NotFound(new { Exitoso = false, Mensaje = "Usuario no autenticado." });

                return Ok(new { Exitoso = true, UsuarioId = userId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener ID del usuario. {ex.Message}" });
            }
        }

        // ✅ Obtener el Nombre y Apellido de un usuario
        [HttpGet]
        public ActionResult<string> GetUserFullNameByEmployeeId(string userId)
        {
            try
            {
                var user = _userRepository.GetbyId(userId);
                if (user == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Usuario no encontrado." });

                var empleadoId = user.EmpleadoId;
                if (empleadoId == null)
                    return Ok(new { Exitoso = true, NombreCompleto = "Admin" });

                var empleado = _empleadoRepository.Get((int)empleadoId);
                if (empleado == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Empleado no encontrado." });

                var nombreCompleto = empleado.NombreYApellidos;
                return Ok(new { Exitoso = true, NombreCompleto = nombreCompleto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener el nombre del usuario. {ex.Message}" });
            }
        }

        // ✅ 1. Obtener todos los registros
        [HttpGet]
        public ActionResult<List<HospitalizacionExamenPdf>> GetAll()
        {
            try
            {
                return Ok(_hospitalizacionExamenPdfService.GetAll());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener los registros. {ex.Message}" });
            }
        }

        // ✅ 2. Obtener un registro por ID
        [HttpGet]
        public ActionResult<HospitalizacionExamenPdf> GetById(int id)
        {
            try
            {
                var pdf = _hospitalizacionExamenPdfService.GetById(id);
                if (pdf == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Registro no encontrado." });

                return Ok(pdf);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener el registro. {ex.Message}" });
            }
        }

        // ✅ 3. Crear un nuevo registro
        [HttpPost]
        public ActionResult Add([FromBody] HospitalizacionExamenPdf entity)
        {
            try
            {
                if (entity == null)
                    return BadRequest(new { Exitoso = false, Mensaje = "Datos inválidos." });

                // ✅ Obtener el ID del usuario autenticado
                var userId = _userManager.GetUserId(HttpContext.User);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { Exitoso = false, Mensaje = "Usuario no autenticado." });

                // ✅ Asignar el UserId al registro
                entity.UserId = userId;
                entity.FechaCreacion = DateTime.Now;

                // ✅ Guardar en la base de datos
                _hospitalizacionExamenPdfService.Add(entity);

                return Ok(new { Exitoso = true, Mensaje = "Registro creado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al crear el registro. {ex.Message}" });
            }
        }

        // ✅ 4. Actualizar un registro existente
        [HttpPut]
        public ActionResult Update(int id, [FromBody] HospitalizacionExamenPdf entity)
        {
            try
            {
                if (entity == null || id != entity.Id)
                    return BadRequest(new { Exitoso = false, Mensaje = "Datos inválidos." });

                var registroExistente = _hospitalizacionExamenPdfService.GetById(id);
                if (registroExistente == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Registro no encontrado." });

                _hospitalizacionExamenPdfService.Update(entity);
                return Ok(new { Exitoso = true, Mensaje = "Registro actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al actualizar el registro. {ex.Message}" });
            }
        }

        // ✅ 5. Marcar un registro como eliminado
        [HttpPut]
        public ActionResult MarkAsDeleted(int id)
        {
            try
            {
                var registro = _hospitalizacionExamenPdfService.GetById(id);
                if (registro == null)
                    return NotFound(new { Exitoso = false, Mensaje = "Registro no encontrado." });

                _hospitalizacionExamenPdfService.MarkAsDeleted(id);
                return Ok(new { Exitoso = true, Mensaje = "Registro eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al eliminar el registro. {ex.Message}" });
            }
        }

        // ✅ 6. Obtener registros por Hospitalización y Examen
        [HttpGet]
        public ActionResult<List<HospitalizacionExamenPdf>> GetByHospitalizacionAndExamen(int hospitalizacionId, int examenId)
        {
            try
            {
                var data = _hospitalizacionExamenPdfService.GetByHospitalizacionAndExamen(hospitalizacionId, examenId);
                foreach (var registro in data)
                {
                    // Obtener el usuario usando el UserId que originalmente se guardó en el registro
                    var user = _userRepository.GetbyId(registro.UserId);
                    string nombreCompleto = "Admin"; // Valor por defecto en caso de que no haya empleado

                    if (user != null && user.EmpleadoId != null)
                    {
                        var empleado = _empleadoRepository.Get((int)user.EmpleadoId);
                        if (empleado != null)
                        {
                            nombreCompleto = empleado.NombreYApellidos;
                        }
                    }
                    // Reemplazar el valor de UserId por el nombre completo
                    registro.UserId = nombreCompleto;
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener registros. {ex.Message}" });
            }
        }

        // ✅ 7. Obtener registros por HospitalizaciónId
        [HttpGet]
        public ActionResult<List<HospitalizacionExamenPdf>> GetByHospitalizacionId(int hospitalizacionId)
        {
            try
            {
                // Obtener la lista de registros para la hospitalización solicitada
                var data = _hospitalizacionExamenPdfService.GetByHospitalizacionId(hospitalizacionId);
                
                // Recorrer cada registro y actualizar la propiedad UserId con el nombre completo del usuario
                foreach (var registro in data)
                {
                    // Obtener el usuario a partir del UserId almacenado en el registro
                    var user = _userRepository.GetbyId(registro.UserId);
                    string nombreCompleto = "Admin"; // Valor por defecto, en caso de que no se encuentre empleado

                    if (user != null && user.EmpleadoId != null)
                    {
                        var empleado = _empleadoRepository.Get((int)user.EmpleadoId);
                        if (empleado != null)
                        {
                            nombreCompleto = empleado.NombreYApellidos;
                        }
                    }
                    // Actualizar la propiedad UserId con el nombre completo
                    registro.UserId = nombreCompleto;
                }
                
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al obtener registros. {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadPdf(IFormFile file, int hospitalizacionId, int examenId, bool isPrincipalPdf = true, string nombreExamen = "PDF Principal")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { Exitoso = false, Mensaje = "Archivo inválido." });

                var extension = Path.GetExtension(file.FileName);
                if (extension.ToLower() != ".pdf")
                    return BadRequest(new { Exitoso = false, Mensaje = "Solo se permiten archivos PDF." });

                // Normalizar el nombre del archivo (sin extensión)
                string originalName = Path.GetFileNameWithoutExtension(file.FileName);
                string normalizedFileName = NormalizeFileName(originalName);

                // Generar identificador alfanumérico de 10 caracteres
                string randomId = GenerateRandomAlphanumeric(10);

                // Concatenar para formar el nuevo nombre de archivo
                string newFileName = $"{normalizedFileName}_{randomId}{extension}";

                // Obtener la ruta física de la carpeta PDFs_Examenes
                string pdfPath = Path.Combine(_environment.WebRootPath, "PDFs_Examenes");
                if (!Directory.Exists(pdfPath))
                {
                    Directory.CreateDirectory(pdfPath);
                }
                string fullPath = Path.Combine(pdfPath, newFileName);

                // Guardar el archivo de forma asíncrona
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Construir la URL relativa del PDF
                string pdfUrl = $"/PDFs_Examenes/{newFileName}";

                // Crear el registro en la base de datos
                HospitalizacionExamenPdf registro = new HospitalizacionExamenPdf
                {
                    PdfUrl = pdfUrl,
                    IsPrincipalPdf = isPrincipalPdf,
                    HospitalizacionId = hospitalizacionId,
                    ExamenId = examenId,
                    FechaCreacion = DateTime.Now,
                    UserId = _userManager.GetUserId(HttpContext.User),
                    NombreExamen = nombreExamen  // Se asigna el nuevo valor recibido
                };

                _hospitalizacionExamenPdfService.Add(registro);

                return Ok(new { Exitoso = true, Mensaje = "Archivo subido exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Exitoso = false, Mensaje = $"Error al subir el archivo. {ex.Message}" });
            }
        }

        // Método auxiliar para normalizar el nombre del archivo
        private string NormalizeFileName(string fileName)
        {
            // Eliminar diacríticos (acentos) y caracteres especiales, y quitar espacios
            string normalized = fileName.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark && !char.IsWhiteSpace(c))
                {
                    if (char.IsLetterOrDigit(c))
                        sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // Método auxiliar para generar un identificador alfanumérico de 10 caracteres
        private string GenerateRandomAlphanumeric(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        public ActionResult DeletePdf([FromForm] string pdfUrl, [FromForm] int id)
        {
            try
            {
                // Construir la ruta completa al archivo, asumiendo que pdfUrl es relativo y comienza con "/"
                string filePath = Path.Combine(_environment.WebRootPath, pdfUrl.TrimStart('/'));

                // Validar si el archivo existe
                if (!System.IO.File.Exists(filePath))
                {
                    return Ok(new { exitoso = false, mensaje = "El archivo no existe." });
                }

                // Intentar eliminar el archivo
                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch (Exception deleteEx)
                {
                    return Ok(new { exitoso = false, mensaje = $"No se pudo eliminar el archivo: {deleteEx.Message}" });
                }

                // Marcar el registro como eliminado en la base de datos
                _hospitalizacionExamenPdfService.MarkAsDeleted(id);

                return Ok(new { exitoso = true, mensaje = "Registro eliminado y archivo borrado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exitoso = false, mensaje = $"Error al eliminar el registro: {ex.Message}" });
            }
        }
    }
}
