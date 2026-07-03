(function () {
  "use strict";

  function ModificarEmpleadoViewModel(root) {
    var self = this;

    // ==========================
    // Alertas UI
    // ==========================
    self.AlertaVisible = ko.observable(false);
    self.AlertaMensaje = ko.observable("");
    self.AlertaTipo = ko.observable("danger");

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
    // Helpers DOM (precarga)
    // ==========================
    function getElByName(name) {
      return root ? root.querySelector('[name="' + name + '"]') : null;
    }

    function getVal(name, fallback) {
      var el = getElByName(name);
      if (!el) return fallback !== undefined ? fallback : "";
      return el.value !== undefined
        ? el.value
        : fallback !== undefined
          ? fallback
          : "";
    }

    function getValInt(name) {
      var v = getVal(name, "");
      if (v === "" || v === null || v === undefined) return null;
      var n = parseInt(v, 10);
      return isNaN(n) ? null : n;
    }

    function str(v) {
      return v === null || v === undefined ? "" : String(v);
    }

    function hasText(v) {
      return v !== null && v !== undefined && String(v).trim() !== "";
    }

    function toIntOrNull(v) {
      if (!hasText(v)) return null;
      var n = parseInt(String(v), 10);
      return isNaN(n) ? null : n;
    }

    function appendForce(fd, key, value) {
      fd.append(key, value === null || value === undefined ? "" : value);
    }

    function appendIfDomHasField(fd, key, value) {
      if (!getElByName(key)) return;
      appendForce(fd, key, value);
    }

    function safeShowLoading() {
      try {
        if (typeof showLoading === "function") showLoading();
      } catch (_) {}
    }

    function safeHideLoading() {
      try {
        if (typeof hideLoading === "function") hideLoading();
      } catch (_) {}
    }

    // ==========================
    // Observables (precarga REAL)
    // ==========================
    self.TipoEmpleadoFormulario = ko.observable(
      getVal("TipoEmpleadoFormulario", "Empleado"),
    );
    self.Id = ko.observable(getValInt("Empleado.Id"));

    self.Nombre = ko.observable(getVal("Empleado.Nombre", ""));
    self.Apellido = ko.observable(getVal("Empleado.Apellido", ""));
    self.Telefono = ko.observable(getVal("Empleado.Telefono", ""));
    self.Telefono_2 = ko.observable(getVal("Empleado.Telefono_2", ""));
    self.Email = ko.observable(getVal("Empleado.Email", ""));
    self.Edad = ko.observable(getVal("Empleado.Edad", ""));
    self.Dpi = ko.observable(getVal("Empleado.Dpi", ""));
    self.Nit = ko.observable(getVal("Empleado.Nit", ""));
    self.EstadoCivil = ko.observable(getVal("Empleado.EstadoCivil", ""));
    self.Direccion = ko.observable(getVal("Empleado.Direccion", ""));
    self.TipoContrato = ko.observable(getVal("Empleado.TipoContrato", ""));
    self.Salario = ko.observable(getVal("Empleado.Salario", ""));
    self.TelefonoClinica = ko.observable(
      getVal("Empleado.TelefonoClinica", ""),
    );
    self.SucursalId = ko.observable(getVal("Empleado.SucursalId", ""));
    self.TipoEmpleado = ko.observable(getVal("Empleado.TipoEmpleado", ""));
    self.JornadaTrabajo = ko.observable(getVal("Empleado.JornadaTrabajo", ""));
    self.FechaInicioLabores = ko.observable(
      getVal("Empleado.FechaInicioLabores", ""),
    );

    // ==========================
    // Ubicación organizacional
    // ==========================
    self.DepartamentoId = ko.observable(null);

    self.DepartamentosOrg = ko.observableArray([]);
    self.UnidadesOrg = ko.observableArray([]);
    self.SeccionesOrg = ko.observableArray([]);

    self.UnidadOrgId = ko.observable(getVal("Empleado.UnidadOrgId", ""));
    self.SeccionOrgId = ko.observable(getVal("Empleado.SeccionOrgId", ""));

    function normalizeIntObservable(obs) {
      // normaliza valor inicial
      var init = toIntOrNull(obs());
      if (obs() !== init) obs(init);

      obs.subscribe(function (v) {
        var parsed = toIntOrNull(v);
        if (obs() !== parsed) obs(parsed);
      });
    }

    normalizeIntObservable(self.DepartamentoId);
    normalizeIntObservable(self.UnidadOrgId);
    normalizeIntObservable(self.SeccionOrgId);

    // ==========================
    // Vacaciones
    // ==========================
    self.VacacionesProgramadas = ko.observable(
      getVal("Empleado.VacacionesProgramadas", ""),
    );
    self.VacacionesProgramadasFinal = ko.observable(
      getVal("Empleado.VacacionesProgramadasFinal", ""),
    );

    self.FechaInicialVacaciones = self.VacacionesProgramadas;
    self.FechaFinalVacaciones = self.VacacionesProgramadasFinal;

    // Config
    self.ColorHexadecimalFondo = ko.observable(
      getVal("Empleado.ColorHexadecimalFondo", ""),
    );
    self.ColorHexadecimalTexto = ko.observable(
      getVal("Empleado.ColorHexadecimalTexto", ""),
    );
    self.Observaciones = ko.observable(getVal("Empleado.Observaciones", ""));
    self.FirmaEmpleadoImagen = ko.observable(null);

    // Médico
    self.Genero = ko.observable(getVal("Empleado.Genero", ""));
    self.Colegiado = ko.observable(getVal("Empleado.Colegiado", ""));
    self.Residente = ko.observable(getVal("Empleado.Residente", ""));
    self.Credenciales = ko.observable(getVal("Empleado.Credenciales", ""));
    self.DireccionClinica = ko.observable(
      getVal("Empleado.DireccionClinica", ""),
    );
    self.TipoBanco = ko.observable(getVal("Empleado.TipoBanco", ""));
    self.TipoCuenta = ko.observable(getVal("Empleado.TipoCuenta", ""));
    self.NumeroCuenta = ko.observable(getVal("Empleado.NumeroCuenta", ""));
    self.NombreCuenta = ko.observable(getVal("Empleado.NombreCuenta", ""));
    self.NitPropietarioCuenta = ko.observable(
      getVal("Empleado.NitPropietarioCuenta", ""),
    );
    self.NombrePropietarioNit = ko.observable(
      getVal("Empleado.NombrePropietarioNit", ""),
    );
    self.TipoRegimen = ko.observable(getVal("Empleado.TipoRegimen", ""));
    self.EspecialidadIdSeleccionada = ko.observable(
      getVal("EspecialidadIdSeleccionada", ""),
    );

    // self.loadFirmaEmpleado = function (element, event) {
    //   var file = event && event.target ? event.target.files[0] : null;
    //   self.FirmaEmpleadoImagen(file || null);
    // };

    // === LÓGICA DE FIRMA MODAL ===
    self.FirmaPrevisualizacion = ko.observable(getVal("FirmaEmpleadoPath", "")); // Carga la firma actual de BD si existe

    // Variables temporales mientras el modal está abierto
    self.FirmaTemporalBase64 = ko.observable(null);
    self.FirmaTemporalFile = ko.observable(null);

    // Exponemos el ViewModel globalmente para que el script del Canvas pueda hablar con él
    window.empleadoVm = self;

    self.prepararModalFirma = function () {
      window.resetearFirmaEmpleadoUI();
      window.abrirModalFirmaEmpleado();
    };

    // Esta función la llama el script del Canvas cuando dibujas o subes archivo
    self.setFirmaTemporal = function (base64, fileObj) {
      self.FirmaTemporalBase64(base64);
      self.FirmaTemporalFile(fileObj);

      // Activamos el botón de confirmar en el modal
      document.getElementById("btnConfirmarFirmaEmpleado").disabled = false;

      // Mostramos la previsualización dentro del modal
      var contenedorPrev = document.getElementById(
        "contenedorPrevFirmaEmpleado",
      );
      var imgPrev = document.getElementById("imgPrevFirmaEmpleado");
      var selector = document.getElementById("selectorFirmaEmpleado");

      if (base64) {
        imgPrev.src = base64;
      }
      selector.classList.add("d-none");
      contenedorPrev.classList.remove("d-none");
    };

    // self.prepararModalFirma = function () {
    //   window.resetearFirmaEmpleadoUI();
    //   window.abrirModalFirmaEmpleado();
    // };

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
        var newFile = new File([u8arr], "firma_canvas.png", { type: mime });
        self.FirmaEmpleadoImagen(newFile);
        self.FirmaPrevisualizacion(base64);
      }

      window.cerrarModalFirmaEmpleado();
    };

    // Única definición de prepararModalFirma (Revisada)
    self.prepararModalFirma = function () {
      // 1. Limpiamos el modal a su estado inicial
      window.resetearFirmaEmpleadoUI();

      // 2. Verificamos si ya hay una firma cargada desde BD
      var firmaActual = self.FirmaPrevisualizacion();

      if (firmaActual) {
        // Si existe, mostramos la imagen directamente
        var contenedorPrev = document.getElementById(
          "contenedorPrevFirmaEmpleado",
        );
        var imgPrev = document.getElementById("imgPrevFirmaEmpleado");
        var selector = document.getElementById("selectorFirmaEmpleado");

        imgPrev.src = firmaActual;
        selector.classList.add("d-none");
        contenedorPrev.classList.remove("d-none");

        // Activamos el botón confirmar
        document.getElementById("btnConfirmarFirmaEmpleado").disabled = false;
      }

      // 3. Mostramos el modal en pantalla
      window.abrirModalFirmaEmpleado();
    };
    // ============================
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
        var ultimoDiaMesAnterior = new Date(
          hoy.getFullYear(),
          hoy.getMonth(),
          0,
        ).getDate();
        dias += ultimoDiaMesAnterior;
        meses--;
        if (meses < 0) {
          meses += 12;
        }
      }

      var edadTexto = anios + " años, " + meses + " meses, " + dias + " días";
      self.Edad(edadTexto);
    });
    // ==========================
    // Catálogos Org (AJAX)
    // ==========================
    function fetchOrg(url) {
      return fetch(url, { method: "GET" })
        .then(function (r) {
          return r.json();
        })
        .then(function (json) {
          if (json && json.success) return json.data || [];
          var msg =
            (json && (json.message || json.mensaje)) ||
            "No se pudo cargar el catálogo.";
          throw new Error(msg);
        });
    }

    function cargarDepartamentosOrg() {
      return fetchOrg("/Empleado/DepartamentosOrg").then(function (data) {
        self.DepartamentosOrg(Array.isArray(data) ? data : []);
        return self.DepartamentosOrg();
      });
    }

    function cargarUnidadesOrg(departamentoOrgId) {
      var depId = toIntOrNull(departamentoOrgId);
      if (!depId) {
        self.UnidadesOrg([]);
        return Promise.resolve([]);
      }

      return fetchOrg(
        "/Empleado/UnidadesOrg?departamentoOrgId=" + encodeURIComponent(depId),
      ).then(function (data) {
        var arr = Array.isArray(data) ? data : [];
        self.UnidadesOrg(arr);
        return arr;
      });
    }

    function cargarSeccionesOrg(unidadOrgId) {
      var uId = toIntOrNull(unidadOrgId);
      if (!uId) {
        self.SeccionesOrg([]);
        return Promise.resolve([]);
      }

      return fetchOrg(
        "/Empleado/SeccionesOrg?unidadOrgId=" + encodeURIComponent(uId),
      ).then(function (data) {
        var arr = Array.isArray(data) ? data : [];
        self.SeccionesOrg(arr);
        return arr;
      });
    }

    function resolverDepartamentoDesdeUnidad(unidadOrgId) {
      var unidadId = toIntOrNull(unidadOrgId);
      if (!unidadId) return Promise.resolve(null);

      var deps = self.DepartamentosOrg();
      if (!Array.isArray(deps) || deps.length === 0)
        return Promise.resolve(null);

      var idx = 0;
      function next() {
        if (idx >= deps.length) return Promise.resolve(null);

        var dep = deps[idx++];
        var depId = toIntOrNull(dep && dep.id);
        if (!depId) return next();

        return fetchOrg(
          "/Empleado/UnidadesOrg?departamentoOrgId=" +
            encodeURIComponent(depId),
        )
          .then(function (unidades) {
            unidades = Array.isArray(unidades) ? unidades : [];
            var found = unidades.some(function (u) {
              return toIntOrNull(u && u.id) === unidadId;
            });
            if (found) return depId;
            return next();
          })
          .catch(function () {
            return next();
          });
      }

      return next();
    }

    // ==========================
    // Cascada UI (usuario)
    // ==========================
    var isOrgInit = true;

    self.DepartamentoId.subscribe(function (newDepId) {
      if (isOrgInit) return;

      var depId = toIntOrNull(newDepId);
      if (!depId) {
        self.UnidadesOrg([]);
        self.UnidadOrgId(null);
        self.SeccionesOrg([]);
        self.SeccionOrgId(null);
        return;
      }

      self.UnidadOrgId(null);
      self.SeccionOrgId(null);
      self.SeccionesOrg([]);

      cargarUnidadesOrg(depId).catch(function (err) {
        self.UnidadesOrg([]);
        self.mostrarAlerta(
          (err && err.message) || "Error al cargar unidades.",
          "danger",
        );
      });
    });

    self.UnidadOrgId.subscribe(function (newUnidadId) {
      if (isOrgInit) return;

      var uId = toIntOrNull(newUnidadId);
      if (!uId) {
        self.SeccionesOrg([]);
        self.SeccionOrgId(null);
        return;
      }

      self.SeccionOrgId(null);

      cargarSeccionesOrg(uId).catch(function (err) {
        self.SeccionesOrg([]);
        self.mostrarAlerta(
          (err && err.message) || "Error al cargar secciones.",
          "danger",
        );
      });
    });

    // ==========================
    // Sugerencia vacaciones
    // ==========================
    self.FechaInicioLabores.subscribe(function (newValue) {
      if (!newValue) return;

      if (
        hasText(self.VacacionesProgramadas()) ||
        hasText(self.VacacionesProgramadasFinal())
      ) {
        return;
      }

      try {
        var fecha = new Date(newValue);
        if (isNaN(fecha.getTime())) return;
        fecha.setFullYear(fecha.getFullYear() + 1);
        self.VacacionesProgramadas(fecha.toISOString().slice(0, 10));
      } catch (_) {}
    });

    // ==========================
    // Validaciones
    // ==========================
    self.validarEmpleado = function () {
      var faltantes = [];

      if (!hasText(self.Id())) faltantes.push("Id (Empleado)");
      if (!hasText(self.Nombre())) faltantes.push("Nombre");
      if (!hasText(self.Apellido())) faltantes.push("Apellido");
      if (!hasText(self.Telefono())) faltantes.push("Telefono");
      if (!hasText(self.Email())) faltantes.push("Email");
      if (!hasText(self.Dpi())) faltantes.push("Dpi");
      if (!hasText(self.Nit())) faltantes.push("Nit");
      if (!hasText(self.FechaInicioLabores()))
        faltantes.push("Fecha de Inicio de Labores");
      if (!hasText(self.VacacionesProgramadas()))
        faltantes.push("Vacaciones (Fecha Inicial)");
      if (!hasText(self.VacacionesProgramadasFinal()))
        faltantes.push("Vacaciones (Fecha Final)");
      if (!hasText(self.JornadaTrabajo())) faltantes.push("Jornada de Trabajo");

      return faltantes;
    };

    self.validarMedico = function () {
      var faltantes = self.validarEmpleado();
      
      if (!hasText(self.EspecialidadIdSeleccionada())) faltantes.push("Especialidad");
      if (!hasText(self.Credenciales())) faltantes.push("Fecha de Nacimiento");
      if (!hasText(self.Colegiado())) faltantes.push("Colegiado");

      // ✅ NUEVA VALIDACIÓN INTELIGENTE: Firma obligatoria
      // Verifica si NO hay un archivo nuevo Y TAMPOCO hay una firma previamente guardada
      if (!self.FirmaEmpleadoImagen() && !hasText(self.FirmaPrevisualizacion())) {
          faltantes.push("Firma del Médico");
      }

      return faltantes;
    };

    // ==========================
    // POST
    // ==========================
    function postModificar(formData, redirectTipoEmpleado) {
      safeShowLoading();

      return fetch("/Empleado/Modificar", { method: "POST", body: formData })
        .then(function (response) {
          var ct = (response.headers.get("content-type") || "").toLowerCase();

          if (ct.includes("application/json")) {
            return response
              .json()
              .catch(function () {
                return null;
              })
              .then(function (data) {
                return {
                  ok: response.ok,
                  status: response.status,
                  kind: "json",
                  data: data,
                };
              });
          }

          return response
            .text()
            .catch(function () {
              return "";
            })
            .then(function (text) {
              return {
                ok: response.ok,
                status: response.status,
                kind: "html",
                text: text,
                url: response.url,
              };
            });
        })
        .then(function (r) {
          safeHideLoading();

          if (r.kind === "json") {
            if (r.ok && r.data && r.data.success) {
              window.location.href =
                "/Empleado/Lista?tipoEmpleado=" + redirectTipoEmpleado;
              return;
            }

            var msg =
              (r.data && (r.data.message || r.data.mensaje)) ||
              "No se pudo guardar la modificación.";
            self.mostrarAlerta(msg, "danger");
            return;
          }

          if (r.ok) {
            window.location.href =
              "/Empleado/Lista?tipoEmpleado=" + redirectTipoEmpleado;
            return;
          }

          self.mostrarAlerta(
            "No se pudo guardar la modificación (respuesta del servidor).",
            "danger",
          );
        })
        .catch(function (error) {
          safeHideLoading();
          self.mostrarAlerta(
            error && error.message
              ? error.message
              : "Ocurrió un error inesperado al procesar la solicitud.",
            "danger",
          );
          console.error(error);
        });
    }

    // ==========================
    // Acciones
    // ==========================
    self.modificarEmpleado = function () {
      self.ocultarAlerta();
      self.TipoEmpleadoFormulario("Empleado");

      var faltantes = self.validarEmpleado();
      if (faltantes.length > 0) {
        self.mostrarAlerta(
          "Faltan campos obligatorios: " + faltantes.join(", "),
          "danger",
        );
        return;
      }

      var fd = new FormData();

      appendForce(fd, "TipoEmpleadoFormulario", self.TipoEmpleadoFormulario());
      appendForce(fd, "Empleado.Id", self.Id());

      appendForce(fd, "Empleado.Nombre", str(self.Nombre()));
      appendForce(fd, "Empleado.Apellido", str(self.Apellido()));
      appendForce(fd, "Empleado.Telefono", str(self.Telefono()));
      appendForce(fd, "Empleado.Telefono_2", str(self.Telefono_2()));
      appendForce(fd, "Empleado.Email", str(self.Email()));
      appendForce(fd, "Empleado.Edad", str(self.Edad()));
      appendForce(fd, "Empleado.Dpi", str(self.Dpi()));
      appendForce(fd, "Empleado.Nit", str(self.Nit()));
      appendForce(fd, "Empleado.EstadoCivil", str(self.EstadoCivil()));
      appendForce(fd, "Empleado.Direccion", str(self.Direccion()));
      appendForce(fd, "Empleado.TipoContrato", str(self.TipoContrato()));
      appendForce(fd, "Empleado.Salario", str(self.Salario()));
      appendForce(fd, "Empleado.TelefonoClinica", str(self.TelefonoClinica()));
      appendForce(fd, "Empleado.SucursalId", str(self.SucursalId()));
      appendForce(fd, "Empleado.TipoEmpleado", str(self.TipoEmpleado()));
      appendForce(fd, "Empleado.JornadaTrabajo", str(self.JornadaTrabajo()));
      appendForce(
        fd,
        "Empleado.FechaInicioLabores",
        str(self.FechaInicioLabores()),
      );

      appendForce(fd, "Empleado.UnidadOrgId", str(self.UnidadOrgId()));
      appendForce(fd, "Empleado.SeccionOrgId", str(self.SeccionOrgId()));

      appendForce(
        fd,
        "Empleado.VacacionesProgramadas",
        str(self.VacacionesProgramadas()),
      );
      appendForce(
        fd,
        "Empleado.VacacionesProgramadasFinal",
        str(self.VacacionesProgramadasFinal()),
      );

      appendForce(
        fd,
        "Empleado.ColorHexadecimalFondo",
        str(self.ColorHexadecimalFondo()),
      );
      appendForce(
        fd,
        "Empleado.ColorHexadecimalTexto",
        str(self.ColorHexadecimalTexto()),
      );
      appendForce(fd, "Empleado.Observaciones", str(self.Observaciones()));

      if (self.FirmaEmpleadoImagen()) {
        fd.append("FirmaEmpleadoImagen", self.FirmaEmpleadoImagen());
      }

      return postModificar(fd, "Normal");
    };

    self.modificarMedico = function () {
      self.ocultarAlerta();
      self.TipoEmpleadoFormulario("Medico");

      var faltantes = self.validarMedico();
      if (faltantes.length > 0) {
        self.mostrarAlerta(
          "Faltan campos obligatorios: " + faltantes.join(", "),
          "danger",
        );
        return;
      }

      var fd = new FormData();

      appendForce(fd, "TipoEmpleadoFormulario", self.TipoEmpleadoFormulario());
      appendForce(fd, "Empleado.Id", self.Id());

      appendForce(fd, "Empleado.Nombre", str(self.Nombre()));
      appendForce(fd, "Empleado.Apellido", str(self.Apellido()));
      appendForce(fd, "Empleado.Telefono", str(self.Telefono()));
      appendForce(fd, "Empleado.Telefono_2", str(self.Telefono_2()));
      appendForce(fd, "Empleado.Email", str(self.Email()));
      appendForce(fd, "Empleado.Edad", str(self.Edad()));
      appendForce(fd, "Empleado.Dpi", str(self.Dpi()));
      appendForce(fd, "Empleado.Nit", str(self.Nit()));
      appendForce(fd, "Empleado.Genero", str(self.Genero()));
      appendForce(fd, "Empleado.EstadoCivil", str(self.EstadoCivil()));
      appendForce(fd, "Empleado.Colegiado", str(self.Colegiado()));
      appendForce(fd, "Empleado.Direccion", str(self.Direccion()));
      appendForce(fd, "Empleado.Credenciales", str(self.Credenciales()));
      appendForce(fd, "Empleado.TipoContrato", str(self.TipoContrato()));
      appendForce(fd, "Empleado.Residente", str(self.Residente()));
      appendForce(fd, "Empleado.Salario", str(self.Salario()));
      appendForce(
        fd,
        "Empleado.DireccionClinica",
        str(self.DireccionClinica()),
      );
      appendForce(fd, "Empleado.TelefonoClinica", str(self.TelefonoClinica()));
      appendForce(fd, "Empleado.SucursalId", str(self.SucursalId()));
      appendForce(fd, "Empleado.JornadaTrabajo", str(self.JornadaTrabajo()));
      appendForce(
        fd,
        "Empleado.FechaInicioLabores",
        str(self.FechaInicioLabores()),
      );

      appendIfDomHasField(fd, "Empleado.UnidadOrgId", str(self.UnidadOrgId()));
      appendIfDomHasField(
        fd,
        "Empleado.SeccionOrgId",
        str(self.SeccionOrgId()),
      );

      appendForce(
        fd,
        "Empleado.VacacionesProgramadas",
        str(self.VacacionesProgramadas()),
      );
      appendForce(
        fd,
        "Empleado.VacacionesProgramadasFinal",
        str(self.VacacionesProgramadasFinal()),
      );

      appendForce(
        fd,
        "EspecialidadIdSeleccionada",
        str(self.EspecialidadIdSeleccionada()),
      );
      appendForce(fd, "Empleado.TipoEmpleado", "Profesional");

      appendForce(fd, "Empleado.TipoBanco", str(self.TipoBanco()));
      appendForce(fd, "Empleado.TipoCuenta", str(self.TipoCuenta()));
      appendForce(fd, "Empleado.NumeroCuenta", str(self.NumeroCuenta()));
      appendForce(fd, "Empleado.NombreCuenta", str(self.NombreCuenta()));
      appendForce(
        fd,
        "Empleado.NitPropietarioCuenta",
        str(self.NitPropietarioCuenta()),
      );
      appendForce(
        fd,
        "Empleado.NombrePropietarioNit",
        str(self.NombrePropietarioNit()),
      );
      appendForce(fd, "Empleado.TipoRegimen", str(self.TipoRegimen()));

      appendForce(
        fd,
        "Empleado.ColorHexadecimalFondo",
        str(self.ColorHexadecimalFondo()),
      );
      appendForce(
        fd,
        "Empleado.ColorHexadecimalTexto",
        str(self.ColorHexadecimalTexto()),
      );
      appendForce(fd, "Empleado.Observaciones", str(self.Observaciones()));

      if (self.FirmaEmpleadoImagen()) {
        fd.append("FirmaEmpleadoImagen", self.FirmaEmpleadoImagen());
      }

      return postModificar(fd, "Medico");
    };

    // ==========================
    // ✅ Init org: precarga determinística (SIN carreras)
    // ==========================
    function initUbicacionOrganizacional() {
      // Solo si existe el select de departamento en este form
      if (!root.querySelector('[data-bind*="DepartamentosOrg"]')) return;

      var unidadInicial = toIntOrNull(self.UnidadOrgId());
      var seccionInicial = toIntOrNull(self.SeccionOrgId());

      safeShowLoading();

      cargarDepartamentosOrg()
        .then(function () {
          if (!unidadInicial) return null;
          return resolverDepartamentoDesdeUnidad(unidadInicial);
        })
        .then(function (depId) {
          // Si no se pudo resolver y no hay unidad, dejamos en blanco
          if (!depId) return null;

          // Setea departamento y carga unidades
          self.DepartamentoId(depId);

          return cargarUnidadesOrg(depId).then(function () {
            if (unidadInicial) self.UnidadOrgId(unidadInicial);
            if (!unidadInicial) return null;

            return cargarSeccionesOrg(unidadInicial).then(function () {
              if (seccionInicial) self.SeccionOrgId(seccionInicial);
              return true;
            });
          });
        })
        .catch(function (err) {
          self.mostrarAlerta(
            (err && err.message) ||
              "Error al inicializar ubicación organizacional.",
            "danger",
          );
        })
        .finally(function () {
          // Ya terminó precarga; habilita cascada de usuario
          isOrgInit = false;
          safeHideLoading();
        });
    }

    initUbicacionOrganizacional();
  }

  // ==========================
  // Bootstrap: bind Global
  // ==========================
  function bootstrap() {
    // 1. Buscamos el formulario para que el script pueda leer los valores que trae la vista
    // Agregamos compatibilidad por si es el formulario de Empleado o el de Médico
    var root =
      document.getElementById("formModificarEmpleado") ||
      document.getElementById("formModificarMedico") ||
      document.querySelector("form");

    if (!root) return;

    var vm = new ModificarEmpleadoViewModel(root);

    // 2. APLICAMOS A TODA LA PÁGINA (Quitamos el parámetro 'root')
    // Esto permite que Knockout controle la Alerta y el Modal de la firma que están fuera del form
    ko.applyBindings(vm);
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", bootstrap);
  } else {
    bootstrap();
  }
})();