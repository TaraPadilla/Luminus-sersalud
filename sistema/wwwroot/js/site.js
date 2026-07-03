let ParametroConfiguracion = {
  ColorHabitacion: "#ffffff",
  ColorHabitacionSeleccionada: "#f5f5f5",
};

//Configuraciones del sistema
let configuracionNombreEstablecimiento;

$(".enlace-redirigir").on("click", function () {
  showLoading();
  setInterval(function () {
    hideLoading();
  }, 1500);
});

$(".select2bs4").select2({
  theme: "bootstrap4",
  tags: true,
});
function contraerMenu() {
  $("#menu-navegacion-izquierdo").addClass("closed-sidebar");
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

function showLoading() {
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
  $("#div-loading").hide();
}

function clearDataTable(tablaId) {
  var table = $("#" + tablaId).DataTable();
  table.clear().draw();

  $("#" + tablaId)
    .dataTable()
    .fnDestroy();
}
function drawDataTable(tablaId) {
  $("#" + tablaId).DataTable({
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
  $("#configuracion-nombre-establecimiento").text(
    configuracionNombreEstablecimiento + " -AVM"
  );
}
function getConfiguracionesSistema() {
  configuracionNombreEstablecimiento = "AVM";
  setConfiguracionesSistema();
  //$.ajax({
  //    method: "POST",
  //    url: '/Grabaciones/Modificar',
  //    success: function (data, textStatus) {
  //        if (data.exitoso) {
  //            window.location.href = "/Grabaciones/Lista";
  //        }
  //        else {
  //            $("#div-loading").hide();
  //            alert(data.mensaje);
  //        }
  //    },
  //    error: function (data) {
  //        $("#div-loading").hide();
  //        alert(data.error);
  //    }
  //});
}

getConfiguracionesSistema();

function initMunicipios() {
  console.log("Inicializando municipios");
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
  initMunicipios();
  $("#rangoFechasPicker").daterangepicker(
    {
      locale: { format: "YYYY-MM-DD" },
      startDate: moment().startOf("month"),
      endDate: moment().endOf("month"),
      opens: "left",
    },
    function (start, end) {
      // Al cambiar, formateo el valor tal como lo espera tu controlador
      var text = start.format("YYYY-MM-DD") + "-" + end.format("YYYY-MM-DD");
      $("#rangoFechasPicker").val(text);
    }
  );
});
$("#MunicipioSelect").change(function () {
  // tomo su valor (el id)
  var codigo = $(this).val() || "";
  // y lo pongo en el input de código
  $("#SelectedMunicipioCodigo").val(codigo);
});
document.getElementById("frmDescarga").addEventListener("submit", function (e) {
  // leemos los selects
  var depto = document.getElementById("DepartamentoSelect").value;
  var muni = document.getElementById("MunicipioSelect").value;
  // sobreescribimos los hidden
  this.querySelector("input[name=departamentoId]").value = depto;
  this.querySelector("input[name=municipioId]").value = muni;
  // el rango de fechas ya lo tenías
});
