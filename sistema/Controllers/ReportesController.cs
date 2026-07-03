using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database.Shared.IRepository;
using sistema.Models;
using Database.Shared.Paginacion;
using System.Linq;
using Database.Shared.Models;
using Rotativa.AspNetCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Database.Shared.Data;
using ClosedXML.Excel;
using System.IO;
using System.Globalization;
using Database.Shared;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;  // para SelectListItem

namespace sistema.Controllers
{

    public class ReportesController : Controller
    {

        private readonly IVenta _ventaRepository = null;
        private readonly ICompra _compraRepository = null;
        //private readonly IVentaServicio _ventaServicioRepository = null;
        private readonly IEmpleado _empleadoRepository = null;
        private readonly ICliente _clienteRepository = null;
        private readonly IProveedor _proveedorRepository = null;
        private readonly IEnvio _envioRepository = null;
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;


        public ReportesController(IVenta ventaRepository, ICompra compraRepository,
            //IVentaServicio ventaServicioRepository,
            Context context, IEmpleado empleadoRepository, ICliente clienteRepository, IProveedor proveedorRepository,
            IEnvio envioRepository, IWebHostEnvironment env)
        {
            _context = context;
            _ventaRepository = ventaRepository;
            _compraRepository = compraRepository;
            //_ventaServicioRepository = ventaServicioRepository;
            _empleadoRepository = empleadoRepository;
            _clienteRepository = clienteRepository;
            _proveedorRepository = proveedorRepository;
            _envioRepository = envioRepository;
            _env = env;

        }
        public IActionResult Utilidad(string fecha)
        {
            var fechas = fecha.Split('-');
            var ventasutilidad = _ventaRepository.GetListadoFecha(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1));
            DateTime fechahoy = DateTime.Now;
            var row = 1;

            //var detalles = _ventaRepository.GetListadoDetalles();

            using (var workboook = new XLWorkbook())
            {
                var worksheet = workboook.Worksheets.Add("Reporte utilidad generado");

                worksheet.Cell(row, 1).Value = "Fecha Emision ";
                worksheet.Cell(row, 2).Value = fechahoy;
                row++;
                row++;

                worksheet.Cell(row, 1).Value = "Venta #";
                worksheet.Cell(row, 2).Value = "Fecha Venta";
                worksheet.Cell(row, 3).Value = "Cliente";
                worksheet.Cell(row, 4).Value = "Vendedor";
                worksheet.Cell(row, 5).Value = "Total Venta";
                worksheet.Cell(row, 6).Value = " Total Adquisición";
                worksheet.Cell(row, 7).Value = "Total Utilidad";




                var sumatoriaTotal = 0.00m;

                foreach (var uti in ventasutilidad)
                {
                    decimal utilidad = 0.00m;
                    decimal costot = 0.00m;
                    decimal ventat = 0.00m;
                    decimal total = 0.00m;

                    foreach (var item in uti.DetalleVenta)
                    {
                        // pcosto = item.Producto.PrecioCosto;
                        // costot += item.Producto.PrecioCosto;
                        // pventa = item.Precio;
                        ventat += item.Total;
                        if (item.Producto != null)
                            costot += item.Producto.PrecioCosto * item.Cantidad;
                        utilidad = ventat - costot;
                        // cantidad = item.Cantidad;
                        // utilidad += cantidad * (pventa - pcosto);
                    }

                    total += utilidad;
                    sumatoriaTotal += total;


                    row++;
                    worksheet.Cell(row, 1).Value = uti.Id;
                    worksheet.Cell(row, 2).Value = uti.FechaVenta;
                    worksheet.Cell(row, 3).Value = uti.Paciente.Nombre;
                    worksheet.Cell(row, 4).Value = uti.Empleado.Nombre;
                    worksheet.Cell(row, 5).Value = "Q" + ventat;
                    worksheet.Cell(row, 6).Value = "Q" + costot;
                    worksheet.Cell(row, 7).Value = "Q" + utilidad;


                }

                row++;
                row++;
                worksheet.Cell(row, 7).Value = $"TOTAL: Q {sumatoriaTotal}";

                using (var stream = new MemoryStream())
                {
                    workboook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officefocument.spreadsheet",
                    "UtilidadVentas.xlsx"
                    );
                }

            }

        }

        public IActionResult ReporteVentas(string fecha, int? empleadoid)
        {
            var fechas = fecha.Split('-');
            var ventas = new List<Venta>();

            if (empleadoid == null)
            {
                ventas = _ventaRepository.GetListadoFecha(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1));
            }
            else
            {
                ventas = _ventaRepository.GetListadoFechaEmpleado(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1), empleadoid);
            }



            DateTime fechahoy = DateTime.Now;
            var row = 1;
            var total = 0.00m;
            var subtotal = 0.00m;
            var descuento = 0.00m;




            //var detalles = _ventaRepository.GetListadoDetalles();

            using (var workboook = new XLWorkbook())
            {
                var worksheet = workboook.Worksheets.Add("Ventas");

                worksheet.Cell(row, 1).Value = "Fecha Emision ";
                worksheet.Cell(row, 2).Value = fechahoy;
                row++;
                row++;

                worksheet.Cell(row, 1).Value = "Vendedor";
                worksheet.Cell(row, 2).Value = "Venta #";
                worksheet.Cell(row, 3).Value = "Cliente";
                worksheet.Cell(row, 4).Value = "NoComprobante";
                worksheet.Cell(row, 5).Value = "Nit";
                worksheet.Cell(row, 6).Value = "Nombres";
                worksheet.Cell(row, 7).Value = "Direccion";
                worksheet.Cell(row, 8).Value = "FechaVenta";
                worksheet.Cell(row, 9).Value = "Subtotal";
                worksheet.Cell(row, 10).Value = "Descuento";
                worksheet.Cell(row, 11).Value = "Total";




                foreach (var ven in ventas)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = ven.Empleado.Nombre;
                    worksheet.Cell(row, 2).Value = ven.Id;
                    worksheet.Cell(row, 3).Value = ven.Paciente?.Nombre ?? ven.Clientes?.Nombre ?? ven.Nombres;
                    worksheet.Cell(row, 4).Value = ven.NoComprobante;
                    worksheet.Cell(row, 5).Value = ven.Nit;
                    worksheet.Cell(row, 6).Value = ven.Nombres;
                    worksheet.Cell(row, 7).Value = ven.Direccion;
                    worksheet.Cell(row, 8).Value = ven.FechaVenta;
                    worksheet.Cell(row, 9).Value = ven.DetalleVenta.Sum(a => a.Subtotal);
                    worksheet.Cell(row, 10).Value = ven.DetalleVenta.Sum(a => a.Descuento);
                    worksheet.Cell(row, 11).Value = ven.DetalleVenta.Sum(a => a.Total);

                    subtotal += ven.DetalleVenta.Sum(a => a.Subtotal);
                    descuento += ven.DetalleVenta.Sum(a => a.Descuento);
                    total += ven.DetalleVenta.Sum(a => a.Total);

                }

                row++;
                worksheet.Cell(row, 10).Value = "Subtotal ";
                worksheet.Cell(row, 11).Value = "Q" + subtotal;

                row++;
                worksheet.Cell(row, 10).Value = "Descuento ";
                worksheet.Cell(row, 11).Value = "Q" + descuento;

                row++;
                worksheet.Cell(row, 10).Value = "Total ";
                worksheet.Cell(row, 11).Value = "Q" + total;


                using (var stream = new MemoryStream())
                {
                    workboook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officefocument.spreadsheet",
                    "Ventas - " + fechas[0] + "a " + fechas[1] + ".xlsx"
                    );

                }



            }

        }

        [Authorize]
        public IActionResult ReporteClientes()
        {
            var clientes = _clienteRepository.GetList()
                .Where(c => c != null && !c.Eliminado)
                .OrderBy(c => c.Nombre)
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Clientes");
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Telefono";
                worksheet.Cell(1, 4).Value = "Celular";
                worksheet.Cell(1, 5).Value = "NIT";
                worksheet.Cell(1, 6).Value = "Direccion";

                var row = 2;
                foreach (var cliente in clientes)
                {
                    worksheet.Cell(row, 1).Value = cliente.Id;
                    worksheet.Cell(row, 2).Value = cliente.Nombre;
                    worksheet.Cell(row, 3).Value = cliente.Telefono;
                    worksheet.Cell(row, 4).Value = cliente.Celular;
                    worksheet.Cell(row, 5).Value = cliente.Nit;
                    worksheet.Cell(row, 6).Value = cliente.Direccion;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Clientes.xlsx");
                }
            }
        }

        [Authorize]
        public IActionResult ReporteEnvios(string fecha)
        {
            var fechas = fecha.Split('-');
            var inicio = DateTime.ParseExact(fechas[0].Trim(), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            var fin = DateTime.ParseExact(fechas[1].Trim(), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture).AddDays(1);

            var envios = _envioRepository.GetListadoFecha(inicio, fin);
            var row = 1;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Envios");
                worksheet.Cell(row, 1).Value = "Envio #";
                worksheet.Cell(row, 2).Value = "Ruta #";
                worksheet.Cell(row, 3).Value = "Piloto";
                worksheet.Cell(row, 4).Value = "Fecha envio";
                worksheet.Cell(row, 5).Value = "Fecha entrega";
                worksheet.Cell(row, 6).Value = "Direccion";
                worksheet.Cell(row, 7).Value = "Estado";

                foreach (var envio in envios)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = envio.Id;
                    worksheet.Cell(row, 2).Value = envio.RutaId;
                    worksheet.Cell(row, 3).Value = envio.NombrePiloto;
                    worksheet.Cell(row, 4).Value = envio.FechaEnvio;
                    worksheet.Cell(row, 5).Value = envio.FechaEntrega;
                    worksheet.Cell(row, 6).Value = envio.DireccionExacta;
                    worksheet.Cell(row, 7).Value = envio.EstadosEnvio?.Estado;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Envios.xlsx");
                }
            }
        }

        public IActionResult ReporteCompras(string fecha, int? empleadoid)
        {
            var fechas = fecha.Split('-');

            var inicio = DateTime.ParseExact(fechas[0].Trim(), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
            var fin = DateTime.ParseExact(fechas[1].Trim(), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture).AddDays(1);

            var compras = new List<Compra>();

            if (empleadoid == null)
            {
                compras = _compraRepository.GetListadoFecha(inicio, fin);
            }
            else
            {
                compras = _compraRepository.GetListadoFechaEmpleado(inicio, fin, empleadoid);
            }

            DateTime fechahoy = DateTime.Now;
            var row = 1;
            var total = 0.00m;

            using (var workboook = new XLWorkbook())
            {
                var worksheet = workboook.Worksheets.Add("Compras");

                worksheet.Cell(row, 1).Value = "Fecha Emisión";
                worksheet.Cell(row, 2).Value = fechahoy;
                row++;
                row++;

                worksheet.Cell(row, 1).Value = "Empleado";
                worksheet.Cell(row, 2).Value = "Compra #";
                worksheet.Cell(row, 3).Value = "Proveedor";
                worksheet.Cell(row, 4).Value = "NoComprobante";
                worksheet.Cell(row, 5).Value = "Estado";
                worksheet.Cell(row, 6).Value = "FechaVenta";
                worksheet.Cell(row, 7).Value = "Total";

                foreach (var com in compras)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = com.Empleado.Nombre;
                    worksheet.Cell(row, 2).Value = com.Id;
                    worksheet.Cell(row, 3).Value = com.Proveedor.Nombre;
                    worksheet.Cell(row, 4).Value = com.NoComprobante;
                    worksheet.Cell(row, 5).Value = com.verEstado();
                    worksheet.Cell(row, 6).Value = com.FechaCompra;
                    worksheet.Cell(row, 7).Value = com.DetalleCompras.Sum(a => a.PrecioTotal);

                    total += com.DetalleCompras.Sum(a => a.PrecioTotal);
                }

                row++;
                worksheet.Cell(row, 6).Value = "Total";
                worksheet.Cell(row, 7).Value = "Q" + total;

                using (var stream = new MemoryStream())
                {
                    workboook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Compras.xlsx");
                }
            }
        }

        //   public IActionResult ReporteVentasServicios(string fecha, int? empleadoid )
        //{


        //    var fechas = fecha.Split('-');

        //    var ventas= new List<VentaServicio>();


        //    if(empleadoid == null)
        //    {
        //         ventas = _ventaServicioRepository.GetListadoFecha(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1));
        //    }
        //    else
        //    {
        //         ventas = _ventaServicioRepository.GetListadoFechaEmpleado(Convert.ToDateTime(fechas[0]), Convert.ToDateTime(fechas[1]).AddDays(1),empleadoid); 
        //    }



        //    DateTime fechahoy = DateTime.Now; 
        //   var row = 1;
        //    var total=0.00m;
        //    var subtotal=0.00m;
        //    var descuento=0.00m;



        //    //var detalles = _ventaRepository.GetListadoDetalles();

        //     using (var workboook = new XLWorkbook())
        //    {
        //        var worksheet = workboook.Worksheets.Add("VentasServicios");

        //        worksheet.Cell(row, 1).Value = "Fecha Emision ";
        //        worksheet.Cell(row, 2).Value = fechahoy;
        //        row++;
        //        row++;

        //        worksheet.Cell(row, 1).Value = "Vendedor";
        //        worksheet.Cell(row, 2).Value = "Venta #";
        //        worksheet.Cell(row, 3).Value = "Cliente";
        //        worksheet.Cell(row, 4).Value = "NoComprobante";
        //        worksheet.Cell(row, 5).Value = "Nit";
        //        worksheet.Cell(row, 6).Value = "Nombres";
        //        worksheet.Cell(row, 7).Value = "Direccion";
        //        worksheet.Cell(row, 8).Value = "FechaVenta";
        //        worksheet.Cell(row, 9).Value = "Subtotal";
        //        worksheet.Cell(row, 10).Value = "Descuento";
        //        worksheet.Cell(row, 11).Value = "Total";




        //        foreach (var ven in ventas)
        //        {
        //             row++;
        //            worksheet.Cell(row, 1).Value = ven.Empleado.Nombre;
        //            worksheet.Cell(row, 2).Value = ven.Id;
        //            worksheet.Cell(row, 3).Value = ven.Paciente.Nombre;
        //            worksheet.Cell(row, 4).Value = ven.NoComprobante;
        //            worksheet.Cell(row, 5).Value = ven.Nit;
        //            worksheet.Cell(row, 6).Value = ven.Nombres;
        //            worksheet.Cell(row, 7).Value = ven.Direccion;
        //            worksheet.Cell(row, 8).Value = ven.FechaVenta;
        //            worksheet.Cell(row, 9).Value = ven.DetalleServicio.Sum(a => a.Subtotal);
        //            worksheet.Cell(row, 10).Value = ven.DetalleServicio.Sum(a => a.Descuento);
        //            worksheet.Cell(row, 11).Value = ven.DetalleServicio.Sum(a => a.Total);

        //            subtotal+=ven.DetalleServicio.Sum(a => a.Subtotal);
        //            descuento+=ven.DetalleServicio.Sum(a => a.Descuento);
        //            total+=ven.DetalleServicio.Sum(a => a.Total);

        //        }

        //        row++;
        //        worksheet.Cell(row, 10).Value = "Subtotal ";
        //        worksheet.Cell(row, 11).Value = "Q"+subtotal;

        //         row++;
        //        worksheet.Cell(row, 10).Value = "Descuento ";
        //        worksheet.Cell(row, 11).Value = "Q"+descuento;

        //         row++;
        //        worksheet.Cell(row, 10).Value = "Total ";
        //        worksheet.Cell(row, 11).Value = "Q"+total;


        //        using (var stream = new MemoryStream())
        //        {
        //            workboook.SaveAs(stream);
        //            var content = stream.ToArray();
        //            return File(content, "application/vnd.openxmlformats-officefocument.spreadsheet",
        //            "VentasServicios.xlsx"
        //            );

        //        }



        //    }

        //}

        public IActionResult ReporteEmpleados()
        {
            var empleado = _empleadoRepository.GetList();
            DateTime fechahoy = DateTime.Now;
            var row = 1;
            int totalEmpleados = empleado.Count(e => e.TipoEmpleado != "Profesional");

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Empleados");

                // Encabezado principal con título, fecha y total de empleados
                worksheet.Range(row, 1, row + 2, 19).Merge();
                worksheet.Cell(row, 1).Value = "Reporte de Empleados - Hospital Naranjo";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 1).Style.Font.FontSize = 20;
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.CornflowerBlue; // Fondo del encabezado
                row += 3;

                worksheet.Cell(row, 1).Value = "Fecha de Emisión:";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 2).Value = fechahoy.ToString("dd/MM/yyyy");

                worksheet.Cell(row, 4).Value = "Total de Empleados:";
                worksheet.Cell(row, 4).Style.Font.Bold = true;
                worksheet.Cell(row, 5).Value = totalEmpleados;
                row += 2;

                // Encabezados de columnas
                worksheet.Cell(row, 1).Value = "Código";
                worksheet.Cell(row, 2).Value = "Nombre";
                worksheet.Cell(row, 3).Value = "Apellido";
                worksheet.Cell(row, 4).Value = "Dirección";
                worksheet.Cell(row, 5).Value = "Edad";
                worksheet.Cell(row, 6).Value = "Sueldo";
                worksheet.Cell(row, 7).Value = "Teléfono";
                worksheet.Cell(row, 8).Value = "Teléfono 2";
                worksheet.Cell(row, 9).Value = "Teléfono 3";
                worksheet.Cell(row, 10).Value = "DPI";
                worksheet.Cell(row, 11).Value = "NIT";
                worksheet.Cell(row, 12).Value = "Estado Civil";
                worksheet.Cell(row, 13).Value = "Tipo Contrato";
                worksheet.Cell(row, 14).Value = "Observaciones";
                worksheet.Cell(row, 15).Value = "Color Fondo";
                worksheet.Cell(row, 16).Value = "Color Texto";
                worksheet.Cell(row, 17).Value = "Imagen";
                worksheet.Cell(row, 18).Value = "Sucursal";
                worksheet.Cell(row, 19).Value = "Tipo de Empleado";

                // Aplicar estilo a los encabezados de columnas
                worksheet.Range(row, 1, row, 19).Style.Font.Bold = true;
                worksheet.Range(row, 1, row, 19).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(row, 1, row, 19).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range(row, 1, row, 19).Style.Fill.BackgroundColor = XLColor.LightSteelBlue; // Fondo de los encabezados
                row++;

                // Definir ancho predefinido de cada columna
                worksheet.Column(1).Width = 15;   // Código
                worksheet.Column(2).Width = 30;   // Nombre
                worksheet.Column(3).Width = 30;   // Apellido
                worksheet.Column(4).Width = 25;   // Dirección
                worksheet.Column(5).Width = 5;    // Edad
                worksheet.Column(6).Width = 10;   // Sueldo
                worksheet.Column(7).Width = 15;   // Teléfono
                worksheet.Column(8).Width = 15;   // Teléfono 2
                worksheet.Column(9).Width = 15;   // Teléfono 3
                worksheet.Column(10).Width = 12;  // DPI
                worksheet.Column(11).Width = 12;  // NIT
                worksheet.Column(12).Width = 10;  // Estado Civil
                worksheet.Column(13).Width = 15;  // Tipo Contrato
                worksheet.Column(14).Width = 75;  // Observaciones
                worksheet.Column(15).Width = 12;  // Color Fondo
                worksheet.Column(16).Width = 12;  // Color Texto
                worksheet.Column(17).Width = 12;  // Imagen
                worksheet.Column(18).Width = 15;  // Sucursal
                worksheet.Column(19).Width = 15;  // Tipo de Empleado

                // Población de datos de empleados
                foreach (var em in empleado.Where(e => e.TipoEmpleado != "Profesional"))
                {
                    worksheet.Cell(row, 1).Value = em.Id;
                    worksheet.Cell(row, 2).Value = em.Nombre;
                    worksheet.Cell(row, 3).Value = em.Apellido;
                    worksheet.Cell(row, 4).Value = em.Direccion;
                    worksheet.Cell(row, 5).Value = em.Edad;
                    worksheet.Cell(row, 6).Value = em.Salario;
                    worksheet.Cell(row, 7).Value = em.Telefono;
                    worksheet.Cell(row, 8).Value = em.Telefono_2;
                    worksheet.Cell(row, 9).Value = em.Email;
                    worksheet.Cell(row, 10).Value = em.Dpi;
                    worksheet.Cell(row, 11).Value = em.Nit;
                    worksheet.Cell(row, 12).Value = em.EstadoCivil;
                    worksheet.Cell(row, 13).Value = em.TipoContrato;
                    worksheet.Cell(row, 14).Value = em.Observaciones;
                    worksheet.Cell(row, 15).Value = em.ColorHexadecimalFondo;
                    worksheet.Cell(row, 16).Value = em.ColorHexadecimalTexto;
                    worksheet.Cell(row, 17).Value = em.Imagen;
                    worksheet.Cell(row, 18).Value = em.Sucursal?.NombreSucursal;
                    worksheet.Cell(row, 19).Value = em.TipoEmpleado;

                    // Centrar vertical y horizontalmente cada celda de datos
                    worksheet.Range(row, 1, row, 19).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range(row, 1, row, 19).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    row++;
                }

                // Bordes para la tabla completa
                worksheet.Range(1, 1, row - 1, 19).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(1, 1, row - 1, 19).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Guardar y retornar el archivo
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Reporte_Empleados_{fechahoy:yyyy-MM-dd}.xlsx");
                }
            }
        }

        public IActionResult ReporteMedicos()
        {
            var empleado = _empleadoRepository.GetList();
            DateTime fechahoy = DateTime.Now;
            var row = 1;
            int totalEmpleados = empleado.Count(e => e.TipoEmpleado == "Profesional");

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Médicos");

                // Encabezado principal con título, fecha y total de médicos
                worksheet.Range(row, 1, row + 2, 32).Merge();
                worksheet.Cell(row, 1).Value = "Reporte de Médicos - Hospital Naranjo";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 1).Style.Font.FontSize = 20;
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(row, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.CornflowerBlue; // Fondo del encabezado
                row += 3;

                worksheet.Cell(row, 1).Value = "Fecha de Emisión:";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 2).Value = fechahoy.ToString("dd/MM/yyyy");

                worksheet.Cell(row, 4).Value = "Total de Medicos:";
                worksheet.Cell(row, 4).Style.Font.Bold = true;
                worksheet.Cell(row, 5).Value = totalEmpleados;
                row += 2;

                // Encabezados de columnas
                string[] headers = {
                    "Código", "Nombre", "Apellido", "Dirección", "Edad", "Sueldo", "Teléfono", "Teléfono 2", "Teléfono 3",
                    "DPI", "NIT", "Estado Civil", "Tipo Contrato", "Observaciones", "Color Fondo", "Color Texto", "Imagen",
                    "Sucursal", "Tipo de Empleado", "Colegiado", "Género", "Residente", "Credenciales", "Dirección de la Clínica",
                    "Teléfono de la Clínica", "Tipo Banco", "Tipo Cuenta", "Número Cuenta", "Nombre Cuenta", "NIT Propietario Cuenta",
                    "Nombre Propietario NIT", "Tipo Régimen"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(row, i + 1).Value = headers[i];
                }

                // Aplicar estilo a los encabezados de columnas
                worksheet.Range(row, 1, row, 32).Style.Font.Bold = true;
                worksheet.Range(row, 1, row, 32).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(row, 1, row, 32).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range(row, 1, row, 32).Style.Fill.BackgroundColor = XLColor.LightSteelBlue; // Fondo de los encabezados
                row++;

                // Definir ancho predefinido de cada columna
                worksheet.Columns().AdjustToContents();

                // Definir ancho predefinido de cada columna
                worksheet.Column(1).Width = 15;   // Código
                worksheet.Column(2).Width = 30;   // Nombre
                worksheet.Column(3).Width = 30;   // Apellido
                worksheet.Column(4).Width = 25;   // Dirección
                worksheet.Column(5).Width = 5;    // Edad
                worksheet.Column(6).Width = 15;   // Sueldo
                worksheet.Column(7).Width = 20;   // Teléfono
                worksheet.Column(8).Width = 20;   // Teléfono 2
                worksheet.Column(9).Width = 20;   // Teléfono 3
                worksheet.Column(10).Width = 12;  // DPI
                worksheet.Column(11).Width = 12;  // NIT
                worksheet.Column(12).Width = 10;  // Estado Civil
                worksheet.Column(13).Width = 15;  // Tipo Contrato
                worksheet.Column(14).Width = 75;  // Observaciones
                worksheet.Column(15).Width = 12;  // Color Fondo
                worksheet.Column(16).Width = 12;  // Color Texto
                worksheet.Column(17).Width = 12;  // Imagen
                worksheet.Column(18).Width = 20;  // Sucursal
                worksheet.Column(19).Width = 20;  // Tipo de Empleado
                worksheet.Column(20).Width = 20; // Colegiado
                worksheet.Column(21).Width = 20; // Genero
                worksheet.Column(22).Width = 20; // Residente
                worksheet.Column(23).Width = 20; // Credenciales
                worksheet.Column(24).Width = 25; // DireccionClinica
                worksheet.Column(25).Width = 20; // TelefonoClinica
                worksheet.Column(26).Width = 20; // TipoBanco
                worksheet.Column(27).Width = 20; // TipoCuenta
                worksheet.Column(28).Width = 20; // NumeroCuenta
                worksheet.Column(29).Width = 20; // NombreCuenta
                worksheet.Column(30).Width = 20; // NitPropietarioCuenta
                worksheet.Column(31).Width = 20; // NombrePropietarioNit
                worksheet.Column(32).Width = 20; // TipoRegimen

                // Población de datos de médicos
                foreach (var em in empleado.Where(e => e.TipoEmpleado == "Profesional"))
                {
                    worksheet.Cell(row, 1).Value = em.Id;
                    worksheet.Cell(row, 2).Value = em.Nombre;
                    worksheet.Cell(row, 3).Value = em.Apellido;
                    worksheet.Cell(row, 4).Value = em.Direccion;
                    worksheet.Cell(row, 5).Value = em.Edad;
                    worksheet.Cell(row, 6).Value = em.Salario;
                    worksheet.Cell(row, 7).Value = em.Telefono;
                    worksheet.Cell(row, 8).Value = em.Telefono_2;
                    worksheet.Cell(row, 9).Value = em.Email;
                    worksheet.Cell(row, 10).Value = em.Dpi;
                    worksheet.Cell(row, 11).Value = em.Nit;
                    worksheet.Cell(row, 12).Value = em.EstadoCivil;
                    worksheet.Cell(row, 13).Value = em.TipoContrato;
                    worksheet.Cell(row, 14).Value = em.Observaciones;
                    worksheet.Cell(row, 15).Value = em.ColorHexadecimalFondo;
                    worksheet.Cell(row, 16).Value = em.ColorHexadecimalTexto;
                    worksheet.Cell(row, 17).Value = em.Imagen;
                    worksheet.Cell(row, 18).Value = em.Sucursal?.NombreSucursal;
                    worksheet.Cell(row, 19).Value = em.TipoEmpleado;
                    worksheet.Cell(row, 20).Value = em.Colegiado;
                    worksheet.Cell(row, 21).Value = em.Genero;
                    worksheet.Cell(row, 22).Value = em.Residente;
                    worksheet.Cell(row, 23).Value = em.Credenciales;
                    worksheet.Cell(row, 24).Value = em.DireccionClinica;
                    worksheet.Cell(row, 25).Value = em.TelefonoClinica;
                    worksheet.Cell(row, 26).Value = em.TipoBanco;
                    worksheet.Cell(row, 27).Value = em.TipoCuenta;
                    worksheet.Cell(row, 28).Value = em.NumeroCuenta;
                    worksheet.Cell(row, 29).Value = em.NombreCuenta;
                    worksheet.Cell(row, 30).Value = em.NitPropietarioCuenta;
                    worksheet.Cell(row, 31).Value = em.NombrePropietarioNit;
                    worksheet.Cell(row, 32).Value = em.TipoRegimen;

                    // Centrar vertical y horizontalmente cada celda de datos
                    worksheet.Range(row, 1, row, 32).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range(row, 1, row, 32).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    row++;
                }

                // Bordes para la tabla completa
                worksheet.Range(1, 1, row - 1, 32).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(1, 1, row - 1, 32).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Guardar y retornar el archivo
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Reporte_Medicos_{fechahoy:yyyy-MM-dd}.xlsx");
                }
            }
        }


        public IActionResult ReporteProveedores()
        {
            var proveedores = _proveedorRepository.GetList();

            DateTime fechahoy = DateTime.Now;
            var row = 1;



            using (var workboook = new XLWorkbook())
            {
                var worksheet = workboook.Worksheets.Add("Proveedores");

                worksheet.Cell(row, 1).Value = "Fecha Emision ";
                worksheet.Cell(row, 2).Value = fechahoy;
                row++;
                row++;

                worksheet.Cell(row, 1).Value = "Nombre";
                worksheet.Cell(row, 2).Value = "Giro";
                worksheet.Cell(row, 3).Value = "Telefono";
                worksheet.Cell(row, 4).Value = "Nit";
                worksheet.Cell(row, 5).Value = "Direccion";
                worksheet.Cell(row, 6).Value = "Correo";
                worksheet.Cell(row, 7).Value = "Celular";
                worksheet.Cell(row, 8).Value = "Cuenta Bancaria";
                worksheet.Cell(row, 9).Value = "Tipo de Proveedor";
                worksheet.Cell(row, 10).Value = "Observaciones";


                foreach (var item in proveedores)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = item.Nombre;
                    worksheet.Cell(row, 2).Value = item.Giro;
                    worksheet.Cell(row, 3).Value = item.Telefono_1;
                    worksheet.Cell(row, 4).Value = item.Nit;
                    worksheet.Cell(row, 5).Value = item.Direccion;
                    worksheet.Cell(row, 6).Value = item.Correo;
                    worksheet.Cell(row, 7).Value = item.Celular_1;
                    worksheet.Cell(row, 8).Value = item.CuentaBancaria;
                    worksheet.Cell(row, 9).Value = item.TipoProveedor;
                    worksheet.Cell(row, 10).Value = item.Observaciones;

                }


                using (var stream = new MemoryStream())
                {
                    workboook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officefocument.spreadsheet",
                    "Proveedores- " + fechahoy + ".xlsx"
                    );

                }



            }

        }
        [Authorize]
        public IActionResult Ine(string rangoFechas, int? departamentoId = null, int? municipioId = null)
        {
            var model = new ReportesIneViewModel
            {
                Departamentos = _context.Departamentos
                                            .Select(d => new SelectListItem
                                            {
                                                Value = d.Id.ToString(),
                                                Text = d.NombreDepartamento
                                            }).ToList(),
                Municipios = Enumerable.Empty<SelectListItem>(),
                RangoFechas = rangoFechas,
                SelectedDepartamentoId = departamentoId,
                SelectedMunicipioId = municipioId    // ← agrega esto
            };

            // 1.2. Si viene departamentoId, poblar lista de municipios
            if (departamentoId.HasValue)
            {
                model.Municipios = _context.Municipios
                    .Where(m => m.DepartamentoId == departamentoId.Value)
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.NombreMunicipio
                    })
                    .ToList();
            }
            if (municipioId.HasValue)
            {
                model.SelectedMunicipioCodigo = municipioId.Value.ToString();

            }
            if (!string.IsNullOrWhiteSpace(rangoFechas))
            {
                if (TryParseIneRangoFechas(rangoFechas, out var inicio, out var fin))
                {
                    model.Registros = GetIneRegistros(inicio, fin, departamentoId, municipioId);
                }
                else
                {
                    ModelState.AddModelError("rangoFechas",
                        "El rango debe tener el formato yyyy-MM-dd - yyyy-MM-dd.");
                }
            }

            return View(model);
        }
        [HttpGet]
        public async Task<JsonResult> GetMunicipios(int departamentoId)
        {
            var lista = await _context.Municipios
                .Where(m => m.DepartamentoId == departamentoId)
                .Select(m => new
                {
                    id = m.Id,
                    nombre = m.NombreMunicipio

                })
                .ToListAsync();

            return Json(lista);
        }
        [HttpGet]
    public IActionResult DescargarIneExcel(
        string rangoFechas,
        int? departamentoId = null,
        int? municipioId    = null)
    {
        // 1) Dividir el rango en dos fechas
        var trozos = rangoFechas.Split(" - ", StringSplitOptions.None);
        if (trozos.Length != 2)
            throw new ArgumentException("rangoFechas debe tener formato yyyy-MM-dd - yyyy-MM-dd");

        // 2) Parsear inicio y fin (+1 día al fin)
        var inicio = DateTime.ParseExact(trozos[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var fin    = DateTime.ParseExact(trozos[1], "yyyy-MM-dd", CultureInfo.InvariantCulture)
                            .AddDays(1);
    var registrosVm = GetIneRegistros(inicio, fin, departamentoId, municipioId);

        // 4) Cargar Departamento y Municipio seleccionados
         // 1) Cargar nombres y códigos
    var departamento = departamentoId.HasValue
        ? _context.Departamentos.Find(departamentoId.Value)
        : null;
    var municipio = municipioId.HasValue
        ? _context.Municipios.Find(municipioId.Value)
        : null;


        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("INE");

        //
        // ——————————————
        //   HOJA OCULTA
        // ——————————————
        var allDeptos = _context.Departamentos
            .OrderBy(d => d.NombreDepartamento)
            .ToList();
        var allMuni  = _context.Municipios
            .OrderBy(m => m.DepartamentoId)
            .ThenBy(m => m.NombreMunicipio)
            .ToList();

        var hidden = wb.Worksheets.Add("_listas_");
        // A/B: Departamentos
        for (int i = 0; i < allDeptos.Count; i++)
        {
            hidden.Cell(i+1, 1).Value = allDeptos[i].NombreDepartamento;
            hidden.Cell(i+1, 2).Value = allDeptos[i].Id;
        }
        // A partir de la col 4: un bloque por departamento con municipios y su código
        int startCol = 4;
        for (int d = 0; d < allDeptos.Count; d++)
    {
        var dept  = allDeptos[d];
        var lista = allMuni.Where(m => m.DepartamentoId == dept.Id).ToList();
        if (!lista.Any()) continue;

        int colNames = startCol + d;
        int colCodes = colNames + allDeptos.Count + 1;

        for (int j = 0; j < lista.Count; j++)
        {
            hidden.Cell(j+1, colNames).Value = lista[j].NombreMunicipio;
            hidden.Cell(j+1, colCodes).Value = lista[j].Id;
        }

        // Limpia espacios para el NamedRange
        string clean = Regex.Replace(dept.NombreDepartamento, @"\s+", "_");

        // NO ASIGNAR EL RESULTADO: sólo invócalo
        wb.NamedRanges.Add(
            clean,
            hidden.Range(1, colNames, lista.Count, colNames)
        );
        wb.NamedRanges.Add(
            clean + "_C",
            hidden.Range(1, colCodes, lista.Count, colCodes)
        );
    }
        hidden.Visibility = XLWorksheetVisibility.VeryHidden;

        //
        // ——————————————
        //   VALIDACIONES
        // ——————————————
        // Dep. (A6)
        var dvDept = ws.Range("A6").SetDataValidation();
        dvDept.List(hidden.Range(1,1,allDeptos.Count,1)
                        .RangeAddress.ToString(XLReferenceStyle.A1, true));
        dvDept.InCellDropdown = true;
        dvDept.IgnoreBlanks   = true;
        // Código Dep. (C6) via VLOOKUP
        ws.Cell("C6").FormulaA1 =
        $"=IF(A6<>\"\",VLOOKUP(A6,'_listas_'!$A:$B,2,FALSE),\"\")";

        // Mun. (A9) dependiente
        var dvMun = ws.Range("A9").SetDataValidation();
        dvMun.List("=INDIRECT(SUBSTITUTE(A6,\" \",\"_\"))");
        dvMun.InCellDropdown = true;
        dvMun.IgnoreBlanks   = true;
        // Código Mun. (C9)
        ws.Cell("C9").FormulaA1 =
        $"=IF(A9<>\"\",VLOOKUP(A9,INDIRECT(SUBSTITUTE(A6,\" \",\"_\" & \"_C\")),2,FALSE),\"\")";

        //
        // ——————————————
        //   DISEÑO PRINCIPAL
        // ——————————————
        var green   = XLColor.FromHtml("#C6EFCE");
        int lastCol = 21;

        // 1) Logo
        var logoPath = Path.Combine(_env.WebRootPath, "images", "ine-logo.png");
        if (System.IO.File.Exists(logoPath))
            ws.AddPicture(logoPath)
            .MoveTo(ws.Cell(1, 1))
            .WithSize(80, 80);

        // 2) Cajita Dep/Mun
        ws.Range("A5:B5").Merge().Value = "DEPARTAMENTO";
        ws.Cell("C5").Value                = "CÓDIGO";
        // Valores seleccionados
        ws.Range("A6:B6").Merge().Value = departamento?.NombreDepartamento ?? "Todos";
        ws.Cell("C6").Value            = departamento?.Id.ToString()         ?? "";

        // Estilo verde/borde (igual que ya tenías)
        ws.Range(5,1,6,3).Style.Fill.BackgroundColor = green;
        ws.Range(5,1,6,3).Style.Border.OutsideBorder  = XLBorderStyleValues.Thin;
        ws.Range(5,1,6,3).Style.Border.InsideBorder   = XLBorderStyleValues.Thin;

// ——————— Cajita Mun ———————
// Encabezados
        ws.Range("A8:B8").Merge().Value = "MUNICIPIO";
        ws.Cell("C8").Value              = "CÓDIGO";
        // Valores seleccionados
        ws.Range("A9:B9").Merge().Value = municipio?.NombreMunicipio ?? "Todos";
        ws.Cell("C9").Value            = municipio?.Id.ToString()       ?? "";

        // Estilo verde/borde
        ws.Range(8,1,9,3).Style.Fill.BackgroundColor = green;
        ws.Range(8,1,9,3).Style.Border.OutsideBorder  = XLBorderStyleValues.Thin;
        ws.Range(8,1,9,3).Style.Border.InsideBorder   = XLBorderStyleValues.Thin;
        // 3) Cabecera principal
        var headers = new[]
        {
            (r1:1,c1:1,r2:1,c2:17,text:"REGISTRO DIARIO DE CONSULTA EXTERNA EN"),
            (r1:2,c1:1,r2:2,c2:17,text:"CENTROS HOSPITALARIOS PRIVADOS"),
            (r1:3,c1:1,r2:3,c2:17,text:"REPUBLICA DE GUATEMALA"),
            (r1:5,c1:7,r2:5,c2:17,text:"HOSPITAL CLÍNICO QUIRÚRGICO")
        };
        foreach (var h in headers)
        {
            var r = ws.Range(h.r1,h.c1,h.r2,h.c2).Merge();
            r.Value = h.text;
            r.Style.Font.Bold      = true;
            r.Style.Font.FontSize  = 12;
            r.Style.Alignment.WrapText = true;
            r.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            r.Style.Alignment.SetVertical(  XLAlignmentVerticalValues.Center);
        }

        var forma = ws.Range(5, 19, 6, 21);
    forma.Merge().Value = "FORMA INEEH-01";
    forma.Style.Fill.BackgroundColor = green;
    forma.Style.Font.Bold = true;

    // Obtén la sección de alineación en una variable
    var aline = forma.Style.Alignment;

    // Ahora sí puedes llamar a ambos métodos sobre IXLAlignment
    aline.SetHorizontal(XLAlignmentHorizontalValues.Center);
    aline.SetVertical(  XLAlignmentVerticalValues.Center);

    // Etiquetas
    ws.Cell(7, 19).Value = "MES";
    ws.Cell(7, 20).Value = "AÑO";

    // Valores con la fecha actual
    var ahora = DateTime.Now;
    ws.Cell(8, 19).Value = ahora.ToString("MMMM", new CultureInfo("es-ES"));
    ws.Cell(8, 20).Value = ahora.Year;

    // Aplicar relleno verde a MES/AÑO
    ws.Range(7, 19, 8, 21)
    .Style.Fill.BackgroundColor = green;

        // 5) Encabezados de columna (filas 11–12)
        var cols = new[]
        {
            (11,1,12,1,"NO. ORDEN"),
            (11,2,12,2,"1/ Paciente tiene derecho a IGSS"),
            (11,3,12,3,"DÍA DE CONSULTA"),
            (11,4,12,4,"NO. HISTORIA CLÍNICA\n(o código de paciente)"),
            (11,5,12,5,"2/ SEXO"),
            (11,6,12,6,"3/ PUEBLO DE PERTENENCIA"),
            (11,7,11,9,"4/ EDAD"),
            (12,7,12,7,"Días"),
            (12,8,12,8,"Meses"),
            (12,9,12,9,"Años"),
            (11,10,11,12,"LUGAR DE RESIDENCIA HABITUAL"),
            (12,10,12,10,"Dirección exacta"),
            (12,11,12,11,"Municipio / Departamento"),
            (12,12,12,12,"Código cartográfico (INE)"),
            (11,13,12,13,"5/ CONSULTA"),
            (11,14,12,14,"6/ CONTROL MATERNO-INFANTIL"),
            (11,15,11,16,"DIAGNÓSTICO"),
            (12,15,12,15,"Descripción"),
            (12,16,12,16,"CIE-10"),
            (11,17,12,17,"7/ TRATAMIENTO RECIBIDO EN")
        };
        foreach (var c in cols)
        {
            var r = ws.Range(c.Item1,c.Item2,c.Item3,c.Item4).Merge();
            r.Value = c.Item5;
            r.Style.Fill.BackgroundColor = green;
            r.Style.Font.Bold = true;
            r.Style.Alignment.WrapText = true;
            r.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            r.Style.Alignment.SetVertical(  XLAlignmentVerticalValues.Center);
        }

        // 6) Población de datos (desde fila 13)
        int fila = 13;
        var hoy  = DateTime.Today;
        foreach (var vm in registrosVm)
        {
            ws.Cell(fila,  1).Value = vm.NoOrden;
        ws.Cell(fila,  2).Value = vm.TieneIgss;
        ws.Cell(fila,  3).Value = vm.FechaConsulta;
        ws.Cell(fila,  4).Value = vm.HistoriaClinica;
        ws.Cell(fila,  5).Value = vm.Sexo;                // ← aquí ya está "Hombre"/"Mujer"
        ws.Cell(fila,  6).Value = vm.Etnia;
        ws.Cell(fila,  7).Value = vm.DiasEdad;
        ws.Cell(fila,  8).Value = vm.MesesEdad;
        ws.Cell(fila,  9).Value = vm.AniosEdad;
        ws.Cell(fila, 10).Value = vm.Direccion;
        ws.Cell(fila, 11).Value = vm.Ubicacion;
        ws.Cell(fila, 12).Value = vm.CodigoCartografico;
        ws.Cell(fila, 13).Value = vm.TipoConsulta;
        ws.Cell(fila, 14).Value = vm.ControlMaternoInfantil;
        ws.Cell(fila, 15).Value = vm.ImpresionClinica;
            ws.Cell(fila, 16).Value = "";  // CIE-10
            ws.Cell(fila, 17).Value = vm.TratamientoRecibidoEn;  // Tratamiento

            fila++;
        }
        int filaFinal = fila - 1;

        // 7) Sólo bordes en la tabla de datos
        var tableRange = ws.Range(11, 1, filaFinal, lastCol);
        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.InsideBorder  = XLBorderStyleValues.Thin;

        // 8) Leyendas agrupadas justo debajo
        int leyRow = filaFinal + 2;
        ws.Row(leyRow).Height = 40;
        ws.Range(leyRow,1,   leyRow,2).Merge().Value =
        "1/ Paciente tiene derecho a IGSS\n1=Si   2=No";
        ws.Range(leyRow,3,   leyRow,4).Merge().Value =
        "2/ SEXO\n1=Hombre   2=Mujer";
        ws.Range(leyRow,5,   leyRow,7).Merge().Value =
        "3/ PUEBLO DE PERTENENCIA\n1=Maya 2=Garifuna 3=Xinka\n4=Mestizo/Ladino 5=Ninguno 6=Otro";
        ws.Range(leyRow,8,   leyRow,10).Merge().Value =
        "4/ EDAD EXCLUYENTE\nDías:0-29   Meses:1-11   Años:1-125";
        ws.Range(leyRow,11,  leyRow,13).Merge().Value =
        "5/ CONSULTA\n1=Primera   2=Reconsulta   3=Emergencia\n4=Inmunizaciones   5=Traumatología";
        ws.Range(leyRow,14,  leyRow,16).Merge().Value =
        "6/ CONTROL MATERNO-INFANTIL\n1=Prenatal   2=Puerperio\n3=Crecimiento y desarrollo   4=Inmunizaciones";
        ws.Range(leyRow,17,  leyRow,lastCol).Merge().Value =
        "7/ TRATAMIENTO RECIBIDO EN\n1=Medicina 2=Cirugía 3=Pediatría\n4=Ginecología y obstetricia 5=Traumatología 6=Oftalmología\n7=Odontología 8=Salud mental 9=Radioterapia\n10=Quimioterapia 11=Otro";

        ws.Range(leyRow,1, leyRow, lastCol).Style.Font.FontSize = 8;
        ws.Range(leyRow,1, leyRow, lastCol).Style.Alignment.WrapText = true;

        // 9) Firma (3 filas más abajo)
        int footer = leyRow + 3;
        ws.Range(footer,  1, footer,  7).Merge().Value = "";
        ws.Range(footer+1,1, footer+1,7).Merge().Value = "NOMBRE DE DIRECTOR MÉDICO";
        ws.Range(footer, 15, footer,lastCol).Merge().Value = "FIRMA Y SELLO";

        // 10) Ajuste final de columnas
        ws.Columns().AdjustToContents();

        // 11) Devolver archivo
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return File(
            ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"INE_{inicio:yyyyMMdd}-{fin:yyyyMMdd}.xlsx"
        );
    }

        private static bool TryParseIneRangoFechas(string rangoFechas, out DateTime inicio, out DateTime fin)
        {
            inicio = default;
            fin = default;

            if (string.IsNullOrWhiteSpace(rangoFechas))
                return false;

            var texto = rangoFechas.Trim();
            const string formato = "yyyy-MM-dd";

            foreach (var separador in new[] { " - ", " – " })
            {
                if (!texto.Contains(separador, StringComparison.Ordinal))
                    continue;

                var trozos = texto.Split(separador, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (trozos.Length != 2)
                    continue;

                if (DateTime.TryParseExact(trozos[0], formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out inicio)
                    && DateTime.TryParseExact(trozos[1], formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var finSinDia))
                {
                    fin = finSinDia.AddDays(1);
                    return true;
                }
            }

            var partes = texto
                .Split('-', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToArray();

            if (partes.Length != 6)
                return false;

            var inicioTxt = $"{partes[0]}-{partes[1]}-{partes[2]}";
            var finTxt = $"{partes[3]}-{partes[4]}-{partes[5]}";

            if (!DateTime.TryParseExact(inicioTxt, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out inicio)
                || !DateTime.TryParseExact(finTxt, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var finDia))
                return false;

            fin = finDia.AddDays(1);
            return true;
        }

        private List<ConsultaIneViewModel> GetIneRegistros(DateTime inicio, DateTime fin, int? departamentoId = null, int? municipioId    = null )
        {
          var query = _context.Consultas
    // 1) Cargo Historia para que r.Historia.ImpresionClinica funcione
    .Include(q => q.Historia)

    // 2) Cargo Citas → Paciente → Municipio
    .Include(q => q.Citas)
        .ThenInclude(cita => cita.Paciente)
            .ThenInclude(pac => pac.Municipio)

    // 3) Cargo Citas → Paciente → Departamento
    .Include(q => q.Citas)
        .ThenInclude(cita => cita.Paciente)
            .ThenInclude(pac => pac.Departamento)

    // 4) Cargo Citas → Especialidad
    .Include(q => q.Citas)
        .ThenInclude(cita => cita.Especialidad)

    // 5) Tu filtro de fechas y parámetros opcionales
    .Where(q => q.FechaYHoraInicioConsulta >= inicio
             && q.FechaYHoraInicioConsulta <  fin
             && (!departamentoId.HasValue || q.Citas.Paciente.DepartamentoId == departamentoId.Value)
             && (!municipioId   .HasValue || q.Citas.Paciente.MunicipioId     == municipioId.Value)
    );
var municipiosDict = _context.Municipios
    .Include(m => m.Departamento)
    .Where(m => m.Departamento != null && !string.IsNullOrWhiteSpace(m.NombreMunicipio) && !string.IsNullOrWhiteSpace(m.Departamento.NombreDepartamento))
    .ToDictionary(
        m => m.NombreMunicipio.Trim().ToUpper() + "|" + m.Departamento.NombreDepartamento.Trim().ToUpper(),
        m => m.Id
    );


            var hoy = DateTime.Today;
            return query
            .AsEnumerable()
            .Select((r, idx) => {
                var pac = r.Citas.Paciente;
                // cálculo de edad igual que en DescargarIneExcel
                var nac = pac.FechaNacimiento ?? hoy;
                int anios = hoy.Year - nac.Year - ((hoy < nac.AddYears(hoy.Year - nac.Year)) ? 1 : 0);
                int meses = hoy.Month - nac.Month - ((hoy.Day < nac.Day) ? 1 : 0);
                if (meses < 0) meses += 12;
                int dias = hoy.Day - nac.Day;
                if (dias < 0) dias += DateTime.DaysInMonth(hoy.Year, hoy.Month == 1 ? 12 : hoy.Month - 1);

                return new ConsultaIneViewModel
                {
                    NoOrden = idx + 1,
                    TieneIgss = pac.esta_Afiliado ? "Sí" : "No",
                    FechaConsulta = r.FechaYHoraInicioConsulta,
                    HistoriaClinica = pac.Id.ToString(),
                    Sexo = pac.SexoId == 1 ? "Hombre"
                            : pac.SexoId == 2 ? "Mujer"
                            : "",
                    Etnia = pac.EtniaPaciente ?? "",
                    DiasEdad = dias,
                    MesesEdad = meses,
                    AniosEdad = anios,
                    Direccion = pac.Direccion,
                   Ubicacion =
                                (pac.Municipio != null && pac.Departamento != null)
                                    ? $"{pac.Municipio.NombreMunicipio}/{pac.Departamento.NombreDepartamento}"
                                    : (!string.IsNullOrWhiteSpace(pac.MunicipioResidencia) && !string.IsNullOrWhiteSpace(pac.DepartamentoResidencia))
                                        ? $"{pac.MunicipioResidencia}/{pac.DepartamentoResidencia}"
                                        : "",
                    


CodigoCartografico =
    pac.MunicipioId.HasValue
        ? pac.MunicipioId.Value.ToString()
        : (!string.IsNullOrWhiteSpace(pac.MunicipioResidencia) && !string.IsNullOrWhiteSpace(pac.DepartamentoResidencia))
            ? (municipiosDict.TryGetValue(
                    pac.MunicipioResidencia.Trim().ToUpper() + "|" + pac.DepartamentoResidencia.Trim().ToUpper(),
                    out var id)
                ? id.ToString()
                : "")
            : "",

                    TipoConsulta = r.TipoConsulta,
                    ControlMaternoInfantil = r.EstaEmbarazada ?? "",
                    ImpresionClinica = r.Historia?.ImpresionClinica ?? "",
                    cie = r.Cie10Codigo ?? "",
                    TratamientoRecibidoEn = r.Citas?.Especialidad?.NombreEspecialidad ?? ""
                    // etc… añade los campos de diagnóstico y tratamiento si los necesitas
                };
            })
            .ToList();
        }

    }


}