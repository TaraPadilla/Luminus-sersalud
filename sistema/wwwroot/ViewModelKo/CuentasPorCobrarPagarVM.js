var CuentaPagarVM = function () {
    let itemPago = 1;
    var model = {};
    var self = this;

    // -------------------------
    // Observables existentes
    // -------------------------
    self.montoPagoAgregar = ko.observable();
    self.totalPagos = ko.observable(0);
    self.saldoPagos = ko.observable(0);
    self.nombreMedico = ko.observable();

    self.pagos = ko.observableArray();

    // -------------------------
    // NUEVOS: Observables FEL/UI
    // -------------------------
    self.cuentaCerrada = ko.observable(false);

    // EstadosFEL: 0=NoIniciado, 1=Pendiente, 2=Emitida, 3=Error
    self.felEstado = ko.observable(0);
    self.felUltimoError = ko.observable("");
    self.felIntentos = ko.observable(0);
    self.uuidFel = ko.observable("");

    // -------------------------
    // NUEVOS: Observables Honorarios a BD
    // -------------------------
    self.honorariosLista = ko.observableArray([]);
    self.medicoSeleccionado = ko.observable();
    self.montoHonorarioNuevo = ko.observable();

    // -------------------------
    // Helpers
    // -------------------------
    function normalizeFelEstado(value) {
        // Back puede enviar int (0..3) o string ("NoIniciado"/"Pendiente"/"Emitida"/"Error")
        if (value === null || value === undefined) return 0;

        if (typeof value === "number") return value;

        const s = String(value).trim().toLowerCase();
        if (s === "noiniciado") return 0;
        if (s === "pendiente") return 1;
        if (s === "emitida") return 2;
        if (s === "error") return 3;

        // Si viene número en string
        const n = parseInt(s, 10);
        return isNaN(n) ? 0 : n;
    }

    function felEstadoToText(value) {
        const estado = normalizeFelEstado(value);
        switch (estado) {
            case 0:
                return "No iniciado";
            case 1:
                return "Pendiente";
            case 2:
                return "Emitida";
            case 3:
                return "Error";
            default:
                return "No iniciado";
        }
    }

    function getPdfUrl(uuid) {
        // Nota: abrir PDF por GET no requiere CORS; solo necesitas que el navegador permita popups.
        return `http://3.128.79.135/Xml/GeneratePDF?uuid=${encodeURIComponent(uuid)}`;
    }

    function tryOpenPdf(uuid) {
        if (!uuid) return;
        const pdfUrl = getPdfUrl(uuid);
        const pdfWindow = window.open(pdfUrl, "_blank");
        if (!pdfWindow) {
            alert("Permite la apertura de ventanas emergentes para mostrar el PDF.");
        }
    }

    function updateFelFromResponse(data) {
        // data puede venir con propiedades directas o anidadas según tu controller.
        // Soportamos varias formas sin asumir de más.
        const estado = normalizeFelEstado(data.FelEstado ?? data.felEstado ?? 0);
        const uuid = data.UuidFel ?? data.uuidFel ?? "";
        const err = data.FelUltimoError ?? data.felUltimoError ?? "";
        const intentos = data.FelIntentos ?? data.felIntentos ?? 0;

        self.felEstado(estado);
        self.uuidFel(uuid || "");
        self.felUltimoError(err || "");
        self.felIntentos(parseInt(intentos, 10) || 0);
    }

    // -------------------------
    // Navegación existente
    // -------------------------
    self.verInformacionPaciente = function () {
        window.open(
            "/Pacientes/Informacion?pacienteId=" + $("#PacienteId").val(),
            "_blank",
        );
    };

    // // Reemplazar completamente la función actualizarTotales
    // self.actualizarTotales = function () {
    //     // Sumar pagos NUEVOS (los que se están agregando ahora)
    //     let totalPagosNuevos = 0;
    //     $(self.pagos()).each(function (idx, pago) {
    //         const monto = parseFloat(pago.Monto) || 0;
    //         const esNuevo = pago.Nuevo === true || String(pago.Nuevo).toLowerCase() === "true";
    //         if (esNuevo) totalPagosNuevos += monto;
    //     });

    //     let totalPagadoAnterior = 0;

    //     $(self.pagos()).each(function (idx, pago) {
    //         const monto = parseFloat(pago.Monto) || 0;
    //         const esExistente = pago.Nuevo !== true && String(pago.Nuevo).toLowerCase() !== "true";
    //         if (esExistente) totalPagadoAnterior += monto;
    //     });

    //     // Total de cuenta (lo que debe pagar el paciente)
    //     let pagoPaciente = parseFloat($("#pagoPaciente").val()) || 0;

    //     let saldo = Math.max(pagoPaciente - totalPagosNuevos, 0);

    //     self.totalPagos(totalPagosNuevos);
    //     self.saldoPagos(saldo.toFixed(2));

    //     console.log('actualizarTotales - Debug:', {
    //         pagos: self.pagos(),
    //         totalPagosNuevos: totalPagosNuevos,
    //         totalPagadoAnterior: totalPagadoAnterior,
    //         pagoPaciente: pagoPaciente,
    //         saldoCalculado: saldo
    //     });
    // };
    self.actualizarTotales = function () {
        let totalPagosNuevos = 0;
        $(self.pagos()).each(function (idx, pago) {
            const monto = parseFloat(pago.Monto) || 0;
            const esNuevo = pago.Nuevo === true || String(pago.Nuevo).toLowerCase() === "true";
            if (esNuevo) totalPagosNuevos += monto;
        });

        // ✅ Usar el totalDeLaCuenta ajustado (ya incluye descuentos por item)
        // Si hay descuento global también, restarlo
        const totalAjustado = parseFloat(document.getElementById("totalDeLaCuenta").value) || 0;
        const montoDescuentoGlobal = parseFloat($("#montoDescuento").val()) || 0;
        const baseParaSaldo = Math.max(totalAjustado - montoDescuentoGlobal, 0);

        let saldo = Math.max(baseParaSaldo - totalPagosNuevos, 0);

        self.totalPagos(totalPagosNuevos);
        self.saldoPagos(saldo.toFixed(2));
    };

    self.obtenerTotalPagadoAnterior = function () {
        let total = 0;
        $(self.pagos()).each(function (idx, pago) {
            const esExistente = pago.Nuevo !== true && String(pago.Nuevo).toLowerCase() !== "true";
            if (esExistente) {
                total += parseFloat(pago.Monto) || 0;
            }
        });
        return total;
    };

    // -------------------------
    // Agregar / Eliminar pagos existentes
    // -------------------------
    self.agregarPago = function () {
        let monto = parseFloat(self.montoPagoAgregar()) || 0;

        if (monto <= 0) {
            alert("El monto debe ser mayor a 0.");
            return;
        }

        let pago = {
            Item: itemPago,
            FormaPagoId: $("#FormaPagoId").val(),
            FormaPago: $("#FormaPagoId option:selected").text(),
            MonedaId: $("#MonedaId").val(),
            Moneda: $("#MonedaId option:selected").text(),
            Monto: monto,
            Nuevo: true,
        };

        self.pagos.push(pago);
        itemPago++;
        self.actualizarTotales();
        self.montoPagoAgregar("");
    };

    self.eliminarPago = function (value) {
        $(self.pagos()).each(function (idx, pago) {
            if (value.Item == pago.Item) {
                self.pagos.splice(idx, 1);
            }
        });
        self.actualizarTotales();
    };

    // -------------------------
    // Consultar pagos existentes
    // -------------------------
    self.consultarPagos = function () {
        showLoading();
        $.ajax({
            url: "/CuentasPorCobrar/ConsultarPagos",
            method: "POST",
            data: { cuentaId: $("#CuentaId").val() },
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.pagos(data.Resultado);
                    self.actualizarTotales();
                } else {
                    alert(data.Mensaje);
                }
            },
            error: function (xhr) {
                hideLoading();
                console.log("❌ Error ConsultarPagos:", xhr);
                let msg = `Error ConsultarPagos\nStatus: ${xhr.status} ${xhr.statusText}\n`;
                if (xhr.responseText) msg += `Response: ${xhr.responseText}\n`;
                alert(msg);
            },
        });
    };

    // -------------------------
    // Productos desde tabla visible (#productos-body)
    // -------------------------
    function construirProductosDesdeTabla() {
        var productos = [];

        $("#productos-body tr").each(function () {
            var fila = $(this);
            // Columnas: Rubro(0), Item(1), Cantidad(2), PrecioUnitario(3), Subtotal(4), Desc/Cargo(5), Accion(6)
            if (fila.find("td").length >= 5) {
                var rubro = fila.find("td:nth-child(1)").text().trim();
                var nombre = fila.find("td:nth-child(2)").text().trim();
                var cantidad = parseInt(fila.find("td:nth-child(3)").text().trim(), 10);
                var precioUnitario = parseFloat(fila.find("td:nth-child(4)").text().replace("Q", "").trim());
                var subtotal = parseFloat(fila.find("td:nth-child(5)").text().replace("Q", "").trim());

                // Leer descuento% y cargo del input en la columna 6
                var descPct = parseFloat(fila.find(".input-desc-pct").val()) || 0;
                var cargo = parseFloat(fila.find(".input-cargo").val()) || 0;

                if (!nombre) return;
                if (isNaN(cantidad)) cantidad = 1;
                if (isNaN(precioUnitario)) precioUnitario = 0;
                if (isNaN(subtotal)) subtotal = cantidad * precioUnitario;

                productos.push({
                    Nombre: nombre,
                    Rubro: rubro,
                    Cantidad: cantidad,
                    PrecioUnitario: precioUnitario,
                    Subtotal: subtotal,
                    DescuentoPorcentaje: descPct,
                    CargoAdicional: cargo
                });
            }
        });

        return productos;
    }

    // -------------------------
    // Lógica de Honorarios Directa a BD
    // -------------------------
    self.consultarHonorariosBD = function () {
        const admisionId = $("#AdmisionId").val();
        if (!admisionId) return;

        $.ajax({
            url: "/CuentasPorCobrar/ConsultarHonorariosHospitalizacion",
            method: "POST",
            data: { hospitalizacionId: admisionId },
            success: function (res) {
                var data = JSON.parse(res);
                if (data.Exitoso) {
                    // Actualizamos el array observable
                    self.honorariosLista(data.Resultado);

                    // INVOCACIÓN GLOBAL: Si la función existe en la ventana, recalculamos todo el formulario
                    if (typeof window.actualizarTotalCuenta === "function") {
                        window.actualizarTotalCuenta();
                    }
                }
            }
        });
    };

    self.agregarHonorarioBD = function () {
        const medicoId = self.medicoSeleccionado();
        const monto = parseFloat(self.montoHonorarioNuevo());
        const admisionId = $("#AdmisionId").val();

        if (!medicoId || isNaN(monto) || monto <= 0) {
            alert("Seleccione un médico y un monto válido.");
            return;
        }

        showLoading();
        $.ajax({
            url: "/CuentasPorCobrar/GuardarHonorarioManual",
            method: "POST",
            data: {
                HospitalizacionId: admisionId,
                EmpleadoId: medicoId,
                Monto: monto
            },
            success: function (res) {
                hideLoading();
                var data = JSON.parse(res);
                if (data.Exitoso) {
                    self.medicoSeleccionado(undefined);
                    self.montoHonorarioNuevo("");
                    self.consultarHonorariosBD();
                } else {
                    alert(data.Mensaje);
                }
            },
            error: function () {
                hideLoading();
                alert("Error al registrar honorario.");
            }
        });
    };

    self.eliminarHonorarioBD = function (item) {
        if (!confirm("¿Desea eliminar este honorario?")) return;

        showLoading();
        $.ajax({
            url: "/CuentasPorCobrar/EliminarHonorarioManual",
            method: "POST",
            data: { id: item.Id },
            success: function (res) {
                hideLoading();
                var data = JSON.parse(res);
                if (data.Exitoso) {
                    self.consultarHonorariosBD();
                }
            }
        });
    };

    // -------------------------
    // ✅ FIX: Enviar al backend el total REAL de la cuenta (incluye honorarios) usando #totalDeLaCuenta.
    //         #Valor (HiddenFor) NO se actualiza cuando cambian honorarios, por eso llegaba 600.
    //         Además: enviar SOLO pagos nuevos para no recontabilizar pagos ya realizados.
    // -------------------------
    self.getModel = function () {
        let empleadoId = $("#EmpleadoId").val();
        if (!empleadoId || String(empleadoId).trim() === "") {
            empleadoId = 3;
            $("#EmpleadoId").val(empleadoId);
        }

        // ✅ Valor = saldo ajustado por descuentos/cargos en tabla
        //    Si hay descuento global adicional, también se resta
        const totalAjustado = parseFloat($("#totalDeLaCuenta").val()) || 0;
        const montoDescuentoGlobal = parseFloat($("#montoDescuento").val()) || 0;
        const valorFinal = Math.max(totalAjustado - montoDescuentoGlobal, 0);

        const pagosNuevos = (self.pagos() || []).filter((p) => {
            return p.Nuevo === true || String(p.Nuevo).toLowerCase() === "true";
        });

        model = {
            NoComprobante: $("#NoComprobante").val(),
            EmpleadoId: empleadoId,
            CuentaId: $("#CuentaId").val(),
            Valor: valorFinal,   // ✅ saldo real ajustado
            Descuento: montoDescuentoGlobal,
            PorcentajeDescuento: parseFloat($("#porcentajeDescuento").val()) || 0,
            TotalConDescuento: valorFinal,
            Observaciones: $("#Observaciones").val(),
            PacienteId: $("#PacienteId").val(),
            PacienteNombre: $("#PacienteNombre").val(),
            PacienteNit: $("#PacienteNit").val(),
            ResponsableNit: $("#ResponsableNit").val(),
            ResponsableNombre: $("#ResponsableNombre").val(),
            ResponsableDireccion: $("#ResponsableDireccion").val(),
            ResponsableCorreo: $("#ResponsableCorreo").val(),
            AdmisionId: $("#AdmisionId").val(),
            PacienteNombreAdmision: $("#PacienteNombreAdmision").val(),
            Pagos: pagosNuevos,
            Productos: construirProductosDesdeTabla(),
        };

        return model;
    };
    // -------------------------
    // ✅ NUEVO: Reintentar FEL (backend)
    // -------------------------
    self.reintentarFel = function () {
        const cuentaId = $("#CuentaId").val();
        if (!cuentaId) {
            alert("No se encontró CuentaId para reintentar FEL.");
            return;
        }

        if (!self.cuentaCerrada()) {
            alert(
                "La cuenta no está cerrada. FEL solo aplica cuando la cuenta está pagada/cerrada.",
            );
            return;
        }

        const estado = normalizeFelEstado(self.felEstado());
        if (!(estado === 1 || estado === 3)) {
            alert(
                "FEL no está en estado Pendiente o Error. Estado actual: " +
                felEstadoToText(estado),
            );
            return;
        }

        if (
            !confirm(
                "¿Desea reintentar la Facturación Electrónica (FEL) para esta cuenta cerrada?",
            )
        )
            return;

        showLoading();
        $.ajax({
            method: "POST",
            url: "/CuentasPorCobrar/ReintentarFel",
            data: { cuentaId: cuentaId },
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);

                if (!data.Exitoso) {
                    alert(data.Mensaje || "No fue posible reintentar FEL.");
                    return;
                }

                // Actualiza estado FEL desde respuesta
                updateFelFromResponse(data);

                if (normalizeFelEstado(self.felEstado()) === 2 && self.uuidFel()) {
                    // Emitida => abrir PDF
                    tryOpenPdf(self.uuidFel());
                    alert("FEL emitida correctamente.");
                } else {
                    // Pendiente/Error
                    const msg =
                        `Pago aplicado. Estado FEL: ${felEstadoToText(self.felEstado())}` +
                        (self.felUltimoError()
                            ? `\nDetalle: ${self.felUltimoError()}`
                            : "");
                    alert(msg);
                }
            },
            error: function (xhr) {
                hideLoading();
                console.log("❌ Error /CuentasPorCobrar/ReintentarFel:", xhr);

                let msg = `Error al reintentar FEL\nStatus: ${xhr.status} ${xhr.statusText}\n`;
                if (xhr.responseText) msg += `Response: ${xhr.responseText}\n`;
                if (xhr.responseJSON)
                    msg += `JSON: ${JSON.stringify(xhr.responseJSON)}\n`;

                alert(msg);
            },
        });
    };

    self.pagarCuenta = function () {
        if (!confirm("¿Desea registrar el pago de esta cuenta?")) return;

        showLoading();

        try {
            const m = self.getModel();

            if (!m.Pagos || m.Pagos.length === 0) {
                hideLoading();
                alert("Debe agregar al menos un pago antes de registrar.");
                return;
            }

            $.ajax({
                method: "POST",
                url: "/CuentasPorCobrar/Pagar",
                data: m,
                success: function (dataResult) {
                    hideLoading();

                    var data = JSON.parse(dataResult);

                    if (!data.Exitoso) {
                        alert(data.Mensaje || "No fue posible registrar el pago.");
                        return;
                    }

                    // Cuenta cerrada / FEL estado
                    self.cuentaCerrada(!!(data.CuentaCerrada ?? data.cuentaCerrada));

                    // Si el backend ya incluye info FEL en la respuesta, la capturamos
                    updateFelFromResponse(data);

                    const cerrada = self.cuentaCerrada();

                    if (!cerrada) {
                        let saldoRestante = data.SaldoRestante || 0;
                        let mensaje = `Pago parcial registrado correctamente.\nSaldo pendiente: Q ${saldoRestante.toFixed(2)}`;
                        alert(mensaje);

                        console.log("Respuesta del servidor:", data);

                        window.location.reload();
                        return;
                    }
                    const estadoFel = normalizeFelEstado(self.felEstado());

                    if (estadoFel === 2 && self.uuidFel()) {
                        tryOpenPdf(self.uuidFel());
                        window.location.href = "/CuentasPorCobrar/Pagadas";
                        return;
                    }

                    let msg = `Pago registrado y cuenta cerrada.\nEstado FEL: ${felEstadoToText(estadoFel)}.`;
                    if (self.felUltimoError())
                        msg += `\nDetalle: ${self.felUltimoError()}`;
                    msg += `\n\nPuede reintentar la FEL desde esta pantalla.`;

                    alert(msg);
                },
                error: function (xhr) {
                    hideLoading();
                    console.log("❌ Error /CuentasPorCobrar/Pagar:", xhr);

                    let msg = `Error al pagar\nStatus: ${xhr.status} ${xhr.statusText}\n`;
                    if (xhr.responseText) msg += `Response: ${xhr.responseText}\n`;
                    if (xhr.responseJSON)
                        msg += `JSON: ${JSON.stringify(xhr.responseJSON)}\n`;

                    alert(msg);
                },
            });
        } catch (e) {
            hideLoading();
            console.error("❌ Error inesperado:", e);
            alert("Error inesperado al procesar el pago.");
        }
    };

    // -------------------------
    // (Opcional) Botón “Ver PDF” si ya existe uuid
    // -------------------------
    self.verPdfFel = function () {
        const uuid = self.uuidFel() || $("#UuidFel").val();
        if (!uuid) {
            alert("No existe UUID FEL para generar PDF.");
            return;
        }
        tryOpenPdf(uuid);
    };
};

// Init
var cuentaPagarVm = new CuentaPagarVM();
ko.applyBindings(cuentaPagarVm);

$(document).ready(function () {
    cuentaPagarVm.consultarPagos();
    cuentaPagarVm.consultarHonorariosBD();
    cuentaPagarVm.actualizarTotales();
    $("#tabs-datos-cuenta").tabs();
});