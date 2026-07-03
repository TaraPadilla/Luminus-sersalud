var InventarioProductoVM = function () {
    let itemEquivalencia = 1;
    var model = {};

    var self = this;
    self.equivalenciasProducto = ko.observableArray();
    self.unidadMedidaCompraSeleccionada = ko.observable();
    //self.unidadMedidaVentaSeleccionada = ko.observable();
    self.unidadesVentaSeleccionadas = ko.observableArray();
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
    //Presentaci�n
    self.presentaciones = ko.observableArray();
    self.presentacionSeleccionada = ko.observable();
    self.presentacionSeleccionada2 = ko.observable();
    self.presentacionSeleccionada3 = ko.observable();
    self.presentacionSeleccionada4 = ko.observable();
    self.presentacionSeleccionada5 = ko.observable();

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
        //$(".unidad-venta").removeClass("unidad-seleccionada");
        let elementoUnidad = $("#unidad-venta-" + value.Id);
        if (!elementoUnidad.hasClass("unidad-seleccionada")) {
            elementoUnidad.addClass("unidad-seleccionada");
            //self.unidadMedidaVentaSeleccionada(value);
            self.unidadesVentaSeleccionadas.push(value);
        } else {
            elementoUnidad.removeClass("unidad-seleccionada");
        }
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
            alert("Proporcione el nombre de la categor�a");
            return false;
        }

        if (confirm("�Desea agregar esta nueva categor�a?")) {
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

        if (confirm("�Desea agregar este nuevo grupo terap�utico?")) {
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

        if (confirm("�Desea agregar esta nueva marca?")) {
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

    self.visibleSelectsCount = ko.observable(1);  // Inicialmente solo un select visible

    self.inicializarPresentacionesVisibles = function () {
        let count = 1; // Por defecto siempre mostramos el primer selector

        if ($("#PresentacionId2").val()) {
            count = 2;
        }
        if ($("#PresentacionId3").val()) {
            count = 3;
        }
        if ($("#PresentacionId4").val()) {
            count = 4;
        }
        if ($("#PresentacionId5").val()) {
            count = 5;
        }

        self.visibleSelectsCount(count);
    };

    self.showSelect2 = ko.computed(function () {
        return self.visibleSelectsCount() >= 2;
    });
    self.showSelect3 = ko.computed(function () {
        return self.visibleSelectsCount() >= 3;
    });
    self.showSelect4 = ko.computed(function () {
        return self.visibleSelectsCount() >= 4;
    });
    self.showSelect5 = ko.computed(function () {
        return self.visibleSelectsCount() >= 5;
    });

    self.addSelect = function () {
        if (self.visibleSelectsCount() < 5) {
            self.visibleSelectsCount(self.visibleSelectsCount() + 1);
        }
    };

    self.removeSelect = function () {
        if (self.visibleSelectsCount() > 1) {
            self.visibleSelectsCount(self.visibleSelectsCount() - 1);
            // Limpiar el valor del select que se oculta
            switch (self.visibleSelectsCount()) {
                case 1:
                    self.presentacionSeleccionada2(null);
                    break;
                case 2:
                    self.presentacionSeleccionada3(null);
                    break;
                case 3:
                    self.presentacionSeleccionada4(null);
                    break;
                case 4:
                    self.presentacionSeleccionada5(null);
                    break;
            }
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
                        // Para la primera presentación
                        if (value.Id == $("#PresentacionId").val()) {
                            self.presentacionSeleccionada(value);
                        }
                        // Para la segunda presentación
                        if (value.Id == $("#PresentacionId2").val()) {
                            self.presentacionSeleccionada2(value);
                        }
                        // Para la tercera presentación
                        if (value.Id == $("#PresentacionId3").val()) {
                            self.presentacionSeleccionada3(value);
                        }
                        // Para la cuarta presentación
                        if (value.Id == $("#PresentacionId4").val()) {
                            self.presentacionSeleccionada4(value);
                        }
                        // Para la quinta presentación
                        if (value.Id == $("#PresentacionId5").val()) {
                            self.presentacionSeleccionada5(value);
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

        if (confirm("�Desea agregar esta nueva presentaci�n?")) {
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

        if (confirm("�Desea agregar esta nueva via de administracion?")) {
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

        if (confirm("�Desea agregar este nuevo laboratorio?")) {
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

        if (confirm("�Desea agregar esta nueva unidad de compra?")) {
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

        if (confirm("�Desea agregar esta nueva unidad de venta?")) {
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
        //if (self.unidadMedidaVentaSeleccionada() == null
        //    || self.unidadMedidaVentaSeleccionada() == undefined) {
        //    alert("Seleccione una unidad de venta");
        //    return false;
        //}
        if (self.unidadesVentaSeleccionadas() == null
            || self.unidadesVentaSeleccionadas() == undefined
            || self.unidadesVentaSeleccionadas().length == 0) {
            alert("Seleccione al menos una unidad de venta");
            return false;
        }

        $(self.unidadesVentaSeleccionadas()).each(function (idxUnidad, vlUnidad) {
            var equivalencia = new Object();
            equivalencia.Item = itemEquivalencia;
            equivalencia.Id = null;
            equivalencia.ProductoId = null;
            equivalencia.UnidadMedidaCompraId = self.unidadMedidaCompraSeleccionada().Id;
            equivalencia.UnidadMedidaCompraNombre = self.unidadMedidaCompraSeleccionada().Nombre;
            equivalencia.PrecioUnidadCompra = 0;
            equivalencia.UnidadMedidaVentaId = vlUnidad.Id;
            equivalencia.UnidadMedidaVentaNombre = vlUnidad.Nombre;
            equivalencia.CantidadEquivalente = 0;
            equivalencia.PrecioUnidadVenta = 0;
            equivalencia.PrecioUnidadVenta_1 = 0;
            self.equivalenciasProducto.push(equivalencia);
            itemEquivalencia++;
        });

        $(".unidad-venta").removeClass("unidad-seleccionada");
        self.unidadesVentaSeleccionadas([]);
        $(".unidad-compra").removeClass("unidad-seleccionada");
        self.unidadMedidaCompraSeleccionada(null);
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
            "TipoBodegaId": $("#TipoBodegaId").val(),
            "TipoProductoId": $("#TipoProductoId").val(),
            "CodigoReferencia": $("#CodigoReferencia").val(),
            "Nombre": $("#Nombre").val(),
            "CategoriaId": categoriaId,
            "MarcaId": marcaId,
            "GrupoId": grupoId,
            "PresentacionId": self.presentacionSeleccionada()?.Id,
            "PresentacionId2": self.presentacionSeleccionada2()?.Id,
            "PresentacionId3": self.presentacionSeleccionada3()?.Id,
            "PresentacionId4": self.presentacionSeleccionada4()?.Id,
            "PresentacionId5": self.presentacionSeleccionada5()?.Id,
            "ViadminId": viadminId,
            "LaboratorioId": laboratorioId,
            "UrlImagen": $("#UrlImagen").val(),
            "ActivoConcentracion": $("#ActivoConcentracion").val(),
            "Descripcion": $("#Descripcion").val(),
            "Ubicacion": $("#Ubicacion").val(),
            "AmbienteId": $("#AmbienteId").val(),
            "Equivalencias": self.equivalenciasProducto()
        };
    };

    self.validateModel = function () {

        //Tipo de producto
        if (
            $("#TipoProductoId").val() == null
            || $("#TipoProductoId").val() == undefined
            || $("#TipoProductoId").val().trim() == ''
        ) {
            alert("Seleccione el tipo de producto");
            return false;
        }

        //Codigo de referencia
        if (
            $("#CodigoReferencia").val() == null
            || $("#CodigoReferencia").val() == undefined
            || $("#CodigoReferencia").val().trim() == ''
        ) {
            alert("Proporcione un codigo de referencia");
            return false;
        }

        //Nombre de producto
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
        // debugger;
        var ambienteId = $("#AmbienteId").val();
        console.log("Ambiente n� " + ambienteId)



        if (self.validateModel()) {
            if (confirm("�Desea registrar este producto?")) {
                showLoading();
                self.getModel();
                $.ajax({
                    method: "POST",
                    url: '/Productos/InventarioProductoNuevo',
                    data: model,
                    success: function (data, textStatus) {
                        var dataResult = JSON.parse(data);
                        if (dataResult.Exitoso)
                            window.location.href = "/Productos/Inventario?" +
                                "AmbienteId=" + parseInt(ambienteId) +
                                "&TipoProductoId=" + model.TipoProductoId;
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
        var ambienteId = $("#AmbienteId").val();
        console.log("Ambiente n� " + ambienteId)
        if (self.validateModel()) {
            if (confirm("�Desea editar este producto?")) {
                showLoading();
                self.getModel();

                $.ajax({
                    method: "POST",
                    url: '/Productos/InventarioProductoModificar',
                    data: model,
                    success: function (data, textStatus) {
                        var dataResult = JSON.parse(data);
                        if (dataResult.Exitoso)
                            window.close();
                            // window.location.href = "/Productos/Inventario?" +
                            //     "AmbienteId=" + parseInt(ambienteId) +
                            //     "&TipoProductoId=" + model.TipoProductoId;
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

    self.editarProductoVencido = function () {
        if (confirm("�Desea editar  la fecha de vencimiento de este producto?")) {
            showLoading();


            $.ajax({
                method: "POST",
                url: '/Productos/InventarioProductoModificarVencidos',
                data: {

                    idProdictoInventario: $("#Id").val(),
                    fechaVencimiento: $("#FechaVencimiento").val()

                },
                success: function (data, textStatus) {

                    var dataResult = JSON.parse(data);
                    debugger;
                    if (dataResult.Exitoso)
                        switch ($("#TipoBodegaId").val()) {
                            case "1":
                                window.location.href = "/Productos/VencidosFarmacia";
                                break;
                            case "2":
                                window.location.href = "/Productos/VencidosClinica";
                                break;
                            case "4":
                                window.location.href = "/Productos/VencidosLaboratorio";
                                break;

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

        //console.log($("#FechaVencimiento").val())
        //console.log($("#TipoBodegaId").val())
    };




    //self.cancelarRegistroProducto = function () {
    //    debugger;
    //    if (confirm("�Desea cancelar el registro de este insumo?")) {
    //        window.location.href = "/Productos/ClinicaMedicamentos/";
    //    }
    //};

    self.cancelarRegistroProducto = function () {
        var AmbienteId = $("#AmbienteId").val();
        var TipoProductoId = $("#TipoProductoId").val();
        debugger;
        if (confirm("�Desea cancelar el registro de este insumo?")) {
            window.location.href = "/Productos/Inventario?TipoProductoId=" + TipoProductoId + "&AmbienteId=" + AmbienteId;
        }
    };
    self.cancelarEdicionProducto = function () {
        if (confirm("¿Desea cancelar la edición de este insumo?")) {
            window.history.back();
        }
    };
}

var productoVm = new InventarioProductoVM();
ko.applyBindings(productoVm);

$(document).ready(function () {
    productoVm.consultarUnidadesCompra();
    productoVm.consultarUnidadesVenta();

    productoVm.consultarCategorias();
    productoVm.consultarMarcas();
    productoVm.consultarGruposT();
    productoVm.consultarPresentaciones();
    productoVm.consultarViasAdministracion();
    productoVm.consultarLaboratorios();
    productoVm.consultarEquivalencias();
    productoVm.inicializarPresentacionesVisibles();

});