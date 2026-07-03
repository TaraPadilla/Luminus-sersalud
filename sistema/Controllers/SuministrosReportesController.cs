using System;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace sistema.Controllers
{
    public class SuministrosReportesController : Controller
    {
        // 1. Vista principal que muestra el menú de reportes
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // 2. Acción que descarga el Reporte 1 (Lavandería Renglon 292-261)
        [HttpGet]
        public IActionResult ExportarExcelLavanderia()
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Informe Semanal");

            // =========================
            // CABECERA 
            // =========================
            ws.Cell(3, 1).Value = "INFORME";
            ws.Cell(4, 1).Value = "CORRESPONDIENTE A LA SEMANA DEL 12/03/2026";
            ws.Cell(5, 1).Value = "PRODUCTOS UTILIZADOS EN EL SERVICIO DE LAVANDERIA Y OTROS";

            // Estilos para la cabecera
            ws.Range(3, 1, 5, 1).Style.Font.SetBold();

            // =========================
            // PRODUCTO 1
            // =========================
            ws.Cell(7, 1).Value = "Reng. 292";
            ws.Cell(8, 1).Value = "DETERGENTE CLORADO AL 16%";
            ws.Range(7, 1, 8, 1).Style.Font.SetBold();

            ws.Cell(9, 1).Value = "CONSUMO MENSUAL APROX.";
            ws.Cell(9, 6).Value = 600;
            ws.Cell(9, 7).Value = "Lbs. / mes";

            ws.Cell(10, 1).Value = "SALDO ACTUAL";
            ws.Cell(10, 6).Value = 4500;
            ws.Cell(10, 7).Value = "Lbs.";

            ws.Cell(11, 1).Value = "EXISTENCIA APROX.";
            ws.Cell(11, 6).Value = 7.5;
            ws.Cell(11, 7).Value = "Meses.";

            // =========================
            // PRODUCTO 2
            // =========================
            ws.Cell(13, 1).Value = "Reng. 292";
            ws.Cell(14, 1).Value = "DETERGENTE ALKALINO EN POLVO";
            ws.Range(13, 1, 14, 1).Style.Font.SetBold();

            ws.Cell(15, 1).Value = "CONSUMO MENSUAL APROX.";
            ws.Cell(15, 6).Value = 1120;
            ws.Cell(15, 7).Value = "Lbs. / mes";

            ws.Cell(16, 1).Value = "SALDO ACTUAL";
            ws.Cell(16, 6).Value = 5000;
            ws.Cell(16, 7).Value = "Lbs.";

            ws.Cell(17, 1).Value = "EXISTENCIA APROX.";
            ws.Cell(17, 6).Value = 4.46;
            ws.Cell(17, 7).Value = "Meses.";

            // =========================
            // PRODUCTO 3
            // =========================
            ws.Cell(19, 1).Value = "Reng. 261";
            ws.Cell(20, 1).Value = "CLORO LIQUIDO";
            ws.Range(19, 1, 20, 1).Style.Font.SetBold();

            ws.Cell(21, 1).Value = "CONSUMO MENSUAL APROX.";
            ws.Cell(21, 6).Value = 123;
            ws.Cell(21, 7).Value = "GL. / mes";

            ws.Cell(22, 1).Value = "SALDO ACTUAL";
            ws.Cell(22, 6).Value = 602;
            ws.Cell(22, 7).Value = "GALON";

            ws.Cell(23, 1).Value = "EXISTENCIA APROX.";
            ws.Cell(23, 6).Value = 0; // Ejemplo vacío/cero
            ws.Cell(23, 7).Value = "Meses.";

            // Ajustar el ancho de las columnas para que se vea bien
            ws.Column(1).Width = 35;
            ws.Column(6).Width = 15;
            ws.Column(7).Width = 15;

            // Guardar y devolver archivo Excel
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            var fileName = $"Informe_Lavanderia_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public IActionResult ExportarMatrizHospital()
        {
            using var workbook = new XLWorkbook();

            // 1. U. Oficina
            GenerarHojaUOficina(workbook);

            // 2. GRUPO_1
            GenerarHojaGrupo1(workbook);

            // 3. Hilados y Telas
            GenerarHojaHiladosYTelas(workbook);

            // 4. Cuarta pestaña: Resumen Grupo 2 (Nueva conexión)
            GenerarHojaResumenGrupo2(workbook);

            // 5. Quinta pestaña: Elementos Quim y Comp. (Nueva conexión)
            GenerarHojaElementosQuimYComp(workbook);

            // 6. Sexta pestaña: Combustibles y lubricantes (Nueva conexión)
            GenerarHojaCombustiblesYLubricantes(workbook);

            // 7. Séptima pestaña: Otros Productos Quim Y Conexos (Nueva conexión)
            GenerarHojaOtrosQuim(workbook);

            GenerarHojaSanitariosYLimpieza(workbook);

            // 9. Novena pestaña: SUMA TOTAL (El gran cierre)
            GenerarHojaSumaTotal(workbook);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var content = ms.ToArray();

            var fileName = $"Matriz_Hospital_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private void GenerarHojaUOficina(XLWorkbook workbook)
        {
            // Creamos la hoja con el nombre exacto
            var ws = workbook.Worksheets.Add("U. Oficina");

            // ==========================================
            // 1. CABECERA PRINCIPAL
            // ==========================================
            ws.Cell("A1").Value = "INSUMOS_Utiles de oficina 241, 243, 244, 291.";
            ws.Cell("A1").Style.Font.SetBold();

            ws.Cell("A3").Value = "ESTABLECIMIENTO: Hospital Pedro de Bethancurt.";
            ws.Cell("A4").Value = "SERVICIO: Almacén de Materiales y Suministros";
            ws.Cell("A5").Value = "MES: Febrero 2026";
            ws.Cell("F5").Value = "FIRMA: ____________________________ Encargado";

            // ==========================================
            // 2. ENCABEZADOS DE LAS TABLAS (Filas 7 y 8)
            // ==========================================
            // --- Lado Izquierdo ---
            ws.Cell("A7").Value = "SERVICIO";
            ws.Cell("B7").Value = "SEXO";
            ws.Range("B7:C7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("D7").Value = "TOTAL";

            ws.Cell("B8").Value = "M";
            ws.Cell("C8").Value = "F";

            // --- Lado Derecho ---
            ws.Cell("F7").Value = "SERVICIO";
            ws.Cell("G7").Value = "SEXO";
            ws.Range("G7:H7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("I7").Value = "TOTAL";

            ws.Cell("G8").Value = "M";
            ws.Cell("H8").Value = "F";

            // Estilos de Bordes y Centrado para los Encabezados
            var headers = ws.Ranges("A7:D8, F7:I8");
            headers.Style.Font.SetBold();
            headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ==========================================
            // 3. DATOS - LADO IZQUIERDO (A partir de fila 9)
            // ==========================================
            int filaIzq = 9;

            ws.Cell(filaIzq, 1).Value = "MEDICINA INTERNA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. DE HOM."; ws.Cell(filaIzq, 4).Value = 1263.95m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. MUJE"; ws.Cell(filaIzq, 4).Value = 387.25m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "CIRUGIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. HOMB"; ws.Cell(filaIzq, 4).Value = 1253.20m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. MUJE"; ws.Cell(filaIzq, 4).Value = 80.00m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "HOSP. TRA/ORTO/H"; ws.Cell(filaIzq, 4).Value = 304.00m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "HOSP. TRA/ORTO/ M"; ws.Cell(filaIzq, 4).Value = 0.00m; filaIzq++;

            // ==========================================
            // 4. DATOS - LADO DERECHO (A partir de fila 9)
            // ==========================================
            int filaDer = 9;

            ws.Cell(filaDer, 6).Value = "GINECOLOGIA"; ws.Cell(filaDer, 6).Style.Font.SetBold(); ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "PLANIFICACION FAMILIAR"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;

            ws.Cell(filaDer, 6).Value = "CONSULTA TRAUMATOLOGIA"; ws.Cell(filaDer, 6).Style.Font.SetBold(); filaDer++;
            ws.Cell(filaDer, 6).Value = "CONSULTA TRA/ORTO GENERAL"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "PSIAQUITRIA"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "CONSULTA NO MEDICA"; ws.Cell(filaDer, 6).Style.Font.SetBold(); filaDer++;
            ws.Cell(filaDer, 6).Value = "Educacion y capacitacion"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "CONSULTA FISICA Y REAB."; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "CONSULTA PSICOLOGIA"; ws.Cell(filaDer, 9).Value = 250.75m; filaDer++;
            ws.Cell(filaDer, 6).Value = "ESTIMULACION TEMPRANA"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "CONSULTA ODONTOLOGIA"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;

            // (Ejemplo de valores fuertes del lado derecho)
            filaDer += 5; // Saltamos algunas líneas para el ejemplo
            ws.Cell(filaDer, 6).Value = "LABOR Y PARTOS"; ws.Cell(filaDer, 9).Value = 6079.85m; filaDer++;
            ws.Cell(filaDer, 6).Value = "BANCO DE LECHE MATERNA"; ws.Cell(filaDer, 9).Value = 473.70m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABORATORIO CLINICO"; ws.Cell(filaDer, 9).Value = 864.50m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LAB. PATOLOGIA"; ws.Cell(filaDer, 9).Value = 601.30m; filaDer++;
            ws.Cell(filaDer, 6).Value = "RAYOS X"; ws.Cell(filaDer, 9).Value = 412.50m; filaDer++;

            // ==========================================
            // 5. FORMATO FINAL (Ancho de columnas y números)
            // ==========================================
            ws.Column(1).Width = 35; // SERVICIO Izq
            ws.Column(2).Width = 5;  // M
            ws.Column(3).Width = 5;  // F
            ws.Column(4).Width = 15; // TOTAL Izq
            ws.Column(5).Width = 3;  // Espaciador (Columna E)
            ws.Column(6).Width = 35; // SERVICIO Der
            ws.Column(7).Width = 5;  // M
            ws.Column(8).Width = 5;  // F
            ws.Column(9).Width = 15; // TOTAL Der

            // Asegurar que toda la columna D e I (Totales) tengan formato decimal (Ej: 1,263.95)
            ws.Range("D9:D100").Style.NumberFormat.Format = "#,##0.00";
            ws.Range("I9:I100").Style.NumberFormat.Format = "#,##0.00";
        }

        private void GenerarHojaGrupo1(XLWorkbook workbook)
        {
            // Agregamos la hoja con el nombre exacto solicitado
            var ws = workbook.Worksheets.Add("GRUPO_1");

            // ==========================================
            // 1. CABECERA PRINCIPAL (Dato del CSV)
            // ==========================================
            ws.Cell("A1").Value = "INSUMOS_RESUMEN GRUPO 1: 122";
            ws.Cell("A1").Style.Font.SetBold();

            ws.Cell("A3").Value = "ESTABLECIMIENTO: Hospital Pedro de Bethancurt.";
            ws.Cell("A4").Value = "SERVICIO: Almacén de Materiales y Suministros";
            ws.Cell("A5").Value = "MES: Febrero 2026";
            ws.Cell("F5").Value = "FIRMA: ____________________________ Encargado";

            // ==========================================
            // 2. ENCABEZADOS DE LAS TABLAS (Idéntico a U. Oficina)
            // ==========================================
            // Lado Izquierdo
            ws.Cell("A7").Value = "SERVICIO";
            ws.Cell("B7").Value = "SEXO";
            ws.Range("B7:C7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("D7").Value = "TOTAL";
            ws.Cell("B8").Value = "M";
            ws.Cell("C8").Value = "F";

            // Lado Derecho
            ws.Cell("F7").Value = "SERVICIO";
            ws.Cell("G7").Value = "SEXO";
            ws.Range("G7:H7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("I7").Value = "TOTAL";
            ws.Cell("G8").Value = "M";
            ws.Cell("H8").Value = "F";

            var headers = ws.Ranges("A7:D8, F7:I8");
            headers.Style.Font.SetBold();
            headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ==========================================
            // 3. DATOS - LADO IZQUIERDO (Datos Reales GRUPO 1)
            // ==========================================
            int filaIzq = 9;

            ws.Cell(filaIzq, 1).Value = "MEDICINA INTERNA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. DE HOM."; ws.Cell(filaIzq, 4).Value = 355.08m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. MUJE"; ws.Cell(filaIzq, 4).Value = 253.55m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "CIRUGIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. HOMB"; ws.Cell(filaIzq, 4).Value = 629.98m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. MUJE"; ws.Cell(filaIzq, 4).Value = 457.99m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "OBSTETRICIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "OBSTETRICIA GENERAL"; ws.Cell(filaIzq, 4).Value = 438.40m; filaIzq++;

            // ==========================================
            // 4. DATOS - LADO DERECHO (Datos Reales GRUPO 1)
            // ==========================================
            int filaDer = 9;

            ws.Cell(filaDer, 6).Value = "EMERGENCIA DE TRAUMATOLOGIA"; ws.Cell(filaDer, 9).Value = 583.45m; filaDer++;
            ws.Cell(filaDer, 6).Value = "EMERGENCIA DE GINECCO-OBSTRICIA"; ws.Cell(filaDer, 9).Value = 390.145m; filaDer++;

            ws.Cell(filaDer, 6).Value = "UCIA ( ADULTOS)"; ws.Cell(filaDer, 9).Value = 299.46m; filaDer++;
            ws.Cell(filaDer, 6).Value = "ANESTESIOLOGIA"; ws.Cell(filaDer, 9).Value = 205.05m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABOR Y PARTOS"; ws.Cell(filaDer, 9).Value = 828.60m; filaDer++;

            // ==========================================
            // 5. FORMATO FINAL (Anchos y Números)
            // ==========================================
            ws.Column(1).Width = 35;
            ws.Column(2).Width = 5;
            ws.Column(3).Width = 5;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 3;  // Espaciador
            ws.Column(6).Width = 35;
            ws.Column(7).Width = 5;
            ws.Column(8).Width = 5;
            ws.Column(9).Width = 15;

            ws.Range("D9:D100").Style.NumberFormat.Format = "#,##0.00";
            ws.Range("I9:I100").Style.NumberFormat.Format = "#,##0.00";
        }

        private void GenerarHojaHiladosYTelas(XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("Hilados y Telas");

            // ==========================================
            // 1. CABECERA PRINCIPAL
            // ==========================================
            ws.Cell("A1").Value = "INSUMOS_HILADOS Y TELAS: 231, 232, 233, 235.";
            ws.Cell("A1").Style.Font.SetBold();

            ws.Cell("A3").Value = "ESTABLECIMIENTO: Hospital Pedro de Bethancurt.";
            ws.Cell("A4").Value = "SERVICIO: Almacén de Materiales y Suministros";
            ws.Cell("A5").Value = "MES: Febrero 2026";
            ws.Cell("F5").Value = "FIRMA: ____________________________ Encargado";

            // ==========================================
            // 2. ENCABEZADOS DE LAS TABLAS
            // ==========================================
            // Izquierda
            ws.Cell("A7").Value = "SERVICIO";
            ws.Cell("B7").Value = "SEXO";
            ws.Range("B7:C7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("D7").Value = "TOTAL";
            ws.Cell("B8").Value = "M";
            ws.Cell("C8").Value = "F";

            // Derecha
            ws.Cell("F7").Value = "SERVICIO";
            ws.Cell("G7").Value = "SEXO";
            ws.Range("G7:H7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("I7").Value = "TOTAL";
            ws.Cell("G8").Value = "M";
            ws.Cell("H8").Value = "F";

            var headers = ws.Ranges("A7:D8, F7:I8");
            headers.Style.Font.SetBold();
            headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ==========================================
            // 3. DATOS - LADO IZQUIERDO (Hilados y Telas)
            // ==========================================
            int filaIzq = 9;

            ws.Cell(filaIzq, 1).Value = "MEDICINA INTERNA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. DE HOM."; ws.Cell(filaIzq, 4).Value = 0.00m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. MUJE"; ws.Cell(filaIzq, 4).Value = 0.00m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "CIRUGIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. HOMB"; ws.Cell(filaIzq, 4).Value = 0.00m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. MUJE"; ws.Cell(filaIzq, 4).Value = 0.00m; filaIzq++;

            // ==========================================
            // 4. DATOS - LADO DERECHO (Hilados y Telas)
            // ==========================================
            int filaDer = 9;

            ws.Cell(filaDer, 6).Value = "GINECOLOGIA"; ws.Cell(filaDer, 6).Style.Font.SetBold(); filaDer++;
            ws.Cell(filaDer, 6).Value = "PLANIFICACION FAMILIAR"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;

            ws.Cell(filaDer, 6).Value = "EMERGENCIA DE TRAUMATOLOGIA"; ws.Cell(filaDer, 9).Value = 10.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "EMERGENCIA DE GINECCO-OBSTRICIA"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;

            ws.Cell(filaDer, 6).Value = "UCIA ( ADULTOS)"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;

            // ==========================================
            // 5. FORMATO FINAL
            // ==========================================
            ws.Column(1).Width = 35;
            ws.Column(2).Width = 5;
            ws.Column(3).Width = 5;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 3;
            ws.Column(6).Width = 35;
            ws.Column(7).Width = 5;
            ws.Column(8).Width = 5;
            ws.Column(9).Width = 15;

            ws.Range("D9:D100").Style.NumberFormat.Format = "#,##0.00";
            ws.Range("I9:I100").Style.NumberFormat.Format = "#,##0.00";
        }

        private void GenerarHojaResumenGrupo2(XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("Resumen Grupo 2");

            // ==========================================
            // 1. CABECERA PRINCIPAL (Título largo del CSV)
            // ==========================================
            ws.Cell("A1").Value = "INSUMOS_RESUMEN GRUPO 2: 239, 252, 267, 268, 283, 287, 297, 299.";
            ws.Cell("A1").Style.Font.SetBold();

            ws.Cell("A3").Value = "ESTABLECIMIENTO: Hospital Pedro de Bethancurt.";
            ws.Cell("A4").Value = "SERVICIO: Almacén de Materiales y Suministros";
            ws.Cell("A5").Value = "MES: Febrero 2026";
            ws.Cell("F5").Value = "FIRMA: ____________________________ Encargado";

            // ==========================================
            // 2. ENCABEZADOS DE LAS TABLAS
            // ==========================================
            // Estructura duplicada (Servicio, M, F, Total)
            ws.Cell("A7").Value = "SERVICIO";
            ws.Cell("B7").Value = "SEXO";
            ws.Range("B7:C7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("D7").Value = "TOTAL";
            ws.Cell("B8").Value = "M";
            ws.Cell("C8").Value = "F";

            ws.Cell("F7").Value = "SERVICIO";
            ws.Cell("G7").Value = "SEXO";
            ws.Range("G7:H7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("I7").Value = "TOTAL";
            ws.Cell("G8").Value = "M";
            ws.Cell("H8").Value = "F";

            var headers = ws.Ranges("A7:D8, F7:I8");
            headers.Style.Font.SetBold();
            headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ==========================================
            // 3. DATOS - LADO IZQUIERDO (Resumen Grupo 2)
            // ==========================================
            int filaIzq = 9;

            ws.Cell(filaIzq, 1).Value = "MEDICINA INTERNA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. DE HOM."; ws.Cell(filaIzq, 4).Value = 1783.00m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. MUJE"; ws.Cell(filaIzq, 4).Value = 1457.50m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "CIRUGIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. HOMB"; ws.Cell(filaIzq, 4).Value = 14163.00m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. MUJE"; ws.Cell(filaIzq, 4).Value = 878.00m; filaIzq++;

            // ==========================================
            // 4. DATOS - LADO DERECHO (Resumen Grupo 2)
            // ==========================================
            int filaDer = 9;

            ws.Cell(filaDer, 6).Value = "EMERGENCIA DE GINECCO-OBSTRICIA"; ws.Cell(filaDer, 9).Value = 589.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "UCIA ( ADULTOS)"; ws.Cell(filaDer, 9).Value = 1513.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "QUIROFANO GENERAL"; ws.Cell(filaDer, 9).Value = 1183.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABOR Y PARTOS"; ws.Cell(filaDer, 9).Value = 1600.50m; filaDer++;
            ws.Cell(filaDer, 6).Value = "BANCO DE LECHE MATERNA"; ws.Cell(filaDer, 9).Value = 210.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABORATORIO CLINICO"; ws.Cell(filaDer, 9).Value = 1463.00m; filaDer++;

            // ==========================================
            // 5. FORMATO FINAL
            // ==========================================
            ws.Column(1).Width = 35;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 3;
            ws.Column(6).Width = 35;
            ws.Column(9).Width = 15;

            // Formato de moneda/número con miles
            ws.Range("D9:D100").Style.NumberFormat.Format = "#,##0.00";
            ws.Range("I9:I100").Style.NumberFormat.Format = "#,##0.00";
        }

        private void GenerarHojaElementosQuimYComp(XLWorkbook workbook)
        {
            // Nombre de la hoja tal cual el requerimiento
            var ws = workbook.Worksheets.Add("Elementos Quim y Comp.");

            // ==========================================
            // 1. CABECERA PRINCIPAL
            // ==========================================
            ws.Cell("A1").Value = "INSUMOS_ELEMENTOS QUIMICOS Y COMPUESTOS: 261";
            ws.Cell("A1").Style.Font.SetBold();

            ws.Cell("A3").Value = "ESTABLECIMIENTO: Hospital Pedro de Bethancurt.";
            ws.Cell("A4").Value = "SERVICIO: Almacén de Materiales y Suministros";
            ws.Cell("A5").Value = "MES: Febrero 2026";
            ws.Cell("F5").Value = "FIRMA: ____________________________ Encargado";

            // ==========================================
            // 2. ENCABEZADOS DE LAS TABLAS
            // ==========================================
            // Lado Izquierdo
            ws.Cell("A7").Value = "SERVICIO";
            ws.Cell("B7").Value = "SEXO";
            ws.Range("B7:C7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("D7").Value = "TOTAL";
            ws.Cell("B8").Value = "M";
            ws.Cell("C8").Value = "F";

            // Lado Derecho
            ws.Cell("F7").Value = "SERVICIO";
            ws.Cell("G7").Value = "SEXO";
            ws.Range("G7:H7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("I7").Value = "TOTAL";
            ws.Cell("G8").Value = "M";
            ws.Cell("H8").Value = "F";

            var headers = ws.Ranges("A7:D8, F7:I8");
            headers.Style.Font.SetBold();
            headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ==========================================
            // 3. DATOS - LADO IZQUIERDO (Renglón 261)
            // ==========================================
            int filaIzq = 9;

            ws.Cell(filaIzq, 1).Value = "MEDICINA INTERNA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. DE HOM."; ws.Cell(filaIzq, 4).Value = 2190.50m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. MUJE"; ws.Cell(filaIzq, 4).Value = 1352.00m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "CIRUGIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. HOMB"; ws.Cell(filaIzq, 4).Value = 1042.50m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. MUJE"; ws.Cell(filaIzq, 4).Value = 780.00m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "OBSTETRICIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "OBSTETRICIA GENERAL"; ws.Cell(filaIzq, 4).Value = 312.00m; filaIzq++;

            // ==========================================
            // 4. DATOS - LADO DERECHO (Renglón 261)
            // ==========================================
            int filaDer = 9;

            ws.Cell(filaDer, 6).Value = "CONSULTA ODONTOLOGIA"; ws.Cell(filaDer, 9).Value = 318.50m; filaDer++;
            ws.Cell(filaDer, 6).Value = "EMERGENCIA DE TRAUMATOLOGIA"; ws.Cell(filaDer, 9).Value = 387.50m; filaDer++;
            ws.Cell(filaDer, 6).Value = "EMERGENCIA DE GINECCO-OBSTRICIA"; ws.Cell(filaDer, 9).Value = 134.75m; filaDer++;

            ws.Cell(filaDer, 6).Value = "ANESTESIOLOGIA"; ws.Cell(filaDer, 9).Value = 1080.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "QUIROFANO GENERAL"; ws.Cell(filaDer, 9).Value = 260.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABOR Y PARTOS"; ws.Cell(filaDer, 9).Value = 312.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "BANCO DE LECHE MATERNA"; ws.Cell(filaDer, 9).Value = 640.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABORATORIO CLINICO"; ws.Cell(filaDer, 9).Value = 810.00m; filaDer++;

            // ==========================================
            // 5. FORMATO FINAL
            // ==========================================
            ws.Column(1).Width = 35;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 3;
            ws.Column(6).Width = 35;
            ws.Column(9).Width = 15;

            ws.Range("D9:D100").Style.NumberFormat.Format = "#,##0.00";
            ws.Range("I9:I100").Style.NumberFormat.Format = "#,##0.00";
        }

        private void GenerarHojaCombustiblesYLubricantes(XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("Combustibles y lubricantes");

            // ==========================================
            // 1. CABECERA PRINCIPAL
            // ==========================================
            ws.Cell("A1").Value = "INSUMOS_COMBUSTIBLES Y LUBRICANTES: 262";
            ws.Cell("A1").Style.Font.SetBold();

            ws.Cell("A3").Value = "ESTABLECIMIENTO: Hospital Pedro de Bethancurt.";
            ws.Cell("A4").Value = "SERVICIO: Almacén de Materiales y Suministros";
            ws.Cell("A5").Value = "MES: Febrero 2026";
            ws.Cell("F5").Value = "FIRMA: ____________________________ Encargado";

            // ==========================================
            // 2. ENCABEZADOS DE LAS TABLAS
            // ==========================================
            // Estructura espejo (Izquierda y Derecha)
            ws.Cell("A7").Value = "SERVICIO";
            ws.Cell("B7").Value = "SEXO";
            ws.Range("B7:C7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("D7").Value = "TOTAL";
            ws.Cell("B8").Value = "M";
            ws.Cell("C8").Value = "F";

            ws.Cell("F7").Value = "SERVICIO";
            ws.Cell("G7").Value = "SEXO";
            ws.Range("G7:H7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("I7").Value = "TOTAL";
            ws.Cell("G8").Value = "M";
            ws.Cell("H8").Value = "F";

            var headers = ws.Ranges("A7:D8, F7:I8");
            headers.Style.Font.SetBold();
            headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ==========================================
            // 3. DATOS - LADO IZQUIERDO (Cifras en 0.00 según CSV)
            // ==========================================
            int filaIzq = 9;

            ws.Cell(filaIzq, 1).Value = "MEDICINA INTERNA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. DE HOM."; ws.Cell(filaIzq, 4).Value = 0.00m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. MUJE"; ws.Cell(filaIzq, 4).Value = 0.00m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "CIRUGIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. HOMB"; ws.Cell(filaIzq, 4).Value = 0.00m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. MUJE"; ws.Cell(filaIzq, 4).Value = 0.00m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "PEDIATRIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "PEDIATRIA GENERAL"; ws.Cell(filaIzq, 4).Value = 0.00m; filaIzq++;

            // ==========================================
            // 4. DATOS - LADO DERECHO (Cifras en 0.00 según CSV)
            // ==========================================
            int filaDer = 9;

            ws.Cell(filaDer, 6).Value = "GINECOLOGIA"; ws.Cell(filaDer, 6).Style.Font.SetBold(); filaDer++;
            ws.Cell(filaDer, 6).Value = "PLANIFICACION FAMILIAR"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;

            ws.Cell(filaDer, 6).Value = "EMERGENCIA GENERAL"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "ANESTESIOLOGIA"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABOR Y PARTOS"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "FARMACIA"; ws.Cell(filaDer, 9).Value = 0.00m; filaDer++;

            // ==========================================
            // 5. FORMATO FINALs
            // ==========================================
            ws.Column(1).Width = 35;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 3;
            ws.Column(6).Width = 35;
            ws.Column(9).Width = 15;

            ws.Range("D9:D100").Style.NumberFormat.Format = "#,##0.00";
            ws.Range("I9:I100").Style.NumberFormat.Format = "#,##0.00";
        }

        private void GenerarHojaOtrosQuim(XLWorkbook workbook)
        {
            // Excel tiene un límite de 31 caracteres para el nombre de la pestaña
            var ws = workbook.Worksheets.Add("Otros Productos Quim Y Conexos");

            // ==========================================
            // 1. CABECERA PRINCIPAL
            // ==========================================
            ws.Cell("A1").Value = "INSUMOS_OTROS PRODUCTOS QUIMICOS Y CONEXOS: 269";
            ws.Cell("A1").Style.Font.SetBold();

            ws.Cell("A3").Value = "ESTABLECIMIENTO: Hospital Pedro de Bethancurt.";
            ws.Cell("A4").Value = "SERVICIO: Almacén de Materiales y Suministros";
            ws.Cell("A5").Value = "MES: Febrero 2026";
            ws.Cell("F5").Value = "FIRMA: ____________________________ Encargado";

            // ==========================================
            // 2. ENCABEZADOS DE LAS TABLAS
            // ==========================================
            // Lado Izquierdo
            ws.Cell("A7").Value = "SERVICIO";
            ws.Cell("B7").Value = "SEXO";
            ws.Range("B7:C7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("D7").Value = "TOTAL";
            ws.Cell("B8").Value = "M";
            ws.Cell("C8").Value = "F";

            // Lado Derecho
            ws.Cell("F7").Value = "SERVICIO";
            ws.Cell("G7").Value = "SEXO";
            ws.Range("G7:H7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("I7").Value = "TOTAL";
            ws.Cell("G8").Value = "M";
            ws.Cell("H8").Value = "F";

            var headers = ws.Ranges("A7:D8, F7:I8");
            headers.Style.Font.SetBold();
            headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ==========================================
            // 3. DATOS - LADO IZQUIERDO (Renglón 269)
            // ==========================================
            int filaIzq = 9;

            ws.Cell(filaIzq, 1).Value = "MEDICINA INTERNA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. DE HOM."; ws.Cell(filaIzq, 4).Value = 196.00m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. MUJE"; ws.Cell(filaIzq, 4).Value = 441.00m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "CIRUGIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. MUJE"; ws.Cell(filaIzq, 4).Value = 73.50m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "OBSTETRICIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "OBSTETRICIA GENERAL"; ws.Cell(filaIzq, 4).Value = 171.50m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "UNIDAD DE ATENCION INTEGRAL"; ws.Cell(filaIzq, 4).Value = 73.50m; filaIzq++;

            // ==========================================
            // 4. DATOS - LADO DERECHO (Renglón 269)
            // ==========================================
            int filaDer = 9;

            ws.Cell(filaDer, 6).Value = "EMERGENCIA GENERAL"; ws.Cell(filaDer, 9).Value = 85.75m; filaDer++;
            ws.Cell(filaDer, 6).Value = "EMERGENCIA DE GINECCO-OBSTRICIA"; ws.Cell(filaDer, 9).Value = 134.75m; filaDer++;

            ws.Cell(filaDer, 6).Value = "UCIA ( ADULTOS)"; ws.Cell(filaDer, 9).Value = 147.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "QUIROFANO GENERAL"; ws.Cell(filaDer, 9).Value = 294.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABOR Y PARTOS"; ws.Cell(filaDer, 9).Value = 343.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABORATORIO CLINICO"; ws.Cell(filaDer, 9).Value = 196.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LAB. PATOLOGIA"; ws.Cell(filaDer, 9).Value = 49.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "RAYOS X"; ws.Cell(filaDer, 9).Value = 294.00m; filaDer++;

            // ==========================================
            // 5. FORMATO FINAL
            // ==========================================
            ws.Column(1).Width = 35;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 3;
            ws.Column(6).Width = 35;
            ws.Column(9).Width = 15;

            ws.Range("D9:D100").Style.NumberFormat.Format = "#,##0.00";
            ws.Range("I9:I100").Style.NumberFormat.Format = "#,##0.00";
        }

        private void GenerarHojaSanitariosYLimpieza(XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("Prod. Sanit. y Limp.");

            // ==========================================
            // 1. CABECERA PRINCIPAL
            // ==========================================
            ws.Cell("A1").Value = "INSUMOS_PRODUCTOS SANITARIOS Y DE LIMPIEZA: 292";
            ws.Cell("A1").Style.Font.SetBold();

            ws.Cell("A3").Value = "ESTABLECIMIENTO: Hospital Pedro de Bethancurt.";
            ws.Cell("A4").Value = "SERVICIO: Almacén de Materiales y Suministros";
            ws.Cell("A5").Value = "MES: Febrero 2026";
            ws.Cell("F5").Value = "FIRMA: ____________________________ Encargado";

            // ==========================================
            // 2. ENCABEZADOS DE LAS TABLAS
            // ==========================================
            ws.Cell("A7").Value = "SERVICIO";
            ws.Cell("B7").Value = "SEXO";
            ws.Range("B7:C7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("D7").Value = "TOTAL";
            ws.Cell("B8").Value = "M";
            ws.Cell("C8").Value = "F";

            ws.Cell("F7").Value = "SERVICIO";
            ws.Cell("G7").Value = "SEXO";
            ws.Range("G7:H7").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell("I7").Value = "TOTAL";
            ws.Cell("G8").Value = "M";
            ws.Cell("H8").Value = "F";

            var headers = ws.Ranges("A7:D8, F7:I8");
            headers.Style.Font.SetBold();
            headers.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headers.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // ==========================================
            // 3. DATOS - LADO IZQUIERDO (Sanitarios y Limpieza)
            // ==========================================
            int filaIzq = 9;

            ws.Cell(filaIzq, 1).Value = "MEDICINA INTERNA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. DE HOM."; ws.Cell(filaIzq, 4).Value = 3518.25m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA MED. MUJE"; ws.Cell(filaIzq, 4).Value = 4817.00m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "CIRUGIA"; ws.Cell(filaIzq, 1).Style.Font.SetBold(); filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. HOMB"; ws.Cell(filaIzq, 4).Value = 3790.00m; filaIzq++;
            ws.Cell(filaIzq, 1).Value = "SALA CIRUG. MUJE"; ws.Cell(filaIzq, 4).Value = 3433.30m; filaIzq++;

            ws.Cell(filaIzq, 1).Value = "UNIDAD DE ATENCION INTEGRAL"; ws.Cell(filaIzq, 4).Value = 314.50m; filaIzq++;

            // ==========================================
            // 4. DATOS - LADO DERECHO (Sanitarios y Limpieza)
            // ==========================================
            int filaDer = 9;

            ws.Cell(filaDer, 6).Value = "UCIA ( ADULTOS)"; ws.Cell(filaDer, 9).Value = 2850.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "QUIROFANO GENERAL"; ws.Cell(filaDer, 9).Value = 2174.75m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABOR Y PARTOS"; ws.Cell(filaDer, 9).Value = 4267.50m; filaDer++;
            ws.Cell(filaDer, 6).Value = "BANCO DE LECHE MATERNA"; ws.Cell(filaDer, 9).Value = 93.75m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LABORATORIO CLINICO"; ws.Cell(filaDer, 9).Value = 589.70m; filaDer++;
            ws.Cell(filaDer, 6).Value = "LAB. PATOLOGIA"; ws.Cell(filaDer, 9).Value = 118.00m; filaDer++;
            ws.Cell(filaDer, 6).Value = "RAYOS X"; ws.Cell(filaDer, 9).Value = 338.50m; filaDer++;
            ws.Cell(filaDer, 6).Value = "CENTRAL DE EQUIPO HOSPITAL"; ws.Cell(filaDer, 9).Value = 223.50m; filaDer++;

            // ==========================================
            // 5. FORMATO FINAL
            // ==========================================
            ws.Column(1).Width = 35;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 3;
            ws.Column(6).Width = 35;
            ws.Column(9).Width = 15;

            ws.Range("D9:D100").Style.NumberFormat.Format = "#,##0.00";
            ws.Range("I9:I100").Style.NumberFormat.Format = "#,##0.00";
        }

        private void GenerarHojaSumaTotal(XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("SUMA TOTAL");

            // ==========================================
            // 1. TÍTULO DE CONSOLIDACIÓN
            // ==========================================
            ws.Cell("A1").Value = "TOTAL DE SUMATORIA DE HOJAS";

            // Le damos un formato más grande y elegante por ser el final
            ws.Cell("A1").Style.Font.SetBold();
            ws.Cell("A1").Style.Font.SetFontSize(14);
            ws.Cell("A1").Style.Font.SetFontColor(XLColor.FromName("DarkBlue"));

            // ==========================================
            // 2. MONTO TOTAL (Dato real del CSV)
            // ==========================================
            // Dejamos unas filas de espacio como en tu Excel original
            ws.Cell("A6").Value = 833982.40m;

            // Formato numérico profesional con separador de miles
            ws.Cell("A6").Style.NumberFormat.Format = "#,##0.00";
            ws.Cell("A6").Style.Font.SetBold();
            ws.Cell("A6").Style.Font.SetFontSize(16);

            // Ajustamos la columna para que el número grande quepa bien
            ws.Column(1).Width = 35;
        }

    }
}

