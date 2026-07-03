var FormConsentimientoVM = function () {
    var self = this;

    // Observables - Datos del Paciente
    self.horaIngreso = ko.observable(new Date().toLocaleString());
    self.numeroPaciente = ko.observable("");
    self.habitacionId = ko.observable("");
    self.numeroHabitacion = ko.observable("");
    self.nombreCompleto = ko.observable("");
    self.estadoCivil = ko.observable("");
    self.dpi = ko.observable("");
    self.fechaNacimiento = ko.observable("");
    self.edad = ko.observable("");
    self.nacionalidad = ko.observable("");
    self.direccion = ko.observable("");
    self.celular = ko.observable("");
    self.email = ko.observable("");
    self.tipoSangre = ko.observable("");
    self.genero = ko.observable("");
    self.religion = ko.observable("");
    self.ocupacion = ko.observable("");
    self.poseeSeguroMedico = ko.observable("false");
    self.aseguradora = ko.observable("");
    self.tipoPoliza = ko.observable("");
    self.nombreEmpresa = ko.observable("");
    self.formularioPreAutorizacion = ko.observable("false");
    self.tratamientoMedico = ko.observable("Necesario");

    // Computed observables para controlar el estado disable
    self.aseguradoraDisabled = ko.computed(function () {
        return self.poseeSeguroMedico() === "false"; // Si "poseeSeguroMedico" es "false", el campo se deshabilita
    });

    self.tipoPolizaDisabled = ko.computed(function () {
        return self.poseeSeguroMedico() === "false";
    });

    self.nombreEmpresaDisabled = ko.computed(function () {
        return self.poseeSeguroMedico() === "false";
    });

    self.formularioPreAutorizacionDisabled = ko.computed(function () {
        return self.poseeSeguroMedico() === "false";
    });

    // Cálculo de Edad
    self.fechaNacimiento.subscribe(function (newValue) {
        var birthDate = new Date(newValue);
        var age = new Date().getFullYear() - birthDate.getFullYear();
        self.edad(age);
    });

    // Observables - Datos del Responsable de la Cuenta
    self.nombreResponsable = ko.observable("");
    self.dpiResponsable = ko.observable("");

    self.fechaNacimientoResponsable = ko.observable("")
    self.edadResponsable = ko.observable("");

    self.direccionResponsable = ko.observable("");
    self.celularResponsable = ko.observable("");
    self.emailResponsable = ko.observable("");
    self.nitResponsable = ko.observable("");
    self.nombreFacturacion = ko.observable("");
    self.nacionalidadResponsable = ko.observable("");
    self.ocupacionResponsable = ko.observable("");

    // Cálculo de Edad Responsable
    self.fechaNacimientoResponsable.subscribe(function (newValue) {
        var birthDate = new Date(newValue);
        var age = new Date().getFullYear() - birthDate.getFullYear();
        self.edadResponsable(age);
    });

    // Observables - Contacto de Emergencia
    // self.nombreContactoEmergencia = ko.observable("");
    // self.celularContactoEmergencia = ko.observable("");
    // self.parentescoContactoEmergencia = ko.observable("");

    // Observables - Información adicional
    self.hospitalProporcionoMedico = ko.observable("false");
    self.medicoAfiliado = ko.observable("false");
    self.nombreMedico = ko.observable("No suministrado");
    self.recetaMedica = ko.observable("false");
    // NUEVOS OBSERVABLES PARA EL MÉDICO
    self.especialidadMedico = ko.observable("");
    self.colegiadoMedico = ko.observable("");
    self.urlFirmaMedico = ko.observable("");
    self.recetaMedica = ko.observable("false");

    // Observables - Firmas y nombres
    self.urlFirmaPaciente = ko.observable("");
    self.urlFirmaResponsable = ko.observable("");
    self.nombreNotaria = ko.observable("");
    self.nombreRepresentanteNarajo = ko.observable("");
    self.urlFirmaNotaria = ko.observable("");
    self.urlFirmaRepresentanteNaranjo = ko.observable("");

    self.contactosEmergencia = ko.observableArray([]);
    self.copyFromPatient = ko.observable(false);



    // Función para auto-completar los datos del médico usando la URL
    self.obtenerDatosMedicoAsignado = function () {
        const urlParams = new URLSearchParams(window.location.search);
        const consultaId = urlParams.get('ConsultaId');
        const citaId = urlParams.get('CitaId');

        // Solo hacemos la búsqueda si tenemos al menos uno de los IDs
        if (citaId || consultaId) {
            $.ajax({
                method: "GET",
                // NOTA: Deberás cambiar esta URL por la ruta real de tu controlador en C# que busca al médico
                url: '/Citas/ObtenerDatosMedicoPorCita',
                data: { citaId: citaId, consultaId: consultaId },
                success: function (data) {
                    if (data.exitoso) {
                        // Llenamos el campo del formulario para que el usuario lo vea
                        self.nombreMedico(data.nombreMedico);

                        // Guardamos la especialidad y la firma de forma oculta para usarlos luego
                        self.especialidadMedico(data.especialidad);
                        self.urlFirmaMedico(data.urlFirma);

                        console.log("Datos del médico cargados exitosamente.");
                    } else {
                        console.warn("No se encontró el médico para esta cita.");
                    }
                },
                error: function () {
                    console.error("Error de conexión al intentar obtener los datos del médico.");
                }
            });
        }
    };

    // Modal y firma del paciente
    self.abrirModalAgregarFirmaPaciente = function () {
        lineas = [];

        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open').css('padding-right', '');

        var $modal = $('#mdl-firma-paciente');

        if ($modal.data('bs.modal')) {
            $modal.data('bs.modal', null);
        }

        if (!$modal.parent().is('body')) {
            $modal.appendTo("body");
        }

        $modal.modal({
            backdrop: false,
            keyboard: true,
            show: true
        });

        $modal.one('shown.bs.modal', function () {
            renderFirma('canvas-paciente');
        });
    };

    self.guardarFirmaRegistro = function (rutaFirma) {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Pacientes/ActualizarFirmaRegistro',
            data: {
                pacienteId: parseInt(self.numeroPaciente(), 10),
                rutaFirma: rutaFirma
            },
            success: function (data) {
                if (data.exitoso) {
                    // alert("La firma del paciente se actualizo correctamente");
                } else {
                    hideLoading();
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };


    self.subirFirmaPaciente = function () {
        var canvas = document.getElementById('canvas-paciente');
        if (!canvas) return;

        var dataBase64 = canvas.toDataURL("image/png");

        showLoading();
        $.ajax({
            method: "POST",
            url: '/Files/SubirArchivo',
            data: {
                base64Archivo: dataBase64,
                extension: ".png"
            },
            success: function (data) {
                hideLoading();
                if (data.exitoso) {
                    self.urlFirmaPaciente(data.url);

                    limpiarFirma('canvas-paciente');

                    $("#mdl-firma-paciente").modal("hide");

                    const boton = document.getElementById("btn-eliminar-firma-paciente");
                    if (boton) {
                        boton.style.display = "";
                    }
                } else {
                    alert(data.mensaje);
                }
            },
            error: function (xhr) {
                hideLoading();
                alert("Error al subir la firma transparente");
            }
        });
    };

    // Modal y firma del responsable de la cuenta
    self.abrirModalAgregarFirmaResponsable = function () {
        lineas = [];

        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open').css('padding-right', '');

        var $modal = $('#mdl-firma-responsable');

        if ($modal.data('bs.modal')) {
            $modal.data('bs.modal', null);
        }

        if (!$modal.parent().is('body')) {
            $modal.appendTo("body");
        }

        $modal.modal({
            backdrop: false,
            keyboard: true,
            show: true
        });

        $modal.one('shown.bs.modal', function () {
            renderFirma('canvas-responsable');
        });
    };
    self.subirFirmaResponsable = function () {
        var canvas = document.getElementById('canvas-responsable');
        if (!canvas) return;

        var dataBase64 = canvas.toDataURL("image/png");

        showLoading();
        $.ajax({
            method: "POST",
            url: '/Files/SubirArchivo',
            data: {
                base64Archivo: dataBase64,
                extension: ".png"
            },
            success: function (data) {
                hideLoading();
                if (data.exitoso) {
                    self.urlFirmaResponsable(data.url);

                    limpiarFirma('canvas-responsable');

                    $("#mdl-firma-responsable").modal("hide");

                    const boton = document.getElementById("btn-eliminar-firma-responsable");
                    if (boton) {
                        boton.style.display = "";
                    }
                } else {
                    alert(data.mensaje);
                }
            },
            error: function (xhr) {
                hideLoading();
                alert("Error al subir la firma transparente");
            }
        });
    };
    self.abrirModalAgregarFirmaNotaria = function () {
        lineas = [];

        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open').css('padding-right', '');

        var $modal = $('#mdl-firma-notaria');

        if ($modal.data('bs.modal')) {
            $modal.data('bs.modal', null);
        }

        if (!$modal.parent().is('body')) {
            $modal.appendTo("body");
        }

        $modal.modal({
            backdrop: false,
            keyboard: true,
            show: true
        });

        $modal.one('shown.bs.modal', function () {
            renderFirma('canvas-notaria');
        });
    };


    self.subirFirmaNotaria = function () {
        var canvas = document.getElementById('canvas-notaria');
        if (!canvas) return;

        var dataBase64 = canvas.toDataURL("image/png");

        showLoading();
        $.ajax({
            method: "POST",
            url: '/Files/SubirArchivo',
            data: {
                base64Archivo: dataBase64,
                extension: ".png"
            },
            success: function (data) {
                hideLoading();
                if (data.exitoso) {
                    self.urlFirmaNotaria(data.url);

                    limpiarFirma('canvas-notaria');

                    $("#mdl-firma-notaria").modal("hide");

                    const boton = document.getElementById("btn-eliminar-firma-notaria");
                    if (boton) {
                        boton.style.display = "";
                    }
                } else {
                    alert(data.mensaje);
                }
            },
            error: function (xhr) {
                hideLoading();
                alert("Error al subir la firma transparente");
            }
        });
    };

    self.abrirModalAgregarFirmaRepresentanteNaranjo = function () {
        lineas = [];

        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open').css('padding-right', '');

        var $modal = $('#mdl-firma-representante');

        if ($modal.data('bs.modal')) {
            $modal.data('bs.modal', null);
        }

        if (!$modal.parent().is('body')) {
            $modal.appendTo("body");
        }

        $modal.modal({
            backdrop: false,
            keyboard: true,
            show: true
        });

        $modal.one('shown.bs.modal', function () {
            renderFirma('canvas-representante');
        });
    };


    self.subirFirmaRepresentante = function () {
        var canvas = document.getElementById('canvas-representante');
        if (!canvas) return;

        var dataBase64 = canvas.toDataURL("image/png");

        showLoading();
        $.ajax({
            method: "POST",
            url: '/Files/SubirArchivo',
            data: {
                base64Archivo: dataBase64,
                extension: ".png"
            },
            success: function (data) {
                hideLoading();
                if (data.exitoso) {
                    self.urlFirmaRepresentanteNaranjo(data.url);

                    limpiarFirma('canvas-representante');

                    $("#mdl-firma-representante").modal("hide");

                    const boton = document.getElementById("btn-eliminar-firma-representante");
                    if (boton) {
                        boton.style.display = "";
                    }
                } else {
                    alert(data.mensaje);
                }
            },
            error: function (xhr) {
                hideLoading();
                alert("Error al subir la firma transparente");
            }
        });
    };

    // Función para verificar que los observables no están vacíos o nulos
    self.verificarCamposObligatorios = function () {
        var camposInvalidos = [];

        // Lista de observables a verificar
        var observables = [
            // self.horaIngreso,
            // self.numeroPaciente,
            // self.numeroHabitacion,
            self.nombreCompleto,
            // self.estadoCivil,
            // self.dpi,
            // self.fechaNacimiento,
            // self.edad,
            // self.nacionalidad,
            // self.direccion,
            // self.celular,
            // self.email,
            // self.tipoSangre,
            // self.genero,
            // self.religion,
            // self.ocupacion,
            // self.poseeSeguroMedico,
            // self.aseguradora,
            // self.tipoPoliza,
            // self.nombreEmpresa,
            // self.formularioPreAutorizacion,
            // self.tratamientoMedico,
            // self.nombreResponsable,
            // self.dpiResponsable,
            // self.fechaNacimientoResponsable,
            // self.edadResponsable,
            // self.direccionResponsable,
            // self.celularResponsable,
            // self.emailResponsable,
            // self.nitResponsable,
            // self.nombreFacturacion,
            // self.nombreContactoEmergencia,
            // self.celularContactoEmergencia,
            // self.parentescoContactoEmergencia,
            // self.hospitalProporcionoMedico,
            // self.medicoAfiliado,
            // self.nombreMedico,
            // self.recetaMedica,
            // self.urlFirmaPaciente,
            // self.urlFirmaResponsable,
            // self.nombreNotaria,
            // self.nombreRepresentanteNarajo,
            // self.urlFirmaNotaria,
            // self.urlFirmaRepresentanteNaranjo
        ];

        // Mapeo de nombres de observables
        var nombresObservables = {
            // horaIngreso: "Hora de Ingreso",
            // numeroPaciente: "# Paciente",
            // numeroHabitacion: "# Habitación",
            nombreCompleto: "Nombre Completo",
            // estadoCivil: "Estado Civil",
            // dpi: "DPI o Pasaporte",
            // fechaNacimiento: "Fecha de Nacimiento",
            // edad: "Edad - Años",
            // nacionalidad: "Nacionalidad",
            // direccion: "Dirección",
            // celular: "Celular",
            // email: "Email",
            // tipoSangre: "Tipo de Sangre",
            // genero: "Género",
            // religion: "Religión",
            // ocupacion: "Ocupación",
            // poseeSeguroMedico: "¿Posee Seguro Médico?",
            // aseguradora: "Aseguradora",
            // tipoPoliza: "Tipo de Póliza",
            // nombreEmpresa: "Nombre de la Empresa",
            // formularioPreAutorizacion: "Formulario de Pre-autorización",
            // tratamientoMedico: "Tratamiento Médico",
            // nombreResponsable: "Nombre Completo del Responsable",
            // dpiResponsable: "DPI o Pasaporte del Responsable",
            // fechaNacimientoResponsable: "Fecha de Nacimiento del Responsable",
            // edadResponsable: "Edad - Años del Responsable",
            // direccionResponsable: "Dirección del Responsable",
            // celularResponsable: "Celular del Responsable",
            // emailResponsable: "Email del Responsable",
            // nitResponsable: "NIT del Responsable",
            // nombreFacturacion: "Nombre para Facturación",
            // nombreContactoEmergencia: "Nombre del Contacto de Emergencia",
            // celularContactoEmergencia: "Celular del Contacto de Emergencia",
            // parentescoContactoEmergencia: "Parentesco del Contacto de Emergencia",
            // hospitalProporcionoMedico: "¿El hospital proporcionó médico tratante?",
            // medicoAfiliado: "¿Cuenta con médico tratante afiliado al hospital?",
            // nombreMedico: "Nombre del médico tratante",
            // recetaMedica: "¿Tiene receta médica por ingresos programados?",
            // nombreNotaria: "Nombre de la Notaría",
            // nombreRepresentanteNarajo: "Nombre del Representante del Hospital"
        };

        // Verificar cada observable
        for (var i = 0; i < observables.length; i++) {
            var observableKey = Object.keys(self).find(key => self[key] === observables[i]);
            var value = observables[i](); // Obtener el valor del observable

            // Verificar si el campo es de los relacionados con seguro médico y si poseeSeguroMedico es "false"
            if (observables[i] === self.aseguradora || observables[i] === self.tipoPoliza || observables[i] === self.nombreEmpresa) {
                if (self.poseeSeguroMedico() === "false") {
                    // Si poseeSeguroMedico es "false", los campos relacionados pueden estar vacíos
                    continue;
                }
            }

            // Verificar si el valor es nulo o una cadena vacía
            if (value === null || value === "" || (typeof value === "string" && value.trim() === "")) {
                var nombreObservable = nombresObservables[observableKey] || "Campo desconocido"; // Obtener el nombre del observable
                camposInvalidos.push(nombreObservable);
            }
        }

        return camposInvalidos;
    };


    // Función para guardar el formulario de consentimiento
    self.guardarFormConsentimientoHospi = function () {
        if (self.edadResponsable() < 18) {
            alert("El responsable de la cuenta debe ser mayor de edad.");
            return;
        }

        showLoading();

        // Verificar campos obligatorios
        var camposInvalidos = self.verificarCamposObligatorios();

        if (camposInvalidos.length > 0) {
            hideLoading();
            alert(
                "Por favor, complete todos los campos. Si algún campo no aplica, ingrese 'No aplica' en lugar de dejarlo vacío.\n\n" +
                "Campos faltantes:\n- " +
                camposInvalidos.join("\n- ")
            );
            return;
        }

        // 1. CAPTURAR LOS PARÁMETROS DE LA URL
        const urlParams = new URLSearchParams(window.location.search);
        const consultaIdDesdeUrl = urlParams.get('ConsultaId') || "";
        const citaIdDesdeUrl = urlParams.get('CitaId') || "";

        const datosConsentimiento =
        {
            PacienteId: parseInt(self.numeroPaciente(), 10),
            HabitacionId: parseInt(self.habitacionId(), 10),

            HospitalizacionId: "No se especifica",

            // Datos del Paciente
            HoraIngreso: self.horaIngreso(),
            NumeroPaciente: self.numeroPaciente(),
            NumeroHabitacion: self.numeroHabitacion(),
            NombreCompleto: self.nombreCompleto(),
            EstadoCivil: self.estadoCivil(),
            DPI: self.dpi(),
            FechaNacimiento: self.fechaNacimiento(),
            Edad: self.edad(),
            Nacionalidad: self.nacionalidad(),
            Direccion: self.direccion(),
            Celular: self.celular(),
            Email: self.email(),
            TipoSangre: self.tipoSangre(),
            Genero: self.genero(),
            Religion: self.religion(),
            Ocupacion: self.ocupacion(),

            // Información del seguro médico
            PoseeSeguroMedico: self.poseeSeguroMedico(),
            Aseguradora: self.aseguradora(),
            TipoPoliza: self.tipoPoliza(),
            NombreEmpresa: self.nombreEmpresa(),
            FormularioPreAutorizacion: self.formularioPreAutorizacion(),
            TratamientoMedico: self.tratamientoMedico(),

            // Datos del Responsable de la Cuenta
            NombreResponsable: self.nombreResponsable(),
            DPIResponsable: self.dpiResponsable(),
            EdadResponsable: self.edadResponsable(),
            DireccionResponsable: self.direccionResponsable(),
            CelularResponsable: self.celularResponsable(),
            EmailResponsable: self.emailResponsable(),
            NITResponsable: self.nitResponsable(),
            NombreFacturacion: self.nombreFacturacion(),
            NacionalidadResponsable: self.nacionalidadResponsable(),
            OcupacionResponsable: self.ocupacionResponsable(),

            // Contacto de Emergencia
            // NombreContactoEmergencia: self.nombreContactoEmergencia(),
            // CelularContactoEmergencia: self.celularContactoEmergencia(),
            // ParentescoContactoEmergencia: self.parentescoContactoEmergencia(),
            ContactosEmergencia: self.contactosEmergencia().map(function (contacto) {
                return {
                    Nombre: contacto.nombre(),
                    Telefono: contacto.celular(),
                    Parentesco: contacto.parentesco()
                };
            }),

            // Información Adicional
            HospitalProporcionoMedico: self.hospitalProporcionoMedico(),
            MedicoAfiliado: self.medicoAfiliado(),
            NombreMedicoTratante: self.nombreMedico(),
            RecetaMedica: self.recetaMedica(),

            // Firmas y nombres de quienes firman
            URLFirmaPaciente: self.urlFirmaPaciente(),
            URLFirmaResponsable: self.urlFirmaResponsable(),
            NombreNotaria: self.nombreNotaria(),
            NombreRepresentanteNarajo: self.nombreRepresentanteNarajo(),
            URLFirmaNotaria: self.urlFirmaNotaria(),
            URLFirmaRepresentanteNaranjo: self.urlFirmaRepresentanteNaranjo(),

            // 2. NUEVOS CAMPOS ENVIADOS AL BACKEND
            ConsultaId: consultaIdDesdeUrl ? parseInt(consultaIdDesdeUrl, 10) : null,
            CitaId: citaIdDesdeUrl ? parseInt(citaIdDesdeUrl, 10) : null
        }

        // return console.log(JSON.stringify(datosConsentimiento, null, 4));
        self.guardarFirmaRegistro(self.urlFirmaPaciente());
        console.log("Normal", datosConsentimiento);

        // Proceder con el guardado si todos los campos son válidos
        // Aquí iría el resto de la lógica para guardar el formulario, como la llamada AJAX
        $.ajax({
            method: "POST",
            url: '/ConsentimientoHospi/Nuevo',
            data: datosConsentimiento,
            success: function (data) {
                hideLoading();
                if (data.exitoso) {
                    // alert("Formulario guardado correctamente.");
                    generarPDFConsentimiento(parseInt(self.numeroPaciente(), 10), parseInt(self.habitacionId(), 10))
                    window.close()
                } else {
                    alert(data.mensaje);
                }
            },
            error: function () {
                hideLoading();
                alert("Hubo un error al guardar los datos. Ver consola para detalles.");
            }
        });
    };

    self.guardarFormConsentimientoConDefaults = function () {
        console.log("Guardando consentimiento con valores por defecto.");
        // if (self.edadResponsable() < 18) {
        //     alert("El responsable de la cuenta debe ser mayor de edad.");
        //     return;
        // }

        showLoading();

        // Verificar campos obligatorios
        var camposInvalidos = self.verificarCamposObligatorios();

        // if (camposInvalidos.length > 0) {
        //     hideLoading();
        //     alert(
        //         "Por favor, complete todos los campos. Si algún campo no aplica, ingrese 'No aplica' en lugar de dejarlo vacío.\n\n" +
        //         "Campos faltantes:\n- " +
        //         camposInvalidos.join("\n- ")
        //     );
        //     return;
        // }

        // 1. CAPTURAR LOS PARÁMETROS DE LA URL
        const urlParams = new URLSearchParams(window.location.search);
        const consultaIdDesdeUrl = urlParams.get('ConsultaId') || "";
        const citaIdDesdeUrl = urlParams.get('CitaId') || "";

        const datosConsentimiento =
        {
            PacienteId: parseInt(self.numeroPaciente(), 10),
            HabitacionId: parseInt(self.habitacionId(), 10),

            HospitalizacionId: "No se especifica",

            // Datos del Paciente
            HoraIngreso: self.horaIngreso(),
            NumeroPaciente: self.numeroPaciente(),
            NumeroHabitacion: self.numeroHabitacion(),
            NombreCompleto: self.nombreCompleto(),
            EstadoCivil: self.estadoCivil(),
            DPI: self.dpi(),
            FechaNacimiento: self.fechaNacimiento(),
            Edad: self.edad(),
            Nacionalidad: self.nacionalidad(),
            Direccion: self.direccion(),
            Celular: self.celular(),
            Email: self.email(),
            TipoSangre: self.tipoSangre(),
            Genero: self.genero(),
            Religion: self.religion(),
            Ocupacion: self.ocupacion(),

            // Información del seguro médico
            PoseeSeguroMedico: self.poseeSeguroMedico(),
            Aseguradora: self.aseguradora(),
            TipoPoliza: self.tipoPoliza(),
            NombreEmpresa: self.nombreEmpresa(),
            FormularioPreAutorizacion: self.formularioPreAutorizacion(),
            TratamientoMedico: self.tratamientoMedico(),

            // Datos del Responsable de la Cuenta
            NombreResponsable: self.nombreResponsable(),
            DPIResponsable: self.dpiResponsable(),
            EdadResponsable: self.edadResponsable(),
            DireccionResponsable: self.direccionResponsable(),
            CelularResponsable: self.celularResponsable(),
            EmailResponsable: self.emailResponsable(),
            NITResponsable: self.nitResponsable(),
            NombreFacturacion: self.nombreFacturacion(),
            NacionalidadResponsable: self.nacionalidadResponsable(),
            OcupacionResponsable: self.ocupacionResponsable(),

            // Contactos de Emergencia (múltiples)
            ContactosEmergencia: self.contactosEmergencia().map(function (contacto) {
                return {
                    Nombre: contacto.nombre(),
                    Telefono: contacto.celular(),
                    Parentesco: contacto.parentesco()
                };
            }),

            // Información Adicional
            HospitalProporcionoMedico: self.hospitalProporcionoMedico(),
            MedicoAfiliado: self.medicoAfiliado(),
            NombreMedicoTratante: self.nombreMedico(),
            RecetaMedica: self.recetaMedica(),

            // Firmas y nombres de quienes firman
            URLFirmaPaciente: self.urlFirmaPaciente(),
            URLFirmaResponsable: self.urlFirmaResponsable(),
            NombreNotaria: self.nombreNotaria(),
            NombreRepresentanteNarajo: self.nombreRepresentanteNarajo(),
            URLFirmaNotaria: self.urlFirmaNotaria(),
            URLFirmaRepresentanteNaranjo: self.urlFirmaRepresentanteNaranjo(),

            // 2. NUEVOS CAMPOS ENVIADOS AL BACKEND
            ConsultaId: consultaIdDesdeUrl ? parseInt(consultaIdDesdeUrl, 10) : null,
            CitaId: citaIdDesdeUrl ? parseInt(citaIdDesdeUrl, 10) : null
        }

        // return console.log(JSON.stringify(datosConsentimiento, null, 4));
        self.guardarFirmaRegistro(self.urlFirmaPaciente());
        console.log("Normal", datosConsentimiento)

        // Proceder con el guardado si todos los campos son válidos
        // Aquí iría el resto de la lógica para guardar el formulario, como la llamada AJAX
        $.ajax({
            method: "POST",
            url: '/ConsentimientoHospi/Nuevo',
            data: datosConsentimiento,
            success: function (data) {
                hideLoading();
                if (data.exitoso) {
                    // alert("Formulario guardado correctamente.");
                    generarPDFConsentimiento(parseInt(self.numeroPaciente(), 10), parseInt(self.habitacionId(), 10))
                    window.close()
                } else {
                    alert(data.mensaje);
                }
            },
            error: function () {
                hideLoading();
                alert("Hubo un error al guardar los datos. Ver consola para detalles.");
            }
        });
    };

    // Método para eliminar un contacto
    self.eliminarContactoEmergencia = function (contacto) {
        self.contactosEmergencia.remove(contacto);
    };

    // Método para agregar un nuevo contacto de emergencia vacío
    self.agregarContactoEmergencia = function () {
        self.contactosEmergencia.push({
            nombre: ko.observable(""),
            celular: ko.observable(""),
            parentesco: ko.observable("")
        });
    };

    self.copyFromPatient.subscribe(function (newValue) {
        if (newValue) {
            self.nombreResponsable(self.nombreCompleto());
            self.dpiResponsable(self.dpi());
            self.fechaNacimientoResponsable(self.fechaNacimiento());
            self.direccionResponsable(self.direccion());
            self.celularResponsable(self.celular());
            self.emailResponsable(self.email());
            self.nacionalidadResponsable(self.nacionalidad());
            self.ocupacionResponsable(self.ocupacion());
            self.nombreFacturacion(self.nombreCompleto());
        } else {
            self.nombreResponsable("");
            self.dpiResponsable("");
            self.fechaNacimientoResponsable("");
            self.edadResponsable(""); 
            self.direccionResponsable("");
            self.celularResponsable("");
            self.emailResponsable("");
            self.nacionalidadResponsable("");
            self.ocupacionResponsable("");
            self.nombreFacturacion("");
        }
    });

};

var formConsentimientoVM = new FormConsentimientoVM();

// Obtener los datos del viewModel desde el script JSON en el archivo Razor
var viewModelData = JSON.parse(document.getElementById("viewModelData").textContent);
// console.log(JSON.stringify(viewModelData, null, 4));

function assignToObservables() {
    for (var property in viewModelData) {
        if (viewModelData.hasOwnProperty(property)) {
            if (property.toLowerCase() === "contactosemergencia") continue;

            if (viewModelData[property] !== null && viewModelData[property] !== "") {
                if (formConsentimientoVM.hasOwnProperty(property)) {
                    formConsentimientoVM[property](viewModelData[property]);
                }
            }
        }
    }

    var contactosData = viewModelData.ContactosEmergencia || viewModelData.contactosEmergencia;
    if (contactosData && Array.isArray(contactosData)) {
        formConsentimientoVM.contactosEmergencia.removeAll();
        contactosData.forEach(function (contacto) {
            formConsentimientoVM.contactosEmergencia.push({
                nombre: ko.observable(contacto.Nombre || contacto.nombre || ""),
                celular: ko.observable(contacto.Celular || contacto.celular || contacto.Telefono || contacto.telefono || ""),
                parentesco: ko.observable(contacto.Parentesco || contacto.parentesco || "")
            });
        });
    }
    // Si no hay contactos, asegurar al menos uno vacío
    if (formConsentimientoVM.contactosEmergencia().length === 0) {
        formConsentimientoVM.agregarContactoEmergencia();
    }
}
// Asignamos los valores a los observables
assignToObservables();

// Aplicamos los bindings de Knockout.js
ko.applyBindings(formConsentimientoVM);

$(document).ready(function () {
    renderFirma("canvas-paciente");
    renderFirma("canvas-responsable");
    renderFirma("canvas-notaria");
    renderFirma("canvas-representante");
    formConsentimientoVM.obtenerDatosMedicoAsignado();
    $("#religion").select2({ theme: "bootstrap4", width: "100%" });

});



function limpiarCanvas(idCanvas) {
    const miCanvas = document.querySelector('#' + idCanvas);
    const ctx = miCanvas.getContext('2d');

    // Limpiar el canvas completamente
    ctx.clearRect(0, 0, miCanvas.width, miCanvas.height);

    // Limpiar las líneas almacenadas
    lineas = [];
}

function eliminarFirma(idFirma, idBoton) {
    // Identificadores de los tipos de firma
    const tiposFirmas = {
        "firma-paciente": "urlFirmaPaciente",
        "firma-responsable": "urlFirmaResponsable",
        "firma-notaria": "urlFirmaNotaria",
        "firma-representante": "urlFirmaRepresentanteNaranjo"
    };

    // Verificar si el identificador corresponde a un tipo de firma conocido
    if (tiposFirmas[idFirma]) {
        // Eliminar la firma correspondiente
        formConsentimientoVM[tiposFirmas[idFirma]]("");
        console.log(`La firma asociada a '${idFirma}' ha sido eliminada.`);

        // Ocultar el botón después de ejecutar correctamente
        const boton = document.getElementById(idBoton);
        if (boton) {
            boton.style.display = "none";
        }
    } else {
        // Retroalimentación para casos inválidos
        console.warn(`El identificador '${idFirma}' no corresponde a ningún tipo de firma válido.`);
    }
}

// function renderFirma(idCanvas) {
//     const miCanvas = document.querySelector('#' + idCanvas);
//     const ctx = miCanvas.getContext('2d');
//     let pintarLinea = false;

//     // Establecer dimensiones
//     miCanvas.width = 900;
//     miCanvas.height = 300;

//     // Corregir posición de la canvas
//     const posicion = miCanvas.getBoundingClientRect();
//     const correccionX = posicion.x;
//     const correccionY = posicion.y;

//     // Configuración de estilo
//     ctx.lineJoin = ctx.lineCap = 'round';
//     ctx.lineWidth = 3;
//     ctx.strokeStyle = '#000000';

//     // Función para manejar el inicio de dibujo
//     function empezarDibujo(event) {
//         pintarLinea = true;
//         lineas.push([]);
//     }

//     // Función para manejar el movimiento del ratón o toque
//     function dibujarLinea(event) {
//         if (pintarLinea) {
//             const nuevaPosicionX = event.changedTouches ? event.changedTouches[0].pageX - correccionX : event.offsetX;
//             const nuevaPosicionY = event.changedTouches ? event.changedTouches[0].pageY - correccionY : event.offsetY;

//             // Guardar la línea
//             lineas[lineas.length - 1].push({ x: nuevaPosicionX, y: nuevaPosicionY });

//             // Redibujar todo el canvas
//             ctx.clearRect(0, 0, miCanvas.width, miCanvas.height);  // Limpiar canvas para no sobrecargarlo
//             ctx.beginPath();
//             lineas.forEach(function (segmento) {
//                 ctx.moveTo(segmento[0].x, segmento[0].y);
//                 segmento.forEach(function (punto) {
//                     ctx.lineTo(punto.x, punto.y);
//                 });
//             });
//             ctx.stroke();
//         }
//     }

//     // Función para manejar la finalización del dibujo
//     function detenerDibujo() {
//         pintarLinea = false;
//     }

//     // Eventos de ratón
//     miCanvas.addEventListener('mousedown', empezarDibujo, false);
//     miCanvas.addEventListener('mousemove', dibujarLinea, false);
//     miCanvas.addEventListener('mouseup', detenerDibujo, false);

//     // Eventos táctiles
//     miCanvas.addEventListener('touchstart', empezarDibujo, false);
//     miCanvas.addEventListener('touchmove', function (event) {
//         event.preventDefault();  // Evitar el desplazamiento de la pantalla
//         dibujarLinea(event);
//     }, false);
//     miCanvas.addEventListener('touchend', detenerDibujo, false);
// }

let lineas = []; // Variable global para los trazos

function renderFirma(idCanvas) {
    const miCanvas = document.getElementById(idCanvas);
    if (!miCanvas) return;
    const ctx = miCanvas.getContext('2d');
    let pintarLinea = false;

    const contenedor = miCanvas.parentElement;
    miCanvas.width = contenedor.clientWidth;
    miCanvas.height = 250;

    ctx.lineJoin = ctx.lineCap = 'round';
    ctx.lineWidth = 3;
    ctx.strokeStyle = '#000000';
    ctx.fillStyle = "white";
    ctx.fillRect(0, 0, miCanvas.width, miCanvas.height);

    function obtenerPos(e) {
        const rect = miCanvas.getBoundingClientRect();
        const clientX = e.touches ? e.touches[0].clientX : e.clientX;
        const clientY = e.touches ? e.touches[0].clientY : e.clientY;
        return {
            x: clientX - rect.left,
            y: clientY - rect.top
        };
    }

    function empezar(e) {
        pintarLinea = true;
        const p = obtenerPos(e);
        lineas.push([{ x: p.x, y: p.y }]);
    }

    function mover(e) {
        if (!pintarLinea) return;
        if (e.cancelable) e.preventDefault();

        const p = obtenerPos(e);
        lineas[lineas.length - 1].push({ x: p.x, y: p.y });

        ctx.clearRect(0, 0, miCanvas.width, miCanvas.height);
        ctx.beginPath();
        lineas.forEach(seg => {
            if (seg.length > 0) {
                ctx.moveTo(seg[0].x, seg[0].y);
                seg.forEach(pto => ctx.lineTo(pto.x, pto.y));
            }
        });
        ctx.stroke();
    }

    function parar() { pintarLinea = false; }

    miCanvas.onmousedown = empezar;
    miCanvas.onmousemove = mover;
    window.onmouseup = parar;

    miCanvas.addEventListener('touchstart', empezar, { passive: false });
    miCanvas.addEventListener('touchmove', mover, { passive: false });
    miCanvas.addEventListener('touchend', parar);
}

function limpiarFirma(idCanvas) {
    const miCanvas = document.getElementById(idCanvas);
    if (miCanvas) {
        const ctx = miCanvas.getContext('2d');
        ctx.clearRect(0, 0, miCanvas.width, miCanvas.height);
        if (typeof lineas !== 'undefined') {
            lineas = [];
        }
    }
}

function generarPDFConsentimiento(idPaciente, idHabitacion, idHospi = null) {
    // 1. CAPTURAR LOS PARÁMETROS DE LA URL
    const urlParams = new URLSearchParams(window.location.search);
    const consultaIdDesdeUrl = urlParams.get('ConsultaId') || "";
    const citaIdDesdeUrl = urlParams.get('CitaId') || "";

    const url = `/CrearPDF/GenerarPDFConsentimientoHospi?idPaciente=${idPaciente}&idHabitacion=${idHabitacion}&idHospi=${idHospi}&ConsultaId=${consultaIdDesdeUrl}&CitaId=${citaIdDesdeUrl}`;

    window.open(url);
}