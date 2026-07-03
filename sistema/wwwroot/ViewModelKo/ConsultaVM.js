var ConsultaVM = function () {
  var itemServicioConsulta = 1;
  let itemServicioDiente1 = 1;
  let itemServicioDiente2 = 1;
  let itemServicioDiente3 = 1;
  let itemServicioDiente4 = 1;
  let itemServicioDiente5 = 1;
  let itemServicioDiente6 = 1;
  let itemServicioDiente7 = 1;
  let itemServicioDiente8 = 1;
  let itemServicioDiente9 = 1;
  let itemServicioDiente10 = 1;
  let itemServicioDiente11 = 1;
  let itemServicioDiente12 = 1;
  let itemServicioDiente13 = 1;
  let itemServicioDiente14 = 1;
  let itemServicioDiente15 = 1;
  let itemServicioDiente16 = 1;
  let itemServicioDiente17 = 1;
  let itemServicioDiente18 = 1;
  let itemServicioDiente19 = 1;
  let itemServicioDiente20 = 1;
  let itemServicioDiente21 = 1;
  let itemServicioDiente22 = 1;
  let itemServicioDiente23 = 1;
  let itemServicioDiente24 = 1;
  let itemServicioDiente25 = 1;
  let itemServicioDiente26 = 1;
  let itemServicioDiente27 = 1;
  let itemServicioDiente28 = 1;
  let itemServicioDiente29 = 1;
  let itemServicioDiente30 = 1;
  let itemServicioDiente31 = 1;
  let itemServicioDiente32 = 1;
  var itemExamenLaboratorio = 1;
  var itemRangoSaludable = 1;
  var itemPrescripcion = 1;
  var model = {};

  var self = this;

  // Offset fijo (Venezuela) SOLO para este módulo
  self._fixedOffsetHours = 0;
  self._fixedOffsetMs = self._fixedOffsetHours * 60 * 60 * 1000;

  self._applyFixedOffset = function (d) {
    if (!d) return null;
    return new Date(d.getTime() + self._fixedOffsetMs);
  };

  // Para recalcular "hace..." sin refrescar página (opcional, pero recomendado)
  self._ahoraTick = ko.observable(new Date());
  setInterval(function () {
    self._ahoraTick(new Date());
  }, 60000); // cada 1 minuto

  //#region Variables Prescripcion

  self.elementosPrescripcion = ko.observableArray();
  self.prescripcionNuevaMedicamento = ko.observable();
  self.prescripcionNuevaCantidad = ko.observable();
  self.prescripcionNuevaObservaciones = ko.observable();

  //#endregion

  self.codigoServicioDentalBuscar = ko.observable();
  self.valorTotalConsulta = ko.observable(0);
  self.caracteristicasDentales = ko.observableArray();
  self.servicios = ko.observableArray();
  self.servicioSeleccionado = ko.observable();
  self.servicioSeleccionadoCantidad = ko.observable();
  self.servicioSeleccionadoDiente = ko.observable();
  self.precios = ko.observableArray();
  self.preciosDiente = ko.observableArray();
  self.precioSeleccionado = ko.observable();
  self.precioSeleccionadoDiente = ko.observable();
  self.serviciosAgregadosConsulta = ko.observableArray();
  self.seguimientosNutricionales = ko.observableArray();
  self.examenesLaboratorio = ko.observableArray();
  self.rangosSaludables = ko.observableArray();
  self.vacunas = ko.observableArray();
  self.antecedentesFamiliares = ko.observableArray();
  self.verServiciosDentales = ko.observable(false);
  self.dienteSeleccionado = ko.observable(0);

  self.medicamentoFechaOtros = ko.observable(
    new Date().toISOString().slice(0, 10)
  );
  self.medicamentoNombreOtros = ko.observable("");
  self.medicamentoCantidadOtros = ko.observable("");
  self.medicamentoIndicacionesOtros = ko.observable("");
  self.medicamentosOtros = ko.observableArray(MedicamentosOtrosData || []);

  //#region Variables verDiente
  self.verDiente1 = ko.observable(false);
  self.verDiente2 = ko.observable(false);
  self.verDiente3 = ko.observable(false);
  self.verDiente4 = ko.observable(false);
  self.verDiente5 = ko.observable(false);
  self.verDiente6 = ko.observable(false);
  self.verDiente7 = ko.observable(false);
  self.verDiente8 = ko.observable(false);
  self.verDiente9 = ko.observable(false);
  self.verDiente10 = ko.observable(false);
  self.verDiente11 = ko.observable(false);
  self.verDiente12 = ko.observable(false);
  self.verDiente13 = ko.observable(false);
  self.verDiente14 = ko.observable(false);
  self.verDiente15 = ko.observable(false);
  self.verDiente16 = ko.observable(false);
  self.verDiente17 = ko.observable(false);
  self.verDiente18 = ko.observable(false);
  self.verDiente19 = ko.observable(false);
  self.verDiente20 = ko.observable(false);
  self.verDiente21 = ko.observable(false);
  self.verDiente22 = ko.observable(false);
  self.verDiente23 = ko.observable(false);
  self.verDiente24 = ko.observable(false);
  self.verDiente25 = ko.observable(false);
  self.verDiente26 = ko.observable(false);
  self.verDiente27 = ko.observable(false);
  self.verDiente28 = ko.observable(false);
  self.verDiente29 = ko.observable(false);
  self.verDiente30 = ko.observable(false);
  self.verDiente31 = ko.observable(false);
  self.verDiente32 = ko.observable(false);
  //#endregion
  //#region Variables servicios dientes
  self.serviciosAgregadosDiente1 = ko.observableArray();
  self.serviciosAgregadosDiente2 = ko.observableArray();
  self.serviciosAgregadosDiente3 = ko.observableArray();
  self.serviciosAgregadosDiente4 = ko.observableArray();
  self.serviciosAgregadosDiente5 = ko.observableArray();
  self.serviciosAgregadosDiente6 = ko.observableArray();
  self.serviciosAgregadosDiente7 = ko.observableArray();
  self.serviciosAgregadosDiente8 = ko.observableArray();
  self.serviciosAgregadosDiente9 = ko.observableArray();
  self.serviciosAgregadosDiente10 = ko.observableArray();
  self.serviciosAgregadosDiente11 = ko.observableArray();
  self.serviciosAgregadosDiente12 = ko.observableArray();
  self.serviciosAgregadosDiente13 = ko.observableArray();
  self.serviciosAgregadosDiente14 = ko.observableArray();
  self.serviciosAgregadosDiente15 = ko.observableArray();
  self.serviciosAgregadosDiente16 = ko.observableArray();
  self.serviciosAgregadosDiente17 = ko.observableArray();
  self.serviciosAgregadosDiente18 = ko.observableArray();
  self.serviciosAgregadosDiente19 = ko.observableArray();
  self.serviciosAgregadosDiente20 = ko.observableArray();
  self.serviciosAgregadosDiente21 = ko.observableArray();
  self.serviciosAgregadosDiente22 = ko.observableArray();
  self.serviciosAgregadosDiente23 = ko.observableArray();
  self.serviciosAgregadosDiente24 = ko.observableArray();
  self.serviciosAgregadosDiente25 = ko.observableArray();
  self.serviciosAgregadosDiente26 = ko.observableArray();
  self.serviciosAgregadosDiente27 = ko.observableArray();
  self.serviciosAgregadosDiente28 = ko.observableArray();
  self.serviciosAgregadosDiente29 = ko.observableArray();
  self.serviciosAgregadosDiente30 = ko.observableArray();
  self.serviciosAgregadosDiente31 = ko.observableArray();
  self.serviciosAgregadosDiente32 = ko.observableArray();
  //#endregion
  //Examenes
  //self.codigoExamenBuscar = ko.observable();
  self.examenSeleccionado = ko.observable();
  self.precioSeleccionadoExamen = ko.observable();
  self.examenesExistentes = ko.observableArray(); // a esta variable se le guardan todos los examenes existentes y es la que imprime en el select
  self.examenesVenta = ko.observableArray();
  self.valorTotalExamenes = ko.observable(0);
  self.valorSubtotalExamenes = ko.observable(0);
  self.examenesArchivo = ko.observableArray();
  self.preciosExamen = ko.observableArray();
  //Productos
  self.codigoProductoBuscar = ko.observable();
  self.nombreProductoBuscar = ko.observable();
  self.productoSeleccionado = ko.observable();
  self.registrosInventario = ko.observableArray();
  self.productosExistentes = ko.observableArray();
  //self.productosBuscadosNombre = ko.observableArray();
  self.productosPrescripcion = ko.observableArray();
  self.preciosProducto = ko.observableArray();
  self.precioSeleccionadoProducto = ko.observable();
  self.unidadesVentaProducto = ko.observableArray();
  self.unidadVentaSeleccionadaProducto = ko.observableArray();

  //HOSPITALIZACION
  self.habitacionesDisponibles = ko.observableArray();

  //Servicios
  //self.codigoServicioBuscar = ko.observable();
  self.servicioSeleccionado = ko.observable();
  self.precioSeleccionadoServicio = ko.observable();
  self.serviciosExistentes = ko.observableArray();
  self.serviciosVenta = ko.observableArray();
  self.preciosServicio = ko.observableArray();
  //totales
  //self.ventaSubtotal = ko.observable(0);
  //self.ventaDescuento = ko.observable(0);
  //self.ventaTotal = ko.observable(0);
  self.pagoMonto = ko.observable();
  self.pagoVuelto = ko.observable(0);
  self.pagoSaldo = ko.observable(0);
  let itemExamen = 1000;
  self.citaId = ko.observable(0);

  //prescripciones exisentes cargar a table
  self.prescripcionesExistEdit = ko.observableArray();
  self.Color = ko.observable("#FFFFFF"); // Inicializa el color como blanco

  //Examenes
  self.examenesExistentes = ko.observableArray();
  self.examenSeleccionado = ko.observable();
  self.preciosExamen = ko.observableArray();
  self.precioSeleccionadoExamen = ko.observable();
  //self.examenesAgregados = ko.observableArray();
  let itemExamenAgregado = 1;
  self.txtModalConfirmacion = ko.observable();
  //Preguntas examenes
  self.pregunta = ko.observable(); // Variable observable para almacenar la pregunta ingresada
  self.examenesPreguntas = ko.observableArray(); //Almacenar las preguntas en los ecamenes

  //Variable para cargar un archivo consulta examen
  self.nombreNuevoArchivo = ko.observable();

  self.archivosConsulta = ko.observableArray();

  //Array que contiene las consultas realizadas
  //al mismo paciente
  self.historialConsultas = ko.observableArray();

  //#region Variables TABS
  self.tabAbiertaArchivos = ko.observable(false);
  self.tabAbiertaHospitalizacion = ko.observable(false);
  //#endregion
  self.modelSignosVitales = {};

  // --- Contadores de Cita (Datos Generales) ---
  self.contadorCitaAgendada = ko.observable(null);
  self.contadorCitaIniciada = ko.observable(null);
  self.contadorCitaFinalizada = ko.observable(null);

  self.cargarContadoresCita = function () {
    var raw = $("#CitaId").val();
    var citaId = parseInt(raw, 10);

    console.log(
      "[cargarContadoresCita] raw CitaId=",
      raw,
      "-> citaId=",
      citaId
    );

    if (isNaN(citaId) || citaId <= 0) {
      console.warn("[cargarContadoresCita] citaId inválido, no se consulta.");
      return;
    }

    $.ajax({
      url: "/Consultas/ObtenerContadoresCita",
      type: "GET",
      data: { citaId: citaId },
      cache: false,
    })
      .done(function (data) {
        console.log("[cargarContadoresCita] response=", data);

        // Soporta PascalCase y camelCase (por default ASP.NET Core suele devolver camelCase)
        var ag =
          (data && (data.ContadorCitaAgendada ?? data.contadorCitaAgendada)) ??
          null;
        var ini =
          (data && (data.ContadorCitaIniciada ?? data.contadorCitaIniciada)) ??
          null;
        var fin =
          (data &&
            (data.ContadorCitaFinalizada ?? data.contadorCitaFinalizada)) ??
          null;

        console.log("[cargarContadoresCita] mapped ->", { ag, ini, fin });

        self.contadorCitaAgendada(ag);
        self.contadorCitaIniciada(ini);
        self.contadorCitaFinalizada(fin);
      })
      .fail(function (jqXHR, textStatus, errorThrown) {
        console.error(
          "[cargarContadoresCita] FAIL ->",
          "status=",
          jqXHR.status,
          "textStatus=",
          textStatus,
          "errorThrown=",
          errorThrown,
          "responseText=",
          jqXHR.responseText
        );
      });
  };

  self._parseNetDate = function (value) {
    if (!value) return null;

    // Caso 1: /Date(1690000000000)/
    var m = /\/Date\((\d+)\)\//.exec(value);
    if (m && m[1]) {
      var ms = parseInt(m[1], 10);
      var dMs = new Date(ms);
      return isNaN(dMs.getTime()) ? null : dMs;
    }

    // Caso 2: ISO con timezone explícito (Z o ±HH:mm) -> parse nativo OK
    var hasTz = /[zZ]$|[+-]\d{2}:\d{2}$/.test(value);
    if (hasTz) {
      var dTz = new Date(value);
      if (!isNaN(dTz.getTime())) return dTz;
    }

    // Caso 3: ISO sin timezone -> parse manual como UTC (estable)
    // Acepta "YYYY-MM-DDTHH:mm:ss" o "YYYY-MM-DD HH:mm:ss"
    var iso = /^(\d{4})-(\d{2})-(\d{2})[T\s](\d{2}):(\d{2})(?::(\d{2}))?/.exec(
      value
    );
    if (iso) {
      var y = parseInt(iso[1], 10);
      var mo = parseInt(iso[2], 10) - 1;
      var da = parseInt(iso[3], 10);
      var hh = parseInt(iso[4], 10);
      var mi = parseInt(iso[5], 10);
      var ss = parseInt(iso[6] || "0", 10);
      return new Date(Date.UTC(y, mo, da, hh, mi, ss));
    }

    return null;
  };

  self._formatHora = function (dateObj) {
    if (!dateObj) return "";

    // Aplicar offset fijo y luego leer en UTC para no depender del timezone local
    var d = self._applyFixedOffset(dateObj);

    var h = d.getUTCHours();
    var m = d.getUTCMinutes();

    var ampm = h >= 12 ? "p. m." : "a. m.";
    h = h % 12;
    if (h === 0) h = 12;

    var mm = (m < 10 ? "0" : "") + m;
    return h + ":" + mm + " " + ampm;
  };

  self._formatFecha = function (dateObj) {
    if (!dateObj) return "";

    var d = self._applyFixedOffset(dateObj);

    var day = d.getUTCDate();
    var month = d.getUTCMonth() + 1;
    var year = d.getUTCFullYear();

    var dd = (day < 10 ? "0" : "") + day;
    var mm = (month < 10 ? "0" : "") + month;

    return dd + "/" + mm + "/" + year;
  };

  // Diferencia por calendario real: years/months + remainder days/hours/minutes
  self._diffCalendario = function (from, to) {
    if (!from || !to) return null;

    var fromMs = from.getTime();
    var toMs = to.getTime();
    var diffMs = toMs - fromMs;

    if (diffMs < 0) diffMs = 0; // por seguridad

    var totalMinutes = Math.floor(diffMs / 60000);
    var days = Math.floor(totalMinutes / (24 * 60));
    var remMinAfterDays = totalMinutes - days * 24 * 60;

    var hours = Math.floor(remMinAfterDays / 60);
    var minutes = remMinAfterDays - hours * 60;

    return {
      days: days,
      hours: hours,
      minutes: minutes,
    };
  };

  self._humanizeHace = function (fromDate, nowDate) {
    if (!fromDate || !nowDate) return "";

    var diff = self._diffCalendario(fromDate, nowDate);
    if (!diff) return "";

    // Si viene invertido, es "en ..." (fecha futura)
    var prefix = diff.invertido ? "en " : "hace ";

    // Regla de salida: años/meses/días/horas/minutos/segundos
    if (diff.years > 0) {
      var s = prefix + diff.years + " año" + (diff.years === 1 ? "" : "s");
      if (diff.months > 0)
        s += " y " + diff.months + " mes" + (diff.months === 1 ? "" : "es");
      return s;
    }

    if (diff.months > 0) {
      var s2 = prefix + diff.months + " mes" + (diff.months === 1 ? "" : "es");
      if (diff.days > 0)
        s2 += " y " + diff.days + " día" + (diff.days === 1 ? "" : "s");
      return s2;
    }

    if (diff.days > 0) {
      var s3 = prefix + diff.days + " día" + (diff.days === 1 ? "" : "s");
      if (diff.hours > 0)
        s3 += " y " + diff.hours + " hora" + (diff.hours === 1 ? "" : "s");
      return s3;
    }

    if (diff.hours > 0) {
      var s4 = prefix + diff.hours + " hora" + (diff.hours === 1 ? "" : "s");
      if (diff.minutes > 0)
        s4 +=
          " y " + diff.minutes + " minuto" + (diff.minutes === 1 ? "" : "s");
      return s4;
    }

    if (diff.minutes > 0) {
      return (
        prefix + diff.minutes + " minuto" + (diff.minutes === 1 ? "" : "s")
      );
    }

    return diff.invertido ? "en unos segundos" : "hace unos segundos";
  };

  self.citaAgendadaHaceTexto = ko.pureComputed(function () {
    self._ahoraTick(); // refresca el "hace..." cada minuto
    var d = self._parseNetDate(self.contadorCitaAgendada());
    if (!d) return "Cita Agendada: N/D";
    return "Cita Agendada desde " + self._humanizeHace(d, new Date());
  });

  self.citaAgendadaFechaTexto = ko.pureComputed(function () {
    var d = self._parseNetDate(self.contadorCitaAgendada());
    if (!d) return "";
    return "Fecha: " + self._formatFecha(d);
  });

  self.citaAgendadaHoraTexto = ko.pureComputed(function () {
    var d = self._parseNetDate(self.contadorCitaAgendada());
    if (!d) return "";
    return "Hora: " + self._formatHora(d);
  });

  self.citaIniciadaHaceTexto = ko.pureComputed(function () {
    self._ahoraTick();
    var d = self._parseNetDate(self.contadorCitaIniciada());
    if (!d) return "Cita Iniciada: N/D";
    return "Cita Iniciada desde " + self._humanizeHace(d, new Date());
  });

  self.citaIniciadaFechaTexto = ko.pureComputed(function () {
    var d = self._parseNetDate(self.contadorCitaIniciada());
    if (!d) return "";
    return "Fecha: " + self._formatFecha(d);
  });

  self.citaIniciadaHoraTexto = ko.pureComputed(function () {
    var d = self._parseNetDate(self.contadorCitaIniciada());
    if (!d) return "";
    return "Hora: " + self._formatHora(d);
  });

  self.citaFinalizadaHaceTexto = ko.pureComputed(function () {
    self._ahoraTick();
    var d = self._parseNetDate(self.contadorCitaFinalizada());
    if (!d) return "Cita Finalizada: N/D";
    return "Cita Finalizada desde " + self._humanizeHace(d, new Date());
  });

  self.citaFinalizadaFechaTexto = ko.pureComputed(function () {
    var d = self._parseNetDate(self.contadorCitaFinalizada());
    if (!d) return "";
    return "Fecha: " + self._formatFecha(d);
  });

  self.citaFinalizadaHoraTexto = ko.pureComputed(function () {
    var d = self._parseNetDate(self.contadorCitaFinalizada());
    if (!d) return "";
    return "Hora: " + self._formatHora(d);
  });

  self.tiempoAgendarIniciarTexto = ko.pureComputed(function () {
    var ag = self._parseNetDate(self.contadorCitaAgendada());
    var ini = self._parseNetDate(self.contadorCitaIniciada());

    if (!ag || !ini) return "N/D";
    return self._humanizeDuracion(ag, ini);
  });

  self.tiempoTotalTexto = ko.pureComputed(function () {
    var ag = self._parseNetDate(self.contadorCitaAgendada());
    var fin = self._parseNetDate(self.contadorCitaFinalizada());

    if (!ag || !fin) return "N/D";
    return self._humanizeDuracion(ag, fin);
  });

  self.tiempoIniciarFinalizarTexto = ko.pureComputed(function () {
    var ini = self._parseNetDate(self.contadorCitaIniciada());
    var fin = self._parseNetDate(self.contadorCitaFinalizada());

    if (!ini || !fin) return "N/D";
    return self._humanizeDuracion(ini, fin);
  });

  self._humanizeDuracion = function (fromDate, toDate) {
    if (!fromDate || !toDate) return "N/D";

    var diff = self._diffCalendario(fromDate, toDate);
    if (!diff) return "N/D";

    // Si está invertido (to < from), lo tratamos como N/D para duraciones
    if (diff.invertido) return "N/D";

    // Requisito: sin días/meses/años. Si hay días/meses/años, se convierten a horas.
    var totalMinutes =
      (diff.minutes || 0) +
      (diff.hours || 0) * 60 +
      (diff.days || 0) * 24 * 60 +
      (diff.months || 0) * 30 * 24 * 60 +
      (diff.years || 0) * 365 * 24 * 60;

    if (totalMinutes <= 0) return "menos de 1 minuto";

    var hours = Math.floor(totalMinutes / 60);
    var minutes = totalMinutes - hours * 60;

    if (hours <= 0) {
      return minutes + " minuto" + (minutes === 1 ? "" : "s");
    }

    if (minutes <= 0) {
      return hours + " hora" + (hours === 1 ? "" : "s");
    }

    return (
      hours +
      " hora" +
      (hours === 1 ? "" : "s") +
      " y " +
      minutes +
      " minuto" +
      (minutes === 1 ? "" : "s")
    );
  };

  self.agregarMedicamentoOtros = function () {
    console.log("Función agregarMedicamentoOtros ejecutada");
    console.log("Valor del nombre:", self.medicamentoNombreOtros());
    console.log("Valor de cantidad:", self.medicamentoCantidadOtros());
    console.log("Valor de indicaciones:", self.medicamentoIndicacionesOtros());

    if (self.medicamentoNombreOtros()) {
      var nuevoMedicamento = {
        id: self.medicamentoIndicacionesOtros(),
        nombre: self.medicamentoNombreOtros(),
        indicaciones: self.medicamentoIndicacionesOtros() || "",
        cantidad: self.medicamentoCantidadOtros() || 1,
        fechaPrescripcion: self.medicamentoFechaOtros()
      };

      console.log("Agregando medicamento:", nuevoMedicamento);
      self.medicamentosOtros.push(nuevoMedicamento);
      console.log(
        "Array después de agregar:",
        ko.toJS(self.medicamentosOtros())
      );

      // Limpiar campos
      self.medicamentoNombreOtros("");
      self.medicamentoIndicacionesOtros("");
      self.medicamentoCantidadOtros("");
    } else {
      console.log("El nombre está vacío, no se agrega el medicamento");
      alert("Por favor, complete el nombre del medicamento.");
    }
  };

  self.eliminarMedicamentoOtros = function (medicamento) {
    self.medicamentosOtros.remove(medicamento);
  };

    self.formatearFechaPrescripcion = function (fecha) {
    var valorFecha = ko.unwrap(fecha);
    if (!valorFecha) return '';
    
    var d = new Date(valorFecha);
    if (isNaN(d)) return '';
    
    var dd = ('0' + d.getDate()).slice(-2);
    var mm = ('0' + (d.getMonth() + 1)).slice(-2);
    var yyyy = d.getFullYear();
    
    var h = d.getHours();
    var m = ('0' + d.getMinutes()).slice(-2);
    var ampm = h >= 12 ? 'PM' : 'AM';
    
    h = h % 12;
    if (h === 0) h = 12;
    var hh = ('0' + h).slice(-2);
    
    return dd + '/' + mm + '/' + yyyy + ' ' + hh + ':' + m + ' ' + ampm;
  };


  self.getModelSignosVitales = function () {
    self.modelSignosVitales = {
      ConsultaId: $("#ConsultaId").val(), // Recoge el valor desde el campo oculto
      ExamenFisicoEstadoGeneral: $("#ExamenFisicoEstadoGeneral").val(),
      ExamenFisicoPeso: $("#ExamenFisicoPeso").val(),
      ExamenFisicoTalla: $("#ExamenFisicoTalla").val(),
      ExamenFisicoFrecuenciaCardiaca: $(
        "#ExamenFisicoFrecuenciaCardiaca"
      ).val(),
      ExamenFisicoFrecuenciaRespiratoria: $(
        "#ExamenFisicoFrecuenciaRespiratoria"
      ).val(),
      ExamenFisicoPresionArterial: $("#ExamenFisicoPresionArterial").val(),
      ExamenFisicoTemperatura: $("#ExamenFisicoTemperatura").val(),
      ExamenFisicoSaturacionOxigeno: $("#ExamenFisicoSaturacionOxigeno").val(),
      ExamenFisicoGlasgow: $("#ExamenFisicoGlasgow").val(),
    };
  };

  self.getModelHistoriaClinica = function () {
    self.modelSignosVitales = {
      ConsultaId: $("#ConsultaId").val(),
      PacienteId: $("#PacienteId").val(),
      ConsultaMotivo: $("#ConsultaMotivo").val(),
      HistoriaEnfermedadActual: $("#HistoriaEnfermedadActual").val(),
      EstaEmbarazada: $("#EstaEmbarazada").val(),
      NumeroSemanasEmbarazo: $("#NumeroSemanasEmbarazo").val(),
      EstaAmamantando: $("#EstaAmamantando").val(),
      PacienteMedicos: $("#PacienteMedicos").val(),
      PacienteQuirurgicos: $("#PacienteQuirurgicos").val(),
      PacienteAlergias: $("#PacienteAlergias").val(),
      PacienteTraumaticos: $("#PacienteTraumaticos").val(),
      PacienteVicios: $("#PacienteVicios").val(),
      PacienteMedicamentos: $("#PacienteMedicamentos").val(),
      ExamenFisicoEstadoGeneral: $("#ExamenFisicoEstadoGeneral").val(),
      ExamenFisicoPeso: $("#ExamenFisicoPeso").val(),
      ExamenFisicoTalla: $("#ExamenFisicoTalla").val(),
      ExamenFisicoFrecuenciaCardiaca: $(
        "#ExamenFisicoFrecuenciaCardiaca"
      ).val(),
      ExamenFisicoFrecuenciaRespiratoria: $(
        "#ExamenFisicoFrecuenciaRespiratoria"
      ).val(),
      ExamenFisicoPresionArterial: $("#ExamenFisicoPresionArterial").val(),
      ExamenFisicoTemperatura: $("#ExamenFisicoTemperatura").val(),
      ExamenFisicoSaturacionOxigeno: $("#ExamenFisicoSaturacionOxigeno").val(),
      ExamenFisicoGlasgow: $("#ExamenFisicoGlasgow").val(),
      RevisionSistemasAparienciaGeneral: $(
        "#RevisionSistemasAparienciaGeneral"
      ).val(),
      RevisionSistemasCabeza: $("#RevisionSistemasCabeza").val(),
      RevisionSistemasOidosBoca: $("#RevisionSistemasOidosBoca").val(),
      RevisionSistemasCuello: $("#RevisionSistemasCuello").val(),
      RevisionSistemasTorax: $("#RevisionSistemasTorax").val(),
      RevisionSistemasAbdomen: $("#RevisionSistemasAbdomen").val(),
      RevisionSistemasDorsoYExtremidades: $(
        "#RevisionSistemasDorsoYExtremidades"
      ).val(),
      RevisionSistemasGenitales: $("#RevisionSistemasGenitales").val(),
      HistoriaClinicaImpresionClinica: $(
        "#HistoriaClinicaImpresionClinica"
      ).val(),
      HistoriaClinicaComentario: $("#HistoriaClinicaComentario").val(),
      Cie10Codigo: $("#cie10-codigo").val(),
    };
  };

  self.editarConsultaSignosVitales = function () {
    showLoading();
    // Llamar a la función para llenar el objeto modelSignosVitales
    self.getModelSignosVitales();

    $.ajax({
      method: "POST",
      url: "/Consultas/EditarConsultaSignosVitales",
      data: self.modelSignosVitales, // Usar self.modelSignosVitales aquí
      success: function (data, textStatus) {
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) window.close();
        else {
          hideLoading();
          alert(dataResult.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };

  self.editarConsultaHistoriaClinica = function () {
    showLoading();
    // Llamar a la función para llenar el objeto modelSignosVitales
    self.getModelHistoriaClinica();

    $.ajax({
      method: "POST",
      url: "/Consultas/EditarConsultaHistoriaClinica",
      data: self.modelSignosVitales, // Usar self.modelSignosVitales aquí
      success: function (data, textStatus) {
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) window.close();
        else {
          hideLoading();
          alert(dataResult.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };

  //#region Funciones Abrir TABS
  self.abrirTabArchivos = function () {
    if (!self.tabAbiertaArchivos()) {
      self.consultarArchivosConsulta();
      self.tabAbiertaArchivos(true);
    }
  };
  self.abrirTabHospitalizacion = function () {
    if (!self.tabAbiertaHospitalizacion()) {
      self.consultarHabitacionesDisponibles();
      self.tabAbiertaHospitalizacion(true);
    }
  };
  //#endregion

  //Funciones examenes a realizar=============
  self.consultarPreciosExamen = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Venta/ConsultarPreciosExamen",
      data: {
        examenLabClinicoId: self.examenSeleccionado().ExamenId,
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
  self.agregarExamen = function () {
    if (
      self.examenSeleccionado() != null &&
      self.examenSeleccionado() != undefined
    ) {
      if (
        self.precioSeleccionadoExamen() != null &&
        self.precioSeleccionadoExamen() != undefined
      ) {
        let examenAgregado = {
          Item: itemExamen,
          ExamenCodigo: self.examenSeleccionado().ExamenCodigo,
          ExamenId: self.examenSeleccionado().ExamenId,
          ExamenNombre: self.examenSeleccionado().ExamenNombre,
          PrecioId: self.precioSeleccionadoExamen().PrecioId,
          PrecioNombre: self.precioSeleccionadoExamen().PrecioNombre,
          ValorUnitario: self.precioSeleccionadoExamen().PrecioValor,
          ValorSubtotal: ko.observable(
            self.precioSeleccionadoExamen().PrecioValor
          ),
          ValorTotal: ko.observable(
            self.precioSeleccionadoExamen().PrecioValor
          ),
          ValorCubiertoSeguro: ko.observable(0),
          ValorCopago: ko.observable(
            self.precioSeleccionadoExamen().PrecioValor
          ),
          Cantidad: ko.observable(1),
          Pagado: false,
        };
        self.examenesVenta.push(examenAgregado);

        itemExamen++;
        mensajeEmergente("Examen agregado");
        self.actualizarTotales();
      } else {
        mensajeEmergenteError("No hay precios agregados para este examen");
      }
    } else {
      alert("No hay ningun examen seleccionado");
    }
  };
  self.consultarExamenesExistentes = function () {
    // consulta los examenes disponibles e inicaliza el select
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
  self.quitarExamen = function (value) {
    $(self.examenesVenta()).each(function (idx, examen) {
      if (value.Item == examen.Item) {
        self.examenesVenta.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };

  self.consultarHistorialConsultas = function () {
    let textosCargando = $(".texto-cargando-historial");
    let textosError = $(".texto-error-cargando-historial");
    textosError.hide();
    textosCargando.show();
    $.ajax({
      url: "/Consultas/ConsultarConsultasPaciente",
      method: "POST",
      data: {
        pacienteId: $("#PacienteId").val(),
      },
      success: function (result) {
        textosCargando.hide();
        let data = JSON.parse(result);
        if (data.Exitoso) {
          // Filtra la consulta actual del historial
          data.Resultado = data.Resultado.filter(function (consulta) {
            return consulta.ConsultaId != $("#ConsultaId").val();
          });

          // Toma las últimas dos consultas
          let ultimasConsultas = data.Resultado.slice(-2);

          self.historialConsultas(ultimasConsultas);
        } else {
          textosError.show();
          console.log(data.Mensaje);
        }
      },
      error: function (errorResult) {
        console.log(errorResult);
        textosError.show();
      },
    });
  };

  self.consultarExamenesAgregadosConsulta = function () {
    let consultaId = $("#ConsultaId").val();
    if (consultaId != null && consultaId.trim() != "") {
      //Se hace la consulta solo en caso de que venga un id de consulta en el modelo
      showLoading();
      self.examenesVenta([]);
      $.ajax({
        url: "/Consultas/ConsultarExamenesAgregadosConsulta",
        method: "POST",
        data: {
          consultaId: consultaId,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            $(data.Resultado).each(function (idx, examen) {
              itemExamen++;
              examen.Item = itemExamen;
              examen.ValorSubtotal = ko.observable(examen.ValorSubtotal);
              examen.ValorTotal = ko.observable(examen.ValorSubtotal());
              examen.Cantidad = ko.observable(examen.Cantidad);
              self.examenesVenta.push(examen);
            });
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
          self.actualizarTotales();
        },
        error: function (dataError) {
          hideLoading();
          console.log("ERROR DE LLAMADO ASINCRONO: " + dataError.error);
          console.log(dataError);
        },
      });
    }
  };
  //self.consultarServiciosConsulta = function () {
  //    let consultaId = $("#ConsultaId").val();
  //    if (consultaId != null) {
  //        //Se hace la consulta solo en caso de que venga un id de consulta en el modelo
  //        showLoading();
  //        $.ajax({
  //            url: "/Consultas/ConsultarPrescripcionesyExamenesConsulta",
  //            method: "POST",
  //            data: {
  //                consultaId: consultaId
  //            },
  //            success: function (dataResult) {
  //                hideLoading();
  //                let data = JSON.parse(dataResult);
  //                if (data.Exitoso) {

  //                    self.examenesVenta(data.Resultado);
  //                    if (data.Resultado.length == 0) {
  //                        /*alert("No hay servicios agregados en esta consulta");*/
  //                    } else {
  //                        mensajeEmergente("Examnes agregados");
  //                    }
  //                } else {
  //                    alert(data.Mensaje);
  //                }
  //                self.actualizarTotales();
  //            },
  //            error: function (dataError) {
  //                hideLoading();
  //                console.log("ERROR DE LLAMADO ASINCRONO: " + dataError.error);
  //                console.log(dataError);
  //            }
  //        });
  //    }
  //};

  self.editarPaciente = function () {
    window.open("/Pacientes/Modificar?id=" + $("#PacienteId").val(), "_blank");
  };

  self.ocultarServiciosDientes = function () {
    self.verServiciosDentales(false);
    self.verDiente1(false);
    self.verDiente2(false);
    self.verDiente3(false);
    self.verDiente4(false);
    self.verDiente5(false);
    self.verDiente6(false);
    self.verDiente7(false);
    self.verDiente8(false);
    self.verDiente9(false);
    self.verDiente10(false);
    self.verDiente11(false);
    self.verDiente12(false);
    self.verDiente13(false);
    self.verDiente14(false);
    self.verDiente15(false);
    self.verDiente16(false);
    self.verDiente17(false);
    self.verDiente18(false);
    self.verDiente19(false);
    self.verDiente20(false);
    self.verDiente21(false);
    self.verDiente22(false);
    self.verDiente23(false);
    self.verDiente24(false);
    self.verDiente25(false);
    self.verDiente26(false);
    self.verDiente27(false);
    self.verDiente28(false);
    self.verDiente29(false);
    self.verDiente30(false);
    self.verDiente31(false);
    self.verDiente32(false);
    $("#caracteristicas-diente-1").hide();
    $("#caracteristicas-diente-2").hide();
    $("#caracteristicas-diente-3").hide();
    $("#caracteristicas-diente-4").hide();
    $("#caracteristicas-diente-5").hide();
    $("#caracteristicas-diente-6").hide();
    $("#caracteristicas-diente-7").hide();
    $("#caracteristicas-diente-8").hide();
    $("#caracteristicas-diente-9").hide();
    $("#caracteristicas-diente-10").hide();
    $("#caracteristicas-diente-11").hide();
    $("#caracteristicas-diente-12").hide();
    $("#caracteristicas-diente-13").hide();
    $("#caracteristicas-diente-14").hide();
    $("#caracteristicas-diente-15").hide();
    $("#caracteristicas-diente-16").hide();
    $("#caracteristicas-diente-17").hide();
    $("#caracteristicas-diente-18").hide();
    $("#caracteristicas-diente-19").hide();
    $("#caracteristicas-diente-20").hide();
    $("#caracteristicas-diente-21").hide();
    $("#caracteristicas-diente-22").hide();
    $("#caracteristicas-diente-23").hide();
    $("#caracteristicas-diente-24").hide();
    $("#caracteristicas-diente-25").hide();
    $("#caracteristicas-diente-26").hide();
    $("#caracteristicas-diente-27").hide();
    $("#caracteristicas-diente-28").hide();
    $("#caracteristicas-diente-29").hide();
    $("#caracteristicas-diente-30").hide();
    $("#caracteristicas-diente-31").hide();
    $("#caracteristicas-diente-32").hide();
  };
  self.verServiciosDiente1 = function () {
    self.ocultarServiciosDientes();
    self.verDiente1(true);
    self.dienteSeleccionado(1);
    $("#caracteristicas-diente-1").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente2 = function () {
    self.ocultarServiciosDientes();
    self.verDiente2(true);
    self.dienteSeleccionado(2);
    $("#caracteristicas-diente-2").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente3 = function () {
    self.ocultarServiciosDientes();
    self.verDiente3(true);
    self.dienteSeleccionado(3);
    $("#caracteristicas-diente-3").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente4 = function () {
    self.ocultarServiciosDientes();
    self.verDiente4(true);
    self.dienteSeleccionado(4);
    $("#caracteristicas-diente-4").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente5 = function () {
    self.ocultarServiciosDientes();
    self.verDiente5(true);
    self.dienteSeleccionado(5);
    $("#caracteristicas-diente-5").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente6 = function () {
    self.ocultarServiciosDientes();
    self.verDiente6(true);
    self.dienteSeleccionado(6);
    $("#caracteristicas-diente-6").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente7 = function () {
    self.ocultarServiciosDientes();
    self.verDiente7(true);
    self.dienteSeleccionado(7);
    $("#caracteristicas-diente-7").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente8 = function () {
    self.ocultarServiciosDientes();
    self.verDiente8(true);
    self.dienteSeleccionado(8);
    $("#caracteristicas-diente-8").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente9 = function () {
    self.ocultarServiciosDientes();
    self.verDiente9(true);
    self.dienteSeleccionado(9);
    $("#caracteristicas-diente-9").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente10 = function () {
    self.ocultarServiciosDientes();
    self.verDiente10(true);
    self.dienteSeleccionado(10);
    $("#caracteristicas-diente-10").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente11 = function () {
    self.ocultarServiciosDientes();
    self.verDiente11(true);
    self.dienteSeleccionado(11);
    $("#caracteristicas-diente-11").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente12 = function () {
    self.ocultarServiciosDientes();
    self.verDiente12(true);
    self.dienteSeleccionado(12);
    $("#caracteristicas-diente-12").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente13 = function () {
    self.ocultarServiciosDientes();
    self.verDiente13(true);
    self.dienteSeleccionado(13);
    $("#caracteristicas-diente-13").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente14 = function () {
    self.ocultarServiciosDientes();
    self.verDiente14(true);
    self.dienteSeleccionado(14);
    $("#caracteristicas-diente-14").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente15 = function () {
    self.ocultarServiciosDientes();
    self.verDiente15(true);
    self.dienteSeleccionado(15);
    $("#caracteristicas-diente-15").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente16 = function () {
    self.ocultarServiciosDientes();
    self.verDiente16(true);
    self.dienteSeleccionado(16);
    $("#caracteristicas-diente-16").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente17 = function () {
    self.ocultarServiciosDientes();
    self.verDiente17(true);
    self.dienteSeleccionado(17);
    $("#caracteristicas-diente-17").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente18 = function () {
    self.ocultarServiciosDientes();
    self.verDiente18(true);
    self.dienteSeleccionado(18);
    $("#caracteristicas-diente-18").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente19 = function () {
    self.ocultarServiciosDientes();
    self.verDiente19(true);
    self.dienteSeleccionado(19);
    $("#caracteristicas-diente-19").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente20 = function () {
    self.ocultarServiciosDientes();
    self.verDiente20(true);
    self.dienteSeleccionado(20);
    $("#caracteristicas-diente-20").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente21 = function () {
    self.ocultarServiciosDientes();
    self.verDiente21(true);
    self.dienteSeleccionado(21);
    $("#caracteristicas-diente-21").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente22 = function () {
    self.ocultarServiciosDientes();
    self.verDiente22(true);
    self.dienteSeleccionado(22);
    $("#caracteristicas-diente-22").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente23 = function () {
    self.ocultarServiciosDientes();
    self.verDiente23(true);
    self.dienteSeleccionado(23);
    $("#caracteristicas-diente-23").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente24 = function () {
    self.ocultarServiciosDientes();
    self.verDiente24(true);
    self.dienteSeleccionado(24);
    $("#caracteristicas-diente-24").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente25 = function () {
    self.ocultarServiciosDientes();
    self.verDiente25(true);
    self.dienteSeleccionado(25);
    $("#caracteristicas-diente-25").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente26 = function () {
    self.ocultarServiciosDientes();
    self.verDiente26(true);
    self.dienteSeleccionado(26);
    $("#caracteristicas-diente-26").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente27 = function () {
    self.ocultarServiciosDientes();
    self.verDiente27(true);
    self.dienteSeleccionado(27);
    $("#caracteristicas-diente-27").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente28 = function () {
    self.ocultarServiciosDientes();
    self.verDiente28(true);
    self.dienteSeleccionado(28);
    $("#caracteristicas-diente-28").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente29 = function () {
    self.ocultarServiciosDientes();
    self.verDiente29(true);
    self.dienteSeleccionado(29);
    $("#caracteristicas-diente-29").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente30 = function () {
    self.ocultarServiciosDientes();
    self.verDiente30(true);
    self.dienteSeleccionado(30);
    $("#caracteristicas-diente-30").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente31 = function () {
    self.ocultarServiciosDientes();
    self.verDiente31(true);
    self.dienteSeleccionado(31);
    $("#caracteristicas-diente-31").show();
    self.verServiciosDentales(true);
  };
  self.verServiciosDiente32 = function () {
    self.ocultarServiciosDientes();
    self.verDiente32(true);
    self.dienteSeleccionado(32);
    $("#caracteristicas-diente-32").show();
    self.verServiciosDentales(true);
  };

  //self.codigoServicioBuscar.subscribe(function (value) {
  //    $(self.servicios()).each(function (idx, servicio) {
  //        if (servicio.CodigoInterno == value) {
  //            self.servicioSeleccionado(servicio);
  //        }
  //    });
  //    self.consultarPrecios();
  //});
  self.codigoServicioDentalBuscar.subscribe(function (value) {
    $(self.servicios()).each(function (idx, servicio) {
      if (servicio.CodigoInterno == value) {
        self.servicioSeleccionadoDiente(servicio);
      }
    });
    self.consultarPreciosDiente();
  });

  self.consultarServiciosAgregadosConsulta = function () {
    self.serviciosAgregadosConsulta([]);
    self.serviciosAgregadosDiente1([]);
    self.serviciosAgregadosDiente2([]);
    self.serviciosAgregadosDiente3([]);
    self.serviciosAgregadosDiente4([]);
    self.serviciosAgregadosDiente5([]);
    self.serviciosAgregadosDiente6([]);
    self.serviciosAgregadosDiente7([]);
    self.serviciosAgregadosDiente8([]);
    self.serviciosAgregadosDiente9([]);
    self.serviciosAgregadosDiente10([]);
    self.serviciosAgregadosDiente11([]);
    self.serviciosAgregadosDiente12([]);
    self.serviciosAgregadosDiente13([]);
    self.serviciosAgregadosDiente14([]);
    self.serviciosAgregadosDiente15([]);
    self.serviciosAgregadosDiente16([]);
    self.serviciosAgregadosDiente17([]);
    self.serviciosAgregadosDiente18([]);
    self.serviciosAgregadosDiente19([]);
    self.serviciosAgregadosDiente20([]);
    self.serviciosAgregadosDiente21([]);
    self.serviciosAgregadosDiente22([]);
    self.serviciosAgregadosDiente23([]);
    self.serviciosAgregadosDiente24([]);
    self.serviciosAgregadosDiente25([]);
    self.serviciosAgregadosDiente26([]);
    self.serviciosAgregadosDiente27([]);
    self.serviciosAgregadosDiente28([]);
    self.serviciosAgregadosDiente29([]);
    self.serviciosAgregadosDiente30([]);
    self.serviciosAgregadosDiente31([]);
    self.serviciosAgregadosDiente32([]);

    if (idConsulta != "") {
      $.ajax({
        method: "POST",
        url: "/Consultas/ConsultarServiciosAgregadosConsulta",
        data: {
          consultaId: idConsulta,
        },
        success: function (data, textStatus) {
          var dataResult = JSON.parse(data);
          if (dataResult.Exitoso) {
            $(dataResult.Resultado).each(function (idx, vl) {
              vl.ServicioValorCubiertoSeguro = ko.observable(
                vl.ServicioValorCubiertoSeguro
              );
              vl.ServicioValorCopago = ko.observable(vl.ServicioValorCopago);
              switch (vl.NumeroDiente) {
                case 1:
                  self.serviciosAgregadosDiente1.push(vl);
                  break;
                case 2:
                  self.serviciosAgregadosDiente2.push(vl);
                  break;
                case 3:
                  self.serviciosAgregadosDiente3.push(vl);
                  break;
                case 4:
                  self.serviciosAgregadosDiente4.push(vl);
                  break;
                case 5:
                  self.serviciosAgregadosDiente5.push(vl);
                  break;
                case 6:
                  self.serviciosAgregadosDiente6.push(vl);
                  break;
                case 7:
                  self.serviciosAgregadosDiente7.push(vl);
                  break;
                case 8:
                  self.serviciosAgregadosDiente8.push(vl);
                  break;
                case 9:
                  self.serviciosAgregadosDiente9.push(vl);
                  break;
                case 10:
                  self.serviciosAgregadosDiente10.push(vl);
                  break;
                case 11:
                  self.serviciosAgregadosDiente11.push(vl);
                  break;
                case 12:
                  self.serviciosAgregadosDiente12.push(vl);
                  break;
                case 13:
                  self.serviciosAgregadosDiente13.push(vl);
                  break;
                case 14:
                  self.serviciosAgregadosDiente14.push(vl);
                  break;
                case 15:
                  self.serviciosAgregadosDiente15.push(vl);
                  break;
                case 16:
                  self.serviciosAgregadosDiente16.push(vl);
                  break;
                case 17:
                  self.serviciosAgregadosDiente17.push(vl);
                  break;
                case 18:
                  self.serviciosAgregadosDiente18.push(vl);
                  break;
                case 19:
                  self.serviciosAgregadosDiente19.push(vl);
                  break;
                case 20:
                  self.serviciosAgregadosDiente20.push(vl);
                  break;
                case 21:
                  self.serviciosAgregadosDiente21.push(vl);
                  break;
                case 22:
                  self.serviciosAgregadosDient22.push(vl);
                  break;
                case 23:
                  self.serviciosAgregadosDiente23.push(vl);
                  break;
                case 24:
                  self.serviciosAgregadosDiente24.push(vl);
                  break;
                case 25:
                  self.serviciosAgregadosDiente25.push(vl);
                  break;
                case 26:
                  self.serviciosAgregadosDiente26.push(vl);
                  break;
                case 27:
                  self.serviciosAgregadosDiente27.push(vl);
                  break;
                case 28:
                  self.serviciosAgregadosDiente28.push(vl);
                  break;
                case 29:
                  self.serviciosAgregadosDiente29.push(vl);
                  break;
                case 30:
                  self.serviciosAgregadosDiente30.push(vl);
                  break;
                case 31:
                  self.serviciosAgregadosDiente31.push(vl);
                  break;
                case 32:
                  self.serviciosAgregadosDiente32.push(vl);
                  break;
                default:
                  self.serviciosAgregadosConsulta.push(vl);
                  break;
              }
            });
            self.actualizarTotales();
          } else alert(data.Mensaje);
        },
        error: function (data) {
          alert(data.error);
        },
      });
    }

    if (idConsulta == "") {
      showLoading();
      $.ajax({
        method: "POST",
        url: "/Consultas/ConsultarServiciosCita",
        data: {
          citaId: $("#CitaId").val(),
        },
        success: function (data, textStatus) {
          hideLoading();
          var dataResult = JSON.parse(data);
          if (dataResult.Exitoso) {
            $(dataResult.Resultado).each(function (idx, value) {
              value.Item = itemServicioConsulta;
              self.serviciosAgregadosConsulta.push(value);
              itemServicioConsulta++;
            });
            self.actualizarTotales();
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (data) {
          hideLoading();
          alert("ERROR DE LLAMADO ASINCRONO: " + data.error);
        },
      });
    }
  };
  self.consultarCaracteristicasDentales = function () {
    $.ajax({
      method: "POST",
      url: "/Consultas/ConsultarCaracteristicasDentales",
      url: "/Consultas/ConsultarCaracteristicasDentales",
      data: {
        consultaId: idConsulta == "" ? null : idConsulta,
      },
      traditional: true,
      success: function (data, textStatus) {
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.caracteristicasDentales(dataResult.Resultado);
        } else alert(data.mensaje);
      },
      error: function (data) {
        alert(data.error);
      },
    });
  };
  self.consultarVacunasPaciente = function () {
    $.ajax({
      method: "POST",
      url: "/Consultas/ConsultarVacunasPaciente",
      data: {
        pacienteId: pacienteId,
      },
      traditional: true,
      success: function (data, textStatus) {
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.vacunas(dataResult.Resultado);
        } else alert(dataResult.Mensaje);
      },
      error: function (data) {
        alert(data.error);
      },
    });
  };

  self.consultarArchivosExamen = function () {
    $.ajax({
      method: "POST",
      url: "/Consultas/ConsultarExamenesArchivo",
      data: {
        consultaId: idConsulta,
      },
      traditional: true,
      success: function (data, textStatus) {
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.examenesArchivo(dataResult.Resultado);
        } else alert(dataResult.Mensaje);
      },
      error: function (data) {
        alert(data.error);
      },
    });
  };
  self.consultarAntecedentesFamiliaresPaciente = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Consultas/ConsultarAntecedentesFamiliaresPaciente",
      data: {
        pacienteId: pacienteId,
      },
      traditional: true,
      success: function (data, textStatus) {
        hideLoading();
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.antecedentesFamiliares(dataResult.Resultado);
        } else alert(dataResult.Mensaje);
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };
  self.consultarExamenesLaboratorio = function () {
    $.ajax({
      method: "POST",
      url: "/Consultas/ConsultarSeguimientosNutricionalesPaciente",
      data: {
        pacienteId: pacienteId,
      },
      traditional: true,
      success: function (data, textStatus) {
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          $(dataResult.Resultado).each(function (idx, vl) {
            if (vl.Fecha != null)
              vl.Fecha = moment(vl.Fecha).format("YYYY-MM-DD");
          });
          self.examenesLaboratorio(dataResult.Resultado);
        } else alert(dataResult.Mensaje);
      },
      error: function (data) {
        alert(data.error);
      },
    });
  };
  self.consultarSeguimientosNutricionales = function () {
    $.ajax({
      method: "POST",
      url: "/Consultas/ConsultarSeguimientosNutricionalesPaciente",
      data: {
        pacienteId: pacienteId,
      },
      traditional: true,
      success: function (data, textStatus) {
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          $(dataResult.Resultado).each(function (idx, vl) {
            if (vl.Fecha != null)
              vl.Fecha = moment(vl.Fecha).format("YYYY-MM-DD");
          });
          self.seguimientosNutricionales(dataResult.Resultado);
        } else alert(dataResult.Mensaje);
      },
      error: function (data) {
        alert(data.error);
      },
    });
  };
  self.consultarServicios = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Consultas/ConsultarServicios",
      traditional: true,
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.servicios(data.Resultado);
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
  self.consultarPrecios = function () {
    showLoading();
    $.ajax({
      url: "/Consultas/ConsultarPreciosServicio",
      method: "POST",
      data: {
        servicioId: self.servicioSeleccionado().Id,
      },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.precios(data.Resultado);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("ERROR INTERNO: " + dataError);
      },
    });
  };
  self.consultarPreciosDiente = function () {
    showLoading();
    $.ajax({
      url: "/Consultas/ConsultarPreciosServicio",
      method: "POST",
      data: {
        servicioId: self.servicioSeleccionadoDiente().Id,
      },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.preciosDiente(data.Resultado);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("ERROR INTERNO: " + dataError);
      },
    });
  };

  self.agregarServicioConsulta = function () {
    if (
      self.servicioSeleccionadoCantidad() == undefined ||
      self.servicioSeleccionadoCantidad() == null ||
      self.servicioSeleccionadoCantidad().trim() == "" ||
      isNaN(self.servicioSeleccionadoCantidad().trim())
    ) {
      alert("Proporcione una cantidad valida");
      return false;
    }
    var servicioAgregado = new Object();
    servicioAgregado.Item = itemServicioConsulta;
    servicioAgregado.ConsultaId = idConsulta == "" ? null : idConsulta;
    servicioAgregado.ServicioId = self.servicioSeleccionado().Id;
    servicioAgregado.ServicioCodigo = self.servicioSeleccionado().CodigoInterno;
    servicioAgregado.ServicioCantidad = self.servicioSeleccionadoCantidad();
    servicioAgregado.NombreServicio =
      self.servicioSeleccionado().NombreServicio;
    servicioAgregado.PrecioId =
      self.precioSeleccionado() == undefined
        ? null
        : self.precioSeleccionado().PrecioId;
    servicioAgregado.PrecioNombre =
      self.precioSeleccionado() == undefined
        ? null
        : self.precioSeleccionado().PrecioNombre;
    servicioAgregado.PrecioNombreValor =
      self.precioSeleccionado() == undefined
        ? null
        : self.precioSeleccionado().PrecioNombreValor;
    servicioAgregado.PrecioValor =
      self.precioSeleccionado() == undefined
        ? 0
        : self.precioSeleccionado().PrecioValor;
    servicioAgregado.ServicioValorTotal =
      servicioAgregado.PrecioValor * servicioAgregado.ServicioCantidad;
    servicioAgregado.ServicioValorCubiertoSeguro = ko.observable(0);
    servicioAgregado.ServicioValorCopago = ko.observable(
      self.precioSeleccionado() == undefined
        ? 0
        : self.precioSeleccionado().PrecioValor
    );
    servicioAgregado.Pagado = false;
    servicioAgregado.Aplicar = true;
    servicioAgregado.Nuevo = true;
    servicioAgregado.NumeroDiente = null;
    self.serviciosAgregadosConsulta.push(servicioAgregado);
    itemServicioConsulta++;
    self.servicioSeleccionadoCantidad("");
    //self.codigoServicioBuscar('');

    self.actualizarTotales();
  };
  self.quitarServicioConsulta = function (value) {
    $(self.serviciosAgregadosConsulta()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosConsulta.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.agregarServicioDiente = function () {
    var diente = self.dienteSeleccionado();

    var servicioAgregado = new Object();
    servicioAgregado.ConsultaId = idConsulta == "" ? null : idConsulta;
    servicioAgregado.ServicioId = self.servicioSeleccionadoDiente().Id;
    servicioAgregado.ServicioCodigo =
      self.servicioSeleccionadoDiente().CodigoInterno;
    servicioAgregado.ServicioCantidad = 1;
    servicioAgregado.NombreServicio =
      self.servicioSeleccionadoDiente().NombreServicio;
    servicioAgregado.PrecioId =
      self.precioSeleccionadoDiente() == undefined
        ? null
        : self.precioSeleccionadoDiente().PrecioId;
    servicioAgregado.PrecioNombre =
      self.precioSeleccionadoDiente() == undefined
        ? null
        : self.precioSeleccionadoDiente().PrecioNombre;
    servicioAgregado.PrecioNombreValor =
      self.precioSeleccionadoDiente() == undefined
        ? null
        : self.precioSeleccionadoDiente().PrecioNombreValor;
    servicioAgregado.PrecioValor =
      self.precioSeleccionadoDiente() == undefined
        ? null
        : self.precioSeleccionadoDiente().PrecioValor;
    servicioAgregado.NumeroDiente = diente;
    servicioAgregado.Aplicar = false;
    servicioAgregado.Nuevo = true;

    switch (diente) {
      case 1:
        servicioAgregado.Item = itemServicioDiente1;
        self.serviciosAgregadosDiente1.push(servicioAgregado);
        itemServicioDiente1++;
        break;
      case 2:
        servicioAgregado.Item = itemServicioDiente2;
        self.serviciosAgregadosDiente2.push(servicioAgregado);
        itemServicioDiente2++;
        break;
      case 3:
        servicioAgregado.Item = itemServicioDiente3;
        self.serviciosAgregadosDiente3.push(servicioAgregado);
        itemServicioDiente3++;
        break;
      case 4:
        servicioAgregado.Item = itemServicioDiente4;
        self.serviciosAgregadosDiente4.push(servicioAgregado);
        itemServicioDiente4++;
        break;
      case 5:
        servicioAgregado.Item = itemServicioDiente5;
        self.serviciosAgregadosDiente5.push(servicioAgregado);
        itemServicioDiente5++;
        break;
      case 6:
        servicioAgregado.Item = itemServicioDiente6;
        self.serviciosAgregadosDiente6.push(servicioAgregado);
        itemServicioDiente6++;
        break;
      case 7:
        servicioAgregado.Item = itemServicioDiente7;
        self.serviciosAgregadosDiente7.push(servicioAgregado);
        itemServicioDiente7++;
        break;
      case 8:
        servicioAgregado.Item = itemServicioDiente8;
        self.serviciosAgregadosDiente8.push(servicioAgregado);
        itemServicioDiente8++;
        break;
      case 9:
        servicioAgregado.Item = itemServicioDiente9;
        self.serviciosAgregadosDiente9.push(servicioAgregado);
        itemServicioDiente9++;
        break;
      case 10:
        servicioAgregado.Item = itemServicioDiente10;
        self.serviciosAgregadosDiente10.push(servicioAgregado);
        itemServicioDiente10++;
        break;
      case 11:
        servicioAgregado.Item = itemServicioDiente11;
        self.serviciosAgregadosDiente11.push(servicioAgregado);
        itemServicioDiente11++;
        break;
      case 12:
        servicioAgregado.Item = itemServicioDiente12;
        self.serviciosAgregadosDiente12.push(servicioAgregado);
        itemServicioDiente12++;
        break;
      case 13:
        servicioAgregado.Item = itemServicioDiente13;
        self.serviciosAgregadosDiente13.push(servicioAgregado);
        itemServicioDiente13++;
        break;
      case 14:
        servicioAgregado.Item = itemServicioDiente14;
        self.serviciosAgregadosDiente14.push(servicioAgregado);
        itemServicioDiente14++;
        break;
      case 15:
        servicioAgregado.Item = itemServicioDiente15;
        self.serviciosAgregadosDiente15.push(servicioAgregado);
        itemServicioDiente15++;
        break;
      case 16:
        servicioAgregado.Item = itemServicioDiente16;
        self.serviciosAgregadosDiente16.push(servicioAgregado);
        itemServicioDiente16++;
        break;
      case 17:
        servicioAgregado.Item = itemServicioDiente17;
        self.serviciosAgregadosDiente17.push(servicioAgregado);
        itemServicioDiente17++;
        break;
      case 18:
        servicioAgregado.Item = itemServicioDiente18;
        self.serviciosAgregadosDiente18.push(servicioAgregado);
        itemServicioDiente18++;
        break;
      case 19:
        servicioAgregado.Item = itemServicioDiente19;
        self.serviciosAgregadosDiente19.push(servicioAgregado);
        itemServicioDiente19++;
        break;
      case 20:
        servicioAgregado.Item = itemServicioDiente20;
        self.serviciosAgregadosDiente20.push(servicioAgregado);
        itemServicioDiente20++;
        break;
      case 21:
        servicioAgregado.Item = itemServicioDiente21;
        self.serviciosAgregadosDiente21.push(servicioAgregado);
        itemServicioDiente21++;
        break;
      case 22:
        servicioAgregado.Item = itemServicioDiente22;
        self.serviciosAgregadosDiente22.push(servicioAgregado);
        itemServicioDiente22++;
        break;
      case 23:
        servicioAgregado.Item = itemServicioDiente23;
        self.serviciosAgregadosDiente23.push(servicioAgregado);
        itemServicioDiente23++;
        break;
      case 24:
        servicioAgregado.Item = itemServicioDiente24;
        self.serviciosAgregadosDiente24.push(servicioAgregado);
        itemServicioDiente24++;
        break;
      case 25:
        servicioAgregado.Item = itemServicioDiente25;
        self.serviciosAgregadosDiente25.push(servicioAgregado);
        itemServicioDiente25++;
        break;
      case 26:
        servicioAgregado.Item = itemServicioDiente26;
        self.serviciosAgregadosDiente26.push(servicioAgregado);
        itemServicioDiente26++;
        break;
      case 27:
        servicioAgregado.Item = itemServicioDiente27;
        self.serviciosAgregadosDiente27.push(servicioAgregado);
        itemServicioDiente27++;
        break;
      case 28:
        servicioAgregado.Item = itemServicioDiente28;
        self.serviciosAgregadosDiente28.push(servicioAgregado);
        itemServicioDiente28++;
        break;
      case 29:
        servicioAgregado.Item = itemServicioDiente29;
        self.serviciosAgregadosDiente29.push(servicioAgregado);
        itemServicioDiente29++;
        break;
      case 30:
        servicioAgregado.Item = itemServicioDiente30;
        self.serviciosAgregadosDiente30.push(servicioAgregado);
        itemServicioDiente30++;
        break;
      case 31:
        servicioAgregado.Item = itemServicioDiente31;
        self.serviciosAgregadosDiente31.push(servicioAgregado);
        itemServicioDiente31++;
        break;
      case 32:
        servicioAgregado.Item = itemServicioDiente32;
        self.serviciosAgregadosDiente32.push(servicioAgregado);
        itemServicioDiente32++;
        break;
    }

    $(self.serviciosAgregadosConsulta()).each(function (idx, servicio) {
      if (
        servicio.ServicioId == servicioAgregado.ServicioId &&
        servicioAgregado.Aplicar
      ) {
        servicio.ServicioCantidad++;
      }
    });
    self.serviciosAgregadosConsulta.push(servicioAgregado);
    self.codigoServicioDentalBuscar("");

    self.actualizarTotales();
  };
  self.quitarServicioDiente1 = function (value) {
    $(self.serviciosAgregadosDiente1()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente1.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente2 = function (value) {
    $(self.serviciosAgregadosDiente2()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente2.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente3 = function (value) {
    $(self.serviciosAgregadosDiente3()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente3.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente4 = function (value) {
    $(self.serviciosAgregadosDiente4()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente4.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente5 = function (value) {
    $(self.serviciosAgregadosDiente5()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente5.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente6 = function (value) {
    $(self.serviciosAgregadosDiente6()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente6.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente7 = function (value) {
    $(self.serviciosAgregadosDiente7()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente7.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente8 = function (value) {
    $(self.serviciosAgregadosDiente8()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente8.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente9 = function (value) {
    $(self.serviciosAgregadosDiente9()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente9.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente10 = function (value) {
    $(self.serviciosAgregadosDiente10()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente10.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente11 = function (value) {
    $(self.serviciosAgregadosDiente11()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente11.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente12 = function (value) {
    $(self.serviciosAgregadosDiente12()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente12.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente13 = function (value) {
    $(self.serviciosAgregadosDiente13()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente13.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente14 = function (value) {
    $(self.serviciosAgregadosDiente14()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente14.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente15 = function (value) {
    $(self.serviciosAgregadosDiente15()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente15.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente16 = function (value) {
    $(self.serviciosAgregadosDiente16()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente16.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente17 = function (value) {
    $(self.serviciosAgregadosDiente17()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente17.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente18 = function (value) {
    $(self.serviciosAgregadosDiente18()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente18.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente19 = function (value) {
    $(self.serviciosAgregadosDiente19()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente19.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente20 = function (value) {
    $(self.serviciosAgregadosDiente20()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente20.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente21 = function (value) {
    $(self.serviciosAgregadosDiente21()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente21.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente22 = function (value) {
    $(self.serviciosAgregadosDiente22()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente22.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente23 = function (value) {
    $(self.serviciosAgregadosDiente23()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente23.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente24 = function (value) {
    $(self.serviciosAgregadosDiente24()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente24.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente25 = function (value) {
    $(self.serviciosAgregadosDiente25()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente25.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente26 = function (value) {
    $(self.serviciosAgregadosDiente26()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente26.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente27 = function (value) {
    $(self.serviciosAgregadosDiente27()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente27.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente28 = function (value) {
    $(self.serviciosAgregadosDiente28()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente28.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente29 = function (value) {
    $(self.serviciosAgregadosDiente29()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente29.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente30 = function (value) {
    $(self.serviciosAgregadosDiente30()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente30.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente31 = function (value) {
    $(self.serviciosAgregadosDiente31()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente31.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };
  self.quitarServicioDiente32 = function (value) {
    $(self.serviciosAgregadosDiente32()).each(function (idx, servicio) {
      if (value.Item == servicio.Item) {
        self.serviciosAgregadosDiente32.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };

  self.imprimirPresupuestoDental = function () {
    showLoading();
    self.getModel();
    $.ajax({
      url: "/Consultas/GuardarPresupuestoDental",
      method: "POST",
      data: model,
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          window.open(
            "/CrearPDF/PacientesPresupuestoDentalPDF?presupuestoId=" +
              data.Resultado,
            "_blank"
          );
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("ERROR INTERNO: " + dataError);
      },
    });
  };

  self.agregarSeguimientoNutricional = function () {
    var seguimiento = new Object();
    seguimiento.Fecha = new Date();
    seguimiento.Peso = null;
    seguimiento.IMC = null;
    seguimiento.PGC = null;
    seguimiento.Cuello = null;
    seguimiento.Busto = null;
    seguimiento.CinturaAbdomen = null;
    seguimiento.Cadera = null;
    seguimiento.Muslo = null;
    seguimiento.Brazo = null;
    seguimiento.Munneca = null;
    self.seguimientosNutricionales.push(seguimiento);
  };
  self.agregarExamenesLaboratorio = function () {
    var examen = new Object();
    examen.Id = null;
    examen.Item = itemExamenLaboratorio;
    examen.Fecha = moment().format("YYYY-MM-DD");
    examen.GlucosaPre = null;
    examen.GlucosaPos = null;
    examen.HemoglobinaGlicosilada = null;
    examen.CurvaGlucosa = null;
    examen.ColesterolTotal = null;
    examen.Trigliceridos = null;
    examen.PerfilLipidico = null;
    examen.Creatinina = null;
    examen.AcidoUrico = null;
    examen.Hemoglobina = null;
    examen.T3 = null;
    examen.T4 = null;
    examen.ExamenHeces = null;
    examen.ExamenOrina = null;
    examen.Otros = null;
    examen.UrlResultados = null;
    self.examenesLaboratorio.push(examen);
    itemExamenLaboratorio++;
  };
  self.quitarExamenLaboratorio = function (value) {
    $(self.examenesLaboratorio()).each(function (index, examen) {
      if (examen.Item == value.Item) {
        self.examenesLaboratorio.splice(index, 1);
      }
    });
  };
  self.agregarRangoSaludable = function () {
    var rango = new Object();
    rango.Id = null;
    rango.Item = itemRangoSaludable;
    rango.Fecha = moment().format("YYYY-MM-DD");
    rango.IMC = null;
    rango.Peso = null;
    rango.PorcentajeGrasaCorporal = null;
    self.rangosSaludables.push(rango);
    itemRangoSaludable++;
  };
  self.quitarRangoSaludable = function (value) {
    $(self.rangosSaludables()).each(function (index, rango) {
      if (value.Item == rango.Item) {
        self.rangosSaludables.splice(index, 1);
      }
    });
  };

  self.obtenerServiciosConsulta = function () {
    $(self.serviciosAgregadosDiente1()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente2()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente3()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente4()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente5()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente6()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente7()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente8()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente9()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente10()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente11()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente12()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente13()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente14()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente15()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente16()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente17()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente18()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente19()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente20()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente21()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente22()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente23()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente24()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente25()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente26()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente27()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente28()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente29()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente30()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente31()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
    $(self.serviciosAgregadosDiente32()).each(function (idx, vl) {
      self.serviciosAgregadosConsulta.push(vl);
    });
  };

  self.getModel = function () {
    model = {
      CitaId: $("#CitaId").val(),
      PacienteId: pacienteId,
      ConsultaId: $("#ConsultaId").val(),
      ConsultaMotivo: $("#ConsultaMotivo").val(),
      MedicoAsignado: $("#MedicoAsignado").val(),
      CaracteristicasDientes: self.caracteristicasDentales(),
      ServiciosAgregados: self.serviciosAgregadosConsulta(),
      SeguimientosNutricionales: self.seguimientosNutricionales(),
      RangosSaludables: self.rangosSaludables(),
      VacunasPaciente: self.vacunas(),
      AntecedentesFamiliaresPaciente: self.antecedentesFamiliares(),
      Cie10Codigo: $("#cie10-codigo").val(),

      //#region ANTECEDENTES PATOLOGICOS - ADULTO

      PacienteMedicos: $("#PacienteMedicos").val(),
      PacienteQuirurgicos: $("#PacienteQuirurgicos").val(),
      PacienteAlergias: $("#PacienteAlergias").val(),
      PacienteTraumaticos: $("#PacienteTraumaticos").val(),
      PacienteVicios: $("#PacienteVicios").val(),
      PacienteMedicamentos: $("#PacienteMedicamentos").val(),

      //#endregion

      //#region ANTECEDENTES PATOLOGICOS - PEDIATRICOS

      PediatricoAntMedicos: $("#PediatricoAntMedicos").val(),
      PediatricoAntQuirurgicos: $("#PediatricoAntQuirurgicos").val(),
      PediatricoAntTraumaticos: $("#PediatricoAntTraumaticos").val(),
      PediatricoAntAlergias: $("#PediatricoAntAlergias").val(),
      PediatricoAntVicios: $("#PediatricoAntVicios").val(),
      PediatricoAntMedicamentos: $("#PediatricoAntMedicamentos").val(),

      //#endregion

      //#region PACIENTE - APNP - NO PATOLOGICOS

      PacienteApnpGestas: $("#PacienteApnpGestas").val(),
      PacienteApnpPartos: $("#PacienteApnpPartos").val(),
      PacienteApnpAbortos: $("#PacienteApnpAbortos").val(),
      PacienteApnpCesareas: $("#PacienteApnpCesareas").val(),
      PacienteApnpMenarquia: $("#PacienteApnpMenarquia").val(),
      PacienteApnpHijosVivos: $("#PacienteApnpHijosVivos").val(),
      PacienteApnpHijosMuertos: $("#PacienteApnpHijosMuertos").val(),
      PacienteApnpFechaUltimaRegla: $("#PacienteApnpFechaUltimaRegla").val(),
      PacienteApnpFechaProbableParto: $(
        "#PacienteApnpFechaProbableParto"
      ).val(),
      PacienteApnpOtros: $("#PacienteApnpOtros").val(),

      //#endregion

      //#region PACIENTE PEDIATRICO - APNP - NO PATOLOGICOS

      PediatricoApnpParto: $("#PediatricoApnpParto").val(),
      PediatricoApnpAtendidoPor: $("#PediatricoApnpAtendidoPor").val(),
      PediatricoApnpPesoAlNacer: $("#PediatricoApnpPesoAlNacer").val(),
      PediatricoApnpInmunizaciones: $("#PediatricoApnpInmunizaciones").val(),
      PediatricoApnpGesta: $("#PediatricoApnpGesta").val(),

      //#endregion

      //ExamenesLaboratorio
      ExamenesLaboratorio: self.examenesLaboratorio(),

      ConsultaExamenArchivos: self.examenesArchivo(), //Para ver los examenes del archivo

      TipoConsulta: $("#TipoConsulta").val(),
      FaseTratamientoId: $("#FaseTratamientoId").val(),
      TipoReferencia: $("#TipoReferencia").val(),
      MedicoReferido: $("#MedicoReferido").val(),
      //"EstadoPagoId": $("#EstadoPagoId").val(),
      FechaYHoraInicio: $("#FechaYHoraInicio").val(),
      FechaProximaConsulta: $("#FechaProximaConsulta").val(),
      FechaUltimaRadiografiaDental: $("#FechaUltimaRadiografiaDental").val(),
      ObservacionesAdicionales: $("#ObservacionesAdicionales").val(),
      CostoConsulta: self.valorTotalConsulta(),
      CuentasPorCobrar: $("#CuentasPorCobrar").val(),
      BebeBebidasAlcoholicas: $(
        "input:radio[name=BebeBebidasAlcoholicas]:checked"
      ).val(),
      AlcoholUltimas24Horas: $("#AlcoholUltimas24Horas").val(),
      AlcoholSemanal: $("#AlcoholSemanal").val(),

      //#region Estetica

      Metabolismo: $("#Metabolismo").val(),
      Grasa: $("#Grasa").val(),
      IMC: $("#IMC").val(),
      Agua: $("#Agua").val(),
      Obesidad: $("#Obesidad").val(),
      ContornoBrazos: $("#ContornoBrazos").val(),
      ContornoBusto: $("#ContornoBusto").val(),
      ContornoAbdomen: $("#ContornoAbdomen").val(),
      ContornoCadera: $("#ContornoCadera").val(),
      ContornoPiernas: $("#ContornoPiernas").val(),
      Estatura: $("#Estatura").val(),

      //#endregion

      //#region EXAMEN FISICO
      ExamenFisicoEstadoGeneral: $("#ExamenFisicoEstadoGeneral").val(),
      ExamenFisicoTemperatura: $("#ExamenFisicoTemperatura").val(),
      ExamenFisicoFrecuenciaRespiratoria: $(
        "#ExamenFisicoFrecuenciaRespiratoria"
      ).val(),
      ExamenFisicoFrecuenciaCardiaca: $(
        "#ExamenFisicoFrecuenciaCardiaca"
      ).val(),
      ExamenFisicoSaturacionOxigeno: $("#ExamenFisicoSaturacionOxigeno").val(),
      PresionArterialBrazoDerecho: $("PresionArterialBrazoDerecho").val(),
      ExamenFisicoPresionArterialBrazoIzquierdo: $(
        "ExamenFisicoPresionArterialBrazoIzquierdo"
      ).val(),
      ExamenFisicoPresionArterial: $("#ExamenFisicoPresionArterial").val(),
      ExamenFisicoTensionArterial: $("#ExamenFisicoTensionArterial").val(),
      ExamenFisicoGlucosa: $("#ExamenFisicoGlucosa").val(),
      ExamenFisicoPeso: $("#ExamenFisicoPeso").val(),
      ExamenFisicoTalla: $("#ExamenFisicoTalla").val(),
      ExamenFisicoIMC: $("#ExamenFisicoIMC").val(),
      ExamenFisicoObservaciones: $("#ExamenFisicoObservaciones").val(),
      ExamenFisicoDerecho: $("#ExamenFisicoDerecho").val(),
      ExamenFisicoIzquierdo: $("#ExamenFisicoIzquierdo").val(),
      ExamenFisicoPresionArterialMedia: $(
        "#ExamenFisicoPresionArterialMedia"
      ).val(),
      ExamenFisicoGlasgow: $("#ExamenFisicoGlasgow").val(),
      //#endregion

      //#region EXAMEN FISICO - PEDIATRIA
      ExamenFisicoPediatriaEstadoGeneral: $(
        "#ExamenFisicoPediatriaEstadoGeneral"
      ).val(),
      ExamenFisicoPediatriaTemperatura: $(
        "#ExamenFisicoPediatriaTemperatura"
      ).val(),
      ExamenFisicoPediatriaFrecuenciaRespiratoria: $(
        "#ExamenFisicoPediatriaFrecuenciaRespiratoria"
      ).val(),
      ExamenFisicoPediatriaFrecuenciaCardiaca: $(
        "#ExamenFisicoPediatriaFrecuenciaCardiaca"
      ).val(),
      ExamenFisicoPediatriaSaturacionOxigeno: $(
        "#ExamenFisicoPediatriaSaturacionOxigeno"
      ).val(),
      ExamenFisicoPediatriaPresionArterialBrazoDerecho: $(
        "ExamenFisicoPediatriaPresionArterialBrazoDerecho"
      ).val(),
      ExamenFisicoPediatriaPresionArterialBrazoIzquierdo: $(
        "ExamenFisicoPediatriaPresionArterialBrazoIzquierdo"
      ).val(),
      ExamenFisicoPediatriaPresionArterial: $(
        "#ExamenFisicoPediatriaPresionArterial"
      ).val(),
      ExamenFisicoPediatriaTensionArterial: $(
        "#ExamenFisicoPediatriaTensionArterial"
      ).val(),
      ExamenFisicoPediatriaGlucosa: $("#ExamenFisicoPediatriaGlucosa").val(),
      ExamenFisicoPediatriaPeso: $("#ExamenFisicoPediatriaPeso").val(),
      ExamenFisicoPediatriaTalla: $("#ExamenFisicoPediatriaTalla").val(),
      ExamenFisicoPediatriaIMC: $("#ExamenFisicoPediatriaIMC").val(),
      ExamenFisicoPediatriaObservaciones: $(
        "#ExamenFisicoPediatriaObservaciones"
      ).val(),
      ExamenFisicoPediatriaDerecho: $("#ExamenFisicoPediatriaDerecho").val(),
      ExamenFisicoPediatriaIzquierdo: $(
        "#ExamenFisicoPediatriaIzquierdo"
      ).val(),
      ExamenFisicoPediatriaGlasgow: $("#ExamenFisicoPediatriaGlasgow").val(),
      ExamenFisicoPediatricoPesoTalla: $(
        "#ExamenFisicoPediatricoPesoTalla"
      ).val(),
      ExamenFisicoPediatricoPesoEdad: $(
        "#ExamenFisicoPediatricoPesoEdad"
      ).val(),
      ExamenFisicoPediatricoTallaEdad: $(
        "#ExamenFisicoPediatricoTallaEdad"
      ).val(),
      //#endregion

      //#region REVISION POR SISTEMAS

      RevisionSistemasAparienciaGeneral: $(
        "#RevisionSistemasAparienciaGeneral"
      ).val(),
      RevisionSistemasCabeza: $("#RevisionSistemasCabeza").val(),
      RevisionSistemasOidosBoca: $("#RevisionSistemasOidosBoca").val(),
      RevisionSistemasCuello: $("#RevisionSistemasCuello").val(),
      RevisionSistemasTorax: $("#RevisionSistemasTorax").val(),
      RevisionSistemasAbdomen: $("#RevisionSistemasAbdomen").val(),
      RevisionSistemasDorsoYExtremidades: $(
        "#RevisionSistemasDorsoYExtremidades"
      ).val(),
      RevisionSistemasGenitales: $("#RevisionSistemasGenitales").val(),

      NewRevisionSistemasNeurologico: $(
        "#NewRevisionSistemasNeurologico"
      ).val(),
      NewRevisionSistemasCardiovascular: $(
        "#NewRevisionSistemasCardiovascular"
      ).val(),
      NewRevisionSistemasRespiratorio: $(
        "#NewRevisionSistemasRespiratorio"
      ).val(),
      NewRevisionSistemasGastrointestinal: $(
        "#NewRevisionSistemasGastrointestinal"
      ).val(),
      NewRevisionSistemasMusculoesqueletico: $(
        "#NewRevisionSistemasMusculoesqueletico"
      ).val(),
      NewRevisionSistemasPielFanera: $("#NewRevisionSistemasPielFanera").val(),
      NewRevisionSistemasGenitourinario: $(
        "#NewRevisionSistemasGenitourinario"
      ).val(),
      //#endregion

      //#region REVISION POR SISTEMAS - PEDIATRIA

      RevisionSistemasPediatriaAparienciaGeneral: $(
        "#RevisionSistemasPediatriaAparienciaGeneral"
      ).val(),
      RevisionSistemasPediatriaCabeza: $(
        "#RevisionSistemasPediatriaCabeza"
      ).val(),
      RevisionSistemasPediatriaOidosBoca: $(
        "#RevisionSistemasPediatriaOidosBoca"
      ).val(),
      RevisionSistemasPediatriaCuello: $(
        "#RevisionSistemasPediatriaCuello"
      ).val(),
      RevisionSistemasPediatriaTorax: $(
        "#RevisionSistemasPediatriaTorax"
      ).val(),
      RevisionSistemasPediatriaAbdomen: $(
        "#RevisionSistemasPediatriaAbdomen"
      ).val(),
      RevisionSistemasPediatriaDorsoYExtremidades: $(
        "#RevisionSistemasPediatriaDorsoYExtremidades"
      ).val(),
      RevisionSistemasPediatriaGenitales: $(
        "#RevisionSistemasPediatriaGenitales"
      ).val(),

      //#endregion

      //#region GINECOLOGIA - ANT NO PATOLOGICOS
      GinecologiaConsultaMotivo: $("#GinecologiaConsultaMotivo").val(),

      GinecologiaAntNoPatologicosMenarquia: $(
        "#GinecologiaAntNoPatologicosMenarquia"
      ).val(),
      GinecologiaAntNoPatologicosFechaUltimaRegla: $(
        "#GinecologiaAntNoPatologicosFechaUltimaRegla"
      ).val(),
      GinecologiaAntNoPatologicosCicloMenstrual: $(
        "#GinecologiaAntNoPatologicosCicloMenstrual"
      ).val(),
      GinecologiaAntNoPatologicosMetodoAnticonceptivo: $(
        "#GinecologiaAntNoPatologicosMetodoAnticonceptivo"
      ).val(),
      GinecologiaAntNoPatologicosLactanciaMaterna: $(
        "#GinecologiaAntNoPatologicosLactanciaMaterna"
      ).val(),
      GinecologiaAntNoPatologicosGestas: $(
        "#GinecologiaAntNoPatologicosGestas"
      ).val(),
      GinecologiaAntNoPatologicosPartos: $(
        "#GinecologiaAntNoPatologicosPartos"
      ).val(),
      GinecologiaAntNoPatologicosAbortos: $(
        "#GinecologiaAntNoPatologicosAbortos"
      ).val(),
      GinecologiaAntNoPatologicosCesareas: $(
        "#GinecologiaAntNoPatologicosCesareas"
      ).val(),
      GinecologiaAntNoPatologicosHijosVivos: $(
        "#GinecologiaAntNoPatologicosHijosVivos"
      ).val(),
      GinecologiaAntNoPatologicosHijosMuertos: $(
        "#GinecologiaAntNoPatologicosHijosMuertos"
      ).val(),
      GinecologiaAntNoPatologicosOtros: $(
        "#GinecologiaAntNoPatologicosOtros"
      ).val(),
      //#endregion

      //#region OBSTETRICIA - ANT NO PATOLOGICOS

      ObstetriciaAntNoPatologicosGestas: $(
        "#ObstetriciaAntNoPatologicosGestas"
      ).val(),
      ObstetriciaAntNoPatologicosPartos: $(
        "#ObstetriciaAntNoPatologicosPartos"
      ).val(),
      ObstetriciaAntNoPatologicosNumeroParejas: $(
        "#ObstetriciaAntNoPatologicosNumeroParejas"
      ).val(),
      ObstetriciaAntNoPatologicosUltrasonido: $(
        "#ObstetriciaAntNoPatologicosUltrasonido"
      ).val(),
      ObstetriciaAntNoPatologicosCotarquia: $(
        "#ObstetriciaAntNoPatologicosCotarquia"
      ).val(),

      ObstetriciaAntNoPatologicosAbortos: $(
        "#ObstetriciaAntNoPatologicosAbortos"
      ).val(),
      ObstetriciaAntNoPatologicosCesareas: $(
        "#ObstetriciaAntNoPatologicosCesareas"
      ).val(),
      ObstetriciaAntNoPatologicosHijosVivos: $(
        "#ObstetriciaAntNoPatologicosHijosVivos"
      ).val(),
      ObstetriciaAntNoPatologicosHijosMuertos: $(
        "#ObstetriciaAntNoPatologicosHijosMuertos"
      ).val(),
      ObstetriciaAntNoPatologicosFechaUltimaRegla: $(
        "#ObstetriciaAntNoPatologicosFechaUltimaRegla"
      ).val(),
      ObstetriciaAntNoPatologicosFechaProbableParto: $(
        "#ObstetriciaAntNoPatologicosFechaProbableParto"
      ).val(),

      //#endregion

      //#region GINECOLOGIA - ANT PATOLOGICOS
      GinecologiaAntPatologicosInfecciones: $(
        "#GinecologiaAntPatologicosInfecciones"
      ).val(),
      GinecologiaAntPatologicosEts: $("#GinecologiaAntPatologicosEts").val(),
      GinecologiaAntPatologicosPapanicolau: $(
        "#GinecologiaAntPatologicosPapanicolau"
      ).val(),
      GinecologiaAntPatologicosOtros: $(
        "#GinecologiaAntPatologicosOtros"
      ).val(),
      //#endregion

      //#region GINECOLOGIA - EXAMEN FISICO
      GinecologiaExamenFisicoMamas: $("#GinecologiaExamenFisicoMamas").val(),
      GinecologiaExamenFisicoEspeculoscopia: $(
        "#GinecologiaExamenFisicoEspeculoscopia"
      ).val(),
      GinecologiaExamenFisicoTactoVaginal: $(
        "#GinecologiaExamenFisicoTactoVaginal"
      ).val(),
      GinecologiaExamenFisicoTactoRectal: $(
        "#GinecologiaExamenFisicoTactoRectal"
      ).val(),
      GinecologiaExamenFisicoVulvaVagina: $(
        "#GinecologiaExamenFisicoVulvaVagina"
      ).val(),
      //#endregion

      //#region HISTORIA CLINICA
      HistoriaProblema: $("#HistoriaProblema").val(),
      Sintomas: $("#Sintomas").val(),
      Diagnostico: $("#Diagnostico").val(),
      HistoriaEnfermedadActual: $("#HistoriaEnfermedadActual").val(),
      HistoriaClinicaImpresionClinica: $(
        "#HistoriaClinicaImpresionClinica"
      ).val(),
      HistoriaClinicaComentario: $("#HistoriaClinicaComentario").val(),
      //#endregion

      //#region HISTORIA CLINICA - PEDIATRIA

      HistoriaPediatriaConsultaMotivo: $(
        "#HistoriaPediatriaConsultaMotivo"
      ).val(),
      HistoriaPediatriaHistoriaProblema: $(
        "#HistoriaPediatriaHistoriaProblema"
      ).val(),
      HistoriaPediatriaSintomas: $("#HistoriaPediatriaSintomas").val(),
      HistoriaPediatriaHistoriaEnfermedadActual: $(
        "#HistoriaPediatriaHistoriaEnfermedadActual"
      ).val(),
      HistoriaPediatriaDiagnostico: $("#HistoriaPediatriaDiagnostico").val(),
      HistoriaPediatriaHistoriaClinicaComentario: $(
        "#HistoriaPediatriaHistoriaClinicaComentario"
      ).val(),
      HistoriaPediatriaHistoriaClinicaImpresionClinica: $(
        "#HistoriaPediatriaHistoriaClinicaImpresionClinica"
      ).val(),

      //#endregion

      //Mujeres
      EstaEmbarazada: $("#EstaEmbarazada").val(),
      NumeroSemanasEmbarazo: $("#NumeroSemanasEmbarazo").val(),
      TomaPildorasAnticonceptivas: $("#TomaPildorasAnticonceptivas").val(),
      EstaAmamantando: $("#EstaAmamantando").val(),

      //#region Area terapeutica

      TerapeuticoDatosGenerales: $("#TerapeuticoDatosGenerales").val(),
      TerapeuticoActividadesDiarias: $("#TerapeuticoActividadesDiarias").val(),
      TerapeuticoConQuienVive: $("#TerapeuticoConQuienVive").val(),
      TerapeuticoHabitosAlimenticios: $(
        "#TerapeuticoHabitosAlimenticios"
      ).val(),
      TerapeuticoEjercicio: $("#TerapeuticoEjercicio").val(),
      TerapeuticoFinesSemana: $("#TerapeuticoFinesSemana").val(),
      TerapeuticoHistoriaMedica: $("#TerapeuticoHistoriaMedica").val(),
      TerapeuticoHistoriaPeso: $("#TerapeuticoHistoriaPeso").val(),

      //#endregion

      //#region Información médica

      MedicaUsaLentesContacto: $("#MedicaUsaLentesContacto").val(),
      MedicaUsaLentesContactoDescripcion: $(
        "#MedicaUsaLentesContactoDescripcion"
      ).val(),
      MedicaArticulacionesArtificiales: $(
        "#MedicaArticulacionesArtificiales"
      ).val(),
      MedicaArticulacionesArtificialesFecha: $(
        "#MedicaArticulacionesArtificialesFecha"
      ).val(),
      MedicaArticulacionesArtificialesComplicaciones: $(
        "#MedicaArticulacionesArtificialesComplicaciones"
      ).val(),
      MedicaTomaAlendronato: $("#MedicaTomaAlendronato").val(),
      MedicaTomaAlendronatoFecha: $("#MedicaTomaAlendronatoFecha").val(),
      MedicaTratamientoDolorHuesos: $("#MedicaTratamientoDolorHuesos").val(),
      MedicaTratamientoDolorHuesosFechaInicio: $(
        "#MedicaTratamientoDolorHuesosFechaInicio"
      ).val(),
      MedicaTratamientoDolorHuesosDescripcionCaso: $(
        "#MedicaTratamientoDolorHuesosDescripcionCaso"
      ).val(),
      MedicaSustanciasReguladorasDrogas: $(
        "#MedicaSustanciasReguladorasDrogas"
      ).val(),
      MedicaSustanciasReguladorasDrogasFecha: $(
        "#MedicaSustanciasReguladorasDrogasFecha"
      ).val(),
      MedicaUsaTabaco: $("#MedicaUsaTabaco").val(),
      MedicaBebidasAlcoholicas: $("#MedicaBebidasAlcoholicas").val(),
      MedicaBebidasAlcoholicasDescripcion: $(
        "#MedicaBebidasAlcoholicasDescripcion"
      ).val(),

      //#endregion

      //#region Información dental

      DentalSangradoCepillar: $("#DentalSangradoCepillar").val(),
      DentalDolorFrio: $("#DentalDolorFrio").val(),
      DentalDolorPresionar: $("#DentalDolorPresionar").val(),
      DentalObjetosAtorados: $("#DentalObjetosAtorados").val(),
      DentalBocaSeca: $("#DentalBocaSeca").val(),
      DentalTratamientoPeriondal: $("#DentalTratamientoPeriondal").val(),
      DentalTratamientoOrtodoncia: $("#DentalTratamientoOrtodoncia").val(),
      DentalProblemasTratamientoDental: $(
        "#DentalProblemasTratamientoDental"
      ).val(),
      DentalProblemasTratamientoDentalDescripcion: $(
        "#DentalProblemasTratamientoDentalDescripcion"
      ).val(),
      DentalFluoradaAguaDomicilio: $("#DentalFluoradaAguaDomicilio").val(),
      DentalBebeAguaFiltrada: $("#DentalBebeAguaFiltrada").val(),
      DentalDolorOidos: $("#DentalDolorOidos").val(),
      DentalMolestiaRuidoAlto: $("#DentalMolestiaRuidoAlto").val(),
      DentalMolestiaRuidoAltoDescripcion: $(
        "#DentalMolestiaRuidoAltoDescripcion"
      ).val(),
      DentalBrumismo: $("#DentalBrumismo").val(),
      DentalLesiones: $("#DentalLesiones").val(),
      DentalLesionesDescripcion: $("#DentalLesionesDescripcion").val(),
      DentalDentaduraPlacas: $("#DentalDentaduraPlacas").val(),
      DentalDentaduraPlacasDescripcion: $(
        "#DentalDentaduraPlacasDescripcion"
      ).val(),
      DentalActividadesRecreacion: $("#DentalActividadesRecreacion").val(),
      DentalActividadesRecreacionDescripcion: $(
        "#DentalActividadesRecreacionDescripcion"
      ).val(),
      DentalLesionesCabeza: $("#DentalLesionesCabeza").val(),
      DentalLesionesCabezaDescripcion: $(
        "#DentalLesionesCabezaDescripcion"
      ).val(),

      //#endregion

      //Prescripcion medica
      ElementosPrescripcion: self.productosPrescripcion(),

      //#region ECOGRAFIA OBSTETRICA

      EcografiaObstetricaFeto: $("#EcografiaObstetricaFeto").val(),
      EcografiaObstetricaEstado: $("#EcografiaObstetricaEstado").val(),
      EcografiaObstetricaSituacion: $("#EcografiaObstetricaSituacion").val(),
      EcografiaObstetricaPresentacion: $(
        "#EcografiaObstetricaPresentacion"
      ).val(),
      EcografiaObstetricaPosicion: $("#EcografiaObstetricaPosicion").val(),
      EcografiaObstetricaDorso: $("#EcografiaObstetricaDorso").val(),

      //#endregion

      //#region ECOGRAFIA ENDOCAVITARIA

      EcografiaEndocavitariaUtero: $("#EcografiaEndocavitariaUtero").val(),
      EcografiaEndocavitariaLongitudinal: $(
        "#EcografiaEndocavitariaLongitudinal"
      ).val(),
      EcografiaEndocavitariaTransverso: $(
        "#EcografiaEndocavitariaTransverso"
      ).val(),
      EcografiaEndocavitariaEndometrio: $(
        "#EcografiaEndocavitariaEndometrio"
      ).val(),
      EcografiaEndocavitariaOvarioDerecho: $(
        "#EcografiaEndocavitariaOvarioDerecho"
      ).val(),
      EcografiaEndocavitariaOvarioIzquierdo: $(
        "#EcografiaEndocavitariaOvarioIzquierdo"
      ).val(),
      EcografiaEndocavitariaFondoSaco: $(
        "#EcografiaEndocavitariaFondoSaco"
      ).val(),
      EcografiaEndocavitariaImpresionClinica: $(
        "#EcografiaEndocavitariaImpresionClinica"
      ).val(),
      EcografiaEndocavitariaComentario: $(
        "#EcografiaEndocavitariaComentario"
      ).val(),

      //#endregion

      //#region OBSTETRICIA - BIOMETRIA
      NumeroBebes: parseInt($("#biometriaSelect").val()), // Asegúrate de que se envíe como número

      ObsteBiometriaRlc: $("#ObsteBiometriaRlc").val(),
      ObsteBiometriaRlc2: $("#ObsteBiometriaRlc2").val(),
      ObsteBiometriaRlc3: $("#ObsteBiometriaRlc3").val(),
      ObsteBiometriaRlc4: $("#ObsteBiometriaRlc4").val(),

      ObsteBiometriaSg: $("#ObsteBiometriaSg").val(),
      ObsteBiometriaSg2: $("#ObsteBiometriaSg2").val(),
      ObsteBiometriaSg3: $("#ObsteBiometriaSg3").val(),
      ObsteBiometriaSg4: $("#ObsteBiometriaSg4").val(),

      ObsteBiometriaDbp: $("#ObsteBiometriaDbp").val(),
      ObsteBiometriaDbp2: $("#ObsteBiometriaDbp2").val(),
      ObsteBiometriaDbp3: $("#ObsteBiometriaDbp3").val(),
      ObsteBiometriaDbp4: $("#ObsteBiometriaDbp4").val(),

      ObsteBiometriaLf: $("#ObsteBiometriaLf").val(),
      ObsteBiometriaLf2: $("#ObsteBiometriaLf2").val(),
      ObsteBiometriaLf3: $("#ObsteBiometriaLf3").val(),
      ObsteBiometriaLf4: $("#ObsteBiometriaLf4").val(),

      ObsteBiometriaAc: $("#ObsteBiometriaAc").val(),
      ObsteBiometriaAc2: $("#ObsteBiometriaAc2").val(),
      ObsteBiometriaAc3: $("#ObsteBiometriaAc3").val(),
      ObsteBiometriaAc4: $("#ObsteBiometriaAc4").val(),

      ObsteBiometriaHc: $("#ObsteBiometriaHc").val(),
      ObsteBiometriaHc2: $("#ObsteBiometriaHc2").val(),
      ObsteBiometriaHc3: $("#ObsteBiometriaHc3").val(),
      ObsteBiometriaHc4: $("#ObsteBiometriaHc4").val(),

      ObsteBiometriaFcf: $("#ObsteBiometriaFcf").val(),
      ObsteBiometriaFcf2: $("#ObsteBiometriaFcf2").val(),
      ObsteBiometriaFcf3: $("#ObsteBiometriaFcf3").val(),
      ObsteBiometriaFcf4: $("#ObsteBiometriaFcf4").val(),

      ObsteBiometriaIla: $("#ObsteBiometriaIla").val(),
      ObsteBiometriaIla2: $("#ObsteBiometriaIla2").val(),
      ObsteBiometriaIla3: $("#ObsteBiometriaIla3").val(),
      ObsteBiometriaIla4: $("#ObsteBiometriaIla4").val(),

      ObsteBiometriaPlacenta: $("#ObsteBiometriaPlacenta").val(),
      ObsteBiometriaPlacenta2: $("#ObsteBiometriaPlacenta2").val(),
      ObsteBiometriaPlacenta3: $("#ObsteBiometriaPlacenta3").val(),
      ObsteBiometriaPlacenta4: $("#ObsteBiometriaPlacenta4").val(),

      ObsteBiometriaGrado: $("#ObsteBiometriaGrado").val(),
      ObsteBiometriaGrado2: $("#ObsteBiometriaGrado2").val(),
      ObsteBiometriaGrado3: $("#ObsteBiometriaGrado3").val(),
      ObsteBiometriaGrado4: $("#ObsteBiometriaGrado4").val(),

      ObsteBiometriaMalformaciones: $("#ObsteBiometriaMalformaciones").val(),
      ObsteBiometriaMalformaciones2: $("#ObsteBiometriaMalformaciones2").val(),
      ObsteBiometriaMalformaciones3: $("#ObsteBiometriaMalformaciones3").val(),
      ObsteBiometriaMalformaciones4: $("#ObsteBiometriaMalformaciones4").val(),

      ObsteBiometriaPeso: $("#ObsteBiometriaPeso").val(),
      ObsteBiometriaPeso2: $("#ObsteBiometriaPeso2").val(),
      ObsteBiometriaPeso3: $("#ObsteBiometriaPeso3").val(),
      ObsteBiometriaPeso4: $("#ObsteBiometriaPeso4").val(),

      ObsteBiometriaEg: $("#ObsteBiometriaEg").val(),
      ObsteBiometriaEg2: $("#ObsteBiometriaEg2").val(),
      ObsteBiometriaEg3: $("#ObsteBiometriaEg3").val(),
      ObsteBiometriaEg4: $("#ObsteBiometriaEg4").val(),

      ObsteBiometriaFechaParto: $("#ObsteBiometriaFechaParto").val(),
      ObsteBiometriaFechaParto2: $("#ObsteBiometriaFechaParto2").val(),
      ObsteBiometriaFechaParto3: $("#ObsteBiometriaFechaParto3").val(),
      ObsteBiometriaFechaParto4: $("#ObsteBiometriaFechaParto4").val(),

      ObsteBiometriaSexo: $("#ObsteBiometriaSexo").val(),
      ObsteBiometriaSexo2: $("#ObsteBiometriaSexo2").val(),
      ObsteBiometriaSexo3: $("#ObsteBiometriaSexo3").val(),
      ObsteBiometriaSexo4: $("#ObsteBiometriaSexo4").val(),

      ObsteBiometriaPresentacion: $("#ObsteBiometriaPresentacion").val(),
      ObsteBiometriaPresentacion2: $("#ObsteBiometriaPresentacion2").val(),
      ObsteBiometriaPresentacion3: $("#ObsteBiometriaPresentacion3").val(),
      ObsteBiometriaPresentacion4: $("#ObsteBiometriaPresentacion4").val(),

      ObsteBiometriaComentario: $("#ObsteBiometriaComentario").val(),
      ObsteBiometriaComentario2: $("#ObsteBiometriaComentario2").val(),
      ObsteBiometriaComentario3: $("#ObsteBiometriaComentario3").val(),
      ObsteBiometriaComentario4: $("#ObsteBiometriaComentario4").val(),

      //#endregion

      //#region OBSTETRICIA - EVALUACION OBSTETRICA

      EvaluacionObstetricaUteroGravio: $(
        "#EvaluacionObstetricaUteroGravio"
      ).val(),
      EvaluacionObstetricaAbdomenObstetrico: $(
        "#EvaluacionObstetricaAbdomenObstetrico"
      ).val(),
      EvaluacionObstetricaFcf: $("#EvaluacionObstetricaFcf").val(),
      EvaluacionObstetricaAu: $("#EvaluacionObstetricaAu").val(),
      EvaluacionObstetricaBishop: $("#EvaluacionObstetricaBishop").val(),

      ConsistenciaGine: $("#ConsistenciaGine").val(),
      EvaluacionObstetricaPresentacionLeopold: $(
        "#EvaluacionObstetricaPresentacionLeopold"
      ).val(),
      EvaluacionObstetricaActividadUterina: $(
        "#EvaluacionObstetricaActividadUterina"
      ).val(),
      EvaluacionObstetricaMovimientoFetalPercetible: $(
        "#EvaluacionObstetricaMovimientoFetalPercetible"
      ).val(),
      EvaluacionObstetricaMovimientoFetalEspecifique: $(
        "#EvaluacionObstetricaMovimientoFetalEspecifique"
      ).val(),
      EvaluacionObstetricaTactoVaginal: $(
        "#EvaluacionObstetricaTactoVaginal"
      ).val(),
      EvaluacionObstetricaD: $("#EvaluacionObstetricaD").val(),
      EvaluacionObstetricaBPorciento: $(
        "#EvaluacionObstetricaBPorciento"
      ).val(),
      EvaluacionObstetricaAltitud: $("#EvaluacionObstetricaAltitud").val(),
      EvaluacionObstetricaPosicionCervix: $(
        "#EvaluacionObstetricaPosicionCervix"
      ).val(),
      EvaluacionObstetricaMembranasOvulares: $(
        "#EvaluacionObstetricaMembranasOvulares"
      ).val(),
      EvaluacionObstetricaLiquidoAmniotico: $(
        "#EvaluacionObstetricaLiquidoAmniotico"
      ).val(),
      EvaluacionObstetricaLiquidoAmnioticoEspecifique: $(
        "#EvaluacionObstetricaLiquidoAmnioticoEspecifique"
      ).val(),
      EvaluacionObstetricaPelvis: $("#EvaluacionObstetricaPelvis").val(),
      EvaluacionObstetricaConsistencia: $(
        "#EvaluacionObstetricaConsistencia"
      ).val(),
      EvaluacionObstetricaCms: $("#EvaluacionObstetricaCms").val(),

      //#endregion

      //nota Operatoria
      notaOperatoria: $("#notaOperatoria").val(),
      //turno
      EstadoTurno: $("#EstadoTurno").val(),

      //URlfiles
      UrlFiles: $("#UrlFiles").val(),

      //Datos Ginecologicos PRUEBA TECNICA ESTUARDO
      CicloMenstGine: $("#CicloMenstGine").val(),
      ETSGine: $("#ETSGine").val(),
      VIHGine: $("#VIHGine").val(),
      GrupoFactorGine: $("#GrupoFactorGine").val(),
      TorchGine: $("#TorchGine").val(),
      InicioVidaSexualGine: $("#InicioVidaSexualGine").val(),
      ParejasSexGine: $("#ParejasSexGine").val(),
      ObesidadGine: $("#ObesidadGine").val(),
      DesnutricionGine: $("#DesnutricionGine").val(),
      QGine: $("#QGine").val(),
      PGine: $("#PGine").val(),
      ABGine: $("#ABGine").val(),
      CGine: $("#CGine").val(),
      FURGine: $("#FURGine").val(),
      MuerteNeoGine: $("#MuerteNeoGine").val(),
      FPPGine: $("#FPPGine").val(),
      HVGine: $("#HVGine").val(),
      MuerteGine: $("#MuerteGine").val(),
      ControlPrenatalGine: $("#ControlPrenatalGine").val(),
      ComadronaGine: $("#ComadronaGine").val(),
      NoControlesGine: $("#NoControlesGine").val(),

      ExamenGinecologico: $("#ExamenGinecologico").val(),

      Productos: ko.toJS(self.productosPrescripcion()), // Esto convierte todos los productos, incluyendo UnidadMedidaVentaNombre
      Servicios: self.serviciosVenta(),
      ExamenesAgregados: self.examenesVenta(),
      MedicamentosOtros: ko.toJS(self.medicamentosOtros()),
      //#endregion OFTALMOLOGÍA
      // ==== OFTALMOLOGÍA ====
      Oft_ConsultaId: $("#Oft_ConsultaId").val() || $("#ConsultaId").val(),
      Oft_Id: $("#Oft_Id").val(),

      // Historia y antecedentes
      Oft_HistoriaEnfermedadActual: $("#Oft_HistoriaEnfermedadActual").val(),
      Oft_PacienteMedicos: $("#Oft_PacienteMedicos").val(),
      Oft_PacienteQuirurgicos: $("#Oft_PacienteQuirurgicos").val(),
      Oft_PacienteTraumaticos: $("#Oft_PacienteTraumaticos").val(),
      Oft_PacienteAlergias: $("#Oft_PacienteAlergias").val(),
      Oft_PacienteFamiliares: $("#Oft_PacienteFamiliares").val(),

      // Agudeza visual sin corrección
      Oft_AgudezaSC_Test: $("#Oft_AgudezaSC_Test").val(),
      Oft_AgudezaSC_OD: $("#Oft_AgudezaSC_OD").val(),
      Oft_AgudezaSC_OS: $("#Oft_AgudezaSC_OS").val(),

      // Contraste y AV cerca
      Oft_Contraste_OD: $("#Oft_Contraste_OD").val(),
      Oft_Contraste_OS: $("#Oft_Contraste_OS").val(),
      Oft_AVCerca_OD: $("#Oft_AVCerca_OD").val(),
      Oft_AVCerca_OS: $("#Oft_AVCerca_OS").val(),

      // Tests especiales
      Oft_TestIshihara_OD: $("#Oft_TestIshihara_OD").val(),
      Oft_TestIshihara_OS: $("#Oft_TestIshihara_OS").val(),
      Oft_TestEstereopsis_OD: $("#Oft_TestEstereopsis_OD").val(),
      Oft_TestEstereopsis_OS: $("#Oft_TestEstereopsis_OS").val(),

      // Lensometría (histórico)
      Oft_Lensometria_OD_Esfera: $("#Oft_Lensometria_OD_Esfera").val(),
      Oft_Lensometria_OD_Cilindro: $("#Oft_Lensometria_OD_Cilindro").val(),
      Oft_Lensometria_OD_Eje: $("#Oft_Lensometria_OD_Eje").val(),
      Oft_Lensometria_OD_Agudeza: $("#Oft_Lensometria_OD_Agudeza").val(),
      Oft_Lensometria_OS_Esfera: $("#Oft_Lensometria_OS_Esfera").val(),
      Oft_Lensometria_OS_Cilindro: $("#Oft_Lensometria_OS_Cilindro").val(),
      Oft_Lensometria_OS_Eje: $("#Oft_Lensometria_OS_Eje").val(),
      Oft_Lensometria_OS_Agudeza: $("#Oft_Lensometria_OS_Agudeza").val(),

      // Final
      Oft_Final_OD_Esfera: $("#Oft_Final_OD_Esfera").val(),
      Oft_Final_OD_Cilindro: $("#Oft_Final_OD_Cilindro").val(),
      Oft_Final_OD_Eje: $("#Oft_Final_OD_Eje").val(),
      Oft_Final_OD_Agudeza: $("#Oft_Final_OD_Agudeza").val(),
      Oft_Final_OS_Esfera: $("#Oft_Final_OS_Esfera").val(),
      Oft_Final_OS_Cilindro: $("#Oft_Final_OS_Cilindro").val(),
      Oft_Final_OS_Eje: $("#Oft_Final_OS_Eje").val(),
      Oft_Final_OS_Agudeza: $("#Oft_Final_OS_Agudeza").val(),
      Oft_Final_Adicion: $("#Oft_Final_Adicion").val(),
      Oft_Final_DIP_mm: $("#Oft_Final_DIP_mm").val(),

      // Retinoscopía
      Oft_Retino_OD_Esfera: $("#Oft_Retino_OD_Esfera").val(),
      Oft_Retino_OD_Cilindro: $("#Oft_Retino_OD_Cilindro").val(),
      Oft_Retino_OD_Eje: $("#Oft_Retino_OD_Eje").val(),
      Oft_Retino_OS_Esfera: $("#Oft_Retino_OS_Esfera").val(),
      Oft_Retino_OS_Cilindro: $("#Oft_Retino_OS_Cilindro").val(),
      Oft_Retino_OS_Eje: $("#Oft_Retino_OS_Eje").val(),

      // Tipo de lente
      Oft_TipoLente: $("input[name='Oft_TipoLente']:checked").val(),
      Oft_LenteMaterialTratamiento: $("#Oft_LenteMaterialTratamiento").val(),

      // Inspección / lámpara / oftalmoscopía (segmentos)
      Oft_Inspeccion_MovExtraoculares_OD: $(
        "#Oft_Inspeccion_MovExtraoculares_OD"
      ).val(),
      Oft_Inspeccion_MovExtraoculares_OS: $(
        "#Oft_Inspeccion_MovExtraoculares_OS"
      ).val(),
      Oft_Inspeccion_Cejas_OD: $("#Oft_Inspeccion_Cejas_OD").val(),
      Oft_Inspeccion_Cejas_OS: $("#Oft_Inspeccion_Cejas_OS").val(),
      Oft_Inspeccion_ParpadosPestanas_OD: $(
        "#Oft_Inspeccion_ParpadosPestanas_OD"
      ).val(),
      Oft_Inspeccion_ParpadosPestanas_OS: $(
        "#Oft_Inspeccion_ParpadosPestanas_OS"
      ).val(),
      Oft_Inspeccion_ViaLagrimal_OD: $("#Oft_Inspeccion_ViaLagrimal_OD").val(),
      Oft_Inspeccion_ViaLagrimal_OS: $("#Oft_Inspeccion_ViaLagrimal_OS").val(),

      Oft_Inspeccion_Conjuntiva_OD: $("#Oft_Inspeccion_Conjuntiva_OD").val(),
      Oft_Inspeccion_Conjuntiva_OS: $("#Oft_Inspeccion_Conjuntiva_OS").val(),
      Oft_Inspeccion_CorneaEsclera_OD: $(
        "#Oft_Inspeccion_CorneaEsclera_OD"
      ).val(),
      Oft_Inspeccion_CorneaEsclera_OS: $(
        "#Oft_Inspeccion_CorneaEsclera_OS"
      ).val(),
      Oft_Inspeccion_CamaraAnteriorAngulo_OD: $(
        "#Oft_Inspeccion_CamaraAnteriorAngulo_OD"
      ).val(),
      Oft_Inspeccion_CamaraAnteriorAngulo_OS: $(
        "#Oft_Inspeccion_CamaraAnteriorAngulo_OS"
      ).val(),
      Oft_Inspeccion_IrisPupila_OD: $("#Oft_Inspeccion_IrisPupila_OD").val(),
      Oft_Inspeccion_IrisPupila_OS: $("#Oft_Inspeccion_IrisPupila_OS").val(),
      Oft_Inspeccion_Cristalino_OD: $("#Oft_Inspeccion_Cristalino_OD").val(),
      Oft_Inspeccion_Cristalino_OS: $("#Oft_Inspeccion_Cristalino_OS").val(),
      Oft_Inspeccion_BUT_OD: $("#Oft_Inspeccion_BUT_OD").val(),
      Oft_Inspeccion_BUT_OS: $("#Oft_Inspeccion_BUT_OS").val(),
      Oft_Inspeccion_PresionIntraocular_OD: $(
        "#Oft_Inspeccion_PresionIntraocular_OD"
      ).val(),
      Oft_Inspeccion_PresionIntraocular_OS: $(
        "#Oft_Inspeccion_PresionIntraocular_OS"
      ).val(),

      Oft_Inspeccion_Vitreo_OD: $("#Oft_Inspeccion_Vitreo_OD").val(),
      Oft_Inspeccion_Vitreo_OS: $("#Oft_Inspeccion_Vitreo_OS").val(),
      Oft_Inspeccion_NervioOptico_OD: $(
        "#Oft_Inspeccion_NervioOptico_OD"
      ).val(),
      Oft_Inspeccion_NervioOptico_OS: $(
        "#Oft_Inspeccion_NervioOptico_OS"
      ).val(),
      Oft_Inspeccion_Macula_OD: $("#Oft_Inspeccion_Macula_OD").val(),
      Oft_Inspeccion_Macula_OS: $("#Oft_Inspeccion_Macula_OS").val(),
      Oft_Inspeccion_Retina_OD: $("#Oft_Inspeccion_Retina_OD").val(),
      Oft_Inspeccion_Retina_OS: $("#Oft_Inspeccion_Retina_OS").val(),

      Oft_HistoriaClinicaImpresionClinica: $(
        "#Oft_HistoriaClinicaImpresionClinica"
      ).val(),
      Oft_HistoriaClinicaComentario: $("#Oft_HistoriaClinicaComentario").val(),
      // ==== /OFTALMOLOGÍA ====
      //#endregion Obtener modelo

      //#REGION PODOLOGIA
      // ==== PODOLOGÍA ====
      Pod_ConsultaId: $("#Pod_ConsultaId").val() || $("#ConsultaId").val(),
      Pod_Id: $("#Pod_Id").val(),

      // 1) Antecedentes Médicos
      Pod_Enfermedades: $("input[name='Pod_Enfermedades']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Pod_Enfermedades_Otros: $("#Pod_Enfermedades_Otros").val(),
      Pod_Medicamentos: $("#Pod_Medicamentos").val(),
      Pod_PresionArterial: $("#Pod_PresionArterial").val(),

      // 2) Examen del Pie
      Pod_Pulso_Pedio: $("#Pod_Pulso_Pedio").val(),
      Pod_Pulso_TibialPosterior: $("#Pod_Pulso_TibialPosterior").val(),
      Pod_Pulso_Popliteo: $("#Pod_Pulso_Popliteo").val(),

      Pod_TemperaturaPie:
        $("input[name='Pod_TemperaturaPie']:checked").val() || null,
      Pod_ProblemasCirculatorios: (function () {
        const v = $("input[name='Pod_ProblemasCirculatorios']:checked").val();
        if (v == null) return null;
        if (/^(true|1|si|sí)$/i.test(v)) return true;
        if (/^(false|0|no)$/i.test(v)) return false;
        return null;
      })(),
      Pod_EstadoPiel: $("input[name='Pod_EstadoPiel']:checked").val() || null,

      Pod_ObservacionesExamen: $("#Pod_ObservacionesExamen").val(),

      // 3) Tratamiento Realizado
      Pod_Procedimientos: $("input[name='Pod_Procedimientos']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Pod_OtrosProcedimientos: $("#Pod_OtrosProcedimientos").val(),
      Pod_ObservacionesTratamiento: $("#Pod_ObservacionesTratamiento").val(),

      // 4) Indicaciones y Datos Finales
      Pod_Indicaciones: $("#Pod_Indicaciones").val(),
      Pod_PesoKg: $("#Pod_PesoKg").val(),
      Pod_EstaturaM: $("#Pod_EstaturaM").val(),
      Pod_FechaAtencion: $("#Pod_FechaAtencion").val(),
      Pod_Profesional: $("#Pod_Profesional").val(),
      // ==== /PODOLOGÍA ====
      //#endregion PODOLOGIA

      //#region HistoriaClinicaEnfermeria
      // ==== HISTORIA CLÍNICA DE ENFERMERÍA ====
      Hce_ConsultaId: $("#Hce_ConsultaId").val() || $("#ConsultaId").val(),
      Hce_Id: $("#Hce_Id").val(),
      Hce_PacienteId: $("#Hce_PacienteId").val() || $("#PacienteId").val(),

      // 2) Tipo de consulta
      Hce_TipoConsulta: $("#Hce_TipoConsulta").val(),

      // 3) Motivo de consulta
      Hce_MotivoConsulta: $("#Hce_MotivoConsulta").val(),

      // 4) Antecedentes personales y familiares
      Hce_AntecedentesPatologicos: $("#Hce_AntecedentesPatologicos").val(),
      Hce_AntecedentesQuirurgicos: $("#Hce_AntecedentesQuirurgicos").val(),
      Hce_AntecedentesTraumaticos: $("#Hce_AntecedentesTraumaticos").val(),
      Hce_Hospitalizaciones: $("#Hce_Hospitalizaciones").val(),
      Hce_Alergias: $("#Hce_Alergias").val(),
      Hce_AntecedentesFamiliares: $("#Hce_AntecedentesFamiliares").val(),

      // 5) Hábitos personales
      Hce_HabitoAlimentacion: $("#Hce_HabitoAlimentacion").val(),
      Hce_ActividadFisica: $("#Hce_ActividadFisica").val(),
      Hce_HabitoAlcoholTexto: $("#Hce_HabitoAlcoholTexto").val(),
      Hce_HabitoTabacoTexto: $("#Hce_HabitoTabacoTexto").val(),
      Hce_OtrosHabitos: $("#Hce_OtrosHabitos").val(),

      // 6) Signos vitales y antropometría
      Hce_PresionArterialTxt: $("#Hce_PresionArterialTxt").val(),
      Hce_FC: $("#Hce_FC").val(),
      Hce_FR: $("#Hce_FR").val(),
      Hce_TemperaturaC: $("#Hce_TemperaturaC").val(),
      Hce_SPO2: $("#Hce_SPO2").val(),
      Hce_PesoKg: $("#Hce_PesoKg").val(),
      Hce_TallaM: $("#Hce_TallaM").val(),
      Hce_IMC: $("#Hce_IMC").val(),

      // 7) Exploración física por aparatos y sistemas
      Hce_CabezaCuello: $("#Hce_CabezaCuello").val(),
      Hce_ToraxPulmones: $("#Hce_ToraxPulmones").val(),
      Hce_Corazon: $("#Hce_Corazon").val(),
      Hce_Abdomen: $("#Hce_Abdomen").val(),
      Hce_Extremidades: $("#Hce_Extremidades").val(),
      Hce_SistemaNeurologico: $("#Hce_SistemaNeurologico").val(),
      Hce_PielAnexos: $("#Hce_PielAnexos").val(),

      // 8) Valoración de enfermería
      Hce_ValConcienciaOrientacion: $("#Hce_ValConcienciaOrientacion").val(),
      Hce_ValEstadoNutricional: $("#Hce_ValEstadoNutricional").val(),
      Hce_ValEliminacion: $("#Hce_ValEliminacion").val(),
      Hce_ValSuenoDescanso: $("#Hce_ValSuenoDescanso").val(),
      Hce_ValActividadMovilidad: $("#Hce_ValActividadMovilidad").val(),
      Hce_ValAutonomia: $("#Hce_ValAutonomia").val(),

      // 9) Laboratorios
      Hce_Laboratorios: $("#Hce_Laboratorios").val(),

      // 10) Diagnóstico de enfermería
      Hce_DiagnosticoEnfermeria: $("#Hce_DiagnosticoEnfermeria").val(),

      // 11) Plan de cuidados / Intervenciones
      Hce_AccionesRealizadas: $("#Hce_AccionesRealizadas").val(),
      Hce_MedicamentosAdministrados: $("#Hce_MedicamentosAdministrados").val(),
      Hce_Tratamiento: $("#Hce_Tratamiento").val(),

      // 12) Seguimiento / Evolución / Cita
      Hce_Seguimiento: $("#Hce_Seguimiento").val(),
      // ==== /HISTORIA CLÍNICA DE ENFERMERÍA ====
      //#endregion HistoriaClinicaEnfermeria

      //#region VALORACIÓN DE ENFERMERÍA (VE)
      Ve_ConsultaId: $("#Ve_ConsultaId").val() || $("#ConsultaId").val(),
      Ve_Id: $("#Ve_Id").val(),

      // 1) Datos de valoración inicial
      Ve_Motivo: $("#Ve_Motivo").val(),
      Ve_DiagnosticoMedico: $("#Ve_DiagnosticoMedico").val(),
      Ve_Labs: $("#Ve_Labs").val(),

      // 2) ¿Cómo se enteró del servicio? (checkbox múltiple)
      Ve_Medio: $("input[name='Ve_Medio']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),

      // 3) Oxigenación y Circulación (checkbox múltiple)
      Ve_Resp: $("input[name='Ve_Resp']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Ve_Circ: $("input[name='Ve_Circ']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),

      // 4) Nutrición
      Ve_Nutricion: $("input[name='Ve_Nutricion']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Ve_NutricionObs: $("#Ve_NutricionObs").val(),

      // 5) Eliminación
      Ve_Urinario: $("input[name='Ve_Urinario']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Ve_Intestinal: $("input[name='Ve_Intestinal']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),

      // 6) Movilización y Estado de Conciencia
      Ve_Mov: $("input[name='Ve_Mov']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Ve_Conciencia: $("input[name='Ve_Conciencia']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),

      // 7) Autocuidado y Reposo
      Ve_Sueno: $("input[name='Ve_Sueno']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Ve_Vestirse: $("input[name='Ve_Vestirse']:checked").val() || null, // radio
      Ve_Higiene: $("input[name='Ve_Higiene']:checked").val() || null, // radio
      Ve_Piel: $("input[name='Ve_Piel']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Ve_PielUbicacion: $("#Ve_PielUbicacion").val(),

      // 8) Comunicación
      Ve_Lenguaje: $("input[name='Ve_Lenguaje']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Ve_Vision: $("input[name='Ve_Vision']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Ve_Oido: $("input[name='Ve_Oido']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),

      // 9) Seguridad y Psicosocial
      Ve_Seg: $("input[name='Ve_Seg']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Ve_Religiosos: $("input[name='Ve_Religiosos']:checked").val() || null, // radio
      Ve_CreenciasObservaciones: $("#Ve_CreenciasObservaciones").val(),
      Ve_ConoceMotivo: $("input[name='Ve_ConoceMotivo']:checked").val() || null, // radio
      Ve_NecesitaInfo: $("input[name='Ve_NecesitaInfo']:checked").val() || null, // radio

      // 10) Medicación y Plan
      Ve_MedicacionActual: $("#Ve_MedicacionActual").val(),
      Ve_PlanTerapeutico: $("#Ve_PlanTerapeutico").val(),

      //#region SUEROTERAPIA
      // ==== SUEROTERAPIA ====
      Suero_ConsultaId: $("#Suero_ConsultaId").val() || $("#ConsultaId").val(),
      Suero_Id: $("#Suero_Id").val(),

      // 1) Datos de valoración inicial
      Suero_Motivo: $("#Suero_Motivo").val(),
      Suero_DiagnosticoMedico: $("#Suero_DiagnosticoMedico").val(),
      Suero_Labs: $("#Suero_Labs").val(),

      // 2) ¿Cómo se enteró del servicio? (checkbox múltiple)
      Suero_Medio: $("input[name='Suero_Medio']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),

      // 3) Oxigenación y circulación (checkbox múltiple)
      Suero_Resp: $("input[name='Suero_Resp']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Suero_Circ: $("input[name='Suero_Circ']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),

      // 4) Necesidad de nutrición
      Suero_Nutricion: $("input[name='Suero_Nutricion']:checked")
        .map(function () {
          return $(this).val();
        })
        .get(),
      Suero_NutricionObs: $("#Suero_NutricionObs").val(),

      // 5) Plan
      Suero_PlanTerapeutico: $("#Suero_PlanTerapeutico").val(),
      // ==== /SUEROTERAPIA ====
      //#endregion SUEROTERAPIA
    };
  };

  self.registrarConsulta = function () {
    if (confirm("¿Desea guardar la consulta?")) {
      ////Guardar los archivos correspondientes
      //var formDataArchivos = new FormData();
      ////Resultados de laboratorios
      //$(self.examenesLaboratorio()).each(function (idx, examen) {
      //    if ($("#resultados-laboratorio-" + examen.Item).get(0).files[0] != null
      //        && $("#resultados-laboratorio-" + examen.Item).get(0).files[0] != undefined) {
      //        formDataArchivos.append("Archivos", $("#resultados-laboratorio-" + examen.Item).get(0).files[0], "ResultadosLaboratorio" + examen.Item);
      //    }
      //});

      showLoading();
      //$.ajax({
      //    type: "POST",
      //    url: "/Files/UploadFiles",
      //    dataType: "json",
      //    contentType: false,
      //    processData: false,
      //    data: formDataArchivos,
      //    success: function (data) {
      //        $(data.archivos).each(function (idx, archivo) {
      //            $(self.examenesLaboratorio()).each(function (idxExamen, examen) {
      //                if (examen.Item == parseInt(archivo.nombreArchivo.replace("ResultadosLaboratorio", ""))) {
      //                    examen.UrlResultados = archivo.urlArchivo;
      //                }
      //            });
      //        });
      //Registrar consulta
      self.obtenerServiciosConsulta();
      self.getModel();
      //self.agregarExamenesToPrescripcion(); //recorre prescripciones y examenes asignar para agregar a table y generar pdf

      $.ajax({
        method: "POST",
        url: "/Consultas/IniciarConsulta",
        data: model,
        success: function (data, textStatus) {
          var dataResult = JSON.parse(data);
          if (dataResult.Exitoso)
            window.location.href =
              "/Consultas/Informacion/" + dataResult.ConsultaId;
          else {
            hideLoading();
            alert(dataResult.Mensaje);
          }
        },
        error: function (data) {
          hideLoading();
          alert(data.error);
        },
      });
      //    }
      //});
    }
  };

  self.registrarConsultaYPagar = function () {
    //if (confirm("¿Desea Pagar la consulta?")) {
    self.txtModalConfirmacion("&iquest;Desea continuar con el Pago?");
    $("#mdl-confirmacion").dialog({
      modal: true,
      width: 800,
      buttons: [
        {
          text: "Si",
          class: "btn btn-success",
          click: function () {
            showLoading();
            self.obtenerServiciosConsulta();
            self.getModel();

            $.ajax({
              method: "POST",
              url: "/Consultas/IniciarConsultaYPagar",
              data: model,
              success: function (data, textStatus) {
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso)
                  window.location.href =
                    "/Consultas/GenerarVenta/" + dataResult.ConsultaId;
                else {
                  hideLoading();
                  console.log(dataResult.Mensaje);
                  mensajeEmergenteError(dataResult.Mensaje);
                }
              },
              error: function (data) {
                hideLoading();
                alert(data.error);
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
  };

  self.registrarConsultaYPagarCredito = function () {
    //if (confirm("¿Desea Pagar la consulta?")) {
    self.txtModalConfirmacion("&iquest;Desea continuar con el Pago?");
    $("#mdl-confirmacion").dialog({
      modal: true,
      width: 800,
      buttons: [
        {
          text: "Si",

          class: "btn btn-success",
          click: function () {
            self.obtenerServiciosConsulta();
            self.getModel();
            $.ajax({
              method: "POST",
              url: "/Consultas/IniciarConsultaYPagar",
              data: model,
              success: function (data, textStatus) {
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                  self.generarCuentaPendiente(model);
                } else {
                  hideLoading();
                  alert(dataResult.Mensaje);
                }
              },
              error: function (data) {
                hideLoading();
                alert(data.error);
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
  };

  //Crea cuentas pendientes cuando se quiere pagar una consulta a credito
  self.generarCuentaPendiente = function (model) {
    $.ajax({
      method: "POST",
      url: "/Consultas/GenerarCuentaPendiente",
      data: model,
      success: function (data, textStatus) {
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          console.log("Cuenta pendiente creada");
          window.location.href = "/Consultas";
        } else {
          hideLoading();
          alert(dataResult.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };

  self.consultarHabitacionesDisponibles = function () {
    let textoCargando = $("#text-cargando-habitaciones-disponibles");
    let textoError = $("#text-error-cargando-habitaciones-disponibles");
    textoError.hide();
    textoCargando.show();
    $.ajax({
      url: "/Consultas/ConsultarHabitacionesDisponibles",
      method: "POST",
      success: function (result) {
        textoCargando.hide();
        let data = JSON.parse(result);
        if (data.Exitoso) {
          self.habitacionesDisponibles(data.Resultado);
        } else {
          textoError.show();
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function (errorResult) {
        textoError.show();
        console.log(errorResult);
        mensajeEmergenteError("Error de servidor");
      },
    });
  };

  self.ocuparHabitacion = function (habitacion) {
    window.open(
      "/Hospitalizacion/Hospitalizar?habitacionId=" +
        habitacion.HabitacionId +
        "&consultaId=" +
        $("#ConsultaId").val(),
      "_blank"
    );
  };

  //registra los examenesfisicos de un pacientes antes de inicar consulta
  self.registrarExamenesFisicosCitasConsulta = function () {
    if (confirm("¿Desea registrar los Examenes Fisicos?")) {
      showLoading();
      //self.obtenerServiciosConsulta();
      self.getModel();

      $.ajax({
        method: "POST",
        url: "/Consultas/IniciarExamenesFisicosCitasConsulta",
        data: model,
        success: function (data, textStatus) {
          var dataResult = JSON.parse(data);
          if (dataResult.Exitoso) window.location.href = "/Cita/Lista";
          else {
            hideLoading();
            alert(dataResult.Mensaje);
          }
        },
        error: function (data) {
          hideLoading();
          alert(data.error);
        },
      });
    }
  };

  //#region Funciones Prescripcion medica
  self.agregarProducto = function () {
    let unidadSeleccionada = self.unidadVentaSeleccionadaProducto();
    let precioSeleccionado = self.precioSeleccionadoProducto();

    // Si la unidad no está en la lista y es un string, la agregamos
    if (unidadSeleccionada && typeof unidadSeleccionada === "string") {
      self.unidadesVentaProducto.push({
        UnidadMedidaVentaNombre: unidadSeleccionada,
      });
    }

    // Si el precio no está en la lista y es un string, lo agregamos
    if (precioSeleccionado && typeof precioSeleccionado === "string") {
      self.preciosProducto.push({ Precio: precioSeleccionado });
    }

    // Convertimos el precio y el valor de compra a números si es necesario
    let precioValor =
      parseFloat(
        precioSeleccionado != null && typeof precioSeleccionado === "object"
          ? precioSeleccionado.PrecioValor
          : precioSeleccionado
      ) || 0;

    let precioCompra =
      parseFloat(
        precioSeleccionado != null && typeof precioSeleccionado === "object"
          ? precioSeleccionado.PrecioCompra
          : 0
      ) || 0;

    // Creación del producto agregado
    let productoAgregado = {
      Color: self.Color(), // Aquí se almacena el color seleccionado
      ProductoCodigo: self.productoSeleccionado().ProductoCodigo ?? "N/A",
      ProductoId: self.productoSeleccionado().ProductoId,
      ProductoInventarioId: self.productoSeleccionado().ProductoInventarioId,
      ProductoNombre:
        self.productoSeleccionado().ProductoNombre ??
        self.productoSeleccionado(),
      UnidadMedidaVentaId:
        unidadSeleccionada != null && typeof unidadSeleccionada === "object"
          ? unidadSeleccionada.Id
          : null,
      UnidadMedidaVentaNombre:
        unidadSeleccionada != null && typeof unidadSeleccionada === "object"
          ? unidadSeleccionada.UnidadMedidaVentaNombre
          : unidadSeleccionada,

      Cantidad: ko.observable(1),
      ProductoIndicaciones: ko.observable(),
      ProductoPrecioId:
        precioSeleccionado != null && typeof precioSeleccionado === "object"
          ? precioSeleccionado.Id
          : null,
      Precio: ko.observable(
        precioSeleccionado != null && typeof precioSeleccionado === "object"
          ? precioSeleccionado.Precio
          : precioSeleccionado
      ),
      PrecioValor: ko.observable(precioValor),
      PrecioCompra: ko.observable(precioCompra),
      Subtotal: ko.observable(precioValor),
      DescuentoPorcentaje: ko.observable(0),
      DescuentoValor: ko.observable(0),
      ValorCubiertoSeguro: ko.observable(0),
      ValorCopago: ko.observable(precioValor),
      ValorTotal: ko.observable(precioValor),
      Nuevo: true,
      UsuarioAutoriza: ko.observable(""),
      Eliminado: ko.observable(false),
      Pagado: false,
    };

    // Agregar el producto a la lista observable
    self.productosPrescripcion.push(productoAgregado);
    mensajeEmergente("Producto agregado");
    self.actualizarTotales();
  };

  self.quitarProducto = function (elemento) {
    elemento.Eliminado(true);
  };

  self.generarPdfPrescripcion = function () {
    if (self.elementosPrescripcion().length == 0) {
      alert("No hay elementos agregados");
      return;
    }

    showLoading();
    $.ajax({
      url: "/Consultas/GenerarPrescripcion",
      method: "POST",
      data: {
        citaId: $("#CitaId").val(),
        elementosPrescripcion: self.elementosPrescripcion(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          let prescripcionId = data.Resultado;
          window.open(
            "/CrearPDF/PrescripcionPDF?prescripcionId=" + prescripcionId,
            "_blank"
          );
        } else {
          alert(data.Mensaje);
        }
      },
    });
  };

  // self.consultarPrescripcion = function () {
  //     showLoading();
  //     self.productosPrescripcion([]);
  //     $.ajax({
  //         url: "/Consultas/ConsultarPrescripcionCita",
  //         data: {
  //             citaId: $("#CitaId").val()
  //         },
  //         method: "POST",
  //         success: function (dataResult) {
  //             hideLoading();
  //             let data = JSON.parse(dataResult);
  //             if (data.Exitoso) {
  //                 $(data.Resultado).each(function (idx, elemento) {
  //                     elemento.Cantidad = ko.observable(elemento.Cantidad);
  //                     elemento.Eliminado = ko.observable(false);
  //                     elemento.PrecioValor = ko.observable(elemento.PrecioValor);
  //                     elemento.ValorCubiertoSeguro = ko.observable(elemento.ValorCubiertoSeguro);
  //                     elemento.ValorCopago = ko.observable(elemento.ValorCopago);
  //                     elemento.ValorTotal = ko.observable(0);
  //                     elemento.Subtotal = ko.observable(0);
  //                     elemento.Precio = elemento.Precio == null || elemento.Precio.trim() == ""
  //                         ? "Normal"
  //                         : elemento.Precio;
  //                     self.productosPrescripcion.push(elemento);
  //                 });
  //                 self.actualizarTotales();
  //             } else {
  //                 console.log(data.Mensaje);
  //             }
  //         }
  //     });
  // };

  self.consultarPrescripcion = function () {
    showLoading();
    self.productosPrescripcion([]);
    $.ajax({
      url: "/Consultas/ConsultarPrescripcionesPaciente",
      data: {
        pacienteId: $("#PacienteId").val(),
        citaId: $("#CitaId").val(),
      },
      method: "POST",
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          data.Resultado.sort((a, b) => {
            const getOrder = (x) => {
              if (!x.Another) return 0; // Actual
              if (x.Another && !x.Pagado) return 1; // Antigua no pagada
              return 2; // Antigua pagada
            };
            return getOrder(a) - getOrder(b);
          });

          data.Resultado.forEach(function (elemento) {
            elemento.Cantidad = ko.observable(elemento.Cantidad);
            elemento.Eliminado = ko.observable(false);
            elemento.PrecioValor = ko.observable(elemento.PrecioValor);
            elemento.ValorCubiertoSeguro = ko.observable(
              elemento.ValorCubiertoSeguro
            );
            elemento.ValorCopago = ko.observable(elemento.ValorCopago);
            elemento.ValorTotal = ko.observable(elemento.ValorTotal || 0);
            elemento.Subtotal = ko.observable(0);
            elemento.Precio =
              elemento.Precio == null || elemento.Precio.trim() == ""
                ? "Normal"
                : elemento.Precio;
            self.productosPrescripcion.push(elemento);
          });

          self.actualizarTotales();

          console.log(`Total de prescripciones: ${data.TotalPrescripciones}`);
          console.log(`Total de detalles cargados: ${data.TotalDetalles}`);
        } else {
          console.log(data.Mensaje);
        }
      },
      error: function (xhr, status, error) {
        hideLoading();
        console.error("Error en la consulta:", error);
      },
    });
  };

  //#endregion

  self.consultarProductosExistentes = function () {
    let textoCargando = $("#texto-cargando-productos-existentes");
    let textoError = $("#texto-error-consultar-productos-existentes");
    self.productosExistentes([]);
    textoCargando.show();
    textoError.hide();
    $.ajax({
      method: "POST",
      url: "/Consultas/ConsultarProductosExistentes",
      success: function (dataResult) {
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.registrosInventario(data.Resultado);
          let productoIds = new Set();
          $(self.registrosInventario()).each(function (idx, vl) {
            productoIds.add(vl.ProductoId);
          });
          for (let id of productoIds) {
            let agregado = false;
            $(self.registrosInventario()).each(function (idx2, vl2) {
              if (!agregado && id == vl2.ProductoId) {
                let productoExistente = {
                  ProductoId: vl2.ProductoId,
                  ProductoInventarioId: vl2.ProductoInventarioId,
                  ProductoCodigo: vl2.ProductoCodigo,
                  ProductoNombre: vl2.ProductoNombre,
                  ProductoNombreMostrar:
                    vl2.ProductoNombre +
                    " - Activo y concentracion: " +
                    vl2.ProductoActivoConcentracion,
                };
                self.productosExistentes.push(productoExistente);
                agregado = true;
              }
            });
          }
          textoCargando.hide();
        } else {
          textoCargando.hide();
          textoError.show();
        }
      },
      error: function (data) {
        textoCargando.show();
        textoError.show();
      },
    });
  };
  self.consultarProductosExistentesConStock = function () {
    let textoCargando = $("#texto-cargando-productos-existentes");
    let textoError = $("#texto-error-consultar-productos-existentes");
    self.productosExistentes([]);
    textoCargando.show();
    textoError.hide();

    // Define el BodegaId específico que deseas filtrar (Farmacia en este caso)
    const bodegaFarmaciaId = 1;

    $.ajax({
      method: "POST",
      url: "/Consultas/ConsultarProductosExistentes",
      success: function (dataResult) {
        let data = JSON.parse(dataResult);

        if (data.Exitoso) {
          self.registrosInventario(data.Resultado);

          // Elimina duplicados por ProductoInventarioId para evitar sumar stock repetido
          let registrosUnicos = {};
          $(self.registrosInventario()).each(function (idx, vl) {
            if (vl.BodegaId === bodegaFarmaciaId) {
              // if (vl.ProductoNombre === "ZELIC - PENTOXIFILINA - 400MG - ORAL - TABLETAS") {
              //     console.log(`ProductoId: ${vl.ProductoId}, ProductoInventarioId: ${vl.ProductoInventarioId}, Stock: ${vl.Stock}`);
              // }
              registrosUnicos[vl.ProductoInventarioId] = vl; // guarda solo un registro por ProductoInventarioId
            }
          });

          let productoIds = new Set();
          let stockPorProducto = {};

          // Suma stock sin duplicados
          for (const key in registrosUnicos) {
            let vl = registrosUnicos[key];
            productoIds.add(vl.ProductoId);
            if (!stockPorProducto[vl.ProductoId]) {
              stockPorProducto[vl.ProductoId] = 0;
            }
            stockPorProducto[vl.ProductoId] += vl.Stock;
          }

          // Construye lista de productos existentes con stock
          for (let id of productoIds) {
            let agregado = false;
            $(self.registrosInventario()).each(function (idx2, vl2) {
              if (
                !agregado &&
                id == vl2.ProductoId &&
                vl2.BodegaId === bodegaFarmaciaId &&
                stockPorProducto[vl2.ProductoId] > 0
              ) {
                // console.log(`Sumando stock para ProductoId ${vl2.ProductoId}: ${stockPorProducto[vl2.ProductoId]}`);
                let productoExistente = {
                  ProductoId: vl2.ProductoId,
                  ProductoInventarioId: vl2.ProductoInventarioId,
                  ProductoCodigo: vl2.ProductoCodigo,
                  ProductoNombre: vl2.ProductoNombre,
                  ProductoNombreMostrar:
                    vl2.ProductoNombre +
                    " - Stock Total: " +
                    stockPorProducto[vl2.ProductoId],
                };
                self.productosExistentes.push(productoExistente);
                agregado = true;
              }
            });
          }

          textoCargando.hide();
        } else {
          textoCargando.hide();
          textoError.show();
        }
      },
      error: function (data) {
        textoCargando.show();
        textoError.show();
      },
    });
  };

  self.consultarUnidadesVentaProducto = function (producto) {
    if (
      !self.productoSeleccionado() ||
      producto == null ||
      producto == undefined ||
      producto.ProductoId == null ||
      producto.ProductoId == undefined
    ) {
      //mensajeEmergenteError("No hay ningun producto valido seleccionado");
      self.unidadesVentaProducto([]);
      return false;
    }
    let productoId = producto.ProductoId;
    self.unidadesVentaProducto([]);
    let registrosInventarioProducto = new Array();
    $(self.registrosInventario()).each(function (idx, registro) {
      if (registro.ProductoId == productoId) {
        registrosInventarioProducto.push(registro);
      }
    });

    let unidadesVentaIds = new Set();
    $(registrosInventarioProducto).each(function (idx, registro) {
      if (
        registro.UnidadMedidaVentaId != null &&
        registro.UnidadMedidaVentaId != undefined
      ) {
        unidadesVentaIds.add(registro.UnidadMedidaVentaId);
      }
    });

    for (let unidadVentaId of unidadesVentaIds) {
      let agregado = false;
      //let unidades = new Array();
      $(registrosInventarioProducto).each(function (idx, vl) {
        if (vl.UnidadMedidaVentaId == unidadVentaId && !agregado) {
          let unidadAgregada = {
            Id: unidadVentaId,
            UnidadMedidaVentaNombre: vl.UnidadMedidaVentaNombre,
          };
          self.unidadesVentaProducto.push(unidadAgregada);
          agregado = true;
        }
      });
    }
  };

  self.consultarPreciosProducto = function (unidadSeleccionada) {
    if (unidadSeleccionada == null || unidadSeleccionada == undefined) {
      self.preciosProducto([]);
      return;
    }

    if (
      !self.productoSeleccionado() ||
      self.productoSeleccionado() == null ||
      self.productoSeleccionado() == undefined
    ) {
      mensajeEmergenteError("No hay ningun producto valido seleccionado");
      return;
    }

    self.preciosProducto([]);
    let registrosInventarioProducto = [];

    $(self.registrosInventario()).each(function (idx, registro) {
      if (
        registro.ProductoId == self.productoSeleccionado().ProductoId &&
        registro.UnidadMedidaVentaId == unidadSeleccionada.Id &&
        registro.PrecioValor > 0
      ) {
        registrosInventarioProducto.push(registro);
      }
    });

    let preciosIds = new Set();
    $(registrosInventarioProducto).each(function (idx, registro) {
      if (registro.PrecioId != null && registro.PrecioId != undefined) {
        preciosIds.add(registro.PrecioId);
      }
    });

    for (let precioId of preciosIds) {
      let precios = [];
      $(registrosInventarioProducto).each(function (idx, vl) {
        if (vl.PrecioId == precioId && vl.PrecioValor > 0) {
          let precioAgregado = {
            ProductoInventarioId: vl.ProductoInventarioId,
            Id: precioId,
            Precio: vl.PrecioNombre + " (Q " + vl.PrecioValor + ")",
            PrecioValor: vl.PrecioValor,
            PrecioCompra: vl.PrecioCompra,
            PrecioNombre: vl.PrecioNombre,
          };
          precios.push(precioAgregado);
        }
      });

      let productoInventarioIds = precios.map((p) => p.ProductoInventarioId);
      let ultimoProductoInventarioId = Math.max(...productoInventarioIds);

      precios = precios.filter(
        (p) => p.ProductoInventarioId === ultimoProductoInventarioId
      );

      // Ordenar para que "NORMAL" quede primero
      precios.sort((a, b) => {
        if (a.PrecioNombre === "NORMAL") return -1;
        if (b.PrecioNombre === "NORMAL") return 1;
        return 0;
      });

      precios.forEach((p) => self.preciosProducto.push(p));
    }
  };

  self.productoSeleccionado.subscribe(function (value) {
    self.consultarUnidadesVentaProducto(value);
  });
  self.unidadVentaSeleccionadaProducto.subscribe(function (unidadSeleccionada) {
    self.consultarPreciosProducto(unidadSeleccionada);
  });

  self.editarConsulta = function () {
    showLoading();
    self.obtenerServiciosConsulta();
    console.log(
      "Medicamentos antes de getModel:",
      ko.toJS(self.medicamentosOtros())
    );
    self.getModel();

    $.ajax({
      method: "POST",
      url: "/Consultas/EditarConsulta",
      data: model,
      success: function (data, textStatus) {
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso)
          // console.log(model.MedicamentosOtros)
          window.location.href =
            "/Consultas/Informacion/" + dataResult.ConsultaId;
        else {
          hideLoading();
          alert(dataResult.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };

  self.actualizarVista = function () {
    //Inicia Cálculo de IMC
    var peso = 0.0;
    var estatura = 0.0;
    try {
      peso = parseFloat($("#Peso").val());
    } catch {
      peso = 0.0;
    }
    try {
      estatura = parseFloat($("#Estatura").val());
    } catch {
      estatura = 0.0;
    }
    var imc = 0.0;
    if (estatura > 0 && peso > 0) {
      imc = peso / (estatura * estatura);
      $("#IMC").val(imc.toString() + " kg/m2");
    } else {
      $("#IMC").val("");
    }
    //Finaliza cálculo de IMC
  };

  self.actualizarTotales = function () {
    let subtotal = 0;
    //let descuento = 0;
    let total = 0;

    let subtotalExamenes = 0;
    let totalExamenes = 0;
    $(self.examenesVenta()).each(function (idx, examen) {
      let subtotalExamen = examen.Cantidad() * examen.ValorUnitario;
      //let descuentoValor = subtotalExamen * (examen.DescuentoPorcentaje / 100);
      let totalExamen = subtotalExamen;
      examen.ValorSubtotal(subtotalExamen);
      examen.ValorTotal(totalExamen);

      subtotalExamenes += subtotalExamen;
      subtotal += subtotalExamen;
      //descuento += descuentoValoar;
      totalExamenes += totalExamen;
      total += totalExamen;
    });
    self.valorTotalExamenes(totalExamenes);
    self.valorSubtotalExamenes(subtotalExamenes);

    //PRODUCTOS PRESCRIPCION
    $(self.productosPrescripcion()).each(function (idx, producto) {
      let subtotalProducto = producto.Cantidad() * producto.PrecioValor();
      //let descuentoValor = subtotalExamen * (examen.DescuentoPorcentaje / 100);
      let totalProducto = subtotalProducto;
      producto.Subtotal(subtotalProducto);
      producto.ValorTotal(totalProducto);
      let valorCubiertoSeguro = producto.ValorCubiertoSeguro();
      let valorCopago = totalProducto - valorCubiertoSeguro;
      producto.ValorCopago(valorCopago);
    });

    self.valorTotalConsulta(total);
  };

  self.consultarExamenesAgregadosCita = function () {
    let consultaId = $("#ConsultaId").val();
    let citaId = $("#CitaId").val();
    if (consultaId == null || consultaId.trim() == "") {
      $.ajax({
        method: "POST",
        url: "/Consultas/ConsultarExamenesAgregadosCita",
        data: {
          citaId: $("#CitaId").val(),
        },
        success: function (data, textStatus) {
          hideLoading();
          var dataResult = JSON.parse(data);
          if (dataResult.Exitoso) {
            $(dataResult.Resultado).each(function (idx, value) {
              value.Item = itemExamenAgregado;
              value.ExamenId = value.Id;
              value.ExamenNombre = value.NombreExamen;
              value.ValorSubtotal = ko.observable(
                value.PrecioValor * value.Cantidad
              );
              value.Cantidad = ko.observable(value.Cantidad);
              value.Pagado = false;
              value.ValorTotal = ko.observable(
                value.PrecioValor * value.Cantidad
              );
              value.ValorUnitario = ko.observable(value.PrecioValor);
              self.examenesVenta.push(value);
              itemExamenAgregado++;
            });
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (data) {
          hideLoading();
          alert("ERROR DE LLAMADO ASINCRONO: " + data.error);
        },
      });
    }
  };

  //Metodo para consultar las preguntas de los examenes pertenecientes ala cita
  self.consultarPreguntasExamen = function () {
    /*showLoading();*/
    $.ajax({
      method: "POST",
      url: "/LaboratorioClinico/ConsultarPreguntasExamenCita",
      data: {
        citaId: $("#CitaId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.examenesPreguntas(data.Resultado);
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
  //Metodo de de selecion con la respuesta
  self.respuestaPregunta = function (data, event) {
    filaSeleccionada = $(this).closest("tr");
    console.log(data);
    console.log(filaSeleccionada + "esta es");
    $.ajax({
      method: "POST",
      url: "/LaboratorioClinico/ModificarPreguntasExamen",
      data: {
        data,
      },
      success: function (dataResult) {
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.examenesPreguntas(data.Resultado);
          self.consultarPreguntasExamen();
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        //hideLoading();
        alert(data.error);
      },
    });
  };
  //Metodos para subir archivos de examenes alas consultas
  //Subir archivo
  self.agregarArchivoExamen = function (rutaArchivo) {
    if (
      self.nombreNuevoArchivo() != null &&
      self.nombreNuevoArchivo() != undefined
    ) {
      let archivoExamenAgregado = {
        NombreArchivo: self.nombreNuevoArchivo(),
        UrlArchivo: rutaArchivo,
      };
      self.examenesArchivo.push(archivoExamenAgregado); // para dibujar la tabla
    } else {
      alert("No hay ningun Archivos seleccionado");
    }
  };

  self.registrarArchivoExamen = function (rutaArchivo) {
    showLoading();

    $.ajax({
      method: "POST",
      url: "/Consultas/RegistrarArchivo",
      data: {
        consultaId: $("#ConsultaId").val(),
        rutaArchivo: rutaArchivo,
        nombreArchivo: self.nombreNuevoArchivo(),
      },
      success: function (data) {
        if (data.exitoso) {
          self.agregarArchivoExamen();
          window.location.reload();
        } else {
          hideLoading();
          alert(data.mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };
  self.consultarArchivosConsulta = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Consultas/ConsultarArchivosConsulta",
      data: {
        pacienteId: $("#PacienteId").val(),
      },
      success: function (data, textStatus) {
        hideLoading();
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.archivosConsulta(dataResult.Resultado);
        } else alert(dataResult.Mensaje);
      },
      error: function (data) {
        alert(data.error);
      },
    });
  };
  self.subirArchivoConsulta = function () {
    var formData = new FormData();
    var fileInput = document.getElementById("archivo-cargar");

    if (fileInput.files.length == 0) {
      alert("No hay ningun archivo cargado");
    } else {
      if (confirm("¿Desea cargar este archivo?")) {
        showLoading();
        var file = fileInput.files[0];
        formData.append("file", file);
        formData.append("consultaId", $("#ConsultaId").val());

        $.ajax({
          url: "/Files/UploadFile",
          type: "POST",
          data: formData,
          contentType: false,
          processData: false,
          success: function (response) {
            let data = JSON.parse(response);
            if (data.Exitoso) {
              mensajeEmergente("Archivo cargado exitosamente");
              self.consultarArchivosConsulta();
            } else {
              hideLoading();
              mensajeEmergenteError(data.Mensaje);
            }
          },
          error: function (jqXHR, textStatus, errorThrown) {
            hideLoading();
            alert("Error al subir el archivo: " + textStatus);
          },
        });
      }
    }
  };
  self.eliminarArchivoConsulta = function (archivo) {
    if (confirm("¿Desea eliminar este archivo?")) {
      showLoading();
      $.ajax({
        url: "/Consultas/EliminarArchivoConsulta",
        method: "POST",
        data: {
          archivoId: archivo.Id,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          alert(data.Mensaje);
          if (data.Exitoso) {
            $(self.archivosConsulta()).each(function (idx, vl) {
              if (vl.ArchivoId == archivo.ArchivoId) {
                self.archivosConsulta.splice(idx, 1);
              }
            });
          }
        },
        error: function (errorData) {
          hideLoading();
          alert("Error de servidor. COmuniquese con servicio tecnico");
        },
      });
    }
  };
  self.subirArchivoExamen = function () {
    if (
      self.nombreNuevoArchivo() == undefined ||
      self.nombreNuevoArchivo() == null
    ) {
      alert("Asegúrese de proporcionar el nombre del archivo");
    } else {
      showLoading();

      $.ajax({
        method: "POST",
        url: "/Files/SubirArchivo",
        data: {
          base64Archivo: $("#base-64-nuevo-archivo").val(),
          extension:
            "." +
            document
              .querySelector("#nuevo-archivo")
              .files[0].name.split(".")[1],
        },
        success: function (data) {
          hideLoading();
          if (data.exitoso) {
            $("#base-64-nuevo-archivo").val("");
            self.agregarArchivoExamen(data.url);
            //self.registrarArchivoExamen(data.url);
          } else {
            alert(data.mensaje);
          }
        },
        error: function (data) {
          hideLoading();
          alert(data.error);
        },
      });
    }
  };
};

var consultaVm = new ConsultaVM();
ko.applyBindings(consultaVm);

$(function () {
  contraerMenu();

  $("#tabs").tabs();

  consultaVm.cargarContadoresCita();
  //consultaVm.consultarServiciosAgregadosConsulta();
  consultaVm.consultarCaracteristicasDentales();

  //Consulta de servicios existentes en Base de datos
  consultaVm.consultarServicios();

  consultaVm.consultarVacunasPaciente();
  consultaVm.consultarArchivosExamen();
  consultaVm.consultarAntecedentesFamiliaresPaciente();
  consultaVm.consultarSeguimientosNutricionales();
  drawDataTable("tabla-informacion-medica");
  drawDataTable("tabla-informacion-dental");

  //Consulta de EXAMENES existentes en BD
  consultaVm.consultarExamenesExistentes();

  //Consulta de SERVICIOS agregados cuando ya es una CONSULTA
  consultaVm.consultarServiciosAgregadosConsulta();

  //Consulta de EXAMENES agregados cuando ya es una CONSULTA
  consultaVm.consultarExamenesAgregadosConsulta();

  //Consulta de PRESCRIPCION de la CONSULTA
  consultaVm.consultarPrescripcion();

  //Consulta de EXAMENES cuando todavia es una CITA
  consultaVm.consultarExamenesAgregadosCita();
  consultaVm.consultarPreguntasExamen();

  //RECETA MEDICA - PRODUCTOS
  // consultaVm.consultarProductosExistentes();
  consultaVm.consultarProductosExistentesConStock();

  //HISTORIAL DE CONSULTAS
  consultaVm.consultarHistorialConsultas();

  //Carga de archivo
  let inputFileNuevoArchivo = document.querySelector("#nuevo-archivo");
  const dataBase64NuevoArchvio = document.querySelector(
    "#base-64-nuevo-archivo"
  );
  async function encodeFileAsBase64URL(file) {
    return new Promise((resolve) => {
      const reader = new FileReader();
      reader.addEventListener("loadend", () => {
        resolve(reader.result);
      });
      reader.readAsDataURL(file);
    });
  }
  if (inputFileNuevoArchivo != null)
    inputFileNuevoArchivo.addEventListener("input", async (event) => {
      if (
        inputFileNuevoArchivo.files[0] != undefined &&
        inputFileNuevoArchivo.files[0] != null
      ) {
        let base64URL = await encodeFileAsBase64URL(
          inputFileNuevoArchivo.files[0]
        );
        dataBase64NuevoArchvio.setAttribute("value", base64URL);
      } else {
        dataBase64NuevoArchvio.removeAttribute("value");
      }
    });

  //Calculo FPP
  $("#PacienteApnpFechaUltimaRegla").on("change", function () {
    let fechaUltimaRegla = $("#PacienteApnpFechaUltimaRegla").val();
    if (fechaUltimaRegla != null) {
      let fechaProbableParto = moment(fechaUltimaRegla)
        .add(1, "years")
        .add(7, "days")
        .subtract(3, "months");
      $("#PacienteApnpFechaProbableParto").val(
        fechaProbableParto.format("YYYY-MM-DD")
      );
    } else {
      $("#PacienteApnpFechaProbableParto").val(null);
    }
  });
});

//var addprescription = function () {
//    var productname = document.getElementById('productname');
//    var productqty = document.getElementById('productqty');
//    var productin = document.getElementById('productin');
//    var prescriptiontbody = document.getElementById('Prescriptions');
//    if (productname.value.length > 0 && productqty.value.length > 0 && productin.value.length > 0) {
//        var cont = 0;
//        var postoedit = 0;
//        //@ts-ignore
//        for (var i = 0; i < prescriptiontbody.rows.length; i++) {
//            //@ts-ignore
//            var row = prescriptiontbody.rows[i];
//            if (row.cells[1].innerHTML === productname.value) {
//                cont = cont + 1;
//                postoedit = i;
//            }
//        }
//        if (cont == 0) {
//            var id = String(prescriptiontbody.childElementCount + 1);
//            var buttons = '<button class="btn btn-warning" onclick="editprescription(' + id + ')" ><i class="fas fa-edit"></i></button>'
//                + '&nbsp &nbsp'
//                + '<button onclick="deleteprescription(' + id + ')" class="btn btn-danger"><i class="fas fa-trash"></i></button>';
//            var tr = document.createElement('tr');
//            var ctd = document.createElement('td');
//            var pntd = document.createElement('td');
//            var pitd = document.createElement('td');
//            var pqtd = document.createElement('td');
//            var pactions = document.createElement('td');
//            var b = document.createElement('b');
//            tr.setAttribute('id', id);
//            b.innerHTML = id;
//            pntd.innerHTML = productname.value;
//            pqtd.innerHTML = productqty.value;
//            pitd.innerHTML = productin.value;
//            pactions.innerHTML = buttons;
//            ctd.appendChild(b);
//            tr.appendChild(ctd);
//            tr.appendChild(pntd);
//            tr.appendChild(pqtd);
//            tr.appendChild(pitd);
//            tr.appendChild(pactions);
//            console.log(tr);
//            prescriptiontbody.appendChild(tr);
//        }
//        else {
//            //@ts-ignore
//            prescriptiontbody.rows[postoedit].cells[2].innerHTML = productqty.value;
//            //@ts-ignore
//            prescriptiontbody.rows[postoedit].cells[3].innerHTML = productin.value;
//        }
//        productname.value = '';
//        productqty.value = '';
//        productin.value = '';
//        self.elementosPrescripcion.push(detalle);
//        itemPrescripcion++;
//        self.prescripcionNuevaMedicamento(null);
//        self.prescripcionNuevaCantidad(null);
//        self.prescripcionNuevaObservaciones(null);
//    }
//    //if (document.getElementById('generateprespdf').hasAttribute('disabled')) {
//    //    document.getElementById('generateprespdf').removeAttribute('disabled');
//    //}
//};

//document.getElementById('generateprespdf').removeAttribute('disabled');
var idConsulta = $("#ConsultaId").val();
var pacienteId = $("#PacienteId").val();

function getIMC() {
  // Obtener los valores de los campos de entrada de texto y convertirlos a números
  let peso = parseFloat($("#ExamenFisicoPeso").val());
  let talla = parseFloat($("#ExamenFisicoTalla").val());
  console.log(peso);
  console.log(talla);

  if (peso > 0 && talla > 0) {
    talla /= 100;

    // Calcular el IMC
    const imc = peso / (talla * talla);
    console.log(imc);
    // Devolver el resultado redondeado a dos decimales
    $("#ExamenFisicoIMC").val(imc.toFixed(2));
  }
}

function generarPdfExamenesLaboratorio(
  examenLaboratorioId,
  pacienteId,
  tipoPDF
) {
  window.open(
    "/CrearPDF/generarPdfExamenesLaboratorio?examenLaboratorioId=" +
      examenLaboratorioId +
      "&pacienteId=" +
      pacienteId +
      "&tipoPDF=" +
      tipoPDF,
    "_blank"
  );
  console.log("accediendo a instrucciones");
  console.log(examenLaboratorioId);
  console.log(tipoPDF);
}
// function generarPrescripcionPdf(prescripcionId, pacienteId) {
//   window.open(
//     "/CrearPDF/PrescripcionPDF?prescripcionId=" +
//       prescripcionId +
//       "&pacienteId=" +
//       pacienteId,
//     "_blank"
//   );
// }
function generarExamenesSolicitarPdf(prescripcionId) {
  window.open(
    "/CrearPDF/generarExamenesSolicitarPdf?prescripcionId=" + prescripcionId,
    "_blank"
  );
}

function generarGinecologiaPdf(consultaId) {
  window.open(
    "/CrearPDF/GinecologiaConsultaPDF?consultaId=" + consultaId,
    "_blank"
  );
}

function generarEndocavitarioPdf(consultaId) {
  //llama a la ruta de pdf  solo una hoja obstetrica
  window.open(
    "/CrearPDF/generarEndocavitarioPdf?consultaId=" + consultaId,
    "_blank"
  );
}

function generarObstetricaPDF(consultaId) {
  //llama a la ruta de pdf  solo una hoja obstetrica
  window.open(
    "/CrearPDF/generarObstetricaPDF?consultaId=" + consultaId,
    "_blank"
  );
}

//------------------generarObstetricaBiometriaPDF------------------

function generarObstetricaBiometriaPDF(consultaId) {
  window.open(
    "/CrearPDF/generarObstetricaBiometriaPDF?consultaId=" + consultaId,
    "_blank"
  );
}

function generarObstetricaBiometriaPDF2(consultaId) {
  window.open(
    "/CrearPDF/generarObstetricaBiometriaPDF2?consultaId=" + consultaId,
    "_blank"
  );
}

function generarObstetricaBiometriaPDF3(consultaId) {
  window.open(
    "/CrearPDF/generarObstetricaBiometriaPDF3?consultaId=" + consultaId,
    "_blank"
  );
}

function generarObstetricaBiometriaPDF4(consultaId) {
  window.open(
    "/CrearPDF/generarObstetricaBiometriaPDF4?consultaId=" + consultaId,
    "_blank"
  );
}

function verConsultaAnother(idConsulta) {
  window.open("/Consultas/Informacion/" + idConsulta, "_blank");
}

//------------------generarObstetricaBiometriaPDF------------------

//#region OFTALMOLOGÍA
// ==== OFTALMOLOGÍA: construir modelo desde el DOM ====
function buildModelOftalmologia() {
  // Importante: ids deben coincidir con asp-for (Oft_*).
  const val = (id) => {
    const el = document.getElementById(id);
    return el ? el.value : null;
  };

  const num = (id) => {
    const v = val(id);
    if (v === null || v === undefined || v === "") return null;
    const n = Number(v.replace(",", ".")); // por si el navegador mete coma
    return isNaN(n) ? null : n;
  };

  return {
    // ids ocultos para upsert
    Oft_Id: num("Oft_Id"),
    Oft_ConsultaId: num("Oft_ConsultaId"),

    // motivo e historia
    Oft_HistoriaEnfermedadActual: val("Oft_HistoriaEnfermedadActual"),

    // antecedentes
    Oft_PacienteMedicos: val("Oft_PacienteMedicos"),
    Oft_PacienteQuirurgicos: val("Oft_PacienteQuirurgicos"),
    Oft_PacienteTraumaticos: val("Oft_PacienteTraumaticos"),
    Oft_PacienteAlergias: val("Oft_PacienteAlergias"),
    Oft_PacienteFamiliares: val("Oft_PacienteFamiliares"),

    // AV sin corrección
    Oft_AgudezaSC_Test: val("Oft_AgudezaSC_Test"),
    Oft_AgudezaSC_OD: val("Oft_AgudezaSC_OD"),
    Oft_AgudezaSC_OS: val("Oft_AgudezaSC_OS"),

    // contraste
    Oft_Contraste_OD: val("Oft_Contraste_OD"),
    Oft_Contraste_OS: val("Oft_Contraste_OS"),

    // AV cerca sin corrección
    Oft_AVCerca_OD: val("Oft_AVCerca_OD"),
    Oft_AVCerca_OS: val("Oft_AVCerca_OS"),

    // tests especiales
    Oft_TestIshihara_OD: val("Oft_TestIshihara_OD"),
    Oft_TestIshihara_OS: val("Oft_TestIshihara_OS"),
    Oft_TestEstereopsis_OD: val("Oft_TestEstereopsis_OD"),
    Oft_TestEstereopsis_OS: val("Oft_TestEstereopsis_OS"),

    // Lensometría (histórico)
    Oft_Lensometria_OD_Esfera: num("Oft_Lensometria_OD_Esfera"),
    Oft_Lensometria_OD_Cilindro: num("Oft_Lensometria_OD_Cilindro"),
    Oft_Lensometria_OD_Eje: num("Oft_Lensometria_OD_Eje"),
    Oft_Lensometria_OD_Agudeza: val("Oft_Lensometria_OD_Agudeza"),
    Oft_Lensometria_OS_Esfera: num("Oft_Lensometria_OS_Esfera"),
    Oft_Lensometria_OS_Cilindro: num("Oft_Lensometria_OS_Cilindro"),
    Oft_Lensometria_OS_Eje: num("Oft_Lensometria_OS_Eje"),
    Oft_Lensometria_OS_Agudeza: val("Oft_Lensometria_OS_Agudeza"),

    // Final
    Oft_Final_OD_Esfera: num("Oft_Final_OD_Esfera"),
    Oft_Final_OD_Cilindro: num("Oft_Final_OD_Cilindro"),
    Oft_Final_OD_Eje: num("Oft_Final_OD_Eje"),
    Oft_Final_OD_Agudeza: val("Oft_Final_OD_Agudeza"),
    Oft_Final_OS_Esfera: num("Oft_Final_OS_Esfera"),
    Oft_Final_OS_Cilindro: num("Oft_Final_OS_Cilindro"),
    Oft_Final_OS_Eje: num("Oft_Final_OS_Eje"),
    Oft_Final_OS_Agudeza: val("Oft_Final_OS_Agudeza"),
    Oft_Final_Adicion: num("Oft_Final_Adicion"),
    Oft_Final_DIP_mm: num("Oft_Final_DIP_mm"),

    // Retinoscopía
    Oft_Retino_OD_Esfera: num("Oft_Retino_OD_Esfera"),
    Oft_Retino_OD_Cilindro: num("Oft_Retino_OD_Cilindro"),
    Oft_Retino_OD_Eje: num("Oft_Retino_OD_Eje"),
    Oft_Retino_OS_Esfera: num("Oft_Retino_OS_Esfera"),
    Oft_Retino_OS_Cilindro: num("Oft_Retino_OS_Cilindro"),
    Oft_Retino_OS_Eje: num("Oft_Retino_OS_Eje"),

    // Tipo de lente
    Oft_TipoLente: val("Oft_TipoLente"),
    Oft_LenteMaterialTratamiento: val("Oft_LenteMaterialTratamiento"),

    // Inspección / lámpara / oftalmoscopía
    Oft_Inspeccion_MovExtraoculares_OD: val(
      "Oft_Inspeccion_MovExtraoculares_OD"
    ),
    Oft_Inspeccion_MovExtraoculares_OS: val(
      "Oft_Inspeccion_MovExtraoculares_OS"
    ),
    Oft_Inspeccion_Cejas_OD: val("Oft_Inspeccion_Cejas_OD"),
    Oft_Inspeccion_Cejas_OS: val("Oft_Inspeccion_Cejas_OS"),
    Oft_Inspeccion_ParpadosPestanas_OD: val(
      "Oft_Inspeccion_ParpadosPestanas_OD"
    ),
    Oft_Inspeccion_ParpadosPestanas_OS: val(
      "Oft_Inspeccion_ParpadosPestanas_OS"
    ),
    Oft_Inspeccion_ViaLagrimal_OD: val("Oft_Inspeccion_ViaLagrimal_OD"),
    Oft_Inspeccion_ViaLagrimal_OS: val("Oft_Inspeccion_ViaLagrimal_OS"),
    Oft_Inspeccion_Conjuntiva_OD: val("Oft_Inspeccion_Conjuntiva_OD"),
    Oft_Inspeccion_Conjuntiva_OS: val("Oft_Inspeccion_Conjuntiva_OS"),
    Oft_Inspeccion_CorneaEsclera_OD: val("Oft_Inspeccion_CorneaEsclera_OD"),
    Oft_Inspeccion_CorneaEsclera_OS: val("Oft_Inspeccion_CorneaEsclera_OS"),
    Oft_Inspeccion_CamaraAnteriorAngulo_OD: val(
      "Oft_Inspeccion_CamaraAnteriorAngulo_OD"
    ),
    Oft_Inspeccion_CamaraAnteriorAngulo_OS: val(
      "Oft_Inspeccion_CamaraAnteriorAngulo_OS"
    ),
    Oft_Inspeccion_IrisPupila_OD: val("Oft_Inspeccion_IrisPupila_OD"),
    Oft_Inspeccion_IrisPupila_OS: val("Oft_Inspeccion_IrisPupila_OS"),
    Oft_Inspeccion_Cristalino_OD: val("Oft_Inspeccion_Cristalino_OD"),
    Oft_Inspeccion_Cristalino_OS: val("Oft_Inspeccion_Cristalino_OS"),
    Oft_Inspeccion_BUT_OD: val("Oft_Inspeccion_BUT_OD"),
    Oft_Inspeccion_BUT_OS: val("Oft_Inspeccion_BUT_OS"),
    Oft_Inspeccion_PresionIntraocular_OD: num(
      "Oft_Inspeccion_PresionIntraocular_OD"
    ),
    Oft_Inspeccion_PresionIntraocular_OS: num(
      "Oft_Inspeccion_PresionIntraocular_OS"
    ),
    Oft_Inspeccion_Vitreo_OD: val("Oft_Inspeccion_Vitreo_OD"),
    Oft_Inspeccion_Vitreo_OS: val("Oft_Inspeccion_Vitreo_OS"),
    Oft_Inspeccion_NervioOptico_OD: val("Oft_Inspeccion_NervioOptico_OD"),
    Oft_Inspeccion_NervioOptico_OS: val("Oft_Inspeccion_NervioOptico_OS"),
    Oft_Inspeccion_Macula_OD: val("Oft_Inspeccion_Macula_OD"),
    Oft_Inspeccion_Macula_OS: val("Oft_Inspeccion_Macula_OS"),
    Oft_Inspeccion_Retina_OD: val("Oft_Inspeccion_Retina_OD"),
    Oft_Inspeccion_Retina_OS: val("Oft_Inspeccion_Retina_OS"),
    Oft_HistoriaClinicaImpresionClinica: val(
      "Oft_HistoriaClinicaImpresionClinica"
    ),
    Oft_HistoriaClinicaComentario: val("Oft_HistoriaClinicaComentario"),
  };
}
//#endregion

//#region PODOLOGIA
// ==== PODOLOGÍA: construir modelo desde el DOM ====
function buildModelPodologia() {
  // Helpers
  const val = (id) => {
    const el = document.getElementById(id);
    return el ? el.value : null;
  };
  const num = (id) => {
    const v = val(id);
    if (v === null || v === undefined || v === "") return null;
    const n = Number((v + "").replace(",", "."));
    return isNaN(n) ? null : n;
  };
  const radio = (name) => {
    const el = document.querySelector(`input[name="${name}"]:checked`);
    return el ? el.value : null;
  };
  const boolFromRadio = (name) => {
    const v = radio(name);
    if (v === null) return null; // deja null si no se eligió nada
    if (v === "true" || v === "si" || v === "sí" || v === "1") return true;
    if (v === "false" || v === "no" || v === "0") return false;
    return null; // valor inesperado
  };
  const arrFromChecks = (name) =>
    Array.from(document.querySelectorAll(`input[name="${name}"]:checked`)).map(
      (x) => x.value
    );

  return {
    // IDs para upsert (ocultos)
    Pod_Id: num("Pod_Id"),
    Pod_ConsultaId: num("Pod_ConsultaId"),

    // 1) Antecedentes Médicos
    // Requiere checkboxes con name="Pod_Enfermedades" y value="DM|HTA|Artritis|Artrosis|Osteop|Otros"
    Pod_Enfermedades: arrFromChecks("Pod_Enfermedades"),
    Pod_Enfermedades_Otros: val("Pod_Enfermedades_Otros"),
    Pod_Medicamentos: val("Pod_Medicamentos"),
    Pod_PresionArterial: val("Pod_PresionArterial"),

    // 2) Examen del Pie
    Pod_Pulso_Pedio: val("Pod_Pulso_Pedio"),
    Pod_Pulso_TibialPosterior: val("Pod_Pulso_TibialPosterior"),
    Pod_Pulso_Popliteo: val("Pod_Pulso_Popliteo"),

    // Radios:
    //   TemperaturaPie -> radios name="Pod_TemperaturaPie" (fria|normal|caliente)
    //   ProblemasCirculatorios -> radios name="Pod_ProblemasCirculatorios" (true/false o si/no)
    //   EstadoPiel -> radios name="Pod_EstadoPiel" (seca|normal|humeda)
    Pod_TemperaturaPie: radio("Pod_TemperaturaPie"),
    Pod_ProblemasCirculatorios: boolFromRadio("Pod_ProblemasCirculatorios"),
    Pod_EstadoPiel: radio("Pod_EstadoPiel"),

    Pod_ObservacionesExamen: val("Pod_ObservacionesExamen"),

    // 3) Tratamiento Realizado
    // Requiere checkboxes name="Pod_Procedimientos"
    Pod_Procedimientos: arrFromChecks("Pod_Procedimientos"),
    Pod_OtrosProcedimientos: val("Pod_OtrosProcedimientos"),
    Pod_ObservacionesTratamiento: val("Pod_ObservacionesTratamiento"),

    // 4) Indicaciones y Datos Finales
    Pod_Indicaciones: val("Pod_Indicaciones"),
    Pod_PesoKg: num("Pod_PesoKg"),
    Pod_EstaturaM: num("Pod_EstaturaM"),
    // Para DateTime? en el backend, basta el string "YYYY-MM-DD"
    Pod_FechaAtencion: val("Pod_FechaAtencion"),
    Pod_Profesional: val("Pod_Profesional"),
  };
}
//#endregion

//#region HistoriaClinicaEnfermeria (estandarizado como Oftalmología)
// ==== ENFERMERÍA: construir modelo desde el DOM ====
function buildModelEnfermeria() {
  // Helpers (mismo estilo que Oftalmología)
  const val = (id) => {
    const el = document.getElementById(id);
    return el ? el.value : null;
  };
  const num = (id) => {
    const v = val(id);
    if (v === null || v === undefined || v === "") return null;
    const n = Number(String(v).replace(",", "."));
    return isNaN(n) ? null : n;
  };
  const numInt = (id) => {
    const v = val(id);
    if (v === null || v === undefined || v === "") return null;
    const cleaned = String(v).replace(",", "."); // tolera coma
    const n = parseInt(cleaned, 10);
    return isNaN(n) ? null : n;
  };
  const radio = (name) => {
    const el = document.querySelector(`input[name="${name}"]:checked`);
    return el ? el.value : null;
  };

  return {
    // PK/FK
    Hce_Id: numInt("Hce_Id"),
    Hce_ConsultaId: numInt("Hce_ConsultaId") ?? numInt("ConsultaId"),

    // 2) Tipo de consulta (radios por name="Hce_TipoConsulta" o input con id)
    Hce_TipoConsulta: val("Hce_TipoConsulta"),

    // 3) Motivo de consulta
    Hce_MotivoConsulta: val("Hce_MotivoConsulta"),

    // 4) Antecedentes personales y familiares
    Hce_AntecedentesPatologicos: val("Hce_AntecedentesPatologicos"),
    Hce_AntecedentesQuirurgicos: val("Hce_AntecedentesQuirurgicos"),
    Hce_AntecedentesTraumaticos: val("Hce_AntecedentesTraumaticos"),
    Hce_Hospitalizaciones: val("Hce_Hospitalizaciones"),
    Hce_Alergias: val("Hce_Alergias"),
    Hce_AntecedentesFamiliares: val("Hce_AntecedentesFamiliares"),

    // 5) Hábitos personales
    Hce_HabitoAlimentacion: val("Hce_HabitoAlimentacion"),
    Hce_ActividadFisica: val("Hce_ActividadFisica"),
    Hce_HabitoAlcoholTexto: val("Hce_HabitoAlcoholTexto"),
    Hce_HabitoTabacoTexto: val("Hce_HabitoTabacoTexto"),
    Hce_OtrosHabitos: val("Hce_OtrosHabitos"),

    // 6) Signos vitales y antropometría
    Hce_PresionArterialTxt: val("Hce_PresionArterialTxt"),
    Hce_FC: numInt("Hce_FC"),
    Hce_FR: numInt("Hce_FR"),
    Hce_TemperaturaC: num("Hce_TemperaturaC"),
    Hce_SPO2: numInt("Hce_SPO2"),
    Hce_PesoKg: num("Hce_PesoKg"),
    Hce_TallaM: num("Hce_TallaM"),
    Hce_IMC: num("Hce_IMC"),

    // 7) Exploración por aparatos y sistemas
    Hce_CabezaCuello: val("Hce_CabezaCuello"),
    Hce_ToraxPulmones: val("Hce_ToraxPulmones"),
    Hce_Corazon: val("Hce_Corazon"),
    Hce_Abdomen: val("Hce_Abdomen"),
    Hce_Extremidades: val("Hce_Extremidades"),
    Hce_SistemaNeurologico: val("Hce_SistemaNeurologico"),
    Hce_PielAnexos: val("Hce_PielAnexos"),

    // 8) Valoración de enfermería
    Hce_ValConcienciaOrientacion: val("Hce_ValConcienciaOrientacion"),
    Hce_ValEstadoNutricional: val("Hce_ValEstadoNutricional"),
    Hce_ValEliminacion: val("Hce_ValEliminacion"),
    Hce_ValSuenoDescanso: val("Hce_ValSuenoDescanso"),
    Hce_ValActividadMovilidad: val("Hce_ValActividadMovilidad"),
    Hce_ValAutonomia: val("Hce_ValAutonomia"),

    // 9) Laboratorios
    Hce_Laboratorios: val("Hce_Laboratorios"),

    // 10) Diagnóstico de enfermería
    Hce_DiagnosticoEnfermeria: val("Hce_DiagnosticoEnfermeria"),

    // 11) Plan de cuidados / Intervenciones
    Hce_AccionesRealizadas: val("Hce_AccionesRealizadas"),
    Hce_MedicamentosAdministrados: val("Hce_MedicamentosAdministrados"),
    Hce_Tratamiento: val("Hce_Tratamiento"),

    // 12) Seguimiento / Evolución / Cita
    Hce_Seguimiento: val("Hce_Seguimiento"),
  };
}
//#endregion

//#region HistoriaClinicaEnfermeria (estandarizado como Oftalmología)
// ==== ENFERMERÍA: construir modelo desde el DOM ====

function buildModelEnfermeria() {
  // Helpers
  const val = (id) => {
    const el = document.getElementById(id);
    return el ? el.value : null;
  };
  const num = (id) => {
    const v = val(id);
    if (v === null || v === undefined || v === "") return null;
    const n = Number(String(v).replace(",", "."));
    return Number.isFinite(n) ? n : null;
  };
  const numInt = (id) => {
    const v = val(id);
    if (v === null || v === undefined || v === "") return null;
    const cleaned = String(v).replace(",", "."); // tolera coma
    const n = parseInt(cleaned, 10);
    return Number.isInteger(n) ? n : null;
  };
  const radio = (name) => {
    const el = document.querySelector(`input[name="${name}"]:checked`);
    return el ? el.value : null;
  };

  return {
    // PK/FK
    Hce_Id: numInt("Hce_Id"),
    Hce_ConsultaId: numInt("Hce_ConsultaId") ?? numInt("ConsultaId"),

    // 1) Tipo de consulta
    Hce_TipoConsulta: radio("Hce_TipoConsulta") ?? val("Hce_TipoConsulta"),

    // 2) Motivo de consulta
    Hce_MotivoConsulta: val("Hce_MotivoConsulta"),

    // 3) Antecedentes personales y familiares
    Hce_AntecedentesPatologicos: val("Hce_AntecedentesPatologicos"),
    Hce_AntecedentesQuirurgicos: val("Hce_AntecedentesQuirurgicos"),
    Hce_AntecedentesTraumaticos: val("Hce_AntecedentesTraumaticos"),
    Hce_Hospitalizaciones: val("Hce_Hospitalizaciones"),
    Hce_Alergias: val("Hce_Alergias"),
    Hce_AntecedentesFamiliares: val("Hce_AntecedentesFamiliares"),

    // 4) Hábitos personales
    Hce_HabitoAlimentacion: val("Hce_HabitoAlimentacion"),
    Hce_ActividadFisica: val("Hce_ActividadFisica"),
    Hce_HabitoAlcoholTexto: val("Hce_HabitoAlcoholTexto"),
    Hce_HabitoTabacoTexto: val("Hce_HabitoTabacoTexto"),
    Hce_OtrosHabitos: val("Hce_OtrosHabitos"),

    // 5) Signos vitales y antropometría
    Hce_PresionArterialTxt: val("Hce_PresionArterialTxt"),
    Hce_FC: numInt("Hce_FC"),
    Hce_FR: numInt("Hce_FR"),
    Hce_TemperaturaC: num("Hce_TemperaturaC"),
    Hce_SPO2: numInt("Hce_SPO2"),
    Hce_PesoKg: num("Hce_PesoKg"),
    Hce_TallaM: num("Hce_TallaM"),
    Hce_IMC: num("Hce_IMC"),

    // 6) Exploración por aparatos y sistemas
    Hce_CabezaCuello: val("Hce_CabezaCuello"),
    Hce_ToraxPulmones: val("Hce_ToraxPulmones"),
    Hce_Corazon: val("Hce_Corazon"),
    Hce_Abdomen: val("Hce_Abdomen"),
    Hce_Extremidades: val("Hce_Extremidades"),
    Hce_SistemaNeurologico: val("Hce_SistemaNeurologico"),
    Hce_PielAnexos: val("Hce_PielAnexos"),

    // 7) Valoración de enfermería
    Hce_ValConcienciaOrientacion: val("Hce_ValConcienciaOrientacion"),
    Hce_ValEstadoNutricional: val("Hce_ValEstadoNutricional"),
    Hce_ValEliminacion: val("Hce_ValEliminacion"),
    Hce_ValSuenoDescanso: val("Hce_ValSuenoDescanso"),
    Hce_ValActividadMovilidad: val("Hce_ValActividadMovilidad"),
    Hce_ValAutonomia: val("Hce_ValAutonomia"),

    // 8) Laboratorios
    Hce_Laboratorios: val("Hce_Laboratorios"),

    // 9) Diagnóstico de enfermería
    Hce_DiagnosticoEnfermeria: val("Hce_DiagnosticoEnfermeria"),

    // 10) Plan de cuidados / Intervenciones
    Hce_AccionesRealizadas: val("Hce_AccionesRealizadas"),
    Hce_MedicamentosAdministrados: val("Hce_MedicamentosAdministrados"),
    Hce_Tratamiento: val("Hce_Tratamiento"),

    // 11) Seguimiento / Evolución / Cita
    Hce_Seguimiento: val("Hce_Seguimiento"),
  };
}
//#endregion
//#region Sueroterapia
function buildModelSueroterapia() {
  // Helpers (mismo patrón que HCE)
  const val = (id) => {
    const el = document.getElementById(id);
    return el ? el.value : null;
  };
  const num = (id) => {
    const v = val(id);
    if (v === null || v === undefined || v === "") return null;
    const n = Number(String(v).replace(",", "."));
    return Number.isFinite(n) ? n : null;
  };
  const numInt = (id) => {
    const v = val(id);
    if (v === null || v === undefined || v === "") return null;
    const cleaned = String(v).replace(",", ".");
    const n = parseInt(cleaned, 10);
    return Number.isInteger(n) ? n : null;
  };
  const radio = (name) => {
    const el = document.querySelector(`input[name="${name}"]:checked`);
    return el ? el.value : null;
  };
  const arr = (name) =>
    Array.from(document.querySelectorAll(`input[name="${name}"]:checked`)).map(
      (x) => x.value
    );

  return {
    // PK / FK
    Suero_Id: numInt("Suero_Id"),
    Suero_ConsultaId: numInt("Suero_ConsultaId") ?? numInt("ConsultaId"),

    // 1) Datos de valoración inicial
    Suero_Motivo: val("Suero_Motivo"),
    Suero_DiagnosticoMedico: val("Suero_DiagnosticoMedico"),
    Suero_Labs: val("Suero_Labs"),

    // 2) ¿Cómo se enteró del servicio? (checkbox múltiple)
    Suero_Medio: arr("Suero_Medio"),

    // 3) Oxigenación y circulación (checkbox múltiple)
    Suero_Resp: arr("Suero_Resp"),
    Suero_Circ: arr("Suero_Circ"),

    // 4) Necesidad de nutrición
    Suero_Nutricion: arr("Suero_Nutricion"),
    Suero_NutricionObs: val("Suero_NutricionObs"),

    // 5) Plan
    Suero_PlanTerapeutico: val("Suero_PlanTerapeutico"),
  };
}
//#endregion
