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

  self.listaUsuarios = ko.observableArray();

  //#region Variables PASO a PASO

  self.pasoEspecialidadAbierto = ko.observable(false);
  self.pasoTarifasAbierto = ko.observable(false);
  self.pasoPacienteAbierto = ko.observable(false);
  self.pasoEstadiaAbierto = ko.observable(false);
  self.pasoConsentimientoAbierto = ko.observable(false);
  self.pasoResumenAbierto = ko.observable(false);

  //#endregion

  //#region Funciones PASO a PASO

  self.ocultarPasos = function () {
    $(".paso").hide();
  };

  self.pasoEspecialidad = function () {
    $("#texto-validacion-especialidad").hide();
    self.ocultarPasos();
    $("#paso-especialidad").show("fast");
    if (!self.pasoEspecialidadAbierto()) {
      self.pasoEspecialidadAbierto(true);
    }
  };
  self.pasoTarifas = function () {
    // $("#texto-validacion-tarifas").hide();
    // let especialidad = self.especialidadSeleccionada();
    // console.log(especialidad);
    // if (especialidad == null || especialidad == undefined) {
    //     $("#texto-validacion-especialidad").show();
    //     return false;
    // } else {
    //     if (especialidad.Id == null || especialidad.Id == undefined) {
    //         self.especialidadId(null);
    //         self.especialidadNombre(especialidad);
    //     } else {
    //         self.especialidadId(especialidad.Id);
    //         self.especialidadNombre(especialidad.NombreEspecialidad);
    //     }
    // }

    self.ocultarPasos();
    $("#paso-tarifas").show("fast");
    if (!self.pasoTarifasAbierto()) {
      self.pasoTarifasAbierto(true);
    }
  };
  self.pasoPaciente = function () {
    $("#texto-validacion-tarifas").hide();
    let tarifa = self.tarifaSeleccionada();
    if (!tarifa) {
      // Si no hay una tarifa seleccionada
      $("#texto-validacion-tarifas").show();
      return false;
    }
    self.tarifaNombre(tarifa.NombreTarifa);
    self.tarifaValor(tarifa.ValorTarifa);

    self.ocultarPasos();
    $("#paso-paciente").show("fast");
    if (!self.pasoPacienteAbierto()) {
      self.consultarPacientes();
      self.pasoPacienteAbierto(true);
    }
  };

  self.tarifaSeleccionada.subscribe(function (nuevaTarifa) {
    if (!nuevaTarifa) {
      // Seleccionar la primera tarifa si está disponible
      if (self.tarifas().length > 0) {
        self.tarifaSeleccionada(self.tarifas()[0]);
      }
    }
  });

  self.pasoEstadia = function () {
    // let pacienteNombre = self.pacienteNombre();
    // if (pacienteNombre == null || pacienteNombre == undefined || pacienteNombre.trim() == '') {
    //     $("#texto-validacion-paciente").show();
    //     return false;
    // }

    self.ocultarPasos();
    $("#paso-estadia").show("fast");
    if (!self.pasoEstadiaAbierto()) {
      self.pasoEstadiaAbierto(true);
    }
  };
  self.pasoConsentimiento = function () {
    self.ocultarPasos(); // Oculta todos los pasos
    $("#paso-consentimiento").show("fast"); // Muestra el paso de consentimiento
    if (!self.pasoConsentimientoAbierto()) {
      self.pasoConsentimientoAbierto(true);
    }
  };

  self.pasoResumen = function () {
    self.ocultarPasos();
    $("#paso-resumen").show("fast");
    if (!self.pasoResumenAbierto()) {
      self.pasoResumenAbierto(true);
    }
  };

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
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarTarifasHabitacion",
      method: "POST",
      data: {
        habitacionId: $("#HabitacionId").val(),
        codigoSeguro: $("#CodigoSeguro").val(),
      },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.tarifas(data.Resultado);

          // Seleccionar automáticamente la primera tarifa válida
          if (data.Resultado.length > 0) {
            self.tarifaSeleccionada(data.Resultado[0]); // Seleccionar automáticamente la primera tarifa
          }
        } else {
          alert(data.Mensaje);
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
        self.pacienteNombre(pacienteSeleccionado.Nombre);
        self.pacienteDpi(pacienteSeleccionado.Dpi);
        self.pacienteTelefono(pacienteSeleccionado.Telefono);
        self.pacienteFechaNacimiento(pacienteSeleccionado.FechaNacimiento);
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
        }
      }
    } else {
      //Si el valor de paciente seleccionado es nulo
      self.pacienteId(null);
      self.pacienteNombre(null);
      self.pacienteDpi(null);
      self.pacienteTelefono(null);
      self.pacienteFechaNacimiento(null);
    }
  };

  self.validateModel = function () {
    // let especialidad = self.especialidadSeleccionada();
    // if (especialidad == null || especialidad == undefined) {
    //     alert("Seleccione una especialidad");
    //     return false;
    // }

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
            let data = JSON.parse(dataResult);
            if (data.Exitoso) {
              self.addHospitIdConsentimiento(data.HospitalizacionId);
              setTimeout(() => {
                // window.location.href = "/Hospitalizacion/Detalles?hospitalizacionId=" + data.HospitalizacionId;
                var citaId = $("#CitaId").val(); // requiere que exista el hidden CitaId en la vista
                var url =
                  "/Hospitalizacion/Detalles?hospitalizacionId=" +
                  data.HospitalizacionId;

                if (citaId && citaId !== "0") {
                  url += "&citaId=" + citaId;
                }

                window.location.href = url;
                return;
              }, 5000);
            } else {
              mensajeEmergenteError(data.Mensaje);
              hideLoading();
            }
          },
          error: function (dataError) {
            hideLoading();
            console.log(dataError);
            alert(dataError);
          },
        });
      }
    }
  };
};

var hospitalizarVm = new HospitalizarVM();
ko.applyBindings(hospitalizarVm);

$(document).ready(function () {
  // Consultar las tarifas al cargar la página
  hospitalizarVm.consultarTarifas();
  hospitalizarVm.consultarPacientes();
  hospitalizarVm.consultarEspecialidades();

  // Ejecutar el paso de consentimiento directamente si es necesario
  hospitalizarVm.pasoConsentimiento();

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
    error: function (data) {},
  });
}
