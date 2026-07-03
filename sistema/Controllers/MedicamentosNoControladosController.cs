using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Database.Shared;
using Database.Shared.Models;
using Database.Shared.IRepository;
using Database.Shared.Enumeraciones;
using farmamest.Service.IService;

namespace sistema.Controllers
{
    [Authorize]
    public class MedicamentoNoControladoController : Controller
    {
        private readonly Context _db;
        private readonly IProducto _productoRepository;
        private readonly IProductosService _productosService;
        private readonly UserManager<User> _userManager;
        private readonly IMedicamentoNoControladoRepository _medicamentoNoControladoRepository;

        public MedicamentoNoControladoController(
            Context db,
            IProducto productoRepository,
            IProductosService productosService,
            UserManager<User> userManager,
            IMedicamentoNoControladoRepository medicamentoNoControladoRepository)
        {
            _db = db;
            _productoRepository = productoRepository;
            _productosService = productosService;
            _userManager = userManager;
            _medicamentoNoControladoRepository = medicamentoNoControladoRepository;
        }

        // ==========================================================
        // 1. Obtener lista de productos (medicamentos con stock)
        // ==========================================================
        [HttpPost]
        public JsonResult ConsultarProductosNoControlados()
        {
            try
            {
                var productos = _productoRepository.GetProductosHospitalizacion();
                return Json(new { exitoso = true, resultado = productos });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, resultado = ex.Message });
            }
        }

        // ==========================================================
        // 2. Guardar registro de medicamentos no controlados
        // ==========================================================
        [HttpPost]
        public JsonResult Guardar([FromBody] GuardarMedNoControladosDto data)
        {
            if (data == null || data.Registros == null || !data.Registros.Any())
                return Json(new { exitoso = false, resultado = "No hay datos para guardar." });

            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var fechaRegistro = DateTime.Now;

                var productosConStock = (
                    from pi in _db.ProductosInventario
                    join p in _db.Productos on pi.ProductoId equals p.Id
                    join b in _db.Bodegas on pi.BodegaId equals b.Id
                    where b.AmbienteId == 3
                       && pi.BodegaId == 8
                       && pi.Stock > 0
                       && p.TipoProductoId == 1
                    select new
                    {
                        ProductoId = p.Id,
                        pi.UnidadMedidaVentaId
                    }
                ).ToList();

                foreach (var reg in data.Registros)
                {
                    // decimal cantidadNeta = (reg.UnidadesIniciales + reg.UnidadesExtra) - reg.Retornadas;

                    // if (cantidadNeta > 0)
                    // {
                    //     var inventarioProducto = productosConStock
                    //         .FirstOrDefault(i => i.ProductoId == reg.ProductoId);

                    //     if (inventarioProducto == null)
                    //         throw new Exception($"No se encontró inventario disponible para el producto {reg.ProductoNombre}");

                    //     _productosService.RealizarDescuentoInventario(
                    //         reg.ProductoId,
                    //         inventarioProducto.UnidadMedidaVentaId,
                    //         (int)Math.Ceiling(cantidadNeta)
                    //     );
                    // }

                    var registro = new MedicamentoNoControlado
                    {
                        HospitalizacionId = data.HospitalizacionId,
                        FechaProcedimiento = data.FechaProcedimiento,
                        FechaRegistro = fechaRegistro,
                        ProductoId = reg.ProductoId,
                        ProductoNombre = reg.ProductoNombre,
                        UnidadesIniciales = reg.UnidadesIniciales,
                        UnidadesExtra = reg.UnidadesExtra,
                        Utilizado = reg.Utilizado,
                        Descartado = reg.Descartado,
                        Retornadas = reg.Retornadas,
                        UsuarioRegistroId = userId,
                        Eliminado = false
                    };
                    _medicamentoNoControladoRepository.Add(registro);
                }

                return Json(new { exitoso = true });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, resultado = ex.Message });
            }
        }

        // ==========================================================
        // 3. Obtener historial de registros
        // ==========================================================
        [HttpPost]
        public JsonResult ObtenerHistorial(int hospitalizacionId)
        {
            try
            {
                var historial = _medicamentoNoControladoRepository.GetHistorialByHospitalizacion(hospitalizacionId)
                    .Select(m => new
                    {
                        fechaRegistro = m.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                        nombre = m.ProductoNombre,
                        unidadesIniciales = m.UnidadesIniciales,
                        unidadesExtra = m.UnidadesExtra,
                        utilizado = m.Utilizado,
                        descartado = m.Descartado,
                        retornadas = m.Retornadas
                    })
                    .ToList();

                return Json(new { exitoso = true, resultado = historial });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = ex.Message });
            }
        }
    }

    // DTOs
    public class GuardarMedNoControladosDto
    {
        public int HospitalizacionId { get; set; }
        public DateTime? FechaProcedimiento { get; set; }
        public List<MedicamentoNoControladoRegistroDto> Registros { get; set; }
    }

    public class MedicamentoNoControladoRegistroDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public decimal UnidadesIniciales { get; set; }
        public decimal UnidadesExtra { get; set; }
        public decimal Utilizado { get; set; }
        public decimal Descartado { get; set; }
        public decimal Retornadas { get; set; }
    }

    public class ProductoInventarioDto
    {
        public int ProductoId { get; set; }
        public string ProductoCodigo { get; set; }
        public string ProductoNombre { get; set; }
    }
}