var CitaVM = function (serverModel) {
  let itemServicio = 1;
  let itemExamenes = 1;
  let model = {};
  var self = this;
  self.txtModalRespuesta = ko.observable();
  self.txtModalConfirmacion = ko.observable();
  self.preciosServicioSeleccionado = ko.observableArray();
  self.servicioAgregarSeleccionado = ko.observable();
  self.precioServicioAgregarSeleccionado = ko.observable();
  self.serviciosExistentes = ko.observableArray();
  self.serviciosAgregados = ko.observableArray();
  self.totalDuracion = ko.observable();
  self.totalDuracionHoras = ko.observable();
  self.totalDuracionMinutos = ko.observable();
  self.totalValor = ko.observable();
  self.citaTipoAtencion = ko.observable();
  //Pacientes
  self.pacientes = ko.observableArray();
  self.pacienteSeleccionado = ko.observable();
  self.pacienteSeleccionadoNombre = ko.observable();
  //Examenes
  self.examenesExistentes = ko.observableArray();
  self.examenSeleccionado = ko.observable();
  self.preciosExamen = ko.observableArray();
  self.precioSeleccionadoExamen = ko.observable();
  self.examenesAgregados = ko.observableArray();
  let itemExamen = 1;
  //Departamentos y Municipios
  self.ejecutarSubscribeDepartamentos = ko.observable(false);
  self.departamentosExistentes = ko.observableArray();
  self.departamentoSeleccionado = ko.observable();

  self.municipiosExistentes = ko.observableArray();
  self.municipioSeleccionado = ko.observable();
  self.departamentoPaciente = ko.observable();
  self.municipioPaciente = ko.observable();
  //Buscar
  self.nombreCodigoExamenBuscar = ko.observable();

  self.cirujanoNombre = ko.observable();

  self.anestesistaId = ko.observable();
  self.primerAyudanteId = ko.observable();
  self.segundoAyudanteId = ko.observable();
  self.instrumentistaId = ko.observable();
  self.circulanteId = ko.observable();

  // Observables para las listas de opciones
  self.anestesistaOptions = ko.observableArray([]);
  self.primerAyudanteOptions = ko.observableArray([]);
  self.segundoAyudanteOptions = ko.observableArray([]);
  self.instrumentistaOptions = ko.observableArray([]);
  self.circulanteOptions = ko.observableArray([]);

  self.habitacionesOptions = ko.observableArray([]);
  self.habitacionSeleccionadaId = ko.observable();

  var initialAnestesistaId = null;
  var initialPrimerAyudanteId = null;
  var initialSegundoAyudanteId = null;
  var initialInstrumentistaId = null;
  var initialCirculanteId = null;

  if (serverModel) {
    initialAnestesistaId = serverModel.AnestesistaId;
    initialPrimerAyudanteId = serverModel.PrimerAyudanteId;
    initialSegundoAyudanteId = serverModel.SegundoAyudanteId;
    initialInstrumentistaId = serverModel.InstrumentistaId;
    initialCirculanteId = serverModel.CirculanteId;
  }

  self.pacientes.subscribe(function () {
    syncSelectPacienteCita();
  });

  self.departamentosExistentes.subscribe(function () {
    refreshKoSelect2ById("departamentoSeleccionado", "Seleccione...");
  });

  self.municipiosExistentes.subscribe(function () {
    syncSelectMunicipioCita();
  });

  self.municipioSeleccionado.subscribe(function (val) {
    var $sel = $("#municipioSeleccionado");
    if (!$sel.length) return;
    var id = val != null && val !== "" ? String(val) : "";
    if ($sel.val() !== id) {
      $sel.val(id);
      if ($sel.hasClass("select2-hidden-accessible")) {
        $sel.trigger("change.select2");
      }
    }
  });

  //FUNCIONES VIEWMODEL
  self.pacienteSeleccionado.subscribe(function (paciente) {
    if (paciente == null || paciente == undefined || paciente === "") {
      self.pacienteSeleccionadoNombre("");
      $("#PacienteNombre").val("");
      $("#PacienteId").val("");
      $("#dpiPacienteSeleccionado").val("");
      $("#SexoId").val("");
      $("#Telefono").val("");
      $("#Email").val("");
      $("#Direccion").val("");
      $("#no_IGGS").val("");
      $("#FechaNacimiento").val("");
      $("#PacienteEdad").val("");
      $("#EtniaPaciente").val("");
      $("#OrigenPaciente").val("");
      self.departamentoSeleccionado(null);
      self.municipioSeleccionado(null);
      self.departamentoPaciente(null);
      self.municipioPaciente(null);
    } else if (paciente.Id == null || paciente.Id == undefined) {
      self.pacienteSeleccionadoNombre(paciente);
      $("#PacienteNombre").val(paciente);
      $("#PacienteId").val("");
      $("#dpiPacienteSeleccionado").val("");
      $("#SexoId").val("");
      $("#Telefono").val("");
      $("#Email").val("");
      $("#Direccion").val("");
      $("#no_IGGS").val("");
      $("#FechaNacimiento").val("");
      $("#PacienteEdad").val("");
      $("#EtniaPaciente").val("");
      $("#OrigenPaciente").val("");
      self.departamentoSeleccionado(null);
      self.municipioSeleccionado(null);
      self.departamentoPaciente(null);
      self.municipioPaciente(null);
    } else {
      $("#PacienteNombre").val(paciente.Nombre);
      $("#PacienteId").val(paciente.Id);
      self.pacienteSeleccionadoNombre(paciente.Nombre);
      $("#dpiPacienteSeleccionado").val(paciente.Dpi);
      $("#SexoId").val(paciente.SexoId);
      $("#Telefono").val(paciente.Telefono);
      $("#Direccion").val(paciente.Direccion);
      $("#Email").val(paciente.Email);
      $("#EtniaPaciente").val(paciente.EtniaPaciente);
      $("#OrigenPaciente").val(paciente.OrigenPaciente);
      $("#no_IGGS").val(paciente.IgssNumeroAfiliacion);
      $("#FechaNacimiento").val(paciente.FechaNacimiento);
      getEdad();
      self.departamentoPaciente(paciente.DepartamentoId);
      self.municipioPaciente(paciente.MunicipioId);
      self.departamentoSeleccionado(Number(paciente.DepartamentoId));
      setTimeout(function () {
        $("#departamentoSeleccionado").trigger("change");
      }, 100);
    }
  });
  if (window.serverModel) {
    $("#PacienteId").val(serverModel.PacienteId);
    $("#PacienteNombre").val(serverModel.PacienteNombre);
    $("#SexoId").val(serverModel.SexoId);
    $("#Telefono").val(serverModel.Telefono);
    $("#Email").val(serverModel.Email);
    $("#Direccion").val(serverModel.Direccion);
    $("#EtniaPaciente").val(serverModel.EtniaPaciente);
    $("#OrigenPaciente").val(serverModel.OrigenPaciente);
    $("#FechaNacimiento").val(serverModel.FechaNacimiento);
    getEdad();
    $("#departamentoSeleccionado")
      .val(serverModel.DepartamentoId)
      .trigger("change");
    self.departamentoPaciente(serverModel.DepartamentoId);
    self.municipioPaciente(serverModel.MunicipioId);
    self.consultarMunicipiosExistentes(serverModel.DepartamentoId);
  }

  self.consultarServiciosExistentes = function () {
    showLoading();
    self.serviciosAgregados([]);
    $.ajax({
      url: "/Cita/ConsultarServiciosExistentes",
      method: "POST",
      data: model,
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.serviciosExistentes(data.Resultado);
          if (
            self.servicioAgregarSeleccionado() != null &&
            self.servicioAgregarSeleccionado() != undefined
          ) {
            self.consultarPreciosServicio(
              self.servicioAgregarSeleccionado().ServicioId
            );
          }
          setTimeout(function () {
            syncSelectServicioCita();
          }, 0);
        } else {
          console.warn("[COEX][Servicios] Backend respondió error:", data.Mensaje);
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        alert(dataError);
      },
    });
  };

  self.consultarPreciosServicio = function (servicioId) {
    if (servicioId == null || servicioId == undefined) {
      return false;
    }
    showLoading();
    $.ajax({
      url: "/Cita/ConsultarPreciosServicio",
      method: "POST",
      data: {
        servicioId: servicioId,
        fecha: $("#FechaHora").val(),
      },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.preciosServicioSeleccionado(data.Resultado);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert(dataError);
      },
    });
  };

  let pacienteId = parseInt($("#PacienteId").val(), 10);
  if (!isNaN(pacienteId)) {
    $("#selectPaciente")
      .val(pacienteId)
      .trigger("change");
    let obj = self.pacientes().find((p) => p.Id === pacienteId);
    if (obj) {
      self.pacienteSeleccionado(obj);
    }
  }

  self.consultarPacientes = function () {
    let textoCargando = $("#texto-cargando-pacientes");
    let textoError = $("#texto-error-consultar-pacientes");
    textoCargando.show();
    textoError.hide();
    $.ajax({
      url: "/Cita/ConsultarPacientes",
      method: "POST",
      success: function (dataResult) {
        textoCargando.hide();
        let data = JSON.parse(dataResult);
        if (!data.Exitoso) {
          return textoError.show();
        }
        self.pacientes(data.Resultado);
        let pacienteId = parseInt($("#PacienteId").val(), 10);
        if (
          pacienteId &&
          !self.pacientes().some((p) => p.Id === pacienteId) &&
          typeof window.serverModel !== "undefined"
        ) {
          let pacienteActual = {
            Id: pacienteId,
            Nombre: serverModel.PacienteNombre,
            Dpi: serverModel.dpiPacienteSeleccionado,
            SexoId: serverModel.SexoId,
            Telefono: serverModel.Telefono,
            Email: serverModel.Email,
            Direccion: serverModel.Direccion,
            EtniaPaciente: serverModel.EtniaPaciente,
            OrigenPaciente: serverModel.OrigenPaciente,
            DepartamentoId: serverModel.DepartamentoId,
            MunicipioId: serverModel.MunicipioId,
            IgssNumeroAfiliacion: serverModel.no_IGGS,
            FechaNacimiento: serverModel.FechaNacimiento,
          };
          self.pacientes.push(pacienteActual);
        }
        if (!isNaN(pacienteId)) {
          var seleccionado = self.pacientes().find((p) => p.Id === pacienteId);
          if (seleccionado) {
            self.pacienteSeleccionado(seleccionado);
          }
        }
        syncSelectPacienteCita();
      },
      error: function (dataError) {
        textoCargando.hide();
        textoError.show();
        console.error(dataError);
      },
    });
  };

  self.consultarDepartamentosExistentes = function () {
    showLoading();
    $.ajax({
      url: "/Cita/ConsultarDepartamentosExistentes",
      method: "POST",
      data: model,
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          var lista = data.Resultado.map((d) => ({
            Id: d.Id,
            NombreDepartamento: d.NombreDepartamento,
            DepartamentoNombreMostrar: `${d.Id} - ${d.NombreDepartamento}`,
          }));
          self.departamentosExistentes(lista);
          if (self.departamentoPaciente()) {
            self.departamentoSeleccionado(Number(self.departamentoPaciente()));
            self.consultarMunicipiosExistentes(
              Number(self.departamentoPaciente())
            );
          }
          setTimeout(function () {
            refreshKoSelect2ById("departamentoSeleccionado", "Seleccione...");
          }, 0);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (e) {
        hideLoading();
        console.error(e);
        alert("Error en AJAX departamentosen");
      },
    });
  };

  self.departamentoSeleccionado.subscribe(function (deptoId) {
    if (deptoId) {
      self.consultarMunicipiosExistentes(deptoId);
    } else {
      self.municipiosExistentes([]);
      self.municipioSeleccionado(null);
    }
  });

  self.departamentosExistentes.subscribe(function (nuevaLista) {
    if (
      self.pacienteSeleccionado() &&
      self.pacienteSeleccionado().DepartamentoId
    ) {
      self.departamentoSeleccionado(
        Number(self.pacienteSeleccionado().DepartamentoId)
      );
      setTimeout(function () {
        $("#departamentoSeleccionado").trigger("change");
      }, 100);
    }
  });

  self.consultarMunicipiosExistentes = function (deptoId) {
    $.ajax({
      url: "/Cita/ConsultarMunicipiosExistentes",
      method: "POST",
      data: { departamentoId: deptoId },
      success: function (res) {
        var data = JSON.parse(res);
        if (data.Exitoso) {
          var lista = (data.Resultado || []).map(function (m) {
            return {
              Id: m.Id,
              NombreMunicipio: m.NombreMunicipio,
              DepartamentoId: m.DepartamentoId,
              MunicipioNombreMostrar: m.MunicipioNombreMostrar
                || ((m.Id || "") + " - " + (m.NombreMunicipio || "")),
            };
          });
          self.municipiosExistentes(lista);
          if (self.municipioPaciente()) {
            self.municipioSeleccionado(self.municipioPaciente());
          }
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (e) {
        console.error(e);
        alert("Error cargando municipios");
      },
    });
  };

  self.nombreCodigoExamenBuscar.subscribe(function (value) {
    self.buscarExamenNombreCodig();
  });
  self.buscarExamenNombreCodig = function () {
    $(self.examenesExistentes()).each(function (idx, vl) {
      if (
        self.nombreCodigoExamenBuscar().toLowerCase() ==
        vl.ExamenNombre.toLowerCase() ||
        self.nombreCodigoExamenBuscar() == vl.ExamenCodigo.toString()
      ) {
        self.examenSeleccionado(vl);
      }
    });
  };

  self.consultarServiciosAgregadosCita = function () {
    showLoading();
    self.serviciosAgregados([]);
    $.ajax({
      url: "/Cita/ConsultarServiciosAgregadosCita",
      method: "POST",
      data: {
        citaId: $("#CitaId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          $(data.Resultado).each(function (idx, servicio) {
            servicio.Item = itemServicio;
            servicio.Nuevo = false;
            servicio.PrecioNombre = servicio.PrecioNombre;
            servicio.PrecioNombreValor =
              servicio.PrecioNombre + " - " + servicio.PrecioValor;
            servicio.PrecioValor = ko.observable(servicio.PrecioValor);
            servicio.PrecioValorCubiertoSeguro = ko.observable(
              servicio.PrecioValorCubiertoSeguro
            );
            servicio.PrecioValorCopago = ko.observable(
              servicio.PrecioValorCopago
            );
            servicio.ServicioDuracionText =
              servicio.ServicioDuracionHoras +
              "h" +
              servicio.ServicioDuracionMinutos +
              "m";
            self.serviciosAgregados.push(servicio);
            itemServicio++;
          });
          self.actualizarTotales();
        } else {
          self.txtModalRespuesta(data.Mensaje);
          showModalRespuesta("mdl-respuesta");
        }
      },
      error: function (dataError) {
        self.txtModalRespuesta(
          "Error as ncrono. Consulte con el administrador"
        );
        showModalRespuesta("mdl-respuesta");
      },
    });
  };

  self.consultarExamenesAgregadosCita = function () {
    let textoCargando = $("#texto-cargando-examenes-agregados");
    let textoError = $("#texto-error-consultar-examenes-agregados");
    textoCargando.show();
    textoError.hide();
    self.examenesAgregados([]);
    $.ajax({
      url: "/Cita/ConsultarExamenesAgregadosCita",
      method: "POST",
      data: {
        citaId: $("#CitaId").val(),
      },
      success: function (dataResult) {
        textoCargando.hide();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          $(data.Resultado).each(function (idx, examen) {
            examen.Item = itemExamenes;
            examen.PrecioValorCubiertoSeguro = ko.observable(
              examen.PrecioValorCubiertoSeguro
            );
            examen.PrecioValorCopago = ko.observable(examen.PrecioValorCopago);
            examen.PrecioValor = ko.observable(examen.PrecioValor);
            self.examenesAgregados.push(examen);
            itemExamen++;
          });
          self.actualizarTotales();
        } else {
          textoError.show();
        }
      },
      error: function (dataError) {
        textoCargando.hide();
        textoError.show();
      },
    });
  };

  self.servicioAgregarSeleccionado.subscribe(function (value) {
    if (!value || !value.ServicioId) {
      self.preciosServicioSeleccionado([]);
      return;
    }
    self.consultarPreciosServicio(value.ServicioId);
  });

  self.examenSeleccionado.subscribe(function (value) {
    if (!value || !value.ExamenId) {
      self.preciosExamen([]);
      return;
    }
    self.consultarPreciosExamen(value.ExamenId);
  });
  self.consultarPreciosExamen = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Venta/ConsultarPreciosExamen",
      data: {
        examenLabClinicoId: self.examenSeleccionado().ExamenId,
        fecha: $("#FechaHora").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.preciosExamen(data.Resultado);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };

  self.agregarServicio = function () {
    var servicioNombreSeleccionado =
      self.servicioAgregarSeleccionado().ServicioNombre ||
      self.servicioAgregarSeleccionado().ServicioNombreMostrar ||
      $("select[data-bind*='serviciosExistentes']")
        .first()
        .find("option:selected")
        .text();
    console.log("[COEX][AgregarServicio]", {
      servicioSeleccionado: self.servicioAgregarSeleccionado(),
      servicioNombreSeleccionado: servicioNombreSeleccionado,
    });
    self.serviciosAgregados.push({
      Item: itemServicio,
      ServicioId: self.servicioAgregarSeleccionado().ServicioId,
      ServicioNombre: servicioNombreSeleccionado,
      Cantidad: 1,
      ServicioDuracionHoras:
        self.servicioAgregarSeleccionado().ServicioDuracionHoras,
      ServicioDuracionMinutos:
        self.servicioAgregarSeleccionado().ServicioDuracionMinutos,
      ServicioDuracionText:
        self.servicioAgregarSeleccionado().ServicioDuracionText,
      PrecioId:
        self.precioServicioAgregarSeleccionado() == undefined
          ? null
          : self.precioServicioAgregarSeleccionado().PrecioId,
      PrecioValor: ko.observable(
        self.precioServicioAgregarSeleccionado() == undefined
          ? null
          : self.precioServicioAgregarSeleccionado().PrecioValor
      ),
      PrecioNombre:
        self.precioServicioAgregarSeleccionado() == undefined
          ? null
          : self.precioServicioAgregarSeleccionado().PrecioNombre,
      PrecioNombreValor:
        self.precioServicioAgregarSeleccionado() == undefined
          ? null
          : self.precioServicioAgregarSeleccionado().PrecioNombreValor,
      PrecioValorCubiertoSeguro: ko.observable(0),
      PrecioValorCopago: ko.observable(
        self.precioServicioAgregarSeleccionado() == undefined
          ? null
          : self.precioServicioAgregarSeleccionado().PrecioValor
      ),
      Nuevo: true,
    });
    itemServicio++;
    self.actualizarTotales();
    self.validarDisponibilidadEmpleado();
  };
  self.quitarServicio = function (value) {
    $(self.serviciosAgregados()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregados.splice(idx, 1);
        return false;
      }
    });
    self.actualizarTotales();
    self.validarDisponibilidadEmpleado();
  };
  self.agregarExamen = function () {
    var examenSeleccionado = self.examenSeleccionado();
    var examenId = getExamenIdCita(examenSeleccionado);
    if (!examenId) {
      examenId = parseInt(
        $("select[data-bind*='examenesExistentes']").first().val(),
        10
      );
    }
    if (!examenId) {
      alert("Seleccione un examen valido.");
      return;
    }
    var examenNombreSeleccionado =
      examenSeleccionado.ExamenNombre ||
      examenSeleccionado.ExamenNombreMostrar ||
      $("select[data-bind*='examenesExistentes']")
        .first()
        .find("option:selected")
        .text();
    self.examenesAgregados.push({
      Item: itemExamen,
      ExamenId: examenId,
      ExamenNombre: examenNombreSeleccionado,
      Cantidad: 1,
      PrecioId:
        self.precioSeleccionadoExamen() == undefined
          ? null
          : self.precioSeleccionadoExamen().PrecioId,
      PrecioValor: ko.observable(
        self.precioSeleccionadoExamen() == undefined
          ? null
          : self.precioSeleccionadoExamen().PrecioValor
      ),
      PrecioValorCubiertoSeguro: ko.observable(0),
      PrecioValorCopago: ko.observable(
        self.precioSeleccionadoExamen() == undefined
          ? null
          : self.precioSeleccionadoExamen().PrecioValor
      ),
      Nuevo: true,
    });
    itemExamen++;
    self.actualizarTotales();
  };
  self.quitarExamen = function (value) {
    $(self.examenesAgregados()).each(function (idx, examen) {
      if (value.Item == examen.Item) {
        self.examenesAgregados.splice(idx, 1);
        return false;
      }
    });
    self.actualizarTotales();
  };

  self.validateModel = function () {
    let empleadoId = $("#EmpleadoId").val();
    let pacienteNombre = self.pacienteSeleccionadoNombre();
    if (
      pacienteNombre == undefined ||
      pacienteNombre == null ||
      pacienteNombre.trim() == ""
    ) {
      mensajeEmergenteError("Debe asignar un paciente");
      return false;
    }
    if (
      empleadoId == undefined ||
      empleadoId == null ||
      empleadoId.trim() == ""
    ) {
      mensajeEmergenteError("Seleccione un empleado");
      return false;
    }
    return true;
  };

  self.getModel = function () {
    var habitacionId = $("#selectHabitacionEdicion").val();
    if (!habitacionId && $("#HabitacionId").length) {
      habitacionId = $("#HabitacionId").val();
    }

    var model = {
      CitaId: $("#CitaId").val(),
      PacienteNombre: $("#PacienteNombre").val(),
      PacienteId: $("#PacienteId").val(),
      EspecialidadId: $("#EspecialidadId").val(),
      EmpleadoId: $("#EmpleadoId").val(),
      SucursalId: $("#SucursalId").val(),
      Motivo: $("#Motivo").val(),
      NombreEncargado: $("#NombreEncargado").val(),
      DPIEncargado: $("#DPIEncargado").val(),
      FechaHora: $("#FechaHora").val(),
      EstadoCita: $("#EstadoCita").val(),
      CodigoCita: $("#CodigoCita").val(),
      CodigoAutorizacion: $("#CodigoAutorizacion").val(),
      Servicios: self.serviciosAgregados(),
      DuracionTotalHoras: self.totalDuracionHoras(),
      DuracionTotalMinutos: self.totalDuracionMinutos(),
      nombrePacienteSeleccionado: $("#nombrePacienteSeleccionado").val(),
      dpiPacienteSeleccionado: $("#dpiPacienteSeleccionado").val(),
      FechaNacimiento: $("#FechaNacimiento").val(),
      SexoId: $("#SexoId").val(),
      Telefono: $("#Telefono").val(),
      Email: $("#Email").val(),
      Direccion: $("#Direccion").val(),
      no_IGGS: $("#no_IGGS").val(),
      NumeroTurno: $("#NumeroTurno").val(),
      NivelPrioridadCita: $("#NivelPrioridadCita").val(),
      EtniaPaciente: $("#EtniaPaciente").val(),
      OrigenPaciente: $("#OrigenPaciente").val(),
      DepartamentoId: $("#DepartamentoId").val() || null,
      MunicipioId: $("#MunicipioId").val() || null,
      Dia: $("#Dia").val(),
      Hora: $("#Hora").val(),
      Examenes: self.examenesAgregados(),
      CitaTipoAtencion: $("#CitaTipoAtencion").val(),
      AnestesistaId: self.anestesistaId(),
      PrimerAyudanteId: self.primerAyudanteId(),
      SegundoAyudanteId: self.segundoAyudanteId(),
      InstrumentistaId: self.instrumentistaId(),
      CirculanteId: self.circulanteId(),
      Procedimiento: $("#Procedimiento").val(),
      HabitacionId: habitacionId ? parseInt(habitacionId) : null,
    };
    return model;
  };

  self.agendarCita = function () {
    function generarDpiAleatorio() {
      let dpi = "";
      for (let i = 0; i < 40; i++) {
        dpi += Math.floor(Math.random() * 10);
      }
      return dpi;
    }
    let newDpi = $("#dpiPacienteSeleccionado").val().trim();
    if (!newDpi) {
      newDpi = generarDpiAleatorio();
      $("#dpiPacienteSeleccionado").val(newDpi);
    }
    let currentDpi = self.pacienteSeleccionado()
      ? self.pacienteSeleccionado().Dpi
      : null;
    if (newDpi !== currentDpi) {
      $("#PacienteId").val(null);
    }
    let edadTexto = $("#PacienteEdad").val();
    let edadAños = parseInt(edadTexto.split(" ")[0], 10);
    let encargado = $("#NombreEncargado").val().trim();
    if (edadAños < 13 && encargado === "") {
      alert("El encargado es obligatorio para pacientes menores de 13 años.");
      $("#NombreEncargado").focus();
      return;
    }
    if (self.validateModel()) {
      var newPacienteId = $("#PacienteId").val();
      var continuarConfirmacion = function () {
        self.validarDisponibilidadEmpleado(function (ok) {
          if (!ok) return;
          self.confirmarAgendarCita();
        });
      };
      if (!newPacienteId) {
        var datoDPI = { Dpi: newDpi };
        $.ajax({
          method: "POST",
          data: datoDPI,
          url: "/Pacientes/GetPacienteByDPI",
          traditional: true,
          success: function (data) {
            if (data.existe) {
              alert("El DPI ya está en uso.");
            } else {
              continuarConfirmacion();
            }
          },
          error: function (data) {
            hideLoading();
            console.log(data.error);
            alert("ERROR DE SERVIDOR:" + data.error);
          },
        });
      } else {
        continuarConfirmacion();
      }
    }
  };

  self.confirmarAgendarCita = function () {
    self.txtModalConfirmacion("¿Desea agendar esta cita?");
    $("#mdl-confirmacion").dialog({
      modal: true,
      buttons: [
        {
          text: "Si",
          class: "btn btn-success",
          click: function () {
            showLoading();
            var model = self.getModel();
            $.ajax({
              url: "/Cita/AgendarCita",
              method: "POST",
              data: model,
              success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                  window.location.href = data.RedirectUrl;

                  // window.location.href = "/Cita/CalendarioLineal";
                } else {
                  hideLoading();
                  alert(data.Mensaje);
                }
              },
              error: function (dataError) {
                hideLoading();
                console.log(dataError);
                alert(dataError);
              },
            });
            $(this).dialog("close");
          },
        },
        {
          text: "No",
          class: "btn btn-danger",
          click: function () {
            $(this).dialog("close");
          },
        },
      ],
    });
  };

  self.enviarDatosMiddleware = function () {
    function generarFolioUnico() {
      var fecha = new Date();
      var año = fecha.getFullYear();
      var mes = (fecha.getMonth() + 1).toString().padStart(2, "0");
      var dia = fecha.getDate().toString().padStart(2, "0");
      var fechaFormato = `${año}${mes}${dia}`;
      var numerosAleatorios = Math.floor(1000 + Math.random() * 9000);
      return `PACS-${fechaFormato}-${numerosAleatorios}`;
    }
    var pacienteNombre = $("#nombrePacienteSeleccionado").val().split(" ");
    var fechaNacimiento = $("#FechaNacimiento").val();
    var sexo =
      $("#SexoId").val() === "1" ? "M" : $("#SexoId").val() === "2" ? "F" : "O";
    var telefono = $("#Telefono").val();
    var email = $("#Email").val();
    var dpi = $("#dpiPacienteSeleccionado").val();
    var motivo = $("#Motivo").val();
    const especialidadesMap = {
      Alergiologia: "001",
      Cardiología: "002",
      Cirugía: "003",
      "Cirugia Plastica": "004",
      "Ginecología y Obstetricia": "005",
      "Laboratorio Clínico": "006",
      "Medicina estética": "007",
      "Medicina general": "008",
      "Medicina interna": "009",
      "Nueva especialidad": "010",
      Nutrición: "011",
      Pediatría: "012",
      Radioterapia: "013",
      Traumatología: "014",
    };
    var datosMiddleware = {
      folio: generarFolioUnico(),
      facility_identifier: "FACILITY_01",
      description: motivo || "Desconocido",
      study_code:
        especialidadesMap[$("#EspecialidadId option:selected").text()],
      study_name: $("#EspecialidadId option:selected").text(),
      modality: "CR",
      room_identifier: "sala-01-radiografia-computarizada-cr-matriz",
      patient_identifier: dpi,
      patient_name: pacienteNombre[0],
      patient_first_surname: pacienteNombre[2],
      patient_last_surname: pacienteNombre[3],
      patient_email: "null@email.com",
      patient_gender: sexo,
      patient_birth_date: fechaNacimiento,
      patient_phone_code: "502",
      patient_phone_number: telefono,
      physician_identifier: "00002221",
      physician_name: $("#EmpleadoId option:selected").text(),
      physician_first_surname: "",
      physician_last_surname: "",
      physician_email: "null@email.com",
      physician_gender: "M",
      physician_phone_code: "502",
      physician_phone_number: "null",
      nursing_unit_identifier: "null",
      nursing_unit_name: "null",
      nursing_unit_description: "null",
    };
    $.ajax({
      url: "/api/pacs/orders",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(datosMiddleware),
      success: function (response) {
        console.log(response);
        window.location.href = "/Cita/CalendarioLineal";
      },
      error: function (error) {
        console.error("Error al enviar datos al middleware", error);
      },
    });
  };

  self.generarTurno = function () {
    var datoEspecialidadId = { id: $("#EspecialidadId").val() };
    if (datoEspecialidadId.id == null || datoEspecialidadId.id == undefined) {
      mensajeEmergenteError("Seleccione una especialidad");
    } else {
      $.ajax({
        method: "POST",
        data: datoEspecialidadId,
        url: "/Cita/GetTurnoEspecialidad",
        traditional: true,
        success: function (data, textStatus) {
          if (data.existe) {
            $("#NumeroTurno").val(
              data.codEspecialidad + "00" + data.turnoactual
            );
          } else {
            mensajeEmergenteError("Fallo el turno, no hay datos");
          }
        },
        error: function (data) {
          hideLoading();
          console.log(data.error);
          alert("ERROR DE SERVIDOR:" + data.error);
        },
      });
    }
  };

  self.editarCita = function () {
    if (self.validateModel()) {
      self.txtModalConfirmacion("&iquest;Desea modificar esta cita?");
      $("#mdl-confirmacion").dialog({
        modal: true,
        buttons: [
          {
            text: "Si",
            class: "btn btn-success",
            click: function () {
              showLoading();
              var model = self.getModel();
              $.ajax({
                url: "/Cita/EditarCita",
                method: "POST",
                data: model,
                success: function (dataResult) {
                  var data = JSON.parse(dataResult);
                  if (data.Exitoso) {
                    window.location.href =
                      "/Cita/CalendarioLineal?fecha=" +
                      data.Fecha +
                      "&sucursalId=" +
                      $("#SucursalId").val();
                  } else {
                    hideLoading();
                    self.txtModalRespuesta(data.Mensaje);
                    showModalRespuesta("mdl-respuesta");
                  }
                },
                error: function (dataError) {
                  hideLoading();
                  console.log(dataError);
                  alert(dataError);
                },
              });
            },
          },
          {
            text: "No",
            class: "btn btn-danger",
            click: function () {
              $(this).dialog("close");
            },
          },
        ],
      });
    }
  };

  self.editarCitaCP = function () {
    showLoading();
    if (self.validateModel()) {
      var Fecha = {
        CitaId: $("#CitaId").val(),
        Dia: $("#Dia").val(),
        Hora: $("#Hora").val(),
      };
      $.ajax({
        url: "/Cita/BuscarAgenda",
        method: "POST",
        data: Fecha,
        success: function (data) {
          hideLoading();
          if (data.exitoso) {
            self.txtModalConfirmacion("&iquest;Desea modificar esta cita?");
            $("#mdl-confirmacion").dialog({
              modal: true,
              buttons: [
                {
                  text: "Si",
                  class: "btn btn-success",
                  click: function () {
                    showLoading();
                    self.getModel();
                    $.ajax({
                      url: "/Cita/EditarCita",
                      method: "POST",
                      data: model,
                      success: function (dataResult) {
                        var data = JSON.parse(dataResult);
                        if (data.Exitoso) {
                          window.location.href =
                            "/Cita/CalendarioLineal?fecha=" +
                            data.Fecha +
                            "&sucursalId=" +
                            $("#SucursalId").val();
                        } else {
                          hideLoading();
                          self.txtModalRespuesta(data.Mensaje);
                          showModalRespuesta("mdl-respuesta");
                        }
                      },
                      error: function (dataError) {
                        hideLoading();
                        console.log(dataError);
                        alert(dataError);
                      },
                    });
                  },
                },
                {
                  text: "No",
                  class: "btn btn-danger",
                  click: function () {
                    $(this).dialog("close");
                  },
                },
              ],
            });
          } else {
            hideLoading();
            alert("Ya se encuentra una cita agendada a esta fecha");
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          alert(dataError);
        },
      });
    }
  };
  self.cancelar = function () {
    self.txtModalConfirmacion("&iquest;Desea cancelar el registro?");
    $("#mdl-confirmacion").dialog({
      modal: true,
      buttons: [
        {
          text: "Si",
          class: "btn btn-success",
          click: function () {
            window.location.href = "/Cita/CalendarioLineal";
          },
        },
        {
          text: "No",
          class: "btn btn-danger",
          click: function () {
            $(this).dialog("close");
          },
        },
      ],
    });
  };

  self.cargarHabitacionesEdicion = function (citaId) {
    $.ajax({
      url: '/Cita/GetHabitacionesParaEdicion',
      method: 'GET',
      data: { citaId: citaId },
      success: function (data) {
        if (data.exitoso) {
          self.habitacionesOptions(data.habitaciones);

          var $select = $('#selectHabitacionEdicion');

          if ($select.data('select2')) {
            $select.select2('destroy');
          }

          if (data.habitacionActualId) {
            self.habitacionSeleccionadaId(data.habitacionActualId);
          } else {
            self.habitacionSeleccionadaId(null);
          }

          $select.select2({
            placeholder: 'Seleccione...',
            allowClear: true
          });

          $select.val(self.habitacionSeleccionadaId()).trigger('change');

          $select.off('change').on('change', function () {
            var val = $(this).val();
            self.habitacionSeleccionadaId(val ? parseInt(val) : null);
          });
        } else {
          console.error('Error cargando habitaciones:', data.mensaje);
        }
      },
      error: function (err) {
        console.error('Error AJAX habitaciones:', err);
      }
    });
  };

  self.habitacionSeleccionadaId.subscribe(function (newValue) {
    var $select = $('#selectHabitacionEdicion');
    if ($select.length && $select.data('select2')) {
      $select.val(newValue).trigger('change');
    }
  });

  self.actualizarTotales = function () {
    let totalDuracionHoras = 0;
    let totalDuracionMinutos = 0;
    $(self.serviciosAgregados()).each(function (idx, servicio) {
      let cantidad = isNaN(servicio.Cantidad) ? 1 : servicio.Cantidad;
      if (
        servicio.ServicioDuracionHoras != undefined &&
        servicio.ServicioDuracionHoras != null
      ) {
        totalDuracionHoras +=
          parseInt(servicio.ServicioDuracionHoras) * cantidad;
      }
      if (
        servicio.ServicioDuracionMinutos != undefined &&
        servicio.ServicioDuracionMinutos != null
      ) {
        totalDuracionMinutos +=
          parseInt(servicio.ServicioDuracionMinutos) * cantidad;
      }
      let valorTotal = servicio.PrecioValor();
      let valorCubiertoSeguro = servicio.PrecioValorCubiertoSeguro();
      let valorCopago = 0;
      if (valorTotal == undefined || valorTotal == null || isNaN(valorTotal)) {
        valorTotal = 0;
      }
      if (
        valorCubiertoSeguro == undefined ||
        valorCubiertoSeguro == null ||
        isNaN(valorCubiertoSeguro)
      ) {
        valorCubiertoSeguro = 0;
      }
      valorCopago = valorTotal - valorCubiertoSeguro;
      servicio.PrecioValorCopago(valorCopago);
    });
    $(self.examenesAgregados()).each(function (idx, examen) {
      let valorTotal = examen.PrecioValor();
      let valorCubiertoSeguro = examen.PrecioValorCubiertoSeguro();
      let valorCopago = 0;
      if (valorTotal == undefined || valorTotal == null || isNaN(valorTotal)) {
        valorTotal = 0;
      }
      if (
        valorCubiertoSeguro == undefined ||
        valorCubiertoSeguro == null ||
        isNaN(valorCubiertoSeguro)
      ) {
        valorCubiertoSeguro = 0;
      }
      valorCopago = valorTotal - valorCubiertoSeguro;
      examen.PrecioValorCopago(valorCopago);
    });
    if (totalDuracionMinutos >= 60) {
      totalDuracionHoras += Math.floor(totalDuracionMinutos / 60);
      totalDuracionMinutos = totalDuracionMinutos % 60;
    }
    self.totalDuracionHoras(totalDuracionHoras);
    self.totalDuracionMinutos(totalDuracionMinutos);
    self.totalDuracion((totalDuracionHoras * 60) + totalDuracionMinutos);
    let totalValor = 0;
    $(self.serviciosAgregados()).each(function (idx, servicio) {
      let cantidad = isNaN(servicio.Cantidad) ? 1 : servicio.Cantidad;
      let precio = 0;
      if (typeof servicio.PrecioValor === "function") {
        precio = servicio.PrecioValor();
      } else if (servicio.PrecioValor != undefined && servicio.PrecioValor != null) {
        precio = servicio.PrecioValor;
      }
      if (precio == undefined || precio == null || isNaN(precio)) {
        precio = 0;
      }
      totalValor += parseFloat(precio) * cantidad;
    });
    self.totalValor("GTQ " + totalValor);
  };

  self.consultarExamenesExistentes = function () {
    console.log("[COEX][Examenes] Consultando examenes existentes...");
    $.ajax({
      method: "POST",
      url: "/Venta/ConsultarExamenesExistentes",
      data: model,
      success: function (dataResult) {
        hideLoading();
        console.log("[COEX][Examenes] Respuesta cruda:", dataResult);
        let data = JSON.parse(dataResult);
        console.log("[COEX][Examenes] Respuesta parseada:", {
          exitoso: data.Exitoso,
          cantidad: data.Resultado ? data.Resultado.length : 0,
          primerItem: data.Resultado && data.Resultado.length ? data.Resultado[0] : null
        });
        if (data.Exitoso) {
          self.examenesExistentes(data.Resultado);
          console.log("[COEX][Examenes] Observable actualizado:", self.examenesExistentes().length);
          setTimeout(function () {
            syncSelectExamenCita();
            console.log("[COEX][Examenes] Opciones en select examen:", $("select[data-bind*='examenesExistentes'] option").length);
          }, 0);
        } else {
          hideLoading();
          console.warn("[COEX][Examenes] Backend respondió error:", data.Mensaje);
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        console.error("[COEX][Examenes] Error AJAX:", data);
        alert(data.error);
      },
    });
  };

  self._setMensajeConflictoEmpleado = function (mensaje) {
    if (mensaje && mensaje.trim() !== "") {
      $("#msgConflictoEmpleado").text(mensaje).show();
    } else {
      $("#msgConflictoEmpleado").text("").hide();
    }
  };

  self.validarDisponibilidadEmpleado = function (onOk) {
    var empleadoId = $("#EmpleadoId").val();
    if (!empleadoId) {
      self._setMensajeConflictoEmpleado("");
      if (onOk) onOk(true);
      return;
    }
    var fechaHora = $("#FechaHora").val();
    if (!fechaHora) {
      self._setMensajeConflictoEmpleado("");
      if (onOk) onOk(true);
      return;
    }
    self.actualizarTotales();
    $.ajax({
      url: "/Cita/ValidarDisponibilidadEmpleado",
      method: "POST",
      data: {
        empleadoId: parseInt(empleadoId),
        fechaHora: fechaHora,
        duracionHoras: self.totalDuracionHoras() || 0,
        duracionMinutos: self.totalDuracionMinutos() || 0,
        citaId: $("#CitaId").val() || null,
      },
      success: function (r) {
        if (r && r.exitoso) {
          self._setMensajeConflictoEmpleado("");
          if (onOk) onOk(true);
        } else {
          self._setMensajeConflictoEmpleado(
            r && r.mensaje ? r.mensaje : "Conflicto de agenda."
          );
          if (onOk) onOk(false);
        }
      },
      error: function () {
        self._setMensajeConflictoEmpleado(
          "No se pudo validar la disponibilidad del profesional. Intente nuevamente."
        );
        if (onOk) onOk(false);
      },
    });
  };



  self.cargarEmpleados = function (filtroEspecialidad, filtroUnidad, targetArray, observableDestino, initialValue, selectId) {
    $.ajax({
      url: '/Cita/GetEmpleadosPorTipo',
      method: 'GET',
      data: {
        especialidadNombre: filtroEspecialidad || '',
        unidadNombre: filtroUnidad || ''
      },
      success: function (data) {
        var mapped = data.map(function (item) {
          return { id: item.id, nombre: item.nombre };
        });
        targetArray(mapped);
        if (observableDestino && initialValue !== undefined && initialValue !== null && initialValue !== 0) {
          observableDestino(initialValue);
        }
        if (selectId) {
          setTimeout(function () {
            refreshKoSelect2ById(selectId, "Seleccione...");
            if (observableDestino && initialValue !== undefined && initialValue !== null && initialValue !== 0) {
              $("#" + selectId).val(String(initialValue)).trigger("change.select2");
            }
          }, 0);
        }
      },
      error: function (err) {
        console.error("Error cargando empleados:", err);
      }
    });
  };

  // ========== FUNCIÓN PÚBLICA PARA CARGAR EQUIPO QUIRÚRGICO ==========
  self.cargarEquipoQuirurgicoDesdeModel = function (modelo) {
    if (!modelo) {
      console.error("modelo es nulo");
      return;
    }

    if (modelo.EmpleadoId && (modelo.EmpleadoNombre || modelo.EmpleadoText)) {
      self.cirujanoNombre(modelo.EmpleadoNombre || modelo.EmpleadoText);
    } else {
      var selectedOption = $("#EmpleadoId option:selected");
      if (selectedOption.length) {
        self.cirujanoNombre(selectedOption.text());
      }
    }
    console.log("🩺 Cargando equipo quirúrgico desde modelo:", modelo);
    self.cargarEmpleados('Anestesista', null, self.anestesistaOptions, self.anestesistaId, modelo.AnestesistaId, 'selectAnestesista');
    self.cargarEmpleados(null, null, self.primerAyudanteOptions, self.primerAyudanteId, modelo.PrimerAyudanteId, 'selectPrimerAyudante');
    self.cargarEmpleados(null, null, self.segundoAyudanteOptions, self.segundoAyudanteId, modelo.SegundoAyudanteId, 'selectSegundoAyudante');
    self.cargarEmpleados(null, 'Instrumentista', self.instrumentistaOptions, self.instrumentistaId, modelo.InstrumentistaId, 'selectInstrumentista');
    self.cargarEmpleados(null, 'Circulante', self.circulanteOptions, self.circulanteId, modelo.CirculanteId, 'selectCirculante');
  };

  self.cargarEquipoQuirurgico = self.cargarEquipoQuirurgicoDesdeModel;
};

function citaSelect2Options(placeholder, extraOptions) {
  return $.extend({
    theme: "bootstrap4",
    width: "100%",
    minimumResultsForSearch: 0,
    allowClear: true,
    placeholder: placeholder || "Buscar...",
    language: {
      noResults: function () { return "No se encontraron resultados"; },
      searching: function () { return "Buscando..."; }
    }
  }, extraOptions || {});
}

function refreshKoSelect2($el, placeholder) {
  if (!$el || !$el.length || !$.fn.select2) return;
  if (typeof ko !== "undefined" && ko.tasks && ko.tasks.runEarly) {
    ko.tasks.runEarly();
  }
  if ($el.find("option").length <= 1) {
    if ($el.hasClass("select2-hidden-accessible")) {
      $el.select2("destroy");
    }
    return;
  }
  var currentVal = $el.val();
  if ($el.hasClass("select2-hidden-accessible")) {
    $el.select2("destroy");
  }
  $el.select2(citaSelect2Options(placeholder));
  if (currentVal) {
    $el.val(currentVal).trigger("change.select2");
  }
}

function refreshKoSelect2ById(id, placeholder) {
  refreshKoSelect2($("#" + id), placeholder);
}

function refreshAllKoSelect2OnPage() {
  if (!window.jQuery || !$.fn.select2) return;
  $("select.select2bs4").each(function () {
    var $el = $(this);
    if ($el.attr("id") === "selectPaciente" || $el.attr("id") === "municipioSeleccionado") return;
    var bindAttr = $el.attr("data-bind") || "";
    if (bindAttr.indexOf("options:") < 0) return;
    if (bindAttr.indexOf("serviciosExistentes") >= 0 || bindAttr.indexOf("examenesExistentes") >= 0) return;
    var placeholder = $el.find('option[value=""]').first().text() || "Buscar...";
    refreshKoSelect2($el, placeholder);
  });
}

function syncSelectServicioCita() {
  if (typeof window.citaVm === "undefined" || !window.citaVm) return;
  var $sel = $("select[data-bind*='serviciosExistentes']").first();
  if (!$sel.length || !$.fn.select2) return;

  var servicios = citaVm.serviciosExistentes() || [];
  var selected = citaVm.servicioAgregarSeleccionado();
  var selectedId = selected && selected.ServicioId ? String(selected.ServicioId) : "";

  if ($sel.hasClass("select2-hidden-accessible")) {
    $sel.select2("destroy");
  }

  $sel.empty();
  $sel.append($("<option>").val("").text("Seleccionar servicio"));
  servicios.forEach(function (s) {
    if (!s || s.ServicioId == null) return;
    var label = s.ServicioNombreMostrar || s.ServicioNombre || String(s.ServicioId);
    $sel.append($("<option>").val(String(s.ServicioId)).text(label));
  });

  if (selectedId) {
    $sel.val(selectedId);
  }

  $sel.select2(citaSelect2Options("Seleccionar servicio"));

  $sel.off("change.citaServicio").on("change.citaServicio", function () {
    var raw = $(this).val();
    if (!raw) {
      citaVm.servicioAgregarSeleccionado(null);
      return;
    }
    var id = parseInt(raw, 10);
    var servicio = citaVm.serviciosExistentes().find(function (s) { return s.ServicioId === id; });
    citaVm.servicioAgregarSeleccionado(servicio || null);
  });
}

function syncSelectExamenCita() {
  if (typeof window.citaVm === "undefined" || !window.citaVm) return;
  var $sel = $("select[data-bind*='examenesExistentes']").first();
  if (!$sel.length || !$.fn.select2) return;

  var examenes = citaVm.examenesExistentes() || [];
  var selected = citaVm.examenSeleccionado();
  var selectedId = selected ? String(getExamenIdCita(selected) || "") : "";

  if ($sel.hasClass("select2-hidden-accessible")) {
    $sel.select2("destroy");
  }

  $sel.empty();
  $sel.append($("<option>").val("").text("Seleccionar examen"));
  examenes.forEach(function (e) {
    var examenId = getExamenIdCita(e);
    if (!examenId) return;
    var label = e.ExamenNombreMostrar || e.ExamenNombre || String(examenId);
    $sel.append($("<option>").val(String(examenId)).text(label));
  });

  if (selectedId) {
    $sel.val(selectedId);
  }

  $sel.select2(citaSelect2Options("Seleccionar examen"));

  $sel.off("change.citaExamen").on("change.citaExamen", function () {
    var raw = $(this).val();
    if (!raw) {
      citaVm.examenSeleccionado(null);
      return;
    }
    var id = parseInt(raw, 10);
    var examen = citaVm.examenesExistentes().find(function (e) { return getExamenIdCita(e) === id; });
    citaVm.examenSeleccionado(examen || null);
  });
}

function getExamenIdCita(examen) {
  if (!examen) return null;
  var id = examen.ExamenId || examen.examenId;
  id = parseInt(id, 10);
  return isNaN(id) || id <= 0 ? null : id;
}

function syncSelectPacienteCita() {
  if (typeof window.citaVm === "undefined" || !window.citaVm) return;
  var $sel = $("#selectPaciente");
  if (!$sel.length || !$.fn.select2) return;

  var pacientes = citaVm.pacientes() || [];
  var selected = citaVm.pacienteSeleccionado();
  var selectedId = selected && selected.Id ? String(selected.Id) : "";
  var selectedText = (typeof selected === "string") ? selected.trim() : "";

  if ($sel.hasClass("select2-hidden-accessible")) {
    $sel.select2("destroy");
  }

  $sel.empty();
  $sel.append($("<option>").val("").text("Seleccionar paciente"));
  pacientes.forEach(function (p) {
    if (!p || p.Id == null) return;
    var label = p.NombreConDpi || p.Nombre || String(p.Id);
    $sel.append($("<option>").val(String(p.Id)).text(label));
  });

  if (selectedId) {
    $sel.val(selectedId);
  } else if (selectedText) {
    $sel.append($("<option>").val(selectedText).text(selectedText).attr("data-select2-tag", "true"));
    $sel.val(selectedText);
  }

  $sel.select2(citaSelect2Options("Seleccionar paciente", {
    tags: true,
    createTag: function (params) {
      var term = $.trim(params.term || "");
      if (term === "") return null;
      return {
        id: term,
        text: term,
        newPaciente: true
      };
    }
  }));

  $sel.off("change.citaPaciente").on("change.citaPaciente", function () {
    var raw = $(this).val();
    if (!raw) {
      citaVm.pacienteSeleccionado(null);
      return;
    }
    var paciente = null;
    if (/^\d+$/.test(raw)) {
      var id = parseInt(raw, 10);
      paciente = citaVm.pacientes().find(function (p) { return p.Id === id; });
    }
    if (paciente) {
      citaVm.pacienteSeleccionado(paciente);
    } else {
      citaVm.pacienteSeleccionado(raw);
    }
  });
}

function syncSelectMunicipioCita() {
  if (typeof window.citaVm === "undefined" || !window.citaVm) return;
  var $sel = $("#municipioSeleccionado");
  if (!$sel.length || !$.fn.select2) return;

  var municipios = citaVm.municipiosExistentes() || [];
  var selected = citaVm.municipioSeleccionado();
  var selectedId = selected != null && selected !== "" ? String(selected) : "";

  if ($sel.hasClass("select2-hidden-accessible")) {
    $sel.select2("destroy");
  }

  $sel.empty();
  $sel.append($("<option>").val("").text("Seleccione..."));
  municipios.forEach(function (m) {
    if (!m || m.Id == null) return;
    var label = m.MunicipioNombreMostrar
      || ((m.Id || "") + " - " + (m.NombreMunicipio || ""));
    $sel.append($("<option>").val(String(m.Id)).text(label));
  });

  if (selectedId) {
    $sel.val(selectedId);
  }

  $sel.select2(citaSelect2Options("Seleccione..."));

  $sel.off("change.citaMunicipio").on("change.citaMunicipio", function () {
    var raw = $(this).val();
    if (!raw) {
      citaVm.municipioSeleccionado(null);
      return;
    }
    citaVm.municipioSeleccionado(parseInt(raw, 10));
  });
}

function initCitaFormSelect2Static() {
  if (!window.jQuery || !$.fn.select2) return;
  $("select.select2bs4").each(function () {
    var $el = $(this);
    var bindAttr = $el.attr("data-bind") || "";
    if (bindAttr.indexOf("options:") >= 0) return;
    if ($el.attr("id") === "selectPaciente" || $el.attr("id") === "municipioSeleccionado") return;
    var placeholder = $el.find('option[value=""]').first().text() || "Buscar...";
    refreshKoSelect2($el, placeholder);
  });
}

if (typeof window.citaVm === "undefined") {
var citaVm = new CitaVM(typeof serverModel !== "undefined" ? serverModel : null);
window.citaVm = citaVm;
ko.applyBindings(citaVm);

$(function () {

  if (typeof serverModel !== 'undefined' && serverModel) {
    console.log("Llamando a cargarEquipoQuirurgicoDesdeModel con serverModel");
    citaVm.cargarEquipoQuirurgicoDesdeModel(serverModel);
  } else if (typeof window.serverModel !== 'undefined' && window.serverModel) {
    console.log("Llamando a cargarEquipoQuirurgicoDesdeModel con window.serverModel");
    citaVm.cargarEquipoQuirurgicoDesdeModel(window.serverModel);
  } else {
    console.error("No se encontró serverModel en ningún lado");
  }

  var citaId2 = $("#CitaId").val();
  if (citaId2 && citaId2.trim() !== "" && $("#selectHabitacionEdicion").length) {
    citaVm.cargarHabitacionesEdicion(parseInt(citaId2));
  }

  syncSelectMunicipioCita();

  citaVm.consultarPacientes();
  citaVm.consultarServiciosExistentes();
  citaVm.consultarExamenesExistentes();
  citaVm.consultarDepartamentosExistentes();

  let citaId = $("#CitaId").val();
  if (citaId && citaId.trim() !== "") {
    citaVm.consultarServiciosAgregadosCita();
    citaVm.consultarExamenesAgregadosCita();
  }

  $("#EmpleadoId").on("change", function () {
    var selectedOption = $(this).find('option:selected');
    var nombreMedico = selectedOption.text();
    citaVm.cirujanoNombre(nombreMedico);
    citaVm.validarDisponibilidadEmpleado();
  });

  $("#fechacita").daterangepicker({
    timePicker: true,
    timePickerIncrement: 30,
    singleDatePicker: true,
    locale: {
      format: "MM/DD/YYYY hh:mm A",
    },
  });

  citaVm.citaTipoAtencion($("#CitaTipoAtencion").val());
  $("#CitaTipoAtencion").on("change", function () {
    citaVm.citaTipoAtencion($(this).val());
  });

  setTimeout(initCitaFormSelect2Static, 300);
});
}

function getEdad() {
  let hoy = new Date();
  let fechaNacimiento = new Date($("#FechaNacimiento").val());
  if (!isNaN(fechaNacimiento)) {
    let edadAnios = hoy.getFullYear() - fechaNacimiento.getFullYear();
    let diferenciaMeses = hoy.getMonth() - fechaNacimiento.getMonth();
    let diferenciaDias = hoy.getDate() - fechaNacimiento.getDate();
    if (diferenciaMeses < 0 || (diferenciaMeses === 0 && diferenciaDias < 0)) {
      edadAnios--;
      diferenciaMeses += diferenciaMeses < 0 ? 12 : 0;
    }
    if (diferenciaDias < 0) {
      diferenciaMeses--;
      let mesAnterior = new Date(hoy.getFullYear(), hoy.getMonth(), 0);
      diferenciaDias += mesAnterior.getDate();
    }
    let textoAnios = edadAnios === 1 ? "Año" : "Años";
    let textoMeses = diferenciaMeses === 1 ? "Mes" : "Meses";
    let textoDias = diferenciaDias === 1 ? "Día" : "Días";
    let edadCompleta = `${edadAnios} ${textoAnios} ${diferenciaMeses} ${textoMeses} ${diferenciaDias} ${textoDias}`;
    $("#PacienteEdad").val(edadCompleta);
  } else {
    $("#PacienteEdad").val("");
  }
}

var windowObjectReference;
var windowFeatures = "menubar=yes,location=yes,resizable=yes,scrollbars=yes,status=yes";
