var VentaUnificadaVM = function () {
  let itemProducto = 1;
  let itemServicio = 1;
  let itemExamen = 1;
  let itemMultipago = 1;
  let self = this;
  let model = {};
  self.txtModalConfirmacion = ko.observable(); //modal confirmacion venta

  self.ventaSubtotal = ko.observable(0);
  self.ventaDescuento = ko.observable(0);
  self.ventaTotal = ko.observable(0);
  self.ventaCubiertoSeguro = ko.observable(0);
  self.pagoMonto = ko.observable();
  self.pagoVuelto = ko.observable(0);
  self.pagoSaldo = ko.observable(0);
  self.agregarMonto = ko.observable(0);

  // --- Honorarios ---
  self.medicosExistentes = ko.observableArray(); 
  self.medicoSeleccionado = ko.observable(); 
  self.precioHonorarioLibre = ko.observable();
  self.honorariosVenta = ko.observableArray();
  self.esHonorario = ko.observable(false);

  //Productos
  self.registrosInventario = ko.observableArray();
  self.codigoProductoBuscar = ko.observable();
  self.NombreCodigoProductoBuscar = ko.observable();
  self.nombreProductoBuscar = ko.observable();
  self.productoSeleccionado = ko.observable();
  self.precioSeleccionadoProducto = ko.observable();
  self.productosExistentes = ko.observableArray();
  self.productosBuscadosNombre = ko.observableArray();
  self.productosVenta = ko.observableArray();
  self.preciosProducto = ko.observableArray();
  self.unidadesVentaProducto = ko.observableArray();
  self.unidadVentaSeleccionadaProducto = ko.observable();

  //activo  y concentradas
  self.activoContentradoBuscar = ko.observable();
  self.activosConcentradosProductos = ko.observableArray();
  self.activoConcentradoProducto = ko.observableArray();

  //Servicios
  self.codigoServicioBuscar = ko.observable();
  self.servicioSeleccionado = ko.observable();
  self.precioSeleccionadoServicio = ko.observable();
  self.serviciosExistentes = ko.observableArray();
  self.serviciosVenta = ko.observableArray();
  self.preciosServicio = ko.observableArray();

  //Examenes
  self.codigoExamenBuscar = ko.observable();
  self.examenSeleccionado = ko.observable();
  self.precioSeleccionadoExamen = ko.observable();
  self.examenesExistentes = ko.observableArray();
  self.examenesVenta = ko.observableArray();
  self.preciosExamen = ko.observableArray();
  // Variable observable para almacenar la pregunta ingresada
  self.pregunta = ko.observable();
  self.examenesPregunta = ko.observableArray(); //Almacenar las preguntas en los ecamenes

  //Multipagos
  self.multipagos = ko.observableArray();

  const bodegaFarmaciaId = 1;
// 4. Función que se ejecuta al presionar el botón "Agregar Honorarios" (Faltaba)
  self.agregarHonorario = function () {
    // Validamos que se haya seleccionado un médico y se haya ingresado un precio
    if (!self.medicoSeleccionado() || !self.precioHonorarioLibre()) {
        alert("Por favor seleccione un médico e ingrese un precio.");
        return;
    }

    // Creamos el objeto con los datos del honorario
    let honorarioAgregado = {
        MedicoNombre: self.medicoSeleccionado().medicoNombreMostrar, // Asegúrate de que esta propiedad exista en tus datos
        MedicoId: self.medicoSeleccionado().Id, // Ajusta "Id" según como venga tu base de datos
        Precio: parseFloat(self.precioHonorarioLibre()),
        Subtotal: ko.observable(parseFloat(self.precioHonorarioLibre())),
        ValorTotal: ko.observable(parseFloat(self.precioHonorarioLibre()))
    };

    // Lo agregamos a la lista de venta
    self.honorariosVenta.push(honorarioAgregado);
    
    // Limpiamos el input del precio para el siguiente ingreso
    self.precioHonorarioLibre("");
    
    // Mostramos un mensaje de éxito
    mensajeEmergente("Honorario agregado");
    
    // Actualizamos los totales (tendremos que modificar actualizarTotales más adelante)
    self.actualizarTotales();
  };
  //Funciones Productos
  // Modificación para excluir productos con stock 0 de la lista
  self.consultarProductosExistentes = function () {
    let textoCargando = $("#texto-cargando-productos-existentes");
    let textoError = $("#texto-error-consultar-productos-existentes");

    var IsEmergencia = $("#IsEmergencia").val()
    var ambiente = 1

    switch (IsEmergencia.toUpperCase()) {
      case "TRUE":
        ambiente = 3
        break;

      default:
        ambiente = 1
        break;
    }

    self.productosExistentes([]);
    textoCargando.show();
    textoError.hide();
    console.log("Enviando petición prueba");
    $.ajax({
      method: "POST",
      url: "/Venta/ConsultarProductosExistentes",
      data: {
        ambienteId: ambiente,
      },
      success: function (dataResult) {
        let data = JSON.parse(dataResult);

        if (data.Exitoso) {
          self.registrosInventario(data.Resultado);
          let productoIds = new Set();
          let stockPorProducto = {};

          // Iterar sobre cada producto en el inventario
          $(self.registrosInventario()).each(function (idx, vl) {
            let stockReal = parseFloat(vl.Stock);

            // Solo considerar productos con stock mayor a 0
            if (stockReal > 0) {
              productoIds.add(vl.ProductoId);
              if (!stockPorProducto[vl.ProductoId]) {
                stockPorProducto[vl.ProductoId] = 0;
              }
              stockPorProducto[vl.ProductoId] = stockReal;
            }
          });

          // Mostrar el stock total correcto para cada producto
          for (let id of productoIds) {
            let agregado = false;
            $(self.registrosInventario()).each(function (idx2, vl2) {
              if (!agregado && id == vl2.ProductoId) {
                let stockTotal = stockPorProducto[vl2.ProductoId];

                let productoExistente = {
                  ProductoId: vl2.ProductoId,
                  ProductoInventarioId: vl2.ProductoInventarioId,
                  ProductoCodigo: vl2.ProductoCodigo,
                  ProductoNombre: vl2.ProductoNombre,
                  ProductoNombreMostrar:
                    vl2.ProductoNombre +
                    " - Activo y concentracion: " +
                    vl2.ProductoActivoConcentracion +
                    " - Stock Total: " +
                    stockTotal.toFixed(2),
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
      },
    });
  };

  self.consultarUnidadesVentaProducto = function (producto) {
    if (
      !self.productoSeleccionado() ||
      producto == null ||
      producto == undefined ||
      producto.ProductoId == null ||
      producto.ProductoId == undefined
    ) {
      return false;
    }

    let productoId = producto.ProductoId;
    self.unidadesVentaProducto([]);
    let registrosInventarioProducto = self
      .registrosInventario()
      .filter((registro) => registro.ProductoId == productoId);

    let stockPorUnidad = {};
    registrosInventarioProducto.forEach((registro) => {
      if (
        registro.UnidadMedidaVentaId != null &&
        registro.UnidadMedidaVentaId != undefined
      ) {
        if (!stockPorUnidad[registro.UnidadMedidaVentaId]) {
          stockPorUnidad[registro.UnidadMedidaVentaId] = {
            Id: registro.UnidadMedidaVentaId,
            UnidadMedidaVentaNombre: registro.UnidadMedidaVentaNombre,
            Stock: 0,
          };
        }
        // Aquí tomamos el stock exacto del inventario sin modificarlo
        //stockPorUnidad[registro.UnidadMedidaVentaId].Stock += parseFloat(registro.Stock);
        stockPorUnidad[registro.UnidadMedidaVentaId].Stock = parseFloat(
          registro.Stock
        );
      }
    });

    self.unidadesVentaProducto(
      Object.values(stockPorUnidad).map((unidad) => ({
        Id: unidad.Id,
        UnidadMedidaVentaNombre: `${unidad.UnidadMedidaVentaNombre
          } - Stock: ${unidad.Stock.toFixed(2)}`,
      }))
    );
  };

  // Consultar precios del producto seleccionado
  self.consultarPreciosProducto = function (unidadSeleccionada) {
    if (unidadSeleccionada == null || unidadSeleccionada == undefined) {
      self.preciosProducto([]);
      return;
    }

    if (
      !self.productoSeleccionado() ||
      self.productoSeleccionado() == null ||
      self.productoSeleccionado() == undefined
    ) {
      mensajeEmergenteError("No hay ningún producto válido seleccionado");
      return;
    }

    self.preciosProducto([]);
    let registrosInventarioProducto = self
      .registrosInventario()
      .filter(
        (registro) =>
          registro.ProductoId == self.productoSeleccionado().ProductoId &&
          registro.UnidadMedidaVentaId == unidadSeleccionada.Id
      );

    let preciosIds = new Set();
    registrosInventarioProducto.forEach((registro) => {
      if (registro.PrecioId != null) {
        preciosIds.add(registro.PrecioId);
      }
    });

    let precios = [];
    preciosIds.forEach((precioId) => {
      registrosInventarioProducto.forEach((vl) => {
        if (vl.PrecioId == precioId) {
          precios.push({
            ProductoInventarioId: vl.ProductoInventarioId,
            Id: precioId,
            Precio: vl.PrecioNombre + " (Q " + vl.PrecioValor + ")",
            PrecioValor: vl.PrecioValor,
            PrecioCompra: vl.PrecioCompra,
            EsPrecioNormal: vl.PrecioNombre.trim().toLowerCase() === "normal", // Marca si es el precio "Normal"
          });
        }
      });
    });

    // Agregar los precios a la lista observable
    self.preciosProducto(precios);

    // Selecciona automáticamente el precio "Normal" si está disponible
    let precioNormal = precios.find((precio) => precio.EsPrecioNormal);
    if (precioNormal) {
      self.precioSeleccionadoProducto(precioNormal);
    } else if (precios.length > 0) {
      // Selecciona el primer precio como opción de respaldo
      self.precioSeleccionadoProducto(precios[0]);
    }
  };

  self.productoSeleccionado.subscribe(function (value) {
    self.consultarUnidadesVentaProducto(value);
  });
  self.unidadVentaSeleccionadaProducto.subscribe(function (unidadSeleccionada) {
    self.consultarPreciosProducto(unidadSeleccionada);
  });
  self.codigoProductoBuscar.subscribe(function (value) {
    self.buscarProductoCodigo();
    self.consultarPreciosProducto();
  });
  self.buscarProductoCodigo = function () {
    $(self.productosExistentes()).each(function (idx, vl) {
      if (self.codigoProductoBuscar() == vl.ProductoCodigo) {
        self.productoSeleccionado(vl);
      }
    });
  };

  self.NombreCodigoProductoBuscar.subscribe(function (value) {
    self.buscarProductoNombreCodigo();
    self.consultarPreciosProducto();
  });

  self.buscarProductoNombreCodigo = function () {
    $(self.productosExistentes()).each(function (idx, vl) {
      if (
        self.NombreCodigoProductoBuscar() == vl.ProductoCodigo ||
        self.NombreCodigoProductoBuscar() == vl.ProductoNombre
      ) {
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
        nombre: self.nombreProductoBuscar(),
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
      },
    });
  };

  self.agregarProducto = function () {
    let productoAgregado = {
      //TipoProductoId: 1,
      ProductoCodigo: self.productoSeleccionado().ProductoCodigo,
      ProductoId: self.productoSeleccionado().ProductoId,
      ProductoInventarioId: self.productoSeleccionado().ProductoInventarioId,
      ProductoNombre: self.productoSeleccionado().ProductoNombre,
      UnidadMedidaVentaId: self.unidadVentaSeleccionadaProducto() != null
        ? self.unidadVentaSeleccionadaProducto().Id
        : null,
      UnidadMedidaVentaNombre: ko.observable(
        self.unidadVentaSeleccionadaProducto() != null
          ? self.unidadVentaSeleccionadaProducto().UnidadMedidaVentaNombre
          : "Unidad"
      ),
      Cantidad: ko.observable(1),
      ProductoPrecioId: self.precioSeleccionadoProducto() != null
        ? self.precioSeleccionadoProducto().Id
        : null,
      Precio: ko.observable(
        self.precioSeleccionadoProducto() != null
          ? self.precioSeleccionadoProducto().Precio
          : "General"
      ),
      PrecioValor: ko.observable(
        self.precioSeleccionadoProducto() != null
          ? self.precioSeleccionadoProducto().PrecioValor
          : 0
      ),
      PrecioCompra: ko.observable(
        self.precioSeleccionadoProducto() != null
          ? self.precioSeleccionadoProducto().PrecioCompra
          : 0
      ),
      Subtotal: ko.observable(
        self.precioSeleccionadoProducto() != null
          ? self.precioSeleccionadoProducto().PrecioValor
          : 0
      ),
      DescuentoPorcentaje: ko.observable(0),
      DescuentoValor: ko.observable(0),
      ValorTotal: ko.observable(
        self.precioSeleccionadoProducto() != null
          ? self.precioSeleccionadoProducto().PrecioValor
          : 0
      ),
      ValorCubiertoSeguro: ko.observable(0),
      ValorCopago: ko.observable(
        self.precioSeleccionadoProducto() != null
          ? self.precioSeleccionadoProducto().PrecioValor
          : 0
      ),
      Nuevo: true,
      VentaPerdida: false,
      UsuarioAutoriza: ko.observable(""),
      Eliminado: ko.observable(false),
      Recargo: ko.observable(0),
      DescuentoProductoPorcentaje: ko.observable(0),
      DescuentoProductoValor: ko.observable(0),
      esDescuento: ko.observable(true),
    };

    productoAgregado.esDescuento.subscribe(function (v) {
      if (v) productoAgregado.Recargo(0); else productoAgregado.DescuentoProductoPorcentaje(0);
      self.actualizarTotales();
    });
    self.productosVenta.push(productoAgregado);
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
      PrecioId: producto.PrecioId,
      PrecioNombre: producto.PrecioNombre,
      ValorUnitario: producto.PrecioValor,
      ValorSubtotal: ko.observable(producto.PrecioValor),
      ValorTotal: ko.observable(producto.PrecioValor),
      Cantidad: ko.observable(1),
      DescuentoPorcentaje: ko.observable(0),
      DescuentoValor: ko.observable(0),
      Recargo: ko.observable(0),
      DescuentoProductoPorcentaje: ko.observable(0),
      DescuentoProductoValor: ko.observable(0),
      UsuarioAutoriza: ko.observable(""),
      esDescuento: ko.observable(true),
    };

    productoAgregado.esDescuento.subscribe(function (v) {
      if (v) productoAgregado.Recargo(0); else productoAgregado.DescuentoProductoPorcentaje(0);
      self.actualizarTotales();
    });
    self.productosVenta.push(productoAgregado);
    itemProducto++;
    mensajeEmergente("Producto agregado");
    self.actualizarTotales();
  };

  self.quitarProducto = function (producto) {
    // Encuentra el índice exacto del producto seleccionado
    let indexToRemove = -1;
    $(self.productosVenta()).each(function (idx, item) {
      if (
        item.ProductoId === producto.ProductoId &&
        item.ProductoCodigo === producto.ProductoCodigo &&
        item.UnidadMedidaVentaId === producto.UnidadMedidaVentaId &&
        item.ProductoPrecioId === producto.ProductoPrecioId &&
        item.Cantidad() === producto.Cantidad()
      ) {
        indexToRemove = idx;
        return false; // Rompe el bucle una vez encontrado
      }
    });

    // Si el producto fue encontrado, se elimina del array
    if (indexToRemove !== -1) {
      self.productosVenta.splice(indexToRemove, 1);
      self.actualizarTotales(); // Actualiza los totales después de eliminar
    }
  };

  //Funciones servicios
  self.consultarServiciosExistentes = function () {
    $.ajax({
      method: "POST",
      url: "/Venta/ConsultarServiciosExistentes",
      data: model,
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.serviciosExistentes(data.Resultado);
        } else {
          hideLoading();
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };
  self.consultarPreciosServicio = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Venta/ConsultarPreciosServicio",
      data: {
        servicioId: self.servicioSeleccionado().ServicioId,
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.preciosServicio(data.Resultado);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
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
  // self.agregarServicio = function () {
  //   let servicioAgregado = {
  //     Item: itemServicio,
  //     ServicioCodigo: self.servicioSeleccionado().ServicioCodigo,
  //     ServicioId: self.servicioSeleccionado().ServicioId,
  //     ServicioNombre: self.servicioSeleccionado().ServicioNombre,
  //     PrecioId: self.precioSeleccionadoServicio().PrecioId,
  //     PrecioNombre: self.precioSeleccionadoServicio().PrecioNombre,
  //     ValorUnitario: self.precioSeleccionadoServicio().PrecioValor,
  //     ValorSubtotal: ko.observable(),
  //     ValorTotal: ko.observable(),
  //     ValorCubiertoSeguro: ko.observable(0),
  //     ValorCopago: ko.observable(self.precioSeleccionadoServicio().PrecioValor),
  //     Cantidad: 1,
  //     DescuentoPorcentaje: 0,
  //     DescuentoValor: ko.observable(0),
  //     Recargo: ko.observable(0),
  //     DescuentoServicioPorcentaje: ko.observable(0),
  //     esDescuento: ko.observable(true),
  //     UsuarioAutoriza: null,
  //   };

  //   servicioAgregado.esDescuento.subscribe(function (valor) {
  //     if (valor) {
  //       servicioAgregado.Recargo(0);
  //     } else {
  //       servicioAgregado.DescuentoServicioPorcentaje(0);
  //     }
  //     self.actualizarTotales();
  //   });
  //   self.serviciosVenta.push(servicioAgregado);
  //   itemServicio++;
  //   mensajeEmergente("Servicio agregado");
  //   self.actualizarTotales();
  // };
  // self.quitarServicio = function (value) {
  //   $(self.serviciosVenta()).each(function (idx, servicio) {
  //     if (value.Item == servicio.Item) {
  //       self.serviciosVenta.splice(idx, 1);
  //       self.actualizarTotales();
  //     }
  //   });
  // };
self.agregarServicio = function () {
    // 1. Validar que se haya seleccionado un servicio
    if (!self.servicioSeleccionado()) {
        alert("Por favor seleccione un servicio.");
        return;
    }

    let precioIdTemp = null;
    let precioNombreTemp = "";
    let precioValorTemp = 0;

    // 2. Extraemos los valores dependiendo de si es honorario o no
    if (self.esHonorario()) {
        // Es campo libre (Input) - Usamos self.precioHonorarioLibre()
        let montoLibre = parseFloat(self.precioHonorarioLibre());
        if (isNaN(montoLibre) || montoLibre <= 0) {
            alert("Por favor ingrese un monto válido para el honorario.");
            return;
        }

        precioIdTemp = null; 
        precioNombreTemp = "Honorario"; 
        precioValorTemp = montoLibre;
    } else {
        // Es listado (Select) - Usamos self.precioSeleccionadoServicio()
        let objPrecio = self.precioSeleccionadoServicio();
        if (!objPrecio) {
            alert("Por favor seleccione un precio de la lista.");
            return;
        }

        precioIdTemp = objPrecio.PrecioId;
        precioNombreTemp = objPrecio.PrecioNombre;
        precioValorTemp = objPrecio.PrecioValor;
    }

    // 3. Armamos el objeto usando las variables temporales para los precios
    let servicioAgregado = {
        Item: itemServicio,
        ServicioCodigo: self.servicioSeleccionado().ServicioCodigo,
        ServicioId: self.servicioSeleccionado().ServicioId,
        ServicioNombre: self.servicioSeleccionado().ServicioNombre,
        PrecioId: precioIdTemp,
        PrecioNombre: precioNombreTemp,
        ValorUnitario: precioValorTemp,
        ValorSubtotal: ko.observable(),
        ValorTotal: ko.observable(),
        ValorCubiertoSeguro: ko.observable(0),
        ValorCopago: ko.observable(precioValorTemp), 
        Cantidad: ko.observable(1), // Lo cambiamos a observable por si editan la cantidad en la grilla
        DescuentoPorcentaje: ko.observable(0),
        DescuentoValor: ko.observable(0),
        Recargo: ko.observable(0),
        DescuentoServicioPorcentaje: ko.observable(0),
        esDescuento: ko.observable(true),
        UsuarioAutoriza: null,
    };

    servicioAgregado.esDescuento.subscribe(function (valor) {
        if (valor) {
            servicioAgregado.Recargo(0);
        } else {
            servicioAgregado.DescuentoServicioPorcentaje(0);
        }
        self.actualizarTotales();
    });

    servicioAgregado.Cantidad.subscribe(function () {
        self.actualizarTotales();
    });

    // 4. Insertamos a la lista y calculamos totales
    self.serviciosVenta.push(servicioAgregado);
    itemServicio++;
    mensajeEmergente("Servicio agregado");
    self.actualizarTotales();

    // 5. Opcional: Limpiar el input si fue honorario para el próximo registro
    if(self.esHonorario()){
        self.precioHonorarioLibre("");
    }
  };
self.quitarServicio = function (value) {
    $(self.serviciosVenta()).each(function (idx, servicio) {
        if (value.Item == servicio.Item) {
            self.serviciosVenta.splice(idx, 1);
            self.actualizarTotales();
        }
    });
};

  //Funciones examenes
  self.consultarExamenesExistentes = function () {
    $.ajax({
      method: "POST",
      url: "/Venta/ConsultarExamenesExistentes",
      data: model,
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.examenesExistentes(data.Resultado);
        } else {
          hideLoading();
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };
  self.consultarPreciosExamen = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Venta/ConsultarPreciosExamen",
      data: {
        examenLabClinicoId: self.examenSeleccionado().ExamenId,
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.preciosExamen(data.Resultado);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };
  self.codigoExamenBuscar.subscribe(function (value) {
    self.buscarExamenCodigo();
  });
  self.buscarExamenCodigo = function () {
    $(self.examenesExistentes()).each(function (idx, vl) {
      if (self.codigoExamenBuscar() == vl.ExamenCodigo) {
        self.examenSeleccionado(vl);
      }
    });
    self.consultarPreciosExamen();
  };
  self.agregarExamen = function () {
    if (
      self.examenSeleccionado() != null &&
      self.examenSeleccionado() != undefined
    ) {
      if (
        self.precioSeleccionadoExamen() != null &&
        self.precioSeleccionadoExamen() != undefined
      ) {
        let examenAgregado = {
          Item: itemExamen,
          ExamenCodigo: self.examenSeleccionado().ExamenCodigo,
          ExamenId: self.examenSeleccionado().ExamenId,
          ExamenNombre: self.examenSeleccionado().ExamenNombre,
          PrecioId: self.precioSeleccionadoExamen().PrecioId,
          PrecioNombre: self.precioSeleccionadoExamen().PrecioNombre,
          ValorUnitario: self.precioSeleccionadoExamen().PrecioValor,
          ValorSubtotal: ko.observable(),
          ValorTotal: ko.observable(),
          ValorCubiertoSeguro: ko.observable(0),
          ValorCopago: ko.observable(
            self.precioSeleccionadoExamen().PrecioValor
          ),
          Cantidad: 1,
          DescuentoPorcentaje: 0,
          DescuentoValor: ko.observable(),
          Recargo: ko.observable(0),
          DescuentoExamenPorcentaje: ko.observable(0),
          esDescuento: ko.observable(true),
          UsuarioAutoriza: null,
        };
        examenAgregado.esDescuento.subscribe(function (v) {
          if (v) examenAgregado.Recargo(0); else examenAgregado.DescuentoExamenPorcentaje(0);
          self.actualizarTotales();
        });
        self.examenesVenta.push(examenAgregado);
        itemExamen++;
        mensajeEmergente("Examen agregado");
        self.actualizarTotales();
      } else {
        alert("No hay precios agregados para este examen");
      }
    } else {
      alert("No hay ningun examen seleccionado");
    }
  };

  //Funcion para agragar preguntas al examnen
  self.agregarPregunta = function () {
    if (self.pregunta() != null && self.pregunta() != undefined) {
      let examenAgregado = {
        Pregunta: self.pregunta(),
      };
      self.examenesPregunta.push(examenAgregado);
      itemExamen++;
      mensajeEmergente("Pregunta agregada");
      self.actualizarTotales();
    } else {
      alert("No hay preguntas para este examen");
    }
  };
  self.quitarExamen = function (value) {
    $(self.examenesVenta()).each(function (idx, examen) {
      if (value.Item == examen.Item) {
        self.examenesVenta.splice(idx, 1);
        self.actualizarTotales();
      }
    });
  };

  //Funciones pago
  self.pagoMonto.subscribe(function (value) {
    self.actualizarTotales();
  });

  //Servicios de consulta
  self.consultarServiciosConsulta = function () {
    let consultaId = $("#ConsultaId").val();
    if (consultaId != null && consultaId.trim() != "") {
      //Se hace la consulta solo en caso de que venga un id de consulta en el modelo
      showLoading();
      $.ajax({
        url: "/Venta/ConsultarServiciosConsulta",
        method: "POST",
        data: {
          consultaId: consultaId,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            $(data.Resultado).each(function (idx, servicio) {
              let servicioAgregado = {
                Item: itemServicio,
                ServicioCodigo: servicio.ServicioCodigo,
                ServicioId: servicio.ServicioId,
                ServicioNombre: servicio.ServicioNombre,
                PrecioId: servicio.PrecioId,
                PrecioNombre: servicio.PrecioNombre,
                ValorUnitario: servicio.ValorUnitario,
                ValorSubtotal: ko.observable(),
                ValorTotal: ko.observable(),
                ValorCubiertoSeguro: ko.observable(
                  servicio.ValorCubiertoSeguro
                ),
                ValorCopago: ko.observable(servicio.ValorCopago),
                Cantidad: 1,
                DescuentoPorcentaje: 0,
                DescuentoValor: ko.observable(),
                Recargo: ko.observable(0),
                DescuentoServicioPorcentaje: ko.observable(0),
                esDescuento: ko.observable(true),
                UsuarioAutoriza: null,
              };

              servicioAgregado.esDescuento.subscribe(function (v) {
                if (v) servicioAgregado.Recargo(0); else servicioAgregado.DescuentoServicioPorcentaje(0);
                self.actualizarTotales();
              });
              self.serviciosVenta.push(servicioAgregado);
              itemServicio++;
              self.actualizarTotales();
            });
            if (data.Resultado.length > 0) {
              mensajeEmergente("Servicios agregados");
            }
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          alert("ERROR DE LLAMADO ASINCRONO: " + dataError.error);
          console.log(dataError);
        },
      });
    }
  };

  self.consultarServiciosEmergencias = function () {
    let emergenciaId = $("#EmergenciaId").val();
    if (emergenciaId != null && emergenciaId.trim() != "") {
      showLoading();
      $.ajax({
        url: "/Venta/ConsultarServiciosEmergencias",
        method: "POST",
        data: { emergenciaId: emergenciaId },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            $(data.Resultado).each(function (idx, servicio) {
              let unitario = servicio.ValorUnitario ?? servicio.valorUnitario ?? 0;
              let subtotal = servicio.ValorSubtotal ?? servicio.valorSubtotal ?? 0;
              let total = servicio.ValorTotal ?? servicio.valorTotal ?? 0;
              let cantidad = servicio.Cantidad ?? servicio.cantidad ?? 1;

              let servicioAgregado = {
                Item: itemServicio,
                ServicioCodigo: servicio.ServicioCodigo ?? servicio.servicioCodigo ?? "",
                ServicioId: servicio.ServicioId ?? servicio.servicioId ?? 0,
                ServicioNombre: servicio.ServicioNombre ?? servicio.servicioNombre ?? "",
                PrecioId: servicio.PrecioId ?? servicio.precioId ?? 0,
                PrecioNombre: servicio.PrecioNombre ?? servicio.precioNombre ?? "",

                ValorUnitario: unitario,
                ValorSubtotal: ko.observable(parseFloat(subtotal)),
                ValorTotal: ko.observable(parseFloat(total)),
                Cantidad: ko.observable(parseInt(cantidad)),

                ValorCubiertoSeguro: ko.observable(parseFloat(servicio.ValorCubiertoSeguro ?? servicio.valorCubiertoSeguro ?? 0)),
                ValorCopago: ko.observable(parseFloat(servicio.ValorCopago ?? servicio.valorCopago ?? 0)),

                DescuentoPorcentaje: 0,
                DescuentoValor: ko.observable(0),
                Recargo: ko.observable(0),
                DescuentoServicioPorcentaje: ko.observable(0),
                esDescuento: ko.observable(true),
                UsuarioAutoriza: null,
              };

              servicioAgregado.Cantidad.subscribe(function () {
                self.actualizarTotales();
              });

              servicioAgregado.esDescuento.subscribe(function (v) {
                if (v) servicioAgregado.Recargo(0);
                else servicioAgregado.DescuentoServicioPorcentaje(0);
                self.actualizarTotales();
              });

              self.serviciosVenta.push(servicioAgregado);
              itemServicio++;
            });

            self.actualizarTotales();
            if (data.Resultado.length > 0) {
              mensajeEmergente("Servicios de emergencia agregados");
            }
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          alert("ERROR DE LLAMADO ASINCRONO");
          console.log(dataError);
        },
      });
    }
  };


  self.consultarProductosPrescripcionConsulta = function () {
    let consultaId = $("#ConsultaId").val();
    if (consultaId != null && consultaId.trim() != "") {
      //Se hace la consulta solo en caso de que venga un id de consulta en el modelo
      showLoading();
      $.ajax({
        url: "/Venta/ConsultarProductosPrescripcionConsulta",
        method: "POST",
        data: {
          consultaId: consultaId,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            $(data.Resultado).each(function (idx, producto) {
              producto.Item = itemProducto;
              producto.Cantidad = ko.observable(producto.Cantidad);
              producto.ValorTotal = ko.observable(0);
              producto.DescuentoPorcentaje = ko.observable(
                producto.DescuentoPorcentaje
              );
              producto.DescuentoValor = ko.observable(producto.DescuentoValor);
              producto.Subtotal = ko.observable(producto.Subtotal);
              producto.PrecioValor = ko.observable(producto.PrecioValor);
              producto.ValorCubiertoSeguro = ko.observable(
                producto.ValorCubiertoSeguro
              );
              producto.ValorCopago = ko.observable(producto.ValorCopago);

              producto.Recargo = ko.observable(0);
              producto.DescuentoProductoPorcentaje = ko.observable(0);
              producto.DescuentoProductoValor = ko.observable(0);
              producto.esDescuento = ko.observable(true);

              producto.esDescuento.subscribe(function (v) {
                if (v) producto.Recargo(0); else producto.DescuentoProductoPorcentaje(0);
                self.actualizarTotales();
              });
              self.productosVenta.push(producto);
              itemProducto++;
              self.actualizarTotales();
            });
            if (data.Resultado.length > 0) {
              mensajeEmergente("Productos agregados");
            }
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          alert("ERROR DE LLAMADO ASINCRONO: " + dataError.error);
          console.log(dataError);
        },
      });
    }
  };



  self.consultarProductosPrescripcionEmergencia = function () {
    let emergenciaId = $("#EmergenciaId").val();
    if (emergenciaId != null && emergenciaId.trim() != "") {
      //Se hace la consulta solo en caso de que venga un id de consulta en el modelo
      showLoading();
      $.ajax({
        url: "/Venta/ConsultarProductosPrescripcionEmergencia",
        method: "POST",
        data: {
          emergenciaId: emergenciaId,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            $(data.Resultado).each(function (idx, producto) {
              producto.Item = itemProducto;
              producto.Cantidad = ko.observable(producto.Cantidad);
              producto.ValorTotal = ko.observable(0);
              producto.DescuentoPorcentaje = ko.observable(
                producto.DescuentoPorcentaje
              );
              producto.DescuentoValor = ko.observable(producto.DescuentoValor);
              producto.Subtotal = ko.observable(producto.Subtotal);
              producto.PrecioValor = ko.observable(producto.PrecioValor);
              producto.ValorCubiertoSeguro = ko.observable(
                producto.ValorCubiertoSeguro
              );
              producto.ValorCopago = ko.observable(producto.ValorCopago);

              producto.Recargo = ko.observable(0);
              producto.DescuentoProductoPorcentaje = ko.observable(0);
              producto.DescuentoProductoValor = ko.observable(0);
              producto.esDescuento = ko.observable(true);
              //producto.TipoProductoId = ko.observable(producto.TipoProductoId || 1);

              producto.esDescuento.subscribe(function (v) {
                if (v) producto.Recargo(0); else producto.DescuentoProductoPorcentaje(0);
                self.actualizarTotales();
              });
              self.productosVenta.push(producto);
              itemProducto++;
              self.actualizarTotales();
            });
            if (data.Resultado.length > 0) {
              mensajeEmergente("Productos agregados");
            }
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          alert("ERROR DE LLAMADO ASINCRONO: " + dataError.error);
          console.log(dataError);
        },
      });
    }
  };


  // self.actualizarTotales = function () {
  //   let subtotal = 0;
  //   let descuento = 0;
  //   let cubiertoSeguro = 0;
  //   let total = 0;

  //   $(self.productosVenta()).each(function (idx, producto) {
  //     let subtotalProducto = producto.Cantidad() * producto.PrecioValor();
  //     let descuentoValor =
  //       subtotalProducto * (producto.DescuentoPorcentaje() / 100);
  //     let totalProducto = subtotalProducto - descuentoValor;
  //     producto.Subtotal(subtotalProducto);
  //     producto.ValorTotal(totalProducto);
  //     producto.DescuentoValor(descuentoValor.toFixed(2));
  //     subtotal += subtotalProducto;
  //     descuento += descuentoValor;
  //     total += totalProducto;
  //   });
  //   $(self.serviciosVenta()).each(function (idx, servicio) {
  //     let subtotalServicio = servicio.Cantidad * servicio.ValorUnitario;
  //     let descuentoValor =
  //       subtotalServicio * (servicio.DescuentoPorcentaje / 100);
  //     let totalServicio = subtotalServicio - descuentoValor;
  //     let valorCubiertoSeguro = 0;

  //     let citaTipoAtencion = $("#CitaTipoAtencion").val();
  //     if (
  //       citaTipoAtencion != undefined &&
  //       citaTipoAtencion != null &&
  //       citaTipoAtencion.trim() != "" &&
  //       citaTipoAtencion == "SEGURO"
  //     ) {
  //       valorCubiertoSeguro = servicio.ValorCubiertoSeguro();
  //       if (
  //         valorCubiertoSeguro == null ||
  //         valorCubiertoSeguro == undefined ||
  //         isNaN(valorCubiertoSeguro)
  //       ) {
  //         valorCubiertoSeguro = 0;
  //       }
  //       cubiertoSeguro += parseFloat(valorCubiertoSeguro);
  //       let valorCopago = totalServicio - parseFloat(valorCubiertoSeguro);
  //       servicio.ValorCopago(valorCopago);
  //     }

  //     servicio.ValorSubtotal(subtotalServicio);
  //     servicio.ValorTotal(totalServicio);
  //     servicio.DescuentoValor(descuentoValor.toFixed(2));
  //     subtotal += subtotalServicio;
  //     descuento += descuentoValor;
  //     total += totalServicio - valorCubiertoSeguro;
  //   });
  //   $(self.examenesVenta()).each(function (idx, examen) {
  //     let subtotalExamen = examen.Cantidad * examen.ValorUnitario;
  //     let descuentoValor = subtotalExamen * (examen.DescuentoPorcentaje / 100);
  //     let totalExamen = subtotalExamen - descuentoValor;
  //     examen.ValorSubtotal(subtotalExamen);
  //     examen.ValorTotal(totalExamen);
  //     examen.DescuentoValor(descuentoValor.toFixed(2));
  //     let valorCubiertoSeguro = 0;

  //     let citaTipoAtencion = $("#CitaTipoAtencion").val();
  //     if (
  //       citaTipoAtencion != undefined &&
  //       citaTipoAtencion != null &&
  //       citaTipoAtencion.trim() != "" &&
  //       citaTipoAtencion == "SEGURO"
  //     ) {
  //       valorCubiertoSeguro = examen.ValorCubiertoSeguro();
  //       if (
  //         valorCubiertoSeguro == null ||
  //         valorCubiertoSeguro == undefined ||
  //         isNaN(valorCubiertoSeguro)
  //       ) {
  //         valorCubiertoSeguro = 0;
  //       }
  //       cubiertoSeguro += parseFloat(valorCubiertoSeguro);
  //       let valorCopago = totalExamen - parseFloat(valorCubiertoSeguro);
  //       examen.ValorCopago(valorCopago);
  //     }

  //     subtotal += subtotalExamen;
  //     descuento += descuentoValor;
  //     total += totalExamen - valorCubiertoSeguro;
  //   });

  //   self.ventaSubtotal(subtotal);
  //   self.ventaDescuento(descuento.toFixed(2));
  //   self.ventaTotal(total);
  //   self.ventaCubiertoSeguro(cubiertoSeguro);

  //   let saldo = total;
  //   let monto = self.pagoMonto();
  //   if (isNaN(monto)) {
  //     //Si el monto es un valor no numerico
  //     monto = 0;
  //   }
  //   saldo -= monto;
  //   if (saldo < 0) {
  //     self.pagoSaldo(0);
  //     self.pagoVuelto((saldo * -1).toFixed(2));
  //   } else {
  //     self.pagoSaldo(saldo);
  //     self.pagoVuelto(0);
  //   }
  // };


  self.actualizarTotales = function () {
    let subtotalGeneral = 0;
    let descuentoAseguradoraTotal = 0;
    let descuentoAdicionalTotal = 0;
    let cubiertoSeguroTotal = 0;
    let totalGeneralVenta = 0;

    let citaTipoAtencion = $("#CitaTipoAtencion").val();

    // --- PRODUCTOS ---
    $(self.productosVenta()).each(function (idx, producto) {
      if (producto.VentaPerdida) return true;

      let cant = parseFloat(ko.unwrap(producto.Cantidad)) || 0;
      let precio = parseFloat(ko.unwrap(producto.PrecioValor)) || 0;
      let subtotalItem = cant * precio;

      let porcDescBase = parseFloat(ko.unwrap(producto.DescuentoPorcentaje)) || 0;
      let descAsegVal = subtotalItem * (porcDescBase / 100);

      let descExtraVal = 0;
      if (producto.DescuentoProductoPorcentaje) {
        let porcExtra = parseFloat(ko.unwrap(producto.DescuentoProductoPorcentaje)) || 0;
        descExtraVal = subtotalItem * (porcExtra / 100);
      }

      let recargoVal = parseFloat(ko.unwrap(producto.Recargo)) || 0;

      let totalItem = subtotalItem - descAsegVal - descExtraVal + recargoVal;

      if (ko.isObservable(producto.Subtotal)) producto.Subtotal(subtotalItem.toFixed(2));
      if (ko.isObservable(producto.ValorTotal)) producto.ValorTotal(totalItem.toFixed(2));
      if (ko.isObservable(producto.DescuentoValor)) producto.DescuentoValor(descAsegVal.toFixed(2));

      if (citaTipoAtencion == "SEGURO") {
        let valCubierto = parseFloat(ko.unwrap(producto.ValorCubiertoSeguro)) || 0;
        cubiertoSeguroTotal += valCubierto;
        if (ko.isObservable(producto.ValorCopago)) {
          producto.ValorCopago((totalItem - valCubierto).toFixed(2));
        }
      }

      subtotalGeneral += subtotalItem;
      descuentoAseguradoraTotal += descAsegVal;
      descuentoAdicionalTotal += descExtraVal;
      totalGeneralVenta += totalItem;
    });

    // --- SERVICIOS ---
    $(self.serviciosVenta()).each(function (idx, servicio) {
      let cant = parseFloat(ko.unwrap(servicio.Cantidad)) || 0;
      let precio = parseFloat(ko.unwrap(servicio.ValorUnitario)) || 0;
      let subtotalItem = cant * precio;

      let porcDescBase = parseFloat(ko.unwrap(servicio.DescuentoPorcentaje)) || 0;
      let descAsegVal = subtotalItem * (porcDescBase / 100);

      let dServP = parseFloat(ko.unwrap(servicio.DescuentoServicioPorcentaje)) || 0;
      let dServV = subtotalItem * (dServP / 100);

      let recargoVal = parseFloat(ko.unwrap(servicio.Recargo)) || 0;

      let totalItem = subtotalItem - descAsegVal - dServV + recargoVal;

      if (ko.isObservable(servicio.ValorSubtotal)) servicio.ValorSubtotal(subtotalItem.toFixed(2));
      if (ko.isObservable(servicio.ValorTotal)) servicio.ValorTotal(totalItem.toFixed(2));
      if (ko.isObservable(servicio.DescuentoValor)) servicio.DescuentoValor(descAsegVal.toFixed(2));

      if (citaTipoAtencion == "SEGURO") {
        let valCubierto = parseFloat(ko.unwrap(servicio.ValorCubiertoSeguro)) || 0;
        cubiertoSeguroTotal += valCubierto;
        if (ko.isObservable(servicio.ValorCopago)) {
          servicio.ValorCopago((totalItem - valCubierto).toFixed(2));
        }
      }

      subtotalGeneral += subtotalItem;
      descuentoAseguradoraTotal += descAsegVal;
      descuentoAdicionalTotal += dServV;
      totalGeneralVenta += totalItem;
    });

    // --- EXÁMENES ---
    $(self.examenesVenta()).each(function (idx, examen) {
      let cant = parseFloat(ko.unwrap(examen.Cantidad)) || 0;
      let precio = parseFloat(ko.unwrap(examen.ValorUnitario)) || 0;
      let subtotalItem = cant * precio;

      let porcDescBase = parseFloat(ko.unwrap(examen.DescuentoPorcentaje)) || 0;
      let descAsegVal = subtotalItem * (porcDescBase / 100);

      let dExamP = parseFloat(ko.unwrap(examen.DescuentoExamenPorcentaje)) || 0;
      let dExamV = subtotalItem * (dExamP / 100);

      let recargoVal = parseFloat(ko.unwrap(examen.Recargo)) || 0;

      let totalItem = subtotalItem - descAsegVal - dExamV + recargoVal;

      if (ko.isObservable(examen.ValorSubtotal)) examen.ValorSubtotal(subtotalItem.toFixed(2));
      if (ko.isObservable(examen.ValorTotal)) examen.ValorTotal(totalItem.toFixed(2));
      if (ko.isObservable(examen.DescuentoValor)) examen.DescuentoValor(descAsegVal.toFixed(2));

      if (citaTipoAtencion == "SEGURO") {
        let valCubierto = parseFloat(ko.unwrap(examen.ValorCubiertoSeguro)) || 0;
        cubiertoSeguroTotal += valCubierto;
        if (ko.isObservable(examen.ValorCopago)) {
          examen.ValorCopago((totalItem - valCubierto).toFixed(2));
        }
      }

      subtotalGeneral += subtotalItem;
      descuentoAseguradoraTotal += descAsegVal;
      descuentoAdicionalTotal += dExamV;
      totalGeneralVenta += totalItem;
    });

    // --- ASIGNACIÓN DE TOTALES GLOBALES ---
    self.ventaSubtotal(subtotalGeneral.toFixed(2));
    self.ventaDescuento(descuentoAseguradoraTotal.toFixed(2));
    self.ventaCubiertoSeguro(cubiertoSeguroTotal.toFixed(2));

    // El total final es lo que el cliente debe pagar (Total - Cubierto por Seguro)
    let totalAPagar = totalGeneralVenta - cubiertoSeguroTotal;
    self.ventaTotal(totalAPagar.toFixed(2));

    // --- CÁLCULO DE SALDO Y VUELTO ---
    let sumaPagos = 0;
    if (self.pagosAgregados) {
      $(self.pagosAgregados()).each(function (i, p) {
        sumaPagos += parseFloat(ko.unwrap(p.Monto)) || 0;
      });
    } else {
      sumaPagos = parseFloat(ko.unwrap(self.pagoMonto)) || 0;
    }

    let saldoCalculado = totalAPagar - sumaPagos;

    if (saldoCalculado <= 0) {
      self.pagoSaldo(0);
      self.pagoVuelto(Math.abs(saldoCalculado).toFixed(2));
    } else {
      self.pagoSaldo(saldoCalculado.toFixed(2));
      self.pagoVuelto(0);
    }
  };

  self.getModel = function () {
    model = {
      AmbienteId: $("#AmbienteId").val(),
      IsClinica: $("#IsClinica").val(),
      IsFarmacia: $("#IsFarmacia").val(),
      IsEmergencia: $("#IsEmergencia").val(),
      IsLaboratorio: $("#IsLaboratorio").val(),
      NumeroComprobante: $("#NumeroComprobante").val(),
      CodigoVendedor: $("#CodigoVendedor").val(),
      CodigoMedico: $("#CodigoMedico").val(),
      //Cliente: $("#Cliente option:selected").text(),
      Cliente: $("#select-cliente option:selected").text() || $("#select-cliente").find(":selected").text(),
      Medico: $("#Medico option:selected").text(),
      Clinica: $("#Clinica option:selected").text(),
      //Cliente: $("#").val(),
      Nit: $("#Nit").val(),
      Direccion: $("#Direccion").val(),
      PacienteFechaNacimiento: $("#PacienteFechaNacimiento").val(),
      SucursalId: $("#SucursalId").val(),
      FormaPagoId: $("#FormaPagoId").val(),
      ConsultaId: $("#ConsultaId").val(),
      EmergenciaId: $("#EmergenciaId").val(),
      EmergenciaResponsable: $("#EmergenciaResponsable").val(),
      //ClienteId: $("#ClienteId").val(),
      //ClienteId: $("#selectCliente").val(),
      ResponsableNit: $("#ResponsableNit").val(),
      ResponsableNombre: $("#ResponsableNombre").val(),
      ResponsableDireccion: $("#ResponsableDireccion").val(),
      ResponsableCorreo: $("#ResponsableCorreo").val(),

      Origen: $("#Origen").val(),

      ClienteId: $("#ClienteId").val(),
      FormaPagoId: $("#FormaPagoId").val(),
      PagoMonto: self.pagoMonto(),
      PagoVuelto: self.pagoVuelto(),

      Productos: self.productosVenta(),
      Servicios: self.serviciosVenta(),
      Examenes: self.examenesVenta(),
      Pagos: self.multipagos(),

      //Valor cubierto seguro
      ValorCubiertoSeguro: self.ventaCubiertoSeguro(),
    };
    console.log(model.Servicios);
  };

  self.validateModel = function () {
    //let numeroComprobante = $("#NumeroComprobante").val();
    //if (numeroComprobante == null
    //    || numeroComprobante == undefined
    //    || numeroComprobante.trim() == '') {
    //    alert("Proporcione el numero de comprobante");
    //    return false;
    //}
    let saldo = self.pagoSaldo();
    if (saldo > 0) {
      alert("Registre el monto del pago");
      return false;
    }

    return true;
  };

  // self.precargarClienteConsulta = function () {
  //   var clienteIdPresel = $("#ClienteId").val();

  //   $.ajax({
  //     method: "POST",
  //     url: "/Emergencias/ConsultarPacientes",
  //     success: function (dataResult) {
  //       var data = JSON.parse(dataResult);
  //       if (data.Exitoso) {
  //         var selectCliente = $("#select-cliente");
  //         selectCliente.empty();
  //         selectCliente.append(new Option("", "", false, false));

  //         $(data.Resultado).each(function (idx, paciente) {
  //           var selected = paciente.PacienteId == clienteIdPresel;
  //           var option = new Option(paciente.Nombre, paciente.PacienteId, selected, selected);
  //           selectCliente.append(option);
  //         });

  //         selectCliente.trigger("change.select2");

  //         if (clienteIdPresel && clienteIdPresel != "") {
  //           selectCliente.val(clienteIdPresel).trigger("change.select2");
  //           $.ajax({
  //             url: "/Venta/ObtenerDatosCliente",
  //             method: "POST",
  //             dataType: "json",
  //             data: { clienteId: clienteIdPresel },
  //             success: function (datos) {
  //               if (datos.exitoso) {
  //                 var d = datos.datos;
  //                 $("#Nit").val(d.nit || "");
  //                 $("#Direccion").val(d.direccion || "");
  //                 $("#Correo").val(d.correo || "");
  //                 $("#PacienteFechaNacimiento").val(
  //                   d.fechaNacimiento ? d.fechaNacimiento.split("T")[0] : ""
  //                 );
  //               }
  //             }
  //           });
  //         }
  //       }
  //     }
  //   });
  // };



  self.precargarClienteConsulta = function () {
    var clienteIdPresel = $("#ClienteId").val();
    var consultaId = $("#ConsultaId").val();
    var emergenciaId = $("#EmergenciaId").val();

    var isEmergencia = $("#IsEmergencia").val() === "true" || $("#IsEmergencia").val() === "True";
    var isClinica = $("#IsClinica").val() === "true" || $("#IsClinica").val() === "True";
    var isFarmacia = $("#IsFarmacia").val() === "true" || $("#IsFarmacia").val() === "True";
    var isLaboratorio = $("#IsLaboratorio").val() === "true" || $("#IsLaboratorio").val() === "True";

    console.log("===== DEBUG PRECARGA CLIENTE =====");
    console.log("Contexto:", {
      isEmergencia: isEmergencia,
      isClinica: isClinica,
      isFarmacia: isFarmacia,
      isLaboratorio: isLaboratorio,
      consultaId: consultaId,
      emergenciaId: emergenciaId,
      clienteIdPresel: clienteIdPresel
    });

    // Es una consulta (tiene consultaId y NO es emergencia)
    if (consultaId && consultaId.trim() !== "" && !isEmergencia) {
      console.log("Cargando paciente específico para CONSULTA ID:", consultaId);
      cargarPacientePorConsulta(consultaId, clienteIdPresel);
    }
    // Es una emergencia (tiene emergenciaId)
    else if (emergenciaId && emergenciaId.trim() !== "" && isEmergencia) {
      console.log("Cargando paciente específico para EMERGENCIA ID:", emergenciaId);
      cargarPacientePorEmergencia(emergenciaId, clienteIdPresel);
    }
    // Es clínica, farmacia, laboratorio o emergencia sin ID
    else {
      console.log("Cargando lista completa de pacientes para:",
        isClinica ? "CLÍNICA" : isFarmacia ? "FARMACIA" : isLaboratorio ? "LABORATORIO" : "EMERGENCIA (sin ID)");
      cargarListaPacientes(clienteIdPresel);
    }
  };

  function cargarPacientePorConsulta(consultaId, clienteIdPresel) {
    $.ajax({
      method: "POST",
      url: "/Consultas/ObtenerPacientePorConsulta",
      data: { consultaId: consultaId },
      success: function (dataResult) {
        console.log("Respuesta Consultas:", dataResult);

        try {
          var data = JSON.parse(dataResult);

          if (data.Exitoso && data.Resultado && data.Resultado.length > 0) {
            var paciente = data.Resultado[0];
            console.log("Paciente encontrado:", paciente);

            var selectCliente = $("#select-cliente");
            selectCliente.empty();
            selectCliente.append(new Option("", "", false, false));

            var nombrePaciente = paciente.Nombre || "Cliente " + paciente.PacienteId;
            var option = new Option(nombrePaciente, paciente.PacienteId, true, true);
            selectCliente.append(option);
            selectCliente.val(paciente.PacienteId).trigger("change.select2");

            llenarCamposCliente(paciente);
          } else {
            console.error("Error al obtener paciente:", data.Mensaje);
            cargarListaPacientes(clienteIdPresel);
          }
        } catch (e) {
          console.error("Error parseando respuesta:", e);
          cargarListaPacientes(clienteIdPresel);
        }
      },
      error: function (xhr, status, error) {
        console.error("Error AJAX en Consultas:", error);
        cargarListaPacientes(clienteIdPresel);
      }
    });
  }

  function cargarPacientePorEmergencia(emergenciaId, clienteIdPresel) {
    $.ajax({
      method: "POST",
      url: "/Emergencias/ObtenerPacientePorEmergencia", 
      data: { emergenciaId: emergenciaId },
      success: function (dataResult) {
        console.log("Respuesta Emergencia específica:", dataResult);

        try {
          var data = JSON.parse(dataResult);

          if (data.Exitoso && data.Resultado && data.Resultado.length > 0) {
            var paciente = data.Resultado[0];
            console.log("Paciente encontrado:", paciente);

            var selectCliente = $("#select-cliente");
            selectCliente.empty();
            selectCliente.append(new Option("", "", false, false));

            var nombrePaciente = paciente.Nombre || "Cliente " + paciente.PacienteId;
            var option = new Option(nombrePaciente, paciente.PacienteId, true, true);
            selectCliente.append(option);
            selectCliente.val(paciente.PacienteId).trigger("change.select2");

            llenarCamposCliente(paciente);
          } else {
            console.error("Error al obtener paciente:", data.Mensaje);
            cargarListaPacientes(clienteIdPresel);
          }
        } catch (e) {
          console.error("Error parseando respuesta:", e);
          cargarListaPacientes(clienteIdPresel);
        }
      },
      error: function (xhr, status, error) {
        console.error("Error AJAX en Emergencia específica:", error);
        cargarListaPacientes(clienteIdPresel);
      }
    });
  }

  function cargarListaPacientes(clienteIdPresel) {

    $.ajax({
      method: "POST",
      url: "/Emergencias/ConsultarPacientes", 
      success: function (dataResult) {
        console.log("Respuesta lista pacientes:", dataResult);

        try {
          var data = JSON.parse(dataResult);

          if (data.Exitoso) {
            console.log("Resultado pacientes:", data.Resultado);

            var selectCliente = $("#select-cliente");
            selectCliente.empty();
            selectCliente.append(new Option("", "", false, false));

            if (data.Resultado && data.Resultado.length > 0) {
              $(data.Resultado).each(function (idx, paciente) {
                console.log("Paciente:", paciente);

                var nombrePaciente = paciente.Nombre ||
                  paciente.NombreCompleto ||
                  paciente.PacienteNombre ||
                  paciente.nombre ||
                  "Cliente " + paciente.PacienteId;

                var pacienteId = paciente.PacienteId || paciente.id || paciente.Id;

                var selected = pacienteId == clienteIdPresel;
                var option = new Option(nombrePaciente, pacienteId, selected, selected);
                selectCliente.append(option);
              });

              selectCliente.trigger("change.select2");

              if (clienteIdPresel && clienteIdPresel != "") {
                selectCliente.val(clienteIdPresel).trigger("change.select2");

                $.ajax({
                  url: "/Venta/ObtenerDatosCliente",
                  method: "POST",
                  dataType: "json",
                  data: { clienteId: clienteIdPresel },
                  success: function (datos) {
                    console.log("Datos detallados del cliente:", datos);

                    if (datos.exitoso) {
                      llenarCamposCliente(datos.datos);
                    }
                  },
                  error: function (err) {
                    console.error("Error cargando datos detallados:", err);
                  }
                });
              }
            } else {
              console.warn("No hay pacientes en el resultado");
            }
          } else {
            console.error("Error en respuesta:", data.Mensaje);
          }
        } catch (e) {
          console.error("Error parseando respuesta:", e);
        }
      },
      error: function (xhr, status, error) {
        console.error("Error AJAX en lista pacientes:", error);
      }
    });
  }

  function llenarCamposCliente(d) {
    var fechaFormateada = "";
    if (d.fechaNacimiento) {
      try {
        var fecha = new Date(d.fechaNacimiento);
        if (!isNaN(fecha.getTime())) {
          var year = fecha.getFullYear();
          var month = String(fecha.getMonth() + 1).padStart(2, '0');
          var day = String(fecha.getDate()).padStart(2, '0');
          fechaFormateada = `${year}-${month}-${day}`;
        }
      } catch (e) {
        console.error("Error formateando fecha:", e);
      }
    }

    $("#Nit").val(d.nit || d.Nit || "");
    $("#Direccion").val(d.direccion || d.Direccion || "");
    $("#Correo").val(d.correo || d.Correo || "");
    $("#PacienteFechaNacimiento").val(fechaFormateada);
  }


  function getFormattedDateTimeForFEL() {
    const now = new Date();

    // Obtener componentes de la fecha y hora
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, "0");
    const day = String(now.getDate()).padStart(2, "0");
    const hours = String(now.getHours()).padStart(2, "0");
    const minutes = String(now.getMinutes()).padStart(2, "0");
    const seconds = String(now.getSeconds()).padStart(2, "0");

    // Zona horaria fija a -06:00
    const timezone = "-06:00";

    return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}${timezone}`;
  }

  const FechaHoraEmisionLocal = getFormattedDateTimeForFEL();

  function roundTo(value, decimals) {
    return Number(Math.round(value + "e" + decimals) + "e-" + decimals);
  }

  self.registrarVenta = function () {
    if (self.validateModel()) {
      self.txtModalConfirmacion("&iquest;Desea continuar con el Pago?");
      $("#mdl-confirmacion").dialog({
        modal: true,
        width: 800,
        buttons: [
          {
            text: "Si",
            class: "btn btn-success",
            click: function () {
              showLoading();
              self.getModel();
              realizarVentaSinXml();

              // ── LÓGICA FEL/XML (desactivada temporalmente) ──────────────────
              // var emergencia = $("#IsEmergencia").val();
              // var clinica = $("#IsClinica").val();
              // var laboratorio = $("#IsLaboratorio").val();
              //
              // if (emergencia === "true" || clinica === "True" || laboratorio === "True") {
              //   realizarVentaSinXml();
              // } else {
              //   if ($("#Nit").val() === "") {
              //     realizarVentaSinXml();
              //   } else {
              //     // Generar XML
              //     const requestXMLData = {
              //       CodigoMoneda: "GTQ",
              //       FechaHoraEmision: FechaHoraEmisionLocal,
              //       TipoDocumento: "FACT",
              //       Emisor: {
              //         AfiliacionIVA: "GEN",
              //         CodigoEstablecimiento: "2",
              //         CorreoEmisor: "recepcion@hcq.com",
              //         NITEmisor: "117286303",
              //         NombreComercial: "FARMACIA SANCTI SPIRITUS",
              //         NombreEmisor: "FARMACIA SANCTI SPIRITUS",
              //         Direccion: {
              //           DetalleDireccion: "2 AVENIDA BARRIO SAN SEBASTIAN ZONA 3 SAN CRISTÓBAL VERAPAZ, ALTA VERAPAZ",
              //           CodigoPostal: "01011",
              //           Municipio: "Guatemala",
              //           Departamento: "Guatemala",
              //           Pais: "GT",
              //         },
              //       },
              //       Receptor: {
              //         CorreoReceptor: $("#Email").val() || "na@gmail.com",
              //         IDReceptor: $("#Nit").val(),
              //         NombreReceptor: $("#ClienteId option:selected").text() || "CF",
              //         Direccion: {
              //           DetalleDireccion: $("#Direccion").val() || "N/A",
              //           CodigoPostal: "01001",
              //           Municipio: "Guatemala",
              //           Departamento: "Guatemala",
              //           Pais: "GT",
              //         },
              //       },
              //       Frases: [{ CodigoEscenario: "1", TipoFrase: "1" }],
              //       Items: model.Servicios.map((servicio, index) => ({
              //         BienOServicio: "S",
              //         Cargo: servicio.Cargo,
              //         NumeroLinea: (index + 1).toString(),
              //         Cantidad: servicio.Cantidad,
              //         UnidadMedida: "UND",
              //         Descripcion: servicio.ServicioNombre,
              //         PrecioUnitario: servicio.ValorUnitario,
              //         Precio: servicio.ValorUnitario * servicio.Cantidad,
              //         Descuento: 0.0,
              //         Impuestos: [{
              //           NombreCorto: "IVA",
              //           CodigoUnidadGravable: "1",
              //           MontoGravable: roundTo((servicio.ValorUnitario * servicio.Cantidad) / 1.12, 2),
              //           MontoImpuesto: roundTo(roundTo((servicio.ValorUnitario * servicio.Cantidad) / 1.12, 2) * 0.12, 2),
              //         }],
              //         Total: servicio.ValorUnitario * servicio.Cantidad,
              //       })),
              //       Totales: {
              //         TotalImpuestos: [{
              //           NombreCorto: "IVA",
              //           TotalMontoImpuesto: model.Servicios.reduce(
              //             (sum, servicio) => sum + roundTo(roundTo((servicio.ValorUnitario * servicio.Cantidad) / 1.12, 2) * 0.12, 2), 0
              //           ),
              //         }],
              //         GranTotal: model.Servicios.reduce(
              //           (sum, servicio) => sum + servicio.ValorUnitario * servicio.Cantidad, 0
              //         ),
              //       },
              //       Adenda: "FACTURA-19",
              //     };
              //
              //     fetch("http://18.222.15.29/Xml/GenerateXml", {
              //       method: "POST",
              //       headers: { "Content-Type": "application/json" },
              //       body: JSON.stringify(requestXMLData),
              //     })
              //       .then((response) => {
              //         if (!response.ok) throw new Error("Error al generar el XML");
              //         return response.json();
              //       })
              //       .then((data) => {
              //         console.log("XML generado:", data);
              //         return fetch("http://18.222.15.29/Xml/SendXml", {
              //           method: "POST",
              //           headers: { "Content-Type": "application/json" },
              //           body: JSON.stringify({
              //             XmlContent: data.xmlContent,
              //             UsuarioFirma: "117286303",
              //             LlaveFirma: "3f324367093d7bef36bdf933a1e95aff",
              //             UsuarioApi: "117286303",
              //             LlaveApi: "DF34FB82B6FE81A4CC20979661FFF8F4",
              //           }),
              //         });
              //       })
              //       .then((response) => {
              //         if (!response.ok) throw new Error("Error al enviar el XML");
              //         return response.json();
              //       })
              //       .then((data) => {
              //         model.UuidFel = data.uuid;
              //         const pdfUrl = `http://18.222.15.29/Xml/GeneratePDF?uuid=${data.uuid}`;
              //         const pdfWindow = window.open(pdfUrl, "_blank");
              //         if (!pdfWindow) {
              //           console.error("El navegador bloqueó la apertura de la pestaña del PDF.");
              //           alert("Permite la apertura de ventanas emergentes para mostrar el PDF.");
              //         }
              //         realizarVentaConUuid();
              //       })
              //       .catch((error) => {
              //         hideLoading();
              //         console.error("Error:", error);
              //         alert("Hubo un error al procesar la solicitud.");
              //       });
              //   }
              // }
              // ── FIN LÓGICA FEL/XML ───────────────────────────────────────────
            },
          },
          {
            text: "No",
            class: "btn btn-danger",
            click: function () {
              $(this).dialog("close");
            },
          },
        ],
      });
    }
  };

  // Función para realizar la venta sin generar XML
  function realizarVentaSinXml() {
    $.ajax({
      method: "POST",
      url: "/Venta/NuevaVentaUnificada",
      data: model,
      success: function (dataResult) {
        procesarRespuestaVenta(dataResult);
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  }

  // Función para realizar la venta con el UUID generado
  function realizarVentaConUuid() {
    $.ajax({
      method: "POST",
      url: "/Venta/NuevaVentaUnificada",
      data: model,
      success: function (dataResult) {
        procesarRespuestaVenta(dataResult);
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  }

  // Función para procesar la respuesta de la venta
  function procesarRespuestaVenta(dataResult) {
    hideLoading();
    let data = JSON.parse(dataResult);

    if (data.Exitoso) {
      if (
        $("#IsLaboratorio").val() == "true" ||
        $("#IsLaboratorio").val() == "True"
      ) {
        window.location.href = "/Venta/ListaVentasLaboratorio";
      } else if (
        $("#IsFarmacia").val() == "True" ||
        $("#IsFarmacia").val() == "true"
      ) {
        window.location.href = "/Venta/ListaVentasFarmacia";
      } else if (
        $("#IsClinica").val() == "True" ||
        $("#IsClinica").val() == "true"
      ) {
        window.location.href = "/Venta/ListaVentasClinica";
      } else if (
        $("#IsEmergencia").val() == "True" ||
        $("#IsEmergencia").val() == "true"
      ) {
        window.location.href = "/Venta/ListaVentasClinica";
      } else {
        window.location.href = "/Venta/Lista";
      }
    } else {
      hideLoading();
      alert(data.Mensaje);
    }
  }

  //Consultando los examenes  de consulta
  self.consultarExamenesAgregadosConsulta = function () {
    let consultaId = $("#ConsultaId").val();
    if (consultaId != null && consultaId.trim() != "") {
      //Se hace la consulta solo en caso de que venga un id de consulta en el modelo
      showLoading();
      $.ajax({
        url: "/Venta/consultarExamenesAgregadosConsulta",
        method: "POST",
        data: {
          consultaId: consultaId,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            $(data.Resultado).each(function (idx, value) {
              let examenAgregado = {
                Item: itemExamen,
                ExamenCodigo: value.ExamenCodigo,
                ExamenId: value.ExamenId,
                ExamenNombre: value.ExamenNombre,
                PrecioId: value.PrecioId,
                PrecioNombre: value.PrecioNombre,
                ValorUnitario: value.ValorUnitario,
                ValorSubtotal: ko.observable(0),
                ValorTotal: ko.observable(0),
                ValorCubiertoSeguro: ko.observable(value.ValorCubiertoSeguro),
                ValorCopago: ko.observable(value.ValorCopago),
                Cantidad: 1,
                DescuentoPorcentaje: 0,
                DescuentoValor: ko.observable(0),
                Recargo: ko.observable(0),
                DescuentoExamenPorcentaje: ko.observable(0),
                UsuarioAutoriza: null,
                esDescuento: ko.observable(true),
              };
              examenAgregado.esDescuento.subscribe(function (v) {
                if (v) examenAgregado.Recargo(0); else examenAgregado.DescuentoExamenPorcentaje(0);
                self.actualizarTotales();
              });
              self.examenesVenta.push(examenAgregado);
              itemExamen++;
            });
            if (data.Resultado.length > 0) {
              mensajeEmergente("Examenes agregados");
            }
            self.actualizarTotales();
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          alert("ERROR DE LLAMADO ASINCRONO: " + dataError.error);
          console.log(dataError);
        },
      });
    }
  };


  self.consultarExamenesAgregadosEmergencias = function () {
    let emergenciaId = $("#EmergenciaId").val();
    if (emergenciaId != null && emergenciaId.trim() != "") {
      showLoading();
      $.ajax({
        url: "/Venta/consultarExamenesAgregadosEmergencias",
        method: "POST",
        data: {
          emergenciaId: emergenciaId,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            $(data.Resultado).each(function (idx, value) {
              let unitario = value.ValorUnitario ?? value.valorUnitario ?? 0;
              let subtotal = value.ValorSubtotal ?? value.valorSubtotal ?? 0;
              let total = value.ValorTotal ?? value.valorTotal ?? 0;
              let cantidad = value.Cantidad ?? value.cantidad ?? 1;

              let examenAgregado = {
                Item: itemExamen,
                ExamenCodigo: value.ExamenCodigo ?? value.examenCodigo ?? "",
                ExamenId: value.ExamenId ?? value.examenId ?? 0,
                ExamenNombre: value.ExamenNombre ?? value.examenNombre ?? "",
                PrecioId: value.PrecioId ?? value.precioId ?? 0,
                PrecioNombre: value.PrecioNombre ?? value.precioNombre ?? "",

                ValorUnitario: unitario,
                ValorSubtotal: ko.observable(parseFloat(subtotal)),
                ValorTotal: ko.observable(parseFloat(total)),

                ValorCubiertoSeguro: ko.observable(parseFloat(value.ValorCubiertoSeguro ?? value.valorCubiertoSeguro ?? 0)),
                ValorCopago: ko.observable(parseFloat(value.ValorCopago ?? value.valorCopago ?? 0)),

                Cantidad: ko.observable(parseInt(cantidad)),

                DescuentoPorcentaje: 0,
                DescuentoValor: ko.observable(0),
                Recargo: ko.observable(0),
                DescuentoExamenPorcentaje: ko.observable(0),
                UsuarioAutoriza: null,
                esDescuento: ko.observable(true),
              };

              examenAgregado.Cantidad.subscribe(function () {
                self.actualizarTotales();
              });

              examenAgregado.esDescuento.subscribe(function (v) {
                if (v) examenAgregado.Recargo(0);
                else examenAgregado.DescuentoExamenPorcentaje(0);
                self.actualizarTotales();
              });

              self.examenesVenta.push(examenAgregado);
              itemExamen++;
            });

            if (data.Resultado.length > 0) {
              mensajeEmergente("Examenes agregados");
            }
            self.actualizarTotales();

          } else {
            alert(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          alert("ERROR DE LLAMADO ASINCRONO: " + dataError.error);
          console.log(dataError);
        },
      });
    }
  };

  self.agregarPagoOld = function () {
    //agrega multiples pagos a la tabla

    if (
      self.agregarMonto() != null &&
      self.agregarMonto() != undefined &&
      self.agregarMonto() != 0
    ) {
      if (self.agregarMonto() <= self.pagoSaldo()) {
        let montoAgregado = {
          Item: itemMultipago,
          PagoCodigo: $("#FormaPagoId").val(),
          PagoId: $("#FormaPagoId").val(),
          FormaPagoId: $("#FormaPagoId").val(),
          PagoNombre: $("#FormaPagoId option:selected").text(),

          ValorTotal: self.agregarMonto(),
        };
        self.multipagos.push(montoAgregado);
        itemMultipago++;
        mensajeEmergente("Pago agregado");
        self.actualizarTotalesMultiPagos();
      } else {
        alert("Monto supera el saldo");
      }
    } else {
      alert("Monto no valido");
    }
  };

  self.agregarPago = function () {
    if (
      self.agregarMonto() != null &&
      self.agregarMonto() != undefined &&
      self.agregarMonto() != 0
    ) {
      // Permitir agregar aunque el monto sea mayor que el saldo
      let montoAgregado = {
        Item: itemMultipago,
        PagoCodigo: $("#FormaPagoId").val(),
        PagoId: $("#FormaPagoId").val(),
        FormaPagoId: $("#FormaPagoId").val(),
        PagoNombre: $("#FormaPagoId option:selected").text(),
        ValorTotal: self.agregarMonto(),
      };
      self.multipagos.push(montoAgregado);
      itemMultipago++;
      mensajeEmergente("Pago agregado");
      self.actualizarTotalesMultiPagos();
    } else {
      alert("Monto no válido");
    }
  };

  self.quitarPago = function (value) {
    $(self.multipagos()).each(function (idx, pago) {
      if (value.Item == pago.Item) {
        self.multipagos.splice(idx, 1);
        self.actualizarTotalesMultiPagos();
      }
    });
  };

  self.actualizarTotalesMultiPagos = function () {
    let subtotal = 0;
    let descuento = 0;
    let total = 0;

    $(self.multipagos()).each(function (idx, pago) {
      // Aseg�rate de que pago.ValorTotal sea un n�mero
      let valorTotal = parseFloat(pago.ValorTotal);

      // Verifica si es un n�mero v�lido antes de sumarlo
      if (!isNaN(valorTotal)) {
        subtotal += valorTotal;
      }
    });
    self.pagoMonto(subtotal);
    //
  };

  //aqui terminan las funciones
};

self.fixMissingProperties = function () {
  // Reparar Productos/Insumos
  // self.productosVenta().forEach(function (p) {
  //   if (!p.TipoProductoId) p.TipoProductoId = ko.observable(1);
  //   else if (!ko.isObservable(p.TipoProductoId)) p.TipoProductoId = ko.observable(p.TipoProductoId);

  //   // Inicializar observables de seguridad
  //   if (!p.Recargo) p.Recargo = ko.observable(0);
  //   if (!p.DescuentoProductoPorcentaje) p.DescuentoProductoPorcentaje = ko.observable(0);
  //   if (!p.esDescuento) {
  //     p.esDescuento = ko.observable(true);
  //     p.esDescuento.subscribe(function (v) {
  //       if (v) p.Recargo(0); else p.DescuentoProductoPorcentaje(0);
  //       self.actualizarTotales();
  //     });
  //   }
  // });

  self.serviciosVenta().forEach(function (s) {
    if (!s.Recargo) s.Recargo = ko.observable(0);
    if (!s.DescuentoServicioPorcentaje) s.DescuentoServicioPorcentaje = ko.observable(0); // O el nombre que uses
    if (!s.esDescuento) {
      s.esDescuento = ko.observable(true);
      s.esDescuento.subscribe(function (v) {
        if (v) s.Recargo(0); else s.DescuentoServicioPorcentaje(0);
        self.actualizarTotales();
      });
    }
  });

  self.examenesVenta().forEach(function (e) {
    if (!e.Recargo) e.Recargo = ko.observable(0);
    if (!e.DescuentoExamenPorcentaje) e.DescuentoExamenPorcentaje = ko.observable(0);
    if (!e.esDescuento) {
      e.esDescuento = ko.observable(true);
      e.esDescuento.subscribe(function (v) {
        if (v) e.Recargo(0); else e.DescuentoExamenPorcentaje(0);
        self.actualizarTotales();
      });
    }
  });
};

var ventaVm = new VentaUnificadaVM();
ko.applyBindings(ventaVm);

$(function () {
  contraerMenu();

  $("#select-cliente").select2({
    tags: true,
    placeholder: "Buscar o escribir paciente...",
    allowClear: true,
    width: "100%",
    minimumResultsForSearch: 0,
    language: {
      noResults: function () {
        return "No se encontró el paciente. Escribe su nombre para registrarlo.";
      }
    },
    createTag: function (params) {
      var term = $.trim(params.term);
      if (term === "") return null;
      var existeExacto = false;
      $("#select-cliente option").each(function () {
        if ($(this).text().toLowerCase() === term.toLowerCase()) {
          existeExacto = true;
          return false;
        }
      });
      if (existeExacto) return null;
      return {
        id: "__nuevo__:" + term,
        text: "✚ Registrar \"" + term + "\" como nuevo paciente",
        newPatient: true,
        patientName: term
      };
    }
  });

  $("#select-cliente").on("select2:select", function (e) {
    var data = e.params.data;
    if (data.newPatient) {
      $("#nuevo-cliente-nombre").val(data.patientName);
      $("#nuevo-cliente-nit").val("");
      $("#nuevo-cliente-direccion").val("");
      $("#nuevo-cliente-telefono").val("");
      $("#nuevo-cliente-fecha-nacimiento").val("");
      $("#nuevo-cliente-genero").val("");
      $("#mdl-nuevo-cliente-error").hide();

      $("#mdl-nuevo-cliente").dialog({
        modal: true,
        width: 650,
        title: "Registrar nuevo paciente",
        buttons: [
          {
            text: "Registrar paciente",
            class: "btn btn-success",
            click: function () {
              var nombre = $("#nuevo-cliente-nombre").val().trim();
              if (!nombre) {
                $("#mdl-nuevo-cliente-error-msg").text("El nombre del paciente es requerido.");
                $("#mdl-nuevo-cliente-error").show();
                return;
              }
              showLoading();
              $.ajax({
                method: "POST",
                url: "/Pacientes/RegistrarPacienteRapido",
                data: {
                  Nombre: nombre,
                  Nit: $("#nuevo-cliente-nit").val(),
                  Direccion: $("#nuevo-cliente-direccion").val(),
                  Telefono: $("#nuevo-cliente-telefono").val(),
                  FechaNacimiento: $("#nuevo-cliente-fecha-nacimiento").val(),
                  Genero: $("#nuevo-cliente-genero").val()
                },
                success: function (dataResult) {
                  hideLoading();
                  var data = JSON.parse(dataResult);
                  if (data.Exitoso) {
                    var nuevo = data.Resultado;
                    $("#ClienteId").val(nuevo.PacienteId);
                    var newOption = new Option(nuevo.Nombre, nuevo.PacienteId, true, true);
                    $("#select-cliente").append(newOption).trigger("change.select2");
                    $("#Nit").val(nuevo.Nit || "");
                    $("#Direccion").val(nuevo.Direccion || "");
                    $("#mdl-nuevo-cliente").dialog("close");
                    var msg = data.YaExistia
                      ? "El paciente ya existía en el sistema y fue seleccionado automáticamente."
                      : "Paciente registrado correctamente.";
                    mensajeEmergente(msg);
                  } else {
                    hideLoading();
                    $("#mdl-nuevo-cliente-error-msg").text(data.Mensaje);
                    $("#mdl-nuevo-cliente-error").show();
                  }
                },
                error: function () {
                  hideLoading();
                  $("#mdl-nuevo-cliente-error-msg").text("Error al comunicarse con el servidor.");
                  $("#mdl-nuevo-cliente-error").show();
                }
              });
            }
          },
          {
            text: "Cancelar",
            class: "btn btn-secondary",
            click: function () {
              $("#ClienteId").val("");
              $("#select-cliente").val(null).trigger("change.select2");
              $(this).dialog("close");
            }
          }
        ]
      });
    } else {
      var clienteId = data.id;
      $("#ClienteId").val(clienteId);
      $.ajax({
        url: "/Venta/ObtenerDatosCliente",
        method: "POST",
        dataType: "json",
        data: { clienteId: clienteId },
        success: function (resp) {
          if (resp.exitoso) {
            var d = resp.datos;
            $("#Nit").val(d.nit || "");
            $("#Direccion").val(d.direccion || "");
            $("#Correo").val(d.correo || "");
            $("#PacienteFechaNacimiento").val(
              d.fechaNacimiento ? d.fechaNacimiento.split("T")[0] : ""
            );
          } else {
            alert(resp.mensaje || "Cliente no encontrado");
          }
        },
        error: function () {
          alert("Error al obtener datos del cliente");
        }
      });
    }
  });

  $("#select-cliente").on("select2:clear", function () {
    $("#ClienteId").val("");
    $("#Nit").val("");
    $("#Direccion").val("");
    $("#Correo").val("");
    $("#PacienteFechaNacimiento").val("");
  });

  ventaVm.consultarProductosExistentes();
  ventaVm.consultarServiciosExistentes();
  ventaVm.consultarProductosPrescripcionConsulta();
  ventaVm.consultarProductosPrescripcionEmergencia();

  ventaVm.consultarExamenesAgregadosConsulta();

  ventaVm.consultarExamenesAgregadosEmergencias();

  /* ventaVm.consultarActivosCocentradosProductos(); */

  $("#SucursalId").on("change", function () {
    ventaVm.consultarProductosExistentes();
  });
  ventaVm.consultarExamenesExistentes();

  ventaVm.consultarServiciosConsulta(); //Servicios agregados en la consulta

  ventaVm.consultarServiciosEmergencias();

  setTimeout(function () {
    ventaVm.fixMissingProperties();
  }, 500);

  ventaVm.precargarClienteConsulta();

  $("#tabs-venta").tabs();
  $(".seccion-body").css("display", "none");
  $(".seccion-header").on("click", function () {
    var nombreSeccion = this.id.replace("-header", "");
    $("#" + nombreSeccion + "-body").slideToggle();
  });

  //$('#codigo-producto-buscar').keypress(function (e) {
  //    var keycode = e.which;
  //    if (keycode == '13') {
  //        ventaVm.buscarProductoCodigo();
  //    }
  //});
  //$('#codigo-nombre-producto-buscar').keypress(function (e) {
  //    var keycode = e.which;
  //    if (keycode == '13') {
  //        ventaVm.buscarProductoNombreCodigo();
  //    }
  //});

  //$('#codigo-servicio-buscar').keypress(function (e) {
  //    var keycode = e.which;
  //    if (keycode == '13') {
  //        ventaVm.buscarServicioCodigo();
  //    }
  //});

  //$('#codigo-examen-buscar').keypress(function (e) {
  //    var keycode = e.which;
  //    if (keycode == '13') {
  //        ventaVm.buscarExamenCodigo();
  //    }
  //});

  //$('#nombre-producto-buscar').keypress(function (e) {
  //    var keycode = e.which;
  //    if (keycode == '13') {
  //        ventaVm.buscarProductosNombre();
  //    }
  //});
});

function generarPdfExamenesLaboratorio(examenLaboratorioId, tipoPDF) {
  var pacienteId = $("#ClienteId").val();
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