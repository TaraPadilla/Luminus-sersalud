const GraficaVm = function () {
  var self = this;
  self.productosExistentes = ko.observableArray();
  self.productoSeleccionadoVentasMensuales = ko.observable();
  self.productoSeleccionadoComparacionPrecios = ko.observable();

  self.ventasMesSemDia = function () {
    var annio = $("#annio-ventas-generales").val() || new Date().getFullYear();
    $.ajax({
      url: "/Grafica/VentasGeneraLes",
      method: "POST",
      data: { annio: annio },
      success: function (data) {
        GraficaVentaGenerales(data.ventasMes, data.ventasDiaMes);
        //GraficaVentasTop();
        //ComprasGenerales(data.ventasMes, data.ventasDiaMes);
        //ComprasProveedor();
        //ComprasProveedorPastel();
        CostoOperativo();
        //ServiciosGenerales();
        //ServiciosGeneralesPorcentaje();
        GraficoDeIngresosPorLinea();
        GraficoDeNumeroExamenes();
        VentasPorDoctor();
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText); // Log any errors
      },
    });
  };


  self.topVentas = function () {
    var fechaRango = $("#fechaRangoTop10").val()
    $.ajax({
      url: "/Grafica/TabProductosMasVendidosServiciosSolicitudes",
      method: "POST",
      dataType: "json",
      data: { fechaRango: fechaRango },
      success: function (data) {
        GraficaVentasTop(data);
      },
      error: function (xhr, status, error) {
        console.error("Error en la petición:", error);
      }
    });
  };



  self.comprasGeneral = function () {
    var annio = $("#TabGeneralVentasComprasGananciasAnnio").val() || new Date().getFullYear();

    $.ajax({
      url: "/Grafica/TabGeneralCompras",
      method: "POST",
      dataType: "json",
      data: { annio: annio },
      success: function (data) {
        ComprasGenerales(data);
      },
      error: function (xhr, status, error) {
        console.error("Error en la petición:", error);
      }
    });
  };


  self.serviciosGenerales = function () {

    $.ajax({
      url: "/Grafica/PacientesAtendidosPorServicio",
      method: "POST",
      dataType: "json",
      success: function (data) {
        ServiciosGenerales(data);
        ServiciosGeneralesPorcentaje(data)
      },
      error: function (xhr, status, error) {
        console.error("Error en la petición:", error);
      }
    });
  };


  self.comprasProveedor = function () {
    $.ajax({
      url: "/Grafica/ComprasProveedor",
      method: "POST",
      dataType: "json",
      success: function (data) {
        ComprasProveedor(data);
        ComprasProveedorPastel(data);
      },
      error: function (xhr, status, error) {
        console.error("Error en la petición:", error);
      }
    });
  };


  self.tendenciasVentas = function () {
    var annio = $("#TabGeneralVentasComprasGananciasAnnio").val() || new Date().getFullYear();

    $.ajax({
      url: "/Grafica/TendenciasVentas",
      method: "POST",
      dataType: "json",
      data: { annio: annio },
      success: function (data) {
        TendenciasVentas(data.series, data.drilldown);
      },
      error: function (xhr, status, error) {
        console.error("Error en la petición:", error);
      }
    });
  };

  self.tiposPagos = function () {
    $.ajax({
      url: "/Grafica/PagosGenerales",
      method: "POST",
      data: {},
      success: function (data) {
        GraficaTiposPagos(data.pagos, data.pagosdia);
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText); // Log any errors
      },
    });
  };

  self.ingresos = function () {
    $.ajax({
      url: "/Grafica/IngresosGenerales",
      method: "POST",
      data: {},
      success: function (data) {
        GraficaIngresosCuentas(data.ingresos, data.ingresosDia);
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText); // Log any errors
      },
    });
  };

  self.ventaIngresos = function () {
    $.ajax({
      url: "/Grafica/IngresosVentasGenerales",
      method: "POST",
      data: {},
      success: function (data) {
        GraficaIngresosVentas(data.ingresosVentas, data.ingresosVentasDia);
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText); // Log any errors
      },
    });
  };

  self.gastos = function () {
    $.ajax({
      url: "/Grafica/GastosGenerales",
      method: "POST",
      data: {},
      success: function (data) {
        GraficaGastosCuentas(data.gastos, data.gastosDia);
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText); // Log any errors
      },
    });
  };


  self.ventasAnio = function () {
    var anio = $("#anio").val();
    $.ajax({
      url: "/Grafica/VentasAnnio",
      method: "POST",
      data: {
        annio: anio,
      },
      success: function (data) {
        GraficaAnnio(data);
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText);
      },
    });
  };


  self.ventasRango = function () {
    var fechaRango = $("#fechaRangoVentas").val();
    $.ajax({
      url: "/Grafica/VentasRango",
      method: "POST",
      data: {
        fechaRango: fechaRango,
      },
      success: function (data) {

        GraficaFechaRango(data);
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText); // Log any errors
      },
    });
  };


  self.gastos = function () {
    $.ajax({
      url: "/Grafica/GastosGenerales",
      method: "POST",
      data: {},
      success: function (data) {
        GraficaGastosCuentas(data.gastos, data.gastosDia);
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText); // Log any errors
      },
    });
  };
  self.consultarProductos = function () {
    showLoading();
    self.productosExistentes([]);
    $.ajax({
      url: "/Grafica/ConsultarProductos",
      method: "POST",
      data: {
        ambienteId: null,
      },
      success: function (result) {
        hideLoading();
        let data = JSON.parse(result);
        if (data.Exitoso) {
          let productoIds = new Set();
          $(data.Resultado).each(function (idx, vl) {
            productoIds.add(vl.ProductoId);
          });
          for (let productoId of productoIds) {
            let agregado = false;
            $(data.Resultado).each(function (idx, vl) {
              if (vl.ProductoId == productoId && !agregado) {
                self.productosExistentes.push(vl);
                agregado = true;
              }
            });
          }
        } else {
          mensajeEmergenteError("Error al consultar productos");
        }
      },
      error: function (resultError) {
        hideLoading();
        mensajeEmergenteError("Error de conexion al consultar productos");
      },
    });
  };
  self.gastosCompras = function () {
    $.ajax({
      url: "/Grafica/GastosComprasGenerales",
      method: "POST",
      data: {},
      success: function (data) {
        GraficaGastosCompras(data.gastosCompras, data.gastosComprasDia);
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText); // Log any errors
      },
    });
  };

  //#region TAB GENERAL

  self.tabGeneralVentasComprasGanancias = function () {
    showLoading();
    $.ajax({
      url: "/Grafica/TabGeneralComprasVentasGanancias",
      method: "POST",
      data: {
        annio: $("#TabGeneralVentasComprasGananciasAnnio").val(),
      },
      success: function (result) {
        let data = JSON.parse(result);
        if (data.Exitoso) {
          TabGeneralGraficaVentasComprasGanancias(data.series, data.drilldown);
          hideLoading();
        } else {
          mensajeEmergenteError(data.Mensaje);
          hideLoading();
        }
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText);
        hideLoading();
      },
    });
  };

  //#endregion

  //#region TAB PRODUCTOS

  self.tabProductosVentasMensuales = function () {
    showLoading();
    $.ajax({
      url: "/Grafica/TabProductosVentasMensuales",
      method: "POST",
      data: {
        annio: $("#annio-producto-ventas-mensuales").val(),
        productoId: self.productoSeleccionadoVentasMensuales().ProductoId,
      },
      success: function (result) {
        let data = JSON.parse(result);
        if (data.Exitoso) {
          TabProductosVentasMensuales(data.series, data.drilldown);
          hideLoading();
        } else {
          mensajeEmergenteError(data.Mensaje);
          hideLoading();
        }
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText);
        hideLoading();
      },
    });
  };
  self.tabProductosComparacionPreciosProveedor = function () {
    showLoading();
    $.ajax({
      url: "/Grafica/TabProductosComparacionPreciosProveedor",
      method: "POST",
      data: {
        productoId: self.productoSeleccionadoComparacionPrecios().ProductoId,
      },
      success: function (result) {
        let data = JSON.parse(result);
        if (data.Exitoso) {
          TabProductosComparacionPreciosProveedor(data.series, data.drilldown);
          hideLoading();
        } else {
          mensajeEmergenteError(data.Mensaje);
          hideLoading();
        }
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText);
        hideLoading();
      },
    });
  };
  self.tabProductosMasVendidos = function () {
    showLoading();
    $.ajax({
      url: "/Grafica/TabProductosMasVendidos",
      method: "POST",
      data: {
        ambienteId: null,
      },
      success: function (result) {
        let data = JSON.parse(result);
        if (data.Exitoso) {
          TabProductosMasVendidos(data.series);
          hideLoading();
        } else {
          mensajeEmergenteError(data.Mensaje);
          hideLoading();
        }
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText);
        hideLoading();
      },
    });
  };
  self.tabProductosMenosVendidos = function () {
    showLoading();
    $.ajax({
      url: "/Grafica/TabProductosMenosVendidos",
      method: "POST",
      data: {
        ambienteId: null,
      },
      success: function (result) {
        let data = JSON.parse(result);
        if (data.Exitoso) {
          TabProductosMenosVendidos(data.series);
          hideLoading();
        } else {
          mensajeEmergenteError(data.Mensaje);
          hideLoading();
        }
      },
      error: function (xhr, status, error) {
        console.error(xhr.responseText);
        hideLoading();
      },
    });
  };

  //#endregion
};
var graficaVm = new GraficaVm();
ko.applyBindings(graficaVm);

function CopiarGraficoGeneralAVentasGenerales(reintentos = 10) {
  const chartGeneral = Highcharts.charts.find(
    (c) => c && c.renderTo && c.renderTo.id === "tab-general-grafica-ventas-compras-ganancias"
  );

  if (chartGeneral) {
    const serieVentas = chartGeneral.series.find(
      (s) => s.name && s.name.toLowerCase().includes("venta")
    );

    if (serieVentas && serieVentas.options.data) {

      const cleanData = JSON.parse(JSON.stringify(serieVentas.options.data));
      const drilldownSeries = chartGeneral.options.drilldown ?
        JSON.parse(JSON.stringify(chartGeneral.options.drilldown.series)) : [];

      Highcharts.chart("graficaMesSemDia", {
        chart: { type: "column" },
        title: { text: "Ventas Generales" },
        lang: {
          decimalPoint: ',',
          thousandsSep: '.'
        },
        xAxis: {
          type: 'category',
          labels: {
            style: {
              color: '#0000ed',
              fontWeight: 'bold',
              textDecoration: 'underline'
            }
          }
        },
        yAxis: {
          title: { text: "Valor (Q)" },
          labels: {
            formatter: function () {
              return this.value >= 1000 ? (this.value / 1000) + 'k' : this.value;
            }
          }
        },
        plotOptions: {
          column: {
            dataLabels: {
              enabled: true,
              format: '{point.y:,.1f}',
              style: {
                color: '#0000ed',
                fontWeight: 'bold',
                fontSize: '11px'
              }
            },
            borderWidth: 0
          }
        },
        series: [{
          name: serieVentas.name,
          colorByPoint: false,
          color: serieVentas.color || "#33adff",
          data: cleanData,
          drilldown: serieVentas.options.drilldown || null
        }],
        drilldown: {
          activeAxisLabelStyle: {
            color: '#0000ed',
            textDecoration: 'underline',
            fontWeight: 'bold'
          },
          series: drilldownSeries
        },
        legend: { enabled: false },
        credits: { enabled: false }
      });

      return;
    }
  }

  if (reintentos > 0) {
    setTimeout(() => CopiarGraficoGeneralAVentasGenerales(reintentos - 1), 500);
  }
}

// Llama a la función al hacer clic en el subtab
$("a[href='#tab-ventas']").on("click", function () {
  CopiarGraficoGeneralAVentasGenerales();
});
// === FIN: Copiar solo la serie de Ventas de General a Ventas Generales ===
$(document).ready(function () {
  $("#subtabs-ventas").tabs();
  $("#subtabs-general").tabs();
  $("#subtabs-productos").tabs();

  graficaVm.ventasMesSemDia();
  graficaVm.ingresos();
  graficaVm.ventaIngresos();
  graficaVm.gastos();
  graficaVm.gastosCompras();
  graficaVm.tiposPagos();
  graficaVm.tendenciasVentas();

  graficaVm.topVentas();
  graficaVm.comprasProveedor();
  graficaVm.comprasGeneral();


  graficaVm.serviciosGenerales();

  graficaVm.consultarProductos();
  graficaVm.tabGeneralVentasComprasGanancias();

  // $("#TabGeneralVentasComprasGananciasAnnio").on(
  //   "change",
  //   graficaVm.tabGeneralVentasComprasGanancias

  // );

  $("#TabGeneralVentasComprasGananciasAnnio").on("change", function () {
    // 1. Ejecuta la función original (actualiza datos generales)
    graficaVm.tabGeneralVentasComprasGanancias();

    graficaVm.tendenciasVentas()

    graficaVm.comprasGeneral()


    console.log("Año actualizado y funciones ejecutadas.");
  });


  $("#fechaRangoVentas").on("change", graficaVm.ventasRango);
  $("#anio").on("change", graficaVm.ventasAnio);
  $("#fechaRangoTop10").on("change", graficaVm.topVentas);


  // window.location.href = "/Grafica/VentasRangoFechaAnio?fechaRango=" +
  // $("#fechaRangoVentas").val();
  document.getElementById("enlace-general").click();

  $(function () {
    $("#fechaRangoVentas").daterangepicker({
      timePicker: true,
      timePickerIncrement: 30,
      locale: {
        format: "DD/MM/YYYY",
      },
    });

    // $("#fechaRangoTop10").daterangepicker({
    //   timePicker: true,
    //   timePickerIncrement: 30,
    //   locale: {
    //     format: "DD/MM/YYYY",
    //   },
    // });

    $("#fechaRangoTop10").daterangepicker({
      autoUpdateInput: false, // Mantiene el input vacío al inicio
      timePicker: true,
      timePickerIncrement: 30,
      locale: {
        format: "DD/MM/YYYY",
        cancelLabel: 'Limpiar'
      },
    }).on('apply.daterangepicker', function (ev, picker) {
      // Cuando el usuario elige fechas en el picker:
      $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
      $(this).trigger('change');
    }).on('cancel.daterangepicker', function (ev, picker) {
      $(this).val('');
      $(this).trigger('change');
    });

    $("#fechaRangoTop10").attr("readonly", true);
  });

  $("#annio-ventas-generales").on("change", function () {
    graficaVm.ventasMesSemDia();
  });

  // Llamar al copiar gráfico cuando se active el subtab de Ventas Generales
  $("a[href='#subtab-ventas-ventas-generales']").on("click", function () {
    setTimeout(CopiarGraficoGeneralAVentasGenerales, 200); // Espera a que el tab esté visible
  });
});

// $("#anio").on("change", function () {
//   window.location.href =
//     "/Grafica/VentasRangoFechaAnio?anio=" + $("#anio").val();
// });

function mostrarContenido(idContenido) {
  // Ocultar todos los contenidos
  var contenidos = document.querySelectorAll(
    "#contenedor-tablas > .contenedor"
  );
  contenidos.forEach(function (elemento) {
    elemento.style.display = "none";
  });

  // Mostrar el contenido correspondiente al ID
  var contenidoMostrar = document.getElementById("contenido-" + idContenido);
  if (contenidoMostrar) {
    contenidoMostrar.style.display = "block";
  }
}

//Funcion para generar graficas  de ventas  generales
function GraficaVentaGenerales(data, data1) {
  // Primera gr�fica de ventas por mes

  //Highcharts.chart('graficaMesSemDia', {
  //    chart: {
  //        type: 'column'
  //    },
  //    title: {
  //        text: 'Ventas Diarias por Categor�a'
  //    },
  //    xAxis: {
  //        categories: ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30', '31']
  //    },
  //    yAxis: {
  //        title: {
  //            text: 'Ventas'
  //        }
  //    },
  //    series: [{
  //        name: 'Cl�nica',
  //        data: [100, 150, 120, 180, 200, 160, 140, 190, 210, 220, 190, 180, 170, 200, 220, 230, 240, 250, 210, 200, 190, 180, 170, 160, 150, 140, 130, 120, 110, 100, 90]
  //    }, {
  //        name: 'Farmacia',
  //        data: [80, 90, 70, 100, 110, 90, 80, 120, 130, 140, 120, 110, 100, 130, 140, 150, 160, 170, 140, 130, 120, 110, 100, 90, 80, 70, 60, 50, 40, 30, 20]
  //    }, {
  //        name: 'Laboratorio',
  //        data: [50, 60, 40, 70, 80, 60, 50, 90, 100, 110, 90, 80, 70, 100, 110, 120, 130, 140, 110, 100, 90, 80, 70, 60, 50, 40, 30, 20, 10, 5, 0]
  //    }],
  //    plotOptions: {
  //        column: {
  //            stacking: 'normal'
  //        }
  //    }
  //});
  Highcharts.chart("graficaMesSemDia", {
    chart: {
      type: "column",
    },
    title: {
      text: " ",
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Ventas",
      },
    },
    legend: {
      enabled: false,
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y:.1f}",
        },
      },
    },
    series: [
      {
        name: "Ventas",
        colorByPoint: true,
        data: data,
      },
    ],
    drilldown: {
      series: data1,
    },
  });
}

function GraficaTiposPagos(data, data1) {
  Highcharts.chart("graficaTiposPagos", {
    chart: {
      type: "column",
    },
    title: {
      text: " ",
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Pagos",
      },
    },
    legend: {
      enabled: false,
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y:.1f}",
        },
      },
    },
    series: [
      {
        name: "Pagos",
        colorByPoint: true,
        data: data,
      },
    ],
    drilldown: {
      series: data1,
    },
  });
}

function GraficaIngresosCuentas(data, data1) {
  Highcharts.chart("graficaIngresos", {
    chart: {
      type: "column",
    },
    title: {
      text: " ",
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Ingresos",
      },
    },
    legend: {
      enabled: false,
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y:.1f}",
        },
      },
    },
    series: [
      {
        name: "Ingresos",
        colorByPoint: true,
        data: data,
      },
    ],
    drilldown: {
      series: data1,
    },
  });
}

function GraficaIngresosVentas(data, data1) {
  Highcharts.chart("graficaIngresosVentas", {
    chart: {
      type: "column",
    },
    title: {
      text: " ",
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Ingresos",
      },
    },
    legend: {
      enabled: false,
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y:.1f}",
        },
      },
    },
    series: [
      {
        name: "Ingresos",
        colorByPoint: true,
        data: data,
      },
    ],
    drilldown: {
      series: data1,
    },
  });
}
function GraficaGastosCuentas(data, data1) {
  Highcharts.chart("graficaGastos", {
    chart: {
      type: "column",
    },
    title: {
      text: " ",
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Egresos",
      },
    },
    legend: {
      enabled: false,
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y:.1f}",
        },
      },
    },
    series: [
      {
        name: "Egresos",
        colorByPoint: true,
        data: data,
      },
    ],
    drilldown: {
      series: data1,
    },
  });
}

function GraficaGastosCompras(data, data1) {
  Highcharts.chart("graficaGastosCompras", {
    chart: {
      type: "column",
    },
    title: {
      text: " ",
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Egresos",
      },
    },
    legend: {
      enabled: false,
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y:.1f}",
        },
      },
    },
    series: [
      {
        name: "Egresos",
        colorByPoint: true,
        data: data,
      },
    ],
    drilldown: {
      series: data1,
    },
  });
}



function TendenciasVentas(series, drilldown) {
  let totalesPorCategoria = {
    "Hospital": 0,
    "Clinica": 0,
    "Farmacia": 0,
    "Laboratorio": 0
  };

  drilldown.forEach(serie => {
    if (serie.data && serie.id.includes("-dia-")) {
      serie.data.forEach(punto => {
        if (totalesPorCategoria.hasOwnProperty(punto.name)) {
          totalesPorCategoria[punto.name] += (punto.y || 0);
        }
      });
    }
  });

  const dataFinalCategorias = Object.keys(totalesPorCategoria).map(cat => {
    return {
      name: cat,
      y: totalesPorCategoria[cat]
    };
  });

  Highcharts.chart("graficaTendencias", {
    chart: {
      type: "line"
    },
    title: { text: "" },
    lang: {
      decimalPoint: ',',
      thousandsSep: '.'
    },
    xAxis: {
      categories: ["Hospital", "Clinica", "Farmacia", "Laboratorio"],
      labels: {
        style: { color: '#333', fontWeight: 'bold' }
      }
    },
    yAxis: {
      title: { text: "Ventas" },
      labels: { format: '{value:,.0f}' }
    },
    tooltip: {
      headerFormat: '<b>{point.key}</b><br/>',
      pointFormat: 'Total Anual: {point.y:,.0f} Q'
    },
    plotOptions: {
      line: {
        dataLabels: {
          enabled: true,
          format: '{point.y:,.0f}',
          style: { color: '#0000ed', fontWeight: 'bold' }
        },
        marker: {
          enabled: true,
          radius: 5,
          fillColor: '#33adff'
        },
        color: '#33adff' // Color azul de la línea
      }
    },
    series: [{
      name: "Valor Q",
      data: dataFinalCategorias
    }],
    credits: { enabled: false }
  });
}


function GraficaAnnio(jsonRaw) {
  const data = typeof jsonRaw === "string" ? JSON.parse(jsonRaw) : jsonRaw;
  if (!data.Exitoso) return;

  Highcharts.chart("graficaAnnio", {
    chart: {
      type: "column"
    },
    title: {
      text: " "
    },
    xAxis: {
      type: "category"
    },
    yAxis: {
      title: {
        text: "Ventas"
      }
    },
    legend: {
      enabled: false
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y:,.2f}"
        }
      }
    },
    series: data.series, // Inicia con "Ventas 2026" -> "Total Anual"
    drilldown: {
      activeAxisLabelStyle: { textDecoration: 'none', color: '#0033ff' },
      activeDataLabelStyle: { textDecoration: 'none', color: '#0033ff' },

      breadcrumbs: {
        enabled: true,
        separator: {
          text: ' / ',
          style: { color: '#666', fontSize: '12px' }
        },
        buttonTheme: {
          fill: 'none',
          stroke: 'none',
          style: {
            color: '#3366ff', //  tipo link
            fontSize: '13px'
          },
          states: {
            hover: {
              fill: '#eeeeee', // Fondo gris en hover
              style: {
                color: '#0033ff',
                fontWeight: 'bold'
              }
            }
          }
        },
        style: {
          color: '#000000',
          fontWeight: 'bold'
        }
      },
      series: data.drilldown
    }
  });
}

function GraficaFechaRango(jsonRaw) {
  const data = typeof jsonRaw === "string" ? JSON.parse(jsonRaw) : jsonRaw;
  if (!data.Exitoso) return;

  Highcharts.chart("graficaFechaRango", {
    chart: { type: "column" },
    title: { text: " " },
    xAxis: { type: "category" },
    yAxis: { title: { text: "Ventas" } },
    legend: { enabled: false },

    drilldown: {
      activeAxisLabelStyle: { textDecoration: 'none', color: '#0033ff' },
      activeDataLabelStyle: { textDecoration: 'none', color: '#0033ff' },

      breadcrumbs: {
        enabled: true,
        separator: {
          text: ' / ',
          style: { color: '#666', fontSize: '12px' }
        },
        buttonTheme: {
          fill: 'none',
          stroke: 'none',
          'stroke-width': 0,
          style: {
            color: '#3366ff',
            fontSize: '13px',
            fontWeight: 'normal'
          },
          states: {
            hover: {
              fill: '#eeeeee',
              style: {
                color: '#0033ff',
                fontWeight: 'bold'
              }
            },
            select: {
              fill: 'none',
              style: {
                color: '#000000',
                fontWeight: 'bold'
              }
            }
          }
        },
        style: {
          color: '#000000',
          fontWeight: 'bold'
        }
      },
      series: data.drilldown
    },

    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y:,.2f}"
        }
      }
    },
    series: data.series
  });
}

function TabGeneralGraficaVentasComprasGanancias(series, drilldown) {
  Highcharts.chart("tab-general-grafica-ventas-compras-ganancias", {
    chart: {
      type: "column",
    },
    title: {
      text:
        "Ventas - Compras - Ganancias (A&ntilde;o " +
        $("#TabGeneralVentasComprasGananciasAnnio").val() +
        ")",
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Valor (Q)",
      },
    },
    legend: {
      enabled: false,
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y:.1f}",
        },
      },
    },
    series: series,
    drilldown: {
      series: drilldown,
    },
  });
}


function GraficaVentasTop(dataBackend) {
  Highcharts.chart("ventasTop", {
    chart: {
      type: "bar",
      height: 800,
      style: { fontFamily: 'Roboto, Arial, sans-serif' }
    },
    title: {
      text: null
    },
    xAxis: {
      type: "category",
      uniqueNames: true
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        grouping: false,
        dataLabels: { enabled: true },
        events: {
          legendItemClick: function () {
            const chart = this.chart;
            const clickedSeries = this;

            if (clickedSeries.visible) {
              return false;
            }

            chart.series.forEach(s => {
              if (s !== clickedSeries) {
                s.hide();
              }
            });

            clickedSeries.show();

            chart.xAxis[0].update({
              categories: clickedSeries.options.data.map(d => d.name)
            });

            return false;
          }
        }
      }
    },
    series: dataBackend.series.map((s, index) => ({
      name: s.name,
      visible: index === 0,
      color: s.color || obtenerColorPorCategoria(s.name),
      data: s.data.map(d => ({
        name: d.name,
        y: d.y,
        drilldown: d.drilldown 
      }))
    })),
    drilldown: {
      activeAxisLabelStyle: { textDecoration: 'none', color: '#003399' },
      series: dataBackend.drilldown 
    }
  });
}


function obtenerColorPorCategoria(nombre) {
  if (nombre.includes("Productos")) return "#7cb5ec";
  if (nombre.includes("Servicios")) return "#434348";
  if (nombre.includes("Laboratorio")) return "#f7a35c";
  if (nombre.includes("Radiología")) return "#90ed7d";
  return "#e4d354";
}

function VentasPorDoctor() {
  Highcharts.chart("ventasPorDotor", {
    chart: {
      type: "column", // Tipo de gr�fico column para barras verticales
    },
    title: {
      text: " ",
    },
    xAxis: {
      categories: [
        "Dr. Smith",
        "Dr. Johnson",
        "Dr. Garcia",
        "Dr. Lee",
        "Dr. Patel",
        "Total",
      ],
    },
    yAxis: {
      title: {
        text: "Ingresos (en $)",
      },
    },
    plotOptions: {
      column: {
        dataLabels: {
          enabled: true,
          format: "${y}", // Muestra el valor de y (ingresos) en formato de d�lares
        },
        colorByPoint: false, // Desactiva colores por punto
        color: "rgba(119, 152, 191, .5)", // Color �nico para todas las barras
      },
    },
    series: [
      {
        name: "Ingresos",
        data: [120000, 135000, 110000, 125000, 140000, 630000], // Datos inventados
      },
    ],
  });
}

function ComprasGenerales(dataBackend) {
  const mainData = dataBackend.series[0].data.map(item => ({
    name: item.name,
    y: item.y,
    drilldown: item.drilldown
  }));

  Highcharts.chart("graficaCompras", {
    chart: {
      type: "column",
    },
    title: {
      align: "left",
      text: "Compras",
    },
    subtitle: {
      align: "left",
      text: "Haga clic en las columnas para observar el detalle",
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Total venta Generales",
      },
    },
    legend: {
      enabled: false,
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y:.2f}",
        },
      },
    },
    tooltip: {
      headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
      pointFormat:
        '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}</b><br/>',
    },
    series: [
      {
        name: "Compras",
        colorByPoint: true,
        data: mainData,
      },
    ],
    drilldown: {
      breadcrumbs: {
        position: {
          align: "right",
        },
      },
      series: dataBackend.drilldown.map(ds => ({
        id: ds.id,
        name: ds.name,
        data: ds.data.map(d => ({
          name: d.name,
          y: d.y,
          drilldown: d.drilldown
        }))
      })),
    },
  });
}

function CostoOperativo() {
  Highcharts.chart("graficaCostoOperativo", {
    title: {
      text: "  ",
    },
    xAxis: {
      categories: [
        "Enero",
        "Febrero",
        "Marzo",
        "Abril",
        "Mayo",
        "Junio",
        "Julio",
        "Agosto",
        "Septiembre",
        "Octubre",
        "Noviembre",
        "Diciembre",
      ],
    },
    yAxis: {
      title: {
        text: "Costo operativo",
      },
    },
    series: [
      {
        name: "Costo operativo",
        data: [
          50000, 55000, 60000, 58000, 62000, 61000, 63000, 64000, 65000, 62000,
          63000, 66000,
        ],
      },
    ],
  });
}

function ComprasProveedor(data) {
  if (!data || !data.Exitoso) {
    console.error("Error en la data:", data ? data.Mensaje : "No hay respuesta");
    return;
  }

  Highcharts.chart("graficaComprasProveedor", {
    chart: {
      type: "bar",
    },
    title: {
      text: " ",
    },
    xAxis: {
      type: "category",
      labels: {
        style: {
          fontSize: '12px'
        }
      }
    },
    yAxis: {
      min: 0,
      max: 8000,
      tickInterval: 1000,
      title: {
        text: "Monto de Compras",
      },
      labels: {
        formatter: function () {
          return (this.value / 1000) + "k";
        }
      }
    },
    legend: {
      enabled: true,
    },
    plotOptions: {
      bar: {
        color: '#33B5FF',
        dataLabels: {
          enabled: false
        }
      }
    },
    tooltip: {
      shared: true,
      pointFormat: '<span style="color:{point.color}">\u25CF</span> {series.name}: <b>{point.y}</b><br/>'
    },
    series: data.series
  });
}

function ComprasProveedorPastel(data) {
  if (!data || !data.Exitoso || !data.series || data.series.length === 0) {
    console.error("No hay datos disponibles para la gráfica de pastel");
    return;
  }

  Highcharts.chart("graficaComprasProveedorPastel", {
    chart: {
      type: "pie",
    },
    title: {
      text: " ",
    },
    tooltip: {
      pointFormat: "{series.name}: <b>{point.percentage:.1f}%</b> - {point.y}",
    },
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: true,
          format: "<b>{point.name}</b>: {point.percentage:.1f}%",
          style: {
            fontSize: '12px'
          }
        },
        showInLegend: true
      },
    },
    series: [
      {
        name: "Compras",
        colorByPoint: true,
        data: data.series[0].data
      },
    ],
  });
}

function TabProductosVentasMensuales(series, drilldown) {
  Highcharts.chart("graficaProductoVentas", {
    chart: {
      type: "line",
    },
    title: {
      text: null,
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Ventas",
      },
    },
    series: series,
    drilldown: {
      series: drilldown,
    },
  });
}

function TabProductosComparacionPreciosProveedor(series, drilldown) {
  Highcharts.chart("graficaProductoProveedor", {
    chart: {
      type: "column",
    },
    title: {
      text: "Comparativa de Precios por Proveedor",
    },
    subtitle: {
      text: 'Haz clic en las columnas para ver el histórico de precios'
    },
    xAxis: {
      type: "category",
    },
    yAxis: {
      title: {
        text: "Precio (Q)",
      },
    },
    legend: {
      enabled: false
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: 'Q{point.y}'
        }
      }
    },
    tooltip: {
      headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
      pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>${point.y}</b><br/>'
    },
    series: series,
    drilldown: {
      series: drilldown
    }
  });
}

function TabProductosMasVendidos(series) {
  Highcharts.chart("graficaProductosMas", {
    chart: {
      type: "pie",
    },
    title: {
      text: null,
    },
    plotOptions: {
      pie: {
        dataLabels: {
          enabled: true,
          format: "<b>{point.name}</b>: {point.y} ({point.percentage:.1f}%)",
        },
      },
    },
    series: series,
  });
}

function TabProductosMenosVendidos(series) {
  Highcharts.chart("graficaProductosMenos", {
    chart: {
      type: "pie",
    },
    title: {
      text: "  ",
    },
    plotOptions: {
      pie: {
        dataLabels: {
          enabled: true,
          format: "<b>{point.name}</b>: {point.y} ({point.percentage:.1f}%)",
        },
      },
    },
    series: series,
  });
}

function ServiciosGenerales(data) {
  // 1. Mapeamos los datos para la serie principal (Nivel 1)
  const seriesData = data.series[0].data.map(item => ({
    name: item.name,
    y: item.y,
    drilldown: item.drilldown // Mantiene la conexión al drilldown
  }));

  Highcharts.chart("graficaServiciosGeneral", {
    chart: {
      type: "bar",
      height: 600,
    },
    title: {
      text: null
    },
    xAxis: {
      type: "category",
      labels: {
        style: {
          color: '#000000',
          textDecoration: 'none'
        }
      }
    },
    yAxis: {
      title: {
        text: "Cantidad de pacientes atendidos",
      },
    },
    legend: {
      enabled: false
    },
    plotOptions: {
      series: {
        borderWidth: 0,
        dataLabels: {
          enabled: true,
          format: "{point.y}",
          style: {
            color: '#000000',
            textDecoration: 'none'
          }
        },
      },
    },
    tooltip: {
      headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
      pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y}</b> pacientes<br/>',
    },
    series: [
      {
        name: "Pacientes",
        colorByPoint: true,
        data: seriesData,
      },
    ],
    drilldown: {
      activeAxisLabelStyle: {
        textDecoration: 'none',
        color: '#000000',
        fontWeight: 'bold'
      },
      activeDataLabelStyle: {
        textDecoration: 'none',
        color: '#000000'
      },
      series: data.drilldown.map(ds => ({
        id: ds.id,
        name: ds.name,
        data: ds.data.map(d => [d.name, d.y])
      })),
      breadcrumbs: {
        position: {
          align: "right",
        },
      },
    },
  });
}


function ServiciosGeneralesPorcentaje(dataa) {
  const pieData = dataa.series[0].data.map(item => ({
    name: item.name,
    y: item.y,
    drilldown: item.drilldown
  }));

  Highcharts.chart("graficaServiciosGeneral%", {
    chart: {
      type: "pie",
      style: { fontFamily: 'Roboto, Arial, sans-serif' }
    },
    title: {
      text: null
    },
    tooltip: {
      pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
    },
    accessibility: {
      point: {
        valueSuffix: '%'
      }
    },
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: "pointer",
        dataLabels: {
          enabled: true,
          format: '<span style="color:#000000; text-decoration:none;"><b>{point.name}</b>: {point.percentage:.1f} %</span>',
          style: {
            textOutline: 'none' 
          }
        },
        showInLegend: true
      }
    },
    series: [
      {
        name: "Porcentaje de pacientes",
        colorByPoint: true,
        data: pieData,
      },
    ],
    drilldown: {
      activeAxisLabelStyle: { textDecoration: 'none', color: '#000000' },
      activeDataLabelStyle: { textDecoration: 'none', color: '#000000' },
      series: dataa.drilldown.map(ds => ({
        id: ds.id,
        name: ds.name,
        data: ds.data.map(d => [d.name, d.y])
      }))
    }
  });
}

function GraficoDeIngresosPorLinea() {
  Highcharts.chart("GraficoIngresosPorLinea", {
    chart: {
      type: "column",
    },
    title: {
      text: "  ",
    },
    xAxis: {
      categories: ["2020", "2021", "2022"],
    },
    yAxis: {
      title: {
        text: "Ingresos (en $)",
      },
    },
    plotOptions: {
      series: {
        cursor: "pointer",
        point: {
          events: {
            click: function () {
              alert("Ingresos: " + this.y + "$");
            },
          },
        },
      },
    },
    series: [
      {
        name: "Consulta Externa",
        data: [150000, 160000, 170000], // Datos inventados
      },
      {
        name: "Urgencias",
        data: [100000, 110000, 120000], // Datos inventados
      },
      {
        name: "Cirug�as",
        data: [200000, 190000, 210000], // Datos inventados
      },
    ],
  });
}

function GraficoDeNumeroExamenes() {
  Highcharts.chart("GraficoNumerodeExamenes", {
    chart: {
      type: "column",
    },
    title: {
      text: "  ",
    },
    xAxis: {
      categories: [
        "Consulta Externa",
        "Urgencias",
        "Laboratorio",
        "Radiolog�a",
        "Cirug�as",
      ],
    },
    yAxis: {
      title: {
        text: "N�mero de Ex�menes Realizados",
      },
    },
    series: [
      {
        name: "N�mero de Ex�menes",
        data: [500, 300, 700, 400, 600], // Datos inventados
      },
    ],
  });
}
