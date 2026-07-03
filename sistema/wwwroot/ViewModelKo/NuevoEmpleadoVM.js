function NuevoEmpleadoViewModel() {
  var self = this;

  // --- Alertas UI ---
  self.AlertaVisible = ko.observable(false);
  self.AlertaMensaje = ko.observable("");
  self.AlertaTipo = ko.observable("danger"); // "danger" | "success" | "warning" | "info"

  self.AlertaCss = ko.pureComputed(function () {
    return "alert-" + (self.AlertaTipo() || "danger");
  });

  self.mostrarAlerta = function (mensaje, tipo) {
    self.AlertaMensaje(mensaje || "Ocurrió un error.");
    self.AlertaTipo(tipo || "danger");
    self.AlertaVisible(true);

    try {
      window.scrollTo({ top: 0, behavior: "smooth" });
    } catch (_) {
      window.scrollTo(0, 0);
    }
  };

  self.ocultarAlerta = function () {
    self.AlertaVisible(false);
    self.AlertaMensaje("");
    self.AlertaTipo("danger");
  };

  // ==========================
  // Helpers
  // ==========================
  self._isBlank = function (v) {
    if (v === undefined || v === null) return true;
    var s = v.toString();
    return !s || !s.trim();
  };

  self._appendIfNotBlank = function (fd, key, val) {
    if (!self._isBlank(val)) fd.append(key, val);
  };

  self._appendRequired = function (fd, key, val) {
    // para requeridos: si viene null/undefined lo mandamos como ""
    fd.append(key, val === undefined || val === null ? "" : val);
  };

  // ==========================
  // Observables
  // ==========================

  // Pivot para backend / lógica (SIEMPRE se envía)
  // Valores esperados: "Empleado" | "Medico"
  self.TipoEmpleadoFormulario = ko.observable("Empleado");

  // Observables compartidos
  self.Nombre = ko.observable("");
  self.Apellido = ko.observable("");
  self.Telefono = ko.observable("");
  self.Telefono_2 = ko.observable("");
  self.Email = ko.observable("");
  self.Edad = ko.observable("");
  self.Dpi = ko.observable("");
  self.Nit = ko.observable("");
  self.EstadoCivil = ko.observable("");
  self.Direccion = ko.observable("");
  self.TipoContrato = ko.observable("");
  self.Salario = ko.observable("");
  self.TelefonoClinica = ko.observable("");
  self.SucursalId = ko.observable(null);
  self.TipoEmpleado = ko.observable("");
  self.JornadaTrabajo = ko.observable("");
  self.FechaInicioLabores = ko.observable("");
  self.VacacionesProgramadas = ko.observable("");
  self.ColorHexadecimalFondo = ko.observable("#ffffff");
  self.ColorHexadecimalTexto = ko.observable("#000000");
  self.Observaciones = ko.observable("");
  self.FirmaEmpleadoImagen = ko.observable(null);

  // ==========================
  // ✅ Ubicación organizacional (se persiste en Empleado)
  // ==========================
  self.DepartamentoId = ko.observable(null); // solo filtro UI (no se persiste en Empleado)
  self.UnidadOrgId = ko.observable(null); // ✅ Empleado.UnidadOrgId
  self.SeccionOrgId = ko.observable(null); // ✅ Empleado.SeccionOrgId

  // ✅ Listados para selects (lo que te faltaba)
  self.DepartamentosOrg = ko.observableArray([]);
  self.UnidadesOrg = ko.observableArray([]);
  self.SeccionesOrg = ko.observableArray([]);

  // flags opcionales (por si quieres deshabilitar selects mientras carga)
  self.CargandoDepartamentos = ko.observable(false);
  self.CargandoUnidades = ko.observable(false);
  self.CargandoSecciones = ko.observable(false);

  // Observables Para Médicos
  self.Colegiado = ko.observable("");
  self.Genero = ko.observable("");
  self.Residente = ko.observable("");
  self.Credenciales = ko.observable("");
  self.DireccionClinica = ko.observable("");
  self.TipoBanco = ko.observable("");
  self.TipoCuenta = ko.observable("");
  self.NumeroCuenta = ko.observable("");
  self.NombreCuenta = ko.observable("");
  self.NitPropietarioCuenta = ko.observable("");
  self.NombrePropietarioNit = ko.observable("");
  self.TipoRegimen = ko.observable("");
  self.EspecialidadIdSeleccionada = ko.observable("");

  // ==========================
  // Subscripciones / eventos
  // ==========================
  // Cálculo automático de edad basado en "Credenciales" (Fecha de Nacimiento)
  self.Credenciales.subscribe(function (fechaNacimiento) {
    if (!fechaNacimiento) {
      self.Edad("");
      return;
    }

    var nacimiento = new Date(fechaNacimiento + "T00:00:00");
    var hoy = new Date();

    var anios = hoy.getFullYear() - nacimiento.getFullYear();
    var meses = hoy.getMonth() - nacimiento.getMonth();
    var dias = hoy.getDate() - nacimiento.getDate();

    if (meses < 0 || (meses === 0 && dias < 0)) {
      anios--;
      meses += 12;
    }

    if (dias < 0) {
      var ultimoDiaMesAnterior = new Date(hoy.getFullYear(), hoy.getMonth(), 0).getDate();
      dias += ultimoDiaMesAnterior;
      meses--;
      if (meses < 0) {
        meses += 12;
      }
    }

    var edadTexto = anios + " años, " + meses + " meses, " + dias + " días";
    self.Edad(edadTexto);
  });
  self.FechaInicioLabores.subscribe(function (newValue) {
    if (newValue) {
      var fecha = new Date(newValue);
      fecha.setFullYear(fecha.getFullYear() + 1);
      self.VacacionesProgramadas(fecha.toISOString().slice(0, 10));
    }
  });

  // self.loadFirmaEmpleado = function (element, event) {
  //   var file = event.target.files && event.target.files[0];
  //   self.FirmaEmpleadoImagen(file || null);
  // };

  // === LÓGICA DE FIRMA MODAL ===
  self.FirmaPrevisualizacion = ko.observable(""); // Inicia vacío porque es nuevo
  self.FirmaTemporalBase64 = ko.observable(null);
  self.FirmaTemporalFile = ko.observable(null);

  // Exponemos el ViewModel globalmente para que el script del Canvas pueda hablar con él
  window.empleadoVm = self;

  self.setFirmaTemporal = function (base64, fileObj) {
    self.FirmaTemporalBase64(base64);
    self.FirmaTemporalFile(fileObj);

    document.getElementById("btnConfirmarFirmaEmpleado").disabled = false;

    var contenedorPrev = document.getElementById("contenedorPrevFirmaEmpleado");
    var imgPrev = document.getElementById("imgPrevFirmaEmpleado");
    var selector = document.getElementById("selectorFirmaEmpleado");

    if (base64) {
      imgPrev.src = base64;
    }
    selector.classList.add("d-none");
    contenedorPrev.classList.remove("d-none");
  };

  self.prepararModalFirma = function () {
    window.resetearFirmaEmpleadoUI();

    var firmaActual = self.FirmaPrevisualizacion();
    if (firmaActual) {
      var contenedorPrev = document.getElementById(
        "contenedorPrevFirmaEmpleado",
      );
      var imgPrev = document.getElementById("imgPrevFirmaEmpleado");
      var selector = document.getElementById("selectorFirmaEmpleado");

      imgPrev.src = firmaActual;
      selector.classList.add("d-none");
      contenedorPrev.classList.remove("d-none");

      document.getElementById("btnConfirmarFirmaEmpleado").disabled = false;
    }

    window.abrirModalFirmaEmpleado();
  };

  self.confirmarFirmaModal = function () {
    var base64 = self.FirmaTemporalBase64();
    var file = self.FirmaTemporalFile();

    if (file) {
      self.FirmaEmpleadoImagen(file);
      self.FirmaPrevisualizacion(base64);
    } else if (base64) {
      var arr = base64.split(","),
        mime = arr[0].match(/:(.*?);/)[1];
      var bstr = atob(arr[1]),
        n = bstr.length,
        u8arr = new Uint8Array(n);
      while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
      }
      // Usamos File para mantener el mismo formato que funciona en tu backend
      var newFile = new File([u8arr], "firma_canvas.png", { type: mime });

      self.FirmaEmpleadoImagen(newFile);
      self.FirmaPrevisualizacion(base64);
    }

    window.cerrarModalFirmaEmpleado();
  };

  // ==========================
  // ✅ Catálogos Org (AJAX) - LO NUEVO
  // ==========================

  function _toIntOrNull(v) {
    if (v === "" || v === undefined || v === null) return null;
    var n = parseInt(v, 10);
    return isNaN(n) ? null : n;
  }

  // Normaliza ids en observables (por si el select manda string)
  self.DepartamentoId.subscribe(function (v) {
    self.DepartamentoId(_toIntOrNull(v));
  });
  self.UnidadOrgId.subscribe(function (v) {
    self.UnidadOrgId(_toIntOrNull(v));
  });
  self.SeccionOrgId.subscribe(function (v) {
    self.SeccionOrgId(_toIntOrNull(v));
  });

  self.cargarDepartamentosOrg = function () {
    self.CargandoDepartamentos(true);
    return fetch("/Empleado/DepartamentosOrg", { method: "GET" })
      .then(function (r) {
        return r.json();
      })
      .then(function (json) {
        if (json && json.success) {
          self.DepartamentosOrg(json.data || []);
          return;
        }
        self.DepartamentosOrg([]);
        self.mostrarAlerta(
          (json && json.message) || "No se pudieron cargar los departamentos.",
          "danger",
        );
      })
      .catch(function (err) {
        self.DepartamentosOrg([]);
        self.mostrarAlerta(
          (err && err.message) || "Error al cargar departamentos.",
          "danger",
        );
      })
      .finally(function () {
        self.CargandoDepartamentos(false);
      });
  };

  self.cargarUnidadesOrg = function (departamentoOrgId) {
    var depId = _toIntOrNull(departamentoOrgId);

    // si no hay departamento, limpiamos y listo
    if (!depId) {
      self.UnidadesOrg([]);
      self.UnidadOrgId(null);
      self.SeccionesOrg([]);
      self.SeccionOrgId(null);
      return Promise.resolve();
    }

    self.CargandoUnidades(true);
    return fetch(
      "/Empleado/UnidadesOrg?departamentoOrgId=" + encodeURIComponent(depId),
      { method: "GET" },
    )
      .then(function (r) {
        return r.json();
      })
      .then(function (json) {
        if (json && json.success) {
          self.UnidadesOrg(json.data || []);

          // si la unidad seleccionada no existe ya, la limpiamos
          var currentUnidad = _toIntOrNull(self.UnidadOrgId());
          if (currentUnidad) {
            var exists = (json.data || []).some(function (x) {
              return x.id === currentUnidad;
            });
            if (!exists) self.UnidadOrgId(null);
          }
          return;
        }
        self.UnidadesOrg([]);
        self.UnidadOrgId(null);
        self.mostrarAlerta(
          (json && json.message) || "No se pudieron cargar las unidades.",
          "danger",
        );
      })
      .catch(function (err) {
        self.UnidadesOrg([]);
        self.UnidadOrgId(null);
        self.mostrarAlerta(
          (err && err.message) || "Error al cargar unidades.",
          "danger",
        );
      })
      .finally(function () {
        self.CargandoUnidades(false);
      });
  };

  self.cargarSeccionesOrg = function (unidadOrgId) {
    var unId = _toIntOrNull(unidadOrgId);

    if (!unId) {
      self.SeccionesOrg([]);
      self.SeccionOrgId(null);
      return Promise.resolve();
    }

    self.CargandoSecciones(true);
    return fetch(
      "/Empleado/SeccionesOrg?unidadOrgId=" + encodeURIComponent(unId),
      { method: "GET" },
    )
      .then(function (r) {
        return r.json();
      })
      .then(function (json) {
        if (json && json.success) {
          self.SeccionesOrg(json.data || []);

          var currentSeccion = _toIntOrNull(self.SeccionOrgId());
          if (currentSeccion) {
            var exists = (json.data || []).some(function (x) {
              return x.id === currentSeccion;
            });
            if (!exists) self.SeccionOrgId(null);
          }
          return;
        }
        self.SeccionesOrg([]);
        self.SeccionOrgId(null);
        self.mostrarAlerta(
          (json && json.message) || "No se pudieron cargar las secciones.",
          "danger",
        );
      })
      .catch(function (err) {
        self.SeccionesOrg([]);
        self.SeccionOrgId(null);
        self.mostrarAlerta(
          (err && err.message) || "Error al cargar secciones.",
          "danger",
        );
      })
      .finally(function () {
        self.CargandoSecciones(false);
      });
  };

  // ✅ Cascada: Dep -> Unidades -> Secciones
  self.DepartamentoId.subscribe(function (depId) {
    // al cambiar depto, limpiamos selección unidad/sección y recargamos unidades
    self.UnidadesOrg([]);
    self.UnidadOrgId(null);
    self.SeccionesOrg([]);
    self.SeccionOrgId(null);

    self.cargarUnidadesOrg(depId);
  });

  self.UnidadOrgId.subscribe(function (unidadId) {
    // al cambiar unidad, limpiamos sección y recargamos secciones
    self.SeccionesOrg([]);
    self.SeccionOrgId(null);

    self.cargarSeccionesOrg(unidadId);
  });

  // ==========================
  // Validaciones
  // ==========================
  self.validarEmpleadoNoMedico = function () {
    var faltantes = [];

    if (self._isBlank(self.Nombre())) faltantes.push("Nombre");
    if (self._isBlank(self.Apellido())) faltantes.push("Apellido");
    if (self._isBlank(self.Telefono())) faltantes.push("Telefono");
    if (self._isBlank(self.Email())) faltantes.push("Email");
    if (self._isBlank(self.Dpi())) faltantes.push("Dpi");
    if (self._isBlank(self.Nit())) faltantes.push("Nit");
    if (self._isBlank(self.FechaInicioLabores()))
      faltantes.push("Fecha de Inicio de Labores");
    if (self._isBlank(self.VacacionesProgramadas()))
      faltantes.push("Vacaciones Programadas");
    if (self._isBlank(self.JornadaTrabajo()))
      faltantes.push("Jornada de Trabajo");

    // ✅ Unidad/Sección NO obligatorias por ahora

    return faltantes;
  };

  self.validarEmpleadoMedico = function () {
    var faltantes = [];

    if (self._isBlank(self.Nombre())) faltantes.push("Nombre");
    if (self._isBlank(self.Apellido())) faltantes.push("Apellido");
    if (self._isBlank(self.Telefono())) faltantes.push("Telefono");
    if (self._isBlank(self.Email())) faltantes.push("Email");
    if (self._isBlank(self.Dpi())) faltantes.push("Dpi");
    if (self._isBlank(self.Nit())) faltantes.push("Nit");
    if (self._isBlank(self.FechaInicioLabores()))
      faltantes.push("Fecha de Inicio de Labores");
    if (self._isBlank(self.VacacionesProgramadas()))
      faltantes.push("Vacaciones Programadas");
    if (self._isBlank(self.JornadaTrabajo()))
      faltantes.push("Jornada de Trabajo");

    if (self._isBlank(self.EspecialidadIdSeleccionada()))
      faltantes.push("Especialidad");

    // (Debajo de los otros if)
    if (self._isBlank(self.Credenciales())) faltantes.push("Fecha de Nacimiento");
    if (self._isBlank(self.Colegiado())) faltantes.push("Colegiado");
    // ✅ Unidad/Sección NO obligatorias por ahora
    if (!self.FirmaEmpleadoImagen()) {
      faltantes.push("Firma del Médico");
    }

    return faltantes;
  };

  // ==========================
  // Acciones
  // ==========================

  // NO médico (1:1 con vista)
  self.agregarEmpleado = function () {
    self.ocultarAlerta();

    self.TipoEmpleadoFormulario("Empleado");

    var faltantes = self.validarEmpleadoNoMedico();
    if (faltantes.length > 0) {
      self.mostrarAlerta(
        "Faltan campos obligatorios: " + faltantes.join(", "),
        "danger",
      );
      return;
    }

    var formData = new FormData();

    self._appendRequired(formData, "TipoEmpleadoFormulario", "Empleado");

    self._appendRequired(formData, "Empleado.Nombre", self.Nombre());
    self._appendRequired(formData, "Empleado.Apellido", self.Apellido());
    self._appendRequired(formData, "Empleado.Telefono", self.Telefono());
    self._appendRequired(formData, "Empleado.Dpi", self.Dpi());
    self._appendRequired(formData, "Empleado.Nit", self.Nit());
    self._appendRequired(
      formData,
      "Empleado.FechaInicioLabores",
      self.FechaInicioLabores(),
    );
    self._appendRequired(
      formData,
      "Empleado.VacacionesProgramadas",
      self.VacacionesProgramadas(),
    );
    self._appendRequired(
      formData,
      "Empleado.JornadaTrabajo",
      self.JornadaTrabajo(),
    );

    // ✅ Unidad/Sección (opcionales)
    self._appendIfNotBlank(
      formData,
      "Empleado.UnidadOrgId",
      self.UnidadOrgId(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.SeccionOrgId",
      self.SeccionOrgId(),
    );

    if (self.FirmaEmpleadoImagen()) {
      formData.append("FirmaEmpleadoImagen", self.FirmaEmpleadoImagen());
    }

    self._appendIfNotBlank(formData, "Empleado.Telefono_2", self.Telefono_2());
    self._appendIfNotBlank(formData, "Empleado.Email", self.Email());
    self._appendIfNotBlank(formData, "Empleado.Edad", self.Edad());
    self._appendIfNotBlank(
      formData,
      "Empleado.EstadoCivil",
      self.EstadoCivil(),
    );
    self._appendIfNotBlank(formData, "Empleado.Direccion", self.Direccion());
    self._appendIfNotBlank(
      formData,
      "Empleado.TipoContrato",
      self.TipoContrato(),
    );
    self._appendIfNotBlank(formData, "Empleado.Salario", self.Salario());
    self._appendIfNotBlank(
      formData,
      "Empleado.TelefonoClinica",
      self.TelefonoClinica(),
    );

    if (
      self.SucursalId() !== undefined &&
      self.SucursalId() !== null &&
      self.SucursalId() !== ""
    ) {
      formData.append("Empleado.SucursalId", self.SucursalId());
    }

    self._appendIfNotBlank(
      formData,
      "Empleado.TipoEmpleado",
      self.TipoEmpleado(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.ColorHexadecimalFondo",
      self.ColorHexadecimalFondo(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.ColorHexadecimalTexto",
      self.ColorHexadecimalTexto(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.Observaciones",
      self.Observaciones(),
    );

    try {
      if (typeof showLoading === "function") showLoading();
    } catch (_) { }

    fetch("/Empleado/Nuevo", { method: "POST", body: formData })
      .then(function (response) {
        return response
          .json()
          .catch(function () {
            return null;
          })
          .then(function (data) {
            return { ok: response.ok, status: response.status, data: data };
          });
      })
      .then(function (r) {
        try {
          if (typeof hideLoading === "function") hideLoading();
        } catch (_) { }

        if (r.ok && r.data && r.data.success) {
          window.location.href = "/Empleado/Lista?tipoEmpleado=Normal";
          return;
        }

        var msg =
          (r.data && (r.data.message || r.data.mensaje)) ||
          "No se pudo guardar el registro.";
        self.mostrarAlerta(msg, "danger");
        console.error("Error al guardar el empleado:", r);
      })
      .catch(function (error) {
        try {
          if (typeof hideLoading === "function") hideLoading();
        } catch (_) { }

        self.mostrarAlerta(
          error && error.message
            ? error.message
            : "Ocurrió un error en la solicitud.",
          "danger",
        );
        console.error("Error en la solicitud:", error);
      });
  };

  // Médico
  self.agregarEmpleadoMedico = function () {
    self.ocultarAlerta();

    self.TipoEmpleadoFormulario("Medico");

    var faltantes = self.validarEmpleadoMedico();
    if (faltantes.length > 0) {
      self.mostrarAlerta(
        "Faltan campos obligatorios: " + faltantes.join(", "),
        "danger",
      );
      return;
    }

    var formData = new FormData();

    self._appendRequired(formData, "TipoEmpleadoFormulario", "Medico");

    self._appendRequired(formData, "Empleado.Nombre", self.Nombre());
    self._appendRequired(formData, "Empleado.Apellido", self.Apellido());
    self._appendRequired(formData, "Empleado.Telefono", self.Telefono());
    self._appendRequired(formData, "Empleado.Dpi", self.Dpi());
    self._appendRequired(formData, "Empleado.Nit", self.Nit());
    self._appendRequired(
      formData,
      "Empleado.FechaInicioLabores",
      self.FechaInicioLabores(),
    );
    self._appendRequired(
      formData,
      "Empleado.VacacionesProgramadas",
      self.VacacionesProgramadas(),
    );
    self._appendRequired(
      formData,
      "Empleado.JornadaTrabajo",
      self.JornadaTrabajo(),
    );

    // ✅ Unidad/Sección (opcionales)
    self._appendIfNotBlank(
      formData,
      "Empleado.UnidadOrgId",
      self.UnidadOrgId(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.SeccionOrgId",
      self.SeccionOrgId(),
    );

    self._appendRequired(
      formData,
      "EspecialidadIdSeleccionada",
      self.EspecialidadIdSeleccionada(),
    );

    if (self.FirmaEmpleadoImagen()) {
      formData.append("FirmaEmpleadoImagen", self.FirmaEmpleadoImagen());
    }

    self._appendIfNotBlank(formData, "Empleado.Telefono_2", self.Telefono_2());
    self._appendIfNotBlank(formData, "Empleado.Email", self.Email());
    self._appendIfNotBlank(formData, "Empleado.Edad", self.Edad());
    self._appendIfNotBlank(
      formData,
      "Empleado.EstadoCivil",
      self.EstadoCivil(),
    );
    self._appendIfNotBlank(formData, "Empleado.Direccion", self.Direccion());
    self._appendIfNotBlank(
      formData,
      "Empleado.TipoContrato",
      self.TipoContrato(),
    );
    self._appendIfNotBlank(formData, "Empleado.Salario", self.Salario());

    if (
      self.SucursalId() !== undefined &&
      self.SucursalId() !== null &&
      self.SucursalId() !== ""
    ) {
      formData.append("Empleado.SucursalId", self.SucursalId());
    }

    self._appendIfNotBlank(
      formData,
      "Empleado.ColorHexadecimalFondo",
      self.ColorHexadecimalFondo(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.ColorHexadecimalTexto",
      self.ColorHexadecimalTexto(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.Observaciones",
      self.Observaciones(),
    );

    // ✅ TipoEmpleado hardcoded según tu lógica
    formData.append("Empleado.TipoEmpleado", "Profesional");

    self._appendIfNotBlank(formData, "Empleado.Colegiado", self.Colegiado());
    self._appendIfNotBlank(formData, "Empleado.Genero", self.Genero());
    self._appendIfNotBlank(formData, "Empleado.Residente", self.Residente());
    self._appendIfNotBlank(
      formData,
      "Empleado.Credenciales",
      self.Credenciales(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.DireccionClinica",
      self.DireccionClinica(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.TelefonoClinica",
      self.TelefonoClinica(),
    );

    self._appendIfNotBlank(formData, "Empleado.TipoBanco", self.TipoBanco());
    self._appendIfNotBlank(formData, "Empleado.TipoCuenta", self.TipoCuenta());
    self._appendIfNotBlank(
      formData,
      "Empleado.NumeroCuenta",
      self.NumeroCuenta(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.NombreCuenta",
      self.NombreCuenta(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.NitPropietarioCuenta",
      self.NitPropietarioCuenta(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.NombrePropietarioNit",
      self.NombrePropietarioNit(),
    );
    self._appendIfNotBlank(
      formData,
      "Empleado.TipoRegimen",
      self.TipoRegimen(),
    );

    try {
      if (typeof showLoading === "function") showLoading();
    } catch (_) { }

    fetch("/Empleado/Nuevo", { method: "POST", body: formData })
      .then(function (response) {
        return response
          .json()
          .catch(function () {
            return null;
          })
          .then(function (data) {
            return { ok: response.ok, status: response.status, data: data };
          });
      })
      .then(function (r) {
        try {
          if (typeof hideLoading === "function") hideLoading();
        } catch (_) { }

        if (r.ok && r.data && r.data.success) {
          window.location.href = "/Empleado/Lista?tipoEmpleado=Medico";
          return;
        }

        var msg =
          (r.data && (r.data.message || r.data.mensaje)) ||
          "No se pudo guardar el registro.";
        self.mostrarAlerta(msg, "danger");
        console.error("Error al guardar el médico:", r);
      })
      .catch(function (error) {
        try {
          if (typeof hideLoading === "function") hideLoading();
        } catch (_) { }

        self.mostrarAlerta(
          error && error.message
            ? error.message
            : "Ocurrió un error en la solicitud.",
          "danger",
        );
        console.error("Error en la solicitud:", error);
      });
  };

  // ==========================
  // ✅ Init (cargar deptos al inicio)
  // ==========================
  self.init = function () {
    self.cargarDepartamentosOrg();
    // Unidades/Secciones se cargarán por cascada cuando selecciones depto/unidad
  };

  self.init();
}

ko.applyBindings(new NuevoEmpleadoViewModel());
