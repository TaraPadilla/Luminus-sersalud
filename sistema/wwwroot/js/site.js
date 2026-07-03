let ParametroConfiguracion = {
  ColorHabitacion: "#ffffff",
  ColorHabitacionSeleccionada: "#f5f5f5",
};

//Configuraciones del sistema
let configuracionNombreEstablecimiento;

function initSelect2Controls() {
  $(".select2bs4").each(function () {
    var $el = $(this);
    var bindAttr = $el.attr("data-bind") || "";
    if (bindAttr.indexOf("options:") >= 0) {
      return;
    }
    if ($el.hasClass("select2-hidden-accessible")) {
      $el.select2("destroy");
    }
    $el.select2({
      theme: "bootstrap4",
      width: "100%",
      minimumResultsForSearch: 0,
    });
  });

  if (window.SelectHelpers) {
    SelectHelpers.cleanupSelectTree(document);
  }
}

$(".enlace-redirigir").on("click", function () {
  showLoading();
  setTimeout(function () {
    hideLoading();
  }, 1500);
});
function contraerMenu() {
  $("#menu-navegacion-izquierdo").addClass("closed-sidebar");
}

/** Keep SerSalud sidebar pinned and expanded on desktop; main.js collapses below 1250px. */
function syncFixedSidebarLayout() {
  var container = document.getElementById("menu-navegacion-izquierdo");
  if (!container || !container.classList.contains("fixed-sidebar")) {
    return;
  }

  if (container.classList.contains("layout-sersalud-compact") && window.innerWidth >= 992) {
    container.classList.remove("closed-sidebar-mobile", "closed-sidebar");
    return;
  }

  if (window.innerWidth >= 992 && container.classList.contains("closed-sidebar-mobile")) {
    container.classList.remove("closed-sidebar-mobile", "closed-sidebar");
  }
}
function mensajeEmergente(mensaje) {
  toastr.options = {
    closeButton: false,
    debug: false,
    newestOnTop: false,
    progressBar: true,
    positionClass: "toast-top-center",
    preventDuplicates: true,
    onclick: null,
    showDuration: "100",
    hideDuration: "1000",
    timeOut: "5000",
    extendedTimeOut: "1000",
    showEasing: "swing",
    hideEasing: "linear",
    showMethod: "show",
    hideMethod: "hide",
  };
  toastr.success(mensaje);
}
function mensajeEmergenteExito(mensaje) {
  mensajeEmergente(mensaje);
}

function mensajeEmergenteError(mensaje) {
  toastr.options = {
    closeButton: false,
    debug: false,
    newestOnTop: false,
    progressBar: true,
    positionClass: "toast-top-center",
    preventDuplicates: true,
    onclick: null,
    showDuration: "100",
    hideDuration: "1000",
    timeOut: "5000",
    extendedTimeOut: "1000",
    showEasing: "swing",
    hideEasing: "linear",
    showMethod: "show",
    hideMethod: "hide",
  };
  toastr.error(mensaje);
}

var _loadingCount = 0;

function showLoading() {
  _loadingCount++;
  $("#div-loading").show();
}

function showModalRespuesta(modalId) {
  $("#" + modalId).dialog({
    modal: true,
    buttons: [
      {
        text: "OK",
        click: function () {
          $(this).dialog("close");
        },
      },
    ],
  });
}

function hideLoading() {
  _loadingCount = Math.max(0, _loadingCount - 1);
  if (_loadingCount === 0) {
    $("#div-loading").hide();
  }
}

function forceHideLoading() {
  _loadingCount = 0;
  $("#div-loading").hide();
}

function clearDataTable(tablaId) {
  var $table = $("#" + tablaId);
  if (!$table.length || !$.fn.DataTable.isDataTable($table)) {
    return;
  }

  $table.DataTable().clear().draw();
  $table.DataTable().destroy();
}
function drawDataTable(tablaId) {
  var $table = $("#" + tablaId);
  if (!$table.length) {
    return;
  }

  if ($.fn.DataTable.isDataTable($table)) {
    $table.DataTable().destroy();
  }

  $table.DataTable({
    searching: true,
    ordering: true,
    paging: true,
    language: {
      search: "Buscar: ",
      lengthMenu: "Mostrar _MENU_ registros por página",
      zeroRecords: "No hay registros para mostrar",
      info: "Mostrando página _PAGE_ de _PAGES_",
      infoEmpty: "",
      infoFiltered: "(filtrado de _MAX_ registros totales)",
      paginate: {
        first: "Primero",
        last: "Último",
        previous: "Anterior",
        next: "Siguiente",
      },
    },
  });
}
function setConfiguracionesSistema() {
  var text = configuracionNombreEstablecimiento || "";
  $("#configuracion-nombre-establecimiento").text(text);
  if (window.__appConfig && window.__appConfig.pestana) {
    document.title = window.__appConfig.pestana;
  }
}
function getConfiguracionesSistema() {
  var cfg = window.__appConfig || {};
  var cliente = cfg.cliente || "";
  configuracionNombreEstablecimiento =
    cfg.nombreEstablecimiento || cfg.pestana || cliente || "";
  setConfiguracionesSistema();
}

getConfiguracionesSistema();

function initMunicipios() {
  // usamos delegación para que funcione si el DOM se inyecta por AJAX
  $(document)
    .off("change", "#DepartamentoSelect")
    .on("change", "#DepartamentoSelect", function () {
      var deptoId = $(this).val();
      console.log("departamentoId:", deptoId);

      $("#SelectedDepartamentoCodigo").val(deptoId);
      if (!deptoId) {
        $("#MunicipioSelect").html(
          '<option value="">-- Seleccione --</option>'
        );
        return;
      }

      $.getJSON("/Reportes/GetMunicipios", { departamentoId: deptoId })
        .done(function (list) {
          console.log("municipios:", list);
          var $mun = $("#MunicipioSelect")
            .empty()
            .append('<option value="">-- Seleccione --</option>');
          list.forEach(function (item) {
            $("<option>").val(item.id).text(item.nombre).appendTo($mun);
          });
          $("#SelectedMunicipioCodigo").val("");
        })
        .fail(function (xhr, status, error) {
          console.error("Error al cargar municipios:", status, error);
        });
    });
}

// Arrancamos también en el load inicial de la página
$(function () {
  if (window.SelectHelpers) {
    SelectHelpers.cleanupSelectTree(document);
  }
  initSelect2Controls();
  initMunicipios();
  syncFixedSidebarLayout();
  setTimeout(syncFixedSidebarLayout, 0);
  setTimeout(syncFixedSidebarLayout, 150);
  $(window).on("resize", function () {
    setTimeout(syncFixedSidebarLayout, 0);
  });

  var $rangoFechasPicker = $("#rangoFechasPicker");
  if ($rangoFechasPicker.length && typeof $rangoFechasPicker.daterangepicker === "function") {
    $rangoFechasPicker.daterangepicker(
      {
        locale: { format: "YYYY-MM-DD" },
        startDate: moment().startOf("month"),
        endDate: moment().endOf("month"),
        opens: "left",
      },
      function (start, end) {
        var text = start.format("YYYY-MM-DD") + "-" + end.format("YYYY-MM-DD");
        $rangoFechasPicker.val(text);
      }
    );
  }

  $("#MunicipioSelect").on("change", function () {
    var codigo = $(this).val() || "";
    $("#SelectedMunicipioCodigo").val(codigo);
  });

  var frmDescarga = document.getElementById("frmDescarga");
  if (frmDescarga) {
    frmDescarga.addEventListener("submit", function () {
      var deptoSelect = document.getElementById("DepartamentoSelect");
      var muniSelect = document.getElementById("MunicipioSelect");
      var deptoInput = this.querySelector("input[name=departamentoId]");
      var muniInput = this.querySelector("input[name=municipioId]");
      if (deptoSelect && deptoInput) {
        deptoInput.value = deptoSelect.value;
      }
      if (muniSelect && muniInput) {
        muniInput.value = muniSelect.value;
      }
    });
  }
});
