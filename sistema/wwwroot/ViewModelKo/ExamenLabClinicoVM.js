var ExamenVM = function () {
    let itemInsumoAgregado = 1;
    let itemExamen = 1;
    let itemPregunta = 1;
    var model = {};
    var self = this;

    self.registrosInventario = ko.observableArray();
    self.precios = ko.observableArray();
  self.preciosInsumo = ko.observableArray();
  self.precioInsumoSeleccionado = ko.observable();
    self.insumos = ko.observableArray();
    self.productoEquivalencias = ko.observableArray();
    self.unidadesVentaInsumo = ko.observableArray();
    self.insumosUtilizados = ko.observableArray();
    self.insumoSeleccionado = ko.observable();
    self.unidadVentaSeleccionada = ko.observable();
    self.cantidadUtilizadaInsumo = ko.observable();
    self.precioCostoInsumo = ko.observable();
    self.totalCostoInsumo = ko.observable();
    self.pregunta = ko.observable();// Variable observable para almacenar la pregunta ingresada
    self.examenesPreguntas = ko.observableArray();//Almacenar las preguntas en los ecamenes



    //Metodo para agregar las preguntas de los examenes
    self.agregarPregunta = function () {
        self.getModel();
        if (self.pregunta() != null
            && self.pregunta() != undefined) {
            let examenPregunta = {
                Item: itemPregunta,
                Pregunta: ko.observable(self.pregunta()),
                Eliminada: ko.observable(false)
            };
            self.examenesPreguntas.push(examenPregunta);
            itemPregunta++;
            self.pregunta(null);
            mensajeEmergente("Pregunta agregada");
        } else {
            alert("Proporcione el texto de la pregunta");
        }
    };
    //Metodo para consultar las preguntas de los examenes con id de examnen
    self.consultarPreguntasExamen = function () {
        showLoading();
        self.examenesPreguntas([]);
        $.ajax({
            method: "POST",
            url: '/LaboratorioClinico/ConsultarPreguntasExamen',
            data: {
                examenLabClinicoId: $("#IdExamen").val()
            },
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    $(data.Resultado).each(function (idx, vl) {
                        vl.Eliminada = ko.observable(vl.Eliminada);
                        vl.Pregunta = ko.observable(vl.Pregunta);
                        self.examenesPreguntas.push(vl);
                    });
                }
                else {
                    mensajeEmergenteError(data.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    //Elminar una pregunta de algun examen
    self.quitarPregunta = function (preguntaQuitar) {
        $(self.examenesPreguntas()).each(function (idx, pregunta) {
            if (pregunta.Item == preguntaQuitar.Item) {
                preguntaQuitar.Eliminada(true);
            }
        });
    };

    self.consultarPreciosExistentes = function () {
        if ($("#IdExamen").val() == null
            || $("#IdExamen").val() == undefined
            || $("#IdExamen").val().trim() == '') {
            showLoading();
            $.ajax({
                url: "/LaboratorioClinico/ConsultarPreciosExistentes",
                method: "POST",
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
                    alert(dataError);
                }
            });
        }
    };
    self.consultarPreciosExamen = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/LaboratorioClinico/ConsultarPreciosExamen',
            data: {
                examenLabClinicoId: $("#IdExamen").val()
            },
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.precios(data.Resultado);
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


    self.cantidadUtilizadaInsumo.subscribe(function (newCantidad) {
        // Asegùrate de que tanto la cantidad como el precio de costo sean nùmeros antes de realizar la operaciùn
        var cantidad = parseFloat(newCantidad);
        var precioCosto = parseFloat(self.precioCostoInsumo());

        // Verifica si tanto la cantidad como el precio de costo son nùmeros vùlidos
        if (!isNaN(cantidad) && !isNaN(precioCosto)) {
            // Realiza la operaciùn de multiplicaciùn
            var totalCosto = cantidad * precioCosto;

            // Actualiza el valor de totalCostoInsumo con el resultado de la operaciùn
            self.totalCostoInsumo(totalCosto);

            // Muestra el resultado en la consola
            //console.log(self.totalCostoInsumo());
        } else {
            // Maneja el caso en el que la cantidad o el precio de costo no sean nùmeros vùlidos
            //console.error("La cantidad o el precio de costo no son nùmeros vùlidos");
        }
    });

    self.precioInsumoSeleccionado.subscribe(function (precioSeleccionado) {
        let precioCompra = 0;
        if (precioSeleccionado != undefined
            && precioSeleccionado != null
            && precioSeleccionado.PrecioCompra != null
            && precioSeleccionado.PrecioCompra != undefined
            && precioSeleccionado.PrecioCompra.trim() != ""
            && !isNaN(precioSeleccionado.PrecioCompra)) {
            precioCompra = precioSeleccionado.PrecioCompra;
        }
        self.precioCostoInsumo(precioCompra);
    });
    self.quitarInsumoUtilizado = function (value) {
        $(self.insumosUtilizados()).each(function (idxInsumo, insumo) {
            if (insumo.Item == value.Item) {
                self.insumosUtilizados.splice(idxInsumo, 1);
            }
        });
    };

    self.getModel = function () {
        model = {
            "IdExamen": $("#IdExamen").val(),
            "NombreExamen": $("#NombreExamen").val(),
            "TipoExamen": $("#TipoExamen").val(),
            "CodigoInterno": $("#CodigoInterno").val(),
            "CategoriaLabClinicoId": $("#CategoriaLabClinicoId").val(),
            "Indicaciones": $("#Indicaciones").val(),
            "Instrucciones": $("#Instrucciones").val(),
            "Advertencias": $("#Advertencias").val(),
            "DuracionHoras": $("#DuracionHoras").val(),
            "DuracionMinutos": $("#DuracionMinutos").val(),
            "DeclaracionConsentimiento": $("#DeclaracionConsentimiento").val(),
            //Insumos
            "InsumosUtilizados": self.insumosUtilizados(),

            "Precios": self.precios(),
            "Preguntas": self.examenesPreguntas()
        };
    };
    self.registrarExamen = function () {
        if (confirm("ùDesea registrar este nuevo examen?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/LaboratorioClinico/CrearExamen',
                data: model,
                success: function (data, textStatus) {
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        window.location.href = "/LaboratorioClinico/ListaExamenes";
                    }
                    else {
                        hideLoading();
                        alert(dataResult.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }
    };
    self.modificarExamen = function () {
        if (confirm("ùDesea editar este examen?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/LaboratorioClinico/ModificarExamenLab',
                data: model,
                success: function (data, textStatus) {
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        window.location.href = "/LaboratorioClinico/ListaExamenes";
                    }
                    else {
                        hideLoading();
                        alert(dataResult.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }
    };

    self.consultarInsumosExistentes = function () {

        let textoCargando = $("#texto-cargando-insumos-existentes");
        let textoError = $("#texto-error-consultar-insumos-existentes");
        self.insumos([]);
        textoCargando.show();
        textoError.hide();
        $.ajax({
            method: "POST",
            url: '/LaboratorioClinico/ConsultarInsumosExistentes',
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
                                    ProductoNombre: vl2.ProductoNombre
                                };
                                self.insumos.push(productoExistente);
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

    // Consultar unidades de venta del producto seleccionado
    self.consultarUnidadesVentaInsumo = function (producto) {
        if (self.insumoSeleccionado() == null
            || self.insumoSeleccionado() == undefined
            || producto == null
            || producto == undefined
            || producto.ProductoId == null
            || producto.ProductoId == undefined) {
            mensajeEmergenteError("No hay ningun producto valido seleccionado");
            return false;
        }
        let productoId = producto.ProductoId;
        self.unidadesVentaInsumo([]);
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
                    self.unidadesVentaInsumo.push(unidadAgregada);
                    agregado = true;
                }
            });
        }

    };

    // Consultar precios del producto seleccionado
    self.consultarPreciosProducto = function (unidadSeleccionada) {
        if (unidadSeleccionada == null || unidadSeleccionada == undefined) {
            self.preciosInsumo([]);
            return;
        }

        if (self.insumoSeleccionado() == null || self.insumoSeleccionado() == undefined) {
            mensajeEmergenteError("No hay ningun insumo valido seleccionado");
        }
        self.preciosInsumo([]);
        let registrosInventarioProducto = new Array();
        $(self.registrosInventario()).each(function (idx, registro) {
            if (registro.ProductoId == self.insumoSeleccionado().ProductoId && registro.UnidadMedidaVentaId == unidadSeleccionada.Id) {
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
                self.preciosInsumo.push(vl);
            });

        }
    };


    self.insumoSeleccionado.subscribe(function (insumo) {
        self.unidadVentaSeleccionada(null);
        self.precioInsumoSeleccionado(null);
        self.preciosInsumo([]);
        self.precioCostoInsumo(null);
        self.consultarUnidadesVentaInsumo(insumo);
    });
    self.unidadVentaSeleccionada.subscribe(function (unidadSeleccionada) {
        self.precioInsumoSeleccionado(null);
        self.consultarPreciosProducto(unidadSeleccionada);
    });
    self.precioInsumoSeleccionado.subscribe(function (precioSeleccionado) {
        var precioCompra = 0;
        if (precioSeleccionado != undefined
            && precioSeleccionado != null
            && precioSeleccionado.PrecioCompra != null
            && precioSeleccionado.PrecioCompra != undefined
            && String(precioSeleccionado.PrecioCompra).trim() != ""
            && !isNaN(precioSeleccionado.PrecioCompra)) {
            precioCompra = precioSeleccionado.PrecioCompra;
        }
        self.precioCostoInsumo(precioCompra);
    });

    self.actualizarTotales = function () {
        //Insumos
        let cantidadInsumo = self.cantidadUtilizadaInsumo();
        let precioCosto = self.precioCostoInsumo();
        let totalCostoInsumo = cantidadInsumo * precioCosto;
        self.totalCostoInsumo(totalCostoInsumo);
    };


    self.agregarInsumo = function () {
        if (self.cantidadUtilizadaInsumo() == null
            || self.cantidadUtilizadaInsumo() == undefined) {
            alert("Proporcione una cantidad valida");
            return false;
        }
        var insumoAgregado = new Object();
        insumoAgregado.Item = itemInsumoAgregado;
        insumoAgregado.ProductoId = self.insumoSeleccionado().ProductoId;
        insumoAgregado.ProductoNombre = self.insumoSeleccionado().ProductoNombre;
        insumoAgregado.UnidadMedidaVentaId = (self.unidadVentaSeleccionada() != null && self.unidadVentaSeleccionada() != undefined)
            ? self.unidadVentaSeleccionada().Id
            : null;
        insumoAgregado.UnidadMedidaVentaNombre = (self.unidadVentaSeleccionada() != null && self.unidadVentaSeleccionada() != undefined)
            ? self.unidadVentaSeleccionada().UnidadMedidaVentaNombre
            : "UN";
        let precioCostoInsumo = 0;
        if (!isNaN(self.precioCostoInsumo()) && self.precioCostoInsumo() != null
            && self.precioCostoInsumo() != undefined) {
            precioCostoInsumo = parseFloat(self.precioCostoInsumo());
        }
        insumoAgregado.PrecioCostoInsumo = precioCostoInsumo;
        insumoAgregado.CantidadUtilizada = parseFloat(self.cantidadUtilizadaInsumo());
        insumoAgregado.TotalInsumo = parseFloat(self.totalCostoInsumo());
        insumoAgregado.Nuevo = true;

        self.insumosUtilizados.push(insumoAgregado);
        itemInsumoAgregado++;
    };

    self.consultarInsumosPreCargadosExamenes = function (idExamen) {

        showLoading();
        $.ajax({
            method: "POST",
            data: {
                "idExamen": idExamen
            },
            url: '/LaboratorioClinico/consultarInsumosPreCargadosExamenes',
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    $(data.Resultado).each(function (idx, value) {
                        value.Item = itemInsumoAgregado;
                        self.insumosUtilizados.push(value);
                        itemInsumoAgregado++;
                    });
                }
                else {
                    //debugger;
                    hideLoading();
                    alert(data.Mensaje);
                    console.log("Error ajax");
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });

    };
    //self.cancelarEdicionServicio = function () {
    //    if (confirm("ùDesea cancelar la ediciùn del servicio?")) {
    //        window.location.href = "/Servicio/Lista";
    //    }
    //};
}

var examenVm = new ExamenVM();
ko.applyBindings(examenVm);

$(function () {
    examenVm.consultarPreciosExistentes();
    if ($("#IdExamen").val() != null &&
        $("#IdExamen").val() != undefined
        && $("#IdExamen").val() != '') {
        examenVm.consultarPreciosExamen();
    }
    examenVm.consultarInsumosExistentes();

    examenVm.consultarPreguntasExamen();

    //validamos si el examen tiene insumos agregados
    if ($("#IdExamen").val() != null &&
        $("#IdExamen").val() != undefined
        && $("#IdExamen").val() != '') {
        var idExamen = $("#IdExamen").val();
        examenVm.consultarInsumosPreCargadosExamenes(idExamen);
    }

});