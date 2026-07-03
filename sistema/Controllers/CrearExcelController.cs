using ClosedXML.Excel;
using Database.Shared.Data;
using Database.Shared.IRepository;
using Database.Shared.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database.Shared.Enumeraciones;
using System.Globalization;
using farmamest.Service.IService;
using Database.Shared;
using Microsoft.EntityFrameworkCore;

namespace farmamest.Controllers
{
    public class CrearExcelController : Controller
    {
        private readonly UserManager<User> _userManager = null;
        private readonly ILaboratorioClinico _laboratorioClinico = null;
        private readonly IVenta _ventaRepository = null;
        private readonly ICompra _compraRepository = null;
        private readonly ICaja _cajaRepository;

        private readonly IMedicamentoNoControladoRepository _medicamentoNoControladoRepository;
        private readonly IPacientes _pacienteRepository;
        private readonly IKitIngresoService _kitIngresoService;

        private readonly Context _context;

        public CrearExcelController(UserManager<User> userManager, ILaboratorioClinico laboratorioClinico,
            IVenta ventaRepository, ICompra compraRepository, ICaja cajaRepository, IMedicamentoNoControladoRepository medicamentoNoControladoRepository, IPacientes pacienteRepository, IKitIngresoService kitIngresoService, Context context)
        {
            _userManager = userManager;
            _laboratorioClinico = laboratorioClinico;
            _ventaRepository = ventaRepository;
            _compraRepository = compraRepository;
            _cajaRepository = cajaRepository;
            _medicamentoNoControladoRepository = medicamentoNoControladoRepository;
            _pacienteRepository = pacienteRepository;
            _kitIngresoService = kitIngresoService;
            _context = context;


        }


        public ActionResult CrearExcelCompraCompras(string fechaInicial, string fechaFinal, string comprobante, string proveedor, string vendedor,
            int numeroCompra)
        {
            var data = _compraRepository.PaginacionCompras(null, null, null, 1000, fechaInicial, fechaFinal, comprobante, proveedor, vendedor, numeroCompra);


            // Aquí puedes generar los datos que deseas exportar al archivo de Excel
            DataTable dt = new DataTable("Hoja");

            // Agregar columnas a la tabla (ejemplo)
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Compra #"), new DataColumn("Fecha compra"),
                    new DataColumn("Proveedor"),new DataColumn("No. Comprobante"),new DataColumn("Vendedor"),
                    new DataColumn("Fecha recepción"),new DataColumn("Fecha limite"),new DataColumn("Gasto") });

            //Totales
            decimal totalTotal = 0;


            foreach (var item in data)
            {

                string empleado = item.Empleado == null ? "-" : item.Empleado.Nombre;

                dt.Rows.Add(item.Id, item.FechaCompra, item.Proveedor.Nombre, item.NoComprobante, empleado,
                    item.FechaRecepcion, item.FechaLimite, item.DetalleCompras.Sum(x => x.PrecioTotal));
                totalTotal += item.DetalleCompras.Sum(a => a.PrecioTotal);
            }

            dt.Rows.Add("", "", "", "", "", "", "", totalTotal);

            return ReturnFileExcel(dt, "Compras");
        }


        public ActionResult CrearExcelComprasOrdenes(string fechaInicial, string fechaFinal, int comprobante,
            string proveedor, string vendedor)
        {
            var data = _compraRepository.PaginacionOrdenesCompra(null, null, null, 25, fechaInicial, fechaFinal, comprobante, proveedor, vendedor);


            // Aquí puedes generar los datos que deseas exportar al archivo de Excel
            DataTable dt = new DataTable("Hoja");

            // Agregar columnas a la tabla (ejemplo)
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("No."), new DataColumn("Fecha emisión"),
                    new DataColumn("Fecha estimada recepción"),new DataColumn("Proveedor"),new DataColumn("Resp."),
                    new DataColumn("Total") });

            //Totales
            decimal totalTotal = 0;


            foreach (var item in data)
            {

                string empleado = item.Empleado == null ? "-" : item.Empleado.Nombre;

                dt.Rows.Add(item.Id, item.FechaCompra, item.FechaRecepcion, item.Proveedor.Nombre,
                    empleado, item.DetalleCompras.Sum(a => a.PrecioTotal));
                totalTotal += item.DetalleCompras.Sum(a => a.PrecioTotal);
            }

            dt.Rows.Add("", "", "", "", "", totalTotal);

            return ReturnFileExcel(dt, "OrdenesCompras");
        }

        public ActionResult CrearExcelVentasClinica(string fechaInicial, string fechaFinal, int numeroVenta,
            string comprobante, int formaPago, string origenVenta)
        {
            var data = _ventaRepository.PaginacionVentasClinica(null, null, null, 1000, fechaInicial, fechaFinal, numeroVenta, comprobante, formaPago, origenVenta);


            // Aquí puedes generar los datos que deseas exportar al archivo de Excel
            DataTable dt = new DataTable("Hoja");

            // Agregar columnas a la tabla (ejemplo)
            dt.Columns.AddRange(new DataColumn[10] { new DataColumn("Fecha"), new DataColumn("# Venta"),
                    new DataColumn("Vendedor"),new DataColumn("NoComprobante"),new DataColumn("Nombre"),
                    new DataColumn("Nit"), new DataColumn("Forma de pago"), new DataColumn("Total"),
                    new DataColumn("Monto pagado"), new DataColumn("Vuelto")});

            decimal pagoDefinitivo = 0;

            //Totales
            decimal totalFormaPago = 0;
            decimal totalTotal = 0;
            decimal totalMontoPago = 0;
            decimal totalVuelto = 0;


            foreach (var item in data)
            {
                if (item.Pagos.Count > 0)
                {
                    foreach (var pago in item.Pagos)
                    {
                        pagoDefinitivo = pago.Monto;
                        totalFormaPago += pago.Monto;

                    }
                }

                totalTotal += item.DetalleVenta.Sum(x => x.Total);
                totalMontoPago += item.MontoPago;
                totalVuelto += item.Vuelto;

                dt.Rows.Add(item.FechaVenta, item.Id, item.Empleado.Nombre, item.NoComprobante,
                    item.Nombres, item.Nit, pagoDefinitivo, item.DetalleVenta.Sum(x => x.Total), item.MontoPago,
                    item.Vuelto);
            }

            dt.Rows.Add("", "", "", "", "", "", totalFormaPago, totalTotal, totalMontoPago, totalVuelto);

            return ReturnFileExcel(dt, "ClinicaVentas");
        }

        public async Task<ActionResult> CrearExcelListaExamenesRealizados(
            string fechaInicial, string fechaFinal, string estado,
            string medicoReferido, string usuarioSolicitaRequest, string usuarioIngresoRequest)
        {
            var data = await GetInformationListaExamenesRealizados(fechaInicial, fechaFinal, estado,
                        medicoReferido, usuarioSolicitaRequest, usuarioIngresoRequest);

            // Crear una tabla DataTable para exportar a Excel
            DataTable dt = new DataTable("Hoja");

            // Agregar columnas a la tabla, incluyendo la columna de FechaSolicitud
            dt.Columns.AddRange(new DataColumn[10]
            {
                new DataColumn("Número de examen"),
                new DataColumn("Paciente"),
                new DataColumn("Fecha de solicitud"),
                new DataColumn("Fecha de realización"),
                new DataColumn("Duración"),
                new DataColumn("Médico referido"),
                new DataColumn("Clínica referida"),
                new DataColumn("Estado"),
                new DataColumn("Usuario solicita"),
                new DataColumn("Usuario ingreso"),
            });

            // Rellenar la tabla con los datos
            foreach (var item in data)
            {
                dt.Rows.Add(item.ExamenNumero, item.Paciente, item.FechaSolicitud, item.FechaRealizacion, item.Duracion,
                    item.MedicoReferido, item.ClinicaReferida, item.Estado, item.UsuarioSolicita, item.UsuarioIngreso);
            }

            return ReturnFileExcel(dt, "ListaExamenesRealizados");
        }


        private ActionResult ReturnFileExcel(DataTable dt, string nameFile)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                // Añadir la tabla al libro de trabajo
                var ws = wb.Worksheets.Add(dt);

                // Establecer estilos para el encabezado
                var headerRow = ws.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray; // Fondo para encabezado
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRow.Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                // Aplicar bordes y auto-ajustar todas las columnas
                ws.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                ws.Columns().AdjustToContents();

                // Formatear columnas específicas
                ws.Column(3).Style.DateFormat.Format = "dd/MM/yyyy";
                ws.Column(4).Style.DateFormat.Format = "dd/MM/yyyy";
                ws.Column(5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Establecer filtros automáticos
                // ws.RangeUsed().SetAutoFilter();

                // Guardar el libro de trabajo en un MemoryStream
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    // Establecer el tipo de contenido y el nombre del archivo
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nameFile + ".xlsx");
                }
            }
        }

        [HttpPost]
        public IActionResult CotizacionExcel([FromBody] CotizacionViewModel model)
        {
            if (model == null || model.Productos == null || model.Proveedores == null)
            {
                return BadRequest("Datos de cotización no válidos.");
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Cotización");

                // Encabezado
                ws.Cell(1, 1).Value = "Cotización de Productos";
                ws.Range(1, 1, 1, model.Proveedores.Count + 3).Merge().Style
                    .Font.SetBold(true)
                    .Font.SetFontSize(14)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // Cabecera de la tabla
                var headerRow = ws.Row(3);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRow.Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell(3, 1).Value = "Código";
                ws.Cell(3, 2).Value = "Producto";
                ws.Cell(3, 3).Value = "Cantidad";

                int col = 4;
                foreach (var proveedor in model.Proveedores)
                {
                    var cell = ws.Cell(3, col);
                    cell.Value = proveedor;

                    // Resalta el proveedor principal en amarillo
                    if (proveedor == model.ProveedorPrincipal)
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                    }

                    col++;
                }

                // Agregar los productos
                int row = 4;
                foreach (var producto in model.Productos)
                {
                    ws.Cell(row, 1).Value = producto.CodigoReferencia;
                    ws.Cell(row, 2).Value = producto.ProductoNombre;
                    ws.Cell(row, 3).Value = producto.CantidadTrasladada;

                    col = 4;
                    foreach (var proveedor in model.Proveedores)
                    {
                        ws.Cell(row, col).Value = producto.CantidadesProveedores.ContainsKey(proveedor)
                            ? producto.CantidadesProveedores[proveedor]
                            : 0;
                        col++;
                    }

                    row++;
                }

                // Aplicar formato a las celdas
                ws.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Cotizacion.xlsx");
                }
            }
        }


        private async Task<List<ExamenesRealizadosViewModel>> GetInformationListaExamenesRealizados(
            string fechaInicial, string fechaFinal, string estado,
            string medicoReferido, string usuarioSolicitaRequest, string usuarioIngresoRequest)
        {
            var laboratorio = _laboratorioClinico.GetAllExamenesRealizados();
            List<ExamenesRealizadosViewModel> modeloLista = new List<ExamenesRealizadosViewModel>();

            foreach (var item in laboratorio)
            {
                var paciente = item.Paciente == null ? "" : item.Paciente.Nombre;
                var nombreEstado = item.EstadoExamen == null ? "// sin asignar" : item.EstadoExamen.Nombre;
                var medico = item.Medicos == null ? "// sin referencia" : item.Medicos.Nombres;

                var usuarioSolicita = await _userManager.FindByIdAsync(item.UsuarioSolicita);
                var usuarioIngreso = await _userManager.FindByIdAsync(item.UsuarioIngresa);

                var usuarioSolicitaText = usuarioSolicita == null ? "// sin asignar" : usuarioSolicita.Email;
                var usuarioIngresoText = usuarioIngreso == null ? "// sin asignar" : usuarioIngreso.Email;

                // Verificación y cálculo de la duración
                string duracion = string.Empty;
                if (!string.IsNullOrEmpty(fechaInicial) && DateTime.TryParse(fechaInicial, out DateTime fechaInicio))
                {
                    if (item.FechaRealizacion != default(DateTime))
                    {
                        // Calcular la duración solo si FechaRealizacion es válida
                        TimeSpan diferencia = item.FechaRealizacion - fechaInicio;
                        duracion = $"{diferencia.Days} días, {diferencia.Hours} horas, {diferencia.Minutes} minutos";
                    }
                    else
                    {
                        duracion = "// fecha de realización inválida";
                    }
                }
                else
                {
                    duracion = "// fecha inicial inválida";
                }


                var objeto = new ExamenesRealizadosViewModel()
                {
                    ExamenNumero = item.Id,
                    Paciente = paciente,
                    FechaRealizacion = item.FechaRealizacion,
                    MedicoReferido = medico,
                    ClinicaReferida = item.ClinicaReferida,
                    Estado = nombreEstado,
                    UsuarioSolicita = usuarioSolicitaText,
                    UsuarioIngreso = usuarioIngresoText,
                    Duracion = duracion
                };

                modeloLista.Add(objeto);
            }

            // Filtrar por fechas si están presentes
            if (!string.IsNullOrEmpty(fechaInicial) && !string.IsNullOrEmpty(fechaFinal))
            {
                DateTime fechaInicio = DateTime.Parse(fechaInicial);
                DateTime fechaFin = DateTime.Parse(fechaFinal);

                modeloLista = modeloLista
                    .Where(x => x.FechaRealizacion.Date >= fechaInicio && x.FechaRealizacion.Date <= fechaFin)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(estado))
            {
                modeloLista = modeloLista.Where(x =>
                    x.Estado != null &&
                    x.Estado.ToLower().Trim() == estado.ToLower().Trim()).ToList();
            }

            if (!string.IsNullOrEmpty(usuarioSolicitaRequest))
            {
                modeloLista = modeloLista.Where(x =>
                    x.UsuarioSolicita != null &&
                    x.UsuarioSolicita.ToLower().Trim() == usuarioSolicitaRequest.ToLower().Trim()).ToList();
            }

            if (!string.IsNullOrEmpty(usuarioIngresoRequest))
            {
                modeloLista = modeloLista.Where(x =>
                    x.UsuarioIngreso != null &&
                    x.UsuarioIngreso.ToLower().Trim() == usuarioIngresoRequest.ToLower().Trim()).ToList();
            }

            return modeloLista;
        }


        public IActionResult ReporteExcelVentasFarmacia(string fecha, int? empleadoid = null)
        {

            var fechas = fecha.Split('-');
            var desde = DateTime.ParseExact(fechas[0].Trim(), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            var hasta = DateTime.ParseExact(fechas[1].Trim(), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);

            var ventas = empleadoid == null
                ? _ventaRepository.GetListadoFechaFarmacia(desde, hasta)
                : _ventaRepository.GetListadoFechaEmpleadoFarmacia(desde, hasta, empleadoid);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Ventas Farmacia");

                // Encabezado de información general
                worksheet.Cell(1, 1).Value = "Rango de Fecha:";
                worksheet.Cell(1, 2).Value = $"{desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy}";

                // worksheet.Cell(2, 1).Value = "Solicitado por:";
                // worksheet.Cell(2, 2).Value = nombreSolicitante;

                worksheet.Cell(3, 1).Value = "Fecha y Hora de Reporte:";
                worksheet.Cell(3, 2).Value = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");

                worksheet.Range("A1:B3").Style.Font.Bold = true;
                worksheet.Range("A1:B3").Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Range("A1:B3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Totales y datos del reporte
                decimal totalCostos = 0;
                decimal totalMontoPagado = ventas.Sum(v => v.MontoPago);
                decimal totalVuelto = ventas.Sum(v => v.Vuelto);

                worksheet.Cell(5, 1).Value = "Total de Ventas:";
                worksheet.Cell(5, 2).Value = (totalMontoPagado - totalVuelto);
                worksheet.Cell(6, 1).Value = "Vuelto Total:";
                worksheet.Cell(6, 2).Value = totalVuelto;
                worksheet.Cell(7, 1).Value = "Total de Costos:";
                worksheet.Cell(7, 2).Value = totalCostos;
                worksheet.Cell(8, 1).Value = "Ganancia Total:";
                worksheet.Cell(8, 2).Value = (totalMontoPagado - totalCostos);

                worksheet.Range("A5:B8").Style.Font.Bold = true;
                worksheet.Range("A5:B8").Style.Fill.BackgroundColor = XLColor.LightBlue;
                worksheet.Range("A5:B8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A5:B8").Style.NumberFormat.Format = "#,##0.00";

                // Calculate total sales for Normal and Descuento prices
                decimal totalVentasPrecioNormal = ventas.Sum(v => v.DetalleVenta
                    .Where(d => d.Producto?.ProductosInventario.FirstOrDefault()?.ProductosInventarioPrecios
                        .FirstOrDefault(p => p.Precio?.NombrePrecio == "Normal") != null)
                    .Sum(d => d.Total));

                decimal totalVentasPrecioDescuento = ventas.Sum(v => v.DetalleVenta
                    .Where(d => d.Producto?.ProductosInventario.FirstOrDefault()?.ProductosInventarioPrecios
                        .FirstOrDefault(p => p.Precio?.NombrePrecio == "Descuento") != null)
                    .Sum(d => d.Total));

                // Add totals for Normal and Descuento prices below Ganancia Total with spacing
                int nextRow = 10;
                worksheet.Cell(nextRow, 1).Value = "Total de Ventas con Precio Normal:";
                worksheet.Cell(nextRow, 2).Value = totalVentasPrecioNormal;
                worksheet.Cell(nextRow + 1, 1).Value = "Total de Ventas con Precio Descuento:";
                worksheet.Cell(nextRow + 1, 2).Value = totalVentasPrecioDescuento;

                worksheet.Range($"A{nextRow}:B{nextRow + 1}").Style.Font.Bold = true;
                worksheet.Range($"A{nextRow}:B{nextRow + 1}").Style.Fill.BackgroundColor = XLColor.LightBlue;
                worksheet.Range($"A{nextRow}:B{nextRow + 1}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range($"A{nextRow}:B{nextRow + 1}").Style.NumberFormat.Format = "#,##0.00";

                // Encabezado de columnas para ventas
                int headerRow = 12;
                worksheet.Row(headerRow).Style.Font.Bold = true;
                worksheet.Row(headerRow).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Row(headerRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                string[] headers = {
            "Codigo", "Producto", "Categoria", "Sucursal", "Nombre de Sucursal", "Periodo", "Fecha",
            "Factura", "Tipo Pago", "Tipo Entrega", "Codigo de Cliente", "Cliente", "Cedula",
            "Empleado", "Cantidad", "Precio Costo", "Precio Sugerido", "Precio Venta", "Descuento", "Monto recibido",
            "Vuelto", "Pago", "Pago sin ISV", "Costo", "Costo sin ISV", "Margen Absoluto Unitario", "Margen Absoluto Total"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(headerRow, i + 1).Value = headers[i];
                }

                int row = headerRow + 1;

                foreach (var venta in ventas)
                {
                    foreach (var detalle in venta.DetalleVenta)
                    {
                        decimal precioCosto = detalle.Producto?.ProductosInventario.FirstOrDefault()?.PrecioCosto ?? 0;
                        decimal precioVentaNormal = detalle.Producto?.ProductosInventario.FirstOrDefault()?.ProductosInventarioPrecios
                            .FirstOrDefault(p => p.Precio?.NombrePrecio == "Normal")?.Valor ?? 0;
                        decimal precioVentaDescuento = detalle.Producto?.ProductosInventario.FirstOrDefault()?.ProductosInventarioPrecios
                            .FirstOrDefault(p => p.Precio?.NombrePrecio == "Descuento")?.Valor ?? 0;
                        decimal precioVentaUtilizado = detalle.Producto?.ProductosInventario.FirstOrDefault()?.ProductosInventarioPrecios.FirstOrDefault()?.Valor ?? 0;
                        string nombrePrecioUtilizado = detalle.Producto?.ProductosInventario.FirstOrDefault()?.ProductosInventarioPrecios.FirstOrDefault()?.Precio?.NombrePrecio ?? "Sin Nombre";

                        int cantidad = detalle.Cantidad;
                        decimal costoTotal = precioCosto * cantidad;
                        decimal margenUnitario = precioVentaUtilizado - precioCosto;
                        decimal margenTotal = margenUnitario * cantidad;

                        totalCostos += costoTotal;

                        // Datos de cada fila
                        worksheet.Cell(row, 1).Value = detalle.ProductoId?.ToString() ?? "Sin Producto";
                        worksheet.Cell(row, 2).Value = detalle.Producto?.NombreProducto ?? "Sin Nombre de Producto";
                        worksheet.Cell(row, 3).Value = detalle.Producto?.Categoria?.NombreCategoria ?? "Sin Categoría";
                        worksheet.Cell(row, 4).Value = "CSPM";
                        worksheet.Cell(row, 5).Value = "El Manchen";
                        worksheet.Cell(row, 6).Value = venta.FechaVenta.ToString("yyyyMM");
                        worksheet.Cell(row, 7).Value = venta.FechaVenta.ToString("dd/MM/yyyy");
                        worksheet.Cell(row, 8).Value = $"010-001-01-{venta.Id.ToString().PadLeft(8, '0') ?? "Sin Factura"}";
                        worksheet.Cell(row, 9).Value = venta.Pagos.Any() ? venta.Pagos.First().FormaPago?.NombreFormaPago : "Sin Tipo de Pago";
                        worksheet.Cell(row, 10).Value = "Presencial";
                        worksheet.Cell(row, 11).Value = venta.PacienteId?.ToString() ?? "Sin Código";
                        worksheet.Cell(row, 12).Value = venta.Paciente?.Nombre ?? "Sin Cliente";
                        worksheet.Cell(row, 13).Value = venta.Paciente?.Dpi ?? "Sin Cedula";
                        worksheet.Cell(row, 14).Value = venta.Empleado?.NombreYApellidos ?? "Sin Empleado";
                        worksheet.Cell(row, 15).Value = cantidad;
                        worksheet.Cell(row, 16).Value = precioCosto;
                        worksheet.Cell(row, 17).Value = precioVentaNormal;
                        worksheet.Cell(row, 18).Value = precioVentaDescuento;
                        // worksheet.Cell(row, 19).Value = $"{precioVentaUtilizado} ({nombrePrecioUtilizado})";
                        worksheet.Cell(row, 19).Value = precioVentaNormal < precioVentaDescuento ? 0 : precioVentaNormal - precioVentaDescuento;
                        worksheet.Cell(row, 20).Value = venta.MontoPago;
                        worksheet.Cell(row, 21).Value = venta.Vuelto;
                        worksheet.Cell(row, 22).Value = detalle.Total;
                        worksheet.Cell(row, 23).Value = "N/A"; // Pago sin ISV
                        worksheet.Cell(row, 24).Value = costoTotal;
                        worksheet.Cell(row, 25).Value = "N/A"; // Costo sin ISV
                        worksheet.Cell(row, 26).Value = margenUnitario;
                        worksheet.Cell(row, 27).Value = margenTotal;

                        worksheet.Range(row, 16, row, 27).Style.NumberFormat.Format = "#,##0.00";
                        worksheet.Range(row, 1, row, headers.Length).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        row++;
                    }
                }

                worksheet.Cell(7, 2).Value = totalCostos;
                worksheet.Cell(8, 1).Value = "Ganancia Total:";
                worksheet.Cell(8, 2).Value = (totalMontoPagado - totalCostos);
                worksheet.Cell(8, 2).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Ventas_Farmacia.xlsx");
                }
            }
        }


        public IActionResult ReporteExcelVentasClinica(
          DateTime? fechaInicial,
          DateTime? fechaFinal,
          int? numeroVenta = null,
          string comprobante = null,
          int? formaPago = null,
          int? empleadoid = null)
        {
            var desde = fechaInicial ?? DateTime.MinValue;
            var hasta = fechaFinal.HasValue ? fechaFinal.Value : DateTime.MaxValue;

            var ventasQuery = empleadoid == null
                ? _ventaRepository.GetListadoFechaClinica(desde, hasta)
                : _ventaRepository.GetListadoFechaEmpleadoClinica(desde, hasta, empleadoid);

            if (numeroVenta.HasValue)
                ventasQuery = ventasQuery.Where(v => v.Id == numeroVenta.Value).ToList();

            if (!string.IsNullOrEmpty(comprobante))
                ventasQuery = ventasQuery.Where(v => v.NoComprobante == comprobante).ToList();

            if (formaPago.HasValue)
                ventasQuery = ventasQuery.Where(v => v.Pagos.Any(p => p.FormaPagoId == formaPago.Value)).ToList();

            var ventas = ventasQuery.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Ventas Clínica");

                // decimal totalCostos = 0;
                decimal totalMontoPagado = ventas.Sum(v => v.MontoPago);
                decimal totalVuelto = ventas.Sum(v => v.Vuelto);

                worksheet.Cell(1, 1).Value = "Monto Total Recibido:";
                worksheet.Cell(1, 2).Value = totalMontoPagado;
                worksheet.Cell(2, 1).Value = "Vuelto Total:";
                worksheet.Cell(2, 2).Value = totalVuelto;
                // worksheet.Cell(3, 1).Value = "Total de Costos:";
                // worksheet.Cell(3, 2).Value = totalCostos;

                worksheet.Range("A1:B4").Style.Font.Bold = true;
                worksheet.Range("A1:B4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                worksheet.Cell(1, 2).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(2, 2).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(3, 2).Style.NumberFormat.Format = "#,##0.00";

                int headerRow = 6;
                worksheet.Row(headerRow).Style.Font.Bold = true;
                worksheet.Row(headerRow).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Row(headerRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                string[] headers = {
            "Codigo", "Servicio", "Categoria", "Sucursal", "Nombre de Sucursal", "Periodo", "Fecha",
            "Factura", "Tipo Pago", "Tipo Entrega", "Codigo de Cliente", "Cliente", "Cedula",
            "Empleado", "Cantidad", "Precio Venta", "Descuentos", "Monto recibido",
            "Vuelto", "Pago", "Pago sin ISV"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(headerRow, i + 1).Value = headers[i];
                }

                int row = headerRow + 1;
                foreach (var venta in ventas)
                {
                    foreach (var detalle in venta.DetalleVenta)
                    {
                        // decimal precioCosto = 0;
                        decimal precioVenta = detalle.Servicio?.ServiciosPrecios
                            .Select(sp => sp.Valor)
                            .FirstOrDefault() ?? 0;

                        int cantidad = detalle.Cantidad;
                        // decimal costoTotal = precioCosto * cantidad;
                        // decimal margenUnitario = precioVenta - precioCosto;
                        // decimal margenTotal = margenUnitario * cantidad;

                        // totalCostos += costoTotal;

                        worksheet.Cell(row, 1).Value = detalle.ServicioId?.ToString() ?? "Sin Servicio";
                        worksheet.Cell(row, 2).Value = detalle.Servicio?.NombreServicio ?? "Sin Nombre de Servicio";
                        worksheet.Cell(row, 3).Value = detalle.Servicio?.CategoriaServicio?.NombreCategoria ?? "Sin Categoría";
                        worksheet.Cell(row, 4).Value = "CSPM";
                        worksheet.Cell(row, 5).Value = "Tgu El Centro";
                        worksheet.Cell(row, 6).Value = venta.FechaVenta.ToString("yyyyMM");
                        worksheet.Cell(row, 7).Value = venta.FechaVenta.ToString("dd/MM/yyyy");
                        worksheet.Cell(row, 8).Value = $"009-001-01-0000{venta.Id.ToString().PadLeft(4, '0') ?? "Sin Factura"}";
                        worksheet.Cell(row, 9).Value = venta.Pagos.Any() ? venta.Pagos.First().FormaPago?.NombreFormaPago : "Sin Tipo de Pago";
                        worksheet.Cell(row, 10).Value = "Presencial";
                        worksheet.Cell(row, 11).Value = venta.PacienteId?.ToString() ?? "Sin Código";
                        worksheet.Cell(row, 12).Value = venta.Paciente?.Nombre ?? "Sin Cliente";
                        worksheet.Cell(row, 13).Value = venta.Paciente?.Dpi ?? "Sin Cedula";
                        worksheet.Cell(row, 14).Value = venta.Empleado?.NombreYApellidos ?? "Sin Empleado";
                        worksheet.Cell(row, 15).Value = cantidad;
                        // worksheet.Cell(row, 16).Value = precioCosto;
                        worksheet.Cell(row, 16).Value = precioVenta;
                        worksheet.Cell(row, 17).Value = "N/A";
                        worksheet.Cell(row, 18).Value = venta.MontoPago;
                        worksheet.Cell(row, 19).Value = venta.Vuelto;
                        worksheet.Cell(row, 20).Value = detalle.Total;
                        worksheet.Cell(row, 21).Value = "N/A";
                        // worksheet.Cell(row, 23).Value = costoTotal;
                        // worksheet.Cell(row, 24).Value = "N/A";
                        // worksheet.Cell(row, 25).Value = margenUnitario;
                        // worksheet.Cell(row, 26).Value = margenTotal;

                        worksheet.Range(row, 1, row, headers.Length).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        row++;
                    }
                }

                // worksheet.Cell(3, 2).Value = totalCostos;
                // worksheet.Cell(4, 1).Value = "Ganancia Total:";
                // worksheet.Cell(4, 2).Value = (totalMontoPagado - totalCostos);
                // worksheet.Cell(4, 2).Style.NumberFormat.Format = "#,##0.00";

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Ventas_Clinica.xlsx");
                }
            }
        }

        public IActionResult ReporteExcelVentasLaboratorio(
            DateTime? fechaInicial,
            DateTime? fechaFinal,
            int? numeroVenta = null,
            string comprobante = null,
            int? formaPago = null,
            int? empleadoid = null)
        {
            var desde = fechaInicial ?? DateTime.MinValue;
            var hasta = fechaFinal.HasValue ? fechaFinal.Value : DateTime.MaxValue;

            var ventasQuery = empleadoid == null
                ? _ventaRepository.GetListadoFechaLaboratorio(desde, hasta)
                : _ventaRepository.GetListadoFechaEmpleadoLaboratorio(desde, hasta, empleadoid);

            if (numeroVenta.HasValue)
                ventasQuery = ventasQuery.Where(v => v.Id == numeroVenta.Value).ToList();

            if (!string.IsNullOrEmpty(comprobante))
                ventasQuery = ventasQuery.Where(v => v.NoComprobante == comprobante).ToList();

            if (formaPago.HasValue)
                ventasQuery = ventasQuery.Where(v => v.Pagos.Any(p => p.FormaPagoId == formaPago.Value)).ToList();

            var ventas = ventasQuery.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Ventas Laboratorio");

                decimal totalMontoPagado = ventas.Sum(v => v.MontoPago);
                decimal totalVuelto = ventas.Sum(v => v.Vuelto);

                worksheet.Cell(1, 1).Value = "Monto Total Recibido:";
                worksheet.Cell(1, 2).Value = totalMontoPagado;
                worksheet.Cell(2, 1).Value = "Vuelto Total:";
                worksheet.Cell(2, 2).Value = totalVuelto;

                worksheet.Range("A1:B2").Style.Font.Bold = true;
                worksheet.Range("A1:B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                worksheet.Cell(1, 2).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(2, 2).Style.NumberFormat.Format = "#,##0.00";

                int headerRow = 6;
                worksheet.Row(headerRow).Style.Font.Bold = true;
                worksheet.Row(headerRow).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Row(headerRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                string[] headers = {
            "Codigo", "Examen", "Categoria", "Sucursal", "Nombre de Sucursal", "Periodo", "Fecha",
            "Factura", "Tipo Pago", "Tipo Entrega", "Codigo de Cliente", "Cliente", "Cedula",
            "Empleado", "Cantidad", "Precio Venta", "Descuentos", "Monto recibido",
            "Vuelto", "Pago", "Pago sin ISV"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(headerRow, i + 1).Value = headers[i];
                }

                int row = headerRow + 1;
                foreach (var venta in ventas)
                {
                    foreach (var detalle in venta.DetalleVenta)
                    {
                        decimal precioVenta = detalle.ExamenLabClinico?.ExamenLabClinicosPrecios
                            .Select(ep => ep.PrecioValor)
                            .FirstOrDefault() ?? 0;

                        int cantidad = detalle.Cantidad;

                        worksheet.Cell(row, 1).Value = detalle.ExamenLabClinicoId?.ToString() ?? "Sin Examen";
                        worksheet.Cell(row, 2).Value = detalle.ExamenLabClinico?.NombreExamen ?? "Sin Nombre de Examen";
                        worksheet.Cell(row, 3).Value = detalle.ExamenLabClinico?.CategoriaLabClinico?.Nombre ?? "Sin Categoría";
                        worksheet.Cell(row, 4).Value = "CSPM";
                        worksheet.Cell(row, 5).Value = "Tgu El Centro";
                        worksheet.Cell(row, 6).Value = venta.FechaVenta.ToString("yyyyMM");
                        worksheet.Cell(row, 7).Value = venta.FechaVenta.ToString("dd/MM/yyyy");
                        worksheet.Cell(row, 8).Value = $"009-001-01-0000{venta.Id.ToString().PadLeft(4, '0') ?? "Sin Factura"}";
                        worksheet.Cell(row, 9).Value = venta.Pagos.Any() ? venta.Pagos.First().FormaPago?.NombreFormaPago : "Sin Tipo de Pago";
                        worksheet.Cell(row, 10).Value = "Presencial";
                        worksheet.Cell(row, 11).Value = venta.PacienteId?.ToString() ?? "Sin Código";
                        worksheet.Cell(row, 12).Value = venta.Paciente?.Nombre ?? "Sin Cliente";
                        worksheet.Cell(row, 13).Value = venta.Paciente?.Dpi ?? "Sin Cedula";
                        worksheet.Cell(row, 14).Value = venta.Empleado?.NombreYApellidos ?? "Sin Empleado";
                        worksheet.Cell(row, 15).Value = cantidad;
                        worksheet.Cell(row, 16).Value = precioVenta;
                        worksheet.Cell(row, 17).Value = "N/A";
                        worksheet.Cell(row, 18).Value = venta.MontoPago;
                        worksheet.Cell(row, 19).Value = venta.Vuelto;
                        worksheet.Cell(row, 20).Value = detalle.Total;
                        worksheet.Cell(row, 21).Value = "N/A";

                        worksheet.Range(row, 1, row, headers.Length).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        row++;
                    }
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Ventas_Laboratorio.xlsx");
                }
            }
        }


        public ActionResult ReporteCajaExcel(string fecha, int ambienteIdReporte)
        {
            var fechas = fecha.Split('-');
            var cajas = _cajaRepository.GetListadoFecha(
                Convert.ToDateTime(fechas[0].Trim()),
                Convert.ToDateTime(fechas[1].Trim()).AddDays(1),
                ambienteIdReporte
            );

            // Determinar el nombre del ambiente basado en el ID
            var nombreAmbiente = ambienteIdReporte switch
            {
                (int)AmbienteEnum.Farmacia => "Farmacia",
                (int)AmbienteEnum.Clinica => "Clinica",
                (int)AmbienteEnum.Bodega => "Bodega",
                (int)AmbienteEnum.Laboratorio => "Laboratorio",
                (int)AmbienteEnum.Hospital => "Hospital",
                _ => "Global"
            };

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte Caja");

                // Encabezado
                worksheet.Cell("A1").Value = $"Reporte de Caja - {nombreAmbiente}";
                worksheet.Cell("A1").Style.Font.Bold = true;
                worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:J1").Merge();

                worksheet.Cell("A2").Value = $"Fecha de Emisión: {DateTime.Now:dddd, dd MMMM yyyy}".ToUpper();
                worksheet.Cell("A3").Value = $"Rango de Fechas: {fechas[0].Trim()} - {fechas[1].Trim()}";

                // Encabezado de tabla
                var headers = new[]
                {
            "Sucursal / Ambiente", "Fecha de Apertura", "Resp. Apertura", "Monto de Apertura",
            "Ingresos", "Gastos", "Fecha de Cierre", "Resp. Cierre", "Total Cierre"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(5, i + 1).Value = headers[i];
                    worksheet.Cell(5, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1f4724");
                    worksheet.Cell(5, i + 1).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(5, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                int currentRow = 6;

                // Filas de datos
                foreach (var caja in cajas)
                {
                    var sucursalNombre = caja.Sucursal != null ? caja.Sucursal.NombreSucursal : "TODAS";
                    var ambienteNombre = caja.Ambiente != null ? caja.Ambiente.NombreAmbiente : nombreAmbiente;
                    var responsableApertura = caja.ResponsableApertura != null ? caja.ResponsableAperturaText : "Sin Responsable";
                    var responsableCierre = caja.ResponsableCierre != null ? caja.ResponsableCierreText : "Sin Responsable";

                    decimal ingresos = caja.DetalleCajas.Sum(x => x.Ingreso);
                    decimal gastos = caja.DetalleCajas.Sum(x => x.Gasto);
                    decimal totalCierre = ingresos - gastos + caja.MontoApertura;

                    worksheet.Cell(currentRow, 1).Value = $"{sucursalNombre} / {ambienteNombre}";
                    worksheet.Cell(currentRow, 2).Value = caja.FechaApertura.ToString("dd/MM/yyyy HH:mm") ?? "Sin Apertura";
                    worksheet.Cell(currentRow, 3).Value = responsableApertura;
                    worksheet.Cell(currentRow, 4).Value = caja.MontoApertura;
                    worksheet.Cell(currentRow, 5).Value = ingresos;
                    worksheet.Cell(currentRow, 6).Value = gastos;
                    worksheet.Cell(currentRow, 7).Value = caja.FechaCierre?.ToString("dd/MM/yyyy HH:mm") ?? "Sin Cierre";
                    worksheet.Cell(currentRow, 8).Value = responsableCierre;
                    worksheet.Cell(currentRow, 9).Value = totalCierre;

                    currentRow++;
                }

                // Ajustar el ancho de las columnas
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"ReporteCaja_{nombreAmbiente}_{fechas[0].Trim()}_al_{fechas[1].Trim()}.xlsx"
                    );
                }
            }
        }


        public IActionResult MedicamentosControladosHospiExcel(int hospitalizacionId)
        {
            // Obtener los medicamentos controlados desde la base de datos
            var medicamentos = _context.MedicamentosNoControlado
                .Where(m => m.HospitalizacionId == hospitalizacionId)
                .OrderBy(m => m.ProductoNombre)
                .ToList();

            // Obtener el paciente a través de la hospitalización
            var hospitalizacion = _context.Hospitalizaciones
                .Include(h => h.Paciente)
                .FirstOrDefault(h => h.Id == hospitalizacionId);
            string nombrePaciente = hospitalizacion?.Paciente?.Nombre ?? $"Hospi_{hospitalizacionId}";

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Medicamentos Controlados");

                // --- ENCABEZADO ---
                ws.Cell(1, 1).Value = "ANEXO 14";
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 5).Value = "SERVICIOS GLOBALES PARA LA SALUD. S.A.";
                ws.Cell(1, 5).Style.Font.Bold = true;
                ws.Cell(1, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                ws.Cell(3, 1).Value = "Medicamentos Controlados Transoperatorio";
                ws.Range(3, 1, 3, 7).Merge();
                ws.Cell(3, 1).Style.Font.Bold = true;
                ws.Cell(3, 1).Style.Font.FontSize = 14;
                ws.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(5, 1).Value = "Fecha:";
                ws.Cell(5, 1).Style.Font.Bold = true;
                ws.Cell(5, 2).Value = DateTime.Now.ToString("dd/MM/yyyy");
                ws.Cell(5, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell(6, 1).Value = "Paciente:";
                ws.Cell(6, 1).Style.Font.Bold = true;
                ws.Cell(6, 2).Value = nombrePaciente;
                ws.Range(6, 2, 6, 4).Merge();
                ws.Cell(6, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                // --- CABECERA DE TABLA ---
                int hr = 8;
                ws.Cell(hr, 1).Value = "Medicamento";
                ws.Range(hr, 1, hr + 1, 1).Merge();

                ws.Cell(hr, 2).Value = "Unidades Entregadas (ampollas)";
                ws.Range(hr, 2, hr, 3).Merge();

                ws.Cell(hr, 4).Value = "Medicamento Utilizado (mg/mcg)";
                ws.Range(hr, 4, hr + 1, 4).Merge();

                ws.Cell(hr, 5).Value = "Medicamento Descartado (mg/mcg)";
                ws.Range(hr, 5, hr + 1, 5).Merge();

                ws.Cell(hr, 6).Value = "Unidades Retornadas (Ampollas)";
                ws.Range(hr, 6, hr + 1, 6).Merge();

                ws.Cell(hr + 1, 2).Value = "Inicial";
                ws.Cell(hr + 1, 3).Value = "Extra";

                var headerRange = ws.Range(hr, 1, hr + 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#f2f2f2");
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // --- FILAS DE DATOS ---
                int row = hr + 2;

                if (medicamentos.Any())
                {
                    foreach (var m in medicamentos)
                    {
                        // Calcular descartado y retornadas (si no existen en BD)
                        decimal descartado = m.UnidadesIniciales - m.Utilizado;
                        decimal retornadas = descartado > 0 ? descartado : 0;

                        ws.Cell(row, 1).Value = m.ProductoNombre;
                        ws.Cell(row, 1).Style.Font.Bold = true;
                        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        ws.Cell(row, 2).Value = m.UnidadesIniciales;   // Inicial
                        ws.Cell(row, 3).Value = 0;                     // Extra (no disponible en esta tabla)
                        ws.Cell(row, 4).Value = m.Utilizado;           // Utilizado
                        ws.Cell(row, 5).Value = descartado;            // Descartado
                        ws.Cell(row, 6).Value = retornadas;            // Retornadas

                        var dataRange = ws.Range(row, 1, row, 6);
                        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        row++;
                    }
                }
                else
                {
                    ws.Cell(row, 1).Value = "No hay medicamentos controlados registrados para esta hospitalización.";
                    ws.Range(row, 1, row, 6).Merge();
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 1).Style.Font.FontColor = XLColor.Gray;
                    ws.Cell(row, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    row++;
                }

                // --- FIRMAS ---
                int firmaRow = row + 3;
                ws.Cell(firmaRow, 1).Value = "Firma y sello Responsable de farmacia y bodega";
                ws.Cell(firmaRow, 4).Value = "Firma y Sello de Anestesiólogo";

                // --- AJUSTE FINAL ---
                ws.Columns().AdjustToContents();
                if (ws.Column(1).Width < 30) ws.Column(1).Width = 30;

                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"MedicamentosControlados_{nombrePaciente}_{DateTime.Now:yyyyMMdd}.xlsx"
                    );
                }
            }
        }

    }
}
