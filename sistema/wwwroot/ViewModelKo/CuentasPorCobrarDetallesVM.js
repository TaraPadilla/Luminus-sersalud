var CuentasPorCobrarDetallesVM = function () {
  var self = this;

  self.paquetes = ko.observableArray([]);
  self.medicamentosHospitalizacion = ko.observableArray([]);
  self.depositos = ko.observableArray([]);

  self.valorTotalPaquetes = ko.computed(function () {
    var total = 0;
    ko.utils.arrayForEach(self.paquetes(), function (item) {
      total += parseFloat(item.PrecioPaquete) || 0;
    });
    return total.toFixed(2);
  });

  self.valorTotalMedicamentosHospitalizacion = ko.computed(function () {
    var total = 0;
    ko.utils.arrayForEach(self.medicamentosHospitalizacion(), function (item) {
      total += parseFloat(item.Subtotal) || 0;
    });
    return total.toFixed(2);
  });

  self._paquetesCargados = false;
  self._medicamentosCargados = false;
  self._depositosCargados = false;

  self.obtenerPacienteId = function () {
    return (
      $("#PacienteId").val() ||
      $('input[name="CuentasPorCobrar.PacienteId"]').val() ||
      ""
    );
  };

  self.obtenerCuentaId = function () {
    return $("#CuentaId").val() || "";
  };

  self.verInformacionPaciente = function () {
    var pacienteId = self.obtenerPacienteId();
    if (!pacienteId) {
      alert("No se encontró el identificador del paciente.");
      return;
    }
    var ventana = window.open(
      "/Pacientes/Informacion?pacienteId=" + pacienteId,
      "_blank"
    );
    if (!ventana) {
      alert("Permite ventanas emergentes para ver la información del paciente.");
    }
  };

  self.abrirTabPaquetes = function () {
    if (!self._paquetesCargados) {
      self.consultarPaquetes();
    }
  };

  self.abrirTabMedicamentos = function () {
    if (!self._medicamentosCargados) {
      self.consultarMedicamentosHospitalizacion();
    }
  };

  self.abrirTabDepositos = function () {
    if (!self._depositosCargados) {
      self.consultarDepositos();
    }
  };

  self.consultarPaquetes = function () {
    var cuentaId = self.obtenerCuentaId();
    if (!cuentaId) return;

    $("#texto-cargando-paquetes").show();
    $("#texto-error-consultar-paquetes").hide();

    $.ajax({
      url: "/CuentasPorCobrar/ConsultarPaquetesNoPagados",
      method: "POST",
      data: { cuentaId: cuentaId },
      success: function (dataResult) {
        $("#texto-cargando-paquetes").hide();
        var data =
          typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
        if (data.Exitoso) {
          self.paquetes(data.Resultado || []);
          self._paquetesCargados = true;
        } else {
          $("#texto-error-consultar-paquetes").show();
        }
      },
      error: function () {
        $("#texto-cargando-paquetes").hide();
        $("#texto-error-consultar-paquetes").show();
      },
    });
  };

  self.consultarMedicamentosHospitalizacion = function () {
    var cuentaId = self.obtenerCuentaId();
    if (!cuentaId) return;

    $("#texto-cargando-medicamentos-hospitalizacion").show();
    $("#texto-error-consultar-medicamentos-hospitalizacion").hide();

    $.ajax({
      url: "/CuentasPorCobrar/ConsultarMedicamentosNoPagadosHospitalizaciones",
      method: "POST",
      data: { cuentaId: cuentaId },
      success: function (dataResult) {
        $("#texto-cargando-medicamentos-hospitalizacion").hide();
        var data =
          typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
        if (data.Exitoso) {
          self.medicamentosHospitalizacion(data.Resultado || []);
          self._medicamentosCargados = true;
        } else {
          $("#texto-error-consultar-medicamentos-hospitalizacion").show();
        }
      },
      error: function () {
        $("#texto-cargando-medicamentos-hospitalizacion").hide();
        $("#texto-error-consultar-medicamentos-hospitalizacion").show();
      },
    });
  };

  self.consultarDepositos = function () {
    var cuentaId = self.obtenerCuentaId();
    if (!cuentaId) return;

    $("#texto-error-consultar-depositos").addClass("d-none");

    $.ajax({
      url: "/Hospitalizacion/ConsultarDepositosHospitalizacion",
      method: "POST",
      data: { cuentaId: cuentaId },
      success: function (dataResult) {
        var data =
          typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
        if (data.Exitoso) {
          var lista = (data.Resultado || []).map(function (item) {
            return {
              Fecha: item.FechaHora || item.Fecha || "-",
              Monto: item.Monto,
              FormaPago: item.FormaPago,
            };
          });
          self.depositos(lista);
          self._depositosCargados = true;
        } else {
          $("#texto-error-consultar-depositos").removeClass("d-none");
        }
      },
      error: function () {
        $("#texto-error-consultar-depositos").removeClass("d-none");
      },
    });
  };
};

var detallesVm = new CuentasPorCobrarDetallesVM();
ko.applyBindings(detallesVm);

$(document).ready(function () {
  $("#tabs-datos-cuenta").tabs();
});
