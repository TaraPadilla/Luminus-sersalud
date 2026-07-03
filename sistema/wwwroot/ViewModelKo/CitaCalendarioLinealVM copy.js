var CitaCalendarioLinealVM = function () {
  var self = this;
  self.diaHora = ko.observable();
  self.bloquearDia = function () {
    if (confirm("�Desea bloquear este dia?")) {
      var motivo = prompt("Escribe el motivo!!");
      showLoading();
      $.ajax({
        url: "/Cita/BloquearDia",
        method: "POST",
        data: {
          dia: $("#fechacita").val(),
          empleadoId: $("#EmpleadoId").val(),
          motivo: motivo,
        },
        success: function (dataResult) {
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            window.location.reload();
          } else {
            hideLoading();
            alert(data.Mensaje);
          }
        },
        error: function (dataerror) {
          hideLoading();
          alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
        },
      });
    }
  };

  self.actualizarContadores = function () {
    $.ajax({
      url: "/Cita/ObtenerContadores",
      method: "GET",
      data: {
        buscar: $("#fechacita").val(),
        buscar2: $("#fechacita2").val(), // ✅ agregado: ahora el backend recibe buscar2
        sucursalId: $("#SucursalId").val(),
        empleadoId: $("#EmpleadoId").val(),
        especialidadId: $("#EspecialidadId").val(),
        servicioId: $("#ServicioId").val(), // Include servicioId in the request
        pacienteNombre: ($("#pacienteNombre").val() || "").trim(),
      },
      success: function (data) {
        // Update counters in the DOM
        $("#citasNoPagadas").text(data.CitasNoPagadas);
        $("#citasEnEspera").text(data.CitasEnEspera);
        $("#citasFinalizadas").text(data.CitasFinalizadas);
      },
      error: function (error) {
        console.error("Error updating counters:", error);
      },
    });
  };

  // Llamar a la función para actualizar los contadores cada vez que la página se carga o cambia una fecha/selector
  self.inicializar = function () {
    self.actualizarContadores();

    // Si cambian los filtros, volver a cargar los contadores
    $("#fechacita, #fechacita2, #SucursalId, #EmpleadoId, #EspecialidadId, #ServicioId").on(
      "change",
      function () {
        self.actualizarContadores();
      }
    );
  };

  self.desbloquearDia = function (fechaYHora) {
    if (confirm("¿Desea desbloquear este día?")) {
      showLoading();
      $.ajax({
        url: "/Cita/DesbloquearDia",
        method: "POST",
        data: {
          dia: fechaYHora,
          empleadoId: $("#EmpleadoId").val(),
        },
        success: function (dataResult) {
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            window.location.reload();
          } else {
            hideLoading();
            alert(data.Mensaje);
          }
        },
        error: function (dataerror) {
          hideLoading();
          alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
        },
      });
    }
  };

  self.bloquearDiaHora = function () {
    self.diaHora();
    console.log($("#fechaBloquear").text().trim());
    //if (confirm("�Desea bloquear este dia?")) {
    //    var motivo = prompt("Escribeel motivo!!");
    //    showLoading();
    //    $.ajax({
    //        url: "/Cita/BloquearDia",
    //        method: "POST",
    //        data: {
    //            dia: $("#fechacita").val(),
    //            empleadoId: $("#EmpleadoId").val(),
    //            motivo: motivo
    //        },
    //        success: function (dataResult) {
    //            let data = JSON.parse(dataResult);
    //            if (data.Exitoso) {
    //                window.location.reload();
    //            } else {
    //                hideLoading();
    //                alert(data.Mensaje);
    //            }
    //        },
    //        error: function (dataerror) {
    //            hideLoading();
    //            alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
    //        }
    //    })
    //}
  };
};

var calendarioVm = new CitaCalendarioLinealVM();
calendarioVm.inicializar();
ko.applyBindings(calendarioVm);

function bloquearDiaHora(fechaHora) {
  console.log(fechaHora);
  if (confirm("�Desea bloquear esta hora ?")) {
    var motivo = prompt("Escribe el motivo!!");
    showLoading();
    $.ajax({
      url: "/Cita/BloquearFecha",
      method: "POST",
      data: {
        fecha: fechaHora,
        empleadoId: $("#EmpleadoId").val(),
        motivo: motivo,
      },
      success: function (dataResult) {
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          window.location.reload();
        } else {
          hideLoading();
          alert(data.Mensaje);
        }
      },
      error: function (dataerror) {
        hideLoading();
        alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
      },
    });
  }
}

$(document).ready(function () {
  function navegarCalendario() {
    showLoading();

    var pacienteNombre = ($("#pacienteNombre").val() || "").trim();

    window.location.href =
      "/Cita/CalendarioLineal?buscar=" +
      encodeURIComponent($("#fechacita").val() || "") +
      "&buscar2=" +
      encodeURIComponent($("#fechacita2").val() || "") +
      "&sucursalId=" +
      encodeURIComponent($("#SucursalId").val() || "") +
      "&empleadoId=" +
      encodeURIComponent($("#EmpleadoId").val() || "") +
      "&especialidadId=" +
      encodeURIComponent($("#EspecialidadId").val() || "") +
      "&servicioId=" +
      encodeURIComponent($("#ServicioId").val() || "") +
      "&pacienteNombre=" +
      encodeURIComponent(pacienteNombre);
  }

  // Cambios en filtros existentes
  $(
    "#fechacita, #fechacita2, #SucursalId, #EmpleadoId, #EspecialidadId, #ServicioId"
  ).on("change", function () {
    navegarCalendario();
  });

  // Escribir en buscador (con debounce)
  var pacienteTimer = null;
  $("#pacienteNombre").on("keyup", function (e) {
    // Enter = navegar inmediato
    if (e.keyCode === 13) {
      if (pacienteTimer) clearTimeout(pacienteTimer);
      navegarCalendario();
      return;
    }

    if (pacienteTimer) clearTimeout(pacienteTimer);
    pacienteTimer = setTimeout(function () {
      navegarCalendario();
    }, 400);
  });

  // Si pega texto o sale del input
  $("#pacienteNombre").on("change", function () {
    navegarCalendario();
  });
});

function Eliminar(id, fecha) {
  var option = confirm("�Est� seguro/a que desea eliminar este registro?");

  if (option) {
    var data = {
      id: parseInt(id),
      fecha: fecha,
    };

    $.ajax({
      url: "/Cita/EliminarCita/",
      data: data,
      type: "POST",
      success: function (result) {
        var pacienteNombre = ($("#pacienteNombre").val() || "").trim();
        window.location.href =
          "/Cita/CalendarioLineal?buscar=" +
          encodeURIComponent(result) +
          "&pacienteNombre=" +
          encodeURIComponent(pacienteNombre);
      },
      error: function (error) {
        alert(error);
      },
    });
  }
}

function Finalizar(id, fecha) {
  var option = confirm("�Est� seguro/a que desea finalizar esta cita?");

  if (option) {
    var data = {
      id: parseInt(id),
      fecha: fecha,
    };

    $.ajax({
      url: "/Cita/FinalizarCita/",
      data: data,
      type: "POST",
      success: function (result) {
        window.location.href = "/Cita/CalendarioLineal?buscar=" + result;
      },
      error: function (error) {
        alert(error);
      },
    });
  }
}
