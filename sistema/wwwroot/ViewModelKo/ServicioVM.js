var ServicioVM = function () {
    let itemInsumoAgregado = 1;
    var model = {};
    var self = this;
    self.sucursales = ko.observableArray();
    self.precios = ko.observableArray();
    self.precioMostrar = ko.observable();
    self.insumos = ko.observableArray();
    self.productoEquivalencias = ko.observableArray();
    self.unidadesVentaInsumo = ko.observableArray();
    self.insumosUtilizados = ko.observableArray();
    self.insumoSeleccionado = ko.observable();
    self.unidadVentaSeleccionada = ko.observable();
    self.cantidadUtilizadaInsumo = ko.observable();


    self.consultarInsumos = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Servicio/ConsultarInsumosExistentes',
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.insumos(data.Resultado);
                    //self.consultarUnidadesVentaInsumo(self.insumoSeleccionado().ProductoId);
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
    self.consultarInsumosServicio = function () {
        $.ajax({
            method: "POST",
            url: '/Servicio/ConsultarInsumosServicio',
            data: {
                servicioId: $("#Id").val()
            },
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.insumosUtilizados(data.Resultado);
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

    self.consultarPreciosExistentes = function () {
        if ($("#Id").val() == null && $("#Id").val() == undefined) {
            showLoading();
            $.ajax({
                url: "/Servicio/ConsultarPreciosExistentes",
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

    self.consultarSucursalesExistentes = function () {
        if ($("#Id").val() == null && $("#Id").val() == undefined) {
            showLoading();
            $.ajax({
                url: "/Servicio/ConsultarSucursalesExistentes",
                method: "POST",
                success: function (dataResult) {
                    hideLoading();
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        self.sucursales(data.Resultado);
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
    self.consultarSucursalesServicio = function () {
        if ($("#Id").val() != null && $("#Id").val() != undefined) {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Servicio/ConsultarSucursalesServicio',
                data: {
                    servicioId: $("#Id").val()
                },
                success: function (dataResult) {
                    hideLoading();
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        self.sucursales(data.Resultado);
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
        }
    };

    self.consultarPreciosServicio = function () {
        if ($("#Id").val() != null && $("#Id").val() != undefined) {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Servicio/ConsultarPreciosServicio',
                data: {
                    servicioId: $("#Id").val()
                },
                success: function (dataResult) {
                    hideLoading();
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        self.precios(data.Resultado);
                        let precioId = $("#PrecioMostrarId").val();
                        self.precioMostrar(precioId);
                        $("#precio-mostrar-" + $("#PrecioMostrarId").val()).prop("checked", true);
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
        }
    };

    //self.consultarUnidadesVentaInsumo = function (productoId) {
    //    showLoading();
    //    $.ajax({
    //        method: "POST",
    //        url: '/Servicio/ConsultarUnidadesVentaInsumo',
    //        data: {
    //            "productoId": productoId
    //        },
    //        success: function (data, textStatus) {
    //            hideLoading();
    //            if (data.exitoso) {
    //                self.productoEquivalencias(data.resultado);
    //                self.unidadesVentaInsumo([]);
    //                $(data.resultado).each(function (idx, vl) {
    //                    self.unidadesVentaInsumo.push(vl.unidadMedidaVenta);
    //                });
    //            }
    //            else
    //                hideLoading();
    //                alert(data.mensaje);
    //        },
    //        error: function (data) {
    //            hideLoading();
    //            alert(data.error);
    //        }
    //    });
    //};

    self.consultarUnidadesVentaInsumo = function (productoId) {
        showLoading();
        $.ajax({
            method: "POST",
            data: {
                "productoId": productoId
            },
            url: '/LaboratorioClinico/ConsultarUnidadesVentaInsumo',
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.unidadesVentaInsumo(data.Resultado);
                    
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

    self.insumoSeleccionado.subscribe(function (value) {
        self.consultarUnidadesVentaInsumo(value.ProductoId);
    });

    self.agregarInsumo = function () {
        if (self.cantidadUtilizadaInsumo() == null
            || self.cantidadUtilizadaInsumo() == undefined
            || self.cantidadUtilizadaInsumo().trim() == ''
            || isNaN(self.cantidadUtilizadaInsumo().trim())) {
            alert("Proporcione una cantidad valida");
            return false;
        }
        var insumoAgregado = new Object();
        insumoAgregado.Item = itemInsumoAgregado;
        insumoAgregado.ProductoId = self.insumoSeleccionado().ProductoId;
        insumoAgregado.ProductoNombre = self.insumoSeleccionado().ProductoNombre;
        insumoAgregado.UnidadMedidaVentaId = self.unidadVentaSeleccionada().Id;
        insumoAgregado.UnidadMedidaVentaNombre = self.unidadVentaSeleccionada().Nombre;
        insumoAgregado.CantidadUtilizada = self.cantidadUtilizadaInsumo().trim();
        insumoAgregado.Nuevo = true;

        self.insumosUtilizados.push(insumoAgregado);
        itemInsumoAgregado++;
    };
    self.quitarInsumoUtilizado = function (value) {
        $(self.insumosUtilizados()).each(function (idxInsumo, insumo) {
            if (insumo.Item == value.Item) {
                self.insumosUtilizados.splice(idxInsumo, 1);
            }
        });
    };

    self.validateModel = function () {
        let codigo = $("#CodigoInterno").val();
        let nombre = $("#NombreServicio").val();
        if (codigo == null || codigo == undefined || codigo.trim() == '') {
            alert("Proporcione un codigo para el servicio");
            return false;
        }
        if (nombre == null || nombre == undefined || nombre.trim() == '') {
            alert("Proporcione un nombre para el servicio");
            return false;
        }
        if (self.precioMostrar() == null || self.precioMostrar() == undefined) {
            alert("Seleccione un precio para mostrar en reservas");
            return false;
        }
        return true;
    };
    self.getModel = function () {
        model = {
            //Datos del servicio
            "Id": $("#Id").val(),
            "NombreServicio": $("#NombreServicio").val(),
            "Descripcion": $("#Descripcion").val(),
            "CodigoInterno": $("#CodigoInterno").val(),

            //Duracion del servicio
            "DuracionHoras": $("#DuracionHoras").val(),
            "DuracionMinutos": $("#DuracionMinutos").val(),

            //Insumos
            "InsumosUtilizados": self.insumosUtilizados(),

            //Precios
            "Precios": self.precios(),
            "PrecioMostrarId": self.precioMostrar(),

            //Sucursales
            "Sucursales": self.sucursales()
        };
    };
    self.registrarServicio = function () {
        if (self.validateModel()) {
            if (confirm("¿Desea registrar este nuevo servicio?")) {
                showLoading();
                self.getModel();
                $.ajax({
                    method: "POST",
                    url: '/Servicio/Nuevo',
                    data: model,
                    success: function (data, textStatus) {
                        var dataResult = JSON.parse(data);
                        if (dataResult.Exitoso) {
                            window.location.href = "/Servicio/Lista";
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
        }
    };


    self.cancelarRegistroServicio = function () {
        if (confirm("¿Desea cancelar el registro del servicio?")) {
            window.location.href = "/Servicio/Lista";
        }
    };
    self.editarServicio = function () {
        if (self.validateModel()) {
            if (confirm("¿Desea editar este servicio?")) {
                showLoading();
                self.getModel();
                $.ajax({
                    method: "POST",
                    url: '/Servicio/Modificar',
                    data: model,
                    success: function (data, textStatus) {
                        var dataResult = JSON.parse(data);
                        if (dataResult.Exitoso) {
                            window.location.href = "/Servicio/Lista";
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
        }
    };
    self.cancelarEdicionServicio = function () {
        if (confirm("¿Desea cancelar la edición del servicio?")) {
            window.location.href = "/Servicio/Lista";
        }
    };
}

var servicioVm = new ServicioVM();
ko.applyBindings(servicioVm);

$(document).ready(function () {
    servicioVm.consultarPreciosExistentes();
    servicioVm.consultarPreciosServicio();
    servicioVm.consultarSucursalesExistentes();
    servicioVm.consultarSucursalesServicio();
    servicioVm.consultarInsumos();
    //servicioVm.consultarInsumosServicio();
});