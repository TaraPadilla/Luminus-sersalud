var HomeVM = function () {
  var self = this;
  self.citas = ko.observableArray();
  self.servicios = ko.observableArray();
  self.reconsultas = ko.observableArray();
  self.habitacionesOcupadas = ko.observableArray();
  self.cuentasPendientes = ko.observableArray();
  self.pacientesCumpleannios = ko.observableArray();
  self.pacientesAplicablesMembresia = ko.observableArray();
  self.pacientesContacto = ko.observableArray();
  self.pacientesAniversario = ko.observableArray();
  self.alertasVacunas = ko.observableArray(); //variable para observar las alertasde las vacunas
  self.citasProximas = ko.observableArray();
  self.servicios = ko.observableArray();

  //#region Variables TAB CLINICA
  self.examenesFinalizados = ko.observableArray();
  self.clinicaInventarioStockMinimo = ko.observableArray();
  self.clinicaInventarioProximosVencer = ko.observableArray();
  self.clinicaInventarioVencidos = ko.observableArray();
  //#endregion

  //#region Variables TAB COMPRAS
  self.comprasStockMinimo = ko.observableArray();
  self.cuentasPorPagarHospital = ko.observableArray();
  self.cuentasPorPagarClinica = ko.observableArray();
  self.cuentasPorPagarFarmacia = ko.observableArray();
  self.cuentasPorPagarLaboratorio = ko.observableArray();
  //#endregion

  //#region Variables TAB HOSPITAL
  self.hospitalInventarioStockMinimo = ko.observableArray();
  self.hospitalInventarioProximosVencer = ko.observableArray();
  self.hospitalInventarioVencidos = ko.observableArray();
  //#endregion

  //#region Variables TAB FARMACIA
  self.farmaciaInventarioStockMinimo = ko.observableArray();
  self.farmaciaInventarioProximosVencer = ko.observableArray();
  self.farmaciaInventarioVencidos = ko.observableArray();
  //#endregion

  //#region Variables TAB LABORATORIO
  self.examenesSolicitados = ko.observableArray();
  self.laboratorioInventarioStockMinimo = ko.observableArray();
  self.laboratorioInventarioVencidos = ko.observableArray();
  self.laboratorioInventarioProximosVencer = ko.observableArray();
  //#endregion

  //#region Variables TABS Abiertas
  self.tabAbiertaGeneral = ko.observable(true); // Este tab esta en verdadero porque es el inicial, el que por defecto esta activo
  self.tabAbiertaHospital = ko.observable(false);
  self.tabAbiertaLaboratorio = ko.observable(false);
  self.tabAbiertaClinica = ko.observable(false);
  self.tabAbiertaFarmacia = ko.observable(false);
  self.tabAbiertaRadiologia = ko.observable(false);
  self.tabAbiertaCompras = ko.observable(false);
  //#endregion

  //#region Metodos para abrir TABS
  self.abrirTabHospital = function () {
    if (!self.tabAbiertaHospital()) {
      self.consultarHabitacionesOcupadas();
      self.consultarHospitalInventarioStockMinimo();
      self.consultarHospitalInventarioVencidos();
      self.consultarHospitalInventarioProximosVencer();
      self.tabAbiertaHospital(true);
    }
  };
  self.abrirTabLaboratorio = function () {
    if (!self.tabAbiertaLaboratorio()) {
      self.consultarLaboratorioExamenesSolicitados();
      self.consultarLaboratorioInventarioStockMinimo();
      self.consultarLaboratorioInventarioVencidos();
      self.consultarLaboratorioInventarioProximosVencer();
      self.tabAbiertaLaboratorio(true);
    }
  };
  self.abrirTabClinica = function () {
    if (!self.tabAbiertaClinica()) {
      self.consultarClinicaExamenesFinalizados();
      self.consultarClinicaInventarioStockMinimo();
      self.consultarClinicaInventarioVencidos();
      self.consultarClinicaInventarioProximosVencer();
      self.tabAbiertaClinica(true);
    }
  };
  self.abrirTabFarmacia = function () {
    if (!self.tabAbiertaFarmacia()) {
      self.consultarFarmaciaInventarioStockMinimo();
      self.consultarFarmaciaInventarioVencidos();
      self.consultarFarmaciaInventarioProximosVencer();
      self.tabAbiertaFarmacia(true);
    }
  };
  self.abrirTabCompras = function () {
    if (!self.tabAbiertaCompras()) {
      self.consultarComprasCuentasPagar();
      self.consultarComprasStockMinimo();
      self.tabAbiertaCompras(true);
    }
  };
  //#endregion

  //Metodo para obserbar las citas 3 dias posteriores
  self.consultarCitasProximas = function () {
    showLoading();
    clearDataTable("tabla-citas-proximas");

    $.ajax({
      method: "POST",
      url: "/Home/ConsultarCitasProximas",
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          /*self.servicios(data.Resultado.servicios);*/
          self.citasProximas(data.Resultado);

          drawDataTable("tabla-citas-proximas");
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        console.log(data);
        alert(data);
      },
    });
  };

  self.consultarReconsultas = function () {
    showLoading();
    clearDataTable("tabla-reconsultas");

    $.ajax({
      method: "POST",
      url: "/Home/ConsultarReconsultas",
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.reconsultas(data.Resultado);

          drawDataTable("tabla-reconsultas");
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        console.log(data);
        alert(data);
      },
    });
  };

  //Dibujar la tabla de alerta vacunas
  self.consultarAlertaVacunasPaciente = function () {
    showLoading();
    clearDataTable("tabla-alertas-vacunas");
    $.ajax({
      method: "POST",
      url: "/Home/ConsultarAlertaVacunas",
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.alertasVacunas(data.Resultado);
          drawDataTable("tabla-alertas-vacunas");
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        console.log(data);
        alert(data);
      },
    });
  };

  self.consultarCuentasPendientes = function () {
    $("#div-loading").show();

    $.ajax({
      method: "POST",
      url: "/CuentasPorCobrar/ConsultarCuentasPorCobrar",
      success: function (data, textStatus) {
        $("#div-loading").hide();
        if (data.exitoso) {
          //debugger;
          //var cuentasPendientesProximoPago = new Array();
          //var fechaActual = moment();
          //$(data.resultado).each(function (idx, vl) {

          //    //var diferencia = moment(vl.fechaLimitePago).diff(fechaActual, 'day');
          //    //if (diferencia >= 0 && diferencia <= 3) {
          //    //    cuentasPendientesProximoPago.push(vl);
          //    //}
          //    cuentasPendientesProximoPago.push(vl);
          //});
          self.cuentasPendientes(data.resultado);
          self.drawTableCuentasPendientes();
        } else {
          alert(data.mensaje);
        }
      },
      error: function (data) {
        $("#div-loading").hide();
        alert(data.error);
      },
    });
  };

  self.consultarPacientesCumpleannios = function () {
    showLoading();
    clearDataTable("tabla-pacientes-cumpleannios");

    $.ajax({
      method: "POST",
      url: "/Home/ConsultarPacientesCumpleannios",
      success: function (data, textStatus) {
        $("#div-loading").hide();

        if (data.exitoso) {
          self.pacientesCumpleannios(data.resultado);
          drawDataTable("tabla-pacientes-cumpleannios");
        } else {
          alert(data.mensaje);
        }
      },
      error: function (data) {
        $("#div-loading").hide();
        alert(data.error);
      },
    });
  };
  self.consultarPacientesAplicablesMembresia = function () {
    $("#div-loading").show();
    self.clearTablePacientesAplicablesMembresia();

    $.ajax({
      method: "POST",
      url: "/Home/ConsultarPacientesAplicablesMembresia",
      success: function (data) {
        $("#div-loading").hide();
        if (data.exitoso) {
          self.pacientesAplicablesMembresia(data.resultado);
          self.drawTablePacientesAplicablesMembresia();
        } else alert(data.mensaje);
      },
      error: function (data) {
        $("#div-loading").hide();
        alert(data.error);
      },
    });
  };
  self.consultarPacientesContacto = function () {
    $("#div-loading").show();
    self.clearTablePacientesContacto();

    $.ajax({
      method: "POST",
      url: "/Home/ConsultarPacientesRetiradosContactar",
      success: function (data) {
        $("#div-loading").hide();
        if (data.exitoso) {
          self.pacientesContacto(data.resultado);
          self.drawTablePacientesContacto();
        } else alert(data.mensaje);
      },
      error: function (data) {
        $("#div-loading").hide();
        alert(data.error);
      },
    });
  };
  self.consultarPacientesAniversario = function () {
    $("#div-loading").show();
    self.clearTablePacientesAniversario();

    $.ajax({
      method: "POST",
      url: "/Home/ConsultarPacientesAniversario",
      success: function (data) {
        $("#div-loading").hide();
        if (data.exitoso) {
          self.pacientesAniversario(data.resultado);
          self.drawTablePacientesAniversario();
        } else alert(data.mensaje);
      },
      error: function (data) {
        $("#div-loading").hide();
        alert(data.error);
      },
    });
  };
  self.aplicarMembresia = function (value) {
    $("#div-loading").show();
    window.location.href = "/Pacientes/AplicarMembresia?pacienteId=" + value.id;
  };

  //#region Iniciar tablas TAB HOSPITAL
  self.consultarHabitacionesOcupadas = function () {
    showLoading();
    clearDataTable("tabla-habitaciones-ocupadas");

    $.ajax({
      method: "POST",
      url: "/Home/ConsultarHabitacionesOcupadas",
      success: function (dataResult) {
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          showLoading();
          self.habitacionesOcupadas(data.Resultado);
          drawDataTable("tabla-habitaciones-ocupadas");
          hideLoading();
        } else {
          alert(data.Mensaje);
          hideLoading();
        }
      },
      error: function (data) {
        hideLoading();
        console.log(data);
        alert(data);
      },
    });
  };
  self.consultarHospitalInventarioStockMinimo = function () {
    showLoading();
    clearDataTable("tabla-hospital-inventario-stock-minimo");
    $.ajax({
      url: "/Home/ConsultarHospitalInventarioStockMinimo",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.hospitalInventarioStockMinimo(result.Resultado);
          drawDataTable("tabla-hospital-inventario-stock-minimo");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.consultarHospitalInventarioVencidos = function () {
    showLoading();
    clearDataTable("tabla-hospital-inventario-vencidos");
    $.ajax({
      url: "/Home/ConsultarHospitalInventarioVencidos",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.hospitalInventarioVencidos(result.Resultado);
          drawDataTable("tabla-hospital-inventario-vencidos");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.consultarHospitalInventarioProximosVencer = function () {
    showLoading();
    clearDataTable("tabla-hospital-inventario-proximos-vencer");
    $.ajax({
      url: "/Home/ConsultarHospitalInventarioProximosVencer",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.hospitalInventarioProximosVencer(result.Resultado);
          drawDataTable("tabla-hospital-inventario-proximos-vencer");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  //#endregion

  //#region Funciones consultas TAB FARMACIA
  self.consultarFarmaciaInventarioStockMinimo = function () {
    showLoading();
    clearDataTable("tabla-farmacia-inventario-stock-minimo");
    $.ajax({
      url: "/Home/ConsultarFarmaciaInventarioStockMinimo",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.farmaciaInventarioStockMinimo(result.Resultado);
          drawDataTable("tabla-farmacia-inventario-stock-minimo");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.consultarFarmaciaInventarioVencidos = function () {
    showLoading();
    clearDataTable("tabla-farmacia-inventario-vencidos");
    $.ajax({
      url: "/Home/ConsultarFarmaciaInventarioVencidos",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.farmaciaInventarioVencidos(result.Resultado);
          drawDataTable("tabla-farmacia-inventario-vencidos");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.consultarFarmaciaInventarioProximosVencer = function () {
    showLoading();
    clearDataTable("tabla-farmacia-inventario-proximos-vencer");
    $.ajax({
      url: "/Home/ConsultarFarmaciaInventarioProximosVencer",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.farmaciaInventarioProximosVencer(result.Resultado);
          drawDataTable("tabla-farmacia-inventario-proximos-vencer");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  //#endregion

  //#region Funciones consultas TAB LABORATORIO
  self.consultarLaboratorioExamenesSolicitados = function () {
    showLoading();
    clearDataTable("tabla-laboratorio-examenes-solicitados");
    $.ajax({
      url: "/Home/ConsultarExamenesSolicitados",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.examenesSolicitados(result.Resultado);
          drawDataTable("tabla-laboratorio-examenes-solicitados");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.consultarLaboratorioInventarioStockMinimo = function () {
    showLoading();
    clearDataTable("tabla-laboratorio-inventario-stock-minimo");
    $.ajax({
      url: "/Home/ConsultarLaboratorioInventarioStockMinimo",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.laboratorioInventarioStockMinimo(result.Resultado);
          drawDataTable("tabla-laboratorio-inventario-stock-minimo");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.consultarLaboratorioInventarioVencidos = function () {
    showLoading();
    clearDataTable("tabla-laboratorio-inventario-vencidos");
    $.ajax({
      url: "/Home/ConsultarLaboratorioInventarioVencidos",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.laboratorioInventarioVencidos(result.Resultado);
          drawDataTable("tabla-laboratorio-inventario-vencidos");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.consultarLaboratorioInventarioProximosVencer = function () {
    showLoading();
    clearDataTable("tabla-laboratorio-inventario-proximos-vencer");
    $.ajax({
      url: "/Home/ConsultarLaboratorioInventarioProximosVencer",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.laboratorioInventarioProximosVencer(result.Resultado);
          drawDataTable("tabla-laboratorio-inventario-proximos-vencer");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  //#endregion

  //#region Funciones consultas TAB CLINICA
  self.consultarClinicaExamenesFinalizados = function () {
    showLoading();
    clearDataTable("tabla-clinica-examenes-finalizados");
    $.ajax({
      url: "/Home/ConsultarExamenesFinalizados",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.examenesFinalizados(result.Resultado);
          drawDataTable("tabla-clinica-examenes-finalizados");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.consultarClinicaInventarioStockMinimo = function () {
    showLoading();
    clearDataTable("tabla-clinica-inventario-stock-minimo");
    $.ajax({
      url: "/Home/ConsultarClinicaInventarioStockMinimo",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.clinicaInventarioStockMinimo(result.Resultado);
          drawDataTable("tabla-clinica-inventario-stock-minimo");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.consultarClinicaInventarioVencidos = function () {
    showLoading();
    clearDataTable("tabla-clinica-inventario-vencidos");
    $.ajax({
      url: "/Home/ConsultarClinicaInventarioVencidos",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.clinicaInventarioVencidos(result.Resultado);
          drawDataTable("tabla-clinica-inventario-vencidos");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.consultarClinicaInventarioProximosVencer = function () {
    showLoading();
    clearDataTable("tabla-clinica-inventario-proximos-vencer");
    $.ajax({
      url: "/Home/ConsultarClinicaInventarioProximosVencer",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.clinicaInventarioProximosVencer(result.Resultado);
          drawDataTable("tabla-clinica-inventario-proximos-vencer");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  //#endregion

  //#region Funciones consultas TAB COMPRAS
  self.consultarComprasCuentasPagar = function () {
    showLoading();
    clearDataTable("tabla-compras-cuentas-pagar-hospital");
    clearDataTable("tabla-compras-cuentas-pagar-clinica");
    clearDataTable("tabla-compras-cuentas-pagar-farmacia");
    clearDataTable("tabla-compras-cuentas-pagar-laboratorio");
    clearDataTable("tabla-compras-cuentas-pagar-todos-ambientes");
    $.ajax({
      url: "/Home/ConsultarCuentasPorPagar",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          $(result.Resultado).each(function (idx, vl) {
            switch (vl.AmbienteNombre) {
              case "Hospital":
                self.cuentasPorPagarHospital.push(vl);
                break;
              case "Clinica":
                self.cuentasPorPagarClinica.push(vl);
                break;
              case "Farmacia":
                self.cuentasPorPagarFarmacia.push(vl);
                break;
              case "Laboratorio":
                self.cuentasPorPagarLaboratorio.push(vl);
                break;
              default:
                break;
            }
          });
          drawDataTable("tabla-compras-cuentas-pagar-laboratorio");
          drawDataTable("tabla-compras-cuentas-pagar-hospital");
          drawDataTable("tabla-compras-cuentas-pagar-clinica");
          drawDataTable("tabla-compras-cuentas-pagar-farmacia");
          drawDataTable("tabla-compras-cuentas-pagar-todos-ambientes");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  self.sendEmailReminder = function (item) {
    // Formatea la fecha como tú quieras
    const fechaFormateada = moment(item.FechaInicio).format("DD/MM/YYYY HH:mm");
    // Llama al helper
    enviarRecordatorioEmail(
      item.Id,
      item.PacienteNombre,
      item.Email,
      fechaFormateada
    );
  };
  self.consultarComprasStockMinimo = function () {
    showLoading();
    clearDataTable("tabla-compras-stock-minimo");
    $.ajax({
      url: "/Home/ConsultarComprasStockMinimo",
      method: "POST",
      success: function (data) {
        let result = JSON.parse(data);
        if (result.Exitoso) {
          self.comprasStockMinimo(result.Resultado);
          drawDataTable("tabla-compras-stock-minimo");
          hideLoading();
        } else {
          mensajeEmergenteError(result.Mensaje);
          hideLoading();
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError(
          "Error de servidor. Comuniquese con su administrador"
        );
      },
    });
  };
  //#endregion

  self.verDetallesHospitalizacion = function (value) {
    window.location.href =
      "/Hospitalizacion/Detalles?hospitalizacionId=" + value.HospitalizacionId;
  };

  self.verDetallesCuenta = function (cuenta) {
    debugger;
    var url = "CuentasPorCobrar/VerDetallesCuenta?cuentaId=" + cuenta.id;
    window.location.href = url;
  };
  self.pagarCuenta = function (value) {
    console.log(value);
  };

  //Tables
  self.clearTableCuentasPendientes = function () {
    var table = $("#tabla-cuentas-pendientes").DataTable();
    table.clear().draw();

    $("#tabla-cuentas-pendientes").dataTable().fnDestroy();
  };
  self.drawTableCuentasPendientes = function () {
    $("#tabla-cuentas-pendientes").DataTable({
      searching: true,
      ordering: true,
      paging: true,
      lengthMenu: [
        [5, 10, 25, 50, -1],
        ["5", "10", "25", "50", "Todos"],
      ],
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
  };

  self.clearTableCitas = function () {
    var table = $("#tabla-citas").DataTable();
    table.clear().draw();

    $("#tabla-citas").dataTable().fnDestroy();
  };

  self.drawTableCitas = function () {
    $("#tabla-citas").DataTable({
      searching: true,
      ordering: true,
      paging: true,
      lengthMenu: [
        [5, 10, 25, 50, -1],
        ["5", "10", "25", "50", "Todos"],
      ],
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
  };

  self.clearTablePacientesCumpleannios = function () {
    var table = $("#tabla-pacientes-cumpleannios").DataTable();
    table.clear().draw();

    $("#tabla-pacientes-cumpleannios").dataTable().fnDestroy();
  };
  self.drawTablePacientesCumpleannios = function () {
    $("#tabla-pacientes-cumpleannios").DataTable({
      searching: true,
      ordering: true,
      paging: true,
      lengthMenu: [
        [5, 10, 25, 50, -1],
        ["5", "10", "25", "50", "Todos"],
      ],
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
  };
  self.clearTablePacientesAplicablesMembresia = function () {
    var table = $("#tabla-pacientes-aplicables-membresia").DataTable();
    table.clear().draw();

    $("#tabla-pacientes-aplicables-membresia").dataTable().fnDestroy();
  };
  self.drawTablePacientesAplicablesMembresia = function () {
    $("#tabla-pacientes-aplicables-membresia").DataTable({
      searching: true,
      ordering: true,
      paging: true,
      lengthMenu: [
        [5, 10, 25, 50, -1],
        ["5", "10", "25", "50", "Todos"],
      ],
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
  };
  self.clearTablePacientesAniversario = function () {
    var table = $("#tabla-pacientes-aniversario").DataTable();
    table.clear().draw();

    $("#tabla-pacientes-aniversario").dataTable().fnDestroy();
  };
  self.drawTablePacientesAniversario = function () {
    $("#tabla-pacientes-aniversario").DataTable({
      searching: true,
      ordering: true,
      paging: true,
      lengthMenu: [
        [5, 10, 25, 50, -1],
        ["5", "10", "25", "50", "Todos"],
      ],
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
  };
  self.clearTablePacientesContacto = function () {
    var table = $("#tabla-pacientes-contacto").DataTable();
    table.clear().draw();

    $("#tabla-pacientes-contacto").dataTable().fnDestroy();
  };
  self.drawTablePacientesContacto = function () {
    $("#tabla-pacientes-contacto").DataTable({
      searching: true,
      ordering: true,
      paging: true,
      lengthMenu: [
        [5, 10, 25, 50, -1],
        ["5", "10", "25", "50", "Todos"],
      ],
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
  };
};
async function getWhatsAppToken() {
  try {
    const response = await fetch("/api/whatsapptoken");
    if (response.ok) {
      const token = await response.text();
      return token; // Devuelve el token
    } else {
      console.error("Error al obtener el token de WhatsApp");
      return null;
    }
  } catch (error) {
    console.error("Error al obtener el token de WhatsApp:", error);
    return null;
  }
}
async function enviarRecordatorioWhatsApp(
  citaId,
  nombrePaciente,
  celularPaciente,
  fechaCita
) {
  const token = await getWhatsAppToken();

  if (!celularPaciente) {
    alert("El paciente no tiene un número de celular registrado.");
    return;
  }

  // Ajusta prefijo para Honduras:
  const formattedPhoneNumber = `502${celularPaciente}`;

  // Aquí ya NO generas PDF ni subes nada, simplemente mandas el template:
  const messageBody = {
    messaging_product: "whatsapp",
    to: formattedPhoneNumber,
    type: "template",
    template: {
      name: "recordatorio_cita", // tu plantilla de recordatorio
      language: { code: "es" },
      components: [
        {
          type: "body",
          parameters: [
            { type: "text", text: nombrePaciente },
            { type: "text", text: fechaCita },
          ],
        },
      ],
    },
  };

  const whatsappResponse = await fetch(
    "https://graph.facebook.com/v20.0/TU_PHONE_NUMBER_ID/messages",
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(messageBody),
    }
  );

  if (whatsappResponse.ok) {
    alert("✅ Recordatorio enviado por WhatsApp");
  } else {
    alert("❌ Error al enviar el recordatorio");
  }
}
// 1) Helper fuera de HomeVM:
async function enviarRecordatorioEmail(
  citaId,
  nombrePaciente,
  emailPaciente,
  fechaCita
) {
  if (!emailPaciente) {
    return alert("El paciente no tiene un correo electrónico registrado.");
  }

  // Opcional: si quieres usar una plantilla HTML ligera
  let plantilla = await fetch(
    "../js-utilidades-wp-email/email-plantilla-cita.html"
  ).then((r) => r.text());
  // Reemplaza marcadores por valores reales
  let bodyMessage = plantilla
    .replace("{{nombrePaciente}}", nombrePaciente)
    .replace("{{fechaCita}}", fechaCita);

  // Prepara el FormData para tu API de correo
  const formData = new FormData();
  formData.append("To", emailPaciente);
  formData.append("Subject", "Recordatorio de cita");
  formData.append("Body", bodyMessage);

  // Llama a tu endpoint que envía correos
  const resp = await fetch("/api/SendEmail", {
    method: "POST",
    body: formData,
  });

  if (resp.ok) {
    alert("✅ Correo de recordatorio enviado exitosamente.");
  } else {
    alert("❌ Error al enviar el correo de recordatorio.");
  }
}

var homeVM = new HomeVM();
ko.applyBindings(homeVM);

$(document).ready(function () {
  $("#tabs").tabs();
  $("#subtabs-general").tabs();
  $("#subtabs-hospital").tabs();
  $("#subtabs-laboratorio").tabs();
  $("#subtabs-clinica").tabs();
  $("#subtabs-farmacia").tabs();
  $("#subtabs-compras").tabs();

  homeVM.consultarCitasProximas();
  homeVM.consultarReconsultas();
  homeVM.consultarAlertaVacunasPaciente();
  homeVM.consultarPacientesCumpleannios();
  homeVM.consultarPacientesAplicablesMembresia();
  homeVM.consultarPacientesAniversario();
  homeVM.consultarPacientesContacto();
  homeVM.consultarCuentasPendientes();
});
