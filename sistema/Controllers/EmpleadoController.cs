using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Database.Shared;
using Microsoft.Extensions.Logging;
using sistema.Models;
using Database.Shared.IRepository;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Database.Shared.Models;
using System.Linq;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace sistema.Controllers
{
    [Authorize]
    public class EmpleadoController : Controller
    {
        private readonly IEmpleado _empleadoRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly IEspecialidad _especialidadRepository = null;

        // ✅ NUEVO: Context para leer catálogos org (DepartamentosOrg/UnidadesOrg/SeccionesOrg)
        private readonly Context _context;

        private readonly ILogger<EmpleadoController> _logger;

        public EmpleadoController(IEmpleado empleadoRepository,
                                  ISucursal sucursalRepository,
                                  IEspecialidad especialidadRepository,
                                  Context context,
                                  ILogger<EmpleadoController> logger)
        {
            _empleadoRepository = empleadoRepository;
            _sucursalRepository = sucursalRepository;
            _especialidadRepository = especialidadRepository;

            _context = context;

            _logger = logger;
        }

        // public IActionResult Lista(string sortOrder, string buscar, string currentFilter, int? pageNumber, string tipoEmpleado)
        // {
        //     ViewData["CurrentSort"] = sortOrder;
        //     ViewData["ApellidoSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Apellido_desc" : "";
        //     ViewData["NombreSortParam"] = string.IsNullOrEmpty(sortOrder) ? "Nombre_desc" : "";
        //
        //     if (buscar != null)
        //     {
        //         pageNumber = 1;
        //     }
        //     else
        //     {
        //         buscar = currentFilter;
        //     }
        //
        //     ViewData["CurrentFilter"] = buscar;
        //     ViewData["TipoEmpleado"] = tipoEmpleado;
        //
        //     if (tipoEmpleado == "Medico")
        //     {
        //         var lista = _empleadoRepository.PaginacionEmpleadosMedicos(sortOrder, buscar, pageNumber, 10, tipoEmpleado);
        //         return View(lista);
        //     }
        //     else
        //     {
        //         var lista = _empleadoRepository.PaginacionEmpleados(sortOrder, buscar, pageNumber, 10, tipoEmpleado);
        //         return View(lista);
        //     }
        // }

        public IActionResult Lista(string sortOrder, string buscar, string currentFilter, int? pageNumber, string tipoEmpleado)
        {
            // Ya NO se usa paginación manual ni sorting manual aquí.
            ViewData["TipoEmpleado"] = tipoEmpleado;
            return View();
        }

        // ==============================
        // ✅ NUEVO: Catálogos Org (AJAX)
        // ==============================

        [HttpGet]
        public IActionResult DepartamentosOrg()
        {
            try
            {
                var data = _context.DepartamentosOrg
                    .AsNoTracking()
                    .Where(d => !d.Eliminada)
                    .OrderBy(d => d.Nombre)
                    .Select(d => new
                    {
                        id = d.Id,
                        nombre = d.Nombre
                    })
                    .ToList();

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en DepartamentosOrg: {0}", ex.Message);
                return Json(new { success = false, message = "Ocurrió un error al cargar departamentos." });
            }
        }

        [HttpGet]
        public IActionResult UnidadesOrg(int? departamentoOrgId)
        {
            try
            {
                if (departamentoOrgId == null || departamentoOrgId <= 0)
                {
                    return Json(new { success = true, data = Array.Empty<object>() });
                }

                var data = _context.UnidadesOrg
                    .AsNoTracking()
                    .Where(u => !u.Eliminada && u.DepartamentoOrgId == departamentoOrgId.Value)
                    .OrderBy(u => u.Nombre)
                    .Select(u => new
                    {
                        id = u.Id,
                        nombre = u.Nombre,
                        departamentoOrgId = u.DepartamentoOrgId
                    })
                    .ToList();

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en UnidadesOrg: {0}", ex.Message);
                return Json(new { success = false, message = "Ocurrió un error al cargar unidades." });
            }
        }

        [HttpGet]
        public IActionResult SeccionesOrg(int? unidadOrgId)
        {
            try
            {
                if (unidadOrgId == null || unidadOrgId <= 0)
                {
                    return Json(new { success = true, data = Array.Empty<object>() });
                }

                var data = _context.SeccionesOrg
                    .AsNoTracking()
                    .Where(s => !s.Eliminada && s.UnidadOrgId == unidadOrgId.Value)
                    .OrderBy(s => s.Nombre)
                    .Select(s => new
                    {
                        id = s.Id,
                        nombre = s.Nombre,
                        unidadOrgId = s.UnidadOrgId
                    })
                    .ToList();

                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en SeccionesOrg: {0}", ex.Message);
                return Json(new { success = false, message = "Ocurrió un error al cargar secciones." });
            }
        }

        // ==============================
        // DataTables (Server-side por AJAX)
        // ==============================
        //
        // Nota importante:
        // - Este endpoint usa métodos existentes (PaginacionEmpleados / PaginacionEmpleadosMedicos) para NO romper contrato del repositorio.
        // - A nivel de performance lo óptimo luego será mover TODO a un método IQueryable en repositorio (PostgreSQL + ILIKE/unaccent),
        //   pero aquí cumplimos la regla: NO dañar ni reemplazar métodos existentes, y compilar con lo actual.
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListaDataTable(string tipoEmpleado)
        {
            try
            {
                // DataTables params
                var draw = ParseInt(Request.Form["draw"], 0);
                var start = ParseInt(Request.Form["start"], 0);
                var length = ParseInt(Request.Form["length"], 10);

                if (start < 0) start = 0;
                if (length <= 0) length = 10;
                if (length > 200) length = 200; // límite defensivo

                var searchValue = (Request.Form["search[value]"].ToString() ?? string.Empty).Trim();

                var orderColumnIndex = ParseInt(Request.Form["order[0][column]"], 0);
                var orderDir = (Request.Form["order[0][dir]"].ToString() ?? "desc").Trim().ToLowerInvariant();
                var orderColumnData = Request.Form[$"columns[{orderColumnIndex}][data]"].ToString();

                // DataTables usa start/length (offset/limit). Convertimos a pageNumber.
                var pageNumber = (start / length) + 1;

                // Mapeo mínimo a sortOrder EXISTENTE (sin inventar nuevos ordenamientos)
                // Tus repositorios actuales solo contemplan Nombre_desc o default (Nombre asc).
                string sortOrder = null;
                if (string.Equals(orderColumnData, "nombre", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(orderDir, "desc", StringComparison.OrdinalIgnoreCase))
                {
                    sortOrder = "Nombre_desc";
                }
                // Si no, default del repo: Nombre asc

                // counts: usando métodos existentes, calculamos totals vía GetList() (ya filtra Eliminado=false).
                // IMPORTANTE: aquí mantenemos el mismo criterio de "tipo empleado" que tu repo:
                // - Medico UI => TipoEmpleado == "Profesional"
                // - Empleado UI => TipoEmpleado != "Profesional"
                var baseList = _empleadoRepository.GetList();

                int recordsTotal;
                if (string.Equals(tipoEmpleado, "Medico", StringComparison.OrdinalIgnoreCase))
                    recordsTotal = baseList.Count(e => string.Equals(e.TipoEmpleado, "Profesional", StringComparison.OrdinalIgnoreCase));
                else
                    recordsTotal = baseList.Count(e => !string.Equals(e.TipoEmpleado, "Profesional", StringComparison.OrdinalIgnoreCase));

                // filtered count (misma lógica de búsqueda actual del repo: Normalizar(Nombre) contiene filtro)
                int recordsFiltered = recordsTotal;
                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    var filtro = Normalizar(searchValue);

                    if (string.Equals(tipoEmpleado, "Medico", StringComparison.OrdinalIgnoreCase))
                    {
                        recordsFiltered = baseList
                            .Where(e => string.Equals(e.TipoEmpleado, "Profesional", StringComparison.OrdinalIgnoreCase))
                            .Count(e => Normalizar(e.Nombre).Contains(filtro));
                    }
                    else
                    {
                        recordsFiltered = baseList
                            .Where(e => !string.Equals(e.TipoEmpleado, "Profesional", StringComparison.OrdinalIgnoreCase))
                            .Count(e => Normalizar(e.Nombre).Contains(filtro));
                    }
                }

                // data page: reutilizamos paginación existente (incluye Sucursal)
                // (sin romper IEmpleado/EmpleadoRepository)
                if (string.Equals(tipoEmpleado, "Medico", StringComparison.OrdinalIgnoreCase))
                {
                    var page = _empleadoRepository.PaginacionEmpleadosMedicos(sortOrder, searchValue, pageNumber, length, tipoEmpleado);

                    var data = page.Select(item => new
                    {
                        id = item.Id,
                        nombre = item.Nombre,
                        apellido = item.Apellido,
                        telefono = item.Telefono,
                        // direccion = item.Direccion,
                        especialidad = item.Especialidad != null ? item.Especialidad.NombreEspecialidad : "-",
                        dpi = item.Dpi,
                        nit = item.Nit,
                        sucursalNombre = item.Sucursal != null ? item.Sucursal.NombreSucursal : "-",
                        colorHexadecimalFondo = item.ColorHexadecimalFondo,
                        colorHexadecimalTexto = item.ColorHexadecimalTexto,

                        colegiado = item.Colegiado,
                        credenciales = item.Credenciales,
                        direccionClinica = item.DireccionClinica,
                        telefonoClinica = item.TelefonoClinica,
                        tipoRegimen = item.TipoRegimen,
                        tipoEmpleado = item.TipoEmpleado
                    }).ToList();

                    return Json(new
                    {
                        draw = draw,
                        recordsTotal = recordsTotal,
                        recordsFiltered = recordsFiltered,
                        data = data
                    });
                }
                else
                {
                    var page = _empleadoRepository.PaginacionEmpleados(sortOrder, searchValue, pageNumber, length, tipoEmpleado);

                    var data = page.Select(item => new
                    {
                        id = item.Id,
                        nombre = item.Nombre,
                        apellido = item.Apellido,
                        telefono = item.Telefono,
                        // direccion = item.Direccion,
                        departamento = (item.UnidadOrg != null && item.UnidadOrg.DepartamentoOrg != null) ? item.UnidadOrg.DepartamentoOrg.Nombre : "-",
                        dpi = item.Dpi,
                        nit = item.Nit,
                        sucursalNombre = item.Sucursal != null ? item.Sucursal.NombreSucursal : "-",
                        colorHexadecimalFondo = item.ColorHexadecimalFondo,
                        colorHexadecimalTexto = item.ColorHexadecimalTexto,

                        salario = item.Salario,
                        estadoCivil = item.EstadoCivil
                    }).ToList();

                    return Json(new
                    {
                        draw = draw,
                        recordsTotal = recordsTotal,
                        recordsFiltered = recordsFiltered,
                        data = data
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ListaDataTable: {0}", ex.Message);

                // DataTables espera estructura válida incluso en error
                return Json(new
                {
                    draw = ParseInt(Request.Form["draw"], 0),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = Array.Empty<object>(),
                    error = "Ocurrió un error al cargar los datos."
                });
            }
        }

        private static int ParseInt(string value, int fallback)
        {
            if (string.IsNullOrWhiteSpace(value)) return fallback;
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)) return n;
            return fallback;
        }

        private static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";

            return new string(texto
                .Trim()
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray())
                .ToLowerInvariant();
        }

        public IActionResult Nuevo(string tipoEmpleado)
        {
            var model = new EmpleadoBaseViewModel();
            model.Init(_sucursalRepository, _especialidadRepository); // Incluir el repositorio de especialidades
            ViewData["TipoEmpleado"] = tipoEmpleado;
            return View(model);
        }

        public IActionResult VacacionesLista()
        {
            var empleadosConVacaciones = _empleadoRepository.GetList()
                .Where(e => e.VacacionesProgramadas != null)
                .Select(e => new
                {
                    e.Nombre,
                    VacacionesProgramadas = e.VacacionesProgramadas.ToString("yyyy-MM-dd") // Asegurar formato adecuado
                })
                .ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(empleadosConVacaciones); // Devolver JSON si la petición es AJAX
            }

            return View(empleadosConVacaciones); // Devolver la vista si es acceso normal
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Nuevo(EmpleadoBaseViewModel model)
        {
            _logger.LogInformation("Iniciando proceso de guardado de nuevo empleado.");

            _logger.LogInformation(
                "Datos Recibidos: TipoEmpleadoFormulario: {0}, Nombre: {1}, Apellido: {2}, DPI: {3}, TipoEmpleado(Entidad): {4}, EspecialidadIdSeleccionada: {5}",
                model.TipoEmpleadoFormulario,
                model.Empleado?.Nombre,
                model.Empleado?.Apellido,
                model.Empleado?.Dpi,
                model.Empleado?.TipoEmpleado,
                model.EspecialidadIdSeleccionada
            );

            // Log adicional para confirmar binding del archivo (firma)
            _logger.LogInformation(
                "FirmaEmpleadoImagen: {0}, Length: {1}",
                model.FirmaEmpleadoImagen?.FileName,
                model.FirmaEmpleadoImagen?.Length
            );

            try
            {
                // ✅ Pivot (según KO: "Empleado" o "Medico")
                var esMedicoFormulario = string.Equals(
                    model.TipoEmpleadoFormulario,
                    "Medico",
                    StringComparison.OrdinalIgnoreCase
                );

                // ✅ Blindaje: si viene como médico, forzamos el tipo en la entidad para mantener coherencia
                if (esMedicoFormulario)
                {
                    model.Empleado.TipoEmpleado = "Profesional";
                }

                // ✅ Reglas mínimas coherentes con lo que haces en UI/KO
                // - Médico: especialidad requerida
                if (esMedicoFormulario && model.EspecialidadIdSeleccionada == null)
                {
                    ModelState.AddModelError(
                        nameof(model.EspecialidadIdSeleccionada),
                        "La especialidad es obligatoria para el registro de médicos."
                    );
                }

                // Email obligatorio
                if (string.IsNullOrWhiteSpace(model.Empleado?.Email))
                {
                    ModelState.AddModelError(
                        "Empleado.Email",
                        "El Email es obligatorio."
                    );
                }

                if (ModelState.IsValid)
                {
                    // Relación especialidad (solo si aplica)
                    model.Empleado.EspecialidadId = model.EspecialidadIdSeleccionada;

                    // ✅ NUEVO: Ubicación organizacional (Unidad/Sección)
                    // No agregamos validaciones aquí para no romper flujos actuales.
                    // Si vienen en el POST, se persistirán; si no, quedan NULL.
                    // - model.Empleado.UnidadOrgId
                    // - model.Empleado.SeccionOrgId

                    // Gestión de firma
                    if (model.FirmaEmpleadoImagen != null && model.FirmaEmpleadoImagen.Length > 0)
                    {
                        var rootPath = Directory.GetCurrentDirectory();
                        var firmasPath = Path.Combine(rootPath, "wwwroot/Firmas");
                        if (!Directory.Exists(firmasPath)) Directory.CreateDirectory(firmasPath);

                        // Si el empleado ya tiene una ruta de firma guardada, borrar el archivo físico
                        if (!string.IsNullOrEmpty(model.Empleado.FirmaEmpleado))
                        {
                            var oldFilePath = Path.Combine(rootPath, "wwwroot", model.Empleado.FirmaEmpleado.TrimStart('/'));

                            if (System.IO.File.Exists(oldFilePath))
                            {
                                try
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning("No se pudo eliminar la firma anterior: {0}", ex.Message);
                                }
                            }
                        }

                        var nombreSeguro = (model.Empleado?.Nombre ?? "Empleado").Replace(" ", "_");
                        var sanitizedFileName = $"{nombreSeguro}_Firma_{Guid.NewGuid()}{Path.GetExtension(model.FirmaEmpleadoImagen.FileName)}";
                        var filePath = Path.Combine(firmasPath, sanitizedFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.FirmaEmpleadoImagen.CopyToAsync(stream);
                        }

                        model.Empleado.FirmaEmpleado = $"/Firmas/{sanitizedFileName}";
                    }

                    _empleadoRepository.Add(model.Empleado);
                    _logger.LogInformation("Empleado guardado exitosamente en BD con ID: {0}", model.Empleado.Id);

                    return Json(new
                    {
                        success = true,
                        message = "El registro ha sido creado correctamente en el sistema."
                    });
                }

                var validationErrors = ModelState.Keys
                    .SelectMany(key => ModelState[key].Errors.Select(x => new { Campo = key, Error = x.ErrorMessage }))
                    .ToList();

                _logger.LogWarning("Fallo de validación en {0} campos.", validationErrors.Count);

                return Json(new
                {
                    success = false,
                    message = "Existen campos obligatorios sin completar o con formato incorrecto.",
                    errors = validationErrors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al intentar guardar el empleado: {0}", ex.Message);
                return Json(new
                {
                    success = false,
                    message = "Ocurrió un error interno en el servidor. Contacte a soporte.",
                    detail = ex.Message
                });
            }
        }

        // GET: Empleado/Modificar
        public IActionResult Modificar(int? id, string tipoEmpleado)
        {
            if (id == null) return BadRequest("La solicitud es incorrecta");

            var empleado = _empleadoRepository.Get(id.Value);
            if (empleado == null) return StatusCode(404);

            // Determinar tipo por:
            // 1) parámetro tipoEmpleado (si viene)
            // 2) o por la entidad (Profesional => Médico)
            var esMedico =
                string.Equals(tipoEmpleado, "Medico", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(empleado.TipoEmpleado, "Profesional", StringComparison.OrdinalIgnoreCase);

            var model = new EmpleadoBaseViewModel
            {
                Empleado = empleado,
                EspecialidadIdSeleccionada = empleado.EspecialidadId,

                // ✅ Pivot para KO (igual que Nuevo)
                TipoEmpleadoFormulario = esMedico ? "Medico" : "Empleado"
            };

            model.Init(_sucursalRepository, _especialidadRepository);

            // Mantener ViewData para que la vista decida qué partial cargar
            ViewData["TipoEmpleado"] = esMedico ? "Medico" : (tipoEmpleado ?? "Normal");

            return View(model);
        }

        // POST: Empleado/Modificar
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Modificar(EmpleadoBaseViewModel model)
        {
            try
            {
                if (model?.Empleado == null)
                {
                    return Json(new { success = false, message = "La solicitud no contiene datos del empleado." });
                }

                var empleadoDb = _empleadoRepository.Get(model.Empleado.Id);
                if (empleadoDb == null)
                {
                    return Json(new { success = false, message = "Empleado no encontrado." });
                }

                // Pivot (igual que Nuevo)
                var esMedicoFormulario = string.Equals(
                    model.TipoEmpleadoFormulario,
                    "Medico",
                    StringComparison.OrdinalIgnoreCase
                );

                // Validación mínima coherente (como Nuevo)
                if (esMedicoFormulario && model.EspecialidadIdSeleccionada == null)
                {
                    ModelState.AddModelError(
                        nameof(model.EspecialidadIdSeleccionada),
                        "La especialidad es obligatoria para el registro de médicos."
                    );
                }

                if (!ModelState.IsValid)
                {
                    var validationErrors = ModelState.Keys
                        .SelectMany(key => ModelState[key]!.Errors.Select(x => new { Campo = key, Error = x.ErrorMessage }))
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message = "Existen campos obligatorios sin completar o con formato incorrecto.",
                        errors = validationErrors
                    });
                }

                // ==========================
                // Comparar y asignar solo si cambió (SIN helpers)
                // ==========================
                var cambios = new System.Collections.Generic.List<string>();

                // Nombre
                {
                    var nuevo = (model.Empleado.Nombre ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Nombre ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Nombre = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Nombre));
                    }
                }

                // Apellido
                {
                    var nuevo = (model.Empleado.Apellido ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Apellido ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Apellido = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Apellido));
                    }
                }

                // Teléfonos
                {
                    var nuevo = (model.Empleado.Telefono ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Telefono ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Telefono = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Telefono));
                    }
                }
                {
                    var nuevo = (model.Empleado.Telefono_2 ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Telefono_2 ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Telefono_2 = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Telefono_2));
                    }
                }
                // Email (obligatorio)
                {
                    var nuevoEmail = (model.Empleado.Email ?? string.Empty).Trim();
                    var viejoEmail = (empleadoDb.Email ?? string.Empty).Trim();

                    if (string.IsNullOrWhiteSpace(nuevoEmail))
                    {
                        return Json(new { success = false, message = "El campo Email es obligatorio." });
                    }

                    if (!string.Equals(viejoEmail, nuevoEmail, StringComparison.Ordinal))
                    {
                        empleadoDb.Email = nuevoEmail;
                        cambios.Add(nameof(empleadoDb.Email));
                    }
                }

                // Edad (string en tu entidad)
                {
                    var nuevo = (model.Empleado.Edad ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Edad ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Edad = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Edad));
                    }
                }

                // DPI / NIT
                {
                    var nuevo = (model.Empleado.Dpi ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Dpi ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Dpi = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Dpi));
                    }
                }
                {
                    var nuevo = (model.Empleado.Nit ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Nit ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Nit = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Nit));
                    }
                }

                // Estado civil / Género / Dirección
                {
                    var nuevo = (model.Empleado.EstadoCivil ?? string.Empty).Trim();
                    var viejo = (empleadoDb.EstadoCivil ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.EstadoCivil = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.EstadoCivil));
                    }
                }
                {
                    var nuevo = (model.Empleado.Genero ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Genero ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Genero = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Genero));
                    }
                }
                {
                    var nuevo = (model.Empleado.Direccion ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Direccion ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Direccion = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Direccion));
                    }
                }

                // Contrato / Salario (string) / SucursalId (int?)
                {
                    var nuevo = (model.Empleado.TipoContrato ?? string.Empty).Trim();
                    var viejo = (empleadoDb.TipoContrato ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.TipoContrato = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.TipoContrato));
                    }
                }
                {
                    var nuevo = (model.Empleado.Salario ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Salario ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Salario = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Salario));
                    }
                }
                if (!Nullable.Equals(empleadoDb.SucursalId, model.Empleado.SucursalId))
                {
                    empleadoDb.SucursalId = model.Empleado.SucursalId;
                    cambios.Add(nameof(empleadoDb.SucursalId));
                }

                // Jornada / Fechas (DateTime NO-nullable en tu entidad)
                {
                    var nuevo = (model.Empleado.JornadaTrabajo ?? string.Empty).Trim();
                    var viejo = (empleadoDb.JornadaTrabajo ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.JornadaTrabajo = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.JornadaTrabajo));
                    }
                }
                if (!DateTime.Equals(empleadoDb.FechaInicioLabores, model.Empleado.FechaInicioLabores))
                {
                    empleadoDb.FechaInicioLabores = model.Empleado.FechaInicioLabores;
                    cambios.Add(nameof(empleadoDb.FechaInicioLabores));
                }
                if (!DateTime.Equals(empleadoDb.VacacionesProgramadas, model.Empleado.VacacionesProgramadas))
                {
                    empleadoDb.VacacionesProgramadas = model.Empleado.VacacionesProgramadas;
                    cambios.Add(nameof(empleadoDb.VacacionesProgramadas));
                }
                if (!DateTime.Equals(empleadoDb.VacacionesProgramadasFinal, model.Empleado.VacacionesProgramadasFinal))
                {
                    empleadoDb.VacacionesProgramadasFinal = model.Empleado.VacacionesProgramadasFinal;
                    cambios.Add(nameof(empleadoDb.VacacionesProgramadasFinal));
                }

                // ✅ NUEVO: UnidadOrgId / SeccionOrgId (int?)
                if (!Nullable.Equals(empleadoDb.UnidadOrgId, model.Empleado.UnidadOrgId))
                {
                    empleadoDb.UnidadOrgId = model.Empleado.UnidadOrgId;
                    cambios.Add(nameof(empleadoDb.UnidadOrgId));
                }
                if (!Nullable.Equals(empleadoDb.SeccionOrgId, model.Empleado.SeccionOrgId))
                {
                    empleadoDb.SeccionOrgId = model.Empleado.SeccionOrgId;
                    cambios.Add(nameof(empleadoDb.SeccionOrgId));
                }

                // Clínica / Colores / Observaciones
                {
                    var nuevo = (model.Empleado.TelefonoClinica ?? string.Empty).Trim();
                    var viejo = (empleadoDb.TelefonoClinica ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.TelefonoClinica = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.TelefonoClinica));
                    }
                }
                {
                    var nuevo = (model.Empleado.DireccionClinica ?? string.Empty).Trim();
                    var viejo = (empleadoDb.DireccionClinica ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.DireccionClinica = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.DireccionClinica));
                    }
                }
                {
                    var nuevo = (model.Empleado.ColorHexadecimalFondo ?? string.Empty).Trim();
                    var viejo = (empleadoDb.ColorHexadecimalFondo ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.ColorHexadecimalFondo = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.ColorHexadecimalFondo));
                    }
                }
                {
                    var nuevo = (model.Empleado.ColorHexadecimalTexto ?? string.Empty).Trim();
                    var viejo = (empleadoDb.ColorHexadecimalTexto ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.ColorHexadecimalTexto = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.ColorHexadecimalTexto));
                    }
                }
                {
                    var nuevo = (model.Empleado.Observaciones ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Observaciones ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Observaciones = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Observaciones));
                    }
                }

                // Campos médico
                {
                    var nuevo = (model.Empleado.Colegiado ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Colegiado ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Colegiado = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Colegiado));
                    }
                }
                {
                    var nuevo = (model.Empleado.Residente ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Residente ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Residente = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Residente));
                    }
                }
                {
                    var nuevo = (model.Empleado.Credenciales ?? string.Empty).Trim();
                    var viejo = (empleadoDb.Credenciales ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.Credenciales = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.Credenciales));
                    }
                }
                {
                    var nuevo = (model.Empleado.TipoBanco ?? string.Empty).Trim();
                    var viejo = (empleadoDb.TipoBanco ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.TipoBanco = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.TipoBanco));
                    }
                }
                {
                    var nuevo = (model.Empleado.TipoCuenta ?? string.Empty).Trim();
                    var viejo = (empleadoDb.TipoCuenta ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.TipoCuenta = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.TipoCuenta));
                    }
                }
                {
                    var nuevo = (model.Empleado.NumeroCuenta ?? string.Empty).Trim();
                    var viejo = (empleadoDb.NumeroCuenta ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.NumeroCuenta = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.NumeroCuenta));
                    }
                }
                {
                    var nuevo = (model.Empleado.NombreCuenta ?? string.Empty).Trim();
                    var viejo = (empleadoDb.NombreCuenta ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.NombreCuenta = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.NombreCuenta));
                    }
                }
                {
                    var nuevo = (model.Empleado.NitPropietarioCuenta ?? string.Empty).Trim();
                    var viejo = (empleadoDb.NitPropietarioCuenta ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.NitPropietarioCuenta = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.NitPropietarioCuenta));
                    }
                }
                {
                    var nuevo = (model.Empleado.NombrePropietarioNit ?? string.Empty).Trim();
                    var viejo = (empleadoDb.NombrePropietarioNit ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.NombrePropietarioNit = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.NombrePropietarioNit));
                    }
                }
                {
                    var nuevo = (model.Empleado.TipoRegimen ?? string.Empty).Trim();
                    var viejo = (empleadoDb.TipoRegimen ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.TipoRegimen = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.TipoRegimen));
                    }
                }

                // Especialidad
                if (!Nullable.Equals(empleadoDb.EspecialidadId, model.EspecialidadIdSeleccionada))
                {
                    empleadoDb.EspecialidadId = model.EspecialidadIdSeleccionada;
                    cambios.Add(nameof(empleadoDb.EspecialidadId));
                }

                // TipoEmpleado (forzado para médico)
                if (esMedicoFormulario)
                {
                    if (!string.Equals(empleadoDb.TipoEmpleado, "Profesional", StringComparison.Ordinal))
                    {
                        empleadoDb.TipoEmpleado = "Profesional";
                        cambios.Add(nameof(empleadoDb.TipoEmpleado));
                    }
                }
                else
                {
                    var nuevo = (model.Empleado.TipoEmpleado ?? string.Empty).Trim();
                    var viejo = (empleadoDb.TipoEmpleado ?? string.Empty).Trim();
                    if (!string.Equals(viejo, nuevo, StringComparison.Ordinal))
                    {
                        empleadoDb.TipoEmpleado = string.IsNullOrWhiteSpace(nuevo) ? null : nuevo;
                        cambios.Add(nameof(empleadoDb.TipoEmpleado));
                    }
                }

                // ==========================
                // Firma: solo si viene archivo nuevo
                // ==========================
                if (model.FirmaEmpleadoImagen != null && model.FirmaEmpleadoImagen.Length > 0)
                {
                    var firmasPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Firmas");
                    if (!Directory.Exists(firmasPath)) Directory.CreateDirectory(firmasPath);

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(empleadoDb.FirmaEmpleado) &&
                            empleadoDb.FirmaEmpleado.StartsWith("/Firmas/", StringComparison.OrdinalIgnoreCase))
                        {
                            var anteriorNombre = empleadoDb.FirmaEmpleado.Replace("/Firmas/", "");
                            var anteriorPath = Path.Combine(firmasPath, anteriorNombre);

                            if (System.IO.File.Exists(anteriorPath))
                                System.IO.File.Delete(anteriorPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "No se pudo borrar la firma anterior del empleado ID: {0}", empleadoDb.Id);
                    }

                    var sanitizedFileName =
                        $"{(empleadoDb.Nombre ?? "Empleado").Replace(" ", "_")}_Firma_{Guid.NewGuid()}{Path.GetExtension(model.FirmaEmpleadoImagen.FileName)}";

                    var filePath = Path.Combine(firmasPath, sanitizedFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.FirmaEmpleadoImagen.CopyToAsync(stream);
                    }

                    empleadoDb.FirmaEmpleado = $"/Firmas/{sanitizedFileName}";
                    cambios.Add(nameof(empleadoDb.FirmaEmpleado));
                }

                // ==========================
                // Guardar SOLO si hay cambios
                // ==========================
                if (cambios.Count == 0)
                {
                    return Json(new
                    {
                        success = true,
                        changed = false,
                        message = "No hay datos por actualizar."
                    });
                }

                _empleadoRepository.Update(empleadoDb);

                return Json(new
                {
                    success = true,
                    changed = true,
                    message = "Datos modificados exitosamente.",
                    camposActualizados = cambios
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al intentar modificar el empleado: {0}", ex.Message);
                return Json(new
                {
                    success = false,
                    message = "Ocurrió un error interno en el servidor. Contacte a soporte.",
                    detail = ex.Message
                });
            }
        }

        public IActionResult Eliminar(int? id, string tipoEmpleado)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _empleadoRepository.Get((int)id);

            if (model == null)
            {
                return StatusCode(404);
            }

            model.Eliminado = true;

            _empleadoRepository.Update(model);
            TempData["Message"] = "¡El empleado se ha eliminado con éxito!";
            ViewData["TipoEmpleado"] = tipoEmpleado;
            return RedirectToAction("Lista", new { tipoEmpleado = tipoEmpleado });
        }
    }
}