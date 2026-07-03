using System.ComponentModel.Design;
using System.IO.MemoryMappedFiles;
using System.Linq.Expressions;
using System.Reflection;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using sistema.Models;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Http;
using sistema.Json;
using Wkhtmltopdf.NetCore;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Rotativa.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Database.Shared.Enumeraciones;
using farmamest.Service.IService;
using farmamest.Utilidades;
using sistema.Service.IService;
using System.Text.Encodings.Web;
using Database.Shared;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Globalization;
using farmamest;

namespace sistema.Controllers
{
    [Authorize]
    public class CuentasPorCobrarController : Controller
    {
        private readonly ICuentasPorCobrar _cuentasPorCobrarRepository = null;
        private readonly ICaja _cajaRepository = null;
        private readonly IPacientes _pacientesRepository = null;
        private readonly IHospitalizacion _hospitalizacionRepository = null;
        private readonly IHabitacion _habitacionRepository = null;
        private readonly IEnvio _envioRepository = null;
        private readonly IEmpleado _empleadoRepository = null;
        private readonly UserManager<User> _userManager = null;
        private readonly ICuentasPorCobrarService _cuentasPorCobrarService;
        private readonly IConsultas _consultasRepository = null;
        private readonly IHospitalizacionService _hospitalizacionService;
        private readonly Context _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FelSettings _felSettings;

        public CuentasPorCobrarController(
            ICuentasPorCobrar cuentasPorCobrarRepository,
            ICaja cajaRepository,
            IPacientes pacientesRepository,
            IHospitalizacion hospitalizacionRepository,
            IHabitacion habitacionRepository,
            IEnvio envioRepository,
            IEmpleado empleadoRepository,
            UserManager<User> userManager,
            ICuentasPorCobrarService cuentasPorCobrarService,
            IConsultas consultasRepository,
            IHospitalizacionService hospitalizacionService,
            Context context,
            IHttpClientFactory httpClientFactory,
            IOptions<FelSettings> felOptions
            )
        {
            _cuentasPorCobrarRepository = cuentasPorCobrarRepository;
            _cajaRepository = cajaRepository;
            _pacientesRepository = pacientesRepository;
            _hospitalizacionRepository = hospitalizacionRepository;
            _habitacionRepository = habitacionRepository;
            _envioRepository = envioRepository;
            _empleadoRepository = empleadoRepository;
            _userManager = userManager;
            _cuentasPorCobrarService = cuentasPorCobrarService;
            _consultasRepository = consultasRepository;
            _hospitalizacionService = hospitalizacionService;
            _context = context;
            _httpClientFactory = httpClientFactory;
            _felSettings = felOptions?.Value ?? new FelSettings();
        }

        public IActionResult Pendientes() => View();

        public IActionResult Pagadas() => View();

        public IActionResult VerDetallesCuenta(int? cuentaId)
        {
            if (cuentaId == null) return RedirectToAction("Pendientes");

            var cuenta = _cuentasPorCobrarService.GetDetalleCuentaPorCobrar((int)cuentaId);
            cuenta.CuentaId = (int)cuentaId;

            if (cuenta.CuentasPorCobrar == null) return RedirectToAction("Pendientes");

            return View(cuenta);
        }

        public async Task<IActionResult> Pagar(
      [FromQuery] int? cuentaId,
      [FromQuery] string? ResponsableNit,
      [FromQuery] string? ResponsableNombre,
      [FromQuery] string? ResponsableDireccion,
      [FromQuery] string? ResponsableCorreo,
      [FromQuery] string? SeguroNombre,
      [FromQuery] string? PacienteNombreAdmision,
      [FromQuery] int AdmisionId,
      [FromQuery] bool soloVista = false)
        {
            if (cuentaId == null) return RedirectToAction("Pendientes");

            var cuenta = _cuentasPorCobrarRepository.Get((int)cuentaId);
            if (cuenta == null) return RedirectToAction("Pendientes");

            var paciente = cuenta.Paciente;
            if (paciente == null)
                return View(CreateEmptyPagarViewModel("La cuenta no tiene un paciente asociado."));

            #region Consulta hospitalizacion

            if (AdmisionId <= 0)
                AdmisionId = ResolverAdmisionIdDesdeCuenta(cuenta);

            if (AdmisionId <= 0)
                return View(CreateEmptyPagarViewModel("No se encontró una admisión asociada a esta cuenta."));

            bool cuentaTieneEsaHospitalizacion = cuenta.DetallesCuentaPorCobrar != null
                && cuenta.DetallesCuentaPorCobrar.Any(d => d.HospitalizacionId.HasValue && d.HospitalizacionId.Value == AdmisionId);

            if (!cuentaTieneEsaHospitalizacion && !soloVista && !cuenta.Pagada)
                return RedirectToAction("Pendientes");

            var hospitalizacion = _hospitalizacionRepository.Get(AdmisionId, true, true, false, true, false);
            if (hospitalizacion == null)
                return View(CreateEmptyPagarViewModel("No se encontró la hospitalización asociada a esta cuenta."));


            bool tieneSaldoPendiente = cuenta.Valor > 0;

            bool esSoloVista = soloVista || (cuenta.Pagada && !tieneSaldoPendiente);
            decimal totalHospitalizacionCalculado = 0m;
            DateTime fechaFinDate = DateTime.Now.Date;

            // ── CAMBIO: en modo soloVista/Pagada traemos TODOS los registros del
            //    paciente (incluido el que ya está pagado) para poder mostrar el
            //    costo de estadía tal como fue al momento del pago.
            // var registrosBd = esSoloVista
            //     ? _pacientesRepository.GetHospitalizaciones(paciente.Id)
            //           .Where(b => b.Id == AdmisionId).ToList()
            //     : _pacientesRepository.GetHospitalizaciones(paciente.Id)
            //           .Where(b => !b.Pagada && !b.Finalizada).ToList();
    //         var registrosBd = esSoloVista
    // ? _pacientesRepository.GetHospitalizaciones(paciente.Id)
    //       .Where(b => b.Id == AdmisionId).ToList()
    // : _pacientesRepository.GetHospitalizaciones(paciente.Id)
    //       .Where(b => !b.Pagada).ToList();

          var registrosBd = _pacientesRepository.GetHospitalizaciones(paciente.Id)
    .Where(b => b.Id == AdmisionId).ToList();

            if (registrosBd != null && registrosBd.Any())
            {
                foreach (var registro in registrosBd)
                {
                    if (registro == null) continue;
                    var fechaInicio = registro.FechaInicio.Date;
                    var nochesReg = (fechaInicio == fechaFinDate) ? 1 : (fechaFinDate - fechaInicio).Days;
                    var valorTarifa = Convert.ToDecimal(registro.CategoriaHabitacionTarifa?.ValorTarifa ?? 0);
                    totalHospitalizacionCalculado += valorTarifa * nochesReg;

                    var cambiosHabitacion = _hospitalizacionRepository.GetCambiosHabitacion(registro.Id);
                    if (cambiosHabitacion != null)
                    {
                        foreach (var cambio in cambiosHabitacion)
                        {
                            totalHospitalizacionCalculado += Convert.ToDecimal(cambio.ValorTarifa) * cambio.Dias;
                        }
                    }
                }
            }

            totalHospitalizacionCalculado = Math.Round(totalHospitalizacionCalculado, 2);
            #endregion

            if (hospitalizacion.Habitacion == null || hospitalizacion.Habitacion.CategoriaHabitacion == null)
                return View(CreateEmptyPagarViewModel("La hospitalización no tiene habitación o categoría configurada."));

            var habitacionViewModel = new HabitacionPagarViewModel
            {
                Id = hospitalizacion.Habitacion.Id,
                CategoriaId = hospitalizacion.Habitacion.CategoriaHabitacionId,
                NombreHabitacion = hospitalizacion.Habitacion.NombreNumeroHabitacion,
                NombreCategoriaHabitacion = hospitalizacion.Habitacion.CategoriaHabitacion.NombreCategoria,
                CostoTotal = totalHospitalizacionCalculado
            };

            var totalHabitacion = habitacionViewModel.CostoTotal;

            int calcularEdad(DateTime? fechaNacimiento)
            {
                if (fechaNacimiento == null) return 0;
                var today = DateTime.Today;
                var edad = today.Year - fechaNacimiento.Value.Year;
                if (fechaNacimiento.Value.Date > today.AddYears(-edad)) edad--;
                return edad;
            }

            var consulta = _consultasRepository.GetConsultaPorHospitalizacion((int)hospitalizacion.Id);

            var pacienteInfo = new
            {
                Nombre = paciente.Nombre ?? "No registrado",
                Telefono = paciente.Telefono ?? "No disponible",
                Celular = paciente.Celular ?? "No disponible",
                Direccion = paciente.Direccion ?? "No registrada",
                Nit = paciente.Nit ?? "No disponible",
                Dpi = paciente.Dpi ?? "No disponible",
                FechaNacimiento = paciente.FechaNacimiento?.ToString("dd/MM/yyyy") ?? "No registrada",
                Edad = calcularEdad(paciente.FechaNacimiento).ToString(),
                Sexo = paciente.Sexo?.DescripcionSexo ?? "No definido",
                FechaInicioHops = hospitalizacion.FechaInicio.ToString("dd/MM/yyyy"),
                MedicoHops = consulta?.Citas?.Empleado?.NombreYApellidos ?? "No se encontró ningún profesional.",
                NombreDelSeguro = consulta?.Citas?.CodigoDeCita
            };

            var paquetesHospitalizacion = hospitalizacion.HospitalizacionesPaquetesHospitalizacion
                .Where(p => !p.Eliminado)
                .Select(p => new PaquetePagarViewModel
                {
                    Id = p.PaqueteHospitalizacionId,
                    Nombre = p.PaqueteHospitalizacion.NombrePaquete,
                    Precio = p.PaqueteHospitalizacion.Precio ?? 0,
                    Tipo = "Paquete de Hospitalización"
                }).ToList();

            var totalPaquetes = paquetesHospitalizacion.Sum(p => p.Precio);

            var productosConPrecios = hospitalizacion.HospitalizacionesProductos
                .Where(p => HospitalizacionCargosHelper.AplicacionesVigentes(p.HospitalizacionesProductosAplicaciones).Any())
                .Select(p =>
                {
                    var aplicacionesAplicadas = HospitalizacionCargosHelper
                        .AplicacionesVigentes(p.HospitalizacionesProductosAplicaciones)
                        .ToList();
                    var cantidadAplicada = aplicacionesAplicadas.Sum(a => a.Cantidad);
                    if (cantidadAplicada <= 0) return null;

                    return new ProductoPagarViewModel
                    {
                        Id = p.Id,
                        ProductoId = p.Producto?.Id ?? 0,
                        Nombre = p.Producto?.NombreProducto ?? "Desconocido",
                        Cantidad = cantidadAplicada,
                        PrecioUnitario = Math.Round(p.PrecioValor, 2),
                        Subtotal = Math.Round(cantidadAplicada * p.PrecioValor, 2),
                        Tipo = HospitalizacionCargosHelper.ObtenerTipoProductoCuenta(p.Producto?.TipoProductoId),
                        FechaAplicacion = HospitalizacionCargosHelper.FormatearFechaAplicacionProducto(
                            aplicacionesAplicadas,
                            p.FechaHoraAplicacionManual) is { Length: > 0 } fecha
                            ? fecha
                            : "Sin fecha"
                    };
                })
                .Where(p => p != null)
                .ToList();

            var serviciosDetalleList = _hospitalizacionService.GetServiciosHospitalizacion(hospitalizacion.Id)?.ToList() ?? new List<HospitalizacionServicioViewModel>();

            var insumosDirectosVM = new List<ProductoPagarViewModel>();
            try
            {
                var insumosDirectos = await _context.HospitalizacionInsumosDirectos
                    .Where(i => i.HospitalizacionId == AdmisionId && !i.Eliminado)
                    .Include(i => i.Producto).Include(i => i.Aplicaciones).ToListAsync();

                foreach (var insumo in insumosDirectos)
                {
                    var aplicacionesAplicadas = HospitalizacionCargosHelper
                        .AplicacionesVigentesInsumoDirecto(insumo.Aplicaciones)
                        .ToList();
                    if (!aplicacionesAplicadas.Any()) continue;

                    int cantidadAplicada = aplicacionesAplicadas.Sum(a => a.Cantidad);
                    if (cantidadAplicada <= 0) continue;

                    var subtotal = Math.Round(cantidadAplicada * insumo.PrecioValor, 2);

                    insumosDirectosVM.Add(new ProductoPagarViewModel
                    {
                        Id = insumo.Id,
                        ProductoId = insumo.ProductoId,
                        Nombre = insumo.Producto?.NombreProducto ?? "Producto",
                        Cantidad = cantidadAplicada,
                        PrecioUnitario = Math.Round(insumo.PrecioValor, 2),
                        Subtotal = subtotal,
                        Tipo = HospitalizacionCargosHelper.ObtenerTipoProductoCuenta(insumo.Producto?.TipoProductoId),
                        FechaAplicacion = HospitalizacionCargosHelper.FormatearFechaAplicacionInsumoDirecto(
                            aplicacionesAplicadas,
                            insumo.FechaHoraAplicacionManual) is { Length: > 0 } fecha
                            ? fecha
                            : "Sin fecha"
                    });
                }
            }
            catch (Exception) { /* Manejo silencioso */ }

            var serviciosConPrecios = serviciosDetalleList.Select(s =>
            {
                var cantidad = Convert.ToDecimal(s.Cantidad);
                var precio = Convert.ToDecimal(s.Precio);
                var subtotal = Convert.ToDecimal(s.Subtotal);
                string fechaParseada = "Sin fecha";

                if (!string.IsNullOrEmpty(s.FechaHoraAplicacion))
                {
                    if (DateTime.TryParse(s.FechaHoraAplicacion, new CultureInfo("es-GT"), DateTimeStyles.None, out var date))
                        fechaParseada = date.ToString("dd/MM/yyyy");
                    else
                        fechaParseada = s.FechaHoraAplicacion;
                }

                return new ProductoPagarViewModel
                {
                    Id = s.Id,
                    ProductoId = s.Id,
                    Nombre = s.Nombre ?? "Servicio sin nombre",
                    Cantidad = (int)Math.Round(cantidad, 0),
                    PrecioUnitario = Math.Round(precio, 2),
                    Subtotal = Math.Round(subtotal, 2),
                    Tipo = "Servicio",
                    FechaAplicacion = fechaParseada
                };
            }).ToList();

            var examenesConPrecios = _hospitalizacionRepository.GetExamenes(hospitalizacion.Id)
                .SelectMany(e => e.Examen?.DetalleExamenes ?? Enumerable.Empty<DetalleExamen>(), (e, detalleExamen) => new { e, detalleExamen })
                .Select(x =>
                {
                    var precio = x.detalleExamen?.PrecioValor ?? 0;
                    return new ProductoPagarViewModel
                    {
                        Id = x.e.Id,
                        ProductoId = x.e.ExamenId,
                        Nombre = x.detalleExamen?.ExamenLabClinico?.NombreExamen ?? "Examen sin nombre",
                        Cantidad = 1,
                        PrecioUnitario = Math.Round(Convert.ToDecimal(precio), 2),
                        Subtotal = Math.Round(Convert.ToDecimal(precio), 2),
                        Tipo = x.detalleExamen?.ExamenLabClinico?.CategoriaLabClinico?.Nombre ?? "Sin categoría",
                        FechaAplicacion = x.e.FechaAplicacionFormateada,
                        EsExamen = true
                    };
                }).ToList();

            var dietasConPrecios = new List<ProductoPagarViewModel>();
            var recetasBd = _hospitalizacionRepository.GetHospitalizacionRecetaByIdHospitalizacion(hospitalizacion.Id);

            if (recetasBd != null)
            {
                foreach (var hr in recetasBd)
                {
                    if (hr?.Receta == null) continue;
                    var cantidad = Convert.ToDecimal(hr.Cantidad);
                    var precioVenta = Convert.ToDecimal(hr.Receta.PrecioVenta);
                    if (cantidad <= 0 || precioVenta <= 0) continue;

                    var subtotal = Math.Round(cantidad * precioVenta, 2);
                    dietasConPrecios.Add(new ProductoPagarViewModel
                    {
                        Id = hr.Id,
                        ProductoId = hr.Receta.Id,
                        Nombre = hr.Receta.NombreReceta ?? "Dieta",
                        Cantidad = (int)Math.Round(cantidad, 0),
                        PrecioUnitario = Math.Round(precioVenta, 2),
                        Subtotal = subtotal,
                        Tipo = "Dietas",
                        FechaAplicacion = "Sin fecha"
                    });
                }
            }

            var Ambulancias = await _context.Ambulancias
                .Where(a => a.HospitalizacionId == AdmisionId)
                .Select(a => new { a.Id, a.TipoViaje, a.Precio })
                .ToListAsync();
            var ambulanciasVM = Ambulancias
                .Select(a => new AmbulanciaPagarViewModel { Id = a.Id, TipoTraslado = a.TipoViaje, Precio = a.Precio })
                .ToList();

            decimal totalDepositos = 0m;
            try
            {
                var pagos = _cuentasPorCobrarRepository.GetPagos(cuenta.Id);
                if (pagos != null) totalDepositos = Math.Round(pagos.Sum(p => Convert.ToDecimal(p.Monto)), 2);
            }
            catch (Exception) { totalDepositos = 0m; }

            var depositosComoProducto = new List<ProductoPagarViewModel>();
            if (totalDepositos > 0m)
            {
                depositosComoProducto.Add(new ProductoPagarViewModel
                {
                    Id = 0,
                    ProductoId = 0,
                    Nombre = "Depósitos (resta)",
                    Cantidad = 1,
                    PrecioUnitario = Math.Round(-totalDepositos, 2),
                    Subtotal = Math.Round(-totalDepositos, 2),
                    Tipo = "Depósitos (resta)",
                    FechaAplicacion = "Sin fecha"
                });
            }

            var emergenciasVM = new List<ProductoPagarViewModel>();
            try
            {
                var emergencia = await _context.Emergencias.FirstOrDefaultAsync(e => e.HospitalizacionId == AdmisionId);
                if (emergencia != null)
                {
                    var detallesEmergencia = await _context.EmergenciaDetalles
                        .Where(d => d.EmergenciaId == emergencia.Id && d.Eliminado != true)
                        .Include(d => d.Producto).Include(d => d.Servicio).Include(d => d.ExamenLabClinico)
                        .ToListAsync();

                    if (detallesEmergencia != null && detallesEmergencia.Any())
                    {
                        foreach (var detalle in detallesEmergencia)
                        {
                            decimal subtotal = detalle.Cantidad * detalle.PrecioValor;
                            decimal descuentoValor = subtotal * (detalle.DescuentoPorcentaje / 100);
                            decimal total = subtotal - descuentoValor;

                            string nombreItem = detalle.Producto?.NombreProducto
                                ?? detalle.Servicio?.NombreServicio
                                ?? detalle.ExamenLabClinico?.NombreExamen
                                ?? "Item de emergencia";

                            emergenciasVM.Add(new ProductoPagarViewModel
                            {
                                Id = detalle.Id,
                                ProductoId = detalle.ProductoId ?? detalle.ServicioId ?? detalle.ExamenLabClinicoId ?? 0,
                                Nombre = nombreItem,
                                Cantidad = (int)detalle.Cantidad,
                                PrecioUnitario = Math.Round(detalle.PrecioValor, 2),
                                Subtotal = Math.Round(total, 2),
                                Tipo = "Emergencia",
                                FechaAplicacion = emergencia.FechaEmergencia.ToString("dd/MM/yyyy")
                            });
                        }
                    }
                }
            }
            catch (Exception) { /* Manejo silencioso */ }

            // ── Mapeo maestro del ViewModel ─────────────────────────────────────────
            var model = new CuentasPorCobrarPagarViewModel
            {
                CuentaId = cuenta.Id,
                Valor = Math.Round(Convert.ToDecimal(cuenta.Valor ?? 0m), 2),
                Observaciones = cuenta.Observaciones,
                PacienteId = cuenta.Paciente.Id,
                ResponsableNit = ResponsableNit,
                ResponsableNombre = ResponsableNombre,
                ResponsableDireccion = ResponsableDireccion,
                ResponsableCorreo = ResponsableCorreo,
                Habitacion = habitacionViewModel,

                Productos = productosConPrecios
                    .Concat(serviciosConPrecios)
                    .Concat(examenesConPrecios)
                    .Concat(dietasConPrecios)
                    .Concat(depositosComoProducto)
                    .Concat(emergenciasVM)
                    .Concat(insumosDirectosVM)
                    .ToList(),

                Paquetes = paquetesHospitalizacion,
                SeguroNombre = SeguroNombre,
                PacienteNombreAdmision = PacienteNombreAdmision,
                AdmisionId = AdmisionId,

                PacienteNombre = pacienteInfo.Nombre,
                PacienteTelefono = pacienteInfo.Telefono,
                PacienteCelular = pacienteInfo.Celular,
                PacienteDireccion = pacienteInfo.Direccion,
                PacienteNit = pacienteInfo.Nit,
                PacienteDpi = pacienteInfo.Dpi,
                PacienteFechaNacimiento = pacienteInfo.FechaNacimiento,
                PacienteEdad = pacienteInfo.Edad,
                PacienteSexo = pacienteInfo.Sexo,
                FechaInicioHops = pacienteInfo.FechaInicioHops,
                MedicoHops = pacienteInfo.MedicoHops,
                NombreDelSeguro = pacienteInfo.NombreDelSeguro,
                Ambulancias = ambulanciasVM,

                SoloVista = esSoloVista,

            };

            ViewBag.Hospitalizacion = hospitalizacion;
            model.Init(_cuentasPorCobrarRepository, _empleadoRepository);

            return View(model);
        }


        [HttpPost]
        public string ConsultarPagos(int cuentaId)
        {
            try
            {
                var pagosConsultados = new List<CuentasPorCobrarPagoViewModel>();
                var pagosBd = _cuentasPorCobrarRepository.GetPagos(cuentaId);
                if (pagosBd != null)
                {
                    foreach (var pago in pagosBd)
                    {
                        pagosConsultados.Add(new CuentasPorCobrarPagoViewModel
                        {
                            Fecha = pago.FechaHora != null ? ((DateTime)pago.FechaHora).ToString() : "-",
                            FormaPagoId = pago.FormaPagoId,
                            FormaPago = pago.FormaPago.NombreFormaPago,
                            MonedaId = (int)pago.MonedaId,
                            Moneda = pago.Moneda.NombreMoneda,
                            PagoId = pago.Id,
                            Monto = pago.Monto,
                            Nuevo = false
                        });
                    }
                }
                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = pagosConsultados });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al consultar pagos. " + ex.Message });
            }
        }

        // =========================================================================
        // MÉTODOS DE PAGO
        // =========================================================================
        [HttpPost]
        public async Task<string> Pagar(CuentasPorCobrarPagarViewModel model)
        {
            try
            {
                if (model == null || model.CuentaId <= 0 || model.PacienteId <= 0)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Solicitud inválida." });

                var cajaHospi = _cajaRepository.ListarCajas().Where(a => a.AmbienteId == (int)AmbienteEnum.Hospital);
                var cajaAbierta = cajaHospi.FirstOrDefault(a => a.EstadoCaja);

                if (cajaAbierta == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error. No hay cajas abiertas. Por favor abra una caja." });

                var cuenta = _cuentasPorCobrarRepository.Get(model.CuentaId);
                if (cuenta == null) return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No se encontró la cuenta." });

                var paciente = _pacientesRepository.Get(model.PacienteId);
                var fechaHora = DateTime.Now;

                // =========================================================================================

                var hospitalizacion = _hospitalizacionRepository.Get(model.AdmisionId, true, true, false, true, false);
                if (hospitalizacion == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No se encontró la hospitalización." });

                // 1. Habitación
                decimal totalHabitacion = 0m;
                // var registrosBd = _pacientesRepository.GetHospitalizaciones(model.PacienteId).Where(b => !b.Pagada && !b.Finalizada).ToList();
                var registrosBd = _pacientesRepository.GetHospitalizaciones(model.PacienteId)
    .Where(b => b.Id == model.AdmisionId).ToList();
                
                DateTime fechaFinDate = DateTime.Now.Date;
                foreach (var registro in registrosBd)
                {
                    var nochesReg = (registro.FechaInicio.Date == fechaFinDate) ? 1 : (fechaFinDate - registro.FechaInicio.Date).Days;
                    totalHabitacion += Convert.ToDecimal(registro.CategoriaHabitacionTarifa?.ValorTarifa ?? 0) * nochesReg;
                }

                // 2. Paquetes
                decimal totalPaquetes = hospitalizacion.HospitalizacionesPaquetesHospitalizacion.Where(p => !p.Eliminado).Sum(p => p.PaqueteHospitalizacion.Precio ?? 0);

                // 3. Productos (medicamentos, insumos, etc.) — suma bruta desde BD sin descuentos de línea
                decimal totalProductos = hospitalizacion.HospitalizacionesProductos
                    .Where(p => HospitalizacionCargosHelper.AplicacionesVigentes(p.HospitalizacionesProductosAplicaciones).Any())
                    .Sum(p => HospitalizacionCargosHelper.AplicacionesVigentes(p.HospitalizacionesProductosAplicaciones).Sum(a => a.Cantidad) * p.PrecioValor);

                // 4. Servicios
                var serviciosDetalle = _hospitalizacionService.GetServiciosHospitalizacion(hospitalizacion.Id);
                decimal totalServicios = serviciosDetalle != null ? serviciosDetalle.Sum(s => Convert.ToDecimal(s.Subtotal)) : 0m;

                // 5. Dietas
                var recetasBd = _hospitalizacionRepository.GetHospitalizacionRecetaByIdHospitalizacion(hospitalizacion.Id);
                decimal totalDietas = recetasBd?.Where(r => r?.Receta != null).Sum(r => Convert.ToDecimal(r.Cantidad) * Convert.ToDecimal(r.Receta.PrecioVenta)) ?? 0m;

                // 6. Ambulancias
                decimal totalAmbulancias = await _context.Ambulancias.Where(a => a.HospitalizacionId == model.AdmisionId).SumAsync(a => a.Precio);

                // 7. Insumos Directos
                var insumosDirectos = await _context.HospitalizacionInsumosDirectos.Where(i => i.HospitalizacionId == model.AdmisionId && !i.Eliminado).Include(i => i.Aplicaciones).ToListAsync();
                decimal totalInsumosDirectos = insumosDirectos.Sum(i =>
                    HospitalizacionCargosHelper.AplicacionesVigentesInsumoDirecto(i.Aplicaciones).Sum(a => a.Cantidad) * i.PrecioValor);

                // 8. Emergencias
                decimal totalEmergencias = 0m;
                var emergencia = await _context.Emergencias.FirstOrDefaultAsync(e => e.HospitalizacionId == model.AdmisionId);
                if (emergencia != null)
                {
                    var detallesEme = await _context.EmergenciaDetalles.Where(d => d.EmergenciaId == emergencia.Id && d.Eliminado != true).ToListAsync();
                    totalEmergencias = detallesEme.Sum(d => (d.Cantidad * d.PrecioValor) - ((d.Cantidad * d.PrecioValor) * (d.DescuentoPorcentaje / 100)));
                }

                // 9. Honorarios BD
                var honorariosDb = await _context.HospitalizacionHonorarios.Where(h => h.HospitalizacionId == model.AdmisionId).ToListAsync();
                decimal totalHonorarios = honorariosDb.Sum(h => h.Monto);

                // 10. Gastos Administrativos
                var gastosAdminDb = await _context.HospitalizacionGastosAdministrativos
                    .Where(g => g.HospitalizacionId == model.AdmisionId).ToListAsync();
                decimal totalGastosAdmin = gastosAdminDb.Sum(g => g.Monto);

                // TOTAL BRUTO REAL desde BD (se usa solo para FEL y registros contables)
                var totalBrutoReal = totalHabitacion + totalPaquetes + totalProductos
                    + totalServicios + totalDietas + totalAmbulancias
                    + totalInsumosDirectos + totalEmergencias + totalHonorarios
                    + totalGastosAdmin;

                // Restar exclusiones enviadas desde el frontend
                decimal totalExclusionesFront = model.Exclusiones?.Sum(e => e.Cantidad * e.PrecioUnitario) ?? 0;
                totalBrutoReal = Math.Max(totalBrutoReal - totalExclusionesFront, 0);

                // Descuento global (para FEL)
                decimal descuentoGlobal = 0;
                if (model.PorcentajeDescuento > 0)
                    descuentoGlobal = totalBrutoReal * (model.PorcentajeDescuento / 100m);
                else if (model.Descuento > 0)
                    descuentoGlobal = model.Descuento;

                if (descuentoGlobal > totalBrutoReal)
                    descuentoGlobal = totalBrutoReal;

                decimal totalConDescuento = totalBrutoReal - descuentoGlobal;

                // =========================================================================================
                // CORRECCIÓN CLAVE: CÁLCULO DEL SALDO PENDIENTE USANDO TotalFrontend
                // =========================================================================================
                var pagosBd = _cuentasPorCobrarRepository.GetPagos(cuenta.Id);
                var pagadoAnterior = pagosBd != null ? Math.Round(pagosBd.Sum(p => Convert.ToDecimal(p.Monto)), 2) : 0m;

                decimal totalParaSaldo;

                if (model.TotalFrontend > 0)
                {
                    decimal diferencia = Math.Abs(model.TotalFrontend - totalConDescuento);
                    decimal toleranciaMaxima = 1.00m; // Q1.00 de tolerancia

                    if (diferencia > toleranciaMaxima)
                    {
                        Console.WriteLine($"[Pagar] Diferencia de totales: Frontend={model.TotalFrontend}, BD={totalConDescuento}, Diferencia={diferencia}. Se usa TotalFrontend.");
                    }

                    totalParaSaldo = model.TotalFrontend;
                }
                else
                {
                    // Si por alguna razón TotalFrontend no llegó, usar el total calculado desde BD
                    totalParaSaldo = totalConDescuento;
                }

                var saldoPendienteReal = totalParaSaldo - pagadoAnterior;
                if (saldoPendienteReal < 0) saldoPendienteReal = 0;

                // Si ya está pagado (saldo 0) y no hay nuevos pagos, rechazar
                if (saldoPendienteReal <= 0 && (model.Pagos == null || !model.Pagos.Any(p => p.Nuevo && p.Monto > 0)))
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Esta cuenta ya se encuentra cancelada en su totalidad." });
                }

                // =========================================================================================
                // PROCESAMIENTO DE PAGOS NUEVOS
                // =========================================================================================
                // =========================================================================================
                // PROCESAMIENTO DE PAGOS NUEVOS
                // =========================================================================================
                decimal totalPagoNuevo = 0m;
                if (model.Pagos != null)
                {
                    foreach (var pago in model.Pagos.Where(p => p != null && p.Nuevo && p.Monto > 0))
                    {
                        var monto = Convert.ToDecimal(pago.Monto);

                        // Validar que no se exceda el saldo pendiente (tolerancia de Q0.01 para redondeo)
                        if (totalPagoNuevo + monto > saldoPendienteReal + 0.01m)
                        {
                            var excedente = (totalPagoNuevo + monto) - saldoPendienteReal;
                            return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = $"El monto a pagar excede el saldo pendiente. Excede por: Q {excedente:0.00}." });
                        }

                        if (totalPagoNuevo + monto > saldoPendienteReal)
                            monto = saldoPendienteReal - totalPagoNuevo;

                        var pagoBd = new Pagos
                        {
                            CuentaPorCobrarId = model.CuentaId,
                            FormaPagoId = Convert.ToInt32(pago.FormaPagoId),
                            Monto = monto,
                            MonedaId = pago.MonedaId,
                            FechaHora = fechaHora
                        };

                        _cuentasPorCobrarRepository.AddPago(pagoBd);
                        totalPagoNuevo += monto;

                        // ── AQUÍ LA SOLUCIÓN: Consultamos el médico asignado a esta hospitalización específica
                        var consultaMedica = _consultasRepository.GetConsultaPorHospitalizacion((int)model.AdmisionId);

                        // También obtenemos el ID del cajero por si la hospitalización no tuviera médico asignado
                        var userActual = await _userManager.GetUserAsync(User);
                        int idEmpleadoCajero = userActual?.EmpleadoId ?? 1;

                        var nombrePaciente = !string.IsNullOrWhiteSpace(model.PacienteNombre)
                            ? model.PacienteNombre
                            : (paciente?.Nombre ?? "Paciente");

                        var nombreResponsable = string.IsNullOrWhiteSpace(model.ResponsableNombre)
                            ? "Consumidor Final"
                            : model.ResponsableNombre.Trim();

                        var nitResponsable = string.IsNullOrWhiteSpace(model.ResponsableNit)
                            ? "CF"
                            : model.ResponsableNit.Trim();

                        var nuevaVenta = new Venta
                        {
                            PacienteId = model.PacienteId,
                            EmpleadoId = consultaMedica?.Citas?.EmpleadoId ?? idEmpleadoCajero,
                            AmbienteId = (int)AmbienteEnum.Hospital,  

                            Nit = nitResponsable,
                            Nombres = nombreResponsable,
                            Direccion = string.IsNullOrWhiteSpace(model.ResponsableDireccion) ? "N/A" : model.ResponsableDireccion.Trim(),
                            Correo = string.IsNullOrWhiteSpace(model.ResponsableCorreo) ? "sin-correo@example.com" : model.ResponsableCorreo.Trim(),

                            ResponsableNombre = nombrePaciente,

                            FechaVenta = fechaHora,
                            TipoVenta = "Hospitalizacion",
                            MontoPago = monto,
                            Vuelto = 0m,
                            UuidFel = cuenta.UuidFel,
                            Origen = "Hospitalización",
                            Eliminado = false,
                        
                            Pagos = new List<Pagos>
    {
        new Pagos
        {
            FormaPagoId = Convert.ToInt32(pago.FormaPagoId),
            Monto       = monto,
            FechaHora   = fechaHora
        }
    }
                        };

                        // ── La descripción del DetalleCaja usa el nombre del PACIENTE ──
                        _cajaRepository.Add(new DetalleCaja
                        {
                            CuentaPorCobrarId = model.CuentaId,
                            CuentaPorCobrarPagoId = pagoBd.Id,
                            Descripcion = "Pago de cuenta (Hospitalización). Cliente: " + nombrePaciente,
                            Ingreso = monto,
                            Venta = nuevaVenta,
                            Caja = cajaAbierta
                        });

                    }
                }
                if (totalPagoNuevo <= 0m && saldoPendienteReal > 0m)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Debe registrar un pago válido para cubrir el saldo pendiente." });
                }

                // =========================================================================================
                // ACTUALIZACIÓN DE LA CUENTA
                // =========================================================================================
                var nuevoPagadoAcumulado = pagadoAnterior + totalPagoNuevo;
                cuenta.ValorPagado = nuevoPagadoAcumulado;
                cuenta.Valor = saldoPendienteReal - totalPagoNuevo;  // Nuevo saldo restante
                cuenta.FechaPagoRealizado = fechaHora;

                bool cuentaCerrada = (cuenta.Valor ?? 0m) <= 0.005m;
                cuenta.Pagada = cuentaCerrada;

                // Guardar datos del responsable para FEL
                cuenta.FelReceptorNit = string.IsNullOrWhiteSpace(model.ResponsableNit) ? "CF" : model.ResponsableNit.Trim();
                cuenta.FelReceptorNombre = string.IsNullOrWhiteSpace(model.ResponsableNombre) ? "Consumidor Final" : model.ResponsableNombre.Trim();
                cuenta.FelReceptorDireccion = string.IsNullOrWhiteSpace(model.ResponsableDireccion) ? "N/A" : model.ResponsableDireccion.Trim();
                cuenta.FelReceptorCorreo = string.IsNullOrWhiteSpace(model.ResponsableCorreo) ? "sin-correo@example.com" : model.ResponsableCorreo.Trim();

                _cuentasPorCobrarRepository.Update(cuenta);

                // Cierre de hospitalización si la cuenta queda en cero
                if (cuentaCerrada && cuenta.DetallesCuentaPorCobrar != null)
                {
                    foreach (var detalle in cuenta.DetallesCuentaPorCobrar.Where(d => d.HospitalizacionId != null))
                    {
                        var hospUpdate = _hospitalizacionRepository.Get((int)detalle.HospitalizacionId, false, false, false);
                        if (hospUpdate != null)
                        {
                            hospUpdate.Pagada = true;
                            hospUpdate.Finalizada = true;
                            hospUpdate.FechaHoraFinalizada = fechaHora;
                            hospUpdate.FechaHoraPago = fechaHora;
                            _hospitalizacionRepository.Update(hospUpdate);

                            var habitacion = _habitacionRepository.Get(hospUpdate.HabitacionId);
                            if (habitacion != null)
                                habitacion.EstadoHabitacionId = (int)EstadoHabitacionEnum.Disponible;
                        }
                    }
                }

                var cuentaActualizada = _cuentasPorCobrarRepository.Get(model.CuentaId);
                var saldoRestante = Math.Round(Convert.ToDecimal(cuentaActualizada.Valor ?? 0m), 2);

                // =========================================================================================
                // FACTURACIÓN FEL (solo si la cuenta está cerrada)
                // =========================================================================================
                if (!cuentaCerrada)
                {
                    cuentaActualizada.FelEstado = EstadosFEL.NoIniciado;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(cuentaActualizada.UuidFel))
                    {
                        cuentaActualizada.FelEstado = EstadosFEL.Pendiente;
                        cuentaActualizada.FelFechaUltimoIntento = DateTime.Now;
                    }
                    else
                    {
                        cuentaActualizada.FelEstado = EstadosFEL.Emitida;
                    }
                }
                _cuentasPorCobrarRepository.Update(cuentaActualizada);

                if (cuentaCerrada && string.IsNullOrWhiteSpace(cuentaActualizada.UuidFel))
                {
                    try
                    {
                        var felRequest = BuildFelRequest(
                            model: model,
                            totalBruto: totalBrutoReal,
                            descuentoAplicado: descuentoGlobal,
                            receptorNit: cuenta.FelReceptorNit,
                            receptorNombre: cuenta.FelReceptorNombre,
                            receptorDireccion: cuenta.FelReceptorDireccion,
                            receptorCorreo: cuenta.FelReceptorCorreo
                        );

                        var uuid = await EmitirFelAsync(felRequest);

                        cuentaActualizada.UuidFel = uuid;
                        cuentaActualizada.FelEstado = EstadosFEL.Emitida;
                        cuentaActualizada.FelFechaEmitida = DateTime.Now;
                        cuentaActualizada.FelUltimoError = null;
                        cuentaActualizada.FelFechaUltimoIntento = DateTime.Now;
                        _cuentasPorCobrarRepository.Update(cuentaActualizada);
                    }
                    catch (Exception felEx)
                    {
                        cuentaActualizada.FelEstado = EstadosFEL.Error;
                        cuentaActualizada.FelIntentos = (cuentaActualizada.FelIntentos < 0 ? 0 : cuentaActualizada.FelIntentos) + 1;
                        cuentaActualizada.FelUltimoError = felEx.Message;
                        cuentaActualizada.FelFechaUltimoIntento = DateTime.Now;
                        _cuentasPorCobrarRepository.Update(cuentaActualizada);
                    }
                }

                string mensajeUsuario = cuentaCerrada
                    ? (descuentoGlobal > 0m ? $"¡Pago exitoso! La cuenta ha sido cancelada con un descuento de Q {descuentoGlobal:0.00}." : "¡Pago exitoso! La cuenta ha sido cancelada en su totalidad.")
                    : $"Pago parcial registrado correctamente. Saldo pendiente: Q {saldoRestante:0.00}";

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    CuentaCerrada = cuentaCerrada,
                    TotalCuenta = totalBrutoReal,
                    TotalPagado = nuevoPagadoAcumulado,
                    TotalPagadoAnterior = pagadoAnterior,
                    TotalPagoNuevo = totalPagoNuevo,
                    SaldoRestante = saldoRestante,
                    DescuentoAplicado = descuentoGlobal,
                    PorcentajeDescuento = model.PorcentajeDescuento,
                    Mensaje = mensajeUsuario,
                    FelEstado = cuentaActualizada.FelEstado,
                    UuidFel = cuentaActualizada.UuidFel,
                    FelUltimoError = cuentaActualizada.FelUltimoError,
                    FelIntentos = cuentaActualizada.FelIntentos
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al registrar el pago. " + ex.Message });
            }
        }

        private object BuildFelRequest(
            CuentasPorCobrarPagarViewModel model,
            decimal totalBruto,
            decimal descuentoAplicado,
            string receptorNit,
            string receptorNombre,
            string receptorDireccion,
            string receptorCorreo)
        {
            var granTotalBruto = Math.Round(totalBruto, 2);
            var granTotalNeto = Math.Round(totalBruto - descuentoAplicado, 2);
            var descuento = Math.Round(descuentoAplicado, 2);

            var descripcion = $"Servicio de Hospitalización Paciente: {model.PacienteNombreAdmision ?? model.PacienteNombre ?? "PACIENTE"} (Admision #{model.AdmisionId})";
            var desc = (descripcion ?? string.Empty).ToUpperInvariant();

            var montoGravable = Math.Round(granTotalNeto / 1.12m, 2);
            var montoImpuesto = Math.Round(granTotalNeto - montoGravable, 2);

            return new
            {
                CodigoMoneda = "GTQ",
                FechaHoraEmision = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss-06:00", CultureInfo.InvariantCulture),
                TipoDocumento = "FACT",
                Emisor = new
                {
                    AfiliacionIVA = "GEN",
                    CodigoEstablecimiento = "1",
                    CorreoEmisor = "recepcion@hcq.com",
                    NITEmisor = "117286303",
                    NombreComercial = "HOSPITAL CLINICO QUIRURGICO SANCTI SPIRITUS",
                    NombreEmisor = "HOSPITAL CLINICO QUIRURGICO SANCTI SPIRITUS",
                    Direccion = new
                    {
                        DetalleDireccion = "4 CALLE 1-91 BARRIO SAN SEBASTIAN ZONA 3 SAN CRISTÓBAL VERAPAZ, ALTA VERAPAZ",
                        CodigoPostal = "01011",
                        Municipio = "Guatemala",
                        Departamento = "Guatemala",
                        Pais = "GT"
                    }
                },
                Receptor = new
                {
                    CorreoReceptor = receptorCorreo,
                    IDReceptor = receptorNit,
                    NombreReceptor = receptorNombre,
                    Direccion = new
                    {
                        DetalleDireccion = receptorDireccion,
                        CodigoPostal = "01001",
                        Municipio = "Guatemala",
                        Departamento = "Guatemala",
                        Pais = "GT"
                    }
                },
                Frases = new[] { new { CodigoEscenario = "1", TipoFrase = "1" } },
                Items = new[]
                {
                    new
                    {
                        BienOServicio = "S",
                        NumeroLinea = "1",
                        Cantidad = 1,
                        UnidadMedida = "UND",
                        Descripcion = desc,
                        PrecioUnitario = granTotalBruto,
                        Precio = granTotalBruto,
                        Descuento = descuento,
                        Impuestos = new[]
                        {
                            new
                            {
                                NombreCorto = "IVA",
                                CodigoUnidadGravable = "1",
                                MontoGravable = montoGravable.ToString("0.00", CultureInfo.InvariantCulture),
                                MontoImpuesto = montoImpuesto.ToString("0.00", CultureInfo.InvariantCulture)
                            }
                        },
                        Total = granTotalNeto.ToString("0.00", CultureInfo.InvariantCulture)
                    }
                },
                Totales = new
                {
                    TotalImpuestos = new[]
                    {
                        new
                        {
                            NombreCorto = "IVA",
                            TotalMontoImpuesto = montoImpuesto.ToString("0.00", CultureInfo.InvariantCulture)
                        }
                    },
                    GranTotal = granTotalNeto.ToString("0.00", CultureInfo.InvariantCulture)
                },
                Adenda = "FACTURA-19"
            };
        }

        private async Task<string> EmitirFelAsync(object felRequest)
        {
            if (_felSettings == null)
                throw new Exception("FelSettings no está disponible (inyección/configuración).");

            if (string.IsNullOrWhiteSpace(_felSettings.BaseUrl))
                throw new Exception("Configuración FEL incompleta: Fel:BaseUrl.");

            if (string.IsNullOrWhiteSpace(_felSettings.UsuarioFirma) ||
                string.IsNullOrWhiteSpace(_felSettings.LlaveFirma) ||
                string.IsNullOrWhiteSpace(_felSettings.UsuarioApi) ||
                string.IsNullOrWhiteSpace(_felSettings.LlaveApi))
                throw new Exception("Configuración FEL incompleta: Fel:UsuarioFirma/LlaveFirma/UsuarioApi/LlaveApi.");

            var http = _httpClientFactory.CreateClient();
            http.BaseAddress = new Uri(_felSettings.BaseUrl.TrimEnd('/'));
            http.Timeout = TimeSpan.FromSeconds(_felSettings.TimeoutSeconds > 0 ? _felSettings.TimeoutSeconds : 30);

            var jsonReq = JsonSerializer.Serialize(felRequest);
            using var genContent = new StringContent(jsonReq, Encoding.UTF8, "application/json");
            using var genResp = await http.PostAsync("/Xml/GenerateXml", genContent);
            var genBody = await genResp.Content.ReadAsStringAsync();

            if (!genResp.IsSuccessStatusCode)
                throw new Exception($"FEL GenerateXml error HTTP {(int)genResp.StatusCode}: {genBody}");

            using var genDoc = JsonDocument.Parse(genBody);
            if (!genDoc.RootElement.TryGetProperty("xmlContent", out var xmlEl))
                throw new Exception("FEL GenerateXml no retornó 'xmlContent'.");

            var xmlContent = xmlEl.GetString();
            if (string.IsNullOrWhiteSpace(xmlContent))
                throw new Exception("FEL GenerateXml retornó xmlContent vacío.");

            var sendPayload = new
            {
                XmlContent = xmlContent,
                UsuarioFirma = _felSettings.UsuarioFirma,
                LlaveFirma = _felSettings.LlaveFirma,
                UsuarioApi = _felSettings.UsuarioApi,
                LlaveApi = _felSettings.LlaveApi
            };

            var jsonSend = JsonSerializer.Serialize(sendPayload);
            using var sendContent = new StringContent(jsonSend, Encoding.UTF8, "application/json");
            using var sendResp = await http.PostAsync("/Xml/SendXml", sendContent);
            var sendBody = await sendResp.Content.ReadAsStringAsync();

            if (!sendResp.IsSuccessStatusCode)
                throw new Exception($"FEL SendXml error HTTP {(int)sendResp.StatusCode}: {sendBody}");

            using var sendDoc = JsonDocument.Parse(sendBody);
            if (!sendDoc.RootElement.TryGetProperty("uuid", out var uuidEl))
                throw new Exception("FEL SendXml no retornó 'uuid'.");

            var uuid = uuidEl.GetString();
            if (string.IsNullOrWhiteSpace(uuid))
                throw new Exception("FEL SendXml retornó uuid vacío.");

            return uuid;
        }

        [HttpPost]
        public JsonResult ConsultarCuentasPorCobrar()
        {
            try
            {
                var resultado = _cuentasPorCobrarRepository.GetList().Where(c => !c.Pagada).ToList();
                return Json(new { Exitoso = true, Resultado = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al consultar cuentas por cobrar. " + ex.Message });
            }
        }

        public IActionResult Modificar(int cuentaId)
        {
            var cuenta = _cuentasPorCobrarRepository.Get(cuentaId);
            var model = new CuentasPorCobrarModificarViewModel();
            model.CuentaId = cuenta.Id;
            model.Valor = cuenta.Valor ?? 0;
            model.Observaciones = cuenta.Observaciones;
            return View(model);
        }

        [HttpPost]
        public JsonResult Modificar(CuentasPorCobrarModificarViewModel model)
        {
            try
            {
                var cuenta = _cuentasPorCobrarRepository.Get(model.CuentaId);
                cuenta.Valor = model.Valor;
                cuenta.Observaciones = model.Observaciones;
                _cuentasPorCobrarRepository.Update(cuenta);
                TempData["Message"] = "Se ha modificado la cuenta por cobrar!";
                return Json(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al modificar cuenta. " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<string> ReintentarFel(int cuentaId)
        {
            try
            {
                if (cuentaId <= 0)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "CuentaId inválido." });

                var cuenta = _cuentasPorCobrarRepository.Get(cuentaId);
                if (cuenta == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No se encontró la cuenta por cobrar." });

                if (!cuenta.Pagada)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "La cuenta no está cerrada. No aplica FEL." });

                if (!string.IsNullOrWhiteSpace(cuenta.UuidFel))
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true,
                        Mensaje = "La FEL ya fue emitida.",
                        FelEstado = cuenta.FelEstado,
                        UuidFel = cuenta.UuidFel,
                        FelUltimoError = cuenta.FelUltimoError,
                        FelIntentos = cuenta.FelIntentos
                    });
                }

                if (!(cuenta.FelEstado == EstadosFEL.Pendiente || cuenta.FelEstado == EstadosFEL.Error))
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = $"Estado FEL no permite reintento. Estado actual: {cuenta.FelEstado}"
                    });
                }

                var receptorNit = string.IsNullOrWhiteSpace(cuenta.FelReceptorNit) ? "CF" : cuenta.FelReceptorNit.Trim();
                var receptorNombre = string.IsNullOrWhiteSpace(cuenta.FelReceptorNombre) ? "Consumidor Final" : cuenta.FelReceptorNombre.Trim();
                var receptorDireccion = string.IsNullOrWhiteSpace(cuenta.FelReceptorDireccion) ? "N/A" : cuenta.FelReceptorDireccion.Trim();
                var receptorCorreo = string.IsNullOrWhiteSpace(cuenta.FelReceptorCorreo) ? "sin-correo@example.com" : cuenta.FelReceptorCorreo.Trim();

                var admisionId = cuenta.DetallesCuentaPorCobrar?
                    .FirstOrDefault(d => d.HospitalizacionId.HasValue)?.HospitalizacionId ?? 0;

                var pacienteId = cuenta.PacienteId ?? cuenta.Paciente?.Id ?? 0;
                if (pacienteId <= 0)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "La cuenta no tiene PacienteId asociado. No se puede reintentar FEL." });
                }

                var pacienteNombre = cuenta.Paciente?.Nombre;

                var vm = new CuentasPorCobrarPagarViewModel
                {
                    CuentaId = cuenta.Id,
                    PacienteId = pacienteId,
                    PacienteNombre = pacienteNombre,
                    AdmisionId = admisionId,
                    PacienteNombreAdmision = pacienteNombre
                };

                var montoFacturar = Convert.ToDecimal(cuenta.ValorPagado ?? 0m);

                var felRequest = BuildFelRequest(
                    model: vm,
                    totalBruto: montoFacturar,
                    descuentoAplicado: 0m,
                    receptorNit: receptorNit,
                    receptorNombre: receptorNombre,
                    receptorDireccion: receptorDireccion,
                    receptorCorreo: receptorCorreo
                );

                try
                {
                    var uuid = await EmitirFelAsync(felRequest);

                    cuenta.UuidFel = uuid;
                    cuenta.FelEstado = EstadosFEL.Emitida;
                    cuenta.FelFechaEmitida = DateTime.Now;
                    cuenta.FelUltimoError = null;
                    cuenta.FelFechaUltimoIntento = DateTime.Now;

                    _cuentasPorCobrarRepository.Update(cuenta);

                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true,
                        Mensaje = "FEL emitida correctamente.",
                        FelEstado = cuenta.FelEstado,
                        UuidFel = cuenta.UuidFel,
                        FelUltimoError = cuenta.FelUltimoError,
                        FelIntentos = cuenta.FelIntentos
                    });
                }
                catch (Exception felEx)
                {
                    cuenta.FelEstado = EstadosFEL.Error;
                    cuenta.FelIntentos = (cuenta.FelIntentos < 0 ? 0 : cuenta.FelIntentos) + 1;
                    cuenta.FelUltimoError = felEx.Message;
                    cuenta.FelFechaUltimoIntento = DateTime.Now;

                    _cuentasPorCobrarRepository.Update(cuenta);

                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = true,
                        Mensaje = "No fue posible emitir FEL. Queda pendiente para reintento.",
                        FelEstado = cuenta.FelEstado,
                        UuidFel = cuenta.UuidFel,
                        FelUltimoError = cuenta.FelUltimoError,
                        FelIntentos = cuenta.FelIntentos
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error al reintentar FEL. " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ConsultarCuentasPagadas()
        {
            try
            {
                var resultado = _cuentasPorCobrarRepository.GetList().Where(c => c.Pagada).ToList();
                return Json(new { Exitoso = true, Resultado = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al consultar cuentas pagadas. " + ex.Message });
            }
        }

        [HttpPost]
        public string ConsultarMedicamentosNoPagadosHospitalizaciones(int cuentaId)
        {
            try
            {
                var medNoPagados = _cuentasPorCobrarService.GetMedicamentosNoPagadosHospitalizaciones(cuentaId);
                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = medNoPagados });
            }
            catch
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error de conexion al consultar medicamentos de hospitalizacion" });
            }
        }

        [HttpPost]
        public string ConsultarPaquetesNoPagados(int cuentaId)
        {
            try
            {
                var paquetesNoPagados = _cuentasPorCobrarService.GetPaquetesNoPagadosHospitalizacion(cuentaId);
                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = paquetesNoPagados });
            }
            catch
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Error de conexion al consultar paquetes de hospitalizacion" });
            }
        }

        [HttpPost]
        public async Task<string> GuardarHonorarioManual(HospitalizacionHonorario model)
        {
            try
            {
                if (model.HospitalizacionId <= 0 || model.EmpleadoId <= 0 || model.Monto <= 0)
                {
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Datos del honorario inválidos." });
                }

                _context.HospitalizacionHonorarios.Add(model);
                await _context.SaveChangesAsync();

                return JsonSerializer.Serialize(new { Exitoso = true, Mensaje = "Honorario agregado correctamente." });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<string> EliminarHonorarioManual(int id)
        {
            try
            {
                var honorario = await _context.HospitalizacionHonorarios.FindAsync(id);
                if (honorario == null) return JsonSerializer.Serialize(new { Exitoso = false });

                _context.HospitalizacionHonorarios.Remove(honorario);
                await _context.SaveChangesAsync();

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
            }
        }

        [HttpPost]
        public string ConsultarHonorariosHospitalizacion(int hospitalizacionId)
        {
            try
            {
                var lista = _context.HospitalizacionHonorarios
                    .Where(h => h.HospitalizacionId == hospitalizacionId)
                    .Select(h => new
                    {
                        h.Id,
                        h.EmpleadoId,
                        NombreMedico = h.Empleado.NombreYApellidos,
                        h.Monto
                    }).ToList();

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = lista });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
            }
        }

        public string ConsultarGastosAdministrativos(int hospitalizacionId)
        {
            try
            {
                var lista = _context.HospitalizacionGastosAdministrativos
                    .Where(g => g.HospitalizacionId == hospitalizacionId)
                    .Select(g => new
                    {
                        g.Id,
                        g.HospitalizacionId,
                        g.PorcentajeAplicado,
                        g.Monto,
                        FechaHora = g.FechaHora.ToString("dd/MM/yyyy HH:mm")
                    })
                    .ToList();

                return JsonSerializer.Serialize(new { Exitoso = true, Resultado = lista });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<string> GuardarGastoAdministrativo(int hospitalizacionId, decimal porcentajeAplicado, decimal monto)
        {
            try
            {
                if (hospitalizacionId <= 0 || porcentajeAplicado <= 0 || monto <= 0)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Datos inválidos. Verifique el porcentaje y el monto." });

                if (porcentajeAplicado > 100)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "El porcentaje no puede ser mayor a 100." });

                var gasto = new HospitalizacionGastoAdministrativo
                {
                    HospitalizacionId = hospitalizacionId,
                    PorcentajeAplicado = Math.Round(porcentajeAplicado, 4),
                    Monto = Math.Round(monto, 2),
                    FechaHora = DateTime.Now
                };

                _context.HospitalizacionGastosAdministrativos.Add(gasto);
                await _context.SaveChangesAsync();

                return JsonSerializer.Serialize(new { Exitoso = true, Mensaje = "Gasto administrativo guardado correctamente.", Id = gasto.Id });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<string> EliminarGastoAdministrativo(int id)
        {
            try
            {
                var gasto = await _context.HospitalizacionGastosAdministrativos.FindAsync(id);
                if (gasto == null)
                    return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No se encontró el gasto administrativo." });

                _context.HospitalizacionGastosAdministrativos.Remove(gasto);
                await _context.SaveChangesAsync();

                return JsonSerializer.Serialize(new { Exitoso = true });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
            }
        }

        private CuentasPorCobrarPagarViewModel CreateEmptyPagarViewModel(string mensaje = null)
        {
            var model = new CuentasPorCobrarPagarViewModel();
            model.Init(_cuentasPorCobrarRepository, _empleadoRepository);
            if (!string.IsNullOrWhiteSpace(mensaje))
                TempData["Message"] = mensaje;
            return model;
        }

        private static int ResolverAdmisionIdDesdeCuenta(CuentaPorCobrar cuenta)
        {
            return cuenta?.DetallesCuentaPorCobrar?
                .FirstOrDefault(d => d.HospitalizacionId.HasValue && d.HospitalizacionId.Value > 0)?
                .HospitalizacionId ?? 0;
        }
    }
}