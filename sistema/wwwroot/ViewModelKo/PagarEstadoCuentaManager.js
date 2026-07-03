// =======================================================================
// GESTOR CENTRAL DE ESTADO DE CUENTA (VANILLA JS + JQUERY AJAX)
// =======================================================================
const EstadoCuentaManager = (function () {

    // 1. EL ESTADO ÚNICO DE LA VERDAD
    let State = {
        modelBase: window.ServerModel || {},
        idsExamenes: window.IdsExamenes || [],

        // Identificadores Seguros (Extraídos del DOM)
        cuentaId: 0,
        pacienteId: 0,
        admisionId: 0,

        habitacion: 0,
        productos: [],          // Cargos activos
        exclusiones: [],        // Cargos no elegibles para seguro
        honorarios: [],         // Honorarios desde BD
        gastosAdmin: [],        // Gastos administrativos desde BD 
        pagosExistentes: [],    // Pagos ya registrados en BD
        pagosNuevos: [],        // Pagos en sesión actual

        // Entradas del usuario para cálculos financieros
        finanzas: {
            deducible: 0,
            coaseguroPct: 0,
            copago: 0,
            iva: 0,
            descuentoPctGlobal: 0,
            descuentoMontoGlobal: 0
        },

        // Resultados Calculados (Solo lectura)
        calculos: {
            totalBrutoOriginal: 0,
            totalBruto: 0,
            totalProductos: 0,

            totalExclusiones: 0,
            totalAseguradoraCubre: 0,
            montoCoaseguro: 0,
            responsabilidadSeguro: 0,
            responsabilidadPaciente: 0,
            totalPagado: 0,
            saldoRestante: 0
        },

        // Control FEL
        fel: {
            estado: 0,
            uuid: document.getElementById("UuidFel")?.value || "",
            cerrada: false
        }
    };

    // 2. INICIALIZACIÓN
    function init() {
        console.log("🚀 Inicializando EstadoCuentaManager...");
        $("#tabs-datos-cuenta").tabs();

        // Extraer los IDs directamente de los HiddenFor del HTML
        State.cuentaId = parseInt($("#CuentaId").val()) || 0;
        State.pacienteId = parseInt($("#PacienteId").val()) || 0;
        State.admisionId = parseInt($("#AdmisionId").val()) || 0;

        mapearProductosBase();
        State.habitacion = State.modelBase.Habitacion?.CostoTotal || 0;

        vincularEventosDOM();

        // Cargar data asíncrona existente
        consultarPagosBD();
        consultarHonorariosBD();
        consultarGastosAdminBD();
    }

    // 3. MAPEO INICIAL DE DATOS
    function mapearProductosBase() {
        if (!State.modelBase.Productos) return;

        let fijos = [];
        (State.modelBase.Paquetes || []).forEach(p => {
            fijos.push({
                idOriginal: p.Id, nombre: p.Nombre, tipo: "Paquete", cantidad: 1,
                precioU: p.Precio, precioSeguroOriginal: p.Precio,
                descPct: 0, cargo: 0, noExcluible: true
            });
        });
        (State.modelBase.Ambulancias || []).forEach(a => {
            fijos.push({
                idOriginal: a.Id, nombre: "Ambulancia - " + a.TipoTraslado, tipo: "Ambulancia", cantidad: 1,
                precioU: a.Precio, precioSeguroOriginal: a.Precio,
                descPct: 0, cargo: 0, noExcluible: true
            });
        });

        let normales = State.modelBase.Productos.map(p => {
            const esExamen = State.idsExamenes.includes(p.ProductoId) || p.EsExamen === true;
            const tipoNormalizado = (p.Tipo || "").trim().toUpperCase();
            // Solo si es examen y tipo "EXAMENES" se aplica descuento automático 100%
            const aplicarDescuento100 = esExamen && tipoNormalizado === "EXAMENES";
            const fechaAplicacion = (p.FechaAplicacion && p.FechaAplicacion !== "Sin fecha")
                ? p.FechaAplicacion
                : "";
            return {
                idOriginal: p.Id, productoId: p.ProductoId, nombre: p.Nombre, tipo: p.Tipo,
                cantidad: p.Cantidad, precioU: p.PrecioUnitario, precioSeguroOriginal: p.PrecioUnitario,
                fechaAplicacion: fechaAplicacion,
                descPct: aplicarDescuento100 ? 100 : 0,
                cargo: 0, noExcluible: false, esExamen: esExamen
            };
        });

        State.productos = [...fijos, ...normales];
    }


    // 4. EL MOTOR MATEMÁTICO PURO (CORREGIDO)
    function recalcularMatematica() {
        // 1. Calcular subtotales de productos (con desc/cargo por línea)
        let totalProductos = 0;
        State.productos.forEach(p => {
            let bruto = p.cantidad * p.precioU;
            let descuentoLinea = bruto * (p.descPct / 100);
            p.subtotal = Math.max(bruto - descuentoLinea + parseFloat(p.cargo), 0);
            totalProductos += p.subtotal;
        });
        State.calculos.totalProductos = totalProductos;

        // 2. Sumar honorarios y gastos admin
        let totalHonorarios = State.honorarios.reduce((sum, h) => sum + (parseFloat(h.Monto) || 0), 0);
        let totalGastosAdmin = State.gastosAdmin.reduce((sum, g) => sum + (parseFloat(g.Monto) || 0), 0);

        // 3. Total bruto original (sin descuento global)
        let totalBrutoOriginal = State.habitacion + totalProductos + totalHonorarios + totalGastosAdmin;
        State.calculos.totalBrutoOriginal = totalBrutoOriginal;

        // 4. Aplicar descuento global (si existe)
        let descuentoGlobal = State.finanzas.descuentoMontoGlobal;
        if (descuentoGlobal > totalBrutoOriginal) descuentoGlobal = totalBrutoOriginal;
        let totalBrutoConDescuento = totalBrutoOriginal - descuentoGlobal;
        State.calculos.totalBruto = totalBrutoConDescuento;

        // 5. Exclusiones y base aseguradora
        State.calculos.totalExclusiones = State.exclusiones.reduce((sum, e) => sum + (e.cantidad * e.precioU), 0);
        let baseAseguradora = totalBrutoConDescuento - State.calculos.totalExclusiones;
        State.calculos.totalAseguradoraCubre = Math.max(baseAseguradora, 0);

        // 6. Coaseguro (sobre lo que cubre la aseguradora)
        State.calculos.montoCoaseguro = State.calculos.totalAseguradoraCubre * (State.finanzas.coaseguroPct / 100);

        // 7. Determinar si el seguro realmente cubre algo
        const tieneSeguro = State.modelBase.NombreDelSeguro &&
            State.modelBase.NombreDelSeguro !== "SIN SEGURO" &&
            State.calculos.totalAseguradoraCubre > 0;

        if (!tieneSeguro) {
            // Sin seguro: el paciente paga el total con descuento
            State.calculos.responsabilidadPaciente = totalBrutoConDescuento;
            State.calculos.responsabilidadSeguro = 0;
        } else {
            // Con seguro: el paciente paga exclusiones + coaseguro + deducible + copago + IVA
            let pagoPacientePrevio = State.calculos.totalExclusiones
                + State.calculos.montoCoaseguro
                + State.finanzas.deducible
                + State.finanzas.copago
                + State.finanzas.iva;
            State.calculos.responsabilidadPaciente = Math.max(pagoPacientePrevio, 0);
            // El seguro paga el resto del total con descuento
            State.calculos.responsabilidadSeguro = Math.max(totalBrutoConDescuento - State.calculos.responsabilidadPaciente, 0);
        }

        // 8. Pagos y saldo restante
        let pagosViejos = State.pagosExistentes.reduce((sum, p) => sum + parseFloat(p.Monto), 0);
        let pagosNuevos = State.pagosNuevos.reduce((sum, p) => sum + parseFloat(p.monto), 0);
        State.calculos.totalPagado = pagosViejos + pagosNuevos;
        State.calculos.saldoRestante = Math.max(State.calculos.responsabilidadPaciente - State.calculos.totalPagado, 0);

        // 9. Actualizar la interfaz
        renderUI();
    }


    // 5. RENDERIZADO DEL DOM
    function renderUI() {
        const tbProd = document.getElementById("ui-productos-body");
        tbProd.innerHTML = "";
        State.productos.forEach((p, idx) => {
            const tr = document.createElement("tr");
            if (p.esExamen) tr.className = "is-examen";

            let btnAction = p.noExcluible
                ? `<button class="btn btn-secondary btn-sm" disabled>Fijo</button>`
                : `<button class="btn btn-warning btn-sm" onclick="EstadoCuentaManager.evtExcluir(${idx})">Excluir</button>`;

            let descCargoHtml = p.noExcluible ? "" : `
                <div class="d-flex align-items-center" style="gap:4px">
                    <input type="number" class="form-control form-control-sm in-desc" data-idx="${idx}" value="${p.descPct}" style="width:60px" title="% Desc"> %
                    <input type="number" class="form-control form-control-sm in-cargo" data-idx="${idx}" value="${p.cargo}" style="width:70px" title="+ Cargo"> Q
                </div>`;

            tr.innerHTML = `
                <td>${p.tipo}</td><td>${p.nombre}</td><td>${p.cantidad}</td>
                <td>Q ${p.precioU.toFixed(2)}</td><td class="font-weight-bold">Q ${p.subtotal.toFixed(2)}</td>
                <td>${descCargoHtml}</td><td>${btnAction}</td>
            `;
            tbProd.appendChild(tr);

            const totalCargosSpan = document.getElementById("ui-total-cargos-tabla");
            if (totalCargosSpan) {
                totalCargosSpan.innerText = "Q " + State.calculos.totalProductos.toFixed(2);
            }
        });

        document.querySelectorAll(".in-desc").forEach(inpt => inpt.addEventListener("change", (e) => {
            State.productos[e.target.dataset.idx].descPct = parseFloat(e.target.value) || 0; recalcularMatematica();
        }));
        document.querySelectorAll(".in-cargo").forEach(inpt => inpt.addEventListener("change", (e) => {
            State.productos[e.target.dataset.idx].cargo = parseFloat(e.target.value) || 0; recalcularMatematica();
        }));

        const tbExc = document.getElementById("ui-exclusiones-body");
        tbExc.innerHTML = "";
        State.exclusiones.forEach((e, idx) => {
            tbExc.innerHTML += `<tr>
                <td>${e.nombre}</td><td>${e.cantidad}</td><td>Q ${e.precioU.toFixed(2)}</td>
                <td class="text-danger font-weight-bold">Q ${(e.cantidad * e.precioU).toFixed(2)}</td>
                <td><button class="btn btn-success btn-sm" onclick="EstadoCuentaManager.evtReintegrar(${idx})">Reintegrar</button></td>
            </tr>`;
        });

        const tbHon = document.getElementById("ui-honorarios-body");
        tbHon.innerHTML = "";
        State.honorarios.forEach(h => {
            tbHon.innerHTML += `<tr>
                <td>${h.NombreMedico}</td><td class="text-right">Q ${parseFloat(h.Monto).toFixed(2)}</td>
                <td class="text-center"><button class="btn btn-danger btn-sm" onclick="EstadoCuentaManager.evtEliminarHonorario(${h.Id})"><i class="fa fa-trash"></i></button></td>
            </tr>`;
        });

        // ── RENDER GASTOS ADMINISTRATIVOS ──────────────────────────────────────
        const tbGastos = document.getElementById("ui-gastos-admin-body");
        if (tbGastos) {
            tbGastos.innerHTML = "";
            if (State.gastosAdmin.length === 0) {
                tbGastos.innerHTML = `<tr id="ui-gastos-admin-empty">
                    <td colspan="4" class="text-center text-muted">
                        <i class="fa fa-info-circle"></i> No hay gastos administrativos registrados.
                    </td>
                </tr>`;
            } else {
                State.gastosAdmin.forEach(g => {
                    tbGastos.innerHTML += `<tr>
                        <td>Gastos Administrativos</td>
                        <td class="text-right">${parseFloat(g.PorcentajeAplicado).toFixed(2)} %</td>
                        <td class="text-right font-weight-bold">Q ${parseFloat(g.Monto).toFixed(2)}</td>
                        <td class="text-center">
                            <button class="btn btn-danger btn-sm" onclick="EstadoCuentaManager.evtEliminarGastoAdmin(${g.Id})">
                                <i class="fa fa-trash"></i>
                            </button>
                        </td>
                    </tr>`;
                });
            }
        }
        // ── Actualizar input de monto calculado con base en el % ingresado ────
        _actualizarMontoGastoAdminInput();

        const tbPagos = document.getElementById("ui-pagos-body");
        tbPagos.innerHTML = "";
        State.pagosExistentes.forEach(p => {
            tbPagos.innerHTML += `<tr><td>${p.Fecha}</td><td>${p.FormaPago}</td><td>${p.Moneda}</td><td>Q ${parseFloat(p.Monto).toFixed(2)}</td><td><span class="badge badge-success">Pagado</span></td></tr>`;
        });
        State.pagosNuevos.forEach((p, idx) => {
            tbPagos.innerHTML += `<tr><td>Pendiente</td><td>${p.formaTxt}</td><td>${p.monedaTxt}</td><td>Q ${p.monto.toFixed(2)}</td>
                <td><button class="btn btn-danger btn-sm" onclick="EstadoCuentaManager.evtEliminarPagoNuevo(${idx})"><i class="fa fa-trash"></i></button></td>
            </tr>`;
        });

        document.getElementById("uiTotalCuenta").value = State.calculos.totalBruto.toFixed(2);
        document.getElementById("uiTotalNoElegibles").value = State.calculos.totalExclusiones.toFixed(2);
        document.getElementById("uiAseguradoraCubre").value = State.calculos.totalAseguradoraCubre.toFixed(2);
        document.getElementById("uiMontoCoaseguro").value = State.calculos.montoCoaseguro.toFixed(2);
        document.getElementById("uiPagoSeguro").value = State.calculos.responsabilidadSeguro.toFixed(2);
        document.getElementById("uiPagoPaciente").value = State.calculos.responsabilidadPaciente.toFixed(2);
        document.getElementById("uiTotalPagado").innerText = State.calculos.totalPagado.toFixed(2);
        document.getElementById("uiSaldoRestante").innerText = State.calculos.saldoRestante.toFixed(2);

        manejarBotonesFEL();
    }

    // 6. EVENTOS Y BINDINGS DE ENTRADA
    function vincularEventosDOM() {
        // ========== DELEGACIÓN DE EVENTOS PARA DESCUENTOS Y CARGOS ==========
        const productosBody = document.getElementById("ui-productos-body");
        if (productosBody) {
            productosBody.addEventListener("change", function (e) {
                const target = e.target;
                if (target.classList.contains("in-desc")) {
                    const idx = target.dataset.idx;
                    if (idx !== undefined) {
                        State.productos[idx].descPct = parseFloat(target.value) || 0;
                        recalcularMatematica();
                    }
                } else if (target.classList.contains("in-cargo")) {
                    const idx = target.dataset.idx;
                    if (idx !== undefined) {
                        State.productos[idx].cargo = parseFloat(target.value) || 0;
                        recalcularMatematica();
                    }
                }
            });
        }

        // Descuento global por porcentaje
        document.getElementById("inpDescuentoPct").addEventListener("input", function (e) {
            let pct = parseFloat(e.target.value) || 0;
            let originalTotal = State.calculos.totalBrutoOriginal;
            if (isNaN(originalTotal) || originalTotal <= 0) return;
            let monto = (pct / 100) * originalTotal;
            document.getElementById("inpDescuentoMonto").value = monto.toFixed(2);
            State.finanzas.descuentoPctGlobal = pct;
            State.finanzas.descuentoMontoGlobal = monto;
            recalcularMatematica();
        });

        // Descuento global por monto
        document.getElementById("inpDescuentoMonto").addEventListener("input", function (e) {
            let monto = parseFloat(e.target.value) || 0;
            let originalTotal = State.calculos.totalBrutoOriginal;
            if (isNaN(originalTotal) || originalTotal <= 0) return;
            let pct = (monto / originalTotal) * 100;
            document.getElementById("inpDescuentoPct").value = pct.toFixed(2);
            State.finanzas.descuentoPctGlobal = pct;
            State.finanzas.descuentoMontoGlobal = monto;
            recalcularMatematica();
        });

        // Gasto administrativo: actualizar monto al cambiar el %
        document.getElementById("txtPctGastoAdmin").addEventListener("input", function () {
            _actualizarMontoGastoAdminInput();
        });

        // Botones de acción
        document.getElementById("btnVerInfoPaciente").addEventListener("click", () =>
            window.open("/Pacientes/Informacion?pacienteId=" + State.pacienteId, "_blank")
        );
        document.getElementById("btnAgregarHonorario").addEventListener("click", () => evtAgregarHonorario());
        document.getElementById("btnAgregarGastoAdmin").addEventListener("click", () => evtAgregarGastoAdmin());
        document.getElementById("btnAgregarPago").addEventListener("click", () => evtAgregarPago());
        document.getElementById("btnPagarFinal").addEventListener("click", () => evtPostPagarFinal());
        document.getElementById("btnEstadoCuenta").addEventListener("click", (e) => evtGenerarPDF(e));

        // FEL
        document.getElementById("btnVerPDF_FEL").addEventListener("click", () => {
            if (State.fel.uuid)
                window.open(`http://3.128.79.135/Xml/GeneratePDF?uuid=${encodeURIComponent(State.fel.uuid)}`, "_blank");
        });
        document.getElementById("btnReintentarFel").addEventListener("click", () => evtReintentarFel());
    }

    // ── ACTUALIZAR MONTO EN INPUT SEGÚN % (basado en saldoRestante) ──────────
    function _actualizarMontoGastoAdminInput() {
        let pct = parseFloat(document.getElementById("txtPctGastoAdmin").value) || 0;
        let inpMonto = document.getElementById("txtMontoGastoAdmin");
        if (!inpMonto) return;

        let base = State.calculos.saldoRestante;
        if (base < 0) base = 0;

        if (pct > 0 && base > 0) {
            let monto = (base * (pct / 100)).toFixed(2);
            inpMonto.value = monto;
        } else {
            inpMonto.value = "";
        }
    }

    // 7. FUNCIONES DE API

    window.evtExcluir = function (idx) {
        let prod = State.productos[idx];
        let precioBase = prod.precioSeguroOriginal;
        showLoading();
        $.ajax({
            url: `/Precios/ObtenerPrecioPorSeguro?tipo=${encodeURIComponent(prod.tipo)}&nombre=${encodeURIComponent(prod.nombre)}&seguro=${encodeURIComponent(State.modelBase.SeguroNombre || 'SIN SEGURO')}`,
            method: "GET",
            success: function (r) {
                hideLoading();
                let data = typeof r === 'string' ? JSON.parse(r) : r;
                if (data.exitoso && data.precio && parseFloat(data.precio) > 0) {
                    prod.precioU = parseFloat(data.precio);
                } else {
                    console.warn(`No se encontró precio para ${prod.nombre} con el seguro. Se usará precio original.`);
                    prod.precioU = precioBase;
                }
                State.productos.splice(idx, 1);
                State.exclusiones.push(prod);
                recalcularMatematica();
            },
            error: function () {
                hideLoading();
                console.error("Error al consultar precio de exclusión. Usando precio original.");
                prod.precioU = precioBase;
                State.productos.splice(idx, 1);
                State.exclusiones.push(prod);
                recalcularMatematica();
                alert(`Se excluyó ${prod.nombre} con el precio original Q ${precioBase.toFixed(2)} porque no se pudo obtener el precio con seguro.`);
            }
        });
    };


    window.evtReintegrar = function (idx) {
        let prod = State.exclusiones.splice(idx, 1)[0];
        prod.precioU = prod.precioSeguroOriginal;
        State.productos.push(prod);
        recalcularMatematica();
    };

    function consultarHonorariosBD() {
        if (State.admisionId <= 0) return;
        $.ajax({
            url: "/CuentasPorCobrar/ConsultarHonorariosHospitalizacion",
            method: "POST",
            data: { hospitalizacionId: State.admisionId },
            success: function (r) {
                let d = JSON.parse(r);
                if (d.Exitoso) { State.honorarios = d.Resultado; recalcularMatematica(); }
            }
        });
    }

    function evtAgregarHonorario() {
        let empId = document.getElementById("selMedico").value;
        let mon = parseFloat(document.getElementById("txtMontoHonorario").value);
        if (!empId || !mon || mon <= 0) { alert("Indique médico y monto."); return; }

        showLoading();
        $.ajax({
            url: "/CuentasPorCobrar/GuardarHonorarioManual",
            method: "POST",
            data: { HospitalizacionId: State.admisionId, EmpleadoId: empId, Monto: mon },
            success: function (r) {
                hideLoading(); let d = JSON.parse(r);
                if (d.Exitoso) { document.getElementById("txtMontoHonorario").value = ""; consultarHonorariosBD(); }
                else alert(d.Mensaje);
            }
        });
    }

    window.evtEliminarHonorario = function (id) {
        if (!confirm("¿Eliminar honorario?")) return;
        showLoading();
        $.ajax({
            url: "/CuentasPorCobrar/EliminarHonorarioManual",
            method: "POST", data: { id: id },
            success: function (r) { hideLoading(); let d = JSON.parse(r); if (d.Exitoso) consultarHonorariosBD(); }
        });
    };

    // ── GASTOS ADMINISTRATIVOS ──────────────────────────────────────────────

    function consultarGastosAdminBD() {
        if (State.admisionId <= 0) return;
        $.ajax({
            url: "/CuentasPorCobrar/ConsultarGastosAdministrativos",
            method: "POST",
            data: { hospitalizacionId: State.admisionId },
            success: function (r) {
                let d = JSON.parse(r);
                if (d.Exitoso) {
                    State.gastosAdmin = d.Resultado;
                    recalcularMatematica();
                }
            }
        });
    }

    function evtAgregarGastoAdmin() {
        let pct = parseFloat(document.getElementById("txtPctGastoAdmin").value) || 0;
        if (pct <= 0 || pct > 100) {
            alert("Ingrese un porcentaje válido entre 0.01 y 100.");
            return;
        }

        let base = State.calculos.saldoRestante;
        if (base <= 0) {
            alert("El saldo pendiente actual es 0 o negativo. No se puede calcular el gasto administrativo.");
            return;
        }

        let monto = Math.round(base * (pct / 100) * 100) / 100;

        if (!confirm(`¿Guardar gasto administrativo del ${pct}% = Q ${monto.toFixed(2)}?`)) return;

        showLoading();
        $.ajax({
            url: "/CuentasPorCobrar/GuardarGastoAdministrativo",
            method: "POST",
            data: {
                HospitalizacionId: State.admisionId,
                PorcentajeAplicado: pct,
                Monto: monto
            },
            success: function (r) {
                hideLoading();
                let d = JSON.parse(r);
                if (d.Exitoso) {
                    document.getElementById("txtPctGastoAdmin").value = "";
                    document.getElementById("txtMontoGastoAdmin").value = "";
                    consultarGastosAdminBD();
                } else {
                    alert(d.Mensaje);
                }
            },
            error: function () { hideLoading(); alert("Error de red al guardar gasto administrativo."); }
        });
    }

    window.evtEliminarGastoAdmin = function (id) {
        if (!confirm("¿Eliminar este gasto administrativo?")) return;
        showLoading();
        $.ajax({
            url: "/CuentasPorCobrar/EliminarGastoAdministrativo",
            method: "POST",
            data: { id: id },
            success: function (r) {
                hideLoading();
                let d = JSON.parse(r);
                if (d.Exitoso) {
                    consultarGastosAdminBD();
                } else {
                    alert(d.Mensaje || "No se pudo eliminar.");
                }
            },
            error: function () { hideLoading(); alert("Error de red al eliminar gasto administrativo."); }
        });
    };

    // ── FIN GASTOS ADMINISTRATIVOS ─────────────────────────────────────────

    function consultarPagosBD() {
        if (State.cuentaId <= 0) return;
        $.ajax({
            url: "/CuentasPorCobrar/ConsultarPagos",
            method: "POST",
            data: { cuentaId: State.cuentaId },
            success: function (r) {
                let d = JSON.parse(r);
                if (d.Exitoso) { State.pagosExistentes = d.Resultado; recalcularMatematica(); }
            }
        });
    }

    function evtAgregarPago() {
        let selF = document.getElementById("selFormaPago");
        let selM = document.getElementById("selMoneda");
        let mon = parseFloat(document.getElementById("txtMontoPago").value);
        if (!mon || mon <= 0) { alert("Monto inválido."); return; }

        State.pagosNuevos.push({
            FormaPagoId: selF.value, formaTxt: selF.options[selF.selectedIndex].text,
            MonedaId: selM.value, monedaTxt: selM.options[selM.selectedIndex].text,
            monto: mon, Nuevo: true
        });
        document.getElementById("txtMontoPago").value = "";
        recalcularMatematica();
    }

    window.evtEliminarPagoNuevo = function (idx) {
        State.pagosNuevos.splice(idx, 1); recalcularMatematica();
    };

    // 8. COMUNICACIÓN FINAL AL BACKEND
    function evtPostPagarFinal() {
        if (State.pagosNuevos.length === 0 && State.calculos.saldoRestante > 0) {
            alert("Debe agregar al menos un pago en esta sesión para confirmar.");
            return;
        }
        if (!confirm("¿Confirmar el pago y aplicar a la cuenta?")) return;

        showLoading();

        // =====================================================================
        // CORRECCIÓN: Se envían los descuentos por línea de cada producto al
        // backend para que recalcule el total correctamente, sin depender de
        // los descuentos automáticos que se aplican al cargar la página.
        //
        // También se envía TotalFrontend (responsabilidadPaciente ya calculado)
        // para que el backend lo use como referencia del saldo real a cobrar.
        // =====================================================================
        let payload = {
            CuentaId: State.cuentaId,
            PacienteId: State.pacienteId,
            AdmisionId: State.admisionId,
            PacienteNombre: document.getElementById("ResponsableNombre").value,
            ResponsableNit: document.getElementById("ResponsableNit").value,
            ResponsableNombre: document.getElementById("ResponsableNombre").value,
            ResponsableDireccion: document.getElementById("ResponsableDireccion").value,
            ResponsableCorreo: document.getElementById("ResponsableCorreo").value,
            PorcentajeDescuento: State.finanzas.descuentoPctGlobal,
            Descuento: State.finanzas.descuentoMontoGlobal,

            // --- NUEVO: Total calculado por el frontend (responsabilidad paciente) ---
            // El backend usará este valor para calcular el saldo pendiente real,
            // respetando los descuentos por línea que el usuario modificó en pantalla.
            TotalFrontend: parseFloat(State.calculos.responsabilidadPaciente.toFixed(2)),

            // --- NUEVO: Descuentos por línea de cada producto ---
            // Permite al backend conocer qué descuento tiene cada ítem,
            // especialmente exámenes cuyo descuento fue modificado por el usuario.
            DescuentosLinea: State.productos.map(p => ({
                IdOriginal: p.idOriginal,
                DescPct: p.descPct || 0,
                Cargo: p.cargo || 0,
                Subtotal: parseFloat((p.subtotal || 0).toFixed(2))
            })),

            Pagos: State.pagosNuevos.map(p => ({ FormaPagoId: p.FormaPagoId, MonedaId: p.MonedaId, Monto: p.monto, Nuevo: true })),

            // Productos excluidos (los que están en la tabla de exclusiones)
            Exclusiones: State.exclusiones.map(prod => ({
                IdOriginal: prod.idOriginal,
                Nombre: prod.nombre,
                Tipo: prod.tipo,
                Cantidad: prod.cantidad,
                PrecioUnitario: prod.precioU
            }))
        };

        $.ajax({
            url: "/CuentasPorCobrar/Pagar",
            method: "POST",
            data: payload,
            success: function (res) {
                hideLoading();
                let d = JSON.parse(res);
                if (d.Exitoso) {
                    State.fel.estado = d.FelEstado;
                    State.fel.uuid = d.UuidFel;
                    State.fel.cerrada = d.CuentaCerrada;
                    alert(d.Mensaje + (d.FelUltimoError ? "\nError FEL: " + d.FelUltimoError : ""));

                    if (d.CuentaCerrada && State.fel.uuid) {
                        window.open(`http://3.128.79.135/Xml/GeneratePDF?uuid=${encodeURIComponent(State.fel.uuid)}`, "_blank");
                        window.location.href = "/CuentasPorCobrar/Pagadas";
                    } else if (d.CuentaCerrada) {
                        window.location.href = "/CuentasPorCobrar/Pagadas";
                    } else {
                        State.pagosNuevos = [];
                        consultarPagosBD();
                    }
                } else { alert("Error: " + d.Mensaje); }
            },
            error: function () { hideLoading(); alert("Falla de red crítica al pagar."); }
        });
    }

    function evtReintentarFel() {
        if (!State.fel.cerrada) { alert("Solo cuentas cerradas aplican para FEL."); return; }
        showLoading();
        $.ajax({
            url: "/CuentasPorCobrar/ReintentarFel",
            method: "POST",
            data: { cuentaId: State.cuentaId },
            success: function (res) {
                hideLoading(); let d = JSON.parse(res);
                if (d.Exitoso) {
                    State.fel.uuid = d.UuidFel; State.fel.estado = 2;
                    alert("FEL Reintentada y Emitida con éxito.");
                    manejarBotonesFEL();
                } else alert(d.Mensaje);
            }
        });
    }

    function manejarBotonesFEL() {
        const btnRe = document.getElementById("btnReintentarFel");
        const btnPdf = document.getElementById("btnVerPDF_FEL");

        btnRe.style.display = "none"; btnPdf.style.display = "none";
        if (State.calculos.saldoRestante === 0 && State.calculos.totalPagado > 0) {
            State.fel.cerrada = true;
            if (State.fel.uuid) { btnPdf.style.display = "inline-block"; }
            else { btnRe.style.display = "inline-block"; }
        }
    }

    // 9. GENERACIÓN DE PDF
    function evtGenerarPDF(e) {
        e.preventDefault();

        let jsonPDF = {
            Responsable: {
                Nombre: document.getElementById("ResponsableNombre").value, Nit: document.getElementById("ResponsableNit").value,
                Direccion: document.getElementById("ResponsableDireccion").value, Correo: document.getElementById("ResponsableCorreo").value
            },
            Habitacion: {
                Numero: document.getElementById("lblNumeroHabitacion").innerText, Categoria: document.getElementById("lblCategoriaHabitacion").innerText,
                Tarifa: State.habitacion, Seguro: document.getElementById("lblSeguroHabitacion").innerText
            },
            Productos: State.productos.map(p => ({
                Fecha: p.fechaAplicacion || "",
                Item: p.nombre,
                Tipo: p.tipo,
                Cantidad: p.cantidad,
                PrecioUnitario: p.precioU,
                DescPct: p.descPct || 0,
                Cargo: p.cargo || 0,
                Subtotal: p.subtotal
            })),
            Exclusiones: State.exclusiones.map(x => ({ Item: x.nombre, Cantidad: x.cantidad, PrecioUnitario: x.precioU, Subtotal: x.cantidad * x.precioU })),
            Honorarios: State.honorarios.map(h => ({ Medico: h.NombreMedico, Monto: h.Monto })),
            GastosAdministrativos: State.gastosAdmin.map(g => ({
                Descripcion: "Gastos Administrativos",
                PorcentajeAplicado: g.PorcentajeAplicado,
                Monto: g.Monto
            })),
            DescuentoGlobal: State.finanzas.descuentoMontoGlobal,

            Pagos: {
                TotalCuenta: State.calculos.totalBruto, TotalAseguradora: State.calculos.totalAseguradoraCubre, TotalNoElegibles: State.calculos.totalExclusiones,
                Deducibles: State.finanzas.deducible, Coaseguro: State.calculos.montoCoaseguro, Copago: State.finanzas.copago,
                IVA: State.finanzas.iva, PagoPaciente: State.calculos.responsabilidadPaciente, PagoSeguro: State.calculos.responsabilidadSeguro
            },
            Paciente: {
                Nombre: State.modelBase.PacienteNombre, Telefono: State.modelBase.PacienteTelefono, Celular: State.modelBase.PacienteCelular,
                Direccion: State.modelBase.PacienteDireccion, FechaNacimiento: State.modelBase.PacienteFechaNacimiento,
                Edad: State.modelBase.PacienteEdad, Sexo: State.modelBase.PacienteSexo, Nit: State.modelBase.PacienteNit, Dpi: State.modelBase.PacienteDpi
            },
            Hospitalizacion: {
                FechaInicioHospitalizacion: State.modelBase.FechaInicioHops, MedicoResponsable: State.modelBase.MedicoHops,
                NombreSeguro: State.modelBase.NombreDelSeguro, HospitalizacionId: State.admisionId
            }
        };

        const url = State.admisionId > 0 ? `/CrearPDF/ProcesarEstadoCuenta?id=${State.admisionId}` : '/CrearPDF/ProcesarEstadoCuenta';
        fetch(url, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(jsonPDF) })
            .then(res => res.blob()).then(blob => {
                window.open(window.URL.createObjectURL(blob), '_blank');
            }).catch(err => console.error("Error al generar PDF:", err));
    }

    // Exponer inicializador y eventos onClick del html
    return {
        init,
        evtExcluir: window.evtExcluir,
        evtReintegrar: window.evtReintegrar,
        evtEliminarHonorario: window.evtEliminarHonorario,
        evtEliminarGastoAdmin: window.evtEliminarGastoAdmin,
        evtEliminarPagoNuevo: window.evtEliminarPagoNuevo
    };

})();

// Bootstrap
document.addEventListener("DOMContentLoaded", () => {
    EstadoCuentaManager.init();
});