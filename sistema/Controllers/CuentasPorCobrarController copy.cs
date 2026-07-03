// using System.ComponentModel.Design;
// using System.IO.MemoryMappedFiles;
// using System.Linq.Expressions;
// using System.Reflection;
// using System;
// using Microsoft.AspNetCore.Mvc;
// using System.Threading.Tasks;
// using System.Collections.Generic;
// using System.Linq;
// using sistema.Models;
// using Database.Shared.IRepository;
// using Database.Shared.Models;
// using Microsoft.AspNetCore.Http;
// using sistema.Json;
// using Wkhtmltopdf.NetCore;
// using ClosedXML.Excel;
// using System.IO;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Rotativa.AspNetCore;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using Database.Shared.Enumeraciones;
// using farmamest.Service.IService;
// using sistema.Service.IService;
// using System.Text.Encodings.Web;
// using Database.Shared;
// using Microsoft.EntityFrameworkCore;
// using System.Text;
// using System.Net.Http;
// using Microsoft.Extensions.Options;
// using System.Globalization;
// using farmamest;

// namespace sistema.Controllers
// {
//     [Authorize]
//     public class CuentasPorCobrarController : Controller
//     {
//         private readonly ICuentasPorCobrar _cuentasPorCobrarRepository = null;
//         private readonly ICaja _cajaRepository = null;
//         private readonly IPacientes _pacientesRepository = null;
//         private readonly IHospitalizacion _hospitalizacionRepository = null;
//         private readonly IHabitacion _habitacionRepository = null;
//         private readonly IEnvio _envioRepository = null;
//         private readonly IEmpleado _empleadoRepository = null;
//         private readonly UserManager<User> _userManager = null;
//         private readonly ICuentasPorCobrarService _cuentasPorCobrarService;
//         private readonly IConsultas _consultasRepository = null;
//         private readonly IHospitalizacionService _hospitalizacionService;
//         private readonly Context _context;
//         private readonly IHttpClientFactory _httpClientFactory;
//         private readonly FelSettings _felSettings;

//         public CuentasPorCobrarController(
//             ICuentasPorCobrar cuentasPorCobrarRepository,
//             ICaja cajaRepository,
//             IPacientes pacientesRepository,
//             IHospitalizacion hospitalizacionRepository,
//             IHabitacion habitacionRepository,
//             IEnvio envioRepository,
//             IEmpleado empleadoRepository,
//             UserManager<User> userManager,
//             ICuentasPorCobrarService cuentasPorCobrarService,
//             IConsultas consultasRepository,
//             IHospitalizacionService hospitalizacionService,
//             Context context,
//             IHttpClientFactory httpClientFactory,
//             IOptions<FelSettings> felOptions
//             )
//         {
//             _cuentasPorCobrarRepository = cuentasPorCobrarRepository;
//             _cajaRepository = cajaRepository;
//             _pacientesRepository = pacientesRepository;
//             _hospitalizacionRepository = hospitalizacionRepository;
//             _habitacionRepository = habitacionRepository;
//             _envioRepository = envioRepository;
//             _empleadoRepository = empleadoRepository;
//             _userManager = userManager;
//             _cuentasPorCobrarService = cuentasPorCobrarService;
//             _consultasRepository = consultasRepository;
//             _hospitalizacionService = hospitalizacionService;
//             _context = context;
//             _httpClientFactory = httpClientFactory;
//             _felSettings = felOptions?.Value ?? new FelSettings();
//         }

//         public IActionResult Pendientes()
//         {
//             return View();
//         }

//         public IActionResult Pagadas()
//         {
//             return View();
//         }

//         public IActionResult VerDetallesCuenta(int? cuentaId)
//         {
//             if (cuentaId == null)
//                 return RedirectToAction("Pendientes");

//             var cuenta = _cuentasPorCobrarService.GetDetalleCuentaPorCobrar((int)cuentaId);
//             cuenta.CuentaId = (int)cuentaId;

//             if (cuenta.CuentasPorCobrar == null)
//                 return RedirectToAction("Pendientes");

//             return View(cuenta);
//         }

//         public async Task<IActionResult> Pagar(
//             [FromQuery] int? cuentaId,
//             [FromQuery] string? ResponsableNit,
//             [FromQuery] string? ResponsableNombre,
//             [FromQuery] string? ResponsableDireccion,
//             [FromQuery] string? ResponsableCorreo,
//             [FromQuery] string? SeguroNombre,
//             [FromQuery] string? PacienteNombreAdmision,
//             [FromQuery] int AdmisionId)
//         {
//             Console.WriteLine("=== [GET] CuentasPorCobrar/Pagar ===");
//             Console.WriteLine($"QueryParams -> cuentaId: {(cuentaId.HasValue ? cuentaId.Value.ToString() : "NULL")}, AdmisionId: {AdmisionId}");
//             Console.WriteLine($"QueryParams -> ResponsableNit: {ResponsableNit ?? "NULL"}, ResponsableNombre: {ResponsableNombre ?? "NULL"}");
//             Console.WriteLine($"QueryParams -> ResponsableDireccion: {ResponsableDireccion ?? "NULL"}, ResponsableCorreo: {ResponsableCorreo ?? "NULL"}");
//             Console.WriteLine($"QueryParams -> SeguroNombre: {SeguroNombre ?? "NULL"}, PacienteNombreAdmision: {PacienteNombreAdmision ?? "NULL"}");

//             if (cuentaId == null)
//             {
//                 Console.WriteLine("Salida -> cuentaId NULL, redirect Pendientes");
//                 return RedirectToAction("Pendientes");
//             }

//             var cuenta = _cuentasPorCobrarRepository.Get((int)cuentaId);
//             if (cuenta == null)
//             {
//                 Console.WriteLine($"Salida -> Cuenta NULL para cuentaId: {cuentaId.Value}, redirect Pendientes");
//                 return RedirectToAction("Pendientes");
//             }

//             Console.WriteLine($"Cuenta -> Id: {cuenta.Id}, Valor(antes): {(cuenta.Valor.HasValue ? cuenta.Valor.Value.ToString("0.00") : "NULL")}");

//             var paciente = cuenta.Paciente;
//             if (paciente == null)
//             {
//                 Console.WriteLine("Paciente -> NULL (cuenta.Paciente). Se devuelve vista con VM vacío.");
//                 return View(new CuentasPorCobrarPagarViewModel());
//             }

//             Console.WriteLine($"Paciente -> Id: {paciente.Id}, Nombre: {paciente.Nombre ?? "NULL"}");

//             #region Consulta hospitalizacion

//             Console.WriteLine("=== Consulta hospitalizacion ===");
//             Console.WriteLine($"AdmisionId (HospitalizacionId) recibido: {AdmisionId}");

//             if (AdmisionId <= 0)
//             {
//                 Console.WriteLine("AdmisionId inválido (<= 0). Se devuelve vista con VM vacío.");
//                 return View(new CuentasPorCobrarPagarViewModel());
//             }

//             bool cuentaTieneEsaHospitalizacion =
//                 cuenta.DetallesCuentaPorCobrar != null
//                 && cuenta.DetallesCuentaPorCobrar.Any(d => d.HospitalizacionId.HasValue && d.HospitalizacionId.Value == AdmisionId);

//             Console.WriteLine($"Validación -> cuentaTieneEsaHospitalizacion: {cuentaTieneEsaHospitalizacion}");

//             if (!cuentaTieneEsaHospitalizacion)
//             {
//                 Console.WriteLine("Salida -> La cuenta no contiene la hospitalización indicada. redirect Pendientes");
//                 return RedirectToAction("Pendientes");
//             }

//             var hospitalizacion = _hospitalizacionRepository.Get(AdmisionId, true, true, false, true, false);
//             if (hospitalizacion == null)
//             {
//                 Console.WriteLine($"Hospitalizacion -> NULL para AdmisionId: {AdmisionId}. Se devuelve VM vacío.");
//                 return View(new CuentasPorCobrarPagarViewModel());
//             }

//             Console.WriteLine($"Hospitalizacion -> Id: {hospitalizacion.Id}, PacienteId: {hospitalizacion.PacienteId}, HabitacionId: {hospitalizacion.HabitacionId}");
//             Console.WriteLine($"Hospitalizacion -> FechaInicio: {hospitalizacion.FechaInicio:yyyy-MM-dd HH:mm:ss}, FechaFin: {hospitalizacion.FechaFin:yyyy-MM-dd HH:mm:ss}");

//             var registrosBd = _pacientesRepository.GetHospitalizaciones(paciente.Id)
//                 .Where(b => !b.Pagada && !b.Finalizada)
//                 .ToList();

//             decimal totalHospitalizacionCalculado = 0m;
//             DateTime fechaFinDate = DateTime.Now.Date;

//             if (registrosBd != null && registrosBd.Any())
//             {
//                 foreach (var registro in registrosBd)
//                 {
//                     if (registro == null) continue;

//                     var fechaInicio = registro.FechaInicio.Date;
//                     var nochesReg = (fechaInicio == fechaFinDate) ? 1 : (fechaFinDate - fechaInicio).Days;

//                     var valorTarifa = Convert.ToDecimal(registro.CategoriaHabitacionTarifa?.ValorTarifa ?? 0);
//                     var precioHospitalizacion = valorTarifa * nochesReg;

//                     totalHospitalizacionCalculado += precioHospitalizacion;

//                     if (registro.Id == AdmisionId)
//                     {
//                         Console.WriteLine($"[HOSP FRONT] Registro actual -> FechaInicio: {fechaInicio:yyyy-MM-dd}, Noches: {nochesReg}, ValorTarifa: {valorTarifa:0.00}, Subtotal: {precioHospitalizacion:0.00}");
//                     }
//                 }
//             }

//             totalHospitalizacionCalculado = Math.Round(totalHospitalizacionCalculado, 2);
//             Console.WriteLine($"[HOSP FRONT] Registros activos: {(registrosBd?.Count ?? 0)} | TotalHospitalizacionCalculado: {totalHospitalizacionCalculado:0.00}");

//             #endregion

//             if (hospitalizacion.Habitacion == null)
//             {
//                 Console.WriteLine("Hospitalizacion.Habitacion -> NULL. Se devuelve VM vacío.");
//                 return View(new CuentasPorCobrarPagarViewModel());
//             }

//             if (hospitalizacion.Habitacion.CategoriaHabitacion == null)
//             {
//                 Console.WriteLine("Hospitalizacion.Habitacion.CategoriaHabitacion -> NULL. Se devuelve VM vacío.");
//                 return View(new CuentasPorCobrarPagarViewModel());
//             }

//             var habitacionViewModel = new HabitacionPagarViewModel
//             {
//                 Id = hospitalizacion.Habitacion.Id,
//                 CategoriaId = hospitalizacion.Habitacion.CategoriaHabitacionId,
//                 NombreHabitacion = hospitalizacion.Habitacion.NombreNumeroHabitacion,
//                 NombreCategoriaHabitacion = hospitalizacion.Habitacion.CategoriaHabitacion.NombreCategoria,
//                 CostoTotal = totalHospitalizacionCalculado
//             };

//             var totalHabitacion = habitacionViewModel.CostoTotal;

//             int calcularEdad(DateTime? fechaNacimiento)
//             {
//                 if (fechaNacimiento == null)
//                     return 0;

//                 var today = DateTime.Today;
//                 var edad = today.Year - fechaNacimiento.Value.Year;

//                 if (fechaNacimiento.Value.Date > today.AddYears(-edad)) edad--;

//                 return edad;
//             }

//             var consulta = _consultasRepository.GetConsultaPorHospitalizacion((int)hospitalizacion.Id);

//             var pacienteInfo = new
//             {
//                 Nombre = paciente.Nombre ?? "No registrado",
//                 Telefono = paciente.Telefono ?? "No disponible",
//                 Celular = paciente.Celular ?? "No disponible",
//                 Direccion = paciente.Direccion ?? "No registrada",
//                 Nit = paciente.Nit ?? "No disponible",
//                 Dpi = paciente.Dpi ?? "No disponible",
//                 FechaNacimiento = paciente.FechaNacimiento?.ToString("dd/MM/yyyy") ?? "No registrada",
//                 Edad = calcularEdad(paciente.FechaNacimiento).ToString() ?? "No especificada",
//                 Sexo = paciente.Sexo?.DescripcionSexo ?? "No definido",

//                 FechaInicioHops = hospitalizacion.FechaInicio.ToString("dd/MM/yyyy"),
//                 MedicoHops = consulta?.Citas?.Empleado?.NombreYApellidos ?? "No se encontró ningún profesional.",
//                 NombreDelSeguro = consulta?.Citas?.CodigoDeCita
//             };

//             var paquetesHospitalizacion = hospitalizacion.HospitalizacionesPaquetesHospitalizacion
//                 .Where(p => !p.Eliminado)
//                 .Select(p => new PaquetePagarViewModel
//                 {
//                     Id = p.PaqueteHospitalizacionId,
//                     Nombre = p.PaqueteHospitalizacion.NombrePaquete,
//                     Precio = p.PaqueteHospitalizacion.Precio ?? 0,
//                     Tipo = "Paquete de Hospitalización"
//                 }).ToList();

//             Console.WriteLine("📦 Paquetes de Hospitalización:");
//             foreach (var paquete in paquetesHospitalizacion)
//             {
//                 Console.WriteLine($"ID: {paquete.Id}, Nombre: {paquete.Nombre}, Precio: {paquete.Precio}, Tipo: {paquete.Tipo}");
//             }
//             var totalPaquetes = paquetesHospitalizacion.Sum(p => p.Precio);
//             Console.WriteLine($"Total -> Paquetes: {totalPaquetes:0.00}");

//             var productosConPrecios = hospitalizacion.HospitalizacionesProductos
//                 .Where(p => p?.HospitalizacionesProductosAplicaciones?.Any(a => a.Aplicado) ?? false)
//                 .Select(p =>
//                 {
//                     int tipoProductoId = p.Producto?.TipoProductoId ?? 0;

//                     string nombreTipoProducto = tipoProductoId switch
//                     {
//                         (int)TipoProductoEnum.InsumosMedicos => "Insumos médicos",
//                         (int)TipoProductoEnum.Medicamentos => "Medicamentos",
//                         (int)TipoProductoEnum.EquiposMedicos => "Equipos médicos",
//                         _ => "Sin tipo"
//                     };

//                     var aplicacionesAplicadas = p.HospitalizacionesProductosAplicaciones.Where(a => a.Aplicado == true).ToList();
//                     var cantidadAplicada = aplicacionesAplicadas.Sum(a => a.Cantidad);

//                     return new ProductoPagarViewModel
//                     {
//                         Id = p.Id,
//                         ProductoId = p.Producto?.Id ?? 0,
//                         Nombre = p.Producto?.NombreProducto ?? "Desconocido",
//                         Cantidad = cantidadAplicada,
//                         PrecioUnitario = Math.Round(p.PrecioValor, 2),
//                         Subtotal = Math.Round(cantidadAplicada * p.PrecioValor, 2),
//                         Tipo = nombreTipoProducto,
//                         FechaAplicacion = aplicacionesAplicadas
//                             .OrderBy(a => a.FechaHoraAplicacion)
//                             .FirstOrDefault()?.FechaAplicacionFormateada ?? "Sin fecha"
//                     };
//                 })
//                 .ToList();

//             Console.WriteLine("=== Servicios -> Inicio carga/cálculo (vía _hospitalizacionService.GetServiciosHospitalizacion) ===");

//             List<HospitalizacionServicioViewModel> serviciosDetalleList;
//             var serviciosDetalle = _hospitalizacionService.GetServiciosHospitalizacion(hospitalizacion.Id);

//             if (serviciosDetalle == null)
//                 serviciosDetalleList = new List<HospitalizacionServicioViewModel>();
//             else
//                 serviciosDetalleList = serviciosDetalle.ToList();

//             Console.WriteLine($"Servicios -> registros devueltos por GetServiciosHospitalizacion: {serviciosDetalleList.Count}");

//             // ============================================================
//             // INSUMOS/MEDICAMENTOS DIRECTOS
//             // ============================================================
//             var insumosDirectosVM = new List<ProductoPagarViewModel>();
//             decimal totalInsumosDirectos = 0m;

//             // try
//             // {
//             //     var insumosDirectos = await _context.HospitalizacionInsumosDirectos
//             //         .Where(i => i.HospitalizacionId == AdmisionId && !i.Eliminado)
//             //         .Include(i => i.Producto)
//             //         .ToListAsync();

//             //     foreach (var insumo in insumosDirectos)
//             //     {
//             //         int tipoProductoId = insumo.Producto?.TipoProductoId ?? 0;

//             //         string nombreTipoProducto = tipoProductoId switch
//             //         {
//             //             (int)TipoProductoEnum.InsumosMedicos => "Insumos médicos",
//             //             (int)TipoProductoEnum.Medicamentos => "Medicamentos",
//             //             (int)TipoProductoEnum.EquiposMedicos => "Equipos médicos",
//             //             _ => "Medicamentos"
//             //         };

//             //         var subtotal = Math.Round(insumo.Cantidad * insumo.PrecioValor, 2);
//             //         totalInsumosDirectos += subtotal;

//             //         insumosDirectosVM.Add(new ProductoPagarViewModel
//             //         {
//             //             Id = insumo.Id,
//             //             ProductoId = insumo.ProductoId,
//             //             Nombre = insumo.Producto?.NombreProducto ?? "Medicamento",
//             //             Cantidad = (int)insumo.Cantidad,
//             //             PrecioUnitario = Math.Round(insumo.PrecioValor, 2),
//             //             Subtotal = subtotal,
//             //             Tipo = nombreTipoProducto,
//             //             FechaAplicacion = insumo.FechaHoraAplicacionManual ?? "Sin fecha"
//             //         });
//             //     }
//             // }
//             // catch (Exception ex)
//             // {
//             //     Console.WriteLine($"Error al obtener insumos directos: {ex.Message}");
//             // }

//             try
//             {
//                 var insumosDirectos = await _context.HospitalizacionInsumosDirectos
//                     .Where(i => i.HospitalizacionId == AdmisionId && !i.Eliminado)
//                     .Include(i => i.Producto)
//                     .Include(i => i.Aplicaciones) // Se incluye la relación con la tabla hija
//                     .ToListAsync();

//                 foreach (var insumo in insumosDirectos)
//                 {
//                     // Se filtran solo las aplicaciones que tengan Aplicado == true
//                     var aplicacionesAplicadas = insumo.Aplicaciones?
//                         .Where(a => a.Aplicado == true)
//                         .ToList();

//                     // Si no hay aplicaciones válidas, se omite este insumo
//                     if (aplicacionesAplicadas == null || !aplicacionesAplicadas.Any())
//                         continue;

//                     // Se suma la cantidad solo de las aplicaciones confirmadas
//                     int cantidadAplicada = aplicacionesAplicadas.Sum(a => a.Cantidad);

//                     if (cantidadAplicada <= 0)
//                         continue;

//                     int tipoProductoId = insumo.Producto?.TipoProductoId ?? 0;

//                     string nombreTipoProducto = tipoProductoId switch
//                     {
//                         (int)TipoProductoEnum.InsumosMedicos => "Insumos médicos",
//                         (int)TipoProductoEnum.Medicamentos => "Medicamentos",
//                         (int)TipoProductoEnum.EquiposMedicos => "Equipos médicos",
//                         _ => "Medicamentos"
//                     };

//                     // Se calcula el subtotal usando la cantidad aplicada real
//                     var subtotal = Math.Round(cantidadAplicada * insumo.PrecioValor, 2);
//                     totalInsumosDirectos += subtotal;

//                     insumosDirectosVM.Add(new ProductoPagarViewModel
//                     {
//                         Id = insumo.Id,
//                         ProductoId = insumo.ProductoId,
//                         Nombre = insumo.Producto?.NombreProducto ?? "Medicamento",
//                         Cantidad = cantidadAplicada, // Se asigna la cantidad calculada
//                         PrecioUnitario = Math.Round(insumo.PrecioValor, 2),
//                         Subtotal = subtotal,
//                         Tipo = nombreTipoProducto,
//                         FechaAplicacion = insumo.FechaHoraAplicacionManual ?? "Sin fecha" // Se mantiene tu asignación original
//                     });
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Error al obtener insumos directos: {ex.Message}");
//             }

//             Console.WriteLine($"Total -> Insumos Directos: {totalInsumosDirectos:0.00}");



//             foreach (var s in serviciosDetalleList)
//             {
//                 Console.WriteLine($"Servicio -> Id: {s.Id}, Nombre: {s.Nombre}, Cantidad: {s.Cantidad}, Precio: {s.Precio}, Subtotal: {s.Subtotal}");
//             }

//             var serviciosConPrecios = serviciosDetalleList
//      .Select(s =>
//      {
//          var cantidad = Convert.ToDecimal(s.Cantidad);
//          var precio = Convert.ToDecimal(s.Precio);
//          var subtotal = Convert.ToDecimal(s.Subtotal);

//          // Parseo seguro de fecha usando la cultura correcta
//          string fechaParseada = "Sin fecha";
//          if (!string.IsNullOrEmpty(s.FechaHoraAplicacion))
//          {
//              if (DateTime.TryParse(s.FechaHoraAplicacion, new CultureInfo("es-GT"), DateTimeStyles.None, out var date))
//              {
//                  fechaParseada = date.ToString("dd/MM/yyyy");
//              }
//              else
//              {
//                  // Si el parseo falla por alguna razón anómala, devuelve el string original en lugar de romper la app
//                  fechaParseada = s.FechaHoraAplicacion;
//              }
//          }

//          return new ProductoPagarViewModel
//          {
//              Id = s.Id,
//              ProductoId = s.Id,
//              Nombre = s.Nombre ?? "Servicio sin nombre",
//              Cantidad = (int)Math.Round(cantidad, 0),
//              PrecioUnitario = Math.Round(precio, 2),
//              Subtotal = Math.Round(subtotal, 2),
//              Tipo = "Servicio",
//              FechaAplicacion = fechaParseada
//          };
//      })
//      .ToList();

//             var totalServicios = serviciosConPrecios.Sum(x => x.Subtotal);
//             Console.WriteLine($"Servicios -> Total items mapeados a VM: {serviciosConPrecios.Count}");
//             Console.WriteLine($"Servicios -> Total subtotal sumado: {totalServicios:0.00}");
//             Console.WriteLine("=== Servicios -> Fin carga/cálculo ===");

//             // var examenesConPrecios = _hospitalizacionRepository.GetExamenes(hospitalizacion.Id)
//             //     .Select(e =>
//             //     {
//             //         var detalleExamen = e.Examen?.DetalleExamenes?.FirstOrDefault();
//             //         var examenLabClinico = detalleExamen?.ExamenLabClinico;
//             //         var categoriaLabClinico = examenLabClinico?.CategoriaLabClinico;
//             //         var nombreCategoria = categoriaLabClinico?.Nombre ?? "Sin categoría";

//             //         var precio = detalleExamen?.PrecioValor ?? 0;

//             //         return new ProductoPagarViewModel
//             //         {
//             //             Id = e.Id,
//             //             ProductoId = e.ExamenId,
//             //             Nombre = examenLabClinico?.NombreExamen ?? "Examen sin nombre",
//             //             Cantidad = 1,
//             //             PrecioUnitario = Math.Round(Convert.ToDecimal(precio), 2),
//             //             Subtotal = Math.Round(Convert.ToDecimal(precio), 2),
//             //             Tipo = nombreCategoria,
//             //             FechaAplicacion = e.FechaAplicacionFormateada
//             //         };
//             //     }).ToList();



//             var examenesConPrecios = _hospitalizacionRepository.GetExamenes(hospitalizacion.Id)
//                 .SelectMany(e => e.Examen?.DetalleExamenes ?? Enumerable.Empty<DetalleExamen>(),
//                     (e, detalleExamen) => new { e, detalleExamen })
//                 .Select(x =>
//                 {
//                     var examenLabClinico = x.detalleExamen?.ExamenLabClinico;
//                     var categoriaLabClinico = examenLabClinico?.CategoriaLabClinico;
//                     var nombreCategoria = categoriaLabClinico?.Nombre ?? "Sin categoría";
//                     var precio = x.detalleExamen?.PrecioValor ?? 0;

//                     return new ProductoPagarViewModel
//                     {
//                         Id = x.e.Id,
//                         ProductoId = x.e.ExamenId,
//                         Nombre = examenLabClinico?.NombreExamen ?? "Examen sin nombre",
//                         Cantidad = 1,
//                         PrecioUnitario = Math.Round(Convert.ToDecimal(precio), 2),
//                         Subtotal = Math.Round(Convert.ToDecimal(precio), 2),
//                         Tipo = nombreCategoria,
//                         FechaAplicacion = x.e.FechaAplicacionFormateada
//                     };
//                 }).ToList();
//             var dietasConPrecios = new List<ProductoPagarViewModel>();
//             var recetasBd = _hospitalizacionRepository.GetHospitalizacionRecetaByIdHospitalizacion(hospitalizacion.Id);

//             if (recetasBd != null)
//             {
//                 foreach (var hr in recetasBd)
//                 {
//                     if (hr?.Receta == null) continue;

//                     var cantidad = Convert.ToDecimal(hr.Cantidad);
//                     var precioVenta = Convert.ToDecimal(hr.Receta.PrecioVenta);
//                     if (cantidad <= 0 || precioVenta <= 0) continue;

//                     var subtotal = Math.Round(cantidad * precioVenta, 2);

//                     dietasConPrecios.Add(new ProductoPagarViewModel
//                     {
//                         Id = hr.Id,
//                         ProductoId = hr.Receta.Id,
//                         Nombre = hr.Receta.NombreReceta ?? "Dieta",
//                         Cantidad = (int)Math.Round(cantidad, 0),
//                         PrecioUnitario = Math.Round(precioVenta, 2),
//                         Subtotal = subtotal,
//                         Tipo = "Dietas",
//                         FechaAplicacion = "Sin fecha"
//                     });
//                 }
//             }

//             var totalDietas = dietasConPrecios.Sum(d => d.Subtotal);
//             Console.WriteLine($"Total -> Dietas: {totalDietas:0.00}");

//             var Ambulancias = await _context.Ambulancias
//                 .Where(a => a.HospitalizacionId == AdmisionId)
//                 .Select(a => new { a.Id, a.TipoViaje, a.Precio })
//                 .ToListAsync();

//             var ambulanciasVM = Ambulancias.Select(a => new AmbulanciaPagarViewModel
//             {
//                 Id = a.Id,
//                 TipoTraslado = a.TipoViaje,
//                 Precio = a.Precio
//             }).ToList();

//             Console.WriteLine("🚑 Ambulancias:");
//             foreach (var a in ambulanciasVM)
//                 Console.WriteLine($"ID: {a.Id}, Tipo: {a.TipoTraslado}, Precio: {a.Precio:0.00}");

//             var totalAmbulancias = ambulanciasVM.Sum(a => a.Precio);
//             Console.WriteLine($"Total -> Ambulancias: {totalAmbulancias:0.00}");

//             decimal totalDepositos = 0m;
//             try
//             {
//                 var pagos = _cuentasPorCobrarRepository.GetPagos(cuenta.Id);
//                 if (pagos != null)
//                     totalDepositos = Math.Round(pagos.Sum(p => Convert.ToDecimal(p.Monto)), 2);
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"AVISO -> No se pudo calcular depósitos por GetPagos. Ex: {ex.GetType().Name} - {ex.Message}");
//                 totalDepositos = 0m;
//             }

//             var depositosComoProducto = new List<ProductoPagarViewModel>();
//             if (totalDepositos > 0m)
//             {
//                 depositosComoProducto.Add(new ProductoPagarViewModel
//                 {
//                     Id = 0,
//                     ProductoId = 0,
//                     Nombre = "Depósitos (resta)",
//                     Cantidad = 1,
//                     PrecioUnitario = Math.Round(-totalDepositos, 2),
//                     Subtotal = Math.Round(-totalDepositos, 2),
//                     Tipo = "Depósitos (resta)",
//                     FechaAplicacion = "Sin fecha"
//                 });
//             }

//             Console.WriteLine($"Total -> Depósitos (resta): {-totalDepositos:0.00}");


//             Console.WriteLine(" Emergencias - Inicio carga");

//             decimal totalEmergencias = 0m;
//             var emergenciasVM = new List<ProductoPagarViewModel>();

//             try
//             {
//                 var emergencia = await _context.Emergencias
//                     .FirstOrDefaultAsync(e => e.HospitalizacionId == AdmisionId);

//                 if (emergencia != null)
//                 {
//                     int emergenciaId = emergencia.Id;
//                     Console.WriteLine($"Emergencia encontrada ID: {emergenciaId}");

//                     string fechaEmergencia = emergencia.FechaEmergencia.ToString("dd/MM/yyyy");

//                     var detallesEmergencia = await _context.EmergenciaDetalles
//                         .Where(d => d.EmergenciaId == emergenciaId && d.Eliminado != true)
//                         .Include(d => d.Producto)
//                         .Include(d => d.Servicio)
//                         .Include(d => d.ExamenLabClinico)
//                         .ToListAsync();

//                     if (detallesEmergencia != null && detallesEmergencia.Any())
//                     {
//                         Console.WriteLine($"Detalles de emergencia encontrados: {detallesEmergencia.Count}");

//                         foreach (var detalle in detallesEmergencia)
//                         {
//                             decimal cantidad = detalle.Cantidad;
//                             decimal valorUnitario = detalle.PrecioValor;
//                             decimal descuento = detalle.DescuentoPorcentaje;

//                             decimal subtotal = cantidad * valorUnitario;
//                             decimal descuentoValor = subtotal * (descuento / 100);
//                             decimal total = subtotal - descuentoValor;

//                             totalEmergencias += total;

//                             string nombreItem = detalle.Producto?.NombreProducto
//                                 ?? detalle.Servicio?.NombreServicio
//                                 ?? detalle.ExamenLabClinico?.NombreExamen
//                                 ?? "Item de emergencia";

//                             string fechaItem = fechaEmergencia;

//                             emergenciasVM.Add(new ProductoPagarViewModel
//                             {
//                                 Id = detalle.Id,
//                                 ProductoId = detalle.ProductoId ?? detalle.ServicioId ?? detalle.ExamenLabClinicoId ?? 0,
//                                 Nombre = nombreItem,
//                                 Cantidad = (int)cantidad,
//                                 PrecioUnitario = Math.Round(valorUnitario, 2),
//                                 Subtotal = Math.Round(total, 2),
//                                 Tipo = "Emergencia",
//                                 FechaAplicacion = fechaItem
//                             });
//                         }
//                     }
//                 }
//                 else
//                 {
//                     Console.WriteLine("ℹNo hay emergencia asociada a esta hospitalización");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Error al obtener emergencias: {ex.Message}");
//             }

//             Console.WriteLine($"Total emergencias calculado: {totalEmergencias:0.00}");
//             // ============================================================

//             decimal totalHonorariosBD = 0m;
//             try
//             {
//                 var honorariosDb = _context.HospitalizacionHonorarios
//                     .Where(h => h.HospitalizacionId == AdmisionId)
//                     .ToList();
//                 totalHonorariosBD = honorariosDb.Sum(h => h.Monto);
//                 Console.WriteLine($"Total -> Honorarios: {totalHonorariosBD:0.00}");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Error al obtener honorarios para el cálculo inicial: {ex.Message}");
//             }

//             var totalProductos = productosConPrecios.Sum(p => p.Subtotal);
//             var totalExamenes = examenesConPrecios.Sum(e => e.Subtotal);

//             var valorTotalCalculado =
//                 totalHabitacion
//                 + totalProductos
//                 + totalServicios
//                 + totalExamenes
//                 + totalDietas
//                 + totalPaquetes
//                 + totalAmbulancias
//                 + totalEmergencias
//                 + totalInsumosDirectos
//                 + totalHonorariosBD
//                 - totalDepositos;

//             Console.WriteLine($"Total -> Habitacion/Hospitalización (FRONT): {totalHabitacion:0.00}");
//             Console.WriteLine($"Total -> Productos: {totalProductos:0.00}");
//             Console.WriteLine($"Total -> Servicios: {totalServicios:0.00}");
//             Console.WriteLine($"Total -> Examenes: {totalExamenes:0.00}");
//             Console.WriteLine($"Total -> Dietas: {totalDietas:0.00}");
//             Console.WriteLine($"Total -> Paquetes: {totalPaquetes:0.00}");
//             Console.WriteLine($"Total -> Ambulancias: {totalAmbulancias:0.00}");
//             Console.WriteLine($"Total -> Emergencias: {totalEmergencias:0.00}");
//             Console.WriteLine($"Total -> Depósitos (resta): {-totalDepositos:0.00}");
//             Console.WriteLine($"Total -> valorTotalCalculado: {Math.Round(valorTotalCalculado, 2):0.00}");

//             var valorTotalCalculadoRedondeado = Math.Round(valorTotalCalculado, 2);
//             var cuentaValorRedondeado = Math.Round(cuenta.Valor ?? 0m, 2);

//             if (cuentaValorRedondeado != valorTotalCalculadoRedondeado)
//             {
//                 Console.WriteLine(
//                     $"[SYNC] Diferencia detectada. " +
//                     $"Cuenta.Valor={cuentaValorRedondeado:0.00} -> {valorTotalCalculadoRedondeado:0.00}. " +
//                     $"Se actualiza Cuenta.Valor en GET /Pagar."
//                 );

//                 cuenta.Valor = valorTotalCalculadoRedondeado;
//                 _cuentasPorCobrarRepository.Update(cuenta);
//             }

//             var model = new CuentasPorCobrarPagarViewModel
//             {
//                 CuentaId = cuenta.Id,
//                 // Valor = cuenta.Valor ?? 0,
//                 Valor = Math.Round(valorTotalCalculado, 2),
//                 Observaciones = cuenta.Observaciones,
//                 PacienteId = cuenta.Paciente.Id,
//                 ResponsableNit = ResponsableNit,
//                 ResponsableNombre = ResponsableNombre,
//                 ResponsableDireccion = ResponsableDireccion,
//                 ResponsableCorreo = ResponsableCorreo,
//                 Habitacion = habitacionViewModel,

//                 Productos = productosConPrecios
//                     .Concat(serviciosConPrecios)
//                     .Concat(examenesConPrecios)
//                     .Concat(dietasConPrecios)
//                     .Concat(depositosComoProducto)
//                         .Concat(emergenciasVM)
//                         .Concat(insumosDirectosVM)

//                     .ToList(),

//                 Paquetes = paquetesHospitalizacion,
//                 SeguroNombre = SeguroNombre,
//                 PacienteNombreAdmision = PacienteNombreAdmision,
//                 AdmisionId = AdmisionId,

//                 PacienteNombre = pacienteInfo.Nombre,
//                 PacienteTelefono = pacienteInfo.Telefono,
//                 PacienteCelular = pacienteInfo.Celular,
//                 PacienteDireccion = pacienteInfo.Direccion,
//                 PacienteNit = pacienteInfo.Nit,
//                 PacienteDpi = pacienteInfo.Dpi,
//                 PacienteFechaNacimiento = pacienteInfo.FechaNacimiento,
//                 PacienteEdad = pacienteInfo.Edad,
//                 PacienteSexo = pacienteInfo.Sexo,

//                 FechaInicioHops = pacienteInfo.FechaInicioHops,
//                 MedicoHops = pacienteInfo.MedicoHops,
//                 NombreDelSeguro = pacienteInfo.NombreDelSeguro,
//                 Ambulancias = ambulanciasVM,
//             };

//             ViewBag.Hospitalizacion = hospitalizacion;

//             model.Init(_cuentasPorCobrarRepository, _empleadoRepository);

//             Console.WriteLine("=== [GET] CuentasPorCobrar/Pagar -> FIN ===");
//             return View(model);
//         }

//         [HttpPost]
//         public string ConsultarPagos(int cuentaId)
//         {
//             try
//             {
//                 var pagosConsultados = new List<CuentasPorCobrarPagoViewModel>();
//                 var pagosBd = _cuentasPorCobrarRepository.GetPagos(cuentaId);
//                 if (pagosBd != null)
//                 {
//                     foreach (var pago in pagosBd)
//                     {
//                         pagosConsultados.Add(new CuentasPorCobrarPagoViewModel
//                         {
//                             Fecha = pago.FechaHora != null ? ((DateTime)pago.FechaHora).ToString() : "-",
//                             FormaPagoId = pago.FormaPagoId,
//                             FormaPago = pago.FormaPago.NombreFormaPago,
//                             MonedaId = (int)pago.MonedaId,
//                             Moneda = pago.Moneda.NombreMoneda,
//                             PagoId = pago.Id,
//                             Monto = pago.Monto,
//                             Nuevo = false
//                         });
//                     }
//                 }

//                 return JsonSerializer.Serialize(new
//                 {
//                     Exitoso = true,
//                     Resultado = pagosConsultados
//                 });
//             }
//             catch (Exception ex)
//             {
//                 return JsonSerializer.Serialize(new
//                 {
//                     Exitoso = false,
//                     Mensaje = "Error al consultar pagos. " + ex.Message
//                 });
//             }
//         }

//         // [HttpPost]
//         // public async Task<string> Pagar(CuentasPorCobrarPagarViewModel model)
//         // {
//         //     try
//         //     {
//         //         if (model == null)
//         //         {
//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = "Solicitud inválida: modelo vacío."
//         //             });
//         //         }

//         //         if (model.CuentaId <= 0 || model.PacienteId <= 0)
//         //         {
//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = "Solicitud inválida: CuentaId/PacienteId no válidos."
//         //             });
//         //         }

//         //         var cajaClinica = _cajaRepository.ListarCajas()
//         //             .Where(a => a.AmbienteId == (int)AmbienteEnum.Clinica);

//         //         if (!cajaClinica.Any(a => a.EstadoCaja == true))
//         //         {
//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = "Error. No hay cajas abiertas. Por favor debe abrir una caja."
//         //             });
//         //         }

//         //         var cajaAbierta = cajaClinica.FirstOrDefault(a => a.EstadoCaja);
//         //         if (cajaAbierta == null)
//         //         {
//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = "Error. No se encontró caja abierta válida."
//         //             });
//         //         }

//         //         var cuenta = _cuentasPorCobrarRepository.Get(model.CuentaId);
//         //         if (cuenta == null)
//         //         {
//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = "No se encontró la cuenta por cobrar."
//         //             });
//         //         }

//         //         var paciente = _pacientesRepository.Get(model.PacienteId);
//         //         if (paciente == null)
//         //         {
//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = "No se encontró el paciente."
//         //             });
//         //         }

//         //         model.EmpleadoId = 3;
//         //         var fechaHora = DateTime.Now;

//         //         // === VALIDACIÓN: cuenta ya cancelada (antes de registrar pagos nuevos) ===
//         //         // Si el estado de cuenta y lo pagado ya son iguales, no se permiten más pagos.
//         //         var totalCuentaActual = Math.Round(Convert.ToDecimal(cuenta.Valor ?? 0m), 2);
//         //         var totalPagadoActual = Math.Round(Convert.ToDecimal(cuenta.ValorPagado ?? 0m), 2);

//         //         if (totalCuentaActual > 0m && totalPagadoActual == totalCuentaActual)
//         //         {
//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = "Esta cuenta ya se encuentra cancelada en su totalidad. No se permiten más pagos."
//         //             });
//         //         }

//         //         decimal totalPagoNuevo = 0m;

//         //         if (model.Pagos != null)
//         //         {
//         //             foreach (var pago in model.Pagos)
//         //             {
//         //                 if (pago == null) continue;
//         //                 if (!pago.Nuevo) continue;

//         //                 var monto = Convert.ToDecimal(pago.Monto);
//         //                 if (monto <= 0m) continue;

//         //                 var pagoBd = new Pagos
//         //                 {
//         //                     CuentaPorCobrarId = model.CuentaId,
//         //                     FormaPagoId = Convert.ToInt32(pago.FormaPagoId),
//         //                     Monto = monto,
//         //                     MonedaId = pago.MonedaId,
//         //                     FechaHora = fechaHora
//         //                 };

//         //                 _cuentasPorCobrarRepository.AddPago(pagoBd);
//         //                 totalPagoNuevo += monto;

//         //                 var nombreCliente = !string.IsNullOrWhiteSpace(model.PacienteNombre)
//         //                     ? model.PacienteNombre
//         //                     : (paciente.Nombre ?? "Paciente");

//         //                 var nuevoDetalleCaja = new DetalleCaja()
//         //                 {
//         //                     CuentaPorCobrarId = model.CuentaId,
//         //                     CuentaPorCobrarPagoId = pagoBd.Id,
//         //                     Descripcion = "Pago de cuenta por cobrar. Cliente: " + nombreCliente,
//         //                     Ingreso = monto,
//         //                     Caja = cajaAbierta
//         //                 };

//         //                 _cajaRepository.Add(nuevoDetalleCaja);
//         //             }
//         //         }

//         //         if (totalPagoNuevo <= 0m)
//         //         {
//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = "No se registró ningún pago nuevo válido."
//         //             });
//         //         }

//         //         // === Sync de Cuenta.Valor cuando el total aumenta desde la vista (ej: Honorarios médicos) ===
//         //         // En este método, model.Valor ya se recibe desde la vista.
//         //         // La lógica existente del método usa cuenta.Valor como totalCuenta; por eso el sync debe ir ANTES
//         //         // de validar saldo y cerrar.
//         //         var valorCuentaActual = Math.Round(cuenta.Valor ?? 0m, 2);
//         //         var valorRecibido = Math.Round(model.Valor, 2);

//         //         // Solo se permite incremento (no se reduce Cuenta.Valor desde el cliente).
//         //         if (valorRecibido > 0m && valorRecibido > valorCuentaActual)
//         //         {
//         //             Console.WriteLine(
//         //                 $"[SYNC POST] Se detectó incremento en el total desde la vista. " +
//         //                 $"Cuenta.Valor={valorCuentaActual:0.00} -> {valorRecibido:0.00}. Se actualiza en BD."
//         //             );

//         //             cuenta.Valor = valorRecibido;
//         //             _cuentasPorCobrarRepository.Update(cuenta);
//         //         }

//         //         // === VALIDACIONES DE MONTO / SALDO (con valores ya sincronizados) ===
//         //         var totalCuenta = Math.Round(Convert.ToDecimal(cuenta.Valor ?? 0m), 2);
//         //         var pagadoAcumulado = Math.Round(Convert.ToDecimal(cuenta.ValorPagado ?? 0m), 2);
//         //         var pagoNuevo = Math.Round(totalPagoNuevo, 2);

//         //         if (totalCuenta <= 0m)
//         //         {
//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = "La cuenta no tiene un estado de cuenta válido para pagar. Por favor verifique el total de la cuenta."
//         //             });
//         //         }

//         //         // Revalidar cancelación total (por si cambió totalCuenta tras el sync)
//         //         if (pagadoAcumulado == totalCuenta)
//         //         {
//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = "Esta cuenta ya se encuentra cancelada en su totalidad. No se permiten más pagos."
//         //             });
//         //         }

//         //         var saldoPendiente = Math.Round(totalCuenta - pagadoAcumulado, 2);

//         //         if (pagoNuevo > saldoPendiente)
//         //         {
//         //             var excedente = Math.Round(pagoNuevo - saldoPendiente, 2);

//         //             return JsonSerializer.Serialize(new
//         //             {
//         //                 Exitoso = false,
//         //                 Mensaje = $"El monto a pagar es superior al del estado de cuenta. Saldo pendiente: {saldoPendiente:0.00}. Excede por: {excedente:0.00}. Por favor verificar monto."
//         //             });
//         //         }

//         //         // Aplicar pago (parcial o total)
//         //         var nuevoPagadoAcumulado = Math.Round(pagadoAcumulado + pagoNuevo, 2);

//         //         cuenta.ValorPagado = nuevoPagadoAcumulado;
//         //         cuenta.FechaPagoRealizado = fechaHora;

//         //         // Cerrar SOLO si queda exactamente igual al total
//         //         bool cuentaCerrada = (nuevoPagadoAcumulado == totalCuenta);
//         //         cuenta.Pagada = cuentaCerrada;

//         //         var saldoRestante = Math.Round(totalCuenta - nuevoPagadoAcumulado, 2);

//         //         #region Actualizar Hospitalización (solo si cuenta cerrada)
//         //         if (cuentaCerrada && cuenta.DetallesCuentaPorCobrar != null)
//         //         {
//         //             foreach (var detalle in cuenta.DetallesCuentaPorCobrar)
//         //             {
//         //                 if (detalle.HospitalizacionId != null)
//         //                 {
//         //                     var hospitalizacion = _hospitalizacionRepository
//         //                         .Get((int)detalle.HospitalizacionId, false, false, false);

//         //                     if (hospitalizacion != null)
//         //                     {
//         //                         hospitalizacion.Pagada = true;
//         //                         hospitalizacion.Finalizada = true;
//         //                         hospitalizacion.FechaHoraFinalizada = fechaHora;
//         //                         hospitalizacion.FechaHoraPago = fechaHora;
//         //                         _hospitalizacionRepository.Update(hospitalizacion);

//         //                         var habitacion = _habitacionRepository.Get(hospitalizacion.HabitacionId);
//         //                         if (habitacion != null)
//         //                         {
//         //                             habitacion.EstadoHabitacionId = (int)EstadoHabitacionEnum.Disponible;
//         //                         }
//         //                     }
//         //                 }
//         //             }
//         //         }
//         //         #endregion

//         //         // ===== Persistir datos receptor FEL (para ReintentarFel) =====
//         //         var receptorNit = string.IsNullOrWhiteSpace(model.ResponsableNit) ? "CF" : model.ResponsableNit.Trim();
//         //         var receptorNombre = string.IsNullOrWhiteSpace(model.ResponsableNombre) ? "Consumidor Final" : model.ResponsableNombre.Trim();
//         //         var receptorDireccion = string.IsNullOrWhiteSpace(model.ResponsableDireccion) ? "N/A" : model.ResponsableDireccion.Trim();
//         //         var receptorCorreo = string.IsNullOrWhiteSpace(model.ResponsableCorreo) ? "sin-correo@example.com" : model.ResponsableCorreo.Trim();

//         //         cuenta.FelReceptorNit = receptorNit;
//         //         cuenta.FelReceptorNombre = receptorNombre;
//         //         cuenta.FelReceptorDireccion = receptorDireccion;
//         //         cuenta.FelReceptorCorreo = receptorCorreo;

//         //         // ===== Persistimos pago/cierre primero =====
//         //         if (!cuentaCerrada)
//         //         {
//         //             cuenta.FelEstado = EstadosFEL.NoIniciado;
//         //         }
//         //         else
//         //         {
//         //             if (string.IsNullOrWhiteSpace(cuenta.UuidFel))
//         //             {
//         //                 cuenta.FelEstado = EstadosFEL.Pendiente;
//         //                 cuenta.FelFechaUltimoIntento = DateTime.Now;
//         //             }
//         //             else
//         //             {
//         //                 cuenta.FelEstado = EstadosFEL.Emitida;
//         //             }
//         //         }

//         //         _cuentasPorCobrarRepository.Update(cuenta);

//         //         // ===== Emitir FEL SOLO si cerrada y sin UUID =====
//         //         if (cuentaCerrada && string.IsNullOrWhiteSpace(cuenta.UuidFel))
//         //         {
//         //             try
//         //             {
//         //                 var felRequest = BuildFelRequest(
//         //                     model: model,
//         //                     totalFactura: totalCuenta,
//         //                     receptorNit: receptorNit,
//         //                     receptorNombre: receptorNombre,
//         //                     receptorDireccion: receptorDireccion,
//         //                     receptorCorreo: receptorCorreo
//         //                 );

//         //                 var uuid = await EmitirFelAsync(felRequest);

//         //                 cuenta.UuidFel = uuid;
//         //                 cuenta.FelEstado = EstadosFEL.Emitida;
//         //                 cuenta.FelFechaEmitida = DateTime.Now;
//         //                 cuenta.FelUltimoError = null;
//         //                 cuenta.FelFechaUltimoIntento = DateTime.Now;

//         //                 _cuentasPorCobrarRepository.Update(cuenta);
//         //             }
//         //             catch (Exception felEx)
//         //             {
//         //                 cuenta.FelEstado = EstadosFEL.Error;
//         //                 cuenta.FelIntentos = (cuenta.FelIntentos < 0 ? 0 : cuenta.FelIntentos) + 1;
//         //                 cuenta.FelUltimoError = felEx.Message;
//         //                 cuenta.FelFechaUltimoIntento = DateTime.Now;

//         //                 _cuentasPorCobrarRepository.Update(cuenta);
//         //             }
//         //         }

//         //         TempData["Message"] = "El pago de la cuenta ha sido registrado";

//         //         return JsonSerializer.Serialize(new
//         //         {
//         //             Exitoso = true,
//         //             CuentaCerrada = cuentaCerrada,
//         //             TotalCuenta = totalCuenta,
//         //             TotalPagado = nuevoPagadoAcumulado,
//         //             SaldoRestante = saldoRestante,
//         //             Mensaje = cuentaCerrada
//         //                 ? "Pago registrado. La cuenta quedó cancelada en su totalidad."
//         //                 : $"Pago registrado. Falta por pagar: {saldoRestante:0.00}.",
//         //             FelEstado = cuenta.FelEstado,
//         //             UuidFel = cuenta.UuidFel,
//         //             FelUltimoError = cuenta.FelUltimoError,
//         //             FelIntentos = cuenta.FelIntentos
//         //         });
//         //     }
//         //     catch (Exception ex)
//         //     {
//         //         return JsonSerializer.Serialize(new
//         //         {
//         //             Exitoso = false,
//         //             Mensaje = "Error al registrar el pago. " + ex.Message
//         //         });
//         //     }
//         // }


//         [HttpPost]
//         public async Task<string> Pagar(CuentasPorCobrarPagarViewModel model)
//         {
//             try
//             {
//                 if (model == null)
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = false,
//                         Mensaje = "Solicitud inválida: modelo vacío."
//                     });
//                 }

//                 if (model.CuentaId <= 0 || model.PacienteId <= 0)
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = false,
//                         Mensaje = "Solicitud inválida: CuentaId/PacienteId no válidos."
//                     });
//                 }

//                 var cajaClinica = _cajaRepository.ListarCajas()
//                     .Where(a => a.AmbienteId == (int)AmbienteEnum.Clinica);

//                 if (!cajaClinica.Any(a => a.EstadoCaja == true))
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = false,
//                         Mensaje = "Error. No hay cajas abiertas. Por favor debe abrir una caja."
//                     });
//                 }

//                 var cajaAbierta = cajaClinica.FirstOrDefault(a => a.EstadoCaja);
//                 if (cajaAbierta == null)
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = false,
//                         Mensaje = "Error. No se encontró caja abierta válida."
//                     });
//                 }

//                 var cuenta = _cuentasPorCobrarRepository.Get(model.CuentaId);

//                 Console.WriteLine("========== INICIO PAGO ==========");
//                 Console.WriteLine($"[CUENTA DB] cuenta.Valor (neto) = {cuenta?.Valor}");
//                 Console.WriteLine($"[CUENTA DB] cuenta.ValorPagado = {cuenta?.ValorPagado}");
//                 Console.WriteLine($"[MODEL] model.Valor recibido = {model.Valor}");
//                 Console.WriteLine($"[MODEL] Pagos nuevos: {model.Pagos?.Count(p => p.Nuevo) ?? 0}");

//                 if (cuenta == null)
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = false,
//                         Mensaje = "No se encontró la cuenta por cobrar."
//                     });
//                 }

//                 var paciente = _pacientesRepository.Get(model.PacienteId);
//                 if (paciente == null)
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = false,
//                         Mensaje = "No se encontró el paciente."
//                     });
//                 }

//                 model.EmpleadoId = 3;
//                 var fechaHora = DateTime.Now;

//                 var totalCuenta = Math.Round(Convert.ToDecimal(cuenta.Valor ?? 0m), 2); // Neto actual (saldo pendiente)
//                 var pagadoAnterior = Math.Round(Convert.ToDecimal(cuenta.ValorPagado ?? 0m), 2);

//                 var saldoPendienteReal = totalCuenta;

//                 Console.WriteLine($"[DEBUG PAGO] Inicio - TotalCuenta (neto): {totalCuenta}, PagadoAnterior: {pagadoAnterior}, SaldoPendienteReal: {saldoPendienteReal}");

//                 // === VALIDACIÓN: cuenta ya cancelada ===
//                 if (totalCuenta <= 0m && pagadoAnterior > 0m)
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = false,
//                         Mensaje = "Esta cuenta ya se encuentra cancelada en su totalidad. No se permiten más pagos."
//                     });
//                 }

//                 // === PROCESAR PAGOS NUEVOS ===
//                 decimal totalPagoNuevo = 0m;

//                 if (model.Pagos != null)
//                 {
//                     foreach (var pago in model.Pagos)
//                     {
//                         if (pago == null) continue;
//                         if (!pago.Nuevo) continue;

//                         var monto = Convert.ToDecimal(pago.Monto);
//                         if (monto <= 0m) continue;

//                         // VALIDAR que el pago nuevo no exceda el saldo pendiente
//                         if (totalPagoNuevo + monto > saldoPendienteReal)
//                         {
//                             var excedente = (totalPagoNuevo + monto) - saldoPendienteReal;
//                             return JsonSerializer.Serialize(new
//                             {
//                                 Exitoso = false,
//                                 Mensaje = $"El monto a pagar excede el saldo pendiente. " +
//                                          $"Saldo pendiente: {saldoPendienteReal:0.00}, " +
//                                          $"Excede por: {excedente:0.00}. Por favor verificar monto."
//                             });
//                         }

//                         var pagoBd = new Pagos
//                         {
//                             CuentaPorCobrarId = model.CuentaId,
//                             FormaPagoId = Convert.ToInt32(pago.FormaPagoId),
//                             Monto = monto,
//                             MonedaId = pago.MonedaId,
//                             FechaHora = fechaHora
//                         };

//                         _cuentasPorCobrarRepository.AddPago(pagoBd);
//                         totalPagoNuevo += monto;

//                         var nombreCliente = !string.IsNullOrWhiteSpace(model.PacienteNombre)
//                             ? model.PacienteNombre
//                             : (paciente.Nombre ?? "Paciente");

//                         var nuevoDetalleCaja = new DetalleCaja()
//                         {
//                             CuentaPorCobrarId = model.CuentaId,
//                             CuentaPorCobrarPagoId = pagoBd.Id,
//                             Descripcion = "Pago de cuenta por cobrar. Cliente: " + nombreCliente,
//                             Ingreso = monto,
//                             Caja = cajaAbierta
//                         };

//                         _cajaRepository.Add(nuevoDetalleCaja);
//                     }
//                 }

//                 if (totalPagoNuevo <= 0m)
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = false,
//                         Mensaje = "No se registró ningún pago nuevo válido."
//                     });
//                 }

//                 // === ACTUALIZAR VALORES DE LA CUENTA ===
//                 var nuevoPagadoAcumulado = Math.Round(pagadoAnterior + totalPagoNuevo, 2);
//                 cuenta.ValorPagado = nuevoPagadoAcumulado;

//                 // Reducir el neto de la cuenta (saldo pendiente)
//                 cuenta.Valor = Math.Round((cuenta.Valor ?? 0m) - totalPagoNuevo, 2);
//                 cuenta.FechaPagoRealizado = fechaHora;

//                 // Determinar si la cuenta queda cerrada (neto <= 0)
//                 bool cuentaCerrada = Math.Abs(cuenta.Valor ?? 0m) < 0.01m;
//                 cuenta.Pagada = cuentaCerrada;

//                 // ===== Persistir datos receptor FEL =====
//                 var receptorNit = string.IsNullOrWhiteSpace(model.ResponsableNit) ? "CF" : model.ResponsableNit.Trim();
//                 var receptorNombre = string.IsNullOrWhiteSpace(model.ResponsableNombre) ? "Consumidor Final" : model.ResponsableNombre.Trim();
//                 var receptorDireccion = string.IsNullOrWhiteSpace(model.ResponsableDireccion) ? "N/A" : model.ResponsableDireccion.Trim();
//                 var receptorCorreo = string.IsNullOrWhiteSpace(model.ResponsableCorreo) ? "sin-correo@example.com" : model.ResponsableCorreo.Trim();

//                 cuenta.FelReceptorNit = receptorNit;
//                 cuenta.FelReceptorNombre = receptorNombre;
//                 cuenta.FelReceptorDireccion = receptorDireccion;
//                 cuenta.FelReceptorCorreo = receptorCorreo;

//                 // Guardar cambios en la cuenta
//                 _cuentasPorCobrarRepository.Update(cuenta);

//                 // Si la cuenta queda cerrada, actualizar hospitalizaciones asociadas
//                 if (cuentaCerrada && cuenta.DetallesCuentaPorCobrar != null)
//                 {
//                     foreach (var detalle in cuenta.DetallesCuentaPorCobrar)
//                     {
//                         if (detalle.HospitalizacionId != null)
//                         {
//                             var hospitalizacion = _hospitalizacionRepository
//                                 .Get((int)detalle.HospitalizacionId, false, false, false);

//                             if (hospitalizacion != null)
//                             {
//                                 hospitalizacion.Pagada = true;
//                                 hospitalizacion.Finalizada = true;
//                                 hospitalizacion.FechaHoraFinalizada = fechaHora;
//                                 hospitalizacion.FechaHoraPago = fechaHora;
//                                 _hospitalizacionRepository.Update(hospitalizacion);

//                                 var habitacion = _habitacionRepository.Get(hospitalizacion.HabitacionId);
//                                 if (habitacion != null)
//                                 {
//                                     habitacion.EstadoHabitacionId = (int)EstadoHabitacionEnum.Disponible;
//                                 }
//                             }
//                         }
//                     }
//                 }

//                 // ===== Obtener cuenta actualizada para respuesta =====
//                 var cuentaActualizada = _cuentasPorCobrarRepository.Get(model.CuentaId);
//                 var saldoRestante = Math.Round(Convert.ToDecimal(cuentaActualizada.Valor ?? 0m), 2);

//                 // Calcular el total de cargos originales 
//                 var totalFactura = Math.Round(nuevoPagadoAcumulado + saldoRestante, 2);

//                 Console.WriteLine("========== DEBUG PAGO (POST) ==========");
//                 Console.WriteLine($"TotalCuenta ORIGINAL (antes): {totalCuenta}");
//                 Console.WriteLine($"TotalCuenta ACTUALIZADO (después de pago): {saldoRestante}");
//                 Console.WriteLine($"PagadoAnterior ORIGINAL: {pagadoAnterior}");
//                 Console.WriteLine($"Pagado ACTUALIZADO: {nuevoPagadoAcumulado}");
//                 Console.WriteLine($"TotalPagoNuevo: {totalPagoNuevo}");
//                 Console.WriteLine($"SaldoRestante CORREGIDO: {saldoRestante}");
//                 Console.WriteLine($"CuentaCerrada: {cuentaCerrada}");
//                 Console.WriteLine("========================================");

//                 // ===== Actualizar estado FEL =====
//                 if (!cuentaCerrada)
//                 {
//                     cuentaActualizada.FelEstado = EstadosFEL.NoIniciado;
//                 }
//                 else
//                 {
//                     if (string.IsNullOrWhiteSpace(cuentaActualizada.UuidFel))
//                     {
//                         cuentaActualizada.FelEstado = EstadosFEL.Pendiente;
//                         cuentaActualizada.FelFechaUltimoIntento = DateTime.Now;
//                     }
//                     else
//                     {
//                         cuentaActualizada.FelEstado = EstadosFEL.Emitida;
//                     }
//                 }

//                 _cuentasPorCobrarRepository.Update(cuentaActualizada);

//                 // ===== Emitir FEL SOLO si cerrada y sin UUID =====
//                 if (cuentaCerrada && string.IsNullOrWhiteSpace(cuentaActualizada.UuidFel))
//                 {
//                     try
//                     {
//                         var felRequest = BuildFelRequest(
//                             model: model,
//                             totalFactura: totalFactura,
//                             receptorNit: receptorNit,
//                             receptorNombre: receptorNombre,
//                             receptorDireccion: receptorDireccion,
//                             receptorCorreo: receptorCorreo
//                         );

//                         var uuid = await EmitirFelAsync(felRequest);

//                         cuentaActualizada.UuidFel = uuid;
//                         cuentaActualizada.FelEstado = EstadosFEL.Emitida;
//                         cuentaActualizada.FelFechaEmitida = DateTime.Now;
//                         cuentaActualizada.FelUltimoError = null;
//                         cuentaActualizada.FelFechaUltimoIntento = DateTime.Now;

//                         _cuentasPorCobrarRepository.Update(cuentaActualizada);
//                     }
//                     catch (Exception felEx)
//                     {
//                         cuentaActualizada.FelEstado = EstadosFEL.Error;
//                         cuentaActualizada.FelIntentos = (cuentaActualizada.FelIntentos < 0 ? 0 : cuentaActualizada.FelIntentos) + 1;
//                         cuentaActualizada.FelUltimoError = felEx.Message;
//                         cuentaActualizada.FelFechaUltimoIntento = DateTime.Now;

//                         _cuentasPorCobrarRepository.Update(cuentaActualizada);
//                     }
//                 }

//                 TempData["Message"] = "El pago de la cuenta ha sido registrado";

//                 string mensajeUsuario;
//                 if (cuentaCerrada)
//                 {
//                     mensajeUsuario = "¡Pago exitoso! La cuenta ha sido cancelada en su totalidad.";
//                 }
//                 else
//                 {
//                     mensajeUsuario = $"Pago parcial registrado correctamente. Saldo pendiente: Q {saldoRestante:0.00}";
//                 }

//                 Console.WriteLine($"[RESPUESTA] Enviando SaldoRestante: {saldoRestante}");
//                 Console.WriteLine("========== FIN PAGO ==========");

//                 return JsonSerializer.Serialize(new
//                 {
//                     Exitoso = true,
//                     CuentaCerrada = cuentaCerrada,
//                     TotalCuenta = totalFactura,
//                     TotalPagado = nuevoPagadoAcumulado,
//                     TotalPagadoAnterior = pagadoAnterior,
//                     TotalPagoNuevo = totalPagoNuevo,
//                     SaldoRestante = saldoRestante,
//                     Mensaje = mensajeUsuario,
//                     FelEstado = cuentaActualizada.FelEstado,
//                     UuidFel = cuentaActualizada.UuidFel,
//                     FelUltimoError = cuentaActualizada.FelUltimoError,
//                     FelIntentos = cuentaActualizada.FelIntentos
//                 });
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"[ERROR Pagar] {ex.Message}");
//                 Console.WriteLine($"[ERROR StackTrace] {ex.StackTrace}");
//                 return JsonSerializer.Serialize(new
//                 {
//                     Exitoso = false,
//                     Mensaje = "Error al registrar el pago. " + ex.Message
//                 });
//             }
//         }

//         private object BuildFelRequest(
//             CuentasPorCobrarPagarViewModel model,
//             decimal totalFactura,
//             string receptorNit,
//             string receptorNombre,
//             string receptorDireccion,
//             string receptorCorreo)
//         {
//             var granTotal = Math.Round(totalFactura, 2);

//             var descripcion = $"Servicio de Hospitalización Paciente: {model.PacienteNombreAdmision ?? model.PacienteNombre ?? "PACIENTE"} (Admision #{model.AdmisionId})";
//             var desc = (descripcion ?? string.Empty).ToUpperInvariant();

//             var montoGravable = Math.Round(granTotal / 1.12m, 2);
//             var montoImpuesto = Math.Round(granTotal - montoGravable, 2);

//             return new
//             {
//                 CodigoMoneda = "GTQ",
//                 FechaHoraEmision = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss-06:00", CultureInfo.InvariantCulture),
//                 TipoDocumento = "FACT",
//                 Emisor = new
//                 {
//                     AfiliacionIVA = "GEN",
//                     CodigoEstablecimiento = "1",
//                     CorreoEmisor = "recepcion@hcq.com",
//                     NITEmisor = "117286303",
//                     NombreComercial = "HOSPITAL CLINICO QUIRURGICO SANCTI SPIRITUS",
//                     NombreEmisor = "HOSPITAL CLINICO QUIRURGICO SANCTI SPIRITUS",
//                     Direccion = new
//                     {
//                         DetalleDireccion = "4 CALLE 1-91 BARRIO SAN SEBASTIAN ZONA 3 SAN CRISTÓBAL VERAPAZ, ALTA VERAPAZ",
//                         CodigoPostal = "01011",
//                         Municipio = "Guatemala",
//                         Departamento = "Guatemala",
//                         Pais = "GT"
//                     }
//                 },
//                 Receptor = new
//                 {
//                     CorreoReceptor = receptorCorreo,
//                     IDReceptor = receptorNit,
//                     NombreReceptor = receptorNombre,
//                     Direccion = new
//                     {
//                         DetalleDireccion = receptorDireccion,
//                         CodigoPostal = "01001",
//                         Municipio = "Guatemala",
//                         Departamento = "Guatemala",
//                         Pais = "GT"
//                     }
//                 },
//                 Frases = new[]
//                 {
//                     new { CodigoEscenario = "1", TipoFrase = "1" }
//                 },
//                 Items = new[]
//                 {
//                     new
//                     {
//                         BienOServicio = "S",
//                         NumeroLinea = "1",
//                         Cantidad = 1,
//                         UnidadMedida = "UND",
//                         Descripcion = desc,
//                         PrecioUnitario = granTotal,
//                         Precio = granTotal,
//                         Descuento = 0,
//                         Impuestos = new[]
//                         {
//                             new
//                             {
//                                 NombreCorto = "IVA",
//                                 CodigoUnidadGravable = "1",
//                                 MontoGravable = montoGravable.ToString("0.00", CultureInfo.InvariantCulture),
//                                 MontoImpuesto = montoImpuesto.ToString("0.00", CultureInfo.InvariantCulture)
//                             }
//                         },
//                         Total = granTotal.ToString("0.00", CultureInfo.InvariantCulture)
//                     }
//                 },
//                 Totales = new
//                 {
//                     TotalImpuestos = new[]
//                     {
//                         new
//                         {
//                             NombreCorto = "IVA",
//                             TotalMontoImpuesto = montoImpuesto.ToString("0.00", CultureInfo.InvariantCulture)
//                         }
//                     },
//                     GranTotal = granTotal.ToString("0.00", CultureInfo.InvariantCulture)
//                 },
//                 Adenda = "FACTURA-19"
//             };
//         }

//         private async Task<string> EmitirFelAsync(object felRequest)
//         {
//             if (_felSettings == null)
//                 throw new Exception("FelSettings no está disponible (inyección/configuración).");

//             if (string.IsNullOrWhiteSpace(_felSettings.BaseUrl))
//                 throw new Exception("Configuración FEL incompleta: Fel:BaseUrl.");

//             if (string.IsNullOrWhiteSpace(_felSettings.UsuarioFirma) ||
//                 string.IsNullOrWhiteSpace(_felSettings.LlaveFirma) ||
//                 string.IsNullOrWhiteSpace(_felSettings.UsuarioApi) ||
//                 string.IsNullOrWhiteSpace(_felSettings.LlaveApi))
//                 throw new Exception("Configuración FEL incompleta: Fel:UsuarioFirma/LlaveFirma/UsuarioApi/LlaveApi.");

//             var http = _httpClientFactory.CreateClient();
//             http.BaseAddress = new Uri(_felSettings.BaseUrl.TrimEnd('/'));
//             http.Timeout = TimeSpan.FromSeconds(_felSettings.TimeoutSeconds > 0 ? _felSettings.TimeoutSeconds : 30);

//             var jsonReq = JsonSerializer.Serialize(felRequest);
//             using var genContent = new StringContent(jsonReq, Encoding.UTF8, "application/json");
//             using var genResp = await http.PostAsync("/Xml/GenerateXml", genContent);
//             var genBody = await genResp.Content.ReadAsStringAsync();

//             if (!genResp.IsSuccessStatusCode)
//                 throw new Exception($"FEL GenerateXml error HTTP {(int)genResp.StatusCode}: {genBody}");

//             using var genDoc = JsonDocument.Parse(genBody);
//             if (!genDoc.RootElement.TryGetProperty("xmlContent", out var xmlEl))
//                 throw new Exception("FEL GenerateXml no retornó 'xmlContent'.");

//             var xmlContent = xmlEl.GetString();
//             if (string.IsNullOrWhiteSpace(xmlContent))
//                 throw new Exception("FEL GenerateXml retornó xmlContent vacío.");

//             var sendPayload = new
//             {
//                 XmlContent = xmlContent,
//                 UsuarioFirma = _felSettings.UsuarioFirma,
//                 LlaveFirma = _felSettings.LlaveFirma,
//                 UsuarioApi = _felSettings.UsuarioApi,
//                 LlaveApi = _felSettings.LlaveApi
//             };

//             var jsonSend = JsonSerializer.Serialize(sendPayload);
//             using var sendContent = new StringContent(jsonSend, Encoding.UTF8, "application/json");
//             using var sendResp = await http.PostAsync("/Xml/SendXml", sendContent);
//             var sendBody = await sendResp.Content.ReadAsStringAsync();

//             if (!sendResp.IsSuccessStatusCode)
//                 throw new Exception($"FEL SendXml error HTTP {(int)sendResp.StatusCode}: {sendBody}");

//             using var sendDoc = JsonDocument.Parse(sendBody);
//             if (!sendDoc.RootElement.TryGetProperty("uuid", out var uuidEl))
//                 throw new Exception("FEL SendXml no retornó 'uuid'.");

//             var uuid = uuidEl.GetString();
//             if (string.IsNullOrWhiteSpace(uuid))
//                 throw new Exception("FEL SendXml retornó uuid vacío.");

//             return uuid;
//         }

//         [HttpPost]
//         public JsonResult ConsultarCuentasPorCobrar()
//         {
//             try
//             {
//                 var resultado = _cuentasPorCobrarRepository.GetList().Where(c => !c.Pagada).ToList();
//                 return Json(new
//                 {
//                     Exitoso = true,
//                     Resultado = resultado
//                 });
//             }
//             catch (Exception ex)
//             {
//                 return Json(new
//                 {
//                     Exitoso = false,
//                     Mensaje = "Error al consultar cuentas por cobrar. " + ex.Message
//                 });
//             }
//         }

//         public IActionResult Modificar(int cuentaId)
//         {
//             var cuenta = _cuentasPorCobrarRepository.Get(cuentaId);
//             var model = new CuentasPorCobrarModificarViewModel();
//             model.CuentaId = cuenta.Id;
//             model.Valor = cuenta.Valor ?? 0;
//             model.Observaciones = cuenta.Observaciones;
//             return View(model);
//         }

//         [HttpPost]
//         public JsonResult Modificar(CuentasPorCobrarModificarViewModel model)
//         {
//             try
//             {
//                 var cuenta = _cuentasPorCobrarRepository.Get(model.CuentaId);
//                 cuenta.Valor = model.Valor;
//                 cuenta.Observaciones = model.Observaciones;
//                 _cuentasPorCobrarRepository.Update(cuenta);
//                 TempData["Message"] = "Se ha modificado la cuenta por cobrar!";
//                 return Json(new
//                 {
//                     Exitoso = true
//                 });
//             }
//             catch (Exception ex)
//             {
//                 return Json(new
//                 {
//                     Exitoso = false,
//                     Mensaje = "Error al modificar cuenta. " + ex.Message
//                 });
//             }
//         }

//         [HttpPost]
//         public async Task<string> ReintentarFel(int cuentaId)
//         {
//             try
//             {
//                 if (cuentaId <= 0)
//                     return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "CuentaId inválido." });

//                 var cuenta = _cuentasPorCobrarRepository.Get(cuentaId);
//                 if (cuenta == null)
//                     return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "No se encontró la cuenta por cobrar." });

//                 if (!cuenta.Pagada)
//                     return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "La cuenta no está cerrada. No aplica FEL." });

//                 if (!string.IsNullOrWhiteSpace(cuenta.UuidFel))
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = true,
//                         Mensaje = "La FEL ya fue emitida.",
//                         FelEstado = cuenta.FelEstado,
//                         UuidFel = cuenta.UuidFel,
//                         FelUltimoError = cuenta.FelUltimoError,
//                         FelIntentos = cuenta.FelIntentos
//                     });
//                 }

//                 if (!(cuenta.FelEstado == EstadosFEL.Pendiente || cuenta.FelEstado == EstadosFEL.Error))
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = false,
//                         Mensaje = $"Estado FEL no permite reintento. Estado actual: {cuenta.FelEstado}"
//                     });
//                 }

//                 // ✅ Receptor desde DB (persistido)
//                 var receptorNit = string.IsNullOrWhiteSpace(cuenta.FelReceptorNit) ? "CF" : cuenta.FelReceptorNit.Trim();
//                 var receptorNombre = string.IsNullOrWhiteSpace(cuenta.FelReceptorNombre) ? "Consumidor Final" : cuenta.FelReceptorNombre.Trim();
//                 var receptorDireccion = string.IsNullOrWhiteSpace(cuenta.FelReceptorDireccion) ? "N/A" : cuenta.FelReceptorDireccion.Trim();
//                 var receptorCorreo = string.IsNullOrWhiteSpace(cuenta.FelReceptorCorreo) ? "sin-correo@example.com" : cuenta.FelReceptorCorreo.Trim();

//                 // ✅ Reconstruir admisión (si existe)
//                 var admisionId = cuenta.DetallesCuentaPorCobrar?
//                     .FirstOrDefault(d => d.HospitalizacionId.HasValue)?.HospitalizacionId ?? 0;

//                 // ✅ FIX CS0266: cuenta.PacienteId es int? y VM.PacienteId es int
//                 var pacienteId = cuenta.PacienteId ?? cuenta.Paciente?.Id ?? 0;
//                 if (pacienteId <= 0)
//                 {
//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = false,
//                         Mensaje = "La cuenta no tiene PacienteId asociado. No se puede reintentar FEL."
//                     });
//                 }

//                 var pacienteNombre = cuenta.Paciente?.Nombre;

//                 var vm = new CuentasPorCobrarPagarViewModel
//                 {
//                     CuentaId = cuenta.Id,
//                     PacienteId = pacienteId, // ✅ ya no truena
//                     PacienteNombre = pacienteNombre,
//                     AdmisionId = admisionId,
//                     PacienteNombreAdmision = pacienteNombre
//                 };

//                 var totalCuenta = Convert.ToDecimal(cuenta.Valor ?? 0m);

//                 var felRequest = BuildFelRequest(
//                     model: vm,
//                     totalFactura: totalCuenta,
//                     receptorNit: receptorNit,
//                     receptorNombre: receptorNombre,
//                     receptorDireccion: receptorDireccion,
//                     receptorCorreo: receptorCorreo
//                 );

//                 try
//                 {
//                     var uuid = await EmitirFelAsync(felRequest);

//                     cuenta.UuidFel = uuid;
//                     cuenta.FelEstado = EstadosFEL.Emitida;
//                     cuenta.FelFechaEmitida = DateTime.Now;
//                     cuenta.FelUltimoError = null;
//                     cuenta.FelFechaUltimoIntento = DateTime.Now;

//                     _cuentasPorCobrarRepository.Update(cuenta);

//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = true,
//                         Mensaje = "FEL emitida correctamente.",
//                         FelEstado = cuenta.FelEstado,
//                         UuidFel = cuenta.UuidFel,
//                         FelUltimoError = cuenta.FelUltimoError,
//                         FelIntentos = cuenta.FelIntentos
//                     });
//                 }
//                 catch (Exception felEx)
//                 {
//                     cuenta.FelEstado = EstadosFEL.Error;
//                     cuenta.FelIntentos = (cuenta.FelIntentos < 0 ? 0 : cuenta.FelIntentos) + 1;
//                     cuenta.FelUltimoError = felEx.Message;
//                     cuenta.FelFechaUltimoIntento = DateTime.Now;

//                     _cuentasPorCobrarRepository.Update(cuenta);

//                     return JsonSerializer.Serialize(new
//                     {
//                         Exitoso = true,
//                         Mensaje = "No fue posible emitir FEL. Queda pendiente para reintento.",
//                         FelEstado = cuenta.FelEstado,
//                         UuidFel = cuenta.UuidFel,
//                         FelUltimoError = cuenta.FelUltimoError,
//                         FelIntentos = cuenta.FelIntentos
//                     });
//                 }
//             }
//             catch (Exception ex)
//             {
//                 return JsonSerializer.Serialize(new
//                 {
//                     Exitoso = false,
//                     Mensaje = "Error al reintentar FEL. " + ex.Message
//                 });
//             }
//         }

//         [HttpPost]
//         public JsonResult ConsultarCuentasPagadas()
//         {
//             try
//             {
//                 var resultado = _cuentasPorCobrarRepository.GetList().Where(c => c.Pagada).ToList();
//                 return Json(new
//                 {
//                     Exitoso = true,
//                     Resultado = resultado
//                 });
//             }
//             catch (Exception ex)
//             {
//                 return Json(new
//                 {
//                     Exitoso = false,
//                     Mensaje = "Error al consultar cuentas pagadas. " + ex.Message
//                 });
//             }
//         }

//         [HttpPost]
//         public string ConsultarMedicamentosNoPagadosHospitalizaciones(int cuentaId)
//         {
//             try
//             {
//                 var medNoPagados = _cuentasPorCobrarService.GetMedicamentosNoPagadosHospitalizaciones(cuentaId);
//                 return JsonSerializer.Serialize(new
//                 {
//                     Exitoso = true,
//                     Resultado = medNoPagados
//                 });
//             }
//             catch
//             {
//                 return JsonSerializer.Serialize(new
//                 {
//                     Exitoso = false,
//                     Mensaje = "Error de conexion al consultar medicamentos de hospitalizacion"
//                 });
//             }
//         }

//         [HttpPost]
//         public string ConsultarPaquetesNoPagados(int cuentaId)
//         {
//             try
//             {
//                 var paquetesNoPagados = _cuentasPorCobrarService.GetPaquetesNoPagadosHospitalizacion(cuentaId);
//                 return JsonSerializer.Serialize(new
//                 {
//                     Exitoso = true,
//                     Resultado = paquetesNoPagados
//                 });
//             }
//             catch
//             {
//                 return JsonSerializer.Serialize(new
//                 {
//                     Exitoso = false,
//                     Mensaje = "Error de conexion al consultar paquetes de hospitalizacion"
//                 });
//             }
//         }

//         // --- Agregar al final de CuentasPorCobrarController.cs ---

//         [HttpPost]
//         public async Task<string> GuardarHonorarioManual(HospitalizacionHonorario model)
//         {
//             try
//             {
//                 if (model.HospitalizacionId <= 0 || model.EmpleadoId <= 0 || model.Monto <= 0)
//                 {
//                     return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = "Datos del honorario inválidos." });
//                 }

//                 _context.HospitalizacionHonorarios.Add(model);
//                 await _context.SaveChangesAsync();

//                 return JsonSerializer.Serialize(new { Exitoso = true, Mensaje = "Honorario agregado correctamente." });
//             }
//             catch (Exception ex)
//             {
//                 return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
//             }
//         }

//         [HttpPost]
//         public async Task<string> EliminarHonorarioManual(int id)
//         {
//             try
//             {
//                 var honorario = await _context.HospitalizacionHonorarios.FindAsync(id);
//                 if (honorario == null) return JsonSerializer.Serialize(new { Exitoso = false });

//                 _context.HospitalizacionHonorarios.Remove(honorario);
//                 await _context.SaveChangesAsync();

//                 return JsonSerializer.Serialize(new { Exitoso = true });
//             }
//             catch (Exception ex)
//             {
//                 return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
//             }
//         }

//         [HttpPost]
//         public string ConsultarHonorariosHospitalizacion(int hospitalizacionId)
//         {
//             try
//             {
//                 var lista = _context.HospitalizacionHonorarios
//                     .Where(h => h.HospitalizacionId == hospitalizacionId)
//                     .Select(h => new
//                     {
//                         h.Id,
//                         h.EmpleadoId,
//                         NombreMedico = h.Empleado.NombreYApellidos, 
//                         h.Monto
//                     }).ToList();

//                 return JsonSerializer.Serialize(new { Exitoso = true, Resultado = lista });
//             }
//             catch (Exception ex)
//             {
//                 return JsonSerializer.Serialize(new { Exitoso = false, Mensaje = ex.Message });
//             }
//         }
//     }
// }