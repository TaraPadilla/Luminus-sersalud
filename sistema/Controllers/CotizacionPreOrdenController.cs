using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Shared;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;

namespace sistema.Controllers
{
    [Route("CotizacionPreOrden")]
    public class CotizacionPreOrdenController : Controller
    {
        private readonly Context _context;

        public CotizacionPreOrdenController(Context context)
        {
            _context = context;
        }

        [HttpGet("Listado")]
        public IActionResult Listado()
        {
            var cotizaciones = _context.CotizacionesPreOrden
                .Select(c => new { c.Id, c.Fecha })
                .OrderByDescending(c => c.Fecha)
                .ToList();

            return View(cotizaciones);
        }

        [HttpGet("Ver/{id}")]
        public IActionResult Ver(int id)
        {
            var cotizacion = _context.CotizacionesPreOrden.Find(id);
            if (cotizacion == null) return NotFound();

            var items = System.Text.Json.JsonSerializer
                .Deserialize<List<CotizacionItemViewModel>>(cotizacion.Items);

            var proveedoresPrecargados = items.SelectMany(i => i.Proveedores.Keys).Distinct().ToList();

            ViewBag.Proveedores = proveedoresPrecargados;
            ViewBag.ProveedorPrincipal = cotizacion.ProveedorPrincipal;
            
            // Reconstruimos el diccionario para saber quién es el principal de cada item
            ViewBag.ProveedorPrincipalPorItem = items
                .Where(i => i.ProveedorPrincipal != null)
                .ToDictionary(i => i.ProductoId.ToString(), i => i.ProveedorPrincipal);

            ViewBag.TodosProveedores = _context.Proveedores
                                        .Where(x => !x.Eliminado)
                                        .Select(x => x.Nombre)
                                        .ToList();

            // Pasamos el ID aunque el JS lo tome de la URL, por consistencia
            ViewBag.Id = id;

            return View(items);
        }

        [HttpPost("Guardar")]
        public async Task<IActionResult> GuardarCotizacion([FromBody] CotizacionRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Items))
                {
                    return BadRequest("Datos inválidos.");
                }

                CotizacionesPreOrden cotizacion;

                // === LÓGICA UPSERT (Actualizar o Insertar) ===
                if (request.Id.HasValue && request.Id > 0)
                {
                    // BUSCAR Y ACTUALIZAR
                    cotizacion = _context.CotizacionesPreOrden.Find(request.Id.Value);
                    if (cotizacion == null)
                    {
                        return NotFound(new { message = "La cotización a actualizar no existe." });
                    }

                    // Actualizamos los datos
                    cotizacion.Items = request.Items;
                    cotizacion.ProveedorPrincipal = request.ProveedorPrincipal;
                    // Opcional: cotizacion.Fecha = DateTime.Now; // Si deseas actualizar la fecha al editar

                    _context.CotizacionesPreOrden.Update(cotizacion);
                }
                else
                {
                    // CREAR NUEVA
                    cotizacion = new CotizacionesPreOrden
                    {
                        Fecha = DateTime.Now,
                        Items = request.Items,
                        ProveedorPrincipal = request.ProveedorPrincipal
                    };
                    _context.CotizacionesPreOrden.Add(cotizacion);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cotización guardada correctamente", id = cotizacion.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al guardar la cotización", error = ex.Message });
            }
        }
    }

    // Modelo DTO actualizado para recibir el ID
    public class CotizacionRequest
    {
        public int? Id { get; set; } // <--- NUEVO CAMPO
        public string Fecha { get; set; }
        public string Items { get; set; }
        public string ProveedorPrincipal { get; set; }
    }
}