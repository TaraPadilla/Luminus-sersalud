var EmergenciaVM = function () {
    let self = this;
    let model = {};
    self.txtModalConfirmacion = ko.observable(); //modal confirmacion venta

    self.ventaSubtotal = ko.observable(0);
    self.ventaDescuento = ko.observable(0);
    self.ventaTotal = ko.observable(0);

    //PACIENTES
    self.pacientesExistentes = ko.observableArray();
    self.pacienteSeleccionado = ko.observable();
    self.pacienteId = ko.observable();
    self.pacienteNombre = ko.observable();

    //Habitaciones
    self.habitacionesDisponibles = ko.observableArray();

    //Productos
    self.productoSeleccionado = ko.observable();
    self.precioSeleccionadoProducto = ko.observable();
    self.registrosInventario = ko.observableArray();
    self.productosExistentes = ko.observableArray();
    self.preciosProducto = ko.observableArray();
    self.unidadesVentaProducto = ko.observableArray();
    self.unidadVentaSeleccionadaProducto = ko.observable();
    self.productosEmergencia = ko.observableArray();

    // Insumos
    // self.insumosExistentes = ko.observableArray();
    // self.insumoSeleccionado = ko.observable();
    // self.unidadesVentaInsumo = ko.observableArray(); // Lista propia
    // self.unidadVentaSeleccionadaInsumo = ko.observable();
    // self.preciosInsumo = ko.observableArray(); // Lista propia
    // self.precioSeleccionadoInsumo = ko.observable();

    //Servicios
    self.servicioSeleccionado = ko.observable();
    self.precioSeleccionadoServicio = ko.observable();
    self.serviciosExistentes = ko.observableArray();
    self.serviciosEmergencia = ko.observableArray();
    self.preciosServicio = ko.observableArray();

    //Examenes
    self.examenSeleccionado = ko.observable();
    self.precioSeleccionadoExamen = ko.observable();
    self.examenesExistentes = ko.observableArray();
    self.examenesEmergencia = ko.observableArray();
    self.preciosExamen = ko.observableArray();
    // Variable observable para almacenar la pregunta ingresada
    self.pregunta = ko.observable();
    self.examenesPregunta = ko.observableArray();

    //ELEMENTOS PRESCRIPCION
    self.consultarElementosEmergencia = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Emergencias/ConsultarElementosEmergencia',
            data: {
                emergenciaId: $("#EmergenciaId").val()
            },
            success: function (dataResult) {
                hideLoading();
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {

                    if (data.Resultado.Observaciones) {
                        $("#Observaciones").val(data.Resultado.Observaciones);
                    }
                    self.productosEmergencia([]);
                    self.serviciosEmergencia([]);
                    self.examenesEmergencia([]);
                    $(data.Resultado.Productos).each(function (idx, producto) {
                        producto.Eliminado = ko.observable(producto.Eliminado);
                        producto.Cantidad = ko.observable(producto.Cantidad);
                        producto.ValorUnitario = ko.observable(producto.ValorUnitario);
                        producto.DescuentoPorcentaje = ko.observable(producto.DescuentoPorcentaje);
                        producto.DescuentoValor = ko.observable(producto.DescuentoValor);
                        producto.ValorSubtotal = ko.observable(producto.ValorSubtotal);
                        producto.ValorTotal = ko.observable(producto.ValorTotal);
                        self.productosEmergencia.push(producto);
                    });
                    $(data.Resultado.Servicios).each(function (idx, servicio) {
                        servicio.Eliminado = ko.observable(servicio.Eliminado);
                        servicio.ValorSubtotal = ko.observable(servicio.ValorSubtotal);
                        servicio.ValorTotal = ko.observable(servicio.ValorTotal);
                        servicio.DescuentoValor = ko.observable(servicio.DescuentoValor);
                        self.serviciosEmergencia.push(servicio);
                    });
                    $(data.Resultado.Examenes).each(function (idx, examen) {
                        examen.Eliminado = ko.observable(examen.Eliminado);
                        examen.ValorSubtotal = ko.observable(examen.ValorSubtotal);
                        examen.ValorTotal = ko.observable(examen.ValorTotal);
                        examen.DescuentoValor = ko.observable(examen.DescuentoValor);
                        self.examenesEmergencia.push(examen);
                    });
                    self.actualizarTotales();
                } else {
                    console.log(data.Mensaje);
                    mensajeEmergenteError(data.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                mensajeEmergenteError(data.Mensaje);
                console.log(data);
            }
        });
    };

    //PACIENTES
    self.consultarPacientesExistentes = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Emergencias/ConsultarPacientes',
            success: function (dataResult) {
                hideLoading();
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.pacientesExistentes(data.Resultado);
                    if ($("#PacienteId").val() != "") {
                        $(self.pacientesExistentes()).each(function (idx, paciente) {
                            if (paciente.PacienteId == $("#PacienteId").val()) {
                                self.pacienteSeleccionado(paciente);
                            }
                        });
                    }
                } else {
                    mensajeEmergenteError(data.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                mensajeEmergenteError(data.Mensaje);
                console.log(data);
            }
        });
    };
    self.pacienteSeleccionado.subscribe(function (paciente) {
        if (paciente == null) return;

        if (paciente.PacienteId == null || paciente.PacienteId == undefined) {
            self.pacienteId(null);
            self.pacienteNombre(paciente);
            $("#PacienteNit").val(null);
            $("#PacienteDireccion").val(null);

            if (typeof paciente === 'string' && paciente !== "") {
                $("#nuevo-paciente-nombre").val(paciente);
                $("#nuevo-paciente-nit").val("");
                $("#nuevo-paciente-direccion").val("");
                $("#nuevo-paciente-telefono").val("");
                $("#mdl-nuevo-paciente-error").hide();

                $("#mdl-nuevo-paciente").dialog({
                    modal: true,
                    width: 650,
                    title: "Registrar nuevo paciente",
                    buttons: [
                        {
                            text: "Registrar paciente",
                            class: "btn btn-success",
                            click: function () {
                                self.registrarPacienteRapido();
                            }
                        },
                        {
                            text: "Cancelar",
                            class: "btn btn-secondary",
                            click: function () {
                                $(this).dialog("close");
                            }
                        }
                    ]
                });
            }
        } else {
            // Paciente existente
            self.pacienteId(paciente.PacienteId);
            self.pacienteNombre(paciente.Nombre);
            $("#PacienteNit").val(paciente.Nit);
            $("#PacienteDireccion").val(paciente.Direccion);
        }
    });

    self.registrarPacienteRapido = function () {
        var nombre = $("#nuevo-paciente-nombre").val().trim();
        if (!nombre) {
            $("#mdl-nuevo-paciente-error-msg").text("El nombre del paciente es requerido.");
            $("#mdl-nuevo-paciente-error").show();
            return;
        }

        showLoading();
        $.ajax({
            method: "POST",
            url: "/Pacientes/RegistrarPacienteRapido",
            data: {
                Nombre: nombre,
                Nit: $("#nuevo-paciente-nit").val(),
                Direccion: $("#nuevo-paciente-direccion").val(),
                Telefono: $("#nuevo-paciente-telefono").val(),
                FechaNacimiento: $("#nuevo-paciente-fecha-nacimiento").val(),
                Genero: $("#nuevo-paciente-genero").val()
            },
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    var nuevo = data.Resultado;

                    self.pacienteId(nuevo.PacienteId);
                    self.pacienteNombre(nuevo.Nombre);

                    $("#PacienteNit").val(nuevo.Nit || "");
                    $("#PacienteDireccion").val(nuevo.Direccion || "");

                    $("#mdl-nuevo-paciente").dialog("close");
                    mensajeEmergente("Paciente registrado correctamente.");
                } else {
                    $("#mdl-nuevo-paciente-error-msg").text(data.Mensaje);
                    $("#mdl-nuevo-paciente-error").show();
                }
            },
            error: function () {
                hideLoading();
                $("#mdl-nuevo-paciente-error-msg").text("Error al comunicarse con el servidor.");
                $("#mdl-nuevo-paciente-error").show();
            }
        });
    };

    //HABITACIONES
    self.consultarHabitacionesDisponibles = function () {
        let textoCargando = $("#texto-cargando-habitaciones-disponibles");
        let textoError = $("#texto-error-cargando-habitaciones-disponibles");
        self.habitacionesDisponibles([]);
        textoCargando.show();
        textoError.hide();
        $.ajax({
            method: "POST",
            url: '/Emergencias/ConsultarHabitacionesDisponibles',
            success: function (dataResult) {
                textoCargando.hide();
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.habitacionesDisponibles(data.Resultado);
                } else {
                    textoError.show();
                }
            },
            error: function (data) {
                textoCargando.hide();
                textoError.show();
            }
        });
    };
    self.ocuparHabitacion = function (habitacion) {
        window.location.href = '/Hospitalizacion/Hospitalizar?habitacionId=' + habitacion.HabitacionId
            + '&emergenciaId=' + $("#EmergenciaId").val();
    };

    //PRODUCTOS
    self.consultarProductosExistentes = function () {
        let textoCargando = $("#texto-cargando-productos-existentes");
        let textoError = $("#texto-error-cargando-productos-existentes");
        self.productosExistentes([]);
        textoCargando.show();
        textoError.hide();
        $.ajax({
            method: "POST",
            url: '/Emergencias/ConsultarProductosExistentes',
            //data: { tipoProductoId: 1 },
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
                                    ProductoCodigo: vl2.ProductoCodigo,
                                    ProductoNombre: vl2.ProductoNombre,
                                    ProductoNombreMostrar: vl2.ProductoNombre + " - Activo y concentracion: " + vl2.ProductoActivoConcentracion
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
                textoCargando.hide();
                textoError.show();
            }
        });
    };


    // self.consultarInsumosExistentes = function () {
    //     let textoCargando = $("#texto-cargando-insumos-existentes");
    //     self.insumosExistentes([]);
    //     textoCargando.show();

    //     $.ajax({
    //         method: "POST",
    //         url: '/Emergencias/ConsultarProductosExistentes',
    //         data: { tipoProductoId: 2 }, // Diferenciador para el Back
    //         success: function (dataResult) {
    //             let data = JSON.parse(dataResult);
    //             if (data.Exitoso) {
    //                 // Mapeamos los resultados para el select de insumos
    //                 let idsProcesados = new Set();
    //                 $(data.Resultado).each(function (idx, item) {
    //                     if (!idsProcesados.has(item.ProductoId)) {
    //                         self.insumosExistentes.push({
    //                             ProductoId: item.ProductoId,
    //                             ProductoCodigo: item.ProductoCodigo,
    //                             ProductoNombre: item.ProductoNombre,
    //                             ProductoNombreMostrar: item.ProductoNombre + (item.ProductoActivoConcentracion ? " - " + item.ProductoActivoConcentracion : "")
    //                         });
    //                         idsProcesados.add(item.ProductoId);
    //                     }
    //                 });
    //                 // Guardamos en el array global de inventario para que las unidades/precios funcionen
    //                 self.registrosInventario(self.registrosInventario().concat(data.Resultado));
    //             }
    //             textoCargando.hide();
    //         }
    //     });
    // };

    // self.insumoSeleccionado.subscribe(function (nuevoInsumo) {
    //     if (nuevoInsumo) {
    //         self.consultarDetallesInsumo(nuevoInsumo.ProductoId);
    //     } else {
    //         self.unidadesVentaInsumo([]);
    //         self.preciosInsumo([]);
    //     }
    // });


    // // Función para obtener Unidades y Precios del Insumo (Tipo 2)
    // self.consultarDetallesInsumo = function (productoId) {
    //     self.unidadesVentaInsumo([]);
    //     self.preciosInsumo([]);

    //     // Filtramos del array global registrosInventario que ya llenamos en consultarInsumosExistentes
    //     $(self.registrosInventario()).each(function (idx, item) {
    //         if (item.ProductoId == productoId) {
    //             // Llenar Unidades (Evitando duplicados)
    //             if (!self.unidadesVentaInsumo().find(u => u.Id == item.UnidadMedidaVentaId)) {
    //                 self.unidadesVentaInsumo.push({
    //                     Id: item.UnidadMedidaVentaId,
    //                     UnidadMedidaVentaNombre: item.UnidadMedidaVentaNombre
    //                 });
    //             }
    //             // Llenar Precios
    //             self.preciosInsumo.push({
    //                 Id: item.PrecioId,
    //                 Precio: item.PrecioNombre + " - Q" + item.PrecioValor,
    //                 PrecioValor: item.PrecioValor
    //             });
    //         }
    //     });
    // };

    // self.agregarInsumo = function () {
    //     let p = self.insumoSeleccionado();
    //     let u = self.unidadVentaSeleccionadaInsumo();
    //     let pr = self.precioSeleccionadoInsumo();

    //     if (!p) return;

    //     let nuevoElemento = {
    //         // ESTA LÍNEA ES LA QUE EVITA EL ERROR
    //         TipoProductoId: 2,

    //         ProductoId: p.ProductoId,
    //         ProductoCodigo: p.ProductoCodigo,
    //         ProductoNombre: p.ProductoNombre,
    //         UnidadMedidaVentaId: u ? u.Id : null,
    //         UnidadMedidaVentaNombre: ko.observable(u ? u.UnidadMedidaVentaNombre : "U"),
    //         PrecioId: pr ? pr.Id : null,
    //         PrecioNombre: ko.observable(pr ? pr.Precio : "General"),
    //         Cantidad: ko.observable(1),
    //         ValorUnitario: ko.observable(pr ? pr.PrecioValor : 0),
    //         ValorSubtotal: ko.observable(0),
    //         DescuentoPorcentaje: ko.observable(0),
    //         DescuentoValor: ko.observable(0),
    //         ValorTotal: ko.observable(0),
    //         Eliminado: ko.observable(false)
    //     };

    //     self.productosEmergencia.push(nuevoElemento);
    //     self.actualizarTotales();
    // };

    self.consultarUnidadesVentaProducto = function (producto) {
        if (!self.productoSeleccionado() || producto == null || producto == undefined
            || producto.ProductoId == null || producto.ProductoId == undefined) {
            //mensajeEmergenteError("No hay ningun producto valido seleccionado");
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
            if (registro.UnidadMedidaVentaId != null && registro.UnidadMedidaVentaId != undefined) {
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
                        UnidadMedidaVentaNombre: vl.UnidadMedidaVentaNombre
                    };
                    self.unidadesVentaProducto.push(unidadAgregada);
                    agregado = true;
                }
            });

        }
    }
    self.consultarPreciosProducto = function (unidadSeleccionada) {
        if (unidadSeleccionada == null || unidadSeleccionada == undefined) {
            self.preciosProducto([]);
            return;
        }

        if (!self.productoSeleccionado() || self.productoSeleccionado() == null || self.productoSeleccionado() == undefined) {
            mensajeEmergenteError("No hay ningun producto valido seleccionado");
        }
        self.preciosProducto([]);
        let registrosInventarioProducto = new Array();
        $(self.registrosInventario()).each(function (idx, registro) {
            if (registro.ProductoId == self.productoSeleccionado().ProductoId && registro.UnidadMedidaVentaId == unidadSeleccionada.Id) {
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
            let precios = new Array();
            $(registrosInventarioProducto).each(function (idx, vl) {
                if (vl.PrecioId == precioId) {
                    let precioAgregado = {
                        ProductoInventarioId: vl.ProductoInventarioId,
                        Id: precioId,
                        Precio: vl.PrecioNombre + " (Q " + vl.PrecioValor + ")",
                        PrecioValor: vl.PrecioValor,
                        PrecioCompra: vl.PrecioCompra
                    };
                    precios.push(precioAgregado);
                }
            });
            //Ahora se debe eliminar las duplicidades de precios
            //eligiendo el precio del ultimo registro en inventario
            let productoInventarioIds = new Array();
            $(precios).each(function (idx, vl) {
                productoInventarioIds.push(vl.ProductoInventarioId);
            });
            let ultimoProductoInventarioId = productoInventarioIds[0];
            for (let i = 0; i < productoInventarioIds.length; i++) {
                if (productoInventarioIds[i] > ultimoProductoInventarioId) {
                    ultimoProductoInventarioId = productoInventarioIds[i];
                }
            }
            $(precios).each(function (idx, vl) {
                if (vl.ProductoInventarioId != ultimoProductoInventarioId) {
                    precios.splice(idx, 1);
                }
            });
            $(precios).each(function (idx, vl) {
                self.preciosProducto.push(vl);
            });

        }
    };

    self.productoSeleccionado.subscribe(function (value) {
        self.consultarUnidadesVentaProducto(value);
    });
    self.unidadVentaSeleccionadaProducto.subscribe(function (unidadSeleccionada) {
        self.consultarPreciosProducto(unidadSeleccionada);
    });
    self.agregarProducto = function () {
        let productoAgregado = {
            //TipoProductoId: 1,
            ProductoCodigo: self.productoSeleccionado().ProductoCodigo,
            ProductoId: self.productoSeleccionado().ProductoId,
            ProductoNombre: self.productoSeleccionado().ProductoNombre,
            UnidadMedidaVentaId: self.unidadVentaSeleccionadaProducto() != null ? self.unidadVentaSeleccionadaProducto().Id : null,
            UnidadMedidaVentaNombre: ko.observable(self.unidadVentaSeleccionadaProducto() != null ? self.unidadVentaSeleccionadaProducto().UnidadMedidaVentaNombre : "Unidad"),
            Cantidad: ko.observable(1),
            PrecioId: self.precioSeleccionadoProducto() != null ? self.precioSeleccionadoProducto().Id : null,
            PrecioNombre: ko.observable(self.precioSeleccionadoProducto() != null ? self.precioSeleccionadoProducto().Precio : "General"),
            ValorUnitario: ko.observable(self.precioSeleccionadoProducto() != null ? self.precioSeleccionadoProducto().PrecioValor : 0),
            ValorSubtotal: ko.observable(self.precioSeleccionadoProducto() != null ? self.precioSeleccionadoProducto().PrecioValor : 0),
            DescuentoPorcentaje: ko.observable(0),
            DescuentoValor: ko.observable(0),
            ValorTotal: ko.observable(self.precioSeleccionadoProducto() != null ? self.precioSeleccionadoProducto().PrecioValor : 0),
            Eliminado: ko.observable(false)
        };
        self.productosEmergencia.push(productoAgregado);
        mensajeEmergente("Producto agregado");
        self.actualizarTotales();
    };

    //SERVICIOS
    self.consultarServiciosExistentes = function () {
        $.ajax({
            method: "POST",
            url: '/Venta/ConsultarServiciosExistentes',
            data: model,
            success: function (dataResult) {
                hideLoading();
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.serviciosExistentes(data.Resultado);
                }
                else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.consultarPreciosServicio = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Venta/ConsultarPreciosServicio',
            data: {
                servicioId: self.servicioSeleccionado().ServicioId
            },
            success: function (dataResult) {
                hideLoading();
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.preciosServicio(data.Resultado);
                }
                else {
                    alert(data.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.agregarServicio = function () {
        let servicioAgregado = {
            ServicioCodigo: self.servicioSeleccionado().ServicioCodigo,
            ServicioId: self.servicioSeleccionado().ServicioId,
            ServicioNombre: self.servicioSeleccionado().ServicioNombre,
            PrecioId: self.precioSeleccionadoServicio().PrecioId,
            PrecioNombre: self.precioSeleccionadoServicio().PrecioNombre,
            ValorUnitario: self.precioSeleccionadoServicio().PrecioValor,
            ValorSubtotal: ko.observable(),
            ValorTotal: ko.observable(),
            Cantidad: 1,
            DescuentoPorcentaje: 0,
            DescuentoValor: ko.observable(),
            Eliminado: ko.observable(false)
        };
        self.serviciosEmergencia.push(servicioAgregado);
        mensajeEmergente("Servicio agregado");
        self.actualizarTotales();
    };

    //EXAMENES
    self.consultarExamenesExistentes = function () {
        $.ajax({
            method: "POST",
            url: '/Venta/ConsultarExamenesExistentes',
            data: model,
            success: function (dataResult) {
                hideLoading();
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.examenesExistentes(data.Resultado);
                }
                else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.consultarPreciosExamen = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Venta/ConsultarPreciosExamen',
            data: {
                examenLabClinicoId: self.examenSeleccionado().ExamenId
            },
            success: function (dataResult) {
                hideLoading();
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.preciosExamen(data.Resultado);
                }
                else {
                    alert(data.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.agregarExamen = function () {
        if (self.examenSeleccionado() != null && self.examenSeleccionado() != undefined) {
            if (self.precioSeleccionadoExamen() != null
                && self.precioSeleccionadoExamen() != undefined) {
                let examenAgregado = {
                    ExamenCodigo: self.examenSeleccionado().ExamenCodigo,
                    ExamenId: self.examenSeleccionado().ExamenId,
                    ExamenNombre: self.examenSeleccionado().ExamenNombre,
                    PrecioId: self.precioSeleccionadoExamen().PrecioId,
                    PrecioNombre: self.precioSeleccionadoExamen().PrecioNombre,
                    ValorUnitario: self.precioSeleccionadoExamen().PrecioValor,
                    ValorSubtotal: ko.observable(),
                    ValorTotal: ko.observable(),
                    Cantidad: 1,
                    DescuentoPorcentaje: 0,
                    DescuentoValor: ko.observable(),
                    Eliminado: ko.observable(false)
                };
                self.examenesEmergencia.push(examenAgregado);
                mensajeEmergente("Examen agregado");
                self.actualizarTotales();
            } else {
                alert("No hay precios agregados para este examen");
            }
        } else {
            alert("No hay ningun examen seleccionado");
        }
    };
    self.quitarElemento = function (elemento) {
        elemento.Eliminado(true);
        self.actualizarTotales();
    }
    self.actualizarTotales = function () {
        let subtotal = 0;
        let descuento = 0;
        let total = 0;

        $(self.productosEmergencia()).each(function (idx, producto) {
            if (!producto.Eliminado()) {
                let subtotalProducto = producto.Cantidad() * producto.ValorUnitario();
                let descuentoValor = subtotalProducto * (producto.DescuentoPorcentaje() / 100);
                let totalProducto = subtotalProducto - descuentoValor;
                producto.ValorSubtotal(subtotalProducto.toFixed(2));
                producto.ValorTotal(totalProducto.toFixed(2));
                producto.DescuentoValor(descuentoValor.toFixed(2));
                subtotal += subtotalProducto;
                descuento += descuentoValor;
                total += totalProducto;
            }
        });
        // $(self.productosEmergencia()).each(function (idx, prod) {
        //     // Validamos que el producto no esté marcado como eliminado
        //     // (Asumiendo que usas prod.Eliminado como observable)
        //     let eliminado = (typeof prod.Eliminado === "function") ? prod.Eliminado() : prod.Eliminado;

        //     if (!eliminado) {
        //         let cantidad = parseFloat(ko.unwrap(prod.Cantidad)) || 0;
        //         let precio = parseFloat(ko.unwrap(prod.ValorUnitario)) || 0;
        //         let desc = parseFloat(ko.unwrap(prod.DescuentoPorcentaje)) || 0;

        //         let filaSubtotal = cantidad * precio;
        //         let filaDescuento = filaSubtotal * (desc / 100);
        //         let filaTotal = filaSubtotal - filaDescuento;

        //         // Seteamos valores de la fila (si son observables)
        //         if (typeof prod.ValorSubtotal === "function") prod.ValorSubtotal(filaSubtotal.toFixed(2));
        //         if (typeof prod.ValorTotal === "function") prod.ValorTotal(filaTotal.toFixed(2));

        //         subtotal += filaSubtotal;
        //         descuento += filaDescuento;
        //         total += filaTotal;
        //     }
        // });
        $(self.serviciosEmergencia()).each(function (idx, servicio) {
            if (!servicio.Eliminado()) {
                let subtotalServicio = servicio.Cantidad * servicio.ValorUnitario;
                let descuentoValor = subtotalServicio * (servicio.DescuentoPorcentaje / 100);
                let totalServicio = subtotalServicio - descuentoValor;
                servicio.ValorSubtotal(subtotalServicio.toFixed(2));
                servicio.ValorTotal(totalServicio.toFixed(2));
                servicio.DescuentoValor(descuentoValor.toFixed(2));
                subtotal += subtotalServicio;
                descuento += descuentoValor;
                total += totalServicio;
            }
        });
        $(self.examenesEmergencia()).each(function (idx, examen) {
            if (!examen.Eliminado()) {
                let subtotalExamen = examen.Cantidad * examen.ValorUnitario;
                let descuentoValor = subtotalExamen * (examen.DescuentoPorcentaje / 100);
                let totalExamen = subtotalExamen - descuentoValor;
                examen.ValorSubtotal(subtotalExamen.toFixed(2));
                examen.ValorTotal(totalExamen.toFixed(2));
                examen.DescuentoValor(descuentoValor.toFixed(2));
                subtotal += subtotalExamen;
                descuento += descuentoValor;
                total += totalExamen;
            }
        });

        self.ventaSubtotal(subtotal.toFixed(2));
        self.ventaDescuento(descuento.toFixed(2));
        self.ventaTotal(total.toFixed(2));
    };


    self.getModel = function () {

        model = {
            EmergenciaId: $("#EmergenciaId").val(),
            CodigoVendedor: $("#CodigoVendedor").val(),
            SucursalId: $("#SucursalId").val(),
            Responsable: $("#Responsable").val(),
            PacienteId: self.pacienteId(),
            PacienteNombre: self.pacienteNombre(),
            PacienteNit: $("#PacienteNit").val(),
            PacienteDireccion: $("#PacienteDireccion").val(),
            Observaciones: $("#Observaciones").val(),

            Productos: self.productosEmergencia(),
            Servicios: self.serviciosEmergencia(),
            Examenes: self.examenesEmergencia()
        };
    };

    self.validateModel = function () {
        return true;
    };

    self.registrarEmergencia = function () {
        if (self.validateModel()) {
            self.txtModalConfirmacion("&iquest;Desea registrar la emergencia?")
            $("#mdl-confirmacion").dialog({
                modal: true,
                width: 800,
                buttons: [{
                    text: "Si",
                    class: "btn btn-success",
                    click: function () {
                        showLoading();
                        self.getModel();
                        $.ajax({
                            url: "/Emergencias/GuardarEmergencia",
                            method: "POST",
                            data: model,
                            success: function (result) {
                                hideLoading();
                                let data = JSON.parse(result);
                                if (data.Exitoso) {
                                    window.location.href = "/Emergencias/Lista?ingresadas=false";
                                } else {
                                    console.log(data.Mensaje);
                                    mensajeEmergenteError(data.Mensaje);
                                }
                            },
                            error: function (errorResult) {
                                hideLoading();
                                mensajeEmergenteError("Error de servidor");
                                console.log(errorResult);
                            }
                        })
                    }
                },
                {
                    text: "No",
                    class: "btn btn-danger",
                    click: function () {
                        $(this).dialog("close");
                    }
                }]


            });
        }
    };
    self.editarEmergencia = function () {
        if (self.validateModel()) {
            self.txtModalConfirmacion("&iquest;Desea modificar la emergencia?")
            $("#mdl-confirmacion").dialog({
                modal: true,
                width: 800,
                buttons: [{
                    text: "Si",
                    class: "btn btn-success",
                    click: function () {
                        showLoading();
                        self.getModel();
                        $.ajax({
                            url: "/Emergencias/EditarEmergencia",
                            method: "POST",
                            data: model,
                            success: function (result) {
                                hideLoading();
                                let data = JSON.parse(result);
                                if (data.Exitoso) {
                                    window.location.href = "/Emergencias/Lista?ingresadas=false";
                                } else {
                                    console.log(data.Mensaje);
                                    mensajeEmergenteError(data.Mensaje);
                                }
                            },
                            error: function (errorResult) {
                                hideLoading();
                                mensajeEmergenteError("Error de servidor");
                                console.log(errorResult);
                            }
                        })
                    }
                },
                {
                    text: "No",
                    class: "btn btn-danger",
                    click: function () {
                        $(this).dialog("close");
                    }
                }]


            });
        }
    };
}

var emergenciaVM = new EmergenciaVM();
ko.applyBindings(emergenciaVM);

$(function () {

    contraerMenu();

    emergenciaVM.consultarHabitacionesDisponibles();

    emergenciaVM.consultarPacientesExistentes();

    emergenciaVM.consultarProductosExistentes();
    emergenciaVM.consultarServiciosExistentes();
    emergenciaVM.consultarExamenesExistentes();
    //emergenciaVM.consultarInsumosExistentes();

    //Se consultan los elementos de la emergencia cuando ya esta registrada
    //y se accede la vista de Detalles o Edicion
    if ($("#EmergenciaId").val() != null
        && $("#EmergenciaId").val() != undefined
        && $("#EmergenciaId").val() != "") {
        emergenciaVM.consultarElementosEmergencia();
    }


    $("#tabs-venta").tabs();
    $(".seccion-body").css("display", 'none');
    $(".seccion-header").on('click', function () {
        var nombreSeccion = this.id.replace('-header', '');
        $("#" + nombreSeccion + "-body").slideToggle();
    });

    let habilitarEdicion = $("#HabilitarEdicion").val() == "True" || $("#HabilitarEdicion").val() == "true";
    if (!habilitarEdicion) {
        //lOS CONTROLES TIPO INPUT SE CONVIERTEN EN TEXTO
        //ESTO PERMITE UTILIZAR MENOS HTML Y QUE SEA MAS ORDENADA
        //LA VISTA DE INFORMACION, NUEVA Y EDITAR
        let controlesDatos = document.getElementsByClassName("dato-emergencia");
        $(controlesDatos).each(function (idx, control) {
            var input = control;
            var span = document.createElement('p');
            span.textContent = input.value;
            input.parentNode.replaceChild(span, input);
        });

        let selects = $(".select-emergencia");
        $(selects).each(function (idx, select) {
            $(select).attr("disabled", true);
        });
    }
});


function generarPdfExamenesLaboratorio(examenLaboratorioId, tipoPDF) {
    var pacienteId = emergenciaVM.pacienteId();
    window.open(
        "/CrearPDF/generarPdfExamenesLaboratorio?examenLaboratorioId=" +
        examenLaboratorioId +
        "&pacienteId=" +
        pacienteId +
        "&tipoPDF=" +
        tipoPDF,
        "_blank"
    );
}