var VentaUnificadaVM = function () {
    let itemProducto = 1;
    let itemServicio = 1;
    let itemLaboratorio = 1;
    let self = this;
    let model = {};
    self.ventaSubtotal = ko.observable(0);
    self.ventaDescuento = ko.observable(0);
    self.ventaTotal = ko.observable(0);
    self.costoTotalPaquete = ko.observable(0);
    self.pagoMonto = ko.observable();
    self.pagoVuelto = ko.observable(0);
    self.pagoSaldo = ko.observable(0);
    self.gananciaPaquete = ko.observable(0);


    // Productos
    self.registrosInventario = ko.observableArray();
    self.codigoProductoBuscar = ko.observable();
    self.nombreProductoBuscar = ko.observable();
    self.productoSeleccionado = ko.observable();
    self.preciosProducto = ko.observableArray();
    self.precioSeleccionadoProducto = ko.observable();
    self.unidadesVentaProducto = ko.observableArray();
    self.unidadVentaSeleccionadaProducto = ko.observable();
    self.productosExistentes = ko.observableArray();
    self.productosBuscadosNombre = ko.observableArray();
    self.productosPaquete = ko.observableArray();

    //Servicios
    self.registrosServiciosBd = ko.observableArray();
    self.codigoServicioBuscar = ko.observable();
    self.servicioSeleccionado = ko.observable();
    self.precioSeleccionadoServicio = ko.observable();
    self.serviciosExistentes = ko.observableArray();
    self.serviciosPaquete = ko.observableArray();
    self.preciosServicio = ko.observableArray();

    //Laboratorio
    self.laboratorioProductoExistente = ko.observableArray();
    self.laboratorioPaquete = ko.observableArray();
    self.laboratorioSeleccionado = ko.observable();
    self.nombreLaboratorioBuscar = ko.observable();
    self.preciosLaboratorioExistentes = ko.observableArray();
    self.precioSeleccionadoLaboratorio = ko.observable();


    self.currentPageElementos = ko.observable(1);
    self.pageSizeElementos = ko.observable(5);
    self.filtroBusqueda = ko.observable("");

    // Lista combinada de elementos
    self.filteredElementos = ko.computed(function () {
        let filtro = self.filtroBusqueda().toLowerCase();
        let elementos = [];

        self.productosPaquete().forEach(function (p) {
            if (!p.Eliminado() && (p.ProductoNombre.toLowerCase().includes(filtro) || p.ProductoCodigo.includes(filtro))) {
                p.Tipo = "Producto";
                elementos.push(p);
            }
        });

        self.serviciosPaquete().forEach(function (s) {
            if (!s.Eliminado() && (s.ServicioNombre.toLowerCase().includes(filtro) || s.ServicioCodigo.includes(filtro))) {
                s.Tipo = "Servicio";
                elementos.push(s);
            }
        });

        self.laboratorioPaquete().forEach(function (l) {
            if (!l.Eliminado() && (l.NombreExamen.toLowerCase().includes(filtro))) {
                l.Tipo = "Laboratorio";
                elementos.push(l);
            }
        });

        // Mantener el orden de agregado por timestamp
        return elementos.sort((a, b) => b.timestamp - a.timestamp);
    });



    // Paginación de elementos
    self.paginatedElementos = ko.computed(function () {
        let startIndex = (self.currentPageElementos() - 1) * self.pageSizeElementos();
        return self.filteredElementos().slice(startIndex, startIndex + self.pageSizeElementos());
    });

    self.totalPagesElementos = ko.computed(function () {
        return Math.ceil(self.filteredElementos().length / self.pageSizeElementos());
    });


    // Siguiente página
    self.nextPageElementos = function () {
        if (self.currentPageElementos() < self.totalPagesElementos()) {
            self.currentPageElementos(self.currentPageElementos() + 1);
        }
    };

    // Página anterior
    self.prevPageElementos = function () {
        if (self.currentPageElementos() > 1) {
            self.currentPageElementos(self.currentPageElementos() - 1);
        }
    };

    //Funciones laboratorio

    self.consultarLaboratorioProductoExistentes = function () {
        $.ajax({
            method: "POST",
            url: '/LaboratorioClinico/ConsultarLaboratorioExistentes',
            data: model,
            success: function (dataResult) {
                hideLoading();
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.laboratorioProductoExistente(data.Resultado);
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
    self.consultarPreciosLaboratorioExistentes = function (Id) {
        showLoading();
        $.ajax({
            method: "POST",
            data: { "Id": Id },
            url: '/LaboratorioClinico/ConsultarPreciosLaboratorioExistentes',
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    let preciosLaboratorio = [];

                    $(data.Resultado).each(function (idx, vl) {
                        let precio = {
                            PrecioId: vl.Id,
                            Precio: vl.Nombre,
                            PrecioValor: vl.Precio,
                            PrecioNombreMostrar: vl.Nombre + "(Q " + vl.Precio.toFixed(2) + ")"
                        };
                        preciosLaboratorio.push(precio);
                    });

                    self.preciosLaboratorioExistentes(preciosLaboratorio);
                } else {
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

    self.laboratorioSeleccionado.subscribe(function (value) {
        self.consultarPreciosLaboratorioExistentes(value.Id);
    });
    self.agregarLaboratorio = function () {
        let laboratorioAgregado = {
            Item: ++itemLaboratorio,
            NombreExamen: self.laboratorioSeleccionado().NombreExamen,
            Id: self.laboratorioSeleccionado().Id,
            Cantidad: ko.observable(1),
            PrecioId: self.precioSeleccionadoLaboratorio().Id,
            Precio: self.precioSeleccionadoLaboratorio().Precio,
            PrecioValor: ko.observable(self.precioSeleccionadoLaboratorio() ? self.precioSeleccionadoLaboratorio().PrecioValor : 0),
            PrecioCompra: ko.observable(self.precioSeleccionadoLaboratorio() ? self.precioSeleccionadoLaboratorio().PrecioCompra : 0),
            Subtotal: ko.observable(self.precioSeleccionadoLaboratorio() ? self.precioSeleccionadoLaboratorio().PrecioValor : 0),
            DescuentoPorcentaje: ko.observable(0),
            ValorTotal: ko.observable(self.precioSeleccionadoLaboratorio() ? self.precioSeleccionadoLaboratorio().PrecioValor : 0),
            Nuevo: true,
            Eliminado: ko.observable(false),
            timestamp: Date.now() // Asignar timestamp para orden correcto
        };

        self.laboratorioPaquete.push(laboratorioAgregado);
        mensajeEmergente("Laboratorio agregado");
        self.actualizarTotales();
    };


    self.quitarLaboratorio = function (value) {
        $(self.laboratorioPaquete()).each(function (idx, lab) {
            if (value.Item == lab.Item) {
                lab.Eliminado(true);
            }
        });
        self.actualizarTotales();
    };

    self.quitarElemento = function (elemento) {
        if (elemento.ProductoId !== undefined && elemento.ProductoId !== null) {
            self.quitarProducto(elemento);
        } else if (elemento.ServicioId !== undefined && elemento.ServicioId !== null) {
            self.quitarServicio(elemento);
        } else if (elemento.Id !== undefined && elemento.Id !== null) {
            self.quitarLaboratorio(elemento);
        }
    };

    // Consultar productos existentes
    self.consultarProductosExistentes = function () {
        let textoCargando = $("#texto-cargando-productos-existentes");
        let textoError = $("#texto-error-consultar-productos-existentes");
        self.productosExistentes([]);
        textoCargando.show();
        textoError.hide();
        $.ajax({
            method: "POST",
            url: '/HospitalizacionPaquetes/ConsultarProductosExistentes',
            data: { BodegaId: $("#BodegaId").val() },
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
                textoCargando.show();
                textoError.show();
            }
        });
    };

    // Consultar unidades de venta del producto seleccionado
    self.consultarUnidadesVentaProducto = function (producto) {
        if (!self.productoSeleccionado() || producto == null || producto == undefined
            || producto.ProductoId == null || producto.ProductoId == undefined) {
            mensajeEmergenteError("No hay ningun producto valido seleccionado");
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
    };
    self.obtenerCodigoElemento = function (elemento) {
        return elemento.ProductoCodigo ? elemento.ProductoCodigo :
            elemento.ServicioCodigo ? elemento.ServicioCodigo :
                elemento.Id ? elemento.Id : "N/A";
    };

    self.obtenerNombreElemento = function (elemento) {
        return elemento.ProductoNombre ? elemento.ProductoNombre :
            elemento.ServicioNombre ? elemento.ServicioNombre :
                elemento.NombreExamen ? elemento.NombreExamen : "Sin nombre";
    };


    // Consultar precios del producto seleccionado
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
    self.nombreLaboratorioBuscar.subscribe(function (value) {
        self.buscarLaboratorioNombre();
    });

    self.buscarLaboratorioNombre = function () {
        $(self.laboratorioProductoExistente()).each(function (idx, vl) {
            if (self.nombreLaboratorioBuscar() == vl.NombreExamen) {
                self.laboratorioSeleccionado(vl);
            }
        });
    };

    self.codigoProductoBuscar.subscribe(function (value) {
        self.buscarProductoCodigo();
    });
    self.buscarProductoCodigo = function () {
        $(self.productosExistentes()).each(function (idx, vl) {
            if (self.codigoProductoBuscar() == vl.ProductoCodigo) {
                self.productoSeleccionado(vl);
            }
        });
    };
    self.buscarProductosNombre = function () {
        showLoading();
        self.productosBuscadosNombre([]);
        $.ajax({
            url: "/Venta/BuscarProductosNombre",
            method: "POST",
            data: {
                nombre: self.nombreProductoBuscar()
            },
            success: function (dataResult) {
                hideLoading();
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.productosBuscadosNombre(data.Resultado);
                } else {
                    alert(data.Mensaje);
                }
            },
            error: function (dataError) {
                hideLoading();
                alert("ERROR DE LLAMADO ASINCRONO: " + dataError.error);
                console.log(dataError);
            }
        })
    };
    // Agregar producto
    self.agregarProducto = function () {
        let productoAgregado = {
            Item: ++itemProducto,
            ProductoCodigo: self.productoSeleccionado().ProductoCodigo,
            ProductoId: self.productoSeleccionado().ProductoId,
            ProductoNombre: self.productoSeleccionado().ProductoNombre,
            UnidadMedidaVentaId: self.unidadVentaSeleccionadaProducto() ? self.unidadVentaSeleccionadaProducto().Id : null,
            UnidadMedidaVentaNombre: ko.observable(self.unidadVentaSeleccionadaProducto() ? self.unidadVentaSeleccionadaProducto().UnidadMedidaVentaNombre : "Unidad"),
            Cantidad: ko.observable(1),
            ProductoPrecioId: self.precioSeleccionadoProducto() ? self.precioSeleccionadoProducto().Id : null,
            Precio: ko.observable(self.precioSeleccionadoProducto() ? self.precioSeleccionadoProducto().Precio : "General"),
            PrecioValor: ko.observable(self.precioSeleccionadoProducto() ? self.precioSeleccionadoProducto().PrecioValor : 0),
            PrecioCompra: ko.observable(self.precioSeleccionadoProducto() ? self.precioSeleccionadoProducto().PrecioCompra : 0),
            Subtotal: ko.observable(self.precioSeleccionadoProducto() ? self.precioSeleccionadoProducto().PrecioValor : 0),
            DescuentoPorcentaje: ko.observable(0),
            ValorTotal: ko.observable(self.precioSeleccionadoProducto() ? self.precioSeleccionadoProducto().PrecioValor : 0),
            Nuevo: true,
            Eliminado: ko.observable(false),
            timestamp: Date.now() // Asignar timestamp para orden correcto
        };

        self.productosPaquete.push(productoAgregado);
        mensajeEmergente("Producto agregado");
        self.actualizarTotales();
    };


    self.agregarProductoListaBuscados = function (producto) {
        let productoAgregado = {
            Item: itemProducto,
            ProductoCodigo: producto.ProductoCodigo,
            ProductoId: producto.ProductoId,
            ProductoInventarioId: producto.ProductoInventarioId,
            ProductoNombre: producto.ProductoNombre,
            UnidadMedidaVentaId: producto.UnidadMedidaVentaId,
            UnidadMedidaVentaNombre: producto.UnidadMedidaVentaNombre,
            Cantidad: 1
        };
        self.productosPaquete.push(productoAgregado);
        itemProducto++;
        mensajeEmergente("Producto agregado");
        self.actualizarTotales();
    };
    self.quitarProducto = function (value) {
        $(self.productosPaquete()).each(function (idx, producto) {
            if (value.Item == producto.Item) {
                producto.Eliminado(true);
            }
        });
        self.actualizarTotales();
    };

    //Funciones servicios
    self.consultarServiciosExistentes = function () {
        let textoCargando = $("#texto-cargando-servicios-existentes");
        let textoError = $("#texto-error-consultar-servicios-existentes");
        self.serviciosExistentes([]);
        textoCargando.show();
        textoError.hide();
        $.ajax({
            method: "POST",
            url: '/HospitalizacionPaquetes/ConsultarServiciosExistentes',
            success: function (dataResult) {
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.registrosServiciosBd(data.Resultado);
                    let servicioIds = new Set();
                    $(self.registrosServiciosBd()).each(function (idx, vl) {
                        servicioIds.add(vl.ServicioId);
                    });
                    for (let id of servicioIds) {
                        let agregado = false;
                        $(self.registrosServiciosBd()).each(function (idx2, vl2) {
                            if (!agregado && id == vl2.ServicioId) {
                                let servicioExistente = {
                                    ServicioId: vl2.ServicioId,
                                    ServicioCodigo: vl2.ServicioCodigo,
                                    ServicioNombre: vl2.ServicioNombre,
                                    ServicioNombreMostrar: vl2.ServicioCodigo + " - " + vl2.ServicioNombre
                                };
                                self.serviciosExistentes.push(servicioExistente);
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
                textoCargando.show();
                textoError.show();
            }
        });
    };
    self.servicioSeleccionado.subscribe(function (servicio) {
        self.consultarPreciosServicio(servicio);
    });
    self.consultarPreciosServicio = function (servicio) {
        self.preciosServicio([]);

        if (servicio == null || servicio == undefined) {
            return;
        }

        let registrosServicios = new Array();
        $(self.registrosServiciosBd()).each(function (idx, registro) {
            if (registro.ServicioId == servicio.ServicioId) {
                registrosServicios.push(registro);
            }
        });

        $(registrosServicios).each(function (idx, vl) {
            let precio = {
                PrecioId: vl.PrecioId,
                PrecioNombre: vl.PrecioNombre,
                PrecioValor: vl.PrecioValor,
                PrecioNombreMostrar: vl.PrecioNombre + " (Q " + vl.PrecioValor + ")"
            };
            self.preciosServicio.push(precio);
        });
    };
    self.codigoServicioBuscar.subscribe(function (value) {
        self.buscarServicioCodigo();
    });
    self.buscarServicioCodigo = function () {
        $(self.serviciosExistentes()).each(function (idx, vl) {
            if (self.codigoServicioBuscar() == vl.ServicioCodigo) {
                self.servicioSeleccionado(vl);
            }
        });
    };
    self.agregarServicio = function () {
        let servicioAgregado = {
            Item: ++itemServicio,
            ServicioCodigo: self.servicioSeleccionado().ServicioCodigo,
            ServicioId: self.servicioSeleccionado().ServicioId,
            ServicioNombre: self.servicioSeleccionado().ServicioNombre,
            Cantidad: ko.observable(1),
            PrecioId: self.precioSeleccionadoServicio().Id,
            Precio: ko.observable(self.precioSeleccionadoServicio() ? self.precioSeleccionadoServicio().PrecioNombre : "General"),
            PrecioValor: ko.observable(self.precioSeleccionadoServicio() ? self.precioSeleccionadoServicio().PrecioValor : 0),
            PrecioCompra: ko.observable(0),
            Subtotal: ko.observable(self.precioSeleccionadoServicio() ? self.precioSeleccionadoServicio().PrecioValor : 0),
            DescuentoPorcentaje: ko.observable(0),
            ValorTotal: ko.observable(self.precioSeleccionadoServicio() ? self.precioSeleccionadoServicio().PrecioValor : 0),
            Nuevo: true,
            Eliminado: ko.observable(false),
            timestamp: Date.now() // Asignar timestamp para orden correcto
        };

        self.serviciosPaquete.push(servicioAgregado);
        mensajeEmergente("Servicio agregado");
        self.actualizarTotales();
    };


    self.quitarServicio = function (value) {
        $(self.serviciosPaquete()).each(function (idx, servicio) {
            if (value.Item == servicio.Item) {
                servicio.Eliminado(true);
            }
        });
        self.actualizarTotales();
    };

    //Funciones consultar elementos paquete
    self.consultarElementosPaquete = function () {
        showLoading();
        $.ajax({
            url: "/HospitalizacionPaquetes/ConsultarElementosPaquete",
            data: {
                paqueteId: $("#Id").val()
            },
            method: "POST",
            success: function (result) {
                hideLoading();
                let data = JSON.parse(result);
                if (data.Exitoso) {
                    let timestampBase = Date.now() - 1000000; // Un timestamp antiguo para los existentes

                    // Productos existentes
                    $(data.Productos).each(function (idx, vl) {
                        vl.Item = itemProducto++;
                        vl.Cantidad = ko.observable(vl.Cantidad);
                        vl.PrecioCompra = ko.observable(vl.PrecioCompra);
                        vl.PrecioValor = ko.observable(vl.PrecioValor);
                        vl.Subtotal = ko.observable(vl.Subtotal);
                        vl.DescuentoPorcentaje = ko.observable(vl.DescuentoPorcentaje);
                        vl.ValorTotal = ko.observable(vl.ValorTotal);
                        vl.Eliminado = ko.observable(vl.Eliminado);
                        vl.timestamp = timestampBase + idx; // Asignar timestamp basado en el índice
                        self.productosPaquete.push(vl);
                    });

                    // Laboratorios existentes
                    $(data.Laboratorios).each(function (idx, vl) {
                        vl.Item = itemLaboratorio++;
                        vl.Cantidad = ko.observable(vl.Cantidad);
                        vl.PrecioCompra = ko.observable(vl.PrecioCompra);
                        vl.PrecioValor = ko.observable(vl.PrecioValor);
                        vl.Subtotal = ko.observable(vl.Subtotal);
                        vl.DescuentoPorcentaje = ko.observable(vl.DescuentoPorcentaje);
                        vl.ValorTotal = ko.observable(vl.ValorTotal);
                        vl.Eliminado = ko.observable(vl.Eliminado);
                        vl.timestamp = timestampBase + idx; // Asignar timestamp basado en el índice
                        self.laboratorioPaquete.push(vl);
                    });

                    // Servicios existentes
                    $(data.Servicios).each(function (idx, vl) {
                        vl.Item = itemServicio++;
                        vl.Cantidad = ko.observable(vl.Cantidad);
                        vl.PrecioValor = ko.observable(vl.PrecioValor);
                        vl.Subtotal = ko.observable(vl.Subtotal);
                        vl.DescuentoPorcentaje = ko.observable(vl.DescuentoPorcentaje);
                        vl.ValorTotal = ko.observable(vl.ValorTotal);
                        vl.Eliminado = ko.observable(vl.Eliminado);
                        vl.timestamp = timestampBase + idx; // Asignar timestamp basado en el índice
                        self.serviciosPaquete.push(vl);
                    });

                    // Actualización de totales
                    self.actualizarTotales();
                } else {
                    alert("Error al consultar elementos de paquete");
                }
            },
            error: function (errorResult) {
                hideLoading();
                alert("Error al consultar elementos de paquete");
            }
        });
    };

    //Funciones pago
    self.pagoMonto.subscribe(function (value) {
        self.actualizarTotales();
    });

    self.actualizarTotales = function () {
        let subtotal = 0;
        let descuento = 0;
        let total = 0;
        let precioCostoPaquete = 0;
        let ganancia = 0;

        $(self.productosPaquete()).each(function (idx, producto) {
            if (!producto.Eliminado()) {
                let precioCostoProducto = producto.Cantidad() * producto.PrecioCompra();
                let subtotalProducto = producto.Cantidad() * producto.PrecioValor();
                producto.Subtotal(subtotalProducto);
                let descuentoValor = subtotalProducto * (producto.DescuentoPorcentaje() / 100);
                let totalProducto = subtotalProducto - descuentoValor;
                producto.ValorTotal(totalProducto);
                subtotal += subtotalProducto;
                descuento += descuentoValor;
                precioCostoPaquete += precioCostoProducto;
                total += totalProducto;
            }
        });
        $(self.serviciosPaquete()).each(function (idx, servicio) {
            if (!servicio.Eliminado()) {
                let subtotalServicio = servicio.Cantidad() * servicio.PrecioValor();
                servicio.Subtotal(subtotalServicio);
                let descuentoValor = subtotalServicio * (servicio.DescuentoPorcentaje() / 100);
                let totalServicio = subtotalServicio - descuentoValor;
                servicio.ValorTotal(totalServicio);
                subtotal += subtotalServicio;
                descuento += descuentoValor;
                total += totalServicio;
            }
        });
        $(self.laboratorioPaquete()).each(function (idx, laboratorio) {
            if (!laboratorio.Eliminado()) {
                let subtotalLaboratorio = laboratorio.Cantidad() * laboratorio.PrecioValor();
                laboratorio.Subtotal(subtotalLaboratorio);
                let descuentoValor = subtotalLaboratorio * (laboratorio.DescuentoPorcentaje() / 100);
                let totalLaboratorio = subtotalLaboratorio - descuentoValor;
                laboratorio.ValorTotal(totalLaboratorio);
                subtotal += subtotalLaboratorio;
                descuento += descuentoValor;
                total += totalLaboratorio;
            }
        });

        let precioFinalVenta = parseFloat($("#PrecioPaquete").val());
        if (isNaN(precioFinalVenta)) {
            precioFinalVenta = 0;
        }
        ganancia = precioFinalVenta - precioCostoPaquete;
        self.gananciaPaquete(ganancia);

        self.ventaSubtotal(subtotal);
        self.ventaDescuento(descuento.toFixed(2));
        self.ventaTotal(total);
        self.costoTotalPaquete(precioCostoPaquete);

        let saldo = total;
        let monto = self.pagoMonto();
        if (isNaN(monto)) {
            //Si el monto es un valor no numerico
            monto = 0;
        }
        saldo -= monto;
        if (saldo < 0) {
            self.pagoSaldo(0);
            self.pagoVuelto((saldo * -1).toFixed(2));
        } else {
            self.pagoSaldo(saldo);
            self.pagoVuelto(0);
        }
    };

    self.validateModel = function () {
        let codigo = $("#CodigoInterno").val();
        if (codigo == null || codigo == undefined || codigo.trim() == '') {
            mensajeEmergenteError("Proporcione el codigo del paquete");
            return false;
        }

        let nombre = $("#Nombre").val();
        if (nombre == null || nombre == undefined || nombre.trim() == '') {
            mensajeEmergenteError("Proporcione el nombre del paquete");
            return false;
        }

        return true;
    };

    self.getModel = function () {
        model = {
            Id: $("#Id").val(),
            CodigoInterno: $("#CodigoInterno").val(),
            Nombre: $("#Nombre").val(),
            Descripcion: $("#Descripcion").val(),
            PrecioPaquete: $("#PrecioPaquete").val(),
            PrecioCosto: self.costoTotalPaquete(),

            Productos: self.productosPaquete().map(function (p) {
                return {
                    DetallePaqueteId: p.DetallePaqueteId,
                    ProductoId: p.ProductoId,
                    Cantidad: ko.unwrap(p.Cantidad),
                    UnidadMedidaVentaId: p.UnidadMedidaVentaId,
                    ProductoPrecioId: p.ProductoPrecioId,
                    PrecioValor: ko.unwrap(p.PrecioValor),
                    PrecioCompra: ko.unwrap(p.PrecioCompra),
                    DescuentoPorcentaje: ko.unwrap(p.DescuentoPorcentaje),
                    Nuevo: p.Nuevo,
                    Eliminado: ko.unwrap(p.Eliminado)   
                };
            }),

            Servicios: self.serviciosPaquete().map(function (s) {
                return {
                    DetallePaqueteId: s.DetallePaqueteId,
                    ServicioId: s.ServicioId,
                    Cantidad: ko.unwrap(s.Cantidad),
                    PrecioId: s.PrecioId,
                    PrecioValor: ko.unwrap(s.PrecioValor),
                    DescuentoPorcentaje: ko.unwrap(s.DescuentoPorcentaje),
                    Nuevo: s.Nuevo,
                    Eliminado: ko.unwrap(s.Eliminado)  
                };
            }),

            Laboratorios: self.laboratorioPaquete().map(function (l) {
                return {
                    DetallePaqueteId: l.DetallePaqueteId,
                    Id: l.Id,
                    Cantidad: ko.unwrap(l.Cantidad),
                    PrecioId: l.PrecioId,
                    PrecioValor: ko.unwrap(l.PrecioValor),
                    DescuentoPorcentaje: ko.unwrap(l.DescuentoPorcentaje),
                    Nuevo: l.Nuevo,
                    Eliminado: ko.unwrap(l.Eliminado)  
                };
            })
        };
    };
    self.registrarPaquete = function () {
        if (!self.validateModel()) {
            return;
        }

        if (confirm("�Desea registrar este paquete?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/HospitalizacionPaquetes/Nuevo',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/HospitalizacionPaquetes/Lista";
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
        }
    };
    self.editarPaquete = function () {
        if (confirm("�Desea editar este paquete?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/HospitalizacionPaquetes/Modificar',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/HospitalizacionPaquetes/Detalles?paqueteId=" + $("#Id").val();
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
        }
    };

    self.cancelarRegistroPaquete = function () {
        if (confirm("�Desea cancelar el registro de este paquete?")) {
            window.location.href = "/HospitalizacionPaquetes/Lista";
        }
    }
    self.cancelarEdicionPaquete = function () {
        if (confirm("�Desea cancelar la edicion de este paquete?")) {
            window.location.href = "/HospitalizacionPaquetes/Lista";
        }
    }
}

var ventaVm = new VentaUnificadaVM();
ko.applyBindings(ventaVm);

$(function () {

    ventaVm.actualizarTotales();

    let paqueteId = $("#Id").val();
    if (paqueteId != null && paqueteId != undefined && paqueteId.trim() != '') {
        ventaVm.consultarElementosPaquete();
    }

    ventaVm.consultarProductosExistentes();
    ventaVm.consultarServiciosExistentes();
    ventaVm.consultarLaboratorioProductoExistentes();

    $("#tabs-venta").tabs();
    $(".seccion-body").css("display", 'none');
    $(".seccion-header").on('click', function () {
        var nombreSeccion = this.id.replace('-header', '');
        $("#" + nombreSeccion + "-body").slideToggle();
    });

    $('#codigo-producto-buscar').on('keypress', function (e) {
        var keycode = e.which;
        if (keycode == '13') {
            ventaVm.buscarProductoCodigo();
        }
    });

    $('#codigo-servicio-buscar').on('keypress', function (e) {
        var keycode = e.which;
        if (keycode == '13') {
            ventaVm.buscarServicioCodigo();
        }
    });

    $('#nombre-producto-buscar').on('keypress', function (e) {
        var keycode = e.which;
        if (keycode == '13') {
            ventaVm.buscarProductosNombre();
        }
    });
    $('#nombre-laboratorio-buscar').on('keypress', function (e) {
        var keycode = e.which;
        if (keycode == '13') {
            ventaVm.buscarLaboratorioNombre();
        }
    });

    $("#PrecioPaquete").on('change', function () {
        ventaVm.actualizarTotales();
    });
    $("#PrecioPaquete").on('keyup', function () {
        ventaVm.actualizarTotales();
    });
});