using Database.Shared.Data;
using Database.Shared.Enumeraciones;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using sistema.Models;
using System.Collections.Generic;
using System;
using System.Collections.Immutable;
using System.Linq;
using Wkhtmltopdf.NetCore;
using System.Text.Json;
using DocumentFormat.OpenXml.EMMA;
using System.Globalization;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Bibliography;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using sistema.Service.IService;
using farmamest.Service.IService;

namespace farmamest.Controllers
{
    public class GraficaController : Controller
    {

        private readonly IGrafica _graficaRepository = null;
        private IUser _userRepository;
        private DateTime fechaIncial;
        private readonly UserManager<User> _userManager = null;

        //Servicios (logica de negocio)
        private IGraficasService _graficasService = null;
        private IProductosService _productosService = null;


        public GraficaController(
        IGrafica graficaRepository,
        IUser userRepository,
        UserManager<User> userManager,
        //Servicio (logica de negocio)
        IGraficasService graficasService,
        IProductosService productosService)
        {
            _graficaRepository = graficaRepository;
            _userManager = userManager;
            _userRepository = userRepository;

            //Servicios (logica de negocio)
            _graficasService = graficasService;
            _productosService = productosService;
        }
        public IActionResult IndexGrafica()
        {
            var prueba = _graficaRepository.GetVentasGenerales();
            var ventasClinica = _graficaRepository.GetVentasClinica();
            var ventasFarmacia = _graficaRepository.GetVentasFarmacia();
            //var ventasLaboratorio = _graficaRepository.GetVentaLaboratorio();


            var montosClinicaPorMes = ventasClinica
              .GroupBy(v => v.FechaVenta.Month)
              .Select(g => new VentaClinicaViewModel { Mes = g.Key, MontoTotal = g.Sum(v => v.MontoPago) })
              .ToList();

            var montosFarmaciaPorMes = ventasFarmacia
                .GroupBy(v => v.FechaVenta.Month)
                .Select(g => new VentaFarmaciaViewModel { Mes = g.Key, MontoTotal = g.Sum(v => v.MontoPago) })
                .ToList();

            //var montosLaboratorioPorMes = ventasLaboratorio
            //    .GroupBy(v => v.FechaVenta.Month)
            //    .Select(g => new VentaLaboratorioViewModel { Mes = g.Key, MontoTotal = g.Sum(v => v.MontoPagado) })
            //    .ToList();


            var model = new GraficaBaseViewModel
            {
                VentasClinica = montosClinicaPorMes,
                VentasFarmacia = montosFarmaciaPorMes,
                //VentasLaboratorio = montosLaboratorioPorMes
            };
            return View(model);
        }

        public JsonResult VentasGeneraLes()
        {

            try
            {
                //Trae todas la ventas
                var ventasGenerales = _graficaRepository.GetVentasGenerales();
                var ventasGneralesDia = _graficaRepository.GetVentasGeneralesxDia();



                var model = new GraficaBaseViewModel();
                var VentasXMes = new List<GraficaVentasMesViewModel>();
                foreach (var (mes, indice) in model.Meses.Select((mes, indice) => (mes, indice)))
                {
                    var ventasMes = ventasGenerales[indice];

                    var ventasxMes = new GraficaVentasMesViewModel
                    {
                        Name = mes,
                        y = ventasMes,
                        drilldown = mes,
                    };

                    VentasXMes.Add(ventasxMes);
                }


                return Json(new
                {

                    ventasMes = VentasXMes,
                    ventasDiaMes = ventasGneralesDia,
                });

                //var ventasClinica = _graficaRepository.GetVentasClinica();
                //var ventasFarmacia = _graficaRepository.GetVentasFarmacia();
                //var ventasLaboratorio = _graficaRepository.GetVentaLaboratorio();

                //// Crear una lista para almacenar las ventas totales por mes
                //var todasLasVentasPorMes = new List<decimal>();

                //// Iterar sobre todos los meses del año (de 1 a 12)

                //var model = new GraficaBaseViewModel();
                //for (int mes = 1; mes <= 12; mes++)
                //{
                //    // Filtrar las ventas por el mes actual
                //    var ventasDelMes = ventasClinica
                //        .Where(v => v.FechaVenta.Month == mes)
                //        .Select(v => new { Mes = v.FechaVenta.Month, Monto = v.MontoPago })
                //        .Concat(ventasFarmacia.Where(v => v.FechaVenta.Month == mes)
                //        .Select(v => new { Mes = v.FechaVenta.Month, Monto = v.MontoPago }))
                //        .Concat(ventasLaboratorio.Where(v => v.FechaVenta.Month == mes)
                //        .Select(v => new { Mes = v.FechaVenta.Month, Monto = v.MontoPagado }))
                //        .ToList();

                //    // Calcular el monto total para el mes actual
                //    decimal montoTotal = ventasDelMes.Any() ? ventasDelMes.Sum(v => v.Monto) : 0;

                //    // Crear un objeto VentaTotalViewModel para el mes actual

                //    // Agregar el objeto VentaTotalViewModel a la lista
                //    todasLasVentasPorMes.Add(montoTotal);
                //}


                //var VentasXMes = new List<GraficaVentasMesViewModel>();
                //foreach (var (mes, indice) in model.Meses.Select((mes, indice) => (mes, indice)))
                //{
                //    var nombremes = mes;
                //    var ventas = todasLasVentasPorMes[indice];

                //    var ventasxMes = new GraficaVentasMesViewModel
                //    {
                //        Name = mes,
                //        y = ventas,
                //        drilldown = mes,
                //    };

                //    VentasXMes.Add(ventasxMes);
                //}

                //var ventasxAño = new List<GraficaVentasDiaViewModel>();

                //// Iterar sobre todos los meses del año (de 1 a 12)
                //for (int mes = 1; mes <= 12; mes++)
                //{
                //    var ventaxdia = new List<DataPoint>(); // Crear una lista para almacenar las ventas por día para el mes actual

                //    int diasEnElMes = DateTime.DaysInMonth(DateTime.Now.Year, mes); // Obtener el número de días en el mes actual

                //    // Iterar sobre todos los días del mes (de 1 al número de días en el mes)
                //    for (int dia = 1; dia <= diasEnElMes; dia++)
                //    {
                //        // Filtrar las ventas por el mes y día actual
                //        var ventasDelDia = ventasClinica
                //            .Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                //            .Select(v => new { Mes = v.FechaVenta.Month, Dia = v.FechaVenta.Day, Monto = v.MontoPago })
                //            .Concat(ventasFarmacia.Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                //            .Select(v => new { Mes = v.FechaVenta.Month, Dia = v.FechaVenta.Day, Monto = v.MontoPago }))
                //            .Concat(ventasLaboratorio.Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                //            .Select(v => new { Mes = v.FechaVenta.Month, Dia = v.FechaVenta.Day, Monto = v.MontoPagado }))
                //            .ToList();

                //        // Calcular el monto total para el día actual
                //        decimal montoTotal = ventasDelDia.Any() ? ventasDelDia.Sum(v => v.Monto) : 0;

                //        // Agregar el monto total a la lista de ventas por día para este mes
                //        ventaxdia.Add(new DataPoint { X = dia, Y = montoTotal });
                //    }

                //    var culturaEspañola = new CultureInfo("es-ES");
                //    var nombreMes = culturaEspañola.DateTimeFormat.GetMonthName(mes);
                //    nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1); // Convertir la primera letra a mayúscula

                //    var ventasxDia = new GraficaVentasDiaViewModel
                //    {
                //        Name = nombreMes,
                //        Id = nombreMes,
                //        Data = ventaxdia
                //    };

                //    ventasxAño.Add(ventasxDia); // Agregar las ventas por día para este mes a la lista de todas las ventas por mes
                //}
                //return Json(new
                //{

                //    ventasMes = VentasXMes,
                //    ventasAnio = ventasxAño,

                //});

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Error observar las ventas. " + ex.Message
                });
            }

        }

        public JsonResult PagosGenerales()
        {

            try
            {


                var pagosGenerales = _graficaRepository.GetPagosGenerales();
                var pagosGeneralesDia = _graficaRepository.GetPagosGeneralesxDia();

                var model = new GraficaBaseViewModel();
                var pagos = new List<GraficaPagosMesViewModel>();
                foreach (var (mes, indice) in model.Meses.Select((mes, indice) => (mes, indice)))
                {
                    var PagosMontoMes = pagosGenerales[indice];

                    var pagosxMes = new GraficaPagosMesViewModel
                    {
                        Name = mes,
                        y = PagosMontoMes,
                        drilldown = mes,
                    };

                    pagos.Add(pagosxMes);
                }


                return Json(new
                {

                    pagos = pagos,
                    pagosdia = pagosGeneralesDia
                });



            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Error observar las ventas. " + ex.Message
                });
            }

        }


        public JsonResult IngresosGenerales()
        {



            try
            {
                var ingresosGenerales = _graficaRepository.GetIngresosGenerales();
                var ingresosGeneralesDia = _graficaRepository.GetIngresosGeneralesxDia();

                var model = new GraficaBaseViewModel();
                var ingresos = new List<GraficaIngresosMesViewModel>();
                foreach (var (mes, indice) in model.Meses.Select((mes, indice) => (mes, indice)))
                {
                    var ingresosMontoMes = ingresosGenerales[indice];

                    var ingresosxMes = new GraficaIngresosMesViewModel
                    {
                        Name = mes,
                        y = ingresosMontoMes,
                        drilldown = mes,
                    };

                    ingresos.Add(ingresosxMes);
                }


                return Json(new
                {
                    Ingresos = ingresos,
                    IngresosDia = ingresosGeneralesDia
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Error observar los ingresos generales. " + ex.Message
                });
            }

        }

        public JsonResult IngresosVentasGenerales()
        {



            try
            {
                var ingresosVentasGenerales = _graficaRepository.GetIngresosVentasGenerales();
                var ingresosVentasGeneralesDia = _graficaRepository.GetIngresosVentasGeneralesxDia();

                var model = new GraficaBaseViewModel();
                var ingresosVentas = new List<GraficaIngresosMesViewModel>();
                foreach (var (mes, indice) in model.Meses.Select((mes, indice) => (mes, indice)))
                {
                    var ingresosVentasMontoMes = ingresosVentasGenerales[indice];

                    var ingresosxMes = new GraficaIngresosMesViewModel
                    {
                        Name = mes,
                        y = ingresosVentasMontoMes,
                        drilldown = mes,
                    };

                    ingresosVentas.Add(ingresosxMes);
                }


                return Json(new
                {
                    IngresosVentas = ingresosVentas,
                    IngresosVentasDia = ingresosVentasGeneralesDia
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Error observar los ingresos generales. " + ex.Message
                });
            }

        }

        [HttpPost]
        public string VentasAnnio(int annio)
        {
            try
            {
                Console.WriteLine(annio);

                var resultadoGraficasVentasAnnio = _graficasService.TabVentasAnuales(annio);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGraficasVentasAnnio.series,
                    drilldown = resultadoGraficasVentasAnnio.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar ventar por año: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string VentasRango(string fechaRango)
        {
            try
            {
                Console.WriteLine(fechaRango);

                var resultadoGraficasVentasxRango = _graficasService.TabVentasPorRango(fechaRango);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGraficasVentasxRango.series,
                    drilldown = resultadoGraficasVentasxRango.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar ventar por rango: " + ex.Message
                });
            }
        }
        public JsonResult GastosGenerales()
        {


            try
            {


                var gastosGenerales = _graficaRepository.GetGastosGenerales();
                var gastosGeneralesDia = _graficaRepository.GetgastosGeneralesxDia();

                var model = new GraficaBaseViewModel();
                var gastos = new List<GraficaGastosMesViewModel>();
                foreach (var (mes, indice) in model.Meses.Select((mes, indice) => (mes, indice)))
                {
                    var gastosMontoMes = gastosGenerales[indice];

                    var gastosxMes = new GraficaGastosMesViewModel
                    {
                        Name = mes,
                        y = gastosMontoMes,
                        drilldown = mes,
                    };

                    gastos.Add(gastosxMes);
                }

                return Json(new
                {
                    gastos = gastos,
                    gastosDia = gastosGeneralesDia
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Error observar los gastos generales. " + ex.Message
                });
            }





        }
        public JsonResult GastosComprasGenerales()
        {
            try
            {
                var gastosComprasGenerales = _graficaRepository.GetGastosComprasGenerales();
                var gastosCompraGeneralesDia = _graficaRepository.GetGastosComprasGeneralesxDia();

                var model = new GraficaBaseViewModel();
                var gastosCompras = new List<GraficaIngresosMesViewModel>();
                foreach (var (mes, indice) in model.Meses.Select((mes, indice) => (mes, indice)))
                {
                    var gastoComprasMontoMes = gastosComprasGenerales[indice];

                    var gastoComprasxMes = new GraficaIngresosMesViewModel
                    {
                        Name = mes,
                        y = gastoComprasMontoMes,
                        drilldown = mes,
                    };

                    gastosCompras.Add(gastoComprasxMes);
                }


                return Json(new
                {
                    GastosCompras = gastosCompras,
                    GastosComprasDia = gastosCompraGeneralesDia
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exitoso = false,
                    Mensaje = "Error observar los ingresos generales. " + ex.Message
                });
            }

        }
        [HttpPost]
        public string TabGeneralComprasVentasGanancias(int annio)
        {
            try
            {
                var resultadoGraficaVentasComprasGanancias = _graficasService.TabGeneralGetVentasComprasGanancias(annio);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGraficaVentasComprasGanancias.series,
                    drilldown = resultadoGraficaVentasComprasGanancias.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar compras, ventas y ganancias: " + ex.Message
                });
            }
        }


        [HttpPost]
        public string TabGeneralCompras(int annio)
        {
            try
            {
                var resultadoGraficaVentasComprasGanancias = _graficasService.TabGeneralGetCompras(annio);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGraficaVentasComprasGanancias.series,
                    drilldown = resultadoGraficaVentasComprasGanancias.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar compras: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string TendenciasVentas(int annio)
        {
            try
            {
                var resultadoGraficasTendenciasVentas = _graficasService.TabTendenciasVentas(annio);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGraficasTendenciasVentas.series,
                    drilldown = resultadoGraficasTendenciasVentas.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar tendencias de ventas: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string TabProductosVentasMensuales(int annio, int productoId)
        {
            try
            {
                var resultadoGraficaVentasMensuales = _graficasService.TabProductosVentasMensuales(annio, productoId);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGraficaVentasMensuales.series,
                    drilldown = resultadoGraficaVentasMensuales.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar ventas mensuales de producto: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string TabProductosComparacionPreciosProveedor(int productoId)
        {
            try
            {
                var resultadoGrafico = _graficasService.TabProductosComparacionPreciosProveedor(productoId);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGrafico.series,
                    drilldown = resultadoGrafico.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar ventas mensuales de producto: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string TabProductosMasVendidos()
        {
            try
            {
                var resultadoGrafico = _graficasService.TabProductosMasVendidos();
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGrafico.series,
                    drilldown = resultadoGrafico.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos mas vendidos: " + ex.Message
                });
            }
        }


        [HttpPost]
        public string TabProductosMasVendidosServiciosSolicitudes(string fechaRango)
        {
            try
            {
                var resultadoGrafico = _graficasService.TabProductosMasVendidosServiciosSolicitudes(fechaRango);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGrafico.series,
                    drilldown = resultadoGrafico.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos mas vendidos servicios solicitudes: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string TabVentasPorMedico()
        {
            try
            {
                var resultadoGrafico = _graficasService.TabVentasPorMedico();
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGrafico.series,
                    drilldown = resultadoGrafico.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos mas vendidos servicios solicitudes: " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ComprasProveedor()
        {
            try
            {
                var resultadoGrafico = _graficasService.ComprasProveedor();
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGrafico.series,
                    drilldown = resultadoGrafico.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar compras proveedor: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string TabProductosMenosVendidos()
        {
            try
            {
                var resultadoGrafico = _graficasService.TabProductosMenosVendidos();
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGrafico.series,
                    drilldown = resultadoGrafico.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos menos vendidos: " + ex.Message
                });
            }
        }


        [HttpPost]
        public string PacientesAtendidosPorServicio()
        {
            try
            {
                var resultadoGrafico = _graficasService.PacientesAtendidosPorServicio();
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    series = resultadoGrafico.series,
                    drilldown = resultadoGrafico.drilldown
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar PacientesAtendidosPorServicios: " + ex.Message
                });
            }
        }

        [HttpPost]
        public string ConsultarProductos(int? ambienteId)
        {
            try
            {
                var productos = _productosService.GetInventario(ambienteId,null);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = productos
                });
            }
            catch
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false
                });
            }
        }
    }



}

