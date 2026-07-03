var ClinicaMedicamentoVM = function () {
    let itemEquivalencia = 1;
    var model = {};

    var self = this;
    self.equivalenciasProducto = ko.observableArray();
    self.unidadMedidaCompraSeleccionada = ko.observable();
    self.unidadMedidaVentaSeleccionada = ko.observable();
    //Categoria
    self.categorias = ko.observableArray();
    self.categoriaSeleccionada = ko.observable();
    self.nombreNuevaCategoria = ko.observable();
    //Marca
    self.marcas = ko.observableArray();
    self.marcaSeleccionada = ko.observable();
    self.nombreNuevaMarca = ko.observable();
    //Grupo
    self.gruposT = ko.observableArray();
    self.grupoTSeleccionado = ko.observable();
    self.nombreNuevoGrupoT = ko.observable();
    //Presentación
    self.presentaciones = ko.observableArray();
    self.presentacionSeleccionada = ko.observable();
    self.nombreNuevaPresentacion = ko.observable();
    //Via de administracion
    self.viasAdministracion = ko.observableArray();
    self.viaAdministracionSeleccionada = ko.observable();
    self.nombreNuevaViaAdministracion = ko.observable();
    //Laboratorio
    self.laboratorios = ko.observableArray();
    self.laboratorioSeleccionado = ko.observable();
    self.nombreNuevoLaboratorio = ko.observable();
    //Unidad de compra
    self.unidadesCompra = ko.observableArray();
    self.nombreNuevaUnidadCompra = ko.observable();
    self.abreviaturaNuevaUnidadCompra = ko.observable();
    //Unidad de venta
    self.unidadesVenta = ko.observableArray();
    self.nombreNuevaUnidadVenta = ko.observable();
    self.abreviaturaNuevaUnidadVenta = ko.observable();

    self.seleccionarUnidadCompra = function (value) {
        $(".unidad-compra").removeClass("unidad-seleccionada");
        $("#unidad-compra-" + value.Id).addClass("unidad-seleccionada");
        self.unidadMedidaCompraSeleccionada(value);
    };
    self.seleccionarUnidadVenta = function (value) {
        $(".unidad-venta").removeClass("unidad-seleccionada");
        $("#unidad-venta-" + value.Id).addClass("unidad-seleccionada");
        self.unidadMedidaVentaSeleccionada(value);
    };


    //Categorias
    self.consultarCategorias = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Productos/ConsultarCategorias',
            success: function (data, textStatus) {
                hideLoading();
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.categorias(dataResult.Resultado);
                    $(self.categorias()).each(function (idx, value) {
                        if (value.Id == $("#CategoriaId").val()) {
                            self.categoriaSeleccionada(value);
                        }
                    });
                }
                else {
                    alert(dataResult.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.registrarNuevaCategoria = function () {

        //Validaciones
        let nombreCategoria = self.nombreNuevaCategoria();
        if (nombreCategoria == undefined || nombreCategoria == null || nombreCategoria.trim() == '') {
            alert("Proporcione el nombre de la categoría");
            return false;
        }

        if (confirm("¿Desea agregar esta nueva categoría?")) {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Categoria/NuevaCategoriaProducto',
                data: {
                    NombreCategoria: self.nombreNuevaCategoria()
                },
                success: function (data, textStatus) {
                    hideLoading();
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        self.nombreNuevaCategoria(null);
                        self.consultarCategorias();
                    }
                    else
                        alert(dataResult.Mensaje);
                },
                error: function (data) {
                    alert(data.error);
                }
            });
        }
    };

    //Grupos T
    self.consultarGruposT = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Productos/ConsultarGruposT',
            success: function (data, textStatus) {
                hideLoading();
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.gruposT(dataResult.Resultado);
                    $(self.gruposT()).each(function (idx, value) {
                        if (value.Id == $("#GrupoId").val()) {
                            self.grupoTSeleccionado(value);
                        }
                    });
                }
                else {
                    alert(dataResult.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.registrarNuevoGrupoT = function () {

        //Validaciones
        let nombreGrupo = self.nombreNuevoGrupoT();
        if (nombreGrupo == undefined || nombreGrupo == null || nombreGrupo.trim() == '') {
            alert("Proporcione el nombre del grupo terapeutico");
            return false;
        }

        if (confirm("¿Desea agregar este nuevo grupo terapéutico?")) {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Categoria/NuevoGrupoT',
                data: {
                    NombreGrupoT: self.nombreNuevoGrupoT()
                },
                success: function (data, textStatus) {
                    hideLoading();
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        mensajeEmergente("Grupo terapeutico registrado");
                        self.nombreNuevoGrupoT(null);
                        self.consultarGruposT();
                    }
                    else
                        alert(dataResult.Mensaje);
                },
                error: function (data) {
                    alert(data.error);
                }
            });
        }
    };

    //Marcas
    self.consultarMarcas = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Productos/ConsultarMarcas',
            success: function (data, textStatus) {
                hideLoading();
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.marcas(dataResult.Resultado);
                    $(self.marcas()).each(function (idx, value) {
                        if (value.Id == $("#MarcaId").val()) {
                            self.marcaSeleccionada(value);
                        }
                    });
                }
                else {
                    alert(dataResult.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.registrarNuevaMarca = function () {

        //Validaciones
        let nombreMarca = self.nombreNuevaMarca();
        if (nombreMarca == undefined || nombreMarca == null || nombreMarca.trim() == '') {
            alert("Proporcione el nombre de la marca");
            return false;
        }

        if (confirm("¿Desea agregar esta nueva marca?")) {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Categoria/NuevaMarcaProducto',
                data: {
                    NombreMarca: self.nombreNuevaMarca()
                },
                success: function (data, textStatus) {
                    hideLoading();
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        mensajeEmergente("Marca registrada");
                        self.nombreNuevaMarca(null);
                        self.consultarMarcas();
                    }
                    else
                        alert(dataResult.Mensaje);
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }
    };

    //Presentaciones
    self.consultarPresentaciones = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Productos/ConsultarPresentaciones',
            success: function (data, textStatus) {
                hideLoading();
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.presentaciones(dataResult.Resultado);
                    $(self.presentaciones()).each(function (idx, value) {
                        if (value.Id == $("#PresentacionId").val()) {
                            self.presentacionSeleccionada(value);
                        }
                    });
                }
                else {
                    alert(dataResult.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.registrarNuevaPresentacion = function () {

        //Validaciones
        let nombrePresentacion = self.nombreNuevaPresentacion();
        if (nombrePresentacion == undefined || nombrePresentacion == null || nombrePresentacion.trim() == '') {
            alert("Proporcione el nombre de la presentacion");
            return false;
        }

        if (confirm("¿Desea agregar esta nueva presentación?")) {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Categoria/NuevaPresentacionProducto',
                data: {
                    NombrePresentacion: self.nombreNuevaPresentacion()
                },
                success: function (data, textStatus) {
                    hideLoading();
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        self.nombreNuevaPresentacion(null);
                        self.consultarPresentaciones();
                    }
                    else
                        alert(dataResult.Mensaje);
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }
    };

    //Vias de administracion
    self.consultarViasAdministracion = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Productos/ConsultarViasAdministracion',
            success: function (data, textStatus) {
                hideLoading();
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.viasAdministracion(dataResult.Resultado);
                    $(self.viasAdministracion()).each(function (idx, value) {
                        if (value.Id == $("#ViadminId").val()) {
                            self.viaAdministracionSeleccionada(value);
                        }
                    });
                }
                else {
                    alert(dataResult.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.registrarNuevaViaAdministracion = function () {

        //Validaciones
        let nombreVia = self.nombreNuevaViaAdministracion();
        if (nombreVia == undefined || nombreVia == null || nombreVia.trim() == '') {
            alert("Proporcione el nombre de la via de administracion");
            return false;
        }

        if (confirm("¿Desea agregar esta nueva via de administracion?")) {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Categoria/NuevaViaAdministracion',
                data: {
                    NombreViaAdministracion: self.nombreNuevaViaAdministracion()
                },
                success: function (data, textStatus) {
                    hideLoading();
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        self.nombreNuevaViaAdministracion(null);
                        self.consultarViasAdministracion();
                    }
                    else
                        alert(dataResult.Mensaje);
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }
    };

    //Laboratorios
    self.consultarLaboratorios = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Productos/ConsultarLaboratorios',
            success: function (data, textStatus) {
                hideLoading();
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.laboratorios(dataResult.Resultado);
                    $(self.laboratorios()).each(function (idx, value) {
                        if (value.Id == $("#LaboratorioId").val()) {
                            self.laboratorioSeleccionado(value);
                        }
                    });
                }
                else {
                    alert(dataResult.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.registrarNuevoLaboratorio = function () {

        //Validaciones
        let nombreLaboratorio = self.nombreNuevoLaboratorio();
        if (nombreLaboratorio == undefined || nombreLaboratorio == null || nombreLaboratorio.trim() == '') {
            alert("Proporcione el nombre del laboratorio");
            return false;
        }

        if (confirm("¿Desea agregar este nuevo laboratorio?")) {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Categoria/NuevoLaboratorio',
                data: {
                    NombreLaboratorio: self.nombreNuevoLaboratorio()
                },
                success: function (data, textStatus) {
                    hideLoading();
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        self.nombreNuevoLaboratorio(null);
                        self.consultarLaboratorios();
                    }
                    else
                        alert(dataResult.Mensaje);
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }
    };

    //Unidades de compra
    self.consultarUnidadesCompra = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Productos/ConsultarUnidadesCompra',
            success: function (data, textStatus) {
                hideLoading();
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.unidadesCompra(dataResult.Resultado);
                }
                else
                    alert(dataResult.Mensaje);
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.registrarNuevaUnidadCompra = function () {

        //Validaciones
        let nombreUnidad = self.nombreNuevaUnidadCompra();
        if (nombreUnidad == undefined || nombreUnidad == null || nombreUnidad.trim() == '') {
            alert("Proporcione el nombre de la unidad");
            return false;
        }

        if (confirm("¿Desea agregar esta nueva unidad de compra?")) {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Productos/AgregarUnidadCompra',
                data: {
                    nombreUnidad: self.nombreNuevaUnidadCompra(),
                    abreviatura: self.abreviaturaNuevaUnidadCompra()
                },
                success: function (data, textStatus) {
                    hideLoading();
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        mensajeEmergente("Unidad registrada");
                        self.nombreNuevaUnidadCompra('');
                        self.abreviaturaNuevaUnidadCompra('');
                        self.consultarUnidadesCompra();
                    }
                    else
                        alert(dataResult.Mensaje);
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }
    };

    //Unidades de venta
    self.consultarUnidadesVenta = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Productos/ConsultarUnidadesVenta',
            success: function (data, textStatus) {
                hideLoading();
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso)
                    self.unidadesVenta(dataResult.Resultado);
                else
                    alert(dataResult.Mensaje);
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.registrarNuevaUnidadVenta = function () {

        //Validaciones
        let nombreUnidad = self.nombreNuevaUnidadVenta();
        if (nombreUnidad == undefined || nombreUnidad == null || nombreUnidad.trim() == '') {
            alert("Proporcione el nombre de la unidad");
            return false;
        }

        if (confirm("¿Desea agregar esta nueva unidad de venta?")) {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Productos/AgregarUnidadVenta',
                data: {
                    nombreUnidad: self.nombreNuevaUnidadVenta(),
                    abreviatura: self.abreviaturaNuevaUnidadVenta()
                },
                success: function (data, textStatus) {
                    hideLoading();
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        mensajeEmergente("Unidad registrada");
                        self.nombreNuevaUnidadVenta('');
                        self.abreviaturaNuevaUnidadVenta('');
                        self.consultarUnidadesVenta();
                    }
                    else
                        alert(dataResult.Mensaje);
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }
    };

    //Equivalencias
    self.agregarEquivalencia = function () {
        if (self.unidadMedidaCompraSeleccionada() == null || self.unidadMedidaCompraSeleccionada() == undefined) {
            alert("Seleccione una unidad de compra");
            return false;
        }
        if (self.unidadMedidaVentaSeleccionada() == null || self.unidadMedidaVentaSeleccionada() == undefined) {
            alert("Seleccione una unidad de venta");
            return false;
        }
        var equivalencia = new Object();
        equivalencia.Item = itemEquivalencia;
        equivalencia.Id = null;
        equivalencia.ProductoId = null;
        equivalencia.UnidadMedidaCompraId = self.unidadMedidaCompraSeleccionada().Id;
        equivalencia.UnidadMedidaCompraNombre = self.unidadMedidaCompraSeleccionada().Nombre;
        equivalencia.PrecioUnidadCompra = 0;
        equivalencia.UnidadMedidaVentaId = self.unidadMedidaVentaSeleccionada().Id;
        equivalencia.UnidadMedidaVentaNombre = self.unidadMedidaVentaSeleccionada().Nombre;
        equivalencia.CantidadEquivalente = 0;
        equivalencia.PrecioUnidadVenta = 0;
        equivalencia.PrecioUnidadVenta_1 = 0;
        self.equivalenciasProducto.push(equivalencia);
        itemEquivalencia++;
    };
    self.quitarEquivalencia = function (value) {
        $(self.equivalenciasProducto()).each(function (idx, equivalencia) {
            if (equivalencia.Item == value.Item) {
                self.equivalenciasProducto.splice(idx, 1);
            }
        });
    };
    self.consultarEquivalencias = function (value) {
        showLoading();
        self.equivalenciasProducto([]);
        if ($("#Id").val() != '') {
            $.ajax({
                url: "/Productos/ConsultarEquivalencias",
                method: "POST",
                data: {
                    productoId: $("#Id").val()
                },
                success: function (dataResult) {
                    hideLoading();
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        $(data.Resultado).each(function (idx, value) {
                            value.Item = itemEquivalencia;
                            self.equivalenciasProducto.push(value);
                            itemEquivalencia++;
                        });
                    } else {
                        console.log(data.Mensaje);
                        alert(data.Mensaje);
                    }
                }
            })
        }
    };

    self.getModel = function () {
        let categoriaId = null;
        if (self.categoriaSeleccionada() != null && self.categoriaSeleccionada() != undefined) {
            categoriaId = self.categoriaSeleccionada().Id;
        }
        console.log(categoriaId);
        let marcaId = null;
        if (self.marcaSeleccionada() != null && self.marcaSeleccionada() != undefined) {
            marcaId = self.marcaSeleccionada().Id;
        }
        let grupoId = null;
        if (self.grupoTSeleccionado() != null && self.grupoTSeleccionado() != undefined) {
            grupoId = self.grupoTSeleccionado().Id;
        }
        let presentacionId = null;
        if (self.presentacionSeleccionada() != null && self.presentacionSeleccionada() != undefined) {
            presentacionId = self.presentacionSeleccionada().Id;
        }
        let viadminId = null;
        if (self.viaAdministracionSeleccionada() != null && self.viaAdministracionSeleccionada() != undefined) {
            viadminId = self.viaAdministracionSeleccionada().Id;
        }
        let laboratorioId = null;
        if (self.laboratorioSeleccionado() != null && self.laboratorioSeleccionado() != undefined) {
            laboratorioId = self.laboratorioSeleccionado().Id;
        }

        model = {
            "Id": $("#Id").val(),
            "CodigoReferencia": $("#CodigoReferencia").val(),
            "Nombre": $("#Nombre").val(),
            "CategoriaId": categoriaId,
            "MarcaId": marcaId,
            "GrupoId": grupoId,
            "PresentacionId": presentacionId,
            "ViadminId": viadminId,
            "LaboratorioId": laboratorioId,
            "UrlImagen": $("#UrlImagen").val(),
            "ActivoConcentracion": $("#ActivoConcentracion").val(),
            "Descripcion": $("#Descripcion").val(),

            "Equivalencias": self.equivalenciasProducto()
        };
    };

    self.validateModel = function () {
        if (
            $("#CodigoReferencia").val() == null
            || $("#CodigoReferencia").val() == undefined
            || $("#CodigoReferencia").val().trim() == ''
        ) {
            alert("Proporcione un codigo de referencia");
            return false;
        }
        if (
            $("#Nombre").val() == null
            || $("#Nombre").val() == undefined
            || $("#Nombre").val().trim() == ''
        ) {
            alert("Proporcione un nombre de producto");
            return false;
        }
        return true;
    };

    self.registrarProducto = function () {
        if (self.validateModel()) {
            if (confirm("¿Desea registrar este medicamento?")) {
                showLoading();
                self.getModel();
                $.ajax({
                    method: "POST",
                    url: '/Productos/ClinicaMedicamentosNuevo',
                    data: model,
                    success: function (data, textStatus) {
                        var dataResult = JSON.parse(data);
                        if (dataResult.Exitoso)
                            window.location.href = "/Productos/ClinicaMedicamentos/";
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
    self.editarProducto = function () {
        //Validaciones
        if (
            $("#CodigoReferencia").val() == null
            || $("#CodigoReferencia").val() == undefined
            || $("#CodigoReferencia").val().trim() == ''
        ) {
            alert("Proporcione un codigo de referencia");
            return false;
        }
        if (
            $("#Nombre").val() == null
            || $("#Nombre").val() == undefined
            || $("#Nombre").val().trim() == ''
        ) {
            alert("Proporcione un nombre de producto");
            return false;
        }

        if (confirm("¿Desea editar este medicamento?")) {
            showLoading();
            self.getModel();

            $.ajax({
                method: "POST",
                url: '/Productos/ClinicaMedicamentosModificar',
                data: model,
                success: function (data, textStatus) {
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso)
                        window.location.href = "/Productos/ClinicaMedicamentos/";
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

    self.cancelarRegistroProducto = function () {
        if (confirm("¿Desea cancelar el registro de este insumo?")) {
            window.location.href = "/Productos/ClinicaMedicamentos/";
        }
    };
    self.cancelarEdicionProducto = function () {
        if (confirm("¿Desea cancelar la edición de este insumo?")) {
            window.location.href = "/Productos/ClinicaMedicamentos/";
        }
    };
}

var medicamentoVm = new ClinicaMedicamentoVM();
ko.applyBindings(medicamentoVm);

$(document).ready(function () {
    medicamentoVm.consultarUnidadesCompra();
    medicamentoVm.consultarUnidadesVenta();

    medicamentoVm.consultarCategorias();
    medicamentoVm.consultarMarcas();
    medicamentoVm.consultarGruposT();
    medicamentoVm.consultarPresentaciones();
    medicamentoVm.consultarViasAdministracion();
    medicamentoVm.consultarLaboratorios();
    medicamentoVm.consultarEquivalencias();
});