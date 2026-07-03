// ============================================================
//  CitaCalendarioLinealVM.js  — versión con soporte de MODO
//  Cambios marcados con  // ← NUEVO  o  // ← MODIFICADO
// ============================================================

var CitaCalendarioLinealVM = function () {
  var self = this;
  self.diaHora = ko.observable();

  self.bloquearDia = function () {
    if (confirm("¿Desea bloquear este dia?")) {
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
        buscar:         $("#fechacita").val(),
        buscar2:        $("#fechacita2").val(),
        sucursalId:     $("#SucursalId").val(),
        empleadoId:     $("#EmpleadoId").val(),
        especialidadId: $("#EspecialidadId").val(),
        servicioId:     $("#ServicioId").val(),
        pacienteNombre: ($("#pacienteNombre").val() || "").trim(),
        modo:           $("#modoCalendario").val(),   // ← NUEVO
      },
      success: function (data) {
        $("#citasNoPagadas").text(data.CitasNoPagadas);
        $("#citasEnEspera").text(data.CitasEnEspera);
        $("#citasFinalizadas").text(data.CitasFinalizadas);
      },
      error: function (error) {
        console.error("Error updating counters:", error);
      },
    });
  };

  self.inicializar = function () {
    self.actualizarContadores();

    // Si cambian los filtros, recargar contadores
    // ← MODIFICADO: se agrega #modoCalendario
    $("#fechacita, #fechacita2, #SucursalId, #EmpleadoId, #EspecialidadId, #ServicioId, #modoCalendario").on(
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
  };
};

var calendarioVm = new CitaCalendarioLinealVM();
calendarioVm.inicializar();
ko.applyBindings(calendarioVm);

// ─────────────────────────────────────────────────────────────
// bloquearDiaHora — sin cambios
// ─────────────────────────────────────────────────────────────
function bloquearDiaHora(fechaHora) {
  if (confirm("¿Desea bloquear esta hora?")) {
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

// ─────────────────────────────────────────────────────────────
// navegarCalendario — MODIFICADO: incluye modo en la URL
// ─────────────────────────────────────────────────────────────
$(document).ready(function () {

  function navegarCalendario() {
    showLoading();
    var pacienteNombre = ($("#pacienteNombre").val() || "").trim();
    var modo = $("#modoCalendario").val() || "consulta";   // ← NUEVO

    window.location.href =
      "/Cita/CalendarioLineal?buscar="        + encodeURIComponent($("#fechacita").val()       || "") +
      "&buscar2="                             + encodeURIComponent($("#fechacita2").val()      || "") +
      "&sucursalId="                          + encodeURIComponent($("#SucursalId").val()      || "") +
      "&empleadoId="                          + encodeURIComponent($("#EmpleadoId").val()      || "") +
      "&especialidadId="                      + encodeURIComponent($("#EspecialidadId").val()  || "") +
      "&servicioId="                          + encodeURIComponent($("#ServicioId").val()      || "") +
      "&pacienteNombre="                      + encodeURIComponent(pacienteNombre)                   +
      "&modo="                                + encodeURIComponent(modo);                   // ← NUEVO
  }

  // Cambios en filtros existentes — MODIFICADO: agrega #modoCalendario
  $("#fechacita, #fechacita2, #SucursalId, #EmpleadoId, #EspecialidadId, #ServicioId, #modoCalendario").on(
    "change",
    function () {
      navegarCalendario();
    }
  );

  // Buscador con debounce — sin cambios
  var pacienteTimer = null;
  $("#pacienteNombre").on("keyup", function (e) {
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

  $("#pacienteNombre").on("change", function () {
    navegarCalendario();
  });
});

// ─────────────────────────────────────────────────────────────
// Eliminar — MODIFICADO: preserva el modo en el redirect
// ─────────────────────────────────────────────────────────────
function Eliminar(id, fecha) {
  var option = confirm("¿Está seguro/a que desea eliminar este registro?");

  if (option) {
    var data = { id: parseInt(id), fecha: fecha };

    $.ajax({
      url: "/Cita/EliminarCita/",
      data: data,
      type: "POST",
      success: function (result) {
        var pacienteNombre = ($("#pacienteNombre").val() || "").trim();
        var modo = $("#modoCalendario").val() || "consulta";   // ← NUEVO
        window.location.href =
          "/Cita/CalendarioLineal?buscar=" + encodeURIComponent(result) +
          "&pacienteNombre=" + encodeURIComponent(pacienteNombre) +
          "&modo=" + encodeURIComponent(modo);                  // ← NUEVO
      },
      error: function (error) {
        alert(error);
      },
    });
  }
}

// ─────────────────────────────────────────────────────────────
// Finalizar — MODIFICADO: preserva el modo en el redirect
// ─────────────────────────────────────────────────────────────
function Finalizar(id, fecha) {
  var option = confirm("¿Está seguro/a que desea finalizar esta cita?");

  if (option) {
    var data = { id: parseInt(id), fecha: fecha };

    $.ajax({
      url: "/Cita/FinalizarCita/",
      data: data,
      type: "POST",
      success: function (result) {
        var modo = $("#modoCalendario").val() || "consulta";   // ← NUEVO
        window.location.href =
          "/Cita/CalendarioLineal?buscar=" + result +
          "&modo=" + encodeURIComponent(modo);                  // ← NUEVO
      },
      error: function (error) {
        alert(error);
      },
    });
  }
}