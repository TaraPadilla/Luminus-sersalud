var HospitalizarVM = function () {
  let model = {};
  var self = this;
  self.terminoBusquedaNombre = ko.observable();
  self.terminoBusquedaDpi = ko.observable();
  self.pacientesExistentes = ko.observableArray();

  self.urlArchivoConsentimiento = ko.observable();

  //#region Especialidades

  self.especialidades = ko.observableArray();
  self.especialidadSeleccionada = ko.observable();
  self.especialidadId = ko.observable();
  self.especialidadNombre = ko.observable();

  //#endregion

  //#region Datos del Paciente
  self.pacienteSeleccionado = ko.observable();
  self.pacienteId = ko.observable();
  self.pacienteNombre = ko.observable();
  self.pacienteDpi = ko.observable();
  self.pacienteTelefono = ko.observable();
  self.pacienteFechaNacimiento = ko.observable();
  self.pacienteCorreo = ko.observable("");
  self.edadCalculada = ko.observable();

  // Función para calcular la edad
  self.calcularEdad = function () {
    var fechaNacimiento = new Date(self.pacienteFechaNacimiento());
    if (!isNaN(fechaNacimiento.getTime())) {
      var hoy = new Date();
      var años = hoy.getFullYear() - fechaNacimiento.getFullYear();
      var meses = hoy.getMonth() - fechaNacimiento.getMonth();
      var dias = hoy.getDate() - fechaNacimiento.getDate();

      if (dias < 0) {
        meses--;
        dias += new Date(hoy.getFullYear(), hoy.getMonth(), 0).getDate();
      }

      if (meses < 0) {
        años--;
        meses += 12;
      }

      self.edadCalculada(
        años +
        " Año" +
        (años !== 1 ? "s" : "") +
        " " +
        meses +
        " Mes" +
        (meses !== 1 ? "es" : "") +
        " y " +
        dias +
        " Dia" +
        (dias !== 1 ? "s" : ""),
      );
    } else {
      self.edadCalculada(""); // Vacío si no hay fecha válida
    }
  };

  // Observa los cambios en la fecha de nacimiento para calcular automáticamente
  self.pacienteFechaNacimiento.subscribe(self.calcularEdad);
  //#endregion

  //Tarifas
  self.tarifas = ko.observableArray();
  self.tarifaSeleccionada = ko.observable();
  self.tarifaNombre = ko.observable();
  self.tarifaValor = ko.observable();

  self.periodo = ko.observable();
  self.observaciones = ko.observable();

  self.firmaPacienteBase64 = ko.observable(null);
  self.firmaResponsableBase64 = ko.observable(null);
  self.urlFirmaPaciente = ko.observable('');
  self.urlFirmaResponsable = ko.observable('');

  self.listaUsuarios = ko.observableArray();

  //#region Variables PASO a PASO

  self.pasoEspecialidadAbierto = ko.observable(false);
  self.pasoTarifasAbierto = ko.observable(false);
  self.pasoPacienteAbierto = ko.observable(false);
  self.pasoEstadiaAbierto = ko.observable(false);
  self.pasoConsentimientoAbierto = ko.observable(false);
  self.pasoCuestionarioAbierto = ko.observable(false);
  self.pasoListaChequeoAbierto = ko.observable(false);
  self.pasoFirmasAbierto = ko.observable(false);
  self.pasoResumenAbierto = ko.observable(false);

  //#endregion


  // ========== SERVICIOS ==========
  self.serviciosExistentes = ko.observableArray([]);
  self.servicioAgregarSeleccionado = ko.observable();
  self.precioServicioAgregarSeleccionado = ko.observable();
  self.preciosServicioSeleccionado = ko.observableArray([]);
  self.serviciosAgregados = ko.observableArray([]);
  self.itemServicio = 1;


  self.cuestionario = {
    // Datos del paciente
    pacienteRegistro: ko.observable(""),
    pacienteNombre: ko.observable(""),
    pacienteEdad: ko.observable(""),
    fechaCuestionario: ko.observable(moment().format("YYYY-MM-DD")),
    peso: ko.observable(""),
    estatura: ko.observable(""),
    fechaUltimaRegla: ko.observable(""),
    fechaProcedimiento: ko.observable(""),
    procedimientoProgramado: ko.observable(""),
    cirujano: ko.observable(""),

    // Antecedentes - Columna Izquierda
    pa_alergia: ko.observable(""),
    pa_alergiaCual: ko.observable(""),
    pa_fuma: ko.observable(""),
    pa_fumaCuanto: ko.observable(""),
    pa_drogas: ko.observable(""),
    pa_drogasCuales: ko.observable(""),
    pa_alcohol: ko.observable(""),
    pa_alcoholCuanto: ko.observable(""),
    pa_embarazo: ko.observable(""),
    pa_transfusion: ko.observable(""),
    pa_asma: ko.observable(""),
    pa_pulmones: ko.observable(""),
    pa_corazon: ko.observable(""),

    // Antecedentes - Columna Derecha
    pa_ataqueCardiaco: ko.observable(""),
    pa_angina: ko.observable(""),
    pa_soplo: ko.observable(""),
    pa_presion: ko.observable(""),
    pa_higado: ko.observable(""),
    pa_rinones: ko.observable(""),
    pa_diabetes: ko.observable(""),
    pa_epilepsia: ko.observable(""),
    pa_derrame: ko.observable(""),
    pa_tiroides: ko.observable(""),
    pa_anestesico: ko.observable(""),
    pa_aceptaTransfusion: ko.observable(""),

    // Información adicional
    ai_medicamentos: ko.observable(""),
    ai_actividad: ko.observable(""),
    ai_actividadDetalle: ko.observable(""),
    ai_operacionesPrevias: ko.observable(""),
    ai_comentarios: ko.observable(""),
  };
  //#endregion

  //#region Lista de Chequeo
  self.listaChequeo = {
    // Encabezado
    fechaChequeo: ko.observable(moment().format("YYYY-MM-DD")),
    horaChequeo: ko.observable(moment().format("HH:mm")),

    // ENTRADA - Paciente
    ci_nombreConfirma: ko.observable(""),
    ci_apellidoConfirma: ko.observable(""),
    ci_fechaNacConfirma: ko.observable(""),
    ci_consentimiento: ko.observable(""),
    ci_operacion: ko.observable(""),
    ci_ladoOperar: ko.observable(""),
    ci_sitioMarcado: ko.observable(""),
    ci_alergia: ko.observable(""),

    // ENTRADA - Anestesiólogo
    ci_evalPreanestesica: ko.observable(""),
    ci_accesoIV: ko.observable(""),
    ci_equipoAnestesia: ko.observable(""),
    ci_medicamentos: ko.observable(""),
    ci_oximetro: ko.observable(""),
    ci_equipoAspiracion: ko.observable(""),
    ci_viaAerea: ko.observable(""),

    // PAUSA - Cirujano
    cp_presentacion: ko.observable(""),
    cp_nombrePacienteCirujano: ko.observable(""),
    cp_nombreCirugia: ko.observable(""),
    cp_eventosCriticos: ko.observable(""),
    cp_tiempoDuracion: ko.observable(""),
    cp_imagenesDiagnosticas: ko.observable(""),
    cp_perdidaSangre: ko.observable(""),

    // PAUSA - Instrumentista
    cp_esterilidad: ko.observable(""),
    cp_materialesAdicionales: ko.observable(""),

    // PAUSA - Anestesiólogo
    cp_eventosCriticosAnest: ko.observable(""),
    cp_profilaxisAntibiotica: ko.observable(""),
    cp_tromboprofilaxis: ko.observable(""),
    cp_manejoDolor: ko.observable(""),

    // SALIDA - Enfermera
    cs_nombreOperacion: ko.observable(""),
    cs_nombreEnfermera: ko.observable(""),
    cs_recuentoCompleto: ko.observable(""),
    cs_etiquetadoMuestras: ko.observable(""),

    // SALIDA - Recuperación
    cs_repasoPostOp: ko.observable(""),
    cs_porQue: ko.observable(""),
    cs_traslado: ko.observable(""),
    cs_complicaciones: ko.observable(""),
    cs_servicioNumCama: ko.observable(""),
  };

  self.urlArchivoConsentimiento.subscribe(function (nuevoValor) {
    if (self.listaChequeo && nuevoValor && nuevoValor !== "") {
      self.listaChequeo.ci_consentimiento("SI");
      console.log("Consentimiento detectado, check marcado como SI");
    }
  });

  //#region Funciones PASO a PASO

  // self.ocultarPasos = function () {
  //   $(".paso").hide();
  // };

  // self.pasoEspecialidad = function () {
  //   $("#texto-validacion-especialidad").hide();
  //   self.ocultarPasos();
  //   $("#paso-especialidad").show("fast");
  //   if (!self.pasoEspecialidadAbierto()) {
  //     self.pasoEspecialidadAbierto(true);
  //   }
  // };
  // self.pasoTarifas = function () {
  //   // $("#texto-validacion-tarifas").hide();
  //   // let especialidad = self.especialidadSeleccionada();
  //   // console.log(especialidad);
  //   // if (especialidad == null || especialidad == undefined) {
  //   //     $("#texto-validacion-especialidad").show();
  //   //     return false;
  //   // } else {
  //   //     if (especialidad.Id == null || especialidad.Id == undefined) {
  //   //         self.especialidadId(null);
  //   //         self.especialidadNombre(especialidad);
  //   //     } else {
  //   //         self.especialidadId(especialidad.Id);
  //   //         self.especialidadNombre(especialidad.NombreEspecialidad);
  //   //     }
  //   // }

  //   self.ocultarPasos();
  //   $("#paso-tarifas").show("fast");
  //   if (!self.pasoTarifasAbierto()) {
  //     self.pasoTarifasAbierto(true);
  //   }
  // };
  // self.pasoPaciente = function () {
  //   $("#texto-validacion-tarifas").hide();
  //   let tarifa = self.tarifaSeleccionada();
  //   if (!tarifa) {
  //     // Si no hay una tarifa seleccionada
  //     $("#texto-validacion-tarifas").show();
  //     return false;
  //   }
  //   self.tarifaNombre(tarifa.NombreTarifa);
  //   self.tarifaValor(tarifa.ValorTarifa);

  //   self.ocultarPasos();
  //   $("#paso-paciente").show("fast");
  //   if (!self.pasoPacienteAbierto()) {
  //     self.consultarPacientes();
  //     self.pasoPacienteAbierto(true);
  //   }
  // };

  // self.tarifaSeleccionada.subscribe(function (nuevaTarifa) {
  //   if (!nuevaTarifa) {
  //     // Seleccionar la primera tarifa si está disponible
  //     if (self.tarifas().length > 0) {
  //       self.tarifaSeleccionada(self.tarifas()[0]);
  //     }
  //   }
  // });

  // self.pasoEstadia = function () {
  //   // let pacienteNombre = self.pacienteNombre();
  //   // if (pacienteNombre == null || pacienteNombre == undefined || pacienteNombre.trim() == '') {
  //   //     $("#texto-validacion-paciente").show();
  //   //     return false;
  //   // }

  //   self.ocultarPasos();
  //   $("#paso-estadia").show("fast");
  //   if (!self.pasoEstadiaAbierto()) {
  //     self.pasoEstadiaAbierto(true);
  //   }
  // };
  // self.pasoConsentimiento = function () {
  //   self.ocultarPasos(); // Oculta todos los pasos
  //   $("#paso-consentimiento").show("fast"); // Muestra el paso de consentimiento
  //   if (!self.pasoConsentimientoAbierto()) {
  //     self.pasoConsentimientoAbierto(true);
  //   }
  // };

  // self.pasoListaChequeo = function () {
  //   self.ocultarPasos(); // Oculta todos los pasos
  //   $("#paso-chequeo").show("fast"); // Muestra el paso de cuestionario
  //   if (!self.pasoListaChequeoAbierto()) {
  //     self.pasoListaChequeoAbierto(true);
  //   }
  // };

  // self.pasoResumen = function () {
  //   self.ocultarPasos();
  //   $("#paso-resumen").show("fast");
  //   if (!self.pasoResumenAbierto()) {
  //     self.pasoResumenAbierto(true);
  //   }
  // };

  //#endregion

  self.consultarPacientes = function () {
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarPacientes",
      method: "POST",
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.listaUsuarios(data.Resultado);

          //Preseleccionar paciente proveniente de consulta
          let pacienteId = $("#PacienteId").val();
          if (
            pacienteId != null &&
            pacienteId != undefined &&
            pacienteId.trim() != ""
          ) {
            $(self.listaUsuarios()).each(function (idx, usuario) {
              if (usuario.Id == $("#PacienteId").val()) {
                self.pacienteSeleccionado(usuario);
              }
            });
            self.validarExistenciaPaciente();
          }
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("Error al consultar los usuarios.");
      },
    });
  };

  self.consultarTarifas = function () {
    var ctx = obtenerContextoHospitalizar();
    if (ctx.habitacionId <= 0 && ctx.categoriaId <= 0) {
      console.error("Hospitalizar: habitacionId y categoriaId inválidos", ctx);
      alert("No se pudo identificar la habitación o categoría. Recargue la página.");
      return;
    }

    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarTarifasHabitacion",
      method: "POST",
      data: {
        habitacionId: ctx.habitacionId,
        categoriaId: ctx.categoriaId,
        codigoSeguro: ctx.codigoSeguro,
      },
      success: function (dataResult) {
        hideLoading();
        var data = parseAjaxJson(dataResult);
        if (!data) {
          alert("Error al leer las tarifas del servidor. Recargue la página e intente de nuevo.");
          return;
        }

        var exitoso = data.Exitoso === true || data.exitoso === true;
        var resultado = data.Resultado || data.resultado || [];

        if (exitoso) {
          self.tarifas(resultado);
          $("#texto-validacion-tarifas").hide();
          $("#mensaje-sin-tarifas").toggle(resultado.length === 0);

          if (resultado.length > 0) {
            var seleccionActual = self.tarifaSeleccionada();
            var mismaTarifa = seleccionActual
              ? ko.utils.arrayFirst(resultado, function (t) {
                  return t.TarifaId === seleccionActual.TarifaId;
                })
              : null;
            self.tarifaSeleccionada(mismaTarifa || resultado[0]);
          } else {
            self.tarifaSeleccionada(null);
            $("#texto-validacion-tarifas").show();
          }
        } else {
          alert(data.Mensaje || data.mensaje || "Error al consultar tarifas.");
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("Error al consultar las tarifas.");
      },
    });
  };

  self.modificarCategoria = function () {
    window.open(
      "/Habitaciones/Modificarcategoria?categoriaId=" +
      $("#HabitacionCategoriaId").val(),
      "_blank",
    );
  };

  self.validarExistenciaPaciente = function () {
    let pacienteSeleccionado = self.pacienteSeleccionado();

    if (pacienteSeleccionado != undefined && pacienteSeleccionado != null) {
      if (
        pacienteSeleccionado.Id != null &&
        pacienteSeleccionado.Id != undefined
      ) {
        //Si el paciente existe
        self.pacienteId(pacienteSeleccionado.Id);
        $("#PacienteId").val(pacienteSeleccionado.Id);
        self.pacienteNombre(pacienteSeleccionado.Nombre);
        self.pacienteDpi(pacienteSeleccionado.Dpi);
        self.pacienteTelefono(pacienteSeleccionado.Telefono);
        self.pacienteFechaNacimiento(pacienteSeleccionado.FechaNacimiento);
        self.pacienteCorreo(pacienteSeleccionado.Correo || "");
      } else {
        if (
          pacienteSeleccionado != null &&
          pacienteSeleccionado != undefined &&
          pacienteSeleccionado.trim() != ""
        ) {
          //Si el paciente NO existe y viene contenido en el nombre
          self.pacienteId(null);
          self.pacienteNombre(pacienteSeleccionado);
          self.pacienteDpi(null);
          self.pacienteTelefono(null);
          self.pacienteFechaNacimiento(null);
          self.pacienteCorreo("");
        }
      }
    } else {
      //Si el valor de paciente seleccionado es nulo
      self.pacienteId(null);
      self.pacienteNombre(null);
      self.pacienteDpi(null);
      self.pacienteTelefono(null);
      self.pacienteFechaNacimiento(null);
      self.pacienteCorreo("");
    }
  };

  self.validateModel = function () {
    let pacienteIdVal = parseInt(self.pacienteId(), 10);
    if (!pacienteIdVal || isNaN(pacienteIdVal) || pacienteIdVal <= 0) {
      pacienteIdVal = parseInt($("#PacienteId").val(), 10);
    }
    if (!pacienteIdVal || isNaN(pacienteIdVal) || pacienteIdVal <= 0) {
      alert("Seleccione un paciente válido de la lista (paso 3).");
      return false;
    }
    self.pacienteId(pacienteIdVal);
    $("#PacienteId").val(pacienteIdVal);

    let pacienteNombre = self.pacienteNombre();
    if (
      pacienteNombre == null ||
      pacienteNombre == undefined ||
      pacienteNombre.trim() == ""
    ) {
      alert("Seleccione un paciente");
      return false;
    }

    let tarifa = self.tarifaSeleccionada();
    if (tarifa == undefined || tarifa == null) {
      alert("Seleccione una tarifa");
      return false;
    }

    // Validar firma del paciente
    if (!self.urlFirmaPaciente() && !self.firmaPacienteBase64()) {
      alert("La firma del paciente es obligatoria. Regrese al paso 8 y confirme la firma.");
      return false;
    }

    // Validar firma del responsable de cuenta
    if (!self.urlFirmaResponsable() && !self.firmaResponsableBase64()) {
      alert("La firma del responsable de cuenta es obligatoria. Regrese al paso 8 y confirme la firma.");
      return false;
    }

    return true;
  };

  self.subirArchivoConsentimientoFirmado = function () {
    var formData = new FormData();
    var fileInput = document.getElementById("archivo-consentimiento-firmado");
    var nuevoNombreArchivo = document.getElementById(
      "nuevo-nombre-archivo",
    ).value; // Obtén el nombre nuevo

    if (fileInput.files.length == 0) {
      alert("No hay ningún archivo cargado");
    } else if (!nuevoNombreArchivo) {
      alert("Debe ingresar un nombre para el archivo");
    } else {
      showLoading();
      var file = fileInput.files[0];
      var extension = file.name.split(".").pop(); // Obtiene la extensión original del archivo
      var nuevoNombreConExtension = `${nuevoNombreArchivo}.${extension}`; // Añade la extensión al nuevo nombre
      formData.append("file", file);
      formData.append("nuevoNombreArchivo", nuevoNombreConExtension); // Envía el nombre nuevo con la extensión

      $.ajax({
        url: "/Hospitalizacion/UploadConsentimientoFirmado",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
          hideLoading();
          let data = JSON.parse(response);
          if (data.Exitoso) {
            mensajeEmergente("Archivo cargado exitosamente");
            self.urlArchivoConsentimiento(data.Resultado.UrlArchivo); // Actualiza la URL del archivo cargado
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (jqXHR, textStatus, errorThrown) {
          hideLoading();
          alert("Error al subir el archivo: " + textStatus);
        },
      });
    }
  };

  self.getModel = function () {
    model = {
      HabitacionId: $("#HabitacionId").val(),
      ConsultaId: $("#ConsultaId").val(),
      EmergenciaId: $("#EmergenciaId").val(),
      CitaId: $("#CitaId").val(),

      //Consentimiento
      UrlArchivoConsentimiento: self.urlArchivoConsentimiento(),
      Base64FirmaConsentimiento: $("#Base64FirmaConsentimiento").val(),

      // Firmas de Conformidad (base64 y URL en servidor)
      FirmaPacienteBase64: self.firmaPacienteBase64(),
      FirmaResponsableBase64: self.firmaResponsableBase64(),
      UrlFirmaPaciente: self.urlFirmaPaciente(),
      UrlFirmaResponsable: self.urlFirmaResponsable(),

      //Tarifa
      TarifaId: self.tarifaSeleccionada().TarifaId,
      TarifaValor: self.tarifaSeleccionada().ValorTarifa,

      //Estadia
      Periodo: self.periodo(),

      //Paciente
      PacienteId: self.pacienteId(),
      PacienteNombre: self.pacienteNombre(),
      PacienteDpi: self.pacienteDpi(),
      PacienteTelefono: self.pacienteTelefono(),
      PacienteFechaNacimiento: self.pacienteFechaNacimiento(),

      //Especialidad
      EspecialidadId: self.especialidadId(),
      EspecialidadNombre: self.especialidadNombre(),

      //Observaciones
      Observaciones: self.observaciones(),
    };
  };
  self.consultarEspecialidades = function () {
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarEspecialidades",
      method: "POST",
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.especialidades(data.Resultado);

          // Precargar Especialidad proveniente de consulta
          let especialidadIdRaw = $("#EspecialidadId").val();
          let especialidadId = especialidadIdRaw
            ? parseInt(especialidadIdRaw, 10)
            : 0;

          // Regla: si viene null/undefined/0 => Medicina General (Id=1)
          if (!especialidadId || isNaN(especialidadId)) {
            especialidadId = 1;
          }

          // Intentar seleccionar la especialidad indicada (o MG por defecto)
          let encontrada = false;
          $(self.especialidades()).each(function (idx, especialidad) {
            if (especialidad && especialidad.Id == especialidadId) {
              self.especialidadSeleccionada(especialidad);
              self.especialidadId(especialidad.Id);
              self.especialidadNombre(especialidad.NombreEspecialidad);
              encontrada = true;
              return false; // break
            }
          });

          // Si no se encontró match y solo hay una especialidad, se toma esa
          if (!encontrada && data.Resultado.length === 1) {
            let especialidad = data.Resultado[0];
            self.especialidadSeleccionada(especialidad);
            self.especialidadId(especialidad.Id);
            self.especialidadNombre(especialidad.NombreEspecialidad);
            encontrada = true;
          }

          // Si aún no se encontró (por ejemplo, MG no viene en el catálogo), forzar Id=1
          // Manteniendo el requerimiento: default Medicina General (Id=1)
          if (!encontrada) {
            self.especialidadSeleccionada(null);
            self.especialidadId(1);
            self.especialidadNombre("Medicina General");
          }
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("Error al consultar las especialidades.");
      },
    });
  };

  self.addHospitIdConsentimiento = function (idHospi) {
    self.getModel();
    let pacienteId = parseInt(model.PacienteId);
    let habitacionId = parseInt(model.HabitacionId);
    let idHospitalizacion = idHospi.toString();
    const modelDt = {
      PacienteId: pacienteId,
      HabitacionId: habitacionId,
      HospitalizacionId: idHospitalizacion,
    };
    // console.log(pacienteId, " +++ ", habitacionId, " +++ ", idHospitalizacion, " +++ ", modelDt);
    $.ajax({
      method: "POST",
      url: "/ConsentimientoHospi/ActualizarHospitalizacionId",
      data: modelDt,
      success: function (dataResult) {
        let data = JSON.parse(dataResult);
        if (data.exitoso) {
          console.log(data.mensaje);
        } else {
          console.log(data.mensaje);
        }
      },
      error: function (dataError) {
        console.log(dataError);
      },
    });
  };
  // self.registrarHospitalizacion = function () {
  //   if (self.validateModel()) {
  //     if (confirm("¿Desea registrar la hospitalización?")) {
  //       showLoading();
  //       self.getModel();
  //       $.ajax({
  //         url: "/Hospitalizacion/Hospitalizar",
  //         method: "POST",
  //         data: model,
  //         success: function (dataResult) {
  //           //let data = JSON.parse(dataResult);
  //           let data = (typeof dataResult === "string") ? JSON.parse(dataResult) : dataResult;
  //           if (data.Exitoso) {
  //             self.addHospitIdConsentimiento(data.HospitalizacionId);

  //             let hospitalizacionId = data.HospitalizacionId;
  //             let pacienteId = self.pacienteId();
  //             let habitacionId = parseInt($("#HabitacionId").val()) || 0;
  //             let citaId = $("#CitaId").val();
  //             let citaIdNum = (citaId && citaId !== "0") ? citaId : "0";
  //             let nombrePaciente = self.pacienteNombre();

  //             self.guardarCuestionario(hospitalizacionId, function () {
  //               self.guardarListaChequeo(hospitalizacionId);
  //             });

  //             // ─── ENVÍO DE CORREOS AL EQUIPO ───────────────────────────
  //             // Recopilamos todos los destinatarios que tengan correo
  //             var destinatarios = [
  //               { correo: data.EmailPaciente, nombre: "Paciente" },
  //               { correo: data.EmailCirujano, nombre: "Cirujano" },
  //               { correo: data.EmailPrimerAyudante, nombre: "Primer Ayudante" },
  //               { correo: data.EmailSegundoAyudante, nombre: "Segundo Ayudante" },
  //               { correo: data.EmailAnestesista, nombre: "Anestesista" },
  //               { correo: data.EmailInstrumentista, nombre: "Instrumentista" },
  //               { correo: data.EmailCirculante, nombre: "Circulante" }
  //             ].filter(function (d) { return d.correo && d.correo.trim() !== ""; });

  //             // Enviamos el correo a cada destinatario encontrado
  //             destinatarios.forEach(function (dest) {
  //               enviarHospitalizacionEmail(
  //                 hospitalizacionId,
  //                 nombrePaciente,
  //                 dest.correo,
  //                 pacienteId,
  //                 habitacionId,
  //                 citaIdNum
  //               );
  //             });

  //             mensajeEmergente("Hospitalización registrada con éxito.");
  //             // ─────────────────────────────────────────────────────────

  //             setTimeout(() => {
  //               var url = "/Hospitalizacion/Detalles?hospitalizacionId=" + hospitalizacionId;
  //               if (citaId && citaId !== "0") {
  //                 url += "&citaId=" + citaId;
  //               }
  //               window.location.href = url;
  //               return;
  //             }, 10000);
  //           } else {
  //             mensajeEmergenteError(data.Mensaje);
  //             hideLoading();
  //           }
  //         },
  //         error: function (dataError) {
  //           hideLoading();
  //           console.log(dataError);
  //           alert(dataError);
  //         },
  //       });
  //     }
  //   }
  // };

  self.registrarHospitalizacion = function () {
    if (self.validateModel()) {
      if (confirm("¿Desea registrar la hospitalización?")) {
        showLoading();
        self.getModel();

        $.ajax({
          url: "/Hospitalizacion/Hospitalizar",
          method: "POST",
          data: model,
          success: function (dataResult) {
            let data = (typeof dataResult === "string") ? JSON.parse(dataResult) : dataResult;

            if (data.Exitoso) {
              self.addHospitIdConsentimiento(data.HospitalizacionId);


              if (data.PacienteId && data.PacienteId > 0) {
                self.pacienteId(data.PacienteId);
                console.log("PacienteId actualizado a:", self.pacienteId());
              } else {
                console.warn("No se recibió PacienteId del servidor");
              }

              let hospitalizacionId = data.HospitalizacionId;
              let pacienteId = self.pacienteId();
              let habitacionId = parseInt($("#HabitacionId").val()) || 0;
              let citaId = $("#CitaId").val();
              let citaIdNum = (citaId && citaId !== "0") ? citaId : "0";
              let nombrePaciente = self.pacienteNombre();

              // 1. Filtrar destinatarios de correo
              var destinatarios = [
                { correo: data.EmailPaciente },
                { correo: data.EmailCirujano },
                { correo: data.EmailPrimerAyudante },
                { correo: data.EmailSegundoAyudante },
                { correo: data.EmailAnestesista },
                { correo: data.EmailInstrumentista },
                { correo: data.EmailCirculante }
              ].filter(function (d) { return d.correo && d.correo.trim() !== ""; });

              // 2. Promesas de correos
              var promesasEnvio = destinatarios.map(function (dest) {
                return enviarHospitalizacionEmail(
                  hospitalizacionId,
                  nombrePaciente,
                  dest.correo,
                  pacienteId,
                  habitacionId,
                  citaIdNum
                );
              });

              var promesaCuestionario = new Promise(function (resolve) {
                self.guardarCuestionario(hospitalizacionId, function () {
                  self.guardarListaChequeo(hospitalizacionId);
                  resolve();
                });
              });

              var todasLasPromesas = promesasEnvio.concat([promesaCuestionario]);
              Promise.all(todasLasPromesas).finally(function () {
                finalizarRegistro(hospitalizacionId, citaId);
              });

            } else {
              mensajeEmergenteError(data.Mensaje);
              hideLoading();
            }
          },
          error: function (dataError) {
            hideLoading();
            console.error(dataError);
            alert("Error en el servidor al procesar la hospitalización.");
          },
        });
      }
    }

    // Función auxiliar para manejar el éxito y la redirección
    function finalizarRegistro(hId, cId) {
      mensajeEmergente("Hospitalización registrada con éxito.");

      // Delay de 1.5s para que el usuario lea el mensaje de éxito
      setTimeout(function () {
        var url = "/Hospitalizacion/Detalles?hospitalizacionId=" + hId;
        if (cId && cId !== "0") {
          url += "&citaId=" + cId;
        }
        window.location.href = url;
      }, 1500);
    }
  };

  // --- FUNCIONES PARA AVANZAR (CON VALIDACIÓN) ---
  self.pasoEspecialidad = function () {
    irAPaso(1, "#paso-especialidad");
    $("#texto-validacion-especialidad").hide();
  };

  self.pasoTarifas = function () {
    // Validar especialidad
    if (!self.especialidadId() || self.especialidadId() == 0) {
      $("#texto-validacion-especialidad").show();
      return false;
    }
    $("#texto-validacion-especialidad").hide();

    // Consultar tarifas antes de avanzar
    self.consultarTarifas();
    irAPaso(2, "#paso-tarifas");
  };

  self.pasoPaciente = function () {
    // Validar tarifa
    if (!self.tarifaSeleccionada()) {
      $("#texto-validacion-tarifas").show();
      return false;
    }
    $("#texto-validacion-tarifas").hide();

    // Establecer datos de tarifa seleccionada
    let tarifa = self.tarifaSeleccionada();
    self.tarifaNombre(tarifa.NombreTarifa);
    self.tarifaValor(tarifa.ValorTarifa);

    // Consultar pacientes si no se ha hecho antes
    if (!self.pasoPacienteAbierto()) {
      self.consultarPacientes();
      self.pasoPacienteAbierto(true);
    }

    irAPaso(3, "#paso-paciente");
  };

  self.pasoEstadia = function () {
    // Validar paciente
    if (!self.pacienteId() || self.pacienteId() == "0") {
      $("#texto-validacion-paciente").show();
      return false;
    }
    $("#texto-validacion-paciente").hide();

    if (!self.pasoEstadiaAbierto()) {
      self.pasoEstadiaAbierto(true);
    }

    irAPaso(4, "#paso-estadia");
  };

  self.pasoConsentimiento = function () {
    if (!self.pasoConsentimientoAbierto()) {
      self.pasoConsentimientoAbierto(true);
    }
    irAPaso(5, "#paso-consentimiento");
  };

  self.pasoCuestionario = function () {
    // PRECARGAR DATOS DEL PACIENTE EN EL CUESTIONARIO
    if (self.cuestionario) {
      console.log("Precargando datos en cuestionario");
      self.cuestionario.pacienteRegistro(self.pacienteDpi() || "");
      self.cuestionario.pacienteNombre(self.pacienteNombre() || "");
      self.cuestionario.pacienteEdad(self.edadCalculada() || "");

      // Precargar Procedimiento y Cirujano desde la cita agendada
      var citaId = $("#CitaId").val();
      if (citaId && citaId !== "0" && citaId !== "") {
        $.ajax({
          url: "/Cita/GetDatosCita",
          type: "GET",
          data: { citaId: citaId },
          success: function (data) {
            if (data.exitoso) {
              self.cuestionario.procedimientoProgramado(
                data.procedimiento || "",
              );
              self.cuestionario.cirujano(data.cirujano || "");
            }
          },
          error: function () {
            console.error(
              "Error al obtener datos de la cita para precargar cuestionario",
            );
          },
        });
      }
    }

    if (!self.pasoCuestionarioAbierto()) {
      self.pasoCuestionarioAbierto(true);
    }
    irAPaso(6, "#paso-cuestionario");
  };

  self.pasoListaChequeo = function () {
    // Precargar datos del paciente en la lista de chequeo
    if (self.listaChequeo) {
      console.log("Precargando datos en lista de chequeo");

      // Dividir nombre completo en nombre y apellido
      var nombreCompleto = self.pacienteNombre() || "";
      var partes = nombreCompleto.split(" ");
      var primerNombre = partes[0] || "";
      var apellido = partes.slice(1).join(" ") || "";

      self.listaChequeo.ci_nombreConfirma(primerNombre);
      self.listaChequeo.ci_apellidoConfirma(apellido);
      self.listaChequeo.cp_nombrePacienteCirujano(primerNombre);
      self.listaChequeo.ci_fechaNacConfirma(
        self.pacienteFechaNacimiento() || "",
      );

      // ===== PRECARGAR PROCEDIMIENTO =====
      var citaId = $("#CitaId").val();
      if (citaId && citaId !== "0" && citaId !== "") {
        if (self.cuestionario && self.cuestionario.procedimientoProgramado()) {
          var proc = self.cuestionario.procedimientoProgramado();
          self.listaChequeo.ci_operacion(proc);
          self.listaChequeo.cp_nombreCirugia(proc);
          console.log("Procedimiento precargado desde cuestionario:", proc);
        } else {
          $.ajax({
            url: "/Cita/GetDatosCita",
            type: "GET",
            data: { citaId: citaId },
            success: function (data) {
              if (data.exitoso && data.procedimiento) {
                self.listaChequeo.ci_operacion(data.procedimiento);
                self.listaChequeo.cp_nombreCirugia(data.procedimiento);
                console.log("Procedimiento precargado desde cita:", data.procedimiento);
              }
            },
            error: function () {
              console.error("Error al obtener datos de la cita para lista de chequeo");
            }
          });
        }
      }
      // ===== FIN PRECARGA =====

      // Precargar el check de consentimiento si ya existe
      if (
        self.urlArchivoConsentimiento() &&
        self.urlArchivoConsentimiento() !== ""
      ) {
        self.listaChequeo.ci_consentimiento("SI");
      }
    }

    if (!self.pasoListaChequeoAbierto()) {
      self.pasoListaChequeoAbierto(true);
    }
    irAPaso(7, "#paso-chequeo");
  };

  // --- PASO 8: FIRMAS ---
  self.pasoFirmas = function () {
    if (!self.pasoFirmasAbierto()) {
      self.pasoFirmasAbierto(true);
    }
    irAPaso(8, "#paso-firmas");

    // SOLUCIÓN CANVAS: Darle 150ms para que el div sea visible antes de configurar el canvas
    // Así evitamos que el lápiz dibuje fuera de lugar.
    setTimeout(function () {
      if (typeof inicializarCanvases === "function") {
        inicializarCanvases();
      }
    }, 150);
  };

  // --- PASO 9: RESUMEN ---
  self.pasoResumen = function () {
    if (!self.urlFirmaPaciente()) {
      let panelCanvasP = document.getElementById("panel-canvas-paciente");
      if (panelCanvasP && !panelCanvasP.classList.contains("d-none")) {
        let canvasP = document.getElementById("canvasFirmaPaciente");
        if (canvasP) self.firmaPacienteBase64(canvasP.toDataURL("image/png"));
      }
    }

    if (!self.urlFirmaResponsable()) {
      let panelCanvasR = document.getElementById("panel-canvas-responsable");
      if (panelCanvasR && !panelCanvasR.classList.contains("d-none")) {
        let canvasR = document.getElementById("canvasFirmaResponsable");
        if (canvasR) self.firmaResponsableBase64(canvasR.toDataURL("image/png"));
      }
    }

    if (!self.pasoResumenAbierto()) {
      self.pasoResumenAbierto(true);
    }
    irAPaso(9, "#paso-resumen");

    if (typeof window.consultarExistenciaConsentimiento === "function") {
      window.consultarExistenciaConsentimiento();
    } else if (typeof window.actualizarEstadoBtnFinalizar === "function") {
      var tieneConsentimiento = $("#botonImprimir2").is(":visible");
      window.actualizarEstadoBtnFinalizar(tieneConsentimiento);
    }
  };

  /// --- FUNCIONES PARA VOLVER ---
  self.volverEspecialidad = function () {
    irAPaso(1, "#paso-especialidad");
  };
  self.volverTarifas = function () {
    irAPaso(2, "#paso-tarifas");
  };
  self.volverPaciente = function () {
    irAPaso(3, "#paso-paciente");
  };
  self.volverEstadia = function () {
    irAPaso(4, "#paso-estadia");
  };
  self.volverConsentimiento = function () {
    irAPaso(5, "#paso-consentimiento");
  };
  self.volverCuestionario = function () {
    irAPaso(6, "#paso-cuestionario");
  };
  self.volverChequeo = function () {
    irAPaso(7, "#paso-chequeo");
  };

  // NUEVA: Para el botón "Anterior" en el Resumen (Paso 9)
  self.volverFirmas = function () {
    irAPaso(8, "#paso-firmas");
  };

  self.guardarCuestionario = function (hospitalizacionId, callback) {
    // Obtener la edad completa del paciente
    var edadCompleta = self.edadCalculada() || "";

    // Extraer SOLO los años para el cuestionario
    var soloAnios = "";
    if (edadCompleta) {
      var match = edadCompleta.match(/(\d+)\s*Año/);
      if (match) {
        soloAnios = match[1]; // Solo el número
      } else {
        // Si no encuentra el patrón, tomar el primer número
        var numeros = edadCompleta.match(/\d+/);
        soloAnios = numeros ? numeros[0] : "";
      }
    }

    let cuestionarioData = {
      HospitalizacionId: hospitalizacionId,

      // Datos del paciente
      NombreCompleto: self.pacienteNombre() || "",
      RegistroMedico: self.pacienteDpi() || "",
      Edad: soloAnios, // SOLO EL NÚMERO (ej: "25")
      FechaCuestionario: self.cuestionario.fechaCuestionario()
        ? new Date(self.cuestionario.fechaCuestionario())
        : null,
      Peso: self.cuestionario.peso()
        ? parseFloat(self.cuestionario.peso())
        : null,
      Estatura: self.cuestionario.estatura()
        ? parseFloat(self.cuestionario.estatura())
        : null,
      FechaUltimaRegla: self.cuestionario.fechaUltimaRegla()
        ? new Date(self.cuestionario.fechaUltimaRegla())
        : null,
      FechaProcedimiento: self.cuestionario.fechaProcedimiento()
        ? new Date(self.cuestionario.fechaProcedimiento())
        : null,
      ProcedimientoProgramado:
        self.cuestionario.procedimientoProgramado() || "",
      Cirujano: self.cuestionario.cirujano() || "",

      // Antecedentes - Columna Izquierda (SIN TRUNCAR)
      PA_Alergia: self.cuestionario.pa_alergia() || "",
      PA_AlergiaCual: self.cuestionario.pa_alergiaCual() || "",
      PA_Fuma: self.cuestionario.pa_fuma() || "",
      PA_FumaCuanto: self.cuestionario.pa_fumaCuanto() || "",
      PA_Drogas: self.cuestionario.pa_drogas() || "",
      PA_DrogasCuales: self.cuestionario.pa_drogasCuales() || "",
      PA_Alcohol: self.cuestionario.pa_alcohol() || "",
      PA_AlcoholCuanto: self.cuestionario.pa_alcoholCuanto() || "",
      PA_Embarazo: self.cuestionario.pa_embarazo() || "",
      PA_Transfusion: self.cuestionario.pa_transfusion() || "",
      PA_Asma: self.cuestionario.pa_asma() || "",
      PA_Pulmones: self.cuestionario.pa_pulmones() || "",
      PA_Corazon: self.cuestionario.pa_corazon() || "",

      // Antecedentes - Columna Derecha
      PA_AtaqueCardiaco: self.cuestionario.pa_ataqueCardiaco() || "",
      PA_Angina: self.cuestionario.pa_angina() || "",
      PA_Soplo: self.cuestionario.pa_soplo() || "",
      PA_Presion: self.cuestionario.pa_presion() || "",
      PA_Higado: self.cuestionario.pa_higado() || "",
      PA_Rinones: self.cuestionario.pa_rinones() || "",
      PA_Diabetes: self.cuestionario.pa_diabetes() || "",
      PA_Epilepsia: self.cuestionario.pa_epilepsia() || "",
      PA_Derrame: self.cuestionario.pa_derrame() || "",
      PA_Tiroides: self.cuestionario.pa_tiroides() || "",
      PA_Anestesico: self.cuestionario.pa_anestesico() || "",
      PA_AceptaTransfusion: self.cuestionario.pa_aceptaTransfusion() || "",

      // Información adicional
      AI_Medicamentos: self.cuestionario.ai_medicamentos() || "",
      AI_Actividad: self.cuestionario.ai_actividad() || "",
      AI_ActividadDetalle: self.cuestionario.ai_actividadDetalle() || "",
      AI_OperacionesPrevias: self.cuestionario.ai_operacionesPrevias() || "",
      AI_Comentarios: self.cuestionario.ai_comentarios() || "",
    };

    console.log("Edad original:", edadCompleta);
    console.log("Edad para cuestionario:", soloAnios);
    console.log("Guardando cuestionario:", cuestionarioData);

    console.log(
      "Fecha procedimiento desde binding:",
      self.cuestionario.fechaProcedimiento(),
    );
    console.log(
      "Fecha procedimiento convertida:",
      self.cuestionario.fechaProcedimiento()
        ? new Date(self.cuestionario.fechaProcedimiento())
        : null,
    );

    $.ajax({
      url: "/CuestionarioPreAnestesico/AgregarCuestionario",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(cuestionarioData),
      processData: false,
      success: function (response) {
        try {
          let data =
            typeof response === "string" ? JSON.parse(response) : response;
          if (data.exitoso) {
            console.log("Cuestionario guardado exitosamente");


            var pesoRaw = self.cuestionario.peso();
            var pesoValor = null;

            if (pesoRaw !== null && pesoRaw !== undefined && pesoRaw.toString().trim() !== "") {
              var pesoStr = pesoRaw.toString().trim().replace(',', '.'); 
              pesoStr = pesoStr.replace(/[^0-9.]/g, ''); // elimina letras y símbolos, deja solo números y punto

              var partes = pesoStr.split('.');
              if (partes.length > 2) pesoStr = partes[0] + '.' + partes.slice(1).join('');

              pesoValor = parseFloat(pesoStr);
              if (isNaN(pesoValor)) pesoValor = null;
            }

            // ===== ACTUALIZAR PESO Y ESTATURA EN EL PACIENTE =====
            var pacienteId = self.pacienteId();
            if (!pacienteId || pacienteId === 0) {
              console.error("No se puede actualizar peso/estatura: pacienteId inválido");
              return;
            }
            if (pacienteId && pacienteId > 0) {
              var estaturaRaw = self.cuestionario.estatura();
              var estaturaValor = null;
              if (estaturaRaw !== null && estaturaRaw !== undefined && estaturaRaw.toString().trim() !== "") {
                var estaturaStr = estaturaRaw.toString().trim().replace(',', '.');
                estaturaStr = estaturaStr.replace(/[^0-9.]/g, '');
                var partesEst = estaturaStr.split('.');
                if (partesEst.length > 2) estaturaStr = partesEst[0] + '.' + partesEst.slice(1).join('');
                estaturaValor = parseFloat(estaturaStr);
                if (isNaN(estaturaValor)) estaturaValor = null;
              }

              var promesasPesoEstatura = [];

              if (pacienteId && pesoValor !== null && !isNaN(pesoValor)) {
                promesasPesoEstatura.push($.ajax({
                  url: '/Pacientes/ActualizarCampo',
                  type: 'POST',
                  data: { pacienteId: pacienteId, campo: 'Peso', valor: pesoValor.toString() },
                  success: function (resp) {
                    if (resp.success) console.log("Peso actualizado:", pesoValor);
                    else console.warn("Error al actualizar peso:", resp);
                  },
                  error: function (xhr) { console.error("Error HTTP al actualizar peso:", xhr.status); }
                }));
              } else {
                console.warn("Valor de peso inválido o vacío:", pesoRaw);
              }

              if (estaturaValor && !isNaN(estaturaValor)) {
                promesasPesoEstatura.push($.ajax({
                  url: '/Pacientes/ActualizarCampo',
                  type: 'POST',
                  data: { pacienteId: pacienteId, campo: 'Estatura', valor: estaturaValor.toString() },
                  success: function (resp) {
                    if (resp.success) console.log("Estatura actualizada");
                    else console.warn("Error al actualizar estatura:", resp);
                  },
                  error: function () { console.error("Error de red al actualizar estatura"); }
                }));
              }

              Promise.all(promesasPesoEstatura).finally(function () {
                if (typeof callback === "function") callback();
              });
              return;

            } else {
              console.warn("No se puede actualizar peso/estatura: pacienteId inválido", pacienteId);
            }
            // ===== FIN ACTUALIZACIÓN =====

          } else {
            console.error("Error al guardar cuestionario:", data.resultado);
          }
        } catch (e) {
          console.error("Error parsing response:", e);
        }
        if (typeof callback === "function") callback();
      },
      error: function (xhr, status, error) {
        console.error("Error en la petición AJAX:", error);
        console.error("Response:", xhr.responseText);
        if (typeof callback === "function") callback();
      },
    });
  };

  self.guardarListaChequeo = function (hospitalizacionId) {
    // Obtener nombre y apellido del paciente
    let nombreCompleto = self.pacienteNombre() || "";
    let nombreParts = nombreCompleto.split(" ");
    let primerNombre = nombreParts[0] || "";
    let apellido = nombreParts.length > 1 ? nombreParts.slice(1).join(" ") : "";

    let listaData = {
      HospitalizacionId: hospitalizacionId,

      // Encabezado
      NombrePaciente: primerNombre,
      ApellidoPaciente: apellido,
      MedicoTratante: self.cuestionario.cirujano() || "",
      FechaNacimiento: self.pacienteFechaNacimiento()
        ? new Date(self.pacienteFechaNacimiento())
        : null,
      FechaChequeo: self.listaChequeo.fechaChequeo()
        ? new Date(self.listaChequeo.fechaChequeo())
        : null,
      HoraChequeo: self.listaChequeo.horaChequeo() || "",

      // ENTRADA - Paciente
      CI_NombreConfirma: self.listaChequeo.ci_nombreConfirma() || "",
      CI_ApellidoConfirma: self.listaChequeo.ci_apellidoConfirma() || "",
      CI_FechaNacConfirma: self.listaChequeo.ci_fechaNacConfirma()
        ? new Date(self.listaChequeo.ci_fechaNacConfirma())
        : null,
      CI_Consentimiento: self.listaChequeo.ci_consentimiento() || "",
      CI_Operacion: self.listaChequeo.ci_operacion() || "",
      CI_LadoOperar: self.listaChequeo.ci_ladoOperar() || "",
      CI_SitioMarcado: self.listaChequeo.ci_sitioMarcado() || "",
      CI_Alergia: self.listaChequeo.ci_alergia() || "",

      // ENTRADA - Anestesiólogo
      CI_EvalPreanestesica: self.listaChequeo.ci_evalPreanestesica() || "",
      CI_AccesoIV: self.listaChequeo.ci_accesoIV() || "",
      CI_EquipoAnestesia: self.listaChequeo.ci_equipoAnestesia() || "",
      CI_Medicamentos: self.listaChequeo.ci_medicamentos() || "",
      CI_Oximetro: self.listaChequeo.ci_oximetro() || "",
      CI_EquipoAspiracion: self.listaChequeo.ci_equipoAspiracion() || "",
      CI_ViaAerea: self.listaChequeo.ci_viaAerea() || "",

      // PAUSA - Cirujano
      CP_Presentacion: self.listaChequeo.cp_presentacion() || "",
      CP_NombrePacienteCirujano:
        self.listaChequeo.cp_nombrePacienteCirujano() || "",
      CP_NombreCirugia: self.listaChequeo.cp_nombreCirugia() || "",
      CP_EventosCriticos: self.listaChequeo.cp_eventosCriticos() || "",
      CP_TiempoDuracion: self.listaChequeo.cp_tiempoDuracion() || "",
      CP_ImagenesDiagnosticas:
        self.listaChequeo.cp_imagenesDiagnosticas() || "",
      CP_PerdidaSangre: self.listaChequeo.cp_perdidaSangre() || "",

      // PAUSA - Instrumentista
      CP_Esterilidad: self.listaChequeo.cp_esterilidad() || "",
      CP_MaterialesAdicionales:
        self.listaChequeo.cp_materialesAdicionales() || "",

      // PAUSA - Anestesiólogo
      CP_EventosCriticosAnest:
        self.listaChequeo.cp_eventosCriticosAnest() || "",
      CP_ProfilaxisAntibiotica:
        self.listaChequeo.cp_profilaxisAntibiotica() || "",
      CP_Tromboprofilaxis: self.listaChequeo.cp_tromboprofilaxis() || "",
      CP_ManejoDolor: self.listaChequeo.cp_manejoDolor() || "",

      // SALIDA - Enfermera
      CS_NombreOperacion: self.listaChequeo.cs_nombreOperacion() || "",
      CS_NombreEnfermera: self.listaChequeo.cs_nombreEnfermera() || "",
      CS_RecuentoCompleto: self.listaChequeo.cs_recuentoCompleto() || "",
      CS_EtiquetadoMuestras: self.listaChequeo.cs_etiquetadoMuestras() || "",

      // SALIDA - Recuperación
      CS_RepasoPostOp: self.listaChequeo.cs_repasoPostOp() || "",
      CS_PorQue: self.listaChequeo.cs_porQue() || "",
      CS_Traslado: self.listaChequeo.cs_traslado() || "",
      CS_Complicaciones: self.listaChequeo.cs_complicaciones() || "",
      CS_ServicioNumCama: self.listaChequeo.cs_servicioNumCama() || "",
    };

    console.log("Guardando lista de chequeo:", listaData);

    $.ajax({
      url: "/ListaChequeo/AgregarListaChequeo",
      type: "POST",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(listaData),
      processData: false,
      success: function (response) {
        try {
          let data =
            typeof response === "string" ? JSON.parse(response) : response;
          if (data.exitoso) {
            console.log("Lista de chequeo guardada exitosamente");
          } else {
            console.error("Error al guardar lista de chequeo:", data.resultado);
          }
        } catch (e) {
          console.error("Error parsing response:", e);
        }
      },
      error: function (xhr, status, error) {
        console.error("Error en la petición AJAX:", error);
        console.error("Response:", xhr.responseText);
      },
    });
  };

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
          if (self.servicioAgregarSeleccionado()) {
            self.consultarPreciosServicio(self.servicioAgregarSeleccionado().ServicioId);
          }
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("Error al consultar servicios.");
      }
    });
  };

  self.consultarPreciosServicio = function (servicioId) {
    if (!servicioId) return;
    showLoading();
    $.ajax({
      url: "/Cita/ConsultarPreciosServicio",
      method: "POST",
      data: { servicioId: servicioId, fecha: new Date().toISOString() },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.preciosServicioSeleccionado(data.Resultado);
          if (data.Resultado.length === 1) {
            self.precioServicioAgregarSeleccionado(data.Resultado[0]);
          }
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("Error al consultar precios.");
      }
    });
  };

  self.agregarServicio = function () {
    if (!self.servicioAgregarSeleccionado()) {
      alert("Seleccione un servicio.");
      return;
    }
    if (!self.precioServicioAgregarSeleccionado()) {
      alert("Seleccione un precio para el servicio.");
      return;
    }
    var servicio = self.servicioAgregarSeleccionado();
    var precio = self.precioServicioAgregarSeleccionado();
    self.serviciosAgregados.push({
      Item: self.itemServicio++,
      ServicioId: servicio.ServicioId,
      ServicioNombre: servicio.ServicioNombre,
      Cantidad: 1,
      ServicioDuracionHoras: servicio.ServicioDuracionHoras,
      ServicioDuracionMinutos: servicio.ServicioDuracionMinutos,
      PrecioId: precio.PrecioId,
      PrecioNombre: precio.PrecioNombre,
      PrecioValor: ko.observable(precio.PrecioValor),
      PrecioValorCubiertoSeguro: ko.observable(0),
      PrecioValorCopago: ko.observable(precio.PrecioValor),
      Nuevo: true
    });
    self.actualizarTotales();
  };

  self.quitarServicio = function (servicio) {
    self.serviciosAgregados.remove(servicio);
    self.actualizarTotales();
  };

  // Suscripción para cargar precios al cambiar el servicio seleccionado
  self.servicioAgregarSeleccionado.subscribe(function (nuevoServicio) {
    if (nuevoServicio) {
      self.consultarPreciosServicio(nuevoServicio.ServicioId);
    } else {
      self.preciosServicioSeleccionado([]);
      self.precioServicioAgregarSeleccionado(null);
    }
  });


  self.actualizarTotales = function () {
    let totalDuracionHoras = 0;
    let totalDuracionMinutos = 0;

    $(self.serviciosAgregados()).each(function (idx, servicio) {
      let cantidad = isNaN(servicio.Cantidad) ? 1 : servicio.Cantidad;
      if (servicio.ServicioDuracionHoras) {
        totalDuracionHoras += parseInt(servicio.ServicioDuracionHoras) * cantidad;
      }
      if (servicio.ServicioDuracionMinutos) {
        totalDuracionMinutos += parseInt(servicio.ServicioDuracionMinutos) * cantidad;
      }
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
      } else if (servicio.PrecioValor) {
        precio = servicio.PrecioValor;
      }
      totalValor += parseFloat(precio) * cantidad;
    });
    self.totalValor("GTQ " + totalValor.toFixed(2));
  };

};

var hospitalizarVm = new HospitalizarVM();
ko.applyBindings(hospitalizarVm);

$(document).ready(function () {
  // Consultar las tarifas al cargar la página
  hospitalizarVm.consultarTarifas();
  hospitalizarVm.consultarPacientes();
  hospitalizarVm.consultarEspecialidades();

  $(".dpi-paciente-nuevo").hide();

  let fechaEntrada = moment(); // Hora actual
  let fechaSalida = moment().add(24, "hours"); // Agregar 24 horas

  $("#Periodo").daterangepicker({
    startDate: fechaEntrada,
    endDate: fechaSalida,
    timePicker: true,
    locale: {
      applyLabel: "Aplicar",
      cancelLabel: "Cancelar",
      format: "DD/MM/YYYY hh:mm A",
      daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
      monthNames: [
        "Enero",
        "Febrero",
        "Marzo",
        "Abril",
        "Mayo",
        "Junio",
        "Julio",
        "Agosto",
        "Setiembre",
        "Octubre",
        "Noviembre",
        "Diciembre",
      ],
    },
  });
});

$("#revisarRentas").click("change", function () {
  var name = $("#nombre-cliente-buscado option:selected").text();
  window.location.href = "/Recepcion/Historial?buscar=" + name + "&estado=2";
});

$("#numero-adultos").keyup(function () {
  //@* calcularMonto(); *@
  //@* setearTarifa(); *@
});

function validarRentas(data) {
  if (data.rentasPendientes) {
    $("#msgCuentas").css("display", "block");
  } else {
    $("#msgCuentas").css("display", "none");
  }
}

// comprobar si el cliente existe o no
$("#nombreCliente").on("change", function () {
  //ObtenerCliente();
});

function ObtenerCliente() {
  var name = $("#nombre-cliente-buscado option:selected").text();
  var nombre = { nombre: name };
  $.ajax({
    method: "POST",
    data: nombre,
    dataType: "json",
    url: "/Cliente/RetornarCliente",
    traditional: true,
    success: function (data, state) {
      if (data == null) {
        document.getElementById("label-nuevo-paciente").show();
        //document.getElementById("nuevo-cliente-t").style.display = "inline-flex";
      } else {
        document.getElementById("nuevo-badge").style.display = "none";
        //document.getElementById("nuevo-cliente-t").style.display = "none";
        validarRentas(data);
      }

      document.getElementById("tipoCliente").textContent = data.tipoCliente;
      document.getElementById("empresa").textContent = data.empresa;
      document.getElementById("placas").textContent = data.placas;
      document.getElementById("marcavehiculo").textContent = data.marcaVehiculo;
    },
    error: function (data) { },
  });
}

function parseAjaxJson(dataResult) {
  if (dataResult == null) return null;
  if (typeof dataResult === "object") return dataResult;
  try {
    return JSON.parse(dataResult);
  } catch (e) {
    console.error("Respuesta no JSON:", dataResult);
    return null;
  }
}

function obtenerContextoHospitalizar() {
  var ctxEl = document.getElementById("hospitalizar-context");
  var habitacionId = parseInt($("#HabitacionId").val(), 10);
  var categoriaId = parseInt($("#HabitacionCategoriaId").val(), 10);
  var codigoSeguro = ($("#CodigoSeguro").val() || "").trim();

  if (ctxEl) {
    if (!habitacionId || isNaN(habitacionId)) {
      habitacionId = parseInt(ctxEl.getAttribute("data-habitacion-id"), 10);
    }
    if (!categoriaId || isNaN(categoriaId)) {
      categoriaId = parseInt(ctxEl.getAttribute("data-categoria-id"), 10);
    }
    if (!codigoSeguro) {
      codigoSeguro = (ctxEl.getAttribute("data-codigo-seguro") || "").trim();
    }
  }

  return {
    habitacionId: habitacionId && !isNaN(habitacionId) ? habitacionId : 0,
    categoriaId: categoriaId && !isNaN(categoriaId) ? categoriaId : 0,
    codigoSeguro: codigoSeguro,
  };
}

function actualizarVisualizacionStepper(pasoActual) {
  $(".stepper-item").removeClass("active completed");

  $(".stepper-item").each(function (index) {
    var n = index + 1;
    if (n < pasoActual) {
      $(this).addClass("completed");
    } else if (n === pasoActual) {
      $(this).addClass("active");
    }
  });
}

function irAPaso(numero, idSelector) {
  $(".paso").hide();

  $(idSelector).show("fast");

  actualizarVisualizacionStepper(numero);
}