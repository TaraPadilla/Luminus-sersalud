using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wkhtmltopdf.NetCore;
using System.Threading.Tasks;
using Database.Shared.IRepository;

namespace sistema.Controllers
{
    [Authorize]
    public class MedicamentosProximosVencerController : Controller
    {
        private readonly IGeneratePdf        _generatePdf;
        private readonly IDevolucionNacional _devolucionRepository;

        public MedicamentosProximosVencerController(
            IGeneratePdf        generatePdf,
            IDevolucionNacional devolucionRepository)
        {
            _generatePdf          = generatePdf;
            _devolucionRepository = devolucionRepository;
        }

        private static string ObtenerNumeroFormulario(int? numeroOficio)
        {
            if (!numeroOficio.HasValue) return "000000";
            return numeroOficio.Value.ToString("D6");
        }

     
        private async Task<List<MedicamentoProximoVencerItemViewModel>> ObtenerDatosAsync()
        {
            // Campos quemados
            const string fechaFormulario         = "01/03/2026";
            const string fechaEntregaProveedor   = "03/03/2026";
            const string fechaRecepcionReingreso = "10/03/2026";
            const int    diasCartaCompromiso      = 7;

            var listaDTO = await _devolucionRepository.GetAllAsync();

            var resultado = new List<MedicamentoProximoVencerItemViewModel>();
            int numero = 1;

            foreach (var dto in listaDTO.Where(d => d.Estado == Models.DevolucionEstado.Autorizado).OrderBy(d => d.Id))
            {
                var dev = await _devolucionRepository.GetByIdAsync(dto.Id);

                if (dev?.Detalles == null || !dev.Detalles.Any())
                    continue;

                string oficioTexto = dev.NumeroOficio.HasValue
                    ? dev.NumeroOficio.Value.ToString("D3") + "-" + dev.FechaSolicitud?.Year.ToString()
                    : "-";

                string fechaTexto = dev.FechaSolicitud.HasValue
                    ? dev.FechaSolicitud.Value.ToString("dd/MM/yyyy")
                    : "-";

                foreach (var detalle in dev.Detalles)
                {
                    var inv            = detalle.ProductoInventario;
                    var nombreProducto = inv?.Producto?.NombreProducto ?? "-";
                    var unidad         = inv?.UnidadMedidaVenta?.Nombre ?? "unidad";
                    var lote           = !string.IsNullOrEmpty(inv?.Lote)
                                            ? $" Lote: {inv.Lote}"
                                            : "";
                    var vencimiento    = (inv?.FechaVencimientoArticuloCompra.HasValue == true)
                                            ? $" Venc: {inv.FechaVencimientoArticuloCompra.Value:MM-yyyy}"
                                            : "";

                    var descripcion    = $"{detalle.CantidadDevuelta} {unidad} de {nombreProducto}{lote}{vencimiento}";

                    decimal precioUnitario = inv?.PrecioCosto ?? 0m;
                    decimal precioTotal    = detalle.CantidadDevuelta * precioUnitario;

                    resultado.Add(new MedicamentoProximoVencerItemViewModel
                    {
                        Numero                       = numero++,
                        NumeroFormulario             = ObtenerNumeroFormulario(dev.NumeroOficio),
                        FechaFormularioTexto         = fechaFormulario,
                        Oficio                       = oficioTexto,
                        FechaTexto                   = fechaTexto,
                        Unidad                       = dto.UnidadNombre ?? "-",
                        Descripcion                  = descripcion,
                        PrecioUnitario               = precioUnitario,
                        PrecioTotal                  = precioTotal,
                        FechaEntregaProveedorTexto   = fechaEntregaProveedor,
                        FechaRecepcionReingresoTexto = fechaRecepcionReingreso,
                        DiasCartaCompromiso          = diasCartaCompromiso
                    });
                }
            }

            return resultado;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await ObtenerDatosAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcel()
        {
            var datos = await ObtenerDatosAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("MEDICAMENTOS PROX. VENCER");

            const int totalColumnas = 12;

            ws.Cell(1, 1).Value = "LISTADO DE MEDICAMENTOS ENVIADOS A CAMBIO VENCIMIENTO, EJECUTANDO LAS CARTAS DE COMPROMISOS CORRESPONDIENTES.";
            ws.Range(1, 1, 1, totalColumnas).Merge();
            ws.Cell(1, 1).Style.Font.Bold            = true;
            ws.Cell(1, 1).Style.Font.FontSize        = 12;
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(1, 1).Style.Alignment.Vertical   = XLAlignmentVerticalValues.Center;
            ws.Cell(1, 1).Style.Alignment.WrapText   = true;

            const int filaEncabezado = 3;

            ws.Cell(filaEncabezado, 1).Value  = "No.";
            ws.Cell(filaEncabezado, 2).Value  = "No. de Formulario";
            ws.Cell(filaEncabezado, 3).Value  = "Fecha de Formulario";
            ws.Cell(filaEncabezado, 4).Value  = "Oficio";
            ws.Cell(filaEncabezado, 5).Value  = "Fecha";
            ws.Cell(filaEncabezado, 6).Value  = "Unidad";
            ws.Cell(filaEncabezado, 7).Value  = "Descripcion";
            ws.Cell(filaEncabezado, 8).Value  = "Precio Unitario";
            ws.Cell(filaEncabezado, 9).Value  = "Precio Total";
            ws.Cell(filaEncabezado, 10).Value = "Fecha de Entrega al Proveedor";
            ws.Cell(filaEncabezado, 11).Value = "Fecha de Recepción de Reingreso";
            ws.Cell(filaEncabezado, 12).Value = "Dias que indica la Carta de Compromiso";

            var rangoEncabezado = ws.Range(filaEncabezado, 1, filaEncabezado, totalColumnas);
            rangoEncabezado.Style.Font.Bold            = true;
            rangoEncabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangoEncabezado.Style.Alignment.Vertical   = XLAlignmentVerticalValues.Center;
            rangoEncabezado.Style.Alignment.WrapText   = true;
            rangoEncabezado.Style.Fill.BackgroundColor = XLColor.LightGray;
            rangoEncabezado.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangoEncabezado.Style.Border.InsideBorder  = XLBorderStyleValues.Thin;

            var filaActual = filaEncabezado + 1;

            foreach (var item in datos)
            {
                ws.Cell(filaActual, 1).Value  = item.Numero;
                ws.Cell(filaActual, 2).Value  = item.NumeroFormulario;
                ws.Cell(filaActual, 3).Value  = item.FechaFormularioTexto;
                ws.Cell(filaActual, 4).Value  = item.Oficio;
                ws.Cell(filaActual, 5).Value  = item.FechaTexto;
                ws.Cell(filaActual, 6).Value  = item.Unidad;
                ws.Cell(filaActual, 7).Value  = item.Descripcion;
                ws.Cell(filaActual, 8).Value  = item.PrecioUnitario;
                ws.Cell(filaActual, 9).Value  = item.PrecioTotal;
                ws.Cell(filaActual, 10).Value = item.FechaEntregaProveedorTexto;
                ws.Cell(filaActual, 11).Value = item.FechaRecepcionReingresoTexto;
                ws.Cell(filaActual, 12).Value = item.DiasCartaCompromiso;

                var rangoFila = ws.Range(filaActual, 1, filaActual, totalColumnas);
                rangoFila.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rangoFila.Style.Border.InsideBorder  = XLBorderStyleValues.Thin;
                rangoFila.Style.Alignment.Vertical   = XLAlignmentVerticalValues.Center;
                rangoFila.Style.Alignment.WrapText   = true;

                ws.Cell(filaActual, 8).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(filaActual, 9).Style.NumberFormat.Format = "#,##0.00";

                filaActual++;
            }

            ws.Column(1).Width  = 8;
            ws.Column(2).Width  = 18;
            ws.Column(3).Width  = 18;
            ws.Column(4).Width  = 18;
            ws.Column(5).Width  = 15;
            ws.Column(6).Width  = 22;
            ws.Column(7).Width  = 45;
            ws.Column(8).Width  = 15;
            ws.Column(9).Width  = 15;
            ws.Column(10).Width = 22;
            ws.Column(11).Width = 24;
            ws.Column(12).Width = 20;

            ws.Rows().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ListadoMedicamentosProximosVencer.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ExportarPdf()
        {
            var model = await ObtenerDatosAsync();
            return await _generatePdf.GetPdf("Views/MedicamentosProximosVencer/pdf.cshtml", model);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  ViewModel
    // ─────────────────────────────────────────────────────────────────────────
    public class MedicamentoProximoVencerItemViewModel
    {
        public int     Numero                       { get; set; }
        public string  NumeroFormulario             { get; set; }
        public string  FechaFormularioTexto         { get; set; }
        public string  Oficio                       { get; set; }
        public string  FechaTexto                   { get; set; }
        public string  Unidad                       { get; set; }
        public string  Descripcion                  { get; set; }
        public decimal PrecioUnitario               { get; set; }
        public decimal PrecioTotal                  { get; set; }
        public string  FechaEntregaProveedorTexto   { get; set; }
        public string  FechaRecepcionReingresoTexto { get; set; }
        public int     DiasCartaCompromiso          { get; set; }
    }
}