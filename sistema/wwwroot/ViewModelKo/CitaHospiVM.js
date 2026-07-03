var CitaHospiVM = function () {
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
  self.edadAniosPaciente = ko.observable();
  // Seguros
  self.seguros = ko.observableArray();
  self.seguroSeleccionado = ko.observable();
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
  //self.nombreServicioBuscar = ko.observable();
  self.nombreCodigoExamenBuscar = ko.observable();



  self.departamentoSeleccionado.subscribe(function (newValue) {
    $("#DepartamentoId").val(newValue);
  });
  self.municipioSeleccionado.subscribe(function (newValue) {
    $("#MunicipioId").val(newValue);
  });

  // Servicios
  // self.serviciosExistentes = ko.observableArray();
  // self.servicioAgregarSeleccionado = ko.observable();
  // self.preciosServicioSeleccionado = ko.observableArray();
  // self.precioServicioAgregarSeleccionado = ko.observable();
  // self.serviciosAgregados = ko.observableArray();
  // self.totalValor = ko.observable();

  // ==============================
  //  VALIDACION DISPONIBILIDAD (DOCTOR OCUPADO)
  // ==============================
  self._setMensajeConflictoEmpleado = function (mensaje) {
    if (mensaje && mensaje.trim() !== "") {
      $("#msgConflictoEmpleado").text(mensaje).show();
    } else {
      $("#msgConflictoEmpleado").text("").hide();
    }
  };

  self._getFechaHoraHospi = function () {
    // En Hospi, el usuario edita #fecha (yyyy-MM-dd) y #hora (HH:mm)
    // Si existen, los usamos y sincronizamos el hidden #FechaHora.
    var f = $("#fecha").length ? $("#fecha").val() : null;
    var h = $("#hora").length ? $("#hora").val() : null;

    if (f && h) {
      var fechaHora = f + " " + h;
      if ($("#FechaHora").length) {
        $("#FechaHora").val(fechaHora);
      }
      return fechaHora;
    }

    // fallback: si no hay inputs separados, usamos el hidden
    return $("#FechaHora").val();
  };

  self.validarDisponibilidadEmpleado = function (onOk) {
    var empleadoId = $("#EmpleadoId").val();
    if (!empleadoId) {
      self._setMensajeConflictoEmpleado("");
      if (onOk) onOk(true);
      return;
    }

    var fechaHora = self._getFechaHoraHospi();
    if (!fechaHora) {
      self._setMensajeConflictoEmpleado("");
      if (onOk) onOk(true);
      return;
    }

    // Asegurar que duración esté calculada
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

  //FUNCIONES VIEWMODEL
  self.pacienteSeleccionado.subscribe(function (paciente) {
    if (paciente.Id == null || paciente.Id == undefined) {
      self.pacienteSeleccionadoNombre(paciente);
      $("#PacienteNombre").val(paciente);
      $("#PacienteId").val(null);
      $("#dpiPacienteSeleccionado").val(null);
      $("#SexoId").val(null);
      $("#Telefono").val(null);
      $("#Email").val(null);
      $("#Direccion").val(null);
      $("#no_IGGS").val(null);
      $("#FechaNacimiento").val(null);
      $("#PacienteEdad").val(null);
      $("#EtniaPaciente").val(null);
      $("#OrigenPaciente").val(null);
      $("#ReligionPaciente").val(null);

      self.departamentoSeleccionado(null);
      self.municipioSeleccionado(null);
    } else {
      $("#PacienteNombre").val(paciente.Nombre);
      $("#PacienteId").val(paciente.Id);
      self.pacienteSeleccionadoNombre(paciente.Nombre);
      $("#dpiPacienteSeleccionado").val(paciente.Dpi);
      $("#SexoId").val(paciente.SexoId);
      $("#Telefono").val(paciente.Telefono);
      $("#Email").val(paciente.Email);
      $("#Direccion").val(paciente.Direccion);
      $("#EtniaPaciente").val(paciente.EtniaPaciente);
      $("#OrigenPaciente").val(paciente.OrigenPaciente);
      $("#ReligionPaciente").val(paciente.ReligionPaciente);
      $("#no_IGGS").val(paciente.IgssNumeroAfiliacion);
      $("#FechaNacimiento").val(paciente.FechaNacimiento);
      getEdad();

      // Guardamos el municipioId para usarlo después
      var municipioIdToSet = paciente.MunicipioId;

      // Primero establecemos el departamento sin lanzar el evento change
      self.departamentoSeleccionado(paciente.DepartamentoId);

      // Cargamos los municipios manualmente y después establecemos el municipio
      self.consultarMunicipiosExistentes(paciente.DepartamentoId, function () {
        // Una vez cargados los municipios, establecemos el valor del municipio
        self.municipioSeleccionado(municipioIdToSet);
        $("#municipioSeleccionado").val(municipioIdToSet).trigger("change");
      });

      // Ahora lanzamos el evento change del departamento
      $("#departamentoSeleccionado")
        .val(paciente.DepartamentoId)
        .trigger("change");

      // Responsable
      $("#ResponsableNit").val(paciente.ResponsableNit);
      $("#ResponsableNombre").val(paciente.ResponsableNombre);
      $("#ResponsableDireccion").val(paciente.ResponsableDireccion);
      $("#ResponsableCorreo").val(paciente.ResponsableCorreo);
      $("#ResponsableTelefono").val(paciente.ResponsableTelefono);
      $("#ResponsableDPI").val(paciente.ResponsableDPI);
      $("#ResponsablePasaporte").val(paciente.ResponsablePasaporte);
      $("#ResponsableNacionalidad").val(paciente.ResponsableNacionalidad);
      $("#ResponsableOcupacion").val(paciente.ResponsableOcupacion);

      // Padre
      $("#NombrePadre").val(paciente.NombrePadre);
      $("#FechaNacimientoPadre").val(paciente.FechaNacimientoPadre);
      $("#EdadPadre").val(paciente.EdadPadre);
      $("#DPIPadre").val(paciente.DPIPadre);
      $("#DireccionPadre").val(paciente.DireccionPadre);
      $("#TelefonoPadre").val(paciente.TelefonoPadre);
      $("#CorreoPadre").val(paciente.CorreoPadre);
      $("#OcupacionPadre").val(paciente.OcupacionPadre);
      $("#EmpresaPadre").val(paciente.EmpresaPadre);
      $("#TelefonoEmpresaPadre").val(paciente.TelefonoEmpresaPadre);
      $("#DireccionEmpresaPadre").val(paciente.DireccionEmpresaPadre);

      // Madre
      $("#NombreMadre").val(paciente.NombreMadre);
      $("#FechaNacimientoMadre").val(paciente.FechaNacimientoMadre);
      $("#EdadMadre").val(paciente.EdadMadre);
      $("#DPIMadre").val(paciente.DPIMadre);
      $("#DireccionMadre").val(paciente.DireccionMadre);
      $("#TelefonoMadre").val(paciente.TelefonoMadre);
      $("#CorreoMadre").val(paciente.CorreoMadre);
      $("#OcupacionMadre").val(paciente.OcupacionMadre);
      $("#EmpresaMadre").val(paciente.EmpresaMadre);
      $("#TelefonoEmpresaMadre").val(paciente.TelefonoEmpresaMadre);
      $("#DireccionEmpresaMadre").val(paciente.DireccionEmpresaMadre);

      // Acompanante
      $("#AcompananteNombre").val(paciente.AcompananteNombre);
      $("#acompananteRelacion").val(paciente.AcompananteRelacion);
      $("#AcompananteTelefono").val(paciente.AcompananteTelefono);
      $("#AcompananteDPI").val(paciente.AcompananteDPI);
      $("#AcompananteDireccion").val(paciente.AcompananteDireccion);
      $("#AcompananteCorreo").val(paciente.AcompananteCorreo);
      $("#AcompananteOcupacion").val(paciente.AcompananteOcupacion);
      $("#AcompananteEmpresa").val(paciente.AcompananteEmpresa);
      $("#AcompananteTelefonoEmpresa").val(paciente.AcompananteTelefonoEmpresa);
      $("#AcompananteDireccionEmpresa").val(
        paciente.AcompananteDireccionEmpresa
      );
      $("#AcompananteTipoIdentificacion").val(
        paciente.AcompananteTipoIdentificacion
      );
      $("#AcompananteFechaNacimiento").val(paciente.AcompananteFechaNacimiento);
      $("#AcompananteEdad").val(paciente.AcompananteEdad);
      $("#AcompananteFechaIngreso").val(paciente.AcompananteFechaIngreso);
      $("#AcompananteAntiguedad").val(paciente.AcompananteAntiguedad);
    }
  });


  //self.nombreServicioBuscar.subscribe(function (value) {
  //    self.buscarServicioNombre();
  //});
  //self.buscarServicioNombre = function () {
  //    $(self.serviciosExistentes()).each(function (idx, vl) {
  //        if (self.nombreServicioBuscar().toLowerCase() == vl.ServicioNombre.toLowerCase()) {
  //            self.servicioAgregarSeleccionado(vl);
  //        }
  //    });
  //};
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
        if (data.Exitoso) {
          let pacienteId = parseInt($("#PacienteId").val());
          self.pacientes(data.Resultado);
          let citaId = $("#CitaId").val();
          if (citaId != null && citaId != undefined && citaId != "") {
            $(self.pacientes()).each(function (idx, vl) {
              if (parseInt(vl.Id) == pacienteId) {
                self.pacienteSeleccionado(vl);
              }
            });
          }
        } else {
          textoError.show();
        }
      },
      error: function (dataError) {
        textoCargando.hide();
        textoError.show();
        console.error(dataError);
      },
    });
  };
  self.consultarSeguros = function () {
    let textoCargando = $("#texto-cargando-seguros");
    let textoError = $("#texto-error-consultar-seguros");
    textoCargando.show();
    textoError.hide();

    $.ajax({
      url: "/Cita/ConsultarSeguros",
      method: "POST",
      success: function (dataResult) {
        textoCargando.hide();
        let data = JSON.parse(dataResult);
        // console.log(data);

        if (data.Exitoso) {
          self.seguros(data.Resultado);
          // console.log(self.seguros());
          // setTimeout(function () {
          //     $(".select2bs4").select2({
          //         theme: 'bootstrap4'
          //     });
          // }, 0);
        } else {
          textoError.show();
        }
      },
      error: function (dataError) {
        textoCargando.hide();
        textoError.show();
        // console.error(dataError);
      },
    });
  };
  self.seguroSeleccionado.subscribe(function (seguro) {
    if (seguro) {
      console.log("Seguro seleccionado: ", seguro);
    }
  });
  self.consultarDepartamentosExistentes = function () {
    showLoading();
    $.ajax({
      url: "/Cita/ConsultarDepartamentosExistentes",
      method: "POST",
      data: model,
      success: function (dataResult) {
        hideLoading();
        try {
          var data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.departamentosExistentes(data.Resultado);
          } else {
            alert(data.Mensaje);
          }
        } catch (e) {
          console.error("Error parsing response:", e);
          alert("Error processing response");
        }
      },
      error: function (dataError) {
        hideLoading();
        console.error(dataError);
        alert("Error in AJAX request");
      },
    });
  };
  self.departamentoSeleccionado.subscribe(function (departamentoId) {
    if (departamentoId) {
      self.consultarMunicipiosExistentes(departamentoId);
    } else {
      self.municipioSeleccionado(null);
    }
  });

  // Busca los municipios segun los departamentos existentes
  // self.consultarMunicipiosExistentes = function (Id) {
  //   $.ajax({
  //     url: "/Cita/ConsultarMunicipiosExistentes",
  //     method: "POST",
  //     data: { departamentoId: Id },
  //     success: function (dataResult) {
  //       try {
  //         var data = JSON.parse(dataResult);
  //         if (data.Exitoso) {
  //           self.municipiosExistentes(data.Resultado);
  //           let citaId = $("#CitaId").val();
  //           if (citaId && citaId.trim() !== "") {
  //             self.municipioSeleccionado(self.municipioPaciente());
  //           }
  //         } else {
  //           alert(data.Mensaje);
  //         }
  //       } catch (e) {
  //         console.error("Error parsing response:", e);
  //         alert("Error processing response");
  //       }
  //     },
  //     error: function (dataError) {
  //       hideLoading();
  //       console.error(dataError);
  //       alert("Error in AJAX request");
  //     },
  //   });
  // };


  self.consultarMunicipiosExistentes = function (Id, callback) {
    $.ajax({
      url: "/Cita/ConsultarMunicipiosExistentes",
      method: "POST",
      data: { departamentoId: Id },
      success: function (dataResult) {
        try {
          var data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.municipiosExistentes(data.Resultado);
            if (callback && typeof callback === "function") {
              callback();
            }
            let citaId = $("#CitaId").val();
            if (citaId && citaId.trim() !== "") {
              self.municipioSeleccionado(self.municipioPaciente());
            }
          } else {
            alert(data.Mensaje);
          }
        } catch (e) {
          console.error("Error parsing response:", e);
          alert("Error processing response");
        }
      },
      error: function (dataError) {
        hideLoading();
        console.error(dataError);
        alert("Error in AJAX request");
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
          "Error as�ncrono. Consulte con el administrador"
        );
        showModalRespuesta("mdl-respuesta");
      },
    });
  };

  self.consultarExamenesAgregadosCita = function () {
    //consultamos los examenes agregados previemente en la cita
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

  // self.servicioAgregarSeleccionado.subscribe(function (value) {
  //   self.consultarPreciosServicio(value.ServicioId);
  // });

  self.examenSeleccionado.subscribe(function (value) {
    self.consultarPreciosExamen(value.ServicioId);
  });
  self.consultarPreciosExamen = function (examenId) {
    showLoading();

    $.ajax({
      url: "/Venta/ConsultarPreciosExamen",
      method: "POST",
      data: {
        examenLabClinicoId: examenId,
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
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert(dataError);
      },
    });
  };

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

  // self.agregarServicio = function () {
  //   self.serviciosAgregados.push({
  //     Item: itemServicio,
  //     ServicioId: self.servicioAgregarSeleccionado().ServicioId,
  //     ServicioNombre: self.servicioAgregarSeleccionado().ServicioNombre,
  //     Cantidad: 1,
  //     ServicioDuracionHoras:
  //       self.servicioAgregarSeleccionado().ServicioDuracionHoras,
  //     ServicioDuracionMinutos:
  //       self.servicioAgregarSeleccionado().ServicioDuracionMinutos,
  //     ServicioDuracionText:
  //       self.servicioAgregarSeleccionado().ServicioDuracionText,
  //     PrecioId:
  //       self.precioServicioAgregarSeleccionado() == undefined
  //         ? null
  //         : self.precioServicioAgregarSeleccionado().PrecioId,
  //     PrecioValor: ko.observable(
  //       self.precioServicioAgregarSeleccionado() == undefined
  //         ? null
  //         : self.precioServicioAgregarSeleccionado().PrecioValor
  //     ),
  //     PrecioNombre:
  //       self.precioServicioAgregarSeleccionado() == undefined
  //         ? null
  //         : self.precioServicioAgregarSeleccionado().PrecioNombre,
  //     PrecioNombreValor:
  //       self.precioServicioAgregarSeleccionado() == undefined
  //         ? null
  //         : self.precioServicioAgregarSeleccionado().PrecioNombreValor,
  //     PrecioValorCubiertoSeguro: ko.observable(0),
  //     PrecioValorCopago: ko.observable(
  //       self.precioServicioAgregarSeleccionado() == undefined
  //         ? null
  //         : self.precioServicioAgregarSeleccionado().PrecioValor
  //     ),
  //     Nuevo: true,
  //   });
  //   itemServicio++;
  //   self.actualizarTotales();
  //   self.validarDisponibilidadEmpleado();
  // };
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
  //btn agregar examen
  self.agregarExamen = function () {
    self.examenesAgregados.push({
      Item: itemExamen,
      ExamenId: self.examenSeleccionado().ExamenId,
      ExamenNombre: self.examenSeleccionado().ExamenNombre,
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
      //PrecioNombreValor: self.precioServicioAgregarSeleccionado() == undefined ? null :
      //    self.precioServicioAgregarSeleccionado().PrecioNombreValor,
      Nuevo: true,
    });
    itemExamen++;
    //self.actualizarTotales();
    self.actualizarTotales();
  };
  self.quitarExamen = function (value) {
    $(self.examenesAgregados()).each(function (idx, examen) {
      if (value.Item == examen.Item) {
        self.examenesAgregados.splice(idx, 1);
        return false;
      }
    });
    //self.actualizarTotales();
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
    // Sincronizar FechaHora con inputs de Hospi (si aplican)
    var fechaHoraHospi = self._getFechaHoraHospi();
    if (fechaHoraHospi && $("#FechaHora").length) {
      $("#FechaHora").val(fechaHoraHospi);
    }

    var model = {
      // Datos de la cita
      CitaId: $("#CitaId").val(),
      PacienteNombre: $("#PacienteNombre").val(),
      PacienteId: $("#PacienteId").val(),
      EspecialidadId: $("#EspecialidadId").val(),
      HabitacionId: $("#HabitacionId").val(),
      CategoriaHabitacionId: $("#CategoriaHabitacionId").val(),

      Coex: $("#esCOEX").val(),
      EmpleadoId: $("#EmpleadoId").val(),
      SucursalId: $("#SucursalId").val(),
      Motivo: $("#Motivo").val(),
      NombreEncargado: $("#NombreEncargado").val(),
      DPIEncargado: $("#DPIEncargado").val(),
      FechaHora: $("#FechaHora").val(),
      EstadoCita: $("#EstadoCita").val(),
      CodigoCita: self.seguroSeleccionado(),
      CitaTipoAtencion: $("#CitaTipoAtencion").val(),
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
      no_IGGS: $("#no_IGGS").val(),
      NumeroTurno: $("#NumeroTurno").val(),
      NivelPrioridadCita: $("#NivelPrioridadCita").val(),
      EtniaPaciente: $("#EtniaPaciente").val(),
      OrigenPaciente: $("#origenPaciente").val(),
      ReligionPaciente: $("#religionPaciente").val(),

      Dia: $("#Dia").val(),
      Hora: $("#Hora").val(),
      Examenes: self.examenesAgregados(),

      // Responsable de pago
      ResponsableNit: $("#ResponsableNit").val(),
      ResponsableNombre: $("#ResponsableNombre").val(),
      ResponsableDireccion: $("#ResponsableDireccion").val(),
      ResponsableCorreo: $("#ResponsableCorreo").val(),
      ResponsableDPI: $("#ResponsableDPI").val(),
      ResponsableTelefono: $("#ResponsableTelefono").val(),
      ResponsablePasaporte: $("#ResponsablePasaporte").val(),
      ResponsableNacionalidad: $("#ResponsableNacionalidad").val(),
      ResponsableOcupacion: $("#ResponsableOcupacion").val(),

      // Datos del Acompanante
      AcompananteNombre: $("#AcompananteNombre").val(),
      AcompananteRelacion: $("#acompananteRelacion").val(),

      AcompananteTelefono: $("#AcompananteTelefono").val(),
      AcompananteDPI: $("#AcompananteDPI").val(),
      AcompananteDireccion: $("#AcompananteDireccion").val(),
      AcompananteCorreo: $("#AcompananteCorreo").val(),
      AcompananteOcupacion: $("#AcompananteOcupacion").val(),
      AcompananteEmpresa: $("#AcompananteEmpresa").val(),
      AcompananteTelefonoEmpresa: $("#AcompananteTelefonoEmpresa").val(),
      AcompananteDireccionEmpresa: $("#AcompananteDireccionEmpresa").val(),
      AcompananteTipoIdentificacion: $("#AcompananteTipoIdentificacion").val(),
      AcompananteFechaNacimiento: $("#AcompananteFechaNacimiento").val(),
      AcompananteEdad: $("#AcompananteEdad").val(), // Este campo es solo de lectura, pero si es necesario, puedes capturarlo de esta manera.
      AcompananteFechaIngreso: $("#AcompananteFechaIngreso").val(),
      AcompananteAntiguedad: $("#AcompananteAntiguedad").val(), // Este también es solo de lectura

      // Datos del padre
      NombrePadre: $("#NombrePadre").val(),
      FechaNacimientoPadre: $("#FechaNacimientoPadre").val(),
      EdadPadre: $("#EdadPadre").val(),
      DPIPadre: $("#DPIPadre").val(),
      DireccionPadre: $("#DireccionPadre").val(),
      TelefonoPadre: $("#TelefonoPadre").val(),
      CorreoPadre: $("#CorreoPadre").val(),
      OcupacionPadre: $("#OcupacionPadre").val(),
      EmpresaPadre: $("#EmpresaPadre").val(),
      TelefonoEmpresaPadre: $("#TelefonoEmpresaPadre").val(),
      DireccionEmpresaPadre: $("#DireccionEmpresaPadre").val(),

      // Datos de la madre
      NombreMadre: $("#NombreMadre").val(),
      FechaNacimientoMadre: $("#FechaNacimientoMadre").val(),
      EdadMadre: $("#EdadMadre").val(),
      DPIMadre: $("#DPIMadre").val(),
      DireccionMadre: $("#DireccionMadre").val(),
      TelefonoMadre: $("#TelefonoMadre").val(),
      CorreoMadre: $("#CorreoMadre").val(),
      OcupacionMadre: $("#OcupacionMadre").val(),
      EmpresaMadre: $("#EmpresaMadre").val(),
      TelefonoEmpresaMadre: $("#TelefonoEmpresaMadre").val(),
      DireccionEmpresaMadre: $("#DireccionEmpresaMadre").val(),

      // Dirección del paciente
      Direccion: $("#Direccion").val(),
      DepartamentoId: $("#departamentoSeleccionado").val(),
      MunicipioId: $("#municipioSeleccionado").val(),
      Procedimiento: $("#Procedimiento").val(),

      // Campos de Sala de Operaciones — nombre para mostrar, ID para lookup posterior
      Anestesista: $("#Anestesista option:selected").text() !== "Seleccione..." ? $("#Anestesista option:selected").text() : "",

      PrimerAyudante: $("#PrimerAyudante option:selected").text() !== "Seleccione..." ? $("#PrimerAyudante option:selected").text() : "",

      SegundoAyudante: $("#SegundoAyudante option:selected").text() !== "Seleccione..." ? $("#SegundoAyudante option:selected").text() : "",

      Instrumentista: $("#Instrumentista option:selected").text() !== "Seleccione..." ? $("#Instrumentista option:selected").text() : "",

      Circulante: $("#Circulante option:selected").text() !== "Seleccione..." ? $("#Circulante option:selected").text() : "",


      AnestesistaId: $("#Anestesista").val() || null,
      PrimerAyudanteId: $("#PrimerAyudante").val() || null,
      SegundoAyudanteId: $("#SegundoAyudante").val() || null,
      InstrumentistaId: $("#Instrumentista").val() || null,
      CirculanteId: $("#Circulante").val() || null,

    };
    return model;
  };

  // Función de agendar cita
  self.agendarCita = function () {
    // Obtenemos la edad del paciente desde el campo calculado
    // let edadTexto = $("#PacienteEdad").val();
    // let edadAños = parseInt(edadTexto.split(" ")[0], 10);  // Extrae solo el número de años
    // let encargado = $("#NombreEncargado").val().trim();

    // Verificamos si la edad es menor de 13 años y si el campo encargado está vacío
    // if (edadAños < 13 && encargado === "") {
    //     alert("El encargado es obligatorio para pacientes menores de 13 años.");
    //     $("#NombreEncargado").focus();
    //     return;  // Detenemos la ejecución de agendarCita si el campo está vacío
    // }

    // Continuamos con la validación y el agendamiento de la cita si la validación anterior pasa
    if (self.validateModel()) {
      var newPacienteId = $("#PacienteId").val();

      var continuarConfirmacion = function () {
        self.validarDisponibilidadEmpleado(function (ok) {
          if (!ok) return;
          self.confirmarAgendarCita();
        });
      };

      if (newPacienteId == 0) {
        var datoDPI = { Dpi: $("#dpiPacienteSeleccionado").val() };
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

  // Función para confirmar y agendar la cita
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

            if (!model.Coex || model.Coex === "NO") {
              $.ajax({
                url: "/Cita/AgendarCitaHospi",
                method: "POST",
                data: model,
                success: function (dataResult) {
                  var data = JSON.parse(dataResult);
                  if (data.Exitoso) {
                    var consultaId = data.ConsultaId;
                    var citaId = data.CitaId;
                    /*   window.location.href =
                        "/Hospitalizacion/Hospitalizar?habitacionId=" +
                        model.HabitacionId +
                        "&consultaId=" +
                        consultaId +
                        "&citaId=" +
                        citaId; */
                    // window.location.href =
                    //   "/Hospitalizacion/Hospitalizar?habitacionId=" +
                    //   model.HabitacionId +
                    //   "&citaId=" +
                    //   citaId;
                    if (data.EsSalaOperaciones) {
                      // Caso Sala de Operaciones → ir al calendario
                      window.location.href = data.RedirectUrl;
                    } else {
                      // Caso Admisión normal → ir a Hospitalizar
                      window.location.href = "/Hospitalizacion/Hospitalizar?habitacionId=" +
                        model.HabitacionId +
                        "&citaId=" + data.CitaId;
                    }
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
            }
            if (model.Coex == "SI") {
              $.ajax({
                url: "/Cita/AgendarCitaConsulta",
                method: "POST",
                data: model,
                success: function (dataResult) {
                  var data = JSON.parse(dataResult);
                  if (data.Exitoso) {
                    var citaId = data.CitaId;
                    window.location.href =
                      "/Consultas/IniciarConsulta?citaId=" + citaId;
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
            }
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
    // Función para generar un folio único
    function generarFolioUnico() {
      var fecha = new Date();
      var año = fecha.getFullYear();
      var mes = (fecha.getMonth() + 1).toString().padStart(2, "0");
      var dia = fecha.getDate().toString().padStart(2, "0");
      var fechaFormato = `${año}${mes}${dia}`;

      // Generar 4 números aleatorios entre 0000 y 9999
      var numerosAleatorios = Math.floor(1000 + Math.random() * 9000);

      return `PACS-${fechaFormato}-${numerosAleatorios}`;
    }

    // Obtener datos del formulario
    var pacienteNombre = $("#nombrePacienteSeleccionado").val().split(" ");
    var fechaNacimiento = $("#FechaNacimiento").val();
    var sexo =
      $("#SexoId").val() === "1" ? "M" : $("#SexoId").val() === "2" ? "F" : "O";
    var telefono = $("#Telefono").val();
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
            //// Paciente encontrado, puedes acceder a la informaci�n en data.paciente
            //console.log("El turno Seleccionado es:", data.turnoactual);
            ////Console.log("El Turno es:" + data.turnoactual + "");
            //console.log("El turno Seleccionado es:", data.codEspecialidad);
            $("#NumeroTurno").val(
              data.codEspecialidad + "00" + data.turnoactual
            );
          } else {
            mensajeEmergenteError("Fallo el turno, no hay datos");
            // Paciente no encontrado
            //console.log("Fallo el turno no hay datos");
          } //fin validacion si es paciente nuevo o existente
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



  // Función para consultar servicios existentes
  self.consultarServiciosExistentes = function () {
    showLoading();
    $.ajax({
      url: "/Cita/ConsultarServiciosExistentes",
      method: "POST",
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.serviciosExistentes(data.Resultado);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("Error al consultar servicios");
      }
    });
  };

  // Función para consultar precios de un servicio
  self.consultarPreciosServicio = function (servicioId) {
    if (!servicioId) return;
    showLoading();
    $.ajax({
      url: "/Cita/ConsultarPreciosServicio",
      method: "POST",
      data: { servicioId: servicioId, fecha: self._getFechaHoraHospi() },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          var preciosFiltrados = data.Resultado.filter(function (item) {
            return item.PrecioNombre === "NORMAL";
          });
          self.preciosServicioSeleccionado(preciosFiltrados);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("Error al consultar precios");
      }
    });
  };
  // Suscripción para cuando se selecciona un servicio, consultar sus precios
  self.servicioAgregarSeleccionado.subscribe(function (servicio) {
    if (servicio && servicio.ServicioId) {
      self.consultarPreciosServicio(servicio.ServicioId);
    } else {
      self.preciosServicioSeleccionado([]);
    }
  });

  // Agregar servicio a la lista
  self.agregarServicio = function () {
    if (!self.servicioAgregarSeleccionado()) {
      alert("Seleccione un servicio");
      return;
    }
    if (!self.precioServicioAgregarSeleccionado()) {
      alert("Seleccione un precio");
      return;
    }

    var nuevoServicio = {
      Item: self.serviciosAgregados().length + 1,
      ServicioId: self.servicioAgregarSeleccionado().ServicioId,
      ServicioNombre: self.servicioAgregarSeleccionado().ServicioNombre,
      Cantidad: 1,
      ServicioDuracionHoras: self.servicioAgregarSeleccionado().ServicioDuracionHoras || 0,
      ServicioDuracionMinutos: self.servicioAgregarSeleccionado().ServicioDuracionMinutos || 0,
      PrecioId: self.precioServicioAgregarSeleccionado().PrecioId,
      PrecioValor: ko.observable(self.precioServicioAgregarSeleccionado().PrecioValor),
      PrecioNombre: self.precioServicioAgregarSeleccionado().PrecioNombre,
      PrecioValorCubiertoSeguro: ko.observable(0),
      PrecioValorCopago: ko.observable(self.precioServicioAgregarSeleccionado().PrecioValor),
      Nuevo: true
    };
    self.serviciosAgregados.push(nuevoServicio);
    self.actualizarTotales();
    self.validarDisponibilidadEmpleado();
  };

  // Quitar servicio
  self.quitarServicio = function (servicio) {
    self.serviciosAgregados.remove(servicio);
    self.actualizarTotales();
    self.validarDisponibilidadEmpleado();
  };


  // self.actualizarTotales = function () {
  //   //Calculo de duracion total
  //   let totalDuracionHoras = 0;
  //   let totalDuracionMinutos = 0;
  //   $(self.serviciosAgregados()).each(function (idx, servicio) {
  //     let cantidad = isNaN(servicio.Cantidad) ? 1 : servicio.Cantidad;

  //     if (
  //       servicio.ServicioDuracionHoras != undefined &&
  //       servicio.ServicioDuracionHoras != null
  //     ) {
  //       totalDuracionHoras +=
  //         parseInt(servicio.ServicioDuracionHoras) * cantidad;
  //     }
  //     if (
  //       servicio.ServicioDuracionMinutos != undefined &&
  //       servicio.ServicioDuracionMinutos != null
  //     ) {
  //       totalDuracionMinutos +=
  //         parseInt(servicio.ServicioDuracionMinutos) * cantidad;
  //     }

  //     //Calculo de valores
  //     let valorTotal = servicio.PrecioValor();
  //     let valorCubiertoSeguro = servicio.PrecioValorCubiertoSeguro();
  //     let valorCopago = 0;
  //     if (valorTotal == undefined || valorTotal == null || isNaN(valorTotal)) {
  //       valorTotal = 0;
  //     }
  //     if (
  //       valorCubiertoSeguro == undefined ||
  //       valorCubiertoSeguro == null ||
  //       isNaN(valorCubiertoSeguro)
  //     ) {
  //       valorCubiertoSeguro = 0;
  //     }
  //     valorCopago = valorTotal - valorCubiertoSeguro;
  //     servicio.PrecioValorCopago(valorCopago);
  //   });
  //   $(self.examenesAgregados()).each(function (idx, examen) {
  //     //Calculo de valores
  //     let valorTotal = examen.PrecioValor();
  //     let valorCubiertoSeguro = examen.PrecioValorCubiertoSeguro();
  //     let valorCopago = 0;
  //     if (valorTotal == undefined || valorTotal == null || isNaN(valorTotal)) {
  //       valorTotal = 0;
  //     }
  //     if (
  //       valorCubiertoSeguro == undefined ||
  //       valorCubiertoSeguro == null ||
  //       isNaN(valorCubiertoSeguro)
  //     ) {
  //       valorCubiertoSeguro = 0;
  //     }
  //     valorCopago = valorTotal - valorCubiertoSeguro;
  //     examen.PrecioValorCopago(valorCopago);
  //   });

  //   // Normalizar minutos a horas/minutos
  //   if (totalDuracionMinutos >= 60) {
  //     totalDuracionHoras += Math.floor(totalDuracionMinutos / 60);
  //     totalDuracionMinutos = totalDuracionMinutos % 60;
  //   }

  //   // Setear observables de duración total (NECESARIO para validar traslapes)
  //   self.totalDuracionHoras(totalDuracionHoras);
  //   self.totalDuracionMinutos(totalDuracionMinutos);
  //   self.totalDuracion((totalDuracionHoras * 60) + totalDuracionMinutos);

  //   //Calculo de precio total
  //   let totalValor = 0;
  //   $(self.serviciosAgregados()).each(function (idx, servicio) {
  //     let cantidad = isNaN(servicio.Cantidad) ? 1 : servicio.Cantidad;

  //     // PrecioValor es observable (ko.observable)
  //     let precio = 0;
  //     if (typeof servicio.PrecioValor === "function") {
  //       precio = servicio.PrecioValor();
  //     } else if (servicio.PrecioValor != undefined && servicio.PrecioValor != null) {
  //       precio = servicio.PrecioValor;
  //     }

  //     if (precio == undefined || precio == null || isNaN(precio)) {
  //       precio = 0;
  //     }

  //     totalValor += parseFloat(precio) * cantidad;
  //   });
  //   self.totalValor("GTQ " + totalValor);
  // };


  self.actualizarTotales = function () {
    // Cálculo de duración total (horas y minutos)
    let totalDuracionHoras = 0;
    let totalDuracionMinutos = 0;

    $(self.serviciosAgregados()).each(function (idx, servicio) {
      let cantidad = isNaN(servicio.Cantidad) ? 1 : servicio.Cantidad;

      // Sumar horas
      if (servicio.ServicioDuracionHoras != undefined && servicio.ServicioDuracionHoras != null) {
        totalDuracionHoras += parseInt(servicio.ServicioDuracionHoras) * cantidad;
      }
      // Sumar minutos
      if (servicio.ServicioDuracionMinutos != undefined && servicio.ServicioDuracionMinutos != null) {
        totalDuracionMinutos += parseInt(servicio.ServicioDuracionMinutos) * cantidad;
      }

      // Calcular copago si aplica (por si se integra seguro en el futuro)
      let valorTotal = servicio.PrecioValor();
      let valorCubiertoSeguro = servicio.PrecioValorCubiertoSeguro();
      let valorCopago = 0;

      if (valorTotal == undefined || valorTotal == null || isNaN(valorTotal)) {
        valorTotal = 0;
      }
      if (valorCubiertoSeguro == undefined || valorCubiertoSeguro == null || isNaN(valorCubiertoSeguro)) {
        valorCubiertoSeguro = 0;
      }
      valorCopago = valorTotal - valorCubiertoSeguro;
      servicio.PrecioValorCopago(valorCopago);
    });

    // Normalizar minutos a horas/minutos
    if (totalDuracionMinutos >= 60) {
      totalDuracionHoras += Math.floor(totalDuracionMinutos / 60);
      totalDuracionMinutos = totalDuracionMinutos % 60;
    }

    // Asignar los observables de duración total
    self.totalDuracionHoras(totalDuracionHoras);
    self.totalDuracionMinutos(totalDuracionMinutos);
    self.totalDuracion((totalDuracionHoras * 60) + totalDuracionMinutos);

    // Cálculo del valor total de los servicios
    let totalValor = 0;
    $(self.serviciosAgregados()).each(function (idx, servicio) {
      let cantidad = isNaN(servicio.Cantidad) ? 1 : servicio.Cantidad;

      // Obtener precio (observable o valor directo)
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

    self.totalValor("GTQ " + totalValor.toFixed(2));
  };

  self.consultarExamenesExistentes = function () {
    $.ajax({
      method: "POST",
      url: "/Venta/ConsultarExamenesExistentes",
      data: model,
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.examenesExistentes(data.Resultado);
        } else {
          hideLoading();
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };
};

var citaHospiVm = new CitaHospiVM();
ko.applyBindings(citaHospiVm);

$(function () {
  citaHospiVm.consultarPacientes();
  citaHospiVm.consultarSeguros();
  console.log(citaHospiVm.seguroSeleccionado());

  citaHospiVm.consultarServiciosExistentes();
  citaHospiVm.consultarExamenesExistentes();
  citaHospiVm.consultarDepartamentosExistentes();

  let citaId = $("#CitaId").val();
  if (citaId && citaId.trim() !== "") {
    citaHospiVm.consultarServiciosAgregadosCita();
    citaHospiVm.consultarExamenesAgregadosCita();
  }

  // Validar disponibilidad al cambiar el profesional y/o fecha/hora (mostrar mensaje en la vista)
  $("#EmpleadoId").on("change", function () {
    citaHospiVm.validarDisponibilidadEmpleado();
  });
  $("#fecha").on("change", function () {
    citaHospiVm.validarDisponibilidadEmpleado();
  });
  $("#hora").on("change", function () {
    citaHospiVm.validarDisponibilidadEmpleado();
  });

  $("#fechacita").daterangepicker({
    timePicker: true,
    timePickerIncrement: 30,
    singleDatePicker: true,
    locale: {
      format: "MM/DD/YYYY hh:mm A",
    },
  });
  //$('#nombre-servicio-buscar').on('keypress', function (e) {
  //    var keycode = e.which;
  //    if (keycode == '13') {
  //        citaHospiVm.buscarServicioNombre();
  //    }
  //});

  //Cita tipo de Atencion
  citaHospiVm.citaTipoAtencion($("#CitaTipoAtencion").val());
  $("#CitaTipoAtencion").on("change", function () {
    citaHospiVm.citaTipoAtencion($(this).val());
  });



  $("#EmpleadoId").on("change", function () {
    var empleadoId = $(this).val();
    if (empleadoId && empleadoId !== "") {
      $.ajax({
        url: "/Cita/GetEspecialidadByEmpleado",
        method: "GET",
        data: { empleadoId: empleadoId },
        success: function (response) {
          if (response.exitoso) {
            if (response.especialidadId) {
              $("#EspecialidadId").val(response.especialidadId).trigger("change");
            } else {
              $("#EspecialidadId").val(null).trigger("change");
            }
          } else {
            console.warn("No se pudo obtener especialidad: " + response.mensaje);
          }
        },
        error: function (err) {
          console.error("Error al obtener especialidad", err);
        }
      });
    } else {
      $("#EspecialidadId").val(null).trigger("change");
    }
  });


});

function getEdad() {
  let hoy = new Date();
  let fechaNacimiento = new Date($("#FechaNacimiento").val());

  if (!isNaN(fechaNacimiento)) {
    // Verifica que la fecha de nacimiento sea válida
    let edadAnios = hoy.getFullYear() - fechaNacimiento.getFullYear();
    let diferenciaMeses = hoy.getMonth() - fechaNacimiento.getMonth();
    let diferenciaDias = hoy.getDate() - fechaNacimiento.getDate();

    // Ajuste si el mes actual es menor al mes de nacimiento
    if (diferenciaMeses < 0 || (diferenciaMeses === 0 && diferenciaDias < 0)) {
      edadAnios--;
      diferenciaMeses += diferenciaMeses < 0 ? 12 : 0;
    }

    // Ajuste si el día actual es menor al día de nacimiento
    if (diferenciaDias < 0) {
      diferenciaMeses--;
      let mesAnterior = new Date(hoy.getFullYear(), hoy.getMonth(), 0); // Último día del mes anterior
      diferenciaDias += mesAnterior.getDate();
    }

    // Determina los textos correctos en singular o plural con mayúscula inicial
    let textoAnios = edadAnios === 1 ? "Año" : "Años";
    let textoMeses = diferenciaMeses === 1 ? "Mes" : "Meses";
    let textoDias = diferenciaDias === 1 ? "Día" : "Días";

    // Mostrar la edad en el formato "XX Años YY Meses ZZ Días"
    let edadCompleta = `${edadAnios} ${textoAnios} ${diferenciaMeses} ${textoMeses} ${diferenciaDias} ${textoDias}`;
    citaHospiVm.edadAniosPaciente(edadAnios);
    $("#PacienteEdad").val(edadCompleta);
  } else {
    $("#PacienteEdad").val(""); // Limpia el campo si la fecha no es válida
  }
}

var windowObjectReference;
var windowFeatures =
  "menubar=yes,location=yes,resizable=yes,scrollbars=yes,status=yes";