using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using sistema.Service.IService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Shared;
using Microsoft.EntityFrameworkCore;

namespace farmamest.Controllers
{
    public class KitIngresoController : Controller
    {
        private readonly IKitIngresoService _kitIngresoService;
        private readonly IProductosService _productosService;
        private readonly UserManager<User> _userManager;

        private readonly Context _context;


        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public KitIngresoController(
            IKitIngresoService kitIngresoService,
            IProductosService productosService,
            UserManager<User> userManager,
            Context context)
        {
            _kitIngresoService = kitIngresoService;
            _productosService = productosService;
            _userManager = userManager;
            _context = context;
        }

        // ── CREAR KIT (global, HospitalizacionId = null) ────────────────────
        [HttpPost]
        public string AgregarKitIngreso([FromBody] KitIngresoInputVM model)
        {
            try
            {
                if (model == null)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos inválidos." }, _jsonOpts);

                var kit = new KitIngreso
                {
                    FechaRegistro = DateTime.Now,
                    HospitalizacionId = null, // siempre null para kits globales
                    UserId = _userManager.GetUserId(HttpContext.User),
                    NombreKit = model.NombreKit,
                    NombrePaciente = model.NombrePaciente,
                    Medico = model.Medico,
                    Procedimiento = model.Procedimiento,
                    Responsable = model.Responsable,
                    FechaKit = model.FechaKit,
                };

                _kitIngresoService.Add(kit);
                return JsonSerializer.Serialize(new { exitoso = true, resultado = kit.Id }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                }, _jsonOpts);
            }
        }

        [HttpPost]
        public JsonResult ObtenerKitsGlobales(int? hospitalizacionId)
        {
            if (hospitalizacionId.HasValue)
            {
                var kits = _kitIngresoService.GetGlobalesYPorHospitalizacion(hospitalizacionId.Value);
                return Json(new { exitoso = true, resultado = kits });
            }
            else
            {
                var kits = _kitIngresoService.GetGlobalKits();
                return Json(new { exitoso = true, resultado = kits });
            }
        }

        // ── OBTENER UTILIZADO DE UN DETALLE EN UNA HOSPITALIZACIÓN ──────────
        [HttpPost]
        public async Task<JsonResult> ObtenerUtilizadoKitDetalle(int detalleId, int hospitalizacionId)
        {
            var utilizado = await _kitIngresoService.ObtenerUtilizadoAsync(detalleId, hospitalizacionId);
            return Json(new { exitoso = true, utilizado = utilizado });
        }

        // ── GUARDAR UTILIZADO DE UN DETALLE EN UNA HOSPITALIZACIÓN ──────────
        [HttpPost]
        public async Task<JsonResult> GuardarUtilizadoKitDetalle(int detalleId, int hospitalizacionId, decimal utilizado)
        {
            try
            {
                await _kitIngresoService.GuardarUtilizadoAsync(detalleId, hospitalizacionId, utilizado);
                return Json(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }

        // ── CLONAR KIT (copiar plantilla a una hospitalización) ─────────────
        // [HttpPost]
        // public async Task<JsonResult> ClonarKitIngreso(int kitOrigenId, int hospitalizacionDestinoId)
        // {
        //     try
        //     {
        //         var userId = _userManager.GetUserId(HttpContext.User);
        //         var nuevoKit = _kitIngresoService.ClonarKit(kitOrigenId, hospitalizacionDestinoId, userId);
        //         return Json(new { exitoso = true, resultado = nuevoKit.Id });
        //     }
        //     catch (Exception ex)
        //     {
        //         return Json(new { exitoso = false, mensaje = ex.Message });
        //     }
        // }

        [HttpPost]
        public async Task<JsonResult> ClonarKitIngreso(int kitOrigenId, int hospitalizacionDestinoId)
        {
            try
            {
                var hospitalizacion = await _context.Hospitalizaciones
                    .Include(h => h.Paciente)
                    .FirstOrDefaultAsync(h => h.Id == hospitalizacionDestinoId);

                if (hospitalizacion == null)
                    return Json(new { exitoso = false, mensaje = "Hospitalización destino no encontrada." });

                var consulta = await _context.Consultas
                    .Include(c => c.Citas)   
                    .FirstOrDefaultAsync(c => c.HospitalizacionId == hospitalizacionDestinoId);

                string nombrePaciente = hospitalizacion.Paciente?.Nombre ?? "";
                string medico = "";
                string procedimiento = "";
                string responsable = "";

                if (consulta?.Citas != null)
                {
                    var cita = consulta.Citas;
                    if (cita.EmpleadoId != null)
                    {
                        var empleado = await _context.Empleados.FindAsync(cita.EmpleadoId);
                        medico = empleado?.NombreYApellidos ?? "";
                    }
                    procedimiento = cita.Procedimiento ?? "";
                    responsable = cita.ResponsableNombre ?? "";
                }
                else
                {
                    var citaFallback = await _context.Citass
                        .Where(c => c.PacienteId == hospitalizacion.PacienteId && !c.Eliminado)
                        .OrderByDescending(c => c.Id)
                        .FirstOrDefaultAsync();
                    if (citaFallback != null)
                    {
                        if (citaFallback.EmpleadoId != null)
                        {
                            var emp = await _context.Empleados.FindAsync(citaFallback.EmpleadoId);
                            medico = emp?.NombreYApellidos ?? "";
                        }
                        procedimiento = citaFallback.Procedimiento ?? "";
                        responsable = citaFallback.ResponsableNombre ?? "";
                    }
                }

                var userId = _userManager.GetUserId(HttpContext.User);
                var nuevoKit = _kitIngresoService.ClonarKitConDatos(
                    kitOrigenId,
                    hospitalizacionDestinoId,
                    userId,
                    nombrePaciente,
                    medico,
                    procedimiento,
                    responsable);

                return Json(new { exitoso = true, resultado = nuevoKit.Id });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }



        [HttpPost]
        public JsonResult ActualizarEncabezadoKit([FromBody] ActualizarEncabezadoKitRequest request)
        {
            try
            {
                var kit = _context.KitsIngreso.Find(request.Id);
                if (kit == null)
                    return Json(new { exitoso = false, mensaje = "Kit no encontrado." });

                kit.NombreKit = request.NombreKit;
                kit.NombrePaciente = request.NombrePaciente;
                kit.Medico = request.Medico;
                kit.Procedimiento = request.Procedimiento;
                kit.Responsable = request.Responsable;

                _context.SaveChanges();
                return Json(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }

        public class ActualizarEncabezadoKitRequest
        {
            public int Id { get; set; }
            public string NombreKit { get; set; }
            public string NombrePaciente { get; set; }
            public string Medico { get; set; }
            public string Procedimiento { get; set; }
            public string Responsable { get; set; }
        }


        // ── ACTUALIZAR DETALLE DE KIT (solo para editar plantilla, NO usado para Utilizado) ──
        // Este método se mantiene pero sin lógica de clonación. Solo actualiza la plantilla.
        [HttpPost]
        public string ActualizarDetalleKitIngreso([FromBody] KitIngresoDetalleInputVM model)
        {
            try
            {
                if (model == null || model.Id == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos inválidos." }, _jsonOpts);

                _kitIngresoService.ActualizarDetalle(model);

                if (!string.IsNullOrEmpty(model.NombreKit))
                {
                    var kit = _kitIngresoService.GetById(model.KitIngresoId);
                    if (kit != null && kit.NombreKit != model.NombreKit)
                    {
                        kit.NombreKit = model.NombreKit;
                        _kitIngresoService.UpdateKit(kit);
                    }
                }

                return JsonSerializer.Serialize(new { exitoso = true }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, resultado = ex.Message }, _jsonOpts);
            }
        }

        // ── AGREGAR PRODUCTO AL KIT ─────────────────────────────────────────
        [HttpPost]
        public string AgregarProductoKitIngreso([FromBody] KitIngresoDetalleInputVM model)
        {
            try
            {
                if (model == null || model.KitIngresoId == 0)
                    return JsonSerializer.Serialize(new { exitoso = false, resultado = "Datos inválidos." }, _jsonOpts);

                _kitIngresoService.AgregarProducto(model);
                return JsonSerializer.Serialize(new { exitoso = true, resultado = true }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    resultado = "Error: " + ex.Message + " | " + ex.InnerException?.Message
                }, _jsonOpts);
            }
        }

        // ── ELIMINAR PRODUCTO DEL KIT ───────────────────────────────────────
        [HttpPost]
        public string EliminarDetalleKitIngreso(int detalleId)
        {
            try
            {
                _kitIngresoService.EliminarDetalle(detalleId);
                return JsonSerializer.Serialize(new { exitoso = true }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    exitoso = false,
                    mensaje = "Error al eliminar: " + ex.Message
                }, _jsonOpts);
            }
        }

        // ── OBTENER DETALLE POR ID ─────────────────────────────────────────
        [HttpPost]
        public string ObtenerDetalleKitIngreso(int detalleId)
        {
            try
            {
                var detalle = _kitIngresoService.GetDetalleById(detalleId);
                if (detalle == null)
                    return JsonSerializer.Serialize(new { exitoso = false, mensaje = "Detalle no encontrado" }, _jsonOpts);
                return JsonSerializer.Serialize(new { exitoso = true, resultado = detalle }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, mensaje = ex.Message }, _jsonOpts);
            }
        }
        [HttpPost]
        public string ConsultarProductosExistentes()
        {
            try
            {
                var productos = new List<object>
        {
            new { ProductoId = 1, ProductoCodigo = "ANG-20", ProductoNombre = "Angiocath # 20" },
            new { ProductoId = 2, ProductoCodigo = "ANG-22", ProductoNombre = "Angiocath # 22" },
            new { ProductoId = 3, ProductoCodigo = "ANG-24", ProductoNombre = "Angiocath # 24" },
            new { ProductoId = 4, ProductoCodigo = "1624W", ProductoNombre = "Aposito transparente TEGADERM 5*7CM" },
            new { ProductoId = 5, ProductoCodigo = "1683", ProductoNombre = "Aposito transparente TEGADERM IV advance" },
            new { ProductoId = 6, ProductoCodigo = "147", ProductoNombre = "Equipo de Suero Venoset corto" },
            new { ProductoId = 7, ProductoCodigo = "147R", ProductoNombre = "Equipo de suero Venoset largo" },
            new { ProductoId = 8, ProductoCodigo = "CONEC-2", ProductoNombre = "Conector de dos vías" },
            new { ProductoId = 9, ProductoCodigo = "DEL-0082", ProductoNombre = "Suero de cloruro de sodio al 9% salina de 1000 ML" },
            new { ProductoId = 10, ProductoCodigo = "DEL-195", ProductoNombre = "Suero de Lactato de Ringer Hartman 1000 ml." },
            new { ProductoId = 11, ProductoCodigo = "UP-01", ProductoNombre = "Uniforme para Paciente" },
            new { ProductoId = 12, ProductoCodigo = "", ProductoNombre = "Marcador, liga, micropore" }
        };

                return JsonSerializer.Serialize(new { exitoso = true, resultado = productos }, _jsonOpts);
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { exitoso = false, mensaje = ex.Message }, _jsonOpts);
            }
        }

        [HttpPost]
        public JsonResult ObtenerDetallesKit(int kitId)
        {
            var kit = _kitIngresoService.GetById(kitId);
            if (kit == null) return Json(new { exitoso = false });
            return Json(new { exitoso = true, detalles = kit.Detalles.Where(d => !d.Eliminado) });
        }
    }
}