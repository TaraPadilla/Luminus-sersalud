using System;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wkhtmltopdf.NetCore;

namespace sistema.Controllers
{
    [Authorize]
    public class BresController : Controller
    {
        private readonly IGeneratePdf _generatePdf;

        public BresController(IGeneratePdf generatePdf)
        {
            _generatePdf = generatePdf;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ExportarExcel()
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("BRES FEBRERO");

            // =========================
            // CABECERA (quemada por ahora)
            // =========================
            ws.Cell(1, 1).Value = "BALANCE, REQUISICIÓN Y ENVÍO DE SUMINISTROS";
            ws.Cell(2, 1).Value = "MEDICAMENTOS";

            ws.Range(1, 1, 1, 22).Merge().Style
                .Font.SetBold()
                .Font.SetFontSize(12)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Range(2, 1, 2, 22).Merge().Style
                .Font.SetBold()
                .Font.SetFontSize(11)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell(4, 1).Value = "Unidad";
            ws.Cell(4, 3).Value = "Hospital Pedro de Bethancourt";
            ws.Cell(5, 1).Value = "Fecha de Elaboración:";
            ws.Cell(5, 3).Value = "01/02/2026";
            ws.Cell(6, 1).Value = "Cuantos servicios deben informar";
            ws.Cell(6, 3).Value = 3;
            ws.Cell(7, 1).Value = "Cuantos servicios informan";
            ws.Cell(7, 3).Value = 3;

            // Caja "NIVELES DE SEGURIDAD" (aprox)
            ws.Cell(4, 15).Value = "NIVELES DE\nSEGURIDAD";
            ws.Range(4, 15, 4, 16).Merge().Style
                .Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Cell(5, 15).Value = "Mínimo";
            ws.Cell(5, 16).Value = "Máximo";
            ws.Cell(6, 15).Value = 3;
            ws.Cell(6, 16).Value = 6;

            ws.Range(4, 15, 6, 16).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(4, 15, 6, 16).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Range(5, 15, 6, 16).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Caja Mes/Año (aprox)
            ws.Cell(4, 18).Value = "Mes";
            ws.Cell(4, 19).Value = "Año";
            ws.Cell(5, 18).Value = 3;
            ws.Cell(5, 19).Value = 2026;

            ws.Range(4, 18, 5, 19).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(4, 18, 5, 19).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Range(4, 18, 5, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // =========================
            // TABLA COMPLETA (1ra pestaña)
            // =========================
            const int headerRow = 9;

            // Columnas (A..V = 22 columnas)
            // Nota: el Excel original incluye separadores en blanco; aquí se representan con headers vacíos.
            ws.Cell(headerRow, 1).Value = "No.";
            ws.Cell(headerRow, 2).Value = "CODIGO DE\nFINANZAS";
            ws.Cell(headerRow, 3).Value = "CODIGO\nMINISTERIO";
            ws.Cell(headerRow, 4).Value = "INDICADOR";
            ws.Cell(headerRow, 5).Value = "ACTIVO/\nINACTIVO";
            ws.Cell(headerRow, 6).Value = "DESCRIPCION DE PRODUCTO";
            ws.Cell(headerRow, 7).Value = "DESCRIPCION DE PRODUCTO\nDEL MINISTERIO";
            ws.Cell(headerRow, 8).Value = "MEDIDA";
            ws.Cell(headerRow, 9).Value = "SALDO\nMES\nANTERIOR";
            ws.Cell(headerRow, 10).Value = "ENTRADA\nA NIVEL\nSUPERIOR";
            ws.Cell(headerRow, 11).Value = "ENTREGA\nUSUARIO";

            ws.Cell(headerRow, 12).Value = ""; // separador (en el Excel se ve como columna vacía)
            ws.Cell(headerRow, 13).Value = "REAJUSTES\n(+) (-)";
            ws.Cell(headerRow, 14).Value = "EXISTENCIA\nA";
            ws.Cell(headerRow, 15).Value = "EXISTENCIA";
            ws.Cell(headerRow, 16).Value = "PROMEDIO\nCONSUMO";
            ws.Cell(headerRow, 17).Value = "EXISTENC\nMES";
            ws.Cell(headerRow, 18).Value = "A\nSOLICITA\nR";

            ws.Cell(headerRow, 19).Value = ""; // separador
            ws.Cell(headerRow, 20).Value = "PRECIO\nUNITARIO";
            ws.Cell(headerRow, 21).Value = "TOTAL";
            ws.Cell(headerRow, 22).Value = "OBSERVACIONES";

            // Estilo header
            var headerRange = ws.Range(headerRow, 1, headerRow, 22);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            headerRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            headerRange.Style.Alignment.WrapText = true;
            headerRange.Style.Fill.SetBackgroundColor(XLColor.LightGray);
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Row(headerRow).Height = 35;

            // Helpers
            int row = headerRow + 1;

            void AddRow(
                int no,
                string codigoFinanzas,
                string codigoMinisterio,
                string indicador,
                string activoInactivo,
                string descripcionProducto,
                string descripcionProductoMinisterio,
                string medida,
                decimal saldoMesAnterior,
                decimal entradaNivelSuperior,
                decimal entregaUsuario,
                decimal reajustes,
                decimal promedioConsumo,
                decimal aSolicitar,
                decimal precioUnitario,
                string observaciones
            )
            {
                // En el Excel real hay doble existencia. Aquí:
                // Existencia A = saldo + entrada - entrega + reajustes
                // Existencia = misma por ahora (luego la ajustamos si tu Excel tiene lógica distinta)
                var existenciaA = saldoMesAnterior + entradaNivelSuperior - entregaUsuario + reajustes;
                var existencia = existenciaA;

                // Existencias mes = existencia / promedio (si promedio=0, 0)
                var existMes = promedioConsumo <= 0 ? 0 : Math.Round(existencia / promedioConsumo, 2);

                // Total = a solicitar * precio unitario
                var total = Math.Round(aSolicitar * precioUnitario, 2);

                ws.Cell(row, 1).Value = no;
                ws.Cell(row, 2).Value = codigoFinanzas;
                ws.Cell(row, 3).Value = codigoMinisterio;
                ws.Cell(row, 4).Value = indicador;
                ws.Cell(row, 5).Value = activoInactivo;
                ws.Cell(row, 6).Value = descripcionProducto;
                ws.Cell(row, 7).Value = descripcionProductoMinisterio;
                ws.Cell(row, 8).Value = medida;

                ws.Cell(row, 9).Value = saldoMesAnterior;
                ws.Cell(row, 10).Value = entradaNivelSuperior;
                ws.Cell(row, 11).Value = entregaUsuario;

                ws.Cell(row, 12).Value = ""; // separador

                ws.Cell(row, 13).Value = reajustes;
                ws.Cell(row, 14).Value = existenciaA;
                ws.Cell(row, 15).Value = existencia;
                ws.Cell(row, 16).Value = promedioConsumo;
                ws.Cell(row, 17).Value = existMes;
                ws.Cell(row, 18).Value = aSolicitar;

                ws.Cell(row, 19).Value = ""; // separador

                ws.Cell(row, 20).Value = precioUnitario;
                ws.Cell(row, 21).Value = total;
                ws.Cell(row, 22).Value = observaciones;

                // Bordes fila
                var r = ws.Range(row, 1, row, 22);
                r.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                r.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                r.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                r.Style.Alignment.WrapText = true;

                // Alineaciones numéricas
                ws.Range(row, 9, row, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range(row, 13, row, 18).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range(row, 20, row, 21).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                row++;
            }

            // =========================
            // DATOS QUEMADOS (mock)
            // =========================
            AddRow(
                no: 4,
                codigoFinanzas: "25",
                codigoMinisterio: "",
                indicador: "",
                activoInactivo: "Inactivo",
                descripcionProducto: "Aciclovir, Concentración: 200mg; Forma farmacéutica: Tableta; Vía de administración: Oral;",
                descripcionProductoMinisterio: "",
                medida: "Unidad/1\nUnidad(es)",
                saldoMesAnterior: 0,
                entradaNivelSuperior: 0,
                entregaUsuario: 0,
                reajustes: 0,
                promedioConsumo: 2,
                aSolicitar: 12,
                precioUnitario: 0.37m,
                observaciones: ""
            );

            AddRow(
                no: 3,
                codigoFinanzas: "59479",
                codigoMinisterio: "MX0007430101",
                indicador: "",
                activoInactivo: "Activo",
                descripcionProducto: "Acetaminofén (paracetamol), Concentración: 10mg/ml; Forma farmacéutica: solución inyectable; vía: intravenosa;",
                descripcionProductoMinisterio: "",
                medida: "Bols, 100\nMililitro",
                saldoMesAnterior: 0,
                entradaNivelSuperior: 0,
                entregaUsuario: 0,
                reajustes: 0,
                promedioConsumo: 2300,
                aSolicitar: 13800,
                precioUnitario: 18.10m,
                observaciones: ""
            );

            AddRow(
                no: 17,
                codigoFinanzas: "90480",
                codigoMinisterio: "MX0038430201",
                indicador: "",
                activoInactivo: "Activo",
                descripcionProducto: "Albúmina humana, Concentración: 20%; Forma farmacéutica: Solución inyectable; Vía: intravenosa;",
                descripcionProductoMinisterio: "",
                medida: "Vial, 50\nMililitro",
                saldoMesAnterior: 735,
                entradaNivelSuperior: 0,
                entregaUsuario: 680,
                reajustes: 55,
                promedioConsumo: 450,
                aSolicitar: 2645,
                precioUnitario: 375.00m,
                observaciones: ""
            );

            // Formatos numéricos (aprox)
            ws.Range(headerRow + 1, 9, row - 1, 18).Style.NumberFormat.Format = "#,##0.00";
            ws.Range(headerRow + 1, 20, row - 1, 21).Style.NumberFormat.Format = "\"Q\" #,##0.00";

            // Column widths (aprox, afinamos luego)
            ws.Column(1).Width = 4;   // No
            ws.Column(2).Width = 10;  // Finanzas
            ws.Column(3).Width = 14;  // Ministerio
            ws.Column(4).Width = 10;  // Indicador
            ws.Column(5).Width = 10;  // Activo/Inactivo
            ws.Column(6).Width = 45;  // Desc producto
            ws.Column(7).Width = 28;  // Desc ministerio
            ws.Column(8).Width = 10;  // Medida
            ws.Column(9).Width = 10;
            ws.Column(10).Width = 10;
            ws.Column(11).Width = 10;
            //ws.Column(12).Width = 2;  // separador
            ws.Column(13).Width = 10;
            ws.Column(14).Width = 10;
            ws.Column(15).Width = 10;
            ws.Column(16).Width = 10;
            ws.Column(17).Width = 10;
            ws.Column(18).Width = 10;
            ws.Column(19).Width = 5;  // separador
            ws.Column(20).Width = 10; // precio
            ws.Column(21).Width = 10; // total
            ws.Column(22).Width = 25; // observaciones

            ws.SheetView.FreezeRows(headerRow);

            // Guardar y devolver archivo
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            var fileName = $"BRES_FEBRERO_mock_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportarPdf()
        {
            // Renderiza la vista Razor como PDF (datos quemados en la vista por ahora)
            return await _generatePdf.GetPdf("Views/Bres/Pdf.cshtml");
        }
    }
}