var CompraVM = function () {
  var model = {};
  var itemProductosComprados = 0;
  var self = this;
  self.preciosExistentes = ko.observableArray();
  self.laboratorios = ko.observableArray();
  self.laboratorioSeleccionado = ko.observable();
  self.productosRegistrados = ko.observableArray();
  self.productoRegistradoSeleccionado = ko.observable();
  self.productosComprados = ko.observableArray();
  self.unidadesVenta = ko.observableArray();
  self.preciosVenta = ko.observableArray();
  self.sucursalesExistentes = ko.observableArray();
  self.bodegasExistentes = ko.observableArray();
  self.ubicaciones = ko.observableArray();
  self.lotesExistentes = ko.observableArray();
  self.totalCompra = ko.observable(0);

  //#region Variables Prorrateo
  var itemGastosProrrateo = 1;
  self.cambioMonedaNacional = ko.observable(0);
  self.totalGastosImportacion = ko.observable(0);
  self.gastosProrrateo = ko.observableArray();
  //#endregion

  self.productosStockBajo = ko.observableArray();

  self.tipoCompraId = ko.observable();
  self.proveedorId = ko.observable();
  self.infoProveedor = ko.observable({
    ProveedorPoliticasDevolucion: "No disponible",
    ProveedorNombre: "N/A",
  });
  self.proveedorPoliticasDevolucion = ko.observable();

  self.unidadesCompra = ko.observableArray([]);

  self.consultarInfoProveedor = function () {
    showLoading();
    $.ajax({
      url: "/Compra/ConsultarInfoProveedor",
      method: "POST",
      data: {
        proveedorNombre: $("#EncabezadoProveedor").val(),
      },
      success: function (result) {
        hideLoading();
        let data = JSON.parse(result);
        if (data.Exitoso) {
          self.infoProveedor(data.Resultado);
        } else {
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de servidor");
      },
    });
  };

  self.consultarLaboratorios = function () {
    $("#div-loading").show();
    $.ajax({
      method: "POST",
      url: "/Compra/ConsultarLaboratorios",
      success: function (data) {
        $("#div-loading").hide();
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.laboratorios.push({
            Id: null,
            NombreLaboratorioProducto: "Todos",
          });
          $(dataResult.Laboratorios).each(function (_, vl) {
            self.laboratorios.push(vl);
          });
        } else alert(dataResult.Mensaje);
      },
      error: function () {
        $("#div-loading").hide();
        alert("Error");
      },
    });
  };

  self.laboratorioSeleccionado.subscribe(function () {
    self.consultarProductosRegistrados();
  });

  self.consultarProductosRegistrados = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Compra/ConsultarProductosRegistrados",
      data: {
        laboratorioId:
          self.laboratorioSeleccionado() == null
            ? null
            : self.laboratorioSeleccionado().Id,
        ambienteId: $("#CompraAmbienteId").val(),
      },
      success: function (data) {
        hideLoading();
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.productosRegistrados(dataResult.ProductosRegistrados);
        } else alert(dataResult.Mensaje);
      },
      error: function () {
        hideLoading();
        alert("Error");
      },
    });
  };

  self.consultarProductosCompra = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Compra/ConsultarProductosCompra",
      data: { compraId: $("#CompraId").val() },
      success: function (data) {
        hideLoading();
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          $(dataResult.Resultado).each(function (idx, vl) {
            // 🔹 Aseguramos que cada producto tenga un Item único
            if (vl.Item === undefined || vl.Item === null) {
              vl.Item = idx;
            }
            itemProductosComprados = Math.max(itemProductosComprados, vl.Item);

            // Aseguramos observables
            vl.ProductoId = ko.observable(vl.ProductoId);
            vl.Cantidad = ko.observable(vl.Cantidad || 0);
            vl.PrecioCompra = ko.observable(vl.PrecioCompra || 0);
            vl.Total = ko.observable(
              parseFloat(vl.Cantidad()) * parseFloat(vl.PrecioCompra())
            );
            vl.ProveedorSugeridoPrecio = ko.observable(
              vl.ProveedorSugeridoPrecio || "No disponible"
            );
            vl.ProveedorSugeridoCredito = ko.observable(
              vl.ProveedorSugeridoCredito || "No disponible"
            );

            self.productosComprados.push(vl);
            self.calcularPrecios(vl);

            self.cargarConfiguracionProductoExistente(vl);
          });
        } else {
          alert(dataResult.Mensaje);
        }
      },
      error: function () {
        hideLoading();
        alert("Error");
      },
    });
  };

  self.consultarPreciosExistentes = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Compra/ConsultarPreciosExistentes",
      success: function (data) {
        hideLoading();
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.preciosExistentes(dataResult.Resultado);
        } else alert(dataResult.Mensaje);
      },
      error: function () {
        hideLoading();
        alert("Error");
      },
    });
  };

  self.consultarBodegasExistentes = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Compra/ConsultarBodegasExistentes",
      success: function (data) {
        hideLoading();
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.bodegasExistentes(dataResult.Resultado);
        } else alert(dataResult.Mensaje);
      },
      error: function () {
        hideLoading();
        alert("Error");
      },
    });
  };

  self.consultarSucursalesExistentes = function () {
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Compra/ConsultarSucursalesExistentes",
      success: function (data) {
        hideLoading();
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.sucursalesExistentes(dataResult.Resultado);
        } else alert(dataResult.Mensaje);
      },
      error: function () {
        hideLoading();
        alert("Error");
      },
    });
  };

  self.consultarUnidadesCompra = function (ProductoId) {
    console.log(ProductoId);
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Compra/ConsultarUnidadesCompra",
      data: {
        productoId: ProductoId,
      },
      success: function (data) {
        hideLoading();
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          self.unidadesCompra(dataResult.Resultado);
        } else alert(dataResult.Mensaje);
      },
      error: function () {
        hideLoading();
        alert("Error");
      },
    });
  };

  self.consultarUnidadesVenta = function (value) {
    var noItem = value.Item;
    var productoId = $("#producto-id-item-" + noItem).val();
    let unidadCompraId = $("#unidad-compra-item-" + noItem).val();
    var ambiente = $("#EncabezadoAmbienteId").val();

    $("#div-loading").show();
    $.ajax({
      method: "POST",
      url: "/Compra/ConsultarUnidadesVentaProducto",
      data: {
        productoId: productoId,
        unidadCompraId: unidadCompraId,
        bodegaId: ambiente,
      },
      success: function (data) {
        $("#div-loading").hide();
        var dataResult = JSON.parse(data);

        if (dataResult.Exitoso) {
          $(self.productosComprados()).each(function (_, vl1) {
            vl1.UnidadesVenta = [];
            $(dataResult.UnidadesVenta).each(function (_, unidad) {
              vl1.UnidadesVenta.push(unidad.Id);
            });
          });

          let unidadesExistentes = self.unidadesVenta();
          self.unidadesVenta([]);
          $(unidadesExistentes).each(function (_, vlUnidad) {
            if (vlUnidad.Item != noItem) {
              self.unidadesVenta.push(vlUnidad);
            }
          });

          let preciosExistentes = self.preciosVenta();
          self.preciosVenta([]);
          $(preciosExistentes).each(function (_, vlPrecio) {
            if (vlPrecio.Item != noItem) {
              self.preciosVenta.push(vlPrecio);
            }
          });

          $(dataResult.UnidadesVenta).each(function (_, vl) {
            let cantidadComprada = 0;
            if (!isNaN(value.Cantidad)) {
              cantidadComprada = value.Cantidad;
            }
            let TotalCompra = 0;
            if (!isNaN(value.Total)) {
              TotalCompra = value.Total;
            }

            let nuevaUnidadVenta = {
              Item: noItem,
              Id: vl.Id,
              Equivalencia: vl.Equivalencia,
              NombreUnidad: vl.NombreUnidad,
              CantidadEquivalenteDestino: vl.CantidadEquivalenteDestino,
              CantidadCalculada: ko.observable(
                vl.CantidadEquivalenteDestino * cantidadComprada
              ),
              precioUnidadCompra: ko.observable(0),
            };
            self.unidadesVenta.push(nuevaUnidadVenta);

            let productoPrecios = dataResult.ProductoPrecios;

            $(self.preciosExistentes()).each(function (_, precio) {
              let nuevoPrecioAgregado = {
                Item: noItem,
                UnidadMedidaVentaId: vl.Id,
                PrecioId: precio.PrecioId,
                PrecioNombre: precio.PrecioNombre,
                PrecioValor: ko.observable(0),
              };

              let matchedProductoPrecio = productoPrecios.find(
                (pp) => pp.PrecioId === precio.PrecioId
              );
              if (matchedProductoPrecio) {
                nuevoPrecioAgregado.PrecioValor(matchedProductoPrecio.Valor);
              }

              self.preciosVenta.push(nuevoPrecioAgregado);
            });

            $(self.bodegasExistentes()).each(function (_, bodega) {
              if (
                bodega.SucursalId == $("#EncabezadoSucursalId").val() &&
                bodega.AmbienteId == $("#EncabezadoAmbienteId").val()
              ) {
                let nuevaUbicacionAgregada = {
                  Item: noItem,
                  UnidadMedidaVentaId: vl.Id,
                  CantidadEquivalenteDestino: vl.CantidadEquivalenteDestino,
                  BodegaId: bodega.BodegaId,
                  NombreUbicacion: bodega.BodegaNombre,
                  Cantidad: ko.observable(0),
                };
                self.ubicaciones.push(nuevaUbicacionAgregada);
              }
            });
          });
        } else alert(dataResult.Mensaje);
      },
      error: function () {
        $("#div-loading").hide();
        alert("Error");
      },
    });
  };

  //Metodo para cuando cambia de sucursal o de ambiente
  self.actualizarUbicaciones = function () {
    self.ubicaciones([]);
    $(self.productosComprados()).each(function (_, vlProductoCompra) {
      $(self.unidadesVenta()).each(function (_, vl) {
        $(self.bodegasExistentes()).each(function (_, bodega) {
          if (
            bodega.SucursalId == $("#EncabezadoSucursalId").val() &&
            bodega.AmbienteId == $("#EncabezadoAmbienteId").val()
          ) {
            let nuevaUbicacionAgregada = {
              Item: vlProductoCompra.Item,
              UnidadMedidaVentaId: vl.Id,
              BodegaId: bodega.BodegaId,
              NombreUbicacion: bodega.BodegaNombre,
              Cantidad: ko.observable(0),
            };
            self.ubicaciones.push(nuevaUbicacionAgregada);
          }
        });
      });
    });
  };

  self.verConfiguracionInventario = function (itemCompra) {
    $("#configuracion-inventario-item-" + itemCompra.Item).slideToggle("fast");
  };

  self.agregarProductoDesdeServidor = function (producto) {
    console.log("Agregando producto desde servidor:", producto);

    var nuevoProducto = {
      Item: itemProductosComprados,
      ProductoId: ko.observable(producto.ProductoId),
      ProductoCodigo: ko.observable(producto.Codigo || "N/A"),
      NombreProducto: ko.observable(producto.Producto),
      Cantidad: ko.observable(producto.Cantidad || 1),
      PrecioCompra: ko.observable(producto.PrecioCompra || 0),
      Total: ko.observable(
        parseFloat(producto.Cantidad || 1) *
          parseFloat(producto.PrecioCompra || 0)
      ),
      Lote: ko.observable(producto.Lote || ""),
      FechaVencimiento: ko.observable(producto.FechaVencimiento || null),
      UnidadMedidaCompraId: ko.observable(1),
      PoliticaDevolucionProducto: ko.observable(
        producto.PoliticaDevolucionProducto || ""
      ),
      PoliticaDevolucionPersonalizadaDias: ko.observable(
        producto.PoliticaDevolucionPersonalizadaDias || 0
      ),
      PoliticaDevolucionVencimientoProducto: ko.observable(
        producto.PoliticaDevolucionVencimientoProducto || ""
      ),
      PoliticaDevolucionVencimientoPersonalizadaDias: ko.observable(
        producto.PoliticaDevolucionVencimientoPersonalizadaDias || 0
      ),
      CreditoProducto: ko.observable(producto.CreditoProducto || ""),
      CreditoPersonalizadoDias: ko.observable(
        producto.CreditoPersonalizadoDias || 0
      ),
      ProveedorSugeridoPrecio: ko.observable(
        producto.ProveedorSugeridoPrecio || "No disponible"
      ),
      ProveedorSugeridoCredito: ko.observable(
        producto.ProveedorSugeridoCredito || "No disponible"
      ),
      Nuevo: true,
    };

    self.productosComprados.push(nuevoProducto);
    itemProductosComprados++;
  };

  self.agregarProductoCompra = function (productoId) {
    if (!productoId) {
      mensajeEmergenteError("Producto no identificado");
      return false;
    }

    var productoSeleccionado = self.productosRegistrados().find(function (p) {
      return p.Id === productoId;
    });

    if (productoSeleccionado) {
      self.productoRegistradoSeleccionado().Id = productoSeleccionado.Id;
      self.productoRegistradoSeleccionado().ProductoCodigo =
        productoSeleccionado.ProductoCodigo;
      self.productoRegistradoSeleccionado().NombreProducto =
        productoSeleccionado.NombreProducto;
    }

    itemProductosComprados++;
    var ambienteId = $("#EncabezadoAmbienteId").val();

    if (!self.productoRegistradoSeleccionado()) {
      mensajeEmergenteError("No hay ningun producto seleccionado");
      return false;
    }

    showLoading();
    $.ajax({
      method: "POST",
      url: "/Compra/ConsultarUnidadesCompraProducto",
      data: {
        productoId: self.productoRegistradoSeleccionado().Id,
        bodegaId: ambienteId,
      },
      success: function (data) {
        hideLoading();
        var dataResult = JSON.parse(data);

        if (dataResult.Exitoso) {
          var productoComprado = {
            Item: itemProductosComprados,
            Lote: ko.observable(),
            ProductoCodigo:
              self.productoRegistradoSeleccionado().ProductoCodigo,
            ProductoId: self.productoRegistradoSeleccionado().Id,
            NombreProducto:
              self.productoRegistradoSeleccionado().NombreProducto,
            Cantidad: ko.observable(0),
            FechaVencimiento: ko.observable(null),
            CantidadCalculada: ko.observable(0),
            PrecioCompra: ko.observable(dataResult.UltimoPrecioCompra),
            Total: ko.observable(0),
            PoliticaDevolucionVencimientoProducto: ko.observable(
              "radio-politica-vencimiento-no-maneja"
            ),
            PoliticaDevolucionVencimientoPersonalizadaDias: ko.observable(0),
            PoliticaDevolucionProducto: ko.observable(
              "radio-politica-no-maneja"
            ),
            PoliticaDevolucionPersonalizadaDias: ko.observable(0),
            CreditoProducto: ko.observable("radio-credito-no-maneja"),
            CreditoPersonalizadoDias: ko.observable(0),
            Nuevo: true,
            StockActual: dataResult.stock,
            ProveedorSugeridoPrecio: dataResult.ProveedorSugeridoPrecio,
            ProveedorSugeridoCredito: dataResult.ProveedorSugeridoCredito,
          };

          self.productosComprados.push(productoComprado);

          $(dataResult.UnidadesCompra).each(function (_, vl) {
            $("#unidad-compra-item-" + itemProductosComprados).append(
              "<option value='" + vl.Id + "'>" + vl.NombreUnidad + "</option>"
            );
          });

          $(dataResult.LotesExistentes).each(function (_, vl) {
            vl.Item = itemProductosComprados;
            self.lotesExistentes.push(vl);
          });

          self.consultarUnidadesVenta(productoComprado);
        } else mensajeEmergenteError(dataResult.Mensaje);
      },
      error: function () {
        $("#div-loading").hide();
        alert("Error");
      },
    });

    mensajeEmergente("Producto agregado");
    self.productoRegistradoSeleccionado(null);
  };

  self.quitarProductoCompra = function (value) {
    $(self.productosComprados()).each(function (idx, producto) {
      if (producto.Item == value.Item) {
        self.productosComprados.splice(idx, 1);
      }
    });
  };

  self.calcularPrecios = function (value) {
    var cantidad = 0;
    var precioCompra = 0;

    if (
      !isNaN(value.Cantidad()) &&
      value.Cantidad() != null &&
      value.Cantidad() != undefined
    ) {
      cantidad = parseFloat(value.Cantidad());
    }

    precioCompra = parseFloat(value.PrecioCompra());

    if (cantidad == 0) {
      value.Total(0);
    } else {
      value.Total((cantidad * precioCompra).toFixed(2));
    }

    var totalCompra = 0;
    $(self.productosComprados()).each(function (_, vl) {
      totalCompra += parseFloat(vl.Total());
    });
    self.totalCompra(totalCompra.toFixed(2));

    // Cálculo de unidades finales para inventario
    $(self.productosComprados()).each(function (_, vl) {
      let cantidadComprada = 0;
      if (!isNaN(vl.Cantidad())) {
        cantidadComprada = vl.Cantidad();
      }

      $(self.unidadesVenta()).each(function (_, vlUnidad) {
        if (vlUnidad.Item == vl.Item) {
          let cantidadCalculada =
            parseFloat(vlUnidad.CantidadEquivalenteDestino) * cantidadComprada;

          vlUnidad.CantidadCalculada(cantidadCalculada);

          if (cantidadCalculada > 0) {
            var precioUnidadVenta = totalCompra / cantidadCalculada;
            precioUnidadVenta = parseFloat(precioUnidadVenta.toFixed(3));
            vlUnidad.precioUnidadCompra(precioUnidadVenta);
            $("#precioUnidadCompra").val(precioUnidadVenta);
          } else {
            vlUnidad.precioUnidadCompra(0);
            $("#precioUnidadCompra").val(0);
          }
        }
      });
    });
  };

  self.getModel = function () {
    var u = ko.unwrap || ko.utils.unwrapObservable;

    var model = {
      CompraId: $("#CompraId").val(),
      EncabezadoProveedor: $("#EncabezadoProveedor").val(),
      EncabezadoNoComprobante: $("#EncabezadoNoComprobante").val(),
      EncabezadoEmpleadoId: $("#EncabezadoEmpleadoId").val(),
      EncabezadoTipoBodegaId: $("#EncabezadoTipoBodegaId").val(),
      EncabezadoFechaRecepcion: $("#EncabezadoFechaRecepcion").val(),
      EncabezadoTipoDocumentoId: $("#EncabezadoTipoDocumentoId").val(),
      EncabezadoSucursalId: $("#EncabezadoSucursalId").val(),
      EncabezadoAmbienteId: $("#EncabezadoAmbienteId").val(),
      EncabezadoTipoCompraId: $("#EncabezadoTipoCompraId").val(),
      DiasCredito: $("#DiasCredito").val(),
      EncabezadoFechaLimite: $("#EncabezadoFechaLimite").val(),
      EncabezadoObservacion: $("#EncabezadoObservacion").val(),

      ValorTotalCompra: parseFloat(self.totalCompra() || 0),

      ProductosComprados: self.productosComprados().map(function (p) {
        return {
          ProductoId: u(p.ProductoId),
          ProductoCodigo: u(p.ProductoCodigo),
          NombreProducto: u(p.NombreProducto),
          Cantidad: Number(u(p.Cantidad) || 0),
          PrecioCompra: Number(u(p.PrecioCompra) || 0),
          Total: Number(u(p.Total) || 0),
          Lote: u(p.Lote),
          FechaVencimiento: u(p.FechaVencimiento),
          PoliticaDevolucionProducto: u(p.PoliticaDevolucionProducto),
          PoliticaDevolucionPersonalizadaDias: Number(
            u(p.PoliticaDevolucionPersonalizadaDias) || 0
          ),
          PoliticaDevolucionVencimientoProducto: u(
            p.PoliticaDevolucionVencimientoProducto
          ),
          PoliticaDevolucionVencimientoPersonalizadaDias: Number(
            u(p.PoliticaDevolucionVencimientoPersonalizadaDias) || 0
          ),
          CreditoProducto: u(p.CreditoProducto),
          CreditoPersonalizadoDias: Number(u(p.CreditoPersonalizadoDias) || 0),
          ProveedorSugeridoPrecio: u(p.ProveedorSugeridoPrecio),
          ProveedorSugeridoCredito: u(p.ProveedorSugeridoCredito),
          Nuevo: !!u(p.Nuevo),
        };
      }),
    };

    return model;
  };

  self.validateModel = function () {
    let tipoDocumentoId = $("#EncabezadoTipoDocumentoId").val();
    if (!tipoDocumentoId || tipoDocumentoId.trim() == "") {
      alert("Seleccione un tipo de documento");
      return false;
    }

    let sucursalId = $("#EncabezadoSucursalId").val();
    if (!sucursalId || sucursalId.trim() == "") {
      alert("Seleccione una sucursal");
      return false;
    }

    let tipoCompraId = $("#EncabezadoTipoCompraId").val();
    if (!tipoCompraId || tipoCompraId.trim() == "") {
      alert("Seleccione un tipo de compra");
      return false;
    }

    let ambienteId = $("#EncabezadoAmbienteId").val();
    if (!ambienteId || ambienteId.trim() == "") {
      alert("Seleccione un módulo");
      return false;
    }

    if (self.productosComprados().length == 0) {
      alert("No hay ningún producto agregado");
      return false;
    }

    var productosValidos = true;
    $(self.productosComprados()).each(function (_, vlProducto) {
      if (!vlProducto.Cantidad() || vlProducto.Cantidad() == 0) {
        alert("Proporcione una cantidad válida");
        productosValidos = false;
      }
    });

    if (!productosValidos) return false;

    return true;
  };

  self.guardarCompra = function () {
    if (confirm("¿Desea guardar este registro?")) {
      showLoading();
      var model = self.getModel();

      $.ajax({
        method: "POST",
        url: "/Compra/GuardarCompra",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(model),
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);

          if (data.Exitoso) {
            if (data.IsOrden) {
              var ambienteId = $("#EncabezadoAmbienteId").val();
              window.location.href =
                "/Compra/ListaPeticiones?ambienteId=" + ambienteId;
            } else if (data.IsCompra) {
              var tipoDocumentoId = $("#EncabezadoTipoDocumentoId").val();
              window.location.href =
                "/Compra/ListaComprados?tipoDocumentoId=" + tipoDocumentoId;
            } else {
              window.location.href = "/Compra/ListaComprados";
            }
          } else {
            alert(data.Mensaje || "No se pudo guardar.");
          }
        },
        error: function () {
          hideLoading();
          alert("Error en la petición");
        },
      });
    }
  };

  self.modificarOrdenCompra = function () {
    if (!confirm("¿Desea modificar esta orden?")) {
      return;
    }

    showLoading();
    var model = self.getModel();

    $.ajax({
      method: "POST",
      url: "/Compra/Modificar",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(model),
      success: function (dataResult) {
        hideLoading();

        var data =
          typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;

        if (data.Exitoso) {
          window.location.reload();
        } else {
          alert(data.Mensaje || "No se pudo modificar la orden.");
        }
      },
      error: function (xhr, status, error) {
        hideLoading();
        console.log("Error en Modificar:", status, error, xhr.responseText);
        alert("Error en la petición al modificar. Código HTTP: " + xhr.status);
      },
    });
  };

  self.cambiarAComprado = function () {
    var tipoDocumentoId = $("#EncabezadoTipoDocumentoId").val();

    // 🟢 Caso 2: COMPRA DIRECTA
    // Aquí NO tiene sentido pedir CompraId ni llamar a CambiarAComprado.
    // La forma correcta de "generar la compra" es usar GuardarCompra,
    // porque el backend ya agrega a inventario y maneja caja cuando el tipo es COMPRA.
    if (tipoDocumentoId == "2") {
      // Reutilizamos exactamente el flujo de guardar, que ya:
      // - envía el modelo completo
      // - llama a /Compra/GuardarCompra
      // - y en el backend genera la compra + inventario + caja
      self.guardarCompra();
      return;
    }

    // 🟠 Caso 1: ORDEN DE COMPRA
    // Aquí sí existe la orden en BD, sí tenemos CompraId,
    // y sí aplica llamar a CambiarAComprado(ordenCompraId)
    if (tipoDocumentoId == "1") {
      if (!confirm("¿Desea cambiar esta ORDEN a COMPRADO?")) {
        return;
      }

      showLoading();

      $.ajax({
        method: "POST",
        url: "/Compra/CambiarAComprado",
        data: {
          ordenCompraId: $("#CompraId").val(),
        },
        success: function (dataResult) {
          hideLoading();
          let data =
            typeof dataResult === "string"
              ? JSON.parse(dataResult)
              : dataResult;

          if (data.Exitoso) {
            window.location.href = "/Compra/ListaComprados";
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (xhr) {
          hideLoading();
          alert(
            "Error en la petición al cambiar a COMPRADO. Código HTTP: " +
              xhr.status
          );
        },
      });

      return;
    }

    // 🔴 Por si en el futuro aparece otro tipo de documento
    alert("Tipo de documento no soportado para cambio a COMPRADO.");
  };

  self.cancelarRegistroCompra = function () {
    if (confirm("¿Desea cancelar el registro de la compra?")) {
      const params = new URLSearchParams(window.location.search);
      const tipoDocumentoId = params.get("tipoDocumentoId");

      if (tipoDocumentoId === "1") {
        window.location.href = "/Compra/ListaPeticiones";
      } else if (tipoDocumentoId === "2") {
        window.location.href = "/Compra/ListaComprados";
      } else {
        window.location.href = "/Compra/ListaComprados";
      }
    }
  };

  //#region Funciones Prorrateo
  self.agregarGastoProrrateo = function () {
    let gasto = {
      Item: itemGastosProrrateo,
      Descripcion: null,
      Valor: ko.observable(0),
      EsDolar: ko.observable(false),
      ValorMonedaLocal: ko.observable(0),
      ValorDolares: ko.observable(0),
    };

    self.gastosProrrateo.push(gasto);
    $("#descripcion-gasto-prorrateo-item-" + itemGastosProrrateo).focus();
    itemGastosProrrateo++;
  };

  self.cambioMonedaNacional.subscribe(function () {
    self.actualizarTotalesProrrateo();
  });

  self.actualizarTotalesProrrateo = function () {
    let totalGastosImportacion = 0;

    $(self.gastosProrrateo()).each(function (_, vl) {
      if (!isNaN(vl.Valor())) {
        totalGastosImportacion += parseFloat(vl.Valor());

        if (vl.EsDolar()) {
          vl.ValorDolares(vl.Valor());
          vl.ValorMonedaLocal(
            (
              parseFloat(vl.Valor()) * parseFloat(self.cambioMonedaNacional())
            ).toFixed(2)
          );
        } else {
          vl.ValorMonedaLocal(vl.Valor());

          let cambioMonedaNacional = self.cambioMonedaNacional();
          if (isNaN(cambioMonedaNacional)) {
            cambioMonedaNacional = 0;
          }

          if (cambioMonedaNacional == 0) {
            vl.ValorDolares(0);
          } else {
            vl.ValorDolares(
              (
                parseFloat(vl.Valor()) / parseFloat(self.cambioMonedaNacional())
              ).toFixed(2)
            );
          }
        }
      }
    });

    self.totalGastosImportacion(totalGastosImportacion);
  };
  //#endregion

  self.actualizarTabla = function () {
    var tipoCompraId = $("#TipoCompraId").val();
    var proveedorId = $("#ProveedorId").val();

    showLoading();
    self.clearTableSugeridoCompra();

    $.ajax({
      url: "/Compra/ObtenerProductos",
      data: {
        tipoCompraId: tipoCompraId,
        proveedorId: proveedorId,
      },
      type: "GET",
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);

        if (data.Exitoso) {
          self.productosStockBajo(data.Resultado.$values);
          self.drawTableSugeridoCompra();
        } else {
          alert("Falla");
        }
      },
      error: function () {
        hideLoading();
        alert("Error al actualizar la tabla");
      },
    });
  };

  self.calcularYConcatenar = function (producto) {
    var resultado = "";
    producto.ProductoEquivalencias.$values.forEach(function (equivalencia) {
      var calculo = producto.Precio / equivalencia.CantidadEquivalente;
      calculo = parseFloat(calculo.toFixed(2));

      resultado +=
        "Q " + calculo + " - " + equivalencia.UnidadMedidaVentaNombre + "; ";
    });
    return resultado;
  };

  self.drawTableSugeridoCompra = function () {
    $("#tabla-sugerido-compra").DataTable({
      searching: true,
      ordering: true,
      paging: true,
      lengthMenu: [
        [5, 10, 25, 50, -1],
        ["5", "10", "25", "50", "Todos"],
      ],
      language: {
        search: "Buscar: ",
        lengthMenu: "Mostrar _MENU_ registros por página",
        zeroRecords: "No hay registros para mostrar",
        info: "Mostrando página _PAGE_ de _PAGES_",
        infoEmpty: "",
        infoFiltered: "(filtrado de _MAX_ registros totales)",
        paginate: {
          first: "Primero",
          last: "Último",
          previous: "Anterior",
          next: "Siguiente",
        },
      },
    });
  };

  self.clearTableSugeridoCompra = function () {
    var table = $("#tabla-sugerido-compra").DataTable();
    table.clear().draw();
    $("#tabla-sugerido-compra").dataTable().fnDestroy();
  };

  self.cargarConfiguracionProductoExistente = function (producto) {
    var u = ko.unwrap || ko.utils.unwrapObservable;
    var ambienteId = $("#EncabezadoAmbienteId").val();

    showLoading();
    $.ajax({
      method: "POST",
      url: "/Compra/ConsultarUnidadesCompraProducto",
      data: {
        productoId: u(producto.ProductoId),
        bodegaId: ambienteId,
      },
      success: function (data) {
        hideLoading();
        var dataResult = JSON.parse(data);
        if (dataResult.Exitoso) {
          // Unidades de compra
          $(dataResult.UnidadesCompra).each(function (idx, vl) {
            $("#unidad-compra-item-" + producto.Item).append(
              "<option value='" + vl.Id + "'>" + vl.NombreUnidad + "</option>"
            );
          });

          // Lotes existentes
          $(dataResult.LotesExistentes).each(function (idx, vl) {
            vl.Item = producto.Item;
            self.lotesExistentes.push(vl);
          });

          // Unidades de venta / precios / ubicaciones
          self.consultarUnidadesVenta(producto);
        } else {
          mensajeEmergenteError(dataResult.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        alert(data.error);
      },
    });
  };
}; // 🔥 CIERRE DEL CONSTRUCTOR CompraVM

// =======================
// INSTANCIA DEL VIEWMODEL
// =======================

var compraVM = new CompraVM();
ko.applyBindings(compraVM);

// =======================
// EVENTOS DOCUMENT READY
// =======================

$(function () {
  $("#tabs").tabs();

  compraVM.consultarInfoProveedor();

  $("#EncabezadoProveedor").on("change", function () {
    compraVM.consultarInfoProveedor();
  });

  $("#EncabezadoProveedor").on("change", function () {
    var nombreProveedor = $(this).val();

    if (nombreProveedor) {
      $.ajax({
        url: "/Proveedor/GetProveedorDetails",
        type: "GET",
        data: { nombreProveedor: nombreProveedor },
        success: function (response) {
          if (response.success) {
            $("#DiasCredito").val(response.diasCredito);
            $("#EncabezadoTipoCompraId")
              .val(response.tipoCompraId)
              .trigger("change");
          } else {
            alert("No se pudieron obtener los detalles del proveedor.");
          }
        },
      });
    }
  });

  $("#EncabezadoTipoCompraId").change(function () {
    var selectedValue = $(this).val();
    if (selectedValue == "2") {
      $(".DiasCredito").prop("disabled", false);
    } else {
      $(".DiasCredito").prop("disabled", true);
    }
  });

  $("#EncabezadoAmbienteId").on("change", function () {
    compraVM.actualizarUbicaciones();
  });

  $("#EncabezadoSucursalId").on("change", function () {
    compraVM.actualizarUbicaciones();
  });

  compraVM.consultarLaboratorios();
  compraVM.consultarProductosRegistrados();
  compraVM.consultarPreciosExistentes();
  compraVM.consultarSucursalesExistentes();
  compraVM.consultarBodegasExistentes();

  let compraId = $("#CompraId").val();
  if (compraId != null && compraId.trim() != "") {
    compraVM.consultarProductosCompra();
  }

  compraVM.actualizarTabla();
  drawDataTable("tabla-clinica-medicamentos");
});
