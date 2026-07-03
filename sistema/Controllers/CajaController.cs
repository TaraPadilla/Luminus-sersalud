using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Shared.IRepository;
using sistema.Models;
using Database.Shared.Paginacion;
using System.Linq;
using Database.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore;
using System.Text.Json;
using Database.Shared.Enumeraciones;
using System.Collections.Generic;
using sistema.Service.IService;
using System.Security.Claims;

namespace sisrest.Controllers
{
    [Authorize]
    public class CajaController : Controller
    {
        private readonly ICaja _cajaRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly UserManager<User> _userManager = null;
        private readonly IGeneratePdf _generatePdf;

        //Service (logica de negocio)
        private readonly ICajaService _cajaService = null;


        public CajaController(
            ICaja cajaRepository,
            UserManager<User> userManager,
            IGeneratePdf generatePdf,
            ISucursal sucursalRepository,
            //Servicio (logica)
            ICajaService cajaService)
        {
            _cajaRepository = cajaRepository;
            _userManager = userManager;
            _generatePdf = generatePdf;
            _sucursalRepository = sucursalRepository;

            _cajaService = cajaService;
        }

        public IActionResult Aperturar(int? ambienteId)
        {
            if (ambienteId == null)
            {
                TempData["Message"] = "Error de navegacion";
                return RedirectToAction("Index", "Home");
            }

            var model = new CajaBaseViewModel
            {
                AperturarAmbienteId = (int)ambienteId,
                CajaMontoApertura = 1000
            };
            model.Init(_cajaRepository, _sucursalRepository);

            return View(model);
        }

        [HttpPost]
        public async Task<string> Aperturar(CajaBaseViewModel model)
        {

            try
            {
                var cajita = _cajaRepository.ListarCajas()
                    .Where(a => a.AmbienteId == model.AperturarAmbienteId
                                && a.SucursalId == model.AperturarSucursalId)
                    .ToList();

                // Depuración
                System.Diagnostics.Debug.WriteLine($"Total de cajas encontradas: {cajita.Count}");
                foreach (var caja in cajita)
                {
                    System.Diagnostics.Debug.WriteLine($"Caja Id: {caja.Id}, EstadoCaja: {caja.EstadoCaja}");
                }

                if (cajita.Any(a => a.EstadoCaja == true))
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "Ya hay una caja abierta para este ambiente en esta sucursal"
                    });
                }

                else
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);

                    var nuevaCaja = new Caja()
                    {
                        MontoApertura = model.CajaMontoApertura,
                        NombrePersonalizado = model.CajaNombrePersonalizado,
                        FechaApertura = DateTime.Now,
                        AmbienteId = model.AperturarAmbienteId,
                        SucursalId = model.AperturarSucursalId,
                        EstadoCaja = true,
                        ResponsableApertura = user,
                    };

                    _cajaRepository.Add(nuevaCaja);

                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al abrir caja. " + ex.Message
                });
            }
        }

        public async Task<string> Cerrar(int cajaId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                _cajaService.CerrarCaja(user, cajaId);

                TempData["Message"] = "Caja cerrada con Exito";

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al cerrar caja. Ex: " + ex.Message
                });
            }
        }

        public string ReabrirCaja(int cajaId, int empleadoId)
        {
            try
            {
                var usuarioLog = HttpContext.User;
                var guidUsuario = usuarioLog.FindFirst(ClaimTypes.NameIdentifier).Value;

                var resultadoVerificarEmpleado = _cajaService.VerificarEmpleado(guidUsuario, empleadoId);
                if (resultadoVerificarEmpleado)
                {
                    _cajaService.ReabrirCaja(cajaId);
                    TempData["Message"] = "Reapertura exitosa";

                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true
                    });
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Codigo de empleado incorrecto"
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al intentar reabrir caja. Ex: " + ex.Message
                });
            }
        }
        public IActionResult VerDetalle(int? id)
        {

            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _cajaService.GetDetallesCaja((int)id);

            return View(model);
        }

        public JsonResult GuardarIngreso(string monto, string descripcion, int? cajaClinicaId,
            int banco, int cuenta, string numeroComprobante)
        {

            if (monto != null && descripcion != null)
            {

                //var cajaAbierta = _cajaRepository.GetCajaAbierta();
                var cajaAbierta = _cajaRepository.GetCajaAbiertaById((int)cajaClinicaId);

                var nuevoDetalleCaja = new DetalleCaja()
                {
                    Fecha = DateTime.Now,
                    Descripcion = descripcion,
                    Ingreso = Convert.ToDecimal(monto),
                    Caja = cajaAbierta,
                    BancoId = banco,
                    CuentaId = cuenta,
                    NumeroComprabante = numeroComprobante,
                    CuentaContableId = null
                };

                _cajaRepository.Add(nuevoDetalleCaja);

                TempData["Message"] = "¡El ingreso se ha guardado con exito.!";

                return Json(nuevoDetalleCaja.Caja.Id);
            }

            return Json("Ha ocurrido un error");

        }

        public JsonResult GuardarEgreso(string monto, string descripcion, int? cajaClinicaId, int cuenta, string numeroComprobante)
        {

            var idCajaActual = cajaClinicaId;

            if (monto != null && descripcion != null)
            {
                //var cajaAbierta = _cajaRepository.GetCajaAbierta();
                var cajaAbierta = _cajaRepository.GetCajaAbiertaById((int)cajaClinicaId);

                var nuevoDetalleCaja = new DetalleCaja()
                {
                    Fecha = DateTime.Now,
                    Descripcion = descripcion,
                    Gasto = Convert.ToDecimal(monto),
                    Caja = cajaAbierta,
                    BancoId = null,
                    CuentaId = null,
                    CuentaContableId = cuenta,
                    NumeroComprabante = numeroComprobante

                };

                _cajaRepository.Add(nuevoDetalleCaja);

                TempData["Message"] = "¡El gasto se ha guardado con exito.!";

                return Json(nuevoDetalleCaja.Caja.Id);
            }

            return Json("Ha ocurrido un error");
        }

        public async Task<IActionResult> Reporte(string fecha, int ambienteIdReporte)
        {
            var fechas = fecha.Split('-');
            var cajas = _cajaRepository.GetListadoFecha(Convert.ToDateTime(fechas[0].Trim()),
                Convert.ToDateTime(fechas[1].Trim()).AddDays(1), ambienteIdReporte);

            var nombreAmbiente = "Global";

            switch (ambienteIdReporte)
            {
                case (int)AmbienteEnum.Farmacia:
                    nombreAmbiente = "Farmacia";
                    break;
                case (int)AmbienteEnum.Clinica:
                    nombreAmbiente = "Clinica";
                    break;
                case (int)AmbienteEnum.Bodega:
                    nombreAmbiente = "Bodega";
                    break;
                case (int)AmbienteEnum.Laboratorio:
                    nombreAmbiente = "Laboratorio";
                    break;
                case (int)AmbienteEnum.Hospital:
                    nombreAmbiente = "Hospital";
                    break;
                default:
                    nombreAmbiente = "Global";
                    break;
            }

            var model = new ReporteCajaFarmaciaViewModel()
            {
                Cajas = cajas,
                Desde = fechas[0].Trim(),
                Hasta = fechas[1].Trim(),
                NombreAmbiente = nombreAmbiente
            };

            return await _generatePdf.GetPdf("Views/Caja/Reporte.cshtml", model);
        }

        public async Task<IActionResult> ReporteDetalle(int? id)
        {
            if (id == null)
            {
                return BadRequest("request is incorrect");
            }

            var model = _cajaService.GetDetallesCaja((int)id);

            return await _generatePdf.GetPdf("Views/Caja/ReporteDetalle.cshtml", model);
        }

        public JsonResult EliminarDetalle(int detalleId)
        {
            _cajaRepository.DeleteDetalleCaja(detalleId);
            TempData["Message"] = "¡El registro se ha eliminado con éxito.!";

            return Json(new object());
        }
    }
}