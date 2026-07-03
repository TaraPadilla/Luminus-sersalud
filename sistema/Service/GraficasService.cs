using Database.Shared.Dto;
using Database.Shared.Enumeraciones;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using sistema.Models.Graficas;
using sistema.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sistema.Service
{
    public class GraficasService : IGraficasService
    {
        private readonly IVenta _ventaRepository = null;
        private readonly ICompra _compraRepository = null;
        private readonly ICaja _cajaRepository = null;
        private readonly IProducto _productoRepository = null;

        private readonly IServicio _servicioRepository = null;

        private readonly ISolicitudMedicamentoRepository _solicitudRepository = null;

        private readonly IEmpleado _empleadoRepository = null;

        private readonly IConsultas _consultaRepository = null;

        private readonly ICitas _citaRepository = null;

        private readonly IConsultasService _consultasService = null;

        private readonly ILaboratorioClinico _laboratorioService = null;

        private readonly IProveedor _proveedorService = null;

        public GraficasService(IVenta ventaRepository,
            ICompra compraRepository,
            ICaja cajaRepository,
            IProducto productoRepository,
            IServicio servicioRepository,
            ISolicitudMedicamentoRepository solicitudRepository,
            IEmpleado empleadoRepository,
            IConsultas consultaRepository,
            ICitas citaRepository,
            IConsultasService consultasService,
            ILaboratorioClinico laboratorioService,
            IProveedor proveedorService)
        {
            _ventaRepository = ventaRepository;
            _compraRepository = compraRepository;
            _cajaRepository = cajaRepository;
            _productoRepository = productoRepository;
            _servicioRepository = servicioRepository;
            _solicitudRepository = solicitudRepository;
            _empleadoRepository = empleadoRepository;
            _consultaRepository = consultaRepository;
            _citaRepository = citaRepository;
            _consultasService = consultasService;
            _laboratorioService = laboratorioService;
            _proveedorService = proveedorService;

        }

        public string GetNombreMes(int numeroMes)
        {
            var nombreMes = "";

            switch (numeroMes)
            {
                case 1: nombreMes = "Enero"; break;
                case 2: nombreMes = "Febrero"; break;
                case 3: nombreMes = "Marzo"; break;
                case 4: nombreMes = "Abril"; break;
                case 5: nombreMes = "Mayo"; break;
                case 6: nombreMes = "Junio"; break;
                case 7: nombreMes = "Julio"; break;
                case 8: nombreMes = "Agosto"; break;
                case 9: nombreMes = "Septiembre"; break;
                case 10: nombreMes = "Octubre"; break;
                case 11: nombreMes = "Noviembre"; break;
                case 12: nombreMes = "Diciembre"; break;
                default:
                    break;
            }

            return nombreMes;
        }

        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosVentasMensuales(int annio, int productoId)
        {
            var series = new List<GraficaColumnSeries>();
            var drilldown = new List<GraficaColumnSeries>();

            var ventasDetalles = _ventaRepository.GetListadoDetalles();
            var ventasGeneral = _ventaRepository.GetList();
            var infoProducto = _productoRepository.GetProdutoById(productoId);
            string productoNombre = infoProducto?.NombreProducto ?? "Producto " + productoId;

            var seriesDataMensual = new List<GraficaColumnData>();

            for (int mes = 1; mes <= 12; mes++)
            {
                decimal totalVentasMes = 0;
                var drilldownDataDias = new List<GraficaColumnData>();
                string drilldownId = $"dr-mes-{mes}";

                // Filtrar ventas por Año y Mes
                var ventasDelMes = ventasGeneral.Where(v => v.FechaVenta.Year == annio && v.FechaVenta.Month == mes);

                // Agrupar por Día para el Drilldown
                var ventasPorDia = ventasDelMes.GroupBy(v => v.FechaVenta.Day).OrderBy(g => g.Key);

                foreach (var grupoDia in ventasPorDia)
                {
                    int dia = grupoDia.Key;
                    decimal acumuladoDia = 0;

                    foreach (var venta in grupoDia)
                    {
                        // Match: Detalle que pertenece a esta venta Y es del producto solicitado
                        var detalles = ventasDetalles.Where(d => d.VentaId == venta.Id && d.ProductoId == productoId);

                        foreach (var det in detalles)
                        {
                            acumuladoDia += (decimal)(det.Precio * det.Cantidad);
                        }
                    }

                    if (acumuladoDia > 0)
                    {
                        totalVentasMes += acumuladoDia;
                        drilldownDataDias.Add(new GraficaColumnData
                        {
                            name = $"Día {dia}",
                            y = acumuladoDia,
                            drilldown = $"v-mes-{mes}-dia-{dia}"
                        });
                    }
                }

                // Agregar al nivel principal (Meses)
                seriesDataMensual.Add(new GraficaColumnData
                {
                    name = GetNombreMes(mes),
                    y = totalVentasMes,
                    drilldown = totalVentasMes > 0 ? drilldownId : null
                });


                drilldown.Add(new GraficaColumnSeries
                {
                    id = drilldownId,
                    name = $"Detalle Diario - {GetNombreMes(mes)}",
                    data = drilldownDataDias
                });
            }

            series.Add(new GraficaColumnSeries
            {
                name = productoNombre,
                data = seriesDataMensual
            });

            return (series, drilldown);
        }

        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosComparacionPreciosProveedor(int productoId)
        {
            var series = new List<GraficaColumnSeries>();
            var drilldown = new List<GraficaColumnSeries>();

            var compras = _compraRepository.GetListaTodas();
            var listaTemporal = new List<dynamic>();

            foreach (var c in compras)
            {
                var detalleCompras = _compraRepository.GetDetalles(c.Id);
                var detallesProducto = detalleCompras.Where(d => d.ProductoId == productoId).ToList();

                foreach (var d in detallesProducto)
                {

                    listaTemporal.Add(new
                    {
                        ProveedorId = c.ProveedorId,
                        ProveedorNombre = c.Proveedor?.Nombre ?? $"Proveedor {c.ProveedorId}",
                        PrecioNum = Convert.ToDouble(d.Precio),
                        FechaStr = c.FechaCompra.ToString("dd/MM/yyyy")
                    });
                }
            }

            if (!listaTemporal.Any()) return (series, drilldown);

            // --- PROCESAMIENTO DE SERIES ---
            var datosPrincipales = listaTemporal
                .GroupBy(x => new { x.ProveedorId, x.ProveedorNombre })
                .Select(g => new GraficaColumnData
                {
                    name = g.Key.ProveedorNombre,

                    y = (int)Math.Round(g.Average(x => (double)x.PrecioNum)),
                    drilldown = g.Key.ProveedorId.ToString()
                }).ToList();

            series.Add(new GraficaColumnSeries { name = "Precio Proveedor", data = datosPrincipales });

            // --- PROCESAMIENTO DE DRILLDOWNS ---
            var grupos = listaTemporal.GroupBy(x => new { x.ProveedorId, x.ProveedorNombre });

            foreach (var grupo in grupos)
            {
                var listaDetalle = new List<GraficaColumnData>();
                foreach (var item in grupo)
                {
                    listaDetalle.Add(new GraficaColumnData
                    {
                        name = item.FechaStr,
                        y = (int)Math.Round(item.PrecioNum)
                    });
                }

                drilldown.Add(new GraficaColumnSeries
                {
                    name = $"Histórico: {grupo.Key.ProveedorNombre}",
                    id = grupo.Key.ProveedorId.ToString(),
                    data = listaDetalle
                });
            }

            return (series, drilldown);
        }
        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosMasVendidos()
        {
            var series = new List<GraficaColumnSeries>();
            var seriesDataPrincipal = new List<GraficaColumnData>();
            var drilldown = new List<GraficaColumnSeries>();

            var ventasDetalles = _ventaRepository.GetListadoDetalles().ToList();
            var ventasGeneral = _ventaRepository.GetList().ToList();

            // Cálculo del Top 10
            var top10Ranking = ventasDetalles
                .Where(d => d.ProductoId.HasValue && d.ProductoId > 0)
                .GroupBy(d => d.ProductoId.Value)
                .Select(g => new { ProductoId = g.Key, TotalUnidades = g.Sum(d => d.Cantidad) })
                .OrderByDescending(x => x.TotalUnidades)
                .Take(10)
                .ToList();

            var top10Ids = top10Ranking.Select(x => x.ProductoId).ToList();

            var nombresProductos = top10Ids.ToDictionary(
                id => id,
                id => _productoRepository.GetProdutoById(id)?.NombreProducto ?? $"Producto {id}"
            );

            var detallesConAmbiente = (from det in ventasDetalles
                                       join v in ventasGeneral on det.VentaId equals v.Id
                                       where top10Ids.Contains(det.ProductoId ?? 0)
                                       select new
                                       {
                                           det.ProductoId,
                                           det.Cantidad,
                                           v.AmbienteId
                                       }).ToList();

            foreach (var item in top10Ranking)
            {
                string nombreProducto = nombresProductos[item.ProductoId];
                string drilldownId = $"dr-prod-{item.ProductoId}";

                seriesDataPrincipal.Add(new GraficaColumnData
                {
                    name = nombreProducto,
                    y = (decimal)item.TotalUnidades,
                    drilldown = drilldownId
                });

                var drilldownDataAmbientes = detallesConAmbiente
                    .Where(x => x.ProductoId == item.ProductoId)
                    .GroupBy(x => x.AmbienteId)
                    .Select(g => new GraficaColumnData
                    {
                        name = (g.Key.HasValue && Enum.IsDefined(typeof(AmbienteEnum), g.Key.Value))
            ? ((AmbienteEnum)g.Key.Value).ToString()
            : "Otros",
                        y = g.Sum(x => (decimal)x.Cantidad)
                    })
                    .Where(x => (decimal)x.y > 0)
                    .ToList();

                drilldown.Add(new GraficaColumnSeries
                {
                    id = drilldownId,
                    name = $"Distribución: {nombreProducto}",
                    data = drilldownDataAmbientes
                });
            }

            series.Add(new GraficaColumnSeries
            {
                name = "Cantidad Vendida (Histórico)",
                data = seriesDataPrincipal
            });

            return (series, drilldown);
        }


        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabVentasPorMedico()
        {
            var series = new List<GraficaColumnSeries>();
            var seriesDataPrincipal = new List<GraficaColumnData>();
            var drilldown = new List<GraficaColumnSeries>();

            var ventasDetalles = _ventaRepository.GetListadoDetalles().ToList();
            var ventasGeneral = _ventaRepository.GetList().ToList();


            foreach (var general in ventasGeneral)
            {
                Console.WriteLine($"--- VENTA ID: {general.Id} (Empleado: {general.EmpleadoId}) ---");

                if (general.EmpleadoId.HasValue)
                {
                    var empleado = _empleadoRepository.Get(general.EmpleadoId.Value);
                    Console.WriteLine($"NombreMedico:  {empleado.NombreYApellidos}");
                }

                foreach (var detalle in ventasDetalles)
                {
                    if (detalle.VentaId == general.Id)
                    {
                        string nombreProducto = "N/A";
                        string nombreServicio = "N/A";

                        if (detalle.ProductoId.HasValue)
                        {
                            var producto = _productoRepository.GetProdutoById(detalle.ProductoId.Value);
                            nombreProducto = producto.NombreProducto ?? "No encontrado";
                        }

                        if (detalle.ServicioId.HasValue)
                        {
                            var servicio = _servicioRepository.Get(detalle.ServicioId.Value);
                            nombreServicio = servicio.NombreServicio ?? "No encontrado";
                        }

                        Console.WriteLine($"   > Producto: {nombreProducto} | Precio: {detalle.Precio} | Cantidad: {detalle.Cantidad}");
                    }
                }

                // Línea en blanco para separar visualmente cada bloque de venta
                Console.WriteLine(new string('-', 50));
            }



            return (series, drilldown);


        }


        // public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) PacientesAtendidosPorServicio()
        // {
        //     var series = new List<GraficaColumnSeries>();
        //     var seriesDataPrincipal = new List<GraficaColumnData>();
        //     var drilldown = new List<GraficaColumnSeries>();

        //     var consultasPaciente = _consultaRepository.ListaConsultas().ToList();
        //     var citas = _citaRepository.GetAll().ToList();

        //     // para agrupar pacientes únicos por ServicioId
        //     var conteoPacientesPorServicio = new Dictionary<int, HashSet<int>>();

        //     foreach (var consulta in consultasPaciente)
        //     {
        //         var cita = citas.FirstOrDefault(c => c.Id == consulta.CitasId);

        //         // que la cita exista y tenga PacienteId
        //         if (cita != null && cita.PacienteId.HasValue)
        //         {
        //             var serviciosAsociados = _consultaRepository.GetServiciosAgregados(consulta.Id);

        //             foreach (var s in serviciosAsociados)
        //             {
        //                 if (!conteoPacientesPorServicio.ContainsKey(s.ServicioId))
        //                 {
        //                     conteoPacientesPorServicio[s.ServicioId] = new HashSet<int>();
        //                 }

        //                 conteoPacientesPorServicio[s.ServicioId].Add(cita.PacienteId.Value);
        //             }
        //         }
        //     }

        //     foreach (var item in conteoPacientesPorServicio)
        //     {
        //         var servicioInfo = _servicioRepository.Get(item.Key);
        //         string nombreServicio = servicioInfo != null ? servicioInfo.NombreServicio : $"Servicio {item.Key}";

        //         seriesDataPrincipal.Add(new GraficaColumnData
        //         {
        //             name = nombreServicio,
        //             y = item.Value.Count, 
        //             drilldown = item.Key.ToString() 
        //         });
        //     }

        //     series.Add(new GraficaColumnSeries
        //     {
        //         name = "Pacientes Atendidos por Servicio",
        //         data = seriesDataPrincipal
        //     });

        //     return (series, drilldown);
        // }


        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) PacientesAtendidosPorServicio()
        {
            var series = new List<GraficaColumnSeries>();
            var seriesDataPrincipal = new List<GraficaColumnData>();
            var drilldown = new List<GraficaColumnSeries>();

            var consultasPaciente = _consultaRepository.ListaConsultas().ToList();
            var citas = _citaRepository.GetAll().ToList();

            // Diccionario para agrupar pacientes únicos por ServicioId
            var conteoPacientesPorServicio = new Dictionary<int, HashSet<int>>();

            foreach (var consulta in consultasPaciente)
            {
                var cita = citas.FirstOrDefault(c => c.Id == consulta.CitasId);

                if (cita != null && cita.PacienteId.HasValue)
                {
                    var serviciosAsociados = _consultaRepository.GetServiciosAgregados(consulta.Id);

                    foreach (var s in serviciosAsociados)
                    {
                        if (!conteoPacientesPorServicio.ContainsKey(s.ServicioId))
                        {
                            conteoPacientesPorServicio[s.ServicioId] = new HashSet<int>();
                        }

                        conteoPacientesPorServicio[s.ServicioId].Add(cita.PacienteId.Value);
                    }
                }
            }

            // --- LÓGICA PARA EL TOP 10 ---
            // Ordenamos por la cantidad de elementos en el HashSet (pacientes únicos) y tomamos 10
            var topServicios = conteoPacientesPorServicio
                .OrderByDescending(x => x.Value.Count)
                .Take(10)
                .ToList();

            foreach (var item in topServicios)
            {
                var servicioInfo = _servicioRepository.Get(item.Key);
                string nombreServicio = servicioInfo != null ? servicioInfo.NombreServicio : $"Servicio {item.Key}";

                seriesDataPrincipal.Add(new GraficaColumnData
                {
                    name = nombreServicio,
                    y = item.Value.Count,
                    drilldown = item.Key.ToString()
                });

                // Aquí podrías agregar la lógica para llenar la lista 'drilldown' 
                // si deseas ver el detalle de esos pacientes al hacer clic.
            }

            series.Add(new GraficaColumnSeries
            {
                name = "Pacientes Atendidos por Servicio",
                data = seriesDataPrincipal
            });

            return (series, drilldown);
        }


        // public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosMasVendidosServiciosSolicitudes()
        // {
        //     var series = new List<GraficaColumnSeries>();
        //     var drilldown = new List<GraficaColumnSeries>();

        //     try
        //     {
        //         var ventasGeneral = _ventaRepository.GetList().ToList();

        //         foreach (var item in ventasGeneral)
        //         {
        //             var fecha = item.FechaVenta;   
        //         }

        //         var ventasDetalles = _ventaRepository.GetListadoDetalles().ToList();

        //         // --- 1. TOP 10 PRODUCTOS ---
        //         // (Mantiene la lógica anterior)
        //         var topProd = ventasDetalles
        //             .Where(d => d.ProductoId.HasValue && d.ProductoId > 0)
        //             .GroupBy(d => d.ProductoId.Value)
        //             .Select(g => new { Id = g.Key, Cant = g.Sum(x => x.Cantidad) })
        //             .OrderByDescending(x => x.Cant).Take(10).ToList();

        //         var dataP = new List<GraficaColumnData>();
        //         foreach (var p in topProd)
        //         {

        //             var prod = _productoRepository.GetProdutoById(p.Id);
        //             string nombre = prod?.NombreProducto ?? $"Producto {p.Id}";
        //             string dId = $"dr-p-{p.Id}";
        //             dataP.Add(new GraficaColumnData { name = nombre, y = (decimal)p.Cant, drilldown = dId });
        //             // ... (Drilldown de productos omitido por brevedad, mantiene misma lógica)
        //         }

        //         // --- 2. OBTENCIÓN Y FILTRADO DE EXÁMENES (LAB vs RX) ---
        //         // Primero obtenemos todos los exámenes únicos presentes en las ventas
        //         var examenesVendidosIds = ventasDetalles
        //             .Where(d => d.ExamenLabClinicoId.HasValue && d.ExamenLabClinicoId > 0)
        //             .Select(d => d.ExamenLabClinicoId.Value)
        //             .Distinct().ToList();

        //         // Clasificamos los exámenes por nombre (RX vs Otros)
        //         var diccExamenes = new Dictionary<int, (string Nombre, bool EsRadiologia)>();
        //         foreach (var id in examenesVendidosIds)
        //         {
        //             var exame = _laboratorioService.GetExamenLab(id);
        //             if (exame != null)
        //             {
        //                 bool esRX = exame.NombreExamen.Trim().StartsWith("RX", StringComparison.OrdinalIgnoreCase);
        //                 diccExamenes.Add(id, (exame.NombreExamen, esRX));
        //                 Console.WriteLine($"Clasificando: {exame.NombreExamen} | Radiología: {esRX}");
        //             }
        //         }

        //         // --- 3. TOP 10 RADIOLOGÍA (RX) ---
        //         var topRX = ventasDetalles
        //             .Where(d => d.ExamenLabClinicoId.HasValue && diccExamenes.ContainsKey(d.ExamenLabClinicoId.Value) && diccExamenes[d.ExamenLabClinicoId.Value].EsRadiologia)
        //             .GroupBy(d => d.ExamenLabClinicoId.Value)
        //             .Select(g => new { Id = g.Key, Cant = g.Sum(x => x.Cantidad) })
        //             .OrderByDescending(x => x.Cant).Take(10).ToList();

        //         var dataRX = new List<GraficaColumnData>();
        //         foreach (var r in topRX)
        //         {
        //             string nombre = diccExamenes[r.Id].Nombre;
        //             string dId = $"dr-rx-{r.Id}";
        //             dataRX.Add(new GraficaColumnData { name = nombre, y = (decimal)r.Cant, drilldown = dId });

        //             drilldown.Add(new GraficaColumnSeries
        //             {
        //                 id = dId,
        //                 name = nombre,
        //                 data = ventasDetalles.Where(x => x.ExamenLabClinicoId == r.Id).GroupBy(x => x.ProductoId)
        //                     .Select(g => new GraficaColumnData
        //                     {
        //                         name = _productoRepository.GetProdutoById(g.Key ?? 0)?.NombreProducto ?? "Material RX",
        //                         y = (decimal)g.Sum(p => p.Cantidad)
        //                     }).ToList()
        //             });
        //         }

        //         // --- 4. TOP 10 LABORATORIO (No RX) ---
        //         var topLab = ventasDetalles
        //             .Where(d => d.ExamenLabClinicoId.HasValue && diccExamenes.ContainsKey(d.ExamenLabClinicoId.Value) && !diccExamenes[d.ExamenLabClinicoId.Value].EsRadiologia)
        //             .GroupBy(d => d.ExamenLabClinicoId.Value)
        //             .Select(g => new { Id = g.Key, Cant = g.Sum(x => x.Cantidad) })
        //             .OrderByDescending(x => x.Cant).Take(10).ToList();

        //         var dataL = new List<GraficaColumnData>();
        //         foreach (var l in topLab)
        //         {
        //             string nombre = diccExamenes[l.Id].Nombre;
        //             string dId = $"dr-l-{l.Id}";
        //             dataL.Add(new GraficaColumnData { name = nombre, y = (decimal)l.Cant, drilldown = dId });
        //             // ... (Drilldown de laboratorio)
        //         }

        //         // --- 5. TOP 10 SERVICIOS GENERALES ---
        //         var topServ = ventasDetalles
        //             .Where(d => d.ServicioId.HasValue && d.ServicioId > 0 && (!d.ExamenLabClinicoId.HasValue || d.ExamenLabClinicoId == 0))
        //             .GroupBy(d => d.ServicioId.Value)
        //             .Select(g => new { Id = g.Key, Cant = g.Sum(x => x.Cantidad) })
        //             .OrderByDescending(x => x.Cant).Take(10).ToList();

        //         var dataS = new List<GraficaColumnData>();
        //         foreach (var s in topServ)
        //         {
        //             string nombre = _servicioRepository.Get(s.Id)?.NombreServicio ?? $"Servicio {s.Id}";
        //             dataS.Add(new GraficaColumnData { name = nombre, y = (decimal)s.Cant, drilldown = $"dr-s-{s.Id}" });
        //         }

        //         // Agregar las 4 series finales
        //         series.Add(new GraficaColumnSeries { name = "Top Productos", data = dataP });
        //         series.Add(new GraficaColumnSeries { name = "Top Servicios", data = dataS });
        //         series.Add(new GraficaColumnSeries { name = "Top Laboratorio", data = dataL });
        //         series.Add(new GraficaColumnSeries { name = "Top Radiología (RX)", data = dataRX });

        //     }
        //     catch (Exception ex) { Console.WriteLine("Error Crítico: " + ex.Message); }

        //     return (series, drilldown);
        // }


        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosMasVendidosServiciosSolicitudes(string rangoFecha = "")
        {
            var series = new List<GraficaColumnSeries>();
            var drilldown = new List<GraficaColumnSeries>();

            try
            {
                DateTime? fInicio = null;
                DateTime? fFin = null;

                if (!string.IsNullOrEmpty(rangoFecha))
                {
                    if (rangoFecha.Contains(" - "))
                    {
                        var partes = rangoFecha.Split(" - ");
                        if (DateTime.TryParse(partes[0], out DateTime fi)) fInicio = fi;
                        if (DateTime.TryParse(partes[1], out DateTime ff)) fFin = ff;
                    }
                    else if (DateTime.TryParse(rangoFecha, out DateTime f))
                    {
                        fInicio = f.Date;
                        fFin = f.Date.AddDays(1).AddTicks(-1);
                    }
                }

                var ventasGeneral = _ventaRepository.GetList();
                if (fInicio.HasValue) ventasGeneral = ventasGeneral.Where(v => v.FechaVenta >= fInicio.Value).ToList();
                if (fFin.HasValue) ventasGeneral = ventasGeneral.Where(v => v.FechaVenta <= fFin.Value).ToList();

                var diccVentas = ventasGeneral.ToDictionary(v => v.Id, v => v.FechaVenta);

                // Obtener detalles y filtrar por los IDs de las ventas que pasaron el filtro de fecha
                var todosLosDetalles = _ventaRepository.GetListadoDetalles();
                var ventasDetalles = todosLosDetalles
                    .Where(d => diccVentas.ContainsKey(d.VentaId))
                    .ToList();

                var topProd = ventasDetalles
                    .Where(d => d.ProductoId.HasValue && d.ProductoId > 0)
                    .GroupBy(d => d.ProductoId.Value)
                    .Select(g => new { Id = g.Key, Cant = g.Sum(x => x.Cantidad) })
                    .OrderByDescending(x => x.Cant).Take(10).ToList();

                var dataP = new List<GraficaColumnData>();
                foreach (var p in topProd)
                {
                    var prod = _productoRepository.GetProdutoById(p.Id);
                    string nombre = prod?.NombreProducto ?? $"Producto {p.Id}";
                    string dId = $"dr-p-{p.Id}";
                    dataP.Add(new GraficaColumnData { name = nombre, y = (decimal)p.Cant, drilldown = dId });

                    drilldown.Add(new GraficaColumnSeries
                    {
                        id = dId,
                        name = nombre,
                        data = ventasDetalles.Where(x => x.ProductoId == p.Id)
                            .GroupBy(x => diccVentas[x.VentaId].ToShortDateString())
                            .Select(g => new GraficaColumnData { name = g.Key, y = (decimal)g.Sum(z => z.Cantidad) }).ToList()
                    });
                }

                var examenesIds = ventasDetalles.Where(d => d.ExamenLabClinicoId > 0)
                                                .Select(d => d.ExamenLabClinicoId.Value).Distinct();
                var diccExamenes = new Dictionary<int, (string Nombre, bool EsRX)>();
                foreach (var id in examenesIds)
                {
                    var ex = _laboratorioService.GetExamenLab(id);
                    if (ex != null) diccExamenes.Add(id, (ex.NombreExamen, ex.NombreExamen.Trim().StartsWith("RX", StringComparison.OrdinalIgnoreCase)));
                }

                var topRX = ventasDetalles
                    .Where(d => d.ExamenLabClinicoId.HasValue && diccExamenes.ContainsKey(d.ExamenLabClinicoId.Value) && diccExamenes[d.ExamenLabClinicoId.Value].EsRX)
                    .GroupBy(d => d.ExamenLabClinicoId.Value)
                    .Select(g => new { Id = g.Key, Cant = g.Sum(x => x.Cantidad) })
                    .OrderByDescending(x => x.Cant).Take(10).ToList();

                var dataRX = new List<GraficaColumnData>();
                foreach (var r in topRX)
                {
                    string nombre = diccExamenes[r.Id].Nombre;
                    string dId = $"dr-rx-{r.Id}";
                    dataRX.Add(new GraficaColumnData { name = nombre, y = (decimal)r.Cant, drilldown = dId });

                    drilldown.Add(new GraficaColumnSeries
                    {
                        id = dId,
                        name = nombre,
                        data = ventasDetalles.Where(x => x.ExamenLabClinicoId == r.Id)
                            .GroupBy(x => x.ProductoId)
                            .Select(g => new GraficaColumnData
                            {
                                name = _productoRepository.GetProdutoById(g.Key ?? 0)?.NombreProducto ?? "Material RX",
                                y = (decimal)g.Sum(p => p.Cantidad)
                            }).ToList()
                    });
                }

                var topLab = ventasDetalles
                    .Where(d => d.ExamenLabClinicoId.HasValue && diccExamenes.ContainsKey(d.ExamenLabClinicoId.Value) && !diccExamenes[d.ExamenLabClinicoId.Value].EsRX)
                    .GroupBy(d => d.ExamenLabClinicoId.Value)
                    .Select(g => new { Id = g.Key, Cant = g.Sum(x => x.Cantidad) })
                    .OrderByDescending(x => x.Cant).Take(10).ToList();

                var dataL = new List<GraficaColumnData>();
                foreach (var l in topLab)
                {
                    string nombre = diccExamenes[l.Id].Nombre;
                    string dId = $"dr-l-{l.Id}";
                    dataL.Add(new GraficaColumnData { name = nombre, y = (decimal)l.Cant, drilldown = dId });

                    drilldown.Add(new GraficaColumnSeries
                    {
                        id = dId,
                        name = nombre,
                        data = ventasDetalles.Where(x => x.ExamenLabClinicoId == l.Id)
                            .GroupBy(x => diccVentas[x.VentaId].ToShortDateString())
                            .Select(g => new GraficaColumnData { name = g.Key, y = (decimal)g.Sum(z => z.Cantidad) }).ToList()
                    });
                }

                var topServ = ventasDetalles
                    .Where(d => d.ServicioId.HasValue && d.ServicioId > 0 && (!d.ExamenLabClinicoId.HasValue || d.ExamenLabClinicoId == 0))
                    .GroupBy(d => d.ServicioId.Value)
                    .Select(g => new { Id = g.Key, Cant = g.Sum(x => x.Cantidad) })
                    .OrderByDescending(x => x.Cant).Take(10).ToList();

                var dataS = new List<GraficaColumnData>();
                foreach (var s in topServ)
                {
                    string nombre = _servicioRepository.Get(s.Id)?.NombreServicio ?? $"Servicio {s.Id}";
                    string dId = $"dr-s-{s.Id}";
                    dataS.Add(new GraficaColumnData { name = nombre, y = (decimal)s.Cant, drilldown = dId });

                    drilldown.Add(new GraficaColumnSeries
                    {
                        id = dId,
                        name = nombre,
                        data = ventasDetalles.Where(x => x.ServicioId == s.Id)
                            .GroupBy(x => diccVentas[x.VentaId].ToShortDateString())
                            .Select(g => new GraficaColumnData { name = g.Key, y = (decimal)g.Sum(z => z.Cantidad) }).ToList()
                    });
                }

                series.Add(new GraficaColumnSeries { name = "Top Productos", data = dataP });
                series.Add(new GraficaColumnSeries { name = "Top Servicios", data = dataS });
                series.Add(new GraficaColumnSeries { name = "Top Laboratorio", data = dataL });
                series.Add(new GraficaColumnSeries { name = "Top Radiología (RX)", data = dataRX });
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }

            return (series, drilldown);
        }


        // public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) ComprasProveedor()
        // {
        //     var series = new List<GraficaColumnSeries>();
        //     var seriesDataPrincipal = new List<GraficaColumnData>();
        //     var drilldown = new List<GraficaColumnSeries>();

        //     var compras = _compraRepository.ListaComprados();   


        //     foreach (var c in compras)
        //     {

        //         var detalle = _compraRepository.GetDetalles(c.Id);


        //         foreach (var d in detalle)
        //         {

        //             var proveedor = _proveedorService.Get(c.ProveedorId);

        //             Console.WriteLine($"CompraID:{c.Id}  Proveedor{proveedor.Nombre} CompraIDCONEXION: {d.CompraId}  ProductoID{d.ProductoId}  Cantidad{d.Cantidad}  Precio{d.Precio}");

        //         }


        //     }

        //     return (series, drilldown);
        // }
        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) ComprasProveedor()
        {
            var seriesDataPrincipal = new List<GraficaColumnData>();
            var drilldownVacio = new List<GraficaColumnSeries>();

            var listaCompras = _compraRepository.ListaComprados() ?? new List<Compra>();
            var comprasAgrupadas = listaCompras.GroupBy(c => c.ProveedorId);

            foreach (var grupo in comprasAgrupadas)
            {
                var proveedor = _proveedorService.Get(grupo.Key);
                if (proveedor == null) continue;

                double totalCantidad = 0;
                foreach (var compra in grupo)
                {
                    var detalles = _compraRepository.GetDetalles(compra.Id) ?? new List<DetalleCompra>();
                    totalCantidad += detalles.Sum(d => (double)d.Cantidad);
                }

                seriesDataPrincipal.Add(new GraficaColumnData
                {
                    name = proveedor.Nombre,
                    y = totalCantidad
                });
            }

            var seriesFinal = new List<GraficaColumnSeries>
            {
                new GraficaColumnSeries
                {
                    name = "Monto",
                    data = seriesDataPrincipal
                }
            };

            return (seriesFinal, drilldownVacio);
        }
        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabProductosMenosVendidos()
        {
            var series = new List<GraficaColumnSeries>();
            var seriesDataPrincipal = new List<GraficaColumnData>();
            var drilldown = new List<GraficaColumnSeries>();

            var ventasDetalles = _ventaRepository.GetListadoDetalles().ToList();
            var ventasGeneral = _ventaRepository.GetList().ToList();

            var top10MenosVendidos = ventasDetalles
                .Where(d => d.ProductoId.HasValue && d.ProductoId > 0)
                .GroupBy(d => d.ProductoId.Value)
                .Select(g => new
                {
                    ProductoId = g.Key,
                    TotalUnidades = g.Sum(d => d.Cantidad)
                })
                .OrderBy(x => x.TotalUnidades) // Ascendente para los menos vendidos
                .Take(10)
                .ToList();

            var top10Ids = top10MenosVendidos.Select(x => x.ProductoId).ToList();

            var nombresProductos = top10Ids.ToDictionary(
                id => id,
                id => _productoRepository.GetProdutoById(id)?.NombreProducto ?? $"Producto {id}"
            );

            var detallesConAmbiente = (from det in ventasDetalles
                                       join v in ventasGeneral on det.VentaId equals v.Id
                                       where top10Ids.Contains(det.ProductoId ?? 0)
                                       select new
                                       {
                                           det.ProductoId,
                                           det.Cantidad,
                                           v.AmbienteId
                                       }).ToList();

            foreach (var item in top10MenosVendidos)
            {
                string nombreProducto = nombresProductos[item.ProductoId];
                string drilldownId = $"dr-prod-menos-{item.ProductoId}";

                seriesDataPrincipal.Add(new GraficaColumnData
                {
                    name = nombreProducto,
                    y = (decimal)item.TotalUnidades,
                    drilldown = drilldownId
                });

                var drilldownDataAmbientes = detallesConAmbiente
                    .Where(x => x.ProductoId == item.ProductoId)
                    .GroupBy(x => x.AmbienteId)
                    .Select(g => new GraficaColumnData
                    {
                        name = (g.Key.HasValue && Enum.IsDefined(typeof(AmbienteEnum), g.Key.Value))
                                ? ((AmbienteEnum)g.Key.Value).ToString()
                                : "Otros",
                        y = g.Sum(x => (decimal)x.Cantidad)
                    })
                    .Where(x => Convert.ToDecimal(x.y) > 0)
                    .ToList();

                drilldown.Add(new GraficaColumnSeries
                {
                    id = drilldownId,
                    name = $"Distribución: {nombreProducto}",
                    data = drilldownDataAmbientes
                });
            }

            series.Add(new GraficaColumnSeries
            {
                name = "Cantidad Vendida (Menos Vendidos)",
                data = seriesDataPrincipal
            });

            return (series, drilldown);
        }

        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabGeneralGetVentasComprasGanancias(int año)
        {
            var series = new List<GraficaColumnSeries>();
            var seriesDataVentas = new List<GraficaColumnData>();
            var seriesDataCompras = new List<GraficaColumnData>();
            var seriesDataGanancias = new List<GraficaColumnData>();
            var drilldown = new List<GraficaColumnSeries>();

            var detallesCaja = _cajaRepository.GetDetallesCaja(año);

            #region VentasIngresos

            if (detallesCaja != null)
            {
                for (int mes = 1; mes <= 12; mes++)
                {
                    decimal totalValorVentasMes = 0;
                    var ventasMes = detallesCaja
                        .Where(a => a.Fecha.Month == mes && a.Ingreso > 0)
                        .ToList();
                    if (ventasMes != null)
                    {
                        totalValorVentasMes += ventasMes.Sum(a => a.Ingreso);
                    }

                    seriesDataVentas.Add(new GraficaColumnData
                    {
                        name = GetNombreMes(mes),
                        y = totalValorVentasMes,
                        drilldown = "ventas-mes-" + mes
                    });
                }
            }

            series.Add(new GraficaColumnSeries
            {
                name = "Ventas + Ingresos",
                data = seriesDataVentas
            });

            #endregion

            #region ComprasEgresos

            if (detallesCaja != null)
            {
                for (int mes = 1; mes <= 12; mes++)
                {
                    decimal totalValorComprasMes = 0;
                    var comprasMes = detallesCaja
                        .Where(a => a.Fecha.Month == mes && a.Gasto > 0)
                        .ToList();
                    if (comprasMes != null)
                    {
                        totalValorComprasMes += comprasMes.Sum(a => a.Gasto);
                    }

                    seriesDataCompras.Add(new GraficaColumnData
                    {
                        name = GetNombreMes(mes),
                        y = totalValorComprasMes,
                        drilldown = "compras-mes-" + mes
                    });
                }
            }

            series.Add(new GraficaColumnSeries
            {
                name = "Compras + Egresos",
                data = seriesDataCompras
            });

            #endregion

            #region Ganancias

            if (detallesCaja != null)
            {
                for (int mes = 1; mes <= 12; mes++)
                {
                    //Calculo de ventas del mes
                    decimal totalValorVentasMes = 0;
                    var ventasMes = detallesCaja
                        .Where(a => a.Fecha.Month == mes && a.Ingreso > 0)
                        .ToList();
                    if (ventasMes != null)
                    {
                        totalValorVentasMes += ventasMes.Sum(a => a.Ingreso);
                    }
                    //Calculo de compras del mes
                    decimal totalValorComprasMes = 0;
                    var comprasMes = detallesCaja
                        .Where(a => a.Fecha.Month == mes && a.Gasto > 0)
                        .ToList();
                    if (comprasMes != null)
                    {
                        totalValorComprasMes += comprasMes.Sum(a => a.Gasto);
                    }

                    //Calculo de ganancias
                    decimal valorGanancias = totalValorVentasMes - totalValorComprasMes;

                    seriesDataGanancias.Add(new GraficaColumnData
                    {
                        name = GetNombreMes(mes),
                        y = valorGanancias,
                        drilldown = "ganancias-mes-" + mes
                    });
                }
            }

            series.Add(new GraficaColumnSeries
            {
                name = "Ganancias",
                data = seriesDataGanancias
            });

            #endregion


            #region Datos por dia

            for (int mes = 1; mes <= 12; mes++)
            {
                var diasDelMes = DateTime.DaysInMonth(año, mes);

                var drilldownDataVentasMes = new List<GraficaColumnData>();
                var drilldownDataComprasMes = new List<GraficaColumnData>();
                var drilldownDataGananciasMes = new List<GraficaColumnData>();

                for (int dia = 1; dia <= diasDelMes; dia++)
                {

                    var drilldownDataVentasDiaAmbiente = new List<GraficaColumnData>();
                    var drilldownDataComprasDiaAmbiente = new List<GraficaColumnData>();
                    var drilldownDataGananciasDiaAmbiente = new List<GraficaColumnData>();


                    var ventasDia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia).Sum(a => a.Ingreso);
                    var ventasDiaHospital = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Hospital).Sum(a => a.Ingreso);
                    var ventasDiaClinica = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Clinica).Sum(a => a.Ingreso);
                    var ventasDiaFarmacia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Farmacia).Sum(a => a.Ingreso);
                    var ventasDiaLaboratorio = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Laboratorio).Sum(a => a.Ingreso);

                    var comprasDia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia).Sum(a => a.Gasto);
                    var comprasDiaHospital = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Hospital).Sum(a => a.Gasto);
                    var comprasDiaClinica = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Clinica).Sum(a => a.Gasto);
                    var comprasDiaFarmacia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Farmacia).Sum(a => a.Gasto);
                    var comprasDiaLaboratorio = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Laboratorio).Sum(a => a.Gasto);

                    var gananciasDia = ventasDia - comprasDia;
                    var gananciasDiaHospital = ventasDiaHospital - comprasDiaHospital;
                    var gananciasDiaClinica = ventasDiaClinica - comprasDiaClinica;
                    var gananciasDiaFarmacia = ventasDiaFarmacia - comprasDiaFarmacia;
                    var gananciasDiaLaboratorio = ventasDiaLaboratorio - comprasDiaLaboratorio;


                    #region Ventas

                    drilldownDataVentasMes.Add(new GraficaColumnData
                    {
                        name = "Dia " + dia,
                        y = ventasDia,
                        drilldown = "ventas-mes-" + mes + "-dia-" + dia
                    });
                    drilldownDataVentasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Hospital",
                        y = ventasDiaHospital
                    });
                    drilldownDataVentasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Clinica",
                        y = ventasDiaClinica
                    });
                    drilldownDataVentasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Farmacia",
                        y = ventasDiaFarmacia
                    });
                    drilldownDataVentasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Laboratorio",
                        y = ventasDiaLaboratorio
                    });
                    drilldown.Add(new GraficaColumnSeries
                    {
                        id = "ventas-mes-" + mes + "-dia-" + dia,
                        name = "Valor Q",
                        data = drilldownDataVentasDiaAmbiente
                    });

                    #endregion


                    #region Compras

                    drilldownDataComprasMes.Add(new GraficaColumnData
                    {
                        name = "Dia " + dia,
                        y = comprasDia,
                        drilldown = "compras-mes-" + mes + "-dia-" + dia
                    });
                    drilldownDataComprasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Hospital",
                        y = comprasDiaHospital
                    });
                    drilldownDataComprasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Clinica",
                        y = comprasDiaClinica
                    });
                    drilldownDataComprasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Farmacia",
                        y = comprasDiaFarmacia
                    });
                    drilldownDataComprasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Laboratorio",
                        y = comprasDiaLaboratorio
                    });
                    drilldown.Add(new GraficaColumnSeries
                    {
                        id = "compras-mes-" + mes + "-dia-" + dia,
                        name = "Valor Q",
                        data = drilldownDataComprasDiaAmbiente
                    });

                    #endregion

                    #region Ganancias

                    drilldownDataGananciasMes.Add(new GraficaColumnData
                    {
                        name = "Dia " + dia,
                        y = gananciasDia,
                        drilldown = "ganancias-mes-" + mes + "-dia-" + dia
                    });
                    drilldownDataGananciasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Hospital",
                        y = gananciasDiaHospital
                    });
                    drilldownDataGananciasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Clinica",
                        y = gananciasDiaClinica
                    });
                    drilldownDataGananciasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Farmacia",
                        y = gananciasDiaFarmacia
                    });
                    drilldownDataGananciasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Laboratorio",
                        y = gananciasDiaLaboratorio
                    });
                    drilldown.Add(new GraficaColumnSeries
                    {
                        id = "ganancias-mes-" + mes + "-dia-" + dia,
                        name = "Valor Q",
                        data = drilldownDataGananciasDiaAmbiente
                    });

                    #endregion

                }
                drilldown.Add(new GraficaColumnSeries
                {
                    id = "ventas-mes-" + mes,
                    name = "Valor Q",
                    data = drilldownDataVentasMes
                });
                drilldown.Add(new GraficaColumnSeries
                {
                    id = "compras-mes-" + mes,
                    name = "Valor Q",
                    data = drilldownDataComprasMes
                });
                drilldown.Add(new GraficaColumnSeries
                {
                    id = "ganancias-mes-" + mes,
                    name = "Valor Q",
                    data = drilldownDataGananciasMes
                });
            }

            #endregion


            return (series, drilldown);
        }





        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabGeneralGetCompras(int año)
        {
            var series = new List<GraficaColumnSeries>();
            var seriesDataCompras = new List<GraficaColumnData>();
            var drilldown = new List<GraficaColumnSeries>();

            var detallesCaja = _cajaRepository.GetDetallesCaja(año);

            #region ComprasEgresos

            if (detallesCaja != null)
            {
                for (int mes = 1; mes <= 12; mes++)
                {
                    decimal totalValorComprasMes = 0;
                    var comprasMes = detallesCaja
                        .Where(a => a.Fecha.Month == mes && a.Gasto > 0)
                        .ToList();
                    if (comprasMes != null)
                    {
                        totalValorComprasMes += comprasMes.Sum(a => a.Gasto);
                    }

                    seriesDataCompras.Add(new GraficaColumnData
                    {
                        name = GetNombreMes(mes),
                        y = totalValorComprasMes,
                        drilldown = "compras-mes-" + mes
                    });
                }
            }

            series.Add(new GraficaColumnSeries
            {
                name = "Compras",
                data = seriesDataCompras
            });

            #endregion

            #region Datos por dia

            for (int mes = 1; mes <= 12; mes++)
            {
                var diasDelMes = DateTime.DaysInMonth(año, mes);

                var drilldownDataComprasMes = new List<GraficaColumnData>();

                for (int dia = 1; dia <= diasDelMes; dia++)
                {
                    var drilldownDataComprasDiaAmbiente = new List<GraficaColumnData>();

                    var ventasDia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia).Sum(a => a.Ingreso);
                    var ventasDiaHospital = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Hospital).Sum(a => a.Ingreso);
                    var ventasDiaClinica = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Clinica).Sum(a => a.Ingreso);
                    var ventasDiaFarmacia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Farmacia).Sum(a => a.Ingreso);
                    var ventasDiaLaboratorio = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Laboratorio).Sum(a => a.Ingreso);

                    var comprasDia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia).Sum(a => a.Gasto);
                    var comprasDiaHospital = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Hospital).Sum(a => a.Gasto);
                    var comprasDiaClinica = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Clinica).Sum(a => a.Gasto);
                    var comprasDiaFarmacia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Farmacia).Sum(a => a.Gasto);
                    var comprasDiaLaboratorio = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Laboratorio).Sum(a => a.Gasto);

                    var gananciasDia = ventasDia - comprasDia;
                    var gananciasDiaHospital = ventasDiaHospital - comprasDiaHospital;
                    var gananciasDiaClinica = ventasDiaClinica - comprasDiaClinica;
                    var gananciasDiaFarmacia = ventasDiaFarmacia - comprasDiaFarmacia;
                    var gananciasDiaLaboratorio = ventasDiaLaboratorio - comprasDiaLaboratorio;

                    #region Compras

                    drilldownDataComprasMes.Add(new GraficaColumnData
                    {
                        name = "Dia " + dia,
                        y = comprasDia,
                        drilldown = "compras-mes-" + mes + "-dia-" + dia
                    });
                    drilldownDataComprasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Hospital",
                        y = comprasDiaHospital
                    });
                    drilldownDataComprasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Clinica",
                        y = comprasDiaClinica
                    });
                    drilldownDataComprasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Farmacia",
                        y = comprasDiaFarmacia
                    });
                    drilldownDataComprasDiaAmbiente.Add(new GraficaColumnData
                    {
                        name = "Laboratorio",
                        y = comprasDiaLaboratorio
                    });
                    drilldown.Add(new GraficaColumnSeries
                    {
                        id = "compras-mes-" + mes + "-dia-" + dia,
                        name = "Valor Q",
                        data = drilldownDataComprasDiaAmbiente
                    });

                    #endregion

                }

                drilldown.Add(new GraficaColumnSeries
                {
                    id = "compras-mes-" + mes,
                    name = "Valor Q",
                    data = drilldownDataComprasMes
                });
            }

            #endregion


            return (series, drilldown);
        }


        // public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabTendenciasVentas()
        // {
        //     var series = new List<GraficaColumnSeries>();
        //     var seriesDataVentas = new List<GraficaColumnData>();
        //     var drilldown = new List<GraficaColumnSeries>();

        //     var detallesCaja = _cajaRepository.GetDetallesCaja(DateTime.Now.Year);

        //     #region VentasIngresos

        //     if (detallesCaja != null)
        //     {
        //         for (int mes = 1; mes <= 12; mes++)
        //         {
        //             decimal totalValorVentasMes = 0;
        //             var ventasMes = detallesCaja
        //                 .Where(a => a.Fecha.Month == mes && a.Ingreso > 0)
        //                 .ToList();
        //             if (ventasMes != null)
        //             {
        //                 totalValorVentasMes += ventasMes.Sum(a => a.Ingreso);
        //             }

        //             seriesDataVentas.Add(new GraficaColumnData
        //             {
        //                 name = GetNombreMes(mes),
        //                 y = totalValorVentasMes,
        //                 drilldown = "ventas-mes-" + mes
        //             });
        //         }
        //     }

        //     series.Add(new GraficaColumnSeries
        //     {
        //         name = "Ventas",
        //         data = seriesDataVentas
        //     });

        //     #endregion


        //     #region Datos por dia

        //     for (int mes = 1; mes <= 12; mes++)
        //     {
        //         var diasDelMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);

        //         var drilldownDataVentasMes = new List<GraficaColumnData>();

        //         for (int dia = 1; dia <= diasDelMes; dia++)
        //         {

        //             var drilldownDataVentasDiaAmbiente = new List<GraficaColumnData>();
        //             var drilldownDataComprasDiaAmbiente = new List<GraficaColumnData>();
        //             var drilldownDataGananciasDiaAmbiente = new List<GraficaColumnData>();


        //             var ventasDia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia).Sum(a => a.Ingreso);
        //             var ventasDiaHospital = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Hospital).Sum(a => a.Ingreso);
        //             var ventasDiaClinica = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Clinica).Sum(a => a.Ingreso);
        //             var ventasDiaFarmacia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Farmacia).Sum(a => a.Ingreso);
        //             var ventasDiaLaboratorio = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia && a.Caja.AmbienteId == (int)AmbienteEnum.Laboratorio).Sum(a => a.Ingreso);

        //             #region Ventas

        //             drilldownDataVentasMes.Add(new GraficaColumnData
        //             {
        //                 name = "Dia " + dia,
        //                 y = ventasDia,
        //                 drilldown = "ventas-mes-" + mes + "-dia-" + dia
        //             });
        //             drilldownDataVentasDiaAmbiente.Add(new GraficaColumnData
        //             {
        //                 name = "Hospital",
        //                 y = ventasDiaHospital
        //             });
        //             drilldownDataVentasDiaAmbiente.Add(new GraficaColumnData
        //             {
        //                 name = "Clinica",
        //                 y = ventasDiaClinica
        //             });
        //             drilldownDataVentasDiaAmbiente.Add(new GraficaColumnData
        //             {
        //                 name = "Farmacia",
        //                 y = ventasDiaFarmacia
        //             });
        //             drilldownDataVentasDiaAmbiente.Add(new GraficaColumnData
        //             {
        //                 name = "Laboratorio",
        //                 y = ventasDiaLaboratorio
        //             });
        //             drilldown.Add(new GraficaColumnSeries
        //             {
        //                 id = "ventas-mes-" + mes + "-dia-" + dia,
        //                 name = "Valor Q",
        //                 data = drilldownDataVentasDiaAmbiente
        //             });

        //             #endregion


        //         }
        //         drilldown.Add(new GraficaColumnSeries
        //         {
        //             id = "ventas-mes-" + mes,
        //             name = "Valor Q",
        //             data = drilldownDataVentasMes
        //         });

        //     }

        //     #endregion



        //     return (series, drilldown);
        // }

        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabTendenciasVentas(int anio)
        {
            var series = new List<GraficaColumnSeries>();
            var seriesDataVentas = new List<GraficaColumnData>();
            var drilldown = new List<GraficaColumnSeries>();

            var detallesCaja = _cajaRepository.GetDetallesCaja(anio);

            if (detallesCaja == null) return (series, drilldown);

            #region VentasIngresos

            for (int mes = 1; mes <= 12; mes++)
            {
                var ventasMes = detallesCaja
                    .Where(a => a.Fecha.Month == mes && a.Ingreso > 0)
                    .Sum(a => a.Ingreso);

                seriesDataVentas.Add(new GraficaColumnData
                {
                    name = GetNombreMes(mes),
                    y = ventasMes,
                    drilldown = "ventas-mes-" + mes
                });
            }

            series.Add(new GraficaColumnSeries
            {
                name = "Ventas",
                data = seriesDataVentas
            });

            #endregion

            #region Datos por dia

            for (int mes = 1; mes <= 12; mes++)
            {
                var diasDelMes = DateTime.DaysInMonth(anio, mes);
                var drilldownDataVentasMes = new List<GraficaColumnData>();

                for (int dia = 1; dia <= diasDelMes; dia++)
                {
                    var registrosDia = detallesCaja.Where(a => a.Fecha.Month == mes && a.Fecha.Day == dia).ToList();

                    var ventasDia = registrosDia.Sum(a => a.Ingreso);

                    drilldownDataVentasMes.Add(new GraficaColumnData
                    {
                        name = "Dia " + dia,
                        y = ventasDia,
                        drilldown = "ventas-mes-" + mes + "-dia-" + dia
                    });

                    var drilldownDataVentasDiaAmbiente = new List<GraficaColumnData>
            {
                new GraficaColumnData { name = "Hospital", y = registrosDia.Where(a => a.Caja.AmbienteId == (int)AmbienteEnum.Hospital).Sum(a => a.Ingreso) },
                new GraficaColumnData { name = "Clinica", y = registrosDia.Where(a => a.Caja.AmbienteId == (int)AmbienteEnum.Clinica).Sum(a => a.Ingreso) },
                new GraficaColumnData { name = "Farmacia", y = registrosDia.Where(a => a.Caja.AmbienteId == (int)AmbienteEnum.Farmacia).Sum(a => a.Ingreso) },
                new GraficaColumnData { name = "Laboratorio", y = registrosDia.Where(a => a.Caja.AmbienteId == (int)AmbienteEnum.Laboratorio).Sum(a => a.Ingreso) }
            };

                    drilldown.Add(new GraficaColumnSeries
                    {
                        id = "ventas-mes-" + mes + "-dia-" + dia,
                        name = "Valor Q",
                        data = drilldownDataVentasDiaAmbiente
                    });
                }

                drilldown.Add(new GraficaColumnSeries
                {
                    id = "ventas-mes-" + mes,
                    name = "Valor Q",
                    data = drilldownDataVentasMes
                });
            }

            #endregion

            return (series, drilldown);
        }


        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabVentasAnuales(int annio)
        {
            var series = new List<GraficaColumnSeries>();
            var drilldown = new List<GraficaColumnSeries>();
            var detallesCaja = _cajaRepository.GetDetallesCaja(annio);

            if (detallesCaja == null || !detallesCaja.Any()) return (series, drilldown);

            decimal totalAnual = detallesCaja.Where(a => a.Ingreso > 0).Sum(a => a.Ingreso);

            series.Add(new GraficaColumnSeries
            {
                name = "Ventas " + annio,
                data = new List<GraficaColumnData> {
            new GraficaColumnData {
                name = "Total Anual",
                y = totalAnual,
                drilldown = "detalle-dias-" + annio
            }
        }
            });

            GenerarDrilldownPorDia(detallesCaja, drilldown, "detalle-dias-" + annio);

            return (series, drilldown);
        }
        public (List<GraficaColumnSeries> series, List<GraficaColumnSeries> drilldown) TabVentasPorRango(string fechaRango)
        {
            var series = new List<GraficaColumnSeries>();
            var drilldown = new List<GraficaColumnSeries>();

            if (string.IsNullOrEmpty(fechaRango) || !fechaRango.Contains("-")) return (series, drilldown);

            var partes = fechaRango.Split('-');

            if (!DateTime.TryParse(partes[0].Trim(), out DateTime inicio) ||
                !DateTime.TryParse(partes[1].Trim(), out DateTime fin))
            {
                return (series, drilldown);
            }

            var listaCompleta = new List<DetalleCaja>();

            if (inicio.Year == fin.Year)
            {
                var data = _cajaRepository.GetDetallesCaja(inicio.Year);
                if (data != null) listaCompleta.AddRange(data);
            }
            else
            {
                var dataInicio = _cajaRepository.GetDetallesCaja(inicio.Year);
                var dataFin = _cajaRepository.GetDetallesCaja(fin.Year);
                if (dataInicio != null) listaCompleta.AddRange(dataInicio);
                if (dataFin != null) listaCompleta.AddRange(dataFin);
            }

            var datosFiltrados = listaCompleta
                .Where(a => a.Fecha.Date >= inicio.Date && a.Fecha.Date <= fin.Date)
                .OrderBy(a => a.Fecha)
                .ToList();

            decimal totalRango = datosFiltrados.Where(a => a.Ingreso > 0).Sum(a => a.Ingreso);

            series.Add(new GraficaColumnSeries
            {
                name = "Ventas del Periodo",
                data = new List<GraficaColumnData>
        {
            new GraficaColumnData
            {
                name = $"{inicio.ToString("dd/MM/yyyy")} al {fin.ToString("dd/MM/yyyy")}",
                y = totalRango,
                drilldown = "detalle-rango"
            }
        }
            });

            GenerarDrilldownPorDia(datosFiltrados, drilldown, "detalle-rango");

            return (series, drilldown);
        }
        private void GenerarDrilldownPorDia(List<DetalleCaja> datos, List<GraficaColumnSeries> drilldownList, string idPrincipal)
        {
            var ventasPorDia = datos
                .Where(a => a.Ingreso > 0)
                .GroupBy(a => a.Fecha.Date)
                .OrderBy(g => g.Key);

            var listaDias = new List<GraficaColumnData>();

            foreach (var grupo in ventasPorDia)
            {
                string idAmbiente = "ambientes-" + grupo.Key.ToString("yyyyMMdd");

                listaDias.Add(new GraficaColumnData
                {
                    name = grupo.Key.ToShortDateString(),
                    y = grupo.Sum(a => a.Ingreso),
                    drilldown = idAmbiente
                });

                // Detalle por Ambiente
                drilldownList.Add(new GraficaColumnSeries
                {
                    id = idAmbiente,
                    name = "Ambientes " + grupo.Key.ToShortDateString(),
                    data = new List<GraficaColumnData> {
                new GraficaColumnData { name = "Hospital", y = grupo.Where(x => x.Caja.AmbienteId == (int)AmbienteEnum.Hospital).Sum(x => x.Ingreso) },
                new GraficaColumnData { name = "Clinica", y = grupo.Where(x => x.Caja.AmbienteId == (int)AmbienteEnum.Clinica).Sum(x => x.Ingreso) },
                new GraficaColumnData { name = "Farmacia", y = grupo.Where(x => x.Caja.AmbienteId == (int)AmbienteEnum.Farmacia).Sum(x => x.Ingreso) },
                new GraficaColumnData { name = "Laboratorio", y = grupo.Where(x => x.Caja.AmbienteId == (int)AmbienteEnum.Laboratorio).Sum(x => x.Ingreso) }
            }
                });
            }

            drilldownList.Add(new GraficaColumnSeries
            {
                id = idPrincipal,
                name = "Desglose por Día",
                data = listaDias
            });
        }

    }


}
