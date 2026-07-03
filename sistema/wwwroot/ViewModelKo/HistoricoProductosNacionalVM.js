var HistoricoProductosVM = function () {
  var self = this;

  // Dataset base
  self.movimientosProductos = ko.observableArray([]);

  // Selecciones
  self.selectedTiposProducto = ko.observableArray([]);
  self.selectedAmbientes = ko.observableArray([]);
  self.selectedBodegas = ko.observableArray([]);
  self.selectedProductos = ko.observableArray([]);
  self.totalRegistros = ko.observable(0);

  // Modal genérico
  self.modalFiltroTipo = ko.observable("");
  self.modalFiltroTitulo = ko.observable("Filtrar");
  self.modalFiltroColumnaNombre = ko.observable("Nombre");
  self.modalFiltroOpciones = ko.observableArray([]);
  self.modalSelectAll = ko.observable(false);

  // Referencia a DataTable
  self.dataTable = null;

  function normalizeText(value) {
    return (value || "").toString().trim().toLowerCase();
  }

  function distinctSorted(values) {
    var map = {};
    (values || []).forEach(function (v) {
      var key = normalizeText(v);
      if (!key) return;
      map[key] = v;
    });

    return Object.keys(map)
      .sort(function (a, b) {
        return a.localeCompare(b);
      })
      .map(function (k) {
        return map[k];
      });
  }

  function openFiltroModal(title, columnName, opciones) {
    self.modalFiltroTitulo(title);
    self.modalFiltroColumnaNombre(columnName);
    self.modalFiltroOpciones(opciones);

    var allSelected = (opciones || []).length > 0 &&
      opciones.every(function (o) {
        return o.selected && o.selected();
      });
    self.modalSelectAll(!!allSelected);

    $("#mdl-filtro-generico").dialog({
      modal: true,
      width: 720,
      title: title,
    });
  }

  self.modalSelectAll.subscribe(function (val) {
    var opciones = self.modalFiltroOpciones() || [];
    opciones.forEach(function (o) {
      if (o.selected) o.selected(!!val);
    });
  });

  self.buildOpcionesFromMovimientos = function (tipo) {
    var movimientos = self.movimientosProductos() || [];
    var selectedArray;

    var values = [];
    if (tipo === "TipoProducto") {
      values = movimientos.map(function (x) { return x.TipoProductoNombre; });
      selectedArray = self.selectedTiposProducto;
    } else if (tipo === "Ambiente") {
      values = movimientos.map(function (x) { return x.AmbienteNombre; });
      selectedArray = self.selectedAmbientes;
    } else if (tipo === "Bodega") {
      values = movimientos.map(function (x) { return x.BodegaNombre; });
      selectedArray = self.selectedBodegas;
    } else if (tipo === "Producto") {
      values = movimientos.map(function (x) { return x.Medicamento; });
      selectedArray = self.selectedProductos;
    } else {
      values = [];
      selectedArray = ko.observableArray([]);
    }

    var selectedSet = {};
    (selectedArray() || []).forEach(function (s) {
      selectedSet[normalizeText(s)] = true;
    });

    var distinct = distinctSorted(values);
    return distinct.map(function (name) {
      return {
        name: name,
        selected: ko.observable(!!selectedSet[normalizeText(name)]),
      };
    });
  };

  self.showModalFiltrarTipoProducto = function () {
    self.modalFiltroTipo("TipoProducto");
    openFiltroModal(
      "Filtrar tipo de producto",
      "Tipo de producto",
      self.buildOpcionesFromMovimientos("TipoProducto")
    );
  };

  self.showModalFiltrarAmbientes = function () {
    self.modalFiltroTipo("Ambiente");
    openFiltroModal(
      "Filtrar ambientes",
      "Ambiente",
      self.buildOpcionesFromMovimientos("Ambiente")
    );
  };

  self.showModalFiltrarBodegas = function () {
    self.modalFiltroTipo("Bodega");
    openFiltroModal(
      "Filtrar bodegas",
      "Bodega",
      self.buildOpcionesFromMovimientos("Bodega")
    );
  };

  self.showModalFiltrarProductos = function () {
    self.modalFiltroTipo("Producto");
    openFiltroModal(
      "Filtrar productos",
      "Producto",
      self.buildOpcionesFromMovimientos("Producto")
    );
  };

  // Configuración de DataTables
  function getDataTableConfig(pageLength) {
    return {
      paging: true,
      searching: true,
      ordering: true,
      pageLength: pageLength || 10,
      lengthMenu: [10, 25, 50, 100],
      language: {
        processing: "Procesando...",
        search: "Buscar:",
        lengthMenu: "Mostrar _MENU_ registros",
        info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
        infoEmpty: "Mostrando 0 a 0 de 0 registros",
        infoFiltered: "(filtrado de _MAX_ registros totales)",
        loadingRecords: "Cargando...",
        zeroRecords: "No se encontraron resultados",
        emptyTable: "No hay información para mostrar",
        paginate: {
          first: "Primero",
          previous: "Anterior",
          next: "Siguiente",
          last: "Último",
        },
        aria: {
          sortAscending: ": activar para ordenar ascendente",
          sortDescending: ": activar para ordenar descendente",
        },
      },
      autoWidth: false,
      destroy: true,
      data: self.getFilteredData(),
      columns: [
        {
          data: "Fecha",
          render: function (data) {
            return moment(data).format('DD-MM-YYYY HH:mm');
          }
        },
        { data: "AmbienteNombre" },
        { data: "BodegaNombre" },
        { data: "TipoMovimientoNombre" },
        { data: "DescripcionMovimiento" },
        { data: "TipoProductoNombre" },
        { data: "Medicamento" },
        { data: "Lote" },
        { data: "UnidadNombre" },
        {
          data: "Cantidad",
          className: "text-right",
          render: function (data) { return data; }
        },
        {
          data: "SaldoActual",
          className: "text-right",
          render: function (data) { return data; }
        },
        {
          data: null,
          render: function () { return "-"; },
          className: "text-muted text-nowrap"
        },
        { data: "UsuarioNombre" },
        { data: "UsuarioEntrega" },
        { data: "BodegaNombre" },
        {
          data: null,
          defaultContent: "999"
        }

      ]
    };
  }

  // Obtener datos filtrados
  self.getFilteredData = function () {
    var items = self.movimientosProductos() || [];

    // Filtro de fechas
    var rango = parseRangoFechas();

    // Filtros de selección
    var tipos = self.selectedTiposProducto() || [];
    var ambientes = self.selectedAmbientes() || [];
    var bodegas = self.selectedBodegas() || [];
    var productos = self.selectedProductos() || [];

    var tiposSet = {}, ambientesSet = {}, bodegasSet = {}, productosSet = {};

    tipos.forEach(function (x) { tiposSet[normalizeText(x)] = true; });
    ambientes.forEach(function (x) { ambientesSet[normalizeText(x)] = true; });
    bodegas.forEach(function (x) { bodegasSet[normalizeText(x)] = true; });
    productos.forEach(function (x) { productosSet[normalizeText(x)] = true; });

    var hasTipos = tipos.length > 0;
    var hasAmb = ambientes.length > 0;
    var hasBod = bodegas.length > 0;
    var hasProd = productos.length > 0;
    var hasFechas = rango !== null;

    return items.filter(function (x) {
      // Filtro de fechas
      var fechaOk = true;
      if (hasFechas) {
        var fechaMov = moment(x.Fecha);
        fechaOk = fechaMov.isBetween(rango.fechaInicio, rango.fechaFin, null, '[]');
      }

      // Filtros de selección
      var tipoOk = !hasTipos || !!tiposSet[normalizeText(x.TipoProductoNombre)];
      var ambienteOk = !hasAmb || !!ambientesSet[normalizeText(x.AmbienteNombre)];
      var bodegaOk = !hasBod || !!bodegasSet[normalizeText(x.BodegaNombre)];
      var productoOk = !hasProd || !!productosSet[normalizeText(x.Medicamento)];

      return fechaOk && tipoOk && ambienteOk && bodegaOk && productoOk;
    });
  };

  // Parse rango fechas
  function parseRangoFechas() {
    var raw = ($("#reservationtime").val() || "").trim();
    if (!raw) return null;
    if (raw.indexOf(" - ") < 0) return null;

    var parts = raw.split(" - ");
    var inicio = moment(parts[0], "MM/DD/YYYY hh:mm A", true);
    var fin = moment(parts[1], "MM/DD/YYYY hh:mm A", true);

    if (!inicio.isValid() || !fin.isValid()) return null;

    return {
      fechaInicio: inicio,
      fechaFin: fin
    };
  }


  // Actualizar DataTable
  self.refreshDataTable = function () {
    var $tbl = $("#tabla-historico");
    var currentPageLength = self.dataTable ? self.dataTable.page.len() : 10;

    // Destruir DataTable existente si hay
    if ($.fn.DataTable && $.fn.DataTable.isDataTable($tbl)) {
      $tbl.DataTable().destroy();
    }

    // Limpiar tbody
    $tbl.find("tbody").empty();

    // Crear nueva DataTable
    setTimeout(function () {
      self.dataTable = $tbl.DataTable(getDataTableConfig(currentPageLength));

      // Actualizar contador de registros (usando movimientosFiltrados)
      $(".text-muted.small.ml-2").text(self.movimientosFiltrados().length + ' registros');
    }, 50);
  };


  self.movimientosFiltrados = ko.pureComputed(function () {
    return self.getFilteredData();
  });



  self.aplicarFiltroModal = function () {
    var tipo = self.modalFiltroTipo();
    var opciones = self.modalFiltroOpciones() || [];

    var selectedNames = opciones
      .filter(function (o) { return o.selected && o.selected(); })
      .map(function (o) { return o.name; });

    if (tipo === "TipoProducto") self.selectedTiposProducto(selectedNames);
    else if (tipo === "Ambiente") self.selectedAmbientes(selectedNames);
    else if (tipo === "Bodega") self.selectedBodegas(selectedNames);
    else if (tipo === "Producto") self.selectedProductos(selectedNames);

    $("#mdl-filtro-generico").dialog("close");

    // Forzar actualización de la computada antes de refrescar la tabla
    self.movimientosFiltrados();
    self.refreshDataTable();
  };

  self.limpiarFiltroModal = function () {
    (self.modalFiltroOpciones() || []).forEach(function (o) {
      if (o.selected) o.selected(false);
    });
    self.modalSelectAll(false);
  };

  self.limpiarFecha = function () {
    $("#reservationtime").val("");
    var drp = $("#reservationtime").data("daterangepicker");
    if (drp) {
      drp.setStartDate(moment());
      drp.setEndDate(moment());
    }
    self.refreshDataTable();
  };

  self.limpiarFiltros = function () {
    self.selectedTiposProducto([]);
    self.selectedAmbientes([]);
    self.selectedBodegas([]);
    self.selectedProductos([]);
    self.limpiarFecha();
  };

  // Consultar histórico
  self.consultarHistorico = function () {
    if (typeof showLoading === "function") showLoading();

    var rango = parseRangoFechas();
    var payload = {
      fechaInicio: rango ? rango.fechaInicio.toISOString() : null,
      fechaFin: rango ? rango.fechaFin.toISOString() : null,
    };

    $.ajax({
      url: "/Productos/ConsultarHistoricoProductosNacional",
      method: "POST",
      data: payload,
      success: function (result) {
        if (typeof hideLoading === "function") hideLoading();

        var data = result;
        if (typeof result === "string") {
          try {
            data = JSON.parse(result);
          } catch (e) {
            data = null;
          }
        }

        if (data && data.Exitoso) {
          self.movimientosProductos(data.Resultado || []);
          self.refreshDataTable();
        }
      },
      error: function () {
        if (typeof hideLoading === "function") hideLoading();
      },
    });
  };

  // Exportaciones
  function buildExportQueryString() {
    function addParam(list, key, value) {
      if (value === null || value === undefined) return;
      var s = value.toString();
      if (!s) return;
      list.push(encodeURIComponent(key) + "=" + encodeURIComponent(s));
    }

    function normalizeList(arr) {
      return (arr || [])
        .map(function (x) {
          return x === null || x === undefined ? "" : x.toString().trim();
        })
        .filter(function (x) {
          return x.length > 0;
        });
    }

    var qs = [];
    var rango = parseRangoFechas();

    if (rango) {
      addParam(qs, "fechaInicio", rango.fechaInicio.toISOString());
      addParam(qs, "fechaFin", rango.fechaFin.toISOString());
    }

    var tipos = normalizeList(self.selectedTiposProducto());
    var ambientes = normalizeList(self.selectedAmbientes());
    var bodegas = normalizeList(self.selectedBodegas());
    var productos = normalizeList(self.selectedProductos());

    if (tipos.length) addParam(qs, "tiposProducto", tipos.join("|"));
    if (ambientes.length) addParam(qs, "ambientes", ambientes.join("|"));
    if (bodegas.length) addParam(qs, "bodegas", bodegas.join("|"));
    if (productos.length) addParam(qs, "productos", productos.join("|"));

    return qs.join("&");
  }

  function openExport(urlBase) {
    var query = buildExportQueryString();
    var url = query ? urlBase + "?" + query : urlBase;
    window.open(url, "_blank");
  }

  self.generarReportePDF = function () {
    openExport("/CrearPDF/HistoricoProductosPDFNacional");
  };

  self.generarReporteExcel = function () {
    openExport("/Productos/HistoricoProductosExcel");
  };
};

// Inicialización
var historicoVm = null;

$(function () {
  // Inicializar datepicker
  $('#reservationtime').daterangepicker({
    timePicker: true,
    timePickerIncrement: 30,
    autoUpdateInput: false,
    locale: {
      format: 'MM/DD/YYYY hh:mm A',
      applyLabel: 'Aplicar',
      cancelLabel: 'Limpiar',
      fromLabel: 'Desde',
      toLabel: 'Hasta',
      customRangeLabel: 'Personalizado',
      weekLabel: 'S',
      daysOfWeek: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa'],
      monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
      firstDay: 1
    }
  });

  $('#reservationtime').on('apply.daterangepicker', function (ev, picker) {
    $(this).val(
      picker.startDate.format('MM/DD/YYYY hh:mm A') +
      ' - ' +
      picker.endDate.format('MM/DD/YYYY hh:mm A')
    );
    if (historicoVm) historicoVm.refreshDataTable();
  });

  $('#reservationtime').on('cancel.daterangepicker', function () {
    $(this).val('');
    if (historicoVm) historicoVm.refreshDataTable();
  });

  // Inicializar ViewModel
  historicoVm = new HistoricoProductosVM();
  ko.applyBindings(historicoVm);

  // Carga inicial
  if (historicoVm && historicoVm.consultarHistorico) {
    historicoVm.consultarHistorico();
  }
});